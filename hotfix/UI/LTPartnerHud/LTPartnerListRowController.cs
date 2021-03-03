using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerListRowController : DynamicRowController<LTPartnerData, LTPartnerListCellController>
    {
        public override void Awake()
        {
            cellCtrls = new LTPartnerListCellController[2];
            cellCtrls[0] = mDMono.transform.Find("Item").GetMonoILRComponent<LTPartnerListCellController>();
            cellCtrls[1] = mDMono.transform.Find("Item (1)").GetMonoILRComponent<LTPartnerListCellController>();

            base.Awake();
        }
    }
}