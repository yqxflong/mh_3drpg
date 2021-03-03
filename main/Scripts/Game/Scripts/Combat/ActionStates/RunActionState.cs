using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class RunActionState : CombatActionState
    {
        private const float DefaultSpeed = 2.5f;

        public float Speed
        {
            get;
            set;
        }

        public Vector3 Direction
        {
            get;
            private set;
        }

        public Vector3 SourceLocation
        {
            get;
            private set;
        }

        public Vector3 DestinationLocation
        {
            get;
            set;
        }

        public RunActionState(Combatant combatant)
            : base(combatant)
        {
            MoveState = MoveController.CombatantMoveState.kLocomotion;
            SourceLocation = StartPosition;
            Speed = DefaultSpeed;
        }

        public RunActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveState = MoveController.CombatantMoveState.kLocomotion;
            SourceLocation = StartPosition;
            Speed = DefaultSpeed;
        }

        public override void CleanUp()
        {
            Speed = 0.0f;
            Direction = Vector3.zero;
            SourceLocation = Vector3.zero;
            DestinationLocation = Vector3.zero;

            base.CleanUp();
        }

        public override void Start()
        {
            // calculate rotation, rotate immediately
            Combatant.transform.rotation = CalculateForwardRotation(DestinationLocation);

            if (ArrivedAtDestination())
            {
                End();
                return;
            }

            // change state and cross fade to forward animator state
            Combatant.MoveController.TransitionTo(MoveState);
            Combatant.MoveController.CrossFade(MoveController.m_locomotion_hash, defaultBlendTime, defaultLayer, NormalizedTime);

            Direction = (DestinationLocation - SourceLocation).normalized;
        }

        public override void Update()
        {
            base.Update();

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (move_state != MoveState)
            {
                EB.Debug.LogError("move state not match");
                Stop();
                return;
            }

            if (state_hash != state_info.fullPathHash)
            {
                return;
            }

            NormalizedTime = state_info.normalizedTime;

            UpdateLocation();
            if (ArrivedAtDestination())
            {
                End();
            }
        }

        public override void End()
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(true));
        }

        public override void Stop()
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(true));
        }

        protected Quaternion CalculateForwardRotation(Vector3 target_position)
        {
            Vector3 move_direction = target_position - Combatant.transform.position;
            Quaternion rotation = Quaternion.LookRotation(move_direction, Vector3.up);
            return rotation;
        }

        protected void UpdateLocation()
        {
            Combatant.transform.position += Time.deltaTime * Speed * Direction;
        }

        protected bool ArrivedAtDestination()
        {
            float current_distance = Vector3.Distance(SourceLocation, Combatant.transform.position);
            float target_distance = Vector3.Distance(SourceLocation, DestinationLocation);
            if (current_distance > target_distance - distanceEpsilon)
            {
                return true;
            }

            return false;
        }

        public void ResetSpeed()
        {
            Speed = DefaultSpeed;
        }

        public override float CalculateLeftTime()
        {
            return 0.0f;
        }
    }
}