using UnityEngine;
using EB.Sparx;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class OppLoginUILogic : UIControllerHotfix
    {
        public GameObject Template;
        public GameObject m_Container;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_EntryGrid = t.GetComponent<UIGrid>("Container/Entries");
            m_Container = t.Find("Container").gameObject;
            controller.hudRoot = t.parent;
            Template = m_EntryGrid.gameObject.FindEx("Template");
            Template.CustomSetActive(false);

        }

        public override void Show(bool isShowing)
        {
            m_Container.CustomSetActive(isShowing);
        }

        public UIGrid m_EntryGrid;
        
        public override IEnumerator OnAddToStack()
    	{
            var temps = LoginManager.Instance.Authenticators;
            if (temps.Length == 0)EB.Debug.LogError("Hub.Instance.LoginManager.Authenticators.Length == 0!");
            for (int i = 0; i < temps.Length; ++i)
            {
                if (AuthenticatorEntryConfig.Dic.ContainsKey(temps[i].Name))
                {
                    var go = GameObject.Instantiate(Template, m_EntryGrid.transform);
                    AuthenticatorEntryController ctrl = go.GetMonoILRComponent<AuthenticatorEntryController>();
                    ctrl.Init(AuthenticatorEntryConfig.Dic[temps[i].Name], delegate (string name) { OnEntryClick(name); });
                    go.CustomSetActive(true);
                }
                else
                {
                   EB.Debug.LogWarning("AuthenticatorEntryConfig.Dic can't find key —— {0}" , temps[i].Name);
                }
            }

            m_EntryGrid.Reposition();

            yield return base.OnAddToStack();
            yield return null;
            Hotfix_LT.Messenger.Raise("LTLoginBGPanelCtrlEven");
        }
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		yield return base.OnRemoveFromStack();
    	}
        
    	public void OnEntryClick(string entryName)
        {
            LoginManager.Instance.Login(entryName);
            controller.Close();
    	}
    }
    
    public class AuthenticatorEntryConfig
    {
        public static Dictionary<string ,AuthenticatorEntry> Dic = new Dictionary<string ,AuthenticatorEntry>()
        {
            {"device" ,new AuthenticatorEntry("device","快速开始") },

            { "account" , new AuthenticatorEntry("account","账号登录") },

            { "as" ,new AuthenticatorEntry("as","爱思助手") },

            { "facebook" ,new AuthenticatorEntry("facebook","Facebook","Login_Icon_Facebook") },

            { "xy" , new AuthenticatorEntry("xy","XY助手") },

            { "kuaiyong" , new AuthenticatorEntry("kuaiyong","快用助手") },

            { "uc" ,new AuthenticatorEntry("uc","UC") },

            { "qihoo" ,new AuthenticatorEntry("qihoo","360")},

            { "oppo" , new AuthenticatorEntry("oppo","OPPO")},

            { "xiaomi" ,  new AuthenticatorEntry("xiaomi","小米")},

            { "vivo" , new AuthenticatorEntry("vivo","VIVO")},

            { "tencent_wx" ,  new AuthenticatorEntry("tencent_wx","微信")},

            { "tencent_qq" ,   new AuthenticatorEntry("tencent_qq","QQ")},

            { "winner" ,   new AuthenticatorEntry("winner","蔚蓝")},

            { "huawei" ,   new AuthenticatorEntry("huawei","华为")},

            { "yijie" ,   new AuthenticatorEntry("yijie","易接")},

            { "ewan" ,  new AuthenticatorEntry("ewan","益玩")},

            { "lb" ,  new AuthenticatorEntry("lb","猎宝")},

            { "k7k" ,   new AuthenticatorEntry("k7k","7k7k")},

            { "asdk" ,   new AuthenticatorEntry("asdk","A游戏")},

            { "manhuang" ,    new AuthenticatorEntry("manhuang","蛮荒游戏","Login_Icon_Manhuang",true) },

            { "google" ,   new AuthenticatorEntry("google","Google")},

            { "m4399" ,   new AuthenticatorEntry("m4399","4399")},

            { "gamecenter" ,   new AuthenticatorEntry("gamecenter","Game Center","Login_Icon_GameCenter",true) },

            { "wechat" ,    new AuthenticatorEntry("wechat","微信","Login_Icon_Weixin")},

            { "vfpk" ,    new AuthenticatorEntry("vfpk","威风PK")},

            { "xinkuai" ,    new AuthenticatorEntry("xinkuai","新快专服")},

            { "xinkuaifx" ,    new AuthenticatorEntry("xinkuaifx","新快专服")},
        };
    }
}
