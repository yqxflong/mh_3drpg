using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public enum eCombatEffectType
    {
        INVALID = -1,
        REACTION,
        MISS,
        DAMAGE,
        HEAL,
        IMPACT_ACTIVE,
        IMPACT_FADEOUT,
        BREAK_MANUAL_SKILL,
        Add_COMBATANT,
        REMOVE_COMBATANT,
        EXILE,
        SHIELD,
        ATTRIBUTES,
        REVIVE,
        TIPS,
        MAX
    }

    public interface IHitQueueSplittable
    {
        int Times { get; set; }
        int TotalTimes { get; set; }
        CombatEffectEvent Split(float rating);
        bool IsFirst();
        bool IsLast();
        List<eCombatEventTiming> TransferChildren();
    }

    public interface IHitQueueHitable
    {
        bool PlayEffect { get; set; }
        bool EffectPlayed { get; set; }
        void Hit(bool use_effect_action, MoveEditor.PlayHitReactionProperties.eReactionType default_action);
    }

    public class CombatEffectEvent : CombatEvent
    {
        protected eCombatEffectType m_effect_type = eCombatEffectType.INVALID;
        protected CombatantIndex m_target = null;
        protected CombatantIndex m_sender = null;

        public eCombatEffectType EffectType
        {
            get { return m_effect_type; }
            set { m_effect_type = value; }
        }

        public CombatantIndex Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        public CombatantIndex Sender
        {
            get { return m_sender; }
            set { m_sender = value; }
        }

        public CombatEffectEvent()
        {
            m_type = eCombatEventType.EFFECT;
        }

        public override string GetLogId()
        {
            return m_log_id = m_log_id ?? string.Format("{0}_{1}", m_effect_type.ToString(), base.GetLogId());
        }

        private List<CombatantIndex> cachedInvolved = null;
        public override List<CombatantIndex> GetInvolved()
        {
            if (cachedInvolved != null)
            {
                return cachedInvolved;
            }

            List<CombatantIndex> list = new List<CombatantIndex>();
            list.Add(m_target);
            if (m_sender != null)
            {
                list.Add(m_sender);
            }
            cachedInvolved = list;
            return list;
        }

        public override bool Parse(Hashtable info)
        {
            m_timing = ParseTiming(info);
            if (m_timing == eCombatEventTiming.INVALID)
            {
                EB.Debug.LogWarning("CombatEffectEvent.Parse: invalid timing = {0}", m_timing);
                return false;
            }

            int target_team_index = EB.Dot.Integer("target.team", info, -1);
            if (target_team_index < 0)
            {
                EB.Debug.LogWarning("CombatEffectEvent.Parse: target.team is empty");
                return false;
            }
            int target_index_on_team = EB.Dot.Integer("target.combatant", info, -1);
            if (target_index_on_team < 0)
            {
                EB.Debug.LogWarning("CombatEffectEvent.Parse: target.combatant is empty");
                return false;
            }
            m_target = new CombatantIndex();
            m_target.TeamIndex = target_team_index;
            m_target.IndexOnTeam = target_index_on_team;

            int parent_effect_index = EB.Dot.Integer("parent", info, -1);
            if (parent_effect_index >= 0)
            {
                m_parent = parent_effect_index;
            }

            int sender_team_index = EB.Dot.Integer("sender.team", info, -1);
            int sender_index_on_team = EB.Dot.Integer("sender.combatant", info, -1);
            if (sender_team_index >= 0 && sender_index_on_team >= 0)
            {
                m_sender = new CombatantIndex();
                m_sender.TeamIndex = sender_team_index;
                m_sender.IndexOnTeam = sender_index_on_team;
            }

            return true;
        }
    }
}