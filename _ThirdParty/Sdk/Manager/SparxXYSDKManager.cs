#if USE_XYSDK && UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace EB.Sparx
{
    public class XYSDKManager : Manager
    {
        [DllImport("__Internal")]
        private static extern void XYSDKInit(int appId, string appKey, string appScheme);
        //Init
        public void pxXYInit(int appId, string appKey, string appScheme)
        {
            if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
            {
                XYSDKInit(appId, appKey, appScheme);
            }
        }

        [DllImport("__Internal")]
        private static extern void XYLogin();
        //Login
        public void pxXYLogin()
        {
            if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
            {
                XYLogin();
            }
        }

        [DllImport("__Internal")]
        private static extern void XYLogout();
        //Logout
        public void pxXYLogout()
        {
            if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
            {
                XYLogout();
            }
        }

        [DllImport("__Internal")]
        private static extern bool XYIsLogin();
        //IsLogined
        public bool pxXYIsLogined()
        {
            if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
            {
                return XYIsLogin();
            }
            else
            {
                return false;
            }
        }

        [DllImport("__Internal")]
        private static extern void XYPay(int amount, int serverId, string extra);
        //Pay
        public void pxXYPay(int amount, int serverId, string extra)
        {
            if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
            {
                XYPay(amount, serverId, extra);
            }
        }

        private bool isInited = false;
        private System.Action<string, bool> initCallBack;
        private System.Action<string, object> loginCallBack;
        private System.Action<string, string> payResultCallBack;

        public override void Initialize(Config config)
        {

        }

        public void InitSDK(object options, System.Action<string, bool> callback)
        {
            EB.Debug.Log("XYSDKManager.InitSDK");
#if UNITY_EDITOR
            isInited = true;
            callback(null, isInited);
#elif UNITY_IPHONE
            if (isInited)
            {
                callback(null, true);
            }
            else
            {
                new GameObject("xy_callback", typeof(SparxXYSDKManager));
                int appId = Dot.Integer("appId", options, 100030624);
                string appKey = Dot.String("appKey", options, "N3jupiNhcJEeBTi3cVmr8Vj4TGtRQISw");
                pxXYInit(appId, appKey, Application.bundleIdentifier + ".alipay");
                initCallBack = callback;
            }
#else
            isInited = false;
            callback(null, isInited);
#endif
        }

        public void Login(System.Action<string, object> callback)
        {
            EB.Debug.Log("XYSDKManager.Login");
#if UNITY_EDITOR
            callback("XYSDK not support in Editor", null);
#elif UNITY_IPHONE
            if (!isInited)
            {
                callback("XYSDK has not been inited", null);
                return;
            }

            pxXYLogin();
            loginCallBack = callback;
#else
            callback("XYSDK not support on this platform", null);
#endif
        }

        public void Logout()
        {
            pxXYLogout();
        }

        public bool IsInited()
        {
            return isInited;
        }

        public bool IsLoggedIn()
        {
            return pxXYIsLogined();
        }

        public void OnXYInit()
        {
            EB.Debug.Log("XYSDKManager.OnXYInit");
            if (initCallBack != null)
            {
                isInited = true;
                initCallBack(null, isInited);
                initCallBack = null;
            }
        }

        public void OnXYLogin(string userInfo)
        {
            EB.Debug.Log("XYSDKManager.OnXYLogin userInfo = {0}", userInfo);            
            if (!string.IsNullOrEmpty(userInfo))
            {
                var data = JSON.Parse(userInfo);
                EB.Debug.Log("XYSDKManager.OnXYLogin data = {0}", data);
                Hashtable auth = new Hashtable();
                auth["uid"] = Dot.String("uid", data, null);
                auth["appid"] = "100030624";
                auth["token"] = Dot.String("token", data, null);
                EB.Debug.Log("XYSDKManager.OnXYLogin auth[uid] = {0}", auth["uid"]);
                EB.Debug.Log("XYSDKManager.OnXYLogin auth[appid] = {0}", auth["appid"]);
                EB.Debug.Log("XYSDKManager.OnXYLogin auth[token] = {0}", auth["token"]);
                if (loginCallBack != null)
                {
                    loginCallBack(null, auth);
                    loginCallBack = null;
                }
            }
            else
            {
                if (loginCallBack != null)
                {
                    loginCallBack("LoginFailed", null);
                    loginCallBack = null;
                }
            }
        }


        public void Pay(EB.IAP.Item item,string transactionId, System.Action<string, string> callback)
        {
            if (!IsInited())
            {
                EB.Debug.LogWarning("XYSDKManager.Pay: XYSDK not inited");
                return;
            }
            payResultCallBack += callback;
            int worldId = EB.Sparx.Hub.Instance.LoginManager.LocalUser.WorldId;
            pxXYPay((int)item.cost, worldId, transactionId);
        }

        public void OnXYPay(string payResultInfo)
        {
            EB.Debug.Log("XYSDKManager.OnXYPay payResultInfo = {0}", payResultInfo);
            if (!string.IsNullOrEmpty(payResultInfo))
            {
                var data = JSON.Parse(payResultInfo);
                EB.Debug.Log("XYSDKManager.OnXYPay data = {0}", data);
                string resultCode = Dot.String("resultCode", data, null);
                string orderId = Dot.String("orderId", data, null);
                EB.Debug.Log("XYSDKManager.OnXYLogin resultCode = {0}", resultCode);
                EB.Debug.Log("XYSDKManager.OnXYLogin orderId = {0}", orderId);
                if (payResultCallBack != null)
                {
                    payResultCallBack(resultCode, orderId);
                    payResultCallBack = null;
                }
            }
        }

        public void OnXYLogout()
        {
            Hub.Instance.Disconnect(true);
        }
    }

    class SparxXYSDKManager : UnityEngine.MonoBehaviour
    {
        void Awake()
        {
            EB.Debug.Log("Creating XY SDKManager");
            DontDestroyOnLoad(gameObject);
        }

        public void OnXYInit(string msg)
        {
            EB.Debug.Log("OnXYInit");
            Hub.Instance.XYSDKManager.OnXYInit();
        }

        public void OnXYLogin(string userInfo)
        {
            EB.Debug.Log("OnXYLogin: {0}", userInfo);
            Hub.Instance.XYSDKManager.OnXYLogin(userInfo);
        }

        public void OnXYPay(string payResultInfo)
        {
            EB.Debug.Log("OnXYPay: {0}", payResultInfo);
            Hub.Instance.XYSDKManager.OnXYPay(payResultInfo);
        }

        public void OnXYLogout(string msg)
        {
            EB.Debug.Log("OnXYLogout");
            Hub.Instance.XYSDKManager.OnXYLogout();
        }
    }
}
#endif