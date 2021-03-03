using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public struct PlayState
    {
        public const string PlayState_InitCombat = "init"; //初始

        public const string PlayState_MainLand = "MainLand"; //主城
        public const string PlayState_LCCampaign = "LCCampaign";//副本
        public const string PlayState_Combat = "Combat";//战斗
    }

    public class IdleFlowAction : FlowControlAction
    {
        public override void OnAwake()
        {
            base.OnAwake();
            Name = "Idle";
        }

        private string m_State = string.Empty;
        public override void OnEnter()
        {
            LegionLogic.GetInstance();
            GuideManager.Instance.InitGuideState();//tOdo为什么删掉了
            //预先加载主城场景
            LoadingLogic.AddCustomProgress(2);
            SceneLoadManager.LoadOTALevelGroupAsync("MainLandView", SceneLoadBegin, SceneLoadFailed, SceneLoadLoading, SceneLoadFinished);
        }

        protected override void SceneLoadFinished(SceneRootEntry sceneRoot)
        {
            if (sceneRoot != null && sceneRoot.IsLoaded())
            {
                var themeLoadManager = sceneRoot.GetComponentInChildren<ThemeLoadManager>();

                if (themeLoadManager != null)
                {
                    LoadingLogic.AddCustomProgress(2);
                    string levelName = SceneManager.MainLandName;
                    themeLoadManager.LoadOTALevelAsync(levelName, levelName + ".unity.json", null, OnMainLandSceneLoadFailed, null, OnMainLandSceneLoaded);
                }
            }
        }

        private void OnMainLandSceneLoadFailed()
        {
            EB.Debug.LogError("[IdleFlowAction]InitializeSceneFailed: Fsm.ActiveStateName = {0}" , GameFlowHotfixController.ActiveStateName);
            UIStack.Instance.HideLoadingScreen();
            FatalError(EB.Localizer.GetString("ID_FSM_SCENE_LOAD_FAILED"));
        }

        private void OnMainLandSceneLoaded(SceneRootEntry sceneRoot)
        {
            m_State = PlayState.PlayState_MainLand;
            if (!DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out m_State))
            {
                EB.Debug.LogWarning("DataLookupsCache ===> playstate.state can't find!");
            }
            string state = m_State;

            if (string.Compare(state, PlayState.PlayState_InitCombat) == 0) //初始（合服状态也会变为init,因此新增新手引导字段判断）
            {
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
            }
            else if (string.Compare(state, PlayState.PlayState_MainLand) == 0) //主城
            {
                Hashtable mainland_session = null;
                DataLookupsCache.Instance.SearchDataByID<Hashtable>("mainlands", out mainland_session);
                if (mainland_session != null)
                {
                    MainLandLogic.GetInstance().OnSceneEnter(mainland_session);
                }
            }
            else if (string.Compare(state, PlayState.PlayState_LCCampaign) == 0)//副本
            {
                if (GameFlowControlManager.Instance != null)
                {
                    GameFlowControlManager.Instance.SendEvent("GoToInstanceView");
                }
            }
            else if (string.Compare(state, PlayState.PlayState_Combat) == 0)//战斗
            {
                if (GameFlowControlManager.Instance != null)
                {
                    GameFlowControlManager.Instance.SendEvent("GoToMainLandView");
                }
            }
        }
    }
}