#if USE_ASDK && UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
    public class ASdkConfig
    {
        public const int ORIENTATION_PORTRAIT = 0;//竖屏

        public const int ORIENTATION_LANDSCAPE = 1;//横屏


        public const int LOGIN_SUCCESS = 0;

        public const int LOGIN_FAIL = 1;

        public const int LOGIN_OUT = 2;

        public const int USER_EXIT = 2;


        public const int PAY_SUCCESS = 0;

        public const int PAY_CANCEL = 2;

        public const int PAY_ERROR = 3;

        public const int PAY_NO_RESULT = 4;


        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_EXIT = "Exit";
    }

    /*public class ASdkParamInfo
    {
        public string CPID { get; set; }

        public string GameID { get; set; }

        public string GameKey { get; set; }

        public string GameName { get; set; }

        public ASdkParamInfo(string cpid,string gameid,string gamekey, string gamename)
        {
            CPID = cpid;
            GameID=gameid;
            GameKey=gameid;
            GameName = gamename;
        }
    }*/

    public class ASdkUserInfo
    {
        public string ingot { get; set; }
        public string playerId { get; set; }
        public string factionName { get; set; }
        public string vipLevel { get; set; }
        public string serverName { get; set; }
        public string playerLevel { get; set; }
        public string serverId { get; set; }
        public string playerName { get; set; }
        public string campId { get; set; }
        public string roleSex { get; set; }
        public string careerId { get; set; }
        public string experience { get; set; }
        public string coin { get; set; }
        public string payment { get; set; }

        //public string roleCTime { get; set; }
        //public string sceneValue { get; set; }
    }

    public class AStatusCode
    {
        // 调用成功
        public const int SUCCESS = 0;

        // 调用失败
        public const int FAIL = 1;

        //审核中
        public const int CHEAKING = 2;
        
    }

    /// <summary>
    /// SDK调用入口类
    /// </summary>
    public class ASdk : MonoBehaviour
    {
        /// <summary>
        /// 调用android java端接口类
        /// </summary>
        private const string SDK_JAVA_CLASS = "cn.asdk.gamesdk.unity3d.AYGameSDK";

        /// <summary>
        /// 注册加载
        /// </summary>
        /// <param name="info"></param>
        public static void initSDK()
        {
            callSdkApi("initSDK");//横竖屏在sdk里直接写死配置了
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        public static void login()
        {
            callSdkApi("login");
        }

        /// <summary>
        /// 支付
        /// </summary>
        public static void pay(string customorderid, string url, string sum, string desc, string callBackData)
        {
            callSdkApi("pay", customorderid, url, sum, desc, callBackData);
        }


        public static void setRoLeData(string RoLeData)
        {
            callSdkApi("setRoLeData", RoLeData);
        }


        /// <summary>
        /// 退出账号
        /// </summary>
        public static void loginOut()
        {
            callSdkApi("loginOut");
        }

        /// <summary>
        /// 退出SDK
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

    }

    public class ASDKManager : Manager
    {
        private static bool mInitialized = false;
        private static bool mInitializing = false;
        private static System.Action<string, bool> mInitCallback = null;
        private System.Action<string, object> mAuthCallback = null;
        private System.Action<int, object> mPayCallback = null;
        private System.Action<bool> mExitCallback = null;

        //游戏数据存于客户端
        //private static ASdkParamInfo Info = new ASdkParamInfo("100079", "100861", "edfe59740f28b1f5", "slzc");

        private bool mLoggedIn = false;
        private string mSid = string.Empty;
        private string mAid = string.Empty;

        public bool IsLoggedIn { get { return mLoggedIn; } }

        public string Sid { get { return mSid; } }

        private string mNotifyUrl = string.Empty;

        public override void Initialize(Config config)
        {

        }
        public override void OnLoggedIn()
        {
            var user = Hub.LoginManager.LocalUser;
            Hashtable loginGameRole = new Hashtable();
            loginGameRole["roleId"] = user.Id.Value.ToString();
            loginGameRole["roleName"] = user.Name;
            loginGameRole["zoneId"] = user.WorldId.ToString();
            loginGameRole["zoneName"] = string.Empty;
            loginGameRole["roleCTime"] = 0L;
            loginGameRole["roleLevel"] = user.Level.ToString();
            loginGameRole["roleLevelMTime"] = null;

            //ASdk.setRoLeData(EB.JSON.Stringify(loginGameRole));
        }
        /*public static void InitializeSDK(System.Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("ASDKManager.InitializeSDK: Initialized");
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

                SparxASDKManager.InitCallback += delegate (bool sucess, string msg)
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

                new GameObject("asdk_plugin_listener", typeof(SparxASDKManager));

                ASdk.initSDK(Info);
            }
        }*/

        public void InitializeSDK(object options, System.Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("ASDKManager.InitializeSDK: Initialized");
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

                new GameObject("asdk_plugin_listener", typeof(SparxASDKManager));

                ASdk.initSDK();
            }
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
                        mPayCallback(AStatusCode.CHEAKING, null);
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
                        {"sessionid", mSid },
                        { "accountid", mAid }
                };
                callback(null, data);
                return;
            }

            mAuthCallback += callback;

            Hub.RunInBackground = true;
            ASdk.login();
        }

        public void Logout()
        {
            if (mLoggedIn)
            {
                mLoggedIn = false;
                ASdk.loginOut();
            }
        }

        public void ExitSDK(System.Action<bool> callback)
        {
            if (mInitialized)
            {
                UnityEngine.Debug.Log("ASDKManager ExitSDK!!");
                mExitCallback += callback;
                ASdk.exitSDK();
            }
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int, object> callback)
        {
            if (!mInitialized)
            {
                Debug.LogError("ASDKManager.Pay: not initialized");
                callback(AStatusCode.FAIL, null);
                return;
            }

            mPayCallback += callback;

            var user = Hub.Instance.LoginManager.LocalUser;
            long uid = user.Id.Value;
            //int worldId = user.WorldId;
            string name = user.Name;
            int level = user.Level;

            //object extraInfo = EB.JSON.Parse(transaction.payload);
            //string tid = EB.Dot.String("tid", extraInfo, string.Empty);
            string transid = transaction.transactionId;
            //customorderid;订单
            //url回调地址
            //sum数额
            //desc描述
            //callBackData其他
            EB.Debug.Log(string.Format("！！！ASDKManager Pay = customorderid {0},url ={1}, cost ={2},desc={3}, callBackData={4}！！！", transid, mNotifyUrl, item.cost, item.category, null));
            ASdk.pay(transid, mNotifyUrl, item.cost.ToString(),item.category,"");
            
        }

       
        public void OnInitResult(bool success, string msg)
        {
           EB.Debug.Log(string.Format("OnInitResult: success={0}, msg={1}", success, msg));

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


        public void OnLoginResult(int code, object msg)
        {
            EB.Debug.Log("OnLoginResult: code={0}, msg={1}", code, msg);

            Hub.RunInBackground = false;

            if (code==ASdkConfig.LOGIN_SUCCESS)
            {
                mLoggedIn = true;
                mSid = EB.Dot.String("sessionid", msg, string.Empty);
                mAid = EB.Dot.String("accountid", msg, string.Empty);
                if (mAuthCallback != null)
                {
                    Hashtable data = new Hashtable()
                    {
                        {"sessionid", mSid },
                        { "accountid", mAid }
                    };
                    mAuthCallback(null, data);
                    mAuthCallback = null;
                }
            }
            else if(code == ASdkConfig.LOGIN_FAIL)
            {
               EB.Debug.Log("注销");
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

        public System.Action DefaultFunc=null;

        public void OnExitCallback()
        {
            UnityEngine.Debug.Log("OnExitCallback");

            if(DefaultFunc!=null) DefaultFunc();
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


public class SparxASDKManager : MonoBehaviour {
    public static System.Action<bool, string> InitCallback = null;
    private bool mOrderCreated = false;

    void Awake()
    {
        EB.Debug.Log("Creating SparxASDKManager");
        DontDestroyOnLoad(gameObject);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        EB.Debug.Log("SparxASDKManager.OnApplicationPause: status = {0}", pauseStatus);
        if (!pauseStatus && SparxHub.Instance != null)
        {
            SparxHub.Instance.ASDKManager.OnLoginEnteredForeground();
        }
    }

    /*public void onInitSucc(string emptyString)
    {
        if (SparxHub.Instance != null)
        {
            SparxHub.Instance.ASDKManager.OnInitResult(true, emptyString);
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
            SparxHub.Instance.ASDKManager.OnInitResult(false, data);
        }

        if (InitCallback != null)
        {
            InitCallback(false, data);
            InitCallback = null;
        }
    }*/
    
    
    public void OnASDKCallback(string jsonString)
    {
        UnityEngine.Debug.Log(string.Format("SparxASDKManager.OnASDKCallback:jsonString = {0}", jsonString));
        object json = EB.JSON.Parse(jsonString);
        string callbackType = EB.Dot.String("callbackType", json, string.Empty);
        int code = EB.Dot.Integer("code", json, -1);
        switch (callbackType)
        {
            case EB.Sparx.ASdkConfig.CALLBACKTYPE_INITSDK:
                EB.Sparx.Hub.Instance.ASDKManager.OnInitResult(code == 0, null);
                break;

            case EB.Sparx.ASdkConfig.CALLBACKTYPE_LOGIN:
                EB.Sparx.Hub.Instance.ASDKManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
                break;

            case EB.Sparx.ASdkConfig.CALLBACKTYPE_PAY:
                EB.Sparx.Hub.Instance.ASDKManager.OnPayCallback(code, json);
                break;
            case EB.Sparx.ASdkConfig.CALLBACKTYPE_EXIT:
                EB.Sparx.Hub.Instance.ASDKManager.OnExitCallback();
                break;
        }
    }
}
#endif
