using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class UIStoreGridScroll : DynamicTableScroll<StoreItemData, UIStoreCellController, UIStoreRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            columns = 2;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.parent.GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 36;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }
        protected override void SetColumns()
        {

        }
    }
}
