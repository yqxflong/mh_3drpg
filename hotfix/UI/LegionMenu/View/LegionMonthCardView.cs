using Hotfix_LT.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMonthCardView : DynamicMonoHotfix { 

        public override void Awake()
        {
            base.Awake();

            levelLab = mDMono.transform.Find("Info/LevelSprite/LevelLabel").GetComponent<UILabel>();
            nameLab = mDMono.transform.Find("Info/NameLabel").GetComponent<UILabel>();
            contentLab = mDMono.transform.Find("Sprite/Label").GetComponent<UILabel>();
            headIcon = mDMono.transform.Find("Info/Border/Icon").GetComponent<UISprite>();
            headFrame = mDMono.transform.Find("Info/Border/Icon/Frame").GetComponent<UISprite>();
            cancelMonthCardBtn = mDMono.transform.Find("CancelBtn").GetComponent<UIButton>();
            giveMonthCardBtn = mDMono.transform.Find("GiveBtn").GetComponent<UIButton>();
            backBtn = mDMono.transform.Find("BackButton").GetComponent<UIButton>();
            backBGBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            if (cancelMonthCardBtn != null) cancelMonthCardBtn.onClick.Add(new EventDelegate(OnClickCancelMonthCard));
            if (backBtn != null) backBtn.onClick.Add(new EventDelegate(OnClickCancelMonthCard));
            if (backBGBtn != null) backBGBtn.onClick.Add(new EventDelegate(OnClickCancelMonthCard));
            if (giveMonthCardBtn != null) giveMonthCardBtn.onClick.Add(new EventDelegate(OnClickGiveMonthCard));
        }

        public UILabel levelLab;
        public UILabel nameLab;
        public UILabel contentLab;
        public UISprite headIcon;
        public UISprite headFrame;
        public UIButton cancelMonthCardBtn;
        public UIButton giveMonthCardBtn;
        public UIButton backBtn;
        public UIButton backBGBtn;

        public Action<long> onClickGiveMonthCard;
        public Action<long> onClickCancelMonthCard;

        private long uid;
        private LegionMemberData mData;

        public bool IsShow { get { return mDMono.gameObject.activeInHierarchy; } }

        public override void OnDestroy()
        {
            if (cancelMonthCardBtn != null) cancelMonthCardBtn.onClick.Clear();
            if (backBtn != null) backBtn.onClick.Clear();
            if (backBGBtn != null) backBGBtn.onClick.Clear();
            if (giveMonthCardBtn != null) giveMonthCardBtn.onClick.Clear();
        }

        public void ShowUI(bool isShow)
        {
            if (isShow)
            {
                TweenScale TS = mDMono.transform.GetComponent<TweenScale>();
                TS.ResetToBeginning();
                TS.PlayForward();
            }
            mDMono.gameObject.SetActive(isShow);
        }

        public void SetData(LegionData legionData, long choiceUid)
        {

            if (choiceUid == 0)
            {
                return;
            }

            uid = choiceUid;

            mData = null;
            for (int i = 0; i < legionData.listMember.Count; i++)
            {
                if (legionData.listMember[i].uid == uid)
                {
                    mData = legionData.listMember[i];
                    break;
                }
            }

            if (mData == null)
            {
                return;
            }

            nameLab.text = mData.memberName;
            levelLab.text = mData.level.ToString();
            headIcon.spriteName = string.Format("{0}{1}", mData.portrait, (mData.skin > 0) ? "_" + mData.skin : string.Empty);// "Partner_Head_Sidatuila"; // 暂时默认
            headFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(mData.headFrame).iconId;
            contentLab.text = EB.Localizer.GetString("ID_uifont_in_LTLegionMainMenu_Label_41");
        }
        public void OnClickCancelMonthCard()
        {
            if (onClickCancelMonthCard != null)
            {
                onClickCancelMonthCard(uid);
            }
        }

        void OnClickGiveMonthCard()
        {
            if (onClickGiveMonthCard != null)
            {
                onClickGiveMonthCard(uid);
            }
        }

    }
}

