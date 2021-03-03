using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI {
    public abstract class DynamicScroll<T, U> : DynamicMonoHotfix, IHotfixUpdate where U : DynamicCellController<T> {
        public UIScrollView scrollView;
        public UIWidget placeholderWidget;
        public float thresholdFactor = 0.5f;
        public float padding;
        public int addition;
        public bool IsNoNeedForDelayFill = false;

        protected UIPanel mPanel = null;
        protected Vector2 mPanelSize = Vector2.zero;
        protected Vector2 mPanelOrigionClipOffset = Vector2.zero;

        protected Vector2 mLastScrollOffset = Vector2.zero;
        protected int mLastDataStartIndex = 0;
        protected int mActiveRefStartIndex = 0;
        protected int mActiveRefreshCount = 0;
        protected float mLastGridPosition = 0;
        protected int mMoveTargetIndex = 0;

        protected List<GameObject> mPool = new List<GameObject>();
        protected List<GameObject> mActivates = new List<GameObject>();
		public bool Dirty { get { return m_Dirty; } set {
				if (value)
					RegisterMonoUpdater();				
				else
					ErasureMonoUpdater();
				if(m_Dirty != value) m_Dirty = value;
			}
		}
		protected bool m_Dirty = false;

		protected bool mMoveDirty = false;
        protected bool mReposition = true;
        protected T[] mDataItems = new T[0];
        
		public override void OnEnable()
		{
			Dirty = true;
		}

		public void SetItemDatas(T[] data, bool force = true)
        {
	        if (data != null)
	        {
		        //EB.Debug.Log("Base.SetItemDatas => " + data.Length);
			}
	        else
			{
				EB.Debug.LogWarning("Base.SetItemDatas Error! ");
			}
			if (mPool.Count > 0 || force)
	            Dirty = true;
			else EB.Debug.LogWarning("Pool.Count Is 0!");
            mReposition |= data.Length != mDataItems.Length;
            mDataItems = data;
        }

        public List<GameObject> activates {
            get {
                return mActivates;
            }
        }

        public override void Awake()
        {
            base.Awake();
            scrollView = mDMono.transform.parent.parent.GetComponent<UIScrollView>();
            placeholderWidget = mDMono.transform.parent.GetComponent<UIWidget>();
            
            for (int i = 0; i < mDMono.transform.childCount; ++i) {
                Recycle(mDMono.transform.GetChild(i).gameObject);
            }
        }

        public override void Start() {
            mPanel = scrollView.GetComponent<UIPanel>();
            mPanelSize = mPanel.GetViewSize();
            mPanelOrigionClipOffset = mPanel.clipOffset;
            mLastScrollOffset = mPanelOrigionClipOffset;

            Dirty = true;
        }

        public virtual void Update() {
            if (Dirty) {
                if (mReposition) {
                    Retire();
                    Allocate();
                    Reposition();
                    Fill();

                    mReposition = false;
                } else {
                    Fill(0, mLastDataStartIndex, activates.Count);
                }
                Dirty = false;
                return;
            }

            if (mDataItems.Length <= activates.Count) {
                return;
            }
        }

        public void Clear() {
            if (scrollView != null) {
                if (scrollView.horizontalScrollBar != null) {
                    scrollView.horizontalScrollBar.value = 0;
                }
                if (scrollView.verticalScrollBar != null) {
                    scrollView.verticalScrollBar.value = 0;
                }
            }
            SetItemDatas(new T[0]);
            mReposition = true;
            Update();
        }

        public abstract int GetCellLimit();
        
        protected void Recycle(GameObject row) {
            U ctrl = row.GetMonoILRComponent<U>();
            ctrl.ViewIndex = -1;
            ctrl.DataIndex = -1;
            ctrl.Clean();
            row.CustomSetActive(false);
            mPool.Add(row);
            row.transform.SetAsFirstSibling();
        }

        protected GameObject Use() {
            if (mPool.Count == 0) {
				if (mActivates.Count <= 0) return null;
                GameObject row = GameObject.Instantiate<GameObject>(mActivates[0], mActivates[0].transform.parent);
                row.transform.localScale = mActivates[0].transform.localScale;
                row.transform.localRotation = mActivates[0].transform.localRotation;
                row.CustomSetActive(true);
                mActivates.Add(row);
                row.transform.SetAsLastSibling();
                return row;
            } else {
                int last = mPool.Count - 1;
                GameObject row = mPool[last];
                mPool.RemoveAt(last);
                row.CustomSetActive(true);
                mActivates.Add(row);
                row.transform.SetAsLastSibling();
                return row;
            }
        }

        protected void Retire() {
            for (int i = mActivates.Count - 1; i >= 0; --i) {
                GameObject row = mActivates[i];
                mActivates.RemoveAt(i);
                Recycle(row);
            }
        }

        protected virtual void Allocate()
        {
	        int CellLimit = GetCellLimit();
			int cnt = Mathf.Min(mDataItems.Length, CellLimit);

			for (int i = 0; i < cnt; ++i) {
                GameObject go = Use();
				if(go)
				{
					U ctrl = go.GetMonoILRComponent<U>();
					ctrl.ViewIndex = i;
				}
				else
				{
					EB.Debug.LogWarning("when you set {0} to {1},{1} start method Not execute!",typeof(T),this);
				}
            }
        }

        protected virtual void Reposition() {
            scrollView.ResetPosition();
        }

        protected void Fill() {
            mLastDataStartIndex = 0;
            Fill(0, mLastDataStartIndex, activates.Count);
        }

        public override void OnDestroy()
        {
            Dirty = false;
            SetScrollViewStatus(true);
            ILRTimerManager.instance.RemoveTimer(OnTimerUpToFill);
            base.OnDestroy();
        }

        private bool mIsFirstTimeToFill = true;
        //用来是否再次显示延时加载
        public void SetShowToFill(bool set)
        {
            mIsFirstTimeToFill = set;
        }
        private int mActiveStartIndex;
        private int mDataStartIndex;
        private int mFillIndex = 0;
        protected int mInitLength = 0;
        protected void Fill(int activeStartIndex, int dataStartIndex, int length) {
            if (mIsFirstTimeToFill && length > 0 && !IsNoNeedForDelayFill) {
                mActiveStartIndex = activeStartIndex;
                mDataStartIndex = dataStartIndex;
                mFillIndex = 0;

                for (int i = 0; i < mActivates.Count; i++) {
                    mActivates[i].CustomSetActive(false);
                }
                ILRTimerManager.instance.RemoveTimer(OnTimerUpToFill);
                ILRTimerManager.instance.AddTimer(120, length, OnTimerUpToFill);

                mInitLength = length;
                SetScrollViewStatus(false);//动态加载的时候不能拖动，加载完成时再设置成true

                mIsFirstTimeToFill = false;
            } else {
                for (int i = 0; i < length; ++i) {
                    FillItem(mActivates[activeStartIndex + i], mDataItems[dataStartIndex + i], dataStartIndex + i);
                }
            }
        }
        private void OnTimerUpToFill(int seq) {
            if (mActiveStartIndex + mFillIndex >= mActivates.Count || mDataStartIndex + mFillIndex >= mDataItems.Length) {
                SetScrollViewStatus(true);
                ILRTimerManager.instance.RemoveTimer(OnTimerUpToFill);
                return;
            }

            mActivates[mActiveStartIndex + mFillIndex].CustomSetActive(true);
            FillItem(mActivates[mActiveStartIndex + mFillIndex], mDataItems[mDataStartIndex + mFillIndex], mDataStartIndex + mFillIndex);
            mFillIndex++;

            //如果完全加载完之后在设置可拖动，体验不太好（因为在加载尾部item的时候界面上是看不到的），所以改成加载完三分之二就设置可拖动了。
            //  此时如果不是拖动过猛基本不会出问题，如果出问题了在增大这个判定的参数 
            if (mFillIndex >= mInitLength - (mInitLength / 3)) {
                SetScrollViewStatus(true);
            }
        }

        protected virtual void FillItem(GameObject itemGo, T itemData, int dataIndex) {
            U ctrl = itemGo.GetMonoILRComponent<U>();
            ctrl.DataIndex = dataIndex;
            if (itemData == null) {
                ctrl.Clean();
            } else {
                ctrl.Fill(itemData);
            }

            OnFilled(ctrl, itemData);
        }

        protected virtual void OnFilled(U ctrl, T itemData) {

        }

        public Vector2 GetClipOffset() {
            return mPanel.clipOffset - mPanelOrigionClipOffset;
        }

        private void SetScrollViewStatus(bool isShow) {
            if (scrollView != null && scrollView.enabled != isShow) {
                scrollView.enabled = isShow;
            }
        }
    }

    public abstract class DynamicCellController<T> : DynamicMonoHotfix {
        public int ViewIndex { get; set; }
        public int DataIndex { get; set; }
        public abstract void Fill(T itemData);
        public abstract void Clean();
    }

}