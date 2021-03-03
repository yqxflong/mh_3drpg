using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTGeneralInstanceHudController : UIControllerHotfix
    {
        private static LTGeneralInstanceHudController _instance = null;
        public static LTGeneralInstanceHudController Instance { get { return _instance; } }

        public LTInstanceMapCtrl MapCtrl;
        public CampaignTextureCmp MaskTex;
        public RenderSettings RenderSetting;
        public UIPanel uiEdgPanel;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            MapCtrl = t.GetMonoILRComponentByClassPath<LTInstanceMapCtrl>("Hotfix_LT.UI.LTInstanceMapCtrl");
            MaskTex = t.GetComponent<CampaignTextureCmp>("Edge/Mask/MaskBG");
            RenderSetting = t.GetComponent<RenderSettings>("LTInstanceRenderSetting");
            uiEdgPanel = t.GetComponent<UIPanel>("Edge");
            controller.FindAndBindingBtnEvent(new List<string>(2) { "Edge/TopLeft/FriendBtn", "Edge/TopLeft/ChatBtn" },
                new List<EventDelegate>(2) { new EventDelegate(OnFriendBtnClick), new EventDelegate(OnChatBtnClick) });
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _instance = this;
            Hotfix_LT.Messenger.AddListener(EventName.OnEventsDataUpdate, EventsDataUpdateFunc);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _instance = null;
            Hotfix_LT.Messenger.RemoveListener(EventName.OnEventsDataUpdate, EventsDataUpdateFunc);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        
        public override void Show(bool isShowing)
        {
            if (!isShowing)
            {
                uiEdgPanel.alpha = 0;
                return;
            }
            else
            {
                uiEdgPanel.alpha = 1;
            }
            base.Show(isShowing);
        }

        public override void OnFocus()
        {
            base.OnFocus();
            if(RenderSetting!=null) RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSetting.name, RenderSetting);
        }

        protected virtual void InitMap()
        {
            LTInstanceMapModel.Instance.RequestGetChapterState();
            MapCtrl.InitMap();
        }

        protected virtual void InitUI()
        {

        }

        protected virtual void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView");
        }

        protected virtual void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud");
        }

        public virtual void OnFloorClickFunc(LTInstanceNode NodeData, Transform Target)
        {

        }

        private void EventsDataUpdateFunc()
        {
            StartCoroutine(WaitForPlayer());
        }

        protected virtual IEnumerator WaitForPlayer()
        {
            yield return null;
        }
    }
}
