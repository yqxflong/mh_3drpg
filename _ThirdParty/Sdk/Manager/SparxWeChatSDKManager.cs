// #if UNITY_ANDROID &&USE_WECHATSDK

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EB.Sparx
{
    public class WeChatSDKResultCode
    {
        public const int Succ = 0;//登录或支付成功code

        public const int PayFailed = -1;//支付失败code
        
        public const int Cancel = -2;//登录或支付取消code

        public const int LoginReject = -4;//登录拒绝授权code

        public const int WechatInstalledFailed = -8;//自定义错误码，微信未安装
    }

    public class WeChatSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "Init";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";
    }

    public class WeChatSDK
    {
        private const string SDK_JAVA_CLASS = "com.losttemple.mh.WeChatGameSDK";

        public static void Init()
        {
            callSdkApi("Init");
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void Pay(string partnerId, string prepayId, string packageValue, string nonceStr, string timeStamp, string sign)
        {
            callSdkApi("Pay", partnerId, prepayId, packageValue, nonceStr, timeStamp, sign);
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

    public class WeChatSDKManager : Manager
    {
        private System.Action<string, bool> mInitCallback = null;
        private System.Action<string> mTipCallback = null;
        private System.Action<string, object> mAuthCallback = null;
        private System.Action<int> mPayCallback = null;
        private string mSid = string.Empty;
        private string mAid = string.Empty;

        private bool mInitialized = false;
        private bool mLoggedIn = false;
        public bool IsLoggedIn { get { return mLoggedIn; } }


        public void BackFromWX()
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
                }, 1500);
            }
        }

        public override void Initialize(Config config){}

        public void Init(Action<string, bool> callback, Action<string> tipCallBack)
        {
            EB.Debug.Log("WeChatSDKManager.Init");
            if (mInitialized)
            {
                EB.Debug.LogWarning("WeChatSDKManager.Init: Initialized");
                callback(null, true);
                return;
            }
            if (Application.platform != RuntimePlatform.Android)
            {
                callback(null, false);
                return;
            }
            mInitCallback += callback;
            mTipCallback += tipCallBack;
            Hub.RunInBackground = true;
            new GameObject("WeChat_listener", typeof(SparxWeChatSDKManager));
            WeChatSDK.Init();
        }

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("WeChatSDKManager.Login");
            if (!mInitialized)
            {
                EB.Debug.LogError("WeChatSDKManager.Login: not initialized");
                callback("WeChatSDKManager not initialized",null);
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
            WeChatSDK.Login();
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
            EB.Debug.Log("WeChatSDKManager.Pay");
            if (!mInitialized)
            {
                EB.Debug.LogError("WeChatSDKManager.Pay: not initialized");
                callback(WeChatSDKResultCode.PayFailed);
                return;
            }
            mPayCallback += callback;

            object extraInfo = EB.JSON.Parse(transaction.payload);
            string partnerid = EB.Dot.String("partnerid", extraInfo, string.Empty);
            string prepayid = EB.Dot.String("prepayid", extraInfo, string.Empty);
            string package = EB.Dot.String("package", extraInfo, string.Empty);
            string noncestr = EB.Dot.String("noncestr", extraInfo, string.Empty);
            string timestamp = EB.Dot.String("timestamp", extraInfo, string.Empty);
            string sign = EB.Dot.String("sign", extraInfo, string.Empty);

            int je = (int)item.cost;
            string mark = transaction.transactionId;
            string name = item.longName;
            EB.Debug.Log("WeChatSDKManager.Pay: je = {0}, mark={1}, name = {2}", je, mark, name);
            WeChatSDK.Pay(partnerid, prepayid, package, noncestr, timestamp, sign);
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
            if (code == WeChatSDKResultCode.Succ)
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
            else if (code == WeChatSDKResultCode.Cancel)
            {
                if (mAuthCallback != null)
                {
                    mAuthCallback = null;
                }
                EB.Debug.LogWarning("WeChatSDKResultCode.LoginCancel!");
                SparxHub.Instance.Disconnect(true);
            }
            else if (code == WeChatSDKResultCode.LoginReject)
            {
                if (mAuthCallback != null)
                {
                    mAuthCallback = null;
                }
                EB.Debug.LogWarning("WeChatSDKResultCode.LoginReject!");
                SparxHub.Instance.Disconnect(true);
            }
            else if (code == WeChatSDKResultCode.WechatInstalledFailed)
            {
                OnShowTipCall("ID_WECHAT_NOT_FIND");
                if (mAuthCallback != null)
                {
                    mAuthCallback = null;
                }
                EB.Debug.LogWarning("WeChatSDKResultCode.WechatInstalledFailed!");
                SparxHub.Instance.Disconnect(true);
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

        public void OnShowTipCall(string tip)
        {
            if(mTipCallback != null)
            {
                mTipCallback(tip);
            }
        }
    }
}

public class SparxWeChatSDKManager : MonoBehaviour
{
    void Awake()
    {
        EB.Debug.Log("Creating SparxWeChatSDKManager");
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        EB.Debug.Log("SparxWeChatSDKManager.OnApplicationPause: status = {0}", pauseStatus);
        if (pauseStatus && SparxHub.Instance != null)
        {
            SparxHub.Instance.WeChatSDKManager.BackFromWX();
        }
    }


    public void OnWeChatCallback(string jsonString)
    {
        UnityEngine.Debug.Log(string.Format("SparxWeChatSDKManager.OnWeChatCallback:jsonString = {0}", jsonString));
        object json = EB.JSON.Parse(jsonString);
        string callbackType = EB.Dot.String("callbackType", json, string.Empty);
        int code = EB.Dot.Integer("code", json, -1);
        switch (callbackType)
        {
            case EB.Sparx.WeChatSDKCallbackType.CALLBACKTYPE_INITSDK:
                EB.Sparx.Hub.Instance.WeChatSDKManager.OnInitResult(code == 0, null);
                break;

            case EB.Sparx.WeChatSDKCallbackType.CALLBACKTYPE_LOGIN:
                EB.Sparx.Hub.Instance.WeChatSDKManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
                break;

            case EB.Sparx.WeChatSDKCallbackType.CALLBACKTYPE_PAY:
                EB.Sparx.Hub.Instance.WeChatSDKManager.OnPayResult(code);
                break;
            default:
                EB.Debug.LogError("NOT FIND! OnWeChatCallback.callbackType = " + callbackType);
                break;
        }
    }
}
// #endif 
