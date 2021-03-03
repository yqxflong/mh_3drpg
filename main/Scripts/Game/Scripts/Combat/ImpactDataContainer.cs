using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace Hotfix_LT.Combat
{

    public class ImpactData
    {
        public int ImpactId
        {
            get;
            set;
        }

        public int ImpactIndex
        {
            get;
            set;
        }

        public int TurnLeft
        {
            get;
            set;
        }

        public CombatantIndex Sender
        {
            get;
            set;
        }

        public int SkillId
        {
            get;
            set;
        }

        public bool Fadeout
        {
            get;
            set;
        }

        public int CurStack
        {
            get;
            set;
        }
    }


    public class ImpactDataContainer
    {
        private List<ImpactData> m_dataList = new List<ImpactData>();

        public ImpactDataContainer()
        {

        }

        public int GetImpactCount()
        {
            return m_dataList.Count;
        }

        public ImpactData GetImpactData(int index)
        {
            return m_dataList[index];
        }

        public List<ImpactData> GetDataList()
        {
            return m_dataList;
        }

        public void AddImpactData(ImpactData impact_data)
        {
            m_dataList.Add(impact_data);
        }

        public void RemoveImpactData(ImpactData impact_data)
        {
            m_dataList.Remove(impact_data);
        }

        public void RemoveImpactData(int impact_id)
        {
            ImpactData impact_data = FindImpactData(impact_id);
            if (impact_data != null)
            {
                RemoveImpactData(impact_data);
            }
        }

        public ImpactData FindImpactData(int impact_id)
        {
            return m_dataList.Find(impact_data => impact_data.ImpactId == impact_id);
        }

        public List<ImpactData> FindAllImpactData(int impact_id)
        {
            return m_dataList.FindAll(impact_data => impact_data.ImpactId == impact_id);
        }

        public int GetImpactCount(int impact_id)
        {
            return m_dataList.Count(impact_data => impact_data.ImpactId == impact_id);
        }

        public bool HasImpactData(int impact_id)
        {
            return m_dataList.Exists(impact_data => impact_data.ImpactId == impact_id);
        }

        public void Clear()
        {
            m_dataList.Clear();
        }
    }
}