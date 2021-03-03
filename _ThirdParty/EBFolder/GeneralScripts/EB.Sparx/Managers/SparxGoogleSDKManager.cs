#if USE_GOOGLE && UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using EB.IAP;
using EB.IAP.Internal;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace EB.Sparx
{

    public class GoogleSDKManager : Manager
    {
        public bool isLogin = false;

        //Google Play Client配置，提前配置(Windows/Google Game Play/ AndroidSetting)
        public static PlayGamesClientConfiguration clientConfig;

        /// <summary>
        /// Google登录成功返回的TokenId
        /// </summary>
        public string m_TokenID;

        /// <summary>
        /// Google登陆成功返回的AuthCode
        /// </summary>
        public string m_AuthCode;

        /// <summary>
        /// SDK是否初始化
        /// </summary>
        public bool mInitialized = false;

        /// <summary>
        /// Google支付
        /// </summary>
        public SparxGoogleSDKManagerMono mGoogleStore;

        private System.Action<string, object> onLoginCallBack;

        /// <summary>
        /// 发起Google登录窗口
        /// </summary>
        /// <param name="onLogin"></param>
        public void Login(Action<string, object> onLogin)
        {
           EB.Debug.Log("---------------------Google----------------------Login");
            onLoginCallBack = onLogin;
            Hub.RunInBackground = true;
            if (!Social.localUser.authenticated)
            {
                Debug.LogError("---------------------Google----------------------Authenticate");
                Social.localUser.Authenticate(OnLoginIn);
            }
        }

        /// <summary>
        ///  Google SDK登录回调
        /// </summary>
        /// <param name="success">登录是否成功</param>
        public void OnLoginIn(bool success)
        {
           EB.Debug.Log("---------------------Google----------------------OnLoginIn");
            Hub.RunInBackground = false;
            if (success)
            {
               EB.Debug.Log("---------------------Google----------------------OnLoginInSuccess");
                isLogin = true;
                m_AuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                m_TokenID = PlayGamesPlatform.Instance.GetIdToken();
               EB.Debug.Log("---------------------Google----------------------m_TokenID : " + m_TokenID);
               EB.Debug.Log("---------------------Google----------------------m_AuthCode : "+ m_AuthCode);
                if (!string.IsNullOrEmpty(m_TokenID))
                {
                    Hashtable loginData = new Hashtable();
                    //loginData.Add("pltform", "Google");
                    loginData.Add("id_token", m_TokenID);
                    EB.Debug.Log("GoogleSDKManager.Login loginData = {0}", JSON.Stringify(loginData));
                    onLoginCallBack(null, loginData);
                }
            }
            else
            {
               EB.Debug.Log("---------------------Google----------------------OnLoginInFailure");
                EB.Coroutines.SetTimeout(delegate ()
                    {
                       EB.Debug.Log("--------------Google Retry Login ----------------");
                        Social.localUser.Authenticate(OnLoginIn);
                    }, 3 * 1000);
            }
        }

        /// <summary>
        /// SDK登出
        /// </summary>
        public void Logout()
        {
            EB.Debug.Log("GoogleSDK.Logout");
            if (!mInitialized)
            {
                EB.Debug.LogWarning("GoogleSDKManager.Logout TencentSDK has not been inited");
                return;
            }

            if (isLogin)
            {
                PlayGamesPlatform.Instance.SignOut();
                isLogin = false;
            }
        }

        public override void Initialize(Config config)
        {

        }

        /// <summary>
        /// Google Play SDK 初始化
        /// </summary>
        /// <param name="initData">初始化参数</param>
        /// <param name="callback">初始化成功回调</param>
        public void InitializeSDK(object initData, Action<string, bool> callback)
        {
           EB.Debug.Log("GoogleSDKManager.InitializeSDK");
            clientConfig = new PlayGamesClientConfiguration.Builder().RequestServerAuthCode(false).RequestIdToken()
            .Build();
            PlayGamesPlatform.InitializeInstance(clientConfig);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            //
            callback(null, true);

            mInitialized = true;
            Hub.RunInBackground = false;
            mGoogleStore = new GameObject("Google_SDK_listener").AddComponent<SparxGoogleSDKManagerMono>();
           EB.Debug.Log("GoogleSDKManager.InitializeSDK End");
        }

        private Action<string, bool> mInitCallback;
        private Action<int, object> mPayCallback;
        private System.Action<string, object> mLoginCallback;

        /// <summary>
        /// 固定流程成员，登录恢复到前台
        /// </summary>
        public void OnLoginEnteredForeground()
        {
           EB.Debug.Log("--------------Google OnLoginEnteredForeground OnLoginEnteredForeground----------------");
            if (isLogin) return;
            if (mLoginCallback != null)
            {
               EB.Debug.Log("--------------Google OnLoginEnteredForeground mlogincallback  not null----------------");
                EB.Coroutines.SetTimeout(delegate ()
                    {
                        //if (mLoginCallback != null)
                        //{
                        //   EB.Debug.Log("--------------Google OnLoginEnteredForeground logincallback ----------------");
                        //    System.Action<string, object> callback = mLoginCallback;
                        //    mLoginCallback = null;
                        //    callback(null, null);
                        //}
                       EB.Debug.Log("--------------Google OnLoginEnteredForeground logincallback ----------------");
                        Login(mLoginCallback);
                    }, 1 * 1000);
            }
        }

        /// <summary>
        /// 支付，初始化的时候恢复到前台
        /// </summary>
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
                            mPayCallback(-1000, null);
                        }
                    }, 5 * 1000);
            }
        }
    }

    /// <summary>
    /// IAP支付，由于需要Mono的部分Application回调函数所以单独分离出来，并实现UnityIAP Listener需要实现的接口
    /// </summary>
    public class SparxGoogleSDKManagerMono : MonoBehaviour, IStoreListener
    {
        private IStoreController mStoreController;
        private IExtensionProvider mExtensionProvider;
        ConfigurationBuilder builder;
        public Action<int, object> mPayCallback;

        private EB.IAP.Transaction currenTransaction;

        void Awake()
        {
            EB.Debug.Log("Creating Google Manager");
            DontDestroyOnLoad(gameObject);
        }

        //发起UnityIAP 支付请求
        public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, Action<int, object> callback)
        {
            EB.Debug.Log("---------------------------GoogleSDKManager.Pay------------------------");
            currenTransaction = transaction;
            mPayCallback = callback;
            if (!IsInitialized())
            {
                EB.Debug.LogError("GoogleSDKManager.Pay: not initialized");
                return;
            }
            EB.Debug.Log("---------------------------GoogleSDKManager.BuyProduct------------------------"+item.payoutId.ToString());
            BuyProduct(item.payoutId.ToString(), transaction.transactionId);
        }

        //检查是否初始化
        public bool IsInitialized()
        {
            return mStoreController != null && mExtensionProvider != null;
        }

        //---------------IStoreListener的四个接口的实现-----------

#region IStoreListener成员
        //初始化成功
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            mStoreController = controller;
            mExtensionProvider = extensions;
            ///mExtensionProvider.GetExtension<IGooglePlayStoreExtensions>().();
           EB.Debug.Log("----------------初始化成功---------------------");

        }

        //初始化失败
        public void OnInitializeFailed(InitializationFailureReason error)
        {
           EB.Debug.Log("----------------初始化失败-------------------- -：" + error);
        }

        //购买失败
        public void OnPurchaseFailed(Product e, PurchaseFailureReason p)
        {

            //var wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(e.receipt);
            //var store = (string)wrapper["Store"];

            //var payload = (string)wrapper["Payload"];
            //ISGoogle = true;
            //var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            //var gpJson = (string)gpDetails["json"];
            //var gpSig = (string)gpDetails["signature"];

            //NetworkManager.(CLIENT_CUSTOM_MESSAGE_ENUM.CLIENT_CUSTOMMSG_GOOGLE_DELIVERY, (int)p, gpJson, gpSig);
            if (p == PurchaseFailureReason.UserCancelled)
            {
                mPayCallback(-2000, null);
            }
            else
            {
                if (p == PurchaseFailureReason.PurchasingUnavailable || p == PurchaseFailureReason.DuplicateTransaction)
                {
                    OnConfirmProduct(e.definition.id);
                }
                mPayCallback(-3000, null);
                //OnConfirmProduct(e.definition.id);
            }
            currenTransaction = null;
           EB.Debug.Log("购买失败--------------------"+ p.ToString());
        }

        /// <summary>
        /// //购买成功和恢复成功的回调，可以根据id的不同进行不同的操作
        /// </summary>
        /// <param name="e">回调的商品列表</param>
        /// <returns></returns>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
           EB.Debug.Log("购买处理成功--------------------Google返回数据");
           EB.Debug.Log("receipt:" + e.purchasedProduct.receipt);
            CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            var result = validator.Validate(e.purchasedProduct.receipt);
            Dictionary<string, string> data = new Dictionary<string, string>();
            List<Transaction> cacheTransactions;
            foreach (IPurchaseReceipt receipt in result)
            {
               EB.Debug.Log("------------IPurchaseReceipt---------------");
                GooglePlayReceipt google = receipt as GooglePlayReceipt;
                if (google != null)
                {
                    string eProductId = e.purchasedProduct.definition.id;
                    if (currenTransaction == null)
                    {
                        SparxTransactionHelper.GetTranByPayoutId(int.Parse(eProductId), out cacheTransactions);
                        if (cacheTransactions == null || cacheTransactions.Count == 0)
                        {
                            //这个时候就只能去服务器看了，本地的缓存订单已经没了
                           EB.Debug.Log("------------PurchaseProcessingResult.Complete-----------------"+ google.transactionID);
                            return PurchaseProcessingResult.Complete;
                        }
                        //同一个缓存的Google订单号去遍历所有缓存在本地的APP订单逐一请求发货
                        for (int i = 0; i < cacheTransactions.Count; i++)
                        {
                            data.Clear();
                            currenTransaction = cacheTransactions[i];
                           EB.Debug.Log("------------currentGPProductId---------------:" + eProductId);
                            data.Add("productId", e.purchasedProduct.definition.id);
                           EB.Debug.Log("------------currentGPProductId---------------:");
                           EB.Debug.Log("------------currentGPTokenId-----------------:" + google.purchaseToken);
                            data.Add("token", google.purchaseToken);
                           EB.Debug.Log("------------currentGPTokenId-----------------");
                           EB.Debug.Log("------------currentGPTransactionId-----------:" + currenTransaction.transactionId);
                            data.Add("transactionId", currenTransaction.transactionId);
                           EB.Debug.Log("------------currentGPTransactionId-----------:");
                            data.Add("transactionProId", currenTransaction.productId);
                            if (mPayCallback != null)
                            {
                                //支付成功的向APPServer申请处理
                               EB.Debug.Log("------------PayCallback---------------:" + e.purchasedProduct.definition.id);
                                mPayCallback(-1000, data);
                                mPayCallback = null;
                            }
                        }
                    }
                    else
                    {
                        data.Clear();
                       EB.Debug.Log("------------currentGPProductId---------------:" + eProductId);
                        data.Add("productId", e.purchasedProduct.definition.id);
                       EB.Debug.Log("------------currentGPProductId---------------:");
                       EB.Debug.Log("------------currentGPTokenId-----------------:" + google.purchaseToken);
                        data.Add("token", google.purchaseToken);
                       EB.Debug.Log("------------currentGPTokenId-----------------");
                       EB.Debug.Log("------------currentGPTransactionId-----------:" + currenTransaction.transactionId);
                        data.Add("transactionId", currenTransaction.transactionId);
                       EB.Debug.Log("------------currentGPTransactionId-----------:");
                        data.Add("transactionProId", currenTransaction.productId);
                        if (mPayCallback != null)
                        {
                            //支付成功的向APPServer申请处理
                           EB.Debug.Log("------------PayCallback---------------:" + e.purchasedProduct.definition.id);
                            mPayCallback(-1000, data);
                            mPayCallback = null;
                        }
                    }
                }
               EB.Debug.Log("------------for IPurchaseReceipt +1-----------:");
            }
           EB.Debug.Log("------------for IPurchaseReceipt End-----------:");
           EB.Debug.Log("购买处理完毕--------------------");
            currenTransaction = null;
            return PurchaseProcessingResult.Pending;
        }

        /// <summary>
        /// 购买成功，确认已发货
        /// </summary>
        /// <param name="productID"></param>
        public void OnConfirmProduct(string productID)
        {
            Product produdt = mStoreController.products.WithID(productID);
           EB.Debug.Log("Own Product , consume it ---------------------------------------");
            mStoreController.ConfirmPendingPurchase(produdt);
        }

        /// <summary>
        /// 购买 
        /// </summary>
        /// <param name="productID"></param>
        public void BuyProduct(string productID,string tranId)
        {
           EB.Debug.Log("----------Buy Product------------- : "+productID);
            Product produdt = mStoreController.products.WithID(productID);
            if (produdt != null && produdt.availableToPurchase)
            {
                //mStoreController.InitiatePurchase(produdt);
                var payloadObj = new Hashtable
                {
                    { "transactionId", tranId }
                };
                var payload = EB.JSON.Stringify(payloadObj);
                mStoreController.InitiatePurchase(produdt, payload);
               EB.Debug.Log(produdt.metadata.localizedPrice);
            }
            else
            {
               EB.Debug.Log("fail");
            }
        }
#endregion

        /// <summary>
        /// 退回后台
        /// </summary>
        /// <param name="pauseStatus"></param>
        void OnApplicationPause(bool pauseStatus)
        {
            EB.Debug.Log("SparxGoogleSDKManager.OnApplicationPause: status = {0}", pauseStatus);
            if (!pauseStatus)
            {
                SparxHub.Instance.GoogleSDKManager.OnLoginEnteredForeground();
            }
        }

        void OnApplicationQuit()
        {
            
        }
    }
}
#endif