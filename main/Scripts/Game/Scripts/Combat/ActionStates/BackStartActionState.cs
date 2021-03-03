using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class BackStartActionState : BackActionState
    {
        public BackStartActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            EndFrame = BackFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public BackStartActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            EndFrame = BackFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            Combatant.transform.position = Combatant.OriginPosition;
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<FallBackActionState>());
        }
    }
}