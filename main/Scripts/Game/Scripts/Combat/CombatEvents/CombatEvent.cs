using System;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public enum eCombatEventType
    {
        INVALID = -1,
        SKILL,
        IMPACT,
        EFFECT,
        SYNC,
        CONVERSATION,
        QUIT,
        GUIDE,
        LOBBY,
        MAX
    }

    public enum eCombatEventTiming
    {
        RUNTIME = -2,
        INVALID = -1,
        AUTO,
        ON_START,
        ON_END,
        ON_FORWARD_START,
        ON_FORWARD_END,
        ON_SKILL_START,
        ON_SKILL_END,
        ON_HIT_START,
        ON_HIT_END,
        ON_BACKWARD_START,
        ON_BACKWARD_END,
        HIT_QUEUE,  // dynamic
        ON_HIT,     // dynamic
        ON_REACTION_START,
        ON_REACTION_END,
        ON_COMBO,
        COLLIDE_QUEUE,  // dynamic
        MAX
    }

    public class CombatEventTimingComparer : IEqualityComparer<eCombatEventTiming>
    {
        public bool Equals(eCombatEventTiming x, eCombatEventTiming y)
        {
            return x == y;
        }

        public int GetHashCode(eCombatEventTiming obj)
        {
            return (int)obj;
        }

        public static CombatEventTimingComparer Default = new CombatEventTimingComparer();
    }

    public class CombatEvent
    {
        protected eCombatEventType m_type = eCombatEventType.INVALID;
        protected eCombatEventTiming m_timing = eCombatEventTiming.INVALID;
        protected List<CombatEvent> m_children = new List<CombatEvent>();
        protected int m_parent = -1;

        private static int s_index = 0;
        protected int m_unique_code = -1;
        protected string m_log_id = null;

        public eCombatEventType Type
        {
            get { return m_type; }
        }

        public eCombatEventTiming Timing
        {
            get { return m_timing; }
            set { m_timing = value; }
        }

        public List<CombatEvent> Children
        {
            get { return m_children; }
        }

        public int Parent
        {
            get { return m_parent; }
        }

        public CombatEvent()
        {
            m_unique_code = s_index++;
        }

        public int GetUniqueCode()
        {
            if (m_unique_code < 0)
            {
                EB.Debug.LogError("CombatEvent.GetUniqueCode: m_unique_code not inited");
                m_unique_code = s_index++;
            }
            return m_unique_code;
        }

        public virtual string GetLogId()
        {
            return m_log_id = m_log_id ?? string.Format("{0}_{1}_{2}", m_timing.ToString(), m_type.ToString(), m_unique_code.ToString());
        }

        private List<CombatantIndex> emptyInvolved = new List<CombatantIndex>();
        public virtual List<CombatantIndex> GetInvolved()
        {
            return emptyInvolved;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }

            if (obj is CombatEvent == false)
            {
                return false;
            }

            CombatEvent cmp_evt = obj as CombatEvent;
            return cmp_evt.GetUniqueCode() == GetUniqueCode();
        }

        public override int GetHashCode()
        {
            return GetUniqueCode();
        }

        public bool Equals(CombatEvent cmp_evt)
        {
            if (cmp_evt == null)
            {
                return false;
            }

            if (cmp_evt == this)
            {
                return true;
            }

            return cmp_evt.GetUniqueCode() == GetUniqueCode();
        }

        public virtual bool Parse(Hashtable info)
        {
            m_type = ParseType(info);
            if (m_type == eCombatEventType.INVALID)
            {
                EB.Debug.LogWarning("CombatEvent.Parse: invalid type = {0}", m_type);
                return false;
            }

            m_timing = ParseTiming(info);
            if (m_timing == eCombatEventTiming.INVALID)
            {
                EB.Debug.LogWarning("CombatEvent.Parse: invalid timing = {0}", m_timing);
                return false;
            }

            m_parent = EB.Dot.Integer("parent", info, -1);

            return true;
        }

        protected static eCombatEventType ParseType(Hashtable info)
        {
            string type_info = EB.Dot.String("type", info, null);
            if (string.IsNullOrEmpty(type_info))
            {
                return eCombatEventType.INVALID;
            }

            return CombatUtil.GetType(type_info);
        }

        protected static eCombatEventTiming ParseTiming(Hashtable info)
        {
            eCombatEventTiming timing = eCombatEventTiming.AUTO;

            string timing_info = EB.Dot.String("timing", info, null);
            if (!string.IsNullOrEmpty(timing_info))
            {
                timing = CombatUtil.GetTiming(timing_info);
            }

            return timing;
        }
    }
}