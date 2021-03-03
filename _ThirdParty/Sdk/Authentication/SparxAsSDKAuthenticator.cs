#if USE_ASSDK && UNITY_IPHONE
using System.Collections;

namespace EB.Sparx
{
    public class AsSDKAuthenticator : Authenticator
    {
        public void Init(object initData, System.Action<string, bool> callback)
        {
            Hub.Instance.AsSDKManager.InitSDK(initData, callback);
        }

        public void Authenticate(bool silent, System.Action<string, object> callback)
        {
            if (silent)
            {
                if (Hub.Instance.AsSDKManager.IsLoggedIn())
                {
                    callback(null, new Hashtable());
                }
                else
                {
                    callback(null, null);
                }
            }
            else
            {
                Hub.Instance.AsSDKManager.Login(callback);
            }
        }

        public void Logout()
        {
            if (IsLoggedIn)
            {
                Hub.Instance.AsSDKManager.Logout();
            }
        }

        public string Name
        {
            get {
                return "as";
            }
        }

        public bool IsLoggedIn
        {
            get {
                return Hub.Instance.AsSDKManager.IsLoggedIn();
            }
        }

        public AuthenticatorQuailty Quailty
        {
            get {
                return AuthenticatorQuailty.Med;
            }
        }
    }
}
#endif
