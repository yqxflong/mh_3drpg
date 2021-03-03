using Johny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIHelp
{
    public class AIHelpManager
    {
        public readonly Dictionary<string, string> languageCode = new Dictionary<string, string>
        {
                {"en","en"}
        };
        private const string _className = "AIHelpManager";
        public static bool IsAihelpInitialize;
        public static bool IshaveUnreadMessage;

        public static void Init(string callbackobj)
        {
            string appid = "";
#if UNITY_ANDROID
            appid = "ASTGame_platform_5a9aac07-835c-480e-b3d9-b742014e7606";
#endif

#if UNITY_IOS
            appid = "ASTGame_platform_171faba6-3103-4c82-b2f6-b0af3e0f52e4";
#endif
            //AIhelpService.enableLogging(true);\
            RegisterInitializationCallback(callbackobj);
            RegisterMessageArrivedCallback(callbackobj);
            AIhelpService.Instance.Initialize(
            "ASTGAME_app_86ec48dfeb414a5f9be45a291aa8147c",
            "ASTGame.aihelp.net",
            appid);         
        }

        /// <summary>
        /// 客服系统显示，条件不同显示不同弹窗,////机器人客服显示
        /// </summary>
        /// <param name="level"></param>
        public static void showConversation(string Username, string uId, string serverId)
        {
            if (IsAihelpInitialize)
            {
                EB.Debug.Log("{0}.showConversation level:{1}", _className, Username, uId, serverId);
                AIhelpService.Instance.ShowElva(Username, uId, serverId, "1");
            }
        }

        /// <summary>
        /// 调用人工客服
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="sid"></param>
        /// <param name="data"></param>
        public static void showConversation(string uid, string sid)
        {
            if (IsAihelpInitialize)
            {
                AIhelpService.Instance.ShowConversation(uid, sid);
            }
        }


        /// <summary>
        /// 调用显示F&q界面 可以根据条件判断
        /// </summary>
        /// <param name="level"></param>
        public static void showFAQSection()
        {
            if (IsAihelpInitialize)
            {
                AIhelpService.Instance.ShowFAQs();
            }
        }

        public static void RegisterInitializationCallback(string gameobject)
        {
            EB.Debug.Log("AIhelpUnity.RegisterInitializationCallback");
            AIhelpService.Instance.RegisterInitializationCallback(gameobject);
        }

        public static void RegisterMessageArrivedCallback(string gameObject) 
        {
            EB.Debug.Log("AIhelpUnity.RegisterMessageArrivedCallback");
            AIhelpService.Instance.RegisterMessageArrivedCallback(gameObject);
        }

        /// <summary>
        /// 设置开启每5分钟轮巡检测
        /// </summary>
        public static void setUnreadMessageFetchUid(string playerUid)
        {          
            if (IsAihelpInitialize)
            {
                EB.Debug.Log("AIhelpUnity.setUnreadMessageFetchUid"+playerUid);
                AIhelpService.Instance.SetUnreadMessageFetchUid(playerUid);
            }
        }

    }
}
