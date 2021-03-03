using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPushMsgScroll : DynamicTableScroll<LTDailyData, LTPushMsgCell, LTPushMsgGrid>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            columns = 2;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 12f;
            addition = 760;
            IsNoNeedForDelayFill = false;
        }

        protected override void SetColumns()
        {        
        }
    }
}
