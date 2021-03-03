// #if UNITY_ANDROID &&USE_ALIPAYSDK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EB.Sparx
{
    public class AlipaySDKResultCode
    {
        public const int Succ = 0;//成功code

        public const int Failed = 1;//失败code
    }

    public class AlipaySDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "Init";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";
    }

    public class AlipaySDK
    {
        private const string SDK_JAVA_CLASS = "cn.alipay.gamesdk.unity3d.AliPayGameSDK";

        public static void Init()
        {
            callSdkApi("Init");//支付宝初始化无需实现
        }

        public static void Login(string info)
        {
            callSdkApi("Login",info);
        }

        public static void Pay(string info)
        {
            int index = info.IndexOf('?');
            info = info.Substring(index+1);
            EB.Debug.Log("AlipaySDK.Pay => info = " + info);
            callSdkApi("Pay",info);
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

    public class AlipaySDKManager : Manager
    {
        private static System.Action<string, bool> mInitCallback = null;
        private System.Action<string, object> mAuthCallback = null;
        private System.Action<int> mPayCallback = null;
        private string mSid = string.Empty;
        private string mAid = string.Empty;

        private bool mInitialized = false;
        private bool mLoggedIn = false;
        public bool IsLoggedIn { get { return mLoggedIn; } }
        
        public override void Initialize(Config config) { }

        public void Init(Action<string, bool> callback)
        {
            EB.Debug.Log("AlipaySDKManager.Init");
            if (mInitialized)
            {
                EB.Debug.LogWarning("AlipaySDKManager.Init: Initialized");
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
            new GameObject("alipay_plugin_listener", typeof(SparxAlipaySDKManager));
            AlipaySDK.Init();
        }

        public void IAPInit()
        {
            EB.Debug.Log("AlipaySDKManager.IAPInit");
            new GameObject("alipay_plugin_listener", typeof(SparxAlipaySDKManager));
            mInitialized = true;
        }

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("AlipaySDKManager.Login");
            if (!mInitialized)
            {
                EB.Debug.LogError("AlipaySDKManager.Login: not initialized");
                callback("AlipaySDKManager not initialized", null);
                return;
            }
            if (mLoggedIn)
            {
                Hashtable data = new Hashtable()
                {
                        {"sessionid", mSid },
                        {"accountid", mAid }
                };
                callback(null, data);
                return;
            }
            mAuthCallback += callback;
            Hub.RunInBackground = true;
            AlipaySDK.Login(string.Empty);
        }

        public void Logout()
        {
            if (mLoggedIn)
            {
                mLoggedIn = false;
                mSid = string.Empty;
                mAid = string.Empty;
                Hub.Instance.Disconnect(true);
            }
            mLoggedIn = false;

            SparxHub.Instance.Disconnect(false);
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("AlipaySDKManager.Pay");
            if (!mInitialized)
            {
                //EB.Debug.LogError("AlipaySDKManager.Pay: not initialized");
                //callback(AlipaySDKResultCode.Failed);
                //return;
                IAPInit();
            }
            mPayCallback += callback;

            object extraInfo = EB.JSON.Parse(transaction.payload);
            string info = EB.Dot.String("ret", extraInfo, string.Empty);

            int je = (int)item.cost;
            string mark = transaction.transactionId;
            string name = item.longName;
            EB.Debug.Log("AlipaySDKManager.Pay: je = {0}, mark={1}, name = {2}", je, mark, name);
            AlipaySDK.Pay(info);
        }

        public void OnInitResult(bool success, string msg)
        {
           EB.Debug.Log(string.Format("OnInitResult: success={0}, msg={1}", success, msg));
            Hub.RunInBackground = false;
            if (success)
            {
                mInitialized = true;
            }

            if (mInitCallback != null)
            {
                mInitCallback(null, mInitialized);
                mInitCallback = null;
            }
        }

        public void OnLoginResult(int code, object msg)
        {
            EB.Debug.Log("OnLoginResult: code={0}, msg={1}", code, msg);
            Hub.RunInBackground = false;
            if (code == AlipaySDKResultCode.Succ)
            {
                if (mAuthCallback != null)
                {
                    string uid = EB.Dot.String("code", msg, string.Empty);

                    Hashtable auth = new Hashtable();
                    auth["code"] = uid;

                    mAuthCallback(null, auth);
                    mAuthCallback = null;
                }
            }
            else
            {
                if (mAuthCallback != null)
                {
                    mAuthCallback("login failed from sdk", null);
                    mAuthCallback = null;
                }
            }
        }

        public void OnPayResult(int code)
        {
            EB.Debug.Log("OnPayResult: code={0}", code);
            if (mPayCallback != null)
            {
                mPayCallback(code);
                mPayCallback = null;
            }
        }
    }
}

public class SparxAlipaySDKManager : MonoBehaviour
{
    void Awake()
    {
        EB.Debug.Log("Creating SparxAlipaySDKManager");
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        EB.Debug.Log("SparxAlipaySDKManager.OnApplicationPause: status = {0}", pauseStatus);
    }


    public void OnAlipayGameSDKCallback(string jsonString)
    {
        UnityEngine.Debug.Log(string.Format("SparxAlipaySDKManager.OnAlipayGameSDKCallback:jsonString = {0}", jsonString));
        object json = EB.JSON.Parse(jsonString);
        string callbackType = EB.Dot.String("callbackType", json, string.Empty);
        int code = EB.Dot.Integer("code", json, -1);
        switch (callbackType)
        {
            case EB.Sparx.AlipaySDKCallbackType.CALLBACKTYPE_INITSDK:
                EB.Sparx.Hub.Instance.AlipaySDKManager.OnInitResult(code == 0, null);
                break;

            case EB.Sparx.AlipaySDKCallbackType.CALLBACKTYPE_LOGIN:
                EB.Sparx.Hub.Instance.AlipaySDKManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
                break;

            case EB.Sparx.AlipaySDKCallbackType.CALLBACKTYPE_PAY:
                EB.Sparx.Hub.Instance.AlipaySDKManager.OnPayResult(code);
                break;
            default:
                EB.Debug.LogError("NOT FIND! OnAlipayGameSDKCallback.callbackType = " + callbackType);
                break;
        }
    }
}

// #endif
