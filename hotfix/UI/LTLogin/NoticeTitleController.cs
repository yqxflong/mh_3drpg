

namespace Hotfix_LT.UI
{
    public class NoticeTitleController : DynamicMonoHotfix
    {
        public const string SINGLE_NUM_SPRITE = "Ty_Mail_Di2";
        public const string DOUBLE_NUM_SPRITE ="Ty_Mail_Di1";
        public const string SELECT_SPRITE ="Ty_Mail_Di3";
    
        private UISprite m_Sprite
        {
            get { return mDMono != null ? mDMono.transform.GetComponent<UISprite>() : null; }
        }
        private UILabel m_Title
        {
            get {return mDMono.transform.Find("LTFontOutline8_Big").GetComponent<UILabel>(); }
        }

        private NoticeUILogic m_NoticeUILogic
        {
            get
            {
                return mDMono.transform.parent.parent.parent.parent.parent.parent.parent
                    .GetUIControllerILRComponent<NoticeUILogic>();
            }
        }

        public override void Awake()
        {
            base.Awake();
            mDMono.transform.GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
                {
                    m_NoticeUILogic.OnClickTitleBtn(mDMono.gameObject);
                }));
        }

        private void SetSpriteState(string spriteName)
        {
            if (m_Sprite != null)
            {
                m_Sprite.spriteName = spriteName;
            }
        }

        public void SetText(string text)
        {
            mDMono.gameObject.SetActive(true);
            LTUIUtil.SetText(m_Title,text);
            SetSpriteStateByNum();
        }

        public void ClickTitleBtn()
        {
            SetSpriteState(SELECT_SPRITE);
        }

        public void SetSpriteStateByNum()
        {
            if (mDMono.transform.GetSiblingIndex()%2==0)
            {
                SetSpriteState(SINGLE_NUM_SPRITE);
            }
            else
            {
                SetSpriteState(DOUBLE_NUM_SPRITE);
            }
        }
	
    }
}