using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Data
{
    public class ImpactTemplate
    {
        public ImpactTemplate()
        {

        }
        public int ImpactId { get; set; }
        public int LogicId { get; set; }
        public float[] Param;
        public float[] Upgrade;

    }

    public class ImpactTemplateManager
    {
        public static ImpactTemplateManager s_instance;
        private Dictionary<int, ImpactTemplate> m_impactDataDictionary = new Dictionary<int, ImpactTemplate>();

        public static ImpactTemplateManager Instance
        {
            get { return s_instance = s_instance ?? new ImpactTemplateManager(); }
        }

        private ImpactTemplateManager()
        {

        }

        public ImpactTemplate GetTemplate(int impact_id)
        {
            if (m_impactDataDictionary.ContainsKey(impact_id))
            {
                return m_impactDataDictionary[impact_id];
            }

            EB.Debug.LogWarning("GetTemplate: impact not found, id = {0}", impact_id);
            return null;
        }

        public static void ClearUp()
        {
            if (s_instance != null)
            {
                s_instance.m_impactDataDictionary.Clear();
            }
        }

        public bool InitTemplateFromCache(GM.DataCache.Combat combat)
        {
            if (combat == null)
            {
                EB.Debug.LogError("can not find impacts data");
                return false;
            }

            m_impactDataDictionary.Clear();
            var conditionSet = combat.GetArray(0);
            for (int i = 0; i < conditionSet.ImpactsLength; ++i)
            {
                var impact = conditionSet.GetImpacts(i);
                var tpl = ParseTemplate(impact);
                m_impactDataDictionary[tpl.ImpactId] = tpl;
            }

            return true;
        }

        private ImpactTemplate ParseTemplate(GM.DataCache.ImpactsTemplate impact)
        {
            ImpactTemplate impact_data = new ImpactTemplate();

            impact_data.ImpactId = impact.ImpactId;
            impact_data.LogicId = impact.LogicId;

            impact_data.Param = new float[] { impact.Param1, impact.Param2, impact.Param3, impact.Param4, impact.Param5 };

            impact_data.Upgrade = new float[] { impact.Upgrade1, impact.Upgrade2, impact.Upgrade3, impact.Upgrade4, impact.Upgrade5 };



            return impact_data;
        }
    }
}