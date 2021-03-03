using System.Collections;
namespace Hotfix_LT.Combat
{
    public class BreakEffectEvent : CombatEffectEvent
    {
        public BreakEffectEvent()
        {
            m_effect_type = eCombatEffectType.BREAK_MANUAL_SKILL;
        }
    }
}