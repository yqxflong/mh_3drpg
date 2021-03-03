using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class FightingHeadBarHud : HeadBarHud
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Obj = t.gameObject;
            m_SetSortingOrder = t.GetComponentEx<SetSortingOrder>();
            mHeadBarHUDMonitor.m_Offset = new Vector2(0, 40);
        }

        public GameObject m_Obj;
        public SetSortingOrder m_SetSortingOrder;
        private ParticleSystem m_Fx = null;
        private UIPanel m_Panel;

        public override void SetBarState(Hashtable data, bool state)
        {
            ///设置数据
            if (!state)
            {
                RecycleFx();
                m_Obj.SetActive(false);
            }
            else
            {
                m_Obj.SetActive(true);
                ProduceFx();
            }
        }

        void ProduceFx()
        {
            if (m_Fx == null)
            {
                //m_Fx = PSPoolManager.Instance.Use("fx_p_UI_yunbiao_jian");
            }

            if (m_Fx != null)
            {
                m_Fx.transform.parent = mDMono.transform;
                m_Fx.transform.localPosition = Vector3.zero;
                m_Fx.transform.localScale = Vector3.one;
                NGUITools.SetLayer(m_Fx.gameObject, mDMono.transform.parent.gameObject.layer);

                if (m_Panel == null)
                {
                    m_Panel = mDMono.transform.GetComponentInParent<UIPanel>();
                }

                if (m_Panel != null)
                {
                    m_SetSortingOrder.SetLayer(m_Panel.sortingOrder);
                }
                else
                {
                    EB.Debug.LogError("Can Not Found Panel in Parent");
                }

                m_Fx.EnableEmission(true);
                m_Fx.Simulate(0.0001f, true, true);
                m_Fx.Play(true);
            }
        }

        void RecycleFx()
        {
            if (m_Fx != null)
            {
                m_Fx.name = m_Fx.name.Replace("(Clone)", "");
                PSPoolManager.Instance.Recycle(m_Fx);
                m_Fx = null;
            }
        }
    }
}
