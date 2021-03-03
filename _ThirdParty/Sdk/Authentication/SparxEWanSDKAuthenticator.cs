#if UNITY_ANDROID && USE_EWANSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class EWanSDKAuthenticator : Authenticator
    {
        private EWanSDKManager mManager;
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
                return "ewan";
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
            mManager = Sparx.Hub.Instance.EWanSDKManager;
            mManager.InitializeSDK(initData, callback);
        }

        public void Logout()
        {
			//mManager = Sparx.Hub.Instance.EWanSDKManager;
			//mManager.Logout();
			return;
        }
    }
}
#endif