using UnityEngine;
using System.Collections;
using System;


namespace Hotfix_LT.UI
{
    public class GuideNode
    {
        /// <summary> 友盟ID</summary>
        public string UmengID;
        /// <summary> 分组ID</summary>
        public int GroupID;
        /// <summary> 步骤ID 全局唯一</summary>
        public int StepID;
        /// <summary> 前置ID</summary>
        public int ForeID;
        /// <summary> 后步骤ID</summary>
        public int NextID;

        /// <summary> 步骤类型 0非强制 1强制</summary>
        public int[] StepType;
        /// <summary> 引导的条件界面 </summary>
        public string FocusView = string.Empty;

        /// <summary> 回滚ID</summary>
        public int RollBackID;
        /// <summary> 跳过至ID</summary>
        public int SkipToID;
        /// <summary> 条件事件指令</summary>
        public string ConditionEventCmd;
        /// <summary> 条件参数</summary>
        public string ConditionParameter;
        /// <summary> 条件回执判断类型 0等于 1大于 2小于 </summary>
        public int ConditionReceiptType;
        /// <summary> 条件需要参数</summary>
        public string ConditionNeedParameter;
        /// <summary> 执行事件指令</summary>
        public string ExecuteEventCmd;
        /// <summary> 执行事件参数</summary>
        public string ExecuteParameter;
        /// <summary> 执行失败处理类型 0跳过 1回滚</summary>
        public int ExecuteFailType;

        /// <summary>
        /// 是否仅为条件
        /// </summary>
        public bool isOnlyCondition;

        public string ConditionCmd;

        public string ExecuteCmd;

        public bool isConditionReceipt;
        public bool isExecuteReceipt;
        public bool isConditionSucess;
        public bool isExecuteSucess;
        /// <summary> 不等待就执行 执行组合表现的时候需要</summary>
        public bool isNoWait;

        /// <summary> 所在线首节点id 方便查找完成 因为服务器记录首节点完成</summary>
        public int firstNodeStepID;

        public bool isPass;//保留 暂时无用
        public string conditionReceiptContent; //最近一次收到的条件回参 暂时不用

        private bool _isCompleted;
        private bool _isOnceUndone; //一次不可完成
        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        public void SetCompleted()
        {
            if (_isOnceUndone)
            {
                _isOnceUndone = false;
                return;
            }
            _isCompleted = true;
        }

        public void SetOnceUndone()
        {
            _isOnceUndone = true;
            _isCompleted = false;
        }


        private void ConditionReceipt(bool isSucess, string receiptParameter)
        {
            double need = 0;
            double receipt = 0;
            isConditionReceipt = true;

            if (!isSucess) //如果非正常返回
            {
                return;
            }

            switch (ConditionReceiptType)
            {
                case 0:
                    if (string.IsNullOrEmpty(ConditionNeedParameter) || ConditionNeedParameter.Equals(receiptParameter)) //如果需要参数为空 或相等
                    {
                        isConditionSucess = true;
                    }
                    else
                    {
                        isConditionSucess = false;
                    }
                    break;
                case 1:
                    need = double.Parse(ConditionNeedParameter);
                    receipt = double.Parse(receiptParameter);
                    if (receipt > need)
                    {
                        isConditionSucess = true;
                    }
                    else
                    {
                        isConditionSucess = false;
                    }
                    break;
                case 2:
                    need = double.Parse(ConditionNeedParameter);
                    receipt = double.Parse(receiptParameter);
                    if (receipt < need)
                    {
                        isConditionSucess = true;
                    }
                    else
                    {
                        isConditionSucess = false;
                    }
                    break;
            }

        }

        private void ExecuteReceipt(bool isSucess)
        {
            isExecuteReceipt = true;
            isExecuteSucess = isSucess;
        }

        public void DispatchCondition()
        {
            isConditionReceipt = false;
            isConditionSucess = false;
            NodeMessageManager.GetInstance().DispatchCondition(/*GroupID,*/ ConditionCmd, ConditionParameter, ConditionReceipt);
        }

        public void DispatchExecute()
        {
            isExecuteReceipt = false;
            NodeMessageManager.GetInstance().DispatchExecute(/*GroupID,*/ ExecuteCmd, ExecuteParameter, ExecuteReceipt);
        }

        public void Process()
        {
            if (string.IsNullOrEmpty(ConditionEventCmd))
            {
                ConditionCmd = NodeMessageManager.None;
            }
            else
            {
                ConditionCmd = ConditionEventCmd;
            }

            if (string.IsNullOrEmpty(ExecuteEventCmd))
            {
                isOnlyCondition = true;
                ExecuteCmd = NodeMessageManager.None;
            }
            else
            {
                ExecuteCmd = ExecuteEventCmd;
            }
            switch (ExecuteCmd) //执行需要特殊处理的
            {
                case NodeMessageManager.Monolog:  //表现需要
                case NodeMessageManager.TouchMengBan:
                case NodeMessageManager.SetGuideType:
                case NodeMessageManager.SetLockCamera:
                case NodeMessageManager.MoveTips:
                    if (ConditionCmd.Equals(NodeMessageManager.None))
                    {
                        isNoWait = true; //没有条件的节点才是不需要等待立刻执行
                    }
                    else
                    {
                        isNoWait = false;
                    }
                    break;
                default:
                    isNoWait = false;
                    break;
            }

            switch (ConditionCmd) //条件需要特殊处理的
            {
                case NodeMessageManager.GetFocusView:
                    ConditionNeedParameter = ConditionNeedParameter.Replace("(Clone)", ""); //如果是焦点判断 无视Clone这种默认添加的
                    break;
            }
        }

        private T StringConvertToEnum<T>(T df, string str)
        {
            T t = df;
            try
            {
                t = (T)Enum.Parse(typeof(T), str);
            }
            catch (Exception ex)
            {
                return df;
            }

            return t;
        }
    }

    public class GuideNodeEvent
    {
        public static Action<string> CombatBtnEvent;
    }
}