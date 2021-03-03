using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 爬塔数据信息
    /// </summary>
    public class ClimingTowerInfo
    {
        /// <summary>
        /// 上个天
        /// </summary>
        public int last_reset_day;
        /// <summary>
        /// 时间
        /// </summary>
        public long last_reset_ts;
        /// <summary>
        /// 今天要刷新的时间戳
        /// </summary>
        public long v_TodayResetTime;
        /// <summary>
        /// 怪物阵容
        /// </summary>
        public string v_Layout;
        /// <summary>
        /// 重置的时间
        /// </summary>
        public long v_ResetTime;
        /// <summary>
        /// 当前层
        /// </summary>
        public int v_CurrentLayer;
        /// <summary>
        /// 沉睡的伙伴列表
        /// </summary>
        public int[] v_SleepHero;
        /// <summary>
        /// 当前层
        /// </summary>
        public int v_Level;

        public int recordPoint;
    }

    /// <summary>
    /// 爬塔副本UI界面
    /// 对应挂载的预置体：LTClimbingTowerHud
    /// </summary>
    public class LTClimingTowerHudController : UIControllerHotfix
    {
        public UILabel v_ResetTime; 
        public UILabel recordLabel;
        public LTClimingTowerLayer[] v_StyleLayer;
        public UIGrid v_Grid;
        public GameObject v_RewardBtn;
        private Coroutine m_ResetTimeCoroutine;
        public ClimingTowerInfo v_CurrentLayerData;
        public GameObject v_RewardRed;
        public GameObject v_RuleBtn;
        public TweenPosition v_TweenPosition;
        public Transform StartPos, EndPos, EndPos2;
        private List<ClimingTowerTemplate> m_ServerLayerData;
        private static LTClimingTowerHudController m_Instance;
        public static LTClimingTowerHudController Instance
        {
            get
            {
                return m_Instance;
            }
        }
        public LTClimingTowerAPI v_Api
        {
            get; private set;
        }
        
        public override bool IsFullscreen() {
            return true;
        }
        
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            v_ResetTime = t.GetComponent<UILabel>("UINormalFrameBG/Top/Label");
            recordLabel = t.GetComponent<UILabel>("UINormalFrameBG/Top/Record");
            v_Grid = t.GetComponent<UIGrid>("ContentViewRoot/ContentView/TowerPanel/Root/Grid");
            v_RewardBtn = t.FindEx("UINormalFrameBG/TopRight/Reward").gameObject;
            v_RewardRed = t.FindEx("UINormalFrameBG/TopRight/Reward/Red").gameObject;
            v_RuleBtn = t.FindEx("UINormalFrameBG/Top/RuleBtn").gameObject;
            v_TweenPosition = t.GetComponent<TweenPosition>("ContentViewRoot/ContentView");
            StartPos = t.GetComponent<Transform>("ContentViewRoot/StartPoint");
            EndPos = t.GetComponent<Transform>("ContentViewRoot/EndPoint");
            EndPos2 = t.GetComponent<Transform>("ContentViewRoot/EndPoint2");
            t.GetComponent<UIButton>("UINormalFrameBG/TopLeft/CancelBtn").onClick
                .Add(new EventDelegate(OnCancelButtonClick));
            v_StyleLayer = new LTClimingTowerLayer[4];
            v_StyleLayer[0] = t.GetMonoILRComponent<LTClimingTowerLayer>("ContentViewRoot/ContentView/TowerPanel/Root/Grid/0");
            v_StyleLayer[1] = t.GetMonoILRComponent<LTClimingTowerLayer>("ContentViewRoot/ContentView/TowerPanel/Root/Grid/1");
            v_StyleLayer[2] = t.GetMonoILRComponent<LTClimingTowerLayer>("ContentViewRoot/ContentView/TowerPanel/Root/Grid/2");
            v_StyleLayer[3] = t.GetMonoILRComponent<LTClimingTowerLayer>("ContentViewRoot/ContentView/TowerPanel/Root/Grid/3");
            m_Instance = this;
            v_TweenPosition.from = StartPos.localPosition;
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.climingtower, ClimingTowerRP);
            
            string key = LoginManager.Instance.LocalUserId.Value + "SleepTowerHud" + EB.Time.LocalMonth+ EB.Time.LocalDay;
            PlayerPrefs.SetInt(key, 1);
        }

        private void ClimingTowerRP(RedPointNode node)
        {
            v_RewardRed.CustomSetActive(node.num > 0);
        }
    
        public override void OnDestroy()
        {
            base.OnDestroy();
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.climingtower, ClimingTowerRP);
            m_Instance = null;
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            controller.gameObject.SetActive(true);
            UIEventListener.Get(v_RewardBtn).onClick = OnClickReward;
            UIEventListener.Get(v_RuleBtn).onClick = OnClickRule;
            
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.sleep_topic,
                FusionTelemetry.GamePlayData.sleep_event_id,FusionTelemetry.GamePlayData.sleep_umengId,"open");
        }
    
        public override IEnumerator OnAddToStack()
        {
            GlobalMenuManager.Instance.PushCache("LTClimbingTowerHud");
            v_TweenPosition.transform.localPosition = StartPos.localPosition;
            yield return base.OnAddToStack();
            //判断是否刷新
            UpdatePanel(true);
        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            base.OnRemoveFromStack();
            DestroySelf();
            yield break;
        }
    
        public override void OnCancelButtonClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTClimbingTowerHud");
            base.OnCancelButtonClick();
        }
        
        public void UpdatePanel(bool requestNet)
        {
            //
            v_Api = new LTClimingTowerAPI();
            KeyValuePair<ClimingTowerInfo, List<Hotfix_LT.Data.ClimingTowerTemplate>> keyValue = new KeyValuePair<ClimingTowerInfo, List<Hotfix_LT.Data.ClimingTowerTemplate>>();
            if (requestNet)
            {
                v_Api.RequestClimingTowerData((Hashtable hashtable) =>
                {
                    if (hashtable != null)
                    {
                        //重置存的奖励状态 层数
                        DataLookupsCache.Instance.CacheData("userSleepTower",null);
                        DataLookupsCache.Instance.CacheData(hashtable);
                        Hashtable data = EB.Dot.Object("userSleepTower", hashtable, null);
                        keyValue = ParstData(data);
                        SetPanel(keyValue.Key, keyValue.Value);
                    }
                });
            }
            else
            {
                //
                Hashtable data = Johny.HashtablePool.Claim();
                DataLookupsCache.Instance.SearchDataByID<Hashtable>("userSleepTower", out data);
                keyValue = ParstData(data);
                SetPanel(keyValue.Key, keyValue.Value);
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.climingtower,
                LTClimingTowerManager.Instance.GetRewardRedPoint() ? 1 : 0);
        }
    
        private KeyValuePair<ClimingTowerInfo,List<Hotfix_LT.Data.ClimingTowerTemplate>> ParstData(Hashtable data)
        {
            if (data == null)
            {
                return new KeyValuePair<ClimingTowerInfo, List<Hotfix_LT.Data.ClimingTowerTemplate>>(null, null);
            }
            //
            ClimingTowerInfo currentLayerInfo = new ClimingTowerInfo();
            currentLayerInfo.v_CurrentLayer = int.MaxValue;
            //
            currentLayerInfo.v_SleepHero = Hotfix_LT.EBCore.Dot.Array<int>("ban_list", data, currentLayerInfo.v_SleepHero,
                delegate(object val)
                {
                    return int.Parse(val.ToString());
                });
            currentLayerInfo.last_reset_day = EB.Dot.Integer("last_reset_day", data, 0);
            currentLayerInfo.last_reset_ts = EB.Dot.Integer("last_reset_ts", data, 0);
            currentLayerInfo.recordPoint= EB.Dot.Integer("totalScore", data, 0);
            //获取活动天数
            int activityDay = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("SleepTower");
            currentLayerInfo.v_ResetTime = currentLayerInfo.last_reset_ts + activityDay * 24 * 60 * 60;
            currentLayerInfo.v_TodayResetTime = EB.Dot.Integer("last_ban_ts", data, 0) + 24 * 60 * 60;
            currentLayerInfo.v_TodayResetTime = EB.Dot.Integer("last_ban_ts", data, 0) + 24 * 60 * 60;
           
            recordLabel.text = EB.Localizer.GetString("ID_SLEEPTOWER_RECORD")+currentLayerInfo.recordPoint;
            Hashtable levelData = EB.Dot.Object("init_data", data, null);
            currentLayerInfo.v_Level = EB.Dot.Integer("level", levelData, 30);
            //
            data = EB.Dot.Object("tower", data, null);
            List<Hotfix_LT.Data.ClimingTowerTemplate> allLayerData = new List<Hotfix_LT.Data.ClimingTowerTemplate>();
            foreach (DictionaryEntry entry in data)
            {
                Hotfix_LT.Data.ClimingTowerTemplate template = new Hotfix_LT.Data.ClimingTowerTemplate();
    
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
                allLayerData.Add(template);
            }
            return new KeyValuePair<ClimingTowerInfo, List<Hotfix_LT.Data.ClimingTowerTemplate>>(currentLayerInfo, allLayerData);
        }
    
        private void SetPanel(ClimingTowerInfo data, List<Hotfix_LT.Data.ClimingTowerTemplate> climingTowerTemplates)
        {
            v_CurrentLayerData = data;
            LTClimingTowerManager.Instance.v_CurrentLayerData = v_CurrentLayerData;
            m_ServerLayerData = climingTowerTemplates;
            if (data == null)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleMatchHudController_10973"));
                return;
            }
            //创建塔层,说明已经都打通关了
            SetLayer(data, data.v_CurrentLayer == int.MaxValue);
            //设置倒计时间
            if (m_ResetTimeCoroutine != null)
            {
                StopCoroutine(m_ResetTimeCoroutine);
                m_ResetTimeCoroutine = null;
            }
            if(controller.gameObject.activeSelf) m_ResetTimeCoroutine = StartCoroutine(SetResetTime(data.v_ResetTime,data.v_TodayResetTime));
        }

        private void SetLayer(ClimingTowerInfo data, bool isAllPass)
        {
            if (v_StyleLayer != null)
            {
                for (int i = 0; i < v_StyleLayer.Length; i++)
                {
                    v_StyleLayer[i].v_HaveSetData = false;
                }
            }

            //拿塔的数据,拿4层就可以了
            int total = 4;
            List<Data.ClimingTowerTemplate> datas = Data.EventTemplateManager.Instance.GetClimingTowerData(data.v_Level, isAllPass ? 20 : data.v_CurrentLayer, total);

            if (datas != null && datas.Count > 0)
            {
                Data.ClimingTowerTemplate layerData = null;
                LTClimingTowerLayer layerController = null;
                int result = (datas[0].layer % 2 == 0) ? 0 : 1;

                for (int i = 0; i < total; i++)
                {
                    //层的数据
                    if (datas.Count > i)
                    {
                        layerData = datas[i];
                        SetLayerState(ref layerData);
                    }
                    else
                    {
                        layerData = null;
                    }

                    //设置层的界面
                    layerController = v_StyleLayer[i];

                    if (layerController != null )
                    {
                        layerController.mDMono.gameObject.name = (total - i).ToString();
                        if(layerData != null) layerController.F_SetData(v_Api.RequestChallengeLayer, isAllPass ? null : layerData,layerData.layer);
                    }
                }
            }

            if (v_Grid != null)
            {
                v_Grid.Reposition();
            }

            if (v_TweenPosition != null)
            {
                v_TweenPosition.to = (data != null && data.v_CurrentLayer == 1) ? EndPos.localPosition : EndPos2.localPosition;

                var tweenAlpha = v_TweenPosition.GetComponent<TweenAlpha>();

                if (tweenAlpha != null)
                {
                    tweenAlpha.ResetToBeginning();
                    tweenAlpha.PlayForward();
                }

                v_TweenPosition.ResetToBeginning();
                v_TweenPosition.PlayForward();
            }
        }
    
        private IEnumerator SetResetTime(long resetTime,long todayResetTime)
        {
            while (true)
            {
                System.TimeSpan ts = EB.Time.FromPosixTime(resetTime) - EB.Time.FromPosixTime(EB.Time.Now);
                System.TimeSpan todayResetTs = EB.Time.FromPosixTime(todayResetTime) - EB.Time.FromPosixTime(EB.Time.Now);
                int days = ts.Days;
                int hours = ts.Hours;
                int minutes = ts.Minutes;
                int seconds = ts.Seconds;
                string time = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                string timeS = string.Format(EB.Localizer .GetString("ID_DAY_FORMAT"),  days, time);
    
                string context = EB.Localizer.Format("ID_RESET_CLIMINGTOWER_TIME",string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, timeS ));
                LTUIUtil.SetText(v_ResetTime, context);
                if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
                {
                    this.UpdatePanel(true);
                    break;
                }
                if (todayResetTs.Days == 0 && todayResetTs.Hours == 0 && todayResetTs.Minutes == 0 && todayResetTs.Seconds == 0)
                {
                    this.UpdatePanel(true);
                    break;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void OnClickReward(GameObject btn)
        {
            if (v_CurrentLayerData == null)
            {
                return;
            }
            GlobalMenuManager.Instance.Open("LTClimbingTowerRewardHud", null);
        }
    

        private void OnClickRule(GameObject btn)
        {
            string text = EB.Localizer.GetString("ID_SLEEPTOWER_RULE_TEXT");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }
    
        private void SetLayerState(ref Data.ClimingTowerTemplate data)
        {
            int layer = data.layer;
            data.v_CanChallenge = v_CurrentLayerData.v_CurrentLayer == layer;
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
    }
}
