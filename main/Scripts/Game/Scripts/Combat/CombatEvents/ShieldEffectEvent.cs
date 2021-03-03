using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ShieldEffectEvent : CombatEffectEvent
    {
        protected int m_shield = -1;

        public int Shield
        {
            get { return m_shield; }
        }

        public ShieldEffectEvent()
        {
            m_effect_type = eCombatEffectType.SHIELD;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            m_shield = EB.Dot.Integer("shield", info, 0);

            return true;
        }
    }
}