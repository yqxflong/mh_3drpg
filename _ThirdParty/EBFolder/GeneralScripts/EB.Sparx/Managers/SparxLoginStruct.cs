using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EB.Sparx
{
	public enum LoginState
	{
		Disconnected,
		Initializing,
		Initialized,
		Authenticating,
		Authenticated,
		LoggedIn,
	}
	
    public interface LoginConfigListener
    {
        void OnMHLogin(object param);
        void OnLoggedIn();
        void OnLoginFailed(string error);
        void OnDisconnected(string error);
        void OnUpdateRequired(string storeUrl);
    }
    
    public class LoginConfig
	{
        public LoginConfigListener Listener;
        public System.Type[] Authenticators = new System.Type[]
        {
			//设备登陆
            typeof(DeviceAuthenticator),

#if USE_ACCOUNT_AUTH
			//账号登陆
            typeof(AccountAuthenticator),
#endif

#if UNITY_IOS && USE_GAMECENTER_AUTH
			//Gamecenter登陆
            typeof(GameCenterAuthenticator),
#endif

#if USE_MH_LOGIN
			//蛮荒账号登陆
			typeof(MHAuthenticator),
#endif

#if USE_FB
			typeof(FacebookAuthenticator),
#endif

#if USE_ASSDK && UNITY_IPHONE
			typeof(AsSDKAuthenticator),
#endif

#if USE_XYSDK && UNITY_IPHONE
			typeof(XYSDKAuthenticator),
#endif

#if USE_KUAIYONGSDK && UNITY_IPHONE
			typeof(KuaiYongSDKAuthenticator),
#endif

#if USE_WINNERIOSSDK && UNITY_IPHONE
			typeof(WinnerIOSSDKAuthenticator),
#endif

#if USE_UCSDK && UNITY_ANDROID
			typeof(UCSDKAuthenticator),
#endif

#if USE_QIHOOSDK && UNITY_ANDROID
			typeof(QiHooSDKAuthenticator),
#endif

#if USE_XIAOMISDK && UNITY_ANDROID
			typeof(XiaoMiSDKAuthenticator),
#endif

#if USE_VIVOSDK && UNITY_ANDROID
			typeof(VivoSDKAuthenticator),
#endif

#if USE_OPPOSDK && UNITY_ANDROID
			typeof(OPPOSDKAuthenicator),
#endif

#if USE_TENCENTSDK && UNITY_ANDROID
			typeof(QQAuthenticator),
			typeof(WeiXinAuthenticator),
#endif

#if USE_WINNERSDK && UNITY_ANDROID
			typeof(WinnerSDKAuthenticator),
#endif

#if USE_HUAWEISDK && UNITY_ANDROID
			typeof(HuaweiSDKAuthenticator),
#endif
#if USE_EWANSDK && UNITY_ANDROID
			typeof(EWanSDKAuthenticator),
#endif

#if USE_LBSDK && UNITY_ANDROID
			typeof(LBSDKAuthenticator),
#endif

#if USE_YIJIESDK
			typeof(YiJieSDKAuthenticator),
#endif

#if USE_K7KSDK
			typeof(K7KSDKAuthenticator),
#endif

#if USE_QINGYUANSDK
			typeof(QingYuanSDKAuthenticator),
#endif

#if USE_AIBEISDK
            typeof(AibeiSDKAuthenticator),
#endif

#if USE_ASDK
            typeof(ASDKAuthenicator),
#endif

#if USE_GOOGLE
		    typeof(SparxGoogleSDKAuthenticator),
#endif

#if USE_M4399SDK
            typeof(M4399SDKAuthenticator),
#endif

#if USE_WECHATSDK
            typeof(SparxWeChatSDKAuthenticator),
#endif

#if USE_ALIPAYSDK
            typeof(SparxAlipaySDKAuthenticator),
#endif

#if USE_VFPKSDK
            typeof(SparxVFPKSDKAuthenticator),
#endif

#if USE_XINKUAISDK
			//新快登陆
            typeof(SparxXinkuaiSDKAuthenticator),
#endif

#if USE_AOSHITANGSDK
			//傲世堂登陆
            typeof(SparxAoshitangAuthenticator),
#endif
        };
	}

    public class LoginDataStore
    {
        public Id LocalUserId;
        public Hashtable LoginData;
    }
    
}

