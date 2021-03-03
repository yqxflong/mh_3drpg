using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTChargeLimitTimeNoticeHudController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DiscountLab = t.GetComponent<UILabel>("Content/Discount/DiscountLab");
            PriceValueLab = t.GetComponent<UILabel>("Content/PriceBtn/PriceValue");
            DescriptLab = t.GetComponent<UILabel>("Content/Tip/Label");
            OverTimeLab = t.GetComponent<UILabel>("Content/TimeLabel");
            ItemsGrid = t.GetComponent<UIGrid>("Content/GiftGrid");
            ItemsList = new List<LTShowItem> ();
            for(int i=0;i< ItemsGrid.transform.childCount; ++i)
            {
                ItemsList.Add(ItemsGrid.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/PriceBtn").clickEvent.Add(new EventDelegate(OnBtnClick));

        }


    
        public UILabel DiscountLab;
        public UILabel PriceValueLab;
        public UILabel DescriptLab;
        public UILabel OverTimeLab;
        public List<LTShowItem> ItemsList;
        public UIGrid ItemsGrid;
    
        private LTChargeLimitTimeGiftData Data;
        private EB.IAP.Item curChargeData;
        private List<LTShowItemData> curItemDataList;
        private bool IsTimeOver;
    
        public override bool IsFullscreen() { return false; }
        public override bool ShowUIBlocker { get { return true; } }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Data = (LTChargeLimitTimeGiftData)param;
            InitUI(Data);
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            FusionAudio.PostEvent("UI/New/XianShiLiBao");
            Hotfix_LT.Messenger.AddListener < EB.IAP.Item, EB.IAP.Transaction  > ("OnOfferPurchaseSuceeded", OnOfferPurchaseSuceeded);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener<EB.IAP.Item, EB.IAP.Transaction > ("OnOfferPurchaseSuceeded", OnOfferPurchaseSuceeded);
            controller.GetComponent<TweenScale>().ResetToBeginning();
            DestroySelf();
            yield break;
        }
    
        private void InitUI(LTChargeLimitTimeGiftData itemData)
        {
            if (itemData != null)
            {
                Data = itemData;
                EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(Data.PaymentId, out curChargeData);
    
                DiscountLab.text = LTChargeManager.GetDiscountTextEx(curChargeData.discount);
                DiscountLab.transform.parent.gameObject.CustomSetActive(curChargeData.discount > 0);
    
                PriceValueLab.text = curChargeData.localizedCost;
    
                DescriptLab.text = curChargeData.localizedDesc;
    
                InitChargeShowData();
    
                InitTimeLabel();

                controller.gameObject.CustomSetActive(true);
            }
        }
    
    
        private void InitTimeLabel()
        {
            int ts = Data.OverTime - EB.Time.Now;
            if (ts < 0)
            {
                IsTimeOver = true;
                controller.Close();
            }
            else
            {
                IsTimeOver = false;
            }
        }
    
        private void InitChargeShowData()
        {
            if (curItemDataList == null)
            {
                curItemDataList = new List<LTShowItemData>();
            }
            curItemDataList.Clear();
            for (int i = 0; i < curChargeData.redeemers.Count; ++i)
            {
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(curChargeData.redeemers[i].Type))
                {
                    LTShowItemData tempData = new LTShowItemData(curChargeData.redeemers[i].Data, curChargeData.redeemers[i].Quantity, curChargeData.redeemers[i].Type);
                    curItemDataList.Add(tempData);
                }
            }
    
            for (int i = 0; i < ItemsList.Count; i++)
            {
                if (i < curItemDataList.Count)
                {
                    ItemsList[i].mDMono.gameObject.CustomSetActive(true);
                    ItemsList[i].LTItemData = curItemDataList[i];
                }
                else
                {
                    ItemsList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            ItemsGrid.repositionNow =true;
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            //时间更新
            if (!IsTimeOver)
            {
                int ts = Data.OverTime- EB.Time.Now;
                if (ts < 0)
                {
                    IsTimeOver = true;
                    ts = 0;
                }
                string timeStr = "";
                timeStr = (ts > 0) ? (string.Format("{0:D2}:{1:D2}:{2:D2}", ts / (60 * 60), ts % (60 * 60) / 60, ts % 60)) : "00:00:00";
                OverTimeLab.text = string.Format(EB.Localizer.GetString("ID_CHARGE_LIMIT_TIME_OVER_TIME"), timeStr);
            }
        }
    
        public void OnBtnClick()
        {
            if (IsTimeOver) return;
            curChargeData.LimitedTimeGiftId = Data.TriggerID;
            LTChargeManager.Instance.CurLimitTimeGiftLocalWeighting = string.Format("{0}:{1}" ,Data.PaymentId, Data.OverTime);
            LTChargeManager.Instance.PurchaseOfferExpand(curChargeData, LTChargeStoreController.EventTable);
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
    
            LTChargeManager.Instance.ReflashLimitedTimeGiftInfo(false,true);
    
            controller.Close();
        }
    }
}
