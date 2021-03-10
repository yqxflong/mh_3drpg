//GameStateDownload
//游戏状态--下载。（包括但不限于，加载本地bundles，下载远程bundles）

// #define LOG_GameStateDownload

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[GameState(eGameState.Download)]
public class GameStateDownload : GameState
{
    public static void LogWithTime(string msg)
    {
#if LOG_GameStateDownload
        DateTime now = DateTime.Now;
        EB.Debug.LogError($"{now.Hour}:{now.Minute}:{now.Second}=={msg}");
#endif
    }

#if !UNITY_EDITOR && USE_AOSHITANGSDK
    private void ShowWaitSDKInitScreen(bool shown)
    {
        var go = GameObject.Find("DownloadHudUI/WaitSDKInitScreen");
        if(go)
        {
            go.CustomSetActive(shown);
        }
    }
#endif


    public override IEnumerator Start(GameState oldState)
    {
        //debug服务器选择界面 debug screen view
#if DEBUG && !NO_DEBUG_SCREEN
        DebugSystem.DebugCameraClearNothing();
#endif

#if !UNITY_EDITOR && USE_AOSHITANGSDK
        EB.Sparx.AoshitangSDKManager.getInstance().AstSDKInit();
        yield return new WaitUntil(() => EB.Sparx.AoshitangSDKManager.getInstance().SDKInitSuccess);
        //ShowWaitSDKInitScreen(false);花屏问题，得等loading出来再隐藏
#endif

        //获取本地的玩家设置 load playersetting
        UserData.DeserializePrefs();

		//加载当前语言的基础包
		EB.Localizer.LoadCurrentLanguageBase(UserData.Locale);

        //加载证书 load certificates
        EB.Net.TcpClientFactory.LoadCertStore("Crypto/Certs");

        //加载hosts配置 load hosts
        EB.Net.DNS.LoadHosts("hosts");      

        //展示黑白提示界面，如果需要跳过请在协程内部最后加判断跳过 show warning loading
        yield return GameEngine.Instance.StartCoroutine(LTDownloadHudConroller.instance.ShowWarningScreen()); //GameEngine.Instance.StartCoroutine(UIStack.Instance.ShowWarningScreen());

        //显示普通loading界面（加载中。。。） show splash screen
        yield return GameEngine.Instance.StartCoroutine(LTDownloadHudConroller.instance.ShowSplashScreen());//GameEngine.Instance.StartCoroutine(ShowSplashScreen());

#if !UNITY_EDITOR && USE_AOSHITANGSDK
                ShowWaitSDKInitScreen(false);
#endif

        //网络检测是否联网 confirm network
        yield return LTDownloadHudConroller.instance.WaitForNetwork();//BlockForNetworkWithDialog();

#if USE_AOSHITANGSDK
        if (PlayerPrefs.GetInt("astfirstopen", -1) != 1)
        {
            EB.Sparx.AoshitangSDKManager.getInstance().UploadLog("firstOpen", delegate(bool scucess) {
                if (scucess)
                {
                    PlayerPrefs.SetInt("astfirstopen", 1);
                    PlayerPrefs.Save();
                }
            });
        }
        EB.Sparx.AoshitangSDKManager.getInstance().UploadLog("open", null);
#endif
        EB.Debug.Log("USE_AOSHITANGSDK Pass!!!!");

        //获取api跟ota服务器信息 fetch api sever and ota server
        yield return GameEngine.Instance.StartCoroutine(FetchServerList());

        EB.Debug.Log("FetchServerList Pass!!!!");

        if (GameEngine.Instance.ServerMaintenance)
        {
            //服务器维护 server maintenance
            LTDownloadHudConroller.instance.ShowTip(GameEngine.Instance.MaintenanceMessage);
            yield break;
        }

        EB.Debug.Log("ServerMaintenance Pass!!!!");

        #region 下载AB包
        //资源限制配置 resource limit
        GM.AssetManager.MaxDecompressQueueSize = 100;
        GM.AssetManager.MaxMemorySize = 200 * 1024 * 1024;
        GM.AssetManager.InitilizeThreadPool();

        //加载本地包体中的bundles
        yield return GameEngine.Instance.StartCoroutine(DownloadFromStreamingFolder());

        EB.Debug.Log("DownloadFromStreamingFolder Pass!!!!");

        //下载服务器的bundles
        yield return GameEngine.Instance.StartCoroutine(DowloadFromOtaServer());

        EB.Debug.Log("DowloadFromOtaServer Pass!!!!");

        #region 强制回收GC
        System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        #endregion

        EB.Debug.Log("GC Pass!!!!");

        //背景下载资源限制配置 background download resource limit
        GM.AssetManager.MaxDecompressQueueSize = 100;
        GM.AssetManager.MaxMemorySize = 50 * 1024 * 1024;
        GM.BundleDownloader.threadPool.Resize(2);

        //保存版本文件信息 save version info
        if (UserData.InitedVersion != EB.Version.GetFullVersion())
        {
            UserData.InitedVersion = EB.Version.GetFullVersion();
            UserData.SerializePrefs();
        }

        EB.Debug.Log("save version info Pass!!!!");
        #endregion



        #region 加载CommonText
        InitBundleTextData();
        yield return _waitUntilBundleTextData;
        #endregion

        EB.Debug.Log("InitBundleTextData Pass!!!!");

        //加载Bundle中的当前语言文本
        yield return LoadCurrentLanguageText();

        EB.Debug.Log("LoadCurrentLanguageText Pass!!!!");

#if !UNITY_EDITOR
        //加载bundle配置表信息 load datacaches
        yield return LoadAllDataCachesFromBundles();
#endif

        EB.Debug.Log("LoadAllDataCachesFromBundles Pass!!!!");

        #region InjectFix初始化 Inject Init
#if !UNITY_EDITOR
		yield return IFix.InjectPatchManager.Instance.LoadScriptPatch();
#endif
        #endregion

        EB.Debug.Log("InjectPatchManager Pass!!!!");

        #region ILR初始化 ILR Init
        EB.Coroutines.Run(HotfixILRManager.GetInstance().StartProcess());
        while (!HotfixILRManager.GetInstance().IsInit)
        {
            yield return null;
        }
        yield return null; //多等一帧 给依赖HotfixILRManager.GetInstance().IsInit判断初始化的协程能执行
        #endregion
        
        #region 初始化DynamicMonoILR
        EB.Debug.Log("DynamicMonoILR LTGameStateHofixController");
        var mono= GameStateManager.Instance.gameObject.AddComponent<DynamicMonoILR>();
        mono.hotfixClassPath = "Hotfix_LT.GameState.LTGameStateHofixController";
        mono.ILRObjInit();
        #endregion
        
        //初始化数据统计管理器
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.FusionTelemetry", "Initialize");



        #region 加载一坨不知道啥
        EB.Assets.LoadAsyncAndInit<UnityEngine.Object>("CharacterDependencies", null, GameEngine.Instance.gameObject);
        EB.Assets.LoadAsyncAndInit<UnityEngine.Object>("fx_aura04_Blueuv_02", null, GameEngine.Instance.gameObject);
        EB.Assets.LoadAsyncAndInit<UnityEngine.Object>("fx_dj_Reduv_fx02", null, GameEngine.Instance.gameObject);
        EB.Assets.LoadAsyncAndInit<UnityEngine.Object>("fx_Glo_0049", null, GameEngine.Instance.gameObject);
        EB.Assets.LoadAsyncAndInit<UnityEngine.Object>("fx_glwo_guangyun", null, GameEngine.Instance.gameObject);
        EB.Assets.LoadAsyncAndInit<UnityEngine.Object>("fx_m_bai_s", null, GameEngine.Instance.gameObject);
        #endregion

        EB.Debug.Log("GameDownload Pass!!!!");
    }

    #region Load TextData in Bundle
    private static bool _bundleTextDataFinished = false;
	private int _bundleTextDataInitCount = 0;
	private List<System.Action<System.Action<bool>>> _bundleTextDataInitList = new List<System.Action<System.Action<bool>>>()
	{
		GlobalBalanceData.Init,
		AnimationMetadataCatalog.Init,
		GlobalCameraData.Init,
		GlobalDialogueData.Init,
		GlobalStringData.Init,
		GlobalZoneData.Init,
		GlobalCameraMotionData.Init,
	};

	private WaitUntil _waitUntilBundleTextData = new WaitUntil(() =>
	{
		return _bundleTextDataFinished;
	});

	private void OnInitBundleTextData(bool succ)
	{
		_bundleTextDataInitCount += succ ? 1 : 0;
		if (_bundleTextDataInitCount == _bundleTextDataInitList.Count)
		{
			_bundleTextDataFinished = true;

		}
	}
	private void InitBundleTextData()
	{
		_bundleTextDataFinished = false;
		_bundleTextDataInitCount = 0;
		for (int i = 0; i < _bundleTextDataInitList.Count; i++)
		{
			_bundleTextDataInitList[i](OnInitBundleTextData);
		}
	}
    #endregion
    
	private IEnumerator FetchServerList()
	{
		while (!GameEngine.Instance.ApiCheckCompleted)
		{
			bool requestDone = false;

            // Fetch from server
#if !USE_AMAZON_API
            GameEngine.Instance.CheckApiRequest(delegate (EB.Sparx.Response response)
			{
				requestDone = true;
				GameEngine.Instance.ApiCheckCompleted = response.sucessful && !response.empty;
			}, GameEngine.Instance.ApiServerAddress);
#else
            yield return GameEngine.Instance.StartCoroutine(GameEngine.Instance.CheckApiRequest(delegate (bool response)
            {
                UnityEngine.Debug.Log("检查与解析API完成结果response："+ response);
                requestDone = true;
                GameEngine.Instance.ApiCheckCompleted = response;
            }, GameEngine.Instance.ApiServerAddress));
#endif
            while (!requestDone)
			{
				yield return null;
			}
            if (!GameEngine.Instance.ApiCheckCompleted)
			{
				yield return GameEngine.Instance.StartCoroutine(LTDownloadHudConroller.instance.ShowNetworkRetryDialog());
			}
		}
	}

    private WaitForSeconds _DownloadFromStreamingFolder_wait = new WaitForSeconds(1.0f);
    
    /// <summary>
    /// 加载本地的bundles
    /// </summary>
    private IEnumerator DownloadFromStreamingFolder()
	{
		if (EB.Version.GetFullVersion() == UserData.InitedVersion)
		{
			EB.Debug.Log("DownloadFromStreamingFolder: full version doesn't changed, skip local asset bundles");
			yield break;
		}

		// load & check local manifest
		DownloadLocalAssetBundleManifest();

		while (EB.Assets.IsDownloadingManifest)
		{
			yield return null;
		}
        
		if (!GM.AssetManager.IsReady)
		{
			EB.Debug.Log("DownloadFromStreamingFolder: skip local asset bundles");
			yield break;
		}

        // download each of the assets in order
        GM.AssetManager.DownloadLocalAssetBundles();
		long lastDownloadedSize = GM.AssetManager.GetDownloadedSize();
		while (EB.Assets.IsDownloadingOnLoad)
		{
            long currentDownloadedSize = GM.AssetManager.GetDownloadedSize();
            if (lastDownloadedSize != 0)
            {
                EB.Assets.DownloadSpeed = currentDownloadedSize - lastDownloadedSize;
                EB.Assets.DownloadSpeed = EB.Assets.DownloadSpeed < 0 ? 0 : EB.Assets.DownloadSpeed;
            }
			lastDownloadedSize = currentDownloadedSize;
            yield return _DownloadFromStreamingFolder_wait;
		}

		if (EB.Assets.DownloadedBundles != EB.Assets.ToDownloadBundles)
		{
			EB.Debug.LogError("DownloadFromStreamingFolder: Download asset bundles failed, downloaded {0} != total {1}", EB.Assets.DownloadedBundles, EB.Assets.ToDownloadBundles);
		}

		ClearStreamingBundlesCache();
    }

    /// <summary>
    /// 加载本地的bundleManifest
    /// </summary>
	private void DownloadLocalAssetBundleManifest()
	{
		EB.Assets.IsDownloadingManifest = true;

		string server = UserData.localAssetAddress + "/" + EB.Version.GetVersion().ToString();
		string url;
#if UNITY_EDITOR
		url = string.Format("{0}/{1}/", server, GetCurrentBuildPlatform().ToString());
#else
		url = string.Format("{0}/{1}/", server, GetRuntimeBuildPlatform().ToString());
#endif
        GM.AssetManager.SetRemoteBundleFileBaseUrl(url);

        url = string.Format("{0}BundleShipInfo.json", url);
        EB.Debug.Log("DownloadLocalAssetBundleManifest===>url: {0}", url);
        GM.AssetManager.GetRemoteBundlesInfoFile(url, DownloadLocalAssetBundleManifestResult, GameEngine.Instance.gameObject);
	}

    /// <summary>
    /// 从StreamingAsset中下载Bundle配置文件结果
    /// </summary>
    /// <param name="success"></param>
	private void DownloadLocalAssetBundleManifestResult(bool success)
	{
		if (!success)
		{
			EB.Debug.LogWarning("DownloadLocalAssetBundleManifestResult failed");
			EB.Assets.IsDownloadingManifest = false;
		}
		else
		{
			EB.Assets.IsDownloadingManifest = false;
		}
	}

	private void ClearStreamingBundlesCache()
	{
		EB.Debug.Log("GameStateDownload: Clear caches because full version changed");

		// version changed, execute clean operation
		string cachePath = System.IO.Path.Combine(Application.persistentDataPath, "Caches");
		if (System.IO.Directory.Exists(cachePath))
		{
			System.IO.Directory.Delete(cachePath, true);
		}

#if !UNITY_EDITOR
		//PlayerPrefs.DeleteAll();
#endif
	}

    /// <summary>
    /// 从OTA服务器下载Bundle
    /// </summary>
    /// <returns></returns>
	private IEnumerator DowloadFromOtaServer()
	{   
		if (GameEngine.Instance.OtaServerAddress == UserData.localAssetAddress && EB.Assets.DownloadedBundles > 0 && EB.Assets.DownloadedBundles == EB.Assets.ToDownloadBundles)
		{
			EB.Debug.Log("DowloadFromOtaServer: local asset bundles totally downloaded");
			yield break;
		}

		// download asset bundle manifest
		DownloadAssetBundleManifest();
		while (EB.Assets.IsDownloadingManifest)
		{
			yield return null;
		}

        // download each of the assets in order
        GM.AssetManager.DownloadRequiredAssetBundles(UserData.InitedVersion == EB.Version.GetFullVersion() ? -1 : 0);

        if (EB.Assets.DownloadedBundles != EB.Assets.ToDownloadBundles)
        {

            yield return LTDownloadHudConroller.instance.ConfirmCarrierDataNetwork();
		}

		long lastDownloadedSize = GM.AssetManager.GetDownloadedSize();
		while (EB.Assets.DownloadedBundles != EB.Assets.ToDownloadBundles)
		{
			while (EB.Assets.IsDownloadingOnLoad)
			{
				long currentDownloadedSize = GM.AssetManager.GetDownloadedSize();
                if (lastDownloadedSize != 0)
                {
                    EB.Assets.DownloadSpeed = currentDownloadedSize - lastDownloadedSize;
                    EB.Assets.DownloadSpeed = EB.Assets.DownloadSpeed < 0 ? 0 : EB.Assets.DownloadSpeed;
                }
				lastDownloadedSize = currentDownloadedSize;
                yield return _DownloadFromStreamingFolder_wait;
			}

			if (EB.Assets.DownloadedBundles != EB.Assets.ToDownloadBundles)
			{
				EB.Debug.LogWarning("DowloadFromOtaServer: Download asset bundles failed, downloaded {0} != total {1}", EB.Assets.DownloadedBundles, EB.Assets.ToDownloadBundles);
                yield return GameEngine.Instance.StartCoroutine(LTDownloadHudConroller.instance.ShowNetworkRetryDialog());
                GM.AssetManager.DownloadRequiredAssetBundles(UserData.InitedVersion == EB.Version.GetFullVersion() ? -1 : 0);
			}
		}
	}

    //乐变解压资源接口调用
    static void SetResExtracting(bool bOpen)
    {
        EB.Debug.Log("SetResExtracting——{0}", bOpen);
#if !UNITY_EDITOR && UNITY_ANDROID
        try
        {
            using (AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject jo = up.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass jc = new AndroidJavaClass("com.excelliance.lbsdk.LebianSdk");
                jc.CallStatic("setResExtracting", jo, bOpen);
            }
        }
        catch (Exception e)
        {
            EB.Debug.LogError("SetResExtracting Fail!——{0}",e);
        }
#endif
    }

    /// <summary>
    /// 从OTA服务器下载Bundle配置文件
    /// </summary>
	private void DownloadAssetBundleManifest()
	{
		EB.Assets.IsDownloadingManifest = true;

		string server = GameEngine.Instance.OtaServer;
		string url;
#if UNITY_EDITOR
		url = string.Format("{0}/{1}/", server, GetCurrentBuildPlatform().ToString());
#else
		url = string.Format("{0}/{1}/", server, GetRuntimeBuildPlatform().ToString());
#endif
        GM.AssetManager.SetRemoteBundleFileBaseUrl(url);

        url = string.Format("{0}BundleShipInfo.json", url);
        EB.Debug.Log("DownloadAssetBundleManifest===>url: {0}", url);
		GM.AssetManager.GetRemoteBundlesInfoFile(url, DownloadAssetBundleManifestResult, GameEngine.Instance.gameObject);
	}

    /// <summary>
    /// 从OTA服务器下载Bundle配置文件结果
    /// </summary>
    /// <param name="success"></param>
	private void DownloadAssetBundleManifestResult(bool success)
	{
		if (!success)
		{
			EB.Debug.LogError("DownloadAssetBundleManifest failed");
            LTDownloadHudConroller.instance.ShowNetworkRetryDialog(DownloadAssetBundleManifest);
		}
		else
		{
			EB.Assets.IsDownloadingManifest = false;
		}
	}
    
    /// <summary>
    /// 加载Bundle中的当前语言文本（目的是用最新的多语言文本）
    /// </summary>
    /// <returns></returns>
	private IEnumerator LoadCurrentLanguageText()
	{				
        string l = EB.Localizer.GetSparxLanguageCode(EB.Localizer.Current);
        string langTextPath = $"Assets/Bundles/Languages/{l}";
        yield return EB.Assets.LoadAsync(langTextPath, typeof(TextAsset), (o)=>
        {
            TextAsset ta = o as TextAsset;
            EB.Localizer.LoadStrings(ta);
        });
	}

    private List<string> cacheList = new List<string>()
    {
        "Combat",
        "Economy",
        "Task",
        "Resource",
        "Scene",
        "Character",
        "Guide",
        "Event",
        "Vip",
        "Alliance",
        "Shop",
        "NewGameConfig",
        "Gacha",
    };

    private IEnumerator LoadAllDataCachesFromBundles()
    {
        string dir = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, "Caches");
        if (Directory.Exists(dir))
        {
            EB.Debug.Log("GameStateDownLoad.LoadAllDataCachesFromBundles.LoadCache: cache file exists {0}", dir);
            yield break;
        }

        yield return EB.Assets.LoadBundle("Caches", delegate (string bundleName, AssetBundle bundle, bool succ)
        {
            if (!succ)
            {
                EB.Debug.LogWarning("GameStateDownLoad.LoadAllDataCachesFromBundles: create bundle {0} failed", bundleName);
                return;
            }

            if (!Directory.Exists(dir))
            {
                if (Directory.CreateDirectory(dir) == null)
                {
                    EB.Debug.LogError("GameStateDownLoad.LoadAllDataCachesFromBundles: create directory {0} failed ", dir);
                    return;
                }
            }

            for (int i = 0; i < cacheList.Count; i++)
            {
                string name = cacheList[i];
                TextAsset asset = bundle.LoadAsset<TextAsset>(name);
                string path = Path.Combine(dir, name);
                using (FileStream fs = File.Create(path))
                {
                    fs.Write(asset.bytes, 0, asset.bytes.Length);
                    fs.Close();
                    fs.Dispose();
#if UNITY_IPHONE
                UnityEngine.iOS.Device.SetNoBackupFlag(path);
#endif
                }
            }
            EB.Assets.UnloadBundle(bundleName);
        }, GameEngine.Instance.gameObject);
    }
}
