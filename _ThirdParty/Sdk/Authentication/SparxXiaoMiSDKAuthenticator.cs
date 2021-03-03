#if USE_XIAOMISDK && UNITY_ANDROID
using System.Collections;

namespace EB.Sparx
{
	public class XiaoMiSDKAuthenticator : Authenticator
	{
		private XiaoMiSDKManager mManager = null;

		public bool IsLoggedIn
		{
			get { return mManager.IsLoggedIn; }
		}

		public string Name
		{
			get { return "xiaomi"; }
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
					callback(null, null);
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
			mManager = Hub.Instance.XiaoMiSDKManager;
			mManager.InitializeSDK(initData, callback);
		}

		public void Logout()
		{
			//mManager.Logout();
		}
	}
}
#endif
