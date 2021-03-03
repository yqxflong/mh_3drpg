using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EB.Sparx;
using System.Text;
using System.Reflection.Emit;

namespace Hotfix_LT.UI
{
    /// <summary>
    ///  引导管理器
    /// </summary>
    public class GuideNodeManager : ManagerUnit
    {

        #region Instance
        private static GuideNodeManager _instance;
        public static GuideNodeManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = LTHotfixManager.GetManager<GuideNodeManager>();
            }
            return _instance;
        }
        #endregion

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            Open();
        }
        
        #region const
        public const string GUIDE_CANNOT_RETURN = "ID_codefont_in_GuideNodeManager_564";//新手引导中，无法退出
        public const string GUIDE_CANNOT_FUNCCLICK = "ID_codefont_in_GuideNodeManager_627";//新手引导中暂时无法点击
        public const string GUIDE_CANNOT_PARTNER = "ID_codefont_in_GuideNodeManager_690";//请先拖拽小伙伴上阵，在进行下一步引导
        public const string GUIDE_FUNCTION_OPEN = "NPC_UI_EXIT";//手指点击右侧功能栏展开的

        /// <summary> 等待超时时间 (单位秒)  </summary>
        public float TimeOut = 60;
        /// <summary> 回滚的循环判断时间 </summary>
        const int RollbackMaxNum = 99999999; //改成接近无限 应对条件过程依赖 //240; //大概4秒 对应比较慢打开的界面 

        const float _frameWaitTime = 0.5f;
        #endregion


        #region Member
        /// <summary>是否为新手引导状态 </summary>
        public static bool IsGuide{get;set;}
        
        /// <summary>新手引导执行中</summary>
        public static bool IsRunGuide{get;set;}

        public static bool IsVirtualBtnGuide;

        public static string VirtualBtnStr;

        public static bool isFuncOpenGuide;

        public static string FuncOpenGuideId = string.Empty;

        public static string GateString = string.Empty;

        public static int partnerStatID = 0;

        public static string GuideFailState = "None";

        ///当前ForbidOther的Id
        private static int m_currentGuideId;
        public static int currentGuideId
        {
            get{
                return m_currentGuideId;
            }
            set{
                m_currentGuideId = value;
            }
        }

        /// <summary>战斗禁止点击选择怪物的状态</summary>
        public static bool combatLimitClick
        {
            get { return (IsGuide && (MengBanController.Instance.controller.gameObject.activeSelf || SceneLogic.BattleType == eBattleType.FirstBattle)); }
        }

        /// <summary> 总节点列表</summary>
        public Dictionary<int, GuideNode> dicNode;

        /// <summary> 保存有全部起始节点的列表</summary>
        public List<GuideNode> listStartNode;

        /// <summary> 保存全部完成后的首节点ID</summary>
        public static List<int> listCompleteFirstStepID;

        /// <summary> 服务器记录的首节点的首节点分组和ID</summary>
        public static Dictionary<int, int> listCompleteIntStepID;

        /// <summary> 是否收到基础节点数据 </summary>
        public static bool isReceiveGuideNodes;

        /// <summary> 是否收到完成节点数据 </summary>
        public static bool isReceiveCompletedData;

        public static bool isFirstCombatGuide()
        {
            return GameEngine.Instance.IsFTE;
        }

        public static float/*[]*/ _startTime;
        public static float/*[]*/ _beforeMemoryTime;
        public static string _serverSaveStr = string.Empty;
        #endregion


        #region Private Mem
        ///条件管理器
        private CommonConditionParse _commonCondition;

        ///执行管理器
        private CommonExecuteParse _commonExecute;

        ///当前在跑的一套引导模块
        private GuideNode _currentGuideNodeArray;

        private GuideNodeAPI mGuideNodeApi;

        ///
        private WaitForSeconds _normalFrameWait;

        //准备开启引导的携程,准备好则结束
        private Coroutine _cPrepare;
        //处理当前引导的携程,处理完则结束
        private Coroutine _cProcessCurrentArray;

        //Update的Seq
        private int _Update_Seq = 0;
        #endregion


        #region Private Func
        /// <summary>执行跳过</summary>
        public static Action<string> ExecuteJump;
        public static Action<bool> ExecuteGuideAudio;
        /// <summary> 当前stepID</summary>
        public int CurrentStepID(/*int GroupID*/)
        {
            return (_currentGuideNodeArray == null/*|| _currentGuideNodeArray[index]==null */) ? 0 : _currentGuideNodeArray/*[index]*/.StepID;
        }
       
        /// <summary> 当前stepID</summary>
        public string CurrentUmengID(/*int GroupID*/)
        {
            return (_currentGuideNodeArray == null /*|| _currentGuideNodeArray[index] == null*/) ? null : _currentGuideNodeArray/*[index]*/.UmengID;
        }

        ///当前GuideID
        public int CurrentGroupID()
        {
            return _currentGuideNodeArray == null ? -1 : _currentGuideNodeArray.GroupID;
        }
        #endregion

        public GuideNodeManager()
        {
            dicNode = new Dictionary<int, GuideNode>();
            listStartNode = new List<GuideNode>();
            listCompleteFirstStepID = new List<int>();
            listCompleteIntStepID = new Dictionary<int, int>();
            _normalFrameWait = new WaitForSeconds(0.5f);
        }

        ///监控器Update
        private void Update(int seq) {
            if(_currentGuideNodeArray != null){ //当前有引导要处理
                if(_cProcessCurrentArray == null)
                {
                    _cProcessCurrentArray = EB.Coroutines.Run(ProcessCurrent());
                }
                return;
            }
            else if(_cProcessCurrentArray != null){//当前没引导
                EB.Coroutines.Stop(_cProcessCurrentArray);_cProcessCurrentArray=null;
            }
            
            #region 当前没有步骤
            bool isGroupCompleted = true;
            // 遍历初始节点
            for (int i = 0; i < listStartNode.Count; i++)
            {
                var node = listStartNode[i];
                //前置ID不为0
                if (node.ForeID != 0)
                {
                    continue;
                }

                //此节点已完成
                if (node.IsCompleted)
                {
                    continue;
                }

                //判断执行该条件前的前置完成条件
                if (node.StepType != null)
                {
                    if (node.FocusView.Equals(CommonConditionParse.FocusViewName)
                        &&listCompleteIntStepID.TryGetValue(node.StepType[0], out var step0ID) &&
                        node.StepType[1] <= step0ID)
                    {
                        //特殊条件提前判断
                        switch (node.ConditionCmd)
                        {
                            case NodeMessageManager.GetGuideType:
                                {
                                    if (GuideNodeManager.IsGuide)
                                    {
                                        isGroupCompleted = false;
                                        continue;
                                    }
                                }; break;
                            case NodeMessageManager.GetChallengeDiedAction:
                                {
                                    isGroupCompleted = false;
                                    continue;
                                };
                            default: { }; break;
                        }
                    }
                    else
                    {
                        isGroupCompleted = false;
                        continue;
                    }
                }

                isGroupCompleted = false;

                node.DispatchCondition();
                //条件满足
                if (node.isConditionSucess)
                {
                    if (!node.isOnlyCondition)
                    {
                        node.DispatchExecute();
                        if (node.isExecuteSucess)
                        {
                            GuideNode buff = GetNode(node.NextID);
                            if (buff == null)
                            {
                                SetLinkCompleted(node);//标记完成
                            }
                            _currentGuideNodeArray = buff;
                            break;
                        }
                        else
                        {
                            switch (node.ExecuteFailType)
                            {
                                case 1:
                                    GuideNode buff = GetNode(node.RollBackID);
                                    if (buff == null)
                                    {
                                        EB.Debug.LogError(" GuideNode {0}no RollBackID" , node.StepID);
                                    }
                                    _currentGuideNodeArray = buff;
                                    continue;
                                case 0:
                                default:
                                    if (node.SkipToID == 0)
                                    {
                                        //这个相关联节点都成功 并发送告知服务器 
                                        SetLinkCompleted(node);
                                        _currentGuideNodeArray = null;
                                    }
                                    else
                                    {
                                        _currentGuideNodeArray = GetNode(node.SkipToID);
                                        if (_currentGuideNodeArray == null) //假如因为填写异常 报错提示并完成
                                        {
                                            EB.Debug.LogError("GuideNode SkipToID {0}no found,so Completed. " , node.SkipToID);
                                            SetLinkCompleted(node);
                                        }
                                    }
                                    continue;
                            }
                        }
                    }
                    else //处理仅条件节点 相当于同帧并行判断
                    {
                        GuideNode buff = GetNode(node.NextID);
                        while (buff != null && buff.isOnlyCondition) //遍历仅条件节点 不含执行的
                        {
                            buff.DispatchCondition();
                            if (buff.isConditionSucess)
                            {
                                buff = GetNode(buff.NextID);
                            }
                            else //如果不成功 仅条件节点就需要回滚 因为其是多条件组合 
                            {
                                buff = GetNode(buff.RollBackID);
                                break;
                            }
                        }

                        //空或 含执行节点 交由下一逻辑帧处理
                        _currentGuideNodeArray = buff; 
                        if(_currentGuideNodeArray!=null) break;
                    }
                }
            }
            #endregion

            //全部引导结束,关闭携程，Update
            if (isGroupCompleted) 
            {
                ILRTimerManager.instance.RemoveTimerSafely(ref _Update_Seq);
                EB.Coroutines.Stop(_cProcessCurrentArray);_cProcessCurrentArray=null;
            }
        }

        public override void Initialize(Config config)
        {
            mGuideNodeApi = new GuideNodeAPI();
            mGuideNodeApi.ErrorHandler += ErrorHandler;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public void SetGuideNodes(List<Hotfix_LT.Data.GuideNodeTemplate> listNodeTpl)  
        {
            dicNode.Clear();
            for (int i = 0; i<listNodeTpl.Count; i++)
            {
                Hotfix_LT.Data.GuideNodeTemplate tplNode = listNodeTpl[i];
                if (dicNode.ContainsKey(tplNode.step_id))
                {
                    continue;
                }

                GuideNode node = new GuideNode();
                node.UmengID = tplNode.umeng_id;
                node.GroupID = tplNode.group_id;
                node.StepID = tplNode.step_id;
                node.NextID = tplNode.next_id;
                node.ForeID = tplNode.fore_id;
                if (tplNode.step_type != null)
                {
                    node.StepType = tplNode.step_type;
                    node.FocusView = tplNode.focus_view;
                }
                node.RollBackID = tplNode.roll_back_id;
                node.SkipToID = tplNode.skip_to_id;
                node.ConditionEventCmd = tplNode.condition_cmd;
                node.ConditionParameter = tplNode.c_parameter;
                switch (tplNode.c_receipt_type)
                {
                    case "=":
                        node.ConditionReceiptType = 0;
                        break;
                    case ">":
                        node.ConditionReceiptType = 1;
                        break;
                    case "<":
                        node.ConditionReceiptType = 2;
                        break;
                }
                node.ConditionNeedParameter = tplNode.c_need_parameter;
                node.ExecuteEventCmd = tplNode.execute_cmd;
                node.ExecuteParameter = tplNode.e_parameter;
                node.ExecuteFailType = tplNode.e_fail_type;
                node.Process();
                dicNode.Add(node.StepID, node);
            }
            isReceiveGuideNodes = true;
        }

        ///新手引导开启准备阶段
        private IEnumerator PrepareOpen()
        {
            while (!isReceiveGuideNodes) //等待收到所有引导节点的数据
            {
                yield return null;
            }

            while (!isReceiveCompletedData) //等待收到完成数据
            {
                yield return null;
            }

            if (_cProcessCurrentArray != null)
            {
                EB.Coroutines.Stop(_cProcessCurrentArray);
            }

            bool isError = false;

            listCompleteFirstStepID.Clear();
            listCompleteIntStepID.Clear();
            List<int> temp = new List<int>();
            string[] saveCompletes = _serverSaveStr.Split('%');
            for (int i = 0; i < saveCompletes.Length; i++)
            {
                if (!string.IsNullOrEmpty(saveCompletes[i]))
                {
                    if (dicNode.ContainsKey(int.Parse(saveCompletes[i])))
                    {
                        listCompleteIntStepID.Add(dicNode[int.Parse(saveCompletes[i])].GroupID, int.Parse(saveCompletes[i]));
                    }
                    temp.Add(int.Parse(saveCompletes[i]));
                }
            }
            SetStartNodes();
            // 配置完成的节点
            //bool isLoadGuideAudio = false;
            for (int i = 0; i < temp.Count; i++)
            {
                if (dicNode.ContainsKey(temp[i]))
                {
                    int groupID = dicNode[temp[i]].GroupID;
                    for (int j = 0; j < listStartNode.Count; j++)
                    {
                        if (listStartNode[j].GroupID == groupID)
                        {
                            if (listStartNode[j].StepID <= temp[i] && dicNode.ContainsKey(listStartNode[j].StepID))
                            {
                                GuideNode node = dicNode[listStartNode[j].StepID];
                                if (node == null) continue;
                                SetLinkCompleted(node, false);
                                listCompleteFirstStepID.Add(int.Parse(saveCompletes[i]));
                            }
                            //else if (listStartNode[j].GroupID == 0 && listStartNode[j].StepID > temp[i])//判断是否要加载引导音效
                            //{
                            //    isLoadGuideAudio = true;
                            //}
                        }
                    }
                }
            }
            //if (isLoadGuideAudio || temp.Count == 0)
            //{
            //    FusionAudio.LoadGuideAudioClip();
            //}
            //else
            //{
            //    FusionAudio.ReleaseGuideAudioClips();
            //}

            //没有错误，开启新手引导
            if (!isError)
            {
                if (_commonCondition != null)
                {
                    _commonCondition.Dispose();
                }

                if (_commonExecute != null)
                {
                    _commonExecute.Dispose();
                }

                _commonCondition = new CommonConditionParse();
                _commonExecute = new CommonExecuteParse();
                _startTime = 0;
                _beforeMemoryTime = 0;
                _currentGuideNodeArray = null;
                if(_Update_Seq == 0)
                {
                    _Update_Seq = ILRTimerManager.instance.AddTimer(1, 0, Update);
                }
            }
        }

        ///开启新手引导总入口
        public void Open()
        {
            if (!GameEngine.Instance.IsFTE) { return; }

            Close();
            _cPrepare = EB.Coroutines.Run(PrepareOpen());

            mGuideNodeApi.GetGuideNodeCompleted((Response response) =>
            {
                if (response.sucessful)
                {
                    _serverSaveStr = EB.Dot.String("guideNodeInfo", response.hashtable, string.Empty);
                    isReceiveCompletedData = true;
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// 获取上传的新手引导完成状态的api
        /// </summary>
        /// <param name="callback"></param>
        public void GetGuideNodeCompleted(System.Action<bool> callback = null)
        {
            mGuideNodeApi.GetGuideNodeCompleted((Response response) =>
            {
                if (response.sucessful)
                {
                    string str = EB.Dot.String("guideNodeInfo", response.hashtable, null);
                    if (callback != null)
                    {
                        callback(string.IsNullOrEmpty(str));
                    }
                    return true;
                }
                return false;
            });
        }

        ///关闭新手引导
        public void Close()
        {
            if(_currentGuideNodeArray != null)
            {
                EB.Debug.Log("GuideNodeManager Close _currentGuideNodeArray!");
                MengBanController.Instance.UnFobiddenAll();
                MengBanController.Instance.Hide();
                GuideToolController.Instance.Hide();
                IsGuide = false;
                IsVirtualBtnGuide = false;
                isFuncOpenGuide = false;
                FuncOpenGuideId = string.Empty;
                GateString = string.Empty;
                partnerStatID = 0;
                VirtualBtnStr = null;
                currentGuideId = -1;
                if (_currentGuideNodeArray != null && !_currentGuideNodeArray.isExecuteReceipt)
                {
                    NotifyExecuteJump(_currentGuideNodeArray.ExecuteCmd); //执行跳过
                }
                _currentGuideNodeArray = null;
            }

            if (_cPrepare != null)
            {
                EB.Coroutines.Stop(_cPrepare);
                _cPrepare = null;
            }
            if (_cProcessCurrentArray != null)
            {
                EB.Coroutines.Stop(_cProcessCurrentArray);
                _cProcessCurrentArray = null;
            }
            ILRTimerManager.instance.RemoveTimerSafely(ref _Update_Seq);
            
            isReceiveCompletedData = false;
            _commonCondition = null;
            _commonExecute = null;
            GuideNodeManager.GateString = string.Empty;

        }

        public void MainClose()
        {
            GuideNodeManager.IsVirtualBtnGuide = false;
            GuideNodeManager.IsGuide = false;
            if (currentGuideId == -1)
            {
                EB.Debug.LogError("GuideNodeManager currentguideId = -1 is Error!");
                return;
            }

            List<GuideNode> Temp = new List<GuideNode>();
            if (currentGuideId == 0)
            {
                for (int i = 0; i < listStartNode.Count; i++)
                {
                    if (listStartNode[i].GroupID == 1)
                    {
                        Temp.Add(listStartNode[i]);
                    }
                }
            }
            for (int i = 0; i < listStartNode.Count; i++)
            {
                if (listStartNode[i].GroupID == GuideNodeManager.currentGuideId)
                {
                    Temp.Add(listStartNode[i]);
                }
            }

            for (int i = 0; i < Temp.Count; i++)
            {
                if (i == Temp.Count - 1)
                {
                    SetLinkCompleted(Temp[i], true);
                }
                else
                {
                    SetLinkCompleted(Temp[i], true, false);
                }
            }
            
            if (_currentGuideNodeArray != null && !_currentGuideNodeArray.isExecuteReceipt)
            {
                NotifyExecuteJump(_currentGuideNodeArray.ExecuteCmd); //执行跳过
            }

            if(_currentGuideNodeArray != null)
            {
                string ExecuteJumpStr = "直接跳过引导";
                FusionTelemetry.GuideData.PostEvent(ExecuteJumpStr, _currentGuideNodeArray.StepID, LoginManager.Instance.LocalUser.CreateTime);
                GlobalUtils.FBSendRecordEvent(ExecuteJumpStr);
            }
        }

        /// <summary>
        /// 保存起始节点及设置首节点
        /// </summary>
        void SetStartNodes()
        {
            listStartNode.Clear();
            foreach (var v in dicNode)
            {
                if (v.Value.ForeID == 0)
                {
                    listStartNode.Add(v.Value);
                    v.Value.firstNodeStepID = v.Value.StepID;
                }
                else
                {
                    GuideNode fn = GetFirstNode(v.Value);
                    if (fn == null)
                    {
                        v.Value.firstNodeStepID = v.Value.StepID;
                    }
                    else
                    {
                        v.Value.firstNodeStepID = fn.StepID;
                    }
                }
            }
        }

        /// <summary>
        /// 跳过机制，目前执行的为组跳过，同一组的跳过后不再执行
        /// </summary>
        /// <param name="em"></param>
        private void NotifyExecuteJump(string em)
        {
            ExecuteJump?.Invoke(em);
        }

        ///处理当前Guide
        IEnumerator ProcessCurrent()
        {
            while (true)
            {
                if (_currentGuideNodeArray/*[arrayIndex]*/ != null) //有当前步骤
                {
                    _currentGuideNodeArray/*[arrayIndex]*/.DispatchCondition();
                    _startTime/*[arrayIndex]*/ = Time.realtimeSinceStartup;
                    while (!_currentGuideNodeArray/*[arrayIndex]*/.isConditionReceipt)
                    {
                        if (Time.realtimeSinceStartup - _startTime/*[arrayIndex]*/ > TimeOut)//超时处理
                        {
                            break;
                        }
                        yield return null;
                    }

                    if (_currentGuideNodeArray/*[arrayIndex]*/.isConditionSucess)
                    {
                        if (!_currentGuideNodeArray/*[arrayIndex]*/.isOnlyCondition) //非仅条件节点
                        {
                            IsRunGuide = true;
                            _currentGuideNodeArray/*[arrayIndex]*/.DispatchExecute();
                            _startTime/*[arrayIndex]*/ = Time.realtimeSinceStartup;
                            while (!_currentGuideNodeArray/*[arrayIndex]*/.isExecuteReceipt)
                            {
                                yield return null;
                            }

                            if (_currentGuideNodeArray/*[arrayIndex]*/.isExecuteSucess)
                            {
                                GuideNode buff = GetNode(_currentGuideNodeArray/*[arrayIndex]*/.NextID);
                                if (buff == null)
                                {
                                    SetLinkCompleted(_currentGuideNodeArray/*[arrayIndex]*/);
                                }

                                if (buff != null && _currentGuideNodeArray/*[arrayIndex]*/.isNoWait && buff.isNoWait) //当前和上一个节点都是不需要等待就立即执行
                                {
                                    _currentGuideNodeArray/*[arrayIndex]*/ = buff;
                                    continue;
                                }
                                else
                                {
                                    _currentGuideNodeArray/*[arrayIndex]*/ = buff;
                                }
                            }
                            else
                            {
                                switch (_currentGuideNodeArray/*[arrayIndex]*/.ExecuteFailType)
                                {
                                    case 1:
                                        int stepID = _currentGuideNodeArray/*[arrayIndex]*/.StepID;
                                        _currentGuideNodeArray/*[arrayIndex]*/ = GetNode(_currentGuideNodeArray/*[arrayIndex]*/.RollBackID);

                                        if (_currentGuideNodeArray/*[arrayIndex]*/ == null)
                                        {
                                            EB.Debug.LogError(" GuideNode {0}no RollBackID" , stepID);
                                        }
                                        break;
                                    case 0:
                                    default:
                                        if (_currentGuideNodeArray/*[arrayIndex]*/.SkipToID == 0)
                                        {
                                            SetLinkCompleted(_currentGuideNodeArray/*[arrayIndex]*/);
                                            //发送同步完成的信息
                                            _currentGuideNodeArray/*[arrayIndex]*/ = null;
                                        }
                                        else
                                        {
                                            GuideNode buff = GetNode(_currentGuideNodeArray/*[arrayIndex]*/.SkipToID);
                                            if (buff == null) //假如因为填写异常 报错提示并完成
                                            {
                                                EB.Debug.LogError("GuideNode SkipToID {0}no found,so Completed. " , _currentGuideNodeArray/*[arrayIndex]*/.SkipToID );
                                                SetLinkCompleted(_currentGuideNodeArray/*[arrayIndex]*/);
                                            }
                                            _currentGuideNodeArray/*[arrayIndex]*/ = buff;
                                        }
                                        break;
                                }
                            }
                            IsRunGuide = false;
                        }
                        else //处理仅条件节点 相当于同帧并行判断
                        {

                            GuideNode buff = GetNode(_currentGuideNodeArray/*[arrayIndex]*/.NextID);
                            while (buff != null && buff.isOnlyCondition) //遍历仅条件节点 不含执行的
                            {
                                buff.DispatchCondition();
                                _startTime/*[arrayIndex]*/ = Time.realtimeSinceStartup;
                                while (!buff.isConditionReceipt)
                                {
                                    if (Time.realtimeSinceStartup - _startTime/*[arrayIndex]*/ > TimeOut)//超时处理
                                    {
                                        break;
                                    }
                                    yield return null;
                                }

                                if (buff.isConditionSucess) //如果条件成功那么就继续判断后续的仅条件节点
                                {
                                    buff = GetNode(buff.NextID);
                                }
                                else //如果不成功 仅条件节点就需要回滚 因为其是多条件组合 
                                {
                                    buff = GetNode(buff.RollBackID);
                                    break;
                                }
                            }
                            _currentGuideNodeArray/*[arrayIndex]*/ = buff; //空或 含执行节点 交由下一逻辑帧处理
                        }
                    }
                    else  //条件不满足
                    {
                        if (_currentGuideNodeArray/*[arrayIndex]*/.ForeID == 0) //首节点条件不满足直接pass
                        {
                            _currentGuideNodeArray/*[arrayIndex]*/ = null;
                        }
                        else
                        {
                            GuideNode buff = GetNode(_currentGuideNodeArray/*[arrayIndex]*/.RollBackID);
                            if (buff == null)
                            {
                                //连续任务中间节点不满足且无回滚节点 直接标注为完成（除非不写条件就靠执行判断）
                                SetLinkCompleted(_currentGuideNodeArray/*[arrayIndex]*/);
                            }
                            _currentGuideNodeArray/*[arrayIndex]*/ = buff; //跳至需要回滚的节点 或==null 下一逻辑帧处理下一初始节点
                        }
                    }
                }

                yield return null;
            }
        }

        /// <summary>
        /// 是否所有经历的节点都是仅条件节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsForeOnlyCondition(GuideNode node)
        {
            if (!node.isOnlyCondition)
            {
                return false;
            }
            node = GetNode(node.ForeID);
            if (node == null)
            {
                return true;
            }
            return IsForeOnlyCondition(node);
        }

        /// <summary>
        /// 是否关联线已完成
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsLinkCompleted(GuideNode node)
        {
            if (node.IsCompleted)
            {
                return true;
            }
            else
            {
                if (node.firstNodeStepID != node.StepID && dicNode.ContainsKey(node.firstNodeStepID))
                {
                    return dicNode[node.firstNodeStepID].IsCompleted;
                }
                return false;
            }
        }

        /// <summary>
        /// 设置相关联的节点都完成
        /// </summary>
        /// <param name="node"></param>
        /// <param name="isSave"></param>
        public void SetLinkCompleted(GuideNode node, bool isSave = true, bool isSend = true)
        {
            if (node.NextID == 0 && node.ForeID == 0)
            {
                node.SetCompleted();
                if (isSave && node.IsCompleted)//增加一次性不保存的特性判断 必须IsCompleted==true 
                {
                    if (!listCompleteFirstStepID.Contains(node.StepID))
                    {
                        listCompleteFirstStepID.Add(node.StepID);
                    }
                }
            }
            else
            {
                List<GuideNode> buffCompletes = GetLinkNodes(node);
                for (int i = 0; i < buffCompletes.Count; i++)
                {
                    buffCompletes[i].SetCompleted(); //标记完成
                }
                if (isSave && node.IsCompleted)//增加一次性不保存的特性判断 必须IsCompleted==true 
                {
                    if (!listCompleteFirstStepID.Contains(node.StepID))
                    {
                        listCompleteFirstStepID.Add(buffCompletes[0].StepID);
                    }
                }
            }

            #region 如果要保存上传
            if (isSave && isSend) 
            {
                for (var i = 0; i < listCompleteFirstStepID.Count; i++)
                {
                    int stepId = listCompleteFirstStepID[i];
                    var guideNode = dicNode[stepId];

                    if (listCompleteIntStepID.TryGetValue(guideNode.GroupID, out var theStepID))
                    {
                        if (theStepID < stepId)
                        {
                            listCompleteIntStepID[guideNode.GroupID] = stepId;
                        }
                    }
                    else
                    {
                        listCompleteIntStepID[guideNode.GroupID] = stepId;
                    }
                }
                StringBuilder sb = new StringBuilder();
                foreach (var value in listCompleteIntStepID.Values)
                {
                    sb.Append("%");
                    sb.Append(value);
                }
                string data = sb.ToString();
                if(data.Length > 0)
                {
                    data = data.Remove(0, 1);
                }
                mGuideNodeApi.SaveGuideNode(data, (Response res) => { return true; }); //默认完成 
            }
            #endregion
        }

        public void SetLinkOnceUndone(GuideNode node, bool isSave = true, bool isSend = true)
        {
            if (node.NextID == 0 && node.ForeID == 0)
            {
                node.SetOnceUndone();
                if (isSave)
                {
                    if (listCompleteFirstStepID.Contains(node.StepID))
                    {
                        listCompleteFirstStepID.Remove(node.StepID);
                    }
                    EB.Debug.Log("<color=#00ff00ff>激活StepID = {0}</color>" , node.StepID);
                }
            }
            else
            {
                List<GuideNode> buffUndones = GetLinkNodes(node);
                for (int i = 0; i < buffUndones.Count; i++)
                {
                    buffUndones[i].SetOnceUndone(); //标记一次不能完成
                }
                if (isSave)
                {
                    if (listCompleteFirstStepID.Contains(node.StepID))
                    {
                        listCompleteFirstStepID.Remove(buffUndones[0].StepID);
                    }
                    EB.Debug.Log("<color=#00ff00ff>重新激活StepID = {0}- {1}</color>", buffUndones[0].StepID , buffUndones[buffUndones.Count - 1].StepID);
                }
            }
            if (isSave && isSend) //如果要保存上传
            {
                StringBuilder sb = new StringBuilder();
                bool isfirst = true;
                foreach (var value in listCompleteIntStepID.Values)
                {
                    if (isfirst)
                    {
                        sb.Append(value);
                        isfirst = false;
                        continue;
                    }
                    sb.Append("%");
                    sb.Append(value);
                }
                string data = sb.ToString();
                mGuideNodeApi.SaveGuideNode(data, (Response res) => { return true; }); //默认完成 
            }
        }

        public GuideNode GetFirstNode(GuideNode node)
        {
            GuideNode buff = GetNode(node.ForeID);
            return buff != null ? GetFirstNode(buff) : node;
        }

        public GuideNode GetNode(int stepID)
        {
            dicNode.TryGetValue(stepID, out var node);
            return node;
        }

        public List<GuideNode> GetLinkNodes(GuideNode node)
        {
            List<GuideNode> listNodes = new List<GuideNode>();
            listNodes.Add(node);
            SetForeNode(listNodes);
            SetNextEndNode(listNodes);
            return listNodes;
        }

        public void SetForeNode(List<GuideNode> list)
        {
            int findStepID = list[0].ForeID;
            if (findStepID != 0)
            {
                GuideNode node = GetNode(findStepID);
                if (node == null)
                {
                    EB.Debug.LogError("miss node ForeID={0}" , findStepID);
                    return;
                }
                list.Insert(0, node);
                SetForeNode(list);
            }
        }

        public void SetNextEndNode(List<GuideNode> list)
        {
            int findStepID = list[list.Count - 1].NextID;
            if (findStepID != 0)
            {
                GuideNode node = GetNode(findStepID);
                if (node == null)
                {
                    EB.Debug.LogError("miss node NextID={0}" , findStepID);
                    return;
                }
                list.Add(node);
                SetNextEndNode(list);
            }
        }

        public override void Connect()
        {
            //NONE
        }

        public override void Disconnect(bool isLogout)
        {
            Close();
        }

        public void JumpGuide()
        {
            InputBlockerManager.Instance.UnBlock(InputBlockReason.CONVERT_FLY_ANIM);
            MengBanController.Instance.UnFobiddenAll();
            MengBanController.Instance.Hide();
            GuideToolController.Instance.Hide();

            //这是全跳过(仅跳过该组的引导)
            GuideNodeManager.IsGuide = false;
            GuideNodeManager.GetInstance().MainClose();
            currentGuideId = -1;
        }
    }

    public class NodeMessageManager
    {
        public const string Fail = "0";
        public const string Sucess = "1";
        public const string None = "None";
        /// <summary> 获取引导完成</summary>
        public const string GetGuideComplete = "GetGuideComplete";
        /// <summary> 获取焦点界面</summary>
        public const string GetFocusView = "GetFocusView";
        /// <summary> 获取战斗类型</summary>
        public const string GetBattleType = "GetBattleType";
        /// <summary> 获取剧情对话框状态</summary>
        public const string GetDialogueState = "GetDialogueState";
        /// <summary> 战斗准备完成</summary>
        public const string BattleReady = "BattleReady";
        /// <summary> 当前地图位置</summary>
        public const string CurMapNode = "CurMapNode";
        ///<summary>获取绝对路径下的物体活性</summary>
        public const string GetPathObjAction = "GetPathObjAction";
        ///<summary>获得主线副本team1上阵伙伴数量</summary>
        public const string GetParticipantNum = "GetParticipantNum";
        ///<summary>获得战斗中自己回合</summary>
        public const string GetNeedSetSkill = "GetNeedSetSkill";
        ///<summary>获得引导状态</summary>
        public const string GetGuideType = "GetGuideType";
        ///<summary>获得是否达到开放功能等级</summary>
        public const string GetFuncIsOpen = "GetFuncIsOpen";
        ///<summary>获得是否功能图标能点击状态</summary>
        public const string GetFuncBtnAction = "GetFuncBtnAction";
        ///<summary>是否发现传送门</summary>
        public const string GetFindGate = "GetFindGate";
        ///<summary>MengBanController是否在使用</summary>
        public const string GetMengBanState = "GetMengBanState";
        ///<summary>队伍战斗死亡发生</summary>
        public const string GetChallengeDiedAction = "GetChallengeDiedAction";
        ///<summary>处于挑战副本第几层</summary>
        public const string GetChallengeLevel = "GetChallengeLevel";
        public const string OpenView = "OpenView";
        /// <summary> 关闭界面</summary>
        public const string CloseView = "CloseView";
        /// <summary> 单独显示界面控件，禁止点击其他</summary>
        //ViewForbidOther,
        /// <summary> 独白</summary>
        public const string Monolog = "Monolog";
        /// <summary> 遮挡蒙版响应触摸</summary>
        public const string TouchMengBan = "TouchMengBan";
        /// <summary> 路径对象高亮蒙版</summary>
        //PathTouchMengBan,
        /// <summary> 引导，禁止点击其他</summary>
        public const string ForbidOther = "ForbidOther";
        /// <summary> 等待时间</summary>
        public const string WaitTime = "WaitTime";
        /// <summary> 强制禁止操作</summary>
        public const string ForceForbid = "ForceForbid";
        /// <summary> 设置是否为引导状态（用于特殊处理一些调用）</summary>
        public const string SetGuideType = "SetGuideType";
        /// <summary> 锁定摄像机无法旋转</summary>
        public const string SetLockCamera = "SetLockCamera";
        /// <summary> 模拟触碰按钮</summary>
        public const string ClickButton = "ClickButton";
        /// <summary> 设置对象显示</summary>
        public const string OpenObject = "OpenObject";
        /// <summary> 设置对象关闭</summary>
        public const string CloseObject = "CloseObject";
        /// <summary> 移动提示</summary>
        public const string MoveTips = "MoveTips";
        /// <summary> 设置节点完成</summary>
        public const string SetNodeCompleted = "SetNodeCompleted";
        /// <summary> 设置节点一次不能完成 用来做循环执行的节点 以后不需要循环检测由SetNodeCompleted 来关闭</summary>
        public const string SetNodeOnceUndone = "SetNodeOnceUndone";
        /// <summary> 显示拖拽引导指示</summary>
        public const string ShowDragUI = "ShowDragUI";
        /// <summary> 根据场景3d设置物体设置虚拟按键</summary>
        public const string SetVirtualBtn = "SetVirtualBtn";
        /// <summary> 设置组件的激活状态</summary>
        public const string SetComponentAction = "SetComponentAction";
        /// <summary> 设置开场大战的参数</summary>
        public const string SetFirstBattleValue = "SetFirstBattleValue";
        /// <summary> 功能引导强制回到主界面,只会在升级界面之后</summary>
        public const string SetBackToMianMenu = "SetBackToMianMenu";
        /// <summary> 功能引导强制回到主界面,只会在结算界面之后</summary>
        public const string ReturnToMianMenu = "ReturnToMianMenu";
        /// <summary> 设置功能引导</summary>
        public const string SetFuncOpen = "SetFuncOpen";
        /// <summary> 设置目标节点的前置完成</summary>
        public const string SetCompletedByTargetNode = "SetCompletedByTargetNode";
        /// <summary> 展示主城旋转镜头</summary>
        public const string SetSpecialCam = "SetSpecialCam";
        /// <summary> 展示新章节开启</summary>
        public const string SetNewChapterOpen = "SetNewChapterOpen";
        /// <summary> 设置战斗按钮相关</summary>
        public const string SetCombatBtnMod = "SetCombatBtnMod";
        /// <summary> 在伙伴界面选择指定伙伴 </summary>
        public const string SelectPartnerInPartnerView = "SelectPartnerInPartnerView";
        /// <summary> 手指引导点击副本格子位置 </summary>
        public const string ForbidInstanceFloor = "ForbidInstanceFloor";
        /// <summary> 检查战斗是否为失败 </summary>
        public const string CheckBattleIsFailed = "CheckBattleIsFailed";
        /// <summary> 在伙伴界面指定伙伴进行升级 </summary>
        public const string MakePartnerLevelUp = "MakePartnerLevelUp";
        /// <summary> 在伙伴界面指定伙伴进行一键穿戴 </summary>
        public const string MakePartnerDressEquip = "MakePartnerDressEquip";
        /// <summary> 当玩家等级小于指定等级 </summary>
        public const string OnLessThanLevel = "OnLessThanLevel";
        /// <summary> 获取失败引导触发状态 </summary>
        public const string GetGuideFailState = "GetGuideFailState";
        /// <summary> 设置失败引导触发状态 </summary>
        public const string SetGuideFailState = "SetGuideFailState";

        private static NodeMessageManager _instance;
        public static NodeMessageManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NodeMessageManager();
            }
            return _instance;
        }

        public void DisConnectResetFunc()
        {
            dicConditionNotify = new /*Dictionary<int,*/ Dictionary<string, Callback>/*>*/();
            dicConditionReceipt = new /*Dictionary<int,*/ Dictionary<string, Action<bool, string>>/*>*/();
            dicExecuteNotify = new /*Dictionary<int,*/ Dictionary<string, Callback>/*>*/();
            dicExecuteReceipt = new /*Dictionary<int,*/ Dictionary<string, Action<bool>>/*>*/();
        }

        //支持groupId分组的扩展
        public /*Dictionary<int,*/ Dictionary<string, Callback>/*>*/ dicConditionNotify;
        public /*Dictionary<int,*/Dictionary<string, Action<bool, string>>/*>*/ dicConditionReceipt;

        public delegate void Callback(string str);

        public /*Dictionary<int,*/ Dictionary<string, Callback>/*>*/ dicExecuteNotify;
        public /*Dictionary<int,*/Dictionary<string, Action<bool>>/*> */dicExecuteReceipt;

        private NodeMessageManager()
        {
            dicConditionNotify = new /*Dictionary<int,*/ Dictionary<string, Callback>/*>*/();
            dicConditionReceipt = new /* Dictionary<int,*/ Dictionary<string, Action<bool, string>>/*>*/();
            dicExecuteNotify = new /*Dictionary<int,*/ Dictionary<string, Callback>/*>*/();
            dicExecuteReceipt = new/* Dictionary<int,*/ Dictionary<string, Action<bool>>/*>*/();
        }

        /// <summary>
        /// 添加条件
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void AddCondition(/*int groupId,*/string cmd, Callback callback)
        {
              dicConditionNotify[cmd] = callback;
        }

        /// <summary>
        /// 移除条件
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void RemoveCondition(/*int groupId,*/string cmd, Callback callback)
        {
            if (dicConditionNotify.TryGetValue(cmd, out var act))
            {
                Delegate d = Delegate.Remove(act, callback);
                if (d == null) dicConditionNotify.Remove(cmd);
                else dicConditionNotify[cmd] = (Callback)d;
            }
        }

        /// <summary>
        /// 添加条件接收
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmd"></param>
        /// <param name="act"></param>
        private void AddConditionReceipt(/*int groupId,*/ string cmd, Action<bool, string> act)
        {
             dicConditionReceipt[cmd] = act;
        }

        /// <summary>
        /// 添加执行
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void AddExecute(/*int groupId,*/string cmd, Callback callback)
        {
             dicExecuteNotify[cmd] =  callback;
        }

        /// <summary>
        /// 移除执行
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        public void RemoveExecute(/*int groupId,*/string cmd, Callback callback)
        {
            if (dicExecuteNotify.TryGetValue(cmd, out var act))
            {
                Delegate d = Delegate.Remove(act, callback);
                if (d == null) dicExecuteNotify.Remove(cmd);
                else dicExecuteNotify[cmd] = (Callback)d;
            }
        }

        /// <summary>
        /// 添加执行接受
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="cmd"></param>
        /// <param name="act"></param>
        private void AddExecuteReceipt(/*int groupId,*/string cmd, Action<bool> act)
        {
            dicExecuteReceipt[cmd] = act;
        }

        /// <summary>
        /// 通知条件
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="groupId"></param>
        /// <param name="parameter"></param>
        /// <param name="receipt"></param>
        public void DispatchCondition(/*int groupId,*/ string cmd, string parameter, Action<bool, string> receipt)
        {
            try
            {
                AddConditionReceipt(/*groupId,*/cmd, receipt);
                if (string.IsNullOrEmpty(cmd)/* == eConditionMessage.None*/) //如果是未填写的 就代表忽略条件
                {
                    receipt(true, string.Empty);
                    return;
                }

                if (dicConditionNotify.TryGetValue(cmd, out var act))
                {
                    act?.Invoke(parameter);
                }
                else
                {
                    receipt(false, string.Empty);
                }
            }
            catch (Exception e)
            {
                EB.Debug.LogError("[Error]cmd = {1};parameter = {2};DispatchCondition{0}", e.StackTrace, cmd, parameter);
                receipt(false, string.Empty);
            }
        }

        /// <summary>
        /// 通知条件的返回
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameter"></param>
        public void DispatchConditionReceipt(/*int groupID,*/ string cmd, string parameter)
        {
            if (dicConditionReceipt.TryGetValue(cmd, out var act))
            {
                act?.Invoke(true, parameter);
            }
        }

        /// <summary>
        /// 通知执行
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameter"></param>
        public void DispatchExecute(/*int groupId,*/ string cmd, string parameter, Action<bool> result)
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.3f);
            AddExecuteReceipt(/*groupId,*/ cmd, result);
            if (dicExecuteNotify.TryGetValue(cmd, out var act))
            {
               act?.Invoke(parameter);
            }
        }

        /// <summary>
        /// 通知执行的返回
        /// </summary>
        /// <param name="cmd"> 指令</param>
        /// <param name="isSucess"> 是否正确执行</param>
        public void DispatchExecuteReceipt(/*int groupId,*/ string cmd, bool isSucess)
        {
            if (dicExecuteReceipt.TryGetValue(cmd, out var act))
            {
                act?.Invoke(isSucess);
            }
        }

    }
}