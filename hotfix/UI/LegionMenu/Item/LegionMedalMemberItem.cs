using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMedalMemberItem : DynamicMonoHotfix
    {
        UIButton HotfixBtn0;
        public override void Awake()
        {
            base.Awake();

            HeadIconSp = mDMono.transform.Find("Member/Border/Icon").GetComponent<UISprite>();
            LevelLab = mDMono.transform.Find("Member/LevelSprite/LevelLabel").GetComponent<UILabel>();
            NameLab = mDMono.transform.Find("Member/NameLabel").GetComponent<UILabel>();
            HotfixBtn0 = mDMono.transform.Find("Member/GiveBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnClickGiveBtn));
            FrameIconSp= mDMono.transform.Find("Member/Border/Icon/Frame").GetComponent<UISprite>();
        }
        /// <summary>
        /// 头像
        /// </summary>
        public UISprite HeadIconSp;

        /// <summary>
        /// 头像框
        /// </summary>
        public UISprite FrameIconSp;

        /// <summary>
        /// 等级
        /// </summary>
        public UILabel LevelLab;

        /// <summary>
        /// 名字
        /// </summary>
        public UILabel NameLab;

        /// <summary>
        /// 当前的公会成员数据
        /// </summary>
        private LegionMemberData mMemberData;

        /// <summary>
        /// 赠送完成的事件
        /// </summary>
        private System.Action mSendAction;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data">公会成员数据</param>
        /// <param name="index">数据序号</param>
        public void Init(LegionMemberData data)
        {
            mMemberData = data;
            HeadIconSp.spriteName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(data.templateId, data.skin).icon;
            FrameIconSp.spriteName = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetHeadFrame(data.headFrame).iconId;
            LevelLab.text = mMemberData.level.ToString();
            NameLab.text = mMemberData.memberName;

        }

        /// <summary>
        /// 显示Item
        /// </summary>
        /// <param name="isShow">是否显示</param>
        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);
        }

        /// <summary>
        /// 设置赠送事件
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(System.Action action)
        {
            mSendAction = action;
        }

        /// <summary>
        /// 点击赠与按钮
        /// </summary>
        public void OnClickGiveBtn()
        {
            if (mMemberData == null)
            {
                EB.Debug.LogError("LegionMedalMemberItem OnClickGiveBtn is Error, mMemberData is Null");
                return;
            }

            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, string.Format(EB.Localizer.GetString("ID_LEGION_MEDAL_GIVE_CONFIRM"), mMemberData.memberName), delegate (int result)
            {
                if (result == 0)
                {
                    LegionLogic.GetInstance().SendMedal(mMemberData.uid, delegate
                    {
                        if (mSendAction != null)
                        {
                            mSendAction();
                        }

                        ArrayList aList = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("AllianceMedalSendReward")) as ArrayList;

                        List<LTShowItemData> showItemsList = new List<LTShowItemData>();
                        for (int i = 0; i < aList.Count; i++)
                        {
                            string id = EB.Dot.String("data", aList[i], string.Empty);
                            int count = EB.Dot.Integer("quantity", aList[i], 0);
                            string type = EB.Dot.String("type", aList[i], string.Empty);
                            if (!string.IsNullOrEmpty(id))
                            {
                                LTShowItemData showItemData = new LTShowItemData(id, count, type);
                                showItemsList.Add(showItemData);
                            }
                        }

                        GlobalMenuManager.Instance.Open("LTShowRewardView", showItemsList);
                    });

                }
            });
        }

    }
}

