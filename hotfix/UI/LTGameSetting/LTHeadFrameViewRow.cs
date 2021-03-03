using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHeadFrameViewRow : DynamicRowController<HeadFrame, LTHeadFrameViewItem>
    {
        public override void Awake()
        {
            base.Awake();
            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTHeadFrameViewItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTHeadFrameViewItem>();
                }
            }
        }
    }
}
