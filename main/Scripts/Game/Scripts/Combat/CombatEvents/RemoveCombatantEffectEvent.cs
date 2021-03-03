using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class RemoveCombatantEffectEvent : CombatEffectEvent
    {
        public CombatantIndex RemoveIndex
        {
            get;
            set;
        }

        public RemoveCombatantEffectEvent()
        {
            m_effect_type = eCombatEffectType.REMOVE_COMBATANT;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            RemoveIndex = CombatantIndex.Parse(info["removeIndex"] as Hashtable);
            if (RemoveIndex == null)
            {
                EB.Debug.LogWarning("RemoveCombatantEffectEvent.Parse: removeIndex is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            return true;
        }
    }
}