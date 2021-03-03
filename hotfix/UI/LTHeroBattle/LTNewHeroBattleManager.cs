using UnityEngine;
using System.Collections;
using EB.Sparx;
using System.Collections.Generic;
using Hotfix_LT.Data;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public enum HeroBattleType
    {
        Null=-1,
        Newbie=0,
        Middle=1,
        High=2
    }
    
    public class LTNewHeroBattleManager : ManagerUnit
    {
        public static int FirstLayMax = 110;  //新手训练最大层数
        public static int SecondLayMax = 210; //高手进阶最大层数
        
        private static LTNewHeroBattleManager _instance;
        public static LTNewHeroBattleManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = LTHotfixManager.GetManager<LTNewHeroBattleManager>();
            }
            return _instance;
        }

        private LTNewHeroBattleAPI api;
        
        private HeroBattleType currentType=HeroBattleType.Null;

        public HeroBattleType CurrentType
        {
            get
            {
                int lay=GetCurrentFinishLayer();
                if (lay<FirstLayMax)
                {
                    currentType = HeroBattleType.Newbie;
                }else if (lay<SecondLayMax)
                {
                    currentType = HeroBattleType.Middle;
                }else if (lay>=SecondLayMax)
                {
                    currentType = HeroBattleType.High;
                }
                return currentType;
            }
        }

        public HeroBattleType GetCacheCurrentType()
        {
            if (currentType == HeroBattleType.Null) return CurrentType;
            return currentType;
        }

        public override void Initialize(Config config)
        {
            GetInstance().api = new LTNewHeroBattleAPI();
            CostAcountDic = null;
        }

        public override void OnLoggedIn()
        {
            GetNewHeroBattleInfo(delegate { LTDailyDataManager.Instance.SetDailyDataRefreshState(); });
        }
        public override void Dispose()
        {
            CostAcountDic = null;
            base.Dispose();
        }

        public override void Connect()
        {
            //State = EB.Sparx.SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {
            //State = EB.Sparx.SubSystemState.Disconnected;
            finLayer = 100;
        }


        #region 新英雄交锋的协议
        /// <summary>最高花费 </summary>
        public int NewHeroBattleMaxCost
        {
            get
            {
                return Mathf.Max(1, (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("heroBattleMaxCost"));
            }
        }
        /// <summary>已花费 </summary>
        public int NewHeroBattleCurCost
        {
            get
            {
                int curCost;
                DataLookupsCache.Instance.SearchIntByID("userClashOfHeroes.score", out curCost);
                return curCost;
            }
        }
        /// <summary>最大胜场 </summary>
        public int NewHeroBattleMaxWinCount
        {
            get
            {
                IDictionary dic;
                DataLookupsCache.Instance.SearchDataByID<IDictionary>("userClashOfHeroes.opponent", out dic);
                return dic.Count;
            }
        }
        /// <summary>当前胜场 </summary>
        public int NewHeroBattleCurWinCount
        {
            get
            {
                int CurWinCount;
                DataLookupsCache.Instance.SearchIntByID("userClashOfHeroes.win", out CurWinCount);
                return CurWinCount;
            }
        }

        public bool IsTodayWinOneTeam()
        {
            IDictionary dic;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("userClashOfHeroes.opponent", out dic);
            bool iswin;
            if (dic == null) return false;
            for (int i = 0; i < dic.Count; i++)
            {
                iswin = EB.Dot.Bool(string.Format("{0}.dead",i), dic, false);
                if (iswin)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetHeroBattleTipNum()
        {
            if (NewHeroBattleCurWinCount == EventTemplateManager.Instance.GetClashOfHeroesRewardTpls().Count)
            {
                return 0;
            }

            long time;
            DataLookupsCache.Instance.SearchDataByID<long>("userClashOfHeroes.last_training_time", out time);
            if (EB.Time.IsLocalToday(time))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        
        /// <summary>等阶花费字典 </summary>
        private Dictionary<PartnerGrade, int> CostAcountDic;
        /// <summary>已挑战过的英雄列表 </summary>
        public List<int> HasChallengeHeroInfoID;
        /// <summary>
        /// 由等阶获取花费
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public int GetCostByRoleGrade(int grade)
        {
            if (CostAcountDic == null || CostAcountDic.Count == 0)
            {
                string str = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("heroBattleCostByRoleGrade");
                string[] strs = null;
                if (str != null)
                {
                    strs = str.Split(',');
                }
                if (strs != null && strs.Length == 5)
                {
                    CostAcountDic = new Dictionary<PartnerGrade, int>()
                {
                    { PartnerGrade.UR,int.Parse(strs[4])},
                    { PartnerGrade.SSR , int.Parse(strs[3])},
                    { PartnerGrade.SR , int.Parse(strs[2])},
                    { PartnerGrade.R , int.Parse(strs[1])},
                    { PartnerGrade.N , int.Parse(strs[0])}
                };
                }
                else
                {
                    EB.Debug.LogWarning("NewGameConfig heroBattleCostByRoleGrade is Error!!");
                    CostAcountDic = new Dictionary<PartnerGrade, int>()
                {
                    { PartnerGrade.UR ,  4},
                    { PartnerGrade.SSR , 4},
                    { PartnerGrade.SR , 2},
                    { PartnerGrade.R , 0},
                    { PartnerGrade.N , 0}
                };
                }
            }
            return CostAcountDic[(PartnerGrade)grade];
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="callback"></param>
        public void GetNewHeroBattleInfo(System.Action<bool> callback = null)
        {
            api.GetMatchBaseInfo((EB.Sparx.Response response) =>
            {
                HasChallengeHeroInfoID = new List<int>();
                if (response.sucessful)
                {
                    DataLookupsCache.Instance.CacheData("userClashOfHeroes", null);
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    GetCacheCurrentType();
                    ArrayList list = Hotfix_LT.EBCore.Dot.Array("userClashOfHeroes.usedTpls", response.hashtable, null);
                    if (list != null)
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            string str = list[i] as string;
                            HasChallengeHeroInfoID.Add(int.Parse(str));
                        }
                    }
                    if (callback != null)
                        callback(true);
                    return true;
                }
                if (callback != null)
                    callback(false);
                return false;
            });
        }
        /// <summary>
        /// 新英雄交锋挑战
        /// </summary>
        /// <param name="callback"></param>
        public void GetNewHeroBattleChallenge(int index, System.Action<bool> callback = null)
        {
            api.StartCombat(index, (EB.Sparx.Response response) =>
            {
                if (response.sucessful)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    if (callback != null)
                        callback(true);
                    return true;
                }
                if (callback != null)
                    callback(false);
                return false;
            });
        }
        
        public void GetNewHeroBattleChallenge(List<int> data,int index, System.Action<bool> callback = null)
        {
            api.StartCombat(data,index, (EB.Sparx.Response response) =>
            {
                if (response.sucessful)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    if (callback != null)
                        callback(true);
                    return true;
                }
                if (callback != null)
                    callback(false);
                return false;
            });
        }
        #endregion
        
        
        
        public bool JudgeFinish(int ID)
        {
             // return true;
            bool isFinish = false;
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userClashOfHeroes.campaigns", out temp))
            {
                if (DataLookupsCache.Instance.SearchDataByID
                    (string.Format("userClashOfHeroes.campaigns.{0}.finish", ID), out isFinish))
                    return true;
            }
            return isFinish;
        }

        public bool JudgeIsLock(int ID, int Stage)
        {
            if (Stage == 1) return false;
            bool isLock = true;
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userClashOfHeroes.campaigns", out temp))
            {
                if (DataLookupsCache.Instance.SearchDataByID
                    (string.Format("userClashOfHeroes.campaigns.{0}.finish", ID - 1), out isLock))
                    return false;
            }
            return true;
        }

        int finLayer = 100;
        private bool isCache;
        public int GetCurrentFinishLayer()
        {
            IDictionary temp;
            if (DataLookupsCache.Instance.SearchDataByID("userClashOfHeroes.campaigns", out temp))
            {
                foreach (DictionaryEntry entry in temp)
                {
                    string key = (string)entry.Key;
                    if (int.Parse(key) > finLayer)
                    {
                        finLayer = int.Parse(key);
                    }
                }
            }

            isCache = true;
            return finLayer;
        }

        public int GetCacheFinishLayer()
        {
            if (finLayer == 100 && !isCache) return GetCurrentFinishLayer();
            return finLayer;
        }
        
        public bool GetHeroBattleTypeIsLock(HeroBattleType i)
        {
            
            if (i==HeroBattleType.Newbie)
            {
                return false;
            }
            if (i==HeroBattleType.Middle)
            {
                return !JudgeFinish(FirstLayMax);
            }
            if (i==HeroBattleType.High)
            {
                return !JudgeFinish(SecondLayMax);
            }

            return true;
        }
        
        
    }
}