using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class RegionDymamicScroll : DynamicGridScroll<RegionCollection, RegionCellController>
    {
        public System.Action<RegionCellController> callBack;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.parent .parent . GetComponent<UIScrollView>();
            placeholderWidget = t.parent .GetComponent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 14f;
            addition = 0;
            IsNoNeedForDelayFill = false;

        }

        public void SetAction(System.Action<RegionCellController> action)
        {
            callBack = action;
        }

        protected override void OnFilled(RegionCellController ctrl, RegionCollection itemData)
        {
            base.OnFilled(ctrl, itemData);
            ctrl.SetAction(callBack);
        }
    }
}
