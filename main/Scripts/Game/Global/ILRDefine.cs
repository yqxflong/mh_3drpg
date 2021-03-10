#if USE_DEBUG
#define ENABLE_LOGGING
#endif

public class ILRDefine
{
#if UNITY_EDITOR
    public static bool UNITY_EDITOR = true;
#else
    public static bool UNITY_EDITOR = false;
#endif

#if UNITY_IPHONE
    public static bool UNITY_IPHONE = true;
#else
    public static bool UNITY_IPHONE = false;
#endif

#if UNITY_ANDROID
    public static bool UNITY_ANDROID = true;
#else
    public static bool UNITY_ANDROID = false;
#endif
    
#if ENABLE_LOGGING
    public static bool ENABLE_LOGGING = true;
#else
    public static bool ENABLE_LOGGING = false;
#endif

#if DEBUG
    public static bool DEBUG = true;
#else
    public static bool DEBUG = false;
#endif
    
#if USE_VFPKSDK
    public static bool USE_VFPKSDK = true;
#else
    public static bool USE_VFPKSDK = false;
#endif
    
#if USE_UMENG
    public static bool USE_UMENG = true;
#else
    public static bool USE_UMENG = false;
#endif
    
#if USE_GM
    public static bool USE_GM = true;
#else
    public static bool USE_GM = false;
#endif
    
    
#if USE_XINKUAISDK
    public static bool USE_XINKUAISDK = true;
#else
    public static bool USE_XINKUAISDK = false;
#endif


#if USE_WECHATSDK
    public static bool USE_WECHATSDK = true;
#else
    public static bool USE_WECHATSDK = false;
#endif 
    
#if USE_ALIPAYSDK
    public static bool USE_ALIPAYSDK = true;
#else
    public static bool USE_ALIPAYSDK = false;
#endif

#if IS_FX
    public static bool IS_FX = true;
#else
    public static bool IS_FX = false;
#endif

#if USE_APPSFLYER
    public static bool USE_APPSFLYER = true;
#else
    public static bool USE_APPSFLYER = false;
#endif

#if USE_AOSHITANGSDK
    public static bool USE_AOSHITANGSDK = true;
#else
    public static bool USE_AOSHITANGSDK = false;
#endif

#if USE_ASTTEST||USE_AOSHITANGSDK
    public static bool USE_ASTTEST = true;
#else
    public static bool USE_ASTTEST = false;
#endif

#if USE_AIHELP
    public static bool USE_AIHELP = true;
#else
    public static bool USE_AIHELP = false;
#endif

#if USE_OVERSEAS
    public static bool USE_OVERSEAS = true;
#else
    public static bool USE_OVERSEAS = false;
#endif

    public void Test()
    {
        if (ILRDefine.ENABLE_LOGGING)
        {
            
        }
        else
        {
            
        }
        
        if (ILRDefine.UNITY_EDITOR)
        {
            
        } else
        {
            
        }

        if (ILRDefine.USE_VFPKSDK)
        {
            
        }
        
        if (ILRDefine.UNITY_ANDROID)
        {
            
        }

        if (ILRDefine.DEBUG)
        {
            
        }

        if (ILRDefine.USE_UMENG)
        {
            
        }
    }
}