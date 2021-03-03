using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ReactionActionState : CombatActionState
    {
        protected float m_offset_time = 0.0f;

        public string MoveName
        {
            get;
            set;
        }

        public string AnimatorStateName
        {
            get { return "Base Layer.Moves." + MoveName; }
        }

        public int AnimatorStateNameHash
        {
            get { return Animator.StringToHash(AnimatorStateName); }
        }

        public Vector3 Offset
        {
            get;
            set;
        }

        public float OffsetTime
        {
            get;
            set;
        }

        public ReactionEffectEvent ReactionEvent
        {
            get { return Combatant.EventState.Event as ReactionEffectEvent; }
        }

        public ReactionActionState(Combatant combatant)
            : base(combatant)
        {

        }

        public ReactionActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);
        }

        public override void CleanUp()
        {
            m_offset_time = 0.0f;
            MoveName = string.Empty;
            Offset = Vector3.zero;
            OffsetTime = 0.0f;

            base.CleanUp();
        }

        public override void Start()
        {
            TryRotateTowardToSender();
            TryStartOffset();
        }

        public override void Update()
        {
            base.Update();

            UpdateOffset();
        }

        public override float CalculateLeftTime()
        {
            float normalized_time = NormalizedTime;
            float speed = Combatant.Animator.speed;

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int current_state_hash = Combatant.MoveController.GetCurrentAnimHash();
            int this_state_hash = AnimatorStateNameHash;
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (this_state_hash != current_state_hash)
            {// just new
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {// leaving
                normalized_time = 1.0f;
            }
            else if (current_state_hash != state_info.fullPathHash)
            {// blending
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (NormalizedTime > 0.0f && state_info.normalizedTime >= NormalizedTime)
            {// not repeat
                normalized_time = state_info.normalizedTime;
            }

            float left_time = move._animationClip.length * (1.0f - normalized_time);
            return left_time / speed;
        }

        public virtual float CalculateComboTime()
        {
            return CalculateLeftTime();
        }

        public void SetupMove(string move_name, string animator_state_name, float start_time)
        {
            SetupMove(move_name, animator_state_name, start_time, 0.0f);
        }

        public void SetupMove(string move_name, string animator_state_name, float start_time, float blend_frame)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMoveIfExists(move_name);
            if (move == null)
            {
                EB.Debug.LogError("move not found");
                Stop();
                return;
            }

            if (move._moveState != MoveController.CombatantMoveState.kHitReaction)
            {
                EB.Debug.LogError("move state not match");
                Stop();
                return;
            }

            int anim_hash = Animator.StringToHash(animator_state_name);

            // set state and default move and auto cross to default animation
            Combatant.MoveController.TransitionTo(move._moveState);
            // set real move
            Combatant.MoveController.SetMove(move);
            // force cross fade to real animator state
            Combatant.MoveController.m_reaction_hash = anim_hash;
            Combatant.MoveController.CrossFade(anim_hash, blend_frame / move.NumFrames, 0, start_time);
        }

        public void SetupMove(MoveController.CombatantMoveState move_state, int anim_hash, float start_time)
        {
            Combatant.MoveController.TransitionTo(move_state);
            Combatant.MoveController.CrossFade(anim_hash, 0.0f, 0, start_time);
        }

        public void SetupMoveUseDefault(string move_name, string animator_state_name, float start_time)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMoveIfExists(move_name);
            if (move == null)
            {
                move = Combatant.MoveController.GetMoveByState(MoveController.CombatantMoveState.kHitReaction);
                if (move == null)
                {
                    EB.Debug.LogError("move not found");
                    Stop();
                    return;
                }

                move_name = move.name;
                animator_state_name = "Base Layer." + move_name;
                start_time = 0.0f;
            }

            SetupMove(move_name, animator_state_name, start_time);
        }

        public void TryRotateTowardToSender()
        {
            Combatant sender = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(ReactionEvent.Sender);
            if (sender != null && !IsTowardToMe())
            {
                Vector3 direction = sender.OriginPosition - Combatant.OriginPosition;
                Quaternion world_target_rotation = Quaternion.LookRotation(direction, Vector3.up);
                Quaternion local_target_rotation = world_target_rotation * Combatant.transform.parent.transform.rotation;
                float rotate_time = 0.5f * Combatant.CurrentMove._animationClip.length;
                StartRotate(Combatant.transform.localRotation, local_target_rotation, rotate_time);
            }
        }

        protected bool IsTowardToMe()
        {
            Combatant sender = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(ReactionEvent.Sender);
            if (!sender.IsAttacking())
            {
                return false;
            }

            SkillEventState skill_state = sender.EventState as SkillEventState;
            return skill_state.GetAttackTarget<CombatantIndex>().Equals(Combatant.Index);
        }

        public void TryRotateTowardToIdle()
        {
            float remaining_time = 1.0f - NormalizedTime;
            if (remaining_time > 0.0f && remaining_time < 0.5f && !IsRotating())
            {
                float rotate_time = remaining_time * Combatant.CurrentMove._animationClip.length;
                Quaternion local_start_rotation = StartRotation * Combatant.transform.parent.transform.rotation;
                StartRotate(Combatant.transform.localRotation, local_start_rotation, rotate_time);
            }
        }

        public void TryStartOffset()
        {
            if (Offset.Equals(Vector3.zero))
            {
                return;
            }

            OffsetTime = Mathf.Min(OffsetTime, Combatant.CurrentMove._animationClip.length / 2);
            m_offset_time = 0.0f;
        }

        public void UpdateOffset()
        {
            if (OffsetTime <= 0.0f)
            {
                return;
            }

            if (m_offset_time >= OffsetTime)
            {// offset end
                OffsetTime = 0.0f;
                m_offset_time = 0.0f;
                return;
            }

            m_offset_time = Mathf.Min(m_offset_time + Time.deltaTime * Combatant.Animator.speed, OffsetTime);
            float lerp_time = m_offset_time / OffsetTime;
            lerp_time = Mathf.Clamp01(lerp_time);
            Vector3 lerp_offset = Vector3.Lerp(Vector3.zero, Offset, lerp_time);

            Combatant.transform.position = Combatant.OriginPosition + lerp_offset;
        }
    }
}