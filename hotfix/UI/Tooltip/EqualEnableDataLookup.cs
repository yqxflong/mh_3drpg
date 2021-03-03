using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class EqualEnableDataLookup : DataLookupHotfix
    {
        public string m_Value;
        // Use this for initialization
        public List<GameObject> m_EnableList = new List<GameObject>();
        public List<GameObject> m_DisableList = new List<GameObject>();

        public override void Awake()
        {
            base.Awake();
            m_Value = "enchant.socket";
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);

            if (dataID != null)
            {
                string tmp = mDL.GetDefaultLookupData<string>();

                if (m_Value.Equals(tmp))
                {
                    Enable();
                }
                else
                {
                    Disable();
                }
            }
        }

        public void Enable()
        {
            for (int i = 0; i < m_EnableList.Count; i++)
            {
                m_EnableList[i].SetActive(true);
            }

            for (int i = 0; i < m_DisableList.Count; i++)
            {
                m_DisableList[i].SetActive(false);
            }
        }

        public void Disable()
        {
            for (int i = 0; i < m_EnableList.Count; i++)
            {
                m_EnableList[i].SetActive(false);
            }

            for (int i = 0; i < m_DisableList.Count; i++)
            {
                m_DisableList[i].SetActive(true);
            }
        }

        public bool GetIsEqual()
        {
            string tmp = mDL.GetDefaultLookupData<string>();
            return m_Value.Equals(tmp);
        }
    }
}
