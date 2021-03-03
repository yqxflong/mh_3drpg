namespace Hotfix_LT.UI
{
    public class LTpartnerInfoDynamicScroll : DynamicTableScroll<Hotfix_LT.Data.HeroInfoTemplate, LTpartnerInfoItem, LTpartnerInforRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 0.0f;
            addition = 0;
            IsNoNeedForDelayFill = false;
            columns = 4;
        }

        protected override void SetColumns()
        {

        }
    }
}
