using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Reflection;

/// <summary>
/// 动作控件
/// </summary>
public class MoveController : MonoBehaviour
{
    public enum CombatantMoveState
    {
        // DO NOT REORDER
        kIdle = 0,
        kLocomotion = 1,
        kAttackTarget = 2,
        kHitReaction = 3,
        kDeath = 4,
        kVictoryDance = 5,// no use
        kRevive = 6,// no use
        kEntry = 7,
        kLobby = 8,
        kReady = 9,
        kLaunch = 10,
        kForward = 11,
        kBackward = 12,
        kSpecial=13,
        // DO NOT REORDER
        kNone = 14,

        //kImmobilize  //special action State
    };

    // names of states - attack varies because it is dependent on which skill is being used
    public static int m_idle_hash = Animator.StringToHash("Base Layer.Idle");//1432961145
    public static int m_locomotion_hash = Animator.StringToHash("Base Layer.Locomation");//191190880
    public static int m_entry_hash = Animator.StringToHash("Base Layer.Entry");//1873608182
    public static int m_revive_hash = Animator.StringToHash("Base Layer.Revive");//-276161943
    public static int m_dance_hash = Animator.StringToHash("Base Layer.Victory");//1355815490
    public static int m_death_hash = Animator.StringToHash("Base Layer.Death");//-1546996312
    public static int m_ready_hash = Animator.StringToHash("Base Layer.Ready");//1813570857
    public static int m_launch_hash = Animator.StringToHash("Base Layer.Launch"); //2099838080(no use)
    public static int m_forward_hash = Animator.StringToHash("Base Layer.Forward");//-939903586
    public static int m_backward_hash = Animator.StringToHash("Base Layer.Backward");//1103654941

    // NON-static cuz they vary per dude
    public int m_attack_hash = Animator.StringToHash("Base Layer.Moves.SKILL_Default");//855318043
    public int m_reaction_hash = Animator.StringToHash("Base Layer.Moves.HitReaction");//1592224553
    public int m_lobby_hash = Animator.StringToHash("Base Layer.Lobby.LobbyIdle");//2055657787

    public MoveEditor.Move[] m_Moves;

    private bool m_IsInitialized = false;
    public bool IsInitialized
    {
        get { return m_IsInitialized; }
    }

    private List<System.Action> InitializedSucc = new List<System.Action>();
    public void RegisterInitSuccCallBack(System.Action fn){
        InitializedSucc.Add(fn);
    }
    private Animator m_Animator;
    private AvatarComponent m_AvatarComponent;
    private MoveEventsDispatcher m_MoveEventDispatcher;
    private MoveMotion m_MoveMotion;

    public MoveEventsDispatcher MoveEventsDispatcher
    {
        get
        {
            if (m_MoveEventDispatcher == null)
            {
                m_MoveEventDispatcher = new MoveEventsDispatcher(this);
                m_MoveEventDispatcher.Awake();
            }
            return m_MoveEventDispatcher;
        }
    }

    public MoveMotion MoveMotion
    {
        get
        {
            if (m_MoveMotion == null)
            {
                m_MoveMotion = new MoveMotion(this);
                m_MoveMotion.Awake();
            }
            return m_MoveMotion;
        }
    }

    private CombatantMoveState m_CurrentState = CombatantMoveState.kNone;
    public CombatantMoveState CurrentState
    {
        get {
            return m_CurrentState;
        }
        set {
            m_CurrentState = value;
        }
    }

	//public bool IsInitWithLocomotion { get; set; }

    private MoveEditor.Move m_CurrentMove = null;
    public MoveEditor.Move CurrentMove
    {
        get { return m_CurrentMove; }
    }

    private void Awake()
    {
        m_MoveEventDispatcher = new MoveEventsDispatcher(this);
        m_MoveEventDispatcher.Awake();

        m_MoveMotion = new MoveMotion(this);
        m_MoveMotion.Awake();

        InitAnimator();
        m_IsInitialized = true;
        TransitionTo(CombatantMoveState.kIdle);
        for(int i = 0; i < InitializedSucc.Count; i++){
            InitializedSucc[i]?.Invoke();
        }
        InitializedSucc.Clear();
    }


    private void Update()
    {
        m_MoveEventDispatcher.Update();
    }

    void OnEnable()
    {
        if (m_IsInitialized)
        {
            MoveEditor.Move currentMove = m_CurrentMove;
            if (m_CurrentState != CombatantMoveState.kIdle)
            {
                TransitionTo(m_CurrentState);
                SetMove(currentMove);
            }

            if (GetCurrentAnimHash() != m_idle_hash)
            {
                CrossFade(GetCurrentAnimHash(), 0, 0, 0);
            }
        }
    }

    private void OnDisable()
    {
        m_CurrentState = CombatantMoveState.kIdle;
    }

    private void OnAnimatorMove()
    {
        m_MoveMotion.OnAnimatorMove();
    }

    public void InitAnimator()
    {
        m_Animator = GetComponent<Animator>();
        m_AvatarComponent = GetComponent<AvatarComponent>();
        //主要战斗当中~有些模型会是从屏幕外进入屏幕内的情况
        m_Animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        //
        for (int i = 0, count = m_Moves.Length; i < count; ++i)
        {
            if (m_Moves[i] != null)
            {
                m_Moves[i]._animationEvents = m_Moves[i].CompileEvents();
            }
        }
    }

    public void Initialize()
    {
        if (m_IsInitialized)
        {
            return;
        }

        InitAnimator();

        m_IsInitialized = true;
        
        TransitionTo(CombatantMoveState.kIdle);
    }

    public MoveEditor.Move GetMoveIfExists(string move, bool isNew = false)
    {
        for (int i = 0; i < m_Moves.Length; i++)
        {
            if (move.Equals(m_Moves[i].name, System.StringComparison.OrdinalIgnoreCase))
            {
                return m_Moves[i];
            }
        }
        return null;
    }

    public MoveEditor.Move GetMoveByState(CombatantMoveState state)
    {
        for (int i = 0; i < m_Moves.Length; i++)
        {
            if (state == m_Moves[i]._moveState)
            {
                return m_Moves[i];
            }
        }
        return null;
    }

    public MoveEditor.Move[] GetMovesByState(CombatantMoveState state)
    {
        return m_Moves.Where(s => s._moveState == state).ToArray();
    }

    public MoveEditor.Move GetMove(string move, bool useDefaultIfNecessary)
    {
        MoveEditor.Move theMove = GetMoveIfExists(move);
        if (theMove == null && m_Moves.Length > 0 && useDefaultIfNecessary)
        {
            //Debug.LogError (gameObject.name + " trying to play move " + move + " but doesn't have it defined!");
            theMove = m_Moves[0];
        }
        return theMove;
    }
    
    public void TransitionTo(CombatantMoveState state)
    {
        if (!m_IsInitialized)
        {
            if (state != CombatantMoveState.kIdle)
            {
                EB.Debug.LogError("TransitionTo: {0} not initialized", state);
            }
            return;
        }

        if ((int)m_CurrentState == (int)state)
        {
            return;
        }
        // don't delete me - i'm awesome for debugging
        //EB.Debug.Log (myName + " " + m_state + " -> " + state);
        MoveEditor.Move nextMove = GetMoveByState(state);
        if (nextMove == null)
        {
            EB.Debug.LogError("TransitionTo: move not found for state {0}", state);
            return;
        }

        SetMove(nextMove);
        m_Animator.SetInteger("State", (int)state);

        CurrentState = state;

        if (m_AvatarComponent != null)
        {
            m_AvatarComponent.FullPathHash = GetCurrentAnimHash();
        }
    }

    public int GetCombatEventCount(MoveEditor.CombatEventInfo.CombatEventType theType, MoveEditor.Move theMove)
    {
        if (theMove == null)
        {
            EB.Debug.LogError("Try to use unexisted move");
            return 0;
        }

        int count = 0;
        for (int ii = 0; ii < theMove._combatEvents.Count; ++ii)
        {
            if (theMove._combatEvents[ii].eventType == theType)
            {
                count++;
            }
        }
        return count;
    }

    public float GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType theType, MoveEditor.Move theMove)
    {
        if (theMove == null)
        {
            EB.Debug.LogError("Try to use unexisted move");
        }

        for (int ii = 0; ii < theMove._combatEvents.Count; ++ii)
        {
            if (theMove._combatEvents[ii].eventType == theType)
            {
                return theMove._combatEvents[ii]._frame;
            }
        }
        return -1.0f;
    }

    public void SetMoveAndUseDefault(string move)
    {
        MoveEditor.Move theMove = GetMove(move, true);
        SetMove(theMove);
    }

    public void SetMove(string move)
    {
        MoveEditor.Move theMove = GetMoveIfExists(move);
        if (theMove != null)
        {
            SetMove(theMove);
        }
    }

    public void SetMove(MoveEditor.Move theMove)
    {
        m_CurrentMove = theMove;
        if (m_MoveMotion != null)
        {
            m_MoveMotion.ToNormalState();
        }
        if (m_MoveEventDispatcher != null)
        {
            m_MoveEventDispatcher.SetMove(theMove, 0.0f);
        }
        if (m_Animator)
        {
            m_Animator.speed = m_CurrentMove.Speed;
        }
    }

    public void CrossFade(int crossFadeHash, float normalizedBlendTime, int layer, float normalizedTime)
    {
        m_Animator.CrossFade(crossFadeHash, normalizedBlendTime, layer, normalizedTime);
        m_MoveEventDispatcher.OnCrossFade(m_CurrentMove, normalizedTime);

        if (m_AvatarComponent != null)
        {
            m_AvatarComponent.FullPathHash = crossFadeHash;
        }

        CurrentState = GetCombatantState(crossFadeHash);
#if UNITY_EDITOR
        if (!m_Animator.HasState(0, crossFadeHash))
        {
            EB.Debug.LogError("MoveController CrossFade failed: crossFadeHash = {0} not exit", crossFadeHash);
        }
#endif
    }

    public int GetCurrentAnimHash()
    {
        return GetStateAnimHash(CurrentState);
    }

    public CombatantMoveState GetCombatantState(int hash)
    {
        if (hash == m_idle_hash)
            return CombatantMoveState.kIdle;
        if (hash == m_ready_hash)
            return CombatantMoveState.kReady;
        if (hash == m_launch_hash)
            return CombatantMoveState.kLaunch;
        if (hash == m_locomotion_hash)
            return CombatantMoveState.kLocomotion;
        if (hash == m_death_hash)
            return CombatantMoveState.kDeath;
        if (hash == m_forward_hash)
            return CombatantMoveState.kForward;
        if (hash == m_backward_hash)
            return CombatantMoveState.kBackward;
        if (hash == m_attack_hash)
            return CombatantMoveState.kAttackTarget;
        if (hash == m_reaction_hash)
            return CombatantMoveState.kHitReaction;
        if (hash == m_revive_hash)
            return CombatantMoveState.kRevive;
        if (hash == m_entry_hash)
            return CombatantMoveState.kEntry;
        if (hash == m_dance_hash)
            return CombatantMoveState.kVictoryDance;
        if (hash == m_lobby_hash)
            return CombatantMoveState.kLobby;
        if (hash == m_entry_hash)
            return CombatantMoveState.kSpecial;
        return CombatantMoveState.kNone;
    }

    public int GetStateAnimHash(CombatantMoveState state)
    {
        switch (state)
        {
            case CombatantMoveState.kIdle:
                return m_idle_hash;
            case CombatantMoveState.kReady:
                return m_ready_hash;
            case CombatantMoveState.kLaunch:
                return m_launch_hash;
            case CombatantMoveState.kLocomotion:
                return m_locomotion_hash;
            case CombatantMoveState.kDeath:
                return m_death_hash;
            case CombatantMoveState.kForward:
                return m_forward_hash;
            case CombatantMoveState.kBackward:
                return m_backward_hash;
            case CombatantMoveState.kAttackTarget:
                return m_attack_hash;
            case CombatantMoveState.kHitReaction:
                return m_reaction_hash;
            case CombatantMoveState.kRevive:
                return m_revive_hash;
            case CombatantMoveState.kEntry:
                return m_entry_hash;
            case CombatantMoveState.kVictoryDance:
                return m_dance_hash;
            case CombatantMoveState.kLobby:
                return m_lobby_hash;
            case CombatantMoveState.kSpecial:
                return m_entry_hash;
            default:
                break;
        }
        return -1;
    }

    public string GetStateString(CombatantMoveState state, int stateHash)
    {
        StringBuilder result = new StringBuilder(state.ToString());
        result.Append("[");
        if (stateHash == m_attack_hash)
            result.Append("Attack]");
        else if (stateHash == m_reaction_hash)
            result.Append("Hit]");
        else if (stateHash == m_entry_hash)
            result.Append("Block]");
        else if (stateHash == m_death_hash)
            result.Append("Death]");
        else if (stateHash == m_idle_hash)
            result.Append("Idle]");
        else
            result.Append("???]");
        return result.ToString();
    }

    public string GetStateString()
    {
        AnimatorStateInfo state_info = m_Animator.GetCurrentAnimatorStateInfo(0);
        return GetStateString(CurrentState, state_info.fullPathHash);
    }

    //Animation Event Handlers
    public void OnSetAnimationSpeed(MoveEditor.MoveAnimationEvent ee)
    {
        MoveEditor.AnimationSpeedEventInfo info = ee.EventRef as MoveEditor.AnimationSpeedEventInfo;
        float variance = UnityEngine.Random.Range(-info.asep._plusMinus, info.asep._plusMinus);
        variance = (variance < 0 ? -1 : 1) * variance;
        m_Animator.speed = info._floatParameter + variance;
    }

    public bool InTransition(int layer)
    {
        if (layer < m_Animator.layerCount)
            return m_Animator.IsInTransition(layer);
        return false;
    }

    public AnimatorStateInfo GetCurrentStateInfo()
    {
        AnimatorStateInfo stateInfo;
		if(m_Animator==null)
			m_Animator = GetComponent<Animator>();
		if (m_Animator.IsInTransition(0))
        {
            stateInfo = m_Animator.GetNextAnimatorStateInfo(0);
        }
        else
        {
            stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        }
        return stateInfo;
    }

    public float NormalizedTimeToFrame_Wrapped(float normalizedTime)
    {
        if (normalizedTime > 1.0f)
        {
            normalizedTime = normalizedTime - Mathf.Floor(normalizedTime);
        }

        if (CurrentMove == null)
        {
            EB.Debug.LogError("{0}对象丢失Move组件，请立即修复...", this.name);
            return normalizedTime;
        }
        
        if (CurrentMove._animationClip == null)
        {
            EB.Debug.LogError("{0}对象的{1}丢失AnimationClip资源，请立即修复...", this.name, CurrentMove.name);
            return normalizedTime;
        }

        return normalizedTime * CurrentMove._animationClip.length * CurrentMove._animationClip.frameRate;
    }
}
