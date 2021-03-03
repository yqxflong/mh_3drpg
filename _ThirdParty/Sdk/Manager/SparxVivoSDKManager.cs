#if UNITY_ANDROID && USE_VIVOSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
	public class VivoStatusCode
	{
		public const int SUCCESS = 0;

		public const int FAIL = -1;

		public const int PAY_SUCCESS_CODE = 9000;

		public const int PAY_CANCEL_CODE = 6001;

		public const int PAY_FAIL_CODE = 4006;

		public const int CALLBACK_TIMEOUT = -1000;
	}

	public class VivoSDKCallbackType
	{
		public const string CALLBACKTYPE_INITSDK = "InitSDK";

		public const string CALLBACKTYPE_LOGIN = "Login";

		public const string CALLBACKTYPE_PAY = "Pay";

		public const string CALLBACKTYPE_REPORT = "ReportAccountRoleInfo";

		public const string CALLBACKTYPE_EXITSDK = "ExitSDK";

	}

	public class VivoGameSDK
	{
		private const string SDK_JAVA_CLASS = "cn.vivo.gamesdk.unity3d.VivoGameSDK";

		public static void InitSDK()
		{
			callSdkApi("InitSDK");
		}

		public static void Login()
		{
			callSdkApi("Login");
		}

		public static void Pay(int price, int level, String productName, String productDes, String vip, String party, String roleId, String roleName, String serverName, String transNo, String accessKey)
		{
			callSdkApi("Pay", price, level, productName, productDes, vip, party, roleId, roleName, serverName, transNo, accessKey);
		}

		public static void ReportAccountRoleInfo(String roleId, String roleLevel, String serviceArea, String roleName, String serviceAreaName)
		{
			callSdkApi("ReportAccountRoleInfo", roleId, roleLevel, serviceArea, roleName, serviceAreaName);
		}

		public static void ExitSDK()
		{
			callSdkApi("ExitSDK");
		}

		public static void ShowVivoAssitView()
		{
			callSdkApi("ShowVivoAssitView");
		}

		public static void HideVivoAssitView()
		{
			callSdkApi("HideVivoAssitView");
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

	public class VivoSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mLogined = false;

		private System.Action<string, bool> mInitCallback;
		private System.Action<string, object> mLoginCallback;
		private System.Action<int,object> mPayCallback;
		//private System.Action<bool> mExitSDKCallback;
	
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
			ReportAccountRoleInfo(roleId, roleLevel, "serviceAreaId", roleName, "serviceAreaName");
		}

		public override void OnEnteredForeground()
		{
			//if (mLogined)
			//{
			//	VivoGameSDK.ShowVivoAssitView();
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
						mPayCallback(VivoStatusCode.CALLBACK_TIMEOUT, null);
						mPayCallback = null;
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

			//if (mLogined)
			//{
			//	VivoGameSDK.HideVivoAssitView();
			//}
		}

		//public override void Destroy(Action callback)
		//{
		//    if (!mInitialized)
		//    {
		//        callback();
		//        return;
		//    }
		//    ExitSDK(callback);
		//}

		public void InitSDK(object options, Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.LogWarning("VivoSDKManager.InitializeSDK: Initialized");
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
			new GameObject("vivo_plugin_listener", typeof(SparxVivoSDKManager));
			VivoGameSDK.InitSDK();
		}

		public void OnInitCallback(int code)
		{
			Hub.RunInBackground = false;
			if (code == VivoStatusCode.SUCCESS)
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
			EB.Debug.Log("VivoSDKManager.Login");
			if (!mInitialized)
			{
				callback("VivoSD has not been inited", null);
				return;
			}

			mLoginCallback += callback;

			Hub.RunInBackground = true;
			VivoGameSDK.Login();
		}

		public void OnLoginCallback(int code,object msg)
		{
			EB.Debug.Log("VivoSDKManager.OnLoginResult:code={0}", code);
			Hub.RunInBackground = false;
			if (code == VivoStatusCode.SUCCESS)
			{
				VivoGameSDK.ShowVivoAssitView();
				if (mLoginCallback != null)
				{
					mLogined = true;
					string nameStr = EB.Dot.String("name", msg, string.Empty);
					string openId = EB.Dot.String("openid", msg, string.Empty);
					String authtoken = EB.Dot.String("authtoken", msg, string.Empty);
					Hashtable data = new Hashtable()
					{
						{"name", nameStr },
						{"openid",openId},
						{ "authtoken",authtoken}
					};
					mLoginCallback(null, data);
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

		public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int,object> callback)
		{
			EB.Debug.Log("VivoSDKManager.Pay");
			if (!mInitialized)
			{
				EB.Debug.LogError("QiHooSDKManager.Pay: not initialized");
				callback(VivoStatusCode.FAIL,null);
				return;
			}

			mPayCallback += callback;
			var user = Hub.Instance.LoginManager.LocalUser;
			long uid = user.Id.Value;
			//int worldId = user.WorldId;
			string userName = user.Name;
			int level = user.Level;
			int vipLevel = user.Vip;
			object payload = JSON.Parse(transaction.payload);
			string accessKey = EB.Dot.String("accessKey", payload, string.Empty);
			string orderNumber = EB.Dot.String("orderNumber", payload,string.Empty);
			VivoGameSDK.Pay(item.cents,level,item.longName, item.localizedDesc, vipLevel.ToString(),"noalliance",uid.ToString(), userName, "serverName", orderNumber, accessKey);			
			//EB.Debug.LogError("price={0},level={1},productName={2},productDes={3},vip={4},party={5},roleId={6},roleName={7},serverName={8},transNo={9},accessKey={10}", item.cents, level, item.longName, item.localizedDesc, vipLevel.ToString(), "noalliance", uid.ToString(), userName, "serverName", orderNumber, accessKey);
		}

		public void OnPayCallback(int code,object msg)
		{
			EB.Debug.Log("VivoSDKManager.OnPayCallback: code = {0}", code);
			if (mPayCallback != null)
			{
				mPayCallback(code, msg);
				mPayCallback = null;
			}
		}

		public void ReportAccountRoleInfo(String roleId, String roleLevel, String serviceArea, String roleName, String serviceAreaName)
		{
			VivoGameSDK.ReportAccountRoleInfo(roleId, roleLevel, serviceArea, roleName, serviceAreaName);
		}

		public void ExitSDK()
		{
			VivoGameSDK.ExitSDK();
		}

		public void OnExitSDK(int code)
		{
			//Hub.RunInBackground = false;
			//if (mExitSDKCallback != null)
			//{
			//	if (code == VivoStatusCode.SUCCESS)
			//	{
			//		mExitSDKCallback(true);
			//	}
			//	else
			//	{
			//		mExitSDKCallback(false);
			//	}
			//}
		}
	}

	public class SparxVivoSDKManager : MonoBehaviour
	{
		void Awake()
		{
			EB.Debug.Log("Creating Vivo Manager");
			DontDestroyOnLoad(gameObject);
		}

		public void OnVivoGameSDKCallback(string jsonString)
		{
			EB.Debug.Log("SparxVivoSDKManager.OnVivoGameSDKCallback:jsonString = {0}", jsonString);
			object json = EB.JSON.Parse(jsonString);
			string callbackType = Dot.String("callbackType", json, string.Empty);
			int code = Dot.Integer("code", json, -1);
			switch (callbackType)
			{
				case VivoSDKCallbackType.CALLBACKTYPE_INITSDK:
					Sparx.Hub.Instance.VivoSDKManager.OnInitCallback(code);
					break;
				case VivoSDKCallbackType.CALLBACKTYPE_LOGIN:
					Sparx.Hub.Instance.VivoSDKManager.OnLoginCallback(code, EB.Dot.Object("data", json, null));
					break;
				case VivoSDKCallbackType.CALLBACKTYPE_PAY:
					Sparx.Hub.Instance.VivoSDKManager.OnPayCallback(code, EB.Dot.Object("data", json, null));
					break;
				case VivoSDKCallbackType.CALLBACKTYPE_EXITSDK:
					Sparx.Hub.Instance.VivoSDKManager.OnExitSDK(code);
					break;
			}
		}

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxVivoSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus)
            {
                SparxHub.Instance.VivoSDKManager.OnLoginEnteredForeground();
            }
        }
    }
}
#endif
