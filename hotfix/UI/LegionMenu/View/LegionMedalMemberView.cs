using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMedalMemberView : DynamicMonoHotfix
    {
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        public override void Awake()
        {
            base.Awake();

            mGrid = mDMono.transform.Find("Scroll View/Grid").GetComponent<UIGrid>();
            mTemplateItem = mDMono.transform.Find("Template/Item").GetMonoILRComponent<LegionMedalMemberItem>();
            HotfixBtn0 = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnClickCloseBtn));
            HotfixBtn1 = mDMono.transform.Find("CloseBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnClickCloseBtn));
        }



        /// <summary>
        /// item的grid
        /// </summary>
        public UIGrid mGrid;

        /// <summary>
        /// 模板公会成员的Item
        /// </summary>
        public LegionMedalMemberItem mTemplateItem;

        /// <summary>
        /// 所有的公会成员的Item
        /// </summary>
        private List<LegionMedalMemberItem> mMemberItemList;

        /// <summary>
        /// 所有公会成员数据
        /// </summary>
        private List<LegionMemberData> mLegionMemberDataList;

        /// <summary>
        /// 赠送完成的事件
        /// </summary>
        private System.Action mSendAction;

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="isShow">是否显示</param>
        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.CustomSetActive(isShow);

            if (isShow)
            {
                InitUI();
            }
        }

        /// <summary>
        /// 设置所有公会成员数据
        /// </summary>
        /// <param name="legionMemberDataList"></param>
        public void SetLegionMemberData(List<LegionMemberData> legionMemberDataList)
        {
            mLegionMemberDataList = legionMemberDataList;

            for (int i = 0; i < mLegionMemberDataList.Count; i++)
            {
                if (mLegionMemberDataList[i].uid ==LoginManager.Instance.LocalUserId.Value)
                {
                    mLegionMemberDataList.RemoveAt(i);
                    break;
                }
            }

            if (mDMono.gameObject.activeInHierarchy)
            {
                MessageDialog.HideCurrent();
                InitUI();
            }
        }

        /// <summary>
        /// 设置赠送事件
        /// </summary>
        /// <param name="action"></param>
        public void SetAction(System.Action action)
        {
            mSendAction = delegate
            {
                ShowUI(false);
                if (action != null)
                {
                    action();
                }
            };
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitUI()
        {
            if (mMemberItemList == null)
            {
                mMemberItemList = new List<LegionMedalMemberItem>();
            }

            int count = mLegionMemberDataList.Count - mMemberItemList.Count;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Transform tf = UnityEngine.Object.Instantiate(mTemplateItem.mDMono.gameObject).transform;
                    tf.SetParent(mGrid.transform);
                    tf.localPosition = Vector3.zero;
                    tf.localEulerAngles = Vector3.zero;
                    tf.localScale = Vector3.one;
                    LegionMedalMemberItem item = tf.GetMonoILRComponent<LegionMedalMemberItem>();
                    mMemberItemList.Add(item);
                }
            }

            for (int i = 0; i < mMemberItemList.Count; i++)
            {
                if (i < mLegionMemberDataList.Count)
                {
                    mMemberItemList[i].ShowUI(true);
                    mMemberItemList[i].SetAction(mSendAction);
                    mMemberItemList[i].Init(mLegionMemberDataList[i]);
                }
                else
                {
                    mMemberItemList[i].ShowUI(false);
                }
            }

            mGrid.Reposition();
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        public void OnClickCloseBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            ShowUI(false);
        }
    }
}