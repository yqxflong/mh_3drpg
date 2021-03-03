#if USE_KUAIYONGSDK && UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class KuaiYongSDKAuthenticator : Authenticator
    {
        public void Init(object initData, Action<string, bool> callback)
        {
            EB.Debug.Log("KuaiYongSDKAuthenticator.Init");
            Hub.Instance.KuaiYongSDKManager.InitSDK(initData, callback);
        }

        public void Authenticate(bool silent, Action<string, object> callback)
        {
            EB.Debug.Log("KuaiYongSDKAuthenticator.Authenticate");
            if (silent)
            {
                if (Hub.Instance.KuaiYongSDKManager.IsLoggedIn())
                {
                    callback(null, new Hashtable());
                }
                else
                {
                    callback(null, null);
                }
            }
            else
            {
                Hub.Instance.KuaiYongSDKManager.Login(callback);
            }
        }

        public void Logout()
        {
            if (IsLoggedIn)
            {
                Hub.Instance.KuaiYongSDKManager.Logout();
            }
        }

        public string Name
        {
            get
            {
                return "kuaiyong";
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return Hub.Instance.KuaiYongSDKManager.IsLoggedIn();
            }
        }

        public AuthenticatorQuailty Quailty
        {
            get
            {
                return AuthenticatorQuailty.Med;
            }
        }
    }
}
#endif