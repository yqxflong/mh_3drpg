using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class UIControllerHotfix : UIControllerILRObject {
        public UIControllerILR controller;

        public override void SetUIController(UIControllerILR uicontroller) {
            controller = uicontroller;
        }

        public override UIControllerILR GetUIController() {
            return controller;
        }

        public static PrefabCreator GetCreator(PrefabCreator[] temps, string name) {
            for (int i = 0; i < temps.Length; i++) {
                if (temps[i].name.Equals(name)) {
                    return temps[i];
                }
            }
            return null;
        }

        public static UIControllerHotfix Current;

        public List<System.Action> CloseCallbacks = new List<Action>();

        private int panelTimer, rendererTiemr = 0;

        protected bool InvokeCloseAction = true;

		public override void Awake() {
        }

        public override void Start() {
        }

        public override IEnumerator OnPrepareAddToStack() {
            yield return null;
        }

        public override IEnumerator OnAddToStack()
        {
            //yield return controller.BaseOnAddToStack();
            if ((controller.IsHudUI || IsFullscreen()) && !controller.gameObject.activeSelf)
            {
                controller.gameObject.CustomSetActive(true);
            }
            Show(true);

            var box = controller.GetComponent<BoxCollider>();
            if (box != null && !box.enabled) box.enabled = true;

            if (ShowUIBlocker)
            {
                controller.mBlocker = UIStack.Instance.GetPanelBlocker(controller);
                controller.mCollider = controller.mBlocker.GetComponentInChildren<BoxCollider>();
                controller.mTrigger = controller.mCollider.GetComponent<ConsecutiveClickCoolTrigger>();
                if (controller.mTrigger == null)
                {
                    controller.mTrigger = controller.mCollider.gameObject.AddComponent<ConsecutiveClickCoolTrigger>();
                }
                controller.mTrigger.clickEvent.Clear();
                controller.mTrigger.clickEvent.Add(new EventDelegate(OnCancelButtonClick));

                Transform bg = controller.mBlocker.transform.Find("Background");
                UISprite bgSprite = bg.GetComponent<UISprite>();
                bgSprite.enabled = true;
            }
            if (GlobalMenuManager.Instance != null)
            {
                GlobalMenuManager.Instance.AddOpenController(controller);
            }

            yield break;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            //yield return controller.BaseOnRemoveFromStack();
            StopAllCoroutines();
            if (ShowUIBlocker)
            {
               controller.ResetUIBlockerArgs();
            }
            //热更工程这边的mParam获取不到 在热更工程这边不会使用
            //controller.mParam = null;
		
            Show(false);
            if ((controller.IsHudUI || IsFullscreen()) && controller.gameObject.activeSelf)
            {
                controller.gameObject.CustomSetActive(false);
            }

            yield break;
        }

        public override void OnBlur() {
        }

        public override void OnDestroy() {
        }

        public override void OnFocus() {
	        PausePanelUpdate(UIPanel.PauseType.Others, false);
            Current = this;
			UIPanel[] panels = controller.transform.GetComponentsInChildren<UIPanel>();
            if (panels != null)
            {
                for (int i = 0; i < panels.Length; i++)
                {
                    panels[i].SetDirty();
                }
            }
            //作用于当前新手引导界面识别，暂不能删除
            CommonConditionParse.SetFocusViewName(controller.gameObject.name.Replace("(Clone)",""));
        }

        public virtual void OnFocusWithoutPauser()
        {
	        UIPanel[] panels = controller.transform.GetComponentsInChildren<UIPanel>();
	        if (panels != null)
	        {
		        for (int i = 0; i < panels.Length; i++)
		        {
			        panels[i].SetDirty();
		        }
	        }
	        //作用于当前新手引导界面识别，暂不能删除
	        CommonConditionParse.SetFocusViewName(controller.gameObject.name.Replace("(Clone)", ""));
		}

        //public virtual void Update(){
		//}

		protected virtual void RegisterMonoUpdater()
		{
			Hotfix_LT.ILRUpdateManager.RegisterNeedUpdateMono(this);
		}

		protected virtual void ErasureMonoUpdater()
		{
			Hotfix_LT.ILRUpdateManager.UnRegisterNeedUpdateMono(this);
		}

		public override void OnPrefabSave() {
        }

        public override void SetMenuData(object param) {
        }

        public override void Show(bool isShowing)
        {
	        if (this == null || controller.GetComponent<UIController>() == null)
            {
                return;
            }

            if (isShowing) ResumePanelUpdate();

            if (controller.IsHudUI || IsFullscreen() || controller.WaitFrameForBoot.y != 1)
            {
                UIPanel panel = controller.GetComponent<UIPanel>();

                if (panel != null)
                {
                    if (controller.WaitFrameForBoot.y != 1 && controller.HasPlayedTween)
                    {
                        panel.alpha = 0.0f;
                    }
                    else
                    {
                        if (isShowing)
                        {
                            TimerManager.instance.RemoveTimerSafely(ref panelTimer);
                            panel.alpha = 1.0f;
                        }
                        else
                            panelTimer = TimerManager.instance.AddFramer(controller.WaitFrameForDisabled, 1, delegate {
                                if (panel)
                                {
                                    panelTimer = 0;
                                    panel.alpha = 0.0f;
                                }
                            });
                    }
                }

                if (controller.IsHudUI || IsFullscreen())
                {
                    Renderer[] renderers = controller.GetComponentsInChildren<Renderer>();

                    if (renderers != null && renderers.Length > 0)
                    {
                        for (var i = 0; i < renderers.Length; i++)
                        {
                            var renderer = renderers[i];

                            if (renderer.gameObject.layer == GameEngine.Instance.transparentUI3DLayer || renderer.gameObject.layer == GameEngine.Instance.ui3dLayer)
                            {
                                renderer.gameObject.layer = isShowing ? GameEngine.Instance.ui3dLayer : GameEngine.Instance.transparentUI3DLayer;
                            }
                            else
                            {
                                if (isShowing)
                                {
                                    TimerManager.instance.RemoveTimerSafely(ref rendererTiemr);
                                    renderer.gameObject.layer = GameEngine.Instance.uiLayer;
                                }
                                else
                                    rendererTiemr = TimerManager.instance.AddFramer(controller.WaitFrameForDisabled, 1, delegate {
                                        if (renderer)
                                        {
                                            rendererTiemr = 0;
                                            renderer.gameObject.layer = GameEngine.Instance.transparentFXLayer;
                                        }
                                    });
                            }
                        }
                    }
                }
                else
                    controller.gameObject.CustomSetActive(isShowing);
            }
            else
            {
                controller.gameObject.CustomSetActive(isShowing);
            }
        }

        public override bool IsFullscreen() {
            return false;
        }

        public override bool CanAutoBackstack() {
            return !controller.IsFullscreen() || controller.IsHudUI;
        }

        public override bool IsRenderingWorldWhileFullscreen() {
            return false;
        }

        public override bool Visibility {
            get {
                return controller.gameObject.activeSelf && controller.GetComponent<UIPanel>().alpha > 0.0f;
            }
        }

        public override float BackgroundUIFadeTime {
            get {
                return 0;
            }
        }

        public override bool ShowUIBlocker {
            get {
                return false;
            }
        }

        protected void DestroySelf() {
            if(controller!=null) controller.DestroySelf(false);
        }

        public override void OnCancelButtonClick() {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            controller.Close();
            if (CloseCallbacks.Count > 0 && InvokeCloseAction)
            {
	            for (int i = 0; i < CloseCallbacks.Count; i++)
	            {
		            CloseCallbacks[i]?.Invoke();
		            CloseCallbacks[i] = null;
	            }
				CloseCallbacks.Clear();
            }
        }

        protected void SetCurrentPanelAlpha(float value)
        {
			FetchPanelWhileNull();
			//if(Exist(CurrentPanel))EB.Debug.LogError("SetCurrentPanelAlpha "+ (CurrentPanel.name));
			if (CurrentPanel != null && !controller.IsTweenAlphaOnMainPanel && !controller.HasAnimatedFadeIn)
			{
				CurrentPanel.alpha = value;
				//EB.Debug.LogError("SetCurrentPanelAlpha " + (value));
			}
			LTTools.SwitchToUICamera(controller.IsFullscreen());

			controller.HasPlayedTween = false;
        }

		// 很多窗口的业务逻辑重写了这个方法，注意
		public override void StartBootFlash()
        {
	        SetCurrentPanelAlpha(1);

            if (controller == null)
            {
                return;
            }

			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();

            if (tweeners == null)
            {
                return;
            }

			for (int j = 0; j < tweeners.Length; ++j) {
				tweeners[j].enabled = true;
				tweeners[j].ResetToBeginning();
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        static public GameObject InstantiateEx(GameObject target, Transform parent, string name = "Clone") {
            GameObject go = GameObject.Instantiate(target);
            go.name = name;
            go.transform.SetParent(parent, false);
            if (false == go.activeSelf) go.gameObject.SetActive(true);
            return go;
        }

        static public GameObject InstantiateEx(Transform target, Transform parent, string name = "Clone") {
            return InstantiateEx(target.gameObject, parent, name);
        }

        static public T InstantiateEx<T>(T target, Transform parent, string name = "Clone") where T : DynamicMonoHotfix {
            GameObject obj = target.mDMono.gameObject;
            GameObject go = InstantiateEx(obj, parent, name);
            return go.GetMonoILRComponent<T>();
        }

        static public T InstantiateExOrig<T>(T target, Transform parent, string name = "Clone") where T : MonoBehaviour {
            GameObject obj = target.gameObject;
            GameObject go = InstantiateEx(obj, parent, name);
            return go.GetComponent<T>();
        }

        #region Coroutine
        protected Coroutine StartCoroutine(IEnumerator routine) {
            if (controller == null || !controller.gameObject.activeInHierarchy)
            {
                return null;
            }

            return controller.StartCoroutine(routine);
        }

        protected void StopCoroutine(Coroutine c) {
            if (controller != null)
            {
                controller.StopCoroutine(c);
            }
        }

        protected void StopCoroutine(IEnumerator c) {
            if (controller != null)
            {
                controller.StopCoroutine(c);
            }
        }

        protected void StopAllCoroutines() {
            if (controller != null)
            {
                controller.StopAllCoroutines();
            }
        }
        #endregion

        #region UISeverRequest Call
        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
        {
            
        }
		#endregion

		#region UIPanelUpdate Manul Handler

		protected T[] GetArray<T>(params T[] array)
		{
			return array;
		}

		protected List<T> GetList<T>(params T[] array)
		{
			return new List<T>(array);
		}

		protected UIPanel CurrentPanel;

		protected void FetchPanelWhileNull()
		{
			if (CurrentPanel == null && controller != null)
			{
                if (controller.hudRoot != null)
                {
                    CurrentPanel = controller.hudRoot.GetComponent<UIPanel>();
                }

                if (CurrentPanel == null)
                {
                    CurrentPanel = controller.GetComponent<UIPanel>();
                }
			}
		}

		protected void PausePanelUpdate(UIPanel.PauseType type = UIPanel.PauseType.All, bool selfValue = true, float wait = 2f, System.Action callback = null)
		{
			FetchPanelWhileNull();

			if (CurrentPanel != null)
			{
				//EB.Debug.Log("<color=yellow>PausePanelUpdate (name, type)=> " + CurrentPanel.name + ", " + type + "</color>");
				//ILRTimerManager.instance.AddTimer(Mathf.RoundToInt(1000 * wait), 1, delegate(int sequence)
				//{
					if (CurrentPanel != null) CurrentPanel.SetPause(type, selfValue);
					callback?.Invoke();
				//});
			}
		}

		protected void ResumePanelUpdate(bool autoDisable = false, UIPanel.PauseType type = UIPanel.PauseType.Others, bool pauseValue = false, float wait = 1f)
		{
			Current = this;

			FetchPanelWhileNull();

			//if (CurrentPanel) EB.Debug.Log("<color=yellow>ResumePanelUpdate (auto_pause, name)=> " + autoDisable + ","+ CurrentPanel.name + "</color>");
			if (CurrentPanel) CurrentPanel.IsPause = false;
			if (autoDisable) PausePanelUpdate(type, pauseValue, wait);
		}
		#endregion
	}
}