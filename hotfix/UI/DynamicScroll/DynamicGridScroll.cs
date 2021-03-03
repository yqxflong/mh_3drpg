using System.Linq;

namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections.Generic;

    [RequireComponent(typeof(UIGrid))]
    public abstract class DynamicGridScroll<T, U> : DynamicScroll<T, U> where U : DynamicCellController<T>
    {
        protected UIGrid mGrid = null;

        protected Vector3 mGridOrigionPosition = Vector3.zero;

        public bool CloseUpdate { get; set; }

        public int viewVerticalLimit
        {
            get { return mGrid.arrangement == UIGrid.Arrangement.Vertical ? Mathf.CeilToInt(mPanelSize.y / mGrid.cellHeight) : 0; }
        }

        public int viewHorizontalLimit
        {
            get { return mGrid.arrangement == UIGrid.Arrangement.Horizontal ? Mathf.CeilToInt(mPanelSize.x / mGrid.cellWidth) : 0; }
        }

        public float gridHeight
        {
            get { return mGrid.arrangement == UIGrid.Arrangement.Vertical ? mGrid.cellHeight * activates.Count : 0; }
        }

        public float contentHeight
        {
            get { return mGrid.arrangement == UIGrid.Arrangement.Vertical ? mGrid.cellHeight * mDataItems.Length : 0; }
        }

        public float gridWidth
        {
            get { return mGrid.arrangement == UIGrid.Arrangement.Horizontal ? mGrid.cellWidth * activates.Count : 0; }
        }

        public float contentWidth
        {
            get { return mGrid.arrangement == UIGrid.Arrangement.Horizontal ? mGrid.cellWidth * mDataItems.Length : 0; }
        }

        // Use this for initialization
        public override void Start()
        {
            mGrid =mDMono.GetComponent<UIGrid>();
            if (mGrid.arrangement != UIGrid.Arrangement.Horizontal && mGrid.arrangement != UIGrid.Arrangement.Vertical)
            {
                EB.Debug.LogError("DynamicGridScroll.Start: Grid Arrangement error");
                mGrid.arrangement = UIGrid.Arrangement.Vertical;
            }
            mGrid.hideInactive = true;
            mGridOrigionPosition = mGrid.transform.localPosition;

            base.Start();
		}
		// Update is called once per frame
		public override void Update()
        {
            if (Dirty)
            {
                if (mReposition)
                {
                    Retire();
                    Allocate();
                    Reposition();
                    Fill();

                    mReposition = false;
                }
                else
                {
                    for (int i = 0; i < activates.Count; i++)
                    {
                        activates[i].transform.SetAsLastSibling();
                    }
                    Fill(0, mLastDataStartIndex, activates.Count);
                    mGrid.Reposition();
                }
                
                if (CloseUpdate)
	                Dirty = false;
                else
					m_Dirty = false;
				return;
            }

            if (mMoveDirty)
            {
                MoveInternal();
                mMoveDirty = false;
            }

            if (mDataItems.Length <= activates.Count)
            {
	            if (m_Dirty)
		            return;

	            Dirty = false;
            }

            Vector2 updateThreshold = mPanelSize * thresholdFactor;
            if (mPanel == null)
            {
                if (scrollView == null) return;
                mPanel = scrollView.GetComponent<UIPanel>();
            }
			Vector2 offset = mPanel.clipOffset;

            float viewLength = 0f;
            float contentLength = 0f;
            float cellLength = 0f;
            float offsetLength = 0f;
            float gridLength = 0f;

            if (mGrid.arrangement == UIGrid.Arrangement.Vertical && Mathf.Abs(offset.y - mLastScrollOffset.y) > updateThreshold.y)
            {
                viewLength = mPanelSize.y;
                contentLength = contentHeight;
                cellLength = mGrid.cellHeight;
                offsetLength = offset.y - mPanelOrigionClipOffset.y;
                gridLength = gridHeight;

                Recalculate(viewLength, contentLength, cellLength, gridLength, offsetLength);

                mGrid.transform.localPosition = mGridOrigionPosition + new Vector3(0, mLastGridPosition);
                //Fill(mActiveRefStartIndex, mLastDataStartIndex, mActiveRefreshCount);
                //Fill(0, mLastDataStartIndex, activates.Count);

                mLastScrollOffset = offset;
            }

            if (mGrid.arrangement == UIGrid.Arrangement.Horizontal && Mathf.Abs(offset.x - mLastScrollOffset.x) > updateThreshold.x)
            {
                viewLength = mPanelSize.x;
                contentLength = contentWidth;
                cellLength = mGrid.cellWidth;
                offsetLength = offset.x - mPanelOrigionClipOffset.x;
                gridLength = gridWidth;

                Recalculate(viewLength, contentLength, cellLength, gridLength, offsetLength);

                mGrid.transform.localPosition = mGridOrigionPosition + new Vector3(mLastGridPosition, 0);
                //Fill(mActiveRefStartIndex, mLastDataStartIndex, mActiveRefreshCount);
                //Fill(0, mLastDataStartIndex, activates.Count);

                mLastScrollOffset = offset;
            }
        }
        
        public override int GetCellLimit()
        {
            int viewLimit = 0;
			if (mGrid == null) mGrid = mDMono.GetComponent<UIGrid>();
            if (mGrid.arrangement == UIGrid.Arrangement.Horizontal)
            {
                viewLimit = viewHorizontalLimit;
            }
            else if (mGrid.arrangement == UIGrid.Arrangement.Vertical)
            {
                viewLimit = viewVerticalLimit;
            }

            int cellLimit = Mathf.CeilToInt(viewLimit * (1 + 2 * thresholdFactor));
            if (mGrid.maxPerLine > 0)
            {
                if (mGrid.maxPerLine < cellLimit)
                {
                    mGrid.maxPerLine = cellLimit;
                }

                return Mathf.Max(mGrid.maxPerLine, cellLimit);
            }
            else
            {
                return Mathf.Max(mGrid.transform.childCount, cellLimit);
            }
        }

        protected void Recalculate(float viewLength, float contentLength, float cellLength, float gridLength, float offsetLength)
        {
            float visibleStart = offsetLength <= 0 ? -offsetLength : offsetLength;//fix when horizen offsetLength>0

            int cellCount = Mathf.CeilToInt(gridLength / cellLength);
            int dataCount = Mathf.CeilToInt(contentLength / cellLength);
            int viewCount = Mathf.CeilToInt(viewLength / cellLength);

            int index = mLastDataStartIndex;

            mLastDataStartIndex = Mathf.FloorToInt(visibleStart / cellLength);
            mLastDataStartIndex -= (cellCount - viewCount) / 2;
            if (mLastDataStartIndex < 0)
            {
                mLastDataStartIndex = 0;
            }
            if (mLastDataStartIndex + cellCount > dataCount)
            {
                mLastDataStartIndex = dataCount - cellCount;
            }

            mActiveRefreshCount = Mathf.Abs(index - mLastDataStartIndex);

            if (mActiveRefreshCount > 0 && mActiveRefreshCount < cellCount)
            {
                mActiveRefStartIndex = index - mLastDataStartIndex <= 0 ? 0 : cellCount - mActiveRefreshCount;

                List<Transform> trans = new List<Transform>();
                for (int i = 0; i < mActiveRefreshCount; i++)
                {
                    trans.Add(mGrid.transform.GetChild(mActiveRefStartIndex + i));

                    int actIndex = mActivates.IndexOf(trans[i].gameObject);
                    if (actIndex >= 0)
                    {
                        Fill(actIndex, mLastDataStartIndex + (cellCount - mActiveRefStartIndex - mActiveRefreshCount) + i, 1);
                    }
                }

                for (int i = 0; i < trans.Count; i++)
                {
                    if (index - mLastDataStartIndex > 0)
                    {
                        trans[trans.Count - i - 1].SetAsFirstSibling();
                    }
                    else
                    {
                        trans[i].SetAsLastSibling();
                    }
                }
                mGrid.Reposition();
            }
            else if (mActiveRefreshCount >= cellCount)
            {
                for (int i = 0; i < activates.Count; i++)
                {
                    activates[i].transform.SetAsLastSibling();
                }
                mGrid.Reposition();
                Fill(0, mLastDataStartIndex, activates.Count);
            }

            mLastGridPosition = offsetLength <= 0 ? -mLastDataStartIndex * cellLength : mLastDataStartIndex * cellLength;//fix when horizen
        }

        protected override void Allocate()
        {
			//EB.Debug.Log("Grids Allocate!");
            base.Allocate();

            if (mGrid.arrangement == UIGrid.Arrangement.Horizontal)
            {
                if (mDataItems.Length < viewHorizontalLimit)
                {
                    placeholderWidget.width = (int)mPanelSize.x + addition;
                }
                else
                {
                    placeholderWidget.width = Mathf.CeilToInt(mGrid.cellWidth * mDataItems.Length - padding) + addition;
                }
            }
            else if (mGrid.arrangement == UIGrid.Arrangement.Vertical)
            {
                if (mDataItems.Length < viewVerticalLimit)
                {
                    placeholderWidget.height = (int)mPanelSize.y + addition;
                }
                else
                {
                    placeholderWidget.height = Mathf.CeilToInt(mGrid.cellHeight * mDataItems.Length - padding) + addition;
                }
            }
        }

        protected override void Reposition()
        {
            mGrid.transform.localPosition = mGridOrigionPosition;
            mGrid.Reposition();

            base.Reposition();
        }

        public bool IsVisible(DynamicCellController<T> ctrl)
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

                return ctrl.DataIndex >= visibleStartIndex && ctrl.DataIndex <= visibleEndIndex;
            }
            else if (mGrid.arrangement == UIGrid.Arrangement.Vertical)
            {
                float cellLength = mGrid.cellHeight;
                float offsetLength = GetClipOffset().y;
                float contentLength = contentHeight;

                float visibleStart = offsetLength <= 0 ? -offsetLength : 0;
                int visibleStartIndex = Mathf.FloorToInt(visibleStart / cellLength);
                float visibleEnd = Mathf.Min(contentLength, offsetLength + mPanelSize.y);
                int visibleEndIndex = Mathf.FloorToInt(visibleEnd / cellLength);

                return ctrl.DataIndex >= visibleStartIndex && ctrl.DataIndex <= visibleEndIndex;
            }

            return false;
        }

        public bool IsFade(DynamicCellController<T> ctrl)
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
                    return false;
                }

                if (ctrl.DataIndex == visibleStartIndex && visibleStartIndex * cellLength < visibleStart)
                {
                    return true;
                }

                if (ctrl.DataIndex == visibleEndIndex && (visibleEndIndex + 1) * cellLength > visibleEnd)
                {
                    return true;
                }
            }
            else if (mGrid.arrangement == UIGrid.Arrangement.Vertical)
            {
                float cellLength = mGrid.cellHeight;
                float offsetLength = GetClipOffset().y;
                float contentLength = contentHeight;

                float visibleStart = offsetLength <= 0 ? -offsetLength : 0;
                int visibleStartIndex = Mathf.FloorToInt(visibleStart / cellLength);
                float visibleEnd = Mathf.Min(contentLength, offsetLength + mPanelSize.y);
                int visibleEndIndex = Mathf.FloorToInt(visibleEnd / cellLength);

                if (ctrl.DataIndex < visibleStartIndex || ctrl.DataIndex > visibleEndIndex)
                {// not visible
                    return false;
                }

                if (ctrl.DataIndex == visibleStartIndex && visibleStartIndex * cellLength < visibleStart)
                {
                    return true;
                }

                if (ctrl.DataIndex == visibleEndIndex && (visibleEndIndex + 1) * cellLength > visibleEnd)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">0 1 2 3</param>
        public void MoveTo(int index)
        {
            mMoveTargetIndex = index;
            mMoveDirty = true;
        }

        private void MoveInternal()
        {
            float cellLength = 0f;
            float offsetLength = 0f;

            if (mGrid.arrangement == UIGrid.Arrangement.Horizontal)
            {
                cellLength = mGrid.cellWidth;
                offsetLength = cellLength * mMoveTargetIndex + mPanelOrigionClipOffset.x - mPanel.clipOffset.x;
                scrollView.MoveRelative(new Vector3(-1 * offsetLength, 0, 0));
            }
            else
            {
                cellLength = mGrid.cellHeight;
                offsetLength = -1 * (cellLength * mMoveTargetIndex + mPanelOrigionClipOffset.y) - mPanel.clipOffset.y;
                scrollView.MoveRelative(new Vector3(0, -1 * offsetLength, 0));
            }
        }

        public void MoveInternalNow(int index)
        {
            mMoveTargetIndex = index;
            MoveInternal();
        }
    }

    public abstract class DynamicRowController<T, V> : DynamicCellController<IEnumerable<T>>
        where V : DynamicCellController<T>
    {
        public V[] cellCtrls;
        public T[] cellDatas;

        public override void Fill(IEnumerable<T> rowItemDatas)
        {
            List<T> list = new List<T>(rowItemDatas);
            cellDatas = list.ToArray();

            for (int i = 0; i < cellCtrls.Length; ++i)
            {
                /// 这里需要显式标明类型，否则在调用父类属性及方法会报类型转换错误：InvalidCastException: Specified cast is not valid.
                var cell = (V)cellCtrls[i];

                if (cell == null)
                {
                    continue;
                }

                cell.ViewIndex = ViewIndex;
                cell.DataIndex = DataIndex * cellCtrls.Length + i;

                if (i < list.Count)
                {
                    cell.Fill(cellDatas[i]);
                }
                else
                {
                    cell.Clean();
                }
            }
        }

        public override void Clean()
        { 
            cellDatas = new T[0];

            for (int i = 0; i < cellCtrls.Length; ++i)
            {
                /// 这里需要显式标明类型，否则在调用父类属性及方法会报类型转换错误：InvalidCastException: Specified cast is not valid.
                var cell = (V)cellCtrls[i];

                if (cell == null)
                {
                    continue;
                }

                cell.ViewIndex = ViewIndex;
                cell.DataIndex = DataIndex * cellCtrls.Length + i;
                cell.Clean();
            }
        }

        public void ForEach(System.Action<V, T, int> action)
        {
            for (int i = 0; i < cellCtrls.Length; ++i)
            {
                /// 这里需要显式标明类型，否则在调用父类属性及方法会报类型转换错误：InvalidCastException: Specified cast is not valid.
                var cell = (V)cellCtrls[i];

                if (cell == null)
                {
                    continue;
                }

                action(cell, i < cellDatas.Length ? cellDatas[i] : default(T), i);
            }
        }

        public void ForEach(System.Action<V, T> action)
        {
            for (int i = 0; i < cellCtrls.Length; ++i)
            {
                /// 这里需要显式标明类型，否则在调用父类属性及方法会报类型转换错误：InvalidCastException: Specified cast is not valid.
                var cell = (V)cellCtrls[i];

                if (cell == null)
                {
                    continue;
                }
                action(cell, i < cellDatas.Length ? cellDatas[i] : default(T));
            }
        }
    }

    public abstract class DynamicTableScroll<T, V, U> : DynamicGridScroll<IEnumerable<T>, U> where V : DynamicCellController<T> where U : DynamicRowController<T, V>
    {
        //一行多少个
        public int columns=1;
        protected abstract void SetColumns();

        public override void Awake()
        {
            SetColumns();
            base.Awake();
        }

        public void SetItemDatas(IEnumerable<T> data, bool force = true)
        {
	        //EB.Debug.Log("Set Item Datas!");
			List<T> list = new List<T>(data);
            //此处调用Split方法会报错（KeyNotFoundException: Cannot find method:Split in type:Hotfix_LT.UI.DynamicTableScroll），原因暂未找到，所以先把Split方法中的代码实现内联进来
            //List<List<T>> splits = Split(list, columns);
            //this.dataItems = splits.ToArray();

            var splits = new List<List<T>>();
            for (int i = 0; i < list.Count; i += columns) {
                splits.Add(list.GetRange(i, Mathf.Min(columns, list.Count - i)));
            }
            if (splits.Count <= 0)
            {
				EB.Debug.LogWarning("Splits.Count <= 0!");
            }
			SetItemDatas(splits.ToArray(), force);
        }

        protected override void OnFilled(U rowCtrl, IEnumerable<T> rowItemDatas)
        {
            V[] cellCtrls = rowCtrl.cellCtrls;
            for (int i = 0; i < cellCtrls.Length; ++i)
            {
                OnFilled(cellCtrls[i], i < rowCtrl.cellDatas.Length ? rowCtrl.cellDatas[i] : default(T));
            }
        }

        protected virtual void OnFilled(V itemCtrl, T itemData)
        {

        }

        public static List<List<T>> Split(List<T> items, int sliceSize)
        {
            List<List<T>> list = new List<List<T>>();

            for (int i = 0; i < items.Count; i += sliceSize) {
                list.Add(items.GetRange(i, Mathf.Min(sliceSize, items.Count - i)));
            }

            return list;
        }
    }

}