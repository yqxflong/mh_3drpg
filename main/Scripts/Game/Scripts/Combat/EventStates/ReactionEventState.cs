using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ReactionEventState : CombatEventState
    {
        public ReactionEffectEvent ReactionEvent
        {
            get;
            private set;
        }

        public ReactionEventState(Combatant combatant, ReactionEffectEvent reaction)
            : base(combatant, reaction)
        {
            ReactionEvent = reaction;
        }

        public ReactionEventState()
        {

        }

        public override void Init(Combatant combatant, CombatEvent combat_event)
        {
            //Debug.Assert(combat_event is ReactionEffectEvent, "invalid event type");

            base.Init(combatant, combat_event);

            ReactionEvent = combat_event as ReactionEffectEvent;
        }

        public override void CleanUp()
        {
            Combatant.StopAllCoroutines();
            ReactionEvent = null;
            base.CleanUp();
        }

        protected override void OnStart()
        {
            //CombatEventReceiver.Instance.OnCombatEventTiming(ReactionEvent, eCombatEventTiming.ON_REACTION_START);

            StartActionState();

            if (ReactionEvent.IsMiss)
            {
                Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.Missed);
            }
            else if (ReactionEvent.IsBlocked)
            {
                Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.Blocked);
            }
            else
            {
                Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.Hit);
            }

            long hp = Combatant.GetHP() - ReactionEvent.Damage;
            Combatant.SetHP(hp);
            //CombatInfoData.GetInstance().Log("damage applyed, hp = " + hp + ", source = " + ReactionEvent.Sender + ", target = " + ReactionEvent.Target);
            //if(ReactionEvent.Damage>0)
            //	EventManager.instance.Raise(new CombatHitDamageEvent(Combatant.gameObject, ReactionEvent.Damage, ReactionEvent.Show, ReactionEvent.Shield));

            // EB.Debug.Log(string.Format("ReactionEventState.OnStart: {0} start action {1}", Combatant.myName, Combatant.ActionState.GetType().ToString()));

            if (!Combatant.IsAlive())
            {
                //CombatSkillEvent skill_event = CombatEventReceiver.Instance.FindEventTree(ReactionEvent).Parent.Event as CombatSkillEvent;
                //if (CombatEventReceiver.Instance.IsLastSkill(skill_event))
                //{
                //    CombatEventReceiver.Instance.PlayRadialBlurEffect(Combatant);
                //}
            }
        }

        protected override void OnUpdate()
        {
            MoveController.CombatantMoveState move_state = Combatant.GetMoveState();

            if (move_state != MoveController.CombatantMoveState.kHitReaction)
            {
                End();
                return;
            }
        }

        protected override void OnEnd()
        {
            //CombatEventReceiver.Instance.OnCombatEventTiming(ReactionEvent, eCombatEventTiming.ON_REACTION_END);
        }

        protected override void OnTransitionTo(CombatEventState next_action)
        {
            End();

            Combatant.EventState = next_action;
            Combatant.EventState.Start();
        }

        protected void StartActionState()
        {
            bool action_exists = ActionExists(ReactionEvent.Reaction);

            if (!action_exists)
            {
                EB.Debug.LogWarning("reaction not found, use hit reaction default");
                Combatant.ActionState.TransitionToHit(ReactionEvent);
                return;
            }

            switch (ReactionEvent.Reaction)
            {
                case MoveEditor.PlayHitReactionProperties.eReactionType.Back:
                    Combatant.ActionState.TransitionToBack(ReactionEvent);
                    break;
                case MoveEditor.PlayHitReactionProperties.eReactionType.Down:
                    Combatant.ActionState.TransitionToDown(ReactionEvent);
                    break;
                case MoveEditor.PlayHitReactionProperties.eReactionType.LittleFlow:
                    Combatant.ActionState.TransitionToLittleFlow(ReactionEvent);
                    break;
                case MoveEditor.PlayHitReactionProperties.eReactionType.LargeFlow:
                    Combatant.ActionState.TransitionToLargeFlow(ReactionEvent);
                    break;
                case MoveEditor.PlayHitReactionProperties.eReactionType.Block:
                    Combatant.ActionState.TransitionToBlock(ReactionEvent);
                    break;
                case MoveEditor.PlayHitReactionProperties.eReactionType.Hit:
                    try
                    {
                        Combatant.ActionState.TransitionToHit(ReactionEvent);
                    }
                    catch
                    {
                       EB.Debug.LogError("Combatant.ActionState is null");
                    }
                    break;
            }
        }

        protected bool ActionExists(MoveEditor.PlayHitReactionProperties.eReactionType reaction)
        {
            MoveController mc = Combatant.MoveController;
            switch (reaction)
            {
                case MoveEditor.PlayHitReactionProperties.eReactionType.Back:
                    return mc.GetMoveIfExists(BackActionState.backMoveName) != null;
                case MoveEditor.PlayHitReactionProperties.eReactionType.Down:
                    return mc.GetMoveIfExists(DownActionState.downMoveName) != null;
                case MoveEditor.PlayHitReactionProperties.eReactionType.LittleFlow:
                case MoveEditor.PlayHitReactionProperties.eReactionType.LargeFlow:
                    return mc.GetMoveIfExists(FlowActionState.flowMoveName) != null;
                case MoveEditor.PlayHitReactionProperties.eReactionType.Block:
                    return mc.GetMoveIfExists(BlockActionState.blockMoveName) != null;
            }

            return true;
        }
    }
}