using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialHudCtrl : UIControllerHotfix
    {
        public override bool IsFullscreen()
        {
            return true;
        }

        public override bool ShowUIBlocker
        {
            get
            {
                return false;
            }
        }

        public override bool CanAutoBackstack()
        {
            return true;
        }

        private UILabel mLevelLabel;

        public UILabel mLayerLabel;

        private UITexture mMonsterTex;

        private TweenAlpha mTexTweenAlpha;

        private UISprite mBoxSprite;

        public UITexture mEnterNextSprite;

        private UILabel mTimesLabel;

        private UILabel mGetRewardLabel;

        private UISprite mBattleSprite;

        private UISprite mIsGetRewardSprite;

        private LTShowItem[] mItemList;

        private GameObject mBg1;

        private GameObject mBg2;

        private TweenAlpha mBgMask;
        //Buff
        private UIButton BuffBtn;//Sort By ——0.LevelLabel,  1.NameLabel,  2.IconSprite（use getchild(x) get it）
        private Transform BuffView;
        private Transform BuffView_IconTran;//Sort By ——0.LevelLabel,  1.NameLabel,  2.IconSprite（use getchild(x) get it）
        private UILabel PassNumLabel;
        private UILabel BuffDescLabel;
        private UILabel BuffInfoLabel;
        private UITable BuffTable;
        private UIButton BuffViewMask;

        public BoxCollider CompeteBtnBox;
        public GameObject CompeteBtnLockObj;
        private UILabel CompeteBtnLabel;
        public GameObject CompeteBtnRP;

        private bool isClickBtn=false;//防止手机连点挑战与扫荡

        private Coroutine CreateMonsterCor;

        private Coroutine PlayAnimCor;

        private Coroutine PlayExitAnimCor;
        private EnterVigorController VigorController;
        private int EnterVigor;

        private LTClearanceLineupBtn _clearanceLineupBtn;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            UIButton backBtn = t.GetComponent<UIButton>("Edge/TopLeft/CancelBtn");
            backBtn.onClick.Add(new EventDelegate(OnCacncelBtnClick));

            UIButton rankBtn = t.GetComponent<UIButton>("Edge/BottomLeft/RankBtn");
            rankBtn.onClick.Add(new EventDelegate(OnRankBtnClick));

            UIButton sweepBtn = t.GetComponent<UIButton>("Edge/BottomLeft/BlitzBtn");
            sweepBtn.onClick.Add(new EventDelegate(OnSweepBtnClick));

            ConsecutiveClickCoolTrigger enterNextBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("Center/EnterNext");
            enterNextBtn.clickEvent.Add(new EventDelegate(OnEnterNextBtnClick));
            enterNextBtn.gameObject.CustomSetActive(false);

            ConsecutiveClickCoolTrigger battleBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("Edge/Bottom/BattleBtn");
            battleBtn.clickEvent.Add(new EventDelegate(OnBattleBtnClick));

            VigorController = t.GetMonoILRComponent<EnterVigorController>("Edge/Bottom/BattleBtn/Sprite");
            UIButton noticeBtn =t.Find("Edge/TopRight/Notice").GetComponent<UIButton>();
            noticeBtn.onClick.Add(new EventDelegate(OnNoticeBtnClick));

            ConsecutiveClickCoolTrigger boxBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("Center/BoxSprite");
            boxBtn.clickEvent.Add(new EventDelegate(OnBoxClick));
            boxBtn.gameObject.CustomSetActive(false);



            mLevelLabel =t.GetComponent<UILabel>("Edge/TopRight/LevelInfo/LevelLabel");

            mLayerLabel = t.GetComponent<UILabel>("Edge/TopRight/LevelInfo/LayerLabel");

            mMonsterTex = t.GetComponent<UITexture>("Center/MonsterTexture");
            mTexTweenAlpha = mMonsterTex.GetComponent<TweenAlpha>();
            mMonsterTex.gameObject.CustomSetActive(false);

            mBoxSprite = boxBtn.gameObject.GetComponent<UISprite>();
            mBoxSprite.gameObject.CustomSetActive(false);

            mEnterNextSprite = enterNextBtn.gameObject.GetComponent<UITexture>();
            mEnterNextSprite.gameObject.CustomSetActive(false);

            mTimesLabel = t.GetComponent<UILabel>("Edge/Bottom/BattleBtn/TimesLabel");

            mGetRewardLabel = t.GetComponent<UILabel>("Edge/Bottom/RewardLabel");

            mBattleSprite = battleBtn.gameObject.GetComponent<UISprite>();

            mIsGetRewardSprite = t.GetComponent<UISprite>("Edge/BottomRight/GetRewardSprite");

            Transform itemListTran = t.Find("Edge/BottomRight/ItemList");
            mItemList = itemListTran.GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem",true,true);

            mBg1 = t.Find("BG1").gameObject;

            mBg2 = t.Find("BG2").gameObject;

            mBgMask = t.GetComponent<TweenAlpha>("Edge/Mask");

            BuffBtn=t.GetComponent<UIButton>("Edge/TopLeft/BuffBtn");
            BuffBtn.onClick.Add(new EventDelegate(OnBuffBtnClick));

            BuffView = t.GetComponent<Transform>("Edge/TopLeft/Panel");

            BuffView_IconTran = t.GetComponent<Transform>("Edge/TopLeft/Panel/BuffView/Buff"); 

            PassNumLabel = t.GetComponent<UILabel>("Edge/TopLeft/Panel/BuffView/PassNum/num");

            BuffDescLabel = t.GetComponent<UILabel>("Edge/TopLeft/Panel/BuffView/Table/BuffValue");

            BuffInfoLabel = t.GetComponent<UILabel>("Edge/TopLeft/Panel/BuffView/Table/BuffLevelValue");

            BuffTable = t.GetComponent<UITable>("Edge/TopLeft/Panel/BuffView/Table");

            BuffViewMask = t.GetComponent<UIButton>("Edge/TopLeft/Panel/BuffView/Mask");
            BuffViewMask.onClick.Add(new EventDelegate(OnBuffViewClose));
            BuffView.gameObject.CustomSetActive(false);

            CompeteBtnBox = t.GetComponent<BoxCollider>("Edge/TopRight/CompeteBtn");
            CompeteBtnLabel= CompeteBtnBox.transform.GetComponent<UILabel>("Label");
            CompeteBtnLockObj = CompeteBtnBox.transform.Find("LockObj").gameObject;
            CompeteBtnRP= CompeteBtnBox.transform.Find("RedPoint").gameObject;
            CompeteBtnBox.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnCompeteBtnClick));

            _clearanceLineupBtn = t.GetMonoILRComponent<LTClearanceLineupBtn>("Edge/LTClearanceLineupBtn");
        }

        public void OnCompeteBtnClick()
        {
            if (LTUltimateTrialDataManager.Instance.GetTimeTip(false, true).Equals("close"))
            {
                //赛季结束,不在活动时间
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_NOT_IN_ACTIVITY_TIME"));
                return;
            }
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTUltimateTrialCompeteHud");
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private int intParam = 0;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null) intParam = (int)param;
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.infinite_topic,
                FusionTelemetry.GamePlayData.infinite_event_id,FusionTelemetry.GamePlayData.infinite_umengId,"open",intParam);
        }

        private bool isFirstEnterUI;
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            LTMainHudManager.Instance.isBattleBtnClick = false;
            isFirstEnterUI =LTUltimateTrialDataManager.Instance.isFirstEnterUI();
            bool pastIsLayerComplete = LTUltimateTrialDataManager.Instance.IsLayerComplete();
            LTUltimateTrialDataManager.Instance.OnResetTimesLabel += new System.Action(OnResetTimesLabelFunc);
            LTUltimateTrialDataManager.Instance.RequestGetInfo(delegate
            {
                bool curIsLayerComplete = LTUltimateTrialDataManager.Instance.IsLayerComplete();

                if (LTUltimateTrialDataManager.Instance.IsOverMaxLayer())
                {
                    InitUI();
                    mEnterNextSprite.gameObject.CustomSetActive(true);
                    mBattleSprite.gameObject.CustomSetActive(false);
                }
                else
                {
                    if (!pastIsLayerComplete && curIsLayerComplete)
                    {
                        PlayAnimCor = EB.Coroutines.Run(PlayAnimation());
                        InitUI();
                    }
                    else
                    {
                        InitUI();
                        InitMonster();
                    }
                }

                if (intParam != 0)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    FusionAudio.PostEvent("UI/General/ButtonClick");
                    GlobalMenuManager.Instance.Open("LTUltimateTrialSelectView",intParam);
                }
                GlobalMenuManager.Instance.PushCache("LTUltimateTrialHud");
                mBgMask.PlayForward();
            });
        }

        public override IEnumerator OnRemoveFromStack()
        {
            mMonsterTex.gameObject.CustomSetActive(false);
            if (mTimer > 0)
            {
                ILRTimerManager.instance.RemoveTimer(mTimer);
                mTimer = 0;
            }

            LTUltimateTrialDataManager.Instance.OnResetTimesLabel -= new System.Action(OnResetTimesLabelFunc);
            EB.Coroutines.Stop(CreateMonsterCor);
            EB.Coroutines.Stop(PlayExitAnimCor);
            EB.Coroutines.Stop(PlayAnimCor);
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            RenderSettings rs = controller .transform.GetComponentInChildren<RenderSettings>();
            if (rs != null)
            {
                RenderSettingsManager.Instance.SetActiveRenderSettings(rs.name, rs);
            }
            LTChargeManager.Instance.UpdateLimitedTimeGiftNotice();
            if (mTimer > 0)
            {
                CompeteBtnRP.CustomSetActive(LTUltimateTrialDataManager.Instance.GetCompeteRP());
            }
            if (Lobby!=null&&Lobby.mDMono.gameObject.activeSelf == false) Lobby.mDMono.gameObject.CustomSetActive(true);
        }

        private void InitMonster()
        {
            int layer = LTUltimateTrialDataManager.Instance.GetCurLayer();

            bool isComplete = LTUltimateTrialDataManager.Instance.IsLayerComplete();

            bool isGetReward = LTUltimateTrialDataManager.Instance.IsGetReward();

            var tpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetInfiniteChallengeTpl(layer);
            if (tpl == null)
            {
                return;
            }

            mMonsterTex.gameObject.CustomSetActive((!isComplete) && (!isGetReward));
            if ((!isComplete) && (!isGetReward))
            {
                mTexTweenAlpha.ResetToBeginning();
                mTexTweenAlpha.PlayForward();
            }
            if (!isComplete)
            {
                CreateMonsterCor =  EB.Coroutines.Run(CreateMonster(tpl.model_name));
            }
        }
        
        private void InitUI()
        {
            int layer = LTUltimateTrialDataManager.Instance.GetCurLayer();
            bool isComplete = LTUltimateTrialDataManager.Instance.IsLayerComplete();
            bool isGetReward = LTUltimateTrialDataManager.Instance.IsGetReward();

            var tpl = EventTemplateManager.Instance.GetInfiniteChallengeTpl(layer);
            if (tpl == null)
            {
                return;
            }

            if (tpl.layer % 5 == 0)
            {
                mBoxSprite.spriteName = "Train_Icon_Baoxiang2";
                mBg1.CustomSetActive(false);
                mBg2.CustomSetActive(true);
            }
            else
            {
                mBoxSprite.spriteName = "Train_Icon_Baoxiang1";
                mBg2.CustomSetActive(false);
                mBg1.CustomSetActive(true);
            }

            mLevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_EconomyTemplateManager_68326"),tpl.level.ToString());
            mLayerLabel.text = string.Format(EB.Localizer.GetString("ID_LEVEL_TEXT_FORMAT"), (EventTemplateManager.Instance.CalculCurLayer(tpl)).ToString());

            mBoxSprite.gameObject.CustomSetActive(isComplete && !isGetReward);
            mEnterNextSprite.gameObject.CustomSetActive(isComplete && isGetReward);
            mTimesLabel.gameObject.CustomSetActive(!isComplete && !isGetReward);
            mGetRewardLabel.gameObject.CustomSetActive(isComplete && !isGetReward);
            mBattleSprite.gameObject.CustomSetActive(!isComplete && !isGetReward);
            if (LTUltimateTrialDataManager.Instance.IsOverMaxLayer())
            {
                mIsGetRewardSprite.gameObject.CustomSetActive(true);
            }
            else
            {
                mIsGetRewardSprite.gameObject.CustomSetActive(isComplete && isGetReward);
            }

            InitFirstReward(tpl.first_award);

            InitBuffInfo();
            OnResetTimesLabelFunc();
            
            if(LTUltimateTrialDataManager.Instance.IsCanCompete())
            {
                CompeteBtnBox.enabled = true;
                CompeteBtnLockObj.CustomSetActive(false);
                CompeteBtnRP.CustomSetActive(LTUltimateTrialDataManager.Instance.GetCompeteRP());
                UpdateTime();
                if (mTimer == 0) mTimer = ILRTimerManager.instance.AddTimer(30000, int.MaxValue, UpdateTime);
            }
            else
            {
                CompeteBtnBox.enabled = false;
                CompeteBtnLockObj.CustomSetActive(true);
                var mtpl = EventTemplateManager.Instance.GetInfiniteChallengeTpl(LTUltimateTrialDataManager.Instance.GetCompeteCondition());
                CompeteBtnLabel.text = string.Format(EB.Localizer.GetString("ID_ULTIMATE_COMPETE_TIP"), mtpl.level);
            }

            RefreshClearanceLineup();
        }

        private void RefreshClearanceLineup()
        {
            var hasLowestInfo = LTUltimateTrialDataManager.Instance.GetUidFromLowest() > 0;
            _clearanceLineupBtn.mDMono.gameObject.SetActive(hasLowestInfo);

            if (hasLowestInfo)
            {
                _clearanceLineupBtn.RegisterClickEvent(LTUltimateTrialDataManager.Instance.RequestGetLowestTeamViews);
                _clearanceLineupBtn.SetLevel(LTUltimateTrialDataManager.Instance.GetCurLayer());
                _clearanceLineupBtn.SetIcon(LTUltimateTrialDataManager.Instance.GetAvatarFromLowest());
                _clearanceLineupBtn.SetFrame(LTUltimateTrialDataManager.Instance.GetHeadFrameFromLowest());
            }
        }

        private int mTimer = 0;
        private void UpdateTime(int timer = 0)
        {
            CompeteBtnLabel.text = LTUltimateTrialDataManager.Instance.GetTimeTip();
        }

        private void InitBuffInfo()
        {
            List<Hotfix_LT.Data.InfiniteBuffTemplate> buffTp = Hotfix_LT.Data.EventTemplateManager.Instance.GetAllInfiniteBuffTp1();
            int PassNum = LTUltimateTrialDataManager.Instance.GetPeoplePassNum();
            int level = 0;
            for(int i = 0; i < buffTp.Count; i++)
            {
                if (PassNum >= buffTp[i].conditionNum) level = buffTp[i].level;
                else break;
            }
            BuffBtn.transform.GetChild(0).GetComponent<UILabel>().text = BuffView_IconTran.transform.GetChild(0).GetComponent<UILabel>().text = level.ToString();
            BuffBtn.transform.GetChild(1).GetComponent<UILabel>().text = BuffView_IconTran.transform.GetChild(1).GetComponent<UILabel>().text = buffTp[Mathf.Clamp(level-1,0,10)].name;
            PassNumLabel.text = PassNum.ToString();
            string buffDes = level > 0 ? string.Format("[43fe79]{0}", Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(buffTp[level - 1].effect).Description) : "";
            BuffDescLabel.text = buffDes;
            if (string.IsNullOrEmpty(buffDes))
            {
                BuffDescLabel.gameObject.CustomSetActive(false);
                BuffTable.Reposition();
            }

            string buffInfoStr = getBuffInfoStr(buffTp,level);
            BuffInfoLabel.text = buffInfoStr;
        }
        private string getBuffInfoStr(List<Hotfix_LT.Data.InfiniteBuffTemplate> buffTp,int level=0)
        {
            string str = null;
            for(int i=0;i< buffTp.Count; i++)
            {
                str += string.Format(EB.Localizer.GetString("ID_codefont_in_LTUltimateTrialHudCtrl_13134"), level - 1 >= i ? "[42fe79]" : "[ffffff]", buffTp[i].name, buffTp[i].level, buffTp[i].conditionNum);
            }
            return str;
        }
        
        private void InitFirstReward(List<LTShowItemData> dataList)
        {
            for (int i = 0; i < mItemList.Length; i++)
            {
                if (i < dataList.Count)
                {
                    mItemList[i].mDMono.gameObject.CustomSetActive(true);
                    mItemList[i].LTItemData = dataList[i];
                }
                else
                {
                    mItemList[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }

        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private string ModelName = null;
        private const int CharacterPoolSize = 10;

        private IEnumerator CreateMonster(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
            {
                yield break;
            }

            if (modelName == ModelName)
            {
                yield break;
            }

            ModelName = modelName;
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                UI3DLobby.Preload(modelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = mMonsterTex.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = mMonsterTex;
                    Lobby.CharacterPoolSize = CharacterPoolSize;
                    Lobby.SetCameraMode(1.5f, true);
                }
            }

            if (Lobby != null)
            {
                Lobby.VariantName = modelName;
                while (Lobby.Current == null || Lobby.Current.character == null)
                {
                    yield return null;
                }
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kReady);
            }
        }

        private IEnumerator PlayAnimation()
        {
            int layer = LTUltimateTrialDataManager.Instance.GetCurLayer();

            var tpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetInfiniteChallengeTpl(layer);
            if (tpl == null)
            {
                yield break;
            }

            if (mMonsterTex != null)
            {
                mMonsterTex.gameObject.CustomSetActive(!isFirstEnterUI); //mMonsterTex.enabled = !isFirstEnterUI;
            }

            yield return CreateMonster(tpl.model_name);
            yield return new WaitForSeconds(0.3f);

            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kDeath);
            yield return new WaitForSeconds(1);

            if (mMonsterTex != null)
            {
                mMonsterTex.gameObject.CustomSetActive(false);
            }

            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kReady);
        }

        private void OnCacncelBtnClick()
        {
            GlobalMenuManager.Instance.RemoveCache("LTUltimateTrialHud");
            FusionAudio.PostEvent("UI/General/ButtonClick");
            controller.Close();
        }

        private void OnRankBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            FusionAudio.PostEvent("UI/General/ButtonClick");
			GlobalMenuManager.Instance.Open("LTRankListHud", "UltimateChallenge");
		}

        private void OnNoticeBtnClick()
        {
            if (isClickBtn) return;
            isClickBtn = true;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            string text = EB.Localizer.GetString("ID_TRIAL_RULES");
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
            isClickBtn = false;
        }

        public void OnSweepBtnClick()
        {
            if (isClickBtn) return;
            isClickBtn = true;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTUltimateTrialSelectView");
            isClickBtn = false;
        }


        public void OnBattleBtnClick()
        {
            if (isClickBtn) return;
            isClickBtn = true;
            FusionAudio.PostEvent("UI/General/ButtonClick");

            //判断体力是否足够
            if (BalanceResourceUtil.EnterVigorCheck(EnterVigor))
            {
                isClickBtn = false;
                return;
            }

            if (AllianceUtil.GetIsInTransferDart(""))
            {
                isClickBtn = false;
                return;
            }
            else
            {
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                int layer = LTUltimateTrialDataManager.Instance.GetCurLayer();
                string enemyLayout = "";
                var tpl = EventTemplateManager.Instance.GetInfiniteChallengeTpl(layer);
                if (tpl != null) enemyLayout = tpl.combat_layout;
                System.Action startCombatCallback = delegate { LTUltimateTrialDataManager.Instance.RequestStartChallenge(layer); };
                BattleReadyHudController.Open(eBattleType.InfiniteChallenge, startCombatCallback, enemyLayout);
                isClickBtn = false;
            }
        }

        public void OnBoxClick()
        {
            if (isClickBtn) return;
            isClickBtn = true;
            FusionAudio.PostEvent("UI/General/ButtonClick");
            int layer = LTUltimateTrialDataManager.Instance.GetCurLayer();
            LTUltimateTrialDataManager.Instance.RequestGetFirstReward(layer,delegate 
            {
                List<LTShowItemData> list = LTUltimateTrialDataManager.Instance.GetFirstReward();
                if (list.Count > 0)
                {
                    for(int i=0;i< list.Count; i++)
                    {
                        if (list[i].id == "hc")
                            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, list[i].count, "极限试炼获得");
                        if (list[i].id == "gold")
                            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, list[i].count, "极限试炼获得");
                    }
                    UI.CommonConditionParse.SetFocusViewName("IsTrialShowReward");
                    InitUI();
                    InitMonster();
                    GlobalMenuManager.Instance.Open("LTShowRewardView", list);
                }
                isClickBtn = false;
            });
        }

        public void OnEnterNextBtnClick()
        {
            if (isClickBtn) return;
            isClickBtn = true;
            FusionAudio.PostEvent("UI/Floor/Transfer");

            if (LTUltimateTrialDataManager.Instance.IsOverMaxLayer())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTUltimateTrialHudCtrl_20398"));
                isClickBtn = false;
                return;
            }

            if (LTUltimateTrialDataManager.Instance.IsMaxLayer())
            {
                LTUltimateTrialDataManager.Instance.RequestEnterNextLayer(delegate { });
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTUltimateTrialHudCtrl_20398"));
                isClickBtn = false;
                return;
            }
            LTUltimateTrialDataManager.Instance.RequestEnterNextLayer(delegate 
            {
                PlayExitAnimCor = EB.Coroutines.Run(OnEnterNextAmi());
            });
        }
        private IEnumerator OnEnterNextAmi()//过场动画
        {
            InputBlockerManager.Instance.Block(InputBlockReason.BINDABLE_ITEM_FLY_ANIM);
            mBgMask.PlayReverse();
            yield return new WaitForSeconds(mBgMask.duration);
            InitUI();
            mBgMask.PlayForward();
            InitMonster();
            isClickBtn = false;
            yield break;
        }

        private void OnResetTimesLabelFunc()
        {
            int dayDisCountTime = 0;
            int oldVigor = 0;
            int NewVigor = 0;
            int times = LTUltimateTrialDataManager.Instance.GetCurrencyTimes();
            NewGameConfigTemplateManager.Instance.GetEnterVigor(eBattleType.InfiniteChallenge, out dayDisCountTime, out NewVigor, out oldVigor);
            int curDisCountTime = dayDisCountTime - times;
            curDisCountTime = curDisCountTime > 0 ? curDisCountTime : 0;
            EnterVigor = curDisCountTime > 0 ? NewVigor : oldVigor;
            mTimesLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_2985"), curDisCountTime.ToString());
            VigorController.Init(oldVigor, NewVigor, curDisCountTime > 0);
        }

        public void OnBuffBtnClick()
        {
            if (isClickBtn) return;
            isClickBtn = true;
            FusionAudio.PostEvent("UI/General/ButtonClick");
            LTUltimateTrialDataManager.Instance.RequestGetInfo(delegate
            {
                InitBuffInfo();
                BuffView.gameObject.CustomSetActive(true);
                isClickBtn = false;
            });
        }
        public void OnBuffViewClose()
        {
            BuffView.gameObject.CustomSetActive(false);
        }
    }
}
