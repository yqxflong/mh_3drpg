#if UNITY_ANDROID && USE_TENCENTSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class QQAuthenticator : Authenticator
    {
        private TencentSDKManager mManager;
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
                return "tencent_qq";
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
                if (mManager.GetPlatform() == TencentSDKManager.QQPlatform)
                {
                    callback(null, new Hashtable());
                }
                else
                {
                    callback(null, null);
                }
                return;
            }
            else
            {
                mManager.Login(TencentSDKManager.QQPlatform, callback);
            }
        }

        public void Init(object initData, Action<string, bool> callback)
        {
            mManager = Hub.Instance.TencentSDKManager;
            mManager.InitSDK(initData, callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif
