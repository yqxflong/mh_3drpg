namespace Hotfix_LT.UI
{
    public class ApplyHelpDynamicScroll : DynamicGridScroll<ApplyHelpNode, ApplyHelpDynamicCellController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            thresholdFactor = 0.5f;
            padding = 26;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
    }
}
