using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EB.Sparx;
using System.Text;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public enum EDailyType
    {
        Limit,
        AllDay
    }

    public class LTDailyDataManager : ManagerUnit
    {
        private static LTDailyDataManager instance = null;

        private LTDailyData vitDailyData;
        public LTDailyData CurChoseData;
        private List<LTDailyData> mDailyData;
        private List<LTDailyData> mDailyLimitData;
        private List<LTDailyData> mDailyAllDayData;
        public bool iSNeedRefreshDailyData = false, iSNeedRefreshDailyShow = false, IsDailyDataInit = false;
        private LTDailyAPI Api;
        private List<int> doubleTimeActIDList = new List<int>() { 9010, 9013 };
        public LTBounTaskData bounTask
        {
            get; private set;
        }

        private Dictionary<int, string> mMonthVIPAdditionDic = new Dictionary<int, string>()
        {
            {9005, VIPPrivilegeKey.EscortTimes},
            {9006, VIPPrivilegeKey.RobTimes},
            {9009, VIPPrivilegeKey.InfiDiscountTimes},
            {9014, VIPPrivilegeKey.BountyTaskTimes},
            {9015, VIPPrivilegeKey.ArenaTimes},
        };

        public static int TimeNow
        {
            get { return EB.Time.Now;/*EB.Time.ToPosixTime(System.DateTime.UtcNow);*/ }
        }

        public static LTDailyDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTDailyDataManager>();
                }

                return instance;
            }
        }

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new LTDailyAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
            bounTask = GameDataSparxManager.Instance.Register<LTBounTaskData>("userTaskStatus");
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
        }

        private void DataRefresh()
        {
            if (Daily_Daily() || IsCouldReceiveVit(true)) { }
        }

        public override void OnLoggedIn()
        {
            IsDailyDataInit = false;
            mDailyLimitData = null;
            mDailyAllDayData = null;
            GetDailyData(SetActivityTime);
            if (Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10018).IsConditionOK())
            {
                ArenaManager.Instance.GetInfo();
            }
            AlliancesManager.Instance.GetTransferInfo();
        }

        private bool ErrorHandler(Response response, eResponseCode errCode)
        {
            return false;
        }

        public void InitDaiylData()
        {
            mDailyData = new List<LTDailyData>();
            vitDailyData = null;
            var datas = Hotfix_LT.Data.EventTemplateManager.Instance.GetAllSpecActData();

            for (int i = 0; i < datas.Count; i++)
            {
                mDailyData.Add(new LTDailyData());
                mDailyData[i].ActivityData = datas[i];
                SetDailyData(mDailyData[i]);
            }
        }

        private List<LTDailyData> GetDailyData(EDailyType type)
        {
            List<LTDailyData> datas = new List<LTDailyData>();
            if (mDailyData == null)
            {
                return datas;
            }

            for (int i = 0; i < mDailyData.Count; i++)
            {
                if (mDailyData[i].DailyType == type)
                {
                    datas.Add(mDailyData[i]);
                    mDailyData[i].IsSelected = false;
                }
            }

            datas.Sort(new LTDailyComparer());
            return datas;
        }

        private void RefreshDailyData()
        {
            //EB.Debug.LogError("RefreshDailyData");
            mDailyLimitData = null;
            mDailyAllDayData = null;
            mDailyLimitData = GetDailyData(EDailyType.Limit);
            mDailyAllDayData = GetDailyData(EDailyType.AllDay);
        }

        public List<LTDailyData> GetDailyDataByDailyType(EDailyType type)
        {
            if (iSNeedRefreshDailyData)
            {
                RefreshDailyData();
                iSNeedRefreshDailyData = false;
            }
            switch (type)
            {
                case EDailyType.Limit:
                    if (mDailyLimitData == null)
                    {
                        mDailyLimitData = GetDailyData(type);
                    }
                    return mDailyLimitData;
                case EDailyType.AllDay:
                    if (mDailyAllDayData == null)
                    {
                        mDailyAllDayData = GetDailyData(type);
                    }
                    return mDailyAllDayData;
                default:
                    return null;
            }
        }

        public void SetDailyDataRefreshState()
        {
            iSNeedRefreshDailyData = true;
            iSNeedRefreshDailyShow = true;
        }

        public bool GetDailyShowIconData(out LTDailyData curdata)
        {
            bool hasCurdata = false;
            CurChoseData = null;

            List<LTDailyData> tempdata = GetDailyDataByDailyType(EDailyType.Limit);
            if (tempdata != null && tempdata.Count > 0)
            {
                if (IsDailyDataCouldPlay(tempdata[0]))
                {
                    CurChoseData = tempdata[0];
                    hasCurdata = true;
                    curdata = CurChoseData;
                }

            }
            tempdata = GetDailyDataByDailyType(EDailyType.AllDay);
            if (!hasCurdata )
            {
                if (tempdata.Count > 0 &&IsDailyDataCouldPlay(tempdata[0]))
                {
                    CurChoseData = tempdata[0];
                    hasCurdata = true;
                    curdata = CurChoseData;
                    return hasCurdata;
                }
                else
                {
                    curdata = null;
                    return hasCurdata;
                }
            }
            curdata = CurChoseData;
            return hasCurdata;
        }

        private bool IsDailyDataCouldPlay(LTDailyData data)
        {
            if ((data.OpenTimeValue!=0&&data.StopTimeValue!=0)&&(data.OpenTimeValue > TimeNow || data.StopTimeValue < TimeNow))
            {
                return false;
            }
            else if (GetActivityCount(data.ActivityData.id) <= 0)
            {
                return false;
            }
            return true;
        }

        public List<LTDailyData> GetDailyDataAll()
        {
            return mDailyData;
        }

        public LTDailyData GetDailyDataByActivityID(int id)
        {
            for (int i = 0; i < mDailyData.Count; i++)
            {
                if (mDailyData[i].ActivityData.id == id) return mDailyData[i];
            }
            return null;
        }

        public bool Daily_Daily()
        {
            List<LTDailyData> curDailyData = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.AllDay);
            for (int i = 0; i < curDailyData.Count; i++)
            {
                if (curDailyData[i] == null) continue;
                var curData = curDailyData[i].ActivityData;
                var func = Data.FuncTemplateManager.Instance.GetFunc(curData.funcId);
                if (curData != null && (func == null || func.IsConditionOK()) && LTDailyDataManager.Instance.GetActivityCount(curData.id) > 0)
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.dailyact, 1);
                    return true;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.dailyact, 0);
            return false;
        }

        public int GetActivityCount(int activityID)//仅用于活动是否需要提示以及日常界面显示，部分活动次数不准确
        {
            Hotfix_LT.Data.SpecialActivityTemplate curAct = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(activityID);

            if (curAct == null)
            {
                return 0;
            }

            if (activityID == 9001 || activityID == 9002 || activityID == 9007)
            {
                //经验副本,金币副本，夺宝奇兵
                int times = 0;
                string path = string.Format("special_activity.{0}.c_times", activityID);
                DataLookupsCache.Instance.SearchDataByID(path, out times);
                return curAct.times - times;
            }
            else if(activityID == 9003)
            {
                DataLookupsCache.Instance.SearchDataByID("world_boss.fightTimes", out int ChallengeTime);
                DataLookupsCache.Instance.SearchDataByID("world_boss.buyTimes", out int BuyTimes);
                int Maxtime = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("worldBossChallengeMaxTimes");
                return Maxtime + BuyTimes - ChallengeTime;
            }else if(activityID == 9004)
            {//军团战
                //EB.Debug.LogError("军团战数据获取");
                if (!LTLegionWarManager.Instance.IsOpenWarTime()) return 0;
                LegionWarTimeLine status = LTLegionWarManager.GetLegionWarStatus();
                if (status == LegionWarTimeLine.QualifyGame)
                {
                    if (!AllianceUtil.IsJoinedAlliance) return 1;
                    int myScore = LTLegionWarManager.Instance.QualifyEnemyList.MyScore;
                    return myScore >= LTLegionWarManager.Instance.MaxScore ? 0 : 1;
                }else if(status == LegionWarTimeLine.SemiFinal)
                {
                    return 1;
                }
                else if(status == LegionWarTimeLine.Final)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }                   
            }
            else if (activityID == 9005)
            {
                //军团护送
                if (AlliancesManager.Instance.Account.State != eAllianceState.Joined)
                {
                    return curAct.times + GetVIPAdditionTimes(activityID);
                }
                return AllianceEscortUtil.GetResidueTransferDartNum();
            }
            else if (activityID == 9006)
            {
                //军团拦截
                if (AlliancesManager.Instance.Account.State != eAllianceState.Joined)
                {
                    return curAct.times + GetVIPAdditionTimes(activityID);
                }
                return AllianceEscortUtil.GetResidueRobDartNum();
            }
            else if (activityID == 9009)
            {
                //极限试炼
                int dayDisCountTime = 0;
                int oldVigor = 0;
                int NewVigor = 0;
                int times = LTUltimateTrialDataManager.Instance.GetCurrencyTimes();
                NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out dayDisCountTime, out NewVigor, out oldVigor);
                int curDisCountTime = dayDisCountTime - times;
                return  Mathf.Max(0,curDisCountTime);
                //if (!DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.challengeTimes", out times))
                //{
                //    times = curAct.times;
                //}
                //return Mathf.Max(0, times + GetVIPAdditionTimes(activityID));
            }else if(activityID == 9010)
            {//天梯
                LTDailyData tempdailydata = GetDailyDataByActivityID(9010);
                if (tempdailydata == null) return 0;
                else
                return tempdailydata.OpenTimeValue < LTDailyDataManager.TimeNow && tempdailydata.StopTimeValue > LTDailyDataManager.TimeNow ? 1 : 0;
            }
            else if(activityID == 9011)
            {//英雄交锋
                //今天赢了或者达到最大胜场 0 就不显示红点
                return LTNewHeroBattleManager.GetInstance().GetHeroBattleTipNum();
            }
            else if(activityID == 9013)
            {               
                return IsCouldReceiveVit()?1:0;
            }
            else if (activityID == 9014)
            {
                //幸运悬赏
                int times = LTBountyTaskHudController.CurHantTimes;
                return times;
            }
            else if (activityID == 9015)
            {
                //角斗场
                int totalTimes = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes) + ArenaManager.Instance.Info.buyTimes;
                int times = Mathf.Max(0, totalTimes - ArenaManager.Instance.Info.usedTimes);
                return times;
            }else if (activityID == 9016)
            {
                //荣耀角斗场
                int freetimes = HonorArenaManager.Instance.GetHonorArenaFreeTimes();  
                int usetimes = HonorArenaManager.Instance.Info.usedTimes;
                return freetimes-usetimes;
            }
            return 0;
        }


        private void SetDailyData(LTDailyData data, bool isInitOther = true)
        {
            Hotfix_LT.Data.ActivityTime openTime = GetActivityOpenTimeByActivityID(data.ActivityData.id);
            Hotfix_LT.Data.ActivityTime stopTime = GetActivityStopTimeByActivityID(data.ActivityData.id);

            if (doubleTimeActIDList.Contains(data.ActivityData.id) && isInitOther)
            {
                //天梯数据需要特殊处理，因为有双时段
                InitOtherData(openTime, stopTime, data);
            }

            data.DailyType = data.ActivityData.showType == 1 ? EDailyType.Limit : EDailyType.AllDay;
            data.OpenTimeWeek = openTime == null ? "*" : openTime.weekStr;
            System.DateTime startD = new System.DateTime();
            System.DateTime stopD = new System.DateTime();

            if (openTime != null && stopTime != null && openTime.hour >= 0 && stopTime.hour >= 0)
            {
                startD = Hotfix_LT.Data.ZoneTimeDiff.GetDateTime(openTime);
                data.OpenTimeValue = EB.Time.ToPosixTime(startD.ToUniversalTime());
                stopD = Hotfix_LT.Data.ZoneTimeDiff.GetDateTime(stopTime);
                data.StopTimeValue = EB.Time.ToPosixTime(stopD.ToUniversalTime());
            }

            StringBuilder str = new StringBuilder("");
            if (data.OpenTimeWeek.Contains("*"))
            {
                str.Append(EB.Localizer.GetString("ID_EVERYDAY"));
            }
            else if (!string.IsNullOrEmpty(data.OpenTimeWeek))
            {
                str = str.Append(EB.Localizer.GetString("ID_codefont_in_EventTemplateManager_43848") + " ");
                string[] strs = data.OpenTimeWeek.Split(',');
                for (int i = 0; i < strs.Length; i++)
                {
                    str.Append(EB.Localizer.GetString(Hotfix_LT.Data.EventTemplateManager.NumText[int.Parse(strs[i])]));
                    if (i < strs.Length - 1)
                    {
                        str.Append(",");
                    }
                }
            }

            if (stopTime == null || openTime == null || openTime.hour < 0 || stopTime.hour < 0)
            {
                str.Append(EB.Localizer.GetString("ID_codefont_in_LTDailyDataManager_6581"));
            }
            else
            {
                str.Append(string.Format(" {0:t}-{1:t}", startD, stopD));
            }

            data.OpenTimeStr = str.ToString();
        }

        private void InitOtherData(Hotfix_LT.Data.ActivityTime openTime, Hotfix_LT.Data.ActivityTime stopTime, LTDailyData data)
        {
            int nowTimestamp = EB.Time.Now;
            int stopD = Hotfix_LT.Data.ZoneTimeDiff.GetTime(stopTime);

            if (nowTimestamp > stopD)
            {
                string[] strHours = openTime.hourStr.Split(',');
                string[] strHourStops = stopTime.hourStr.Split(',');
                if (strHours.Length < 2 || strHourStops.Length < 2)
                {
                    return;
                }
                openTime.hour = int.Parse(strHours[1]);
                stopTime.hour = int.Parse(strHourStops[1]);

                string[] strMinutes = openTime.minuteStr.Split(',');
                if (strMinutes.Length > 1)
                {
                    openTime.minute = int.Parse(strMinutes[1]);
                }

                string[] strMinuteStops = stopTime.minuteStr.Split(',');
                if (strMinuteStops.Length > 1)
                {
                    stopTime.minute = int.Parse(strMinuteStops[1]);
                }
            }
            else
            {
                ILRTimerManager.instance.AddTimer((stopD - TimeNow - 1) * 1000, 1, (int i) =>
                {
                    string[] strHours = openTime.hourStr.Split(',');
                    string[] strHourStops = stopTime.hourStr.Split(',');
                    if (strHours.Length < 2 || strHourStops.Length < 2)
                    {
                        return;
                    }
                    openTime.hour = int.Parse(strHours[1]);
                    stopTime.hour = int.Parse(strHourStops[1]);

                    string[] strMinutes = openTime.minuteStr.Split(',');
                    if (strMinutes.Length > 1)
                    {
                        openTime.minute = int.Parse(strMinutes[1]);
                    }

                    string[] strMinuteStops = stopTime.minuteStr.Split(',');
                    if (strMinuteStops.Length > 1)
                    {
                        stopTime.minute = int.Parse(strMinuteStops[1]);
                    }

                    SetDailyData(data, false);
                });
            }
        }

        public int GetVIPAdditionTimes(int activityID)
        {
            int times = 0;
            if (mMonthVIPAdditionDic.ContainsKey(activityID))
            {
                times = VIPTemplateManager.Instance.GetTotalNum(mMonthVIPAdditionDic[activityID]);
            }

            return times;
        }

        /// <summary>
        /// 是否可以领取当前时段体力
        /// </summary>
        /// <returns></returns>
        public bool IsCouldReceiveVit(bool isRefreshPR = false)
        {
            if (mDailyData == null)
            {
                if (isRefreshPR) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.limitact, 0);
                return false;
            }

            if (vitDailyData == null)
            {
                for (int i = 0; i < mDailyData.Count; i++)
                {
                    if (mDailyData[i].ActivityData != null && mDailyData[i].ActivityData.id == 9013)
                    {
                        vitDailyData = mDailyData[i];
                    }
                }
            }

            if (vitDailyData == null)
            {
                EB.Debug.LogError("LTDailyDataManager IsCouldReceiveVit vitDailyData is Null, Error!!!");
                if (isRefreshPR) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.limitact, 0);
                return false;
            }

            Hotfix_LT.Data.ActivityTime openTime = GetActivityOpenTimeByActivityID(9013);

            if (openTime == null || openTime.hourStr.IndexOf(openTime.hour.ToString()) < 0)
            {
                if (isRefreshPR) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.limitact, 0);
                return false;
            }

            bool isCouldGet = false;
            if (openTime.hourStr.IndexOf(openTime.hour.ToString()) == 0)
            {
                isCouldGet = false;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.daily_login.is_draw_daily_vigor1", out isCouldGet);
            }
            else
            {
                isCouldGet = false;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.daily_login.is_draw_daily_vigor2", out isCouldGet);
            }

            isCouldGet = !isCouldGet && (vitDailyData.OpenTimeValue < TimeNow && vitDailyData.StopTimeValue > TimeNow);
            if (isRefreshPR) LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.limitact, isCouldGet ? 1 : 0);
            return isCouldGet;
        }

        #region API
        public void GetDailyData(System.Action<Hashtable> callback = null)
        {
            Api.RequestDailyData(delegate (Hashtable result)
            {
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void GetVit(System.Action<Hashtable> callback = null)
        {
            Api.ExceptionFun = (error) =>
            {
                if (error.Equals("user can not draw daily vigor now") || error.Equals("user has drew the first daily vigor") ||
                error.Equals("user has drew the all daily vigor") || error.Equals("user has drew the second daily vigor"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDailyDataManager_9876"));
                    return true;
                }
                return false;
            };

            Api.RequestVit(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void BuyDailyVigor(System.Action<Hashtable> callback = null)
        {
            Api.RequestBuyVit(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback(result);
                }
            });
        }
        #endregion

        #region From: Hotfix_LT.Data.EventTemplateManager
        private Dictionary<int, Hotfix_LT.Data.ActivityTime> mActivityOpenTime = null;//所有的活动开启时间
        private Dictionary<int, Hotfix_LT.Data.ActivityTime> mActivityStopTime = null;//所有的活动关闭时间

        public void SetActivityTime(Hashtable table)
        {
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("eventTimeSet", table, null);
            mActivityOpenTime = new Dictionary<int, Hotfix_LT.Data.ActivityTime>();
            mActivityStopTime = new Dictionary<int, Hotfix_LT.Data.ActivityTime>();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    int id = EB.Dot.Integer("id", list[i], 0);
                    Hashtable openTable = EB.Dot.Object("start_time", list[i], null);
                    Hashtable stopTable = EB.Dot.Object("stop_time", list[i], null);
                    if (openTable != null)
                    {
                        mActivityOpenTime.Add(id, GetActivityTime(openTable));
                    }
                    if (stopTable != null)
                    {
                        mActivityStopTime.Add(id, GetActivityTime(stopTable));
                    }
                }
            }

            InitDaiylData();
            IsDailyDataInit = true;
            UserData.PushMsgPrefs();
        }

        private Hotfix_LT.Data.ActivityTime GetActivityTime(Hashtable table)
        {
            Hotfix_LT.Data.ActivityTime actTime = new Hotfix_LT.Data.ActivityTime();
            actTime.monthStr = EB.Dot.String("month", table, "");
            actTime.weekStr = EB.Dot.String("dayOfWeek", table, "");
            actTime.dayStr = EB.Dot.String("dayOfMonth", table, "");
            actTime.hourStr = EB.Dot.String("hour", table, "");
            actTime.minuteStr = EB.Dot.String("minute", table, "");
            actTime.secondStr = EB.Dot.String("second", table, "");

            actTime.month = GetIntValue(actTime.monthStr);
            actTime.day = GetIntValue(actTime.dayStr);
            actTime.hour = GetIntValue(actTime.hourStr);
            actTime.minute = GetIntValue(actTime.minuteStr);
            actTime.second = GetIntValue(actTime.secondStr);

            return actTime;
        }

        private int GetIntValue(string str)
        {
            int value = 0;
            if (str.Contains("*"))
            {
                value = -1;
            }
            else if (!string.IsNullOrEmpty(str))
            {
                string[] strs = str.Split(',');
                int.TryParse(strs[0], out value);
            }

            return value;
        }

        public Hotfix_LT.Data.ActivityTime GetActivityOpenTimeByActivityID(int actID)
        {
            // 如果是全天的活动只有开始时间
            Hotfix_LT.Data.ActivityTime actTime = null;
            mActivityOpenTime.TryGetValue(actID, out actTime);
            return actTime;
        }

        public Hotfix_LT.Data.ActivityTime GetActivityStopTimeByActivityID(int actID)
        {
            // 如果是全天的活动是没有结束时间的，这里返回的是null
            Hotfix_LT.Data.ActivityTime actTime = null;
            mActivityStopTime.TryGetValue(actID, out actTime);
            return actTime;
        }
        #endregion
    }

    //public class LTDailyComparer : IComparer<LTDailyData>
    //{
    //    public int Compare(LTDailyData x, LTDailyData y)
    //    {
    //        int flg = 0;

    //        int xLock = x.ActivityData.level > BalanceResourceUtil.GetUserLevel() ? 0 : 1;
    //        int yLock = y.ActivityData.level > BalanceResourceUtil.GetUserLevel() ? 0 : 1;

    //        flg = yLock - xLock;
    //        if (flg != 0)
    //        {
    //            return flg;
    //        }

    //        if (xLock == 0 && yLock == 0)
    //        {
    //            return x.ActivityData.level - y.ActivityData.level;
    //        }

    //        int curWeek = EB.Time.LocalWeek;
    //        int xWeek = x.OpenTimeWeek.Contains(curWeek.ToString()) || x.OpenTimeWeek.Contains("*") ? 0 : 1;
    //        int yWeek = y.OpenTimeWeek.Contains(curWeek.ToString()) || y.OpenTimeWeek.Contains("*") ? 0 : 1;

    //        flg = xWeek - yWeek;
    //        if (flg != 0)
    //        {
    //            return flg;
    //        }

    //        if (x.DailyType == EDailyType.Limit && y.DailyType == EDailyType.Limit)
    //        {
    //            int xTime = x.OpenTimeValue < LTDailyDataManager.TimeNow && x.StopTimeValue > LTDailyDataManager.TimeNow ? 0 : 1;
    //            int yTime = y.OpenTimeValue < LTDailyDataManager.TimeNow && y.StopTimeValue > LTDailyDataManager.TimeNow ? 0 : 1;

    //            flg = xTime - yTime;
    //            if (flg != 0)
    //            {
    //                return flg;
    //            }

    //            int xHaveCount = LTDailyDataManager.Instance.GetActivityCount(x.ActivityData.id) > 0 ? 0 : 1;
    //            int yHaveCount = LTDailyDataManager.Instance.GetActivityCount(y.ActivityData.id) > 0 ? 0 : 1;

    //            flg = xHaveCount - yHaveCount;
    //            if (flg != 0)
    //            {
    //                return flg;
    //            }

    //            return y.OpenTimeValue - x.OpenTimeValue;
    //        }

    //        return x.ActivityData.id - y.ActivityData.id;
    //    }
    //}
    public class LTDailyComparer : IComparer<LTDailyData>
    {
        public int Compare(LTDailyData x, LTDailyData y)
        {
            int flg = 0;
            var xfun = FuncTemplateManager.Instance.GetFunc(x.ActivityData.funcId);
            var yfun = FuncTemplateManager.Instance.GetFunc(y.ActivityData.funcId);
            if (xfun != null && yfun != null)
            {
                int xLock = xfun.IsConditionOK() ? 1 : 0;
                int yLock = yfun.IsConditionOK() ? 1 : 0;
                flg = yLock - xLock;
                if (flg != 0)
                {
                    return flg;
                }

                if (xLock == 0 && yLock == 0)
                {
                    return xfun.conditionParam - xfun.conditionParam;
                }
            }
            int curWeek = EB.Time.LocalWeek;
            int xWeek = x.OpenTimeWeek.Contains(curWeek.ToString()) || x.OpenTimeWeek.Contains("*") ? 0 : 1;
            int yWeek = y.OpenTimeWeek.Contains(curWeek.ToString()) || y.OpenTimeWeek.Contains("*") ? 0 : 1;

            flg = xWeek - yWeek;
            if (flg != 0)
            {
                return flg;
            }
            int xTime, yTime;
            if (x.DailyType == EDailyType.AllDay && y.DailyType == EDailyType.AllDay)
            {
                if (x.OpenTimeValue == 0 && x.StopTimeValue == 0)
                {
                    xTime = 0;
                }
                else if (x.OpenTimeValue < LTDailyDataManager.TimeNow && x.StopTimeValue > LTDailyDataManager.TimeNow)
                {
                    xTime = 0;
                }
                else
                {
                    xTime = 1;
                }
                if (y.OpenTimeValue == 0 && y.StopTimeValue == 0)
                {
                    yTime = 0;
                }
                else if (y.OpenTimeValue < LTDailyDataManager.TimeNow && y.StopTimeValue > LTDailyDataManager.TimeNow)
                {
                    yTime = 0;
                }
                else
                {
                    yTime = 1;
                }
            }
            else
            {
                xTime = x.OpenTimeValue < LTDailyDataManager.TimeNow && x.StopTimeValue > LTDailyDataManager.TimeNow ? 0 : 1;
                yTime = y.OpenTimeValue < LTDailyDataManager.TimeNow && y.StopTimeValue > LTDailyDataManager.TimeNow ? 0 : 1;
            }

            flg = xTime - yTime;//是否在开放时间内
            if (flg != 0)
            {
                return flg;
            }
            if (xTime == 0 && yTime == 0)
            {
                int xHaveCount = LTDailyDataManager.Instance.GetActivityCount(x.ActivityData.id) > 0 ? 0 : 1;
                int yHaveCount = LTDailyDataManager.Instance.GetActivityCount(y.ActivityData.id) > 0 ? 0 : 1;

                flg = xHaveCount - yHaveCount;

                if (flg != 0)
                {
                    return flg;
                }
            }
            if (x.DailyType == EDailyType.Limit && y.DailyType == EDailyType.Limit)
            {


                int xRemainTime = x.StopTimeValue - x.OpenTimeValue;
                int yRemainTime = y.StopTimeValue - y.OpenTimeValue;
                flg = xRemainTime - yRemainTime;
                if (flg != 0)
                {
                    return flg;
                }
                return y.OpenTimeValue - x.OpenTimeValue;
            }
            if (x.DailyType == EDailyType.AllDay && y.DailyType == EDailyType.AllDay)
            {
                //需要新增排序字段
                return x.ActivityData.order - y.ActivityData.order;
            }

            return x.ActivityData.id - y.ActivityData.id;
        }
    }
}