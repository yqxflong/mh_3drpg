using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
    public class LTShowGetNewItemsRowController : DynamicRowController<LTShowItemData, ShowGetNewItem>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new ShowGetNewItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<ShowGetNewItem>();
                }
            }
        }

        public override void Fill(IEnumerable<LTShowItemData> rowItemDatas)
        {
	        //EB.Debug.Log(mDMono.transform.name + ".say: RowController Fill!");
			if (rowItemDatas == null || rowItemDatas.Count() <= 0 )
				EB.Debug.LogWarning("Row Item Data Is Empty!");
            base.Fill(rowItemDatas);
            var grid = mDMono.GetComponent<UIGrid>();

            if (LTGetItemUIController.m_isHC)
            {
                grid.cellWidth = 480;
            }
            else
            {
                grid.cellWidth = 360;
            }

            grid.Reposition();
        }
    }
}
