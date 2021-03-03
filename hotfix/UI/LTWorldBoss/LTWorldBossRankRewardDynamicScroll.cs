namespace Hotfix_LT.UI
{
    public class LTWorldBossRankRewardDynamicScroll : DynamicGridScroll<Hotfix_LT.Data.BossRewardTemplate, LTWorldBossRankRewardCell>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.GetComponentInParent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 0f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
    	
    }
}
