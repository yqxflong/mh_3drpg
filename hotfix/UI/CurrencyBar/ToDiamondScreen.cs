namespace Hotfix_LT.UI
{
    public class ToDiamondScreen : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            mDMono.transform.GetComponentEx<UIButton>().onClick.Add(new EventDelegate(() =>
            {
                GlobalMenuManager.Instance.Open("LTChargeStoreHud");
                FusionAudio.PostEvent("UI/General/ButtonClick");
            }));
        }
    }
}

