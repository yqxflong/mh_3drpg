using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI { 

public abstract class DynamicUITableScroll<T, U> : DynamicScroll<T, U> where U : DynamicUITableCellController<T>
{
    public int CellLimit = 10;

    public bool IsForward = true;

    public float mOffsetValue;

    protected UITable mTable;

    protected Vector3 mTableOrigionPosition = Vector3.zero;
    
    public bool CloseUpdate { get; set; }

    public override void Start()
    {
        base.Start();

        mTable = mDMono.GetComponent<UITable>();
        mTableOrigionPosition = mTable.transform.localPosition;
        placeholderWidget.enabled = false;
    }

 //    public override void OnDisable()
 //    {
 //        base.OnDisable();
 //        CloseUpdate = true;
 //    }
 //
 //    public override void OnEnable()
	// {
	// 	base.OnEnable();
 //        CloseUpdate = false;
 //    }

	private Vector2 updateThreshold;
    private Vector2 offset;
    public override void Update()
    {
        base.Update();
        // if (CloseUpdate == false)
        // {
        //     Dirty = true;
        // }
        if (mPanel != null)
        {
            updateThreshold = mPanelSize * thresholdFactor;
            offset = mPanel.clipOffset;

            if (Mathf.Abs(offset.y - mLastScrollOffset.y) > updateThreshold.y)
            {
                float offsetLength = offset.y - mPanelOrigionClipOffset.y;

                Recalculate(offsetLength);

                mLastScrollOffset = offset;
            }
        }
    }

    public override int GetCellLimit()
    {
        return CellLimit;
    }

    private int refreshIndex = 0;
    protected void Recalculate(float offsetLength)
    {
        float offsetDiff = offsetLength - mLastGridPosition + mOffsetValue;

        float visibleStart = 0;

        int index = mLastDataStartIndex;

        if (offsetDiff > 0)
        {
            if (mLastDataStartIndex <= 0)
            {
                return;
            }

            for (int i = mDMono.transform.childCount - 1; i >= 0; i--)
            {

                if (mLastDataStartIndex <= 0)
                {
                    break;
                }

                float height = (mDMono.transform.GetChild(i).GetMonoILRComponent<U>()).Height + mTable.padding.y;

                if (offsetDiff - visibleStart < height / 2)
                {
                    break;
                }

                (mDMono.transform.GetChild(i).GetMonoILRComponent<U>()).RefreshHeightEvent = delegate (int height1)
                {
                    mLastGridPosition += height1 + mTable.padding.y;

                    mTable.transform.localPosition = mTableOrigionPosition + new Vector3(0, mLastGridPosition);

                    mTable.Reposition();
                };

                mLastDataStartIndex--;
                SetWidgetStu();
                visibleStart += height;
                if (offsetDiff < visibleStart)
                {
                    break;
                }
            }
        }
        else
        {
            if (mLastDataStartIndex >= mDataItems.Length - mDMono.transform.childCount)
            {
                return;
            }

            for (int i = 0; i < mActivates.Count; i++)
            {
                if (mLastDataStartIndex >= mDataItems.Length - mDMono.transform.childCount)
                {
                    break;
                }

                float height = (mDMono.transform.GetChild(i).GetMonoILRComponent<U>()).Height + mTable.padding.y;

                if (Mathf.Abs(offsetDiff - visibleStart) < height / 2)
                {
                    break;
                }

                mLastDataStartIndex++;
                SetWidgetStu();
                visibleStart -= height;
                if (offsetDiff > visibleStart)
                {
                    break;
                }
            }
        }

        if (offsetDiff <= 0)
        {
            mLastGridPosition += visibleStart;

            mTable.transform.localPosition = mTableOrigionPosition + new Vector3(0, mLastGridPosition);
        }

        mActiveRefreshCount = Mathf.Abs(index - mLastDataStartIndex);

        if (mActiveRefreshCount > 0 && mActiveRefreshCount < CellLimit)
        {
            mActiveRefStartIndex = index - mLastDataStartIndex <= 0 ? 0 : CellLimit - mActiveRefreshCount;

            List<Transform> trans = new List<Transform>();
            for (int i = 0; i < mActiveRefreshCount; i++)
            {
                trans.Add(mTable.transform.GetChild(mActiveRefStartIndex + i));

                int actIndex = mActivates.IndexOf(trans[i].gameObject);
                if (actIndex >= 0)
                {
                    Fill(actIndex, mLastDataStartIndex + (CellLimit - mActiveRefStartIndex - mActiveRefreshCount) + i, 1);
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
            if (offsetDiff <= 0)
            {
                mTable.Reposition();
            }
        }
        else if (mActiveRefreshCount >= CellLimit)
        {
            RepositionActivate();
            Fill(0, mLastDataStartIndex, activates.Count);
            if (offsetDiff <= 0)
            {
                mTable.Reposition();
            }
        }
    }

    protected void SetWidgetStu()
    {
        if (mLastDataStartIndex <= 0 || mLastDataStartIndex >= mDataItems.Length - CellLimit)
        {
            placeholderWidget.enabled = false;
        }
        else
        {
            placeholderWidget.enabled = true;
        }
    }

    protected override void Reposition()
    {
        if (mTable == null)
        {
            mTable = mDMono.GetComponent<UITable>();
        }

        mTable.transform.localPosition = mTableOrigionPosition;
        mTable.Reposition();

        base.Reposition();
    }

    protected void RepositionActivate()
    {
        for (int i = 0; i < activates.Count; i++)
        {
            activates[i].transform.SetAsLastSibling();
        }
    }

    public void SetOffsetValue(float offsetValue)
    {
        mOffsetValue = offsetValue;
    }

    public void ClearItemData()
    {
        Clear();
        refreshIndex = 0;
        mLastGridPosition = 0;
        mTable.transform.localPosition = mTableOrigionPosition;
    }

    public void StartEvent(Action evt)
    {
        if (mDMono.gameObject.activeInHierarchy)
        {
//            StartCoroutine(WaitFrameEvent(evt));
            EB.Coroutines.Run(WaitFrameEvent(evt));
        }
    }

    public IEnumerator WaitFrameEvent(Action evt)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        if (evt != null)
        {
            evt();
        }
    }

    public void SetItemData(T[] itemData, int MaxDataCount)
    {
        if (itemData == null)
        {
            EB.Debug.LogError("DynamicUITableScroll SetItemData is Error! itemData is Null");
            return;
        }

        if (mDataItems == null || mDataItems.Length <= 0)
        {
            if (!IsForward)
            {
                    SetItemDatas(itemData);
            }
            else
            {
                mDataItems = itemData;
                if (activates == null || activates.Count < CellLimit)
                {
                    Retire();
                    Allocate();
                    Reposition();
                }
                RepositionActivate();
                mLastDataStartIndex = Mathf.Max(mDataItems.Length - CellLimit, 0);
                SetWidgetStu();
                Fill(0, mLastDataStartIndex, activates.Count);

                mTable.onRepositionOnce = () =>
                {
                    if (scrollView.verticalScrollBar != null)
                    {
                        placeholderWidget.enabled = false;
                        scrollView.verticalScrollBar.value = 1;
                        StartEvent(() => { placeholderWidget.enabled = false; scrollView.verticalScrollBar.value = 1; });
                    }
                };
                mTable.repositionNow = true;
            }
        }
        else
        {
            if (!IsForward)
            {
                List<T> tempData = new List<T>(itemData);
                tempData.AddRange(mDataItems);
                if (tempData.Count > MaxDataCount)
                {
                    int count = tempData.Count - MaxDataCount;
                    tempData.RemoveRange(tempData.Count - count, count);
                }
                SetItemDatas(tempData.ToArray());
            }
            else
            {
                List<T> tempData = new List<T>(mDataItems);
                tempData.AddRange(itemData);
                int removeCount = 0;
                if (tempData.Count > MaxDataCount)
                {
                    removeCount = tempData.Count - MaxDataCount;
                    tempData.RemoveRange(0, removeCount);
                }
                mDataItems = tempData.ToArray();

                if (activates.Count < CellLimit)
                {
                    Retire();
                    Allocate();
                    Reposition();

                    if (mDataItems.Length <= CellLimit)
                    {
                        mLastDataStartIndex = 0;
                        Fill(0, mLastDataStartIndex, mDataItems.Length);
                    }
                    else
                    {
                        RepositionActivate();
                        mLastDataStartIndex = mDataItems.Length - CellLimit;
                        Fill(0, mLastDataStartIndex, activates.Count);
                        mTable.repositionNow = true;
                    }

                    SetWidgetStu();
                    mTable.onRepositionOnce = () =>
                    {
                        if (scrollView.verticalScrollBar != null)
                        {
                            scrollView.verticalScrollBar.value = 1;
                            StartEvent(() => { scrollView.verticalScrollBar.value = 1; });
                        }
                    };
                    mTable.repositionNow = true;
                }
                else if ((mLastDataStartIndex >= mDataItems.Length - CellLimit - (itemData.Length - removeCount)) || (mLastDataStartIndex <= 0 || mLastDataStartIndex - itemData.Length <= 0))
                {
                    bool isLast = mLastDataStartIndex >= mDataItems.Length - CellLimit - (itemData.Length - removeCount);
                    mLastDataStartIndex = isLast ? mDataItems.Length - CellLimit : 0;
                    SetWidgetStu();
                    if (itemData.Length >= CellLimit)
                    {
                        RepositionActivate();
                        Fill(0, mLastDataStartIndex, activates.Count);
                        mTable.repositionNow = true;
                    }
                    else
                    {
                        List<Transform> trans = new List<Transform>();
                        int moveCount = isLast ? itemData.Length : removeCount;
                        for (int i = 0; i < moveCount; i++)
                        {
                            trans.Add(mTable.transform.GetChild(i));

                            int actIndex = mActivates.IndexOf(trans[i].gameObject);
                            if (actIndex >= 0)
                            {
                                int maxCount = isLast ? mDataItems.Length : CellLimit;
                                int refIndex = maxCount - moveCount + i;
                                Fill(actIndex, refIndex, 1);
                            }
                        }

                        for (int i = 0; i < trans.Count; i++)
                        {
                            trans[i].SetAsLastSibling();
                        }

                        mTable.onRepositionOnce = () =>
                        {
                            if (scrollView.verticalScrollBar != null)
                            {
                                scrollView.verticalScrollBar.value = isLast ? 1 : 0;
                                StartEvent(() => { scrollView.verticalScrollBar.value = isLast ? 1 : 0; });
                            }
                        };
                        mTable.repositionNow = true;
                    }
                }
                else
                {
                    mLastDataStartIndex = Mathf.Max(mLastDataStartIndex - itemData.Length, 0);
                    SetWidgetStu();
                }
            }
        }
    }
}

public abstract class DynamicUITableCellController<T> : DynamicCellController<T>
{
    public int Height;

    public Action<int> RefreshHeightEvent;

    protected abstract IEnumerator SetHeight();
}
}
