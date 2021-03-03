using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    public class LTPartnerListTableScroll : DynamicTableScroll<LTPartnerData, LTPartnerListCellController, LTPartnerListRowController>
    {

        protected override void SetColumns()
        {
            columns = 2;
        }

        public override void Awake()
        {
            base.Awake();
            scrollView = mDMono.transform.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = mDMono.transform.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 36;
            addition = 0;
            IsNoNeedForDelayFill = false;
            columns = 2;
        }
    }
}