using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatHealEvent : GameEvent
    {
        public GameObject Target
        {
            get;
            set;
        }

        public int Heal
        {
            get;
            set;
        }

        public int ShowHeal
        {
            get;
            set;
        }

        public HealEffectEvent.eHealType HealType
        {
            get;
            set;
        }

        public CombatHealEvent(GameObject target, int heal, int show_heal, HealEffectEvent.eHealType heal_type)
        {
            Target = target;
            Heal = heal;
            ShowHeal = show_heal;
            HealType = heal_type;
        }
    }
}