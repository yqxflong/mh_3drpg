using UnityEngine;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{

    /// <summary>
    /// 战斗信息数据类
    /// </summary>
    public class CombatInfoData
    {
        public static CombatInfoData _instance;
        public static CombatInfoData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CombatInfoData();
                _instance.Init();
            }
            return _instance;
        }

        /// <summary>
        /// 战斗日志
        /// </summary>
        public List<string> combatLog;
        static int LogPerPage = 100;
        static int MaxPage = 100;
        private List<string> logCache;
        public string GetLogCache(int Page) { if (logCache != null && Page < logCache.Count) return logCache[Page]; else return ""; }
        public int GetLogCachePages() { if (logCache != null) return logCache.Count; else return 0; }
        public bool Dirty = false;

        public bool isActive;

        private CombatInfoData()
        {
            combatLog = new List<string>();
            logCache = new List<string>();

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                isActive = true;
            }
        }

        private void Init()
        {
            LT.MainMessenger.AddListener("ClearLog", delegate () { ClearLog(); });
            LT.MainMessenger.AddListenerEx<int>("GetLogCachePages", delegate () { return this.GetLogCachePages(); });
            LT.MainMessenger.AddListenerEx<int, string>("GetLogCache", delegate (int logpage) { return this.GetLogCache(logpage); });
            LT.MainMessenger.AddListenerEx<bool>("GetDirty", delegate () { return this.Dirty; });
            LT.MainMessenger.AddListener<bool>("SetDirty", delegate (bool dirty) { this.Dirty = dirty; });
        }

        public void ClearLog()
        {
            combatLog.Clear();
            logCache.Clear();
            Dirty = true;
        }

        public void LogEvent(CombatSyncEventBase e)
        {
            if (ILRDefine.ENABLE_LOGGING){
                LogString("战斗事件类型:" + e.GetType() + ",具体内容:" + e.GenerateLog());
            }
        }

        private void UpdateLogCache(int pos)
        {
            if (ILRDefine.UNITY_EDITOR)
            {
                int page = pos / LogPerPage;

                string cache = "<color=#ffffffff>";
                for (int i = page * LogPerPage; i < (page + 1) * LogPerPage && i < combatLog.Count; i++)
                {
                    cache += combatLog[i];
                }
                cache += "</color>";
                Dirty = true;

                if (page < logCache.Count)
                {
                    logCache[page] = cache;
                }
                else
                {
                    logCache.Add(cache);
                }

                if (logCache.Count > MaxPage)
                {
                    logCache.RemoveAt(0);
                    combatLog.RemoveRange(0, LogPerPage);
                }
            }
        }

        public void LogSeparator(CombatSyncEventBase e)
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                LogString(string.Format("--EventEnd-{0}--\n", e.SubEid));
            }
        }

        public void LogString(string message)
        {
            if (ILRDefine.UNITY_EDITOR && ILRDefine.ENABLE_LOGGING)
            {
                if (isActive)
                {
                    combatLog.Add(message);
                    UpdateLogCache(combatLog.Count - 1);
                }
            }
            EB.Debug.Log(message);
        }

        public void LogError(string msgStr, string msgDetail)
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                string str = "<color=#ff0000ff>Error</color> <color=#ffffffff>msg:" + msgStr + " detail:" + msgDetail + "</color>\n";
                LogString(str);
            }
        }

        public void LogResumeCombat(int combatID)
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                LogString("<color=#00ff00ff>战斗重连 combat_id =" + combatID + "</color>\n");
            }
        }

        public void LogJoinCombat(long combatID)
        {
            if (ILRDefine.ENABLE_LOGGING)
            {
                LogString("<color=#00ff00ff>战斗开始 combat_id =" + combatID + "</color>\n");
            }
        }
    }
}