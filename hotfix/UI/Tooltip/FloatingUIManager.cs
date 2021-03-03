using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class FloatingUIManager : DynamicMonoHotfix
    {
        public List<FloatingUIItem> m_Queue;
        public FloatingUIItem m_Template;
        public GameObject m_Container;
        private int m_CurrentFloatingNum;
        public int m_DataQueueSize = 20;
        public int m_PanelSortingOrder = 30000;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Template = t.GetMonoILRComponent<FloatingUIItem>("UI_Piaozi_Prefab");
            m_Container = t.gameObject;
            m_DataQueueSize = 20;
            m_PanelSortingOrder = 30000;
        }

        public override void Start()
        {
            m_CurrentFloatingNum = 0;
            m_Queue = new List<FloatingUIItem>();
            UIPanel panel = mDMono.transform.GetComponentEx<UIPanel>();

            if (panel != null)
            {
                panel.sortingOrder = m_PanelSortingOrder;
            }

            mDMono.gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            if (m_Queue != null)
            {
                m_Queue.Clear();
            }
        }

        public void ShowFloatingUI(object data)
        {
            FloatingUIItem item = GetFloatingItem();

            if (null != item)
            {
                m_CurrentFloatingNum++;

                if (m_CurrentFloatingNum == 1)
                {
                    mDMono.gameObject.SetActive(true);
                }

                item.Play(data);
            }
        }

        public FloatingUIItem GetFloatingItem()
        {
            if (m_Queue.Count == 0)
            {
                //新建一个出来加入进去
                var t = Object.Instantiate(m_Template.mDMono).transform;
                t.parent = m_Container.transform;
                t.localScale = m_Template.mDMono.transform.localScale;
                FloatingUIItem obj = t.GetMonoILRComponent<FloatingUIItem>();
                obj.mDMono.gameObject.SetActive(true);
                return obj;
            }
            else
            {
                FloatingUIItem obj = m_Queue[m_Queue.Count - 1];
                m_Queue.Remove(obj);
                return obj;
            }
            //return null;
        }

        public void Recyle(FloatingUIItem obj)
        {
            m_CurrentFloatingNum--;

            if (m_CurrentFloatingNum == 0)
            {
                mDMono.gameObject.SetActive(false);
            }

            obj.mDMono.gameObject.SetActive(false);
            m_Queue.Add(obj);
        }
    }
}
