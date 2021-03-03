#if UNITY_IPHONE && USE_WINNERIOSSDK
using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class WinnerIOSSDKAuthenticator : Authenticator
	{

		private WinnerIOSSDKManager mManager;

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
				return "winner";
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
				callback(null, null);
			}
			else
			{
				mManager.Login(callback);
			}
		}

		public void Init(object initData, Action<string, bool> callback)
		{
			mManager = Hub.Instance.WinnerIOSSDKManager; 
            mManager.InitSDK(initData, callback);
		}

		public void Logout()
		{
			return;
		}
	}
}
#endif
