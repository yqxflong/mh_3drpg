using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FlowDownActionState : FlowActionState
    {
        public FlowDownActionState(Combatant combatant, eHeightType height)
            : base(combatant, height)
        {
            //MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            //StartFrame = FlowTopFrame;
            //NormalizedTime = StartFrame / move.NumFrames;

            //EndFrame = FlowLandFrame;
            //EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public FlowDownActionState()
        {

        }

        public override void Start()
        {
            base.Start();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = FlowTopFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = FlowLandFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            if (IsReachedLandPoint())
            {
                End();
                return;
            }

            //EB.Debug.Log("FlowDownActionState.Start: NormalizedTime = {0}, EndNormalizedTime = {1}", NormalizedTime, EndNormalizedTime);

            float flow_point = Combatant.CurrentMove._yOffsetCurve.Evaluate(FlowTopFrame);
            float land_point = Combatant.CurrentMove._yOffsetCurve.Evaluate(FlowLandFrame);

            float anim_distance = flow_point - land_point;
            float actual_distance = (FlowTopPoint.y - Combatant.OriginPosition.y) - land_point;

            Scale = actual_distance / anim_distance;
        }

        protected override void OnEnd()
        {
            if (Combatant.Animator.speed > 0.0f)
            {
                if (ReactionEvent.PlayEffect && ReactionEvent.WillBeCombo)
                {
                    EB.Debug.LogWarning("FlowDownActionState.OnEnd: waiting for combo");
                    Combatant.PauseAnimation(0.0f);
                    return;
                }

                End();
                return;
            }
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<LandActionState>().SetHeight(FlowHeight) as LandActionState);
        }

        public override void Stop()
        {
            if (Combatant.Animator.speed > 0.0f)
            {
                base.Stop();
            }
            else
            {
                End();
            }
        }

        protected override void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            if (current_frame > EndFrame)
            {
                current_frame = EndFrame;
            }
            float anim_y_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(current_frame);
            //float y = anim_y_offset + Combatant.OriginPosition.y;

            Vector3 current_position = Combatant.transform.position;
            current_position.y = FlowLandPoint.y + (anim_y_offset - (FlowLandPoint.y - Combatant.OriginPosition.y)) * Scale;
            Combatant.transform.position = current_position;
        }

        protected bool IsReachedLandPoint()
        {
            Vector3 current = FlowLandPoint - Combatant.transform.position;
            Vector3 direction = FlowLandPoint - FlowTopPoint;
            if (Vector3.Dot(current, direction) < 0)
            {
                return true;
            }

            if (current.magnitude < distanceEpsilon)
            {
                return true;
            }

            return false;
        }

        public override void TransitionToHit(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<FlowHitActionState>().SetHeight(FlowHeight) as FlowHitActionState);
        }

        public override void TransitionToBack(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<FallBackActionState>());
        }

        public override void TransitionToDown(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<FallActionState>());
        }

        public override void TransitionToLittleFlow(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            FlowUpActionState fu = Combatant.GetActionState<FlowUpActionState>().SetHeight(FlowActionState.eHeightType.LITTLE) as FlowUpActionState;
            SwitchState(fu);
        }

        public override void TransitionToLargeFlow(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            FlowUpActionState fu = Combatant.GetActionState<FlowUpActionState>().SetHeight(FlowActionState.eHeightType.LARGE) as FlowUpActionState;
            SwitchState(fu);
        }
    }
}