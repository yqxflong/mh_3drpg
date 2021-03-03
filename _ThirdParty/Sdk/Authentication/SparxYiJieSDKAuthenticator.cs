#if USE_YIJIESDK

using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class YiJieSDKAuthenticator : Authenticator
    {
        private YiJieSDKManager mManager = null;

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
                return "yijie";
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
                    callback(null, mManager.GetAuthData());
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
            mManager = Hub.Instance.YiJieSDKManager;
            mManager.Init(initData, callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}

#endif
