#if UNITY_ANDROID &&USE_WECHATSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EB.Sparx
{
    public class SparxWeChatSDKAuthenticator : Authenticator
    {
        private WeChatSDKManager mManager;

        public string Name
        {
            get
            {
                return "wechat";
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
            mManager = Sparx.Hub.Instance.WeChatSDKManager;
            mManager.Init(callback, delegate (string tip) { Hub.Instance.TipCallBack(tip);});
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