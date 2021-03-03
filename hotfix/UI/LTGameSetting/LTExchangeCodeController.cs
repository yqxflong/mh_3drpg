using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTExchangeCodeController : UIControllerHotfix
    {
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        public override void Awake()
        {
            base.Awake();

            codeInput = controller.transform.Find("Control/Table/InputBtn").GetComponent<UIInput>();
            UIButton backButton = controller.transform.Find("Control/CloseButton").GetComponent<UIButton>();
            controller.backButton = backButton;

            HotfixBtn1 = controller.transform.Find("Control/Table/ButtonGrid/OKButton").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnSureBtnClick));
            HotfixBtn2 = controller.transform.Find("Control/Table/ButtonGrid/CancelButton").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(OnCacncelBtnClick));
        }
        public UIInput codeInput;

        public override bool ShowUIBlocker { get { return true; } }
        
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        private void OnCacncelBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            controller.Close();
        }

        public void OnSureBtnClick()
        {
            if (string.IsNullOrEmpty(codeInput.value))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_EMPTY")); //MenuManager.Warning("ID_INPUT_EMPTY");
                return;
            }

            if (codeInput.value.IndexOf(" ") >= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_CONTAINS_SPACE")); //MenuManager.Warning("ID_INPUT_CONTAINS_SPACE");
                return;
            }

            if (codeInput.value.IndexOf("\n") >= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_CONTAINS_NEWLINE")); //MenuManager.Warning("ID_INPUT_CONTAINS_NEWLINE");
                return;
            }
            LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    if (response.fatal)
                    {
                        SparxHub.Instance.FatalError(response.localizedError);
                        return true;
                    }
                    else
                    {
                        int messageId = 0;
                        int.TryParse(response.error.ToString(), out messageId);
                        if (messageId > 0)
                        {
                            MessageTemplateManager.ShowMessage(messageId);
                            controller.Close();
                        }
                        return true;
                    }

                }
                return false;
            };
            LTGameSettingManager.Instance.RequestRedeemCode(codeInput.value,delegate(bool success)
            {
                if (success)
                {
                    MessageTemplateManager.ShowMessage(902117);
                    controller.Close();
                }
            });
        }
    }

}
