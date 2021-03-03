#if UNITY_ANDROID && USE_TENCENTSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class TencentEFlag
    {
        //常量含义参见 http://wiki.open.qq.com/wiki/YSDK_Android_%E5%B8%B8%E9%87%8F%E6%9F%A5%E8%AF%A2
        public const int Succ = 0;

        public const int Error = -1;

        public const int QQ_UserCancel = 1001;

        public const int QQ_LoginFail = 1002;

        public const int QQ_NetworkErr = 1003;

        public const int QQ_NotInstall = 1004;

        public const int QQ_NotSupportApi = 1005;

        public const int WX_NotInstall = 2000;

        public const int WX_NotSupportApi = 2001;

        public const int WX_UserCancel = 2002;

        public const int WX_UserDeny = 2003;

        public const int WX_LoginFail = 2004;

        public const int Login_TokenInvalid = 3100;

        public const int Login_NotRegisterRealName = 3101;

        public const int Login_CheckingToken = 3102;

        public const int Relation_RelationNoPerson = 3201;

        public const int Wakeup_NeedUserLogin = 3301;

        public const int Wakeup_YSDKLogining = 3302;

        public const int Wakeup_NeedUserSelectAccount = 3303;

        public const int Pay_User_Cancle = 4001;

        public const int Pay_Param_Error = 4002;

        public const int Pay_Time_Out = 4003;
    }

    public class TencentPayState
    {
        public const int PAYSTATE_PAYUNKOWN = -1;

        public const int PAYSTATE_PAYSUCC = 0;

        public const int PAYSTATE_PAYCANCEL = 1;

        public const int PAYSTATE_PAYERROR = 2;
    }

    public class TencentPayRet
    {
        public const int RET_SUCC = 0;

        public const int RET_FAIL = 1;
    }

    public class TencentSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";

    }

    public class TencentSDK
    {
        private const string SDK_JAVA_CLASS = "cn.tencent.gamesdk.unity3d.TencentGameSDK";

        public static void InitSDK()
        {
            callSdkApi("InitSDK");
        }

        public static void Login(string platform)
        {
            callSdkApi("Login", platform);
        }

        public static string GetLoginRecord()
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                return androidJavaClass.CallStatic<string>("GetLoginRecord");
            }
        }

        public static void Logout()
        {
            callSdkApi("Logout");
        }

        public static void Pay(string zoneId,string saveValue)
        {
            callSdkApi("Pay", zoneId, saveValue);
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

    public class TencentSDKManager : Manager
    {
        public const string QQPlatform = "qq";

        public const string WXPlatform = "wx";

        private bool mInitialized = false;
        private Action<string, bool> mInitCallback;

        private bool mLogined = false;
        private string mPlatform = string.Empty;
        private System.Action<string, object> mLoginCallback;

        //private System.Action<object> mGetLoginRecordCallback;

        private System.Action<int, object> mPayCallback;

        private string zoneId = string.Empty;

        public bool IsLoggedIn { get { return mLogined; } }

        public string Platform { get { return mPlatform; } }

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
                        Hashtable data = new Hashtable();
                        data.Add("flag", TencentEFlag.Pay_Time_Out);
                        mPayCallback(TencentPayRet.RET_FAIL, data);
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

        public void InitSDK(object initData, Action<string, bool> callback)
        {

            if (mInitialized)
            {
                EB.Debug.LogWarning("TencentSDKManager.InitializeSDK: Initialized");
                callback(null, true);
                return;
            }
            if (Application.platform != RuntimePlatform.Android)
            {
                callback(null, false);
                return;
            }
            zoneId = EB.Dot.String("zoneId", initData, zoneId);
            mInitCallback += callback;
            Hub.RunInBackground = true;
            new GameObject("tencent_plugin_listener", typeof(SparxTencentSDKManager));
            TencentSDK.InitSDK();
        }

        public void OnInitSDKCallback(int code)
        {
            Hub.RunInBackground = false;
            if (code == TencentEFlag.Succ)
            {
                mInitialized = true;
            }

            if (mInitCallback != null)
            {
                mInitCallback(null, mInitialized);
                mInitCallback = null;
            }
            
        }

        public void Login(string platform, Action<string, object> callback)
        {
            EB.Debug.Log("TencentSDKManager.Login platform = {0}", platform);
            if (!mInitialized)
            {
                callback("TencentSDK has not been inited", null);
                return;
            }
            mPlatform = platform;
            mLoginCallback += callback;

            var loginRecordData = GetLoginRecord(mPlatform);
            if (loginRecordData != null)
            {
                string accessToken = Dot.String("accessToken", loginRecordData, string.Empty);
                string openId = Dot.String("openId", loginRecordData, string.Empty);
                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(openId))
                {
                    Hub.RunInBackground = true;
                    TencentSDK.Login(platform);
                }
                else
                {
                    Hashtable loginData = new Hashtable();
                    loginData.Add("pltform", TencentSDKManager.QQPlatform);
                    loginData.Add("accessToken", accessToken);
                    loginData.Add("openId", openId);
                    EB.Debug.Log("TencentSDKManager.Login loginData = {0}", JSON.Stringify(loginData));
                    mLoginCallback(null, loginData);
                    SavePlatform(mPlatform);
                    mLogined = true;
                }
            }
            else
            {
                Hub.RunInBackground = true;
                TencentSDK.Login(platform);
            }
        }

        public void OnLoginCallback(int code, object data)
        {
            EB.Debug.Log("TencentSDKManager.OnLoginResult:code={0}  data = {1}", code, data);
            Hub.RunInBackground = false;
            if (code == TencentEFlag.Succ)
            {
                if (data != null)
                {
                    mLogined = true;
                    SavePlatform(mPlatform);
                    string accessToken = Dot.String("accessToken", data, string.Empty);
                    string openId = Dot.String("openId", data, string.Empty);
                    Hashtable loginData = new Hashtable();
                    loginData.Add("pltform", mPlatform);
                    loginData.Add("accessToken", accessToken);
                    loginData.Add("openId", openId);
                    if (mLoginCallback != null)
                    {
                        mLoginCallback(null, loginData);
                        mLoginCallback = null;
                    }
                }
            }
            else if (code == TencentEFlag.QQ_UserCancel || code == TencentEFlag.WX_UserCancel)
            {
                if (mLoginCallback != null)
                {
                    mLoginCallback(null, null);
                    mLoginCallback = null;
                }
            }
            else
            {
                if (mLoginCallback != null)
                {
                    if (data != null)
                    {
                        if (code == TencentEFlag.WX_NotInstall)
                        {
                            Application.OpenURL("http://weixin.qq.com/");
                        }
                        else if(code == TencentEFlag.QQ_NotInstall)
                        {
                            Application.OpenURL("http://im.qq.com/immobile/android/");
                        }
                        string errMsg = Dot.String("errMsg", data, string.Empty);
                        if (mLoginCallback != null)
                        {
                            mLoginCallback(errMsg, null);
                            mLoginCallback = null;
                        }
                    }
                }
            }
        }

        public object GetLoginRecord(string platform)
        {
            EB.Debug.Log("TencentSDKManager.GetLoginRecord:platform = {0}", platform);
            mPlatform = platform;
            string jsonString = TencentSDK.GetLoginRecord();
            EB.Debug.Log("TencentSDKManager.GetLoginRecord:jsonString = {0}", jsonString);
            return JSON.Parse(jsonString);
        }

        /// <summary>
        /// 仅限支付使用
        /// </summary>
        /// <param name="callback"></param>
        public object GetLoginRecordWithoutPlatform()
        {
            EB.Debug.Log("TencentSDKManager.GetLoginRecordWithoutPlatform");
            string jsonString = TencentSDK.GetLoginRecord();
            EB.Debug.Log("TencentSDKManager.GetLoginRecord:jsonString = {0}", jsonString);
            return JSON.Parse(jsonString);
        }

        public void Logout()
        {
            EB.Debug.Log("TencentSDKManager.Logout");
            if (!mInitialized)
            {
                EB.Debug.LogWarning("TencentSDKManager.Logout TencentSDK has not been inited");
                return;
            }

            if (mLogined)
            {
                TencentSDK.Logout();
                mLogined = false;
                DeletePlatform();
            }
        }


        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, Action<int, object> callback)
        {
            EB.Debug.Log("TencentSDKManager.Pay");
            if (!mInitialized)
            {
                EB.Debug.LogError("TencentSDKManager.Pay: not initialized");
                var callbackData = new Hashtable();
                callbackData.Add("flag", TencentPayState.PAYSTATE_PAYERROR);
                callback(TencentPayRet.RET_FAIL, TencentPayState.PAYSTATE_PAYERROR);
                return;
            }

            var extraInfo = JSON.Parse(transaction.payload);
            int balance = EB.Dot.Integer("balance", extraInfo, 0);
            if (balance < item.cost)
            {
                mPayCallback += callback;
                TencentSDK.Pay(zoneId, (item.cost * 10).ToString());
            }
            else
            {
                var callbackData = new Hashtable();
                callbackData.Add("payState", TencentPayState.PAYSTATE_PAYSUCC);
                callback(TencentPayRet.RET_SUCC, callbackData);
            }
        }

        public void OnPayCallback(int code, object data)
        {
            EB.Debug.Log("TencentSDKManager.OnPayCallback: code = {0}", data);
            if (mPayCallback != null)
            {
                mPayCallback(code, data);
                mPayCallback = null;
            }
        }

        public string GetPlatform()
        {
            string lastPlatform = SecurePrefs.GetString("TencentPlatform", null);
            EB.Debug.Log("GetPlatform: lastPlatform = {0}", lastPlatform);
            return lastPlatform;
        }

        public void SavePlatform(string platform)
        {
            EB.Debug.Log("SavePlatform: platform = {0}", platform);
            SecurePrefs.SetString("TencentPlatform", platform);
        }

        public void DeletePlatform()
        {
            EB.Debug.Log("DeletePlatform");
            SecurePrefs.DeleteKey("TencentPlatform");
        }
    }

    public class SparxTencentSDKManager : MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating Tencent Manager");
            DontDestroyOnLoad(gameObject);
        }

        public void OnTencentGameSDKCallback(string jsonString)
        {
            EB.Debug.Log("SparxTencentSDKManager.OnTencentGameSDKCallback:jsonString = {0}", jsonString);
            object json = EB.JSON.Parse(jsonString);
            string callbackType = Dot.String("callbackType", json, string.Empty);
            int code = Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case TencentSDKCallbackType.CALLBACKTYPE_INITSDK:
                    Sparx.Hub.Instance.TencentSDKManager.OnInitSDKCallback(code);
                    break;

                case TencentSDKCallbackType.CALLBACKTYPE_LOGIN:
                    Sparx.Hub.Instance.TencentSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
                    break;

                case TencentSDKCallbackType.CALLBACKTYPE_PAY:
                    Sparx.Hub.Instance.TencentSDKManager.OnPayCallback(code, Dot.Object("data", json, null));
                    break;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxTencentSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus)
            {
                SparxHub.Instance.TencentSDKManager.OnLoginEnteredForeground();
            }
        }
    }
}
#endif