#if USE_AIBEISDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class AibeiSDKAuthenticator : Authenticator
    {
        private AibeiSDKManager mManager = null;

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
                return "aibei";
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
                if (IsLoggedIn)
                {
                    callback(null, new Hashtable());
                    return;
                }
                else
                {
                    callback(null, null);
                    return;
                }
            }
            else
            {
                mManager.Login(callback);
            }
        }

        public void Init(object initData, Action<string, bool> callback)
        {
            mManager = Hub.Instance.AibeiSDKManager;
            mManager.InitSDK(initData, callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif