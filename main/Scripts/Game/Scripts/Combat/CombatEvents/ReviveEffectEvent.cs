using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ReviveEffectEvent : CombatEffectEvent
    {
        public ReviveEffectEvent()
        {
            m_effect_type = eCombatEffectType.REVIVE;
        }
    }
}