using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 主城状态
    /// </summary>
    public class MainLandViewAction : FlowControlAction
    {
        public override void OnAwake()
        {
            base.OnAwake();
            Name = "MainLandView";
        }

        private int kMovementUpdateInterval = 2;
        private ThemeLoadManager m_ThemeLoadManager;
        private List<EnemyController> m_EnemiesInCombatRange = new List<EnemyController>();
        private Vector3 _PlayerPos;
        private System.DateTime _nextHeartbeat = System.DateTime.Now;
        private SceneRootEntry m_SceneRootEntry;

        private string playState = PlayState.PlayState_MainLand;

        public override void OnEnter()
        {
            LoadingLogic.AddCustomProgress(10);
            hudLoaded = false;
            sceneLoaded = false;
            PartitionObject.FlushAllAndUnload();
            base.OnEnter();
            FusionAudio.PostGlobalMusicEvent("MUS_CampaignView_Demo", true);
            
            Hotfix_LT.Messenger.AddListener<EnemyController, eBattleType>("AttackEnemyImediatly", AttackEnemyImediatly);
            Hotfix_LT.Messenger.Raise("LTSpeedSnatchEvent.IdleFollow");
            MainlandViewAction();

            LTWelfareModel.Instance.SetMaxCampaignLevel();//更新通关的最大章节
        }

        public void MainlandViewAction()
        {
            if (!GuideNodeManager.GuideFailState.Equals("None"))
            {
                LTInstanceMapModel.Instance.IsEnterChallenge = false;
                LTInstanceMapModel.Instance.IsEnterAlienMaze = false;
                return;
            }

            if (LTInstanceMapModel.Instance.IsEnterChallenge)
            {
                LTInstanceMapModel.Instance.IsEnterChallenge = false;
                if (GlobalMenuManager.Instance != null && !GlobalMenuManager.Instance.IsContain("LTChallengeInstanceSelectHud"))
                {
                    GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
                    if (LTInstanceMapModel.Instance.IsTemporaryQuit)
                    {
                        LTInstanceMapModel.Instance.IsTemporaryQuit = false;
                        LTInstanceMapModel.Instance.RequestChallengeLevelInfo();//刷新挑战副本状态，用于征战红点
                    }
                    else
                    {
                        GlobalMenuManager.Instance.PushCache("LTChallengeInstanceSelectHud");
                    }
                }
            }
            if (LTInstanceMapModel.Instance.IsEnterAlienMaze)
            {
                LTInstanceMapModel.Instance.IsEnterAlienMaze = false;
                if (GlobalMenuManager.Instance!=null &&!GlobalMenuManager.Instance.IsContain("LTAlienMazeHud"))
                {
                    GlobalMenuManager.Instance.PushCache("LTInstanceMapHud");
                    GlobalMenuManager.Instance.PushCache("LTAlienMazeHud");
                } //LTInstanceMapModel.Instance.RequestChallengeLevelInfo(null, LTInstanceMapModel.AlienMazeTypeStr);//刷新迷宫副本状态，用于征战红点
            }
        }
        
        public override void OnExit()
        {
            base.OnExit();
            if(UITooltipManager.Instance!=null) UITooltipManager.Instance.HideTooltipForPress();

            Hotfix_LT.Messenger.RemoveListener<EnemyController, eBattleType>("AttackEnemyImediatly", AttackEnemyImediatly);
            //要先于销毁当前场景的执行 先把人物，任务等逻辑卸掉，否则会有问题
            if (MainLandLogic.GetInstance() != null)
            {
                if (MainLandLogic.GetInstance().IsInScene())
                {
                    MainLandLogic.GetInstance().OnExitSceneView();
                }
                MainLandLogic.GetInstance().DestroyDynamicGameObjects();
            }

            //压缩pspool，释放mainland ps资源
            {
                PSPoolManager.Instance.CompressPool();
                PSPoolManager.Instance.UnloadStandardMainlandFX();
                EB.Assets.UnloadBundle("SpiritAnimaFx");
            }
        }

        protected override void InitializeCamera()
        {
            base.InitializeCamera();
            GlobalCameraData.LoadCameraData(GlobalCameraData.CAMPAIGN_VIEW_CAMERA);
        }

        bool hudLoaded = false;
        bool sceneLoaded = false;

        protected override void HudLoadComplete(bool NoError, string[] Show)
        {
            base.HudLoadComplete(NoError, Show);
            hudLoaded = true;
            OnComplete();
        }
        
        #region 主城加载结束
        private bool OnComplete()
        {
            if (!sceneLoaded || !hudLoaded)
            {
                return false;
            }

            if (!DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out playState))
            {
                EB.Debug.LogWarning("DataLookupsCache ===> playstate.state can't find!");
            }

            if (string.Compare(playState, PlayState.PlayState_Combat) == 0) //若是战斗场景就进入战斗场景
            {
                int combat_session_id = -1;
                DataLookupsCache.Instance.SearchIntByID("playstate.Combat.combat_id", out combat_session_id);
                MainLandLogic.GetInstance().RequestResumeCombat(combat_session_id);
                int type = -1;
                DataLookupsCache.Instance.SearchIntByID("playstate.Combat.origin.combatType", out type);
                BattleReadyHudController.SetBattleType((eBattleType)type);
                if (type==(int)eBattleType.HeroBattle)
                {
                    LTNewHeroBattleManager.GetInstance().OnLoggedIn();
                }
                return false;
            }
            else if (string.Compare(playState, PlayState.PlayState_InitCombat) == 0) //（合服状态也会变为init,因此新增新手引导字段判断）若是首次登录就申请开场大战
            {
                UIStack.Instance.ShowLoadingScreen(() =>
                {
                    CombatCamera.isBoss = true;
                    if (GuideNodeManager.isFirstCombatGuide())
                    {
                        GuideNodeManager.GetInstance().GetGuideNodeCompleted(delegate (bool success)
                        {
                            if (success)
                            {
                                MainLandLogic.GetInstance().RequestFastCombatTransition(eBattleType.FastCampaignBattle, "1_1");
                            }
                            else
                            {
                                MainLandLogic.GetInstance().RequestTransfer(SceneManager.MainLandName, Vector3.zero, 0, SceneManager.MainLandName, SceneManager.HeroStart, 0);
                            }
                        });
                    }
                    else
                    {
                        MainLandLogic.GetInstance().RequestTransfer(SceneManager.MainLandName, Vector3.zero, 0, SceneManager.MainLandName, SceneManager.HeroStart, 0);
                    }
                }, false, true);
                return false;
            }
            return true;
        }
        
        protected override void SceneLoadFinished(SceneRootEntry sceneRoot)
        {
            EB.Debug.Log("MainLandViewAction.SceneLoadFinished=====>");
            LoadingLogic.AddCustomProgress(15);
            if (sceneRoot != null && sceneRoot.IsLoaded())
            {
                m_SceneRootEntry = sceneRoot;
                m_ThemeLoadManager = sceneRoot.GetComponentInChildren<ThemeLoadManager>();

                sceneLoaded = true;
                LoadingLogic.AddCustomProgress(5);
                OnComplete();

                if (m_ThemeLoadManager != null && !string.IsNullOrEmpty(MainLandLogic.GetInstance().CurrentEnvironmentName))
                {
                    string levelName = MainLandLogic.GetInstance().CurrentEnvironmentName;
                    m_ThemeLoadManager.LoadOTALevelAsync(levelName, levelName + ".unity.json", null, OnInitSceneFailed, null, (SceneRootEntry entry)=> { EB.Coroutines.Run(OnInitSceneSucced(entry)); });
                }
            }
            LoadingLogic.AddCustomProgress(5);
        }

        void OnInitSceneFailed()
        {
            EB.Debug.LogError("MainLandViewAction.OnInitSceneFailed=====> Fsm.ActiveStateName = {0}",GameFlowHotfixController.ActiveStateName);
            UIStack.Instance.HideLoadingScreen();
            FatalError(EB.Localizer.GetString("ID_FSM_SCENE_LOAD_FAILED"));
        }

        #region 后续工作
        IEnumerator OnInitSceneSucced(SceneRootEntry sceneRoot)
        {
            LoadingLogic.AddCustomProgress(5);
            if (m_SceneRootEntry != null)
            {
                m_SceneRootEntry.ShowLevel();
            }
            SetShadowQuality(sceneRoot);
            SetLayerMask(sceneRoot);
            sceneRoot.SetZonesTag();
            sceneRoot.SetMainLight();
            LoadingLogic.AddCustomProgress(15);
            PSPoolManager.Instance.LoadStandardMainlandFX();
            yield return new WaitUntil(() =>
            {
                LoadingLogic.AddCustomProgress(1);
                return HudLoadManager.Completed;
            });
            GlobalMenuManager.PopCaches();
            MainLandLogic.GetInstance().FreshMapName();
            MainLandLogic.GetInstance().OnSceneViewLoaded();
            MainLandLogic.GetInstance().RobDartFight();
            GameEngine.Instance.IsRunFromEnterGameBtn = false;
            LoadingLogic.AddCustomProgress(5);
        }
        #endregion

        #endregion

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if (SceneLogic.SceneState != SceneLogic.eSceneState.SceneLoop)
            {
                return;
            }

            if (System.DateTime.Now < _nextHeartbeat)
            {
                return;
            }

            _nextHeartbeat = _nextHeartbeat.AddSeconds(kMovementUpdateInterval);

            PlayerController local_player_controller = PlayerManager.LocalPlayerController();
            if (local_player_controller == null)
            {
                return;
            }

            GameObject player = PlayerManager.LocalPlayerGameObject();
            Vector3 newPos = player.transform.position;
            float newDir = player.transform.rotation.eulerAngles.y;
            float dist = Vector3.Distance(_PlayerPos, newPos);

            if (dist > 1.0f)
            {
                LTHotfixManager.GetManager<SceneManager>().UpdatePlayerMovement(MainLandLogic.GetInstance().SceneId, newPos, newDir, null);
                _PlayerPos = newPos;
            }

            LTHotfixManager.GetManager<SceneManager>().OnLateUpdate();
        }

        public void AttackEnemyImediatly(EnemyController ec, eBattleType battletype)
        {
            if (SceneLogic.SceneState != SceneLogic.eSceneState.SceneLoop)
            {
                return;
            }

            PlayerController local_player_controller = PlayerManager.LocalPlayerController();
            if (local_player_controller == null)
            {
                return;
            }

            m_EnemiesInCombatRange.Clear();
            m_EnemiesInCombatRange.Add(ec);
            GameObject player = PlayerManager.LocalPlayerGameObject();

            if (battletype == eBattleType.HantBattle)
            {
                MainLandLogic.GetInstance().RequestBountyTaskCombatTransition();
            }
            else if (battletype == eBattleType.GhostBattle)
            {
                //夺宝奇兵 改成通过事件告诉给热更层
                FusionAudio.PostEvent("SFX_CombatView_BattleStart");
                Hotfix_LT.Messenger.Raise<string, string>("LTSpeedSnatchEvent.TouchEnemy", ec.name, MainLandLogic.GetInstance().SceneId.ToString());
            }
            else
            {
                if (ec.name == "EnemySpawns_11")
                {
                    EnemyController emCtrl = MainLandLogic.GetInstance().EnemyControllers["EnemySpawns_11"];
                    MainLandLogic.GetInstance().RequestCombatTransition(eBattleType.BossBattle, player.transform.position, player.transform.rotation, new List<EnemyController> { emCtrl });
                }
                else
                {
                    UIStack.Instance.ShowLoadingScreen(null, false, true);
                    MainLandLogic.GetInstance().RequestCombatTransition(battletype, player.transform.position, player.transform.rotation, m_EnemiesInCombatRange);
                }
            }
        }

    }
}
