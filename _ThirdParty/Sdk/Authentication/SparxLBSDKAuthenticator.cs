#if UNITY_ANDROID && USE_LBSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class LBSDKAuthenticator : Authenticator
    {
        private LBSDKManager mManager;
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
                return "lb";
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
                    callback(null, null);
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
            mManager = Sparx.Hub.Instance.LBSDKManager;
            mManager.InitSDK(initData, callback);
        }

        public void Logout()
        {
            return;
        }
    }
}
#endif