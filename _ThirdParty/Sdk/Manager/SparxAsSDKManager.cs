#if USE_ASSDK && UNITY_IPHONE
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace EB.Sparx
{
	public class AsSDKManager : Manager
	{
		[DllImport("__Internal")]
		private static extern void AsInit(int appId, string appKey);
		//Init
		public void pxAsInit(int appId, string appKey)
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsInit(appId, appKey);
			}
		}

		[DllImport("__Internal")]
		private static extern void AsLogin();
		//Login
		public void pxAsLogin()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsLogin();
			}
		}

		[DllImport("__Internal")]
		private static extern void AsCenter();
		//AsCenter
		public void pxAsCenter()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsCenter();
			}
		}

		[DllImport("__Internal")]
		private static extern void AsLogout();
		//Logout
		public void pxAsLogout()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsLogout();
			}
		}

		[DllImport("__Internal")]
		private static extern string AsUserID();
		//IsLogined
		public string pxAsUserID()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				return AsUserID();
			}

			return string.Empty;
		}

		[DllImport("__Internal")]
		private static extern string AsUserName();
		//UserName
		public string pxAsUserName()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				return AsUserName();
			}

			return string.Empty;
		}

		[DllImport("__Internal")]
		private static extern void AsIsLog(bool isLog);
		//SetUpdateDebug
		public void pxAsIsLog(bool isLog)
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsIsLog(isLog);
			}
		}

		[DllImport("__Internal")]
		private static extern void AsPayRMB(int amount, string paramBillNo, string paramBillTitle, string paramRoleId, int zoneID);
		//Pay RMB
		public void pxAsPayRMB(int amount, string paramBillNo, string paramBillTitle, string paramRoleId, int zoneID)
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsPayRMB(amount, paramBillNo, paramBillTitle, paramRoleId, zoneID);
			}
		}

		[DllImport("__Internal")]
		private static extern void AsHiddenCloseLoginX();
		public void pxAsHiddenCloseLoginX()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsHiddenCloseLoginX();
			}
		}

		[DllImport("__Internal")]
		private static extern int AsIsLogined();
		public int pxAsIsLogined()
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				return AsIsLogined();
			}

			return 0;
		}

		[DllImport("__Internal")]
		private static extern void AsAlertView(string title, string msg);
		public void pxAsAlertView(string title, string msg)
		{
			if (UnityEngine.Application.platform != UnityEngine.RuntimePlatform.OSXEditor)
			{
				AsAlertView(title, msg);
			}
		}

		private bool mIsInited = false;
		private bool mIsLoggedIn = false;

		public System.Action<string, object> mLoginCallBack;
		public System.Action<string> mPayResultCallback;

		public override void Initialize(Config config)
		{

		}

		public void InitSDK(object options, System.Action<string, bool> callback)
		{            
#if UNITY_EDITOR
			mIsInited = false;
			mIsLoggedIn = false;
			callback(null, mIsInited);
#elif UNITY_IPHONE
			if (mIsInited)
			{
				callback(null, true);
			}
			else
			{
				new GameObject("as_callback", typeof(SparxAsSDKManager));

				//int appId = Dot.Integer("appId", options, 2241);
				//string appKey = Dot.String("appKey", options, "9d5e83cf5653400b959cd03c422579c6");
				int appId = Dot.Integer("appId", options, 2356);
				string appKey = Dot.String("appKey", options, "2cd489c773204183af5c30c44360501e");
				pxAsInit(appId, appKey);
				mIsInited = true;
				mIsLoggedIn = SecurePrefs.GetInt("AsUid", 0) > 0;
				callback(null, mIsInited);
			}
#else
			mIsInited = false;
			mIsLoggedIn = false;
			callback(null, mIsInited);
#endif
		}

		public void Login(System.Action<string, object> callback)
		{
#if UNITY_EDITOR
			callback("AsSDK not support in Editor", null);
#elif UNITY_IPHONE
			if (!mIsInited)
			{
				callback("AsSDK has not been inited", null);
				return;
			}

			pxAsLogin();
			mLoginCallBack = callback;
#else
			callback("AsSDK not support on this platform", null);
#endif
		}

		public void Logout()
		{
			if (!mIsInited)
			{
				return;
			}

			if (IsLoggedIn())
			{
				EB.Debug.Log("AsSDKManager.Logout");
				mIsLoggedIn = false;
				SecurePrefs.DeleteKey("AsUid");
				pxAsLogout();
			}
		}

		public bool IsLoggedIn()
		{
			return mIsLoggedIn;
		}

		public bool IsInited()
		{
			return mIsInited;
		}

		public void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<string> callback)
		{
			if (!IsInited())
			{
				EB.Debug.LogWarning("AsSDKManager.Pay: AsSDK not inited");
				return;
			}

			mPayResultCallback += callback;

			string uid = EB.Sparx.Hub.Instance.LoginManager.LocalUserId.Value.ToString();
			int worldId = EB.Sparx.Hub.Instance.LoginManager.LocalUser.WorldId;
			pxAsPayRMB((int)item.cost, transaction.transactionId, item.longName, uid, worldId);
			EB.Debug.Log("AsSDKManager.Pay Success item.cost = {0}; transaction.transactionId = {1}; item.longName = {2}; uid = {3}; worldId = {4}",
				item.cost, transaction.transactionId, item.longName, uid, worldId);
		}

		public void OnAsVerifyingUpdatePass(string msg)
		{
			EB.Debug.Log("OnAsVerifyingUpdatePass: {0}", msg);
		}

		public void OnAsLogin(string token)
		{
			EB.Debug.Log("OnAsLogin token = {0} ", token);

			if (!string.IsNullOrEmpty(token))
			{
				mIsLoggedIn = true;
				SecurePrefs.SetInt("AsUid", int.Parse(pxAsUserID()));

				if (mLoginCallBack != null)
				{
					Hashtable auth = new Hashtable();
					auth["token"] = token;
					mLoginCallBack(null, auth);
					mLoginCallBack = null;
				}
			}
			else
			{
				if (mLoginCallBack != null)
				{
					mLoginCallBack("LoginFailed", null);
					mLoginCallBack = null;
				}
			}
		}

		public void OnAsLogOff(string msg)
		{
			EB.Debug.Log("OnAsLogOff: {0}", msg);
		}

		public void OnAsPayResult(string resultCode)
		{
			EB.Debug.Log("OnAsPayResult: {0}", resultCode);

			if (mPayResultCallback != null)
			{
				mPayResultCallback(resultCode);
				mPayResultCallback = null;
			}
		}

		public void OnAsCloseCharge(string msg)
		{
			EB.Debug.Log("OnAsCloseCharge: {0}", msg);
		}

		public void OnAsCloseLoginView(string resultCode)
		{
			EB.Debug.Log("OnAsCloseLoginView: {0}", resultCode);
		}
	}
}

class SparxAsSDKManager : UnityEngine.MonoBehaviour
{
	void Awake()
	{
		EB.Debug.Log("Creating As Manager");
		DontDestroyOnLoad(gameObject);
	}

	public void asVerifyingUpdatePassCallBack(string msg)
	{
		EB.Debug.Log("asVerifyingUpdatePassCallBack: {0}", msg);
		SparxHub.Instance.AsSDKManager.OnAsVerifyingUpdatePass(msg);
	}

	public void asLoginCallBack(string token)
	{
		EB.Debug.Log("asLoginCallBack: {0}", token);
		SparxHub.Instance.AsSDKManager.OnAsLogin(token);
	}

	public void asLogOffCallBack(string msg)
	{
		EB.Debug.Log("asLogOffCallBack: {0}", msg);
		SparxHub.Instance.AsSDKManager.OnAsLogOff(msg);
	}

	public void asPayResultCallBack(string resultCode)
	{
		EB.Debug.Log("asPayResultCallBack: {0}", resultCode);
		SparxHub.Instance.AsSDKManager.OnAsPayResult(resultCode);
	}

	public void asCloseChargeCallBack(string msg)
	{
		EB.Debug.Log("asCloseChargeCallBack: {0}", msg);
		SparxHub.Instance.AsSDKManager.OnAsCloseCharge(msg);
	}

	public void asCloseLoginViewCallBack(string resultCode)
	{
		EB.Debug.Log("asCloseLoginViewCallBack: {0}", resultCode);
		SparxHub.Instance.AsSDKManager.OnAsCloseLoginView(resultCode);
	}
}
#endif