namespace Hotfix_LT.UI
{
    public class LTDailyDynamicGrid : DynamicGridScroll<LTDailyData, LTDailyCell>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.parent.parent.GetComponentEx<UIScrollView>();
            placeholderWidget = t.parent.GetComponentEx<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 0f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        } 	
    }
}
