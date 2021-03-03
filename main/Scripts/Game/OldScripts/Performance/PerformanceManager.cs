///////////////////////////////////////////////////////////////////////
//
//  PerformanceManager.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

/// <summary>
/// 游戏性能配置管理器
/// </summary>
public class PerformanceManager
{
	private static PerformanceManager _Instance = null;
	public static PerformanceManager Instance 
	{ 
		get 
		{
			if (_Instance == null)
			{
				_Instance = new PerformanceManager();
			}
			return _Instance;
		}
	}
	/// <summary>
    /// 当前的性能配置信息
    /// </summary>
	public PerformanceInfo PerformanceInfo;

	public string CurrentScene
	{
		get; private set;
	}

	public PerformanceInfo.EnvironmentInfo CurrentEnvironmentInfo
	{
		get
        {
            if(PerformanceInfo == null) // by pj 修正当PerformanceInfo对象为初始化带来的报错
            {
                return null;
            }
            else
            {
                return PerformanceInfo.EnvironmentInfoForScene(CurrentScene);
            }
        }
	}

	private PerformanceManager()
	{
		PerformanceInfo = null;
	}
	
	public void DataLoaded(object data)
	{
		string Path = string.Format("performance.{0}", EB.Dot.String("profile.cpu", data, "High"));
		Dictionary<string, object> wrapper = new Dictionary<string, object>();
		wrapper.Add(Path, data);
		if (!SparxHub .Instance.PerformanceManager.wrapper.ContainsKey(Path)) SparxHub.Instance.PerformanceManager.wrapper.Add(Path, data);
		DataLookupsCache.Instance.CacheData(wrapper);

		PerformanceInfo = new PerformanceInfo(data);
		DebugSystem.Log("Loaded CPU profile " + PerformanceInfo.CpuProfileName + ". Setting values for state = " + GameStateManager.Instance.State, "PerformanceManager");
	}
	
	public int GetPlatform()
	{
#if UNITY_ANDROID
		return (int)PerformanceInfo.ePLATFORM.Android;
#else 
		return (int)PerformanceInfo.ePLATFORM.iOS;
#endif
	}

    /// <summary>
    /// 设置相应的场景性能配置信息
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <returns></returns>
	public PerformanceInfo.EnvironmentInfo UseScene(string sceneName)
	{
		PerformanceInfo.EnvironmentInfo info = PerformanceInfo.EnvironmentInfoForScene(sceneName);
		if (info != null)
		{
			CurrentScene = sceneName;
		}
		return info;
	}

	public void OnGameStateTransition(eGameState newGameState)
	{

	}

	private string EnumArrayToString<T>(T[] array)
	{
		string res = "";
		for(int i = 0; i < array.Length; ++i)
		{
			res += array[i].ToString();
			if (i < array.Length - 1) { res += ", "; }
		}
		return res;
	}
}

