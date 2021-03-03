using UnityEngine;
using System.Collections;
namespace Hotfix_LT.Combat
{
    public class CombatShowSkillPanelEvent : GameEvent
    {
        public bool IsSkillPanelShown
        {
            get;
            private set;
        }

        public CombatShowSkillPanelEvent(bool isShown)
        {
            IsSkillPanelShown = isShown;
        }

        public void ShowSkillPanel(bool isShown)
        {
            IsSkillPanelShown = isShown;
        }
    }
}