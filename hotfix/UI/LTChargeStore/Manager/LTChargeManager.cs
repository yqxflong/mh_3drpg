using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChargeManager : ManagerUnit
    {
        public enum ECycleTimeType
        {
            eDay = 0,
            eWeek = 1,
            eMonth = 2,
            eYear = 3,

        }
        private static LTChargeManager instance = null;

        private LTChargeAPI Api;

        public static string SilverMonthCardPath = "user_prize_data.silver_monthly_vip";
        public static string GoldMonthCardPath = "user_prize_data.gold_monthly_vip";
        private static int cumulativeRecharge;//累充字段
        public static int CumulativeRecharge
        {
            get
            {
                DataLookupsCache.Instance.SearchIntByID("user.revenue", out cumulativeRecharge);
                return cumulativeRecharge;
            }
        }//累充金额

        public List<string> IgnoreItemList = new List<string> { "mcard", "bpt", "pab", "pdb", "heromedal", "act", "mcr" };

        public int ShowChargeGiftLimit
        {
            get
            {
                return 3000;
            }
        }//显示礼包额度

        public static LTChargeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTChargeManager>();
                }
                return instance;
            }
        }
        
        public override void Initialize(Config config)
        {
            Instance.Api = new LTChargeAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
        }

        private void DataRefresh()
        {
            IsCouldGetFreeGift();
            IsCouldGetPrivilegeRreward();
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        /// <summary>
        /// 请求领取月卡
        /// </summary>
        /// <param name="cardType"></param>
        /// <param name="callBack"></param>
        public void ReceiveMonthCard(string cardType, System.Action<bool> callBack)
        {
            Api.ReceiveMonthCard(cardType, delegate (Hashtable result)
            {
                if (result != null && cardType == "gold")
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                callBack(result != null);
            });
        }

        /// <summary>
        /// 请求购买礼包
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="callBack"></param>
        public void ReceiveBuyGift(string Id, System.Action callBack)
        {
            Api.ReceiveBuyGift(Id, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                callBack();
            });
        }
        /// <summary>
        /// 请求购买免费礼包
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="callback"></param>
        public void ReceiveBuyFreeGift(string Id, System.Action callback)
        {
            Api.ReceiveBuyFreeGift(Id, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                callback();
            });
        }

        //是否显示周月日礼包页签
        public bool IsShowMoreGiftCate()
        {
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10089);
            bool isconditionOk = false;
            if (ft != null && ft.IsConditionOK())
            {
                return true;
            }
            return (CumulativeRecharge >= ShowChargeGiftLimit) || isconditionOk;
        }
        public bool IsCouldGetDayFreeGift(bool isUpdateRedPoint=false)
        {
            if (!IsShowMoreGiftCate())
            {
                if(isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst .chargedaygift,0);
                return false;
            }
            //存在可读取礼包
            if (Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("dailypayment") != string.Empty)
            {
                int dayFreeGiftLastTime = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.free_payment.daily.ts", out dayFreeGiftLastTime);
                if (dayFreeGiftLastTime == 0)
                {
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargedaygift, 1);
                    return true;
                }
                System.DateTime time1 = EB.Time.FromPosixTime(dayFreeGiftLastTime + Hotfix_LT.Data.ZoneTimeDiff.GetTimeZone() * 3600);
                if ((EB.Time.Now - dayFreeGiftLastTime) / 86400 >= 1)
                {
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargedaygift, 1);
                    return true;
                }
                else
                {
                    if (time1.Day != Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Day)
                    {
                        if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargedaygift, 1);
                        return true;
                    }
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargedaygift, 0);
                    return false;
                }
            }
            if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargedaygift, 0);
            return false;
        }
        public bool IsCouldGetWeekFreeGift(bool isUpdateRedPoint = false)
        {
            if (!IsShowMoreGiftCate())
            {
                if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargeweekgift, 0);
                return false;
            }
            if (Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("weeklypayment") != string.Empty)
            {
                int weekFreeGiftLastTime;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.free_payment.weekly.ts", out weekFreeGiftLastTime);
                if (weekFreeGiftLastTime == 0)
                {
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargeweekgift, 1);
                    return true;
                }
                int buygiftday = (int)EB.Time.FromPosixTime(weekFreeGiftLastTime + Hotfix_LT.Data.ZoneTimeDiff.GetTimeZone() * 3600).DayOfWeek;
                int nowday = (int)(Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().DayOfWeek);

                if ((EB.Time.Now - weekFreeGiftLastTime) / (24 * 3600) < 7)
                {
                    if (buygiftday == 0)
                    {
                        buygiftday = 7;
                    }
                    if (nowday == 0)
                    {
                        nowday = 7;
                    }
                    if (buygiftday > nowday)
                    {
                        if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargeweekgift, 1);
                        return true;
                    }
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargeweekgift, 0);
                    return false;
                }
                else
                {
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargeweekgift, 1);
                    return true;
                }
            }
            if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargeweekgift, 0);
            return false;
        }

        public bool IsCouldGetMonthFreeGift(bool isUpdateRedPoint = false)
        {
            if (!IsShowMoreGiftCate())
            {
                if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargemonthgift, 0);
                return false;
            }
            if (Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("monthlypayment") != string.Empty)
            {
                int monthFreeGiftLastTime;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.free_payment.monthly.ts", out monthFreeGiftLastTime);
                if (monthFreeGiftLastTime == 0)
                {
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargemonthgift, 1);
                    return true;
                }
                if (EB.Time.FromPosixTime(monthFreeGiftLastTime + Hotfix_LT.Data.ZoneTimeDiff.GetTimeZone() * 3600).Month != Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Month)
                {
                    if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargemonthgift, 1);
                    return true;
                }
            }
            if (isUpdateRedPoint) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.chargemonthgift, 0);
            return false;
        }
        public void IsCouldGetFreeGift()
        {
            if (!IsShowMoreGiftCate())
            {
                return;
            }
            if (IsCouldGetDayFreeGift(true) || IsCouldGetWeekFreeGift(true) || IsCouldGetMonthFreeGift(true)) { }
        }
        /// <summary>
        /// 是否是白银VIP
        /// </summary>
        /// <returns></returns>
        public bool IsSilverVIP()
        {
            bool isVip = false;
            bool isExpire = true;//是否到期了，服务器在到期了不会下发数据，需要加上这个做判断
            double expire_time = 0;
            if (DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.expire_time", SilverMonthCardPath), out expire_time))
            {
                isExpire = expire_time != -1 && expire_time - EB.Time.Now <= 0;
            }

            DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.is_monthly_vip", SilverMonthCardPath), out isVip);

            return isVip && !isExpire;
        }
        private int dayGiftNextReflashTime;
        private int weekGiftNextReflashTime;
        private int monthGiftNextReflashTime;
        public int DayGiftNextReflashTime
        {
            get
            {
                if (dayGiftNextReflashTime == 0)
                {
                    InitReflashTime();
                }
                return dayGiftNextReflashTime;
            }
        }
        public int WeekGiftNextReflashTime
        {
            get
            {
                if (weekGiftNextReflashTime == 0)
                {
                    InitReflashTime();
                }
                return weekGiftNextReflashTime;
            }
        }
        public int MonthGiftNextReflashTime
        {
            get
            {
                if (monthGiftNextReflashTime == 0)
                {
                    InitReflashTime();
                }
                return monthGiftNextReflashTime;
            }
        }
        public int GetNextReflashTime(int startTime, ECycleTimeType type)
        {
            int daysecond = 86400;
            int daynum = 0;
            switch (type)
            {
                case ECycleTimeType.eDay:
                    return startTime + daysecond;
                case ECycleTimeType.eWeek:
                    return startTime + daysecond * 7;
                case ECycleTimeType.eMonth:
                    daynum = System.DateTime.DaysInMonth(EB.Time.FromPosixTime(startTime).Year, EB.Time.FromPosixTime(startTime).Month);
                    return startTime + daysecond * daynum;
                case ECycleTimeType.eYear:
                    int year = EB.Time.FromPosixTime(startTime).Year;
                    daynum = (year % 100 != 0 && year % 4 == 0) || (year % 400 == 0) ? 366 : 365;
                    return startTime + daysecond * daynum;
                default:
                    return -1;
            }
        }
        public void InitReflashTime()
        {

            int startTime = EB.Time.ToPosixTime(Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Date);
            dayGiftNextReflashTime = GetNextReflashTime(startTime, ECycleTimeType.eDay);
            int day = (int)Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().DayOfWeek - 1;
            if (day == -1)
            {
                day = 6;
            }
            startTime = EB.Time.ToPosixTime(Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Date) - day * 86400;
            weekGiftNextReflashTime = GetNextReflashTime(startTime, ECycleTimeType.eWeek);
            day = Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Day - 1;
            startTime = EB.Time.ToPosixTime(Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Date) - day * 86400;
            monthGiftNextReflashTime = GetNextReflashTime(startTime, ECycleTimeType.eMonth);

        }
        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            InitReflashTime();

            hasInit = false;
        }
        /// <summary>
        /// 是否是黄金VIP
        /// </summary>
        /// <returns></returns>
        public bool IsGoldVIP()
        {
            bool isVip = false;
            bool isExpire = true;//是否到期了，服务器在到期了不会下发数据，需要加上这个做判断
            double expire_time = 0;
            if (DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.expire_time", GoldMonthCardPath), out expire_time))
            {
                isExpire = expire_time - EB.Time.Now <= 0;
            }

            DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.is_monthly_vip", GoldMonthCardPath), out isVip);

            return isVip && !isExpire;
        }

        /// <summary>
        /// 是否是月卡VIP
        /// </summary>
        /// <returns></returns>
        public bool IsMonthVIP()
        {
            return IsSilverVIP() || IsGoldVIP();
        }

        /// <summary>
        /// 是否可以领取白银特权的钻石奖励
        /// </summary>
        /// <returns></returns>
        public bool IsCouldGetSilverReward()
        {
            bool isGot = true;
            if (IsSilverVIP())
            {
                DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.is_draw", SilverMonthCardPath), out isGot);
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.silverprivilege, (isGot)?0:1);
            return !isGot;
        }

        /// <summary>
        /// 是否可以领取黄金特权的钻石奖励
        /// </summary>
        /// <returns></returns>
        public bool IsCouldGetGoldReward()
        {
            bool isGot = true;
            if (IsGoldVIP())
            {
                DataLookupsCache.Instance.SearchDataByID(string.Format("{0}.is_draw", GoldMonthCardPath), out isGot);
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.goldprivilege, (isGot) ? 0 : 1);
            return !isGot;
        }

        /// <summary>
        /// 是否可以领取特权的钻石奖励
        /// </summary>
        /// <returns></returns>
        public void IsCouldGetPrivilegeRreward()
        {
            if (IsCouldGetSilverReward() || IsCouldGetGoldReward()) { }
        }

        private bool isShowDrawGift = false;
        /// <summary>
        /// 是否需要显示伙伴培养礼包界面
        /// </summary>
        public bool IsShowDrawGift
        {
            set { isShowDrawGift = value; }
            get { return isShowDrawGift; }
        }

        public void PurchaseOfferExpand(EB.IAP.Item item, Hashtable table = null)
        {
            if (ILRDefine.USE_WECHATSDK && ILRDefine.USE_ALIPAYSDK)
            {
                System.Action callBack = delegate {
                    InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
                    EB.Sparx.Hub.Instance.WalletManager.PurchaseOffer(item, table);
                };
                GlobalMenuManager.Instance.Open("LTPayFuncUI", callBack);
            }
            else
            {
                EB.Sparx.Hub.Instance.mBaseSdkManager.SetCurHCCount(BalanceResourceUtil.GetUserDiamond());
                EB.Sparx.Hub.Instance.WalletManager.PurchaseOffer(item, table);
            }
        }


        #region 限时礼包相关
        private int LTGLateOverTime = 0;
        public int GetLTGLateOverTime()
        {
            return LTGLateOverTime;
        }
        private Queue<int> ShowGiftList = new Queue<int>();
        public int GetGiftNoticeId()
        {
            if (ShowGiftList.Count > 0) return ShowGiftList.Dequeue();
            else return 0;
        }


        /// <summary>
        /// 是否有限时礼包存在
        /// </summary>
        /// <returns></returns>
        public bool HasLimitedTimeGift()
        {
            return EB.Time.Now - LTGLateOverTime < 0;
        }

        bool hasInit = false;
        /// <summary>
        /// 刷新限时礼包数据
        /// </summary>
        public void ReflashLimitedTimeGiftInfo(bool isInit = false, bool buyItemReflash = false)
        {
            if (isInit)
            {
                if (hasInit || EB.Sparx.Hub.Instance.WalletManager.Payouts.Length == 0) return;
                hasInit = true;
            }
            EB.Debug.Log("ReflashLimitedTimeGiftInfo");
            LTGLateOverTime = 0;
            Hashtable datas;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userLimitedTimeGift", out datas);
            if (datas != null)
            {
                foreach (DictionaryEntry data in datas)
                {
                    string TriggerID = EB.Dot.String("triggerId", data.Value, string.Empty);
                    var ArrTemp = Hotfix_LT.EBCore.Dot.Array("data", data.Value, Johny.ArrayListPool.Claim());
                    for (int j = 0; j < ArrTemp.Count; ++j)
                    {
                        int PayoutID = EB.Dot.Integer("payoutId", ArrTemp[j], 0);
                        if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(PayoutID))
                        {
                            int OverTime = EB.Dot.Integer("overTime", ArrTemp[j], 0);
                            if (EB.Time.Now < OverTime)
                            {
                                if (LTGLateOverTime == 0 || OverTime < LTGLateOverTime) LTGLateOverTime = OverTime;
                            }
                        }
                    }
                }
            }
            if (isInit) LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.TimeStamp);
            if (buyItemReflash) SetLocalWeighting();
        }

        /// <summary>
        /// 获取限时礼包数据，界面显示
        /// </summary>
        /// <returns></returns>
        public List<LTChargeLimitTimeGiftData> GetLTChargeLimitTimeGiftList()
        {
            LTGLateOverTime = 0;
            List<LTChargeLimitTimeGiftData> temp = new List<LTChargeLimitTimeGiftData>();
            Hashtable datas;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userLimitedTimeGift", out datas);
            if (datas != null)
            {
                foreach (DictionaryEntry data in datas)
                {
                    string TriggerID = EB.Dot.String("triggerId", data.Value, string.Empty);
                    var ArrTemp = Hotfix_LT.EBCore.Dot.Array("data", data.Value, Johny.ArrayListPool.Claim());
                    Dictionary<int, List<int>> PayoutDic = new Dictionary<int, List<int>>();
                    for (int j = 0; j < ArrTemp.Count; ++j)
                    {
                        int PayoutID = EB.Dot.Integer("payoutId", ArrTemp[j], 0);
                        if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(PayoutID))
                        {
                            int OverTime = EB.Dot.Integer("overTime", ArrTemp[j], 0);
                            if (EB.Time.Now < OverTime)
                            {
                                if (PayoutDic.ContainsKey(PayoutID)) PayoutDic[PayoutID].Add(OverTime);
                                else PayoutDic.Add(PayoutID, new List<int> { OverTime });

                                if (LTGLateOverTime == 0 || OverTime < LTGLateOverTime) LTGLateOverTime = OverTime;
                            }
                        }
                    }
                    foreach (var item in PayoutDic)
                    {
                        temp.Add(new LTChargeLimitTimeGiftData(TriggerID, item.Key, item.Value));
                    }
                }
            }
            temp.Sort((a, b) =>
            {
                if (a.OverTime < b.OverTime)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }

            });
            return temp;
        }

        /// <summary>
        /// 根据限时礼包payid获得数据
        /// </summary>
        /// <param name="PayoutId"></param>
        /// <returns></returns>
        public LTChargeLimitTimeGiftData GetChargeLimitTimeNoticeData(int PayoutId)
        {
            Hashtable datas;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userLimitedTimeGift", out datas);
            if (datas != null)
            {
                foreach (DictionaryEntry data in datas)
                {
                    string TriggerID = EB.Dot.String("triggerId", data.Value, string.Empty);
                    var ArrTemp = Hotfix_LT.EBCore.Dot.Array("data", data.Value, Johny.ArrayListPool.Claim());
                    List<int> timeList = new List<int>();
                    for (int j = 0; j < ArrTemp.Count; ++j)
                    {
                        int PayoutID = EB.Dot.Integer("payoutId", ArrTemp[j], 0);
                        if (PayoutID == PayoutId && EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(PayoutID))
                        {
                            int OverTime = EB.Dot.Integer("overTime", ArrTemp[j], 0);
                            if (EB.Time.Now < OverTime)
                            {
                                timeList.Add(OverTime);
                            }
                        }
                    }
                    if (timeList.Count != 0) return new LTChargeLimitTimeGiftData(TriggerID, PayoutId, timeList);
                }
            }
            return null;
        }

        /// <summary>
        /// 统一的触发接口
        /// </summary>
        public void CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType type, string param = null)
        {
            EB.Debug.Log("检测触发：{0};参数：{1}", type, param);

            switch (type)
            {
                case Hotfix_LT.Data.LTGTriggerType.TimeStamp:
                case Hotfix_LT.Data.LTGTriggerType.StarUp:
                    OnTimer(type, param);     //时间检测时触发
                    break;
                case Hotfix_LT.Data.LTGTriggerType.Draw:
                case Hotfix_LT.Data.LTGTriggerType.DrawSame:
                case Hotfix_LT.Data.LTGTriggerType.DrawFail:
                    OnRepeated(type, param);  //可反复触发
                    break;
                default:
                    {
                        //单触发,仅会触发一次
                        bool isTrigger = false;
                        var data = Hotfix_LT.Data.TaskTemplateManager.Instance.GetLimitedTimeGiftByTypeAndParam(type, param);
                        if (data != null)
                        {
                            switch (data.TriggerType)
                            {
                                case Hotfix_LT.Data.LTGTriggerType.Level:
                                case Hotfix_LT.Data.LTGTriggerType.Main:
                                case Hotfix_LT.Data.LTGTriggerType.Challenge:
                                case Hotfix_LT.Data.LTGTriggerType.UltimateTrial:
                                    CanTrigger(data, out isTrigger);
                                    ; break;
                                default:
                                    EB.Debug.LogError("LTChargeManger CheckLimitedTimeGiftTrigger Fail!");
                                    ; break;
                            }
                        }
                        if (isTrigger && data != null) TriggerLimitedTimeGift(data);
                    }
                    break;
            }
        }

        private void OnTimer(Hotfix_LT.Data.LTGTriggerType type, string param)
        {
            //时间类型由进入游戏后读表检测触发
            List<Hotfix_LT.Data.LimitedTimeGiftTemplate> temps;
            if (param != null)
            {
                Hotfix_LT.Data.LimitedTimeGiftTemplate temp = Hotfix_LT.Data.TaskTemplateManager.Instance.GetLimitedTimeGiftByTypeAndParam(type, param);
                temps = new List<Hotfix_LT.Data.LimitedTimeGiftTemplate>() { temp };
            }
            else
            {
                temps = Hotfix_LT.Data.TaskTemplateManager.Instance.GetLimitedTimeGiftListByType(type);
            }
            string triggerId;
            for (int i = 0; i < temps.Count; ++i)
            {
                //存在则检测是否需要再次触发
                if (temps[i].TriggerOpenTime > EB.Time.Now) continue;
                if (DataLookupsCache.Instance.SearchDataByID<string>(string.Format("userLimitedTimeGift.{0}.triggerId", temps[i].ID), out triggerId) && !string.IsNullOrEmpty(triggerId))
                {
                    ArrayList ArrTemp;
                    DataLookupsCache.Instance.SearchDataByID<ArrayList>(string.Format("userLimitedTimeGift.{0}.data", temps[i].ID), out ArrTemp);
                    if (ArrTemp != null)
                    {
                        //for (int j = 0; j < ArrTemp.Count; ++j)
                        //{
                        //    int PayoutID = EB.Dot.Integer("payoutId", ArrTemp[j], 0);
                        //    if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(PayoutID))
                        //    {
                        //        int OverTime = EB.Dot.Integer("overTime", ArrTemp[j], 0);
                        //        if (temps[i].TriggerOpenTime > OverTime)
                        //        {
                        //            TriggerLimitedTimeGift(temps[i]);
                        //        }
                        //        else if (temps[i].TriggerOpenTime == 0 && !string.IsNullOrEmpty(temps[i].TargetParameter))
                        //        {
                        //            var str = temps[i].TargetParameter.Split(',');
                        //            if (EB.Time.Now > OverTime - temps[i].Duration + int.Parse(str[str.Length - 1]))
                        //            {
                        //                TriggerLimitedTimeGift(temps[i]);
                        //            }
                        //        }
                        //    }
                        //}

                        if (ArrTemp.Count == 0) continue;

                        int PayoutID = EB.Dot.Integer("payoutId", ArrTemp[0], 0);
                        if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(PayoutID))
                        {
                            int OverTime = EB.Dot.Integer("overTime", ArrTemp[0], 0);
                            if (temps[i].TriggerOpenTime > OverTime)
                            {
                                TriggerLimitedTimeGift(temps[i]);
                            }
                            else if (temps[i].TriggerOpenTime == 0 && !string.IsNullOrEmpty(temps[i].TargetParameter))
                            {
                                if (temps[i].TargetParameter.Contains(","))
                                {
                                    var str = temps[i].TargetParameter.Split(',');
                                    if (EB.Time.Now > OverTime - temps[i].Duration + int.Parse(str[str.Length - 1]))
                                    {
                                        TriggerLimitedTimeGift(temps[i]);
                                    }
                                }
                                else
                                {
                                    if (EB.Time.Now > OverTime)
                                    {
                                        TriggerLimitedTimeGift(temps[i]);
                                    }
                                }
                            }
                        }
                    }
                }
                else//不存在则直接触发
                {
                    TriggerLimitedTimeGift(temps[i]);
                }
            }
        }
        private void OnRepeated(Hotfix_LT.Data.LTGTriggerType type, string param)
        {
            //直接触发
            var temp = Hotfix_LT.Data.TaskTemplateManager.Instance.GetLimitedTimeGiftByTypeAndParam(type, param);
            if (temp != null)
            {
                TriggerLimitedTimeGift(temp);
            }
        }
        private void CanTrigger(Hotfix_LT.Data.LimitedTimeGiftTemplate data, out bool isTrigger)
        {
            isTrigger = false;
            string triggerId;
            if ((!DataLookupsCache.Instance.SearchDataByID<string>(string.Format("userLimitedTimeGift.{0}.triggerId", data.ID), out triggerId) && string.IsNullOrEmpty(triggerId)))
            {
                //不存在才触发
                isTrigger = true;
            }
        }
        private void TriggerLimitedTimeGift(Hotfix_LT.Data.LimitedTimeGiftTemplate data)
        {
            //额度检测
            int PayoutID = 0;
            float Recharge = LTChargeManager.CumulativeRecharge;
            float Weighting = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LimitTimeGiftWeighting");
            //本地额度运用
            Recharge = Recharge / Mathf.Max(Weighting, 1f);
            float LocalWeighting = GetLocalWeighting(Recharge);
            if (LocalWeighting > 0)
            {
                Recharge = Recharge - LocalWeighting;
            }
            EB.IAP.Item item = null;
            for (int i = 0; i < data.GiftList.Count; ++i)
            {
                if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(data.GiftList[i], out item))
                {


                    PayoutID = item.payoutId;
                    if (item.cost * 100 > Recharge)
                    {
                        break;
                    }
                }
            }
            if (PayoutID > 0)
            {
                ArrayList ArrTemp;
                DataLookupsCache.Instance.SearchDataByID<ArrayList>(string.Format("userLimitedTimeGift.{0}.data", data.ID), out ArrTemp);
                int count = 0;
                if (ArrTemp != null)
                {
                    for (int j = 0; j < ArrTemp.Count; ++j)
                    {
                        int PID = EB.Dot.Integer("payoutId", ArrTemp[j], 0);
                        if (PID == PayoutID)
                        {
                            int OverTime = EB.Dot.Integer("overTime", ArrTemp[j], 0);
                            if (EB.Time.Now < OverTime) count++;
                        }
                    }
                    if (item != null && item.limitNum > 0 && count >= item.limitNum)
                    {
                        return;
                    }
                }
                Api.RequestTriggerLimitedTimeGift(data.ID, PayoutID, delegate (Hashtable result)
                {
                    if (result != null)
                    {
                        DataLookupsCache.Instance.CacheData(result);
                        ReflashLimitedTimeGiftInfo();
                        if (!GuideNodeManager.IsGuide && !ShowGiftList.Contains(PayoutID))
                        {
                            ShowGiftList.Enqueue(PayoutID);
                        }
                        //增加本地额度
                        SetLocalWeightingList(data.ID);
                    }

                });
            }
        }
        private float GetLocalWeighting(float Recharge)
        {
            float LocalWeighting = PlayerPrefs.GetFloat("LocalWeighting" + LoginManager.Instance.LocalUserId.Value);
            string LocalWeightingList = PlayerPrefs.GetString("LocalWeightingList" + LoginManager.Instance.LocalUserId.Value);
            if (!string.IsNullOrEmpty(LocalWeightingList))
            {
                bool hasChange = false;
                string NewLocalWeightingList = "";
                string[] splits = LocalWeightingList.Split(',');
                float localWeighting = Mathf.Max(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("LocalTimeGiftWeighting"), 1f);
                for (int i = 0; i < splits.Length - 1; ++i)
                {
                    if (!string.IsNullOrEmpty(splits[i]))
                    {
                        string[] splits2 = splits[i].Split(':');
                        if (splits2.Length > 1)
                        {
                            int OverTime = int.Parse(splits2[1]);
                            if (EB.Time.Now > OverTime)
                            {
                                EB.IAP.Item item;
                                if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(int.Parse(splits2[0]), out item))
                                {
                                    LocalWeighting += (float)item.cents / localWeighting;
                                }
                                hasChange = true;
                            }
                            else
                            {
                                NewLocalWeightingList = string.Format("{0}{1},", NewLocalWeightingList, splits[i]);
                            }
                        }
                    }
                }
                if (hasChange)
                {
                    if (LocalWeighting > Recharge) LocalWeighting = Recharge;
                    PlayerPrefs.SetFloat("LocalWeighting" + LoginManager.Instance.LocalUserId.Value, LocalWeighting);
                    PlayerPrefs.SetString("LocalWeightingList" + LoginManager.Instance.LocalUserId.Value, NewLocalWeightingList);
                }
            }

            EB.Debug.Log("当前本地额度参数为:{0}", LocalWeighting);
            return LocalWeighting;
        }
        public string CurLimitTimeGiftLocalWeighting;
        private void SetLocalWeighting()
        {
            if (!string.IsNullOrEmpty(CurLimitTimeGiftLocalWeighting))
            {
                string LocalWeightingList = PlayerPrefs.GetString("LocalWeightingList" + LoginManager.Instance.LocalUserId.Value);
                if (LocalWeightingList.Contains(CurLimitTimeGiftLocalWeighting))
                {
                    string NewLocalWeightingList = LocalWeightingList.Replace(string.Format("{0},", CurLimitTimeGiftLocalWeighting), "");
                    CurLimitTimeGiftLocalWeighting = string.Empty;
                    PlayerPrefs.SetString("LocalWeightingList" + LoginManager.Instance.LocalUserId.Value, NewLocalWeightingList);
                }

            }
        }
        private void SetLocalWeightingList(string ID)
        {
            string LocalWeightingList = PlayerPrefs.GetString("LocalWeightingList" + LoginManager.Instance.LocalUserId.Value);
            string[] splits = LocalWeightingList.Split(',');
            ArrayList ArrTemp;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(string.Format("userLimitedTimeGift.{0}.data", ID), out ArrTemp);
            if (ArrTemp != null)
            {
                for (int j = 0; j < ArrTemp.Count; ++j)
                {
                    int PayoutID = EB.Dot.Integer("payoutId", ArrTemp[j], 0);
                    if (EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(PayoutID))
                    {
                        int OverTime = EB.Dot.Integer("overTime", ArrTemp[j], 0);
                        if (EB.Time.Now < OverTime)
                        {
                            string str = string.Format("{0}:{1}", PayoutID, OverTime);
                            if (!splits.Contains(str))
                            {
                                LocalWeightingList += string.Format("{0},", str);
                                EB.Debug.Log("添加本地额度配置:{0};当前存储为:{1}", str, LocalWeightingList);
                            }
                        }
                    }
                }
                PlayerPrefs.SetString("LocalWeightingList" + LoginManager.Instance.LocalUserId.Value, LocalWeightingList);
            }
        }

        /// <summary>
        /// 检测礼包
        /// </summary>
        public bool UpdateLimitedTimeGiftNotice()
        {
            int giftId = LTChargeManager.Instance.GetGiftNoticeId();
            if (!GuideNodeManager.IsGuide && giftId > 0)
            {
                var data = LTChargeManager.Instance.GetChargeLimitTimeNoticeData(giftId);
                if (data != null)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.8f);
                    GlobalMenuManager.Instance.Open("LTChargeLimitTimeNoticeHud", data);
                    return true;
                }
            }
            return false;
        }


        #endregion

        #region 通用接口

        /// <summary>
        /// 获取折扣本地化文本
        /// </summary>
        /// <param name="discountValue">折扣值</param>
        /// <returns>折扣文本</returns>
        public static string GetDiscountText(float discountValue)
        {
            if (discountValue > 1)
            {
                EB.Debug.LogError("LTChargeManager GetDiscountText is Error, discountValue more than 1, discountValue : {0}", discountValue);
                return string.Empty;
            }

            string text = string.Empty;
            text = string.Format(EB.Localizer.GetString("ID_CHARGE_DISCOUNT"), EB.Localizer.GetDiscountChange(discountValue));

            return text;
        }

        public static string GetDiscountTextEx(float discountValue)
        {
            if (discountValue <= 0)
            {
                EB.Debug.LogError("LTChargeManager GetDiscountText is Error, discountValue is fewer than 0, discountValue : {0}", discountValue);
                return string.Empty;
            }

            string text = string.Empty;
            text = string.Format("{0}%", (1f / discountValue * 100f).ToString("f0"));

            return text;
        }

        #endregion
    }

    public class LTChargeLimitTimeGiftData
    {
        public string TriggerID;
        public int PaymentId;
        public int OverTime;
        public List<int> OverTimeList;

        public LTChargeLimitTimeGiftData(string TriggerID, int PaymentId, List<int> OverTimeList)
        {
            this.TriggerID = TriggerID;
            this.PaymentId = PaymentId;
            this.OverTimeList = OverTimeList;
            OverTimeList.Sort((a, b) =>
            {
                if (a > b)
                    return 1;
                else
                    return -1;
            });
            OverTime = OverTimeList[0];
        }

        public void SetNext()
        {
            OverTimeList.Remove(OverTime);
            OverTime = OverTimeList[0];
        }
    }
}