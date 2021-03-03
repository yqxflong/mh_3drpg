using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class DownActionState : ReactionActionState
    {
        public static readonly string downMoveName = "Down";

        public float StartFrame
        {
            get;
            set;
        }

        public float EndFrame
        {
            get;
            set;
        }

        public float EndNormalizedTime
        {
            get;
            set;
        }

        public float LieFrame
        {
            get;
            set;
        }

        public float StandUpFrame
        {
            get;
            set;
        }

        public Vector3 LiePoint
        {
            get;
            set;
        }

        public DownActionState(Combatant combatant)
            : base(combatant)
        {
            MoveName = downMoveName;

            InitDownPoint();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = 0.0f;
            NormalizedTime = 0.0f;

            EndFrame = move.NumFrames;
            EndNormalizedTime = 1.0f - timeEpsilon;
        }

        public DownActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = downMoveName;

            InitDownPoint();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = 0.0f;
            NormalizedTime = 0.0f;

            EndFrame = move.NumFrames;
            EndNormalizedTime = 1.0f - timeEpsilon;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            StartFrame = 0.0f;
            EndFrame = 0.0f;
            EndNormalizedTime = 0.0f;
            LieFrame = 0.0f;
            StandUpFrame = 0.0f;
            LiePoint = Vector3.zero;
        }

        protected void InitDownPoint()
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            LieFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.DownLieStart, move);
            StandUpFrame = Combatant.MoveController.GetFrameByCombatType(MoveEditor.CombatEventInfo.CombatEventType.DownStandUpStart, move);

            float lie_y_offset = move._yOffsetCurve.Evaluate(LieFrame);
            float lie_z_offset = move._zOffsetCurve.Evaluate(LieFrame);

            float lie_y = lie_y_offset + Combatant.OriginPosition.y;
            float lie_z = lie_z_offset + Combatant.OriginPosition.z;

            LiePoint = new Vector3(Combatant.OriginPosition.x, lie_y, lie_z);
        }

        public override void Update()
        {
            base.Update();

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {
                EB.Debug.LogError("invalid move state");
                Stop();
                return;
            }

            if (state_hash != state_info.fullPathHash)
            {
                return;
            }

            UpdateLocation();
            if (state_info.normalizedTime > EndNormalizedTime && NormalizedTime > EndNormalizedTime)
            {
                OnEnd();
                return;
            }

            NormalizedTime = state_info.normalizedTime;
        }

        protected virtual void OnEnd()
        {
            End();
        }

        public virtual void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            if (current_frame > EndFrame)
            {
                current_frame = EndFrame;
            }
            float y_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(current_frame);
            float z_offset = Combatant.CurrentMove._zOffsetCurve.Evaluate(current_frame);
            if (Combatant.Index.TeamIndex == CombatLogic.Instance.DefenderTeamIndex)
            {
                z_offset = -z_offset;
            }

            Vector3 current_position = Combatant.transform.position;
            current_position.y = Combatant.OriginPosition.y + y_offset;
            current_position.z = Combatant.OriginPosition.z + z_offset;
            Combatant.transform.position = current_position;
        }

        public override float CalculateComboTime()
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

            float current_frame = normalized_time * move.NumFrames;
            if (current_frame >= StandUpFrame)
            {
                return 0.0f;
            }

            float combo_time = (StandUpFrame - current_frame) / move.NumFrames * move._animationClip.length;
            return combo_time / speed;
        }
    }
}