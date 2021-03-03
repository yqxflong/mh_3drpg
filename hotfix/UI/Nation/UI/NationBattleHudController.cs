using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class NationBattleHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TargetTerritoryIconSprite = t.GetComponent<UISprite>("Content/Top/TargetNationInfo/Icon");
            TargetTerritoryNameLabel = t.GetComponent<UILabel>("Content/Top/TargetNationInfo/Name");
            TargetTerritoryHPSprite = t.GetComponent<UISprite>("Content/Top/TargetNationInfo/Hp");
            TargetTerritoryHPSpriteMask = t.GetComponent<UISprite>("Content/Top/TargetNationInfo/Hp/Hp (1)");
            TargetTerritoryHPTransitionSprite = t.GetComponent<UISprite>("Content/Top/TargetNationInfo/HpLerp");
            TargetTerritoryHPLabel = t.GetComponent<UILabel>("Content/Top/TargetNationInfo/HpValue");
            LeftTerritoryFlag = t.GetComponent<UISprite>("Content/TerritoryModel/Left/Main");
            RightTerritoryFlag = t.GetComponent<UISprite>("Content/TerritoryModel/Right/Main");
            LeftTimeLabel = t.GetComponent<UILabel>("Content/TopRight/LeftTimeLabel/LeftTimeValue");
            SkillTipsNameLabel = t.GetComponent<UILabel>("SkillTipsPanel/TweenHUD/SkillName");
            SkillTipsDescLabel = t.GetComponent<UILabel>("SkillTipsPanel/TweenHUD/SkillDesc");
            SkillTipsPanel = t.GetComponent<Transform>("SkillTipsPanel");
            SkillTipsXOffset = 550f;
            SkillTipsYOffset = -150f;
            AttackPathTabGrid = t.FindEx("Content/AttackPathTabGrid").gameObject;
            DefendPathTabGrid = t.FindEx("Content/DefendPathTabGrid").gameObject;
            ChatHint = t.FindEx("Content/TopLeft/ChatButton/RedPoint").gameObject;

            SkillBtns = new UIButton[3];
            SkillBtns[0] = t.GetComponent<UIButton>("Content/BottomRight/SkillList/0");
            SkillBtns[1] = t.GetComponent<UIButton>("Content/BottomRight/SkillList/1");
            SkillBtns[2] = t.GetComponent<UIButton>("Content/BottomRight/SkillList/2");

            StoneThrowerTpl = t.GetMonoILRComponent<StoneThrowerTemplate>("StoneThrower/StoneThrowerTpl");

            StoneThrowerYPosList = new Transform[3];
            StoneThrowerYPosList[0] = t.GetComponent<Transform>("StoneThrower/StoneThrowerYPos/Up");
            StoneThrowerYPosList[1] = t.GetComponent<Transform>("StoneThrower/StoneThrowerYPos/Middle");
            StoneThrowerYPosList[2] = t.GetComponent<Transform>("StoneThrower/StoneThrowerYPos/Down");

            StoneThrowerOffsetX = 0.15f;
            DamageHUD = t.GetMonoILRComponent<NationBattleDamageHUD>("DamagesHUD");
            KingSkillFX = t.FindEx("SkillFX/King").gameObject;
            TowerShootAnimFX = t.FindEx("SkillFX/Marshal/PTAnim").gameObject;
            TowerShootFX = t.FindEx("SkillFX/Marshal/PTFX").gameObject;
            TowerBulletFX = t.FindEx("SkillFX/Marshal/PDFX").gameObject;
            SpeedTipsLeft = t.FindEx("SpeedTips/Left").gameObject;
            SpeedTipsRight = t.FindEx("SpeedTips/Right").gameObject;
            ShellShootPosXOffset = 0.12f;
            ShellShootPosYOffset = -0.03f;
            ShellLeftBorder = t.GetComponent<Transform>("StoneThrower/LeftShellBorder");
            ShellRightBorder = t.GetComponent<Transform>("StoneThrower/RightShellBorder");
            CityMildDamagedFX = t.FindEx("CityFX/TargetState/Mild").gameObject;
            CitySeriousDamagedFX = t.FindEx("CityFX/TargetState/Serious").gameObject;
            CityHitFX = t.GetComponent<Transform>("CityFX/HitFX");
            LeftCityHitEffect = t.GetComponent<UITweener>("CityFX/LeftCityHitEffect");
            RightCityHitEffect = t.GetComponent<UITweener>("CityFX/RightCityHitEffect");

            UpPathPositionList = new List<Transform>();
            UpPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/UpStart"));
            UpPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/UpLeft"));
            UpPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/UpRight"));
            UpPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/UpEnd"));

            MiddlePathPositionList = new List<Transform>();
            MiddlePathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/MiddleStart"));
            MiddlePathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/MiddleEnd"));

            DownPathPositionList = new List<Transform>();
            DownPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/DownStart"));
            DownPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/DownLeft"));
            DownPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/DownMiddle"));
            DownPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/DownRight"));
            DownPathPositionList.Add(t.GetComponent<Transform>("UIFrameBG/FrameFG/Quad/Path/DownEnd"));

            ModelContainer = t.FindEx("ModelContainer").gameObject;
            MySelfArrowFlag = t.GetComponent<Transform>("MySelfFlag");

            RoleShadowMy = t.GetComponent<Transform>("RoleShadowMy");
            RoleShadowOther = t.GetComponent<Transform>("RoleShadowOther");
            RoleShadowContainer = t.GetComponent<Transform>("ShadowContainer");

            BattleFlagTemplate = t.FindEx("BattleFlag").gameObject;
            BattleFlagContainer = t.FindEx("BattleFlagContainer").gameObject;
            SkillCasterContainer = t.FindEx("StoneThrower/SkillModelContainer").gameObject;

            CollisionDistance = 0.03f;
            PlayCharacterCollisionFXTime = 0.45f;
            BattleFlagYOffset = 0f;
            CharacterCollisionFX = t.FindEx("CharCollisionFX").gameObject;
            ScoreBoardCtrl = t.GetMonoILRComponent<NationBattleScoreBoardController>("Content/TopRight/ScoreBoard");
            ModelRotate = new Vector3(0, 0, 0);
            MySelfArrowOffset = new Vector3(0, 0.25f, 0);
            RoleShadowOffset = new Vector3(-0.04f, 0, 0);
            ModelPos = new Vector3(0, 0, -500f);
            ModelScale = new Vector3(100, 100, 100);
            ModelRotationAttackDir = new Vector3(0, 90, -75);
            ModelRotationDefendDir = new Vector3(0, -90, 75);
            RedColor = new Color(233 / 255f, 25 / 255f, 56 / 255f);
            GreenColor = new Color(96 / 255f, 233 / 255f, 56 / 255f);

            IsContinueWalk = false;
            sExitField = false;

            AttOrDefRPObjs = new List<GameObject>();
            AttOrDefRPObjs.Add(t.FindEx("Content/AttackPathTabGrid/0/RedPoint").gameObject);
            AttOrDefRPObjs.Add(t.FindEx("Content/AttackPathTabGrid/1/RedPoint").gameObject);
            AttOrDefRPObjs.Add(t.FindEx("Content/AttackPathTabGrid/2/RedPoint").gameObject);
            AttOrDefRPObjs.Add(t.FindEx("Content/DefendPathTabGrid/0/RedPoint").gameObject);
            AttOrDefRPObjs.Add(t.FindEx("Content/DefendPathTabGrid/1/RedPoint").gameObject);
            AttOrDefRPObjs.Add(t.FindEx("Content/DefendPathTabGrid/2/RedPoint").gameObject);

            battleReadyRP = t.FindEx("Content/BottomLeft/EmbattleBtn/RedPoint").gameObject;
            ShellMoveSpeed = 2.5f;
            ShellMoveDistance = 1.5f;
            MyNationName = t.GetComponent<UILabel>("GMPanel/MyNation/MyNation (1)");
            GMPanel = t.FindEx("GMPanel").gameObject;
            controller.backButton = t.GetComponent<UIButton>("Content/TopLeft/CancelBtn");

            t.GetComponent<UIButton>("Content/TopLeft/ChatButton").onClick.Add(new EventDelegate(OnChatBtnClick));
            t.GetComponent<UIButton>("Content/TopLeft/FriendButton").onClick.Add(new EventDelegate(OnFriendBtnClick));
            t.GetComponent<UIButton>("Content/TopRight/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Content/BottomLeft/EmbattleBtn").onClick.Add(new EventDelegate(OnEmbattleBtnClick));
            t.GetComponent<UIButton>("Content/BottomRight/DefendButton").onClick.Add(new EventDelegate(OnDefendBtnClick));
            t.GetComponent<UIButton>("Content/AttackButton").onClick.Add(new EventDelegate(OnAttackBtnClick));
            t.GetComponent<UIButton>("Content/AttackPathTabGrid/0").onClick.Add(new EventDelegate(() => OnAttackPathBtnClick(t.FindEx("Content/AttackPathTabGrid/0").gameObject)));
            t.GetComponent<UIButton>("Content/AttackPathTabGrid/1").onClick.Add(new EventDelegate(() => OnAttackPathBtnClick(t.FindEx("Content/AttackPathTabGrid/1").gameObject)));
            t.GetComponent<UIButton>("Content/AttackPathTabGrid/2").onClick.Add(new EventDelegate(() => OnAttackPathBtnClick(t.FindEx("Content/AttackPathTabGrid/2").gameObject)));
            t.GetComponent<UIButton>("Content/DefendPathTabGrid/0").onClick.Add(new EventDelegate(() => OnDefendPathBtnClick(t.FindEx("Content/DefendPathTabGrid/0").gameObject)));
            t.GetComponent<UIButton>("Content/DefendPathTabGrid/1").onClick.Add(new EventDelegate(() => OnDefendPathBtnClick(t.FindEx("Content/DefendPathTabGrid/1").gameObject)));
            t.GetComponent<UIButton>("Content/DefendPathTabGrid/2").onClick.Add(new EventDelegate(() => OnDefendPathBtnClick(t.FindEx("Content/DefendPathTabGrid/2").gameObject)));
            t.GetComponent<UIButton>("GMPanel/AddRobotPanel/Grid/StoreBtn").onClick.Add(new EventDelegate(AddRobotPersian_Attack));
            t.GetComponent<UIButton>("GMPanel/AddRobotPanel/Grid/StoreBtn (1)").onClick.Add(new EventDelegate(AddRobotRoman_Attack));
            t.GetComponent<UIButton>("GMPanel/AddRobotPanel/Grid/StoreBtn (2)").onClick.Add(new EventDelegate(AddRobotEgypt_Attack));
            t.GetComponent<UIButton>("GMPanel/AddRobotPanel/Grid (1)/StoreBtn").onClick.Add(new EventDelegate(AddRobotPersian_Defend));
            t.GetComponent<UIButton>("GMPanel/AddRobotPanel/Grid (1)/StoreBtn (1)").onClick.Add(new EventDelegate(AddRobotRoman_Defend));
            t.GetComponent<UIButton>("GMPanel/AddRobotPanel/Grid (1)/StoreBtn (2)").onClick.Add(new EventDelegate(AddRobotEgypt_Defend));
            t.GetComponent<UIButton>("GMPanel/WorkPanel/Grid/LeftUp").onClick.Add(new EventDelegate(WorkLeftUp));
            t.GetComponent<UIButton>("GMPanel/WorkPanel/Grid/LeftMiddle").onClick.Add(new EventDelegate(WorkLeftMiddle));
            t.GetComponent<UIButton>("GMPanel/WorkPanel/Grid/LeftDown").onClick.Add(new EventDelegate(WorkLeftDown));
            t.GetComponent<UIButton>("GMPanel/WorkPanel/Grid (1)/RightUp").onClick.Add(new EventDelegate(WorkRightUp));
            t.GetComponent<UIButton>("GMPanel/WorkPanel/Grid (1)/RightMiddle").onClick.Add(new EventDelegate(WorkRightMiddle));
            t.GetComponent<UIButton>("GMPanel/WorkPanel/Grid (1)/RightDown").onClick.Add(new EventDelegate(WorkRightDown));
            t.GetComponent<UIButton>("GMPanel/KingSkill").onClick.Add(new EventDelegate(KingSkill));
            t.GetComponent<UIButton>("GMPanel/YSSkill").onClick.Add(new EventDelegate(MarshalSkill));
            t.GetComponent<UIButton>("GMPanel/JJSkill").onClick.Add(new EventDelegate(GeneralSkill));
            t.GetComponent<UIButton>("GMBtn").onClick.Add(new EventDelegate(OpenGMPanel));

            t.GetComponent<UIEventTrigger>("Content/BottomRight/SkillList/CloseTrigger").onClick.Add(new EventDelegate(OnCloseAttackOrDefendPathSelect));
            t.GetComponent<UIEventTrigger>("Content/AttackPathTabGrid/CloseTrigger").onClick.Add(new EventDelegate(OnCloseAttackOrDefendPathSelect));
            t.GetComponent<UIEventTrigger>("Content/DefendPathTabGrid/CloseTrigger").onClick.Add(new EventDelegate(OnCloseAttackOrDefendPathSelect));
            t.GetComponent<UIEventTrigger>("SkillTipsPanel/UIBlock").onClick.Add(new EventDelegate(OnCloseSkillTipsClick));

            t.GetComponent<ContinueClickCDTrigger>("Content/TopRight/ScoreBoardBtn").m_CallBackPress.Add(new EventDelegate(t.GetMonoILRComponent<NationBattleScoreBoardController>("Content/TopRight/ScoreBoard").OnOpenBtnClick));
            t.GetComponent<ContinueClickCDTrigger>("Content/BottomLeft/CallBtn").m_CallBackPress.Add(new EventDelegate(OnCallUpBtnClick));
            t.GetComponent<ContinueClickCDTrigger>("Content/BottomRight/SkillList/0").m_CallBackPress.Add(new EventDelegate(() => OnSkillClick(t.FindEx("Content/BottomRight/SkillList/0").gameObject)));
            t.GetComponent<ContinueClickCDTrigger>("Content/BottomRight/SkillList/1").m_CallBackPress.Add(new EventDelegate(() => OnSkillClick(t.FindEx("Content/BottomRight/SkillList/1").gameObject)));
            t.GetComponent<ContinueClickCDTrigger>("Content/BottomRight/SkillList/2").m_CallBackPress.Add(new EventDelegate(() => OnSkillClick(t.FindEx("Content/BottomRight/SkillList/2").gameObject)));

            t.GetComponent<PressOrClick>("Content/BottomRight/SkillList/0").m_CallBackPress.Add(new EventDelegate(() => OnLongPressSkill(t.FindEx("Content/BottomRight/SkillList/0").gameObject)));
            t.GetComponent<PressOrClick>("Content/BottomRight/SkillList/1").m_CallBackPress.Add(new EventDelegate(() => OnLongPressSkill(t.FindEx("Content/BottomRight/SkillList/1").gameObject)));
            t.GetComponent<PressOrClick>("Content/BottomRight/SkillList/2").m_CallBackPress.Add(new EventDelegate(() => OnLongPressSkill(t.FindEx("Content/BottomRight/SkillList/2").gameObject)));

            Instance = this;

            UIWidget uiw = LeftCityHitEffect.GetComponent<UIWidget>();
            LeftCityHitEffect.SetOnFinished(delegate ()
            {
                uiw.color = new Color(uiw.color.r, uiw.color.g, uiw.color.b, 0);
            });
            UIWidget uiw2 = RightCityHitEffect.GetComponent<UIWidget>();
            RightCityHitEffect.SetOnFinished(delegate ()
            {
                uiw2.color = new Color(uiw2.color.r, uiw2.color.g, uiw2.color.b, 0);
            });
        }

        public class PathSectionEntry
        {
            public float length;
            public Vector3 leftPosition;
            public Vector3 rightPosition;
        }
        public class PathEntry
        {
            public float moveSpeed;
            public List<PathSectionEntry> sectionList;
            public float length;
        }
        public class TargetPosEntry
        {
            public Vector3 position;
            public Vector3 dir;
        }

        public enum eSkillUseState
        {
            Cannot,
            Can,
            Have
        }
        public override bool IsFullscreen() { return true; }

        static public NationBattleHudController Instance;
        public UISprite TargetTerritoryIconSprite;
        public UILabel TargetTerritoryNameLabel;
        public UISprite TargetTerritoryHPSprite;
        public UISprite TargetTerritoryHPSpriteMask;
        public UISprite TargetTerritoryHPTransitionSprite;
        public UILabel TargetTerritoryHPLabel;
        public UISprite LeftTerritoryFlag, RightTerritoryFlag;
        public UILabel LeftTimeLabel;
        public UILabel SkillTipsNameLabel;
        public UILabel SkillTipsDescLabel;
        public Transform SkillTipsPanel;
        public float SkillTipsXOffset = 1;
        public float SkillTipsYOffset = 2;
        private eSkillUseState[] mSkillUseStates = new eSkillUseState[3];

        public GameObject AttackPathTabGrid;
        public GameObject DefendPathTabGrid;
        public GameObject ChatHint;
        public UIButton[] SkillBtns;
        public StoneThrowerTemplate StoneThrowerTpl;
        public Transform[] StoneThrowerYPosList;
        public float StoneThrowerOffsetX = 0.2f;
        Dictionary<eBattleDirection, StoneThrowerTemplate> StoneThrowerInsDic = new Dictionary<eBattleDirection, StoneThrowerTemplate>();
        public NationBattleDamageHUD DamageHUD;
        //skill fx
        public GameObject KingSkillFX;
        public GameObject TowerShootAnimFX, TowerShootFX, TowerBulletFX;
        public GameObject SpeedTipsLeft, SpeedTipsRight;
        public float ShellShootPosXOffset;
        public float ShellShootPosYOffset;
        public Transform ShellLeftBorder, ShellRightBorder;
        //city fx
        public GameObject CityMildDamagedFX;
        public GameObject CitySeriousDamagedFX;
        public Transform CityHitFX;
        public UITweener LeftCityHitEffect, RightCityHitEffect;

        public List<Transform> UpPathPositionList;
        public List<Transform> MiddlePathPositionList;
        public List<Transform> DownPathPositionList;
        PathEntry mUpPathEntry;
        PathEntry mMiddlePathEntry;
        PathEntry mDownPathEntry;
        public long FullPathDuration = 10;
        public Vector3 ModelRotate;
        public GameObject ModelContainer;
        public Transform MySelfArrowFlag;
        public Vector3 MySelfArrowOffset;
        public Transform RoleShadowMy, RoleShadowOther, RoleShadowContainer;
        public Vector3 RoleShadowOffset;
        public GameObject BattleFlagTemplate;
        public GameObject BattleFlagContainer;
        public GameObject SkillCasterContainer;
        public Vector3 ModelPos;
        public Vector3 ModelScale;
        public Vector3 ModelRotationAttackDir;
        public Vector3 ModelRotationDefendDir;
        public float CollisionDistance = 2;
        public float PlayCharacterCollisionFXTime = 1;
        public float BattleFlagYOffset;
        public GameObject CharacterCollisionFX;

        public NationBattleScoreBoardController ScoreBoardCtrl;
        public Color RedColor;
        public Color GreenColor;
        static TerritoryData mTerritoryData;
        public static bool IsContinueWalk;
        static public TerritoryData TerritoryData { get { return mTerritoryData; } set { mTerritoryData = value; } }
        public static bool sExitField;
        bool mBattleOver;

        private bool RPState = false;
        public List<GameObject> AttOrDefRPObjs;
        public GameObject battleReadyRP;


        public override void OnDestroy()
        {
            Instance = null;
            base.OnDestroy();
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);

            if (NationManager.Instance.Account.TeamList.Count == 0)
            {
                NationManager.Instance.GetTeamInfo(null);
            }

            sExitField = false;
            TerritoryData data = param as TerritoryData;
            bool isAttack = false;
            if (data != null)
            {
                mTerritoryData = data;
                isAttack = mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack;
            }
            else if (mTerritoryData != null)
            {
                isAttack = mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack;
            }
            else
            {
               EB.Debug.LogError("mTerritoryData=null");
            }
            SetUIMode(isAttack);
            SetTerritoryFlag(mTerritoryData);
            SetTerritoryMainUI(mTerritoryData);

            InitPosition();

            InitMovePath();

            if (NationManager.Instance.BattleTimeConfig.InitCountDownTime())
                StartCoroutine(CountdownBattleEndTime());
            else
                LeftTimeLabel.transform.parent.gameObject.CustomSetActive(false);
        }
        private int[] sequences = new int[3];
        public override IEnumerator OnAddToStack()
        {
            NationManager.Instance.ScoreRankList.SetDefendNation(NationBattleHudController.TerritoryData.Owner);
            NationManager.Instance.EnterField(mTerritoryData.Index, delegate (bool successful) { if (!successful) { controller.Close(); } });
            GameDataSparxManager.Instance.RegisterListener(NationManager.BattleDataId, OnBattleInfoListener);
            SparxHub.Instance.ChatManager.OnMessages += OnMessages;
            FusionAudio.PostBGMEvent("BGM/GuoZhan", true);
            FusionAudio.StartBGM();
            if (IsContinueWalk)
            {
                IsContinueWalk = false;
                NationManager.Instance.ContinueWalk(null);
                DisableOperation(true);
            }

            ScoreBoardCtrl.mDMono.GetComponent<UIPanel>().sortingOrder = ScoreBoardCtrl.mDMono.GetComponent<UIPanel>().sortingOrder + 3;
            NationManager.Instance.PopAttackCityResultPanel();
            SetRP();
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Nation, SetRP);
            sequences[0] = ILRTimerManager.instance.AddTimer(3000,int .MaxValue ,delegate { CheakTeamState(); });
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            for (int i = 0; i < sequences.Length; ++i)
            {
                if (sequences[i] != 0)
                {
                    ILRTimerManager.instance.RemoveTimer(sequences[i]);
                    sequences[i] = 0;
                }
            }
            NationManager.Instance.BattleSyncData.CleanUp();
            NationManager.Instance.ScoreRankList.CleanUp();
            GameDataSparxManager.Instance.UnRegisterListener(NationManager.BattleDataId, OnBattleInfoListener);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Nation, SetRP);
            SparxHub.Instance.ChatManager.OnMessages -= OnMessages;
            FusionAudio.StopBGM();
            MySelfArrowFlag.gameObject.CustomSetActive(false);
            //回收模型
            ClearModelList();
            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();
            SetBattleReadyRP();
        }

        private void SetRP()
        {
            RPState = Nation_AttOrDef();//队伍情况
            for (int i=0;i<AttOrDefRPObjs.Count;++i)
            {
                AttOrDefRPObjs[i].CustomSetActive(RPState);
            }
        }
        private void SetBattleReadyRP()
        {
            bool state = Nation_BattleReady();//布阵情况
            battleReadyRP.CustomSetActive(state);
        }

        private void CheakTeamState()//每3秒检测国战队伍
        {
            if (Nation_AttOrDef() != RPState)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Nation);
            }
        }

        void OnBattleInfoListener(string path, INodeData data)
        {
            NationBattleSyncData syncData = data as NationBattleSyncData;

            if (mBattleOver)
            {
                UpdateHP(syncData.HP, syncData.MaxHP);
                return;
            }

            CreatePath(syncData.UpPathData);
            CreatePath(syncData.MiddlePathData);
            CreatePath(syncData.DownPathData);
            ActionOver(syncData);

            if (syncData.MaxHP <= 0)
            {
                syncData.MaxHP = mTerritoryData.MaxHP;
            }

            if (syncData.Damage != 0)
            {
                PlayDamage(syncData.Damage);
            }

            UpdateHP(syncData.HP, syncData.MaxHP);

            if (syncData.UseSkillDataUpdated)
            {
                syncData.UseSkillDataUpdated = false;
                SetSkillState(eRanks.king, syncData.UseSkillDatas);
                SetSkillState(eRanks.marshal, syncData.UseSkillDatas);
                SetSkillState(eRanks.general, syncData.UseSkillDatas);
            }

            if (syncData.IsSpeedDataUpdated)
            {
                syncData.IsSpeedDataUpdated = false;
                HandleSpeedEvent(syncData);
            }

            HandleStoneThrower(syncData);
        }

        void CreatePath(BattlePathData pathData)
        {
            if (mBattleOver)
                return;
            if (pathData.AttackDataUpdated && pathData.OnceAttackBattleCellList != null)
            {
                pathData.AttackDataUpdated = false;
                for (var i = 0; i < pathData.OnceAttackBattleCellList.Count; i++)
                {
                    var cell = pathData.OnceAttackBattleCellList[i];
                    var cacheCell = pathData.CacheAttackBattleCellList.Find(m => m.uid == cell.uid);
                    if (cacheCell == null)
                    {
                       EB.Debug.LogError("cacheCell is null for cell{0}", cell);
                        continue;
                    }

                    if (cacheCell.state == eBattleCellState.Walk)
                    {
                        CreateModelAsync(cacheCell);
                    }
                    else if (cacheCell.state == eBattleCellState.Battling)
                    {
                        var defendCellData = pathData.CacheDefendBattleCellList.Find(m => m.uid == cacheCell.battleUID);
                        if (cacheCell.battleFlagTrans == null)
                        {
                            GameObject battleFlagIns = GameUtils.InstantiateEx(BattleFlagTemplate.transform, BattleFlagContainer.transform, cacheCell.direction.ToString());
                            if (cacheCell.modelTrans != null && defendCellData != null)
                            {
                                if (defendCellData.modelTrans != null)
                                    battleFlagIns.transform.position = (cacheCell.modelTrans.position + defendCellData.modelTrans.position) / 2;
                                else
                                    battleFlagIns.transform.position = (GetPosition(cacheCell) + GetPosition(defendCellData)) / 2;
                            }
                            else
                            {
                                battleFlagIns.transform.position = cacheCell.modelTrans != null ? cacheCell.modelTrans.position : GetPosition(cacheCell);
                            }
                            battleFlagIns.transform.position += new Vector3(0, BattleFlagYOffset, 0);
                            cacheCell.battleFlagTrans = battleFlagIns.transform;
                            StartCoroutine(PlayCharacterCollisionFX(cacheCell.battleFlagTrans.position));
                        }


                        if (cacheCell.modelTrans)
                        {
                            DestroyModel(cacheCell);
                        }

                        if (defendCellData != null)
                        {
                            defendCellData.battleFlagTrans = cacheCell.battleFlagTrans;
                            if (defendCellData.modelTrans)
                            {
                                DestroyModel(defendCellData);
                            }
                        }
                        else
                        {
                            //Debug.LogError("defendCellData is null");
                        }
                    }
                    else if (cacheCell.state == eBattleCellState.BattleOver)  //战胜
                    {
                        if (cacheCell.battleFlagTrans)
                        {
                            Object.Destroy(cacheCell.battleFlagTrans.gameObject);
                            cacheCell.battleFlagTrans = null;
                        }
                        var defendCellData = pathData.CacheDefendBattleCellList.Find(m => m.uid == cacheCell.battleUID);
                        if (defendCellData != null)
                        {
                            defendCellData.battleFlagTrans = null;
                        }

                        CreateModelAsync(cacheCell);
                    }
                }
            }
            //defend list 
            if (pathData.DefendDataUpdated && pathData.OnceDefendBattleCellList != null)
            {
                pathData.DefendDataUpdated = false;
                for (var i = 0; i < pathData.OnceDefendBattleCellList.Count; i++)
                {
                    var cell = pathData.OnceDefendBattleCellList[i];
                    var cacheCell = pathData.CacheDefendBattleCellList.Find(m => m.uid == cell.uid);
                    if (cacheCell == null)
                    {
                       EB.Debug.LogError("cacheCell is null for uid:{0}", cell.uid);
                        continue;
                    }

                    if (cacheCell.state == eBattleCellState.Walk)
                    {
                        CreateModelAsync(cacheCell);
                    }
                    else if (cacheCell.state == eBattleCellState.Battling)
                    {
                        var attackCellData = pathData.CacheAttackBattleCellList.Find(m => m.uid == cacheCell.battleUID);

                        if (cacheCell.battleFlagTrans == null)
                        {
                            GameObject battleFlagIns = GameUtils.InstantiateEx(BattleFlagTemplate.transform, BattleFlagContainer.transform, cacheCell.direction.ToString());
                            if (cacheCell.modelTrans != null && attackCellData != null)
                            {
                                if (attackCellData.modelTrans != null)
                                    battleFlagIns.transform.position = (cacheCell.modelTrans.position + attackCellData.modelTrans.position) / 2;
                                else
                                    battleFlagIns.transform.position = (GetPosition(cacheCell) + GetPosition(attackCellData)) / 2;
                            }
                            else
                            {
                                battleFlagIns.transform.position = cacheCell.modelTrans != null ? cacheCell.modelTrans.position : GetPosition(cacheCell);
                            }
                            battleFlagIns.transform.position += new Vector3(0, BattleFlagYOffset, 0);
                            cacheCell.battleFlagTrans = battleFlagIns.transform;
                            StartCoroutine(PlayCharacterCollisionFX(cacheCell.battleFlagTrans.position));
                        }

                        if (cacheCell.modelTrans)
                        {
                            DestroyModel(cacheCell);
                        }

                        if (attackCellData != null)
                        {
                            attackCellData.battleFlagTrans = cacheCell.battleFlagTrans;
                            if (attackCellData.modelTrans)
                            {
                                DestroyModel(attackCellData);
                            }
                        }
                        else
                        {
                            //Debug.LogError("attackCellData is null");
                        }
                    }
                    else if (cacheCell.state == eBattleCellState.BattleOver)
                    {
                        if (cacheCell.battleFlagTrans)
                        {
                            Object.Destroy(cacheCell.battleFlagTrans.gameObject);
                            cacheCell.battleFlagTrans = null;
                        }
                        var attackCellData = pathData.CacheAttackBattleCellList.Find(m => m.uid == cacheCell.battleUID);
                        if (attackCellData != null)
                        {
                            attackCellData.battleFlagTrans = null;
                        }

                        CreateModelAsync(cacheCell);
                    }
                }
            }
        }

        void HandleStoneThrower(NationBattleSyncData syncData)
        {
            for (int i = 0; i < syncData.StoneThrowerDataList.Count; ++i)
            {
                StoneThrowerData data = syncData.StoneThrowerDataList[i];
                if (data.hp <= 0)
                {
                    syncData.StoneThrowerDataList.Remove(data);
                    Vector3 pos = StoneThrowerInsDic[data.dir].mDMono.transform.position;
                    if (StoneThrowerInsDic.ContainsKey(data.dir))
                    {
                        StoneThrowerInsDic[data.dir].SetDeath(data.isFire);
                        StoneThrowerInsDic.Remove(data.dir);
                    }

                    if (data.isFire)
                    {
                        StartCoroutine(PlayTowerFX(NationUtil.GetIsAttack(data.dir), pos));
                    }
                    return;
                }

                if (!StoneThrowerInsDic.ContainsKey(data.dir))
                {
                    StoneThrowerTemplate ins = GameUtils.InstantiateEx(StoneThrowerTpl.mDMono.transform, SkillCasterContainer.transform, "stoneThrower." + data.dir).GetMonoILRComponent<StoneThrowerTemplate>();
                    ins.Init(data);
                    ins.mDMono.transform.position = GetStoneThrownerPos(data.dir);
                    float x = ins.mDMono.transform.position.x + (NationUtil.GetIsAttack(data.dir) ? -StoneThrowerOffsetX : StoneThrowerOffsetX);
                    float y = StoneThrowerYPosList[NationUtil.GetPathIndex(data.dir)].transform.position.y;
                    float z = ins.mDMono.transform.position.z;
                    ins.mDMono.transform.position = new Vector3(x, y, z);
                    StoneThrowerInsDic.Add(data.dir, ins);
                }
                else
                {
                    if (StoneThrowerInsDic[data.dir].Data.endTime != data.endTime)
                    {
                       EB.Debug.Log("reInitStoneThrower dir={0}", data.dir);
                        StoneThrowerInsDic[data.dir].Init(data);
                    }
                    else
                    {
                        StoneThrowerInsDic[data.dir].UpdateHP(data.hp);
                    }
                }
            }
        }

        void HandleSpeedEvent(NationBattleSyncData syncData)
        {
            if (mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack)
            {
                if (EB.Time.Now < syncData.AttackSpeedUPBuffEndTs)
                {
                    SpeedTipsLeft.gameObject.CustomSetActive(true);
                    if (sequences[1] != 0)
                    {
                        ILRTimerManager.instance.RemoveTimer(sequences[1]);
                        sequences[1] = 0;
                    }
                    sequences[1] = ILRTimerManager.instance.AddTimer(((int)syncData.AttackSpeedUPBuffEndTs - EB.Time.Now) * 1000, int.MaxValue,delegate { HideLeftSpeedTips(); });
                }
            }
            else
            {
                if (EB.Time.Now < syncData.DefendSpeedUPBuffEndTs)
                {
                    SpeedTipsRight.gameObject.CustomSetActive(true);
                    if (sequences[2] != 0)
                    {
                        ILRTimerManager.instance.RemoveTimer(sequences[2]);
                        sequences[2] = 0;
                    }
                    sequences[2] = ILRTimerManager.instance.AddTimer(((int)syncData.DefendSpeedUPBuffEndTs - EB.Time.Now) * 1000, int.MaxValue, delegate { HideRightSpeedTips(); });
                }
            }
        }

        void HideLeftSpeedTips()
        {
            SpeedTipsLeft.gameObject.CustomSetActive(false);
        }

        void HideRightSpeedTips()
        {
            SpeedTipsRight.gameObject.CustomSetActive(false);
        }

        [ContextMenu("TestCreateStone")]
        public void TestCreateStone()
        {
            StoneThrowerTemplate ins = GameUtils.InstantiateEx(StoneThrowerTpl.mDMono.transform, SkillCasterContainer.transform, "testStone").GetMonoILRComponent<StoneThrowerTemplate>();
            ins.mDMono.transform.position = GetStoneThrownerPos(eBattleDirection.DownDefend);
            bool isAttack = false;
            float x = ins.mDMono.transform.position.x + (isAttack ? -StoneThrowerOffsetX : StoneThrowerOffsetX);
            float y = StoneThrowerYPosList[2].transform.position.y;
            float z = ins.mDMono.transform.position.z;
            ins.mDMono.transform.position = new Vector3(x, y, z);
        }

        [ContextMenu("TestOpenMenu")]
        public void TestOpenMenu()
        {
            GlobalMenuManager.Instance.Open("LTNationBattleCityVictory");
        }

        Vector3 GetStoneThrownerPos(eBattleDirection dir)
        {
            if (dir == eBattleDirection.UpAttack || dir == eBattleDirection.UpDefend)
            {
                float pathLength = mUpPathEntry.moveSpeed * FullPathDuration;
                float needLength = pathLength * 0.3f;
                List<TargetPosEntry> needPosList = new List<TargetPosEntry>();
                return GetPosition(needLength, mUpPathEntry, dir == eBattleDirection.UpDefend, false, out needPosList);
            }
            else if (dir == eBattleDirection.MiddleAttack || dir == eBattleDirection.MiddleDefend)
            {
                float pathLength = mMiddlePathEntry.moveSpeed * FullPathDuration;
                float needLength = pathLength * 0.3f;
                List<TargetPosEntry> needPosList = new List<TargetPosEntry>();
                return GetPosition(needLength, mMiddlePathEntry, dir == eBattleDirection.MiddleDefend, false, out needPosList);
            }
            else
            {
                float pathLength = mDownPathEntry.moveSpeed * FullPathDuration;
                float needLength = pathLength * 0.3f;
                List<TargetPosEntry> needPosList = new List<TargetPosEntry>();
                return GetPosition(needLength, mDownPathEntry, dir == eBattleDirection.DownDefend, false, out needPosList);
            }
        }

        void ActionOver(NationBattleSyncData syncData)
        {
            if (mBattleOver)
                return;

            ActionOver(syncData.UpPathData, syncData.UpPathActionOverUID, syncData.UpPathActionOverMeetCity);
            ActionOver(syncData.MiddlePathData, syncData.MiddlePathActionOverUID, syncData.MiddlePathActionOverMeetCity);
            ActionOver(syncData.DownPathData, syncData.DownPathActionOverUID, syncData.DownPathActionOverMeetCity);
        }

        void ActionOver(BattlePathData pathData, long uid, bool isMeetCity)  //战败 或 直接到终点
        {
            if (uid > 0)
            {
                BattleCellData attackCellData = pathData.CacheAttackBattleCellList.Find(m => m.uid == uid);
                BattleCellData defendCellData = pathData.CacheDefendBattleCellList.Find(m => m.uid == uid);
                if (attackCellData != null)
                {
                    if (isMeetCity)
                    {
                        PlayCityHitFX(true);
                    }

                    RemoveData(pathData.CacheAttackBattleCellList, attackCellData);

                    if (defendCellData != null)
                       EB.Debug.LogError("ActionOver: attackCellData and defendCellData all not is null");
                }
                else if (defendCellData != null)
                {
                    if (isMeetCity)
                    {
                        PlayCityHitFX(false);
                    }
                    RemoveData(pathData.CacheDefendBattleCellList, defendCellData);
                }
                else
                   EB.Debug.LogError("ActionOver:attackCellData defendCellData all is null");
            }
        }

        void RemoveData(List<BattleCellData> cellDataList, BattleCellData cellData)
        {
            cellDataList.Remove(cellData);
            if (cellData.state == eBattleCellState.Walk)
            {
                if (cellData.modelTrans)
                {
                    DestroyModel(cellData);
                }
            }
            else if (cellData.state == eBattleCellState.Battling)
            {
                if (cellData.battleFlagTrans)
                {
                    Object.Destroy(cellData.battleFlagTrans.gameObject);
                    cellData.battleFlagTrans = null;
                }
            }
            else if (cellData.state == eBattleCellState.BattleOver)
            {
                if (cellData.modelTrans)
                {
                    DestroyModel(cellData);
                }
            }
        }

        void SetUIMode(bool attack)
        {
            AttackPathTabGrid.gameObject.CustomSetActive(false);
            DefendPathTabGrid.gameObject.CustomSetActive(false);
            AttackPathTabGrid.gameObject.CustomSetActive(attack);
            DefendPathTabGrid.gameObject.CustomSetActive(!attack);
        }

        void SetTerritoryFlag(TerritoryData territoryData)
        {
            if (mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack)
            {
                LeftTerritoryFlag.spriteName = NationUtil.NationBattleFlag(NationManager.Instance.Account.NationName);
                RightTerritoryFlag.spriteName = NationUtil.NationBattleFlag(territoryData.Owner);
            }
            else
            {
                LeftTerritoryFlag.gameObject.CustomSetActive(false);
                RightTerritoryFlag.spriteName = NationUtil.NationBattleFlag(NationManager.Instance.Account.NationName);
            }
        }

        void SetTerritoryModel(GameObject territoryRoot, TerritoryData territoryData)
        {
            for (var i = 0; i < territoryRoot.transform.childCount; i++)
            {
                territoryRoot.transform.GetChild(i).gameObject.CustomSetActive(false); 
            }
            string territoryType = NationManager.Instance.Config.TerritoryConfigs[territoryData.Index].Type.ToString();
            Transform territoryTrans = territoryRoot.transform.Find(territoryType);
            if (territoryTrans == null)
            {
               EB.Debug.LogError("territoryData.Type error for type:{0}", territoryType);
                return;
            }
            GameObject territoryGO = territoryTrans.gameObject;
            territoryGO.CustomSetActive(true);

            List<GameObject> roofGOs = new List<GameObject>();
            List<GameObject> roofSmallGOs = new List<GameObject>();
            List<GameObject> gemGOs = new List<GameObject>();
            for (var i = 0; i < territoryGO.transform.childCount; i++)
            {
                Transform t = territoryGO.transform.GetChild(i);
                if (t.gameObject.name == "RoofFlag")
                    roofGOs.Add(t.gameObject);
                else if (t.gameObject.name == "RoofFlagSmall")
                    roofSmallGOs.Add(t.gameObject);
                else if (t.gameObject.name == "LingFlag")
                    gemGOs.Add(t.gameObject);
            }
            roofGOs.ForEach(go => go.GetComponent<UISprite>().spriteName = NationUtil.NationRoofFlag(territoryData.Owner));
            roofSmallGOs.ForEach(go => go.GetComponent<UISprite>().spriteName = NationUtil.NationRoofFlagSmall(territoryData.Owner));
            gemGOs.ForEach(go => go.GetComponent<UISprite>().spriteName = NationUtil.NationGemFlag(territoryData.Owner));
        }

        void SetTerritoryMainUI(TerritoryData territoryData)
        {
            TargetTerritoryIconSprite.spriteName = NationUtil.NationIcon(territoryData.Owner);

            LTUIUtil.SetText(TargetTerritoryNameLabel, EB.Localizer.GetString(territoryData.CityName) /*NationUtil.LocalizeNationName(territoryData.Owner)*/);

            if (territoryData.ADType == eTerritoryAttackOrDefendType.Attack)
            {
                TargetTerritoryHPSprite.color = RedColor;
            }
            else
            {
                TargetTerritoryHPSprite.color = GreenColor;
            }
        }

        void SetSkillState(eRanks skillType, bool[] isUseSkillDatas)
        {
            int skillIndex = 5 - (int)skillType;
            SetSkillState(skillIndex, isUseSkillDatas);
        }

        void SetSkillState(int skillType, bool[] isUseSkillDatas)
        {
            int skillIndex = skillType;
            int skillLevel = 5 - skillIndex;
            int myRankIndex = System.Array.IndexOf(NationUtil.RankArr, NationManager.Instance.Account.Rank);

            if (myRankIndex < skillLevel)
            {
                mSkillUseStates[skillIndex] = eSkillUseState.Cannot;
            }
            else if (!isUseSkillDatas[skillIndex])
            {
                mSkillUseStates[skillIndex] = eSkillUseState.Can;
            }
            else
            {
                mSkillUseStates[skillIndex] = eSkillUseState.Have;
            }

            if (mSkillUseStates[skillIndex] == eSkillUseState.Have)
            {
                UIButton btn = SkillBtns[skillIndex];
                Color useColor = Color.magenta;
                UISprite[] sps = btn.GetComponentsInChildren<UISprite>();

                if (sps != null)
                {
                    for (var i = 0; i < sps.Length; i++)
                    {
                        sps[i].color = useColor;
                    }
                }
            }
        }

        private WaitForSeconds wait1 = new WaitForSeconds(1f);
        IEnumerator CountdownBattleEndTime()
        {
            while (true)
            {
                bool isEnd;
                LTUIUtil.SetText(LeftTimeLabel, NationManager.Instance.BattleTimeConfig.GetCountDownStr(out isEnd));
                if (isEnd)
                {
                    GlobalMenuManager.Instance.Open("LTNationBattleResultUI");
                    yield break;
                }
                yield return wait1;
            }
        }

        void UpdateHP(int hp, int max_hp)
        {
            LTUIUtil.SetText(TargetTerritoryHPLabel, string.Format("{0}/{1}", hp, max_hp));
            if (hp > 0)
            {
                float hpPercent = (float)hp / max_hp;
                if (hpPercent > 1)
                {
                    hpPercent = 1;
                   EB.Debug.LogError("hpPercent > 1");
                }
                int curHpWidth = (int)(889 * hpPercent);
                if (!IsFirstUpdateHp)
                {
                    int perHpWidth = TargetTerritoryHPTransitionSprite.width;
                    if (perHpWidth != curHpWidth)
                    {
                        if (LerpHpCoroutine != null)
                            StopCoroutine(LerpHpCoroutine);
                        if (perHpWidth > curHpWidth)
                        {
                            TargetTerritoryHPSpriteMask.width = TargetTerritoryHPSprite.width = curHpWidth;
                            LerpHpCoroutine = StartCoroutine(LerpHp(perHpWidth, curHpWidth, false));
                        }
                        else
                        {
                            LerpHpCoroutine = StartCoroutine(LerpHp(perHpWidth, curHpWidth, true));
                        }
                    }
                }
                else
                {
                    TargetTerritoryHPTransitionSprite.width = TargetTerritoryHPSpriteMask.width = TargetTerritoryHPSprite.width = curHpWidth;
                }
                IsFirstUpdateHp = false;
                TargetTerritoryHPSprite.gameObject.CustomSetActive(true);
                TargetTerritoryHPSpriteMask.gameObject.CustomSetActive(true);
                TargetTerritoryHPTransitionSprite.gameObject.CustomSetActive(true);

                if (hp == max_hp)
                {
                    CityMildDamagedFX.CustomSetActive(false);
                    CitySeriousDamagedFX.CustomSetActive(false);
                }
                else if (hpPercent >= 0.5f)
                {
                    CityMildDamagedFX.CustomSetActive(true);
                    CitySeriousDamagedFX.CustomSetActive(false);
                }
                else
                {
                    CityMildDamagedFX.CustomSetActive(false);
                    CitySeriousDamagedFX.CustomSetActive(true);
                }
            }
            else
            {
                TargetTerritoryHPSprite.gameObject.CustomSetActive(false);
                TargetTerritoryHPSpriteMask.gameObject.CustomSetActive(false);
                TargetTerritoryHPTransitionSprite.gameObject.CustomSetActive(false);
            }
        }

        Coroutine LerpHpCoroutine;
        bool IsFirstUpdateHp = true;
        IEnumerator LerpHp(float previous_value, float target_value, bool isgrow)
        {
            float lerpTime = 0;
            float transition_value = previous_value;
            while (Mathf.Abs(transition_value - target_value) >= 0.002f)
            {
                transition_value = Mathf.Lerp(previous_value, target_value, 1.0f - Mathf.Cos(lerpTime * Mathf.PI * 0.3f * 2));
                lerpTime += Time.deltaTime;

                if (isgrow)
                {
                    TargetTerritoryHPTransitionSprite.width = (int)transition_value;
                    TargetTerritoryHPSprite.width = (int)transition_value;
                    TargetTerritoryHPSpriteMask.width = (int)transition_value;
                }
                else
                    TargetTerritoryHPTransitionSprite.width = (int)transition_value;
                yield return null;
            }
            if (isgrow)
            {
                TargetTerritoryHPTransitionSprite.width = (int)target_value;
                TargetTerritoryHPSprite.width = (int)target_value;
                TargetTerritoryHPSpriteMask.width = (int)target_value;
            }
            else
                TargetTerritoryHPTransitionSprite.width = (int)target_value;
            LerpHpCoroutine = null;
            yield break;
        }

        public void PlayDamage(int damage)
        {
            FusionAudio.PostEvent("UI/New/ShouJi", true);
            DynamicMonoILR ilr = new DynamicMonoILR();
            ilr.hotfixClassPath = "Hotfix_LT.UI.NationBattleDamageHUD";
            GameObject.Instantiate(ilr, controller.transform);
            NationBattleDamageHUD hud = controller.transform.GetMonoILRComponent<NationBattleDamageHUD>();
            hud.Play(true, damage);
        }

        //should pool 
        IEnumerator PlayCharacterCollisionFX(Vector3 pos)
        {
            if (!NationManager.Instance.IsPushMsg)
                yield break;
            GameObject fxIns = GameUtils.InstantiateEx(CharacterCollisionFX.transform, controller.transform, CharacterCollisionFX.gameObject.name);

            fxIns.transform.position = pos;
            yield return new WaitForSeconds(PlayCharacterCollisionFXTime);
            Object.Destroy(fxIns);
            yield break;
        }

        public float ShellMoveSpeed;
        public float ShellMoveDistance;
        IEnumerator PlayTowerFX(bool isAttackSide, Vector3 pos)
        {
            if (isAttackSide)
            {
                PlayFX(TowerShootAnimFX, pos);
                PlayFX(TowerShootFX, pos);
            }
            else
            {
                PlayFX(TowerShootAnimFX, pos, true);
                PlayFX(TowerShootFX, pos, true);
            }

            Transform shellTrans = PlayFX(TowerBulletFX, pos).transform;
            if (isAttackSide)
            {
                shellTrans.position += new Vector3(ShellShootPosXOffset, -ShellShootPosYOffset, 0);
            }
            else
            {
                shellTrans.position += new Vector3(-ShellShootPosXOffset, -ShellShootPosYOffset, 0);
            }
            if (isAttackSide)
            {
                while (shellTrans.position.x < ShellRightBorder.position.x)
                {
                    float dis = ShellMoveSpeed * Time.deltaTime;
                    //moveDistance += dis;
                    //if (isAttackSide)
                    {
                        shellTrans.position += new Vector3(dis, 0, 0);
                    }
                    //else
                    //{
                    //	shellTrans.position -= new Vector3(dis, 0, 0);
                    //}
                    yield return null;
                }
            }
            else
            {
                while (shellTrans.position.x > ShellLeftBorder.position.x)
                {
                    float dis = ShellMoveSpeed * Time.deltaTime;
                    //moveDistance += dis;
                    shellTrans.position -= new Vector3(dis, 0, 0);
                    yield return null;
                }
            }
            if (isAttackSide)
            {
                PlayFX(CityHitFX.gameObject, shellTrans.position);
            }
            else
            {
                PlayFX(CityHitFX.gameObject, shellTrans.position);
            }
            Object.Destroy(shellTrans.gameObject);
        }

        void PlayCityHitFX(bool isAttackSide)
        {
            if (isAttackSide)
            {
                RightCityHitEffect.ResetToBeginning();
                RightCityHitEffect.PlayForward();
            }
            else
            {
                LeftCityHitEffect.ResetToBeginning();
                LeftCityHitEffect.PlayForward();
            }
        }

        GameObject PlayFX(GameObject fx, Vector3 pos, bool flip = false)
        {
            GameObject fxIns = GameUtils.InstantiateEx(fx.transform, controller.transform, fx.gameObject.name);
            fxIns.transform.position = pos;
            if (flip)
            {
                fxIns.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            return fxIns;
        }

        public void DisableOperation(bool isTrue)
        {
            if (mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack)
            {
                AttackPathTabGrid.gameObject.CustomSetActive(!isTrue);
            }
            else
            {
                DefendPathTabGrid.gameObject.CustomSetActive(!isTrue);
            }
            if (!isTrue)
            {
                SetRP();
            }
        }

        public Vector3 GetPosition(BattleCellData cellData)
        {
            List<TargetPosEntry> nextPosList;
            return GetPosition(cellData, false, out nextPosList);
        }

        public Vector3 GetPosition(BattleCellData cellData, bool needPosList, out List<TargetPosEntry> nextPosList)
        {
            switch (cellData.direction)
            {
                case eBattleDirection.UpAttack:
                    return GetPosition(cellData, mUpPathEntry, false, needPosList, out nextPosList);
                case eBattleDirection.UpDefend:
                    return GetPosition(cellData, mUpPathEntry, true, needPosList, out nextPosList);
                case eBattleDirection.MiddleAttack:
                    return GetPosition(cellData, mMiddlePathEntry, false, needPosList, out nextPosList);
                case eBattleDirection.MiddleDefend:
                    return GetPosition(cellData, mMiddlePathEntry, true, needPosList, out nextPosList);
                case eBattleDirection.DownAttack:
                    return GetPosition(cellData, mDownPathEntry, false, needPosList, out nextPosList);
                case eBattleDirection.DownDefend:
                    return GetPosition(cellData, mDownPathEntry, true, needPosList, out nextPosList);
                default:
                   EB.Debug.LogError("cellData.direction error for direction:{0}", cellData.direction);
                    nextPosList = new List<TargetPosEntry>();
                    return Vector3.zero;
            }
        }

        Vector3 GetPosition(BattleCellData cellData, PathEntry pathEntry, bool isDefend, bool needPosList, out List<TargetPosEntry> nextPosList)
        {
            float needLength = pathEntry.moveSpeed * cellData.speedMulti * cellData.moveTime;
            Vector3 getPos = GetPosition(needLength, pathEntry, isDefend, needPosList, out nextPosList);
            if (getPos != Vector3.zero)
            {
                return getPos;
            }

            if (cellData.modelTrans)
            {
                NationManager.Instance.BattleSyncData.Remove(cellData);
                DestroyModel(cellData);
            }
            else
            {
               EB.Debug.LogError("GetPosition error for:{0}", cellData);
            }

           EB.Debug.LogError("Not Found Postion for{0}", cellData);
            if (isDefend)
            {
                return pathEntry.sectionList[0].leftPosition;
            }
            else
            {
                return pathEntry.sectionList[pathEntry.sectionList.Count - 1].rightPosition;
            }
        }

        Vector3 GetPosition(float needLength, PathEntry pathEntry, bool isDefend, bool needPosList, out List<TargetPosEntry> nextPosList)
        {
            nextPosList = new List<TargetPosEntry>();

            float addLength = 0;
            if (isDefend)
            {
                for (int i = pathEntry.sectionList.Count - 1; i >= 0; --i)
                {
                    PathSectionEntry section = pathEntry.sectionList[i];
                    addLength += section.length;
                    if (needLength <= addLength)
                    {
                        if (needPosList)
                        {
                            for (int nextStartIndex = i; nextStartIndex >= 0; --nextStartIndex)
                            {
                                TargetPosEntry tpEntry = new TargetPosEntry();
                                var sec = pathEntry.sectionList[nextStartIndex];
                                tpEntry.position = sec.leftPosition;
                                tpEntry.dir = (sec.leftPosition - sec.rightPosition).normalized;
                                nextPosList.Add(tpEntry);
                            }
                        }

                        Vector3 sectionStartPos = section.rightPosition;
                        Vector3 dir = section.leftPosition - section.rightPosition;
                        Vector3 littleMove = dir.normalized * (needLength - addLength + section.length);
                        return sectionStartPos + littleMove;
                    }
                }
            }
            else
            {
                for (int i = 0; i < pathEntry.sectionList.Count; ++i)
                {
                    PathSectionEntry section = pathEntry.sectionList[i];
                    addLength += section.length;
                    if (needLength <= addLength)
                    {
                        if (needPosList)
                        {
                            for (int nextStartIndex = i; nextStartIndex < pathEntry.sectionList.Count; ++nextStartIndex)
                            {
                                TargetPosEntry tpEntry = new TargetPosEntry();
                                var sec = pathEntry.sectionList[nextStartIndex];
                                tpEntry.position = sec.rightPosition;
                                tpEntry.dir = (sec.rightPosition - sec.leftPosition).normalized;
                                nextPosList.Add(tpEntry);
                            }
                        }

                        Vector3 sectionStartPos = section.leftPosition;
                        Vector3 dir = section.rightPosition - section.leftPosition;
                        Vector3 littleMove = dir.normalized * (needLength - addLength + section.length);
                        return sectionStartPos + littleMove;
                    }
                }
            }
            return Vector3.zero;
        }

        void InitPosition()
        {
            controller.transform.localPosition = new Vector3(0, 0, 1000);
        }

        void InitMovePath()
        {
            InitPath(UpPathPositionList, ref mUpPathEntry);
            InitPath(MiddlePathPositionList, ref mMiddlePathEntry);
            InitPath(DownPathPositionList, ref mDownPathEntry);
        }

        void InitPath(List<Transform> posList, ref PathEntry pathEntry)
        {
            pathEntry = pathEntry ?? new PathEntry();
            float total_magnitude = 0;
            pathEntry.sectionList = new List<PathSectionEntry>();
            for (int positionIndex = 0; positionIndex < posList.Count - 1; ++positionIndex)
            {
                PathSectionEntry sectionEntry = new PathSectionEntry();
                float magnitude = (posList[positionIndex + 1].transform.position - posList[positionIndex].transform.position).magnitude;
                if (magnitude < 0)
                   EB.Debug.LogError("magnitude < 0");
                sectionEntry.length = magnitude;
                sectionEntry.leftPosition = posList[positionIndex].position;
                sectionEntry.rightPosition = posList[positionIndex + 1].position;
                pathEntry.sectionList.Add(sectionEntry);
                total_magnitude += magnitude;
            }
            pathEntry.moveSpeed = total_magnitude / FullPathDuration;
        }

        private Coroutine CreateModelAsync(BattleCellData cellData)
        {
            if (cellData.modelTrans)
            {
               EB.Debug.LogError("model has create for cell:{0}", cellData.ToString());
                return null;
            }
            else
                return EB.Coroutines.Run(CreateModelCoroutine(cellData));
        }

        private IEnumerator CreateModelCoroutine(BattleCellData cellData)
        {
            string prefab_path = "Bundles/Player/Variants/" + cellData.model + "-I";

            var listener = this;
            GameObject variantObj = null;
            Coroutine coroutine = PoolModel.GetModelAsync(prefab_path, Vector3.zero, Quaternion.identity, delegate (Object obj, object param)
            {
                variantObj = obj as GameObject;
                if (variantObj == null)
                {
                    return;
                }

                if (listener == null)
                {
                    PoolModel.DestroyModel(variantObj);
                    return;
                }
                InitModel(variantObj, cellData);

                modelList.Add(cellData);
            }, null);

            yield return coroutine;
        }

        private void InitModel(GameObject variantObj, BattleCellData cellData)
        {
            cellData.ObjectNmae = variantObj.gameObject.name;
            variantObj.gameObject.name = cellData.ToString();
            variantObj.transform.parent = ModelContainer.transform;
            variantObj.transform.localScale = Vector3.one;
            variantObj.transform.localRotation = Quaternion.identity;
            CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
            variant.InstantiateCharacter();

            GameObject character = variant.CharacterInstance;
            character.transform.parent = variant.transform;
            character.transform.localScale = ModelScale;
            if (NationUtil.GetIsAttack(cellData.direction))
                character.transform.localRotation = Quaternion.Euler(ModelRotationAttackDir);
            else
                character.transform.localRotation = Quaternion.Euler(ModelRotationDefendDir);
            character.transform.localPosition = Vector3.zero;

            SetObjLayer(character, GameEngine.Instance.uiLayer);

            character.GetComponent<MoveEditor.FXHelper>().CanPlayParticle = false;
            MoveController mc = character.GetComponent<MoveController>();
            System.Action fn = ()=>{
                mc.TransitionTo(MoveController.CombatantMoveState.kLocomotion);
            };
            if(!mc.IsInitialized){
                mc.RegisterInitSuccCallBack(fn);
            }
            else{
                fn();
            }


            cellData.modelTrans = variantObj.transform;
            List<TargetPosEntry> nextPosList;
            variantObj.transform.position = GetPosition(cellData, true, out nextPosList);
            float moveSpeed = 0;
            switch (cellData.direction)
            {
                case eBattleDirection.UpAttack:
                case eBattleDirection.UpDefend:
                    moveSpeed = mUpPathEntry.moveSpeed * cellData.speedMulti;
                    break;
                case eBattleDirection.MiddleAttack:
                case eBattleDirection.MiddleDefend:
                    moveSpeed = mMiddlePathEntry.moveSpeed * cellData.speedMulti;
                    break;
                case eBattleDirection.DownAttack:
                case eBattleDirection.DownDefend:
                    moveSpeed = mDownPathEntry.moveSpeed * cellData.speedMulti;
                    break;
            }
            variantObj.AddComponent<DynamicMonoILR>().hotfixClassPath = "Hotfix_LT.UI.NationBattleModelHelper";
            NationBattleModelHelper modelHelper = variantObj.GetMonoILRComponent<NationBattleModelHelper>();
            GameObject shadowIns;
            Transform useRoleShadow;
            if (mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack)
            {
                useRoleShadow = NationUtil.GetIsAttack(cellData.direction) ? RoleShadowMy : RoleShadowOther;
            }
            else
            {
                useRoleShadow = !NationUtil.GetIsAttack(cellData.direction) ? RoleShadowMy : RoleShadowOther;

            }
            shadowIns = GameUtils.InstantiateEx(useRoleShadow, RoleShadowContainer, useRoleShadow.name);
            shadowIns.transform.position = variantObj.transform.position;
            Vector3 offsetValue = NationUtil.GetIsAttack(cellData.direction) ? RoleShadowOffset : -RoleShadowOffset;
            cellData.shadowTrans = shadowIns.transform;

            modelHelper.SetData(cellData, moveSpeed, nextPosList, CollisionDistance, shadowIns.transform, offsetValue);
            if (cellData.uid == LoginManager.Instance.LocalUser.Id.Value)
            {
                MySelfArrowFlag.transform.position = variantObj.transform.position;
                MySelfArrowFlag.transform.position += MySelfArrowOffset;
                MySelfArrowFlag.gameObject.CustomSetActive(true);
                modelHelper.SetSelfFlag(MySelfArrowFlag, MySelfArrowOffset);
            }
        }

        void SetObjLayer(GameObject obj, int layer)
        {
            obj.layer = layer;
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].gameObject.layer = layer;
            }
        }

        private List<BattleCellData> modelList = new List<BattleCellData>();
        private HashSet<string> resourceList = new HashSet<string>();
        private void ClearModelList()
        {
            for (int i = modelList.Count - 1; i >= 0; i--)
            {
                if (modelList[i] != null && modelList[i].modelTrans != null)
                {
                    Object.Destroy(modelList[i].modelTrans.GetMonoILRComponent<SmallPartnerModelData>().mDMono);
                    Object.Destroy(modelList[i].shadowTrans.gameObject);
                    if (modelList[i].battleFlagTrans)
                    {
                        Object.Destroy(modelList[i].battleFlagTrans.gameObject);
                        modelList[i].battleFlagTrans = null;
                    }
                    SetObjLayer(modelList[i].modelTrans.gameObject, GameEngine.Instance.defaultLayer);
                    modelList[i].modelTrans.gameObject.name = modelList[i].ObjectNmae;
                    PoolModel.DestroyModel(modelList[i].modelTrans.gameObject);
                }
            }
            modelList.Clear();
        }

        void DestroyModel(BattleCellData cellData)
        {
            if (cellData.modelTrans != null)
            {
                MoveEditor.FXHelper fxHelper = cellData.modelTrans.GetComponent<MoveEditor.FXHelper>();
                if (fxHelper != null)
                {
                    fxHelper.StopAll(true);
                }
            }

            CharacterVariant variant = cellData.modelTrans.gameObject.GetComponentInChildren<CharacterVariant>();
            variant.name = cellData.ObjectNmae;
            variant.Recycle();
            PoolModel.DestroyModel(variant.gameObject);
            cellData.modelTrans = null;

            Object.Destroy(cellData.shadowTrans.gameObject);

            if (cellData.uid ==LoginManager.Instance.LocalUser.Id.Value)
            {
                MySelfArrowFlag.gameObject.CustomSetActive(false);
            }
            modelList.Remove(cellData);

        }

        public void SetBattleOver()
        {
            mBattleOver = true;
            DestroyPath(NationManager.Instance.BattleSyncData.UpPathData.CacheAttackBattleCellList);
            DestroyPath(NationManager.Instance.BattleSyncData.UpPathData.CacheDefendBattleCellList);
            DestroyPath(NationManager.Instance.BattleSyncData.MiddlePathData.CacheAttackBattleCellList);
            DestroyPath(NationManager.Instance.BattleSyncData.MiddlePathData.CacheDefendBattleCellList);
            DestroyPath(NationManager.Instance.BattleSyncData.DownPathData.CacheAttackBattleCellList);
            DestroyPath(NationManager.Instance.BattleSyncData.DownPathData.CacheDefendBattleCellList);
        }

        void DestroyPath(List<BattleCellData> pathCellDataList)
        {
            for (var i = 0; i < pathCellDataList.Count; i++)
            {
                var some = pathCellDataList[i];

                if (some.modelTrans != null)
                {
                    DestroyModel(some);
                }
                else if (some.battleFlagTrans)
                {
                    Object.Destroy(some.battleFlagTrans.gameObject);
                    some.battleFlagTrans = null;
                }
            }
        }

        public void OnChatBtnClick()
        {
            GlobalMenuManager.Instance.Open("ChatHudView", ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION);
            ChatHint.CustomSetActive(false);
        }

        private void OnMessages(EB.Sparx.ChatMessage[] msgs)
        {
            if (ChatHint != null)
                ChatHint.CustomSetActive(true);
        }

        public void OnFriendBtnClick()
        {
            GlobalMenuManager.Instance.Open("FriendHud");
            ChatHint.CustomSetActive(false);
        }

        public void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_NATION_BATTLE_FIELD_RULE");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public void OnSkillClick(GameObject type)
        {
            int index = int.Parse(type.name);
            if (mSkillUseStates[index] == eSkillUseState.Cannot)
            {
                string needRank = "";
                if (index == 0)
                {
                    needRank = EB.Localizer.GetString("ID_codefont_in_NationUtil_king");
                }
                else if (index == 1)
                {
                    needRank = EB.Localizer.GetString("ID_codefont_in_NationUtil_marshal");
                }
                else if (index == 2)
                {
                    needRank = EB.Localizer.GetString("ID_codefont_in_NationUtil_general");
                }
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_53218"), needRank));
            }
            else if (mSkillUseStates[index] == eSkillUseState.Can)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_53393"), delegate (int result)
                {
                    if (result == 0)
                    {
                        NationManager.Instance.BattleSyncData.UseSkillDatas[index] = true;
                        SetSkillState(index, NationManager.Instance.BattleSyncData.UseSkillDatas);
                        NationManager.Instance.SetSkill(index, delegate (bool isSuccessful)
                        {
                            if (isSuccessful)
                            {
                                if (index == 0)
                                {
                                    FusionAudio.PostEvent("UI/New/GuoWang", true);
                                    PlayFX(KingSkillFX, Vector3.zero);
                                }
                                else if (index == 1)
                                {
                                    FusionAudio.PostEvent("UI/New/YuanShuai", true);
                                }
                                else
                                {
                                    FusionAudio.PostEvent("UI/New/JiangJun", true);
                                }
                            }
                        });
                    }
                });
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_54033"));
            }
        }

        public void OnLongPressSkill(GameObject source)
        {
            int index = int.Parse(source.name);
            string skillName, skillDesc;
            if (index == 0)
            {
                skillName = EB.Localizer.GetString("ID_NATION_BATTLE_SKILL_NAME_KING");
                skillDesc = EB.Localizer.GetString("ID_NATION_BATTLE_SKILL_DESC_KING");
            }
            else if (index == 1)
            {
                skillName = EB.Localizer.GetString("ID_NATION_BATTLE_SKILL_NAME_MARSHAL");
                skillDesc = EB.Localizer.GetString("ID_NATION_BATTLE_SKILL_DESC_MARSHAL");
            }
            else
            {
                skillName = EB.Localizer.GetString("ID_NATION_BATTLE_SKILL_NAME_GENERAL");
                skillDesc = EB.Localizer.GetString("ID_NATION_BATTLE_SKILL_DESC_GENERAL");
            }
            SkillTipsNameLabel.text = skillName;
            SkillTipsDescLabel.text = skillDesc;

            Transform container = SkillTipsPanel.transform.GetChild(0).transform;
            container.transform.position = source.transform.position;
            container.GetComponent<UIWidget>().pivot = UIWidget.Pivot.TopRight;
            container.transform.localPosition += new Vector3(-SkillTipsXOffset, -SkillTipsYOffset, 0);

            SkillTipsPanel.gameObject.CustomSetActive(true);
            UITweener[] ts = SkillTipsPanel.transform.GetComponentsInChildren<UITweener>();

            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    var t = ts[i];
                    t.tweenFactor = 0;
                    t.PlayForward();
                }
            }
        }

        public void OnCloseSkillTipsClick()
        {
            SkillTipsNameLabel.text = "";
            SkillTipsDescLabel.text = "";
            SkillTipsPanel.gameObject.CustomSetActive(false);
            UITweener[] ts = SkillTipsPanel.transform.GetComponentsInChildren<UITweener>();

            if (ts != null)
            {
                for (var i = 0; i < ts.Length; i++)
                {
                    var t = ts[i];
                    t.tweenFactor = 0;
                    t.PlayReverse();
                }
            }
        }

        public void OnAttackBtnClick()
        {
            AttackPathTabGrid.gameObject.CustomSetActive(true);
        }

        public void OnDefendBtnClick()
        {
            DefendPathTabGrid.gameObject.CustomSetActive(true);
        }

        public void OnAttackPathBtnClick(GameObject source)
        {
            if (NationManager.Instance.BattleSyncData.HP <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_56063"));
                return;
            }

            int pathIndex = int.Parse(source.name);
            var ht = Johny.HashtablePool.Claim();
            ht.Add("path", pathIndex);
            ht.Add("isAttack", mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack);
            GlobalMenuManager.Instance.Open("LTNationBattleSelectTeamUI", ht);
        }

        public void OnDefendPathBtnClick(GameObject source)
        {
            if (NationManager.Instance.BattleSyncData.HP <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_56526"));
                return;
            }

            int path = int.Parse(source.name);
            var ht = Johny.HashtablePool.Claim();
            ht.Add("path", path);
            ht.Add("isAttack", false);
            GlobalMenuManager.Instance.Open("LTNationBattleSelectTeamUI", ht);
        }

        public void OnCloseAttackOrDefendPathSelect()
        {
            AttackPathTabGrid.gameObject.CustomSetActive(false);
            DefendPathTabGrid.gameObject.CustomSetActive(false);
        }

        public void OnEmbattleBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTNationBattleFormationUI");
        }

        public void OnCallUpBtnClick()
        {
            if (!NationUtil.IsKing)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_57236"));
                return;
            }

            NationManager.Instance.Call(delegate (bool successful)
            {
                if (successful)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_57431"));
                }
            });
        }

        public override void OnCancelButtonClick()
        {
            if (NationManager.Instance.Actioned)
            {
                if (mTerritoryData.ADType == eTerritoryAttackOrDefendType.Attack)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_57716"), delegate (int result)
                    {
                        if (result == 0)
                        {
                            NationManager.Instance.Actioned = false;
                            sExitField = true;
                            NationManager.Instance.ExitField(mTerritoryData.Index, null);
                            base.OnCancelButtonClick();
                        }
                    });
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_NationBattleHudController_58086"), delegate (int result)
                    {
                        if (result == 0)
                        {
                            NationManager.Instance.Actioned = false;
                            sExitField = true;
                            NationManager.Instance.ExitField(mTerritoryData.Index, null);
                            base.OnCancelButtonClick();
                        }
                    });
                }

            }
            else
            {
                sExitField = true;
                NationManager.Instance.ExitField(mTerritoryData.Index, null);
                base.OnCancelButtonClick();
            }
          
            GlobalMenuManager.Instance.RemoveCache("LTNationBattleEntryUI");
            GlobalMenuManager.Instance.RemoveCache("LTNationBattleHudUI");
        }

        #region debugTool
        public UILabel MyNationName;
        public GameObject GMPanel;

        public void OpenGMPanel()
        {
            if (ILRDefine.DEBUG)
            {
                GMPanel.gameObject.CustomSetActive(!GMPanel.activeSelf);
                if (GMPanel.activeSelf)
                    LTUIUtil.SetText(MyNationName, NationManager.Instance.Account.NationName);
            }
        }

        public void AddRobotPersian_Attack()
        {
            NationManager.Instance.AddRobot("persian", "attack", null);
        }

        public void AddRobotPersian_Defend()
        {
            NationManager.Instance.AddRobot("persian", "defend", null);
        }

        public void AddRobotRoman_Attack()
        {
            NationManager.Instance.AddRobot("roman", "attack", null);
        }

        public void AddRobotRoman_Defend()
        {
            NationManager.Instance.AddRobot("roman", "defend", null);
        }

        public void AddRobotEgypt_Attack()
        {
            NationManager.Instance.AddRobot("egypt", "attack", null);
        }

        public void AddRobotEgypt_Defend()
        {
            NationManager.Instance.AddRobot("egypt", "defend", null);
        }

        public void WorkLeftUp()
        {
            NationManager.Instance.RobotWork("attack", "up", null);
        }

        public void WorkLeftMiddle()
        {
            NationManager.Instance.RobotWork("attack", "median", null);
        }

        public void WorkLeftDown()
        {
            NationManager.Instance.RobotWork("attack", "down", null);
        }

        public void WorkRightUp()
        {
            NationManager.Instance.RobotWork("defend", "up", null);
        }

        public void WorkRightMiddle()
        {
            NationManager.Instance.RobotWork("defend", "median", null);
        }

        public void WorkRightDown()
        {
            NationManager.Instance.RobotWork("defend", "down", null);
        }

        public void KingSkill()
        {
            NationManager.Instance.TestSetSkill(0, null);
        }

        public void MarshalSkill()
        {
            NationManager.Instance.TestSetSkill(1, null);
        }

        public void GeneralSkill()
        {
            NationManager.Instance.TestSetSkill(2, null);
        }
        #endregion

        #region From: RedPointManager
        public bool Nation_BattleReady()
        {
            bool isNotReady = false;
            int TeamCount = 0;
            int PartnerCount = 0;
            int TramMaxCount = NationUtil.TeamNames.Length * 4;

            for (int teamIndex = 0; teamIndex < NationUtil.TeamNames.Length; ++teamIndex)
            {
                List<TeamMemberData> teamMemDataList = LTFormationDataManager.Instance.GetTeamMemList(NationUtil.TeamNames[teamIndex]);
                TeamCount += teamMemDataList.Count;
            }

            var dataList = LTPartnerDataManager.Instance.GetGeneralPartnerList();
            for (var i = 0; i < dataList.Count; i++)
            {
                var data = dataList[i];
                if (data != null && data.Level > 0)
                {
                    PartnerCount++;
                }
            }

            isNotReady = TeamCount != TramMaxCount && (PartnerCount - TeamCount > 0);
            return isNotReady;
        }

        public bool Nation_AttOrDef()
        {
            for (var i = 0; i < NationManager.Instance.Account.TeamList.Count; i++)
            {
                var data = NationManager.Instance.Account.TeamList[i];

                if (data.RealState == eTeamState.Available)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
