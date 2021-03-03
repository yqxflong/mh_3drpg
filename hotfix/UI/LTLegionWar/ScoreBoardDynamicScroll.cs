using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class ScoreBoardDynamicScroll : DynamicGridScroll<ScoreBoardData, ScoreBoardCell>
    {
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            thresholdFactor = 0.5f;
            padding = 0f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }


    
    }
}
