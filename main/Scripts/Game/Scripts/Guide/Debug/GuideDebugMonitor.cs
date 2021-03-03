//GuideDebugMonitor
//引导Debug相关监控器
//Johny

using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using UnityEngine;

public class GuideDebugMonitor : MonoBehaviour
{

#if UNITY_EDITOR && !ILRuntime
    [Header("是否开启了开场大战")]
    [ReadOnlyAttribute(true)]
    public bool isFirstCombatGuide;


    ///是否收到基础节点数据
    [Header("是否收到基础节点数据")]
    [ReadOnlyAttribute(true)]
    public bool isReceiveGuideNodes;

    ///是否收到完成节点数据
    [Header("是否收到完成节点数据")]
    [ReadOnlyAttribute(true)]
    public bool isReceiveCompletedData;

    ///可能是一种开放的状态，像副本，你可以随便点，但好友聊天根据这个状态，你点了就飘字无法点击
    [Header("是否为新手引导状态")]
    [ReadOnlyAttribute(true)]
    public bool IsGuide;

    ///一般像出现手指引导，新手引导提示这种情况下这个为true，主要用来阻塞其他引导的检测，防止两个引导并行的情况
    [Header("新手引导执行中")]
    [ReadOnlyAttribute(true)]
    public bool IsRunGuide;


    [Header("当前正在执行的引导ID")]
    [ReadOnlyAttribute(true)]
    public int currentGuideId;

    [Header("是否开启限制点击")]
    [ReadOnlyAttribute(true)]
    public bool combatLimitClick;


    ///当前界面有虚拟按钮，只有一个
    [Header("是否是虚拟按钮")]
    [ReadOnlyAttribute(true)]
    public bool IsVirtualBtnGuide;

    ///虚拟按钮的参数
    [Header("虚拟按钮的参数")]
    [ReadOnlyAttribute(true)]
    public string VirtualBtnStr;

    ///有新功能开启的时候
    [Header("是否是功能引导")]
    [ReadOnlyAttribute(true)]
    public bool isFuncOpenGuide;

    [Header("新功能开启的参数")]
    [ReadOnlyAttribute(true)]
    public string FuncOpenGuideId;

    ///副本门的位置,存在才赋值
    [Header("挑战副本的门")]
    [ReadOnlyAttribute(true)]
    public string GateString;

    ///特殊引导ID
    [Header("伙伴界面特殊参数")]
    [ReadOnlyAttribute(true)]
    public int partnerStatID;

    ///保存全部完成后的首节点ID
    [Header("保存全部完成后的首节点ID")]
    [ReadOnlyAttribute(true)]
    public List<int> listCompleteFirstStepID;

    [Header("超时判断的开始时间")]
    [ReadOnlyAttribute(true)]
    public float/*[]*/ _startTime;

    [Header("暂停判断的开始时间")]
    [ReadOnlyAttribute(true)]
    public float/*[]*/ _beforeMemoryTime;

    [Header("服务器返回信息")]
    [ReadOnlyAttribute(true)]
    public string _serverSaveStr;


    #region Private
    private Assembly _assembly = null;
    #endregion

    private object MonoGetStaticField(string hotfixClassPath, string propertyName)
    {
        Type type = _assembly.GetType(hotfixClassPath);
        FieldInfo info = type.GetField(propertyName);
        return info.GetValue(null);
    }

    private object MonoGetStaticProperty(string hotfixClassPath, string propertyName)
    {
        Type type = _assembly.GetType(hotfixClassPath);
        PropertyInfo property = type.GetProperty(propertyName);
        return property.GetValue(null);
    }

    private object MonoGetStaticMethod(string hotfixClassPath, string method)
    {
         Type type = _assembly.GetType(hotfixClassPath);
         MethodInfo info = type.GetMethod(method);
         return info.Invoke(null, null);
    }


    void Update()
    {
        if(_assembly == null)
        {
            _assembly = HotfixILRManager.GetInstance().assembly;
        }
        if(_assembly == null)
        {
            return;
        }

        #region GuideManager
        //记录服务器收到数据情况
        isReceiveGuideNodes = (bool)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "isReceiveGuideNodes");
        isReceiveCompletedData = (bool)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "isReceiveCompletedData");
        //-----
        IsGuide = (bool)MonoGetStaticProperty("Hotfix_LT.UI.GuideNodeManager", "IsGuide");
        IsRunGuide = (bool)MonoGetStaticProperty("Hotfix_LT.UI.GuideNodeManager", "IsRunGuide");
        isFirstCombatGuide = (bool)MonoGetStaticMethod("Hotfix_LT.UI.GuideNodeManager", "isFirstCombatGuide");
        combatLimitClick = (bool)MonoGetStaticProperty("Hotfix_LT.UI.GuideNodeManager", "combatLimitClick");
        IsVirtualBtnGuide = (bool)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "IsVirtualBtnGuide");
        VirtualBtnStr = MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "VirtualBtnStr") as string;
        isFuncOpenGuide = (bool)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "isFuncOpenGuide");
        FuncOpenGuideId = MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "FuncOpenGuideId") as string;
        GateString = MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "GateString") as string;
        partnerStatID = (int)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "partnerStatID");
        currentGuideId = (int)MonoGetStaticProperty("Hotfix_LT.UI.GuideNodeManager", "currentGuideId");
        listCompleteFirstStepID = (List<int>)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "listCompleteFirstStepID");
        //----
        _startTime = (float)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "_startTime");
        _serverSaveStr = MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "_serverSaveStr") as string;
        _beforeMemoryTime = (float)MonoGetStaticField("Hotfix_LT.UI.GuideNodeManager", "_beforeMemoryTime");

        #endregion

    }
#endif
}
