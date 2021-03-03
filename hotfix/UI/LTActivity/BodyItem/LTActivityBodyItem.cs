using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 运营活动面板——基类（一般的前往活动面板）
    /// </summary>
    public class LTActivityBodyItem : DynamicMonoHotfix
    {
        public UITexture BG;
        public UILabel desc;
        public UILabel Countdown_Day;
        public UILabel Countdown_Time;
        public UILabel Countdown_Text;
        public UILabel Event_Time;
        public UIButton NavButton;
        public UILabel NavLabel;
        protected string[] NavString;
        protected string TagString;
        protected string eventType;
        public LTActivityTitleItem title;
        public UIController controller;

        public Transform TimeParent;
        public Transform ContenPartent;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            BG = t.GetComponent<UITexture>("BG");
            Countdown_Day = t.GetComponent<UILabel>("TIME/CDDay");
            Countdown_Time = t.GetComponent<UILabel>("TIME/CDTime");
            Countdown_Text = t.GetComponent<UILabel>("TIME/Text");

            TimeParent = t.Find("TIME");
            ContenPartent = t.Find("CONTENT");

            Transform descTf = t.Find("CONTENT/DESC");
            if (descTf != null)
            {
                desc = descTf.GetComponent<UILabel>();
            }

            Transform EventTimeTf = t.Find("CONTENT/EventTime");
            if (EventTimeTf != null)
            {
                Event_Time = EventTimeTf.GetComponent<UILabel>();
            }

            Transform nav = t.Find("NavButton");
            if (nav != null)
            {
                NavButton = nav.GetComponent<UIButton>();
                NavLabel = nav.GetComponent<UILabel>("Label", false);
                NavButton.onClick.Add(new EventDelegate(OnNavClick));
            }
        }

        public virtual void SetData(object data)
        {
            starttime = EB.Dot.Integer("start", data, 0);
            fintime = EB.Dot.Integer("end", data, 0);
            disappeartime = EB.Dot.Integer("displayuntil", data, 0);
            state = EB.Dot.String("state", data, "");
            int timeZone = ZoneTimeDiff.GetTimeZone();
            if (Event_Time != null) Event_Time.text = EB.Time.FromPosixTime(starttime + timeZone * 3600).ToString("yyyy.MM.dd") + " - " + EB.Time.FromPosixTime(fintime + timeZone * 3600).ToString("yyyy.MM.dd");
            NavString = EB.Dot.String("nav_string", data, "").Split('-');
            TagString = EB.Dot.String("tag", data, "");
            eventType = EB.Dot.String("eventtype", data, "");

            if (NavString.Length <= 0)
            {
                int aid = EB.Dot.Integer("activity_id", data, 0);
                var activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(aid);
                NavString = activity.nav_parameter.Split('-');
            }
            if (NavButton != null && NavLabel != null) UpdateNavText();
            UpdateCountDown(EB.Time.Now);
        }

        protected void SetNavLabelPurchased()
        {
            if (NavLabel == null || NavButton == null) return;
            NavLabel.text = EB.Localizer.GetString("ID_PURCHASED");//已购买
            NavButton.SetState(UIButtonColor.State.Disabled, true);
            NavButton.isEnabled = false;
        }

        protected void SetNavLabelFinised()
        {
            if (NavLabel == null || NavButton == null) return;
            NavLabel.text = EB.Localizer.GetString("ID_FINISHED");//已结束
            NavButton.SetState(UIButtonColor.State.Disabled, true);
            NavButton.isEnabled = false;
        }

        protected void SetNavLabelNotice()
        {
            if (NavLabel == null || NavButton == null) return;
            NavLabel.text = EB.Localizer.GetString("ID_uifont_in_LTLegionMainMenu_Label_16");//未开启
            NavButton.SetState(UIButtonColor.State.Disabled, true);
            NavButton.isEnabled = false;
        }

        protected void UpdateNavText()
        {
            string[] cmd = NavString;
            if (cmd.Length < 2) return;
            switch (cmd[0])
            {
                case "P":
                    var payouts = SparxHub.Instance.GetManager<WalletManager>().Payouts;
                    bool purchased = true;
                    for (int i = 0; i < payouts.Length; i++)
                    {
                        if (payouts[i].payoutId == int.Parse(cmd[1]))
                        {
                            NavLabel.text = payouts[i].localizedCost;
                            purchased = false;
                            return;
                        }
                    }
                    if (purchased)
                    {
                        SetNavLabelPurchased();
                    }
                    return;
                case "R":
                    NavLabel.text = EB.Localizer.GetString("ID_uifont_in_ArenaRankAwardUI_Title_0");//排名奖励
                    return;
                default:
                    NavLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");//前往
                    return;
            }
        }

        protected virtual void OnNavClick()
        {
            string[] cmd = NavString;
            if (cmd.Length < 2) return;
            int data = int.Parse(cmd[1]);
            switch (cmd[0])
            {
                case "P":
                    BuyItem(data);
                    return;
                case "G":
                    GotoGacha(TagString);
                    return;
                case "M":
                    GotoMainCamp();
                    controller.Close();
                    return;
                case "C":
                    GotoChargeStore(data);
                    return;
                case "T":
                    GotoTurntableView(data);
                    return;
                case "R":
                    GotoTurnRewardShowView(cmd[1]);
                    return;
            }
        }

        public void BuyItem(int payoutId)
        {
            var manager = SparxHub.Instance.GetManager<WalletManager>();
            var payouts = manager.Payouts;
            for (int i = 0; i < payouts.Length; i++)
            {
                if (payouts[i].payoutId == payoutId)
                {
                    LTChargeManager.Instance.PurchaseOfferExpand(payouts[i], CreatePurchaseEventTable());
                    return;
                }
            }
        }

        public Hashtable CreatePurchaseEventTable()
        {
            var eventTable = Johny.HashtablePool.Claim();
            eventTable["callBack"] = new System.Action<Hashtable>(delegate (Hashtable table)
            {
                DataLookupsCache.Instance.CacheData(table);

                var payouts = SparxHub.Instance.GetManager<WalletManager>().Payouts;
                var rewards = new List<LTShowItemData>();
                for (int i = 0; i < payouts.Length; i++)
                {
                    if (payouts[i].payoutId == int.Parse(NavString[1]))
                    {
                        for (var j = 0; j < payouts[i].redeemers.Count; j++)
                        {
                            var itemdata = payouts[i].redeemers[j];
                            rewards.Add(new LTShowItemData(itemdata.Data, itemdata.Quantity, itemdata.Type, false));
                        }
                        break;
                    }
                }
                SetNavLabelPurchased();
                GlobalMenuManager.Instance.Open("LTShowRewardView", rewards);
            });
            eventTable["loadingEvent"] = new System.Action<bool>(delegate (bool isShow) { if (isShow) { LoadingSpinner.Show(); } else { LoadingSpinner.Hide(); } });
            return eventTable;
        }

        public void GotoGacha(string boxid)
        {
            FuncTemplate ft = FuncTemplateManager.Instance.GetFunc(10011);

            if (ft != null && !ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }

            GlobalMenuManager.Instance.Open("LTDrawCardTypeUI", boxid);
        }

        public void GotoMainCamp()
        {
            GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
        }

        public void GotoChargeStore(int page)
        {
            GlobalMenuManager.Instance.Open("LTChargeStoreHud", page);
        }

        public void GotoTurntableView(int activityId)
        {
            GlobalMenuManager.Instance.Open("LTActivityTurntableView", activityId);
        }

        //跳转排行榜奖励界面
        public void GotoTurnRewardShowView(string UrPartnerId)
        {
            int infoid;
            if (int.TryParse(UrPartnerId, out infoid))
            {
                Hashtable table = new Hashtable();
                table.Add("itemDatas", EventTemplateManager.Instance.GetURPartnerEventRewardList(infoid));
                table.Add("tip", EB.Localizer.GetString("ID_uifont_in_LTWorldBossRewardPreviewUI_Label_0"));//活动结束后通过邮件发放奖励
                GlobalMenuManager.Instance.Open("LTGeneralRankAwardUI", table);
            }
        }

        protected string state = "";
        protected int starttime = 0;
        protected int fintime = 0;
        protected int disappeartime = 0;


        public void UpdateCountDown(int now)
        {
            int timeTemp = 0;
            if (state.Equals("pending"))
            {
                Countdown_Text.text = EB.Localizer.GetString("ID_WELFARE_CHENGZHANG_STARTTIME");  // 活动开启时间
                SetNavLabelNotice();
                timeTemp = Mathf.Max(starttime - now, 0);
            }
            else if (fintime <= now)
            {
                timeTemp = 0;
                SetNavLabelFinised();
                Countdown_Text.text = EB.Localizer.GetString("ID_WELFARE_CHENGZHANG_ENDTIME");  // 活动结束时间
                if (disappeartime > now)
                {
                    timeTemp = Mathf.Max(disappeartime - now, 0);
                    Countdown_Text.text = eventType.StartsWith("bosschallenge") ? EB.Localizer.GetString("ID_STORE_CLOSING_TIME") : EB.Localizer.GetString("ID_ACTIVITY_DESC_CLOSE");  // 活动关闭时间
                }
            }
            else
            {
                timeTemp = Mathf.Max(fintime - now, 0);
                Countdown_Text.text = EB.Localizer.GetString("ID_WELFARE_CHENGZHANG_ENDTIME");  // 活动结束时间
            }

            System.TimeSpan timeleft = new System.TimeSpan(0, 0, timeTemp);
            Countdown_Day.text = timeleft.Days.ToString();
            Countdown_Time.text = string.Format(EB.Localizer.GetString("ID_COUNTDOWN_WITHOUT_DAY"), timeleft.Hours, timeleft.Minutes, timeleft.Seconds);
        }
    }
}