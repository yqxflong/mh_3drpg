using EB.Sparx;
using System;
using UnityEngine;



namespace Hotfix_LT.UI
{
    /// <summary>
    /// 多语言设置
    /// </summary>
    public class LTLanguageSetting
    {
        /// <summary>
        /// 点击切换语言回调
        /// </summary>
        private Action m_OnClickSwitchlanguage;
        /// <summary>
        /// 切换的语言
        /// </summary>
        private EB.Language m_Language;

        public LTLanguageSetting(Transform transform, Action onClickSwitchLanguage)
        {
            m_OnClickSwitchlanguage = onClickSwitchLanguage;
            transform.GetComponent<UISprite>(string.Format("Grid/Language_{0}/OtherSetting", EB.Symbols.LanguageList[global::UserData.Locale])).spriteName = "Ty_Mail_Di3";
            UIEventListener.Get(transform.Find("Grid/Language_CN/OtherSetting").gameObject).onClick = OnClickCN;
            UIEventListener.Get(transform.Find("Grid/Language_EN/OtherSetting").gameObject).onClick = OnClickEN;
        }

        private void OnClickCN(GameObject go)
        {
            if (global::UserData.Locale == EB.Language.ChineseSimplified)
            {
                return;
            }
            m_Language = EB.Language.ChineseSimplified;
            SetMessageDialog();
        }

        private void OnClickEN(GameObject go)
        {
            if (global::UserData.Locale == EB.Language.English)
            {
                return;
            }
            m_Language = EB.Language.English;
            SetMessageDialog();
        }

        /// <summary>
        /// 一定要注意和能出界面退出逻辑LTGameSettingController.OnExitBtnClick()要一致:
        /// </summary>
        private void SetMessageDialog()
        {
            string content = EB.Localizer.GetString("ID_LANGUAGE_SWITCHTIPS_3");
            MessageDialog.Show(EB.Localizer.GetString("ID_LOGOUT"), content, EB.Localizer.GetString(MessageTemplate.OkBtn), EB.Localizer.GetString(MessageTemplate.CancelBtn), false, false, false, delegate (int result)
            {
                if (result == 0)
                {
                    global::UserData.Locale = m_Language;
                    Hub.Instance.Config.Locale = global::UserData.Locale;
                //切换语言
                EB.Localizer.LoadCurrentLanguageBase(global::UserData.Locale);

                    if (m_OnClickSwitchlanguage != null)
                    {
                        m_OnClickSwitchlanguage();
                    }
#if USE_FB
                //by:wwh 因为Facebook没有这个接口来切换账号 20190823
                if (SparxHub.Instance.FacebookManager.IsLoggedIn)
                {
                   EB.Debug.Log("------------调用退出Facebook接口------------");
                    SparxHub.Instance.FacebookManager.Logout();
                }
#endif

#if USE_EWANSDK
				SparxHub.Instance.EWanSDKManager.Logout();
#else

                //Reset();
                if (FriendManager.Instance != null && FriendManager.Instance.AcHistory != null)
                    {
                        FriendManager.Instance.AcHistory.ClearAllChatList();
                    }
                    SparxHub.Instance.Disconnect(true);
#endif
            }
            }, NGUIText.Alignment.Center);
        }
    }
}
