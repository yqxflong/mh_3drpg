using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 军团搜索界面ScrollView内的动态item组件
/// </summary>
/// 
namespace Hotfix_LT.UI
{
    public class LegionSearchItem : DynamicCellController<SearchItemData>
    {
        private int legionID;

        public UISprite BG;

        public UIButton applyBtn;
        public UIButton cancelApplyBtn;

        public UILabel conditionLabel;
        public UILabel conditionNoteLabel;

        public UILabel peopleNumLabel;

        public UILabel legionLevelLabel;

        public UILabel legionNameLabel;

        public UISprite lgionIcon;
        public UISprite iconBG;

        public UILabel applyLabel;

        public Action<int, bool> onItemApply;

        /// <summary>
        /// 是否能够点击
        /// </summary>
        private bool isCouldClick = true;

        private bool _isActiveSend;
        public bool IsActiveSend
        {
            get { return _isActiveSend; }
            set
            {
                _isActiveSend = value;
                if (applyBtn != null)
                {
                    LTUIUtil.SetGreyButtonEnable(applyBtn, value);
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            BG = mDMono.transform.Find("BG").GetComponent<UISprite>();
            applyBtn = mDMono.transform.Find("ShenqingButton").GetComponent<UIButton>();
            cancelApplyBtn = mDMono.transform.Find("CancelJoinButton").GetComponent<UIButton>();
            conditionLabel = mDMono.transform.Find("ConditionLabel").GetComponent<UILabel>();
            conditionNoteLabel = mDMono.transform.Find("NoteLabel").GetComponent<UILabel>();
            peopleNumLabel = mDMono.transform.Find("PeopleNumLabel").GetComponent<UILabel>();
            legionLevelLabel = mDMono.transform.Find("Level/LevelLabel").GetComponent<UILabel>();
            legionNameLabel = mDMono.transform.Find("NameLabel").GetComponent<UILabel>();
            lgionIcon = mDMono.transform.Find("Badge/LegionIcon").GetComponent<UISprite>();
            iconBG = mDMono.transform.Find("Badge/IconBG").GetComponent<UISprite>();
            applyLabel = mDMono.transform.Find("ShenqingButton/Label").GetComponent<UILabel>();
        }


        public override void Start()
        {
            if (applyBtn != null && applyBtn.onClick.Count == 0)
            {
                applyBtn.onClick.Add(new EventDelegate(OnClickApply));
            }
            if (cancelApplyBtn != null && cancelApplyBtn.onClick.Count == 0)
            {
                cancelApplyBtn.onClick.Add(new EventDelegate(OnClickCancelApply));
            }
            Hotfix_LT.Messenger.AddListener<int >(Hotfix_LT.EventName.LegionSearchMessageCallBack, LegionSearchMessageCallBackFucc);
        }

        public override void OnDestroy()
        {
            if (applyBtn != null)
            {
                applyBtn.onClick.Clear();
            }
            if (cancelApplyBtn != null)
            {
                cancelApplyBtn.onClick.Clear();
            }
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.LegionSearchMessageCallBack, LegionSearchMessageCallBackFucc);
        }

        void OnClickApply()
        {
            if (!isCouldClick)
            {
                return;
            }

            if (LegionEvent.SendApplyJoinLegion != null)
            {
                LegionEvent.SendApplyJoinLegion(legionID);
            }
        }

        void OnClickCancelApply()
        {
            if (!isCouldClick)
            {
                return;
            }

            if (LegionEvent.SendCancelApplyJoinLegion != null)
            {
                LegionEvent.SendCancelApplyJoinLegion(legionID);
            }
        }

        private void LegionSearchMessageCallBackFucc(int evt)
        {
            if (evt == 2) isCouldClick = false;
            else if (evt == 3) isCouldClick = true;
        }

        public void SetData(SearchItemData data)
        {
            if (data == null)
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            else
            {
                mDMono.gameObject.CustomSetActive(true);
            }

            legionID = data.id;
            LTUIUtil.SetText(legionNameLabel, data.legionName);
            LTUIUtil.SetText(legionLevelLabel, data.legionLevel.ToString());
            LTUIUtil.SetText(peopleNumLabel, data.currentPeopleNum.ToString() + "/" + data.maxPeopleNum.ToString());

            if (data.iconSpriteName != null)
            {
                lgionIcon.spriteName = data.iconSpriteName;
                iconBG.spriteName = data.iconBGSpriteName;
            }

            if (data.conditionLevel > 0)
            {
                LTUIUtil.SetText(conditionLabel, string.Format(EB.Localizer.GetString("ID_LEVEL_FORMAT"), data.conditionLevel));
            }
            else
            {
                LTUIUtil.SetText(conditionLabel, EB.Localizer.GetString("ID_codefont_in_LegionSearchItem_2268"));
            }

            if (data.isNeedApprove)
            {
                LTUIUtil.SetText(conditionNoteLabel, EB.Localizer.GetString("ID_codefont_in_LegionSearchItem_2385"));
            }
            else
            {
                LTUIUtil.SetText(conditionNoteLabel, EB.Localizer.GetString("ID_codefont_in_LegionSearchItem_2477"));
            }

            IsActiveSend = data.isReachCondition;

            if (data.currentPeopleNum >= data.maxPeopleNum)
            {
                LTUIUtil.SetText(applyLabel, EB.Localizer.GetString("ID_codefont_in_LegionSearchItem_2651"));
            }
            else if (data.isHasApplyed)
            {
                LTUIUtil.SetText(applyLabel, EB.Localizer.GetString("ID_codefont_in_LegionSearchItem_2755"));

                if (cancelApplyBtn != null)
                {
                    applyBtn.gameObject.CustomSetActive(false);
                    cancelApplyBtn.gameObject.CustomSetActive(true);
                }
            }
            else
            {
                LTUIUtil.SetText(applyLabel, EB.Localizer.GetString("ID_codefont_in_LegionSearchItem_3019"));
                if (cancelApplyBtn != null)
                {
                    applyBtn.gameObject.CustomSetActive(true);
                    cancelApplyBtn.gameObject.CustomSetActive(false);
                }
            }
        }

        public void SetItemBG(int index)
        {
            // index: 0：浅底，1：深底
            BG.spriteName = index % 2 == 0 ? "Ty_Mail_Di1" : "Ty_Mail_Di2";
        }

        public override void Fill(SearchItemData itemData)
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