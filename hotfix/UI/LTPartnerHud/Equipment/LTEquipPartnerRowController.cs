using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTEquipPartnerRowController : DynamicRowController<LTPartnerData,LTEquipPartnerCellController>
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;
                cellCtrls = new LTEquipPartnerCellController[count];
                for (int i = 0; i < count; i++)
                {
                    if (mDMono.ObjectParamList[i] != null)
                    {
                        cellCtrls[i] = (mDMono.ObjectParamList[i] as GameObject).GetMonoILRComponent<LTEquipPartnerCellController>();
                    }
                }

            }
        }
    }
    
    public class LTEquipPartnerScroll : DynamicTableScroll<LTPartnerData, LTEquipPartnerCellController, LTEquipPartnerRowController> {
        protected override void SetColumns()
        {
            columns = 4;
        }
    }
}