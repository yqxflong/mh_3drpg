using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class FriendChatController : DynamicMonoHotfix, IHotfixUpdate
    {
        public UIInput inputLabel;
        public GameObject talkUI;
        public UIButton talkButton, keyboardBtn, sendButton;
        public GameObject emoticonUI;
        public UIButton emoticonButton;
        public GameObject emoticonTable;
        public GameObject moveFingerObj, releaseFingerObj, pressTalkObj;
        public ChatItemDynamicScroll TabDynaScroll;
        public int sendInterval = 2;

        public void HideEmoticonUI()
        {
            //FusionAudio.PostEvent("UI/General/ButtonClick");
            emoticonUI.CustomSetActive(false);
        }

        public void ShowEmoticonUI()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            emoticonUI.CustomSetActive(true);
        }

        private void InitEmoticon()
        {
            HideEmoticonUI();
            for (int i = 0; i < emoticonTable.transform.childCount; ++i)
            {
                UIEventListener.Get(emoticonTable.transform.GetChild(i).gameObject).onClick += SelectEmoticon;
            }
        }

        private void CleanEmoticon()
        {
            HideEmoticonUI();
            //FusionAudio.PostEvent("UI/General/ButtonClick");
            for (int i = 0; i < emoticonTable.transform.childCount; ++i)
            {
                UIEventListener.Get(emoticonTable.transform.GetChild(i).gameObject).onClick -= SelectEmoticon;
            }
        }

        private void SelectEmoticon(GameObject go)
        {
            //inputLabel.value += go.name;

            string tempInputValue = inputLabel.value;
            inputLabel.value = go.name;
            OnSendBtnClick();
            inputLabel.value = tempInputValue;
            //FusionAudio.PostEvent("UI/General/ButtonClick");
            HideEmoticonUI();
        }

        private Vector3 originPos;
        private FriendHudController _friendCtr;

        public FriendHudController friendCtrl
        {
            get
            {
                if (_friendCtr==null)
                {
                    _friendCtr = control.parent.parent.GetUIControllerILRComponent<FriendHudController>();
                }
                return _friendCtr;
            }
        }

		public void OnOpenKeyboard()
        {
            originPos = friendCtrl.controller.transform.position;
            if (ILRDefine.UNITY_ANDROID)
            {
                mDMono.transform.localPosition = new Vector3(mDMono.transform.localPosition.x,
                    mDMono.transform.localPosition.y + UIRoot.list[0].manualHeight * 1 / 2f,
                    mDMono.transform.localPosition.z);
            }
            else
            {
                Bounds bds = NGUIMath.CalculateAbsoluteWidgetBounds(mDMono.transform);
                float yValue = TouchScreenKeyboard.area.yMax + bds.size.y / 2 + 0.1f;
                friendCtrl.controller.transform.position = new Vector3(originPos.x, yValue, originPos.z);
            }
            //LeanTween.moveY(gameObject, targetY, costTime).tweenType = tweenType;

            StartCoroutine(LookupKeyboardState());
        }

        public void OnCloseKeyboard()
        {
            friendCtrl.controller.transform.position = originPos;
        }

        private IEnumerator LookupKeyboardState()
        {
            yield return new WaitForSeconds(0.2f);

            while (true)
            {
                yield return null;
                if (ILRDefine.UNITY_ANDROID)
                {
                    if (!AndroidKeyboard.IsVisible)
                    {
                        OnCloseKeyboard();
                        yield break;
                    }
                }
                else
                {
                    if (!TouchScreenKeyboard.visible)
                    {
                        OnCloseKeyboard();
                        yield break;
                    }
                }
            }
        }

        public void OnSendBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            bool privateChannel = true;
            string inputStr = inputLabel.value.Trim();
            string privateTarget = string.Empty;
            if (!ChatController.instance.CanSend(inputStr))
            {
                EB.Debug.LogWarning("OnSendBtnClick: can't send");
                return;
            }

            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime("private") +
                GetSendInterval(GetCurChannelType()) - Time.realtimeSinceStartup;
            if (leftTime > 0)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("OnSendBtnClick: time limited");
                return;
            }

            inputStr = EB.ProfanityFilter.Filter(inputLabel.label.text);
            // 如果在别人的黑名单里面或者别人在自己的黑名单里面，该消息只做本地显示，不向服务器上发
            if (FriendManager.Instance.CheckBeblack(ChatController.instance.TargetPrivateUid) ||
                FriendManager.Instance.CheckBlacklist(ChatController.instance.TargetPrivateUid))
            {
                EB.Sparx.ChatMessage msg =
                    ChatController.instance.NewChatMessage(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE, inputStr);
                msg.privateUid = ChatController.instance.TargetPrivateUid;
                msg.privateName = ChatController.instance.TargetPrivateName;
                SparxHub.Instance.ChatManager.OnMessages(new EB.Sparx.ChatMessage[] {msg});
                inputLabel.value = string.Empty;
            }
            else if (privateChannel)
            {
                ChatController.instance.RequestSendChat(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE, inputStr,
                    ChatController.instance.TargetPrivateUid, ChatController.instance.TargetPrivateName);
                inputLabel.value = privateTarget;

                LaunchChat();
            }
            else
            {
                EB.Debug.LogError("happen error! channel not is private");
            }

            mInvalid = true;
        }

        #region voice

        private AudioClip audioRecord = null;
        private string microphoneDevice = string.Empty;
        private AudioSource audioPlay = null;
        private string pausedMusic = string.Empty;
        private bool isDragFinger;
        public GameObject InputBtn;

        public void OnAudioBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (Microphone.devices.Length <= 0)
            {
                LTChatManager.Instance.AskMicrophone();
                return;
            }
            OpenAudioInput(true);
            InputBtn.CustomSetActive(false);
        }

        public void OnKeyboardBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (Microphone.devices.Length <= 0)
            {
                LTChatManager.Instance.AskMicrophone();
                return;
            }
            OpenAudioInput(false);
            InputBtn.CustomSetActive(true);
        }

        private void OpenAudioInput(bool yes)
        {
            talkButton.gameObject.CustomSetActive(!yes);
            keyboardBtn.gameObject.CustomSetActive(yes);
            pressTalkObj.CustomSetActive(yes);
        }

        public void OnPress()
        {
            isDragFinger = false;
            OpenChatTalkUI();
        }

        public void OnRelease()
        {
            if (isDragFinger)
            {
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;

                talkUI.CustomSetActive(false);
            }
            else
            {
                CloseChatTalkUI();
            }
        }

        public void OnDragStart()
        {
            isDragFinger = true;
            moveFingerObj.CustomSetActive(false);
            releaseFingerObj.CustomSetActive(true);
        }

        public void OnDragOut()
        {
           EB.Debug.Log("OnDragOut");
        }

        public void OnDragOver()
        {
           EB.Debug.Log("OnDragOver");
        }

        public void OnDragEnd()
        {
           EB.Debug.Log("OnDragEnd");
        }

        public void OpenChatTalkUI()
        {
            string channelStr = ChatRule.CHANNEL2STR[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) +
                GetSendInterval(GetCurChannelType()) - Time.realtimeSinceStartup;
            if (leftTime > 0)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("OpenChatTalkUI: time limited");
                return;
            }

            talkUI.CustomSetActive(true);
            StartRecord();
        }

        public void CloseChatTalkUI()
        {
            talkUI.CustomSetActive(false);
            EndRecord();
        }

        private void StartRecord()
        {
            moveFingerObj.CustomSetActive(true);
            releaseFingerObj.CustomSetActive(false);

            if (audioRecord != null)
            {
                EB.Debug.LogWarning("StartRecord: recording ...");
                return;
            }

            if (Microphone.devices.Length <= 0)
            {
                EB.Debug.LogWarning("StartRecord: their is no microphone devices");
                return;
            }

            microphoneDevice = Microphone.devices[0];
            if (string.IsNullOrEmpty(microphoneDevice))
            {
                EB.Debug.LogWarning("StartRecord: invalid microphone device");
                return;
            }

            if (audioPlay != null)
            {
                audioPlay.Stop();
                audioPlay = null;
            }
            else
            {
                pausedMusic = FusionAudio.StopMusic();
            }

            audioRecord = Microphone.Start(microphoneDevice, false, Mathf.FloorToInt(AudioWraper.MAXLENGTH),
                AudioWraper.FREQUENCY);
            if (audioRecord == null)
            {
                int minFreq = 0, maxFreq = 0;
                Microphone.GetDeviceCaps(microphoneDevice, out minFreq, out maxFreq);

                EB.Debug.LogWarning(
                    "StartRecord: Microphone.Start with device {0} minFreq = {1} maxFreq = {2} audioFreq = {3} maxLength = {4} failed",
                    microphoneDevice, minFreq, maxFreq, AudioWraper.FREQUENCY, AudioWraper.MAXLENGTH);

                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                microphoneDevice = string.Empty;
                
                LTChatManager.Instance.AskMicrophone();
            }
        }

        private void EndRecord()
        {
            if (Microphone.devices.Length <= 0)
            {
                EB.Debug.LogWarning("EndRecord: their is no microphone devices");
                return;
            }

            if (audioRecord == null)
            {
                EB.Debug.LogWarning("EndRecord: audio clip is null");
                return;
            }

            if (string.IsNullOrEmpty(microphoneDevice))
            {
                EB.Debug.LogWarning("EndRecord: invalid microphone device");
                return;
            }

            int samplePos = audioRecord.samples;
            if (Microphone.IsRecording(microphoneDevice))
            {
                samplePos = Microphone.GetPosition(microphoneDevice);
                Microphone.End(microphoneDevice);
            }

            int limit = Mathf.FloorToInt(AudioWraper.MINLENGTH * AudioWraper.FREQUENCY);
            if (samplePos < limit)
            {
                MessageTemplateManager.ShowMessage(902102);
                EB.Debug.LogWarning("EndRecord: too short, {0} < {1}", samplePos, limit);
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }

            ChatRule.CHAT_CHANNEL channelType = GetCurChannelType();
            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE)
            {
                EB.Debug.LogWarning("EndRecord: invalid channel");
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }

            bool privateChannel = true;

            string channelStr = ChatRule.CHANNEL2STR[channelType];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) + GetSendInterval(channelType) -
                             Time.realtimeSinceStartup;
            if (leftTime > 0)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("EndRecord: time limited");
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }

            // 如果在别人的黑名单里面或者别人在自己的黑名单里面，该消息只做本地显示，不向服务器上发
            if (FriendManager.Instance.CheckBeblack(ChatController.instance.TargetPrivateUid) ||
                FriendManager.Instance.CheckBlacklist(ChatController.instance.TargetPrivateUid))
            {
                EB.Sparx.ChatMessage msg =
                    ChatController.instance.NewChatMessage(ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE, string.Empty);
                msg.privateUid = ChatController.instance.TargetPrivateUid;
                msg.privateName = ChatController.instance.TargetPrivateName;
                msg.isAudio = true;
                msg.audioClip = audioRecord;
                SparxHub.Instance.ChatManager.OnMessages(new EB.Sparx.ChatMessage[] {msg});
            }
            else if (privateChannel)
            {
                ChatController.instance.RequestSendChat(channelType, audioRecord, samplePos,
                    ChatController.instance.TargetPrivateUid, ChatController.instance.TargetPrivateName);

                LaunchChat();
            }
            else
            {
                ChatController.instance.RequestSendChat(channelType, audioRecord, samplePos);
            }

            audioRecord = null;
            microphoneDevice = string.Empty;
            FusionAudio.ResumeMusic(pausedMusic);
            pausedMusic = string.Empty;
            mInvalid = true;
        }

        public void PlayAudio(AudioClip clip)
        {
            if (audioRecord != null)
            {
                EB.Debug.Log("PlayAudio: recording ...");
                return;
            }

            if (audioPlay != null)
            {
                audioPlay.Stop();
                audioPlay = null;
            }
            else
            {
                pausedMusic = FusionAudio.StopMusic();
            }

            audioPlay = NGUITools.PlaySound(clip);
        }

        #endregion

        #region chat content items

        private void ShowCurChannelChatList()
        {
            RecycleAll();

            mInvalid = true;

            long targetId = ChatController.instance.TargetPrivateUid;
            TabDynaScroll.ClearItemData();
            FriendManager.Instance.GetChatHistory(targetId,
                delegate(List<EB.Sparx.ChatMessage> msgs) { AddMessages(msgs.ToArray()); });
        }

        private void OnHandleAsyncMessage(EB.Sparx.ChatMessage[] msgs)
        {
            ChatRule.CHAT_CHANNEL channelType = GetCurChannelType();
            string channel = ChatRule.CHANNEL2STR[channelType];

            List<EB.Sparx.ChatMessage> channelMsgs = new List<EB.Sparx.ChatMessage>();
            for (int i = 0, len = msgs.Length; i < len; ++i)
            {
                var msg = msgs[i];
                if (msg.channelType == channel)
                {
                    channelMsgs.Add(msg);
                }
            }

            if (channelMsgs.Count > 0)
            {
                EB.Sparx.ChatMessage[] msgArr = channelMsgs.ToArray();
                EB.Sparx.ChatMessage[] curTargetMsgs = System.Array.FindAll(msgArr,
                    m => m.privateUid == ChatController.instance.TargetPrivateUid ||
                         m.uid == ChatController.instance.TargetPrivateUid);
                if (curTargetMsgs.Length > 0)
                    AddMessages(curTargetMsgs);
            }
        }

        private void OnConnected()
        {
            ShowCurChannelChatList();
        }

        private void OnError()
        {
            // disable send
            talkButton.isEnabled = false;
            sendButton.isEnabled = false;

            StopAllCoroutines();
        }

        public UITable mainTable;
        public UIWidget placeholder;
        public UIScrollView scrollView;

        private bool mInvalid;
        private const int MaxHistoryCount = 50;
        private EB.CircularBuffer<ChatItem> mActive = new EB.CircularBuffer<ChatItem>(MaxHistoryCount);
        private EB.CircularBuffer<ChatItem> mPool = new EB.CircularBuffer<ChatItem>(MaxHistoryCount);

        public void AddMessages(EB.Sparx.ChatMessage[] msgs)
        {
            if (msgs.Length > 0 && mDMono.gameObject.activeInHierarchy)
                AddMessagesCoroutine(msgs);
        }

        private void AddMessagesCoroutine(EB.Sparx.ChatMessage[] msgs)
        {
            ChatUIMessage[] uiMsgs = new ChatUIMessage[msgs.Length];
            for (int i = 0; i < msgs.Length; ++i)
            {
                uiMsgs[i] = new ChatUIMessage(msgs[i]);

                ChatRule.CHAT_CHANNEL channel = ChatRule.STR2CHANNEL[msgs[i].channelType];
                uiMsgs[i].Channel = channel;
                uiMsgs[i].ChannelSpriteName =
                    ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            }

            if (mActive.Count == 0 && mPool.Count == 0) SetChatItem();
            
            TabDynaScroll.SetItemData(uiMsgs, MaxHistoryCount);
        }

        private void AddMessage(EB.Sparx.ChatMessage msg)
        {
            ChatItem item = Use();

            ChatUIMessage uimsg = new ChatUIMessage(msg);
            var channel = ChatRule.STR2CHANNEL[msg.channelType];
            uimsg.Channel = channel;
            uimsg.ChannelSpriteName = ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            item.SetItemData(uimsg);
        }

        private void CleanOverflow()
        {
            int clean = mActive.Count + 1 - MaxHistoryCount;
            for (int i = 0; i < clean; ++i)
            {
                ChatItem item = mActive.Dequeue();
                Recycle(item);
            }
        }

        /*private void Reposition()
        {
            scrollView.InvalidateBounds();
            if (scrollView.canMoveVertically)
            {
    #if REVERSE_ORDER
                float target = 0.0f;
    #else
                float target = 1.0f;
    #endif
                if (mInvalid)
                {
                    scrollView.SetDragAmount(1f, target, false);
                    scrollView.SetDragAmount(1f, target, true);
                }
                else if (Mathf.Abs(target - scrollView.verticalScrollBar.value) < 0.01f)
                {
                    scrollView.SetDragAmount(1f, target, false);
                }
                scrollView.UpdatePosition();
            }
            mInvalid = false;
        }*/

        private ChatItem Use()
        {
            if (mPool.Count > 0)
            {
                ChatItem item = mPool.Dequeue();
                item.mDMono.gameObject.CustomSetActive(true);
                mActive.Enqueue(item);
                item.mDMono.transform.parent = mainTable.transform;
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.SetAsLastSibling();
                return item;
            }
            else
            {
                DynamicMonoILR ilr = UnityEngine.Object.Instantiate(mActive[mActive.Count - 1].mDMono);
                ChatItem item = ilr._ilrObject as ChatItem;
                mActive.Enqueue(item);
                item.mDMono.transform.parent = mainTable.transform;
                item.mDMono.transform.localPosition = Vector3.zero;
                item.mDMono.transform.localScale = Vector3.one;
                item.mDMono.transform.localEulerAngles = Vector3.zero;
                item.mDMono.transform.SetAsLastSibling();
                item.SetItemData(null);
                return item;
            }
        }

        private void Recycle(ChatItem item)
        {
            item.mDMono.gameObject.CustomSetActive(false);
            mPool.Enqueue(item);
        }

        private void RecycleAll()
        {
            while (mActive.Count > 0)
            {
                ChatItem item = mActive.Dequeue();
                Recycle(item);
            }
        }

        #endregion

        public override void Awake()
        {
            base.Awake();
            InitView();
            SetChatItem();
        }

        bool isfirst = true;

        void SetChatItem()
        {
            if (isfirst)
            {
                isfirst = false;
                for (int i = 0; i < mainTable.transform.childCount; ++i)
                {
                    ChatItem item = mainTable.transform.GetChild(i).GetMonoILRComponent<ChatItem>();
                    Recycle(item);
                }
            }
        }

        public void Update()
        {
            if (audioRecord != null && !Microphone.IsRecording(microphoneDevice))
            {
                CloseChatTalkUI();
            }

            if (audioPlay != null && !audioPlay.isPlaying)
            {
                audioPlay = null;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
            }
        }

        public void OnAddToStack()
        {
            InitEmoticon();
        }

        public void OnRemoveFromStack()
        {
            CleanEmoticon();

            if (audioPlay != null)
            {
                audioPlay.Stop();
                audioPlay = null;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
            }
        }

        public override void OnEnable()
        {
			//base.OnEnable();
			RegisterMonoUpdater();

			RegisterListeners();
        }

        public override void OnDisable()
        {
            ErasureMonoUpdater();

            UnRegisterListeners();
        }

        public void RegisterListeners()
        {
            var cm = SparxHub.Instance.ChatManager;
            cm.OnConnected += OnConnected;
            cm.OnMessages += OnHandleAsyncMessage;
            cm.OnDisconnected += OnError;
        }

        public void UnRegisterListeners()
        {
            var cm = SparxHub.Instance.ChatManager;
            cm.OnConnected -= OnConnected;
            cm.OnMessages -= OnHandleAsyncMessage;
            cm.OnDisconnected -= OnError;
        }

        public void Show(long uid, string name)
        {
            ChatController.instance.TargetPrivateUid = uid;
            ChatController.instance.TargetPrivateName = name;
            ShowCurChannelChatList();
        }

        private ChatRule.CHAT_CHANNEL GetCurChannelType()
        {
            return ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE;
        }

        private int GetSendInterval(ChatRule.CHAT_CHANNEL channel)
        {
            return sendInterval;
        }

        private void LaunchChat()
        {
            if (friendCtrl.TabController.Type == eFriendType.My ||
                friendCtrl.TabController.Type == eFriendType.Black)
            {
                var recently = FriendManager.Instance.Recentlys.Find(ChatController.instance.TargetPrivateUid);
                if (recently == null)
                {
                    FriendManager.Instance.LaunchChat(ChatController.instance.TargetPrivateUid,
                        delegate(bool successful)
                        {
                            if (successful)
                            {
                                FriendManager.Instance.MarkDirty(FriendManager.MyFriendListId);
                            }
                        });
                }
            }
            else
            {
                if (!FriendManager.Instance.CheckBeblack(ChatController.instance.TargetPrivateUid))
                {
                    FriendManager.Instance.LaunchChat(ChatController.instance.TargetPrivateUid,
                        delegate(bool successful)
                        {
                            if (successful)
                            {
                            }
                        });
                }
            }
        }

        private Transform control;
        private UIEventTrigger HotfixBtn1;

        public void InitView()
        {
            control = mDMono.transform.parent;  //parent:RightSide  sub:ChatWindow  InputLayout
            inputLabel = control.Find("InputLayout/InputBtn").GetComponent<UIInput>();
            talkUI = control.Find("ChatWindow/TalkUI").gameObject;
            emoticonUI = control.parent.Find("EmoticonUI").gameObject;
            control.parent.Find("EmoticonUI/Blocker").GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(HideEmoticonUI));
            emoticonTable = emoticonUI.transform.Find("EmoticonFrame/Grid").gameObject;
            moveFingerObj = control.Find("ChatWindow/TalkUI/moveFinger").gameObject;
            releaseFingerObj = control.Find("ChatWindow/TalkUI/releaseFinger").gameObject;
            pressTalkObj = control.Find("InputLayout/TalkBtn").gameObject;
            TabDynaScroll = control.Find("ChatWindow/Content/Placeholder/Container").GetMonoILRComponent<ChatItemDynamicScroll>();
            sendInterval = 2;
            InputBtn = control.Find("InputLayout/InputBtn").gameObject;
            mainTable = control.Find("ChatWindow/Content/Placeholder/Container").GetComponent<UITable>();
            placeholder = control.Find("ChatWindow/Content/Placeholder").GetComponent<UIWidget>();
            scrollView = control.Find("ChatWindow/Content").GetComponent<UIScrollView>();
          
            talkButton = control.Find("InputLayout/RecordBtn").GetComponent<UIButton>();
            talkButton.onClick.Add(new EventDelegate(OnAudioBtnClick));
            keyboardBtn = control.Find("InputLayout/KeyboardBtn").GetComponent<UIButton>();
            keyboardBtn.onClick.Add(new EventDelegate(OnKeyboardBtnClick));
            emoticonButton = control.Find("InputLayout/EmoticonBtn").GetComponent<UIButton>();
            emoticonButton.onClick.Add(new EventDelegate(ShowEmoticonUI));
            sendButton = control.Find("InputLayout/SendBtn").GetComponent<UIButton>();
            sendButton.onClick.Add(new EventDelegate(OnSendBtnClick));

            HotfixBtn1 = pressTalkObj.GetComponent<UIEventTrigger>();
            HotfixBtn1.onPress.Add(new EventDelegate(OnPress));
            HotfixBtn1.onRelease.Add(new EventDelegate(OnRelease));
            HotfixBtn1.onDragStart.Add(new EventDelegate(OnDragStart));
        }
    }
}