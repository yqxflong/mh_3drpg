namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceTicketCtrl : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            t.parent.GetComponent<UIButton>("Notice").onClick.Add(new EventDelegate(OnNoticeClick));
        }

        public void OnNoticeClick()
        {
            string text = EB.Localizer.GetString("ID_CHALLENGE_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }
    
        public void OnAddBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        }
    }
}
