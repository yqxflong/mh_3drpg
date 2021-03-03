using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTChallengeInstanceSelectHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<LTChallengeInstanceSelectDynamicScroll>("Center/LevelList/Placeholder/Grid");
            BlitzObj= t.FindEx("Center/Drop/BlitzBtn").gameObject;
            BlitzRequest = BlitzObj.transform.GetComponent<UIServerRequest>("Request");
            BlitzCostLabel = t.GetComponent<UILabel>("Center/Drop/BlitzBtn/Num");
            RecommendLevelLabel = t.GetComponent<UILabel>("Center/Drop/RecommendLevelLabel");
            RecommendBGSprite = t.GetComponent<UISprite>("Center/Drop/RecommendLevelLabel/BG");
            controller. backButton = t.GetComponent<UIButton>("Edge/TopLeft/BackBtn");

            BGFxList = new List<ParticleSystemUIComponent>();
            BGFxList.Add(t.GetComponent<ParticleSystemUIComponent>("BGFX/Green"));
            BGFxList.Add(t.GetComponent<ParticleSystemUIComponent>("BGFX/Blue"));
            BGFxList.Add(t.GetComponent<ParticleSystemUIComponent>("BGFX/Purple"));
            BGFxList.Add(t.GetComponent<ParticleSystemUIComponent>("BGFX/Gold"));
            BGFxList.Add(t.GetComponent<ParticleSystemUIComponent>("BGFX/Red"));

            DropItemList = new List<LTShowItem>();
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Center/Drop/ItemList/Item"));
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Center/Drop/ItemList/Item (1)"));
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Center/Drop/ItemList/Item (2)"));
            DropItemList.Add(t.GetMonoILRComponent<LTShowItem>("Center/Drop/ItemList/Item (3)"));

            t.GetComponent<UIButton>("Edge/TopRight/Notice").onClick.Add(new EventDelegate(OnNoticeClick));
            t.GetComponent<UIButton>("Center/Drop/BlitzBtn").onClick.Add(new EventDelegate(OnBlitzBtnClick));
            t.GetComponent<UIButton>("Center/Drop/EnterBtn").onClick.Add(new EventDelegate(OnEnterBtnClick));
            
            BlitzRequest.onResponse.Add(new EventDelegate(controller, "OnFetchData"));
        }
        
        public List<ParticleSystemUIComponent> BGFxList;
        public LTChallengeInstanceSelectDynamicScroll DynamicScroll;
        public List<LTShowItem> DropItemList;
        public GameObject BlitzObj;
        public UIServerRequest BlitzRequest;
        public UILabel BlitzCostLabel;
        public UILabel RecommendLevelLabel;
        public UISprite RecommendBGSprite;
    
        private bool showBtn = false;
        private int maxCheckPointLevel = 0;
        public static int CurSelectLevel = 0;

        public override bool IsFullscreen() { return true; }
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            GetBlitzCostInfo();
            LTInstanceMapModel.Instance.RequestChallengeLevelInfo(delegate { InitUI(); });
            Hotfix_LT.Messenger.AddListener<System .Action>(EventName.PlayCloudFXCallback, PlayCloudFxFunc);
            Hotfix_LT.Messenger.AddListener<int>(EventName.LTChallengeInstanceLevelSelect, OnLevelSelect);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener<System.Action>(EventName.PlayCloudFXCallback, PlayCloudFxFunc);
            Hotfix_LT.Messenger.RemoveListener<int>(EventName.LTChallengeInstanceLevelSelect, OnLevelSelect);
            CurSelectLevel = 0;
            StopAllCoroutines();
            CloudFx = null;
            DestroySelf();
            yield break;
        }
    
        private int hcCost;
        private int blitzCost;
        private void GetBlitzCostInfo()
        {
            ArrayList aList = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("ChallengeInstanceBlitzCost")) as ArrayList;
            
            hcCost = 0;
            blitzCost = 0;
            for (int i = 0; i < aList.Count; i++)
            {
                string id = EB.Dot.String("data", aList[i], string.Empty);
                int count = EB.Dot.Integer("quantity", aList[i], 0);
                if (id.Equals("hc"))
                {
                    hcCost = count;
                }
                if (id.Equals("chall-camp-point"))
                {
                    blitzCost = count;
                }
            }
    
            BlitzCostLabel.text = hcCost.ToString();
        }
    
        /// <summary>
        /// 初始化挑战副本关卡信息
        /// </summary>
        /// <returns></returns>
        private void InitUI()
        {
            StartCoroutine(InitUIIEnuMerator());
        }
    
        private IEnumerator InitUIIEnuMerator()
        {
            InitLevelList();
            yield return null;
            InitSelect();
            Hotfix_LT.Messenger.Raise(EventName.LTChallengeInstanceLevelSelect, CurSelectLevel);
            yield return null;
            InitCurLevel();
        }

        /// <summary>
        /// 更新挑战副本关卡ItemList
        /// </summary>
        private void InitLevelList()
        {
            List<Hotfix_LT.Data.LostChallengeChapter> list = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeCheckPointChapterList();
            DynamicScroll.SetItemDatas ( list.ToArray());
        }
    
        /// <summary>
        /// 更新掉落物品信息
        /// </summary>
        private void InitDrop()
        {
            int floor = LTInstanceUtil.GetChallengeLevel(CurSelectLevel);
            Hotfix_LT.Data.LostChallengeRewardTemplate temp= Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeReward(System.DateTime.Now.DayOfWeek, floor);
            List<string> itemList = temp.DropList;
            if (itemList != null)
            {
                for (int i = 0; i < DropItemList.Count; i++)
                {
                    if (i < itemList.Count)
                    {
                        DropItemList[i].mDMono.gameObject.CustomSetActive(true);
                        DropItemList[i].LTItemData = new LTShowItemData(itemList[i], 0, LTShowItemType.TYPE_GAMINVENTORY);
                        DropItemList[i].SetDropRateText(string.Format("{0}%", temp.DropRate * 100));
                    }
                    else
                    {
                        DropItemList[i].mDMono.gameObject.CustomSetActive(false);
                    }
                }
            }
            if (DropItemList.Count > 0)
            {
                if (BGFxList != null)
                {
                    int select= DropItemList[0].QualityLevel - 2;
                    for (var i = 0; i < BGFxList.Count; ++i)
                    {
                        var fx = BGFxList[i];
                        fx.gameObject.CustomSetActive(i == select);
                    }
                }
            }

            ShowHideItemEquipmentWish();
        }
    
        /// <summary>
        /// 初始化初始选择Item
        /// </summary>
        private void InitSelect()
        {
            int maxLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);
            maxLevel += 1;

            Hotfix_LT.Data.LostChallengeChapter temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetCheckPointChapterByChapter(maxLevel);
            if (temp==null)//满层通关
            {
                maxCheckPointLevel = 1000;
                temp= Hotfix_LT.Data.SceneTemplateManager.Instance.GetCheckPointChapterByChapter(maxLevel-1);
            }
            else
            {
                maxCheckPointLevel = temp.Level;
            }

            CurSelectLevel = temp.Level;
    
            List<Hotfix_LT.Data.LostChallengeChapter> list = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeCheckPointChapterList();
            int index  = list.IndexOf(temp);
            DynamicScroll.MoveInternalNow((index - 4 < 0) ? 0 : index - 4);
            InitDrop();

            BlitzObj.CustomSetActive(GetShowBlitzObj());
            RecommendLevelLabel.gameObject.CustomSetActive(temp.RecommendLevel > 0 &&maxCheckPointLevel <= CurSelectLevel);
            if (temp.RecommendLevel > 0 && maxCheckPointLevel <= CurSelectLevel)
            {
                RecommendLevelLabel.text = string.Format("{0}:{1}", EB.Localizer.GetString("ID_CHALLENGE_RECOMMEND"), temp.RecommendLevel);
                int userLevel = BalanceResourceUtil.GetUserLevel();
                RecommendBGSprite.spriteName = (userLevel >= temp.RecommendLevel) ? "Ty_Welfare_Label_5" : "Welfare_Label_6";
            }
        }
    
        /// <summary>
        /// 恢复挑战界面事件
        /// </summary>
        private void InitCurLevel()
        {
            int curLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.curLevel", out curLevel);
    
            if (curLevel > 0)
            {
                Hashtable actTable = Johny.HashtablePool.Claim();
    
                actTable["okAction"] = new System.Action(delegate
                {
                    LTChallengeInstanceHudController.EnterInstance(curLevel, true);
                });
    
                actTable["cancelAction"] = new System.Action(delegate
                {
                    controller.Close();
                });
    
                GlobalMenuManager.Instance.Open("LTChallengeInstanceLoadView", actTable);
            }
        }
    
        /// <summary>
        /// 关卡难度选择事件
        /// </summary>
        /// <param name="evt"></param>
        public void OnLevelSelect(int level)
        {
            if (level != CurSelectLevel)
            {
                CurSelectLevel = level;
                Hotfix_LT.Data.LostChallengeChapter temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetCheckPointChapter(CurSelectLevel);
                InitDrop();
                BlitzObj.CustomSetActive(GetShowBlitzObj());
                RecommendLevelLabel.gameObject.CustomSetActive(temp.RecommendLevel > 0 && maxCheckPointLevel <= CurSelectLevel);
                if (temp.RecommendLevel > 0 && maxCheckPointLevel <= CurSelectLevel)
                {
                    RecommendLevelLabel.text = string.Format("{0}:{1}", EB.Localizer.GetString("ID_CHALLENGE_RECOMMEND"), temp.RecommendLevel);
                    int userLevel = BalanceResourceUtil.GetUserLevel();
                    RecommendBGSprite.spriteName = (userLevel >= temp.RecommendLevel) ? "Ty_Welfare_Label_5" : "Welfare_Label_6";
                }
            }
        }

        public bool GetShowBlitzObj()
        {
            if (VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.CanBlitzChallCampaign) > 0)
            {
                return maxCheckPointLevel > CurSelectLevel;
            }

            return false;
        }
    
        /// <summary>
        /// 进入挑战副本按钮点击
        /// </summary>
        public void OnEnterBtnClick()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID("res.chall-camp-point.v", out num);
            if (num <= 0)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstancePortalCtrl_1451"));
                return;
            }

            System.Action callback = delegate {
                LTChallengeInstanceHudController.EnterInstance(CurSelectLevel, false);
                LTInstanceMapModel.Instance.IsInstanceShowTheme = true;
                //上报友盟，开始挑战关卡
                FusionTelemetry.PostStartCombat(eBattleType.ChallengeCampaign.ToString() + CurSelectLevel);
            };
            Hotfix_LT.Messenger.Raise(EventName.PlayCloudFXCallback, callback);
        }
    
        /// <summary>
        /// 扫荡按钮点击
        /// </summary>
        public void OnBlitzBtnClick()
        {
            int hcnum = 0;//判断钻石是否足够
            DataLookupsCache.Instance.SearchIntByID("res.hc.v", out hcnum);
            if (hcnum < hcCost)
            {
                MessageTemplateManager.ShowMessage(901030, null, delegate (int r)
                {
                    if (r == 0)
                    {
                        InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
						GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);						
                    }
                });
                return;
            }
    
            MessageTemplateManager.ShowMessage(902308, null, delegate (int result)
            {
                if (result == 0)
                {
                    //判断券是否足够
                    int num = 0;//判断钻石是否足够
                    DataLookupsCache.Instance.SearchIntByID("res.chall-camp-point.v", out num);
                    if (num < blitzCost)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTChallengeInstancePortalCtrl_1451"));               
                        return;
                    }
                    showBtn = false;//true;
                    BlitzRequest.parameters[0].parameter = CurSelectLevel.ToString();
                    BlitzRequest.parameters[1].parameter = "0";
                    LoadingSpinner.Show();
                    curHcCount = BalanceResourceUtil.GetUserDiamond();
                    BlitzRequest.SendRequest();
                }
            });
        }
    
        private int curHcCount=0;
        /// <summary>
        /// 扫荡回调
        /// </summary>
        /// <param name="res"></param>
        public override void OnFetchData(EB.Sparx.Response res, int RequestID)
        {
            LoadingSpinner.Hide();

            if (res.sucessful)
            {
                //任务回调
                DataLookupsCache.Instance.CacheData(res.hashtable);

                if (res.hashtable != null && res.hashtable["BlitzResult"] != null)
                {
                    List<LTShowItemData> list = new List<LTShowItemData>();
                    Dictionary<string, LTShowItemData> dic = new Dictionary<string, LTShowItemData>();
                    ArrayList BlitzList = EBCore.Dot.Array("BlitzResult", res.hashtable, null);

                    if (BlitzList != null)
                    {
                        ArrayList wishRewards;
                        DataLookupsCache.Instance.SearchDataByID(string.Format("tl_acs.{0}.wish_reward", LTEquipmentWishController.equipmentWishActivityId), out wishRewards);

                        if (wishRewards == null)
                        {
                            wishRewards = new ArrayList();
                        }

                        for (var i = 0; i < BlitzList.Count; i++)
                        {
                            object entry = BlitzList[i];
                            string id = EB.Dot.String("data", entry, string.Empty);
                            int num = EB.Dot.Integer("quantity", entry, 0);
                            bool fromWish = EB.Dot.Bool("wishReward", entry, false);

                            if (!string.IsNullOrEmpty(id) && num >= 0)
                            {
                                if (dic.ContainsKey(id))
                                {
                                    dic[id].count += num;
                                }
                                else
                                {
                                    string type = EB.Dot.String("type", entry, string.Empty);
                                    dic.Add(id, new LTShowItemData(id, num, type, isFromWish:fromWish));
                                }

                                if (fromWish)
                                {
                                    for (var j = 0; j < num; j++)
                                    {
                                        wishRewards.Add(id);
                                    }
                                }
                            }
                        }

                        // 套装许愿数量刷新
                        if (wishRewards != null && wishRewards.Count > 0)
                        {
                            DataLookupsCache.Instance.CacheData(string.Format("tl_acs.{0}.wish_reward", LTEquipmentWishController.equipmentWishActivityId), wishRewards);
                        }

                        foreach (var value in dic.Values)
                        {
                            list.Add(value);
                        }
                    }

                    if (showBtn)
                    {
                        Hashtable data = Johny.HashtablePool.Claim();
                        data.Add("reward", list);
                        data.Add("showBtn", showBtn);
                        data.Add("positive", new System.Action(OnDoubleBlitz));
                        data.Add("positiveDesc", EB.Localizer.GetString("ID_CHALLENGE_BLITZ_POSITIVE"));
                        data.Add("costType", "hc");
                        data.Add("costNum", 200);
                        data.Add("negative", null);
                        data.Add("negativeDesc", EB.Localizer.GetString("ID_CHALLENGE_BLITZ_NEGATIVE"));
                        GlobalMenuManager.Instance.Open("LTShowDoubleRewardView", data);
                    }
                    else
                    {
                        Hashtable data = Johny.HashtablePool.Claim();
                        data.Add("reward", list);
                        Hashtable wheeldata = EB.Dot.Object("wheelData", res.hashtable, null);

                        if (wheeldata != null)
                        {
                            data.Add("callback", new System.Action(delegate {
                                int count = EB.Dot.Integer("count", wheeldata, 0);
                                var wheelList = EB.Dot.Array("wheelData", wheeldata, null);
                                List<LTShowItemData> dataList = null;

                                if (wheelList != null)
                                {
                                    dataList = new List<LTShowItemData>();

                                    for (int i = 0; i < wheelList.Count; i++)
                                    {
                                        var wheelData = wheelList[i];
                                        dataList.Add(new LTShowItemData(wheelData));
                                    }
                                }

                                Hashtable Setwheeldata = Johny.HashtablePool.Claim();
                                Setwheeldata.Add("type", LTInstanceConfig.OutChallengeState);
                                Setwheeldata.Add("count", count);
                                Setwheeldata.Add("list", dataList);
								TimerManager.instance.AddFramer(1, 1, (t) => { GlobalMenuManager.Instance.Open("LTChallengeInstanceTurntableView", Setwheeldata); });								
                            }));
                        }         
                        
                        GlobalMenuManager.Instance.Open("LTShowRewardView", data);
                    }
                }

                //存入新装备数据
                if (res.hashtable != null && res.hashtable["inventory"] != null)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("inventory", res.hashtable["inventory"]);
                    DataLookupsCache.Instance.CacheData(ht);
                    Johny.HashtablePool.Release(ht);
                }

                if (curHcCount - BalanceResourceUtil.GetUserDiamond() > 0)
                {
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, BalanceResourceUtil.GetUserDiamond() - curHcCount, "挑战扫荡");
                }
            }
            else if (res.fatal)
            {
                SparxHub.Instance.FatalError(res.localizedError);
            }
        }
    
        /// <summary>
        /// 二次扫荡方法
        /// </summary>
        private void OnDoubleBlitz()
        {
            showBtn = false;
            BlitzRequest.parameters[0].parameter = CurSelectLevel.ToString();
            BlitzRequest.parameters[1].parameter = "1";
            BlitzRequest.SendRequest();
        }
    
        /// <summary>
        /// 打开掉落信息界面
        /// </summary>
        public void OnNoticeClick()
        {
            GlobalMenuManager.Instance.Open("LTChallengeInstanceDropView");
        }
    
        /// <summary>
        /// 打开资源购买商店
        /// </summary>
        public void OnAddBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        }

        Coroutine CloudFx =null;
        /// <summary>
        /// 开云特效方法
        /// </summary>
        /// <param name="evt"></param>
        private void PlayCloudFxFunc(System.Action Action)
        {
            if (Action != null && CloudFx == null)
            {
                UIStack.Instance.ShowLoadingScreen(Action, false, true, true);
            }
        }

        private LTShowItem _itemWish;
        private GameObject _itemWishChange;
        private UIEventTrigger _itemWishEventTrigger;
        private int _activityFuncId = 10079;           // 活动(10079单独走一条)
        private int _equipmentWishActivityId = 6518;   // 套装许愿活动id：6518
        private int _equipmentWishActivityEndTime = -1;

        private void ShowHideItemEquipmentWish()
        {
            if (_itemWish == null)
            {
                var go = controller.gameObject;
                _itemWish = go.GetMonoILRComponent<LTShowItem>("Center/Drop/ItemList/ItemWish");
                _itemWishChange = go.FindEx("Center/Drop/ItemList/ItemWish/Change");
                _itemWishEventTrigger = go.GetComponent<UIEventTrigger>("Center/Drop/ItemList/ItemWish");

                if (_itemWishEventTrigger == null)
                {
                    _itemWishEventTrigger = go.FindEx("Center/Drop/ItemList/ItemWish").AddComponent<UIEventTrigger>();
                }

                _itemWishEventTrigger.onClick.Add(new EventDelegate(() =>
                {
                    if (_equipmentWishActivityEndTime != -1 && EB.Time.Now > _equipmentWishActivityEndTime)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));
                        return;
                    }

                    //副本进行中不能更换许愿装备
                    //int curLevel = 0;
                    //DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.curLevel", out curLevel);

                    //if (curLevel > 0)
                    //{
                    //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIPMENT_WISH_NOT_REPLACEMENT_TIPS"));
                    //    return;
                    //}

                    System.Action<int> callback = SetItemEquipmentWish;
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    GlobalMenuManager.Instance.Open("LTEquipmentWishUI", callback);
                }));
            }

            if (_itemWish == null)
            {
                return;
            }

            Data.FuncTemplate ft = Data.FuncTemplateManager.Instance.GetFunc(_activityFuncId);
            bool isShow = false;

            if (ft != null && ft.IsConditionOK())
            {
                ArrayList eventlist;
                DataLookupsCache.Instance.SearchDataByID("events.events", out eventlist);
                 
                if (eventlist != null) 
                {
                    for (var i = 0; i < eventlist.Count; i++)
                    {
                        var e = eventlist[i];
                        int activityId = EB.Dot.Integer("activity_id", e, 0);
                        
                        if (activityId == _equipmentWishActivityId)
                        {
                            int start = EB.Dot.Integer("start", e, 0);
                            _equipmentWishActivityEndTime = EB.Dot.Integer("end", e, 0);

                            if (EB.Time.Now >= start && EB.Time.Now <= _equipmentWishActivityEndTime)
                            {
                                isShow = true;
                                break;
                            }
                        }
                    }
                }
            }
            
            _itemWish.mDMono.gameObject.SetActive(isShow);

            var grid = _itemWish.mDMono.transform.parent.GetComponent<UIGrid>();

            if (grid != null)
            {
                grid.Reposition();
            }

            int itemId;

            if (DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current", _equipmentWishActivityId), out itemId))
            {
                SetItemEquipmentWish(itemId);
            }
        }

        private void SetItemEquipmentWish(int itemId)
        {
            if (_itemWish != null)
            {
                _itemWish.LTItemData = new LTShowItemData(itemId.ToString(), 0, LTShowItemType.TYPE_GAMINVENTORY, isFromWish:true);
                _itemWishChange.CustomSetActive(itemId > 0);
            }
        }
    }
}
