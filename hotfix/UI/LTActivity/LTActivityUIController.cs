using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 活动界面
    /// </summary>
    public class LTActivityUIController : UIControllerHotfix, IHotfixUpdate
    {
        public enum SortOrderType
        {
            Activity = 0,
            Holiday = 1,
        }

        public override bool IsFullscreen() { return true; }

        public UIGrid ButtonGrid;
        public LTActivityTitleItem TitleTemplate;
        public Transform InsPartnerTransform;
        public LTActivityBodyItem_Gift Template3;
        public LTActivityBodyItem Template4;
        public LTActivityBodyItem_ScoreRepeat Template5;
        public LTActivityBodyItem_ScoreOnce Template6;
        public LTActivityBodyItem_SevenDay Template7;
        public LTActivityBodyItem_Draw Template8;
        public LTActivityBodyItem_Turntable Template9;
        public LTActivityBodyItem_LuckyCat Template10;
        public LTActivityBodyItem_URPartner Template11;
        public LTActivityBodyItem_EquipmentWish Template12;
        public LTActivityBodyItem_BossChallenge Template13;
        public LTActivityBodyItem_SSRWish Template14;
        public LTActivityBodyItem_Monopoly Template15;
        public LTActivityBodyItem_Racing Template16;
        private UIProgressBar progresBar;
        private GameObject ArrowObj;
        private bool isMoreThenScrollView = false;
        private float Timer = 0;
        private LTActivityBodyItem bodyItem;
        private int lastupdate = 0;
        private List<LTActivityTitleItem> titleitems = new List<LTActivityTitleItem>();
        private LTActivityTitleItem currentTitle;
        private SortOrderType sortOrderLimit;
        
        protected DataLookupSparxManager DataLookupsSparxManager
        {
            get { return SparxHub.Instance.GetManager<DataLookupSparxManager>(); }
        }
        
        public override void SetMenuData(object param)
        {
            sortOrderLimit = SortOrderType.Activity;
            if (param != null)
            {
                currentTitle = param as LTActivityTitleItem;
                if (currentTitle == null)
                {
                    sortOrderLimit = (SortOrderType)param;
                }
            }
        }

        public override void Show(bool isShowing)
        {
            if (!isShowing)
            {
                if (bodyItem != null && bodyItem.mDMono != null)
                {
                    GameObject.DestroyImmediate(bodyItem.mDMono.gameObject);
                }

                bodyItem = null;
                return;
            }

            for (int i = 0; i < titleitems.Count; i++)
            {
                GameObject.DestroyImmediate(titleitems[i].mDMono.gameObject);
            }

            titleitems.Clear();
            LTActivityTitleItem refreshItem = null;
            ArrayList eventlist;
            DataLookupsCache.Instance.SearchDataByID("events.events", out eventlist);

            for (int i = 0; i < eventlist.Count; ++i)
            {
                for (int j = 0; j < eventlist.Count - i - 1; ++j)
                {
                    int bst = EB.Dot.Integer("start", eventlist[j + 1], 0);

                    if (bst > EB.Time.Now)
                    {
                        continue;
                    }

                    int ast = EB.Dot.Integer("start", eventlist[j], 0);
                    int aid = EB.Dot.Integer("priority", eventlist[j], 0);
                    int bid = EB.Dot.Integer("priority", eventlist[j + 1], 0);

                    if (EB.Time.Now < ast || bid != 0 && (aid == 0 || aid > bid))
                    {
                        var temp = eventlist[j];
                        eventlist[j] = eventlist[j + 1];
                        eventlist[j + 1] = temp;
                    }
                }
            }

            int count = 0;

            for (int i = 0; i < eventlist.Count; ++i)
            {
                int sort_order = EB.Dot.Integer("sort_order", eventlist[i], 0);
                if (sort_order!= (int)sortOrderLimit)
                {
                    continue;
                }

                int aid = EB.Dot.Integer("activity_id", eventlist[i], 0);
                int disappear = EB.Dot.Integer("displayuntil", eventlist[i], 0);
                TimeLimitActivityTemplate activity = EventTemplateManager.Instance.GetTimeLimitActivity(aid);
                if (activity == null || EB.Time.Now >= disappear)
                {
                    continue;
                }

                string state = EB.Dot.String("state", eventlist[i], "");
                string title = EB.Dot.String("title", eventlist[i], "");
                string titlebg = EB.Dot.String("titlebg", eventlist[i], "");
                string navString = EB.Dot.String("nav_string", eventlist[i], "");
                LTActivityTitleItem titleItem = InstantiateEx<LTActivityTitleItem>(TitleTemplate, TitleTemplate.mDMono.transform.parent, aid.ToString());
                titleitems.Add(titleItem);
                titleItem.Title.text = EB.Localizer.GetString(title);
                titleItem.ActivityData = eventlist[i];
                titleItem.v_ActivityId = aid;
                titleItem.v_NavString = navString;

                int start = EB.Dot.Integer("start", eventlist[i], 0);
                if (state.Equals("pending"))
                {
                    if (EB.Time.Now > start)
                    {
                        LTMainHudManager.Instance.UpdateEventsLoginData(delegate { });
                    }
                }
                titleItem.OpenNotice.CustomSetActive(state.Equals("pending"));
                titleItem.parent = this;

                if (currentTitle != null && currentTitle.v_ActivityId == aid && currentTitle.v_NavString == navString)
                {
                    refreshItem = titleItem;
                }

                GlobalMenuManager.Instance.LoadRemoteUITexture(titlebg, titleItem.BG);
                count++;
            }

            isMoreThenScrollView = count > 5;
            OnChange();

            ButtonGrid.Reposition();
            UpdateRedPoint();

            if (refreshItem != null)
            {
                SelectTitle(refreshItem);
            }
            else
            {
                if (titleitems.Count > 0)
                {
                    SelectTitle(titleitems[0]);
                }
                else
                {
                    this.controller.OnCancelButtonClick();
                    if (LTMainMenuHudController.Instance != null) LTMainMenuHudController.Instance.RefreshEvent();
                }
            }
        }

        public override IEnumerator OnAddToStack()
        {
            if (AutoRefreshingManager.Instance.GetRefreshed(AutoRefreshingManager.RefreshName.LoginActivity))
            {
                GetLoginData(false, false, delegate {
                    base.OnAddToStack();
                    LTInstanceMapModel.Instance.ClearMonopolyData();
                    Show(true);
                });
                yield return null;
            }
            else
            {
                yield return base.OnAddToStack();
            }

            if (GlobalMenuManager.Instance.IsContain("LTActivityHud"))
            {
                GlobalMenuManager.Instance.RemoveCache("LTActivityHud");
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            progresBar.value = 0;
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ButtonGrid = t.GetComponent<UIGrid>("Content/ScrollView/Placeholder/ButtonList");
            TitleTemplate = t.GetMonoILRComponent<LTActivityTitleItem>("Content/ScrollView/Placeholder/ButtonList/template");
            InsPartnerTransform = t.FindEx("Content/ViewList");
            Template3 = t.GetMonoILRComponent<LTActivityBodyItem_Gift>("Content/ViewList/template3");
            Template4 = t.GetMonoILRComponent<LTActivityBodyItem>("Content/ViewList/template4");
            Template5 = t.GetMonoILRComponent<LTActivityBodyItem_ScoreRepeat>("Content/ViewList/template5");
            Template6 = t.GetMonoILRComponent<LTActivityBodyItem_ScoreOnce>("Content/ViewList/template6");
            Template7 = t.GetMonoILRComponent<LTActivityBodyItem_SevenDay>("Content/ViewList/template7");
            Template8 = t.GetMonoILRComponent<LTActivityBodyItem_Draw>("Content/ViewList/template8");
            Template9 = t.GetMonoILRComponent<LTActivityBodyItem_Turntable>("Content/ViewList/template9");
            Template10 = t.GetMonoILRComponent<LTActivityBodyItem_LuckyCat>("Content/ViewList/template10");
            Template11 = t.GetMonoILRComponent<LTActivityBodyItem_URPartner>("Content/ViewList/template11");
            Template12 = t.GetMonoILRComponent<LTActivityBodyItem_EquipmentWish>("Content/ViewList/template12");
            Template13 = t.GetMonoILRComponent<LTActivityBodyItem_BossChallenge>("Content/ViewList/template13");
            Template14 = t.GetMonoILRComponent<LTActivityBodyItem_SSRWish>("Content/ViewList/template14");
            Template15 = t.GetMonoILRComponent<LTActivityBodyItem_Monopoly>("Content/ViewList/template15");
            Template16 = t.GetMonoILRComponent<LTActivityBodyItem_Racing>("Content/ViewList/template16");
            controller.backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");

            progresBar = t.GetComponent<UIProgressBar>("Content/ScrollView/UIScrollBar");
            ArrowObj = t.FindEx("Content/Arrow").gameObject;
            progresBar.onChange.Add(new EventDelegate(OnChange));
            TitleTemplate.mDMono.gameObject.CustomSetActive(false);
            Template3.mDMono.gameObject.CustomSetActive(false);
            Template4.mDMono.gameObject.CustomSetActive(false);
            Template5.mDMono.gameObject.CustomSetActive(false);
            Template6.mDMono.gameObject.CustomSetActive(false);
            Template7.mDMono.gameObject.CustomSetActive(false);
            Template8.mDMono.gameObject.CustomSetActive(false);
            Template9.mDMono.gameObject.CustomSetActive(false);
            Template10.mDMono.gameObject.CustomSetActive(false);
            Template11.mDMono.gameObject.CustomSetActive(false);
            Template12.mDMono.gameObject.CustomSetActive(false);
            Template13.mDMono.gameObject.CustomSetActive(false);
            Template14.mDMono.gameObject.CustomSetActive(false);
            Template15.mDMono.gameObject.CustomSetActive(false);
            Template16.mDMono.gameObject.CustomSetActive(false);
        }

        public override void OnEnable()
        {
            RegisterMonoUpdater();
        }

        public void Update()
        {
            int now = EB.Time.Now;
            if (now > lastupdate)
            {
                lastupdate = now;
                if (bodyItem != null) bodyItem.UpdateCountDown(now);
            }
            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            bodyItem = null;
            titleitems.Clear();
        }

        public void SelectTitle(LTActivityTitleItem t)
        {
            if (currentTitle != null)
            {
                currentTitle.Pressed.CustomSetActive(false);
            }

            currentTitle = t;
            t.Pressed.CustomSetActive(true);
            bool isEvent = false;

            if (UpdateDataFunc(t.ActivityData, out isEvent))
            {
                GetLoginData(true, isEvent, () => CreateBodyItem(t));
            }
            else
            {
                CreateBodyItem(t);
            }
        }

        private bool UpdateDataFunc(object e, out bool isEvent)
        {
            int activityId = EB.Dot.Integer("activity_id", e, 0);
            isEvent = false;

            switch (activityId) {
                case 6010:  //元旦登录
                case 6011:  //春节登录
                case 6012:  //劳动节登录
                case 6013:  //端午节登录
                case 6014:  //中秋节登录
                case 6015:  //国庆节登录
                case 6016:  //圣诞节登录
                case 6017:  //清明节登录
                case 6018:  //万圣节登录
                case 6019:  //七夕登录
                case 6506:  //副本运营活动需刷新次数                  
                    return true;
            }

            string state = EB.Dot.String("state", e, "");

            if (EB.Dot.Integer("start", e, 0) < EB.Time.Now && state.Equals("pending"))//活动预告需刷新是否开启
            {
                isEvent = true;
                return true;
            } else if (EB.Dot.Integer("end", e, 0) < EB.Time.Now && state.Equals("running"))
            {
                isEvent = true;
                return true;
            }

            return false;
        }

        private void CreateBodyItem(LTActivityTitleItem t)
        {
            object e = t.ActivityData;
            if (bodyItem != null && bodyItem.mDMono != null)
            {
                GameObject.DestroyImmediate(bodyItem.mDMono.gameObject);
            }

            int aid = EB.Dot.Integer("activity_id", e, 0);
            int nav_type = EB.Dot.Integer("nav_type", e, 0);
            if (nav_type == 0)
            {
                Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(aid);
                if (activity == null) return;
                nav_type = (int)activity.nav_type;
            }

            switch (nav_type)
            {
                case 3:
                    bodyItem = InstantiateEx<LTActivityBodyItem_Gift>(Template3, InsPartnerTransform, aid.ToString());
                    break;
                case 4:
                    bodyItem = InstantiateEx<LTActivityBodyItem>(Template4, InsPartnerTransform, aid.ToString());
                    break;
                case 5:
                    bodyItem = InstantiateEx<LTActivityBodyItem_ScoreRepeat>(Template5, InsPartnerTransform, aid.ToString());
                    break;
                case 6:
                    bodyItem = InstantiateEx<LTActivityBodyItem_ScoreOnce>(Template6, InsPartnerTransform, aid.ToString());
                    break;
                case 7:
                    bodyItem = InstantiateEx<LTActivityBodyItem>(Template8,InsPartnerTransform, aid.ToString());
                    bodyItem.TimeParent.localPosition = new Vector3(400, 440, 0);//400,340,0
                    bodyItem.ContenPartent.localPosition = new Vector3(0, -500, 0);//zero
                    bodyItem.desc.width = bodyItem.desc.transform.GetChild(0).GetComponent<UIWidget>().width = 1200;//1700
                    break;
                case 8:
                    bodyItem = InstantiateEx<LTActivityBodyItem>(Template8, InsPartnerTransform, aid.ToString());
                    bodyItem.TimeParent.localPosition = new Vector3(-800, 440, 0);//400,340,0
                    bodyItem.ContenPartent.localPosition = new Vector3(0, -500, 0);//zero
                    bodyItem.desc.width = bodyItem.desc.transform.GetChild(0).GetComponent<UIWidget>().width = 1200;//1700
                    break;
                case 9:
                    bodyItem = InstantiateEx(Template7, InsPartnerTransform, aid.ToString());
                    break;
                case 10:
                    bodyItem = InstantiateEx(Template9, InsPartnerTransform, aid.ToString());
                    break;
                case 11:
                    bodyItem = InstantiateEx(Template10, InsPartnerTransform, aid.ToString());
                    break;
                case 12:
                    bodyItem = InstantiateEx(Template11, InsPartnerTransform, aid.ToString());
                    break;
                case 13:
                    bodyItem = InstantiateEx(Template12, InsPartnerTransform, aid.ToString());
                    break;
                case 14:
                    bodyItem = InstantiateEx(Template13,InsPartnerTransform, aid.ToString());
                    break;
                case 15:
                    bodyItem = InstantiateEx(Template14, InsPartnerTransform, aid.ToString());
                    break;
                case 16:
                    bodyItem = InstantiateEx(Template15, InsPartnerTransform, aid.ToString());
                    break;
                case 17:
                    bodyItem = InstantiateEx(Template16, InsPartnerTransform, aid.ToString());
                    break;
                default:
                    return;
            }

            string desc = EB.Dot.String("desc", e, "");
            string bg = EB.Dot.String("bg", e, "");
            if (bodyItem.desc != null) bodyItem.desc.text = EB.Localizer.GetString(desc);
            GlobalMenuManager.Instance.LoadRemoteUITexture(bg, bodyItem.BG);

            bodyItem.title = t;
            bodyItem.controller = controller;
            bodyItem.SetData(e);
        }

        private void GetLoginData(bool timerLimit, bool isEvents = true, System.Action callBack = null)
        {
            if (timerLimit && Timer > 0)
            {
                if (callBack != null) callBack();
                return;
            }

            Timer = 60;

            if (isEvents)
            {
                LTMainHudManager.Instance.UpdateEventsLoginData(callBack);
            }
            else
            {
                LTMainHudManager.Instance.UpdateActivityLoginData(callBack);
            }
        }

        private void UpdateRedPoint()
        {
            ArrayList eventlist;
            DataLookupsCache.Instance.SearchDataByID("events.events", out eventlist);
            for (int i = 0; i < titleitems.Count; ++i)
            {
                titleitems[i].UpdateRedPoint();
            }
        }

        private void OnChange()
        {
            if (isMoreThenScrollView)
            {
                ArrowObj.CustomSetActive(progresBar.value < 0.98f);
            }
            else
            {
                ArrowObj.CustomSetActive(false);
            }
        }

    }
}