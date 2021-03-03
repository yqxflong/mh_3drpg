namespace Hotfix_LT.UI
{
    public class LTClimingTowerDynamicScroll : DynamicGridScroll<Hotfix_LT.Data.ClimingTowerRewardTemplate, LTClimingTowerRewardCell>
    {
        public override void Awake()
        {
            base.Awake();
            thresholdFactor = 0.5f;
            padding = 28f;
            addition = 0;
            IsNoNeedForDelayFill = true;
        }
    }
}