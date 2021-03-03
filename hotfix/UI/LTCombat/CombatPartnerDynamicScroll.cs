using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class CombatPartnerDynamicScroll : DynamicGridScroll<LTPartnerData, CombatPartnerCellController>
    {
        private System.Action<CombatPartnerCellController> onDragStartFunc;
        private System.Action onDragFunc;
        private System.Action onDragEndFunc;

		private bool isInited;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 10f;
            addition = 0;
            IsNoNeedForDelayFill = false;			
		}

		public override void OnEnable()
		{
			Debug.Log("DynamicScroll.Enabled!");
			base.OnEnable();

			ILRTimerManager.instance.AddTimer(120, 1, (_) => {
				if (mDMono != null)
				{
					if (!isInited)
					{
						int itemCount = LTPartnerDataManager.Instance.GetOwnPartnerList().Count;
						ILRTimerManager.instance.AddTimer(220 * itemCount, 1, (__) =>
						{
							if (mDMono != null)
							{
								Transform parent = mDMono.transform.parent.parent;
								parent.GetComponent<UIPanel>().EmptyingAnchors();
								parent.GetComponent<UIScrollView>().enabled = true;
							}
						});						
					}
					else
					{
						Transform parent = mDMono.transform.parent.parent;
						parent.GetComponent<UIPanel>().EmptyingAnchors();
						BattleReadyHudController controller = UIControllerHotfix.Current as BattleReadyHudController;
						if (controller != null) controller.RefreshPartnerList(eAttrTabType.All);
					}
				}				
				isInited = true;
			});			
		}

		public void SetItemDragStartAction(System.Action<CombatPartnerCellController> _onDragStartFunc)
    	{
    		onDragStartFunc = _onDragStartFunc;
    	}
    
    	public void SetItemDragAction(System.Action _onDragFunc)
    	{
    		onDragFunc = _onDragFunc;
    	}
    
    	public void SetItemDragEndAction(System.Action _onDragEndFunc)
    	{
    		onDragEndFunc = _onDragEndFunc;
    	}
    
    	protected override void OnFilled(CombatPartnerCellController ctrl, LTPartnerData itemData)
    	{
    		base.OnFilled(ctrl, itemData);
    
    		ctrl.SetItemDragStartActionNew(onDragStartFunc);
    		ctrl.SetItemDragAcionNew(onDragFunc);
    		ctrl.SetItemDragEndActionNew(onDragEndFunc);
    	}
    }
}
