#if USE_YIJIESDK
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;

namespace EB.Sparx
{
    public class YiJieSDKConfig
    {
        public const int INIT_SUCCESS = 0;

        public const int INIT_FAILURE = 1;

        /** logout*/
        public const int LOGOUT = 0;

        /** login success */
        public const int LOGIN_SUCCESS = 1;

        /** login failed*/
        public const int LOGIN_FAILED = 2;


        /** pay success */
        public const int PAY_SUCCESS = 0;

        /** pay failure*/
        public const int PAY_FAILURE = 1;

        /** pay orderNo*/
        public const int PAY_ORDER_NO = 2;

        /** exit success */
        public const int EXIT_SUCCESS = 0;

        /** exit cancel*/
        public const int EXIT_CANCEL = 1;

        /** no sdk exit */
        public const int EXIT_NOT_EXIST = 2;


        //public const string LEWAN_CHANNEL_ID = "{2BE115F0-3999EFA6}";

        //public const string KUAIYONG_CHANNEL_ID = "{CBD29905-6C9CC89B}";

        //public const string UC_CHANNEL_ID = "{F52F35C5-A04A1876}";


        public const string CREATE_ROLE = "createrole";

        public const string LEVEL_UP = "levelup";

        public const string ENTER_SERVER = "enterServer";

        //乐玩SDK必接
        //public const string ENTER_GAME = "enterGame";

        //快用SDK必接
        //public const string GAME_START = "gamestart";

        //快用SDK必接
        //public const string GAME_EXIT = "gameexit";

        //快用SDK必接
        //public const string ON_START = "onStart";

        //UCSDK必接
        //public const string LOGIN_GAME_ROLE = "loginGameRole";
    }

    public class YiJieSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_EXIT = "Exit";

    }

    public class YiJieSDK
    {
#if UNITY_IPHONE
        	
        [DllImport("__Internal")]
        private static extern void InitSDK();

        [DllImport("__Internal")]
        private static extern void Login();

        [DllImport("__Internal")]
        private static extern void Logout();

        [DllImport("__Internal")]
        private static extern void Pay(int unitPrice, string itemName, int count, string callBackInfo, string callBackUrl);

        [DllImport("__Internal")]
        private static extern void SetRoleData(string roleId, string roleName, string roleLevel, string zoneId, string zoneName);

        [DllImport("__Internal")]
        private static extern void SetData(string dataType, string jsonString);

        [DllImport("__Internal")]
        public static extern string GetChannelId();

        [DllImport("__Internal")]
        public static extern void ExitSDK();

#endif

#if UNITY_ANDROID
        private const string SDK_JAVA_CLASS = "cn.yijie.gamesdk.unity3d.YiJieSDK";

        private static void callSdkApi(string apiName, params object[] args)
        {
            EB.Debug.Log("Unity3D " + apiName + " calling...");

            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                androidJavaClass.CallStatic(apiName, args);
            }
        }

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

        public static void Pay(int unitPrice, string itemName, int count, string callBackInfo, string callBackUrl)
        {
            callSdkApi("Pay", unitPrice, itemName, count, callBackInfo, callBackUrl);
        }

        public static void SetRoleData(string roleId, string roleName, string roleLevel, string zoneId, string zoneName)
        {
            callSdkApi("SetRoleData", roleId, roleName, roleLevel, zoneId, zoneName);
        }

        public static void SetData(string dataType, string jsonString)
        {
            callSdkApi("SetData", dataType, jsonString);
        }

        public static void ExitSDK()
        {
            callSdkApi("ExitSDK");
        }

        public static string GetChannelId()
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(SDK_JAVA_CLASS))
            {
                return androidJavaClass.CallStatic<string>("GetChannelId");
            }
        }
#endif
        public static void pxInit()
        {
            InitSDK();
        }

        public static void pxLogin()
        {
            Login();
        }

        public static void pxLogout()
        {
            Logout();
        }

        public static void pxPay(int unitPrice, string unitName, int count, string callBackInfo, string callBackUrl)
        {
            Pay(unitPrice, unitName, count, callBackInfo, callBackUrl);
        }

        public static void pxSetRoleData(string roleId, string roleName, string roleLevel, string zoneId, string zoneName)
        {
            SetRoleData(roleId, roleName, roleLevel, zoneId, zoneName);
        }

        public static void pxSetData(string dataType, string jsonString)
        {
            SetData(dataType, jsonString);
        }

        public static void pxExitSDK()
        {
            ExitSDK();
        }

        public static string pxGetChannelId()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return GetChannelId();
#elif UNITY_IPHONE && !UNITY_EDITOR
            return GetChannelId();
#else
			return string.Empty;
#endif
        }
    }

    public class YiJieSDKManager : Manager
    {
        public bool IsLoggedIn { get { return mLoggedIn; } }
        private static Hashtable authData = new Hashtable();

        private string plugin_listener = "yijie_plugin_listener";

        private string mCallbackUrl = string.Empty;
        private bool mInitialized = false;
        private Action<string, bool> mInitCallback;

        private bool mLoggedIn = false;
        private Action<string, object> mLoginCallback;
		//private static bool mSDKLogout=false;

        private Action<int> mPayCallback;

        private Action<int> mExitSDKCallback;

        public override void Initialize(Config config)
        {

        }

        public override void OnLoggedIn()
        {
            Hub.RunInBackground = false;
            SetRoleData();
            /*if (YiJieSDK.pxGetChannelId() == YiJieSDKConfig.UC_CHANNEL_ID)
            {
                SetData(YiJieSDKConfig.LOGIN_GAME_ROLE);
            }
            if (YiJieSDK.pxGetChannelId() == YiJieSDKConfig.LEWAN_CHANNEL_ID)
            {
                SetData(YiJieSDKConfig.GAME_START);
            }*/
            SetData(YiJieSDKConfig.ENTER_SERVER);
            SparxHub.Instance.LevelRewardsManager.OnLevelChange += delegate 
            {
                SetData(YiJieSDKConfig.LEVEL_UP);
            };
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
                //EB.Debug.Log("YiJieSDKManager.OnEnteredForeground");
                EB.Coroutines.SetTimeout(delegate ()
                {
                    //EB.Debug.Log("YiJieSDKManager.OnEnteredForeground PayCallback");
                    if (mPayCallback != null)
                    {
                        mPayCallback(YiJieSDKConfig.PAY_FAILURE);
                        mPayCallback = null;
                    }
                }, 5 * 1000);
            }
        }

        public void OnLoginEnteredForeground()
        {
            if (IsLoggedIn)
            {
                return;
            }
            if (mLoginCallback != null)
            {
                EB.Coroutines.SetTimeout(delegate ()
                {
                    if (mLoginCallback != null)
                    {
                        EB.Debug.Log("OnLoginEnteredForeground logincallback");
                        System.Action<string, object> callback = mLoginCallback;
                        mLoginCallback = null;
                        callback(null, null);
                    }
                }, 3 * 1000);
            }
        }

        public void Init(object initData, Action<string, bool> callback)
        {
            EB.Debug.Log("YiJieSDKManager.Init");
            if (mInitialized)
            {
                EB.Debug.LogWarning("YiJieSDKManager.Init: Initialized");
                callback(null, true);
                return;
            }
            mCallbackUrl = EB.Dot.String("notifyUrl", initData, mCallbackUrl);
            EB.Debug.Log("YiJieSDKManager.Init", mCallbackUrl);
            mInitCallback += callback;
            Hub.RunInBackground = true;
            new GameObject(plugin_listener, typeof(SparxYiJieSDKManager));
            YiJieSDK.pxInit();
        }

        public void OnInitCallback(int resultCode)
        {
            EB.Debug.Log("YiJieSDKManager.OnInitCallback resultCode = {0}", resultCode);
            Hub.RunInBackground = false;
            if (resultCode.Equals(YiJieSDKConfig.INIT_SUCCESS))
            {
                mInitialized = true;
            }
            if (mInitCallback != null)
            {
                mInitCallback(null, mInitialized);
                mInitCallback = null;
            }
        }

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("YiJieSDKManager.Login");
            if (!mInitialized)
            {
                callback("YiJieSDK has not been inited", null);
                EB.Debug.LogError("YiJiSDK.Login: not initialized");
                return;
            }

            if (mLoggedIn)
            {
                callback(null, authData);
                return;
            }

            mLoginCallback += callback;
            Hub.RunInBackground = true;

            YiJieSDK.pxLogin();
            /*if (!mSDKLogout)
			{
				YiJieSDK.pxLogin();
			}
			else
			{
				mSDKLogout = false;
				mLoggedIn = true;
				if (mLoginCallback != null)
				{
					mLoginCallback(null, authData);
					mLoginCallback = null;
				}
			}*/
        }

        public void OnLoginCallback(int resultCode, object resultData)
        {
            EB.Debug.Log("YiJieSDKManager.OnLoginCallback resultCode = {0} result = {1} mLoggedIn = {2}", resultCode, JSON.Stringify(resultData), mLoggedIn);
            //Hub.RunInBackground = false;
            if (resultCode == YiJieSDKConfig.LOGOUT)
            {
                if (mLoggedIn)
                {
                    mLoggedIn = false;
					//mSDKLogout = true;
					authData = InitAuthData(resultData);
					Sparx.Hub.Instance.Disconnect(true);
                }
            }
            else if (resultCode == YiJieSDKConfig.LOGIN_SUCCESS)
            {
                if (mLoggedIn)
                {
                    //mLoggedIn = false;
					//mSDKLogout = true;
                    authData = InitAuthData(resultData);
                    Sparx.Hub.Instance.Disconnect(false);
                }
                else
                {
                    if (resultData != null)
                    {
                        mLoggedIn = true;
                        authData = InitAuthData(resultData);
                        if (mLoginCallback != null)
                        {
                            mLoginCallback(null, authData);
                            mLoginCallback = null;
                        }
                    }
                }
            }
            else if (resultCode == YiJieSDKConfig.LOGIN_FAILED)
            {
                if (mLoginCallback != null)
                {
                    mLoginCallback(null, null);
                    mLoginCallback = null;
                }
            }
        }

        public Hashtable InitAuthData(object resultData)
        {
            string appId = EB.Dot.String("appId", resultData, string.Empty);
            string channelId = EB.Dot.String("channelId", resultData, string.Empty);
            string userId = EB.Dot.String("userId", resultData, string.Empty);
            string token = EB.Dot.String("token", resultData, string.Empty);
            Hashtable auth = new Hashtable();
            auth["app"] = appId;
            auth["sdk"] = channelId;
            auth["uin"] = userId;
            auth["sess"] = token;
            return auth;
        }

        public Hashtable GetAuthData()
        {
            EB.Debug.Log("YiJieSDKManager.GetAuthData authData = {0}", authData);
            return authData;
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, Action<int> callback)
        {
            if (!mInitialized)
            {
                callback(YiJieSDKConfig.PAY_FAILURE);
                EB.Debug.LogError("YiJiSDK.Pay: not initialized");
                return;
            }
            mPayCallback += callback;
            int unitPrice = 10;
            string unitName = EB.Localizer.GetString("ID_LADDER_STAGE_ZUANSHI");
            int count = item.cents / 10;
            if (item.productId == "com.manhuang.fusesandbox.aliancecard" || item.productId == "com.manhuang.fusesandbox.monthcard")
            {
                unitPrice = item.cents;
                unitName = item.longName;
                count = 1;
            }
            string callbackInfo = transaction.transactionId;
            YiJieSDK.pxPay(unitPrice, unitName, count, callbackInfo, mCallbackUrl);
            EB.Debug.Log("YiJieSDKManager.Pay unitPrice = {0} unitName = {1} count = {2} callBackInfo = {3} callBackUrl = {4}", unitPrice, unitName, count, callbackInfo, mCallbackUrl);
        }

        public void OnPayCallback(int resultCode)
        {
            EB.Debug.Log("YiJieSDKManager.OnPayCallback resultCode = {0}", resultCode);
            if (mPayCallback != null)
            {
                if (resultCode != YiJieSDKConfig.PAY_ORDER_NO)
                {
                    mPayCallback(resultCode);
                    mPayCallback = null;
                }
            }
        }

        public void Logout()
        {
            YiJieSDK.pxLogout();
        }

        public void SetRoleData()
        {
            if (!mInitialized)
            {
                EB.Debug.LogError("YiJiSDK.SetRoleData: not initialized");
                return;
            }

            User curUser = Hub.LoginManager.LocalUser;
            if (curUser == null)
            {
                return;
            }
            var worlds = Hub.LoginManager.GameWorlds;
            var world = System.Array.Find(worlds, w => w.Id == curUser.WorldId);
            string worldName = world != null ? world.Name : (worlds.Length > 0 ? worlds[0].Name : "Default");
            YiJieSDK.pxSetRoleData(curUser.Id.Value.ToString(), curUser.Name, curUser.Level.ToString(), curUser.WorldId.ToString(), worldName);
        }

        public void SetData(string dataType)
        {
            User curUser = Hub.LoginManager.LocalUser;
            EB.Debug.Log("YiJiSDK.SetData curUser = {0} datatype = {1}", curUser, dataType);
            if (curUser == null)
            {
                Hashtable nullGameInfo = new Hashtable();
                nullGameInfo.Add("roleId", 1);
                nullGameInfo.Add("roleName", "Default");
                nullGameInfo.Add("roleLevel", 1);
                nullGameInfo.Add("zoneId", 1);
                nullGameInfo.Add("zoneName", "Default");
                nullGameInfo.Add("balance", "0");
                nullGameInfo.Add("vip", "1");
                nullGameInfo.Add("partyName", EB.Localizer.GetString("ID_LABEL_NAME_NONE") + EB.Localizer.GetString("ID_ALLIANCE"));
                nullGameInfo.Add("roleCTime", "-1");
                nullGameInfo.Add("roleLevelMTime", "-1");
                YiJieSDK.pxSetData(dataType, JSON.Stringify(nullGameInfo));
                return;
            }
            var worlds = Hub.LoginManager.GameWorlds;
            var world = System.Array.Find(worlds, w => w.Id == curUser.WorldId);
            string worldName = world != null ? world.Name : (worlds.Length > 0 ? worlds[0].Name : "Default");

            Hashtable gameInfo = new Hashtable();
            gameInfo.Add("roleId", curUser.Id.Value > 0 ? curUser.Id.Value : 1);
            gameInfo.Add("roleName", string.IsNullOrEmpty(curUser.Name) ? "Default" : curUser.Name);
            gameInfo.Add("roleLevel", curUser.Level > 0 ? curUser.Level : 1);
            gameInfo.Add("zoneId", world.Id > 0 ? world.Id : 1);
            gameInfo.Add("zoneName", worldName);
            gameInfo.Add("balance", "0");
            gameInfo.Add("vip", "1");
            gameInfo.Add("partyName", EB.Localizer.GetString("ID_LABEL_NAME_NONE") + EB.Localizer.GetString("ID_ALLIANCE"));
            gameInfo.Add("roleCTime", "-1");
            gameInfo.Add("roleLevelMTime", "-1");
            YiJieSDK.pxSetData(dataType, JSON.Stringify(gameInfo));
        }

        public void ExitSDK(System.Action<int> callback)
        {
            if (mInitialized)
            {
                Hub.RunInBackground = true;
                mExitSDKCallback += callback;
                YiJieSDK.pxExitSDK();
            }
        }

        public void OnExitSDKCallback(int code)
        {
            Hub.RunInBackground = false;
            if (mExitSDKCallback != null)
            {
                mExitSDKCallback(code);
            }
        }
    }

    public class SparxYiJieSDKManager : MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating YiJie Manager");
            DontDestroyOnLoad(gameObject);
        }

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxYiJieSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus && SparxHub.Instance != null)
            {
                SparxHub.Instance.YiJieSDKManager.OnLoginEnteredForeground();
            }
        }

        public void OnYiJieGameSDKCallback(string jsonString)
        {
            EB.Debug.Log("SparxYiJieSDKManager.OnYiJieGameSDKCallback:jsonString = {0}", jsonString);
            object json = EB.JSON.Parse(jsonString);
            string callbackType = Dot.String("callbackType", json, string.Empty);
            int code = Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case YiJieSDKCallbackType.CALLBACKTYPE_INITSDK:
                    Sparx.Hub.Instance.YiJieSDKManager.OnInitCallback(code);
                    break;

                case YiJieSDKCallbackType.CALLBACKTYPE_LOGIN:
                    Sparx.Hub.Instance.YiJieSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
                    break;

                case YiJieSDKCallbackType.CALLBACKTYPE_PAY:
                    Sparx.Hub.Instance.YiJieSDKManager.OnPayCallback(code);
                    break;

                case YiJieSDKCallbackType.CALLBACKTYPE_EXIT:
                    Sparx.Hub.Instance.YiJieSDKManager.OnExitSDKCallback(code);
                    break;
            }
        }
    }
}
#endif