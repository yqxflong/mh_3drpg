using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTActivityNoticeController : UIControllerHotfix
    {
        private UIButton confirmBtn;
        private UITexture bgTexture;
    
        private readonly Stack<object> noticeStack = new Stack<object>();

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            confirmBtn = t.GetComponent<UIButton>("Board/ConfirmBtn");
            controller.backButton = t.GetComponent<UIButton>("Board/CloseBtn");
            bgTexture = t.GetComponent<UITexture>("Board/BgTexture");
            confirmBtn.onClick.Add(new EventDelegate(ConfirmClick));
        }

        public override void OnDestroy()
        {
            if (confirmBtn!=null)
            {
                confirmBtn.onClick.Clear();
            }
            base.OnDestroy();
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            var list = param as List<object>;
            if (list!=null)
            {
                noticeStack.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    noticeStack.Push(list[i]);
                }
            }
            ShowNotice(noticeStack.Pop());
        }
    
        public void ConfirmClick()
        {
            if (initBG) return;
            if (noticeStack.Count>0)
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                StartBootFlash();
                ShowNotice(noticeStack.Pop());
            }
            else
            {
                OnCancelButtonClick();
            }
        }
    
        private bool initBG = false;
        private void ShowNotice(object data)
        {
            initBG = true;
            var bgUrl = EB.Dot.String("notify_bg", data, "");
            if (!string.IsNullOrEmpty(bgUrl))
            {
                GlobalMenuManager.Instance.LoadRemoteUITexture(bgUrl, bgTexture);
            }
            initBG = false;
        }
    }
}
