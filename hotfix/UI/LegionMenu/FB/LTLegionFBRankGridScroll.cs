using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTLegionFBRankGridScroll : DynamicGridScroll<LTLegionFBRankItemData, LegionFBRankItem>
    {

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            thresholdFactor = 0.5f;
            padding = 19;
            addition = 0;
            IsNoNeedForDelayFill = false;

        }

    }
}
