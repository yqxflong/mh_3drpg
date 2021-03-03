using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{

    [System.Serializable]
    public class ChatItemStruct
    {
        public GameObject Container;
        public UISprite HeroIcon;
        public UISprite FrameIcon;
        public GameObject Audio;
        public UISymbolLabel AudioLabel;
        public UILabel AudioDurationLabel;
        public UISprite AudioUnreadSprite;
        public UISymbolLabel TitleLabel;
        public GameObject Text;
        public UISymbolLabel TextLabel;
        public UISprite TextBGSprite;
        public UILabel LevelLabel;
        public UISprite ChannelSprite;
        public Transform VipTagTf;
        public GameObject[] AudioSpObjs;
    }

    public class ChatItem : DynamicUITableCellController<ChatUIMessage>, IHotfixUpdate
    {
        public static Dictionary<ChatRule.CHAT_CHANNEL, Color> ChannelSpriteColor = new Dictionary<ChatRule.CHAT_CHANNEL, Color>{
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD, LT.Hotfix.Utility.ColorUtility.GreenColor },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE, LT.Hotfix.Utility.ColorUtility.YellowColor },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, LT.Hotfix.Utility.ColorUtility.RedColor },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM, LT.Hotfix.Utility.ColorUtility.BlueColor },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION, new Color32(255,119,47,255) },
    };

        public static Dictionary<ChatRule.CHAT_CHANNEL, string> ChannelName = new Dictionary<ChatRule.CHAT_CHANNEL, string> {
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD,"ID_CHAT_CHANNEL_WORLD" },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE,"ID_ALLIANCE" },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM,"ID_CHAT_CHANNEL_SYSTEM" },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_TEAM,"ID_CHAT_CHANNEL_TEAM" },
        {ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_NATION,"ID_NATION_NAME" }
    };

        public enum ALIGN_TYPE
        {
            LEFT,
            RIGHT,
        }

        private ALIGN_TYPE mAlignType = ALIGN_TYPE.LEFT;

        public ChatItemStruct leftChatItemStruct;
        public ChatItemStruct rightChatItemStruct;

        public ChatHudController _chatHud;

        public ChatHudController chatHud
        {
            get
            {
                if (_chatHud == null)
                {
                    if (mDMono.transform.parent.parent.parent.parent.parent.parent.parent.parent .GetComponent<UIControllerILR>() != null)
                    {
                        _chatHud = mDMono.transform.parent.parent.parent.parent.parent.parent.parent.parent .GetUIControllerILRComponent<ChatHudController>();
                    }
                }

                return _chatHud;
            }
        }
        public FriendChatController FriendChatHud;
        private ChatItemStruct curChatItemStruct;
        private bool isPlayAudio;

        #region show
        private GameObject UiContainer
        {
            get { return curChatItemStruct.Container; }
        }
        private UISymbolLabel titleLabel
        {
            get { return curChatItemStruct.TitleLabel; }
        }

        private UISymbolLabel textLabel
        {
            get { return curChatItemStruct.TextLabel; }
        }
        private UISprite textBGSprite
        {
            get { return curChatItemStruct.TextBGSprite; }
        }
        private UISymbolLabel audioLabel
        {
            get { return curChatItemStruct.AudioLabel; }
        }
        private UILabel audioDurationLabel
        {
            get { return curChatItemStruct.AudioDurationLabel; }
        }
        private UISprite audioUnreadSprite
        {
            get { return curChatItemStruct.AudioUnreadSprite; }
        }
        private UISprite heroIcon
        {
            get { return curChatItemStruct.HeroIcon; }
        }
        private UISprite frameIcon
        {
            get { return curChatItemStruct.FrameIcon; }
        }
        private GameObject audioContainer
        {
            get { return curChatItemStruct.Audio; }
        }
        private GameObject textContainer
        {
            get { return curChatItemStruct.Text; }
        }

        private Transform vipTagTf
        {
            get { return curChatItemStruct.VipTagTf; }
        }

        private UILabel levelLabel
        {
            get { return curChatItemStruct.LevelLabel; }
        }

        private UISprite ChannelSprite
        {
            get { return curChatItemStruct.ChannelSprite; }
        }

        private GameObject[] AudioSpObjs
        {
            get { return curChatItemStruct.AudioSpObjs; }
        }
        #endregion

        private ChatUIMessage mItemData;

        private float playAudioTime;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            leftChatItemStruct = new ChatItemStruct();
            leftChatItemStruct.Container = t.Find("LeftContainer").gameObject;
            if (t.Find("LeftContainer/HeroPortraitTemplate/Icon") != null)
            {
                leftChatItemStruct.HeroIcon = t.GetComponent<UISprite>("LeftContainer/HeroPortraitTemplate/Icon");
                leftChatItemStruct.FrameIcon = t.GetComponent<UISprite>("LeftContainer/HeroPortraitTemplate/Icon/Frame");
            }
            leftChatItemStruct.Audio = t.Find("LeftContainer/Audio").gameObject;
            leftChatItemStruct.AudioLabel = t.GetComponent<UISymbolLabel>("LeftContainer/Audio/AudioLabel");
            leftChatItemStruct.Audio.transform.GetComponent<UIButton>("AudioBackground").onClick.Add(new EventDelegate (OnAudioClick));

            leftChatItemStruct.AudioDurationLabel = t.GetComponent<UILabel>("LeftContainer/Audio/AudioBackground/DurationLabel");
            leftChatItemStruct.AudioUnreadSprite = t.GetComponent<UISprite>("LeftContainer/Audio/AudioBackground/DurationLabel/UnReadSprite");
            leftChatItemStruct.TitleLabel = t.GetComponent<UISymbolLabel>("LeftContainer/TitleLabel");
            leftChatItemStruct.Text = t.Find("LeftContainer/Text").gameObject;
            leftChatItemStruct.TextLabel = t.GetComponent<UISymbolLabel>("LeftContainer/Text/TextLabel");
            leftChatItemStruct.TextBGSprite = t.GetComponent<UISprite>("LeftContainer/Text/TextBackground");
            if (t.Find("LeftContainer/HeroPortraitTemplate/Level/Label") != null)
            {
                t.GetComponent<UIButton>("LeftContainer/HeroPortraitTemplate").onClick.Add(new EventDelegate(ShowOtherPlayerInfo));
                leftChatItemStruct.LevelLabel = t.GetComponent<UILabel>("LeftContainer/HeroPortraitTemplate/Level/Label");
                leftChatItemStruct.ChannelSprite = t.GetComponent<UISprite>("LeftContainer/ChannelSprite");
                leftChatItemStruct.VipTagTf = t.Find("LeftContainer/VipObj");

                leftChatItemStruct.AudioSpObjs = new GameObject[3];
                leftChatItemStruct.AudioSpObjs[0] = t.Find("LeftContainer/Audio/AudioBackground/AudioSpObj/AudioSprite").gameObject;
                leftChatItemStruct.AudioSpObjs[1] = t.Find("LeftContainer/Audio/AudioBackground/AudioSpObj/AudioSprite (1)").gameObject;
                leftChatItemStruct.AudioSpObjs[2] = t.Find("LeftContainer/Audio/AudioBackground/AudioSpObj/AudioSprite (2)").gameObject;
            }


            rightChatItemStruct = new ChatItemStruct();
            rightChatItemStruct.Container = t.Find("RightContainer").gameObject;
            if (t.Find("RightContainer/HeroPortraitTemplate/Icon") != null)
            {
                rightChatItemStruct.HeroIcon = t.GetComponent<UISprite>("RightContainer/HeroPortraitTemplate/Icon");
                rightChatItemStruct.FrameIcon = t.GetComponent<UISprite>("RightContainer/HeroPortraitTemplate/Icon/Frame");
            }
            rightChatItemStruct.Audio = t.Find("RightContainer/Audio").gameObject;
            rightChatItemStruct.AudioLabel = t.GetComponent<UISymbolLabel>("RightContainer/Audio/AudioLabel");
            rightChatItemStruct.Audio.transform.GetComponent<UIButton>("AudioBackground").onClick.Add(new EventDelegate(OnAudioClick));

            rightChatItemStruct.AudioDurationLabel = t.GetComponent<UILabel>("RightContainer/Audio/AudioBackground/DurationLabel");
            rightChatItemStruct.AudioUnreadSprite = t.GetComponent<UISprite>("RightContainer/Audio/AudioBackground/DurationLabel/UnReadSprite");
            rightChatItemStruct.TitleLabel = t.GetComponent<UISymbolLabel>("RightContainer/TitleLabel");
            rightChatItemStruct.Text = t.Find("RightContainer/Text").gameObject;
            rightChatItemStruct.TextLabel = t.GetComponent<UISymbolLabel>("RightContainer/Text/TextLabel");
            rightChatItemStruct.TextBGSprite = t.GetComponent<UISprite>("RightContainer/Text/TextBackground");
            if (t.Find("RightContainer/HeroPortraitTemplate/Level/Label") != null)
            {
                rightChatItemStruct.LevelLabel = t.Find("RightContainer/HeroPortraitTemplate/Level/Label")
                    .GetComponent<UILabel>();
                rightChatItemStruct.ChannelSprite = t.Find("RightContainer/ChannelSprite")
                    .GetComponent<UISprite>();
                rightChatItemStruct.VipTagTf = t.Find("RightContainer/VipObj");

                rightChatItemStruct.AudioSpObjs = new GameObject[3];
                rightChatItemStruct.AudioSpObjs[0] = t.Find("RightContainer/Audio/AudioBackground/AudioSpObj/AudioSprite").gameObject;
                rightChatItemStruct.AudioSpObjs[1] = t.Find("RightContainer/Audio/AudioBackground/AudioSpObj/AudioSprite (1)").gameObject;
                rightChatItemStruct.AudioSpObjs[2] = t.Find("RightContainer/Audio/AudioBackground/AudioSpObj/AudioSprite (2)").gameObject;
            }


            if (t.parent.parent.parent.parent.GetComponent<DynamicMonoILR>() != null)
            {
                FriendChatHud = t.parent.parent.parent.parent.GetMonoILRComponent<FriendChatController>();
            }
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
            if (isPlayAudio)ResetPlayAudioState();
        }
        public void Update()
        {
            if (isPlayAudio)
            {
                if (playAudioTime - (int)playAudioTime < 0.33)
                {
                    SetAudioSpObjActive(0);
                }
                else if (playAudioTime - (int)playAudioTime < 0.66)
                {
                    SetAudioSpObjActive(1);
                }
                else if (playAudioTime - (int)playAudioTime < 1)
                {
                    SetAudioSpObjActive(2);
                }
                playAudioTime += Time.deltaTime;
            }
        }

        public void SetItemData(object _data)
        {
            mItemData = _data as ChatUIMessage;

            if (mItemData == null)
            {
                Clean();
            }
            else
            {
                Align();
                FreshItem();
            }
        }

        public override void Fill(ChatUIMessage itemData)
        {
            mItemData = itemData;

            if (mItemData == null)
            {
                Clean();
            }
            else
            {
                Align();
                FreshItem();
            }
        }

        public override void Clean()
        {
            if (curChatItemStruct == null)
            {
                curChatItemStruct = leftChatItemStruct;
            }
            leftChatItemStruct.TitleLabel.text = null;
            rightChatItemStruct.TitleLabel.text = null;
            leftChatItemStruct.TextLabel.text = null;
            rightChatItemStruct.TextLabel.text = null;
            leftChatItemStruct.AudioLabel.text = null;
            rightChatItemStruct.AudioLabel.text = null;
            leftChatItemStruct.AudioDurationLabel.text = null;
            rightChatItemStruct.AudioDurationLabel.text = null;
            if (heroIcon != null)
            {
                heroIcon.spriteName = "";
                frameIcon.spriteName = "";
            }

            mAlignType = ALIGN_TYPE.LEFT;
            mItemData = null;
        }

        private void Align()
        {
            if (LoginManager.Instance.LocalUser.Id.Value == mItemData.Message.uid)
            {
                mAlignType = ALIGN_TYPE.RIGHT;
                curChatItemStruct = rightChatItemStruct;
            }
            else
            {
                mAlignType = ALIGN_TYPE.LEFT;
                curChatItemStruct = leftChatItemStruct;
            }
        }

        private void FreshItem()
        {
            ShowLeftOrRight();
            
            if (heroIcon != null)
            {
                heroIcon.spriteName = string.Format("{0}", mItemData.Message.icon);
                frameIcon.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(mItemData.Message.frame).iconId;
            }
            titleLabel.text = mItemData.Message.name;

            if (ChannelSprite != null)
            {
                ChannelSprite.color = ChannelSpriteColor[mItemData.Channel];
                ChannelSprite.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString(ChannelName[mItemData.Channel]);
            }

            if (levelLabel != null)
            {
                levelLabel.text = mItemData.GetLevel().ToString();
            }
            ShowAudioOrText();
            SetVipTag();
            mDMono.gameObject.CustomSetActive(false);
            mDMono.gameObject.CustomSetActive(true);

            if (mDMono.gameObject.activeInHierarchy)
            {
                StartCoroutine(SetHeight());
            }
        }

        private void ShowLeftOrRight()
        {
            if (mAlignType == ALIGN_TYPE.LEFT)
            {
                leftChatItemStruct.Container.CustomSetActive(true);
                rightChatItemStruct.Container.CustomSetActive(false);
            }
            else
            {
                leftChatItemStruct.Container.CustomSetActive(false);
                rightChatItemStruct.Container.CustomSetActive(true);
            }
        }

        private void ShowAudioOrText()
        {
            if (mItemData.Message.isAudio)
            {
                audioContainer.CustomSetActive(true);
                textContainer.CustomSetActive(false);

                audioLabel.text = mAlignType == ALIGN_TYPE.LEFT ? mItemData.GetLeftAudioString() : mItemData.GetRightAudioString();
                audioDurationLabel.text = mItemData.GetAudioDurationString();
                if (FriendChatHud == null)
                {
                    audioUnreadSprite.gameObject.CustomSetActive(!FriendManager.Instance.AcHistory.GetAudioIsRead(mItemData.Message.uid, mItemData.Message.ts));
                }
                else
                {
                    audioUnreadSprite.gameObject.CustomSetActive(!FriendManager.Instance.ChatHistory.GetAudioIsRead(mItemData.Message.uid, mItemData.Message.ts));
                }
            }
            else
            {
                audioContainer.CustomSetActive(false);
                textContainer.CustomSetActive(true);

                textLabel.text = mItemData.GetText();

                if (mItemData.Message.channelType.CompareTo("private") == 0)
                {
                    SetText();
                }
            }
        }

        private void SetText()
        {
            int w = titleLabel.width - textLabel.width;
            while (w > 0)
            {
                mItemData.Message.text = string.Format("{0} ", mItemData.Message.text);
                w -= 7;
            }
            textLabel.text = mItemData.GetText();
        }

        private void SetVipTag()
        {
            if (vipTagTf == null)
            {
                return;
            }

            if (mItemData.Message.monthVipType <= 0)
            {
                vipTagTf.gameObject.CustomSetActive(false);
                return;
            }
            vipTagTf.gameObject.CustomSetActive(true);

            for (int i = 0; i < vipTagTf.childCount; i++)
            {
                vipTagTf.GetChild(i).gameObject.CustomSetActive(i == mItemData.Message.monthVipType - 1);
            }
        }

        public void OnAudioClick()
        {
            if (mItemData.Message.isAudio && mItemData.Message.audioClip != null)
            {
                mItemData.Message.isRead = true;
                audioUnreadSprite.gameObject.CustomSetActive(!mItemData.Message.isRead);

                if (chatHud != null)
                {
                    FriendManager.Instance.AcHistory.ReadAudio(mItemData.Message.uid, mItemData.Message.ts);
                    chatHud.PlayAudio(mItemData.Message.audioClip);

                    if (chatHud.CurPlayAudioItem != null)
                    {
                        chatHud.CurPlayAudioItem.ResetPlayAudioState();
                    }

                    isPlayAudio = true;
                    playAudioTime = 0;
                    chatHud.CurPlayAudioItem = this;
                }
                else if (FriendChatHud != null)
                {
                    FriendManager.Instance.ChatHistory.ReadAudio(mItemData.Message.uid, mItemData.Message.ts);
                    FriendChatHud.PlayAudio(mItemData.Message.audioClip);
                }
            }
        }

        public void ShowOtherPlayerInfo()
        {
            if (mItemData.Message.uid == LoginManager.Instance.LocalUser.Id.Value)
            {
                return;
            }

            if (!SceneLogicManager.isMainlands())
            {
                return;
            }

            if (mItemData.Message.uid <= 0)
            {
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add("uid", mItemData.Message.uid);
            ht.Add("icon", string.Format("{0}", mItemData.Message.icon));
            ht.Add("headFrame", mItemData.Message.frame);
            ht.Add("name", mItemData.Message.name);
            ht.Add("level", mItemData.Message.level);
            ht.Add("fight", mItemData.Message.battleRating);
            ht.Add("a_name", mItemData.Message.allianceName);
            GlobalMenuManager.Instance.Open("OtherPlayerInfoView", ht);
        }

        private void SetAudioSpObjActive(int index)
        {
            for (int i = 0; i < AudioSpObjs.Length; i++)
            {
                AudioSpObjs[i].CustomSetActive(i == index);
            }
        }

        public void ResetPlayAudioState()
        {
            isPlayAudio = false;
            SetAudioSpObjActive(2);
        }
        protected override IEnumerator SetHeight()
        {
            yield return new WaitForFixedUpdate();
            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(mDMono.transform);
            Height = Mathf.CeilToInt(bounds.size.y);
            if (RefreshHeightEvent != null)
            {
                RefreshHeightEvent(Height);
                RefreshHeightEvent = null;
            }
            //EB.Debug.Log("ViewIndex : " + ViewIndex + "   Height : " + Height + "    Index:" + DataIndex);
            yield break;
        }
    }

}