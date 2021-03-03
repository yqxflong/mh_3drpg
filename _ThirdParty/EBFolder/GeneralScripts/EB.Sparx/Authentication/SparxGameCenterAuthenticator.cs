using System.Runtime.InteropServices;
using UnityEngine;

namespace EB.Sparx
{
	class GameCenterAuthenticator : Authenticator
	{
#region Authenticator implementation

#if USE_GAMECENTER_AUTH
		[DllImport("__Internal")]
		static extern bool _GameCenterAuthenticationSupported();

		[DllImport("__Internal")]
		static extern bool _GameCenterAuthenticate();

		[DllImport("__Internal")]
		static extern bool _GameCenterLogout();
#endif

		public static GameCenterAuthenticator Instance {get;private set;}
		System.Action<string,object> _authCallback;
		System.Action<string> _logoutCallback;

		public void Init (object initData, System.Action<string, bool> callback)
		{
			bool supported = false;
#if USE_GAMECENTER_AUTH
			supported = _GameCenterAuthenticationSupported();
			if (supported)
			{
				Instance = this;
				new GameObject("gca_callbacks", typeof(SparxGameCenterAuthenticator));
			}
#endif
			callback(null,supported);
		}

		public void Authenticate (bool silent, System.Action<string, object> callback)
		{
#if USE_GAMECENTER_AUTH
            if (silent)
            {
                callback(null,null);
            }
            else
            {
               EB.Debug.Log("-----------苹果认证 GameCenterAuthenticator  Authenticate------------");
                _authCallback = callback;
                _GameCenterAuthenticate();
            }
#endif
		}

		public string Name {
			get {
				return "gamecenter";
			}
		}

		public bool IsLoggedIn {
			get {
#if USE_GAMECENTER_AUTH
				var local = Social.localUser;
				return local != null && local.authenticated;
#else
				return false;
#endif
			}
		}

		public AuthenticatorQuailty Quailty {
			get {
				return AuthenticatorQuailty.Med;
			}
		}

		public void OnAuthenticateError( string error )
		{
			Debug.Log("GameCenterAuthenticator: OnAuthenticateError: {0}", error);
			if (_authCallback != null)
			{
				_authCallback(null,null);
				_authCallback = null;
			}
		}

		public void OnAuthenticate( object data )
		{
			Debug.Log("GameCenterAuthenticator: OnAuthenticate: {0}", data);
			if (_authCallback != null)
			{
				_authCallback(null,data);
				_authCallback = null;
			}
		}

		public void OnLogout(string result)
		{
			Debug.Log("GameCenterAuthenticator: OnLogout: {0}", result);
			if (_logoutCallback != null)
			{
				_logoutCallback(result);
				_logoutCallback = null;
			}
		}

		public void Logout()
		{
			if (IsLoggedIn)
			{
#if USE_GAMECENTER_AUTH
				_GameCenterLogout();
#endif
			}
		}

#endregion

	}
}

public class SparxGameCenterAuthenticator : UnityEngine.MonoBehaviour
{
	void Awake()
	{
		EB.Debug.Log("Creating Game Center Authenticator");
		DontDestroyOnLoad(gameObject);
	}

	public void OnAuthenticateError( string error )
	{
		EB.Sparx.GameCenterAuthenticator.Instance.OnAuthenticateError(error);
	}
	
	public void OnAuthenticate( string json )
	{
		var data = EB.JSON.Parse(json);
		EB.Sparx.GameCenterAuthenticator.Instance.OnAuthenticate( data );
	}

	public void OnLogout(string result)
	{
		EB.Sparx.GameCenterAuthenticator.Instance.OnLogout(result);
	}
}


