using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace EB
{
	public static class Version
	{
		private const string _defaultVersionString = "0.0.0.0";

#if UNITY_IOS
		static string _cachedUdid = string.Empty;
		static string _cachedLanguageCode = string.Empty;
		//static string _cachedPreferredLanguageCode = string.Empty;
		static string _cachedCountryCode= string.Empty;
			
		[DllImport("__Internal")]
		static extern string _GetLanguageCode();
		
		[DllImport("__Internal")]
		static extern string _GetPreferredLanguageCode();
		
		[DllImport("__Internal")]
		static extern string _GetCountryCode();
		
		[DllImport("__Internal")]
		static extern string _GetOpenUDID();
		
		[DllImport("__Internal")]
		static extern string _GetUDID();
		
		[DllImport("__Internal")]
		static extern string _GetIFA();
		
		[DllImport("__Internal")]
		static extern string _GetMACAddress();
		
		[DllImport("__Internal")]
		static extern string _GetModel();
#endif

		public static string GetUDID()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			if (string.IsNullOrEmpty(_cachedUdid))
			{
				_cachedUdid = _GetUDID();
			}
			return _cachedUdid;
#else
			return SystemInfo.deviceUniqueIdentifier;
#endif
		}

		public static Hashtable GetDeviceInfo()
		{
			Hashtable data = Johny.HashtablePool.Claim();

			data["os"] = SystemInfo.operatingSystem;
#if UNITY_IPHONE && !UNITY_EDITOR
			var mac = _GetMACAddress();
			if(!string.IsNullOrEmpty(mac))
			{
				var bytes = Encoding.FromHexString(mac);
				data["mac"] = mac;
				data["odin1"] = Encoding.ToHexString(Digest.Sha1().Update(bytes).Final());
			}
			data["model"] = _GetModel();
			data["openudid"] = _GetOpenUDID();
			data["udid"] = _GetUDID();
			data["ifa"] = _GetIFA();
#elif UNITY_ANDROID && !UNITY_EDITOR
			data["mac"] = GetMACAddress(); 
			data["device_id"] = GetAndroidDeviceID();
			data["model"] = SystemInfo.deviceModel;
#elif UNITY_EDITOR
			data["mac"] = GetMACAddress();
			data["device_id"] = SystemInfo.deviceName;
			data["model"] = EB.Sparx.Device.DeviceModel;
			data["gpu"] = EB.Sparx.Device.DeviceGPU;
			data["caps"] = EB.Sparx.Device.DeviceCaps;
#endif
			return data;
		}

		private static string _version = null;
		private static string _changeList = "0";
		public static string GetVersion()
		{
			if(string.IsNullOrEmpty(_version)){
				var versionAsset = Resources.Load("version") as TextAsset;
				if (versionAsset)
				{
					_version = versionAsset.text.TrimEnd(new char[] { '\n', '\r' });
					string[] numbers = _version.Split('.');
					string[] shorts = new string[3];
					System.Array.Copy(numbers, shorts, shorts.Length);
					_version = string.Join(".", shorts);
					_changeList = numbers[numbers.Length - 1];
					EB.Assets.Unload("version");
				}
				else
				{
					_version = _defaultVersionString;
					Debug.LogError("Could not load version text file. Defaulting to " + _defaultVersionString);
				}
			}
			
			return _version;
		}

		public static string GetFullVersion()
		{
			string version = GetVersion();

			return string.Format("{0}.{1}", version, _changeList);
		}
        
		public static string GetVersionCode()
		{
			string version = GetVersion();

			return _changeList.ToString();
		}

		public static string GetLocale()
		{
			return GetLanguageCode() + "_" + GetCountryCode();
		}

		public static string GetLanguageCode()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			if (string.IsNullOrEmpty(_cachedLanguageCode))
			{
				_cachedLanguageCode = _GetLanguageCode();
			}
			return _cachedLanguageCode;
#elif UNITY_ANDROID && !UNITY_EDITOR
			switch (Application.systemLanguage)
			{
			case SystemLanguage.English:
				return EB.Localizer.GetLanguageCode(EB.Language.English);
			case SystemLanguage.French:
				return EB.Localizer.GetLanguageCode(EB.Language.French);
			case SystemLanguage.Italian:
				return EB.Localizer.GetLanguageCode(EB.Language.Italian);
			case SystemLanguage.German:
				return EB.Localizer.GetLanguageCode(EB.Language.German);
			case SystemLanguage.Spanish:
				return EB.Localizer.GetLanguageCode(EB.Language.Spanish);
			case SystemLanguage.Portuguese:
				return EB.Localizer.GetLanguageCode(EB.Language.Portuguese);
			case SystemLanguage.Russian:
				return EB.Localizer.GetLanguageCode(EB.Language.Russian);
			case SystemLanguage.Korean:
				return EB.Localizer.GetLanguageCode(EB.Language.Korean);
			case SystemLanguage.Chinese:
				AndroidJavaClass actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject resources = playerActivityContext.Call<AndroidJavaObject>("getResources");
				AndroidJavaObject configuration = resources.Call<AndroidJavaObject>("getConfiguration");
				AndroidJavaObject localeObj = configuration.Get<AndroidJavaObject>("locale");
				string locale = localeObj.Call<string>("toString");
				
				switch(locale)
				{
				case ("zh"):
				case ("zh_CN"):
				case ("zh_rCN"):
				case ("zh_Hans"):
				case ("zh_Hans_CN"):
				case ("zh_Hans_HK"):
				case ("zh_Hans_MO"):
				case ("zh_Hans_SG"):
				case ("zh_SG"):				//alias zh_Hans_SG
					return EB.Localizer.GetLanguageCode(EB.Language.ChineseSimplified);
					
				case ("zh_TW"):
				case ("zh_rTW"):
				case ("zh_Hant"):
				case ("zh_Hant_HK"):
				case ("zh_HK"): 			//alias zh_Hant_HK
				case ("zh_Hant_MO"):
				case ("zh_MO"): 			//alias zh_Hant_MO
				case ("zh_Hant_TW"):
					return EB.Localizer.GetLanguageCode(EB.Language.ChineseTraditional);
					
				default:
					return EB.Localizer.GetLanguageCode(EB.Language.ChineseSimplified);
				}
			case SystemLanguage.Japanese:
				return EB.Localizer.GetLanguageCode(EB.Language.Japanese);
			case SystemLanguage.Turkish:
				return EB.Localizer.GetLanguageCode(EB.Language.Turkish);
			default:
				return EB.Localizer.GetLanguageCode(EB.Language.English);
			}
#else
			return "en";
#endif
		}

		public static string GetCountryCode()
		{
#if UNITY_IPHONE && !UNITY_EDITOR
			if (string.IsNullOrEmpty(_cachedCountryCode))
			{
				_cachedCountryCode = _GetCountryCode();
			}
			return _cachedCountryCode;
#elif UNITY_ANDROID && !UNITY_EDITOR
			//TODO: Need to get real location, not location based off language set.
			AndroidJavaClass actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject playerActivityContext = actClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject resources = playerActivityContext.Call<AndroidJavaObject>("getResources");
			AndroidJavaObject configuration = resources.Call<AndroidJavaObject>("getConfiguration");
			AndroidJavaObject locale = configuration.Get<AndroidJavaObject>("locale");
			return locale.Call<string>("getCountry");
#else
			return "US";
#endif
		}

		public static int GetChangeList()
		{
			GetVersion();

			try
			{
				return int.Parse(_changeList);
			}
			catch
			{
				return 0;
			}
		}


		public static string GetIOSDeviceID()
		{
#if UNITY_IOS && !UNITY_EDITOR
			return _GetIFA();
#else
			return "PC_EDITOR";
#endif
		}


		public static string GetAndroidDeviceID()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			string deviceID = string.Empty;
			var deviceInfoClass = new AndroidJavaClass("org.manhuang.android.UnityAndroidDeviceInfo");
			if( deviceInfoClass != null )
			{
				deviceID = deviceInfoClass.CallStatic<string>( "GetDeviceID" );
			}
			return deviceID;
#else
			return "PC_EDITOR";
#endif
		}


		public static string GetMACAddress()
		{
			string macAddress = string.Empty;

#if UNITY_IPHONE && !UNITY_EDITOR
			macAddress = _GetMACAddress();
#elif UNITY_ANDROID && !UNITY_EDITOR
			var deviceInfoClass = new AndroidJavaClass("org.manhuang.android.UnityAndroidDeviceInfo");
			if( deviceInfoClass != null )
			{
				macAddress = deviceInfoClass.CallStatic<string>( "WifiMacAddress" );
				macAddress = macAddress.Replace( ":", "" );
			}
#elif UNITY_EDITOR
			macAddress = SystemInfo.deviceUniqueIdentifier;
#endif
			return macAddress;
		}

		private static int _TimeZoneOffset = -1;
		public static int GetTimeZoneOffset()
		{
			if (_TimeZoneOffset == -1)
			{
				_TimeZoneOffset = (int)System.TimeZone.CurrentTimeZone.GetUtcOffset(System.DateTime.Now).TotalSeconds;
			}

			return _TimeZoneOffset;
		}

#if UNITY_EDITOR
		public static void Reset()
		{
			_TimeZoneOffset = -1;
			_version = string.Empty;
			_changeList = "0";
		}
#endif
	}
}
