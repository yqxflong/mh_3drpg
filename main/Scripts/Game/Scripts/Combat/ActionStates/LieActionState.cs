using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class LieActionState : DownActionState
    {
        public LieActionState(Combatant combatant)
            : base(combatant)
        {
            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = LieFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = StandUpFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public LieActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveEditor.Move move = Combatant.MoveController.GetMove(MoveName, false);

            StartFrame = LieFrame;
            NormalizedTime = StartFrame / move.NumFrames;

            EndFrame = StandUpFrame;
            EndNormalizedTime = EndFrame / move.NumFrames;
        }

        public override void Start()
        {
            base.Start();

            SetupMove(MoveName, AnimatorStateName, NormalizedTime);

            Vector3 current_position = Combatant.transform.position;
            current_position.y = LiePoint.y;
            Combatant.transform.position = current_position;
        }

        protected override void OnEnd()
        {
            if (Combatant.Animator.speed > 0.0f)
            {
                if (ReactionEvent.PlayEffect && ReactionEvent.WillBeCombo)
                {
                    EB.Debug.LogWarning("LieActionState.OnEnd: waiting for combo");
                    Combatant.PauseAnimation(0.0f);
                    return;
                }

                End();
                return;
            }
        }

        public override void End()
        {
            base.End();

            SwitchState(Combatant.GetActionState<StandUpActionState>());
        }

        public override void Stop()
        {
            if (Combatant.Animator.speed > 0.0f)
            {
                base.Stop();
            }
            else
            {
                End();
            }
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