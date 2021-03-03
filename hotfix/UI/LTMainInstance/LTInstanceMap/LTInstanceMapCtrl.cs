using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceSmallMapCtrl : DynamicMonoHotfix
    {
        private Dictionary<int, LTInstanceRowCtrl> rowCtrlDic;
        public GameObject RowObj;
        public GameObject RowObjContainer;
        
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            RowObj = t.FindEx("TemplatePrefab/LTInstanceMapRowObj").gameObject;
            RowObjContainer = t.FindEx("Cull/RowContainer").gameObject;
            rowCtrlDic = new Dictionary<int, LTInstanceRowCtrl>();
        }

        public void UpdateMap()
        {
            InitMap();
            InitPos();
        }

        public void ReInit()
        {
            rowCtrlDic.Clear();
            for (int i = RowObjContainer.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(RowObjContainer.transform.GetChild(i).gameObject);
            }
        }

        private void InitMap()
        {
            Dictionary<int, LTInstanceNode> dataDic = LTInstanceMapModel.Instance.NodeDataHashDic;
            if (dataDic == null)
            {
                return;
            }
            List<LTInstanceNode> dataList = new List<LTInstanceNode>();

            int HeightCount = 4;
            int WidthCount = 5;
            int yMax = LTInstanceMapModel.Instance.CurNode.y + HeightCount;
            int xMax = LTInstanceMapModel.Instance.CurNode.x + WidthCount;
            for (int y = LTInstanceMapModel.Instance.CurNode.y - HeightCount; y <= yMax; y++)
            {
                int hash_1 = y * 100;
                for (int x = LTInstanceMapModel.Instance.CurNode.x - WidthCount; x < xMax; x++)
                {
                    int hash = hash_1 + x;
                    LTInstanceNode cell;
                    if (dataDic.TryGetValue(hash, out cell))
                    {
                        if (!cell.CanPass) continue;
                        dataList.Add(cell);
                    }
                }
            }
            
            if (dataList == null || dataList.Count < 0)
            {
                return;
            }

            for (var i = 0; i < dataList.Count; ++i)
            {
                var node = dataList[i];
                LTInstanceRowCtrl row = null;
                if (rowCtrlDic.TryGetValue(node.y, out row))
                {
                    LTInstanceNodeTemp cell = null;
                    if (row.itemObjDic.TryGetValue(node.x, out cell))
                    {
                        cell.UpdateData(node);
                    }
                    else
                    {
                        row.CreateNodeFromCache(node);
                    }
                }
                else
                {
                    CreateRow(node);
                }
            }
        }

        private void InitPos()
        {
            if (LTInstanceMapModel.Instance.CurNode != null)
            {
                Vector3 endPos = new Vector3((LTInstanceMapModel.Instance.CurNode.x + LTInstanceMapModel.Instance.CurNode.y) * LTInstanceConfig.SMALL_MAP_SCALE_X, (LTInstanceMapModel.Instance.CurNode.x - LTInstanceMapModel.Instance.CurNode.y) * LTInstanceConfig.SMALL_MAP_SCALE_Y, 0) * (-1);// new Vector3(mCurNode.x * LTInstanceConfig.SMALL_MAP_SCALE, -mCurNode.y * LTInstanceConfig.SMALL_MAP_SCALE, 0);
                RowObjContainer.transform.localPosition = endPos;
            }
        }

        private void CreateRow(LTInstanceNode node)
        {
            GameObject row = GameObject.Instantiate(RowObj);
            row.CustomSetActive(true);
            row.transform.SetParent(RowObjContainer.transform);
            row.transform.localPosition = new Vector3(node.y * LTInstanceConfig.SMALL_MAP_SCALE_X, -node.y * LTInstanceConfig.SMALL_MAP_SCALE_Y);
            row.transform.localScale = Vector3.one;
            row.name = node.y.ToString();
            LTInstanceRowCtrl ctrl = row.GetMonoILRComponent<LTInstanceRowCtrl>();
            ctrl.CreateNodeFromCache(node);
            rowCtrlDic.Add(node.y, ctrl);
        }
    }

    /// <summary>
    /// 负责更新地形的生成和移动以及角色状态
    /// </summary>
    public class LTInstanceMapCtrl : DynamicMonoHotfix, IHotfixUpdate
    {
        private enum MoveParam
        {
            None,//无
            Direct,//直接
            Simulate,//模拟
            Fabricate,//虚构
        }

        public static System.Action<List<LTInstanceNode>, bool> stMapDataUpdateAct = null;
        public static System.Action EnterCallback = null;
        public static System.Action FloorDown = null;

        public GameObject HpObj;
        public GameObject EdgeObj;
        public GameObject RowObj;
        public GameObject RowObjContainer;
        public GameObject PlayerObj;
        public Instance.Instance3DLobby Lobby = null;
        public Instance.LTBoomItemTemp BoomObj;

        public bool isPlayerShow = true;
        public System.Action<bool> OnRealEnd;
        public LTInstanceNode curNode;
        private LTInstanceNode lastNode;

        private Dictionary<int, LTInstanceRowCtrl> rowCtrlDic;
        private List<GameObject> mRows = new List<GameObject>();
        private LTInstanceNode mRealEndNode = null;
        private LTInstanceSmallMapCtrl SmallMapCtrl;

        private List<Vector2> moveActionList;
        
        private bool WaitOptimize = true;
        private bool isNotRequest = true;
        private bool setRowObjContainerPos = false;
        private bool isWaitForUpdataMap = false;
        private bool setSpecialBoob = false;
        private bool hasSetStartPos = false;
        private bool bossRewardStop = false;
        private Vector2 StartPos;
        private Coroutine MoveCoroutine = null;
        private Vector2 SelectDisPos = new Vector2(10000, 10000);
        private MoveParam moveType = MoveParam.None;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            EdgeObj = t.FindEx("Edge").gameObject;
            
            Transform temp = t.Find("Edge/PlayerPanel/Panel/Hp");
            if (temp != null) HpObj = temp.gameObject;
            SmallMapCtrl = t.GetMonoILRComponent<LTInstanceSmallMapCtrl>("Edge/TopRight/MapPanel",false);

            UIButton mapBtn = t.Find("Edge/TopRight/MapPanel/Map").GetComponent<UIButton>();
            mapBtn.onClick.Add(new EventDelegate(OnMapViewBtnClick));

            ConsecutiveClickCoolTrigger touchBtn = t.Find("Touch").GetComponent<ConsecutiveClickCoolTrigger>();
            touchBtn.clickEvent.Add(new EventDelegate(OnTouchMapBtnClick));

            EdgeObj.CustomSetActive(false);
            rowCtrlDic = new Dictionary<int, LTInstanceRowCtrl>();

            stMapDataUpdateAct = (nodes, isInit) => { MapDataUpdateFunc_EnqueueNode(nodes, isInit); };
        }

        public override void OnEnable()
        {
            RegisterMonoUpdater();
        }

        public void Update()
        {
            if (WaitOptimize)
            {
                if (IsPlayerReady())
                {
                    RowObj = Instance.LTInstanceOptimizeManager.Instance.RowObj;
                    RowObjContainer = Instance.LTInstanceOptimizeManager.Instance.RowObjContainer;
                    PlayerObj = Instance.LTInstanceOptimizeManager.Instance.PlayerObj;
                    Lobby = Instance.LTInstanceOptimizeManager.Instance.Lobby;
                    WaitOptimize = false;
                    Update_CheckMapDataUpdate(true);
                }
                return;
            }

            if (isNotRequest && LTInstanceMapModel.Instance.RequestDirQueue != null && LTInstanceMapModel.Instance.RequestDirQueue.Count > 0)
            {
                isNotRequest = false;
                LTInstanceMapModel.Instance.RequestMoveData(delegate { isNotRequest = true;});
            }

            Update_CheckMapDataUpdate();
        }

        public override void OnDisable()
        {
            ErasureMonoUpdater();
        }
        
        public override void OnDestroy()
        {
            stMapDataUpdateAct = null;
            EnterCallback = null;
            FloorDown = null;
            rowCtrlDic.Clear();
            for(int i = 0; i < mRows.Count; i++)
            {
                GameObject.Destroy(mRows[i]);
            }
            mRows.Clear();
        }

        public void OnFloorClick(LTInstanceNode NodeData, Transform Target, bool needStop = false)
        {
            if (LTInstanceMapModel.Instance.moveResultList.Count > 0 || LTInstanceMapModel.Instance.DealDirQueue.Count > 0)
            {
                return;
            }

            if (NodeData == null || isWaitForUpdataMap)
            {
                return;
            }

            if (NodeData == curNode)
            {
                if (OnRealEnd != null)
                {
                    System.Action<bool> tmp = OnRealEnd;
                    OnRealEnd = null;
                    tmp(true);
                }
                return;
            }

            if (NodeData.RoleData.Img == "Copy_Icon_Guanmen" && !NodeData.RoleData.CampaignData.IsDoorOpen)
            {
                OnRealEnd = null;
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTInstanceMapCtrl_9085"));
                return;
            }

            mRealEndNode = NodeData;
            bossRewardStop = false;
            moveActionList = LTInstanceMapModel.Instance.GetRoad(LTInstanceMapModel.Instance.CurNode, mRealEndNode, needStop, ref bossRewardStop);
            if (moveActionList != null)
            {
                Instance.LTInstanceOptimizeManager.Instance.ShowSelectObj(true, Target);
            }
            else
            {
                Instance.LTInstanceOptimizeManager.Instance.ShowSelectObj(false, Target);
                return;
            }

            if (bossRewardStop)
            {
                OnRealEnd = new System.Action<bool>(delegate (bool isPath)
                {
                    LTInstanceMapModel.Instance.RequestBossReward();
                });
            }

            if (MoveCoroutine == null)
            {
                if (!HasNext())//针对在玩家旁边，不需要寻路的格子，因为不调用Move协程，得直接结束调用
                {
                    RealEndHandler(LTInstanceMapModel.Instance.CurNode, false);
                }
                moveType = MoveParam.Simulate;
                AutoMoveMap();
            }
        }

        public void OnDestinationMove(LTInstanceNode cur, LTInstanceNode target , int num)
        {
            bossRewardStop = false;
            moveActionList = LTInstanceMapModel.Instance.GetRoad(cur, target, false, ref bossRewardStop);
            if (moveActionList == null) return;
            if(num< moveActionList.Count) moveActionList.RemoveRange(num, moveActionList.Count- num);
            moveType = MoveParam.Fabricate;
            AutoMoveMap();
        }

        public void InitMap()
        {
            if (LTInstanceMapModel.Instance.NodeDataHashDic == null)
            {
                return;
            }
        }

        public void OpenCloud()
        {
            if (!EdgeObj.activeInHierarchy)
            {
                EB.Debug.Log("<======OpenCloud");
                EnterCallback?.Invoke();
                UIStack.Instance.HideLoadingScreenImmediately(false, true);
                EdgeObj.CustomSetActive(true);
            }
        }

        public void ReInit()
        {
            if (SmallMapCtrl != null) SmallMapCtrl.ReInit();
            foreach (var pair in rowCtrlDic)
            {
                GameObject.Destroy(pair.Value.mDMono.gameObject);
            }
            rowCtrlDic.Clear();
            hasSetStartPos = false;
            PlayerObj.transform.localPosition = Vector2.zero;
            curNode = lastNode = null;
            Instance.LTInstanceOptimizeManager.Instance.HideSelectObj();

            EdgeObj.CustomSetActive(false);
        }

        public void Release()
        {
            StopAllCoroutines();
        }
    
        public void SetBGTexture(string name)
        {
            if (Instance.LTInstanceOptimizeManager.Instance == null)
            {
                Instance.LTInstanceOptimizeManager.PreBGName = name;
            }
            else
            {
                Instance.LTInstanceOptimizeManager.Instance.SetBGTexture(name);
            }
        }

        public void SetPlayerState(bool state)
        {
            HpObj.CustomSetActive(state);
            isPlayerShow = state;
        }

        #region 分帧刷新
        private bool _CheckMapDataUpdate_Dirty = false;
        private const int _CheckMapDataUpdate_MaxOnce = 1;
        private const int _CheckMapDataUpdate_MaxMore = 15;
        private Queue<LTInstanceNode> _CheckMapDataUpdate_Queue=new Queue<LTInstanceNode>();
        private int _DelayUpdateNum = 0;

        private void Update_CheckMapDataUpdate(bool isInit = false)
        {
            if (WaitOptimize||!IsPlayerReady())
            {
                return;
            }

            if(!_CheckMapDataUpdate_Dirty){
                return;
            }
            if (_DelayUpdateNum > 0)
            {
                _DelayUpdateNum--;
                return;
            }
            int counterOnce = 0;
            if (PlayerObj.transform.localPosition.Equals(Vector3.zero)&& LTInstanceMapModel.Instance.CurNode!=null)
            {
                PlayerObj.transform.localPosition = new Vector3(PlayerObj.transform.localPosition.x, PlayerObj.transform.localPosition.y, -LTInstanceMapModel.Instance.CurNode.y * LTInstanceConfig.MAP_YZ + LTInstanceMapModel.Instance.CurNode.x * LTInstanceConfig.MAP_XZ);
            }
            //特殊初始化处理
            if(setRowObjContainerPos && LTInstanceMapModel.Instance.CurNode != null)
            {
                var end = LTInstanceMapModel.Instance.CurNode;
                RowObjContainer.transform.localPosition = new Vector3(-(end.x + end.y) * LTInstanceConfig.MAP_X, (end.y - end.x) * LTInstanceConfig.MAP_Y, 0);
                setRowObjContainerPos = false;
            }
            if (setSpecialBoob)
            {
                SetSpecialBoob();
                setSpecialBoob = false;
            }
            //

            while(_CheckMapDataUpdate_Queue.Count > 0)
            {
                var node = _CheckMapDataUpdate_Queue.Dequeue();
                //_CheckMapDataUpdate_Queue.RemoveAt(0);
                LTInstanceRowCtrl row;
                if (rowCtrlDic.TryGetValue(node.y, out row))
                {
                    row.mDMono.gameObject.CustomSetActive(true);
                    LTInstanceNodeTemp cell;
                    if (row.itemObjDic.TryGetValue(node.x, out cell))
                    {
                        if ((node.Type == LTInstanceNode.NodeType.WALL && !(cell is Instance.LTInstanceWallTemp)) || (node.Type == LTInstanceNode.NodeType.Floor && !(cell is Instance.LTInstanceFloorTemp)))//密道或格子类型发生反转的时候处理
                        {
                            GameObject.Destroy(cell.mDMono.gameObject);
                            row.itemObjDic.Remove(cell.Num);
                            row.CreateNodeFromCache(node);
                        }
                        else
                        {
                            if (!isInit && node.Type == LTInstanceNode.NodeType.WALL) continue;
                            cell.mDMono.gameObject.CustomSetActive(true);
                            cell.UpdateData(node);
                            continue;
                        }
                    }
                    else
                    {
                        row.CreateNodeFromCache(node);
                    }
                }
                else
                {
                    CreateRowFromCache(node);
                }

                counterOnce++;
                if (LTInstanceMapModel.Instance.isFirstCreatFloor)
                {
                    if (counterOnce % _CheckMapDataUpdate_MaxMore == 0)
                    {
                        LoadingLogic.AddCustomProgress(1);
                        break;
                    }
                }
                else if(counterOnce % _CheckMapDataUpdate_MaxOnce == 0)
                {
                    _DelayUpdateNum = 1;
                    break;
                }
            }


            if (_CheckMapDataUpdate_Queue.Count == 0)
            {
                if (LTInstanceMapModel.Instance.isFirstCreatFloor) LTInstanceMapModel.Instance.isFirstCreatFloor = false;
                OpenCloud();
                _CheckMapDataUpdate_Dirty = false;
            }
        }

        public void MapDataUpdateFunc_EnqueueNode(List<LTInstanceNode> nodes, bool isInit)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                _CheckMapDataUpdate_Queue.Enqueue(nodes[i]);
            }
            _CheckMapDataUpdate_Dirty = true;
            RestSmallMap();
            if (isInit){
                Update_CheckMapDataUpdate(true);
            }
        }

        public void RestSmallMap()
        {
            if (SmallMapCtrl != null) SmallMapCtrl.UpdateMap();
        }

        public void MapDataUpdateFunc_EnqueueNode(LTInstanceNode node, bool isInit)
        {
            _CheckMapDataUpdate_Queue.Enqueue(node);
            _CheckMapDataUpdate_Dirty = true;
            if (isInit)
            {
                Update_CheckMapDataUpdate(true);
            }
        }
        #endregion

        public bool IsPlayerReady()
        {
            return 
                Instance.LTInstanceOptimizeManager.Instance!=null && 
                Instance.LTInstanceOptimizeManager.Instance.isPlayerReady&&
                LTInstanceMapModel.Instance.IsLoadAtlasReady();
        }

        public bool HasNext()
        {
            bool hasNext = moveActionList != null && moveActionList.Count > 0;
            return hasNext;
        }
 
        public void InstanceWaitUpdataMap(System.Action callback = null, bool isCleanMove = true)
        {
            StartCoroutine(WaitUpdataMap(callback, isCleanMove));
        }
        private IEnumerator WaitUpdataMap(System.Action callback = null, bool isCleanMove = true)
        {
            LoadingSpinner.Show();
            isWaitForUpdataMap = true;
            if (isCleanMove && moveActionList != null)
            {
                moveActionList.Clear();
            }
            bool iswait = false;
            while (LTInstanceMapModel.Instance.RequestDirQueue.Count > 0 || LTInstanceMapModel.Instance.DealDirQueue.Count > 0)
            {
                iswait = true;
                yield return null;
            }
            while (!isNotRequest)
            {
                iswait = true;
                yield return null;
            }

            if (iswait)//如果等待过就多等下
            {
                yield return null;
            }

            isWaitForUpdataMap = false;
            if (callback != null)
            {
                callback();
            }
            LoadingSpinner.Hide();
            yield break;
        }
        
        public void MajorDataUpdateFunc()
        {
            if (LTInstanceMapModel.Instance.moveResultList.Count > 0 && MoveCoroutine == null)//改成从服务器返回的移动队列里更新
            {
                var chapterData = LTInstanceMapModel.Instance.moveResultList.Dequeue();
                LTInstanceMapModel.Instance.ApplyChapterData(chapterData,delegate {
                    SetNodeData(LTInstanceMapModel.Instance.CurNode);
                });
                if (!TryMove(lastNode, curNode)) MajorDataUpdateFunc();
            }

            if (LTInstanceMapModel.Instance.DealDirQueue.Count > 0 && MoveCoroutine == null)
            {
                LTInstanceMapModel.Instance.ApplySimulData();
                SetNodeData(LTInstanceMapModel.Instance.CurNode);
                TryMove(lastNode, curNode);
            }

            if (Instance.LTInstanceOptimizeManager.Instance == null|| PlayerObj==null)
            {
                setSpecialBoob = true;
            }
            else
            {
                SetSpecialBoob();
            }
        }
        private void SetNodeData(LTInstanceNode cNode)
        {
            lastNode = curNode;
            curNode = cNode;
        }

        public void ShowTrapTrigger()
        {
            if (Lobby != null)
            {
                StartCoroutine(SetCharHitState());
            }
        }

        public LTInstanceNodeTemp GetNodeObjByPos(Vector2 pos)
        {
            if (rowCtrlDic.ContainsKey((int)pos.y))
            {
                if (rowCtrlDic[(int)pos.y].itemObjDic.ContainsKey((int)pos.x))
                {
                    return rowCtrlDic[(int)pos.y].itemObjDic[(int)pos.x];
                }
            }
            return null;
        }

        public LTInstanceNodeTemp GetNodeObjByPos(int posx, int posy)
        {
            if (rowCtrlDic.ContainsKey(posy))
            {
                if (rowCtrlDic[posy].itemObjDic.ContainsKey(posx))
                {
                    return rowCtrlDic[posy].itemObjDic[posx];
                }
            }
            return null;
        }

        public void ClearMoveActionList()
        {
            // 清除掉移动数据，防止界面关闭了，服务器那边已经判定离开了副本，但是这边还在向服务器发移动消息，这样会报 service not found 的错误。
            if (moveActionList != null)
            {
                moveActionList.Clear();
                StopMove();
            }
        }

        private void CreateRowFromCache(LTInstanceNode node)
        {
            GameObject row = null;
            LTInstanceRowCtrl ctrl = null;
            row = GameObject.Instantiate(RowObj);
            ctrl = row.GetMonoILRComponent<LTInstanceRowCtrl>();

            row.CustomSetActive(true);
            row.transform.SetParent(RowObjContainer.transform);
            row.transform.localPosition = new Vector3(node.y * LTInstanceConfig.MAP_X, -node.y * LTInstanceConfig.MAP_Y, -node.y * LTInstanceConfig.MAP_YZ);
            row.transform.localScale = Vector3.one;
            row.name = node.y.ToString();
            mRows.Add(row);
            //------
            ctrl.MapCtrl = this;
            ctrl.Num = node.y;
            ctrl.CreateNodeFromCache(node);
            rowCtrlDic.Add(node.y, ctrl);
        }

        private void AutoMoveMap()
        {
            var targetNode = GetNextPos();
            RequestChallengeMoveChar(targetNode, moveType);
        }
        private LTInstanceNode GetNextPos()
        {
            if (moveActionList == null || moveActionList.Count <= 0)
            {
                return null;
            }

            var targetNode = LTInstanceMapModel.Instance.GetNodeByPos((int)moveActionList[0].x, (int)moveActionList[0].y);
            moveActionList.RemoveAt(0);
            return targetNode;
        }

        private void RealEndHandler(LTInstanceNode targetNode, bool isPath = true)
        {
            if (targetNode == null) return;

            Instance.LTInstanceFloorTemp floorTmp = GetNodeObjByPos(new Vector2(targetNode.x, targetNode.y)) as Instance.LTInstanceFloorTemp;
            if (floorTmp != null && floorTmp.OnCheckGuideNotice()) return;

            if (OnRealEnd != null)
            {
                System.Action<bool> tmp = OnRealEnd;
                OnRealEnd = null;
                tmp(isPath);
                return;
            }

            if (mRealEndNode == null) return;
            if (moveActionList == null || moveActionList.Count > 0) return;
            if (targetNode.IsSameNodePos(mRealEndNode)) return;
            if (mRealEndNode.RoleData != null && mRealEndNode.RoleData.CampaignData != null && !string.IsNullOrEmpty(mRealEndNode.RoleData.CampaignData.Password)) return;

            RequestChallengeMoveChar(mRealEndNode, MoveParam.Direct);
        }

        private void SetSpecialBoob()
        {
            if (LTInstanceMapModel.Instance.SpecialBoob != 0)
            {
                int x = LTInstanceMapModel.Instance.SpecialBoob % 100;
                int y = LTInstanceMapModel.Instance.SpecialBoob / 100;
                var node = LTInstanceMapModel.Instance.GetNodeByPos(x, y);
                int BombTimer = node.RoleData.CampaignData.Bomb;
                if (BombTimer > 0)
                {
                    if (BoomObj == null)
                    {
                        GameObject obj = Instance.LTInstanceOptimizeManager.Instance.GetBoomItem(PlayerObj.transform.Find("Boom"));
                        BoomObj = obj.GetMonoILRComponent<Instance.LTBoomItemTemp>();
                    }
                    BoomObj.SetNum(BombTimer);
                }
                else
                {
                    if (BoomObj != null)
                    {
                        Instance.LTInstanceOptimizeManager.Instance.SetBoomItem(BoomObj.mDMono.gameObject);
                    }
                }
            }
            else
            {
                if (BoomObj != null)
                {
                    Instance.LTInstanceOptimizeManager.Instance.SetBoomItem(BoomObj.mDMono.gameObject);
                }
            }
        }
        
        #region  移动相关
        private bool TryMove(LTInstanceNode start, LTInstanceNode end)
        {
            if (start == null && end != null)//初始化地图时候
            {
                if (RowObjContainer != null)
                {
                    RowObjContainer.transform.localPosition = new Vector3(-(end.x + end.y) * LTInstanceConfig.MAP_X,( end.y-end.x )* LTInstanceConfig.MAP_Y, 0);
                }
                else
                {
                    setRowObjContainerPos = true;
                }
                StopMove();
                return false;
            }
    
            if (start == end)//无法移动的点
            {
                StopMove();
                return false;
            }
    
            if (start != null && end != null)
            {
                MoveCoroutine = StartCoroutine(Move(start, end));
            }
            return true;
        }
    
        private IEnumerator Move(LTInstanceNode start, LTInstanceNode end)
        {
            if (Instance.LTInstanceOptimizeManager.Instance == null || !Instance.LTInstanceOptimizeManager.Instance.MyRoleMoveDir(start, end)|| PlayerObj==null)
            {
                yield break;
            }

            if (!hasSetStartPos)
            {
                hasSetStartPos = true;
                StartPos = new Vector2(start.x ,start .y);
            }
    
            LTInstanceNode.DirType dir = start.GetDirByPos(end.x, end.y);

            float x = (start.x - StartPos.x);
            float y = (start.y - StartPos.y);
            //防止错位
            Vector3 startPos = new Vector3((x+ y) * LTInstanceConfig.MAP_X, (x-y) * LTInstanceConfig.MAP_Y, -start.y * LTInstanceConfig.MAP_YZ + start.x * LTInstanceConfig.MAP_XZ);//  PlayerObj.transform.localPosition.z);//PlayerObj.transform.localPosition;
            Vector3 endPos = startPos;
            if (dir == LTInstanceNode.DirType.UP)
            {
                endPos += new Vector3(-LTInstanceConfig.MAP_X, LTInstanceConfig.MAP_Y, LTInstanceConfig.MAP_YZ);
            }
            else if (dir == LTInstanceNode.DirType.Down)
            {
                endPos += new Vector3(LTInstanceConfig.MAP_X, -LTInstanceConfig.MAP_Y, -LTInstanceConfig.MAP_YZ);
            }
            else if (dir == LTInstanceNode.DirType.Left)
            {
                endPos += new Vector3(-LTInstanceConfig.MAP_X, -LTInstanceConfig.MAP_Y, -LTInstanceConfig.MAP_XZ);
            }
            else if (dir == LTInstanceNode.DirType.Right)
            {
                endPos += new Vector3(LTInstanceConfig.MAP_X, LTInstanceConfig.MAP_Y, LTInstanceConfig.MAP_XZ);
            }

            var node = GetNodeObjByPos(end.x,end.y);
            if (node != null)
            {
                node.OnFloorAni();
            }

            float time = 0;
            while (time < LTInstanceConfig.MODEL_MOVE_TIME)
            {
                Vector3 curTargetPos = Vector3.Lerp(startPos, endPos, time / LTInstanceConfig.MODEL_MOVE_TIME);
                PlayerObj.transform.localPosition = new Vector3(curTargetPos.x, curTargetPos.y, curTargetPos.z);
                time += Time.deltaTime;
                yield return null;
            }
            
            PlayerObj.transform.localPosition = endPos;

            if (FloorDown != null)
            {
                FloorDown();
            }

            MoveCoroutine = null;
    
            if (LTInstanceMapModel.Instance.moveResultList.Count <= 0)
            {
                if (HasNext())
                {
                    AutoMoveMap();
                }
                else
                {
                    yield return null;
                    StopMove();
                    if (OnRealEnd != null)
                    {
                        RealEndHandler(end);
                    }
                }
            }
            else
            {
                MajorDataUpdateFunc();
            }
        }
    
        private void StopMove()
        {
            if (Lobby != null)
            {
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kReady);
            }
            Instance.LTInstanceOptimizeManager.Instance?.HideSelectObj();
        }
        #endregion

        private IEnumerator SetCharHitState()
        {
            yield return null;
            Lobby.SetCharMoveState(MoveController.CombatantMoveState.kHitReaction, false, MoveController.CombatantMoveState.kReady);
        }
    
        private void OnMapViewBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            GlobalMenuManager.Instance.Open("LTInstanceSmallMapView", curNode);
        }

        private void OnTouchMapBtnClick()
        {
            //屏幕点击输入
            if (Camera.main != null && !WaitOptimize)
            {
                var pos = Camera.main.ScreenToWorldPoint(UICamera.lastEventPosition);

                var pos2 = Instance.LTInstanceUtils.ClickPos2GridPos(new Vector2(pos.x, pos.y+0.45f),RowObjContainer.transform.position, 1.8f, 0.8f);
                var node = GetNodeObjByPos(pos2);
                if (node != null)
                {
                    node.OnFloorClick();
                }

                //foreach (var row in rowCtrlDic.Values)
                //{
                //    foreach (var node in row.itemObjDic.Values)
                //    {
                //        if (node.nodeData.Type == LTInstanceNode.NodeType.Floor)
                //        {
                //            Vector2[] vec2s = new Vector2[4];
                //            float nodex = node.mDMono.transform.position.x;
                //            float nodey = node.mDMono.transform.position.y;
                //            vec2s[0] = new Vector2(nodex, nodey - 0.3f);
                //            vec2s[1] = new Vector2(nodex - 0.9f, nodey);
                //            vec2s[2] = new Vector2(nodex, nodey + 0.5f);
                //            vec2s[3] = new Vector2(nodex + 0.9f, nodey);
                //            if (IsPointInPolygon(pos, vec2s))
                //            {
                //                node.OnFloorClick();
                //                break;
                //            }
                //        }
                //    }
                //}
            }
        }
        private bool IsPointInPolygon(Vector2 p, Vector2[] vertexs)
        {
            int crossNum = 0;
            int vertexCount = vertexs.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 v1 = vertexs[i];
                Vector2 v2 = vertexs[(i + 1) % vertexCount];
                if (((v1.y <= p.y) && (v2.y > p.y)) || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNum += 1;
                    }
                }
            }
            if (crossNum % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void RequestChallengeMoveChar(LTInstanceNode targetNode, MoveParam type)
        {
            if (targetNode == null || LTInstanceMapModel.Instance.CurNode == null) return;

            int dir = (int)LTInstanceMapModel.Instance.CurNode.GetDirByPos(targetNode.x, targetNode.y);
            if (dir <= 0)
            {
                if (moveActionList != null && moveActionList.Count > 0)ClearMoveActionList();
                return;
            }

            bool isLoayout = !string.IsNullOrEmpty(targetNode.Layout);

            if(type == MoveParam.Fabricate)
            {
                LTInstanceMapModel.Instance.SimulateFunc(dir);
                return;
            }
            
            //直接或Boss区域特殊逻辑或时，直接调用协议，不等待
            if (type == MoveParam.Direct || LTInstanceMapModel.Instance.IsBossArea())
            {
                RequestMoveCharWithCallback(dir, isLoayout);
                return;
            }

            //前端模拟
            if (type == MoveParam.Simulate  && (HasNext() || !HasNext() && (targetNode.RoleData.Id == 0 || LTInstanceConfig.EmptyRoleList.Contains(targetNode.RoleData.Img))))
            {
                LTInstanceMapModel.Instance.SimulateFunc(dir);
                return;
            }

            //同步数据回调后才能调用
            InstanceWaitUpdataMap(delegate
            {
                RequestMoveCharWithCallback(dir, isLoayout);
            }, false);
        }

        private void RequestMoveCharWithCallback(int dir, bool isLayout)
        {
            isWaitForUpdataMap = true;
            if (isLayout)
            {
                InputBlockerManager.Instance.Block(InputBlockReason.UI_SERVER_REQUEST, 2f);
            }
            LTInstanceMapModel.Instance.RequestMoveChar(dir, delegate (bool moved)
            {
                if (moved)//如果没有任何移动，说明服务器已经返回停止移动（可能是因为阻挡，事件等）
                {
                    moveType = MoveParam.None;
                    AutoMoveMap();
                }
                else
                {
                    ClearMoveActionList();
                }
                isWaitForUpdataMap = false;
            });
        }

    }
}
