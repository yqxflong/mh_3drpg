using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationMemberDynamicScroll : DynamicGridScroll<NationMember,NationMemberScrollCellController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.GetComponent<UIScrollView>("LTNationRanksUI/Content/MembersScrollView");
            placeholderWidget = t.GetComponent<UIWidget>("LTNationRanksUI/Content/MembersScrollView/Placeholder");
            thresholdFactor = 0.5f;
            padding = 15f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
    
    }
}
