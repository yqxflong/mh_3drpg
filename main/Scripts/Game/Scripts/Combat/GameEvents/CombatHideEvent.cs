using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class CombatHideEvent : GameEvent
    {
        public List<GameObject> HideObjects
        {
            get;
            set;
        }

        public CombatHideEvent(List<GameObject> hide_objects)
        {
            HideObjects = hide_objects;
        }

    }
}