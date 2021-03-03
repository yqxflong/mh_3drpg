#if UNITY_ANDROID &&USE_ALIPAYSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EB.Sparx
{
    public class SparxAlipaySDKAuthenticator : Authenticator
    {
        private AlipaySDKManager mManager;

        public string Name
        {
            get
            {
                return "alipay";
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

        public void Init(object initData, System.Action<string, bool> callback)
        {
            mManager = Sparx.Hub.Instance.AlipaySDKManager;
            mManager.Init(callback);
        }

        public void Authenticate(bool silent, System.Action<string, object> callback)
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

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif
