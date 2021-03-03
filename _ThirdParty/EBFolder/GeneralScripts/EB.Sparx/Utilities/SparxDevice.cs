using UnityEngine;
using System.Collections;

namespace EB.Sparx
{
	public static class Device
	{
		public static string UniqueIdentifier
		{
			get
			{
				var udid = PlayerPrefs.GetString("udid", string.Empty);
				if ( string.IsNullOrEmpty(udid) )
				{
					udid = Version.GetUDID();
					if ( string.IsNullOrEmpty(udid) )
					{
						udid = NewUniqueIdentifier();
					}
				}

#if UNITY_EDITOR
				udid += "-EDITOR";
#endif

				return udid;
			}
			set
			{
				PlayerPrefs.SetString("udid", value );
				PlayerPrefs.Save();
				EB.Debug.Log("Setting udid: " + value);
			}
		}

		public static string NewUniqueIdentifier()
		{
			var udid = System.Guid.NewGuid().ToString();
			PlayerPrefs.SetString("udid", udid);
			PlayerPrefs.Save();
			return udid;
		}

		public static string Platform
		{
			get
			{
#if UNITY_EDITOR
				return "editor";
#elif UNITY_IPHONE
				return "iphone";
#elif UNITY_ANDROID
				return "android";
#else
				return "unknown";
#endif
			}
		}

		public static string DeviceModel
		{
			get
			{
#if UNITY_EDITOR
				return "editor";
#elif UNITY_IPHONE
				return UnityEngine.iOS.Device.generation.ToString();
#else
				return SystemInfo.deviceModel;
#endif
			}
		}

		public static string DeviceGPU
		{
			get
			{
#if UNITY_EDITOR
				return "editor";
#else
				return SystemInfo.graphicsDeviceName;
#endif
			}
		}

		public static string DeviceCaps
		{
			get
			{
#if UNITY_EDITOR
				return "editor";
#else
				return SystemInfo.graphicsDeviceVersion;
#endif
			}
		}

		public static string MobilePlatform
		{
			get
			{
#if UNITY_IPHONE
				return "iphone";
#else
				return "android";
#endif
			}
		}

		public static string Source
		{
			get
			{
#if UNITY_EDITOR
				return "editor";
#elif UNITY_IPHONE
#if USE_ASSDK
				return "as";
#elif USE_MH_LOGIN
                return "manhuang";
#elif USE_XYSDK
				return "xy";
#elif USE_KUAIYONGSDK
				return "kuaiyong";
#elif USE_WINNERIOSSDK
				return "winner";
#elif USE_YIJIESDK
				return "yijie";
#elif USE_K7KSDK
				return "k7k";
#elif USE_QINGYUANSDK
				return "qingyuan";		
#elif USE_CHANGDASHISDK
				return "changdashi";
#else
				return "manhuang";  //iphone
#endif
#elif UNITY_ANDROID
#if USE_GOOGLE
				return "google";
#elif USE_MH_LOGIN
                return "manhuang";
#elif USE_AMAZON
				return "amazon";
#elif USE_UCSDK
				return "uc";
#elif USE_QIHOOSDK
				return "qihoo";
#elif USE_XIAOMISDK
				return "xiaomi";
#elif USE_VIVOSDK
				return "vivo";
#elif USE_OPPOSDK
				return "oppo";
#elif USE_TENCENTSDK
				return "tencent";
#elif USE_WINNERSDK
				return "winner";
#elif USE_HUAWEISDK
				return "huawei";
#elif USE_YIJIESDK
				return "yijie";
#elif USE_EWANSDK
				return "ewan";
#elif USE_LBSDK
				return "lb";
#elif USE_ASDK
                return "asdk";
#elif USE_M4399SDK
			    return "m4399";
#elif USE_WECHATSDK
			    return "wechat";
#elif USE_ALIPAYSDK
			    return "alipay";
#elif USE_VFPKSDK
			    return "vfpk";
#elif USE_XINKUAISDK
			    return "xinkuai";
#elif USE_AOSHITANGSDK
				return "aoshitang";
#else
				return "android";
#endif
#else
				return "unknown";
#endif
			}
        }

		public static string ChildSource
		{
			get
			{
#if USE_YIJIESDK
				return EB.Sparx.YiJieSDK.pxGetChannelId();
#else
				return "";
#endif
			}
		}
	}
}


