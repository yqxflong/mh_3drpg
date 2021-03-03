using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class CombatSyncEvent : CombatEvent
    {
        public CombatSyncEvent()
        {
            m_type = eCombatEventType.SYNC;
            m_timing = eCombatEventTiming.AUTO;
        }

        public override bool Parse(Hashtable info)
        {
            eCombatEventType type = ParseType(info);
            if (type != eCombatEventType.SYNC)
            {
                EB.Debug.LogWarning("CombatSyncEvent.Parse: invalid type = {0}", type);
                return false;
            }

            List<CombatEvent> children = CombatUtil.ParseEffects(info);
            if (children != null)
            {
                m_children = children;
            }

            int len = m_children.Count;
            for(int i = 0; i < len; i++)
            {
                CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
                if (effect.Timing == eCombatEventTiming.AUTO)
                {
                    effect.Timing = eCombatEventTiming.ON_START;
                }

                if (effect.Sender == null)
                {
                    effect.Sender = new CombatantIndex(effect.Target.TeamIndex, effect.Target.IndexOnTeam);
                }
            }

            return true;
        }

        private List<CombatantIndex> cachedInvolved = null;
        public override List<CombatantIndex> GetInvolved()
        {
            if (cachedInvolved != null)
            {
                return cachedInvolved;
            }

            List<CombatantIndex> list = new List<CombatantIndex>();

            int len = m_children.Count;
            for (int i = 0; i < len; i++)
            {
                CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
                list.AddRange(effect.GetInvolved());
            }

            list.Sort();
            for (int i = list.Count - 1; i > 0; --i)
            {
                if (list[i].Equals(list[i - 1]))
                {
                    list.RemoveAt(i);
                }
            }

            cachedInvolved = list;
            return list;
        }
    }
}