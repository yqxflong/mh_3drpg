using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
    
    /// <summary>
    /// 
    /// </summary>
namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceHudController : UIControllerHotfix
    {
        public List<LTAwakeningInstanceTypeItem> AwakeningInstanceList = new List<LTAwakeningInstanceTypeItem>();

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("Edge/TopRight/CancelBtn");

            AwakeningInstanceList.Add(t.GetMonoILRComponent<LTAwakeningInstanceTypeItem>("Center/Grid/Wind"));
            AwakeningInstanceList.Add(t.GetMonoILRComponent<LTAwakeningInstanceTypeItem>("Center/Grid/Fire"));
            AwakeningInstanceList.Add(t.GetMonoILRComponent<LTAwakeningInstanceTypeItem>("Center/Grid/Water"));

            t.GetComponent<UIButton>("Edge/TopLeft/NewCurrency/Table/Notice").onClick.Add(new EventDelegate(OnRuleBtnClick));
        }

    
        public override bool IsFullscreen() { return true; }
        public override bool ShowUIBlocker { get { return false; } }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            RefreshCost();
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            UpdateUI();
           
            Messenger.AddListener(Hotfix_LT.EventName.AwakeningInstance_UpdataTime, UpdateUI);
            GlobalMenuManager.Instance.PushCache("LTAwakeningInstanceHud");
        }
        
        public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTAwakeningInstanceHud");
            if (GlobalMenuManager.Instance.GetMenuByPrefabName("LTInstanceMapHud")!=null)
            {
                GlobalMenuManager.Instance.GetMenuByPrefabName("LTInstanceMapHud").HasPlayedTween = false;
            }
			base.OnCancelButtonClick();
        }
    
    
        public override IEnumerator OnRemoveFromStack()
        {
            Messenger.RemoveListener(Hotfix_LT.EventName.AwakeningInstance_UpdataTime, UpdateUI);
            ResetUI();
            DestroySelf();
            yield break;
        }
    
        private void UpdateUI()
        {
            for (int i = 0; i < AwakeningInstanceList.Count; i++)
            {
                AwakeningInstanceList[i].UpdateUI();
            }
        }
    
        private void ResetUI()
        {
            for (int i = 0; i < AwakeningInstanceList.Count; i++)
            {
                AwakeningInstanceList[i].ResetUI();
            }
        }
    
        public void OnRuleBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_AWAKENING_INSTANCE"));
        }
    
        private void RefreshCost()
        {
            if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.AwakenStone))
            {
                int uid = 0;
                DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.uid", out uid);
                LTAwakeningInstanceManager.Instance.RequestTrialStone(uid, null);
                
                UpdateUI();
            }
           
        }
    }
}
