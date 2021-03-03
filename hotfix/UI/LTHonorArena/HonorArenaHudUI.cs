using System;
using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public enum HonorOpenState
    {
        Open,
        Close
    }
    
    public class HonorArenaHudUI : UIControllerHotfix
    {
        public LTArenaRankListCtrl RankListCtrl;
        public HonorArenaAwardView awardView;
        public UIButton RuleBtn;
        public UIButton RewardBtn;
        public GameObject RewardBtnRedPoint;
        public UIButton ShopBtn;
        public UIButton EmbattleBtn;
        public UIButton LogBtn;
        public UIButton ChallengeBtn;

        public UISprite LeaderMainIcon;
        public UILabel ScoreLabel;
        public UILabel PowerLabel;
      
        public UILabel OpenTimeLabelTip;
        public UILabel OpenTimeLabel;
        public UILabel StoneLabel;

        public UILabel OnHookincomeLabel;
        public UIProgressBar ProgressBar;
        public ContinueClickCDTrigger progressRewardBtn;
        public UILabel ProgressBarLabel;
        public Transform fxTransform;
        private HonorOpenState _openState;
        public HonorArenaAPI v_Api
        {
            get; private set;
        }
        
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            RankListCtrl = t.GetMonoILRComponent<LTArenaRankListCtrl>("CenterBG");
        
            awardView = t.GetMonoILRComponent<HonorArenaAwardView>("AwardView");
            RuleBtn = t.GetComponent<UIButton>("LeftTopHold/RuleBtn");
            RewardBtn = t.GetComponent<UIButton>("RightTopHold/Reward_Button");
            ShopBtn = t.GetComponent<UIButton>("RightTopHold/Shop_Button");
            EmbattleBtn = t.GetComponent<UIButton>("RightTopHold/EmbattleBtn");
            LogBtn = t.GetComponent<UIButton>("RightTopHold/LogBtn");

            LeaderMainIcon = t.GetComponent<UISprite>("LeftTopHold/IconBG/Icon");
            ScoreLabel = t.GetComponent<UILabel>("LeftTopHold/ScoreLabel");
            PowerLabel = t.GetComponent<UILabel>("LeftTopHold/CombatPower/Base");

            OpenTimeLabelTip = t.GetComponent<UILabel>("LeftTopHold/ServerOpen");
            OpenTimeLabel = t.GetComponent<UILabel>("LeftTopHold/ServerOpenTime");
            StoneLabel = t.GetComponent<UILabel>("LeftTopHold/HonorStore/Bg/Label");
             OnHookincomeLabel =t.GetComponent<UILabel>("LeftTopHold/OnHookincomeLabel");
            ProgressBar = t.GetComponent<UIProgressBar>("LeftTopHold/ProgressWidget/ProgressBar");
            progressRewardBtn = t.GetComponent<ContinueClickCDTrigger>("LeftTopHold/ProgressWidget/ProgressBar/RewardBtn");
            RewardBtnRedPoint = t.FindEx("LeftTopHold/ProgressWidget/ProgressBar/RewardBtn/RedPoint").gameObject;
            ProgressBarLabel = t.GetComponent<UILabel>("LeftTopHold/ProgressWidget/ProgressBar/Label");
            ChallengeBtn = t.GetComponent<UIButton>("LeftTopHold/ChallengeBtn");
            fxTransform = ProgressBar.transform.Find("Foreground/fx");
            UIButton backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
            RuleBtn.onClick.Add(new EventDelegate(OnRuleBtnClick));
            RewardBtn.onClick.Add(new EventDelegate(OnRewardBtnClick));
            ShopBtn.onClick.Add(new EventDelegate(OnShopBtnClick));
            EmbattleBtn.onClick.Add(new EventDelegate(OnEmbattleButtonClick));
            LogBtn.onClick.Add(new EventDelegate(OnGoToLogsButtonClick));
            ChallengeBtn.onClick.Add(new EventDelegate(OnChallengeBtnClick));
            progressRewardBtn.m_CallBackPress.Add(new EventDelegate(OnProgressRewardBtnClick));
            controller.transform.GetComponent<UIButton>("LeftTopHold/HonorStore/Bg").onClick.Add(new EventDelegate(OnBuyTimesButtonClick));
            awardView.SetData(HonorArenaConfig.Instance.InitRewardView());
            GameDataSparxManager.Instance.RegisterListener(HonorArenaManager.HonorArenaInfoDataId, OnArenaInfoListener);
            Messenger.AddListener<int>(EventName.HonorArenaRankChange,HonorArenaRankChange);
            
            LTUIUtil.SetText(OnHookincomeLabel,String.Format(EB.Localizer.GetString("ID_LEGION_TECH_REWARDRATE"),0));
            ProgressBarLabel.text = string.Format("{0}/" + HonorArenaConfig.Instance.GetHonorArenaUpperLimit(), reward);
        }

        private int reward;
        private int rank;
        private HonorArenaInfo info;
        private int hour;
        private void HonorArenaRankChange(int rank)
        {
            this.rank = rank;
            UpdateIdelReward();
        }

        private void UpdateIdelReward()
        {
            HonorArenaInfo info = HonorArenaManager.Instance.Info;
            // EB.Debug.Log(EB.Time.FromPosixTime(info.last_one_hour).ToLocalTime());
            int onehoursNum = 0;
            long ts = EB.Time.Now - info.last_one_hour;
            if (info.last_one_hour > 0 && ts > 0)
            {
                hour = (int) (ts / (10 * 60));
                onehoursNum = HonorArenaConfig.Instance.GetOneHourByReward(rank);
            }
            reward = info.reward + (hour * onehoursNum / 6);
            reward = Math.Min(reward, HonorArenaConfig.Instance.GetHonorArenaUpperLimit());
            RewardBtnRedPoint.CustomSetActive(reward > onehoursNum);
            ProgressBar.value = HonorArenaConfig.Instance.GetHonorArenaUpperLimit() == 0 ? 0 :
                reward * 1.0f / HonorArenaConfig.Instance.GetHonorArenaUpperLimit();
            string forme = "{0}/" + HonorArenaConfig.Instance.GetHonorArenaUpperLimit();
            ProgressBarLabel.text = string.Format(forme, reward);
        }
        
        private Coroutine m_CountRewardTimeCoroutine;
        private IEnumerator StartCountReward()
        {
            while (true)
            {
                long ts = (long)EB.Time.Now - info.last_one_hour;
                int hour = (int)(ts / (10 * 60));
                if (this.hour!=hour)
                {
                    UpdateIdelReward();
                }
                yield return new WaitForSeconds(1.0f);
            }
        }

        private void OnProgressRewardBtnClick()
        {
            int curreward = reward;
            if (reward<=0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_REWARD_NO_TIP"));
                return;
            }
            HonorArenaManager.Instance.GetOneHourReward(()=>
            {
                HonorArenaRankChange(rank);
                List<LTShowItemData> list = new List<LTShowItemData>();
                string type = "res";
                int num = curreward;
                string id = "honorarena-gold";
                list.Add(new LTShowItemData(id, num, type));
                GlobalMenuManager.Instance.Open("LTShowRewardView", list);
            });
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameDataSparxManager.Instance.UnRegisterListener(HonorArenaManager.HonorArenaInfoDataId, OnArenaInfoListener);
            Messenger.RemoveListener<int>(EventName.HonorArenaRankChange,HonorArenaRankChange);
        }

        private void OnArenaInfoListener(string path, INodeData data)
        {
            info = data as HonorArenaInfo;

            string oldText = ScoreLabel.text.ToString();
            
            ScoreLabel.text = info.point;
            StoneLabel.text = info.ticket.ToString();
            UpdateIdelReward();

            if (!String.IsNullOrEmpty(oldText) && !String.IsNullOrEmpty(info.point) && !info.point.Equals(oldText))
            {
                Messenger.Raise(EventName.HonorArenaRankNeedReq);
            }
        }


        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            LeaderMainIcon.spriteName = LTMainHudManager.Instance.UserHeadIcon;
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.honor_topic,
                FusionTelemetry.GamePlayData.honor_event_id,FusionTelemetry.GamePlayData.honor_umengId,"open");
        }
        
        public void OnBuyTimesButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int buyCost = HonorArenaManager.Instance.Info.quantity;
            //购买花费
            if (BalanceResourceUtil.GetUserDiamond() <buyCost)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add("0", buyCost);
            MessageTemplateManager.ShowMessage(902123, ht, delegate (int result)
            {
                if (result == 0)
                {
                    HonorArenaManager.Instance.BuyChallengeTimes(delegate (bool successful)
                    {
                        StoneLabel.text = string.Format("{0}", HonorArenaManager.Instance.Info.ticket);
                    });
                }
            });
            Johny.HashtablePool.Release(ht);
        }

        public void OnEmbattleButtonClick()
        {
            LTHonorArenaBattleController.Open(true,0,false);
        }
        
        public void OnChallengeBtnClick()
        {
            // LTHonorArenaSelectRivalUI
            if (_openState==HonorOpenState.Close)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HONOR_ARENA_RAKN_OPEN_TIP"));
                return;
            }
            GlobalMenuManager.Instance.Open("LTHonorArenaSelectRivalUI");
        }
        
        public void OnGoToLogsButtonClick()
        {
             GlobalMenuManager.Instance.Open("ArenaLogUI",HonorArenaManager.ArenaBattleLogsDataId);
        }
        
        private Coroutine m_ResetTimeCoroutine;

        public override bool IsFullscreen() { return true; }

        public override IEnumerator OnAddToStack()
        {
             HonorArenaManager.Instance.GetInfo();
                
            onCombatTeamPowerUpdate(HonorArenaManager.Instance.AllCombatPower);
            Messenger.AddListener<int>(EventName.HonorCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
            
            JudgeState();
            
            if (m_CountRewardTimeCoroutine != null)
            {
                StopCoroutine(m_CountRewardTimeCoroutine);
                m_CountRewardTimeCoroutine = null;
            }

            m_CountRewardTimeCoroutine = StartCoroutine(StartCountReward());
            
            yield return base.OnAddToStack();
            StartCoroutine(FirstToFormation());
        }

        public override void OnFocus()
        {
            base.OnFocus();
        }

        private IEnumerator FirstToFormation()
        {
            yield return new WaitForSeconds(1f);
            // LTFormationDataManager.Instance.RequestHonorMulToFormationPos((hs) =>
            //     {
            //         HonorArenaManager.Instance.setBR();
            //     });
            if (!HonorArenaManager.Instance.Info.IsOpen)
            {
                HonorArenaManager.Instance.Info.IsOpen = true;
                LTFormationDataManager.Instance.RequestHonorMulToFormationPos((hs) =>
                {
                    HonorArenaManager.Instance.setBR();
                });
            }
        }

        private void onCombatTeamPowerUpdate(int power)
        {
            // LTUIUtil.SetText(CombatbatPowerLabel,);          
            LTUIUtil.SetText(PowerLabel, power.ToString());
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            Messenger.RemoveListener<int>(EventName.HonorCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
            return base.OnRemoveFromStack();
        }

       

        private void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_HONOR_ARENA_RULE_TEXT");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        private void OnRewardBtnClick()
        {
            awardView.mDMono.gameObject.SetActive(true);
            UITweener tw = awardView.mDMono.gameObject.GetComponent<UITweener>();
            tw.ResetToBeginning();
            tw.PlayForward();
        }

        private void OnShopBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTStoreUI", "honor_arena");
        }
        
        
        private IEnumerator SetResetTime(long resetTime)
        {
            while (true)
            {
                System.TimeSpan ts = EB.Time.FromPosixTime(resetTime) - EB.Time.LocalNow;
                int days = ts.Days;
                int hours = ts.Hours;
                int minutes = ts.Minutes;
                int seconds = ts.Seconds;
                string time = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                string timeS = string.Format(EB.Localizer .GetString("ID_DAY_FORMAT"),  days, time);
    
                LTUIUtil.SetText(OpenTimeLabel, timeS);
                if (ts.Days == 0 && ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds <= 0)
                {
                    JudgeState();
                    break;
                }
                yield return new WaitForSeconds(1.0f);
            }
        }
        
    
        
        private void JudgeState()
        {
            if (m_ResetTimeCoroutine != null)
            {
                StopCoroutine(m_ResetTimeCoroutine);
                m_ResetTimeCoroutine = null;
            }
            DateTime start = EventTemplateManager.Instance.GetNextTime("honorarenastart");
            DateTime stop = EventTemplateManager.Instance.GetNextTime("honorarenastop");
            // DateTime reward = EventTemplateManager.Instance.GetNextTime("InfiniteCompeteReward");
            if (stop>start)
            {
                //关闭状态 赛季开启倒计时
                m_ResetTimeCoroutine = StartCoroutine(SetResetTime(EB.Time.ToPosixTime(start)));
                LTUIUtil.SetText(OpenTimeLabelTip,EB.Localizer.GetString("ID_HONOR_ARENA_RAKN_OPEN"));
                _openState = HonorOpenState.Close;
            }
            else if (stop<start)
            {
                //开启状态  赛季关闭倒计时
                m_ResetTimeCoroutine = StartCoroutine(SetResetTime(EB.Time.ToPosixTime(stop)));
                LTUIUtil.SetText(OpenTimeLabelTip,EB.Localizer.GetString("ID_HONOR_ARENA_RAKN_CLOSE"));
                _openState = HonorOpenState.Open;
            }
        }
    }
    
}