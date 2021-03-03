using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LegionIconEditView : DynamicMonoHotfix
    {
        public UIButton CloseBtn;
        public UIButton BackBGBtn;
        public UIButton EditBtn;
        public UISprite Icon;
        public UISprite IconBG;
        public GameObject[] IconButtons = new GameObject[12];
        public GameObject[] IconBGButtons = new GameObject[4];
        public GameObject[] IconSelects = new GameObject[12];
        public GameObject[] IconBGSelects = new GameObject[4];

        private TweenScale mTs;

        private int iconID;  // 头像总ID，如102，则是以1号为底以2号为面的头像

        private int curIconID;   // 当前头像面ID
        private int curIconBGID; // 当前头像底ID
        private int oldIconID;   // 上一个头像面ID
        private int oldIconBGID; // 上一个头像底ID

        public bool IsShow { get { return mDMono.gameObject.activeInHierarchy; } }

        public override void OnDestroy()
        {
            if (CloseBtn != null) CloseBtn.onClick.Clear();
            if (BackBGBtn != null) BackBGBtn.onClick.Clear();
            if (EditBtn != null) EditBtn.onClick.Clear();
        }

        public void SetData(int iconID)
        {
            string iconStr = LegionModel.GetInstance().dicLegionSpriteName[iconID % 100];
            string iconBGStr = LegionModel.GetInstance().dicLegionBGSpriteName[iconID / 100];
            SetData(iconStr, iconBGStr, iconID);
        }

        public void SetData(string iconStr, string iconBGStr, int iconID)
        {
            Icon.spriteName = iconStr;
            IconBG.spriteName = iconBGStr;

            this.iconID = iconID;
            curIconID = oldIconID = iconID % 100;
            curIconBGID = oldIconBGID = iconID / 100;

            for (int i = 0; i < IconSelects.Length; i++)
            {
                IconSelects[i].SetActive(false);
            }

            for (int i = 0; i < IconBGSelects.Length; i++)
            {
                IconBGSelects[i].SetActive(false);
            }

            IconSelects[curIconID].SetActive(true);
            IconBGSelects[curIconBGID].SetActive(true);

            ShowUI(true);
        }

        private void OnClickIconBtn(GameObject obj)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int tempIconID = Array.IndexOf(IconButtons,obj);
            if (curIconID == tempIconID)
            {
                return;
            }

            curIconID = tempIconID;
            Icon.spriteName = LegionModel.GetInstance().dicLegionSpriteName[curIconID];
            IconSelects[curIconID].SetActive(true);
            IconSelects[oldIconID].SetActive(false);
            oldIconID = curIconID;
        }

        private void OnClickIconBGBtn(GameObject obj)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int tempIconBGID = Array.IndexOf(IconBGButtons,obj);
            if (curIconBGID == tempIconBGID)
            {
                return;
            }

            curIconBGID = tempIconBGID;
            IconBG.spriteName = LegionModel.GetInstance().dicLegionBGSpriteName[curIconBGID];
            IconBGSelects[curIconBGID].SetActive(true);
            IconBGSelects[oldIconBGID].SetActive(false);
            oldIconBGID = curIconBGID;
        }

        private void OnClickConfirmBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int tempIconID = curIconBGID * 100 + curIconID;
            Messenger.Raise(Hotfix_LT.EventName.LegionIconIDEdit,tempIconID);
            mDMono.gameObject.SetActive(false);
        }

        public void ShowUI(bool isShow)
        {
            if (isShow)
            {
                if (mTs == null)
                {
                    mTs = mDMono.transform.GetComponent<TweenScale>();
                }
                mTs.ResetToBeginning();
                mTs.PlayForward();
            }
            mDMono.gameObject.SetActive(isShow);
        }

        private void OnClickCloseBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            mDMono.gameObject.SetActive(false);
        }
    



        public override void Awake()
        {
            base.Awake();

            CloseBtn = mDMono.transform.Find("BackButton").GetComponent<UIButton>();
            BackBGBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            EditBtn = mDMono.transform.Find("Left/EditNameButton").GetComponent<UIButton>();
            Icon = mDMono.transform.Find("Left/Badge/LegionIcon").GetComponent<UISprite>();
            IconBG = mDMono.transform.Find("Left/Badge/IconBG").GetComponent<UISprite>();
            IconButtons[0] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item").gameObject;
            IconButtons[1] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (1)").gameObject;
            IconButtons[2] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (2)").gameObject;
            IconButtons[3] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (3)").gameObject;
            IconButtons[4] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (4)").gameObject;
            IconButtons[5] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (5)").gameObject;
            IconButtons[6] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (6)").gameObject;
            IconButtons[7] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (7)").gameObject;
            IconButtons[8] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (8)").gameObject;
            IconButtons[9] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (9)").gameObject;
            IconButtons[10] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (10)").gameObject;
            IconButtons[11] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (11)").gameObject;
            IconBGButtons[0] = mDMono.transform.Find("Right/Top/Grid/Item").gameObject;
            IconBGButtons[1] = mDMono.transform.Find("Right/Top/Grid/Item (1)").gameObject;
            IconBGButtons[2] = mDMono.transform.Find("Right/Top/Grid/Item (2)").gameObject;
            IconBGButtons[3] = mDMono.transform.Find("Right/Top/Grid/Item (3)").gameObject;
            IconSelects[0] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item/SelectBG").gameObject;
            IconSelects[1] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (1)/SelectBG").gameObject;
            IconSelects[2] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (2)/SelectBG").gameObject;
            IconSelects[3] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (3)/SelectBG").gameObject;
            IconSelects[4] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (4)/SelectBG").gameObject;
            IconSelects[5] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (5)/SelectBG").gameObject;
            IconSelects[6] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (6)/SelectBG").gameObject;
            IconSelects[7] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (7)/SelectBG").gameObject;
            IconSelects[8] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (8)/SelectBG").gameObject;
            IconSelects[9] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (9)/SelectBG").gameObject;
            IconSelects[10] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (10)/SelectBG").gameObject;
            IconSelects[11] = mDMono.transform.Find("Right/Bottom/Scroll View/Grid/Item (11)/SelectBG").gameObject;
            IconBGSelects[0] = mDMono.transform.Find("Right/Top/Grid/Item/SelectBG").gameObject;
            IconBGSelects[1] = mDMono.transform.Find("Right/Top/Grid/Item (1)/SelectBG").gameObject;
            IconBGSelects[2] = mDMono.transform.Find("Right/Top/Grid/Item (2)/SelectBG").gameObject;
            IconBGSelects[3] = mDMono.transform.Find("Right/Top/Grid/Item (3)/SelectBG").gameObject;
            if (CloseBtn != null) CloseBtn.onClick.Add(new EventDelegate(OnClickCloseBtn));
            if (BackBGBtn != null) BackBGBtn.onClick.Add(new EventDelegate(OnClickCloseBtn));
            if (EditBtn != null) EditBtn.onClick.Add(new EventDelegate(OnClickConfirmBtn));

            for (int i = 0; i < IconButtons.Length; i++)
            {
                if (IconButtons[i] != null)
                {
                    UIEventListener.Get(IconButtons[i]).onClick = OnClickIconBtn;
                }
            }

            for (int i = 0; i < IconBGButtons.Length; i++)
            {
                if (IconBGButtons[i] != null)
                {
                    UIEventListener.Get(IconBGButtons[i]).onClick = OnClickIconBGBtn;
                }
            }
        }

    }

}
