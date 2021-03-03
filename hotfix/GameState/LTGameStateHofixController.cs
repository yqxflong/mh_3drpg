using Hotfix_LT.UI;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.GameState
{
    public class LTGameStateHofixController : DynamicMonoHotfix, IHotfixUpdate
    {
        private eGameState CurGameState;
        private List<GameStateUnit> GameStateList;
        public override void Awake()
        {
            EB.Debug.Log("Creat LTGameStateHofixController Awake!");
            Hotfix_LT.Messenger.AddListener<eGameState>("GameStateChangeOnStart", OnStart);
            Hotfix_LT.Messenger.AddListener<eGameState>("GameStateChangeOnEnd", OnEnd);

            GameStateList = new List<GameStateUnit>();
            GameStateList.Add(new LTGameStateStart());
            GameStateList.Add(new LTGameStateLogin());
            GameStateList.Add(new LTGameStateLoadGame());

            StartCoroutine(new LTGameStateDownload().Start());
            RegisterMonoUpdater();
        }

        float timer = 0;
        public void Update()
        {
            if (timer < 0.5f)//每0.5秒更新一次
            {
                timer += Time.deltaTime;
                return;
            }
            timer = 0;
            if (CurGameState == eGameState.LoadGame)
            {
                PlayerManagerForFilter.Process();
                NpcManager.Process();
            }
        }

        public override void OnDestroy()
        {
            ErasureMonoUpdater();
            Hotfix_LT.Messenger.RemoveListener<eGameState>("GameStateChangeOnStart", OnStart);
            Hotfix_LT.Messenger.RemoveListener<eGameState>("GameStateChangeOnEnd", OnEnd);
            EB.Debug.Log("LTGameStateHofixController OnDestroy!");
            base.OnDestroy();
        }

        private Coroutine StartCor = null;
        private void OnStart(eGameState gameState)
        {
            for (int i = 0; i < GameStateList.Count; ++i)
            {
                if (GameStateList[i].mGameState == gameState)
                {
                    EB.Debug.Log("GameStateList.OnStart =====>{0}", gameState);
                    if (StartCor != null)
                    {
                        StopCoroutine(StartCor);
                        StartCor = null;
                    }
                    StartCor = StartCoroutine(GameStateList[i].Start());
                    CurGameState = gameState;
                    break;
                }
            }
        }

        private void OnEnd(eGameState gameState)
        {
            CurGameState = eGameState.DebugStartScreen;
            for (int i = 0; i < GameStateList.Count; ++i)
            {
                if (GameStateList[i].mGameState == gameState)
                {
                    EB.Debug.Log("GameStateList.OnEnd =====>{0}", gameState);
                    GameStateList[i].End();
                    break;
                }
            }
        }

    }

    public class GameStateUnit
    {
        public virtual eGameState mGameState { get { return eGameState.DebugStartScreen; }}
        public virtual IEnumerator Start() { yield break; }
        public virtual void End() { }

        public static GameStateManager Mgr
        {
            get
            {
                return GameStateManager.Instance;
            }
        }

        protected IEnumerator ShowNetworkRetryDialog()
        {
            bool proceed = false;

            ShowNetworkRetryDialog(delegate () { proceed = true; });

            while (!proceed)
            {
                yield return null;
            }
        }

        protected void ShowNetworkRetryDialog(System.Action retry)
        {
            UIStack.Instance.GetDialog("Confirm",EB.Localizer.GetString("ID_SPARX_NETWORK_RETYR"), delegate (eUIDialogueButtons button, UIDialogeOption opt)
            {
                if (button == eUIDialogueButtons.Accept)
                {
                    retry();
                }
                else
                {
                    Application.Quit();
                }
            });
        }

        protected virtual void ShowLoadingScreen(System.Action onReady)
        {
            UIStack.Instance.ShowLoadingScreen(onReady, true, false);
        }

        protected virtual IEnumerator ShowLoadingScreen()
        {
            bool ready = false;
            UIStack.Instance.ShowLoadingScreen(() =>
            {
                ready = true;
            }, true, false);

            while (!ready)
            {
                yield return null;
            }
        }
    }
}
