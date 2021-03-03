using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ExileEffectEvent : CombatEffectEvent
    {
        public ExileEffectEvent()
        {
            m_effect_type = eCombatEffectType.EXILE;
        }
    }
}