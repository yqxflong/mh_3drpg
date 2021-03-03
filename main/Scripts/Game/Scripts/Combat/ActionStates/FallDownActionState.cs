using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FallDownActionState : DownActionState
    {
        public FallDownActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            EndFrame = LieFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public FallDownActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            EndFrame = LieFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

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

            SwitchState(new LieActionState(Combatant));
        }
    }
}