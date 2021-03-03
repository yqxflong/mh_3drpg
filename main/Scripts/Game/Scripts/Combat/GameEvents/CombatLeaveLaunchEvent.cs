using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatLeaveLaunchEvent : GameEvent
    {
        public Combatant Target
        {
            get;
            set;
        }

        public CombatLeaveLaunchEvent(Combatant target)
        {
            Target = target;
        }
    }
}