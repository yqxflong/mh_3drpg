namespace Hotfix_LT.UI
{
    public class FloatingUIItem : DynamicMonoHotfix
    {
        public FloatingUIManager m_Manager;
        public UILabel m_ContextLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Manager = t.parent.GetMonoILRComponent<FloatingUIManager>();
            m_ContextLabel = t.GetComponent<UILabel>("Label");

            t.GetComponentEx<TweenAlpha>().onFinished.Add(new EventDelegate(OnTweenAlphaFinish));
            t.GetComponentEx<TweenPosition>().onFinished.Add(new EventDelegate(OnTweenPositionFinish));
        }

        public void OnTweenPositionFinish()
        {
            mDMono.transform.GetComponent<TweenPosition>().ResetToBeginning();
        }
    
        public void OnTweenAlphaFinish()
        {
            mDMono.transform.GetComponent<TweenAlpha>().ResetToBeginning();
            m_Manager.Recyle(this);
        }
    
        public void Play(object data)
        {
            if (data != null && data is string && m_ContextLabel != null)
            {
                m_ContextLabel.text = string.Format("{0}", data as string);
            }
           
            mDMono.gameObject.SetActive(true);
            Reset();
        }
    
        void Reset()
        {
            UITweener[] tweeners = mDMono.transform.GetComponents<UITweener>();

            if (tweeners == null)
            {
                return;
            }

            for(int i=0;i<tweeners.Length;i++)
            {
                tweeners[i].ResetToBeginning();
                tweeners[i].enabled = true;
            }
        }
    }
}
