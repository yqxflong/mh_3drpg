#if USE_KUAIYONGSDK && UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace EB.Sparx
{
	public class KuaiYongSDKManager : Manager
	{
		[DllImport("__Internal")]
		private static extern void KYSDKInit();

		[DllImport("__Internal")]
		private static extern void KYLogin();

		[DllImport("__Internal")]
		private static extern void KYLogout();

		[DllImport("__Internal")]
		private static extern bool KYIsLogin();

		[DllImport("__Internal")]
		private static extern bool KYPay(string userId, string serverId, string orderId, string productId, string productName, string productNumber, string productPrice, string money);

		[DllImport("__Internal")]
		private static extern void XYEnteredBackground();

		private bool isInited = false;
		private Action<string, bool> initCallBack;
		private Action<string, object> loginCallBack;
		private Action<string> payResultCallBack;

		public override void Initialize(Config config)
		{

		}

		public override void OnEnteredBackground()
		{
			XYEnteredBackground();
		}

		public void InitSDK(object options, Action<string, bool> callback)
		{
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
				new GameObject("ky_callback", typeof(SparxKuaiYongSDKManager));
				KYSDKInit();
				initCallBack = callback;
			}
#else
			isInited = false;
			callback(null, isInited);
#endif
		}

		public void OnKYInit(string msg)
		{
			EB.Debug.Log("KuaiYongSDKManager.OnXYInit");
			if (initCallBack != null)
			{
				if (msg == "successed")
				{
					isInited = true;
					initCallBack(null, true);
					initCallBack = null;
				}
				else
				{
					initCallBack("KuaiYongSDK Init Failed", false);
					initCallBack = null;
				}
			}
		}


		public void Login(Action<string, object> callback)
		{
			EB.Debug.Log("KuaiYongSDKManager.Login");
#if UNITY_EDITOR
			callback("KuaiYongSDK not support in Editor", null);
#elif UNITY_IPHONE
			if (!isInited)
			{
				callback("KuaiYongSDK has not been inited", null);
				return;
			}
			KYLogin();
			loginCallBack = callback;
#else
			callback("XYSDK not support on this platform", null);
#endif
		}

		public void OnKYLogin(string token)
		{
			EB.Debug.Log("KuaiYongSDKManager.OnKYLogin token = {0}", token);
			if (!string.IsNullOrEmpty(token))
			{
				Hashtable auth = new Hashtable();
				auth["token"] = token;
				auth["platform"] = "ios";
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


		public void Logout()
		{
			KYLogout();
		}

		public void OnKYLogout()
		{
			Hub.Instance.Disconnect(true);
		}


		public void Pay(EB.IAP.Item item, string transactionId, Action<string> callback)
		{
			if (!IsInited())
			{
				EB.Debug.LogWarning("KuaiYongSDKManager.Pay: KuaiYongSDK not inited");
				return;
			}
			payResultCallBack += callback;
			string guid = GetGuid();
			int worldId = EB.Sparx.Hub.Instance.LoginManager.LocalUser.WorldId;
			KYPay(guid, worldId.ToString(), transactionId, item.productId, item.longName, "1", item.cost.ToString(), item.cost.ToString());
		}

		public void OnKYPay(string resultCode)
		{
			EB.Debug.Log("KuaiYongSDKManager.OnKYPay resultCode = {0}", resultCode);
			if (!string.IsNullOrEmpty(resultCode))
			{
				if (payResultCallBack != null)
				{
					payResultCallBack(resultCode);
					payResultCallBack = null;
				}
			}
		}


		public bool IsInited()
		{
			return isInited;
		}

		public bool IsLoggedIn()
		{
#if UNITY_EDITOR
			return false;
#else
			return KYIsLogin();
#endif
		}

		private string GetGuid()
		{
			string guid = string.Empty;
			foreach (AuthData authData in Hub.Instance.LoginManager.Account.Authenticators)
			{
				if (authData.Authenticator != null)
				{
					if (authData.Authenticator.Name == "kuaiyong")
					{
						guid = authData.Id;
						break;
					}
				}
			}
			return guid;
		}
	}

	class SparxKuaiYongSDKManager : MonoBehaviour
	{
		void Awake()
		{
			EB.Debug.Log("Creating KY SDKManager");
			DontDestroyOnLoad(gameObject);
		}

		public void OnKYInit(string msg)
		{
			EB.Debug.Log("OnKYInit");
			Hub.Instance.KuaiYongSDKManager.OnKYInit(msg);
		}

		public void OnKYLogin(string token)
		{
			EB.Debug.Log("OnXYLogin: {0}", token);
			Hub.Instance.KuaiYongSDKManager.OnKYLogin(token);
		}

		public void OnKYLogout(string msg)
		{
			EB.Debug.Log("OnKYLogout");
			Hub.Instance.KuaiYongSDKManager.OnKYLogout();
		}

		public void OnKYPay(string payResultInfo)
		{
			EB.Debug.Log("OnXYPay: {0}", payResultInfo);
			Hub.Instance.KuaiYongSDKManager.OnKYPay(payResultInfo);
		}
	}
}
#endif

