#if UNITY_ANDROID && USE_LBSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
	public class LBStatusCode
	{
		public const int SUCCESS = 1;
		public const int FAIL = 0;
		public const int Cancel = -1;
		public const int CALLBACK_TIMEOUT = -1000;
	}

	public class LBSDKCallbackType
	{
		public const string CALLBACKTYPE_INITSDK = "InitSDK";
		public const string CALLBACKTYPE_LOGIN = "Login";
		public const string CALLBACKTYPE_LOGOUT = "Logout";
		public const string CALLBACKTYPE_PAY = "Pay";
		public const string CALLBACKTYPE_SUBMIT_USER_DATA = "SubmitUserData";
		public const string CALLBACKTYPE_EXITSDK = "ExitSDK";
	}

	public class LBGameSDK
	{
		private const string SDK_JAVA_CLASS = "cn.lb.gamesdk.unity3d.LBGameSDK";

		public static void InitSDK()
		{
			callSdkApi("InitSDK");
		}

		public static void Login()
		{
			callSdkApi("Login");
		}

		public static void Logout()
		{
			callSdkApi("Logout");
		}

		public static void Pay(String roleId, String roleName, String roleLevel, String serverId, String serverName, String productId, String productName, String productDesc, String price, String transactionId)
		{
			callSdkApi("Pay", roleId, roleName, roleLevel, serverId, serverName, productId, productName, productDesc, price, transactionId);
		}

		public static void SubmitUserData(String roleId, String roleName, String roleLevel, String serverId, String serverName, int moneyNum, String uid)
		{
			callSdkApi("SubmitUserData", roleId, roleName, roleLevel, serverId, serverName, moneyNum, uid);
		}

		public static void ExitSDK()
		{
			callSdkApi("ExitSDK");
		}

		private static void callSdkApi(string apiName, params object[] args)
		{
			EB.Debug.Log("Unity3D " + apiName + " calling...");

			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
			{
				androidJavaClass.CallStatic(apiName, args);
			}
		}
	}

	public class LBSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mLogined = false;

		private System.Action<string, bool> mInitCallback;
		private System.Action<string, object> mLoginCallback;
		private System.Action<int,object> mPayCallback;
		private System.Action<bool> mExitSDKCallback;
	
		public bool IsLoggedIn { get { return mLogined; } }

		public override void Initialize(Config config)
		{

		}

		public override void OnLoggedIn()
		{
			
			var user = Hub.LoginManager.LocalUser;
			string roleId = user.Id.Value.ToString();
			string roleName = user.Name;
			string roleLevel = user.Level.ToString();
			int money = user.Revenue;
			Debug.LogError("Unity money="+money);
			SubmitUserData(roleId, roleName,roleLevel, "serviceId", "serverName", money, roleId);
		}

		public override void OnEnteredForeground()
		{

			if (mInitCallback != null)
			{
				EB.Coroutines.SetTimeout(delegate ()
				{
					if (mInitCallback != null)
					{
						mInitCallback(null, false);
						mInitCallback = null;
					}
				}, 5 * 1000);
			}

			if (mPayCallback != null)
			{
				EB.Coroutines.SetTimeout(delegate ()
				{
					if (mPayCallback != null)
					{
						mPayCallback(LBStatusCode.CALLBACK_TIMEOUT, null);
						mPayCallback = null;
					}
				}, 5 * 1000);
			}

			if (mExitSDKCallback != null)
			{
				EB.Coroutines.SetTimeout(delegate ()
				{
					if (mExitSDKCallback != null)
					{
						mExitSDKCallback(false);
						mExitSDKCallback = null;
					}
				}, 5 * 1000);
			}
		}

        public void OnLoginEnteredForeground()
        {
            if (mLogined) return;
            if (mLoginCallback != null)
            {
                EB.Coroutines.SetTimeout(delegate ()
                {
                    if (mLoginCallback != null)
                    {
                        System.Action<string, object> callback = mLoginCallback;
                        mLoginCallback = null;
                        callback(null, null);
                    }
                }, 1 * 1000);
            }
        }

        public override void OnEnteredBackground()
		{
			base.OnEnteredBackground();
		}

		//public override void Destroy(Action callback)
		//{
		//	if (!mInitialized)
		//	{
		//		callback();
		//		return;
		//	}
		//	ExitSDK(callback);
		//}

		public void InitSDK(object options, Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.LogWarning("LBSDKManager.InitializeSDK: Initialized");
				callback(null, true);
				return;
			}
			if (Application.platform != RuntimePlatform.Android)
			{
				callback(null, false);
				return;
			}
			mInitCallback += callback;
			Hub.RunInBackground = true;
			new GameObject("lb_plugin_listener", typeof(SparxLBSDKManager));
			LBGameSDK.InitSDK();
		}

		public void OnInitCallback(int code)
		{
			Hub.RunInBackground = false;
			if (code == LBStatusCode.SUCCESS)
			{
				mInitialized = true;
			}

			if (mInitCallback != null)
			{
				mInitCallback(null, mInitialized);
				mInitCallback = null;
			}
		}

		public void Login(Action<string, object> callback)
		{
			EB.Debug.Log("LBSDKManager.Login");
			if (!mInitialized)
			{
				callback("LBSDK has not been inited", null);
				return;
			}

			mLoginCallback += callback;

			Hub.RunInBackground = true;
			LBGameSDK.Login();
		}

		public void OnLoginCallback(int code,object data)
		{
			EB.Debug.Log("LBSDKManager.OnLoginResult:code={0}", code);
			Hub.RunInBackground = false;
			if (code == LBStatusCode.SUCCESS)
			{
				if (mLoginCallback != null)
				{
					mLogined = true;
					string username = EB.Dot.String("username", data, string.Empty);
					string logintime = EB.Dot.String("logintime", data, string.Empty);
					Hashtable hs = new Hashtable()
					{
						{"username", username },
						{"logintime",logintime},
					};
					mLoginCallback(null, hs);
					mLoginCallback = null;
				}
			}
			else
			{
				if (mLoginCallback != null)
				{
					mLoginCallback(null, null);
					mLoginCallback = null;
				}
			}
		}

		public void OnLogOutCallback(int code)
		{
			EB.Debug.Log("LBSDKManager.OnLoginOut:code={0}", code);
			Hub.RunInBackground = false;
			if (code == LBStatusCode.SUCCESS)
			{
				Hub.Disconnect(true);
			}
		}

		public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int,object> callback)
		{
			EB.Debug.Log("LBSDKManager.Pay");
			if (!mInitialized)
			{
				EB.Debug.LogError("LBSDKManager.Pay: not initialized");
				callback(LBStatusCode.FAIL,null);
				return;
			}

			mPayCallback += callback;
			var user = Hub.Instance.LoginManager.LocalUser;
			string roleId = user.Id.Value.ToString();
			string roleName = user.Name;
			int level = user.Level;
			LBGameSDK.Pay(roleId, roleName, level.ToString(), "serverId", "serverName",item.productId, item.longName, item.localizedDesc, ((int)item.cost).ToString(), transaction.transactionId);						
		}

		public void OnPayCallback(int code,object data)
		{
			EB.Debug.Log("LBSDKManager.OnPayCallback: code = {0}", code);
			if (mPayCallback != null)
			{
				mPayCallback(code, data);
				mPayCallback = null;
			}
		}

		public void SubmitUserData(String roleId, String roleName, String roleLevel, String serverId, String serverName, int moneyNum, String uid)
		{
			LBGameSDK.SubmitUserData(roleId, roleName, roleLevel, serverId, serverName, moneyNum, uid);
		}

		public void ExitSDK(System.Action<bool> callback)
		{
			if (mInitialized)
			{
				mExitSDKCallback += callback;
				LBGameSDK.ExitSDK();
			}
		}

		public void OnExitSDK(int code)
		{
			EB.Debug.Log("OnExitSDK: code={0}", code);
			Hub.RunInBackground = false;
			if (mExitSDKCallback != null)
			{
				mExitSDKCallback(code==LBStatusCode.SUCCESS);
				mExitSDKCallback = null;
			}
		}
	}

	public class SparxLBSDKManager : MonoBehaviour
	{
		void Awake()
		{
			EB.Debug.Log("Creating LB Manager");
			DontDestroyOnLoad(gameObject);
		}

		public void OnLBGameSDKCallback(string jsonString)
		{
			EB.Debug.Log("SparxLBSDKManager.OnLBGameSDKCallback:jsonString = {0}", jsonString);
			object json = EB.JSON.Parse(jsonString);
			string callbackType = Dot.String("callbackType", json, string.Empty);
			int code = Dot.Integer("code", json, -1);
			switch (callbackType)
			{
				case LBSDKCallbackType.CALLBACKTYPE_INITSDK:
					Sparx.Hub.Instance.LBSDKManager.OnInitCallback(code);
					break;
				case LBSDKCallbackType.CALLBACKTYPE_LOGIN:
					Sparx.Hub.Instance.LBSDKManager.OnLoginCallback(code, EB.Dot.Object("data", json, null));
					break;
				case LBSDKCallbackType.CALLBACKTYPE_LOGOUT:
					Sparx.Hub.Instance.LBSDKManager.OnLogOutCallback(code);
					break;
				case LBSDKCallbackType.CALLBACKTYPE_PAY:
					Sparx.Hub.Instance.LBSDKManager.OnPayCallback(code, EB.Dot.Object("data", json, null));
					break;
				case LBSDKCallbackType.CALLBACKTYPE_EXITSDK:
					Sparx.Hub.Instance.LBSDKManager.OnExitSDK(code);
					break;
			}
		}

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxLBSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus)
            {
                SparxHub.Instance.LBSDKManager.OnLoginEnteredForeground();
            }
        }
    }
}
#endif
