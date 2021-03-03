#if USE_XIAOMISDK && UNITY_ANDROID
using System.Collections;
using UnityEngine;

namespace EB.Sparx
{
	// <summary>
	// 调用SDK和回调消息响应可能需要用到的常量
	// </summary>
	public class XiaoMiConstants
	{
		//错误信息级别，记录错误日志
		public const int LOGLEVEL_ERROR = 0;
		//警告信息级别，记录错误和警告日志
		public const int LOGLEVEL_WARN = 1;
		//调试信息级别，记录错误、警告和调试信息，为最详尽的日志级别
		public const int LOGLEVEL_DEBUG = 2;

		//竖屏
		public const int ORIENTATION_PORTRAIT = 0;
		//横屏
		public const int ORIENTATION_LANDSCAPE = 1;

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
	}

	// <summary>
	// 定义 SDK 方法调用的返回状态码
	// </summary>
	public class XiaoMiStatusCode
	{
		// 没有初始化后不允许执行
		public const int NO_INIT = -10;

		// 没有登录后不允许执行
		public const int NO_LOGIN = -11;

		// 检测不了网咯不允许执行
		public const int NO_NETWORK = -12;

		// 初始化失败
		public const int INIT_FAIL = -100;

		// 登录失败
		public const int LOGIN_FAIL = -102;


        public const int MI_XIAOMI_GAMECENTER_SUCCESS = 0;						// 购买成功

		public const int MI_XIAOMI_GAMECENTER_ERROR_PAY_CANCEL = -18004;		// 取消购买

		public const int MI_XIAOMI_GAMECENTER_ERROR_PAY_FAILURE = -18003;		// 购买失败

		public const int MI_XIAOMI_GAMECENTER_ERROR_ACTION_EXECUTED = -18006;   //操作正在进行中

		public const int MI_XIAOMI_EXIT = 10001;

		public const int CALLBACK_TIMEOUT = -1000;
	}

	public class XiaoMiGameSdk
	{
		private const string SDK_JAVA_CLASS = "cn.xiaomi.gamesdk.unity3d.XiaoMiGameSDK";

		public static void initSDK()
		{
			EB.Debug.Log("call initSDK....");
			callSdkApi("initSDK");
		}

		public static void login()
		{
			callSdkApi("login");
		}

		public static void pay(int amount,					
					string serverId,
					string roleId,
					string roleName,
					string roleLevel,
					string vipLevel,
					string allianceName,
					string transactionNumCP)
		{
			//如游戏为横屏，请在initSDK接口增加横屏属性
			callSdkApi("pay", amount, serverId, roleId, roleName, roleLevel, vipLevel, allianceName, transactionNumCP);
		}

		// <summary>
		// 退出当前登录的账号
		// </summary>
		public static void logout()
		{
			callSdkApi("logout");
		}

		/// <summary>
		/// 退出SDK，游戏退出前必须调用此方法，以清理SDK占用的系统资源。如果游戏退出时不调用该方法，可能会引起程序错误。
		/// </summary>
		public static void exitSDK()
		{
			callSdkApi("exitSDK");
		}

		private static void callSdkApi(string apiName, params object[] args)
		{
			log("Unity3D " + apiName + " calling...");

			using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
			{
				cls.CallStatic(apiName, args);
			}
		}

		private static void log(string msg)
		{
			EB.Debug.Log(msg);
		}
	}

	public class XiaoMiSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mInitializing = false;
		private bool mLoggedIn = false;
		private long mUid;
		private System.Action<string, object> mAuthCallback = null;
		private System.Action<string, bool> mInitCallback = null;
		private System.Action<int, object> mPayCallback = null;
		private System.Action mDestroyCallback = null;

		public long Uid { get { return mUid; } }

		//private string mNotifyUrl = string.Empty;

		public bool IsLoggedIn { get { return mLoggedIn; } }

		public override void Initialize(Config config)
		{

		}

		//public override void Destroy(System.Action callback)
		//{
		//	if (!mInitialized)
		//	{
		//		callback();
		//		return;
		//	}

		//	mDestroyCallback += callback;
		//	DestroySDK();
		//}

		public override void OnEnteredForeground()
		{
			//if (mAuthCallback != null)
			//{
			//	EB.Coroutines.SetTimeout(delegate ()
			//	{
			//		if (mAuthCallback != null)
			//		{
			//			mAuthCallback(null, null);
			//			mAuthCallback = null;
			//		}
			//	}, 5 * 1000);
			//}

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
						mPayCallback(XiaoMiStatusCode.CALLBACK_TIMEOUT, null);
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
				EB.Debug.LogWarning("XiaoMiSDKManager.InitializeSDK: Initialized");
				callback(null, true);
				return;
			}

			if (Application.platform != RuntimePlatform.Android)
			{
				callback(null, false);
				return;
			}

			mInitCallback += callback;

			//mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);

			//bool debugMode = EB.Dot.Bool("debugMode", options, false);
			//int gameId = EB.Dot.Integer("gameId", options, 0);

			if (!mInitializing)
			{
				mInitializing = true;

				Hub.RunInBackground = true;

				new GameObject("xiaomi_plugin_listener", typeof(SparxXiaoMiSDKManager));
				XiaoMiGameSdk.initSDK();
			}
		}

		//public override void OnLoggedIn()
		//{
		//	var user = Hub.LoginManager.LocalUser;
		//	Hashtable loginGameRole = new Hashtable();
		//	loginGameRole["roleId"] = user.Id.Value.ToString();
		//	loginGameRole["roleName"] = user.Name;
		//	loginGameRole["zoneId"] = user.WorldId.ToString();
		//	loginGameRole["zoneName"] = string.Empty;
		//	loginGameRole["roleCTime"] = 0L;
		//	loginGameRole["roleLevel"] = user.Level.ToString();
		//	//loginGameRole["roleLevelMTime"] = null;	

		//	SubmitLoginData(EB.JSON.Stringify(loginGameRole));
		//}

		public void Login(System.Action<string, object> callback)
		{
			if (mLoggedIn)
			{
				callback(null, null);
				return;
			}

			mAuthCallback += callback;

			Hub.RunInBackground = true;
			XiaoMiGameSdk.login();
		}

		public void Logout()
		{
			if (mLoggedIn)
			{
				mLoggedIn = false;
				//XiaoMiGameSdk.logout();
			}
		}

		public void DestroySDK()
		{
			if (mInitialized)
			{
				mInitialized = false;
				mLoggedIn = false;
				mAuthCallback = null;
				mInitCallback = null;
				mPayCallback = null;
				XiaoMiGameSdk.exitSDK();
			}
		}

		public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int, object> callback)
		{
			if (!mInitialized)
			{
				EB.Debug.LogError("XiaoMiSDKManager.Pay: not initialized");
				callback(XiaoMiStatusCode.NO_INIT, null);
				return;
			}

			mPayCallback += callback;

			var user = Hub.Instance.LoginManager.LocalUser;
			long uid = user.Id.Value;
			int worldId = user.WorldId;
			string name = user.Name;
			int level = user.Level;
			int vipLevel = user.Vip;
			XiaoMiGameSdk.pay((int)item.cost, worldId.ToString(), uid.ToString(), name, level.ToString(), vipLevel.ToString(), string.Empty, transaction.transactionId);
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
			EB.Debug.Log("XiaoMiSDKManager.OnLoginResult:code={0} msg={1}",code,msg);
			Hub.RunInBackground = false;
			if (code == 0)
			{
				if (mAuthCallback != null)
				{
					//object data = EB.JSON.Parse(msg);
					string session = EB.Dot.String("session", msg, string.Empty);
					long uid = EB.Dot.Long("uid", msg, 0);
					mUid = uid;
					Hashtable data = new Hashtable()
					{
						{"session", session },
						{"uid",uid}
					};
					mAuthCallback(null, data);
					mAuthCallback = null;
				}
			}
			else
			{
                if(code== XiaoMiStatusCode.LOGIN_FAIL)
                {
                    if (mAuthCallback != null)
                    {
                        mAuthCallback(null, null);
                        mAuthCallback = null;
                    }
                }
                else
                {
                    if (mAuthCallback != null)
                    {
                        string session = EB.Dot.String("session", msg, string.Empty);
                        mAuthCallback(session, null);
                        mAuthCallback = null;
                    }
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
			EB.Debug.Log("OnExitSDK: code={0}, msg={1}", msg);
			
			if (mDestroyCallback != null)
			{
				mDestroyCallback();
				mDestroyCallback = null;
			}
		}
	}
}

public class SparxXiaoMiSDKManager : MonoBehaviour
{
	void Awake()
	{
		EB.Debug.Log("Creating XiaoMi Manager");
		DontDestroyOnLoad(gameObject);
	}

	public void OnXiaoMiGameSdkCallback(string jsonstr)
	{
		EB.Debug.Log("OnXiaoMiGameSdkCallback message: jsonstr = {0}", jsonstr);

		object json = EB.JSON.Parse(jsonstr);
		string callbackType = EB.Dot.String("callbackType", json, string.Empty);
		int code = EB.Dot.Integer("code", json, 0);

		switch (callbackType)
		{
			case EB.Sparx.XiaoMiConstants.CALLBACKTYPE_InitSDK:
				SparxHub.Instance.XiaoMiSDKManager.OnInitResult(code,EB.Dot.String("data", json, null));
				break;
			case EB.Sparx.XiaoMiConstants.CALLBACKTYPE_Login:
				SparxHub.Instance.XiaoMiSDKManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
				break;
			case EB.Sparx.XiaoMiConstants.CALLBACKTYPE_Logout:
				SparxHub.Instance.XiaoMiSDKManager.OnLogout(code, EB.Dot.String("data", json, null));
				break;
			case EB.Sparx.XiaoMiConstants.CALLBACKTYPE_Pay:
				SparxHub.Instance.XiaoMiSDKManager.OnPayCallback(code, EB.Dot.Object("data", json, null));
				break;
			case EB.Sparx.XiaoMiConstants.CALLBACKTYPE_ExitSDK:
				SparxHub.Instance.XiaoMiSDKManager.OnExitSDK(code, EB.Dot.String("data", json, null));
				break;
			default:
				EB.Debug.LogWarning("OnXiaoMiGameSdkCallback: unknown type = {0}", callbackType);
				break;
		}
	}

    void OnApplicationPause(bool pauseStatus)
    {
        EB.Debug.Log("XiaoMiSDKManager.OnApplicationPause: status = {0}", pauseStatus);
        if (!pauseStatus)
        {
            SparxHub.Instance.XiaoMiSDKManager.OnLoginEnteredForeground();
        }
    }
}
#endif