#if USE_UCSDK && UNITY_ANDROID
using System.Collections;

namespace EB.Sparx
{
	public class UCSDKAuthenticator : Authenticator
	{
		private UCSDKManager mManager = null;

		public bool IsLoggedIn
		{
			get { return mManager.IsLoggedIn; }
		}

		public string Name
		{
			get { return "uc"; }
		}

		public AuthenticatorQuailty Quailty
		{
			get { return AuthenticatorQuailty.Med; }
		}

		public void Authenticate(bool silent,System.Action<string, object> callback)
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

		public void Init(object initData, System.Action<string, bool> callback)
		{
			mManager = Hub.Instance.UCSDKManager;
			mManager.InitializeSDK(initData, callback);
		}

		public void Logout()
		{
			mManager.Logout();
		}
	}
}
#endif
