using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatSkillActionEvent : GameEvent
    {
        public Combatant Sender
        {
            get;
            private set;
        }

        //public Hotfix_LT.Data.SkillTemplate SkillData
        //{
        //    get;
        //    private set;
        //}

        public MoveEditor.Move SkillMove
        {
            get;
            set;
        }

        public bool TriggerCombo
        {
            get;
            private set;
        }

        public CombatSkillEvent SkillEvent
        {
            get;
            set;
        }

        //public CombatSkillActionEvent(Combatant sender, Hotfix_LT.Data.SkillTemplate skill_data, bool trigger_combo)
        //{
        //    Sender = sender;
        //    SkillData = skill_data;
        //    TriggerCombo = trigger_combo;
        //}
    }
}