using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionPageMember : DynamicMonoHotfix
    {

        public UISprite legionIconSpt;
        public UISprite iconBGSp;
        public UILabel legionNameLabel;
        public UILabel legionLevelLabel;
        public UILabel expLabel;
        public UILabel peopleNumLabel;
        public UILabel bossNameLabel;
        public UILabel logLabel;
        public UIButton logWriteSaveBtn;
        public UIButton managerBtn;
        public UIButton mailBtn;
        public UIButton leaveLegionBtn;
        public UIButton rankBtn;
        public UIButton shopBtn;
        public UIButton editNameBtn;
        public UIScrollView scrollView;
        public UIGrid grid;
        private bool isShow = false;
        public GameObject managetRedPointObj;
        public LegionMemberScroll memberItemScroll;

        public Action onClickShowMail;
        public Action onClickManager;
        public Action onClickLeaveLegion;
        public Action onClickNoticeEditBtn;
        public Action onClickEditNameBtn;

        public enum State
        {
            Member,
            Admin,
        }

        private State _state;
        public State PageMemberState
        {
            get { return _state; }
            set
            {
                switch (value)
                {
                    case State.Admin:
                        ChangeShow(true);
                        break;
                    case State.Member:
                        ChangeShow(false);
                        break;
                }
            }
        }

        public override  void OnDestroy()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.haveapply, SetApplyRP);
            managerBtn.onClick.Clear();
            mailBtn.onClick.Clear();
            leaveLegionBtn.onClick.Clear();
            rankBtn.onClick.Clear();
            shopBtn.onClick.Clear();
            logWriteSaveBtn.onClick.Clear();
        }

        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);
            this.isShow = isShow;
            if (isShow)
            {
                memberItemScroll.SetItemDatas(LegionModel.GetInstance().legionData.listMember.ToArray());
            }
        }

        void OnClickManager()
        {
            if (onClickManager != null)
            {
                onClickManager();
            }
        }
        void OnClickMail()
        {
            if (onClickShowMail != null)
            {
                onClickShowMail();
            }
        }
        void OnClickLeaveLegion()
        {
            if (onClickLeaveLegion != null)
            {
                onClickLeaveLegion();
            }
        }

        void OnClickRank()
        {
            GlobalMenuManager.Instance.Open("LTRankListHud", "Alliance");
        }

        void OnClickShop()
        {
            GlobalMenuManager.Instance.Open("LTStoreUI", "alliance");
        }

        void OnClickNoticeEdit()
        {
            if (onClickNoticeEditBtn != null)
            {
                onClickNoticeEditBtn();
            }
        }


        public void SetData(LegionData data)
        {
            if (!data.legionIconSptName.Equals(""))
            {
                legionIconSpt.spriteName = data.legionIconSptName;
            }
            iconBGSp.spriteName = data.legionIconBGSptName;

            LTUIUtil.SetText(legionNameLabel, data.legionName);
            LTUIUtil.SetText(legionLevelLabel, data.legionLevel.ToString());
            LTUIUtil.SetText(expLabel, string.Format("{0}/{1}", data.currentExp, data.growupExp));
            LTUIUtil.SetText(peopleNumLabel, string.Format("{0}/{1}", data.currentPeopleNum, data.maxPeopleNum));
            LTUIUtil.SetText(bossNameLabel, data.ownerName);
            if (data.notice != null)
            {
                LTUIUtil.SetText(logLabel, data.notice.Equals("") ? logLabel.text : data.notice);
            }
            if (isShow)
            {
                memberItemScroll.SetItemDatas(data.listMember.ToArray());
            }
            //权限
            if (data.userMemberData != null)
            {
                editNameBtn.gameObject.CustomSetActive(data.userMemberData.dutyType == eAllianceMemberRole.Owner);
                switch (data.userMemberData.dutyType)
                {
                    case eAllianceMemberRole.Member:
                        PageMemberState = State.Member;
                        ChangeOpen(false, false, false);
                        break;
                    case eAllianceMemberRole.Admin:
                        PageMemberState = State.Admin;
                        ChangeOpen(true, false, false);
                        break;
                    case eAllianceMemberRole.ExtraOwner:
                        PageMemberState = State.Admin;
                        ChangeOpen(true, true, false);
                        break;
                    case eAllianceMemberRole.Owner:
                        PageMemberState = State.Admin;
                        ChangeOpen(true, true, true);
                        break;
                }
            }

            if (scrollView != null)
            {
                scrollView.RestrictWithinBounds(true);
            }
        }


        private void ChangeShow(bool IsAdmin)
        {
            if (mailBtn != null)
            {
                mailBtn.gameObject.CustomSetActive(IsAdmin);
            }

            if (managerBtn != null)
            {
                LTUIUtil.SetGreyButtonEnable(managerBtn, IsAdmin);
            }

        }

        private void ChangeOpen(bool isAdminApply, bool isAdminNotice, bool isAdminEmail)
        {
            if (mailBtn != null)
            {
                LTUIUtil.SetGreyButtonEnable(mailBtn, isAdminEmail);
            }

            if (managerBtn != null)
            {
                LTUIUtil.SetGreyButtonEnable(managerBtn, isAdminApply);
            }

            if (logWriteSaveBtn != null)
            {
                logWriteSaveBtn.gameObject.CustomSetActive(isAdminNotice);
            }
        }

        private void OnClickEditNameBtn()
        {
            if (onClickEditNameBtn != null)
            {
                onClickEditNameBtn();
            }
        }
        
        public override void Awake()
        {
            base.Awake();

            legionIconSpt = mDMono.transform.Find("Left/Top/Badge/LegionIcon").GetComponent<UISprite>();
            iconBGSp = mDMono.transform.Find("Left/Top/Badge/IconBG").GetComponent<UISprite>();
            legionNameLabel = mDMono.transform.Find("Left/Top/NameLabel").GetComponent<UILabel>();
            legionLevelLabel = mDMono.transform.Find("Left/Top/LevelSprite/LevelLabel").GetComponent<UILabel>();
            expLabel = mDMono.transform.Find("Left/LegionExp/ExpLabel").GetComponent<UILabel>();
            peopleNumLabel = mDMono.transform.Find("Left/MemberNum/MemberNumLabel").GetComponent<UILabel>();
            bossNameLabel = mDMono.transform.Find("Left/LegionLeader/LeaderLabel").GetComponent<UILabel>();
            logLabel = mDMono.transform.Find("Left/Notice/LogLabel").GetComponent<UILabel>();
            logWriteSaveBtn = mDMono.transform.Find("Left/Notice/NoticeEditBtn").GetComponent<UIButton>();
            managerBtn = mDMono.transform.Find("Button/AdminButton").GetComponent<UIButton>();
            mailBtn = mDMono.transform.Find("Left/Top/MailButton").GetComponent<UIButton>();
            leaveLegionBtn = mDMono.transform.Find("Button/LeaveButton").GetComponent<UIButton>();
            rankBtn = mDMono.transform.Find("Button/RankButton").GetComponent<UIButton>();
            shopBtn = mDMono.transform.Find("Button/ShopButton").GetComponent<UIButton>();
            editNameBtn = mDMono.transform.Find("Left/Top/EditBtn").GetComponent<UIButton>();
            scrollView = mDMono.transform.Find("Right/Scroll View").GetComponent<UIScrollView>();
            grid = mDMono.transform.Find("Right/Scroll View/holder/Grid").GetComponent<UIGrid>();
            managetRedPointObj = mDMono.transform.Find("Button/AdminButton/RedPoint").gameObject;
            memberItemScroll = mDMono.transform.Find("Right/Scroll View/holder/Grid").GetMonoILRComponent<LegionMemberScroll>();
            managerBtn.onClick.Add(new EventDelegate(OnClickManager));
            mailBtn.onClick.Add(new EventDelegate(OnClickMail));
            leaveLegionBtn.onClick.Add(new EventDelegate(OnClickLeaveLegion));
            rankBtn.onClick.Add(new EventDelegate(OnClickRank));
            shopBtn.onClick.Add(new EventDelegate(OnClickShop));
            logWriteSaveBtn.onClick.Add(new EventDelegate(OnClickNoticeEdit));
            editNameBtn.onClick.Add(new EventDelegate(OnClickEditNameBtn));

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.haveapply, SetApplyRP);
        }

        private void SetApplyRP(RedPointNode node)
        {
            managetRedPointObj.CustomSetActive(node.num > 0);
        }

    }

}
