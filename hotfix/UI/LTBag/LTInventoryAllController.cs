using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTInventoryAllController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");
        }

    	public override bool ShowUIBlocker { get { return false; } }
    	public override bool IsFullscreen(){return true;}
    
        public static UIInventoryBagCellController _CurrentSelectCell;
        public static string _CurrentSelectCellId;
        public static Queue ShowAwards;
    
        public override IEnumerator OnAddToStack()
        {
            UITooltipManager.Instance.CheakToolTip();

            yield return base.OnAddToStack();
            GlobalMenuManager.Instance.PushCache("InventoryView");
            UIInventoryBagLogic.Instance.RefeshBag(ShowBagContent.Instance.CurType);

            Messenger.AddListener(Hotfix_LT.EventName.InventoryEvent,OnInventoryEvent);
            yield break;
    	}
    
        public static void SetShowAwardsQueue(LTShowItemData itemData)
        {
            if (ShowAwards == null)
            {
                ShowAwards = new Queue();
            }
            ShowAwards.Enqueue(itemData);
        }
    
        public void OnInventoryEvent()
        {
            if (ShowAwards == null || ShowAwards.Count == 0)
            {
                return;
            }
            LTShowItemData itemData = ShowAwards.Dequeue() as LTShowItemData;
            if (itemData != null)
            {
                GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
            }
        }
    
        public static void SelectFirst(string id) {
            if (_CurrentSelectCell != null && _CurrentSelectCell.Border != null)
            {
                _CurrentSelectCell.Border.gameObject.SetActive(false);
            }

            _CurrentSelectCellId = id;
        }
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            _CurrentSelectCell = null;
            LTMainHudManager .Instance.UpdataItemsData();
    	    Messenger.RemoveListener(Hotfix_LT.EventName.InventoryEvent,OnInventoryEvent);
            DestroySelf();
    		yield break;
    	}
    
    	public override void OnFocus()
    	{
    		base.OnFocus();
            
        }
    
        public override void StartBootFlash() {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();

            for(int j=0;j<tweeners.Length;++j) {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    
    	public override void OnCancelButtonClick()
    	{
            GlobalMenuManager.Instance.RemoveCache("InventoryView");
            base.OnCancelButtonClick();
    	}
    
    }
    
}
