#if UNITY_ANDROID && USE_M4399SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public class M4399SDKAuthenticator : Authenticator
    {
        private M4399SDKManager mManager;

        public string Name
        {
            get
            {
                return "m4399";
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return mManager.IsLoggedIn;
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
                mManager.Login(callback);
            }
        }

        public void Init(object initData, Action<string, bool> callback)
        {
            mManager = Sparx.Hub.Instance.M4399SDKManager;
            mManager.InitSDK(callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif