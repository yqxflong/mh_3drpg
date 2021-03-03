using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GM;
using EB.Sparx;
using System.Threading;

/// <summary>
/// 界面控件基类
/// </summary>
[RequireComponent(typeof(UIPanel))]
public class UIController : MonoBehaviour, IStackableUI
{
	/// <summary>
	/// 返回按钮，如果不为null，在start中会赋予点击事件
	/// </summary>
	public UIButton backButton;
    
	/// <summary>
	/// hudRoot must inited before Start, if parent is not set it should inited after Awake
	/// </summary>
	public Transform hudRoot = null;

	public System.Action<UIController> destroyHandler = null;
    
    [HideInInspector]
	public GameObject mBlocker;
    [HideInInspector]
    public BoxCollider mCollider;
    [HideInInspector]
    public ConsecutiveClickCoolTrigger mTrigger;
    [HideInInspector]
    public bool IsFirstLoad = true;
    [HideInInspector]
    public bool HasPlayedTween;

    public bool HasAnimatedFadeIn;
	public Vector2Int WaitFrameForBoot = Vector2Int.one;
	public int WaitFrameForDisabled = 1;

	public bool IsTweenAlphaOnMainPanel { get; set; }

	/// <summary>
	/// 移动到远处的坐标
	/// </summary>
	protected Vector3 mHideUIPos = new Vector3(0, 100000, 0);

    /// <summary>
    /// 界面初始的ui坐标
    /// </summary>
    protected Vector3 mInitUIPos = Vector3.zero;

    /// <summary>
    /// return this is Hud Root UI or not
    /// </summary>
    public bool IsHudUI
	{
		get { return hudRoot == transform; }
	}

	private int mGuideStatus = 0;
	private object mDestroyHandle = null;

    /// <summary>
    /// SetMenuData所传参数将存储于此，SetMenuData中赋值
    /// </summary>
	protected object mParam = null;

    protected virtual void Awake()
	{
		//EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>Awake</color>方法", this.gameObject.name);
		UITweener[] ts = transform.GetComponents<UITweener>();
		for (int i = 0; i < ts.Length; i++)
		{
			if (ts[i] is TweenAlpha)
			{
				IsTweenAlphaOnMainPanel = true;
				break;
			}
		}
	}

	// Use this for initialization
	protected virtual void Start()
	{
        //EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>Start</color>方法", this.gameObject .name);
        if (backButton != null)
		{// setup default cancel button click handler
			backButton.onClick.Clear();
			backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
		}

		if (!IsHudUI && !ShowUIBlocker && IsFullscreen()&& backButton != null)
		{
			BoxCollider bc = GetComponent<BoxCollider>();
			if (bc == null)
			{
				NGUITools.AddWidgetCollider(gameObject);
			}
			else
			{
				NGUITools.UpdateWidgetCollider(bc.gameObject);
            }
		}

		IsFirstLoad = false;
	}

    /// <summary>
    /// Start后执行的协程，用于播放Tween动画
    /// </summary>
    /// <returns></returns>
    public void PlayTween()
	{
        if (IsFullscreen())
        {
            transform.localPosition = mInitUIPos;

            //打开界面时重新刷新锚点
            //ToDo:暂时屏蔽
            //bool isContain = (GlobalMenuManager.Instance != null && GlobalMenuManager.Instance.IsCouldFindController(this));
            bool isContain = (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "IsCouldFindControllerFromILR", this);

            if (isContain)
            {
                var uIWidgets = transform.GetComponentsInChildren<UIWidget>();
                for (int i = 0; i < uIWidgets.Length; i++)
                {
                    uIWidgets[i].UpdateAnchors();
                }
            }
        }
        //EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>StartAfterCoroutine</color>协程", this.gameObject.name);
        HasPlayedTween = true;
        //设为true是为了解决反复点击同一触发区出现的界面不弹出问题
        gameObject.SetActive(true);
		// 点级特效要关闭
		ClickFxPoolManager.Instance.StopAll();

        int waitFrame = IsFirstLoad ? WaitFrameForBoot.x : WaitFrameForBoot.y;

        if (IsFirstLoad) PauseBootFlash();
        TimerManager.instance.AddFramer(waitFrame, 1, delegate { StartBootFlash(); });
		LTTools.SwitchToPlayerCamera(IsFullscreen());
	}

    protected void PauseBootFlash()
    {
	    UITweener[] ts = transform.GetComponents<UITweener>();
	    foreach (var t in ts)
	    {
		    t.enabled = false;
	    }
    }

	protected virtual void StartBootFlash()
	{
		UITweener[] ts = transform.GetComponents<UITweener>();
		foreach (var t in ts)
		{
			t.tweenFactor = 0;
            t.ResetToBeginning();
			t.PlayForward();
		}
	}

	// Update is called once per frame
	//protected virtual void Update()
	//{

	//}

	protected virtual void OnDestroy()
    {
        if (this.gameObject != null)
        {
            EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#ff0000>OnDestroy</color>方法", this.gameObject.name);
        }

        DestroyController();
        if (UIStack.Instance != null && UIStack.Instance.gameObject.activeSelf)
		{
			if (UIStack.Instance.IsStacked(this))
			{
				UIStack.Instance.Destack(this);
			}
			
			if (UIStack.Instance.IsQueued(this))
			{
				UIStack.Instance.Dequeue(this);
			}
        }

        if(mParam is Hashtable){
            Johny.HashtablePool.ReleaseRecursion(mParam as Hashtable);
        }
    }

	/// <summary>
	/// shortcut for UIStack.Queue
	/// </summary>
	public void Queue()
	{
        if (this.gameObject != null)
        {
            EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>Queue无参</color>方法", this.gameObject.name);
        }

        if (UIStack.Instance != null && UIStack.Instance.gameObject.activeSelf)
		{
			UIStack.Queue(this);
            //ToDo:暂时屏蔽
            //if (GlobalMenuManager.Instance != null && gameObject != null)
            //{
            //    GlobalMenuManager.Instance.SetUIEnabled(MCurrentViewName, true);
            //}
            if (gameObject != null)
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "SetUIEnabledFromILR", MCurrentViewName, true);
            }
        }
	}

	public void Queue(object param)
	{
        if (this.gameObject != null)
        {
            EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>Queue有参</color>方法param", this.gameObject.name);
        }

        SetMenuData(param);
        PlayTween();
        Open();
    }

	/// <summary>
	/// shortcut for UIStack.Open
	/// </summary>
	public void Open()
	{
        if (this.gameObject != null)
        {
            EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>Open</color>方法", this.gameObject.name);
        }

        if (UIStack.Instance != null && UIStack.Instance.gameObject.activeSelf)
		{
			UIStack.Open(this);
            //ToDo:暂时屏蔽
            //if (GlobalMenuManager.Instance != null && gameObject != null)
            //{
            //    GlobalMenuManager.Instance.SetUIEnabled(MCurrentViewName, true);
            //}
            if (gameObject != null)
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "SetUIEnabledFromILR", MCurrentViewName, true);
            }
        }
	}

	public void Open(object param)
	{
        SetMenuData(param);
        PlayTween();
		Open();
	}

	/// <summary>
	/// shortcut for UIStack.Close
	/// </summary>
	public void Close()
	{
        if (this.gameObject != null)
        {
            EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#ff0000>Close</color>方法", this.gameObject.name);     
        }

        if (UIStack.Instance != null && UIStack.Instance.gameObject.activeSelf)
		{
			UIStack.Close(this);
            //ToDo:暂时屏蔽
            //if (GlobalMenuManager.Instance != null && gameObject != null)
            //{
            //    GlobalMenuManager.Instance.SetUIEnabled(MCurrentViewName, false);
            //}
            if (gameObject != null)
            {
                GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "SetUIEnabledFromILR", MCurrentViewName, false);
            }
        }
        IsFirstLoad = false;
        HasPlayedTween = false;
	}

    /// <summary>
    /// 界面是否打开
    /// </summary>
    /// <returns></returns>
	public bool IsOpen()
	{
		return this != null && UIStack.Instance != null && UIStack.Instance.IsStacked(this);
	}

    /// <summary>
    /// 返回按钮点击事件
    /// </summary>
	public virtual void OnCancelButtonClick()
	{
	    FusionAudio.PostEvent("UI/General/ButtonClick");
        Close();
    }

    public void OnBtnClickWithAnimation(GameObject btn)
    {
        if (btn.GetComponent<TweenScale>() != null)
        {
            btn.GetComponent<TweenScale>().ResetToBeginning();
            btn.GetComponent<TweenScale>().PlayForward();
        }
        else
        {
            EB.Debug.LogError("can not find tween animation, please set first");
        }
    }

    public virtual float BackgroundUIFadeTime
	{
		get { return 0.0f; }
	}

	public virtual bool EnstackOnCreate
	{
		get { return IsHudUI; }
	}

    /// <summary>
    /// 是否显示黑色透明背景遮罩，一般非全屏界面才需要显示。
    /// </summary>
	public virtual bool ShowUIBlocker
	{
		get { return false; }
	}

    /// <summary>
    /// 是否可见
    /// </summary>
	public virtual bool Visibility
	{
		get { return gameObject.activeSelf && GetComponent<UIPanel>().alpha > 0.0f; }
	}

    public string MCurrentViewName { get; set; } = string.Empty;

    public virtual bool CanAutoBackstack()
	{
		return !IsHudUI /*&& backButton != null*/;
	}

    /// <summary>
    /// 是否是全屏界面
    /// </summary>
    /// <returns></returns>
	public virtual bool IsFullscreen()
	{
		return false;
	}

    public virtual void ClearData()
    {
        mParam = null;
    }

	/// <summary>
	/// if return false Camera.main is null
	/// </summary>
	/// <returns></returns>
	public virtual bool IsRenderingWorldWhileFullscreen()
	{
		return !IsFullscreen() || IsHudUI;
	}

    public virtual IEnumerator OnPrepareAddToStack()
    {
        UIPanel panel = gameObject.GetComponent<UIPanel>();
        if (panel != null)
        {
            panel.alpha = 0.01f;
        }
        yield break;
    }

	public virtual IEnumerator OnAddToStack()
	{
        yield return BaseOnAddToStack();
    }

    public IEnumerator BaseOnAddToStack()
    {
        if ((IsHudUI || IsFullscreen()) && !gameObject.activeSelf)
        {
            gameObject.CustomSetActive(true);
        }
        Show(true);

        if (GetComponent<BoxCollider>() != null && !GetComponent<BoxCollider>().enabled) GetComponent<BoxCollider>().enabled = true;

        if (ShowUIBlocker)
        {
            mBlocker = UIStack.Instance.GetPanelBlocker(this);
            mCollider = mBlocker.GetComponentInChildren<BoxCollider>();
            mTrigger = mCollider.GetComponent<ConsecutiveClickCoolTrigger>();
            if (mTrigger == null)
            {
                mTrigger = mCollider.gameObject.AddComponent<ConsecutiveClickCoolTrigger>();
            }
            mTrigger.clickEvent.Clear();
            mTrigger.clickEvent.Add(new EventDelegate(OnCancelButtonClick));

            Transform bg = mBlocker.transform.Find("Background");
            UISprite bgSprite = bg.GetComponent<UISprite>();
            bgSprite.enabled = true;
        }
        //ToDo:暂时屏蔽
        //if (GlobalMenuManager.Instance != null)
        //{
        //    GlobalMenuManager.Instance.AddOpenController(this);
        //}
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "AddOpenControllerFromILR", this);

        yield break;
    }

	public virtual IEnumerator OnRemoveFromStack()
	{
        yield return BaseOnRemoveFromStack();
    }

    public IEnumerator BaseOnRemoveFromStack()
    {
        StopAllCoroutines();
        if (ShowUIBlocker)
        {
            ResetUIBlockerArgs();
        }
        mParam = null;

        Show(false);
        if ((IsHudUI || IsFullscreen()) && gameObject.activeSelf)
        {
            gameObject.CustomSetActive(false);
        }

        yield break;
    }

    public void ResetUIBlockerArgs()
    {
        mTrigger = null;
        mCollider = null;
        mBlocker = null;
    }

    /// <summary>
    /// ui的显示隐藏与一些层级相关
    /// </summary>
    /// <param name="isShowing"></param>
	public virtual void Show(bool isShowing)
	{
        if (this == null || this.GetComponent<UIController>() == null)
        {
            return;
        }
		if (IsHudUI || IsFullscreen())
        {
            UIPanel panel = GetComponent<UIPanel>();
            if (panel != null)
            {
                panel.alpha = isShowing ? 1.0f : 0.0f;
            }

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.layer == GameEngine.Instance.transparentUI3DLayer || renderer.gameObject.layer == GameEngine.Instance.ui3dLayer)
                {
                    renderer.gameObject.layer = isShowing ? GameEngine.Instance.ui3dLayer : GameEngine.Instance.transparentUI3DLayer;
                }
                else
                {
                    renderer.gameObject.layer = isShowing ? GameEngine.Instance.uiLayer : GameEngine.Instance.transparentFXLayer;
                }
            }
        }
		else
		{
			gameObject.CustomSetActive(isShowing);
		}
	}

    /// <summary>
    /// 成为焦点界面时调用的方法
    /// </summary>
	public virtual void OnFocus()
	{
        UIPanel[] panels = GetComponentsInChildren<UIPanel>();
		if (panels != null)
		{
			for (int i = 0; i < panels.Length; i++)
			{
				panels[i].SetDirty();
			}
		}
        // //作用于当前新手引导界面识别，暂不能删除
        // GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.CommonConditionParse", "SetFocusViewName", gameObject.name.Replace("(Clone)", ""));
    }

    /// <summary>
    /// 脱离焦点界面时调用的方法，注意这个方法很多业务子类重写了这个方法，开关很可能没有关闭成功
    /// </summary>
	public virtual void OnBlur()
	{
		//EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#ff0000>OnBlur</color>方法", this.gameObject.name);
		HasPlayedTween = false;
		IsFirstLoad = false;
		mGuideStatus++;
	}
    //未发现引用，测试没问题删除：TODO
 //   public virtual void ExcuteGuideStep()
	//{
	//	//GuideManager.Instance.Excute(gameObject);
	//	GuideManager.Instance.ExcuteEx(transform, null);
	//}
    
    /// <summary>
    /// 设置界面相关数据
    /// </summary>
    /// <param name="param"></param>
	public virtual void SetMenuData(object param)
	{
		mParam = param;
        //EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设UIController中<color=#42fe79>SetMenuData</color>方法", this.gameObject.name);
	}

    /// <summary>
    /// 临时的一个不可删除的Controller的列表，以后会逐步清除
    /// </summary>
    private List<string> mTempCouldnotDestroyCon = new List<string>()
    {
    };

    /// <summary>
    /// 销毁ui，需主动调用，一般在OnRemoveFromStack中调用
    /// </summary>
    /// <param name="isGc"></param>
	public void DestroySelf(bool isGc = false)
    {
        //先这样写，以后会往下走。
        //DestroyController();
        //return;
        if (this ==null|| gameObject == null) return; 

        EB.Debug.LogUI("-----------执行【<color=#00ff00>{0}</color>】预设<color=#ff0000>清除UI</color>界面对象, 是否全屏 = {1}", MCurrentViewName, IsFullscreen());

        Show(false);
        //ToDo:暂时屏蔽
        //if (GlobalMenuManager.Instance != null)
        //{
        //    GlobalMenuManager.Instance.RemoveOpenController(this);
        //}
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "RemoveOpenControllerFromILR", this);

        if (IsFullscreen())
        {
            transform.localPosition = mHideUIPos;
            if (GetComponent<BoxCollider>() != null)
            {
                GetComponent<BoxCollider>().enabled = false;
            }
            //ToDo:暂时屏蔽
            //GlobalMenuManager.Instance.AddUIController(this);
            GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "AddUIControllerFromILR", this);
        }
    }

    /// <summary>
    /// 外部调用的销毁逻辑，只会有一个调用，多了就不正常
    /// </summary>
    public void DestroyControllerForm()
    {
        DestroyController();
    }

    /// <summary>
    /// 销毁Controller
    /// </summary>
    private void DestroyController()
	{
        if (IsDestroyed(this))
		{
			return;
		}

        if (gameObject != null)
        {
            EB.Debug.LogUI("执行【<color=#00ff00>{0}</color>】预设<color=#ff0000>清除UI</color>界面对象", gameObject.name);
        }
        
        StopAllCoroutines();
        if (ShowUIBlocker)
        {
            mTrigger = null;
            mCollider = null;
            mBlocker = null;
        }
        mParam = null;

        Show(false);
        DestroyObject();
        //ToDo:暂时屏蔽
        //if (GlobalMenuManager.Instance != null)
        //{
        //    GlobalMenuManager.Instance.RemoveOpenController(this);
        //}
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "RemoveOpenControllerFromILR", this);
    }

    /// <summary>
    /// 销毁Obj
    /// </summary>
	private void DestroyObject()
	{
        //
        //ClearUIAtals(this.gameObject);
        //
        if (IsDestroyed(this))
		{
			EB.Debug.LogWarning("DestroyObject: already destroyed");
			return;
		}

		if (IsOpen() || Visibility)
		{
			EB.Debug.Log("DestroyObject: already re-open");
			return;
		}
        if (destroyHandler != null)
		{
			destroyHandler(this);
		}
		else
		{
			// mark as destroyed, use IsDestroyed() to detect destroyed
			UIPanel panel = GetComponent<UIPanel>();
			if (panel != null)
			{
				panel.enabled = false;
			}

			// OnDestroy called end of this frame, then gameObject == null
			Destroy(gameObject);
		}
    }

    /// <summary>
    /// 某个界面管理器是否被删除了
    /// </summary>
    /// <param name="ui"></param>
    /// <returns></returns>
	public static bool IsDestroyed(UIController ui)
	{
		if (ui == null)
		{
			return true;
		}

        bool isDestroyed = false;


        UIPanel panel = ui.GetComponent<UIPanel>();
        if (panel == null)
        {
            isDestroyed = ui.gameObject.activeSelf;
        }
        else
        {
            //isDestroyed = !panel.enabled;
            isDestroyed = false;
        }

        if (isDestroyed)
        {
            //ToDo:暂时屏蔽
            //if (GlobalMenuManager.Instance != null)
            //{
            //    GlobalMenuManager.Instance.RemoveOpenController(ui);
            //}
            GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.GlobalMenuManager", "RemoveOpenControllerFromILR", ui);
        }

        return isDestroyed;
	}


    /// <summary>
    /// 清除对象上的图集
    /// </summary>
    /// <param name="obj">要清除的对象</param>
    private void ClearUIAtals(GameObject obj)
    {
        UISprite[] allUISprite = obj.GetComponentsInChildren<UISprite>();
        List<UIAtlas> allUIAtlas = new List<UIAtlas>();
        for (int i = 0; i < allUISprite.Length; i++)
        {
            if (allUISprite[i].atlas != null
                && !allUISprite[i].atlas.name.Contains("LTGeneral_Atlas")
                && !allUISprite[i].atlas.name.Contains("LTMainHud_Atlas")
                && !allUISprite[i].atlas.name.Contains("Partner_Head")
                && !allUIAtlas.Contains(allUISprite[i].atlas)
                )
            {
                allUIAtlas.Add(allUISprite[i].atlas);
                try
                {
                    Resources.UnloadAsset(allUISprite[i].atlas.spriteMaterial.mainTexture);
                }
                catch
                {

                }
            }
        }

        UITexture[] allUITexture = obj.GetComponentsInChildren<UITexture>();
        for (int i = 0; i < allUITexture.Length; i++)
        {
            if (allUITexture[i].mainTexture != null)
            {
                try
                {
                    Resources.UnloadAsset(allUITexture[i].mainTexture);
                }
                catch
                {

                }
            }
        }
    }

    public virtual void OnPrefabSave()
	{
#if UNITY_EDITOR

#endif
	}

#if UNITY_EDITOR
	[ContextMenu("BtnSetting")]
	public void BtnSetting()
	{
		UIButton[] btns = GetComponentsInChildren<UIButton>(true);
		foreach (var btn in btns)
		{
			btn.hover = Color.white;
			btn.pressed = Color.white;
			UIButtonScale btnScale = btn.GetComponent<UIButtonScale>();
			if (btnScale == null)
			{
				Debug.LogError("add UIButtonScale Component For Btn=" + btn.gameObject.name);
				btn.gameObject.AddComponent<UIButtonScale>();
			}
		}
	}

	[ContextMenu("FontSetting")]
	public void FontSetting()
	{
		UILabel[] allLabel = transform.GetComponentsInChildren<UILabel>(true);
		foreach (var one in allLabel)
		{
			one.fontStyle = FontStyle.Normal;
			if (one.effectColor == Color.black && one.effectStyle != UILabel.Effect.None && one.effectDistance.x != 3)
			{
				one.effectStyle = UILabel.Effect.Outline8;
				one.effectDistance = new Vector2(2, 2);
			}

			if (one.color == Color.black && one.transform.parent.GetComponent<UILabel>() != null)
			{
				one.transform.localPosition = new Vector3(0, -4, 0);
			}
		}
	}
#endif
}
