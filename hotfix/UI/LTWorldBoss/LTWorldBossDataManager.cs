using EB.Sparx;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWorldBossDataManager : ManagerUnit
    {
        public List<int> BossIdList = new List<int>() { 10031, 10021, 10011 };

        public static LTWorldBossDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTWorldBossDataManager>();
                }
                return instance;
            }
        }

        private static LTWorldBossDataManager instance;

        public const int ActivityId = 9003;

        private int start;
        private int end;
        private ArrayList mTimeCondition;
        private int needLevel;

        public bool IsHaveWorldBossRoll;
        public bool IsOpenDebugWorldBoss;//unity编辑器下才会使用的字段

        private readonly string worldBossStartName = "world_boss_start";
        private readonly string worldBossStopName = "world_boss_stop";

        public override void Initialize(Config config)
        {

        }

        public override void OnLoggedIn()
        {
            InitTime();
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        private void InitTime()
        {
            start = 0;
            end = 0;

            Hotfix_LT.Data.CronJobs startTbl = Hotfix_LT.Data.EventTemplateManager.Instance.GetCronJobsByName(worldBossStartName);
            Hotfix_LT.Data.CronJobs stopTbl = Hotfix_LT.Data.EventTemplateManager.Instance.GetCronJobsByName(worldBossStopName);

            if (startTbl == null || stopTbl == null)
            {
                return;
            }

            EB.Debug.Log(string.Format("**LTWorldBossDataManager**  startTbl::{0}", startTbl.interval));
            EB.Debug.Log(string.Format("**LTWorldBossDataManager**  stopTbl::{0}", stopTbl.interval));

            string open_time = GetWorldBossOpenWeekTime(startTbl.interval);
            int curweek = EB.Time.LocalWeek;

            if (open_time.Contains(curweek.ToString()))
            {
                Hotfix_LT.Data.ActivityTime startAct = GetWorldBossTime(startTbl.interval);
                Hotfix_LT.Data.ActivityTime endAct = GetWorldBossTime(stopTbl.interval);
                start = Hotfix_LT.Data.ZoneTimeDiff.GetTime(startAct);
                end = Hotfix_LT.Data.ZoneTimeDiff.GetTime(endAct);
            }
        }

        private string GetWorldBossOpenWeekTime(string interval)
        {
            string str = "0,1,2,3,4,5,6";
            string[] inteStrs = interval.Split(' ');

            if (inteStrs.Length >= 6 && inteStrs[5].CompareTo("*") != 0)
            {
                str = inteStrs[5];
            }

            return str;
        }

        private Hotfix_LT.Data.ActivityTime GetWorldBossTime(string interval)
        {
            Hotfix_LT.Data.ActivityTime act = new Hotfix_LT.Data.ActivityTime();
            string[] inteStrs = interval.Split(' ');

            if (inteStrs.Length < 3)
            {
                EB.Debug.LogError("GetWorldBossTime is Error, inteStrs.Length < 3");
                return act;
            }

            act.hour = int.Parse(inteStrs[2]);
            act.minute = int.Parse(inteStrs[1]);
            act.second = int.Parse(inteStrs[0]);

            return act;
        }

        private void ParseStartTime(string time)
        {
            object ob = EB.JSON.Parse(time);

            if (ob == null || !(ob is ArrayList))
            {
                EB.Debug.LogWarning("Time Condition is illegal{0}", time);
                return;
            }

            mTimeCondition = ob as ArrayList;
        }

        public bool IsWorldBossStart()
        {
            if (IsOpenDebugWorldBoss)
            {
                //unity编辑器下才会使用的逻辑
                return true;
            }

            if (EB.Time.Now >= start && EB.Time.Now < end && Hotfix_LT.Data.EventTemplateManager.Instance.GetRealmIsOpen(worldBossStartName))
            {
                return true;
            }

            EB.Debug.Log(string.Format("**LTWorldBossDataManager**  start::{0}", start));
            EB.Debug.Log(string.Format("**LTWorldBossDataManager**  end::{0}", end));
            EB.Debug.Log(string.Format("**LTWorldBossDataManager**  EB.Time.Now::{0}", EB.Time.Now));
            return false;
        }

        public void InitBossMainLandData()
        {
            EB.Debug.LogError("InitBossMainLandData   InitBossMainLandData   InitBossMainLandData");
            string str = "Layout50103";

            if (!DataLookupsCache.Instance.SearchDataByID<string>("mainlands.npc_list.EnemySpawns_11.bossLayoutId", out str))
            {
                EB.Debug.LogError("mainlands   EnemySpawns_11   bossLayoutId");
                DataLookupsCache.Instance.CacheData("mainlands.npc_list.EnemySpawns_11.bossLayoutId", str);
            }
        }

        public int GetBossEndTime()
        {
            return end;
        }

        public bool IsLive()
        {
            float left = 0;
            DataLookupsCache.Instance.SearchDataByID("world_boss.blood.l", out left);
            return left > 0;
        }

        public float GetMaxBossHp()
        {
            float left = 0;
            DataLookupsCache.Instance.SearchDataByID("world_boss.blood.m", out left);
            return left;
        }

        public string GetCurLayout()
        {
            string layout = string.Empty;
            DataLookupsCache.Instance.SearchDataByID("mainlands.npc_list.EnemySpawns_11.bossLayoutId", out layout);
            return layout;
        }

        public float GetRollStageValue()
        {
            long l = 1;
            DataLookupsCache.Instance.SearchDataByID<long>("world_boss.blood.l", out l);
            long m = 1;
            DataLookupsCache.Instance.SearchDataByID<long>("world_boss.blood.m", out m);
            float v = (float)l / (float)m;
            return v;
        }

        /// <summary>
        /// 获取世界boss的怪物id
        /// </summary>
        /// <returns></returns>
        public int GetWorldBossMonsterID()
        {
            string layout = GetCurLayout();

            if (string.IsNullOrEmpty(layout))
            {
                EB.Debug.Log("LTWorldBossDataManager GetCurBossInpId, CurBoss is Null");

                DataLookupsCache.Instance.SearchDataByID("mainlands.lastWeekBossLayoutId", out layout);

                if (string.IsNullOrEmpty(layout))
                {
                    return 0;
                }
            }

            string monsterIdStr = layout.Remove(0, 6); // 把前面的Layout截取，留下id
            int monsterId = 0;

            if (!int.TryParse(monsterIdStr, out monsterId))
            {
                EB.Debug.LogError("LTWorldBossDataManager GetCurBossInpId, Layout is Error Layout = {0}, monsterIdStr = {1}", layout, monsterIdStr);
                return 0;
            }

            return monsterId;
        }

        public static bool IsOpenWorldBossFromILR()
        {
            if (Instance != null)
            {
                return Instance.IsOpenWorldBoss();
            }

            return false;
        }

        /// <summary>
        /// 世界boss是否开启
        /// </summary>
        /// <returns></returns>
        public bool IsOpenWorldBoss()
        {
            return IsLive() && IsWorldBossStart();
        }

        /// <summary>
        /// 获取世界boss属性。（注 1：风，2：水，3：火）
        /// </summary>
        /// <returns></returns>
        public Hotfix_LT.Data.eRoleAttr GetWorldBossIndex()
        {
            Hotfix_LT.Data.eRoleAttr attr = Hotfix_LT.Data.eRoleAttr.None;
            int monsterId = GetWorldBossMonsterID();

            if (monsterId > 0)
            {
                int index = monsterId % 10;
                attr = (Hotfix_LT.Data.eRoleAttr)index;
            }
            else
            {
                EB.Debug.Log("LTWorldBossDataManager GetWorldBossIndex monsterId : {0}", monsterId);
            }

            return attr;
        }

        /// <summary>
        /// 是否播放世界boss的特殊出场镜头
        /// </summary>
        /// <returns></returns>
        public bool IsPlayWorldBossSpecialCam(bool isOnlyRead = false)
        {
            bool isPlay = false;
            string dateStr = string.Format("WorldBossSpecialCam_{0}_{1}", LoginManager.Instance.LocalUserId.Value, end);
            string flag = UnityEngine.PlayerPrefs.GetString(dateStr);

            if (string.IsNullOrEmpty(flag))
            {
                isPlay = true;

                if (!isOnlyRead)
                {
                    UnityEngine.PlayerPrefs.SetString(dateStr, "TRUE");
                }
            }

            return isPlay;
        }
    }
}
