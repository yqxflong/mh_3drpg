using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTGameSettingController : UIControllerHotfix, IHotfixUpdate
    {
        private List<UIToggle> toggleList;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            HeroIcon = t.GetComponent<UISprite>("View/InfoView/Head/Icon");
            UIDLabel = t.GetComponent<UILabel>("View/InfoView/Head/UIDLabel");
            AllianceLabel = t.GetComponent<UILabel>("View/InfoView/Head/AllianceLabel");
            ExpLabel = t.GetComponent<UILabel>("View/InfoView/Level/ProgressBar/Label");
            XpSlider = t.GetComponent<UISlider>("View/InfoView/Level/ProgressBar");
            PlayerNumLabel = t.GetComponent<UILabel>("View/SettingView/Placeholder/ProgressBar/Thumb/Label");
            PlayerNumSlider = t.GetComponent<UISlider>("View/SettingView/Placeholder/ProgressBar");
            PlayerNumSlider.onChange.Add(new EventDelegate(()=>OnPlayerNumSlideValueChanged(PlayerNumSlider.value)));
            BGMGroup = t.GetComponent<ToggleGroupState>("View/SettingView/Placeholder/VoiceList/BGM/ValueBtnList");        
            for (int i = 0; i < BGMGroup.m_Toggles.Count; i++)
            {
                Transform tr = BGMGroup.transform.Find(i.ToString());
                tr.GetComponent<UIToggle>().onChange.Add(new EventDelegate(() => OnBGMToggleClick(tr)));
            }
            VoiceGroup = t.GetComponent<ToggleGroupState>("View/SettingView/Placeholder/VoiceList/Voice/ValueBtnList");
            for (int i = 0; i < VoiceGroup.m_Toggles.Count; i++)
            {
                Transform tr = VoiceGroup.transform.Find(i.ToString());
                tr.GetComponent<UIToggle>().onChange.Add(new EventDelegate(() => OnVoiceToggleClick(tr)));
            }
            //PushGrid = t.GetMonoILRComponent<LTPushMsgScroll>("View/SettingView/Placeholder/PushMessage");
            QualityGroup = t.GetComponent<ToggleGroupState>("View/SettingView/Placeholder/GameQuality/Grid");
            t.GetComponent<UIToggle>("View/SettingView/Placeholder/GameQuality/Grid/High").onChange.Add(new EventDelegate(()=>OnQualityTogleClick(QualityGroup.transform.Find("High"))));
            t.GetComponent<UIToggle>("View/SettingView/Placeholder/GameQuality/Grid/Medium").onChange.Add(new EventDelegate(() => OnQualityTogleClick(QualityGroup.transform.Find("Medium"))));
            t.GetComponent<UIToggle>("View/SettingView/Placeholder/GameQuality/Grid/Low").onChange.Add(new EventDelegate(() => OnQualityTogleClick(QualityGroup.transform.Find("Low"))));
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseButton");
            t.GetComponent<UIButton>("View/InfoView/Head/NameLabel/NameBtn").onClick.Add(new EventDelegate(ChangeNickNameBtnClick));
            t.GetComponent<UIButtonText>("View/InfoView/ButtonGrid/GiftButton").onClick.Add(new EventDelegate(OnGiftBtnClick));
            t.GetComponent<UIButtonText>("View/InfoView/ButtonGrid/ExitButton").onClick.Add(new EventDelegate(OnExitBtnClick));
            SetExchangecodeVisible();
            //留后台给人可以传回主城
            BoxCollider collider = HeroIcon.gameObject.AddComponent<BoxCollider>();
            collider.size = HeroIcon.localSize;
            UIEventListener.Get(HeroIcon.gameObject).onClick = OnClickHeadIcon;
            //
            m_LanguageSetting = new LTLanguageSetting(controller.transform.Find("View/LanguageView"), OnCancelButtonClick);

            FrameRedPoint=t.Find("ButtonList/2_Other/OtherSetting/RedPoint").gameObject;
        }


    
        public override bool ShowUIBlocker { get { return true; } }
        //InfoView
        public UISprite HeroIcon;
        public UILabel UIDLabel, AllianceLabel,ExpLabel;//ServerLbael
        public UISlider XpSlider;
    
        //SettingView
        public UILabel PlayerNumLabel;
        public UISlider PlayerNumSlider;
        public ToggleGroupState BGMGroup;
        public ToggleGroupState VoiceGroup;
        //public LTPushMsgScroll PushGrid;//推送一直没生效，先去掉先
        public ToggleGroupState QualityGroup;

        //OtherView
        public GameObject FrameRedPoint;

        /// <summary>
        /// 多语言设置
        /// </summary>
        private LTLanguageSetting m_LanguageSetting;
    
        public override IEnumerator OnAddToStack()
        {
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.otherSetting, FrameRPFunc);
            yield return base.OnAddToStack();
            if (HeadFrameEvent.InitEvent != null)
            {
                HeadFrameEvent.InitEvent();
            }
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.otherSetting, FrameRPFunc);
            DestroySelf();
            yield break;
        }

        public void FrameRPFunc(RedPointNode node)
        {
            FrameRedPoint.CustomSetActive(node.num>0);
        }
        
        public override void OnFocus()
        {
            base.OnFocus();
            ShowInfoUI();
            ShowSettingUI();
        }
    
        public override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick();
    
            if (IsHaveSettingPlayerNum)
            {
                int playerNum = int.Parse(PlayerNumLabel.text);
                PlayerManagerForFilter.Instance.MaxManagedPlayer = playerNum;
                global::UserData.PlayerNum = playerNum;
            }
            UserData.SetPushMsgPrefs();
            global::UserData.SerializePrefs();
        }
    
        private bool IsHaveSettingPlayerNum = false;
        private bool IsFirstPlayerNumValueChanged = true;
        private int curSize = 0;
        public void OnPlayerNumSlideValueChanged(float value)
        {
            if (IsFirstPlayerNumValueChanged)
            {
                IsFirstPlayerNumValueChanged = false;
                return;
            }
            IsHaveSettingPlayerNum = true;
            if (curSize != (int)(value * 30))
            {
                FusionAudio.PostEvent("UI/New/DaoShu");
                curSize = (int)(value * 30);
                PlayerNumLabel.text = PlayerNumLabel.transform.GetChild(0).GetComponent<UILabel>().text = curSize.ToString();
            }
        }
    
        /// <summary>
        /// 个人信息
        /// </summary>
        #region 设置个人信息
        public void ShowInfoUI()
        {
            HeroIcon.spriteName = LTMainHudManager.Instance.UserHeadIcon;
            UIDLabel.text = string.Format("UID:{0}", LoginManager.Instance.LocalUserId.Value);
            SetAllianceName();
            SetExp();
        }
    
        private void SetExp()
        {
            int curXp = 0;
            DataLookupsCache.Instance.SearchIntByID("res.xp.v", out curXp);
            int level = 0;
            DataLookupsCache.Instance.SearchIntByID("level", out level);
    
            Hotfix_LT.Data.PlayerLevelTemplate thisInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetPlayerLevelInfo(level);
            curXp -= thisInfo.expRequirement;
            Hotfix_LT.Data.PlayerLevelTemplate nextInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetPlayerLevelInfo(level+1);
            int maxXp = nextInfo.expRequirement- thisInfo.expRequirement;
            XpSlider.value = maxXp != 0 ? ((float)curXp / (float)maxXp) : 0;
    
            ExpLabel.text = ExpLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}/{1}", curXp, maxXp);
        }

        public static string GetPlayerName()
        {
            string playerName;

            if (!DataLookupsCache.Instance.SearchDataByID<string>("user.name", out playerName))
            {
                playerName = string.Empty;
            }

            return playerName;
        }

        private void SetAllianceName()
        {
            string tempName;
            tempName = GetAllianceName();
            AllianceLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTGameSettingController_3045"), tempName);
        }

        private string GetAllianceName()
        {
            if (AlliancesManager.Instance.Account.State == eAllianceState.Joined)
                return AlliancesManager.Instance.Detail.Name;
            else
                return EB.Localizer.GetString("ID_NOT_JOIN_ALLIANCE");
        }
    
        private KeyValuePair<float,int> m_ClickRecord = new KeyValuePair<float, int>(0,0);
        /// <summary>
        /// 点击头像
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickHeadIcon(GameObject obj)
        {
            if (m_ClickRecord.Value == 0)
            {
                m_ClickRecord = new KeyValuePair<float, int>(Time.time, 1);
            }
            else
            {
                m_ClickRecord = new KeyValuePair<float, int>(m_ClickRecord.Key, m_ClickRecord.Value + 1);
            }
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            //人物拉回
            if (m_ClickRecord.Key > 0)
            {
                if (Time.time - m_ClickRecord.Key <= 0.6f)
                {
                    if (m_ClickRecord.Value >= 3)
                    {
                        //将人物拉回原点，留后门防止人都卡在一个地方
                        PlayerController player = PlayerManager.LocalPlayerController();
                        EB.Debug.Log(string.Format("[{0}]当前的人物坐标,Position:{1}", Time.frameCount, player.transform.position));
                        player.transform.position = SceneManager.HeroStart;
                        m_ClickRecord = new KeyValuePair<float, int>(0, 0);
                        EB.Debug.Log(string.Format("被拉后的人物坐标,Position:{0}", player.transform.position));
                    }
                }
                else
                {
                    m_ClickRecord = new KeyValuePair<float, int>(0, 0);
                }
            }
        }
        #endregion
    
    
        /// <summary>
        /// 游戏设置
        /// </summary>
        #region 设置游戏设置
        public void ShowSettingUI()
        {
            SetPlayerNum();
            SetVoiceValue();
            SetQualityValue();
            //PushGrid.SetItemDatas( LTDailyDataManager.Instance.GetDailyDataByDailyType(EDailyType.Limit));
        }
    
        private void SetPlayerNum()
        {
            curSize = global::UserData.PlayerNum;
            PlayerNumLabel.text = PlayerNumLabel.transform.GetChild (0).GetComponent <UILabel>().text = curSize.ToString();
            PlayerNumSlider.value = curSize / 30f;
        }
        private void SetVoiceValue()
        {
            float value = global::UserData.MusicVolume;
            float interval = 1f / (float)(BGMGroup.m_Toggles.Count - 1);
            float index = value / interval;
            for (int i=0;i<BGMGroup.m_Toggles.Count; i++)
            {
                if((int)index ==i)BGMGroup.m_Toggles[i].Set(true);
            }
    
            value = global::UserData.SFXVolume;
            interval = 1f / (float)(VoiceGroup.m_Toggles.Count - 1);
            index = value / interval;
            for (int i = 0; i < VoiceGroup.m_Toggles.Count; i++)
            {
                if ((int)index == i) VoiceGroup.m_Toggles[i].Set(true);
            }
        }
        private void SetQualityValue()
        {
            int index = QualitySettings.GetQualityLevel();// GetQualityLevel(UserData.UserQualitySet);
            for (int i = 0; i < QualityGroup.m_Toggles.Count; i++)
            {
                if (index == i) QualityGroup.m_Toggles[i].Set(true);
            }
        }
        private static int GetQualityLevel(string quality)
        {
            switch (quality)
            {
                case "High": return 0;
                case "Medium": return 1;
                case "Low": return 2;
                default: return 0;
            }
        }
        private static string GetQualityLevel(int quality)
        {
            switch (quality)
            {
                case 0: return "High";
                case 1: return "Medium";
                case 2: return "Low";
                default: return "High";
            }
        }
    
        public void OnBGMToggleClick(Transform toggle)
        {
            if (!toggle.GetComponent <UIToggle>().value) return;
            float interval = 1f / (float)(BGMGroup.m_Toggles.Count - 1);
            float index = float.Parse(toggle.name);
            float value = interval * index;
            global::UserData.MusicVolume = value;
            global::UserData.SerializePrefs();
            AudioManager.Instance.SetDeltaVolume(AudioManager.eSoundFolders.Ambience| AudioManager.eSoundFolders.Music | AudioManager.eSoundFolders.BGM, value);
        }
        public void OnVoiceToggleClick(Transform toggle)
        {
            if (!toggle.GetComponent<UIToggle>().value) return;
            float interval = 1f / (float)(VoiceGroup.m_Toggles.Count - 1);
            float index = float.Parse(toggle.name);
            float value = interval * index;
            global::UserData.SFXVolume = value;
            global::UserData.SerializePrefs();
            AudioManager.Instance.SetDeltaVolume(AudioManager.eSoundFolders.UI | AudioManager.eSoundFolders.SFX, value);
            FusionAudio.PostEvent("UI/General/ButtonClick");
        }
        public void OnQualityTogleClick(Transform toggle)
        {
            if (!toggle.GetComponent<UIToggle>().value) return;
            string level = toggle.name;
            SetGameQualityLevel(level);
        }
    
        public static void SetGameQualityLevel(string level)
        {
            int levelInt = GetQualityLevel(level);
            EB.Sparx.PerformanceManager.PerformanceUserSetting = level;
            global::UserData.UserQualitySet = level;
            global::UserData.SerializePrefs();
            EB.Sparx.Hub.Instance.PerformanceManager.ResetPerformanceData(
                delegate
                {
                    global::UserData.SetUserQuality(levelInt);
                    var perfInfo = PerformanceManager.Instance.CurrentEnvironmentInfo;
                    QualitySettings.SetQualityLevel(levelInt, true);
                    Shader.globalMaximumLOD = perfInfo.lod;// default: 0
                    QualitySettings.blendWeights = BlendWeights.FourBones;//by:wwh 强制调整为4融合权重
                    QualitySettings.antiAliasing = (int)perfInfo.msaa;// default: disabled
                    QualitySettings.anisotropicFiltering = (AnisotropicFiltering)perfInfo.aniso;// default: disabled
                });
        }
        public static void SetGameQualityLevel(int level)
        {
            SetGameQualityLevel(level.ToString());
        }
    
        #endregion
    
        private void SetExchangecodeVisible()
        {
            Transform inputExchangeCodeBtn = controller.transform.Find("View/InfoView/ButtonGrid/GiftButton");
            bool isOpen = true;
            if (!DataLookupsCache.Instance.SearchDataByID<bool>("isOpenEC", out isOpen))
                EB.Debug.LogError("isOpenEC not found");
            inputExchangeCodeBtn.gameObject.SetActive(isOpen);
        }
    
        public void ChangeNickNameBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTChangeNickNameView");
        }
    
        public void OnExitBtnClick()
        {
            string content = EB.Localizer.GetString("ID_codefont_in_LTGameSettingController_8789");
            MessageDialog.Show(EB.Localizer.GetString("ID_LOGOUT"), content, EB.Localizer.GetString(MessageTemplate.OkBtn), EB.Localizer.GetString(MessageTemplate.CancelBtn), false, false, false, delegate (int result)
            {
                if (result == 0)
                {
                    SparxHub.Instance.Logout();

                    if (FriendManager.Instance != null && FriendManager.Instance.AcHistory != null)
                    {
                        FriendManager.Instance.AcHistory.ClearAllChatList();
                    }
                   
                    if(ILRDefine.USE_XINKUAISDK)
                        return;// SparxHub.Instance.Logout();已去执行相关操作，会有回调，因此不需要再执行SparxHub.Instance.Disconnect(true)不然会有问题

                    SparxHub.Instance.Disconnect(true);
                }
            }, NGUIText.Alignment.Center);
        }
    
        public void OnGiftBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTExchangeCodeView");
        }
    }
}
