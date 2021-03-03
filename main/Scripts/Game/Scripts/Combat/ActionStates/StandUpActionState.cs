using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class StandUpActionState : DownActionState
    {
        public StandUpActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = StandUpFrame;
            NormalizedTime = StartFrame / move.NumFrames;
        }

        public StandUpActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = StandUpFrame;
            NormalizedTime = StartFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);
        }

        public override void End()
        {
            base.End();

            TryReturnToOriginPosition();
        }
    }
}