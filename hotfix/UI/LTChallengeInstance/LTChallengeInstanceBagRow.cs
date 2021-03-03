using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceBagRow : DynamicRowController<LTChallengeInstanceBagData, LTChallengeInstanceBagCell>
    {
        public override void Awake()
        {
            base.Awake();
            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTChallengeInstanceBagCell[t.childCount];

                for (var i = 0; i < t.childCount; ++i)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTChallengeInstanceBagCell>();
                }
            }
        }

        public void SetOnBtnClickAction(System.Action<LTChallengeInstanceBagCell> OnClick)
        {
            for (var i = 0; i < cellCtrls.Length; ++i)
            {
                cellCtrls[i].SetOnBtnClickAction(OnClick);
            }
        }
    }
}
