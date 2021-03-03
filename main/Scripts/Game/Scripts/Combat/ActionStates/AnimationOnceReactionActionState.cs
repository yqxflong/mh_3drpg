using UnityEngine;
using System.Collections;

namespace Hotfix_LT.Combat
{
    public class AnimationOnceReactionActionState : ReactionActionState
    {
        public bool RotateToIdle
        {
            get;
            set;
        }

        public float EndNormalizedTime
        {
            get;
            set;
        }

        public bool UseDefaultMove
        {
            get;
            set;
        }

        public AnimationOnceReactionActionState(Combatant combatant)
            : base(combatant)
        {
            RotateToIdle = false;
            EndNormalizedTime = 1.0f - timeEpsilon;
            UseDefaultMove = false;
        }

        public AnimationOnceReactionActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            RotateToIdle = false;
            EndNormalizedTime = 1.0f - timeEpsilon;
            UseDefaultMove = false;
        }

        public override void CleanUp()
        {
            base.CleanUp();

            RotateToIdle = false;
            EndNormalizedTime = 0.0f;
            UseDefaultMove = false;
        }

        public override void Start()
        {
            base.Start();

            if (!UseDefaultMove)
            {
                SetupMove(MoveName, AnimatorStateName, NormalizedTime);
            }
            else
            {
                SetupMoveUseDefault(MoveName, AnimatorStateName, NormalizedTime);
            }

            // stay in original position
            Combatant.transform.position = Combatant.OriginPosition;
        }

        public override void Update()
        {
            base.Update();

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {
                EB.Debug.LogError("invalid move state");
                Stop();
                return;
            }

            if (state_hash != state_info.fullPathHash)
            {
                // animator does not start blending
                return;
            }

            if (state_info.normalizedTime > EndNormalizedTime && NormalizedTime > EndNormalizedTime)
            {
                End();
                return;
            }

            NormalizedTime = state_info.normalizedTime;

            if (RotateToIdle)
            {
                TryRotateTowardToIdle();
            }
        }
    }
}