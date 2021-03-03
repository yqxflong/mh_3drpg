using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleHelper : MonoBehaviour 
{
	public bool m_blocking;
}

/// <summary>
/// 场景细节描述池管理器(单例)
/// </summary>
public class LevelDescriptionPoolManager
{
    private static LevelDescriptionPoolManager _instance;
    /// <summary>
    /// 场景细节描述
    /// </summary>
	public Dictionary<string, string> _levelDescriptionPool;
    public static LevelDescriptionPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LevelDescriptionPoolManager();
            }

            return _instance;
        }
    }

    private LevelDescriptionPoolManager()
    {
        _levelDescriptionPool = new Dictionary<string, string>();
    }
    /// <summary>
    /// 存储场景名称与JSON数据
    /// </summary>
    /// <param name="levelName">场景名称</param>
    /// <param name="descrJson">JSON描述数据</param>
	public void RegisterLevelDescription(string levelName, string descrJson)
    {
        _levelDescriptionPool.Add(levelName, descrJson);
    }
    /// <summary>
    /// 尝试获取这个场景相应的JSON配置数据
    /// </summary>
    /// <param name="levelName"></param>
    /// <param name="descrJson"></param>
    /// <returns></returns>
	public bool TryGetLevelDescription(string levelName, out string descrJson)
    {
        return _levelDescriptionPool.TryGetValue(levelName, out descrJson);
    }
    /// <summary>
    /// 是否存储过这个场景的JSON数据
    /// </summary>
    /// <param name="levelName">场景名称</param>
    /// <returns></returns>
	public bool IsLevelRegitered(string levelName)
    {
        return _levelDescriptionPool.ContainsKey(levelName);
    }
}
