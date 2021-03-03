using EB.Sparx;
using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTClimingTowerManager : ManagerUnit
    {
        public SubSystemState State {get;set;}
        private static LTClimingTowerManager instance;
        public ClimingTowerInfo v_CurrentLayerData;
        public static LTClimingTowerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTClimingTowerManager>();
                }
                return instance;
            }
        }
    
        public bool CanUpTeam(int uid)
        {
            bool canUpTeam = true;
            for (int i=0;i< v_CurrentLayerData.v_SleepHero.Length;i++)
            {
                if (v_CurrentLayerData.v_SleepHero[i] == uid)
                {
                    canUpTeam = false;
                    break;
                }
            }
            return canUpTeam;
        }
        //public int LoginTimeDate;//登陆时data时间戳，用于跨点刷新
        public override void Connect()
        {
            State = SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {
            State = SubSystemState.Disconnected;
        }

        public override void Initialize(Config config)
        {
        
        }

        /// <summary>
        /// 判断睡梦之塔的红点
        /// </summary>
        /// <returns></returns>
        public void Expedition_SleepTower()
        {
            Hashtable data;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userSleepTower", out data);
            FuncTemplate m_FuncTpl =FuncTemplateManager.Instance.GetFunc(10085);
            if(data == null || !m_FuncTpl.IsConditionOK()){
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst .climingtower ,0);
                return;
            }
            ParstData(data);
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="data">数据</param>
        private void ParstData(Hashtable data)
        {
            ClimingTowerInfo currentLayerInfo = new ClimingTowerInfo();
            currentLayerInfo.v_CurrentLayer = int.MaxValue;
            //
            currentLayerInfo.v_SleepHero = EBCore.Dot.Array<int>("ban_list", data, currentLayerInfo.v_SleepHero,
                delegate(object val)
                {
                    return int.Parse(val.ToString());
                });
            currentLayerInfo.last_reset_day = EB.Dot.Integer("last_reset_day", data, 0);
            currentLayerInfo.last_reset_ts = EB.Dot.Integer("last_reset_ts", data, 0);
            currentLayerInfo.recordPoint =  EB.Dot.Integer("totalScore", data, 0);
            //获取活动天数
            int activityDay = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("SleepTower");
            currentLayerInfo.v_ResetTime = currentLayerInfo.last_reset_ts + activityDay * 24 * 60 * 60;
            currentLayerInfo.v_TodayResetTime = EB.Dot.Integer("last_ban_ts", data, 0) + 24 * 60 * 60;
            currentLayerInfo.v_TodayResetTime = EB.Dot.Integer("last_ban_ts", data, 0) + 24 * 60 * 60;

            Hashtable levelData = EB.Dot.Object("init_data", data, null);
            currentLayerInfo.v_Level = EB.Dot.Integer("level", levelData, 30);
            //
            data = EB.Dot.Object("tower", data, null);
            foreach (DictionaryEntry entry in data)
            {
                ClimingTowerTemplate template = new ClimingTowerTemplate();
                var floor = int.Parse(entry.Key.ToString());
                template.layer = floor;
                bool finish = EB.Dot.Bool("finish", entry.Value, false);
                bool diffculty_finish = EB.Dot.Bool("diffculty_finish", entry.Value, false);
                finish = finish || diffculty_finish;
               
                if (currentLayerInfo.v_CurrentLayer > floor && !finish)
                {
                    currentLayerInfo.v_CurrentLayer = floor;
                    currentLayerInfo.v_Layout = EB.Dot.String("layout", entry.Value, "");
                }
            }

            int sleepRedPoint = 0;
            string key = LoginManager.Instance.LocalUserId.Value + "SleepTowerHud" + EB.Time.LocalMonth+ EB.Time.LocalDay;
            int value = PlayerPrefs.GetInt(key);
            if (value==0 && currentLayerInfo.v_CurrentLayer<data.Count)
            {
                sleepRedPoint++;
            }

            sleepRedPoint = GetRewardRedPoint() ? sleepRedPoint + 1 : sleepRedPoint;
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.climingtower, sleepRedPoint);
        }

        public int GetCurrentRecord()
        {
            int point;
            DataLookupsCache.Instance.SearchDataByID("userSleepTower.totalScore", out point);
            return point;
        }
        
        public int GetMaxRecord()
        {
            return EventTemplateManager.Instance.GetSleepRewardList().Last().Record;
        }
        
        public bool GetRewardRedPoint()
        {
            int cur = GetCurrentRecord();
            List<ClimingTowerRewardTemplate> sleep = EventTemplateManager.Instance.GetSleepRewardList();
            for (int i = 0; i < sleep.Count; i++)
            {
                if (cur >= sleep[i].Record && !isGetReward(sleep[i].id))
                {
                   
                    return true;
                }
            }
            return false;
        }

        public int GetCurrentPosition()
        {
            int position = 0;
            int cur = GetCurrentRecord();
            List<ClimingTowerRewardTemplate> sleep = EventTemplateManager.Instance.GetSleepRewardList();
            for (int i = 0; i < sleep.Count; i++)
            {
                if (cur >= sleep[i].Record)
                {
                    position = i;
                    if (!isGetReward(sleep[i].id)) return position;
                }
            }

            return position;
        }

        public bool isGetReward(int id)
        {
            int isGet;
            DataLookupsCache.Instance.SearchDataByID($"userSleepTower.rewards_status.{id}", out isGet);
            return isGet == 1;
        }


    }

}