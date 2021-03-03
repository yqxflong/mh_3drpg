using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    [System.Serializable]
    public class GameChest
    {
        public RewardStageData StageData;
        public GameObject Bg;
        public GameObject Open;
        public GameObject UnOpen;
        public GameObject Light;
        public UILabel Value;
        public Vector3 OriginPos;

        public void SetUI(RewardStageData stageData)
        {
            this.StageData = stageData;
            if (Value != null)
                LTUIUtil.SetText(Value, stageData.Stage.ToString());
            UpdateReceiveState(stageData.ReceiveState);
        }

        public void UpdateReceiveState(eReceiveState rs)
        {
            StageData.ReceiveState = rs;
            switch (this.StageData.ReceiveState)
            {
                case eReceiveState.cannot:
                    {
                        UnOpen.CustomSetActive(true);
                        Open.CustomSetActive(false);
                        Light.CustomSetActive(false);
                        TweenPosition tp = UnOpen.GetComponent<TweenPosition>();
                        if (tp != null)
                        {
                            UnOpen.transform.localPosition = OriginPos;
                        }
                        UnOpen.transform.localScale = Vector3.one;
                        UITweener[] tws = UnOpen.GetComponents<UITweener>();
                        for (int i = 0; i < tws.Length; i++)
                        {
                            tws[i].enabled = false;
                        }
                    }
                    break;
                case eReceiveState.have:
                    {
                        UnOpen.CustomSetActive(true);
                        Open.CustomSetActive(true);
                        Light.CustomSetActive(false);
                        TweenPosition tp = UnOpen.GetComponent<TweenPosition>();
                        if (tp != null)
                        {
                            UnOpen.transform.localPosition = OriginPos;
                        }
                        UnOpen.transform.localScale = Vector3.one;
                        UITweener[] tws = UnOpen.GetComponents<UITweener>();
                        for (int i = 0; i < tws.Length; i++)
                        {
                            tws[i].enabled = false;
                        }
                    }
                    break;
                case eReceiveState.can:
                    {
                        UnOpen.CustomSetActive(true);
                        Open.CustomSetActive(false);
                        Light.CustomSetActive(true);
                        TweenPosition tp = UnOpen.GetComponent<TweenPosition>();
                        if (tp != null)
                        {
                            tp.from.y = OriginPos.y;
                            tp.to.y = tp.from.y + 21;
                        }
                        UITweener[] tws = UnOpen.GetComponents<UITweener>();
                        for (int i = 0; i < tws.Length; i++)
                        {
                            tws[i].ResetToBeginning();
                            tws[i].PlayForward();
                        }
                    }
                    break;
            }
        }
    }

    public class TaskAchievementLogic : DataLookupHotfix
    {
        public GameObject m_ChestRoot;
        public UILabel m_ProgressLabel;
        public UIProgressBar m_AchievementProgressBar;
        public UIServerRequest m_ReceiveChestRequest;
        public GameChest m_Chest;

        public override void Awake()
        {
            base.Awake();
            InitView();
            m_AchievementProgressBar.value = 0f;
            m_ChestRoot.SetActive(false);
            m_Chest.OriginPos = m_Chest.UnOpen.transform.localPosition;
        }

        private void InitView()
        {
            mDL.DataIDList.Add("user_prize_data.taskacm");
            m_ChestRoot = mDL.transform.Find("AchievementReward").gameObject;
            m_ProgressLabel = mDL.transform.Find("AchievementReward/ProgressBar/Label").GetComponent<UILabel>();
            m_AchievementProgressBar = mDL.transform.Find("AchievementReward/ProgressBar").GetComponent<UIProgressBar>(); 
            m_ReceiveChestRequest = mDL.transform.Find("AchievementReward/ReceiveChestReq").GetComponent<UIServerRequest>();
            m_ReceiveChestRequest.onResponse.Add(new EventDelegate(mDL, "OnFetchData"));
            UIButton HotfixBtn0 = mDL.transform.Find("AchievementReward/Box").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnChestClick));
            m_Chest = new GameChest();
            m_Chest.Open = mDL.transform.Find("AchievementReward/Box/OpenTag").gameObject;
            m_Chest.UnOpen = mDL.transform.Find("AchievementReward/Box/Close").gameObject;
            m_Chest.Light = mDL.transform.Find("AchievementReward/Box/Light").gameObject;
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);

            UpdateChest();
        }

        public bool UpdateChest()
        {
            int currAchievement = 0;
            DataLookupsCache.Instance.SearchIntByID("user_prize_data.taskacm.curr", out currAchievement);

            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(ActivityId);
            int stageCount = stages.Count;
            LTHotfixManager.GetManager<TaskManager>().CurrentIndex = GetCurrentStageIndex();
            if (LTHotfixManager.GetManager<TaskManager>().CurrentIndex <= -1 || LTHotfixManager.GetManager<TaskManager>().CurrentIndex >= stageCount)
            {
               EB.Debug.LogError("CurrentIndex <= -1 || CurrentIndex >= stageCount");
                return false;
            }
            var currentStageData = stages[LTHotfixManager.GetManager<TaskManager>().CurrentIndex];
            int maxAchievement = currentStageData.stage;

            List<LTShowItemData> itemDatas = new List<LTShowItemData>();
            for (int j = 0; j < currentStageData.reward_items.Count; ++j)
            {
                var reward = currentStageData.reward_items[j];
                string id = reward.id.ToString();
                int count = reward.quantity;
                string type = reward.type;
                LTShowItemData itemData = new LTShowItemData(id, count, type, false);
                itemDatas.Add(itemData);
            }

            bool received;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.taskacm_reward." + currentStageData.id, out received);
            eReceiveState state = eReceiveState.cannot;
            if (currAchievement >= currentStageData.stage)
            {
                if (received)
                    state = eReceiveState.have;
                else
                    state = eReceiveState.can;
            }
            m_Chest.SetUI(new RewardStageData(currentStageData.id, currentStageData.stage, itemDatas, state));

            LTUIUtil.SetText(m_ProgressLabel, string.Format("{0}/{1}", currAchievement, maxAchievement));
            m_AchievementProgressBar.value = currAchievement / (float)maxAchievement;
            m_ChestRoot.SetActive(true);

            return true;
        }

        public void OnChestClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (m_Chest.StageData.ReceiveState == eReceiveState.can)
            {
                m_Chest.UpdateReceiveState(eReceiveState.have);
                SendReceiveRewardReq(m_Chest.StageData);
                return;
            }

            string tip = "";
            if (m_Chest.StageData.ReceiveState == eReceiveState.cannot)
            {
                tip = string.Format(EB.Localizer.GetString("ID_codefont_in_TaskAchievementLogic_4553"), m_Chest.StageData.Stage);
            }
            else if (m_Chest.StageData.ReceiveState == eReceiveState.have)
            {
                tip = EB.Localizer.GetString("ID_codefont_in_LadderController_11750");
            }
            var ht = Johny.HashtablePool.Claim();
            ht.Add("data", m_Chest.StageData.Awards);
            ht.Add("tip", tip);
            GlobalMenuManager.Instance.Open("LTRewardShowUI", ht);
        }

        private void SendReceiveRewardReq(RewardStageData stageData)
        {
            m_ReceiveChestRequest.parameters[0].parameter = stageData.Id.ToString();
            m_ReceiveChestRequest.SendRequest();
            LoadingSpinner.Show();
        }

        public override void OnFetchData(EB.Sparx.Response res,int id)
        {
            LoadingSpinner.Hide();
            if (res.sucessful)
            {
                List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stageTmps = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(ActivityId);
                if (LTHotfixManager.GetManager<TaskManager>().CurrentIndex < stageTmps.Count - 1)
                    LTHotfixManager.GetManager<TaskManager>().CurrentIndex++;
                string cachePath;
                cachePath = "user_prize_data.taskacm_reward." + m_Chest.StageData.Id;
                DataLookupsCache.Instance.CacheData(cachePath, true);
                //上传友盟获得钻石，任务
                int hcCount = 0;
                List<LTShowItemData> mlist = m_Chest.StageData.Awards;
                for (int i = 0; i < mlist.Count; i++)
                {
                    if (mlist[i].id == "hc") hcCount += mlist[i].count;
                }
                FusionTelemetry.PostBonus(hcCount, Umeng.GA.BonusSource.Source2);
                GlobalMenuManager.Instance.Open("LTShowRewardView", m_Chest.StageData.Awards);
                UpdateChest();
            }
            else
            {
                res.CheckAndShowModal();
                //SparxHub.Instance.FatalError(res.localizedError);
            }
        }

        public const int ActivityId = 7001;
        static public int GetCurrentStageIndex()
        {
            if (LTHotfixManager.GetManager<TaskManager>().CurrentIndex > 0)
                return LTHotfixManager.GetManager<TaskManager>().CurrentIndex;

            IDictionary have_receive_stages;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.taskacm_reward", out have_receive_stages))
            {
                return 0;
            }

            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stageTmps = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(ActivityId);
            var iter = have_receive_stages.GetEnumerator();
            List<int> have_receive_stageIds = new List<int>();
            while (iter.MoveNext())
            {
                int id = 0;
                string key = iter.Key as string;
                if (!int.TryParse(key, out id))
                    EB.Debug.LogError("parse taskacm_reward.stage.key fail");
                else
                    have_receive_stageIds.Add(id);
            }
            if (have_receive_stageIds.Count <= 0)
            {
                return 0;
            }
            have_receive_stageIds.Sort();
            int maxId = have_receive_stageIds[have_receive_stageIds.Count - 1];
            int index = stageTmps.FindIndex(m => m.id == maxId);
            if (index < stageTmps.Count - 1)
                return LTHotfixManager.GetManager<TaskManager>().CurrentIndex = index + 1;
            else
                return LTHotfixManager.GetManager<TaskManager>().CurrentIndex = index;
        }
    }

}