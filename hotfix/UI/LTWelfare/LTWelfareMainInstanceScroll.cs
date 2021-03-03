using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTWelfareMainInstanceScroll : DynamicGridScroll<MainCampaignRewardTemplate, LTWelfareMainInstanceItem>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 0f;
            addition = 0;
            IsNoNeedForDelayFill = true;
        }
    }
}