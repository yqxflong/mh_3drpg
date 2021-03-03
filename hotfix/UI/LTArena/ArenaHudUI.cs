using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using Hotfix_LT.Data;
using Debug = EB.Debug;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class ArenaHudUI : UIControllerHotfix, IHotfixUpdate
    {
        public CampaignTextureCmp BGTexture;
        public UILabel rankLabel;
        public GameObject logRedPoint, readyRedPoint;
        public UILabel timesLabel;
        public ArenaChallengerController[] challengers;
        public UIButton[] challengers_Button;
        public LTShowItem[] awardItems;
        public GameObject EmptyLabel;
        private bool IsFirstOpen = true;
        private bool canBtnClick = false;
		protected bool m_HasBtnClick = false;
		private bool HasBtnClick
		{
			get { return m_HasBtnClick; }
			set
			{
				if (value) { RegisterMonoUpdater(); }
				if (!value && !IsTimesRefreshChallenger) { ErasureMonoUpdater(); }
				m_HasBtnClick = value;
			}
		}
		private float timer = 0;

        public RenderSettings RenderSetting;
        private bool IsCombatReadyOpen = false;

        public RenderTexture ModelRT;
        public Camera ModelCamera;
        public UITexture ModelTexture;

        public GameObject floatingText;
        public bool isReCreatModel = false;
        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.arena_topic,
                FusionTelemetry.GamePlayData.arena_event_id,FusionTelemetry.GamePlayData.arena_umengId,"open");
        }

        public override void Start()
        {
            base.Start();
            GameDataSparxManager.Instance.RegisterListener(ArenaManager.ArenaInfoDataId, OnArenaInfoListener);
            controller.transform.localPosition = new Vector3(0, 0, 1000);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameDataSparxManager.Instance.UnRegisterListener(ArenaManager.ArenaInfoDataId, OnArenaInfoListener);
        }
        //public override void OnEnable()
		//{
		//	RegisterMonoUpdater();
		//}

		public void Update()
        {
            // base.Update();
            if (HasBtnClick)
            {
                timer += Time.deltaTime;
                if (timer >= 1.5f)
                {
                    HasBtnClick = false;
                    timer = 0;
                }
            }


            if (IsTimesRefreshChallenger)
            {
                pressTimer_RefreshChallengersBtn += Time.deltaTime;
                if (pressTimer_RefreshChallengersBtn > 3)
                {
                    IsTimesRefreshChallenger = false;
                }
            }

        }

        public override void OnFocus()
        {
            base.OnFocus();
           
            logRedPoint.CustomSetActive(ArenaManager.Instance.IsRankChanged);
            readyRedPoint.CustomSetActive(ArenaManager.Instance.UpdateArenaTeam());
            //Mesh layer改变导致相机渲染不到，重新设置layer
            if (!isReCreatModel)
            {
                for (int i = 0; i < challengers.Length; ++i)
                {
                    var challenger = challengers[i];
                    if (challenger != null) challenger.SetObjLayer(challenger.mDMono.transform.GetComponentInChildren<MoveController>().gameObject, 28);
                }
            }
            if (IsCombatReadyOpen)
            {
                IsCombatReadyOpen = false;
                RenderSettingsManager.Instance.SetActiveRenderSettings("ArenaRenderSetting",RenderSetting);
            }
        }

        public override void OnBlur()
        {
            for (int i = 0; i < challengers.Length; ++i)
            {
                var challenger = challengers[i];
                if (NGUITools.GetActive(challenger.mDMono)) challenger.SetParticleLayer();
            }//创建的模型特效返回时间不确定，因此在不聚焦界面时再设置一次特效层级
           
            isReCreatModel = false;
            base.OnBlur();
        }

        public override IEnumerator OnAddToStack()
        {
            Coroutine coroutine = EB.Coroutines.Run(base.OnAddToStack());

            BGTexture.spriteName = "Game_Background_10";
//#if UNITY_EDITOR
            ModelRT = new RenderTexture(Screen.width, Screen.height, 0);//ModelTexture.width, ModelTexture.height , 0/*, RenderTextureFormat.ARGB1555 */);
//#else
//        ModelRT = new RenderTexture(Screen.width, Screen.height, 0);//ModelTexture.width, ModelTexture.height, 0/*, RenderTextureFormat.ARGB1555 */);
//#endif
            ModelRT.depth = 16;
            ModelCamera.targetTexture = ModelRT;
            ModelTexture.mainTexture = ModelRT;

            LTUIUtil.SetText(timesLabel, "");
            LTUIUtil.SetText(rankLabel, "");
            isReCreatModel = true;
            if (IsFirstOpen)
            {
                IsFirstOpen = false;
                for (int i = 0; i < challengers.Length; ++i)
                {
                    var challenger = challengers[i];
                    if (challenger != null) challenger.Clean();
                }

                ArenaManager.Instance.GetInfo();
            }
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 1f);
            yield return null;

            for (int i = 0; i < challengers.Length; ++i)
            {
                var challenger = challengers[i];
                challenger.Register(string.Format("arena.challengeList[{0}]", i), i);
            }

            GlobalMenuManager.Instance.PushCache("ArenaHudUI");

            yield return coroutine;
            canBtnClick = true;
            Messenger.AddListener(Hotfix_LT.EventName.ArenaRankChangeEvent, ArenaRankChangeListenter);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            IsFirstOpen = true;
            canBtnClick = false;
            BGTexture.spriteName = string.Empty;
            //主工程  StopAllCoroutines();
            // EB.Coroutines.StopAll();
            Messenger.RemoveListener(Hotfix_LT.EventName.ArenaRankChangeEvent, ArenaRankChangeListenter);
            for (int i = 0; i < challengers.Length; ++i)
            {
                var challenger = challengers[i];
                if (challenger != null) challenger.Clean();
            }
            DestroySelf();
            yield break;
        }

        private void ArenaRankChangeListenter()
        {
            logRedPoint.SetActive(true);
        }

        private void OnArenaInfoListener(string path, INodeData data)
        {
            ArenaInfo info = data as ArenaInfo;
         
            if (info.rank < 0 || info.rank >= 10000)
                rankLabel.text = rankLabel.transform.GetChild(0).GetComponent<UILabel>().text = "10000+";
            else
                rankLabel.text = rankLabel.transform.GetChild(0).GetComponent<UILabel>().text = (info.rank + 1).ToString();

            SetTimeText(info);
            ShowRankAward(info.rank + 1);
        }
        
        public void SetTimeText(ArenaInfo info)
        {
            int totalTimes = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes) + info.buyTimes;
            int residueTimes = totalTimes - info.usedTimes;
            LTUIUtil.SetText(timesLabel, string.Format("{0}/{1}", residueTimes, VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes)));
        }

        public void OnRefreshCD()
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ARENA_REFRESH_COOLDOWN"));
        }
        
        private void ShowRankAward(int rank)
        {
            Hotfix_LT.Data.ArenaAwardTemplate awardTpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetArenaAwardTemplate(rank);
            if (awardTpl != null)
            {
                int award_count = 0;
                if (awardTpl.hc != 0)
                {
                    awardItems[award_count].LTItemData = new LTShowItemData("hc", awardTpl.hc, "res", false);
                    awardItems[award_count].mDMono.gameObject.CustomSetActive(true);
                    award_count++;

                }
                if (awardTpl.gold != 0)
                {
                    awardItems[award_count].LTItemData = new LTShowItemData("gold", awardTpl.gold, "res", false);
                    awardItems[award_count].mDMono.gameObject.CustomSetActive(true);
                    award_count++;
                }
                if (awardTpl.arena_gold != 0 && award_count <= 2)
                {
                    awardItems[award_count].LTItemData = new LTShowItemData("arena-gold", awardTpl.arena_gold, "res", false);
                    awardItems[award_count].mDMono.gameObject.CustomSetActive(true);
                    award_count++;
                }
                for (int j = award_count; j < awardItems.Length; ++j)
                {
                    awardItems[j].mDMono.gameObject.CustomSetActive(false);
                }
                EmptyLabel.gameObject.CustomSetActive(award_count == 0);

            }
            else
            {
                for (var i = 0; i < awardItems.Length; i++)
                {
                    var item = awardItems[i];
                    item.mDMono.gameObject.SetActive(false);
                }
            }
        }

        public void OnAdjustTeamButtonClick(UIButton sender)
        {
            System.Action<string, int> onClose = delegate (string teamName, int battleRating)
            {
                ArenaManager.Instance.SaveTeam(battleRating);
            };

            Hashtable param = Johny.HashtablePool.Claim();
            param["teamName"] = ArenaManager.Instance.Config.teamName;
            param["onClose"] = onClose;
            GlobalMenuManager.Instance.Open("SmallPartnerView", param);
        }

        public void OnGotoRankAwardButtonClick()
        {
            var temp = EventTemplateManager.Instance.GetArenaAwardTemplates();
            List<LTRankAwardData> data = new List<LTRankAwardData>();
            for (int i = 0; i < temp.Count; ++i)
            {
                data.Add(new LTRankAwardData(temp[i].rank_top, temp[i].rank_down, temp[i].gold, temp[i].hc, new LTShowItemData("arena-gold", temp[i].arena_gold, "res"), null));
            }
            string tip = EB.Localizer.GetString("ID_uifont_in_ArenaRankAwardUI_SendTips_1");

            var ht = Johny.HashtablePool.Claim();
            ht.Add("itemDatas", data);
            ht.Add("tip", tip);

            GlobalMenuManager.Instance.Open("LTGeneralRankAwardUI", ht);
        }

        public void OnGoToLeaderboardButtonClick(UIButton sender)
        {
            GlobalMenuManager.Instance.Open("LTRankListHud", "Arena");
        }

        public void OnGoToLogsButtonClick(UIButton sender)
        {
            GlobalMenuManager.Instance.Open("ArenaLogUI",ArenaManager.ArenaBattleLogsDataId);
        }

        public void OnTipsButtonClick(UIButton sender)
        {
            string text = EB.Localizer.GetString("ID_ARENA_RULES");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }

        public void OnExchangeShopButtonClick(UIButton sender)
        {
            GlobalMenuManager.Instance.Open("LTStoreUI", "arena");
        }

        public void OnEmbattleButtonClick()
        {
            IsCombatReadyOpen = true;
            BattleReadyHudController.Open(eBattleType.ArenaBattle, null);
        }

        public void OnChallengeButtonClick(ArenaChallengerController param)
        {
            FusionAudio.PostEvent("UI/Battle/StartBattle");
            if (!canBtnClick || HasBtnClick) return;
            HasBtnClick = true;
            int residueNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes) + ArenaManager.Instance.Info.buyTimes - ArenaManager.Instance.Info.usedTimes;
            if (residueNum <= 0)
            {
                OnBuyTimesButtonClick();
                return;
            }

            if (AllianceUtil.GetIsInTransferDart("ID_ARENA"))
            {
                return;
            }

            ArenaManager.Instance.StartChallenge(param.Challenger.uid, param.Challenger.rank);
        }

    public void OnFastButtonClick(ArenaChallengerController param)
    {
	    FusionAudio.PostEvent("UI/Battle/StartBattle");
	    if (!canBtnClick || HasBtnClick) return;
	    HasBtnClick = true;        
	    int residueNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes) + ArenaManager.Instance.Info.buyTimes - ArenaManager.Instance.Info.usedTimes;
	    if(residueNum<=0)
	    {
		    OnBuyTimesButtonClick();
		    return;
	    }

	    if (AllianceUtil.GetIsInTransferDart("ID_ARENA"))
	    {
		    return;
	    }
	    ArenaManager.Instance.fastChallenge(param.Challenger.uid, param.Challenger.rank,true);
    }

    public void OnPlayerCheckClick(ArenaChallengerController param)
    {
        if (!canBtnClick) return;
        Hashtable mainData = Johny.HashtablePool.Claim();
        mainData.Add("name", param.Challenger.name);
        string icon = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(param.Challenger.charId, param.Challenger.skin).icon;
        mainData.Add("icon", icon);
        mainData.Add("level", param.Challenger.level);
            mainData.Add("headFrame", EconemyTemplateManager.Instance.GetHeadFrame(param.Challenger.frame).iconId);
            string allianceName = EB.Localizer.GetString("ID_codefont_in_ArenaHudUI_9210");
        if (!string.IsNullOrEmpty(param.Challenger.allianceName))
            allianceName = param.Challenger.allianceName;
        mainData.Add("allianceName", allianceName);

        Hashtable data = Johny.HashtablePool.Claim();
        data.Add(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3_TYPE0, param.Challenger.rank);
        Hashtable viewData = Johny.HashtablePool.Claim();
        viewData["mainData"] = mainData;
        viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0] = param.Challenger.uid;
        viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE0;
        viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE0;
        viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3] = data;
        GlobalMenuManager.Instance.Open("LTCheckPlayerFormationInfoUI", viewData);
    }

        public void OnBuyTimesButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (BalanceResourceUtil.GetUserDiamond() < ArenaManager.Instance.Info.buyCost)
            {
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            var ht = Johny.HashtablePool.Claim();
            ht.Add("0", ArenaManager.Instance.Info.buyCost);
            MessageTemplateManager.ShowMessage(902123, ht, delegate (int result)
            {
                if (result == 0)
                {
                    int totalTimes = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes) + ArenaManager.Instance.Info.buyTimes + 1;
                    int residueTimes = Mathf.Max(0, totalTimes - ArenaManager.Instance.Info.usedTimes);
                    ArenaManager.Instance.BuyChallengeTimes(delegate (bool successful)
                    {
                        timesLabel.text = string.Format("{0}/{1}", residueTimes, VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes));
                        LTDailyDataManager.Instance.SetDailyDataRefreshState();
                    });
                }
            });
            Johny.HashtablePool.Release(ht);
        }


		protected bool IsTimesRefreshChallenger
		{
			get { return m_IsTimesRefreshChallenger; }
			set
			{
				if (value) { RegisterMonoUpdater(); } else { if (!HasBtnClick) ErasureMonoUpdater(); }
				m_IsTimesRefreshChallenger = value;
			}
		}
		private bool m_IsTimesRefreshChallenger;
		private float pressTimer_RefreshChallengersBtn = 0;
        public void OnRefreshChallengersButtonClick()
        {
            ////  冷却刷新计时 
            if (IsTimesRefreshChallenger == false)
            {
                InterOnRefreshChallengersButtonClick();
            }
            else
            {
                OnRefreshCD();
            }
        }

        private void InterOnRefreshChallengersButtonClick()
        {
            pressTimer_RefreshChallengersBtn = 0;
            IsTimesRefreshChallenger = true;

            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!canBtnClick || HasBtnClick) return;
            HasBtnClick = true;
            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 1f);
            ArenaManager.Instance.RefreshChallengers();
        }


        public void OnRefreshCooldownButtonClick(UIButton sender)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!canBtnClick || HasBtnClick) return;
            HasBtnClick = true;
            MessageTemplateManager.ShowMessage(902017, null, delegate (int result)
            {
                if (result == 0)
                {
                    if (BalanceResourceUtil.GetUserDiamond() < ArenaManager.Instance.Config.refreshCooldownCost.HC)
                    {
                        BalanceResourceUtil.HcLessMessage();
                        return;
                    }
                }
            });
        }

        public override void OnCancelButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.RemoveCache("ArenaHudUI");

            controller.Close();
        }


        public UIButton HotfixBtn0;
        public UIButton HotfixBtn1;
        public UIButton HotfixBtn5;
		public UIButton HotfixBtn6;
        public UIButton HotfixBtn7;
        public UIButton HotfixBtn8;
        public UIButton HotfixBtn9;
        public UIButton HotfixBtn13;
        public UIButton HotfixBtn14;
        public UIButton RefreshChallengersBtn;

        public override void Awake()
        {
            base.Awake();

            BGTexture = controller.transform.Find("UIFrameBG/FrameFG").GetComponent<CampaignTextureCmp>();
            rankLabel = controller.transform.Find("UIFrameBG/TopLeftAnchor/Rank/Number").GetComponent<UILabel>();
            logRedPoint = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/LogBtn/RedPoint").gameObject;
            readyRedPoint = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/EmbattleBtn/RedPoint").gameObject;
            timesLabel = controller.transform.Find("Content/Limit/LeftTimes/Number").GetComponent<UILabel>();
            challengers = new ArenaChallengerController[3];
            challengers[0] =
                controller.transform.GetMonoILRComponent<ArenaChallengerController>(
                    "Content/ChallengerList/Challenger0");
            challengers[1] =
                controller.transform.GetMonoILRComponent<ArenaChallengerController>(
                    "Content/ChallengerList/Challenger1");
            challengers[2] =
                controller.transform.GetMonoILRComponent<ArenaChallengerController>(
                    "Content/ChallengerList/Challenger2");
            challengers_Button = new UIButton[3];
            challengers_Button[0] = challengers[0].mDMono.transform.Find("ChallengeBtn").GetComponent<UIButton>();
            challengers_Button[0].onClick.Add(new EventDelegate(() => { OnChallengeButtonClick(challengers[0]); }));
            challengers_Button[1] = challengers[1].mDMono.transform.Find("ChallengeBtn").GetComponent<UIButton>();
            challengers_Button[1].onClick.Add(new EventDelegate(() => { OnChallengeButtonClick(challengers[1]); }));
            challengers_Button[2] = challengers[2].mDMono.transform.Find("ChallengeBtn").GetComponent<UIButton>();
            challengers_Button[2].onClick.Add(new EventDelegate(() => { OnFastButtonClick(challengers[2]); }));

            challengers[0].mDMono.transform.Find("BG").GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(() =>
            {
                OnPlayerCheckClick(challengers[0]);
            }));
            challengers[1].mDMono.transform.Find("BG").GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(() =>
            {
                OnPlayerCheckClick(challengers[1]);
            }));
            challengers[2].mDMono.transform.Find("BG").GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(() =>
            {
                OnPlayerCheckClick(challengers[2]);
            }));

            awardItems = new LTShowItem[3];
            awardItems[0] = controller.transform.Find("Content/Limit/RankAward/Grid/LTShowItem").GetMonoILRComponent<LTShowItem>();
            awardItems[1] = controller.transform.Find("Content/Limit/RankAward/Grid/LTShowItem (1)").GetMonoILRComponent<LTShowItem>();
            awardItems[2] = controller.transform.Find("Content/Limit/RankAward/Grid/LTShowItem (2)").GetMonoILRComponent<LTShowItem>();

            EmptyLabel = controller.transform.Find("Content/Limit/RankAward/EmptyAward").gameObject;
            RenderSetting = controller.transform.Find("Content/ArenaRenderSetting").GetComponent<RenderSettings>();
            ModelTexture = controller.transform.Find("CamTexture").GetComponent<UITexture>();
            ModelCamera = controller.transform.Find("Camera").GetComponent<Camera>();
            HotfixBtn0 = controller.transform.Find("UIFrameBG/TopLeftAnchor/TopLeft/CancelBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnCancelButtonClick));
            HotfixBtn1 = controller.transform.Find("UIFrameBG/TopLeftAnchor/RuleTipsBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(() => { OnTipsButtonClick(HotfixBtn1); }));

            HotfixBtn5 = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/AwardBtn").GetComponent<UIButton>();
            HotfixBtn5.onClick.Add(new EventDelegate(OnGotoRankAwardButtonClick));
            HotfixBtn6 = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/ExchangeShopBtn").GetComponent<UIButton>();
            HotfixBtn6.onClick.Add(new EventDelegate(() => { OnExchangeShopButtonClick(HotfixBtn6); }));
            HotfixBtn7 = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/GoToLeaderboardBtn").GetComponent<UIButton>();
            HotfixBtn7.onClick.Add(new EventDelegate(() => { OnGoToLeaderboardButtonClick(HotfixBtn7); }));
            HotfixBtn8 = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/LogBtn").GetComponent<UIButton>();
            HotfixBtn8.onClick.Add(new EventDelegate(() => { OnGoToLogsButtonClick(HotfixBtn8); }));
            HotfixBtn9 = controller.transform.Find("UIFrameBG/TopRightAnchor/Misc/EmbattleBtn").GetComponent<UIButton>();
            HotfixBtn9.onClick.Add(new EventDelegate(OnEmbattleButtonClick));
            HotfixBtn13 = controller.transform.Find("Content/Limit/LeftTimes/AddBtn").GetComponent<UIButton>();
            HotfixBtn13.onClick.Add(new EventDelegate(OnBuyTimesButtonClick));
            HotfixBtn14 = controller.transform.Find("Content/Limit/LeftTimes/RefreshBtn").GetComponent<UIButton>();
            HotfixBtn14.onClick.Add(new EventDelegate(OnBuyTimesButtonClick));
            RefreshChallengersBtn = controller.transform.Find("Content/Limit/LeftTimes/RefreshChallengersBtn").GetComponent<UIButton>();
            RefreshChallengersBtn.onClick.Add(new EventDelegate(OnRefreshChallengersButtonClick));
        }
    }
}