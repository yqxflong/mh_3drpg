using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// 场景加载配置信息
/// </summary>
public class SceneLoadConfig
{
    /// <summary>
    /// 加载的数据数组
    /// </summary>
	public string[] Load;
    /// <summary>
    /// 隐藏的层数组
    /// </summary>
	public string[] HideLayers;
    /// <summary>
    /// 显示的场景名称
    /// </summary>
	public string Show;

	public uint GetHideLayerMask()
	{
		uint mask = 0xFFFFFFFF;
		for(int i = 0; i < HideLayers.Length; ++i)
		{
			int layerIndex = LayerMask.NameToLayer(HideLayers[i]);
			uint layerMask = ~(1U << layerIndex);
			mask = mask & layerMask;
		}
		return mask;
	}
}

/// <summary>
/// 场景加载管理器（静态类）
/// </summary>
public static class SceneLoadManager
{
    /// <summary>
    /// 当前场景加载资源键值对数据(string:资源路径,SceneRootEntry：数据控件)
    /// </summary>
	static Dictionary<string, SceneRootEntry> m_SceneRootDict = new Dictionary<string, SceneRootEntry>();
    /// <summary>
    /// 场景加载配置信息
    /// </summary>
	static Dictionary<string, SceneLoadConfig> m_SceneLoadConfigDict;
    /// <summary>
    /// 当前场景里边加载过的数据路径列表
    /// </summary>
	static List<string> m_ScenesLoaded = new List<string>();
    /// <summary>
    /// 是否加载完成
    /// </summary>
	static bool m_IsReady = false;
    /// <summary>
    /// 当前的场景名称
    /// </summary>
	static string m_CurrentSceneName = "";
    /// <summary>
    /// 当前场景状态名称
    /// </summary>
	static string m_CurrentStateName = "";

	public static bool IsReady
	{
		get
		{
			return m_IsReady;
		}
	}
    /// <summary>
    /// 是否正在加载配置表
    /// </summary>
	public static bool IsLoadingConfig { get; set; }

	public static string CurrentSceneName
	{
		get
		{
			return m_CurrentSceneName;
		}
	}

	public static string CurrentStateName
	{
		get
		{
			return m_CurrentStateName;
		}
	}

    /// <summary>
    /// 异步加载场景配置信息
    /// </summary>
	public static void LoadConfigAsync(System.Action<bool> fn=null)
	{
		m_IsReady = false;
		IsLoadingConfig = true;
		GameStateManager.Instance.StartCoroutine(LoadSceneLoadConfigAsyncImpl(fn));
	}
	
    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="stateName">状态名称</param>
    /// <param name="begin">开始加载事件</param>
    /// <param name="failed">失败事件</param>
    /// <param name="loading">进度事件</param>
    /// <param name="finish">完成事件</param>
	public static void LoadOTALevelGroupAsync(string stateName, SceneRootEntry.Begin begin, SceneRootEntry.Failed failed, SceneRootEntry.Loading loading, SceneRootEntry.Finished finish)
	{
        //要加载的资源数据列表
		List<string> toLoad = new List<string>();
        //要卸载的资源路径列表
		List<string> toUnload = new List<string>();
		
		SceneLoadConfig config = GetSceneLoadConfig(stateName);
		if(config != null && config.Load != null)
		{
			m_CurrentStateName = stateName;
			
			// find all the scenes to be loaded
			for(int i = 0; i < config.Load.Length; ++i)
			{
				toLoad.Add(config.Load[i]);
			}
			
			// find all the scenes to be unloaded
			for(int i = 0; i < m_ScenesLoaded.Count; ++i)
			{
				string sceneName = m_ScenesLoaded[i];
				
				bool found = false;
				for(int j = 0; j < toLoad.Count; ++j)
				{
					if(sceneName == toLoad[j])
					{
						found = true;
						break;
					}
				}
				
				if(!found)
				{
					toUnload.Add(sceneName);
				}
			}
			
			// unload all the scenes
			for(int i = 0; i < toUnload.Count; ++i)
			{
				DestroyLevel(toUnload[i]);
			}
			
			// hide all the scenes
			HideAll();
			
			// load all the scenes
			for(int i = 0; i < toLoad.Count; ++i)
			{
				if(config.Show == toLoad[i])
				{
					m_CurrentSceneName = config.Show;
					LoadOTALevelAsyncImpl(toLoad[i], begin, failed, loading, finish);
				}
				else
				{
					LoadOTALevelAsyncImpl(toLoad[i], null, null, null, BackLevelLoadFinished);
				}
			}
            
			m_ScenesLoaded = toLoad;

			//================================================================================
			string json = GM.JSON.ToJson(toLoad);
			Hashtable cache = Johny.HashtablePool.Claim();
			Hashtable content = Johny.HashtablePool.Claim();
			content["Content"] = json;
			cache["SceneLoad"] = content;
			DataLookupsCache.Instance.CacheData(cache);
			Johny.HashtablePool.Release(cache);cache=null;
			//================================================================================
		}
		else
		{
			EB.Debug.LogError("[SceneLoadManager]LoadOTALevelGroupAsync: CAN NOT find scene load config file for {0}.", stateName);
		}
        
        
    }
    /// <summary>
    /// 获取指定的场景加载配置信息
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
	public static SceneLoadConfig GetSceneLoadConfig(string stateName)
	{
		EB.Debug.Log("[SceneLoadManager]GetSceneLoadConfig: name = {0}", stateName);
		
		SceneLoadConfig config = null;
		if(m_SceneLoadConfigDict != null)
		{
			m_SceneLoadConfigDict.TryGetValue(stateName, out config);
		}
		
		return config;
	}
    /// <summary>
    /// 获取指定的场景控件
    /// </summary>
    /// <param name="sceneFileName">资源文件名称（资源路径）</param>
    /// <returns></returns>
	public static SceneRootEntry GetSceneRoot(string sceneFileName)
	{
		if(m_SceneRootDict != null && m_SceneRootDict.ContainsKey(sceneFileName))
		{
			return m_SceneRootDict[sceneFileName];
		}
		return null;
	}

    /// <summary>
    /// 清除所有场景
    /// </summary>
	public static void DestroyAllLevel()
	{
		for (int i = 0; i < m_ScenesLoaded.Count; ++i)
		{
			DestroyLevel(m_ScenesLoaded[i]);
		}

		m_ScenesLoaded.Clear();
	}

    /// <summary>
    /// 异步加载场景配置信息
    /// </summary>
    /// <returns></returns>
	static IEnumerator LoadSceneLoadConfigAsyncImpl(System.Action<bool> fn=null)
	{
		while (PerformanceManager.Instance.PerformanceInfo == null)
		{
			yield return null;
		}

		string url = GameEngine.Instance.OtaServer + "/SceneLoad_" + PerformanceManager.Instance.PerformanceInfo.CpuProfileName + ".json";
		if (GM.AssetUtils.LoadFromLocalFile(url))
		{
			EB.Debug.Log("[SceneLoadManager]LoadSceneLoadConfigAsyncImpl: {0}", url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
            if (request.isHttpError || request.isNetworkError)
			{
				EB.Debug.LogError("CAN NOT fetch scene load config file from {0}, error = {1}", url, request.error);
				IsLoadingConfig = false;
                if (fn != null) fn(m_IsReady);
				yield break;
			}
			else
			{
				m_SceneLoadConfigDict = GM.JSON.ToObject<Dictionary<string, SceneLoadConfig>>(request.downloadHandler .text);

				EB.Debug.Log("SCENE CONFIG: length = {0}", request.downloadHandler.text.Length);
				m_IsReady = true;
				IsLoadingConfig = false;
                if (fn != null) fn(m_IsReady);
				yield break;
			}
		}
		else
		{
			EB.Debug.Log("[SceneLoadManager]LoadSceneLoadConfigAsyncImpl: {0}", url);

			HTTP.Request r = new HTTP.Request("GET", url);
			r.acceptGzip = true;
			r.useCache = false;
			r.maximumRedirects = 2;
			yield return r.Send();

			if (r.exception != null)
			{
				EB.Debug.LogError("CAN NOT fetch scene load config file from {0}, error = {1}", url, r.exception.Message);
				IsLoadingConfig = false;
                if (fn != null) fn(m_IsReady);
				yield break;
			}
			else
			{
				if (r.response.status == 404)
				{
					EB.Debug.LogError("HTTP 404 error when gettings file from url: {0}", url);
					IsLoadingConfig = false;
                    if (fn != null) fn(m_IsReady);
					yield break;
				}
				else
				{
					m_SceneLoadConfigDict = GM.JSON.ToObject<Dictionary<string, SceneLoadConfig>>(r.response.Text);

					EB.Debug.Log("SCENE CONFIG: length = {0}", r.response.Text.Length);
				}
				m_IsReady = true;
				IsLoadingConfig = false;
                if (fn != null) fn(m_IsReady);
				yield break;
			}
		}
	}
    /// <summary>
    /// 清除指定场景
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <returns></returns>
	static bool DestroyLevel(string name)
	{
        EB.Debug.LogLoadLevel("<color=#ff0000>清除指定场景</color>_name:" + name);
        //
        SceneRootEntry entry = null;
		m_SceneRootDict.TryGetValue(name, out entry);
		if(entry != null)
		{
            if (name.Contains("MainLandView") )//|| name.Contains("CombatView"))
            {
                if(entry.m_SceneRoot!=null) entry.m_SceneRoot.CustomSetActive(false);
                return true;
            }
            entry.DestroyLevel();
			return true;
		}
		else
		{
			EB.Debug.LogError("[SceneLoadManager]DestroyLevel: There is no definition of level [{0}].  " ,name);
		}
		return false;
	}
    
    /// <summary>
    /// 异步加载指定的场景名称
    /// </summary>
    /// <param name="sceneFileName">场景资源名称（其实是路径带扩展名）</param>
    /// <param name="begin">开始加载事件</param>
    /// <param name="failed">失败事件</param>
    /// <param name="loading">进度事件</param>
    /// <param name="finish">完成事件</param>
    /// <returns></returns>
    static bool LoadOTALevelAsyncImpl(string sceneFileName, SceneRootEntry.Begin begin, SceneRootEntry.Failed failed, SceneRootEntry.Loading loading, SceneRootEntry.Finished finish)
	{
		EB.Debug.Log("LoadOTALevelAsyncImpl=====>");
		if(!m_SceneRootDict.ContainsKey(sceneFileName))
		{
			m_SceneRootDict.Add(sceneFileName, new SceneRootEntry(sceneFileName));
		}

		SceneRootEntry entry = null;
		m_SceneRootDict.TryGetValue(sceneFileName, out entry);
		if(entry != null)
		{
			int length = sceneFileName.IndexOf('.');
			length = (length >= 0 ? length : sceneFileName.Length);
            //场景名称
			string levelName = sceneFileName.Substring(0, length);
			string sceneUrl = GameEngine.Instance.OtaServer + "/" + entry.m_Path;
            entry.LoadOTALevelAsync(levelName, sceneUrl, begin, failed, loading, finish);
			return true;
		}

		EB.Debug.LogError("加载场景生成错误_[SceneLoadManager]LoadOTALevelAsync: There is no definition of level [{0}]. ", sceneFileName);
		return false;
	}

    /// <summary>
    /// 隐藏当前场景的所有实体
    /// </summary>
	static void HideAll()
	{
		Dictionary<string, SceneRootEntry>.Enumerator it = m_SceneRootDict.GetEnumerator();
		while(it.MoveNext())
		{
			KeyValuePair<string, SceneRootEntry> pair = it.Current;
			pair.Value.HideLevel();
		}
	}
    /// <summary>
    /// 已经加载过的场景,
    /// </summary>
    /// <param name="root"></param>
	static void BackLevelLoadFinished(SceneRootEntry root)
	{
		if(root == null)
		{
			return;
		}
		root.HideLevel();
	}
}