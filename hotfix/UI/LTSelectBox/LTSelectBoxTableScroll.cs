using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTSelectBoxTableScroll : DynamicTableScroll<Hotfix_LT.Data.SelectBox, LTSelectBoxCellController, LTSelectBoxRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.GetComponentInParent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 36;
            addition = 0;
            IsNoNeedForDelayFill = false;
            columns = 6;
        }

        protected override void SetColumns()
        {
        }
    }
}
