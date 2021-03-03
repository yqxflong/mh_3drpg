using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTCheakEnemyRowController : DynamicRowController<int, LTCheakEnemyItem>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                UIGrid grid = t.GetComponentInChildren<UIGrid>();
                cellCtrls = new LTCheakEnemyItem[grid.transform.childCount];

                int len = grid.transform.childCount;
                for (var i = 0; i < len; i++)
                {
                    cellCtrls[i] = grid.transform.GetChild(i).GetMonoILRComponent<LTCheakEnemyItem>();
                }
            }
        }

        public override void Fill(IEnumerable<int> rowItemDatas)
        {
            mDMono.transform.GetChild(0).GetComponent<UILabel>().text = mDMono.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text =string.Format(EB.Localizer.GetString("ID_codefont_in_LTCheakEnemyRowController_395"), DataIndex + 1);
            base.Fill(rowItemDatas);
        }
    }
}
