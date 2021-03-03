using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CollideActionState : BackActionState
    {
        public CollideActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = CollideFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = ReturnFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public CollideActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = CollideFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = ReturnFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<ReturnActionState>());
        }

        protected override void UpdateLocation()
        {
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            float current_frame = Combatant.CurrentMove.NumFrames * state_info.normalizedTime;
            if (current_frame > EndFrame)
            {
                current_frame = EndFrame;
            }
            float y_offset = Combatant.CurrentMove._yOffsetCurve.Evaluate(current_frame);

            Vector3 current_position = Combatant.transform.position;
            current_position.y = Combatant.OriginPosition.y + y_offset;
            Combatant.transform.position = current_position;
        }
    }
}