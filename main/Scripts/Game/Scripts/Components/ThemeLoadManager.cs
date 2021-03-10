using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景加载管理器
/// </summary>
public class ThemeLoadManager : MonoBehaviour
{
    /// <summary>
    /// 当前的场景名称
    /// </summary>
	public string m_CurrentLevelName = "";
    /// <summary>
    /// 当前的场景的路径
    /// </summary>
	public string m_CurrentLevelPath = "";
    /// <summary>
    /// 当前的场景控件
    /// </summary>
	SceneRootEntry m_RootEntry;
    /// <summary>
    /// 加载完成事件回调
    /// </summary>
	SceneRootEntry.Finished m_OnFinished;

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="levelName"></param>
    /// <param name="levelPath"></param>
    /// <param name="begin"></param>
    /// <param name="failed"></param>
    /// <param name="loading"></param>
    /// <param name="finish"></param>
    public void LoadOTALevelAsync(string levelName, string levelPath, SceneRootEntry.Begin begin, SceneRootEntry.Failed failed,SceneRootEntry.Loading loading, SceneRootEntry.Finished finish)
	{
        if (m_RootEntry != null && m_RootEntry.m_Path == levelPath)
        {
            m_CurrentLevelName = levelName;
            m_CurrentLevelPath = levelPath;
            m_OnFinished = finish;
            HandleFinished(m_RootEntry);
        }
		else
		{
			InitLoad();

			m_CurrentLevelName = levelName;
			m_CurrentLevelPath = levelPath;
			m_OnFinished = finish;
			m_RootEntry = new SceneRootEntry(levelPath);
			string sceneUrl = GameEngine.Instance.OtaServer + "/" + m_RootEntry.m_Path;
			m_RootEntry.LoadOTALevelAsync(levelName, sceneUrl, begin, failed, loading, HandleFinished);
		}
	}

	public void DestroyCurrentLevel()
	{
		if(m_RootEntry != null)
		{
			m_RootEntry.DestroyLevel();
		}
	}

	public SceneRootEntry GetSceneRoot()
	{
		return m_RootEntry;
	}

	public GameObject GetSceneRootObject()
	{
		if(m_RootEntry != null)
		{
			return m_RootEntry.m_SceneRoot;
		}
		return null;
	}

    /// <summary>
    /// 清除函数
    /// </summary>
	void InitLoad()
	{
		DestroyCurrentLevel();
		m_CurrentLevelName = "";
		m_CurrentLevelPath = "";
		m_OnFinished = null;
	}

	private static bool aa = false;

	void HandleFinished(SceneRootEntry entry)
	{
		// may be destroyed, 如果进了if则会卡76%
		if (Equals(null))
		{
			EB.Debug.LogError("[ThemeLoadManager].HandleFinished===> Equals(null)");
			entry.DestroyLevel();
			return;
		}

		if(entry.IsLoaded())
		{
			entry.SetParent(gameObject);
			entry.ShowLevel();
		}

		m_OnFinished?.Invoke(entry);
	}
}
