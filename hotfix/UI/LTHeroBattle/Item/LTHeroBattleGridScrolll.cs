
namespace Hotfix_LT.UI
{
    public class LTHeroBattleGridScrolll : DynamicGridScroll<LTHeroBattleListData, LTHeroBattleListItem>
    {
        public override void Awake()
        {
            base.Awake();

            thresholdFactor = 0.5f;
            padding = 0f;
            addition = 0;
            IsNoNeedForDelayFill = true;
        }
    }
}