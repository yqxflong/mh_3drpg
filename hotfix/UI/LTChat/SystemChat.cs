using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class SystemChat : DynamicMonoHotfix
    {
        private const int MaxHistoryCount = 30;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            chatContentParent = t.parent.GetComponent<UIWidget>();
            previewTable = t.GetComponent<UITable>();
            placeholder = t.parent.GetComponent<UIWidget>();
            scrollView = t.parent.parent.GetComponent<UIScrollView>();
            
            for (int i = previewTable.transform.childCount - 1; i >= 0; --i)
            {
                MainMenuChatItem item = previewTable.transform.GetChild(i).GetMonoILRComponent<MainMenuChatItem>();
                Recycle(item);
            }

            mPanel = scrollView.GetComponent<UIPanel>();
            mClipRegion = mPanel.baseClipRegion;
        }

        private void OnHandleAsyncMessage(EB.Sparx.ChatMessage[] msgs)
        {
            AddMessages(msgs);
        }

        private void OnConnected()
        {
            RecycleAll();
            //AddMessages(ChatController.instance.GetChatItemDataList());
            AddSysHisMessages();
        }

        private void AddSysHisMessages()
        {
            FriendManager.Instance.AcHistory.GetAllChatHistory(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, delegate (List<EB.Sparx.ChatMessage> msgs)
            {
                AddMessages(msgs.ToArray());
            });
        }

        public override void Start()
        {
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

        public override void OnEnable()
        {
            RecycleAll();
            //AddMessages(ChatController.instance.GetChatItemDataList());
            AddSysHisMessages();
            UIEventListener.Get(placeholder.gameObject).onClick += OnOpenChatUIClick;
            var cm = SparxHub.Instance.ChatManager;
            cm.OnConnected += OnConnected;
            cm.OnMessages += OnHandleAsyncMessage;
        }

        public override void OnDisable()
        {
            var cm = SparxHub.Instance.ChatManager;
            cm.OnConnected -= OnConnected;
            cm.OnMessages -= OnHandleAsyncMessage;
            UIEventListener.Get(placeholder.gameObject).onClick -= OnOpenChatUIClick;
        }

        public UIWidget chatContentParent;

        public UITable previewTable;
        public UIWidget placeholder;
        public UIScrollView scrollView;
        public TweenHeight expandTweener;
        public UIToggle expandToggle;

        private UIPanel mPanel;
        private EB.CircularBuffer<MainMenuChatItem> mActive = new EB.CircularBuffer<MainMenuChatItem>(MaxHistoryCount);
        private List<MainMenuChatItem> mPool = new List<MainMenuChatItem>();
        private Vector4 mClipRegion = Vector4.zero;

        public void AddMessages(EB.Sparx.ChatMessage[] msgArr)
        {
            EB.Sparx.ChatMessage[] msgs = FilterMsg(msgArr);

            //mPanel.widgetsAreStatic = false;

            for (int i = 0; i < msgs.Length; ++i)
            {
                CleanOverflow();
                if (ChatRule.STR2CHANNEL[msgs[i].channelType] == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM)
                    AddMessage(msgs[i]);
            }

            mPanel.Update();
            previewTable.Reposition();
            Reposition();
            mPanel.Refresh();

            previewTable.repositionNow = true;

            //mPanel.widgetsAreStatic = true;
        }

        private void AddMessage(EB.Sparx.ChatMessage msg)
        {
            MainMenuChatItem item = Use();

            ChatUIMessage uimsg = new ChatUIMessage(msg);
            var channel = ChatRule.STR2CHANNEL[msg.channelType];
            uimsg.Channel = channel;
            uimsg.ChannelSpriteName = ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            item.SetSysItemData(uimsg);
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
            //Vector2 panelSize = mPanel.GetViewSize();

            float width = bs.size.x;
            //float height = Mathf.Max(bs.size.y, panelSize.y);
            float height = bs.size.y;
            var pos = placeholder.transform.localPosition;
            placeholder.SetRect(0, 0, width, height);
            placeholder.transform.localPosition = pos;
            //pos.x = 0;
            //pos.y = panelSize.y / 2 + mPanel.baseClipRegion.y;


            scrollView.InvalidateBounds();
            if (scrollView.canMoveVertically)
            {
                float target = 0.0f;
                if (height > 1314f) target = 1.0f;
                if (Mathf.Abs(target - scrollView.verticalScrollBar.value) < 0.01f)
                {
                    scrollView.SetDragAmount(1f, target, false);
                }
                else
                {
                    scrollView.SetDragAmount(1f, target, false);
                    scrollView.SetDragAmount(1f, target, true);
                }

                scrollView.UpdatePosition();
            }
        }

        private MainMenuChatItem Use()
        {
            if (mPool.Count > 0)
            {
                MainMenuChatItem item = mPool[mPool.Count - 1];
                mPool.RemoveAt(mPool.Count - 1);
                item.mDMono.gameObject.SetActive(true);
                mActive.Enqueue(item);
                item.mDMono.transform.parent = previewTable.transform;
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.SetAsFirstSibling();
                return item;
            }
            else
            {
                MainMenuChatItem item = Object.Instantiate(mActive[mActive.Count - 1].mDMono.transform).GetMonoILRComponent<MainMenuChatItem>();
                mActive.Enqueue(item);
                item.mDMono.transform.parent = previewTable.transform;
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.SetAsFirstSibling();
                item.SetSysItemData(null);
                return item;
            }
        }

        private void Recycle(MainMenuChatItem item)
        {
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

        private void OnOpenChatUIClick(GameObject go)
        {
            OnOpenChatUIBtnClick();
        }

        public MenuCreator ChatMenuCreater;
        public void OnOpenChatUIBtnClick()
        {
            if (ChatMenuCreater != null)
                ChatMenuCreater.CreateMenu();
            else
                GlobalMenuManager.Instance.Open("ChatHudView", null);
        }

        public void OnChatSettingBtnClick()
        {

        }

        public void OnExtendBtnClick()
        {
            //mPanel.widgetsAreStatic = false;
            expandTweener.Play(expandToggle.value);
            //expandTweener.SetOnFinished(delegate () {
            //    //mPanel.widgetsAreStatic = true;
            //});
            UISprite sprite = expandToggle.GetComponent<UISprite>();
            sprite.fillGeometry = !expandToggle.value;
            sprite.geometry.Clear();
            sprite.MarkAsChanged();
        }
    }
}
