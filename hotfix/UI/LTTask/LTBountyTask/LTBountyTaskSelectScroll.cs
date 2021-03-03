namespace Hotfix_LT.UI
{
    public class LTBountyTaskSelectScroll : DynamicTableScroll<LTPartnerData, LTBountyTaskSelectItem, LTBountyTaskSelectRowCtrl> {
        protected override void SetColumns()
        {
            columns = 5;
        }
    }
}