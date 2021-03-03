using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTBountyTaskSelectRowCtrl : DynamicRowController<LTPartnerData, LTBountyTaskSelectItem>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTBountyTaskSelectItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTBountyTaskSelectItem>();
                }
            }
        }
    }
    
    
}