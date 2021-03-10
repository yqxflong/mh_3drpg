using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using EB.Sparx;
using ILRuntime.CLR.Method;
using Debug = EB.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 游戏引擎
/// </summary>
public class GameEngine : MonoBehaviour
{
    public enum eOutlineColor
    {
        None,
        EnemyInRange,
        EnemyOutOfRange,
        Friendly,
        Building
    }

    public enum ResolutionType
    {
        Default = 0,
        SD,
        HD
    }

    [System.Serializable]
    public class OutlineShaderSettings
    {
        public float OutlineWidth;
        public Color OutlineColor;
        public eOutlineColor OutlineType;
    }

    [ResourcesReferenceAttribute(typeof(Material))]
    public ResourcesReference outlineTargetMaterialRef;

    private Material outlineTargetMaterialInstance = null;
    public Material outlineTargetMaterial
    {
        get
        {
            if (outlineTargetMaterialInstance == null)
            {
                outlineTargetMaterialInstance = new Material(outlineTargetMaterialRef.Value as Material);
            }

            return outlineTargetMaterialInstance;
        }
    }

    private bool mIsRunFromEnterGameBtn = false;
    /// <summary>
    /// 是否从进入游戏按钮过来的
    /// </summary>
    public bool IsRunFromEnterGameBtn
    {
        get { return mIsRunFromEnterGameBtn; }
        set { mIsRunFromEnterGameBtn = value; }
    }
    /// <summary>
    /// 主场景加载完成
    /// </summary>
    private bool mIsTimeToRootScene = false;
    public bool IsTimeToRootScene
    {
        get { return mIsTimeToRootScene; }
        set
        {
            mIsTimeToRootScene = value;
            UIRect.IsTimeToRootScene = value;
        }
    }

    public List<OutlineShaderSettings> OutlineSettings;

    public int TargetFrameRate = 30;
    public bool IsResetUserData = false;
    public bool IsFTE = true;
    public bool ApiCheckCompleted = false;
    public string ApiServerAddress;
    public string OtaServerAddress;

    public string AuthServerAddress;

    public static int HDMinimumDPI = 200;
    public static int HDMinimumResolution = 960;

    private const float ApiUpdateInterval = 3600.0f;
    private float _nextApiUpdate;
    public bool NeedsUpdate { get; set; }
    public string StoreUrl { get; set; }
    public bool ServerMaintenance { get; set; }
    public string MaintenanceMessage { get; set; }
    public bool IsVerifyState { get; set; }
    public static bool ASTIsHaveCommond = false;
    public string LoginBGPath = "ReplaceableRes/Login_BG.png";
    public string BrandPath = "ReplaceableRes/Login_Brand.png";
    public Dictionary<string, Texture2D> TextureDic = new Dictionary<string, Texture2D>();

    public IEnumerator SetStreamingAssetsBG(string path)
    {
        string url;
        if (!ILRDefine.UNITY_EDITOR && ILRDefine.UNITY_ANDROID)
        {
            url = string.Format("{0}/{1}", Application.streamingAssetsPath, path);
        }
        else
        {

            url = string.Format("file://{0}/{1}", Application.streamingAssetsPath, path);
        }
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            Texture2D localTexture = DownloadHandlerTexture.GetContent(www);
            
            if (TextureDic.ContainsKey(path))
            {
                if (!TextureDic[path].Equals(localTexture))
                {
                    TextureDic[path] = localTexture;
                }
            }
            else
            {
                TextureDic.Add(path, localTexture);
            }
        }
        else
        {
            Debug.Log(www.error);
        }
        www.Dispose();
    }

    public bool SupportsStencil { get; set; }

    public StaticAtlas LTStaticAtlas = new StaticAtlas();

    // prevent copy
    private GameEngine() { }

    private static GameEngine instance = null;
    private static bool shuttingDown = false;

    private int _defaultLayer;
    public int defaultLayer
    {
        get { return _defaultLayer; }
    }

    private int _transparentFXLayer;
    public int transparentFXLayer
    {
        get { return _transparentFXLayer; }
    }

    private int _transparentUI3DLayer;
    public int transparentUI3DLayer
    {
        get { return _transparentUI3DLayer; }
    }


    private int _groundLayer;
    public int groundLayer
    {
        get { return _groundLayer; }
    }

    private int _obstacleLayer;
    public int obstacleLayer
    {
        get { return _obstacleLayer; }
    }

    private int _playerLayer;
    public int playerLayer
    {
        get { return _playerLayer; }
    }

    private int _enemyLayer;
    public int enemyLayer
    {
        get { return _enemyLayer; }
    }

    private int _interactableLayer;
    public int interactableLayer
    {
        get { return _interactableLayer; }
    }

    private int _interactiveLayer;
    public int interactiveLayer
    {
        get { return _interactiveLayer; }
    }

    private int _uiLayer;
    public int uiLayer
    {
        get { return _uiLayer; }
    }

    private int _ui3dLayer;
    public int ui3dLayer
    {
        get { return _ui3dLayer; }
    }

    private LoginListenerFusion _loginListener;
    public LoginListenerFusion LoginListener
    {
        get
        {
            if (_loginListener == null)
            {
                _loginListener = new LoginListenerFusion();
            }
            return _loginListener;
        }
    }

    private GameListenerFusion _gameListener;
    public GameListenerFusion GameListener
    {
        get
        {
            if (_gameListener == null)
            {
                _gameListener = new GameListenerFusion();
            }
            return _gameListener;
        }
    }

    private WalletListenerFusion _walletListener;
    public WalletListenerFusion WalletListener
    {
        get
        {
            if (_walletListener == null)
            {
                _walletListener = new WalletListenerFusion();
            }
            return _walletListener;
        }
    }

    private EB.Sparx.SocketDataSet _sparxSocketData = null;
    public EB.Sparx.SocketDataSet SparxSocketData
    {
        get { return _sparxSocketData; }
        set { _sparxSocketData = value; }
    }

    private GameObject _outlineTarget;
    public GameObject outlineTarget
    {
        get { return _outlineTarget; }
        // no set
    }

    private eOutlineColor _outlineColor = eOutlineColor.None;
    public eOutlineColor OutlineColor
    {
        get { return _outlineColor; }
    }

    public static GameEngine Instance
    {
        get
        {
            if (instance == null)
            {
                DebugSystem.Log("GameEngine haven't been created yet. It needs to be the first GameObject create. It is at Initializer scene", "GameEngine", LogType.Error);
            }
            return instance;
        }
    }

    public static bool IsAvailable
    {
        get { return instance != null; }
    }

    public static bool IsShuttingDown
    {
        get { return shuttingDown; }
    }
    /// <summary>
    /// 外部服务器url路径
    /// </summary>
	public string OtaServer
    {
        get { return OtaServerAddress + "/" + EB.Version.GetVersion(); }
    }

    public static bool UseHDAssets
    {
        get
        {
#if UNITY_EDITOR
            // Editor Forced-HD detection
            ResolutionType resolution = (ResolutionType)EditorPrefs.GetInt("ResolutionType");
            if (resolution != ResolutionType.Default)
            {
                return (resolution == ResolutionType.HD);
            }
#endif

#if UNITY_IPHONE
			// Low memory retina devices
			if (Misc.IsLowMemoryRetina())
			{
				return false;
			}
#endif

            // DPI Detection
            if (Screen.dpi > 0)
            {
                return (Screen.dpi > HDMinimumDPI);
            }

            // Resolution Detection
            // Fallback in case DPI detection fails
            int maxDimension = Mathf.Max(Screen.width, Screen.height);
            return (maxDimension >= HDMinimumResolution);
        }
    }

    private void GlobalizationSetting() {
#if UNITY_IOS && !UNITY_EDITOR
        var culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();

        //switch (culture.Name) {
        //    case "th-TH":
        //        culture.DateTimeFormat.Calendar = new System.Globalization.ThaiBuddhistCalendar();
        //        break;
        //}

        // https://github.com/xamarin/Xamarin.Forms/issues/4037
        if (Environment.CurrentDirectory == "_never_POSSIBLE_") {
            new System.Globalization.ChineseLunisolarCalendar();
            new System.Globalization.HebrewCalendar();
            new System.Globalization.HijriCalendar();
            new System.Globalization.JapaneseCalendar();
            new System.Globalization.JapaneseLunisolarCalendar();
            new System.Globalization.KoreanCalendar();
            new System.Globalization.KoreanLunisolarCalendar();
            new System.Globalization.PersianCalendar();
            new System.Globalization.TaiwanCalendar();
            new System.Globalization.TaiwanLunisolarCalendar();
            new System.Globalization.ThaiBuddhistCalendar();
            new System.Globalization.UmAlQuraCalendar();
        }

        culture.NumberFormat = System.Globalization.NumberFormatInfo.InvariantInfo;
        System.Globalization.CultureInfo.CurrentCulture = culture;
#endif
    }

    void Awake()
    {
        GlobalizationSetting();

        if (instance != null)
        {
            EB.Debug.LogError("There should be only one GameEngine object");
            return;
        }

        EB.Debug.Log("GameEngine Awake");

#if !DEBUG && USE_BUGLY
        BuglyManager.InitBugly();
#endif

#if USE_AIHELP
        AIHelp.AIHelpManager.Init(gameObject.name);
#endif

#if USE_APPSFLYER
        Appsflyer.AppsflyManager.setIsDebug(ILRDefine.DEBUG);
        Appsflyer.AppsflyManager.initSDK(gameObject);
        Appsflyer.AppsflyManager.startSDK();
#endif

#if USE_AOSHITANGSDK
        ASTIsHaveCommond = PlayerPrefs.GetInt("ASTIsHaveCommond", -1) == 1;
#endif

        instance = this;
        DontDestroyOnLoad(gameObject);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 30/60 for iOS
        Application.targetFrameRate = TargetFrameRate;
        // Don't Sync
        //QualitySettings.vSyncCount = 0;

#if DEBUG
        UnityEngine.Profiling.Profiler.maxNumberOfSamplesPerFrame = 8 * 1024 * 1024;
#endif

        // initialize all layers
        _defaultLayer = LayerMask.NameToLayer("Default");
        _transparentFXLayer = LayerMask.NameToLayer("TransparentFX");
        _transparentUI3DLayer = LayerMask.NameToLayer("TransparentUI3D");
        _uiLayer = LayerMask.NameToLayer("UI");
        _groundLayer = LayerMask.NameToLayer("Ground");
        _obstacleLayer = LayerMask.NameToLayer("Obstacle");
        _playerLayer = LayerMask.NameToLayer("Player");
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        _interactableLayer = LayerMask.NameToLayer("Interactable");
        _interactiveLayer = LayerMask.NameToLayer("Interactive");
        _ui3dLayer = LayerMask.NameToLayer("UI3D");

        if (!ApiCheckCompleted)
        {
            ApiServerAddress = UserData.defaultApiServerAddress;
            OtaServerAddress = UserData.localAssetAddress;
            AuthServerAddress = UserData.defaultAuthServerAddress;
        }
        else
        {
            ApiServerAddress = string.IsNullOrEmpty(ApiServerAddress) ? UserData.defaultApiServerAddress : ApiServerAddress;
            OtaServerAddress = string.IsNullOrEmpty(OtaServerAddress) ? UserData.localAssetAddress : OtaServerAddress;
            AuthServerAddress = string.IsNullOrEmpty(AuthServerAddress) ? UserData.defaultAuthServerAddress : AuthServerAddress;
        }

#if DISABLE_LOTTERY
		IsFTE = false;
#endif

        InitializeSubSystems();

        DebugSystem.Log("Device reports DPI of " + Screen.dpi, "GameEngine");
        DebugSystem.Log("Device reports resolution of " + Screen.width + "x" + Screen.height, "GameEngine");
        DebugSystem.Log("Device is using " + (UseHDAssets ? "HD" : "SD") + " assets!", "GameEngine");

        SupportsStencil = CheckSupportsStencil();
        DebugSystem.Log("Device " + (SupportsStencil ? "" : "not") + " support stencil!", "GameEngine");

        EventManager.instance.AddListener<GameStateChangedEvent>(OnGameStateChanged);

        _nextApiUpdate = Time.time + ApiUpdateInterval;

        LoadingSpinner.Init();
    }

    void OnDestroy()
    {
        EB.Debug.Log("GameEngine OnDestroy");

        LoadingSpinner.UnInit();

        instance = null;
    }
    #region(Appsflyer回调)
#if USE_APPSFLYER
    public void onConversionDataSuccess(string conversionData)
    {
        Appsflyer.AppsflyManager.AFLog("onConversionDataSuccess", conversionData);
        Dictionary<string, object> conversionDataDictionary = Appsflyer.AppsflyManager.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        Appsflyer.AppsflyManager.AFLog("onConversionDataFail", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        Appsflyer.AppsflyManager.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = Appsflyer.AppsflyManager.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        Appsflyer.AppsflyManager.AFLog("onAppOpenAttributionFailure", error);
    }
#endif
    #endregion
    #region (AIHelp回调)
#if USE_AIHELP
    //初始化回调
    public void OnAIHelpInitialized(string str)
    {
        AIHelp.AIHelpManager.IsAihelpInitialize = true;
        Debug.Log ("AIhelpInitCallback is called str:" + str);
    }
    //未读消息回调
    public void OnAIHelpMessageArrived(string str)
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.LTGameSettingManager", "RefreshHwCsRedPoint");
        Debug.Log("OnMessageArrived is called str:" + str);
        var jnode = Johny.JSONNode.Parse(str);
        if (jnode!=null&&jnode.IsObject)
        {
            var data = jnode["data"]?["cs_message_count"];
            if(data != null && data.IsNumber){
                AIHelp.AIHelpManager.IshaveUnreadMessage = (int)data > 0;
            }
        }

       
    }

#endif
    #endregion

    public void ShowTipCall(string tip)
    {
        EB.Debug.LogError("OnLoginTip: login tip = {0}", tip);
        if (tip.Equals("ID_SPARX_LOGIN_FAIL_MORE"))
        {
            ShowLoginFailDialog();
        }
        else
        {
            //ToDo:调热更
            //MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(tip));
            GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.MessageTemplateManager", "ShowMessageFromILR", "FloatingText", EB.Localizer.GetString(tip), false);
        }
    }

    protected void ShowLoginFailDialog()
    {
        UIStack.Instance.GetDialog("Error",EB.Localizer.GetString("ID_SPARX_LOGIN_FAIL_MORE"), delegate (eUIDialogueButtons button, UIDialogeOption opt)
        {
#if !UNITY_EDITOR
			Application.Quit();
#else
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        });
    }

    void InitializeSubSystems()
    {
#if DEBUG && !NO_DEBUG_SCREEN
        DebugSystem.Initialize();
#endif

        gameObject.AddComponent<TexturePoolManager>();

        GameStateManager.Initialize();

        LoginListener.LogoffEvent += OnLogOff;
    }

    void OnLogOff(string error)
    {
        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.FusionTelemetry", "PostLogout");
        if (GameStateManager.Instance.State >= eGameState.Login)
        {
            Relogin(error);
        }
    }

#region Relogin
    private void RealRelogin()
    {
		//Debug.LogWarning("RealRelogin!");
        TimerManager.instance.RemoveAllTimer();
        GlobalUtils.CallStaticHotfixEx("Hotfix_LT.ILRTimerManager", "instance", "RemoveAllTimer");
        //UIStack.Instance.ShowLogoutScreen();
		GameStateManager.Instance.SetGameState<GameStateLogin>();
        //UIStack.Instance.ForceHideLoadingScreen();
        //LoadingSpinner.Destroy();
    }

    ///断开连接后，重返登陆
    public void Relogin(string error)
    {
        EB.Debug.Log("GameEngine.Relogin=====>error:{0}",error);
        UIStack.Instance.ExitStack(true);
        UIHierarchyHelper.ReloadAllHierarchyHelpers();

        if (string.IsNullOrEmpty(error))
        {
            UIStack.Instance.ShowLoadingScreen(delegate ()
            {
                RealRelogin();
            }, true, false);
        }
        else
        {
            if (ILRDefine.USE_XINKUAISDK && error.Equals(EB.Localizer.GetString("ID_SPARX_ERROR_DUPLICATE_LOGIN")))
            {
                UIStack.Instance.ExitStack(true);
                if (ILRDefine.UNITY_ANDROID)
                {
                    UIStack.Instance.ShowLoadingScreen(delegate ()
                    {
                        ShowFatalError(error, delegate ()
                        {
                            SparxHub.Instance.Logout();
                        });
                    });
                }
                return;//新快sdk走自己的的顶号登录
            }
            UIStack.Instance.ShowLoadingScreen(delegate ()
            {
                ShowFatalError(error, delegate ()
                {
                    if (error.Equals(EB.Localizer.GetString("ID_SPARX_ERROR_DUPLICATE_LOGIN")))
                    {
                        //被人登了，清凭证
                        SparxHub.Instance.Disconnect(true);
                    }
                    else
                    {
                        RealRelogin();
                    }
                });
            }, true, false);
        }

        EB.Debug.Log("<=====GameEngine.Relogin");
    }
#endregion

    void OnApplicationQuit()
    {
        EB.Debug.Log("GameEngine.OnApplicationQuit");

        shuttingDown = true;

#if DEBUG
        DebugSystem.Destroy();
#endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // first called after Awake
        EB.Debug.Log("GameEngine.OnApplicationPause: status = {0}", pauseStatus);

        GameStateManager.Instance.OnPause(pauseStatus);
        if (GameStateManager.Instance.State >= eGameState.Login)
        {
            SparxHub.Instance.OnPause(pauseStatus);
        }
    }

    void OnApplicationFocus(bool focusStatus)
    {
        // first called after Awake
        //EB.Debug.Log("GameEngine.OnApplicationFocus: status = {0}", focusStatus);

        GameStateManager.Instance.OnFocus(focusStatus);
        if (GameStateManager.Instance.State >= eGameState.Login)
        {
            SparxHub.Instance.OnFocus(focusStatus);
        }
        bool isSceneLoop = false;
        if (HotfixILRManager.GetInstance().IsInit)
        {
            isSceneLoop = (bool)GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.SceneLogic", "isSceneLoop");
        }
        if (isSceneLoop/*isSceneLoop*//*SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop*/)
        {
            if (PerformanceManager.Instance.CurrentEnvironmentInfo.slowDevice)
            {
                //setDesignContentScale(1334,750);
            }
        }
    }

    private void OnGUI()
    {
#if USE_DEBUG && !UNITY_EDITOR
        GUI.skin.label.fontSize = 36;
        Color currentColor = GUI.color;

        if (_fps > 30.0f)
        {
            GUI.color = Color.green;
        }
        else if (_fps > 10.0f)
        {
            GUI.color = Color.yellow;
        }
        else
        {
            GUI.color = Color.red;
        }
        GUILayout.Label("", GUILayout.Width(600));
        GUILayout.Label("FPS: " + _fps, GUILayout.Width(600));
        GUI.color = currentColor;
#endif
    }

    void Update()
    {
        if (Time.time > _nextApiUpdate)
        {
            _nextApiUpdate = Time.time + ApiUpdateInterval;
            CheckApi();
        }
#region call ILRUpdateManager
        // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "Update");
        ILR.HotfixManager.ILRUtils.CallUpdateMono();
#endregion
        TimerManager.instance.Update();
        UpdateFPS();

#if UNITY_EDITOR
        Update_QACommand();
#endif
    }
    
    void FixedUpdate()
    {
#region call ILRUpdateManager
        // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "FixedUpdate");
        ILR.HotfixManager.ILRUtils.CallFixedUpdateMono();
#endregion
    }

    void LateUpdate()
    {
#region call ILRUpdateManager
        // GlobalUtils.CallStaticHotfix("Hotfix_LT.ILRUpdateManager", "LateUpdate");
        ILR.HotfixManager.ILRUtils.CallLateUpdateMono();
#endregion
        TimerManager.instance.LateUpdate();
    }

    public float fps_threshold = 10.0f;
    private const float UpdateInterval = 0.5f;
    private float _fps = 0.0f;
    private float _accum = 0; // FPS accumulated over the interval
    private int _frames = 0; // Frames drawn over the interval
    private float _timeleft; // Left time for current interval
    private float _thresholdTime;
    private bool _lockThreshold = false;
    private void UpdateFPS()
    {
        _timeleft -= Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;
        if (_timeleft <= 0.0)
        {
            _fps = _accum / _frames;
            _timeleft = UpdateInterval;
            _accum = 0.0F;
            _frames = 0;
        }

        if (!_lockThreshold)
        {
            if (_fps < fps_threshold)
            {
                _thresholdTime += Time.deltaTime;
                if (_thresholdTime > 5)
                {
                    _thresholdTime = 0;
                    _lockThreshold = true;//要弹出窗口调画质拉
                    if (QualitySettings.GetQualityLevel() != 2)
                    {
                        //ToDo:
                        //MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_GameEngine_12427"), delegate (int r)
                        //{
                        //    if (r == 0)
                        //    {
                        //        GlobalUtils.SetGameQualityLevel(QualitySettings.GetQualityLevel() + 1);
                        //    }
                        //});
                        GlobalUtils.CallStaticHotfix("Hotfix_LT.UI.MessageTemplateManager", "ShowMessageFromILR", "MessageDialogue_2", EB.Localizer.GetString("ID_codefont_in_GameEngine_12427"), true);
                    }
                    
                }
            }
            else
            {
                _thresholdTime = 0;
            }
        }
        else
        {
            _thresholdTime += Time.deltaTime;
            if (_thresholdTime > 300)
            {
                _lockThreshold = false;
                _thresholdTime = 0;
            }
        }
    }

    public void SetOutlineTarget(GameObject inTarget, eOutlineColor color)
    {
        if (_outlineTarget == inTarget)
        {
            return;
        }
        if (_outlineTarget)
        {
            // remove the outline material from each renderer
            Renderer[] renderers = _outlineTarget.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                if (renderers[i] == null)
                {
                    continue;
                }
                Material[] currentMaterials = renderers[i].sharedMaterials;
                if (currentMaterials.Length < 1 || (currentMaterials[currentMaterials.Length - 1] != null && !currentMaterials[currentMaterials.Length - 1].name.Contains(outlineTargetMaterial.name)))
                {
                    continue;
                }
                Material[] newMaterails = new Material[currentMaterials.Length - 1];
                for (int m = 0; m < newMaterails.Length; ++m)
                {
                    newMaterails[m] = currentMaterials[m];
                }
                renderers[i].sharedMaterials = newMaterails;
            }
        }
        _outlineTarget = inTarget;
        if (_outlineTarget)
        {
            // add outline material to each renderer
            Renderer[] renderers = _outlineTarget.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                if (renderers[i] == null || renderers[i].gameObject.layer == transparentFXLayer)
                {
                    continue;
                }
                Material[] currentMaterials = renderers[i].sharedMaterials;
                if (currentMaterials.Length > 0 && (currentMaterials[currentMaterials.Length - 1] != null && currentMaterials[currentMaterials.Length - 1].name.Contains(outlineTargetMaterial.name)))
                {
                    continue;
                }
                Material[] newMaterails = new Material[currentMaterials.Length + 1];
                for (int m = 0; m < currentMaterials.Length; ++m)
                {
                    newMaterails[m] = currentMaterials[m];
                }
                newMaterails[currentMaterials.Length] = outlineTargetMaterial;
                renderers[i].sharedMaterials = newMaterails;

                SetOutlineColor(color);
            }
        }
    }

    public void SetOutlineColor(eOutlineColor color)
    {
        if (_outlineColor == color)
            return;

        OutlineShaderSettings settings = OutlineSettings[0];
        for (int i = 0; i < OutlineSettings.Count; i++)
        {
            if (OutlineSettings[i].OutlineType == color)
            {
                settings = OutlineSettings[i];
                break;
            }
        }

        outlineTargetMaterial.SetColor("_OutlineColor", settings.OutlineColor);
        outlineTargetMaterial.SetFloat("_Outline", settings.OutlineWidth);
        _outlineColor = color;
    }

    public void SetHideColorTarget(GameObject inTarget)
    {
        //if (inTarget != null && SupportsStencil)
        //{
        //	Material hideColorMaterial = EB.Assets.Load<Material>("Materials/HideColor");

        //          int hideColorRenderQueue = hideColorMaterial.renderQueue;
        //          //int geomeryRenderQueue = 2000;
        //          SkinnedMeshRenderer[] renderers = inTarget.GetComponentsInChildren<SkinnedMeshRenderer>();
        //          for (int i = 0; i < renderers.Length; ++i)
        //          {
        //              if (renderers[i] == null || renderers[i].gameObject.layer == transparentFXLayer || renderers[i].gameObject.layer == ui3dLayer || renderers[i].gameObject.layer == uiLayer)
        //              {
        //                  continue;
        //              }
        //              //by pj ��Ϊ����mesh�Ķ���ʹ�����·�ʽ�ᶪʧuv �����޸�֮
        //              Material[] currentMaterials = renderers[i].materials;
        //              if (currentMaterials.Length > 0 && (currentMaterials[currentMaterials.Length - 1] == hideColorMaterial || currentMaterials[currentMaterials.Length - 1].name.Contains(hideColorMaterial.name)))
        //              {
        //                  continue;
        //              }
        //              Material[] newMaterails = new Material[currentMaterials.Length + 1];
        //              for (int m = 0; m < currentMaterials.Length; ++m)
        //              {
        //                  newMaterails[m] = currentMaterials[m];
        //                  newMaterails[m].renderQueue = hideColorRenderQueue;//i <= 1 ? geomeryRenderQueue : hideColorRenderQueue;
        //              }
        //              newMaterails[currentMaterials.Length] = hideColorMaterial;
        //              renderers[i].materials = newMaterails;

        //              /*GameObject hideRendererGO= GameObject.Instantiate(renderers[i].gameObject);
        //              hideRendererGO.transform.parent = renderers[i].transform.parent;
        //              hideRendererGO.transform.position = renderers[i].transform.position;
        //              SkinnedMeshRenderer hideRenderer =  hideRendererGO.GetComponent<SkinnedMeshRenderer>();
        //              Material[] newMaterails = new Material[hideRenderer.materials.Length];
        //              for (int m = 0; m < newMaterails.Length; ++m)
        //              {
        //                  newMaterails[m] = hideColorMaterial;
        //              }
        //              hideRenderer.materials = newMaterails;*/
        //          }
        //      }
    }

    private void OnGameStateChanged(GameStateChangedEvent evt)
    {
        // reset default shader settings between game state changes
        GameUtils.SetShaderDefaults();
    }

    public void ShowRequiredUpdate(string storeUrl)
    {
        UICustomeDialogueOption option = new UICustomeDialogueOption();
        option.title = EB.Localizer.GetString("ID_SPARX_APP_UPDATE_TITLE");
        option.body = EB.Localizer.GetString("ID_SPARX_APP_UPDATE_MESSAGE");
        option.accept = UICustomeDialogueOption.ButtonGo;
        option.minHeight = UICustomeDialogueOption.ThreeLineHeight;
        if (!string.IsNullOrEmpty(storeUrl))
        {
            EB.Uri uri = new EB.Uri(storeUrl);
            Hashtable qs = EB.QueryString.Parse(uri.GetComponent(EB.Uri.Component.Query, string.Empty));
            qs.Add("platform", EB.Sparx.Device.Platform);
            qs.Add("source", EB.Sparx.Device.Source);
            qs.Add("child", EB.Sparx.Device.ChildSource);
            uri.SetComponent(EB.Uri.Component.Query, EB.QueryString.Stringify(qs));

            option.onClick = delegate (eUIDialogueButtons button, UIDialogeOption opt)
            {
                Application.OpenURL(opt.param as string);
            };

            option.param = uri.ToString();
            option.buttons = eUIDialogueButtons.Accept;
        }
        else
        {
            option.buttons = eUIDialogueButtons.None;
        }

        UIStack.Instance.ShowDialog(option);
    }

    public void ShowMaintenance(string msg)
    {
        UIStack.Instance.GetDialog("Tips", msg,null);
    }

    public void ShowFatalError(string msg, System.Action callback)
    {
        UIStack.Instance.GetDialog("Error", msg, delegate (eUIDialogueButtons button, UIDialogeOption opt)
        {
            callback();
        });
    }
#if !USE_AMAZON_API
    public void CheckApiRequest(System.Action<EB.Sparx.Response> callback, string requestFrom = null)
    {
        EB.Sparx.EndPoint endPoint;
        EB.Sparx.Request request;

        //if (!string.IsNullOrEmpty(requestFrom))
        //{
        //    endPoint = EB.Sparx.EndPointFactory.Create(requestFrom, new EB.Sparx.EndPointOptions());
        //}
        //else if (SparxHub.Instance != null)
        //{
        //    endPoint = SparxHub.Instance.ApiEndPoint;
        //}
        //else
        //{
        //    EB.Debug.LogWarning("CheckApiRequest: url set not");
        //    return;
        //}

        endPoint = EB.Sparx.EndPointFactory.Create(UserData.defaultApiServerAddress, new EB.Sparx.EndPointOptions());

        string _url = string.Format("/public/api/{0}/{1}", EB.Sparx.Device.Platform, EB.Version.GetVersion());
        request = endPoint.Get(_url);
        UnityEngine.Debug.Log("-----------> 一定要日志输出_ API连接的url:" + _url);
        request.numRetries = 2;
        endPoint.Service(request, delegate (EB.Sparx.Response response)
        {
            if (response.sucessful)
            {
                string source = string.Format("{0}{1}.", EB.Sparx.Device.Source, EB.Sparx.Device.ChildSource);
                string locale = EB.Localizer.GetSparxLanguageCode(UserData.Locale);
                ApiServerAddress = EB.Dot.String(source + "api.urls." + locale, response.hashtable, null);
                if (string.IsNullOrEmpty(ApiServerAddress))
                {
                    ApiServerAddress = EB.Dot.String(source + "api.url", response.hashtable, null);
                }

                AuthServerAddress = EB.Dot.String(source + "auth.url", response.hashtable, null);

                NeedsUpdate = string.IsNullOrEmpty(ApiServerAddress);

                StoreUrl = EB.Dot.String(source + "api.storeUrls." + locale, response.hashtable, null);
                if (string.IsNullOrEmpty(StoreUrl))
                {
                    StoreUrl = EB.Dot.String(source + "api.storeUrl", response.hashtable, null);
                }

                OtaServerAddress = EB.Dot.String(source + "ota.urls." + locale, response.hashtable, null);
                if (string.IsNullOrEmpty(OtaServerAddress))
                {
                    OtaServerAddress = EB.Dot.String(source + "ota.url", response.hashtable, null);
                }

                ServerMaintenance = EB.Dot.Bool(source + "maintenance.state", response.hashtable, false);
                MaintenanceMessage = EB.Dot.String(source + "maintenance.messages." + locale, response.hashtable, null);
                if (string.IsNullOrEmpty(MaintenanceMessage))
                {
                    MaintenanceMessage = EB.Dot.String(source + "maintenance.message", response.hashtable, null);
                }

                IsVerifyState = EB.Dot.Bool(source + "verify", response.hashtable, false);
            }
            else
            {
                EB.Debug.LogWarning("CheckApiRequest failed, error: {0}", response.error);
            }

            callback(response);
        });
    }
#else
    public IEnumerator CheckApiRequest(System.Action<bool> callback, string requestFrom = null)
    {
        EB.Sparx.EndPoint endPoint;

        if (!string.IsNullOrEmpty(requestFrom))
        {
            endPoint = EB.Sparx.EndPointFactory.Create(requestFrom, new EB.Sparx.EndPointOptions());
        }
        else if (SparxHub.Instance != null)
        {
            endPoint = SparxHub.Instance.ApiEndPoint;
        }
        else
        {
            EB.Debug.LogWarning("CheckApiRequest: url set not");
            if (callback != null)
            {
                callback(false);
            }
            yield break;
        }

        string _url = string.Format("{0}/public/api/{1}/{2}?time={3}", endPoint.Url, EB.Sparx.Device.Platform, EB.Version.GetVersion(),System.DateTime.Now.Ticks);
        UnityEngine.Debug.Log("-----------> API连接的url:"+ _url);
        UnityWebRequest uwr = UnityWebRequest.Get(_url);
        yield return uwr.SendWebRequest();
        if (uwr.isHttpError || uwr.isNetworkError)
        {
            callback(false);
            yield break;
        }
        else
        {
            string response = uwr.downloadHandler.text;
            UnityEngine.Debug.Log("-----------> 返回结果response:"+ response);
            Hashtable toc1 = (Hashtable)EB.JSON.Parse(response);
            if (toc1 == null)
            {
                UnityEngine.Debug.Log("-----------> 转换成JSON为空了");
            }

            var toc = toc1["result"];

            string source = string.Format("{0}{1}.", EB.Sparx.Device.Source, EB.Sparx.Device.ChildSource);
            string locale = EB.Localizer.GetSparxLanguageCode(UserData.Locale);
            ApiServerAddress = EB.Dot.String(source + "api.urls." + locale, toc, null);
            if (string.IsNullOrEmpty(ApiServerAddress))
            {
                ApiServerAddress = EB.Dot.String(source + "api.url", toc, null);
            }

            AuthServerAddress = EB.Dot.String(source + "auth.url", toc, null);
            NeedsUpdate = string.IsNullOrEmpty(ApiServerAddress);
            StoreUrl = EB.Dot.String(source + "api.storeUrls." + locale, toc, null);
            if (string.IsNullOrEmpty(StoreUrl))
            {
                StoreUrl = EB.Dot.String(source + "api.storeUrl", toc, null);
            }

            OtaServerAddress = EB.Dot.String(source + "ota.urls." + locale, toc, null);
            if (string.IsNullOrEmpty(OtaServerAddress))
            {
                OtaServerAddress = EB.Dot.String(source + "ota.url", toc, null);
            }

            ServerMaintenance = EB.Dot.Bool(source + "maintenance.state", toc, false);
            MaintenanceMessage = EB.Dot.String(source + "maintenance.messages." + locale, toc, null);
            if (string.IsNullOrEmpty(MaintenanceMessage))
            {
                MaintenanceMessage = EB.Dot.String(source + "maintenance.message", toc, null);
            }
            
            //Error  Type `string' does not contain a definition for `hashtable' 
            // IsVerifyState = EB.Dot.Bool(source + "verify", response.hashtable, false);

            callback(true);
            yield break;
        }
    }
#endif
    private Coroutine m_APICoroutine;
    private void CheckApi()
    {
#if !DEBUG
#if !USE_AMAZON_API
        CheckApiRequest(delegate (EB.Sparx.Response response)
        {
            if (response.sucessful && NeedsUpdate)
            {
                ShowRequiredUpdate(StoreUrl);
            }
        });
#else
        if (m_APICoroutine != null)
        {
            StopCoroutine(m_APICoroutine);
        }
        m_APICoroutine = StartCoroutine(CheckApiRequest(delegate (bool sucessful)
      {
          if (sucessful && NeedsUpdate)
          {
              ShowRequiredUpdate(StoreUrl);
          }
      }));
#endif
#endif
    }

    private bool CheckSupportsStencil()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            return true;
        }

        if (Application.platform != RuntimePlatform.Android)
        {
            return true;
        }

        string gpuVender = SystemInfo.graphicsDeviceVendor.ToLower();
        if (gpuVender.Contains("emulated") || gpuVender.Contains("google"))
        {
            return false;
        }

        string gpuName = SystemInfo.graphicsDeviceName.ToLower();
        if (gpuName.Contains("emulated") || gpuName.Contains("opengl es") || gpuName.Contains("pcie") || gpuName.Contains("sse2"))
        {
            return false;
        }

        return true;
    }


#if UNITY_ANDROID
    private int scaleWidth = 0;
    private int scaleHeight = 0;
#endif
    public void setDesignContentScale(int designWidth, int designHeight)
    {
#if UNITY_ANDROID
        if (scaleWidth == 0 && scaleHeight == 0)
        {
            int width = Screen.currentResolution.width;
            int height = Screen.currentResolution.height;
            float s1 = (float)designWidth / (float)designHeight;
            float s2 = (float)width / (float)height;
            if (s1 < s2)
            {
                designWidth = (int)Mathf.FloorToInt(designHeight * s2);
            }
            else if (s1 > s2)
            {
                designHeight = (int)Mathf.FloorToInt(designWidth / s2);
            }
            float contentScale = (float)designWidth / (float)width;
            if (contentScale < 1.0f)
            {
                scaleWidth = designWidth;
                scaleHeight = designHeight;
            }
        }
        if (scaleWidth > 0 && scaleHeight > 0)
        {
            if (scaleWidth % 2 == 0)
            {
                scaleWidth += 1;
            }
            else
            {
                scaleWidth -= 1;
            }
            Screen.SetResolution(scaleWidth, scaleHeight, true);
            EB.Debug.Log("setDesignContentScale scaleWidth={0} scaleHeight={1}", scaleWidth, scaleHeight);
        }
#endif
    }

    public string GetAuthAPIAddress()
    {
#if DEBUG && !NO_DEBUG_SCREEN
        return GameStateDebugStartScreen.authAPIAddress;
#else
       return GameEngine.Instance.AuthServerAddress;
#endif
    }



#region 测试指令
#if UNITY_EDITOR
    [Header("QA_断线不连")]
    public bool _NetWork_Disconnect = false;
    [Header("QA_断线重连")]
    public bool _NetWork_Relogin = false;
    [Header("QA_断线重连加错误提示")]
    public bool _NetWork_ReloginWithError = false;

    [Header("QA_打印缓存信息")]
    public bool _Cache_PrintInfo = false;
    private void Update_QACommand()
    {
        if(_NetWork_Disconnect)
        {
            _NetWork_Disconnect = false;
            SparxHub.Instance.Disconnect(true);
        }

        if(_NetWork_Relogin)
        {
            _NetWork_Relogin = false;
            GameEngine.Instance.Relogin(string.Empty);
        }

        if(_NetWork_ReloginWithError)
        {
            _NetWork_ReloginWithError = false;
            GameEngine.Instance.Relogin("测试断线重连!");
        }

        if(_Cache_PrintInfo)
        {
            _Cache_PrintInfo = false;
            Update_QACommand_DumpCache();
        }
    }

    private void Update_QACommand_DumpCache()
    {
        string time = DateTime.Now.ToString().Replace('/', '_').Replace(':', '_');
        var sw = File.CreateText("E://LT_SnapShot//" + time + ".txt");

#region for 输出Unity堆每个Object
        {
            sw.WriteLine("====================Unity Heap Info=====================");
            long unityReserved = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
            long unityUsed = UnityEngine.Profiling.Profiler.usedHeapSizeLong;
            long unityUnUsed = UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();
            sw.WriteLine($"unityReserved: {unityReserved / (1024*1024)}MB");
            sw.WriteLine($"unityUsed: {unityUsed / (1024*1024)}MB");
            sw.WriteLine($"unityUnUsed: {unityUnUsed / (1024*1024)}MB");

            sw.WriteLine("====================Unity Heap Details=====================");
            var objs = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
            HashSet<UnityEngine.Object> ll = new HashSet<UnityEngine.Object>();
            foreach(var obj in objs)
            {
                ll.Add(obj);
            }
            foreach(var obj in ll)
            {
                sw.WriteLine($"{obj.GetType()}=={obj.name}");
            }
        }
#endregion

#region for 输出Mono堆Info
        Johny.UnityHeapDump.Dump();
#endregion

        sw.Close();
        sw.Dispose();
        EB.Debug.LogError("缓存信息打印成功!");
    }

#endif
#endregion
}
