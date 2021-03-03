using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatQuitEvent : CombatEvent
    {
        public CombatQuitEvent()
        {
            m_type = eCombatEventType.QUIT;
            m_timing = eCombatEventTiming.AUTO;
        }
    }
}