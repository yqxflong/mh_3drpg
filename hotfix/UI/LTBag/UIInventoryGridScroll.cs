using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class UIInventoryGridScroll : DynamicTableScroll<UIInventoryBagCellData, UIInventoryBagCellController, UIInventoryBagRowController> 
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            scrollView = t.GetComponentInParent<UIScrollView>();
            placeholderWidget = t.GetComponentInParent<UIWidget>();
            thresholdFactor = 0.5f;
            padding = 34;
            addition = 0;
            IsNoNeedForDelayFill = false;
            columns = 5;
        }

        protected override void SetColumns()
        {
        }

        public float FadeOffSet(UIInventoryBagRowController ctrl)
        {
            if (mGrid.arrangement == UIGrid.Arrangement.Horizontal)
            {
                float cellLength = mGrid.cellWidth;
                float offsetLength = GetClipOffset().x;
                float contentLength = contentWidth;
    
                float visibleStart = offsetLength <= 0 ? -offsetLength : 0;
                int visibleStartIndex = Mathf.FloorToInt(visibleStart / cellLength);
                float visibleEnd = Mathf.Min(contentLength, offsetLength + mPanelSize.x);
                int visibleEndIndex = Mathf.FloorToInt(visibleEnd / cellLength);
    
                if (ctrl.DataIndex < visibleStartIndex || ctrl.DataIndex > visibleEndIndex)
                {// not visible
                    return 0f;
                }
    
                if (ctrl.DataIndex == visibleStartIndex && visibleStartIndex * cellLength < visibleStart)
                {
                    return ((visibleStartIndex + 1) * cellLength - visibleStart) / cellLength;
                }
    
                if (ctrl.DataIndex == visibleEndIndex && (visibleEndIndex + 1) * cellLength > visibleEnd)
                {
                    return (visibleEnd - (visibleEndIndex) * cellLength) / cellLength;
                }
            }
            else if (mGrid.arrangement == UIGrid.Arrangement.Vertical)
            {
                float cellLength = mGrid.cellHeight;
                float offsetLength = GetClipOffset().y;
                float contentLength = contentHeight;
    
                float visibleStart = offsetLength <= -1f ? -offsetLength : 0;
                int visibleStartIndex = Mathf.FloorToInt(visibleStart / cellLength);
                float visibleEnd = Mathf.Min(contentLength, mPanelSize.y - offsetLength);
                int visibleEndIndex = Mathf.FloorToInt(visibleEnd / cellLength);
    
                if (ctrl.DataIndex < visibleStartIndex || ctrl.DataIndex > visibleEndIndex)
                {// not visible
                    return 0f;
                }
    
                if (ctrl.DataIndex == visibleStartIndex && visibleStartIndex * cellLength < visibleStart)
                {
                    return ((visibleStartIndex+1) * cellLength- visibleStart) / cellLength;
                }
    
                if (ctrl.DataIndex == visibleEndIndex && (visibleEndIndex + 1) * cellLength > visibleEnd)
                {
                    return (visibleEnd- (visibleEndIndex) * cellLength) / cellLength;
                }
            }
    
            return 1f;
        }
    }
}
