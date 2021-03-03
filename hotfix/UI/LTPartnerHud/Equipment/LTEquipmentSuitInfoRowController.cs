namespace Hotfix_LT.UI
{
    public class LTEquipmentSuitInfoRowController : DynamicRowController<Hotfix_LT.Data.SuitTypeInfo, LTEquipmentSuitInfoItem>
    {
        public override void Awake()
        {
            base.Awake();

            if (cellCtrls == null)
            {
                var t = mDMono.transform;
                cellCtrls = new LTEquipmentSuitInfoItem[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponent<LTEquipmentSuitInfoItem>();
                }
            }
        }
    }
}
