#if USE_UCSDK && UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public class SDKParams : Dictionary<string, object>
    {

    }

    public class SDKParamKey
    {
        public const string ROLE_ID = "roleId";
        public const string ROLE_NAME = "roleName";
        public const string ROLE_LEVEL = "roleLevel";
        public const string ROLE_CTIME = "roleCTime";
        public const string ZONE_ID = "zoneId";
        public const string ZONE_NAME = "zoneName";
        public const string GRADE = "grade";
        public const string CALLBACK_INFO = "callbackInfo";
        public const string AMOUNT = "amount";
        public const string SERVER_ID = "serverId";
        public const string NOTIFY_URL = "notifyUrl";
        public const string CP_ORDER_ID = "cpOrderId";
        public const string ACCOUNT_ID = "accountId";
        public const string SIGN_TYPE = "signType";
        public const string SIGN = "sign";

        public const string DEBUG_MODE = "debugMode";
        public const string GAME_PARAMS = "gameParams";
        public const string GAME_ID = "gameId";
        public const string ENABLE_PAY_HISTORY = "enablePayHistory";
        public const string ENABLE_USER_CHARGE = "enableUserCharge";
        public const string ORIENTATION = "orientation";

        public const string STRING_ROLE_ID = "roleId";
        public const string STRING_ROLE_NAME = "roleName";
        public const string LONG_ROLE_LEVEL = "roleLevel";
        public const string LONG_ROLE_CTIME = "roleCTime";
        public const string STRING_ZONE_ID = "zoneId";
        public const string STRING_ZONE_NAME = "zoneName";

        public const string ACTION = "action";
        public const string BUSINESS = "business";
    }
    
    public enum UCOrientation
    {
        PORTRAIT = 0,
        LANDSCAPE = 1
    }

    public class GameParamInfo
    {
        public int GameId { get; set; }

        public bool EnablePayHistory { get; set; }

        public bool EnableUserChange { get; set; }

        public UCOrientation Orientation { get; set; }

    }

    /// <summary>
    /// 定义 SDK 方法调用的返回状态码
    /// </summary>
    public class UCStatusCode
	{
		// 调用成功
		public const int SUCCESS = 0;

		// 没有初始化后不允许执行
		public const int NO_INIT = -10;

		// 用户退出充值界面
		public const int PAY_USER_EXIT = -500;

		// 等待回调超时
		public const int CALLBACK_TIMEOUT = -1000;
	}

    /// <summary>
    /// SDK调用入口类
    /// </summary>
    public class UCGameSdk : MonoBehaviour
    {
        /// <summary>
        /// 调用android java端接口类
        /// </summary>
        private const string SDK_JAVA_CLASS = "cn.uc.gamesdk.unity3d.UCGameSDK";

        /// <summary>
        /// 初始化SDK
        /// </summary>
        public static void initSDK(SDKParams sdkParams)
        {
            bool debugMode = false;
            if (sdkParams.ContainsKey(SDKParamKey.DEBUG_MODE))
            {
                debugMode = (bool)sdkParams[SDKParamKey.DEBUG_MODE];
            }

            string gameId = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.GAME_ID))
            {
                gameId =sdkParams[SDKParamKey.GAME_ID].ToString();
            }

            GameParamInfo gameInfo = new GameParamInfo();
            if (sdkParams.ContainsKey(SDKParamKey.GAME_PARAMS))
            {
                gameInfo = (GameParamInfo)sdkParams[SDKParamKey.GAME_PARAMS];
            }

            int orientation;
            switch (gameInfo.Orientation)
            {
                case UCOrientation.PORTRAIT:
                    orientation = 0;
                    break;
                case UCOrientation.LANDSCAPE:
                    orientation = 1;
                    break;
                default:
                    orientation = 0;
                    break;
            }
            callSdkApi("initSDK", debugMode, gameInfo.GameId, gameInfo.EnablePayHistory, gameInfo.EnableUserChange, orientation);
        }

        /// <summary>
        /// 调用SDK的用户登录 
        /// </summary>
        public static void login()
        {
            callSdkApi("login");
        }

        /// <summary>
        /// 退出当前登录的账号
        /// </summary>
        public static void logout()
        {
            callSdkApi("logout");
        }

        /// <summary>
        /// 设置玩家选择的游戏分区及角色信息 
        /// </summary>
        public static void submitRoleData(SDKParams sdkParams)
        {
            string zoneId = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.STRING_ZONE_ID))
            {
                zoneId = (string)sdkParams[SDKParamKey.STRING_ZONE_ID];
            }

            string zoneName = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.STRING_ZONE_NAME))
            {
                zoneName = (string)sdkParams[SDKParamKey.STRING_ZONE_NAME];
            }

            string roleId = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.STRING_ROLE_ID))
            {
                roleId = (string)sdkParams[SDKParamKey.STRING_ROLE_ID];
            }

            string roleName = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.STRING_ROLE_NAME))
            {
                roleName = (string)sdkParams[SDKParamKey.STRING_ROLE_NAME];
            }

            long roleLevel = 0;
            if (sdkParams.ContainsKey(SDKParamKey.LONG_ROLE_LEVEL))
            {
                roleLevel = (long)sdkParams[SDKParamKey.LONG_ROLE_LEVEL];
            }

            long roleCTime = 0;
            if (sdkParams.ContainsKey(SDKParamKey.LONG_ROLE_CTIME))
            {
                roleCTime = (long)sdkParams[SDKParamKey.LONG_ROLE_CTIME];
            }

            callSdkApi("submitRoleData", zoneId, zoneName, roleId, roleName, roleLevel, roleCTime);
        }

        /// <summary>
        /// 支付
        /// </summary>
        public static void pay(SDKParams sdkParams)
        {
            string accountId = null;
            if (sdkParams.ContainsKey(SDKParamKey.ACCOUNT_ID))
            {
                accountId = (string)sdkParams[SDKParamKey.ACCOUNT_ID];
            }

            string cpOrderId = null;
            if (sdkParams.ContainsKey(SDKParamKey.CP_ORDER_ID))
            {
                cpOrderId = (string)sdkParams[SDKParamKey.CP_ORDER_ID];
            }

            string amount = null;
            if (sdkParams.ContainsKey(SDKParamKey.AMOUNT))
            {
                amount = (string)sdkParams[SDKParamKey.AMOUNT];
            }

            string serverId = null;
            if (sdkParams.ContainsKey(SDKParamKey.SERVER_ID))
            {
                serverId = (string)sdkParams[SDKParamKey.SERVER_ID];
            }

            string roleId = null;
            if (sdkParams.ContainsKey(SDKParamKey.ROLE_ID ))
            {
                roleId = (string)sdkParams[SDKParamKey.ROLE_ID];
            }
            
            string roleName = null;
            if (sdkParams.ContainsKey(SDKParamKey.ROLE_NAME ))
            {
                roleName = (string)sdkParams[SDKParamKey.ROLE_NAME];
            }

            string grade = null;
            if (sdkParams.ContainsKey(SDKParamKey.GRADE))
            {
                grade = (string)sdkParams[SDKParamKey.GRADE];
            }

            string callbackInfo = null;
            if (sdkParams.ContainsKey(SDKParamKey.CALLBACK_INFO))
            {
                callbackInfo = (string)sdkParams[SDKParamKey.CALLBACK_INFO];
            }

            string notifyUrl = null;
            if (sdkParams.ContainsKey(SDKParamKey.NOTIFY_URL))
            {
                notifyUrl = (string)sdkParams[SDKParamKey.NOTIFY_URL];
            }

            string signType = null;
            if (sdkParams.ContainsKey(SDKParamKey.SIGN_TYPE))
            {
                signType = (string)sdkParams[SDKParamKey.SIGN_TYPE];
            }

            string sign = null;
            if (sdkParams.ContainsKey(SDKParamKey.SIGN))
            {
                sign = (string)sdkParams[SDKParamKey.SIGN];
            }

            callSdkApi("pay", accountId, cpOrderId, amount, serverId, roleId, roleName, grade, callbackInfo, notifyUrl, signType, sign);
        }

        /// <summary>
        /// 打开指定页面
        /// </summary>
        public static void showPage(SDKParams sdkParams)
        {
            string action = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.ACTION))
            {
                action = (string)sdkParams[SDKParamKey.ACTION];
            }

            string business = string.Empty;
            if (sdkParams.ContainsKey(SDKParamKey.BUSINESS))
            {
                business = (string)sdkParams[SDKParamKey.BUSINESS];
            }

            int orientation = 0;
            if (sdkParams.ContainsKey(SDKParamKey.ORIENTATION))
            {
                orientation = (int)sdkParams[SDKParamKey.ORIENTATION];
            }

            callSdkApi("showPage", action, business, orientation);
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
            //log("Unity3D " + apiName + " calling...");
            using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                cls.CallStatic(apiName, args);
            }
        }

        /*public static void log(string msg)
        {
            Text mText = GameObject.Find("MsgText").GetComponent<Text>();
            if (mText != null)
            {
                mText.text = mText.text + System.Environment.NewLine + msg;
            }
        }*/

    }

    public class UCSDKManager : Manager
	{
		private static bool mInitialized = false;
		private static bool mInitializing = false;
		private static System.Action<string, bool> mInitCallback = null;

        private const int defaultGameId = 100861;//734222;
        
        private bool mLoggedIn = false;
		private string mSid = string.Empty;
		private System.Action<string, object> mAuthCallback = null;
		private System.Action<int, object> mPayCallback = null;
		private System.Action<bool> mExitCallback = null;

		private string mNotifyUrl = string.Empty;

		public bool IsLoggedIn { get { return mLoggedIn; } }

		public string Sid { get { return mSid; } }

		public override void Initialize(Config config)
		{
			
		}

		public static void InitializeSDK(System.Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.LogWarning("UCSDKManager.InitializeSDK: Initialized");
				callback(null, true);
				return;
			}

			if (Application.platform != RuntimePlatform.Android)
			{
				callback("Not support", false);
				return;
			}

			mInitCallback += callback;

			if (!mInitializing)
			{
				mInitializing = true;

				SparxUCSDKManager.InitCallback += delegate (bool sucess, string msg)
				{
					EB.Debug.Log("InitCallback: sucess={0}, msg={1}", sucess, msg);

					mInitializing = false;

					if (sucess)
					{
						mInitialized = true;
					}

					if (mInitCallback != null)
					{
						mInitCallback(mInitialized ? null : msg, mInitialized);
						mInitCallback = null;
					}
				};

				new GameObject("uc_plugin_listener", typeof(SparxUCSDKManager));

				SDKParams param = new SDKParams();
				param.Add(SDKParamKey.DEBUG_MODE, false);
				param.Add(SDKParamKey.GAME_ID, defaultGameId);

				GameParamInfo info = new GameParamInfo();
				info.GameId = defaultGameId;
				info.Orientation = UCOrientation.LANDSCAPE;
				info.EnablePayHistory = true;
				info.EnableUserChange = true;
				param.Add(SDKParamKey.GAME_PARAMS, info);

				UCGameSdk.initSDK(param);
			}
		}

		public void InitializeSDK(object options, System.Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.LogWarning("UCSDKManager.InitializeSDK: Initialized");
				callback(null, true);
				return;
			}

			if (Application.platform != RuntimePlatform.Android)
			{
				callback(null, false);
				return;
			}

			mInitCallback += callback;

			mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);

			bool debugMode = EB.Dot.Bool("debugMode", options, false);
			int gameId = EB.Dot.Integer("gameId", options, 0);

			if (!mInitializing)
			{
				mInitializing = true;

				Hub.RunInBackground = true;

				new GameObject("uc_plugin_listener", typeof(SparxUCSDKManager));

				SDKParams param = new SDKParams();
				param.Add(SDKParamKey.DEBUG_MODE, debugMode);
				param.Add(SDKParamKey.GAME_ID, gameId);

				GameParamInfo info = new GameParamInfo();
				info.GameId = gameId;
				info.Orientation = UCOrientation.LANDSCAPE;
				info.EnablePayHistory = true;
				info.EnableUserChange = true;
				param.Add(SDKParamKey.GAME_PARAMS, info);

				UCGameSdk.initSDK(param);
			}
		}

		public override void OnLoggedIn()
		{
			SubmitLoginData(Hub.LoginManager.LocalUser.Level);
		}

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
						mPayCallback(UCStatusCode.CALLBACK_TIMEOUT, null);
						mPayCallback = null;
					}
				}, 5 * 1000);
			}

			if (mExitCallback != null)
			{
				EB.Coroutines.SetTimeout(delegate ()
				{
					if (mExitCallback != null)
					{
						mExitCallback(false);
						mExitCallback = null;
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

		public void Login(System.Action<string, object> callback)
		{
			if (mLoggedIn)
			{
				Hashtable data = new Hashtable()
				{
					{"sid", mSid }
				};
				callback(null, data);
				return;
			}

			mAuthCallback += callback;

			Hub.RunInBackground = true;
			UCGameSdk.login();
		}

		public void Logout()
		{
			if (mLoggedIn)
			{
				mLoggedIn = false;
				UCGameSdk.logout();
			}
		}

		public void ExitSDK(System.Action<bool> callback)
		{
			if (mInitialized)
			{
				mExitCallback += callback;
				UCGameSdk.exitSDK();
			}
		}

		public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int, object> callback)
		{
			if (!mInitialized)
			{
				EB.Debug.LogError("UCSDKManager.Pay: not initialized");
				callback(UCStatusCode.NO_INIT, null);
				return;
			}

			mPayCallback += callback;

			var user = Hub.Instance.LoginManager.LocalUser;
			long uid = user.Id.Value;
			//int worldId = user.WorldId;
			string name = user.Name;
			int level = user.Level;

			object extraInfo = EB.JSON.Parse(transaction.payload);

			SDKParams param = new SDKParams();
			param.Add(SDKParamKey.ACCOUNT_ID, EB.Dot.String("accountId", extraInfo, "0"));
			param.Add(SDKParamKey.CP_ORDER_ID,EB.Dot.String("cpOrderId", extraInfo, transaction.transactionId));
			param.Add(SDKParamKey.AMOUNT, EB.Dot.String("amount", extraInfo, string.Format("{0:N2}", item.cost)));
			param.Add(SDKParamKey.SERVER_ID, EB.Dot.String("serverId", extraInfo, "0"));
			param.Add(SDKParamKey.ROLE_ID, EB.Dot.String("roleId", extraInfo, uid.ToString()));
			param.Add(SDKParamKey.ROLE_NAME, EB.Dot.String("roleName", extraInfo, name));
			param.Add(SDKParamKey.GRADE, EB.Dot.String("grade", extraInfo, level.ToString()));
			param.Add(SDKParamKey.CALLBACK_INFO, EB.Dot.String("callbackInfo", extraInfo, "0"));
			param.Add(SDKParamKey.NOTIFY_URL, EB.Dot.String("notifyUrl", extraInfo, mNotifyUrl));
			param.Add(SDKParamKey.SIGN_TYPE, "MD5");
			param.Add(SDKParamKey.SIGN, transaction.signature);
			UCGameSdk.pay(param);
		}

		public void SubmitLoginData(int level)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (!mInitialized)
				{
					EB.Debug.LogError("UCSDKManager.SubmitLoginData: not initialized");
					return;
				}

				var user = Hub.LoginManager.LocalUser;
				if (user == null)
				{
					return;
				}

				var worlds = Hub.LoginManager.GameWorlds;
				var world = System.Array.Find(worlds, w => w.Id == user.WorldId);
				string worldName = world != null ? world.Name : (worlds.Length > 0 ? worlds[0].Name : "Default");

				SDKParams loginGameRole = new SDKParams();
				loginGameRole.Add(SDKParamKey.STRING_ZONE_ID, user.WorldId.ToString());
				loginGameRole.Add(SDKParamKey.STRING_ZONE_NAME, worldName);
				loginGameRole.Add(SDKParamKey.STRING_ROLE_ID, user.Id.Value.ToString());
				loginGameRole.Add(SDKParamKey.STRING_ROLE_NAME, user.Name);
				loginGameRole.Add(SDKParamKey.LONG_ROLE_LEVEL, (long)level);
				loginGameRole.Add(SDKParamKey.LONG_ROLE_CTIME, (long)user.CreateTime);
				UCGameSdk.submitRoleData(loginGameRole);
			}
		}

		public void OnInitResult(bool success, string msg)
		{
			EB.Debug.Log("OnInitResult: success={0}, msg={1}", success, msg);

			mInitializing = false;
			Hub.RunInBackground = false;

			if (success)
			{
				mInitialized = true;
			}

			if (mInitCallback != null)
			{
				mInitCallback(mInitialized ? null : msg, mInitialized);
				mInitCallback = null;
			}
		}

		public void OnLoginResult(bool success, string msg)
		{
			EB.Debug.Log("OnLoginResult: success={0}, msg={1}", success, msg);

			Hub.RunInBackground = false;

			if (success)
			{
				mLoggedIn = true;
				mSid = msg;
				
				if (mAuthCallback != null)
				{
					Hashtable data = new Hashtable()
					{
						{"sid", mSid }
					};
					mAuthCallback(null, data);
					mAuthCallback = null;
				}
			}
			else
			{

				if (mAuthCallback != null)
				{
					mAuthCallback(null, null);
					mAuthCallback = null;
				}
			}
		}

		public void OnLogout(bool success, string msg)
		{
			EB.Debug.Log("OnLogout: success={0}, msg={1}", success, msg);

			if (success)
			{
				if (mLoggedIn)
				{
					mLoggedIn = false;
					Hub.Instance.Disconnect(true);
				}
			}
		}

		public void OnPayCallback(int code, object jsonOrder)
		{
			EB.Debug.Log("OnPayCallback: code={0}", code);

			if (mPayCallback != null)
			{
				mPayCallback(code, jsonOrder);
				mPayCallback = null;
			}
		}

		public void OnExitSDK(bool success, string msg)
		{
			EB.Debug.Log("OnExitSDK: success={0}, msg={1}", success, msg);

			if (mExitCallback != null)
			{
				mExitCallback(success);
				mExitCallback = null;
			}
		}
	}
}

public class SparxUCSDKManager : MonoBehaviour
{
	public static System.Action<bool, string> InitCallback = null;

	private bool mOrderCreated = false;

	void Awake()
	{
		EB.Debug.Log("Creating SparxUCSDKManager");
		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationPause(bool pauseStatus)
	{
		EB.Debug.Log("SparxUCSDKManager.OnApplicationPause: status = {0}", pauseStatus);
		if (!pauseStatus && SparxHub.Instance != null)
		{
			SparxHub.Instance.UCSDKManager.OnLoginEnteredForeground();
		}
	}

	public void onInitSucc(string emptyString)
	{
		if (SparxHub.Instance != null)
		{
			SparxHub.Instance.UCSDKManager.OnInitResult(true, emptyString);
		}

		if (InitCallback != null)
		{
			InitCallback(true, emptyString);
			InitCallback = null;
		}
	}

	public void onInitFailed(string data)
	{
		if (SparxHub.Instance != null)
		{
			SparxHub.Instance.UCSDKManager.OnInitResult(false, data);
		}

		if (InitCallback != null)
		{
			InitCallback(false, data);
			InitCallback = null;
		}
	}

	public void onLoginSucc(string sid)
	{
		SparxHub.Instance.UCSDKManager.OnLoginResult(true, sid);
	}

	public void onLoginFailed(string desc)
	{
		SparxHub.Instance.UCSDKManager.OnLoginResult(false, desc);
	}

	public void onLogoutSucc(string emptyString)
	{
		SparxHub.Instance.UCSDKManager.OnLogout(true, emptyString);
	}

	public void onLogoutFailed(string emptyString)
	{
		SparxHub.Instance.UCSDKManager.OnLogout(false, emptyString);
	}

	public void onExitSucc(string desc)
	{
		SparxHub.Instance.UCSDKManager.OnExitSDK(true, desc);
	}

	public void onExitCanceled(string desc)
	{
		SparxHub.Instance.UCSDKManager.OnExitSDK(false, desc);
	}

	public void onCreateOrderSucc(string orderJson)
	{
		EB.Debug.Log("SparxUCSDKManager.onCreateOrderSucc: {0}", orderJson);
		mOrderCreated = true;
	}

	public void onPayUserExit(string orderJson)
	{
		if (mOrderCreated)
		{
			// can't distinguish sucess or canceld status
			SparxHub.Instance.UCSDKManager.OnPayCallback(EB.Sparx.UCStatusCode.SUCCESS, EB.JSON.Parse(orderJson));
			mOrderCreated = false;
		}
		else
		{
			SparxHub.Instance.UCSDKManager.OnPayCallback(EB.Sparx.UCStatusCode.PAY_USER_EXIT, EB.JSON.Parse(orderJson));
		}
	}
}
#endif