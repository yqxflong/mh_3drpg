using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatRemoveCombatantEvent : GameEvent
    {
        public Combatant Combatant
        {
            get;
            set;
        }

        public CombatRemoveCombatantEvent(Combatant combatant)
        {
            Combatant = combatant;
        }
    }
}