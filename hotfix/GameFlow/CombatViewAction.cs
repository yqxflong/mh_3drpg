using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 战斗状态
    /// </summary>
    public class CombatViewAction : FlowControlAction
    {
        public override void OnAwake()
        {
            base.OnAwake();
            Name = "CombatView";
        }

        private SceneRootEntry m_SceneRootEntry;
        private ThemeLoadManager m_ThemeLoadManager;

        public override void OnEnter()
        {
            LoadingLogic.AddCustomProgress(10);
            EventManager.instance.Raise(eSimpleEventID.Combat_Enter);
            //日志记录
            GM.AssetManager.RecordLoadStart("CombatViewAction", "OnEnter");
            base.OnEnter();
            FusionAudio.PostGlobalMusicEvent("MUS_CombatView_Demo", true);
        }

        public override void OnExit()
        {
            if (GameFlowHotfixController.PreviousActiveStateName != null)
            {
                PartitionObject.FlushAllAndUnload();
            }

            base.OnExit();
            CombatLogic.Instance.ExitCombat();

            if (GameFlowHotfixController.PreviousActiveStateName != null )
            {
                if (PSPoolManager.Instance != null)
                {
                    PSPoolManager.Instance.CompressPool();
                    PSPoolManager.Instance.UnloadStandardCombatFX();
                }

                EB.Assets.UnloadBundle("SpiritAnimaFx");
            }

            //暴力释放掉战斗场景资源
            if (m_ThemeLoadManager != null)
            {
                F_Clear(m_ThemeLoadManager.GetSceneRoot().m_LevelAssets);
            }

            EventManager.instance.Raise(eSimpleEventID.Combat_Exit);
        }
        
        /// <summary>
        /// 清除战斗场景资源
        /// </summary>
        /// <param name="levelABNames">场景资源信息</param>
        public static void F_Clear(SceneRootEntry.PlacedAsset[] levelABNames)
        {
            if (levelABNames != null)
            {
                for (int i = 0; i < levelABNames.Length; i++)
                {
                    //清除相应的资源引用
                    if (levelABNames[i] != null) EB.Assets.UnloadAssetByName(levelABNames[i].name, true);
                }
            }
        }

        protected override void InitializeCamera()
        {
            base.InitializeCamera();
            GlobalCameraData.LoadCameraData(GlobalCameraData.COMBAT_VIEW_CAMERA);
        }

        public static string CITYVIEW_COMBAT_NAME = "CityCombat";
        public static string DEFAULT_COMBAT_NAME = "LvDiCombat";
        public static string BOSS_COMBAT_NAME = "ChiYanJinNiCombat";
        public static string Combat_Scene_Name;
        protected string GetCombatSceneName()
        {
            switch (SceneLogic.BattleType)
            {
                case eBattleType.MainCampaignBattle:
                    var mainCampaigntpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(LTMainInstanceCampaignCtrl.CampaignId.ToString());
                    if (mainCampaigntpl != null)
                    {
                        return mainCampaigntpl.MapName;
                    }
                    break;
                case eBattleType.ChallengeCampaign:
                    var challengeTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetChallengeStyleById(LTInstanceMapModel.Instance.CurMapStyle);
                    if (challengeTpl != null)
                    {
                        return challengeTpl.CombatScene;
                    }
                    break;
                default:
                    return Hotfix_LT.Data.SceneTemplateManager.Instance.GetRandomLostCombatMapByType(SceneLogic.BattleType);
            }
            return DEFAULT_COMBAT_NAME;
        }

        protected override void SceneLoadFinished(SceneRootEntry sceneRoot)
        {
            LoadingLogic.AddCustomProgress(15);
            UIStack.Instance.ResetNextStackDepth();
            EB.Debug.Log("LoginProfile {0}" , ((float)((System.DateTime.UtcNow.Ticks / 10000) % 100000) / 1000));
            if (sceneRoot != null && sceneRoot.IsLoaded())
            {
                m_SceneRootEntry = sceneRoot;
                m_ThemeLoadManager = sceneRoot.GetComponentInChildren<ThemeLoadManager>();
                if (m_ThemeLoadManager != null)
                {
                    LoadingLogic.AddCustomProgress(5);
                    Combat_Scene_Name = GetCombatSceneName();
                    if (string.IsNullOrEmpty(Combat_Scene_Name))
                    {
                        Combat_Scene_Name = DEFAULT_COMBAT_NAME;
                    }
                    m_ThemeLoadManager.LoadOTALevelAsync(Combat_Scene_Name, Combat_Scene_Name + ".unity.json", null, InitializeSceneFailed, null, InitializeScene);
                }

                InteractableObjectManager.Instance.SetParent(sceneRoot.m_SceneRoot.transform);
            }
            LoadingLogic.AddCustomProgress(10);
        }

        void InitializeSceneFailed()
        {
            EB.Debug.LogError("[CombatViewAction]InitializeSceneFailed: Fsm.ActiveStateName = {0}" , GameFlowHotfixController.ActiveStateName);
            UI3DLobby[] lobbys = GameObjectExtension.FindMonoILRObjectsOfType<UI3DLobby>("Hotfix_LT.UI.UI3DLobby", "Hotfix_LT.UI.UI3DVsLobby");

            if (lobbys != null && lobbys.Length > 0)
            {
                for (var i = 0; i < lobbys.Length; i++)
                {
                    var lobby = lobbys[i];

                    if (lobby != null)
                    {
                        EB.Debug.LogWarning("CombatViewAction:SceneLoadFinished Clean lobby={0}" , lobby);
                        GameObject.Destroy(lobby.mDMono.gameObject);
                    }
                }
            }

            UIStack.Instance.HideLoadingScreen();
            //Fsm.GotoPreviousState();
            FatalError(EB.Localizer.GetString("ID_FSM_SCENE_LOAD_FAILED"));
        }

        /// <summary>
        /// 战斗场景加载完毕
        /// </summary>
        /// <param name="sceneRoot"></param>
        void InitializeScene(SceneRootEntry sceneRoot)
        {
            EB.Coroutines.Run(InitializeSceneCoroutine(sceneRoot));
        }

        IEnumerator InitializeSceneCoroutine(SceneRootEntry sceneRoot)
        {
            LoadingLogic.AddCustomProgress(10);
            EB.Debug.Log("LoginProfile {0}", ((float)((System.DateTime.UtcNow.Ticks / 10000) % 100000) / 1000));

            if (m_SceneRootEntry != null)
            {
                m_SceneRootEntry.ShowLevel();
            }

            while (!SceneLogic.MainHeroLoadComplete)
            {
                yield return null;
            }
            LoadingLogic.AddCustomProgress(15);
            SetShadowQuality(sceneRoot);

            while (!HudLoadManager.Completed)
            {
                yield return null;
            }

            UI3DLobby[] lobbys = GameObjectExtension.FindMonoILRObjectsOfType<UI3DLobby>("Hotfix_LT.UI.UI3DLobby", "Hotfix_LT.UI.UI3DVsLobby");

            if (lobbys != null)
            {
                if (lobbys != null && lobbys.Length > 0)
                {
                    for (var i = 0; i < lobbys.Length; i++)
                    {
                        var lobby = lobbys[i];

                        if (lobby != null)
                        {
                            EB.Debug.LogWarning("CombatViewAction: SceneLoadFinished Clean lobby = {0} ---> gameObject name: {1}", lobby, lobby.mDMono.name);
                            GameObject.Destroy(lobby.mDMono.gameObject);
                        }
                    }
                }
            }

            LTMainHudManager.Instance.SetsFromFirstBattleType(false);

            if (SceneLogic.BattleType == eBattleType.FirstBattle)
            {
                LTMainHudManager.Instance.SetsFromFirstBattleType(true);
                UIStack.Instance.HideLoadingScreenImmediately(false, false);
            }
            else if (SceneLogic.isPVP())
            {
                UIStack.Instance.WaitForOtherPlayer();
            }
            else
            {
                //UIStack.Instance.HideLoadingScreenImmediately(false, false);
            }


            GM.AssetManager.RecordLoadEnd("CombatViewAction", "HideLoadingScreen");

            LTCombatEventReceiver.Instance?.OnCombatViewLoaded();

            MainLandLogic.GetInstance().OnCombatViewLoaded();
            EB.Debug.Log("LoginProfile {0}" , ((float)((System.DateTime.UtcNow.Ticks / 10000) % 100000) / 1000));
            EB.Debug.Log("To initialize scene.");
            LoadingLogic.AddCustomProgress(5);
        }
    }
}