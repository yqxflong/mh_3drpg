using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class AuthenticatorEntryController : DynamicMonoHotfix
    {
        public UIButton Btn;
        public UILabel Label;
        public UISprite Icon;

        public System.Action<string> BtnAction;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            Btn = t.GetComponent<UIButton>("Btn");
            Btn.onClick.Add(new EventDelegate ( OnBtnClick));
            Label = t.GetComponent<UILabel>("Btn/Label");
            Icon = t.GetComponent<UISprite>("Btn/Icon");
        }

        public string entryName;
        public void Init(AuthenticatorEntry data, System.Action<string> action)
        {
            entryName = mDMono .name = data.entryName;
            Label.text = data.labelName;
            if (string.IsNullOrEmpty(data.iconName))
            {
                Icon.gameObject.CustomSetActive(false);
                Label.transform.localPosition = new Vector3(0, 4, 0);
            }
            else
            {
                Icon.gameObject.CustomSetActive(true);
                Icon.spriteName = data.iconName;
                Label.transform.localPosition = new Vector3(50, 4, 0);
            }
            Btn.GetComponent<UISprite>().spriteName = (data.btnType) ? "Login_Button_1" : "Login_Button_2";
            BtnAction = action;
        }

        private void OnBtnClick()
        {
            if (BtnAction != null)
            {
                BtnAction(entryName);
            }
        }
    }

    public class AuthenticatorEntry
    {
        public string entryName;
        public string labelName;

        public string iconName;
        public bool btnType;

        public AuthenticatorEntry(string entryName, string labelName, string iconName = "", bool btnType = false)
        {
            this.entryName = entryName;
            this.iconName = iconName;
            this.labelName = labelName;
            this.btnType = btnType;
        }
    }
}
