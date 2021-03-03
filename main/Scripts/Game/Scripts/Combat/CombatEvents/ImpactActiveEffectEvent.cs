using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ImpactActiveEffectEvent : CombatEffectEvent
    {
        public int ImpactId
        {
            get { return ImpactData.ImpactId; }
        }

        public ImpactData ImpactData
        {
            get;
            private set;
        }

        public ImpactActiveEffectEvent()
        {
            m_effect_type = eCombatEffectType.IMPACT_ACTIVE;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            ImpactData = CombatUtil.ParseImpactData(info, null);

            return true;
        }
    }
}