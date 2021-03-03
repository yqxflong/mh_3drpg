using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class FloatingCombatPowerTextManager: DynamicMonoHotfix
    {
        public FloatingCombatPowerManager m_FloatingManager;
        public float m_DelayTime = 0.2f;
        public int m_DataQueueSize = 20;
        public int m_CoroutineNum = 0;//检测防止出现bug

        private EB.Collections.Queue<object> m_DataQueue;

        private static FloatingCombatPowerTextManager m_instance;
        public static FloatingCombatPowerTextManager Instance
        {
            get
            {
                if (null == m_instance)
                {
                    EB.Debug.LogError("FloatingUITextManager is not Init");
                }
                return m_instance;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_FloatingManager = t.GetMonoILRComponent<FloatingCombatPowerManager>("FloatingText");
            m_DelayTime = 0.75f;
            m_DataQueueSize = 20;
            m_CoroutineNum = 0;

            m_instance = this;
            m_DataQueue = new EB.Collections.Queue<object>(m_DataQueueSize);
        }

        public override void OnDestroy()
        {
            m_instance = null;
        }

        public static void ShowFloatingText(string content)
        {
            if (null != Instance) Instance.ShowFloatingUI(content);
        }

        public void ShowFloatingUI(object data)
        {
            m_DataQueue.Enqueue(data);
            if (!m_state)
            {
                m_CoroutineNum++;
                m_state = true;
                StartCoroutine(RealShow());
            }
        }

        private bool m_state = false;
        IEnumerator RealShow()
        {
            yield return new WaitForEndOfFrame();
            while (m_state)
            {
                if (m_DataQueue.Count == 0)
                {
                    m_state = false;
                    m_CoroutineNum--;
                    yield break;
                }
                else
                {
                    object data = m_DataQueue.Dequeue();
                    m_FloatingManager.ShowFloatingUI(data);
                    yield return new WaitForSeconds(m_DelayTime);
                }
            }
            yield break;
        }

        public static void ClearQueue()
        {
            if (null != Instance) Instance.ClearQueueReal();
        }

        public void ClearQueueReal()
        {
            m_DataQueue.Clear();
        }
    }
}
