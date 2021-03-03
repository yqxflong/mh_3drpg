using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerSkillBreakTableScroll : DynamicTableScroll<UIInventoryBagCellData, LTPartnerSkillBreakCellController, LTPartnerSkillBreakRowController>
    {
        protected override void SetColumns()
        {
            columns = 5;
        }
    }
    
    public class LTPartnerSkillBreakRowController : DynamicRowController<UIInventoryBagCellData, LTPartnerSkillBreakCellController>
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;
                cellCtrls = new LTPartnerSkillBreakCellController[count];
                for (int i = 0; i < count; i++)
                {
                    if (mDMono.ObjectParamList[i] != null)
                    {
                        cellCtrls[i] = (mDMono.ObjectParamList[i] as GameObject).GetMonoILRComponent<LTPartnerSkillBreakCellController>();
                    }
                }

            }
        }
    }
}