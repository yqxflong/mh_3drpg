using System.Collections;
using System.Collections.Generic;

namespace EB.Sparx
{
	public class MHAuthenticator : Authenticator
	{
		public class UserInfo
		{
			public string phone;
			public string password;
		}

		private bool mLoggedIn = false;
		static public List<UserInfo> UserInfoList;
		static public string UserPhone;

		#region Authenticator implementation
		public void Init (object initData, System.Action<string, bool> callback)
		{
			UserInfoList = new List<UserInfo>();
			string userInfos = SecurePrefs.GetString("UserInfoData", "");
            EB.Debug.Log("Manhuang:"+userInfos);
            Hashtable userHashtable = EB.JSON.Parse(userInfos) as Hashtable;
			if (userHashtable != null&& userHashtable.Count>0)
			{
				foreach (DictionaryEntry user in userHashtable)
				{
					UserInfoList.Add(new UserInfo() {
						phone = user.Key.ToString(),
						password = user.Value.ToString()
					});
                    EB.Debug.Log("Manhuang_Login:{"+ user.Key.ToString() +","+ user.Value.ToString() + "}");
				}
			}			
			callback(null,true);
		}

		public void Authenticate(bool silent, System.Action<string, object> callback)
		{
			if (silent)
			{
				if (IsLoggedIn)
				{
					callback(null, Johny.HashtablePool.Claim());
					return;
				}
				else
				{
					callback(null, null);
					return;
				}
			}
			else
			{
				System.Action<string,string,string,string> cb = delegate (string phone, string openId, string accessToken, string password) {
					var data = Johny.HashtablePool.Claim();
					Debug.Log("MHAuthenticator callback openId:{0} accessToken:{1}", openId, accessToken);
					data["openId"] = openId;					
					data["accessToken"] = accessToken;
					callback(null, data);
					mLoggedIn = true;
					UserPhone = phone;
					SaveAccountData(phone,password);
				};

				string userID = SecurePrefs.GetString("UserID", string.Empty);
				string userPwd = SecurePrefs.GetString("UserPwd", string.Empty);

                SparxHub.Instance.Config.LoginConfig.Listener.OnMHLogin(new Hashtable() { { "userID", userID }, { "password", userPwd }, { "callback", cb } });
			}
		}

		static public void SaveAccountData(string phone,string password)
		{
			UserInfo userInfo = UserInfoList.Find(u => u.phone == phone);
			if (userInfo == null)
			{
				UserInfoList.Add(new UserInfo()
				{
					phone = phone,
					password = password
				});
			}
			else {
				userInfo.password = password;
			}
			SecurePrefs.SetString("UserID", phone);
			SecurePrefs.SetString("UserPwd", password);

			string userInfos = SecurePrefs.GetString("UserInfoData", "");
			Debug.Log("store before UserInfoData:" + userInfos);
			Hashtable userHashtable = EB.JSON.Parse(userInfos) as Hashtable;
			if (userHashtable != null && userHashtable.ContainsKey(phone))
			{
				userHashtable.Remove(phone);
			}
			userHashtable = userHashtable ?? Johny.HashtablePool.Claim();
			userHashtable.Add(phone, password);
			SecurePrefs.SetString("UserInfoData", EB.JSON.Stringify(userHashtable));
			Debug.Log("store UserInfoData:" + SecurePrefs.GetString("UserInfoData", ""));
		}

		public void Logout()
		{
            mLoggedIn = false;
        }

		public string Name {
			get {
				return "manhuang";
			}
		}

		public bool IsLoggedIn {
			get {
				return mLoggedIn;
			}
		}

		public AuthenticatorQuailty Quailty {
			get {
				return AuthenticatorQuailty.High;
			}
		}
		#endregion
	}
}