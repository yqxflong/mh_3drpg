using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class LegionMailView : DynamicMonoHotfix
    {

        public override void Awake()
        {
            base.Awake();

            NameLabel = mDMono.transform.Find("Addresser/NameLabel").GetComponent<UILabel>();
            InputTitleLabel = mDMono.transform.Find("MailTitle/InputLabel").GetComponent<UILabel>();
            InputContentLabel = mDMono.transform.Find("ContentInputSprite/ContentInputLabel").GetComponent<UILabel>();
            LessTipsLabel = mDMono.transform.Find("LessTipsLabel").GetComponent<UILabel>();
            SendMailBtn = mDMono.transform.Find("SendMailButton").GetComponent<UIButton>();
            CloseBtn = mDMono.transform.Find("BackButton").GetComponent<UIButton>();
            BackBgBtn = mDMono.transform.Find("BG/BackBG").GetComponent<UIButton>();
            SendMailBtn.onClick.Add(new EventDelegate(OnClickSendMail));
            CloseBtn.onClick.Add(new EventDelegate(OnClickClose));
            BackBgBtn.onClick.Add(new EventDelegate(OnClickClose));
        }

        public UILabel NameLabel;
        public UILabel InputTitleLabel;
        public UILabel InputContentLabel;
        public UILabel LessTipsLabel;
        public UIButton SendMailBtn;
        public UIButton CloseBtn;
        public UIButton BackBgBtn;

        private string noInputTitleStr;
        private string noInputContentStr;
        private int lessMailTimes;
        private int waitFrame;

        public bool IsShow { get { return mDMono.gameObject.activeInHierarchy; } }

        public override void Start()
        {
            noInputTitleStr = InputTitleLabel.text;
            noInputContentStr = InputContentLabel.text;
        }

        public override void OnEnable()
        {
			//RegisterMonoUpdater();
            if (InputTitleLabel != null)
            {
                InputTitleLabel.text = noInputTitleStr;
                InputTitleLabel.GetComponent<UIInput>().value = string.Empty;
                InputContentLabel.text = noInputContentStr;
                InputContentLabel.GetComponent<UIInput>().value = string.Empty;
            }
        }

        public override void OnDestroy()
        {
            SendMailBtn.onClick.Clear();
            CloseBtn.onClick.Clear();
            BackBgBtn.onClick.Clear();
        }

        public void ShowUI(bool isShow)
        {
            mDMono.gameObject.SetActive(isShow);
        }

        //void Update()
        //{
        //    waitFrame--;
        //    if(waitFrame <=0)
        //    {
        //        waitFrame = 30;
        //    }
        //    else
        //    {
        //        return;
        //    }

        //    if(inputTitleLabel != null&& inputContentLabel!=null)
        //    {
        //        if(!inputTitleLabel.text.Equals(noInputTitleStr)&& !inputContentLabel.text.Equals(noInputContentStr))
        //        {
        //            if(!sendMailBtn.enabled)
        //            {
        //                LTUIUtil.SetGreyButtonEnable(sendMailBtn, true);
        //            }
        //        }
        //        else if(sendMailBtn.enabled == true)
        //        {
        //            LTUIUtil.SetGreyButtonEnable(sendMailBtn, false);
        //        }
        //    }
        //}

        public void SetData(LegionData data)
        {
            if (data.userMemberData == null)
            {
                return;
            }

            lessMailTimes = AlliancesManager.Instance.Config.MaxMailTimes - data.mailTimes;
            LessTipsLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LegionMailView_2088"), LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, lessMailTimes);
            NameLabel.text = string.Format("【{0}】", data.userMemberData.memberName);
        }


        void OnClickSendMail()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (InputTitleLabel.text.Equals(noInputTitleStr) || InputContentLabel.text.Equals(noInputContentStr))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionMailView_2531"));
            }
            else if (lessMailTimes <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionMailView_2680"));
            }
            else
            {
                if (LegionEvent.SendLegionMail != null)
                {
                    LegionEvent.SendLegionMail(InputTitleLabel.text, InputContentLabel.text);
                }

                if (InputTitleLabel != null)
                {
                    InputTitleLabel.text = noInputTitleStr;
                    InputTitleLabel.GetComponent<UIInput>().value = string.Empty;
                    InputContentLabel.text = noInputContentStr;
                    InputContentLabel.GetComponent<UIInput>().value = string.Empty;
                }
                mDMono.gameObject.SetActive(false);
            }
        }

        void OnClickClose()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}
