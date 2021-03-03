using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTSelectBoxRowController : DynamicRowController<Hotfix_LT.Data.SelectBox, LTSelectBoxCellController>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTSelectBoxCellController[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTSelectBoxCellController>();
                }
            }
        }
    }
}
