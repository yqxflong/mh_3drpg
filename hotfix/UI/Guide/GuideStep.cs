//GuideStep
//新手引导Step
//Johny

using UnityEngine;

namespace Hotfix_LT.UI
{
    public class GuideStep
    {

        protected int m_GuideId;
        protected bool m_IsTrigger;
        public int GuideId
        {
            set
            {
                m_GuideId = value;
            }
            get
            {
                return m_GuideId;
            }
        }

        public GameObject m_StepObj;

        public System.Action onFinish;

        public void Excute()
        {
            if (!m_IsTrigger) ExcuteReal();
        }

        public virtual void ExcuteReal()
        {
            EB.Debug.Log("===============ExcuteReal{0}" , m_GuideId);
            GuideStepData stepdata;
            if (!GuideManager.Instance.GetStep(m_GuideId, out stepdata))
            {
                EB.Debug.LogError("ExcuteReal  cant get stepdata for {1}" , m_GuideId);
                return;
            }
            if (m_StepObj != null)
            {
                MengBanController.Instance.SetMiddle(m_StepObj.GetComponent<UIWidget>(), stepdata.tips_anchor, stepdata.tips);
                if (m_StepObj.GetComponent<UIButton>() != null)
                {
                    m_StepObj.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnFinish));
                }
                else if (m_StepObj.GetComponent<UIEventTrigger>() != null)
                {
                    m_StepObj.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(OnFinish));
                }
            }
        }

        /// <summary>
        ///  Only state set  cant too manny logic
        /// </summary>
        public virtual void FinishReal()
        {
            if (m_StepObj != null)
            {
                if (m_StepObj.GetComponent<UIEventTrigger>() != null)
                {
                    m_StepObj.GetComponent<UIEventTrigger>().onPress.Remove(new EventDelegate(OnFinish));
                }
                if (m_StepObj.GetComponent<UIButton>() != null) m_StepObj.GetComponent<UIButton>().onClick.Remove(new EventDelegate(OnFinish));
            }
        }

        public void OnFinish()
        {
            if (onFinish != null) onFinish();
            FinishReal();
            GuideManager.Instance.FinishStep();
        }

        public void TriggerExcute()
        {
            ExcuteReal();
        }
    }
}