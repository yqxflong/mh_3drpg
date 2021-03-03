namespace Hotfix_LT.UI
{
    public class FloatingCombatPowerItem : DynamicMonoHotfix
    {
        public FloatingCombatPowerManager m_Manager;
        public UILabel m_ContextLabel;
        public UILabel m_AddtextLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Manager = t.parent.GetMonoILRComponent<FloatingCombatPowerManager>();
            m_ContextLabel = t.GetComponent<UILabel>("Base");
            m_AddtextLabel = t.GetComponent<UILabel>("Add");

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
                string[] args=  (data as string).Split(',');
                m_ContextLabel.text = string.Format("{0}", args[0]);
                if (int.Parse(args[1]) >= 0)
                {
                    m_AddtextLabel.gradientTop = new UnityEngine.Color(0.5f, 1f, 0.87f);
                    m_AddtextLabel.gradientBottom = new UnityEngine.Color(0.145f, 1f, 0.337f);
                }
                else
                {
                    m_AddtextLabel.gradientTop = new UnityEngine.Color(1f, 0.41f, 0.41f);
                    m_AddtextLabel.gradientBottom = new UnityEngine.Color(0.99f, 0.07f, 0.274f);
                }
                m_AddtextLabel.text=string.Format("{0}", args[1]);
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