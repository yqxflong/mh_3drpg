using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTDailyHudController : UIControllerHotfix
    {
        public override bool IsFullscreen()
        {
            return true;
        }

        public LTDailyDynamicGrid LTDailyDG;
    
        public LTShowItem[] RewardItems;
        public LTDailyCell FirstCell;
    
        private UILabel GetVitBtn2TimeLabel;
    
        public TitleListController titleCon;
    
    
        private EDailyType curDailyType;
        private LTDailyData curDailyData;
        private Hotfix_LT.Data.SpecialActivityTemplate curActData;
        private LTDailyCell curDailyCell;
    
        private List<LTDailyData> mDailyLimitData;
        private List<LTDailyData> mDailyAllDayData;
        private List<int> GetVitActIDList = new List<int>() { 9013 };
        private int curFuncGuideId = 0;
    
        private int eventSequence = 0;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LTDailyDG = t.GetMonoILRComponent<LTDailyDynamicGrid>("DailyContent/List/ScrollView/Placeholder/Grid");
            RewardItems = new LTShowItem[controller.UiGrids["RewardGrid"].transform.childCount];

            for (var i = 0; i < controller.UiGrids["RewardGrid"].transform.childCount; i++)
	            RewardItems[i] = controller.UiGrids["RewardGrid"].transform.GetChild(i).GetMonoILRComponent<LTShowItem>();

            FirstCell = t.GetMonoILRComponent<LTDailyCell>("DailyContent/List/ScrollView/Placeholder/Grid/Item");
            titleCon = t.GetMonoILRComponent<TitleListController>("DailyContent/Frame/UpButtons/Title");
            controller.backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");

			controller.FindAndBindingBtnEvent(new List<string>(3){ "DailyContent/Content/GotoBtn", "DailyContent/Frame/UpButtons/Title/BtnList/LimitBtn",
				"DailyContent/Frame/UpButtons/Title/BtnList/AllDayBtn" }, new List<EventDelegate>(3){ new EventDelegate(OnGotoBtnClick),
				new EventDelegate(OnLimitTagClick), new EventDelegate(OnAllDayTagClick) });

			controller.BindingCoolTriggerEvent(new List<string>(2){ "GetVitBtn", "GetVitBtn2" }, new List<EventDelegate>(2) {
				new EventDelegate(OnGetVitBtnClick), new EventDelegate(OnGetVitBtn2Click)
			});

			controller.CoolTriggers["GetVitBtn2"].transform.Find("CostLabel").GetComponent<UILabel>().text = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("BuyDailyVigorCost").ToString();
            GetVitBtn2TimeLabel = controller.CoolTriggers["GetVitBtn2"].transform.Find("Label").GetComponent<UILabel>();
        }
    
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
    
            curDailyType = EDailyType.Limit;
            if (param != null)
            {
                curDailyType = EDailyType.AllDay;
                curFuncGuideId = int.Parse((string)param);
            }
            else
            {
                if (LTDailyDataManager.Instance.CurChoseData != null)
                {
                    curDailyType = LTDailyDataManager.Instance.CurChoseData.DailyType;
                    curFuncGuideId = LTDailyDataManager.Instance.CurChoseData.ActivityData.id;
                }
            }
            titleCon.SetTitleBtn((int)curDailyType);
            Init();
        }
    
        public override void OnFocus()
        {
            base.OnFocus();
            ResetRP();
        }
    
        private void ResetRP()
        {
            LTDailyDataManager.Instance.IsCouldReceiveVit(true);
            LTDailyDataManager.Instance.Daily_Daily();
        }
    
        public override IEnumerator OnAddToStack()
        {
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst .dailyact ,SetDailyRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.limitact, SetLimitRP);
            return base.OnAddToStack();
        }
    
        private void SetDailyRP(RedPointNode node)
        {
            controller.GObjects["DailyAllDayRP"].CustomSetActive(node.num > 0);
        }

        private void SetLimitRP(RedPointNode node)
        {
            controller.GObjects["DailyLimitRP"].CustomSetActive(node.num > 0);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            if (eventSequence != 0)
            {
                ILRTimerManager.instance.RemoveTimer(eventSequence);
            }
            if (curDailyCell != null)
            {
                curDailyData.IsSelected = false;
                curDailyCell.SetSelectStatus(false);
            }
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack (RedPointConst.dailyact, SetDailyRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.limitact, SetLimitRP);
            DestroySelf();
            yield break;
        }
    
        private void RefreshVigor()
        {
            if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.VigorUpdate))
            {
                bool b = false;
                int count = 0;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.daily_login.is_draw_daily_vigor1", out b);
                count += b ? 0 : 1;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.daily_login.is_draw_daily_vigor2", out b);
                count += b ? 0 : 1;
                int curCount = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.daily_login.buy_daily_times", out curCount);
    
                DataLookupsCache.Instance.CacheData("user_prize_data.daily_login.is_draw_daily_vigor1",false);
                DataLookupsCache.Instance.CacheData("user_prize_data.daily_login.is_draw_daily_vigor2", false);
                DataLookupsCache.Instance.CacheData("user_prize_data.daily_login.buy_daily_times", Mathf.Min( curCount+ count,10));
            }
    
        }
    
        private void Init()
        {
            RefreshVigor();
    
            InitData();
            InitCurData();
            InitBase();
            InitRefreshAction();
    
            var dataItems = (curDailyType == EDailyType.Limit) ? mDailyLimitData.ToArray() : mDailyAllDayData.ToArray();
            LTDailyDG.SetItemDatas(dataItems);
            curDailyCell = FirstCell;
        }
    
        private void InitData()
        {
            mDailyLimitData = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.Limit);
            mDailyAllDayData = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.AllDay);
        }
    
        private void InitCurData()
        {
            if (curDailyType == EDailyType.Limit && mDailyLimitData.Count > 0)
            {
                curDailyData = mDailyLimitData[0];
                controller.GObjects["RightContent"].CustomSetActive(true);
            }
            else if (curDailyType == EDailyType.AllDay && mDailyAllDayData.Count > 0)
            {
                if (curFuncGuideId != 0)
                {
                    LTDailyData curData = mDailyAllDayData[0];
                    for (int i = 0; i < mDailyAllDayData.Count; i++)
                    {
                        if (mDailyAllDayData[i].ActivityData.id == curFuncGuideId)
                        {
                            curData = mDailyAllDayData[i];
                            break;
                        }
                    }
                    curDailyData = curData;
                    titleCon.SetTitleBtn(1);
                }
                else
                {
                    curDailyData = mDailyAllDayData[0];
                }
                controller.GObjects["RightContent"].CustomSetActive(true);
            }
            else
            {
                controller.GObjects["RightContent"].CustomSetActive(false);
                return;
            }
    
            curActData = curDailyData.ActivityData;
            curDailyData.IsSelected = true;
        }
    
        private void InitBase()
        {
            if (curActData == null)
            {
                return;
            }
            controller.UiSprites["IconSp"].spriteName = curActData.icon;
            controller.UiLabels["NameLab"].text = curActData.display_name;
            controller.UiLabels["DescLab"].text = curActData.desc.Replace("\\n","\n");
    
            SetOpenTime();
            SetReward();
            SetButtonStatus();
        }
    
        private void InitRefreshAction()
        {
            int startTime = 0;
            for (int i = 0; i < mDailyLimitData.Count; i++)
            {
                var func = Data.FuncTemplateManager.Instance.GetFunc(mDailyLimitData[i].ActivityData.funcId);
                if (func!=null&&!func.IsConditionOK())
                {
                    break;
                }
    
                int curWeek = EB.Time.LocalWeek;
                if (!mDailyLimitData[i].OpenTimeWeek.Contains(curWeek.ToString()) && mDailyLimitData[i].OpenTimeWeek.CompareTo("*") != 0)
                {
                    continue;
                }
    
                if (mDailyLimitData[i].OpenTimeValue < LTDailyDataManager.TimeNow && mDailyLimitData[i].StopTimeValue > LTDailyDataManager.TimeNow)
                {
                    int tempTime = mDailyLimitData[i].StopTimeValue - LTDailyDataManager.TimeNow;
                    startTime = startTime > tempTime || startTime <= 0 ? tempTime : startTime;
                }
    
                if (mDailyLimitData[i].OpenTimeValue > LTDailyDataManager.TimeNow)
                {
                    int tempTime = mDailyLimitData[i].OpenTimeValue - LTDailyDataManager.TimeNow;
                    startTime = startTime > tempTime || startTime <= 0 ? tempTime : startTime;
                }
            }
    
            if (startTime > 0)
            {
                eventSequence = ILRTimerManager.instance.AddTimer((startTime + 1) * 1000, 1, RefreshActTime);
            }
        }
    
        private void SetReward()
        {
            for (int i = 0; i < RewardItems.Length; i++)
            {
                var item = RewardItems[i];

                if (curActData.awards != null && i < curActData.awards.Count)
                {
                    var data = curActData.awards[i];
                    item.LTItemData = new LTShowItemData(data.id, data.count, data.type, false);
                    item.Count.gameObject.SetActive(false);
                    item.mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    item.mDMono.gameObject.CustomSetActive(false);
                }
    
                controller.UiGrids["RewardGrid"].Reposition();
            }
        }
    
        private void SetOpenTime()
        {
            string colorStr = LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            if (curDailyData.ActivityData.id == 9004&& !LTLegionWarManager.Instance.IsOpenWarTime())//军团战特殊判断
            {
                controller.UiLabels["OpenTimeLab"].text = string.Format("[{0}]{1}", colorStr, curDailyData.OpenTimeStr.Replace(EB.Localizer.GetString("ID_codefont_in_EventTemplateManager_43848"), EB.Localizer.GetString("ID_NEXT_WEEK")));
                return;
            }
            if ((curDailyData.OpenTimeWeek.Contains(EB.Time.LocalWeek.ToString()) || curDailyData.OpenTimeWeek.Contains("*")) &&
                ((curDailyData.OpenTimeValue < LTDailyDataManager.TimeNow && curDailyData.StopTimeValue > LTDailyDataManager.TimeNow) || curDailyData.OpenTimeValue <= 0))
            {
                colorStr = LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal;
            }
            controller.UiLabels["OpenTimeLab"].text = string.Format("[{0}]{1}", colorStr, curDailyData.OpenTimeStr);
        }
    
        private void SetButtonStatus()
        {
            if (GetVitActIDList.Contains(curActData.id))
            {
                controller.UiButtons["GotoBtn"].gameObject.CustomSetActive(false);
                SetVitBtnStatus();
            }
            else
            {
                controller.CoolTriggers["GetVitBtn"].gameObject.CustomSetActive(false);
                controller.CoolTriggers["GetVitBtn2"].gameObject.CustomSetActive(false);
    
                controller.UiButtons["GotoBtn"].gameObject.CustomSetActive(true);
                var func = Data.FuncTemplateManager.Instance.GetFunc(curActData.funcId);
                bool isOpen = func==null||func.IsConditionOK();
                controller.UiButtons["GotoBtn"].isEnabled = isOpen;
            }
        }
    
        private void SetVitBtnStatus()
        {
            bool isOpen = IsVitCouldGet() && (curDailyData.OpenTimeValue < LTDailyDataManager.TimeNow && curDailyData.StopTimeValue > LTDailyDataManager.TimeNow);
            if (isOpen)
            {
                controller.CoolTriggers["GetVitBtn"].GetComponent<UISprite>().color = Color.white;
                controller.CoolTriggers["GetVitBtn"].gameObject.CustomSetActive(true);
                controller.CoolTriggers["GetVitBtn2"].gameObject.CustomSetActive(false);
            }
            else
            {
                controller.CoolTriggers["GetVitBtn"].GetComponent<BoxCollider>().enabled = isOpen;
                controller.CoolTriggers["GetVitBtn"].GetComponent<UISprite>().color = Color.magenta;
                int time = 0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.daily_login.buy_daily_times", out time);
                if (time > 0)
                {
                    GetVitBtn2TimeLabel.text = string.Format("{0}({1})", EB.Localizer.GetString("ID_DAILY_ACTIVITY_RECEIVE"), time);
                    controller.CoolTriggers["GetVitBtn"].gameObject.CustomSetActive(false);
                    controller.CoolTriggers["GetVitBtn2"].gameObject.CustomSetActive(true);
                }
                else
                {
                    controller.CoolTriggers["GetVitBtn"].gameObject.CustomSetActive(true);
                    controller.CoolTriggers["GetVitBtn2"].gameObject.CustomSetActive(false);
                }
            }
        }
    
        private bool IsVitCouldGet()
        {
            Hotfix_LT.Data.ActivityTime openTime = LTDailyDataManager.Instance.GetActivityOpenTimeByActivityID(curActData.id);
            if (openTime.hourStr.IndexOf(openTime.hour.ToString()) < 0)
            {
                return false;
            }
            if (openTime.hourStr.IndexOf(openTime.hour.ToString()) == 0)
            {
                bool b = false;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.daily_login.is_draw_daily_vigor1", out b);
                return !b;
            }
            else
            {
                bool b = false;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.daily_login.is_draw_daily_vigor2", out b);
                return !b;
            }
        }
    
        private void RefreshActTime(int timerSequence)
        {
            mDailyLimitData = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.Limit);
            if (curDailyType == EDailyType.Limit)
            {
                if (curDailyCell != null)
                {
                    curDailyData.IsSelected = false;
                    curDailyCell.SetSelectStatus(false);
                }
                InitCurData();
                InitBase();
                InitRefreshAction();
                LTDailyDG.SetItemDatas(mDailyLimitData.ToArray());
                curDailyCell = FirstCell;
            }
        }
    
        public void OnLimitTagClick()
        {
            if (curDailyType == EDailyType.Limit)
            {
                return;
            }
            curDailyType = EDailyType.Limit;
            if (mDailyLimitData == null)
            {
                mDailyLimitData = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.Limit);
            }
            if (curDailyCell != null && curDailyData != null)
            {
                curDailyData.IsSelected = false;
                curDailyCell.SetSelectStatus(false);
            }
            InitCurData();
            InitBase();
            LTDailyDG.SetItemDatas(mDailyLimitData.ToArray());
            curDailyCell = FirstCell;
        }
    
        public void OnAllDayTagClick()
        {
            if (curDailyType == EDailyType.AllDay)
            {
                return;
            }
            curDailyType = EDailyType.AllDay;
            if (mDailyAllDayData == null)
            {
                mDailyAllDayData = LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.AllDay);
            }
            if (curDailyCell != null && curDailyData != null)
            {
                curDailyData.IsSelected = false;
                curDailyCell.SetSelectStatus(false);
            }
            InitCurData();
            InitBase();
            LTDailyDG.SetItemDatas(mDailyAllDayData.ToArray());
            curDailyCell = FirstCell;
        }
    
        public void OnDailyItemClick(LTDailyCell cell)
        {
            if (curDailyCell != null)
            {
                if (curDailyCell == cell)
                {
                    return;
                }
                curDailyData.IsSelected = false;
                curDailyCell.SetSelectStatus(false);
            }
            curDailyCell = cell;
            curDailyData = cell.GetDailyData();
            curDailyData.IsSelected = true;
            curDailyCell.SetSelectStatus(true);
    
            curActData = curDailyData.ActivityData;
            InitBase();
        }
    
        public void OnGotoBtnClick()
        {
            if (curActData.need_alliance && AlliancesManager.Instance.Account.State != eAllianceState.Joined)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDailyHudController_10226"));
                return;
            }
    
            if (string.IsNullOrEmpty(curActData.nav_parameter))
            {
                controller.Close();
                return;
            }
    
            int funcID = 0;
            int.TryParse(curActData.nav_parameter, out funcID);
            if (funcID != 0)
            {
                Hotfix_LT.Data.FuncTemplate funcTem = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(funcID);
                if (funcTem != null)
                {
                    Hotfix_LT.Data.FuncTemplateManager.OpenFunc(funcTem.id, funcTem.parameter);
                    //GlobalMenuManager.Instance.Open(funcTem.ui_model, funcTem.parameter);
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTDailyHudController_10933"));
                    EB.Debug.LogError(string.Format("LTDaily GotoFuncID is Error, Data is Null! funcID = {0}", funcID));
                }
                return;
            }
    
            string[] strs = curActData.nav_parameter.Split(';');
            if (strs.Length < 2)
            {
                EB.Debug.LogError(string.Format("LTDaily NavParameter is Error, Length less than 2! activityID = {0}, param = {1}", curActData.id, curActData.nav_parameter));
            }
            else
            {
                if (AllianceUtil.GetIsInTransferDart(null))
                {
                    return;
                }
    
                controller.Close();
                if (strs[1].CompareTo("EnemySpawns_11") == 0)
                {
                    //世界boss特殊处理，需要走到的是目标区域，但是配置的是怪的模型
                    strs[1] = "AreaTrigger_E";
                }
                WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, strs[0], strs[1]);
            }
        }
    
        public void OnGetVitBtnClick()
        {
            LTDailyDataManager.Instance.GetVit((Hashtable result) =>
            {
                LTDailyDataManager.Instance.SetDailyDataRefreshState();
                EverydayAward dataReward = WelfareTemplateManager.Instance.GetEverydayAwardByType("daily_vigor");
                GlobalMenuManager.Instance.Open("LTShowRewardView", new List<LTShowItemData>() { dataReward.AwardItem });
                SetVitBtnStatus();
                curDailyCell.RPObj.CustomSetActive(false);
                controller.GObjects["DailyLimitRP"].CustomSetActive(false);
            });
        }
    
        public void OnGetVitBtn2Click()
        {
            int cost=(int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("BuyDailyVigorCost");
            int num = GameItemUtil.GetItemAlreadyHave("hc", "res");
    
            if (num < cost)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }
    
            LTDailyDataManager.Instance.BuyDailyVigor((Hashtable result) =>
            {
                EverydayAward dataReward = WelfareTemplateManager.Instance.GetEverydayAwardByType("daily_vigor");
                GlobalMenuManager.Instance.Open("LTShowRewardView", new List<LTShowItemData>() { dataReward.AwardItem });
                SetVitBtnStatus();
            });
        }
    }
}
