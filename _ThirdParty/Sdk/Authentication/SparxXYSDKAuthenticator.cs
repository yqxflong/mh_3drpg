#if USE_XYSDK && UNITY_IPHONE
using System.Collections;

namespace EB.Sparx
{
    public class XYSDKAuthenticator : Authenticator
    {
        public void Init(object initData, System.Action<string, bool> callback)
        {
            Hub.Instance.XYSDKManager.InitSDK(initData, callback);
        }

        public void Authenticate(bool silent, System.Action<string, object> callback)
        {
            if (silent)
            {
                if (Hub.Instance.XYSDKManager.IsLoggedIn())
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
                Hub.Instance.XYSDKManager.Login(callback);
            }
        }

        public void Logout()
        {
            if (IsLoggedIn)
            {
                Hub.Instance.XYSDKManager.Logout();
            }
        }

        public string Name
        {
            get { return "xy"; }
        }

        public bool IsLoggedIn
        {
            get { return Hub.Instance.XYSDKManager.IsLoggedIn(); }
        }

        public AuthenticatorQuailty Quailty
        {
            get { return AuthenticatorQuailty.Med; }
        }
    }
}
#endif