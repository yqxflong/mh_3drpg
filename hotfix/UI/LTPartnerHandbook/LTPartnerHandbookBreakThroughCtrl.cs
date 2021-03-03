using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTPartnerHandbookBreakThroughCtrl : UIControllerHotfix
    {
        UIButton HotfixBtn;
        public override void Awake()
        {
            base.Awake();

            Item = controller.transform.Find("Container/BG/Content/LTShowItem").GetMonoILRComponent<LTShowItem>();
            controller.backButton = controller.transform.Find("Container/BG/Title/CloseBtn").GetComponent<UIButton>();
            HotfixBtn = controller.transform.Find("Container/BG/UseButton").GetComponent<UIButton>();
            HotfixBtn.onClick.Add(new EventDelegate(OnBtnClick));
        }
        public override bool ShowUIBlocker { get { return true; } }
        public LTShowItem Item;//知识点数
        private System.Action Callback;
        private int Count;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable ht = param as Hashtable;
            Callback = (System.Action)ht["Callback"];
            Count = (int)ht["Count"];
            Item.LTItemData = new LTShowItemData("handbook-point", Count - LTPartnerHandbookManager.Instance.GetHandBookSpoint(), LTShowItemType.TYPE_RES, false);
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            BtnClick = false;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTPartnerHandbookManager.Instance.ResetHasScore();
            DestroySelf();
            Callback = null;
            yield break;
        }

        private bool BtnClick = false;
        public void OnBtnClick()
        {
            BtnClick = true;
            LTPartnerHandbookManager.Instance.GetPoint(Count, delegate {
                if (Callback != null)
                {
                    Callback();
                    FusionAudio.PostEvent("UI/New/ZuanShi", true);
                    controller.Close();
                    BtnClick = false;
                }
            });
        }

        public override void OnCancelButtonClick()
        {
            return;
        }
    }

}
