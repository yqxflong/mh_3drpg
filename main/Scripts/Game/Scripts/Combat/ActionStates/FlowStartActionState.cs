using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FlowStartActionState : FlowActionState
    {
        public FlowStartActionState(Combatant combatant, eHeightType height)
            : base(combatant, height)
        {
            //MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            //EndFrame = FlowUpFrame;
            //EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public FlowStartActionState()
        {

        }

        public override void Start()
        {
            base.Start();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            EndFrame = FlowUpFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            //EB.Debug.Log("FlowStartActionState.Start: NormalizedTime = {0}, EndNormalizedTime = {1}", NormalizedTime, EndNormalizedTime);

            Combatant.transform.position = Combatant.OriginPosition;

            //HealthBar2D health_bar = Combatant.HealthBar;
            //if (health_bar != null)
            //{
            //    health_bar.HideHealthBar(false);
            //}
            var health_bar = Combatant.HealthBar;
            if (health_bar != null)
            {
                health_bar.OnHandleMessage("HideHealthBar", false);
            }
        }

        public override void End()
        {
            base.End();

            FlowUpActionState up = Combatant.GetActionState<FlowUpActionState>().SetHeight(FlowHeight) as FlowUpActionState;
            SwitchState(up);
        }
    }
}