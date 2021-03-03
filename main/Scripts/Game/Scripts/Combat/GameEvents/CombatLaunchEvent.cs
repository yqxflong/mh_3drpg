using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatLaunchEvent : GameEvent
    {
        public Combatant Target
        {
            get;
            set;
        }

        public CombatLaunchEvent(Combatant target)
        {
            Target = target;
        }
    }
}