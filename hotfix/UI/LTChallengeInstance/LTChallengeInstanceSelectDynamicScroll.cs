using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceSelectDynamicScroll : DynamicGridScroll<Hotfix_LT.Data.LostChallengeChapter, LTChallengeInstanceSelectItemCtrl>
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
