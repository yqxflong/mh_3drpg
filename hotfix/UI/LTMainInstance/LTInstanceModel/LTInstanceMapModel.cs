using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EB.Sparx;
using System.Text;
using Hotfix_LT.Data;
using Debug = EB.Debug;
using UnityEngine.U2D;

namespace Hotfix_LT.UI
{
    public class ScrollData
    {
        public int x;
        public int y;
        public int[] list;

        public ScrollData(int x, int y, int[] list)
        {
            this.x = x;
            this.y = y;
            this.list = list;
        }
    }

    public class LTInstanceMapModel : ManagerUnit
    {
        private static LTInstanceMapModel instance = null;
        public static LTInstanceMapModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = LTHotfixManager.GetManager<LTInstanceMapModel>();
                }
                return instance;
            }
        }

        public override void Initialize(Config config)
        {
            mChallengeInstanceApi = new LTChallengeInstanceApi();
            mChallengeInstanceApi.ErrorHandler += ErrorHandler;

            mMainInstanceApi = new LTMainInstanceApi();
            mMainInstanceApi.ErrorHandler += ErrorHandler;

            mAlienMazeInstanceApi = new LTAlienMazeInstanceApi();
            mAlienMazeInstanceApi.ErrorHandler += ErrorHandler;

            mMonopolyInstanceApi = new LTMonopolyInstanceApi();
            mMonopolyInstanceApi.ErrorHandler += ErrorHandler;

            NodeDataHashDic = new Dictionary<int, LTInstanceNode>();
            WebNodeDataDic = new Dictionary<int, LTInstanceNode>();
            DealDirQueue = new Queue<int>();
            RequestDirQueue = new Queue<int>();
            EventUpdateList = new List<int>();
            BoobMasterList = new List<int>();
            ReflashDataHashList = new List<int>();

            Hotfix_LT.Messenger.AddListener(EventName.InstanceViewActionHudLoad, InstanceViewAction);
            Hotfix_LT.Messenger.AddListener<long, int>(EventName.ChallengeConfirmFastCombat, ChallengeConfirmFastCombat);
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, DataRefresh);
        }

        public override void Connect()
        {
            InstanceType = LTInstanceConfig.InstanceType.None;
        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            ResetAlienMazeInit();
            isRequesting = false;
            ILRTimerManager.instance.RemoveTimerSafely(ref isRequestingTimer);
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            if (response.error.ToString() == "in combat")
            {
                //需要一个通用api组件，调用通用请求，比如getplaystate这种
                Request r = mChallengeInstanceApi.Get("/playstate/getPlayState");
                mChallengeInstanceApi.Service(r, (result) =>
                {
                    if (result != null)
                    {
                        DataLookupsCache.Instance.CacheData(result);
                        if (GameFlowControlManager.Instance != null)
                        {
                            GameFlowControlManager.Instance.SendEvent("GoToMainLandView");
                            return;
                        }
                    }
                });
                return true;
            }
            return false;
        }

        //字段
        public Queue<object> moveResultList = new Queue<object>();
        public Queue<LTInstanceEvent> EventList = new Queue<LTInstanceEvent>();
        public Dictionary<int, LTInstanceNode> NodeDataHashDic;
        public Queue<int> DealDirQueue;
        public Queue<int> RequestDirQueue;
        public ScrollData scrollData;
        public bool IsTemporaryQuit = false;
        public bool IsEnterChallenge = false;
        public bool IsEnterAlienMaze = false;
        public SpriteAtlas InstanceMapStyleAtlas;
        public SpriteAtlas InstanceRoleAtlas;
        public LTInstanceNode CurNode;
        public bool isFirstCreatFloor = true;
        public int CurMapStyle = 0;
        public string MainChapterId = string.Empty;
        public int PrayPoint = 0;
        public bool IsInitPraypoint = false;
        public int CurLevelNum = 0;
        public int ChallengeThemeId = 0;
        public string ChallengeThemeEnvStr = string.Empty;
        public int StartHash = 0;
        public int DoorHash = 0;
        public int SpecialBoob = 0;
        public bool OpenQuickBattle = false;
        public bool IsInstanceShowTheme = false;
        public bool WaitForDialoging = false;

        private static string[] ints_names = { "flag", "x", "y", "terra", "originTerra", "role" };
        private static int[] ints_defaultValue = { 0, -1, -1, 0, 0, 0 };
        private static string[] rd_ints_names = { "stage", "star", "Bomb", "skillid" };
        private static int[] rd_ints_defaultValue = { 0, 0, 0, 0 };
        private static string[] ad_ints_names = { "hire", "count" };
        private static int[] ad_ints_defaultValue = { 0, 0 };
        private static string[] _GetMajorNode_ints_names = { "pos.x", "pos.y" };
        private static int[] _GetMajorNode_ints_defaultValue = { 0, 0 };
        
        private LTAlienMazeInstanceApi mAlienMazeInstanceApi;
        private LTMainInstanceApi mMainInstanceApi;
        private LTChallengeInstanceApi mChallengeInstanceApi;
        private LTMonopolyInstanceApi mMonopolyInstanceApi;

        private List<int> hashViewList = new List<int>(8);
        private bool hasCampOver = false;
        private int EventTimer = 0;
        private int WidthCount = 3;
        private int HeightCount = 3;
        private int AtlasLoadingCount = 0;
        private int curAtlasIndex = 0;
        private bool HasInit = false;
        private bool HasMaze = false;
        private bool isRequesting = false;
        private int isRequestingTimer = 0;
        private bool IsFinishingInstance = false;
        private System.Action RequestChallengeFinshChapterAction = null;
        private bool hasSpecialBoobUpdate = false;
        private Dictionary<int, LTInstanceNode> WebNodeDataDic;
        private List<int> ReflashDataHashList;
        private bool IsChallengeMapFix = false;
        private int BossRewardHashX = 0;
        private int BossRewardHashY = 0;
        private List<int> EventUpdateList;
        private List<int> BoobMasterList;
        private bool IsChallengeFinished = false;
        private LTInstanceConfig.InstanceType InstanceType = LTInstanceConfig.InstanceType.None;
        

        #region  监听方法
        private void ChallengeConfirmFastCombat(long combatId, int confirm)
        {
            LTInstanceMapModel.Instance.RequestChallengeConfirmFastCombat(combatId, confirm);
            if (confirm == 1)
            {
                LTChallengeInstanceHpCtrl.UpdateInstanceHpFromCombat(true);
            }
        }

        private void DataRefresh()
        {
            if (GetChallengeRP() || GetBoxRewardRP() || GetAlienMazeRP() || GetMainChapterRewardRP()) { }
        }
        #endregion 

        #region  副本状态相关
        public bool IsInsatnceViewAction()
        {
            if (GameStateManager.Instance != null)
            {
                return GameFlowControlManager.IsInView("InstanceView");
            }
            return false;
        }

        public void SwitchViewAction(bool isEnter, bool showCloud = false, System.Action callback = null)
        {
            if (isEnter)
            {
                string state = "";
                DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
                if (state != PlayState.PlayState_LCCampaign)
                {
                    SparxHub.Instance.Disconnect(true);
                }
            }

            if (GameStateManager.Instance != null)
            {
                GameStateLoadGame loadState = GameStateManager.Instance.GetGameState<GameStateLoadGame>();
                if (loadState != null)
                {
                    if (showCloud)
                    {
                        UIStack.Instance.ShowLoadingScreen(delegate
                        {
                            LTHotfixManager.GetManager<SceneManager>().EnterSceneByPlayState();
                            callback?.Invoke();
                        }, false, true);
                    }
                    else
                    {
                        LTHotfixManager.GetManager<SceneManager>().EnterSceneByPlayState();
                    }
                }
            }
        }

        private void InstanceViewAction()
        {
            string mCurChapterId = string.Empty;
            int mCurLevelId = 0;
            int mCurMazeId = 0;
            int mCurSelectLevel = 0;
            bool AlreadyInInstance = InstanceType != LTInstanceConfig.InstanceType.None;
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.LCCampaign.LCchapter", out mCurChapterId);
            DataLookupsCache.Instance.SearchIntByID("playstate.LCCampaign.LClevel", out mCurLevelId);
            DataLookupsCache.Instance.SearchIntByID("playstate.LCCampaign.awId", out mCurMazeId);
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.curLevel", out mCurSelectLevel);

            if(InstanceType == LTInstanceConfig.InstanceType.MonopolyInstance)
            {
                GlobalMenuManager.Instance.Open("LTMonopolyInstanceHud");
                return;
            }

            if (!string.IsNullOrEmpty(mCurChapterId))
            {
                if (!AlreadyInInstance)
                {
                    LTInstanceMapModel.Instance.RequestMainEnterChapter(int.Parse(mCurChapterId), delegate
                    {
                        GlobalMenuManager.Instance.Open("LTMainInstanceView", mCurChapterId);
                    });
                }
                else
                {
                    GlobalMenuManager.Instance.Open("LTMainInstanceView", mCurChapterId);
                }
            }
            else if (mCurLevelId != 0)
            {
                if (!AlreadyInInstance)
                {
                    LTInstanceMapModel.Instance.RequestChallengeResumeChapter(mCurSelectLevel, delegate (string error)//重走登录后自动重连
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            LTInstanceMapModel.Instance.SwitchViewAction(true);
                            return;
                        }
                        GlobalMenuManager.Instance.Open("LTChallengeInstanceView", mCurSelectLevel);
                    });
                }
                else
                {
                    GlobalMenuManager.Instance.Open("LTChallengeInstanceView", mCurSelectLevel);//正常进入和战斗后返回
                }
            }
            else if (mCurMazeId != 0)
            {
                if (!AlreadyInInstance)
                {
                    LTInstanceMapModel.Instance.RequestChallengeResumeChapter(mCurMazeId, delegate (string error)//重走登录后自动重连
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            LTInstanceMapModel.Instance.SwitchViewAction(true);
                            return;
                        }
                        GlobalMenuManager.Instance.Open("LTAlienMazeInstanceHud", mCurMazeId);
                    }, LTInstanceConfig.AlienMazeTypeStr);
                }
                else
                {
                    GlobalMenuManager.Instance.Open("LTAlienMazeInstanceHud", mCurMazeId);//正常进入和战斗后返回
                }
            }
        }
        #endregion

        #region 数据更新相关
        public void ApplyChapterData(object data, Action CallBack = null)
        {
            InitWebNodeDicToNodeDic();
            var hash = data as Hashtable;
            if (hash == null || hash.Count == 0) return;

            InitLevelData(data);
            InitPraypointData(data);

            InitMapData(data, CallBack);//delay 更新地图
            InitEventData(data);//delay 更新事件

            IsChallengeFinished = EB.Dot.Bool("userCampaignStatus.challengeChapters.finished", data, false);
            bool allDied = FormationUtil.IsAllPartnerDied();
            if (IsChallengeFinished || allDied)
            {
                Hotfix_LT.Messenger.Raise(EventName.OnChallengeFinished);
            }

            //生成环境特效
            Hotfix_LT.Instance.LTInstanceOptimizeManager.Instance?.GenEnvironmentEffect(CurMapStyle);
        }

        public void ApplySimulData()
        {
            int dir = DealDirQueue.Dequeue();
            DealBeforeEventData();
            ReWriteMapDatas(dir);
            ReWriteEventData(dir);
            if(InstanceType !=LTInstanceConfig.InstanceType.MonopolyInstance)RequestDirQueue.Enqueue(dir);
        }

        public void EventUpdateData()
        {
            InitWebNodeDicToNodeDic(true);
            if (NodeDataHashDic.Count > 0)
            {
                RefreshMapDataAndDisplay();
            }
            RecalcNodeRelation();
        }

        private void InitChapterData(object data)
        {
            moveResultList.Enqueue(data);
            GlobalMenuManager.CurGridMap_MajorDataUpdateFunc();

            RefreshPlayerNode(data);
        }

        private void InitMapData(object data, Action CallBack = null)
        {
            var mapdataList = Hotfix_LT.EBCore.Dot.Array("userCampaignStatus.challengeChapters.mapdata", data, null);
            if (mapdataList == null) return;

            hasSpecialBoobUpdate = SpecialBoob != 0 && ((CurNode.y * 100 + CurNode.x) != GetMajorPosHash(data));

            scrollData = null;
            int mdCnt = mapdataList.Count;
            for (int i = 0; i < mdCnt; ++i)
            {
                var md = mapdataList[i];
                InitMapDataGrid(md);
            }

            if (scrollData != null && !FormationUtil.IsAllPartnerDied())
            {
                GlobalMenuManager.Instance.Open("LTShowReelView");
            }
            if (hasSpecialBoobUpdate)
            {
                hasSpecialBoobUpdate = false;
                DealSpcialBoobData();
            }

            //刷新格子邻居节点
            RecalcNodeRelation();

            //获取角色节点
            RefreshPlayerNode(data);

            if (CallBack != null)
            {
                CallBack();
            }

            //刷新格子数据和显示
            RefreshMapDataAndDisplay(true);
        }

        private void InitLevelData(object data)
        {
            CurLevelNum = EB.Dot.Integer("userCampaignStatus.challengeChapters.level", data, 0);
            CurMapStyle = EB.Dot.Integer("userCampaignStatus.challengeChapters.style", data, 0);
            LoadMapStyleSpriteAtlas();
            if (ChallengeThemeId != EB.Dot.Integer("userCampaignStatus.challengeChapters.majordata.envId", data, 0))
            {
                ChallengeThemeId = EB.Dot.Integer("userCampaignStatus.challengeChapters.majordata.envId", data, 0);
                var envTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeEnvById(LTInstanceMapModel.Instance.ChallengeThemeId);
                if (envTpl != null)
                {
                    if (!string.IsNullOrEmpty(envTpl.EnvLogic) && envTpl.EnvLogic.Contains("D")) ChallengeThemeEnvStr = "D";//瘟疫
                    else if (!string.IsNullOrEmpty(envTpl.EnvLogic) && envTpl.EnvLogic.Contains("H")) ChallengeThemeEnvStr = "H";//生机
                    else ChallengeThemeEnvStr = string.Empty;
                }
                var temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(CurLevelNum);
                if (temp != null)
                {
                    IsChallengeMapFix = temp.IsFixMap;
                }
            }
            Hotfix_LT.Messenger.Raise(EventName.OnLevelDataUpdate);
        }

        private void InitEventData(object data)
        {
            ArrayList eventList = Hotfix_LT.EBCore.Dot.Array("userCampaignStatus.challengeChapters.events", data, null);
            if (eventList == null) return;

            hasCampOver = false;
            bool HasHPInfoData = EB.Dot.Object(LTChallengeInstanceHpCtrl.curHpInfoPath, data, null).Count > 0;
            for (int i = 0; i < eventList.Count; i++)
            {
                string type = EB.Dot.String("event", eventList[i], string.Empty);
                int x = EB.Dot.Integer("x", eventList[i], 0);
                int y = EB.Dot.Integer("y", eventList[i], 0);
                LTInstanceEvent evt = new LTInstanceEvent(type, x, y);
                evt.HasHPInfoData = HasHPInfoData;
                evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", eventList[i]);
                EventList.Enqueue(evt);
            }

            foreach (var instanceEvent in EventList)
            {
                if (instanceEvent.Type == LTInstanceEvent.EVENT_TYPE_MAIN_CAMP_OVER || (instanceEvent.Type == LTInstanceEvent.EVENT_TYPE_MAIN_CAMP_OVER_NORETURN && !GuideNodeManager.IsGuide))
                {
                    hasCampOver = true;
                    break;
                }
            }

            if (EventList.Count > 0)
            {
                RaiseEvent();
            }
        }

        private void InitPraypointData(object data)
        {
            PrayPoint = EB.Dot.Integer("userCampaignStatus.challengeChapters.majordata.praypoint", data, 0);

            if (!IsInitPraypoint)
            {
                Hotfix_LT.Messenger.Raise(EventName.UpDatePraypointUI);
                IsInitPraypoint = true;
            }
        }
        
        private void CreatThemeItem(int x, int y, int Id)
        {
            var node = GetNodeByPos(x, y);
            if (node != null)
            {
                node.RoleData.Id = Id;
                var roleDataTemplate = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(node.RoleData.Id);
                if (roleDataTemplate != null)
                {
                    node.RoleData.Img = roleDataTemplate.Img.Replace("|RoleShake", string.Empty);
                    node.RoleData.Model = roleDataTemplate.Model;
                    node.RoleData.OtherModel = roleDataTemplate.OtherModel;

                    node.RoleData.ModelScale = roleDataTemplate.ModelScale;
                    node.RoleData.Rotation = roleDataTemplate.Rotation;
                    node.RoleData.Span = roleDataTemplate.Span;
                    node.RoleData.Offset = roleDataTemplate.Offset;
                    node.RoleData.Order = roleDataTemplate.Order;
                    node.RoleData.Type = roleDataTemplate.Type;
                    node.RoleData.Param = roleDataTemplate.Params;
                    node.RoleData.IsCorrelation = roleDataTemplate.IsCorrelation;
                }
            }
        }

        private void RecalcNodeRelation()
        {
            var keys = NodeDataHashDic.Keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                int hash = keys[i];
                var cell = NodeDataHashDic[hash];
                RefreshNodeNeighbour(hash, cell);
            }
        }

        private void RefreshPlayerNode(object data)
        {
            int curNodeHash = GetMajorPosHash(data);
            if (curNodeHash != -1)
            {
                LTInstanceNode node;
                if (NodeDataHashDic.TryGetValue(curNodeHash, out node))
                {
                    CurNode = node;
                }
            }
        }

        private void InitMapDataGrid(object data, bool isInit = true)
        {
            int[] ints = EB.Dot.Integers(data, ints_names, ints_defaultValue);
            int flag = ints[0];
            bool sight = false;
            if(InstanceType == LTInstanceConfig.InstanceType.MonopolyInstance)//大富翁地图全开
            {
                sight = true;
            }
            else
            {
                sight = (flag & 1) != 0;
            }

            if (!isInit && !sight)
            {
                return;
            }

            bool controllered = (flag & 2) != 0;

            int x = ints[1];
            int y = ints[2];


            if (hasSpecialBoobUpdate && SpecialBoob == (y * 100 + x))
            {
                hasSpecialBoobUpdate = false;
            }
            int terra = ints[3];
            int originTerra = ints[4];
            if (originTerra != 0) terra = originTerra;
            var terraDataTemplate = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterElement(terra);
            LTInstanceNode node = new LTInstanceNode(x, y, originTerra, sight, controllered, terraDataTemplate);
            bool hasNewData = false;
            object addData = EB.Dot.Object("data", data, null);

            //判断是否需要弹出券轴选择界面
            InitSelectWeel(x, y, addData);

            node.AddData = addData;
            if (addData != null)
            {
                int[] ad_ints = EB.Dot.Integers(addData, ad_ints_names, ad_ints_defaultValue);
                hasNewData = true;
                node.Layout = EB.Dot.String("layout", addData, string.Empty);
                node.HireId = ad_ints[0];
                node.HireCost = EB.Dot.Object("cost", addData, null);
                node.WheelCount = ad_ints[1];
                var wheelList = EB.Dot.Array("WheelData", addData, null);
                if (wheelList != null)
                {
                    List<LTShowItemData> dataList = new List<LTShowItemData>();
                    for (int i = 0; i < wheelList.Count; i++)
                    {
                        var wheelData = wheelList[i];
                        dataList.Add(new LTShowItemData(wheelData));
                    }
                    node.WheelData = dataList;
                }
            }
            var node_roleData = node.RoleData;
            node_roleData.Id = ints[5];
            var node_roleData_compaignData = node_roleData.CampaignData;

            //刷新格子上的物品信息
            if (node_roleData.Id != 0)
            {
                hasNewData = true;
                RefreshNodeRoleData(node);
            }

            #region 耗时不在此
            if (isInit)
            {
                LTInstanceNode tempN;

                int hash = GetHashCodeOfPos(x, y);

                if (node.RoleData.Id == 6)
                {
                    StartHash = hash;
                }

                if (node.RoleData.Id == 7 || node.RoleData.Id == 20 || node.RoleData.Id == 34)
                {
                    DoorHash = hash;
                    if (!node.IsSight && NodeDataHashDic.ContainsKey(DoorHash))
                    {
                        var doorNode = NodeDataHashDic[DoorHash];
                        if (doorNode.RoleData.CampaignData.Kill > node.RoleData.CampaignData.Kill)
                        {
                            node.RoleData.CampaignData.Kill = doorNode.RoleData.CampaignData.Kill;
                            node.RoleData.CampaignData.IsDoorOpen = doorNode.RoleData.CampaignData.IsDoorOpen;
                        }//服务器会发送没kill的门数据，这边得处理取前一次的数据
                    }
                }

                if (node_roleData_compaignData.Bomb > 0 && !BoobMasterList.Contains(hash))
                {
                    if (node.RoleData.Id == 33)
                    {
                        if (node.RoleData.CampaignData.Bomb > 0)
                        {
                            SpecialBoob = hash;//是超级炸弹 }
                        }
                        else
                        {
                            SpecialBoob = 0;
                        }
                    }
                    else
                    {
                        BoobMasterList.Add(hash);//是炸弹怪啊
                    }

                }

                if (!sight && !hasNewData && NodeDataHashDic.TryGetValue(hash, out tempN))
                {
                    tempN.IsControllered = node.IsControllered;
                    return;
                }

                int OldOriginTerra = -1;
                int OldSkillId = 0;
                if (NodeDataHashDic.TryGetValue(hash, out tempN))
                {
                    var tempN_ot = tempN.OriginTerra;
                    if (tempN_ot != terra) OldOriginTerra = tempN_ot;

                    var tempN_rd_skillId = tempN.RoleData.CampaignData.SkillId;
                    if (!sight && tempN_rd_skillId != 0 && node_roleData_compaignData.SkillId == 0) OldSkillId = tempN_rd_skillId;
                }

                NodeDataHashDic[hash] = node;
                if (!isFirstCreatFloor && !ReflashDataHashList.Contains(hash)) ReflashDataHashList.Add(hash);

                if (OldOriginTerra > 0) node.OriginTerra = OldOriginTerra;
                if (OldSkillId > 0) node_roleData_compaignData.SkillId = OldSkillId;
            }
            else if (node.IsSight)
            {
                int hash = GetHashCodeOfPos(node.x, node.y);
                WebNodeDataDic[hash] = node;
            }
            #endregion
        }

        private void InitSelectWeel(int x, int y, object data)
        {
            int[] array = EB.Dot.Array<int>("scrolls", data, null, delegate (object val) { return int.Parse(val.ToString()); });
            if (array != null && array.Length > 0)
            {
                scrollData = new ScrollData(x, y, array);
            }
        }
        
        private void RefreshNodeNeighbour(int hash, LTInstanceNode cell)
        {
            int hashUp = hash - 100;
            int hashDown = hash + 100;
            int hashLeft = hash - 1;
            int hashRight = hash + 1;
            LTInstanceNode up, down, left, right;
            if (NodeDataHashDic.TryGetValue(hashUp, out up))
            {
                cell.SetUp(up);
            }
            if (NodeDataHashDic.TryGetValue(hashDown, out down))
            {
                cell.SetDown(down);
            }
            if (NodeDataHashDic.TryGetValue(hashLeft, out left))
            {
                cell.SetLeft(left);
            }
            if (NodeDataHashDic.TryGetValue(hashRight, out right))
            {
                cell.SetRight(right);
            }
        }

        private void RefreshMapDataAndDisplay(bool isInit = false)
        {
            if (CurNode == null)
            {
                return;
            }
            List<LTInstanceNode> nodes = new List<LTInstanceNode>();
            //if (DoorHash!=0)
            //{
            //    LTInstanceNode node;
            //    if (NodeDataHashDic.TryGetValue(DoorHash, out node))
            //    {
            //        nodes.Add(node);
            //    }
            //}
            if (isFirstCreatFloor)
            {
                foreach (LTInstanceNode node in NodeDataHashDic.Values)
                {
                    if (node.Type == LTInstanceNode.NodeType.Floor && (node.IsSight || node.IsControllered) || node.Type == LTInstanceNode.NodeType.WALL && node.IsNearSightFloor()) nodes.Add(node);
                }
            }
            else
            {
                int extraY = HeightCount;
                int extraX = WidthCount;
                int yMax = CurNode.y + extraY;
                int xMax = CurNode.x + extraX;
                for (int y = CurNode.y - extraY; y <= yMax; y++)
                {
                    int hash_1 = y * 100;
                    for (int x = CurNode.x - extraX; x <= xMax; x++)
                    {
                        int hash = hash_1 + x;
                        if (ReflashDataHashList.Contains(hash)) ReflashDataHashList.Remove(hash);
                        LTInstanceNode node;
                        if (NodeDataHashDic.TryGetValue(hash, out node))
                        {
                            if (node.Type == LTInstanceNode.NodeType.Floor && (node.IsSight || node.IsControllered) || node.Type == LTInstanceNode.NodeType.WALL && node.IsNearSightFloor()) nodes.Add(node);
                        }
                    }
                }
                if (ReflashDataHashList.Count > 0)
                {
                    for (int i = 0; i < ReflashDataHashList.Count; ++i)
                    {
                        LTInstanceNode node;
                        if (NodeDataHashDic.TryGetValue(ReflashDataHashList[i], out node))
                        {
                            if (node.Type == LTInstanceNode.NodeType.Floor && (node.IsSight || node.IsControllered) || node.Type == LTInstanceNode.NodeType.WALL && node.IsNearSightFloor()) nodes.Add(node);
                        }
                    }
                    ReflashDataHashList.Clear();
                }
                for (int i = 0; i < BoobMasterList.Count; ++i)
                {
                    LTInstanceNode node;
                    if (NodeDataHashDic.TryGetValue(BoobMasterList[i], out node))
                    {
                        if (!nodes.Contains(node)) nodes.Add(node);
                    }
                }
            }

            //通知这批Node需要刷新
            LTInstanceMapCtrl.stMapDataUpdateAct?.Invoke(nodes, isInit);
        }

        private void RefreshNodeRoleData(LTInstanceNode node)
        {
            var node_roleData = node.RoleData;
            if (node_roleData.Id != 0)
            {
                var node_roleData_compaignData = node_roleData.CampaignData;
                var addData = node.AddData;
                var roleDataTemplate = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeChapterRole(node_roleData.Id);
                if (roleDataTemplate != null)
                {
                    if (roleDataTemplate.Img.Contains("|RoleShake"))
                    {
                        node_roleData.IsDynImg = true;
                        node_roleData.Img = roleDataTemplate.Img.Replace("|RoleShake", string.Empty); ;
                    }
                    else
                    {
                        node_roleData.IsDynImg = false;
                        node_roleData.Img = roleDataTemplate.Img;
                    }
                    node_roleData.Model = roleDataTemplate.Model;
                    node_roleData.OtherModel = roleDataTemplate.OtherModel;
                    if (roleDataTemplate.Tag.Count > 0)
                    {
                        node_roleData.IsElite = roleDataTemplate.Tag[0].StartsWith("J");
                    }
                    node_roleData.ModelScale = roleDataTemplate.ModelScale;
                    node_roleData.Rotation = roleDataTemplate.Rotation;
                    node_roleData.Span = roleDataTemplate.Span;
                    node_roleData.Offset = roleDataTemplate.Offset;
                    node_roleData.Order = roleDataTemplate.Order;
                    node.RoleData.Type = roleDataTemplate.Type;
                    node_roleData.Param = roleDataTemplate.Params;
                    node_roleData.IsCorrelation  = roleDataTemplate.IsCorrelation;
                }

                if (addData != null)
                {
                    int[] rd_ints = EB.Dot.Integers(addData, rd_ints_names, rd_ints_defaultValue);
                    node_roleData_compaignData.CampaignId = rd_ints[0];
                    node_roleData_compaignData.Star = rd_ints[1];
                    node_roleData_compaignData.Bomb = rd_ints[2];
                    node_roleData_compaignData.SkillId = rd_ints[3];
                    node_roleData_compaignData.Password = EB.Dot.String("password", addData, string.Empty);
                    node_roleData_compaignData.Layout = EB.Dot.String("layout", addData, string.Empty);
                    node_roleData_compaignData.ControlNearby = EB.Dot.Bool("controlNearby", addData, false);
                    node_roleData_compaignData.IsDoorOpen = EB.Dot.Bool("open", addData, false);
                    node_roleData_compaignData.Kill = EB.Dot.Integer("kill", addData, node_roleData_compaignData.Kill);
                    if(InstanceType == LTInstanceConfig.InstanceType.MonopolyInstance)
                    {
                        ArrayList list = EB.Dot.Array("bonus", addData, null);
                        if (list != null)
                        {
                            for (int i = 0; i < list.Count; ++i)
                            {
                                node_roleData_compaignData.Bonus.Add(new LTShowItemData(list[i]));
                            }
                        }
                    }
                }

                if (node_roleData_compaignData.CampaignId > 0)
                {
                    string temp = GetMainInstanceModel(node_roleData_compaignData.CampaignId);
                    node_roleData.Model = string.IsNullOrEmpty(temp) ? node_roleData.Model : temp;
                }
                else if (roleDataTemplate != null && roleDataTemplate.Params.Count > 0)
                {
                    string temp = GetChallengeInstanceModel(roleDataTemplate);
                    node_roleData.Model = string.IsNullOrEmpty(temp) ? node_roleData.Model : temp;
                }
            }
        }

        private void DealBeforeEventData()
        {
            for (int i = BoobMasterList.Count - 1; i >= 0; i--)
            {
                int x = BoobMasterList[i] % 100;
                int y = BoobMasterList[i] / 100;
                var node = GetNodeByPos(x, y);
                if (node.RoleData.CampaignData.Bomb > 0)
                {
                    node.RoleData.CampaignData.Bomb--;
                    if (node.RoleData.CampaignData.Bomb == 0)
                    {
                        LTInstanceEvent evt = new LTInstanceEvent(LTInstanceEvent.EVENT_TYPE_BOMB, x, y);
                        evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", int.Parse(node.RoleData.Param[3]));
                        EventList.Enqueue(evt);
                        //BoobMasterList.Remove(BoobMasterList[i]);
                    }
                }
                else if (node.RoleData.CampaignData.Bomb == 0)
                {
                    BoobMasterList.Remove(BoobMasterList[i]);
                }
            }
            DealSpcialBoobData();
        }

        private void ReWriteMapDatas(int dir)
        {
            var Node = CurNode.GetNodeByDir(dir);
            if (Node != null)
            {
                //标记杀死怪后，生成规则
                int flag = 0;
                if (DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.majordata.flag.cursed", out flag) && flag > 0)
                {
                    var theme = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeEnvById(ChallengeThemeId);
                    if (theme != null)
                    {
                        if (theme.EnvLogic != null)
                        {
                            string[] strs = theme.EnvLogic.Split(',');
                            for (int j = 0; j < strs.Length; j++)
                            {
                                if (strs[j].Contains("C"))//杀死怪后，每次移动生成物品
                                {
                                    int value = 0;
                                    string str2 = strs[j].Substring(1);
                                    string[] str2s = theme.EnvLogic.Substring(1).Split('-');
                                    int.TryParse(str2s[0], out value);
                                    if (value > 0) CreatThemeItem(CurNode.x, CurNode.y, value);
                                    DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.majordata.flag.cursed", --flag);
                                }
                            }
                        }
                    }
                }
                CurNode = Node;
                hashViewList.Clear();
                int curHash = CurNode.GetHashCode();
                if (CurNode.IsSight && CurNode.CanPass)
                {
                    var list = CurNode.GetNeighbourhood();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Type != LTInstanceNode.NodeType.WALL)
                        {
                            bool isModel = false;
                            int hash = GetHashCodeOfPos(list[i].x, list[i].y);
                            LTInstanceNode node = NodeDataHashDic[hash];
                            if (node.Open(list[i].OriginTerra, out isModel))
                            {
                                //添加翻格影响
                                if (!EventUpdateList.Contains(hash))
                                {
                                    EventUpdateList.Add(hash);
                                }
                                //是模型的话锁格子
                                if (isModel)
                                {
                                    //是否是炸弹怪
                                    if (node.RoleData.Order.Equals("BombMonster"))
                                    {
                                        if (!BoobMasterList.Contains(hash))
                                        {
                                            BoobMasterList.Add(hash);
                                        }
                                        node.RoleData.CampaignData.Bomb = int.Parse(node.RoleData.Param[2]);
                                    }
                                    var nearlist = node.GetNeighbourhood();
                                    for (int j = 0; j < nearlist.Count; j++)
                                    {
                                        if (nearlist[j].Type != LTInstanceNode.NodeType.WALL)
                                        {
                                            nearlist[j].Controller();
                                        }
                                    }
                                }
                            }
                            if (CurNode.RoleData.CampaignData.Star > 0 && list[i].IsControllered)
                            {
                                list[i].IsControllered = false;
                            }
                            if (node.IsSight && node.CanPass && !node.IsControllered)
                            {
                                List<int> nearHashs = node.GetNeighbourhoodFloorHash();
                                for (int j = 0; j < nearHashs.Count; ++j)
                                {
                                    if (nearHashs[j] != curHash && !hashViewList.Contains(nearHashs[j]))
                                    {
                                        if(node.RoleData.CampaignData.Star > 0&& NodeDataHashDic[nearHashs[j]].IsControllered)
                                        {
                                            NodeDataHashDic[nearHashs[j]].IsControllered = false;
                                        }
                                        hashViewList.Add(nearHashs[j]);
                                    }
                                }
                            }
                        }
                    }
                    for (int i = 0; i < hashViewList.Count; ++i)
                    {
                        bool isModel = false;
                        int hash = hashViewList[i];
                        LTInstanceNode node = NodeDataHashDic[hashViewList[i]];
                        if (node.Open(node.OriginTerra, out isModel))
                        {
                            //添加翻格影响
                            if (!EventUpdateList.Contains(hash))
                            {
                                EventUpdateList.Add(hash);
                            }
                            //是模型的话锁格子
                            if (isModel)
                            {
                                //是否是炸弹怪
                                if (node.RoleData.Order.Equals("BombMonster"))
                                {
                                    if (!BoobMasterList.Contains(hash))
                                    {
                                        BoobMasterList.Add(hash);
                                    }
                                    node.RoleData.CampaignData.Bomb = int.Parse(node.RoleData.Param[2]);
                                }
                                var nearlist = node.GetNeighbourhood();
                                for (int j = 0; j < nearlist.Count; j++)
                                {
                                    if (nearlist[j].Type != LTInstanceNode.NodeType.WALL)
                                    {
                                        nearlist[j].Controller();
                                    }
                                }
                            }
                        }
                    }
                }
                hashViewList.Clear();
            }

            #region 优化为部分刷新
            int yMax = CurNode.y + HeightCount;
            int xMax = CurNode.x + WidthCount;
            for (int y = CurNode.y - HeightCount; y <= yMax; y++)
            {
                int hash_1 = y * 100;
                for (int x = CurNode.x - WidthCount; x < xMax; x++)
                {
                    int hash = hash_1 + x;
                    LTInstanceNode cell;
                    if (NodeDataHashDic.TryGetValue(hash, out cell))
                    {
                        RefreshNodeNeighbour(hash, cell);
                    }
                }
            }
            #endregion

            RefreshMapDataAndDisplay();
        }

        private void ReWriteEventData(int dir)
        {
            for (int i = 0; i < EventUpdateList.Count; i++)
            {
                var theme = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeEnvById(ChallengeThemeId);
                if (theme != null)
                {
                    int x = EventUpdateList[i] % 100;
                    int y = EventUpdateList[i] / 100;
                    if (theme.EnvLogic != null)
                    {
                        string[] strs = theme.EnvLogic.Split(',');
                        for (int j = 0; j < strs.Length; j++)
                        {
                            if (strs[j].Contains("H"))//翻格回血
                            {
                                int value = 0;
                                int.TryParse(strs[j].Substring(1), out value);
                                LTInstanceEvent evt = new LTInstanceEvent(LTInstanceEvent.EVENT_TYPE_HEAL, x, y);
                                evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", value);
                                EventList.Enqueue(evt);
                            }
                            else if (strs[j].Contains("D"))//翻格扣血
                            {
                                int value = 0;
                                int.TryParse(strs[j].Substring(1), out value);
                                LTInstanceEvent evt = new LTInstanceEvent(LTInstanceEvent.EVENT_TYPE_DAMAGE, x, y);
                                evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", value);
                                EventList.Enqueue(evt);
                            }
                        }
                    }
                }
            }
            if (EventUpdateList.Count > 0) EventUpdateList.Clear();
            if (EventList.Count > 0)
            {
                RaiseEvent();
            }
        }
        
        private void InitWebNodeDicToNodeDic(bool needRecal = false)
        {
            var keys = WebNodeDataDic.Keys.ToList();
            for (int i = 0; i < keys.Count; ++i)
            {
                var key = keys[i];
                var val = WebNodeDataDic[key];
                if (SpecialBoob != 0 && SpecialBoob == val.y * 100 + val.x) continue;
                NodeDataHashDic[key] = val;
                if (needRecal)
                {
                    RefreshNodeNeighbour(key, val);
                }
            }
            WebNodeDataDic.Clear();
        }

        private void RaiseEvent()
        {
            if (EventTimer == 0)
            {
                EventTimer = ILRTimerManager.instance.AddTimer((int)(LTInstanceConfig.MODEL_MOVE_TIME * 1000f), 1, delegate
                {
                    Hotfix_LT.Messenger.Raise(EventName.OnEventsDataUpdate);
                    EventTimer = 0;
                });
            }
        }

        private void DealSpcialBoobData()
        {
            if (SpecialBoob != 0)
            {
                int x = SpecialBoob % 100;
                int y = SpecialBoob / 100;
                var node = GetNodeByPos(x, y);
                if (node.RoleData.CampaignData.Bomb > 0)
                {
                    node.RoleData.CampaignData.Bomb--;
                    if (node.RoleData.CampaignData.Bomb == 0)
                    {
                        LTInstanceEvent evt = new LTInstanceEvent(LTInstanceEvent.EVENT_TYPE_BOMB, x, y);
                        evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", int.Parse(node.RoleData.Param[1]));
                        EventList.Enqueue(evt);
                        SpecialBoob = 0;
                    }
                }
            }
        }
        #endregion

        #region  无返回方法
        public void SetBossRewardHash(int x = 0, int y = 0)
        {
            BossRewardHashX = x;
            BossRewardHashY = y;
        }

        public void InitOpenDoor(int x, int y)
        {
            var node = GetNodeByPos(x, y);
            if (node != null)
            {
                node.RoleData.CampaignData.IsDoorOpen = true;
            }
        }

        public void DealFlyScroll()
        {
            LTInstanceEvent evt = new LTInstanceEvent(LTInstanceEvent.EVENT_TYPE_GETSCROLL, 0, 0);
            EventList.Enqueue(evt);
            if (EventList.Count > 0)
            {
                RaiseEvent();
            }
        }
        #endregion
        
        #region 有返回方法
        public LTInstanceNode GetNodeByPos(int x, int y)
        {
            LTInstanceNode node = null;
            NodeDataHashDic.TryGetValue(y * 100 + x, out node);

            return node;
        }

        public bool HasCampOverEvent()
        {
            return hasCampOver;
        }

        public void SetCampOver(bool state)
        {
            hasCampOver = state;
        }

        public bool GetAlienMazeLockStage(int id, string limit)
        {
            bool isLock = false;
            //前置关卡判断
            var temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetAlienMazeById(id - 1);
            if (temp != null)
            {
                if (!GetAlienMazeFinish(temp.Id)) return true;
            }

            //挑战副本通关条件判断
            if (!string.IsNullOrEmpty(limit))
            {
                int level = int.Parse(limit);
                int maxLevel = 0;
                DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);
                if (maxLevel < level) return true;
            }

            return isLock;
        }

        public string GetAlienMazeLockStr(int id, string limit)
        {
            StringBuilder sb = new StringBuilder();
            int num = 1;
            //前置关卡判断
            var temp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetAlienMazeById(id - 1);
            if (temp != null)
            {
                sb.AppendLine(string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, GetAlienMazeFinish(temp.Id) ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal,
                    num + "."
                    + string.Format(EB.Localizer.GetString("ID_ALIEN_MAZE_LOCK_TIP1"), temp.Name)));
                ++num;
            }
            //挑战副本通关条件判断
            if (!string.IsNullOrEmpty(limit))
            {
                int level = int.Parse(limit);
                int maxLevel = 0;
                DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);
                //var tp = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(level);
                sb.Append(string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, (maxLevel >= level) ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal,
                          num + "."
                          + string.Format(EB.Localizer.GetString("ID_ALIEN_MAZE_LOCK_TIP2"), level)));
                ++num;
            }
            return sb.ToString();
        }

        public bool GetAlienMazeFinish(int id)
        {
            bool hasFinish = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userAWCampaignStatus.campaigns.{0}.finish", id), out hasFinish);
            return hasFinish;
        }

        public bool HasGetMazeReward(int id)
        {
            bool hasGet = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("userAWCampaignStatus.campaigns.{0}.get", id), out hasGet);
            return hasGet;
        }

        public int GetUidFromLowest()
        {
            int uid = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.majordata.lowest.uid", out uid);
            return uid;
        }

        public int GetCurLevel()
        {
            int curLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.level", out curLevel);
            return curLevel;
        }

        public string GetHeadFrameFromLowest()
        {
            string headFrame;
            DataLookupsCache.Instance.SearchDataByID("userCampaignStatus.challengeChapters.majordata.lowest.headFrame", out headFrame);
            return EconemyTemplateManager.Instance.GetHeadFrame(headFrame).iconId;
        }

        public string GetAvatarFromLowest()
        {
            string characterId;
            DataLookupsCache.Instance.SearchDataByID("userCampaignStatus.challengeChapters.majordata.lowest.tid", out characterId);

            int skinId;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.majordata.lowest.skin", out skinId);

            var heroInfo = CharacterTemplateManager.Instance.GetHeroInfo(characterId, skinId);

            if (heroInfo != null)
            {
                return heroInfo.icon;
            }

            return string.Empty;
        }

        public bool IsChallengeEntering()
        {
            return InstanceType == LTInstanceConfig.InstanceType.ChallengeInstance;
        }

        public bool IsMonopolyEntering()
        {
            return InstanceType == LTInstanceConfig.InstanceType.MonopolyInstance;
        }

        public bool IsHunter()
        {
            bool bo = false;
            DataLookupsCache.Instance.SearchDataByID<bool>("userCampaignStatus.challengeChapters.majordata.flag.hunter", out bo);
            return bo;
        }

        public int GetTotalPrayPoint()
        {
            int TotalPrayPoint = (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("prayPointCost");
            return TotalPrayPoint;
        }

        public bool IsBossArea()
        {
            bool isBossLevel = IsChallengeEntering() && CurLevelNum > 0 && Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostChallengeChapterById(CurLevelNum).IsBoss || IsAlienMazeEntering() && ChallengeThemeId == 110;//地狱边境为特殊的boss区域
            return isBossLevel;
        }

        public bool IsMazeHasFinish()
        {
            int kill = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.metric.enemy.kill", out kill);
            int killtotal = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.metric.enemy.total", out killtotal);
            int get = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.metric.box.get", out get);
            int gettotal = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.metric.box.total", out gettotal);
            return (kill == killtotal && get == gettotal) && (killtotal != 0 || gettotal != 0);
        }

        public bool IsAlwayShowRole(int id)
        {
            //if (id == 7 || id == 20 || id == 34)
            //{
            //    return true;
            //}
            //else
            {
                return false;
            }

        }

        public bool IsSimulateWell()
        {
            return IsChallengeEntering() && !IsChallengeMapFix;
        }

        public string GetSimulateWellName()
        {

            Data.LostChallengeStyleTemplate temp = SceneTemplateManager.Instance.GetChallengeStyleById(CurMapStyle);
            if (temp != null && temp.TopWell > 0)
            {
                List<LostChallengeChapterElement> lists = Data.SceneTemplateManager.Instance.GetRandomGroupElementList(temp.TopWell);
                if (lists != null && lists.Count > 0)
                {
                    int index = UnityEngine.Random.Range(0, lists.Count);
                    return lists[index].Img;
                }
            }
            return string.Empty;
        }

        private int GetHashCodeOfPos(int x, int y)
        {
            return y * 100 + x;
        }

        private bool IsMapDataEmpty(object data)
        {
            ArrayList mapDataList = Hotfix_LT.EBCore.Dot.Array("userCampaignStatus.challengeChapters.mapdata", data, null);
            return mapDataList == null || mapDataList.Count <= 0;
        }
        
        private bool IsAlienMazeEntering()
        {
            return InstanceType == LTInstanceConfig.InstanceType.AlienMazeInstance;
        }
        
        private string GetMainInstanceModel(int campaignId)
        {
            var mainCampaignInfo = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaignId.ToString());
            if (mainCampaignInfo != null)
            {
                var layoutInfo = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMaxWaveLayout(mainCampaignInfo.EncounterGroupId);
                if (layoutInfo != null && !string.IsNullOrEmpty(layoutInfo.Model))
                {
                    var monsterInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(int.Parse(layoutInfo.Model));
                    if (monsterInfo != null)
                    {
                        var characterInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(monsterInfo.character_id);
                        if (characterInfo != null)
                        {
                            return characterInfo.model_name;
                        }
                    }
                }
            }
            return string.Empty;
        }

        private string GetChallengeInstanceModel(Hotfix_LT.Data.LostChallengeChapterRole roleDataTemplate)
        {
            var layoutInfo = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMaxWaveLayout(roleDataTemplate.Params[0]);
            if (layoutInfo != null && !string.IsNullOrEmpty(layoutInfo.Model))
            {
                var monsterInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMonsterInfo(int.Parse(layoutInfo.Model));
                if (monsterInfo != null)
                {
                    var characterInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(monsterInfo.character_id);
                    if (characterInfo != null)
                    {
                        return characterInfo.model_name;
                    }
                }
            }
            return string.Empty;
        }
        
        private int GetMajorPosHash(object data)
        {
            var majorData = EB.Dot.Object("userCampaignStatus.challengeChapters.majordata", data, null);

            if ((InstanceType == LTInstanceConfig.InstanceType.MonopolyInstance))
            {
                var temp = EB.Dot.Object("monopoly.major", data, null);
                if (temp != null)
                {
                    var mdata = EB.Dot.Object("pos", temp, null);
                    if (mdata != null)
                    {
                        majorData = temp;
                    }
                }
            }

            if (majorData == null)
            {
                return -1;
            }
            int[] ints = EB.Dot.Integers(majorData, _GetMajorNode_ints_names, _GetMajorNode_ints_defaultValue);

            return GetHashCodeOfPos(ints[0], ints[1]);
        }

        public int GetMonopolyDiceTotleCount()
        {
            return GetMonopolyGeneralDiceCount() + GetMonopolySpecialDiceCount();
        }

        public int GetMonopolyGeneralDiceCount()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format ( "monopoly.dice.{0}.count", LTInstanceConfig.MonopolyDice1), out num);
            return num;
        }

        public int GetMonopolySpecialDiceCount()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("monopoly.dice.{0}.count", LTInstanceConfig.MonopolyDice2), out num);
            return num;
        }

        public int GetMonopolyGeneralDiceEXCount()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("monopoly.dice.{0}.buy_times", LTInstanceConfig.MonopolyDice1), out num);
            return num;
        }

        public int GetMonopolySpecialDiceEXCount()
        {
            int num = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("monopoly.dice.{0}.buy_times", LTInstanceConfig.MonopolyDice2), out num);
            return num;
        }

        public bool GetFreeDice()
        {
            bool canget = false;
            if (!DataLookupsCache.Instance.SearchDataByID<bool>("monopoly.free_dice", out canget))
            {
                return true;
            }
            return canget;
        }
        #endregion

        #region 清除副本数据
        public void ClearInstanceData()
        {
            ClearChapterData(true);
            IsInstanceShowTheme = false;
            DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.reward", null);//清空背包
            DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.majordata.flag", null);//清除副本标记
            DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.majordata.hirelist", null);//清雇佣兵数据
        }

        public void ClearChapterData(bool isAll = false)
        {
            NodeDataHashDic.Clear();
            WebNodeDataDic.Clear();
            DealDirQueue.Clear();
            RequestDirQueue.Clear();
            ReflashDataHashList.Clear();

            CurNode = null;
            isFirstCreatFloor = true;

            MainChapterId = string.Empty;
            PrayPoint = 0;
            IsInitPraypoint = false;

            EventUpdateList.Clear();
            BoobMasterList.Clear();
            SpecialBoob = 0;

            CurLevelNum = 0;
            ChallengeThemeId = 0;
            ChallengeThemeEnvStr = string.Empty;
            IsChallengeMapFix = false;

            StartHash = 0;
            DoorHash = 0;
            BossRewardHashX = 0;
            BossRewardHashY = 0;

            if (isAll)//完全退出副本才会调用到这
            {
                CurMapStyle = 0;
                InstanceType = LTInstanceConfig.InstanceType.None;
                LTInstanceMapModel.Instance.MainChapterId = string.Empty;
            }
        }

        public void ClearMonopolyData()
        {
            bool freedice = false;
            int count = 0;
            string path = "monopoly.free_dice";
            if (DataLookupsCache.Instance.SearchDataByID<bool>(path, out freedice) && !freedice)
            {
                DataLookupsCache.Instance.CacheData("monopoly.free_dice", true);
            }
            count = 0;
            path = string.Format("monopoly.dice.{0}.buy_times", LTInstanceConfig.MonopolyDice1);
            if (DataLookupsCache.Instance.SearchIntByID(path,out count)&& count>0)
            {
                DataLookupsCache.Instance.CacheData(path, 0);
            }
            count = 0;
            path = string.Format("monopoly.dice.{0}.buy_times", LTInstanceConfig.MonopolyDice2);
            if (DataLookupsCache.Instance.SearchIntByID(path, out count) && count > 0)
            {
                DataLookupsCache.Instance.CacheData(path, 0);
            }
        }

        #endregion
        
        #region 加载图集
        public bool IsLoadAtlasReady()
        {
            return InstanceRoleAtlas != null && InstanceMapStyleAtlas != null && AtlasLoadingCount == 0;
        }

        public void LoadInstanceSpriteAtlas()
        {
            AtlasLoadingCount = 0;
            LoadMapStyleSpriteAtlas();
            if (InstanceRoleAtlas == null)
            {
                AtlasLoadingCount++;
                EB.Assets.LoadAsyncAndInit<SpriteAtlas>("LTInstanceRoleAtlas", OnRoleAssetReady, GameEngine.Instance.gameObject);
            }
        }

        public void UnloadInstanceSpriteAtlas()
        {
            UnloadMapStyleSpriteAtlas();
            if (InstanceRoleAtlas != null)
            {
                GameObject.Destroy(InstanceRoleAtlas);
                InstanceRoleAtlas = null;
            }
        }

        private void LoadMapStyleSpriteAtlas()
        {
            if (CurMapStyle != 0 && isFirstCreatFloor)
            {
                if (curAtlasIndex != CurMapStyle)
                {
                    UnloadMapStyleSpriteAtlas();
                    AtlasLoadingCount++;
                    curAtlasIndex = CurMapStyle;
                    EB.Assets.LoadAsyncAndInit<SpriteAtlas>(string.Format("LTInstanceMapStyle_{0}", curAtlasIndex), OnMapStyleAssetReady, GameEngine.Instance.gameObject);
                }
            }
        }

        private void UnloadMapStyleSpriteAtlas()
        {
            if (InstanceMapStyleAtlas != null)
            {
                GameObject.Destroy(InstanceMapStyleAtlas);
                InstanceMapStyleAtlas = null;
                curAtlasIndex = 0;
            }
        }

        private void OnMapStyleAssetReady(string assetname, SpriteAtlas go, bool isSuccessed)
        {
            if (isSuccessed)
            {
                InstanceMapStyleAtlas = go;
            }
            AtlasLoadingCount--;
        }

        private void OnRoleAssetReady(string assetname, SpriteAtlas go, bool isSuccessed)
        {
            if (isSuccessed)
            {
                InstanceRoleAtlas = go;
            }
            AtlasLoadingCount--;
        }
        #endregion
        
        #region 通用api
        public int GetMaxCampaignLevel()
        {
            int maxChapterId = 101;
            Hashtable chapterData;
            if(DataLookupsCache.Instance.SearchDataByID<Hashtable>("userCampaignStatus.normalChapters", out chapterData)){
                foreach (DictionaryEntry data in chapterData)
                {
                    int chapterIntId;
                    string str = data.Key.ToString();
                    if(int.TryParse(str, out chapterIntId)){
                        maxChapterId = Mathf.Max(chapterIntId, maxChapterId);
                    }
                    else{
                        EB.Debug.LogError($"GetMaxCampaignLevel===>data.Key: {str}");
                    }
                }
            }

            int campaignLevel = maxChapterId * 100;
            Hashtable campaignList;
            string keyMaxChapterId = $"userCampaignStatus.normalChapters.{maxChapterId}.campaigns";
            if(DataLookupsCache.Instance.SearchDataByID<Hashtable>(keyMaxChapterId, out campaignList)){
                if (campaignList != null)
                {
                    foreach (DictionaryEntry campaignData in campaignList)
                    {
                        int LevelId = int.Parse(campaignData.Key.ToString());
                        if (LevelId > campaignLevel)
                        {
                            campaignLevel = LevelId;
                        }
                    }
                }
            }

            return campaignLevel;
        }

        public bool NotMainChapterId()
        {
            return string.IsNullOrEmpty(LTInstanceMapModel.Instance.MainChapterId);
        }
        
        public void SimulateFunc(int dir)
        {
            //发送模拟事件
            DealDirQueue.Enqueue(dir);
            GlobalMenuManager.CurGridMap_MajorDataUpdateFunc();
        }

        public void RequestGetChapterState(System.Action callback = null)
        {
            mChallengeInstanceApi.RequestGetChapterState(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    InitChapterData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }

        public void RequestMoveChar(int dir, System.Action<bool> callback = null)
        {
            // 离开的副本不需要再向服务器发送移动消息了
            if (InstanceType == LTInstanceConfig.InstanceType.None)
            {
                return;
            }

            if (IsFinishingInstance)
            {
                return;
            }

            LTInstanceNode targetNode = CurNode.GetNodeByDir(dir);
            if (targetNode == null)
            {
                return;
            }

            if (isRequesting) return;
            SetRequestingTimer();
            mChallengeInstanceApi.RequestMoveChar(dir, delegate (Hashtable result)
            {
                ResetRequestingTimer();
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    if (CurNode != null)
                    {
                        int oldx = CurNode.x, oldy = CurNode.y;
                        InitChapterData(result);
                        int newx = CurNode.x, newy = CurNode.y;
                        if (callback != null)
                        {
                            callback(oldx != newx || oldy != newy);
                        }
                    }

                    object obj = EB.Dot.Object("playstate", result, null);
                    if (obj != null)
                    {
                        // 进入了战斗 
                        Hotfix_LT.Messenger.Raise(EventName.ChallengeBattle, true);
                        //InitChallengeFormation();
                    }
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestMoveData(System.Action callback = null)
        {
            var dir = RequestDirQueue.Dequeue();
            mChallengeInstanceApi.RequestMoveChar(dir, delegate (Hashtable result)
            {
                //数据储存起来，等校对
                if (callback != null)
                {
                    callback();
                }
                ArrayList mapDataList = Hotfix_LT.EBCore.Dot.Array("userCampaignStatus.challengeChapters.mapdata", result, null);
                if (mapDataList == null) return;
                scrollData = null;
                for (int i = 0; i < mapDataList.Count; i++)
                {
                    InitMapDataGrid(mapDataList[i], false);
                }
                //处理券轴
                if (scrollData!=null && !FormationUtil.IsAllPartnerDied())
                {
                     GlobalMenuManager.Instance.Open("LTShowReelView");
                }
                //处理新元素事件
                ArrayList eventList = Hotfix_LT.EBCore.Dot.Array("userCampaignStatus.challengeChapters.events", result, null);
                bool hasEvent = false;
                if (eventList != null)
                {
                    for (int i = 0; i < eventList.Count; i++)
                    {
                        string type = EB.Dot.String("event", eventList[i], string.Empty);
                        if (type.Equals(LTInstanceEvent.EVENT_TYPE_GUIDE))
                        {
                            int x = EB.Dot.Integer("x", eventList[i], 0);
                            int y = EB.Dot.Integer("y", eventList[i], 0);
                            LTInstanceEvent evt = new LTInstanceEvent(type, x, y);
                            evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", eventList[i]);
                            EventList.Enqueue(evt);
                            hasEvent = true;
                        }
                        else if (type.Equals(LTInstanceEvent.EVENT_TYPE_HIDDEN))
                        {
                            int x = EB.Dot.Integer("x", eventList[i], 0);
                            int y = EB.Dot.Integer("y", eventList[i], 0);
                            LTInstanceEvent evt = new LTInstanceEvent(type, x, y);
                            evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", eventList[i]);
                            EventList.Enqueue(evt);
                            hasEvent = true;
                        }
                        else if (type.Equals(LTInstanceEvent.EVENT_TYPE_DIALOG))
                        {
                            SetWaitForDialog();
                            int x = EB.Dot.Integer("x", eventList[i], 0);
                            int y = EB.Dot.Integer("y", eventList[i], 0);
                            LTInstanceEvent evt = new LTInstanceEvent(type, x, y);
                            evt.Param = Hotfix_LT.EBCore.Dot.Find<object>("param", eventList[i]);
                            EventList.Enqueue(evt);
                            hasEvent = true;
                        }
                    }
                }
                if (hasEvent && EventList.Count > 0)
                {
                    RaiseEvent();
                }
                int hploss= EB.Dot.Integer(LTChallengeInstanceHpCtrl.curGlobalHpLoss, result, -1);
                if (hploss > 0)
                {
                    DataLookupsCache.Instance.CacheData(LTChallengeInstanceHpCtrl.curGlobalHpLoss, hploss);
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestLeaveChapter(string save, System.Action callback = null)
        {
            mChallengeInstanceApi.RequestLeaveChapter(save, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    if (callback != null)
                    {
                        callback();
                    }
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }

        private void SetWaitForDialog()
        {
            WaitForDialoging = true;
            ILRTimerManager.instance.AddTimer(500, 1, delegate { WaitForDialoging = false; });
        }

        private void SetRequestingTimer()
        {
            isRequesting = true;
            ILRTimerManager.instance.RemoveTimerSafely(ref isRequestingTimer);
            isRequestingTimer = ILRTimerManager.instance.AddTimer(10000, 1, ResetRequestingTimer);
        }

        private void ResetRequestingTimer(int timer)
        {
            isRequesting = false;
            isRequestingTimer = 0;
        }

        private void ResetRequestingTimer()
        {
            isRequesting = false;
            ILRTimerManager.instance.RemoveTimerSafely(ref isRequestingTimer);
        }
        #endregion

        #region 挑战副本api
        public void RequestChallengeEnterChapter(int levelNum, System.Action<string> callback = null, string type = null)
        {
            if (isRequesting) return;
            SetRequestingTimer();
            if (type == null)
            {
                InstanceType = LTInstanceConfig.InstanceType.ChallengeInstance;
            }
            else
            {
                InstanceType = LTInstanceConfig.InstanceType.AlienMazeInstance;
            }
            mChallengeInstanceApi.RequestEnterChapter(levelNum, delegate (object error, Hashtable result)
            {
                ResetRequestingTimer();
                ChallengeErrorHandle(error, result, delegate (bool isSucc)
                {
                    if (!isSucc)
                    {
                        callback("enter failed");
                        return;
                    }

                    if (result != null)
                    {

                        DataLookupsCache.Instance.CacheData(result);
                        InitChapterData(result);
                    }

                    if (callback != null)
                    {
                        callback(null);
                    }
                });
            }, type);
        }
        
        public void RequestChallengeResumeChapter(int levelNum, System.Action<string> callback = null, string type = null)
        {
            if (isRequesting) return;
            SetRequestingTimer();
            if (type == null)
            {
                InstanceType = LTInstanceConfig.InstanceType.ChallengeInstance;
            }
            else
            {
                InstanceType = LTInstanceConfig.InstanceType.AlienMazeInstance;
            }
            mChallengeInstanceApi.RequestResumeChapter(levelNum, delegate (object error, Hashtable result)
            {
                ResetRequestingTimer();
                ChallengeErrorHandle(error, result, delegate (bool isSucc)
                {
                    if (!isSucc)
                    {
                        callback("enter failed");
                        return;
                    }

                    if (result != null)
                    {
                        DataLookupsCache.Instance.CacheData(result);
                        InitChapterData(result);
                    }

                    if (callback != null)
                    {
                        callback(null);
                    }
                });
            }, type);
        }
        
        public void RequestChallengeFinshChapterActionDo()
        {
            if (RequestChallengeFinshChapterAction != null)
            {
                RequestChallengeFinshChapterAction();
            }
            RequestChallengeFinshChapterAction = null;
        }

        public void RequestChallengeFinshChapter(int param, int id, System.Action callback = null)
        {
            if (IsFinishingInstance)
            {
                return;
            }

            mChallengeInstanceApi.RequestFinshChapter(param, id, delegate (object error, Hashtable result)
            {
                if (!IsFinishingInstance)
                {
                    IsFinishingInstance = true;
                }

                if (error != null)
                {
                    return;
                }

                if (callback != null)
                {
                    callback();
                }

                //上报友盟，完成本层
                FusionTelemetry.PostFinishCombat(eBattleType.ChallengeCampaign.ToString() + LTInstanceMapModel.Instance.CurLevelNum);

                if (param == 0)
                {
                    Hotfix_LT.Messenger.Raise(EventName.OnQuitMapEvent);
                    if (IsChallengeEntering())
                    {
                        var temp=SceneTemplateManager.Instance.GetCheckPointChapter(LTInstanceMapModel.Instance.CurLevelNum);
                        if(temp!=null) LTChargeManager.Instance.CheckLimitedTimeGiftTrigger(Hotfix_LT.Data.LTGTriggerType.Challenge, temp.CurChapter.ToString());
                    }
                }
                else if (param == 1)
                {
                    Hotfix_LT.Messenger.Raise(EventName.OnResetMapEvent);
                }

                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    if(param == 1)//如果是进入下一层延迟处理
                    {
                        RequestChallengeFinshChapterAction = delegate
                        {
                            InitChapterData(result);
                            InitMaxLevel(result);
                        };
                    }
                    else
                    {
                        InitChapterData(result);
                        InitMaxLevel(result);
                    }
                    //上报友盟，完成本层
                    FusionTelemetry.PostStartCombat(eBattleType.ChallengeCampaign.ToString() + LTInstanceMapModel.Instance.CurLevelNum);
                }
                IsFinishingInstance = false;
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestChallengeLevelInfo(System.Action callback = null, string type = null)
        {
            mChallengeInstanceApi.RequestLevelInfo(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, type);
        }
        
        public void RequestChallengeFastCombat(bool param, System.Action callback)
        {
            mChallengeInstanceApi.RequestFastCombat(param, delegate (Hashtable result)
            {
                PlayerPrefs.SetInt("ChallegenInstanceIsFastCombat", param ? 1 : 0);
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestChallengeBuyScroll(int[] pos, int id, System.Action callback)
        {
            mChallengeInstanceApi.RequestBuyScroll(pos, id, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestChallengePickScroll(int[] pos,int id, System.Action callback)
        {
            mChallengeInstanceApi.RequestPickScroll(pos,id, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }

        public void RequestChallengeRevive(System.Action callback = null)
        {
            mChallengeInstanceApi.RequestRevive(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }

        public void RequestChallengeConfirmFastCombat(long combatId, int confirm, System.Action callback = null)
        {
            mChallengeInstanceApi.RequestConfirmFastCombat(combatId, confirm, delegate
            {
                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestChallengeGetHeroHp(ArrayList heroes, System.Action callback = null, string type = null)
        {
            mChallengeInstanceApi.RequestChallengeGetHeroHP(heroes, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, type);
        }
        
        public void RequestChallengeDeath(System.Action callback = null)
        {
            mChallengeInstanceApi.RequestChallengeDeath(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            }, IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        
        public void RequestChallengeCampaignLuckDraw(System.Action<Hashtable> callback = null)
        {
            mChallengeInstanceApi.RequestChallengeCampaignLuckDraw(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback(result);
                }
            });
        }
        
        public void RequestChallengeWipeOutLuckDraw(System.Action<Hashtable> callback = null)
        {
            mChallengeInstanceApi.RequestChallengeWipeOutLuckDraw(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void RequestGetLowestTeamViews(int level, System.Action<Hashtable> callback)
        {
            mChallengeInstanceApi.RequestGetLowestTeamViews(level, callback);
        }

        public void RequestBossReward()
        {
            mChallengeInstanceApi.RequestBossReward(new[] { BossRewardHashX, BossRewardHashY }, delegate (Hashtable result) {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    InitChapterData(result);
                }
            });
        }

        private void ChallengeErrorHandle(object error, Hashtable result, System.Action<bool> callback)
        {
            if (error == null && !IsMapDataEmpty(result))
            {
                callback(true);
                return;
            }

            callback(false);
        }

        private void OnErrorLeave(System.Action<bool> callback)
        {
            RequestLeaveChapter("chal", delegate
            {
                callback(false);
            });
        }

        private void InitMaxLevel(object data)
        {
            CurLevelNum = EB.Dot.Integer("userCampaignStatus.challengeChapters.level", data, 0);

            // 服务器只会在刚进挑战副本的时候更新maxLevel，如果在挑战副本里面最大层数改变了只能客户端这边做设置。。。。。。
            //maxLevel不再使用，没必要做了
            int maxLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.maxLevel", out maxLevel);
            if (CurLevelNum > maxLevel)
            {
                DataLookupsCache.Instance.CacheData("userCampaignStatus.challengeChapters.maxLevel", CurLevelNum);
            }
        }

        #endregion

        #region 主线副本api
        public void RequestMainEnterChapter(int chapterId, System.Action callback = null)
        {
            InstanceType = LTInstanceConfig.InstanceType.MainInstance;
            mMainInstanceApi.RequestEnterChapter(chapterId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    InitChapterData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            });
        }
        
        public void RequestMainFightCampaign(int campaignId, int campaignType = 0, bool isFast = false)
        {
            mMainInstanceApi.RequestFightCampaign(campaignId, isFast, delegate (Hashtable result)
            {
                if (result != null)
                {
                    if (isFast) DataLookupsCache.Instance.CacheData("combat.rewards", null);
                    DataLookupsCache.Instance.CacheData(result);
                }
            }, campaignType);
        }
        
        public void RequestMainBlitzCampaign(int campaignId, int times, System.Action callback)
        {
            mMainInstanceApi.RequestBlitzCampaign(campaignId, times, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    if (callback != null)
                    {
                        callback();
                    }
                }
            });
        }
        
        public void RequestMainGetStarReward(int star, string chapterId, System.Action callback)
        {
            mMainInstanceApi.RequestGetStarReward(star, chapterId, delegate (Hashtable result)//为计算数量差所以先回调 
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    if (callback != null)
                    {
                        callback();
                    }
                }
            });
        }

        public bool GetMainChapterRewardState(string id)
        {
            bool state = false;
            DataLookupsCache.Instance.SearchDataByID<bool>(string.Format ("userCampaignStatus.lotteryRewards.{0}.getlottery", id),out state);
            return state;
        }
        
        public void RequestMainChapterReward(string chapterId, System.Action callback)
        {
            mMainInstanceApi.RequestMainChapterReward(chapterId, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    if (callback != null)
                    {
                        callback();
                    }
                }
            });
        }
        
        public void RequestMainPray(int index, System.Action callback)
        {
            mMainInstanceApi.RequestPray(index, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    //InitChapterData(result);
                }

                if (callback != null)
                {
                    callback();
                }
            });
        }
        #endregion

        #region 异界迷宫api
        public void RequestAlienMazeEnter(int id, System.Action<string> callback = null)
        {
            int curLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userAWCampaignStatus.curLevel", out curLevel);
            if (curLevel == id)//是否暂离后，回到副本
            {
                RequestChallengeResumeChapter(id, callback, LTInstanceConfig.AlienMazeTypeStr);
            }
            else
            {
                LTInstanceMapModel.Instance.IsInstanceShowTheme = true;
                RequestChallengeEnterChapter(id, callback, LTInstanceConfig.AlienMazeTypeStr);
            }
        }

        public void RequestGetAlienMazeReward(int id, System.Action callback = null)
        {
            if (isRequesting) return;
            SetRequestingTimer();
            mAlienMazeInstanceApi.RequestGetAlienMazeReward(id, delegate (Hashtable result)
             {
                 ResetRequestingTimer();
                 if (result != null)
                 {
                     DataLookupsCache.Instance.CacheData(result);
                 }

                 if (callback != null)
                 {
                     callback();
                 }
             });
        }
        #endregion

        #region 大富翁api
        public void RequestEnterMonopoly(System.Action<bool> callback)
        {
            InstanceType = LTInstanceConfig.InstanceType.MonopolyInstance;
            mMonopolyInstanceApi.RequestEnterMonopoly(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                    InitChapterData(result);
                }

                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        public void RequestFinishMonopoly(System.Action<Hashtable> callback)
        {
            mMonopolyInstanceApi.RequestFinish(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void RequestDice(string diceType, int index, bool isBuy, System.Action<Hashtable> callback)
        {
            mMonopolyInstanceApi.RequestDice( diceType,  index,  isBuy, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }

                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void RequestMove(Hashtable pos, System.Action<Hashtable> callback)
        {
            mMonopolyInstanceApi.RequestMove(pos,delegate(Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void RequestFreeDice(System.Action<Hashtable> callback)
        {
            mMonopolyInstanceApi.RequestFreeDice( delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result);
                }
            });
        }
        #endregion

        #region A*寻路相关

        public List<Vector2> GetRoad(LTInstanceNode start, LTInstanceNode end, bool needStop,ref bool bossRewardStop)
        {
            if (start==null||start.IsSameNodePos(end))
            {
                return null;
            }

            List<LTInstanceNode> openList = new List<LTInstanceNode>();
            List<LTInstanceNode> closeList = new List<LTInstanceNode>();
            openList.Add(start);

            while (openList.Count > 0)
            {
                LTInstanceNode curNode = openList[0];

                for (int i = 0; i < openList.Count; i++)
                {
                    if (openList[i].fCost <= curNode.fCost && openList[i].hCost < curNode.hCost)
                    {
                        curNode = openList[i];
                    }
                }

                openList.Remove(curNode);
                closeList.Add(curNode);

                if (LTInstanceMapModel.Instance.BossRewardHashX > 0 || LTInstanceMapModel.Instance.BossRewardHashY > 0)
                {
                    //boss宝箱领取特殊处理方式
                    int temp = Mathf.Abs(curNode.x - LTInstanceMapModel.Instance.BossRewardHashX);
                    if (temp <= 1)
                    {
                        temp = Mathf.Abs(curNode.y - LTInstanceMapModel.Instance.BossRewardHashY);
                        if (temp <= 1)
                        {
                            bossRewardStop = true;
                            return GeneratePath(start, curNode, true);
                        }
                    }
                }

                if (curNode.IsSameNodePos(end))
                {
                    return GeneratePath(start, curNode);
                }

                var neighbourhoodNodeList = curNode.GetNeighbourhood();

                for (var i = 0; i < neighbourhoodNodeList.Count; i++)
                {
                    var neighbourhoodNode = neighbourhoodNodeList[i];
                    if (neighbourhoodNode.Type == LTInstanceNode.NodeType.WALL || closeList.Contains(neighbourhoodNode))//避开墙和closelist中的节点
                    {
                        continue;
                    }

                    if (!neighbourhoodNode.IsSight)//被怪物控制的节点和未翻开的节点
                    {
                        continue;
                    }

                    if (!neighbourhoodNode.CanPass)//避开不能通行的格子
                    {
                        continue;
                    }

                    if (!needStop)
                    {
                        if (!neighbourhoodNode.IsSameNodePos(end) && !IsCanStand(neighbourhoodNode))
                        {
                            continue;
                        }
                    }

                    if (!neighbourhoodNode.IsSameNodePos(end) && !IsRoleEmpty(neighbourhoodNode)&&InstanceType !=LTInstanceConfig.InstanceType.MonopolyInstance)//避开一切非空节点
                    {
                        continue;
                    }

                    int newCost = curNode.gCost + GetDistance(curNode, neighbourhoodNode);

                    if (newCost < neighbourhoodNode.gCost || !openList.Contains(neighbourhoodNode))
                    {
                        neighbourhoodNode.gCost = newCost;
                        neighbourhoodNode.hCost = GetDistance(neighbourhoodNode, end);
                        neighbourhoodNode.Parent = curNode;

                        if (!openList.Contains(neighbourhoodNode))
                        {
                            openList.Add(neighbourhoodNode);
                        }
                    }
                }
            }
            return null;
        }

        private List<Vector2> GeneratePath(LTInstanceNode start, LTInstanceNode end, bool needStop = false)
        {
            List<Vector2> path = new List<Vector2>();
            try
            {
                if (end != null && start != null)
                {
                    LTInstanceNode temp = end;

                    while (!temp.IsSameNodePos(start))
                    {
                        if (temp != null)
                        {
                            if (LTInstanceMapModel.Instance.IsCanStand(temp))
                            {
                                path.Add(new Vector2(temp.x, temp.y));
                                if (path.Count > 300)
                                {
                                    EB.Debug.LogWarning(string.Format("path is err!start=({0},{1});end=({2},{3})", start.x, start.y, end.x, end.y));
                                    path.Clear();
                                    break;
                                }
                            }
                            if (temp.Parent != null)
                            {
                                temp = temp.Parent;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    path.Reverse();

                    //把找到终点后的多余操作移除
                    if (path.Count > 0)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            if (path[i].x == end.x && path[i].y == end.y && (i + 1) < path.Count)
                            {
                                path.RemoveRange(i + 1, path.Count - (i + 1));
                                break;
                            }
                        }
                        if (needStop && path.Count > 0)
                        {
                            path.RemoveAt(path.Count - 1);
                        }
                    }
                }
            }
            catch (System.ApplicationException ex)
            {
                EB.Debug.LogWarning("GeneratePath error = {0}", ex);
                return path;
            }

            string pathString = string.Empty;
            for (int i = 0; i < path.Count; i++)
            {
                pathString += string.Format("({0},{1});", path[i].x, path[i].y);
            }
            //EB.Debug.LogError("Path:"+ pathString); //显示路径

            return path;
        }

        private int GetDistance(LTInstanceNode start, LTInstanceNode end)
        {
            int distance = 0;
            distance = Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y);
            return distance;
        }

        private bool IsRoleEmpty(LTInstanceNode node)
        {
            if (string.IsNullOrEmpty(node.RoleData.Img)||node.RoleData.Type == 1)
            {
                return true;
            }

            if (node.RoleData.Img == "Copy_Icon_Gonggaopai" || node.RoleData.Img == "Copy_Icon_Shangdian" || ((node.RoleData.Img == "Copy_Icon_Guanmen" || node.RoleData.Img == "Copy_Icon_Jiguan") && node.RoleData.CampaignData.IsDoorOpen))
            {
                return true;
            }

            for (int i = 0; i < LTInstanceConfig.EmptyRoleList.Count; i++)
            {
                if (LTInstanceConfig.EmptyRoleList[i] == node.RoleData.Img)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsCanStand(LTInstanceNode node)
        {
            if (!string.IsNullOrEmpty(node.Layout) || (node.RoleData.CampaignData.CampaignId > 0 && node.RoleData.CampaignData.Star <= 0))//避开怪物
            {
                return false;
            }

            if (node.RoleData.Order == "Hero" || node.RoleData.Order == "Hire")
            {
                return false;
            }

            if (!node.CanPass)
            {
                return false;
            }

            if (node.RoleData.Img == "Copy_Icon_Guanmen" && !node.RoleData.CampaignData.IsDoorOpen)//未开启的门
            {
                return false;
            }

            for (int i = 0; i < LTInstanceConfig.CanNotStandList.Count; i++)
            {
                if (LTInstanceConfig.CanNotStandList[i] == node.RoleData.Img)
                {
                    return false;
                }
            }

            if (node.RoleData.Img == "Copy_Icon_Shendenghun" && PrayPoint >= GetTotalPrayPoint())
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 红点相关
        public bool GetChallengeRP()//挑战副本判断
        {
            FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10065);
            if (m_FuncTpl.IsConditionOK())
            {
                if (IsChallengeRewardVaild())
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.challenge, 1);
                    return true;
                }

                int i = 0;
                DataLookupsCache.Instance.SearchDataByID<int>("res.chall-camp-point.v", out i);
                if (i >= 9)
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.challenge, 1);
                    return true;
                }

                int saveTimeText;
                DataLookupsCache.Instance.SearchDataByID<int>("userCampaignStatus.challengeChapters.saveTime", out saveTimeText);
                bool isSave = saveTimeText > 0;
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.challenge, isSave ? 1 : 0);
                return isSave;
            }
            else
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.challenge, 0);
                return false;
            }
        }

        public bool GetBoxRewardRP()//章节宝箱未领
        {
            bool boxReward = false;
            Hashtable campaignList = Johny.HashtablePool.Claim();
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(string.Format("userCampaignStatus.normalChapters"), out campaignList);
            if (campaignList != null)
            {
                foreach (DictionaryEntry campaignData in campaignList)
                {
                    Dictionary<int, List<LTShowItemData>> boxlist = SceneTemplateManager.Instance.GetLostMainChatpterTplById(campaignData.Key.ToString()).RewardDataDic;
                    int starNum = LTInstanceUtil.GetChapterCurStarNum(campaignData.Key.ToString());
                    foreach (int i in boxlist.Keys)
                    {
                        if (starNum >= i)
                        {
                            bool hasGetBox = EB.Dot.Bool(string.Format("box.{0}", i), campaignData.Value, false);
                            boxReward = !hasGetBox;
                            if (boxReward) break;
                        }
                    }
                    if (boxReward) break;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.mainbox, boxReward ? 1 : 0);
            return boxReward;
        }

        public bool GetAlienMazeRP()
        {
            FuncTemplate m_FuncTpl =FuncTemplateManager.Instance.GetFunc(10084);
            if(!m_FuncTpl.IsConditionOK()){
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.alienmaze ,0);
                return false;
            }
            
            if (InitHasActionMaze())
            {
                LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.alienmaze, 1);
                return true;
            }
            var temp = SceneTemplateManager.Instance.GetAllAlienMazeList();
            for (int i = 0; i < temp.Count; ++i)
            {
                if (LTInstanceMapModel.Instance.GetAlienMazeFinish(temp[i].Id) && !LTInstanceMapModel.Instance.HasGetMazeReward(temp[i].Id))
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.alienmaze, 1);
                    return true;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.alienmaze, 0);
            return false;
        }

        public void SetHasMazeState()
        {
            HasMaze = false;
        }

        public bool GetMainChapterRewardRP()
        {
            var list = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainChapterBoxRewards();
            for (int i = 0; i < list.Count; ++i)
            {
                if (LTInstanceUtil.IsChapterComplete(list[i].ForwardChapterId) && !LTInstanceMapModel.Instance.GetMainChapterRewardState(list[i].Id))
                {
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.mainchapterreward,1);
                    return true;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.mainchapterreward, 0);
            return false;
        }
        
        private bool IsChallengeRewardVaild()
        {
            List<LostChallengeChapter> list = SceneTemplateManager.Instance.GetLostChallengeCheckPointChapterList();
            for (int i = 0; i < list.Count; i++)
            {
                int showLevel = list[i].Level / 5 + 1;
                int taskId = showLevel + 7000;
                string state = string.Empty;
                DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.state", taskId), out state);
                if (state == "finished")
                {
                    return true;
                }
            }
            return false;
        }

        public void ResetAlienMazeInit()
        {
            HasInit = false;
        }

        private bool InitHasActionMaze()
        {
            if (!HasInit)
            {
                HasInit = true;
                HasMaze = false;
                var temp = SceneTemplateManager.Instance.GetAllAlienMazeList();
                for (int i = 0; i < temp.Count; ++i)
                {
                    if (LTInstanceMapModel.Instance.GetAlienMazeFinish(temp[i].Id) && !LTInstanceMapModel.Instance.HasGetMazeReward(temp[i].Id)) return true;
                    bool isLock = LTInstanceMapModel.Instance.GetAlienMazeLockStage(temp[i].Id, temp[i].Limit);
                    bool isPass = LTInstanceMapModel.Instance.GetAlienMazeFinish(temp[i].Id);
                    if (!isLock && !isPass)
                    {
                        HasMaze = true;
                        break;
                    }
                }
            }
            return HasMaze;
        }
        #endregion

        #region 主工程调用的静态方法
        public static bool IsInstanceMapFromILR()
        {
            if (Instance != null)
            {
                return Instance.InstanceType != LTInstanceConfig.InstanceType.None && !(LTCombatEventReceiver.Instance != null && LTCombatEventReceiver.Instance.Ready);
            }

            return false;
        }

        public static void SetQuickBattleOpen()
        {
            instance.OpenQuickBattle = true;
        }

        public static void SetQuickBattleClose()
        {
            instance.OpenQuickBattle = false;
        }

        public static void SetDebugBattleOpen()
        {
            instance.mChallengeInstanceApi.RequestQuickBattle(true,null, instance.IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }

        public static void SetDebugBattleClose()
        {
            instance.mChallengeInstanceApi.RequestQuickBattle(false, null, instance.IsAlienMazeEntering() ? LTInstanceConfig.AlienMazeTypeStr : null);
        }
        #endregion

    }
}