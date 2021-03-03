using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using MoveEditor;
using Debug = EB.Debug;
using Object = UnityEngine.Object;

namespace Hotfix_LT.UI
{
    public enum PartnerGrade
    {
        ALL = 0,
        N = 1,
        R = 2,
        SR = 3,
        SSR = 4,
        UR = 5,
    }

    public enum HudType
    {
        EMPTY = -1,
        LEFT = 0,
        RIGHT,
    }

    public class LTPartnerHudController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TableScroll = t.GetMonoILRComponent<LTPartnerListTableScroll>("Panel/Center/Left/PartnerList/Container/Placeholder/Grid");

            ChangeController = t.GetMonoILRComponent<LTPartnerChangeController>("Panel/Mid");
            StarController = t.GetMonoILRComponent<LTPartnerStarController>("Panel/Center/Mid/StarList");
            artifactItemController = t.GetMonoILRComponent<ArtifactItemController>("Panel/Mid/ArtifactBtn");
            indexin = 0;
            controller.backButton = controller.UiButtons["BackButton"];

			EquipItem = new LTPartnerEquipItemController[6];
            EquipItem[0] = t.GetMonoILRComponentByClassPath<LTPartnerEquipItemController>("Panel/Mid/Equip/item", "Hotfix_LT.UI.LTPartnerEquipItemController");
            EquipItem[1] = t.GetMonoILRComponentByClassPath<LTPartnerEquipItemController>("Panel/Mid/Equip/item2", "Hotfix_LT.UI.LTPartnerEquipItemController");
            EquipItem[2] = t.GetMonoILRComponentByClassPath<LTPartnerEquipItemController>("Panel/Mid/Equip/item3", "Hotfix_LT.UI.LTPartnerEquipItemController");
            EquipItem[3] = t.GetMonoILRComponentByClassPath<LTPartnerEquipItemController>("Panel/Mid/Equip/item4", "Hotfix_LT.UI.LTPartnerEquipItemController");
            EquipItem[4] = t.GetMonoILRComponentByClassPath<LTPartnerEquipItemController>("Panel/Mid/Equip/item5", "Hotfix_LT.UI.LTPartnerEquipItemController");
            EquipItem[5] = t.GetMonoILRComponentByClassPath<LTPartnerEquipItemController>("Panel/Mid/Equip/item6", "Hotfix_LT.UI.LTPartnerEquipItemController");
            
            
            controller.BindingBtnEvent(new List<string>() { "TypeBtn0", "TypeBtn1", "TypeBtn2", "TypeBtn3", "TypeBtn4", "TypeBtn5" },
				new List<EventDelegate>()
				{
					new EventDelegate(() => OnPartnerTypeBtnClick("TypeBtn0")),
					new EventDelegate(() => OnPartnerTypeBtnClick("TypeBtn1")),
					new EventDelegate(() => OnPartnerTypeBtnClick("TypeBtn2")),
					new EventDelegate(() => OnPartnerTypeBtnClick("TypeBtn3")),
					new EventDelegate(() => OnPartnerTypeBtnClick("TypeBtn4")),
                    new EventDelegate(() => OnPartnerTypeBtnClick("TypeBtn5")),
                }
			);

			controller.BindingBtnEvent(new List<string>() { "SelectBtn", "FormationBtn", "PartnerTransBtn", "SwitchRightBtn", "SwitchLeftBtn", "Partnerwiki", "EquipBtn", "CultivateBtn", "PartnerStrategy" },
				new List<EventDelegate>()
				{
					new EventDelegate(OnSelectGradeBtnClick),
					new EventDelegate(OnBattleReadyBtnClick),
					new EventDelegate(OnPartnerTransBtnClick),
					new EventDelegate(OnSwitchRightBtnClick),
					new EventDelegate(OnSwitchLeftBtnClick),
					new EventDelegate(OnClickPartnerWikiBtn),
					new EventDelegate(OnPartnerEquipmentBtnClick),
					new EventDelegate(()=>OnLoadCulView(true,-1)),
                    new EventDelegate(OnClickPartnerStrategyBtn),
                }
			);

			controller.BindingCoolTriggerEvent(new List<string>() { "UnEquipAll", "Leader" }, new List<EventDelegate>() {
				new EventDelegate(() => OnFullEquipClick(t.GetComponent<UISprite>("Panel/Mid/UnEquipAll/Icon"))), new EventDelegate(OnSetLeaderBtnClick)
			});
			controller.UiEventTriggers["GradeBtn"].onClick.Add(new EventDelegate(OnSelectGradeBtnClick));

			Hotfix_LT.Messenger.AddListener<int>(Hotfix_LT.EventName.OnPartnerSelect, OnPartnerSelectFunc);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerLevelUpSucc, OnPartnerLevelUpSuccFunc);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerUpGradeSucc, OnPartnerUpGradeSuccFunc);
            Hotfix_LT.Messenger.AddListener<int>(Hotfix_LT.EventName.OnPartnerStarUpSucc, OnPartnerStarUpSuccFunc);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnParnerSkillChange, PartnerCultivateRP);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerTopLevelUpSucc, OnPartnerUpGradeSuccFunc);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnParnerGuardChange, PartnerCultivateRP);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnPartnerEquipChangeFunc);
            Hotfix_LT.Messenger.AddListener<int>(Hotfix_LT.EventName.OnPartnerSummonSucc, OnPartnerSummonSuccFunc);
            Hotfix_LT.Messenger.AddListener<LTPartnerData>(Hotfix_LT.EventName.OnPartnerAwakenSucc, OnPartnerAwakenSuccFunc);
            Hotfix_LT.Messenger.AddListener<int>(Hotfix_LT.EventName.OnPartnerSkinSelect, OnPartnerSkinSelect);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.PartnerWikiRP, RefreshPartnerWikiRP);
            Hotfix_LT.Messenger.AddListener<int,bool>(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, onCombatPowerUpdate);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.OnPartnerSelect, OnPartnerSelectFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerLevelUpSucc, OnPartnerLevelUpSuccFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerUpGradeSucc, OnPartnerUpGradeSuccFunc);
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.OnPartnerStarUpSucc, OnPartnerStarUpSuccFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnPartnerEquipChangeFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnParnerSkillChange, PartnerCultivateRP);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerTopLevelUpSucc, OnPartnerUpGradeSuccFunc);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnParnerGuardChange, PartnerCultivateRP);
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.OnPartnerSummonSucc, OnPartnerSummonSuccFunc);
            Hotfix_LT.Messenger.RemoveListener<LTPartnerData>(Hotfix_LT.EventName.OnPartnerAwakenSucc, OnPartnerAwakenSuccFunc);
            Hotfix_LT.Messenger.RemoveListener<int>(Hotfix_LT.EventName.OnPartnerSkinSelect, OnPartnerSkinSelect);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.PartnerWikiRP, RefreshPartnerWikiRP);
            Hotfix_LT.Messenger.RemoveListener<int, bool>(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, onCombatPowerUpdate);          

        }

        public override bool IsFullscreen()
        {
            return true;
        }
    
        public enum HudType
        {
            EMPTY = -1,
            LEFT = 0,
            RIGHT,
        }
    
        private HudType curHudType = HudType.EMPTY;
        public HudType CurHudType
        {
            get
            {
                return curHudType;
            }
            set
            {
                curHudType = value;
                SetHudType();
            }
        }
    
        private PartnerGrade curPartnerGrade = PartnerGrade.ALL;
        public PartnerGrade CurPartnerGrade
        {
            get
            {
                return curPartnerGrade;
            }
    
            set
            {
                curPartnerGrade = value;
                SetPartnerType();
                SetSelectGradeState();
            }
        }
    
        public LTPartnerListTableScroll TableScroll;

		//public GameObject PartnerCulObj;
		private GameObject m_PartnerCulObj = null;
		private LTPartnerCultivateController CultivateController;//脚本在实例化后获取
		private TweenAlpha TweenAlphaRight; //同上
		private static LTPartnerData CacheData;
		//名字
		public LTPartnerChangeController ChangeController;
		private bool isOpenEuipPanel;
		private bool gotoDevelop;
		private bool isStarUp;
		private bool isUpgrade;
		private bool isDropComeBack;
		private bool isSelectSkill;
		private int UpgradeType;

		public bool IsSelectSkill
        {
            get { return isSelectSkill; }
            set
            {
                isSelectSkill = value;
                SetEquipOrSkill();
            }
    
        }

        private LTPartnerData curSelectPartner;
        public LTPartnerData CurSelectPartner
        {
            get
            {
                return curSelectPartner;
            }
            set
            {
                curSelectPartner = value;
                CacheData = value;

                if (curSelectPartner != null)
                {
                    LTPartnerDataManager.Instance.DropSelectPartnerId = value.StatId;
                }

                SetPartnerSelect();

                if (controller != null && curSelectPartner != null)
                {
                    LTUIUtil.SetText(controller.UiLabels["CombatPower"], curSelectPartner.powerData.curPower.ToString());
                }
            }
        }
        public void onCombatPowerUpdate(int type,bool isshow = true)
        {
            curSelectPartner?.powerData.OnValueChanged(curSelectPartner, isshow, (PowerData.RefreshType)type);
            LTUIUtil.SetText(controller.UiLabels["CombatPower"], curSelectPartner.powerData.curPower.ToString());
            if (CurSelectPartner?.IsGoIntoBattle == true) LTFormationDataManager.OnRefreshMainTeamPower(false);
        }


        private List<LTPartnerData> partnerDataList = new List<LTPartnerData>();
        //用于筛选上阵伙伴
        private LTPartnerData Battledata;
        private List<LTPartnerData> BattlepartnerDataList = new List<LTPartnerData>();

        public override void SetMenuData(object data)
        {            
            isInitLobby = true;
            if (data != null)
            {
                if (data is HudType && (HudType)data == HudType.RIGHT)
                {
                    PlayerTweenPosition(true);
                    PlayerMidTweenAlpha(true);
                }
                else if (data is string)
                {
                    if (data.ToString() == "OpenEquip")
                    {
                        isOpenEuipPanel = true;
                    }
                    else if (data.ToString() == "Develop")
                    {
                        //PlayerTweenPosition(true);
                        //PlayerMidTweenAlpha(true);
                        gotoDevelop = true;
                    }
                    else if (data.ToString() == "Develop_StarUp")
                    {
                        //PlayerTweenPosition(true);
                        //PlayerMidTweenAlpha(true);
                        gotoDevelop = true;
                        isStarUp = true;
                    }
                    else if (data.ToString().Contains("Develop_Upgrade"))
                    {
                        //PlayerTweenPosition(true);
                        //PlayerMidTweenAlpha(true);
                        gotoDevelop = true;
                        isUpgrade = true;
                        string temp = data.ToString().Remove(0, 15);
    
                        //设置进阶提示的内容，其他时走0
                        if (!string.IsNullOrEmpty(temp) && temp.Length >= 2)
                        {
                            UpgradeType = int.Parse(temp.Substring(1, 1));
                        }
                        else
                        {
                            UpgradeType = 0;
    
                        }
    
                    }
                }
            }
            else if (LTPartnerDataManager.Instance.DropSelectPartnerId != 0 && UITooltipManager.Instance.curTemplateid != null)
            {
                PlayerTweenPosition(true);
                PlayerMidTweenAlpha(true);
                isDropComeBack = true;
            }
            if (Lobby != null) Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PartnerWikiRP);
        }
    
        public override void StartBootFlash()
        {
	        SetCurrentPanelAlpha(1);

			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int i = 0; i < tweeners.Length; i++)
            {
                tweeners[i].tweenFactor = 0;
                tweeners[i].PlayForward();
            }
        }
    
        public override void OnFocus()
        {
            base.OnFocus();
            for (int i = 0; i < EquipItem.Length; i++)
            {
                EquipItem[i].AddFxCycleCallBack();
            }
            //不在此处处理
            //if(CultivateController.CurType== LTPartnerCultivateController.CultivateType.StarUp) CultivateController.UpdataChipCount();
            if (isOpenBattleReadyUI)
            {
                InitPartnerList(false);
                isOpenBattleReadyUI = false;
            }
            if (Lobby != null) Lobby.mDMono.gameObject.CustomSetActive(true);
            //PartnerCultivateRP();
            //if (CultivateController.ChipTransController.gameObject.activeSelf) CultivateController.ChipTransController.RefreshUIInfo();
        }

        public override void OnBlur()
        {
			controller.HasPlayedTween = false;
        }

        public override IEnumerator OnAddToStack()
        {
            GlobalMenuManager.Instance.PushCache("LTPartnerHud");
            var coroutine = EB.Coroutines.Run(base.OnAddToStack());
            LTPartnerDataManager.Instance.InitPartnerData();
            LTDrawCardLookupController.DrawType = DrawCardType.wiki;
            CurPartnerGrade = PartnerGrade.ALL;
            if (CurHudType == HudType.EMPTY)
            {
                CurHudType = HudType.LEFT;
            }
            yield return null;
            if (isOpenEuipPanel)
            {               
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, partnerDataList.Find(m => m.IsGoIntoBattle).StatId);
                OnPartnerEquipmentBtnClick();
            }
            else if (gotoDevelop)
            {
                //升星升阶需要排除满星满阶伙伴
                //OnPartnerSelect evt = new OnPartnerSelect(partnerDataList.Find(m => m.IsGoIntoBattle).StatId);
                //EventManager.instance.Raise(evt);
                bool isHaveCanCulvatePartner = false;//防止相关培养出现全满而无法实例化模型的情况
                if (isStarUp)
                {
                    for (int i = 0; i < partnerDataList.Count; i++)
                    {
                        if (partnerDataList[i].HeroId > 0 && partnerDataList[i].Star < LTPartnerConfig.MAX_STAR)
                        {
                            CurSelectPartner = partnerDataList[i];
                            isHaveCanCulvatePartner = true;
                            break;
                        }
                    }
                    if (!isHaveCanCulvatePartner) { CurSelectPartner = partnerDataList.Find(m => m.IsGoIntoBattle); }
                    OnLoadCulView(true,2);
                    //CultivateController.OnTabBtnClick(CultivateController.TabBtnList[2]);
                }
                else if (isUpgrade)
                {
                    if (UpgradeType == 0)//伙伴进阶,选择上阵伙伴中的未满级最低伙伴
                    {
    
                        BattlepartnerDataList = partnerDataList.FindAll(m => m.IsGoIntoBattle);
                        if (BattlepartnerDataList.Count > 0)
                        {
                            Battledata = BattlepartnerDataList[0];
                            for (int i = 1; i < BattlepartnerDataList.Count; i++)
                            {
                                if (BattlepartnerDataList[i].UpGradeId < Battledata.UpGradeId)
                                {
                                    Battledata = BattlepartnerDataList[i];
                                }
                            }
                            if (Battledata.UpGradeId < LTPartnerConfig.MAX_GRADE_LEVEL)
                            {
                                CurSelectPartner = Battledata;
                                isHaveCanCulvatePartner = true;
                            }
                            else
                            {
                                for (int i = 0; i < partnerDataList.Count; i++)
                                {
                                    if (partnerDataList[i].HeroId > 0 && partnerDataList[i].UpGradeId < LTPartnerConfig.MAX_GRADE_LEVEL)
                                    {
                                        CurSelectPartner = partnerDataList[i];
                                        isHaveCanCulvatePartner = true;
                                        break;
                                    }
                                }
                            }
    
                        }
    
                    }
                    else if (UpgradeType == 1)//快速获取材料返回
                    {
                        if (CacheData != null)
                        {
                            CurSelectPartner = CacheData;
                            isHaveCanCulvatePartner = true;
                        }
    
                    }
                    else//特殊进阶等级跳转
                    {
    
                        BattlepartnerDataList = partnerDataList.FindAll(m => m.IsGoIntoBattle);
                        if (BattlepartnerDataList.Count > 0)
                        {
                            Battledata = BattlepartnerDataList[0];
                            for (int i = 1; i < BattlepartnerDataList.Count; i++)
                            {
                                if (BattlepartnerDataList[i].UpGradeId < Battledata.UpGradeId)
                                {
                                    Battledata = BattlepartnerDataList[i];
                                }
                            }
                            if (Battledata.UpGradeId < LTPartnerConfig.MAX_GRADE_LEVEL && Battledata.UpGradeId < UpgradeType / 3)
                            {
                                CurSelectPartner = Battledata;
                                isHaveCanCulvatePartner = true;
                            }
                            else
                            {
                                for (int i = 0; i < partnerDataList.Count; i++)
                                {
                                    if (partnerDataList[i].HeroId > 0 && partnerDataList[i].UpGradeId < UpgradeType / 3)
                                    {
                                        CurSelectPartner = partnerDataList[i];
                                        isHaveCanCulvatePartner = true;
                                        break;
                                    }
                                }
                            }
    
                        }
                        if (!isHaveCanCulvatePartner)
                        {
                            for (int i = 0; i < partnerDataList.Count; i++)
                            {
                                if (partnerDataList[i].HeroId > 0 && partnerDataList[i].UpGradeId < LTPartnerConfig.MAX_GRADE_LEVEL)
                                {
                                    CurSelectPartner = partnerDataList[i];
                                    isHaveCanCulvatePartner = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isHaveCanCulvatePartner) { CurSelectPartner = partnerDataList.Find(m => m.IsGoIntoBattle); }
                    OnLoadCulView(true, 1);
                   ;
                }
                else//升级
                {
                    for (int i = 0; i < partnerDataList.Count; i++)
                    {
                        if (partnerDataList[i].HeroId > 0 && partnerDataList[i].Level < LTPartnerConfig.MAX_LEVEL)
                        {
                            CurSelectPartner = partnerDataList[i];
                            isHaveCanCulvatePartner = true;
                            break;
                        }
                    }
                    if (!isHaveCanCulvatePartner) { CurSelectPartner = partnerDataList.Find(m => m.IsGoIntoBattle); }
                    OnLoadCulView(true, 0);
                    //CultivateController.OnTabBtnClick(CultivateController.TabBtnList[0]);
    
                }
    
            }
            else if (isDropComeBack)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, LTPartnerDataManager.Instance.DropSelectPartnerId);
                OnLoadCulView(true,1);
                //CultivateController.OnTabBtnClick(CultivateController.TabBtnList[1]);
                //CultivateController.TabBtnList[1].OnClickAction();
                ILRTimerManager.instance.AddTimer(1500, 1, delegate
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, LTPartnerDataManager.Instance.DropSelectPartnerId);
                    TableMoveTo(GetParnterIndex(LTPartnerDataManager.Instance.DropSelectPartnerId), false);
                });
            }
            else
            {
                int StatId = 0;
                if (GuideNodeManager.partnerStatID != 0)
                {
                    int statId = GuideNodeManager.partnerStatID;
                    GuideNodeManager.partnerStatID = 0;
                    for (int i = 0; i < partnerDataList.Count; i++)
                    {
                        if (partnerDataList[i].Level > 0)
                        {
                            if (StatId == 0 || partnerDataList[i].StatId == statId) StatId = partnerDataList[i].StatId;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < partnerDataList.Count; i++)
                    {
                        if (partnerDataList[i].Level > 0)
                        {
                            StatId = partnerDataList[i].StatId;
                            break;
                        }
                    }
                }
                if (StatId != 0)
                {
                    //先设置自己的  防止发广播错乱
                    CurSelectPartner = LTPartnerDataManager.Instance.GetPartnerByStatId(StatId);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, StatId);
                }
            }
    
            TableScroll.SetItemDatas(partnerDataList);
            if (GuideNodeManager.IsGuide) OnLoadCulView(false,0);//引导需要预加载伙伴培养界面
            yield return coroutine;
            UITooltipManager.Instance.CheakToolTip();
    
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
            StopLastCharacterAudio();

            if (Lobby != null)
            {
                Object.Destroy(Lobby.mDMono.gameObject);
                Lobby = null;
            }
            if (Loader != null)
            {
                EB.Assets.UnloadAssetByName("UI3DLobby", false);
                Loader = null;
            }
            ModelName = null;
            DestroySelf();
            //LTPartnerHandbookManager.Instance.GetHandbookList();
            yield break;
        }
    
        /// <summary>
        /// 初始化伙伴列表
        /// </summary>
        /// <param name="isSort"></param>
        private void InitPartnerList(bool isSort = true, bool isRefresh = true)
        {
            partnerDataList = LTPartnerDataManager.Instance.GetPartnerListByGrade((int)CurPartnerGrade, isSort);
            GetSkinDic();
            GetAwakenList();
            if (isRefresh)
            {
                TableScroll.SetItemDatas(partnerDataList);
            }
        }
    
        public LTPartnerStarController StarController;
        private UI3DLobby Lobby = null;
        private GM.AssetLoader<GameObject> Loader = null;
        private string ModelName = null;
        private const int CharacterPoolSize = 10;
        private bool isInitLobby;
        private string lastCharacterAudio = null;
        public ArtifactItemController artifactItemController;
        private IEnumerator CreateBuddyModel(string modelName, string heroinfoid = null)
        {
            isInitLobby = true;
            if (string.IsNullOrEmpty(modelName))
            {
	            controller.UiTextures["LobbyTexture"].gameObject.CustomSetActive(true);
                isInitLobby = false;
                yield break;
            }
    
            if (modelName == ModelName)
            {
	            controller.UiTextures["LobbyTexture"].gameObject.CustomSetActive(true);
                isInitLobby = false;
                yield break;
            }
    
            yield return null;
			//只有在需要跟换时才进行处理
			//controller.UiTextures["LobbyTexture"].gameObject.CustomSetActive(false);
            ModelName = modelName;
            if (Lobby == null && Loader == null)
            {
                Loader = new GM.AssetLoader<GameObject>("UI3DLobby", controller.gameObject);
                UI3DLobby.Preload(modelName);
                yield return Loader;
                if (Loader.Success)
                {
                    Loader.Instance.transform.SetParent(controller.UiTextures["LobbyTexture"].transform);
                    Lobby = Loader.Instance.GetMonoILRComponent<UI3DLobby>();
                    Lobby.ConnectorTexture = controller.UiTextures["LobbyTexture"];
                    Lobby.CharacterPoolSize = CharacterPoolSize;
                    Camera Camera = Lobby.mDMono.transform.Find("UI3DCamera").GetComponent<Camera>();
                    Camera.orthographicSize = 2;
                }
            }
            yield return null;
            TweenAlpha lobbyTextureAlpha = controller.UiTextures["LobbyTexture"].GetComponent<TweenAlpha>();
            lobbyTextureAlpha.ResetToBeginning();
            //controller.UiTextures["LobbyTexture"].gameObject.CustomSetActive(true);
            lobbyTextureAlpha.PlayForward();
    
            if (Lobby != null)
            {
                Lobby.mDMono.gameObject.CustomSetActive(true);
                Lobby.VariantName = modelName;
                while (Lobby.Current == null || Lobby.Current.character == null)
                {
                    yield return null;
                }
                Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
                StopLastCharacterAudio();
                if (heroinfoid != null)
                {
                    FusionAudio.PostCharacterAnimation("Character_Appear", heroinfoid, true);
                    lastCharacterAudio = heroinfoid;
                }
            }
            RenderSettings rs = controller.transform.GetComponentInChildren<RenderSettings>();
            if (rs != null)
            {
                EB.Debug.Log("rendersetting set : {0}", rs.name);
                RenderSettingsManager.Instance.SetActiveRenderSettings(rs.name, rs);
            }
            else
            {
                EB.Debug.LogWarning("rendersetting is null");
            }
            Lobby?.SetBloomUITexture(controller.UiTextures["LobbyTexture"]);
            isInitLobby = false;
        }
        
        private void StopLastCharacterAudio()
        {
            if (lastCharacterAudio != null)
            {
                FusionAudio.PostCharacterAnimation("Character_Appear", lastCharacterAudio, false);
                lastCharacterAudio = null;
            }
        }

        //装备列表
        public LTPartnerEquipItemController[] EquipItem;
        public ArtifactItemController ArtifactItem;
        private void SetHudType()
        {
			controller.Transforms["SwitchTran"].gameObject.CustomSetActive(CurHudType == HudType.RIGHT);
			controller.UiLabels["NameLabel"].gameObject.CustomSetActive(CurHudType == HudType.RIGHT);
			controller.UiSprites["PartnerQualitySprite"].gameObject.CustomSetActive(CurHudType == HudType.RIGHT);
			controller.UiLabels["NameLabel2"].gameObject.CustomSetActive(CurHudType == HudType.LEFT);
            bool showNickName = global::UserData.Locale == EB.Language.ChineseSimplified;
            controller.UiLabels["NickName"].gameObject.CustomSetActive(CurHudType == HudType.LEFT && showNickName);
            controller.UiSprites["PartnerQualitySprite2"].gameObject.CustomSetActive(CurHudType == HudType.LEFT);
			controller.UiSprites["RoleProfileSprite"].transform.parent.gameObject.CustomSetActive(CurHudType == HudType.LEFT);
			controller.TweenPositions["SelectGradeTP"].transform.parent.gameObject.CustomSetActive(CurHudType == HudType.LEFT);
            controller.UiButtons["PartnerStrategy"].transform.gameObject.CustomSetActive(CurHudType == HudType.LEFT);
			if (CurSelectPartner != null)
			{
				controller.GObjects["Leader"].CustomSetActive(CurHudType == HudType.LEFT && CurSelectPartner.HeroId > 0);
				SetLeader();
			}
			controller.GObjects["PartnerWikiRedPoint"].transform.parent.gameObject.CustomSetActive(CurHudType == HudType.LEFT);
			controller.GObjects["PartnerTransBtn"].CustomSetActive(CurHudType == HudType.LEFT);
			controller.GObjects["PartnerFormation"].CustomSetActive(CurHudType == HudType.LEFT);
		}
    
        private void SetPartnerType()
        {
            InitPartnerList(true, false);
        }
    
        /// <summary>
        /// 升级成功
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerLevelUpSuccFunc()
        {
            InitPartnerList();
            TableMoveTo(GetParnterIndex(CurSelectPartner.StatId));
            PartnerCultivateRP();
        }
    
        /// <summary>
        /// 升阶成功
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerUpGradeSuccFunc()
        {
            InitPartnerList();
            TableMoveTo(GetParnterIndex(CurSelectPartner.StatId));
            PartnerCultivateRP();
        }
    
        //private ParticleSystemRenderer[] particle;
        /// <summary>
        /// 升星成功
        /// </summary>
        /// <param name="e"></param>
        private void OnPartnerStarUpSuccFunc(int star)
        {
            InitPartnerList(false);//升星不需要排序
            TableMoveTo(GetParnterIndex(CurSelectPartner.StatId));
            if (CurSelectPartner.IsAwaken > 0) SetStarUpFx(star, controller.ParticleSystemUiComponents["AwakenStarUpFx"]);
            else SetStarUpFx(star, controller.ParticleSystemUiComponents["StarUpFx"]);
            ILRTimerManager.instance.AddTimer((int)(controller.ParticleSystemUiComponents["StarUpFx"].playTime * 1000), 
	            1, (int t) =>
            {
                StarController.SetStarAlpha(CurSelectPartner.Star, CurSelectPartner.IsAwaken);
                LTChargeManager.Instance.UpdateLimitedTimeGiftNotice();
            });
            PartnerCultivateRP();
        }
    
        private void SetStarUpFx(int star, ParticleSystemUIComponent starUpFx)
        {
            starUpFx.transform.SetParent(StarController.StarObjList[star - 1].transform);
            starUpFx.transform.localPosition = Vector3.zero;
            starUpFx.transform.localEulerAngles = Vector3.zero;
            starUpFx.transform.localScale = Vector3.one;
            starUpFx.Play();
        }
    
        /// <summary>
        /// 选择伙伴
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerSelectFunc(int statid)
        {
            if (statid != 0)
            {
                CurSelectPartner = LTPartnerDataManager.Instance.GetPartnerByStatId(statid);
            }
        }
    
        /// <summary>
        /// 装备更改
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerEquipChangeFunc()
        {
            SetEquipOrSkill();
        }
    
        /// <summary>
        /// 召唤伙伴成功
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerSummonSuccFunc(int statid)
        {
            LTPartnerDataManager.Instance.InitPartnerData(true);
            partnerDataList = LTPartnerDataManager.Instance.GetPartnerListByGrade((int)CurPartnerGrade);
            GetSkinDic();
            GetAwakenList();
            CurSelectPartner = LTPartnerDataManager.Instance.GetPartnerByStatId(statid);
            TableScroll.SetItemDatas(partnerDataList);
            //需要刷新伙伴百科红点和内容
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PartnerWikiRP);
            LTDrawCardLookupController.DrawType = DrawCardType.none;
        }
        /// <summary>
        /// 觉醒后伙伴界面刷新
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerAwakenSuccFunc(LTPartnerData data)
        {
            CurSelectPartner = data;
            InitPartnerList();
            PartnerCultivateRP();
        }
        ///// <summary>
        ///// 培养按钮红点更新事件
        ///// </summary>
        ///// <param name="rp"></param>
        //private void PartnerCultivateRPEvent(PartnerCultivateRP rp)
        //{
        //    if (rp != null)
        //    {
        //        CultivateBtnRedPoint.CustomSetActive(rp.isShow);
        //    }
        //}
    
        private void PartnerCultivateRP()
        {
	        controller.GObjects["CultivateBtnRedPoint"].CustomSetActive(LTPartnerDataManager.Instance.IsCanCultivate(CurSelectPartner));
        }
    
        /// <summary>
        /// 设置选中的伙伴数据
        /// </summary>
        private void SetPartnerSelect()
        {
            if (CurSelectPartner != null)
            {
                StartCoroutine(CreateBuddyModel(CurSelectPartner.HeroInfo.model_name, CurSelectPartner.HeroInfo.id.ToString()));//需添加皮肤
                StarController.SetStarAlpha(CurSelectPartner.Star, CurSelectPartner.IsAwaken);
				controller.UiLabels["NameLabel"].text = CurSelectPartner.HeroInfo.name;
				controller.UiLabels["NameLabel2"].text = CurSelectPartner.HeroInfo.name;
				controller.UiLabels["NickName"].text = CurSelectPartner.HeroInfo.obj.Title;
				//暂时做屏蔽，只有在伙伴培养界面打开时修改数据
				if (CultivateController != null && CultivateController.isOpenCulView) CultivateController.InitCultivateInfo(CurSelectPartner);
                else PartnerCultivateRP();
                int curIndex = partnerDataList.IndexOf(CurSelectPartner);

				controller.UiSprites["PartnerQualitySprite"].spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)CurSelectPartner.HeroInfo.role_grade];
				controller.UiSprites["PartnerQualitySprite2"].spriteName = controller.UiSprites["PartnerQualitySprite"].spriteName;
				controller.UiLabels["RoleProfileText"].text = CurSelectPartner.HeroInfo.role_profile;
				controller.UiSprites["RoleProfileSprite"].spriteName = CurSelectPartner.HeroInfo.role_profile_icon;

				controller.GObjects["Leader"].CustomSetActive(CurHudType == HudType.LEFT && CurSelectPartner.HeroId > 0);
				SetLeader();
				controller.UiButtons["EquipBtn"].gameObject.CustomSetActive(CurSelectPartner.HeroId > 0);
				controller.UiButtons["CultivateBtn"].gameObject.CustomSetActive(CurSelectPartner.HeroId > 0);

				SetEquipOrSkill();

                artifactItemController.SetArtifact(curSelectPartner.InfoId);
            }
        }

      

        private void OnPartnerSkinSelect(int skin)
        {
            var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(CurSelectPartner.HeroInfo.id, skin);
            StartCoroutine(CreateBuddyModel(heroInfo.model_name));
            InitPartnerList();//需要刷新头像
        }
    
        /// <summary>
        /// 设置伙伴装备数据or技能
        /// </summary>
        private void SetEquipOrSkill()
        {
            if (isSelectSkill)
            {
                ChangeController.InitSkillInfo(curSelectPartner);
                ChangeController.EquipContent2.CustomSetActive(false);
            }
            else
            {
                HeroEquipmentInfo[] infos = CurSelectPartner.EquipmentsInfo;
                for (int i = 0; i < infos.Length; i++)
                {
                    int Eid = infos[i].Eid;
                    EquipItem[i].SetData(Eid, (EquipPartType)(i + 1), CurSelectPartner);
                }
                ChangeController.EquipContent2.CustomSetActive(CurSelectPartner.HeroId > 0);
            }
        }
    
        /// <summary>
        /// 设置领队
        /// </summary>
        private void SetLeader()
        {
			controller.GObjects["LeaderObj"].CustomSetActive(CurSelectPartner.StatId == LTMainHudManager.Instance.UserLeaderTID);
			controller.GObjects["LeaderFObj"].CustomSetActive(CurSelectPartner.StatId != LTMainHudManager.Instance.UserLeaderTID);
		}

		/// <summary>
		/// 点击伙伴稀有度按钮
		/// </summary>
		/// <param name="btn"></param>
		public void OnPartnerTypeBtnClick(string key)
		{
			int index = int.Parse(key.Replace("TypeBtn", string.Empty).Trim());

			if (CurPartnerGrade == (PartnerGrade)index)
			{
				PlaySelectGradeTween();
				return;
			}
			StartCoroutine(ChangePartnerType(index));
		}

		IEnumerator ChangePartnerType(int index)
        {
            CurPartnerGrade = (PartnerGrade)index;
            PlaySelectGradeTween();
            yield return null;

            if (partnerDataList.Count > 0)
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, partnerDataList[0].StatId);
            }
            else
            {
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, 0);
            }
    
            TableScroll.SetItemDatas(partnerDataList);
    
            yield break;
        }
    
        private bool IsUnfold = false;
        private Vector3 FoldPos = Vector3.zero;
        private Vector3 UnFoldPos = Vector3.zero;

        public void OnSelectGradeBtnClick()
        {
            PlaySelectGradeTween();
        }
    
        private void PlaySelectGradeTween()
        {
            if (UnFoldPos == Vector3.zero)
            {
                // 为了做适配，ui的长度是960，初始位置因为锚点定位的关系不一定是960,16:9的情况下是960，其他情况需要一开始去获取
                UnFoldPos = controller.TweenPositions["SelectGradeTP"].transform.localPosition;
                FoldPos = new Vector3(UnFoldPos.x, UnFoldPos.y - 960, 0);
            }
			controller.TweenPositions["SelectGradeTP"].from = IsUnfold ? FoldPos : UnFoldPos;
			controller.TweenPositions["SelectGradeTP"].to = IsUnfold ? UnFoldPos : FoldPos;
			if (IsUnfold)
            {
                IsUnfold = false;
				controller.GObjects["GradeMask"].CustomSetActive(false);
			}
            else
            {
                IsUnfold = true;
				controller.GObjects["GradeMask"].CustomSetActive(true);
			}
			controller.TweenPositions["SelectGradeTP"].ResetToBeginning();
			controller.TweenPositions["SelectGradeTP"].PlayForward();
		}
    
        private void SetSelectGradeState()
        {
			controller.UiSprites["SelectGradeSp"].gameObject.CustomSetActive(CurPartnerGrade != PartnerGrade.ALL);
			controller.GObjects["SelectGradeLab"].CustomSetActive(CurPartnerGrade == PartnerGrade.ALL);
			if (CurPartnerGrade != PartnerGrade.ALL)
            {
				controller.UiSprites["SelectGradeSp"].spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[CurPartnerGrade];
			}
        }
    
        public override void OnCancelButtonClick()
        {
            LTPartnerDataManager.Instance.itemID = string.Empty;
            LTPartnerDataManager.Instance.itemNeedCount = 0;
    
            if (CurHudType == HudType.RIGHT)
            {
                if (CultivateController.IsSkinViewOpen)
                {
                    CultivateController.OnSkinViewOpenAndClose();
                }
                else if (!IsPlayingTween())
                {
                    //if (CultivateController.GradeStarUp.gameObject.activeInHierarchy)
                    //{
                    //    CultivateController.GradeStarUp.OnCloseBtnClick();
                    //    return;
                    //}

                    //if (CultivateController.GetParExp.gameObject.activeInHierarchy)
                    //{
                    //    CultivateController.GetParExp.OnCloseClick();
                    //    return;
                    //}
                   
                    FusionAudio.PostEvent("UI/General/ButtonClick");
                    CultivateController.StopAni(true);
                    PlayerRightTweenAlpha(false);
                    CultivateController.PotenObj.CustomSetActive(false);
                    CultivateController.isOpenCulView = false;
                    gotoDevelop = false;
                    isStarUp = false;
                    isUpgrade = false;
					CultivateController.HideAwakeSkillPanel();
                }
            }
            else
            {
                if (isInitLobby)
                {
                    // 防止在加载模型的时候按了esc退出，导致摄像机看不见主城；
                    return;
                }
                GlobalMenuManager.Instance.RemoveCache("LTPartnerHud");
                LTPartnerDataManager.Instance.DropSelectPartnerId = 0;
                base.OnCancelButtonClick();
            }
        }
        //private bool isCreateSuccess = false;
        //private WaitForSeconds wait1 = new WaitForSeconds(2.2f);
        //private WaitForSeconds wait2 = new WaitForSeconds(0.2f);
        //private IEnumerator CreateCultivatePerfab()
        //{
        //    yield return wait1;
        //    while (isInitLobby) yield return null;
    
        //    yield break;
        //}

        private void AfterLoadCultivateController(int index){
            if (index != -1)
            {
                CultivateController.OnTabBtnClick(CultivateController.TabBtnList[index]);
            }
            CultivateController.InitCultivateInfo(CurSelectPartner);
            CultivateController.SetLobbyMoveAction(() =>
            {
                if (Lobby != null) Lobby.SetCharMoveState(MoveController.CombatantMoveState.kEntry, true);
            });
            //isCreateSuccess = true;
            CultivateController.isOpenCulView = true;                    
            //不需要直接在点击培养按钮时初始化装备信息
            //CultivateController.InitEquipAttr();
            if (CurHudType == HudType.LEFT)
            {
                PlayerTweenPosition(true);
                PlayerMidTweenAlpha(true);
            }
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PartnerCultivateRP, true);
        }


        public void OnLoadCulView(bool isopen,int index)//index表头值 -1为不设置
        {
            if (CultivateController == null)
            {
                string path = "_GameAssets/Res/Prefabs/UIPrefabs/LTPartner/LTPartnerCulUI";
                EB.Assets.LoadAsync(path, typeof(GameObject), o=>{
                    if(!o){
                        return;
                    }
                    m_PartnerCulObj = GameObject.Instantiate(o) as GameObject;
                    m_PartnerCulObj.transform.parent = controller.transform;
                    m_PartnerCulObj.transform.localScale = Vector3.one;
                    m_PartnerCulObj.transform.localPosition = localPos;
                    //层级显示和collider遮挡
                    UIPanel panel = m_PartnerCulObj.GetComponent<UIPanel>();
                    panel.sortingOrder = controller.gameObject.GetComponent<UIPanel>().sortingOrder + 3;
                    panel.depth = controller.gameObject.GetComponent<UIPanel>().depth + 3;
                    panel.alpha = 0;
                    CultivateController = m_PartnerCulObj.GetMonoILRComponent<LTPartnerCultivateController>();
                    CultivateController.LevelUpFx = controller.transform.Find("Panel/Center/Mid/LeveltUpFx").GetComponent<ParticleSystemUIComponent>();
                    CultivateController.PotenObj = controller.transform.Find("RightTop/NewCurrency/Table/0_ ProficiencyExp").gameObject;
                    TweenAlphaRight = m_PartnerCulObj.GetComponent<TweenAlpha>();
                    CultivateController.InitCultivateInfo(CurSelectPartner);                   
                    if (isopen) AfterLoadCultivateController(index);
                });
            }
            else{
                if(isopen) AfterLoadCultivateController(index);
            }                    
        }
        /// <summary>
        /// 点击伙伴转换按钮
        /// </summary>
        public void OnPartnerTransBtnClick()
        {
            isOpenBattleReadyUI = true;
            GlobalMenuManager.Instance.Open("LTPartnerTransUI");
        }
        private bool isOpenBattleReadyUI = false;
        public void OnBattleReadyBtnClick()
        {
            isOpenBattleReadyUI = true;
            BattleReadyHudController.Open(eBattleType.MainCampaignBattle, null);
        }
    
        /// <summary>
        /// 点击伙伴百科按钮
        /// </summary>
        public void OnClickPartnerWikiBtn()
        {
            Hashtable data = new Hashtable();
            data.Add("type", DrawCardType.wiki);
            GlobalMenuManager.Instance.Open("LTLookUpPartnerUI", data, false);
        }
        /// <summary>
        /// 伙伴百科红点刷新
        /// </summary>
        public void RefreshPartnerWikiRP()
        {
	        controller.GObjects["PartnerWikiRedPoint"].CustomSetActive(LTPartnerDataManager.Instance.IsCanReceiveReward());
        }

        /// <summary>
        /// 点击伙伴攻略按钮
        /// </summary>
        public void OnClickPartnerStrategyBtn()
        {
             GlobalMenuManager.Instance.Open("PartnerStrategyView", CurSelectPartner.HeroInfo);
             //GlobalMenuManager.Instance.Open("LTArtifactUIHud");
        }

        private Vector3 LeftPos = Vector3.zero;
        private Vector3 RightPos = new Vector3(-700, 0, 0);
        private Vector3 FarPos = new Vector3(0, 100000, 0);
        private Vector3 localPos = new Vector3(520, -50, 0);
        private void PlayerTweenPosition(bool isMoveToRight)
        {
            float a = (float)Screen.width / Screen.height;
            float b = (float)2730 / 1536;
            if (RightPos.x == -700 && a > b)
            {
                RightPos.x -= (2730 * (a / b) - 2730) / 2;
            }

			TweenPosition tp = controller.TweenPositions["TweenPos"];

			tp.from = isMoveToRight ? LeftPos : RightPos;
			tp.to = isMoveToRight ? RightPos : LeftPos;
			if (isMoveToRight)
			{
				tp.SetOnFinished(OnMoveToRightEnd);
			}
			else
			{
				tp.SetOnFinished(FxClipFun);
			}

			tp.ResetToBeginning();
			tp.PlayForward();
		}
    
        private void FxClipFun()
        {
            EffectClip[] clips = TableScroll.mDMono.transform.GetComponentsInChildren<EffectClip>();
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].Init();
            }
        }
    
        private void OnMoveToRightEnd()
        {
            PlayerRightTweenAlpha(true);
            if (!isSelectSkill) EquipItem[0].mDMono.transform.parent.gameObject.CustomSetActive(false);//主要是为了关闭特效
            CurHudType = HudType.RIGHT;
        }
    
        private void OnMoveToLeftEnd()
        {
            TweenAlphaRight.transform.localPosition = FarPos;
            //TweenAlphaRight.gameObject.CustomSetActive(false);
            PlayerTweenPosition(false);
            PlayerMidTweenAlpha(false);
            if (!isSelectSkill) EquipItem[0].mDMono.transform.parent.gameObject.CustomSetActive(true);
            CurHudType = HudType.LEFT;
        }
    
        private void PlayerMidTweenAlpha(bool isMoveToRight)
        {
	        TweenAlpha ta = controller.TweenAlphas["TweenAlphaMid"];
	        ta.from = isMoveToRight ? 1f : 0f;
	        ta.to = isMoveToRight ? 0f : 1f;
	        ta.ResetToBeginning();
	        ta.PlayForward();
        }
    
        private void PlayerRightTweenAlpha(bool isMoveToRight)
        {
            //移出界面防止频繁显示隐藏，避免alpha为0；
            TweenAlphaRight.from = isMoveToRight ? 0f : 1f;
            TweenAlphaRight.to = isMoveToRight ? 1f : 0f;
            TweenAlphaRight.transform.localPosition = localPos;
            //TweenAlphaRight.gameObject.CustomSetActive(true);
            CultivateController.RefreshByActive();
            if (isMoveToRight)
            {
                TweenAlphaRight.SetOnFinished(() => { });
            }
            else
            {
                TweenAlphaRight.SetOnFinished(OnMoveToLeftEnd);
            }
            TweenAlphaRight.ResetToBeginning();
            TweenAlphaRight.PlayForward();
        }
    
        private bool IsPlayingTween()
        {
            return controller.TweenPositions["TweenPos"].isActiveAndEnabled || controller.TweenAlphas["TweenAlphaMid"].isActiveAndEnabled || TweenAlphaRight.isActiveAndEnabled;
        }
    
        private List<int> HasSkinList = new List<int>();
        private void GetSkinDic()
        {
            HasSkinList.Clear();
            for (int i = 0; i < partnerDataList.Count; ++i)
            {
                if (partnerDataList[i].HeroId > 0 && Hotfix_LT.Data.CharacterTemplateManager.Instance.HasHeroAwakeInfo(partnerDataList[i].InfoId) && !string.IsNullOrEmpty(Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerDataList[i].InfoId).awakeSkin))
                {
                    HasSkinList.Add(i);
                }
            }
        }
        private List<int> OpenAwakenList = new List<int>();
        private void GetAwakenList()
        {
            OpenAwakenList.Clear();
            for (int i = 0; i < partnerDataList.Count; i++)
            {
                if (partnerDataList[i].HeroId > 0 && Hotfix_LT.Data.CharacterTemplateManager.Instance.HasHeroAwakeInfo(partnerDataList[i].InfoId))
                {
                    OpenAwakenList.Add(i);
                }
            }
    
        }
        public void OnSwitchLeftBtnClick()
        {
            int curIndex = GetParnterIndex(CurSelectPartner.StatId);
            if (CultivateController == null) return;
            if (CultivateController.IsSkinViewOpen)
            {
                if (HasSkinList.Count > 0)
                {
                    int oldIndex = curIndex;
                    curIndex = HasSkinList[HasSkinList.Count - 1];
                    bool isPast = false;
                    for (int i = HasSkinList.Count - 1; i >= 0; --i)
                    {
                        if (isPast)
                        {
                            curIndex = HasSkinList[i];
                            break;
                        }
                        if (oldIndex == HasSkinList[i])
                        {
                            isPast = true;
                        }
                    }
                }
            }
            else if (CultivateController.CurType == CultivateType.Awaken)
            {
                if (OpenAwakenList.Count > 0)
                {
                    int oldIndex = curIndex;
                    curIndex = OpenAwakenList[OpenAwakenList.Count - 1];
                    bool isPast = false;
                    for (int i = OpenAwakenList.Count - 1; i >= 0; --i)
                    {
                        if (isPast)
                        {
                            curIndex = OpenAwakenList[i];
                            break;
                        }
                        if (oldIndex == OpenAwakenList[i])
                        {
                            isPast = true;
                        }
                    }
                }
            }
            else
            {
                if (HasPast(curIndex) && partnerDataList[curIndex - 1].HeroId > 0)
                {
                    curIndex -= 1;
                }
                else
                {
                    curIndex = partnerDataList.Count - 1;
                    if (partnerDataList[curIndex].HeroId <= 0)
                    {
                        for (int i = partnerDataList.Count - 1; i >= 0; --i)
                        {
                            curIndex = i - 1;
                            if (partnerDataList[curIndex].HeroId > 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            CultivateController.StopAni();
            TableMoveTo(curIndex);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, partnerDataList[curIndex].StatId);
        }
    
        public void OnSwitchRightBtnClick()
        {
            int curIndex = GetParnterIndex(CurSelectPartner.StatId);
            if (CultivateController == null) return;
            if (CultivateController.IsSkinViewOpen)
            {
                if (HasSkinList.Count > 0)
                {
                    int oldIndex = curIndex;
                    curIndex = HasSkinList[0];
                    bool isNext = false;
                    for (int i = 0; i < HasSkinList.Count; ++i)
                    {
                        if (isNext)
                        {
                            curIndex = HasSkinList[i];
                            break;
                        }
                        if (oldIndex == HasSkinList[i])
                        {
                            isNext = true;
                        }
                    }
                }
            }
            else if (CultivateController.CurType == CultivateType.Awaken)
            {
                if (OpenAwakenList.Count > 0)
                {
                    int oldIndex = curIndex;
                    curIndex = OpenAwakenList[0];
                    bool isNext = false;
                    for (int i = 0; i < OpenAwakenList.Count; ++i)
                    {
                        if (isNext)
                        {
                            curIndex = OpenAwakenList[i];
                            break;
                        }
                        if (oldIndex == OpenAwakenList[i])
                        {
                            isNext = true;
                        }
                    }
                }
            }
            else
            {
                if (HasNext(curIndex) && partnerDataList[curIndex + 1].HeroId > 0)
                {
                    curIndex += 1;
                }
                else
                {
                    for (int i = 0; i < partnerDataList.Count; ++i)
                    {
                        curIndex = i;
                        if (partnerDataList[curIndex].HeroId > 0)
                        {
                            break;
                        }
                    }
                }
            }
    
            CultivateController.StopAni();
            TableMoveTo(curIndex);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, partnerDataList[curIndex].StatId);
        }
    
        private bool HasNext(int index)
        {
            if (index + 1 >= partnerDataList.Count)
            {
                return false;
            }
            return true;
        }
    
        private bool HasPast(int index)
        {
            if (index - 1 < 0)
            {
                return false;
            }
            return true;
        }
    
        private int GetParnterIndex(int statId)
        {
            for (int i = 0; i < partnerDataList.Count; i++)
            {
                if (partnerDataList[i].StatId == statId)
                {
                    return i;
                }
            }
            return 0;
        }
    
        private void TableMoveTo(int index, bool isAllMove = true)
        {
            if (partnerDataList.Count <= 8)
            {
                return;
            }
    
            index = Mathf.Clamp(index, 0, partnerDataList.Count - 5);
            int ind = index / 2;
            if (!isAllMove && ind <= 3)
            {
                return;
            }
    
            TableScroll.MoveTo(ind);
        }
    
        public void OnPartnerEquipmentBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTPartnerEquipmentUI", CurSelectPartner);
        }
    
        public void OnSetLeaderBtnClick()
        {
            if (curSelectPartner.StatId == LTMainHudManager.Instance.UserLeaderTID)
                return;
            if (!SceneLogicManager.isMainlands())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SETLEADER_LIMIT"));
                return;
            }
            //如果leader相同return
            LTPartnerDataManager.Instance.SetLeader(curSelectPartner.StatId, delegate (bool success)
            {
                if (success)
                {
                    SetLeader();
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerHudController_23172"));
    
                    if (!AllianceUtil.IsInTransferDart)
                    {
                        Hotfix_LT.Messenger.Raise("SetLeaderEvent");

                        /*if (PlayerManager.LocalPlayerController() != null)
                        {
                            MoveController moveCtr = PlayerManager.LocalPlayerController().GetComponentInChildren<MoveController>();
                            if (moveCtr.CurrentState == MoveController.CombatantMoveState.kLocomotion)
                            {
                                moveCtr.IsInitWithLocomotion = true;
                            }
                        }*/
                    }
                }
            });
        }
    
        /// <summary>
        /// 伙伴主界面一键装满装备
        /// </summary>
        public void OnFullEquipClick(UISprite uiSprite)
        {
            if (uiSprite.spriteName == "Equipment_Icon_Shangzhuang")
            {
                //一键装备
                LTPartnerEquipDataManager.Instance.RequireEquipAll(CurSelectPartner.HeroId, (success) =>
                {
                    if (success)
                    {
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_DRESS_SUCCESS"));
                    }
                });
                return;
            }
            LTPartnerEquipDataManager.Instance.RequireUnEquipAll(CurSelectPartner.HeroId, delegate (bool success)
            {
                if (success)
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTEquipmentInforUIController_4512"));
                }
            });
        }
   
    
        #region 测试代码
    
        public int indexin;
    
        public void OnClickTestBtn()
        {
            TableScroll.MoveTo(indexin);
        }
    
        #endregion
    }
    
}
