using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class LieHitActionState : AnimationOnceReactionActionState
    {
        public static readonly string lieHitMoveName = "LieHit";

        public LieHitActionState(Combatant combatant)
            : base(combatant)
        {
            MoveName = lieHitMoveName;
        }

        public LieHitActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = lieHitMoveName;
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<StandUpActionState>());
        }

        public override void TransitionToHit(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<LieHitActionState>());
        }

        public override void TransitionToBack(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<BackStartActionState>());
        }

        public override void TransitionToDown(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            SwitchState(Combatant.GetActionState<LieHitActionState>());
        }

        public override void TransitionToLittleFlow(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            FlowStartActionState next = Combatant.GetActionState<FlowStartActionState>().SetHeight(FlowActionState.eHeightType.LITTLE) as FlowStartActionState;
            SwitchState(next);
        }

        public override void TransitionToLargeFlow(ReactionEffectEvent reaction_event)
        {
            IsEnd = true;

            FlowStartActionState next = Combatant.GetActionState<FlowStartActionState>().SetHeight(FlowActionState.eHeightType.LARGE) as FlowStartActionState;
            SwitchState(next);
        }
    }
}