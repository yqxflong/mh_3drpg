using System.Collections;
namespace Hotfix_LT.Combat
{
    public class DamageEffectEvent : CombatEffectEvent
    {
        public enum eDamageType
        {
            None,
            Poisoning,
            Bleeding,
            Fire
        }

        private int m_damage = -1;
        private int m_show = -1;
        private bool m_critical = false;
        private eDamageType m_damageType = eDamageType.None;

        public int Damage
        {
            set { m_damage = value; }
            get { return m_damage; }
        }

        public int Show
        {
            set { m_show = value; }
            get { return m_show; }
        }

        public bool Critical
        {
            set { m_critical = value; }
            get { return m_critical; }
        }

        public eDamageType DamageType
        {
            get { return m_damageType; }
        }

        public DamageEffectEvent()
        {
            m_effect_type = eCombatEffectType.DAMAGE;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            m_damage = EB.Dot.Integer("damage", info, -1);
            if (m_damage < 0)
            {
                EB.Debug.LogWarning("DamageEffectEvent.Parse: damage is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            m_show = EB.Dot.Integer("show", info, -1);
            if (m_show < 0)
            {
                EB.Debug.LogWarning("DamageEffectEvent.Parse: show is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            // optional
            m_critical = EB.Dot.Bool("critical", info, false);

            string damage_type = EB.Dot.String("damageType", info, "");
            switch (damage_type)
            {
                case "poisoning":
                    m_damageType = eDamageType.Poisoning;
                    break;
                case "bleeding":
                    m_damageType = eDamageType.Bleeding;
                    break;
                case "fire":
                    m_damageType = eDamageType.Fire;
                    break;
            }

            return true;
        }
    }
}