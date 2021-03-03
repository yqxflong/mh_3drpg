#if UNITY_ANDROID && USE_OPPOSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class OPPOStatusCode
    {
        public const int SUCCESS = 0;

        public const int FAILURE = 1;

        public const int PAY_TIMEOUT = 2;
    }

    public class OPPOSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_GETTOKENANDSSOID = "GetTokenAndSsoid";

        public const string CALLBACKTYPE_GETUSERINFO = "GetUserInfo";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_REPORT = "ReportUserGameInfoData";

        public const string CALLBACKTYPE_EXITSDK = "ExitSDK";

    }

    public class OPPOGameSDK
    {
        private const string SDK_JAVA_CLASS = "cn.oppo.gamesdk.unity3d.OPPOGameSDK";

        public static void InitSDK(string appId, string appSecret)
        {
            callSdkApi("InitSDK", appId, appSecret);
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void OnResume()
        {
            callSdkApi("OnResume");
        }

        public static void OnPause()
        {
            callSdkApi("OnPause");
        }

        public static void GetTokenAndSsoid()
        {
            callSdkApi("GetTokenAndSsoid");
        }

        public static void GetUserInfo(string token, string ssoid)
        {
            callSdkApi("GetUserInfo", token, ssoid);
        }

        public static void Pay(string orderId, int amount, string productDesc, string productName, string callbackUrl)
        {
            callSdkApi("Pay", orderId, amount, productDesc, productName, callbackUrl);
        }

        public static void ReportUserGameInfoData(string serverId, string roleName, string level)
        {
            callSdkApi("ReportUserGameInfoData", serverId, roleName, level);
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

    public class OPPOSDKManager : Manager
    {
        private bool mInitialized = false;
        private bool mLogined = false;

        private System.Action<string, bool> mInitCallback;
        private System.Action<string, object> mLoginCallback;
        private System.Action<string> mGetTokenAndSsoidCallback;
        private System.Action<int> mPayCallback;
        private System.Action<bool> mExitSDKCallback;

        private string mNotifyUrl = string.Empty;
        private string appId = string.Empty;
        private string appSecret = string.Empty;
        private string token = string.Empty;
        private string ssoid = string.Empty;

        public bool IsLoggedIn { get { return mLogined; } }
        public string Token { get { return token; } }
        public string Ssoid { get { return ssoid; } }

        public override void Initialize(Config config)
        {

        }

        public override void OnLoggedIn()
        {
            var user = Hub.LoginManager.LocalUser;
            ReportUserGameInfoData(user.WorldId.ToString(), user.Name, user.Level.ToString());
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
                        mPayCallback(OPPOStatusCode.PAY_TIMEOUT);
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


            if (mGetTokenAndSsoidCallback != null)
            {
                EB.Coroutines.SetTimeout(delegate ()
                {
                    if (mGetTokenAndSsoidCallback != null)
                    {
                        mGetTokenAndSsoidCallback(string.Empty);
                        mGetTokenAndSsoidCallback = null;
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

        public void InitSDK(object options, Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("OPPOSDKManager.InitializeSDK: Initialized");
                callback(null, true);
                return;
            }
            if (Application.platform != RuntimePlatform.Android)
            {
                callback(null, false);
                return;
            }
            mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);
            appId = EB.Dot.String("appId", options, appId);
            appSecret = EB.Dot.String("appSecret", options, appSecret);
            EB.Debug.Log("OPPOSDKManager.InitSDK notifyUrl = {0}", mNotifyUrl);
            EB.Debug.Log("OPPOSDKManager.InitSDK appId = {0}", appId);
            EB.Debug.Log("OPPOSDKManager.InitSDK appSecret = {0}", appSecret);
            mInitCallback += callback;
            Hub.RunInBackground = true;
            new GameObject("oppo_plugin_listener", typeof(SparxOPPOSDKManager));
            OPPOGameSDK.InitSDK(appId, appSecret);
        }

        public void OnInitCallback(int code)
        {
            Hub.RunInBackground = false;
            if (code == OPPOStatusCode.SUCCESS)
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
            EB.Debug.Log("OPPOSDKManager.Login");
            if (!mInitialized)
            {
                callback("OPPOSD has not been inited", null);
                return;
            }

            mLoginCallback += callback;

            Hub.RunInBackground = true;
            OPPOGameSDK.Login();
        }

        public void OnLoginCallback(int code)
        {
            EB.Debug.Log("OPPOSDKManager.OnLoginResult:code={0}", code);
            Hub.RunInBackground = false;
            if (code == OPPOStatusCode.SUCCESS)
            {
                if (mLoginCallback != null)
                {
                    //OPPOGameSDK.OnResume();
                    Hashtable data = new Hashtable();
                    GetTokenAndSsoid(delegate (string jsonString)
                    {
                        if (!string.IsNullOrEmpty(jsonString))
                        {
                            object json = EB.JSON.Parse(jsonString);
                            token = Dot.String("token", json, string.Empty);
                            ssoid = Dot.String("ssoid", json, string.Empty);
                            data.Add("token", token);
                            data.Add("ssoid", ssoid);
                            mLoginCallback(null, data);
                            mLoginCallback = null;
                        }
                        else
                        {
                            mLoginCallback(null, null);
                            mLoginCallback = null;
                        }
                    });
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

        public void GetTokenAndSsoid(Action<string> callback)
        {
            mGetTokenAndSsoidCallback = callback;
            OPPOGameSDK.GetTokenAndSsoid();
        }

        public void OnGetTokenAndSsoidCallback(int code, string jsonString)
        {
            if (code == OPPOStatusCode.SUCCESS)
            {
                if (mGetTokenAndSsoidCallback != null)
                {
                    mGetTokenAndSsoidCallback(jsonString);
                    mGetTokenAndSsoidCallback = null;
                }
            }
            else
            {
				if (mGetTokenAndSsoidCallback != null)
				{
					mGetTokenAndSsoidCallback(string.Empty);
					mGetTokenAndSsoidCallback = null;
				}
            }
        }

        public void Logout()
        {
            if (mLogined)
            {
                mLogined = false;
            }
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("QiHooSDKManager.Pay");
            if (!mInitialized)
            {
                EB.Debug.LogError("QiHooSDKManager.Pay: not initialized");
                callback(OPPOStatusCode.FAILURE);
                return;
            }

            mPayCallback += callback;
            var user = Hub.Instance.LoginManager.LocalUser;
            OPPOGameSDK.Pay(transaction.transactionId, item.cents, item.localizedDesc, item.longName, mNotifyUrl);
        }

        public void OnPayCallback(int code)
        {
            EB.Debug.Log("QiHooSDKManager.OnPayCallback: code = {0}", code);
            if (mPayCallback != null)
            {
                mPayCallback(code);
                mPayCallback = null;
            }
        }

        public void ReportUserGameInfoData(string serverId, string roleName, string level)
        {
            OPPOGameSDK.ReportUserGameInfoData(serverId, roleName, level);
        }

        public void ExitSDK(Action<bool> callback)
        {
            if (mInitialized)
            {
                Hub.RunInBackground = true;
                mExitSDKCallback += callback;
                OPPOGameSDK.ExitSDK();
            }
        }

        public void OnExitSDK(int code)
        {
            Hub.RunInBackground = false;
            if (mExitSDKCallback != null)
            {
                if (code == OPPOStatusCode.SUCCESS)
                {
                    mExitSDKCallback(true);
                }
                else
                {
                    mExitSDKCallback(false);
                }
            }
        }
    }

    public class SparxOPPOSDKManager : MonoBehaviour
    {

        void Awake()
        {
            EB.Debug.Log("Creating OPPO Manager");
            DontDestroyOnLoad(gameObject);
        }

        public void OnOPPOGameSDKCallback(string jsonString)
        {
            EB.Debug.Log("SparxOPPOSDKManager.OnOPPOGameSDKCallback:jsonString = {0}", jsonString);
            object json = EB.JSON.Parse(jsonString);
            string callbackType = EB.Dot.String("callbackType", json, string.Empty);
            int code = Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case OPPOSDKCallbackType.CALLBACKTYPE_INITSDK:
                    Sparx.Hub.Instance.OPPOSDKManager.OnInitCallback(code);
                    break;

                case OPPOSDKCallbackType.CALLBACKTYPE_LOGIN:
                    Sparx.Hub.Instance.OPPOSDKManager.OnLoginCallback(code);
                    break;

                case OPPOSDKCallbackType.CALLBACKTYPE_GETTOKENANDSSOID:
                    Sparx.Hub.Instance.OPPOSDKManager.OnGetTokenAndSsoidCallback(code, Dot.String("data", json, String.Empty));
                    break;

                case OPPOSDKCallbackType.CALLBACKTYPE_GETUSERINFO:
                    break;

                case OPPOSDKCallbackType.CALLBACKTYPE_PAY:
                    Sparx.Hub.Instance.OPPOSDKManager.OnPayCallback(code);
                    break;

                case OPPOSDKCallbackType.CALLBACKTYPE_REPORT:

                    break;

                case OPPOSDKCallbackType.CALLBACKTYPE_EXITSDK:
                    Sparx.Hub.Instance.OPPOSDKManager.OnExitSDK(code);
                    break;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                EB.Debug.Log("SparxOPPOSDKManager.OnApplicationPause.OnPause");
                OPPOGameSDK.OnPause();
            }
            else
            {
                EB.Debug.Log("SparxOPPOSDKManager.OnApplicationPause.OnResume");
                OPPOGameSDK.OnResume();
                SparxHub.Instance.OPPOSDKManager.OnLoginEnteredForeground();
            }
        }
    }
}
#endif
