using System.Collections;
using System.Collections.Generic;
using EB.Sparx;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTChatManager : ManagerUnit
    {
        private static LTChatManager instance = null;

        public static LTChatManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTChatManager>();
                }
                return instance;
            }
        }

        public LTChatAPI Api
        {
            get; private set;
        }

        public override void Connect()
        {
            //State = SubSystemState.Connected;
            var cm = SparxHub.Instance.ChatManager;
            cm.OnConnected += OnConnected;
            cm.OnMessages += OnHandleAsyncMessage;
        }

        public override void Disconnect(bool isLogout)
        {
            //State = SubSystemState.Disconnected;
            var cm = SparxHub.Instance.ChatManager;
            cm.OnConnected -= OnConnected;
            cm.OnMessages -= OnHandleAsyncMessage;
        }

        public override void Initialize(Config config)
        {
            Instance.Api = new LTChatAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
        }
        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        #region ChatEven
        private int askMicrophoneTimer = 0;
        /// <summary> 询问麦克风权限</summary>
        public void AskMicrophone()
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHAT_MICROPHONE_ERROR"));
            if (askMicrophoneTimer == 0)
            {
                askMicrophoneTimer = ILRTimerManager.instance.AddTimer(1000, 1, delegate
                {
                    askMicrophoneTimer = 0;
                    GlobalUtils.AndroidCall("org.manhuang.android.Util", "RequestRecordAudio");
                });
            }
        }

        private EB.CircularBuffer<MainMenuChatItem> mActive = new EB.CircularBuffer<MainMenuChatItem>(MaxHistoryCount);
        private List<MainMenuChatItem> mPool = new List<MainMenuChatItem>();
        private Vector4 mClipRegion = Vector4.zero;
        private const int MaxHistoryCount = 30;

        //红点判断
        public static bool hasNewSysMessage = false;
        public static bool hasNewWorldMessage = false;
        public static bool hasNewAllianceMessage = false;
        public static bool hasNewTeamMessage = false;
        public static bool hasNewNationMessage = false;

        public static bool GetMainHudRP()
        {
            return hasNewWorldMessage || hasNewAllianceMessage;
        }

        private void AddMessages(EB.Sparx.ChatMessage[] msgArr)
        {
            if (msgArr == null)
            {
                return;
            }

            EB.Sparx.ChatMessage[] msgs = FilterMsg(msgArr);

            //mPanel.widgetsAreStatic = false;

            for (int i = 0; i < msgs.Length; ++i)
            {
                CleanOverflow();
                AddMessage(msgs[i]);
                //if (ChatRule.STR2CHANNEL[msgs[i].channelType] != ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM)
            }

            /*mPanel.Update();
            mPanel.Refresh();

            mPanel.widgetsAreStatic = true;*/
        }

        private void AddMessage(EB.Sparx.ChatMessage msg)
        {
            if (msg == null)
            {
                return;
            }

            ChatUIMessage uimsg = new ChatUIMessage(msg);
            var channel = ChatRule.STR2CHANNEL[msg.channelType];
            uimsg.ChannelSpriteName = ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            uimsg.Channel = channel;
        }

        private EB.Sparx.ChatMessage[] FilterMsg(EB.Sparx.ChatMessage[] msgs)
        {
            List<EB.Sparx.ChatMessage> msgList = new List<EB.Sparx.ChatMessage>();
            for (int i = 0; i < msgs.Length; i++)
            {
                if (msgs[i].channelType == ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE] || FriendManager.Instance.CheckBlacklist(LTChatManager.Instance.GetTargetId(msgs[i].uid, msgs[i].privateUid)))
                {
                    continue;
                }
                msgList.Add(msgs[i]);
            }
            return msgList.ToArray();
        }

        private void CleanOverflow()
        {
            int clean = mActive.Count + 1 - MaxHistoryCount;
            for (int i = 0; i < clean; ++i)
            {
                MainMenuChatItem item = mActive.Dequeue();
                Recycle(item);
            }
        }

        private void Recycle(MainMenuChatItem item)
        {
            item.SetItemData(null);
            item.mDMono.transform.SetAsLastSibling();
            item.mDMono.gameObject.SetActive(false);
            mPool.Add(item);
        }

        private void RecycleAll()
        {
            while (mActive.Count > 0)
            {
                MainMenuChatItem item = mActive.Dequeue();
                Recycle(item);
            }
        }

        public long LastAllMessageTs
        {
            get
            {
                string s = PlayerPrefs.GetString(LoginManager.Instance.LocalUserId.Value + ".LastAllMessageTs", "");

                long ts;
                long.TryParse(s, out ts);
                return ts;
            }
            set
            {
                PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value + ".LastAllMessageTs", value.ToString());
            }
        }

        private bool isFirstInGame = true;//第一次拉取数据不算新数据
        private void OnHandleAsyncMessage(EB.Sparx.ChatMessage[] msgs)
        {
            if (msgs != null)
            {
                for (int i = 0, len = msgs.Length; i < len; ++i)
                {
                    if (msgs[i] == null)
                    {
                        continue;
                    }

                    LastAllMessageTs = msgs[i].ts;
                    FriendManager.Instance.AcHistory.SaveData(msgs[i], ChatRule.STR2CHANNEL[msgs[i].channelType]);

                    if (msgs[i].uid != LoginManager.Instance.LocalUser.Id.Value)
                    {
                        SetHasNewMessage(ChatRule.STR2CHANNEL[msgs[i].channelType]);
                    }
                }

                AddMessages(msgs);
            }

            if (isFirstInGame)
            {
                isFirstInGame = false;
                SetHasNewMessage(false);
            }

            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Chat);
        }

        private void OnConnected()
        {
            RecycleAll();
            AddMessages(ChatController.instance.GetChatItemDataList());
            //AddAllHisMessages();
        }

        public void SetHasNewMessage(ChatRule.CHAT_CHANNEL cHAT_CHANNEL, bool isHas = true)
        {
            switch (cHAT_CHANNEL)
            {
                case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM: hasNewSysMessage = isHas; break;
                case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD: hasNewWorldMessage = isHas; break;
                case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE: hasNewAllianceMessage = isHas; break;
                case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM: hasNewTeamMessage = isHas; break;
                case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION: hasNewNationMessage = isHas; break;
            }
        }

        public void SetHasNewMessage(bool isHas = false)
        {
            hasNewSysMessage = false;
            hasNewWorldMessage = false;
            hasNewAllianceMessage = false;
            hasNewTeamMessage = false;
            hasNewNationMessage = false;
        }

        /*private void AddAllHisMessages()
        {
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE, delegate (List<EB.Sparx.ChatMessage> msgs) {
                AddMessages(msgs.ToArray());
            });
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, delegate (List<EB.Sparx.ChatMessage> msgs) {
                AddMessages(msgs.ToArray());
            });
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM, delegate (List<EB.Sparx.ChatMessage> msgs) {
                AddMessages(msgs.ToArray());
            });
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD, delegate (List<EB.Sparx.ChatMessage> msgs) {
                AddMessages(msgs.ToArray());
            });
        }*/
        #endregion

        public long GetTargetId(long uid,long privateUid)
        {
            long targetId = uid;
            if (targetId == LoginManager.Instance.LocalUserId.Value)
            {
                targetId = privateUid;
            }
            return targetId;
        }
    }
}
