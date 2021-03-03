using System;
using System.Collections;
using UnityEngine;

namespace EB.Sparx
{
    public abstract class BaseSDKManager
    {
        public abstract bool IsLoggedIn();
        public abstract void Init(object options,Action<string, bool> callback, Action<string> tipCallBack);
        public abstract void Login(Action<string, object> callback);
        public abstract void Logout();
        public abstract void Pay(EB.IAP.Item item, EB.IAP.Transaction transaction, System.Action<int> callback);
        public abstract void Exit();
        public abstract int GetAppid();
        public abstract void SetRoleData(RoleData roleData);
        public abstract void OnJoinQQGroup(string key);
        public abstract void OnInitResult(bool success, string msg);
        public abstract void OnLoginResult(int code, object msg);
        public abstract void OnPayResult(int code);
        public abstract void OnLoginOutResult(int code);  
        public abstract void OnSwitchResult(int code, object msg);  //Only for iOS
        public abstract void OnExitResult(int code);
        public abstract void OnShowTipCall(string tip);
        public abstract void SetCurHCCount(int curHCCount);
        public virtual void SwitchAccount()
        {
            EB.Debug.Log("{0}.SwitchAccount", ToString());
        }

        public virtual bool IsGuest()
        {
            EB.Debug.Log("{0}.IsGuest", ToString());
            return true;
        }
    }
    
    public class RoleData
    {
        public int code;
        public string roleGid; 
        public string roleId;
        public string roleName; 
        public int roleLevel;
        public string serverId;
        public string serverName;
        public int coinNum;
    }
}