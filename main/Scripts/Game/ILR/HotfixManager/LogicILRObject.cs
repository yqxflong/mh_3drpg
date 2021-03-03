using EB.Sparx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Other;
/// <summary>
/// 提供给逻辑类使用的对象
/// </summary>
[NeedAdaptor]
public class LogicILRObject
{
    public virtual void Initialize(Config config)
    {
        
    }

    public virtual void Dispose()
    {

    }

    public virtual void Async(string message, object payload)
    {
        EB.Debug.LogError(message);
    }

    public virtual void Disconnect(bool isLogout)
    {
    }

    public virtual void Connect()
    {
    }

    public virtual void OnLoggedIn()
    {

    }
    
    public virtual void Update()
    {
        
    }

    public virtual void OnEnteredBackground()
    {

    }

    public virtual void OnEnteredForeground()
    {

    }
}

public class LogicILR : EB.Sparx.SubSystem,EB.Sparx.Updatable
{
    protected LogicILRObject logicObject;

    public override void Initialize(Config config)
    {
        EB.Coroutines.Run(InitalizeWait(config));
    }

    protected virtual IEnumerator InitalizeWait(Config config)
    {
        while (!HotfixILRManager.GetInstance().IsInit)
        {
            yield return null;
        }
    }

    public override void Dispose()
    {
        if (logicObject != null)
        {
            logicObject.Dispose();
        }
        base.Dispose();
    }

    public override void Async(string message, object payload)
    {
        base.Async(message, payload);
        if (logicObject != null) logicObject.Async(message, payload);
    }

    public override void Disconnect(bool isLogout)
    {
        State = EB.Sparx.SubSystemState.Disconnected;
        if (logicObject != null) logicObject.Disconnect(isLogout);
    }

    public override void Connect()
    {
        State = EB.Sparx.SubSystemState.Connected;
        if (logicObject != null) logicObject.Connect();
    }

    public override void OnLoggedIn()
    {
        if (logicObject != null) logicObject.OnLoggedIn();
    }
    
    public virtual void Update()
    {
        if (logicObject != null) logicObject.Update();
    }

    public bool UpdateOffline { get; }

    public override void OnEnteredBackground()
    {
        if (logicObject != null) logicObject.OnEnteredBackground();
    }

    public override void OnEnteredForeground()
    {
        if (logicObject != null) logicObject.OnEnteredForeground();
    }
}