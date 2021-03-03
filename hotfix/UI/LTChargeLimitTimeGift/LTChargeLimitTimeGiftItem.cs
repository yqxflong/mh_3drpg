using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChargeLimitTimeGiftItem : DynamicCellController<LTChargeLimitTimeGiftData>, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSp = t.GetComponent<UISprite>("BG/IconObj/Icon");
            NameLab = t.GetComponent<UILabel>("NameLabel");
            DiscountLab = t.GetComponent<UILabel>("Discount/DiscountLab");
            PriceValueLab = t.GetComponent<UILabel>("PriceBtn/PriceValue");
            TagLab = t.GetComponent<UILabel>("Tag/Label");
            TagObj = t.FindEx("Tag").gameObject;
            OverTimeLab = t.GetComponent<UILabel>("TimeLabel");
            ItemsGrid = t.GetComponent<UIGrid>("GiftGrid");
            ItemsList = new List<LTShowItem>();
            for(int i=0;i< ItemsGrid.transform.childCount; ++i)
            {
                ItemsList.Add(ItemsGrid.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }
            t.GetComponent<ConsecutiveClickCoolTrigger>("PriceBtn").clickEvent.Add(new EventDelegate (OnBtnClick));
        }

        public UISprite IconSp;
        public UILabel NameLab;
        public UILabel DiscountLab;
        public UILabel PriceValueLab;
    
        public UILabel TagLab;
        public GameObject TagObj;
    
        public UILabel OverTimeLab;
        public List<LTShowItem> ItemsList;
        public UIGrid ItemsGrid;
        
        private LTChargeLimitTimeGiftData Data;
        private EB.IAP.Item curChargeData;
        private List<LTShowItemData> curItemDataList;
        private bool IsTimeOver;

		public override void OnEnable()
		{
			//base.OnEnable();
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public override void Clean()
        {
            Data = null;
            curChargeData = null;
            IsTimeOver = true;
            OverTimeLab.text = string.Empty;
            mDMono.gameObject.CustomSetActive(false);
        }
    
        public override void Fill(LTChargeLimitTimeGiftData itemData)
        {
            if (itemData != null)
            {
                Data = itemData;
                EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(Data.PaymentId, out curChargeData);
    
                IconSp.spriteName = curChargeData.icon;
    
                NameLab.text = curChargeData.longName;
    
                DiscountLab.text = LTChargeManager.GetDiscountTextEx(curChargeData.discount);
                DiscountLab.transform.parent.gameObject.CustomSetActive(curChargeData.discount > 0 );
    
                PriceValueLab.text = curChargeData.localizedCost;
    
                ShowTag();
    
                InitChargeShowData();
    
                InitTimeLabel();

                mDMono.gameObject.CustomSetActive(true);
    
            }
            else
            {
                mDMono.gameObject.CustomSetActive(false);
            }
        }
        
        private void InitTimeLabel()
        {
            int ts = Data.OverTime - EB.Time.Now;
            if (ts < 0)
            {
                IsTimeOver = true;
                Clean();
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
            ItemsGrid.Reposition();
        }
    
        private void ShowTag()
        {
            TagObj.CustomSetActive(true);
            int buylimit = 0;
            if (curChargeData.buyLimit > 1)
            {
                buylimit = curChargeData.buyLimit;
            }
            else if (curChargeData.dayBuyLimit > 1)
            {
                buylimit = curChargeData.dayBuyLimit;
            }
            else if (curChargeData.weeklyBuyLimit > 1)
            {
                buylimit = curChargeData.weeklyBuyLimit;
            }
            else if (curChargeData.monthlyBuyLimit > 1)
            {
                buylimit = curChargeData.monthlyBuyLimit;
            }
            if (buylimit > 1)
            {
                TagLab.text = string.Format(EB.Localizer.GetString("ID_PURCHASE_LIMIT"), Mathf.Min(Data.OverTimeList.Count, buylimit));
            }
            else
            {
                TagLab.text = string.Format(EB.Localizer.GetString("ID_PURCHASE_LIMIT"), Data.OverTimeList.Count); //Localizer.GetString("ID_CHARGE_BUYONLYONE");
            }
        }

        public void Update()
        {
            if (Data == null) return;
            //时间更新
            if(!IsTimeOver)
            {
                int ts = Data.OverTime - EB.Time.Now;
                if (ts < 0)
                {
                    IsTimeOver = true;
                    ts = 0;
                }
                string timeStr = "";
                timeStr = (ts > 0) ? (string.Format("{0:D2}:{1:D2}:{2:D2}", ts / (60 * 60), ts % (60 * 60) / 60, ts % 60)) : "00:00:00";
                OverTimeLab.text = string.Format(EB.Localizer.GetString("ID_CHARGE_LIMIT_TIME_OVER_TIME"), timeStr);
            }
            else
            {
                if (Data.OverTimeList.Count > 1)
                {
                    IsTimeOver = false;
                    Data.SetNext();
                    ShowTag();
                }
            }
        }
    
        public void OnBtnClick()
        {
            if (IsTimeOver) return;
            curChargeData.LimitedTimeGiftId = Data.TriggerID;
            LTChargeManager.Instance.CurLimitTimeGiftLocalWeighting = string.Format("{0}_{1}", Data.PaymentId, Data.OverTime);
            LTChargeManager.Instance.PurchaseOfferExpand(curChargeData, LTChargeStoreController.EventTable);
        }
    }
}
