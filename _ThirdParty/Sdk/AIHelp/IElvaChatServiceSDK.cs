using System.Collections.Generic;
public interface IElvaChatServiceSDK
{
	void init (string appKey, string domain, string appId);
	void showElva(string playerName,string playerUid,string serverId,string showConversationFlag);
	void showElva(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config);
	void showElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag);
	void showElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config);
	void showElvaOP(string playerName,string playerUid,string serverId,string showConversationFlag,Dictionary<string,object> config, int defaultTabIndex);
	void showFAQs();
	void showFAQs(Dictionary<string,object> config);
	void showConversation(string playerUid,string serverId);
	void showConversation(string playerUid,string serverId, Dictionary<string,object> config);
	void showSingleFAQ (string faqId);
	void showSingleFAQ (string faqId, Dictionary<string,object> config);
    void setName(string game_name);
	void setUserName(string userName);
    void setUserId(string userId);
    void setServerId(string serverId);
	void setSDKLanguage (string language);
	void setFcmToken (string deviceToken,bool isVip);
	void showVIPChat (string webAppId,string vipTags);
	void showStoreReview ();
	void setVIP(string playerName,string playerUid,Dictionary<string,object> config);
	void setAccelerateDomain(string domain);
	void showURL (string URL);
	#if UNITY_IOS
	void setChangeDirection();
	int getNotificationMessageCount();
	#endif

	#if UNITY_ANDROID
	string getNotificationMessage ();
	#endif
	void registerInitializationCallback(string gameObject);
	void registerMessageArrivedCallback(string gameObject);
	void setUnreadMessageFetchUid (string userId);

}