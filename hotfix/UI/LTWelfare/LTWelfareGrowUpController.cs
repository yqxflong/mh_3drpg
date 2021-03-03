using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 成长目标UI界面管理器
    /// </summary>
    public class LTWelfareGrowUpController : DynamicMonoHotfix
    {
        public class GrowUpFinalAward
        {
            public UISprite MainIcon;
            public UISprite RoleIcon;
            public UISprite GradeIcon;
            public UISlider CurSlider;
            public UILabel NumLabel;
            public GameObject HeroItem;
            public LTShowItem ShowItem;

            public string id = null;

            private ParticleSystemUIComponent charFx;
            private EffectClip efClip;
            
            public void SetAwardInfo()
            {
                string str = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("WelfareGrowUpAward");
                if (str != null)
                {
                    LTShowItemData itemData = GetItemData(str);
                    if (itemData != null)
                    {
                        if (itemData.type == LTShowItemType.TYPE_HEROSHARD|| itemData.type == LTShowItemType.TYPE_HERO)
                        {
                            id = itemData.id;
                            int cid = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(itemData.id).character_id;
                            Hotfix_LT.Data.HeroInfoTemplate data = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(cid);
                            MainIcon.spriteName = data.icon;
                            RoleIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.char_type]; 
                            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, RoleIcon.transform, (PartnerGrade)data.role_grade, data.char_type);
                            GradeIcon.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)data.role_grade];
                            HeroItem.CustomSetActive(true);
                        }
                        else
                        {
                            ShowItem.LTItemData = itemData;
                            ShowItem.mDMono.gameObject.CustomSetActive(true);
                        }
                    }
                }
            }

            private LTShowItemData GetItemData(string strValue)
            {
                object obj = EB.JSON.Parse(strValue);
                if (obj as ArrayList == null) return null;
                return new LTShowItemData((obj as ArrayList)[0]);
            }
        }

        public class GrowUpTypeBtn
        {
            public UIButton btn;
            public UISprite sprite;
            public UILabel label;
            public GameObject rpObj;
            public int index;
            public GrowUpTypeBtn(int Index, UIButton Btn, UISprite Sprite, UILabel Label, GameObject RpObj, EventDelegate eventDelegate)
            {
                index = Index;
                btn = Btn;
                label = Label;
                sprite = Sprite;
                rpObj = RpObj;
                Btn.onClick.Add(eventDelegate);
            }

            public void SetRP(int day = -1)
            {
                int typeIndex = day * 10 + index;
                rpObj.CustomSetActive(LTWelfareModel.Instance.TypeTabRPList.Contains(typeIndex));
            }
        }

        private GrowUpFinalAward AwardInfo;
        private UILabel DayLabel;
        private UILabel TimeLabel;

        private GameObject DayItem;
        private UIGrid DayGrid;
        private List<GrowUpTypeBtn> TypeBtns;
        private UIGrid TypeGrid;
        private GameObject AwardItem;
        private UIGrid AwardGrid;
        private UIButton RuleBtn;
        private UIButton m_RewardBtn;
        private UILabel m_RewardBtnLabel;

        private static int DayTabNum =-1;
        private static int TypeTabNum =0;

        public static int GrowUpDayNum = -1;
        public static bool DayJudge()
        {
            return DayTabNum < GrowUpDayNum;
        }

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            AwardInfo = new GrowUpFinalAward();
            AwardInfo.MainIcon = t.GetComponent <UISprite>("View1/HeroAward/InfoItem/Icon");
            AwardInfo.RoleIcon = t.GetComponent<UISprite>("View1/HeroAward/InfoItem/Role");
            AwardInfo.GradeIcon = t.GetComponent<UISprite>("View1/HeroAward/InfoItem/Grade");
            AwardInfo.CurSlider = t.GetComponent<UISlider>("View1/HeroAward/ProgressBar");
            AwardInfo.NumLabel = t.GetComponent<UILabel>("View1/HeroAward/ProgressBar/Label");
            AwardInfo.HeroItem = t.Find("View1/HeroAward/InfoItem").gameObject;
            AwardInfo.HeroItem.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnFinalAwardClick));
            AwardInfo.ShowItem = t.GetMonoILRComponent<LTShowItem>("View1/HeroAward/GeneralItem");

            RuleBtn = t.GetComponent<UIButton>("View1/HeroAward/RuleBtn");
            RuleBtn.onClick.Add(new EventDelegate (OnRuleBtnClick));

            DayLabel = t.GetComponent<UILabel>("TaskView/TimeLabel/DayLabel");
            TimeLabel = t.GetComponent<UILabel>("TaskView/TimeLabel/TimeLabel");

            DayItem = t.Find("View1/Item").gameObject;
            DayGrid = t.GetComponent<UIGrid>("View1/ScrollView/Placeholder/Grid");

            TypeBtns = new List<GrowUpTypeBtn>();
            TypeGrid = t.GetComponent<UIGrid>("TaskView/TopBtns");
            UIButton[] btns = TypeGrid.GetComponentsInChildren<UIButton>(true);
            for (int i = 0; i < btns.Length; i++)
            {
                UILabel Label = btns[i].transform.GetComponent<UILabel>("Label");
                UISprite sprite = btns[i].transform.GetComponent<UISprite>("Sprite");
                GameObject RRObj = btns[i].transform.Find("RedPoint").gameObject;
                TypeBtns.Add(new GrowUpTypeBtn(i, btns[i], sprite, Label, RRObj, setBtnEvent(i)));
            }

            AwardItem = t.Find("TaskView/Item").gameObject;
            AwardGrid = t.GetComponent<UIGrid>("TaskView/ScrollView/Placeholder/Grid");
            m_RewardBtn = t.GetComponent<UIButton>("View1/HeroAward/RewardBtn");
            m_RewardBtn.onClick.Add(new EventDelegate(OnRewardBtnClick));
            m_RewardBtnLabel = t.GetComponent<UILabel>("View1/HeroAward/RewardBtn/Label");

            mDayList = new List<LTWelfareGrowUpTabitem>();
            mTasksList = new List<LTWelfareGrowUpTaskItem>();

            timeFinal = 0;
            ActivityOver = false;

            WelfareGrowUpUpdata();
        }

        public override void OnDestroy()
        {
            DayTabNum = -1;
            TypeTabNum = 0;
            LTWelfareEvent.WelfareGrowUpTabClick -= WelfareGrowUpTabClick;
            LTWelfareEvent.WelfareGrowUpUpdata -= WelfareGrowUpUpdata;
            ActivityOver = true;
            EB.Coroutines.Stop(InitData());
            EB.Coroutines.Stop(ShowTimeLabel());
            base.OnDestroy();
        }

        public override void Start()
        {
            if (timeFinal == 0)
            {
                long timeJoin = 0;
                DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
                timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, 7);
            }

            long ts = timeFinal - EB.Time.Now;
            if (ts < 0) ts = 0;
            int day = (int)(ts / (24 * 60 * 60));
            if (GrowUpDayNum != 7 - day)
            {
                GrowUpDayNum = 7 - day;
            }
        }

        IEnumerator InitData()
        {
            yield return null;
            InitInfo();
            yield return null;
            InitDayList();
            EB.Coroutines.Run(ShowTimeLabel());

            LTWelfareEvent.WelfareGrowUpTabClick += WelfareGrowUpTabClick;
            LTWelfareEvent.WelfareGrowUpUpdata += WelfareGrowUpUpdata;
            yield break;
        }

        private bool init = false;
        private void WelfareGrowUpUpdata()
        {
            LTWelfareModel.Instance.UpdataTasks();
            if (!init)
            {
                EB.Coroutines.Run(InitData());
                init = true;
            }
            else
            {
                InitInfo();
                for (int i = 0; i < mTasksList.Count; i++) mTasksList[i].ResetItem();
                if (DayJudge())
                {
                    for(int i=0;i< TypeBtns.Count; i++)
                    {
                        TypeBtns[i].SetRP(DayTabNum);
                    }
                    //TypeBtns[TypeTabNum].SetRP(DayTabNum);
                }
                else
                {
                    for (int i = 0; i < TypeBtns.Count; i++)
                    {
                        TypeBtns[i].SetRP();
                    }
                    //TypeBtns[TypeTabNum].SetRP();
                }
                if (mDayList.Count > DayTabNum || DayTabNum >= 0)
                {
                    for(int i = 0;i< mDayList.Count;i++)
                    {
                        mDayList[i].SetStates();
                    }
                    //mDayList[DayTabNum].SetStates();
                }
            }
        }

        private WaitForSeconds mTimer = new WaitForSeconds(1f);
        private long timeFinal;
        public static bool ActivityOver;
        IEnumerator ShowTimeLabel()
        {
            while (!ActivityOver)
            {
                UpdataTimeFun();
                yield return mTimer;
            }
        }
        public void UpdataTimeFun()
        {
            long ts = timeFinal - EB.Time.Now;// Data.ZoneTimeDiff.GetServerTimeNow();
            string colorStr = "";
            if (ts < 0)
            {
                ActivityOver = true;
                ts = 0;
                colorStr = "[ff6699]";
            }
            int day = (int)(ts / (24 * 60 * 60));
            if (GrowUpDayNum != 7 - day)
            {
                GrowUpDayNum = 7 - day;
                if (LTWelfareEvent.WelfareGrowUpReset != null)
                {
                    LTWelfareEvent.WelfareGrowUpReset();
                }
            }
            DayLabel.text = string.Format("{0}{1}", colorStr, day);

            string timeStr = "";
            timeStr = (ts > 0) ? (string.Format("{0:D2}:{1:D2}:{2:D2}", (ts % (24 * 60 * 60)) / (60 * 60), (ts % (60 * 60)) / 60, ts % 60)) : "00:00:00";
            TimeLabel.text = string.Format(EB.Localizer.GetString("ID_DAY_FORMAT"), colorStr, timeStr);
        }

        private DateTime TimeSpanToDateTime(long span)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            time = startTime.AddSeconds(span);
            return time;
        }
        private static long GetTimeSpan(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(time - startTime).TotalSeconds;
        }


        private void InitInfo()
        {
            AwardInfo.SetAwardInfo();
            int count = Mathf.Min(60, LTWelfareModel.Instance.mLTWelfareGrowUpTaskDatas.Count);
            int cur = LTWelfareModel.Instance.completedNum;
            AwardInfo.CurSlider.value = (float)cur / (float)count;
            AwardInfo.NumLabel.text = string.Format("{0}/{1}", cur, count);
            if (cur >= count)
            {
                m_RewardBtn.gameObject.CustomSetActive (true);
                AwardInfo.CurSlider.gameObject.CustomSetActive(false);
                RuleBtn.gameObject.CustomSetActive(false);
                //判断是否领取过这个奖励
                bool hasGetGrowReward = true;
                DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.grow_reward", out hasGetGrowReward);
                m_RewardBtnLabel.text = hasGetGrowReward ? EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL") : EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                m_RewardBtn.enabled = !hasGetGrowReward;
                m_RewardBtn.GetComponent<UISprite>().color = hasGetGrowReward ? Color.magenta : Color.white;
            }
        }

        private List<LTWelfareGrowUpTabitem> mDayList;
        private void InitDayList()
        {
            for (int i = 0; i < mDayList.Count; i++)
            {
                GameObject.Destroy(mDayList[i].mDMono.gameObject);
            }
            mDayList.Clear();

            int DayCount = LTWelfareModel.Instance.WelfareGrowUpDayCount();
            for (int i = 0; i < DayCount; i++)
            {
                GameObject obj = GameObject.Instantiate(DayItem);
                obj.CustomSetActive(true);
                obj.transform.SetParent(DayGrid.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                LTWelfareGrowUpTabitem levelCtrl = obj.GetComponent<DynamicMonoILR>()._ilrObject as LTWelfareGrowUpTabitem;
                if (DayTabNum == -1 && i < LTWelfareGrowUpController.GrowUpDayNum && LTWelfareModel.Instance.DayTabRPList.Contains(i))
                {
                    DayTabNum = i;
                    for (int j = 0; j < TypeBtns.Count; j++)
                    {
                        int typeIndex = DayTabNum * 10 + j;
                        if (DayJudge() && LTWelfareModel.Instance.TypeTabRPList.Contains(typeIndex))
                        {
                            TypeTabNum = j;
                            break;
                        }
                    }
                }
                levelCtrl.InitData(i);
                mDayList.Add(levelCtrl);
            }
            DayGrid.enabled = true;
            DayGrid.Reposition();
            DayGrid.transform.parent.GetComponent<UIWidget>().height = (int)(DayCount * DayGrid.cellHeight);

            if (DayTabNum == -1) DayTabNum = GrowUpDayNum - 1;
            for (int i = 0; i < mDayList.Count; i++)
            {
                mDayList[i].SetSelectColor(i == DayTabNum);
            }
            int activeCount = (int)(DayGrid.transform.parent.parent.GetComponent<UIPanel>().baseClipRegion.z / DayGrid.cellHeight);
            float value = (float)(DayTabNum+1 - activeCount) / (float)(DayCount - activeCount);
            float scrollValue = DayTabNum+1 > activeCount ? value : 0f;
            DayGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdatePosition();
            DayGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdateScrollbars();
            DayGrid.transform.parent.parent.GetComponent<UIScrollView>().verticalScrollBar.value = scrollValue;
            
            InitTasksList();
        }

        private List<LTWelfareGrowUpTaskItem> mTasksList;
        private void InitTasksList()
        {
            List<LTWelfareGrowUpTaskData> levelList = LTWelfareModel.Instance.GetGrowUpsByDayAndType(DayTabNum, TypeTabNum);
            for (int i = 0; i < TypeBtns.Count; i++)
            {
                TypeBtns[i].sprite.color = i == TypeTabNum ? new Color(1, 1, 1) : new Color(1, 0, 1);
                LTWelfareGrowUpTaskData data = LTWelfareModel.Instance.GetGrowUpByDayAndType(DayTabNum, i);
                TypeBtns[i].label.text = data.TaskTpl != null ? data.TaskTpl.task_name : null;
                if (DayJudge()) TypeBtns[i].SetRP(DayTabNum);
                else TypeBtns[i].SetRP();
            }
            for (int i = 0; i < levelList.Count; i++)
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
                    LTWelfareGrowUpTaskItem TaskCtrl = obj.GetComponent<DynamicMonoILR>()._ilrObject as LTWelfareGrowUpTaskItem;
                    TaskCtrl.InitData(levelList[i]);
                    mTasksList.Add(TaskCtrl);
                }
            }
            for (int i = levelList.Count; i < mTasksList.Count; i++)
            {
                mTasksList[i].mDMono.gameObject.CustomSetActive(false);
            }
            AwardGrid.enabled = true;
            AwardGrid.Reposition();
            AwardGrid.transform.parent.GetComponent<UIWidget>().height = (int)(levelList.Count * AwardGrid.cellHeight);
            AwardGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdatePosition();
            AwardGrid.transform.parent.parent.GetComponent<UIScrollView>().UpdateScrollbars();

        }
        

        private EventDelegate setBtnEvent(int i)
        {
            switch (i)
            {
                case 0: {
                        return new EventDelegate(OnFirstTabClick);
                    } 
                case 1: {
                        return new EventDelegate(OnSecondTabClick);
                    } 
                case 2:
                    {
                        return new EventDelegate(OnThirdTabClick);
                    }
                default: {
                        return new EventDelegate(OnFirstTabClick);
                    }
            }
        }
        private void OnFirstTabClick()
        {
            if (TypeTabNum == 0 || btnClick) return;
            btnClick = true;
            TypeTabNum = 0;
            InitTasksList();
            btnClick = false;
        }
        private void OnSecondTabClick()
        {
            if (TypeTabNum == 1 || btnClick) return;
            btnClick = true;
            TypeTabNum = 1;
            InitTasksList();
            btnClick = false;
        }
        private void OnThirdTabClick()
        {
            if (TypeTabNum == 2|| btnClick) return;
            btnClick = true;
            TypeTabNum = 2;
            InitTasksList();
            btnClick = false;
        }

        private bool btnClick=false;
        private void WelfareGrowUpTabClick(int day)
        {
            if (DayTabNum == day|| btnClick) return;
            btnClick = true;
            DayTabNum = day;
            TypeTabNum = 0;
            for (int i = 0; i < mDayList.Count; i++)
            {
                mDayList[i].SetSelectColor(i == DayTabNum);
            }

            InitTasksList();
            btnClick = false;
        }

        private void OnFinalAwardClick()
        {
            Vector2 screenPos = UICamera.lastEventPosition;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("id", AwardInfo.id);
            ht.Add("screenPos", screenPos);
            GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
        }

        public void OnRuleBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTRuleUIView", EB.Localizer.GetString("ID_RULE_WELFARE_GROWUP"));
        }

        private void OnRewardBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            //
            LTWelfareModel.Instance.DrawGrowReward(delegate (bool success)
            {
                if (success)
                {
                    string str = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("WelfareGrowUpAward");
                    DataLookupsCache.Instance.CacheData("user_prize_data.grow_reward", true);
                    m_RewardBtn.gameObject.CustomSetActive(true);
                    AwardInfo.CurSlider.gameObject.CustomSetActive(false);
                    RuleBtn.gameObject.CustomSetActive(false);
                    m_RewardBtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                    m_RewardBtn.enabled = false;
                    m_RewardBtn.GetComponent<UISprite>().color = Color.magenta;
                    if (str != null)
                    {
                        LTShowItemData itemData = GetItemData(str);
                        if (itemData.type == "hero")
                        {
                            //如果是英雄将弹出获得伙伴界面
                            GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                        }
                    }
                }
            });
        }

        private LTShowItemData GetItemData(string strValue)
        {
            object obj = EB.JSON.Parse(strValue);
            if (obj as ArrayList == null) return null;
            return new LTShowItemData((obj as ArrayList)[0]);
        }
    }
}
