using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Combat;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialDataManager
    {
        private static LTUltimateTrialDataManager instance;

        public static LTUltimateTrialDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LTUltimateTrialDataManager();
                }
                return instance;
            }
        }

        public int curSelectLevel;
        public System.Action<int> OnLevelSelect;

        public System.Action OnGotoBtnClick;

        public System.Action OnResetTimesLabel;

        public int GetUidFromLowest()
        {
            int uid = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.lowest.uid", out uid);
            return uid;
        }

        public string GetHeadFrameFromLowest()
        {
            string headFrame;
            DataLookupsCache.Instance.SearchDataByID("infiniteChallenge.info.lowest.headFrame", out headFrame);
            return EconemyTemplateManager.Instance.GetHeadFrame(headFrame).iconId;
        }

        public string GetAvatarFromLowest()
        {
            string characterId;
            DataLookupsCache.Instance.SearchDataByID("infiniteChallenge.info.lowest.tid", out characterId);

            int skinId;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.lowest.skin", out skinId);

            var heroInfo = CharacterTemplateManager.Instance.GetHeroInfo(characterId, skinId);

            if (heroInfo != null)
            {
                return heroInfo.icon;
            }

            return string.Empty;
        }

        public int GetCurLayer()
        {
            int levelNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.currentlayer", out levelNum);

            int MaxNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.toplayer", out MaxNum);

            return levelNum > MaxNum ? MaxNum : levelNum;
        }

        public bool IsLayerComplete()
        {
            bool isComplete = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("infiniteChallenge.info.isComplete", out isComplete);
            return isComplete;
        }
        
        public bool IsTopLayer(int layer)
        {
            int MaxNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.toplayer", out MaxNum);

            return layer == MaxNum;
        }

        public bool IsGetReward()
        {
            bool isGetReward = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("infiniteChallenge.info.isGetReward", out isGetReward);
            return isGetReward;
        }

        public bool UltimateTrialRP()//极限试炼红点判断
        {
            FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10015);
            if (m_FuncTpl.IsConditionOK())
            {
                bool rp = GetChallengeTimes() > 0;
                if (!rp)//新增竞速模式红点
                {
                    rp= GetCompeteRP();
                }
                return rp;
            }
            else
            {
                return false;
            }
        }

        public int GetChallengeTimes()
        {
            int times = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.challengeTimes", out times);
            return times + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InfiniteChallengeTimes);
        }
        
        public int GetCurrencyTimes()
        {
            int times = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.currentTimes", out times);
            return times ;
        }

        public int GetPeoplePassNum()
        {
            int i = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.clearanceNum", out i);
            return i;
        }

        public bool IsMaxLayer()
        {
            int levelNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.currentlayer", out levelNum);
            int MaxNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.toplayer", out MaxNum);

            return levelNum == MaxNum;
        }

        public bool IsOverMaxLayer()
        {
            int levelNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.currentlayer", out levelNum);
            int MaxNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.info.toplayer", out MaxNum);

            return (levelNum - MaxNum) >= 1;
        }

        public bool isFirstEnterUI()
        {
            int levelNum = 0;
            DataLookupsCache.Instance.SearchIntByID("infiniteChallenge.currentLevel", out levelNum);
            return (levelNum==0);
        }

        public List<LTShowItemData> GetFirstReward()
        {
            var data = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("infiniteChallenge.firstAward", out data);
            return ParseReward(data);
        }

        public List<LTShowItemData> GetSweepReward()
        {
            var data = Johny.ArrayListPool.Claim();
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("infiniteChallenge.sweepAward", out data);
            return ParseReward(data);
        }

        public int GetChallengeMaxTimes
        {
            get
            {
                return Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(9009).times + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InfiniteChallengeTimes);
            }
        }

        private List<LTShowItemData> ParseReward(ArrayList data)
        {
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.infinite_topic,
                FusionTelemetry.GamePlayData.infinite_event_id,FusionTelemetry.GamePlayData.infinite_umengId,"reward");
            List<LTShowItemData> list = new List<LTShowItemData>();
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (data[i] == null) continue;
                    string type = EB.Dot.String("type", data[i], string.Empty);
                    string id = EB.Dot.String("data", data[i], string.Empty);
                    int num = EB.Dot.Integer("quantity", data[i], 0);
                    if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id))
                    {
                        LTShowItemData item = new LTShowItemData(id, num, type);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public void RequestGetInfo(System.Action callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/getInfo");
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback?.Invoke();
            });
        }

        public void RequestStartChallenge(int level)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/startChallenge");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
            });
        }

        public void RequestGetFirstReward(int level, System.Action callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/getFirstReward");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback?.Invoke();
            });
        }

        public void RequestGetLowestTeamViews(int level, System.Action<Hashtable> callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/getLowestTeamViews");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                callback?.Invoke(data);
            });
        }

        bool _isPost = false;
        public void RequestSweepByIndex(int level, System.Action callback)
        {
            if (_isPost) return; //同时只能允许一次 等待回复
            _isPost = true;
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/startSweepByIndex");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback?.Invoke();
                _isPost = false;
            });
        }

        public void RequestEnterNextLayer(System.Action callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/infinitechallenge/enterNextLayer");
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback?.Invoke();
            });
        }

        #region  竞速模式
        public System.Action<int> OnCompeteSelect;
        public int curCompeteLevel = 0;

        public bool IsCanCompete()
        {
            int layer = GetCurLayer();
            int level = GetCompeteCondition();
            return layer > level;
        }

        public int GetCompeteCondition()
        {
            int level = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("InfiniteCompetePass");
            return level;
        }

        public bool GetCompeteRP()
        {
            bool rp = GetCurLayer() > Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("InfiniteCompetePass");
            if (rp)
            {
                rp = GetCurCompeteLevel() <= 1&& LTUltimateTrialDataManager.Instance.GetTimeTip(false, true).Equals("open");
            }
            return rp;
        }

        public int GetCurCompeteLevel()
        {
            int levelNum = 0;
            DataLookupsCache.Instance.SearchIntByID("speedinfiniteChallenge.info.highestlayer", out levelNum);
            int max = EventTemplateManager.Instance.GetAllInfiniteCompete().Count;
            return Mathf.Clamp(levelNum, 1, max);
        }

        public string GetCurCompeteLayout(int id)
        {
            string layout = string.Empty;
            DataLookupsCache.Instance.SearchDataByID(string.Format("speedinfiniteChallenge.info.layouts.{0}", id), out layout);
            return layout;
        }

        public int GetCurCompeteTotleTime()
        {
            int time = -1;
            DataLookupsCache.Instance.SearchDataByID("speedinfiniteChallenge.info.totalFinishTime", out time);
            return time;
        }

        public int GetCurCompeteRealmTotleTime()
        {
            int time = -1;
            DataLookupsCache.Instance.SearchDataByID("speedinfiniteChallenge.realmInfo.totalFinishTime", out time);
            return time;
        }

        public int GetCurCompeteRealmInfo()
        {
            int time = -1;
            DataLookupsCache.Instance.SearchDataByID(string.Format("speedinfiniteChallenge.realmInfo.metric.{0}.useTime", curCompeteLevel), out time);
            return time;
        }

        public Hashtable GetCurCompeteRealmInfoTeam()
        {
            Hashtable data;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("speedinfiniteChallenge.realmInfo.metric.{0}.teamView", curCompeteLevel), out data);
            return data;
        }

        public int GetCurCompeteTime(int id)
        {
            int time = -1;
            DataLookupsCache.Instance.SearchDataByID(string.Format("speedinfiniteChallenge.info.metric.{0}.useTime", id), out time);
            return time;
        }

        public string GetTimeTip(bool withColar = false,bool isBool=false)
        {
            string timeStr = string.Empty;
            System.DateTime now = Data.ZoneTimeDiff.GetServerTime();// EB.Time.LocalNow;
            var mStartCronJobs = EventTemplateManager.Instance.GetCronJobsByName("InfiniteCompete_start");
            string[] start= mStartCronJobs.interval.Split(' ');
            System.DateTime startdate = new System.DateTime();
            if (start.Length == 6)
            {
                TimerScheduler timerScheduler = new TimerScheduler();
                timerScheduler.m_TimerRegular = string.Format("{0} {1} {2} {3} {4}", start[1], start[2], start[3], start[4], start[5]);
                timerScheduler.ParseTimerRegular();
                if (!timerScheduler.isLegalTimer)
                {
                    EB.Debug.LogError("GetStartDateTime: cronFormat is illegal");
                }
                timerScheduler.GetNext(now, out startdate);
            }
            var mStopCronJobs = EventTemplateManager.Instance.GetCronJobsByName("InfiniteCompete_stop");
            string[] stop = mStopCronJobs.interval.Split(' ');
            System.DateTime stopdate=new System.DateTime();
            if (stop.Length == 6)
            {
                TimerScheduler timerScheduler = new TimerScheduler();
                timerScheduler.m_TimerRegular = string.Format("{0} {1} {2} {3} {4}", stop[1], stop[2], stop[3], stop[4], stop[5]);
                timerScheduler.ParseTimerRegular();
                if (!timerScheduler.isLegalTimer)
                {
                    EB.Debug.LogError("GetEndDateTime: cronFormat is illegal");
                }
                timerScheduler.GetNext(now, out stopdate);
            }
            var mRewardCronJobs = EventTemplateManager.Instance.GetCronJobsByName("InfiniteCompeteReward");
            string[] reward = mRewardCronJobs.interval.Split(' ');
            System.DateTime rewarddate = new System.DateTime();
            if (reward.Length == 6)
            {
                TimerScheduler timerScheduler = new TimerScheduler();
                timerScheduler.m_TimerRegular = string.Format("{0} {1} {2} {3} {4}", reward[1], reward[2], reward[3], reward[4], reward[5]);
                timerScheduler.ParseTimerRegular();
                if (!timerScheduler.isLegalTimer)
                {
                    EB.Debug.LogError("GetRewardDateTime: cronFormat is illegal");
                }
                timerScheduler.GetNext(now, out rewarddate);
            }

            string color = (withColar)?"42fe79": "ffffff";
            
            int resultIndex = 0;
            double nextStart = (startdate - now).TotalSeconds;
            double nextStop = (stopdate - now).TotalSeconds;
            double nextReward = (rewarddate - now).TotalSeconds;

            if (nextStop- nextStart > 0)
            {
                if(nextReward - nextStart > 0)
                {
                    resultIndex = 3;//等下一次开启
                }
                else
                {
                    resultIndex = 2;//结算期间
                }
            }
            else
            {
                resultIndex = 1;//开启期间
            }

            switch (resultIndex)
            {
                case 1: {
                        //活动开启期间
                        if (isBool) return "open";

                        var span = stopdate - now;//EB.Time.LocalNow;
                        string time = string.Format(EB.Localizer.GetString("ID_codefont_in_NationHudController_7771"), span.Days, span.Hours, span.Minutes);
                        timeStr = string.Format("{0}[{1}]{2}[-]", EB.Localizer.GetString("ID_uifont_in_LadderUI_LeftEndTime_6"), color, time);
                    } break;
                case 2: {
                        //活动结算期间
                        if (isBool) return "end";

                        var span = rewarddate - now;//EB.Time.LocalNow;
                        string time = string.Format(EB.Localizer.GetString("ID_codefont_in_NationHudController_7771"), span.Days, span.Hours, span.Minutes);
                        timeStr = string.Format("{0}[{1}]{2}[-]", EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TIME_TIP"), color, time);
                    } break;
                case 3: {
                        //等待下赛季
                        if (isBool||withColar) return "close";

                        var span = startdate - now;//EB.Time.LocalNow;
                        string time = string.Format(EB.Localizer.GetString("ID_codefont_in_NationHudController_7771"), span.Days, span.Hours, span.Minutes);
                        timeStr = string.Format("{0}[{1}]{2}[-]", EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TIME_TIP2"), color, time);
                    } break;
            }
            
            return timeStr;
        }

        public void RequestGetCompeteInfo(System.Action callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/speedinfinitechallenge/getInfo");
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback?.Invoke();
            });
        }

        public void RequestGetCompeteOtherInfo(int level, System.Action callback)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/speedinfinitechallenge/getFastInfo");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
                callback?.Invoke();
            });
        }

        public void RequestStarttCompete(int level)
        {
            EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/speedinfinitechallenge/startChallenge");
            request.AddData("level", level);
            LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
            {
                DataLookupsCache.Instance.CacheData(data);
            });
        }
        
        #endregion
    }
}
