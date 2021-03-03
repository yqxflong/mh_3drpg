using UnityEngine;
using System.Collections.Generic;
using System;

public class AIhelpService
{
    private IElvaChatServiceSDK sdk;

    private static AIhelpService _instance;

    public static AIhelpService Instance
    {
        get{
			if (_instance == null)
			{
				//Debug.LogError ("AIHelp service is not initialized!");

				_instance = new AIhelpService ();
			}
			return _instance;
        }
    }

	public void Initialize(string appkey, string domain, string appId)
	{
		if (sdk != null)
		{
			sdk.init(appkey, domain, appId);
			postInitSetting ();
		}
	}

	public AIhelpService()
    {
        #if UNITY_ANDROID && USE_AIHELP
			sdk = new ElvaChatServiceSDKAndroid();
        #endif
		
		#if UNITY_IOS && USE_AIHELP
			sdk = new ElvaChatServiceSDKIOS();
		#endif
    }
		
	private void postInitSetting()
    {
        if(sdk != null)
        {
			SetIOSChangeDirection();//设置ios强制竖屏
        }
    }

    public void ShowFAQs()
    {
        if(sdk != null)
        {
			Dictionary<string, object> tags = new Dictionary<string, object> ();
            List<string> tag = new List<string>();
            tag.Add ("server1");
            tag.Add ("pay3");
            tags.Add ("elva-tags", tag);
            Dictionary<string, object> config = new Dictionary<string, object> ();
            config.Add ("elva-custom-metadata", tags);
            config.Add ("showContactButtonFlag", "1");
//            config.Add ("showConversationFlag", "1");
            sdk.showFAQs(config);
        }
    }
	public void ShowElva(string playerName,string playerUid,string serverId,string showConversationFlag)
	{
		if(sdk != null)
		{
			sdk.showElva(playerName, playerUid,serverId,showConversationFlag);
		}
	}
	public void ShowElva(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config)
	{
		if(sdk != null)
		{
			sdk.showElva(playerName, playerUid,serverId,showConversationFlag,config);
		}
	}
	public void ShowElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag)
	{
		if(sdk != null)
		{
			sdk.showElvaOP(playerName, playerUid,serverId,showConversationFlag);
		}
	}

	public void ShowElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config)
	{
		if(sdk != null)
		{
			sdk.showElvaOP(playerName, playerUid,serverId,showConversationFlag,config);
		}
	}

	public void ShowElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config, int tabIndex)
	{
		if(sdk != null)
		{
			sdk.showElvaOP(playerName, playerUid,serverId,showConversationFlag,config, tabIndex);
		}
	}
    //游戏名字
    public void SetName(string game_name)
    {
        if(sdk != null)
        {
            sdk.setName(game_name);
        }
    }

    public void SetUserId(string userId)
    {
        if(sdk != null)
        {
            sdk.setUserId(userId);
        }
    }

    public void SetUserName(string userName)
    {
        if(sdk != null)
        {
            sdk.setUserName(userName);
        }
    }

    public void SetServerId(string serverId)
    {
        if(sdk != null)
        {
            sdk.setServerId(serverId);
        }
    }

    public void ShowConversation(string uid,string serverId)
    {
        if(sdk != null)
        {
            sdk.showConversation(uid,serverId);
        }
    }

    public void SetSDKLanguage(string lang)
    {
        if(sdk != null)
        {
            sdk.setSDKLanguage(lang);
        }
    }

	public void ShowURL(string url)
	{
		if(sdk != null)
		{
			#if UNITY_ANDROID
			sdk.showURL(url);
			#endif
		}
	}

	public void ShowVIPChat(string webAppId, string vipTags)
	{
		if(sdk != null)
		{
			sdk.showVIPChat(webAppId,vipTags);
		}
	}

	public void SetFcmToken(string deviceToken,bool isVip)
	{
		if(sdk != null)
		{
			sdk.setFcmToken(deviceToken,isVip);
		}
	}

	public void ShowStoreReview()
	{
		if(sdk != null)
		{
			sdk.showStoreReview();
		}
	}

	public string GetNotificationMessage()
	{
		if (sdk != null)
		{
			#if UNITY_ANDROID
			return sdk.getNotificationMessage();
			#endif
			#if UNITY_IOS
			return sdk.getNotificationMessageCount().ToString();
			#endif
		}
		return "没有推送信息数据";
	}

	public void SetIOSChangeDirection()
	{
		if(sdk != null)
		{
			#if UNITY_IOS
			//sdk.setChangeDirection();
			#endif
		}
	}

	public void RegisterInitializationCallback(string gameObject)
	{
		if(sdk != null)
		{
			sdk.registerInitializationCallback(gameObject);
		}
	}

	public void RegisterMessageArrivedCallback(string gameObject)
	{
		if(sdk != null)
		{
			sdk.registerMessageArrivedCallback(gameObject);
		}
	}

	public void SetUnreadMessageFetchUid(string playerUid)
	{
		if(sdk != null)
		{
			sdk.setUnreadMessageFetchUid(playerUid);
		}
	}
	
	public void SetVIP(string playerName,string playerUid,Dictionary<string,object> config)
	{
		if(sdk != null)
		{
			sdk.setVIP(playerName,playerUid,config);
		}		
	}
	public void SetAccelerateDomain(string domain)
	{
		if(sdk != null)
		{
			sdk.setAccelerateDomain(domain);
		}		
	}	
}