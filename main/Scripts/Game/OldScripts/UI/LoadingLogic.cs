using System;
///////////////////////////////////////////////////////////////////////
//
//  LoadingLogic.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////
	
using UnityEngine; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LoadingLogic : MonoBehaviour
{
	public bool EnstackOnCreate { get { return false; } }
	public bool ShowUIBlocker { get { return false; } }
	public float BackgroundUIFadeTime {	get	{ return 0.0f; } }
	public bool Visibility { get { return gameObject.activeSelf; } }
    //Loading
    public float UpdateProgressSpeed = 10;
    public GameObject LoadingScreen;
    public UILabel LoadingLabel;
    public UITexture LoadingTexture;
    public UIProgressBar UIBar;
    public UILabel UIBarLabel;
    
    //Block
    public GameObject BlockPanel;

    #region Mem Cloud
    public GameObject CloudLoadingScreen;
    public ParticleSystemUIComponent CloudHoldCom;
    public ParticleSystemUIComponent CloudOpenCom;
    public ParticleSystemUIComponent CloudCloseCom;
    #endregion

    //Splash
    public GameObject SplashScreen;
    public UILabel _uiLabel;
    public UITexture _uiTexture;
    public GameObject _fxComponent;

    [ResourcesReference(typeof(Texture2D))]
	public ResourcesReference[] LoadingTextureResources;

    //loading图片是否只从预设内读取
    public bool onlyPrefab = false;
    public string ResourceLoadingBGPath = "UI/Textures/LoadingBG";

	private UIPanel _panel = null;
	private bool _showSplash = false;
	private float _outTimer = 0;
    private float _waitOutTimer = 20.0f;

	private float _timer = 0;
	private float _timerFactor = 2f;
	
	private string[] LoadingStr = {".  ", " . ", "  ."};

    //用于输出debug信息
    private static UILabel LABEL_DEBUG_OUTPUT;

    ///自行判空
    public static LoadingLogic Instance{get; set;} = null;

    #region CustomProgress API
    public bool UseCustomProgress{get;set;} = false;

    ///自定义进度，0-100
    private int CustomProgress{get;set;} = 0;

    public static void SetUseCustomProgress(bool used)
    {
        if(Instance != null)
        {
            Instance.UseCustomProgress = used;
        }
    }

    public static void AddCustomProgress(int prg, string flag)
    {
        if(Instance == null)
        {
            return;
        }

        if(Instance.CustomProgress == 100)
        {
            return;
        }
        
        if(Instance.UseCustomProgress)
        {
            Instance.CustomProgress += prg;
            Instance.CustomProgress = Mathf.Clamp(Instance.CustomProgress, 0, 99);
#if DEBUG
            EB.Debug.LogError($"[DEBUG]RealProgress: {Instance.CustomProgress}=Flag: {flag}");
            LABEL_DEBUG_OUTPUT.text = $"[DEBUG]RealProgress: {Instance.CustomProgress}=Flag: {flag}";
#endif
        }
    }
    #endregion

    void Awake()
	{
        //debug
        LABEL_DEBUG_OUTPUT = gameObject.GetComponent<UILabel>("LoadingScreen/Label_Debug");

		_panel = GetComponent<UIPanel>();
        

        string loadingText = EB.Localizer.GetString("ID_LOADING");
		for (int i = 0, len = LoadingStr.Length; i < len; ++i)
		{
			LoadingStr[i] = string.Format("{0} {1}", loadingText, LoadingStr[i]);
		}

        UIBar.value = 0;
        waitforOther = false;

        Instance = this;

        CloudHoldCom.gameObject.CustomSetActive(false);
        CloudCloseCom.gameObject.CustomSetActive(false);
        CloudOpenCom.gameObject.CustomSetActive(false);
    }

    void OnDestroy()
    {
        Instance = null;
    }
    
    void Update()
	{
        if (_showSplash)
        {
            SetLoadingText(_uiLabel);
        }

        if (waitforOther)
        {
            _outTimer -= Time.deltaTime;
			if(_outTimer >= 0)
			{
                GlobalUtils.SetText(UIBarLabel, string.Format(EB.Localizer.GetString("ID_COMBAT_WAITFOROTHER"), (int)_outTimer));
			}
			else
			{
                GlobalUtils.SetText(UIBarLabel, EB.Localizer.GetString("ID_LOADING"));
                if (CombatLogic.Instance.LocalPlayerIsObserver)
                {
                    GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "ExitWatchAsk");
                }
            }
            if (_outTimer <= -_waitOutTimer)
            {
                _outTimer = 0;
				EB.Debug.LogError("LoadingLogic.Update: loading timedout");
				SparxHub.Instance.Disconnect(true);
            }
        }
        else
        {
            if (!_showSplash && _outTimer > 0)
            {
                _outTimer -= Time.deltaTime;
                if (_outTimer <= 0)
                {
                    _outTimer = 0;
                    EB.Debug.LogError("LoadingLogic.Update: loading timedout");
                    SparxHub.Instance.Disconnect(true);
                    return;
                }
            }

            if(UseCustomProgress)
            {
                UpdateCustomProgress();
            }
        }
    }

	void SetLoadingText(UILabel label)
	{
		label.text = LoadingStr[Mathf.FloorToInt(_timer % 3)];
		_timer += Time.smoothDeltaTime * _timerFactor;
	}

    /// <summary>
    /// 等待其他玩家的状态值
    /// </summary>
    private bool waitforOther = false;
    private const float intervalProgressTime = 0.5f;
    private float currentProgressTime = 0f;

    #region Update自定义进度条
    private int _UpdateCustomProgress_CurPrg = 0;
    private Action<bool> _UpdateCustomProgress_HideAct = null;
    private bool _UpdateCustomProgress_HideAct_Fade = false;
    private float _UpdateCustomProgress_After100Delay = 0.5f;
    public void UpdateCustomProgress()
    {
        _UpdateCustomProgress_CurPrg += UnityEngine.Random.Range(1, 5);
        _UpdateCustomProgress_CurPrg = Mathf.Min(_UpdateCustomProgress_CurPrg, CustomProgress);
        UIBar.value = (float)_UpdateCustomProgress_CurPrg / 100;
        GlobalUtils.SetText(UIBarLabel, $"{_UpdateCustomProgress_CurPrg}%");

        if(_UpdateCustomProgress_CurPrg >= 100)
        {
            if(_UpdateCustomProgress_After100Delay <= 0.0f)
            {
                _UpdateCustomProgress_After100Delay = 0.5f;
                UseCustomProgress = false;
                CustomProgress = 0;
                _UpdateCustomProgress_HideAct?.Invoke(_UpdateCustomProgress_HideAct_Fade);
                _UpdateCustomProgress_HideAct = null;_UpdateCustomProgress_HideAct_Fade=false;
            }
            else
            {
                _UpdateCustomProgress_After100Delay -= Time.deltaTime;
            }
        }
    }
    #endregion

	public void SetProgressFinsh(Action<bool> act, bool fade, bool immediately)
	{
        if(waitforOther)
        {
            act?.Invoke(fade);
        }
        else
        {
            if(!UseCustomProgress)
            {
                act?.Invoke(fade);
                return;
            }
            CustomProgress = 100;
            _UpdateCustomProgress_HideAct = act;
            _UpdateCustomProgress_HideAct_Fade = fade;
            if(immediately)
            {
                _UpdateCustomProgress_CurPrg = 100;
            }
            //释放资源
            EB.Assets.UnloadUnusedAssets();
            #region 强制回收GC
            System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
            #endregion
        }
    }

    ///设置等待其他玩家模式
    public void SetWaitForPlayer()
    {
        UseCustomProgress = false;
        UIBar.value = 1f;
        waitforOther = true;
        _outTimer = (!CombatLogic.Instance.LocalPlayerIsObserver)?60:20;//设为与服务器自动战斗时间相同非观战60秒，观战20秒
        _waitOutTimer = 20;//超时时间
        GlobalUtils.SetText(UIBarLabel,string.Format( EB.Localizer .GetString ("ID_COMBAT_WAITFOROTHER"), (int)_outTimer));
    }

    #region SetLoadingScreen
    private Action _SetLoadingScreen_OnReady = null;
    private void SetLoadingScreen_DelayOnReady(int seq)
    {
        _SetLoadingScreen_OnReady?.Invoke();
        _SetLoadingScreen_OnReady = null;
    }
    public void SetLoadingScreen(bool showSplash,bool showBlock, bool showCloud = false, System.Action onReady = null)
	{
        LoadingLabel.text = string.Empty;
        _showSplash = showSplash && !showBlock;
        SplashScreen.CustomSetActive(showSplash);
        CloudLoadingScreen.CustomSetActive(showCloud);
        LoadingScreen.CustomSetActive(!_showSplash && !showCloud);
		BlockPanel.CustomSetActive(showBlock);

        if (_showSplash)
        {
            StartCoroutine(SetLoadingBG(onReady));
        }
        else if (showCloud)
        {
            _outTimer = 20;
            ShowCloudScreen(onReady);
        }
        else
        {
            _outTimer = 30; //改回20秒
            if (LoadingTexture.mainTexture == null)
            {
                Texture2D tex2d =null;
                if (!onlyPrefab)
                {
                    var objs = Resources.LoadAll(ResourceLoadingBGPath, typeof(Texture2D));
                    if (objs.Length > 0)
                    {
                        int splashIndex = UnityEngine.Random.Range(0, objs.Length);
                        tex2d = objs[splashIndex] as Texture2D;
                    }
                }

                if (tex2d == null)
                {
                    int splashIndex = UnityEngine.Random.Range(0, LoadingTextureResources.Length);
                    tex2d = LoadingTextureResources[splashIndex].Value as Texture2D;
                }
                //此设置至少delay一帧（可能与lateUpdate更新有关）
                LoadingTexture.SetTexture(tex2d);
                LoadingTexture.MakePixelPerfect();
                SetUITexture(LoadingTexture);
            }
            UseCustomProgress = true;
            LoadingLabel.text = GetLoadingTips();
            TimerManager.instance.AddFramer(5, 1, SetLoadingScreen_DelayOnReady);
            _SetLoadingScreen_OnReady = onReady;
        }

        _timer = 0;
	}
    #endregion


    IEnumerator SetLoadingBG(System.Action onReady)
    {
        if (ILRDefine.IS_FX)
        {
            _fxComponent.CustomSetActive(false);
            if (!GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.LoginBGPath))
            {
                yield return GameEngine.Instance.SetStreamingAssetsBG(GameEngine.Instance.LoginBGPath);
            }
            _uiTexture.mainTexture = GameEngine.Instance.TextureDic[GameEngine.Instance.LoginBGPath];
            _uiTexture.MakePixelPerfect();
            SetUITexture(_uiTexture);
            yield return null;
        }
        else
        {
            _fxComponent.CustomSetActive(true);
        }

        onReady?.Invoke();
    }

    /// <summary>
    /// 设置UITexture
    /// </summary>
    /// <param name="uiTexture"></param>
    private void SetUITexture(UITexture uiTexture)
    {
        float FSWidth = (float) UIRoot.list[0].manualWidth;
        float FSHeight = (float) UIRoot.list[0].manualHeight;
        float ScreenScale = (float)Screen.width / (float)Screen.height;
        float TextureScale = (float)uiTexture.width / (float)uiTexture.height;

        if (FSWidth/ Screen.width > FSHeight / Screen.height)
        {
            FSHeight = FSWidth / ScreenScale;
            uiTexture.height = (int)FSHeight + 2;
            uiTexture.width = (int)(uiTexture.height * TextureScale) + 2;
            if (uiTexture.width < FSWidth)
            {
                uiTexture.width = (int)FSWidth + 2;
                uiTexture.height = (int)(uiTexture.width / ScreenScale);
            }
        }
        else
        {
            FSWidth = FSHeight * ScreenScale;
            uiTexture.width = (int)FSWidth + 2;
            uiTexture.height = (int)(uiTexture.width / TextureScale) + 2;
            if (uiTexture.height < FSHeight)
            {
                uiTexture.height = (int)FSHeight + 2;
                uiTexture.width = (int)(uiTexture.height * ScreenScale);
            }
        }
        
    }

    private System.Action CloudCallback;

    private void ShowCloudScreen_DelayShow(int seq)
    {
        if(!CloudCloseCom.IsPlaying())
        {
            CloudCloseCom.Play();
            TimerManager.instance.AddTimer(1000, 1, HoldCloudScreen);
        }
    }

    private void ShowCloudScreen(System.Action callback)
    {
        CloudCloseCom.gameObject.CustomSetActive(true);
        CloudOpenCom.gameObject.CustomSetActive(false);
        CloudHoldCom.gameObject.CustomSetActive(false);
        CloudCallback = callback;
        TimerManager.instance.AddFramer(2, 1, ShowCloudScreen_DelayShow);
    }

    private void HoldCloudScreen(int seq)
    {
        CloudHoldCom.gameObject.CustomSetActive(true);
        CloudCloseCom.gameObject.CustomSetActive(false);
        CloudOpenCom.gameObject.CustomSetActive(false);
        if (CloudCallback != null)
        {
            CloudCallback();
            WithCloud = true;
            CloudCallback = null;
        }
    }

    public IEnumerator CloseCloudScreen()
    {
        if (LoadingScreen.activeSelf|| SplashScreen.activeSelf)
        {
            yield return new WaitForSeconds(0.8f);
            CloudLoadingScreen.CustomSetActive(true);
            LoadingScreen.CustomSetActive(false);
            SplashScreen.CustomSetActive(false);
        }

        if (CloudOpenCom != null)
        {
            CloudOpenCom.gameObject.CustomSetActive(true);
        }

        if (CloudCloseCom != null)
        {
            CloudCloseCom.gameObject.CustomSetActive(false);
        }

        if (CloudHoldCom != null)
        {
            CloudHoldCom.gameObject.CustomSetActive(false);
        }

        IsStartWithCloud = true;
        yield return new WaitForSeconds(1.0f);
    }

    private string GetLoadingTips()
	{
		string prefix = "ID_LOADING_TIPS_";
		int randomIndex = UnityEngine.Random.Range(1, 100);
		string text = string.Empty;
		for (int i = 0; i < 3; ++i)
		{
			string id = prefix + randomIndex;
			if (!EB.Localizer.GetString(id, out text))
			{
				text = string.Empty;
				if (randomIndex > 0)
				{
					randomIndex = randomIndex / 2;
				}
				else
				{
					break;
				}
			}
			else
			{
				break;
			}
		}
		return text;
	}

    private bool _isStartWithCloud;

    public bool IsStartWithCloud
    {
        get { return _isStartWithCloud; }
        private set { _isStartWithCloud = value; }
    }

    public bool Fade = true;
    public bool WithCloud = false;
    public IEnumerator OnRemoveFromStack()
	{
		_outTimer = 0;
		
        GameEngine.Instance.IsTimeToRootScene = true;

        if (WithCloud)
        {
           
            yield return CloseCloudScreen();
            WithCloud = false;
            IsStartWithCloud = false;
        }

        if (LoadingLabel != null)
        {
            LoadingLabel.text = string.Empty;
        }

        GM.AssetManager.RecordLoadStart("OnRemoveFromStack", "Fade");
        GM.AssetManager.RecordLoadEnd("OnRemoveFromStack", "Fade");

        //hide UIBar
        if (UIBarLabel != null)
        {
            UIBarLabel.gameObject.CustomSetActive(false);
        }

        if (UIBar != null)
        {
            UIBar.gameObject.CustomSetActive(false);
        }

        // release textures
        if (LoadingTexture != null && LoadingTexture.mainTexture != null)
        {
            EB.Assets.Unload(LoadingTexture.mainTexture, true);
            LoadingTexture.mainTexture = null;
        }
        yield break;
	}
}