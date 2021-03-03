using UnityEngine;
using System.Collections;
using EB;
using System;

/// <summary>
/// 日志输出系统
/// </summary>
public class LogConfig : MonoBehaviour
{
    public string[] LogGroups = new string[] {};
    /// <summary>
    /// 日志输出开关
    /// </summary>
    public bool EnableLog = false;
    public bool WriteLog = true;
    
    // Use this for initialization
    void Start()
    {
        if (EnableLog)
        {
            EB.Log.OpenLog(EB.Log.LogLevel.LogLevel_Trace, WriteLog);
            SetLogUser();
        }
    }

    public void SetLogUser()
    {
        LogGroupData m_logdata = Resources.Load("LogConfig", typeof(LogGroupData)) as LogGroupData;
        if (m_logdata != null)
        {
            EB.Log.MaxLogLevel = (EB.Log.LogLevel)m_logdata.logLevel;
            try
            {
                foreach (var item in LogGroups)
                {
                    EB.Log.CloseGroup(item);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }

            for (int i = 0; i < m_logdata.LogUsers.Count; i++)
            {
                EB.Log.OpenGroup(m_logdata.LogUsers[i]);
            }
        }

    }
#if UNITY_EDITOR
	// Update is called once per frame
	void Update () {
        EB.Log.EnableLog = EnableLog;
        EB.Log.m_bWriteLogFile = WriteLog;

    }
#endif
}
