#if USE_VFPKSDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace EB.Sparx
{
    public class VFPKSDKResultCode
    {
        public const int Succ = 0;//成功code

        public const int Failed = 1;//失败code

        public const int Cancel = 2;//取消code
    }

    public class VFPKSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "Init";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_LOGINOUT = "LoginOut";

        public const string CALLBACKTYPE_PAY = "Pay";
        
        public const string CALLBACKTYPE_EXIT = "Exit";
    }

    public class VFPKSDK
    {
#if UNITY_IPHONE
        [DllImport("__Internal")]
        public static extern void Init(string appid);

        [DllImport("__Internal")]
        public static extern void Login();

        [DllImport("__Internal")]
        public static extern void Logout();

        [DllImport("__Internal")]
        public static extern void Pay(string payInfo);

        [DllImport("__Internal")]
        public static extern void SetRoleData(string serverId, string serverName, string roleId, string roleName, string roleLevel, string roleCTime);

        [DllImport("__Internal")]
        public static extern void Exit();

        [DllImport("__Internal")]
        public static extern void joinQQGroup();
#endif

#if UNITY_ANDROID
        private const string SDK_JAVA_CLASS = "com.vfpk.mh.VFPKGameSDK";

        public static void Init()
        {
            callSdkApi("Init");
        }

        public static void Exit()
        {
            callSdkApi("Exit");
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void LoginOut()
        {
            callSdkApi("LoginOut");
        }

        public static void Pay(float rmb, string productname, string serverId, string charId, string cporderid, string callbackInfo)
        {
            callSdkApi("Pay", rmb, productname, serverId, charId, cporderid, callbackInfo);
        }

        public static void SetRoleData(string serverId, string serverName, string roleId, string roleName, string roleLevel, string roleCTime)
        {
            callSdkApi("SetRoleData", serverId, serverName, roleId, roleName, roleLevel, roleCTime);
        }

        public static void joinQQGroup(string key)
        {
            callSdkApi("joinQQGroup",key);
        }

        private static void callSdkApi(string apiName, params object[] args)
        {
            EB.Debug.Log("Unity3D " + apiName + " calling...");

            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                androidJavaClass.CallStatic(apiName, args);
            }
        }
#endif
    }

    public class VFPKSDKManager : Manager
    {
        private System.Action<string, bool> mInitCallback = null;
        private System.Action<string, object> mAuthCallback = null;
        private System.Action<int> mPayCallback = null;

        private System.Action<string> mTipCallback = null;

        private string mSid = string.Empty;
        private string mAid = string.Empty;

        private bool mInitialized = false;
        private bool mLoggedIn = false;
        public bool IsLoggedIn { get { return mLoggedIn; } }

        public override void Initialize(Config config) { }

        public void Init(Action<string, bool> callback, Action<string> tipCallBack)
        {
            EB.Debug.Log("VFPKSDKManager.Init");
            if (mInitialized)
            {
                EB.Debug.LogWarning("VFPKSDKManager.Init: Initialized");
                callback(null, true);
                return;
            }
            mInitCallback += callback;
            mTipCallback += tipCallBack;
            Hub.RunInBackground = true;
            new GameObject("VFPK_listener", typeof(SparxVFPKSDKManager));

#if UNITY_ANDROID
            VFPKSDK.Init();
#endif
#if UNITY_IPHONE
            VFPKSDK.Init("544");
#endif
        }

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("VFPKSDKManager.Login");
            if (!mInitialized)
            {
                EB.Debug.LogError("VFPKSDKManager.Login: not initialized");
                callback("VFPKSDKManager not initialized", null);
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
#if UNITY_ANDROID
            VFPKSDK.Login();
#endif
#if UNITY_IPHONE
            VFPKSDK.Login();
#endif
        }

        public void Logout()
        {
#if UNITY_ANDROID
            VFPKSDK.LoginOut();
#endif
#if UNITY_IPHONE
            VFPKSDK.Logout();
#endif
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("VFPKSDKManager.Pay");
            if (!mInitialized)
            {
                EB.Debug.LogError("VFPKSDKManager.Pay: not initialized");
                callback(VFPKSDKResultCode.Failed);
                return;
            }
            mPayCallback += callback;

            object extraInfo = EB.JSON.Parse(transaction.payload);
            float cost = item.cost;
            string name = item.longName;
            string serverId = string.Empty;
            var gameWorlds = Hub.Instance.LoginManager.GameWorlds;
            var gameWorld = System.Array.Find(gameWorlds, w => w.Default);
            if (gameWorld != null)
            {
                serverId = gameWorld.Id.ToString();
            }
            string charId = EB.Sparx.LoginManager.Instance.LocalUserId.Value.ToString();
            string cporderId = transaction.transactionId;

            string callbackInfo = EB.Dot.String("callbackInfo", extraInfo, string.Empty);

            EB.Debug.Log("VFPKSDKManager.Pay: cost = {0},name = {1}", cost, name);
#if UNITY_ANDROID
            VFPKSDK.Pay(cost, name, serverId, charId, cporderId, callbackInfo);
#endif

#if UNITY_IPHONE
            //rmb 充值金额 单位元
            //productID iTunes 苹果后台配置的内购物品的产品ID
            //name 商品名
            //charid 角色ID
            //serverid 服务器ID
            //info 扩展信息
            //cporderid 游戏商订单ID
            string payInfo = cost + ";"
                + transaction.productId + ";"
                + name + ";"
                + charId + ";"
                + serverId + ";"
                + callbackInfo + ";"
                + cporderId;
            Debug.LogError("支付内容:payInfo:" + payInfo);
            VFPKSDK.Pay(payInfo);
#endif
        }

        public void SetRoleData(string serverId, string serverName, string roleId, string roleName, string roleLevel, string roleCTime)
        {
            VFPKSDK.SetRoleData(serverId, serverName, roleId, roleName, roleLevel, roleCTime);
        }

        public void Exit()
        {
            if (mInitialized)
            {
                VFPKSDK.Exit();
            }
        }

        public void OnJoinQQGroup(string key)
        {
#if UNITY_ANDROID
            VFPKSDK.joinQQGroup(key);
#endif
#if UNITY_IPHONE
            VFPKSDK.joinQQGroup();
#endif
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
            if (code == VFPKSDKResultCode.Succ)
            {
                mLoggedIn = true;
                if (mAuthCallback != null)
                {
                    mAid = EB.Dot.String("uid", msg, string.Empty);
                    mSid = EB.Dot.String("sid", msg, string.Empty);
                    Hashtable auth = new Hashtable();
                    auth["uid"] = mAid;
                    auth["sid"] = mSid;

                    mAuthCallback(null, auth);
                    mAuthCallback = null;
                }
            }
            else if (code == VFPKSDKResultCode.Cancel || code == VFPKSDKResultCode.Failed)
            {
                if (mAuthCallback != null)
                {
                    mAuthCallback = null;
                }
                EB.Debug.LogWarning("VFPKSDKResultCode.LoginCancel!");
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

        public void OnLoginOutResult(int code)
        {
            EB.Debug.Log("OnLogout: code={0}", code);
            if (code == VFPKSDKResultCode.Succ)
            {
                mLoggedIn = false;
                mSid = string.Empty;
                mAid = string.Empty;
                Hub.Instance.Disconnect(true);
            }
        }

        public void OnExitResult(int code)
        {
            EB.Debug.Log("OnExit: code={0}", code);
        }

        public void OnShowTipCall(string tip)
        {
            if (mTipCallback != null)
            {
                mTipCallback(tip);
            }
        }
    }
}

public class SparxVFPKSDKManager : MonoBehaviour
{
    void Awake()
    {
        EB.Debug.Log("Creating SparxVFPKSDKManager");
        DontDestroyOnLoad(gameObject);
    }
    
    public void OnVFPKCallback(string jsonString)
    {
        EB.Debug.Log(string.Format("SparxVFPKSDKManager.OnVFPKCallback:jsonString = {0}", jsonString));
        object json = EB.JSON.Parse(jsonString);
        string callbackType = EB.Dot.String("callbackType", json, string.Empty);
        int code = EB.Dot.Integer("code", json, -1);
        switch (callbackType)
        {
            case EB.Sparx.VFPKSDKCallbackType.CALLBACKTYPE_INITSDK:
                EB.Sparx.Hub.Instance.VFPKSDKManager.OnInitResult(code == 0, null);
                break;

            case EB.Sparx.VFPKSDKCallbackType.CALLBACKTYPE_LOGIN:
                EB.Sparx.Hub.Instance.VFPKSDKManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
                break;

            case EB.Sparx.VFPKSDKCallbackType.CALLBACKTYPE_PAY:
                EB.Sparx.Hub.Instance.VFPKSDKManager.OnPayResult(code);
                break;
                
            case EB.Sparx.VFPKSDKCallbackType.CALLBACKTYPE_LOGINOUT:
                EB.Sparx.Hub.Instance.VFPKSDKManager.OnLoginOutResult(code);
                break;

            case EB.Sparx.VFPKSDKCallbackType.CALLBACKTYPE_EXIT:
                EB.Sparx.Hub.Instance.VFPKSDKManager.OnExitResult(code);
                break;

            default:
                EB.Debug.LogError("NOT FIND! OnVFPKCallback.callbackType = " + callbackType);
                break;
        }
    }

    /// <summary>1  
    /// SDK会回调-通知初始化完成
    /// </summary>
    void OnInitFinish(string contect)
    {
        EB.Sparx.Hub.Instance.VFPKSDKManager.OnInitResult(true, null);
    }

    /// <summary>
    /// SDK会回调-通知登入成功
    /// </summary>
    void OnLoginSuccessful(string contect)
    {
        string[] allStr = contect.Split(';');
        Hashtable data = new Hashtable();
        data.Add("uid", allStr[0]);
        data.Add("sid", allStr[1]);
        EB.Sparx.Hub.Instance.VFPKSDKManager.OnLoginResult(EB.Sparx.VFPKSDKResultCode.Succ, data);
    }

    /// <summary>
    /// SDK会回调-通知支付成功
    /// </summary>
    void OnPaySuccessful(string contect)
    {
        Debug.LogError("SDK通知支付成功");
        EB.Sparx.Hub.Instance.VFPKSDKManager.OnPayResult(EB.Sparx.VFPKSDKResultCode.Succ);
    }

    /// <summary>
    /// SDK会回调-通知支付失败
    /// </summary>
    void OnPayFail(string ignore)
    {
        Debug.LogError("SDK通知支付失败");
        EB.Sparx.Hub.Instance.VFPKSDKManager.OnPayResult(EB.Sparx.VFPKSDKResultCode.Failed);
    }

    /// <summary>
    /// SDK会回调-通知注销成功
    /// </summary>
    void OnLogoutSuccessful(string contect)
    {
        Debug.LogError("SDK通知注销成功");
        EB.Sparx.Hub.Instance.VFPKSDKManager.OnLoginOutResult(EB.Sparx.VFPKSDKResultCode.Succ);
    }

}
#endif