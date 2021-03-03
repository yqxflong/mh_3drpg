namespace Hotfix_LT.UI
{
    public class MailDynamicScroll : DynamicGridScroll<MailItemData, MailDynamicCellController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.parent.parent.GetComponentEx<UIScrollView>();
            placeholderWidget = t.parent.GetComponentEx<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 16f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
    }
}
