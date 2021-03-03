using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTAlienMazeInstanceHudController : LTGeneralInstanceHudController
    {
        public static void EnterInstance(int id)
        {
            if (id > 0)
            {
                LTChallengeInstanceHpCtrl.RestHpSum();//进入之前重置血量
                LTInstanceMapModel.Instance.RequestAlienMazeEnter(id, delegate(string error)
                {
                    RequestInstanceCallBack(error);
                });
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

        public LTChallengeInstanceHpCtrl HpCtrl;
        public GameObject FullScreenMask;
        public UISprite SkipSprite;
        public UILabel ThemeNameLabel;
        public UISprite ThemeIcon;
        public UILabel ThemeTipNameLabel;
        public UISprite ThemeTipsIcon;
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
        private bool IsMazeSkip = false;
        private bool isFinish = false;
        private bool isFirst = true;

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

            ThemeNameLabel = t.GetComponent<UILabel>("Edge/TopLeft/Theme/Label");
            ThemeIcon = t.GetComponent<UISprite>("Edge/TopLeft/Theme");
            ThemeTipNameLabel = t.GetComponent<UILabel>("Edge/TopLeft/Theme/Tips/NameLabel");
            ThemeTipsIcon = t.GetComponent<UISprite>("Edge/TopLeft/Theme/Tips/Theme");
            ThemeTipDescLabel = t.GetComponent<UILabel>("Edge/TopLeft/Theme/Tips/DescLabel");
            ThemeTipObj = t.FindEx("Edge/TopLeft/Theme/Tips").gameObject;

            FlyMagicObj = t.FindEx("Edge/BottomRight/BagBtn/Magic/Fly").gameObject;
            FlyMagicsObj = t.FindEx("Edge/BottomRight/BagBtn/Magic/Fly2").gameObject;
            MagicObj = t.FindEx("Edge/BottomRight/BagBtn/Magic").gameObject;
            BottomRightObj = t.FindEx("Edge/BottomRight").gameObject;
            SpCtrl = t.GetDataLookupILRComponent<LTChallengeSPCtrl>("Edge/BottomRight/BagBtn/Magic/Label");
            MagicAlphaTween = t.GetComponent<TweenAlpha>("Edge/BottomRight/BagBtn/Magic");
            FlyScrollObj = t.FindEx("Edge/BottomRight/BagBtn/ScrollFly").gameObject;
            MagnifyingGlass = t.FindEx("Edge/PlayerPanel/Panel/MagnifyingGlass").gameObject;

            controller.FindAndBindingBtnEvent(new List<string>(6) { "Edge/TopLeft/Theme", "Edge/TopLeft/Theme/Tips/Box",
                "Edge/TopRight/RuleBtn","Edge/BottomLeft/SkipBtn","Edge/BottomRight/BagBtn","Edge/BottomRight/BattleReadyBtn"},
               new List<EventDelegate>(6) { new EventDelegate(OnThemeBtnClick), new EventDelegate(OnThemeTipClick) ,
                new EventDelegate(OnRuleBtnClick), new EventDelegate(OnSkipBtnClick), new EventDelegate(OnBagBtnClick), new EventDelegate(OnBattleReadyBtnClick)});

        }

        public override void OnEnable()
        {
            base.OnEnable();
            Hotfix_LT.Messenger.AddListener(EventName .OnLevelDataUpdate, OnLevelDataUpdateFunc);//更新层级事件
            Hotfix_LT.Messenger.AddListener(EventName.OnQuitMapEvent, OnQuitMapFunc);//传送门退出当前副本
            Hotfix_LT.Messenger.AddListener(EventName.OnChallengeFinished, OnChallengeFinishedFunc);//挑战副本阵亡
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Hotfix_LT.Messenger.RemoveListener(EventName.OnLevelDataUpdate, OnLevelDataUpdateFunc);//更新层级事件
            Hotfix_LT.Messenger.RemoveListener(EventName.OnQuitMapEvent, OnQuitMapFunc);//传送门退出当前副本
            Hotfix_LT.Messenger.RemoveListener(EventName.OnChallengeFinished, OnChallengeFinishedFunc);//挑战副本阵亡

            StopAllAction();
        }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            LTInstanceMapModel.Instance.IsEnterAlienMaze = true;
            yield return new WaitForSeconds(1);
            InitMap();
            InitUI();
        }

        public override void OnFocus()
        {
            base.OnFocus();
            HpCtrl.UpdateHp(LTChallengeInstanceHpCtrl.HPEventType.INIT);
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.INIT);

            FinishCheckFunc();
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
            int skipNum = PlayerPrefs.GetInt("MazeInstanceIsSkip",0);
            IsMazeSkip = (skipNum == 1);
            SkipSprite.gameObject.CustomSetActive(IsMazeSkip);
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.INIT);
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
                    controller.Close();
                });
                tempTable["isAlienMaze"] = true;
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

                    if (IsMazeSkip)//跳过布阵
                    {
                        FusionAudio.PostEvent("UI/New/Fight", true);
                        InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2f);
                        OnGetToMonster(NodeData, Target, isBoss);
                        return;
                    }
                    else//正常战斗
                    {
                        BattleReadyHudController.Open(eBattleType.AlienMazeBattle, delegate ()
                        {
                            OnGetToMonster(NodeData, Target, isBoss);
                        }, NodeData.RoleData.CampaignData.Layout);
                        return;
                    }
                });
                MapCtrl.OnFloorClick(NodeData, Target, isBoss);
                return;
            }
            else if (!string.IsNullOrEmpty(NodeData.RoleData.CampaignData.Password))//箱子需要密码才能打开，此处密码由前端检查
            {
                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    var tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(NodeData.RoleData.Id);
                    string title = string.Empty;
                    string tip = string.Empty;
                    if (tpl != null)
                    {
                        if (tpl.Guide[0] == "#")
                        {
                            tpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(int.Parse(tpl.Guide[1]));
                        }
                        title = tpl.Guide[0];
                        tip = tpl.Guide[1];
                    }
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("data", NodeData);
                    ht.Add("title", title);
                    ht.Add("tip", tip);
                    GlobalMenuManager.Instance.Open("LTMainInstancePasswordBoxView", ht);
                });
                MapCtrl.OnFloorClick(NodeData, Target);
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
                    OpenBox(evt.Param);
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
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_PASSWORD)//显示密码
                {
                    FusionAudio.PostEvent("UI/New/Zhi", true);
                    ShowPasswordView(evt.Param);
                }
            }
        }

        private void FinishCheckFunc()
        {
            if (!HpCtrl.mIsDeath && LTInstanceMapModel.Instance.IsMazeHasFinish() && !isFinish)
            {
                isFinish = true;
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ALIEN_MAZE_FINISH_TIP"));
                LTInstanceMapModel.Instance.RequestChallengeFinshChapter(0, LTInstanceMapModel.Instance.CurLevelNum, delegate
                {
                    FusionAudio.PostEvent("UI/Floor/Transfer");
                    LTInstanceMapModel.Instance.ResetAlienMazeInit();
                });
            }
        }

        private void ShowCurLevelReward(System.Action callback = null)
        {
            List<LTShowItemData> list = new List<LTShowItemData>();
            Hashtable rewardTable = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.challengeChapters.reward", out rewardTable);
            foreach (DictionaryEntry data in rewardTable)
            {
                string type = EB.Dot.String("type", data.Value, string.Empty);
                int num = EB.Dot.Integer("quantity", data.Value, 0);
                string id = EB.Dot.String("data", data.Value, string.Empty);
                bool fromWish = EB.Dot.Bool("wishReward", data.Value, false);
                list.Add(new LTShowItemData(id, num, type, false, isFromWish:fromWish));
            }
            if (list.Count > 0)
            {
                //上传友盟获得钻石，挑战
                FusionTelemetry.ItemsUmengCurrency(list, "异界迷宫获得");
                
                if (callback != null)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("reward", list);
                    ht.Add("callback", callback );
                    GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                }
                else
                {
                    GlobalMenuManager.Instance.Open("LTShowRewardView", list);
                }
            }
        }
    
        private void OnQuitMapFunc()
        {
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
    
        private void OnDefaultViewClose(int confirm)
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
        }
    
        public void OnBagBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTChallengeInstanceBagView");
        }
    
        public void OnBattleReadyBtnClick()
        {
            if (MapCtrl.IsPlayerReady())
            {
                BattleReadyHudController.Open(eBattleType.AlienMazeBattle, null);
            }
        }
    
        public void OnSkipBtnClick()
        {
            IsMazeSkip = !IsMazeSkip;
            SkipSprite.gameObject.CustomSetActive(IsMazeSkip);
            PlayerPrefs.SetInt("MazeInstanceIsSkip", IsMazeSkip ? 1 : 0);
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
    
        public void OnRuleBtnClick()
        {
            string text = EB.Localizer.GetString("ID_ALIEN_MAZE_RULE_TEXT");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text);
        }
    
        private void OnLevelDataUpdateFunc()
        {
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
                        GlobalMenuManager.Instance.Open("LTChallengeInstanceThemeHud", true);
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
    
            if (isFirst)
            {
                isFirst = false;
                FinishCheckFunc();
            }
        }
        
        private void OnChallengeFinishedFunc()
        {
            //不走复活，失败就直接退出
            MapCtrl.ClearMoveActionList();
            OnRevieFinished(false);
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
                int dir = (int)LTInstanceMapModel.Instance.CurNode.GetProbablyDirByPos(NodeData.x, NodeData.y,true);
                if (dir <= 0) return;
    
                LTInstanceMapModel.Instance.RequestMoveChar(dir);
            }
            else
            {
                MapCtrl.OnFloorClick( NodeData,  Target);
            }
        }
    
        #region  获得物品触发事件
        private void OpenBox(object data)
        {
            ArrayList list = data as ArrayList;
            if (list != null)
            {
                List<LTShowItemData> gamList = new List<LTShowItemData>();
                for (var i = 0; i < list.Count; i++)
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
                    GlobalMenuManager.Instance.Open("LTShowBoxView", gamList);
                }
    
            }
        }
        #endregion
    
        #region 陷阱，炸弹，魔力炸弹相关表现
        private void ShowBombUpdateHpFx(object num, bool HasHPInfoData, LTChallengeInstanceHpCtrl.HPEventType type)
        {
            Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_UI_zhadan_Boom");
            StartCoroutine(OnBombUpdateHpFxEnd(num, HasHPInfoData, type));
        }

        private IEnumerator OnBombUpdateHpFxEnd(object num, bool HasHPInfoData, LTChallengeInstanceHpCtrl.HPEventType type)
        {
            yield return m_Wait;
    
            MapCtrl.ShowTrapTrigger();
            HpCtrl.UpdateHp(type, HasHPInfoData, num);
        }

        private void ShowBombUpdateSpFx(LTInstanceEvent evt)
        {
            Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance.HoldPlayerEffect("fx_fb_UI_Anniu_Boom_lan");
            StartCoroutine(OnBombUpdateSpFxEnd(evt));
        }

        private IEnumerator OnBombUpdateSpFxEnd(LTInstanceEvent evt)
        {
            yield return m_Wait;
    
            MapCtrl.ShowTrapTrigger();
            MagicAlphaTween.ResetToBeginning();
            MagicAlphaTween.PlayForward();
            SpCtrl.UpdateSp(LTChallengeSPCtrl.SPEventType.REMOVE);
        }
        #endregion
    
        #region 获得魔力药水或魔力袋表现
        private void MagicFly(GameObject flyObj, EventDelegate.Callback evt)
        {
            flyObj.CustomSetActive(true);
            Vector3 start = MagicObj.transform.localPosition - BottomRightObj.transform.localPosition ;//- new Vector3(0, 50000, 0);
    
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
                if (!o) return;

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
        private void FlyScroll()
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
    
        #region 展示密码界面
        private void ShowPasswordView(object data)
        {
            string password = data as string;
            if (!string.IsNullOrEmpty(password))
            {
                GlobalMenuManager.Instance.Open("LTMainInstancePasswordView", password);
            }
        }
        #endregion
    }
}
