namespace Hotfix_LT.UI
{
    public class LTEquipmentSuitInfoGridController : DynamicTableScroll<Hotfix_LT.Data.SuitTypeInfo, LTEquipmentSuitInfoItem, LTEquipmentSuitInfoRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            columns = 2;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 0f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }

        protected override void SetColumns()
        {
        }
    }
}
