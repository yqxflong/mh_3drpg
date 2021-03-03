using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ReviveActionState : CombatActionState
    {
        public static readonly string reviveMoveName = "Revive";

        public ReviveActionState(Combatant combatant)
            : base(combatant)
        {
            MoveState = MoveController.CombatantMoveState.kRevive;
        }

        public ReviveActionState()
        {

        }

        public override void Init(Combatant combatant)
        {
            base.Init(combatant);

            MoveState = MoveController.CombatantMoveState.kRevive;
        }

        public override void Start()
        {
            if (Combatant.MoveController.GetMoveByState(MoveController.CombatantMoveState.kRevive) == null)
            {
                EB.Debug.LogWarning("ReviveActionState.Start: revive move not exists");
                End();
                return;
            }

            Combatant.MoveController.TransitionTo(MoveState);
            Combatant.MoveController.CrossFade(MoveController.m_revive_hash, 0, defaultLayer, NormalizedTime);

            //HealthBar2D health_bar = Combatant.HealthBar;
            //if (health_bar != null && health_bar.Hidden)
            //{
            //    health_bar.ShowHealthBar();
            //}
            var health_bar = Combatant.HealthBar;
            if (health_bar != null)
            {
                health_bar.OnHandleMessage("ShowHealthBar", null);
            }

            Hotfix_LT.UI.LTCombatEventReceiver.Instance.SetDeath(Combatant, false);
            Combatant.RemoveLoopImpactFX();  //need RemoveLoopImpactFX
        }

        public override void Update()
        {
            base.Update();

            int state_hash = Combatant.MoveController.GetCurrentAnimHash();
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
            if (state_hash != state_info.fullPathHash)
            {// animation not ready
                return;
            }

            NormalizedTime = state_info.normalizedTime;

            bool revive_anim_end = NormalizedTime > 1.0f - timeEpsilon;
            if (revive_anim_end)
            {
                End();
            }
        }

        public override void End()
        {
            base.End();

            Combatant.RestoreLoopImpactFX();
            Combatant.EndRevive();

            SwitchState(Combatant.GetActionState<ReadyActionState>().SetAutoCrossFade(false));

            Hotfix_LT.UI.LTCombatEventReceiver.Instance.EndSkill(Combatant.Data);
        }

        public override void Stop()
        {
            End();
        }

        public override float CalculateLeftTime()
        {
            float normalized_time = NormalizedTime;
            float speed = Combatant.Animator.speed;

            MoveEditor.Move move = Combatant.MoveController.GetMove(reviveMoveName, false);
            if (move == null)
            {
                EB.Debug.LogWarning("ReviveActionState.CalculateLeftTime: revive move not exists");
                return 0.0f;
            }

            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();
            int current_state_hash = Combatant.MoveController.GetCurrentAnimHash();
            int this_state_hash = MoveController.m_revive_hash;
            AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();

            if (this_state_hash != current_state_hash)
            {// just new
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {// leaving
                normalized_time = 1.0f;
            }
            else if (current_state_hash != state_info.fullPathHash)
            {// blending
                normalized_time = NormalizedTime;
                speed = move.Speed;
            }
            else if (NormalizedTime > 0.0f && state_info.normalizedTime >= NormalizedTime)
            {// not repeat
                normalized_time = state_info.normalizedTime;
            }

            float left_time = move._animationClip.length * (1.0f - normalized_time);
            return left_time / speed;
        }
    }
}