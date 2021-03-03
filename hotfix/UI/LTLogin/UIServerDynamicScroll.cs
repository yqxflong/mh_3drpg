using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class UIServerDynamicScroll : DynamicTableScroll<ServerData, UIServerCellController,UIServerRowController>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            columns = 2;
            scrollView = t.parent .parent . GetComponent<UIScrollView>();
            placeholderWidget = t.parent . GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 9.5f;
            addition = 0;
            IsNoNeedForDelayFill = false;
        }

        protected override void SetColumns()
        {

        }
    }
}
