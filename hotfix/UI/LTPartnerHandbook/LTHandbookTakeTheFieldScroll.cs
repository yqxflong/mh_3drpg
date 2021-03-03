using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTHandbookTakeTheFieldScroll : DynamicTableScroll<LTPartnerData, HandbookTakeTheFieldItem, LTHandbookTakeTheFieldRowController>
    {
        protected override void SetColumns()
        {
            columns = 2;
        }
    }
}
