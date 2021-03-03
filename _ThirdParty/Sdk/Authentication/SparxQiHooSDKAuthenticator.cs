#if UNITY_ANDROID && USE_QIHOOSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class QiHooSDKAuthenticator : Authenticator
    {
        private QiHooSDKManager mManager;

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
                return "qihoo";
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
            mManager = Hub.Instance.QiHooSDKManager;
            mManager.InitSDK(initData, callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif

