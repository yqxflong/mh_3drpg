#if UNITY_ANDROID && USE_M4399SDK
using EB;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public class M4399ResultCode
    {
        public const int Succ = 0;

        public const int Failed = 1;

        public const int Cancel = 2;
    }

    public class M4399SDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_LOGOUT = "Logout";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_QUIT = "Quit";
    }

    public class M4399SDK
    {
        private const string SDK_JAVA_CLASS = "cn.m4399.gamesdk.unity3d.M4399GameSDK";

        public static void InitSDK()
        {
            callSdkApi("Init");
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void SetServer(int serverId)
        {
            callSdkApi("SetServer", serverId);
        }

        public static void Logout()
        {
            callSdkApi("Logout");
        }

        public static void Pay(int cost, string tranId, string productName)
        {
            callSdkApi("Pay", cost, tranId, productName);
        }

        public static void Quit()
        {
            callSdkApi("Quit");
        }

        public static void OnDestory()
        {
            callSdkApi("OnDestory");
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

    public class M4399SDKManager : Manager
    {
        public bool IsLoggedIn { get { return mLogined; } }

        private bool mInitialized = false;
        private Action<string, bool> mInitCallback;

        public override void Initialize(Config config)
        {

        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            M4399SDK.SetServer(LoginManager.Instance.LocalUser.WorldId);
        }

        public void InitSDK(Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("M4399SDKManager.InitializeSDK: Initialized");
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
            new GameObject("m4399_plugin_listener", typeof(SparxM4399SDKManager));
            M4399SDK.InitSDK();
        }

        public void OnInitCallback(int code)
        {
            Hub.RunInBackground = false;
            if (code == M4399ResultCode.Succ)
            {
                mInitialized = true;
            }

            if (mInitCallback != null)
            {
                mInitCallback(null, mInitialized);
                mInitCallback = null;
            }
        }

        private bool mLogined = false;
        private System.Action<string, object> mLoginCallback;

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

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("M4399SDKManager.Login");
            if (!mInitialized)
            {
                callback("M4399SDK has not been inited", null);
                return;
            }

            mLoginCallback += callback;
            Hub.RunInBackground = true;
            M4399SDK.Login();
        }

        public void OnLoginCallback(int code, object data)
        {
            EB.Debug.Log("M4399SDKManager.OnLoginResult:code = {0}", code);
            Hub.RunInBackground = false;
            if (code == M4399ResultCode.Succ)
            {
                if (mLoginCallback != null)
                {
                    if (mLoginCallback != null)
                    {
                        string state = EB.Dot.String("state", data, string.Empty);
                        string uid = EB.Dot.String("uid", data, string.Empty);

                        Hashtable auth = new Hashtable();
                        auth["state"] = state;
                        auth["uid"] = uid;

                        mLoginCallback(null, auth);
                        mLoginCallback = null;
                    }
                }
            }
            else
            {
                if (mLoginCallback != null)
                {
                    mLoginCallback("login failed from sdk", null);
                    mLoginCallback = null;
                }
            }
        }

        public void Logout()
        {             
            M4399SDK.Logout();
        }

        private bool mIsFromSDKLogout = false;

        public void OnLogoutCallback(int code)
        {
            if (code == M4399ResultCode.Succ)
            {
                SparxHub.Instance.Disconnect(false);
            }
        }

        private System.Action<int> mPayCallback;

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("M4399SDKManager.Pay");
            if (!mInitialized)
            {
                EB.Debug.LogError("M4399SDKManager.Pay: not initialized");
                callback(M4399ResultCode.Failed);
                return;
            }

            mPayCallback += callback;
            var user = Hub.Instance.LoginManager.LocalUser;
            int je = (int)item.cost;
            string mark = transaction.transactionId;
            string name = item.longName;
            EB.Debug.Log("M4399SDKManager.Pay: je = {0}, mark={1}, name = {2}", je, mark, name);
            M4399SDK.Pay((int)item.cost, transaction.transactionId, item.longName);
        }

        public void OnPayCallback(int code, object data)
        {
            EB.Debug.Log("M4399SDKManager.OnPayCallback: code = {0}", code);
            if (mPayCallback != null)
            {
                EB.Coroutines.SetTimeout(delegate ()
                {
                    mPayCallback(code);
                    mPayCallback = null;
                }, 1 * 1000);
            }
        }

        private System.Action<bool> mExitSDKCallback;

        public void Quit(Action<bool> callback)
        {
            if (mInitialized)
            {
                Hub.RunInBackground = true;
                mExitSDKCallback += callback;
                M4399SDK.Quit();
            }
        }
        public void OnQuitCallback(int code)
        {
            Hub.RunInBackground = false;
            if (mExitSDKCallback != null)
            {
                if (code == M4399ResultCode.Succ)
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
}


public class SparxM4399SDKManager : MonoBehaviour
{

    public void OnM4399GameSDKCallback(string jsonString)
    {
        EB.Debug.Log("SparxM4399SDKManager.OnM4399GameSDKCallback:jsonString = {0}", jsonString);
        object json = EB.JSON.Parse(jsonString);
        string callbackType = Dot.String("callbackType", json, string.Empty);
        int code = Dot.Integer("code", json, -1);
        switch (callbackType)
        {
            case M4399SDKCallbackType.CALLBACKTYPE_INITSDK:
                Hub.Instance.M4399SDKManager.OnInitCallback(code);
                break;

            case M4399SDKCallbackType.CALLBACKTYPE_LOGIN:
                Hub.Instance.M4399SDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
                break;

            case M4399SDKCallbackType.CALLBACKTYPE_LOGOUT:
                Hub.Instance.M4399SDKManager.OnLogoutCallback(code);
                break;

            case M4399SDKCallbackType.CALLBACKTYPE_PAY:
                Hub.Instance.M4399SDKManager.OnPayCallback(code, Dot.Object("data", json, null));
                break;

            case M4399SDKCallbackType.CALLBACKTYPE_QUIT:
                Hub.Instance.M4399SDKManager.OnQuitCallback(code);
                break;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        EB.Debug.Log("SparxM4399SDKManager.OnApplicationPause: status = {0}", pauseStatus);
        if (!pauseStatus)
        {
            Hub.Instance.M4399SDKManager.OnLoginEnteredForeground();
        }
    }

    private void OnApplicationQuit()
    {
        M4399SDK.OnDestory();
    }
}

#endif
