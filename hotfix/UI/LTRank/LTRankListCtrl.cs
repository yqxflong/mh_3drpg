using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//UIRankController
namespace Hotfix_LT.UI
{
    public class LTRankListCtrl : UIControllerHotfix
    {
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        UIButton HotfixBtn3;
        UIButton HotfixBtn4;

        UIConditionTabController categoryTabs;
        private GameObject DefaultModelGO;
        GameObject CreateModelFx;
        UITexture LobbyTexture;
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        protected string ModelName = null;
        private TweenAlpha mTweenAlpha;


        public override void Awake()
        {
            base.Awake();
          
            if (categoryTabs == null)
            {
                categoryTabs = controller.transform.GetMonoILRComponent<UIConditionTabController>("ContentView");
                Init();
            }

            CreateModelFx = UnityHelper.FindTheChildNode(controller.gameObject, "Fx").gameObject;
            DefaultModelGO = UnityHelper.FindTheChildNode(controller.gameObject, "Default").gameObject;
            LobbyTexture = UnityHelper.GetTheChildNodeComponetScripts<UITexture>(controller.gameObject, "Texture");
            HotfixBtn0 = controller.transform.Find("UINormalFrameBG/CancelBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnCancelButtonClick));
            
        }
        

        private void Init()
        {
            categoryTabs.TabLibPrefabs = new List<UITabControllerHotFix.TabLibEntry>();
            UITabControllerHotFix.TabLibEntry entry = new UITabControllerHotFix.TabLibEntry();
            
            Transform mDMono =categoryTabs.mDMono.transform;
            
            GameObject TabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_Level/Tab1").gameObject;
            GameObject PressedTabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_Level/Tab2").gameObject;
            GameObject GameViewObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_Level/Level").gameObject;
            entry.TabObj = TabObj1;
            entry.PressedTabObj = PressedTabObj1;
            entry.GameViewObj = GameViewObj1;
            categoryTabs.TabLibPrefabs.Add(entry);
            
            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj1);
            }));
            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj1.GetMonoILRComponent<PersonalLevelRankController>().ClickTitleRefreshGrid();
            }));

            GameObject TabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Legion/Tab1").gameObject;
            GameObject PressedTabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Legion/Tab2").gameObject;
            GameObject GameViewObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Legion/Alliance").gameObject;
            entry.TabObj = TabObj2;
            entry.PressedTabObj = PressedTabObj2;
            entry.GameViewObj = GameViewObj2;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj2);
            }));
            TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj2.GetMonoILRComponent<AllianceLevelRankController>().ClickTitleRefreshGrid();
            }));

            GameObject TabObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/2_Arena/Tab1").gameObject;
            GameObject PressedTabObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/2_Arena/Tab2").gameObject;
            GameObject GameViewObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/2_Arena/Arena").gameObject;
            entry.TabObj = TabObj3;
            entry.PressedTabObj = PressedTabObj3;
            entry.GameViewObj = GameViewObj3;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj3.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj3);
            }));
            TabObj3.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                (GameViewObj3.GetMonoILRComponent<PersonalArenaRankController>()).ClickTitleRefreshGrid();
            }));

            GameObject TabObj4 = mDMono.transform.Find("UpButtons/ButtonGrid/3_Ladder/Tab1").gameObject;
            GameObject PressedTabObj4 = mDMono.transform.Find("UpButtons/ButtonGrid/3_Ladder/Tab2").gameObject;
            GameObject GameViewObj4 = mDMono.transform.Find("UpButtons/ButtonGrid/3_Ladder/Ladder").gameObject;
            entry.TabObj = TabObj4;
            entry.PressedTabObj = PressedTabObj4;
            entry.GameViewObj = GameViewObj4;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj4.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj4);
            }));
            TabObj4.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj4.GetMonoILRComponent<PersonalLadderRankController>().ClickTitleRefreshGrid();
            }));

            GameObject TabObj5 = mDMono.transform.Find("UpButtons/ButtonGrid/4_ChallengeInstance/Tab1").gameObject;
            GameObject PressedTabObj5 =
                mDMono.transform.Find("UpButtons/ButtonGrid/4_ChallengeInstance/Tab2").gameObject;
            GameObject GameViewObj5 = mDMono.transform
                .Find("UpButtons/ButtonGrid/4_ChallengeInstance/ChallengeInstance").gameObject;
            entry.TabObj = TabObj5;
            entry.PressedTabObj = PressedTabObj5;
            entry.GameViewObj = GameViewObj5;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj5.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj5);
            }));
            TabObj5.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj5.GetMonoILRComponent<PersonalChallengeInstanceRankController>().ClickTitleRefreshGrid();
            }));

            GameObject TabObj6 = mDMono.transform.Find("UpButtons/ButtonGrid/5_UltimateChallenge/Tab1").gameObject;
            GameObject PressedTabObj6 =
                mDMono.transform.Find("UpButtons/ButtonGrid/5_UltimateChallenge/Tab2").gameObject;
            GameObject GameViewObj6 = mDMono.transform
                .Find("UpButtons/ButtonGrid/5_UltimateChallenge/UltimateChallenge").gameObject;
            entry.TabObj = TabObj6;
            entry.PressedTabObj = PressedTabObj6;
            entry.GameViewObj = GameViewObj6;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj6.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj6);
            }));
            TabObj6.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj6.GetMonoILRComponent<InfiniteChallengeRankController>().ClickTitleRefreshGrid();
            }));
        }

        public override bool IsFullscreen() { return true; }

        public override IEnumerator OnRemoveFromStack()
        {
            categoryTabs.ClearData();
            DestroyModel();
            DestroySelf();
            yield break;
        }

        public override void SetMenuData(object param)
        {
            string path = param as string;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (categoryTabs == null)
            {
                categoryTabs = controller.transform.GetMonoILRComponent<UIConditionTabController>("ContentView");
            }

            var categoryindex = categoryTabs.TabLibPrefabs.FindIndex(tab => tab.GameViewObj != null && path.StartsWith(tab.GameViewObj.name));
            if (categoryindex < 0)
            {
                EB.Debug.LogWarning("UIRankController: path {0} not found", path);
                return;
            }

            List<EventDelegate> eventDelegates = categoryTabs.TabLibPrefabs[categoryindex].TabObj.GetComponent<UIEventTrigger>().onPress;
            for (int i = 0; i < eventDelegates.Count; i++)
            {
                eventDelegates[i].Execute();
            }
        }
        
        public void OnSwithModel(string model)
        {
            if (string.IsNullOrEmpty(model))
            {
                DefaultModelGO.gameObject.CustomSetActive(true);
                LobbyTexture.gameObject.CustomSetActive(false);
            }
            else
            {
                DefaultModelGO.gameObject.CustomSetActive(false);
                LobbyTexture.gameObject.CustomSetActive(true);
                EB.Coroutines.Run(CreateBuddyModel(model));
            }
        }

        private IEnumerator CreateBuddyModel(string modelName)
        {
            CreateModelFx.CustomSetActive(false);
            yield return new WaitForEndOfFrame();
            yield return null;
            CreateModelFx.CustomSetActive(true);
            if (mTweenAlpha == null)
            {
                if (LobbyTexture == null) yield break;
                mTweenAlpha = LobbyTexture.GetComponent<TweenAlpha>();
            }
            mTweenAlpha.ResetToBeginning();
            mTweenAlpha.PlayForward();
            if (string.IsNullOrEmpty(modelName) || ModelName == modelName)
            {
                yield break;
            }
            ModelName = modelName;
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);

                UI3DLobby.Preload(ModelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.parent = LobbyTexture.transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = LobbyTexture;
                    Lobby.SetCameraMode(2, true);
                }
            }

            if (Lobby != null)
            {
                Lobby.VariantName = ModelName;
            }
            yield break;
        }

        protected void DestroyModel()
        {
            if (Lobby != null)
            {
                Object.Destroy(Lobby.mDMono.gameObject, 0.0f);
            }
            if (Loader != null)
            {
                EB.Assets.UnloadAssetByName("UI3DLobby", false);
            }
            Lobby = null;
            Loader = null;
            ModelName = null;
        }
    }
}