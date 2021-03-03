using Hotfix_LT.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMemberItem : DynamicCellController<LegionMemberData>
    {
        public override void Awake()
        {
            base.Awake();

            BG = mDMono.transform.Find("BG").GetComponent<UISprite>();
            HeadIcon = mDMono.transform.Find("Border/Icon").GetComponent<UISprite>();
            HeadFrame = mDMono.transform.Find("Border/Icon/Frame").GetComponent<UISprite>();
            Level = mDMono.transform.Find("LevelSprite/LevelLabel").GetComponent<UILabel>();
            Name = mDMono.transform.Find("NameLabel").GetComponent<UILabel>();
            Duty = mDMono.transform.Find("DutyLabel").GetComponent<UILabel>();
            Contribute = mDMono.transform.Find("ContributeLabel").GetComponent<UILabel>();
            ContributeShadow = mDMono.transform.Find("ContributeLabel/ContributeLabel(Clone)").GetComponent<UILabel>();
            State = mDMono.transform.Find("StateLabel").GetComponent<UILabel>();
            ItemBtn = mDMono.GetComponent<UIButton>();
            SelfBGObj = mDMono.transform.Find("SelfBG").gameObject;
            ItemBtn.onClick.Add(new EventDelegate(OnClickItem));

        }

        public UISprite BG;
        public UISprite HeadIcon;
        public UISprite HeadFrame;
        public UILabel Level;
        public UILabel Name;
        public UILabel Duty;
        public UILabel Contribute, ContributeShadow;
        public UILabel State;
        public UIButton ItemBtn;
        public GameObject SelfBGObj;

        private long uid;
        private LegionMemberData memberData;

        public override void OnDestroy()
        {
            ItemBtn.onClick.Clear();
        }

        public void SetData(LegionMemberData data)
        {
            try
            {
                if (data == null)
                {
                    mDMono.gameObject.CustomSetActive(false);
                    return;
                }

                mDMono.gameObject.CustomSetActive(true);
                memberData = data;
                HeadIcon.spriteName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.templateId, data.skin).icon;
                HeadFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(data.headFrame).iconId;
                uid = data.uid;
                Name.text = data.memberName;
                Contribute.text = ContributeShadow.text = string.Format("[fff348]{0}[-]/{1}", GameUtils.ApplyNumFormat(data.todaydonate,true), GameUtils.ApplyNumFormat(data.totaldonate, true));
                Level.text = data.level.ToString();
                Duty.text = data.dutyTypeStr;
                bool isSelf = data.uid ==LoginManager.Instance.LocalUserId.Value;
                SelfBGObj.CustomSetActive(isSelf);

                if (data.offlineTime == 0)
                {
                    State.text = string.Format("[42fe79]{0}[-]",EB.Localizer.GetString("ID_ON_LINE"));
                    HeadIcon.color = HeadFrame.color= Color.white;
                }
                else
                {
                    HeadIcon.color = HeadFrame.color = Color.magenta;
                    if (data.offlineHour < 24)
                    {
                        State.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMemberItem_1531"), LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal);
                    }
                    else if (data.offlineHour < 168)
                    {
                        State.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMemberItem_1689"), LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal, (data.offlineHour / 24).ToString());
                    }
                    else
                    {
                        State.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMemberItem_1856"), LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal);
                    }
                }
            }
            catch(System.NullReferenceException e)
            {
                EB.Debug.LogError(e.ToString());
            }
        }

        public void SetItemBG(int index)
        {
            // index: 0：浅底，1：深底
            BG.spriteName = index % 2 == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
        }

        void OnClickItem()
        {
            if (LegionEvent.OnClickMember != null)
            {

                LegionEvent.OnClickMember(uid);
            }
        }

        public override void Fill(LegionMemberData itemData)
        {
            SetData(itemData);
            SetItemBG(DataIndex);
        }

        public override void Clean()
        {
            SetData(null);
        }

    }

}