using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LTDownloadHudConroller : MonoBehaviour
{
    public static LTDownloadHudConroller instance;
    public static LTDownloadHudConroller Instance { get { return instance; } }

    private GameObject WarningScreen;
    private UILabel QualityControlLabel;

    private GameObject SplashScreen;
    private UILabel SplashLabel;
    private UILabel VersionLabel;
    private UIProgressBar SplashProgressBar;
    private LTDownloadDialogueController DownloadDialogue;


    private bool ShowProgress = false;
    private float _timer = 1;
    private float _timerFactor = 2f;
    private int loadingIndex = 0;
    private string loadingText;
    private string[] LoadingStr = { ".  ", " . ", "  ." };

    private UITexture _uiTexture;
    private GameObject _fxComponent;

    private UIAtlas _uiAtlas;

    public void Awake()
    {
        instance = this;

        WarningScreen = transform.Find("WarningScreen").gameObject;
        QualityControlLabel = WarningScreen.GetComponent<UILabel>("QualityControlLabel");
        SplashScreen = transform.Find("SplashScreen").gameObject;
        SplashLabel = SplashScreen.transform.Find("SplashLabel").GetComponent<UILabel>();
        VersionLabel = SplashScreen.transform.Find("DistributeAnchor/Layout/FullVersion").GetComponent<UILabel>();
        SplashProgressBar = SplashScreen.transform.Find("ProgressBar").GetComponent<UIProgressBar>();
        DownloadDialogue= transform.Find("DialogueScreen").GetComponent<LTDownloadDialogueController>();
        
        SplashProgressBar.value = 0;

        _uiTexture = SplashScreen.GetComponent<UITexture>("Panel/Texture");
        _fxComponent = SplashScreen.FindEx("Panel/Texture/FX");
        _uiAtlas = WarningScreen.GetComponent<UISprite>().atlas;
      
    }

    private void SetLoadingStr()
    {
        if (string.IsNullOrEmpty(loadingText))
        {
            SplashLabel.text = string.Empty;
            loadingText = EB.Localizer.GetString("ID_LOADING");
            if (loadingText.StartsWith("MS_"))
            {
                loadingText = string.Empty;
            }
            else
            {
                for (int i = 0, len = LoadingStr.Length; i < len; ++i)
                {
                    LoadingStr[i] = string.Format("{0} {1}", loadingText, LoadingStr[i]);
                }
                loadingIndex = 0;
                SplashLabel.text = LoadingStr[loadingIndex];
            }
        }
    }

    public void OnDestroy()
    {
        instance = null;

        if (_uiTexture.mainTexture != null)
        {
            _uiTexture.mainTexture = null;
        }

        // 强制释放从Resources加载的资源，否则会常驻内存
        if (_uiAtlas != null)
        {
            Resources.UnloadAsset(_uiAtlas.texture);
            Resources.UnloadAsset(_uiAtlas.spriteMaterial);
            _uiAtlas = null;
        }
    }

    public void Update()
    {
        if (ShowProgress)
        {
            SetDownloadingText();
        }
    }

    private float BytesToMega(long bytes)
    {
        float mb = bytes / 1048576;
        return mb;
    }

    private void SetDownloadingText()
    {
        if (EB.Assets.IsDownloadingOnLoad && EB.Assets.ToDownloadBundles > 0)
        {
            if (Application.isMobilePlatform)
            {
                var needSpaceSize = BytesToMega(GM.AssetManager.GetTotalDownloadSize());
                var remainSpaceSize = 100f;  // 预留100MB给游戏中写入数据
                var freeSpaceSize = (float)SimpleDiskUtils.DiskUtils.CheckAvailableSpace();
                var isDiskFull = freeSpaceSize < (needSpaceSize + remainSpaceSize);

                if (isDiskFull)
                {
                    DownloadDialogue.Show_Tip(EB.Localizer.GetString("ID_DISK_FULL_TIPS"), buttonType =>
                    {
                        if (buttonType == eUIDialogueButtons.Accept)
                        {
#if !UNITY_EDITOR
                        Application.Quit();
#else
                            UnityEditor.EditorApplication.isPlaying = false;
#endif
                        }
                    });

                    ShowProgress = false;
                    return;
                }
            }

            SplashProgressBar.gameObject.CustomSetActive(true);
            int downloaded = EB.Assets.DownloadedBundles;
            int tobeDownload = EB.Assets.ToDownloadBundles;
            int totalBundles = EB.Assets.TotalBundles;
            long toBeDownloadSize = (long)((tobeDownload * 1.0f / totalBundles) * GM.AssetManager.GetTotalDownloadSize());

            if (downloaded < tobeDownload)
            {
                float downloadRate = (float)downloaded / (float)tobeDownload;
                float downLoadedSize = downloadRate * toBeDownloadSize;
                bool local = GM.AssetUtils.LoadFromLocalFile(GM.AssetManager.GetRemoteBundleFileBaseUrl());//判定远程服务器是否是本地OTA
                if (local)//从本地OTA释放资源
                {
                    SplashLabel.text = string.Format("{2} {0,3}/{1,-3}", SizeFormat(downLoadedSize), SizeFormat(toBeDownloadSize), EB.Localizer.GetString("ID_EXTRACT_ASSETBUNDLE"));
                    SplashProgressBar.value = (float)downLoadedSize / (float)toBeDownloadSize;
                }
                else//从远端OTA下载资源
                {
                    SplashLabel.text = string.Format("{3} {0,3}/{1,-3} {2}", SizeFormat(downLoadedSize), SizeFormat(toBeDownloadSize), FormatDownloadSpeed(EB.Assets.DownloadSpeed), EB.Localizer.GetString("ID_DOWNLOADING_ASSETBUNDLE"));
                    SplashProgressBar.value = (float)downLoadedSize / (float)toBeDownloadSize;
                }
            }
            else
            {
                SplashProgressBar.gameObject.CustomSetActive(false);
                SetLoadingText();
            }
        }
        else
        {
            SplashProgressBar.gameObject.CustomSetActive(false);
            SetLoadingText();
        }
    }
    private void SetLoadingText()
    {
        if (_timer >= 1)
        {
            if (string.IsNullOrEmpty(loadingText))
            {
                SetLoadingStr();
            }
            else
            {
                loadingIndex++;
                SplashLabel.text = LoadingStr[loadingIndex % 3];
            }
            _timer = 0;
        }
        else
        {
            _timer += Time.smoothDeltaTime * _timerFactor;
        }
    }
    private string SizeFormat(float bytes)
    {
        if (bytes < 0) bytes = 0;
        float kb = (float)bytes / 1024f;
        if (kb < 1)
        {
            return string.Format("{0} B", bytes.ToString("0.00"));
        }

        float mb = kb / 1024f;
        if (mb < 1)
        {
            return string.Format("{0} K", kb.ToString("0.00"));
        }

        float gb = mb / 1024f;
        if (gb < 1)
        {
            return string.Format("{0} M", mb.ToString("0.00"));
        }

        return string.Format("{0} G", gb.ToString("0.00"));
    }
    private string FormatDownloadSpeed(long bytes)
    {
        if (bytes < 0) bytes = 0;
        float kb = (float)bytes / 1024f;
        if (kb < 1)
        {
            return string.Format("{0,4} B/s", bytes.ToString("0.00"));
        }

        float mb = kb / 1024f;
        if (mb < 1)
        {
            return string.Format("{0,4} K/s", kb.ToString("0.00"));
        }

        float gb = mb / 1024f;
        if (gb < 1)
        {
            return string.Format("{0,4} M/s", mb.ToString("0.00"));
        }

        return string.Format("{0,4} G/s", gb.ToString("0.00"));
    }

    public IEnumerator ShowWarningScreen()
    {
#if IS_FX
            QualityControlLabel.text = "";
            _fxComponent.CustomSetActive(false);
            StartCoroutine(SetLoadingBG());
#else
        _fxComponent.CustomSetActive(true);
#endif

#if !USE_AOSHITANGSDK
        if (UserData.Locale == EB.Language.ChineseSimplified)
        {
            WarningScreen.CustomSetActive(true);
            //两秒后再消散云层
            yield return new WaitForSeconds(2);
            WarningScreen.CustomSetActive(false);
        }
#endif
        yield break;
    }

    string LoadingBGPath = "ReplaceableRes/Loading_BG.png";
    IEnumerator SetLoadingBG()
    {
#if !UNITY_EDITOR && UNITY_ANDROID
           string url =string.Format( "{0}/{1}", Application.streamingAssetsPath ,LoadingBGPath);
#else
        string url = string.Format("file://{0}/{1}", Application.streamingAssetsPath, LoadingBGPath);
#endif
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.error == null)
        {
            Texture2D localTexture = DownloadHandlerTexture.GetContent(www);
            UITexture tex = _uiTexture.GetComponent<UITexture>();
            tex.mainTexture = localTexture;
            tex.MakePixelPerfect();
            SetUITexture(tex);
        }
        else
        {
            _fxComponent.CustomSetActive(true);
            Debug.Log(www.error);
        }
        www.Dispose();
    }

    private void SetUITexture(UITexture uiTexture)
    {
        float FSWidth = (float)UIRoot.list[0].manualWidth;
        float FSHeight = (float)UIRoot.list[0].manualHeight;
        float ScreenScale = (float)Screen.width / (float)Screen.height;
        float TextureScale = (float)uiTexture.width / (float)uiTexture.height;

        if (FSWidth / Screen.width > FSHeight / Screen.height)
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

    public IEnumerator ShowSplashScreen()
    {
        SplashScreen.CustomSetActive(true);
        VersionLabel.text = EB.Version.GetFullVersion();
        ShowProgress = true;
        yield break;
    }

    public IEnumerator WaitForNetwork()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return GameEngine.Instance.StartCoroutine(ShowNetworkRetryDialog());
        }
    }

    public IEnumerator ConfirmCarrierDataNetwork()
    {
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            yield return GameEngine.Instance.StartCoroutine(ShowNetworkRetryDialog());
        }

        if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            yield return GameEngine.Instance.StartCoroutine(ShowNetworkConfirmDialog());
        }
    }

    public IEnumerator ShowNetworkRetryDialog()
    {
        bool proceed = false;

        ShowNetworkRetryDialog(delegate () { proceed = true; });

        while (!proceed)
        {
            yield return null;
        }
    }

    public void ShowNetworkRetryDialog(System.Action retry)
    {
        DownloadDialogue.Show_Confirm(EB.Localizer.GetString("ID_codefont_in_UIAccountController_1302"), delegate (eUIDialogueButtons button)
        {
            if (button == eUIDialogueButtons.Accept)
            {
                retry();
            }
            else if (button == eUIDialogueButtons.Cancel)
            {
#if !UNITY_EDITOR
                    Application.Quit();
#else
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

        });
    }

    public IEnumerator ShowNetworkConfirmDialog()
    {
        yield return new WaitForSeconds(0.5f);
        bool proceed = false;

        ShowNetworkConfirmDialog(delegate () { proceed = true; });

        while (!proceed)
        {
            yield return null;
        }
    }

    public void ShowNetworkConfirmDialog(System.Action retry)
    {
        DownloadDialogue.Show_Confirm(EB.Localizer.GetString("ID_SPARX_NETWORK_CARRIER_DATA"), delegate (eUIDialogueButtons button)
        {
            if (button == eUIDialogueButtons.Accept)
            {
                retry();
            }
            else if (button == eUIDialogueButtons.Cancel)
            {
#if !UNITY_EDITOR
                    Application.Quit();
#else
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }

        });
    }

    public void ShowTip(string Tip)
    {
        DownloadDialogue.Show_Tip(Tip, delegate (eUIDialogueButtons button)
        {
            if (button == eUIDialogueButtons.Accept)
            {
#if !UNITY_EDITOR
                    Application.Quit();
#else
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        });
    }

}
