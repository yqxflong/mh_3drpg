using System;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class ShowRewardScrollFunc
    {
        public static System.Action<int> shouldScroll;
    }

    public class LTShowRewardCtrl : UIControllerHotfix, IHotfixUpdate
    {
        public WaitForSeconds WaitTime = new WaitForSeconds(0.15f);
        public UIWidget PlaceholderWidget;
        public UIScrollView ScrollVew;
        public UIDynamicShowItem m_UIDynamicShowItem;
        public GameObject ClickTips;
        private List<LTShowItemData> mItemDataList;
        private Queue<List<LTShowItemData>> mNextItemDataQueue;
        private Action mCallback;
        private bool showBtn = false;
        private bool btnClickLimit = false;
        private int showTime = 0;
        public GameObject fx;
        public GameObject BlurBG;
        public override bool IsFullscreen()
        {
            return true;
        }

        bool isPlayAnimFinished;

        public GameObject ClickBtn;
        public GameObject titleObj;
        public UILabel NegativeLabel;
        public UILabel PositiveLabel;
        public UILabel PositiveCostLabel;
        public DynamicUISprite PositiveSprite;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            PlaceholderWidget = t.GetComponent<UIWidget>("Content/Reward/Placeholder");
            ScrollVew = t.GetComponent<UIScrollView>("Content/Reward");
            m_UIDynamicShowItem = t.GetMonoILRComponent<UIDynamicShowItem>("Content/Reward/Placeholder/RewardGrid");
            m_UIDynamicShowItem.OnShowFinished = TweenFinishCallBack;
            ClickTips = t.FindEx("Content/Tip").gameObject;
            BlurBG = t.FindEx("Bg").gameObject;
            BlurBG.CustomSetActive(true);
            fx = t.FindEx("Bg/FX").gameObject;
            ClickBtn = t.FindEx("Content/ClickBtns").gameObject;
            titleObj = t.FindEx("Content/Title").gameObject;
            NegativeLabel = t.GetComponent<UILabel>("Content/ClickBtns/NegativeBtn/Label");
            PositiveLabel = t.GetComponent<UILabel>("Content/ClickBtns/PositiveBtn/Label");
            PositiveCostLabel = t.GetComponent<UILabel>("Content/ClickBtns/PositiveBtn/CoinLabel");
            PositiveSprite = t.GetComponent<DynamicUISprite>("Content/ClickBtns/PositiveBtn/CoinLabel/Sprite");

            t.GetComponent<UIEventTrigger>("Bg").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/ClickBtns/NegativeBtn").clickEvent.Add(new EventDelegate(OnNegativeBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Content/ClickBtns/PositiveBtn").clickEvent.Add(new EventDelegate(OnPositiveBtnClick));

            mNextItemDataQueue = new Queue<List<LTShowItemData>>();
        }

        public override void SetMenuData(object param)
        {
            InputBlockerManager .Instance .Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);

            if (mItemDataList != null)
            {
                List<LTShowItemData> temp = param as List<LTShowItemData>;
                if (temp == null)
                {
                    Hashtable ht = param as Hashtable;
                    temp = (ht["reward"] != null) ? ht["reward"] as List<LTShowItemData> : new List<LTShowItemData>();
                    mCallback = (ht["callback"] != null) ? ht["callback"] as Action : null;
                }
                mNextItemDataQueue.Enqueue(temp);
                return;
            }

            showBtn = false;
            mCallback = null;
            btnClickLimit = false;
            isPlayAnimFinished = false;
            showTime = EB.Time.Now;

            mItemDataList = param as List<LTShowItemData>;

            ClickBtn.CustomSetActive(false);
            ClickTips.gameObject.CustomSetActive(false);
            PlaceholderWidget.GetComponent<BoxCollider>().enabled = false;

            if (mItemDataList == null)
            {
                Hashtable ht = param as Hashtable;
                mItemDataList = (ht["reward"] != null) ? ht["reward"] as List<LTShowItemData> : new List<LTShowItemData>();
                mCallback = (ht["callback"] != null) ? ht["callback"] as Action : null;
                showBtn = (ht["showBtn"] != null) ? (bool)ht["showBtn"] : false;
                if (showBtn)
                {
                    btnClickLimit = true;
                    PositiveBtnAction = ht["positive"] as System.Action;
                    PositiveDesc = Convert.ToString(ht["positiveDesc"]);
                    mCostType = Convert.ToString(ht["costType"]);
                    mCostNum = (int)ht["costNum"];
                    NegativeBtnAction = ht["negative"] as System.Action;
                    NegativeDesc = Convert.ToString(ht["negativeDesc"]);
                }
            }

            ProcessItemData();
        }

        private bool m_guideToolState = false;
        public override IEnumerator OnAddToStack()
        {
            Messenger.AddListener(Hotfix_LT.EventName.InventoryEvent, OnInventoryEvent);
            fx.CustomSetActive(true);
            yield return base.OnAddToStack();

            if (MengBanController.Instance.controller.gameObject.activeSelf)
            {
                m_guideToolState = true;
                MengBanController.Instance.controller.transform.parent.gameObject.CustomSetActive(false);
            }

            yield return null;
            titleObj.CustomSetActive(true);
            curScrollRow = 1;
            ShowRewardScrollFunc.shouldScroll += ScrollFunc;
            FusionAudio.PostEvent("UI/ShowReward");
            InitUI();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            if (m_guideToolState)
            {
                m_guideToolState = false;

                if (MengBanController.Instance != null)
                {
                    MengBanController.Instance.controller.transform.parent.gameObject.CustomSetActive(true);
                }
            }
            mItemDataList = null;
            ShowAwards = null;
            mNextItemDataQueue.Clear();

            Messenger.RemoveListener(Hotfix_LT.EventName.InventoryEvent, OnInventoryEvent);
            StopAllCoroutines();
            yield return null;

            if (titleObj != null)
            {
                titleObj.CustomSetActive(false);
            }

            ShowRewardScrollFunc.shouldScroll -= ScrollFunc;

            if (mCallback != null)
            {
                mCallback();
            }
            else if (LTMainHudManager.Instance != null)
            {
                LTMainHudManager.Instance.CheckLevelUp();
            }

            if (showBtn)
            {
                //装备数量发生变化需要通知发送下
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
            }

            fx.CustomSetActive(false);
            DestroySelf();

            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();
        }

        private int row;
        private float scrollValue;
        private bool isInit;

        private void InitUI()
        {
            row = (mItemDataList.Count - 1) / 5 + 1;
            PlaceholderWidget.height = row * 300;
            scrollValue = Mathf.Clamp(1f / (float)(row - 2), 0, 1);
            ScrollVew.verticalScrollBar.value = (row == 1) ? 0.5f : 0f;
            StartCoroutine(PlayTween(OnTweenFinished));
            isInit = true;
        }

        private IEnumerator PlayTween(System.Action callback)
        {
            yield return WaitTime;
            callback?.Invoke();
        }

        /// <summary>
        /// 对数据做加工处理（屏蔽掉一些不用显示的物品）
        /// </summary>
        private void ProcessItemData()
        {
            if (mItemDataList != null)
            {
                List<LTShowItemData> AddList = new List<LTShowItemData>();

                for (int i = mItemDataList.Count - 1; i >= 0; i--)
                {
                    if (LTChargeManager.Instance.IgnoreItemList.Contains(mItemDataList[i].type))
                    {
                        if (mItemDataList[i].type.Equals(LTShowItemType.TYPE_ACTIVITY) || mItemDataList[i].type.Equals(LTShowItemType.TYPE_HEROMEDAL))
                        {
                            //可显示，不处理
                        }
                        else
                        {
                            mItemDataList.RemoveAt(i);
                        }
                    }
                    else if (mItemDataList[i].type.Equals(LTShowItemType.TYPE_HERO))
                    {
                        if (ShowAwards == null)
                        {
                            ShowAwards = new Queue();
                        }

                        ShowAwards.Enqueue(mItemDataList[i]);
                    }
                    else if (mItemDataList[i].type.Equals("res") && mItemDataList[i].count < 0)
                    {
                        mItemDataList.Remove(mItemDataList[i]);
                    }
                    else
                    {
                        //装备需要做不叠加显示（服务器下发的是叠加的）
                        if (mItemDataList[i].count > 1)
                        {
                            var tpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(mItemDataList[i].id);

                            if (tpl != null && tpl is Hotfix_LT.Data.EquipmentItemTemplate)
                            {
                                for (int j = 0; j < mItemDataList[i].count; ++j)
                                {
                                    LTShowItemData newData = new LTShowItemData(mItemDataList[i].id, 1, mItemDataList[i].type, isFromWish: mItemDataList[i].isFromWish);
                                    AddList.Add(newData);
                                }
                                mItemDataList.Remove(mItemDataList[i]);
                            }
                        }
                    }
                }

                if (AddList.Count > 0)
                {
                    mItemDataList.AddRange(AddList);
                }
            }
        }

        /// <summary>
        /// 获得物品表现结束
        /// </summary>
        private void OnTweenFinished()
        {
            m_UIDynamicShowItem.Clear();
            m_UIDynamicShowItem.ShowItems(mItemDataList);
        }

        private void TweenFinishCallBack()
        {
            if (ShowAwards != null && ShowAwards.Count > 0)
            {
                LTPartnerDataManager.Instance.InitPartnerData();
                Messenger.Raise(Hotfix_LT.EventName.InventoryEvent);
            }

            //仅仅调用物品获得的系统消息提示，飘字提示干掉
            if (mItemDataList != null)
            {
                GameUtils.ShowAwardMsgOnlySys(mItemDataList);
            }

            if (mNextItemDataQueue!=null&& mNextItemDataQueue.Count > 0)
            {
                curScrollRow = 1;
                StopAllCoroutines();
                mItemDataList = mNextItemDataQueue.Dequeue();
                ProcessItemData();
                InitUI();
                return;
            }

                isPlayAnimFinished = true;
            ClickTips.gameObject.CustomSetActive(!showBtn);
            ClickBtn.CustomSetActive(showBtn);

            if (showBtn)
            {
                NegativeLabel.text = NegativeDesc;
                PositiveLabel.text = PositiveDesc;
                PositiveSprite.spriteName = BalanceResourceUtil.GetResIcon(mCostType, 0);
                PositiveCostLabel.text = mCostNum.ToString();
                PositiveCostLabel.color = BalanceResourceUtil.GetUserDiamond() >= mCostNum ? LT.Hotfix.Utility.ColorUtility.WhiteColor : LT.Hotfix.Utility.ColorUtility.RedColor;
            }

            if (mItemDataList.Count >= 11)
            {
                PlaceholderWidget.GetComponent<BoxCollider>().enabled = true;
            }

            if (row == 1)
            {
                ScrollVew.verticalScrollBar.value = 0.5f;
            }
        }

        private int curScrollRow = 1;

        protected bool CanScroll
        {
            get { return m_CanScroll; }
            set
            {
                if (value) RegisterMonoUpdater(); else ErasureMonoUpdater();
                if (value != m_CanScroll) m_CanScroll = value;
            }
        }
        private bool m_CanScroll = false;

        private void ScrollFunc(int evt)//2时才动作
        {
            if (isInit) {
                ScrollVew.verticalScrollBar.value = row == 1 ? 0.5f : 0f;
                isInit = false;
            }

            if (evt <= curScrollRow || isPlayAnimFinished) {
                return; 
            }

            curScrollRow = evt;
            CanScroll = true;
        }

        public void Update()
        {
            if (CanScroll)
            {
                if (ScrollVew.verticalScrollBar.value < scrollValue * (curScrollRow - 1) && ScrollVew.verticalScrollBar.value < 1)
                {
                    ScrollVew.verticalScrollBar.value += Time.deltaTime;
                }
                else
                {
                    CanScroll = false;
                }
            }
        }

        public override void OnCancelButtonClick()
        {
            if (!isPlayAnimFinished || btnClickLimit)
            {
                //最多限制15秒，如果超过将不在阻塞
                if (EB.Time.Now - showTime <= 15) return;
            }

            PlaceholderWidget.height = 300;
            m_UIDynamicShowItem.Clear();

            base.OnCancelButtonClick();
            // GlobalMenuManager.Instance.Open("LTShowReelView");
        }

        private System.Action PositiveBtnAction;
        private string PositiveDesc;
        private string mCostType;
        private int mCostNum;

        public void OnPositiveBtnClick()
        {
            int own = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("res.{0}.v", mCostType), out own);

            if (own < mCostNum)
            {
                BalanceResourceUtil.ResLessMessage(mCostType);
                return;
            }

            if (!btnClickLimit)
            {
                return;
            }

            btnClickLimit = false;

            if (PositiveBtnAction != null)
            {
                PositiveBtnAction();
            }

            OnCancelButtonClick();
        }

        private System.Action NegativeBtnAction;
        private string NegativeDesc;

        public void OnNegativeBtnClick()
        {
            if (!btnClickLimit)
            {
                return;
            }

            btnClickLimit = false;

            if (NegativeBtnAction != null)
            {
                NegativeBtnAction();
            }

            OnCancelButtonClick();
        }

        private Queue ShowAwards;
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
    }
}
