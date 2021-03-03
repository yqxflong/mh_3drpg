using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.GameState
{
    public class LTGameStateDownload : GameStateUnit
    {
        public override eGameState mGameState { get { return eGameState.Download; } }

        public override IEnumerator Start()
        {
            yield return EB.Assets.LoadAsync("_GameAssets/Res/Prefabs/UIPrefabs/MainHudUI", typeof(GameObject), o =>
            {
                if (o)
                {
                    var mainhud= GameObject.Instantiate(o) as GameObject;
                    mainhud.name = "MainHudUI";
                    GameObject camera = GameObject.Find("DownloadHudUI/OrthoCam");
                    if (camera != null)
                    {
                        camera.transform.SetParent(mainhud.transform);
                    }
                }
            });

            //切到GameStateStart状态 transform to GameStateStart
            Mgr.SetGameState<GameStateStart>();

            //加载UIHelper全局的ui预设 Load UIHelper
            UIStack.Instance.LoadUIHelper();
            //加载StoryPrefab
            LoadStoryPrefab();
            yield break;
        }

        private void LoadStoryPrefab()
        {
            EB.Assets.LoadAsync("Bundles/Prefab/StoryPrefab", typeof(GameObject), o => {
            if (o != null)
                {
                    GameObject.Instantiate(o).name = "StoryPrefab";
                }
            });
        }
    }
}