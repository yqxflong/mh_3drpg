using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChargeStoreController : UIControllerHotfix
    {
        public GameObject vipPanel;
        public UILabel levelLabel;
        public UIProgressBar vipProgressBar;
        public UILabel vipProgressBarLabel;
        
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

            vipPanel = t.Find("RightButtom/VIPPanel").gameObject;
            levelLabel = t.GetComponent<UILabel>("RightButtom/VIPPanel/Icon/Label");
            vipProgressBar = t.GetComponent<UIProgressBar>("RightButtom/VIPPanel/ProgressWidget/ProgressBar");
            vipProgressBarLabel = t.GetComponent<UILabel>("RightButtom/VIPPanel/ProgressWidget/ProgressBar/Label");
            t.GetComponent<ContinueClickCDTrigger>("RightButtom/VIPPanel/Sprite").m_CallBackPress.Add(new EventDelegate(() =>
            {
                GlobalMenuManager.Instance.Open("LTVIPRewardHud");
            }));
            ChargeAndGift = t.GetMonoILRComponent<LTChargeAndGift>("Content/ChargeAndGift");
            ChargePrivilege = t.GetMonoILRComponent<LTChargePrivilege>("Content/Privilege");
            titleCon = t.GetMonoILRComponent<TitleListController>("BG/Title");

            LimitGift = controller.FetchComponentList<GameObject>(GetArray("BG/Title/BtnList/DayGiftBtn","BG/Title/BtnList/WeekGiftBtn","BG/Title/BtnList/MonthGiftBtn"), true).ToArray();
            Line = controller.FetchComponentList<GameObject>(GetArray("BG/Title/BG/GridLine/Line3", "BG/Title/BG/GridLine/Line4", "BG/Title/BG/GridLine/Line5"), true).ToArray();

            isOpen = false;
            controller.backButton = t.GetComponent<UIButton>("LeftTop/CancelBtn");

			controller.FindAndBindingBtnEvent(GetList("BG/Title/BtnList/PrivilegeBtn", "BG/Title/BtnList/GiftBtn", "BG/Title/BtnList/DayGiftBtn",
				"BG/Title/BtnList/WeekGiftBtn", "BG/Title/BtnList/MonthGiftBtn", "BG/Title/BtnList/ChargeBtn"), GetList(new EventDelegate(OnClickPrivilegeBtn),
				new EventDelegate(OnClickGiftBtn), new EventDelegate(OnClickGift1Btn), new EventDelegate(OnClickGift2Btn), new EventDelegate(OnClickGift3Btn), 
				new EventDelegate(OnClickChargeBtn)));

            HidePrivilege();
            //SetVIPPanel();
        }

        public void SetVIPPanel()
        {
			DataLookupsCache.Instance.SearchDataByID<bool>("isOpenEC", out bool isOpen);

			LTVIPDataManager.Instance.UpdateVIPBaseData();
            VIPBaseInfo info = LTVIPDataManager.Instance.GetVIPBaseInfo();
            vipPanel.gameObject.CustomSetActive(isOpen);
            levelLabel.text = info.Level.ToString();
            vipProgressBar.value = info.CurrentExp / (float)info.FullExp;
               
            if (info.Level == LTVIPDataManager.Instance.GetLevelCount())
            {
                vipProgressBarLabel.text = EB.StringUtil.Concat(info.CurrentExp.ToString(), "/MAX");
            }
            else
            {
                vipProgressBarLabel.text = EB.StringUtil.Concat(info.CurrentExp.ToString(), "/", info.FullExp);
            }
			
        }

        public override void OnFocus()
        {
            base.OnFocus();
            vipPanel.transform.Find("Icon/RedPoint").gameObject.SetActive(LTVIPDataManager.Instance.IsExistsGiftUnreceived());
        }

        public enum EChargeType
        {
            eOther = -1,
            ePrivilege = 0,
            eGift = 1,
            eCharge = 2,
            eGift1 = 3,
            eGift2 = 4,
            eGift3 = 5,

        }

        public LTChargeAndGift ChargeAndGift;
        public LTChargePrivilege ChargePrivilege;
        public TitleListController titleCon;
        

        public GameObject[] LimitGift = new GameObject[3];
        public GameObject[] Line = new GameObject[3];

        #region 为了在ios上做屏蔽特权的操作才会使用到的

        #endregion

        private List<EB.IAP.Item> curDataList;
        private List<EB.IAP.Item> freegift = new List<EB.IAP.Item>();
        private EChargeType curChargeType;
        private Dictionary<int, int> GiftNum = new Dictionary<int, int>() { { -1, 0 }, { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };//存储礼包数量
        public static bool isOpen;

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            if (param != null)
            {
                curChargeType = (EChargeType)param;
            }
            else
            {
                curChargeType = GetCurChargrType();
            }

            
            ShowUI(curChargeType);
            //是否显示周日月礼包
            InitTitleList();
            SetTimeLabelShow(curChargeType);
            titleCon.SetTitleBtn((int)curChargeType);
        }
        public override IEnumerator OnAddToStack()
        {
            isOpen = true;
            FusionAudio.PostBGMEvent("BGM/Shop", true);
            FusionAudio.StartBGM();
            yield return base.OnAddToStack();
            ReflashTimeTest();
            ReflashFreeGiftRP();
            Hotfix_LT.Messenger.AddListener("OnOffersFetched", OnOffersFetched);
            Hotfix_LT.Messenger.AddListener<EB.IAP.Item, EB.IAP.Transaction>(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, OnOfferPurchaseSuceeded);
            Hotfix_LT.Messenger.AddListener<string>(Hotfix_LT.EventName.OnOfferPurchaseFailed, OnOfferPurchaseFailed);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnOfferPurchaseCanceled, OnOfferPurchaseCanceled);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.ChargeFreeGiftReflash, ReflashUI);

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.chargedaygift, ChargeDayGiftRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.chargeweekgift, ChargeWeekGiftRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.chargemonthgift, ChargeMonthGiftRP);
        }

        private void ChargeDayGiftRP(RedPointNode node)
        {
            controller.GObjects["Gift1RedPoint"].CustomSetActive(node.num > 0);
        }
        private void ChargeWeekGiftRP(RedPointNode node)
        {
            controller.GObjects["Gift2RedPoint"].CustomSetActive(node.num > 0);
        }
        private void ChargeMonthGiftRP(RedPointNode node)
        {
            controller.GObjects["Gift3RedPoint"].CustomSetActive(node.num > 0);
        }
        
        public override IEnumerator OnRemoveFromStack()
        {

            isOpen = false;
            FusionAudio.StopBGM();
            CloseReflashTime();
            Hotfix_LT.Messenger.RemoveListener("OnOffersFetched", OnOffersFetched);
            Hotfix_LT.Messenger.RemoveListener<EB.IAP.Item, EB.IAP.Transaction>(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, OnOfferPurchaseSuceeded);
            Hotfix_LT.Messenger.RemoveListener<string>(Hotfix_LT.EventName.OnOfferPurchaseFailed, OnOfferPurchaseFailed);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnOfferPurchaseCanceled, OnOfferPurchaseCanceled);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.ChargeFreeGiftReflash, ReflashUI);

            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.chargedaygift, ChargeDayGiftRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.chargeweekgift, ChargeWeekGiftRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.chargemonthgift, ChargeMonthGiftRP);

            DestroySelf();
            yield break;
        }

        private EChargeType GetCurChargrType()
        {

            EB.IAP.Item[] tempArray = EB.Sparx.Hub.Instance.WalletManager.Payouts;
            GiftNum[-1] = 0;
            GiftNum[0] = 0;
            GiftNum[1] = 0;
            GiftNum[2] = 0;
            GiftNum[3] = 0;
            GiftNum[4] = 0;
            GiftNum[5] = 0;
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (tempArray[i].show)
                {
                    GiftNum[tempArray[i].categoryValue]++;
                }
            }
            if (GiftNum[1] > 0)
            {
                return EChargeType.eGift;
            }
            if (GiftNum[5] > 0 || LTChargeManager.Instance.IsCouldGetMonthFreeGift())
            {
                return EChargeType.eGift3;

            }
            if (GiftNum[4] > 0 || LTChargeManager.Instance.IsCouldGetWeekFreeGift())
            {
                return EChargeType.eGift2;

            }
            if (GiftNum[3] > 0 || LTChargeManager.Instance.IsCouldGetDayFreeGift())
            {
                return EChargeType.eGift1;

            }
            return EChargeType.eCharge;


        }

        private void ReflashUI()
        {
            ShowUI(curChargeType);
            if (curChargeType == EChargeType.eGift)
            {
                if (curDataList.Count == 0)
                {

                    return;
                }
            }
            ReflashFreeGiftRP();
            InitTitleList();
            ShowUI(curChargeType);
        }


        private void ShowUI(EChargeType etype)
        {
			SetVIPPanel();
			InitChargeData(etype);
            ChangeMainState(etype);
        }

        private void InitGiftTitleList()//设置萌新礼包以及月周日礼包显示
        {

            if (curChargeType == EChargeType.eGift || curChargeType == EChargeType.eGift1 || curChargeType == EChargeType.eGift2 || curChargeType == EChargeType.eGift3)
            {
                EChargeType temp = GetCurChargrType();
                if (GiftNum[(int)curChargeType] <= 0)
                {
                    switch (curChargeType)
                    {
                        case EChargeType.eGift:
                            curChargeType = temp;
                            break;
                        case EChargeType.eGift1:
                            if (!LTChargeManager.Instance.IsCouldGetDayFreeGift())
                                curChargeType = temp;
                            break;
                        case EChargeType.eGift2:
                            if (!LTChargeManager.Instance.IsCouldGetWeekFreeGift())
                                curChargeType = temp;
                            break;
                        case EChargeType.eGift3:
                            if (!LTChargeManager.Instance.IsCouldGetMonthFreeGift())
                                curChargeType = temp;
                            break;
                        default:
                            break;
                    }
                    SetTimeLabelShow(curChargeType);
                    titleCon.SetTitleBtn((int)curChargeType);
                }

                if (GiftNum[1] <= 0)
                {
                    controller.GObjects["GiftList"].CustomSetActive(false);
                    controller.GObjects["Giftline"].CustomSetActive(false);
                    controller.UiGrids["BtnGrid"].Reposition();
                    controller.UiGrids["LineGrid"].Reposition();
                }
                else
                {
                    controller.GObjects["GiftList"].CustomSetActive(true);
                    controller.GObjects["Giftline"].CustomSetActive(true);
                    controller.UiGrids["BtnGrid"].Reposition();
                    controller.UiGrids["LineGrid"].Reposition();
                }
            }
        }

        private void InitTitleList()
        {
            InitGiftTitleList();
            bool isShow = LTChargeManager.Instance.IsShowMoreGiftCate();
            for (int i = 0; i < 3; i++)
            {
                LimitGift[i].CustomSetActive(isShow);
                Line[i].CustomSetActive(isShow);
            }
            controller.UiGrids["BtnGrid"].Reposition();
            controller.UiGrids["LineGrid"].Reposition();

        }


        /// <summary>
        /// 刷新时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerator ReflashGiftTime(int nextflashtime, LTChargeManager.ECycleTimeType type)
        {
            controller.GObjects["ReflashPanel"].CustomSetActive(true);
            while (true)
            {
                int timestamp = nextflashtime - EB.Time.ToPosixTime(Hotfix_LT.Data.ZoneTimeDiff.GetServerTime()) + 60;
                int day = timestamp / 86400;
                int hour = timestamp / 3600 % 24;
                int min = timestamp / 60 % 60;
                if (timestamp < 60)
                {
                    LoadingSpinner.Show();
                    LTChargeManager.Instance.InitReflashTime();
                    switch (type)
                    {
                        case LTChargeManager.ECycleTimeType.eDay:
                            nextflashtime = LTChargeManager.Instance.DayGiftNextReflashTime;
                            break;
                        case LTChargeManager.ECycleTimeType.eWeek:
                            nextflashtime = LTChargeManager.Instance.WeekGiftNextReflashTime;
                            break;
                        case LTChargeManager.ECycleTimeType.eMonth:
                            nextflashtime = LTChargeManager.Instance.MonthGiftNextReflashTime;
                            break;
                        case LTChargeManager.ECycleTimeType.eYear:
                            break;
                        default:
                            break;
                    }

                    Hub.Instance.GetManager<WalletManager>().Connect();
                    yield return new WaitForSeconds(0.2f);
                    ShowUI(curChargeType);
                    ReflashFreeGiftRP();
                    LoadingSpinner.Hide();
                    controller.UiLabels["ReflashTime"].text = string.Format(EB.Localizer.GetString("ID_CHARGE_GIFTREFLASH_TIME"), day, hour, min);

                }
                else
                {
                    controller.UiLabels["ReflashTime"].text = string.Format(EB.Localizer.GetString("ID_CHARGE_GIFTREFLASH_TIME"), day, hour, min);
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
        private void CloseReflashTime()
        {
            controller.GObjects["ReflashPanel"].CustomSetActive(false);
            if (reflash != null)
            {
                StopCoroutine(reflash);
                reflash = null;
            }
        }
        private IEnumerator reflash;



        private void ChangeMainState(EChargeType etype)
        {
            if (etype == EChargeType.eCharge || etype == EChargeType.eGift || etype == EChargeType.eGift1 || etype == EChargeType.eGift2 || etype == EChargeType.eGift3)
            {
                ChargeAndGift.SetType(etype);
            }

            ChargeAndGift.ShowUI(etype == EChargeType.eCharge || etype == EChargeType.eGift || etype == EChargeType.eGift1 || etype == EChargeType.eGift2 || etype == EChargeType.eGift3, curDataList);
            ChargePrivilege.ShowUI(etype == EChargeType.ePrivilege, curDataList);

            controller.GObjects["ChargeAndGiftViewBG"].CustomSetActive(etype != EChargeType.ePrivilege);
        }

        private void InitChargeData(EChargeType etype)
        {
            //累计充值*600             
            if (curDataList == null)
            {
                curDataList = new List<EB.IAP.Item>();
            }
            curDataList.Clear();

            EB.IAP.Item[] tempArray = EB.Sparx.Hub.Instance.WalletManager.Payouts;
            EB.Debug.Log("++++++++++LTChargeStoreController InitChargeData tempArray.Length : {0}", tempArray.Length);
            int tempflag = 0;//存储超过累冲值的礼包数

            for (int i = 0; i < tempArray.Length; i++)
            {

                if (/*(etype == EChargeType.eCharge || etype == EChargeType.eGift || etype == EChargeType.ePrivilege ) &&*/ tempArray[i].show && ((tempArray[i].categoryValue == (int)etype)))
                {
                    if (etype == EChargeType.eGift1 || etype == EChargeType.eGift2 || etype == EChargeType.eGift3)
                    {
                        if (tempArray[i].value * 100 <= LTChargeManager.CumulativeRecharge)
                        {
                            curDataList.Add(tempArray[i]);
                        }
                        else
                        {
                            tempflag++;
                            if (tempflag == 1)
                            {
                                curDataList.Add(tempArray[i]);
                            }

                        }
                    }
                    else
                    {
                        curDataList.Add(tempArray[i]);
                    }


                }

            }

            //添加免费礼包
            SetFreeGiftData();
            for (int i = 0; i < freegift.Count; i++)
            {
                if (freegift[i].categoryValue == (int)etype)
                {
                    curDataList.Add(freegift[i]);
                }
            }
        }
        /// <summary>
        /// 设置免费礼包数据
        /// </summary>
        private void SetFreeGiftData()
        {


            if (freegift.Count == 0)
            {
                object dayfreeitem = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("dailypayment"));
                object weekfreeitem = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("weeklypayment"));
                object monthfreeitem = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("monthlypayment"));
                if (dayfreeitem != null)
                {
                    freegift.Add(new EB.IAP.Item(dayfreeitem));
                }
                if (weekfreeitem != null)
                {
                    freegift.Add(new EB.IAP.Item(weekfreeitem));
                }
                if (monthfreeitem != null)
                {
                    freegift.Add(new EB.IAP.Item(monthfreeitem));
                }

            }

        }

        /// <summary>
        /// 屏蔽特权界面
        /// </summary>
        private void HidePrivilege()
        {
            bool isOpen = true;
            DataLookupsCache.Instance.SearchDataByID("isOpenEC", out isOpen);
            if (!isOpen)
            {
                controller.GObjects["PrivilegeBtnObj"].CustomSetActive(false);
                controller.GObjects["Line1Obj"].CustomSetActive(false);
                controller.UiGrids["BtnGrid"].enabled = true;
                controller.UiGrids["LineGrid"].enabled = true;
            }
        }

        public void OnClickChargeBtn()
        {
            CloseReflashTime();
            if (curChargeType == EChargeType.eCharge)
            {
                return;
            }
            curChargeType = EChargeType.eCharge;
            //ShowUI(new ChargeFreeGiftReflash(curChargeType));
            ShowUI(curChargeType);
            InitTitleList();

        }
        /// <summary>
        /// 萌新礼包点击
        /// </summary>
        public void OnClickGiftBtn()
        {
            CloseReflashTime();
            if (curChargeType == EChargeType.eGift)
            {
                return;
            }
            curChargeType = EChargeType.eGift;
            //ShowUI(new ChargeFreeGiftReflash(curChargeType));
            ShowUI(curChargeType);
        }
        /// <summary>
        /// 日礼包点击
        /// </summary>
        public void OnClickGift1Btn()
        {
            if (curChargeType == EChargeType.eGift1)
            {
                return;
            }
            curChargeType = EChargeType.eGift1;
            //ShowUI(new ChargeFreeGiftReflash(curChargeType));     
            ShowUI(curChargeType);
            SetTimeLabelShow(curChargeType);

        }

        private void SetTimeLabelShow(EChargeType type)
        {
            ReflashTimeTest();
            CloseReflashTime();
            switch (type)
            {
                case EChargeType.eGift1:
                    reflash = ReflashGiftTime(LTChargeManager.Instance.DayGiftNextReflashTime, LTChargeManager.ECycleTimeType.eDay);
                    break;
                case EChargeType.eGift2:
                    reflash = ReflashGiftTime(LTChargeManager.Instance.WeekGiftNextReflashTime, LTChargeManager.ECycleTimeType.eWeek);
                    break;
                case EChargeType.eGift3:
                    reflash = ReflashGiftTime(LTChargeManager.Instance.MonthGiftNextReflashTime, LTChargeManager.ECycleTimeType.eMonth);
                    break;
                default:
                    reflash = null;
                    break;
            }           
            if(reflash!=null) StartCoroutine(reflash);
        }

        private void ReflashTimeTest()
        {
            int time = LTChargeManager.Instance.DayGiftNextReflashTime - EB.Time.ToPosixTime(Hotfix_LT.Data.ZoneTimeDiff.GetServerTime());
            if (time < 0)
            {
                LTChargeManager.Instance.InitReflashTime();
                Hub.Instance.GetManager<WalletManager>().Connect();

            }
        }

        /// <summary>
        /// 周礼包点击
        /// </summary>
        public void OnClickGift2Btn()
        {
            if (curChargeType == EChargeType.eGift2)
            {
                return;
            }
            curChargeType = EChargeType.eGift2;
            // ShowUI(new ChargeFreeGiftReflash(curChargeType));
            ShowUI(curChargeType);
            SetTimeLabelShow(curChargeType);
        }
        /// <summary>
        /// 月礼包点击
        /// </summary>
        public void OnClickGift3Btn()
        {
            if (curChargeType == EChargeType.eGift3)
            {
                return;
            }
            curChargeType = EChargeType.eGift3;
            //ShowUI(new ChargeFreeGiftReflash(curChargeType));
            ShowUI(curChargeType);
            SetTimeLabelShow(curChargeType);
        }
        public void OnClickPrivilegeBtn()
        {
            if (curChargeType == EChargeType.ePrivilege)
            {
                return;
            }
            curChargeType = EChargeType.ePrivilege;
            //ShowUI(new ChargeFreeGiftReflash(curChargeType));
            ShowUI(curChargeType);
        }

        private void ReflashFreeGiftRP()
        {
            LTChargeManager.Instance.IsCouldGetDayFreeGift(true);
            LTChargeManager.Instance.IsCouldGetWeekFreeGift(true);
            LTChargeManager.Instance.IsCouldGetMonthFreeGift(true);

        }
        /// <summary>
        /// 更新数据
        /// </summary>
        private void OnOffersFetched()
        {
            EB.Debug.Log("OnOffersFetched");
            //ShowUI(new ChargeFreeGiftReflash(curChargeType));
            InitTitleList();
            ShowUI(curChargeType);
            ReflashFreeGiftRP();
        }

        /// <summary>
        /// 支付成功
        /// </summary>
        /// <param name="item"></param>
        private void OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans)
        {
            if (!UIStack.Instance.IsStacked(controller))
            {
                EB.Debug.Log("OnOfferPurchaseSuceeded——return!");
                return;
            }
            if (curChargeType == EChargeType.eGift || curChargeType == EChargeType.eGift1 || curChargeType == EChargeType.eGift2 || curChargeType == EChargeType.eGift3)
            {
                return;
            }

            //只处理非礼包类型的
            EB.Debug.Log("OnOfferPurchaseSuceeded");
            List<LTShowItemData> itemDataList = new List<LTShowItemData>();
            for (int i = 0; i < item.redeemers.Count; i++)
            {
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(item.redeemers[i].Type))//item.redeemers[i].Type.CompareTo("mcard") != 0 && item.redeemers[i].Type.CompareTo("bpt") != 0 && item.redeemers[i].Type.CompareTo("pab") != 0 && item.redeemers[i].Type.CompareTo("pdb") != 0&& item.redeemers[i].Type.CompareTo("heromedal") != 0)
                {
                    LTShowItemData tempData = new LTShowItemData(item.redeemers[i].Data, item.redeemers[i].Quantity, item.redeemers[i].Type);
                    itemDataList.Add(tempData);
                }
            }

            //永久月卡额外赠送的特写
            if (item.payoutId == 2001)
            {
                var world = System.Array.Find(LoginManager.Instance.GameWorlds, w => w.Default);
                if (world.OpenTime > 0)
                {
                    int timeZone = Hotfix_LT.Data.ZoneTimeDiff.GetTimeZone();//需处理时区问题
                    int day = System.TimeSpan.FromSeconds(EB.Time.Now + timeZone * 3600).Days - System.TimeSpan.FromSeconds(world.OpenTime + timeZone * 3600).Days;
                    var reward = Hotfix_LT.Data.EventTemplateManager.Instance.GetDailyRewardByType("silver_month_card");
                    if (reward != null && day > 0)
                    {
                        for (int i = 0; i < reward.ItemList.Count; i++)
                        {
                            itemDataList.Add(new LTShowItemData(reward.ItemList[i].id, reward.ItemList[i].count * day, reward.ItemList[i].type));
                        }
                    }
                }
            }

            if (itemDataList.Count > 0) GlobalMenuManager.Instance.Open("LTShowRewardView", itemDataList);

            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_SUCC"));
        }

        /// <summary>
        /// 支付失败
        /// </summary>
        /// <param name="error"></param>
        private void OnOfferPurchaseFailed(string error)
        {
            if (!UIStack.Instance.IsStacked(controller))
            {
                EB.Debug.Log("OnOfferPurchaseSuceeded——return!");
                return;
            }
            EB.Debug.Log("OnOfferPurchaseFailed");

            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_FAIL"));
        }

        /// <summary>
        /// 支付取消
        /// </summary>
        private void OnOfferPurchaseCanceled()
        {
            if (!UIStack.Instance.IsStacked(controller))
            {
                EB.Debug.Log("OnOfferPurchaseSuceeded——return!");
                return;
            }
            EB.Debug.Log("OnOfferPurchaseCanceled");

            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_CANCEL"));
        }

        /// <summary>
        /// 点击右上角的钻石图标
        /// </summary>
        public void OnClickDiamondBtn()
        {
            if (curChargeType != EChargeType.eCharge)
            {
                curChargeType = EChargeType.eCharge;
                titleCon.SetTitleBtn((int)curChargeType);
                //ShowUI(new ChargeFreeGiftReflash(curChargeType));
                ShowUI(curChargeType);
            }
        }

        private static Hashtable eventTable;
        public static Hashtable EventTable
        {
            get
            {
                if (eventTable == null)
                {
                    eventTable = Johny.HashtablePool.Claim();
                    eventTable["callBack"] = new System.Action<Hashtable>(delegate (Hashtable table) { DataLookupsCache.Instance.CacheData(table); });
                    eventTable["loadingEvent"] = new System.Action<bool>(delegate (bool isShow) { if (isShow) { LoadingSpinner.Show(); } else { LoadingSpinner.Hide(); } });
                }
                return eventTable;
            }
        }

    }
}