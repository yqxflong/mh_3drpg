#if UNITY_IPHONE && USE_WINNERIOSSDK
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
namespace EB.Sparx
{

	public class WinnerIOSResultCode
	{
		public const int RESULT_OK = 0;
		public const int RESULT_FAIL = 1;
	}

	public class WinnerIOSSDKManager : Manager
	{
		private bool mInitialized = false;
		private bool mLogined = false;
		private System.Action<string, object> mLoginCallback;
		private SparxWinnerIOSSDKManager mSparxWinnerIOSSDKManager = null;
		public bool IsLoggedIn { get { return mLogined; } }
		public override void Initialize(Config config)
		{
		}

		public void InitSDK(object initData, Action<string, bool> callback)
		{
			if (mInitialized)
			{
				EB.Debug.Log("WinnerIOSSDKManage.InitSDK: Initialized");
				callback(null, true);
				return;
			}
			if (Application.platform != RuntimePlatform.IPhonePlayer)
			{
				callback(null, false);
				return;
			}
			Hub.RunInBackground = true;
			mInitialized = true;
			GameObject listner= new GameObject("winner_plugin_listener", typeof(SparxWinnerIOSSDKManager));
			mSparxWinnerIOSSDKManager = listner.GetComponent<SparxWinnerIOSSDKManager>();
			mSparxWinnerIOSSDKManager.InitSDK();
			callback(null, true);
		}

		public void Login(Action<string, object> callback)
		{
			if (!mInitialized)
			{
				callback("WinnerIOSSDKManage has not been inited", null);
				return;
			}
			SetEnterGameUIBGState(false);
            EB.Debug.Log("WinnerIOSSDKManage.Login");
			mLoginCallback += callback;
			Hub.RunInBackground = true;
			mSparxWinnerIOSSDKManager.LoginSDK();
		}

		void SetEnterGameUIBGState(bool state)
		{
			GameObject obj = GameObject.Find("EnterGameUI");
			if (obj != null)
			{
				obj.SendMessage("SetEnterGameUIBGState", state);
			}
			else
			{
				EB.Debug.LogWarning("Can not find EnterGameUI obj!!");
			}
		}

		public void OnLoginCallback(int code, object data)
		{
			SetEnterGameUIBGState(true);
			EB.Debug.Log("WinnerIOSSDKManage.OnLoginResult:code={0} data = {1}", code, data);
			if(code != WinnerIOSResultCode.RESULT_OK)
			{
				if (mLoginCallback != null)
				{
					mLoginCallback(null, null);
					mLoginCallback = null;
					EB.Debug.Log("HuaweiSDKManager.OnLogin !RESULT_OK 0");
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
	}

	public class SparxWinnerIOSSDKManager : MonoBehaviour {

		void Awake()
		{
			EB.Debug.Log("Creating WinnerIOS Manager");
			DontDestroyOnLoad(gameObject);
		}

		public void InitSDK()
		{
        }

		[DllImport("__Internal")]
		private static extern void WinnerLogin();
		public void LoginSDK()
		{
			WinnerLogin();
        }

		public void OnLogin(string result)
		{
			Debug.Log("OnLogin" + result);
			if(string.IsNullOrEmpty(result))
			{
				Hub.Instance.WinnerIOSSDKManager.OnLoginCallback(WinnerIOSResultCode.RESULT_FAIL,null);
			}
			else
			{
				Hub.Instance.WinnerIOSSDKManager.OnLoginCallback(WinnerIOSResultCode.RESULT_OK, new Hashtable() { { "accessCode", result }, { "type","ios"} });
			}
        }
	}
}
#endif
