using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

/// <summary>
/// 游戏流程管理器
/// </summary>
public class GameFlowControlManager : MonoBehaviour
{
	public PlayMakerFSM m_StateMachine;
	public string		m_PrevStateName;

    private static GameFlowControlManager instance;
    public static GameFlowControlManager Instance
    {
        get
        {
            return instance;
        }
    }

    public void Awake()
    {
        instance = this;
    }

    public void OnDestroy()
    {
        instance = null;
    }

    public void SendEvent(string eventName)
	{
        EB.Debug.Log("<color=#ff0000>[{0}]通知游戏状态发生变化eventName:{1}</color>", Time.frameCount, eventName);
		if(m_StateMachine != null)
		{		
			m_PrevStateName = m_StateMachine.ActiveStateName;
			m_StateMachine.Fsm.BroadcastEvent(eventName);
		}
		else
		{
			EB.Debug.LogError("[GameFlowControlManager]SendEvent: The state machine failed to be initialized.");
		}
	}

    public static bool IsInView(string  viewName= "CombatView")
    {
        if (instance != null)
        {
            if (instance != null && instance.m_StateMachine != null)
            {
                if (string.Compare(instance.m_StateMachine.ActiveStateName, viewName) == 0)
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
