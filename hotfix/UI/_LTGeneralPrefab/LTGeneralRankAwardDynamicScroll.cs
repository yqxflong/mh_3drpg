﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTGeneralRankAwardDynamicScroll : DynamicGridScroll<LTRankAwardData, LTGeneralRankAwardCell>
    {
        public override void Awake()
        {
            base.Awake();
            scrollView = mDMono.transform.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = mDMono.transform.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 28;
            addition = 0;
            IsNoNeedForDelayFill = true;
        }
    }
}
