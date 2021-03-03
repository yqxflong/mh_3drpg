using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class UIStoreController : UIControllerHotfix
    {
        //public UITabController categoryTabs;
        private UIConditionTabController categoryTabs;

        private bool isPlayedAnimation = false;
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        public List<BoxCollider> m_TopButtonsCollider = new List<BoxCollider>();

        private void InitConditionTab()
        {
            categoryTabs.TabLibPrefabs = new List<UITabControllerHotFix.TabLibEntry>();
            UITabControllerHotFix.TabLibEntry entry = new UITabControllerHotFix.TabLibEntry();

            categoryTabs.NormalTextColor = Color.white;
            categoryTabs.SelectedTextColor = Color.white;

            var mDMono = categoryTabs.mDMono;

            GameObject TabObj1 = mDMono.transform.Find("TopButtons/ButtonGrid/0_mystery/EnchantTab1").gameObject;
            GameObject PressedTabObj1 = mDMono.transform.Find("TopButtons/ButtonGrid/0_mystery/EnchantTab2").gameObject;
            GameObject GameViewObj1 = mDMono.transform.Find("BlacksmithViews/mystery").gameObject;
            entry.TabObj = TabObj1;
            entry.PressedTabObj = PressedTabObj1;
            entry.GameViewObj = GameViewObj1;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/0_mystery/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj1.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj1);
            }));

            GameObject TabObj2 = mDMono.transform.Find("TopButtons/ButtonGrid/1_alliance/ConvertTab1").gameObject;
            GameObject PressedTabObj2 = mDMono.transform.Find("TopButtons/ButtonGrid/1_alliance/ConvertTab2").gameObject;
            GameObject GameViewObj2 = mDMono.transform.Find("BlacksmithViews/alliance").gameObject;
            entry.TabObj = TabObj2;
            entry.PressedTabObj = PressedTabObj2;
            entry.GameViewObj = GameViewObj2;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/1_alliance/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj2.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj2);
            }));

            GameObject TabObj3 = mDMono.transform.Find("TopButtons/ButtonGrid/2_arena/ConvertTab1").gameObject;
            GameObject PressedTabObj3 = mDMono.transform.Find("TopButtons/ButtonGrid/2_arena/ConvertTab2").gameObject;
            GameObject GameViewObj3 = mDMono.transform.Find("BlacksmithViews/arena").gameObject;
            entry.TabObj = TabObj3;
            entry.PressedTabObj = PressedTabObj3;
            entry.GameViewObj = GameViewObj3;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/2_arena/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj3.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj3);
            }));

            GameObject TabObj4 = mDMono.transform.Find("TopButtons/ButtonGrid/3_ladder/ConvertTab1").gameObject;
            GameObject PressedTabObj4 = mDMono.transform.Find("TopButtons/ButtonGrid/3_ladder/ConvertTab2").gameObject;
            GameObject GameViewObj4 = mDMono.transform.Find("BlacksmithViews/ladder").gameObject;
            entry.TabObj = TabObj4;
            entry.PressedTabObj = PressedTabObj4;
            entry.GameViewObj = GameViewObj4;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/3_ladder/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj4.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj4);
            }));

            GameObject TabObj5 = mDMono.transform.Find("TopButtons/ButtonGrid/4_herobattle/ConvertTab1").gameObject;
            GameObject PressedTabObj5 = mDMono.transform.Find("TopButtons/ButtonGrid/4_herobattle/ConvertTab2").gameObject;
            GameObject GameViewObj5 = mDMono.transform.Find("BlacksmithViews/herobattle").gameObject;
            entry.TabObj = TabObj5;
            entry.PressedTabObj = PressedTabObj5;
            entry.GameViewObj = GameViewObj5;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/4_herobattle/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj5.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj5);
            }));

            GameObject TabObj6 = mDMono.transform.Find("TopButtons/ButtonGrid/5_nation/ConvertTab1").gameObject;
            GameObject PressedTabObj6 = mDMono.transform.Find("TopButtons/ButtonGrid/5_nation/ConvertTab2").gameObject;
            GameObject GameViewObj6 = mDMono.transform.Find("BlacksmithViews/nation").gameObject;
            entry.TabObj = TabObj6;
            entry.PressedTabObj = PressedTabObj6;
            entry.GameViewObj = GameViewObj6;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/5_nation/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj6.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj6);
            }));
            
            GameObject TabObj7 = mDMono.transform.Find("TopButtons/ButtonGrid/6_honor_arena/ConvertTab1").gameObject;
            GameObject PressedTabObj7 = mDMono.transform.Find("TopButtons/ButtonGrid/6_honor_arena/ConvertTab2").gameObject;
            GameObject GameViewObj7 = mDMono.transform.Find("BlacksmithViews/honor_arena").gameObject;
            entry.TabObj = TabObj7;
            entry.PressedTabObj = PressedTabObj7;
            entry.GameViewObj = GameViewObj7;
            entry.TabTitle = mDMono.transform.Find("TopButtons/ButtonGrid/6_honor_arena/Label").GetComponent<UILabel>();

            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj7.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj7);
            }));
        }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            categoryTabs = t.GetMonoILRComponent<UIConditionTabController>("Store/NewBlacksmithView");
            InitConditionTab();

            controller.backButton = t.GetComponent<UIButton>("UINormalFrameBG/CancelBtn");

            m_TopButtonsCollider = controller.FetchComponentList<BoxCollider>(new string[] {
	            "Store/NewBlacksmithView/TopButtons/ButtonGrid/0_mystery", "Store/NewBlacksmithView/TopButtons/ButtonGrid/1_alliance",
				"Store/NewBlacksmithView/TopButtons/ButtonGrid/2_arena", "Store/NewBlacksmithView/TopButtons/ButtonGrid/3_ladder",
				"Store/NewBlacksmithView/TopButtons/ButtonGrid/4_herobattle", "Store/NewBlacksmithView/TopButtons/ButtonGrid/5_nation",
                "Store/NewBlacksmithView/TopButtons/ButtonGrid/6_honor_arena"
			});

            controller.FindAndBindingEventTriggerEvent(
	            GetList("Store/NewBlacksmithView/TopButtons/ButtonGrid/0_mystery", "Store/NewBlacksmithView/TopButtons/ButtonGrid/0_mystery/EnchantTab1",
					"Store/NewBlacksmithView/TopButtons/ButtonGrid/1_alliance", "Store/NewBlacksmithView/TopButtons/ButtonGrid/1_alliance/ConvertTab1",
					"Store/NewBlacksmithView/TopButtons/ButtonGrid/2_arena", "Store/NewBlacksmithView/TopButtons/ButtonGrid/2_arena/ConvertTab1",
					"Store/NewBlacksmithView/TopButtons/ButtonGrid/3_ladder", "Store/NewBlacksmithView/TopButtons/ButtonGrid/3_ladder/ConvertTab1",
					"Store/NewBlacksmithView/TopButtons/ButtonGrid/4_herobattle", "Store/NewBlacksmithView/TopButtons/ButtonGrid/4_herobattle/ConvertTab1",
					"Store/NewBlacksmithView/TopButtons/ButtonGrid/5_nation", "Store/NewBlacksmithView/TopButtons/ButtonGrid/5_nation/ConvertTab1",
                    "Store/NewBlacksmithView/TopButtons/ButtonGrid/6_honor_arena", "Store/NewBlacksmithView/TopButtons/ButtonGrid/6_honor_arena/ConvertTab1"),
	            GetList(new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/0_mystery")); }),
		            new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/1_alliance")); }), 
					new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/2_arena")); }),
		            new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/3_ladder")); }),
		            new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/4_herobattle")); }),
		            new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/5_nation")); }),
                    new EventDelegate(OnLimitButtonClick), new EventDelegate(() => { ButtonFlow(t.FindEx("Store/NewBlacksmithView/TopButtons/ButtonGrid/6_honor_arena")); })
					));

            t.GetComponent<TweenPosition>("Store/NewBlacksmithView").onFinished.Add(new EventDelegate(() => { FxClipFun(); }));
        }


        public override bool IsFullscreen()
        {
            return true;
        }

        public override void Start()
    	{
    		base.Start();
        }
    
        public override IEnumerator OnAddToStack() {
    
            StartCoroutine(LoadBuddy());
            yield return base.OnAddToStack();
            yield return null;
            StartCoroutine(StartAnimation());
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            TweenPosition tweener = controller.GObjects["mAniamtionTarget"].GetComponent<TweenPosition>();
            tweener.transform.localPosition = tweener.from;
            isPlayedAnimation = false;
            categoryTabs.ClearData();
            StopAllCoroutines();
            DestroySelf();
            yield break;
        }
    
        public override void SetMenuData(object param)
        {
            string path = param as string;
            InitializeTopButtons();
            SetDiscount();
            if (string.IsNullOrEmpty(path))
            {
                path = "mystery";
            }
    
            var categoryindex = categoryTabs.TabLibPrefabs.FindIndex(tab => tab.GameViewObj != null && path.StartsWith(tab.GameViewObj.name));
            if (categoryindex < 0)
            {
                EB.Debug.LogWarning("UIStoreController: path {0} not found", path);
                return;
            }
    
            var entry = categoryTabs.TabLibPrefabs[categoryindex];
            categoryTabs.SelectTab(categoryindex);
            ButtonFlow(categoryTabs.TabLibPrefabs[categoryindex].TabObj.transform.parent);
    
            path = path.Replace(entry.GameViewObj.name, "").Trim(new char[] { '/' });
            UITabController subTabs = entry.GameViewObj.GetComponentInChildren<UITabController>();
            if (subTabs == null)
            {
                EB.Debug.LogWarning("UIStoreController: sub tab controller not found");
                return;
            }
    
            var subIndex = subTabs.TabLibPrefabs.FindIndex(tab => tab.GameViewObj != null && path.StartsWith(tab.GameViewObj.name));
            if (subIndex < 0)
            {
                EB.Debug.LogWarning("UIStoreController: sub path {0} not found", path);
                return;
            }
    
            subTabs.SelectTab(subIndex);
        }
        
        public static bool IsStoreEnable(int storeId)
        {
            var tpl = Hotfix_LT.Data.ShopTemplateManager.Instance.GetShop(storeId);
            if (tpl == null)
            {
                return true;
            }
            int level = BalanceResourceUtil.GetUserLevel();
            if (level < tpl.level_limit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    
    	IEnumerator StartAnimation()
    	{
    		yield return null;
    		if (isPlayedAnimation) yield break; ;
    		isPlayedAnimation = true;
            TweenPosition tweener = controller.GObjects["mAniamtionTarget"].GetComponent<TweenPosition>();
            tweener.ResetToBeginning();
            tweener.PlayForward();
        }
    
        private IEnumerator LoadBuddy() {
            controller.UiTextures["LobbyTexture"].gameObject.CustomSetActive(false);
            string configId = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("storeModelId");
            if (string.IsNullOrEmpty(configId)) configId = "10530";
             var character = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(configId);
            string curModeName = character.model_name;
            string configCam = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("storeModelCam");
            LobbyCameraData lobby_camera =SetCameraData(configCam);
            if (Lobby == null && Loader == null) {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                UI3DLobby.Preload(curModeName);
                yield return Loader;
                if (Loader.Success) {
    
                    Loader.Instance.transform.parent = controller.UiTextures["LobbyTexture"].transform;
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = controller.UiTextures["LobbyTexture"];
                    Lobby.SetCameraPos(lobby_camera.Position);
                    Lobby.SetCameraRot(lobby_camera.Rotation);
                    Lobby.SetCameraMode(lobby_camera.Size, lobby_camera.Orthographic);
                }
            }
            Lobby.mDMono.gameObject.SetActive(true);
            if (Lobby != null) {
                Lobby.VariantName = curModeName;
            }
            yield return null;
            TweenAlpha lobbyTextureAlpha = controller.UiTextures["LobbyTexture"].GetComponent<TweenAlpha>();
            lobbyTextureAlpha.ResetToBeginning();
            controller.UiTextures["LobbyTexture"].gameObject.CustomSetActive(true);
            lobbyTextureAlpha.PlayForward();
        }
    
        private LobbyCameraData SetCameraData(string lobbyCamera)
        {
            string cameraParam;
            if (lobbyCamera != null && lobbyCamera != "")
            {
                cameraParam = lobbyCamera;
            }
            else
            {
                cameraParam = "-0.29,2,0,20.7,5.3,0,0.6,1";
            }
    
            string[] sArray = cameraParam.Split(',');
            float[] fArray = LTUIUtil.ToFloat(sArray);
            Vector3 position = new Vector3(fArray[0], fArray[1], fArray[2]);
            Vector3 rotation = new Vector3(fArray[3], fArray[4], fArray[5]);
            float size = fArray[6];
            bool orthographic = fArray[7] != 0;
    
            Vector3 iconPosition = new Vector3(0, -6, 0);
            Vector3 iconRot = Vector3.zero;
            if (fArray.Length > 8)
            {
                iconPosition = new Vector3(fArray[8], fArray[9], fArray[10]);
                iconRot = new Vector3(fArray[11], fArray[12], fArray[13]);
            }
    
            LobbyCameraData lobbyCameraData = new LobbyCameraData
            {
                Orthographic = orthographic,
                Position = position,
                Rotation = rotation,
                Size = size,
                IconPosition = iconPosition,
                IconRotation = iconRot
            };
            return lobbyCameraData;
        }
    
        public void ButtonFlow(Transform ClickButton)
        {
            Vector3 toPosition = ClickButton.localPosition;
            toPosition.y = -80;
            controller.TweenScales["FlowTween"].ResetToBeginning();
            controller.TweenScales["FlowTween"].transform.localPosition = toPosition;
            controller.TweenScales["FlowTween"].PlayForward();
            controller.TweenScales["FlowTween"].transform.GetChild(0).GetComponent<UILabel>().text = GetStoreName(ClickButton.name);
        }
    
        private string GetStoreName(string BtnName)
        {
    		if (BtnName.Contains("0")) return EB.Localizer.GetString("ID_STORE_NAME_MYSTERY");
    		else if (BtnName.Contains("1")) return EB.Localizer.GetString("ID_ALLIANCE");
    		else if (BtnName.Contains("2")) return EB.Localizer.GetString("ID_ARENA");
    		else if (BtnName.Contains("3")) return EB.Localizer.GetString("ID_LADDER_NAME");
    		else if (BtnName.Contains("4")) return EB.Localizer.GetString("ID_HERO_BATTLE_NAME");
    		else if (BtnName.Contains("5")) return EB.Localizer.GetString("ID_NATION_NAME");
            else if (BtnName.Contains("6")) return EB.Localizer.GetString("ID_HONOR_ARENA");
    		else return EB.Localizer.GetString("ID_ALLIANCE_CURRENT");
    	}
    
        public void InitializeTopButtons() {
            bool[] openCondition = new bool[7];
            openCondition[0] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10023).IsConditionOK();
            openCondition[1] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10033).IsConditionOK();
            openCondition[2] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10024).IsConditionOK();
            openCondition[3] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10032).IsConditionOK();
            openCondition[4] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10066).IsConditionOK();
            openCondition[5] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10067).IsConditionOK();
            //TODO 荣耀角斗场开启
            openCondition[6] = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10091).IsConditionOK();
            for(int i = 0; i < 7; i++) {
                m_TopButtonsCollider[i].enabled = !openCondition[i];
                m_TopButtonsCollider[i].gameObject.CustomSetActive(openCondition[i]);
            }
            controller.UiGrids["iGrid"].Reposition();
        }
    
        public void OnLimitButtonClick() {
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEVEL_NOT_ENOUGH"));
        }
    
        private void SetDiscount()
        {
            float discount = VIPTemplateManager.Instance.GetVIPPercent(VIPPrivilegeKey.ShopDiscount);
            controller.UiLabels["DiscountLab"].transform.parent.gameObject.CustomSetActive(discount < 1);
            if (discount < 1)
            {
                controller.UiLabels["DiscountLab"].text = string.Format(EB.Localizer.GetString("ID_STORE_DISCOUNT"),
                    EB.Localizer.GetDiscountChange(discount));
            }
        }
    
    
        public void FxClipFun()
        {
            EffectClip[] clips = controller.GObjects["mAniamtionTarget"].transform.GetComponentsInChildren<EffectClip>();
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].Init();
            }
        }
    }
}
