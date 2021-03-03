using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class HelpApplyDynamicScroll : DynamicGridScroll<HelpApplyNode, HelpApplyDynamicCellController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            thresholdFactor = 0.5f;
            padding = 26f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }

    }
}
