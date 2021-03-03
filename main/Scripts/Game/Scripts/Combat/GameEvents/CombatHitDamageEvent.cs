using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatHitDamageEvent : GameEvent
    {
        public GameObject TargetCombatant
        {
            get;
            private set;
        }

        public int Damage
        {
            get;
            private set;
        }

        public int ShowDamage
        {
            get;
            private set;
        }

        public int Shield
        {
            get;
            private set;
        }

        public bool IsCrit
        {
            get;
            private set;
        }

        public CombatHitDamageEvent(GameObject targetCombatant, int damage, int show_damage, int shield, bool isCrit)
        {
            TargetCombatant = targetCombatant;
            Damage = damage;
            ShowDamage = show_damage;
            Shield = shield;
            IsCrit = isCrit;
        }
    }
}