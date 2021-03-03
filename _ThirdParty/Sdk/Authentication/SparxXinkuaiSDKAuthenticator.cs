#if USE_XINKUAISDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EB.Sparx
{
    public class SparxXinkuaiSDKAuthenticator : Authenticator
    {
        private XinkuaiSDKManager mManager;

        public string Name
        {
            get
            {
#if USE_XINKUAIFX//标识字段
                return "xinkuaifx";
#else
                return "xinkuai";
#endif
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return mManager.IsLoggedIn();
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
            mManager = (XinkuaiSDKManager)Sparx.Hub.Instance.mBaseSdkManager;
            mManager.Init(initData, callback, delegate (string tip) { Hub.Instance.TipCallBack(tip); });
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