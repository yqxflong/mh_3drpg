using EB.IAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

namespace EB.Sparx
{
#if USE_AOSHITANGSDK
    public class AoshitangSDK
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        public static extern void ASTInit(string isDebug);

        [DllImport("__Internal")]
        public static extern void ASTLogin();

        [DllImport("__Internal")]
        public static extern void ASTLogout();

        [DllImport("__Internal")]
        public static extern void ASTShowReviewAction(string content);

        [DllImport("__Internal")]
        public static extern void ASTShareLinkToFB(string url);

        [DllImport("__Internal")]
        public static extern void ASTPay(string itemId,string serverId,string roleId,string cost,string extra,string outsideId);

        [DllImport("__Internal")]
        public static extern void ASTCreateRole(string userId, string roleId, string serverId,string level, string vipLv, string gameDeviveId);

        [DllImport("__Internal")]
        public static extern void ASTLoginRole(string userId, string roleId, string serverId,string level, string vipLv, string gameDeviveId);

        [DllImport("__Internal")]
        public static extern void ASTLevelUpRole(string userId, string roleId, string serverId,string level, string vipLv, string gameDeviveId);
#endif

#if UNITY_ANDROID
        private const string SDK_JAVA_CLASS = "cn.unity3d.astgoogle.AsthwSDKManager";

        private static void callSdkApi(string apiName, params object[] args)
        {
            EB.Debug.Log("Unity3D " + apiName + " calling...");

            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                androidJavaClass.CallStatic(apiName, args);
            }
        }

        public static void ASTInit()
        {
            callSdkApi("init");
        }

        public static void ASTExit()
        {
            callSdkApi("exit");
        }

        public static void ASTLogin()
        {
            callSdkApi("login");
        }

        public static bool ASTIsGuest()
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                return androidJavaClass.CallStatic<bool>("isGuest");
            }
        }

        public static void ASTLogout()
        {
            callSdkApi("logout");
        }

        public static void ASTSwitchAccount()
        {
            callSdkApi("switchAccount");
        }

        public static void ASTRealnameAuth()
        {
            callSdkApi("launchRealnameAuth");
        }

        public static void ASTPay(string itemId, string serverId, string roleId, string cost, string extra, string outsideId)
        {
            callSdkApi("pay", itemId, serverId, roleId, cost, extra);
        }

        public static void ASTCreateRole(string roleId, string serverId, string level, string vipLv, string gameDeviveId)
        {
            callSdkApi("reportcreat", roleId, serverId, level, vipLv, gameDeviveId);
        }

        public static void ASTLoginRole(string roleId, string serverId, string level, string vipLv, string gameDeviveId)
        {
            callSdkApi("reportlogin",  roleId, serverId, level, vipLv, gameDeviveId);
        }

        public static void ASTLevelUpRole(string roleId, string serverId, string level, string vipLv, string gameDeviveId)
        {
            callSdkApi("repoetlevelup", roleId, serverId, level, vipLv, gameDeviveId);
        }

        public static void ASTjoinQQGroup(string key)
        {
            callSdkApi("joinQQGroup", key);
        }

        public static void ASTShowContent()
        {
            callSdkApi("moveToGooGlePlay");
        }

        public static void SharePicToFB(List<string> picUrl)
        {
            callSdkApi("SharePicUrlToFB",picUrl);
        }

        public static void ASTShareLinkToFB(string linkUrl)
        {
            callSdkApi("ShareLinkToFB",linkUrl);
        }

#endif
    }
#endif
    public class AoshitangSDKManager : BaseSDKManager
    {
        private const string t_classname = "AoshitangSDKManager";
        private const string t_objectname = "Aoshitang_listener";
        public const string gameId = "112";
        private bool m_Initialized = false;
        private bool isLoggedIn = false;
        private string mNotifyUrl;
        private System.Action<string, bool> mInitCallback = null;
        private System.Action<string> mTipCallback = null;
        private Action<string, object> mLoginCallback = null;
        private System.Action<int> mShareCallback = null;
        private System.Action<int> mPayCallback = null;
        private System.Action<int, object> mPurchaseLocaleCallback = null;
        private RoleData roleData;
        public string Deviceid;
        private int CurHc;
        private static AoshitangSDKManager _instance;
        private int userId;
        private long Logints;
        private string loginsign;
        private string serverId = "0";

        public bool SDKInitSuccess{get;private set;} = false;

        enum eReportType
        {
            none = 0,
            login = 1,
            levelup = 2,
            creatRole = 3,
            selfdefine = 4,
        }

        public static AoshitangSDKManager getInstance()
        {
            if (_instance == null)
            {
                _instance = new AoshitangSDKManager();
            }
            return _instance;
        }

        #region 初始化ast sdk
        //安卓由于要很早检查obb，需要抽出来给外部调用
        public void AstSDKInit() 
        {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            new GameObject(t_objectname, typeof(SparxAoshitangSDKManager));
            AoshitangSDK.ASTInit();
#endif

#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            new GameObject(t_objectname, typeof(SparxAoshitangSDKManager));
#if DEBUG
            AoshitangSDK.ASTInit("YES");
#else
            AoshitangSDK.ASTInit("NO");
#endif
#endif
        }
        #endregion

        #region Upload log
        private const string URL_LOG = "http://log-pub.marsgame.hk/log.action";
        private System.Text.StringBuilder UploadLog_SB = new System.Text.StringBuilder();
        private System.Action<bool> UploadLog_OnResonse = null;
        public void UploadLog(string flag, System.Action<bool> onResponse)
        {
            UploadLog_SB.Clear();
            UploadLog_SB.Append($"{gameId}#");
#if UNITY_IOS
            UploadLog_SB.Append($"ios#");
#elif UNITY_ANDROID
            UploadLog_SB.Append($"android#");
#endif
            UploadLog_SB.Append($"{flag}#");
#if UNITY_IOS
            string deviceId = EB.Version.GetIOSDeviceID();
            UploadLog_SB.Append($"{deviceId}#");
#elif UNITY_ANDROID
            string deviceId = EB.Version.GetAndroidDeviceID();
            UploadLog_SB.Append($"{deviceId}#");
#endif
            UploadLog_SB.Append($"{SystemInfo.operatingSystem}");
            string finalLog = UploadLog_SB.ToString();
            finalLog = UnityWebRequest.EscapeURL(finalLog);
            UploadLog_SB.Clear();

            if(onResponse!=null)
                UploadLog_OnResonse = onResponse;

            EB.Coroutines.Run(DoUploadLog(finalLog));
        }

        private IEnumerator DoUploadLog(string finalLog)
        {
            string urlWithParams = $"{URL_LOG}?gameId={gameId}&log={finalLog}";
            var request = UnityWebRequest.Get(urlWithParams);
            yield return request.SendWebRequest();
            int returnCode = -1;
            if (request.isHttpError || request.isNetworkError)
            {
                EB.Debug.LogWarning("DoUploadLog===>Failed==Err: {0}", request.error);
            }
            else
            {
                string respText = request.downloadHandler.text;
                EB.Debug.Log("DoUploadLog===>urlWithParams: {0}", urlWithParams);
                EB.Debug.Log("DoUploadLog===>respText: {0}", respText);
                try
                {
                    var jnode =  Johny.JSONNode.Parse(respText);
                    if(jnode.IsObject)
                    {
                        var jnode_code = jnode["code"];
                        if(jnode_code.IsNumber)
                        {
                            returnCode = jnode_code.AsInt;
                        }
                    }
                }
                catch(System.NullReferenceException e)
                {
                    EB.Debug.LogError(e.ToString());
                }
            }
            UploadLog_OnResonse?.Invoke(returnCode == 1);
            UploadLog_OnResonse = null;
        } 
        #endregion

        public void AddShareCallback(System.Action<int> callback)
        {
            mShareCallback = callback;
        }

        public override int GetAppid()
        {
            int.TryParse(gameId, out int appid);
            return appid;
        }

        ///游戏业务初始化流程至此
        public override void Init(object options, Action<string, bool> callback, Action<string> tipCallBack)
        {
            EB.Debug.Log("{0}.Init", t_classname);
            if (m_Initialized)
            {
                EB.Debug.LogWarning("{0} hasInit", t_classname);
                callback(null, true);
                return;
            }
            mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);
            mInitCallback += callback;
            mTipCallback += tipCallBack;
            
            UploadLog("startInitSDK", null);

#if !UNITY_EDITOR && USE_AOSHITANGSDK
            //之前已经初始化过，这里直接算初始化成功
            bool success = true;
            if (success)
            {
                m_Initialized = true;
            }
            mInitCallback?.Invoke(null, m_Initialized);
            mInitCallback = null;
#endif
        }

        public override bool IsLoggedIn()
        {
            return isLoggedIn;
        }

        public override void Login(Action<string, object> callback)
        {
            EB.Debug.Log("{0}.Login", t_classname);
            if (!m_Initialized)
            {
                EB.Debug.LogWarning("{0}.Login didnotInit", t_classname);
                callback(t_classname + "not initialized", null);
                return;
            }

            EB.Debug.Log("isLoggedIn: ", isLoggedIn);

            if (isLoggedIn)
            {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
                Logout();
                return;
#endif

#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
                Hashtable data = new Hashtable()
                {
                    {"userId", userId },
                    {"gameId", gameId },
                    { "serverId",serverId},
                    { "sign",loginsign},
                    { "ts",Logints},
                };
                callback(null, data);//需要传递游戏内参数，可以拿到登录数据
                return;
#endif
            }
            Hub.Instance.RunInBackground = true;
            mLoginCallback = callback;
            UploadLog("startLoginSDK", null);
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTLogin();
#endif
#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTLogin();
#endif
        }

        //当前是否是游客登录sdk
        public override bool IsGuest()
        {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            return AoshitangSDK.ASTIsGuest();
#endif
#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            //throw new NotImplementedException();
#endif
            return true;
        }

        //切换账号、弹出SDK账号管理系统
        public override void SwitchAccount()
        {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTSwitchAccount();
#endif
#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTLogout();
#endif
        }


        public override void Logout()//SDK未提供登出接口，暂时使用调起登录界面
        {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTLogout();
#endif
#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            isLoggedIn = false;
#endif
        }

        public void FetchPurchaseLocale(System.Action<int, object> callback)
        {
            mPurchaseLocaleCallback = callback;
        }

        public override void Pay(Item item, Transaction transaction, Action<int> callback)
        {
#if USE_AOSHITANGSDK && !UNITY_EDITOR
            EB.Debug.Log("{0}.Pay", t_classname);
            if (!m_Initialized)
            {
                EB.Debug.LogWarning("{0}.Pay didnotInit", t_classname);
                callback(SparxAoshitangSDKManager.RESULT_FAILED);
                return;
            }
            if (item == null || transaction == null)
            {
                EB.Debug.LogError("{0}.Pay item == null|| transaction == null");
                callback(SparxAoshitangSDKManager.RESULT_FAILED);
                return;
            }
            mPayCallback = callback;
            string itemid = item.productId;
            string serverid = roleData.serverId;
            string roleid = roleData.roleId;
            string usedamount = item.cents.ToString();
            string extrastr = $"{gameId},{transaction.transactionId}";
            string toappflyid = "";//appsflyerid 测试传空
            AoshitangSDK.ASTPay(itemid, serverid, roleid, usedamount, extrastr, toappflyid);
#endif
        }

        /// <summary>
        /// 上报角色行为信息
        /// </summary>
        /// <param name="reportType">上报类型 3创建角色 1登录 2升级 4 自定义</param>
        /// <param name="curroledata"></param>
        public void ReportRoleInfo(RoleData curroledata)
        {
            if (curroledata == null)
            {
                EB.Debug.LogError("{0}.ReportRoleInfo curroledata == null");
                return;
            }
            eReportType reportType = (eReportType)curroledata.code;
            string userid = roleData.roleGid;
            string roleid = roleData.roleId;
            string serverid = roleData.serverId;
            string level = roleData.roleLevel.ToString();
            string viplevel = "0";
            string deviveid;
#if UNITY_IOS
            deviveid = EB.Version.GetIOSDeviceID();
#elif UNITY_ANDROID
            deviveid = EB.Version.GetAndroidDeviceID();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            switch (reportType)
            {
                case eReportType.creatRole:
                    AoshitangSDK.ASTCreateRole(roleid, serverid, level, viplevel, deviveid);
                    break;
                case eReportType.login:
                    AoshitangSDK.ASTLoginRole(roleid, serverid, level, viplevel, deviveid);
                    break;
                case eReportType.levelup:
                    AoshitangSDK.ASTLevelUpRole(roleid, serverid, level, viplevel, deviveid);
                    break;
                default:
                    break;
            }

#endif
#if UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            switch (reportType)
            {
                case eReportType.creatRole:
                    AoshitangSDK.ASTCreateRole(userid, roleid, serverid, level, viplevel, deviveid);
                    break;
                case eReportType.login:
                    AoshitangSDK.ASTLoginRole(userid, roleid, serverid, level, viplevel, deviveid);
                    break;
                case eReportType.levelup:
                    AoshitangSDK.ASTLevelUpRole(userid, roleid, serverid, level, viplevel, deviveid);
                    break;
                default:
                    break;
            }
#endif
        }

        public void OnShareLinkToFB(string LinkUrl)
        {
#if !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTShareLinkToFB(LinkUrl);
#endif
        }

        public void OnSharePicToFB(List<string> PicUrl)
        {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.SharePicToFB(PicUrl);
#endif
        }

        public override void SetCurHCCount(int curHCCount)
        {
            CurHc = curHCCount;
        }

        public override void SetRoleData(RoleData roleData)
        {
            this.roleData = roleData;
            ReportRoleInfo(this.roleData);
        }
        public override void OnJoinQQGroup(string key)
        {
            throw new NotImplementedException();
        }


        public override void Exit()
        {
            Logout();
        }

        public void ShowStoreContentView(string content)
        {
#if UNITY_ANDROID && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTShowContent();
#elif UNITY_IOS && !UNITY_EDITOR && USE_AOSHITANGSDK
            AoshitangSDK.ASTShowReviewAction(content);
#endif
        }

        public override void OnExitResult(int code)
        {
            EB.Debug.Log("{0}.OnExitResult code:{1}", t_classname, code);
        }

        public override void OnInitResult(bool success, string msg)
        {
#if !DEBUG
            EB.Debug.Log("OnInitResult===>{0}", success.ToString());
            SDKInitSuccess = success;
            if(success == false)
            {
                EB.Debug.LogError("OnInitResult===>AST SDK Init Failed!!!");
            }
#else
            EB.Debug.Log("OnInitResult===>true");
            SDKInitSuccess = true;
#endif
        }

        public override void OnSwitchResult(int code, object msg) {
            EB.Debug.Log("{0}.OnSwitchResult code:{1}", t_classname, code);

            if (code == SparxAoshitangSDKManager.RESULT_SUCESS) {
                if (msg != null) {
                    userId = EB.Dot.Integer("userId", msg, 0);
                    Logints = EB.Dot.Long("ts", msg, 0);
                    loginsign = EB.Dot.String("sign", msg, string.Empty);

                    EB.Debug.Log("{0}.OnSwitchResult userId:{1}", t_classname, userId);
                }

                isLoggedIn = true;
                Hub.Instance.Disconnect(true);
            }
        }

        public override void OnLoginOutResult(int code)
        {
            EB.Debug.Log("{0}.OnLoginOutResult code:{1} and isLoggedIn:{2}", t_classname, code, isLoggedIn);
            Hub.Instance.RunInBackground = false;

            if (code == SparxAoshitangSDKManager.RESULT_SUCESS && isLoggedIn)
            {
                isLoggedIn = false;
                Hub.Instance.Disconnect(true);
            }
        }

        private int FailCount = 0;
        public override void OnLoginResult(int code, object msg)
        {
            EB.Debug.Log("{0}.OnLoginResult code:{1}", t_classname, code);
            UploadLog("loginSDKOver", null);
            Hub.Instance.RunInBackground = false;
            if (code == SparxAoshitangSDKManager.RESULT_SUCESS)
            {
                isLoggedIn = true;
                userId = EB.Dot.Integer("userId", msg, 0);
                Logints = EB.Dot.Long("ts", msg, 0);
                loginsign = EB.Dot.String("sign", msg, string.Empty);
                if (mLoginCallback != null)
                {
                    Hashtable data = new Hashtable()
                    {
                        {"userId", userId },
                        {"gameId", gameId },
                        { "serverId",serverId},
                        { "sign",loginsign},
                        { "ts",Logints},
                    };

                    mLoginCallback?.Invoke(null, data);
                    mLoginCallback = null;
                }
            }
            else if (code == SparxAoshitangSDKManager.RESULT_FAILED)
            {
                // if (mLoginCallback != null)
                // {
                //     mLoginCallback = null;
                // }
                // FailCount++;
                // if (FailCount >= 8)
                // {
                //     mTipCallback?.Invoke("ID_SPARX_LOGIN_FAIL_MORE");
                //     return;
                // }
                // Hub.Instance.Disconnect(true);
            }
        }

        public override void OnPayResult(int code)
        {
            EB.Debug.Log("{0}.OnPayResult code:{1}", t_classname, code);
            mPayCallback?.Invoke(code);
            mPayCallback = null;
        }

        public void OnShareResult(int code)
        {
            EB.Debug.Log("{0}.OnShareResult code:{1}", t_classname, code);
            mShareCallback?.Invoke(code);
        }

        public void OnFetchPurchaseLocaleResult(int code, object info)
        {
            EB.Debug.Log("{0}.OnFetchPurchaseLocaleResult code:{1} info:{2}", t_classname, code, info);
            mPurchaseLocaleCallback?.Invoke(code, info);
        }

        public override void OnShowTipCall(string tip)
        {
            mTipCallback?.Invoke(tip);
        }

    }


    public class SparxAoshitangSDKManager : MonoBehaviour//IOS ANDROID 回调
    {
        private const string classname = "SparxAoshitangSDKManager";
        private const string CALLBACKTYPE_INITSDK = "Init";
        private const string CALLBACKTYPE_LOGIN = "Login";
        private const string CALLBACKTYPE_PAY = "Pay";
        private const string CALLBACKTYPE_LOGINOUT = "Logout";
        private const string CALLBACKTYPE_EXIT = "Exit";
        private const string CALLBACKTYPE_SHARE = "Share";
        private const string CALLBACKTYPE_GETPAYMENT = "Getpayment";
        private const string CALLBACKTYPE_SWITCH = "Switch";

        public const int RESULT_FAILED = 0;
        public const int RESULT_SUCESS = 1;

        private void Awake()
        {
            EB.Debug.Log("Creating {0}", classname);
            DontDestroyOnLoad(gameObject);
        }

        private void OnAoshitangCallback(string jsonString)
        {
            EB.Debug.Log("{0}.OnASTCallback:jsonString = {1}", classname, jsonString);
            object json = EB.JSON.Parse(jsonString);
            string callbackType = EB.Dot.String("callbackType", json, string.Empty);
            int code = EB.Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case CALLBACKTYPE_INITSDK:
                    EB.Sparx.AoshitangSDKManager.getInstance().OnInitResult(code == 1, null);
                    break;

                case CALLBACKTYPE_LOGIN:
                    EB.Sparx.Hub.Instance.mBaseSdkManager.OnLoginResult(code, EB.Dot.Object("data", json, null));
                    break;
                case CALLBACKTYPE_PAY:
                    EB.Sparx.Hub.Instance.mBaseSdkManager.OnPayResult(code);
                    break;

                case CALLBACKTYPE_LOGINOUT:
                    EB.Sparx.Hub.Instance.mBaseSdkManager.OnLoginOutResult(code);
                    break;

                case CALLBACKTYPE_EXIT:
                    EB.Sparx.Hub.Instance.mBaseSdkManager.OnExitResult(code);
                    break;
                case CALLBACKTYPE_SHARE:
                    AoshitangSDKManager.getInstance().OnShareResult(code);
                    break;
                case CALLBACKTYPE_GETPAYMENT:
                    AoshitangSDKManager.getInstance().OnFetchPurchaseLocaleResult(code, json);
                    break;
                case CALLBACKTYPE_SWITCH:
                    EB.Sparx.Hub.Instance.mBaseSdkManager.OnSwitchResult(code, EB.Dot.Object("data", json, null));
                    break;
                default:
                    EB.Debug.LogError("NOT FIND! OnAoshitangCallback.callbackType = " + callbackType);
                    break;
            }
        }
    }
}
