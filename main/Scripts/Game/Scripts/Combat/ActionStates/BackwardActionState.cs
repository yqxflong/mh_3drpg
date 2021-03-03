using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class BackwardActionState : CombatActionState
    {
        public Vector3 DestnationLocation
        {
            get;
            set;
        }

        public Vector3 SourceLocation
        {
            get;
            private set;
        }

        public BackwardActionState(Combatant combatant)
            : base(combatant)
        {
            SourceLocation = StartPosition;
        }

        public BackwardActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            SourceLocation = StartPosition;
        }

        public override void CleanUp()
        {
            SourceLocation = Vector3.zero;
            DestnationLocation = Vector3.zero;

            base.CleanUp();
        }

        public override void Start()
        {
            // calculate rotation, rotate immediately
            Combatant.transform.rotation = CalculateBackwardRotation(DestnationLocation);

            if (ArrivedAtDestination())
            {
                End();
                return;
            }

            if (Combatant.MoveController.GetMoveByState(MoveController.CombatantMoveState.kBackward) == null)
            {
                EB.Debug.LogWarning("Backward move not found");
                End();
                return;
            }

            //AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            //Combatant.Animator.speed = (DestnationLocation - SourceLocation).magnitude / state_info.normalizedTime;

            // change state and cross fade to forward animator state
            Combatant.MoveController.TransitionTo(MoveController.CombatantMoveState.kBackward);
            Combatant.MoveController.CrossFade(MoveController.m_backward_hash, 0f, defaultLayer, NormalizedTime);
        }

        public override void Update()
        {
            base.Update();

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (move_state != MoveController.CombatantMoveState.kBackward)
            {
                EB.Debug.LogError("move state not match");
                Stop();
                return;
            }

            if (state_hash != state_info.fullPathHash)
            {
                return;
            }

            UpdateLocation();

            float end_time = 1.0f - timeEpsilon;
            if (state_info.normalizedTime > end_time && NormalizedTime > end_time)
            {
                End();
                return;
            }

            NormalizedTime = state_info.normalizedTime;
        }

        public override void End()
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(true));
        }

        public override void Stop()
        {
            base.Stop();

            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(true));
        }

        public override float CalculateLeftTime()
        {
            float normalized_time = NormalizedTime;
            float speed = Combatant.Animator.speed;

            MoveEditor.Move move = Combatant.MoveController.GetMoveByState(MoveController.CombatantMoveState.kBackward);

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (MoveController.m_backward_hash != state_hash)
            {
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (move_state != MoveController.CombatantMoveState.kBackward)
            {
                normalized_time = 1.0f;
            }
            else if (state_hash != state_info.fullPathHash)
            {
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (NormalizedTime > 0.0f && state_info.normalizedTime >= NormalizedTime)
            {
                normalized_time = state_info.normalizedTime;
            }

            float left_time = move._animationClip.length * (1.0f - normalized_time);
            return left_time / speed;
        }

        protected Quaternion CalculateBackwardRotation(Vector3 target_position)
        {
            Vector3 opposite_move_direction = Combatant.transform.position - target_position;
            Quaternion rotation = Quaternion.LookRotation(opposite_move_direction, Vector3.up);
            return rotation;
        }

        protected void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            float z_offset = -Combatant.CurrentMove._zOffsetCurve.Evaluate(current_frame);
            float z_max_offset = -Combatant.CurrentMove._zOffsetCurve.Evaluate(Combatant.CurrentMove.NumFrames);

            Vector3 offset = z_offset / z_max_offset * (DestnationLocation - SourceLocation);
            Combatant.transform.position = SourceLocation + offset;
        }

        public bool ArrivedAtDestination()
        {
            float current_distance = Vector3.Distance(SourceLocation, Combatant.transform.position);
            float target_distance = Vector3.Distance(SourceLocation, DestnationLocation);
            if (current_distance > target_distance - distanceEpsilon)
            {
                return true;
            }

            return false;
        }
    }
}