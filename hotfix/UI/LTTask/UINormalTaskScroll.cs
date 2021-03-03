namespace Hotfix_LT.UI
{
    public class UINormalTaskScroll : DynamicGridScroll<UINormalTaskScrollItemData, UINormalTaskScrollItem>
    {
        public override void Awake()
        {
            thresholdFactor = 0.5f;
            padding = 8;
            addition = 0;
            IsNoNeedForDelayFill = false;
            base.Awake();
        }
    }
}