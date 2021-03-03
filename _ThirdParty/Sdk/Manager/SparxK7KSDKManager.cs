#if USE_K7KSDK
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace EB.Sparx
{

	public class K7KResultCode
	{
		public const int RESULT_OK = 0;
		public const int RESULT_FAIL = 1;
		public const int RESULT_CANCEL =2;
		public const int PAY_NO_RESULT = 3;
	}

	public class K7KSDKCallbackType
	{
		public const string CALLBACKTYPE_LOGIN = "Login";

		public const string CALLBACKTYPE_PAY = "Pay";

	}

	public class K7KSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mLogined = false;
		private System.Action<string, object> mLoginCallback;
		private System.Action<int> payResultCallBack;
        private SparxK7KSDKManager mSparxK7KSDKManager = null;
		public bool IsLoggedIn { get { return mLogined; } }
		public override void Initialize(Config config)
		{
		}

		public void InitSDK(object initData, Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.Log("K7KSDKManager.InitSDK: Initialized");
				callback(null, true);
				return;
			}
			Hub.RunInBackground = true;
			mInitialized = true;
			GameObject listner=new GameObject("k7k_plugin_listener", typeof(SparxK7KSDKManager));
            mSparxK7KSDKManager = listner.GetComponent<SparxK7KSDKManager>();
			callback(null, true);
		}

		public void Login(Action<string, object> callback)
		{
			if (!mInitialized)
			{
				callback("K7KSDKManager has not been inited", null);
				return;
			}
            EB.Debug.Log("K7KSDKManager.Login");
			mLoginCallback += callback;
			Hub.RunInBackground = true;
			mSparxK7KSDKManager.LoginSDK();
		}

		public void OnLoginCallback(int code, object data)
		{
			EB.Debug.Log("K7KSDKManager.OnLoginResult:code={0} data = {1}", code, data);
			if(code != K7KResultCode.RESULT_OK)
			{
				if (mLoginCallback != null)
				{
					mLoginCallback(null, null);
					mLoginCallback = null;
				}
				return;
			}

			mLogined = true;
			if (mLoginCallback != null)
			{
				Action<string, object> callback = mLoginCallback;
				mLoginCallback = null;
				callback(null, data);
			}
			else
			{
				EB.Debug.Log("mLoginCallback is null");
			}
		}

		public void Pay(EB.IAP.Item item, string transactionId, Action<int> callback)
		{
			payResultCallBack += callback;
			User curUser = Hub.LoginManager.LocalUser;
			if (curUser == null)
			{
				return;
			}
			mSparxK7KSDKManager.Pay(curUser.WorldId.ToString(), curUser.Id.Value.ToString(), item.cost.ToString(), item.productId, item.longName, transactionId, transactionId);
			EB.Coroutines.SetTimeout(delegate ()
			{
				OnPayCallback(K7KResultCode.PAY_NO_RESULT);
			}, 2 * 1000);		
		}

		public void OnPayCallback(int resultCode)
		{
			EB.Debug.Log("K7KSDKManager.OnPay resultCode = {0}", resultCode);

			if (payResultCallBack != null)
			{
				payResultCallBack(resultCode);
				payResultCallBack = null;
			}
		}

		public void Logout()
		{

		}
	}

	public class SparxK7KSDKManager : MonoBehaviour {

		void Awake()
		{
			EB.Debug.Log("Creating K7K Manager");
			DontDestroyOnLoad(gameObject);
		}

		public void InitSDK()
		{
        }

		[DllImport("__Internal")]
		private static extern void K7KLogin();
		[DllImport("__Internal")]
		private static extern void K7KShowCenterVC();
		[DllImport("__Internal")]
		private static extern void K7KPay(string serverid,string roleid, string amt,string goodsid,string paygold,string cpoder, string customParameter);
		[DllImport("__Internal")]
		private static extern void K7KCensus(string mark, string roleid,string rolename,string rolelevel,string zoneid,string zonename);

		public void LoginSDK()
		{
			K7KLogin();
        }

		public void Pay(string serverid, string roleid, string amt, string goodsid, string paygold, string cpoder, string customParameter)
		{
			K7KPay(serverid, roleid, amt, goodsid, paygold, cpoder, customParameter);
		}

		public void Census(string mark, string roleid, string rolename, string rolelevel, string zoneid, string zonename)
		{
			K7KCensus(mark, roleid, rolename, rolelevel, zoneid, zonename);
		}

		public void ShowCenterVC()
		{
			K7KShowCenterVC();
        }

		public void OnK7KGameSDKCallback(string jsonString)
		{
			EB.Debug.Log("SparxK7KSDKManager.OnK7KGameSDKCallback:jsonString = {0}", jsonString);
			object json = EB.JSON.Parse(jsonString);
			string callbackType = Dot.String("callbackType", json, string.Empty);
			int code = Dot.Integer("code", json, -1);
			switch (callbackType)
			{
				case K7KSDKCallbackType.CALLBACKTYPE_LOGIN:
					Sparx.Hub.Instance.K7KSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
					break;

				case K7KSDKCallbackType.CALLBACKTYPE_PAY:
					Sparx.Hub.Instance.K7KSDKManager.OnPayCallback(code);
					break;
			}
		}
	}
}
#endif
