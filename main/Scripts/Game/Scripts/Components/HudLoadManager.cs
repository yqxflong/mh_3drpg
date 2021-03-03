using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

/// <summary>
/// 预置体的加载信息
/// </summary>
public class HudLoadConfig
{
	public string[] Load;
	public string[] Show;
}

public class HudLoadResult
{
	public string m_Name;
	public bool m_Result;

	//--------------------------------------------------------------------------------------------------
	//  Function: HudLoadResult
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	public HudLoadResult(string name, bool result)
	{
		m_Name = name;
		m_Result = result;
	}
}

/// <summary>
/// 预置体的加载管理器
/// </summary>
public static class HudLoadManager
{
	public delegate void HudLoadComplete(bool NoError, string[] Show);

	static Dictionary<string, HudRootEntry> m_HudRootDict = new Dictionary<string, HudRootEntry>();
    /// <summary>
    /// 当前所有的状态与相应的加载信息
    /// </summary>
	static Dictionary<string, HudLoadConfig> m_HudLoadConfigDict;
	static bool m_IsReady = false;

	//--------------------------------------------------------------------------------------------------
	//  Function: IsReady
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	public static bool IsReady
	{
		get
		{
			return m_IsReady;
		}
	}

	public static bool Completed
	{
		get; private set;
	}

	public static bool IsLoadingConfig { get; set; }

	/// <summary>
    /// 通过状态名称获取相应的加载信息
    /// </summary>
    /// <param name="state_name"></param>
    /// <returns></returns>
	static HudLoadConfig GetHudLoadConfig(string state_name)
	{
		HudLoadConfig config = null;
		
		if(m_HudLoadConfigDict != null)
		{
			m_HudLoadConfigDict.TryGetValue(state_name, out config);
		}
		
		return config;
	}

	//--------------------------------------------------------------------------------------------------
	//  Function: GetHudRoot
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	public static HudRootEntry GetHudRoot(string name)
	{
		if(m_HudRootDict != null && m_HudRootDict.ContainsKey(name))
		{
			return m_HudRootDict[name];
		}
		
		return null;
	}
	
	/// <summary>
    /// 加载预置的所有信息数据
    /// </summary>
	public static void LoadConfigAsync(System.Action<bool> fn=null)
	{
		m_IsReady = false;
		IsLoadingConfig = true;
		GameStateManager.Instance.StartCoroutine(LoadConfigAsyncImpl(fn));
	}

	/// <summary>
    /// 协程加载预置体信息数据
    /// </summary>
    /// <returns></returns>
	static IEnumerator LoadConfigAsyncImpl(System.Action<bool> fn)
	{
		while (PerformanceManager.Instance.PerformanceInfo == null)
		{
			yield return null;
		}

		string url = GameEngine.Instance.OtaServer + "/HudLoad_" + PerformanceManager.Instance.PerformanceInfo.CpuProfileName + ".json";
		if (GM.AssetUtils.LoadFromLocalFile(url))
		{
			EB.Debug.Log("[HudLoadManager]LoadConfigAsyncImpl: {0}", url);
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();
			if (request.isHttpError || request.isNetworkError)
			{
				EB.Debug.LogError("CAN NOT fetch hud load config file from {0}, error = {1}", url, request.error);
				IsLoadingConfig = false;
				if(fn!=null)fn(m_IsReady);
				yield break;
			}
			else
			{
				m_HudLoadConfigDict = GM.JSON.ToObject<Dictionary<string, HudLoadConfig>>(request.downloadHandler.text);

				EB.Debug.Log("HUD CONFIG: length = {0}", request.downloadHandler.text.Length);
				m_IsReady = true;
				IsLoadingConfig = false;
                if (fn != null) fn(m_IsReady);
				yield break;
			}
		}
		else
		{
			EB.Debug.Log("[HudLoadManager]LoadConfigAsyncImpl: {0}" , url);

			HTTP.Request r = new HTTP.Request("GET", url);
			r.acceptGzip = true;
			r.useCache = false;
			r.maximumRedirects = 2;
			yield return r.Send();

			if (r.exception != null)
			{
				EB.Debug.LogError("Can not fetch hud load config file from {0}, error = {1}", url, r.exception.Message);
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
					m_HudLoadConfigDict = GM.JSON.ToObject<Dictionary<string, HudLoadConfig>>(r.response.Text);

					EB.Debug.Log("HUD CONFIG: length = {0}", r.response.Text.Length);
				}
				m_IsReady = true;
				IsLoadingConfig = false;
                if (fn != null) fn(m_IsReady);
				yield break;
			}
		}
	}

	// for recording download results
	static List<string> m_HudToLoad = new List<string>();
	static int m_LoadIndex = 0;
	static HudLoadComplete m_Complete;
	static HudLoadConfig m_Config;
	
	//--------------------------------------------------------------------------------------------------
	//  Function: DestroyHud
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	static bool DestroyHud (string hudName)
	{
		HudRootEntry entry = null;
		m_HudRootDict.TryGetValue(hudName, out entry);
		if(entry != null)
		{
			entry.DestroyHud();
			return true;
		}
		else
		{
			EB.Debug.LogError("[HudLoadManager]DestroyHud: There is no definition of hud [" + hudName + "]. ");
		}
		return false;
	}

	//--------------------------------------------------------------------------------------------------
	//  Function: DestroyAllHud
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	public static void DestroyAllHud()
	{
		Dictionary<string, HudRootEntry>.Enumerator it = m_HudRootDict.GetEnumerator();
		while (it.MoveNext())
		{
			KeyValuePair<string, HudRootEntry> pair = it.Current;
			pair.Value.DestroyHud();
		}
		m_HudRootDict.Clear();
		m_HudToLoad.Clear();
		Clear();
	}
    
    /// <summary>
    /// 异步加载预置体
    /// </summary>
    /// <param name="stateName">状态名称</param>
    /// <param name="finish">加载完成回调</param>
	public static void LoadHudAsync(string stateName, HudLoadComplete finish)
	{
		Completed = false;

		HideAll();

		HudLoadConfig config = GetHudLoadConfig(stateName);
		if(config != null)
		{
			Clear();

			// 
			List<string> toLoad = new List<string>();
			List<string> toUnload = new List<string>();

			// find what to load
			for(int i = 0; i < config.Load.Length; ++i)
			{
				toLoad.Add(config.Load[i]);
			}

			// find what to unload
			for(int i = 0; i < m_HudToLoad.Count; ++i)
			{
				string hudName = m_HudToLoad[i];
				
				bool found = false;
				for(int j = 0; j < toLoad.Count; ++j)
				{
					if(hudName == toLoad[j])
					{
						found = true;
						break;
					}
				}
				
				if(!found)
				{
					toUnload.Add(hudName);
				}
			}

			// destroy all hud that is not needed
			for(int i = 0; i < toUnload.Count; ++i)
			{
				DestroyHud(toUnload[i]);
			}

			// sync load all hud that is needed
			m_HudToLoad = toLoad;
			m_Config = config;
			m_Complete = finish;

			EB.Debug.Log("[HudLoadManager]LoadHudAsync: Length = {0}", m_HudToLoad.Count.ToString());

			m_LoadIndex = 0;
			//// serial load
			//LoadHudAsyncImpl(m_HudToLoad[m_LoadIndex], OnLoadFailed, OnLoadFinish);

			//parallel load
			for (int i = 0; i < m_HudToLoad.Count; ++i)
			{
				LoadHudAsyncImpl(m_HudToLoad[i], OnParallelLoadFailed, OnParallelLoadFinish);
			}
		}
		else
		{
			EB.Debug.LogError("[HudLoadManager]LoadHudAsync: There is no config for {0}", stateName);
		}
	}
	
	//--------------------------------------------------------------------------------------------------
	//  Function: OnLoadFailed
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	static void OnLoadFailed(string HudName)
	{
		if (m_Complete != null)
		{
			Completed = true;
			m_Complete(false, m_Config.Show);
		}
		Clear();
	}
	
	//--------------------------------------------------------------------------------------------------
	//  Function: OnLoadFinish
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	static void OnLoadFinish(string HudName)
	{
		EB.Debug.Log("[HudLoadManager]OnLoadFinish: HudName = {0}", HudName);

		++m_LoadIndex;

		if(m_LoadIndex < m_HudToLoad.Count)
		{
			LoadHudAsyncImpl(m_HudToLoad[m_LoadIndex], OnLoadFailed, OnLoadFinish);
		}
		else
		{
			if (m_Complete != null)
			{
				Completed = true;
				m_Complete(true, m_Config.Show);
			}
			Clear();
		}
	}

	static void OnParallelLoadFailed(string HudName)
	{
		if (m_Complete != null)
		{
			Completed = true;
			m_Complete(false, m_Config.Show);
		}
		Clear();
	}

	static void OnParallelLoadFinish(string HudName)
	{
		EB.Debug.Log("[HudLoadManager]OnParallelLoadFinish: HudName = {0}", HudName);

		if (Completed || !m_HudToLoad.Contains(HudName))
		{
			EB.Debug.LogWarning("[HudLoadManager]OnParallelLoadFinish: status changed, skip HudName = {0}", HudName);
			return;
		}

		++m_LoadIndex;

		if (m_LoadIndex >= m_HudToLoad.Count)
		{
			if (m_Complete != null)
			{
				Completed = true;
				m_Complete(true, m_Config.Show);
			}
			Clear();
		}
	}

	//--------------------------------------------------------------------------------------------------
	//  Function: Clear
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	static void Clear()
	{
		m_Config = null;
		m_Complete = null;
	}
	
	//--------------------------------------------------------------------------------------------------
	//  Function: LoadHudAsyncImpl
	//  
	//  Description: 
	//--------------------------------------------------------------------------------------------------
	static bool LoadHudAsyncImpl(string assetName, HudRootEntry.Failed failed, HudRootEntry.Finished finish)
	{
		if(!m_HudRootDict.ContainsKey(assetName))
		{
			m_HudRootDict.Add(assetName, new HudRootEntry(assetName));
		}

		HudRootEntry entry = null;
		m_HudRootDict.TryGetValue(assetName, out entry);
		if(entry != null && entry.OnFinished != finish && entry.OnFailed != failed)
		{
			entry.LoadHudAsync(assetName, failed, finish);
			return true;
		}

		return false;
	}

	static void HideAll()
	{
		Dictionary<string, HudRootEntry>.Enumerator it = m_HudRootDict.GetEnumerator();
		while(it.MoveNext())
		{
			KeyValuePair<string, HudRootEntry> pair = it.Current;
			pair.Value.Hide();
		}
	}
}
