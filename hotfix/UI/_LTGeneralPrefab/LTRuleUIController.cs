using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTRuleUIController : UIControllerHotfix
    {
        public UILabel Context;
        public UIScrollView ScrollView;

        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            t.GetComponent<UIButton>("SureBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("BG/Top/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));

            Context = t.GetComponent<UILabel>("Content/Scroll View/Container/Text");
            ScrollView = t.GetComponent<UIScrollView>("Content/Scroll View");
        }

        public override void SetMenuData(object _menuData)
        {
            /*string id = _menuData as string;
            string text=EB.Localizer.GetString(EB.Symbols.LocIdPrefix + id.ToUpper());
            context.text =text;*/
            string tmpContent = _menuData as string;
            Context.text = tmpContent;

            float panelHeight = ScrollView.GetComponent<UIPanel>().GetViewSize().y;
            ScrollView.enabled = Context.height > panelHeight;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            //yield return base.OnRemoveFromStack();
            DestroySelf();
            yield break;
        }

        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();

            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    }
}
