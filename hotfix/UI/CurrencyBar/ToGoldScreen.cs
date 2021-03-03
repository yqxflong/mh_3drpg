namespace Hotfix_LT.UI
{
    public class ToGoldScreen : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            mDMono.transform.GetComponentEx<UIButton>().onClick.Add(new EventDelegate(() =>
            {
                GlobalMenuManager.Instance.Open("LTResourceShopUI");
                FusionAudio.PostEvent("UI/General/ButtonClick");
            }));
        }
    }
}
