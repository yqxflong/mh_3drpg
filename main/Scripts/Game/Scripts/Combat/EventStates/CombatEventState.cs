using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatEventState
    {
        protected Combatant m_combatant = null;
        protected CombatEvent m_event = null;
        protected bool m_disabled = false;
        protected float m_startTime = .0f;

        public bool Disabled
        {
            get { return m_disabled; }
            protected set { m_disabled = value; }
        }

        public float StartTime
        {
            get { return m_startTime; }
            private set { m_startTime = value; }
        }

        public float StateTime
        {
            get { return Time.time - StartTime; }
        }

        public CombatEvent Event
        {
            get { return m_event; }
            private set { m_event = value; }
        }

        public Combatant Combatant
        {
            get { return m_combatant; }
            private set { m_combatant = value; }
        }

        public CombatEventState(Combatant combatant, CombatEvent combat_event)
        {
            Combatant = combatant;
            Event = combat_event;

            Disabled = true;
            StartTime = Time.time;
        }

        public CombatEventState()
        {

        }

        public virtual void Init(Combatant combatant, CombatEvent combat_event)
        {
            Combatant = combatant;
            Event = combat_event;
            Disabled = true;
            StartTime = Time.time;
        }

        public virtual void CleanUp()
        {
            Disabled = false;
            StartTime = 0.0f;
            Event = null;
            Combatant = null;
        }

        public virtual void Start()
        {
            if (!Disabled)
            {
                return;
            }
            Disabled = false;

            // clean state
            Combatant.FXHelper.InterruptFX();

            OnStart();
        }

        protected virtual void OnStart()
        {

        }

        public virtual void Update()
        {
            if (Disabled)
            {
                return;
            }

            OnUpdate();
        }

        protected virtual void OnUpdate()
        {

        }

        public virtual void End()
        {
            if (Disabled)
            {
                return;
            }
            Disabled = true;

            if (Combatant.EventState == this)
            {
                Combatant.EventState = null;
            }
            else
            {
                EB.Debug.LogWarning(string.Format("CombatEventState.Stop: EventState changed, {0} => {1}", Event.GetHashCode(), Combatant.EventState.Event.GetHashCode()));
            }

            OnEnd();

            Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.None);

            Combatant.StashEventState(this);
        }

        protected virtual void OnEnd()
        {

        }

        public virtual void Stop()
        {
            if (Disabled)
            {
                return;
            }
            Disabled = true;

            Combatant.ActionState.Stop();

            if (Combatant.EventState == this)
            {
                Combatant.EventState = null;
            }
            else
            {
                EB.Debug.LogWarning(string.Format("CombatEventState.Stop: EventState changed, {0} => {1}", Event.GetHashCode(), Combatant.EventState.Event.GetHashCode()));
            }

            OnStop();

            //var list = CombatEventReceiver.Instance.FindEventTree(Event).ChildrenToListNonAlloc();
            //while (list.MoveNext())
            //{
            //    var node = list.Current;
            //    if (node.NodeState == CombatEventTreeState.INITED)
            //    {
            //        CombatEventReceiver.Instance.OnCombatEventEnd(node.Event);
            //    }
            //}
            //CombatEventReceiver.Instance.OnCombatEventEnd(Event);

            Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.None);

            Combatant.StashEventState(this);
        }

        protected virtual void OnStop()
        {

        }

        public virtual void Interrupt()
        {
            if (Disabled)
            {
                return;
            }
            Disabled = true;

            Combatant.ActionState.Stop();

            if (Combatant.EventState == this)
            {
                Combatant.EventState = null;
            }
            else
            {
                EB.Debug.LogWarning(string.Format("CombatEventState.Interrupt: EventState changed, {0} => {1}", Event.GetHashCode(), Combatant.EventState.Event.GetHashCode()));
            }

            //CombatEventReceiver.Instance.OnCombatEventEnd(Event);

            Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.None);

            Combatant.StashEventState(this);
        }

        public virtual void TransitionTo(CombatEventState next_action)
        {
            if (next_action == null)
            {
                Stop();
                return;
            }

            if (Disabled)
            {
                Combatant.EventState = next_action;
                Combatant.EventState.Start();
                return;
            }

            OnTransitionTo(next_action);
        }

        protected virtual void OnTransitionTo(CombatEventState next_action)
        {

        }

        #region move editor events
        public virtual void OnPlayTargetHitReactionEvent(MoveEditor.MoveAnimationEvent ee)
        {

        }

        public virtual void OnReactionComboEvent(MoveEditor.MoveAnimationEvent ee)
        {

        }

        public virtual void OnInflictHit(MoveEditor.MoveAnimationEvent ee)
        {

        }

        public virtual void OnTriggerCombo(MoveEditor.MoveAnimationEvent ee)
        {

        }
        #endregion
    }
}