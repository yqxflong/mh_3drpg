using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EB.IAP.Internal
{
	public interface Provider
	{
		string Name {get;}
		void PurchaseItem( Item item);
		void Enumerate(List<Item> items);
		void Complete( Transaction transaction );
        void OnPayCallbackFromServer(string transactionId);
        string GetPayload(Transaction transaction);//获取SDK的相关信息
	}
    
    public enum ProviderPayWay
    {
        none = -1,
        wechat = 0,
        alipay,
    }

    internal class ProviderFactory
	{
		public static Provider Create( Config config, ProviderPayWay payWay = ProviderPayWay.none)
		{
#if UNITY_EDITOR||USE_GM_DEBUG_IAP
            return DebugProvider.Create(config);

#elif UNITY_IPHONE

#if USE_ASSDK
			return AsSDKProvider.Create(config);
#elif USE_XYSDK
			return XYSDKProvider.Create(config);
#elif USE_KUAIYONGSDK
			return KuaiYongSDKProvider.Create(config);
#elif USE_YIJIESDK
            return YiJieSDKProvider.Create(config);
#elif USE_K7KSDK
			return K7KSDKProvider.Create(config);
#elif USE_QINGYUANSDK
			return QingYuanSDKProvider.Create(config);
#elif USE_AIBEISDK
            return AibeiSDKProvider.Create(config);
#elif USE_VFPKSDK
            return VFPKSDKProvider.Create(config);
#elif USE_XINKUAISDK
            return XinkuaiSDKProvider.Create(config);
#elif USE_AOSHITANGSDK
            return IAPAoshitangSDKProvider.Create(config);
#else
			return AppleProvider.Create(config);
#endif

#elif UNITY_ANDROID

#if USE_UCSDK
			return UCSDKProvider.Create( config );
#elif USE_AMAZON
			return AmazonProvider.Create( config );
#elif USE_GOOGLE
            return IAPGooglePayProvider.Create( config );
#elif USE_QIHOOSDK
			return QiHooSDKProvider.Create(config);
#elif USE_XIAOMISDK
			return XiaoMiSDKProvider.Create(config);
#elif USE_VIVOSDK
			return VivoSDKProvider.Create(config);
#elif USE_OPPOSDK
			return OPPOSDKProvider.Create(config);
#elif USE_TENCENTSDK
			return TencentSDKProvider.Create(config);
#elif USE_HUAWEISDK
            return HuaweiSDKProvider.Create(config);
#elif USE_WINNERSDK
            return WinnerSDKProvider.Create(config);
#elif USE_YIJIESDK
            return YiJieSDKProvider.Create(config);
#elif USE_EWANSDK
			return EWanSDKProvider.Create(config);
#elif USE_LBSDK
			return LBSDKProvider.Create(config);
#elif USE_AIBEISDK
            return AibeiSDKProvider.Create(config);
#elif USE_ASDK
			return ASDKProvider.Create( config );
#elif USE_M4399SDK
			return M4399SDKProvider.Create( config );
#elif USE_WECHATSDK && USE_ALIPAYSDK
            if (payWay== ProviderPayWay.wechat)
                return WeChatSDKProvider.Create(config);
            else if(payWay == ProviderPayWay.alipay)
                return AlipaySDKProvider.Create(config);
            else
                return null;
#elif USE_WECHATSDK
            return WeChatSDKProvider.Create( config );
#elif USE_ALIPAYSDK
            return AlipaySDKProvider.Create( config );
#elif USE_VFPKSDK
              return VFPKSDKProvider.Create( config );
#elif USE_XINKUAISDK
              return XinkuaiSDKProvider.Create( config );
#elif USE_AOSHITANGSDK
            return IAPAoshitangSDKProvider.Create(config);
#else
			return DefaultProvider.Create( config );
#endif

#else
			return DefaultProvider.Create( config );
#endif
        }
    }
	
}
