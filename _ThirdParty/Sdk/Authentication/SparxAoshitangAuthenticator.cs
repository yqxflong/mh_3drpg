#if USE_AOSHITANGSDK
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public class SparxAoshitangAuthenticator : Authenticator
    {
        private AoshitangSDKManager m_Manager;
        public string Name
        {
            get { return "aoshitang"; }
        }

        public bool IsLoggedIn
        {
            get
            {
                return m_Manager.IsLoggedIn();
            }
        }

        public AuthenticatorQuailty Quailty 
        {
            get
            {
                return AuthenticatorQuailty.High;
            }
        }

        public void Authenticate(bool silent, Action<string, object> callback)
        {
            if (silent)
            {
                callback(null, null);
            }
            else
            {
                m_Manager.Login(callback);
            }
        }

        public void Init(object initData, Action<string, bool> callback)
        {
            m_Manager = Sparx.Hub.Instance.mBaseSdkManager as AoshitangSDKManager;
            m_Manager?.Init(initData, callback, delegate (string tip) { Hub.Instance.TipCallBack(tip); });
        }

        public void Logout()
        {
            m_Manager.Logout();
        }
    }
}
#endif