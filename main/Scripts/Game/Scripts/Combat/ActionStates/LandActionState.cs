using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class LandActionState : FlowActionState
    {
        public LandActionState(Combatant combatant, eHeightType height)
            : base(combatant, height)
        {
            //MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            //StartFrame = FlowLandFrame;
            //NormalizedTime = StartFrame / move.NumFrames;
        }

        public LandActionState()
        {

        }

        public override void Start()
        {
            base.Start();

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = FlowLandFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            Vector3 current_position = Combatant.transform.position;
            current_position.y = FlowLandPoint.y;
            Combatant.transform.position = current_position;
        }

        public override void End()
        {
            base.End();

            TryReturnToOriginPosition();
        }
    }
}