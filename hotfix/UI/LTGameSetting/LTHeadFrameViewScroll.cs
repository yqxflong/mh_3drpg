using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTHeadFrameViewScroll : DynamicTableScroll<HeadFrame, LTHeadFrameViewItem, LTHeadFrameViewRow>
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            columns = 4;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 32f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }

        protected override void SetColumns()
        {
        }
    }
}