using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public enum ChoiceState
    {
        /// <summary>
        /// 成员页面
        /// </summary>
        Member,
        /// <summary>
        /// 活动页面
        /// </summary>
        Activity,
        /// <summary>
        /// 科技页面
        /// </summary>
        Technology,
        /// <summary>
        /// 捐献页面
        /// </summary>
        Donate,
        /// <summary>
        /// 事件信息页面
        /// </summary>
        Message,
        /// <summary>
        /// 勋章界面
        /// </summary>
        Medal,
        /// <summary>
        /// 佣兵界面
        /// </summary>
        Mercenary,
    }

    public class LegionMainHudController : UIControllerHotfix
    {

        public override void Awake()
        {
            base.Awake();

            MemberPage = controller.transform.GetMonoILRComponentByClassPath<LegionPageMember>("Content/Member", "Hotfix_LT.UI.LegionPageMember");
            ActivityPage = controller.transform.Find("Content/Activity").GetMonoILRComponent<LegionPageActivity>();
            TechnologyPage = controller.transform.Find("Content/Technology").GetMonoILRComponent<LegionPageTechnology>();
            DonatePage = controller.transform.Find("Content/Donate").GetMonoILRComponent<LegionPageDonate>();
            MessagePage = controller.transform.Find("Content/Message").GetMonoILRComponent<LegionPageMessage>();
            MedalPage = controller.transform.Find("Content/Medal").GetMonoILRComponent<LegionPageMedal>();
            MercenaryPage = controller.transform.GetMonoILRComponent<LegionPageMercenary>("Content/Mercenary");
            MailView = controller.transform.Find("LTLegionMailView").GetMonoILRComponent<LegionMailView>();
            MemberInfoView = controller.transform.Find("LTLegionMemberInfoView").GetMonoILRComponent<LegionMemberInfoView>();
            GiveMonthCardView = controller.transform.Find("LTLegionMonthCardView").GetMonoILRComponent<LegionMonthCardView>();
            NameEditView = controller.transform.Find("LTLegionNameEditView").GetMonoILRComponent<LegionNameEditView>();
            IconEditView = controller.transform.Find("LTLegionIconEditView").GetMonoILRComponent<LegionIconEditView>();
            NoticeEditView = controller.transform.Find("LTLegionNoticeEditView").GetMonoILRComponent<LegionNoticeEditView>();
            titleCtrl = controller.transform.Find("Title").GetMonoILRComponent<TitleListController>();
            controller.backButton = controller.transform.Find("LeftTop/CancelBtn").GetComponent<UIButton>();

            controller.BindingBtnEvent(GetList("ChoiceMemberBtn","ChoiceActivityBtn","ChoiceTechnologyBtn","ChoiceDonateBtn","ChoiceMessageBtn","ChoiceMedalBtn","ChoiceMercenaryBtn"),
				GetList(new EventDelegate(OnClickChoiceMember), new EventDelegate(OnClickChoiceActivity), new EventDelegate(OnClickChoiceTechnology),
					new EventDelegate(OnClickChoiceDonate), new EventDelegate(OnClickChoiceMessage), new EventDelegate(OnClickChoiceMedal), new EventDelegate(OnClickMercenaryBtn)));

            if (MemberPage != null)
            {
                MemberPage.onClickShowMail = OnClickShowMail;
                MemberPage.onClickManager = OnClickManager;
                MemberPage.onClickNoticeEditBtn = OnClickNoticeEditBtn;
                MemberPage.onClickLeaveLegion = OnClickLeaveLegion;
                MemberPage.onClickEditNameBtn = OnClickEditNameBtn;

            }


			if (DonatePage != null)
            {
                DonatePage.SetGoldDonateAction(OnClickGoldDontae);
                DonatePage.SetDiamondDonateAction(OnClickDiamondDonate);
                DonatePage.SetLuxuryDonateAction(OnClickZZDonate);
            }

            if (MemberInfoView != null)
            {
                MemberInfoView.onClickAddFriend = OnClickAddFriend;
                MemberInfoView.onClickTalk = OnClickTalk;
                MemberInfoView.onClickPromote = OnClickPromote;
                MemberInfoView.onClickDemote = OnClickDemote;
                MemberInfoView.onClickGiveOwner = OnClickGiveOwner;
                MemberInfoView.onClickKickOut = OnClickKickOut;
                MemberInfoView.onClickMonthCard = OnClickMonthCard;
            }

            if (GiveMonthCardView != null)
            {
                GiveMonthCardView.onClickCancelMonthCard = OnClickCancelMonthCard;
                GiveMonthCardView.onClickGiveMonthCard = OnClickGiveMonthCard;
            }

            if (NoticeEditView != null)
            {
                NoticeEditView.onClickSaveNotice = OnClickSaveNotice;
            }

            LegionEvent.CloseLegionHudUI += controller.Close;
            LegionEvent.NotifyUpdateLegionData += SetData;
            LegionEvent.NotifyUpdateLegionMessages += SetLegionMessages;
            LegionEvent.NotifyLegionAccount += OnLegionAccount;
            LegionEvent.NotifyByKickOut += OnByKickOut;
            LegionEvent.OnClickMember += OnClickMemberItem;
        }




        public LegionPageMember MemberPage;
        public LegionPageActivity ActivityPage;
        public LegionPageTechnology TechnologyPage;
        public LegionPageDonate DonatePage;
        public LegionPageMessage MessagePage;
        public LegionPageMedal MedalPage;
        public LegionPageMercenary MercenaryPage;

        public LegionMailView MailView;
        public LegionMemberInfoView MemberInfoView;
        public LegionMonthCardView GiveMonthCardView;
        public LegionNameEditView NameEditView;
        public LegionIconEditView IconEditView;
        public LegionNoticeEditView NoticeEditView;


        public TitleListController titleCtrl;

        private LegionData legionData;

        
        private ChoiceState _mainChoiceState;
        public ChoiceState MainChoiceState
        {
            get { return _mainChoiceState; }
            set
            {
                _mainChoiceState = value;
                ChangeMainState(value);
            }
        }

        public override void OnDestroy()
        {
            if (controller.UiButtons["ChoiceMemberBtn"] != null) controller.UiButtons["ChoiceMemberBtn"].onClick.Clear();
            if (controller.UiButtons["ChoiceActivityBtn"] != null) controller.UiButtons["ChoiceActivityBtn"].onClick.Clear();
            if (controller.UiButtons["ChoiceTechnologyBtn"] != null) controller.UiButtons["ChoiceTechnologyBtn"].onClick.Clear();
            if (controller.UiButtons["ChoiceDonateBtn"] != null) controller.UiButtons["ChoiceDonateBtn"].onClick.Clear();
            if (controller.UiButtons["ChoiceMessageBtn"] != null) controller.UiButtons["ChoiceMessageBtn"].onClick.Clear();
            LegionEvent.CloseLegionHudUI -= controller.Close;
            LegionEvent.NotifyUpdateLegionData -= SetData;
            LegionEvent.NotifyUpdateLegionMessages -= SetLegionMessages;
            LegionEvent.NotifyLegionAccount -= OnLegionAccount;
            LegionEvent.NotifyByKickOut -= OnByKickOut;
            LegionEvent.OnClickMember -= OnClickMemberItem;
            base.OnDestroy();
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            InitRedPoint();

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.haveapply, SetMemberRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.legionactivity, SetActivityRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.donate, SetDonateRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.legiontechnology, SetTechnologyRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.haveevent, SetMessageRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.mercenary, SetMercenaryRP);
        }

        private void SetMemberRP(RedPointNode node)
        {
            controller.GObjects["MemberRedPoint"].CustomSetActive(node.num > 0);
        }
        private void SetActivityRP(RedPointNode node)
        {
            controller.GObjects["ActivityRedPoint"].CustomSetActive(node.num > 0);
        }
        private void SetDonateRP(RedPointNode node)
        {
            controller.GObjects["DonateRedPoint"].CustomSetActive(node.num > 0);
        }
        private void SetTechnologyRP(RedPointNode node)
        {
            controller.GObjects["TechnologyRedPoint"].CustomSetActive(node.num > 0);
        }
        private void SetMessageRP(RedPointNode node)
        {
            controller.GObjects["MessageRedPoint"].CustomSetActive(node.num > 0);
        }
        
        private void SetMercenaryRP(RedPointNode node)
        {
            controller.GObjects["MercenaryRedPoint"].CustomSetActive(node.num > 0);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.haveapply, SetMemberRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.legionactivity, SetActivityRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.donate, SetDonateRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.legiontechnology, SetTechnologyRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.haveevent, SetMessageRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.mercenary, SetMercenaryRP);
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

        /// <summary>
        /// 打开后传参
        /// </summary>
        /// <param name="param"></param>
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable table = param as Hashtable;
            if (table == null)
            {
                LegionData data = param as LegionData;

                if (data == null)
                {
                    return;
                }
                SetData(data);
                MainChoiceState = ChoiceState.Member;
                titleCtrl.SetTitleBtn((int)MainChoiceState);
            }
            else
            {
                if (table["legionData"] != null)
                {
                    LegionData data = (LegionData)table["legionData"];
                    if (data == null) return;
                    SetData(data);
                }
                if (table["choiceState"] != null)
                {
                    ChoiceState state = (ChoiceState)table["choiceState"];
                    titleCtrl.SetTitleBtn((int)state);
                    MainChoiceState = state;
                }
            }

            //刷新军团被雇佣次数
            LegionLogic.GetInstance().OnSendGetCurDonateInfo((ha) =>
            {
                DataLookupsCache.Instance.CacheData(ha);
            });
        }

        public override void OnFocus()
        {
	        PausePanelUpdate(UIPanel.PauseType.Others, false);
			if (MainChoiceState == ChoiceState.Medal)
            {
                MedalPage.SetMedalCoin();
            }
            if (MainChoiceState == ChoiceState.Mercenary)
            {
                MercenaryPage.OnFocus();
                LegionLogic.GetInstance().IsHaveMercenary();
            }
        }


        private void SetData(LegionData data)
        {
            if (data == null)
            {
               EB.Debug.LogWarning("LegionData == null");
                return;
            }

            if (MemberPage != null)
            {
                MemberPage.SetData(data);
            }

            if (DonatePage != null)
            {
                DonatePage.SetData(data);
            }

            if (MessagePage != null)
            {
                MessagePage.SetMessageData(data.listMessageItem);
            }

            if (MemberInfoView != null)
            {
                MemberInfoView.SetData(data, 0);
            }

            if (MailView != null)
            {
                MailView.SetData(data);
            }

            if (MedalPage != null && MainChoiceState == ChoiceState.Medal)
            {
                MedalPage.SetLegionMemberData(data.listMember);
                MedalPage.ShowUI(true);
            }

            legionData = data;
        }

        public override void OnCancelButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (MailView.IsShow)
            {
                MailView.ShowUI(false);
                return;
            }

            if (GiveMonthCardView.IsShow)
            {
                GiveMonthCardView.OnClickCancelMonthCard();
                return;
            }

            if (MemberInfoView.IsShow)
            {
                MemberInfoView.ShowUI(false);
                return;
            }

            if (IconEditView.IsShow)
            {
                IconEditView.ShowUI(false);
                return;
            }

            if (NameEditView.IsShow)
            {
                NameEditView.ShowUI(false);
                return;
            }

            if (NoticeEditView.IsShow)
            {
                NoticeEditView.ShowUI(false);
                return;
            }

            controller.Close();
        }

        void SetLegionMessages(List<MessageItemData> messages)
        {
            if (MessagePage != null)
            {
                MessagePage.SetMessageData(messages);
            }
        }


        private void ChangeMainState(ChoiceState state)
        {
            MemberPage.ShowUI(state == ChoiceState.Member);
            ActivityPage.ShowUI(state == ChoiceState.Activity);
            TechnologyPage.ShowUI(state == ChoiceState.Technology);
            DonatePage.ShowUI(state == ChoiceState.Donate);
            MessagePage.ShowUI(state == ChoiceState.Message);
            MedalPage.ShowUI(state == ChoiceState.Medal);
            MercenaryPage.ShowUI(state == ChoiceState.Mercenary);
        }

        /// <summary>
        /// 点击成员标题
        /// </summary>
        private void OnClickChoiceMember()
        {
            MainChoiceState = ChoiceState.Member;
        }

        /// <summary>
        /// 点击活动标题
        /// </summary>
        private void OnClickChoiceActivity()
        {
            MainChoiceState = ChoiceState.Activity;
        }

        /// <summary>
        /// 点击科技标题
        /// </summary>
        private void OnClickChoiceTechnology()
        {
            MainChoiceState = ChoiceState.Technology;
            TechnologyPage.SetTechnologyInfo(legionData);
        }

        /// <summary>
        /// 点击捐献标题
        /// </summary>
        private void OnClickChoiceDonate()
        {
            MainChoiceState = ChoiceState.Donate;
        }

        /// <summary>
        /// 点击事件标题
        /// </summary>
        private void OnClickChoiceMessage()
        {
            MainChoiceState = ChoiceState.Message;
            LegionLogic.GetInstance().IsHaveNewEvent = false;
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.haveevent, 0);
            if (LegionEvent.SendGetLegionMessages != null)
            {
                LegionEvent.SendGetLegionMessages();
            }
        }

        /// <summary>
        /// 点击勋章标题
        /// </summary>
        private void OnClickChoiceMedal()
        {
            MedalPage.SetLegionMemberData(legionData.listMember);
            MainChoiceState = ChoiceState.Medal;
        }
        
        private void OnClickMercenaryBtn()
        {
            SceneLogic.BattleType = eBattleType.None;//进入佣兵界面置为none，防止Heroitem显示错误
            MainChoiceState = ChoiceState.Mercenary;

        }

        void OnClickShowMail()
        {
            TweenScale TS = MailView.mDMono.GetComponent<TweenScale>();
            TS.ResetToBeginning();
            TS.PlayForward();
            MailView.ShowUI(true);
        }

        private void OnClickNoticeEditBtn()
        {
            NoticeEditView.ShowUI(true);
            NoticeEditView.SetData(legionData.notice);
        }

        void OnClickManager()
        {
            if (LegionEvent.OpenManagerMenu != null)
            {
                LegionEvent.OpenManagerMenu();
            }
        }

        void OnClickSaveNotice(string notice)
        {
            if (LegionEvent.SendSaveLegionNotice != null)
            {
                LegionEvent.SendSaveLegionNotice(notice);
            }
        }

        void OnClickLeaveLegion()
        {
            if (LegionEvent.SendLeaveLegion != null)
            {
                LegionEvent.SendLeaveLegion();
            }
        }

        void OnClickMemberItem(long uid)
        {
            if (uid == LoginManager.Instance.LocalUserId.Value) //若是玩家自己就不打开
            {
                return;
            }
            LTFormationDataManager.Instance.GetOtherPlayerData(uid, "normal", "current", null, delegate (List<OtherPlayerPartnerData> partnerDataList, string allianceName)
            {
                if (partnerDataList == null)
                {
                    controller.Close();
                    return;
                }
                if (MemberInfoView != null)
                {
                    TweenScale TS = MemberInfoView.mDMono.GetComponent<TweenScale>();
                    TS.ResetToBeginning();
                    TS.PlayForward();
                    MemberInfoView.ShowUI(true);
                    MemberInfoView.SetData(legionData, uid, partnerDataList);
                }
            });
        }
        private void OnClickEditNameBtn()
        {
            NameEditView.SetData(legionData);
        }

        void OnClickAddFriend(long uid)
        {
            if (LegionEvent.SendMemberAddFriend != null)
            {
                LegionEvent.SendMemberAddFriend(uid);
            }
        }

        void OnClickTalk(long uid)
        {
            if (LegionEvent.OpenMemberTalk != null)
            {
                LegionEvent.OpenMemberTalk(uid);
            }
        }

        //升职
        void OnClickPromote(long uid)
        {
            if (LegionEvent.SendMemberPromote != null)
            {
                LegionEvent.SendMemberPromote(uid);
            }
        }

        //降职
        void OnClickDemote(long uid)
        {
            if (LegionEvent.SendMemberDemote != null)
            {
                LegionEvent.SendMemberDemote(uid);
            }
        }

        //给团长
        void OnClickGiveOwner(long uid)
        {
            if (LegionEvent.SendMemberGiveOwner != null)
            {
                LegionEvent.SendMemberGiveOwner(uid);
            }
        }

        //踢出军团
        void OnClickKickOut(long uid)
        {
            if (LegionEvent.SendMemberKickOut != null)
            {
                LegionEvent.SendMemberKickOut(uid);
            }
        }

        void OnLegionAccount(AllianceAccount data)
        {
            if (data.State == eAllianceState.Leaved)
            {
                controller.Close();
                if (AlliancesManager.Instance.TotalMessage != null)
                {
                    // 离开军团后清空军团事件
                    AlliancesManager.Instance.TotalMessage.CleanUp();
                }
            }
        }

        void OnByKickOut()
        {
            // 已废弃，为什么不知道，不会调这里了！
            //需要补充界面告诉他被踢出公会
            controller.Close();

            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionMainHudController_12495"));
        }


        void OnClickGoldDontae()
        {
            if (LegionEvent.SendGoldDonate != null)
            {
                LegionEvent.SendGoldDonate();
            }
        }

        void OnClickDiamondDonate()
        {
            if (LegionEvent.SendDiamandDonate != null)
            {
                LegionEvent.SendDiamandDonate();
            }
        }

        void OnClickZZDonate()
        {
            if (LegionEvent.SendLuxuryDonate != null)
            {
                LegionEvent.SendLuxuryDonate();
            }
        }

        void OnClickMonthCard(long uid)
        {
            if (GiveMonthCardView != null)
            {
                GiveMonthCardView.ShowUI(true);
                GiveMonthCardView.SetData(legionData, uid);
            }
            if (MemberInfoView != null)
            {
                MemberInfoView.ShowUI(false);
            }
        }

        void OnClickCancelMonthCard(long uid)
        {
            //打开InfoView
            if (GiveMonthCardView != null)
            {
                GiveMonthCardView.ShowUI(false);
            }

            if (MemberInfoView != null)
            {
                TweenScale TS = MemberInfoView.mDMono.GetComponent<TweenScale>();
                TS.ResetToBeginning();
                TS.PlayForward();
                MemberInfoView.ShowUI(true);
                MemberInfoView.SetData(legionData, uid);
            }
        }

        void OnClickGiveMonthCard(long uid)
        {
            if (LegionEvent.SendGiveMonthCard != null)
            {
                LegionEvent.SendGiveMonthCard(uid, GiveMonthCardSucess);
            }

        }

        void GiveMonthCardSucess(long uid)
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionMainHudController_14075"));

            if (GiveMonthCardView != null)
            {
                GiveMonthCardView.ShowUI(false);
            }

            if (MemberInfoView != null)
            {
                TweenScale TS = MemberInfoView.mDMono.GetComponent<TweenScale>();
                TS.ResetToBeginning();
                TS.PlayForward();
                MemberInfoView.ShowUI(true);
                MemberInfoView.SetData(legionData, uid);
            }
        }
        
        private void InitRedPoint()
        {
            LegionLogic.GetInstance().IsHaveApplyMember();
            LegionLogic.GetInstance().IsOpenActivity();
            LegionLogic.GetInstance().HaveDonateRP();
            LegionLogic.GetInstance().IsHaveTechRP();
            LegionLogic.GetInstance().IsHaveApplyMember();
            LegionLogic.GetInstance().IsHaveMercenary();
        }
    }
}
