#if USE_ChangDaShiSDK
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace EB.Sparx
{

	public class ChangDaShiResultCode
	{
		public const int RESULT_OK = 0;
		public const int RESULT_FAIL = 1;
		public const int RESULT_CANCEL =2;
	}

	public class ChangDaShiSDKCallbackType
	{
		public const string CALLBACKTYPE_LOGIN = "Login";

		public const string CALLBACKTYPE_PAY = "Pay";

	}

	public class ChangDaShiSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mLogined = false;
		private System.Action<string, object> mLoginCallback;
		private System.Action<int> payResultCallBack;
        private SparxChangDaShiSDKManager mSparxChangDaShiSDKManager = null;
		public bool IsLoggedIn { get { return mLogined; } }
		public override void Initialize(Config config)
		{
		}

		public void InitSDK(object initData, Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.Log("ChangDaShiSDKManager.InitSDK: Initialized");
				callback(null, true);
				return;
			}
			Hub.RunInBackground = true;
			mInitialized = true;
			GameObject listner=new GameObject("changdashi_plugin_listener", typeof(SparxChangDaShiSDKManager));
            mSparxChangDaShiSDKManager = listner.GetComponent<SparxChangDaShiSDKManager>();
			callback(null, true);
		}

		public void Login(Action<string, object> callback)
		{
			if (!mInitialized)
			{
				callback("ChangDaShiSDKManager has not been inited", null);
				return;
			}
            EB.Debug.Log("ChangDaShiSDKManager.Login");
			mLoginCallback += callback;
			Hub.RunInBackground = true;
			mSparxChangDaShiSDKManager.LoginSDK();
		}

		public void OnLoginCallback(int code, object data)
		{
			EB.Debug.Log("ChangDaShiSDKManager.OnLoginResult:code={0} data = {1}", code, data);
			if(code != ChangDaShiResultCode.RESULT_OK)
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
			var worlds = Hub.LoginManager.GameWorlds;
			var world = System.Array.Find(worlds, w => w.Id == curUser.WorldId);
			string worldName = world != null ? world.Name : (worlds.Length > 0 ? worlds[0].Name : "Default");
			mSparxChangDaShiSDKManager.Pay(worldName, curUser.Id.Value.ToString(), item.cost.ToString(), item.productId, transactionId, transactionId, "");
		}

		public void OnPayCallback(int resultCode)
		{
			EB.Debug.Log("ChangDaShiSDKManager.OnPay resultCode = {0}", resultCode);

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

	public class SparxChangDaShiSDKManager : MonoBehaviour {

		void Awake()
		{
			EB.Debug.Log("Creating ChangDaShi Manager");
			DontDestroyOnLoad(gameObject);
		}

		public void InitSDK()
		{
        }

		[DllImport("__Internal")]
		private static extern void ChangDaShiLogin();
		[DllImport("__Internal")]
		private static extern void ChangDaShiShowCenterVC();
		[DllImport("__Internal")]
		private static extern void ChangDaShiPay(string serverid,string roleid, string amt,string goodsid,string paygold,string cpoder, string customParameter);
		[DllImport("__Internal")]
		private static extern void ChangDaShiCensus(string mark, string roleid,string rolename,string rolelevel,string zoneid,string zonename);

		public void LoginSDK()
		{
			ChangDaShiLogin();
        }

		public void Pay(string serverid, string roleid, string amt, string goodsid, string paygold, string cpoder, string customParameter)
		{
			ChangDaShiPay(serverid, roleid, amt, goodsid, paygold, cpoder, customParameter);
		}

		public void Census(string mark, string roleid, string rolename, string rolelevel, string zoneid, string zonename)
		{
			ChangDaShiCensus(mark, roleid, rolename, rolelevel, zoneid, zonename);
		}

		public void ShowCenterVC()
		{
			ChangDaShiShowCenterVC();
        }

		public void OnChangDaShiGameSDKCallback(string jsonString)
		{
			EB.Debug.Log("SparxChangDaShiSDKManager.OnChangDaShiGameSDKCallback:jsonString = {0}", jsonString);
			object json = EB.JSON.Parse(jsonString);
			string callbackType = Dot.String("callbackType", json, string.Empty);
			int code = Dot.Integer("code", json, -1);
			switch (callbackType)
			{
				case ChangDaShiSDKCallbackType.CALLBACKTYPE_LOGIN:
					Sparx.Hub.Instance.ChangDaShiSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
					break;

				case ChangDaShiSDKCallbackType.CALLBACKTYPE_PAY:
					Sparx.Hub.Instance.ChangDaShiSDKManager.OnPayCallback(code);
					break;
			}
		}
	}
}
#endif
