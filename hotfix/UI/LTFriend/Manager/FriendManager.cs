using EB.Sparx;
using System;
using System.Collections;
using System.Collections.Generic;
using EB;

namespace Hotfix_LT.UI
{
    public class FriendManager : ManagerUnit, IManagerUnitUpdatable
    {
        private Dictionary<string, FriendDataMeta> mDataMetas = new Dictionary<string, FriendDataMeta>();

        private static FriendManager sInstance = null;
        public SubSystemState State { get; set; }
        public static FriendManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<FriendManager>(); }
        }

        public FriendAPI Api
        {
            get; private set;
        }

        public FriendConfig Config
        {
            get; private set;
        }

        public FriendInfo Info
        {
            get; private set;
        }

        public MyFriendList MyFriends
        {
            get; private set;
        }

        public BlackList BlackLists
        {
            get; private set;
        }

        public RecentlyList Recentlys
        {
            get; private set;
        }

        public TeamList Teams
        {
            get; private set;
        }

        public RecommendList Recommends
        {
            get; private set;
        }

        public ApplyList Applys
        {
            get; private set;
        }

        public bool UpdateOffline
        {
            get { return false; }
        }

        public ChatHistoryController ChatHistory;
        public AllChatHistoryController AcHistory;

        public static string FriendConfigId = "friendsInfo.config";
        public static string FriendInfoDataId = "friendsInfo.info";
        public static string MyFriendListId = "friendsInfo.my";
        public static string BlacklistListId = "friendsInfo.blacklist";
        public static string RecentlyListId = "friendsInfo.recently";
        public static string TeamListId = "friendsInfo.team";
        public static string RecommendListId = "friendsInfo.recommend";
        public static string ApplyListId = "friendsInfo.apply";
        public static string SearchListId = "friendsInfo.search";

        #region 消息码
        ///<summary> 你的好友数量已达上限 </summary>
        public static int CodeFriendNumMax = 901068;
        ///<summary> 对方的好友数量已达上限 </summary>
        public static int CodeOtherFriendNumMax = 901069;
        ///<summary> 是否删除{0}好友，删除好友后将无法互赠体力！ </summary>
        public static int CodeDeleteFriend = 902233;
        ///<summary> 获得{0}赠送的{1}点体力值 </summary>
        public static int CodeGetVigor = 902234;
        ///<summary> 赠送{0}{1}点体力值成功 </summary>
        public static int CodeSendVigorSuccess = 902235;
        ///<summary> 你已达到每日赠送体力上限，无法继续赠送 </summary>
        public static int CodeSendVigorLimit = 902236;
        ///<summary> 对方已经是你的好友，请勿重复添加好友 </summary>
        public static int CodeHasFriend = 902238;
        ///<summary> {0}已经同意你的好友申请 </summary>
        public static int CodeAcceptFriendInvite = 902242;
        ///<summary> {0}已经拒绝你的好友申请 </summary>
        public static int CodeRejectFriendInvite = 902243;
        ///<summary> 你搜索的玩家不存在 </summary>
        public static int CodeHasNotPlayer = 902245;
        ///<summary> 不能查找自己 </summary>
        public static int CodeCouldNotFindSelf = 902246;
        ///<summary> 你已拒绝{0}的好友申请 </summary>
        public static int CodeRejectOtherInvite = 902247;
        ///<summary> 你已成功发出了好友申请 </summary>
        public static int CodeSendFriendInvite = 902248;
        ///<summary> 你已达到每日领取体力上限，无法继续领取 </summary>
        public static int CodeGetVitLimit = 902249;
        ///<summary> 体力上限不能超过9999，你确定领取？ </summary>
        public static int CodeVitMax = 902251;
        ///<summary> 您确认将对方加入黑名单？ </summary>
        public static int CodeTrueIntoBlack = 902252;
        ///<summary> 已将{0}加入黑名单 </summary>
        public static int CodeIntoBlack = 902253;
        ///<summary> 已经添加{0}为好友 </summary>
        public static int CodeAddFriend = 902256;
        #endregion

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new FriendAPI();
            Instance.Api.ErrorHandler += ErrorHandler;

            Config = GameDataSparxManager.Instance.Register<FriendConfig>(FriendConfigId);
            Info = GameDataSparxManager.Instance.Register<FriendInfo>(FriendInfoDataId);
            MyFriends = GameDataSparxManager.Instance.Register<MyFriendList>(MyFriendListId);
            BlackLists = GameDataSparxManager.Instance.Register<BlackList>(BlacklistListId);
            Recentlys = GameDataSparxManager.Instance.Register<RecentlyList>(RecentlyListId);
            Teams = GameDataSparxManager.Instance.Register<TeamList>(TeamListId);
            Recommends = GameDataSparxManager.Instance.Register<RecommendList>(RecommendListId);
            Applys = GameDataSparxManager.Instance.Register<ApplyList>(ApplyListId);

            ChatHistory = new ChatHistoryController();
            AcHistory = new AllChatHistoryController();

            mDataMetas[MyFriendListId] = new FriendDataMeta(MyFriendListId, delegate () { GetInfo(); });
            mDataMetas[ApplyListId] = new FriendDataMeta(ApplyListId, GetApplyList);
        }

        public int ResetTime;
        public override void OnLoggedIn()
        {
            Hashtable loginData = Hub.Instance.DataStore.LoginDataStore.LoginData;

            object friend = loginData["friendsInfo"];
            if (friend == null)
            {
                EB.Debug.LogWarning("FriendManager.OnLoggedIn: friend not found in LoginData");
                return;
            }
            loginData.Remove("friendsInfo");

            //MarkAllDirty();
            //GetInfo();
            GetInfo();
            string str;
            DataLookupsCache.Instance.SearchDataByID<string>("limit_state.sendVigor.resetTime", out str);
            ResetTime = ParseResetTime(str);
        }

        private int ParseResetTime(string str)
        {
            if (str == null) return 0;
            int timer = 0;
            string[] strs = str.Split(' ');
            if (strs.Length != 6)
                EB.Debug.LogError("cronTable Str Length!=6");

            string secondStr = strs[0], minuteStr = strs[1], hourStr = strs[2], dayStr = strs[3], monthStr = strs[4], weekStr = strs[5];
            DateTime DateStart = Data.ZoneTimeDiff.GetServerStartTime();// new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime now = Data.ZoneTimeDiff.GetServerTime();
            int i = 0;
            int j = 0;
            int k = 0;
            int.TryParse(hourStr, out i);
            int.TryParse(minuteStr, out j);
            int.TryParse(secondStr, out k);
            DateTime ResetTime = new DateTime(now.Year, now.Month, now.Day, i, j, k);
            timer = (int)(ResetTime - DateStart).TotalSeconds;
            /*while (EB.Time.Now > timer)
            {
                timer += 60 * 60 * 24;
            }*/
            return timer;
        }

        public bool IsResetVigorTime
        {
            get
            {
                int i = 0;
                DataLookupsCache.Instance.SearchIntByID("limit_state.sendVigor.ts", out i);
                while (i > ResetTime)
                {
                    ResetTime += 60 * 60 * 24;
                }
                bool temp = EB.Time.Now > ResetTime;//EB.Time .Now ;
                return temp;
            }
        }

        public bool IsResidueVigorReceiveNum()
        {
            return Info.HaveVigorReceiveNum < Config.MaxVigorReceiveNum;
        }

        public bool IsResidueVigorSendNum()
        {
            return Info.HaveVigorSendNum < Config.MaxVigorSendNum;
        }

        //    public override void OnEnteredBackground()
        // {
        //
        // }
        //
        // public override void OnEnteredForeground()
        // {
        //
        // }

        public override void Connect()
        {
            State = EB.Sparx.SubSystemState.Connected;

            var im = LTHotfixManager.GetManager<InvitesManager>();
            im.OnAcceptListener += OnAccept;
            im.OnRejectListener += OnReject;
            //im.OnRemoveListener += OnRemove;
            im.OnRequestListener += OnRequest;
            //im.OnInviteListener += OnInvite;
            //im.OnRemoveTargetListener += OnRemoveTarget;

            var cm = SparxHub.Instance.ChatManager;
            cm.OnMessages += OnHandleAsyncMessage;
        }

        public override void Disconnect(bool isLogout)
        {
            var im = LTHotfixManager.GetManager<InvitesManager>();
            im.OnAcceptListener -= OnAccept;  //同意
            im.OnRejectListener -= OnReject;  //拒绝
                                              //im.OnRemoveListener -= OnRemove;  //移除
            im.OnRequestListener -= OnRequest;//请求
                                              //im.OnInviteListener -= OnInvite;   
                                              //im.OnRemoveTargetListener -= OnRemoveTarget;

            State = EB.Sparx.SubSystemState.Disconnected;

            var cm = SparxHub.Instance.ChatManager;
            cm.OnMessages -= OnHandleAsyncMessage;
        }

        public override void Async(string message, object payload)
        {
            switch (message)
            {
                case "delete":
                    GetInfo();
                    break;
                case "blacklist":
                    long s_uid = EB.Dot.Long("friend.invite.s_uid", payload, 0);
                    if (s_uid <= 0)
                    {
                        EB.Debug.LogError("blacklist s_uid<=0");
                        return;
                    }
                    if (!Instance.Info.BeBlacklists.Contains(s_uid))
                        Instance.Info.BeBlacklists.Add((double)s_uid);
                    GetInfo();
                    break;
                case "removeBlacklist":
                    long s_uid2 = EB.Dot.Long("friend.invite.s_uid", payload, 0);
                    if (s_uid2 <= 0)
                    {
                        EB.Debug.LogError("blacklist s_uid<=0");
                        return;
                    }
                    Instance.Info.BeBlacklists.Remove((double)s_uid2);
                    break;
                case "sendMsg":
                    GetInfo();
                    break;
                case "sendVigor":
                    GetInfo();
                    Messenger.Raise(Hotfix_LT.EventName.FriendSendVigorEvent);
                    break;
            }
        }

        private void OnHandleAsyncMessage(EB.Sparx.ChatMessage[] msgs)
        {
            ChatRule.CHAT_CHANNEL channelType = ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE;
            string privateChannel = ChatRule.CHANNEL2STR[channelType];

            for (int i = 0, len = msgs.Length; i < len; ++i)
            {
                var msg = msgs[i];
                if (msg.channelType == privateChannel)
                {
                    Info.LastMessageTs = msg.ts;
                    Info.NewestSendId = LTChatManager.Instance.GetTargetId(msg.uid,msg.privateUid);
                    ChatHistory.SaveData(msg);

                    if (msg.privateUid == LoginManager.Instance.LocalUserId.Value)
                    {
                        Info.AddUnreadMessageId(Info.NewestSendId);
                    }

                    Messenger.Raise(Hotfix_LT.EventName.FriendMessageEvent, msg.uid);
                }
            }
        }

        public void Update()
        {
            //测试先屏蔽 看是否有问题
            DataUpdateCheck();
            //
            // ChatHistory.ThreadUpdate();
            // AcHistory.ThreadUpdate();
        }

        private void DataUpdateCheck()
        {
            var iter = mDataMetas.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;

                if (pair.Value.LifeTime > 0 && UnityEngine.Time.realtimeSinceStartup > pair.Value.LifeTime)
                {
                    MarkDirty(pair.Key);
                }

                if (IsDirty(pair.Key) && GameDataSparxManager.Instance.HasListener(pair.Key))
                {
                    ClearDirty(pair.Key);

                    if (pair.Value.Updater != null)
                    {
                        pair.Value.Updater();
                    }
                }
            }
            iter.Dispose();
        }

        public bool CheckBeblack(long uid)
        {
            return Instance.Info.BeBlacklists.Contains((double)uid);
        }

        public bool CheckBlacklist(long uid)
        {
            return Instance.BlackLists.Find(uid) != null;
        }

        //public void AddChatHistory(EB.Sparx.ChatMessage msg)
        //{
        //	ChatHistory.AddChatHistory(msg);
        //}

        public void GetChatHistory(long uid, Action<List<EB.Sparx.ChatMessage>> del)
        {
            ChatHistory.GetChatHistory(uid, del);
        }

        private void OnInvite(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.FRIEND_OPERATE)
            {
                return;
            }
        }

        private void OnRequest(InviteData invite)   //请求加好友
        {
            if (invite.Catalog == InvitesMessage.FRIEND_OPERATE)
            {
                var sender = MyFriends.Find(invite.SenderUid);
                if (sender != null)
                {
                    EB.Debug.LogError("already is my friend uid={0}" , invite.SenderUid);
                    return;
                    //Applys.Add(sender);
                }

                if (invite.SenderUid != AllianceUtil.GetLocalUid() && Applys.Find(invite.SenderUid) == null)
                {
                    Info.ApplyCount++;
                    Messenger.Raise(Hotfix_LT.EventName.FriendApplyEvent, true);
                    //MarkDirty(ApplyListId);
                    GetApplyList();

                }
            }
        }

        private void OnAccept(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.FRIEND_OPERATE)
            {
                long agreeUid = 0;
                long.TryParse(invite.OrgnizationId.ToString(), out agreeUid);
                string agreeName = EB.Dot.String(agreeUid + ".name", invite.Infos, "");
                if (agreeUid <= 0)
                {
                    EB.Debug.LogError("friend agree uid=null");
                }
                if (string.IsNullOrEmpty(agreeName))
                {
                    EB.Debug.LogError("friend agree name=null");
                }

                long localUid = AllianceUtil.GetLocalUid();
                if (agreeUid != localUid)
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeAcceptFriendInvite, agreeName, null);
                }

                //var friendData = MyFriends.Parse(invite.Data,invite.SenderUid);
                //MyFriends.Add(friendData);
                //GameDataSparxManager.Instance.SetDirty(MyFriendListId);
                GetInfo();
                return;
            }
        }

        private void OnReject(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.FRIEND_OPERATE)
            {
                long rejectUid = 0;
                long.TryParse(invite.OrgnizationId.ToString(), out rejectUid);
                string rejectName = EB.Dot.String(rejectUid + ".name", invite.Infos, "");
                if (rejectUid <= 0)
                {
                    EB.Debug.LogError("friend reject uid<=0");
                }
                if (string.IsNullOrEmpty(rejectName))
                {
                    EB.Debug.LogError("friend reject name=null");
                }

                long localUid = AllianceUtil.GetLocalUid();
                if (rejectUid != localUid)
                {
                    FriendRejectTarget target = new FriendRejectTarget();
                    target.Uid = rejectUid;
                    target.Ts = EB.Time.Now;
                    var existTarget = Instance.Info.RejectTargets.Find(m => m.Uid == target.Uid);
                    if (existTarget != null)
                    {
                        EB.Debug.Log("modify rejectTarget");
                        existTarget.Ts = EB.Time.Now;
                    }
                    else
                    {
                        Instance.Info.RejectTargets.Add(target);
                    }
                    MessageTemplateManager.ShowMessage(FriendManager.CodeRejectFriendInvite, rejectName, null);
                }
                return;
            }
        }

        private void OnRemove(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.FRIEND_OPERATE)
            {
                //var friend = MyFriends.Find(invite.SenderUid);
                //if ( friend == null)
                //{
                //	EB.Debug.LogError("on remove friend not in my list uid="+invite.SenderUid);
                //	return;
                //}
                //MyFriends.Remove(invite.SenderUid);
                //GameDataSparxManager.Instance.SetDirty(MyFriendListId);
                GetInfo();
                //MarkDirty(RecentlyListId);
                //MarkDirty(TeamListId);
            }
        }

        private void OnRemoveTarget(RemoveData remove)
        {

        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public static void OnConfigUpdated(string name, object payload)
        {
            Instance.Config.OnUpdate(payload);
        }

        private void FetchDataHandler(EB.Sparx.Response response)
        {
            if (response != null && response.hashtable != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(response.hashtable, false);
            }
        }

        private void MergeDataHandler(EB.Sparx.Response response)
        {
            if (response != null && response.hashtable != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(response.hashtable, true);
            }
        }

        public void MarkDirty(string flag)
        {
            mDataMetas[flag].Dirty = true;
            mDataMetas[flag].LifeTime = 0f;
        }

        public void ClearDirty(string flag)
        {
            mDataMetas[flag].Dirty = false;
            if (mDataMetas[flag].LifeDuration > 0f)
            {
                mDataMetas[flag].LifeTime = UnityEngine.Time.realtimeSinceStartup + mDataMetas[flag].LifeDuration;
            }
        }

        public bool IsDirty(string flag)
        {
            return mDataMetas[flag].Dirty;
        }

        public void MarkAllDirty()
        {
            foreach (var pair in mDataMetas)
            {
                pair.Value.Dirty = true;
                pair.Value.LifeTime = 0f;
            }
        }

        public void ClearAllDirty()
        {
            foreach (var pair in mDataMetas)
            {
                pair.Value.Dirty = false;
                pair.Value.LifeTime = 0f;
            }
        }

        public void GetInfo(long uid = 0)
        {
            bool isReqHistory = false;
            //if (!Info.IsHaveReqHistory)
            //{
            //	Info.IsHaveReqHistory = true;
            //	isReqHistory = true;
            //}
            long lastTime = Instance.Info.LastMessageTs + 2;

            Api.GetInfo(uid, lastTime, isReqHistory, delegate (EB.Sparx.Response response)
            {
                if (response != null && response.hashtable != null)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    ArrayList history = Hotfix_LT.EBCore.Dot.Array("friendsInfo.history", response.hashtable, null);
                    if (isReqHistory && history == null)
                    {
                        EB.Debug.LogError("isReqHistory && history array == null");
                    }
                    if (isReqHistory && history != null)
                    {
                        for (int i = 0; i < history.Count; ++i)
                        {
                            var msg = EB.Sparx.ChatMessage.Parse(history[i]);

                            if (msg.ts <= lastTime)
                            {
                                EB.Debug.LogError("newest offline msg ts<=lasttime lasttime={0},msg.ts={1}" , lastTime , msg.ts);
                                continue;
                            }

                            Instance.Info.LastMessageTs = msg.ts;
                            Info.AddUnreadMessageId(LTChatManager.Instance.GetTargetId(msg.uid, msg.privateUid));
                            ChatHistory.SaveData(msg);
                        }
                    }
                    response.hashtable.Remove("history");
                    FetchDataHandler(response);
                }
            });
        }

        public void Add(long uid, string verifyInfo, System.Action<bool> callback)
        {
            if (Instance.CheckBeblack(uid))
            {
                return;
            }

            if (MyFriends.Find(uid) != null)
            {
                MessageTemplateManager.ShowMessage(CodeHasFriend);
                return;
            }

            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("send Invite too much"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_FriendManager_17152"));
                    return true;
                }
                return false;
            };

            Api.Add(uid, verifyInfo, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void LaunchChat(long uid, System.Action<bool> callback)
        {
            Api.LaunchChat(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                    callback(response.sucessful);
                else
                    callback(false);
            });
        }

        public void Delete(long uid, eFriendType type, System.Action<bool> callback)
        {
            Api.Delete(uid, type, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    MyFriends.Remove(uid);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void Remove(long uid, eFriendType type, System.Action<bool> callback)
        {
            Api.Remove(uid, type, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void Blacklist(long uid, System.Action<bool> callback)
        {
            Api.Blacklist(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void AddFromBlacklist(long uid, string verifyInfo, System.Action<bool> callback)
        {
            Api.AddFromBlacklist(uid, verifyInfo, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void RemoveBlacklist(long uid, Action<bool> callback)
        {
            Api.RemoveBlacklist(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void Agree(long uid, System.Action<bool> callback)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("ID_TARGET_FRIEND_HAVE_FULL"))
                {
                    Info.MyFriendNum--;
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FriendManager_19520"));
                    return true;
                }
                return false;
            };

            Api.Agree(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void Reject(long uid, System.Action<bool> callback)
        {
            Api.Reject(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    callback(response.sucessful);
                }
                else
                {
                    callback(false);
                }
            });
        }

        public void GetApplyList()
        {
            Api.GetApplyList(FetchDataHandler);
        }

        public void GetRecommendList()
        {
            Api.GetRecommendList(FetchDataHandler);
        }

        public void Search(string text, System.Action<Hashtable> callback)
        {
            Api.Search(text, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                    callback(response.hashtable);
                else
                    callback(null);
            });
        }

        public void SendVigor(long uid, System.Action<bool> callback)
        {
            Api.SendVigor(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    callback(response.sucessful);
                }
                else
                {
                    callback(false);
                }
            });
        }

        public void ReceiveVigor(long uid, System.Action<bool> callback)
        {
            Api.ReceiveVigor(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    FetchDataHandler(response);
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
        }

        public void Like(long uid, System.Action<bool> callback)
        {
            Api.Like(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                    callback(response.sucessful);
                else
                    callback(false);
            });
        }

        public void UnLike(long uid, System.Action<bool> callback)
        {
            Api.UnLike(uid, delegate (EB.Sparx.Response response)
            {
                if (response != null)
                    callback(response.sucessful);
                else
                    callback(false);
            });
        }

        public void ResetVigorTime(System.Action<bool> callback = null)
        {
            Api.ResetVigorTime(delegate (EB.Sparx.Response response)
            {
                if (response != null)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    callback(response.sucessful);
                }
                else
                    callback(false);
            });
            // else callback(true);
        }

        public void SendAllVigor(System.Action<Hashtable> callback)
        {
            Api.SendAllVigor(delegate (EB.Sparx.Response response)
            {
                if (response != null && response.hashtable.Count > 0)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    callback(response.hashtable);
                }
            });
        }

        public void ReceiveAllVigor(System.Action<Hashtable> callback)
        {
            Api.ReceiveAllVigor(delegate (EB.Sparx.Response response)
            {
                if (response != null && response.hashtable.Count > 0)
                {
                    DataLookupsCache.Instance.CacheData(response.hashtable);
                    callback(response.hashtable);
                }
            });
        }


        public bool CanAllSend()
        {
            for (int i = 0; i < MyFriends.List.Count; i++)
            {
                if (MyFriends.List[i].IsHaveSendVigor == false)
                {
                    return true;
                }
            }

            return false;
        }


        public bool CanAllReceive()
        {
            for (int i = 0; i < MyFriends.List.Count; i++)
            {
                if (MyFriends.List[i].IsCanReceiveVigor == true)
                {
                    return true;
                }
            }

            return false;
        }
        
        public eFriendType ParseFriendType(string name,object value,eFriendType defaultType)
        {
            Hashtable obj=null;
            if (value is Hashtable)
            {
                var ht = (Hashtable)value;
                obj = (Hashtable)ht[name];
                if (obj!=null)
                {
                    foreach (int VARIABLE in Enum.GetValues(typeof(eFriendType)))
                    {
                        string key = Enum.GetName(typeof(eFriendType), VARIABLE)?.ToLower();
                        if (obj.Contains(key))
                        {
                            return (eFriendType) VARIABLE;
                        }
                    }
                }
            }
            Debug.LogWarning("-----obj:{0} not Contain an eFriendType key--------",obj);
            return defaultType;
        }
    }
}