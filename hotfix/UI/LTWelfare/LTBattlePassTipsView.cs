using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    /// <summary>
    /// 密令购买提示窗
    /// </summary>
    public class LTBattlePassTipsView : UIControllerHotfix
    {
        public UILabel TitleLab;
        public UILabel DiscountLab;
        public UILabel DescLab;
        public UILabel MoneyPriceLab;
        public UIGrid Grid;
        public UIScrollView ScrollView;
        public GameObject ItemObj;
    
        private EGiftType curGiftType;
        private List<LTShowItem> ShowItemList;
        private EB.IAP.Item curChargeData;//商城礼包的数据
    
        private List<LTShowItemData> curItemDataList;//当前显示的物品数据
    
        private List<ChargeGiftData> giftDataList;//抽奖礼包的数据
    
        private int curGiftIndex = 0;//当前的礼包序号（用于抽奖礼包）
    
        /// <summary>
        /// 最大item显示数量（超过了需要滚动）
        /// </summary>
        private const int MAXSHOWCOUNT = 12;
    
        public override bool ShowUIBlocker
        {
            get { return true; }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TitleLab = t.GetComponent<UILabel>("Container/Content/Tips");
            DiscountLab = t.GetComponent<UILabel>("Container/Content/Tips");
            DescLab = t.GetComponent<UILabel>("Container/Content/Context");
            MoneyPriceLab = t.GetComponent<UILabel>("Container/Content/SureBtn/Money");
            Grid = t.GetComponent<UIGrid>("Container/Content/Scroll/GiftGrid");
            ScrollView = t.GetComponent<UIScrollView>("Container/Content/Scroll");
            ItemObj = t.FindEx("Container/Content/Item").gameObject;

            t.GetComponent<UIButton>("Container/Content/Title/CloseBtn").onClick.Add(new EventDelegate(controller.Close));
            t.GetComponent<UIButton>("Container/Content/SureBtn").onClick.Add(new EventDelegate(OnClickBuyBtn));

            Hotfix_LT.Messenger.AddListener<EB.IAP.Item, EB.IAP.Transaction>(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, OnOfferPurchaseSuceeded);
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<EB.IAP.Item, EB.IAP.Transaction>(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, OnOfferPurchaseSuceeded);
            base.OnDestroy();
        }

        public override void SetMenuData(object param)
        {
            if (param == null)
            {
                return;
            }

            base.SetMenuData(param);

            if (param is EB.IAP.Item)
            {
                curGiftType = EGiftType.eCharge;
                curChargeData = param as EB.IAP.Item;

                //设置商品价钱
                if (MoneyPriceLab != null && curChargeData != null)
                {
                    MoneyPriceLab.text = curChargeData.localizedCost;
                }

                //设置奖励图标
                if (ShowItemList == null)
                { 
                    //写死的密令奖励展示
                    string str = Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("BattlePassTips");
                    string[] allItemInfos = str.Split(',');
                    ShowItemList = new List<LTShowItem>();
                    string[] itemInfo;

                    if (ItemObj == null)
                    {
                        ItemObj = controller.transform.FindEx("Container/Content/Item").gameObject;
                    }

                    for (int i = 0; i < allItemInfos.Length; i++)
                    {
                        itemInfo = allItemInfos[i].Split(':');
                        GameObject itemObj = GameObject.Instantiate(this.ItemObj, Grid.transform);
                        LTShowItem tempItem = itemObj.transform.Find("LTShowItem").GetMonoILRComponent<LTShowItem>();
                        itemObj.SetActive(true);
                        tempItem.LTItemData = new LTShowItemData(itemInfo[0], int.Parse(itemInfo[2]), itemInfo[1], false);
                        ShowItemList.Add(tempItem);
                    }

                    if (Grid != null)
                    {
                        Grid.enabled = true;
                        Grid.Reposition();
                    }

                    if (ScrollView != null)
                    {
                        SpringPanel.Begin(ScrollView.gameObject, new Vector3(0, -40, 0), 13.0f);
                        ScrollView.enabled = allItemInfos.Length > 12;
                    }
                }
            }
            else
            {
               EB.Debug.LogError( "[{0}]为什么传入的数据类型不是 EB.IAP.Item？ param.GetType():{1}" , Time.frameCount, param.GetType());
            }
        }
    
        /// <summary>
        /// 刷新抽奖礼包
        /// </summary>
        private void RefreshDrawUI(ChargeGiftData data)
        {
            TitleLab.text = data.title;
    
            string colorStr = BalanceResourceUtil.GetUserDiamond() >= data.coinValue ? LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
    
            DiscountLab.text = LTChargeManager.GetDiscountText(data.discount);
            DiscountLab.transform.parent.gameObject.CustomSetActive(data.discount > 0 && data.discount < 1);
    
            curItemDataList = data.rewardList;
            RefreshShowItem();
        }
    
        /// <summary>
        /// 刷新Item显示数据
        /// </summary>
        private void RefreshShowItem()
        {
            int num = curItemDataList.Count - ShowItemList.Count;
            if (num > 0)
            {
                GameObject obj = GameObject.Instantiate(ShowItemList[0].mDMono.gameObject);
                obj.transform.SetParent(Grid.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                LTShowItem tempItem = obj.GetMonoILRComponent<LTShowItem>();
                ShowItemList.Add(tempItem);
            }
            else
            {
    
            }
    
            for (int i = 0; i < ShowItemList.Count; i++)
            {
                if (i < curItemDataList.Count)
                {
                    ShowItemList[i].mDMono.gameObject.CustomSetActive(true);
                    ShowItemList[i].LTItemData = curItemDataList[i];
                }
                else
                {
                    ShowItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            Grid.enabled = true;
    
            if (curItemDataList.Count <= MAXSHOWCOUNT && ScrollView.enabled)
            {
                ScrollView.enabled = false;
                ResetScoll();
            }
    
            if (curItemDataList.Count > MAXSHOWCOUNT)
            {
                ScrollView.enabled = true;
                ScrollView.ResetPosition();
            }
        }
    
        /// <summary>
        /// 重置滚动条状态
        /// </summary>
        private void ResetScoll()
        {
            if (ScrollView.GetComponent<SpringPanel>() != null)
            {
                Object.Destroy(ScrollView.GetComponent<SpringPanel>());
            }
    
            ScrollView.transform.localPosition = Vector3.zero;
            ScrollView.panel.clipOffset = Vector2.zero;
        }
    
        /// <summary>
        /// 初始化ShowItem数据
        /// </summary>
        private void InitChargeShowData()
        {
            if (curItemDataList == null)
            {
                curItemDataList = new List<LTShowItemData>();
            }
            curItemDataList.Clear();
    
            for (int i = 0; i < curChargeData.redeemers.Count; i++)
            {
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(curChargeData.redeemers[i].Type))//!curChargeData.redeemers[i].Type.Equals("bpt") && !curChargeData.redeemers[i].Type.Equals("pab") && !curChargeData.redeemers[i].Type.Equals("pdb") && !curChargeData.redeemers[i].Type.Equals("act"))
                {
                    LTShowItemData tempData = new LTShowItemData(curChargeData.redeemers[i].Data, curChargeData.redeemers[i].Quantity, curChargeData.redeemers[i].Type, false);
                    curItemDataList.Add(tempData);
                }
            }
        }
    
        /// <summary>
        /// 点击商城礼包购买按钮
        /// </summary>
        public void OnClickBuyBtn()
        {
            LTChargeManager.Instance.PurchaseOfferExpand(curChargeData, LTChargeStoreController.EventTable);
        }
    
        /// <summary>
        /// 商城礼包购买成功调用
        /// </summary>
        /// <param name="item"></param>
        private void OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans)
        {
            if (!UIStack.Instance.IsStacked(controller))
            {
                EB.Debug.Log("OnOfferPurchaseSuceeded——return!");
                return;
            }
            EB.Debug.Log("OnOfferPurchaseSuceeded");
            List<LTShowItemData> itemDataList = new List<LTShowItemData>();
            for (int i = 0; i < item.redeemers.Count; i++)
            {
                if (!LTChargeManager.Instance .IgnoreItemList.Contains(item.redeemers[i].Type))//item.redeemers[i].Type.CompareTo("mcard") != 0 && item.redeemers[i].Type.CompareTo("bpt") != 0 && item.redeemers[i].Type.CompareTo("pab") != 0 && item.redeemers[i].Type.CompareTo("pdb") != 0)
                {
                    LTShowItemData tempData = new LTShowItemData(item.redeemers[i].Data, item.redeemers[i].Quantity, item.redeemers[i].Type);
                    itemDataList.Add(tempData);
                }
            }
            if (itemDataList.Count > 0) GlobalMenuManager.Instance.Open("LTShowRewardView", itemDataList);
    
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_SUCC"));
    
            controller.Close();
        }
    
        /// <summary>
        /// 点击购买抽奖礼包按钮
        /// </summary>
        public void OnCilckDiamondBuyBtn()
        {
            if (giftDataList.Count <= curGiftIndex)
            {
                EB.Debug.LogError("LTChargeGiftRewadUI OnCilckDiamondBuyBtn is Error, curGiftIndex : {0}, giftDataList.Count : {1}", curGiftIndex, giftDataList.Count);
                return;
            }
    
            ChargeGiftData data = giftDataList[curGiftIndex];
            if (BalanceResourceUtil.GetUserDiamond() < data.coinValue)
            {
                BalanceResourceUtil.HcLessMessage(delegate { controller.Close(); });
                return;
            }
    
            LTChargeManager.Instance.ReceiveBuyGift(data.id, delegate
            {
                GlobalMenuManager.Instance.Open("LTShowRewardView", data.rewardList);
    
                giftDataList.RemoveAt(curGiftIndex);
    
                if (giftDataList.Count <= 0)
                {
                    ClearGiftData();
                    controller.Close();
                    return;
                }
    
                curGiftIndex = 0;
                RefreshDrawUI(giftDataList[0]);
            });
        }
    
        /// <summary>
        /// 清理本地Dache缓存的礼包数据（在全部购买完毕或者全部过时的情况下才会调用）
        /// </summary>
        private void ClearGiftData()
        {
            DataLookupsCache.Instance.CacheData("userCultivateGift.gifts", null);
        }
    
        public void OnMoreGiftBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTChargeStoreHud", EChargeType.eGift);
            controller.Close();
        }
    }
}
