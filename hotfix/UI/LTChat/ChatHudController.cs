using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class ChatHudController : UIControllerHotfix
    {
        #region left items
        public ChatItemDynamicScroll TabDynaScroll;
        private Dictionary<ChatRule.CHAT_CHANNEL, ChannelLabelData> channelLabelDic = new Dictionary<ChatRule.CHAT_CHANNEL, ChannelLabelData>();
        public ChannelLabelData CurChannel = null;
        public int maxWidth = 100;
        public const int MaxHistoryCount = 50;

        private GameObject WorldRP, AllianceRP, TeamRP, NationRP, SysRP;

        private Dictionary<ChatRule.CHAT_CHANNEL, string> mChatTempTalk;

		//private Rect m_SafeArea;

		// private void SetBtnState(GameObject _btn, bool selected)
		// {
		//     UISprite btnSprite = _btn.GetComponent<UISprite>();
		//     UIButton btnComp = _btn.GetComponent<UIButton>();
		//     if (btnSprite == null || btnComp == null)
		//         return;

		//     string tepSpriteName = ChatRule.CHANNEL_SPRITE[selected];
		//     btnComp.normalSprite = tepSpriteName;
		//     btnSprite.spriteName = tepSpriteName;
		// }

		// private void SetTextColor(GameObject _obj, Color _color)
		// {
		//     UILabel textCmp = _obj.GetComponentInChildren<UILabel>();
		//     if (textCmp != null)
		//     {
		//         textCmp.color = _color;
		//     }
		// }

		private void InitLeftList()
        {
            InitChannelDic();
            FreshLeftList();
        }
        
        private void InitChannelDic()
        {
            ChannelLabelData[] labels = controller.UiGrids["leftList"].transform.GetMonoILRComponentsInChildren<ChannelLabelData>("Hotfix_LT.UI.ChannelLabelData");
            channelLabelDic.Clear();
            for (int i = 0, len = labels.Length; i < len; ++i)
            {
                channelLabelDic.Add(labels[i].channelType, labels[i]);
                switch (labels[i].channelType)
                {
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD: WorldRP = labels[i].newMessage; break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE: AllianceRP = labels[i].newMessage; break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM: TeamRP =labels[i].newMessage; break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION: NationRP = labels[i].newMessage; break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM: SysRP = labels[i].newMessage; break;
                }
            }
        }
        private void SetRP()
        {
            WorldRP.CustomSetActive(LTChatManager.hasNewWorldMessage);
            AllianceRP.CustomSetActive(LTChatManager.hasNewAllianceMessage);
            TeamRP.CustomSetActive(LTChatManager.hasNewTeamMessage);
            NationRP.CustomSetActive(LTChatManager.hasNewNationMessage);
            SysRP.CustomSetActive(LTChatManager.hasNewSysMessage);
            if (CurChannel != null)
            {
                switch (CurChannel.channelType)
                {
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD: WorldRP.CustomSetActive(false); break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE: AllianceRP.CustomSetActive(false); break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM: TeamRP.CustomSetActive(false); break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION: NationRP.CustomSetActive(false); break;
                    case ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM: SysRP.CustomSetActive(false); break;
                }
                LTChatManager.Instance.SetHasNewMessage(CurChannel.channelType, false);
            }
        }
        private void FreshLeftList()
        {
            var iter = channelLabelDic.GetEnumerator();
            while (iter.MoveNext())
            {
                iter.Current.Value.mDMono.gameObject.CustomSetActive(true);
            }
            iter.Dispose();
        }

        private bool first = true;
        public void OnChannelLabelClick(ChannelLabelData channel)
        {
            bool RaiseEvent = true;
            if (channel.channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM && channel.tab.value)
            {
                RaiseEvent = false;
                controller.UiTables["mainTable"].gameObject.CustomSetActive(false);
                controller.UiTables["SysTable"].gameObject.CustomSetActive(true);
                if (!first)
                    MainMenuChat.hasNewSysMessage = false;
                else
                    first = false;
                SetChannel(channel.channelType);
                PlayChannelLabelAni(channel.mDMono.transform);
                controller.BoxColliders["inputCollider"].enabled = false;
                controller.GObjects["InputBtn"].transform.GetChild(0).GetComponent<UISymbolLabel>().text = EB.Localizer.GetString("ID_codefont_in_ChatHudController_3979");
            }
            else if ((channel.channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE || channel.channelType != GetCurChannelType()) && channel.tab.value)
            {
                RaiseEvent = false;
                bool setSuccess = SetChannel(channel.channelType);
                if (setSuccess)
                {
                    controller.UiTables["mainTable"].gameObject.CustomSetActive(true);
                    controller.UiTables["SysTable"].gameObject.CustomSetActive(false);
                    PlayChannelLabelAni(channel.mDMono.transform);
                    ShowCurChannelChatList();
                    controller.BoxColliders["inputCollider"].enabled = true;
                    controller.GObjects["InputBtn"].transform.GetChild(0).GetComponent<UISymbolLabel>().text = EB.Localizer.GetString("ID_CHAT_INPUT_PLACEHOLDER");

                    string tempTalk = GetChatTempTalk(CurChannel.channelType);
                    if (!string.IsNullOrEmpty(tempTalk))
                    {
                        controller.UiSymbolInputs["inputLabel"].value = tempTalk;
                        controller.GObjects["InputBtn"].transform.GetChild(0).GetComponent<UISymbolLabel>().text = tempTalk;
                    }
                }
                else
                {
                    CurChannel.tab.Set(true);
                    CurChannel.mDMono.transform.GetComponent<UIToggledObjects>().SetToggle(CurChannel.tab);
                    AddChatTempTalk(channel.channelType, string.Empty);
                }
            }

            if(RaiseEvent) Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Chat);
        }

        private bool SetChannel(ChatRule.CHAT_CHANNEL _channel)
        {
            if (CurChannel != null)
            {
                AddChatTempTalk(CurChannel.channelType, controller.UiSymbolInputs["inputLabel"].value);
            }
            if (_channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE)
            {
                controller.UiSymbolInputs["inputLabel"].characterLimit += NGUISymbolText.GetStringWidth(ChatController.instance.TargetPrivateName);

                bool isHasNotPrivate = ChatController.instance.TargetPrivateUid <= 0;

                SetMagenta(controller.UiButtons["sendBtn"], isHasNotPrivate);
                SetMagenta(controller.UiButtons["keyboardBtn"], isHasNotPrivate);
                SetMagenta(controller.UiButtons["talkBtn"], isHasNotPrivate );
                SetMagenta(controller.UiButtons["emoticonBtn"], isHasNotPrivate);
                SetMagenta(controller.UiButtons["pressTalkBtn"], isHasNotPrivate || Microphone.devices.Length <= 0);
                controller.UiSymbolInputs["inputLabel"].value = string.Empty;
                controller.UiSymbolInputs["inputLabel"].enabled = !isHasNotPrivate;
            }
            else if (_channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE)
            {
                bool isHasNotAlliance = AlliancesManager.Instance.Account.State != eAllianceState.Joined;

                if (isHasNotAlliance)
                {
                    string name = EB.Localizer.GetString(ChatItem.ChannelName[_channel]);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_ChatHudController_6727"), name, name));
                    return false;
                }

                SetMagenta(controller.UiButtons["sendBtn"], isHasNotAlliance);
                SetMagenta(controller.UiButtons["keyboardBtn"], isHasNotAlliance);
                SetMagenta(controller.UiButtons["talkBtn"], isHasNotAlliance);
                SetMagenta(controller.UiButtons["emoticonBtn"], isHasNotAlliance);
                SetMagenta(controller.UiButtons["pressTalkBtn"], isHasNotAlliance || Microphone.devices.Length <= 0);
                controller.UiSymbolInputs["inputLabel"].value = string.Empty;
                controller.UiSymbolInputs["inputLabel"].enabled = !isHasNotAlliance;
            }
            else if (_channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION)
            {
                string nation = string.Empty;
                DataLookupsCache.Instance.SearchDataByID("user.nation", out nation);
                bool isHasNotNation = string.IsNullOrEmpty(NationManager.Instance.Account.NationName) && string.IsNullOrEmpty(nation);
                if (isHasNotNation)
                {
                    string name = EB.Localizer.GetString(ChatItem.ChannelName[_channel]);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_ChatHudController_6727"), name, name));
                    return false;
                }

                SetMagenta(controller.UiButtons["sendBtn"], isHasNotNation);
                SetMagenta(controller.UiButtons["keyboardBtn"], isHasNotNation);
                SetMagenta(controller.UiButtons["talkBtn"], isHasNotNation);
                SetMagenta(controller.UiButtons["emoticonBtn"], isHasNotNation);
                SetMagenta(controller.UiButtons["pressTalkBtn"], isHasNotNation || Microphone.devices.Length <= 0);
                controller.UiSymbolInputs["inputLabel"].value = string.Empty;
                controller.UiSymbolInputs["inputLabel"].enabled = !isHasNotNation;
            }
            else if (_channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM)
            {
                string name = EB.Localizer.GetString(ChatItem.ChannelName[_channel]);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_ChatHudController_6727"), name, name));
                return false;
            }
            else if (_channel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD)
            {
                SetMagenta(controller.UiButtons["sendBtn"], false);
                SetMagenta(controller.UiButtons["keyboardBtn"], false);
                SetMagenta(controller.UiButtons["talkBtn"],false);
                SetMagenta(controller.UiButtons["emoticonBtn"], false);
                SetMagenta(controller.UiButtons["pressTalkBtn"], Microphone.devices.Length <= 0);
                controller.UiSymbolInputs["inputLabel"].value = string.Empty;
                controller.UiSymbolInputs["inputLabel"].enabled = true;
            }
            else
            {
                SetMagenta(controller.UiButtons["sendBtn"], true);
                SetMagenta(controller.UiButtons["keyboardBtn"], true);
                SetMagenta(controller.UiButtons["talkBtn"], true);
                SetMagenta(controller.UiButtons["emoticonBtn"], true);
                SetMagenta(controller.UiButtons["pressTalkBtn"], true);
                controller.UiSymbolInputs["inputLabel"].value = string.Empty;
                controller.UiSymbolInputs["inputLabel"].enabled = false;
            }

            CurChannel = channelLabelDic[_channel];
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Chat);

            controller.UiSymbolInputs["inputLabel"].characterLimit = maxWidth;

            return true;
        }

        private void PlayChannelLabelAni(Transform tf)
        {
            TweenScale ts = tf.Find("Active/Sprite").GetComponent<TweenScale>();
            if (ts != null)
            {
                ts.ResetToBeginning();
                ts.PlayForward();
            }
        }

        private void SetMagenta(UIButton btn, bool isSetMagenta)
        {
            LTUIUtil.SetGreyButtonEnable(btn, !isSetMagenta);

            UISprite[] sps = btn.GetComponentsInChildren<UISprite>();
            for (int i = 0; i < sps.Length; i++)
            {
                if (isSetMagenta)
                {
                    sps[i].color = Color.magenta;
                }
                else
                {
                    sps[i].color = Color.white;
                }
            }
        }

        public void GetChatHistory(ChatRule.CHAT_CHANNEL channel, Action<List<EB.Sparx.ChatMessage>> del)
        {
            FriendManager.Instance.AcHistory.GetAllChatHistory(channel, del);
        }

        private ChatRule.CHAT_CHANNEL GetCurChannelType()
        {
            if (CurChannel == null)
                return ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE;
            return CurChannel.channelType;
        }
        #endregion

        public override bool ShowUIBlocker { get { return true; } }

        #region input logic

        public void HideEmoticonUI()
        {
            controller.GObjects["emoticonUI"].CustomSetActive(false);
        }

        public void ShowEmoticonUI()
        {
            controller.GObjects["emoticonUI"].CustomSetActive(true);
        }

        private void InitEmoticon()
        {
            HideEmoticonUI();
            for (int i = 0; i < controller.GObjects["emoticonTable"].transform.childCount; ++i)
            {
                UIEventListener.Get(controller.GObjects["emoticonTable"].transform.GetChild(i).gameObject).onClick += SelectEmoticon;
            }
        }

        private void CleanEmoticon()
        {
            HideEmoticonUI();
            for (int i = 0; i < controller.GObjects["emoticonTable"].transform.childCount; ++i)
            {
                UIEventListener.Get(controller.GObjects["emoticonTable"].transform.GetChild(i).gameObject).onClick -= SelectEmoticon;
            }
        }

        private void SelectEmoticon(GameObject go)
        {
            string tempInputValue = controller.UiSymbolInputs["inputLabel"].value;
            controller.UiSymbolInputs["inputLabel"].value = go.name;
            OnSendBtnClick();
            controller.UiSymbolInputs["inputLabel"].value = tempInputValue;

            HideEmoticonUI();
        }

        private float GetSendInterval(ChatRule.CHAT_CHANNEL channel)
        {
            ChannelLabelData channelData = null;
            if (channelLabelDic.TryGetValue(channel, out channelData))
            {
                return channelData.sendInterval;
            }

            return 0.0f;
        }

        private Vector3 originPos;
        public void OnOpenKeyboard()
        {
            originPos = controller.transform.position;
            if (CurChannel != null)
            {
                string tempTalk = GetChatTempTalk(CurChannel.channelType);
                if (!string.IsNullOrEmpty(tempTalk))
                {
                    controller.UiSymbolInputs["inputLabel"].value = tempTalk;
                    controller.GObjects["InputBtn"].transform.GetChild(0).GetComponent<UISymbolLabel>().text = tempTalk;
                }
            }
            if (ILRDefine.UNITY_ANDROID)
            {
                controller.transform.localPosition = new Vector3(controller.transform.localPosition.x, controller.transform.localPosition.y + UIRoot.list[0].manualHeight * 2 / 3f, controller.transform.localPosition.z);
            }
            else
            {
                Bounds bds = NGUIMath.CalculateAbsoluteWidgetBounds(controller.transform);
                float yValue = TouchScreenKeyboard.area.yMax + bds.size.y / 2 + 0.1f;
                controller.transform.position = new Vector3(originPos.x, yValue, originPos.z);
            }

            StartCoroutine(LookupKeyboardState());
        }

        public void OnCloseKeyboard()
        {
            controller.transform.position = originPos;
            if (CurChannel != null)
            {
                AddChatTempTalk(CurChannel.channelType, controller.UiSymbolInputs["inputLabel"].value);
            }
        }

        private IEnumerator LookupKeyboardState()
        {
            yield return new WaitForSeconds(0.2f);

            while (true)
            {
                yield return null;
                if (ILRDefine.UNITY_ANDROID)
                {
                    if (!AndroidKeyboard.IsVisible || AndroidKeyboard.IsCanceled)
                    {
                        OnCloseKeyboard();
                        yield break;
                    }
                }
                else
                {
                    if (!TouchScreenKeyboard.visible || TouchScreenKeyboard.hideInput)
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
            ChatRule.CHAT_CHANNEL channelType = GetCurChannelType();

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE || !ChatController.instance.CanSend(controller.UiSymbolInputs["inputLabel"].value))
            {
                LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_ChatHudController_14801"));
                EB.Debug.LogWarning("OnSendBtnClick: can't send");
                return;
            }

            bool privateChannel = false;
            string inputStr = controller.UiSymbolInputs["inputLabel"].value.Trim();
            string privateTarget = string.Empty;
            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE)
            {
                int startIndex = 0, endIndex = 0;
                string privateUrl = controller.UiSymbolInputs["inputLabel"].label.DropUrlAtCharacterIndex(0, out startIndex, out endIndex);
                if (!string.IsNullOrEmpty(privateUrl) && ChatController.instance.TargetPrivateUid != 0)
                {
                    EB.Uri uri = new EB.Uri(privateUrl);
                    if (uri.Host == ChatController.instance.TargetPrivateUid.ToString())
                    {
                        privateChannel = true;
                        privateTarget = inputStr.Substring(startIndex, endIndex - startIndex);
                        inputStr = inputStr.Substring(0, startIndex) + inputStr.Substring(endIndex);

                        if (!ChatController.instance.CanSend(inputStr))
                        {
                            EB.Debug.LogWarning("OnSendBtnClick: can't send");
                            return;
                        }
                    }
                }

                if (!privateChannel)
                {
                    ChatController.instance.TargetPrivateUid = 0;
                    ChatController.instance.TargetPrivateName = string.Empty;
                    channelLabelDic[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD].tab.value = true;
                    ShowCurChannelChatList();
                    channelType = ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD;
                }
            }

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE && AlliancesManager.Instance.Account.State != eAllianceState.Joined)
            {
                MessageTemplateManager.ShowMessage(902101);
                EB.Debug.LogWarning("OnSendBtnClick: not joined alliance");
                return;
            }

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION && string.IsNullOrEmpty(NationManager.Instance.Account.NationName))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ChatHudController_16590"));
                EB.Debug.LogWarning("OnSendBtnClick: not joined nation");
                return;
            }

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM)
            {
                EB.Debug.LogWarning("OnSendBtnClick: team not support");
                return;
            }
            int userlevel = BalanceResourceUtil.GetUserLevel();
            int chatlevel = BalanceResourceUtil.GetChatLevel();
            if ((channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD || channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE) && userlevel < chatlevel)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add( "0", chatlevel);
                MessageTemplateManager.ShowMessage(902014, ht, null);
                return;
            }

            string channelStr = ChatRule.CHANNEL2STR[channelType];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) + GetSendInterval(channelType) - Time.realtimeSinceStartup;
            if (leftTime > 0)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("OnSendBtnClick: time limited");
                return;
            }

            inputStr = controller.UiSymbolInputs["inputLabel"].label.ProfanityFilter(inputStr, EB.ProfanityFilter.Filter);
            if (privateChannel)
            {
                ChatController.instance.RequestSendChat(channelType, inputStr, ChatController.instance.TargetPrivateUid, ChatController.instance.TargetPrivateName);
                controller.UiSymbolInputs["inputLabel"].value = privateTarget;
            }
            else
            {
                ChatController.instance.RequestSendChat(channelType, inputStr);
                controller.UiSymbolInputs["inputLabel"].value = string.Empty;
            }

            mInvalid = true;
        }


        #region voice
        private AudioClip audioRecord = null;
        private string microphoneDevice = string.Empty;
        private AudioSource audioPlay = null;
        private string pausedMusic = string.Empty;
        private bool isDragFinger;

        public void OnAudioBtnClick()
        {
            if (Microphone.devices.Length <= 0)
            {
                LTChatManager.Instance.AskMicrophone();
                return;
            }
            OpenAudioInput(true);
        }

        public void OnKeyboardBtnClick()
        {
            if (Microphone.devices.Length <= 0)
            {
                LTChatManager.Instance.AskMicrophone();
                return;
            }
            OpenAudioInput(false);
        }

        private void OpenAudioInput(bool yes)
        {
            controller.UiButtons["talkBtn"].gameObject.CustomSetActive(!yes);
            controller.UiButtons["keyboardBtn"].gameObject.CustomSetActive(yes);
            controller.UiButtons["pressTalkBtn"].gameObject.CustomSetActive(yes);
            controller.GObjects["InputBtn"].CustomSetActive(!yes);
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
                FusionAudio.PostEvent("UI/New/YuYin", true);
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;

                controller.GObjects["talkUI"].CustomSetActive(false);
            }
            else
            {
                CloseChatTalkUI();
            }
        }

        public void OnDragStart()
        {
            isDragFinger = true;
            controller.GObjects["moveFingerObj"].CustomSetActive(false);
            controller.GObjects["releaseFingerObj"].CustomSetActive(true);
        }

        public void OpenChatTalkUI()
        {
            ChatRule.CHAT_CHANNEL channelType = GetCurChannelType();
            int userlevel = BalanceResourceUtil.GetUserLevel();
            int chatlevel = BalanceResourceUtil.GetChatLevel();
            if ((channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD || channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE) && userlevel < chatlevel)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", chatlevel);
                MessageTemplateManager.ShowMessage(902014, ht, null);
                return;
            }

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE)
            {
                EB.Debug.LogWarning("OpenChatTalkUI: can't send");
                return;
            }

            string channelStr = ChatRule.CHANNEL2STR[channelType];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) + GetSendInterval(channelType) - Time.realtimeSinceStartup;
            if (leftTime > 0)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("OpenChatTalkUI: time limited");
                return;
            }
            controller.GObjects["talkUI"].CustomSetActive(true);
            StartRecord();
        }

        public void CloseChatTalkUI()
        {
            controller.GObjects["talkUI"].CustomSetActive(false);
            EndRecord();
        }

        private void StartRecord()
        {
            controller.GObjects["moveFingerObj"].CustomSetActive(true);
            controller.GObjects["releaseFingerObj"].CustomSetActive(false);

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

            audioRecord = Microphone.Start(microphoneDevice, false, Mathf.FloorToInt(AudioWraper.MAXLENGTH), AudioWraper.FREQUENCY);
            if (audioRecord == null)
            {
                int minFreq = 0, maxFreq = 0;
                Microphone.GetDeviceCaps(microphoneDevice, out minFreq, out maxFreq);

                EB.Debug.LogWarning("StartRecord: Microphone.Start with device {0} minFreq = {1} maxFreq = {2} audioFreq = {3} maxLength = {4} failed",
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

            bool privateChannel = false;
            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE)
            {
                int startIndex = 0, endIndex = 0;
                string privateUrl = controller.UiSymbolInputs["inputLabel"].label.DropUrlAtCharacterIndex(0, out startIndex, out endIndex);
                if (!string.IsNullOrEmpty(privateUrl) && ChatController.instance.TargetPrivateUid != 0)
                {
                    EB.Uri uri = new EB.Uri(privateUrl);
                    if (uri.Host == ChatController.instance.TargetPrivateUid.ToString())
                    {
                        privateChannel = true;
                    }
                }

                if (!privateChannel)
                {
                    ChatController.instance.TargetPrivateUid = 0;
                    ChatController.instance.TargetPrivateName = string.Empty;
                    channelLabelDic[ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD].tab.value = true;
                    ShowCurChannelChatList();
                    channelType = ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD;
                }
            }

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE && AlliancesManager.Instance.Account.State != eAllianceState.Joined)
            {
                MessageTemplateManager.ShowMessage(902102);
                EB.Debug.LogWarning("EndRecord: not joined alliance");
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }

            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM)
            {
                EB.Debug.LogWarning("EndRecord: team not support");
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }

            string channelStr = ChatRule.CHANNEL2STR[channelType];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) + GetSendInterval(channelType) - Time.realtimeSinceStartup;
            if (leftTime > 0)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add( "0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("EndRecord: time limited");
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }

            if (privateChannel)
            {
                ChatController.instance.RequestSendChat(channelType, audioRecord, samplePos, ChatController.instance.TargetPrivateUid, ChatController.instance.TargetPrivateName);
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
        #endregion

        #region chat content items


        private void ShowCurChannelChatList()
        {
            ChatRule.CHAT_CHANNEL channelType = GetCurChannelType();
            RecycleAll();
            mInvalid = true;

            //if (controller.UiTables["mainTable"] == null)
            //{
            //    return;
            //}
            //controller.UiTables["mainTable"].gameObject.CustomSetActive(false);
            //controller.UiTables["mainTable"].gameObject.CustomSetActive(true);

            if (channelLabelDic != null && channelLabelDic.ContainsKey(channelType))
            {
                var channel = channelLabelDic[channelType];

                if (channel.channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM && channel.tab.value)
                {
                    controller.UiTables["mainTable"].gameObject.CustomSetActive(false);
                    controller.UiTables["SysTable"].gameObject.CustomSetActive(true);
                }
                else if ((channel.channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE || channel.channelType != channelType) && channel.tab.value)
                {
                    controller.UiTables["mainTable"].gameObject.CustomSetActive(true);
                    controller.UiTables["SysTable"].gameObject.CustomSetActive(false);
                }
            }

            if (TabDynaScroll != null)
            {
                TabDynaScroll.ClearItemData();
            }

            GetChatHistory(channelType, delegate (List<EB.Sparx.ChatMessage> msgs)
            {
                AddMessages(msgs.ToArray());
            });
        }

        private void OnHandleAsyncMessage(EB.Sparx.ChatMessage[] msgs)
        {
            ChatRule.CHAT_CHANNEL channelType = GetCurChannelType();
            if (channelType == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE)
            {
                return;
            }
            string channel = ChatRule.CHANNEL2STR[channelType];

            List<EB.Sparx.ChatMessage> channelMsgs = new List<EB.Sparx.ChatMessage>();
            for (int i = 0, len = msgs.Length; i < len; ++i)
            {
                var msg = msgs[i];

                if (msg.channelType == channel)
                {
                    if (msg.uid != LoginManager.Instance.LocalUserId.Value)
                    {
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Chat);//channelLabelDic[ChatRule.STR2CHANNEL[msg.channelType]].newMessage.CustomSetActive(true);
                    }

                    channelMsgs.Add(msg);
                }
                else if (ChatRule.STR2CHANNEL.ContainsKey(msg.channelType) && channelLabelDic.ContainsKey(ChatRule.STR2CHANNEL[msg.channelType]))
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Chat);
                }
            }
            if (channelMsgs.Count > 0)
            {
                AddMessages(channelMsgs.ToArray());
            }
        }

        private void OnConnected()
        {
            // refresh
            if (channelLabelDic.ContainsKey(GetCurChannelType()))
            {
                channelLabelDic[GetCurChannelType()].tab.value = true;
                
                ShowCurChannelChatList();
            }
            
        }

        private void OnError()
        {
            // disable send
            if (controller.UiButtons["pressTalkBtn"] != null)
            {
                controller.UiButtons["pressTalkBtn"].isEnabled = false;
            }
            if (controller.UiButtons["sendBtn"] != null)
            {
                controller.UiButtons["sendBtn"].isEnabled = false;
            }

            StopAllCoroutines();
        }


        private bool mInvalid;
        private EB.CircularBuffer<DynamicMonoILR> mActive = new EB.CircularBuffer<DynamicMonoILR>(MaxHistoryCount);
        private EB.CircularBuffer<DynamicMonoILR> mPool = new EB.CircularBuffer<DynamicMonoILR>(MaxHistoryCount);

        public void AddMessages(EB.Sparx.ChatMessage[] msgArr)
        {
            EB.Sparx.ChatMessage[] msgs = FilterMsg(msgArr);

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
                uiMsgs[i].ChannelSpriteName = ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            }

            TabDynaScroll.SetItemData(uiMsgs, MaxHistoryCount);
        }

        private void AddMessage(EB.Sparx.ChatMessage msg)
        {
            var channel = ChatRule.STR2CHANNEL[msg.channelType];
            DynamicMonoILR item = Use();

            ChatUIMessage uimsg = new ChatUIMessage(msg);

            uimsg.Channel = channel;
            uimsg.ChannelSpriteName = ChatRule.CHANNEL2ICON.ContainsKey(channel) ? ChatRule.CHANNEL2ICON[channel] : "";
            (item._ilrObject as ChatItem).SetItemData(uimsg);
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
                DynamicMonoILR item = mActive.Dequeue();
                Recycle(item);
            }
        }

        private void Reposition()
        {
            Bounds bs = NGUIMath.CalculateRelativeWidgetBounds(controller.UiTables["mainTable"].transform);

            float width = bs.size.x;
            float height = bs.size.y;

            var pos = controller.UiWidgets["placeholder"].transform.localPosition;
            controller.UiWidgets["placeholder"].SetRect(0, 0, width, height);
            controller.UiWidgets["placeholder"].transform.localPosition = pos;

            controller.UiScrollViews["scrollView"].InvalidateBounds();
            if (controller.UiScrollViews["scrollView"].canMoveVertically)
            {
                float target = 1.0f;
                if (height > 1314f) target = 1.0f;
                if (mInvalid)
                {
                    controller.UiScrollViews["scrollView"].SetDragAmount(1f, target, false);
                    controller.UiScrollViews["scrollView"].SetDragAmount(1f, target, true);
                }
                else if (Mathf.Abs(target - controller.UiScrollViews["scrollView"].verticalScrollBar.value) < 0.01f)
                {
                    controller.UiScrollViews["scrollView"].SetDragAmount(1f, target, false);
                }
                controller.UiScrollViews["scrollView"].UpdatePosition();
            }

            mInvalid = false;
        }

        private DynamicMonoILR Use()
        {
            if (mPool.Count > 0)
            {
                DynamicMonoILR item = mPool.Dequeue();
                item.gameObject.CustomSetActive(true);
                mActive.Enqueue(item);
                item.transform.SetParent(controller.UiTables["mainTable"].transform);
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.SetAsLastSibling();
                return item;
            }
            else
            {
                DynamicMonoILR item = GameObject.Instantiate(mActive[mActive.Count - 1]);
                mActive.Enqueue(item);
                item.transform.SetParent(controller.UiTables["mainTable"].transform);
                item.transform.localPosition = Vector3.zero;
                item.transform.localScale = Vector3.one;
                item.transform.localEulerAngles = Vector3.zero;
                item.transform.SetAsLastSibling();
                (item._ilrObject as ChatItem).SetItemData(null);
                return item;
            }
        }

        private void Recycle(DynamicMonoILR item)
        {
            item.gameObject.CustomSetActive(false);
            mPool.Enqueue(item);
        }

        private void RecycleAll()
        {
            while (mActive.Count > 0)
            {
                DynamicMonoILR item = mActive.Dequeue();
                Recycle(item);
            }
        }

        #endregion

        public override void Awake()
        {
            base.Awake();
            InitView();
            mChatTempTalk = new Dictionary<ChatRule.CHAT_CHANNEL, string>();
            for (int i = controller.UiTables["mainTable"].transform.childCount - 1; i >= 0; --i)
            {
                DynamicMonoILR item = controller.UiTables["mainTable"].transform.GetChild(i).GetComponent<DynamicMonoILR>();
                Recycle(item);
            }
            AddAction(true);
        }

        private void InitView()
        {
            //TODO
            var t = controller.transform;
            TabDynaScroll = t.GetMonoILRComponent<ChatItemDynamicScroll>("AnchorArea/MotionFrame/CenterSide/Content/Scroll View/Placeholder/Container");
            maxWidth = 100; sOpen = false;

			controller.BindingBtnEvent( GetList("talkBtn", "keyboardBtn", "emoticonBtn", "sendBtn"), 
				GetList( new EventDelegate(OnAudioBtnClick), new EventDelegate(OnKeyboardBtnClick), new EventDelegate(ShowEmoticonUI),
					new EventDelegate(OnSendBtnClick) ) );

			t.GetComponent<UIButton>("AnchorArea/MotionFrame/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            
            UIEventTrigger TalkBtn = controller.UiButtons["pressTalkBtn"].GetComponent<UIEventTrigger>();
            TalkBtn.onPress.Add(new EventDelegate(OnPress));
            TalkBtn.onDragStart.Add(new EventDelegate(OnDragStart));
            TalkBtn.onRelease.Add(new EventDelegate(OnRelease));

            t.GetComponent<UIEventTrigger>("EmoticonBlocker/Blocker").onClick.Add(new EventDelegate(HideEmoticonUI));
            UIToggle[] array = controller.FetchComponentList<UIToggle>(GetArray("AnchorArea/MotionFrame/TopSide/Anchor/Channels/WorldChannel",
	            "AnchorArea/MotionFrame/TopSide/Anchor/Channels/Grid/AllianceChannel", "AnchorArea/MotionFrame/TopSide/Anchor/Channels/Grid/TeamChannel",
	            "AnchorArea/MotionFrame/TopSide/Anchor/Channels/Grid/SystemChannel")).ToArray();
			controller.BindingToggleEvent(array, GetList(GetTglEvent(array[0]), GetTglEvent(array[1]), GetTglEvent(array[2]), GetTglEvent(array[3])));

			//m_SafeArea = Screen.safeArea;
		}

        public EventDelegate GetTglEvent(UIToggle toggle)
        {
			EventDelegate e = new EventDelegate(() =>
			{
				OnChannelLabelClick(toggle.transform.GetMonoILRComponent<ChannelLabelData>());
				toggle.GetComponent<UIToggledObjects>().Toggle();

			});
			return e;
        }

        public override void OnDestroy()
        {
            AddAction(false);
        }
        
		public ChatItem CurPlayAudioItem;
        private int mTimer;
        public void UpdateAudio(int timer)
        {
            if (audioRecord != null && !Microphone.IsRecording(microphoneDevice))
            {
                CloseChatTalkUI();
            }

            if (audioPlay != null && !audioPlay.isPlaying)
            {
                if (CurPlayAudioItem != null)
                {
                    CurPlayAudioItem.ResetPlayAudioState();
                }
                audioPlay = null;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
            }
        }

        private void RemoveTimer()
        {
            if (mTimer > 0)
            {
                ILRTimerManager.instance.RemoveTimer(mTimer);
                mTimer = 0;
            }
        }

        public override void Show(bool isShowing)
        {
	        base.Show(isShowing);
	        PlayTweenSliderIn();
        }

        private void SetBgHeight()
        {
            controller.UiSprites["BG"].height = controller.UiSprites["TargetBG"].height;
        }

        public static bool sOpen = false;

        public override IEnumerator OnAddToStack()
        {
            InitEmoticon();
            sOpen = true;

            SetBgHeight();
            yield return base.OnAddToStack();

            controller.UiWidgets["slideWidget"].gameObject.CustomSetActive(true);

            ChatRule.CHAT_CHANNEL eChannel = GetCurChannelType();
            if (eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE)
            {
                eChannel = ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD;
            }

            ChatController.instance.TargetPrivateUid = 0;
            ChatController.instance.TargetPrivateName = string.Empty;

            if (openEvent != null)
            {
                openEvent();
                openEvent = null;
            }
            else if (channelLabelDic[eChannel].tab.value)
            {
                OnChannelLabelClick(channelLabelDic[eChannel]);
            }
            else
            {
                channelLabelDic[eChannel].tab.value = true;
            }

            SetRP();

            if (!controller.IsOpen())
            {
                yield break;
            }

            
            SetBgHeight();
            yield return new WaitForSeconds(controller.TweenPositions["slideTween"].duration + controller.TweenPositions["slideTween"].delay);

            SetBgHeight();

            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Chat, SetRP);
            this.AddEventListener();

            mTimer = ILRTimerManager.instance.AddTimer(500, int.MaxValue, UpdateAudio);

			//Transform anchor = controller.transform.Find("AnchorArea");
			//anchor.localPosition = new Vector3(-684f + m_SafeArea.x, 3, 0);

			// 打印安全区域
			//Debug.LogError("安全区域为：" + m_SafeArea);//-684f+x
		}

        void PlayTweenSliderIn()
        {
	        Vector3 to = Vector3.zero;
	        Vector3 from = Vector3.zero;

	        float a = (float)Screen.width / Screen.height;
	        float b = (float)2730 / 1536;
	        float x = 0;
	        if (from.x == 0 && to.x == 0 && a > b)
	        {
		        x = (2730 * (a / b) - 2730) / 2;
	        }

	        from.x = -2055 - x;
	        to.x = -674 - x;
	        controller.UiWidgets["slideWidget"].transform.localPosition = from;

	        controller.TweenPositions["slideTween"].from = from;
	        controller.TweenPositions["slideTween"].to = to;
	        controller.TweenPositions["slideTween"].PlayForward();
		}

        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Chat, SetRP);
            LTChatManager.Instance.SetHasNewMessage(CurChannel.channelType, false);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Chat);
            CleanEmoticon();

            if (audioPlay != null)
            {
                audioPlay.Stop();
                audioPlay = null;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
            }

            controller.TweenPositions["slideTween"].PlayReverse();
            yield return new WaitForSeconds(controller.TweenPositions["slideTween"].duration + controller.TweenPositions["slideTween"].delay);
            sOpen = false;
            CurChannel = null;
            if (controller!=null && controller.gameObject != null) DestroySelf();
            yield break;
        }

        private void AddAction(bool isAdd)
        {
            var cm = EB.Sparx.Hub.Instance.ChatManager;
            if (isAdd)
            {
                cm.OnConnected += OnConnected;
                cm.OnMessages += OnHandleAsyncMessage;
                cm.OnDisconnected += OnError;
            }
            else
            {
                cm.OnConnected -= OnConnected;
                cm.OnMessages -= OnHandleAsyncMessage;
                cm.OnDisconnected -= OnError;
            }
        }

        private System.Action openEvent;
        private object mParam;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mParam = param;
            InitLeftList();
            if (mParam != null)
            {
                ChatRule.CHAT_CHANNEL channel = (ChatRule.CHAT_CHANNEL)param;
                if (channel != ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NONE)
                {
                    if (channelLabelDic[channel].tab.value)
                    {
                        openEvent = () =>
                        {
                            OnChannelLabelClick(channelLabelDic[channel]);
                        };
                    }
                    else
                    {
                        openEvent = () =>
                        {
                            channelLabelDic[channel].tab.value = true;
                            OnChannelLabelClick(channelLabelDic[channel]);
                        };
                    }
                }
            }
        }

        private void onDrag(GameObject go, Vector2 delta)
        {
            if (MyFollowCamera.delTouchDownInView != null)
                MyFollowCamera.delTouchDownInView();
        }

        private void onPress(GameObject go, bool isPress)
        {
            if (MyFollowCamera.delTouchDownInView != null && isPress)
                MyFollowCamera.delTouchDownInView();
        }

        private UIEventListener[] _dontMoveCameraListeners;

        private void AddEventListener()
        {
            var colliders = controller.gameObject.GetComponentsInChildren<BoxCollider>(true);
            for (int j = 0; j < colliders.Length; j++)
            {
                if (colliders[j].transform.GetComponent<UIEventListener>() == null)
                {
                    colliders[j].gameObject.AddComponent<UIEventListener>();
                }
            }
            var EventListeners = controller.gameObject.GetComponentsInChildren<UIEventListener>(true);

            _dontMoveCameraListeners = new UIEventListener[EventListeners.Length];
            int i = 0;
            for (int j = 0; j < EventListeners.Length; j++)
            {
                _dontMoveCameraListeners[i] = EventListeners[j];
                _dontMoveCameraListeners[i].onDrag += onDrag;
                _dontMoveCameraListeners[i].onPress += onPress;
            }
        }

        private void AddChatTempTalk(ChatRule.CHAT_CHANNEL channel, string talk)
        {
            if (mChatTempTalk == null)
            {
                return;
            }
            if (mChatTempTalk.ContainsKey(channel))
            {
                mChatTempTalk[channel] = talk;
            }
            else
            {
                mChatTempTalk.Add(channel, talk);
            }
        }

        private string GetChatTempTalk(ChatRule.CHAT_CHANNEL channel)
        {
            if (mChatTempTalk.ContainsKey(channel))
            {
                return mChatTempTalk[channel];
            }

            return string.Empty;
        }
    }

}