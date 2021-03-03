#if UNITY_ANDROID && USE_HUAWEISDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class HuaweiSDKAuthenticator : Authenticator
    {
        private HuaweiSDKManager mManager;

        public bool IsLoggedIn
        {
            get
            {
                return mManager.IsLoggedIn;
            }
        }

        public string Name
        {
            get
            {
                return "huawei";
            }
        }

        public AuthenticatorQuailty Quailty
        {
            get
            {
                return AuthenticatorQuailty.Med;
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
                mManager.Login(callback);
            }
        }

        public void Init(object initData, Action<string, bool> callback)
        {
            mManager = Hub.Instance.HuaweiSDKManager;
            mManager.InitSDK(initData, callback);
        }

        public void Logout()
        {
            return;
        }
    }
}
#endif