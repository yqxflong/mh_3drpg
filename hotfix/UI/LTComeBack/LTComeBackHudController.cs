using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTComeBackHudController : UIControllerHotfix
    {
        private GameObject LoginRP;
        private GameObject TaskRP;
        private GameObject GiftRP;

        private int mTimer = 0;
        private int overTime = 0;
        private UILabel DayLabel;
        private UILabel TimeLabel;

        private UITabController tabController;

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void Awake()
        {
            base.Awake();
            Transform t = controller.transform;
            UIButton backBtn = t.Find("FrameBG/Panel/CancelBtn").GetComponent<UIButton>();
            controller.backButton = backBtn;

            tabController = t.GetComponent<UITabController>("Content/ScrollView/Placeholder/ButtonList");
            LoginRP = t.Find("Content/ScrollView/Placeholder/ButtonList/1_login/Btn/RedPoint").gameObject;
            TaskRP = t.Find("Content/ScrollView/Placeholder/ButtonList/2_task/Btn/RedPoint").gameObject;
            GiftRP = t.Find("Content/ScrollView/Placeholder/ButtonList/3_gift/Btn/RedPoint").gameObject;
            DayLabel = t.GetComponent<UILabel>("Content/ViewList/BG/TimeLabel/DayLabel");
            TimeLabel = t.GetComponent<UILabel>("Content/ViewList/BG/TimeLabel/TimeLabel");
        }
        
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            ResetView();
        }

        private void ResetView()
        {
            if (tabController.TabLibPrefabs[2].TabObj.transform.parent.gameObject.activeSelf)
            {
                EB.IAP.Item[] tempArray = EB.Sparx.Hub.Instance.WalletManager.Payouts;
                bool hasGift = false;
                for (int i = 0; i < tempArray.Length; ++i)
                {
                    if (tempArray[i].category.CompareTo("comeback") == 0)
                    {
                        hasGift = true;
                        break;
                    }
                }
                if (!hasGift)
                {
                    //礼包按钮隐藏
                    tabController.TabLibPrefabs[2].TabObj.transform.parent.gameObject.CustomSetActive(false);
                    if (tabController.TabLibPrefabs[2].PressedTabObj.activeSelf)
                    {
                        tabController.SelectTab(0);
                    }
                    tabController.GetComponent<UIGrid>().repositionNow = true;
                }
            }
        }

        public override IEnumerator OnAddToStack()
        {
            Hotfix_LT.Messenger.Raise(EventName.LTComeBackHudOpen);
            if (overTime == 0)
            {
                overTime = LTWelfareModel.Instance.overTime;
            }
            LabelTimer();
            mTimer=ILRTimerManager.instance.AddTimer(1000, int.MaxValue, LabelTimer);


            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.cblogin, SetLoginRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.cbtask, SetTaskRP);

            LTComeBackEvent.ComeBackResetView += ResetView;

            return base.OnAddToStack();
        }

        private void SetLoginRP(RedPointNode node) { LoginRP.CustomSetActive(node.num > 0);}
        private void SetTaskRP(RedPointNode node) { TaskRP.CustomSetActive(node.num > 0); }

        public override void OnFocus()
        {
            base.OnFocus();
            if (LTComeBackEvent .ComeBackUpdata  != null)
            {
                LTComeBackEvent.ComeBackUpdata();
            }
            if (LTComeBackEvent.ComeBackOnfocus != null)
            {
                LTComeBackEvent.ComeBackOnfocus();
            }

            LTWelfareModel.Instance.ComeBack_Login();
            LTWelfareModel.Instance.ComeBack_Task();
        }

        private void LabelTimer(int timer=0)
        {
            int timeTemp = Mathf.Max(overTime - EB.Time.Now, 0);
            System.TimeSpan timeleft = new System.TimeSpan(0, 0, timeTemp);
            DayLabel.text = timeleft.Days.ToString();
            TimeLabel.text = string.Format(EB.Localizer.GetString("ID_COUNTDOWN_WITHOUT_DAY"), timeleft.Hours, timeleft.Minutes, timeleft.Seconds);
        }

        private void RemoveTimer()
        {
            if (mTimer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(mTimer);
                mTimer = 0;
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.cblogin, SetLoginRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.cbtask, SetTaskRP);
            LTComeBackEvent.ComeBackResetView -= ResetView;
            RemoveTimer();
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        public override void OnDestroy()
        {
            RemoveTimer();
            base.OnDestroy();
        }
    }

    //回归签到
    public class LTComeBackLoginAwardController : DynamicMonoHotfix
    {
        private List<LTWelfareSevenDayAwardItem> Items;
        private UILabel DayLabel;

        public override void Awake()
        {
            base.Awake();
            mDMono.transform.Find("BG").GetComponent<CampaignTextureCmp>().spriteName = "Welfare_Di_Qiridenglu";
            Items = new List<LTWelfareSevenDayAwardItem>();
            DayLabel = mDMono.transform.Find("BG/TotleDayTitle").GetComponent<UILabel>();
        }

        public override void Start()
        {
            EB.Coroutines.Run(InitData());
        }
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.LTComeBackHudOpen, InitItems);
            base.OnDestroy();
        }

        IEnumerator InitData()
        {
            yield return null;
            DynamicMonoILR[] items = mDMono.transform.Find("GiftGrid").GetComponentsInChildren<DynamicMonoILR>();
            for (int i = 0; i < items.Length; i++)
            {
                LTWelfareSevenDayAwardItem temp = items[i]._ilrObject as LTWelfareSevenDayAwardItem;
                if (temp != null)
                {
                    Items.Add(temp);
                    temp.mDMono.gameObject.CustomSetActive(false);
                }
            }
            InitItems();
            Hotfix_LT.Messenger.AddListener(EventName.LTComeBackHudOpen, InitItems);
        }

        int curIdc = 0;
        void InitItems()
        {
            if (curIdc == LTWelfareModel.Instance.ComeBackLoginDayCount) return;
            curIdc = LTWelfareModel.Instance.ComeBackLoginDayCount;
            DayLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_QIRI_LEIJI"), curIdc);
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(LTWelfareModel.ComeBackActivityID);
            for (int i = 0; i < Items.Count; ++i)
            {
                Hotfix_LT.Data.TimeLimitActivityStageTemplate data = FindStageTpl(stages, i + 1);
                List<LTShowItemData> itemDatas = new List<LTShowItemData>();
                eReceiveState state = eReceiveState.cannot;
                if (data != null && data.reward_items != null)
                {
                    for (int j = 0; j < data.reward_items.Count; j++)
                    {
                        string id = data.reward_items[j].id.ToString();
                        int count = data.reward_items[j].quantity;
                        string type = data.reward_items[j].type;
                        itemDatas.Add(new LTShowItemData(id, count, type, false));
                    }
                    if (i + 1 <= curIdc)
                    {
                        if (GetReceiveState(i + 1))
                            state = eReceiveState.have;
                        else
                            state = eReceiveState.can;
                    }
                    else
                        state = eReceiveState.cannot;

                }
                Items[i].SetUI(new RewardStageData(data.id, i + 1, itemDatas, state), LTWelfareSevenDayAwardItem.ItemType.ComeBack);
            }
        }

        private Hotfix_LT.Data.TimeLimitActivityStageTemplate FindStageTpl(List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> datas, int stage)
        {
            Hotfix_LT.Data.TimeLimitActivityStageTemplate data = new Hotfix_LT.Data.TimeLimitActivityStageTemplate();
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].stage == stage) { data = datas[i]; break; }
            }
            return data;
        }
        
        private bool GetReceiveState(int day)
        {
            bool state = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("user_prize_data.afterdays_login_reward.{0}{1:00}",LTWelfareModel.ComeBackActivityID, day), out state);
            return state;
        }

        public bool GetReceiveable()
        {
            int ldc = LTWelfareModel.Instance.ComeBackLoginDayCount;
            for (int day = 1; day < 8; ++day)
            {
                if (day <= ldc)
                {
                    if (!GetReceiveState(day))
                        return true;
                }
                else
                    return false;
            }
            return false;
        }

    }

    //回归任务
    public class LTComeBackTaskController : DynamicMonoHotfix
    {
        private List<LTComeBackTabItem> mDayList;
        private List<LTComeBackTaskItem> mTasksList;
        private GameObject AwardItem;
        private UIGrid AwardGrid;
        private GameObject DayItem;
        private UIGrid DayGrid;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            DayItem = t.Find("View1/Item").gameObject;
            DayGrid = t.GetComponent<UIGrid>("View1/ScrollView/Placeholder/Grid");
            AwardItem = t.Find("TaskView/Item").gameObject;
            AwardGrid = t.GetComponent<UIGrid>("TaskView/ScrollView/Placeholder/Grid");
            mDayList = new List<LTComeBackTabItem>();
            mTasksList = new List<LTComeBackTaskItem>();
            ComeBackUpdata();
        }

        public override void OnDestroy()
        {
            LTWelfareModel.Instance.ComeBackDayTabNum = -1;
            LTComeBackEvent.ComeBackTabClick -= ComeBackTabClick;
            LTComeBackEvent.ComeBackUpdata -= ComeBackUpdata;
            EB.Coroutines.Stop(InitData());
        }

        IEnumerator InitData()
        {
            yield return null;
            InitDayList();

            LTComeBackEvent.ComeBackTabClick += ComeBackTabClick;
            LTComeBackEvent.ComeBackUpdata += ComeBackUpdata;
            yield break;
        }

        private bool btnClick = false;
        private void ComeBackTabClick(int day)
        {
            if (LTWelfareModel.Instance.ComeBackDayTabNum == day || btnClick) return;
            btnClick = true;
            LTWelfareModel.Instance.ComeBackDayTabNum = day;
            for (int i = 0; i < mDayList.Count; i++)
            {
                mDayList[i].SetSelectColor(i == LTWelfareModel.Instance.ComeBackDayTabNum);
            }

            InitTasksList();
            btnClick = false;
        }
        
        private void InitDayList()
        {
            for (int i = 0; i < mDayList.Count; i++)
            {
                GameObject.Destroy(mDayList[i].mDMono.gameObject);
            }
            mDayList.Clear();
            
            int DayCount = LTWelfareModel.Instance.ComebackDayCount();
            for (int i = 0; i < DayCount; i++)
            {
                GameObject obj = GameObject.Instantiate(DayItem);
                obj.CustomSetActive(true);
                obj.transform.SetParent(DayGrid.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                LTComeBackTabItem levelCtrl = obj.transform .GetMonoILRComponent<LTComeBackTabItem>();
                if (LTWelfareModel.Instance.ComeBackDayTabNum == -1 && i < LTWelfareModel.Instance.ComeBackLoginDayCount && LTWelfareModel.Instance.ComeBackDayTabRPList.Contains(i))
                {
                    LTWelfareModel.Instance.ComeBackDayTabNum = i;
                }
                levelCtrl.InitData(i);
                mDayList.Add(levelCtrl);
            }
            DayGrid.enabled = true;
            DayGrid.Reposition();
            DayGrid.transform.parent.GetComponent<UIWidget>().height = (int)(DayCount * DayGrid.cellHeight);

            if (LTWelfareModel.Instance.ComeBackDayTabNum == -1)
            {
                LTWelfareModel.Instance.ComeBackDayTabNum = Mathf.Min(LTWelfareModel.Instance.ComeBackLoginDayCount, DayCount) - 1;
            }

            for (int i = 0; i < mDayList.Count; ++i)
            {
                mDayList[i].SetSelectColor(i == LTWelfareModel.Instance.ComeBackDayTabNum);
            }
            int activeCount = (int)(DayGrid.transform.parent.parent.GetComponent<UIPanel>().baseClipRegion.z / DayGrid.cellHeight);
            float value = (float)(LTWelfareModel.Instance.ComeBackDayTabNum + 1 - activeCount) / (float)(DayCount - activeCount);
            float scrollValue = LTWelfareModel.Instance.ComeBackDayTabNum + 1 > activeCount ? value : 0f;
            DayGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdatePosition();
            DayGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdateScrollbars();
            DayGrid.transform.parent.parent.GetComponent<UIScrollView>().verticalScrollBar.value = scrollValue;

            InitTasksList();
        }
        private void InitTasksList()
        {
            List<LTWelfareGrowUpTaskData> levelList = LTWelfareModel.Instance.GetComeBacksByDay(LTWelfareModel.Instance.ComeBackDayTabNum);
            for (int i = 0; i < levelList.Count; ++i)
            {
                if (i < mTasksList.Count)
                {
                    mTasksList[i].InitData(levelList[i]);
                    mTasksList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    GameObject obj = GameObject.Instantiate(AwardItem);
                    obj.CustomSetActive(true);
                    obj.transform.SetParent(AwardGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    LTComeBackTaskItem TaskCtrl = obj.transform .GetMonoILRComponent<LTComeBackTaskItem>();
                    TaskCtrl.InitData(levelList[i]);
                    mTasksList.Add(TaskCtrl);
                }
            }
            for (int i = levelList.Count; i < mTasksList.Count; ++i)
            {
                mTasksList[i].mDMono.gameObject.CustomSetActive(false);
            }
            AwardGrid.enabled = true;
            AwardGrid.Reposition();
            AwardGrid.transform.parent.GetComponent<UIWidget>().height = (int)(levelList.Count * AwardGrid.cellHeight);
            AwardGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdatePosition();
            AwardGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdateScrollbars();

        }

        private bool init = false;
        private void ComeBackUpdata()
        {
            LTWelfareModel.Instance.UpdataComeBackTasks();
            if (!init)
            {
                EB.Coroutines.Run(InitData());
                init = true;
            }
            else
            {
                for (int i = 0; i < mTasksList.Count; i++) mTasksList[i].ResetItem();
                if (mDayList.Count > LTWelfareModel.Instance.ComeBackDayTabNum || LTWelfareModel.Instance.ComeBackDayTabNum >= 0)
                {
                    for (int i = 0; i < mDayList.Count; i++)
                    {
                        mDayList[i].SetStates();
                    }
                }
            }
        }
    }
    public class LTComeBackTabItem : DynamicMonoHotfix
    {
        private UIButton DayBtn;
        private UISprite BGSprite;
        private UILabel DayLabel;
        private GameObject LockObj;
        private GameObject RedPointObj;
        private GameObject PrefectObj;

        private int index = -1;
        private bool isLock = false;

        public override void Awake()
        {
            base.Awake();
            DayBtn = mDMono.transform.GetComponent<UIButton>();
            DayBtn.onClick.Add(new EventDelegate(OnTabClick));
            BGSprite = mDMono.transform.Find("BG").GetComponent<UISprite>();
            DayLabel = mDMono.transform.Find("DayLabel").GetComponent<UILabel>();
            LockObj = mDMono.transform.Find("LockObj").gameObject;
            RedPointObj = mDMono.transform.Find("RedPoint").gameObject;
            PrefectObj = mDMono.transform.Find("Prefect").gameObject;
        }

        private void OnTabClick()
        {
            if (isLock)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_TARGET_LOCKED"));
                return;
            }
            if (LTComeBackEvent.ComeBackTabClick != null)
            {
                LTComeBackEvent.ComeBackTabClick(index);
            }
        }
        
        public void SetSelectColor(bool isSelect = false)
        {
            BGSprite.color = isSelect ? new Color(1, 1, 1) : new Color(1, 0, 1);
        }

        public void InitData(int i)
        {
            index = i;
            InitData();
        }
        private void InitData()
        {
            isLock = index >= LTWelfareModel .Instance.ComeBackLoginDayCount;
            DayLabel.text = string.Format(EB.Localizer.GetString("ID_DAY"), index + 1);
            LockObj.CustomSetActive(isLock);
            SetStates();
        }

        public void SetStates()
        {
            RedPointObj.CustomSetActive(index < LTWelfareModel .Instance.ComeBackLoginDayCount && LTWelfareModel.Instance.ComeBackDayTabRPList.Contains(index));
            PrefectObj.CustomSetActive(!LTWelfareModel.Instance.ComeBackDayTabUnPrefectList.Contains(index));
        }
    }
    public class LTComeBackTaskItem : DynamicMonoHotfix
    {
        private UILabel TitleLabel;
        private UIGrid ItemGrid;
        private List<LTShowItem> ItemList;
        private ConsecutiveClickCoolTrigger ReceiveBtn;

        private UILabel BtnLabel;
        private UILabel TipLabel;

        private bool sTaskOver = true;

        private LTWelfareGrowUpTaskData Data;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            TitleLabel = t.GetComponent<UILabel>("Label");
            ItemList = new List<LTShowItem>();
            ItemGrid = t.GetComponent<UIGrid>("GiftGrid");
            LTShowItem[] items = ItemGrid.transform.GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem");

            for (int i = 0; i < items.Length; i++)
            {
                ItemList.Add(items[i]);
            }
            
            ReceiveBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("GetButton");
            ReceiveBtn.clickEvent.Add(new EventDelegate(OnReceiveBtnClick));
            BtnLabel = t.GetComponent<UILabel>("GetButton/Label");
            TipLabel = t.GetComponent<UILabel>("TipLabel");
        }

        public void InitData(LTWelfareGrowUpTaskData data)
        {
            Data = data;
            if (data == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            Hotfix_LT.Data.TaskTemplate TaskTpl = data.TaskTpl;
            TitleLabel.text = data.TaskTpl.target_tips;
            TitleLabel.transform.GetComponent<UISprite>("LevelSprite").UpdateAnchors();
            List<LTShowItemData> rewardDatas = TaskStaticData.GetItemRewardList(Data.TaskId);
            for (int i = 0; i < ItemList.Count; i++)
            {
                if (Data.hasBattlePass && i == ItemList.Count - 1)
                {
                    List<LTShowItemData> battlePassList = Hotfix_LT.Data.TaskTemplateManager.Instance.GetBattlePassByTaskId(Data.TaskId);
                    var item = battlePassList[0];
                    ItemList[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else if (rewardDatas.Count > i)
                {
                    var item = rewardDatas[i];
                    ItemList[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                    ItemList[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    ItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            ItemGrid.Reposition();
            TipLabel.text = string.Format("{0}/{1}", data.Finished ? ("[42fe79]" + data.TargetNum) : data.CurNum.ToString(), data.TargetNum);
            SetReceiveBtn();

        }

        private void SetReceiveBtn()
        {
            if (Data != null && Data.State != null)
            {
                if (!LTWelfareModel.Instance.JudgeComeBackTaskOpenDay())//明日开放
                {
                    TipLabel.text = EB.Localizer.GetString("ID_OPEN_TOMORROW");
                    ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                    ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    ReceiveBtn.GetComponent<BoxCollider>().enabled = false;
                    if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");
                    return;
                }
                if (Data.Finished)//完成任务
                {
                    if (Data.State.Equals(TaskSystem.COMPLETED))//已领取奖励
                    {
                        ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                        ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                        ReceiveBtn.GetComponent<BoxCollider>().enabled = false;
                        if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                    }
                    else//可领取奖励
                    {
                        ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                        ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                        ReceiveBtn.GetComponent<BoxCollider>().enabled = true;
                        if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                    }
                }
                else//未完成任务
                {
                    ReceiveBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                    ReceiveBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    ReceiveBtn.GetComponent<BoxCollider>().enabled = true;
                    if (BtnLabel != null) BtnLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");
                }
            }
        }

        public void OnReceiveBtnClick()
        {
            if (Data.Finished)
            {
                OnCompleteBtnClick();
            }
            else
            {
                OnGoBtnClick();
            }
        }

        private void OnCompleteBtnClick()
        {

            if (EB.Time.Now > LTWelfareModel.Instance.overTime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_4620"));
                return;
            }

            if (!sTaskOver)
            {
                return;
            }
            sTaskOver = false;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            LTWelfareModel.Instance.RequestComplete(Data.TaskId.ToString(), delegate (bool success)
            {
                if (success)
                {
                    if (LTComeBackEvent.ComeBackUpdata != null)
                    {
                        LTComeBackEvent.ComeBackUpdata();
                    }

                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("0", TitleLabel.text);
                        MessageTemplateManager.ShowMessage(901036, ht, null);
                        Johny.HashtablePool.Release(ht);
                    }

                    System.Action callback = delegate ()
                    {
                        if (EB.Sparx.Hub.Instance.LevelRewardsManager.IsLevelUp)
                        {
                            LTMainHudManager.Instance.CheckLevelUp(delegate ()
                            {
                                sTaskOver = true;
                            });
                        }
                        else
                            sTaskOver = true;
                    };
                    List<LTShowItemData> ItemDatas = TaskStaticData.GetItemRewardList(Data.TaskId);
                    for (int i = 0; i < ItemDatas.Count; i++)
                    {
                        if (ItemDatas[i].id == "hc")
                            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, ItemDatas[i].count, "福利成长目标");
                        if (ItemDatas[i].id == "gold")
                            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, ItemDatas[i].count, "福利成长目标");
                    }

                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("reward", ItemDatas);
                        ht.Add("callback", callback);
                        GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                    }
                }
                sTaskOver = true;
            });
        }

        private void OnGoBtnClick()
        {
            TaskSystem.ProcessTaskRunning(Data.TaskId.ToString());
        }

        public void ResetItem()
        {
            InitData(LTWelfareModel.Instance.FindComebackDataById(Data.TaskId));
        }
    }

    //回归礼包
    public class LTComeBackGiftController : DynamicMonoHotfix
    {
        private LTComeBackGiftItem[] Items;
        private UIGrid uIGrid;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            Items = new LTComeBackGiftItem[3];
            Items[0] =t.GetMonoILRComponent <LTComeBackGiftItem>("Grid/Item");
            Items[1] = t.GetMonoILRComponent<LTComeBackGiftItem>("Grid/Item (1)");
            Items[2] = t.GetMonoILRComponent<LTComeBackGiftItem>("Grid/Item (2)");
            uIGrid = t.GetComponent<UIGrid>("Grid");

            InitItems();

            LTComeBackEvent.ComeBackOnfocus += InitItems;
            Hotfix_LT.Messenger.AddListener<EB.IAP.Item, EB.IAP.Transaction>(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, OnOfferPurchaseSuceeded);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LTComeBackEvent.ComeBackOnfocus -= InitItems;
            Hotfix_LT.Messenger.RemoveListener<EB.IAP.Item, EB.IAP.Transaction>(Hotfix_LT.EventName.OnOfferPurchaseSuceeded, OnOfferPurchaseSuceeded);
        }

        private void InitItems()
        {
            EB.IAP.Item[] tempArray = EB.Sparx.Hub.Instance.WalletManager.Payouts;
            int count = 3;
            EB.IAP.Item[] temps = new EB.IAP.Item[count];
            int index = 0;
            for (int i = 0; i < tempArray.Length; ++i)
            {
                if(tempArray[i].category.CompareTo("comeback") == 0)
                {
                    temps[index] = tempArray[i];
                    index++;
                    if (count == index) break;
                }
            }

            if (index==0)
            {
               if(LTComeBackEvent.ComeBackResetView != null)
                {
                    LTComeBackEvent.ComeBackResetView();
                    return;
                }
            }

            for(int i=0;i< Items.Length; ++i)
            {
                if (i < index)
                {
                    Items[i].InitItem(temps[i]);
                }
                else
                {
                    Items[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
            uIGrid.Reposition();
        }

        /// <summary>
        /// 商城礼包购买成功调用
        /// </summary>
        /// <param name="item"></param>
        private void OnOfferPurchaseSuceeded(EB.IAP.Item item, EB.IAP.Transaction trans)
        {
            if (item.category.CompareTo("comeback") != 0) return;//非回归礼包return;

            EB.Debug.Log("OnOfferPurchaseSuceeded");
            List<LTShowItemData> itemDataList = new List<LTShowItemData>();
            for (int i = 0; i < item.redeemers.Count; i++)
            {
                if (!LTChargeManager.Instance.IgnoreItemList.Contains(item.redeemers[i].Type))
                {
                    LTShowItemData tempData = new LTShowItemData(item.redeemers[i].Data, item.redeemers[i].Quantity, item.redeemers[i].Type);
                    itemDataList.Add(tempData);
                }
            }
            if (itemDataList.Count > 0) GlobalMenuManager.Instance.Open("LTShowRewardView", itemDataList);

            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHARGE_PAY_SUCC"));

            LTChargeManager.Instance.ReflashLimitedTimeGiftInfo(false, true);
        }

    }
    public class LTComeBackGiftItem : DynamicMonoHotfix
    {
        private UILabel NameLabel;
        private UILabel TimesLabel;

        private LTShowItem[] ShowItems;

        private UIButton Btn;

        private EB.IAP.Item curChargeData;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            NameLabel = t.GetComponent<UILabel>("Name");
            TimesLabel = t.GetComponent<UILabel>("Times/Label");
            Btn = t.GetComponent<UIButton>("ReceiveButton");
            Btn.onClick.Add(new EventDelegate(OnReceiveBtnClick));
            ShowItems = new LTShowItem[4];
            ShowItems[0] = t.GetMonoILRComponent<LTShowItem>("GiftGrid/LTShowItem");
            ShowItems[1] = t.GetMonoILRComponent<LTShowItem>("GiftGrid/LTShowItem (1)");
            ShowItems[2] = t.GetMonoILRComponent<LTShowItem>("GiftGrid/LTShowItem (2)");
            ShowItems[3] = t.GetMonoILRComponent<LTShowItem>("GiftGrid/LTShowItem (3)");
        }

        public void InitItem(EB.IAP.Item item)
        {
            if (item != null)
            {
                curChargeData = item;
                NameLabel.text = curChargeData.longName;
                UILabel uILabel = Btn.transform.GetComponent<UILabel>("Label");
                uILabel.text = curChargeData.localizedCost;
                for (int i = 0; i < ShowItems.Length; ++i)
                {
                    if (i < curChargeData.redeemers.Count)
                    {
                        if (!LTChargeManager.Instance.IgnoreItemList.Contains(curChargeData.redeemers[i].Type))
                        {
                            LTShowItemData tempData = new LTShowItemData(curChargeData.redeemers[i].Data, curChargeData.redeemers[i].Quantity, curChargeData.redeemers[i].Type, false);
                            ShowItems[i].LTItemData = tempData;
                            ShowItems[i].mDMono.gameObject.CustomSetActive(true);
                        }
                    }
                    else
                    {
                        ShowItems[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
                TimesLabel.text = curChargeData.buyLimit.ToString();
                mDMono.gameObject.CustomSetActive(true);
            }
            else
            {
                mDMono.gameObject.CustomSetActive(false);
            }
        }

        private void OnReceiveBtnClick()
        {
            if (EB .Time .Now > LTWelfareModel.Instance.overTime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FinalPlayerItem_4620"));
                return;
            }
            LTChargeManager.Instance.PurchaseOfferExpand(curChargeData, LTChargeStoreController.EventTable);
        }
    }
}
