namespace Hotfix_LT.UI
{
    public class LTGetGold : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("CloseBtn");

            t.GetComponent<UIButton>("Grid/MainFBBtn").onClick.Add(new EventDelegate(OnMainFBBtnClick));
            t.GetComponent<UIButton>("Grid/GoldFBBtn").onClick.Add(new EventDelegate(OnGoldFBBtnClick));
            t.GetComponent<UIButton>("Grid/OpenBuyGoldBtn").onClick.Add(new EventDelegate(OnOpenBuyGoldBtnClick));
        }
    
        public override bool ShowUIBlocker { get { return true; } }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
        }
    
        //public override IEnumerator OnRemoveFromStack()
        //{
        //    DestroySelf();
        //    yield break;
        //}
    
        public void OnMainFBBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
            controller.Close();
        }
    
        public void OnGoldFBBtnClick()
        {
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10020);
            if (!ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTResourceInstanceUI", "Gold");
            controller.Close();
        }
    
        public void OnOpenBuyGoldBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
            controller.Close();
        }
    }
}
