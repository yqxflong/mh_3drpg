using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Reflection;
using ILRuntime.CLR.Method;

/// <summary>
/// 非玩家实体控件
/// </summary>
public class EnemyController : Controller
{
    public DynamicMonoILR HotfixController;

    public System.Action<EnemyController> onDestroy;
    
    new void Awake()
    {
        base.Awake();
        HotfixController = GetComponent<DynamicMonoILR>();
        if (HotfixController != null) HotfixController.ILRObjInit();
    }
    
}
