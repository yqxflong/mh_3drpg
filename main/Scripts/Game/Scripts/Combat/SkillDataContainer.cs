using System.Collections.Generic;
using System.Linq;
namespace Hotfix_LT.Combat
{
    public class SkillData
    {
        public int SkillId
        {
            get;
            set;
        }

        public CombatantIndex Index
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        }

        public int CoolDown
        {
            get;
            set;
        }

        public bool DeathStatus
        {
            get;
            set;
        }

        public bool EffectStatus
        {
            get;
            set;
        }

        public List<int> Required
        {
            get;
            set;
        }

        public SkillData()
        {
            Required = new List<int>();
        }
    }

    public class SkillDataContainer
    {
        private List<SkillData> m_dataList = new List<SkillData>();

        public SkillDataContainer()
        {

        }

        public void AddSkillData(SkillData skill_data)
        {
            m_dataList.Add(skill_data);
        }

        public void RemoveSkillData(SkillData skill_data)
        {
            m_dataList.Remove(skill_data);
        }

        public SkillData FindSkillData(int skill_id)
        {
            return m_dataList.Where(m => m.SkillId == skill_id).FirstOrDefault();
        }

        public SkillData FindSkillData(CombatantIndex idx)
        {
            return m_dataList.Where(m => m.Index.Equals(idx)).FirstOrDefault();
        }

        public bool HasSkillData(int skill_id)
        {
            return false;
        }

        public int GetSkillDataCount()
        {
            return m_dataList.Count;
        }

        public SkillData GetSkillData(int idx)
        {
            if (idx >= 0 && idx < m_dataList.Count)
            {
                return m_dataList[idx];
            }
            else
            {
                return null;
            }
        }

        public void Clear()
        {
            m_dataList.Clear();
        }
    }

    public enum EnvironmentDisappearType
	{
	    Invalid = -1,
	    None = 0,
	    Fade = 1,
	    Black = 2
    }
}