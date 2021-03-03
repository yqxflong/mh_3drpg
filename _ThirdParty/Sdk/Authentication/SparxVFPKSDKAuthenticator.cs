#if USE_VFPKSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EB.Sparx
{
    public class SparxVFPKSDKAuthenticator : Authenticator
    {
        private VFPKSDKManager mManager;

        public string Name
        {
            get
            {
                return "vfpk";
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

        public void Init(object initData, Action<string, bool> callback)
        {
            mManager = Sparx.Hub.Instance.VFPKSDKManager;
            mManager.Init(callback, delegate (string tip) { Hub.Instance.TipCallBack(tip); });
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

        public void Logout()
        {
            mManager.Logout();
        }
    }
}
#endif
