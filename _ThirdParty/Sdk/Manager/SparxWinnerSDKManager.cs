#if UNITY_ANDROID && USE_WINNERSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class WinnerSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_LOGOUT = "Logout";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_DESTORYSDK = "DestroySDK";
    }

    public class WinnerStatusCode
    {
        //支付成功
        public const int SUCCESS = 0;

        //支付失败
        public const int FAIL = 1;

        public const int PAY_TIMEOUT = 2;
    }

    public class WinnerGameSDK
    {
        private const string SDK_JAVA_CLASS = "cn.winner.gamesdk.unity3d.WinnerGameSDK";

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

        public static void Pay(string userId, string money, string serverid,string productName, string productdesc, string uri, string attach)
        {
            callSdkApi("Pay", userId, money, serverid,productName, productdesc, uri, attach);
        }

        public static void DestroySDK()
        {
            callSdkApi("DestroySDK");
        }

        public static void ShowFloatView()
        {
            callSdkApi("ShowFloatView");
        }

        public static void RemoveFloatView()
        {
            callSdkApi("RemoveFloatView");
        }

        private static void callSdkApi(string apiName, params object[] args)
        {
            log("Unity3D " + apiName + " calling...");

            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                androidJavaClass.CallStatic(apiName, args);
            }
        }

        private static void log(string msg)
        {
            EB.Debug.Log(msg);
        }
    }

    public class WinnerSDKManager : Manager
    {

        private bool mInitialized = false;
        private bool mInitializing = false;
        private bool mLogined = false;

        private string mNotifyUrl;
        private System.Action<string, bool> mInitCallback;
        private System.Action<string, object> mLoginCallback;
        private System.Action<int> mPayCallback;
        //private System.Action mDestroyCallback;
        private System.Action<bool> mExitCallback = null;
        public bool IsLoggedIn { get { return mLogined; } }

        public override void Initialize(Config config)
        {

        }

        public override void OnLoggedIn()
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
                        mPayCallback(WinnerStatusCode.PAY_TIMEOUT);
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

        public void InitSDK(object options, System.Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("WinnerSDKManager.InitializeSDK: Initialized");
                callback(null, true);
                return;
            }

            if (Application.platform != RuntimePlatform.Android)
            {
                callback(null, false);
                return;
            }
            mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);
            EB.Debug.Log("WinnerSDKManager.InitSDK notifyUrl= {0}", mNotifyUrl);
            mInitCallback += callback;
            if (!mInitializing)
            {
                mInitializing = true;

                Hub.RunInBackground = true;

                new GameObject("winner_plugin_listener", typeof(SparxWinnerSDKManager));
                WinnerGameSDK.InitSDK();
            }
        }

        public void OnInitResult()
        {
            EB.Debug.Log("WinneroSDK.OnInitResult");
			StartCoroutine(LazyInitCallBack());
        }

		public IEnumerator LazyInitCallBack()
		{
			yield return new WaitForSeconds(1);
			mInitializing = false;
			Hub.RunInBackground = false;
			mInitialized = true;
			if (mInitCallback != null)
			{
				mInitCallback(null, mInitialized);
				mInitCallback = null;
			}
			yield break;
		}

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("WinnerGameSDK.Login");
            if (!mInitialized)
            {
                callback("WinnerSDK has not been inited", null);
                return;
            }
            mLoginCallback += callback;
            Hub.RunInBackground = true;
            WinnerGameSDK.Login();
        }

        public void OnLoginResult(int code,object data)
        {
            EB.Debug.Log("WinnerGameSDK.OnLoginResult:code={0}", code);
            Hub.RunInBackground = false;
            if(code== WinnerStatusCode.SUCCESS)
            {
                mLogined = true;
                //WinnerGameSDK.ShowFloatView();
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
            else
            {
                EB.Debug.Log("WinnerGameSDK.OnLoginResult:msg={0}", data);
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
        }

        public void Logout()
        {
            if (mLogined)
            {
                EB.Debug.Log("WinnerGameSDK.Logout");
				mLogined = false;
				Hub.Instance.Disconnect(true);
				//WinnerGameSDK.Logout();
            }
        }

        public void OnLogout(int code ,string msg)
        {
            if (code == WinnerStatusCode.SUCCESS)
            {
                //WinnerGameSDK.RemoveFloatView();
                if (mLogined)
                {
                    mLogined = false;
                    Hub.Instance.Disconnect(true);
                }
            }
            else
            {
                EB.Debug.LogError("OnLogout: unknown error: code={0}, msg={1}", code, msg);
            }
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("WinnerSDKManager.Pay={0}");
            if (!mInitialized)
            {
                EB.Debug.LogError("WinnerSDKManager.Pay: not initialized");
                callback(WinnerStatusCode.FAIL);
                return;
            }
            var user = Hub.Instance.LoginManager.LocalUser;
            mPayCallback += callback;
            WinnerGameSDK.Pay(GetUserId(), item.cost.ToString(), user.WorldId.ToString(), item.longName, item.localizedDesc, mNotifyUrl, transaction.transactionId);
        }

        public void OnPayCallback(int code)
        {
            EB.Debug.Log("WinnerSDKManager.OnPayCallback: code = {0}", code);
            if (mPayCallback != null)
            {
                mPayCallback(code);
                mPayCallback = null;
            }
        }

        //public void DestroySDK()
        //{
        //    if (mInitialized)
        //    {
        //        mInitialized = false;
        //        mLogined = false;
        //        mNotifyUrl = null;
        //        mInitCallback = null;
        //        mLoginCallback = null;
        //        mPayCallback = null;
        //        WinnerGameSDK.DestroySDK();
        //    }
        //}

        //public void OnDestorySDK()
        //{
        //    EB.Debug.Log("WinnerSDKManager.OnDestorySDK");
        //    if (mDestroyCallback != null)
        //    {
        //        mDestroyCallback();
        //        mDestroyCallback = null;
        //    }
        //}

        private string GetUserId()
        {
            string guid = string.Empty;
            foreach (AuthData authData in Hub.Instance.LoginManager.Account.Authenticators)
            {
                if (authData.Authenticator != null)
                {
                    if (authData.Authenticator.Name == "winner")
                    {
                        guid = authData.Id;
                        break;
                    }
                }
            }
            return guid;
        }

        public void ExitSDK(System.Action<bool> callback)
        {
            if (mInitialized)
            {
                mInitialized = false;
                mLogined = false;
                mLoginCallback = null;
                mInitCallback = null;
                mPayCallback = null;
                mNotifyUrl = string.Empty;
                mExitCallback += callback;
                WinnerGameSDK.DestroySDK();
            }
        }

        public void OnExitSDK(int code, string msg)
        {
            EB.Debug.Log("OnExitSDK: code={0}, msg={1}", code, msg);

            if (mExitCallback != null)
            {
                mExitCallback(code == WinnerStatusCode.SUCCESS);
                mExitCallback = null;
            }
        }

        public void ShowFloatView()
        {
            if (mLogined)
            {
                WinnerGameSDK.ShowFloatView();
            }
        }

        public void RemoveFloatView()
        {
            if (mLogined)
            {
                WinnerGameSDK.RemoveFloatView();
            }
        }

    }

    public class SparxWinnerSDKManager : MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating Winner Manager");
            DontDestroyOnLoad(gameObject);
        }

        public void OnWinnerGameSdkCallback(string jsonstr)
        {
            EB.Debug.Log("OnWinnerGameSdkCallback message: jsonstr = {0}", jsonstr);

            object json = EB.JSON.Parse(jsonstr);
            string callbackType = EB.Dot.String("callbackType", json, string.Empty);

            switch (callbackType)
            {
                case EB.Sparx.WinnerSDKCallbackType.CALLBACKTYPE_INITSDK:
                    SparxHub.Instance.WinnerSDKManager.OnInitResult();
                    break;

                case EB.Sparx.WinnerSDKCallbackType.CALLBACKTYPE_LOGIN:
                    SparxHub.Instance.WinnerSDKManager.OnLoginResult(EB.Dot.Integer("code", json, 0), EB.Dot.Object("data", json, null));
                    break;

                case EB.Sparx.WinnerSDKCallbackType.CALLBACKTYPE_LOGOUT:
                    SparxHub.Instance.WinnerSDKManager.OnLogout(EB.Dot.Integer("code", json, 0), EB.Dot.String("data", json, null));
                    break;

                case EB.Sparx.WinnerSDKCallbackType.CALLBACKTYPE_PAY:
                    SparxHub.Instance.WinnerSDKManager.OnPayCallback(EB.Dot.Integer("code", json, 0));
                    break;

                case EB.Sparx.WinnerSDKCallbackType.CALLBACKTYPE_DESTORYSDK:
                    SparxHub.Instance.WinnerSDKManager.OnExitSDK(EB.Dot.Integer("code", json, 0), EB.Dot.String("data", json, null));
                    break;

                default:
                    EB.Debug.LogWarning("OnWinnerGameSdkCallback: unknown type = {0}", callbackType);
                    break;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxWinnerSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus)
            { 
                //SparxHub.Instance.WinnerSDKManager.ShowFloatView();
                SparxHub.Instance.WinnerSDKManager.OnLoginEnteredForeground();
            }
            else
            {
                //SparxHub.Instance.WinnerSDKManager.RemoveFloatView();
            }
        }
    }
}
#endif