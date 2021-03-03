using UnityEngine;


namespace Hotfix_LT.UI
{
    public class OpenGuideStep
    {
        protected int m_GuideId;
        public int GuideId
        {
            set
            {
                m_GuideId = value;
            }
        }
        public GameObject m_StepObj;
        public GameObject m_Panel;
        public System.Action onFinish;

        public void RemoveDelegate()
        {
            if (m_StepObj != null)
            {
                if (m_StepObj.GetComponent<UIEventTrigger>() != null)
                {
                    m_StepObj.GetComponent<UIEventTrigger>().onPress.Remove(new EventDelegate(OnFinish));
                }
                if (m_StepObj.GetComponent<UIButton>() != null) m_StepObj.GetComponent<UIButton>().onClick.Remove(new EventDelegate(OnFinish));
                if (m_StepObj.GetComponent<UIToggle>() != null)
                {
                    m_StepObj.GetComponent<UIToggle>().onChange.Remove(new EventDelegate(OnFinish));
                }
            }
        }

        public virtual void ExcuteReal()
        {
            EB.Debug.Log("===============ExcuteReal{0}" , m_GuideId);
            GuideStepData stepdata;
            if (!GuideManager.Instance.GetStep(m_GuideId, out stepdata))
            {
                EB.Debug.LogError("ExcuteReal  cant get stepdata for {0}" , m_GuideId);
                return;
            }
            if (m_StepObj != null)
            {
                //MengBanController.Instance.SetMiddle(m_StepObj.GetComponent<UIWidget>(), stepdata.tips_anchor, stepdata.tips);
                UIPanel panel = null;
                if (m_Panel != null)
                {
                    panel = m_Panel.GetComponent<UIPanel>();

                }
                AddFx(m_StepObj.transform, panel);

                if (m_StepObj.GetComponent<UIButton>() != null)
                {
                    m_StepObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnFinish));
                }
                else if (m_StepObj.GetComponent<UIEventTrigger>() != null)
                {
                    m_StepObj.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(OnFinish));
                }
                else if (m_StepObj.GetComponent<UIToggle>() != null)
                {
                    m_StepObj.GetComponent<UIToggle>().onChange.Add(new EventDelegate(OnFinish));
                }
            }
        }

        public virtual void AddFx(Transform parent, UIPanel panel)
        {
            Transform fx_transform = parent.Find("GuidFx");
            if (null != fx_transform)
            {
                fx_transform.gameObject.SetActive(true);
                return;
            }

            GameObject fx_prefab = MengBanController.Instance.m_GuideClickPrefab;// Resources.Load<GameObject>("Prefabs/Guide/Guide_DianJi");
            GM.AssetUtils.FixShaderInEditor(fx_prefab);
            GameObject fx = GameObject.Instantiate(fx_prefab);
            if (panel == null)
            {
                panel = parent.GetComponentInParent<UIPanel>();
            }
            ParticleSystemUIComponent psuc = fx.GetComponent<ParticleSystemUIComponent>();
            psuc.panel = panel;
            fx.SetActive(true);
            fx.name = "GuidFx";
            fx.transform.parent = parent;
            fx.transform.localPosition = Vector3.zero;
            OpenGuideManager.Instance.AddFx(fx, this);
        }

        void RemoveFx(Transform parent)
        {
            if (parent == null) return;
            Transform fx_transform = parent.Find("GuidFx");
            if (null != fx_transform)
            {
                fx_transform.gameObject.SetActive(false);
                GameObject.Destroy(fx_transform.gameObject);
                return;
            }
        }

        public virtual void FinishReal()
        {
            RemoveFx(m_StepObj.transform);
        }

        public void OnFinish()
        {
            if (onFinish != null) onFinish();
            if (m_StepObj != null)
            {
                if (m_StepObj.GetComponent<UIEventTrigger>() != null)
                {
                    m_StepObj.GetComponent<UIEventTrigger>().onPress.Remove(new EventDelegate(OnFinish));
                }
                if (m_StepObj.GetComponent<UIButton>() != null) m_StepObj.GetComponent<UIButton>().onClick.Remove(new EventDelegate(OnFinish));
                if (m_StepObj.GetComponent<UIToggle>() != null)
                {
                    m_StepObj.GetComponent<UIToggle>().onChange.Remove(new EventDelegate(OnFinish));
                }
            }
            FinishReal();
            OpenGuideManager.Instance.FinishStep();
        }
    }
}