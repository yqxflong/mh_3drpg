#if UNITY_IOS && USE_AIHELP
using UnityEngine;
using System;
using System.Collections;
// We need this one for importing our IOS functions
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AIHelpMiniJSON;

public class ElvaChatServiceSDKIOS:IElvaChatServiceSDK
{

    		[DllImport ("__Internal")]
			private static extern void elvaInit (string appKey,string domain,string appId);
			[DllImport ("__Internal")]

			private static extern void elvaShowElva (string playerName,string playerUid,string serverId,string playerParseId,string showConversationFlag);
			[DllImport ("__Internal")]

			private static extern void elvaShowElvaOP (string playerName,string playerUid,string serverId,string playerParseId,string showConversationFlag,string config);
			[DllImport ("__Internal")]

			private static extern void elvaShowElvaOPWithTabIndex (string playerName,string playerUid,string serverId,string playerParseId,string showConversationFlag,string config,int defaultTabIndex);
			[DllImport ("__Internal")]

			private static extern void elvaShowElvaWithConfig (string playerName,string playerUid,string serverId,string playerParseId,string showConversationFlag,string jsonConfig);
			[DllImport ("__Internal")]
			private static extern void elvaShowConversation (string playerUid,string serverId);
			[DllImport ("__Internal")]
			private static extern void elvaShowConversationWithConfig (string playerUid,string serverId,string jsonConfig);
			[DllImport ("__Internal")]
			private static extern void elvaShowSingleFAQ (string faqId);
			[DllImport ("__Internal")]
			private static extern void elvaShowSingleFAQWithConfig (string faqId,string jsonConfig);
			[DllImport ("__Internal")]
			private static extern void elvaShowFAQSection (string sectionPublishId);
			[DllImport ("__Internal")]
			private static extern void elvaShowFAQSectionWithConfig (string sectionPublishId,string jsonConfig);
			[DllImport ("__Internal")]
			private static extern void elvaShowFAQList ();
			[DllImport ("__Internal")]
			private static extern void elvaShowFAQListWithConfig (string jsonConfig);
			[DllImport ("__Internal")]
			private static extern void elvaSetName (string gameName);
			[DllImport ("__Internal")]
			private static extern void elvaSetUserId (string playerUid);
			[DllImport ("__Internal")]
			private static extern void elvaSetServerId (string serverId);
			[DllImport ("__Internal")]
			private static extern void elvaSetUserName (string playerName);
			[DllImport ("__Internal")]
			private static extern void elvaSetSDKLanguage(string sdkLanguage);
			[DllImport ("__Internal")]
			private static extern void elvaSetUseDevice ();
			[DllImport ("__Internal")]
			private static extern void elvaSetEvaluateStar (int star);
			[DllImport ("__Internal")]
			private static extern void elvaRegisterDeviceToken (string deviceToken,bool isVip);
			[DllImport ("__Internal")]
			private static extern void elvaShowVIPChat (string webAppId, string vipTags);
			[DllImport ("__Internal")]
			private static extern void elvaShowStoreReview();
			[DllImport ("__Internal")]
			private static extern void elvaSetChangeDirection();
			[DllImport ("__Internal")]
			private static extern int elvaGetNotificationMessageCount();
			[DllImport ("__Internal")]
			private static extern void elvaShowQACommunity(string playerUid, string playerName);
			[DllImport ("__Internal")]
			private static extern void elvaRegisterInitializationCallback(string gameObject);
			[DllImport ("__Internal")]
			private static extern void elvaRegisterMessageArrivedCallback(string gameObject);
			[DllImport ("__Internal")]
			private static extern void elvaSetUnreadMessageFetchUid(string playerUid);
			[DllImport ("__Internal")]
			private static extern void elvaSetVIP(string playerName,string playerUid,string config);
			[DllImport ("__Internal")]
			private static extern void elvaSetAccelerateDomain(string domain);
			[DllImport ("__Internal")]
			private static extern void elvaShowURL(string url);
			
	public ElvaChatServiceSDKIOS()
	{

	}
    public ElvaChatServiceSDKIOS(string appKey, string domain, string appId)
	{
        init(appKey, domain, appId);
	}
	public void init(string appKey,string domain,string appId){
		elvaInit(appKey,domain,appId);
	}
	public void showElva(string playerName,string playerUid,string serverId,string showConversationFlag)
	{
		elvaShowElva(playerName,playerUid,serverId,"",showConversationFlag);
	}
	public void showElva(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config)
	{
		string jsonConf = AIHelpMiniJSON.Json.Serialize(config);
		elvaShowElvaWithConfig(playerName,playerUid,serverId,"",showConversationFlag,jsonConf);
	}
	public void showElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag)
	{
		elvaShowElvaOP(playerName,playerUid,serverId,"",showConversationFlag,AIHelpMiniJSON.Json.Serialize(""));
	}
	public void showElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config)
	{
		elvaShowElvaOP(playerName,playerUid,serverId,"",showConversationFlag,AIHelpMiniJSON.Json.Serialize(config));
	}
	public void showElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config,int defaultTabIndex)
	{
		elvaShowElvaOPWithTabIndex(playerName,playerUid,serverId,"",showConversationFlag,AIHelpMiniJSON.Json.Serialize(config), defaultTabIndex);
	}
	public void showConversation(string playerUid,string serverId)
	{
		//call setUserName before this method
		elvaShowConversation(playerUid,serverId);
	}
	public void showConversation(string playerUid,string serverId,Dictionary<string,object> config)
	{
		elvaShowConversationWithConfig(playerUid,serverId,AIHelpMiniJSON.Json.Serialize(config));
	}
	public void showSingleFAQ(string faqId)
	{
		elvaShowSingleFAQ(faqId);
	}
	public void showSingleFAQ(string faqId,Dictionary<string,object> config)
	{
		elvaShowSingleFAQWithConfig(faqId,AIHelpMiniJSON.Json.Serialize(config));
	}
	public void showFAQSection(string sectionPublishId)
	{
		elvaShowFAQSection(sectionPublishId);
	}
	public void showFAQSection(string sectionPublishId,Dictionary<string,object> config)
	{
		elvaShowFAQSectionWithConfig(sectionPublishId, AIHelpMiniJSON.Json.Serialize(config));
	}
	public void showFAQs()
	{
		elvaShowFAQList();
	}
	public void showFAQs(Dictionary<string,object> config)
	{
		elvaShowFAQListWithConfig(Json.Serialize(config));
	}
	public void setName(string gameName)
	{
		elvaSetName(gameName);
	}

	public void setUserId(string playerUid)
	{//自助服务，在showFAQ之前调用
		elvaSetUserId(playerUid);
	}
	public void setServerId(string serverId)
	{//自助服务，在showFAQ之前调用
		elvaSetServerId(serverId);
	}
	public void setUserName(string playerName)
	{//在需要的接口之前调用，建议游戏刚进入就默认调用
		elvaSetUserName(playerName);
	}
	public void setSDKLanguage(string sdkLanguage)
	{
		elvaSetSDKLanguage(sdkLanguage);
	}

	public void setUseDevice()
	{
		elvaSetUseDevice();
	}

	public void setEvaluateStar(int star)
	{
		elvaSetEvaluateStar(star);
	}

	public void setFcmToken (string deviceToken,bool isVip)
	{
		elvaRegisterDeviceToken(deviceToken,isVip);
	}

	public void showVIPChat (string webAppId, string vipTags)
	{
		elvaShowVIPChat(webAppId, vipTags);
	}

	public void showStoreReview()
	{
		elvaShowStoreReview ();
	}

	public void setChangeDirection()
	{
		elvaSetChangeDirection();
	}

	public int getNotificationMessageCount()
	{
		return elvaGetNotificationMessageCount();
	}

	public void showQACommunity(string playerUid, string playerName)
	{
		elvaShowQACommunity(playerUid,playerName);
	}

	public void registerInitializationCallback(string gameObject)
	{
		elvaRegisterInitializationCallback(gameObject);
	}	

	public void registerMessageArrivedCallback(string gameObject)
	{
		elvaRegisterMessageArrivedCallback(gameObject);
	}

	public void setUnreadMessageFetchUid(string playerUid)
	{
		elvaSetUnreadMessageFetchUid(playerUid);
	}
	
	public void setVIP(string playerName, string playerUid, Dictionary<string,object> config)
	{
		string jsonConf = AIHelpMiniJSON.Json.Serialize(config);
		elvaSetVIP(playerName,playerUid,jsonConf);
	}
	public void setAccelerateDomain(string domain)
	{
		elvaSetAccelerateDomain(domain);
	}

    public void showURL(string url)
	{
		elvaShowURL(url);
	}
}
#endif