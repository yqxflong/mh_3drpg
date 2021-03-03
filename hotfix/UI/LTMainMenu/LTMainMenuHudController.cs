using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using _HotfixScripts.Utils;
using BE.Level;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTMainMenuHudController : UIControllerHotfix, IHotfixUpdate
    {
        public UILabel CombatbatPowerLabel;
        public UIInvitesController UIInvitesCtrl;
        public JoystickController JoystickCtrl;
        public List<TweenPosition> MainMenuTweenPositions;
        private LTDailyData CurrentDailyData, NextDailyData;
        private List<LTDailyData> mMainUIDailyData;
        //private List<LTDailyData> mAllActivityData;

        // private Transform curHeadTransform = null;

        public static LTMainMenuHudController Instance;
        public PlayerVigourTip VigourTip;

        private bool isInitDailyTimer;

        public List<string> BtnPathList = new List<string>
        {
			"Edge/Right/RightFuncPanel/Content/Function/Bag",
			"Edge/Bottom/Grid/Friend",
			"Edge/Right/RightFuncPanel/Content/Function/Legion",
            "Edge/TopLeftAnchor/TopLeft/Left/Nation",
			"Edge/Bottom/Grid/Rank",
			"Edge/Right/RightFuncPanel/Content/Function/HandBook",
            "Edge/TopLeftAnchor/TopLeft/Left/Btn",
            "Edge/TopRightAnchor/TopRight/FuncList/ChargeGift",
            "Edge/TopRightAnchor/TopRight/FuncList/Privilege",
            "Edge/TopRightAnchor/TopRight/FuncList/HolidayActivity",
            "Edge/TopRightAnchor/TopRight/FuncList/ChargeStore",
            "Edge/TopRightAnchor/TopRight/FuncList/SignIn",
            "Edge/TopRightAnchor/TopRight/FuncList/Welfare",
            "Edge/TopRightAnchor/TopRight/FuncList/Store",
			"Edge/Right/RightFuncPanel/Content/Function/Lottery",
            "Edge/TopRightAnchor/TopRight/FuncList2/VIP",
            "Edge/TopRightAnchor/TopRight/FuncList/InviteFriend",
            "Edge/TopRightAnchor/TopRight/FuncList/ComeBack",        
            "Edge/Bottom/Pet",
			"Edge/Bottom/Grid/MailBtn",
			"Edge/Bottom/Grid/Setting",
            "Edge/Right/RightFuncPanel/Content/Function/Task",
            "Edge/Right/RightFuncPanel/Content/Function/Partner",
            "Edge/Right/RightFuncPanel/Content/Campaign",
            "Edge/Right/RightFuncPanel/Content/Btn",
            "Edge/Right/WorldBossRank/LTWorldBossRankView/Rank/CancelBtn",
            "Edge/Right/WorldBossRank/LTWorldBossRankView/BtnList/Reward",
            "Edge/Right/WorldBossRank/LTWorldBossRankView/BtnList/Rank",
            "Edge/Right/WorldBossRank/LTWorldBossRankView/BtnList/BattleReady",
			"Edge/Bottom/Grid/ChatBtn",
            "Edge/Right/RightFuncPanel/Content/Function/Promotion",
            "Edge/TopRightAnchor/TopRight/FuncList/Activity"
        };

        /// <summary>
		/// 可见时候回调
		/// </summary>
		public Action v_OnEnableCallback;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            controller.hudRoot = t;
            Grid1CountLimitNum = 10;

            CombatbatPowerLabel = t.GetComponent<UILabel>("Edge/TopLeftAnchor/TopLeft/CombatPower/Container/Base");
            UIInvitesCtrl = t.GetMonoILRComponent<UIInvitesController>("Edge/Bottom/NoticeBoard");
            JoystickCtrl = t.GetMonoILRComponent<JoystickController>("Center/Joystick");
            VigourTip = t.GetMonoILRComponent<PlayerVigourTip>("Center/PlayerVigourTipUI");

            var worldBossRankLogic = t.GetDataLookupILRComponent<UIWorldBossRankLogic>("Edge/Right/WorldBossRank/LTWorldBossRankView");

            MainMenuTweenPositions = controller.FetchComponentList<TweenPosition>(new string[3]{
                "Edge/TopLeftAnchor/TopLeft", "Edge/TopRightAnchor/TopRight", "Edge/Right/RightFuncPanel"});

            Trans = controller.FetchComponentList<Transform>(new string[3]{
                "Edge/Right/RightFuncPanel", "Edge/Right/RightFuncPanel/Content/Btn/Left", "Edge/Right/RightFuncPanel/Content/Campaign"});

            TopRightBtnList = controller.FetchComponentList<TweenPosition>(new string[10]{
				"Edge/TopRightAnchor/TopRight/FuncList/Privilege", "Edge/TopRightAnchor/TopRight/FuncList/Store", "Edge/TopRightAnchor/TopRight/FuncList/ChargeStore",
				"Edge/TopRightAnchor/TopRight/FuncList/Activity", "Edge/TopRightAnchor/TopRight/FuncList/Welfare", "Edge/TopRightAnchor/TopRight/FuncList/SignIn",
                "Edge/TopRightAnchor/TopRight/FuncList/HolidayActivity", "Edge/TopRightAnchor/TopRight/FuncList/ChargeGift", "Edge/TopRightAnchor/TopRight/FuncList/ComeBack",
				"Edge/TopRightAnchor/TopRight/FuncList/InviteFriend"
			});

            TopSliderBtnList = controller.FetchComponentList<GameObject>(new string[8]{ "Edge/Right/RightFuncPanel/Content/Function/Lottery",
                "Edge/TopRightAnchor/TopRight/FuncList/Store", "Edge/TopRightAnchor/TopRight/FuncList/Welfare",
                "Edge/TopRightAnchor/TopRight/FuncList/SignIn", "Edge/TopRightAnchor/TopRight/FuncList/ChargeStore",
                "Edge/TopRightAnchor/TopRight/FuncList/Privilege", "Edge/TopRightAnchor/TopRight/FuncList/ComeBack",
                "Edge/TopRightAnchor/TopRight/FuncList/InviteFriend",
            }, true);

            controller.BindingBtnEvent(new List<string>() {  "DailyButton" }, new List<EventDelegate>()
            {
                new EventDelegate(ToLTDailyScreen)
            });

            controller.FindAndBindingBtnEvent(BtnPathList, new List<EventDelegate>
            {
                new EventDelegate(ToInventoryScreen), new EventDelegate(ToFriendView), new EventDelegate(ToLegion), new EventDelegate(ToLTNationView),
                new EventDelegate(ToRankList),new EventDelegate(ToPartnerHandbook),new EventDelegate(OnMoreBtnClick),new EventDelegate(ToChargeGiftRewardScreen),
                new EventDelegate(ToPrivilegeScreen), new EventDelegate(ToHolidayActHudScreen), new EventDelegate(ToChargeStoreScreen),
                new EventDelegate(ToSigninScreen), new EventDelegate(ToWelfareScreen), new EventDelegate(OnStoreBtnClick), new EventDelegate(ToLotteryView),
                new EventDelegate(ToLTVIPHud),new EventDelegate(ToInviteScreen), new EventDelegate(ToLTComeBackHud), new EventDelegate(ToLTTestGMView), new EventDelegate(ToMailView), new EventDelegate(ToLTGameSettingView),
                new EventDelegate(ToTaskScreen), new EventDelegate(ToLTPartnerScreen), new EventDelegate(ToDungeonScreen),
                new EventDelegate(t.GetMonoILRComponent<NpcColliderUI>("Edge/Bottom/NPCFuncList").OnRightSwithBtnClick),
                new EventDelegate(worldBossRankLogic.OnRankCancelBtnClick), new EventDelegate(worldBossRankLogic.OnRewardBtnClick),
                new EventDelegate(worldBossRankLogic.OnRankBtnClick),new EventDelegate(worldBossRankLogic.OnBattleReadyBtnClick),
				new EventDelegate(OnChatBtnClick),//SetChatMiniPreviewVisibility)不再使用小弹窗
                new EventDelegate(ToPromotionView),new EventDelegate(ToActivityScreen)
            });

			t.GetComponent<ConsecutiveClickCoolTrigger>("Edge/TopRightAnchor/TopRight/Btn").clickEvent.Add(new EventDelegate(OnIconBtnClick));
			t.GetComponent<UIButton>("Edge/TopLeftAnchor/TopLeft/VipLevel/Icon").onClick.Add(new EventDelegate(ToLTVIPRewardHud));

            if (controller.TweenPositions["LeftSecondMenuTweenPos"] == null || TopRightBtnList == null ||
                controller.TweenPositions["TopLeftMoreBtn"] == null || controller.GObjects["GMToolObj"] == null)
            {
                EB.Debug.LogError("[{0}]为什么这个东西为空呢？LeftSecondMenuTweenPos", Time.frameCount);
                return;
            }

            if (ILRDefine.DEBUG || ILRDefine.USE_GM)
                controller.GObjects["GMToolObj"].gameObject.CustomSetActive(true);
            else
                controller.GObjects["GMToolObj"].gameObject.CustomSetActive(false);

            enableFuncList = new bool[MenuFuncId.Length];
            if (LeftSecondMenuGrid == null)
                LeftSecondMenuGrid = controller.TweenPositions["LeftSecondMenuTweenPos"].GetComponent<UIGrid>();

            UpdateEnableList();

            for (int i = 0; i < TopRightBtnList.Count; i++)
                TopRightBtnList[i].gameObject.CustomSetActive(false);

            controller.TweenPositions["TopLeftMoreBtn"].gameObject.CustomSetActive(false);
            LeftSecondMenuGrid.gameObject.CustomSetActive(false);

        }

        public override void Start()
        {
            base.Start();
            Instance = this;
            SetUserJudgeLevel();
            if (LTMainHudManager.Instance.GetFromFirstBattleType()) SetGuideBG();

            AutoRefreshingManager.Instance.AddWorldBossRollingMsg();

            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.MailUpdateEvent, MailUpdateListenter);

            Hotfix_LT.Messenger.AddListener<bool>(EventName.FriendApplyEvent, OnFriendApplyListener);
            Hotfix_LT.Messenger.AddListener<long>(EventName.FriendMessageEvent, OnFriendMessageListener);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Chat, SetChatRP);

            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.setting, SetSettingRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.expedition, SetExpeditionRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.handbook, SetHandBookRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.task, SetTaskRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.chargegift, SetChargeGiftRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.chargeprivilege, SetChargePrivilegeRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.drawcard, SetDrawCardRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.bag, SetBagRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.partner, SetPartnerRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.legion, SetLegionRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.welfare, SetWelfareRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.sign, SetSignInRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.dayactivity, SetDailyRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.comeback, SetComeBackRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.invite, SetInviteRP);
			LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.vipgift, SetVIPRewardRP);
            LTRedPointSystem.Instance.AddRedPointNodeCallBack(RedPointConst.promotion, SetPromotionRP);
        }


        //未添加
        //public GameObject NationRedPointObj;
        private void SetChatRP() { controller.GObjects["ChatRPObj"].CustomSetActive(LTChatManager.GetMainHudRP()); }
        private void SetSettingRP(RedPointNode node) { controller.GObjects["SettingRPObj"].CustomSetActive(node.num > 0); }
        private void SetExpeditionRP(RedPointNode node) { controller.GObjects["ExpeditionRedPointObj"].CustomSetActive(node.num > 0); }
        private void SetHandBookRP(RedPointNode node) { controller.GObjects["HandBookRedPointObj"].CustomSetActive(node.num > 0); }
        private void SetTaskRP(RedPointNode node) { controller.GObjects["TaskRedPoint"].CustomSetActive(node.num > 0); }
        private void SetChargePrivilegeRP(RedPointNode node) { controller.GObjects["ChargePrivilegeRedPoint"].CustomSetActive(node.num > 0); }
        private void SetChargeGiftRP(RedPointNode node) { controller.GObjects["ChargeGiftRedPoint"].CustomSetActive(node.num > 0); }
        private void SetDrawCardRP(RedPointNode node) { controller.GObjects["DrawCardRedPointObj"].CustomSetActive(node.num > 0); }
        private void SetBagRP(RedPointNode node) { controller.GObjects["BagRedPointObj"].CustomSetActive(node.num > 0); }
        private void SetPartnerRP(RedPointNode node) { controller.GObjects["PartnerRedPointObj"].CustomSetActive(node.num > 0); }
        private void SetLegionRP(RedPointNode node) { controller.GObjects["LegionRedPointObj"].CustomSetActive(node.num > 0); }
        private void SetWelfareRP(RedPointNode node) { controller.GObjects["WelfareRPObj"].CustomSetActive(node.num > 0); }
        private void SetSignInRP(RedPointNode node) { controller.GObjects["SignInRPObj"].CustomSetActive(node.num > 0); }
        private void SetDailyRP(RedPointNode node) { controller.GObjects["DailyRPObj"].CustomSetActive(node.num > 0); }
        private void SetComeBackRP(RedPointNode node) { controller.GObjects["ComebackRPObj"].CustomSetActive(node.num > 0); }
        private void SetInviteRP(RedPointNode node) { controller.GObjects["InviteRPObj"].CustomSetActive(node.num > 0); }
		private void SetVIPRewardRP(RedPointNode node) { controller.GObjects["VIPRewardRPObj"].CustomSetActive(node.num > 0); }
        private void SetPromotionRP(RedPointNode node) { controller.GObjects["PromotionRPObj"].CustomSetActive(node.num > 0); }


        public override void OnDestroy()
        {
            Instance = null;
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.MailUpdateEvent, MailUpdateListenter);

            Hotfix_LT.Messenger.RemoveListener<bool>(EventName.FriendApplyEvent, OnFriendApplyListener);
            Hotfix_LT.Messenger.RemoveListener<long>(EventName.FriendMessageEvent, OnFriendMessageListener);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Chat, SetChatRP);

            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.setting, SetSettingRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.expedition, SetExpeditionRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.handbook, SetHandBookRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.task, SetTaskRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.chargegift, SetChargeGiftRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.chargeprivilege, SetChargePrivilegeRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.drawcard, SetDrawCardRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.bag, SetBagRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.partner, SetPartnerRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.legion, SetLegionRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.welfare, SetWelfareRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.sign, SetSignInRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.dayactivity, SetDailyRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.comeback, SetComeBackRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.invite, SetInviteRP);
			LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.vipgift, SetVIPRewardRP);
            LTRedPointSystem.Instance.RemoveRedPointNodeCallBack(RedPointConst.promotion, SetPromotionRP);
            base.OnDestroy();
        }

        public override void OnEnable()
        {
            RegisterMonoUpdater();

            //将主城相机的渲染层调整
            for (int i = 0; i < Camera.allCameras.Length; i++)
            {
                //主城的相机
                PlayerCameraComponent cameraComponent = Camera.allCameras[i].GetComponent<PlayerCameraComponent>();
                if (cameraComponent != null)
                {
                    Camera.allCameras[i].cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
                    //将这个字段设置不可使用~要不相机会被移动到别的地方,by:WWH 20190523
                    cameraComponent._isCinematicActive = false;
                    cameraComponent._isStillCamActive = false;
                }
            }
            //调整左边按钮控件位置
            AdjustLeftButtonPosition();
            ShowMainMenuTP(true);
			v_OnEnableCallback?.Invoke();
		}

        /// <summary>
        /// 调整左边按钮控件位置
        /// </summary>
        private void AdjustLeftButtonPosition()
        {
            float rate = (float)Screen.width / Screen.height;
            if (rate >= 1.9f)
            {
                controller.UiWidgets["TopLeftAnchor"].leftAnchor.absolute = 140;
                controller.UiWidgets["TopLeftAnchor"].updateAnchors = UIRect.AnchorUpdate.OnEnable;
            }
            else
            {
                controller.UiWidgets["TopLeftAnchor"].leftAnchor.absolute = 0;
            }
        }

        public override void OnDisable()
        {
            ShowMainMenuTP(false);
        }

        private void ShowMainMenuTP(bool isShow)
        {
            if (isShow)
            {
                for (int i = 0; i < MainMenuTweenPositions.Count; i++)
                {
                    MainMenuTweenPositions[i].ResetToBeginning();
                    MainMenuTweenPositions[i].PlayForward();
                }
            }
            else
            {
                for (int i = 0; i < MainMenuTweenPositions.Count; i++)
                {
                    MainMenuTweenPositions[i].ResetToBeginning();
                }
            }
        }

        public static void SetActiveFromILR(bool isActive)
        {
            if (Instance != null)
            {
                Instance.controller.gameObject.SetActive(isActive);
            }
        }

        private void SetGuideBG(int req = 0)
        {
            if (req == 0)
            {
                int Index = UnityEngine.Random.Range(1, 6);
                controller.TextureCmps["BGTexture"].spriteName = string.Format("Game_Background_{0}", Index);
                controller.TextureCmps["BGTexture"].transform.parent.gameObject.CustomSetActive(true);
            }
            else
            {
                if (controller.TextureCmps["BGTexture"].GetComponent<TweenAlpha>() != null)
                {
                    TweenAlpha TA = controller.TextureCmps["BGTexture"].GetComponent<TweenAlpha>();
                    if (TA.onFinished.Count == 0) TA.AddOnFinished(HideGuideBG);
                    TA.ResetToBeginning();
                    TA.PlayForward();
                }
                else
                {
                    HideGuideBG();
                }
            }
        }

        public void HideGuideBG()
        {
            controller.TextureCmps["BGTexture"].transform.parent.gameObject.CustomSetActive(false);
            controller.TextureCmps["BGTexture"].spriteName = string.Empty;
        }

        public void ShowGO(bool isShow)
        {
            controller.gameObject.CustomSetActive(isShow);
        }

        public override IEnumerator OnAddToStack()
        {
            var coroutine = EB.Coroutines.Run(base.OnAddToStack());
			// 更新头像
            HeadIconUpdate();
			// 更新队伍战力
            LTFormationDataManager.MainTeamPower = LTPartnerDataManager.Instance.GetAllPower();
            onCombatTeamPowerUpdate(LTFormationDataManager.MainTeamPower);
			// 新开主界面处理之前程序在主界面缓存的预处理界面
            OpenCacheMenu();			
			// 播弹出菜单的动画
			if (LTMainHudManager.topListState)
            {
                SetFirstMenu(LTMainHudManager.topListState);
            }
            //需要新的图鉴刷新方式
            //HandBookRP.SetRedPoint(LTPartnerHandbookManager.Instance != null && LTPartnerHandbookManager.Instance.HasHandBookRedPoint());//TODORP
            yield return coroutine;
			// LTMainHudManager.Instance.SetDefeatSkipPanel();
			// 更新限时菜单按钮
            ReflashLimitedTimeGiftInfo();
			// 添加推送消息
            Hotfix_LT.Messenger.AddListener("OnOffersFetchSuceeded", ReflashLimitedTimeGiftInfo);
            Hotfix_LT.Messenger.AddListener<int>(EventName.onCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
            Hotfix_LT.Messenger.AddListener(EventName.LegionTechSkillLevelUp, OnShowPowerChange);
            Hotfix_LT.Messenger.AddListener<int,bool>(EventName.OnRefreshAllPowerChange, OnRefreshAllPowerChange);          
            //yield return null;
            //if (LTDailyDataManager.Instance.IsDailyDataInit)
            //    InitMainUIDaily();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener("OnOffersFetchSuceeded", ReflashLimitedTimeGiftInfo);
            Hotfix_LT.Messenger.RemoveListener<int>(EventName.onCombatTeamPowerUpdate, onCombatTeamPowerUpdate);
            Hotfix_LT.Messenger.RemoveListener(EventName.LegionTechSkillLevelUp, OnShowPowerChange);
            Hotfix_LT.Messenger.RemoveListener<int, bool> (EventName.OnRefreshAllPowerChange, OnRefreshAllPowerChange);
            StopAllCoroutines();
            if (eventSequence != -1)
            {
                ILRTimerManager.instance.RemoveTimer(eventSequence);
                eventSequence = -1;
            }
            isInitDailyTimer = false;
            DestroySelf();
            yield break;
        }

        public override void OnFocus()
        {
            base.OnFocus();

            RenderSettingsManager.Instance.SetActiveRenderSettings("s001a_RenderSettings");
            
            Hotfix_LT.Messenger.Raise(EventName.AllRedPointDataRefresh);
            if (!LTLegionWarManager.Instance.isInitQualityData)
            {
                LTLegionWarManager.Instance.GetQualifyData(SetDailyIcon);
            }
            else
            {
                SetDailyIcon();
            }
            isGuideCampaign = true;

            HeadIconUpdate();
            LTMainHudManager.Instance.CheckLevelUp(delegate { SetUserJudgeLevel(); });
            UpdateRedPoint();
            UpdatePlayerLevel();
            OnLevelChange();
            CheckDrawGift();
            CheckLevelCompensatedReward();

            //升级刷新体力提示
            VigourTip.ShowLevelupVigourTip();

			//// 新入栈界面已处理时，不需要在显示时再更新
			//if (!HasUpdateTopMenu)
			UpdateTopFunc();						
			
            UIInvitesCtrl.UpdateBtnState();

            JoystickCtrl.AddTouchListener();
            Hotfix_LT.Messenger.Raise(EventName.RedPoint_NPCFunc);

            ILRTimerManager.instance.AddTimer(600, 1, UpdateSignInState);

            LTMainHudManager.Instance.SetDefeatSkipPanel();
        }

        private void SetDailyIcon()
        {
            if (!LTDailyDataManager.Instance.IsDailyDataInit)
            {
                SetDefaultDailyData();
            }
            else if (!isInitDailyTimer || LTDailyDataManager.Instance.iSNeedRefreshDailyShow)
            {
                InitMainUIDaily();
            }
        }

        public override void OnBlur()
        {
            base.OnBlur();
            ResetGuideCampaign();
			//HasUpdateTopMenu = false;
            JoystickCtrl.RemoveTouchListener();
            ILRTimerManager.instance.RemoveTimer(UpdateSignInState);
        }

        public List<Transform> Trans;
        private bool isGuideCampaign = false;
        private float timer = 0;
        private int guideToCampaignLevel = 0;
        private int curLevel = 0;
        private int curGirdCount = 0;
        public int Grid1CountLimitNum = 10;
        private void ResetGuideCampaign()
        {
            controller.GObjects["GrideFxObj"].CustomSetActive(false);
            isGuideCampaign = false;
            timer = 0;
        }

        bool isFirst = true;
        private void ReflashLimitedTimeGiftInfo()
        {
			// 先更新顶部菜单的显示
			if (isFirst) { UpdateTopFunc(false); isFirst = false; }//仅需做一次，等一次数据刷新时做的，后续走onfocus

			LTChargeManager.Instance.ReflashLimitedTimeGiftInfo(true);
            if (controller != null)
            {
                controller.GObjects["ChargeGiftReward"].CustomSetActive(LTChargeManager.Instance.HasLimitedTimeGift());
                controller.UiGrids["TopMenuGrid"].Reposition();
                //controller.UiGrids["TopMenuGrid2"].Reposition();
            }
        }

		private bool IsOverGrid1CountLimit()
		{
			List<Transform> iconlist = controller.UiGrids["TopMenuGrid"].GetChildList();
			List<Transform> iconlist2 = controller.UiGrids["TopMenuGrid2"].GetChildList();

			return iconlist.Count + iconlist2.Count > Grid1CountLimitNum;
		}

		private void ResetUIIconPos()
        {
            List<Transform> iconlist = controller.UiGrids["TopMenuGrid"].GetChildList();
            List<Transform> iconlist2 = controller.UiGrids["TopMenuGrid2"].GetChildList();
            if (controller.GObjects["VipBtn"].activeSelf == false) return;
            if (iconlist.Count + iconlist2.Count > Grid1CountLimitNum)
            {
				SpringPosition sp = controller.GObjects["VipBtn"].GetComponent<SpringPosition>();
				if (sp) UnityEngine.Object.Destroy(sp);

				if (iconlist2.Contains(controller.GObjects["VipBtn"].transform)) return;

				controller.GObjects["VipBtn"].transform.SetParent(controller.UiGrids["TopMenuGrid2"].transform);
                controller.GObjects["VipBtn"].transform.localScale = Vector3.one;
				controller.GObjects["VipBtn"].transform.localPosition = Vector3.zero;				

				 TweenPosition tp = controller.GObjects["VipBtn"].GetComponent<TweenPosition>();
				if (TopRightBtnList.Contains(tp)) TopRightBtnList.Remove(tp);
            }
            else if (iconlist.Count + iconlist2.Count <= Grid1CountLimitNum)
            {
                if (!iconlist2.Contains(controller.GObjects["VipBtn"].transform)) return;

                controller.GObjects["VipBtn"].transform.SetParent(controller.UiGrids["TopMenuGrid"].transform);
                controller.GObjects["VipBtn"].transform.SetAsFirstSibling();
                controller.GObjects["VipBtn"].transform.localScale = Vector3.one;				

				TweenPosition tp = controller.GObjects["VipBtn"].GetComponent<TweenPosition>();
				if (!TopRightBtnList.Contains(tp) && TopRightBtnList.Count < 10) TopRightBtnList.Add(tp);
			}
        }

        private bool JudgeLevel()
        {
            if (guideToCampaignLevel == 0)
            {
                int lv = (int)(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("GuideToCampaignLevel"));
                guideToCampaignLevel = lv == 0 ? 11 : lv;
            }
            return curLevel > guideToCampaignLevel;
        }
        private void SetUserJudgeLevel()
        {
            DataLookupsCache.Instance.SearchIntByID("level", out curLevel);
        }

        private FuncTemplate _ft;
        private string _maxChapterId;

        public void Update()
        {
            if (_ft == null)
            {
                _ft = FuncTemplateManager.Instance.GetFunc(10094);
            }

            if (string.IsNullOrEmpty(_maxChapterId))
            {
                _maxChapterId = LTInstanceUtil.GetMaxChapterId().ToString();
                LostMainChapterTemplate tpl = SceneTemplateManager.Instance.GetNextChapter(_maxChapterId);

                if (!string.IsNullOrEmpty(_maxChapterId) && tpl != null && LTInstanceUtil.IsChapterComplete(_maxChapterId))
                {
                    _maxChapterId = tpl.Id;
                }
            }

            if (_ft != null && !string.IsNullOrEmpty(_maxChapterId) && !GuideNodeManager.IsGuide && !_ft.IsConditionOK() &&
                LTInstanceUtil.GetIsChapterLimitConditionComplete(_maxChapterId, out int currNum) &&
                LTInstanceUtil.GetChapterIsPassByChapterId(_maxChapterId) &&
                LTInstanceUtil.HasUnfinishedCampaignBefore(_ft.conditionParam.ToString())&&
                LTWorldBossMainMenuCtrl.Instance != null && !LTWorldBossMainMenuCtrl.Instance.isOpen)
            {
                controller.GObjects["GrideFxObj"].transform.position = Trans[(Trans[0].localPosition.x > 0) ? 1 : 2].position;
                controller.GObjects["GrideFxObj"].CustomSetActive(true);
            }
            else
            {
                controller.GObjects["GrideFxObj"].CustomSetActive(false);
            }

            if (EB.Time.Now >= NextEventFinTime && NextEventFinTime != 0)
            {
                CheckEventList();
            }
        }

        IEnumerator IE_ClickLimit()//设置蒙版防止点穿
        {
            yield return new WaitForSeconds(0.5f);
        }

        private void OnFriendApplyListener(bool isShow)
        {
            if (controller.GObjects["FriendRedPoint"] != null)
            {
                controller.GObjects["FriendRedPoint"].gameObject.CustomSetActive(isShow);
            }
        }

        private void OnFriendMessageListener(long FromUid)
        {

            if (controller.GObjects["FriendRedPoint"] != null && FromUid != LoginManager.Instance.LocalUserId.Value)
                controller.GObjects["FriendRedPoint"].gameObject.CustomSetActive(true);
        }

        private void MailUpdateListenter()
        {
            UpdateMailNum();
        }

        private bool MoreBtnRPJudge(Transform target)
        {
            bool isRP = false;
            for (int i = 0; i < target.childCount; i++)
            {
                Transform tran = target.GetChild(i);
                if (tran.gameObject.activeSelf && tran.childCount > 2 && tran.GetChild(2).gameObject.activeSelf)
                {
                    isRP = true; break;
                }
            }
            return isRP;
        }



        private void UpdateRedPoint()//红点事件
        {
            UpdateMailNum();
            SetFriendRP();
        }

        private void SetFriendRP()
        {
            bool isFriendRedpoint = FriendManager.Instance.Info.IsHaveNewMessage || FriendManager.Instance.Info.ApplyCount > 0 || FriendManager.Instance.MyFriends.canReceiveVigor;
            controller.GObjects["FriendRedPoint"].gameObject.CustomSetActive(isFriendRedpoint);
        }

        void UpdateMailNum()
        {
            int mailNum = MailBoxManager.Instance.MailList.Mails.FindAll(m => m.Rewards.ItemCount <= 0 && !m.HasRead).Count + MailBoxManager.Instance.MailList.Mails.FindAll(m => m.Rewards.ItemCount > 0 && !m.HasReceived).Count;
            if (mailNum > 0)
            {
                controller.UiLabels["UnreadMailNumLabel"].transform.parent.gameObject.CustomSetActive(true);
                controller.UiLabels["UnreadMailNumLabel"].text = mailNum.ToString();
            }
            else
            {
                controller.UiLabels["UnreadMailNumLabel"].transform.parent.gameObject.CustomSetActive(false);
            }
        }

        void OpenCacheMenu()
        {
            if (DartResultController.sOpenFlag)
            {
                DartResultController.sOpenFlag = false;
                GlobalMenuManager.Instance.Open("LTShowDartResultHud");
            }
        }

        private void HeadIconUpdate()
        {
            controller.UiSprites["LeaderMainIcon"].spriteName = LTMainHudManager.Instance.UserHeadIcon;
        }

        private void onCombatTeamPowerUpdate(int power)
        {
            // LTUIUtil.SetText(CombatbatPowerLabel,);          
            LTUIUtil.SetText(CombatbatPowerLabel, power.ToString());
            
            //战力任务是否存在 
            TaskSystem.RequestCombatPowerFinish(power);
        }

        private void OnShowPowerChange()
        {
            LTPartnerDataManager.Instance.OnDestineTypePowerChanged(Data.eRoleAttr.None, (prm) =>
            {
                LTFormationDataManager.OnRefreshMainTeamPower(prm);
            }, true);
        }

        private void OnRefreshAllPowerChange(int type,bool showfloat)
        {
            LTPartnerDataManager.Instance.OnDestineTypePowerChanged((Data.eRoleAttr)type, (prm) =>
            {
                LTFormationDataManager.OnRefreshMainTeamPower(prm);
            }, showfloat);
        }


        public UISlider XpSlider;
        public void UpdatePlayerLevel()
        {
            int curXp = 0;
            DataLookupsCache.Instance.SearchIntByID("res.xp.v", out curXp);
            int level = 0;
            DataLookupsCache.Instance.SearchIntByID("level", out level);

            Hotfix_LT.Data.PlayerLevelTemplate thisInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetPlayerLevelInfo(level);
            curXp -= thisInfo.expRequirement;
            Hotfix_LT.Data.PlayerLevelTemplate nextInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetPlayerLevelInfo(level + 1);
            int maxXp = nextInfo.expRequirement - thisInfo.expRequirement;

            controller.UiSliders["XpSlider"].value = maxXp != 0 ? ((float)curXp / (float)maxXp) : 0;
        }

        /// <summary>
        /// 判断是否打开签到界面
        /// </summary>
        /// <param name="seq"></param>
        public void UpdateSignInState(int seq)
        {
            if (UpdateActivityNotice())
            {
                return;
            }

            Hotfix_LT.Data.FuncTemplate ft = null;
            ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10076);//签到开放
            if (!ft.IsConditionOK() || GuideNodeManager.IsRunGuide)
            {
                LTChargeManager.Instance.UpdateLimitedTimeGiftNotice();
                return;
            }

            if (UIStack.Instance.GetBackStackCount() == 1
                && !AllianceUtil.IsInTransferDart
                && LTMainHudManager.isFirstInGame
                && !GuideNodeManager.IsGuide
                && !LTWelfareDataManager.Instance.SignInData.IsSigned)
            {
                LTMainHudManager.isFirstInGame = false;
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTSigninHud");
            }
            else
            {
                LTChargeManager.Instance.UpdateLimitedTimeGiftNotice();
            }
        }

        /// <summary>
        /// 检测活动通知数据
        /// </summary>
        /// <returns></returns>
        public bool UpdateActivityNotice()
        {
            if (LTMainHudManager.isFirstActivityNotice)
            {
                LTMainHudManager.isFirstActivityNotice = false;
                Data.FuncTemplate ft = Data.FuncTemplateManager.Instance.GetFunc(10079);//活动开放

                if ((ft != null && !ft.IsConditionOK()) || GuideNodeManager.IsRunGuide)
                {
                    return false;
                }

                ArrayList activityList;
                DataLookupsCache.Instance.SearchDataByID("events.events", out activityList);
                var noticeList = new List<object>();

                if (activityList != null)
                {
                    for (var i = 0; i < activityList.Count; i++)
                    {
                        var item = activityList[i];
                        var state = EB.Dot.String("state", item, "");
                        var notifyBg = EB.Dot.String("notify_bg", item, "");

                        if (state == "running" && !string.IsNullOrEmpty(notifyBg))
                        {
                            noticeList.Add(item);
                        }
                    }
                }

                if (noticeList.Count > 0)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    GlobalMenuManager.Instance.Open("LTActivityNoticeHud", noticeList);
                    return true;
                }
            }

            return false;
        }

        private int eventSequence = -1;
        private void InitMainUIDaily()
        {
            isInitDailyTimer = true;
            //如果日常活动没有开启则不初始化
            if (!Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10074).IsConditionOK())
            {
                return;
            }
            
            mMainUIDailyData = new List<LTDailyData>();
            List<LTDailyData> tempList = LTDailyDataManager.Instance.GetDailyDataAll();
            if (tempList == null)
            {
                return;
            }
            for (int i = 0; i < tempList.Count; i++)
            {
                var func = Data.FuncTemplateManager.Instance.GetFunc(tempList[i].ActivityData.funcId);
                if (tempList[i].OpenTimeValue != 0 &&
                    (func==null||func.IsConditionOK()) &&
                    (tempList[i].OpenTimeWeek.Contains(EB.Time.LocalWeek.ToString()) || tempList[i].OpenTimeWeek.Contains("*")))
                {
                    mMainUIDailyData.Add(tempList[i]);
                }
            }

            if (mMainUIDailyData.Count > 0)
            {
                if (mMainUIDailyData.Count > 1)
                {
                    mMainUIDailyData.Sort((LTDailyData x, LTDailyData y) => { return x.OpenTimeValue - y.OpenTimeValue; });
                }

                DailyRefreshActTime(0);
            }
            //EB.Debug.LogError("RefreshDailyShow");
            LTDailyDataManager.Instance.iSNeedRefreshDailyShow = false;
            
        }

        private void DailyRefreshActTime(int timerSequence)
        {
            int tempTime = 0, time = 0, nextIndex = -1;
            string name;
            string spName;
            if (eventSequence != -1) LTDailyDataManager.Instance.iSNeedRefreshDailyData = true;
            if (LTDailyDataManager.Instance.GetDailyShowIconData(out CurrentDailyData))
            {
                int id = CurrentDailyData.ActivityData.id;
                spName = CurrentDailyData.ActivityData.icon;
                name = CurrentDailyData.ActivityData.display_name;
                for (int i = 0; i < mMainUIDailyData.Count; i++)
                {
                    if (id == mMainUIDailyData[i].ActivityData.id && i < mMainUIDailyData.Count - 1)
                    {
                        nextIndex = i + 1;
                        break;
                    }
                }
                if (nextIndex != -1)
                {
                    for (int i = nextIndex; i < mMainUIDailyData.Count; i++)
                    {
                        if (mMainUIDailyData[i].OpenTimeValue - EB.Time.Now > 0)
                        {
                            nextIndex = i;
                            break;
                        }
                        else
                        {
                            nextIndex = -1;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < mMainUIDailyData.Count; i++)
                    {
                        if (mMainUIDailyData[i].OpenTimeValue - EB.Time.Now > 0 && i < mMainUIDailyData.Count)
                        {
                            nextIndex = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                name = EB.Localizer.GetString("ID_codefont_in_LTMainMenuHudController_13242");
                spName = "Maininterface_Icons_Jiangli_Nor";
                for (int i = 0; i < mMainUIDailyData.Count; i++)
                {
                    if (mMainUIDailyData[i].OpenTimeValue - EB.Time.Now > 0 && i < mMainUIDailyData.Count)
                    {
                        nextIndex = i;
                        break;
                    }
                }
            }
            if (eventSequence != -1) ILRTimerManager.instance.RemoveTimer(DailyRefreshActTime);
            if (CurrentDailyData != null && nextIndex != -1)
            {
                if (CurrentDailyData.StopTimeValue == 0 || CurrentDailyData.StopTimeValue > mMainUIDailyData[nextIndex].OpenTimeValue)
                {
                    time = mMainUIDailyData[nextIndex].OpenTimeValue;
                }
                else
                {
                    time = CurrentDailyData.StopTimeValue;
                }
                tempTime = time - EB.Time.Now;

            }
            else if (CurrentDailyData != null && nextIndex == -1)
            {
                time = CurrentDailyData.StopTimeValue;
                tempTime = time - EB.Time.Now;
            }
            else if (CurrentDailyData == null && nextIndex != -1)
            {
                time = mMainUIDailyData[nextIndex].OpenTimeValue;
                tempTime = time - EB.Time.Now;
            }
            else if (CurrentDailyData == null && nextIndex == -1)
            {
                time = 0;
                tempTime = 0;
            }
            if (tempTime > 0)
            {
                eventSequence = ILRTimerManager.instance.AddTimer((tempTime+10) * 1000, 1, DailyRefreshActTime);
            }
            controller.UiLabels["DailyName"].text = name;
            controller.UiSprites["DailyIcon"].spriteName = spName;
            controller.UiButtons["DailyButton"].normalSprite = spName;
            controller.GObjects["DailyFxObj"].CustomSetActive(CurrentDailyData != null);
            //日常任务红点更新
            //SetDailyRP();TODORP
        }

        private void SetDefaultDailyData()
        {
            isInitDailyTimer = true;
            if (!Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10074).IsConditionOK())
            {
                return;
            }
            string name = EB.Localizer.GetString("ID_codefont_in_LTMainMenuHudController_13242");
            string spName = "Maininterface_Icons_Jiangli_Nor";
            controller.UiLabels["DailyName"].text = name;
            controller.UiSprites["DailyIcon"].spriteName = spName;
            controller.UiButtons["DailyButton"].normalSprite = spName;
            controller.GObjects["DailyFxObj"].CustomSetActive(CurrentDailyData != null);
           
        }

        /// <summary>
        /// 菜单
        /// </summary>
        #region 菜单对应id及UI物体
        private void OnLevelChange()
        {
            UpdateEnableList();
            if (LTMainHudManager.topListState && changeCount)
            {
                ReSetLeftListState();
                SetFirstMenu(true);
            }

            if (LTPromotionManager.Instance != null
                && !LTPromotionManager.Instance.HasPromotionBadgeInMainland()
                && LTPromotionManager.Instance.IsPromotionFunctionEnabled()) {
                LTPromotionManager.Instance.Api.SendPromotionFunctionIsEnabled(ht => {
                    DataLookupsCache.Instance.CacheData("promotion", null);
                    DataLookupsCache.Instance.CacheData(ht);
                    LTPromotionManager.Instance.SetPlayerPromotionBadge();
                });
            }
        }

        private void ReSetLeftListState()
        {
            for (int i = 0; i < TopRightBtnList.Count; i++)
            {
                TopRightBtnList[i].ResetToBeginning();
                TopRightBtnList[i].gameObject.CustomSetActive(false);
            }
        }

        //左侧菜单
        private static int[] MenuFuncId = {
            10100, 10054, 10057,10019, 10069,10058, 10049   //晋升，背包，好友，军团，国家,排行榜，图鉴
        };
        private bool[] enableFuncList;
        public List<TweenPosition> TopRightBtnList;
        private List<TweenPosition> FirstEnableList;
		private readonly string[] MenuFuncItems = {
            "Edge/Right/RightFuncPanel/Content/Function/Promotion",
            "Edge/Right/RightFuncPanel/Content/Function/Bag",
			"Edge/Bottom/Grid/Friend",
			"Edge/Right/RightFuncPanel/Content/Function/Legion",
			"Edge/TopLeftAnchor/TopLeft/Left/Nation",
			"Edge/Bottom/Grid/Rank",
			"Edge/Right/RightFuncPanel/Content/Function/HandBook"
		};

        private UIGrid LeftSecondMenuGrid;

        public void OnIconBtnClick()//头像点击
        {
            LTMainHudManager.topListState = !LTMainHudManager.topListState;
            SetFirstMenu(LTMainHudManager.topListState);
        }
        private int distance_x = 170;//顶部第一栏菜单之间的距离
        private int funcIndex = -1;
        private int firstMenuMaxCount = 10;//左边第一栏菜单最大数量

		private void UpdateMenuItemActivities()
		{
			if(enableFuncList != null && enableFuncList.Length > 0)
			{
				for ( int i = 0; i < enableFuncList.Length; i++ )
				{
					if (i < MenuFuncItems.Length)
					{
						string menuName = MenuFuncItems[i];
						Transform menu = controller.transform.Find(menuName);
						if(menu)
						{
							menu.gameObject.SetActive(enableFuncList[i]);
						}
					}
				}
			}
		}

        private void SetFirstMenu(bool isState)
        {
			//Debug.Log("SetFirstMenu = " + isState);
            changeCount = false;
			UIGrid grid = controller.Transforms["FirstParent"].GetComponent<UIGrid>();

			// 更新除顶部菜单以外菜单的显示
			UpdateMenuItemActivities();

			controller.Transforms["IconBtnTipUI"].localRotation = Quaternion.Euler(new Vector3(0, 0, (isState) ? 90 : -90));
            if (isState)
            {				
				controller.GObjects["IconBtnRP"].CustomSetActive(false);
                
                FirstEnableList = new List<TweenPosition>();
                funcIndex = -1;

				FetchFirstMenuList();
            }
            else
            {
				if(FirstEnableList != null && FirstEnableList.Count <= 0)FetchFirstMenuList();

				funcIndex = FirstEnableList.Count;
                controller.GObjects["IconBtnRP"].CustomSetActive(MoreBtnRPJudge(controller.Transforms["FirstParent"]));
            }
			if (grid != null) grid.enabled = false;

			OnShowFunc();
        }

		private void FetchFirstMenuList()
		{
			int pos_x = 170;
			DataLookupsCache.Instance.SearchDataByID<bool>("isOpenEC", out bool isOpen);

			for (int i = 0; i < TopRightBtnList.Count; i++)
			{
				bool partCondition = GetParticularCondition(i, isOpen, out FuncTemplate ft);

				if ((IsFuncTemplateValid(ft, partCondition) && TopRightBtnList[i].gameObject.activeSelf) || TopRightBtnList[i].gameObject.activeSelf)
				{
					TopRightBtnList[i].from.x = pos_x;
					pos_x -= distance_x;
					TopRightBtnList[i].to.x = pos_x;
					TopRightBtnList[i].SetOnFinished(OnShowFunc);
					//TopRightBtnList[i].transform.SetParent(controller.Transforms["FirstParent"]);
					FirstEnableList.Add(TopRightBtnList[i]);
				}			
			}
		}

		private void SetMenuItemFeasibility(GameObject @object, bool value)
		{
			UIWidget widget = @object.GetComponent<UIWidget>();
			if (widget) widget.alpha = value ? 1 : 0;
			BoxCollider collider = @object.GetComponent<BoxCollider>();
			if (collider) collider.enabled = value;
            GameObject fx = @object.FindEx("Fx",false);
            if (fx) fx.CustomSetActive(value);			
		}

        private void OnFuncPlay(int index)
        {
			if (FirstEnableList.Count == index)
			{
				UIGrid grid = controller.Transforms["FirstParent"].GetComponent<UIGrid>();
				if (grid) grid.enabled = true;

				if(IsOverGrid1CountLimit()) SetMenuItemFeasibility(controller.GObjects["VipBtn"].gameObject, true);
				return;
			}

			if (index > FirstEnableList.Count || index < 0) return;
			SetMenuItemFeasibility(FirstEnableList[index].gameObject, true);
			FirstEnableList[index].PlayForward();
        }

        private void OnFuncClose(int index)
        {
			if (index + 1 != FirstEnableList.Count)
			{
				SetMenuItemFeasibility(FirstEnableList[index + 1].gameObject, false);
			}

			if (index == 0 && IsOverGrid1CountLimit()) SetMenuItemFeasibility(controller.GObjects["VipBtn"].gameObject, false);

			if (index >= 0) FirstEnableList[index].PlayReverse();
        }

        private void OnShowFunc()
        {
            if (LTMainHudManager.topListState)
            {
                funcIndex++;
                OnFuncPlay(funcIndex);
            }
            else
            {
                if (funcIndex < 0) return;
				funcIndex--;
                OnFuncClose(funcIndex);
            }			
		}

        public void OnMoreBtnClick()//"+"键点击
        {
            LTMainHudManager.rightListState = !LTMainHudManager.rightListState;
            SetSecondMenu(LTMainHudManager.rightListState);
        }
        private void SetSecondMenu(bool isState)
        {
            if (isState)
            {
                controller.TweenPositions["TopLeftMoreBtn"].transform.localRotation = Quaternion.Euler(Vector3.zero);
                controller.GObjects["AdditionBtnRP"].CustomSetActive(false);
                controller.TweenPositions["LeftSecondMenuTweenPos"].SetOnFinished(Empty);
                controller.TweenPositions["LeftSecondMenuTweenPos"].gameObject.CustomSetActive(true);
                LeftSecondMenuGrid.Reposition();
                controller.TweenPositions["LeftSecondMenuTweenPos"].PlayForward();
            }
            else
            {
                controller.TweenPositions["TopLeftMoreBtn"].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -45));
                controller.GObjects["AdditionBtnRP"].CustomSetActive(MoreBtnRPJudge(LeftSecondMenuGrid.transform));
                if (controller.TweenPositions["LeftSecondMenuTweenPos"].gameObject.activeSelf)
                {
                    controller.TweenPositions["LeftSecondMenuTweenPos"].PlayReverse();
                    controller.TweenPositions["LeftSecondMenuTweenPos"].SetOnFinished(HideSecondMenu);
                }
            }
        }

        private void HideSecondMenu()
        {
            LeftSecondMenuGrid.Reposition();
            controller.TweenPositions["LeftSecondMenuTweenPos"].gameObject.CustomSetActive(false);
        }
        private void Empty()
        {

        }

        private bool changeCount = false;
        private int enableListCount = 0;
        private void UpdateEnableList()
        {
            int curCount = 0;

            if (MenuFuncId != null && MenuFuncId.Length > 0)
            {
                for (int i = 0; i < MenuFuncId.Length; i++)
                {
                    Data.FuncTemplate ft = Data.FuncTemplateManager.Instance.GetFunc(MenuFuncId[i]);
                    enableFuncList[i] = (ft != null) ? ft.IsConditionOK() : false;

                    if (enableFuncList[i])
                    {
                        curCount++;
                    }
                }
            }

            if (enableListCount == 0)
            {
                enableListCount = curCount;
            }
            else if (enableListCount < curCount)
            {
                enableListCount = curCount;
                changeCount = true;
            }
        }


        //顶部菜单
        public List<GameObject> TopSliderBtnList;
        private int FlagAreaFuncId = 10074;
        private int[] TopMenuFuncId = {
            10011,10013,10075,10076,10078,10078,10092,10093 //抽奖,商店,福利，签到，商城，特权,回归,邀请
        };
        private int ActivityFuncId = 10079;//，活动(10079单独走一条)

        private int NextEventFinTime = 0;
        public void RefreshEvent()
        {
            if (controller.IsOpen())
            {
                CheckEventList();
            }
        }
		//public bool HasUpdateTopMenu = false;

        private void CheckEventList()
        {
            Hotfix_LT.Data.FuncTemplate ft = null;
            ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(ActivityFuncId);
            if (ft.IsConditionOK())
            {
                bool hasEvent = false;
                bool hasEventRP = false;
                bool hasHoliday = false;
                bool hasHolidayRP = false;
                ArrayList eventlist;
                DataLookupsCache.Instance.SearchDataByID("events.events", out eventlist);
                NextEventFinTime = 0;

                if (eventlist != null)
                {
                    for (var i = 0; i < eventlist.Count; i++)
                    {
                        var e = eventlist[i];
                        int displayUntil = EB.Dot.Integer("displayuntil", e, 0);
                        if (EB.Time.Now >= displayUntil) continue;

                        if (NextEventFinTime == 0 || displayUntil < NextEventFinTime)
                        {
                            NextEventFinTime = displayUntil;
                        }

                        int sort_order = EB.Dot.Integer("sort_order", e, 0);

                        if (!hasEvent||!hasHoliday)
                        {
                            int nav_type = EB.Dot.Integer("nav_type", e, 0);
                            if (nav_type == 0)
                            {
                                int aid = EB.Dot.Integer("activity_id", e, 0);
                                Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(aid);
                                if (activity == null) return;
                                nav_type = (int)activity.nav_type;
                            }
                            
                            if (nav_type >= 3)
                            {
                                if (sort_order == 0)
                                {
                                    hasEvent = true;
                                }
                                else
                                {
                                    hasHoliday = true;
                                }
                            }
                        }

                        if (sort_order == 0)
                        {
                            if (!hasEventRP)
                            {
                                hasEventRP = LTMainHudManager.Instance.CheckEventRedPoint(e);
                            }
                        }
                        else
                        {
                            if (!hasHolidayRP)
                            {
                                hasHolidayRP = LTMainHudManager.Instance.CheckEventRedPoint(e);
                            }
                        }

                        if (hasEventRP && hasEvent&& hasHoliday&& hasHolidayRP) break;
                    }
                }
                if (hasEvent)
                {
                    controller.GObjects["EventIcon"].CustomSetActive(true);
                    var RedPoint = controller.GObjects["EventIcon"].FindEx("RedPoint");
                    if (hasEventRP)
                    {
                        RedPoint.CustomSetActive(true);
                    }
                    else
                    {
                        RedPoint.CustomSetActive(false);
                    }
                }
                else
                {
                    controller.GObjects["EventIcon"].CustomSetActive(false);
                }
                if (hasHoliday)
                {
                    controller.GObjects["HolidayActivity"].CustomSetActive(true);
                    var RedPoint = controller.GObjects["HolidayActivity"].FindEx("RedPoint");
                    if (hasHolidayRP)
                    {
                        RedPoint.CustomSetActive(true);
                    }
                    else
                    {
                        RedPoint.CustomSetActive(false);
                    }
                }
                else
                {
                    controller.GObjects["HolidayActivity"].CustomSetActive(false);
                }
            }
            else
            {
                controller.UiButtons["EventIcon"].gameObject.CustomSetActive(false);
                controller.UiButtons["HolidayActivity"].gameObject.CustomSetActive(false);
            }

            controller.UiGrids["TopMenuGrid"].Reposition();
        }

		private bool GetParticularCondition(int i, bool isOpen, out FuncTemplate ft)
		{
			bool otherCondition = true;

			if (i < TopMenuFuncId.Length)
			{
				ft = FuncTemplateManager.Instance.GetFunc(TopMenuFuncId[i]);
				if (TopMenuFuncId[i] == 10075)
				{
					otherCondition = false;
					for (int j = 1; j < 7; ++j)
					{
						if (!LTWelfareModel.Instance.JudgeViewClose(j))
						{
							otherCondition = true;
							break;
						}
					}
				}
				if (TopSliderBtnList[i].name.Equals("Privilege"))//特权屏蔽
				{
					if (!isOpen)
					{
						otherCondition = false;
					}
				}
				if (TopMenuFuncId[i] == 10092)
				{
					otherCondition = LTWelfareModel.Instance.GetOpenComeBack();
				}
			}
			else
			{
				ft = null;
			}
			return otherCondition;
		}

		private bool IsFuncTemplateValid(Hotfix_LT.Data.FuncTemplate ft, bool otherCondition)
		{
			return (ft != null && ft.IsConditionOK() || ft == null) && otherCondition;
		}

		/// <summary>
		/// 更新顶部按钮状态
		/// </summary>
		public void UpdateTopFunc(bool doReposition = true)
        {
            bool isOpen = true;
            DataLookupsCache.Instance.SearchDataByID<bool>("isOpenEC", out isOpen);

            CheckEventList();
            Hotfix_LT.Data.FuncTemplate FlagAreaft = null;
            FlagAreaft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(FlagAreaFuncId);
            controller.GObjects["FlagArea"].CustomSetActive((FlagAreaft != null && FlagAreaft.IsConditionOK() || FlagAreaft == null) && !LTWorldBossMainMenuCtrl.Instance.isOpen);
            controller.GObjects["ChargeGiftReward"].CustomSetActive(LTChargeManager.Instance.HasLimitedTimeGift());

            if (LTMainHudManager.topListState)
            {
                for (int i = 0; i < TopSliderBtnList.Count; i++)
                {
                    bool partCondition = GetParticularCondition(i, isOpen, out FuncTemplate ft);

                    TopSliderBtnList[i].CustomSetActive(IsFuncTemplateValid(ft, partCondition));
                }
            }
            
            controller.GObjects["VipBtn"].CustomSetActive(isOpen && LTChargeManager.CumulativeRecharge >= 1000 * 100);
			ResetUIIconPos();
			if(doReposition)
			{
				//controller.UiGrids["TopMenuGrid2"].Reposition();
				controller.UiGrids["TopMenuGrid"].Reposition();
                controller.UiGrids["LeftBottomGrid"].Reposition();
            }

			// 更新右下角菜单的位置
			UIGrid grid = controller.transform.Find("Edge/Right/RightFuncPanel/Content/Function").GetComponent<UIGrid>();
			if (grid) grid.Reposition();

			//判断活动旗帜是否显示
			if(controller.gameObject.activeSelf)
			{
				bool dailyShown = controller.GObjects["FlagArea"].activeSelf || LTWorldBossMainMenuCtrl.Instance.isOpen;
				Transform popupBtnNode = controller.UiGrids["TopMenuGrid"].transform.parent.Find("Btn");
				Transform menuRootNode = controller.UiGrids["TopMenuGrid"].transform;
				Transform menuRootNode2nd = controller.UiGrids["TopMenuGrid2"].transform;
				popupBtnNode.localPosition = dailyShown ? new Vector3(90, 9, 0) : new Vector3(380, 9, 0);
				menuRootNode.localPosition = dailyShown ? new Vector3(-60, 20, 0) : new Vector3(230, 20, 0);
				menuRootNode2nd.localPosition = dailyShown ? new Vector3(-90, -180, 0) : new Vector3(200, -180, 0);
			}
		}

        /// <summary>
        /// 检测是否需要弹出伙伴培养礼包界面
        /// </summary>
        private void CheckDrawGift()
        {
            if (!GuideNodeManager.IsGuide && LTChargeManager.Instance.IsShowDrawGift)
            {
                ArrayList array;
                DataLookupsCache.Instance.SearchDataByID("userCultivateGift.gifts", out array);
                if (array != null && array.Count > 0)
                {
                    GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI");
                }
                else
                {
                    LTChargeManager.Instance.IsShowDrawGift = false;
                }
            }
        }

        /// <summary>
        /// 免费等级补偿奖励领取检测
        /// </summary>
        private void CheckLevelCompensatedReward()
        {
            if (LTMainHudManager.isCheckLevelCompensated)
            {
                int id;
                DataLookupsCache.Instance.SearchDataByID("user_prize_data.level_compensated", out id);
                if (!GuideNodeManager.IsGuide && Data.EventTemplateManager.Instance.IsFreeLevelCompenstatedRewardId(id))
                {
                    GlobalMenuManager.Instance.Open("LevelCompensatedRewardHud", id);
                }
                LTMainHudManager.isCheckLevelCompensated = false;
            }
        }

        #endregion


        public void ToInventoryScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("InventoryView");
        }

        public void ToActivityScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTActivityHud");
        }

        public void ToTaskScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("NormalTaskView", null);
        }
        
        public void ToDungeonScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            if (GuideNodeManager.IsGuide)
            {
                GuideSpecialFunc();
            }
            else
            {
                GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
            }
        }

        public void GuideSpecialFunc()
        {
            bool hasGetFirstBox = false;
            Hotfix_LT.Data.LostMainChapterTemplate temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById("101");
            int boxKey = 0;
            string boxValue = "101";
            foreach (var boxTemp in temp.RewardDataDic)
            {
                boxKey = boxTemp.Key;
            }
            Hotfix_LT.Data.LostMainChapterTemplate m_ChapterData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById("101");
            string flagStr = PlayerPrefs.GetString(LoginManager.Instance.LocalUserId.Value + m_ChapterData.BeforeChapter);
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userCampaignStatus.normalChapters.{0}.box.{1}", boxValue, boxKey), out hasGetFirstBox);
            if (hasGetFirstBox)//是否领取了第一章通关奖励
            {
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
            }
            else if (!string.IsNullOrEmpty(m_ChapterData.BeforeChapter) && string.IsNullOrEmpty(flagStr))
            {
                LTStoryController.OpenStory(GuideFunc);
                PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value + m_ChapterData.BeforeChapter, "True");//本地临时保存，等服务器做好了会保存到服务器
                PlayerPrefs.Save();
                return;
            }
            else
            {
                GuideFunc();
            }
        }

        public void GuideFunc()
        {
            System.Action act = () =>
            {
                LTMainInstanceHudController.EnterInstance("101");
            };
            UIStack.Instance.ShowLoadingScreen(act, false, true, true);
        }

        public void OnBtnClickWithAnimation(GameObject btn)
        {
            if (btn.GetComponent<TweenScale>() != null)
            {
                btn.GetComponent<TweenScale>().ResetToBeginning();
                btn.GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                EB.Debug.LogError("can not find tween animation, please set first");
            }
        }

		public void SetChatMiniPreviewVisibility()
		{
			Transform coord = controller.transform.Find("Edge/Bottom/Chat");
			if (coord)
			{
				UIWidget widget = coord.GetComponent<UIWidget>();
				widget.alpha = widget.alpha > 0.5f ? 0 : 1;
			}
		}

        public void OnChatBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("ChatHudView", null);
        }

        public void ToLTPartnerScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTPartnerView", HudType.LEFT);
        }

        public void ToLTPartnerCultivateScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTPartnerView", HudType.RIGHT);
        }

        public void ToLotteryView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTDrawCardTypeUI");
        }

        public void ToMailView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("MailView");
        }

        public void ToFriendView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("FriendHud", null);
        }

        public void ToRankList()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTRankListHud", "Level");
        }

        public void ToPartnerHandbook()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            Hotfix_LT.Data.FuncTemplateManager.OpenFunc(10049);
        }

        public void ToArena()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            Hotfix_LT.Data.FuncTemplateManager.OpenFunc(10018);
        }

        public void ToLadder()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            Hotfix_LT.Data.FuncTemplateManager.OpenFunc(10031);
        }

        public void ToLegion()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            LegionLogic.GetInstance().ShowUI();
        }

        public void ToLTTestGMView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTTestGMView");
        }

        public void ToLTLegionWarJoinView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTLegionWarJoinView");
        }

        public void ToLTLegionWarQualifyView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTLegionWarQualifyView");
        }

        public void ToLTNationView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            Hotfix_LT.Data.FuncTemplateManager.OpenFunc(10069);
        }

        public void ToLTGameSettingView()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTGameSettingHud");
        }

        public void ToNotOpenFuncClick()
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LadderController_8669"));
        }

        public void ToLTDailyScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTDailyUI", string.IsNullOrEmpty(GuideNodeManager.FuncOpenGuideId) ? null : GuideNodeManager.FuncOpenGuideId);
            if (!string.IsNullOrEmpty(GuideNodeManager.FuncOpenGuideId)) GuideNodeManager.FuncOpenGuideId = null;//功能引导项
        }

        public void OnStoreBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTStoreUI");
        }

        public void ToWelfareScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTWelfareHud");
        }

        public void ToSigninScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTSigninHud");
        }

        public void ToChargeStoreScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTChargeStoreHud");
        }

        public void ToChargeGiftRewardScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTChargeLimitTimeGiftHud");
        }

        public void ToPrivilegeScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTChargeStoreHud", EChargeType.ePrivilege);
        }

        public void ToConcernHudScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTConcernHud");
        }

        public void ToHolidayActHudScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTActivityHud", LTActivityUIController.SortOrderType.Holiday);
        }

        public void ToLTVIPHud()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTVIPHud");
        }

        public void ToLTComeBackHud()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTComeBackHud");
        }

        public void ToInviteScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTPlayerInvite");
        }

		public void ToLTVIPRewardHud()
		{
			InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
			GlobalMenuManager.Instance.Open("LTVIPRewardHud");
		}

        public void ToPromotionView() 
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTPromotion");
        }
    }
}
