using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace EB
{
    public static class Log
    {
        public enum LogLevel
        {
            LogLevel_Group = 1,
            // 输出信息
            LogLevel_Info,
            // 错误信息
            LogLevel_Error,
            // 警告信息
            LogLevel_Warning,
            // 跟踪信息
            LogLevel_Trace,
        };

        public delegate void LogException();

        private static bool m_IsOpen = false;
        private static Queue<string> m_LogQueue;
        // 是否打开Log
#if UNITY_EDITOR || USE_DEBUG
        static public bool EnableLog = true;
#else
        static public bool EnableLog = false;
#endif
        // 允许输出的最大Log等级
        static public LogLevel MaxLogLevel = LogLevel.LogLevel_Trace;

        static public bool m_bWriteLogFile = false;

        static private StreamWriter m_LogFile = null;
        // log异常回调
        static public LogException logException = null;
        static public List<string> m_lstOpenGroup = null;
        static private string m_strGroupName = "";

        public static void OpenLog(LogLevel maxLevel, bool bWriteLogFile)
        {
            if (m_IsOpen)
            {
                return;
            }

            m_IsOpen = true;

            EnableLog = true;
            // 开启的组
            m_lstOpenGroup = new List<string>();

            // Editor模式下写在各项目自己的目录 多开时不会影响
            DateTime now = DateTime.Now;
            string strLogFile = string.Format("/{0:d2}-{1:d2}-{2:d2}.log", now.Year, now.Month, now.Day);
            string strOutPath = "";
            if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                strLogFile = string.Format("/{0:d2}-{1:d2}-{2:d2}-{3:d2}-{4:d2}-{5:d2}.log", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            }
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                strOutPath = Application.persistentDataPath + "/Log";
                if (!Directory.Exists(strOutPath))
                {
                    Directory.CreateDirectory(strOutPath);
                }
                strOutPath = strOutPath + strLogFile;
            }
            else
            {

                strOutPath = Application.dataPath + "/../Log";
                if (!Directory.Exists(strOutPath))
                {
                    Directory.CreateDirectory(strOutPath);
                }
                strOutPath = strOutPath + strLogFile;

            }

            MaxLogLevel = maxLevel;
            m_bWriteLogFile = bWriteLogFile;
            m_LogFile = new StreamWriter(strOutPath, true, System.Text.Encoding.UTF8);

            // 注册系统Log回调
            // if (m_bWriteLogFile)
            // {
            //     Application.logMessageReceived += HandleLog;
            // }
        }

        static public void Info(string strFormat, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string strFileLine = GetFileLine();

            LogOutput(LogLevel.LogLevel_Info, strFileLine, strFormat, args);
        }

        static public void Error(string strFormat, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string strFileLine = GetFileLine();
            LogOutput(LogLevel.LogLevel_Error, strFileLine, strFormat, args);
        }


        static public void Warning(string strFormat, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string strFileLine = GetFileLine();

            LogOutput(LogLevel.LogLevel_Warning, strFileLine, strFormat, args);
        }

        static public void Trace(string NetType,string strFormat, params object[] args)
        {

            if (!EnableLog)
            {
                return;
            }

            m_strGroupName = NetType;
            string strFileLine = GetFileLine();
            LogOutput(LogLevel.LogLevel_Trace, strFileLine, strFormat, args);
        }
        // 开启组显示
        public static void OpenGroup(string strGroupName)
        {
            if (!m_lstOpenGroup.Contains(strGroupName))
            {
                m_lstOpenGroup.Add(strGroupName);
            }
        }
        //-------------------------------------------------------------------------------------------------------
        // 关闭组显示
        public static void CloseGroup(string strGroupName)
        {
            if (m_lstOpenGroup.Contains(strGroupName))
            {
                m_lstOpenGroup.Remove(strGroupName);
            }
        }

        // 组Log 走Trace通道
        static public void LogGroup(string strGroup, string strFormat, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            if (string.IsNullOrEmpty(strGroup))
            {
                return;
            }

            if (m_lstOpenGroup == null)
            {
                return;
            }

            if (m_lstOpenGroup.Count <= 0)
            {
                return;
            }

            if (m_lstOpenGroup.Contains(strGroup))
            {
                m_strGroupName = strGroup;
                Group(strFormat, args);
            }
        }
        static void Group(string strFormat, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            string strFileLine = "";
            StackTrace insStackTrace = new StackTrace(true);
            if (insStackTrace != null)
            {
                StackFrame insStackFrame = insStackTrace.GetFrame(2);
                if (insStackFrame != null)
                {
                    strFileLine = String.Format("{0}({1})", insStackFrame.GetFileName(), insStackFrame.GetFileLineNumber());
                    strFileLine = GetScriptFileName(strFileLine);
                }
            }
            LogOutput(LogLevel.LogLevel_Group, strFileLine, strFormat, args);
        }
        private static string GetFileLine()
        {
            string strFileLine = "";
            StackTrace insStackTrace = new StackTrace(true);
            if (insStackTrace != null)
            {
                StackFrame insStackFrame = insStackTrace.GetFrame(2);
                if (insStackFrame != null)
                {
                    strFileLine = String.Format("{0}({1})", insStackFrame.GetFileName(), insStackFrame.GetFileLineNumber());
                    strFileLine = GetScriptFileName(strFileLine);
                }
            }
            return strFileLine;
        }

        private static string GetScriptFileName(string strFileLine)
        {
            if (strFileLine == null || strFileLine == "")
            {
                return "";
            }

            strFileLine = strFileLine.Replace("\\", "/");
            int pos = strFileLine.IndexOf("/Scripts/");
            string strResult = strFileLine.Substring(pos + 1, strFileLine.Length - pos - 1);
            return strResult;
        }

        static private void LogOutput(LogLevel level, string strFileLine, string strFormat, params object[] args)
        {
            if (!EnableLog)
            {
                return;
            }

            if (level > MaxLogLevel)
            {
                return;
            }

            // 添加时间参数
            DateTime now = DateTime.Now;
            string strTime = string.Format("{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            string strLog = "";
            string strOutLog = "";
            //保证json格式输出 格式化正确
            EB.Debug.PushToDebugBuffer(strFormat, args);
            if (args != null && args.Length > 0 && args[0] != null)
            {
                strOutLog = string.Format(strFormat, args);
            }
            else
            {
                strOutLog = strFormat;
            }
            switch (level)
            {
                case LogLevel.LogLevel_Info:
                    strLog = string.Format("{0}{1} ---<{2}>", "[Info]", strOutLog, strFileLine);
                    EB.Debug.Log(strLog, null);
                    break;
                case LogLevel.LogLevel_Trace:
                    strLog = string.Format("<color=magenta>[{0}]</color> {1} ---<{2}>", m_strGroupName, strOutLog, strFileLine);
                    EB.Debug.Log(strLog, null);
                    break;
                case LogLevel.LogLevel_Error:
                    strLog = string.Format("{0}{1} ---<{2}>", "[Error]", strOutLog, strFileLine);
                    EB.Debug.LogError(strLog, null);
                    break;
                case LogLevel.LogLevel_Group:
                    strLog = string.Format("<color=green>[G:{0}]</color> {1} ---<{2}>", m_strGroupName, strOutLog, strFileLine);
                    EB.Debug.Log(strLog, null);
                    break;
                case LogLevel.LogLevel_Warning:
                    strLog = string.Format("{0}{1} <{2}>", "[Warning]", strOutLog, strFileLine);
                    EB.Debug.LogWarning(strLog, null);
                    break;
            }
        }

        public static void HandleLog(string logString, string stackTrace, LogType type)
        {
            string strLog = logString;
            string strStackTrace = stackTrace;
            System.Text.StringBuilder strLogBuilder = new System.Text.StringBuilder();
            switch (type)
            {
                case LogType.Log:
                    {
                        PushLogToFile(strLog);
                        strLogBuilder.Append("[00FF00]");
                        strLogBuilder.Append(strLog);
                        PushLog(strLogBuilder.ToString());
                        break;
                    }
                case LogType.Error:
                    {
                        PushLogToFile(strLog);
                        strLogBuilder.Append("[FF0000]");
                        strLogBuilder.Append(strLog);
                        PushLog(strLogBuilder.ToString());
                        break;
                    }
                case LogType.Warning:
                    {
                        PushLogToFile(strLog);
                        strLogBuilder.Append("[FFFF00]");
                        strLogBuilder.Append(strLog);
                        PushLog(strLogBuilder.ToString());
                        break;
                    }
                case LogType.Exception:
                    {
                        strLogBuilder.Append("[Exception]");
                        strLogBuilder.Append(DateTime.Now.ToString());
                        strLogBuilder.Append(logString);
                        PushLogToFile(strLogBuilder.ToString());
                        strLog = "[FF0000]" + strLog;
                        strLogBuilder.Append("[FF0000]");
                        strLogBuilder.Append(strLog);
                        PushLog(strLogBuilder.ToString());

                        System.Text.StringBuilder strStackTraceBuilder = new System.Text.StringBuilder();
                        strStackTraceBuilder.Append("[StackTrace]");
                        strStackTraceBuilder.Append(DateTime.Now.ToString());
                        strStackTraceBuilder.Append(":");
                        strStackTraceBuilder.Append(stackTrace);
                        PushLogToFile(strStackTraceBuilder.ToString());
                        strStackTraceBuilder.Append("[FF0000]");
                        strStackTraceBuilder.Append(strStackTrace);
                        PushLog(strStackTraceBuilder.ToString());

                        // 异常回调
                        if (logException != null)
                        {
                            logException();
                        }
                    }
                    break;
            }
        }

        private static void PushLog(string str)
        {
            if (m_LogQueue == null)
            {
                m_LogQueue = new Queue<string>();
            }

            m_LogQueue.Enqueue(str);

            if (m_LogQueue.Count > 100)
            {
                m_LogQueue.Dequeue();
            }
        }

        public static void ReadFromStack(ref List<string> OutList)
        {
            if (m_LogQueue != null)
            {
                string[] Arr = m_LogQueue.ToArray();
                for (int i = 0; i < Arr.Length; ++i)
                {
                    OutList.Add(Arr[i]);
                }
            }
        }

        private static void PushLogToFile(string str)
        {
            if (m_LogFile != null && m_bWriteLogFile)
            {
                m_LogFile.WriteLine(str);
                m_LogFile.Flush();
            }
        }

        public static void Close()
        {
            //Application.logMessageReceived -= HandleLog;

            if (m_LogFile != null)
            {
                m_LogFile.Close();
            }
        }
    }
}
