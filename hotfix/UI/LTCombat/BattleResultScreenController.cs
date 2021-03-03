using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class BattleResult
    {
        public static string ROB_BATTLE = "rob_battle";
        public static string CAMPAIGN_BATTLE = "campaign_battle";
        public string type = "";
    }

    public class BattleResultScreenController : UIControllerHotfix
    {
        public override bool IsFullscreen(){  return true; }

        static public BattleResultScreenController Instance { get; private set; }

        public GameObject ScreenCollider;
        public GameObject VictoryAnimation;
        public GameObject DefeatRating;
        public GameObject DefeatAnimation;
        public GameObject CampaignRating;
        public GameObject CommonCampaignRating;
        public GameObject LadderRating;
        public GameObject ResInstanceRating;
        public GameObject ArenaRating;
        public GameObject WorldBossRating;
        public GameObject HonorArenaResult;
        public LTHeroBattleResultView heroBattleResult;
        public eCombatOutcome m_CombatResult;
        public GameObject DarkObj;
        public GameObject ContinueTips;
        public UILabel timeTips;//用于自动退出战斗倒计时
        public GameObject AnalyseBtn;
        public bool debugTest = false;
        public int reviveCost = 10;

        bool _isShowingBattleResult = false;
        bool _isCampaignFinished = false;
        eBattleType _battleType = eBattleType.None;
        bool _isShowingRating = false;
        bool isCombatOut = false;//主动退出战斗的标记
        private bool isFastCombat = false;
        public override void Awake()
        {
            base.Awake();
            Instance = this;

            var t = controller.transform;
            ScreenCollider = t.FindEx("Collider").gameObject;
            VictoryAnimation = t.FindEx("VictoryObj").gameObject;
            DefeatRating = t.FindEx("BattleDefeatRating").gameObject;
            DefeatAnimation = t.FindEx("DefeatObj").gameObject;
            CampaignRating = t.FindEx("CampaignRatingGM").gameObject;
            CommonCampaignRating = t.FindEx("CommonBuddyRatingGM").gameObject;
            LadderRating = t.FindEx("LadderRatingGM").gameObject;
            ResInstanceRating = t.FindEx("ResInstanceRatingGM").gameObject;
            ArenaRating = t.FindEx("ArenaRatingGM").gameObject;
            WorldBossRating = t.FindEx("WorldBoss").gameObject;
            HonorArenaResult = t.FindEx("HonorArenaResult").gameObject;
            heroBattleResult = t.GetMonoILRComponent<LTHeroBattleResultView>("HeroBattleResultGM");
            m_CombatResult = eCombatOutcome.Win;
            DarkObj = t.FindEx("Dark").gameObject;
            ContinueTips = t.FindEx("TipPanel/Tip").gameObject;
            timeTips = t.GetComponent<UILabel>("TipPanel/Time");
            AnalyseBtn = t.FindEx("TipPanel/BattleAnalyse").gameObject;
            AnalyseBtn.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnAnalyseBtnClick));
            debugTest = false;
            reviveCost = 5000;

            t.GetComponent<UIEventTrigger>("Collider").onClick.Add(new EventDelegate(OnContinueClick));
        }

        public override void OnDestroy()
        {
            Instance = null;
        }

        public override void OnEnable()
        {
            Reset();
        }

        public override void OnDisable()
        {
            UICamera.onDrag = null;
            Reset();
        }

        private object mParam;

        public override void SetMenuData(object param)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 1.2f);
            base.SetMenuData(param);
            mParam = param;
        }

        public override IEnumerator OnAddToStack()
        {
            Hashtable ht = mParam as Hashtable;
            eBattleType battleType = (eBattleType)ht["battleType"];
            if (SceneLogic.BattleType != battleType) SceneLogic.BattleType = battleType;
            eCombatOutcome outCome = (eCombatOutcome)ht["outCome"];
            bool isCampaignFinished = (bool)ht["campaignFinished"];
            isCombatOut = (bool)ht["isCombatOut"];
            isFastCombat = EB.Dot.Bool("isFastCombat", ht, false);
            yield return base.OnAddToStack();

            Show(battleType, outCome, isCampaignFinished);
            if (BattleReadyHudController.ShowMercenary())
            {
                //战斗结束 预先把雇佣的伙伴换下
                LTFormationDataManager.Instance.SetCurTeamMemberData();
            }
        }

        public void Reset()
        {
            DisableAllRating();
            ScreenCollider.CustomSetActive(false);
            ContinueTips.gameObject.CustomSetActive(false);
            _isShowingBattleResult = false;
            _isShowingRating = false;

            ArrayList list;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("battlerMetric", out list);
            AnalyseBtn.CustomSetActive(list != null && list.Count > 0);//战斗统计显示
        }

        private void DisableAllRating()
        {
            VictoryAnimation.CustomSetActive(false);
            DefeatRating.CustomSetActive(false);
            DefeatAnimation.CustomSetActive(false);
            CampaignRating.CustomSetActive(false);
            CommonCampaignRating.CustomSetActive(false);
            LadderRating.CustomSetActive(false);
            ResInstanceRating.CustomSetActive(false);
            ArenaRating.CustomSetActive(false);
            HonorArenaResult.CustomSetActive(false);
        }

        public void Show(eBattleType battletype, eCombatOutcome outcome, bool isCampaignFinished)
        {
            if (Instance == null) return;
            _isCampaignFinished = isCampaignFinished;
            _battleType = battletype;
            if (!_isShowingBattleResult)
            {
                RecordBattle(outcome);
                controller.gameObject.CustomSetActive(true);
                EB.Coroutines.Run(ShowOutcome(outcome));
                _isShowingBattleResult = true;
            }
        }

        private void RecordBattle(eCombatOutcome outcome)//上报友盟，是否通关
        {
            bool tmpIsWon = outcome == eCombatOutcome.Win ? true : false;
            string tmpLevel = string.Empty;
            if (_battleType == eBattleType.MainCampaignBattle)
            {
                tmpLevel = LTMainInstanceCampaignCtrl.CampaignId.ToString();
                if (!string.IsNullOrEmpty(tmpLevel))
                {
                    JudgeBattle(tmpIsWon, "Main" + tmpLevel);
                    if (tmpIsWon) LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.Main, tmpLevel);
                }
            }
            else if (_battleType == eBattleType.InfiniteChallenge)
            {
                DataLookupsCache.Instance.SearchDataByID<string>("infiniteChallenge.info.currentlayer", out tmpLevel);
                if (!string.IsNullOrEmpty(tmpLevel))
                {
                    JudgeBattle(tmpIsWon, _battleType.ToString() + tmpLevel);
                    if (tmpIsWon) LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.UltimateTrial, tmpLevel);
                }
                Data.NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out int dayDisCountTime, out int NewVigor, out int oldVigor);
                int curDisCountTime = dayDisCountTime - LTUltimateTrialDataManager.Instance.GetCurrencyTimes();
                if (outcome == eCombatOutcome.Win && curDisCountTime >= 0) LTDailyDataManager.Instance.SetDailyDataRefreshState();
            }
            else if (_battleType == eBattleType.TreasuryBattle)
            {
                ArrayList rewardData;
                DataLookupsCache.Instance.SearchDataByID<ArrayList>("combat.rewards", out rewardData);
                Hashtable rewardDataTable = Johny.HashtablePool.Claim();
                if (rewardData != null)
                {
                    rewardDataTable = rewardData[0] as Hashtable;
                }
                tmpLevel = EB.Dot.String("gold.currentDifficulty", rewardDataTable, null);

                if (!string.IsNullOrEmpty(tmpLevel))
                {
                    JudgeBattle(tmpIsWon, _battleType.ToString() + tmpLevel);
                }
            }
            else if (_battleType == eBattleType.ExpSpringBattle)
            {
                ArrayList rewardData;
                DataLookupsCache.Instance.SearchDataByID<ArrayList>("combat.rewards", out rewardData);
                Hashtable rewardDataTable = Johny.HashtablePool.Claim();
                if (rewardData != null)
                {
                    rewardDataTable = rewardData[0] as Hashtable;
                }
                tmpLevel = EB.Dot.String("currentDifficulty", rewardDataTable, null);
                if (!string.IsNullOrEmpty(tmpLevel))
                {
                    JudgeBattle(tmpIsWon, _battleType.ToString() + tmpLevel);
                }
            }
            else if (_battleType == eBattleType.ChallengeCampaign)
            {
                tmpLevel = LTInstanceMapModel.Instance.CurLevelNum.ToString();
            }
            int chapterId = 0;

            int.TryParse(tmpLevel, out chapterId);

            string teamName = FormationUtil.GetCurrentTeamName(_battleType);
            string teamDataPath = SmallPartnerPacketRule.USER_TEAM + "." + teamName + ".team_info";
            List<int> heroInfos = new List<int>();
            ArrayList teamDatas;
            DataLookupsCache.Instance.SearchDataByID(teamDataPath, out teamDatas);
            if (teamDatas != null)
            {
                for (var i = 0; i < teamDatas.Count; i++)
                {
                    var td = teamDatas[i];
                    string heroid = EB.Dot.String("hero_id", td, "");
                    if (!string.IsNullOrEmpty(heroid))
                    {
                        LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByHeroId(int.Parse(heroid));
                        if (data != null)
                        {
                            // IDictionary<string, int> hero = new Dictionary<string, int>();
                            // hero.Add(data.HeroInfo.name, data.Level);
                            heroInfos.Add(data.InfoId);
                        }
                    }
                }
            }
            FusionTelemetry.PostCombat(_battleType.ToString(), chapterId, tmpIsWon ? 1 : 2, heroInfos, isCombatOut ? 2 : 1);
        }

        private void JudgeBattle(bool isWon, string level)
        {
            if (isWon)
            {
                FusionTelemetry.PostFinishCombat(level);
            }
            else
            {
                FusionTelemetry.PostFailCombat(level);
            }
        }

        IEnumerator ShowOutcome(eCombatOutcome outcome)
        {
            FusionAudio.PostEvent("MUS/CombatView/Demo", false);

            m_CombatResult = outcome;
            Reset();
            switch (outcome)
            {
                case eCombatOutcome.Win:
                    FusionAudio.PostEvent("MUS/CombatView/Stinger/Victory", controller.gameObject, true);
                    switch (_battleType)
                    {
                        default:
                            ShowBattleResult(true);
                            break;
                    }
                    break;
                case eCombatOutcome.Draw:
                case eCombatOutcome.Lose:
                    switch (_battleType)
                    {
                        case eBattleType.MainCampaignBattle:
                        case eBattleType.ChallengeCampaign:
                        case eBattleType.SleepTower:
                        case eBattleType.InfiniteChallenge:
                            ShowDefeatMessage();
                            break;
                        case eBattleType.AlienMazeBattle:
                            if (isCombatOut)
                            {
                                ShowDefeatMessage();
                            }
                            else
                            {
                                ShowBattleResult(false);
                            };
                            break;
                        default:
                            ShowBattleResult(false);
                            break;
                    }
                    FusionAudio.PostEvent("MUS/CombatView/Stinger/Defeat", controller.gameObject, true);
                    break;
                default:
                    EB.Debug.LogError("BattleResultScreenController: No corresponding response for {0}" , outcome);
                    break;
            }

            PushCache(outcome);
            yield break;
        }

        void PushCache(eCombatOutcome outcome)
        {
            switch (SceneLogic.BattleType)
            {
                case eBattleType.LadderBattle:
                    GlobalMenuManager.Instance.PushCache("LadderUI");
                    break;
                case eBattleType.ArenaBattle:
                    GlobalMenuManager.Instance.PushCache("ArenaHudUI");
                    break;
                case eBattleType.HonorArena:
                    GlobalMenuManager.Instance.PushCache("HonorArenaHudUI");
                    break;
                case eBattleType.HantBattle:
                    if (!LTBountyTaskConversationController.sRunAway && LTBountyTaskHudController.HantTimes < LTBountyTaskHudController.TotalHantTimes)
                    {
                        GlobalMenuManager.Instance.PushCache("LTBountyTaskOverUI");
                    }
                    LTBountyTaskConversationController.sRunAway = false;
                    if (outcome==eCombatOutcome.Win)
                        FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.bounty_topic,
                            FusionTelemetry.GamePlayData.bounty_event_id,FusionTelemetry.GamePlayData.bounty_umengId,"reward");
                    break;
                case eBattleType.TransferOrRob:
                    if (outcome == eCombatOutcome.Win)
                    {
                        if (AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Rob)
                        {
                            AlliancesManager.Instance.DartData.State = eAllianceDartCurrentState.None;
                            if (AlliancesManager.Instance.RobDartInfo.RobAwards != null)
                            {
                                var list = new List<LTShowItemData>();

                                for (var i = 0; i < AlliancesManager.Instance.RobDartInfo.RobAwards.Count; i++)
                                {
                                    var item = AlliancesManager.Instance.RobDartInfo.RobAwards[i];
                                    list.Add(new LTShowItemData(item.id, item.count, item.type, false));
                                }

                                DartResultController.sRewardDataList = list;
                                AllianceEscortUtil.SetEscortResultHudCache(eDartResultType.RobSuccess);
                                FusionTelemetry.GamePlayData.PostEsortEvent("reward", "rob");
                            }
                            else
                            {
                                EB.Debug.LogError("rob dart success but award = null");
                            }
                        }
                    }
                    else
                    {
                        if (AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Rob)
                        {
                            AllianceEscortUtil.SetEscortResultHudCache(eDartResultType.RobFail);
                        }
                    }
                    break;
                case eBattleType.HeroBattle:
                    GlobalMenuManager.Instance.PushCache("LTHeroBattleMatch");
                    break;
                case eBattleType.NationBattle:
                    NationBattleHudController.IsContinueWalk = outcome == eCombatOutcome.Win;
                    int cityId = 0;
                    DataLookupsCache.Instance.SearchIntByID("combat.cityId", out cityId);
                    GlobalMenuManager.Instance.PushCache("LTNationBattleEntryUI", cityId);
                    break;
                case eBattleType.AwakeningBattle:
                    {
                        if (!GlobalMenuManager.Instance.IsContain("LTAwakeningInstanceHud"))
                        {
                            GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
                            GlobalMenuManager.Instance.PushCache("LTAwakeningInstanceHud");
                        }
                    }; break;
                case eBattleType.SleepTower:
                    {
                        if (!GlobalMenuManager.Instance.IsContain("LTClimbingTowerHud"))
                        {
                            GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
                            GlobalMenuManager.Instance.PushCache("LTClimbingTowerHud");
                        }
                    }; break;
                case eBattleType.ExpSpringBattle:
                    {
                        if (!GlobalMenuManager.Instance.IsContain("LTResourceInstanceUI"))
                        {
                            GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
                            GlobalMenuManager.Instance.PushCache("LTResourceInstanceUI", "Exp");
                        }
                    }; break;
                case eBattleType.TreasuryBattle:
                    {
                        if (!GlobalMenuManager.Instance.IsContain("LTResourceInstanceUI"))
                        {
                            GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
                            GlobalMenuManager.Instance.PushCache("LTResourceInstanceUI", "Gold");
                        }
                    }; break;
                case eBattleType.InfiniteChallenge:
                    if (!GlobalMenuManager.Instance.IsContain("LTUltimateTrialHud"))
                    {
                        GlobalMenuManager.Instance.PushCache("LTUltimateTrialHud");
                    }
                    break;
            }
        }

        public void ShowVictoryMessage()
        {
            // FusionAudio.PostEvent("MUS/CombatView/Stinger/Victory", gameObject, true);
            VictoryAnimation.CustomSetActive(true);
        }

        /// <summary>
        /// 显示失败界面
        /// </summary>
        public void ShowDefeatMessage()
        {
            CombatLogic.Instance.ExitCombat();
            DefeatRating.CustomSetActive(true);
            OpenGuideManager.Instance.PopFrameGuideStackExForce();//弥补战斗中失败，没有清理开放式引导
            OnRatingShownAnimFinished();
            FusionAudio.PostEvent("MUS/CombatView/Stinger/Defeat", controller.gameObject, true);
        }

        //显示战斗结算界面
        public void ShowBattleResult(bool win)
        {
            CombatCamera combatCamera = null;

            if (Camera.main != null)
            {
                combatCamera = Camera.main.GetComponent<CombatCamera>();

                if (combatCamera != null)
                {
                    combatCamera.HoldingCamera = true;
                }
            }
            switch (_battleType)
            {
                case eBattleType.ExpeditionBattle:
                    break;
                case eBattleType.TreasuryBattle:
                case eBattleType.ExpSpringBattle:
                    LTResourceInstanceRatingGM com = ResInstanceRating.GetMonoILRComponent<LTResourceInstanceRatingGM>();
                    com.OnShownAnimCompleted = OnRatingShownAnimFinished;
                    com.BattleType = _battleType;
                    ResInstanceRating.CustomSetActive(true);
                    if(win&&_battleType == eBattleType.TreasuryBattle)
                    {
                        FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.gold_camp_topic,
                        FusionTelemetry.GamePlayData.gold_camp_event_id, FusionTelemetry.GamePlayData.gold_camp_umengId, "reward");
                    }
                    else if(win && _battleType == eBattleType.ExpSpringBattle)
                    {
                        FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.exp_camp_topic,
                        FusionTelemetry.GamePlayData.exp_camp_event_id, FusionTelemetry.GamePlayData.exp_camp_umengId, "reward");
                    }
                    break;
                case eBattleType.MainCampaignBattle:
                    CampaignRatingDialogMH main_rating = CampaignRating.GetMonoILRComponent<CampaignRatingDialogMH>();
                    main_rating.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                    main_rating.onShownAnimCompleted = OnRatingShownAnimFinished;
                    CampaignRating.CustomSetActive(true);
                    LTGuideTips.IsEnableGuideTips = main_rating.IsWon;                   
                    break;
                case eBattleType.ChallengeCampaign:
                case eBattleType.AlienMazeBattle:
                    CommonRatingDialogLT challenge_rating = CommonCampaignRating.GetMonoILRComponent<CommonRatingDialogLT>();
                    challenge_rating.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                    challenge_rating.IsShowHp = true;
                    challenge_rating.onShownAnimCompleted = OnRatingShownAnimFinished;
                    CommonCampaignRating.CustomSetActive(true);
                    break;
                case eBattleType.ArenaBattle:
                    ArenaRatingDialogLT arena_rating = ArenaRating.GetMonoILRComponent<ArenaRatingDialogLT>();
                    arena_rating.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                    arena_rating.onShownAnimCompleted = OnRatingShownAnimFinished;
                    ArenaRating.CustomSetActive(true);
                    LTDailyDataManager.Instance.SetDailyDataRefreshState();
                    if(win)FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.arena_topic,
                        FusionTelemetry.GamePlayData.arena_event_id,FusionTelemetry.GamePlayData.arena_umengId,"reward");
                    break;
                case eBattleType.HonorArena:
                    var harc = HonorArenaResult.GetMonoILRComponent<HonorArenaResultController>();
                    harc.onShownAnimCompleted = OnRatingShownAnimFinished;
                    HonorArenaResult.CustomSetActive(true);
                    if(win)FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.honor_topic,
                        FusionTelemetry.GamePlayData.honor_event_id,FusionTelemetry.GamePlayData.honor_umengId,"reward");
                    break;
                case eBattleType.BossBattle:                  
                case eBattleType.AllianceCampaignBattle:
                    if(_battleType == eBattleType.BossBattle) LTDailyDataManager.Instance.SetDailyDataRefreshState();
                    LTWorldBossBattleResult boss_rating = WorldBossRating.GetMonoILRComponent<LTWorldBossBattleResult>();
                    boss_rating.onShownAnimCompleted = OnRatingShownAnimFinished;
                    WorldBossRating.CustomSetActive(true);
                    break;
                case eBattleType.HeroBattle:
                    heroBattleResult.mDMono.gameObject.CustomSetActive(true);
                    heroBattleResult.onShownAnimCompleted = OnRatingShownAnimFinished;
                    heroBattleResult.Show(m_CombatResult == eCombatOutcome.Win ? true : false);
                    if(win)FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.clash_topic,
                        FusionTelemetry.GamePlayData.clash_event_id,FusionTelemetry.GamePlayData.clash_umengId,"reward");
                    break;
                case eBattleType.TransferOrRob:
                    CommonRatingDialogLT transferOrRob = CommonCampaignRating.GetMonoILRComponent<CommonRatingDialogLT>();
                    transferOrRob.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                    transferOrRob.onShownAnimCompleted = OnRatingShownAnimFinished;
                    CommonCampaignRating.CustomSetActive(true);
                    //添加自动结束战斗
                    timer = 5;
                    sequence = ILRTimerManager.instance.AddTimer(1000, 10, delegate { AntoContinueClick(); });
                    break;
                case eBattleType.LadderBattle:
                    CommonRatingDialogLT ladder_rating = CommonCampaignRating.GetMonoILRComponent<CommonRatingDialogLT>();
                    ladder_rating.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                    ladder_rating.onShownAnimCompleted = OnRatingShownAnimFinished;
                    CommonCampaignRating.CustomSetActive(true);
                    timer = 3;
                    if(win)FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.ladder_topic,
                        FusionTelemetry.GamePlayData.ladder_event_id,FusionTelemetry.GamePlayData.ladder_umengId,"reward");
                    if (LadderManager .Instance .IsTrusteeship) sequence = ILRTimerManager.instance.AddTimer(1000, 10, delegate { AntoContinueClick(); });
                    break;
                default:
                    if(_battleType==eBattleType.SleepTower&&win) FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.sleep_topic,
                        FusionTelemetry.GamePlayData.sleep_event_id,FusionTelemetry.GamePlayData.sleep_umengId,"reward");
                    CommonRatingDialogLT common_rating = CommonCampaignRating.GetMonoILRComponent<CommonRatingDialogLT>();
                    common_rating.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                    common_rating.onShownAnimCompleted = OnRatingShownAnimFinished;
                    CommonCampaignRating.CustomSetActive(true);
                    break;
            }
        }
        private int timer = 5;
        private int sequence = 0;
        /// <summary>
        /// 
        /// </summary>
        /// 
        public void AntoContinueClick()
        {
            timeTips.gameObject.CustomSetActive(true);
            timeTips.text = timer.ToString();
            if (timer == 0)
            {
                timeTips.gameObject.CustomSetActive(false);
                OnContinueClick();
            }
            timer--;
        }

        void OnRatingShownAnimFinished()
        {
            _isShowingRating = true;
            ScreenCollider.CustomSetActive(true);
            ContinueTips.gameObject.CustomSetActive(true);
        }

        
        bool ladderRatingOver = false;
        bool heroBattleRatingOver = false;
        bool honorArenaOver = false;

        public void OnContinueClick()
        {
            if (sequence != 0)
            {
                timeTips.gameObject.CustomSetActive(false);
                ILRTimerManager.instance.RemoveTimer(sequence);
                sequence = 0;
            }

            switch (_battleType)
            {
                case eBattleType.LadderBattle:
                    LadderRatingDialogMH ratingDlg_LadderBattle = LadderRating.GetMonoILRComponent<LadderRatingDialogMH>();
                    if (!LadderRating.gameObject.activeSelf)
                    {
                        DisableAllRating();
                        ratingDlg_LadderBattle.IsWon = m_CombatResult == eCombatOutcome.Win ? true : false;
                        ratingDlg_LadderBattle.onShownAnimCompleted = delegate ()
                        {
                            ladderRatingOver = true;
                            ContinueTips.gameObject.CustomSetActive(true);
                        };
                        LadderRating.CustomSetActive(true);
                        ContinueTips.gameObject.CustomSetActive(false);

                        if (LadderManager.Instance.IsTrusteeship)
                        {
                            timer = 3;
                            sequence = ILRTimerManager.instance.AddTimer(1000, 10, delegate { AntoContinueClick(); });
                        }
                    }
                    if (ladderRatingOver)
                    {
                        SafeContinue();
                    }
                    break;
                case eBattleType.AlienMazeBattle:
                    if (m_CombatResult == eCombatOutcome.Lose && !isCombatOut)//非主动的战斗失败，直接离开副本退回到主城
                    {
                        LTInstanceMapModel.Instance.RequestLeaveChapter("", delegate
                        {
                            LTInstanceMapModel.Instance.ClearInstanceData();
                            SafeContinue();
                        });
                    }
                    else
                    {
                        DefaultContinue();
                    }
                    break;
                case eBattleType.HonorArena:
                    if (honorArenaOver)
                    {
                        SafeContinue();
                    }
                    else
                    {
                        ScreenCollider.CustomSetActive(false);
                        ContinueTips.gameObject.CustomSetActive(false);
                        var harc = HonorArenaResult.GetMonoILRComponent<HonorArenaResultController>();
                        harc.ShowTeamInfo(() =>
                        {
                            honorArenaOver = true;
                            OnRatingShownAnimFinished();
                        });
                    }
                    break;
                default:
                    DefaultContinue();
                    break;
            }
        }

        private void DefaultContinue()
        {
            controller.Close();//主动调用关闭结算界面，不然会出现表现异常
            if (m_Rewards == null)
            {
                m_Rewards = m_Rewards != null ? m_Rewards : GetRewardItemDatas();
                if (m_Rewards.Count > 0)
                {
                    ContinueTips.gameObject.CustomSetActive(false);
                    DisableAllRating();
                    DarkObj.CustomSetActive(false);
                    Action callback = delegate ()
                    {
                        if (LTMainHudManager.Instance.isCheckLevelUp)
                        {
                            System.Action cb = delegate ()
                            {
                                SafeContinue();
                            };
                            LTMainHudManager.Instance.CheckLevelUp(cb);
                        }
                        else
                        {
                            SafeContinue();
                        }
                    };
                    //夺宝奇兵特殊处理,添加双倍按钮
                    if (_battleType == eBattleType.GhostBattle)
                    {
                        System.Action act = ()=>{
                           LTMainHudManager.Instance.OnGhostDoubleAward(success=> {
                                List<LTShowItemData> TempList = m_Rewards;
                                if (success)
                                {
                                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, -(int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("GhostDoubleAwardCost"), "夺宝双倍");
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_RECEIVE_DOUBLE_AWARD"));
                                    if (TempList != null) GameUtils.ShowAwardMsgOnlySys(TempList);
                                } 
                            }); 
                        };
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("reward", m_Rewards);
                        ht.Add("callback", callback);
                        ht.Add("showBtn", true);
                        ht.Add("positive", act);
                        ht.Add("positiveDesc", EB.Localizer.GetString("ID_RECEIVE_DOUBLE_AWARD"));
                        ht.Add("costType", "hc");
                        ht.Add("costNum",(int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("GhostDoubleAwardCost"));
                        ht.Add("negative", null);
                        ht.Add("negativeDesc",  EB.Localizer.GetString("ID_RECEIVE_ONCE_AWARD"));
                        GlobalMenuManager.Instance.Open("LTShowDoubleRewardView", ht);
                        if (LTDailyDataManager.Instance.GetActivityCount(9007) <= 0) LTDailyDataManager.Instance.SetDailyDataRefreshState();
                    }
                    else
                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("reward", m_Rewards);
                        ht.Add("callback", callback);
                        GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                    }
                }
                else
                {
                    if (LTMainHudManager.Instance.isCheckLevelUp)
                    {
                        DarkObj.CustomSetActive(false);
                        System.Action cb = delegate ()
                        {
                            SafeContinue();
                        };
                        LTMainHudManager.Instance.CheckLevelUp(cb);
                    }
                    else
                    {
                        SafeContinue();
                    }
                }
            }
        }

        public void SafeContinue()//playstate not get must resume
        {
            if (isFastCombat)//快速战斗
            {
                if (controller != null)
                {
                    controller.Close();
                    DestroySelf();
                    controller.DestroyControllerForm();
                }

                if (_battleType == eBattleType.MainCampaignBattle)
                {
                    if (!LTInstanceMapModel.Instance.NotMainChapterId())
                    {
                        LTInstanceMapModel.Instance.RequestGetChapterState();
                        Hotfix_LT.Messenger.Raise(EventName.MainBattleQuick);
                    }
                }

                if (_battleType==eBattleType.HonorArena)
                {
                    GlobalMenuManager.PopCaches();
                }
                return;
            }
            string state = "";
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
            if (string.IsNullOrEmpty(state) || state.CompareTo("Combat") == 0)
            {
                MainLandLogic.GetInstance().RequestCombatResumeToScene();
            }
            else
            {
                Continue();
            }
        }

        public void Continue()
        {
            if (_isShowingRating)
            {
                FadeOutRating();
            }
            else
            {
                LeaveResultScreen();
            }
        }

        public static void DirectExitCombat()
        {
            string state = "";
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
            if (string.IsNullOrEmpty(state) || state.CompareTo("Combat") == 0)
            {
                MainLandLogic.GetInstance().RequestCombatResumeToScene();
            }
            else
            {
                CombatLogic.Instance.ExitCombat();

                if (Camera.main != null)
                {
                    CombatCamera combat_camera = Camera.main.GetComponent<CombatCamera>();

                    if (combat_camera != null)
                    {
                        combat_camera.DisableBlurEffect();
                        combat_camera.HoldingCamera = false;
                    }
                }

                if (GameStateManager.Instance != null)
                {
                    GameStateLoadGame loadState = GameStateManager.Instance.GetGameState<GameStateLoadGame>();
                    UIStack.Instance.ShowLoadingScreen(() =>
                    {
                        if (loadState != null)
                        {
                            LTHotfixManager.GetManager<SceneManager>().EnterSceneByPlayState();
                        }
                    }, false, true);
                }
            }
        }

        void FadeOutRating()
        {
            _isShowingRating = false;
            ScreenCollider.CustomSetActive(false);
            CombatLogic.Instance.ExitCombat();
            CombatCamera combat_camera = null;

            if (Camera.main != null)
            {
                combat_camera = Camera.main.GetComponent<CombatCamera>();

                if (combat_camera != null)
                {
                    combat_camera.DisableBlurEffect();
                }
            }
            OnVictoryFadedOut();
        }

        void OnVictoryFadedOut()
        {
            CombatCamera combatCamera = null;

            if (Camera.main != null)
            {
                combatCamera = Camera.main.GetComponent<CombatCamera>();

                if (combatCamera != null)
                {
                    combatCamera.HoldingCamera = false;
                }
            }

            VictoryAnimation.CustomSetActive(false);
            DefeatRating.CustomSetActive(false);
            {
                LeaveResultScreen();
            }
        }

        public static void LeaveResultScreen()
        {
            if (GameStateManager.Instance != null)
            {
                GameStateLoadGame loadState = GameStateManager.Instance.GetGameState<GameStateLoadGame>();
                UIStack.Instance.ShowLoadingScreen(() =>
                {
                    if (loadState != null)
                    {
                        LTHotfixManager.GetManager<SceneManager>().EnterSceneByPlayState();
                    }
                }, false, true);
            }
        }

        public void OnReviveRequest(EB.Sparx.Response result)
        {
            if (result.sucessful)
            {
                Continue();
            }
            else if (result.fatal)
            {
                SparxHub.Instance.FatalError(result.localizedError);
            }
            else
            {
                MessageDialog.Show(DataLocalizationLookup.Localize("ID_DIALOG_TITLE_TIPS", DataLocalizationLookup.eLOCALTYPE.Normal),
                    result.localizedError,
                    DataLocalizationLookup.Localize("ID_DIALOG_TITLE_CONFIRM", DataLocalizationLookup.eLOCALTYPE.Normal), null, false, true, true, delegate (int ret)
                {
                    UIStack.Instance.HideLoadingScreen();
                }, NGUIText.Alignment.Center);
            }
        }

        IEnumerator DestroyLater(GameObject go, float timer)
        {
            yield return new WaitForSeconds(timer);

            if (go != null)
            {
                UnityEngine.Object.Destroy(go);
            }
        }

        static public string m_RewardListDataID = "combat.rewards[0].rewardItems";
        private List<LTShowItemData> m_Rewards;
        static public List<LTShowItemData> GetRewardItemDatas()
        {
            int gold = 0;
            float gold_mul = 1;
            DataLookupsCache.Instance.SearchIntByID("combat.rewards[0].gold.quantity", out gold);
            if (!DataLookupsCache.Instance.SearchDataByID<float>("combat.rewards[0].gold.mul", out gold_mul))
            {
                gold_mul = 1;
            }

            ArrayList rewards = null;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>(m_RewardListDataID, out rewards);
            List<LTShowItemData> datas = new List<LTShowItemData>();
            if (rewards != null)
            {
                for (int j = 0; j < rewards.Count; j++)
                {
                    string id = EB.Dot.String("data", rewards[j], "");
                    int num = EB.Dot.Integer("quantity", rewards[j], 1);
                    float mul = (float)EB.Dot.Double("mul", rewards[j], 1);
                    string type = EB.Dot.String("type", rewards[j], LTShowItemType.TYPE_GAMINVENTORY);
                    bool fromWish = EB.Dot.Bool("wishReward", rewards[j], false);

                    int index = datas.FindIndex(delegate (LTShowItemData data)
                    {
                        if (data.id == id) return true;
                        return false;
                    });

                    if (index < 0)
                    {
                        datas.Add(new LTShowItemData(id, num, type, false, mul, fromWish));
                    }
                    else
                    {
                        datas[index].count = datas[index].count + num;
                    }
                }

                if (gold > 0)
                {
                    var item = new LTShowItemData("gold", gold, LTShowItemType.TYPE_RES, false, gold_mul);
                    datas.Add(item);
                    FusionTelemetry.ItemsUmengCurrency(item, "战斗结算");
                }
            }

            return datas;
        }

        public void OnAnalyseBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTBattleAnalyseUI");
        }
    }
}
