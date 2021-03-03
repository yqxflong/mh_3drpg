using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class NationRanksRewardDynamicScroll : DynamicGridScroll<NationRankRewardItemData,NationRankRewardCellController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 19f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
    	
    }
}
