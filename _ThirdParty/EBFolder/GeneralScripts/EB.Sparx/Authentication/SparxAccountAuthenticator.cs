using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public class AccountAuthenticator : Authenticator
	{
		private bool mLoggedIn = false;
#if USE_ACCOUNT_AUTH
		private string mAccountId = string.Empty;
		private GameObject mLoginUI = null;
		private Action<string, object> mAuthCallback;
#endif

		public bool IsLoggedIn
		{
			get { return mLoggedIn; }
		}

		public string Name
		{
			get { return "account"; }
		}

		public AuthenticatorQuailty Quailty
		{
			get { return AuthenticatorQuailty.High; }
		}

		public void Authenticate(bool silent, System.Action<string, object> callback)
		{
#if USE_ACCOUNT_AUTH
			if (silent)
			{
				callback(null, null);
			}
			else
			{
				mAuthCallback = callback;
				mLoginUI.SetActive(true);
			}
#else
			callback("Not support", null);
#endif
		}

		public void Init(object initData, System.Action<string, bool> callback)
		{
#if USE_ACCOUNT_AUTH
			mAccountId = SecurePrefs.GetString("AccountId", mAccountId);
			if (mLoginUI == null)
			{
				mLoginUI = new GameObject("account_auth_ui", typeof(SparxAccountAuthenticator));
				mLoginUI.SetActive(false);
				var comp = mLoginUI.GetComponent<SparxAccountAuthenticator>();
				comp.SetInput(mAccountId);
				comp.OnSubmit += OnSubmit;
			}
			
			callback(null, true);
#else
			callback(null, false);
#endif
		}

		public void Logout()
		{
#if USE_ACCOUNT_AUTH
			if (mLoggedIn)
			{
				mLoggedIn = false;
				mAccountId = string.Empty;
				SecurePrefs.DeleteKey("AccountId");
			}
#endif
		}

		private void OnSubmit(string accountId)
		{
#if USE_ACCOUNT_AUTH
			EB.Debug.Log("OnSubmit: accountId = {0}", accountId);

			if (string.IsNullOrEmpty(accountId))
			{
				if (mAuthCallback != null)
				{
					mAuthCallback("Empty Account Id!", null);
					mAuthCallback = null;
				}
			}
			else
			{
				if (mAuthCallback != null)
				{
					mLoggedIn = true;
					mAccountId = accountId;
					SecurePrefs.SetString("AccountId", mAccountId);

					var data = new Hashtable();
					data["accountId"] = mAccountId;
					mAuthCallback(null, data);
					mAuthCallback = null;
				}
			}
#endif
		}
	}
}

#if USE_ACCOUNT_AUTH
class SparxAccountAuthenticator : MonoBehaviour
{
	private string mAccountId;
	private int mWindowId;
	private Rect mWindowRect;

	private readonly Vector2 mSize = new Vector2(800, 600);

	public System.Action<string> OnSubmit;

	public void SetInput(string accountId)
	{
		mAccountId = accountId;
	}

	void Resize(float width, float height)
	{
		if (Screen.width > width || Screen.height > height)
		{
			Vector3 ratio = new Vector3((float)Screen.width / width, (float)Screen.height / height, 1.0f);
			GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, ratio);
		}
		else
		{
			GUI.matrix = Matrix4x4.identity;
		}
	}

	void OnEnable()
	{
		int width = Mathf.Min((int)mSize.x, Screen.width);
		int height = Mathf.Min((int)mSize.y, Screen.height);

		mWindowId = (int)(Random.value * 1000000);
		mWindowRect = new Rect();
		mWindowRect.width = 400;
		mWindowRect.height = 100;
		mWindowRect.x = (width - mWindowRect.width) / 2;
		mWindowRect.y = (height - mWindowRect.height) / 2;
	}

	void OnGUI()
	{
		Resize(800, 600);

		GUI.ModalWindow(mWindowId, mWindowRect, delegate (int id)
		{
			GUILayout.BeginVertical();

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Account ID", GUILayout.Width(80));
			mAccountId = GUILayout.TextField(mAccountId, GUILayout.Width(250));
			
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Submit", GUILayout.Width(80)))
			{
				if (OnSubmit != null)
				{
					OnSubmit(mAccountId);
				}
				gameObject.SetActive(false);
			}
			if (GUILayout.Button("Cancel", GUILayout.Width(80)))
			{
				if (OnSubmit != null)
				{
					OnSubmit(null);
				}
				gameObject.SetActive(false);
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUILayout.EndVertical();
		}, "Input Account ID");
	}
}
#endif