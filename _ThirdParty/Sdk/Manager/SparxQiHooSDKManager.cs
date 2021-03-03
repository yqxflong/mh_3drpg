#if UNITY_ANDROID && USE_QIHOOSDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class QiHooSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

		public const string CALLBACKTYPE_SWITCH = "Switch";

		public const string CALLBACKTYPE_LOGOUT = "Logout";

        public const string CALLBACKTYPE_PAY = "Pay";

        public const string CALLBACKTYPE_QUITSDK = "QuitSDK";

    }

    public class QiHooPayStatusCode
    {
        //支付成功
        public const int SUCCESS = 0;

        //支付失败
        public const int FAIL = 1;

        //支付取消
        public const int CANCEL = -1;

        //正在支付
        public const int PAYING = -2;

        //accessToken已失效
        public const int ACCESS_TOKEN_INVAILD = 4010201;

        //QT已失效
        public const int QT_INVAILD = 4009911;

        public const int CALLBACK_TIMEOUT = -1000;
    }

    public class QiHooGameSDK
    {
        private const string SDK_JAVA_CLASS = "cn.qihoo.gamesdk.unity3d.QiHooGameSDK";

        public static void InitSDK()
        {
            callSdkApi("InitSDK");
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void SubmitUserInfo(string jsonString)
        {
            callSdkApi("SubmitUserInfo", jsonString);
        }

        public static void Logout()
        {
            callSdkApi("Logout");
        }

        public static void Pay(string userId, string amount, string productName, string productId, string uri, string playerName, string playerId, string orderId
			,int num,string serverId,string serverName,int rate,string moneyName,int grade,int balance,string vip,string aName )
        {
            callSdkApi("Pay", userId, amount, productName, productId, uri, playerName, playerId, orderId,
				num,serverId,serverName,rate,moneyName,playerId,playerName,grade,balance,vip,aName);
        }

        public static void QuitSDK()
        {
            callSdkApi("QuitSDK");
        }

        public static void DestorySDK()
        {
            callSdkApi("DestorySDK");
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

    public class QiHooSDKManager : Manager
    {
        private bool mInitialized = false;
        private bool mInitializing = false;
        private bool mLogined = false;

        private string mNotifyUrl;

        private System.Action<string, bool> mInitCallback;
        private System.Action<string, object> mLoginCallback;
        private System.Action<int> mPayCallback;
        private System.Action<bool> mQuitCallback;
        private System.Action mDestroyCallback;

        public bool IsLoggedIn { get { return mLogined; } }

        public override void Initialize(Config config)
        {

        }

        public override void OnLoggedIn()
        {

        }

		public void SubmitUserInfoToQihoo(string type,int power)
		{
			var user = Hub.LoginManager.LocalUser;
			if(user==null)
			{
				return;
			}
			var worlds = Hub.LoginManager.GameWorlds;
			var world = System.Array.Find(worlds, w => w.Id == user.WorldId);
			string worldName = world != null ? world.Name : (worlds.Length > 0 ? worlds[0].Name : "Default");
			Hashtable loginGameRole = new Hashtable();
			loginGameRole["type"] = type;
			loginGameRole["zoneid"] = user.WorldId;
			loginGameRole["zonename"] = worldName;
			loginGameRole["roleid"] = user.Id.Value.ToString();
			loginGameRole["rolename"] = user.Name;
			loginGameRole["professionid"] = user.CharacterId;
			loginGameRole["profession"] = user.CharacterId.ToString();
			loginGameRole["gender"] = "无";
			loginGameRole["rolelevel"] = user.Level;
			loginGameRole["power"] = power;
			loginGameRole["vip"] = user.Vip;
			loginGameRole["partyid"] = 0;
			loginGameRole["partyname"] = "无";
			loginGameRole["partyroleid"] = 0;
			loginGameRole["partyrolename"] = "无";
			loginGameRole["friendlist"] = "无";
			EB.Debug.Log("SubmitUserInfoToQihoo type:{0} power:{1}", type, power);
			SubmitUserInfo(EB.JSON.Stringify(loginGameRole));
		}

        public override void OnEnteredForeground()
        {
            //if (mLoginCallback != null)
            //{
            //	EB.Coroutines.SetTimeout(delegate ()
            //	{
            //		if (mLoginCallback != null)
            //		{
            //			mLoginCallback(null, null);
            //			mLoginCallback = null;
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
                        mPayCallback(QiHooPayStatusCode.CALLBACK_TIMEOUT);
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

        public void InitSDK(object options, System.Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.LogWarning("QiHooSDKManager.InitializeSDK: Initialized");
                callback(null, true);
                return;
            }

            if (Application.platform != RuntimePlatform.Android)
            {
                callback(null, false);
                return;
            }
            mNotifyUrl = EB.Dot.String("notifyUrl", options, mNotifyUrl);
            EB.Debug.Log("QiHooSDKManager.InitSDK notifyUrl= {0}", mNotifyUrl);
            mInitCallback += callback;
            if (!mInitializing)
            {
                mInitializing = true;

                Hub.RunInBackground = true;

                new GameObject("qihoo_plugin_listener", typeof(SparxQiHooSDKManager));
                QiHooGameSDK.InitSDK();
            }
        }

        public void OnInitResult()
        {
            EB.Debug.Log("QiHooSDK.OnInitResult");
            mInitializing = false;
            Hub.RunInBackground = false;
            mInitialized = true;
            if (mInitCallback != null)
            {
                mInitCallback(null, mInitialized);
                mInitCallback = null;
            }
        }

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("QiHooSDKManager.Login");
            if (!mInitialized)
            {
                callback("QiHooSDK has not been inited", null);
                return;
            }

            mLoginCallback += callback;

            Hub.RunInBackground = true;
            QiHooGameSDK.Login();
        }

        public void OnLoginResult(int code, string msg)
        {
            EB.Debug.Log("QiHooSDKManager.OnLoginResult: code = {0} msg={1}", code, msg);
            Hub.RunInBackground = false;
            if (mLoginCallback != null)
            {
                if (code == 0)
                {
                    mLogined = true;
                    Hashtable data = new Hashtable();
                    data.Add("access_token", msg);
                    mLoginCallback(null, data);
                    mLoginCallback = null;
                }
                else
                {
                    mLoginCallback(null, null);  //"user canceled login"
                    mLoginCallback = null;
                }
            }
        }

		private static string access_token=null;
		public void OnSwitchResult(int code, string msg)
		{
			EB.Debug.Log("QiHooSDKManager.OnSwitchResult: code = {0} msg={1}", code, msg);
			Hub.RunInBackground = false;
			if (code == 0)
			{
				mLogined = true;
				access_token = msg;
                SparxHub.Instance.Disconnect(true);
			}
		}

		public void SubmitUserInfo(string jsonString)
        {
            EB.Debug.Log("QiHooSDKManager.SubmitUserInfo:jsonString={0}", jsonString);
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!mInitialized)
                {
                    EB.Debug.LogError("QiHooSDKManager.SubmitUserData: not initialized");
                    return;
                }

                QiHooGameSDK.SubmitUserInfo(jsonString);
            }
        }

        public void Logout()
        {
            if (mLogined)
            {
                EB.Debug.Log("QiHooSDKManager.Logout");
                QiHooGameSDK.Logout();
            }
        }

        public void OnLogout()
        {
            if (mLogined)
            {
                mLogined = false;
                Hub.Instance.Disconnect(true);
            }
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback)
        {
            EB.Debug.Log("QiHooSDKManager.Pay={0}");
            if (!mInitialized)
            {
                EB.Debug.LogError("QiHooSDKManager.Pay: not initialized");
                callback(QiHooPayStatusCode.FAIL);
                return;
            }

            mPayCallback += callback;
            var user = Hub.Instance.LoginManager.LocalUser;
			object obj= EB.JSON.Parse(transaction.payload);
			string servername = EB.Dot.String("serverName",obj,"111");
			int balance = EB.Dot.Integer("balance", obj, 1000);
			string aname = EB.Dot.String("aName", obj, "111");
			EB.Debug.Log(servername+ balance+aname);
			QiHooGameSDK.Pay(GetUserId(), item.cents.ToString(), item.longName, item.productId, mNotifyUrl, user.Name, user.Id.ToString(), transaction.transactionId
				,1,user.WorldId.ToString(), servername, 10,"钻石",user.Level, balance, user.Vip.ToString(), aname);
        }

        public void OnPayCallback(string msg)
        {
            EB.Debug.Log("QiHooSDKManager.OnPayCallback: msg = {0}", msg);
            if (mPayCallback != null)
            {
                mPayCallback(int.Parse(msg));
                mPayCallback = null;
            }
        }

        public void QuitSDK(System.Action<bool> callback)
        {
            if (mInitialized)
            {
                mQuitCallback = callback;
                QiHooGameSDK.QuitSDK();
            }
        }

        public void OnQuitSDK(int code)
        {
            if (mQuitCallback != null)
            {
                if (code == 1)
                {
                    mQuitCallback(false);
                }
                else
                {
                    mQuitCallback(true);
                }
                mQuitCallback = null;
            }
        }

        private string GetUserId()
        {	
            string guid = string.Empty;
            foreach (AuthData authData in Hub.Instance.LoginManager.Account.Authenticators)
            {
                if (authData.Authenticator != null)
                {
                    if (authData.Authenticator.Name == "qihoo")
                    {
                        guid = authData.Id;
                        break;
                    }
                }
            }
            return guid;
        }
    }

    public class SparxQiHooSDKManager : MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating QiHoo Manager");
            DontDestroyOnLoad(gameObject);
        }

        public void OnQiHooGameSdkCallback(string jsonstr)
        {
            EB.Debug.Log("OnQiHooGameSdkCallback message: jsonstr = {0}", jsonstr);

            object json = EB.JSON.Parse(jsonstr);
            string callbackType = EB.Dot.String("callbackType", json, string.Empty);
            int code = EB.Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case EB.Sparx.QiHooSDKCallbackType.CALLBACKTYPE_INITSDK:
                    SparxHub.Instance.QiHooSDKManager.OnInitResult();
                    break;

                case EB.Sparx.QiHooSDKCallbackType.CALLBACKTYPE_LOGIN:
                    SparxHub.Instance.QiHooSDKManager.OnLoginResult(code, EB.Dot.String("data", json, null));
                    break;

				case EB.Sparx.QiHooSDKCallbackType.CALLBACKTYPE_SWITCH:
					SparxHub.Instance.QiHooSDKManager.OnSwitchResult(code, EB.Dot.String("data", json, null));
					break;

				case EB.Sparx.QiHooSDKCallbackType.CALLBACKTYPE_LOGOUT:
                    SparxHub.Instance.QiHooSDKManager.OnLogout();
                    break;

                case EB.Sparx.QiHooSDKCallbackType.CALLBACKTYPE_PAY:
                    SparxHub.Instance.QiHooSDKManager.OnPayCallback(EB.Dot.String("data", json, null));
                    break;

                case EB.Sparx.QiHooSDKCallbackType.CALLBACKTYPE_QUITSDK:
                    SparxHub.Instance.QiHooSDKManager.OnQuitSDK(code);
                    break;

                default:
                    EB.Debug.LogWarning("OnQiHooGameSdkCallback: unknown type = {0}", callbackType);
                    break;
            }
        }

        void OnApplicationQuit()
        {
            QiHooGameSDK.DestorySDK();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("QiHooSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus)
            {
                SparxHub.Instance.QiHooSDKManager.OnLoginEnteredForeground();
            }
        }
    }
}
#endif
