using System.Collections;
using EB.Sparx;
using System;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTWelfareDataManager : ManagerUnit
    {
        public override void Dispose(){}
        public override void Connect(){}
        public override void Disconnect(bool isLogout){}

        private static LTWelfareDataManager sInstance = null;

        public static LTWelfareDataManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<LTWelfareDataManager>(); }
        }

        static public bool IsCanResignin
        {
            get { return ResidueCanResigninDays > 0 && ResidueResigninTimes > 0; }
        }

        static public int ResidueCanResigninDays
        {
            get
            {
                if (Instance.SignInData.IsSigned)
                {
                    return Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Day - Instance.SignInData.Num;
                }
                else
                {
                    return Hotfix_LT.Data.ZoneTimeDiff.GetServerTime().Day - Instance.SignInData.Num - 1;
                }
            }
        }

        static public int ResidueResigninTimes
        {
            get
            {
                return VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ResignInTimes) - Instance.SignInData.HaveResigninNum;
            }
        }

        private WelfareAPI API;
        public SignInData SignInData;
        public EverydayAwardInfo EverydayAwardData;

        private static string SignInDataId = "user_prize_data.sign_in";
        private static string UserPrizeDataId = "user_prize_data";

        public override void Initialize(Config config)
        {
            Instance.API = new WelfareAPI();
            Instance.API.ErrorHandler += ErrorHandler;

            SignInData = GameDataSparxManager.Instance.Register<SignInData>(SignInDataId);
            EverydayAwardData = GameDataSparxManager.Instance.Register<EverydayAwardInfo>(UserPrizeDataId);

            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.DataRefresh);
        }

        public void DataRefresh()
        {
            LTWelfareModel.Instance.Welfare_Main();
            LTWelfareModel.Instance.Welfare_SignIn();
            LTWelfareModel.Instance.ComeBack_Main();
        }

        public override void OnLoggedIn()
        {
            Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;
            object welfare = loginData[UserPrizeDataId];

            if (welfare == null)
            {
                EB.Debug.LogError("WelfareManager.OnLoggedIn: welfare not found in LoginData");
                return;
            }

            loginData.Remove(UserPrizeDataId);

            //上线获得七日最终奖或拉已完成任务列表
            bool hasGetGrowReward = true;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.grow_reward", out hasGetGrowReward);

            if (LTWelfareModel.Instance.JudgeGrowUpActivityClose())
            {
                if (!hasGetGrowReward)
                {
                    LTWelfareModel.Instance.DrawGrowReward(delegate (bool success)
                    {
                        DataLookupsCache.Instance.CacheData("user_prize_data.grow_reward", true);
                    }, true);
                }
            }
            else
            {
                LTWelfareModel.Instance.GetNewPlayerTasks();
            }

            if(LTWelfareModel.Instance.GetOpenComeBack()) LTWelfareModel.Instance.GetCombackLoginTasks();

            //是否购买过密令
            int count = 0;
            DataLookupsCache.Instance.SearchIntByID("userBattlePass.token", out count);
            LTWelfareModel.Instance.HasBattlePass = (count > 0);
        }

        public bool GetIsHaveSigninAward()
        {
            return !SignInData.IsSigned || IsCanResignin;
        }

        public bool GetIsHaveFirstChargeAward()
        {
            return BalanceResourceUtil.ChargeHc > 0 && EverydayAwardData.IsHaveReceiveFirstChargeGift == false;
        }

        public bool GetIsHaveEverydayAward()
        {
            if (!EverydayAwardData.IsHaveReceiveAllianceMonthCardGift && EverydayAwardData.ResidueAllianceMonthCardDay > 0)
                return true;
            if (!EverydayAwardData.IsHaveReceiveMonthCardGift && EverydayAwardData.ResidueMonthCardDay > 0)
                return true;
            if (EverydayAwardData.IsHaveShare && !EverydayAwardData.IsHaveReceiveShareGift)
                return true;
            if (!EverydayAwardData.IsHaveReceiveLoginGift)
                return true;
            if (BalanceResourceUtil.VipLevel > 0 && !EverydayAwardData.IsHaveReceiveVIPGift)
                return true;
            var now= Data.ZoneTimeDiff.GetServerTime();
            if (12 <= now.Hour && now.Hour < 14)
            {
                if (!EverydayAwardData.IsHaveReceiveVogirGift1)
                    return true;
            }
            else if (18 <= now.Hour && now.Hour < 20)
            {
                if (!EverydayAwardData.IsHaveReceiveVogirGift2)
                    return true;
            }
            return false;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        //api request
        public void Signin(System.Action<bool> callBack)
        {
            API.Signin(delegate (Hashtable result) {
                callBack(result != null);
                FetchDataHandler(result);
            });
        }

        public void AdditionalSignin(System.Action<bool> callBack)
        {
            API.AdditionalSignin(delegate (Hashtable result)
            {
                callBack(result != null);
                FetchDataHandler(result);
            });
        }

        public void ReceiveMonthCard(System.Action<bool> callBack)
        {
            API.ReceiveMonthCard(delegate (Hashtable result) {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void ReceiveAllianceMonthCard(System.Action<bool> callBack)
        {
            API.ReceiveAllianceMonthCard(delegate (Hashtable result) {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void ReceiveShareGift(System.Action<bool> callBack)
        {
            API.ReceiveShareGift(delegate (Hashtable result) {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void ReceiveEverydayGift(System.Action<bool> callBack)
        {
            API.ReceiveEverydayGift(delegate (Hashtable result) {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void ReceiveVigorGift(System.Action<bool> callBack)
        {
            API.ReceiveVigorGift(delegate (Hashtable result) {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void ReceiveVipGift(System.Action<bool> callBack)
        {
            API.ReceiveVipGift(delegate (Hashtable result) {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void GetFirstChargeInfo(System.Action<Hashtable> result)
        {
            API.GetFirstChargeInfo(result);
        }

        public void ReceiveFirstChargeGift(System.Action<bool> callBack)
        {
            API.ReceiveFirstChargeGift(delegate (Hashtable result) {
                callBack(result != null);
                FetchDataHandler(result);
            });
        }

        public void ReceiveSevendayAward(int id, System.Action<bool> callBack)
        {
            API.ReceiveSevendayAward(id, delegate (Hashtable result)
            {
                callBack(result != null);
                FetchDataHandler(result);
            });
        }

        public void ReceiveWeekendAward(int id, System.Action<bool> callBack)
        {
            API.ReceiveWeekendAward(id, delegate (Hashtable result)
            {
                callBack(result != null);
                FetchDataHandler(result);
            });
        }

        public void ReceiveGrowPlanAward(int id, System.Action<bool> callBack)
        {
            API.ReceiveGrowPlanAward(id, delegate (Hashtable result)
            {
                callBack(result != null);
                FetchDataHandler(result);
            });
        }

        private void FetchDataHandler(Hashtable payload)
        {
            if (payload != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(payload, false);

                payload.Remove(UserPrizeDataId);
                DataLookupsCache.Instance.CacheData(payload);
            }
        }

        private void MergeDataHandler(Hashtable payload)
        {
            if (payload != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
            }
        }
    }

    public class LTWelfareEvent
    {
        //public static Action WelfareHadOpen;
        public static Action WelfareOnfocus;

        public static Action WelfareHadFirstCharge;
        public static Action<int> WelfareGrowUpTabClick;
        public static Action WelfareGrowUpUpdata;
        public static Action WelfareGrowUpReset;
        public static Action WelfareSignInUpdata;

        public static Action WelfareDiamondReset;
        public static Action<int> WelfareDiamondDayItemClicked;
    }

    public class LTComeBackEvent
    {
        public static Action<int> ComeBackTabClick;
        public static Action ComeBackUpdata;

        public static Action ComeBackOnfocus;
        public static Action ComeBackResetView;
    }

    public class LTWelfareGrowUpTaskData
    {
        public int TaskId;
        public string State;
        public int TargetNum;
        public int CurNum;
        public bool Finished;

        /// <summary>
        /// 是否有密令奖励
        /// </summary>
        public bool hasBattlePass;
        /// <summary>
        /// 是否已领取密令奖励
        /// </summary>
        public bool hasGetBattlePassReward;

        /// <summary>
        /// 排序判断，任务是否属于完全完成
        /// </summary>
        /// <returns></returns>
        public bool SortBattlePass()
        {
            return State.Equals(TaskSystem.COMPLETED)&&(!hasBattlePass||(hasBattlePass &&hasGetBattlePassReward));
        }

        public bool JudgeCondition(int day, int type = -1)
        {
            bool bo = DayCondition == day && (type == -1 || TypeCondition == type);
            return bo;
        }
        public int DayCondition{
            get {return (TaskId % 1000) / 100; //与策划定的规则，百位区别天数
            }}
        public int TypeCondition{
            get{return (TaskId % 100)/10;//十位区别类型
            }
        }

        public Hotfix_LT.Data.TaskTemplate TaskTpl {
            get {
                if (TaskId == 0) return null;
                return Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(TaskId);
            }
        }
    }

    public class LTWelfareModel
    {
        private static LTWelfareModel instance = null;
        public static LTWelfareModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LTWelfareModel();
                }
                return instance;
            }
        }
        
        public List<LTWelfareGrowUpTaskData> mLTWelfareGrowUpTaskDatas=new List<LTWelfareGrowUpTaskData> ();

        public LTWelfareGrowUpTaskData FindGrowUpDataById(int id)
        {
            for(int i=0; i< mLTWelfareGrowUpTaskDatas.Count; i++)
            {
                if (mLTWelfareGrowUpTaskDatas[i].TaskId == id) return mLTWelfareGrowUpTaskDatas[i];
            }
            return null;
        }

        public bool JudgeViewClose(int index)
        {
            switch (index)
            {
                case 1:
                    return LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift ||
                    LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift1 &&
                    LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift2 &&
                    LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift3;
                case 2:
                    {
                        IDictionary cachedata;
                        if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.sevenday_reward", out cachedata)) return false;
                        List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(6001);
                        return cachedata.Count == stages.Count;
                    }
                case 3:
                    {
                        return LTWelfareModel.Instance.JudgeGrowUpActivityClose();
                    }
                case 4:
                    {
                        IDictionary cachedata;
                        if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.level_reward_extra", out cachedata)) return false;
                        List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(3001);
                        return cachedata.Count == stages.Count;
                    }
                case 5:
                    {
                        long timeFinal = 0;
                        DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.start_ts", out timeFinal);
                        return (timeFinal == 0);
                    }
                case 6:
                    return Welfare_DiamondGiftRemainTime() <= 0;
                case 7:
                    {
                        bool hasGet = true;
                        List<Hotfix_LT.Data.MainCampaignRewardTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetAllMainCampaignRewardTp1();
                        for(int i=0;i< stages.Count; ++i)
                        {
                            if (!DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userMainCampaignReward.redeem.{0}.extra", stages[i].stage), out hasGet)|| !hasGet)
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }

        public long Welfare_DiamondGiftRemainTime()
        {
            long timeJoin;
            DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
            long timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, 7);
            var ts = timeFinal - EB.Time.Now;
            return ts;
        }

        public bool JudgeGrowUpActivityClose()
        {
            long timeFinal = 0;
            long timeJoin = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
            timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, 7);
            long ts = timeFinal - EB.Time.Now;
            if (ts>0)
            {
                int TaskRefresh = 0;
                DataLookupsCache.Instance.SearchDataByID <int>("task_refresh", out TaskRefresh);
                if (TaskRefresh > 0)
                {
                    DataLookupsCache.Instance.CacheData("task_refresh", 0);
                    LTWelfareModel.Instance.GetNewPlayerTasks();
                }
            }
            return ts < 0;
        }
        
        public int completedNum = 0;
        public List<int> DayTabRPList;
        public List<int> TypeTabRPList;
        public List<int> DayTabUnPrefectList;
        public void UpdataTasks()
        {
            Hashtable datas;
            datas = TaskSystem.GetTaskHashTable();
            completedNum = 0;
            DayTabRPList = new List<int>();
            TypeTabRPList = new List<int>();
            DayTabUnPrefectList = new List<int>();
            mLTWelfareGrowUpTaskDatas = new List<LTWelfareGrowUpTaskData>();
            bool cheakBattlePass = HasBattlePass;
            if (datas != null)
            {
                foreach (DictionaryEntry v in datas)
                {
                    IDictionary dic = v.Value as IDictionary;
                    if (dic["task_type"]!=null && dic["task_type"].ToString() == "6")
                    {
                        LTWelfareGrowUpTaskData data = new LTWelfareGrowUpTaskData();
                        data.TaskId = EB.Dot.Integer("task_id", v.Value, 0);
                        data.State = EB.Dot.String("state", v.Value, null);
                        data.Finished = EB.Dot.Bool("event_count.finished", v.Value, false);
                        data.TargetNum = EB.Dot.Integer("event_count.target_num", v.Value, 0);
                        data.CurNum = EB.Dot.Integer("event_count.current_num", v.Value, 0);
                        data.hasBattlePass = Hotfix_LT.Data.TaskTemplateManager.Instance.CheckBattlePassByTaskId(data.TaskId);
                        if (data.hasBattlePass) data.hasGetBattlePassReward = HasGetBattlePassReward(data.TaskId);
                        if (data.State.Equals(TaskSystem.COMPLETED))
                        {
                            completedNum++;
                            if (cheakBattlePass && data.hasBattlePass && !data.hasGetBattlePassReward)
                            {
                                if (!DayTabRPList.Contains(data.DayCondition)) DayTabRPList.Add(data.DayCondition);
                                int tabIndex = (data.TaskId % 1000) / 10;
                                if (!TypeTabRPList.Contains(tabIndex)) TypeTabRPList.Add(tabIndex);
                            }
                            if (data.hasBattlePass && !data.hasGetBattlePassReward && !DayTabUnPrefectList.Contains(data.DayCondition))
                            {
                                DayTabUnPrefectList.Add(data.DayCondition);
                            }
                        }
                        else
                        {
                            if (!DayTabUnPrefectList.Contains(data.DayCondition))
                            {
                                DayTabUnPrefectList.Add(data.DayCondition);
                            }
                            if (data.Finished)
                            {
                                if (!DayTabRPList.Contains(data.DayCondition)) DayTabRPList.Add(data.DayCondition);
                                int tabIndex = (data.TaskId % 1000) / 10;
                                if (!TypeTabRPList.Contains(tabIndex)) TypeTabRPList.Add(tabIndex);
                            }
                        }
                        mLTWelfareGrowUpTaskDatas.Add(data);
                    }
                }
            }
        }
        private bool HasGetBattlePassReward(int id)
        {
            bool bo;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userBattlePass.rewards.{0}", id), out bo);
            return bo;
        }


        #region 红点相关
        public bool Welfare_Main()
        {
            return Welfare_FirstCharge(true) || Welfare_SevenDay() || Welfare_GrowUpAward() || Welfare_LevelAward() || Welfare_HeroMedal() || Welfare_DiamondGift()||Welfare_MainInstanceGift();

        }

        public bool Welfare_FirstCharge(bool RPRefresh=false)
        {
            bool b = BalanceResourceUtil.ChargeHc > 0 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift && (
                LTWelfareDataManager.Instance.EverydayAwardData.FirstChargeLoginDay > 0 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift1 ||
                LTWelfareDataManager.Instance.EverydayAwardData.FirstChargeLoginDay > 1 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift2 ||
                LTWelfareDataManager.Instance.EverydayAwardData.FirstChargeLoginDay > 2 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift3);
            if (RPRefresh)
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.firstcharge, (b)?1:0);
            }
            return b;
        }

        public bool Welfare_SevenDay()
        {
            return GetReceiveable();
        }
        private int LoginDayCount
        {
            get
            {
                int ldc = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.daily_login.login_day_count", out ldc);
                return ldc;
            }
        }
        private bool GetReceiveState(int day)
        {
            bool state = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.sevenday_reward.6001" + string.Format("{0:00}", day), out state);
            return state;
        }
        private bool GetReceiveable()
        {
            int ldc = LoginDayCount;
            for (int day = 1; day < 8; ++day)
            {
                if (day <= ldc)
                {
                    if (!GetReceiveState(day))
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.sevenreward, 1);
                        return true;
                    }
                }
                else
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.sevenreward, 0);
                    return false;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.sevenreward, 0);
            return false;
        }

        public bool Welfare_GrowUpAward()
        {
            Hashtable datas;
            datas = TaskSystem.GetTaskHashTable();
            long timeFinal = 0;
            long timeJoin = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
            timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, 7);
            long ts = timeFinal - EB.Time.Now;
            if (ts < 0 || datas == null)
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.growupaward, 0);
                return false;
            }

            int day = (int)(ts / (24 * 60 * 60));
            int GrowUpDayNum = 0;
            if (GrowUpDayNum != 7 - day)
            {
                GrowUpDayNum = 7 - day;
            }

            bool hasBattlePass;
            int count;
            DataLookupsCache.Instance.SearchIntByID("userBattlePass.token", out count);
            hasBattlePass = count > 0;
            int completedNum = 0;
            foreach (DictionaryEntry v in datas)
            {
                IDictionary dic = v.Value as IDictionary;
                if (dic["task_type"] != null && dic["task_type"].ToString() == "6")
                {
                    int Id = EB.Dot.Integer("task_id", v.Value, 0);
                    string State = EB.Dot.String("state", v.Value, "");
                    bool Finished = EB.Dot.Bool("event_count.finished", v.Value, false);
                    if (State.Equals(TaskSystem.COMPLETED))
                    {
                        completedNum++;
                    }

                    if (Finished && !State.Equals(TaskSystem.COMPLETED))
                    {
                        int curday = (Id % 1000) / 100;
                        if (curday < GrowUpDayNum)
                        {
                            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.growupaward, 1);
                            return true;
                        }
                    }
                    else if (State.Equals(TaskSystem.COMPLETED) && hasBattlePass && TaskTemplateManager.Instance.CheckBattlePassByTaskId(Id)) 
                    {
                        bool bo;
                        DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userBattlePass.rewards.{0}", Id), out bo);
                        if (!bo)
                        {
                            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.growupaward, 1);
                            return true;
                        }
                    }
                }
            }
            bool hasGetGrowReward = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.grow_reward", out hasGetGrowReward);
            if (!hasGetGrowReward && completedNum >= 60)//可领取最终奖励
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.growupaward, 1);
                return true;
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.growupaward, 0);
            return false;
        }
        public bool Welfare_GrowUpClose()
        {
            long timeFinal = 0;
            long timeJoin = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
            timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, 7);
            long ts = timeFinal - EB.Time.Now;
            if (ts < 0) return true;
            else return false;
        }

        public bool Welfare_LevelAward()
        {
            IDictionary cachedata;
            int level = BalanceResourceUtil.GetUserLevel();
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.level_reward", out cachedata);
            IDictionary cachedataExtra;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.level_reward_extra", out cachedataExtra);
            bool state = false;
            bool isVip = LTChargeManager.Instance.IsSilverVIP();
            List<TimeLimitActivityStageTemplate> stages = EventTemplateManager.Instance.GetTimeLimitActivityStages(3001); 
                               if (stages != null)
            {
                for (int i = 0; i < stages.Count; i++)
                {
                    if (level >= stages[i].stage)
                    {
                        if (!EB.Dot.Bool(stages[i].id.ToString(), cachedata, false))
                        {
                            state = true;
                            break;
                        }
                        else if (isVip && !EB.Dot.Bool(stages[i].id.ToString(), cachedataExtra, false))
                        {
                            state = true;
                            break;
                        }
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.levelaward, state?1:0);
            return state;
        }

        public bool Welfare_HeroMedal()
        {
            long timeFinal = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.last_reset_ts", out timeFinal);
            int resetData = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("HeroMedalResetDay");
            resetData = (resetData > 0) ? resetData : 30;
            if (timeFinal + resetData * 24 * 60 * 60 - EB.Time.Now <= 0)
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.heromedal, 0);
                return false;
            }
            int score;
            DataLookupsCache.Instance.SearchIntByID("userHeroMedal.score", out score);
            int extra;
            DataLookupsCache.Instance.SearchIntByID("userHeroMedal.extra", out extra);
            bool hasHeroMedal = extra > 0;

            var temps = EventTemplateManager.Instance.GetAllHeroMedalTp1();
            for (int i = 0; i < temps.Count; ++i)
            {
                if (score >= temps[i].stage)
                {
                    if (!HeroMedalRewardType(temps[i].stage) || hasHeroMedal && !HeroMedalExRewardType(temps[i].stage))
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.heromedal, 1);
                        return true;
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.heromedal, 0);
            return false;
        }

        public bool Welfare_SignIn()
        {
            bool b = LTWelfareDataManager.Instance.GetIsHaveSigninAward();
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.sign,b?1: 0);
            return b;
        }
        
        public bool Welfare_DiamondGift()
        {
            long ts = Welfare_DiamondGiftRemainTime();
            long uid =LoginManager .Instance .LocalUserId.Value;
            string key = uid.ToString() + "HasOpenWelfareDiamondGiftToday";

            if (ts < 0)
            {
                PlayerPrefs.DeleteKey(key);
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.diamondgift, 0);
                return false;
            }

            var dayNum = (int)(ts / (24 * 60 * 60));
            var curDay = 7 - dayNum;

            if (PlayerPrefs.GetInt(key) == curDay)
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.diamondgift, 0);
                return false;
            }

            for (var day = 1; day <= curDay; day++)
            {
                var list = EventTemplateManager.Instance.DiamondGiftDict[curDay]; 

                for (var i = 0; i < list.Count; i++)
                {
                    bool isBuy;
                    DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.diamond_gift." + list[i].id, out isBuy);

                    if (!isBuy)
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.diamondgift, 1);
                        return true;
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.diamondgift, 0);
            return false;
        }

        public bool Welfare_MainInstanceGift()
        {
            int score = GetMaxCampaignLevel();
            int extra;
            DataLookupsCache.Instance.SearchIntByID("userMainCampaignReward.extra", out extra);
            bool hasMain = extra > 0;

            var temps = EventTemplateManager.Instance.GetAllMainCampaignRewardTp1();
            for (int i = 0; i < temps.Count; ++i)
            {
                if (score >= temps[i].stage)
                {
                    if (!MainRewardType(temps[i].stage) || hasMain && !MainExRewardType(temps[i].stage))
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.maininstancegift, 1);
                        return true;
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.maininstancegift, 0);
            return false;
        }

        private int dayCount=0;
        public int WelfareGrowUpDayCount()
        {
            if(dayCount == 0)
            {
                UpdataTasks();
                List<int> daylist = new List<int>();
                for(int i = 0; i < mLTWelfareGrowUpTaskDatas.Count; i++)
                {
                    if (!daylist.Contains(mLTWelfareGrowUpTaskDatas[i].DayCondition)) daylist.Add(mLTWelfareGrowUpTaskDatas[i].DayCondition);
                }
                dayCount = daylist.Count;
            }
            return dayCount;
        }

        #endregion

        public LTWelfareGrowUpTaskData GetGrowUpByDayAndType(int day, int type)
        {
            List<LTWelfareGrowUpTaskData> temps = mLTWelfareGrowUpTaskDatas;
            LTWelfareGrowUpTaskData data = new LTWelfareGrowUpTaskData();
            for (int j =0; j< temps.Count;j++)
            {
                if (temps[j].JudgeCondition(day, type)) { data = temps[j]; break; }
            }
            return data;
        }
        
        public List<LTWelfareGrowUpTaskData> GetGrowUpsByDayAndType(int day,int type)
        {
            List<LTWelfareGrowUpTaskData> temps = mLTWelfareGrowUpTaskDatas;
            List<LTWelfareGrowUpTaskData> datas = new List<LTWelfareGrowUpTaskData>();
            for (int i=0;i< temps.Count; i++)
            {
                if (temps[i].JudgeCondition(day,type))datas.Add(temps[i]);
            }
            LTWelfareGrowUpTaskData temp = new LTWelfareGrowUpTaskData();
            for (int i=0;i< datas.Count;i++)
            {
                for(int j = i+1; j < datas.Count; j++)
                {
                    if (datas[i].SortBattlePass())
                    {
                        if (!datas[j].SortBattlePass())
                        {
                            temp = datas[i];
                            datas[i] = datas[j];
                            datas[j] = temp;
                            continue;
                        }
                    }
                    else if (datas[j].SortBattlePass())
                    {
                        continue;
                    }

                    if (datas[i].TaskId> datas[j].TaskId)
                    {
                        temp= datas[i];
                        datas[i] = datas[j];
                        datas[j] = temp;
                    }
                }
            }
            return datas;
        }

        public List<LTWelfareGrowUpTaskData> GetHandBookTasks()
        {
            List<LTWelfareGrowUpTaskData> temps = mLTWelfareGrowUpTaskDatas;
            List<LTWelfareGrowUpTaskData> datas = new List<LTWelfareGrowUpTaskData>();
            for (int i = 0; i < temps.Count; i++)
            {
                if (!temps[i].Finished &&temps[i].TaskTpl .target_type ==302&& temps[i].TaskTpl.target_parameter_1== "9") datas.Add(temps[i]);
            }
            return datas;
        }
        
        private bool hasBattlePass;
        public bool HasBattlePass
        {
            get
            {
                if (hasBattlePass) return true;
                int count;
                DataLookupsCache.Instance.SearchIntByID("userBattlePass.token", out count);
                hasBattlePass = count > 0;
                return hasBattlePass;
            }
            set
            {
                hasBattlePass = value;
            }
        }
        
        public bool HasHeroMedal
        {
            get
            {
                int count;
                DataLookupsCache.Instance.SearchIntByID("userHeroMedal.extra", out count);
                return count > 0;
            }
        }

        private int maxCampaignLevel = 0;
        public int GetMaxCampaignLevel()
        {
            return maxCampaignLevel;
        }
        public void SetMaxCampaignLevel()
        {
            maxCampaignLevel = 0;
            int maxLevel = LTInstanceMapModel.Instance.GetMaxCampaignLevel();
            int chapterid = maxLevel / 100;
            var temp=SceneTemplateManager.Instance.GetLostMainChatpterTplById(chapterid.ToString());
            if (temp != null)
            {
                int level = maxLevel % 100;
                maxCampaignLevel = chapterid - (temp.CampaignList.Count== level?0:1);
            }
        }

        public bool HasMain
        {
            get
            {
                int count;
                DataLookupsCache.Instance.SearchIntByID("userMainCampaignReward.extra", out count);
                return count > 0;
            }
        }

        public bool IsHeroMedalActivityOpen()
        {
            long timeFinal = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.start_ts", out timeFinal);
            return (timeFinal > 0);
        }

        public bool IsHeroMedalActivityOver()
        {
            long timeFinal = 0;
            int resetData = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("HeroMedalResetDay");
            resetData = (resetData > 0) ? resetData : 30;
            DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.last_reset_ts", out timeFinal);
            return timeFinal + resetData * 24 * 60 * 60 - EB.Time.Now <= 0;
        }

        public long GetHeroMedalActivityTime()
        {
            long timeFinal = 0;
            int resetData = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("HeroMedalResetDay");
            resetData = (resetData > 0) ? resetData : 30;
            DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.last_reset_ts", out timeFinal);
            return timeFinal + resetData * 24 * 60 * 60;
        }

        public void ResetHeroMedalData()
        {
            DataLookupsCache.Instance.CacheData("userHeroMedal.redeem", null);
            DataLookupsCache.Instance.CacheData("userHeroMedal.score", 0);
            DataLookupsCache.Instance.CacheData("userHeroMedal.extra", 0);
            int resetData = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("HeroMedalResetDay");
            resetData = (resetData > 0) ? resetData : 30;
            long RestDayTemp = 0;
            DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.last_reset_day", out RestDayTemp);
            DataLookupsCache.Instance.CacheData("userHeroMedal.last_reset_day", RestDayTemp + resetData);
            DataLookupsCache.Instance.SearchDataByID<long>("userHeroMedal.last_reset_ts", out RestDayTemp);
            DataLookupsCache.Instance.CacheData("userHeroMedal.last_reset_ts", RestDayTemp + resetData * 24 * 60 * 60);
        }

        public int HeroMedalStage
        {
            get
            {
                int count;
                DataLookupsCache.Instance.SearchIntByID("userHeroMedal.score", out count);
                return count;
            }
        }
        
        public bool HeroMedalRewardType(int stage)
        {
            bool type=false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userHeroMedal.redeem.{0}.normal", stage), out type);
            return type;
        }

        public bool HeroMedalExRewardType(int stage)
        {
            bool type = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string .Format ("userHeroMedal.redeem.{0}.extra", stage), out type);
            return type;
        }

        public bool MainRewardType(int stage)
        {
            bool type = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userMainCampaignReward.redeem.{0}.normal", stage), out type);
            return type;
        }

        public bool MainExRewardType(int stage)
        {
            bool type = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userMainCampaignReward.redeem.{0}.extra", stage), out type);
            return type;
        }

        #region API调用相关
        bool hasRequest = false;
        public void ReceiveSevendayAward(int id, System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/drawSevenDayReward");
            request.AddData("achievementId", id);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                hasRequest = false;
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }

        public void ReceiveFirstChargeGift(int day, System.Action<bool> callback)
        {
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) => {
                if ((!response.sucessful || response.fatal) && !response.error.Equals(string.Empty))
                {
                    if (response.error.Equals("user has not finish the first charge"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_RECEIVECHARGEGIFT_FAIL"));
                        return true;
                    }
                }
                return false;
            };

            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/drawFirstCharge");
            request.AddData("day", day);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }

        public void ReceiveLevelAward(int id, System.Action<bool> callback, int type = 0)
        {
            if (hasRequest) return;
            hasRequest = true;

            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) => {
                if ((!response.sucessful || response.fatal) && !response.error.Equals(string.Empty))
                {
                    if (response.error.Equals("ID_ERROR_STAGE_NOT_REACH"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, "ID_ERROR_STAGE_NOT_REACH");
                        return true;
                    }
                }
                return false;
            };

            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/drawLevelReward");
            request.AddData("achievementId", id);
            if (type > 0) { request.AddData("type", type); }
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
                hasRequest = false;
            });
        }

        public void ReceiveHeroMedalAward(int stage, bool isEx, System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;

            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) => {
                if ((!response.sucessful || response.fatal) && !response.error.Equals(string.Empty))
                {
                    if (response.error.Equals("ID_ERROR_STAGE_NOT_REACH"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, "ID_ERROR_STAGE_NOT_REACH");
                        return true;
                    }
                }
                return false;
            };

            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post(string.Format("/heromedal/{0}", isEx ? "getExtraReward" : "getReward"));
            request.AddData("stage", stage);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                hasRequest = false;
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }
        public void ReceiveMainAward(int stage, bool isEx, System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post(string.Format("/maincampaignreward/{0}", isEx ? "getExtraReward" : "getReward"));
            request.AddData("stage", stage.ToString());
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                hasRequest = false;
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }

        public void RequestComplete(string taskid, System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/mhjtasks/complete");
            request.AddData("task_id", taskid);

            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                hasRequest = false;
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }

        public void Signin(System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/signin");

            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
                hasRequest = false;
            });
        }

        public void AdditionalSignin(System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/additionalSignIn");

            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
                hasRequest = false;
            });
        }

        public void GetNewPlayerTasks(System.Action<bool> callback = null)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/mhjtasks/getNewPlayerTasks");
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                if (callback != null) callback(data != null);
            });
        }

        public void GetCombackLoginTasks(System.Action<bool> callback = null)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/mhjtasks/getCombackLoginTasks");
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                if (callback != null) callback(data != null);
            });
        }

        public void DrawGrowReward(System.Action<bool> callback, bool isAuto = false)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/drawGrowReward");
            request.AddData("isAuto", isAuto);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                if (!isAuto) DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }

        public void ResetHandBookData(System.Action<bool> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/buddy_mannual/refresh");

            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }

        public void RequestBattlePassComplete(string taskid, System.Action<bool> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/mhjtasks/getBattlePassReward");
            request.AddData("task_id", taskid);

            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                hasRequest = false;
                DataLookupsCache.Instance.CacheData(data);
                callback(data != null);
            });
        }
        #endregion

        #region  回归福利相关
        public const int ComeBackActivityID = 6601;
        public List<int> ComeBackDayTabRPList = new List<int>();
        public List<int> ComeBackDayTabUnPrefectList = new List<int>();
        public bool ComeBack_Main()
        {
            return ComeBack_Login() || ComeBack_Task();
        }

        public bool ComeBack_Login()
        {
            if (!LTWelfareModel.Instance.GetOpenComeBack())
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cblogin, 0);
                return false;
            }
            return GetComeBackReceiveable();
        }
        private bool GetComeBackReceiveState(int day)
        {
            bool state = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string .Format ("user_prize_data.afterdays_login_reward.{0}{1:00}", ComeBackActivityID, day), out state);
            return state;
        }
        private bool GetComeBackReceiveable()
        {
            int ldc = ComeBackLoginDayCount;
            for (int day = 1; day < 8; ++day)
            {
                if (day <= ldc)
                {
                    if (!GetComeBackReceiveState(day))
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cblogin, 1);
                        return true;
                    }
                }
                else
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cblogin, 0);
                    return false;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cblogin, 0);
            return false;
        }

        public bool ComeBack_Task()
        {
            if (!LTWelfareModel.Instance.GetOpenComeBack())
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cbtask, 0);
                return false;
            }
            Hashtable datas;
            datas = TaskSystem.GetTaskHashTable();
            if (datas != null)
            {
                foreach (DictionaryEntry v in datas)
                {
                    IDictionary dic = v.Value as IDictionary;
                    if (dic["task_type"] != null && dic["task_type"].ToString() == "9")
                    {
                        int Id = EB.Dot.Integer("task_id", v.Value, 0);
                        string State = EB.Dot.String("state", v.Value, "");
                        bool Finished = EB.Dot.Bool("event_count.finished", v.Value, false);

                        if (Finished && !State.Equals(TaskSystem.COMPLETED))
                        {
                            int curday = (Id % 1000) / 100;
                            if (curday < ComeBackLoginDayCount)
                            {
                                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cbtask, 1);
                                return true;
                            }
                        }
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.cbtask, 0);
            return false;
        }

        public bool GetOpenComeBack()
        {
            bool condition = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.combackData.offlineDay", out condition);
            return condition && EB.Time.Now < overTime;
        }

        public int ComeBackDayTabNum = -1;
        public List<LTWelfareGrowUpTaskData> mLTComebackTaskDatas = new List<LTWelfareGrowUpTaskData>();
        public void UpdataComeBackTasks()
        {
            Hashtable datas;
            datas = TaskSystem.GetTaskHashTable();
            mLTComebackTaskDatas.Clear();
            ComeBackDayTabRPList.Clear();
            ComeBackDayTabUnPrefectList.Clear();
            if (datas != null)
            {
                foreach (DictionaryEntry v in datas)
                {
                    IDictionary dic = v.Value as IDictionary;
                    if (dic["task_type"] != null && dic["task_type"].ToString() == "9")
                    {
                        LTWelfareGrowUpTaskData data = new LTWelfareGrowUpTaskData();
                        data.TaskId = EB.Dot.Integer("task_id", v.Value, 0);
                        data.State = EB.Dot.String("state", v.Value, null);
                        data.Finished = EB.Dot.Bool("event_count.finished", v.Value, false);
                        data.TargetNum = EB.Dot.Integer("event_count.target_num", v.Value, 0);
                        data.CurNum = EB.Dot.Integer("event_count.current_num", v.Value, 0);
                        if (!data.State.Equals(TaskSystem.COMPLETED))
                        {
                            if (data.Finished && !ComeBackDayTabRPList.Contains(data.DayCondition)) ComeBackDayTabRPList.Add(data.DayCondition);
                            if (!ComeBackDayTabUnPrefectList.Contains(data.DayCondition)) ComeBackDayTabUnPrefectList.Add(data.DayCondition);
                        }

                        mLTComebackTaskDatas.Add(data);
                    }
                }
            }
        }
        public List<LTWelfareGrowUpTaskData> GetComeBacksByDay(int day)
        {
            List<LTWelfareGrowUpTaskData> temps = mLTComebackTaskDatas;
            List<LTWelfareGrowUpTaskData> datas = new List<LTWelfareGrowUpTaskData>();
            for (int i = 0; i < temps.Count; ++i)
            {
                if (temps[i].JudgeCondition(day)) datas.Add(temps[i]);
            }
            LTWelfareGrowUpTaskData temp = new LTWelfareGrowUpTaskData();
            for (int i = 0; i < datas.Count; ++i)
            {
                for (int j = i + 1; j < datas.Count; j++)
                {
                    if (datas[i].State.Equals(TaskSystem.COMPLETED))
                    {
                        if (!datas[j].State.Equals(TaskSystem.COMPLETED))
                        {
                            temp = datas[i];
                            datas[i] = datas[j];
                            datas[j] = temp;
                            continue;
                        }
                    }
                    else if (datas[j].State.Equals(TaskSystem.COMPLETED))
                    {
                        continue;
                    }

                    if (datas[i].TaskId > datas[j].TaskId)
                    {
                        temp = datas[i];
                        datas[i] = datas[j];
                        datas[j] = temp;
                    }
                }
            }
            return datas;
        }
        public bool JudgeComeBackTaskOpenDay()
        {
            return ComeBackLoginDayCount >= ComeBackDayTabNum;
        }

        public int overTime
        {
            get
            {
                int mTime = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.combackData.expireTime", out mTime);
                return mTime;
            }
        }

        public int ComeBackLoginDayCount
        {
            get
            {
                int ldc = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.combackData.combackLoginCount", out ldc);
                return ldc;
            }
        }

        private int comebackDayCount = 0;
        public int ComebackDayCount()
        {
            if (comebackDayCount == 0)
            {
                UpdataComeBackTasks();
                List<int> daylist = new List<int>();
                for (int i = 0; i < mLTComebackTaskDatas.Count; i++)
                {
                    if (!daylist.Contains(mLTComebackTaskDatas[i].DayCondition)) daylist.Add(mLTComebackTaskDatas[i].DayCondition);
                }
                comebackDayCount = daylist.Count;
            }
            return comebackDayCount;
        }

        public LTWelfareGrowUpTaskData FindComebackDataById(int id)
        {
            for (int i = 0; i < mLTComebackTaskDatas.Count; i++)
            {
                if (mLTComebackTaskDatas[i].TaskId == id) return mLTComebackTaskDatas[i];
            }
            return null;
        }
        #endregion
    }

}
