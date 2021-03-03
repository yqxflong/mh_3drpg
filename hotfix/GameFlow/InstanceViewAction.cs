using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 副本状态
    /// </summary>
    public class InstanceViewAction : FlowControlAction
    {
        public override void OnAwake()
        {
            base.OnAwake();
            Name = "InstanceView";
        }

        public override void OnEnter()
        {
            LoadingLogic.AddCustomProgress(10);
            base.OnEnter();
            LTInstanceMapModel.Instance.LoadInstanceSpriteAtlas();
            FusionAudio.PostGlobalMusicEvent("MUS_FuBen", true);
            if (LTMainHudManager.Instance.GetFromFirstBattleType()) LTMainHudManager.Instance.SetsFromFirstBattleType(false);
            Instance.LTInstanceConfigManager.LoadDatas();
        }

        public override void OnExit()
        {
            base.OnExit();
            LTInstanceMapModel.Instance.UnloadInstanceSpriteAtlas();
            LTInstanceMapModel.Instance.ClearChapterData();
            Instance.LTInstanceConfigManager.UnloadDatas();
        }

        protected override void HudLoadComplete(bool NoError, string[] Show)
        {
            base.HudLoadComplete(NoError, Show);
            Hotfix_LT.Messenger.Raise(EventName.InstanceViewActionHudLoad);
        }
        
        protected override void SceneLoadFinished(SceneRootEntry sceneRoot)
        {
            LoadingLogic.AddCustomProgress(15);
            if (sceneRoot != null)
            {
                sceneRoot.ShowLevel();
            }
            SceneLogic.SceneState = SceneLogic.eSceneState.SceneLoop;
            //UIStack.Instance.HideLoadingScreen(false);
        }
    }
}
