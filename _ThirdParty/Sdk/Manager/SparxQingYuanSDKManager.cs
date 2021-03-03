#if USE_QINGYUANSDK
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace EB.Sparx
{

	public class QingYuanResultCode
	{
		public const int RESULT_OK = 0;
		public const int RESULT_FAIL = 1;
		public const int RESULT_TIMEOUT = 2;
	}

	public class QingYuanSDKCallbackType
	{
		public const string CALLBACKTYPE_LOGIN = "Login";

		public const string CALLBACKTYPE_PAY = "Pay";

	}

	public class QingYuanSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mLogined = false;
		private System.Action<string, object> mLoginCallback;
		private System.Action<int> payResultCallBack;
        private SparxQingYuanSDKManager mSparxQingYuanSDKManager = null;
		private string mNotifyUrl;
        public bool IsLoggedIn { get { return mLogined; } }

        private static object _cleanupHandle = null;

        public override void Initialize(Config config)
		{
		}

		public void InitSDK(object initData, Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.Log("QingYuanSDKManager.InitSDK: Initialized");
				callback(null, true);
				return;
			}
			mNotifyUrl = EB.Dot.String("notifyUrl", initData, mNotifyUrl);
			EB.Debug.Log("QingYuanSDKManager.InitSDK notifyUrl= {0}", mNotifyUrl);
			mInitialized = true;
			GameObject listner=new GameObject("qingyuan_plugin_listener", typeof(SparxQingYuanSDKManager));
            mSparxQingYuanSDKManager = listner.GetComponent<SparxQingYuanSDKManager>();
			callback(null, true);
		}

		public void Login(Action<string, object> callback)
		{
			if (!mInitialized)
			{
				callback("QingYuanSDKManager has not been inited", null);
				return;
			}
            EB.Debug.Log("QingYuanSDKManager.Login");
			mLoginCallback += callback;
			Hub.RunInBackground = true;
			mSparxQingYuanSDKManager.LoginSDK();
		}

		public void OnLoginCallback(int code, object data)
		{
			EB.Debug.Log("QingYuanSDKManager.OnLoginResult:code={0} data = {1}", code, data);
			Hub.RunInBackground = false;
			if(code != QingYuanResultCode.RESULT_OK)
			{
				if (mLoginCallback != null)
				{
					mLoginCallback(null, null);
					mLoginCallback = null;
				}
				return;
			}

			mLogined = true;
			if (mLoginCallback != null)
			{
				Action<string, object> callback = mLoginCallback;
				mLoginCallback = null;
				callback(null, data);
			}
			else
			{
				EB.Debug.Log("mLoginCallback is null");
			}
		}

		public void Pay(EB.IAP.Item item, string transactionId, Action<int> callback)
		{
			payResultCallBack += callback;
			User curUser = Hub.LoginManager.LocalUser;
			if (curUser == null)
			{
				return;
			}
			mSparxQingYuanSDKManager.Pay(item.longName, item.cost, transactionId, curUser.Name, item.productId, mNotifyUrl, "");
		}

		public void OnPayCallback(int resultCode)
		{
			EB.Debug.Log("QingYuanSDKManager.OnPay resultCode = {0}", resultCode);

			if (payResultCallBack != null)
			{
				payResultCallBack(resultCode);
				payResultCallBack = null;
                if (_cleanupHandle != null)
                {
                    EB.Coroutines.ClearTimeout(_cleanupHandle);
                    _cleanupHandle = null;
                }
			}
		}

		public override void OnEnteredForeground()
		{

			if (payResultCallBack != null)
			{
                EB.Coroutines.ClearTimeout(_cleanupHandle);
                _cleanupHandle = EB.Coroutines.SetTimeout(delegate ()
				{
					if (payResultCallBack != null)
					{
						payResultCallBack(QingYuanResultCode.RESULT_TIMEOUT);
						payResultCallBack = null;
					}
				}, 10 * 1000);
			}
		}

		public void Logout()
		{

		}
	}

	public class SparxQingYuanSDKManager : MonoBehaviour {

		void Awake()
		{
			EB.Debug.Log("Creating QingYuan Manager");
			DontDestroyOnLoad(gameObject);
		}

		public void InitSDK()
		{
        }

		[DllImport("__Internal")]
		private static extern void QingYuanLogin();

		[DllImport("__Internal")]
		private static extern void QingYuanPay(string goodsName, float goodsPrice, string order, string username, string productID, string notyfyUrl, string attach);

		public void LoginSDK()
		{
			QingYuanLogin();
        }

		public void Pay(string goodsName, float goodsPrice, string order, string username, string productID, string notyfyUrl, string attach)
		{
			QingYuanPay(goodsName, goodsPrice, order, username, productID, notyfyUrl, attach);
		}

		public void OnQingYuanGameSDKCallback(string jsonString)
		{
			EB.Debug.Log("SparxQingYuanSDKManager.OnQingYuanGameSDKCallback:jsonString = {0}", jsonString);
			object json = EB.JSON.Parse(jsonString);
			string callbackType = Dot.String("callbackType", json, string.Empty);
			int code = Dot.Integer("code", json, -1);
			switch (callbackType)
			{
				case QingYuanSDKCallbackType.CALLBACKTYPE_LOGIN:
					Sparx.Hub.Instance.QingYuanSDKManager.OnLoginCallback(code, Dot.Object("data", json, null));
					break;

				case QingYuanSDKCallbackType.CALLBACKTYPE_PAY:
					Sparx.Hub.Instance.QingYuanSDKManager.OnPayCallback(code);
					break;
			}
		}
	}
}
#endif
