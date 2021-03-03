using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatSenderHitEvent : GameEvent
    {
        public Combatant Sender
        {
            get;
            private set;
        }

        public MoveEditor.HitEventInfo HitEvent
        {
            get;
            private set;
        }

        public CombatSkillEvent SkillEvent
        {
            get;
            private set;
        }

        public float Weight
        {
            get;
            private set;
        }

        public CombatSenderHitEvent(Combatant sender, MoveEditor.HitEventInfo hit_event, CombatSkillEvent skill_event)
        {
            Sender = sender;
            HitEvent = hit_event;
            SkillEvent = skill_event;

            MoveEditor.PlayHitReactionProperties reaction_info = hit_event._hitRxnProps;
            Weight = reaction_info.totalWeight <= 0 ? 1.0f : (float)reaction_info.weight / (float)reaction_info.totalWeight;
        }
    }
}