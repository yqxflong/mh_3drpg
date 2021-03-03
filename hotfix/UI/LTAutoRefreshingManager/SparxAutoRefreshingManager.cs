using UnityEngine;
using System.Collections;
using EB.Sparx;

namespace Hotfix_LT .UI
{
    public class AutoRefreshingManager : ManagerUnit, IManagerUnitUpdatable
    {
        public enum RefreshName {
            LoginActivity,
            VigorUpdate,
            Signin,
            SpecialActivity,
            AwakenStone,
            HeroBattleRefresh,
            Promotion
        }

        // private static AutoRefreshingManager sInstance = null;

        public static AutoRefreshingManager Instance
        {
            get { return LTHotfixManager.GetManager<AutoRefreshingManager>(); }
        }

        //定时执行
        private CronRefreshManager m_cronRefreshManager = new CronRefreshManager(15);
        //自动增加(类似用于体力恢复)
        private DeltaTimeRefresherManager m_deltaTimeRefresherManager = new DeltaTimeRefresherManager(1);
		private DeltaActionManager m_deltaActionManager = new DeltaActionManager(1);

        //刷新事件相关
		private TimeRefresherManager m_timeRefresherManager = new TimeRefresherManager();

        //游戏活动开启结束跑马灯相关
        private ActivityRollingMsgManager m_activityRollingMsgManager = new ActivityRollingMsgManager(5);

        public override void Initialize(Config config)
        {
            m_cronRefreshManager.InitRefresher(null);
            m_deltaTimeRefresherManager.InitRefresher(null);
			m_deltaActionManager.InitRefresher(null);
        }

		public override void OnLoggedIn()
		{
			base.OnLoggedIn();

			m_cronRefreshManager.InitRefresher(null);
			AddTaskRefreshExcuter();
            AddUserPrizeDataRefreshExcuter();

            AddRollingMsgRefresh();
            AddVigorRefresh();

        }

		public override void Connect()
        {
            InitRefresh();
            m_cronRefreshManager.InitRefresher(null);
			m_deltaTimeRefresherManager.InitRefresher(null);
            m_activityRollingMsgManager.InitRefresher(null);
            var now= Data.ZoneTimeDiff.GetServerTime();
            AddRefresh(RefreshName.LoginActivity, now, 0, 0);
            AddRefresh(RefreshName.VigorUpdate, now, 0, 0);
            AddRefresh(RefreshName.Signin, now, 0, 0);
            AddRefresh(RefreshName.SpecialActivity, now, 0, 0);
            AddRefresh(RefreshName.AwakenStone, now, 0, 0);
            Hotfix_LT.Data.CronJobs cj = Hotfix_LT.Data.EventTemplateManager.Instance.GetCronJobsByName("clashofheroestimes");
            string[] strs = cj.interval.Split(' ');
            int minute = 0;
            int hour = 5;
            if (strs.Length != 6)
            {
                EB.Debug.LogError("cronTable Str Length!=6");
            }
            else
            {
                minute = int.Parse(strs[1]);
                hour = int.Parse(strs[2]);
            }
            AddRefresh(RefreshName.HeroBattleRefresh, now, hour, minute);
            AddRefresh(RefreshName.Promotion, now, 0, 0);
        }

        public override void Disconnect(bool isLogout)
        {
            isInitWorldBossRoll = false;
            m_cronRefreshManager.End();
            m_deltaTimeRefresherManager.End();
            EndRefresh();
            m_activityRollingMsgManager.End();
            if (isLogout)
            {
                m_deltaActionManager.End();
            }
        }

        public override void Async(string message, object payload)
        {

        }

        public void InitRefresh()
        {
            m_timeRefresherManager.InitRefresher();
        }

        public void AddRefresh(RefreshName refreshName, System.DateTime beginTime, int refreshHour, int refreshMinute)
        {
            m_timeRefresherManager.AddRefreshExcuter(new TimeRefresher(refreshName.ToString(), beginTime, refreshHour, refreshMinute));
        }

        public bool GetRefreshed(RefreshName refreshName)
        {
            return m_timeRefresherManager.GetRefreshed(refreshName.ToString());
        }

        public System.DateTime GetRefreshNextTime(string name)
        {
            return m_timeRefresherManager.GetRefreshNextTime(name);
        }

        public void EndRefresh()
        {
            m_timeRefresherManager.End();
        }

        // public bool UpdateOffline { get { return false; } }

        public void Update()
        {
            m_cronRefreshManager.Update();
            m_deltaTimeRefresherManager.Update();
			m_deltaActionManager.Update();
            m_activityRollingMsgManager.Update();
        }

        public void AddDeltaRefresher<T>(IDictionary rule) where T : DeltaTimeRefresher, new()
        {
            m_deltaTimeRefresherManager.AddRefresher<T>(rule);
        }

        public void RemoveDeltaRefresher<T>() where T : DeltaTimeRefresher
        {
            m_deltaTimeRefresherManager.RemoveRefresher<T>();
        }

        public T GetDeltaRefresher<T>() where T : DeltaTimeRefresher
        {
            return m_deltaTimeRefresherManager.GetRefresher<T>();
        }

        public void AddCronRefreshExcuter(string name, IDictionary rule)
        {
            CronRefreshExcuter excuter = new CronRefreshExcuter(name);
            excuter.Init(rule);
            AddCronRefreshExcuter(excuter);
        }

        public void AddCronRefreshExcuter(CronRefreshExcuter excuter)
        {
			m_cronRefreshManager.AddCronRefreshExcuter(excuter);
        }

        public void RemoveCronRefreshExcuter(string name)
        {
            m_cronRefreshManager.RemoveCronRefreshExcuter(name);
        }

		public void AddCountdownExcuter(string name)
		{

		}
		public void AddDeltaActionExcute(DeltaActionExcuter excuter)
		{
			m_deltaActionManager.AddAction(excuter);
        }

		public void RemoveDeltaActionExcute(string id)
		{
			m_deltaActionManager.RemoveAction(id);
		}

		public CronRefreshExcuter GetCronRefreshExcuter(string name)
		{
			return m_cronRefreshManager.GetCronRefreshExcuter(name);
		}

		void AddTaskRefreshExcuter()
		{
			string taskRefreshTime;
			DataLookupsCache.Instance.SearchDataByID<string>("userTaskStatus.refreshTaskTime", out taskRefreshTime);
			Hashtable rule = Johny.HashtablePool.Claim();
			rule.Add("regular", TimerScheduler.AmendCronFormat(taskRefreshTime));
			rule.Add("url", "/mhjtasks/refreshTaskState");
			CronRefreshExcuter excuter = new CronRefreshExcuter("refreshTaskState");
			excuter.Init(rule, delegate (Hashtable hash) {
                string npcLocator;
				DataLookupsCache.Instance.SearchDataByID<string>(string.Format("tasks.{0}.event_count.locator", LTBountyTaskHudController.TaskID()), out npcLocator);
				LTBountyTaskHudController.DeleteMonster(npcLocator);
                DataLookupsCache.Instance.CacheData("user_prize_data.taskliveness_reward", string.Empty);//重置任务完成情况
                DataLookupsCache.Instance.CacheData("user_prize_data.taskliveness.curr", 0);
                DataLookupsCache.Instance.CacheData("user_prize_data.taskliveness_week_reward", string.Empty);//重置任务完成情况
                DataLookupsCache.Instance.CacheData("user_prize_data.taskweekliveness.curr", 0);
                DataLookupsCache.Instance.CacheData("task_refresh", 1);
            });
			AddCronRefreshExcuter(excuter);
		}
        void AddUserPrizeDataRefreshExcuter()
        {
            string taskRefreshTime= "0 0 0 * * *";
            //DataLookupsCache.Instance.SearchDataByID<string>("userTaskStatus.refreshTaskTime", out taskRefreshTime);
            Hashtable rule = Johny.HashtablePool.Claim();
            rule.Add("regular", TimerScheduler.AmendCronFormat(taskRefreshTime));
            rule.Add("url", "/sign_in/getUserPrizeData");
            CronRefreshExcuter excuter = new CronRefreshExcuter("refreshUserPrizeData",false);
            excuter.Init(rule, delegate (Hashtable hash) {
                if (hash != null)
                {
                    DataLookupsCache.Instance.CacheData(hash);
                    GameDataSparxManager.Instance.ProcessIncomingData(hash, false);
                }
            });
            AddCronRefreshExcuter(excuter);
        }

        void AddVigorRefresh()
        {
            RemoveDeltaRefresher<VigorDeltaTimeRefresher>();
            var ht = Johny.HashtablePool.Claim();
            ht.Add("data_path", "res.vigor.v");
            ht.Add("time_path", "res.vigor.nextGrowthTime");
            ht.Add("min", 0 );
            ht.Add("max", BalanceResourceUtil.GetUserVigorMax());
            ht.Add("delta", VigorDeltaTimeRefresher.GetVigorFreshDeltaTime());
            ht.Add("offset", 1);
            AddDeltaRefresher<VigorDeltaTimeRefresher>(ht);
        }

        void AddRollingMsgRefresh()
        {
            //国战开始
            /*m_activityRollingMsgManager.AddRollingMsgActivity(902290, "nationswarstartstage1");
            m_activityRollingMsgManager.AddRollingMsgActivity(902291, "nationswarstartstage2");
            m_activityRollingMsgManager.AddRollingMsgActivity(902292, "nationswarstartstage3");*/

            //天梯
            m_activityRollingMsgManager.AddRollingMsgActivity(902302, "ladder_start");
            m_activityRollingMsgManager.AddRollingMsgActivity(902303, "ladder_stop");

            //军团战开始
            m_activityRollingMsgManager.AddRollingMsgActivity(902284, "alliancewarprestart");
            m_activityRollingMsgManager.AddRollingMsgActivity(902304, "alliancewarsemistart");
            m_activityRollingMsgManager.AddRollingMsgActivity(902305, "alliancewarfinalstart");
            if (!LTLegionWarManager.Instance.IsOpenWarTime())
            {
                m_activityRollingMsgManager.MoveNextRollingMsgActivity(902284, LTLegionWarManager.Instance.OpenWarTime());
                m_activityRollingMsgManager.MoveNextRollingMsgActivity(902304, LTLegionWarManager.Instance.OpenWarTime());
                m_activityRollingMsgManager.MoveNextRollingMsgActivity(902305, LTLegionWarManager.Instance.OpenWarTime());
            }
        }

        private bool isInitWorldBossRoll;

        public void AddWorldBossRollingMsg()
        {
            if (isInitWorldBossRoll)
            {
                return;
            }

            //世界boss开始
            Hotfix_LT.Data.eRoleAttr attr = LTWorldBossDataManager.Instance.GetWorldBossIndex();

            if (attr == Hotfix_LT.Data.eRoleAttr.None)
            {
                return;
            }

            if (attr == Hotfix_LT.Data.eRoleAttr.Huo)
            {
                m_activityRollingMsgManager.AddRollingMsgActivity(902295, "world_boss_start");//902295,902296,902297,火风水
            }
            else if (attr == Hotfix_LT.Data.eRoleAttr.Feng)
            {
                m_activityRollingMsgManager.AddRollingMsgActivity(902296, "world_boss_start");//902295,902296,902297,火风水
            }
            else if (attr == Hotfix_LT.Data.eRoleAttr.Shui)
            {
                m_activityRollingMsgManager.AddRollingMsgActivity(902297, "world_boss_start");//902295,902296,902297,火风水
            }

            //世界boss结束
            if (attr == Hotfix_LT.Data.eRoleAttr.Huo)
            {
                m_activityRollingMsgManager.AddRollingMsgActivity(902299, "world_boss_stop");//902299,902296,902297,火风水
            }
            else if (attr == Hotfix_LT.Data.eRoleAttr.Feng)
            {
                m_activityRollingMsgManager.AddRollingMsgActivity(902300, "world_boss_stop");//902299,902296,902297,火风水
            }
            else if (attr == Hotfix_LT.Data.eRoleAttr.Shui)
            {
                m_activityRollingMsgManager.AddRollingMsgActivity(902301, "world_boss_stop");//902299,902296,902297,火风水
            }

            isInitWorldBossRoll = true;
        }
        
        public void RemoveWorldBossRollingMsg()
        {
            Hotfix_LT.Data.eRoleAttr attr = LTWorldBossDataManager.Instance.GetWorldBossIndex();
            if (attr == Hotfix_LT.Data.eRoleAttr.Huo)
            {
                RemoveRollingMsg(902299);//902299,902296,902297,火风水
            }
            else if (attr == Hotfix_LT.Data.eRoleAttr.Feng)
            {
                RemoveRollingMsg(902300);//902299,902296,902297,火风水
            }
            else if (attr == Hotfix_LT.Data.eRoleAttr.Shui)
            {
                RemoveRollingMsg(902301);//902299,902296,902297,火风水
            }
        }

        /// <summary>
        /// 移除掉某条跑马灯
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="isPlay">是否播放</param>
        public void RemoveRollingMsg(int msgId, bool isPlay = true)
        {
            m_activityRollingMsgManager.RemoveRollingMsgActivity(msgId);

            if (isPlay)
            {
                MessageTemplateManager.ShowMessage(msgId);
            }
        }

    }
}
