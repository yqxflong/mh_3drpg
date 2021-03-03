namespace Hotfix_LT.UI
{
    public class LegionMercenaryScroll : DynamicGridScroll<LegionMemberData, LegionMercenaryItem>
    {
        public override void Awake()
        {
            base.Awake();
            scrollView = mDMono.transform.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = mDMono.transform.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 7;
            addition = 0;
            IsNoNeedForDelayFill = false;            
        }
    }
}