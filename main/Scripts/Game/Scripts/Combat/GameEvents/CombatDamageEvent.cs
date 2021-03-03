using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatDamageEvent : GameEvent
    {
        public GameObject Target
        {
            get;
            set;
        }

        public int Damage
        {
            get;
            set;
        }

        public int ShowDamage
        {
            get;
            set;
        }

        public DamageEffectEvent.eDamageType DamageType
        {
            get;
            set;
        }

        public CombatDamageEvent(GameObject target, int damage, int show_damage, DamageEffectEvent.eDamageType damage_type)
        {
            Target = target;
            Damage = damage;
            ShowDamage = show_damage;
            DamageType = damage_type;
        }
    }
}