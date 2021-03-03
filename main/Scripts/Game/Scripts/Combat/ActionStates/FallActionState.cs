using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FallActionState : AnimationOnceReactionActionState
    {
        public static readonly string fallMoveName = "Fall";

        public FallActionState(Combatant combatant)
            : base(combatant)
        {
            MoveName = fallMoveName;
        }

        public FallActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = fallMoveName;
        }

        public override void End()
        {
            base.End();

            SwitchState(new LieActionState(Combatant));
        }

        public override float CalculateLeftTime()
        {
            float current_left_time = base.CalculateLeftTime();

            LieActionState next = new LieActionState(Combatant);

            return current_left_time + next.CalculateLeftTime();
        }

        public override float CalculateComboTime()
        {
            float current_left_time = base.CalculateLeftTime();

            LieActionState next = new LieActionState(Combatant);

            return current_left_time + next.CalculateComboTime();
        }
    }
}