using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    //public class LegionSearchMessageCallBack : GameEvent
    //{
    //    // index: =1 搜索军团列表，=2 屏蔽军团搜索ui的所有点击事件, =3 恢复军团搜索ui的所有点击事件
    //    public int index = 0;

    //    public LegionSearchMessageCallBack(int ind)
    //    {
    //        index = ind;
    //    }
    //}
    public class LegionSearchHudController : UIControllerHotfix
    {


        public override void Awake()
        {      
            base.Awake();

            searchLegionBtn = controller.transform.Find("Bottom/SearchButton").GetComponent<UIButton>();
            quickJoinBtn = controller.transform.Find("Bottom/QuickJoinButton").GetComponent<UIButton>();
            createLegionBtn = controller.transform.Find("Bottom/CreateButton").GetComponent<UIButton>();
            searchInput = controller.transform.Find("Bottom/Input/InputLabel").GetComponent<UIInput>();
            scrollView = controller.transform.Find("Center/Scroll View").GetComponent<UIScrollView>();
            grid = controller.transform.Find("Center/Scroll View/holder/Grid").GetComponent<UIGrid>();
            searchItemSearchScroll = controller.transform.Find("Center/Scroll View/holder/Grid").GetMonoILRComponent<LegionSearchScroll>();
            legionCreateView = controller.transform.Find("LTLegionNameEditView").GetMonoILRComponent<LegionCreateView>();
            controller.backButton = controller.transform.Find("LeftTop/CancelBtn").GetComponent<UIButton>();
            if (searchLegionBtn != null)
            {
                searchLegionBtn.onClick.Add(new EventDelegate(OnClickSearchGroup));
            }
            if (quickJoinBtn != null)
            {
                quickJoinBtn.onClick.Add(new EventDelegate(OnClickQuickJoin));
            }
            if (createLegionBtn != null)
            {
                createLegionBtn.onClick.Add(new EventDelegate(OnClickCreateGroup));
            }

            if (legionCreateView != null)
            {
                //legionCreateView.onClickChangeLegionIcon += OnClickChangeLegionIcon;
                legionCreateView.onClickSendCreateLegion += OnClickSendCreateLegion;
            }

            //if(legionChangeIconView!=null)
            //{
            //    legionChangeIconView.confirmIconAction += OnConfirmLegionIcon;
            //    legionChangeIconView.gameObject.SetActive(false);
            //}

            if (searchInput != null)
            {
                searchNormalStr = "";
            }
        }


        public UIButton searchLegionBtn;

        public UIButton quickJoinBtn;

        public UIButton createLegionBtn;

        /// <summary>
        /// 搜索文字显示
        /// </summary>
        public UIInput searchInput;

        public UIScrollView scrollView;

        public UIGrid grid;

        //public LegionSearchItem searchItemTemplate;
        public LegionSearchScroll searchItemSearchScroll;

        public List<LegionSearchItem> searchItems;

        public LegionCreateView legionCreateView;

        private string searchNormalStr;

        public static bool isOpen = false;



        public override void OnDestroy()
        {
            if (searchLegionBtn != null)
            {
                searchLegionBtn.onClick.Clear();
            }
            if (quickJoinBtn != null)
            {
                quickJoinBtn.onClick.Clear();
            }
            if (createLegionBtn != null)
            {
                createLegionBtn.onClick.Clear();
            }
            if (legionCreateView != null)
            {
                //legionCreateView.onClickChangeLegionIcon -= OnClickChangeLegionIcon;
                legionCreateView.onClickSendCreateLegion -= OnClickSendCreateLegion;
            }
            //if (legionChangeIconView != null)
            //{
            //    legionChangeIconView.confirmIconAction -= OnConfirmLegionIcon;
            //}

            base.OnDestroy();
        }
        /// <summary>
        /// 打开后传参
        /// </summary>
        /// <param name="param"></param>
        public override void SetMenuData(object param)
        {
            LegionEvent.CloseLegionHudUI += controller.Close;
            LegionEvent.NotifyLegionAccount += OnLegionAccount;
            LegionEvent.NotifyUpdateSearchItemDatas += SetData;
            Hotfix_LT.Messenger.AddListener<int>(Hotfix_LT.EventName.LegionSearchMessageCallBack, LegionSearchMessageCallBackFucc);            
            if (LegionModel.GetInstance().searchItemDatas != null)
            {
                SetData(LegionModel.GetInstance().searchItemDatas);
            }     
            //要是当前的创建军团界面没有关掉的就先关掉，因为界面会缓存上一次界面情况
            if (legionCreateView != null)
            {
                legionCreateView.mDMono.gameObject.SetActive(false);
            }
        }

        public void SetData(SearchItemData[] searchItemDatas)
        {
            searchItemSearchScroll.SetItemDatas(searchItemDatas);
        }


        public override bool IsFullscreen()
        {
            return true;
        }

        public override IEnumerator OnAddToStack()
        {
            isOpen = true;
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LegionEvent.CloseLegionHudUI -= controller.Close;
            LegionEvent.NotifyLegionAccount -= OnLegionAccount;
            LegionEvent.NotifyUpdateSearchItemDatas -= SetData;
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.LegionSearchMessageCallBack, LegionSearchMessageCallBackFucc);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.LegionSearchMessageCallBack, 3);
            isOpen = false;
            DestroySelf();
            yield break;
        }

        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        private void LegionSearchMessageCallBackFucc(int evt)
        {
            // 如果ExceptionFun不为空则说明有其他的地方向服务器请消息，此时不需要查找
            if (evt == 1 && searchNormalStr != null && LTHotfixApi.GetInstance().ExceptionFunc == null)
            {
            }

            if (evt == 2)
            {
                waitTime = 10;
                waitSearchTime = Time.time;
            }
        }

        private float waitSearchTime;//屏蔽快速点击
        private float waitTime = 1;
        private bool isCouldClick()
        {
            if (Time.time - waitSearchTime < waitTime)
            {
                return false;
            }
            waitTime = 1;
            waitSearchTime = Time.time;
            return true;
        }

        public void OnClickSearchGroup()
        {
            if (!isCouldClick()) return;

            if (LegionEvent.SearchLegion != null && searchInput != null
                && !searchInput.value.Equals(searchNormalStr))
            {
                searchNormalStr = searchInput.value;
                LegionEvent.SearchLegion(searchInput.value);
            }
            else if (searchInput.value.Equals(searchNormalStr))
            {
                searchNormalStr = "";
                LegionEvent.SearchLegion("");
            }

        }

        public void OnClickQuickJoin()
        {
            if (!isCouldClick()) return;

            if (LegionEvent.SearchQuickJoinLegion != null)
            {
                LegionEvent.SearchQuickJoinLegion();
            }
        }

        public void OnClickCreateGroup()
        {
            legionCreateView.OpenUI();
        }

        void OnClickSendCreateLegion(string createName, int iconID)
        {
            if (LegionEvent.SendCreateLegionMsg != null)
            {
                LegionEvent.SendCreateLegionMsg(createName, iconID);
            }
        }

        void OnItemApply(int legionID, bool isApply)
        {
            if (isApply && LegionEvent.SendApplyJoinLegion != null)
            {
                LegionEvent.SendApplyJoinLegion(legionID);
            }
            else if (!isApply && LegionEvent.SendCancelApplyJoinLegion != null)
            {
                LegionEvent.SendCancelApplyJoinLegion(legionID);
            }
        }

        void OnLegionAccount(AllianceAccount data)
        {
            if (data.State == eAllianceState.Joined)
            {
                controller.Close();
                if (LegionEvent.LegionShowUI != null)
                {
                    LegionEvent.LegionShowUI();
                }
            }
        }
    }
}

