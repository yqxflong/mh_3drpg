using System;
using System.Collections;
using System.Collections.Generic;
using Fabric;
using ILR.HotfixManager;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIControllerILR : UIController
{
    public string hotfixClassPath;
	public string FilePath;

	public List<bool> BoolParamList;
    public List<int> IntParamList;
    public List<float> FloatParamList;
    public List<string> StringParamList;
    public List<Object> ObjectParamList;

    public UIControllerILRObject ilinstance;

    public bool IsFixShader;

    public List<ParmStruct> ParmPathList;

	public enum ParmType
	{
		GameObject,
		Transform,

		UISprite,
		UILabel,
		UIButton,
		UIInputs,
		UITexture,
		TweenAlpha,
		TweenPosition,
		UIEventTrigger,

		DynamicUISprite,
		ParticleSystemUIComponent,
		ConsecutiveClickCoolTrigger,
		CampaignTextureCmp,

		TweenScale,
		UISlider,
		UIGrid,
		UIWidget,
		UIScrollView,
		UIDragScrollView,
		UIPanel,
		UIProgressBar,

		BoxCollider,
		UIToggle,
		Camera,
		RenderTexture,

		DragEventDispatcher,
		UITable,
		UISymbolInput,
	}

	[System.Serializable]
	public struct ParmStruct
	{
		public string Name;
		public ParmType Type;
		public string Path;
	}

	public Dictionary<string, GameObject> GObjects;
	public Dictionary<string, Transform> Transforms;

	public Dictionary<string, UISprite> UiSprites;
	public Dictionary<string, UILabel> UiLabels;
	public Dictionary<string, UIButton> UiButtons;
	public Dictionary<string, UIInput> UiInputs;
	public Dictionary<string, UITexture> UiTextures;
	public Dictionary<string, TweenAlpha> TweenAlphas;
	public Dictionary<string, TweenPosition> TweenPositions;
	public Dictionary<string, UIEventTrigger> UiEventTriggers;

	public Dictionary<string, DynamicUISprite> DynamicUiSprites;
	public  Dictionary<string, ParticleSystemUIComponent> ParticleSystemUiComponents;
	public Dictionary<string, CampaignTextureCmp> TextureCmps;
	public Dictionary<string, ConsecutiveClickCoolTrigger> CoolTriggers;

	public Dictionary<string, TweenScale> TweenScales;
	public Dictionary<string, UISlider> UiSliders;
	public Dictionary<string, UIGrid> UiGrids;
	public Dictionary<string, UIWidget> UiWidgets;
	public Dictionary<string, UIScrollView> UiScrollViews;
	public Dictionary<string, UIDragScrollView> UiDragScrollViews;
	public Dictionary<string, UIPanel> UiPanels;
	public Dictionary<string, UIProgressBar> UiProgressBars;
	public Dictionary<string, UIToggle> UiToggles;

	public Dictionary<string, BoxCollider> BoxColliders;
	public Dictionary<string, Camera> Cameras;
	public Dictionary<string, RenderTexture> RenderTextures;

	public Dictionary<string, DragEventDispatcher> DragEventDispatchers;
	public Dictionary<string, UITable> UiTables;
	public Dictionary<string, UISymbolInput> UiSymbolInputs;

	/// <summary>
	/// 初始化ILR，可主动调用
	/// </summary>
	public void ILRObjInit()
    {
        if (ilinstance == null && !string.IsNullOrEmpty(hotfixClassPath))
        {

#if ILRuntime
            ilinstance = HotfixILRManager.GetInstance().appdomain.Instantiate<UIControllerILRObject>(hotfixClassPath);
#else
            var type = HotfixILRManager.GetInstance().assembly.GetType(hotfixClassPath);
            ilinstance = System.Activator.CreateInstance(type) as UIControllerILRObject;
#endif

            ilinstance.SetUIController(this);

            BindingParam();

            base.Awake();
            if (ilinstance != null)
            {
                ilinstance.Awake();

                if (IsFixShader)
                {
	                //修正Shader
	                GM.AssetUtils.FixShaderInEditor(this);
				}
            }
        }
    }

	private void FetchComponent<T>(ref Dictionary<string,T> container, string key, string path, bool isAttachment = false) where T : Object
	{
		if(container == null)container = new Dictionary<string, T>();
		if (isAttachment)
		{
			Transform coordinate = transform.FindEx(path);
			if (coordinate)
			{
				//Debug.LogFormat("path of <color=yellow>{0}</color> is typed for <color=yellow>{1}</color>!", path, typeof(T).Name);
				if (typeof(T).Name == "Transform")
					container.Add(key, coordinate as T);
				else
					container.Add(key, coordinate.gameObject as T);
			}
			else
				Debug.LogFormat("<color=red>path of {0} is invalid!</color>", path);
		}
		else
		{
			var comp = transform.GetSuperComponent<T>(path);
			if (comp != null)
			{
				//Debug.LogFormat("path of <color=yellow>{0}</color> is typed for <color=yellow>{1}</color>!", path, typeof(T).Name);
				if(!container.ContainsKey(key))
					container.Add(key, comp);
				else
					Debug.LogWarning("key of "+ key + " is exist in dict of "+ container.GetType().Name);
			}
			else
				Debug.LogFormat("<color=red>path of {0} is invalid!</color>", path);
		}
	}

	private void BindingParam()
	{
		if (ParmPathList != null && ParmPathList.Count > 0)
		{
			for (int i = 0; i < ParmPathList.Count; i++)
			{
				ParmStruct data = ParmPathList[i];
				switch (data.Type)
				{
					case ParmType.GameObject:
						FetchComponent(ref GObjects, data.Name, data.Path, true);
						break;
					case ParmType.Transform:
						FetchComponent(ref Transforms, data.Name, data.Path, true);
						break;
					case ParmType.UISprite:
						FetchComponent(ref UiSprites, data.Name, data.Path);
						break;
					case ParmType.UILabel:
						FetchComponent(ref UiLabels, data.Name, data.Path);
						break;
					case ParmType.UIButton:
						FetchComponent(ref UiButtons, data.Name, data.Path);
						break;
					case ParmType.UIInputs:
						FetchComponent(ref UiInputs, data.Name, data.Path);
						break;
					case ParmType.UITexture:
						FetchComponent(ref UiTextures, data.Name, data.Path);
						break;
					case ParmType.TweenAlpha:
						FetchComponent(ref TweenAlphas, data.Name, data.Path);
						break;
					case ParmType.TweenPosition:
						FetchComponent(ref TweenPositions, data.Name, data.Path);
						break;
					case ParmType.UIEventTrigger:
						FetchComponent(ref UiEventTriggers, data.Name, data.Path);
						break;
					case ParmType.DynamicUISprite:
						FetchComponent(ref DynamicUiSprites, data.Name, data.Path);
						break;
					case ParmType.ParticleSystemUIComponent:
						FetchComponent(ref ParticleSystemUiComponents, data.Name, data.Path);
						break;
					case ParmType.CampaignTextureCmp:
						FetchComponent(ref TextureCmps, data.Name, data.Path);
						break;
					case ParmType.ConsecutiveClickCoolTrigger:
						FetchComponent(ref CoolTriggers, data.Name, data.Path);
						break;
					case ParmType.TweenScale:
						FetchComponent(ref TweenScales, data.Name, data.Path);
						break;
					case ParmType.UISlider:
						FetchComponent(ref UiSliders, data.Name, data.Path);
						break;
					case ParmType.UIGrid:
						FetchComponent(ref UiGrids, data.Name, data.Path);
						break;
					case ParmType.UIWidget:
						FetchComponent(ref UiWidgets, data.Name, data.Path);
						break;
					case ParmType.UIScrollView:
						FetchComponent(ref UiScrollViews, data.Name, data.Path);
						break;
					case ParmType.UIDragScrollView:
						FetchComponent(ref UiDragScrollViews, data.Name, data.Path);
						break;
					case ParmType.UIPanel:
						FetchComponent(ref UiPanels, data.Name, data.Path);
						break;
					case ParmType.UIProgressBar:
						FetchComponent(ref UiProgressBars, data.Name, data.Path);
						break;
					case ParmType.BoxCollider:
						FetchComponent(ref BoxColliders, data.Name, data.Path);
						break;
					case ParmType.UIToggle:
						FetchComponent(ref UiToggles, data.Name, data.Path);
						break;
					case ParmType.Camera:
						FetchComponent(ref Cameras, data.Name, data.Path);
						break;
					case ParmType.RenderTexture:
						FetchComponent(ref RenderTextures, data.Name, data.Path);
						break;
					case ParmType.DragEventDispatcher:
						FetchComponent(ref DragEventDispatchers, data.Name, data.Path);
						break;
					case ParmType.UITable:
						FetchComponent(ref UiTables, data.Name, data.Path);
						break;
					case ParmType.UISymbolInput:
						FetchComponent(ref UiSymbolInputs, data.Name, data.Path);
						break;
				}
			}
		}
	}

	public void BindingBtnEvent(List<string> keyList, List<EventDelegate> eventList)
	{
		for (int i = 0; i < keyList.Count; i++)
		{
			string key = keyList[i];
			//Debug.Log("key = "+key);
			EventDelegate e = eventList[i];
			UIButton btn = null;
			if (UiButtons.TryGetValue(key, out btn))
			{
				//Debug.Log("key exist!");
				if (btn != null)
				{
					//Debug.Log("add event! e = "+e.methodName);
					btn.onClick.Add(e);
				}
			}
		}
	}

	public void BindingBtnEvent(UIButton[] buttons, EventDelegate[] eventList)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			UIButton button = buttons[i];
			EventDelegate e = eventList[i];

			if (button != null && e != null)
			{
				button.onClick.Add(e);
			}
		}
	}

	public void BindingIlrAction(Action[] actions, params Action[] methods)
	{
		for (int i = 0; i < actions.Length; i++)
		{
			actions[i] = methods[i];
		}
	}

	public void BindingIlrAction<T>(Action<T>[] actions, params Action<T>[] methods)
	{
		for (int i = 0; i < actions.Length; i++)
		{
			actions[i] = methods[i];
		}
	}

	public List<UIButton> FindAndBindingBtnEvent(List<string> pathList, List<EventDelegate> eventList)
	{
		List<UIButton> buttons = new List<UIButton>();
		for (int i = 0; i < pathList.Count; i++)
		{
			string path = pathList[i];

			if (!string.IsNullOrEmpty(path))
			{
				EventDelegate e = eventList[i];
				UIButton btn = transform.GetComponent<UIButton>(path);
				if (btn != null)
				{
					buttons.Add(btn);
					btn.onClick.Add(e);
				}
				else
					Debug.LogError("add <UIButton> event failed by path = " + path);
			}
		}
		return buttons;
	}

	public void BindingCoolTriggerEvent(List<string> keyList, List<EventDelegate> eventList)
	{
		for (int i = 0; i < keyList.Count; i++)
		{
			string key = keyList[i];
			EventDelegate e = eventList[i];
			ConsecutiveClickCoolTrigger trg = null;
			if (CoolTriggers.TryGetValue(key, out trg))
			{
				if (trg != null)
				{
					trg.clickEvent.Add(e);
				}
			}
		}
	}

	public List<ConsecutiveClickCoolTrigger> FindAndBindingCoolTriggerEvent(List<string> pathList, List<EventDelegate> eventList)
	{
		List<ConsecutiveClickCoolTrigger> triggers = new List<ConsecutiveClickCoolTrigger>();
		for (int i = 0; i < pathList.Count; i++)
		{
			string path = pathList[i];

			if (!string.IsNullOrEmpty(path))
			{
				EventDelegate e = eventList[i];
				ConsecutiveClickCoolTrigger trigger = transform.GetComponent<ConsecutiveClickCoolTrigger>(path);
				if (trigger != null)
				{
					triggers.Add(trigger);
					trigger.clickEvent.Add(e);
				}
				else
					Debug.LogError("add <ConsecutiveClickCoolTrigger> event failed by path = " + path);
			}
		}
		return triggers;
	}

	public List<T> FetchComponentList<T>(string[] paths, bool isGameObject = false) where T : Object
	{
		List<T> list = new List<T>();
		for (int i = 0; i < paths.Length; i++)
		{
			string path = paths[i];
			if(string.IsNullOrEmpty(path))
				continue;
			if (isGameObject)
			{
				Transform coordinate = transform.FindEx(path);
				if (coordinate)
					list.Add(coordinate.gameObject as T);
			}
			else
			{
				list.Add(transform.GetSuperComponent<T>(path));
			}
		}
		return list;
	}

	public List<UIEventTrigger> FindAndBindingEventTriggerEvent(List<string> pathList, List<EventDelegate> eventList, string type = "onPress")
	{
		List<UIEventTrigger> triggers = new List<UIEventTrigger>();
		for (int i = 0; i < pathList.Count; i++)
		{
			string path = pathList[i];

			if (!string.IsNullOrEmpty(path))
			{
				EventDelegate e = eventList[i];
				UIEventTrigger trigger = transform.GetComponent<UIEventTrigger>(path);

				if (trigger != null)
				{
					switch (type)
					{
						case "onClick":
							trigger.onClick.Add(e);
							break;
						case "onPress":
							trigger.onPress.Add(e);
							break;
						case "onDrag":
							trigger.onDrag.Add(e);
							break;
						// ...
					}
					triggers.Add(trigger);
				}
				else
					Debug.LogError("add <UIEventTrigger> event failed by path = " + path);
			}
		}
		return triggers;
	}

	public void FindAndBindingTweenFinishedEvent(List<string> pathList, List<EventDelegate> eventList)
	{
		for (int i = 0; i < pathList.Count; i++)
		{
			string path = pathList[i];

			if (!string.IsNullOrEmpty(path))
			{
				EventDelegate e = eventList[i];
				UITweener tweener = transform.GetComponent<UITweener>(path);
				if (tweener != null)
				{
					tweener.onFinished.Add(e);
				}
				else
					Debug.LogError("add tween finished event failed by path = " + path);
			}
		}
	}

	public void BindingToggleEvent(List<string> keyList, List<EventDelegate> eventList)
	{
		for (int i = 0; i < keyList.Count; i++)
		{
			string key = keyList[i];
			EventDelegate e = eventList[i];
			UIToggle toggle = null;
			if (UiToggles.TryGetValue(key, out toggle))
			{
				if (toggle != null)
				{
					toggle.onChange.Add(e);
				}
			}
		}
	}

	public void BindingToggleEvent(UIToggle[] toggles, List<EventDelegate> eventList)
	{
		for (int i = 0; i < toggles.Length; i++)
		{
			UIToggle toggle = toggles[i];
			EventDelegate e = eventList[i];

			if (toggle != null && e != null)
			{
				toggle.onChange.Add(e);
			}
		}
	}

	public List<UIToggle> FindAndBindingToggleEvent(List<string> pathList, List<EventDelegate> eventList)
	{
		List<UIToggle> toggles = new List<UIToggle>();
		for (int i = 0; i < pathList.Count; i++)
		{
			string path = pathList[i];

			if (!string.IsNullOrEmpty(path))
			{
				EventDelegate e = eventList[i];
				UIToggle toggle = transform.GetComponent<UIToggle>(path);
				if (toggle != null)
				{
					toggle.onChange.Add(e);
					toggles.Add(toggle);
				}
				else
					Debug.LogError("add <UIToggle> event failed by path = " + path);
			}
		}
		return toggles;
	}

	protected override void Awake()
    {
        ILRObjInit();
    }

    protected override void Start() {
        base.Start();

        if (ilinstance != null) {
            ilinstance.Start();
        }
    }

    private void OnEnable()
    {
        if (ilinstance != null)
        {
            ilinstance.OnEnable();
            //ILRUtils.RegisterNeedUpdateMono(ilinstance);
        }
    }

    private void OnDisable()
    {
        if (ilinstance != null)
        {
            ilinstance.OnDisable();
            ILRUtils.UnRegisterNeedUpdateMono(ilinstance);
        }
    }
    
    public override void OnFocus() {
        if (ilinstance != null) {
            ilinstance.OnFocus();
        }
        else
        {
            base.OnFocus();
        }
    }

    protected override void OnDestroy() {
        StopAllCoroutines();
        if (this != null && ilinstance != null)
        {
            ilinstance.OnDestroy();
        }

        base.OnDestroy();
    }

    public override IEnumerator OnPrepareAddToStack() {
        yield return base.OnPrepareAddToStack();

        if (ilinstance != null)
        {
            yield return ilinstance.OnPrepareAddToStack();
        }
    }

    public override IEnumerator OnAddToStack() {
        if (ilinstance != null)
        {
            yield return ilinstance.OnAddToStack();
        }
        else
        {
            yield return base.OnAddToStack();
        }
    }

    public override IEnumerator OnRemoveFromStack() {
        if (ilinstance != null)
        {
            yield return ilinstance.OnRemoveFromStack();
        }
        else
        {
            yield return base.OnRemoveFromStack();
        }
    }

    public override bool IsFullscreen() {
        if (ilinstance != null) {
            return ilinstance.IsFullscreen();
        }

        return base.IsFullscreen();
    }

    public override void SetMenuData(object param) {
        base.SetMenuData(param);
        if (ilinstance != null) {
            ilinstance.SetMenuData(param);
        }
    }

    public override void Show(bool isShowing) {
        if (ilinstance != null)
        {
            ilinstance.Show(isShowing);
        }
        else
        {
            base.Show(isShowing);
        }
    }

    public override void OnBlur() {
        base.OnBlur();
        ilinstance.OnBlur();
    }

    public override bool CanAutoBackstack() {
        if (ilinstance != null) {
            return ilinstance.CanAutoBackstack();
        }

        return base.CanAutoBackstack();
    }

    public override bool IsRenderingWorldWhileFullscreen() {
        if (ilinstance != null) {
            ilinstance.IsRenderingWorldWhileFullscreen();
        }

        return base.IsRenderingWorldWhileFullscreen();
    }

    public override bool Visibility {
        get {
            if (ilinstance != null) {
                return ilinstance.Visibility;
            }

            return base.Visibility;
        }
    }

    public override float BackgroundUIFadeTime {
        get {
            if (ilinstance != null) {
                return ilinstance.BackgroundUIFadeTime;
            }

            return base.BackgroundUIFadeTime;
        }
    }

    public override void OnPrefabSave() {
        base.OnPrefabSave();

        if (ilinstance != null) {
            ilinstance.OnPrefabSave();
        }
    }

    public override bool ShowUIBlocker {
        get {
            if (ilinstance != null) {
                return ilinstance.ShowUIBlocker;
            }

            return base.ShowUIBlocker;
        }
    }

    protected override void StartBootFlash()
    {
        if (ilinstance != null)
        {
            ilinstance.StartBootFlash();
        }
    }

    public override void OnCancelButtonClick()
    {
        if (ilinstance != null)
        {
            ilinstance.OnCancelButtonClick();
            return;
        }
        base.OnCancelButtonClick();
    }

    public void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    {
        if (ilinstance != null)
        {
            ilinstance.OnFetchData(res, reqInstanceID);
        }
    }

}

/// <summary>
/// 热更代码里不直接继承这个类而是 继承这个类的热更里的hotfix子类 
/// </summary>
public class UIControllerILRObject: IUpdateable {
    public virtual void SetUIController(UIControllerILR uicontroller) {
    }

    public virtual UIControllerILR GetUIController() {
        return null;
    }

    public virtual void Awake() {
    }

    public virtual void Start() {
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
    }

    public virtual void Update(int e) {
    }

    public virtual void OnFocus() {
    }

    public virtual void OnDestroy() {
    }

    public virtual IEnumerator OnPrepareAddToStack() {
        yield return null;
    }

    public virtual IEnumerator OnAddToStack() {
        yield return null;
    }

    public virtual IEnumerator OnRemoveFromStack() {
        yield return null;
    }

    public virtual bool IsFullscreen() {
        return false;
    }

    public virtual void SetMenuData(object param) {
    }

    public virtual void Show(bool isShowing) {
    }

    public virtual void OnBlur() {
    }

    public virtual void OnPrefabSave() {
    }

    public virtual bool CanAutoBackstack() {
        return !GetUIController().IsFullscreen() || GetUIController().IsHudUI;
    }

    public virtual bool IsRenderingWorldWhileFullscreen() {
        return false;
    }

    public virtual bool Visibility {
        get {
            return GetUIController().gameObject.activeSelf && GetUIController().GetComponent<UIPanel>().alpha > 0.0f;
        }
    }

    public virtual float BackgroundUIFadeTime {
        get {
            return 0;
        }
    }

    public virtual bool ShowUIBlocker {
        get {
            return false;
        }
    }

    public virtual void StartBootFlash()
    {
        
    }

    public virtual void OnCancelButtonClick()
    {

    }

    public virtual void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
    {
    }

}