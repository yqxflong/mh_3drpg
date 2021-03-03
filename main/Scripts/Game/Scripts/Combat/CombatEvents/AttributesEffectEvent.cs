using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class AttributesEffectEvent : CombatEffectEvent
    {
        public CombatantAttributes Attributes
        {
            get;
            private set;
        }

        public AttributesEffectEvent()
        {
            m_effect_type = eCombatEffectType.ATTRIBUTES;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            Attributes = CombatUtil.ParseCombatantAttributes(info, null);

            return true;
        }
    }
}