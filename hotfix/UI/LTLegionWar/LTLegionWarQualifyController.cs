using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI {
    public class LTLegionWarQualifyController : UIControllerHotfix, IHotfixUpdate {
        public override void Awake() {
            base.Awake();

            var t = controller.transform;
            slideTween = t.GetComponent<TweenPosition>("ScoreBoard/Container/MotionFrame");
            ScoreBoardArrow = t.GetComponent<UISprite>("ScoreBoard/Container/MotionFrame/Btn/arrow");
            ScoreBoardDynamicScroll = t.GetMonoILRComponent<ScoreBoardDynamicScroll>("ScoreBoard/Container/MotionFrame/Scroll View/Placeholder/Grid");
            PlayerItem = t.GetMonoILRComponent<ScoreBoardCell>("ScoreBoard/Container/MotionFrame/PlayerItem");
            TimeLabel = t.GetComponent<UILabel>("Edge/Top/Time");
            EndLabel = t.GetComponent<UILabel>("Edge/Top/End");
            BGTexture = t.GetComponent<CampaignTextureCmp>("BG");
            CloseBg = t.FindEx("ScoreBoard/Container/CloseBg").gameObject;
            m_LivenessProgressBar = t.GetComponent<UIProgressBar>("Edge/LivenessReward/ProgressBar");
            m_CurrLivenessLabel = t.GetComponent<UILabel>("Edge/LivenessReward/Score");
            ActivityId = 10001;
            controller.backButton = t.GetComponent<UIButton>("Edge/BG/TopLeft/CancelBtn");

            t.GetComponent<UIButton>("Edge/BG/Chat").onClick.Add(new EventDelegate(OnChatBtnClick));
            t.GetComponent<UIButton>("Edge/BG/Friend").onClick.Add(new EventDelegate(OnFriendBtnClick));
            t.GetComponent<UIButton>("Edge/LivenessReward/0").onClick.Add(new EventDelegate(() => OnChestClick(t.FindEx("Edge/LivenessReward/0").gameObject)));
            t.GetComponent<UIButton>("Edge/LivenessReward/1").onClick.Add(new EventDelegate(() => OnChestClick(t.FindEx("Edge/LivenessReward/1").gameObject)));
            t.GetComponent<UIButton>("Edge/LivenessReward/2").onClick.Add(new EventDelegate(() => OnChestClick(t.FindEx("Edge/LivenessReward/2").gameObject)));
            t.GetComponent<UIButton>("ScoreBoard/Container/MotionFrame/Btn").onClick.Add(new EventDelegate(OnScoreBoardBtnClick));

            t.GetComponent<UIEventTrigger>("ScoreBoard/Container/CloseBg").onClick.Add(new EventDelegate(OnScoreBoardBtnClick));

            m_Chests = new GameChest[3];
            for (int chestIndex = 0; chestIndex < m_Chests.Length; ++chestIndex)
            {
                m_Chests[chestIndex] = new GameChest();
                m_Chests[chestIndex].Open = t.FindEx("Edge/LivenessReward/"+chestIndex+"/Open").gameObject;
                m_Chests[chestIndex].UnOpen = t.FindEx("Edge/LivenessReward/" + chestIndex + "/Close").gameObject;
                m_Chests[chestIndex].Light = t.FindEx("Edge/LivenessReward/" + chestIndex + "/Light").gameObject;
                m_Chests[chestIndex].Value = t.GetComponent<UILabel>("Edge/LivenessReward/" + chestIndex + "/ActivityNum");
                m_Chests[chestIndex].OriginPos = Vector3.zero;
                m_Chests[chestIndex].OriginPos = m_Chests[chestIndex].UnOpen.transform.localPosition;
            }
            FlowEnemyHudList = new FlowEnemyHud[7];
            FlowEnemyHudList[0] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud");
            FlowEnemyHudList[1] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud (1)");
            FlowEnemyHudList[2] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud (2)");
            FlowEnemyHudList[3] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud (3)");
            FlowEnemyHudList[4] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud (4)");
            FlowEnemyHudList[5] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud (5)");
            FlowEnemyHudList[6] = t.GetMonoILRComponent<FlowEnemyHud>("FlowEnemyHud/Root/FlowEnemyHud (6)");
        }
        
        public override bool IsFullscreen() { return true; }
        private bool isOpen;
    
        public TweenPosition slideTween;
        public UISprite ScoreBoardArrow;
        public ScoreBoardDynamicScroll ScoreBoardDynamicScroll;
        public ScoreBoardCell PlayerItem;
        
        public UILabel TimeLabel;
        public UILabel EndLabel;
        public CampaignTextureCmp BGTexture;
    
        public FlowEnemyHud[] FlowEnemyHudList;
    
        private bool isWarOver;
    
        private float updateTimer = 1;

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

        public override void OnDisable()
        {
            ErasureMonoUpdater();
        }

        public void Update()
        {
            // base.Update();
            //更新时间
            if (!isOpen) return;
            if (updateTimer < 1)
            {
                updateTimer += Time.deltaTime;
            }
            else
            {
                CheckTime();
                updateTimer =0;
            }
        }
    
        public override IEnumerator OnAddToStack()
        {
            BGTexture.spriteName = "Game_Background_21";
            yield return base.OnAddToStack();
            GlobalMenuManager.Instance.PushCache("LTLegionWarQualifyView");
            yield return new WaitUntil(() => LTLegionWarJoinController.callBack);
            GameDataSparxManager.Instance.RegisterListener(LTLegionWarManager.QualifyEnemyListDataId, OnFlowEnemyListListener);
            if (LTLegionWarManager.Instance.serveCurState != 1)
            {
                controller.Close();
                yield break;
            }
            else
            {
                int ts = 0;
                if (LTLegionWarTimeLine.Instance != null)
                {
                    ts = Convert.ToInt32(LTLegionWarTimeLine.Instance.QualifyEndTime - EB.Time.Now);
                }
                if (ts <= 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                    CheckTime();
                    yield break;
                }
            }
            isOpen = true;
            FusionAudio.PostBGMEvent("BGM/JunTuanZhan", true);
            FusionAudio.StartBGM();
            LTLegionWarManager.Instance.GetQualifyWarEnemyList();//获取预赛数据
        }
    
        public override IEnumerator OnRemoveFromStack() {
            isOpen = false;
            StopAllCoroutines();
            controller.CancelInvoke();
            GameDataSparxManager.Instance.UnRegisterListener(LTLegionWarManager.QualifyEnemyListDataId, OnFlowEnemyListListener);
            FusionAudio.StopBGM();
            TimeLabel.text = TimeLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Empty;
            BGTexture.spriteName = string.Empty;
            DestroySelf();
            yield break;
        }
    
        public void OnRuleBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            string rule = EB.Localizer.GetString("ID_RULE_ALLIANCEWAR");
            GlobalMenuManager.Instance.Open("LTRuleUIView", rule);
        }
    
        private void CheckTime()
        {
            bool isShowTime = false;
            LegionWarTimeLine time = LTLegionWarManager.GetLegionWarStatus(); 

            if (time != LegionWarTimeLine.QualifyGame)
            {
                isWarOver = true;
                isShowTime = false;
            }
            else
            {
                int ts = 0;

                if (LTLegionWarTimeLine.Instance != null)
                {
                    ts = Convert.ToInt32(LTLegionWarTimeLine.Instance.QualifyEndTime - EB.Time.Now);
                }
                
                ts = ts <= 0 ? 0 : ts;
                DateTime dt = EB.Time.FromPosixTime(ts);
                string timeStr;

                if (dt.Hour > 0)
                {
                    timeStr = dt.ToString("hh:mm:ss");
                }
                else
                {
                    timeStr = dt.ToString("mm:ss");
                }

                if (TimeLabel != null)
                {
                    var child = TimeLabel.transform.GetChild(0);

                    if (child != null)
                    {
                        var label = child.GetComponent<UILabel>();

                        if (label != null)
                        {
                            label.text = timeStr;
                        }
                    }
                    
                    TimeLabel.text = timeStr;
                }

                isShowTime = true;
            }

            if (EndLabel != null)
            {
                EndLabel.gameObject.CustomSetActive(!isShowTime);
            }

            if (TimeLabel != null)
            {
                TimeLabel.gameObject.CustomSetActive(isShowTime);
            }
        }
    
        private bool ScoreBoardOpen = false;
        public GameObject CloseBg;
        public void OnScoreBoardBtnClick() {
            ScoreBoardOpen = !ScoreBoardOpen;
            if (ScoreBoardOpen) {
                slideTween.PlayForward();
                CloseBg.CustomSetActive(true);
                ScoreBoardArrow.transform.localRotation=Quaternion.Euler(new Vector3(0,0,90));
            }
            else {
                slideTween.PlayReverse();
                CloseBg.CustomSetActive(false);
                ScoreBoardArrow.transform.localRotation = Quaternion.Euler(new Vector3(0, 0,-90));
            }
            SetLegionScoreBoard();
        }
    
        private void SetLegionScoreBoard() {
            if (isWarOver)
            {
                UpdateLegionRank();
            }
            else
            {
                LTLegionWarManager.Instance.GetLegionRank(delegate
                {
                    UpdateLegionRank();
                });
            }
        }
        private void UpdateLegionRank()
        {
            List<LegionRankData> temp = LTLegionWarManager.Instance.LegionRankList.LegionRankList;
            List<ScoreBoardData> rankList = SetLegionScoreBoard(temp);
            ScoreBoardData myRank = LTLegionWarManager.Instance.LegionRankList.MyLegionRank;
            SetScoreBoard(rankList);
            SetPlayerItem(myRank);
        }
        private void SetScoreBoard(List<ScoreBoardData> rankList)
        {
            ScoreBoardDynamicScroll.SetItemDatas(rankList.ToArray());
        }
        private void SetPlayerItem(ScoreBoardData rank) {
            if (rank!=null &&rank.Name!=null) {
                PlayerItem.mDMono.gameObject.SetActive(true);
                PlayerItem.Fill(rank);
            }
            else {
                PlayerItem.mDMono.gameObject.SetActive(false);
            }
        }
        private List<ScoreBoardData> SetLegionScoreBoard(List<LegionRankData> rankList) {
            List<ScoreBoardData> board = new List<ScoreBoardData>();
            for(int i = 0; i < rankList.Count; i++) {
                ScoreBoardData data = new ScoreBoardData();
                data.Name = rankList[i].Name;
                data.Rank = rankList[i].Rank;
                data.Score = rankList[i].Score;
                board.Add(data);
            }
            return board;
        }
    
        public void OnChatBtnClick() {
            GlobalMenuManager.Instance.Open("ChatHudView", null);
        }
    
        public void OnFriendBtnClick() {
            GlobalMenuManager.Instance.Open("FriendHud", null);
        }
    
        private void OnFlowEnemyListListener(string path, INodeData data)
        {
            var datas = LTLegionWarManager.Instance.QualifyEnemyList.GetFlowEnemyDataList();
            for (int i=0;i< datas.Count; i++)
            {
                FlowEnemyHudList[i].Fill(datas[i]);
            }
            int myScore = LTLegionWarManager.Instance.QualifyEnemyList.MyScore;
            UpdateChest(myScore);
        }

        public override void StartBootFlash()
        {
	        base.StartBootFlash();

	        ILRTimerManager.instance.AddTimer(50, 1, sequence =>
	        {
		        if (FlowEnemyHudList != null)
		        {
			        int length = FlowEnemyHudList.Length;
			        for (int i = 0; i < length; i++)
			        {
				        var flow = FlowEnemyHudList[i];

                        if (flow != null)
                        {
                            flow.ShowFx();
                        }
			        }
		        }
			});
        }

        public override void OnCancelButtonClick() {
            GlobalMenuManager.Instance.RemoveCache("LTLegionWarQualifyView");
            base.OnCancelButtonClick();
        }
    
        #region 宝箱
        public GameChest[] m_Chests;
        private GameChest m_CurrReceiveChest;
        public UIProgressBar m_LivenessProgressBar;
        public UILabel m_CurrLivenessLabel;
        public int ActivityId = 10001;
    
        private float leftPos = -700f;
        private float length = 1700f;
        
        public void UpdateChest(int currLiveness) {        
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = LTLegionWarManager.Instance.GetQualityRewardList();
            int maxLiveness = LTLegionWarManager.Instance.MaxScore;
            for (int i = 0; i < stages.Count; ++i) {
                var stage = stages[i];
                List<LTShowItemData> itemDatas = new List<LTShowItemData>();
                for (int j = 0; j < stage.reward_items.Count; ++j) {
                    var reward = stage.reward_items[j];
                    string id = reward.id.ToString();
                    int count = reward.quantity;
                    string type = reward.type;
                    LTShowItemData itemData = new LTShowItemData(id, count, type, false);
                    itemDatas.Add(itemData);
                }
    
                bool received=false;
                received = LTLegionWarManager.Instance.QualifyEnemyList.awarScoreReward.Contains(stage.stage.ToString());
                eReceiveState rs = eReceiveState.cannot;
                if (currLiveness >= stage.stage) {
                    if (received)
                        rs = eReceiveState.have;
                    else
                        rs = eReceiveState.can;
                }
                if (m_Chests[i] != null)
                    m_Chests[i].SetUI(new RewardStageData(stage.id, stage.stage, itemDatas, rs));
                float x = length*((float)stage.stage / (float)maxLiveness) + leftPos;
                m_Chests[i].Open.transform.parent.transform.localPosition = new Vector3(x, m_Chests[i].Open.transform.parent.transform.localPosition.y, 0);
            }
            m_CurrLivenessLabel.text = m_CurrLivenessLabel.transform .GetChild (0).GetComponent <UILabel>().text = currLiveness.ToString();
            m_LivenessProgressBar.value = currLiveness / (float)maxLiveness;
    
        }
    
        public void OnChestClick(GameObject go) {
            if (isWarOver) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTLegionWarQualifyController_13360"));return; }
            int index;
            int.TryParse(go.name, out index);
            GameChest chest =  m_Chests[index];
    
            if (chest.StageData.ReceiveState == eReceiveState.can) {
                m_CurrReceiveChest = chest;
                chest.UpdateReceiveState(eReceiveState.have);
                SendReceiveRewardReq(chest.StageData);
                return;
            }
    
            string tip = "";
            if (chest.StageData.ReceiveState == eReceiveState.cannot) {            
                    tip = string.Format(EB.Localizer.GetString("ID_codefont_in_LTLegionWarQualifyController_13908"), chest.StageData.Stage);
            }
            else if (chest.StageData.ReceiveState == eReceiveState.have) {
                tip = EB.Localizer.GetString("ID_codefont_in_LadderController_11750");
            }
            var ht = Johny.HashtablePool.Claim();
            ht.Add("data", chest.StageData.Awards);
            ht.Add("tip", tip);
            GlobalMenuManager.Instance.Open("LTRewardShowUI", ht);
        }
    
        private void SendReceiveRewardReq(RewardStageData stageData) {

            LTLegionWarManager.Instance.ReceiveBox(stageData.Id.ToString(), OnReceiveChestResponse);

        }
    
        public void OnReceiveChestResponse(Hashtable res) {
            if (res != null) {
                DataLookupsCache.Instance.CacheData(res);//手动存储，钻石抽奖无法存储
                LTLegionWarManager.Instance.FetchAward(res);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTLegionWarQualifyController_14751"));
                //上传友盟获得钻石，军团
                FusionTelemetry.ItemsUmengCurrency(m_CurrReceiveChest.StageData.Awards, "军团战获得");
                FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.alliance_war_topic,
                FusionTelemetry.GamePlayData.alliance_war_event_id, FusionTelemetry.GamePlayData.alliance_war_umengId, "reward");
                GlobalMenuManager.Instance.Open("LTShowRewardView", m_CurrReceiveChest.StageData.Awards);
            }
            else {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_62898"));
                //res.CheckAndShowModal();
            }
        }
        #endregion
    }
}
