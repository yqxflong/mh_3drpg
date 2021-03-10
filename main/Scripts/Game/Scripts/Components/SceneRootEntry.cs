using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

/// <summary>
/// 场景控件
/// </summary>
public class SceneRootEntry
{
    /// <summary>
    /// 开始加载回调事件
    /// </summary>
	public delegate void Begin();
    /// <summary>
    /// 失败回调事件
    /// </summary>
	public delegate void Failed();
    /// <summary>
    /// 完成回调事件
    /// </summary>
    /// <param name="entry"></param>
	public delegate void Finished(SceneRootEntry entry);
    /// <summary>
    /// 加载进度回调事件
    /// </summary>
    /// <param name="loaded">已经加载值</param>
    /// <param name="total">加载总值</param>
	public delegate void Loading(int loaded, int total);
    /// <summary>
    /// 放置资源的数据结构
    /// </summary>
	public class PlacedAsset
	{
        /// <summary>
        /// 资源名称
        /// </summary>
		public string name;
        /// <summary>
        /// 挂载的父级名称 
        /// </summary>
		public string parent;
        /// <summary>
        /// 本地位标
        /// </summary>
		public Vector3 localPosition;
        /// <summary>
        /// 本地大小
        /// </summary>
		public Vector3 localScale;
        /// <summary>
        /// 本地的旋转值
        /// </summary>
		public Quaternion localRotation;
        /// <summary>
        /// 是否为阻碍物
        /// </summary>
		public bool blocking;

		public PlacedAsset()
		{
		}
	}
    /// <summary>
    /// 当前的场景资源路径
    /// </summary>
	public string			m_Path;
    /// <summary>
    /// 当前场景的所有放置资源数据
    /// </summary>
	public PlacedAsset[]	m_LevelAssets;
    /// <summary>
    /// 当前加载场景资源的索引序号
    /// </summary>
	public int				m_LevelAssetIndex;
    /// <summary>
    /// 场景实体对象
    /// </summary>
	public GameObject		m_SceneRoot;
	bool					m_Loading = false;
    /// <summary>
    /// 开始加载回调事件
    /// </summary>
    public Begin			OnBegin;
    /// <summary>
    /// 失败回调事件
    /// </summary>
	public Failed			OnFailed;
    /// <summary>
    /// 完成回调事件
    /// </summary>
	public Finished			OnFinished;
    /// <summary>
    /// 加载进度回调事件
    /// </summary>
	public Loading			OnLoading;

	public SceneRootEntry(string path)
	{
		m_Path = path;
	}

	public bool IsLoaded()
	{
		return (m_SceneRoot != null);
	}
	
    /// <summary>
    /// 清除场景
    /// </summary>
	public void DestroyLevel()
	{
		if (m_SceneRoot != null)
		{
            //ClearLevelTextureAndLightMap(m_SceneRoot);
            
            GameObject.DestroyImmediate (m_SceneRoot);
			m_SceneRoot = null;

			// Unload asset bundles
			int level_asset_length = m_LevelAssets.Length;
			for(int i = 0; i < level_asset_length; ++i)
			{
                EB.Assets.UnloadAssetByName(m_LevelAssets[i].name, true);
            }
			m_LevelAssets = null;

			ClearLoading();
		}
	}

    /// <summary>
    /// 清除场景的贴图和光照贴图
    /// </summary>
    /// <param name="obj">场景对象</param>
    private void ClearLevelTextureAndLightMap(GameObject obj)
    {
        Renderer[] allRenderer = obj.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < allRenderer.Length; i++)
        {
            if (allRenderer[i].lightmapIndex >= 0)
            {
                allRenderer[i].material = null;
            }
        }
        
        for (int i=0;i< LightmapSettings.lightmaps.Length;i++)
        {
            Resources.UnloadAsset(LightmapSettings.lightmaps[i].lightmapColor);
        }
    }

	public void ShowLevel()
	{
		if (m_SceneRoot != null && !m_SceneRoot.activeSelf)
		{
			m_SceneRoot.SetActive(true);
		}
	}

    /// <summary>
    /// 隐藏场景对象
    /// </summary>
	public void HideLevel()
	{
		if (m_SceneRoot != null && m_SceneRoot.activeSelf)
		{
			m_SceneRoot.SetActive(false);
		}
	}

	public void SetParent(GameObject go)
	{
		if (m_SceneRoot != null && go != null)
		{
			m_SceneRoot.transform.parent = go.transform;
		}
	}

    public void SetZonesTag()
    {
        if (m_SceneRoot != null)
        {
            var zones = GameObject.Find("Zones");
            if (zones != null && !zones.CompareTag("Zones"))
            {
                zones.tag = "Zones";
            }
        }
    }

    public void SetMainLight()
    {
        if (m_SceneRoot != null)
        {
            var mainLight = m_SceneRoot.GetComponentInChildren<Light>();
            if (mainLight != null)
            {
                mainLight.cullingMask = 1;
                mainLight.shadowResolution = UnityEngine.Rendering.LightShadowResolution.FromQualitySettings;
            }
        }
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="levelName">场景名称（不带扩展名）</param>
    /// <param name="url">指定场景url</param>
    /// <param name="begin">开始加载事件</param>
    /// <param name="failed">失败事件</param>
    /// <param name="loading">进度事件</param>
    /// <param name="finish">完成事件</param>
    public void LoadOTALevelAsync(string levelName, string url, Begin begin, Failed failed, Loading loading, Finished finish)
	{
		if(m_Loading)
		{
			EB.Debug.LogLoadLevel("[SceneRootEntry]LoadOTALevelAsync: Loading is in progress.");
			return;
		}
		
		OnBegin = begin;
		OnFailed = failed;
		OnFinished = finish;
		OnLoading = loading;

		m_LevelAssetIndex = 0;
		m_Loading = true;

		OnBegin?.Invoke();

		if(m_SceneRoot != null)
		{
		    OnLoading?.Invoke(m_LevelAssets.Length, m_LevelAssets.Length);
			OnFinished?.Invoke(this);
			ClearLoading();
		}
        else
        {
            //加载相应的场景JSON配置信息
            GM.AssetManager.FetchSceneDescriptionFile(levelName, url, LoadedSceneDescription);
        }
	}

	public T GetComponentInChildren<T>() where T:MonoBehaviour
	{
		if(m_SceneRoot != null)
		{
			T[] components = m_SceneRoot.GetComponentsInChildren<T>(true);
			if(components.Length > 0)
			{
				return components[0];
			}
		}
		return null;
	}
	/// <summary>
    /// 清空函数
    /// </summary>
	void ClearLoading()
	{
		OnBegin = null;
		OnFailed = null;
		OnFinished = null;
		OnLoading = null;
		m_Loading = false;
	}

    /// <summary>
    /// 加载好场景资源配置信息
    /// </summary>
    /// <param name="level_name">场景名称</param>
    /// <param name="level_description">场景资源描述json</param>
    void LoadedSceneDescription(string level_name, string level_description)
	{
		if (EB.Util.IsJson(level_description))
		{
			m_LevelAssets = GM.JSON.ToObject<PlacedAsset[]>(level_description);

            if (m_LevelAssets != null && m_LevelAssets.Length > 0)
            {
                //实体化场景对象
				m_SceneRoot = new GameObject(level_name);
				// parallel load
				for (int i = 0; i < m_LevelAssets.Length; ++i)
				{
                    EB.Debug.LogLoadLevel("<color=#000000>开始</color>逐个_加载场景需要的指定资源StartLoad:{0} ,realtimeSinceStartup={1}, time = {2},Length={3},index={4}", m_LevelAssets[i].name, Time.realtimeSinceStartup, Time.time , m_LevelAssets.Length, i);
					EB.Assets.LoadAsyncAndInit<GameObject>(m_LevelAssets[i].name, ParallelDownloadResult, GameEngine.Instance.gameObject);
				}

				if (level_name != "MainLandView" && level_name != "CombatView")
				{
					GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.SceneLogic", "PreLoadMainHero");
				}
			}
			else
			{
				EB.Debug.Log(level_description);
				EB.Debug.LogError("加载场景发生错误_Level {0} has no data definition in its Json file!", level_name);
				HandleFaild ();
			}
		}
		else
		{
			EB.Debug.Log(level_description);
			EB.Debug.LogError("加载场景发生错误_Level {0}'s Json file is invalid!", level_name);
			
			HandleFaild ();
		}
	}

	void ParallelDownloadResult(string assetname, GameObject go, bool bSuccessed)
	{
		EB.Debug.LogLoadLevel("<color=#ffffff>结束</color>逐个_加载场景需要的指定资源_assetname = {0}, 是否成功:{1}, realtimeSinceStartup = {2} ,time={3}", assetname, bSuccessed, Time.realtimeSinceStartup,Time.time);

        if (bSuccessed && m_SceneRoot != null)
		{
			go.transform.localPosition = m_LevelAssets[m_LevelAssetIndex].localPosition;
			go.transform.localScale = m_LevelAssets[m_LevelAssetIndex].localScale;
			go.transform.localRotation = m_LevelAssets[m_LevelAssetIndex].localRotation;
			go.transform.parent = m_SceneRoot.transform;

			m_LevelAssetIndex++;

            if (OnLoading != null)
			{
				OnLoading(m_LevelAssetIndex, m_LevelAssets.Length);
			}

			if (m_LevelAssetIndex >= m_LevelAssets.Length)
			{
				HandleFinished();
            }
            
			if (go.name.IndexOf("Boss01Combat") >= 0 && !(bool)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.LTWorldBossDataManager", "Instance", "IsPlayWorldBossSpecialCam", true) && WorldBossCombatScene.Instance != null)
			{
                TimerManager.instance.AddTimer(150, 1, delegate
                {
                    WorldBossCombatScene.Instance.SetSceneStatus(2);//世界boss播放出场镜头的时候需要特殊设置场景状态
                });
            }
        }
		else
		{
			EB.Debug.LogError("逐个_加载场景需要的指定资源出错_Download a block scene item {0} failed!", assetname);
			if(bSuccessed)
			{
				GameObject.Destroy(go);
			}

			HandleFaild();
		}
	}
	
	void DownloadNextBlockItem(string assetname, GameObject go, bool bSuccessed)
	{
		EB.Debug.Log("[SceneRootEntry]DownloadNextBlockItem: assetname = {0}, bSuccessed = {1}, ts = {2}", assetname, bSuccessed, Time.realtimeSinceStartup);

		if (bSuccessed && m_SceneRoot != null&& go!=null)
		{
			go.transform.localPosition = m_LevelAssets[m_LevelAssetIndex].localPosition;
			go.transform.localScale = m_LevelAssets[m_LevelAssetIndex].localScale;
			go.transform.localRotation = m_LevelAssets[m_LevelAssetIndex].localRotation;
			go.transform.parent = m_SceneRoot.transform;
			
			m_LevelAssetIndex++;
			
			if(OnLoading != null)
			{
				OnLoading(m_LevelAssetIndex, m_LevelAssets.Length);
			}
			
			if (m_LevelAssetIndex < m_LevelAssets.Length)
			{
				EB.Debug.Log("[SceneRootEntry]LoadSceneAsync: name = {0}, ts = {1}", m_LevelAssets[m_LevelAssetIndex].name, Time.realtimeSinceStartup);
				EB.Assets.LoadAsyncAndInit<GameObject>(m_LevelAssets[m_LevelAssetIndex].name, DownloadNextBlockItem, GameEngine.Instance.gameObject);
			}
			else
			{
				HandleFinished();
			}
		}
		else
		{
			EB.Debug.LogError("Download a block scene item {0} failed!", assetname);
			HandleFaild ();
		}
	}
	
	void HandleFaild ()
	{
		if (OnFailed != null)
		{
			OnFailed();
		}
		
		ClearLoading();
		PostLoadCleanUp();
	}
	
	void HandleFinished ()
	{		
		OnFinished?.Invoke(this);
		ClearLoading();
		PostLoadCleanUp();
	}
	
	void PostLoadCleanUp()
	{
		//GM.AssetManager.UnloadUnusedAssets();
		//System.GC.Collect();
	}
}
