using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GM.DataCache;
using Hotfix_LT.UI;
using System.Linq;

namespace Hotfix_LT.Data
{
    public class NormalActivityInstanceTemplate
    {
        public int id;
        public int activity_id;
        public int t;
        public string s;
        public string e;
    }

    public class SpecialActivityTemplate
    {
        public int id;
        public string display_name;
        public string open_time;
        public int type;
        public eBattleType battle_type;
        public int times;
        public int liveness;
        public string desc;
        public string start_end_time;
        public int funcId;
        public bool need_alliance;
        public string icon;
        public eActivityNavType nav_type;
        public string nav_parameter;
        public string notification_title;
        public string notification_content;
        public string open_tip;
        public bool isShowDaily;
        public int showType; //用于区分是限时活动还是全天活动
        public int order;
        public List<LTShowItemData> awards = new List<LTShowItemData>();
    }

    public class SpecialActivityLevelTemplate
    {
        public int id;
        public int activity_id;
        public string icon;
        public string map;
        public string layout;
        public string parameter;
        public int level;
        public int difficulty;
        public int gold;
        public string reward_desc;
        public int[] recommend_partners_id;
        public string name;
    }

    public class TimeLimitActivityTemplate
    {
        public int id;
        public string name;
        public string title;
        public string ui_model;
        public string parameter1;
        public string parameter2;
        public eActivityNavType nav_type;
        public string nav_parameter;
        public string icon;
        public string notice_content;
        public List<LTShowItemData> awards = new List<LTShowItemData>();
    }

    public class TimeLimitActivityStageTemplate
    {
        public int id;
        public int activity_id;
        public int stage;
        public int num;
        public int realm_num;
        public int sort;
        //public string rt1;
        //public int ri1;
        //public int rn1;
        //public string rt2;
        //public int ri2;
        //public int rn2;
        //public string rt3;
        //public int ri3;
        //public int rn3;
        public List<ItemStruct> reward_items;
    }

    public class BossChallengeActivityConfigTemplate
    {
        public int id;
        public int activity_id;
        public int level;
        public int difficulty;
        public int vigor;
        public int partner_attr;
        public int bonus_add;
        public string name;
        public string layout;
        public List<string> reward_box;
		public string bonus_name;
		public string bonus_describe;
	}

	public class LadderSeasonAwardTemplate
    {
        public int min_rank;
        public int max_rank;
        public int gold;
        public int diamond;
        public int ladder_gold;
        public LTShowItemData item;
    }

    public class ArenaAwardTemplate
    {
        public int rank_top;
        public int rank_down;
        public int hc;
        public int gold;
        public int arena_gold;
    }

    public class ClashOfHeroesRewardTemplate
    {
        public int id;
        public string item_type1;
        public string item_id1;
        public int item_num1;
        public string item_type2;
        public string item_id2;
        public int item_num2;
        public string item_type3;
        public string item_id3;
        public int item_num3;
        public string item_type4;
        public string item_id4;
        public int item_num4;
    }

    public class NationRatingRewardTemplate
    {
        public int rating;
        public List<LTShowItemData> rewardDatas;
    }

    public class NationRankRewardTemplate
    {
        public int rank;
        public List<LTShowItemData> rewardDatas;
    }

    public class NationSpecialEventTemplate
    {
        public int id;
        public string desc;
        public string result_desc1;
        public string result_desc2;
    }

    public class InfiniteChallengeTemplate
    {
        public int layer;
        public int level;
        public List<LTShowItemData> award;
        public List<LTShowItemData> first_award;
        public string model_name;
        public string name;
        public string combat_layout;
        public int model_level;
    }

    public class InfiniteBuffTemplate
    {
        public int level;
        public string name;
        public int conditionNum;
        public int effect;
    }

    public class BossRewardTemplate
    {
        public int id;
        public string ranks;
        public string reward;
        public float stageReward;
        public string box;
    }

	public class BossChallengeActivityTotalTimeTemplate
	{
		public int id;
		public int activity_id;
		public string activity_name;
		public string reward_item;
		public int battle_time;		
	}

    public class UREventRewardTemplate
    {
        public int id;
        public int type;
        public int top;
        public int down;
        public List<LTShowItemData> reward;
    }

    public class HonorArenaRewardTemplate
    {
        public int id;
        public string ranks;
        public string reward;
        public string oneHourReward;
    }
    
    public class HeroBattleTemplate
    {
        public int Id => data.Id;
        public int Type=> data.Type;
        public int Stage=> data.Stage;
        public string Name=> EB.Localizer.GetTableString(string.Format("ID_event_hero_battle_{0}_name", data.Id), data.Name);//data.Name;

        public List<string> OurHeroConfig;

        public List<string> EnemyHeroConfig;
        public string Award=> data.Award;
        public int CostLimit=> data.CostLimit;
        public int Condition=> data.Condition;
        public string Desc => EB.Localizer.GetTableString(string.Format("ID_event_hero_battle_{0}_desc", data.Id), data.Desc);//data.Desc;
        public HeroBattle data;
        public HeroBattleTemplate(HeroBattle data)
        {
            this.data = data;
            string[] str = data.OurHeroConfig.Split(',');
            OurHeroConfig = new List<string>();
            for (int i = 0; i < str.Length; i++)
            {
                OurHeroConfig.Add(str[i].Replace("\"",""));
            }
            
            str = data.EnemyHeroConfig.Split(',');
            EnemyHeroConfig = new List<string>();
            for (int j = 0; j < 4; j++)
            {
                EnemyHeroConfig.Add(str[j].Replace("\"",""));
            }
        }

    }

    public class CronJobs
    {
        public string name;
        public string interval;
        public string type;
        public bool isSchedule;
        public string scope;
        public bool isOpen;
        public int[] closeRealmId;
    }

    public class ActivityTime
    {
        // 如果服务器下发的是*，值为-1；*代表每的意思，hour = -1，就是指每小时。weekStr = *，就是指每天
        // Str都是服务器下发的原始数据，int是去每个Str的第一项，例：hourStr = 17,18; 那么hour = 17.
        // 数据都是待定的，如果以后有什么不方便的地方或者更好的方法，请联系 惟荣
        // 旁边的DB
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int second;
        public string weekStr;
        public string monthStr;
        public string dayStr;
        public string hourStr;
        public string minuteStr;
        public string secondStr;

        public ActivityTime()
        { }

        public ActivityTime(int hour = 0, int minute = 0, int second = 0)
        {
            this.hour = hour;
            this.minute = minute;
            this.second = second;
        }
    }

    public class PartnerCultivateGift
    {
        public int id;
        public string title;
        public string icon;
        public float discount;
    }

    public class LTActivityTime//仅用于夺宝奇兵解析cron表达式，获取时间点
    {
        //public int month;
        //public int day;
        public int[] week;
        public int[] hour;
        public int minute;
        public int second;

        public LTActivityTime()
        {
            week = null; hour = new int[1] { 0 }; minute = 0; second = 0;
        }
        public LTActivityTime(string str)
        {
            string[] strs = str.Split(' ');
            if (strs.Length != 6)
                EB.Debug.LogError("cronTable Str Length!=6");

            int.TryParse(strs[0], out this.second);
            this.minute = GetTiemSplit(strs[1]);
            this.hour = GetTiemSplits(strs[2]);
            this.week = GetTiemSplits(strs[5]);
        }

        private int GetTiemSplit(string str)
        {
            if (str.Contains(","))
            {
                string[] weekStr = str.Split(',');
                int[] data = new int[weekStr.Length];
                for (int i = 0; i < weekStr.Length; i++)
                {
                    int.TryParse(weekStr[i], out data[i]);
                }
                return data[0];
            }
            if (str.Contains("-"))
            {
                string[] weekStr = str.Split('-');
                int[] data = new int[weekStr.Length];
                for (int i = 0; i < weekStr.Length; i++)
                {
                    int.TryParse(weekStr[i], out data[i]);
                }
                return data[0];
            }
            int temp = 0;
            int.TryParse(str, out temp);
            return temp;
        }
        private int[] GetTiemSplits(string str)
        {
            if (str.Contains("*")) return new int[7] { 0, 1, 2, 3, 4, 5, 6 };//应该只有周才会出现*号,现在不止周，夺宝奇兵也会
            if (str.Contains("-"))
            {
                string[] strs = str.Split('-');
                int[] temp = new int[strs.Length];
                for (int i = 0; i < strs.Length; i++)
                {
                    int.TryParse(strs[i], out temp[i]);
                }
                return new int[1] { temp[0] };
            }
            string[] weekStr = str.Split(',');
            int[] data = new int[weekStr.Length];
            for (int i = 0; i < weekStr.Length; i++)
            {
                int.TryParse(weekStr[i], out data[i]);
            }
            return data;
        }
    }

    public class DailyRewardTemplate
    {
        public int id;
        public string type;
        public string title;
        public string desc;
        public List<LTShowItemData> ItemList;
    }

    public class HeroMedalTemplate
    {
        public int stage;
        public List<LTShowItemData> ItemList;
        public List<LTShowItemData> ExItemList;
    }

    public class MainCampaignRewardTemplate
    {
        public int stage;
        public List<LTShowItemData> ItemList;
        public List<LTShowItemData> ExItemList;
    }

    public class AwakenDungeonTemplate
    {
        public int ID;
        public eRoleAttr Type;
        public int Stage;
        public string Name;
        public string CombatLayoutName;
        public List<LTShowItemData> DropAward;
        public List<LTShowItemData> DropAwardFirst;
    }

    /// <summary>
    /// 爬塔数据
    /// </summary>
    public class ClimingTowerTemplate
    {
        /// <summary>
        /// 等级 
        /// </summary>
        public int level;
        /// <summary>
        /// 层
        /// </summary>
        public int layer;
        /// <summary>
        /// 递增参数
        /// </summary>
        public float param;
     
        public int normal_record;
      
        public int diff_record;

        public int sleep_Num;
       
        /// <summary>
        /// 是否可以挑战，只有当前层可以挑战
        /// </summary>
        public bool v_CanChallenge;
    }

    public class ClimingTowerRewardTemplate
    {
        public int id;
        public int Record;
        public string Reward;
    }

    public class DiamondGiftTemplate
    {
        public int id;
        public int days;
        public int originalPrice;
        public int activityPrice;
        public LTShowItemData lTShowItemData;
    }

    public class LevelCompensatedRewardTemplate
    {
        public int id;
        public int days;
        public int minLevel;
        public int maxLevel;
        public List<LTShowItemData> rewardList;
    }

    public class InfiniteCompeteTemplate
    {
        public int id;
        public List<LTShowItemData> first_award;
    }

    public class InfiniteCompeteRewardTemplate
    {
        public int id;
        public string ranks;
        public List<LTShowItemData> reward;
    }

    public class InviteTaskTemplate
    {
        public int taskId;
        public int pretask;
        public int showpage;
        public int tasktype;
        public int param1;
        public int param2;
        public int groupid;
        public LTShowItemData reward;           

    }

    public class EventTemplateManager
    {
        private static EventTemplateManager sInstance = null;

        private List<NormalActivityInstanceTemplate> mNormalActivityInstanceTbl = null;
        private Dictionary<int, SpecialActivityTemplate> mSpectialActivityTbl = null;
        private Dictionary<int, SpecialActivityLevelTemplate> mSpectialActivityLevelTbl = null;
        private Dictionary<int, List<SpecialActivityLevelTemplate>> mSpecialActivityLevels = new Dictionary<int, List<SpecialActivityLevelTemplate>>();

        private Dictionary<int, TimeLimitActivityTemplate> mTimeLimitActivityTbl = null;
        private Dictionary<int, TimeLimitActivityStageTemplate> mTimeLimitActivityStageTbl = null;
        private Dictionary<int, List<TimeLimitActivityStageTemplate>> mTimeLimitActivityStages = new Dictionary<int, List<TimeLimitActivityStageTemplate>>();

		private Dictionary<int, BossChallengeActivityConfigTemplate> mBossChallengeActivityConfigTbl = null;
        private Dictionary<int, List<BossChallengeActivityConfigTemplate>> mBossChallengeActivityConfigs = new Dictionary<int, List<BossChallengeActivityConfigTemplate>>();
		private Dictionary<int, BossChallengeActivityTotalTimeTemplate> mBossChallengeActivityTotalTimeTbl = null;
		private Dictionary<int, List<BossChallengeActivityTotalTimeTemplate>> mBossChallengeActivityTotalTimes = new Dictionary<int, List<BossChallengeActivityTotalTimeTemplate>>();

		private List<LadderSeasonAwardTemplate> mLadderSeasonAwardTpl = null;
        private List<ArenaAwardTemplate> mArenaAwardTpl = null;

        private List<ClashOfHeroesRewardTemplate> mClashOfHeroesRewardTemplate = null;
        private List<NationRatingRewardTemplate> mNationRatingRewardTpl = null;
        private List<NationRankRewardTemplate> mNationRankRewardTpl = null;
        private List<NationSpecialEventTemplate> mNationSpecialEventTpl = null;

        private Dictionary<int, InfiniteChallengeTemplate> mInfiniteChallengeTplDic = null;
        private List<InfiniteBuffTemplate> mInfiniteBuffTpl = null;
        private List<BossRewardTemplate> mBossRewardTpl = null;
        private List<UREventRewardTemplate> mUREventRewardTpl = null;
        private List<HonorArenaRewardTemplate> mHonorArenaRewardTpl = null;
        private List<HeroBattleTemplate> mHeroBattleTpl=null;
        private Dictionary<float, string> mBossStageRewardDic = null;//boss的节点奖励，
        private Dictionary<string, CronJobs> mCronJobsTbl = null;
        private Dictionary<int, ActivityTime> mActivityOpenTime = null;//所有的活动开启时间
        private Dictionary<int, ActivityTime> mActivityStopTime = null;//所有的活动关闭时间
        private Dictionary<int, PartnerCultivateGift> mPartnerCultivateGiftDic = null;//伙伴培养礼包
        private Dictionary<int, LevelCompensatedRewardTemplate> mLevelCompensatedRewardDic = null;//等级低保奖励

        private List<DailyRewardTemplate> mDailyRewardList = null;
        private List<HeroMedalTemplate> mHeroMedalList = null;
        private List<MainCampaignRewardTemplate> mMainCampaignRewardList = null;
        private List<AwakenDungeonTemplate> mAwakenDungeonList = null;
        private List<ClimingTowerTemplate> mClimingTowerList = new List<ClimingTowerTemplate>();
        private List<InfiniteCompeteTemplate> mInfiniteCompeteTpl = null;
        private List<InfiniteCompeteRewardTemplate> mInfiniteCompeteRewardTpl = null;
        private List<InviteTaskTemplate> mInviteTaskList = null;
        private List<ClimingTowerRewardTemplate> mClimingTowerRewardList = new List<ClimingTowerRewardTemplate>();


        private GM.DataCache.ConditionEvent conditionSet;
        public Dictionary<int, List<DiamondGiftTemplate>> DiamondGiftDict
        {
            get;
            private set;
        }

        public static string[] NumText = { "ID_WEEK_0", "ID_WEEK_1", "ID_WEEK_2", "ID_WEEK_3", "ID_WEEK_4", "ID_WEEK_5", "ID_WEEK_6" };

        public static EventTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new EventTemplateManager(); }
        }

        public bool InitFromDataCache(GM.DataCache.Event evt)
        {
            if (evt == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            conditionSet = evt.GetArray(0);

            if (!InitSpecialActivityTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init SpecialActivity table failed");
                return false;
            }

            if (!InitSpecialActivityLevelTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init SpecialActivityLevel table failed");
                return false;
            }

            if (!InitNormalActivityInstanceTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init NormalActivityInstance table failed");
                return false;
            }

            mSpecialActivityLevels.Clear();

            var enumerator = mSpectialActivityLevelTbl.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var entry = enumerator.Current;
                if (!mSpecialActivityLevels.ContainsKey(entry.Value.activity_id))
                {
                    mSpecialActivityLevels.Add(entry.Value.activity_id, new List<SpecialActivityLevelTemplate>() { entry.Value });
                }
                else
                {
                    mSpecialActivityLevels[entry.Value.activity_id].Add(entry.Value);
                }
            }

            if (!InitTimeLimitActivityTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init TimeLimitActivity table failed");
                return false;
            }

            if (!InitTimeLimitActivityStageTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init TimeLimitActivityStage table failed");
                return false;
            }

            mTimeLimitActivityStages.Clear();
            var enumerator1 = mTimeLimitActivityStageTbl.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                var entry = enumerator1.Current;
                if (!mTimeLimitActivityStages.ContainsKey(entry.Value.activity_id))
                {
                    mTimeLimitActivityStages.Add(entry.Value.activity_id, new List<TimeLimitActivityStageTemplate>() { entry.Value });
                }
                else
                {
                    mTimeLimitActivityStages[entry.Value.activity_id].Add(entry.Value);
                }
            }

            if (!InitBossChallengeActivityConfigTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init BossChallengeActivity table failed");
                return false;
            }

			if(!InitBossChallengeActivityTotalTimeTbl(conditionSet))
			{
				EB.Debug.LogError("InitFromDataCache: init BossChallengeTotalTime table failed");
				return false;
			}

            mBossChallengeActivityConfigs.Clear();
            var enumerator2 = mBossChallengeActivityConfigTbl.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                var entry = enumerator2.Current;
                if (!mBossChallengeActivityConfigs.ContainsKey(entry.Value.activity_id))
                {
                    mBossChallengeActivityConfigs.Add(entry.Value.activity_id, new List<BossChallengeActivityConfigTemplate>() { entry.Value });
                }
                else
                {
                    mBossChallengeActivityConfigs[entry.Value.activity_id].Add(entry.Value);
                }
            }

			mBossChallengeActivityTotalTimes.Clear();
			var enumerator3 = mBossChallengeActivityTotalTimeTbl.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				var entry = enumerator3.Current;
				if (!mBossChallengeActivityTotalTimes.ContainsKey(entry.Value.activity_id))
				{
					mBossChallengeActivityTotalTimes.Add(entry.Value.activity_id, new List<BossChallengeActivityTotalTimeTemplate>() { entry.Value });
				}
				else
				{
					mBossChallengeActivityTotalTimes[entry.Value.activity_id].Add(entry.Value);
				}
			}

			if (!InitLadderSeasonAwardTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init LadderSeasonAward table failed");
                return false;
            }
            
            if (!InitSleepReward(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init LadderSeasonAward table failed");
                return false;
            }

            if (!InitArenaAwardTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init ArenaAward table failed");
                return false;
            }

            if (!InitClashOfHeroesRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init ClashOfHeroesReward table failed");
                return false;
            }

            if (!InitNationRatingRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init NationRatingReward table failed");
                return false;
            }

            if (!InitNationRankRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init NationRankReward table failed");
                return false;
            }

            if (!InitNationSpecialEventTbl(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init NationSpecialEvent table failed");
                return false;
            }

            if (!InitInfiniteChallengeTbl(conditionSet))
            {
                EB.Debug.LogError("InitInfiniteChallengeTbl: init InfiniteChallengeTbl table failed");
                return false;
            }

            if (!InitInfiniteBuffTbl(conditionSet))
            {
                EB.Debug.LogError("InitInfiniteBuffTbl: init InfiniteBuffTbl table failed");
                return false;
            }

            if (!InitBossRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitBossRewardTbl: init InitBossRewardTbl table failed");
                return false;
            }            
            if (!InitUREventRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitUREventRewardTbl: init InitUREventRewardTbl table failed");
                return false;
            }

            if (!InitHonorArenaRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitHonorArenaRewardTbl: init InitHonorArenaRewardTbl table failed");
                return false;
            }

            if (!InitHeroBattle(conditionSet))
            {
                EB.Debug.LogError("InitHeroBattleTbl: init HeroBattle table failed");
                return false;
            }

            if (!InitCronJobsTbl(conditionSet))
            {
                EB.Debug.LogError("InitCronJobsTbl: init InitCronJobsTbl table failed");
                return false;
            }

            if (!InitPartnerCultivateGiftTbl(conditionSet))
            {
                EB.Debug.LogError("InitPartnerCultivateGiftTbl: init InitPartnerCultivateGiftTbl table failed");
                return false;
            }

            if (!InitDailyRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitDailyRewardTbl: init InitDailyRewardTbl table failed");
                return false;
            }

            if (!InitHeroMedalTbl(conditionSet))
            {
                EB.Debug.LogError("InitHeroMedalTbl: init InitHeroMedalTbl table failed");
                return false;
            }

            if (!InitMainCampaignRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitMainCampaignRewardTbl: init InitMainCampaignRewardTbl table failed");
                return false;
            }

            if (!InitAwakenDungeonTbl(conditionSet))
            {
                EB.Debug.LogError("InitAwakenDungeonTbl: init InitAwakenDungeonTbl table failed");
                return false;
            }

            //if (!InitClimingTowerTbl(conditionSet))
            //{
            //    EB.Debug.LogError("InitAwakenDungeonTbl: init InitAwakenDungeonTbl table failed");
            //    return false;
            //}
            if (!InitLevelCompensatedRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitLevelCompensatedRewardTbl: init InitLevelCompensatedRewardTbl table failed");
                return false;
            }

            if (!InitInfiniteCompeteTbl(conditionSet))
            {
                EB.Debug.LogError("InitInfiniteCompeteTbl: init InitInfiniteCompeteTbl table failed");
                return false;
            }
            if (!InitInfiniteCompeteRewardTbl(conditionSet))
            {
                EB.Debug.LogError("InitInfiniteCompeteRewardTbl: init InitInfiniteCompeteRewardTbl table failed");
                return false;
            }

            InitDiamondGiftTemplate(conditionSet);

            //初始化赛跑活动配置
            LTActivityRacingManager.Instance.InitConfigs(conditionSet);

            return true;
        }

        private bool InitSleepReward(ConditionEvent data)
        {
            mClimingTowerRewardList = new List<ClimingTowerRewardTemplate>();
            for (int i = 0; i < data.SleepTowerRewardLength; i++)
            {
                var tpl = ParseSleepReward(data.GetSleepTowerReward(i));
                mClimingTowerRewardList.Add(tpl);
            }
            return true;
        }

        private ClimingTowerRewardTemplate ParseSleepReward(SleepTowerReward data)
        {
            ClimingTowerRewardTemplate temp = new ClimingTowerRewardTemplate();
            if (data!=null)
            {
                temp.id = data.Id;
                temp.Record = data.Record;
                temp.Reward = data.Reward;
            }
            return temp;
        }

        private bool InitInfiniteCompeteRewardTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mInfiniteCompeteRewardTpl = new List<InfiniteCompeteRewardTemplate>(conditonSet.InfiniteCompeteRewardLength);
            for (int i = 0; i < conditonSet.InfiniteCompeteRewardLength; ++i)
            {
                var tpl = ParseInfiniteCompeteRewardTpl(conditonSet.GetInfiniteCompeteReward(i));
                mInfiniteCompeteRewardTpl.Add(tpl);
            }
            return true;
        }
        private InfiniteCompeteRewardTemplate ParseInfiniteCompeteRewardTpl(InfiniteCompeteReward obj)
        {
            InfiniteCompeteRewardTemplate tpl = new InfiniteCompeteRewardTemplate();
            tpl.id = obj.Id;
            tpl.ranks = obj.Ranks;
            tpl.reward = ParseShowItem(obj.Reward);
            return tpl;
        }

        private bool InitInfiniteCompeteTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mInfiniteCompeteTpl = new List<InfiniteCompeteTemplate>(conditonSet.InfiniteCompeteLength);
            for (int i = 0; i < conditonSet.InfiniteCompeteLength; ++i)
            {
                var tpl = ParseInfiniteCompeteTpl(conditonSet.GetInfiniteCompete(i));
                mInfiniteCompeteTpl.Add(tpl);
            }
            return true;
        }
        private InfiniteCompeteTemplate ParseInfiniteCompeteTpl(InfiniteCompete obj)
        {
            InfiniteCompeteTemplate tpl = new InfiniteCompeteTemplate();
            tpl.id = obj.Id;
            tpl.first_award = ParseShowItem(obj.FirstAward);
            return tpl;
        }

        private bool InitClimingTowerTbl()
        {
            int len = conditionSet.SleepTowerLength;
            for (int i = 0; i < len; i++)
            {
                var tpl = ParseClimingTowerTpl(conditionSet.GetSleepTower(i));
                mClimingTowerList.Add(tpl);
            }
            return true;
        }
        private void InitTowerList()
        {
            if (mClimingTowerList.Count < 1)
            {
                InitClimingTowerTbl();
            }
        }

        private ClimingTowerTemplate ParseClimingTowerTpl(SleepTower obj)
        {
            ClimingTowerTemplate tpl = new ClimingTowerTemplate();
            tpl.level = obj.Level;
            tpl.layer = obj.Floor;
            tpl.param = obj.Param;
            tpl.diff_record = obj.DifficultRecord;
            tpl.normal_record = obj.NormalRecord;
            tpl.sleep_Num = obj.DifficultSleep;
            return tpl;
        }

        private bool InitNormalActivityInstanceTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mNormalActivityInstanceTbl = new List<NormalActivityInstanceTemplate>(conditonSet.NormalActivityInstanceLength);
            for (int i = 0; i < conditonSet.NormalActivityInstanceLength; ++i)
            {
                var tpl = ParseNormalActivityInstance(conditonSet.GetNormalActivityInstance(i));
                mNormalActivityInstanceTbl.Add(tpl);
            }

            return true;
        }

        public List<NormalActivityInstanceTemplate> GetNormalActivityInstanceTemplates(int activityId)
        {
            List<NormalActivityInstanceTemplate> list = new List<NormalActivityInstanceTemplate>();

            int len = mNormalActivityInstanceTbl.Count;
            for (int i = 0; i < len; i++)
            {
                var item = mNormalActivityInstanceTbl[i];
                if (item.activity_id == activityId)
                    list.Add(item);
            }
            return list;
        }

        private NormalActivityInstanceTemplate ParseNormalActivityInstance(GM.DataCache.NormalActivityInstance obj)
        {
            NormalActivityInstanceTemplate tpl = new NormalActivityInstanceTemplate();

            tpl.id = obj.Id;
            tpl.activity_id = obj.ActivityId;
            tpl.t = obj.T;
            tpl.s = obj.S;
            tpl.e = obj.E;
            return tpl;
        }

        private bool InitSpecialActivityTbl(GM.DataCache.ConditionEvent conditionSet)
        {
            mSpectialActivityTbl = new Dictionary<int, SpecialActivityTemplate>(conditionSet.SpecialActivityLength);
            for (int i = 0; i < conditionSet.SpecialActivityLength; ++i)
            {
                var tpl = ParseSpecialActivity(conditionSet.GetSpecialActivity(i));
                if (mSpectialActivityTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitSpecialActivityTbl: {0} exists", tpl.id);
                    mSpectialActivityTbl.Remove(tpl.id);
                }
                mSpectialActivityTbl.Add(tpl.id, tpl);
            }
            return true;
        }

        private SpecialActivityTemplate ParseSpecialActivity(GM.DataCache.SpecialActivity obj)
        {
            SpecialActivityTemplate tpl = new SpecialActivityTemplate();

            tpl.id = obj.Id;
            tpl.display_name = EB.Localizer.GetTableString(string.Format("ID_event_special_activity_{0}_display_name", tpl.id), obj.DisplayName);//;
            tpl.open_time = obj.OpenTime;
            tpl.type = obj.Type;
            tpl.battle_type = (eBattleType)obj.BattleType;
            tpl.times = obj.Times;
            tpl.liveness = obj.Liveness;
            tpl.desc = EB.Localizer.GetTableString(string.Format("ID_event_special_activity_{0}_desc", tpl.id), obj.Desc);//;
            tpl.start_end_time = obj.StartEndTime;
            tpl.funcId = obj.Level;
            tpl.need_alliance = obj.NeedAlliance;
            tpl.icon = obj.Icon;
            tpl.nav_type = (eActivityNavType)obj.NavType;
            tpl.nav_parameter = obj.NavParameter;
            tpl.notification_title = EB.Localizer.GetTableString(string.Format("ID_event_special_activity_{0}_title", tpl.id), obj.Title);//;
            tpl.notification_content = EB.Localizer.GetTableString(string.Format("ID_event_special_activity_{0}_content", tpl.id), obj.Content);//;
            tpl.open_tip = EB.Localizer.GetTableString(string.Format("ID_event_special_activity_{0}_open_tip", tpl.id), obj.OpenTip);//;
            tpl.isShowDaily = obj.IsShowDaily;
            tpl.showType = obj.ShowType;
            tpl.order = obj.Order;
            tpl.awards = ParseShowItem(obj.Award);
            return tpl;
        }

        private bool InitSpecialActivityLevelTbl(GM.DataCache.ConditionEvent conditionSet)
        {
            mSpectialActivityLevelTbl = new Dictionary<int, SpecialActivityLevelTemplate>(conditionSet.SpecialActivityLevelLength);
            for (int i = 0; i < conditionSet.SpecialActivityLevelLength; ++i)
            {
                var tpl = ParseSpecialActivityLevel(conditionSet.GetSpecialActivityLevel(i));
                if (mSpectialActivityLevelTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitSpecialActivityTbl: {0} exists", tpl.id);
                    mSpectialActivityLevelTbl.Remove(tpl.id);
                }
                mSpectialActivityLevelTbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private SpecialActivityLevelTemplate ParseSpecialActivityLevel(GM.DataCache.SpecialActivityLevel obj)
        {
            SpecialActivityLevelTemplate tpl = new SpecialActivityLevelTemplate();

            tpl.id = obj.Id;
            tpl.activity_id = obj.ActivityId;
            tpl.icon = obj.Icon;
            tpl.map = obj.Map;
            tpl.layout = obj.Layout;
            tpl.parameter = obj.Parameter;
            tpl.level = obj.Level;
            tpl.difficulty = obj.Difficulty;
            tpl.gold = obj.Gold;
            tpl.reward_desc = obj.RewardDesc;
            tpl.recommend_partners_id = SetRecommendPartner(obj.RecommendPartner);
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_event_special_activity_level_{0}_name", tpl.id), obj.Name);//;
            return tpl;
        }

        private int[] SetRecommendPartner(string recommendPartner)
        {
            string[] sArray = recommendPartner.Split(',');
            int[] iArray = LTUIUtil.ToInt(sArray);
            return iArray;
        }

        private bool InitTimeLimitActivityTbl(GM.DataCache.ConditionEvent conditionSet)
        {
            mTimeLimitActivityTbl = new Dictionary<int, TimeLimitActivityTemplate>(conditionSet.TimeLimitActivityLength);
            for (int i = 0; i < conditionSet.TimeLimitActivityLength; ++i)
            {
                var tpl = ParseTimeLimitActivity(conditionSet.GetTimeLimitActivity(i));
                if (mTimeLimitActivityTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitTimeLimitActivityTbl: {0} exists", tpl.id);
                    mTimeLimitActivityTbl.Remove(tpl.id);
                }
                mTimeLimitActivityTbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private TimeLimitActivityTemplate ParseTimeLimitActivity(GM.DataCache.TimeLimitActivity obj)
        {
            TimeLimitActivityTemplate tpl = new TimeLimitActivityTemplate();
            tpl.id = obj.Id;
            tpl.name = obj.Name;
            tpl.title = obj.Title;
            tpl.ui_model = obj.UiModel;
            tpl.parameter1 = obj.Parameter1;
            tpl.parameter2 = obj.Parameter2;
            tpl.nav_type = (eActivityNavType)obj.NavType;
            tpl.nav_parameter = obj.NavParameter;
            tpl.icon = obj.Icon;
            tpl.notice_content = obj.NoticeContent;
            string[] awardStrs = new string[4] { obj.Award1, obj.Award2, obj.Award3, obj.Award4 };
            for (int i = 0; i < 4; ++i)
            {
                if (!string.IsNullOrEmpty(awardStrs[i]))
                {
                    string[] splits = awardStrs[i].Split(',');
                    string type = splits[0];
                    string id = splits[1];
                    int count = 1;
                    if (!int.TryParse(splits[2], out count))
                        EB.Debug.LogError("time_limit_activity award id error");
                    tpl.awards.Add(new LTShowItemData(id, count, type,false));
                }
                else
                    break;
            }
            return tpl;

        }

        private bool InitTimeLimitActivityStageTbl(GM.DataCache.ConditionEvent conditionSet)
        {
            mTimeLimitActivityStageTbl = new Dictionary<int, TimeLimitActivityStageTemplate>(conditionSet.TimeLimitActivityStageLength);
            for (int i = 0; i < conditionSet.TimeLimitActivityStageLength; ++i)
            {
                var tpl = ParseTimeLimitActivityStage(conditionSet.GetTimeLimitActivityStage(i));
                if (mTimeLimitActivityStageTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitTimeLimitActivityStageTbl: {0} exists", tpl.id);
                    mTimeLimitActivityStageTbl.Remove(tpl.id);
                }
                mTimeLimitActivityStageTbl.Add(tpl.id, tpl);
            }

            return true;
        }

        private TimeLimitActivityStageTemplate ParseTimeLimitActivityStage(GM.DataCache.TimeLimitActivityStage obj)
        {
            TimeLimitActivityStageTemplate tpl = new TimeLimitActivityStageTemplate();
            tpl.id = obj.Id;
            tpl.activity_id = obj.ActivityId;
            tpl.stage = obj.Stage;
            tpl.num = obj.Num;
            tpl.realm_num = obj.RealmNum;
            tpl.sort = obj.Sort;

            tpl.reward_items = new List<ItemStruct>();
            do
            {
                string id = obj.Ri1;
                int amount = obj.Rn1;
                string type = obj.Rt1;
                if (string.IsNullOrEmpty(id) || amount <= 0 || string.IsNullOrEmpty(type)) break;
                tpl.reward_items.Add(new ItemStruct(id, type, amount));

                id = obj.Ri2;
                amount = obj.Rn2;
                type = obj.Rt2;
                if (string.IsNullOrEmpty(id) || amount <= 0 || string.IsNullOrEmpty(type)) break;
                tpl.reward_items.Add(new ItemStruct(id, type, amount));

                id = obj.Ri3;
                amount = obj.Rn3;
                type = obj.Rt3;
                if (string.IsNullOrEmpty(id) || amount <= 0 || string.IsNullOrEmpty(type)) break;
                tpl.reward_items.Add(new ItemStruct(id, type, amount));

                id = obj.Ri4;
                amount = obj.Rn4;
                type = obj.Rt4;
                if (string.IsNullOrEmpty(id) || amount <= 0 || string.IsNullOrEmpty(type)) break;
                tpl.reward_items.Add(new ItemStruct(id, type, amount));
            } while (false);

            return tpl;
        }

        private bool InitBossChallengeActivityConfigTbl(GM.DataCache.ConditionEvent conditionSet)
        {
            mBossChallengeActivityConfigTbl = new Dictionary<int, BossChallengeActivityConfigTemplate>(conditionSet.BossChallengeActivityConfigLength);
           
            for (int i = 0; i < conditionSet.BossChallengeActivityConfigLength; ++i)
            {
                var tpl = ParseBossChallengeActivityConfig(conditionSet.GetBossChallengeActivityConfig(i));
                
                if (mBossChallengeActivityConfigTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitBossChallengeActivityTbl: {0} exists", tpl.id);
                    mBossChallengeActivityConfigTbl.Remove(tpl.id);
                }

                mBossChallengeActivityConfigTbl.Add(tpl.id, tpl);
            }

            return true;
        }

		private bool InitBossChallengeActivityTotalTimeTbl(GM.DataCache.ConditionEvent conditionSet)
		{
			mBossChallengeActivityTotalTimeTbl = new Dictionary<int, BossChallengeActivityTotalTimeTemplate>(conditionSet.BossChallengeActivityTotalTimeLength);

			for (int i = 0; i < conditionSet.BossChallengeActivityTotalTimeLength; ++i)
			{
				var tpl = ParseBossChallengeActivityTotalTimeConfig(conditionSet.GetBossChallengeActivityTotalTime(i));

				if (mBossChallengeActivityTotalTimeTbl.ContainsKey(tpl.id))
				{
					EB.Debug.LogError("InitBossChallengeTotalTime: {0} exists", tpl.id);
					mBossChallengeActivityTotalTimeTbl.Remove(tpl.id);
				}

				mBossChallengeActivityTotalTimeTbl.Add(tpl.id, tpl);
			}
			return true;
		}

        private BossChallengeActivityConfigTemplate ParseBossChallengeActivityConfig(GM.DataCache.BossChallengeActivityConfig obj)
        {
            BossChallengeActivityConfigTemplate tpl = new BossChallengeActivityConfigTemplate();
            tpl.id = obj.Id;
            tpl.activity_id = obj.ActivityId;
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_event_boss_challenge_activity_config_{0}_name", obj.Id), obj.Name);
            tpl.layout = obj.Layout;
            tpl.level = obj.Level;
            tpl.difficulty = obj.Difficulty;
            tpl.vigor = obj.Vigor;
            tpl.partner_attr = obj.PartnerAttr;
            tpl.bonus_add = obj.BonusAdd;

			var list = new List<string>();
            for (int i = 0; i < obj.RewardBoxLength; i++)
            {
                list.Add(obj.GetRewardBox(i));
            }
            tpl.reward_box = list;

            return tpl;
        }

		private BossChallengeActivityTotalTimeTemplate ParseBossChallengeActivityTotalTimeConfig(GM.DataCache.BossChallengeActivityTotalTime obj)
		{
			BossChallengeActivityTotalTimeTemplate tpl = new BossChallengeActivityTotalTimeTemplate();
			tpl.id = obj.Id;
			tpl.activity_id = obj.ActivityId;
			tpl.activity_name = obj.Name;
			tpl.battle_time = obj.BattleTime;
			tpl.reward_item = obj.RewardItem;

			return tpl;
		}

		private bool InitLadderSeasonAwardTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mLadderSeasonAwardTpl = new List<LadderSeasonAwardTemplate>(conditonSet.LadderSeasonAwardLength);
            for (int i = 0; i < conditonSet.LadderSeasonAwardLength; ++i)
            {
                var tpl = ParseLadderSeasonAward(conditonSet.GetLadderSeasonAward(i));
                mLadderSeasonAwardTpl.Add(tpl);
            }

            return true;
        }

        private LadderSeasonAwardTemplate ParseLadderSeasonAward(GM.DataCache.LadderSeasonAward obj)
        {
            LadderSeasonAwardTemplate tpl = new LadderSeasonAwardTemplate();

            tpl.min_rank = obj.MinRank;
            tpl.max_rank = obj.MaxRank;
            tpl.gold = obj.Gold;
            tpl.diamond = obj.Diamond;
            tpl.ladder_gold = obj.LadderGold;
            string itemStr = obj.Item;
            if (!string.IsNullOrEmpty(itemStr))
            {
                string[] tempReStr = itemStr.Split(',');
                if (tempReStr.Length < 3)
                {
                    EB.Debug.LogError("LadderSeasonAwardTemplate Init Error,MinRank:{0}" , tpl.min_rank);
                }

                LTShowItemData tempData = new LTShowItemData(tempReStr[0], int.Parse(tempReStr[1]), tempReStr[2]);
                tpl.item = tempData;
            }
            return tpl;
        }

        public List<LadderSeasonAwardTemplate> GetLadderSeasonAwardTemplates()
        {
            return mLadderSeasonAwardTpl;
        }

        public LadderSeasonAwardTemplate GetLadderSeasonAwardTemplate(int rank)
        {
            if (rank == 0 && mLadderSeasonAwardTpl[mLadderSeasonAwardTpl.Count - 1].max_rank == -1)
                return mLadderSeasonAwardTpl[mLadderSeasonAwardTpl.Count - 1];

            int len = mLadderSeasonAwardTpl.Count;
            for (int i = 0; i < len; i++)
            {
                var tpl = mLadderSeasonAwardTpl[i];
                if (tpl.min_rank <= rank)
                {
                    if (tpl.max_rank == -1)
                        return tpl;
                    else if (rank <= tpl.max_rank)
                        return tpl;
                }
            }
            return null;
        }

        private bool InitArenaAwardTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mArenaAwardTpl = new List<ArenaAwardTemplate>(conditonSet.ArenaAwardLength);
            for (int i = 0; i < conditonSet.ArenaAwardLength; ++i)
            {
                var tpl = ParseArenaAward(conditonSet.GetArenaAward(i));
                mArenaAwardTpl.Add(tpl);
            }

            return true;
        }

        private ArenaAwardTemplate ParseArenaAward(GM.DataCache.ArenaAward obj)
        {
            ArenaAwardTemplate tpl = new ArenaAwardTemplate();

            tpl.rank_top = obj.RankTop;
            tpl.rank_down = obj.RankDown;
            tpl.hc = obj.Hc;
            tpl.gold = obj.Gold;
            tpl.arena_gold = obj.ArenaGold;
            return tpl;
        }

        public List<ArenaAwardTemplate> GetArenaAwardTemplates()
        {
            return mArenaAwardTpl;
        }

        public ArenaAwardTemplate GetArenaAwardTemplate(int rank)
        {
            if (rank == 0 && mArenaAwardTpl[mArenaAwardTpl.Count - 1].rank_down == -1)
                return mArenaAwardTpl[mArenaAwardTpl.Count - 1];

            int len = mArenaAwardTpl.Count;
            for (int i = 0; i < len; i++)
            {
                var tpl = mArenaAwardTpl[i];
                if (tpl.rank_top <= rank)
                {
                    if (tpl.rank_down == -1)
                        return tpl;
                    else if (rank <= tpl.rank_down)
                        return tpl;
                }
            }
            return null;
        }

        private bool InitClashOfHeroesRewardTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mClashOfHeroesRewardTemplate = new List<ClashOfHeroesRewardTemplate>();
            for (int i = 0; i < conditonSet.ClashOfHeroesRewardLength; i++)
            {
                var tpl = ParseClashOfHeroesReward(conditonSet.GetClashOfHeroesReward(i));
                mClashOfHeroesRewardTemplate.Add(tpl);
            }

            mClashOfHeroesRewardTemplate.Sort((x, y) => x.id > y.id ? -1 : 0);
            return true;
        }
        private ClashOfHeroesRewardTemplate ParseClashOfHeroesReward(GM.DataCache.ClashOfHeroesReward obj)
        {
            ClashOfHeroesRewardTemplate tpl = new ClashOfHeroesRewardTemplate();

            tpl.id = obj.Id;
            tpl.item_id1 = obj.ItemId1;
            tpl.item_num1 = obj.ItemNum1;
            tpl.item_type1 = obj.ItemType1;
            tpl.item_id2 = obj.ItemId2;
            tpl.item_num2 = obj.ItemNum2;
            tpl.item_type2 = obj.ItemType2;
            tpl.item_id3 = obj.ItemId3;
            tpl.item_num3 = obj.ItemNum3;
            tpl.item_type3 = obj.ItemType3;
            tpl.item_id4 = obj.ItemId4;
            tpl.item_num4 = obj.ItemNum4;
            tpl.item_type4 = obj.ItemType4;
            return tpl;
        }

        public List<ClashOfHeroesRewardTemplate> GetClashOfHeroesRewardTpls()
        {
            return mClashOfHeroesRewardTemplate;
        }

        #region nation
        private bool InitNationRatingRewardTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mNationRatingRewardTpl = new List<NationRatingRewardTemplate>();
            for (int i = 0; i < conditonSet.NationRatingRewardLength; i++)
            {
                var tpl = ParseNationRatingReward(conditonSet.GetNationRatingReward(i));
                mNationRatingRewardTpl.Add(tpl);
            }
            return true;
        }
        private NationRatingRewardTemplate ParseNationRatingReward(GM.DataCache.NationRatingReward obj)
        {
            NationRatingRewardTemplate tpl = new NationRatingRewardTemplate();

            tpl.rating = obj.Rating;
            tpl.rewardDatas = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.ItemId1))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId1, obj.ItemNum1, obj.ItemType1,false);
                tpl.rewardDatas.Add(itemData);
            }
            if (!string.IsNullOrEmpty(obj.ItemId2))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId2, obj.ItemNum2, obj.ItemType2, false);
                tpl.rewardDatas.Add(itemData);
            }
            if (!string.IsNullOrEmpty(obj.ItemId3))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId3, obj.ItemNum3, obj.ItemType3, false);
                tpl.rewardDatas.Add(itemData);
            }
            if (!string.IsNullOrEmpty(obj.ItemId4))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId4, obj.ItemNum4, obj.ItemType4, false);
                tpl.rewardDatas.Add(itemData);
            }
            return tpl;
        }

        public List<NationRatingRewardTemplate> GetNationRatingRewardTpls()
        {
            return mNationRatingRewardTpl;
        }

        public NationRatingRewardTemplate GetNationRatingRewardTpl(int rating)
        {
            NationRatingRewardTemplate result = mNationRatingRewardTpl.Find(m => m.rating == rating);
            if (result == null)
            {
                EB.Debug.LogWarning("GetNationRatingRewardTpl: NationRatingReward not found, rating = {0}", rating);
            }
            return result;
        }

        private bool InitNationRankRewardTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mNationRankRewardTpl = new List<NationRankRewardTemplate>();
            for (int i = 0; i < conditonSet.NationRankRewardLength; i++)
            {
                var tpl = ParseNationRankReward(conditonSet.GetNationRankReward(i));
                mNationRankRewardTpl.Add(tpl);
            }
            return true;
        }
        private NationRankRewardTemplate ParseNationRankReward(GM.DataCache.NationRankReward obj)
        {
            NationRankRewardTemplate tpl = new NationRankRewardTemplate();

            tpl.rank = obj.Rank;
            tpl.rewardDatas = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.ItemId1))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId1, obj.ItemNum1, obj.ItemType1, false);
                tpl.rewardDatas.Add(itemData);
            }
            if (!string.IsNullOrEmpty(obj.ItemId2))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId2, obj.ItemNum2, obj.ItemType2, false);
                tpl.rewardDatas.Add(itemData);
            }
            if (!string.IsNullOrEmpty(obj.ItemId3))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId3, obj.ItemNum3, obj.ItemType3, false);
                tpl.rewardDatas.Add(itemData);
            }
            if (!string.IsNullOrEmpty(obj.ItemId4))
            {
                LTShowItemData itemData = new LTShowItemData(obj.ItemId4, obj.ItemNum4, obj.ItemType4, false);
                tpl.rewardDatas.Add(itemData);
            }
            return tpl;
        }

        public List<NationRankRewardTemplate> GetNationRankRewardTpls()
        {
            return mNationRankRewardTpl;
        }

        public NationRankRewardTemplate GetNationRankRewardTpl(int rank)
        {
            NationRankRewardTemplate result = mNationRankRewardTpl.Find(m => m.rank == rank);
            if (result == null)
            {
                EB.Debug.LogWarning("GetNationRankRewardTpl: NationRankReward not found, rank = {0}", rank);
            }
            return result;
        }

        private bool InitNationSpecialEventTbl(GM.DataCache.ConditionEvent conditonSet)
        {
            mNationSpecialEventTpl = new List<NationSpecialEventTemplate>();
            for (int i = 0; i < conditonSet.NationSpecialEventLength; i++)
            {
                var tpl = ParseNationSpecialEvent(conditonSet.GetNationSpecialEvent(i));
                mNationSpecialEventTpl.Add(tpl);
            }
            return true;
        }
        private NationSpecialEventTemplate ParseNationSpecialEvent(GM.DataCache.NationSpecialEvent obj)
        {
            NationSpecialEventTemplate tpl = new NationSpecialEventTemplate();

            tpl.id = obj.Id;
            tpl.desc = EB.Localizer.GetTableString(string.Format("ID_event_nation_special_event_{0}_desc", tpl.id), obj.Desc);// ;
            tpl.result_desc1 = EB.Localizer.GetTableString(string.Format("ID_event_nation_special_event_{0}_result_desc1", tpl.id), obj.ResultDesc1);// ;
            tpl.result_desc2 = EB.Localizer.GetTableString(string.Format("ID_event_nation_special_event_{0}_result_desc2", tpl.id), obj.ResultDesc2);//;		
            return tpl;
        }

        private bool InitInfiniteChallengeTbl(GM.DataCache.ConditionEvent data)
        {
            mInfiniteChallengeTplDic = new Dictionary<int, InfiniteChallengeTemplate>();
            for (int i = 0; i < data.InfinitechallengeLength; i++)
            {
                var tpl = ParseInfiniteChallengeTpl(data.GetInfinitechallenge(i), i + 1);
                mInfiniteChallengeTplDic.Add(tpl.layer, tpl);
            }
            return true;
        }

        private InfiniteChallengeTemplate ParseInfiniteChallengeTpl(GM.DataCache.InfiniteChallenge obj, int index)
        {
            InfiniteChallengeTemplate tpl = new InfiniteChallengeTemplate();
            tpl.layer = index;
            tpl.level = obj.Chapter;
            tpl.award = ParseShowItem(obj.Award);
            tpl.first_award = ParseShowItem(obj.FirstAward);
            tpl.model_name = obj.ModelName;
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_event_infinitechallenge_{0}_name", tpl.layer), obj.Name);//;
            tpl.model_level = obj.Level;
            tpl.combat_layout = obj.CombatLayoutName;
            return tpl;
        }
        private List<LTShowItemData> ParseShowItem(string itemStr)
        {
            if (itemStr == null) return null;
            List<LTShowItemData> itemList = new List<LTShowItemData>();
            string[] itemData = itemStr.Split(';');
            for (int i = 0; i < itemData.Length; i++)
            {
                string[] item = itemData[i].Split(',');
                if(item.Length>2) itemList.Add(new LTShowItemData(item[0], int.Parse(item[1]), item[2], false));
            }
            return itemList;
        }

        private List<LTShowItemData> ParseShowItemWithMultiple(string itemStr)
        {
            if (itemStr == null) return null;
            List<LTShowItemData> itemList = new List<LTShowItemData>();
            string[] itemData = itemStr.Split(';');
            for (int i = 0; i < itemData.Length; i++)
            {
                string[] item = itemData[i].Split(',');
                itemList.Add(new LTShowItemData(item[0], int.Parse(item[1]), item[2], false, item.Length > 3 ? float.Parse(item[3]) : 1));
            }
            return itemList;
        }

        private bool InitInfiniteBuffTbl(GM.DataCache.ConditionEvent data)
        {
            mInfiniteBuffTpl = new List<InfiniteBuffTemplate>();
            for (int i = 0; i < data.InfiniteBuffLength; i++)
            {
                var tpl = ParseInfiniteBuffTpl(data.GetInfiniteBuff(i));
                mInfiniteBuffTpl.Add(tpl);
            }
            return true;
        }

        private InfiniteBuffTemplate ParseInfiniteBuffTpl(GM.DataCache.InfiniteBuff obj)
        {
            InfiniteBuffTemplate tpl = new InfiniteBuffTemplate();
            tpl.level = obj.Level;
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_event_infinite_buff_{0}_name", tpl.level), obj.Name);//;
            tpl.conditionNum = obj.ConditionNum;
            tpl.effect = obj.Effect;
            return tpl;
        }

        private bool InitBossRewardTbl(GM.DataCache.ConditionEvent data)
        {
            mBossRewardTpl = new List<BossRewardTemplate>();
            mBossStageRewardDic = new Dictionary<float, string>();
            for (int i = 0; i < data.BossrewardLength; i++)
            {
                var tpl = ParseBossRewardTpl(data.GetBossreward(i));
                if (tpl.id != 0)
                {
                    mBossRewardTpl.Add(tpl);
                }
                else if (tpl.stageReward != 0)
                {
                    mBossStageRewardDic.Add(tpl.stageReward, tpl.box);
                }
            }
            return true;
        }

        private BossRewardTemplate ParseBossRewardTpl(GM.DataCache.BossReward obj)
        {
            BossRewardTemplate tpl = new BossRewardTemplate();
            tpl.id = obj.Id;
            tpl.ranks = obj.Ranks;
            tpl.reward = obj.Reward;
            tpl.stageReward = obj.StageReward;
            tpl.box = obj.Box;
            return tpl;
        }

        private bool InitUREventRewardTbl(GM.DataCache.ConditionEvent data)
        {
            mUREventRewardTpl = new List<UREventRewardTemplate>();
            for (int i = 0; i < data.UrEventRewardLength; i++)
            {
                var tpl = ParseURPartnerRewardTpl(data.GetUrEventReward(i));
                mUREventRewardTpl.Add(tpl);
            }
            return true;
        }

        private UREventRewardTemplate ParseURPartnerRewardTpl(GM.DataCache.UrEventReward obj)
        {
            UREventRewardTemplate tpl = new UREventRewardTemplate();
            tpl.id = obj.Id;
            tpl.type = obj.Type;
            string[] rankarray = obj.Ranks.Split(';');
            if (rankarray.Length >= 2)
            {
                int.TryParse(rankarray[0], out tpl.top);
                int.TryParse(rankarray[1], out tpl.down);
            }
            tpl.reward = ParseShowItem(obj.Reward);
            return tpl;
        }

        private bool InitHonorArenaRewardTbl(GM.DataCache.ConditionEvent data)
        {
            mHonorArenaRewardTpl = new List<HonorArenaRewardTemplate>();
            for (int i = 0; i < data.HonorArenaRewardLength; i++)
            {
                var tpl = ParseArenaRewardTpl(data.GetHonorArenaReward(i));
                if (tpl.id != 0)
                {
                    mHonorArenaRewardTpl.Add(tpl);
                }
            }
            return true;
        }

        private bool InitHeroBattle(GM.DataCache.ConditionEvent data)
        {
            mHeroBattleTpl = new List<HeroBattleTemplate>();
            for (int i = 0; i < data.HeroBattleLength; i++)
            {
                var tpl = ParseHeroBattleTpl(data.GetHeroBattle(i));
                mHeroBattleTpl.Add(tpl);
            }
            return true;
        }

        public List<HeroBattleTemplate> GetHeroBattleData()
        {
            return mHeroBattleTpl;
        }
        public HeroBattleTemplate GetNextHeroBattleData(int id)
        {
            int index = mHeroBattleTpl.FindIndex(hb => hb.Id == id);
            index = Math.Min(index+1, mHeroBattleTpl.Count - 1);
            return mHeroBattleTpl[index];
        }
        
        private HeroBattleTemplate ParseHeroBattleTpl(GM.DataCache.HeroBattle obj)
        {
            HeroBattleTemplate tpl = new HeroBattleTemplate(obj);
            return tpl;
        }

        private HonorArenaRewardTemplate ParseArenaRewardTpl(GM.DataCache.HonorArenaReward obj)
        {
            HonorArenaRewardTemplate tpl = new HonorArenaRewardTemplate();
            tpl.id = obj.Id;
            tpl.ranks = obj.Ranks;
            tpl.reward = obj.Reward;
            tpl.oneHourReward = obj.OneHourReward;
            return tpl;
        }

        private bool InitCronJobsTbl(GM.DataCache.ConditionEvent data)
        {
            mCronJobsTbl = new Dictionary<string, CronJobs>();
            for (int i = 0; i < data.CronjobsLength; i++)
            {
                var tpl = ParseCronJobsTpl(data.GetCronjobs(i));
                if (!mCronJobsTbl.ContainsKey(tpl.name))
                {
                    mCronJobsTbl.Add(tpl.name, tpl);
                }
                else
                {
                    mCronJobsTbl[tpl.name] = tpl;
                }

                //if (tpl.name == "escort_start")
                //{
                //    tpl.interval = "0 0 10 * * *";
                //}

                //if (tpl.name == "escort_stop")
                //{
                //    tpl.interval = "0 0 18 * * *";
                //}
            }
            return true;
        }

        private CronJobs ParseCronJobsTpl(GM.DataCache.CronJobs obj)
        {
            CronJobs tpl = new CronJobs();
            tpl.name = obj.Name;
            tpl.interval = obj.Interval;
            tpl.type = obj.Type;
            tpl.isSchedule = obj.IsSchedule;
            tpl.scope = obj.Scope;
            tpl.isOpen = obj.Open;
            tpl.closeRealmId = new int[obj.CloseRealmIdLength];
            for (int i = 0; i < obj.CloseRealmIdLength; ++i)
            {
                tpl.closeRealmId[i] = obj.GetCloseRealmId(i);
            }
            return tpl;
        }

        /// <summary>
        /// 初始化伙伴培养礼包
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool InitPartnerCultivateGiftTbl(GM.DataCache.ConditionEvent data)
        {
            mPartnerCultivateGiftDic = new Dictionary<int, PartnerCultivateGift>();
            for (int i = 0; i < data.PartnerCultivateGiftLength; i++)
            {
                var tpl = ParsePartnerCultivateGiftTpl(data.GetPartnerCultivateGift(i));
                if (!mPartnerCultivateGiftDic.ContainsKey(tpl.id))
                {
                    mPartnerCultivateGiftDic.Add(tpl.id, tpl);
                }
                else
                {
                    mPartnerCultivateGiftDic[tpl.id] = tpl;
                }
            }
            return true;
        }

        /// <summary>
        /// 实例化伙伴培养礼包
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private PartnerCultivateGift ParsePartnerCultivateGiftTpl(GM.DataCache.PartnerCultivateGift obj)
        {
            PartnerCultivateGift tpl = new PartnerCultivateGift();
            tpl.id = obj.Id;
            tpl.title = EB.Localizer.GetTableString(string.Format("ID_event_partner_cultivate_gift_{0}_title", tpl.id), obj.Title);
            tpl.icon = obj.Icon;
            tpl.discount = obj.Discount;

            return tpl;
        }

        private bool InitDailyRewardTbl(GM.DataCache.ConditionEvent data)
        {
            mDailyRewardList = new List<DailyRewardTemplate>();
            for (int i = 0; i < data.DailyRewardLength; i++)
            {
                var tpl = ParseDailyRewardTpl(data.GetDailyReward(i));
                mDailyRewardList.Add(tpl);
            }
            return true;
        }

        private DailyRewardTemplate ParseDailyRewardTpl(GM.DataCache.DailyReward obj)
        {
            DailyRewardTemplate tpl = new DailyRewardTemplate();
            tpl.id = obj.Id;
            tpl.type = obj.Type;
            tpl.title = obj.Title;
            tpl.desc = obj.Desc;
            tpl.ItemList = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.ItemId)) tpl.ItemList.Add(new LTShowItemData(obj.ItemId, obj.ItemNum, obj.ItemType, false));
            if (!string.IsNullOrEmpty(obj.ItemId2)) tpl.ItemList.Add(new LTShowItemData(obj.ItemId2, obj.ItemNum2, obj.ItemType2, false));
            if (!string.IsNullOrEmpty(obj.ItemId3)) tpl.ItemList.Add(new LTShowItemData(obj.ItemId3, obj.ItemNum3, obj.ItemType3, false));
            if (!string.IsNullOrEmpty(obj.ItemId4)) tpl.ItemList.Add(new LTShowItemData(obj.ItemId4, obj.ItemNum4, obj.ItemType4, false));
            return tpl;
        }

        private bool InitHeroMedalTbl(GM.DataCache.ConditionEvent data)
        {
            mHeroMedalList = new List<HeroMedalTemplate>();
            for (int i = 0; i < data.HeroMedalLength; i++)
            {
                var tpl = ParseHeroMedalTpl(data.GetHeroMedal(i));
                mHeroMedalList.Add(tpl);
            }
            return true;
        }

        private HeroMedalTemplate ParseHeroMedalTpl(GM.DataCache.HeroMedal obj)
        {
            HeroMedalTemplate tpl = new HeroMedalTemplate();
            tpl.stage = obj.Stage;
            tpl.ItemList = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.Ri1)) tpl.ItemList.Add(new LTShowItemData(obj.Ri1, obj.Rn1, obj.Rt1, false));
            if (!string.IsNullOrEmpty(obj.Ri2)) tpl.ItemList.Add(new LTShowItemData(obj.Ri2, obj.Rn2, obj.Rt2, false));
            if (!string.IsNullOrEmpty(obj.Ri3)) tpl.ItemList.Add(new LTShowItemData(obj.Ri3, obj.Rn3, obj.Rt3, false));
            tpl.ExItemList = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.Eri1)) tpl.ExItemList.Add(new LTShowItemData(obj.Eri1, obj.Ern1, obj.Ert1, false));
            if (!string.IsNullOrEmpty(obj.Eri2)) tpl.ExItemList.Add(new LTShowItemData(obj.Eri2, obj.Ern2, obj.Ert2, false));
            if (!string.IsNullOrEmpty(obj.Eri3)) tpl.ExItemList.Add(new LTShowItemData(obj.Eri3, obj.Ern3, obj.Ert3, false));
            return tpl;
        }

        private bool InitMainCampaignRewardTbl(GM.DataCache.ConditionEvent data)
        {
            mMainCampaignRewardList = new List<MainCampaignRewardTemplate>();
            for (int i = 0; i < data.MainCampaignRewardLength; ++i)
            {
                var tpl = ParseMainCampaignRewardTpl(data.GetMainCampaignReward(i));
                mMainCampaignRewardList.Add(tpl);
            }
            return true;
        }

        private MainCampaignRewardTemplate ParseMainCampaignRewardTpl(GM.DataCache.MainCampaignReward obj)
        {
            MainCampaignRewardTemplate tpl = new MainCampaignRewardTemplate();
            tpl.stage =int.Parse(obj.Stage);
            tpl.ItemList = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.Ri1)) tpl.ItemList.Add(new LTShowItemData(obj.Ri1, obj.Rn1, obj.Rt1, false));
            if (!string.IsNullOrEmpty(obj.Ri2)) tpl.ItemList.Add(new LTShowItemData(obj.Ri2, obj.Rn2, obj.Rt2, false));
            if (!string.IsNullOrEmpty(obj.Ri3)) tpl.ItemList.Add(new LTShowItemData(obj.Ri3, obj.Rn3, obj.Rt3, false));
            tpl.ExItemList = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.Eri1)) tpl.ExItemList.Add(new LTShowItemData(obj.Eri1, obj.Ern1, obj.Ert1, false));
            if (!string.IsNullOrEmpty(obj.Eri2)) tpl.ExItemList.Add(new LTShowItemData(obj.Eri2, obj.Ern2, obj.Ert2, false));
            if (!string.IsNullOrEmpty(obj.Eri3)) tpl.ExItemList.Add(new LTShowItemData(obj.Eri3, obj.Ern3, obj.Ert3, false));
            return tpl;
        }

        private bool InitAwakenDungeonTbl(GM.DataCache.ConditionEvent data)
        {
            mAwakenDungeonList = new List<AwakenDungeonTemplate>();
            for (int i = 0; i < data.AwakenDungeonLength; i++)
            {
                var tpl = ParseAwakenDungeonTpl(data.GetAwakenDungeon(i));
                mAwakenDungeonList.Add(tpl);
            }
            return true;
        }

        private AwakenDungeonTemplate ParseAwakenDungeonTpl(GM.DataCache.AwakenDungeon obj)
        {
            AwakenDungeonTemplate tpl = new AwakenDungeonTemplate();
            tpl.ID = obj.Id;
            tpl.Type = (eRoleAttr)obj.Type;
            tpl.Stage = obj.Stage;
            //  tpl.Name = EB.Localizer.GetTableString(string.Format("ID_event_awaken_dungeon_{0}_name", tpl.ID), obj.Name);
            tpl.Name = string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68326"), obj.Stage);
            tpl.CombatLayoutName = obj.CombatLayoutName;
            tpl.DropAward = ParseShowItemWithMultiple(obj.DropAward);
            tpl.DropAwardFirst = ParseShowItem(obj.FirstAward);
            return tpl;
        }

        private void InitDiamondGiftTemplate(GM.DataCache.ConditionEvent data)
        {
            DiamondGiftDict = new Dictionary<int, List<DiamondGiftTemplate>>();

            for (int i = 0; i < data.DiamondGiftLength; i++)
            {
                var diamondGift = data.GetDiamondGift(i);
                var diamondGiftTemplate = ParseDiamondGiftTemplate(diamondGift);
                var days = diamondGift.Days;

                if (DiamondGiftDict.ContainsKey(days))
                {
                    DiamondGiftDict[days].Add(diamondGiftTemplate);
                }
                else
                {
                    var list = new List<DiamondGiftTemplate>();
                    list.Add(diamondGiftTemplate);
                    DiamondGiftDict.Add(days, list);
                }
            }
        }

        private DiamondGiftTemplate ParseDiamondGiftTemplate(GM.DataCache.DiamondGift obj)
        {
            var tpl = new DiamondGiftTemplate();
            tpl.id = obj.Id;
            tpl.days = obj.Days;
            tpl.originalPrice = obj.OriginalPrice;
            tpl.activityPrice = obj.ActivityPrice;
            tpl.lTShowItemData = new LTShowItemData(obj.ItemId, obj.ItemNum, obj.ItemType, false);
            return tpl;
        }

        private bool InitLevelCompensatedRewardTbl(GM.DataCache.ConditionEvent data)
        {
            mLevelCompensatedRewardDic = new Dictionary<int, LevelCompensatedRewardTemplate>();
            for (int i = 0; i < data.LevelCompensatedRewardLength; i++)
            {
                LevelCompensatedRewardTemplate tempreward = ParseLevelCompensatedRewardTpl(data.GetLevelCompensatedReward(i));
                mLevelCompensatedRewardDic.Add(tempreward.id, tempreward);
            }
            return true;
        }

        public LevelCompensatedRewardTemplate ParseLevelCompensatedRewardTpl(GM.DataCache.LevelCompensatedReward obj)
        {
            LevelCompensatedRewardTemplate tempReward = new LevelCompensatedRewardTemplate();
            tempReward.id = obj.Id;
            tempReward.days = obj.Days;
            tempReward.minLevel = obj.MinLevel;
            tempReward.maxLevel = obj.MaxLevel;
            List<LTShowItemData> data = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.RewardContent))
            {
                string[] rewardStrArray = obj.RewardContent.Split(';');
                string[] paramArray;
                for (int i = 0; i < rewardStrArray.Length; i++)
                {
                    paramArray = rewardStrArray[i].Split(',');
                    data.Add(new LTShowItemData(paramArray[0], int.Parse(paramArray[1]), paramArray[2]));
                }
                tempReward.rewardList = data;
            }
            return tempReward;
        }

        public List<AwakenDungeonTemplate> GetAwakenDungeonsByType(eRoleAttr type)
        {
            List<AwakenDungeonTemplate> TypeList = new List<AwakenDungeonTemplate>();
            for (int i = 0; i < mAwakenDungeonList.Count; i++)
            {
                if (mAwakenDungeonList[i].Type == type) TypeList.Add(mAwakenDungeonList[i]);
            }
            return TypeList;
        }

        public AwakenDungeonTemplate GetAwakenDungeonByTypeAndStage(eRoleAttr type, int stage)
        {
            for (int i = 0; i < mAwakenDungeonList.Count; i++)
            {
                if (mAwakenDungeonList[i].Type == type && mAwakenDungeonList[i].Stage == stage)
                {
                    return mAwakenDungeonList[i];
                }
            }
            return null;
        }

        public HeroMedalTemplate GetHeroMedalByType(int stage)
        {
            for (int i = 0; i < mHeroMedalList.Count; i++)
            {
                if (mHeroMedalList[i].stage == stage)
                {
                    return mHeroMedalList[i];
                }
            }
            return null;
        }

        public List<HeroMedalTemplate> GetAllHeroMedalTp1()
        {
            return mHeroMedalList;
        }

        public MainCampaignRewardTemplate GetMainCampaignRewardByType(int stage)
        {
            for (int i = 0; i < mMainCampaignRewardList.Count; i++)
            {
                if (mMainCampaignRewardList[i].stage == stage)
                {
                    return mMainCampaignRewardList[i];
                }
            }
            return null;
        }

        public List<MainCampaignRewardTemplate> GetAllMainCampaignRewardTp1()
        {
            return mMainCampaignRewardList;
        }

        public List<LTShowItemData> GetTotalHeroMedalItemTp1()
        {
            Dictionary<string, LTShowItemData> temp = new Dictionary<string, LTShowItemData>();
            for (int i = 0; i < mHeroMedalList.Count; ++i)
            {
                for (int j = 0; j < mHeroMedalList[i].ExItemList.Count; ++j)
                {
                    if (temp.ContainsKey(mHeroMedalList[i].ExItemList[j].id))
                    {
                        temp[mHeroMedalList[i].ExItemList[j].id].count += mHeroMedalList[i].ExItemList[j].count;
                    }
                    else
                    {
                        temp[mHeroMedalList[i].ExItemList[j].id] = new LTShowItemData(mHeroMedalList[i].ExItemList[j].id, mHeroMedalList[i].ExItemList[j].count, mHeroMedalList[i].ExItemList[j].type, false);
                    }
                }
            }

            return temp.Values.ToList();
        }

        public List<LTShowItemData> GetTotalMainInstanceItemTp1()
        {
            Dictionary<string, LTShowItemData> temp = new Dictionary<string, LTShowItemData>();
            for (int i = 0; i < mMainCampaignRewardList.Count; ++i)
            {
                for (int j = 0; j < mMainCampaignRewardList[i].ExItemList.Count; ++j)
                {
                    if (temp.ContainsKey(mMainCampaignRewardList[i].ExItemList[j].id))
                    {
                        temp[mMainCampaignRewardList[i].ExItemList[j].id].count += mMainCampaignRewardList[i].ExItemList[j].count;
                    }
                    else
                    {
                        temp[mMainCampaignRewardList[i].ExItemList[j].id] = new LTShowItemData(mMainCampaignRewardList[i].ExItemList[j].id, mMainCampaignRewardList[i].ExItemList[j].count, mMainCampaignRewardList[i].ExItemList[j].type, false);
                    }
                }
            }

            return temp.Values.ToList();
        }

        public DailyRewardTemplate GetDailyRewardByType(string type)
        {
            for (int i = 0; i < mDailyRewardList.Count; i++)
            {
                if (mDailyRewardList[i].type == type)
                {
                    return mDailyRewardList[i];
                }
            }
            return null;
        }

        public List<InfiniteBuffTemplate> GetAllInfiniteBuffTp1()
        {
            return mInfiniteBuffTpl;
        }

        public InfiniteChallengeTemplate GetInfiniteChallengeTpl(int layer)
        {
            if (mInfiniteChallengeTplDic.ContainsKey(layer))
            {
                return mInfiniteChallengeTplDic[layer];
            }
            return null;
        }

        public int GetInfiniteChallengeLevelByLayer(int layer)
        {
            if (mInfiniteChallengeTplDic.ContainsKey(layer))
            {
                return mInfiniteChallengeTplDic[layer].level;
            }
            return 0;
        }

        public int CalculCurLayer(InfiniteChallengeTemplate tpl)
        {
            List<InfiniteChallengeTemplate> list = GetInfiniteChallengeTplListByLevel(tpl.level);
            return list.IndexOf(tpl) + 1;
        }

        public List<int> GetInfiniteChallengeAllLevel()
        {
            List<int> levelList = new List<int>();
            int curLevel = 0;

            var enumerator = mInfiniteChallengeTplDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                if (curLevel != pair.Value.level)
                {
                    curLevel = pair.Value.level;
                    levelList.Add(pair.Value.level);
                }
            }
            return levelList;
        }

        public List<InfiniteChallengeTemplate> GetInfiniteChallengeTplListByLevel(int level)
        {
            List<InfiniteChallengeTemplate> list = new List<InfiniteChallengeTemplate>();
            var enumerator = mInfiniteChallengeTplDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                if (level == pair.Value.level)
                {
                    list.Add(pair.Value);
                }
            }
            return list;
        }

        public List<NationSpecialEventTemplate> GetNationSpecialEventTpls()
        {
            return mNationSpecialEventTpl;
        }

        public NationSpecialEventTemplate GetNationSpecialEventTpl(int id)
        {
            NationSpecialEventTemplate result = mNationSpecialEventTpl.Find(m => m.id == id);
            if (result == null)
            {
                EB.Debug.LogWarning("GetNationSpecialEventTpl: NationSpecialEvent not found, id = {0}", id);
            }
            return result;
        }
        #endregion

        public List<SpecialActivityTemplate> GetAllSpecActData()
        {
            List<SpecialActivityTemplate> list = new List<SpecialActivityTemplate>(mSpectialActivityTbl.Values);
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (!list[i].isShowDaily)
                {
                    list.Remove(list[i]);
                }
            }

            return list;
        }

        public SpecialActivityTemplate GetSpecialActivity(int id)
        {
            SpecialActivityTemplate result = null;
            if (!mSpectialActivityTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetSpecialActivity: SpecialActivity not found, id = {0}", id);
            }
            return result;
        }

        public SpecialActivityLevelTemplate GetSpecialActivityLevel(int id)
        {
            SpecialActivityLevelTemplate result = null;
            if (!mSpectialActivityLevelTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetSpecialActivityLevel: SpecialActivityLevel not found, id = {0}", id);
            }
            return result;
        }

        public List<SpecialActivityLevelTemplate> GetSpecialActivityLevels(int id)
        {
            if (mSpecialActivityLevels.ContainsKey(id))
            {
                List<SpecialActivityLevelTemplate> enableList = new List<SpecialActivityLevelTemplate>();
                for (int i = 0; i < mSpecialActivityLevels[id].Count; i++)
                {
                    if (mSpecialActivityLevels[id][i].level < 1000)
                    {
                        enableList.Add(mSpecialActivityLevels[id][i]);
                    }
                }
                return enableList;
            }
            else
            {
                EB.Debug.LogError("GetSpecialActivityLevels: SpecialActivity contains nothing, id = {0}", id);
                return new List<SpecialActivityLevelTemplate>();
            }
        }

        public int GetWorldBossIndex(string curLayout)
        {
            List<SpecialActivityLevelTemplate> levelTplList = EventTemplateManager.Instance.GetSpecialActivityLevels(LTWorldBossDataManager.ActivityId);
            for (int i = 0; i < levelTplList.Count; i++)
            {
                if (levelTplList[i].layout == curLayout)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetSpecialActivityLevelID(int id, int index)
        {
            if (mSpectialActivityLevelTbl.ContainsKey(id + 1))
            {

                return mSpectialActivityLevelTbl[(id + 1)].id;
            }
            else
            {
                string idStr = id.ToString();
                if (index == 0)
                {
                    List<SpecialActivityLevelTemplate> goldLists = mSpecialActivityLevels[9001];
                    if (id < goldLists[0].id) return goldLists[0].id;
                    else if (id == goldLists[goldLists.Count - 1].id) return goldLists[goldLists.Count - 1].id;
                    else
                    {
                        EB.Debug.LogError("GetSpecialActivityLevelID id err {0}" , idStr);
                        return 0;
                    }
                }
                else if (index == 1)
                {
                    List<SpecialActivityLevelTemplate> expLists = mSpecialActivityLevels[9002];
                    if (id < expLists[0].id) return expLists[0].id;
                    else if (id == expLists[expLists.Count - 1].id) return expLists[expLists.Count - 1].id;
                    else
                    {
                        EB.Debug.LogError("GetSpecialActivityLevelID id err {0}" , idStr);
                        return 0;
                    }
                }
                else
                {
                    EB.Debug.LogError("GetSpecialActivityLevelID id can't find {0}" , idStr);
                    return 0;
                }
            }
        }

        public List<NormalActivityInstanceTemplate> GetNormalActivityInstances()
        {
            return mNormalActivityInstanceTbl;
        }

        public TimeLimitActivityTemplate GetTimeLimitActivity(int id)
        {
            TimeLimitActivityTemplate result = null;
            if (!mTimeLimitActivityTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetTimeLimitActivity: TimeLimitActivity not found, id = {0}", id);
            }
            return result;
        }

        public TimeLimitActivityStageTemplate GetTimeLimitActivityStage(int id)
        {
            TimeLimitActivityStageTemplate result = null;
            if (!mTimeLimitActivityStageTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetTimeLimitActivityStage: TimeLimitActivityStage not found, id = {0}", id);
            }
            return result;
        }

        public BossChallengeActivityConfigTemplate GetBossChallengeActivityConfig(int id)
        {
            BossChallengeActivityConfigTemplate result = null;
            if (!mBossChallengeActivityConfigTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetBossChallengeActivityConfig: BossChallengeActivityConfig not found, id = {0}", id);
            }
            return result;
        }

		public BossChallengeActivityTotalTimeTemplate GetBossChallengeActivityTotalTime(int id)
		{
			BossChallengeActivityTotalTimeTemplate result = null;
			if (!mBossChallengeActivityTotalTimeTbl.TryGetValue(id, out result))
			{
				EB.Debug.LogWarning("GetBossChallengeActivityTotalTime: BossChallengeActivityTotalTime not found, id = {0}", id);
			}
			return result;
		}

        public List<LTRankAwardData> GetURPartnerEventRewardList(int infoId)
        {
            List<LTRankAwardData> result = new List<LTRankAwardData>();
            int count = mUREventRewardTpl.Count;
            for (int i = 0; i < count; i++)
            {
                var temp = mUREventRewardTpl[i];
                if (temp.type == infoId)
                {
                    result.Add(new LTRankAwardData(temp.top, temp.down, temp.reward));
                }
            }
            return result;
        }

        public int GetURPartnerEventRewardMaxNum(int infoId)
        {
            int count = mUREventRewardTpl.Count;
            int value = 0;
            for (int i = 0; i < count; i++)
            {
                var temp = mUREventRewardTpl[i];               
                if (temp.type == infoId)
                {
                    value = temp.top > value ? temp.top : value;
                }
            }
            return value-1;
        }

        public List<TimeLimitActivityStageTemplate> GetTimeLimitActivityStages(int id)
        {
            if (mTimeLimitActivityStages.ContainsKey(id))
            {
                return mTimeLimitActivityStages[id];
            }
            else
            {
                //EB.Debug.LogError("GetTimeLimitActivityStages: TimeLimitActivity contains nothing, id = {0}", id);
                return new List<TimeLimitActivityStageTemplate>();
            }
        }

        public List<BossChallengeActivityConfigTemplate> GetBossChallengeActivityConfigs(int activityId)
        {
            if (mBossChallengeActivityConfigs.ContainsKey(activityId))
            {
                return mBossChallengeActivityConfigs[activityId];
            }
            else
            {
                return new List<BossChallengeActivityConfigTemplate>();
            }
        }

		public List<BossChallengeActivityTotalTimeTemplate> GetBossChallengeActivityTotalTimes(int activityId)
		{
			if (mBossChallengeActivityTotalTimes.ContainsKey(activityId))
			{
				return mBossChallengeActivityTotalTimes[activityId];
			}
			else
				return new List<BossChallengeActivityTotalTimeTemplate>();
		}

        public List<BossRewardTemplate> GetBossRewardList()
        {
            return mBossRewardTpl;
        }

        public List<HonorArenaRewardTemplate> GetHonorArenaRewardList()
        {
            return mHonorArenaRewardTpl;
        }

        public List<float> GetBossStages()
        {
            return mBossStageRewardDic.Keys.ToList();
        }

        public string GetBossStageReward(float stage)
        {
            if (mBossStageRewardDic.ContainsKey(stage))
            {
                return mBossStageRewardDic[stage];
            }
            return "";
        }

        public CronJobs GetCronJobsByName(string name)
        {
            CronJobs cronJobs = null;
            mCronJobsTbl.TryGetValue(name, out cronJobs);
            return cronJobs;
        }
        
        /// <summary>
        /// InfiniteCompete_start
        /// </summary>
        /// <param name="key"></param>
        public DateTime GetNextTime(string key)
        {
            string timeStr = string.Empty;
            System.DateTime now = Data.ZoneTimeDiff.GetServerTime();
            System.DateTime startdate = new System.DateTime();
            var mStartCronJobs = EventTemplateManager.Instance.GetCronJobsByName(key);
            if (mStartCronJobs!=null)
            {
                string[] start= mStartCronJobs.interval.Split(' ');
                if (start.Length == 6)
                {
                    TimerScheduler timerScheduler = new TimerScheduler();
                    timerScheduler.m_TimerRegular = string.Format("{0} {1} {2} {3} {4}", start[1], start[2], start[3], start[4], start[5]);
                    timerScheduler.ParseTimerRegular();
                    if (!timerScheduler.isLegalTimer)
                    {
                        EB.Debug.LogError(string.Format("{0}: cronFormat is illegal",key));
                    }
                    timerScheduler.GetNext(now, out startdate);
                }
            }
            return startdate;
        }

        /// <summary>
        ///  //解析cron
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool IsTimeOK(LTActivityTime startTime, LTActivityTime endTime)
        {
            int curweek = EB.Time.LocalWeek;
            if (startTime.week != null)
            {
                for (int i = 0; i < startTime.week.Length; i++)
                {
                    if (curweek == startTime.week[i])
                    {
                        return IsTimeOKByDay(startTime, endTime);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 活动是否开启了
        /// </summary>
        private bool IsTimeOKByDay(LTActivityTime startTime, LTActivityTime endTime)
        {
            if (startTime.hour.Length != endTime.hour.Length)
            {
                EB.Debug.LogError("EventTemplateManager IsTimeOKByDay is Error!!!");
                return false;
            }

            for (int i = 0; i < startTime.hour.Length; i++)
            {
                ActivityTime startT = new ActivityTime(startTime.hour[i], startTime.minute, startTime.second);
                ActivityTime endT = new ActivityTime(endTime.hour[i], endTime.minute, endTime.second);

                int start = ZoneTimeDiff.GetTime(startT);
                int end = ZoneTimeDiff.GetTime(endT);

                if (EB.Time.Now >= start && EB.Time.Now <= end)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsTimeOK(string startTime, string endTime)
        {
            LTActivityTime OpenTime = GetActivityTimeByCronJobsName(startTime);
            LTActivityTime EndTime = GetActivityTimeByCronJobsName(endTime);
            bool istimeok = IsTimeOK(OpenTime, EndTime);
            return istimeok;
        }

        //解析cron:拓展的解析，目前仅用于夺宝奇兵,处理每小时开启一次的情况，其他情况调用IsTimeOK
        public bool IsTimeOKEX(LTActivityTime OpenTime, LTActivityTime EndTime)
        {
            int curtime = EB.Time.LocalTimeOfDay.Minutes;
            return (EB.Time.LocalTimeOfDay.Minutes >= OpenTime.minute && curtime < EndTime.minute);
        }

        public LTActivityTime GetActivityTimeByCronJobsName(string name)
        {
            CronJobs cronJobs = null;
            mCronJobsTbl.TryGetValue(name, out cronJobs);
            LTActivityTime data = new LTActivityTime();
            if (cronJobs != null) data = new LTActivityTime(cronJobs.interval);
            return data;
        }
        public void SetActivityTime(Hashtable table)
        {
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("eventTimeSet", table, null);
            mActivityOpenTime = new Dictionary<int, ActivityTime>();
            mActivityStopTime = new Dictionary<int, ActivityTime>();
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
            LTDailyDataManager.Instance.InitDaiylData();
            UI.UserData.PushMsgPrefs();
        }

        private ActivityTime GetActivityTime(Hashtable table)
        {
            ActivityTime actTime = new ActivityTime();
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

        public ActivityTime GetActivityOpenTimeByActivityID(int actID)
        {
            // 如果是全天的活动只有开始时间
            ActivityTime actTime = null;
            mActivityOpenTime.TryGetValue(actID, out actTime);
            return actTime;
        }

        public ActivityTime GetActivityStopTimeByActivityID(int actID)
        {
            // 如果是全天的活动是没有结束时间的，这里返回的是null
            ActivityTime actTime = null;
            mActivityStopTime.TryGetValue(actID, out actTime);
            return actTime;
        }

        public string GetActivityOpenTimeStr(string startTimeName, string endTimeName)
        {
            string startTime = mCronJobsTbl[startTimeName].interval;
            string endTime = mCronJobsTbl[endTimeName].interval;
            string[] startTimeSps = startTime.Split(' ');
            string[] endTimeSps = endTime.Split(' ');
            if (startTimeSps[5] != endTimeSps[5])
            {
                EB.Debug.LogError("GetActivityOpenTimeStr: Cron week Not fit");
            }
            string dayResultStr = "";
            if (startTimeSps[5] == "*")
            {
                dayResultStr = EB.Localizer.GetString("ID_EVERY_DAY");
            }
            else
            {
                dayResultStr = EB.Localizer.GetString("ID_codefont_in_EventTemplateManager_43848") + " ";
                string[] weekStr = startTimeSps[5].Split(',');
                for (int j = 0; j < weekStr.Length; ++j)
                {
                    dayResultStr += (EB.Localizer.GetString(EventTemplateManager.NumText[int.Parse(weekStr[j])]));
                    if (j < dayResultStr.Length - 1)
                    {
                        dayResultStr += ",";
                    }
                }
            }

            string timeResultStr = "";
            string[] starthourSps = startTimeSps[2].Split(',');
            string[] endhourSps = endTimeSps[2].Split(',');
            int startminute = int.Parse(startTimeSps[1]);
            int endminute = int.Parse(endTimeSps[1]);
            for (int i = 0; i < starthourSps.Length; ++i)
            {
                string startTimeStr = string.Format("{0:00}:{1:00}", starthourSps[i], startminute);
                string endTimeStr = string.Format("{0:00}:{1:00}", endhourSps[i], endminute);
                timeResultStr += string.Format("{0}-{1}", startTimeStr, endTimeStr);
                if (i < starthourSps.Length - 1)
                    timeResultStr += EB.Localizer.GetString("ID_codefont_in_EventTemplateManager_44829");
            }
            return string.Format("{0}{1}", dayResultStr, timeResultStr);
        }

        public bool GetRealmIsOpen(string activityTimeName)
        {
            int[] closeRealmIds = EventTemplateManager.Instance.GetCronJobsByName(activityTimeName).closeRealmId;
            if (closeRealmIds.Length > 0)
            {
                int defaultRealmId = LoginManager.Instance.GetDefaultRealmId();

                int len = closeRealmIds.Length;
                for (int i = 0; i < len; i++)
                {
                    var realmid = closeRealmIds[i];
                    if (realmid == defaultRealmId)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据id获取伙伴培养礼包
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartnerCultivateGift GetPartnerCultivateGiftById(int id)
        {
            if (mPartnerCultivateGiftDic.ContainsKey(id))
            {
                return mPartnerCultivateGiftDic[id];
            }

            return null;
        }

        /// <summary>
        /// 获取指定的爬塔数据
        /// </summary>
        /// <param name="level">等级</param>
        /// <param name="layer">层</param>
        /// <param name="total">个数</param>
        /// <returns></returns>
        public List<ClimingTowerTemplate> GetClimingTowerData(int level, int layer, int total)
        {
            InitTowerList();
            layer--;
            List<ClimingTowerTemplate> list = new List<ClimingTowerTemplate>();
            for (int i = 0; i < total; i++)
            {
                ClimingTowerTemplate data = mClimingTowerList.Find(p => (p.level == level && p.layer == layer));
                if (data != null)
                {
                    list.Add(data);
                }
                layer++;
            }
            return list;
        }

        public ClimingTowerTemplate GetClimingTowerData()
        {
            InitTowerList();

            ClimingTowerTemplate data = mClimingTowerList.Find(p => (p.v_CanChallenge == true));
            return data;
        }
        public LevelCompensatedRewardTemplate GetFreeLevelCompensatedReward(int id)
        {
            if (mLevelCompensatedRewardDic.ContainsKey(id))
            {
                return mLevelCompensatedRewardDic[id];
            }
            return null;
        }
        public bool IsFreeLevelCompenstatedRewardId(int id)
        {
            return mLevelCompensatedRewardDic.ContainsKey(id);
        }

        public List<InfiniteCompeteTemplate> GetAllInfiniteCompete()
        {
            return mInfiniteCompeteTpl;
        }

        public List<InfiniteCompeteRewardTemplate> GetAllInfiniteCompeteReward()
        {
            return mInfiniteCompeteRewardTpl;
        }

        public List<InviteTaskTemplate> GetInviteTaskList()
        {
            return mInviteTaskList;
        }

        public List<ClimingTowerRewardTemplate> GetSleepRewardList()
        {
            return mClimingTowerRewardList;
        }

    }



    /// <summary>
    /// 服务器时区和本地时区的差异
    /// </summary>
    public static class ZoneTimeDiff
    {
        public static int GetTimeZone()
        {
            string serverTime = "";
            if (DataLookupsCache.Instance.SearchDataByID<string>("time_str", out serverTime))
            {
                int timeZone = 0;
                string ZoneStr = serverTime.Substring(serverTime.Length - 6, 3);
                timeZone = int.Parse(ZoneStr);
                return timeZone;
            }
            else
            {
                return 8;
            }
        }

        public static int GetLocalTimeZone()
        {
            var t = System.DateTime.Now - System.DateTime.UtcNow;
            return t.Hours;
        }
        
        public static int GetTime(ActivityTime activeTime)
        {
            int time = 0;
            DateTime dateTime = GetDateTime(activeTime);
            time = EB.Time.ToPosixTime(dateTime.ToUniversalTime());
            return time;
        }

        public static DateTime GetDateTime(ActivityTime activeTime)
        {
            string serverTime = "";
            if (DataLookupsCache.Instance.SearchDataByID<string>("time_str", out serverTime))
            {
                DateTime tempDT = GetServerTime();

                string newString = string.Format("{0}-{1}-{2}T{3}:{4}:{5}{6}",
                    tempDT.Year,
                    tempDT.Month,
                    tempDT.Day,
                    activeTime != null ? activeTime.hour.ToString("00") : "00",
                    activeTime != null ? activeTime.minute.ToString("00") : "00",
                    activeTime != null ? activeTime.second.ToString("00") : "00",
                    serverTime.Substring(serverTime.Length - 6));

                return DateTime.Parse(newString);
            }
            else
            {
                string newString = string.Format("{0}-{1}-{2}T{3}:{4}:{5}+08:00",
                    EB.Time.LocalYear,
                    EB.Time.LocalMonth,
                    EB.Time.LocalDay,
                    activeTime != null ? activeTime.hour.ToString("00") : "00",
                    activeTime != null ? activeTime.minute.ToString("00") : "00",
                    activeTime != null ? activeTime.second.ToString("00") : "00");

                return DateTime.Parse(newString);
            }
        }

        public static DateTime GetDateTime(int hour = 0, int minute = 0, int second = 0)
        {
            return GetDateTime(new ActivityTime(hour, minute, second));
        }

        /// <summary>
        /// 获取当前服务器日期时间,如果有参数TimeStamp则将时间戳转化成服务器日期时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerTime(long TimeStamp = 0)
        {
            DateTime dtStart = GetServerStartTime();

            long lTime;
            lTime = ((TimeStamp > 0) ? TimeStamp : (long)EB.Time.Now) * 10000000;
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }
        
        public static int GetFinishServerTime(long standard, int offset_month = 0, int offset_day = 0, int offset_hour = 0, int offset_minute = 0, int offset_secound = 0)
        {
            DateTime jionTime = ZoneTimeDiff.GetServerTime(standard);
            DateTime startTime = ZoneTimeDiff.GetServerStartTime();
            var ti = jionTime - startTime;
            DateTime finish = startTime.AddDays((int)ti.TotalDays);

            if (offset_month > 0) finish = finish.AddMonths(offset_month);
            if (offset_day > 0) finish = finish.AddDays(offset_day);
            if (offset_hour > 0) finish = finish.AddHours(offset_hour);
            if (offset_minute > 0) finish = finish.AddMinutes(offset_minute);
            if (offset_secound > 0) finish = finish.AddSeconds(offset_secound);

          
            long timeFinal = (long)(finish - startTime).TotalSeconds;
            return (int)timeFinal - GetTimeZone() * 3600;
        }

        /// <summary>
        /// 获得服务器相对标准时间开始的时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetServerStartTime()
        {
            int timeZone = GetTimeZone();
            DateTime dtStart;
            if (timeZone >= 0)
            {
                dtStart = new DateTime(1970, 1, 1, timeZone, 0, 0);
            }
            else
            {
                dtStart = new DateTime(1969, 12, 31, 24 + timeZone, 0, 0);
            }
            return dtStart;
        }
    }
}