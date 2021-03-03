using UnityEngine;
using System.Collections;

namespace Hotfix_LT.Combat
{
    public class CombatFloatFontEvent : GameEvent
    {
        public string Font { get; set; }
        public int Type { get; set; }
        public Vector3 Offset { get; set; }
        public Combatant Target { get; set; }

        public CombatFloatFontEvent(Combatant target, string font, int type, Vector3 offset)
        {
            Font = font;
            Type = type;
            Offset = offset;
            Target = target;
        }
    }
}