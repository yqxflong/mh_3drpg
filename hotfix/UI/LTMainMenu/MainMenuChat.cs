using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class MainMenuChat : DynamicMonoHotfix
    {
        public GameObject RedPoint;
        //private RedPoint_Chat redPoint;
        private const int MaxHistoryCount = 30;

        private List<EB.Sparx.ChatMessage> mMainChatDataList;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
           CurrentPanel = t.GetComponent<UIPanel>("Scroll View");
			RedPoint = t.FindEx("RedPoint").gameObject;
            chatContentParent = t.GetComponent<UIWidget>("Scroll View/Placeholder");
            previewTable = t.GetComponent<UITable>("Scroll View/Placeholder/Container");
            placeholder = t.GetComponent<UIWidget>("Scroll View/Placeholder");
            scrollView = t.GetComponent<UIScrollView>("Scroll View");
            expandTweener = t.GetComponent<TweenHeight>("PreviewBg");
            expandToggle = t.GetComponent<UIToggle>("ExpandBtn");
            hasNewSysMessage = false;

            t.GetComponent<UIToggle>("ExpandBtn").onChange.Add(new EventDelegate(OnExtendBtnClick));
            t.GetComponent<UIEventTrigger>("PreviewBg").onClick.Add(new EventDelegate(OnOpenChatUIBtnClick));

            mPreviewTableTransform = previewTable.transform;

            for (int i = mPreviewTableTransform.childCount - 1; i >= 0; --i)
            {
                MainMenuChatItem item = mPreviewTableTransform.GetChild(i).GetMonoILRComponent<MainMenuChatItem>();
                Recycle(item);
            }

            mPanel = scrollView.GetComponent<UIPanel>();
            mClipRegion = mPanel.baseClipRegion;
            isFirst = true;
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

        private void OnHandleAsyncMessage(EB.Sparx.ChatMessage[] msgs)
        {
            for (int i = 0, len = msgs.Length; i < len; ++i)
            {
                LastAllMessageTs = msgs[i].ts;
                FriendManager.Instance.AcHistory.SaveData(msgs[i], ChatRule.STR2CHANNEL[msgs[i].channelType]);
                if (ChatRule.STR2CHANNEL[msgs[i].channelType] == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM)
                    hasNewSysMessage = true;
            }
            AddMessages(msgs);
        }

        private void OnConnected()
        {
            RecycleAll();
            AddMessages(ChatController.instance.GetChatItemDataList());
            //AddAllHisMessages();
        }

        private int showTimes = 0;
        private int maxShowTimes = 3;
        private void AddAllHisMessages()
        {
            showTimes = 0;
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE, delegate (List<EB.Sparx.ChatMessage> msgs)
            {
                //AddMessages(msgs.ToArray());
                ShowMainChatOnFirst();
            });
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, delegate (List<EB.Sparx.ChatMessage> msgs)
            {
                //AddMessages(msgs.ToArray());
                ShowMainChatOnFirst();
            });
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION, delegate (List<EB.Sparx.ChatMessage> msgs)
            {
                //AddMessages(msgs.ToArray());
                ShowMainChatOnFirst();
            });
            //FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD, delegate (List<EB.Sparx.ChatMessage> msgs) {
            //    AddMessages(msgs.ToArray());
            //});
        }

        public override void Start()
        {
            if (isFirst)
            {
                RecycleAll();
                AddAllHisMessages();
                isFirst = false;
            }
            else
            {
                ShowMainChat();
            }
            UIEventListener.Get(placeholder.gameObject).onClick += OnOpenChatUIClick;
            SparxHub.Instance.ChatManager.OnMessages += OnMessages;
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Chat, SetRP);

            //redPoint = new RedPoint_Chat(RedPoint);
            SetRP();
            mPanel.onClipMove += delegate (UIPanel panel)
            {
                Vector2 panelSize = panel.GetViewSize();
                if (panel.baseClipRegion != mClipRegion)
                {
                    var pos = placeholder.transform.localPosition;
                    pos.x = 0;
                    pos.y = panelSize.y / 2 + mPanel.baseClipRegion.y;
                    placeholder.transform.localPosition = pos;

                    mClipRegion = panel.baseClipRegion;
                }
            };
        }
        private static bool isFirst;
        private static bool isFirstShow;

        public override void OnEnable()
        {

            //AddMessages(ChatController.instance.GetChatItemDataList());


            /*var cm = SparxHub.Instance.ChatManager;
    		cm.OnConnected += OnConnected;
    		cm.OnMessages += OnHandleAsyncMessage;*/
        }

        public override void OnDisable()
        {
            /* var cm = SparxHub.Instance.ChatManager;
             cm.OnConnected -= OnConnected;
             cm.OnMessages -= OnHandleAsyncMessage;*/
        }

        public override void OnDestroy()
        {

            SparxHub.Instance.ChatManager.OnMessages -= OnMessages;
            UIEventListener.Get(placeholder.gameObject).onClick -= OnOpenChatUIClick;
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Chat, SetRP);
        }

        private void ShowMainChat()
        {
            RecycleAll();
            List<EB.Sparx.ChatMessage> chatList = FriendManager.Instance.AcHistory.GetAllChatList();
            if (chatList.Count > MaxHistoryCount)
            {
                chatList.RemoveRange(0, chatList.Count - MaxHistoryCount);
            }
            EB.Sparx.ChatMessage[] msgArr = chatList.ToArray();
            AddMessages(msgArr);
        }

        private void ShowMainChatOnFirst()
        {
            showTimes++;
            if (showTimes >= maxShowTimes)
            {
                ShowMainChat();
            }
        }

        void SetRP()
        {
            RedPoint.CustomSetActive(LTChatManager.hasNewAllianceMessage);
        }

        public UIWidget chatContentParent;

        public UITable previewTable;
        public UIWidget placeholder;
        public UIScrollView scrollView;
        public TweenHeight expandTweener;
        public UIToggle expandToggle;

        private Transform mPreviewTableTransform;
        private UIPanel mPanel;
        private EB.CircularBuffer<MainMenuChatItem> mActive = new EB.CircularBuffer<MainMenuChatItem>(MaxHistoryCount);
        private List<MainMenuChatItem> mPool = new List<MainMenuChatItem>();
        private Vector4 mClipRegion = Vector4.zero;
        public static bool hasNewSysMessage = false;

        private void AddMessages(EB.Sparx.ChatMessage[] msgArr)
        {
            EB.Sparx.ChatMessage[] msgs = FilterMsg(msgArr);

            mPanel.widgetsAreStatic = false;

            for (int i = 0; i < msgs.Length; ++i)
            {
                CleanOverflow();
                AddMessage(msgs[i]);
                //if (ChatRule.STR2CHANNEL[msgs[i].channelType] != ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM)
            }

            mPanel.Update();
            previewTable.Reposition();
            Reposition();
            mPanel.Refresh();

            previewTable.repositionNow = true;

            mPanel.widgetsAreStatic = true;

            previewTable.gameObject.SetActive(false);
            previewTable.gameObject.SetActive(true);
        }

        private void AddMessage(EB.Sparx.ChatMessage msg)
        {
            MainMenuChatItem item = Use();

            ChatUIMessage uimsg = new ChatUIMessage(msg);
            var channel = ChatRule.STR2CHANNEL[msg.channelType];
            uimsg.ChannelSpriteName = ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            uimsg.Channel = channel;
            item.SetItemData(uimsg);
        }

        private EB.Sparx.ChatMessage[] FilterMsg(EB.Sparx.ChatMessage[] msgs)
        {
            List<EB.Sparx.ChatMessage> msgList = new List<EB.Sparx.ChatMessage>();
            for (var i = 0; i < msgs.Length; i++)
            {
                var msg = msgs[i];
                if (msg.channelType == ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE] || FriendManager.Instance.CheckBlacklist(LTChatManager.Instance.GetTargetId(msg.uid, msg.privateUid)))
                {
                    continue;
                }
                msgList.Add(msg);
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

        private void Reposition()
        {
            Bounds bs = NGUIMath.CalculateRelativeWidgetBounds(previewTable.transform);
            Vector2 panelSize = mPanel.GetViewSize();

            float width = bs.size.x;
            float height = Mathf.Max(bs.size.y, panelSize.y);

            placeholder.SetRect(0, 0, width, height);
            var pos = placeholder.transform.localPosition;
            pos.x = 0;
            pos.y = panelSize.y / 2 + mPanel.baseClipRegion.y;
            placeholder.transform.localPosition = pos;

            scrollView.SetDragAmount(1f, 0f, false);
            //scrollView.SetDragAmount(1f, 0f, true);
            scrollView.ResetPosition();
        }

        private MainMenuChatItem Use()
        {
            if (mPool.Count > 0)
            {
                MainMenuChatItem item = mPool[mPool.Count - 1];
                mPool.RemoveAt(mPool.Count - 1);
                item.mDMono.gameObject.SetActive(true);
                mActive.Enqueue(item);
                item.mDMono.transform.SetParent(mPreviewTableTransform);
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.SetAsFirstSibling();
                return item;
            }
            else
            {
                var ilr = Object.Instantiate(mActive[mActive.Count - 1].mDMono);
                MainMenuChatItem item = ilr._ilrObject as MainMenuChatItem;
                mActive.Enqueue(item);
                item.mDMono.transform.SetParent(mPreviewTableTransform);
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.SetAsFirstSibling();
                item.SetItemData(null);
                return item;
            }
        }

        private void Recycle(MainMenuChatItem item)
        {
            item.mDMono.transform.SetAsLastSibling();
            item.mDMono.gameObject.CustomSetActive(false);
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

        private void OnOpenChatUIClick(GameObject go)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!expandToggle.value)
                OnOpenChatUIBtnClick();
        }

        public MenuCreator ChatMenuCreater;
        public void OnOpenChatUIBtnClick()
        {
            if (ChatMenuCreater != null)
                ChatMenuCreater.CreateMenu();
            else
            {
                if (!ChatHudController.sOpen)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    GlobalMenuManager.Instance.Open("ChatHudView", null);
                }
            }
        }

        public void OnChatSettingBtnClick()
        {

        }

        public UIButton ChatBg;
        public void OnExtendBtnClick()
        {
            //mPanel.widgetsAreStatic = false;
            //expandTweener.Play(expandToggle.value);
            //      if (expandToggle.value) {
            //          ChatBg.enabled = false;
            //      }
            //      else
            //          ChatBg.enabled = true;
            //expandTweener.SetOnFinished(delegate ()
            //{
            //	mPanel.widgetsAreStatic = true;
            //          ResetItem();
            //});
            //UISprite sprite = expandToggle.GetComponent<UISprite>();
            //sprite.fillGeometry = !expandToggle.value;
            //sprite.geometry.Clear();
            //sprite.MarkAsChanged();
            //      scrollView.SetDragAmount(1f, 0f, false);
            //      //scrollView.SetDragAmount(1f, 0f, true);
            //   scrollView.ResetPosition();
        }

        private void ResetItem()
        {
            List<Transform> itemList = previewTable.GetChildList();
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].gameObject.SetActive(false);
                itemList[i].gameObject.SetActive(true);
            }
        }
        private void OnMessages(EB.Sparx.ChatMessage[] msgs)
        {
            AddMessages(msgs);
            if (isFirstShow)
            {
                isFirstShow = true;
                if (showTimes >= maxShowTimes)
                {
                    ShowMainChat();
                }
            }
            
        }


    }
}
