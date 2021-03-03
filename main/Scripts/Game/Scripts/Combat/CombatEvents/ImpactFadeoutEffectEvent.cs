using System.Collections;
namespace Hotfix_LT.Combat
{
    public class ImpactFadeoutEffectEvent : CombatEffectEvent
    {
        public int ImpactId
        {
            get { return ImpactData.ImpactId; }
        }

        public ImpactData ImpactData
        {
            get;
            set;
        }

        public CombatantAttributes Attributes
        {
            get;
            private set;
        }

        public ImpactFadeoutEffectEvent()
        {
            m_effect_type = eCombatEffectType.IMPACT_FADEOUT;
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