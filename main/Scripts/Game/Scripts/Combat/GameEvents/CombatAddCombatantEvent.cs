using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatAddCombatantEvent : GameEvent
    {
        public Combatant Combatant
        {
            get;
            set;
        }

        public CombatAddCombatantEvent(Combatant combatant)
        {
            Combatant = combatant;
        }
    }
}