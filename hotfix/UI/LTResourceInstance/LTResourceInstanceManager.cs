using EB.Sparx;
using Hotfix_LT.Data;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTResourceInstanceManager : ManagerUnit
    {
        public int LTResourceInstanceTime;

        private static LTResourceInstanceManager mInstance = null;

        public static LTResourceInstanceManager Instance
        {
            get
            {
                return mInstance = mInstance ?? LTHotfixManager.GetManager<LTResourceInstanceManager>();
            }
        }

        public LTResourceInstanceApi Api
        {
            get; private set;
        }

        public override void Initialize(Config config)
        {
            Instance.Api = new LTResourceInstanceApi();
            Instance.Api.ErrorHandler += ErrorHandler;

            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, DataRefresh);
        }

        private void DataRefresh()
        {
            if (Expedition_Exp() || Expedition_Gold()) { }
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            UpdateDiffice();
        }

        private int mPassDifficeGlod;

        public int PassDifficeGlod
        {
            get
            {
                return mPassDifficeGlod;
            }
            private set
            {
                mPassDifficeGlod = value;
            }
        }
        private int mPpassDifficeExp;
        public int PassDifficeExp
        {
            get
            {
                return mPpassDifficeExp;
            }
            private set
            {
                mPpassDifficeExp = value;
            }
        }

        public void UpdateDiffice()
        {
            DataLookupsCache.Instance.SearchIntByID("special_activity.9001.currentPassDifficulty", out mPassDifficeGlod);
            DataLookupsCache.Instance.SearchIntByID("special_activity.9002.currentPassDifficulty", out mPpassDifficeExp);
        }

        public bool IsLock(Hotfix_LT.Data.SpecialActivityLevelTemplate level, ResourceInstanceType type)
        {
            if (type == ResourceInstanceType.Gold)
            {
                return level.id > mPassDifficeGlod;
            }
            else if (type == ResourceInstanceType.Exp)
            {
                return level.id > mPpassDifficeExp;
            }
            return true;
        }

        public bool IsLevelEnough(Hotfix_LT.Data.SpecialActivityLevelTemplate level)
        {
            return BalanceResourceUtil.GetUserLevel() >= level.level;
        }

        public bool IsCanBlitz(Hotfix_LT.Data.SpecialActivityLevelTemplate level, ResourceInstanceType type)
        {
            if (type == ResourceInstanceType.Gold)
            {
                return level.id <= (mPassDifficeGlod - 1);
            }
            else if (type == ResourceInstanceType.Exp)
            {
                return level.id <= (mPpassDifficeExp - 1);
            }
            return false;
        }

        private bool _startBattleRequesting = false;

        public void StartBattle(int levelId, int battleType)
        {
            if (_startBattleRequesting)
            {
                return;
            }

            _startBattleRequesting = true;

            Api.StartBattle(levelId, battleType, delegate (Hashtable data)
            {
                if (data != null)
                {
                    DataLookupsCache.Instance.CacheData(data);
                }

                _startBattleRequesting = false;
            });
        }

        public void GetResourceInstanceTime(int activityId, System.Action callback = null)
        {
            Api.GetResourceInstanceTime(activityId, delegate (Hashtable data)
            {
                if (data != null)
                {
                    int time = EB.Dot.Integer("special_activity." + activityId + ".progressbar", data, 0);
                    LTResourceInstanceTime = time;
                    DataLookupsCache.Instance.CacheData(data);
                }

                UpdateDiffice();

                if (callback != null)
                {
                    callback();
                }
            });
        }

        public void Blitz(int levelId, int battleType, System.Action callback)
        {
            Api.RequestBlitzInstance(levelId, battleType, delegate (Hashtable data)
            {
                if (data != null)
                {
                    DataLookupsCache.Instance.CacheData(data);
                }

                if (callback != null)
                {
                    callback();
                }
            });
        }

        private int GoldActivity = 9001;
        public bool Expedition_Gold()//金币副本判断
        {
            FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10020);
            if (m_FuncTpl.IsConditionOK())
            {
                SpecialActivityTemplate mActivityTbl = EventTemplateManager.Instance.GetSpecialActivity(GoldActivity);
                int times = 0;
                string path = string.Format("special_activity.{0}.c_times", GoldActivity);
                DataLookupsCache.Instance.SearchDataByID<int>(path, out times);
                bool hasTimes = mActivityTbl.times - times > 0;
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.resourcegold, hasTimes ? 1 : 0);
                return hasTimes;
            }
            else
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.resourcegold, 0);
                return false;
            }
        }

        private int ExpActivity = 9002;
        public bool Expedition_Exp()//经验副本判断
        {
            FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10021);
            if (m_FuncTpl.IsConditionOK())
            {
                SpecialActivityTemplate mActivityTbl = EventTemplateManager.Instance.GetSpecialActivity(ExpActivity);
                int times = 0;
                string path = string.Format("special_activity.{0}.c_times", ExpActivity);
                DataLookupsCache.Instance.SearchDataByID<int>(path, out times);
                bool hasTimes = mActivityTbl.times - times > 0;
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst .resourceexp,hasTimes ? 1:0);
                return hasTimes;
            }
            else
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.resourceexp,  0);
                return false;
            }
        }

    }
}