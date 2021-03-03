namespace Hotfix_LT.UI
{
    public class LTShowGetNewItemDynamicScroll : DynamicTableScroll<LTShowItemData, ShowGetNewItem, LTShowGetNewItemsRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 20f;
            addition = 0;
            IsNoNeedForDelayFill = true;
            columns = 5;
        }

        protected override void SetColumns()
        {

        }
    }
}
