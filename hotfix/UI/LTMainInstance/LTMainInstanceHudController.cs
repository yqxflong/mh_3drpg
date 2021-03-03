using GM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 副本格子界面管理器
/// </summary>
namespace Hotfix_LT.UI
{
    public class LTMainInstanceHudController : LTGeneralInstanceHudController
    {
        public static void EnterInstance(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                LTInstanceMapModel.Instance.RequestMainEnterChapter(int.Parse(id), delegate
                {
                    RequestInstanceCallBack();
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

        private UILabel ChapterLabel;
        private GameObject LampFxObj;
        private UILabel StarNumLabel;
        private UISlider StarSlider;
        private UISprite BackBarSprite;
        private UIGrid FireGird;
        private LTGuideTips _guideTips;
        private MainPlot mainPlot;
        private GameObject QuickSelectObj;
        private List<GameObject> StarBoxList;
        private List<UISprite> LampFireList;

        private Hotfix_LT.Data.LostMainChapterTemplate mChapterTpl;
        private int clickCount = 0;
        private bool IsMainQuick;
        private bool IsPlayDialog = false;
        private bool isShowingBoxReward = false;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;

            QuickSelectObj = t.gameObject.FindEx("Edge/Bottom/QuickBtn/Check");
            ChapterLabel = t.GetComponent<UILabel>("Edge/TopRight/MapPanel/Map/LevelLabel");
            LampFxObj = t.gameObject.FindEx("Edge/FireListBG/BottomLeft/LampFX");
            StarNumLabel = t.GetComponent<UILabel>("Edge/Bottom/AllStar/Label");
            StarSlider = t.GetComponent<UISlider>("Edge/Bottom/Back");
            BackBarSprite = t.GetComponent<UISprite>("Edge/Bottom/Back");
            FireGird = t.GetComponent<UIGrid>("Edge/FireList");

            mainPlot = t.GetMonoILRComponentByClassPath<MainPlot>("Hotfix_LT.UI.MainPlot");
            _guideTips = t.GetMonoILRComponent<LTGuideTips>("Edge/Bottom/GuideTips");

            StarBoxList = controller.FetchComponentList<GameObject>(new string[3] { "Edge/Bottom/Back/FirstBox", "Edge/Bottom/Back/SecondBox", "Edge/Bottom/Back/ThirdBox" }, true);
            LampFireList = controller.FetchComponentList<UISprite>(new string[5] { "Edge/FireList/Fire", "Edge/FireList/Fire (1)", "Edge/FireList/Fire (2)", "Edge/FireList/Fire (3)", "Edge/FireList/Fire (4)" });

            controller.FindAndBindingBtnEvent(new List<string>(2) { "Edge/FireListBG/BottomLeft", "Edge/Bottom/QuickBtn" },
                new List<EventDelegate>(2) {new EventDelegate(OnLampClick), new EventDelegate(OnQuickBtnClick) });

            controller.FindAndBindingCoolTriggerEvent(new List<string>(4) { "Edge/TopLeft/BackBtn", "Edge/Bottom/Back/FirstBox/Box", "Edge/Bottom/Back/SecondBox/Box", "Edge/Bottom/Back/ThirdBox/Box" },
                new List<EventDelegate>(4) { new EventDelegate(OnCancelButtonClick), new EventDelegate(() => OnStarBoxClick(StarBoxList[0])), new EventDelegate(() => OnStarBoxClick(StarBoxList[1])), new EventDelegate(() => OnStarBoxClick(StarBoxList[2])) });

        }

        public override void OnDisable()
        {
            base.OnDisable();
            LTInstanceMapModel.Instance.IsInitPraypoint = false;
        }
        
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if (param != null)
            {
                LTInstanceMapModel.Instance.MainChapterId = param as string;
            }
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            InitMap();
            InitUI();

            InitLamp();
            ShowStarBox();
            
            Hotfix_LT.Messenger.AddListener(EventName.UpDatePraypointUI, InitLamp);//更新神灯
            Hotfix_LT.Messenger.AddListener(EventName.MainBattleQuick, QuickBattleEvent);
        }

        public override void OnFocus()
        {
            base.OnFocus();
            LTMainHudManager.Instance.SetDefeatSkipPanel();
            _guideTips.OnEnable();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.UpDatePraypointUI, InitLamp);
            Hotfix_LT.Messenger.RemoveListener(EventName.MainBattleQuick, QuickBattleEvent);
            MapCtrl.Release();
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }

        public override void OnCancelButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (!MapCtrl.IsPlayerReady() || LTInstanceMapModel.Instance.moveResultList.Count > 0) { return; }

            MapCtrl.InstanceWaitUpdataMap(delegate
            {
                MapCtrl.ClearMoveActionList();
                if (GuideNodeManager.IsGuide && !LTInstanceUtil.IsFirstChapterCompleted())
                {
                    if (clickCount >= 3)
                    {
                        clickCount = 0;
                        MessageTemplateManager.ShowMessage(901099, null, delegate (int result)
                        {
                            if (result == 0)
                            {
                                GuideNodeManager.currentGuideId = 0;
                                GuideNodeManager.GetInstance().JumpGuide();//跳过主线
                            }
                            return;
                        });
                    }
                    clickCount++;
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_RETURN));
                    return;
                }

                if (LTInstanceMapModel.Instance.NotMainChapterId())
                {
                    if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                    {
                        LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                        {
                            if(controller!=null) controller.Close();
                        });
                    }
                    return;
                }
                
                LTInstanceMapModel.Instance.ClearInstanceData();
                LTInstanceMapModel.Instance.RequestLeaveChapter("main", delegate
                {
                    if (LTInstanceMapModel.Instance.IsInsatnceViewAction())
                    {
                        LTInstanceMapModel.Instance.SwitchViewAction(false, true, delegate
                        {
                            if (controller != null) controller.Close();
                        });
                    }
                });
            });
        }
        
        protected override void InitUI()
        {
            mChapterTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById(LTInstanceMapModel.Instance.MainChapterId);
            if (mChapterTpl == null) return;

            int fastNum = PlayerPrefs.GetInt("MainInstanceIsFastCombat");
            int openLevel = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("mainQuickBattle");
            int passLevel = LTInstanceMapModel.Instance.GetMaxCampaignLevel();
            bool isOpen = openLevel <= passLevel;
            IsMainQuick = (fastNum == 1 && isOpen);
            QuickSelectObj.CustomSetActive(IsMainQuick);

            InitStarBox();
            ChapterLabel.text = mChapterTpl.Name;
            MapCtrl.SetBGTexture(mChapterTpl.ChapterBg);
            MaskTex.spriteName = mChapterTpl.MaskBg;
        }

        public override void OnFloorClickFunc(LTInstanceNode NodeData, Transform Target)
        {
            if (NodeData == null) return;
            MapCtrl.OnRealEnd = null;
            //设置OnRealEnd到达格子后需要做的事情
            if (NodeData.RoleData.CampaignData.CampaignId > 0)//主线关卡
            {
                LTGuideTips.MainInstanceCampaignId = NodeData.RoleData.CampaignData.CampaignId;

                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    MapCtrl.InstanceWaitUpdataMap(delegate
                    {
                        if (GuideNodeManager.IsGuide && !theFirstCampaignPass)//新手引导特殊处理步骤
                        {
                            //是否已通关了第一关卡    
                            FusionAudio.PostEvent("UI/New/Fight", true);
                            InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2f);
                            MapCtrl.OnFloorClick(NodeData, Target);
                            LTInstanceMapModel.Instance.RequestMainFightCampaign(NodeData.RoleData.CampaignData.CampaignId);
                            return;
                        }

                        if (LTInstanceMapModel.Instance.WaitForDialoging ||DialoguePlayUtil.Instance.State) return;//剧情播放情况，不要再执行下面

                        if (IsMainQuick)
                        {
                            bool isCanQuick = NodeData.RoleData.CampaignData.Star == 0;
                            bool isNotBoss = !Data.SceneTemplateManager.Instance.IsCampaignBoss(NodeData.RoleData.CampaignData.CampaignId.ToString());
                            if (isCanQuick && isNotBoss)
                            {
                                Instance.LTInstanceFloorTemp tmp = MapCtrl.GetNodeObjByPos(new Vector2(NodeData.x, NodeData.y)) as Instance.LTInstanceFloorTemp;
                                if (tmp != null)
                                {
                                    FusionAudio.PostEvent("UI/New/Fight", true);
                                    InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2f);
                                    tmp.ShowQuickBattleFX();
                                    MapCtrl.OnFloorClick(NodeData, Target);
                                    LTInstanceMapModel.Instance.RequestMainFightCampaign(NodeData.RoleData.CampaignData.CampaignId, 0, true);
                                }
                                return;
                            }
                        }

                        Hashtable data = Johny.HashtablePool.Claim();
                        data.Add("id", NodeData.RoleData.CampaignData.CampaignId);
                        data.Add("callback", new System.Action(delegate
                        {
                            MapCtrl.OnFloorClick(NodeData, Target);
                        }));
                        GlobalMenuManager.Instance.Open("LTMainInstanceCampaignView", data);
                    });
                });
                MapCtrl.OnFloorClick(NodeData, Target);
                return;
            }
            else if (!string.IsNullOrEmpty(NodeData.RoleData.CampaignData.Password))//箱子需要密码才能打开，此处密码由前端检查
            {
                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    GlobalMenuManager.Instance.Open("LTMainInstancePasswordBoxView", NodeData);
                });
                MapCtrl.OnFloorClick(NodeData, Target);
                return;
            }
            else if (!string.IsNullOrEmpty(NodeData.RoleData.Model) && (NodeData.RoleData.Order == "Hero"))//奖励英雄
            {
                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    //SetBlurTexState(false);
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("NodeData", NodeData);
                    data.Add("IsHire", false);
                    data.Add("ChapterBg", mChapterTpl.ChapterBg);
                    data.Add("Callback", new System.Action<bool>(delegate (bool isSucc)
                    {
                        //SetBlurTexState(true);
                        if (isSucc)
                        {
                            MapCtrl.OnFloorClick(NodeData, Target);
                            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfoByModel(NodeData.RoleData.Model);
                            if (charTpl != null)
                            {
                                LTShowItemData itemData = new LTShowItemData((charTpl.id + 1).ToString(), 1, "hero", false);
                                GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                            }
                        }
                    }));
                    GlobalMenuManager.Instance.Open("LTMainInstanceGetHeroView", data);
                });
                MapCtrl.OnFloorClick(NodeData, Target);
                return;
            }
            else if (!string.IsNullOrEmpty(NodeData.RoleData.Model) && (NodeData.RoleData.Order == "Hire"))//雇佣兵
            {
                MapCtrl.OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("NodeData", NodeData);
                    data.Add("IsHire", true);
                    data.Add("ChapterBg", mChapterTpl.ChapterBg);
                    data.Add("Callback", new System.Action<bool>(delegate (bool isSucc)
                    {
                        if (isSucc)
                        {
                            MapCtrl.OnFloorClick(NodeData, Target);
                            var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfoByModel(NodeData.RoleData.Model);
                            if (charTpl != null)
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_CHALLENGE_INSTANCE_HIRE_0"), charTpl.name));
                            }

                            Instance.LTInstanceFloorTemp tmp = MapCtrl.GetNodeObjByPos(new Vector2(NodeData.x, NodeData.y)) as Instance.LTInstanceFloorTemp;
                            if (tmp != null)
                            {
                                tmp.HideNotice();
                            }
                        }
                    }));
                    GlobalMenuManager.Instance.Open("LTMainInstanceGetHeroView", data);
                });
                MapCtrl.OnFloorClick(NodeData, Target);
                return;
            }
            else if (NodeData.RoleData.Img == "Copy_Icon_Gonggaopai")
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

            while (isShowingBoxReward)
            {
                yield return null;
            }

            while (LTInstanceMapModel.Instance.EventList.Count > 0)
            {
                LTInstanceEvent evt = LTInstanceMapModel.Instance.EventList.Dequeue();
                EB.Debug.Log("event: type = {0}, x = {1}, y = {2}, param = {3}", evt.Type, evt.x, evt.y, evt.Param);
                if (evt.Type == LTInstanceEvent.EVENT_TYPE_OPEN_BOX)//开箱
                {
                    OpenBox(evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_HIDDEN)//密道
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_18472"));
                    LTInstanceMapModel.Instance.EventUpdateData();
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_DOOR_OPEN)//开开关
                {
                    FusionAudio.PostEvent("UI/New/JiGuan", true);
                    LTInstanceMapModel.Instance.InitOpenDoor(evt.x, evt.y);
                    var nodeObject = MapCtrl.GetNodeObjByPos(evt.x, evt.y);
                    nodeObject?.OpenTheDoor();
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTMainInstanceHudController_16229"));
                }
                //主线
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_DIALOG)//剧情对话
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    if (MapCtrl.isPlayerShow) ShowDialog(evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_MOVIE)//剧情动画
                {
                    ShowMovie(evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_PRAY_POINT)//神灯
                {
                    FusionAudio.PostEvent("UI/New/JianShenDeng", true);
                    FlyFire();
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_PRAYPOINTFULL)//神灯已满
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTMainInstanceHudController_15637"));
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_PASSWORD)//显示密码
                {
                    FusionAudio.PostEvent("UI/New/Zhi", true);
                    ShowPasswordView(evt.Param);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_HERO)//获得英雄
                {
                    OnGetHero(evt);
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_MAIN_CAMP_OVER)//主线副本章节结束事件，回大地图
                {
                    FusionTelemetry.CampaignDate.PostEvent(FusionTelemetry.CampaignDate.Main, LTInstanceMapModel.Instance.GetMaxCampaignLevel().ToString(), 1, 1);
                    while (IsPlayDialog)
                    {
                        yield return null;
                    }
                    if (!string.IsNullOrEmpty(mChapterTpl.AfterChapter))
                    {
                        System.Action goMapAction = () => { GoBackWorldMap(); };//返回大地图

                        //章节结束可能会有剧情动画，也可能会有剧情对话，也可能会同时有； 用;截取，纯数字的就是剧情对话，顺序看配表的顺序
                        string[] strs = mChapterTpl.AfterChapter.Split(';');
                        int dialogueID = 0;
                        if (!string.IsNullOrEmpty(strs[0]))
                        {
                            if (strs[0].IndexOf("Chapter") >= 0)
                            {
                                if (strs.Length > 1 && int.TryParse(strs[1], out dialogueID))
                                {
                                    System.Action afterChapterAction = () => { ShowDialog(dialogueID.ToString(), goMapAction); };
                                    LTStoryController.OpenStory(afterChapterAction, strs[0]);
                                }
                                else
                                {
                                    LTStoryController.OpenStory(goMapAction, strs[0]);
                                }
                            }
                            else if (int.TryParse(strs[0], out dialogueID))
                            {
                                if (strs.Length > 1 && strs[1].IndexOf("Chapter") >= 0)
                                {
                                    System.Action afterPlayDiaAction = () => { LTStoryController.OpenStory(goMapAction, strs[1]); };
                                    ShowDialog(dialogueID.ToString(), afterPlayDiaAction);
                                }
                                else
                                {
                                    ShowDialog(dialogueID.ToString(), goMapAction);
                                }

                            }
                            else
                            {
                                goMapAction();
                            }

                        }
                    }
                    else
                    {
                        //除第一章之外其他章节完成
                        GoBackWorldMap();
                    }
                    break;
                }
                else if (evt.Type == LTInstanceEvent.EVENT_TYPE_MAIN_CAMP_OVER_NORETURN)//副本结束，不返回大地图
                {
                    //第一章完成
                    FusionTelemetry.CampaignDate.PostEvent(FusionTelemetry.CampaignDate.Main, LTInstanceMapModel.Instance.GetMaxCampaignLevel().ToString(), 1, 1);
                    GlobalMenuManager.Instance.Open("LTStoryHud", "Chapter1_2");// 临时数据
                }
            }
        }

        protected override void OnChatBtnClick()
        {
            if (GuideNodeManager.IsGuide)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_FUNCCLICK));
                return;
            }
            base.OnChatBtnClick();
        }

        protected override void OnFriendBtnClick()
        {
            if (GuideNodeManager.IsGuide)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_FUNCCLICK));
                return;
            }
            base.OnFriendBtnClick();
        }

        private void QuickBattleEvent()
        {
            InitStarBox();
            ShowStarBox();
        }
        
        private void InitStarBox()
        {
            int curStarNum = LTInstanceUtil.GetChapterStarNum(LTInstanceMapModel.Instance.MainChapterId);
            int maxStarNum = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChapterMaxStarNumById(LTInstanceMapModel.Instance.MainChapterId);
            StarNumLabel.text = string.Format("{0}/{1}", curStarNum, maxStarNum);
            StarSlider.value = (float)curStarNum / (float)maxStarNum;

            List<int> starNumList = new List<int>(mChapterTpl.RewardDataDic.Keys);
            for (int i = 0; i < StarBoxList.Count; i++)
            {
                if (i < starNumList.Count)
                {
                    if (maxStarNum == 0) return;
                    StarBoxList[i].CustomSetActive(true);
                    StarBoxList[i].name = starNumList[i].ToString();
                    StarBoxList[i].transform.Find("Num").GetComponent<UILabel>().text = string.Format("x{0}", starNumList[i]);
                    StarBoxList[i].transform.localPosition = new Vector2(((float)starNumList[i] / (float)maxStarNum - 0.5f) * BackBarSprite.localSize.x, StarBoxList[i].transform.localPosition.y);
                    SetStarBoxTween(StarBoxList[i], false);
                    bool isOpen = LTInstanceUtil.IsChapterStarBoxOpen(LTInstanceMapModel.Instance.MainChapterId, starNumList[i]);
                    StarBoxList[i].transform.Find("Open").gameObject.CustomSetActive(isOpen);
                    StarBoxList[i].GetComponent<BoxCollider>("Box").enabled = !isOpen;
                    if (!isOpen)
                    {
                        if (curStarNum >= starNumList[i])
                        {
                            SetStarBoxTween(StarBoxList[i], true);
                        }
                    }
                }
                else
                {
                    StarBoxList[i].CustomSetActive(false);
                }
            }

        }

        private void SetStarBoxTween(GameObject starBox, bool state)
        {
            UITweener[] tweenrs = starBox.GetComponentsInChildren<UITweener>();
            for (int j = 0; j < tweenrs.Length; j++)
            {
                tweenrs[j].enabled = state;
            }
        }

        public void OnStarBoxClick(GameObject box)
        {
            int needNum = int.Parse(box.name);

            int ownNum = LTInstanceUtil.GetChapterStarNum(LTInstanceMapModel.Instance.MainChapterId);

            List<LTShowItemData> itemList = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChapterStarReward(LTInstanceMapModel.Instance.MainChapterId, needNum);
            if (itemList == null)
            {
                EB.Debug.LogError("GetLostMainChapterStarReward: not found, itemList is null, chapterId = {0}, starNum = {1}", LTInstanceMapModel.Instance.MainChapterId, needNum);
                return;
            }

            if (ownNum >= needNum)
            {
                LTInstanceMapModel.Instance.RequestMainGetStarReward(needNum, LTInstanceMapModel.Instance.MainChapterId, delegate
                {
                    if (itemList.Count > 0)
                    {
                        InitStarBox();
                        //上传友盟获得钻石，主线
                        List<LTShowItemData> mlist = itemList;
                        FusionTelemetry.ItemsUmengCurrency(itemList, "主线副本");

                        GlobalMenuManager.Instance.Open("LTShowRewardView", itemList);
                    }
                });
            }
            else
            {
                string tip = string.Format(EB.Localizer.GetString("ID_codefont_in_LTMainInstanceHudController_10032"), needNum);
                Hashtable data = Johny.HashtablePool.Claim();
                data.Add("data", itemList);
                data.Add("tip", tip);

                GlobalMenuManager.Instance.Open("LTRewardShowUI", data);
            }
        }
        
        private void InitLamp()
        {
            if (!theFirstCampaignPass)
            {
                controller.transform.Find("Edge/FireListBG").gameObject.CustomSetActive(false);
                controller.transform.Find("Edge/Bottom").gameObject.CustomSetActive(false);

            }

            int curFireNum = LTInstanceMapModel.Instance.PrayPoint;
            for (int i = 0; i < LampFireList.Count; i++)
            {
                LampFireList[i].gameObject.CustomSetActive(i < curFireNum);
            }
            LampFxObj.CustomSetActive(curFireNum >= LTInstanceMapModel.Instance.GetTotalPrayPoint());
            FireGird.Reposition();
        }
        
        public void OnLampClick()
        {
            int curFireNum = LTInstanceMapModel.Instance.PrayPoint;
            int prayPoint = LTInstanceMapModel.Instance.GetTotalPrayPoint();
            if (curFireNum >= prayPoint)
            {
                FusionAudio.PostEvent("UI/New/ShenDeng", true);
                GlobalMenuManager.Instance.Open("LTMainInstanceLampView");
            }
            else
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_MAININSTANCE_LAMP_LIMIT"));
            }
        }
        
        private bool theFirstCampaignPass
        {
            get
            {
                int IsPass = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("userCampaignStatus.normalChapters.{0}.campaigns.{1}.complete", 101, 10101), out IsPass);
                return (IsPass != 0);
            }
        }
        
        private void OnQuickBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.3f);
            int openLevel = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("mainQuickBattle");
            int passLevel = LTInstanceMapModel.Instance.GetMaxCampaignLevel();
            bool isOpen = openLevel <= passLevel;
            if (!isOpen)
            {
                var data = Data.SceneTemplateManager.Instance.GetLostMainChatpterTplById((openLevel / 100).ToString());
                if (data != null) MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_MAIN_QUICK_BATTLE_TIP"), data.Name));
                return;
            }
            IsMainQuick = !IsMainQuick;
            PlayerPrefs.SetInt("MainInstanceIsFastCombat", IsMainQuick ? 1 : 0);
            QuickSelectObj.CustomSetActive(IsMainQuick);
        }

        #region 开箱获得物品事件逻辑
        private void OpenBox(object data)
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
                    if (list.Count == 1 && (type == LTShowItemType.TYPE_RES || type == LTShowItemType.TYPE_HEROSHARD || id == LTDrawCardConfig.LOTTERY_GOLD_ID || id == LTDrawCardConfig.LOTTERY_HC_ID))
                    {
                        if (id == LTDrawCardConfig.LOTTERY_GOLD_ID || id == LTDrawCardConfig.LOTTERY_HC_ID || type == LTShowItemType.TYPE_HEROSHARD || id == LTResID.HcName)
                        {
                            FusionAudio.PostEvent("UI/New/ZuanShi", true);
                        }
                        else if (id == LTResID.GoldName)
                        {
                            FusionAudio.PostEvent("UI/New/Coin", true);
                        }
                        else if (id == LTResID.BuddyExpName)
                        {
                            FusionAudio.PostEvent("UI/New/YaoShui", true);
                        }
                        LTIconNameQuality icon_name_lvl = LTItemInfoTool.GetInfo(id, type);
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_codefont_in_LTChallengeInstanceHudController_20066"), icon_name_lvl.name, num));
                        continue;
                    }
                    LTShowItemData itemData = new LTShowItemData(id, num, type, false);
                    gamList.Add(itemData);
                }
                if (gamList.Count > 0)
                {
                    GlobalMenuManager.Instance.Open("LTShowBoxView", gamList);
                }

            }
        }
        #endregion

        #region 主线剧情对话
        private void ShowDialog(object data, System.Action callBack = null)
        {
            string storyId = data as string;
            if (!string.IsNullOrEmpty(storyId))
            {
                IsPlayDialog = true;
                mainPlot.Enter(delegate
                {
                    if (callBack != null)
                    {
                        callBack();
                    }
                    IsPlayDialog = false;
                }, storyId);

                GameObject go = GameObject.Find("Sequencer Camera");
                if (go != null)
                {
                    GameObject.Destroy(go);
                }
            }
        }
        #endregion

        #region 主线剧情动画
        private void ShowMovie(object data)
        {
            string storyId = data as string;
            GlobalMenuManager.Instance.Open("LTStoryHud", storyId);
        }
        #endregion

        #region 获得神灯表现
        private void FlyFire()
        {
            int curFireNum = LTInstanceMapModel.Instance.PrayPoint;
            if (0 < curFireNum && curFireNum <= LampFireList.Count)
            {
                GameObject fireObj = LampFireList[curFireNum - 1].gameObject;
                fireObj.SetActive(true);

                TweenPosition tweenPos = fireObj.GetComponent<TweenPosition>();
                tweenPos.from = -FireGird.transform.localPosition;// - new Vector3(0, 50000, 0);
                tweenPos.ResetToBeginning();
                tweenPos.PlayForward();

                fireObj.GetComponent<TweenScale>().ResetToBeginning();
                fireObj.GetComponent<TweenScale>().PlayForward();
            }
            LampFxObj.CustomSetActive(curFireNum >= LTInstanceMapModel.Instance.GetTotalPrayPoint());
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

        #region  主线副本获得英雄
        private void OnGetHero(LTInstanceEvent evt)
        {
            Instance.LTInstanceFloorTemp tmp = MapCtrl.GetNodeObjByPos(new Vector2(evt.x, evt.y)) as Instance.LTInstanceFloorTemp;
            if (tmp != null)
            {
                tmp.HideNotice();
            }
        }
        #endregion

        #region  返回大地图
        private void GoBackWorldMap()
        {
            GlobalMenuManager.Instance.ClearCache();
            GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");

            OnCancelButtonClick();
        }
        #endregion

        #region 星级宝箱奖励自动领取功能
        public void ShowStarBox()
        {
            //检查宝箱是否可领取，可领取情况帮忙自动领取
            int starNum = LTInstanceUtil.GetChapterCurStarNum(mChapterTpl.Id);
            foreach (int key in mChapterTpl.RewardDataDic.Keys)
            {
                if (starNum >= key)
                {
                    if (!LTInstanceUtil.IsChapterStarBoxOpen(mChapterTpl.Id, key))
                    {
                        isShowingBoxReward = true;
                        LTInstanceMapModel.Instance.RequestMainGetStarReward(key, LTInstanceMapModel.Instance.MainChapterId, delegate
                        {
                            List<LTShowItemData> itemList = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChapterStarReward(LTInstanceMapModel.Instance.MainChapterId, key);
                            if (itemList != null && itemList.Count > 0)
                            {
                                InitStarBox();
                                //上传友盟获得钻石，主线
                                FusionTelemetry.ItemsUmengCurrency(itemList, "主线副本");
                                System .Action callback =delegate { isShowingBoxReward = false; } ;
                                var ht = Johny.HashtablePool.Claim();
                                ht["reward"] = itemList;
                                ht["callback"] = callback;
                                ht["title"] =EB.Localizer .GetString("ID_MAIN_INSTANCE_BOX_REWRAD_TIP");
                                GlobalMenuManager.Instance.Open("LTShowBoxView", ht);
                            }
                            else
                            {
                                isShowingBoxReward = false;
                            }
                        });
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
