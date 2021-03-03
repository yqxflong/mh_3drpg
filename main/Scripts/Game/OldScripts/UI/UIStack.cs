///////////////////////////////////////////////////////////////////////
//
//  UIStack.cs
//
//  Copyright (c) 2006-2014 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ui堆栈
/// </summary>
public partial class UIStack : MonoBehaviour
{
    private class StackedItem
    {
        public int firstSortingOrder;
        public int lastSortingOrder;
    }

    private class IStackableWrapper : StackedItem
    {
        public IStackableUI stackable;
        public int firstStackDepth;
        public GameObject inputBlockerInstance;
    }

    public static UIStack Instance
    {
        get;
        private set;
    }

    public delegate void OnEnstack(IStackableUI stackable);

    public event OnEnstack onEnstack;

    public delegate void OnBackstack(IStackableUI stackable);

    public event OnBackstack onBackstack;

    public bool IsUIBlockingScreen
    {
        get { return _isFullScreenOpened || _isInputBlockerVisible; }
    }

    public bool IsUIFullScreenOpened
    {
        get { return _isFullScreenOpened; }
    }

    float _loadingMinTime = 1f;
    float _loadingStartTime = 0f;
    bool _isLoadingScreenUp = false;
    public bool IsLoadingScreenUp
    {
        get { return _isLoadingScreenUp; }
        private set
        {
            if (value)
            {
                _loadingStartTime = Time.time;
            }

            _isLoadingScreenUp = value;
        }
    }

    float _waittingStartTime = 0f;
    bool _isWaittingScreenUp = false;
    public bool IsWaittingScreenUp
    {
        get { return _isWaittingScreenUp; }
        private set
        {
            if (value)
            {
                _waittingStartTime = Time.time;
            }

            _isWaittingScreenUp = value;
        }
    }

    public GameObject inputBlockerPrefab;
    public GameObject fadeScreenPrefab;
    public string loadingScreenPrefabPath;
    public string dialoguePrefabPath;

    private GameObject _loadingScreen;
    private LoadingLogic _loadingScreen_logic = null;

    private EB.Collections.Queue<IStackableUI> _toEnstackWhenPossible = new EB.Collections.Queue<IStackableUI>();
    private IStackableUI _currentQueuedStackableDisplayed = null;

    private UICustomDialogue _dialogueInstance;

    private int _enstackFlag = 0;
    private int _backstackFlag = 0;
    private static int s_seed = 0;

    private EB.Collections.Stack<IStackableWrapper> _backStack = new EB.Collections.Stack<IStackableWrapper>();
    private EB.Collections.Stack<StackedItem> _fullStack = new EB.Collections.Stack<StackedItem>();
    private const int STACK_DEPTH_OFFSET = 10;
    private int _nextStackDepth = STACK_DEPTH_OFFSET;
    private bool _resetNextStackDepth = false;

    private bool _isFullScreenOpened = false;
    private bool _isInputBlockerVisible = false;
    private bool _interruptExitStack = false;

    private bool _exitConfirm = false;

    /// <summary>
    /// 常驻资源加载UIHelperPrefab
    /// </summary>
    /// <returns></returns>
	public void LoadUIHelper()
    {
		string path = "Assets/_GameAssets/Res/Prefabs/UIPrefabs/Tooltip/UIHelperPrefab";
		EB.Assets.LoadAsync(path, typeof(GameObject), o=>
		{
			if(o!=null)
			{
				var go = GameObject.Instantiate(o) as GameObject;
				OnLoadResult(path, go, true);
			}
		});
    }

    //资源加载是否成功的回调函数
    private void OnLoadResult(string assetname, GameObject go, bool bSuccessed)
    {
        EB.Debug.Log("[Asset]DownloadResult: assetname ={0},bSuccessed ={1}", assetname, bSuccessed);
		UIHierarchyHelper.Instance.Place(go, UIHierarchyHelper.eUIType.None, null);
		if (null != UIHierarchyHelper.Instance)
		{
			UIHierarchyHelper.Instance.SetBlockPanel(false);
		}
    }

    /// <summary>
    /// 获取通用提示框
    /// </summary>
    /// <returns></returns>
	public void GetDialog(string type, string body, OnUIDialogueButtonClick callBack)
    {

        if (_dialogueInstance == null || UIController.IsDestroyed(_dialogueInstance))
        {
            UIHierarchyHelper.Instance.LoadAndPlaceAsync(go =>
            {
                _dialogueInstance = go.GetComponent<UICustomDialogue>();
                dialogueInstanceFunc(type, body, callBack);

            }, dialoguePrefabPath, UIHierarchyHelper.eUIType.None, null, true);
        }
        else
        {
            dialogueInstanceFunc(type, body, callBack);
        }
    }

    private void dialogueInstanceFunc(string type, string body, OnUIDialogueButtonClick callBack)
    {
        switch (type)
        {
            case "Error":
                {
                    _dialogueInstance.Error(body, callBack);
                }; break;
            case "Confirm":
                {
                    _dialogueInstance.Confirm(body, callBack);
                }; break;
            case "Alert":
                {
                    _dialogueInstance.Alert(body, callBack);
                }; break;
            case "Tips":
                {
                    _dialogueInstance.Tips(body);
                }; break;
        }
    }

    /// <summary>
    /// 获取通用提示框并展示option中内容
    /// </summary>
    /// <param name="option"></param>
	public void ShowDialog(UICustomeDialogueOption option)
    {
        if (_dialogueInstance == null || UIController.IsDestroyed(_dialogueInstance))
        {
            UIHierarchyHelper.Instance.LoadAndPlaceAsync(go =>
            {
                _dialogueInstance = go.GetComponent<UICustomDialogue>();
                dialogueInstanceFunc(option);
            }, dialoguePrefabPath, UIHierarchyHelper.eUIType.None, null, true);
        }
        else
        {
            dialogueInstanceFunc(option);
        }
    }

    private void dialogueInstanceFunc(UICustomeDialogueOption option)
    {
        _dialogueInstance.Show(option);
    }

	#region ShowLoadingScreen
    private void ShowLoadingScreen_AfterFunc(bool showSplash, bool showBlock, bool showCloud, System.Action onReady)
    {
        System.Action newOnReady = delegate
        {
            if (_backStack.Count > 0 && !_backStack.Peek().stackable.CanAutoBackstack() && _backstackFlag == 0 && _enstackFlag == 0)
            {
                _backStack.Peek().stackable.OnBlur();
            }
            _nextStackDepth = STACK_DEPTH_OFFSET;

			onReady?.Invoke();
        };

        _loadingScreen_logic.SetLoadingScreen(showSplash, showBlock, showCloud, onReady);
    }

	///展示loading一次的计数器
	private int _ShowLoadingScreen_CounterOnce = 0;

    /// <summary>
    /// 显示loading界面
    /// </summary>
    /// <param name="onReady">回调</param>
    /// <param name="showSplash">显示loading的Label</param>
    /// <param name="showDownloading">显示下载的Label</param>
    /// <param name="showBlock">显示蒙版，防止点击</param>
    /// <param name="showUpdateAsk">显示下载请求</param>
    /// <param name="showCloud">显示云</param>
    public void ShowLoadingScreen(System.Action onReady, bool showSplash = true, bool showBlock = false, bool showCloud = false)
    {
		_ShowLoadingScreen_CounterOnce++;

        if (IsLoadingScreenUp)
        {
            EB.Debug.Log("loading界面正在显示,新的回调立即执行!");
			onReady?.Invoke();
			return;
        }

        EB.Debug.Log("loading界面开始=====>");
        FusionAudio.StopMusic();
		FusionAudio.StopAmbience();

		if (null == UIHierarchyHelper.Instance)
		{
			EB.Debug.LogWarning("ShowLoadingScreen: UIHierarchyHelper is null");
			return;
		}

		if (!UIHierarchyHelper.Instance.gameObject.activeSelf)
		{
			EB.Debug.LogWarning("ShowLoadingScreen: UIHierarchyHelper is disabled");
			return;
		}

		ClearEnstackQueue();

        IsLoadingScreenUp = true;
        RemoveTimer();

        if (_loadingScreen == null)
        {
            UIHierarchyHelper.Instance.LoadAndPlaceAsync(go =>
            {
                _loadingScreen = go;
				_loadingScreen_logic = go.GetComponent<LoadingLogic>();
                ShowLoadingScreen_AfterFunc(showSplash, showBlock, showCloud, onReady);
            }, loadingScreenPrefabPath, UIHierarchyHelper.eUIType.None, null, true);
        }
        else
        {
            ShowLoadingScreen_AfterFunc(showSplash, showBlock, showCloud, onReady);
        }
    }

    private int hideTimeoutTimer = 0;
    private bool[] hideParam = new bool[2];

	public void HideLoadingScreenImmediately(bool fade = true,bool WithCloud = false, System.Action fn = null)
	{
		EB.Debug.Log("<=====loading界面结束");
        if (!IsLoadingScreenUp)
        {
			fn?.Invoke();
            return;
        }
        RemoveTimer();

		_ShowLoadingScreen_CounterOnce = 0;
        if (_loadingScreen != null)
        {
            EB.Debug.Log("_loadingScreen!=null"); 
			_loadingScreen_logic.Fade = fade;
			_loadingScreen_logic.WithCloud = WithCloud;
			_loadingScreen_logic.SetProgressFinsh(f=>
            {
                StartCoroutine(WaitForFadeout(fade, fn));
            }, fade, true);
        }
	}

    /// <summary>
    /// 隐藏加载的加载界面?
    /// </summary>
    /// <param name="fade">就否淡出</param>
    /// <param name="WithCloud">就否以云特效结尾</param>
    public void HideLoadingScreen(bool fade = true,bool WithCloud = false)
    {
		EB.Debug.Log("<=====loading界面结束");
        if (!IsLoadingScreenUp)
        {
            return;
        }
        RemoveTimer();

        //引用计数-1
        _ShowLoadingScreen_CounterOnce = Mathf.Max(0, _ShowLoadingScreen_CounterOnce - 1);
		if(_ShowLoadingScreen_CounterOnce > 0)
		{
            hideParam[0] = fade;
            hideParam[1] = WithCloud;
            hideTimeoutTimer = TimerManager.instance.AddTimer(5000, 1, HideTimeout);
            EB.Debug.Log("_ShowLoadingScreen_CounterOnce = {0}", _ShowLoadingScreen_CounterOnce);
            return;
		}

        if (_loadingScreen != null)
        {
            EB.Debug.Log("_loadingScreen!=null"); 
			_loadingScreen_logic.Fade = fade;
			_loadingScreen_logic.WithCloud = WithCloud;
			_loadingScreen_logic.SetProgressFinsh(f=>
            {
                StartCoroutine(WaitForFadeout(fade));
            }, fade, false);
        }
    }
	
    private void HideTimeout(int timer)
    {
        hideTimeoutTimer = 0;
        HideLoadingScreen(hideParam[0], hideParam[1]);
    }

    private void RemoveTimer()
    {
        if (hideTimeoutTimer != 0)
        {
            TimerManager.instance.RemoveTimer(hideTimeoutTimer);
            hideTimeoutTimer = 0;
        }
    }

	public void ForceHideLoadingScreen(bool fade = true)
	{
		if (!IsLoadingScreenUp)
        {
            return;
        }
		_ShowLoadingScreen_CounterOnce = 0;
		if (_loadingScreen != null)
		{
            StartCoroutine(WaitForFadeout(fade));
        }
	}

	public void ShowLogoutScreen()
	{
		GameObject go = transform.Find("LogoutBGPanel").gameObject;
		if(go)go.SetActive(true);
	}

	public void HideLogoutScreen()
	{
		GameObject go = transform.Find("LogoutBGPanel").gameObject;
		if (go) go.SetActive(false);
	}
	#endregion

	/// <summary>
	/// Loading中等待其他玩家加载的机制
	/// </summary>
	public void WaitForOtherPlayer()
    {
        if (_loadingScreen != null)
        {
            _loadingScreen_logic.SetWaitForPlayer();
        }
    }

    IEnumerator WaitForFadeout(bool fade, System.Action fn = null)
    {
        EnstackQueue();
        if (_loadingScreen != null)
        {
            EB.Debug.Log("WaitForFadeout!");
            yield return _loadingScreen_logic.OnRemoveFromStack();
			GameObject.Destroy(_loadingScreen);
            _loadingScreen = null;
			_loadingScreen_logic = null;
        }
        EB.Debug.Log("WaitForFadeout——Finish!");
		IsLoadingScreenUp = false;
        yield return null;
        fn?.Invoke();
    }
    
    /// <summary>
    /// ui入栈
    /// </summary>
    /// <param name="stackable"></param>
    /// <param name="queued"></param>
    /// <param name="manualDepth"></param>
	public void EnStack(IStackableUI stackable, bool queued = false)
	{
        if (!queued || CanDisplayNextOnQueue())
		{
            StartCoroutine(EnStackCoroutine(stackable));
		}
		else
		{
            stackable.Show(false);
			_toEnstackWhenPossible.Enqueue(stackable);
		}
	}
    
	/// <summary>
    /// 入栈的协程
    /// </summary>
    /// <param name="stackable"></param>
    /// <param name="manualDepth"></param>
    /// <returns></returns>
	private IEnumerator EnStackCoroutine(IStackableUI stackable)
	{
        if (stackable != null && !stackable.Equals(null))
		{
			// disable Main Camera
			if (stackable.IsFullscreen())
			{
				_isFullScreenOpened = true;

				GameUtils.SetMainCameraActive(stackable.IsRenderingWorldWhileFullscreen());
			}

			// OnBlur Event on top ui
			if (_backStack.Count > 0)
			{
				if (_enstackFlag == 0 && _backstackFlag == 0)
                {
                    _backStack.Peek().stackable.OnBlur();
				}
			}

			_enstackFlag = 0;
            _backstackFlag = 0;

			// Create wrapper
			IStackableWrapper wrapper = new IStackableWrapper();
			wrapper.stackable = stackable;

			// Insert an input blocker if needed
			if (stackable.ShowUIBlocker)
			{
				_isInputBlockerVisible = true;
				wrapper.inputBlockerInstance = GameObject.Instantiate(inputBlockerPrefab);
				wrapper.inputBlockerInstance.name = string.Format("InputBlockerFor{0}", stackable.ToString());
				wrapper.inputBlockerInstance.transform.SetParent(gameObject.transform);
				wrapper.inputBlockerInstance.transform.localPosition = Vector3.zero;
				wrapper.inputBlockerInstance.transform.localScale = Vector3.one;
				wrapper.inputBlockerInstance.GetComponentInChildren<UISprite>().alpha = 0.05f;
				TweenAlpha ta = wrapper.inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
				ta.tweenFactor = 1.0f;  //0.0f
				ta.PlayForward();
			}

			// Warn other screens that we're about to stack someone
			if (onEnstack != null)
			{
				onEnstack(stackable);
			}

            _nextStackDepth = AssignDepths(wrapper);

            // Hide below, set visible variables
            EB.Collections.Stack<IStackableWrapper> tempStack = new EB.Collections.Stack<IStackableWrapper>();
			while (_backStack.Count > 0)
			{
				tempStack.Push(_backStack.Pop());

				if (stackable.IsFullscreen())
				{
					tempStack.Peek().stackable.Show(false);
				}
				if (tempStack.Peek().inputBlockerInstance != null && (stackable.ShowUIBlocker || stackable.IsFullscreen()))
				{
					if (stackable.IsFullscreen() && wrapper.inputBlockerInstance == null)
					{
						_isInputBlockerVisible = false;
					}
					
					// hide blocker for next frame
					var tempPanel = tempStack.Peek().inputBlockerInstance.GetComponent<UIPanel>();
					if (wrapper.inputBlockerInstance && wrapper.inputBlockerInstance.name.Contains("DataPanelNew"))
					{
						tempPanel.alpha = 0.0f;
					}
					else
					{
						EB.Coroutines.EndOfFrame(delegate ()
						{
							tempPanel.alpha = 0.0f;
						});
					}
				}
			}

			while (tempStack.Count > 0)
			{
				_backStack.Push(tempStack.Pop());
			}

			// Fix again
			if (_isInputBlockerVisible && stackable.IsFullscreen() && wrapper.inputBlockerInstance == null)
			{
				_isInputBlockerVisible = false;
			}

			// Place stackable
			_backStack.Push(wrapper);
			_fullStack.Push(wrapper);

			// OnFocus Event
			_enstackFlag = ++s_seed;
			int currentFlag = _enstackFlag;
			yield return StartCoroutine(stackable.OnAddToStack());

            if (_enstackFlag == currentFlag && !IsLoadingScreenUp)
            {
				_enstackFlag = 0;
                stackable.OnFocus();
			}
		}
	}

    public void TopUIOnFocus()
    {
        if (_backStack.Count > 0)
        {
            _enstackFlag = 0;
            _backStack.Peek().stackable.OnFocus();
        }
    }

    /// <summary>
    /// ui出栈，移除栈顶的ui
    /// </summary>
	public void BackStack()
	{
		StartCoroutine(BackstackCoroutine(false));
	}

    /// <summary>
    /// 执行ui出栈操作，移除栈顶ui
    /// </summary>
    /// <param name="isPartOfExitStack">是否是退出状态的一部分，从而决定是否做些uicontroller中的一些逻辑或回调</param>
    /// <returns></returns>
	private IEnumerator BackstackCoroutine(bool isPartOfExitStack)
	{
		if (_backStack.Count == 0)
		{
			EB.Debug.LogWarning("BackstackCoroutine: backStack is empty");
			yield break;
		}

		IStackableWrapper wrapper = _backStack.Pop();
        EB.Debug.LogUI("界面UIStack:【<color=#00ff00>{0}</color>】在<color=#fff348>UIStack</color>中<color=#ff0000>从栈顶出栈</color>", wrapper.stackable);

        // Clear eventual stacked renderers from the full stack
        while (_fullStack.Pop() != wrapper) ;

		if (onBackstack != null)
		{
			onBackstack(wrapper.stackable);
		}

		if (wrapper.inputBlockerInstance != null)
		{
			// Value might be changed later depending on whether we find another input blocker before the next panel
			_isInputBlockerVisible = false;
			TweenAlpha ta = wrapper.inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
			ta.tweenFactor = 1.0f;
			EventDelegate.Add(ta.onFinished, () => Destroy(wrapper.inputBlockerInstance));
			ta.PlayReverse();
		}

		if (wrapper.firstStackDepth >= 0&& wrapper.firstStackDepth< _nextStackDepth)
		{
            _nextStackDepth = wrapper.firstStackDepth;
		}

		// Let the other windows know what happened, show what needs to be shown, etc.
		if (_backStack.Count > 0)
		{
			EB.Collections.Stack<IStackableWrapper> tempStack = new EB.Collections.Stack<IStackableWrapper>();
			while (_backStack.Count > 0 && !_backStack.Peek().stackable.IsFullscreen())
			{
				tempStack.Push(_backStack.Pop());

				if (wrapper.stackable.IsFullscreen())
				{
					tempStack.Peek().stackable.Show(true);
				}
				if (tempStack.Peek().inputBlockerInstance != null && !_isInputBlockerVisible)
				{
					// show top blocker
					//TweenAlpha tempTa = tempStack.Peek().inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
					_isInputBlockerVisible = true;
					tempStack.Peek().inputBlockerInstance.GetComponent<UIPanel>().alpha = 1f;// tempTa.to;

					// update tween parameters
					if (wrapper.inputBlockerInstance != null)
					{
						TweenAlpha ta = wrapper.inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
						ta.tweenFactor = 0.0f;
						wrapper.inputBlockerInstance.GetComponentInChildren<UISprite>().alpha = 0.05f;
					}
				}
			}

			if (wrapper.stackable.IsFullscreen())
			{
				// If the count is positive, it means that we've hit a full screen
				if (_backStack.Count > 0)
				{
					// show top full screen
					_backStack.Peek().stackable.Show(true);
					if (_backStack.Peek().inputBlockerInstance != null && !_isInputBlockerVisible)
					{
						// show top blocker
						TweenAlpha tempTa = tempStack.Peek().inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
						_isInputBlockerVisible = true;
						_backStack.Peek().inputBlockerInstance.GetComponent<UIPanel>().alpha = tempTa.to;

						// update tween parameters
						if (wrapper.inputBlockerInstance != null)
						{
							TweenAlpha ta = wrapper.inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
							ta.tweenFactor = 0.0f;
							wrapper.inputBlockerInstance.GetComponentInChildren<UISprite>().alpha = 0.05f;
						}
					}

					GameUtils.SetMainCameraActive(_backStack.Peek().stackable.IsRenderingWorldWhileFullscreen());
				}
				else
				{
					// No full screen, show the HUD
					_isFullScreenOpened = false;
				}
			}

			while (tempStack.Count > 0)
			{
				_backStack.Push(tempStack.Pop());
			}
		}
		else
		{
			// There's nothing in the stack, so no full screen, no blocker
			_isFullScreenOpened = false;
			_isInputBlockerVisible = false;
		}

		if (!_isFullScreenOpened && !IsLoadingScreenUp)
		{
			GameUtils.SetMainCameraActive(true);
		}

		if (wrapper.stackable == _currentQueuedStackableDisplayed)
		{
			_currentQueuedStackableDisplayed = null;
		}

		if (_enstackFlag == 0 && _backstackFlag == 0 && !isPartOfExitStack)
		{
            wrapper.stackable.OnBlur();
		}

		_enstackFlag = 0;
        _backstackFlag = 0;

		_backstackFlag = ++s_seed;
		var currentFlag = _backstackFlag;

        wrapper.stackable.ClearData();
        yield return StartCoroutine(wrapper.stackable.OnRemoveFromStack());

		if (currentFlag == _backstackFlag)
		{
			_backstackFlag = 0;

			if (_backStack.Count > 0 && !isPartOfExitStack)
			{
				//EB.Debug.LogWarning("Backstack: finish {0} onfocus", backStack.Peek().stackable);
				_backStack.Peek().stackable.OnFocus();
			}
			else
			{
				//EB.Debug.LogWarning("Backstack: finish");
			}

			if (!isPartOfExitStack)
			{
				OnFinishedBackstacking();
			}
		}
	}

    /// <summary>
    /// 截断退出栈的操作
    /// </summary>
	public void InterruptExitStack()
	{
		_interruptExitStack = true;
	}

    /// <summary>
    /// 对所有ui进行出栈处理
    /// </summary>
    /// <param name="forceClose"></param>
	public void ExitStack(bool forceClose = false)
	{
		// Reset the interrupt flag
		_interruptExitStack = false;

		// top blur & non loading screen object OnBlur
		if (!_interruptExitStack && _backStack.Count > 0 && (forceClose || _backStack.Peek().stackable.CanAutoBackstack()) && _enstackFlag == 0 && _backstackFlag == 0)
		{
            if (_loadingScreen == null)
            {
                _backStack.Peek().stackable.OnBlur();
            }
        }

		while (!_interruptExitStack && _backStack.Count > 0 && (forceClose || _backStack.Peek().stackable.CanAutoBackstack()))
		{
            if (_loadingScreen != null)
            {
                InterruptExitStack();
            }
            else
            {
                StartCoroutine(BackstackCoroutine(true));
            }
        }

		// Reset the interrupt flag, in case it was used
		_interruptExitStack = false;

		// Notify the previews stack object that it will be shown.
		// If there are no other objects, open the HUD
		if (_backStack.Count > 0)
		{
			_backStack.Peek().stackable.OnFocus();
		}
		else
		{
			
		}

		OnFinishedBackstacking();
	}
    /// <summary>
    /// 清除队列中的ui
    /// </summary>
	public void ClearEnstackQueue()
	{
		while (_toEnstackWhenPossible.Count > 0)
		{
			IStackableUI stackable = _toEnstackWhenPossible.Dequeue();
			if (stackable is Component)
			{
                Component c = (Component)stackable;
                if (c != null && c.transform.parent != null)
                {
					c.gameObject.CustomSetActive(false);
                    Destroy(c.gameObject);
                }
			}
		}
	}
    
    /// <summary>
    /// 获取最大的SortingOrder层级
    /// </summary>
	public int GetHighestSortingOrder()
	{
		int result = 0;

		if (_fullStack.Count > 0)
		{
            result = _fullStack.Peek().lastSortingOrder + 10;  //弹窗这里的排序还是加上10才保险，可以预留10个深度来用
        }
		return Mathf.Clamp(result + 1, 10, int.MaxValue);
	}

    /// <summary>
    /// 重新计算深度值
    /// </summary>
    public void ResetNextStackDepth()
    {
        _resetNextStackDepth = true;
    }

    /// <summary>
    /// 分配的UI深度
    /// </summary>
	private int AssignDepths(IStackableWrapper wrapper)
	{
		int stackDepth = _nextStackDepth;

        int originStartSortingOrder = wrapper.lastSortingOrder = wrapper.firstSortingOrder = GetHighestSortingOrder();
		if (wrapper.stackable is Component)
		{
			List<UIPanel> panels = new List<UIPanel>();
			List<Renderer> renderers = new List<Renderer>();

			panels.AddRange(((Component)wrapper.stackable).GetComponentsInChildren<UIPanel>(true));
			renderers.AddRange(((Component)wrapper.stackable).GetComponentsInChildren<Renderer>(true));

			panels.Sort((UIPanel panel1, UIPanel panel2) => panel1.sortingOrder != panel2.sortingOrder ? panel1.sortingOrder - panel2.sortingOrder : panel1.depth - panel2.depth);
			renderers.Sort((Renderer r1, Renderer r2) => r1.sortingOrder - r2.sortingOrder);

			if (wrapper.inputBlockerInstance != null)
			{
				if (panels.Count == 0)
				{
					EB.Debug.LogWarning("AssignDepths: need panel above blocker on {0}", wrapper.stackable);
					UIPanel uiPanel = ((Component)wrapper.stackable).gameObject.AddComponent<UIPanel>();
					panels.Add(uiPanel);
				}

				UIPanel blockerPanel = wrapper.inputBlockerInstance.GetComponent<UIPanel>();
				blockerPanel.depth = panels[0].depth - 1;
				blockerPanel.sortingOrder = panels[0].sortingOrder;
				panels.Insert(0, blockerPanel);
			}

			if (panels.Count > 0)
			{
				int currentOffset = 0;
				int previousDepth = panels[0].depth;

				panels[0].renderQueue = UIPanel.RenderQueue.StartAt;

				panels[0].depth = stackDepth;

				wrapper.firstStackDepth = panels[0].depth;
                
				int previousSortingOrder = originStartSortingOrder = panels[0].sortingOrder;
				panels[0].sortingOrder = wrapper.firstSortingOrder;
				int sortingOrder = panels[0].sortingOrder;
                for (int i = 1; i < panels.Count; i++)
				{
					int currentSortingOrderOffset = panels[i].sortingOrder - previousSortingOrder;
					previousSortingOrder = panels[i].sortingOrder;
					panels[i].sortingOrder = sortingOrder + currentSortingOrderOffset;
					sortingOrder = panels[i].sortingOrder;
					wrapper.lastSortingOrder = Mathf.Max(sortingOrder, wrapper.lastSortingOrder);
					if (currentSortingOrderOffset > 0)
					{
						previousDepth = panels[i].depth;
						panels[i].depth = stackDepth + currentSortingOrderOffset + 100;
						stackDepth = panels[i].depth;
					}
					else
					{
						currentOffset = panels[i].depth - previousDepth;
						previousDepth = panels[i].depth;
						panels[i].depth = stackDepth + currentOffset;
						stackDepth = panels[i].depth;
					}
                }

				panels[0].SetDirty();
			}
			else
			{
				wrapper.firstStackDepth = stackDepth;
			}

			if (renderers.Count > 0)
			{
				int previousSortingOrder = originStartSortingOrder;
				int sortingOrder = wrapper.firstSortingOrder;

				List<Material> materials = new List<Material>();
				for (int i = 0; i < renderers.Count; i++)
				{
					if (renderers[i].materials != null)
					{
						materials.AddRange(renderers[i].materials);
					}
                    
					SetSortingOrder sso = renderers[i].GetComponent<SetSortingOrder>();
                    int currentSortingOrder = sso != null ? sso.SortingOrder : 0;
					int currentSortingOrderOffset = currentSortingOrder - previousSortingOrder;
					previousSortingOrder = currentSortingOrder;
					sortingOrder = sortingOrder + currentSortingOrderOffset;
					if (sso != null)
					{
						sso.SortingOrder = sortingOrder;
					}
					wrapper.lastSortingOrder = Mathf.Max(sortingOrder, wrapper.lastSortingOrder);
                }

				if (materials.Count > 0)
				{
					materials.Sort((Material m1, Material m2) => m1.renderQueue - m2.renderQueue);
				}
			}
		}
		else
		{
			wrapper.firstStackDepth = stackDepth;
		}

        stackDepth += 10;
        return stackDepth;
    }

    /// <summary>
    /// ui完成了出栈操作
    /// </summary>
	private void OnFinishedBackstacking()
	{
		if (_toEnstackWhenPossible.Count > 0 && CanDisplayNextOnQueue())
		{
			_currentQueuedStackableDisplayed = _toEnstackWhenPossible.Dequeue();
			EnStack(_currentQueuedStackableDisplayed, false);
		}
	}

	private bool CanDisplayNextOnQueue()
	{
		if (_isFullScreenOpened)
		{
			return false;
		}

		if (System.Object.ReferenceEquals(_currentQueuedStackableDisplayed, null))
		{
			return true;
		}

		return _currentQueuedStackableDisplayed.Equals(null);
	}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

	void Update()
    {
        if (IsLoadingScreenUp || Hotfix_LT.UI.LTCombatEventReceiver.Instance!=null && Hotfix_LT.UI.LTCombatEventReceiver.Instance.spawnCameraing) return;
        if (Input.GetKeyDown(KeyCode.Escape))
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			// Escape to None for android back button, keep touch scheme on android device
			if (UICamera.mainCamera != null)
			{
				UICamera mainUICamera = UICamera.mainCamera.GetComponent<UICamera>();
				if (mainUICamera.useTouch)
				{
					UICamera.currentKey = KeyCode.None;
					UICamera.currentScheme = UICamera.ControlScheme.Touch;
				}
			}
#endif
            if (GameFlowControlManager.IsInView())
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "OnCancelButtonClick");
            }
            else   if (!_exitConfirm)
			{
				_exitConfirm = true;
				OnExiting(ExitingResult);
			}
		}
	}

	private void OnExiting(System.Action<bool> result)
	{
#if USE_QIHOOSDK
		if (SparxHub.Instance != null)
		{
			SparxHub.Instance.QiHooSDKManager.QuitSDK(result);
		}
#elif USE_OPPOSDK
		if (SparxHub.Instance != null)
		{
			SparxHub.Instance.OPPOSDKManager.ExitSDK(result);
		}
#elif USE_UCSDK
		if (SparxHub.Instance != null)
		{
			SparxHub.Instance.UCSDKManager.ExitSDK(result);
		}
#elif USE_WINNERSDK
		DefaultExit(delegate(bool quit)
		{
			if (quit && SparxHub.Instance != null)
			{
				SparxHub.Instance.WinnerSDKManager.ExitSDK(null);
			}
			result(quit);
		});
#elif USE_VIVOSDK
		DefaultExit(delegate (bool quit)
		{
			if (quit && SparxHub.Instance != null)
			{
				SparxHub.Instance.VivoSDKManager.ExitSDK();
			}
			result(quit);
		});
#elif USE_YIJIESDK
        if (SparxHub.Instance != null)
        {
            SparxHub.Instance.YiJieSDKManager.ExitSDK(delegate(int code) 
            {
                if (code == EB.Sparx.YiJieSDKConfig.EXIT_SUCCESS)
                {
                    result(true);
                }
                else if (code == EB.Sparx.YiJieSDKConfig.EXIT_CANCEL)
                {
                    result(false);
                }
                else if (code == EB.Sparx.YiJieSDKConfig.EXIT_NOT_EXIST)
                {
                    DefaultExit(result);
                }
            });
        }
#elif USE_EWANSDK
		if (SparxHub.Instance != null)
		{
			SparxHub.Instance.EWanSDKManager.ExitSDK(delegate (int code)
			{
				if (code == EB.Sparx.EWanStatusCode.GamePopExitDialog)
				{
					EB.Debug.Log("unity sdk GamePopExitDialog");
					DefaultExit(delegate (bool quit)
					{
						result(quit);
						if (quit && SparxHub.Instance != null)
						{
							SparxHub.Instance.EWanSDKManager.DestroySDK();
						}
					});
				}
				else if (code == EB.Sparx.EWanStatusCode.GameExit)
				{
					EB.Debug.Log("unity sdk GameExit");
					result(true);
				}
			});
		}
#elif USE_ASDK
		if (SparxHub.Instance != null)
		{
            if (EB.Sparx.Hub.Instance.ASDKManager.DefaultFunc==null)EB.Sparx.Hub.Instance.ASDKManager.DefaultFunc = DefaultExit;
            UnityEngine.Debug.Log("USE_ASDK sdk GameExit");
            _exitConfirm = false;
            SparxHub.Instance.ASDKManager.ExitSDK(result);
		}
#elif USE_M4399SDK
        if (EB.Sparx.Hub.Instance != null)
        {
            EB.Sparx.Hub.Instance.M4399SDKManager.Quit(result);
        }
#elif USE_VFPKSDK
        if (SparxHub.Instance != null)
        {
            _exitConfirm = false;
            SparxHub.Instance.VFPKSDKManager.Exit();
        }
#elif USE_XINKUAISDK   //已改
       if (SparxHub.Instance != null)
		{
			_exitConfirm = false;
			SparxHub.Instance.mBaseSdkManager.Exit();
		}
#else
        DefaultExit(result);
#endif
    }
    /// <summary>
    /// 目前无参用于asdk中的退出响应，当asdk表示使用我们游戏自带的退出框时，引用该方法
    /// </summary>
    public void DefaultExit()
    {
        _exitConfirm = true;
        DefaultExit(ExitingResult);
    }

    /// <summary>
    /// 游戏自带退出方法
    /// </summary>
    /// <param name="result"></param>
    private void DefaultExit(System.Action<bool> result)
	{
		GetDialog("Confirm",EB.Localizer.GetString("ID_SPARX_APP_EXIT_CONFIRM"), delegate (eUIDialogueButtons button, UIDialogeOption option)
		{
			result(button == eUIDialogueButtons.Accept);
		});
	}
    /// <summary>
    /// 退出回调
    /// </summary>
    /// <param name="quit"></param>
	private void ExitingResult(bool quit)
	{
		if (quit)
		{
#if !UNITY_EDITOR
			Application.Quit();
#else
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}
		else
		{
			_exitConfirm = false;
		}
	}

    /// <summary>
    /// 获取ui集合（包含预设、controller、深度等信息）
    /// </summary>
    /// <param name="ui">UIController</param>
    /// <returns></returns>
	private IStackableWrapper GetWrapper(IStackableUI ui)
	{
		return _backStack.Where(m => m.stackable == ui).FirstOrDefault();
	}

    /// <summary>
    /// 获取黑色遮罩背景
    /// </summary>
    /// <param name="ui"></param>
    /// <returns></returns>
	public GameObject GetPanelBlocker(IStackableUI ui)
	{
		var wrapper = GetWrapper(ui);
		if (wrapper == null)
		{
			return null;
		}

		return wrapper.inputBlockerInstance;
	}

    /// <summary>
    /// 获取顶层的SortingOrder
    /// </summary>
    /// <param name="ui"></param>
    /// <returns></returns>
	public int GetTopSortingOrder(IStackableUI ui)
	{
		var wrapper = GetWrapper(ui);
		if (wrapper == null)
		{
			return 0;
		}

		return wrapper.lastSortingOrder;
	}

	public bool IsStacked(IStackableUI ui)
	{
		return GetWrapper(ui) != null;
	}

	public bool IsQueued(IStackableUI ui)
	{
		return _toEnstackWhenPossible.Where(m => m == ui).FirstOrDefault() != null;
	}

    /// <summary>
    /// 是否在栈顶
    /// </summary>
    /// <param name="ui"></param>
    /// <returns></returns>
	public bool IsTop(IStackableUI ui)
	{
		return _backStack.Count > 0 && _backStack.Peek().stackable == ui;
	}

    /// <summary>
    /// 把ui移动到栈顶（注：该方法是把目标ui上面的所有ui都从栈中移除掉来达到效果的）
    /// </summary>
    /// <param name="ui"></param>
	public void MoveToTop(IStackableUI ui)
	{
		IStackableUI[] stack = _backStack.Select(b => b.stackable).ToArray();
		int index = System.Array.FindIndex(stack, m => m == ui);

        //把目标ui上面的ui都移除出栈
		for (int i = stack.Length - 1; i > index; --i)
		{
			Instance.BackStack();
		}

		var top = _backStack.Peek() as IStackableWrapper;
	}

    /// <summary>
    /// 让指定的ui出栈
    /// </summary>
    /// <param name="ui"></param>
	public void Destack(IStackableUI ui)
	{
		if (IsTop(ui))
		{
			BackStack();
		}
		else
		{
			StartCoroutine(Remove(ui));
		}
	}
    /// <summary>
    /// 让指定的ui队列中移除
    /// </summary>
    /// <param name="ui"></param>
	public void Dequeue(IStackableUI ui)
	{
		EB.Collections.Queue<IStackableUI> tmpQueue = new EB.Collections.Queue<IStackableUI>();

		while (_toEnstackWhenPossible.Count > 0)
		{
			IStackableUI q = _toEnstackWhenPossible.Dequeue();
			if (q == ui)
			{
				break;
			}

			tmpQueue.Enqueue(q);
		}

		while (_toEnstackWhenPossible.Count > 0)
		{
			tmpQueue.Enqueue(_toEnstackWhenPossible.Dequeue());
		}

		_toEnstackWhenPossible = tmpQueue;
	}

    /// <summary>
    /// 从队列中让ui出队列入栈
    /// </summary>
	public void EnstackQueue()
	{
        if (_toEnstackWhenPossible.Count == 0) TopUIOnFocus();
		while (_toEnstackWhenPossible.Count > 0)
		{
			_currentQueuedStackableDisplayed = _toEnstackWhenPossible.Dequeue();
			EnStack(_currentQueuedStackableDisplayed, false);
			_currentQueuedStackableDisplayed = null;
		}
	}

    /// <summary>
    /// 把ui从堆栈中移除（可以不是栈顶的ui）
    /// </summary>
    /// <param name="ui"></param>
    /// <returns></returns>
	private IEnumerator Remove(IStackableUI ui)
	{
        EB.Debug.LogUI("界面UIStack:【<color=#00ff00>{0}</color>】在<color=#fff348>UIStack</color>中<color=#ff0000>从非栈顶出栈</color>，调用Remove方法", ui);
        // assume invisible first, update later
        _isInputBlockerVisible = false;
		_isFullScreenOpened = false;

		// remove from backStack
		IStackableWrapper wrapper = null;
		EB.Collections.Stack<IStackableWrapper> tmp = new EB.Collections.Stack<IStackableWrapper>();
		while (_backStack.Count > 0)
		{
			var top = _backStack.Pop();
			if (top.stackable != ui)
			{
				if (top.stackable.IsFullscreen())
				{
					_isFullScreenOpened = true;
				}
				if (top.inputBlockerInstance != null && !_isInputBlockerVisible)
				{
					_isInputBlockerVisible = true;
					top.inputBlockerInstance.GetComponent<UIPanel>().alpha = top.inputBlockerInstance.GetComponentInChildren<TweenAlpha>().to;
				}
				tmp.Push(top);
			}
			else
			{
				wrapper = top;
				break;
			}
		}

		IStackableWrapper[] below = _backStack.ToArray();

		while (tmp.Count > 0)
		{
			_backStack.Push(tmp.Pop());
		}

		// remove from fullStack
		EB.Collections.Stack<StackedItem> fullTmp = new EB.Collections.Stack<StackedItem>();
		while (_fullStack.Count > 0)
		{
			var top = _fullStack.Pop();
			if (top != wrapper)
			{
				fullTmp.Push(top);
			}
			else
			{
				break;
			}
		}

		while (fullTmp.Count > 0)
		{
			_fullStack.Push(fullTmp.Pop());
		}

		// update visibility
		for (int i = 0; i < below.Length; ++i)
		{
			if (!_isFullScreenOpened)
			{
				below[i].stackable.Show(true);

				if (below[i].inputBlockerInstance != null && !_isInputBlockerVisible)
				{
					_isInputBlockerVisible = true;
					below[i].inputBlockerInstance.GetComponent<UIPanel>().alpha = below[i].inputBlockerInstance.GetComponentInChildren<TweenAlpha>().to;
				}

				if (below[i].stackable.IsFullscreen())
				{
					_isFullScreenOpened = true;

					GameUtils.SetMainCameraActive(below[i].stackable.IsRenderingWorldWhileFullscreen());
				}
			}

			if (_isFullScreenOpened)
			{
				break;
			}
		}

		if (!_isFullScreenOpened && !IsLoadingScreenUp)
		{
			GameUtils.SetMainCameraActive(true);
		}

		// remove
		if (wrapper.inputBlockerInstance != null)
		{
			TweenAlpha ta = wrapper.inputBlockerInstance.GetComponentInChildren<TweenAlpha>();
			ta.tweenFactor = _isInputBlockerVisible ? 0.1f : 1.0f;
			EventDelegate.Add(ta.onFinished, () => Destroy(wrapper.inputBlockerInstance));
			ta.PlayReverse();
		}

        ui.ClearData();
		yield return StartCoroutine(ui.OnRemoveFromStack());
	}

	/// <summary>
	/// wrap of EnStack
	/// </summary>
	/// <param name="ui"></param>
	public static void Open(IStackableUI ui)
	{
        if (Instance.IsStacked(ui))
		{
			if (Instance.IsTop(ui))
			{
				EB.Debug.Log(string.Format("UIStack.Open: {0} is already opened", ui));
				return;
			}
			else
			{
				Instance.MoveToTop(ui);
				return;
			}
		}

		if (Instance.IsQueued(ui))
		{
			Instance.Dequeue(ui);
		}

		Instance.EnStack(ui, false);
	}
    
	/// <summary>
	/// wrap of EnStack
	/// </summary>
	/// <param name="ui"></param>
	public static void Queue(IStackableUI ui)
	{
		if (Instance.IsStacked(ui))
		{
			Instance.Destack(ui);
		}

		if (Instance.IsQueued(ui))
		{
			Instance.Dequeue(ui);
		}

		Instance.EnStack(ui, true);
	}

	/// <summary>
	/// wrap of Destack
	/// </summary>
	/// <param name="ui"></param>
	public static void Close(IStackableUI ui)
	{
        if (!Instance.IsStacked(ui) && !Instance.IsQueued(ui) && !object.ReferenceEquals(ui, null) && !ui.Equals(null) && ui.Visibility)
		{
			ui.Show(false);
			return;
		}

		if (Instance.IsStacked(ui))
		{
			Instance.Destack(ui);
		}

		if (Instance.IsQueued(ui))
		{
			Instance.Dequeue(ui);
		}
	}

    public int GetBackStackCount()
    {
        return _backStack.Count;
    }
    public string GetBackStackItem()
    {
        string str = "";
        for(int i=0;i< _backStack.Count; i++)
        {
            str = string.Format("{0}+{1}", str, _backStack[i].stackable);
        }
        return str;
    }
}
