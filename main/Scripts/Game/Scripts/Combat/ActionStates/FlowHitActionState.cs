using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class FlowHitActionState : AnimationOnceReactionActionState
    {
        private static readonly string flowHitMoveName = "FlowHit";

        public FlowActionState.eHeightType FlowHeight
        {
            get;
            set;
        }

        public FlowHitActionState(Combatant combatant, FlowActionState.eHeightType height)
            : base(combatant)
        {
            MoveName = flowHitMoveName;
            FlowHeight = height;
        }

        public FlowHitActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveName = flowHitMoveName;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            FlowHeight = FlowActionState.eHeightType.LITTLE;
        }

        public FlowHitActionState SetHeight(FlowActionState.eHeightType height)
        {
            FlowHeight = height;
            return this;
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<LandActionState>().SetHeight(FlowHeight) as LandActionState);
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