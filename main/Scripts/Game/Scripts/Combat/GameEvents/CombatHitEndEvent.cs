using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatHitEndEvent : GameEvent
    {
        public GameObject Target
        {
            get; private set;
        }

        public CombatSkillEvent SkillEvent
        {
            get; private set;
        }

        public CombatHitEndEvent(GameObject target, CombatSkillEvent skillEvent)
        {
            Target = target;
            SkillEvent = skillEvent;
        }
    }
}