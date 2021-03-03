using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipRowController : DynamicRowController<BaseEquipmentInfo, LTPartnerEquipCellController>
    {
        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null && mDMono.ObjectParamList.Count > 0)
            {
                int count = mDMono.ObjectParamList.Count;
                cellCtrls = new LTPartnerEquipCellController[count];
                for (int i = 0; i < count; i++)
                {
                    if (mDMono.ObjectParamList[i] != null)
                    {
                        cellCtrls[i] = (mDMono.ObjectParamList[i] as GameObject).GetMonoILRComponent<LTPartnerEquipCellController>();
                    }
                }

            }
        }
    }
    
    
    public class LTPartnerEquipScroll : DynamicTableScroll<BaseEquipmentInfo, LTPartnerEquipCellController, LTPartnerEquipRowController>, IHotfixUpdate
    {
  //       public float ScrollValue
		// {
		// 	get { return m_ScroolValue; }
		// 	set
		// 	{
		// 		if (value != m_ScroolValue) m_ScroolValue = value;
  //
		// 		if (value != 0)
		// 			RegisterMonoUpdater();
		// 		else
		// 			ErasureMonoUpdater();
		// 	}
		// }
		// private float m_ScroolValue = 0;
  //
  //       private float curValue = 0;
        // protected override void Reposition()
        // {
        //     if (LTPartnerEquipMainController.isEquipLevelView)
        //     {
        //         UIScrollView scrollView = mDMono.transform.GetComponentInParent<UIScrollView>();
        //         ScrollValue = scrollView.verticalScrollBar.value;
        //     }
        //     base.Reposition();
        // }

		// public override void OnDisable()
		// {
		// 	ErasureMonoUpdater();
		// }

		// public new void Update()
  //       {
  //           base.Update();
  //           if (ScrollValue != 0)
  //           {
  //               curValue = ScrollValue;
  //               ScrollValue = 0;
  //               ILRTimerManager.instance.RemoveTimer(SetScrollValue);
  //               ILRTimerManager.instance.AddTimer(120, 1, SetScrollValue);
  //           }
  //       }

        // private void SetScrollValue(int req)
        // {
        //     scrollView.verticalScrollBar.value = curValue;
        // }

        protected override void SetColumns()
        {
            columns = 5;
        }
    }

}