using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPushMsgGrid : DynamicRowController<LTDailyData, LTPushMsgCell>
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;
                cellCtrls = new LTPushMsgCell[count];
                for (int i = 0; i < count; i++)
                {
                    if (mDMono.ObjectParamList[i] != null)
                    {
                        cellCtrls[i] = (mDMono.ObjectParamList[i] as GameObject).GetMonoILRComponent<LTPushMsgCell>();
                    }
                }

            }
        }

    }

}

