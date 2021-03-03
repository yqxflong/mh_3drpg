using System;
using EB.IAP;
using EB.Sparx;

public class DefaultEmptySDKManager:BaseSDKManager
{
    public override bool IsLoggedIn()
    {
        return false;
    }

    public override void Init(object options, Action<string, bool> callback, Action<string> tipCallBack)
    {
    }

    public override void Login(Action<string, object> callback)
    {
    }

    public override void Logout()
    {
    }

    public override void Pay(Item item, Transaction transaction, Action<int> callback)
    {
    }

    public override void Exit()
    {
    }

    public override int GetAppid()
    {
        return 0;
    }

    public override void SetRoleData(RoleData roleData)
    {
    }

    public override void OnJoinQQGroup(string key)
    {
    }

    public override void OnInitResult(bool success, string msg)
    {
    }

    public override void OnLoginResult(int code, object msg)
    {
    }

    public override void OnPayResult(int code)
    {
    }

    public override void OnLoginOutResult(int code)
    {
    }

    public override void OnSwitchResult(int code, object msg) {
    }

    public override void OnExitResult(int code)
    {
    }

    public override void OnShowTipCall(string tip)
    {
       
    }

    public override void SetCurHCCount(int curHCCount)
    {
       
    }
}