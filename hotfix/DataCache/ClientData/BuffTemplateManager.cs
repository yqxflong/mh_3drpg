using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Data
{


    public enum BuffType
    {
        Friend = 0,
        Enemy = 1,
        Indicator = 2,
        Special = 3,
    }

    public class BuffTemplate
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public BuffType Type { get; set; }
        // public bool Invisible { get { return Type == BuffType.Special || Type == BuffType.Indicator; } }
        public bool Invisible  { get; set; }
        public bool SpecialStateInvisible { get { return Type == BuffType.Indicator; } }//为了让Special类型的buff也能显示SpecialState的buff特效（冰冻、石化）
        public bool FXInvisible { get { return Type == BuffType.Indicator; } }//为了让Special类型的buff也能显示特效
        public string Description { get; set; }
        public int LifeTurn { get; set; }
        public int StackNum { get; set; }
        public bool IsActiveImmediately { get { return LifeTurn == 0; } }
        public bool IsForever { get { return LifeTurn < 0; } }
        public string Buff { get; set; }
        public int SpecialState { get; set; }
        public List<int> BuffFloatFont { get; set; }
        public string OnceFX { get; set; }
        public MoveEditor.BodyPart OnceFXAttachment { get; set; }
        public string OnceFXEventName { get { return OnceFX + ID.ToString(); } }
        public string LoopFX { get; set; }
        public MoveEditor.BodyPart LoopFXAttachment { get; set; }
        public string LoopFXEventName { get { return "_ImpactLoopFlag_" + LoopFX + ID.ToString(); } }
    }

    public class BuffTemplateManager
    {
        public static BuffTemplateManager s_instance;
        private Dictionary<int, BuffTemplate> m_impactDataDictionary = new Dictionary<int, BuffTemplate>();

        public static BuffTemplateManager Instance
        {
            get { return s_instance = s_instance ?? new BuffTemplateManager(); }
        }

        private BuffTemplateManager()
        {

        }

        public BuffTemplate GetTemplate(int impact_id)
        {
            if (m_impactDataDictionary.ContainsKey(impact_id))
            {
                return m_impactDataDictionary[impact_id];
            }

            EB.Debug.LogWarning("GetTemplate: impact not found, id = {0}", impact_id);
            return null;
        }

        public Hashtable GetBufferInfo(int id)
        {
            BuffTemplate data = GetTemplate(id);
            return new Hashtable() { 
                { "fxinvisible", data.FXInvisible }, { "stacknum", data.StackNum}, 
                { "isActiveImmediately", data.IsActiveImmediately }, { "OnceFX", data.OnceFX },
                { "OnceFXEventName", data.OnceFXEventName }, { "OnceFXAttachment", (int)data.OnceFXAttachment },
                { "LoopFX", data.LoopFX }, { "LoopFXEventName", data.LoopFXEventName },
                { "LoopFXAttachment", (int)data.LoopFXAttachment }, { "Name", data.Name }
            };
        }
        public Hashtable GetBufferInfoEx(int id)
        {
            BuffTemplate data = GetTemplate(id);
            return new Hashtable() {
                { "Name", data.Name }, { "BuffFloatFont", data.BuffFloatFont },
                { "SpecialStateInvisible", data.SpecialStateInvisible }, { "SpecialState", data.SpecialState },
            };
        }

        public string GetBufferName(int id)
        {
            var skill = GetTemplate(id);
            return skill != null ? skill.Name : "UnknownBuff";
        }
        public int GetSpecialState(int id)
        {
            BuffTemplate data = GetTemplate(id);
            return data.SpecialState;
        }
        public int GetBufferState(int impact_id)
        {
            BuffTemplate data = GetTemplate(impact_id);
            return data.SpecialState;
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
            for (int i = 0; i < conditionSet.BuffsLength; ++i)
            {
                var impact = conditionSet.GetBuffs(i);
                var tpl = ParseTemplate(impact);
                m_impactDataDictionary[tpl.ID] = tpl;
            }

            return true;
        }

        private BuffTemplate ParseTemplate(GM.DataCache.BuffTemplate impact)
        {
            BuffTemplate impact_data = new BuffTemplate();

            impact_data.ID = impact.Id;
            impact_data.Name = EB.Localizer.GetTableString(string.Format("ID_combat_buffs_{0}_name", impact_data.ID), impact.Name);//impact.Name;
            impact_data.Type = (BuffType)impact.BuffType;
            impact_data.Invisible = impact.BuffShow == 0;
            // impact_data.Invisible = impact_data.Type == BuffType.Special || impact_data.Type == BuffType.Indicator;
            impact_data.Description = EB.Localizer.GetTableString(string.Format("ID_combat_buffs_{0}_description", impact_data.ID), impact.Description);// impact.Description;
            impact_data.LifeTurn = impact.EffectTurns;
            impact_data.StackNum = impact.StackNum;
            impact_data.Buff = impact.Buff;
            impact_data.SpecialState = impact.SpecialState;
            impact_data.BuffFloatFont = new List<int>();
            if (!string.IsNullOrEmpty(impact.BuffFloatFont))
            {
                string[] splitStr = impact.BuffFloatFont.Split(',');

                int len = splitStr.Length;
                for (int i = 0; i < len; i++)
                {
                    string data = splitStr[i];
                    int dataIndex = int.Parse(data);
                    impact_data.BuffFloatFont.Add(dataIndex);
                }
            }
            impact_data.OnceFX = impact.OnceFX;
            impact_data.OnceFXAttachment = (MoveEditor.BodyPart)impact.OnceFXAttachment;
            impact_data.LoopFX = impact.LoopFX;
            impact_data.LoopFXAttachment = (MoveEditor.BodyPart)impact.LoopFXAttachment;
            return impact_data;
        }
    }
}