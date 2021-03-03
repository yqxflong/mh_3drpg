using LT.Hotfix.Utility;

namespace Hotfix_LT.UI
{
    public class LTEquipmentSortRowController : DynamicRowController<EquipmentSortType, LTEquipmentSortItem>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTEquipmentSortItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTEquipmentSortItem>();
                }
            }
        }
    }
}
