using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTSSRWishScroll : DynamicTableScroll<HeroInfoTemplate, SSRWishItem, LTSSRWishRowController>
    {
        protected override void SetColumns()
        {
            columns = 2;
        }
    }
}