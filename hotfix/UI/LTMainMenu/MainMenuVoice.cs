using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class MainMenuVoice : DynamicMonoHotfix
    {
        public GameObject moveFingerObj;
        public GameObject releaseFingerObj;
        public GameObject pressTalkObj;
        public GameObject talkUI;
    
        private bool isDragFinger;
        private AudioClip audioRecord = null;
        private string microphoneDevice = string.Empty;
        private AudioSource audioPlay = null;
        private string pausedMusic = string.Empty;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            var edge = t.parent.parent.parent;
            moveFingerObj = edge.FindEx("Center/TalkUI/moveFinger").gameObject;
            releaseFingerObj = edge.FindEx("Center/TalkUI/releaseFinger").gameObject;
            pressTalkObj = edge.FindEx("Edge/Bottom/VoiceBtn").gameObject;
            talkUI = edge.FindEx("Center/TalkUI").gameObject;

            var eventTrigger = t.GetComponentEx<UIEventTrigger>();
            eventTrigger.onPress.Add(new EventDelegate(() => OnPress(mDMono.isActiveAndEnabled)));
            eventTrigger.onDragStart.Add(new EventDelegate(OnDragStart));
            eventTrigger.onRelease.Add(new EventDelegate(OnRelease));
        }

        public void OnPress(bool flag) {
            if(Microphone.devices.Length <= 0)
            {
                LTChatManager.Instance.AskMicrophone();
                return;
            }
            if (flag)
            {
                isDragFinger = false;
                
                OpenChatTalkUI();
            }
        }
    
        public void OnDragStart() {
            isDragFinger = true;
            moveFingerObj.CustomSetActive(false);
            releaseFingerObj.CustomSetActive(true);
        }
        public void OnRelease() {
            if (isDragFinger) {
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
    
                talkUI.CustomSetActive(false);
            }
            else {
                CloseChatTalkUI();
            }
        }
    
        public void OpenChatTalkUI()
        {
            ChatRule.CHAT_CHANNEL channelType = ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD;
            int userlevel = BalanceResourceUtil.GetUserLevel();
            int chatlevel = BalanceResourceUtil.GetChatLevel();
            if (userlevel < chatlevel) {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", chatlevel);
                MessageTemplateManager.ShowMessage(902014, ht, null);
                return;
            }
    
            string channelStr = ChatRule.CHANNEL2STR[channelType];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) + GetSendInterval(channelType) - Time.realtimeSinceStartup;
            if (leftTime > 0) {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("0", Mathf.CeilToInt(leftTime).ToString());
                MessageTemplateManager.ShowMessage(902100, ht, null);
                EB.Debug.LogWarning("OpenChatTalkUI: time limited");
                return;
            }
            talkUI.CustomSetActive(true);
            StartRecord();
        }
    
        public void CloseChatTalkUI() {
            talkUI.CustomSetActive(false);
            EndRecord();
        }
    
        private float GetSendInterval(ChatRule.CHAT_CHANNEL channel) {      
            return 10f;
        }
    
        private void StartRecord() {
            moveFingerObj.CustomSetActive(true);
            releaseFingerObj.CustomSetActive(false);
    
            if (audioRecord != null) {
                EB.Debug.LogWarning("StartRecord: recording ...");
                return;
            }
    
            if (Microphone.devices.Length <= 0) {
                EB.Debug.LogWarning("StartRecord: their is no microphone devices");
                return;
            }
    
            microphoneDevice = Microphone.devices[0];
            if (string.IsNullOrEmpty(microphoneDevice)) {
                EB.Debug.LogWarning("StartRecord: invalid microphone device");
                return;
            }
    
            if (audioPlay != null) {
                audioPlay.Stop();
                audioPlay = null;
                // keep pausedMusic
            }
            else {
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
    
        private void EndRecord() {
            if (Microphone.devices.Length <= 0) {
                EB.Debug.LogWarning("EndRecord: their is no microphone devices");
                return;
            }
    
            if (audioRecord == null) {
                EB.Debug.LogWarning("EndRecord: audio clip is null");
                return;
            }
    
            if (string.IsNullOrEmpty(microphoneDevice)) {
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
            if (samplePos < limit) {
                MessageTemplateManager.ShowMessage(902102);
                EB.Debug.LogWarning("EndRecord: too short, {0} < {1}", samplePos, limit);
                audioRecord = null;
                microphoneDevice = string.Empty;
                FusionAudio.ResumeMusic(pausedMusic);
                pausedMusic = string.Empty;
                return;
            }
    
            ChatRule.CHAT_CHANNEL channelType =ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD;
    
            bool privateChannel = false;
    
            string channelStr = ChatRule.CHANNEL2STR[channelType];
            float leftTime = SparxHub.Instance.ChatManager.GetLastSendTime(channelStr) + GetSendInterval(channelType) - Time.realtimeSinceStartup;
            if (leftTime > 0) {
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
    
            if (privateChannel) {
                ChatController.instance.RequestSendChat(channelType, audioRecord, samplePos, ChatController.instance.TargetPrivateUid, ChatController.instance.TargetPrivateName);
            }
            else {
                ChatController.instance.RequestSendChat(channelType, audioRecord, samplePos);
            }
    
            audioRecord = null;
            microphoneDevice = string.Empty;
            FusionAudio.ResumeMusic(pausedMusic);
            pausedMusic = string.Empty;
        }
    }
}
