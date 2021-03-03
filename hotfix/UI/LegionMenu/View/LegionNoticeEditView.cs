using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionNoticeEditView : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            CloseBtn = mDMono.transform.Find("BackButton").GetComponent<UIButton>();
            BackBgBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            ConfirmBtn = mDMono.transform.Find("ConfirmBtn").GetComponent<UIButton>();
            InputContentLabel = mDMono.transform.Find("ContentInputSprite/ContentInputLabel").GetComponent<UILabel>();
            ConfirmBtn.onClick.Add(new EventDelegate(OnConfirmBtn));
            CloseBtn.onClick.Add(new EventDelegate(OnClickClose));
            BackBgBtn.onClick.Add(new EventDelegate(OnClickClose));
        }

        public UIButton CloseBtn;
        public UIButton BackBgBtn;
        public UIButton ConfirmBtn;
        public UILabel InputContentLabel;

        private string noInputContentStr;
        private string noticeStr;

        public Action<string> onClickSaveNotice;

        public bool IsShow { get { return mDMono.gameObject.activeInHierarchy; } }

        public override void Start()
        {
            noInputContentStr = InputContentLabel.text;
        }

        public override void OnEnable()
        {
            if (InputContentLabel != null)
            {
                InputContentLabel.text = noInputContentStr;
                InputContentLabel.GetComponent<UIInput>().value = string.Empty;
            }
        }

        public override void OnDestroy()
        {
            ConfirmBtn.onClick.Clear();
            CloseBtn.onClick.Clear();
            BackBgBtn.onClick.Clear();
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

        public void SetData(string noticeStr)
        {
            this.noticeStr = noticeStr;
            InputContentLabel.text = noticeStr.Equals(string.Empty) ? InputContentLabel.text : noticeStr;
        }

        private void OnClickClose()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            mDMono.gameObject.SetActive(false);
        }

        private void OnConfirmBtn()
        {
            if (InputContentLabel.text.Equals(noInputContentStr) || InputContentLabel.text.Equals(noticeStr))
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionNoticeEditView_1931"));
                return;
            }

            if (onClickSaveNotice != null)
            {
                onClickSaveNotice(InputContentLabel.text);
                OnClickClose();
            }
            else
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
            }
        }
    }
}
