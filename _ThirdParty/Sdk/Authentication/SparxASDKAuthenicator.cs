#if USE_ASDK && UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public class ASDKAuthenicator : Authenticator
    {
        private ASDKManager  mManager = null;

        public bool IsLoggedIn
        {
            get { return mManager.IsLoggedIn; }
        }

        public string Name
        {
            get { return "asdk"; }
        }

        public AuthenticatorQuailty Quailty
        {
            get { return AuthenticatorQuailty.Med; }
        }

        public void Authenticate(bool silent, Action<string, object> callback)
        {
            if (silent)
            {
                if (IsLoggedIn)
                {
                    callback(null, new Hashtable() { { "sid", mManager.Sid } });
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
            mManager = Hub.Instance.ASDKManager;
            mManager.InitializeSDK(initData, callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif