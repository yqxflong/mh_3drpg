namespace Hotfix_LT.UI
{
    public class LTAwakeningInstanceSelectDynamicScroll : DynamicGridScroll<Hotfix_LT.Data.AwakenDungeonTemplate, LTAwakeningInstanceSelectItemController>
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
            IsNoNeedForDelayFill = true;
        }
    }
}
