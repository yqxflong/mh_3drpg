using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using DG.Tweening;

namespace Hotfix_LT.UI
{
    public class UIBroadCastMessageController : DynamicMonoHotfix, IHotfixUpdate
    {
        private static UIBroadCastMessageController m_Instance;
        public static UIBroadCastMessageController Instance
        {
            get { return m_Instance; }
        }

        public UIPanel m_Panel;
        public int m_PanelSortingOrder = 30000;
        public int Length = 850;
        public int QueueSize = 50;
        //public int FontSize = 42;
        public int StartX = 250;
        //public float RollingTime = 5.0f;
        public float RollingSpeed = 200f;
        public UILabel m_ContextLabel;
        //消息队列
        private EB.Collections.Queue<string> m_MessageQueue = new EB.Collections.Queue<string>();
        private bool m_IsPlaying = false;
        public int MessageCount { get { return m_MessageQueue.Count; } }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Panel = t.GetComponent<UIPanel>("Panel");
            m_PanelSortingOrder = 30000;
            Length = 850;
            QueueSize = 50;
            StartX = 400;
            RollingSpeed = 200f;
            m_ContextLabel = t.GetComponent<UILabel>("Panel/Panel/Label");

            m_Instance = this;
            t.gameObject.SetActive(false);
            m_Panel.sortingOrder = m_PanelSortingOrder;
        }

        public static void PutOneMessageFromILR(string message)
        {
            if (Instance != null)
            {
                Instance.PutOneMessage(message);
            }
        }

        public void PutOneMessage(string message)
        {
            if (m_MessageQueue.Count < QueueSize)
            {
                m_MessageQueue.Enqueue(message);
                PlayOneMessage();
            }
        }

        //一次播放一条
        public void PlayOneMessage()
        {
            //判定当前是否正在播放
            if (!m_IsPlaying)
            {
                //string sceneType = SceneLogicManager.getSceneType();
                //if (sceneType == null || SceneLogicManager.getSceneType()!="mainlands") return;
                //if (UIStack.Instance == null || UIStack.Instance.IsUIFullScreenOpened) return;
                //从队列里面取出一条，播放
                if (m_MessageQueue.Count > 0)
                {
                    m_IsPlaying = true;
                    m_ContextLabel.text = string.Empty;
                    mDMono.gameObject.SetActive(true);
                    StartCoroutine(Play(m_MessageQueue.Dequeue()));
                }
                else//说明没有需要播放的 就隐掉滚动条
                {
                    mDMono.gameObject.SetActive(false);
                }
            }
        }

        IEnumerator Play(string context)
        {
            //计算context长度 决定了滚动条要移动多少长度
            yield return new WaitForEndOfFrame();
            //int contextsize=context.ToCharArray().Length;
            //int movelength = contextsize * FontSize + length;

            m_ContextLabel.transform.localPosition = new Vector3(StartX, 0, 0);
            m_ContextLabel.text = context;
            int contextsize = m_ContextLabel.width;
            int movelength = contextsize + Length;
            float RollingTime = movelength / RollingSpeed;

			var option = m_ContextLabel.transform.DOLocalMoveX(StartX - 1 * movelength, RollingTime);
			option.SetEase( Ease.Linear );
			option.SetUpdate(true);
			option.onComplete = OnPlayEnd;
        }

        void OnPlayEnd()
        {
            m_IsPlaying = false;
            PlayOneMessage();//取下一条
        }

        public void ClearAllMessage()
        {
            m_IsPlaying = false;
            m_MessageQueue.Clear();
            //gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
            m_Instance = null;
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public void Update()
        {
            if (!m_IsPlaying && m_MessageQueue.Count >= 1)
            {
                PlayOneMessage();
            }
        }
    }
}
