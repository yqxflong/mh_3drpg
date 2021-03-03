#if USE_AIBEISDK
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace EB.Sparx
{
    public class AibeiConfig
    {
        public const int ORIENTATION_PORTRAIT = 0;//竖屏

        public const int ORIENTATION_LANDSCAPE = 1;//横屏

        public const int LOGIN_SUCCESS = 0;

        public const int LOGIN_FAIL = 1;

        public const int USER_EXIT = 2;

        public const int PAY_SUCCESS = 0;

        public const int PAY_CANCEL = 2;

        public const int PAY_ERROR = 3;

        public const int PAY_NO_RESULT = 4;

        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";
    }

    public class AibeiSDK
    {
#if UNITY_IPHONE

        [DllImport("__Internal")]
        private static extern void InitSDK(string appid);

        [DllImport("__Internal")]
        private static extern void Login();

        [DllImport("__Internal")]
        private static extern void Logout();

        [DllImport("__Internal")]
        private static extern void Pay(string url);
         
        [DllImport("__Internal")]
        private static extern string GetAppid();

#endif

#if UNITY_ANDROID
        private const string SDK_JAVA_CLASS = "cn.aibei.gamesdk.unity3d.AibeiGameSDK";

        public static void InitSDK(int screenType, string appid)
        {
            callSdkApi("InitSDK", screenType, appid);
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void Logout()
        {
            callSdkApi("Logout");
        }

        public static void Pay(string transid, string appId)
        {
            callSdkApi("Pay", transid, appId);
        }

        public static string GetAppid()
        {
            return string.Empty;
        }

        private static void callSdkApi(string apiName, params object[] args)
        {
            using (AndroidJavaClass cls = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                cls.CallStatic(apiName, args);
            }
        }

#endif

        public static void pxInitSDK(int screenType, string appid)
        {

#if UNITY_IPHONE
			InitSDK(appid);
#elif UNITY_ANDROID
            InitSDK(screenType, appid);
#endif
        }

        public static void pxLogin()
        {
            Login();
        }

        public static void pxLogout()
        {
            Logout();
        }

        public static void pxPay(string transid, string appId)
        {
            EB.Debug.Log("AibeiSDKManager.pxPay: transid = {0},  appId = {1}", transid, appId);
            Pay(transid, appId);
        }

        public static string pxGetAppid()
        {
            return GetAppid();
        }
    }

    public class AibeiSDKManager : Manager
    {
        private static bool mInitialized = false;
        private static System.Action<string, bool> mInitCallback = null;
        private string appId = "3022219862";
        public string AppId
        {
            get
            {
                return appId;
            }
        }

        private bool mLoggedIn = false;
        private System.Action<string, object> mLoginCallback = null;

        //private System.Action<bool> mLogoutCallback = null;

        private System.Action<int> mPayCallback = null;

        public bool IsLoggedIn
        {
            get
            {
                return mLoggedIn;
            }
        }

        public override void Initialize(Config config)
        {

        }

        private object payCallbackHandle; //用于记录支付回调的计时器
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
                Hub.Instance.WalletManager.ShowOrHideLoadingUI(true);
                payCallbackHandle = EB.Coroutines.SetTimeout(delegate ()
                {
                    OnPayCallback(AibeiConfig.PAY_ERROR);
                }, 3 * 1000);
            }
        }

        public void OnLoginEnteredForeground()
        {
            if (mLoggedIn) return;
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

        public void InitSDK(object options, System.Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("AibeiSDKManager.InitializeSDK: Initialized");
                callback(null, true);
                return;
            }


            //appId = EB.Dot.String("appId", options, null);
			mInitCallback += callback;
            //appId = AibeiSDK.pxGetAppid();
            //appId = EB.Dot.String("appId", options, string.Empty);
            EB.Debug.Log("AibeiSDKManager.InitSDK: appId = {0}", appId);

            Hub.RunInBackground = true;

            new GameObject("aibei_plugin_listener", typeof(SparxAibeiSDKManager));

            AibeiSDK.pxInitSDK(AibeiConfig.ORIENTATION_LANDSCAPE, appId);
        }

        public void OnInitSDKCallback()
        {
            Hub.RunInBackground = false;
            mInitialized = true;

            if (mInitCallback != null)
            {
                mInitCallback(null, mInitialized);
                mInitCallback = null;
            }
        }

        public void Login(System.Action<string, object> callback)
        {
            if (mLoggedIn)
            {
                callback(null, null);
                return;
            }

            mLoginCallback = callback;
            Hub.RunInBackground = true;
            AibeiSDK.pxLogin();
        }

        public void OnLoginCallback(int code, object data)
        {
            Hub.RunInBackground = false;
            switch (code)
            {
                case AibeiConfig.LOGIN_SUCCESS:
                    if (!mLoggedIn)
                    {
                        mLoggedIn = true;
                        if (mLoginCallback != null)
                        {
                            Hashtable param = new Hashtable
                            {
                                {"appId",appId},
                                {"userId",EB.Dot.String("userID",data,string.Empty)},
                                {"loginName",EB.Dot.String("loginName",data,string.Empty)},
                                {"loginToken",EB.Dot.String("loginToken",data,string.Empty)}
                            };
                            mLoginCallback(null, param);
                        }
                    }
                    break;
                case AibeiConfig.LOGIN_FAIL:
                    if (mLoginCallback != null)
                    {
                        mLoginCallback(null, null);
                    }
                    break;
                case AibeiConfig.USER_EXIT:
                    if (mLoggedIn)
                    {
                        mLoggedIn = false;
                        Hub.Instance.Disconnect(true);
                    }
                    break;
            }
        }

        public void Logout()
        {
            if (mLoggedIn)
            {
                mLoggedIn = false;
                AibeiSDK.pxLogout();
            }
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            if (!mInitialized)
            {
                EB.Debug.LogError("AibeiSDKManager.Pay: mInitialized = {0}", mInitialized);
                callback(AibeiConfig.PAY_ERROR);
                return;
            }

            mPayCallback = callback;

            object extraInfo = EB.JSON.Parse(transaction.payload);
            string tid = EB.Dot.String("tid", extraInfo, string.Empty);
            string transid = transaction.transactionId;
            EB.Debug.Log("AibeiSDKManager.Pay: tid = {0}", tid);
            EB.Debug.Log("AibeiSDKManager.Pay: transid = {0}", transid);
            EB.Debug.Log("AibeiSDKManager.Pay: appId = {0}", appId);
            if (!string.IsNullOrEmpty(appId))
            {
                AibeiSDK.pxPay(tid, appId);

                //EB.Coroutines.SetTimeout(delegate ()
                //{
                //    OnPayCallback(AibeiConfig.PAY_NO_RESULT);
                //}, 3 * 1000);
            }
        }

        public void OnPayCallback(int code)
        {
            if (payCallbackHandle != null)
            {
                Hub.Instance.WalletManager.ShowOrHideLoadingUI(false);
                Coroutines.ClearTimeout(payCallbackHandle);
                payCallbackHandle = null;
            }

            if (mPayCallback != null)
            {
                mPayCallback(code);
                mPayCallback = null;
            }
        }
    }

    public class SparxAibeiSDKManager : MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating SparxAibeiSDKManager");
            DontDestroyOnLoad(gameObject);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxAibeiSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus && SparxHub.Instance != null)
            {
                SparxHub.Instance.AibeiSDKManager.OnLoginEnteredForeground();
            }
        }

        public void OnAibeiGameSDKCallback(string jsonString)
        {
            EB.Debug.Log("SparxAibeiSDKManager.OnAibeiGameSDKCallback:jsonString = {0}", jsonString);
            object json = EB.JSON.Parse(jsonString);
            string callbackType = Dot.String("callbackType", json, string.Empty);
            int code = Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case AibeiConfig.CALLBACKTYPE_INITSDK:
                    Sparx.Hub.Instance.AibeiSDKManager.OnInitSDKCallback();
                    break;

                case AibeiConfig.CALLBACKTYPE_LOGIN:
                    Sparx.Hub.Instance.AibeiSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
                    break;

                case AibeiConfig.CALLBACKTYPE_PAY:
                    Sparx.Hub.Instance.AibeiSDKManager.OnPayCallback(code);
                    break;
            }
        }
    }
}
#endif