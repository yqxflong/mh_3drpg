#if USE_EWANSDK && UNITY_ANDROID
using System.Collections;
using UnityEngine;

namespace EB.Sparx
{
	// <summary>
	// 调用SDK和回调消息响应可能需要用到的常量
	// </summary>
	public class EWanConstants
	{
		//回调消息类型
		//初始化结果回调
		public const string CALLBACKTYPE_InitSDK = "InitSDK";

		//登录结果回调
		public const string CALLBACKTYPE_Login = "Login";

		//退出登录回调
		public const string CALLBACKTYPE_Logout = "Logout";

		//充值下单结果回调
		public const string CALLBACKTYPE_Pay = "Pay";

		//退出 SDK 回调
		public const string CALLBACKTYPE_ExitSDK = "ExitSDK";

		//提交用户数据
		public const string Collect_Data = "CollectData";
	}

	// <summary>
	// 定义 SDK 方法调用的返回状态码
	// </summary>
	public class EWanStatusCode
	{
		public const int SUCCESS = 0;

		public const int FAIL = 1;

		public const int CANCEL = 2;

		public const int SwithAccout = 10;

		public const int InternalSDKSwithAccout = 11;

		public const int GamePopExitDialog = 101;

		public const int GameExit = 102;

		public const int CALLBACK_TIMEOUT = -1000;
	}

	public class EWanGameSdk
	{
		private const string SDK_JAVA_CLASS = "cn.ewan.gamesdk.unity3d.EWanGameSDK";

		public static void initSDK()
		{
			EB.Debug.Log("call initSDK....");
			callSdkApi("InitSDK");
		}

		public static void login()
		{
			callSdkApi("Login");
		}

		public static void collectData(string serverid, string servername, string roleid, string rolename, int rolelevel, string extend)
		{
			callSdkApi("CollectData", serverid, servername, roleid, rolename, rolelevel,extend);
		}

		public static void pay(float price, string serverId, string productName, int productNumber, string customInfo)
		{
			callSdkApi("Pay", price, serverId, productName, productNumber, customInfo);
		}

		// <summary>
		// 退出当前登录的账号
		// </summary>
		public static void logout()
		{
			callSdkApi("Logout");
		}

		/// <summary>
		/// 退出SDK，游戏退出前必须调用此方法，以清理SDK占用的系统资源。如果游戏退出时不调用该方法，可能会引起程序错误。
		/// </summary>
		public static void exitSDK()
		{
			callSdkApi("ExitSDK");
		}

		public static void destroySDK()
		{
			callSdkApi("DestroySDK");
		}

		public static bool isHasPlatform()
		{
			return callSdkApiReturn("IsHasPlatform");
		}

		public static bool isHasSwitchAccount()
		{			
			return callSdkApiReturn("IsHasSwitchAccount");
		}

		private static void callSdkApi(string apiName, params object[] args)
		{
			log("Unity3D " + apiName + " calling...");

			using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
			{
				cls.CallStatic(apiName, args);
			}
		}

		private static bool callSdkApiReturn(string apiName, params object[] args)
		{
			log("Unity3D " + apiName + " calling...");

			using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
			{
				return cls.CallStatic<bool>(apiName, args);
			}
		}

		private static void log(string msg)
		{
			EB.Debug.Log(msg);
		}
	}

	public class EWanSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mInitializing = false;
		private bool mLoggedIn = false;
		private static bool mInternalSDKLoggedIn = false;
		private static string mOpenId;
		private static string mToken;
		private static string mSign;
		private System.Action<string, bool> mInitCallback = null;
		private System.Action<string, object> mAuthCallback = null;
		private System.Action<int, object> mPayCallback = null;
		private System.Action<int> mExitSDKCallback = null;
		private System.Action mDestroyCallback = null;

		public bool IsLoggedIn { get { return mLoggedIn; } }

		public override void Initialize(Config config)
		{

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
						mPayCallback(EWanStatusCode.CALLBACK_TIMEOUT, null);
						mPayCallback = null;
					}
				}, 5 * 1000);
			}
		}

        public void OnLoginEnteredForeground()
        {
            if (mLoggedIn) return;
            if (mAuthCallback != null)
            {
                EB.Coroutines.SetTimeout(delegate ()
                {
                    if (mAuthCallback != null)
                    {
                        System.Action<string, object> callback = mAuthCallback;
                        mAuthCallback = null;
                        callback(null, null);
                    }
                }, 1 * 1000);
            }
        }

        public void InitializeSDK(object options, System.Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.LogWarning("EWanSDKManager.InitializeSDK: Initialized");
				callback(null, true);
				return;
			}

			if (Application.platform != RuntimePlatform.Android)
			{
				callback(null, false);
				return;
			}

			mInitCallback += callback;

			if (!mInitializing)
			{
				mInitializing = true;

				Hub.RunInBackground = true;

				new GameObject("ewan_plugin_listener", typeof(SparxEWanSDKManager));
				EWanGameSdk.initSDK();
			}
		}

		public override void OnLoggedIn()
		{
			var user = Hub.LoginManager.LocalUser;
			string serverid = user.WorldId.ToString();
			string servername = string.Empty;
			string roleid = user.Id.Value.ToString();
			string rolename = user.Name;
			int rolelevel = user.Level;
			string extend = "login role";

			//EB.Debug.LogError("unityTest OnLoggedIn collectData");
			EWanGameSdk.collectData(serverid, servername, roleid, rolename, rolelevel, extend);
		}

		public void Login(System.Action<string, object> callback)
		{
			if (mLoggedIn)
			{
				callback(null, null);
				return;
			}

			mAuthCallback += callback;

			Hub.RunInBackground = true;

			if (!mInternalSDKLoggedIn)
			{
				EWanGameSdk.login();
			}
			else
			{
				mInternalSDKLoggedIn = false;
				mLoggedIn = true;
				if (mAuthCallback != null)
				{
					Hashtable data = new Hashtable()
					{
						{"openid", mOpenId},
						{"token",mToken},
						{"sign",mSign},
					};
					mAuthCallback(null, data);
					mAuthCallback = null;
				}
			}
		}

		public void Logout()
		{
			//if (mLoggedIn)
			//{
				mLoggedIn = false;
			//}
			EWanGameSdk.logout();
		}

		//public void DestroySDK()
		//{
		//	if (mInitialized)
		//	{
		//		mInitialized = false;
		//		mLoggedIn = false;
		//		mAuthCallback = null;
		//		mInitCallback = null;
		//		mPayCallback = null;
		//		XiaoMiGameSdk.exitSDK();
		//	}
		//}

		public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int, object> callback)
		{
			if (!mInitialized)
			{
				EB.Debug.LogError("EWanSDKManager.Pay: not initialized");
				callback(EWanStatusCode.FAIL, null);
				return;
			}

			mPayCallback += callback;

			var user = Hub.Instance.LoginManager.LocalUser;
			int worldId = user.WorldId;
			string diamondName = Localizer.GetString("ID_RESOURCE_NAME_HC");
			EWanGameSdk.pay(item.cost, worldId.ToString(), diamondName, item.value, transaction.transactionId);
		}

		public void ExitSDK(Action<int> callback)
		{
			if (mInitialized)
			{
				Hub.RunInBackground = true;
				mExitSDKCallback += callback;
				EWanGameSdk.exitSDK();
			}
		}

		public void DestroySDK()
		{
			EWanGameSdk.destroySDK();
		}

		public void OnInitResult(int code, string msg)
		{
			EB.Debug.Log("OnInitResult:code={0} msg={1}",code,msg);

			mInitializing = false;		
			Hub.RunInBackground = false;

			mInitialized = true;

			if (mInitCallback != null)
			{
				mInitCallback(null, mInitialized);
				mInitCallback = null;
			}
			//EB.Debug.LogError("unityTest OnInitResult collectData");
			EWanGameSdk.collectData("1", "servername", "roleid", "rolename", 1, "create role");
		}

		public void OnPayCallback(int code, object jsonOrder)
		{
			EB.Debug.Log("OnPayCallback: code={0} data={1}", code, jsonOrder);

			if (mPayCallback != null)
			{
				mPayCallback(code, jsonOrder);
				mPayCallback = null;
			}
		}

		public void OnLoginResult(int code, object msg)
		{
			EB.Debug.Log("EWanSDKManager.OnLoginResult:code={0} msg={1}",code,msg);
			Hub.RunInBackground = false;
			if (code == EWanStatusCode.SUCCESS)
			{
				mLoggedIn = true;
				if (mAuthCallback != null)
				{
					//object data = EB.JSON.Parse(msg);
					string openId = EB.Dot.String("openid", msg, "");
					string token = EB.Dot.String("token", msg, "");
					string sign = EB.Dot.String("sign", msg, "");
					Hashtable data = new Hashtable()
					{
						{"openid", openId},
						{"token",token},
						{"sign",sign},
					};
					mAuthCallback(null, data);
					mAuthCallback = null;
				}
			}
			else if (code == EWanStatusCode.SwithAccout)
			{
				mLoggedIn = false;
				SparxHub.Instance.Disconnect(true);
			}
			else if (code == EWanStatusCode.InternalSDKSwithAccout)
			{
				mLoggedIn = false;
				mInternalSDKLoggedIn = true;
				mOpenId = EB.Dot.String("openid", msg, "");
				mToken = EB.Dot.String("token", msg, "");
				mSign = EB.Dot.String("sign", msg, "");
				SparxHub.Instance.Disconnect(true);
			}
			else
			{
				mLoggedIn = false;
				if (mAuthCallback != null)
				{
					mAuthCallback(null, null);
					mAuthCallback = null;
				}
			}
		}

		public void OnLogout(int code, string msg)
		{
			EB.Debug.Log("OnLogout: code={0}, msg={1}",code,msg);

			if (mLoggedIn)
			{
				mLoggedIn = false;
				Hub.Instance.Disconnect(true);
			}
		}

		public void OnExitSDK(int code, string msg)
		{
			EB.Debug.Log("OnExitSDK: code={0}, msg={1}", code, msg);

			if (mExitSDKCallback != null)
			{
				mExitSDKCallback(code);
				mExitSDKCallback = null;
			}
		}

		public bool IsHasPlatform()
		{
			return EWanGameSdk.isHasPlatform();
		}

		public bool IsHasSwitchAccount()
		{
			return EWanGameSdk.isHasSwitchAccount();
		}
	}
}

public class SparxEWanSDKManager : MonoBehaviour
{
	void Awake()
	{
		EB.Debug.Log("Creating EWan Manager");
		DontDestroyOnLoad(gameObject);
	}

	public void OnEWanGameSdkCallback(string jsonstr)
	{
		EB.Debug.Log("OnEWanGameSdkCallback message: jsonstr = {0}", jsonstr);

		object json = EB.JSON.Parse(jsonstr);
		string callbackType = EB.Dot.String("callbackType", json, string.Empty);
		int code = EB.Dot.Integer("code", json, 0);

		switch (callbackType)
		{
			case EB.Sparx.EWanConstants.CALLBACKTYPE_InitSDK:
				SparxHub.Instance.EWanSDKManager.OnInitResult(code,EB.Dot.String("data", json, null));
				break;
			case EB.Sparx.EWanConstants.CALLBACKTYPE_Login:
				SparxHub.Instance.EWanSDKManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
				break;
			case EB.Sparx.EWanConstants.CALLBACKTYPE_Logout:
				SparxHub.Instance.EWanSDKManager.OnLogout(code, EB.Dot.String("data", json, null));
				break;
			case EB.Sparx.EWanConstants.CALLBACKTYPE_Pay:
				SparxHub.Instance.EWanSDKManager.OnPayCallback(code, EB.Dot.Object("data", json, null));
				break;
			case EB.Sparx.EWanConstants.CALLBACKTYPE_ExitSDK:
				SparxHub.Instance.EWanSDKManager.OnExitSDK(code, EB.Dot.String("data", json, null));
				break;
			default:
				EB.Debug.LogWarning("OnEWanGameSdkCallback: unknown type = {0}", callbackType);
				break;
		}
	}

    void OnApplicationPause(bool pauseStatus)
    {
        EB.Debug.Log("EWanSDKManager.OnApplicationPause: status = {0}", pauseStatus);
        if (!pauseStatus)
        {
            SparxHub.Instance.EWanSDKManager.OnLoginEnteredForeground();
        }
    }
}
#endif