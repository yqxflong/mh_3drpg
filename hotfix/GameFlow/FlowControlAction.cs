using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 状态机控件基类
    /// </summary>
    public class FlowControlAction : FsmStateUnit
    {
        public override void OnEnter()
        {
            base.OnEnter();
            MessageDialog.HideCurrent();
            FlowControlAction.onLoadComplete = LoadCompleteExtend;
            StartInitialize();
            PSPoolManager.Instance.LoadStandardHudFX();
            if (GuideNodeManager.ExecuteJump != null)
            {
                GuideNodeManager.ExecuteJump(NodeMessageManager.ForbidInstanceFloor);
                GuideNodeManager.ExecuteJump(NodeMessageManager.ForbidOther);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            //先释放掉CoreAsset的缓存池对象
            //EB.Assets.FlushAllAndUnload();
        }

        public static System.Action<bool> onLoadComplete;

        private void StartInitialize()
        {
            EB.Debug.Log("FlowControlAction.StartInitialize=====>");
            //设置性能配置
            PerformanceManager.Instance.UseScene(GameFlowHotfixController.ActiveStateName);
            //设置相机配置
            InitializeCamera();

            //异步加载场景跟ui
            HudLoadManager.LoadHudAsync(GameFlowHotfixController.ActiveStateName, HudLoadComplete);
            SceneLoadManager.LoadOTALevelGroupAsync(GameFlowHotfixController.ActiveStateName, SceneLoadBegin, SceneLoadFailed, SceneLoadLoading, SceneLoadFinished);

            //性能
            var perfInfo = PerformanceManager.Instance.CurrentEnvironmentInfo;
            SetQualityLevel(PerformanceManager.Instance.PerformanceInfo.CpuProfileName);
            Shader.globalMaximumLOD = perfInfo.lod;
            QualitySettings.blendWeights = (BlendWeights)perfInfo.blendWeights;
            QualitySettings.antiAliasing = (int)perfInfo.msaa;
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)perfInfo.aniso;
            EB.Debug.Log("<=====FlowControlAction.StartInitialize");
        }

        private void SetQualityLevel(string index)
        {
            switch (index)
            {
                case "High": QualitySettings.SetQualityLevel(0, true); break;
                case "Medium": QualitySettings.SetQualityLevel(1, true); break;
                case "Low": QualitySettings.SetQualityLevel(2, true); break;
                default: QualitySettings.SetQualityLevel(0, true); break;
            }
            EB.Debug.Log("QualitySettings.Level______{0}" , QualitySettings.GetQualityLevel());
        }

        private void LoadCompleteExtend(bool NoError)
        {
            if (!NoError)
            {
                string error = EB.Localizer.GetString("ID_codefont_in_IdleFlowAction_6208");
                EB.Debug.LogError(error);
                SparxHub.Instance.FatalError(error);
                return;
            }
        }

        protected void SetShadowQuality(SceneRootEntry sceneRoot)
        {
            if (sceneRoot == null || sceneRoot.m_SceneRoot == null)
            {
                return;
            }
            PerformanceInfo.eSHADOW_QUALITY shadowquality = PerformanceManager.Instance.CurrentEnvironmentInfo.shadowQuality;
            Light[] lights = sceneRoot.m_SceneRoot.GetComponentsInChildren<Light>(true);

            for (int i = 0; i < lights.Length; ++i)
            {
                Light shadowlight = lights[i];
                if (shadowquality == PerformanceInfo.eSHADOW_QUALITY.On)
                {
                    shadowlight.gameObject.CustomSetActive(true);
                }
                else
                {
                    shadowlight.gameObject.CustomSetActive(false);
                }
            }
        }
        protected void SetLayerMask(SceneRootEntry sceneRoot)
        {
            int hiddenLayers = PerformanceManager.Instance.CurrentEnvironmentInfo.hiddenLayers;
            Camera[] cams = sceneRoot.m_SceneRoot.GetComponentsInChildren<Camera>(true);

            int particleFXLayer = LayerMask.NameToLayer("TransparentFX");
            float[] distances = new float[sizeof(int) * 8];
            distances[particleFXLayer] = 30.0f;

            for (int i = 0; i < cams.Length; ++i)
            {
                Camera cam = cams[i];
                cam.cullingMask &= ~hiddenLayers;

                cam.layerCullSpherical = true;
                cam.layerCullDistances = distances;
            }
        }

        protected virtual void InitializeCamera()
        {

        }

        protected virtual void HudLoadComplete(bool NoError, string[] Show)
        {
            if (Show != null)
            {
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < Show.Length; ++i)
                {
                    str.Append("[" + i + "]:" + Show[i]);

                    HudRootEntry entry = HudLoadManager.GetHudRoot(Show[i]);
                    entry.Show();
                    UIHierarchyHelper.Instance.Place(entry.m_Root, UIHierarchyHelper.eUIType.HUD_Dynamic, UIAnchor.Side.Center);
                }
                EB.Debug.LogUI("【<color=#00ff00>{0}</color>】加载UI界面:<color=#00ff00>{1}</color>: result = <color=#00ff00>{2}</color>", GameFlowHotfixController.ActiveStateName, str, NoError);
            }
            else
            {
                EB.Debug.LogError(string.Format("[{0}]HudLoadComplete: There is nothing to show.", GameFlowHotfixController.ActiveStateName));
            }

            if (onLoadComplete != null)
            {
                onLoadComplete(NoError);
                onLoadComplete = null;
            }
        }

        protected virtual void SceneLoadBegin()
        {

        }

        protected virtual void SceneLoadFailed()
        {
            EB.Debug.LogError("[FlowControlAction]SceneLoadFailed: Fsm.ActiveStateName = {0}" , GameFlowHotfixController.ActiveStateName);
            UIStack.Instance.HideLoadingScreen();
            FatalError(EB.Localizer.GetString("ID_FSM_SCENE_LOAD_FAILED"));
        }

        protected virtual void SceneLoadLoading(int loaded, int total)
        {

        }

        protected virtual void SceneLoadFinished(SceneRootEntry sceneRoot)
        {
            if (sceneRoot != null)
            {
                sceneRoot.ShowLevel();
            }
            UIStack.Instance.HideLoadingScreen();
        }

        protected virtual void FatalError(string error)
        {
            SparxHub.Instance.FatalError(error);
        }

    }
}