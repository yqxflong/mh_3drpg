using System.Collections;
using UnityEngine;
namespace Hotfix_LT.UI
{

    /// <summary>
    /// 天梯UI界面
    /// </summary>
    public class LTPVPBattleHudController : LTHeroBattleHudController
    {

        /// <summary>
        /// 卡片备选面板
        /// </summary>
        private LTPVPPrepareCardHud m_PrepareCardPanel;
        /// <summary>
        /// 队伍阵型面板
        /// </summary>
        private LTPVPTeamHud[] m_TeamPlane;
        /// <summary>
        /// 当前战斗数据
        /// </summary>
        private HeroBattleChoiceData m_HeroBattleChoiceData;
        /// <summary>
        /// 确认按钮
        /// </summary>
        private GameObject m_ConfirmBtn;
        /// <summary>
        /// 确认按钮文本
        /// </summary>
        private UILabel m_ConfirmLabel;
        /// <summary>
        /// 段位信息
        /// </summary>
        private UILevelInfo[] m_LevelInfo;

        /// <summary>
        /// 托管按钮
        /// </summary>
        private GameObject TrusteeshipBtn;
        /// <summary>
        /// 托管按钮选择
        /// </summary>
        private GameObject TrusteeshipOpenSprite;

        public override void Awake()
        {
            base.Awake();
            InitView();
            m_ConfirmBtn = controller.transform.Find("CenterHold/Confirm_Button").gameObject;
            UIEventListener.Get(m_ConfirmBtn).onClick = OnClickConformBtn;
            m_ConfirmLabel = controller.transform.Find("CenterHold/Confirm_Button/Label").GetComponent<UILabel>();
            m_PrepareCardPanel = new LTPVPPrepareCardHud(controller.transform.Find("Bottom"));
            //
            m_TeamPlane = new LTPVPTeamHud[2];
            m_TeamPlane[0] = new LTPVPTeamHud(controller.transform.Find("LeftHold"), true);
            m_TeamPlane[1] = new LTPVPTeamHud(controller.transform.Find("RightHold"), false);
            //
            m_LevelInfo = new UILevelInfo[2];
            m_LevelInfo[0] = new UILevelInfo(controller.transform.Find("LeftTopHold/Level"));
            m_LevelInfo[1] = new UILevelInfo(controller.transform.Find("RightTopHold/Level"));
        }

        private void InitView()
        {
            var t = controller.transform;
            vsTexture = t.GetComponent<UITexture>("VsTexture");
            myPointLabel = t.GetComponent<UILabel>("LeftTopHold/Jifei/ProgressBar/Label");
            otherPointLabel = t.GetComponent<UILabel>("RightTopHold/Jifei (1)/ProgressBar/Label");
            myLeftPointBar = t.GetComponent<UIProgressBar>("LeftTopHold/Jifei/ProgressBar");
            otherLeftPointBar = t.GetComponent<UIProgressBar>("RightTopHold/Jifei (1)/ProgressBar");
            timeLabel = t.GetComponent<UILabel>("TopCenterHold/Time_Label");
            startTimeLabel = t.GetComponent<UILabel>("CenterHold/StartTime_Label");
            choiceStateTipsLabel = t.GetComponent<UILabel>("TopCenterHold/ChoiceState_Label");
            turnGO = t.FindEx("TopCenterHold/YourTurn").GetComponent<UIWidget>();
            otherTurnGO = t.FindEx("TopCenterHold/DifangTurn").GetComponent<UIWidget>();
            changeTurnTweenTime = 1.8f;
            selfNameLabel = t.GetComponent<UILabel>("LeftTopHold/YourName_Label");
            OtherNameLabel = t.GetComponent<UILabel>("RightTopHold/YourName_Label (1)");
            selfLevelLabel = t.GetComponent<UILabel>("LeftTopHold/LTPlayerPortrait/TargetLevelBG/Level");
            otherLevelLabel = t.GetComponent<UILabel>("RightTopHold/LTPlayerPortrait (1)/TargetLevelBG/Level");
            selfHeadIconSpt = t.GetComponent<UISprite>("LeftTopHold/LTPlayerPortrait/Icon");
            otherHeadIconSpt = t.GetComponent<UISprite>("RightTopHold/LTPlayerPortrait (1)/Icon");
            selfFrameIconSpt = t.GetComponent<UISprite>("LeftTopHold/LTPlayerPortrait/Icon/Frame");
            otherFrameIconSpt = t.GetComponent<UISprite>("RightTopHold/LTPlayerPortrait (1)/Icon/Frame");
            myModelShadow = t.FindEx("MyModelShadow").gameObject;
            otherModelShadow = t.FindEx("OtherModelShadow").gameObject;
            canChangeMyModel = true;
            canChangeOtherModel = true;
            t.GetComponent<UIButton>("LeftTopHold/CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("LeftTopHold/AdmitDefeatBtn").onClick.Add(new EventDelegate(OnAdmitDefeatBtnClick));

            BattleReadyTitle battleReadyTitle = t.GetMonoILRComponent<BattleReadyTitle>("Bottom/BG/Title");
            t.GetComponent<UIButton>("Bottom/BG/Title/BtnList/AllBtn").onClick.Add(new EventDelegate(() => battleReadyTitle.OnTitleBtnClick(t.Find("Bottom/BG/Title/BtnList/AllBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("Bottom/BG/Title/BtnList/FengBtn").onClick.Add(new EventDelegate(() => battleReadyTitle.OnTitleBtnClick(t.Find("Bottom/BG/Title/BtnList/FengBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("Bottom/BG/Title/BtnList/HuoBtn").onClick.Add(new EventDelegate(() => battleReadyTitle.OnTitleBtnClick(t.Find("Bottom/BG/Title/BtnList/HuoBtn/Sprite").gameObject)));
            t.GetComponent<UIButton>("Bottom/BG/Title/BtnList/ShuiBtn").onClick.Add(new EventDelegate(() => battleReadyTitle.OnTitleBtnClick(t.Find("Bottom/BG/Title/BtnList/ShuiBtn/Sprite").gameObject)));

            t.GetComponent<TweenScale>("Bottom/BG/Title/BtnList/AllBtn/Sprite").onFinished.Add(new EventDelegate(() => battleReadyTitle.OnFinishShow(t.Find("Bottom/BG/Title/BtnList/AllBtn/Sprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Bottom/BG/Title/BtnList/FengBtn/Sprite").onFinished.Add(new EventDelegate(() => battleReadyTitle.OnFinishShow(t.Find("Bottom/BG/Title/BtnList/FengBtn/Sprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Bottom/BG/Title/BtnList/HuoBtn/Sprite").onFinished.Add(new EventDelegate(() => battleReadyTitle.OnFinishShow(t.Find("Bottom/BG/Title/BtnList/HuoBtn/Sprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Bottom/BG/Title/BtnList/ShuiBtn/Sprite").onFinished.Add(new EventDelegate(() => battleReadyTitle.OnFinishShow(t.Find("Bottom/BG/Title/BtnList/ShuiBtn/Sprite/Sprite (1)").gameObject)));
            
            TrusteeshipBtn = t.Find("CenterHold/TrusteeshipBtn").gameObject;
            TrusteeshipBtn.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnTrusteeshipBtnClick));
            TrusteeshipOpenSprite = t.Find("CenterHold/TrusteeshipBtn/Open").gameObject;
            if (LadderManager.Instance.IsTrusteeship)
            {
                TrusteeshipOpenSprite.CustomSetActive(true);
            }
        }

        public override IEnumerator OnAddToStack()
        {
            isPVPSelectPartner = true;
            _HeroBattleFinish = false;
            LTHeroBattleEvent.NotifyHeroBattleHudFinish += OnNotifyHeroBattleFinish;
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            isPVPSelectPartner = false;
            LTHeroBattleModel.GetInstance().F_ClearData();
            LTHeroBattleEvent.NotifyHeroBattleHudFinish -= OnNotifyHeroBattleFinish;
            return base.OnRemoveFromStack();
        }
        public static bool isPVPSelectPartner { get; private set; }

        /// <summary>
        /// 设置英雄操作数据
        /// </summary>
        /// <param name="data">操作的数据</param>
        protected override void SetChoiceData(HeroBattleChoiceData data)
        {
            SetPlayerInfo(data.selfInfo, data.otherInfo);

            bool isSelfTurn = (data.openUid == data.selfInfo.uid);
            if ( data.choiceState != 2)
            {
                turnGO.alpha = (isSelfTurn) ? 1 : 0;
                otherTurnGO.alpha = (!isSelfTurn) ? 1 : 0;
				if (_vsLobby != null)
                    _vsLobby.SetTurn(isSelfTurn);
            }

            _choiceState = data.choiceState;

            //设置按钮内容
            m_ConfirmBtn.CustomSetActive(data.choiceState != 2 && isSelfTurn);
            string confirmStr = data.choiceState == 0 ? EB.Localizer.GetString("ID_uifont_in_LTHeroBattleMenu_Label (1)_4") : EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");
            m_ConfirmLabel.text = confirmStr;

            switch (data.choiceState)
            {
                case 0:
                    LTUIUtil.SetText(choiceStateTipsLabel, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleHudController_13617"));
                    break;
                case 1:
                    LTUIUtil.SetText(choiceStateTipsLabel, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleHudController_14889"));
                    break;
                case 2: //非操作模式
                    break;
            }

            if (data.selfChoices != null && data.selfChoices.Count == 6)
            {
                StartChangShowModel(data.selfChoices[data.selfChoices.Count - 1].modelName, data.selfChoices[data.selfChoices.Count - 1].heroName,
                    LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.selfChoices[data.selfChoices.Count - 1].type]);
            }
            else
            {
                StartChangShowModel("", "", "");
            }

            if (data.otherChoices != null && data.otherChoices.Count > 0) //如果有已选用 优先显示最近选用的  ps 这是他人的 自己的由于机制不同 在LTHeroBattleModel处理
            {
                HeroBattleChoiceCellData otherLastChoiceCell = data.otherChoices[data.otherChoices.Count - 1];
                StartChangeOtherShowModel(otherLastChoiceCell.modelName, otherLastChoiceCell.heroName,
                   LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[otherLastChoiceCell.type]);
            }
            else if (data.otherBans != null && data.otherBans.Count > 0) //如果没有已选用 优先显示最近禁用的
            {
                HeroBattleChoiceCellData otherLastChoiceCell = data.otherBans[data.otherBans.Count - 1];
                StartChangeOtherShowModel(otherLastChoiceCell.modelName, otherLastChoiceCell.heroName,
                    LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[otherLastChoiceCell.type]);
            }
            else //都没有久清空显示的模型 
            {
                StartChangeOtherShowModel("", "", "");
            }

            //刷新显示剩余点数
            LTUIUtil.SetText(myPointLabel, data.lessPoint + "/10");
            LTUIUtil.SetText(otherPointLabel, data.otherLessPoint + "/10");
            myLeftPointBar.value = data.lessPoint / 10f;
            otherLeftPointBar.value = data.otherLessPoint / 10f;
            //设置当前的阵容
            m_TeamPlane[0].F_SetTeam(data.selfChoices, data.selfInfo, data.selfBans);
            m_TeamPlane[1].F_SetTeam(data.otherChoices, data.otherInfo, data.otherBans);
            if (data.choiceState != 2)
            {
                StartChangeTurn(isSelfTurn, data.lessTime);
                StartShowTime(data.lessTime);
                //设置高亮队伍表示操作中
                m_TeamPlane[0].F_SetHeightLight((isSelfTurn && data.choiceState == 1) || (!isSelfTurn && data.choiceState == 0), data.selfChoices.Count, data.choiceState == 0);
                m_TeamPlane[1].F_SetHeightLight((!isSelfTurn && data.choiceState == 1) || (isSelfTurn && data.choiceState == 0), data.otherChoices.Count, data.choiceState == 0);
            }

            m_HeroBattleChoiceData = data;
            //
            SetPartnerPanel(data);
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="selfInfo">我的信息</param>
        /// <param name="otherInfo">对手的信息</param>
        private void SetPlayerInfo(SidePlayerInfoData selfInfo, SidePlayerInfoData otherInfo)
        {
            LTUIUtil.SetText(selfNameLabel, selfInfo.name);
            LTUIUtil.SetText(OtherNameLabel, otherInfo.name);
            m_LevelInfo[0].F_SetInfo(selfInfo);
            m_LevelInfo[1].F_SetInfo(otherInfo);
            //
            LTUIUtil.SetText(selfLevelLabel, BalanceResourceUtil.GetUserLevel().ToString() /*data.selfInfo.level.ToString()*/);
            LTUIUtil.SetText(otherLevelLabel, otherInfo.level.ToString());
            selfHeadIconSpt.spriteName = LTMainHudManager.Instance.UserHeadIcon;
            selfFrameIconSpt.spriteName = LTMainHudManager.Instance.UserLeaderHeadFrame.iconId;
            otherHeadIconSpt.spriteName = otherInfo.portrait;
            otherFrameIconSpt.spriteName = otherInfo.frame;
        }

        /// <summary>
        /// 设置伙伴备选面板
        /// </summary>
        /// <param name="data">当前数据</param>
        private void SetPartnerPanel(HeroBattleChoiceData data)
        {
            m_PrepareCardPanel.F_SetCardInfos(data);
        }

        /// <summary>
        /// 点击确认按钮
        /// </summary>
        /// <param name="btn">按钮对象</param>
        private void OnClickConformBtn(GameObject btn)
        {
            if (m_HeroBattleChoiceData == null)
            {
                EB.Debug.LogError("不应该这个时间没有数据，请检验");
                return;
            }

            if (LadderManager.Instance.IsTrusteeshiping()) return;

            //判断当前的操作状态
            switch (m_HeroBattleChoiceData.choiceState)
            {
                case 0:
                    if (LTHeroBattleModel.GetInstance().choiceData.choiceHeroCellData == null)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleHudController_Tips"));
                        return;
                    }
                    if (LTHeroBattleEvent.ConfirmBanHero != null)
                    {
                        FusionAudio.PostEvent("UI/New/JinRen", true);
                        LTHeroBattleEvent.ConfirmBanHero();
                    }
                    break;
                case 1:
                    if (LTHeroBattleModel.GetInstance().choiceData.choiceHeroCellData == null)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleHudController_14889"));
                        return;
                    }
                    if (LTHeroBattleEvent.ConfirmChoiceHero != null)
                    {
                        LTHeroBattleEvent.ConfirmChoiceHero();
                    }
                    break;
                default:
                    EB.Debug.LogError("不应该存在其他情况下的，点击确认按钮情况存在的");
                    break;

            }
        }

        /// <summary>
        /// 托管按钮响应
        /// </summary>
        private void OnTrusteeshipBtnClick()
        {
            LadderManager.Instance.OpenOrCloseTrusteeship(!LadderManager.Instance.IsTrusteeship);
            if (LadderManager.Instance.IsTrusteeship)
            {
                TrusteeshipOpenSprite.CustomSetActive(true);
            }
            else
            {
                TrusteeshipOpenSprite.CustomSetActive(false);
            }
        }

        private bool isRequest = false;
        private bool _HeroBattleFinish = false;
        /// <summary>
        /// 认输按钮响应
        /// </summary>
        private void OnAdmitDefeatBtnClick()
        {
            if (LadderManager.Instance.IsTrusteeshiping()) return;
            if (_HeroBattleFinish)
            {
                return;
            }

            MessageDialog.HideCurrent();
            string content = EB.Localizer.GetString("ID_LADDER_ADMIT_DEFEAT_TIP");
            MessageDialog.Show(EB.Localizer.GetString(MessageTemplate.Title), content, EB.Localizer.GetString(MessageTemplate.OkBtn), EB.Localizer.GetString(MessageTemplate.CancelBtn), true, false, true, delegate (int result)
            {
                if (result == 0)
                {
                    if (_HeroBattleFinish)
                    {
                        return;
                    }
                    isRequest = true;
                    LadderManager.Instance.GiveUp(delegate
                    {
                        isRequest = false;
                    });
                }
            });//一些判断跳过
            //MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_LADDER_ADMIT_DEFEAT_TIP"), delegate (int result)
            //{
            //    if (result == 0)
            //    {
            //        if (_HeroBattleFinish)
            //        {
            //            return;
            //        }
            //        isRequest = true;
            //        LadderManager.Instance.GiveUp(delegate 
            //        {
            //            isRequest = false;
            //        });
            //    }
            //});
        }
        
        private void OnNotifyHeroBattleFinish()
        {
            _HeroBattleFinish = true;
        }

        /// <summary>
        /// 段位信息
        /// </summary>
        private class UILevelInfo
        {
            /// <summary>
            /// 段位图标
            /// </summary>
            private DynamicUISprite m_LevelIcon;
            /// <summary>
            /// 段位信息
            /// </summary>
            private UILabel m_Level;
            public UILevelInfo(Transform transform)
            {
                m_LevelIcon = transform.GetComponent<DynamicUISprite>();
                m_Level = transform.Find("Label").GetComponent<UILabel>();
            }
            /// <summary>
            /// 设置信息
            /// </summary>
            /// <param name="data">数据</param>
            public void F_SetInfo(SidePlayerInfoData data)
            {
                string levelName = "";
                //换算当前的段位
                for (int i = 0; i < GameStringValue.Ladder_Stage_Names.Length; i++)
                {
                    int currentNeed = LadderManager.Instance.Config.GetStageNeedScore(GameStringValue.Ladder_Stage_Names[i]);
                    if (data.score < currentNeed)
                    {
                        break;
                    }
                    else
                    {
                        levelName = GameStringValue.Ladder_Stage_Names[i];
                    }
                }
                //段位
                m_LevelIcon.spriteName = LadderController.GetStageSpriteName(levelName);
                levelName = LadderController.GetStageCharacterName(levelName);
                LTUIUtil.SetText(m_Level, levelName);
            }
        }

        protected override void UpdateTimeLabelContent(ref float time)
        {
            float buff = Time.unscaledTime - time;
            if (Mathf.RoundToInt(buff) >= 1)
            {
                time = time + 1;
                _lessTime -= 1;
                //等功能完成后在此后加入放大缩小的动画 
                LTUIUtil.SetText(timeLabel, _lessTime.ToString());
                if (m_ConfirmBtn.activeSelf)
                {
                    if (LadderManager.Instance.IsTrusteeship && _lessTime < 7 && LadderManager.Instance.IsTrusteeship && _lessTime > 0)//托管
                    {
                        switch (m_HeroBattleChoiceData.choiceState)
                        {
                            case 0:
                                if (LTHeroBattleEvent.ConfirmBanHero != null)
                                {
                                    FusionAudio.PostEvent("UI/New/JinRen", true);
                                    int heroTplID = m_TeamPlane[1].GetAutoSelectHero();
                                    LTHeroBattleModel.GetInstance().SetChoiceHero(heroTplID);
                                    LTHeroBattleEvent.ConfirmBanHero();
                                }
                                break;
                            case 1:
                                if (LTHeroBattleEvent.ConfirmChoiceHero != null)
                                {
                                    int heroTplID = m_TeamPlane[0].GetAutoSelectHero();
                                    LTHeroBattleModel.GetInstance().SetChoiceHero(heroTplID);
                                    LTHeroBattleEvent.ConfirmChoiceHero();
                                }
                                break;
                            default:
                                EB.Debug.LogError("不应该存在其他情况下的，点击确认按钮情况存在的");
                                break;

                        }
                    }
                }
                if (_lessTime < 10)
                {
                    FusionAudio.PostEvent("UI/New/DaoShu", true);
                }
            }
        }
    }

}