using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class IdleActionState : ReadyActionState
    {
        public IdleActionState(Combatant combatant)
            : base(combatant, false)
        {
            MoveState = MoveController.CombatantMoveState.kIdle;
        }

        public IdleActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveState = MoveController.CombatantMoveState.kIdle;
        }

        public override void Start()
        {
            float blend_time = 0.15f * Combatant.MoveController.GetCurrentStateInfo().length;
            Combatant.MoveController.TransitionTo(MoveState);
            Combatant.MoveController.CrossFade(MoveController.m_idle_hash, blend_time, defaultLayer, NormalizedTime);
        }
    }
}