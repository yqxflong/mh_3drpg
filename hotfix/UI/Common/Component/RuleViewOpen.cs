using Hotfix_LT;
using Hotfix_LT.UI;

namespace LT.Hotfix.UI {
    public class RuleViewOpen : DynamicMonoHotfix {
        public override void Awake() {
            base.Awake();

            if (mDMono.StringParamList != null && !string.IsNullOrEmpty(mDMono.StringParamList[0])) {
                var btn = mDMono.transform.GetComponent<UIButton>();

                if (btn != null) {
                    btn.onClick.Add(new EventDelegate(() => {
                        OpenRuleView(mDMono.StringParamList[0]);
                    }));
                } else {
                    EB.Debug.LogError("RuleViewOpen.Awake -> btn (UIButton) is null!");
                }
            }
        }

        private void OpenRuleView(string key) {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString(key));
        }
    }
}
