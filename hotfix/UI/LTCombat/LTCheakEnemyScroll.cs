using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTCheakEnemyScroll : DynamicTableScroll<int, LTCheakEnemyItem,LTCheakEnemyRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            columns = 6;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 12;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
        protected override void SetColumns()
        {

        }
    }
}
