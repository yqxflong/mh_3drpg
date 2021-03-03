#if USE_XINKUAISDK
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace EB.Sparx
{

    public class XinkuaiSDKResultCode
    {
        public const int Succ = 0;//成功code

        public const int Failed = 1;//失败code

        public const int Cancel = 2;//取消code

        public const int Unknow = 3;//未安装支付宝
    }

    public class XinkuaiSDKConfig
    {
        public const int CREATE_ROLE = 0;
        public const int ENTER_SERVER = 1;

        public const int LEVEL_UP = 2;

        public const int ReCharge = 5;//充值，unity做区分，上报SDk时实际用升级
    }

    public class XinkuaiSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "Init";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_LOGINOUT = "LoginOut";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_EXIT = "Exit";

        public const string CALLBACKTYPE_REALNAME = "RealnameAuth";
    }

    public class XinkuaiSDK
    {
#if UNITY_IPHONE
        [DllImport("__Internal")]
        public static extern void Init();

        [DllImport("__Internal")]
        public static extern void Login();

        [DllImport("__Internal")]
        public static extern void Logout();

        [DllImport("__Internal")]
        public static extern void Pay(string unitName, string unitDesc, string transactionId, string thirdpatyId, string serverid, int unitPrice, string callBackInfo);

        [DllImport("__Internal")]
        public static extern void SetRoleData(string serverId, string rolename, string rolelevel, string servername, string roleid, string code,  int coinNum, string allianceName = "");

        [DllImport("__Internal")]
        public static extern void Exit();

        //[DllImport("__Internal")]
        //public static extern void RealnameAuth();
#endif

#if UNITY_ANDROID
        private const string SDK_JAVA_CLASS = "cn.xinkuai.gamesdk.unity3d.XinkuaiGameSDK";

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

        public static void RealnameAuth()
        {
            callSdkApi("launchRealnameAuth");
        }
        public static void Pay(int price, string productId, string productName, string productDesc, string orderId, string roleId, string roleName, int roleLevel, string serverId, string serverName, int coinNum, string payNotifyUrl, string extension,int vipLevel = 0)
        {
            callSdkApi("Pay", price, productId, productName, productDesc, orderId, roleId, roleName, roleLevel, serverId, serverName, coinNum, payNotifyUrl, extension, vipLevel);
        }

        public static void SetRoleData(int code, string roleGid, string roleId, string roleName, int roleLevel, string serverId, string serverName, int coinNum, string allianceName = "",int viplevel = 0)
        {
            callSdkApi("reportRoleInfo", code, roleGid, roleId, roleName, roleLevel, serverId, serverName, coinNum,allianceName, viplevel);
        }

        public static void joinQQGroup(string key)
        {
            callSdkApi("joinQQGroup", key);
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
    public class XinkuaiSDKManager : BaseSDKManager
    {
        public int CurHCCount = 0;
        private System.Action<string, bool> mInitCallback = null;
        private System.Action<string, object> mAuthCallback = null;
        private System.Action<int> mPayCallback = null;

        private System.Action<string> mTipCallback = null;

        private string mNotifyUrl = string.Empty;

        private string mUid = string.Empty;
        private string mSid = string.Empty;
        //private string mName = string.Empty;
        private string mAppid = string.Empty;
        private bool mInitialized = false;
        private bool mLoggedIn = false;
        private bool isrealnameAuth = false;
        private int age = 0;

        private RoleData roleData;

        private static XinkuaiSDKManager _instance;

        public static XinkuaiSDKManager getInstance()
        {
            if (null == _instance)
            {
                _instance = new XinkuaiSDKManager();
            }
            return _instance;
        }

        private XinkuaiSDKManager()
        {

        }

        public override bool IsLoggedIn()
        {
            return mLoggedIn;

        }

        public override void SetCurHCCount(int curHCCount)
        {
            this.CurHCCount = curHCCount;
        }

        public override int GetAppid()
        {
            int appid = 1;
            int.TryParse(mAppid, out appid);
            return appid;
        }

        //public void SetData(int code)//由LTMainHudManager中调用
        //{
        //    var account = EB.Sparx.Hub.Instance.LoginManager.Account;
        //    var user = Hub.LoginManager.LocalUser;
        //    var gameWorlds = Hub.Instance.LoginManager.GameWorlds;
        //    var gameWorld = System.Array.Find(gameWorlds, w => w.Id == user.WorldId);
        //    EB.Sparx.Hub.Instance.XinkuaiTapSDKManager.SetRoleData(code, account.AccountId.ToString(), user.Id.ToString(), user.Name, CurUserLevel, user.WorldId.ToString(), gameWorld.Name, CurHCCount);
        //}

        public override void Init(object options, Action<string, bool> callback, Action<string> tipCallBack)
        {
            EB.Debug.Log("XinkuaiSDKManager.Init");
            if (mInitialized)
            {
                EB.Debug.LogWarning("XinkuaiSDKManager.Init: Initialized");
                callback(null, true);
                return;
            }

            mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);

            mInitCallback += callback;
            mTipCallback += tipCallBack;
            Hub.Instance.RunInBackground = true;
            new GameObject("Xinkuai_listener", typeof(SparxXinkuaiSDKManager));

#if UNITY_ANDROID
            XinkuaiSDK.Init();
#endif
#if UNITY_IPHONE
            XinkuaiSDK.Init();
            // OnInitResult(true, "");
#endif
        }

        public override void Login(Action<string, object> callback)
        {
            EB.Debug.Log("XinkuaiSDKManager.Login");
            if (!mInitialized)
            {
                EB.Debug.LogError("XinkuaiSDKManager.Login: not initialized");
                callback("XinkuaiSDKManager not initialized", null);
                return;
            }
            if (mLoggedIn)
            {
                Hashtable data = new Hashtable()
                {
                        {"userid", mUid },
                        {"sid", mSid },
                        {"realnameAuth", isrealnameAuth },
                        { "age",age},
                        {"appid",mAppid }
                };
                callback(null, data);
                return;
            }
            mAuthCallback += callback;
            Hub.Instance.RunInBackground = true;
#if UNITY_ANDROID
            XinkuaiSDK.Login();
#endif
#if UNITY_IPHONE
            XinkuaiSDK.Login();
#endif
        }

        public override void Logout()
        {
#if UNITY_ANDROID
            XinkuaiSDK.LoginOut();
#endif
#if UNITY_IPHONE
            XinkuaiSDK.Logout();
#endif
        }

        public void RealnameAuth()
        {
#if UNITY_ANDROID
            XinkuaiSDK.RealnameAuth();
#endif
//#if UNITY_IPHONE
//            XinkuaiSDK.RealnameAuth();
//#endif
        }

        public override void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("XinkuaiSDKManager.Pay");

            if(item == null || transaction == null)
            {
                EB.Debug.LogError("XinkuaiSDKManager.Pay: item == null || transaction == null!!!");
                callback?.Invoke(XinkuaiSDKResultCode.Failed);
                return;
            }

            if (!mInitialized)
            {
                EB.Debug.LogError("XinkuaiSDKManager.Pay: not initialized!!!");
                callback?.Invoke(XinkuaiSDKResultCode.Failed);
                return;
            }

            mPayCallback += callback;

            object extraInfo = EB.JSON.Parse(transaction.payload);
            int price = Mathf.FloorToInt(item.cost * 100f);
            string productId = item.productId;
            string productName = item.longName;
            string productDesc = item.localizedDesc;
            string orderId = transaction.transactionId;
            string roleId = roleData.roleId;
            string roleName = roleData.roleName;
            int roleLevel = roleData.roleLevel;
            string serverId = roleData.serverId;
            string serverName = roleData.serverName;
#if UNITY_IPHONE
            string extension = EB.Dot.String("callbackInfo", extraInfo, "0");
#else
            string extension = EB.Dot.String("callbackInfo", extraInfo, string.Empty);
#endif


            EB.Debug.Log("XinkuaiSDKManager.Pay: roleId = {0},roleName = {1},roleLevel = {2},serverId = {3},serverName = {4},CurHCCount = {5}", roleId, roleName, roleLevel, serverId, serverName, CurHCCount);
#if UNITY_ANDROID
            //price, productId, productName, productDesc, orderId, roleId, roleName, roleLevel, serverId, serverName, coinNum, payNotifyUrl, extension
            XinkuaiSDK.Pay(price, productId, productName, productDesc, orderId, roleId, roleName, roleLevel, serverId, serverName, CurHCCount, mNotifyUrl, extension);
#endif

#if UNITY_IPHONE
            //rmb 充值金额 单位元
            //productID iTunes 苹果后台配置的内购物品的产品ID
            //name 商品名
            //charid 角色ID
            //serverid 服务器ID
            //info 扩展信息
            //cporderid 游戏商订单ID
            // string payInfo = cost + ";"
            //     + transaction.productId + ";"
            //     + name + ";"
            //     + charId + ";"
            //     + serverId + ";"
            //     + callbackInfo + ";"
            //     + cporderId;
            // Debug.LogError("支付内容:payInfo:" + payInfo);
            EB.Debug.Log("XinkuaiSDKManager.Pay: productName = {0},productDesc = {1},orderId = {2},productId = {3},price = {4},extension = {5}", productName, productDesc, orderId, productId, price, extension);
            XinkuaiSDK.Pay(productName, productDesc, orderId, productId, serverId, price, extension/*, mNotifyUrl*/);
#endif
        }

        public override void SetRoleData(RoleData roleData)
        {
            //注意这里 在登录的时候获取到值后 后面支付参数才有值
             this.roleData = roleData;
#if UNITY_ANDROID
            XinkuaiSDK.SetRoleData(roleData.code, roleData.roleGid, roleData.roleId, roleData.roleName, roleData.roleLevel, roleData.serverId, roleData.serverName, roleData.coinNum,"");
#endif
#if UNITY_IPHONE
            XinkuaiSDK.SetRoleData(roleData.serverId, roleData.roleName, roleData.roleLevel.ToString(), roleData.serverName, roleData.roleId, roleData.code.ToString(), roleData.coinNum,"");
#endif
        }

        public override void Exit()
        {
            if (mInitialized)
            {
                XinkuaiSDK.Exit();
            }
        }

        public override void OnJoinQQGroup(string key)
        {
#if UNITY_ANDROID
            XinkuaiSDK.joinQQGroup(key);
#endif
            // #if UNITY_IPHONE
            //             XinkuaiSDK.joinQQGroup();
            // #endif
        }

        public override void OnInitResult(bool success, string msg)
        {
            Debug.Log(string.Format("OnInitResult: success={0}, msg={1}", success, msg));
            Hub.Instance.RunInBackground = false;
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

        private int FailCount = 0;
        public override void OnLoginResult(int code, object msg)
        {
            EB.Debug.Log("OnLoginResult: code={0}, msg={1}", code, msg);
            Hub.Instance.RunInBackground = false;
            if (code == XinkuaiSDKResultCode.Succ)
            {
                mLoggedIn = true;
                mSid = EB.Dot.String("sid", msg, string.Empty);
                mUid = EB.Dot.String("userid", msg, string.Empty);
                isrealnameAuth = EB.Dot.Bool("realnameAuth", msg, false);
                age = EB.Dot.Integer("age", msg, 0);
                mAppid = EB.Dot.String("appid", msg, string.Empty);
                if (mAuthCallback != null)
                {
                    Hashtable auth = new Hashtable();
                    auth["sid"] = mSid;
                    auth["userid"] = mUid;
                    auth["realnameAuth"] = isrealnameAuth;
                    auth["age"] = age;
                    auth["appid"] = mAppid;
                    mAuthCallback(null, auth);
                    mAuthCallback = null;
                }
            }
            else if (code == XinkuaiSDKResultCode.Cancel || code == XinkuaiSDKResultCode.Failed)
            {
                if (mAuthCallback != null)
                {
                    mAuthCallback = null;
                }
                if (code == XinkuaiSDKResultCode.Failed)
                {
                    FailCount++;
                    EB.Debug.LogWarning("XinkuaiSDKResultCode.LoginFailed!");
                    if (FailCount >= 8)
                    {
                        if (mTipCallback != null) mTipCallback("ID_SPARX_LOGIN_FAIL_MORE");
                        return;
                    }
                }
                else
                {
                    EB.Debug.LogWarning("XinkuaiSDKResultCode.LoginCancel!");
                }
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

        public override void OnPayResult(int code)
        {
            EB.Debug.Log("OnPayResult: code={0}", code);
            if (code == XinkuaiSDKResultCode.Unknow)
            {
                if (mTipCallback != null) mTipCallback("支付宝未安装");
            }
            if (mPayCallback != null)
            {
                mPayCallback(code);
                mPayCallback = null;
            }
        }

        public override void OnLoginOutResult(int code)
        {
            EB.Debug.Log("OnLogout: code={0}", code);
            if (code == XinkuaiSDKResultCode.Succ && mLoggedIn == true)
            {
                mLoggedIn = false;
                mUid = string.Empty;
                mSid = string.Empty;
                isrealnameAuth = false;
                age = 0;
                mAppid = string.Empty;
                Hub.Instance.Disconnect(true);
            }
        }

        public override void OnExitResult(int code)
        {
            EB.Debug.Log("OnExit: code={0}", code);
        }

        public override void OnShowTipCall(string tip)
        {
            if (mTipCallback != null)
            {
                mTipCallback(tip);
            }
        }
    }
}

public class SparxXinkuaiSDKManager : MonoBehaviour
{
    void Awake()
    {
        EB.Debug.Log("Creating SparxXinkuaiSDKManager");
        DontDestroyOnLoad(gameObject);
    }

    public void OnXinkuaiCallback(string jsonString)
    {
        EB.Debug.Log(string.Format("SparxXinkuaiSDKManager.OnXinkuaiCallback:jsonString = {0}", jsonString));
        object json = EB.JSON.Parse(jsonString);
        string callbackType = EB.Dot.String("callbackType", json, string.Empty);
        int code = EB.Dot.Integer("code", json, -1);
        switch (callbackType)
        {
            case EB.Sparx.XinkuaiSDKCallbackType.CALLBACKTYPE_INITSDK:
                EB.Sparx.Hub.Instance.mBaseSdkManager.OnInitResult(code == 0, null);
                break;

            case EB.Sparx.XinkuaiSDKCallbackType.CALLBACKTYPE_LOGIN:
                EB.Sparx.Hub.Instance.mBaseSdkManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
                break;

            case EB.Sparx.XinkuaiSDKCallbackType.CALLBACKTYPE_PAY:
                EB.Sparx.Hub.Instance.mBaseSdkManager.OnPayResult(code);
                break;

            case EB.Sparx.XinkuaiSDKCallbackType.CALLBACKTYPE_LOGINOUT:
                EB.Sparx.Hub.Instance.mBaseSdkManager.OnLoginOutResult(code);
                break;

            case EB.Sparx.XinkuaiSDKCallbackType.CALLBACKTYPE_EXIT:
                EB.Sparx.Hub.Instance.mBaseSdkManager.OnExitResult(code);
                break;

            default:
                EB.Debug.LogError("NOT FIND! OnXinkuaiCallback.callbackType = " + callbackType);
                break;
        }
    }

    /// <summary>1  
    /// SDK会回调-通知初始化完成
    /// </summary>
    void OnInitFinish(string contect)
    {
        EB.Sparx.Hub.Instance.mBaseSdkManager.OnInitResult(true, null);
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
        EB.Sparx.Hub.Instance.mBaseSdkManager.OnLoginResult(EB.Sparx.XinkuaiSDKResultCode.Succ, data);
    }

    /// <summary>
    /// SDK会回调-通知支付成功
    /// </summary>
    void OnPaySuccessful(string contect)
    {
        Debug.LogError("SDK通知支付成功");
        EB.Sparx.Hub.Instance.mBaseSdkManager.OnPayResult(EB.Sparx.XinkuaiSDKResultCode.Succ);
    }

    /// <summary>
    /// SDK会回调-通知支付失败
    /// </summary>
    void OnPayFail(string ignore)
    {
        Debug.LogError("SDK通知支付失败");
        EB.Sparx.Hub.Instance.mBaseSdkManager.OnPayResult(EB.Sparx.XinkuaiSDKResultCode.Failed);
    }

    /// <summary>
    /// SDK会回调-通知注销成功
    /// </summary>
    void OnLogoutSuccessful(string contect)
    {
        Debug.LogError("SDK通知注销成功");
        EB.Sparx.Hub.Instance.mBaseSdkManager.OnLoginOutResult(EB.Sparx.XinkuaiSDKResultCode.Succ);
    }

}
#endif