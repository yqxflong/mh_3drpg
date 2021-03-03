using System.Collections;

namespace EB.Sparx
{
	public class DeviceAuthenticator : Authenticator
	{
		private bool mLoggedIn = false;
		private string mDeviceAuthId = string.Empty;

		#region Authenticator implementation
		public void Init (object initData, System.Action<string, bool> callback)
		{
			mLoggedIn = SecurePrefs.GetInt("DeviceLoggedIn", 0) > 0;
			mDeviceAuthId = SecurePrefs.GetString("DeviceAuthId", Device.UniqueIdentifier);

			callback(null,true);
		}

		public void Authenticate(bool silent, System.Action<string, object> callback)
		{
			if (silent)
			{
				if (mLoggedIn)
				{
					var data = Johny.HashtablePool.Claim();
					data["udid"] = mDeviceAuthId;
					callback(null, data);
				}
				else
				{
					callback(null, null);
				}
			}
			else
			{
				mLoggedIn = true;

				SecurePrefs.SetInt("DeviceLoggedIn", EB.Time.Now);
				SecurePrefs.SetString("DeviceAuthId", mDeviceAuthId);

				var data = Johny.HashtablePool.Claim();
				data["udid"] = mDeviceAuthId;
				callback(null, data);
			}
		}

		public void Logout()
		{
			if (mLoggedIn)
			{
				SecurePrefs.DeleteKey("DeviceLoggedIn");
				mLoggedIn = false;
				mDeviceAuthId = string.Empty;
			}
		}

		public string Name {
			get {
				return "device";
			}
		}

		public bool IsLoggedIn {
			get {
				return mLoggedIn;
			}
		}

		public AuthenticatorQuailty Quailty {
			get {
				return AuthenticatorQuailty.Low;
			}
		}
		#endregion
	}
}