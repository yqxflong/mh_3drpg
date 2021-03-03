///////////////////////////////////////////////////////////////////////
//
//  FusionTelemetry.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using Umeng;
using System.Collections.Generic;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public static class FusionTelemetry
    {
        public class BaseData
        {
            //public string topic;//消息主题
            //public string event_id;//事件名：8001战斗，8002任务，8003副本，8004登录，8005货币，80006新手引导

            public int app_id = 1;
            public int channel_id;
            public int server_id;
            public long naid_id;
            public long udid;

            public string role_name;//角色名
            public int role_level;//角色等级
            public string event_time;//事件时间
            public int event_timestamp;//事件时间戳

            /// <summary>
            /// 基础友盟统计日志
            /// </summary>
            /// <param name="app_id">APPID</param>
            /// <param name="channel_id">渠道id：1快游，2taptap</param>
            /// <param name="server_id">游戏服务器id</param>
            /// <param name="naid_id">账号id</param>
            /// <param name="udid">UID</param>
            public BaseData(int app_id, int channel_id, int server_id, long naid_id, long udid)
            {
                this.app_id = app_id;
                this.channel_id = channel_id;
                this.server_id = server_id;
                this.naid_id = naid_id;
                this.udid = udid;
            }

            private void UpdateInfo()
            {
                DataLookupsCache.Instance.SearchDataByID<string>("user.name", out role_name);
                DataLookupsCache.Instance.SearchIntByID("level", out role_level);
                event_time = EB.Time.LocalNow.ToString("yyyy-MM-dd hh:mm:ss");
                event_timestamp = EB.Time.Now;
            }

            public Dictionary<string, object> ToDictionary()
            {
                UpdateInfo();
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("app_id", app_id);
                data.Add("channel_id", channel_id);
                data.Add("server_id", server_id);
                data.Add("naid_id", naid_id);
                data.Add("udid", udid);
                data.Add("role_name", role_name);
                data.Add("role_level", role_level);
                data.Add("event_time", event_time);
                data.Add("event_timestamp", event_timestamp);
                return data;
            }
        }

        public class MatchData
        {
            public const string UmengId = "match";

            public const string topic = "战斗";
            public const string event_id = "8001";

            /// <summary>
            /// 战斗日志
            /// </summary>
            /// <param name="gameId">战斗id</param>
            /// <param name="gameType">战斗类型</param>
            /// <param name="chapterId">章节id</param>
            /// <param name="Result">战斗结果：1胜利，2失败</param>
            /// <param name="startTime">战斗开始时间</param>
            /// <param name="gameTime">战斗耗时</param>
            /// <param name="hero_Lv">伙伴信息{name名字,num等级}</param>
            /// <param name="isFinish">是否中途退出：1完成，2中途退出</param>
            public static void PostEvent(long gameId, string gameType, int chapterId, int Result, int startTime, int gameTime, List<int> hero_Id, int isFinish)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("gameId", gameId);
                data.Add("gameType", gameType);
                data.Add("chapterId", chapterId);
                data.Add("Result", Result);
                data.Add("startTime", startTime);
                data.Add("gameTime", gameTime);
                // data.Add("hero_Lv", hero_Lv);
                data.Add("isFinish", isFinish);
                AddUmengEvent(UmengId, data);

                for (int i = 0; i < hero_Id.Count; i++)
                {
                    Dictionary<string, object> data_i = new Dictionary<string, object>();
                    data_i.Add("hero_Id", hero_Id[i]);
                    AddUmengEvent("match_partner", data_i);
                }
            }
        }

        public class TaskData
        {
            public const string UmengId = "task";

            public const string topic = "任务完成";
            public const string event_id = "8002";

            /// <summary>
            /// 任务日志
            /// </summary>
            /// <param name="taskID">任务id</param>
            /// <param name="taskName">任务名称</param>
            /// <param name="taskType">任务类型</param>
            public static void PostEvent(int taskID, string taskName, string taskType)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("taskID", taskID);
                data.Add("taskName", taskName);
                data.Add("taskType", taskType);
                AddUmengEvent(UmengId, data);
            }
        }

        public class CampaignDate
        {
            public const string UmengId = "campaign";

            public const string topic = "副本完成";
            public const string event_id = "8003";

            public const int Main = 1;
            public const int Challenge = 2;

            /// <summary>
            /// 副本完成日志
            /// </summary>
            /// <param name="campaignType">副本：1主线，2挑战</param>
            /// <param name="campaignID">章节id</param>
            /// <param name="isFinish">是否完成章节：1完成，2中途退出</param>
            /// <param name="Result">章节是否胜利：1胜利，2失败</param>
            public static void PostEvent(int campaignType, string campaignID, int isFinish, int Result)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("campaignType", campaignType);
                data.Add("campaignID", campaignID);
                data.Add("isFinish", isFinish);
                data.Add("Result", Result);

                AddUmengEvent(UmengId, data);
            }
        }

        public class LoginData
        {
            public const string UmengId = "login";

            public const string topic = "登录";
            public const string event_id = "8004";

            /// <summary>
            /// 登录日志
            /// </summary>
            /// <param name="vipType">特权：0未开通，1黄金，2永久，3两者</param>
            /// <param name="lastloginTime">上次登录时间戳</param>
            /// <param name="payAmount">充值总额</param>
            /// <param name="battlepass">是否购买密令：0未买，1购买</param>
            /// <param name="campaignLevel">主线进度</param>
            /// <param name="challengeLevel">挑战进度</param>
            public static void PostEvent(int vipType, int lastloginTime, int payAmount, int battlepass, string campaignLevel, string challengeLevel)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("vipType", vipType);
                data.Add("lastloginTime", lastloginTime);
                data.Add("payAmount", payAmount);
                data.Add("battlepass", battlepass);
                data.Add("campaignLevel", campaignLevel);
                data.Add("challengeLevel", challengeLevel);
                AddUmengEvent(UmengId, data);
            }
        }

        public class CurrencyChangeData
        {
            public const string UmengId = "currency_change";

            //public string time;//时间

            public const string topic = "货币变化";
            public const string event_id = "8005";

            public const string hc = "钻石";
            public const string gold = "金币";


            /// <summary>
            /// 货币变化日志
            /// </summary>
            /// <param name="currency_type">货币类型（钻石，金币）</param>
            /// <param name="currency_count">货币变化量</param>
            /// <param name="reason">变化原因（商店购买、钻石抽奖）</param>
            /// <param name="currency_left">货币变化后数量</param>
            /// <param name="items_id">涉及物品名</param>
            /// <param name="items_num">物品数量</param>
            public static void PostEvent(string currency_type, int currency_count, string reason, string items_id = "", int items_num = 0)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("currency_type", currency_type);
                data.Add("currency_count", currency_count);
                data.Add("reason", reason);
                if (currency_type.Equals(hc))
                {
                    int currency_left = BalanceResourceUtil.GetUserDiamond();
                    data.Add("currency_left", currency_left);
                }
                else if (currency_type.Equals(gold))
                {
                    int currency_left = BalanceResourceUtil.GetUserGold();
                    data.Add("currency_left", currency_left);
                }
                data.Add("items_id", items_id);
                data.Add("items_num", items_num);
                AddUmengEvent(UmengId, data);
            }
        }

        public class GuideData
        {
            public const string UmengId = "guide";

            public const string topic = "新手引导";
            public const string event_id = "8006";

            /// <summary>
            /// 新手引导日志
            /// </summary>
            /// <param name="umengId">友盟id</param>
            /// <param name="stepID">步骤id</param>
            /// <param name="createTime">角色创建时间戳</param>
            public static void PostEvent(string umengId, int stepID, int createTime)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("umengId", umengId);
                data.Add("stepID", stepID);
                data.Add("createTime", createTime);
                AddUmengEvent(UmengId, data);
            }
        }

        public class GamePlayData
        {
            public const string arena_topic = "参与竞技场";
            public const string arena_event_id = "8101";
            public const string arena_umengId = "play_arena";
            
            public const string ladder_topic = "天梯";
            public const string ladder_event_id = "8102";
            public const string ladder_umengId = "play_ladder";
            
            public const string infinite_topic = "极限试炼";
            public const string infinite_event_id = "8103";
            public const string infinite_umengId = "play_infinite_challenge";
            
            public const string bounty_topic = "悬赏";
            public const string bounty_event_id = "8104";
            public const string bounty_umengId = "play_bounty";
            
            public const string clash_topic = "英雄交锋";
            public const string clash_event_id = "8105";
            public const string clash_umengId = "play_clash_hero";
            
            public const string sleep_topic = "睡梦之塔";
            public const string sleep_event_id = "8106";
            public const string sleep_umengId = "play_sleep_tower";
            
            public const string honor_topic = "荣耀竞技场";
            public const string honor_event_id = "8107";
            public const string honor_umengId = "play_honor_arena";

            public const string alliance_camp_topic = "军团副本";
            public const string alliance_camp_event_id = "8108";
            public const string alliance_camp_umengId = "play_alliances_camp";

            public const string alliance_war_topic = "军团战";
            public const string alliance_war_event_id = "8109";
            public const string alliance_war_umengId = "play_alliances_war";

            public const string gold_camp_topic = "金币副本";
            public const string gold_camp_event_id = "8110";
            public const string gold_camp_umengId = "play_activity_treasure";

            public const string exp_camp_topic = "经验副本";
            public const string exp_camp_event_id = "8111";
            public const string exp_camp_umengId = "play_activity_exp";

            public const string esort_topic = "运镖";
            public const string esort_event_id = "8112";
            public const string esort_umengId = "play_esort";

            public const string worldboss_topic = "世界boss";
            public const string worldboss_event_id = "8113";
            public const string worldboss_umengId = "play_world_boss";

            public const string puzzle_camp_topic = "异界迷宫";
            public const string puzzle_camp_event_id = "8114";
            public const string puzzle_camp_umengId = "play_puzzle_camp";


            public static void PostEvent(string topic, string event_id,string umengId, string type)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic); //消息主题 参与竞技场
                data.Add("event_id", event_id);//8101
                data.Add("type", type);//open reward
                AddUmengEvent(umengId, data); //play_arena
            }
            
           //极限试炼处理  
            public static void PostEvent(string topic, string event_id,string umengId, string type,int level)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic); //消息主题 参与竞技场
                data.Add("event_id", event_id);//8101
                data.Add("type", type);//open reward
                data.Add("level", level);
                AddUmengEvent(umengId, data); //play_arena
            }
            //军团护送
            public static void PostEsortEvent(string type, string side)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", esort_topic); //消息主题 参与竞技场
                data.Add("event_id", esort_event_id);//8101
                data.Add("type", type);//open reward
                data.Add("side", side);//运镖上传esort,劫镖上传rob
                AddUmengEvent(esort_umengId, data); //play_arena
            }
        }

        public class ShopData
        {
            public const string UmengId = "shop";

            public const string topic = "商店购买";
            public const string event_id = "8007";

            /// <summary>
            /// 商店购买日志
            /// </summary>
            /// <param name="ItemId">物品id</param>
            public static void PostEvent(int ItemId)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("topic", topic);
                data.Add("event_id", event_id);

                data.Add("ItemId", ItemId);
                AddUmengEvent(UmengId, data);
            }
        }

        public static BaseData UmnegBaseData;
        public static Dictionary<string, object> MergeDictionary(Dictionary<string, object> first, Dictionary<string, object> second)
        {
            if (first == null) first = new Dictionary<string, object>();
            if (second == null) return first;

            foreach (var item in second)
            {
                if (!first.ContainsKey(item.Key))
                    first.Add(item.Key, item.Value);
            }

            return first;
        }

        public static void RecordCombat(long CombatID)
        {
            if (CombatID != curCombatID)
            {
                curCombatID = CombatID;
                startTime = EB.Time.Now;
            }
        }
        public static void PostCombat(string gameType, int chapterId, int Result, List<int> hero_Id, int isFinish)
        {
            FusionTelemetry.MatchData.PostEvent(curCombatID, gameType, chapterId, Result, startTime, EB.Time.Now - startTime, hero_Id, isFinish);
        }

        private static long curCombatID;
        private static int startTime;

        public enum UseHC
        {
            hc_vigor = 0,//购买_体力（热更中）
            hc_gold,//购买_金币（热更中）
            hc_exp,//购买_伙伴经验（热更中）
            hc_camppoint,//购买_入场券（热更中）
            hc_draw,//购买_钻石抽卡（热更中）
            hc_store,//购买_商店道具
            hc_chargegift,//购买_培养礼包
            hc_resetstore,//刷新_商店
            hc_escort,//刷新_军团护送
            hc_bountytask,//刷新_悬赏
            hc_worldboss,//刷新_世界BOSS挑战
            hc_nationwar,//刷新_国战出征
            hc_resetchallenge,//刷新_挑战复活
            hc_handbookcell,//解锁_图鉴栏位
            hc_creatalliance,//创建军团
            hc_donate,//军团捐献
            hc_alliancename,//修改_军团名
            hc_playername,//修改_玩家名
            hc_blitzdouble,//双倍领取_挑战扫荡
            hc_snatchdouble,//双倍领取_夺宝奇兵
        }

        public static void Initialize()
        {
            Hotfix_LT.Messenger.AddListener<EB.IAP.Item, EB.IAP.Transaction>("OnOfferPurchaseSuceeded", OnPurchaseSucced);
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                if (ILRDefine.UNITY_IPHONE)
                {
                    //IPHONE的在UnityAppController.mm文件里实现初始化
                    // InitUMengSDK("5c04d50bf1f5569c3a0002eb", Application.identifier); 
                }else if (ILRDefine.UNITY_ANDROID)
                {
                    InitUMengSDK("5dcbaf05570df3ced2000a61", Application.identifier);
                    //测试一个新的确认是否接入成功
                    // InitUMengSDK("5f61bf09a4ae0a7f7d0675e3", Application.identifier);
                    
                }
            }
        }

        private static void OnPurchaseSucced(EB.IAP.Item offer, EB.IAP.Transaction trans)
        {
            PostPay(offer.cost, trans.platform, offer.value);
        }

        private static void OnLevelChange(EB.Sparx.LevelRewardsStatus status)
        {
            PostUserLevel(status.Level);
        }

        private static void InitUMengSDK(string appKey, string channelId)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR && ILRDefine.UNITY_ANDROID)
            {
                //ios暂时没有实现
                EB.Debug.Log("UMU3DCommonSDKInit appKey = {0} channelId = {1}", appKey, channelId);
                UMU3DCommonSDKInit(appKey, channelId);

                EB.Debug.Log("InitUMengSDK appKey = {0} channelId = {1}", appKey, channelId);
                Umeng.GA.StartWithAppKeyAndChannelId(appKey, channelId);
                Umeng.GA.SetLogEnabled(true);
                Umeng.GA.SetLogEncryptEnabled(false);
            }
        }

        /// <summary>
        /// 友盟SDK初始化
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="channelId"></param>
        private static void UMU3DCommonSDKInit(string appKey, string channelId)
        {
//#if UNITY_EDITOR
//            //Debug.LogWarning(EB.Localizer.GetString("ID_codefont_in_Analytics_709"));
//#elif UNITY_IPHONE

            if (ILRDefine.UNITY_ANDROID)
            {
                AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaClass util = new AndroidJavaClass("com.umeng.commonsdk.UMConfigure");
                if (unityPlayerClass != null)
                {
                    AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                    util.CallStatic("init", currentActivity, appKey, channelId, 1, "");
                }
            }
            // GlobalUtils.AndroidCall("com.umeng.commonsdk.UMConfigure", "init", appKey, channelId, 1, "");
        }

        /// <summary>
        /// 友盟登录统计
        /// </summary>
        /// <param name="account"></param>
        /// <param name="user"></param>
        public static void PostLogin(Account account, EB.Sparx.User user)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR && user != null && user.Id != null)
            {
                EB.Debug.Log("PostLogin userId = {0}", user.Id.Value);
                Umeng.GA.ProfileSignIn(user.Id.Value.ToString());
            }

            LoginEvenTimer = ILRTimerManager.instance.AddTimer(10000, 1, UmengLoginEven);//10秒后才进行登录统计

            if (SparxHub.Instance != null && SparxHub.Instance.LevelRewardsManager != null)
            {
                SparxHub.Instance.LevelRewardsManager.OnLevelChange += OnLevelChange;
            }
        }

        private static int LoginEvenTimer = 0;
        private static void UmengLoginEven(int timer)
        {
            LoginEvenTimer = 0;
            int Appid = 0;
            int channelid = 0;
            if (ILRDefine.USE_XINKUAISDK)
            {
                Appid = SparxHub.Instance.mBaseSdkManager.GetAppid();
                channelid = 1;
            }else if (ILRDefine.USE_WECHATSDK)
            {
              Appid = 1;
              channelid = 2;
            }
            UmnegBaseData = new BaseData(Appid, channelid, LoginManager.Instance.LocalUser.WorldId, LoginManager.Instance.Account.AccountId, LoginManager.Instance.LocalUser.Id.Value);
            int vipType = 0;
            bool isSvip = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.silver_monthly_vip.is_monthly_vip", out isSvip);
            bool isGvip = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.gold_monthly_vip.is_monthly_vip", out isGvip);
            if (isSvip && isGvip) vipType = 3;
            else if (isSvip) vipType = 2;
            else if (isGvip) vipType = 1;
            int lastloginTime;
            DataLookupsCache.Instance.SearchIntByID("user.time_last", out lastloginTime);
            int payAmount;
            DataLookupsCache.Instance.SearchIntByID("user.revenue", out payAmount);
            int battlepass;
            DataLookupsCache.Instance.SearchIntByID("userBattlePass.token", out battlepass);

            Hashtable chapterData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.normalChapters", out chapterData);
            int maxChapterId = 101;
            if (chapterData != null)
            {
                foreach (DictionaryEntry data in chapterData)
                {
                    int chapterIntId = int.Parse(data.Key.ToString());
                    if (chapterIntId > maxChapterId)
                    {
                        maxChapterId = chapterIntId;
                    }
                }
            }

            Hashtable campaignList;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("userCampaignStatus.normalChapters.{0}.campaigns", maxChapterId), out campaignList);
            int campaignLevel = 0;
            if (campaignList != null)
            {
                foreach (DictionaryEntry campaignData in campaignList)
                {
                    int LevelId = int.Parse(campaignData.Key.ToString());
                    if (LevelId > campaignLevel)
                    {
                        campaignLevel = LevelId;
                    }
                }
            }

            int challengeLevel;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out challengeLevel);
            //友盟统计
            FusionTelemetry.LoginData.PostEvent(vipType, lastloginTime, payAmount, battlepass, campaignLevel.ToString(), challengeLevel.ToString());
        }

        /// <summary>
        /// 友盟退统计
        /// </summary>
        public static void PostLogout()
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("PostLogout");
                Umeng.GA.ProfileSignOff();
            }
            if (LoginEvenTimer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(LoginEvenTimer);
                LoginEvenTimer = 0;
            }
            SparxHub.Instance.LevelRewardsManager.OnLevelChange -= OnLevelChange;
        }

        /// <summary>
        /// 充值统计
        /// </summary>
        /// <param name="cash">现金</param>
        /// <param name="platform">平台</param>
        /// <param name="value">价值</param>
        public static void PostPay(double cash, string platform, double value)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                Umeng.GA.PaySource platformCode = (Umeng.GA.PaySource)System.Enum.Parse(typeof(Umeng.GA.PaySource), platform);
                EB.Debug.Log("Pay cash = {0} value = {1} platform = {2} platformCode = {3}", cash, value, platform, platformCode);
                Umeng.GA.Pay(cash, platformCode, value);
            }
        }

        /// <summary>
        /// 钻石消费统计事件
        /// </summary>
        /// <param name="item">物品id</param>
        /// <param name="amount">购买数量</param>
        /// <param name="price">花费</param>
        public static void PostBuy(string item, int amount, double price)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("Buy item = {0} amount = {1} price = {2}", item, amount, price);
                Umeng.GA.Buy(item, amount, price);
            }
        }

        public static void PostUse(string item, int amount, double price)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("Use item = {0} amount = {1} price = {2}", item, amount, price);
                Umeng.GA.Use(item, amount, price);
            }
        }

        /// <summary>
        /// 开始挑战关卡
        /// </summary>
        /// <param name="level">定义的id</param>
        public static void PostStartCombat(string level)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("StartLevel StartLevel level = {0}", level);
                Umeng.GA.StartLevel(level);
            }
        }

        /// <summary>
        /// 完成关卡
        /// </summary>
        /// <param name="level">定义的id</param>
        public static void PostFinishCombat(string level)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("FinishLevel FinishLevel level = {0}", level);
                Umeng.GA.FinishLevel(level);
            }
        }

        /// <summary>
        /// 失败关卡
        /// </summary>
        /// <param name="level">定义的id</param>
        public static void PostFailCombat(string level)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("PostFailLevel FailLevel level = {0}", level);
                Umeng.GA.FailLevel(level);
            }
        }

        /// <summary>
        /// 发送玩家升级事件
        /// </summary>
        /// <param name="level">等级</param>
        public static void PostUserLevel(int level)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("PostUserLevel SetUserLevel level = {0}", level);
                Umeng.GA.SetUserLevel(level);
            }
        }

        /// <summary>
        /// 发送获得虚拟币奖励事件
        /// </summary>
        /// <param name="coin">奖励额</param>
        /// <param name="source">事件</param>
        public static void PostBonus(double coin, Umeng.GA.BonusSource source)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("PostBonus coin = {0},source = {1}", coin, source);
                Umeng.GA.Bonus(coin,source);
            }
        }

        /// <summary>
        /// 发送自定义事件（计数事件）
        /// </summary>
        /// <param name="UmengId">自定义ID</param>
        public static void PostEvent(string UmengId)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("PostEvent Id = {0}", UmengId);
                Umeng.GA.Event(UmengId);
            }
        }

        /// <summary>
        /// 发送多参数事件
        /// </summary>
        /// <param name="UmengId">自定义ID</param>
        /// <param name="data">多参数</param>
        public static void PostEvent(string UmengId, Dictionary<string, object> data)
        {
            if (ILRDefine.USE_UMENG && !ILRDefine.UNITY_EDITOR)
            {
                EB.Debug.Log("PostEvent Id = {0}", UmengId);
                Umeng.GA.Event(UmengId, data);
            }
        }

        /// <summary>
        /// 混合基础属性，再发送多参数事件
        /// </summary>
        /// <param name="UmengId"></param>
        /// <param name="data"></param>
        public static void AddUmengEvent(string UmengId, Dictionary<string, object> datas)
        {
            if (LoginEvenTimer != 0)
            {
                ILRTimerManager.instance.RemoveTimer(LoginEvenTimer);
                LoginEvenTimer = 0;
                UmengLoginEven(0);
            }
            if (UmnegBaseData != null)
            {
                var temps = MergeDictionary(datas, UmnegBaseData.ToDictionary());
                if (ILRDefine.DEBUG)
                {
                    string dicStr = "";
                    foreach (var temp in temps)
                    {
                        dicStr += string.Format("{0}={1};", temp.Key, (temp.Value != null) ? temp.Value.ToString() : string.Empty);
                    }
                    EB.Debug.Log("UmengId = {0}|DictionaryData : {1}", UmengId, dicStr);
                }
                PostEvent(UmengId, temps);
            }
            else
            {
                EB.Debug.LogError("FusionTelemetry.UmnegBaseData == null!");
            }
        }

        public static void ItemsUmengCurrency(List<LTShowItemData> mlist, string reason)// FusionTelemetry.ItemsUmengCurrency(
        {
            for (int i = 0; i < mlist.Count; i++)
            {
                if (mlist[i].id == "hc")
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, mlist[i].count, reason);
                if (mlist[i].id == "gold")
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, mlist[i].count, reason);
            }
        }
        public static void ItemsUmengCurrency(LTShowItemData item, string reason)// FusionTelemetry.ItemsUmengCurrency(
        {
            if (item.id == "hc")
                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, item.count, reason);
            if (item.id == "gold")
                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, item.count, reason);
        }
    }
}