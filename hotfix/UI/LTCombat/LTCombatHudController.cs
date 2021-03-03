using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using DG.Tweening;

/// <summary>
/// 战斗UI界面管理器
/// </summary>
namespace Hotfix_LT.UI
{
    public class LTCombatHudController : UIControllerHotfix, IHotfixUpdate
    {
        public static readonly Color32 MyTeamColor = new Color32(29, 27, 144, 255);
        public static readonly Color32 EnemyTeamColor = new Color32(103, 17, 25, 255);


        [System.Serializable]
        public class ActionCharacterUIItem
        {
            public DynamicUISprite Icon;
            public UISprite TeamColorFrame;
            public UISprite TeamColorFrameBG;
            public UISprite TeamColorBG;
            public UILabel TipsLabel;

            public void SetTeamColor(bool isMyTeam)
            {
                TeamColorFrame.spriteName = isMyTeam ? "Ty_Quality_Blue" : "Ty_Quality_Gules";
                TeamColorFrameBG.color = isMyTeam ? MyTeamColor : EnemyTeamColor;
                TeamColorBG.spriteName = isMyTeam ? "Combat_Flag_Youfang" : "Combat_Flag_Difang";
            }
        }

        public bool testBool = false;
        public int actorNum = 0;
        public GameObject NewStartFX;
        private UILabel RoundInfo;

        private int countdownTime = 0;
        #region UILabel
        private UILabel CurWaveLabel;
        private UILabel[] CurWaveLabel_Sub;
        private UILabel RoundLabel;
        private UILabel[] RoundLabel_Sub;
        private UILabel MagicNumLabel;
        private UILabel[] MagicNumLabel_Sub;
        private UILabel TimeLabel;
        private UILabel CountdownTimeLabel;
        private UILabel CurrentActionerSkillTypeLabel;
        private UILabel CurrentActionerSkillNameLabel;
        private UILabel ActionCountdownLabel;
        #endregion

        #region honor
        private UILabel readTeamName;
        private UILabel blueTeamName;
        private UILabel readTeamWin;
        private UILabel blueTeamWin;

        public List<UISprite> redTeamWins = new List<UISprite>();
        public List<UISprite> blueTeamWins = new List<UISprite>();

        private Color winColor = new Color(0.26f, 1f, 0.27f);
        private Color loseColor = new Color(1f, 0.4f, 0.6f);
        #endregion

        #region ladder
        private GameObject TrusteeshipBtn;
        private GameObject TrusteeshipOpenSprite;
        #endregion


        public GameObject TurnPannel;
        public GameObject CornerAnchorRoot;
        public GameObject ActionOrderGORoot;
        public UIButton AutoButton;
        public UIButton ResetCameraViewBtn;
        public GameObject AutoCombatGrid;
        public UISprite AutoCombatSkillBGSprite;
        public CombatSkillController CombatSkillCtrl;
        public DamgeBuffController DamgeBuffController;
        public LTBossHealthBarController BossHealthBarCtrl;
        public LTCombatMoveIconProgress MoveActorIconProgress;
        public GameObject CurrentActionerSkillItem;
        public UISprite CurrentActionerSkillTypeBG;
        public bool IsCombatOut = false;//是否主动离开战斗
        public GameObject NotSpeedModeUI, SpeedModeUI;
        public GameObject NotAutoModeUI, AutoModeUI;
        public GameObject NotSpeedModeBG;

        private static LTCombatHudController s_instance = null;
        public static LTCombatHudController Instance { get { return s_instance; } }
        public List<ParticleSystemUIComponent> FullScreenFXAssets = new List<ParticleSystemUIComponent>();
        GameObject CurFullScreenFX;
        GameObject honorTitle;

        [System.NonSerialized]
        public List<AutoCombatItem> AutoCombatItems;

        public ActionCharacterUIItem CurrentActionCharacter = new ActionCharacterUIItem();
        public ActionCharacterUIItem ReadyActionCharacter = new ActionCharacterUIItem();
        public ActionCharacterUIItem ThreeActionCharacter = new ActionCharacterUIItem();
        public ActionCharacterUIItem BackActionCharacter = new ActionCharacterUIItem();
        private UITweener[] CurrentActionerSkillItemTws;
        private List<UIEventListener> _dontMoveCameraListeners;
        private bool Ready;
        private bool IsAutoInit;
        private bool PlayVictoryCameraing;

        public override void Awake()
        {
            base.Awake();
            IsCombatOut = false;
            s_instance = this;
            Ready = false;

            var t = controller.transform;

            readTeamName = t.GetComponent<UILabel>("BattleHud/HonorTitle/Left/LName");
            blueTeamName = t.GetComponent<UILabel>("BattleHud/HonorTitle/Right/RName");
            readTeamWin = t.GetComponent<UILabel>("BattleHud/HonorTitle/Left/Lstar/Lstartext");
            blueTeamWin = t.GetComponent<UILabel>("BattleHud/HonorTitle/Right/Rstar/Rstartext");

            redTeamWins.Add(t.GetComponent<UISprite>("BattleHud/HonorTitle/Left/LWin2"));
            redTeamWins.Add(t.GetComponent<UISprite>("BattleHud/HonorTitle/Left/LWin1"));
            redTeamWins.Add(t.GetComponent<UISprite>("BattleHud/HonorTitle/Left/LWin0"));
            blueTeamWins.Add(t.GetComponent<UISprite>("BattleHud/HonorTitle/Right/RWin0"));
            blueTeamWins.Add(t.GetComponent<UISprite>("BattleHud/HonorTitle/Right/RWin1"));
            blueTeamWins.Add(t.GetComponent<UISprite>("BattleHud/HonorTitle/Right/RWin2"));
            honorTitle = t.FindEx("BattleHud/HonorTitle").gameObject;

            controller.backButton = t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftUp/ExitBattleButtonAnchor/CtrlButton");
            controller.hudRoot = t.GetComponent<Transform>();

            NewStartFX = t.FindEx("BattleHud/StartFX").gameObject;
            RoundInfo = t.GetComponent<UILabel>("BattleHud/StartFX/Round");

            FullScreenFXAssets.Add(t.GetComponent<ParticleSystemUIComponent>("BattleHud/FullScreenFXs/FullScreenFX (1)"));
            FullScreenFXAssets.Add(t.GetComponent<ParticleSystemUIComponent>("BattleHud/FullScreenFXs/FullScreenFX (2)"));
            FullScreenFXAssets.Add(t.GetComponent<ParticleSystemUIComponent>("BattleHud/FullScreenFXs/FullScreenFX (4)"));
            FullScreenFXAssets.Add(t.GetComponent<ParticleSystemUIComponent>("BattleHud/FullScreenFXs/FullScreenFX (6)"));

            CurWaveLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/Up/Turn/Wave");
            CurWaveLabel_Sub = CurWaveLabel.GetComponentsInChildren<UILabel>();
            RoundLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/Up/Turn/TurnLabel");
            RoundLabel_Sub = RoundLabel.GetComponentsInChildren<UILabel>();

            TurnPannel = t.FindEx("BattleHud/CornerAnchor/Up/Turn").gameObject;
            TimeLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/RightUp/TimeLabel");
            CountdownTimeLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/Up/CountdownLabel");
            CornerAnchorRoot = t.FindEx("BattleHud/CornerAnchor").gameObject;
            ActionOrderGORoot = t.FindEx("BattleHud/CornerAnchor/RightUp/Container").gameObject;
            AutoButton = t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftDown/ButtonContainer/AutoModeButton");
            ResetCameraViewBtn = t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftDown/ButtonContainer/ResetCameraBtn");
            AutoCombatGrid = t.FindEx("BattleHud/CornerAnchor/SkillPanel/RightDown/AutoCombatGrid").gameObject;
            AutoCombatSkillBGSprite = t.GetComponent<UISprite>("BattleHud/CornerAnchor/SkillPanel/RightDown/AutoCombatBG");
            CombatSkillCtrl = t.GetMonoILRComponent<CombatSkillController>("BattleHud/CornerAnchor/SkillPanel/RightDown/SkillList");
            DamgeBuffController = t.GetMonoILRComponent<DamgeBuffController>("BattleHud/CornerAnchor/LeftUp/DamageBuff");
            BossHealthBarCtrl = t.GetMonoILRComponent<LTBossHealthBarController>("BattleHud/BOSSHUDController");
            MagicNumLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/SkillPanel/RightDown/ScrollBag/Magic/Label");
            MagicNumLabel_Sub = MagicNumLabel.GetComponentsInChildren<UILabel>();

            CurrentActionCharacter.Icon = t.GetComponent<DynamicUISprite>("BattleHud/CornerAnchor/RightUp/Container/Current/Icon");
            CurrentActionCharacter.TeamColorFrame = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Current/TeamFrameColor");
            CurrentActionCharacter.TeamColorFrameBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Current/TeamFrameColor/Sprite");
            CurrentActionCharacter.TeamColorBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Current/BG");
            CurrentActionCharacter.TipsLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/RightUp/Container/Current/Label");

            ReadyActionCharacter.Icon = t.GetComponent<DynamicUISprite>("BattleHud/CornerAnchor/RightUp/Container/Ready/Icon");
            ReadyActionCharacter.TeamColorFrame = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Ready/TeamFrameColor");
            ReadyActionCharacter.TeamColorFrameBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Ready/TeamFrameColor/Sprite");
            ReadyActionCharacter.TeamColorBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Ready/BG");
            ReadyActionCharacter.TipsLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/RightUp/Container/Ready/Label (1)");

            ThreeActionCharacter.Icon = t.GetComponent<DynamicUISprite>("BattleHud/CornerAnchor/RightUp/Container/Three/Icon");
            ThreeActionCharacter.TeamColorFrame = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Three/TeamFrameColor");
            ThreeActionCharacter.TeamColorFrameBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Three/TeamFrameColor/Sprite");
            ThreeActionCharacter.TeamColorBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Three/BG");
            ThreeActionCharacter.TipsLabel = null;

            BackActionCharacter.Icon = t.GetComponent<DynamicUISprite>("BattleHud/CornerAnchor/RightUp/Container/Back/Icon");
            BackActionCharacter.TeamColorFrame = null;
            BackActionCharacter.TeamColorFrameBG = null;
            BackActionCharacter.TeamColorBG = null;
            BackActionCharacter.TipsLabel = null;

            MoveActorIconProgress = t.GetMonoILRComponent<LTCombatMoveIconProgress>("BattleHud/CornerAnchor/RightUp/Container/MoveIconProgress");
            CurrentActionerSkillItem = t.FindEx("BattleHud/CornerAnchor/RightUp/Container/Current/SkillName").gameObject;
            CurrentActionerSkillTypeBG = t.GetComponent<UISprite>("BattleHud/CornerAnchor/RightUp/Container/Current/SkillName/Type");
            CurrentActionerSkillTypeLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/RightUp/Container/Current/SkillName/Type/Label");
            CurrentActionerSkillNameLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/RightUp/Container/Current/SkillName/Label");
            ActionCountdownLabel = t.GetComponent<UILabel>("BattleHud/CornerAnchor/RightUp/Container/Current/Label");
            IsCombatOut = false;
            NotSpeedModeUI = t.FindEx("BattleHud/CornerAnchor/LeftDown/ButtonContainer/SpeedUpButton/Deactive").gameObject;
            SpeedModeUI = t.FindEx("BattleHud/CornerAnchor/LeftDown/ButtonContainer/SpeedUpButton/Active").gameObject;
            NotAutoModeUI = t.FindEx("BattleHud/CornerAnchor/LeftDown/ButtonContainer/AutoModeButton/Deactive").gameObject;
            AutoModeUI = t.FindEx("BattleHud/CornerAnchor/LeftDown/ButtonContainer/AutoModeButton/Active").gameObject;
            NotSpeedModeBG = t.FindEx("BattleHud/CornerAnchor/LeftDown/ButtonContainer/SpeedUpButton/NotSpeedUpBG").gameObject;


            t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftUp/ChatButton").onClick.Add(new EventDelegate(OnChatBtnClick));
            t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftUp/FriendButton").onClick.Add(new EventDelegate(OnFriendBtnClick));
            t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftDown/ButtonContainer/AutoModeButton").onClick.Add(new EventDelegate(onAutoButtonClick));
            t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftDown/ButtonContainer/SpeedUpButton").onClick.Add(new EventDelegate(onSpeedButtonClick));
            t.GetComponent<UIButton>("BattleHud/CornerAnchor/LeftDown/ButtonContainer/ResetCameraBtn").onClick.Add(new EventDelegate(OnResetCameraClick));

            t.GetComponent<UIEventTrigger>("BattleHud/BossHeadInfo/Icon").onClick.Add(new EventDelegate(t.GetMonoILRComponent<LTBossHealthBarController>("BattleHud/BOSSHUDController").OnPopSkillTips));
            t.GetComponent<UIEventTrigger>("BattleHud/BossHeadInfo/NextSkillIcon").onClick.Add(new EventDelegate(t.GetMonoILRComponent<LTBossHealthBarController>("BattleHud/BOSSHUDController").OnPopNextSkillTips));
            t.GetComponent<UIEventTrigger>("BattleHud/CornerAnchor/SkillPanel/RightDown/SkillList/Template/Icon").onClick.Add(new EventDelegate(t.GetMonoILRComponent<CombatSkillItem>("BattleHud/CornerAnchor/SkillPanel/RightDown/SkillList/Template").OnSelectClick));
            t.GetComponent<UIEventTrigger>("BattleHud/BOSSHUDController/Container/SkillList").onClick.Add(new EventDelegate(t.GetMonoILRComponent<LTBossHealthBarController>("BattleHud/BOSSHUDController").OnCloseSkillTips));
            t.GetComponent<UIEventTrigger>("BattleHud/BOSSHUDController/Container/SkillList/SkillGuideBlar").onClick.Add(new EventDelegate(t.GetMonoILRComponent<LTBossHealthBarController>("BattleHud/BOSSHUDController").OnCloseSkillTips));
            t.GetComponent<UIEventTrigger>("BattleHud/BOSSHUDController/Container/Panel/NextSkillTips").onClick.Add(new EventDelegate(t.GetMonoILRComponent<LTBossHealthBarController>("BattleHud/BOSSHUDController").OnCloseNextSkillTips));

            TrusteeshipBtn = t.Find("BattleHud/CornerAnchor/LeftDown/TrusteeshipBtn").gameObject;
            TrusteeshipBtn.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnTrusteeshipBtnClick));
            TrusteeshipOpenSprite = t.Find("BattleHud/CornerAnchor/LeftDown/TrusteeshipBtn/Open").gameObject;

            _dontMoveCameraListeners = new List<UIEventListener>();
            AddMoveCameraCollider(t.gameObject);
            CornerAnchorRoot.gameObject.CustomSetActive(false);
            ActionOrderGORoot.CustomSetActive(false);
            DisplayCameraViewButton(false);
            CurrentActionerSkillItemTws = CurrentActionerSkillItem.GetComponents<UITweener>();
            OnFocus();
            GuideNodeEvent.CombatBtnEvent += CombatBtnEvent;
            honorTitle.CustomSetActive(false);

            Hotfix_LT.Messenger.AddListener< eBattleType , eCombatOutcome , bool > (EventName.ShowBattleResultScreen, ShowBattleResultScreenEx);
            InitSpline();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();

			float rotationalSpeed = Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("CameraRotationalSpeed");
			if (rotationalSpeed > 0) { MyFollowCamera.CAMERA_ROTATIONAL_SPEED = rotationalSpeed; }
		}

        public void Update(){
            UpdateSpline(Time.deltaTime);
            InfiniteCompeteTimeLabelUpdate();
        }

        int m_Timer = 0;
        private void InfiniteCompeteTimeLabelUpdate()
        {
            if (countdownTime > 0)
            {
                if (m_Timer < EB.Time.Now)
                {
                    int timer = EB.Time.Now - countdownTime;
                    int hour = timer / 60;
                    if (hour > 99)
                    {
                        CountdownTimeLabel.text = "99:60";
                    }
                    else
                    {
                        CountdownTimeLabel.text = string.Format("{0}:{1}", hour.ToString("00"), (timer % 60).ToString("00"));
                    }
                    m_Timer = EB.Time.Now;
                }
            }
        }

        public Hashtable GetBasicInfo()
        {
            return new Hashtable() { { "testBool", testBool }, { "actorNum", actorNum }, { "autoMode", AutoMode } };
        }

        public void DamgeBuff(int buff, int type)
        {
            DamgeBuffController.AwakeningSetting(buff, (Hotfix_LT.Data.eRoleAttr)type);
        }

        private AutoCombatItem ActionItem, ActionFinishItem = null;
        public void TransformAutoActionSequence(int actionId)
        {
            int num = AutoCombatItems.Count;

            for (int i = 0; i < num; i++)
            {
                if (AutoCombatItems[i].ItemCharSyncData.IngameId == actionId)
                {
                    ActionItem = AutoCombatItems[i];
                    ActionItem.DoAction();
                    if (ActionFinishItem != null)
                        ActionFinishItem.FinishAction();
                    ActionFinishItem = ActionItem;
                }
            }
        }

        public void SetDeath(int teamId, int ingameId, bool isTrue)
        {
            if (teamId == CombatLogic.Instance.LocalPlayerTeamIndex && AutoMode && AutoCombatItems != null)
            {
                var autoItem = AutoCombatItems.Find(item => item.ItemCharSyncData.IngameId == ingameId);
                if (autoItem != null) autoItem.SetDeath(isTrue);
            }
        }

        public void BossCom()
        {
            HideUIHud(true);
            SettingSomeoneFXVisible();
        }
        public void ResetCurrentHurt(long hurt)
        {
            BossHealthBarCtrl.ResetCurrentHurt(hurt);
        }
        public void SetNextSkill(int skill)
        {
            BossHealthBarCtrl.SetNextSkill(skill);
        }
        public void InitCombatSkillCtrl()
        {
            CombatSkillCtrl.InitData();
        }
        public void ClearConvergeInfo(int IngameId)
        {
            CombatSkillCtrl.ClearConvergeInfo(IngameId);
        }
        #region UITweener -> Spline
        private const float _splineScale_during = 0.25f;
        private const float _splineScale_valueMax = 1.25f;
        private Johny.TimedSplineFloat _splineScale = new Johny.TimedSplineFloat();
        private GameObject _splineScale_go = null;
        private bool _splineScale_forward = true;
        private float _splineScale_timer = 0.0f;
        private void InitSpline(){
            _splineScale.addKey(0.0f, 1.0f);
            _splineScale.addKey(_splineScale_during, _splineScale_valueMax);
            _splineScale.CalculateGradient();
        }

        private void UpdateSpline(float dt){
            if(_splineScale_go == null){
                return;
            }

            if(_splineScale_timer < _splineScale_during){
                _splineScale_timer += dt;
                float realTimer = _splineScale_forward ? _splineScale_timer : (_splineScale_during - _splineScale_timer);
                float scale = _splineScale.getGlobalFrame(Mathf.Clamp(realTimer, 0.0f, _splineScale_during));
                _splineScale_go.transform.localScale = Vector3.one * scale;
                if(_splineScale_timer >= _splineScale_during)
                {
                    if(_splineScale_forward){
                        _splineScale_forward = false;
                        _splineScale_timer = 0.0f;
                    }
                    else{
                        _splineScale_go = null;
                    }
                }
            }
        }

        public void ApplyBounceAnim(GameObject target)
        {
            #region 替换为spline
            // UITweener tween = TweenScale.Begin(target, 0.25f, new Vector3(1.2f,1.2f,1.2f));
            // tween.AddOnFinished(()=>{
            //     TweenScale.Begin(target, 0.25f, new Vector3(1.0f,1.0f,1.0f));
            // });
            #endregion
            _splineScale_go = target;
            _splineScale_forward = true;
            _splineScale_timer = 0.0f;
        }
        #endregion

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener< eBattleType , eCombatOutcome , bool > (EventName.ShowBattleResultScreen, ShowBattleResultScreenEx);
            GuideNodeEvent.CombatBtnEvent -= CombatBtnEvent;
            countdownTime = 0;
            IsCombatOut = false;
            if (_dontMoveCameraListeners != null)
            {
                for (int i = 0; i < _dontMoveCameraListeners.Count; i++)
                {
                    if (_dontMoveCameraListeners[i] != null)
                    {
                        _dontMoveCameraListeners[i].onDrag -= OnDrag;
                        _dontMoveCameraListeners[i].onPress -= OnPress;
                    }
                }
            }
            s_instance = null;
            StopAllCoroutines();
            base.OnDestroy();
        }
    
        private void CombatBtnEvent(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                if (mAutoMode) SetAutoMode(!mAutoMode);
            }
            else
            {
                if (!mSpeedMode) SetSpeedMode(!mSpeedMode);
            }
        }
    
    	public void AddMoveCameraCollider(GameObject rootGo)
        {
            BoxCollider[] _dontMoveCameraColliders = rootGo.GetComponentsInChildren<BoxCollider>(true);
            if (_dontMoveCameraColliders != null)
            {
                for (int i = 0; i < _dontMoveCameraColliders.Length; i++)
                {
                    UIEventListener evtLister = _dontMoveCameraColliders[i].GetComponent<UIEventListener>() ?? _dontMoveCameraColliders[i].gameObject.AddComponent<UIEventListener>();
                    if (!_dontMoveCameraListeners.Contains(evtLister))
                    {
                        evtLister.onDrag += OnDrag;
                        evtLister.onPress += OnPress;
                        _dontMoveCameraListeners.Add(evtLister);
                    }
                }
            }
        }
    
        public void UpdateTeamInfo(Hotfix_LT.Combat.CombatTeamSyncData teamdata)
        {
            if (CombatLogic.Instance.IsPlayerOrChallengerSide(teamdata.TeamId))
            {
                if (SceneLogic.BattleType == eBattleType.ChallengeCampaign|| SceneLogic.BattleType == eBattleType.AlienMazeBattle)
                    LTUIUtil.SetText(MagicNumLabel_Sub, teamdata.SPoint.ToString());
            }
            else
            {
                string oldtxt = CurWaveLabel.text;
                LTUIUtil.SetText(CurWaveLabel_Sub, string.Format(EB.Localizer.GetString("ID_codefont_in_LTCombatHudController_5537"), teamdata.CurWave, teamdata.MaxWave));
                if(oldtxt != CurWaveLabel.text)
                {
                    ApplyBounceAnim(TurnPannel);
                }
            }
        }
    
        public void UpdateUI()
        {
            if (Hotfix_LT.Combat.CombatSyncData.Instance.Turn > 0)
            {
                string oldtxt = RoundLabel.text;
                LTUIUtil.SetText(RoundLabel_Sub, Hotfix_LT.Combat.CombatSyncData.Instance.Turn.ToString());
                if(oldtxt != RoundLabel.text)
                {
                    ApplyBounceAnim(RoundLabel.gameObject);
                }
            }
        }
    
        private int WaitForActionTimer;
        private Coroutine ActionTimerCoroutine;

        IEnumerator ActionTimer(int actor, int finishTime)
        {
            int timeleft = finishTime - EB.Time.Now;

    		if (timeleft > 10)
    		{
    			timeleft = 10;
    			WaitForActionTimer = EB.Time.Now + 10;
    		}
            else if (timeleft < 0)
            {
                timeleft = 1;
                WaitForActionTimer = EB.Time.Now + 1;
            }
    		else
    		{
    			WaitForActionTimer = finishTime;
    		}

    		LTUIUtil.SetText(ActionCountdownLabel, timeleft.ToString());

            if (ActionCountdownLabel != null)
            {
//                iTween.Stop(ActionCountdownLabel.gameObject);
                ActionCountdownLabel.gameObject.transform.DOKill();
                ActionCountdownLabel.transform.localScale = Vector3.one;
            }

            while (true)
            {
                yield return new WaitForSecondsRealtime(1);
                timeleft = WaitForActionTimer - EB.Time.Now;
                LTUIUtil.SetText(ActionCountdownLabel, timeleft.ToString());

                if (timeleft <= 3 && ActionCountdownLabel != null)
                {
                    ActionCountdownLabel.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

					var option = ActionCountdownLabel.transform.DOScale(Vector3.one, 0.4f);
					option.SetUpdate(true);
                }

    			if (timeleft <= 0)
                {
    				bool isMyTeam = Combat.CombatSyncData.Instance.GetCharacterData(actor).TeamId == CombatLogic.Instance.LocalPlayerTeamIndex;
    				
                    if (isMyTeam)
    				{
    					if (Combat.CombatSyncData.Instance.WaitCharacterData != null)
    					{
    						CombatSkillCtrl.AttackTargetUseAIWithSpecialSkill(Combat.CombatSyncData.Instance.WaitCharacterData);
                        }
    					else
    					{
    						Debug.LogError("Countdown End ActionByUseAI WaitCharacterData is null");
    					}
    				}

                    yield break;
                }
            }
        }
    
        public void OnSomeOneActionEvent()
        {
            if (SceneLogic.isPVP())
            {
                ResetCountdownLabel();
            }
        }

        private void ResetCountdownLabel()
        {
            if (ActionTimerCoroutine != null)
            {
                StopCoroutine(ActionTimerCoroutine);
                ActionTimerCoroutine = null;
//                iTween.Stop(ActionCountdownLabel.gameObject);
                ActionCountdownLabel.gameObject.transform.DOKill();
                ActionCountdownLabel.transform.localScale = Vector3.one;
            }

            LTUIUtil.SetText(ActionCountdownLabel, EB.Localizer.GetString("ID_ALLIANCE_CURRENT"));
        }
        
        private int[] _preMoveQueue;
        private List<int[]> _listMoveQueue = new List<int[]>();
        public void UpdateActionQueue(int[] moveQueue)
        {
            if (CombatLogic.Instance.LocalPlayerIsObserver) return;
    		if (moveQueue[0] < 0)
    		{
    			EB.Debug.LogWarning("moveQueue[0] < 0");
    			return;
    		}
    		if (!MoveActorIconProgress.isMove && _listMoveQueue.Count == 0)
            {
    			if (_preMoveQueue == null)
    			{
    				StartCoroutine(MoveQueueActionForWaitAnchorInited(moveQueue));
    			}
    			else
    				MoveQueueAction(moveQueue);
            }
            else if (moveQueue[0] != _preMoveQueue[0])
            {
                _listMoveQueue.Add(moveQueue);
            }
            _preMoveQueue = moveQueue;
    		if (!ActionOrderGORoot.activeSelf)
    		{
    			ActionOrderGORoot.CustomSetActive(true);
    		}
    	}
    
    	private IEnumerator MoveQueueActionForWaitAnchorInited(int[] moveQueue)
    	{
    		yield return null;
    		MoveQueueAction(moveQueue);
    	}
        
        #region TODO: CPU耗时 9+ ms
        private void MoveQueueAction(int[] moveQueue)
        {
            BackActionCharacter.Icon.spriteName = string.Empty;
            if (moveQueue.Length >= 1 && !string.IsNullOrEmpty(GetPortrait(moveQueue[0])))
            {
                CurrentActionCharacter.Icon.spriteName = GetPortrait(moveQueue[0]);
                bool isMyTeam = CombatLogic.Instance.IsPlayerOrChallengerSide(Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(moveQueue[0]).TeamId);
                CurrentActionCharacter.SetTeamColor(isMyTeam);
            }
            if (moveQueue.Length >= 2 && !string.IsNullOrEmpty(GetPortrait(moveQueue[1])))
            {
                ReadyActionCharacter.Icon.spriteName = GetPortrait(moveQueue[1]);
                bool isMyTeam = CombatLogic.Instance.IsPlayerOrChallengerSide(Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(moveQueue[1]).TeamId);
                ReadyActionCharacter.SetTeamColor(isMyTeam);
            }
            if (moveQueue.Length >= 3 && !string.IsNullOrEmpty(GetPortrait(moveQueue[2])))
            {
                ThreeActionCharacter.Icon.spriteName = GetPortrait(moveQueue[2]);
                bool isMyTeam = CombatLogic.Instance.IsPlayerOrChallengerSide(Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(moveQueue[2]).TeamId);
                ThreeActionCharacter.SetTeamColor(isMyTeam);
            }
            MoveActorIconProgress.Move(0.6f);
            MoveActorIconProgress.onITweenEnd = OnIconProgressEnd;
        }
        #endregion
    
        string GetPortrait(int actorId)
        {
            var characterData = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterData(actorId);
            return characterData != null ? characterData.Portrait : null;
        }
    
        public void LaunchActionCountdown(int actor,int finishTime)
        {
            if (ActionTimerCoroutine != null)
            {
                StopCoroutine(ActionTimerCoroutine);
            }

            if (mAutoMode == false && controller.gameObject.activeSelf && !LTCombatEventReceiver.Instance.IsBattleOver)
            {
                try{
                    ActionTimerCoroutine = StartCoroutine(ActionTimer(actor, finishTime));
                }
                catch(System.NullReferenceException e)
                {
                    EB.Debug.LogError(e.ToString());
                }
            }
        }
    
        private void OnIconProgressEnd()
        {
            if (_listMoveQueue.Count > 0)
            {
                int[] moveQueue = _listMoveQueue[0];
                _listMoveQueue.RemoveAt(0);
                MoveQueueAction(moveQueue);
            }
        }
    
        public void SetCurrentActionSkill(int skill_id)
        {
            Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skill_id);
            if (skill_id != 0 && skillTpl != null)
            {
                if (skillTpl.Type == Hotfix_LT.Data.eSkillType.ACTIVE)
                {
                    LTUIUtil.SetText(CurrentActionerSkillTypeLabel, EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4070"));
                    CurrentActionerSkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                }
                else if (skillTpl.Type == Hotfix_LT.Data.eSkillType.NORMAL)
                {
                    LTUIUtil.SetText(CurrentActionerSkillTypeLabel, EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4440"));
                    CurrentActionerSkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
                }
                else if (skillTpl.Type == Hotfix_LT.Data.eSkillType.PASSIVE)
                {
                    LTUIUtil.SetText(CurrentActionerSkillTypeLabel, EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4751"));
                    CurrentActionerSkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.BlueColor;
                }
                else if (skillTpl.Type == Hotfix_LT.Data.eSkillType.SCROLL)
                {
                    LTUIUtil.SetText(CurrentActionerSkillTypeLabel, EB.Localizer.GetString("ID_codefont_in_LTCombatHudController_13896"));
                    CurrentActionerSkillTypeBG.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
                }
                LTUIUtil.SetText(CurrentActionerSkillNameLabel, skillTpl.Name);
                for (var i = 0; i < CurrentActionerSkillItemTws.Length; i++)
                {
                    var tw = CurrentActionerSkillItemTws[i];
                    tw.ResetToBeginning();
                    tw.PlayForward();
                }
                CurrentActionerSkillItem.transform.localPosition = new Vector3(-100,0,0);
            }
            else
            {
                CurrentActionerSkillItem.transform.localPosition = new Vector3(1000,1000,0);
            }

            ResetCountdownLabel();
        }
    
        public void ShowSkillPanel(Hotfix_LT.Combat.CombatCharacterSyncData characterData)
        {
            Hotfix_LT.Combat.CombatSyncData.Instance.WaitCharacterData = characterData;
            CombatSkillCtrl.ShowSkill(characterData);
            AddMoveCameraCollider(CombatSkillCtrl.mDMono.gameObject);
        }

        public void ShowAutoSkillSelect(Hotfix_LT.Combat.CombatCharacterSyncData characterData)
        {
            Hotfix_LT.Combat.CombatSyncData.Instance.WaitCharacterData = characterData;
            if(CombatManager .Instance.GetSkillRequestState())
            {
                StartCoroutine(AutoSkillSelectCoroutine());
            }
            else
            {
                AutoSkillSelect(characterData);
            }
        }

        IEnumerator AutoSkillSelectCoroutine()
        {
            yield return new WaitUntil(() => !CombatManager.Instance.GetSkillRequestState());
            AutoSkillSelect(Hotfix_LT.Combat.CombatSyncData.Instance.WaitCharacterData);
        } 

        public void UpdateSkillList(Hotfix_LT.Combat.CombatCharacterSyncData data)
        {
            CombatSkillCtrl.UpdateSkillList(data);
        }


        public bool CombatHudVisible()
        {
            return !(BossHealthBarCtrl.IsSkillHudVisible || CombatSkillCtrl.BagPanel.IsHudVisible);
        }
        
        public Coroutine DoReadyAction()
        {
            return StartCoroutine(ShowCombatStartCoroutine());
        }

        public Coroutine DoHonorReadyAction(int round, List<Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo> teamInfo)
        {
            return StartCoroutine(ShowHonerAreanCombatStart(round, teamInfo));
        }

        public bool SetupBaseUI()
    	{
            var fx = FullScreenFXAssets.Find(asset => asset.fx != null && asset.fx.name.IndexOf(CombatViewAction.Combat_Scene_Name) >= 0);
            if (fx != null)
    		{
    			CurFullScreenFX = fx.gameObject;
    			CurFullScreenFX.CustomSetActive(true);
    		}

            return Visibility;
        }
    
        public void SettingSomeoneFXVisible()
        {
    		if (CurFullScreenFX != null)
            {
                bool isShowing = true;
                Renderer[] renderers = CurFullScreenFX.GetComponentsInChildren<Renderer>();

                if (renderers == null)
                {
                    return;
                }

                for (var i = 0; i < renderers.Length; i++)
                {
                    var renderer = renderers[i];

                    if (renderer.gameObject.layer == GameEngine.Instance.transparentUI3DLayer || renderer.gameObject.layer == GameEngine.Instance.ui3dLayer)
                    {
                        renderer.gameObject.layer = isShowing ? GameEngine.Instance.ui3dLayer : GameEngine.Instance.transparentUI3DLayer;
                    }
                    else
                    {
                        renderer.gameObject.layer = isShowing ? GameEngine.Instance.uiLayer : GameEngine.Instance.transparentFXLayer;
                    }
                }
            }
    	}
    
        IEnumerator ShowCombatStartCoroutine()
        {
            RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.defaultSettings);

            Ready = false;
            LTCombatEventReceiver.Instance.TimeScale = Time.timeScale = 1.0f;
            FusionAudio.SetGroupAudioPinch("SFX", 1);

            FusionAudio.PostEvent("SFX/CombatView/BattleStart");
            RenderSettingsManager.Instance.ResetRecoverSetting();

            NewStartFX.gameObject.CustomSetActive(true);
            ShowCombatBaseUI();
            yield return new WaitForSeconds(1.5f);
            NewStartFX.gameObject.CustomSetActive(false);
            Ready = true;//此标记将用于确认是否可以开始显示完整UI信息
            UpdateUIMode();

            LTCombatEventReceiver.Instance.DoReadyActionOver();
        }

        IEnumerator ShowHonerAreanCombatStart(int round, List<Hotfix_LT.Combat.CombatSyncActionData.CombatTeamInfo> teamInfo)
        {
            RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.defaultSettings);

            Ready = false;
            LTCombatEventReceiver.Instance.TimeScale = Time.timeScale = 1.0f;
            FusionAudio.SetGroupAudioPinch("SFX", 1);
            CornerAnchorRoot.gameObject.CustomSetActive(false);

            FusionAudio.PostEvent("SFX/CombatView/BattleStart");
            RenderSettingsManager.Instance.ResetRecoverSetting();

            NewStartFX.gameObject.CustomSetActive(true);
            RoundInfo.text = string.Format(EB.Localizer.GetString("ID_COMBAT_ROUND_TEXT"), round);
            RoundInfo.gameObject.CustomSetActive(true);
            TweenAlpha RoundInfoTA = RoundInfo.GetComponent<TweenAlpha>();
            RoundInfoTA.ResetToBeginning();
            RoundInfoTA.PlayForward();
            ShowCombatBaseUI();

            int playerWorldID = 0;
            for (int i = 0; i < teamInfo.Count; i++)
            {
                var info = teamInfo[i];
                int win = info.teamWin;

                string teamName;
                
                if (info.teamIndex == 0)
                {
                    playerWorldID = info.worldId;
                    if (info.worldId <= 0)
                    {
                        teamName = string.Format("{0}", info.teamName);
                    }
                    else
                    {
                        teamName = string.Format("{0} 【{1}{2}】", info.teamName, info.worldId, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME"));
                    }
                    readTeamName.text = teamName;
                    readTeamWin.text = win.ToString();

                    for (int j = 0; j < redTeamWins.Count; j++)
                    {
                        if (win-- > 0)
                        {
                            redTeamWins[j].color = winColor;
                        }
                        else
                        {
                            redTeamWins[j].color = loseColor;
                        }
                    }
                }
                else if (info.teamIndex == 1)
                {
                    if (info.worldId <= 0) info.worldId = playerWorldID;
                    if (info.worldId <= 0)
                    {
                        teamName = string.Format("{0}", info.teamName);
                    }
                    else
                    {
                        teamName = string.Format("【{1}{2}】 {0}", info.teamName, info.worldId, EB.Localizer.GetString("ID_LOGIN_SERVER_NAME"));
                    }
                    blueTeamName.text = teamName;
                    blueTeamWin.text = win.ToString();

                    for (int k = 0; k < blueTeamWins.Count; k++)
                    {
                        if (win-- > 0)
                        {
                            blueTeamWins[k].color = winColor;
                        }
                        else
                        {
                            blueTeamWins[k].color = loseColor;
                        }
                    }
                }
            }
            honorTitle.CustomSetActive(true);
            yield return new WaitForSeconds(1.5f);

            RoundInfo.gameObject.CustomSetActive(false);
            NewStartFX.gameObject.CustomSetActive(false);
            Ready = true;//此标记将用于确认是否可以开始显示完整UI信息
            UpdateUIMode();

            LTCombatEventReceiver.Instance.DoReadyActionOver();
        }
        
        public void ShowBattleResultScreenEx(eBattleType battleType, eCombatOutcome outCome, bool isCampaignFinished)
        {
            ShowBattleResultScreen( battleType,  outCome,  isCampaignFinished);
        }

        Coroutine ShowBattleResultScreenC;
    	public void ShowBattleResultScreen(eBattleType battleType, eCombatOutcome outCome, bool isCampaignFinished)
        {
            if (ShowBattleResultScreenC == null)
            {
                ShowBattleResultScreenC = EB.Coroutines.Run(ShowBattleResultScreenCoroutine(battleType, outCome, isCampaignFinished));
            }
        }
    

        ///
        ///展示战斗结算界面
        ///
        IEnumerator ShowBattleResultScreenCoroutine(eBattleType battleType, eCombatOutcome outCome, bool isCampaignFinished)
        {
            FusionAudio.SetGroupAudioPinch("SFX", 1);

            if (LTCombatEventReceiver.Instance != null)
            {
                LTCombatEventReceiver.Instance.TimeScale = Time.timeScale = 1.0f;
                LTCombatEventReceiver.Instance.OnBattleResultScreen();

                while (LTCombatEventReceiver.Instance.Conversationing)
                {
                    yield return null;
                }
            }

            HideUIHud(true);
            SettingSomeoneFXVisible();

            if (!(battleType == eBattleType.BossBattle && LTWorldBossDataManager.Instance.IsHaveWorldBossRoll))
            {
                UIStack.Instance.ExitStack(false);
            }
    
            List<Combat.CombatCharacterSyncData> aliveList = Combat.CombatSyncData.Instance.GetAliveCharacterList();
            bool haveAlive = aliveList != null ? aliveList.Find(c => c.TeamId == CombatLogic.Instance.LocalPlayerTeamIndex) != null : false;

            if (LTCombatEventReceiver.Instance != null)
            {
                LTCombatEventReceiver.Instance.SetWinningTeam(outCome == eCombatOutcome.Win && haveAlive);
            }

            if (!(battleType == eBattleType.HonorArena) && outCome == eCombatOutcome.Win && haveAlive)
            {
    			PlayVictoryCameraing = true;

                if (MyFollowCamera.Instance != null)
                {
                    MyFollowCamera.Instance.isActive = false;
                }

                //EventManager.instance.Raise(new CombatTeamsVictoryEvent());  //事件管理
                // Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.CombatTeamsVictoryEvent);
                if(CombatCamera.Instance != null){
                    CombatCamera.Instance.OnTeamsVictoryListener();
                }
                FusionAudio.PostEvent("UI/CampaignResult/VictoryDance");
                if (LTCombatEventReceiver.Instance != null)
                {
                    LTCombatEventReceiver.Instance.PlayVictoryDance();
                }
                yield return new WaitForSeconds(1.5f);
            }

    		PlayVictoryCameraing = false;
            bool isCombatOut = IsCombatOut;//是否主动离开战斗

            if (outCome == eCombatOutcome.Win && !haveAlive)
            {
                FusionAudio.PostEvent("UI/CampaignResult/VictoryDance");
            }

            if (controller != null)
            {
                //要清除自己
                controller.Close();
                DestroySelf();
                //controller.DestroyControllerForm();    //不能打开，销毁后血条对象池才回收，会报错
            }
            //
            if (battleType == eBattleType.FirstBattle)
    		{
                Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.01f;
                LTStoryController.OpenMovie(() =>
                {
                    BattleResultScreenController.DirectExitCombat();
                }, "LTPrologueEndVideo");
                yield break;
    		}

            CombatManager combat = LTHotfixManager.GetManager<CombatManager>();
            if (combat.State == EB.Sparx.SubSystemState.Connected && !UIStack.Instance.IsLoadingScreenUp)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("battleType", battleType);
                ht.Add("outCome", outCome);
                ht.Add("campaignFinished", isCampaignFinished);
                ht.Add("isCombatOut", isCombatOut);
                GlobalMenuManager.Instance.Open("BattleResultScreen", ht);
            }

            //强行释放战斗界面的AB资源包
            EB.Assets.UnloadAssetByName("CombatHudV4", true);
        }
    
        public void PauseCombat()
        {
    
            //Timer.Paused = true;
            //SkillGuide.Paused = true;
            //UIStack.Instance.MoveToTop(this);
        }
    
        public void ExitCombat()
        {
    		if(CurFullScreenFX!=null)
    			CurFullScreenFX.gameObject.CustomSetActive(false);
        }
    
        #region ui mode

        [System.NonSerialized]
        public char[] AutoSkillString;
        bool mAutoMode;
        public bool AutoMode { get { return mAutoMode; } }
        private List<Hotfix_LT.Combat.CombatCharacterSyncData> AutoList;
        bool mSpeedMode;
    
        public void ShowCombatBaseUI()
        {
            CornerAnchorRoot.gameObject.CustomSetActive(true);
            if (ActionOrderGORoot == null)
            {
                return;
            }
    
            if (!IsCanChangeSpeedMode())
            {
                SetSpeedMode(true);
                SetSpeedModeSpriteStatus(false);
            }
            else
            {
                var config = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10048);               
                if (!config.IsConditionOK())
                {
                    mSpeedMode = false;
                    SetSpeedModeSpriteStatus(false);
                }
                if (PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatSpeed", 1) == 1)
                {
                    mSpeedMode = false;
                    SetSpeedModeSpriteStatus(true);
                }
                else if (PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatSpeed", 1) == 2)
                {
                    mSpeedMode = true;
                    SetSpeedModeSpriteStatus(true);
                }
                else
                {
                    EB.Debug.LogError("The Combat Speed isn't what I expected");
                }
                SetSpeedMode(mSpeedMode);
            }

            mAutoMode = true;
            if (SceneLogic.BattleType == eBattleType.ArenaBattle || SceneLogic.BattleType == eBattleType.HonorArena)
            {
                mAutoMode = true;
                AutoButton.GetComponent<UISprite>().color = AutoButton.GetComponent<UIButton>().hover = AutoButton.GetComponent<UIButton>().pressed = new Color(1, 0, 1, 1);
            }
            else
            {
                var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10047);
                if (!func.IsConditionOK())
                {
                    mAutoMode = false;
                }
                else if (PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatAuto", 1) == 1)
                {
                    mAutoMode = false;
                }
                else if (PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatAuto", 1) == 2)
                    mAutoMode = true;
                else
                    EB.Debug.LogError("The Combat Auto Data isn't what I expected");
            }

            if(SceneLogic.BattleType == eBattleType.InfiniteCompete)
            {
                CountdownTimeLabel.gameObject.CustomSetActive(true);
                if (CombatManager.Instance.startObj != null)
                {
                    countdownTime = EB.Dot.Integer("startTime", CombatManager.Instance.startObj, 0);
                }
            }
            else if (SceneLogic.BattleType == eBattleType.LadderBattle)
            {
                TrusteeshipBtn.CustomSetActive(true);
                if (LadderManager.Instance.IsTrusteeship)
                {
                    mAutoMode = true;
                    TrusteeshipOpenSprite.CustomSetActive(true);
                }
            }
            SetAutoMode(mAutoMode);
        }
    
        private void UpdateUIMode()
        {
            ShowCombatBaseUI();
    
            if (!CombatLogic.Instance.LocalPlayerIsObserver)//非观战
            {
                SetAutoMode(mAutoMode);

                if (Combat.CombatSyncData.Instance.TeamDataDic.ContainsKey(CombatLogic.Instance.LocalPlayerTeamIndex))
                {
                    UpdateTeamInfo(Combat.CombatSyncData.Instance.TeamDataDic[CombatLogic.Instance.LocalPlayerTeamIndex]);
                }
            }
            else//观战
            {
                mAutoMode = false;

                if (AutoButton != null)
                {
                    AutoButton.gameObject.CustomSetActive(false);

                    var uiGrid = AutoButton.transform.parent.gameObject.GetComponent<UIGrid>();

                    if (uiGrid != null)
                    {
                        uiGrid.Reposition();
                    }
                }

                if (SpeedModeUI != null)
                {
                    SpeedModeUI.transform.parent.gameObject.CustomSetActive(false);
                }

                if (CombatSkillCtrl != null)
                {
                    CombatSkillCtrl.mDMono.transform.parent.gameObject.CustomSetActive(false);
                }

                if (CurWaveLabel != null)
                {
                    CurWaveLabel.transform.parent.gameObject.CustomSetActive(false);
                }

                if (MoveActorIconProgress != null)
                {
                    MoveActorIconProgress.mDMono.transform.parent.gameObject.CustomSetActive(false);
                }
            }
    	}
    
        public void HideUIHud(bool isTrue)
        {
            Show(!isTrue);
        }
        
        public void BossCameraHandle()
        {
            HideUIHud(true);
            SettingSomeoneFXVisible();
            BossCom();
        }
          
        public void onSpeedButtonClick()
        {
            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10048);
            if (!func.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                return;
            }
            if (!IsCanChangeSpeedMode())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTCombatHudController_23844"));
                return;
            }
            SetSpeedMode(!mSpeedMode);
        }
    
        private void SetSpeedMode(bool speedMode)
        {
            mSpeedMode = speedMode;
            float timeScale = speedMode ? 2 : 1;
            if (Ready)
            {
                if (LTCombatEventReceiver.Instance.TimeScale == Time.timeScale)
                {
                    Time.timeScale = timeScale;
                    LTCombatEventReceiver.Instance.TimeScale = timeScale;
                    //暂时不加速了
    		    	FusionAudio.SetGroupAudioPinch("SFX", speedMode? 2:1);
    		    }
                else
                {
                    EB.Debug.LogWarning("speed is error or Combat has not ready in this moment");
                }
            }
            else
            {
                Time.timeScale = LTCombatEventReceiver.Instance.TimeScale = 1;
            }
    
            NotSpeedModeUI.CustomSetActive(!mSpeedMode);
            SpeedModeUI.CustomSetActive(mSpeedMode);
    
            SaveSpeedMode(mSpeedMode);
        }
    
        private void SaveSpeedMode(bool speedMode)
        {
            if (speedMode)
                PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatSpeed", 2);
            else
                PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatSpeed", 1);
        }
    
        private bool IsCanChangeSpeedMode()
        {
            if (SceneLogic.isPVP())
            {
                return false;
            }
            return true;
        }
    
        private void SetSpeedModeSpriteStatus(bool isCouldClick)
        {
            NotSpeedModeBG.CustomSetActive(!isCouldClick);
        }
        
        public void onAutoButtonClick()
        {

            if (SceneLogic.BattleType == eBattleType.ArenaBattle)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTCombatHudController_25428")));
                return;
            }
            else if (SceneLogic.BattleType == eBattleType.HonorArena)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_HONOR_ARENA_MUST_AUTO_BATTLE")));
                return;
            }
            else if(SceneLogic.BattleType == eBattleType.LadderBattle)
            {
                if (LadderManager.Instance.IsTrusteeshiping()) return;
            }

            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10047);
            if (!func.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                return;
            }
    
            SetAutoMode(!mAutoMode); 
            if(Ready) CombatSkillCtrl.SkillTipsCtrl.ShowUI(false);
        }
    
        #region CPU耗时4+ms
        private void SetAutoMode(bool autoMode)
        {
            StartCoroutine(DoSetAutoMode(autoMode));
        }
        
        private IEnumerator DoSetAutoMode(bool autoMode)
        {
            mAutoMode = autoMode;

            if (SceneLogic.BattleType != eBattleType.ArenaBattle)
            {
                this.SaveAutoMode(mAutoMode);
            }

            if (Ready)
            {
                if (autoMode)
                {
                    #region  Split Frame
                    if (!IsAutoInit)
                    {
                        InitAutoButton();
                        AddMoveCameraCollider(AutoCombatGrid.gameObject);
                    }
                    #endregion

                    yield return null;

                    #region Split Frame
                    if (AutoCombatItems != null)
                    {
                        AutoCombatItems.ForEach(item => item.SetDeath(false));
                    }

                    Instance.CombatSkillCtrl.SetConvergeTargeting();

                    if (Combat.CombatSyncData.Instance.NeedSetSkill)
                    {
                        AutoSkillSelect(Combat.CombatSyncData.Instance.WaitCharacterData);

                        if (LTCombatEventReceiver.Instance != null)
                        {
                            LTCombatEventReceiver.Instance.ForEach(combatant => combatant.HideRestrainFlag());
                        }
                    }
                    #endregion

                    yield return null;
                }
                else
                {
                    Instance.CombatSkillCtrl.ClearConvergeInfo();
                }
            }
            
            #region Split Frame
            NotAutoModeUI.CustomSetActive(!mAutoMode);
            AutoModeUI.CustomSetActive(mAutoMode);

            if(Ready)
            {
                AutoCombatGrid.CustomSetActive(mAutoMode);
                AutoCombatSkillBGSprite.gameObject.CustomSetActive(mAutoMode);
                RefreshAutoGridAndBGSprite();
            }
            #endregion
        }
        #endregion
    
        private void SaveAutoMode(bool autoMode)
        {
            if (autoMode)
                PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatAuto", 2);
            else
                PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value.ToString() + "CombatAuto", 1);
        }
        
        public void RefreshAutoButton()
        {
            if(mAutoMode)
            {
                InitAutoButton();
                RefreshAutoGridAndBGSprite();
                AutoCombatItems.ForEach(item => item.SetDeath(false));
            }
        }

        private void RefreshAutoGridAndBGSprite()
        {
            AutoCombatGrid.GetComponent<UIGrid>().Reposition();
            AutoCombatSkillBGSprite.width = mAutoMode ? (779 - (4 - AutoList.Count) * 162) : 655;
        }
    
        private void InitAutoButton()
        {
            AutoList=new List<Hotfix_LT.Combat.CombatCharacterSyncData> ();
            List<Hotfix_LT.Combat.CombatCharacterSyncData> mList = Hotfix_LT.Combat.CombatSyncData.Instance.GetCharacterList(CombatLogic.Instance.LocalPlayerTeamIndex);
            for (int i = 0; i < mList.Count; i++)
            {
                if (mList[i].Uid == LoginManager.Instance.LocalUserId.Value) AutoList.Add(mList[i]);
            } 
            if (AutoList == null|| mList==null)
            {
                EB.Debug.LogError("InitAutoButton AutoList == null 若结束战斗报错，战斗协程未关闭造成的？");
                return;
            }
            AutoList.Sort(Hotfix_LT.Combat.CombatCharacterSyncData.Default);
            AutoCombatItems = new List<AutoCombatItem>();
            for(int i=0;i< AutoCombatGrid.transform.childCount; i++)
            {
                AutoCombatGrid.transform.GetChild(i).gameObject.CustomSetActive(false);
            }
            AutoCombatItem autoCombatItem = AutoCombatGrid.transform.GetChild(0).transform.GetMonoILRComponent<AutoCombatItem>();
            for (int i = 0; i < AutoList.Count; i++)
            {
                if (i > 0)
                {
                    if (AutoCombatGrid.transform .childCount>i)
                        autoCombatItem= AutoCombatGrid.transform.GetChild(i).transform.GetMonoILRComponent<AutoCombatItem>();
                    else
                    {
                        //autoCombatItem = GameUtils.InstantiateEx<AutoCombatItem>(autoCombatItem, AutoCombatGrid.transform, "Clone");

                        GameObject go = GameObject.Instantiate(autoCombatItem.mDMono.gameObject) as GameObject;
                        go.name = "Clone";
                        go.transform.SetParent(AutoCombatGrid.transform, false);
                        if (false == go.activeSelf) go.gameObject.SetActive(true);

                        autoCombatItem = go.GetMonoILRComponent<AutoCombatItem>();
                    }
                }
    
               
                AutoCombatItems.Add(autoCombatItem);
                AutoCombatItems[i].ItemCharSyncData = AutoList[i];
                AutoCombatItems[i].Position = i;
                autoCombatItem.mDMono.transform.Find("Icon/Sprite").GetComponent<UISprite>().spriteName = AutoList[i].Portrait;
                autoCombatItem.mDMono.gameObject.CustomSetActive(true);
            }
            string sc = PlayerPrefs.GetString(LoginManager.Instance.LocalUserId.Value.ToString() + "AutoCombatSkill", "ssssss");
            AutoSkillString = sc.ToCharArray();
            for (int i = 0; i < AutoList.Count; i++)
            {
                if (AutoSkillString[i] == 'n')
                {
                    AutoCombatItems[i].SetCommonSkill();
                }
                else if (AutoSkillString[i] == 's')
                {
                    AutoCombatItems[i].SetSpecialSkill();
                }
                else
                    EB.Debug.LogError("this is a fault value in AutoSillString");
            }
        }
    
        public static void ResetAutoSkill()
        {
            PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value.ToString() + "AutoCombatSkill", "ssssss");
        }
    
        private void AutoSkillSelect(Hotfix_LT.Combat.CombatCharacterSyncData ch_data)
        {
            if (AutoList!=null&&AutoList.Count == 0)
            {
                InitAutoButton();
                RefreshAutoGridAndBGSprite();
            }
            CombatSkillCtrl.AutoSelectSkill(ch_data);
        }
        #endregion
    
        private int clickCount = 0;
        public override void OnCancelButtonClick()
        {
            if (LadderManager.Instance.IsTrusteeshiping()) return;

            //if (SceneLogic.BattleType == eBattleType.AllieanceFinalBattle&& !CombatLogic.Instance.LocalPlayerIsObserver)
            //{
            //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTCombatHudController_30940"));
            //    return;
            //}
    
    		if (!LTCombatEventReceiver.IsCombatInit())
            {
                return;
            }
            if (LTCombatEventReceiver.Instance.IsBattleOver)
            {
    			if (PlayVictoryCameraing)
    			{
    				return;
    			}

                if (BattleResultScreenController.Instance != null)
                {
                    BattleResultScreenController.Instance.SafeContinue();
                }

                return;
            }
    
            if (BossHealthBarCtrl.IsSkillHudVisible)
            {
                BossHealthBarCtrl.CloseHud();
                return;
            }
    
            if (CombatSkillCtrl.BagPanel.IsHudVisible)
            {
                CombatSkillCtrl.BagPanel.Container.gameObject.CustomSetActive(false);
                return;
            }
    
            //新手引导特殊处理
            if (GuideNodeManager.IsGuide && !LTInstanceUtil.IsFirstChapterCompleted())
            {
                if (clickCount >= 3 && SceneLogic.BattleType != eBattleType.FirstBattle)
                {
                    clickCount = 0;
                    MessageTemplateManager.ShowMessage(901099, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            GuideNodeManager.currentGuideId = 0;
                            GuideNodeManager.GetInstance().JumpGuide();//战斗中跳过
                        }
                        return;
                    });
                }
                clickCount++;
                if (MengBanController.Instance.controller.gameObject.activeSelf) MengBanController.Instance.Hide();
                if (GuideNodeManager.IsVirtualBtnGuide) EB.Debug.LogError("GuideNodeManager.IsVirtualBtnGuide!");
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_RETURN));
                return;
            }
            if (CombatLogic.Instance.LocalPlayerIsObserver) ExitWatchAsk();
            else ExitAsk();
        }
    
    	public void OnChatBtnClick()
        {
            if (GuideNodeManager.IsGuide)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_FUNCCLICK));
                return;
            }
            GlobalMenuManager.Instance.Open("ChatHudView");
        }
    
        public void OnFriendBtnClick()
        {
            if (GuideNodeManager.IsGuide)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_FUNCCLICK));
                return;
            }
            GlobalMenuManager.Instance.Open("FriendHud");
        }
    
        public void OnResetCameraClick()
        {
    		MyFollowCamera.Instance.ResetCameraView();
    	}

        public void OnTrusteeshipBtnClick()
        {
            LadderManager.Instance.OpenOrCloseTrusteeship(!LadderManager.Instance.IsTrusteeship);
            if (LadderManager.Instance.IsTrusteeship)
            {
                TrusteeshipOpenSprite.CustomSetActive(true);
                SetAutoMode(true);
            }
            else
            {
                TrusteeshipOpenSprite.CustomSetActive(false);
                SetAutoMode(false);
            }
        }

        public void DisplayCameraViewButton(bool isTrue)
        {
            ResetCameraViewBtn.gameObject.CustomSetActive(isTrue);
            ResetCameraViewBtn.transform.parent.GetComponent<UIGrid>().Reposition();
        }
    
        private void ExitAsk()
        {
            string title = EB.Localizer.GetString("ID_COMBAT_EXIT_COMBAT_TITLE");
            string content = EB.Localizer.GetString("ID_COMBAT_EXIT_COMBAT_CONFIRM");
            string ok = EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");
            string cancel = EB.Localizer.GetString("ID_DIALOG_BUTTON_CANCEL");

            MessageDialog.Show(title, content, ok, cancel, false, false, true, delegate (int return_code)
            {
                if (return_code == 0)
                {
                    if (SceneLogic.BattleType == eBattleType.HantBattle)
                    {
                        LTBountyTaskConversationController.sRunAway = true;
                    }

                    CombatManager.Instance.RequestExitCombat(CombatLogic.Instance.CombatId, delegate (EB.Sparx.Response response) {
                        if (Instance != null && response != null)
                        {
                            if (response.sucessful)
                            {
                                LTCombatEventReceiver.Instance.CancelBtnClick();
                                IsCombatOut = true;

                                if (CurFullScreenFX != null)
                                {
                                    CurFullScreenFX.gameObject.CustomSetActive(false);
                                }

                                this.StopAllCoroutines();

                                if (response.hashtable["retry"] != null)
                                {
                                    BattleResultScreenController.DirectExitCombat();
                                    return;
                                }

                                DataLookupsCache.Instance.CacheData(response.hashtable);
                            }
                            else
                            {
                                response.CheckAndShowModal();
                            }
                        }
                    });
                }
            });
        }
    
        public void ExitWatchAsk()
        {
            CombatManager.Instance.LevelWarRequire(CombatLogic.Instance.CombatId);
            BattleResultScreenController.DirectExitCombat();
        }
    
        void OnDrag(GameObject go, Vector2 delta)
        {
            if (MyFollowCamera.delTouchDownInView != null)
            {
                MyFollowCamera.delTouchDownInView();
            }
        }
    
        void OnPress(GameObject go, bool isPress)
        {
            if (MyFollowCamera.delTouchDownInView != null && isPress)
            {
                MyFollowCamera.delTouchDownInView();
            }
        }
        public void BossHealthBarCtrl_UpdateMoveBar(float data)
        {
            BossHealthBarCtrl.UpdateMoveBar(data);
        }

        public void BossHealthBarCtrl_InitializeData(Combat.CombatCharacterSyncData data)
        {
            BossHealthBarCtrl.InitializeData(data);
        }

        public void BossHealthBarCtrl_CleanUp()
        {
            BossHealthBarCtrl.CleanUp();
        }

        public void BossHealthBarCtrl_UpdateBuff(ICollection<Combat.CombatCharacterSyncData.BuffData> buffDatas)
        {
            BossHealthBarCtrl.UpdateBuff(buffDatas);
        }

        public void CombatSkillCtrl_ClearConvergeInfo(int value)
        {
            CombatSkillCtrl.ClearConvergeInfo(value);
        }

        public void BossHealthBarCtrl_Show()
        {
            BossHealthBarCtrl.Show();
        }

        public void BossHealthBarCtrl_UpdateHp(long hp)
        {
            BossHealthBarCtrl.UpdateHp(hp);
        }

        public void BossHealthBarCtrl_UpdateHurt(int value)
        {
            BossHealthBarCtrl.UpdateHurt(value);
        }
        public List<string> GetCombatFloatFont(List<int>data)
        {
            List<string> result = new List<string>();
            int len = data.Count;
            for (int i = 0; i < len; i++)
            {
                int typeIndex = data[i];
                Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType floatType = Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType.Gain;
                if (typeIndex < 0)
                {
                    floatType = Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType.Debuff;
                }
                int absIndex = Mathf.Abs(typeIndex);
                absIndex = absIndex - 1;
                result.Add(EB.Localizer.GetString(Hotfix_LT.UI.CombatFloatFontUIHUD.FontMap[absIndex]));
            }

            return result;
        }
    }
    
    
}
