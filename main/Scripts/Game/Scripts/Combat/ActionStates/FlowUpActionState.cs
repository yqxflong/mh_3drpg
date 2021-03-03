using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FlowUpActionState : FlowActionState
    {
        public FlowUpActionState(Combatant combatant, eHeightType height)
            : base(combatant, height)
        {
            //MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            //StartFrame = FlowUpFrame;
            //NormalizedTime = StartFrame / move.NumFrames;

            //EndFrame = FlowTopFrame;
            //EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public FlowUpActionState()
        {

        }

        public override void Start()
        {
            base.Start();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = FlowUpFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = FlowTopFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            if (IsReachedFlowPoint())
            {
                End();
                return;
            }

            //EB.Debug.Log("FlowUpActionState.Start: NormalizedTime = {0}, EndNormalizedTime = {1}", NormalizedTime, EndNormalizedTime);

            float flowuppoint_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(FlowUpFrame);
            float flowpoint_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(FlowTopFrame);

            float anim_distance = flowpoint_offset - flowuppoint_offset;
            float actual_distance = (FlowTopPoint.y - Combatant.OriginPosition.y) - flowuppoint_offset;

            Scale = actual_distance / anim_distance;

            Vector3 current_position = Combatant.transform.position;
            current_position.y = flowuppoint_offset + Combatant.OriginPosition.y;
            Combatant.transform.position = current_position;
        }

        public override void End()
        {
            base.End();

            FlowDownActionState down = Combatant.GetActionState<FlowDownActionState>().SetHeight(FlowHeight) as FlowDownActionState;
            SwitchState(down);
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
            float y = FlowUpPoint.y + (anim_y_offset - (FlowUpPoint.y - Combatant.OriginPosition.y)) * Scale;

            Vector3 current_position = Combatant.transform.position;
            current_position.y = y;
            Combatant.transform.position = current_position;
        }

        protected bool IsReachedFlowPoint()
        {
            Vector3 current = FlowTopPoint - Combatant.transform.position;
            Vector3 direction = FlowTopPoint - FlowUpPoint;
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
    }
}