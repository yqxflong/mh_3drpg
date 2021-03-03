using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class HealEffectEvent : CombatEffectEvent
    {
        public enum eHealType
        {
            None,
            Absorb,
            HpRecover,
        }

        private int m_heal = -1;
        private int m_show = -1;
        private eHealType m_healType = eHealType.None;

        public int Heal
        {
            set { m_heal = value; }
            get { return m_heal; }
        }

        public int Show
        {
            set { m_show = value; }
            get { return m_show; }
        }

        public eHealType HealType
        {
            get { return m_healType; }
        }

        public HealEffectEvent()
        {
            m_effect_type = eCombatEffectType.HEAL;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            m_heal = EB.Dot.Integer("heal", info, -1);
            if (m_heal < 0)
            {
                EB.Debug.LogWarning("HealEffectEvent.Parse: heal is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            m_show = EB.Dot.Integer("show", info, -1);
            if (m_show < 0)
            {
                EB.Debug.LogWarning("HealEffectEvent.Parse: show is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            string damage_type = EB.Dot.String("healType", info, "");
            switch (damage_type)
            {
                case "hp_recover":
                    m_healType = eHealType.HpRecover;
                    break;
                case "absorb":
                    m_healType = eHealType.Absorb;
                    break;
                default:
                    m_healType = eHealType.None;
                    break;
            }

            return true;
        }
    }
}