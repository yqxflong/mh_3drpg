using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public enum EGiftType
    {
        eCharge,//充值礼包
        eDraw,//抽奖礼包
    }
    
    public class ChargeGiftData
    {
        public int tplId;//表格数据id
        public string id;//服务器唯一id
        public string title;//标题
        public string icon;//图标
        public string discountStr;// *暂时不用*
        public float discount;//折扣
        public int coinValue;//价格
        public int countDownEnd;//结束时间
        public List<LTShowItemData> rewardList;//奖励
    }
    
    public class LTChargeGiftRewardUI : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            IconSp = t.GetComponent<UISprite>("Content/IconObj/Icon");
            TitleLab = t.GetComponent<UILabel>("Content/Title");
            DiscountLab = t.GetComponent<UILabel>("Content/Discount/DiscountLab");
            DescLab = t.GetComponent<UILabel>("Content/Message/GiftMessageLab");
            DescLab2 = t.GetComponent<UILabel>("Content/Message/GiftMessageLab (1)");
            MoneyPriceLab = t.GetComponent<UILabel>("Content/Buttons/MoneyBuyBtn/Price");
            DiamondPriceLab = t.GetComponent<UILabel>("Content/Buttons/DiamondBuyBtn/Price/Price");
            GiftCountLab = t.GetComponent<UILabel>("GiftCount");
            Grid = t.GetComponent<UIGrid>("Content/Gift/GiftScrollView/GiftGrid");
            ScrollView = t.GetComponent<UIScrollView>("Content/Gift/GiftScrollView");
            MoneyPriceObj = t.FindEx("Content/Buttons/MoneyBuyBtn").gameObject;
            DiamondPriceObj = t.FindEx("Content/Buttons/DiamondBuyBtn").gameObject;
            ArrowObj = t.FindEx("Arrow").gameObject;
            TagObj = t.FindEx("Content/Tag").gameObject;
            TagLab = t.GetComponent<UILabel>("Content/Tag/Label");
            TagBGSp = t.GetComponent<UISprite>("Content/Tag/BG");
            MoreGiftObj = t.FindEx("Content/Buttons/MoreGiftBtn").gameObject;
            BtnsGrid = t.GetComponent<UIGrid>("Content/Buttons");
            controller.backButton = t.GetComponent<UIButton>("Content/CloseBtn");

            t.GetComponent<UIButton>("Arrow/LeftArrow").onClick.Add(new EventDelegate(OnClickLeftArrow));
            t.GetComponent<UIButton>("Arrow/RightArrow").onClick.Add(new EventDelegate(OnClickRightArrow));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/Buttons/MoneyBuyBtn").clickEvent.Add(new EventDelegate(OnClickBuyBtn));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/Buttons/DiamondBuyBtn").clickEvent.Add(new EventDelegate(OnCilckDiamondBuyBtn));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/Buttons/MoreGiftBtn").clickEvent.Add(new EventDelegate(OnMoreGiftBtnClick));

        }

        public UISprite IconSp;
        public UILabel TitleLab;
        public UILabel DiscountLab;
        public UILabel DescLab;
        public UILabel DescLab2;
        public UILabel MoneyPriceLab;
        public UILabel DiamondPriceLab;
        public UILabel GiftCountLab;
        public UIGrid Grid;
        public UIScrollView ScrollView;
        public GameObject MoneyPriceObj;
        public GameObject DiamondPriceObj;
        public GameObject ArrowObj;
    
        public GameObject TagObj;
        public UILabel TagLab;
        public UISprite TagBGSp;
    
        public GameObject MoreGiftObj;
        public UIGrid BtnsGrid;
    
        private EGiftType curGiftType;
    
        private List<LTShowItem> ShowItemList;
    
        private EB.IAP.Item curChargeData;//商城礼包的数据
    
        private List<LTShowItemData> curItemDataList;//当前显示的物品数据
    
        private List<ChargeGiftData> giftDataList;//抽奖礼包的数据
        
        private int curGiftIndex = 0;//当前的礼包序号（用于抽奖礼包）
        private int curCountDownEnd;//当前礼包的结算时间
        private bool isShowCountDown;//是否开启倒计时
        private StringBuilder countDownStr = new StringBuilder();
    
        /// <summary>
        /// 最大item显示数量（超过了需要滚动）
        /// </summary>
        private const int MAXSHOWCOUNT = 4;
    
        public override bool ShowUIBlocker
        {
            get { return true; }
        }
    
    
        public override IEnumerator OnAddToStack()
        {
            Hotfix_LT.Messenger.AddListener< EB.IAP.Item  , EB.IAP.Transaction > ("OnOfferPurchaseSuceeded", OnOfferPurchaseSuceeded);
            return base.OnAddToStack();
            
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener < EB.IAP.Item , EB.IAP.Transaction > ("OnOfferPurchaseSuceeded", OnOfferPurchaseSuceeded);
            DestroySelf();
            yield break;
        }

		private float tempCountDown = 1;
        public void Update()
        {
            if (isShowCountDown)
            {
                tempCountDown -= EB.Time.deltaTime;
                if (tempCountDown <= 0)
                {
                    tempCountDown = 1;
                    RefreshDrawCountDown();
                }
            }
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
    
            InitShowItem();
    
            TagObj.CustomSetActive(false);
            DescLab2.text = string.Empty;
            if (param is EB.IAP.Item)
            {
                curGiftType = EGiftType.eCharge;
                curChargeData = param as EB.IAP.Item;
                ShowChargeUI(curChargeData);
                RefreshShowItem();
            }
            else
            {
                curGiftType = EGiftType.eDraw;
                InitDrawGiftData();
                SHowDrawUI();
            }
    
            ShowUIStatus();
    
            LTChargeManager.Instance.IsShowDrawGift = false;
        }
    
        /// <summary>
        /// 显示双倍限一次相关tag
        /// </summary>
        private void ShowTag()
        {
            if (curChargeData.categoryValue == (int)LTChargeStoreController.EChargeType.eCharge)
            {
                TagObj.CustomSetActive(curChargeData.twoMultiple);
                if (curChargeData.twoMultiple)
                {
                    TagLab.text = EB.Localizer.GetString("ID_uifont_in_LTBountyTaskHudUI_Label_5");
                    TagBGSp.spriteName = "Mall_Label_2";
                }
            }
            else
            {
                if (curChargeData.buyLimit == 1)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYONLYONE");
                    TagBGSp.spriteName = "Mall_Label_2";
                }
                else if (curChargeData.buyLimit > 1)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = string .Format ( EB.Localizer.GetString("ID_PURCHASE_LIMIT"), curChargeData.buyLimit);
                    TagBGSp.spriteName = "Mall_Label_2";
                }
                else if (curChargeData.dayBuyLimit > 0)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYEVERYDAY");
                    TagBGSp.spriteName = "Mall_Label_1";
                }else if (curChargeData.weeklyBuyLimit > 0)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYEVERYWEEK");
                    TagBGSp.spriteName = "Mall_Label_1";
                }else if (curChargeData.monthlyBuyLimit>0)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYEVERYMONTH");
                    TagBGSp.spriteName = "Mall_Label_1";
                }
                else
                {
                    TagObj.CustomSetActive(false);
                }
            }
        }
    
        /// <summary>
        /// 显示UI状态
        /// </summary>
        private void ShowUIStatus()
        {
            isShowCountDown = curGiftType == EGiftType.eDraw;
			if(isShowCountDown)
			{
				RegisterMonoUpdater();
			}
            MoneyPriceObj.CustomSetActive(curGiftType == EGiftType.eCharge);
            DiamondPriceObj.CustomSetActive(curGiftType == EGiftType.eDraw);
            ArrowObj.CustomSetActive(curGiftType == EGiftType.eDraw&&giftDataList!=null && giftDataList.Count>1);
            GiftCountLab.gameObject.CustomSetActive(curGiftType == EGiftType.eDraw);
            MoreGiftObj.CustomSetActive(!LTChargeStoreController.isOpen && (curChargeData != null && curChargeData.categoryValue == 1));
            MoneyPriceObj.transform.localPosition = DiamondPriceObj.transform.localPosition = new Vector3((MoreGiftObj.activeSelf) ? -(BtnsGrid.cellWidth / 2) : 0, 0, 0);
        }
    
        /// <summary>
        /// 显示商城礼包UI
        /// </summary>
        /// <param name="data"></param>
        private void ShowChargeUI(EB.IAP.Item data)
        {
            if (data == null)
            {
                EB.Debug.LogError("LTChargeGiftRewardUI ShowUI Charge data is Null!!!");
                controller.Close();
                return;
            }
    
            RefreshChargeUI();
            ShowTag();
        }
    
        /// <summary>
        /// 刷新商城礼包UI
        /// </summary>
        private void RefreshChargeUI()
        {
            InitChargeShowData();
            TitleLab.text = curChargeData.longName;
            MoneyPriceLab.text = curChargeData.localizedCost;
            if (curItemDataList == null || curItemDataList.Count == 0)
            {
                DescLab.text = string.Empty;
                DescLab2.text = curChargeData.localizedDesc;
            }
            else
            {
                DescLab2.text = string.Empty;
                DescLab.text = curChargeData.localizedDesc;
            }
            IconSp.spriteName = curChargeData.icon;
            if (IconSp.keepAspectRatio != UIWidget.AspectRatioSource.Free)
            {
                IconSp.keepAspectRatio = UIWidget.AspectRatioSource.Free;
            }
            IconSp.MakePixelPerfect();
            IconSp.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
            IconSp.width = 358;

            DiscountLab.text = LTChargeManager.GetDiscountText(curChargeData.discount);
            DiscountLab.transform.parent.gameObject.CustomSetActive(curChargeData.discount > 0 && curChargeData.discount < 1);
        }
    
        /// <summary>
        /// 显示抽奖礼包
        /// </summary>
        private void SHowDrawUI()
        {
            if (giftDataList == null || giftDataList.Count <= 0)
            {
                EB.Debug.LogError("LTChargeGiftRewardUI ShowUI Draw data is Null!!!");
                return;
            }
    
            curGiftIndex = 0;
            RefreshDrawUI(giftDataList[0]);
        }
    
        /// <summary>
        /// 刷新抽奖礼包
        /// </summary>
        private void RefreshDrawUI(ChargeGiftData data)
        {
            IconSp.spriteName = data.icon;
            if(IconSp.keepAspectRatio!= UIWidget.AspectRatioSource.Free)
            {
                IconSp.keepAspectRatio = UIWidget.AspectRatioSource.Free;
            }
            IconSp.MakePixelPerfect();
            IconSp.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
            IconSp.width = 358;

            TitleLab.text = data.title;
            GiftCountLab.text = string.Format("{0}/{1}", curGiftIndex + 1, giftDataList.Count);
    
            string colorStr = BalanceResourceUtil.GetUserDiamond() >= data.coinValue ? LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            DiamondPriceLab.text = string.Format("[{0}]{1}[-]", colorStr, data.coinValue.ToString());
    
            DiscountLab.text = LTChargeManager.GetDiscountText(data.discount);
            DiscountLab.transform.parent.gameObject.CustomSetActive(data.discount >0 && data.discount < 1);
    
            curItemDataList = data.rewardList;
            curCountDownEnd = data.countDownEnd;
            tempCountDown = 1;
            RefreshShowItem();
            RefreshDrawCountDown();
        }
    
        /// <summary>
        /// 刷新倒计时
        /// </summary>
        private void RefreshDrawCountDown()
        {
            int countDown = curCountDownEnd - EB.Time.Now;
            if (countDown > 0)
            {
                System.TimeSpan ts = EB.Time.FromPosixTime(curCountDownEnd) - EB.Time.FromPosixTime(EB.Time.Now);
                countDownStr.Remove(0, countDownStr.Length);
                countDownStr.Append(string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds));
                DescLab.text = string.Format(EB.Localizer.GetString("ID_CHARGE_GIFTMESSAGE"), countDownStr);
            }
    
            CheckGiftData(countDown <= 0);
        }
    
        /// <summary>
        /// 检测抽奖礼包是否到时
        /// </summary>
        /// <param name="isRefreshCurData">是否更新当前显示礼包数据</param>
        private void CheckGiftData(bool isRefreshCurData)
        {
            for (int i = giftDataList.Count -1; i >= 0; i--)
            {
                if (giftDataList[i].countDownEnd - EB.Time.Now <= 0)
                {
                    giftDataList.RemoveAt(i);
                    GiftCountLab.text = string.Format("{0}/{1}", curGiftIndex + 1, giftDataList.Count);
                }
            }
    
            if (isRefreshCurData)
            {
                if (giftDataList.Count <= 0)
                {
                    ClearGiftData();
                    controller.Close();
                    return;
                }
    
                ArrowObj.CustomSetActive(curGiftType == EGiftType.eDraw && giftDataList != null && giftDataList.Count > 1);
                curGiftIndex = 0;
                RefreshDrawUI(giftDataList[0]);
            }
        }
    
        /// <summary>
        /// 刷新Item显示数据
        /// </summary>
        private void RefreshShowItem()
        {
            int num = curItemDataList.Count - ShowItemList.Count;
            if (num > 0)
            {
                GameObject obj = Object.Instantiate(ShowItemList[0].mDMono.gameObject);
                obj.transform.SetParent(Grid.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                LTShowItem tempItem = obj.GetMonoILRComponent<LTShowItem>();
                ShowItemList.Add(tempItem);
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
            TweenPosition Tweenr= Grid.gameObject.GetComponent<TweenPosition>();
            if (Tweenr != null)
            {
                Tweenr.ResetToBeginning();
                Tweenr.PlayForward();
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
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(curChargeData.redeemers[i].Type))//!curChargeData.redeemers[i].Type.Equals("bpt") && !curChargeData.redeemers[i].Type.Equals("pab") && !curChargeData.redeemers[i].Type.Equals("pdb") && !curChargeData.redeemers[i].Type.Equals("act") && !curChargeData.redeemers[i].Type.Equals("heromedal"))
                {
                    LTShowItemData tempData = new LTShowItemData(curChargeData.redeemers[i].Data, curChargeData.redeemers[i].Quantity, curChargeData.redeemers[i].Type,false);
                    curItemDataList.Add(tempData);
                }
                if (curItemDataList.Count == 0 && curChargeData.redeemers[i].Type.Equals("heromedal"))
                {
                    curItemDataList = Hotfix_LT.Data.EventTemplateManager.Instance.GetTotalHeroMedalItemTp1();
                }
                if (curItemDataList.Count == 0 && curChargeData.redeemers[i].Type.Equals("mcr"))
                {
                    curItemDataList = Hotfix_LT.Data.EventTemplateManager.Instance.GetTotalMainInstanceItemTp1();
                }
            }
        }
    
        /// <summary>
        /// 初始化UIShowItem
        /// </summary>
        private void InitShowItem()
        {
            if (ShowItemList == null)
            {
                ShowItemList = new List<LTShowItem>();
                for (int i = 0; i < Grid.transform.childCount; i++)
                {
                    LTShowItem tempItem = Grid.transform.GetChild(i).GetMonoILRComponent<LTShowItem>();
                    ShowItemList.Add(tempItem);
                }
            }
        }
    
        /// <summary>
        /// 初始化抽奖礼包数据
        /// </summary>
        private void InitDrawGiftData()
        {
            ArrayList array;
            DataLookupsCache.Instance.SearchDataByID("userCultivateGift.gifts", out array);
    
            if (array == null || array.Count <= 0)
            {
                EB.Debug.LogError("Server GiftData is Null!!");
                controller.Close();
                return;
            }
    
            giftDataList = new List<ChargeGiftData>();
            for (int i = 0; i < array.Count; i++)
            {
                ChargeGiftData data = new ChargeGiftData
                {
                    id = EB.Dot.String("id", array[i], ""),
                    tplId = EB.Dot.Integer("tpl_id", array[i], 0),
                    countDownEnd = EB.Dot.Integer("expiry", array[i], 0)
                };
    
                if (string.IsNullOrEmpty(data.id) || data.tplId <= 0)
                {
                    EB.Debug.Log("InitChargeGiftData is Error, Data is Null, index : {0}", i);
                    continue;
                }
    
                Hotfix_LT.Data.PartnerCultivateGift tplData = Hotfix_LT.Data.EventTemplateManager.Instance.GetPartnerCultivateGiftById(data.tplId);
                if (tplData == null)
                {
                    EB.Debug.Log("InitChargeGiftData is Error, tplData is Null, tplId : {0}", data.tplId);
                    continue;
                }
                data.title = tplData.title;
                data.icon = tplData.icon;
                data.discount = tplData.discount;
    
                ArrayList spends = Hotfix_LT.EBCore.Dot.Array("spends", array[i], null);
                if (spends != null && spends.Count > 0)
                {
                    data.coinValue = EB.Dot.Integer("q", spends[0], 0);
                }
    
                ArrayList redeemersData = Hotfix_LT.EBCore.Dot.Array("redeems", array[i], null);
                data.rewardList = InitRewardData(redeemersData);
    
                giftDataList.Add(data);
            }
    
            giftDataList.Sort(delegate (ChargeGiftData x, ChargeGiftData y) { return y.countDownEnd - x.countDownEnd; });
        }
    
        /// <summary>
        /// 初始化ShowItemData
        /// </summary>
        /// <param name="itemDataArray"></param>
        /// <returns></returns>
        private List<LTShowItemData> InitRewardData(ArrayList itemDataArray)
        {
            List<LTShowItemData> itemDataList = new List<LTShowItemData>();
    
            if (itemDataArray != null)
            {
                for (var i = 0; i < itemDataArray.Count; i++)
                {
                    object candidate = itemDataArray[i];
                    Hashtable redeemer = candidate as Hashtable;
                    if (redeemer != null)
                    {
                        RedeemerItem item = new RedeemerItem(redeemer);
                        
                        if (item.IsValid == true && !LTChargeManager.Instance.IgnoreItemList.Contains(item.Type))// !item.Type.Equals("bpt") && !item.Type.Equals("pab") && !item.Type.Equals("pdb") && !item.Type.Equals("act") && !item.Type.Equals("heromedal"))
                        {
                            LTShowItemData tempData = new LTShowItemData(item.Data, item.Quantity, item.Type,false);
                            itemDataList.Add(tempData);
                        }
                    }
                }
            }
    
            return itemDataList;
        }
    
        /// <summary>
        /// 点击商城礼包购买按钮
        /// </summary>
        public void OnClickBuyBtn()
        {
            if (curChargeData.cost == 0)
            {
                string freeGiftType = string.Empty;
                switch (curChargeData.categoryValue)
                {
                    case 3:
                        freeGiftType = "daily";
                        break;
                    case 4:
                        freeGiftType = "weekly";
                        break;
                    case 5:
                        freeGiftType = "monthly";
                        break;
                    default:
                        break;
                }
                LTChargeManager.Instance.ReceiveBuyFreeGift(freeGiftType, delegate
                {
                    controller.Close();
                    GlobalMenuManager.Instance.Open("LTShowRewardView", curItemDataList);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.ChargeFreeGiftReflash);
                    //OnOfferPurchaseSuceeded(curChargeData);
    
                });
            }
            else
            {
                LTChargeManager.Instance.PurchaseOfferExpand(curChargeData, LTChargeStoreController.EventTable);
            }
            
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
    
            //处理礼包类型的
            EB.Debug.Log("OnOfferPurchaseSuceeded");
            
            List<LTShowItemData> itemDataList = new List<LTShowItemData>();
            for (int i = 0; i < item.redeemers.Count; i++)
            {
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(item .redeemers[i].Type))//item.redeemers[i].Type.CompareTo("mcard") != 0 && item.redeemers[i].Type.CompareTo("bpt") != 0 && item.redeemers[i].Type.CompareTo("pab") != 0 && item.redeemers[i].Type.CompareTo("pdb") != 0 && item.redeemers[i].Type.CompareTo("heromedal") != 0)
                {
                    LTShowItemData tempData = new LTShowItemData(item.redeemers[i].Data, item.redeemers[i].Quantity, item.redeemers[i].Type);
                    itemDataList.Add(tempData);
                }
            }
            if(itemDataList.Count>0) GlobalMenuManager.Instance.Open("LTShowRewardView", itemDataList);
    
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_SUCC"));

            controller.Close();
        }
        
        /// <summary>
        /// 点击左箭头
        /// </summary>
        public void OnClickLeftArrow()
        {
            if (giftDataList.Count <= 1)
            {
                return;
            }
            curGiftIndex = (curGiftIndex + giftDataList.Count - 1) % giftDataList.Count;
            RefreshDrawUI(giftDataList[curGiftIndex]);
    
        }
    
        /// <summary>
        /// 点击右箭头
        /// </summary>
        public void OnClickRightArrow()
        {
            if (giftDataList.Count <= 1)
            {
                return;
            }
            curGiftIndex = (curGiftIndex + 1) % giftDataList.Count;
            RefreshDrawUI(giftDataList[curGiftIndex]);
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
    
                ArrowObj.CustomSetActive(curGiftType == EGiftType.eDraw && giftDataList != null && giftDataList.Count > 1);
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
            GlobalMenuManager.Instance.Open("LTChargeStoreHud");
            controller.Close();
        }
    
    }
}
