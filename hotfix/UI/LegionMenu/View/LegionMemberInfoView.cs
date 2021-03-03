using Hotfix_LT.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMemberInfoView : DynamicMonoHotfix
    {

        public override void Awake()
        {
            base.Awake();
            isthisInit = false;
            headIcon = mDMono.transform.Find("Info/IconBG/Icon").GetComponent<UISprite>();
            bg = mDMono.transform.Find("BG").GetComponent<UISprite>();
			BgUp = mDMono.transform.Find("BG/BG2").GetComponent<UISprite>();
			BgDown = mDMono.transform.Find("BG/BG3").GetComponent<UISprite>();
			headFrame = mDMono.transform.Find("Info/IconBG/Icon/Frame").GetComponent<UISprite>();
			nameLab = mDMono.transform.Find("Info/Name").GetComponent<UILabel>();
            levelLab = mDMono.transform.Find("Info/LevelBG/Level").GetComponent<UILabel>();
            legionNameLab = mDMono.transform.Find("Info/AllianceName").GetComponent<UILabel>();
            addFriendBtn = mDMono.transform.Find("Info/AddFriendButton").GetComponent<UIButton>();
            talkBtn = mDMono.transform.Find("Info/TalkButton").GetComponent<UIButton>();
            promoteBtn = mDMono.transform.Find("Button/HideObj/PromoteButton").GetComponent<UIButton>();
            demoteBtn = mDMono.transform.Find("Button/HideObj/DemoteButton").GetComponent<UIButton>();
            giveOwnerBtn = mDMono.transform.Find("Button/HideObj/GiveOwnerButton").GetComponent<UIButton>();
            kickOutBtn = mDMono.transform.Find("Button/HideObj/KickOutButton").GetComponent<UIButton>();
            exitBtn = mDMono.transform.Find("BG/BG2/BackButton").GetComponent<UIButton>();
            backBGBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            hideButtonObj = mDMono.transform.Find("Button/HideObj").gameObject;
            formationInfo = mDMono.transform.Find("Formation").GetMonoILRComponent<PlayerFormationInfo>();
            if (addFriendBtn != null) addFriendBtn.onClick.Add(new EventDelegate(OnClickAddFriend));
            if (talkBtn != null) talkBtn.onClick.Add(new EventDelegate(OnClickTalk));
            if (promoteBtn != null) promoteBtn.onClick.Add(new EventDelegate(OnClickPromote));
            if (demoteBtn != null) demoteBtn.onClick.Add(new EventDelegate(OnClickDemote));
            if (giveOwnerBtn != null) giveOwnerBtn.onClick.Add(new EventDelegate(OnClickGiveOwner));
            if (kickOutBtn != null) kickOutBtn.onClick.Add(new EventDelegate(OnClickKickOut));
            if (exitBtn != null) exitBtn.onClick.Add(new EventDelegate(OnClickExit));
            if (backBGBtn != null) backBGBtn.onClick.Add(new EventDelegate(OnClickExit));
            //if (monthCardBtn != null) monthCardBtn.onClick.Add(new EventDelegate(OnClickMonthCard));
           
			ButtonTable = mDMono.transform.Find("Button").GetComponent<UIWidget>();
			Formation = mDMono.transform.Find("Formation").GetComponent<UIWidget>();
            isthisInit = true;

        }
        public UISprite headIcon;
        public UISprite headFrame;
        public UISprite bg;
        public UISprite BgUp;
        public UISprite BgDown;
        public UIWidget Formation;
        public UIWidget ButtonTable;
        public UILabel nameLab;
        public UILabel levelLab;
        public UILabel legionNameLab;
        public UIButton addFriendBtn;
        public UIButton talkBtn;
        public UIButton promoteBtn;
        public UIButton demoteBtn;
        public UIButton giveOwnerBtn;
        public UIButton kickOutBtn;
        public UIButton exitBtn;
        public UIButton backBGBtn;

        public GameObject hideButtonObj;

        public PlayerFormationInfo formationInfo;

        public Action<long> onClickAddFriend;
        public Action<long> onClickTalk;
        public Action<long> onClickPromote;
        public Action<long> onClickDemote;
        public Action<long> onClickGiveOwner;
        public Action<long> onClickKickOut;
        public Action<long> onClickMonthCard;

        private long _choiceUid;
        private LegionData legionData;
        private LegionMemberData mData;

        private bool isthisInit = false;
        private int bgFullHeight = 916;
        private int bgSmallHeight = 736;

        private bool isNeedCloseUI = false;
        private float clickTime;
        private string messageTips;

        public bool IsShow { get { return mDMono.gameObject.activeInHierarchy; } }


        public override void OnDestroy()
        {
            if (addFriendBtn != null) addFriendBtn.onClick.Clear();
            if (talkBtn != null) talkBtn.onClick.Clear();
            if (promoteBtn != null) promoteBtn.onClick.Clear();
            if (demoteBtn != null) demoteBtn.onClick.Clear();
            if (giveOwnerBtn != null) giveOwnerBtn.onClick.Clear();
            if (kickOutBtn != null) kickOutBtn.onClick.Clear();
            if (exitBtn != null) exitBtn.onClick.Clear();
            if (backBGBtn != null) backBGBtn.onClick.Clear();
            isthisInit = false;
        }

        public override void OnEnable()
        {
            LegionEvent.MemberPostChangeCallBack += ShowPersonage;
            LegionEvent.MessageCallBack += ShowMessage;
        }

        public override void OnDisable()
        {
            LegionEvent.MemberPostChangeCallBack -= ShowPersonage;
            LegionEvent.MessageCallBack -= ShowMessage;
        }

        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.SetActive(isShow);
        }

        public void SetData(LegionData legionData, long choiceUid, List<OtherPlayerPartnerData> partnerList = null)
        {
            if (legionData == null || legionData.userMemberData == null)
            {
                return;
            }

            this.legionData = legionData;

            if (legionData.userMemberData.dutyType == eAllianceMemberRole.Owner)
            {
                bg.height = bgFullHeight;
                hideButtonObj.SetActive(true);
                UpdateAnchors();
			}
            else if (legionData.userMemberData.dutyType == eAllianceMemberRole.ExtraOwner)
            {
                bg.height = bgFullHeight;
                hideButtonObj.SetActive(true);
				UpdateAnchors();
			}
            else
            {
                bg.height = bgSmallHeight;
                hideButtonObj.SetActive(false);
				UpdateAnchors();
			}

            if (choiceUid == 0)
            {
                if(isthisInit) SetBtnView();
                return;
            }

            formationInfo.Init(choiceUid, partnerList);

            _choiceUid = choiceUid;

            mData = null;
            for (int i = 0; i < legionData.listMember.Count; i++)
            {
                if (legionData.listMember[i].uid == _choiceUid)
                {
                    mData = legionData.listMember[i];
                    break;
                }
            }

            if (mData == null)
            {
                return;
            }

            ShowPersonage();
        }

        private void UpdateAnchors()
        {
			BgUp.UpdateAnchors();
			BgDown.UpdateAnchors();
			Formation.UpdateAnchors();
			ButtonTable.UpdateAnchors();
		}
        private void SetBtnView()
        {
            if (legionData.userMemberData.dutyType == eAllianceMemberRole.Owner)
            {
                LTUIUtil.SetGreyButtonEnable(kickOutBtn, true);
                LTUIUtil.SetGreyButtonEnable(giveOwnerBtn, mData?.offlineTime == 0);
                LTUIUtil.SetGreyButtonEnable(promoteBtn, mData?.dutyType != eAllianceMemberRole.ExtraOwner);
                LTUIUtil.SetGreyButtonEnable(demoteBtn, mData?.dutyType != eAllianceMemberRole.Member);
            }
            else if (legionData.userMemberData.dutyType == eAllianceMemberRole.ExtraOwner)
            {
                LTUIUtil.SetGreyButtonEnable(promoteBtn, false);
                LTUIUtil.SetGreyButtonEnable(demoteBtn, false);
                LTUIUtil.SetGreyButtonEnable(giveOwnerBtn, false);
                LTUIUtil.SetGreyButtonEnable(kickOutBtn, mData?.dutyType != eAllianceMemberRole.Owner && mData?.dutyType != eAllianceMemberRole.ExtraOwner);
            }
        }
        private void ShowPersonage()
        {
            nameLab.text = mData.memberName;
            levelLab.text = mData.level.ToString();
            legionNameLab.text = string.Format("【{0}】", legionData.legionName);
            headIcon.spriteName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(mData.templateId, mData.skin).icon;
            headFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(mData.headFrame).iconId;
            SetBtnView();
            int level = BalanceResourceUtil.GetUserLevel();
            int funLevel = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057).NeedLevel;
            LTUIUtil.SetGreyButtonEnable(talkBtn, mData.level >= funLevel && level >= funLevel);

            if (FriendManager.Instance.MyFriends.Find(_choiceUid) != null)
            {
                LTUIUtil.SetGreyButtonEnable(addFriendBtn, false);
            }
            else
            {
                LTUIUtil.SetGreyButtonEnable(addFriendBtn, true);
            }

            //FriendManager.Instance.Search("#" + _choiceUid, delegate (Hashtable result) {
            //    if (result == null)
            //    {
            //        EB.Debug.LogError("Search result=null");
            //        return;
            //    }
            //    bool isFriend = EB.Dot.Bool("friendsInfo.search." + _choiceUid + ".isFriend", result, false);
            //    if (isFriend)
            //    {
            //        LTUIUtil.SetGreyButtonEnable(addFriendBtn, false);
            //    }
            //    else
            //    {
            //        LTUIUtil.SetGreyButtonEnable(addFriendBtn, true);
            //    }
            //});
        }

        private string GetMemberPos(eAllianceMemberRole curPos, bool isPromote)
        {
            string str = "";
            int index = (int)curPos;

            if (isPromote)
            {
                index++;
            }
            else
            {
                index--;
            }

            str = LegionModel.GetInstance().GetMemberPosName((eAllianceMemberRole)index);

            return str;
        }

        private void ShowMessage()
        {
            if (!string.IsNullOrEmpty(messageTips))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, messageTips);
            }

            if (isNeedCloseUI)
            {
                ShowUI(false);
                isNeedCloseUI = false;
            }
        }

        private bool isCouldClick()
        {
            if (Time.time - clickTime < 0.5f)
            {
                return false;
            }
            clickTime = Time.time;
            return true;
        }

        void OnClickAddFriend()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            if (onClickAddFriend != null)
            {
                onClickAddFriend(_choiceUid);
            }

            LTUIUtil.SetGreyButtonEnable(addFriendBtn, false);
        }

        void OnClickTalk()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            if (onClickTalk != null)
            {
                onClickTalk(_choiceUid);
            }
        }

        void OnClickPromote()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            messageTips = string.Format(LegionConfig.GetLegionText("ID_LEGION_MEMBERINFO_TIP_MESSAGE1"), mData.memberName, GetMemberPos(mData.dutyType, true));

            if (onClickPromote != null)
            {
                onClickPromote(_choiceUid);
            }
        }

        void OnClickDemote()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            messageTips = string.Format(LegionConfig.GetLegionText("ID_LEGION_MEMBERINFO_TIP_MESSAGE2"), mData.memberName, GetMemberPos(mData.dutyType, false));

            if (onClickDemote != null)
            {
                onClickDemote(_choiceUid);
            }
        }

        void OnClickGiveOwner()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            messageTips = string.Format(LegionConfig.GetLegionText("ID_LEGION_MEMBERINFO_TIP_MESSAGE4"), mData.memberName);

            if (onClickGiveOwner != null)
            {
                onClickGiveOwner(_choiceUid);
                UpdateAnchors();
            }
        }

        void OnClickKickOut()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            messageTips = string.Format(LegionConfig.GetLegionText("ID_LEGION_MEMBERINFO_TIP_MESSAGE3"), mData.memberName);
            if (onClickKickOut != null)
            {
                isNeedCloseUI = true;
                onClickKickOut(_choiceUid);
            }
        }

        void OnClickExit()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            ShowUI(false);
        }

        void OnClickMonthCard()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!isCouldClick()) return;

            if (onClickMonthCard != null)
            {
                onClickMonthCard(_choiceUid);
            }
        }
    }
}
