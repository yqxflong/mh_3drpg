using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class AddCombatantEffectEvent : CombatEffectEvent
    {
        public CombatantData Data
        {
            get;
            private set;
        }

        public CombatantAttributes Attributes
        {
            get;
            private set;
        }

        public AddCombatantEffectEvent()
        {
            m_effect_type = eCombatEffectType.Add_COMBATANT;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            Data = CombatUtil.ParseCombatantData(info["data"] as Hashtable, null);
            Attributes = CombatUtil.ParseCombatantAttributes(info["status"] as Hashtable, null);

            return true;
        }
    }
}