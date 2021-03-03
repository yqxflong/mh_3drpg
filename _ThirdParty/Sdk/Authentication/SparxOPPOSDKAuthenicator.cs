#if UNITY_ANDROID && USE_OPPOSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class OPPOSDKAuthenicator : Authenticator
    {
        private OPPOSDKManager mManager;
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
                return "oppo";
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
                    Hashtable data = new Hashtable();
                    data.Add("token", mManager.Token);
                    data.Add("ssoid", mManager.Ssoid);
                    callback(null, data);
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
            mManager = Sparx.Hub.Instance.OPPOSDKManager;
            mManager.InitSDK(initData, callback);
        }

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif