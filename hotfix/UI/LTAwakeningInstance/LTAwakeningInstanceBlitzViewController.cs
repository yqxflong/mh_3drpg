using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceBlitzViewController : UIControllerHotfix
    {
        public LTAwakeningInstanceBlitzDynamicScroll DynamicScroll;
        public GameObject BtnRootObj;
        public List<LTAwakeningInstanceBlitzData> DataList;
        private Hotfix_LT.Data.AwakenDungeonTemplate m_AwakenDungeonTemplate;
        public override bool IsFullscreen() { return false ; }
        public override bool ShowUIBlocker { get { return true; } }
        public UILabel UiLabelEnterVigor;
        private bool canClose;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<LTAwakeningInstanceBlitzDynamicScroll>("Scroll/PlaceHolder/Grid");
            BtnRootObj = t.FindEx("BtnRoot").gameObject;
            controller.backButton = t.GetComponent<UIButton>("Bg/Top/CancelBtn");
            UiLabelEnterVigor = t.GetComponent<UILabel>("BtnRoot/OnceBtn/Sprite/New");
            t.GetComponent<UIButton>("Bg/Top/CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("BtnRoot/OnceBtn").clickEvent.Add(new EventDelegate(OnOnceBtnClick));
            t.GetComponent<UIButton>("BtnRoot/MoreBtn").onClick.Add(new EventDelegate(OnMoreBtnClick));
            
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            canClose = false;
            BtnRootObj.CustomSetActive(false);
            m_AwakenDungeonTemplate = (Hotfix_LT.Data.AwakenDungeonTemplate) param;
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            DataList = new List<LTAwakeningInstanceBlitzData>();
            InitUI();
            //进入时候扫荡一次
            OnOnceBtnClick();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        private void InitUI()
        {
            DynamicScroll.SetItemDatas(DataList.ToArray());
            StartCoroutine(SetScrollState(DataList.Count));
            LTUIUtil.SetText(UiLabelEnterVigor,LTAwakeningInstanceConfig.GetNeedEnterVigor().ToString());
        }
    
        public WaitForSeconds WaitTime = new WaitForSeconds(0.15f);
    
        private IEnumerator SetScrollState(int moveCount)
        {
            yield return WaitTime;
            Hotfix_LT.Messenger.Raise<int>(EventName.LTBlitzCellTweenEvent, 1);

            for (int i = 0; i < moveCount - 1; i++)
            {
                yield return WaitTime;
                DynamicScroll.MoveInternalNow(i);
                Hotfix_LT.Messenger.Raise<int>(EventName.LTBlitzCellTweenEvent, i + 2);
            }

            BtnRootObj.CustomSetActive(true);
        }
    
        private IEnumerator ResetScroll()
        {
            DataList = LTInstanceUtil.GetBlitzDataChange();
            DynamicScroll.Clear();
            yield return null;
            InitUI();
        }
    
        public override void OnCancelButtonClick()
        {
            if (canClose==false)
            {
                return;
            }
            base.OnCancelButtonClick();
        }
    
        public void OnOnceBtnClick()
        {
            if (BalanceResourceUtil.EnterVigorCheck(LTAwakeningInstanceConfig.GetNeedEnterVigor()))
            {
                //   MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_6"));
                return;
            }
    
            if (LTAwakeningInstanceConfig.AwakeningIsLock(m_AwakenDungeonTemplate.Type))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_7"));
                return;
            }
    
            int uid = 0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.uid", out uid);
    
            LTAwakeningInstanceManager.Instance.Blitz(uid,m_AwakenDungeonTemplate.ID,1, () =>
            {
                StartCoroutine(ResetScroll());
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnEnterTimeChange);
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);//装备数量发生变化需要通知发送下
                canClose = true;
            });
        }
    
        public void OnMoreBtnClick()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.ticket", out num);
    
            int uid = 0;
            DataLookupsCache.Instance.SearchIntByID("userAwakenCampaign.uid", out uid);
    
            if (num < LTAwakeningInstanceConfig.GetCost())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_6"));
                return;
            }
            
            if (LTAwakeningInstanceConfig.AwakeningIsLock(m_AwakenDungeonTemplate.Type))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_7"));
                return;
            }
    
            num = num >= 10 * LTAwakeningInstanceConfig.GetCost() ? 10 *LTAwakeningInstanceConfig.GetCost() : num / LTAwakeningInstanceConfig.GetCost();
            LTAwakeningInstanceManager.Instance.Blitz(uid, m_AwakenDungeonTemplate.ID, num, () =>
            {
                StartCoroutine(ResetScroll());
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);//装备数量发生变化需要通知发送下
            });
        }
    }
}
