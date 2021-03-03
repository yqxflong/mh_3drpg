#if  USE_GOOGLE && UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using EB;
using EB.Sparx;
using UnityEngine;
using Debug = EB.Debug;


/// <summary>
/// Google 账户
/// </summary>
public class SparxGoogleSDKAuthenticator : Authenticator
{

    private GoogleSDKManager mManager;

    /// <summary>
    /// 服务器用于校验的登录方式名称
    /// </summary>
    public string Name
    {
        get
        {
            //Init(null,null);
            return "google";
        }
    }

    /// <summary>
    /// 是否登录
    /// </summary>
    public bool IsLoggedIn {
        get { return mManager.isLogin; }
    }

    /// <summary>
    /// 登录优先级
    /// </summary>
    public AuthenticatorQuailty Quailty
    {
        get { return AuthenticatorQuailty.High; }
    }

    /// <summary>
    /// SparxLoginManager集中进行账户创建
    /// </summary>
    /// <param name="silent">是否静默登录</param>
    /// <param name="callback">创建账户成功回调</param>
    public void Authenticate(bool silent, Action<string, object> callback)
    {
        UnityEngine.Debug.Log("---------------------SparxGoogleSDKAuthenticator----------------------Authenticate");
        if (silent)
        {
            if (IsLoggedIn)
            {
                callback(null, new Hashtable());
            }
            else
            {
                callback(null, null);
            }
            return;
        }
        else
        {
            mManager.Login(callback);
        }
        //mManager.Login(callback);
    }

    /// <summary>
    /// 账户初始化
    /// </summary>
    /// <param name="initData"></param>
    /// <param name="callback"></param>
    public void Init(object initData, Action<string, bool> callback)
    {
       EB.Debug.Log("GoogleSDK init");
        mManager = SparxHub.Instance.GoogleSDKManager;
        mManager.InitializeSDK(initData, callback);
    }

    /// <summary>
    /// 账户登出
    /// </summary>
    public void Logout()
    {
        mManager.Logout();
    }
}
#endif