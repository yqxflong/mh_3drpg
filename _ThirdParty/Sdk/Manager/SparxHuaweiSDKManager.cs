#if UNITY_ANDROID && USE_HUAWEISDK
using UnityEngine;
using System.Collections;
using System;

namespace EB.Sparx
{
    public class HuaweiSDKCallbackType
    {
        public const string CALLBACKTYPE_INITSDK = "InitSDK";

        public const string CALLBACKTYPE_LOGIN = "Login";

        public const string CALLBACKTYPE_PAY = "Pay";

    }

    public class HuaweiResultCode
    {
        public const int RESULT_OK = 0;

        public const int RESULT_ERR_SIGN = 1;

        public const int RESULT_ERR_COMM = 2;

        public const int RESULT_ERR_UNSUPPORT = 3;

        public const int RESULT_ERR_PARAM = 4;

        public const int RESULT_ERR_NETWORK = 5;

        public const int RESULT_ERR_AUTH = 6;

        public const int RESULT_ERR_CANCEL = 7;

        public const int RESULT_ERR_NOT_INIT = 8;

        public const string IS_AUTH_0 = "0";

        public const string IS_AUTH_1 = "1";

        public const string IS_CHANGE = "1";

    }

    public class HuaweiPayReturnCode
    {
        public const string USER_CANCEL = "3002";

        public const string CALLBACK_TIMEOUT = "-1000";
    }

    public class HuaweiSDK
    {
        private const string SDK_JAVA_CLASS = "cn.huawei.gamesdk.unity3d.HuaweiGameSDK";

        public static void InitSDK(string appId, string payId, string buoyPrivateKey)
        {
            callSdkApi("InitSDK", appId, payId, buoyPrivateKey);
        }

        public static void CheckUpdate()
        {
            callSdkApi("CheckUpadte");
        }

        public static void Login()
        {
            callSdkApi("Login");
        }

        public static void ShowFloatWindow()
        {
            callSdkApi("ShowFloatWindow");
        }

        public static void HideFloatWindow()
        {
            callSdkApi("HideFloatWindow");
        }

        public static void SetPlayerInfo(string level,string name,string areaName,string sociatyName)
        {
            callSdkApi("SetPlayerInfo", level, name, areaName, sociatyName);
        }

        public static void Pay(string appId, string payId, string userName, string amount, string productName, string requestId, string productDesc, string payPrivateKey)
        {
            callSdkApi("Pay", appId, payId, userName, amount, productName, requestId, productDesc, payPrivateKey);
        }

        public static void DestorySDK()
        {
            callSdkApi("DestorySDK");
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

    public class HuaweiSDKManager : Manager
    {
        private bool mInitialized = false;
        private string appId = string.Empty;
        private string payId = string.Empty;
        private string buoyPrivateKey = string.Empty;
        private string curPlayerId = string.Empty;
        //private string buoyPrivateKey = string.Empty;
        private Action<string, bool> mInitCallback;
		
        private bool mLogined = false;
		private static bool mSDKLogout = false;
		private static Hashtable mLoginData;
		private System.Action<string, object> mLoginCallback;

        //private string privateKey = string.Empty;
        private System.Action<string, object> mPayCallback;

        private HttpEndPoint endPoint = null;
        private LoginAPI api = null;

        public bool IsLoggedIn { get { return mLogined; } }

        public override void Initialize(Config config)
        {
            endPoint = new HttpEndPoint(config.ApiEndpoint, new EndPointOptions()
            {
                Key = config.ApiKey.Value,
                KeepAlive = false
            });

            api = new LoginAPI(endPoint);
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
                        data.Add("returnCode", HuaweiPayReturnCode.CALLBACK_TIMEOUT);
                        mPayCallback(null, data);
                        mPayCallback = null;
                    }
                }, 5 * 1000);
            }
        }

        public void OnLoginEnteredForeground()
        {
            if (mLoginCallback != null)
            {
                EB.Coroutines.SetTimeout(delegate ()
                {
                    if (mLoginCallback != null)
                    {
                        HuaweiSDK.DestorySDK();
                        mInitialized = false;
                        mInitCallback = null;
                        mLogined = false;
                        HuaweiSDK.InitSDK(appId, payId, buoyPrivateKey);
                        System.Action<string, object> callback = mLoginCallback;
                        EB.Debug.Log("HuaweiSDKManager.OnLogin OnLoginEnteredForeground");
                        mLoginCallback = null;
                        callback(null, null);
                    }
                }, 1 * 1000);
            }
        }

        public override void OnLoggedIn()
        {
            SetPlayerInfo();

            Hub.RunInBackground = false;
        }

        public void InitSDK(object initData, Action<string, bool> callback)
        {
            if (mInitialized)
            {
                EB.Debug.Log("HuaweiSDKManager.InitSDK: Initialized");
                callback(null, true);
                return;
            }
            if (Application.platform != RuntimePlatform.Android)
            {
                callback(null, false);
                return;
            }
            EB.Debug.Log("HuaweiSDKManager.InitSDK: initData = {0}", JSON.Stringify(initData));
            appId = EB.Dot.String("appId", initData, string.Empty);
            payId = EB.Dot.String("payId", initData, string.Empty);
            buoyPrivateKey = EB.Dot.String("buoyPrivateKey", initData, string.Empty);
            EB.Debug.Log("HuaweiSDKManager.InitSDK: appId = {0}", appId);
            EB.Debug.Log("HuaweiSDKManager.InitSDK: payId = {0}", payId);
            EB.Debug.Log("HuaweiSDKManager.InitSDK: buoyPrivateKey = {0}", buoyPrivateKey);
            mInitCallback += callback;
            Hub.RunInBackground = true;
            new GameObject("huawei_plugin_listener", typeof(SparxHuaweiSDKManager));
            HuaweiSDK.InitSDK(appId, payId, buoyPrivateKey);
        }

        public void OnInitSDKCallback(int code)
        {
            Hub.RunInBackground = false;
            if (code == HuaweiResultCode.RESULT_OK)
            {
                mInitialized = true;
            }
            string err = string.Empty;
            if (code == HuaweiResultCode.RESULT_ERR_UNSUPPORT)
            {
                err = "ID_SPARX_HUAWEI_UNSUPPORT";
            }

            if (mInitCallback != null)
            {
                if (string.IsNullOrEmpty(err))
                {
                    mInitCallback(null, mInitialized);
                }
                else
                {
                    mInitCallback(err, mInitialized);
                }
                mInitCallback = null;
            }
        }

        public void Login(Action<string, object> callback)
        {
            EB.Debug.Log("HuaweiSDKManager.Login");
            if (!mInitialized)
            {
                callback("TencentSDK has not been inited", null);
                return;
            }

			//EB.Debug.Log("HuaweiSDKManager.Login 0");
			//EB.Debug.Log("HuaweiSDKManager.Login 1");

			mLoginCallback += callback;
			Hub.RunInBackground = true;

			if (!mSDKLogout)
			{
				HuaweiSDK.Login();
				HuaweiSDK.CheckUpdate();
			}
			else
			{
				mSDKLogout = false;
				mLogined = true;
				if (mLoginCallback != null)
				{
					mLoginCallback(null, mLoginData);
					mLoginCallback = null;
				}
			}
        }

        public void OnLoginCallback(int code, object data)
        {
            EB.Debug.Log("HuaweiSDKManager.OnLoginResult:code={0} data = {1}", code, data);
            //Hub.RunInBackground = false;

            if (code == HuaweiResultCode.RESULT_ERR_NOT_INIT)
            {
                if (mLoginCallback != null)
                {
                    mLoginCallback("have not init sdk", null);
                    //EB.Debug.Log("HuaweiSDKManager.OnLogin RESULT_ERR_NOT_INIT 0");
                    mLoginCallback = null;
                    //EB.Debug.Log("HuaweiSDKManager.OnLogin RESULT_ERR_NOT_INIT 1");
                }
                return;
            }

            if(code != HuaweiResultCode.RESULT_OK)
            {
                if (mLoginCallback != null)
                {
                    mLoginCallback(null, null);
                    //EB.Debug.Log("HuaweiSDKManager.OnLogin !RESULT_OK 0");
                    mLoginCallback = null;
                    //EB.Debug.Log("HuaweiSDKManager.OnLogin !RESULT_OK 1");
                }
                return;
            }

            string isAuth = Dot.String("isAuth", data, null);

            if (isAuth != null)
            {
                if (isAuth == HuaweiResultCode.IS_AUTH_0)
                {
                    string playerId = Dot.String("playerId", data, string.Empty);
                    mLoginData = new Hashtable();
					mLoginData.Add("playerId", playerId);
					mLoginData.Add("isAuth", isAuth);
                    if (mLogined && curPlayerId != playerId)
                    {
                        EB.Debug.Log("HuaweiSDKManager OnChange Success");
						mSDKLogout = true;
                        Sparx.Hub.Instance.Disconnect(true);
                        mLogined = false;
                    }
                    else
                    {
                        if (mLoginCallback != null)
                        {
                            mLoginCallback(null, mLoginData);
                            //EB.Debug.Log("HuaweiSDKManager.OnLogin IS_AUTH_0 0");
                            mLoginCallback = null;
                            //EB.Debug.Log("HuaweiSDKManager.OnLogin IS_AUTH_0 1");
                        }
                        mLogined = true;
                    }
                }
                else if (isAuth == HuaweiResultCode.IS_AUTH_1)
                {
                    EB.Debug.Log("HuaweiSDKManager.OnLogin Success");
                    string playerId = Dot.String("playerId", data, string.Empty);
					curPlayerId = playerId;
                    string displayName = Dot.String("displayName", data, string.Empty);
                    string gameAuthSign = Dot.String("gameAuthSign", data, string.Empty);
                    string ts = Dot.String("ts", data, string.Empty);
                    var loginData = new Hashtable();
                    loginData.Add("playerId", playerId);
                    loginData.Add("displayName", displayName);
                    loginData.Add("isAuth", isAuth);
                    loginData.Add("gameAuthSign", gameAuthSign);
                    loginData.Add("ts", ts);
                    //if (mLoginCallback != null)
                    //{
                    //    EB.Debug.Log("HuaweiSDKManager.OnLogin callback success");
                    //    mLoginCallback(null, loginData);
                    //    mLoginCallback = null;
                    //}
                    //else
                    //{
                    //    EB.Debug.Log("HuaweiSDKManager.OnLogin callback failed");
                    //}
                    EB.Coroutines.Run(AuthenticateParallelCoroutine(loginData));
                }
            }
            else
            {
                string isChange = Dot.String("isChange", data, string.Empty);
                if (!string.IsNullOrEmpty(isChange) && isChange == HuaweiResultCode.IS_CHANGE)
                {
                    EB.Debug.Log("HuaweiSDKManager OnChange Success");
                    Sparx.Hub.Instance.Disconnect(true);
                    mLogined = false;
                }
            }
        }

        IEnumerator AuthenticateParallelCoroutine(object authData)
        {
            while (Hub.LoginManager.Account == null)
            {
                yield return null;
            }

            yield return EB.Coroutines.Run(AuthenticateParallel(authData));
        }

		public void Logout()
		{
			if (mLogined)
			{
				mLogined = false;
				curPlayerId = string.Empty;
			}
		}

        public void SetPlayerInfo()
        {
            var user = Hub.Instance.LoginManager.LocalUser;
            var worlds = Hub.LoginManager.GameWorlds;
            var world = System.Array.Find(worlds, w => w.Id == user.WorldId);
            string worldName = world != null ? world.Name : (worlds.Length > 0 ? worlds[0].Name : "Default");
            //string sociatyName = EB.Localizer.GetString("ID_LABEL_NAME_NONE") + EB.Localizer.GetString("ID_ALLIANCE");
            HuaweiSDK.SetPlayerInfo(user.Level.ToString(), user.Name, worldName, "");
        }

        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, Action<string, object> callback)
        {
            EB.Debug.Log("HuaweiSDKManager.Pay");
            if (!mInitialized)
            {
                EB.Debug.LogError("HuaweiSDKManager.Pay: not initialized");
                callback("have not init sdk", null);
                return;
            }

            var extraInfo = JSON.Parse(transaction.payload);
            EB.Debug.Log("HuaweiSDKManager extraInfo = {0}", transaction.payload);

            string payPrivateKey = EB.Dot.String("privateKey", extraInfo, string.Empty);
            EB.Debug.Log("HuaweiSDKManager payPrivateKey = {0}", payPrivateKey);

            mPayCallback += callback;
            var user = Hub.Instance.LoginManager.LocalUser;
            string cost = item.cost.ToString("#0.00");
            EB.Debug.Log("HuaweiSDKManager cost = {0}", cost);
            HuaweiSDK.Pay(appId, payId, user.Name, cost, item.longName, transaction.transactionId, item.localizedDesc, payPrivateKey);
        }

        public void OnPayCallback(object data)
        {
            EB.Debug.Log("HuaweiSDKManager.OnPayCallback: code = {0}", data);
            if (mPayCallback != null)
            {
                mPayCallback(null, data);
                mPayCallback = null;
            }
        }

        public void OnSDKDestory()
        {
            mInitialized = false;
            appId = string.Empty;
            payId = string.Empty;
            //buoyPrivateKey = string.Empty;
            mInitCallback = null;

            mLogined = false;
            mLoginCallback = null;


            //privateKey = string.Empty;
            mPayCallback = null;
        }

        private IEnumerator AuthenticateParallel(object authData)
        {
            endPoint.AddData("stoken", Hub.ApiEndPoint.GetData("stoken"));

            bool? prelogin = null;
            object preloginData = null;

            EB.Debug.Log("HuaweiSDKManager.Authenticate: Start PreLogin");
            api.PreLogin(delegate (string preloginError, object preloginResult)
            {
                if (!string.IsNullOrEmpty(preloginError))
                {
                    prelogin = false;

                    var url = Dot.String("url", preloginResult, null);
                    if (!string.IsNullOrEmpty(url))
                    {
                        Hub.Config.LoginConfig.Listener.OnUpdateRequired(url);
                        return;
                    }
                    else
                    {
                        Hub.FatalError(preloginError);
                        return;
                    }
                }

                prelogin = true;
                preloginData = preloginResult;
                EB.Debug.Log("HuaweiSDKManager.Authenticate: PreLogin Success");
            });

            while (!prelogin.HasValue)
            {
                yield return null;
            }

            if (!prelogin.Value)
            {
                yield break;
            }

            var salt = Dot.String("salt", preloginData, string.Empty);
            yield return Coroutines.Run(Protect.CalculateHmac(salt, delegate (string challenge)
            {
                var securityData = new Hashtable();
                securityData["salt"] = salt;
                securityData["chal"] = challenge;

                EB.Debug.Log("HuaweiSDKManager.Authenticate: Start Login");
                api.Login("huawei", new Hashtable() { { "huawei", authData } }, securityData, delegate (string loginError, object loginResult)
                {
                    if (!string.IsNullOrEmpty(loginError))
                    {
                        Hub.FatalError(loginError);
                        return;
                    }

                    EB.Debug.Log("HuaweiSDKManager.Authenticate: Login success");
                });
            }));
        }
    }

    public class SparxHuaweiSDKManager : MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating Huawei Manager");
            DontDestroyOnLoad(gameObject);
        }

        public void OnHuaweiGameSDKCallback(string jsonString)
        {
            EB.Debug.Log("SparxHuaweiSDKManager.OnHuaweiGameSDKCallback:jsonString = {0}", jsonString);
            object json = EB.JSON.Parse(jsonString);
            string callbackType = Dot.String("callbackType", json, string.Empty);
            int code = Dot.Integer("code", json, -1);
            switch (callbackType)
            {
                case HuaweiSDKCallbackType.CALLBACKTYPE_INITSDK:
                    Sparx.Hub.Instance.HuaweiSDKManager.OnInitSDKCallback(code);
                    break;

                case HuaweiSDKCallbackType.CALLBACKTYPE_LOGIN:
                    Sparx.Hub.Instance.HuaweiSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
                    break;

                case HuaweiSDKCallbackType.CALLBACKTYPE_PAY:
                    Sparx.Hub.Instance.HuaweiSDKManager.OnPayCallback(Dot.Object("data", json, null));
                    break;
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                EB.Debug.Log("HuaweiSDKManager.OnEnteredBackground");
                HuaweiSDK.HideFloatWindow();
            }
            else
            {
                EB.Debug.Log("HuaweiSDKManager.OnEnteredForeground");
                HuaweiSDK.ShowFloatWindow();
                SparxHub.Instance.HuaweiSDKManager.OnLoginEnteredForeground();
            }
        }

        void OnApplicationQuit()
        {
            HuaweiSDK.DestorySDK();
            Hub.Instance.HuaweiSDKManager.OnSDKDestory();
        }
    }
}
#endif
