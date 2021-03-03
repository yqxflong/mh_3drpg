using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceHudController : LTGeneralInstanceHudController
    {
        public static void EnterInstance(int id, bool isResume)
        {
            if (id > 0)
            {
                LTChallengeInstanceHpCtrl.RestHpSum();//进入之前重置血量
                if (isResume)//手动重连挑战副本
                {
                    LTInstanceMapModel.Instance.RequestChallengeResumeChapter(id, delegate (string error)
                    {
                        RequestInstanceCallBack(error);
                    });
                }
                else
                {
                    LTInstanceMapModel.Instance.RequestChallengeEnterChapter(id, delegate (string error)//正常进入
                    {
                        RequestInstanceCallBack(error);
                    });
                }
            }
        }
        private static void RequestInstanceCallBack(string error = null)
        {
            if (!string.IsNullOrEmpty(error))
            {
                UIStack.Instance.HideLoadingScreen();
                return;
            }
            PlayerManagerForFilter.Instance.StopShowPlayerForGoToCombat();//切入副本状态需要清掉主城所有的人物
            LTInstanceMapModel.Instance.SwitchViewAction(true);
        }

        public LTClearanceLineupBtn _clearanceLineupBtn;
        public LTChallengeInstanceHpCtrl HpCtrl;
        public GameObject FullScreenMask;
        public UISprite SkipSprite;
        public UISprite QuickSprite;
        public GameObject QuickContainerObj;
        public GameObject BossLogBtnObj;
        public TweenScale BossLogGuideTS;
        public UIProgressBar DoorOpenProgress;
        public List<UISprite> EnemyKillSpriteList;
        public UISprite KeySprite;
        public GameObject KeyFxObj;
        public GameObject BarFxObj;
        public UILabel ThemeNameLabel;
        public DynamicUISprite ThemeIcon;
        public UILabel ThemeTipNameLabel;
        public DynamicUISprite ThemeTipsIcon;
        public UILabel ThemeTipDescLabel;
        public GameObject ThemeTipObj;
        public GameObject FlyMagicObj;
        public GameObject FlyMagicsObj;
        public GameObject MagicObj;
        public GameObject BottomRightObj;
        public LTChallengeSPCtrl SpCtrl;
        public TweenAlpha MagicAlphaTween;
        public TouziTest DiceCom;
        public GameObject FlyScrollObj;
        public GameObject MagnifyingGlass;

        private WaitForSeconds m_Wait = new WaitForSeconds(0.25f);
        private const int maxV = 5;
        private bool IsChallengeQuick = false;
        private bool IsChallengeSkip = false;

        #region Act成员
        private Johny.Action.ActionGeneralParticle stAct_WY = null;
        private Johny.Action.ActionGeneralParticle stAct_SJ = null;
        private void StopAllAction()
        {
            if(stAct_WY != null)
            {
                stAct_WY.Stop();
                stAct_WY = null;
            }
            if(stAct_SJ != null)
            {
                stAct_SJ.Stop();
                stAct_SJ = null;
            }
        }
        #endregion

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("Edge/TopLeft/BackBtn");

            HpCtrl = t.GetMonoILRComponent<LTChallengeInstanceHpCtrl>("Edge/PlayerPanel/Panel/Hp");
            FullScreenMask = t.FindEx("Edge/HidePanel/FullScreenMask").gameObject;

            SkipSprite = t.GetComponent<UISprite>("Edge/BottomLeft/SkipBtn/Check");
            QuickSprite = t.GetComponent<UISprite>("Edge/BottomLeft/QuickBtn/Check");
            QuickContainerObj = t.gameObject.FindEx("Edge/BottomLeft/QuickBtn");
            BossLogBtnObj = t.gameObject.FindEx("Edge/BottomRight/BossLog");
            BossLogGuideTS = t.GetComponent<TweenScale>("Edge/BottomRight/BossLog/TipLabel");

            ThemeNameLabel = t.GetComponent<UILabel>("Edge/TopLeft/Theme/Label");
            ThemeIcon = t.GetComponent<DynamicUISprite>("Edge/TopLeft/Theme/Icon");
            ThemeTipNameLabel = t.GetComponent<UILabel>("Edge/TopLeft/Theme/Tips/NameLabel");
            ThemeTipsIcon = t.GetComponent<DynamicUISprite>("Edge/TopLeft/Theme/Tips/Theme/Icon");
            ThemeTipDescLabel = t.GetComponent<UILabel>("Edge/TopLeft/Theme/Tips/DescLabel");
            ThemeTipObj = t.gameObject.FindEx("Edge/TopLeft/Theme/Tips");

            FlyMagicObj = t.gameObject.FindEx("Edge/BottomRight/BagBtn/Magic/Fly");
            FlyMagicsObj = t.gameObject.FindEx("Edge/BottomRight/BagBtn/Magic/Fly2");
            MagicObj = t.gameObject.FindEx("Edge/BottomRight/BagBtn/Magic");
            BottomRightObj = t.gameObject.FindEx("Edge/BottomRight");
            SpCtrl = t.GetDataLookupILRComponent<LTChallengeSPCtrl>("Edge/BottomRight/BagBtn/Magic/Label");
            MagicAlphaTween = t.GetComponent<TweenAlpha>("Edge/BottomRight/BagBtn/Magic");
            FlyScrollObj = t.gameObject.FindEx("Edge/BottomRight/BagBtn/ScrollFly");
            MagnifyingGlass = t.gameObject.FindEx("Edge/PlayerPanel/Panel/MagnifyingGlass");

            EnemyKillSpriteList = controller.FetchComponentList<UISprite>(new string[5] { "Edge/Bottom/DoorBar/Root/Enemy", "Edge/Bottom/DoorBar/Root/Enemy (1)", "Edge/Bottom/DoorBar/Root/Enemy (2)", "Edge/Bottom/DoorBar/Root/Enemy (3)", "Edge/Bottom/DoorBar/Root/Enemy (4)" }); new List<UISprite>();
            KeySprite = t.GetComponent<UISprite>("Edge/Bottom/DoorBar/Key");
            KeyFxObj = KeySprite.gameObject.FindEx("FX");
            BarFxObj = t.gameObject.FindEx("Edge/Bottom/DoorBar/Foreground/FX");
            DoorOpenProgress = t.GetComponent<UIProgressBar>("Edge/Bottom/DoorBar");

            _clearanceLineupBtn = t.GetMonoILRComponent<LTClearanceLineupBtn>("Edge/TopRight/LTClearanceLineupBtn");

            controller.FindAndBindingBtnEvent(new List<string>(6) { "Edge/TopLeft/Theme", "Edge/TopLeft/Theme/Tips/Box",
                "Edge/BottomLeft/SkipBtn","Edge/BottomLeft/QuickBtn", "Edge/BottomRight/BagBtn","Edge/BottomRight/BattleReadyBtn","Edge/BottomRight/BossLog"},
                new List<EventDelegate>(6) { new EventDelegate(OnThemeBtnClick), new EventDelegate(OnThemeTipClick) ,
                    new EventDelegate(OnSkipBtnClick), new EventDelegate(OnQuickBtnClick), new EventDelegate(OnBagBtnClick), new EventDelegate(OnBattleReadyBtnClick), new EventDelegate(OnBossLogBtnClick)});

        }

        public override void OnEnable()
        {
            base.OnEnable();
            Hotfix_LT.Messenger.AddListener<bool>(EventName.ChallengeBattle, ChallengeBattleFunc);//快速战斗
            Hotfix_LT.Messenger.AddListener(EventName.OnLevelDataUpdate, OnLevelDataUpdateFunc);//更新层级事件
            Hotfix_LT.Messenger.AddListener(EventName.OnResetMapEvent, OnResetMapFunc);//传送门进入下一层
            Hotfix_LT.Messenger.AddListener(EventName.OnQuitMapEvent, OnQuitMapFunc);//传送门退出当前副本
            Hotfix_LT.Messenger.AddListener(EventName.OnChallengeFinished, OnChallengeFinishedFunc);//挑战副本阵亡
            Hotfix_LT.Messenger.AddListener(EventName.OnShowDoorEvent, OnShowDoorEvent);//挑战副本查看门位置事件
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Hotfix_LT.Messenger.RemoveListener<bool>(EventName.ChallengeBattle, ChallengeBattleFunc);
            Hotfix_LT.Messenger.RemoveListener(EventName.OnLevelDataUpdate, OnLevelDataUpdateFunc);//更新层级事件
            Hotfix_LT.Messenger.RemoveListener(EventName.OnResetMapEvent, OnResetMapFunc);//传送门进入下一层
            Hotfix_LT.Messenger.RemoveListener(EventName.OnQuitMapEvent, OnQuitMapFunc);//传送门退出当前副本
            Hotfix_LT.Messenger.RemoveListener(EventName.OnChallengeFinished, OnChallengeFinishedFunc);//挑战副本阵亡
            Hotfix_LT.Messenger.RemoveListener(EventName.OnShowDoorEvent, OnShowDoorEvent);//挑战副本查看门位置事件

            StopAllAction();
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            LTInstanceMapModel.Instance.IsEnterChallenge = true;
            yield return new WaitForSeconds(1);
            LTInstanceMapCtrl.EnterCallback = SetDoorOpenProgressFunc;
            InitMap();
            InitUI();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            HpCtrl.UpdateHp(LTChallengeInstanceHpCtrl.HPEventType.INIT);
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.INIT);
        }

        public override IEnumerator OnRemoveFromStack()
        {
            MapCtrl.Release();
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        protected override void InitUI()
        {
            //获取参数
            int fastNum = PlayerPrefs.GetInt("ChallegenInstanceIsFastCombat");
            int maxLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);
            int openLevel = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("challengeQuickBattle");
            IsChallengeQuick = LTInstanceMapModel.Instance.OpenQuickBattle && (fastNum == 1 && maxLevel >= openLevel);
            if (IsChallengeQuick)
            {
                LTInstanceMapModel.Instance.RequestChallengeFastCombat(IsChallengeQuick, null);
            }
    
            int skipNum = PlayerPrefs.GetInt("ChallegenInstanceIsSkip");
            IsChallengeSkip = (skipNum == 1);
    
            //设置显示
            SkipSprite.gameObject.CustomSetActive(IsChallengeSkip);
            QuickSprite.gameObject.CustomSetActive(IsChallengeQuick);
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.INIT);

            RefreshClearanceLineup();
        }

        public override void OnCancelButtonClick()
        {
            if (!MapCtrl.IsPlayerReady() || LTInstanceMapModel.Instance.moveResultList.Count > 0)//存在手指引导时需要屏蔽返回
            {
                return;
            }
    
            FusionAudio.PostEvent("UI/General/ButtonClick");
            MapCtrl.InstanceWaitUpdataMap(delegate
            {
                Hashtable tempTable = Johny.HashtablePool.Claim();
                tempTable["BeforeServerAction"] = new System.Action(delegate
                {
                    MapCtrl.ClearMoveActionList();
                });
                tempTable["AfterServerAction"] = new System.Action(delegate
                {
                    LTInstanceMapModel.Instance.ClearInstanceData();
                    controller . Close();
                });
                GlobalMenuManager.Instance.Open("LTChallengeInstanceQuitView", tempTable);
            });
        }

        public override void OnFloorClickFunc(LTInstanceNode NodeData, Transform Target)
        {
            if (NodeData == null) return;
            MapCtrl.OnRealEnd = null;
            //设置OnRealEnd到达格子后需要做的事情
            if (!string.IsNullOrEmpty(NodeData.Layout))//怪物
            {
                bool isBoss = false;
                if (NodeData.RoleData.Order == "Boss" || NodeData.RoleData.Id == -1) { isBoss = true; }

                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    int dir = (int)LTInstanceMapModel.Instance.CurNode.GetDirByPos(NodeData.x, NodeData.y);
                    if (dir <= 0 && !isBoss && !isPath) return;

                    if (IsChallengeQuick)//快速战斗
                    {
                        Instance.LTInstanceFloorTemp tmp = MapCtrl.GetNodeObjByPos(new Vector2(NodeData.x, NodeData.y)) as Instance.LTInstanceFloorTemp;
                        if (tmp != null)
                        {
                            FusionAudio.PostEvent("UI/New/Fight", true);
                            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2f);
                            tmp.ShowQuickBattleFX();
                            MapCtrl.OnFloorClick(NodeData, Target);
                        }
                        return;
                    }

                    if (!IsChallengeQuick && IsChallengeSkip)//跳过布阵
                    {
                        FusionAudio.PostEvent("UI/New/Fight", true);
                        InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2f);
                        OnGetToMonster(NodeData, Target, isBoss);
                        return;
                    }

                    if (!IsChallengeQuick && !IsChallengeSkip)//正常战斗
                    {
                        BattleReadyHudController.Open(eBattleType.ChallengeCampaign, delegate ()
                        {
                            OnGetToMonster(NodeData, Target, isBoss);
                        }, NodeData.RoleData.CampaignData.Layout);
                        return;
                    }
                });
                MapCtrl.OnFloorClick(NodeData, Target, isBoss);
                return;
            }
            else if (NodeData.RoleData.Img == "Copy_Icon_Gonggaopai")//留言板
            {
                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    GlobalMenuManager.Instance.Open("LTInstanceMessageView", NodeData.RoleData.Id);
                });
                MapCtrl.OnFloorClick(NodeData, Target);
                return;
            }

            MapCtrl.OnFloorClick(NodeData, Target);
        }

        protected override IEnumerator WaitForPlayer()
        {
            LoadingSpinner.Show();
            while (!MapCtrl.IsPlayerReady())
            {
                yield return null;
            }
            LoadingSpinner.Hide();

            while (LTInstanceMapModel.Instance.EventList.Count > 0)
            {
                LTInstanceEvent evt = LTInstanceMapModel.Instance.EventList.Dequeue();
                EB.Debug.Log("event: type = {0}, x = {1}, y = {2}, param = {3}", evt.Type, evt.x, evt.y, evt.Param);
                //通用
                if (evt.Type == LTInstanceEvent.EVENT_TYPE_OPEN_BOX)//开箱
                {
                    OpenBox(evt.Param, evt.x, evt.y);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_HIDDEN)//密道
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_18472"));
                    LTInstanceMapModel.Instance.EventUpdateData();
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_DOOR_OPEN)//机关打开
                {
                    FusionAudio.PostEvent("UI/New/JiGuan", true);
                    LTInstanceMapModel.Instance.InitOpenDoor(evt.x, evt.y);
                    var nodeObject = MapCtrl.GetNodeObjByPos(evt.x, evt.y);
                    nodeObject?.OpenTheDoor();
                    if (evt.Param != null && evt.Param.Equals("exit"))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_17782"));
                    }
                    else
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_17940"));
                    }
                }
                //挑战
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_HEAL_TRIGGER)//医疗
                {
                    FusionAudio.PostEvent("UI/New/HuiFu", true);
                    // MapCtrl.ShowHealTrigger();
                    Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_fanzhuan_jiaxue");
                    HpCtrl.UpdateHp(LTChallengeInstanceHpCtrl.HPEventType.ADD, evt.HasHPInfoData, evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_BOMB)//炸弹
                {
                    FusionAudio.PostEvent("UI/New/explode", true);
                    ShowBombUpdateHpFx(evt.Param, evt.HasHPInfoData, LTChallengeInstanceHpCtrl.HPEventType.BOOM);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_TRAP_TRIGGER)//陷阱
                {
                    MapCtrl.ShowTrapTrigger();
                    FusionAudio.PostEvent("UI/New/explode", true);
                    ShowBombUpdateHpFx(evt.Param, evt.HasHPInfoData, LTChallengeInstanceHpCtrl.HPEventType.REMOVE);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_MANA_TRAP_TRIGGER)//魔力陷阱
                {
                    FusionAudio.PostEvent("UI/New/explode", true);
                    ShowBombUpdateSpFx(evt);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_EXIT_POINT)//副本传送门
                {
                    var nodeData = LTInstanceMapModel.Instance.GetNodeByPos(evt.x, evt.y);
                    GlobalMenuManager.Instance.Open("LTChallengeInstancePortalView", nodeData.RoleData.Param.Count > 0 && nodeData.RoleData.Param[0] == "BOSS");
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_CHALLENGE_FAIL)//挑战副本失败
                {
                    Hashtable table = Johny.HashtablePool.Claim();
                    table["flag"] = false;
                    System.Action<int> action = OnDefaultViewClose;
                    table["action"] = action;
                    table["isConfirm"] = false;
                    GlobalMenuManager.Instance.Open("LTChallengeInstanceDefaultView", table);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_MANA_REGEN)//魔力药水
                {
                    FusionAudio.PostEvent("UI/New/YaoShui", true);
                    MagicFly(FlyMagicObj, OnMagicFlyEnd);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_MANA_REGEN_FACTOR)//魔力袋
                {
                    FusionAudio.PostEvent("UI/New/YaoShui", true);
                    MagicFly(FlyMagicsObj, OnMagicsFlyEnd);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_DICE)//色子
                {
                    LoadDice(evt.Param, delegate (int num)
                    {
                        FlyScroll();
                    });
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_LUCK)//幸运色子
                {
                    LoadDice(evt.Param, delegate (int num)
                    {
                    });
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_SHOP)//商店
                {
                    if (!MapCtrl.HasNext())
                    {
                        FusionAudio.PostEvent("UI/New/Mai", true);
                        GlobalMenuManager.Instance.Open("LTChallengeInstanceShopView", evt);
                    }
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_SHOPREFRESH)//商店刷新
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceShopCtrl_4116"));
                    Hotfix_LT.Messenger.Raise(EventName.ChallengeInstanceBuySucc, evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_GETSCROLL)//技能卷轴
                {
                    FusionAudio.PostEvent("UI/New/Zhi", true);
                    FlyScroll();
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_HUNTERMARK)//猎人印记
                {
                    FusionAudio.PostEvent("UI/New/SuoDing", true);
                    PLayMagnifyingGlassAni();
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_EXITLOCKED)//被封印传送门
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_18812"));
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_EXITLOCKED2)//钥匙传送门
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CHALLENGE_INSTANCE_NEED_KEY"));
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_GUIDE)//新元素
                {
                    OnGuide(evt);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_WHEEL)//幸运转盘
                {
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("type", LTInstanceConfig.InChallengeState);
                    data.Add("x", evt.x);
                    data.Add("y", evt.y);
                    data.Add("callback", new System.Action(delegate { LTInstanceMapModel.Instance.RequestGetChapterState(); }));
                    GlobalMenuManager.Instance.Open("LTChallengeInstanceTurntableView", data);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_DAMAGE)//瘟疫
                {
                    if (stAct_WY == null || stAct_WY.IsFinished)
                    {
                        stAct_WY = Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_fanzhuan_wenyi");
                    }
                    MapCtrl.ShowTrapTrigger();
                    HpCtrl.UpdateHp(LTChallengeInstanceHpCtrl.HPEventType.WENYI, evt.HasHPInfoData, evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_HEAL)//生机
                {
                    if (stAct_SJ == null || stAct_SJ.IsFinished)
                    {
                        stAct_SJ = Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_fanzhuan_jiaxue");
                    }
                    HpCtrl.UpdateHp(LTChallengeInstanceHpCtrl.HPEventType.SHENGJI, evt.HasHPInfoData, evt.Param);
                }
            }
        }

        private void RefreshClearanceLineup()
        {
            var hasLowestInfo = LTInstanceMapModel.Instance.GetUidFromLowest() > 0;
            _clearanceLineupBtn.mDMono.gameObject.SetActive(hasLowestInfo);

            if (hasLowestInfo)
            {
                _clearanceLineupBtn.RegisterClickEvent(LTInstanceMapModel.Instance.RequestGetLowestTeamViews);
                _clearanceLineupBtn.SetLevel(LTInstanceMapModel.Instance.GetCurLevel());
                _clearanceLineupBtn.SetIcon(LTInstanceMapModel.Instance.GetAvatarFromLowest());
                _clearanceLineupBtn.SetFrame(LTInstanceMapModel.Instance.GetHeadFrameFromLowest());
            }
        }

        private void SetDoorOpenProgressValue(int value)
        {
            for (int i = 0; i < EnemyKillSpriteList.Count; ++i)
            {
                EnemyKillSpriteList[i].spriteName = (i < value) ? "Copy_Icon_Guai2" : "Copy_Icon_Guai1";
            }
            if (value >= maxV)
            {
                KeySprite.color = Color.white;
                KeyFxObj.CustomSetActive(true);
                BarFxObj.CustomSetActive(false);
            }
            else
            {
                KeySprite.color = Color.gray;
                BarFxObj.CustomSetActive(true);
                KeyFxObj.CustomSetActive(false);
            }
            DoorOpenProgress.value = Mathf.Max(0.2f * (float)value, 0.01f);
        }

        private void ShowCurLevelReward(System.Action callback = null)
        {
            List<LTShowItemData> list = new List<LTShowItemData>();
            Hashtable rewardTable = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.challengeChapters.reward", out rewardTable);
            ArrayList wishRewards;
            DataLookupsCache.Instance.SearchDataByID(string.Format("tl_acs.{0}.wish_reward", LTEquipmentWishController.equipmentWishActivityId), out wishRewards);

            if (wishRewards == null)
            {
                wishRewards = new ArrayList();
            }

            foreach (DictionaryEntry data in rewardTable)
            {
                string type = EB.Dot.String("type", data.Value, string.Empty);
                int num = EB.Dot.Integer("quantity", data.Value, 0);
                string id = EB.Dot.String("data", data.Value, string.Empty);
                bool fromWish = EB.Dot.Bool("wishReward", data.Value, false);
                list.Add(new LTShowItemData(id, num, type, isFromWish:fromWish));

                if (fromWish && !string.IsNullOrEmpty(id))
                {
                    for (var i = 0; i < num; i++)
                    {
                        wishRewards.Add(id);
                    }
                }
            }

            // 套装许愿数量刷新
            if (wishRewards != null && wishRewards.Count > 0)
            {
                DataLookupsCache.Instance.CacheData(string.Format("tl_acs.{0}.wish_reward", LTEquipmentWishController.equipmentWishActivityId), wishRewards);
            }

            if (list.Count > 0)
            {
                //上传友盟获得钻石，挑战
                FusionTelemetry.ItemsUmengCurrency(list, "挑战副本获得");
                if (callback != null)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add( "reward", list );
                    ht.Add("callback", callback);
                    GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                }
                else
                {
                    GlobalMenuManager.Instance.Open("LTShowRewardView", list);
                }
            }
        }

        private void OnResetMapFunc()
        {
            //ShowCurLevelReward();进入下一关不再获取物品，通关boss层后才获得
            LTInstanceMapModel.Instance.ClearChapterData();
            DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.reward", null);
            DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.majordata.flag", null);//清除副本标记
            System.Action callBack = delegate {
                MapCtrl.ReInit();
                InitUI();
                LTInstanceMapModel.Instance.RequestChallengeFinshChapterActionDo();
            };
            GlobalMenuManager.Instance.Open("LTCloudFXUI", callBack);
        }

        private void OnQuitMapFunc()
        {
            FusionTelemetry.CampaignDate.PostEvent(FusionTelemetry.CampaignDate.Challenge, LTInstanceMapModel.Instance.CurLevelNum.ToString(), 1, 1);
            ShowCurLevelReward(new System.Action(delegate
            {
                LTInstanceMapModel.Instance.RequestLeaveChapter("", delegate
                {
                    if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                    {
                        LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                        {
                            LTInstanceMapModel.Instance.ClearInstanceData();
                        });
                    }
                });
            }));
        }
 
        public void OnDefaultViewClose(int confirm)
        {
            FusionTelemetry.CampaignDate.PostEvent(FusionTelemetry.CampaignDate.Challenge, LTInstanceMapModel.Instance.CurLevelNum.ToString(), 1, 2);
            LTInstanceMapModel.Instance.RequestLeaveChapter("", delegate
            {
                if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                {
                    LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                    {
                        LTInstanceMapModel.Instance.ClearInstanceData();
                    });
                }
            });
        }

        public void OnBagBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTChallengeInstanceBagView");
        }

        public void OnBattleReadyBtnClick()
        {
            if (MapCtrl.IsPlayerReady())
            {
                BattleReadyHudController.Open(eBattleType.ChallengeCampaign, null);
            }
        }
    
        public void OnBossLogBtnClick()
        {
            int param = PlayerPrefs.GetInt("InstanceIsGuideBossLog", 0);
            if (param == 0)
            {
                PlayerPrefs.SetInt("InstanceIsGuideBossLog", 1);
                BossLogGuideTS.gameObject.CustomSetActive(false);
            }
            GlobalMenuManager.Instance.Open("LTPassTeamInfoUI");
        }
    
        public void OnSkipBtnClick()
        {
            IsChallengeSkip = !IsChallengeSkip;
            SkipSprite.gameObject.CustomSetActive(IsChallengeSkip);
            PlayerPrefs.SetInt("ChallegenInstanceIsSkip", IsChallengeSkip ? 1 : 0);
        }

        public void OnQuickBtnClick()
        {
            int maxLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);
            int openLevel = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("challengeQuickBattle");
            if (maxLevel < openLevel)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_12101"), openLevel));
                return;
            }

            LTInstanceMapModel.Instance.RequestChallengeFastCombat(!IsChallengeQuick, delegate
            {
                IsChallengeQuick = !IsChallengeQuick;
                QuickSprite.gameObject.CustomSetActive(IsChallengeQuick);
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            });
        }
    
        public void OnThemeBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick", true);
            ThemeTipObj.CustomSetActive(true);
            ThemeTipObj.GetComponent<TweenScale>().ResetToBeginning();
            ThemeTipObj.GetComponent<TweenScale>().PlayForward();
        }

        public void OnThemeTipClick()
        {
            ThemeTipObj.CustomSetActive(false);
        }
        
        private void OnLevelDataUpdateFunc()
        {
            if (LTInstanceMapModel.Instance.CurLevelNum <= 0) return;
            SetDoorOpenProgressFunc();

            bool isBossLevel =  Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(LTInstanceMapModel.Instance.CurLevelNum).IsBoss;
            IsChallengeQuick = !isBossLevel && IsChallengeQuick;
            if (LTInstanceMapModel.Instance.OpenQuickBattle) QuickContainerObj.CustomSetActive(!isBossLevel);
            BossLogBtnObj.CustomSetActive(isBossLevel);
            DoorOpenProgress.gameObject.CustomSetActive(!isBossLevel);
            //首领战报气泡引导
            if (isBossLevel)
            {
                int param = PlayerPrefs.GetInt("InstanceIsGuideBossLog", 0);
                if (param == 0)
                {
                    BossLogGuideTS.gameObject.CustomSetActive(true);
                    BossLogGuideTS.PlayForward();
                }
            }
    
            var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeStyleById(LTInstanceMapModel.Instance.CurMapStyle);
            if (tpl != null)
            {
                MapCtrl.SetBGTexture(tpl.MapBg);
                MaskTex.spriteName = tpl.MapMask;
            }
    
            if (LTInstanceMapModel.Instance.ChallengeThemeId > 0)
            {
                var envTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeEnvById(LTInstanceMapModel.Instance.ChallengeThemeId);
                if (envTpl != null)
                {
                    if (LTInstanceMapModel.Instance.IsInstanceShowTheme)
                    {
                        LTInstanceMapModel.Instance.IsInstanceShowTheme = false;
                        GlobalMenuManager.Instance.Open("LTChallengeInstanceThemeHud");
                    }
    
                    ThemeNameLabel.text = envTpl.Name;
                    ThemeIcon.spriteName = envTpl.Icon;
                    ThemeTipNameLabel.text = envTpl.Name;
                    ThemeTipsIcon.spriteName = envTpl.Icon;
                    ThemeTipDescLabel.text = envTpl.Desc;
                    ThemeTipObj.GetComponent<UIWidget>().height = 300 + ThemeTipDescLabel.height;
                    ThemeNameLabel.transform.parent.gameObject.CustomSetActive(true);
                }
                else
                {
                    ThemeNameLabel.transform.parent.gameObject.CustomSetActive(false);
                }
            }
            else
            {
                ThemeNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }
        }
    
        private void SetDoorOpenProgressFunc()
        {
            if (LTInstanceMapModel.Instance.DoorHash != 0)
            {
                int x = LTInstanceMapModel.Instance.DoorHash % 100;
                int y = LTInstanceMapModel.Instance.DoorHash / 100;
                var temp = LTInstanceMapModel.Instance.GetNodeByPos(x, y);
                if (temp != null)
                {
                    SetDoorOpenProgressValue(temp.RoleData.CampaignData.Kill);
                }
            }
        }

        private void ChallengeBattleFunc(bool isOpenMask)
        {
            if (IsChallengeQuick)
            {
                FullScreenMask.CustomSetActive(isOpenMask);
            }
            if (!isOpenMask)
            {
                LTInstanceMapModel.Instance.RequestGetChapterState();
            }
        }
    
        private void OnChallengeFinishedFunc()
        {
            MapCtrl.ClearMoveActionList();
            GlobalMenuManager.Instance.Open("LTChallengeInstanceReviveView", new System.Action<bool>(OnRevieFinished));
        }
    
        private void OnShowDoorEvent()
        {
            return;//目前版本先不用
            //锁屏加镜头移动移动
            if (LTInstanceMapModel.Instance.DoorHash != 0)
            {
                InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST,3f);
                StartCoroutine(ShowDoor());
            }
        }

        private IEnumerator ShowDoor()
        {
            yield break;
        }
        
        private void OnRevieFinished(bool isSucc)
        {
            if (isSucc)
            {
                InitMap();
                HpCtrl.RestDeathState();
            }
            else
            {
                HpCtrl.DeathState();//血量清零
                Hashtable table = Johny.HashtablePool.Claim();
                table["flag"] = false;
                System.Action<int> action = OnDefaultViewClose;
                table["action"] = action;
                table["isConfirm"] = false;
                GlobalMenuManager.Instance.Open("LTChallengeInstanceDefaultView", table);
            }
        }
    
        private void OnGetToMonster(LTInstanceNode NodeData, Transform Target, bool isBoss)
        {
            if (isBoss)
            {
                int dir = (int)LTInstanceMapModel.Instance.CurNode.GetProbablyDirByPos(NodeData.x, NodeData.y);
                if (dir <= 0) return;
    
                LTInstanceMapModel.Instance.RequestMoveChar(dir);
            }
            else
            {
                MapCtrl.OnFloorClick(NodeData, Target);
            }
        }
    
        #region  获得物品触发事件
        private void OpenBox(object data, int x, int y)
        {
            ArrayList list = data as ArrayList;
            if (list != null)
            {
                List<LTShowItemData> gamList = new List<LTShowItemData>();
                for (var i = 0; i < list.Count; ++i)
                {
                    var item = list[i];
                    string type = EB.Dot.String("type", item, string.Empty);
                    string id = EB.Dot.String("data", item, string.Empty);
                    int num = EB.Dot.Integer("quantity", item, 0);
                    if (list.Count == 1&&(type == LTShowItemType.TYPE_RES || type == LTShowItemType.TYPE_HEROSHARD || id == LTDrawCardConfig.LOTTERY_GOLD_ID || id == LTDrawCardConfig.LOTTERY_HC_ID || type == LTShowItemType.TYPE_ACTIVITY))
                    {
                        if (id == LTDrawCardConfig.LOTTERY_GOLD_ID || id == LTDrawCardConfig.LOTTERY_HC_ID || type == LTShowItemType.TYPE_HEROSHARD || id == LTResID.HcName)
                        {
                            FusionAudio.PostEvent("UI/New/ZuanShi", true);//获取抽奖券、碎片、钻石时播放
                        }
                        else if (id == LTResID.GoldName)
                        {
                            FusionAudio.PostEvent("UI/New/Coin", true);//获得金币时播放
                        }
                        else if (id == LTResID.BuddyExpName)
                        {
                            FusionAudio.PostEvent("UI/New/YaoShui", true);//获得伙伴经验时播放
                        }
                        LTIconNameQuality icon_name_lvl = LTItemInfoTool.GetInfo(id, type);
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_20066"), icon_name_lvl.name, num));
                        continue;
                    }
                    LTShowItemData itemData = new LTShowItemData(id, num, type);
                    gamList.Add(itemData);
                }

                if (gamList.Count > 0)
                {
                    FusionAudio.PostEvent("UI/New/BaoXiang", true);
                    LTInstanceNodeTemp temp = MapCtrl.GetNodeObjByPos(x, y);
                    if (temp != null)
                    {
                        //首领宝箱奖励
                        var floor = temp as Instance.LTInstanceFloorTemp;
                        if (floor != null&& floor.hasOtherModel())
                        {
                            //宝箱表现!
                            floor.ShowBossRewardFX(gamList,UICamera.mainCamera.WorldToScreenPoint(FlyScrollObj.transform.position));
                            return;
                        }
                    }
                    //普通宝箱奖励
                    GlobalMenuManager.Instance.Open("LTShowBoxView", gamList);
                }
    
            }
        }
        #endregion
    
        #region 陷阱，炸弹，魔力炸弹相关表现
        private void ShowBombUpdateHpFx(object num, bool HasHPInfoData, LTChallengeInstanceHpCtrl.HPEventType type)
        {
            // BombFxCom.gameObject.CustomSetActive(true);
            Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_UI_zhadan_Boom");
            StartCoroutine(OnBombUpdateHpFxEnd(num, HasHPInfoData, type));
        }

        private IEnumerator OnBombUpdateHpFxEnd(object num, bool HasHPInfoData, LTChallengeInstanceHpCtrl.HPEventType type)
        {
            yield return m_Wait;
    
            MapCtrl.ShowTrapTrigger();
            HpCtrl.UpdateHp(type, HasHPInfoData, num);
    
            yield return m_Wait;
    
            // BombFxCom.gameObject.CustomSetActive(false);
        }

        private void ShowBombUpdateSpFx(LTInstanceEvent evt)
        {
            // SPBombFxCom.gameObject.CustomSetActive(true);
            Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_UI_Anniu_Boom_lan");
            StartCoroutine(OnBombUpdateSpFxEnd(evt));
        }
        /// <summary>
        /// 魔力炸弹表现结束
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private IEnumerator OnBombUpdateSpFxEnd(LTInstanceEvent evt)
        {
            yield return m_Wait;
    
            MapCtrl.ShowTrapTrigger();
            MagicAlphaTween.ResetToBeginning();
            MagicAlphaTween.PlayForward();
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.REMOVE);
    
            yield return m_Wait;
            // SPBombFxCom.gameObject.CustomSetActive(false);
        }
        #endregion
    
        #region 获得魔力药水或魔力袋表现
        private void MagicFly(GameObject flyObj, EventDelegate.Callback evt)
        {
            flyObj.CustomSetActive(true);
            Vector3 start = MagicObj.transform.localPosition - BottomRightObj.transform.localPosition;// - new Vector3(0, 50000, 0);
    
            TweenPosition tweenPos = flyObj.GetComponent<TweenPosition>();
            tweenPos.from = start;
            tweenPos.to = Vector3.zero;
            tweenPos.SetOnFinished(evt);
            tweenPos.ResetToBeginning();
            tweenPos.PlayForward();
    
            TweenScale tweenScale = flyObj.GetComponent<TweenScale>();
            tweenScale.from = new Vector3(3, 3, 3);
            tweenScale.to = Vector3.one;
            tweenScale.ResetToBeginning();
            tweenScale.PlayForward();
        }

        private void OnMagicFlyEnd()
        {
            FlyMagicObj.CustomSetActive(false);
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.ADD);
        }

        private void OnMagicsFlyEnd()
        {
            FlyMagicsObj.CustomSetActive(false);
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.DOUBLE);
        }
        #endregion
    
        #region 摇色相关
        private void LoadDice(object param, System.Action<int> callback)
        {
            FusionAudio.PostEvent("UI/New/Dice", true);
            string path = "_GameAssets/Res/MISC/Dice/DiceObj";
            int num = int.Parse(param.ToString());
            if (DiceCom != null)
            {
                InitDice(num, callback);
                return;
            }
            EB.Assets.LoadAsync(path, typeof(GameObject), o => 
            {
                if(!o) return;
                
                GameObject diceObj = GameObject.Instantiate(o) as GameObject;
                diceObj.transform.parent = controller.transform;
                diceObj.transform.localScale = Vector3.one;
                DiceCom = diceObj.GetComponent<TouziTest>();

                if (DiceCom != null)
                {
                    InitDice(num, callback);
                    return;
                }
            });
        }

        private void InitDice(int num, System.Action<int> callback)
        {
            //打开全屏遮罩，摇骰子的时候不让操作
            FullScreenMask.CustomSetActive(true);
            DiceCom.InitDice(num - 1, delegate
            {
                FusionAudio.PostEvent("UI/New/Dice", false);
                FullScreenMask.CustomSetActive(false);
                callback(num);
            });
        }
        #endregion

        #region 技能卷轴相关
        public void FlyScroll()
        {
            FlyScrollObj.CustomSetActive(true);
            Vector3 start = -BottomRightObj.transform.localPosition;// - new Vector3(0, 50000, 0);
    
            TweenPosition tweenPos = FlyScrollObj.GetComponent<TweenPosition>();
            tweenPos.from = start;
            tweenPos.to = Vector3.zero;
            tweenPos.SetOnFinished(OnScrollFlyEnd);
            tweenPos.ResetToBeginning();
            tweenPos.PlayForward();
    
    
            TweenScale tweenScale = FlyScrollObj.GetComponent<TweenScale>();
            tweenScale.from = new Vector3(3, 3, 3);
            tweenScale.to = Vector3.one;
            tweenScale.ResetToBeginning();
            tweenScale.PlayForward();
        }

        private void OnScrollFlyEnd()
        {
            FlyScrollObj.CustomSetActive(false);
        }
        #endregion
    
        #region 猎人印记
        private void PLayMagnifyingGlassAni()
        {
            MagnifyingGlass.CustomSetActive(true);
            FullScreenMask.CustomSetActive(true);
            UITweener[] twn = MagnifyingGlass.transform.GetComponentsInChildren<UITweener>();
            for (int i = 0; i < twn.Length; i++)
            {
                twn[i].ResetToBeginning();
                twn[i].PlayForward();
            }
            twn[0].SetOnFinished(OnMagnifyingGlassAniEnd);
        }

        private void OnMagnifyingGlassAniEnd()
        {
            MagnifyingGlass.CustomSetActive(false);
            FullScreenMask.CustomSetActive(false);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_25184"));
    
        }
        #endregion
    
        #region  新元素事件
        private void OnGuide(LTInstanceEvent evt)
        {
            Instance.LTInstanceFloorTemp tmp = MapCtrl.GetNodeObjByPos(new Vector2(evt.x, evt.y)) as Instance.LTInstanceFloorTemp;
            if (tmp != null)
            {
                int roleId = EB.Dot.Integer("id", evt.Param, 0);
                tmp.ShowNotice(roleId);
            }
        }
        #endregion
    }
}
