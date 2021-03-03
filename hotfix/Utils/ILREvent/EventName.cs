namespace Hotfix_LT
{
    public class EventName
    {
        //福利界面打开
        public const string LTWelfareHudOpen = "LTWelfareHudOpen";

        //回归界面打开
        public const string LTComeBackHudOpen = "LTComeBackHudOpen";

        //商店购买
        public const string StoreBuyEvent = "StoreBuyEvent";

        //PK
        public const string PKCancelEvent = "PKCancelEvent";
        public const string PkRejectEvent = "PkRejectEvent";

        //副本
        public const string LTChallengeInstaceRewardGet = "LTChallengeInstaceRewardGet";
        public const string MapCtrlUpdateNodeEvent = "MapCtrlUpdateNodeEvent";
        public const string ChallengeInstanceBuySucc = "ChallengeInstanceBuySucc";
        public const string LTChallengeInstanceLevelSelect = "LTChallengeInstanceLevelSelect";
        public const string LTBlitzCellTweenEvent = "LTBlitzCellTweenEvent";
        public const string PlayCloudFXCallback = "PlayCloudFXCallback";
        public const string ChallengeBattle = "ChallengeBattle";
        public const string OnEventsDataUpdate = "OnEventsDataUpdate";
        public const string OnLevelDataUpdate = "OnLevelDataUpdate";
        public const string OnResetMapEvent = "OnResetMapEvent";
        public const string OnQuitMapEvent = "OnQuitMapEvent";
        public const string OnChallengeFinished = "OnChallengeFinished";
        public const string OnShowDoorEvent = "OnShowDoorEvent";
        public const string UpDatePraypointUI = "UpDatePraypointUI";
        public const string MainBattleQuick = "MainBattleQuick";
        public const string PlayCloudFxEvent = "PlayCloudFxEvent";
        public const string InstanceViewActionHudLoad = "InstanceViewActionHudLoad";
        public const string ChallengeConfirmFastCombat = "ChallengeConfirmFastCombat";
        //剧情
        public const string OnStoryPlaySucc = "OnStoryPlaySucc";
        //战斗
        public const string CombatHitDamageEvent = "CombatHitDamageEvent";
        public const string CombatDamageEvent = "CombatDamageEvent";
        public const string CombatHealEvent = "CombatHealEvent";

        //所有红点触发刷新，由主界面Onfocus中调用
        public const string AllRedPointDataRefresh = "AllRedPointDataRefresh";

        public const string LegionWar_UpdataFinal="LegionWar_UpdataFinal";
        public const string LegionIconIDEdit="LegionIconIDEdit";
        public const string BountyTask_Select="BountyTask_Select";
        public const string TransferDartEndEvent = "TransferDartEndEvent";
        public const string MailUpdateEvent = "MailUpdateEvent";
        public const string ShowMenu = "ShowMenu";
        public const string CloseMenu = "CloseMenu";
        public const string PreloadMenu = "PreloadMenu";
        public const string PressReleaseEvent = "PressReleaseEvent";
        public const string OnMaxRollUpdate = "OnMaxRollUpdate";
        public const string ChargeFreeGiftReflash = "ChargeFreeGiftReflash";
        
        //觉醒
        public const string AwakeningInstance_UpdataTime="AwakeningInstance_UpdataTime";
        public const string OnEnterTimeChange="OnEnterTimeChange";
        
        //伙伴相关
        public const string OnPartnerSelect = "OnPartnerSelect";
        public const string OnParnerSkillChange = "OnParnerSkillChange";
        public const string OnPartnerLevelUpSucc = "OnPartnerLevelUpSucc";
        public const string OnPartnerTopLevelUpSucc = "OnPartnerTopLevelUpSucc";
        public const string OnPartnerUpGradeSucc = "OnPartnerUpGradeSucc";
        public const string OnPartnerStarUpSucc = "OnPartnerStarUpSucc";
        public const string OnPartnerEquipChange = "OnPartnerEquipChange";
        public const string OnPartnerSummonSucc = "OnPartnerSummonSucc";
        public const string OnPartnerAwakenSucc = "OnPartnerAwakenSucc";
        public const string OnPartnerSkinSelect = "OnPartnerSkinSelect";
        public const string OnParnerGuardChange = "OnParnerGuardChange";
        public const string PartnerWikiRP = "PartnerWikiRP";
        public const string OnRefreshPartnerCellRP = "OnRefreshPartnerCellRP";
        public const string OnPartnerUIRefresh = "OnPartnerUIRefresh";
        public const string PartnerCultivateRP = "PartnerCultivateRP";
        public const string OnPartnerHeroChipChange = "OnPartnerHeroChipChange";
        public const string OnPartnerHoleUpError = "OnPartnerHoleUpError";
        public const string OnPartnerAwakenSuccess = "OnPartnerAwakenSuccess";
        public const string OnPartnerTurnToUpgrade = "OnPartnerTurnToUpgrade";
        public const string OnPartnerRollAniSucc = "OnPartnerRollAniSucc";
        public const string OnPartnerRollAniBreak = "OnPartnerRollAniBreak";
        
        //军团相关
        public const string OnAllianceApplyEvent = "OnAllianceApplyEvent";
        public const string OnLegionDonateSucc = "OnLegionDonateSucc";
        public const string OnLegionDonateTimesChaged = "OnLegionDonateTimesChaged";
        public const string OnLegionDonateChestInfoChange = "OnLegionDonateChestInfoChange";
        public const string LegionSearchMessageCallBack = "LegionSearchMessageCallBack";
        public const string LegionTechSkillLevelUp = "LegionTechSkillLevelUp";
        public const string LegionTechChestUpdate = "LegionTechChestUpdate";
        public const string LegionMercenaryUpdateUI = "LegionMercenaryUpdateUI";
        public const string LegionMercenaryUpdateUIDelay = "LegionMercenaryUpdateUIDelay";
        public const string RedPoint_NPCFunc = "RedPoint_NPCFunc";
        public const string RedPoint_Chat = "RedPoint_Chat";
        public const string RedPoint_Nation = "RedPoint_Nation";
        public const string RedPoint_Friend = "RedPoint_Friend";
        
        //伙伴相关
        public const string OnPartnerSkillBreakItemClick = "OnPartnerSkillBreakItemClick";
        public const string OnPartnerSkillBreakGoodsAddSucc= "OnPartnerSkillBreakGoodsAddSucc";
        public const string OnPartnerSkillBreakGoodsRemoveSucc= "OnPartnerSkillBreakGoodsRemoveSucc";
        public const string onCombatTeamPowerUpdate = "onCombatTeamPowerUpdate"; //主界面数字变化  上报
        public const string onPartnerCombatPowerUpdate = "onPartnerCombatPowerUpdate"; //当前伙伴数字变化 飘字当前伙伴
        public const string onCombatTeamPowerChange = "onCombatTeamPowerChange";  //布阵Team变化 飘字Team
        public const string OnRefreshAllPowerChange = "OnRefreshAllPowerChange";//刷新全部伙伴战力//主队飘字int 属性类型，bool是否飘字
        public const string HonorCombatTeamPowerUpdate = "HonorCombatTeamPowerUpdate";
        
        //好友相关
        public const string FriendItemRedPointRefresh = "FriendItemRedPointRefresh";
        public const string FriendApplyEvent = "FriendApplyEvent";//TODORP
        public const string FriendAddToBlacklistEvent = "FriendAddToBlacklistEvent";
        public const string FriendMessageEvent = "FriendMessageEvent";//TODORP
        public const string OtherPlayerDelectFriend = "OtherPlayerDelectFriend";
        public const string FriendOpenRecentlyEvent = "FriendOpenRecentlyEvent";
        public const string FriendSendVigorEvent = "FriendSendVigorEvent";//TODORP
        public const string FriendSendAllButtonState = "FriendSendAllButtonState";

        //Combat
        public const string OnCombatViewLoaded = "OnCombatViewLoaded";
        public const string ShowBattleResultScreen = "ShowBattleResultScreen";
        public const string GetTeamID = "GetTeamID";
        public const string IsAttackingRightSide = "IsAttackingRightSide";
        public const string GetTurn = "GetTurn";
        public const string GetHp = "GetHp";
        public const string GetTargetPosition = "GetTargetPosition";
        public const string GetTargetRadius = "GetTargetRadius";
        public const string GetAnimator = "GetAnimator";
        public const string GetCurrentMove = "GetCurrentMove";
        public const string GetHitPoint = "GetHitPoint";
        public const string IsCurrentAttackCritical = "IsCurrentAttackCritical";
        public const string GetIngameId = "GetIngameId";
        public const string IsAlive = "IsAlive";
        public const string GetmyName = "GetmyName";
        public const string GetID = "GetID";
        public const string DoDebugAction = "DoDebugAction";
        public const string IsCombatInit = "IsCombatInit";
        public const string IsBattleOver = "IsBattleOver";
        public const string IsReady = "IsReady";
        public const string ExitWatchAsk = "ExitWatchAsk";
        public const string OnCancelButtonClick = "OnCancelButtonClick";
        public const string DisplayCameraViewButton = "DisplayCameraViewButton";
        public const string ClearLog = "ClearLog";
        public const string GetLogCachePages = "GetLogCachePages";
        public const string GetLogCache = "GetLogCache";
        public const string GetDirty = "GetDirty";
        public const string SetDirty = "SetDirty";
        public const string CombatCleanUp = "CombatCleanUp";
        public const string DirectExitCombat = "DirectExitCombat";
        public const string BossParticleInSceneComplete = "BossParticleInSceneComplete ";
        public const string CombatTeamsVictoryEvent = "CombatTeamsVictoryEvent ";
        public const string ArenaRankChangeEvent = "ArenaRankChangeEvent";
        public const string CombatBossCameraEvent = "CombatBossCameraEvent";
        public const string ResumeCombat = "ResumeCombat";
        public const string LogResumeCombat = "LogResumeCombat";

        //热更
        public const string OnLocatorTriggerEnterEvent = "OnLocatorTriggerEnterEvent";
        public const string NewGameConfigGetValue = "NewGameConfigGetValue";
        #region Wallet Listener
        public const string OnBalanceUpdated = "OnBalanceUpdated";
        public const string OnCreditSuceeded = "OnCreditSuceeded";
        public const string OnDebitFailed = "OnDebitFailed";
        public const string OnDebitSuceeded = "OnDebitSuceeded";
        public const string OnOffersFetched = "OnOffersFetched";
        public const string OnOffersFetchSuceeded = "OnOffersFetchSuceeded";
        public const string OnOfferPurchaseFailed = "OnOfferPurchaseFailed";
        public const string OnOfferPurchaseSuceeded = "OnOfferPurchaseSuceeded";
        public const string OnOfferPurchaseCanceled = "OnOfferPurchaseCanceled";
        public const string OnOfferPurchaseRedeemer = "OnOfferPurchaseRedeemer";
        public const string OnOfferPurchaseNoResult = "OnOfferPurchaseNoResult";
        #endregion
        public const string PartnerTakeFieldEvent  = "PartnerTakeFieldEvent";
        public const string InventoryEvent  = "InventoryEvent";
        public const string FuncOpenBtnReflash  = "FuncOpenBtnReflash";
        public const string BossDieEvent = "BossDieEvent";
        public const string OnLoadClick = "OnLoadClick";

        public static string HonorArenaRankChange = "HonorArenaRankChange";
         public static string HonorArenaRankNeedReq = "HonorArenaRankNeedReq";
        public static string HeroBattleUpdateUI = "HeroBattleUpdateUI";
        public static string HeroBattleShowDesc = "HeroBattleShowDesc";
        public const string OnInvitePlayerTaskStateChanged = "OnInvitePlayerTaskStateChanged";
        public const string OnInviteShareSucceed = "OnInviteShareSucceed";

        public const string OnURScoreRewardRecieve = "OnURScoreRewardRecieve";
        public const string OnSSRWishRefresh = "OnSSRWishRefresh";
        public const string ArtifactRefresh = "ArtifactRefresh";
    }
}
