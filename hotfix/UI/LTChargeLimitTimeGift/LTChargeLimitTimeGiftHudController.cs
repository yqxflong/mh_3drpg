using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChargeLimitTimeMainTimeLabelController:DynamicMonoHotfix
    {
        private UILabel OverTimeLab;
        public override void Awake()
        {
            OverTimeLab = mDMono.GetComponent<UILabel>();
        }

        private int timer = 0;
        public override void OnEnable()
        {
            base.OnEnable();
            timer = ILRTimerManager.instance.AddTimer(1000, int.MaxValue , UpdateTimer);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            RemoveTimer();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveTimer();
        }

        void RemoveTimer()
        {
            if (timer > 0)
            {
                ILRTimerManager.instance.RemoveTimer(timer);
                timer = 0;
            }
        }

        void UpdateTimer(int tm)
        {
            int ts = LTChargeManager.Instance.GetLTGLateOverTime() - EB.Time.Now;
            string colorStr = string.Empty;
            if (ts < 0)
            {
                ts = 0;
                colorStr = "[ff6699]";
                LTChargeManager.Instance.ReflashLimitedTimeGiftInfo();
                if (!LTChargeManager.Instance.HasLimitedTimeGift())
                {
                    RemoveTimer();
                    mDMono.gameObject.CustomSetActive(false);
                }
            }
            string timeStr = "";
            timeStr = (ts > 0) ? (string.Format("{0:D2}:{1:D2}:{2:D2}", ts / (60 * 60), ts % (60 * 60) / 60, ts % 60)) : "00:00:00";
            OverTimeLab.text = timeStr;
        }
    }

    public class LTChargeLimitTimeGiftHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            DynamicScrool = t.GetMonoILRComponent<LTChargeLimitTimeGiftScroll>("Content/ScrollView/PlaceHolder/Grid");
            controller.backButton = t.GetComponent<UIButton>("LeftTop/CancelBtn");
        }

        public LTChargeLimitTimeGiftScroll DynamicScrool;
    
        public override bool IsFullscreen() { return true; }
        public override bool ShowUIBlocker { get { return false; } }
        
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            yield return null;
            SetData();
            Hotfix_LT.Messenger.AddListener("OnOffersFetchSuceeded", SetData);
            Hotfix_LT.Messenger.AddListener<EB.IAP .Item, EB.IAP.Transaction > ("OnOfferPurchaseSuceeded", OnOfferPurchaseSuceeded);
        }
    
        public override void OnFocus()
        {
            base.OnFocus();
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener("OnOffersFetchSuceeded", SetData);
            Hotfix_LT.Messenger.RemoveListener<EB.IAP.Item, EB.IAP.Transaction > ("OnOfferPurchaseSuceeded", OnOfferPurchaseSuceeded);
            DestroySelf();
            yield break;
        }
    
        private void SetData()
        {
            List<LTChargeLimitTimeGiftData> datalist = LTChargeManager.Instance.GetLTChargeLimitTimeGiftList();
            if (datalist != null)
            {
                var list = datalist.ToArray();
                DynamicScrool.SetItemDatas(list);
                if (controller.gameObject != null && datalist.Count == 0) controller.Close();
            }
            else
            {
                if (controller.gameObject != null) controller.Close();
            }
        }
    
        /// <summary>
        /// 商城礼包购买成功调用
        /// </summary>
        /// <param name="item"></param>
        private void OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans)
        {
            if (!UIStack.Instance.IsStacked(this.controller))
            {
                EB.Debug.Log("OnOfferPurchaseSuceeded——return!");
                return;
            }
            EB.Debug.Log("OnOfferPurchaseSuceeded");
            
            List<LTShowItemData> itemDataList = new List<LTShowItemData>();
            for (int i = 0; i < item.redeemers.Count; i++)
            {
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(item.redeemers[i].Type))
                {
                    LTShowItemData tempData = new LTShowItemData(item.redeemers[i].Data, item.redeemers[i].Quantity, item.redeemers[i].Type);
                    itemDataList.Add(tempData);
                }
            }
            if (itemDataList.Count > 0) GlobalMenuManager.Instance.Open("LTShowRewardView", itemDataList);
    
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_SUCC"));
    
            LTChargeManager.Instance.ReflashLimitedTimeGiftInfo(false,true );
        }
    }
}
