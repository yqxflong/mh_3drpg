using EB.Sparx;
using Hotfix_LT.Data;
using Hotfix_LT.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.GameState
{
    public class LTGameStateLogin : GameStateUnit
    {
        public override eGameState mGameState { get { return eGameState.Login; } }

        public override IEnumerator Start()
        {
            GameStateDownload.LogWithTime("LTGameStateLogin.Start=====>");
            if (ILRDefine.IS_FX)
            {
                if (!GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.LoginBGPath))
                {
                    yield return GameEngine.Instance.SetStreamingAssetsBG(GameEngine.Instance.LoginBGPath);
                }
                if (!GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.BrandPath))
                {
                    yield return GameEngine.Instance.SetStreamingAssetsBG(GameEngine.Instance.BrandPath);
                }
            }
            else if(Application .identifier.Equals("com.mkhx.xinkuai"))
            {
                if (!GameEngine.Instance.TextureDic.ContainsKey(GameEngine.Instance.BrandPath))
                {
                    yield return GameEngine.Instance.SetStreamingAssetsBG(GameEngine.Instance.BrandPath);
                }
            }

            // wait network available
            #region wait network available
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return GameEngine.Instance.StartCoroutine(ShowLoadingScreen());
                yield return GameEngine.Instance.StartCoroutine(ShowNetworkRetryDialog());
            }
            #endregion

            // load LoginUI
            yield return GameEngine.Instance.StartCoroutine(LoadLoginUI());

            UIStack.Instance.ForceHideLoadingScreen();
            LoadingSpinner.Destroy();

            GameObject obj = GameObject.Find("DownloadHudUI");
            if (obj != null) GameObject.Destroy(obj);

            //清除表数据
            ClientDataUtil.OnResetTemplateManager();
            //清datalookup缓存
            DataLookupsCache.Instance.ClearCache();

            GameEngine.Instance.LoginListener.ResolveMHLoginEvent += OnResolveMHLoginEvent;
            GameEngine.Instance.LoginListener.LoginEvent += OnLogin;
            GameEngine.Instance.LoginListener.LoginFailedEvent += OnLoginFailed;
            GameEngine.Instance.LoginListener.UpdateEvent += OnUpdateRequired;

            LoginManager.Instance.LoginExtraListener.ResolveAuthenticatorsEvent += OnResolveAuthenticators;
            LoginManager.Instance.LoginExtraListener.ResolveMultipleAuthenticatorsEvent += OnResolveMultipleAuthenticators;
            LoginManager.Instance.LoginExtraListener.AuthorizedEvent += OnAuthorized;

            LoginManager.Instance.Api.ErrorHandler += OnApiError;

            //设置音效
            AudioManager.Initialize();
            //预加载主场景摄像机
            GlobalCameraData.LoadCameraData(GlobalCameraData.CAMPAIGN_VIEW_CAMERA);

            yield return new WaitForSeconds(0.2f);//等待小段时间防止花屏
            Enumerate();
            
            GameStateDownload.LogWithTime("<=====LTGameStateLogin.Start");
        }

        public override void End()
        {
            GameEngine.Instance.TextureDic.Clear();
            EB.Debug.Log("Ending LTGameState LOGIN");
            GameEngine.Instance.LoginListener.ResolveMHLoginEvent -= OnResolveMHLoginEvent;
            GameEngine.Instance.LoginListener.LoginEvent -= OnLogin;
            GameEngine.Instance.LoginListener.LoginFailedEvent -= OnLoginFailed;
            GameEngine.Instance.LoginListener.UpdateEvent -= OnUpdateRequired;

            LoginManager.Instance.LoginExtraListener.ResolveAuthenticatorsEvent -= OnResolveAuthenticators;
            LoginManager.Instance.LoginExtraListener.ResolveMultipleAuthenticatorsEvent -= OnResolveMultipleAuthenticators;
            LoginManager.Instance.LoginExtraListener.AuthorizedEvent -= OnAuthorized;

            LoginManager.Instance.Api.ErrorHandler -= OnApiError;
            
            DestroyLoginUI();
        }

        GameObject m_LoginUI = null;
        System.Action m_OnLoginUILoaded = null;

        private void Enumerate()
        {
            LoginManager.Instance.Enumerate();
        }

        private void OnResolveAuthenticators(Authenticator authenticator)
        {
            LoginManager.Instance.Login(authenticator.Name);
        }

        private void OnResolveMHLoginEvent(object param)
        {
            EB.Debug.Log("GameStateLogin.OnResolveMHMuthUIEvent");

            LoadLoginUI(() =>
            {
                Hotfix_LT.Messenger.Raise<string, object, bool>(Hotfix_LT.EventName.ShowMenu, "AccountUI", param, false);
                UIStack.Instance.HideLoadingScreen(false);
            });
        }

        private void OnResolveMultipleAuthenticators(Authenticator[] authenticators)
        {
            EB.Debug.Log("GameStateLogin.OnResolveMultipleAuthenticators: {0} authenticators", authenticators.Length);

            LoadLoginUI(() =>
            {
                Hotfix_LT.Messenger.Raise<string, object, bool>(Hotfix_LT.EventName.ShowMenu, "AthenticatorSelect", null, true);
                UIStack.Instance.HideLoadingScreen(false);
            });
        }

        private void OnAuthorized(Authenticator authenticator, Account account)
        {
            EB.Debug.Log("GameStateLogin.OnAuthorized: authenticator = {0}", authenticator.Name);
            var lm = LoginManager.Instance;
            if (lm.GameWorlds.Length == 0)
            {
                if (ILRDefine.DEBUG)
                {
                    if (GameEngine.Instance.IsResetUserData)
                    {
                        lm.DebugResetWorldUser(Johny.HashtablePool.Claim(), delegate (string err, object result)
                        {
                            if (!string.IsNullOrEmpty(err))
                            {
                                EB.Debug.LogError("DebugResetWorldUser: error = {0}", err);
                                return;
                            }

                            GameEngine.Instance.IsResetUserData = false;
                            lm.EnterGame(Johny.HashtablePool.Claim());
                        });
                        return;
                    }
                }
                else
                {
                    lm.EnterGame(Johny.HashtablePool.Claim());
                }
            }
            else
            {
                LoadLoginUI(() =>
                {
                    Hotfix_LT.Messenger.Raise<string, object, bool>(Hotfix_LT.EventName.ShowMenu, "EnterGameUI", null, true);
                    UIStack.Instance.HideLoadingScreen(false);
                });
            }
        }

        private void OnLogin()
        {
            FusionTelemetry.PostLogin(LoginManager.Instance.Account, LoginManager.Instance.LocalUser);
            Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;
            if (loginData != null)
            {
                ProfileManager.Instance.SocialCurrency = EB.Dot.Integer("socialCurrency", loginData, 0);
                ProfileManager.Instance.HardCurrency = EB.Dot.Integer("wallet.balance", loginData, 0);
                
                GameDataSparxManager.Instance.ProcessIncomingData(loginData, false);
                DataLookupsCache.Instance.CacheData(loginData);
            }

            GameEngine.Instance.StartCoroutine(WaitForReady());
        }

        private IEnumerator WaitForReady()
        {
            DateTime t = DateTime.Now;
            EB.Debug.Log("WaitForReady=====>_{0}", t.ToString("yyyy-MM-dd hh:mm:ss fff"));
            while (!HudLoadManager.IsReady)
            {
                if (!HudLoadManager.IsLoadingConfig)
                {
                    HudLoadManager.LoadConfigAsync();
                }
                yield return null;
            }
            LoadingLogic.AddCustomProgress(5);
            while (!SceneLoadManager.IsReady)
            {
                if (!SceneLoadManager.IsLoadingConfig) SceneLoadManager.LoadConfigAsync();
                yield return null;
            }
            LoadingLogic.AddCustomProgress(5);

            Mgr.SetGameState<GameStateLoadGame>();

            t = DateTime.Now;
            EB.Debug.Log("<=====WaitForReady_{0}", t.ToString("yyyy-MM-dd hh:mm:ss fff"));
        }

        private bool OnApiError(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            if (response.error.ToString() == "ID_SPARX_ERROR_TEMP_BANNED" || response.error.ToString() == "ID_SPARX_ERROR_BANNED")
            {
                response.ShowErrorModal(delegate ()
                {
                    Hotfix_LT.Messenger.Raise<string, object, bool>(Hotfix_LT.EventName.ShowMenu, "EnterGameUI", null, true);
                    UIStack.Instance.HideLoadingScreen();
                });
                return true;
            }

            return false;
        }

        private void OnLoginFailed(string error)
        {
            UIStack.Instance.ExitStack(true);
            ShowLoadingScreen(null);

            EB.Debug.LogWarning("OnLoginFailed: login error = {0}", error);
            if (string.IsNullOrEmpty(error))
            {
                EB.Coroutines.SetTimeout(Enumerate, 30);
            }
            else
            {
                ShowLoginErrorRetryDialogue(error, Enumerate);
            }
        }

        private void ShowLoginErrorRetryDialogue(string error, System.Action retry)
        {
            EB.Debug.LogWarning("----------------ShowLoginErrorRetryDialogue----------------");
            string content = error != null && error.StartsWith("ID_") ? EB.Localizer.GetString(error) : EB.Localizer.GetString("ID_SPARX_LOGIN_RETRY");

            if (ILRDefine.DEBUG)
            {
                content = string.Format("{0}\n{1}", content, error);
            }

            EB.Debug.LogWarning("----------------Confirm----------------");
            UIStack.Instance.GetDialog("Confirm",content, delegate (eUIDialogueButtons button, UIDialogeOption opt)
            {
                EB.Debug.LogWarning("----------------OnConfirm----------------");
                if (button == eUIDialogueButtons.Accept)
                {
                    EB.Debug.LogWarning("----------------retry----------------");
                    retry();
                }
            });
        }

        private void OnUpdateRequired(string storeUrl)
        {
            GameEngine.Instance.ShowRequiredUpdate(storeUrl);
        }

        private void LoadLoginUI(System.Action callback)
        {
            if (m_LoginUI != null)
            {
                callback?.Invoke();
                return;
            }

            m_OnLoginUILoaded += callback;
        }

        private IEnumerator LoadLoginUI()
        {
            bool loadFinished = false;
            GameObject go = null;
            EB.Assets.LoadAsync("Assets/_GameAssets/Res/Prefabs/UIPrefabs/LTLogin/UI_Login", typeof(GameObject), o=>
            {
                loadFinished = true;
                if(o != null)
                {
                    go = GameObject.Instantiate(o) as GameObject;
                }
            });

            yield return new WaitUntil(()=> loadFinished);

            if (go != null)
            {
                m_LoginUI = go;
                UIHierarchyHelper.Instance.Place(m_LoginUI, UIHierarchyHelper.eUIType.None, UIAnchor.Side.Center);

                if (m_OnLoginUILoaded != null)
                {
                    m_OnLoginUILoaded();
                    m_OnLoginUILoaded = null;
                }
            }
            else
            {
                EB.Debug.LogError("LoadLoginUI: load UI_Login failed");
            }

            if (m_LoginUI != null)
            {
                var bg = m_LoginUI.transform.GetMonoILRComponent<LTLoginBGPanelCtrl>("BGPanel");
                yield return new WaitUntil(() => bg.BGFinish);
            }

            if (UICamera.currentCamera) UICamera.currentCamera.clearFlags = CameraClearFlags.Depth;
            //UIStack.Instance.HideLogoutScreen();
            UIStack.Instance.HideLoadingScreen();
		}

        private void DestroyLoginUI()
        {
            LoadLoginUI(() =>
            {
                GameObject.DestroyImmediate(m_LoginUI);
                m_LoginUI = null;
                EB.Assets.UnloadAssetByName("UI_Login", true);
				#region 强制回收GC
				System.GC.Collect(System.GC.MaxGeneration, System.GCCollectionMode.Forced);
				System.GC.WaitForPendingFinalizers();
				System.GC.Collect();
				#endregion
            });
        }
    }
}