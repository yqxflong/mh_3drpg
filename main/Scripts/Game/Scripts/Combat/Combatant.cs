using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
//using _HotfixScripts.Utils;
using Unity.Jobs;
using UnityEngine.Jobs;
using Hotfix_LT.Effect;
using Hotfix_LT.UI;

namespace Hotfix_LT.Combat
{
    public class Combatant : MonoBehaviour
    {
        public Transform DamageTextTarget;
        public Vector3 healTextOffset { get { return new Vector3(0f, 0.6f, 0f); } }
        public Vector3 damageTextOffset { get { return new Vector3(0f, 0.4f, 0f); } }
        public Vector3 floatBuffFontTextOffset { get { return new Vector3(0f, 0.85f, 0f); } }
        public AnimationCurve myRotationSCurve;
        public Vector3 hitOffset = new Vector3(0.0f, 0.0f, 0.0f);

        [HideInInspector] public CharacterColorScale colorScale;
        [HideInInspector] public Transform redRing;
        [HideInInspector] public Transform orangeRing;
        [HideInInspector] public Transform blackRing;
        [HideInInspector] public Transform greenRing;
        [HideInInspector] public Transform healthBar;

        private GameObject m_shadowPlane = null;
        private float m_shadowPlaneY = 0f;

        public ParticleSystemReference preparePS = new ParticleSystemReference();
        public ParticleSystemReference targetSelectSenderPS = new ParticleSystemReference();
        public ParticleSystemReference targetSelectTargetPS = new ParticleSystemReference();
        public ParticleSystemReference targetSelectTargetHeadPS = new ParticleSystemReference();
        public ParticleSystemReference targetSelectPS = new ParticleSystemReference();

        public static readonly string defaultTargetSelectSenderPS = "fx_TY_dangqianxingdong";
        public static readonly string defaultTargetSelectTargetPS = "fx_p_TY_skill_set";
        public static readonly string defaultTargetSelectTargetHeadPS = "fx_p_TY_skill_set_head";
        public static readonly string defaultTargetSelectPS = "fx_zd_Guaiwuxuanzhong";

        public Vector3 OriginPosition
        {
            get { return gameObject.transform.parent.position; }
        }

        public Vector3 OriginForward
        {
            get { return gameObject.transform.parent.forward; }
        }

        public Vector3 HitPoint
        {
            get { return gameObject.transform.parent.TransformPoint(hitOffset); }
        }

        private bool m_needLaunch = false;
        private int m_launchSkill = -1;
        public bool NeedLaunch
        {
            get { return m_needLaunch; }
            private set { m_needLaunch = value; }
        }
        public int LaunchSkill
        {
            get { return m_launchSkill; }
            private set { m_launchSkill = value; }
        }

        private ExileEffectEvent m_exileEvent = null;
        public ExileEffectEvent ExileEvent
        {
            get { return m_exileEvent; }
        }
        private ReviveEffectEvent m_reviveEvent = null;
        public ReviveEffectEvent ReviveEvent
        {
            get { return m_reviveEvent; }
        }

        public bool DeathOver = true;

        private float m_meshRadius = 1.0f;
        private CapsuleCollider m_collider = null;
        public float Radius
        {
            get { return m_meshRadius; }
        }
        public CapsuleCollider Collider
        {
            get { return m_collider; }
        }

        private Quaternion m_startRotation = Quaternion.identity;
        private Vector3 m_baseForward = Vector3.forward;
        public Vector3 BaseForward
        {
            get { return m_baseForward; }
        }
        public Quaternion StartRotation
        {
            get { return m_startRotation; }
        }

        private AnimationCurve m_combatTimeScale = null;
        public AnimationCurve TimeScale
        {
            get { return m_combatTimeScale; }
            set { m_combatTimeScale = value; }
        }

        private ImpactDataContainer m_impactContainer = new ImpactDataContainer();

        private SkinnedMeshRenderer[] skinnedMeshRenderer;

        private HitMono m_hitMono = null;

        private MoveController m_moveController = null;
        private MoveMotion m_moveMotion = null;
        private Animator m_animator = null;
        public MoveController MoveController
        {
            get { return m_moveController; }
        }
        public MoveEditor.Move CurrentMove
        {
            get { return m_moveController.CurrentMove; }
        }
        public Animator Animator
        {
            get { return m_animator; }
        }

        private MoveEditor.FXHelper m_fxHelper = null;
        public MoveEditor.FXHelper FXHelper
        {
            get { return m_fxHelper; }
        }

        public AvatarComponent Avatar
        {
            get; private set;
        }

        public bool Ready
        {
            get; private set;
        }

        public CombatantIndex Index
        {
            get
            {
                if (Data == null)
                {
                    return null;
                }
                return Data.Index;
            }
        }

        public CombatantAttributes Attributes
        {
            get;
            set;
        }

        public CombatCharacterSyncData Data
        {
            get { return _data; }
            set
            {
                _data = value;

                if (HealthBar != null) // && HealthBar.HealthBar != null)
                {
                    //HealthBar.HealthBar.Data = value;
                    HealthBar.OnHandleMessage("SetData", value);
                }
            }
        }
        private CombatCharacterSyncData _data;
        private CombatEventState m_eventState = null;
        private CombatActionState m_actionState = null;
        private eSpecialBuffState m_specialBuffState = eSpecialBuffState.None;
        public CombatEventState EventState
        {
            get { return m_eventState; }
            set { m_eventState = value; }
        }
        public CombatActionState ActionState
        {
            get { return m_actionState; }
            set { m_actionState = value; }
        }
        public eSpecialBuffState SpecialBuffState
        {
            get { return m_specialBuffState; }
            set { m_specialBuffState = value; }
        }

        private float m_storedAnimatorSpeed = -1.0f;
        private float m_setAnimatorSpeed = -1.0f;
        public HashSet<CombatantIndex> LTTargets
        {
            get;
            set;
        }

        public DynamicMonoILRObject HealthBar { get; set; }

        private bool m_IsInCombatView = false;

        //public bool Converge
        //{
        //    get { return HealthBar.Converge; }
        //    set { HealthBar.Converge = value; }
        //}

        public string myName
        {
            get
            {
                CombatLogic.CombatTeam team = (CombatLogic.CombatTeam)Index.TeamIndex;
                if (gameObject.gameObject.transform.parent)
                {
                    return string.Format("{0}.{1}.{2}", team.ToString(), gameObject.gameObject.transform.parent.gameObject.name, gameObject.gameObject.name);//team.ToString() + "." + gameObject.transform.parent.gameObject.name + "." + gameObject.name;
                }
                return string.Format("{0}.{1}", team.ToString(), gameObject.gameObject.name);
            }
        }
        bool preExit = false;

        public void Awake()
        {
            preExit = false;
            m_moveController = gameObject.GetComponent<MoveController>();
            m_fxHelper = gameObject.GetComponent<MoveEditor.FXHelper>();
            Attributes = new CombatantAttributes();
            m_hitMono = gameObject.gameObject.AddComponent<HitMono>();

            //HealthBar = gameObject.transform.GetMonoILRComponentByClassPath<HealthBar2D>("Hotfix_LT.Combat.HealthBar2D");
            HealthBar = gameObject.transform.GetMonoILRComponentByClassPath<DynamicMonoILRObject>("Hotfix_LT.Combat.HealthBar2D", false);

            if (HealthBar == null)
            {
                HealthBar = gameObject.AddMonoILRComponent<DynamicMonoILRObject>("Hotfix_LT.Combat.HealthBar2D");
            }
        }

        void Start()
        {
            preExit = false;
            if (m_collider == null)
            {
                m_collider = GetComponent<CapsuleCollider>();
            }

            if (m_fxHelper == null)
            {
                m_fxHelper = GetComponent<MoveEditor.FXHelper>();
                m_fxHelper.Init(this);
            }


            if (colorScale == null)
            {
                colorScale = GetComponent<CharacterColorScale>();
            }

            if (m_animator == null)
            {
                m_animator = GetComponent<Animator>();
            }

            if (m_moveController == null)
            {
                m_moveController = GetComponent<MoveController>();
                m_moveController.Initialize();
            }

            if (m_moveMotion == null)
            {
                m_moveMotion = m_moveController.MoveMotion;
            }

            if (Avatar == null)
            {
                Avatar = GetComponent<AvatarComponent>();
            }

            if (Attributes == null)
            {
                Attributes = new CombatantAttributes();
            }
        }
        private void OnEnable()
        {
            preExit = false;
        }
        private void SetupShadow()
        {
            if (m_shadowPlane == null)
            {
                EB.Assets.LoadAsync("Rendering/ShadowPlane", typeof(GameObject), o =>
                {
                    if (o)
                    {
                        m_shadowPlane = GameObject.Instantiate(o) as GameObject;
                        Vector3 pos = new Vector3(0f, 0.1f, 0f);//m_shadowPlane.transform.localPosition;
                        m_shadowPlane.transform.SetParent(gameObject.transform);
                        m_shadowPlane.transform.localPosition = pos;
                        EB.Debug.Log("ShadowPlane!!=>pos =({0},{1},{2})", m_shadowPlane.transform.localPosition.x.ToString(), m_shadowPlane.transform.localPosition.y.ToString(), m_shadowPlane.transform.localPosition.z.ToString());
                        m_shadowPlane.transform.localRotation = Quaternion.identity;
                        m_shadowPlaneY = m_shadowPlane.transform.position.y;
                        m_shadowPlane.CustomSetActive(true);
                    }
                });
            }
            else
            {
                m_shadowPlane.CustomSetActive(true);
            }
        }
        public void OnDisable()
        {
            if (Hotfix_LT.UI.LTCombatEventReceiver.IsCombatInit())
            {
                if (GetMainCamera != null)
                {
                    CombatCamera combat_camera = GetMainCamera.GetComponent<CombatCamera>();
                    if (combat_camera != null)
                    {
                        combat_camera.CheckUnregisterTarget(gameObject);
                    }
                }
            }
        }

        public void PreExit()
        {
            preExit = true;
            DeleteTimer(ref _DoShowBuffFloatFont_Seq);
            DeleteTimer(ref _OnSpawnProjectile_Seq);
            DeleteTimer(ref _OnHitFramerHandler_Seq);
            DeleteTimer(ref _OnHitDamageDatasFramerHandler_seq);
            DeleteTimer(ref _WaitAndRcoverAnimation_Seq);
        }

        private void DeleteTimer(ref int seq)
        {
            if (seq != 0)
            {
                TimerManager.instance.RemoveTimerSafely(ref seq);
            }
        }
        

        public bool OnHandleMessage(string methodName, object value)
        {
            if (string.Equals(methodName, "OnSetAnimationSpeed"))
            {
                if (m_moveController != null)
                {
                    m_moveController.OnSetAnimationSpeed(value as MoveEditor.MoveAnimationEvent);
                }
            }
            else if (string.Equals(methodName, "OnPlayParticle"))
            {
                OnPlayParticle(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnPlayAudio"))
            {
                OnPlayAudio(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnPlayCameraShake"))
            {
                m_fxHelper.OnPlayCameraShakeEx(value as MoveEditor.MoveAnimationEvent, this.IsCurrentAttackCritical());
            }
            else if (string.Equals(methodName, "OnPlayDynamicLight"))
            {
                m_fxHelper.OnPlayDynamicLight(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnModifyEnvironmentLighting"))
            {
                m_fxHelper.OnModifyEnvironmentLighting(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnPostFxBloomEvent"))
            {
                m_fxHelper.OnPostFxBloomEvent(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnPostFxVignetteEvent"))
            {
                m_fxHelper.OnPostFxVignetteEvent(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnPostFxWarpEvent"))
            {
                m_fxHelper.OnPostFxWarpEvent(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnInflictHit"))
            {
                OnInflictHit(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "PlayTargetHitReaction"))
            {
                PlayTargetHitReaction(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnTriggerFrame"))
            {
                OnTriggerFrame(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnPauseEvent"))
            {
                OnPauseEvent(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnCombatTimeScale"))
            {
                OnCombatTimeScale(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnSpawnProjectile"))
            {
                OnSpawnProjectile(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnCameraMotion"))
            {
                OnCameraMotion(value as MoveEditor.MoveAnimationEvent);
            }
            else if (string.Equals(methodName, "OnSwitchRenderSettings"))
            {
                OnSwitchRenderSettings(value as MoveEditor.MoveAnimationEvent);
            }
            else
            {
                return false;
            }
            return true;
        }

        public void Update()
        {
            m_IsInCombatView = GameFlowControlManager.IsInView() && Hotfix_LT.UI.LTCombatEventReceiver.IsCombatInit();
            if (!m_IsInCombatView)
            {
                return;
            }

            if (!Ready && Avatar != null && Avatar.Ready)
            {
                SetMeshRendererData();
                Ready = true;
            }

            UpdateStates();
        }

        public void SetupCombat(CombatCharacterSyncData data)
        {
            if (Attributes == null)
            {
                Attributes = new CombatantAttributes();
            }
            Attributes.SetIntAttr(CombatantAttributes.eAttribute.Hp, data.Hp);
            Attributes.SetIntAttr(CombatantAttributes.eAttribute.MaxHp, data.MaxHp);

            m_baseForward = transform.forward;
            m_startRotation = transform.rotation;
            if (!GameFlowControlManager.IsInView())
            {
                return;
            }
            HealthBar.OnHandleMessage("InitHealthBar", null); //HealthBar.InitHealthBar();
            HealthBar.OnHandleMessage("SetRoleAttr", data.Attr); //HealthBar.RoleAttr = data.Attr;

            HealthBar.OnHandleMessage("SetHp", data.Hp);
            HealthBar.OnHandleMessage("SetMaxHp", data.MaxHp);

            HealthBar.OnHandleMessage("SetContainer", false); //HealthBar.HealthBar.container.CustomSetActive(false);

            if (data.IsBoss)
            {
                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_InitializeData", data);
                }
            }

            if (m_collider == null)
            {
                m_collider = GetComponent<CapsuleCollider>();

                if (m_collider.direction == 1)
                {
                    m_collider.radius *= 1.4f;
                    m_collider.height *= 1.2f;
                }
                else
                {
                    m_collider.radius *= 1.4f;
                }
            }

            m_collider.enabled = false;

            if (Avatar == null)
            {
                Avatar = GetComponent<AvatarComponent>();
            }

            Ready = Avatar == null || Avatar.Ready;
            if (Ready)
            {
                SetMeshRendererData();
            }

            if (m_moveController == null)
            {
                m_moveController = gameObject.GetComponent<MoveController>();
                m_moveController.Initialize();
            }
            for (int i = 0; i < m_moveController.m_Moves.Length; ++i)
            {
                if (m_moveController.m_Moves[i] != null)
                {
                    m_moveController.m_Moves[i].Init();
                }
                else
                {
                    string str = string.Format("<color=#ff0000>战斗需要的动作事件控件为空了</color>{0}", this.name);
                    EB.Debug.LogError(str);
                    EB.Debug.LogCombat(str);
                }
            }
            if (m_fxHelper == null)
            {
                m_fxHelper = GetComponent<MoveEditor.FXHelper>();
                m_fxHelper.Init(this);
            }
            m_fxHelper.Flip(CombatLogic.Instance.IsOpponentSide(data.Index.TeamIndex));
            Data = data;
        }

        private void OnPlayParticle(MoveEditor.MoveAnimationEvent ee)
        {
            MoveEditor.ParticleEventInfo evt = ee.EventRef as MoveEditor.ParticleEventInfo;

            bool tar = false;
            if (evt._particleProperties._spawnAtOpponent)
            {
                tar = true;
            }

            if (evt._particleProperties._applyOnTargetList)
            {
                evt._particleProperties._applyOnTargetList = false;

                Combatant[] targets = this.GetCurrentTargets();
                if (targets == null)
                {
                    return;
                }

                for (int i = 0; i < targets.Length; i++)
                {
                    Combatant target = targets[i];
                    if (target != null && target.FXHelper != null)
                    {
                        if (tar)
                        {
                            target.FXHelper.OnPlayParticleEx(evt, tar, target.transform.position, target.Animator, target.OriginPosition, target.HitPoint);
                        }
                        else
                        {
                            target.FXHelper.OnPlayParticleEx(evt);
                        }
                    }
                }
            }
            else
            {
                if (tar)
                {
                    Combatant target = this.GetAttackTarget();
                    target.FXHelper.OnPlayParticleEx(evt, tar, target.transform.position, target.Animator, target.OriginPosition, target.HitPoint);
                }
                else
                {
                    m_fxHelper.OnPlayParticleEx(evt);
                }
            }
        }

        private void OnPlayAudio(MoveEditor.MoveAnimationEvent ee)
        {
            MoveEditor.AudioEventInfo evt = ee.EventRef as MoveEditor.AudioEventInfo;
            if (evt._audioProperties._applyOnTargetList)
            {
                evt._audioProperties._applyOnTargetList = false;

                Combatant[] targets = this.GetCurrentTargets();
                if (targets == null)
                {
                    return;
                }

                for (int i = 0; i < targets.Length; i++)
                {
                    Combatant target = targets[i];
                    if (target != null && target.FXHelper != null) target.FXHelper.OnPlayAudioEx(evt);
                }
            }
            else
            {
                m_fxHelper.OnPlayAudioEx(evt);
            }
        }



        private Camera _mainCamera = null;
        private Camera GetMainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }
                return _mainCamera;
            }
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!GameFlowControlManager.IsInView())
            {
                return;
            }

            if (!Hotfix_LT.UI.LTCombatEventReceiver.IsCombatInit())
            {
                return;
            }

            if (gameObject.transform.parent == null)
            {
                return;
            }

            Combatant[] targets = GetTargets();
            if (targets != null && targets.Length > 0)
            {
                Vector3 attackLocation = targets[0].gameObject.gameObject.transform.position;
                attackLocation.y += 1.0f;
                Gizmos.color = Color.red;
                //Gizmos.DrawSphere (attackLocation, 0.5f);
                Vector3 myLocation = OriginPosition;
                myLocation.y += 1.0f;
                Gizmos.DrawLine(attackLocation, myLocation);
            }

            if (m_moveController == null)
            {
                return;
            }
            switch (m_moveController.CurrentState)
            {
                case MoveController.CombatantMoveState.kIdle:
                case MoveController.CombatantMoveState.kReady:
                    Gizmos.color = Color.green;
                    break;
                case MoveController.CombatantMoveState.kDeath:
                    Gizmos.color = Color.red;
                    break;
                case MoveController.CombatantMoveState.kAttackTarget:
                    Gizmos.color = Color.white;
                    //Gizmos.DrawSphere(m_attack_location, 0.15f);
                    Gizmos.color = Color.magenta;
                    break;
                case MoveController.CombatantMoveState.kHitReaction:
                    Gizmos.color = Color.white;
                    break;
                case MoveController.CombatantMoveState.kVictoryDance:
                    Gizmos.color = Color.cyan;
                    break;
                case MoveController.CombatantMoveState.kRevive:
                    Gizmos.color = Color.yellow;
                    break;
            }
            Gizmos.DrawSphere(OriginPosition, 0.25f);
            Gizmos.DrawWireSphere(OriginPosition, this.Radius);
        }

        private void SetMeshRendererData()
        {
            if (skinnedMeshRenderer == null)
            {
                skinnedMeshRenderer = gameObject.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i < skinnedMeshRenderer.Length; ++i)
            {
                Vector3 myExtents = skinnedMeshRenderer[i].bounds.extents;
                myExtents.y = 0;
                m_meshRadius = Mathf.Max(m_meshRadius, myExtents.magnitude);
            }
        }

        private void UpdateStates()
        {
            var ast = ActionState;
            ast?.Update();
            EventState?.Update();
            UpdateToReadyState(ast);
        }

        public void TryEntryAction()
        {
            var theEntryMove = m_moveController.GetMoveByState(MoveController.CombatantMoveState.kEntry);
            if ((theEntryMove == null) || CombatSyncData.Instance.isResume)
            {
                if (ActionState == null)
                    SetActionState(GetActionState<ReadyActionState>().SetAutoCrossFade(false));
                else if (GetMoveState() != MoveController.CombatantMoveState.kReady)
                    EB.Debug.LogError("TryEntryAction: is Not Ready State");
            }
            else
                SetActionState(GetActionState<EntryActionState>().SetAutoCrossFade(false));
        }

        public void TrySpecialAcition(string MoveName)
        {
            SkillActionState skillAction = GetActionState<SkillActionState>();
            skillAction.MoveName = MoveName;
            SetActionState(skillAction);
        }
        private WaitForSeconds waitSpawnFxStayTime = new WaitForSeconds(0.2f);
        public void Spawn()
        {
            //出场特效
            if (Data != null && Data.Attr != -1)
            {
                var actEntry = new Johny.Action.ActionGeneralParticle(0.0f, gameObject, "fx_TY_chuchang_huo", Vector3.zero, Vector3.zero);
                FusionAudio.PostEvent("SFX/General/CharacterSpawn");
            }
            //出场动作
            StartCoroutine(SpawnCoroutine());

        }
        public IEnumerator SpawnCoroutine()
        {
            Hotfix_LT.UI.LTCombatEventReceiver.Instance.SetAnimState(Hotfix_LT.UI.LTCombatEventReceiver.AnimState.Spawning);
            transform.localPosition = new Vector3(1000, 0);
            while (LTCombatEventReceiver.Instance.spawnCameraing)
            {
                yield return null;
            }
            transform.localPosition = Vector3.zero;

            if (Data != null && !Data.IsBoss)
            {
                TryEntryAction();
            }
            Hotfix_LT.UI.LTCombatEventReceiver.Instance.UnsetAnimState(Hotfix_LT.UI.LTCombatEventReceiver.AnimState.Spawning);
        }

        public void CleanCombat()
        {
            PauseAnimation(-1.0f);
            if (IsLaunch())
            {
                StopLaunch();
            }
            if (EventState != null)
            {
                EventState.Stop();
            }
            if (ActionState != null)
            {
                ActionState.Stop();
            }

            FXHelper.StopAll(true);
            EndRevive();
            EndExile();

            if (HealthBar != null)
            {
                //HealthBar.DestroyHealthBar();
                HealthBar.OnHandleMessage("DestroyHealthBar", null);
            }

            ReleaseStates();

            Attributes.Clear();
            if (Data != null && Data.IsBoss)
            {
                //if (Hotfix_LT.UI.LTCombatHudController.Instance != null && Hotfix_LT.UI.LTCombatHudController.Instance.BossHealthBarCtrl != null)
                //{
                //    Hotfix_LT.UI.LTCombatHudController.Instance.BossHealthBarCtrl.CleanUp();
                //}

                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_CleanUp");
                }
            }
            Data = null;
            m_impactContainer.Clear();
            Ready = false;

            if (m_shadowPlane != null)
            {
                m_shadowPlane.CustomSetActive(false);
            }

            MoveController.TransitionTo(MoveController.CombatantMoveState.kReady);
        }

        public int GetTurn()
        {
            if (!GameFlowControlManager.IsInView())
            {
                return 0;
            }

            return Hotfix_LT.UI.LTCombatEventReceiver.Instance.Turn;
        }

        public MoveController.CombatantMoveState GetMoveState()
        {
            if (MoveController == null)
            {
                return MoveController.CombatantMoveState.kIdle;
            }

            return m_moveController.CurrentState;
        }

        public bool IsDead()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kDeath;
        }

        public bool CanSelect()
        {
            bool canSelect = true;
            if (Data != null && Data.Limits != null)
            {
                canSelect = !Data.Limits.Contains("noSelect");
            }

            canSelect = canSelect && IsCouldSetSkill(this);

            return canSelect;
        }

        public bool IsIdle()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kIdle || GetMoveState() == MoveController.CombatantMoveState.kReady;
        }

        public bool IsLaunch()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kLaunch;
        }

        public bool IsForward()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kForward;
        }

        public bool IsBackward()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kBackward;
        }

        public bool IsAttacking()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kAttackTarget;
        }

        public bool IsBeingAttacked()
        {
            return GetMoveState() == MoveController.CombatantMoveState.kHitReaction;
        }

        /// <summary>
        /// 判断该目标是否可以成为技能释放的对象
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool IsCouldSetSkill(Combatant target)
        {
            ///10001，该技能是一个被动技能，又该技能怪物无法被技能选择成为目标
            bool isCould = true;

            if (target != null && target.Data != null && target.Data.SkillDataList != null)
            {
                var enumerator = target.Data.SkillDataList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Key == 10001)
                    {
                        isCould = false;
                        break;
                    }
                }
            }

            return isCould;
        }

        public void SetActionState(CombatActionState action_state)
        {
            if (ActionState == null)
            {
                ActionState = action_state;
                ActionState.Start();
            }
            else
            {
                ActionState.SwitchState(action_state);
            }
        }

        public void StartEffect(CombatEffectEvent effect_event)
        {
            if (effect_event.EffectType == eCombatEffectType.REACTION)
            {
                ReactionEffectEvent effect = effect_event as ReactionEffectEvent;
                StartReaction(effect);
            }
            else if (effect_event.EffectType == eCombatEffectType.DAMAGE)
            {
                DamageEffectEvent effect = effect_event as DamageEffectEvent;
                StartDamage(effect);
            }
            else if (effect_event.EffectType == eCombatEffectType.HEAL)
            {
                HealEffectEvent effect = effect_event as HealEffectEvent;
                StartHeal(effect);
            }
        }

        public void StartDamage(DamageEffectEvent damage_event)
        {
            SetHP(GetHP() - damage_event.Damage);
            //Hotfix_LT.Messenger.Raise<CombatDamageEvent>(Hotfix_LT.EventName.CombatDamageEvent, new CombatDamageEvent(gameObject.gameObject, damage_event.Damage, damage_event.Show, damage_event.DamageType));
            GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "RaiseCombatDamageEvent", new CombatDamageEvent(gameObject.gameObject, damage_event.Damage, damage_event.Show, damage_event.DamageType));
            //EventManager.instance.Raise(new CombatDamageEvent(gameObject.gameObject, damage_event.Damage, damage_event.Show, damage_event.DamageType));
        }

        public void StartHeal(HealEffectEvent heal_event)
        {
            if (heal_event.Heal > 0)
            {
                SetHP(GetHP() + heal_event.Heal);
                //Hotfix_LT.Messenger.Raise<CombatHealEvent>(Hotfix_LT.EventName.CombatHealEvent, new CombatHealEvent(gameObject.gameObject, heal_event.Heal, heal_event.Show, heal_event.HealType));
                GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "RaiseCombatHealEvent", new CombatHealEvent(gameObject.gameObject, heal_event.Heal, heal_event.Show, heal_event.HealType));
                //EventManager.instance.Raise(new CombatHealEvent(gameObject.gameObject, heal_event.Heal, heal_event.Show, heal_event.HealType));
            }
        }

        public void EndExile() { }
        public void EndRevive() { }

        public void StartReaction(ReactionEffectEvent reaction_event)
        {
            ReactionEventState reaction = GetEventState<ReactionEventState>(reaction_event);
            if (EventState != null)
            {
                //EB.Debug.Log("Combatant.StartReaction: transition to reaction, " + myName + " " + reaction_event.Reaction);
                EventState.TransitionTo(reaction);
            }
            else
            {
                //EB.Debug.Log("Combatant.StartReaction: start reaction event state, " + myName + " " + reaction_event.Reaction);
                EventState = reaction;
                EventState.Start();
            }
        }

        public void StartSkill(CombatSkillEvent skill_event)
        {
            if (ActionState is HitActionState)
            {
                HitActionState hitAS = ActionState as HitActionState;
                hitAS.Stop();
            }

            if (EventState != null)
            {
                if (EventState is SkillEventState)
                {
                    EB.Debug.LogWarning("EventState is SkillEventState");
                    EventState.Interrupt();
                }
                else
                {
                    EventState.Stop();
                }
            }

            EventState = GetEventState<SkillEventState>(skill_event);
            EventState.Start();
        }


        public void StopLaunch()
        {
            m_needLaunch = false;
            m_launchSkill = -1;

            if (IsLaunch())
            {
                SetActionState(GetActionState<ReadyActionState>().SetAutoCrossFade(true));
            }
        }

        public void CleanLaunch()
        {
            m_needLaunch = false;
            m_launchSkill = -1;
        }

        /// <summary>
        /// 设置克制箭头
        /// </summary>
        /// <param name="attr"></param>
        public void SetRestrainFlag(int attr)//Hotfix_LT.Data.eRoleAttr attr)
        {
            //HealthBar.SetRestrainFlag(attr);
            HealthBar.OnHandleMessage("SetRestrainFlag", attr);
        }

        /// <summary>
        /// 设置克制箭头为克制状态
        /// </summary>
        public void SetGainFlag()
        {
            //HealthBar.SetGainFlag();
            HealthBar.OnHandleMessage("SetGainFlag", null);
        }

        /// <summary>
        /// 隐藏克制箭头
        /// </summary>
        public void HideRestrainFlag()
        {
            //HealthBar.HideRestrainFlag();
            HealthBar.OnHandleMessage("HideRestrainFlag", null);
        }

        public void UpdateMoveBar(float value)
        {
            if (Data != null && !Data.IsBoss)
            {
                //HealthBar.UpdateMoveBar(value);
                HealthBar.OnHandleMessage("UpdateMoveBar", value);
            }
            else
            {
                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_UpdateMoveBar", value);
                }
            }
        }

        private bool isMarkForBoss = false;
        /// <summary>
        /// 是否被boss标记了
        /// </summary>
        public bool IsMarkForBoss
        {
            get { return isMarkForBoss; }
        }

        Dictionary<int, PraticleLevelHelper> m_praticleLevelHelperDictionary = new Dictionary<int, PraticleLevelHelper>();

        public bool HavaAwakeningBuff()
        {
            if (Data != null)
            {
                var enumerator = Data.BuffDatas.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.Id == 725)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private string GetStrToolFunc(object fx)
        {
            string result = string.Empty;
            if (fx != null)
            {
                result = fx.ToString();
            }

            return result;
        }
        public void UpdateBuff(ICollection<CombatCharacterSyncData.BuffData> buffDatas)
        {
            if (Data != null && !Data.IsBoss)
            {
                //HealthBar.UpdateBuff(buffDatas);
                HealthBar.OnHandleMessage("UpdateBuff", buffDatas);
            }
            else
            {
                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_UpdateBuff", buffDatas);
                }
            }


            isMarkForBoss = false;

            var enumerator = buffDatas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CombatCharacterSyncData.BuffData buffData = enumerator.Current;
                ImpactData impact_data = new ImpactData();
                impact_data.ImpactId = buffData.Id;

                Hashtable buffInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferInfo", buffData.Id);
                if ((bool)buffInfo["fxinvisible"])
                    continue;
                int maxStack = (int)buffInfo["stacknum"];
                int realActiveStack = buffData.LeftTurnArray.Length;
                //if (realActiveStack>1 && string.IsNullOrEmpty(impact_template.LoopFX))
                if (realActiveStack > maxStack)
                {
                    //EB.Debug.LogWarning("realActiveStack > maxStack buffid:" + impact_data.ImpactId);
                    realActiveStack = maxStack;
                }
                impact_data.CurStack = realActiveStack;

                if (buffData.Id == 701)
                {
                    isMarkForBoss = true;
                }

                if (!(bool)buffInfo["isActiveImmediately"] && !m_impactContainer.HasImpactData(buffData.Id))
                {
                    m_impactContainer.AddImpactData(impact_data);
                    {
                        string onceFx = GetStrToolFunc(buffInfo["OnceFX"]);
                        string onceFxEventName = GetStrToolFunc(buffInfo["OnceFXEventName"]);
                        string loopFx = GetStrToolFunc(buffInfo["LoopFX"]);
                        string loopFXEventName = GetStrToolFunc(buffInfo["LoopFXEventName"]);

                        ParticleSystem ps = ActiveImpact(onceFx, onceFxEventName, (MoveEditor.BodyPart)((int)buffInfo["OnceFXAttachment"]), loopFx, loopFXEventName, (MoveEditor.BodyPart)((int)buffInfo["LoopFXAttachment"]));
                        PraticleLevelHelper plHelper;
                        if (ps != null && (plHelper = ps.transform.GetComponent<PraticleLevelHelper>()) != null)
                        {
                            if (!m_praticleLevelHelperDictionary.ContainsKey(impact_data.ImpactId))
                                m_praticleLevelHelperDictionary.Add(impact_data.ImpactId, plHelper);
                        }
                    }
                }

                if (m_praticleLevelHelperDictionary.ContainsKey(impact_data.ImpactId))
                {
                    m_praticleLevelHelperDictionary[impact_data.ImpactId].SetLevel(impact_data.CurStack, null);
                }
            }

            int cnt = m_impactContainer.GetImpactCount();
            List<ImpactData> notExistList = new List<ImpactData>();
            for (int i = 0; i < cnt; ++i)
            {
                ImpactData data = m_impactContainer.GetImpactData(i);
                bool found = false;
                var buffEnumerator = buffDatas.GetEnumerator();
                while (buffEnumerator.MoveNext())
                {
                    CombatCharacterSyncData.BuffData buff = buffEnumerator.Current;
                    if (buff.Id == data.ImpactId)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) notExistList.Add(data);
                //if (buffDatas.Find(buff => buff.Id == data.ImpactId) == null)
                //{
                //	notExistList.Add(data);
                //}
            }

            var existListEnumerator = notExistList.GetEnumerator();
            while (existListEnumerator.MoveNext())
            {
                ImpactData data = existListEnumerator.Current;
                FadeoutImpact(data);

                if (m_praticleLevelHelperDictionary.ContainsKey(data.ImpactId))
                    m_praticleLevelHelperDictionary.Remove(data.ImpactId);
            }

            UpdateSpecialBuffActionState(buffDatas);
        }

        public void ShowBuffFloatFont()
        {
            if (FloatTexts.Count > 0 && _DoShowBuffFloatFont_Seq == 0)
            {
                if (!preExit)
                {
                    _DoShowBuffFloatFont_Seq = TimerManager.instance.AddTimer(1, 0, DoShowBuffFloatFont);
                }
            }
        }

        private Queue<GameEvent> FloatTexts = new Queue<GameEvent>();
        #region Coroutine -> timer
        private int _DoShowBuffFloatFont_Seq = 0;
        private void DoShowBuffFloatFont(int seq)
        {
            if (FloatTexts.Count > 0)
            {
                var e = FloatTexts.Dequeue();
                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatDamagesHUDController", "Instance", "OnCombatFloatFontListener", e as CombatFloatFontEvent);
                }
            }

            if (FloatTexts.Count == 0)
            {
                TimerManager.instance.RemoveTimerSafely(ref _DoShowBuffFloatFont_Seq);
            }
        }
        #endregion

        public void FlashEventFont(string s)
        {
            var e = new CombatFloatFontEvent(this, EB.Localizer.GetString("ID_COMBATFLASH_" + s), 2, damageTextOffset);
            FloatTexts.Enqueue(e);
        }
        public void BuffFloatFont(CombatBuffSyncData buffData)
        {
            Hashtable buffInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferInfoEx", buffData.ID);

            if (buffData.ID/100 == 8)
            {
                FloatTexts.Enqueue(new CombatFloatFontEvent(this, GetStrToolFunc(buffInfo["Name"]), 3, damageTextOffset));
            }
            else
            {
                FloatTexts.Enqueue(new CombatFloatFontEvent(this, GetStrToolFunc(buffInfo["Name"]), 2, damageTextOffset));
            }
            List<int> buffIndex = (List<int>)buffInfo["BuffFloatFont"];
            List<string> buffStrings = (List<string>)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "GetCombatFloatFont", buffInfo["BuffFloatFont"]);
            int len = buffStrings.Count;
            for (int i = 0; i < len; i++)
            {
                int type = 0;
                if (buffIndex[i] < 0)
                {
                    type = 1;
                }
                FloatTexts.Enqueue(new CombatFloatFontEvent(this, buffStrings[i], type, damageTextOffset));
            }
        }

        public ParticleSystem ActiveImpact(string OnceFX, string OnceFXEventName, MoveEditor.BodyPart OnceFXAttachment, string LoopFX, string LoopFXEventName, MoveEditor.BodyPart LoopFXAttachment)
        {
            // once fx
            if (!string.IsNullOrEmpty(OnceFX))
            {
                MoveEditor.ParticleEventProperties once_properties = new MoveEditor.ParticleEventProperties();
                once_properties.ParticleName = OnceFX;
                once_properties.FlippedParticleName = OnceFX;
                once_properties._eventName = OnceFXEventName;
                once_properties._bodyPart = OnceFXAttachment;
                once_properties._parent = true;
                once_properties._stopOnOverride = true;
                once_properties._stopOnExit = false;
                once_properties._interruptable = false;

                FXHelper.PlayParticle(once_properties, false);
            }

            // loop fx

            ParticleSystem ret_ps = null;
            if (!string.IsNullOrEmpty(LoopFX))
            {
                MoveEditor.ParticleEventProperties loop_properties = new MoveEditor.ParticleEventProperties();
                loop_properties.ParticleName = LoopFX;
                loop_properties.FlippedParticleName = LoopFX;
                loop_properties._eventName = LoopFXEventName;
                loop_properties._bodyPart = LoopFXAttachment;
                loop_properties._parent = true;
                loop_properties._stopOnOverride = true;// override
                loop_properties._stopOnExit = false;
                loop_properties._interruptable = false;

                if (HealthBar != null)
                {
                    Hashtable ht = Johny.HashtablePool.Claim();
                    ht.Add("properties", loop_properties);
                    ht.Add("forcePlay", false);
                    ret_ps = HealthBar.GetValueFrom("PlayParticle", ht) as ParticleSystem;  //HealthBar.PlayParticle(loop_properties, false);
                }
                else
                {
                    ret_ps = FXHelper.PlayParticle(loop_properties, false);
                }
            }
            return ret_ps;
        }

        public void RestoreLoopImpactFX()
        {
            int cnt = m_impactContainer.GetImpactCount();
            for (int i = 0; i < cnt; ++i)
            {
                ImpactData data = m_impactContainer.GetImpactData(i);
                Hashtable buffInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferInfo", data.ImpactId);

                string loopfx = GetStrToolFunc(buffInfo["LoopFX"]);
                if (!string.IsNullOrEmpty(loopfx))
                {
                    MoveEditor.ParticleEventProperties loop_properties = new MoveEditor.ParticleEventProperties();
                    loop_properties.ParticleName = loopfx;
                    loop_properties.FlippedParticleName = loopfx;
                    loop_properties._eventName = GetStrToolFunc(buffInfo["LoopFXEventName"]);
                    loop_properties._bodyPart = (MoveEditor.BodyPart)((int)buffInfo["LoopFXAttachment"]);
                    loop_properties._parent = true;
                    loop_properties._stopOnOverride = false;// override
                    loop_properties._stopOnExit = false;
                    loop_properties._interruptable = false;

                    FXHelper.PlayParticle(loop_properties, false);
                }
            }
        }

        public void FadeoutImpact(ImpactData impact_data)
        {
            int impact_id = impact_data.ImpactId;
            Hashtable buffInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferInfo", impact_id);

            m_impactContainer.RemoveImpactData(impact_id);

            int left_count = m_impactContainer.GetImpactCount(impact_id);
            if (left_count <= 0)
            {
                // once fx
                if (!string.IsNullOrEmpty(GetStrToolFunc(buffInfo["OnceFX"])))
                {
                    FXHelper.StopParticle(GetStrToolFunc(buffInfo["OnceFXEventName"]), true);
                }

                // loop fx
                if (!string.IsNullOrEmpty(GetStrToolFunc(buffInfo["LoopFX"])))
                {
                    FXHelper.StopParticle(GetStrToolFunc(buffInfo["LoopFXEventName"]), true);

                    if (string.Equals(GetStrToolFunc(buffInfo["LoopFX"]), "fx_p_P1_beidong_loop"))
                    {
                        MoveEditor.ParticleEventProperties once_properties = new MoveEditor.ParticleEventProperties();
                        string fxName = "fx_p_P1_beidong_bj";
                        once_properties.ParticleName = fxName;
                        once_properties.FlippedParticleName = fxName;
                        once_properties._eventName = fxName;
                        once_properties._bodyPart = (MoveEditor.BodyPart)((int)buffInfo["LoopFXAttachment"]);
                        once_properties._parent = true;
                        once_properties._stopOnOverride = true;
                        once_properties._stopOnExit = false;
                        once_properties._interruptable = false;

                        FXHelper.PlayParticle(once_properties, false);
                    }
                }
            }
        }

        public void RemoveLoopImpactFX()
        {
            int cnt = m_impactContainer.GetImpactCount();
            for (int i = 0; i < cnt; ++i)
            {
                ImpactData data = m_impactContainer.GetImpactData(i);
                Hashtable buffInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferInfo", data.ImpactId);

                if (!string.IsNullOrEmpty(GetStrToolFunc(buffInfo["LoopFX"])))
                {
                    FXHelper.StopParticle(GetStrToolFunc(buffInfo["LoopFXEventName"]), true);
                }
            }
        }

        void UpdateSpecialBuffActionState(ICollection<CombatCharacterSyncData.BuffData> buffDatas)
        {
            int intState = 0;
            var buffEnumerator = buffDatas.GetEnumerator();
            while (buffEnumerator.MoveNext())
            {
                CombatCharacterSyncData.BuffData buffdata = buffEnumerator.Current;
                Hashtable buffInfo = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.BuffTemplateManager", "Instance", "GetBufferInfoEx", buffdata.Id);

                if ((bool)buffInfo["SpecialStateInvisible"])
                    continue;
                if ((int)buffInfo["SpecialState"] > intState)
                    intState = (int)buffInfo["SpecialState"];
            }
            eSpecialBuffState currentState = (eSpecialBuffState)intState;
            if (currentState != m_specialBuffState)
            {
                if (currentState == eSpecialBuffState.None && GetMoveState() == MoveController.CombatantMoveState.kHitReaction)
                {
                    var hitAction = GetActionState<HitActionState>();
                    hitAction.Stop();
                    //SetActionState(GetActionState<ReadyActionState>().SetAutoCrossFade(true));
                }
                else if (GetMoveState() == MoveController.CombatantMoveState.kHitReaction)
                {
                    var currentActionState = ActionState as HitActionState;
                    currentActionState.SetSpecialBuffAction(currentState, m_specialBuffState);
                }
                else if (GetMoveState() == MoveController.CombatantMoveState.kReady)
                {
                    var currentActionState = ActionState as ReadyActionState;
                    currentActionState.SetSpecialBuffAction(currentState, m_specialBuffState);
                }
            }
            m_specialBuffState = currentState;
        }

        public void SetHP(long val)
        {
            if (Data == null)
            {
                return;
            }

            if (val > Data.MaxHp)
            {
                val = Data.MaxHp;
            }

            Data.Hp = val;
        }

        public long GetHP()
        {
            if (Data == null)
            {
                return 0;
            }

            return Data.Hp;
        }

        public long GetMaxHP()
        {
            if (Data == null)
            {
                return 0;
            }

            return Data.MaxHp;
        }

        public bool IsAlive()
        {
            return Data != null && !Data.Dead;
        }

        public void CallDeath()
        {
            if (!DeathOver)
            {
                return;
            }

            if (Data != null)
            {
                LTCombatEventReceiver.Instance.OnPlayerDeathAnimStart(Data.IngameId);
            }

            if (Data != null && !preExit)
            {
                GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "ClearConvergeInfo", Data.IngameId);
            }

            DeathOver = false;
            SetActionState(this.GetActionState<DeathActionState>());
            LTCombatEventReceiver.Instance.PlayRadialBlurEffect(this);
        }

        IEnumerator FallDown()
        {
            int timer = 0;
            while (true)
            {
                yield return new WaitForSeconds(1);
                timer++;
                gameObject.transform.position -= new Vector3(0, 20);

                if (timer >= 20)
                {
                    yield break;
                }
            }
        }

        public bool IsEntryActionPlayOver()
        {
            if (ActionState != null && ActionState is EntryActionState)
            {
                if (MoveController.GetCurrentStateInfo().normalizedTime >= 1)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public void TryVictoryDance()
        {
            if (GetHP() > 0)
            {
                SetActionState(GetActionState<VictoryActionState>());
            }
        }

        public void OnReady()
        {
            gameObject.transform.localPosition = Vector3.zero;
            if (GetMoveState() != MoveController.CombatantMoveState.kReady)
                SetActionState(GetActionState<ReadyActionState>().SetAutoCrossFade(false));
            else if (ActionState == null)
            {
                EB.Debug.LogWarning("OnReady: moveState=kReady ActionState = null");
                SetActionState(GetActionState<ReadyActionState>().SetAutoCrossFade(false));
            }
            if (Data != null && Data.IsBoss)
            {
                //Hotfix_LT.UI.LTCombatHudController.Instance.BossHealthBarCtrl.Show();
                //Hotfix_LT.UI.LTCombatHudController.Instance.BossHealthBarCtrl.UpdateHp(Data.Hp);
                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_Show");
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_UpdateHp", Data.Hp);
                }
            }
            else
            {
                //HealthBar.HealthBar.container.CustomSetActive(true); 
                HealthBar.OnHandleMessage("SetContainer", true);
            }

            if (Data != null)
            {
                UpdateMoveBar(Data.MoveBarValue);
                UpdateBuff(Data.BuffDatas.Values);
            }

            SetupShadow();
        }

        void UpdateToReadyState(CombatActionState ast)
        {
            if (ast is EntryActionState || ast is VictoryActionState)
            {
                if (MoveController.GetCurrentStateInfo().normalizedTime >= 1)
                {
                    if (Data != null && Data.IsBoss)
                    {
                        gameObject.transform.localPosition = Vector3.zero;
                    }
                    SetActionState(GetActionState<ReadyActionState>().SetAutoCrossFade(false));
                }
            }
        }

        public void ResetPosition()
        {
            gameObject.gameObject.transform.position = OriginPosition;
        }

        public void ResetRotation()
        {
            gameObject.gameObject.transform.rotation = m_startRotation;
        }

        public Vector3 GetTargetPosition()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return Vector3.zero;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.CalculateTargetPosition();
        }

        public float GetTargetRadius()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return 0.0f;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.GetTargetRadius();
        }

        public Combatant[] GetTargets()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return null;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.GetTargets<Combatant>().ToArray();
        }

        public Combatant[] GetCurrentTargets()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return null;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            if (skill_state.SkillEvent.TotalHitTimes <= 0)
            {
                return skill_state.GetTargets<Combatant>().ToArray();
            }
            else
            {
                return skill_state.GetHitTargets<Combatant>().ToArray();
            }
        }

        public Combatant GetAttackTarget()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return null;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.GetAttackTarget<Combatant>();
        }

        public List<GameObject> GetTargetGameObjects()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return null;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.GetTargets<GameObject>();
        }

        public bool IsCurrentAttackCritical()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return false;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.IsCriticalAttack();
        }

        public bool IsAttackingRightSide()
        {
            if (EventState == null || EventState is SkillEventState == false)
            {
                return false;
            }

            SkillEventState skill_state = EventState as SkillEventState;
            return skill_state.IsAttackRightSide();
        }

        private List<Transform> GetAllChildTransforms(Transform transformForSearch)
        {
            List<Transform> allChildTransforms = new List<Transform>();

            for (int i = 0, cnt = transformForSearch.childCount; i < cnt; ++i)
            {
                Transform trans = transformForSearch.GetChild(i);

                List<Transform> childTrans = GetAllChildTransforms(trans);
                allChildTransforms.Add(trans);
                allChildTransforms.AddRange(childTrans);
            }
            return allChildTransforms;
        }

        #region About States
        /// <summary>
        /// 请将所有状态注册于此，常驻内存
        /// </summary>
        /// <value></value>
        private Dictionary<System.Type, CombatActionState> m_actionStatePool = new Dictionary<System.Type, CombatActionState>{
            {typeof(AnimationOnceReactionActionState), new AnimationOnceReactionActionState()},
            {typeof(BackActionState), new BackActionState()},
            {typeof(BackStartActionState), new BackStartActionState()},
            {typeof(BackwardActionState), new BackwardActionState()},
            {typeof(BlockActionState), new BlockActionState()},
            {typeof(CollideActionState), new CollideActionState()},
            {typeof(DeathActionState), new DeathActionState()},
            {typeof(DownActionState), new DownActionState()},
            {typeof(EntryActionState), new EntryActionState()},
            {typeof(FallActionState), new FallActionState()},
            {typeof(FallBackActionState), new FallBackActionState()},
            {typeof(FallDownActionState), new FallDownActionState()},
            {typeof(FlowActionState), new FlowActionState()},
            {typeof(FlowDownActionState), new FlowDownActionState()},
            {typeof(FlowHitActionState), new FlowHitActionState()},
            {typeof(FlowStartActionState), new FlowStartActionState()},
            {typeof(FlowUpActionState), new FlowUpActionState()},
            {typeof(ForwardActionState), new ForwardActionState()},
            {typeof(HitActionState), new HitActionState()},
            {typeof(IdleActionState), new IdleActionState()},
            {typeof(LandActionState), new LandActionState()},
            {typeof(LaunchActionState), new LaunchActionState()},
            {typeof(LieActionState), new LieActionState()},
            {typeof(LieHitActionState), new LieHitActionState()},
            {typeof(ReactionActionState), new ReactionActionState()},
            {typeof(ReadyActionState), new ReadyActionState()},
            {typeof(ReturnActionState), new ReturnActionState()},
            {typeof(ReviveActionState), new ReviveActionState()},
            {typeof(RunActionState), new RunActionState()},
            {typeof(SkillActionState), new SkillActionState()},
            {typeof(StandUpActionState), new StandUpActionState()},
            {typeof(VictoryActionState), new VictoryActionState()}
        };

        public T GetActionState<T>() where T : CombatActionState, new()
        {
            CombatActionState state;
            if (m_actionStatePool.TryGetValue(typeof(T), out state))
            {
                state.CleanUp();
                state.Init(this);
                return state as T;
            }

            EB.Debug.LogError("请查看是否m_actionStatePool被clear了，或者有没注册的状态!!");
            return null;
        }

        private Dictionary<System.Type, Queue<CombatEventState>> m_eventStatePool = new Dictionary<System.Type, Queue<CombatEventState>>{
            {typeof(ReactionEventState), new Queue<CombatEventState>()},
            {typeof(SkillEventState), new Queue<CombatEventState>()}
        };

        public T GetEventState<T>(CombatEvent combat_event) where T : CombatEventState, new()
        {
            System.Type type = typeof(T);
            Queue<CombatEventState> queue = null;
            m_eventStatePool.TryGetValue(type, out queue);
            CombatEventState state = null;
            if (queue.Count > 0)
            {
                state = queue.Dequeue();
                state.CleanUp();
            }
            else
            {
                state = new T();
            }

            state.Init(this, combat_event);
            return state as T;
        }

        public void StashEventState<T>(T state) where T : CombatEventState
        {
            if (state == null)
            {
                EB.Debug.LogError("Combatant.StashEventState: state is null");
                return;
            }

            System.Type type = state.GetType();
            Queue<CombatEventState> queue;
            if (!m_eventStatePool.TryGetValue(type, out queue))
            {
                EB.Debug.LogError("请查看是否m_eventStatePool被clear了，或有状态未注册！！");
            }
            queue.Enqueue(state);
        }

        public void ReleaseStates()
        {
            if (EventState != null)
            {
                EventState = null;
            }

            #region clean m_actionStatePool
            {
                var it = m_actionStatePool.GetEnumerator();
                while (it.MoveNext())
                {
                    it.Current.Value.CleanUp();
                }
            }
            #endregion

            if (ActionState != null)
            {
                ActionState = null;
            }

            #region clean m_eventStatePool
            {
                var it = m_eventStatePool.GetEnumerator();
                while (it.MoveNext())
                {
                    var queue = it.Current.Value;
                    while (queue.Count > 0)
                    {
                        var state = queue.Dequeue();
                        state.CleanUp();
                    }
                    it.Current.Value.Clear();
                }
            }
            #endregion
        }
        #endregion

        public void SetupSelectFX()
        {
            string name = targetSelectSenderPS.Valiad ? targetSelectSenderPS.Name : defaultTargetSelectSenderPS;

            MoveEditor.ParticleEventProperties properties = new MoveEditor.ParticleEventProperties();
            properties._eventName = name;
            properties.FlippedParticleName = properties.ParticleName = name;
            properties._bodyPart = MoveEditor.BodyPart.Root;
            properties._parent = false;
            properties._stopOnOverride = false;
            properties._stopOnExit = false;
            properties._offset = new Vector3(0, 0.1f, 0);

            FXHelper.PlayParticle(properties, false);
            //OnPlayParticle(new MoveEditor.MoveAnimationEvent() { EventRef = new MoveEditor.ParticleEventInfo() { _particleProperties = properties } });
        }

        public void SetupSelectableFX()
        {
            string name = targetSelectTargetPS.Valiad ? targetSelectTargetPS.Name : defaultTargetSelectTargetPS;

            MoveEditor.ParticleEventProperties properties = new MoveEditor.ParticleEventProperties();
            properties._eventName = name;
            properties.FlippedParticleName = properties.ParticleName = name;
            properties._bodyPart = MoveEditor.BodyPart.Root;
            properties._parent = false;
            properties._stopOnOverride = true;
            properties._stopOnExit = true;

            FXHelper.PlayParticle(properties, false);
            //OnPlayParticle(new MoveEditor.MoveAnimationEvent() { EventRef = new MoveEditor.ParticleEventInfo() { _particleProperties = properties } });
        }

        public void RemoveSelectionFX(bool all)
        {
            string name = targetSelectSenderPS.Valiad ? targetSelectSenderPS.Name : defaultTargetSelectSenderPS;
            if (all || !IsLaunch())
            {
                FXHelper.StopParticle(name, true);
            }

            name = targetSelectTargetPS.Valiad ? targetSelectTargetPS.Name : defaultTargetSelectTargetPS;
            FXHelper.StopParticle(name, true);

            name = targetSelectTargetHeadPS.Valiad ? targetSelectTargetHeadPS.Name : defaultTargetSelectTargetHeadPS;
            FXHelper.StopParticle(name, true);
        }

        public void PlaySelectedFX(Vector3 worldPos)
        {
            string name = targetSelectPS.Valiad ? targetSelectPS.Name : defaultTargetSelectPS;

            MoveEditor.ParticleEventProperties properties = new MoveEditor.ParticleEventProperties();
            properties._eventName = name;
            properties.FlippedParticleName = properties.ParticleName = name;
            properties._bodyPart = MoveEditor.BodyPart.Root;
            properties._parent = false;
            properties._stopOnOverride = true;
            properties._stopOnExit = true;
            properties._offset = new Vector3(0, -0.06f, 0);
            FXHelper.PlayParticle(properties, false);
        }

        #region Animation Event handlers

        private void OnCombatTimeScale(MoveEditor.MoveAnimationEvent ee)
        {
            MoveEditor.CombatEventInfo info = new MoveEditor.CombatEventInfo(ee.stringParameter);
            m_combatTimeScale = info.timeScaleProps.timeScaleCurve;
            float timeScale = Time.timeScale;
            if (m_combatTimeScale != null && m_combatTimeScale.length > 0)
            {
                // normalized 0 to 1
                var stateInfoTime = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                timeScale = m_combatTimeScale.Evaluate(stateInfoTime - Mathf.Floor(stateInfoTime));
                timeScale *= Hotfix_LT.UI.LTCombatEventReceiver.Instance.TimeScale;
            }
            Time.timeScale = timeScale;
        }

        private void OnSwitchRenderSettings(MoveEditor.MoveAnimationEvent e)
        {
            MoveEditor.SwitchRenderSettingsEventInfo info = e.EventRef as MoveEditor.SwitchRenderSettingsEventInfo;
            if (string.IsNullOrEmpty(info._name))
            {
                RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.defaultSettings);
            }
            else
            {
                RenderSettingsManager.Instance.SetActiveRenderSettings(info._name);
            }
        }

        #region 分帧
        private MoveEditor.MoveAnimationEvent _OnSpawnProjectile_ee;
        private MoveEditor.ProjectileEventInfo _OnSpawnProjectile_info;
        private MoveEditor.ProjectileEventProperties _OnSpawnProjectile_projectileProps;
        private ProjectileMono _OnSpawnProjectile_pmo;
        private int _OnSpawnProjectile_Seq = 0;
        private int _OnSpawnProjectile_BlockNum = 0;
        private void Update_OnSpawnProjectile(int seq)
        {
            switch (_OnSpawnProjectile_BlockNum)
            {
                case 1:
                    Update_OnSpawnProjectile_Block1();
                    _OnSpawnProjectile_BlockNum++;
                    break;
                case 2:
                    Update_OnSpawnProjectile_Block2();
                    _OnSpawnProjectile_BlockNum++;
                    break;
                case 3:
                    Update_OnSpawnProjectile_Block3();
                    _OnSpawnProjectile_BlockNum++;
                    break;
                default:
                    TimerManager.instance.RemoveTimerSafely(ref _OnSpawnProjectile_Seq);
                    _OnSpawnProjectile_BlockNum = 0;
                    break;
            }
        }

        private void Update_OnSpawnProjectile_Block1()
        {
            if (_OnSpawnProjectile_info == null)
            {
                return;
            }

            GameObject go = null;
            if (_OnSpawnProjectile_info._projectileProperties._prefab == null)
            {
                go = new GameObject("ProjectileMono");
                go.AddComponent<ProjectileMono>();
            }
            else
            {
                go = GameObject.Instantiate(_OnSpawnProjectile_info._projectileProperties._prefab);
            }
            _OnSpawnProjectile_pmo = go.GetComponent<ProjectileMono>();
        }

        private void Update_OnSpawnProjectile_Block2()
        {
            if (_OnSpawnProjectile_pmo == null)
            {
                return;
            }

            if (_OnSpawnProjectile_info != null && _OnSpawnProjectile_info._projectileProperties._changeEffect != null)
            {
                _OnSpawnProjectile_pmo.effectPrefab = _OnSpawnProjectile_info._projectileProperties._changeEffect;
            }

            if (_OnSpawnProjectile_projectileProps != null)
            {
                _OnSpawnProjectile_pmo.Spawn(gameObject.transform, _OnSpawnProjectile_projectileProps._spawnOffset, _OnSpawnProjectile_projectileProps._spawnAttachment,
                                          _OnSpawnProjectile_projectileProps._spawnAttachmentPath, _OnSpawnProjectile_projectileProps._reattachment, _OnSpawnProjectile_projectileProps._reattachmentPath,
                                          _OnSpawnProjectile_projectileProps._initialVelocity, _OnSpawnProjectile_projectileProps._flyTime, _OnSpawnProjectile_projectileProps._fadeOutTime,
                                          _OnSpawnProjectile_projectileProps._isTarget, _OnSpawnProjectile_projectileProps._isLookAtTarget, _OnSpawnProjectile_projectileProps._isOnly);
            }
        }

        private void Update_OnSpawnProjectile_Block3()
        {
            try{
                Transform[] targets = null;
                if (this.LTTargets != null)
                {
                    targets = new Transform[this.LTTargets.Count];
                    var it = this.LTTargets.GetEnumerator();
                    int i = 0;
                    while (it.MoveNext())
                    {
                        var ct = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(it.Current);
                        
                        if (ct != null)
                        {
                            targets[i] = ct.transform;
                        }
                        
                        i++;
                    }
                }

                if (_OnSpawnProjectile_pmo != null)
                {
                    _OnSpawnProjectile_pmo.Init(targets);
                }
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        private void OnSpawnProjectile(MoveEditor.MoveAnimationEvent ee)
        {
            _OnSpawnProjectile_ee = ee;
            _OnSpawnProjectile_info = _OnSpawnProjectile_ee.EventRef as MoveEditor.ProjectileEventInfo;
            _OnSpawnProjectile_projectileProps = _OnSpawnProjectile_info._projectileProperties;
            TimerManager.instance.RemoveTimerSafely(ref _OnSpawnProjectile_Seq);
            _OnSpawnProjectile_BlockNum = 1;
            if (!preExit)
            {
                _OnSpawnProjectile_Seq = TimerManager.instance.AddTimer(1, 0, Update_OnSpawnProjectile);
            }
        }
        #endregion

        private void OnCameraMotion(MoveEditor.MoveAnimationEvent e)
        {
            if (Hotfix_LT.UI.LTCombatEventReceiver.Instance == null || !UserData.SkillCameraRotateEnabled)
                return;

            MoveEditor.CameraMotionEventInfo info = new MoveEditor.CameraMotionEventInfo(e.stringParameter);
            MoveEditor.CameraMotionEventProperties motionProps = info._cameraMotionProperties;

            if (motionProps._motionTrigger == MoveEditor.CameraMotionTrigger.LocalPlayerOnly)
            {
                if (Index.TeamIndex != CombatLogic.Instance.LocalPlayerTeamIndex)
                {
                    return;
                }
            }
            else if (motionProps._motionTrigger == MoveEditor.CameraMotionTrigger.EnemyOnly)
            {
                if (Data != null && CombatLogic.Instance.IsPlayerOrChallengerSide(Data.TeamId))
                {
                    return;
                }
            }

            if (GetMainCamera != null)
            {
                CombatCamera combat_camera = GetMainCamera.GetComponent<CombatCamera>();
                if (!combat_camera.m_enablePlayerOrChallengerMotion && CombatLogic.Instance.IsPlayerOrChallengerSide(Index.TeamIndex))
                {
                    return;
                }
                if (!combat_camera.m_enableOpponentMotion && CombatLogic.Instance.IsOpponentSide(Index.TeamIndex))
                {
                    return;
                }
                string camera_motion_json = motionProps._motionOptions;
                List<MoveEditor.CameraMotionOption> options = GM.JSON.ToObject<List<MoveEditor.CameraMotionOption>>(camera_motion_json);
                List<GameObject> targets = new List<GameObject>();
                if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.Attacker)
                {
                    targets.Add(gameObject.gameObject);
                }
                else if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.Defenders)
                {
                    targets.AddRange(GetTargetGameObjects());
                }
                else if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.All)
                {
                    targets.Add(gameObject.gameObject);
                    targets.AddRange(GetTargetGameObjects());
                }
                else if (motionProps._motionTarget == MoveEditor.CameraMotionTarget.DefendersCameraAnchor)
                {
                    EB.Debug.LogError(EB.Localizer.GetString("ID_codefont_in_Combatant_69072"));
                }

                if (combat_camera != null)
                {
                    if (motionProps._blendCurrentCamera)
                    {
                        if (motionProps._onlyLookAtTarget)
                        {
                            combat_camera.CurrentCameraLookAt(ref targets, motionProps._hangonDuration);
                        }
                        else
                        {
                            CameraLerp lerp = CameraLerp.Create();
                            lerp.lerpStyle = CameraLerp.LerpStyle.determineAtRunTime;
                            lerp.dialogueCameraLerpSmoothing = motionProps._blendLerpSmoothing;
                            lerp.animationCurve = motionProps._blendLerpCurve;
                            lerp.dialogueCameraLerpTime = motionProps._lerpDuration;
                            lerp.pitchLerpSmoothing = motionProps._blendPitchLerpSmoothing;
                            lerp.curvePitchLerp = motionProps._blendPitchLerpCurve;
                            lerp.yawLerpSmoothing = motionProps._blendYawLerpSmoothing;
                            lerp.curveYawLerp = motionProps._blendYawLerpCurve;
                            lerp.hangonTime = motionProps._hangonDuration;
                            combat_camera.BlendCurrentCamera(ref targets, motionProps._blendDistanceOffset, motionProps._blendPitchOffset,
                                                             motionProps._blendYawOffset, motionProps._blendHeightOffset, lerp);
                        }
                    }
                    else
                    {
                        MoveEditor.CameraMotionOption option = MoveEditor.MoveUtils.GetCamermotionLottery(ref options, IsCurrentAttackCritical());
                        if (option != null)
                        {
                            CameraMotion motion = GlobalCameraMotionData.Instance.GetCameraMotion(option._motionName);
                            if (motion != null)
                            {
                                combat_camera.State = CombatCamera.MotionState.Lerping;
                                MyFollowCamera.Instance.isActive = false;
                                GameCameraParams gameCameraParams = (GameCameraParams)motion.camera;
                                CameraLerp motion_lerp = motion.cameraLerpOverride;
                                motion_lerp.dialogueCameraLerpTime = motionProps._lerpDuration;
                                motion_lerp.hangonTime = motionProps._hangonDuration;
                                combat_camera.EnterInteractionCamera(ref targets, ref gameCameraParams, motion_lerp);

                                switch (motion.name)
                                {
                                    case "TY_SkillCloseShot":
                                    case "TY_ComboCloseShot":
                                        RenderSettingsManager.Instance.ReplaceGlobalCharacterOutline(0.5f);
                                        break;
                                    default:
                                        RenderSettingsManager.Instance.ReplaceGlobalCharacterOutline(1f);
                                        break;
                                }
                            }
                        }
                    }

                    combat_camera.SetResumeFollowCamera(); //战斗驱动的镜头结束后恢复至当前镜头
                }

            }
        }

        #region  分帧
        private MoveEditor.MoveAnimationEvent _OnHitFramerHandler_ee = null;
        private List<CombatantIndex> _OnHitFramerHandler_CIList = null;
        private int _OnHitFramerHandler_Seq = 0;
        private void Update_OnHitFramerHandler(int seq)
        {
            TimerManager.instance.RemoveTimerSafely(ref _OnHitFramerHandler_Seq);
            if (_OnHitFramerHandler_CIList != null && _OnHitFramerHandler_CIList.Count > 0)
            {
                foreach(var ci in _OnHitFramerHandler_CIList)
                {
                    Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(ci);
                    if (target != null)// && target.IsAlive())
                    {
                        HitMono hm = target.gameObject.GetComponent<HitMono>();
                        if (hm == null)
                        {
                            hm = target.gameObject.AddComponent<HitMono>();
                        }
                        hm.OnInflictHitEx(_OnHitFramerHandler_ee);
                        hm.OnInflictAudioEx(_OnHitFramerHandler_ee);
                    }
                }
            }
        }

        private void OnHitFramerHandler(object arg)
        {
            _OnHitFramerHandler_ee = arg as MoveEditor.MoveAnimationEvent;
            if (_OnHitFramerHandler_ee == null)
                return;

            if (m_hitMono != null)
            {
                MoveEditor.HitEventInfo event_info = _OnHitFramerHandler_ee.EventRef as MoveEditor.HitEventInfo;
                if (event_info._particleProperties._applyOnTargetList)
                {
                    if (this.LTTargets != null)
                    {
                        _OnHitFramerHandler_CIList = this.LTTargets.ToList();
                        TimerManager.instance.RemoveTimerSafely(ref _OnHitFramerHandler_Seq);
                        if (!preExit)
                        {
                            _OnHitFramerHandler_Seq = TimerManager.instance.AddTimer(1, 0, Update_OnHitFramerHandler);
                        }
                    }
                }
                else
                {
                    m_hitMono.OnInflictHitEx(_OnHitFramerHandler_ee);
                    m_hitMono.OnInflictAudioEx(_OnHitFramerHandler_ee);
                }
            }
        }
        #endregion

        private void OnHitEventInfoTimerUpHandler(object arg)
        {
            MoveEditor.MoveAnimationEvent ee = arg as MoveEditor.MoveAnimationEvent;
            if (ee == null)
                return;

            var hitEventInfo = ee.EventRef as MoveEditor.HitEventInfo;
            if (hitEventInfo != null)
            {
                if (hitEventInfo._hitRxnProps.isOnlyPlayEffect)
                {
                    // 只播特效，不触发受击事件！
                    return;
                }
            }

            SkillEventState skill_event_state = (EventState as SkillEventState);
            if (skill_event_state == null)
                return;
            skill_event_state.SkillEvent.HitTimes--;
            if (skill_event_state.SkillEvent.HitTimes < 0)
            {
                EB.Debug.LogError("skill_event_state.SkillEvent.HitTimes < 0");
                return;
            }

            OnHitDamageDatasFramerHandler(ee);
        }

        #region 分帧
        private int _OnHitDamageDatasFramerHandler_seq = 0;
        
        private void OnHitDamageDatasFramerHandler(object arg)
        {
            //战斗改为不在事件触发上做分帧，改为在ui飘字上做分帧。
            if (preExit) return;

            var enumerator = CombatSyncData.Instance.DamageDatas.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if(enumerator.Current.Value.Count > 0)
                {
                    var dmgTargetData = enumerator.Current.Value.Dequeue();
                    if (dmgTargetData.IsDirect && dmgTargetData.Order != -1)
                    {
                        dmgTargetData.OnHit();
                        if (enumerator.Current.Value.Count > 0)
                        {
                            dmgTargetData = enumerator.Current.Value.Peek();
                            if (!dmgTargetData.IsDirect)
                            {
                                dmgTargetData= enumerator.Current.Value.Dequeue();
                                dmgTargetData.OnHit();
                            }
                        }
                    }
                    else
                    {
                        dmgTargetData.OnHit();
                    }
                }
            }
        }
        #endregion

        private void OnInflictHit(MoveEditor.MoveAnimationEvent ee)
        {
            OnHitFramerHandler(ee);
            OnHitEventInfoTimerUpHandler(ee);
        }

        public void ApplyDamageData(CombatDamageSyncData dmgTargetData)
        {
            if (dmgTargetData.Damage != 0)
            {

                if (dmgTargetData.Damage > 0 && dmgTargetData.IsDirect)
                {
                    Hotfix_LT.UI.LTCombatEventReceiver.Instance.StartEffect(dmgTargetData);
                }
                else
                {
                    long hp = GetHP() - dmgTargetData.Damage;
                    SetHP(hp); ;
                }

                //Hotfix_LT.Messenger.Raise<CombatHitDamageEvent>(Hotfix_LT.EventName.CombatHitDamageEvent, new CombatHitDamageEvent(gameObject.gameObject, dmgTargetData.Damage, dmgTargetData.Damage, 0, dmgTargetData.IsCrit));
                GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "RaiseCombatHitDamageEvent", new CombatHitDamageEvent(gameObject.gameObject, dmgTargetData.Damage, dmgTargetData.Damage, 0, dmgTargetData.IsCrit));

                ////判断当前是否为敌方队伍
                var data = CombatSyncData.Instance.GetCharacterData(dmgTargetData.Target);
                if (CombatLogic.Instance.IsOpponentSide(data.TeamId))
                {
                    if (!preExit)
                    {
                        GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTCombatHudController", "Instance", "BossHealthBarCtrl_UpdateHurt", dmgTargetData.Damage);
                    }
                }

            }

            //if (dmgTargetData.Absorb > 0 && Hotfix_LT.UI.CombatDamagesHUDController.Instance != null)
            //{
            //    var ffe = new CombatFloatFontEvent(this, EB.Localizer.GetString("ID_codefont_in_Combatant_77602"), Hotfix_LT.UI.CombatFloatFontUIHUD.eFloatFontType.OnlyFont, this.floatBuffFontTextOffset);
            //    Hotfix_LT.UI.CombatDamagesHUDController.Instance.OnCombatFloatFontListener(ffe);
            //}
            if (dmgTargetData.Absorb > 0)
            {
                var ffe = new CombatFloatFontEvent(this, EB.Localizer.GetString("ID_codefont_in_Combatant_77602"), 2, this.floatBuffFontTextOffset);
                if (!preExit)
                {
                    GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.CombatDamagesHUDController", "Instance", "OnCombatFloatFontListener", ffe);
                }
            }
        }

        private void PlayTargetHitReaction(MoveEditor.MoveAnimationEvent ee)
        {
            if (EventState != null)
            {
                EventState.OnPlayTargetHitReactionEvent(ee);
            }
        }

        private void OnTriggerFrame(MoveEditor.MoveAnimationEvent ee)
        {
            if (EventState != null)
            {
                EventState.OnTriggerCombo(ee);
            }
        }

        private void OnPauseEvent(MoveEditor.MoveAnimationEvent _event)
        {
            MoveEditor.CombatEventInfo pauseEvent = _event.EventRef as MoveEditor.CombatEventInfo;
            if (pauseEvent == null)
                return;
            if (pauseEvent.pauseProps.type == MoveEditor.PauseProperties.PAUSE_TYPE.PAUSE_TYPE_SELF)
            {
                PauseAnimation(pauseEvent.pauseProps.fPauseTime);
            }
        }
        #endregion

        public void PauseAnimation(float wait_senconds)
        {
            if (m_animator == null)
            {
                EB.Debug.LogError("LTComabatEventReceiver PauseAnimation m_animator is Null!!!");
                return;
            }

            if (wait_senconds < 0.0f)
            {// try restore animator speed
                if (m_animator.speed == m_setAnimatorSpeed && m_storedAnimatorSpeed >= 0.0f)
                {// not changed
                    m_animator.speed = m_storedAnimatorSpeed;

                    m_storedAnimatorSpeed = -1.0f;
                    m_setAnimatorSpeed = -1.0f;
                }

                return;
            }

            if (wait_senconds == 0.0f)
            {
                if (m_animator.speed == m_setAnimatorSpeed && m_storedAnimatorSpeed >= 0.0f)
                {
                    // was set
                    if (m_setAnimatorSpeed == 0.0f)
                    {
                        return;
                    }

                    m_setAnimatorSpeed = 0.0f;
                }
                else
                {
                    m_setAnimatorSpeed = 0.0f;
                    m_storedAnimatorSpeed = m_animator.speed;
                }

                m_animator.speed = m_setAnimatorSpeed;

                return;
            }

            if (m_animator.speed == m_setAnimatorSpeed && m_storedAnimatorSpeed >= 0.0f)
            {
                if (m_setAnimatorSpeed == 0)
                {
                    return;
                }

                m_setAnimatorSpeed = 0.0f;
            }
            else
            {
                m_setAnimatorSpeed = 0.0f;
                m_storedAnimatorSpeed = m_animator.speed;
            }
            m_animator.speed = m_setAnimatorSpeed;
            TimerManager.instance.RemoveTimerSafely(ref _WaitAndRcoverAnimation_Seq);
            if (!preExit)
            {
                _WaitAndRcoverAnimation_Seq = TimerManager.instance.AddTimer((int)(wait_senconds * 1000), 1, WaitAndRcoverAnimation);
            }
        }
        #region Coroutine -> timer
        private int _WaitAndRcoverAnimation_Seq = 0;
        private void WaitAndRcoverAnimation(int seq)
        {
            if (m_animator.speed == m_setAnimatorSpeed && m_storedAnimatorSpeed >= 0.0f)
            {// not changed
                m_animator.speed = m_storedAnimatorSpeed;

                m_storedAnimatorSpeed = -1.0f;
                m_setAnimatorSpeed = -1.0f;
            }
            _WaitAndRcoverAnimation_Seq = 0;
        }
        #endregion

        Dictionary<SkinnedMeshRenderer, Shader[]> m_recordMaterialContainer;
        public void ReplaceAsStoneMaterialShader()
        {
            Shader shader = Shader.Find("Unlit/Character Stone");
            if (m_recordMaterialContainer == null)
                m_recordMaterialContainer = new Dictionary<SkinnedMeshRenderer, Shader[]>();

            for (int i = 0; i < skinnedMeshRenderer.Length; i++)
            {
                var sr = skinnedMeshRenderer[i];
                if (!m_recordMaterialContainer.ContainsKey(sr))
                {
                    Shader[] oldMaterialShaderArr = new Shader[sr.materials.Length];
                    for (int matIndex = 0; matIndex < sr.materials.Length; ++matIndex)
                    {
                        oldMaterialShaderArr[matIndex] = sr.materials[matIndex].shader;
                    }
                    m_recordMaterialContainer.Add(sr, oldMaterialShaderArr);
                }
                for (int matIndex = 0; matIndex < sr.materials.Length; ++matIndex)
                {
                    if (shader != null)
                    {
                        sr.materials[matIndex].shader = shader;
                    }
                }
            }
        }

        public void RestoreMaterialShader()
        {
            if (m_recordMaterialContainer == null || m_recordMaterialContainer.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < skinnedMeshRenderer.Length; i++)
            {
                var sr = skinnedMeshRenderer[i];
                if (!m_recordMaterialContainer.ContainsKey(sr))
                {
                    continue;
                }
                for (int matIndex = 0; matIndex < sr.materials.Length; ++matIndex)
                {
                    sr.materials[matIndex].shader = m_recordMaterialContainer[sr][matIndex];
                }
            }
        }
    }
}