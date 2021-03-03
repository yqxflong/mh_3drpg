using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTChargeSelectPayWayUI : UIControllerHotfix
    {
        public override bool ShowUIBlocker { get { return true; } }
    
        private System.Action mCallback;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("UINormalFrame/BGs/Top/CloseBtn");

            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/AliPay").clickEvent.Add(new EventDelegate(OnAlipayBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/WeChat").clickEvent.Add(new EventDelegate(OnWechatBtnClick));
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
    
            mCallback = null;
            mCallback = param as System.Action;
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            hasSelect = false;
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            if (mCallback != null && hasSelect)
            {
                mCallback();
            }
            DestroySelf();
            yield break;
        }
    
        private bool hasSelect;
        public void OnAlipayBtnClick()
        {
            if (hasSelect) return;
            hasSelect = true;
    
            var mng = SparxHub.Instance.GetManager<EB.Sparx.WalletManager>();
            mng.ChangeProvider("alipay");
    
            OnCancelButtonClick();
        }
    
        public void OnWechatBtnClick()
        {
            if (hasSelect) return;
            hasSelect = true;
    
            var mng = SparxHub.Instance.GetManager<EB.Sparx.WalletManager>();
            mng.ChangeProvider("wechat");
    
            OnCancelButtonClick();
        }
    
    }
}
