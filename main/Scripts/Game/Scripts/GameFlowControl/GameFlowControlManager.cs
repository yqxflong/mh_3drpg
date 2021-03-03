using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏流程管理器
/// </summary>
public class GameFlowControlManager : MonoBehaviour
{

    #region instance
    private static GameFlowControlManager instance;
    public static GameFlowControlManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion
	
    private string	m_EventName;
    public string ActiveStateName
    {
        get { return m_EventName.Replace("GoTo",""); }
    }

    #region for 赛跑活动
    //在赛跑中
    private bool m_IsInActivityRacing;
    public void SetInActivityRacing(bool inRacing)
    {
        m_IsInActivityRacing = inRacing;
    }
    public bool InActivityRacing
    {
        get
        {
            return m_IsInActivityRacing;
        }
    }
    #endregion


    public void Awake()
    {
        instance = this;
     }

    public void Start()
    {
        m_EventName = "GoToIdle";
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BaseFlowActionOnEnter", ActiveStateName);
    }

    public void OnDestroy()
    {
        instance = null;
    }
    private void OnDisable()
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BaseFlowActionOnExit",  ActiveStateName);
    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventName">GoToMainLandView</param>
    public void SendEvent(string eventName)
	{
        EB.Debug.Log("<color=#ff0000>[{0}]通知游戏状态发生变化eventName:{1}</color>", Time.frameCount, eventName);

        AsyncRun(() =>
        {
            GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BaseFlowActionOnExit", ActiveStateName);
            m_EventName = eventName;
        });
       
        AsyncRun(() =>
        {
            GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "BaseFlowActionOnEnter", eventName.Replace("GoTo",""));
        });
        
		// else
		// {
		// 	EB.Debug.LogError("[GameFlowControlManager]SendEvent: The state machine failed to be initialized.");
		// }
	}

    public void AsyncRun(Action action)
    {
        TimerManager.instance.AddFramer(1, 1, (t) => { action(); });			
    }

    public static bool IsInView(string  viewName= "CombatView")
    {
        if (instance != null)
        {
            if (instance != null && !string.IsNullOrEmpty(instance.m_EventName))
            {
                if (instance.m_EventName.EndsWith(viewName))
                {
                    return true;
                }
            }
            return false;
        }

#if UNITY_EDITOR
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path.Contains(viewName);
#else
		return false;
#endif
    }
}
