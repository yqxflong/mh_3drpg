namespace Hotfix_LT.UI
{
    public class GuideNodeMonologView : DynamicMonoHotfix
    {
        public UILabel tipsLabel;
        public UISprite iconSpt;
        public UISprite bgSpt;
    
        //public float _primalBgSptWidth;
        //public float _primalBgSptHeight;
        //public float _primalTipsLabelWidth;
        //public float _primalTipsLabelHeight;
    
        private int _dBgTipeWidth =0;
        private int _dBgTipeHeight=0;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            tipsLabel = t.GetComponent<UILabel>("MonologLabel");
            iconSpt = t.GetComponent<UISprite>("Sprite");
            bgSpt = t.GetComponent<UISprite>("BgSprite");
        }

        public void SetLabel(string content,int width)
        {
            if(_dBgTipeWidth==0&& _dBgTipeHeight==0)
            {
                _dBgTipeWidth = bgSpt.width - tipsLabel.fontSize;
                _dBgTipeHeight = bgSpt.height - tipsLabel.fontSize;
            }
            tipsLabel.width = width;
            tipsLabel.text = content;
            bgSpt.width = width + _dBgTipeWidth;
            bgSpt.height = tipsLabel.height + _dBgTipeHeight;
        }
    
        public void SetIcon(bool isShow)
        {
            iconSpt.gameObject.SetActive(isShow);
        }
    }
}
