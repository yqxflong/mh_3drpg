//BattleReadyHudController
//上阵控制器 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB;
using GM.DataCache;
using Hotfix_LT.Data;
using Umeng;
using Debug = UnityEngine.Debug;
using ILRuntime.Runtime;

namespace Hotfix_LT.UI
{
    public enum eAttrTabType
    {
        None = -1,
        All,
        Feng,
        Huo,
        Shui,
        Mercenary
    }

    public enum eDragType
    {
        None = -1,
        DragIcon,
        DragModel,
    }

    public class BattleReadyHudController : UIControllerHotfix
    {
        //英雄交锋相关
        public CombatPartnerDynamicScroll DynamicScroll;
        public UIToggle[] UIToggles;
        public CombatPartnerCellController DragPartnerCell;

        public GameObject[] LockObjs;
        public List<GameObject> mercenaryFlags;
        public UILabel[] LockLevels;
        private UILabel mCountdownLabelCache;

        //觉醒副本相关
        public DamgeBuffController DamgeBuffController;
        public static eBattleType sBattleType { get; private set; }
        public static Hotfix_LT.Data.eRoleAttr sRoleAttr { get; private set; }
        private eAttrTabType mCurPartnerTabType = eAttrTabType.All;

		//首领挑战相关
		public static BossChallengeData ChallengeData;

		private bool mFirstSwithTeam = true;
        public bool mStartBattleClick;

        private Hashtable Param = null;

        public float DRAG_Z = -2f;
        public float MIN_DRAG_DIST = 0.35f;
        public float DRAG_OFFSET_DIST = 0.12f;
        public float MIN_DRAG_IN_DIST = 0.45f;
        public List<GameObject> modelPosList;
        public List<GameObject> judgePosList;
        public Vector3 modelDragOffset = new Vector3(0, 0.35f, 0);
        public Vector3 modelPos = Vector3.zero;
        public Vector3 modelRotation = new Vector3(0, 180f, 0);
        public Vector3 modelScale = new Vector3(200, 200, 200);
        private GameObject curDragModel = null;
        private List<GameObject> modelList = new List<GameObject>();
        private eDragType curDragType = eDragType.None;

        private UIButton _btnTeamSave;
        private UIButton _btnTeamSelect;

		private UIEventListener modelClickListener;
        private List<LTPartnerData> hireDatas;
        private BattleReadyTitle battleReady;

		public override void Awake()
        {
            base.Awake();

            var t = controller.transform;

            controller.backButton = t.GetComponent<UIButton>("Edge/LeftUp/CancelBtn");

            DynamicScroll = t.GetMonoILRComponent<CombatPartnerDynamicScroll>("Edge/Bottom/BuddyList/Placeholder/PartnerGrid");
            DragPartnerCell = t.GetMonoILRComponent<CombatPartnerCellController>("Edge/DragPanel/DragPartnerItem");

            UIToggles = controller.FetchComponentList<UIToggle>(GetArray("Edge/TopRight/Panel/TeamSelect/TeamToggles/1", "Edge/TopRight/Panel/TeamSelect/TeamToggles/2",
		            "Edge/TopRight/Panel/TeamSelect/TeamToggles/3")).ToArray();

            controller.BindingToggleEvent(UIToggles, GetList(new EventDelegate(() => { OnSwithTeamClick( UIToggles[0] ); }),
	            new EventDelegate(() => { OnSwithTeamClick(UIToggles[1]); }), new EventDelegate(() => { OnSwithTeamClick(UIToggles[2]); })));

			LockObjs = controller.FetchComponentList<GameObject>(GetArray("Edge/BuddyModel/Grid/0 (3)/LockObj", "Edge/BuddyModel/Grid/0 (4)/LockObj", 
				"Edge/BuddyModel/Grid/0 (5)/LockObj","Edge/BuddyModel/Grid/0 (1)/LockObj","Edge/BuddyModel/Grid/0 (2)/LockObj"), true).ToArray();

			LockLevels = controller.FetchComponentList<UILabel>(GetArray("Edge/BuddyModel/Grid/0 (3)/LockObj/LockLevel", "Edge/BuddyModel/Grid/0 (4)/LockObj/LockLevel",
				"Edge/BuddyModel/Grid/0 (5)/LockObj/LockLevel")).ToArray();

            DamgeBuffController = t.GetMonoILRComponent<DamgeBuffController>("Edge/LeftUp/DamageBuff");
            
            mStartBattleClick = false;
            DRAG_Z = -2f; MIN_DRAG_DIST = 0.35f; DRAG_OFFSET_DIST = 0.12f; MIN_DRAG_IN_DIST = 0.36f;

            modelPosList = controller.FetchComponentList<GameObject>(GetArray("Edge/BuddyModel/Grid/0/ModelPos", "Edge/BuddyModel/Grid/0 (1)/ModelPos", 
	            "Edge/BuddyModel/Grid/0 (2)/ModelPos", "Edge/BuddyModel/Grid/0 (3)/ModelPos", "Edge/BuddyModel/Grid/0 (4)/ModelPos", "Edge/BuddyModel/Grid/0 (5)/ModelPos"),true);
            mercenaryFlags = controller.FetchComponentList<GameObject>(GetArray("Edge/BuddyModel/Grid/0/ModelPos/Sprite", "Edge/BuddyModel/Grid/0 (1)/ModelPos/Sprite", 
                "Edge/BuddyModel/Grid/0 (2)/ModelPos/Sprite", "Edge/BuddyModel/Grid/0 (3)/ModelPos/Sprite", "Edge/BuddyModel/Grid/0 (4)/ModelPos/Sprite", "Edge/BuddyModel/Grid/0 (5)/ModelPos/Sprite"),true);
            judgePosList = controller.FetchComponentList<GameObject>(GetArray("Edge/BuddyModel/Grid/0/CenterPos", "Edge/BuddyModel/Grid/0 (1)/CenterPos", 
	            "Edge/BuddyModel/Grid/0 (2)/CenterPos", "Edge/BuddyModel/Grid/0 (3)/CenterPos", "Edge/BuddyModel/Grid/0 (4)/CenterPos", "Edge/BuddyModel/Grid/0 (5)/CenterPos") ,true);

            controller.UiButtons["AllBtn"].onClick.Add(new EventDelegate(() => { OnRaceTabClick(controller.UiButtons["AllBtn"].gameObject); }));

            battleReady = t.GetMonoILRComponent<BattleReadyTitle>("Edge/Bottom/BG/Title");

            controller.BindingBtnEvent(GetList("AllBtn", "FengBtn", "HuoBtn", "ShuiBtn","MercenaryBtn"), GetList(
				new EventDelegate(() => { battleReady.OnTitleBtnClick(controller.UiButtons["AllBtn"].transform.FindEx("Sprite").gameObject); }),
				new EventDelegate(() => { OnRaceTabClick(controller.UiButtons["FengBtn"].gameObject); }),
				new EventDelegate(() => { OnRaceTabClick(controller.UiButtons["HuoBtn"].gameObject); }),
				new EventDelegate(() => { OnRaceTabClick(controller.UiButtons["ShuiBtn"].gameObject); }),
                new EventDelegate(() => { OnRaceTabClick(controller.UiButtons["MercenaryBtn"].gameObject); })));

			controller.BindingBtnEvent(GetList("FengBtn", "HuoBtn", "ShuiBtn","MercenaryBtn"), GetList(
				new EventDelegate(() => { battleReady.OnTitleBtnClick(controller.UiButtons["FengBtn"].transform.FindEx("Sprite").gameObject); }),
				new EventDelegate(() => { battleReady.OnTitleBtnClick(controller.UiButtons["HuoBtn"].transform.FindEx("Sprite").gameObject); }),
				new EventDelegate(() => { battleReady.OnTitleBtnClick(controller.UiButtons["ShuiBtn"].transform.FindEx("Sprite").gameObject); }),
                new EventDelegate(() => { battleReady.OnTitleBtnClick(controller.UiButtons["MercenaryBtn"].transform.FindEx("Sprite").gameObject); })));

            controller.FindAndBindingBtnEvent(GetList("Edge/LeftUp/DamageBuff",
	            "Edge/LeftUp/DamageBuff/ToastWindow/Panel/CloaseBtn",
	            "Edge/Bottom/StartBtnPanel/BG/StartBattleBtn", "Edge/Bottom/BG/RuleBtn",
	            "Edge/TopRight/CheckEnemyFormationLabel"), GetList(
	            new EventDelegate(t.GetMonoILRComponent<DamgeBuffController>("Edge/LeftUp/DamageBuff").OnClick),
	            new EventDelegate(t.GetMonoILRComponent<DamgeBuffController>("Edge/LeftUp/DamageBuff").CloseWindow),
	            new EventDelegate(OnStartBattleClick),
	            new EventDelegate(OnAttrInfoBtnClick), new EventDelegate(CheckEnemyInfoBtnClick),
	            new EventDelegate(t.GetComponent<PositionAni>("Edge/TopRight/Panel/TeamSelect").OnBtnCLick)));

            controller.FindAndBindingTweenFinishedEvent(GetList("Edge/Bottom/BG/Title/BtnList/AllBtn/Sprite", "Edge/Bottom/BG/Title/BtnList/FengBtn/Sprite",
				"Edge/Bottom/BG/Title/BtnList/HuoBtn/Sprite", "Edge/Bottom/BG/Title/BtnList/ShuiBtn/Sprite","Edge/Bottom/BG/Title/BtnList/MercenaryBtn/Sprite"), GetList(
	            new EventDelegate(() => { battleReady.OnFinishShow(t.FindEx("Edge/Bottom/BG/Title/BtnList/AllBtn/Sprite/Sprite (1)").gameObject); }),
	            new EventDelegate(() => { battleReady.OnFinishShow(t.FindEx("Edge/Bottom/BG/Title/BtnList/FengBtn/Sprite/Sprite (1)").gameObject); }),
	            new EventDelegate(() => { battleReady.OnFinishShow(t.FindEx("Edge/Bottom/BG/Title/BtnList/HuoBtn/Sprite/Sprite (1)").gameObject); }),
	            new EventDelegate(() => { battleReady.OnFinishShow(t.FindEx("Edge/Bottom/BG/Title/BtnList/ShuiBtn/Sprite/Sprite (1)").gameObject); }),
                new EventDelegate(() => { battleReady.OnFinishShow(t.FindEx("Edge/Bottom/BG/Title/BtnList/MercenaryBtn/Sprite/Sprite (1)").gameObject); })));

			controller.DragEventDispatchers["DraDispatcher"].onDragFunc.Add(new EventDelegate(OnModelDrag));
            controller.DragEventDispatchers["DraDispatcher"].onDragStartFunc.Add(new EventDelegate(() => { OnModelDragStart(); }));
            controller.DragEventDispatchers["DraDispatcher"].onDragEndFunc.Add(new EventDelegate(DragEndFunc));

			GameObject go = controller.DragEventDispatchers["DraDispatcher"].gameObject;
			modelClickListener = UIEventListener.Get(go);
			modelClickListener.onClick += OnModelClicked;
			
			_btnTeamSave = t.GetComponent<UIButton>("Edge/Right/Btn_TeamSave");
            _btnTeamSave.onClick.Add(new EventDelegate(OnTeamSaveBtnClicked));
            _btnTeamSelect = t.GetComponent<UIButton>("Edge/Right/Btn_TeamSelect");
            _btnTeamSelect.onClick.Add(new EventDelegate(OnTeamSelectBtnClicked));

            ShowHideLineupBtn();
            
          
        }

        private void SetMerCenaryFlag(TeamMemberData data)
        {
            int index = -1;
            if (data != null && ShowMercenary()) index = data.Pos;
            for (int i = 0; i < mercenaryFlags.Count; i++)
            {
                if (i==index)
                {
                    mercenaryFlags[i].CustomSetActive(true);
                }
                else
                {
                    mercenaryFlags[i].CustomSetActive(false);
                }
            }
        }

        public static bool ShowMercenary()
        {
            // if (LegionModel.GetInstance().isJoinedLegion)
            // {
                return sBattleType == eBattleType.ChallengeCampaign || sBattleType == eBattleType.AwakeningBattle ||
                       sBattleType == eBattleType.InfiniteChallenge;
             // }
            // return false;
        }

        private void ShowHideLineupBtn()
        {
            if (_btnTeamSave == null || _btnTeamSelect == null)
            {
                return;
            }

            switch (sBattleType)
            {
                case eBattleType.ArenaBattle:
                case eBattleType.HeroBattle:
                case eBattleType.HonorArena:
                case eBattleType.LegionMercenary:
                case eBattleType.LadderBattle:
                    _btnTeamSave.gameObject.SetActive(false);
                    _btnTeamSelect.gameObject.SetActive(false);
                    break;
                default:
                    _btnTeamSave.gameObject.SetActive(true);
                    _btnTeamSelect.gameObject.SetActive(true);
                    break;
            }
        }

        private void OnTeamSaveBtnClicked()
        {
            OpenLineupPresetUI(true);
        }

        private void OnTeamSelectBtnClicked()
        {
            OpenLineupPresetUI(false);
        }

        private int _minLineupCount = 3;
        private int _maxMemberCount = 6;

        private void OpenLineupPresetUI(bool showSavePanel, string lineupType = "default")
        {
            var hashtable = Johny.HashtablePool.Claim();
            hashtable.Add("ShowSavePanel", showSavePanel);
            hashtable.Add("LineupType", lineupType);
            hashtable.Add("BattleType", sBattleType.ToInt32());
            System.Action callback = () => { 
                SetDefaultTeam(); 

                for (var i = 0; i < modelList.Count; i++)
                {
                    PlayEntryAction(modelList[i]);
                }
            };
            hashtable.Add("Callback", callback);
            ArrayList data;

            if (DataLookupsCache.Instance.SearchDataByID("lineup_preset", out data) && data.Count > 0)
            {
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTLineupPresetUI", hashtable);
            }
            else
            {
                var uid = LoginManager.Instance.LocalUser.Id.Value;
                LTFormationDataManager.Instance.API.GetLineupPreset(uid, (result) => {
                    ArrayList arrayList = EB.Dot.Array("lineup_preset", result, new ArrayList());
                    bool existLineupType = false;

                    for (var i = 0; i < arrayList.Count; i++)
                    {
                        var type = EB.Dot.String("lineup_type", arrayList[i], "");

                        if (type.Equals(lineupType))
                        {
                            existLineupType = true;
                            break;
                        }
                    }
                    
                    if (arrayList.Count < 1 || !existLineupType)
                    {
                        //[{"lineup_type":"default", "lineup_infos":[[0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0]]}]
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("lineup_type", "default");
                        var infos = new ArrayList();
                        infos.Add(new int[_maxMemberCount]);
                        infos.Add(new int[_maxMemberCount]);
                        infos.Add(new int[_maxMemberCount]);
                        ht.Add("lineup_infos", infos);
                        arrayList.Add(ht);
                    }
                        
                    DataLookupsCache.Instance.CacheData("lineup_preset", arrayList);

                    // 最少显示3组阵容，阵容数据为空时用默认数据替代
                    for (var i = 0; i < arrayList.Count; i++)
                    {
                        var type = EB.Dot.String("lineup_type", arrayList[i], "");
                        var infos = EB.Dot.Array("lineup_infos", arrayList[i], new ArrayList());

                        if (type.Equals(lineupType))
                        {
                            for (var j = 0; j < _minLineupCount; j++)
                            {
                                if ((infos.Count > j && infos[j] == null) || infos.Count <= j)
                                {
                                    DataLookupsCache.Instance.CacheData(string.Format("lineup_preset[{0}].lineup_infos[{1}]", i, j), new int[_maxMemberCount]);
                                }
                            }

                            break;
                        }
                    }

                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    GlobalMenuManager.Instance.Open("LTLineupPresetUI", hashtable);
                });
            }
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Param = (Hashtable)param;
            mCurPartnerTabType = eAttrTabType.All;
            battleReady.OnTitleBtnClick(controller.UiButtons["AllBtn"].transform.FindEx("Sprite").gameObject);
            if(Param.Contains("battleType")){
                sBattleType = (eBattleType)Param["battleType"];
            }
            controller.UiButtons["MercenaryBtn"].gameObject.CustomSetActive(ShowMercenary());
            mIsLoadingModel = false;
            mStartBattleClick = false;
            LTPartnerDataManager.Instance.InitPartnerData();
            LTFormationDataManager.Instance.SetFormationData();
            LTFormationDataManager.Instance.InitHeroBattleTp();
            InitLockState();
            SetBattleBtn();
            InitUIMode();

            controller.GObjects["CheckEnemyFormationButton"].CustomSetActive(Param != null && Param["enemyLayout"] != null);
            AwakeningSetting();
            ShowHideLineupBtn();
            
            LTFormationDataManager.Instance.SetCurTeamMemberData(() =>
            {
                //会出现先显示模型才下阵佣兵的情况 因此回调再次刷新一遍
                CreateTeamModel(FormationUtil.GetCurrentTeamName());
                SetMerCenaryFlag(LTFormationDataManager.Instance.CurTeamMemberData);
            });
            if (ShowMercenary())
            {
                AlliancesManager.Instance.GetAllianceMercenaries((data) => { hireDatas = data; });
            }
        }

        public void AwakeningSetting()
        {
            List<TeamMemberData> datas = GetCurrentTeamMemList();
			if (sBattleType == eBattleType.AwakeningBattle)
			{
				int needRoleAttr = (int)NewGameConfigTemplateManager.Instance.GetGameConfigValue("AwakenCampaignSkill_" + (int)sRoleAttr);
				//判断角色个数
				int num = 0;
				for (int i = 0; i < datas.Count; i++)
				{
					TeamMemberData temp = datas[i];
					if (CharacterTemplateManager.Instance.GetHeroInfo(temp.InfoID).char_type == (eRoleAttr)needRoleAttr)
					{
						num++;
					}
				}
				if (!string.IsNullOrEmpty(DamgeBuffController.AttrAddNumLabel_Subs[0].text) &&
				   int.Parse(DamgeBuffController.AttrAddNumLabel_Subs[0].text) != num) controller.ParticleSystemUiComponents["DamgeFx"].Play();
				DamgeBuffController.AwakeningSetting(num, (Hotfix_LT.Data.eRoleAttr)needRoleAttr);
			}
			else if (sBattleType == eBattleType.BossChallenge)
			{
				int num = 0;
				for (int i = 0; i < datas.Count; i++)
				{
					TeamMemberData temp = datas[i];
					if (CharacterTemplateManager.Instance.GetHeroInfo(temp.InfoID).char_type == sRoleAttr)
						num++;
				}
				if (!string.IsNullOrEmpty(DamgeBuffController.AttrAddNumLabel_Subs[0].text) &&
				   int.Parse(DamgeBuffController.AttrAddNumLabel_Subs[0].text) != num) controller.ParticleSystemUiComponents["DamgeFx"].Play();
				DamgeBuffController.AwakeningSetting(num, sRoleAttr, ChallengeData.UnitValue, ChallengeData.BuffName, ChallengeData.DescribeFormat);
			}
			else
			{
				DamgeBuffController.mDMono.gameObject.SetActive(false);

			}
        }

        private RenderSettings mRs = null;
        public override void OnFocus()
        {
            base.OnFocus();
            StartCoroutine(SetRendersetting());
        }

        private IEnumerator SetRendersetting()
        {
            UICamera.mainCamera.transform.localPosition = new Vector3(0, 0, -4000);
            while (mRs == null)
            {
                yield return null;
                mRs = controller.transform.GetComponentInChildren<RenderSettings>();
            }
            if (mRs != null)
            {
               EB.Debug.LogWarning("Battle Ready rendersetting active");
                RenderSettingsManager.Instance.SetActiveRenderSettings(mRs.name, mRs);
            }
        }

        public override IEnumerator OnAddToStack()
        {
            controller.transform.localPosition = new Vector3(0, 0, 3000);
            yield return base.OnAddToStack();           
            SetDefaultTeam();
            this.DynamicScroll.mDMono.gameObject.CustomSetActive(true);//放在这是为了解决图片在反复进出的时候偶现会drawcall不渲染问题
        }

        public override IEnumerator OnRemoveFromStack()
        {
            StopAllCoroutines();
            UICamera.mainCamera.transform.localPosition = new Vector3(0, 0, 0);
            this.DynamicScroll.mDMono.gameObject.CustomSetActive(false);
            DynamicScroll.Clear();
            DestroySelf();
            yield break;
        }

        void InitUIMode()
        {
            UIToggles[0].transform.parent.parent.gameObject.CustomSetActive(false);//true改为不能选择阵容
            switch (sBattleType)
            {
                case eBattleType.ArenaBattle:
                    controller.UiLabels["BattleLabel"].text = EB.Localizer.GetString("ID_DIALOG_BUTTON_OK");//确定
                    break;
                case eBattleType.HeroBattle:
                    controller.UiLabels["BattleLabel"].text = EB.Localizer.GetString("ID_uifont_in_LTMainInstanceCampaignView_Label_5");
                    LockObjs[1].CustomSetActive(true);
                    LockObjs[2].CustomSetActive(true);
                    LockLevels[1].text = LockLevels[2].text = string.Empty;
                    break;
                case eBattleType.LegionMercenary:
                    for (int j = 0; j < LockObjs.Length; j++)
                    {
                        LockObjs[j].CustomSetActive(true);
                    }
                    //默认更新佣兵
                    int heroId = AlliancesManager.Instance.GetMercenaryHeroId();
                    if (heroId > 0)
                    {
                        LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);
                        AlliancesManager.Instance.SetAllianceMarcenary(heroId, data.powerData.curPower, (bo) => { });
                    }
                    break;
                default:
                    controller.UiLabels["BattleLabel"].text = EB.Localizer.GetString("ID_uifont_in_LTMainInstanceCampaignView_Label_5");
                    break;
            }

            int i;
            controller.UiProgressBars["HeroBattleCostProgressBar"].gameObject.CustomSetActive(isNeedShowConstProgressBar(out i));
        }

        public bool isNeedShowConstProgressBar(out int cost)
        {
            cost = 0;
            if (sBattleType == eBattleType.HeroBattle)
            {
                if (LTNewHeroBattleManager.GetInstance().GetHeroBattleTypeIsLock(HeroBattleType.High))
                {
                    int layer= LTNewHeroBattleManager.GetInstance().GetCurrentFinishLayer();
                    cost= EventTemplateManager.Instance.GetNextHeroBattleData(layer).CostLimit;
                    return cost>0;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 剩余的花费点数
        /// </summary>
        private int Surplus = -1;
        /// <summary>
        /// 刷新英雄交锋的显示花费
        /// </summary>
        public void ResetHeroBattleCostUI(bool isChange = false)
        {
            int curCost = LTNewHeroBattleManager.GetInstance().NewHeroBattleCurCost;
            int teanCost = GetHeroBattleTeamCost();
            int maxCost = LTNewHeroBattleManager.GetInstance().NewHeroBattleMaxCost;

            if (Surplus != maxCost - curCost - teanCost || isChange)
            {
                Surplus = maxCost - curCost - teanCost;
                controller.TweenAlphas["HeroBattleLight"].ResetToBeginning();
                controller.TweenAlphas["HeroBattleLight"].PlayForward();
                controller.UiProgressBars["HeroBattleCostProgressBar"].value = (float)Surplus / (float)maxCost;
                controller.UiLabels["HeroBattleCostLabel"].text = string.Format("{0}/{1}", Surplus, maxCost);
            }
        }
        
        public void ResetHeroBattleNewBieCostUI()
        {
            int teanCost = GetHeroBattleNewBieTeamCost();
            int maxCost;
            isNeedShowConstProgressBar(out maxCost);

            if (Surplus != maxCost - teanCost)
            {
                Surplus = maxCost - teanCost;
                controller.TweenAlphas["HeroBattleLight"].ResetToBeginning();
                controller.TweenAlphas["HeroBattleLight"].PlayForward();
                controller.UiProgressBars["HeroBattleCostProgressBar"].value = (float)Surplus / (float)maxCost;
                controller.UiLabels["HeroBattleCostLabel"].text = string.Format("{0}/{1}", Surplus, maxCost);
            }
        }

        void SetBattleBtn()
        {
            controller.GObjects["BattleBtnPanel"].CustomSetActive(true);
            Hashtable ht = Param as Hashtable;
            if (ht["startCombatCallback"] == null)//(ht["startCombatCallback"] == null && sBattleType == eBattleType.MainCampaignBattle) || (ht["startCombatCallback"] == null && sBattleType == eBattleType.ChallengeCampaign) || (ht["startCombatCallback"] == null && sBattleType == eBattleType.AlienMazeBattle))
            {
                controller.GObjects["BattleBtnPanel"].CustomSetActive(false);
            }
        }

        private List<TeamMemberData> mTeamMemDataList;
        private bool mIsLoadingModel = false;

        private void CreateTeamModel(string teamName)
        {
            mIsLoadingModel = true;
            ClearModelList();
            mTeamMemDataList = LTFormationDataManager.Instance.GetTeamMemList(teamName);
            
            CreateAllModel();
        }

        /// <summary>
        /// 下阵英雄交锋所有上阵的伙伴
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="callback"></param>
        private void ClearTeam(string teamName, System.Action callback)
        {
            var teamHeros = LTFormationDataManager.Instance.GetTeamMemList(teamName);
            int count = teamHeros.Count;
            if (count > 0)
            {
                for (int i = 0; i < teamHeros.Count; i++)
                {
                    LTFormationDataManager.Instance.RequestRemoveHeroFormation(teamHeros[i].HeroID,teamHeros[i].InfoID+1, teamName, delegate ()
                    {
                        count--;
                        if (count <= 0 && callback != null)
                        {
                            callback();
                        }
                    });
                }
            }
            else
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }
        
        /// <summary>
        /// 一次性创建所有hero
        /// </summary>
        /// <param name="time"></param>
        private void CreateAllModel()
        {
            if (controller.gameObject == null)
            {
                return;
            }

            int loadedCount = 0;
            for(int i = 0; i < mTeamMemDataList.Count; i++){
                TeamMemberData teamMemData = mTeamMemDataList[i] as TeamMemberData;
                CreateModel(teamMemData, delegate(GameObject go, TeamMemberData callData) {
                    var curModelPos = callData.Pos;
                    go.transform.position = curModelPos >= 0 ? modelPosList[curModelPos].transform.position + modelPos : Vector3.zero;
                    loadedCount++;
                    if(loadedCount == mTeamMemDataList.Count){
                        mIsLoadingModel = false;
                    }
                });
            }
            //没有上阵的伙伴的情况处理
            if (mTeamMemDataList.Count==0)
            {
                mIsLoadingModel = false;
            }
        }

        /// <summary>
        /// 刷新伙伴数据列表
        /// </summary>
        /// <param name="tab_type"></param>
        public void RefreshPartnerList(eAttrTabType tab_type)
        {
            mCurPartnerTabType = tab_type;
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();

            if (sBattleType == eBattleType.ChallengeCampaign || sBattleType == eBattleType.MainCampaignBattle)
            {
                partnerList.InsertRange(0, LTInstanceHireUtil.GetHireList());//雇佣兵排在最前面
            }
            else if (sBattleType == eBattleType.HeroBattle)
            {
               //英雄交锋新手挑战
                if (LTNewHeroBattleManager.GetInstance().CurrentType!=HeroBattleType.High)
                {
                     partnerList = LTFormationDataManager.Instance.GetVirtualPartnerList();
                     ResetHeroBattleNewBieCostUI();
                }
                else
                {
                    ResetHeroBattleCostUI();
                }
                partnerList = ResetToHeroBattleList(partnerList);
                
            }else if (sBattleType == eBattleType.SleepTower && LTClimingTowerHudController.Instance != null)
            {
              var sleepList = partnerList.FindAll((a) => { return !LTClimingTowerHudController.Instance.CanUpTeam(a.HeroId); });
              partnerList.RemoveAll((a) => { return !LTClimingTowerHudController.Instance.CanUpTeam(a.HeroId); });
              partnerList.AddRange(sleepList);
            }

            List<LTPartnerData> filterList = new List<LTPartnerData>();

            for (var i = 0; i < partnerList.Count; i++)
            {
                var partner = partnerList[i];

                if (IsInTeam(partner.HeroId))
                    continue;

                if (tab_type == eAttrTabType.All)
                    filterList.Add(partner);
                else if (tab_type == eAttrTabType.Feng && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Feng)
                    filterList.Add(partner);
                else if (tab_type == eAttrTabType.Huo && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Huo)
                    filterList.Add(partner);
                else if (tab_type == eAttrTabType.Shui && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Shui)
                    filterList.Add(partner);
            }

            if (tab_type==eAttrTabType.Mercenary)
            {
                filterList.AddRange(hireDatas);
                if(LTFormationDataManager.Instance.CurTeamMemberData!=null)filterList.RemoveAll((test) => test.uid == LTFormationDataManager.Instance.CurTeamMemberData.Uid);
            }

            if (controller.GObjects["BattleBtnPanel"] != null && controller.GObjects["BattleBtnPanel"].activeSelf)
            {
                filterList.AddRange(new LTPartnerData[2]);
            }

            if (DynamicScroll != null)
            {
                DynamicScroll.SetItemDatas(filterList.ToArray());
            }

            SetIconDragAction();
            AwakeningSetting();
            SetMerCenaryFlag(LTFormationDataManager.Instance.CurTeamMemberData);
        }

        /// <summary>
        /// 英雄交锋的伙伴为满级满阶满星
        /// </summary>
        /// <param name="partnerList"></param>
        /// <returns></returns>
        private List<LTPartnerData> ResetToHeroBattleList(List<LTPartnerData> partnerList)
        {
            List<LTPartnerData> filterList = new List<LTPartnerData>();
            for (var i = 0; i < partnerList.Count; i++)
            {
                var partner = partnerList[i];
                LTPartnerData temp = GetHeroBattlePatnerData(partner);
                filterList.Add(temp);
            }
            //排序，把已使用过的再往后移动
            filterList.Sort((a, b) =>
            {
                if (LTNewHeroBattleManager.GetInstance().HasChallengeHeroInfoID.Contains(a.StatId))
                    return 1;
                else if (LTNewHeroBattleManager.GetInstance().HasChallengeHeroInfoID.Contains(b.StatId))
                    return -1;
                else
                    return 0;
            });
            return filterList;
        }

        private LTPartnerData GetHeroBattlePatnerData(LTPartnerData partner)
        {
            LTPartnerData temp = new LTPartnerData();
            temp.mHeroId = partner.HeroId;
            temp.StatId = partner.StatId;
            temp.InfoId = partner.InfoId;
            temp.HeroInfo = partner.HeroInfo;
            temp.HeroStat = partner.HeroStat;
            temp.IsHeroBattle = true;
            if (temp.HeroInfo.role_grade != 0)
            {
                temp.HeroBattleCost = LTNewHeroBattleManager.GetInstance().GetCostByRoleGrade(temp.HeroInfo.role_grade);
            }
            else
            {
                Debug.LogError("temp.HeroInfo.role_grade is null");
            }
            temp.HeroBattleUpGradeId = LTPartnerConfig.MAX_GRADE_LEVEL;
            temp.HeroBattleStar = LTPartnerConfig.MAX_STAR;
            temp.HeroBattleAwakenLevel = 0;
            return temp;
        }

        private void SetDefaultTeam()
        {
            if (sBattleType == eBattleType.ArenaBattle || sBattleType == eBattleType.ChallengeCampaign || sBattleType == eBattleType.AllieancePreBattle || sBattleType == eBattleType.AllieanceFinalBattle || sBattleType == eBattleType.AlienMazeBattle
                || sBattleType == eBattleType.SleepTower|| sBattleType == eBattleType.LegionMercenary)
            {
                if (sBattleType == eBattleType.ChallengeCampaign || sBattleType == eBattleType.AlienMazeBattle)
                {
                    LTFormationDataManager.Instance.SetFormationData();
                }
                CreateTeamModel(FormationUtil.GetCurrentTeamName());
                RefreshPartnerList(mCurPartnerTabType);
            }
            else if (sBattleType == eBattleType.HeroBattle)
            {
                ClearTeam(FormationUtil.GetCurrentTeamName(), delegate
                {
                    LTFormationDataManager.Instance.SetFormationData();
                    CreateTeamModel(FormationUtil.GetCurrentTeamName());
                    RefreshPartnerList(mCurPartnerTabType);
                });
            }
            else
            {
                UIToggles[0].enabled = FormationUtil.GetTeamOpen("team1");
                UIToggles[1].enabled = FormationUtil.GetTeamOpen("team2");
                UIToggles[2].enabled = FormationUtil.GetTeamOpen("team3");
                for (var i = 0; i < UIToggles.Length; i++)
                {
                    LockTeam(UIToggles[i]);
                }
                int currentTeam = 0;// FormationUtil.GetCurrentTeamIndex();移除了阵容123因此默认选0;
                if (!UIToggles[currentTeam].enabled)
                {
                    mFirstSwithTeam = false;
                    OnSwithTeamClick(UIToggles[0]);
                }

                UIToggles[currentTeam].Start();
                UIToggles[currentTeam].Set(false, true);
                UIToggles[currentTeam].Set(true, true);
            }

        }

        /// <summary>
        /// 计算英雄交锋上阵的花费
        /// </summary>
        /// <returns></returns>
        private int GetHeroBattleTeamCost()
        {
            int Cost = 0;
            if (sBattleType == eBattleType.HeroBattle)
            {
                var Temps = LTFormationDataManager.Instance.GetTeamMemList(FormationUtil.GetCurrentTeamName());
                for (int i = 0; i < Temps.Count; i++)
                {
                    var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(Temps[i].InfoID);
                    if (heroInfo != null)
                    {
                        Cost += LTNewHeroBattleManager.GetInstance().GetCostByRoleGrade(heroInfo.role_grade);
                    }
                }
            }
            return Cost;
        }
        
        private int GetHeroBattleNewBieTeamCost()
        {
            int Cost = 0;
            if (sBattleType == eBattleType.HeroBattle)
            {
                var Temps = LTFormationDataManager.Instance.HeroBattleTempPartner;
                foreach (var VARIABLE in Temps)
                {
                    var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(VARIABLE.Value-1);
                    if (heroInfo != null)
                    {
                        Cost += LTNewHeroBattleManager.GetInstance().GetCostByRoleGrade(heroInfo.role_grade);
                    }
                }
            }
            return Cost;
        }

        private void InitLockState()
        {
            int mMaxIndex = FormationConfig.GetTeamMaxIndex(sBattleType);
            //int level = BalanceResourceUtil.GetUserLevel();
            //原来只需要3个
            for (int i = 0; i < 3; ++i)
            {
                var func = FuncTemplateManager.Instance.GetFunc(FormationConfig.OpenTeamFuncID[i]);
                if (func != null && !func.IsConditionOK())
                {
                    LockObjs[i].CustomSetActive(true);
                    LockLevels[i].text = func.GetConditionStrShort();
                }
                else
                {
                    LockObjs[i].CustomSetActive(false);
                }
            }
            LockObjs[3].CustomSetActive(false);
            LockObjs[4].CustomSetActive(false);
        }

        void LockTeam(UIToggle toggle)
        {
            if (toggle.enabled)
                return;

            toggle.transform.Find("BGLight").gameObject.CustomSetActive(false);
            toggle.transform.Find("Lock").gameObject.CustomSetActive(true);
        }

        bool IsInTeam(int heroID)
        {
            if (sBattleType == eBattleType.LegionMercenary)
            {
                if (AlliancesManager.Instance.GetMercenaryHeroId()==heroID)
                {
                    return true;    
                }
                return false;
            }
            
            List<TeamMemberData> teamList = LTFormationDataManager.Instance.GetTeamMemList(FormationUtil.GetCurrentTeamName());

            for (int i = 0; i < teamList.Count; i++)
            {
                if (teamList[i].HeroID == heroID)
                    return true;
            }
            return false;
        }

        List<TeamMemberData> GetCurrentTeamMemList()
        {
            return LTFormationDataManager.Instance.GetTeamMemList(FormationUtil.GetCurrentTeamName());
        }

        bool GetCanAddHeroToFormation(TeamMemberData memberData, int pos, bool move = false)
        {
            string message = string.Empty;
            int maxIndex = 0;
            if (FormationConfig.IsIndexVaild(sBattleType, pos, out maxIndex, out message))
            {
                if (!string.IsNullOrEmpty(message))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, message);
                }
                return false;
            }

            if (!move && IsInTeam(memberData.HeroID))
            {
               EB.Debug.LogError("is in team for heroid={0}" , memberData.HeroID);
                return false;
            }
            return true;
        }

        bool GetOnlyOneFire(TeamMemberData memberData, int pos)
        {
            if (memberData.IsHire==true &&LTFormationDataManager.Instance.CurTeamMemberData!= null && LTFormationDataManager .Instance.CurTeamMemberData.Pos != pos)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ALLIANCE_ONLYONE_MERCENARY"));
                return false;
            }
            return true;
        }
        

        public void OnSwithTeamClick(UIToggle sender)
        {
            if (sender.value)
            {
                string teamName = "team" + sender.gameObject.name;
                if (mFirstSwithTeam)
                {
                    mFirstSwithTeam = false;
                    //CreateTeamModel(teamName);
                }
                else
                {
                    CreateTeamModel(teamName);
                    FormationUtil.SetCurrentTeamName(teamName);
                    LTCombatHudController.ResetAutoSkill();
                    LTFormationDataManager.Instance.RequestSwitchTeam(teamName, delegate ()
                    {
                    });
                }
                RefreshPartnerList(mCurPartnerTabType);
            }
        }

        public void OnRaceTabClick(GameObject obj)
        {
            if (curDragType != eDragType.None) DragEndFunc();
            eAttrTabType tabType = ParseTabType(obj.name);
            RefreshPartnerList(tabType);
        }

        public void OnAttrInfoBtnClick()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            GlobalMenuManager.Instance.Open("LTAttributeInfo");
        }

        public void OnStartBattleClick()//战斗按钮
        {
            if (GuideToolController.Instance.m_DragGuide.gameObject.activeSelf)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_PARTNER));
                return;
            }

            if (sBattleType == eBattleType.HeroBattle)
            {
                if (LTNewHeroBattleManager.GetInstance().CurrentType!=HeroBattleType.High)
                {
                    Dictionary<int,int> temp = LTFormationDataManager.Instance.HeroBattleTempPartner;
                    if (temp == null || temp.Count == 0)
                    {
                        MessageTemplateManager.ShowMessage(902314);
                        return;
                    }
                }
                else
                {
                    var temp = GetCurrentTeamMemList();
                    if (temp == null || temp.Count == 0)
                    {
                        MessageTemplateManager.ShowMessage(902314);
                        return;
                    }
                }
            }

            if (AllianceUtil.GetIsInTransferDart(null))
            {
                FusionAudio.PostEvent("UI/General/ButtonClick");
                return;
            }

            if (mStartBattleClick || mIsLoadingModel)
            {
                return;
            }
            if (sBattleType != eBattleType.SleepTower)
            {
                mStartBattleClick = true;
            }

            if (sBattleType == eBattleType.ArenaBattle)
            {
                controller.Close();
            }
            else if (sBattleType == eBattleType.ChallengeCampaign || sBattleType == eBattleType.AlienMazeBattle)
            {
                if (LTChallengeInstanceHpCtrl.GetCurHpSum() <= 0)
                {
                    FusionAudio.PostEvent("UI/General/ButtonClick");
                    MessageTemplateManager.ShowMessage(902258);
                    return;
                }
                else
                {
                    FusionAudio.PostEvent("UI/Battle/StartBattle");
                    StartCombatCallback();
                    return;
                }
            }
            else if (sBattleType == eBattleType.AllieanceFinalBattle)
            {
                StartCombatCallback();
                controller.Close();
            }
            else
            {
                FusionAudio.PostEvent("UI/Battle/StartBattle");
                StartCombatCallback();

                //关闭布阵时，重新设置伙伴上阵数据
                LTPartnerDataManager.Instance.InitPartnerBattleTeam();
            }
            FusionAudio.PostEvent("UI/General/ButtonClick");
        }

        void StartCombatCallback()
        {
            Hashtable ht = Param as Hashtable;
            if (ht != null && ht["startCombatCallback"] != null)
            {
                System.Action startCombatCallback = ht["startCombatCallback"] as System.Action;
                if (startCombatCallback != null)
                    startCombatCallback();
                OnPostUmengData();
            }
        }

        private void OnPostUmengData()
        {
            string level = null;
            if (sBattleType == eBattleType.MainCampaignBattle) level = LTMainInstanceCampaignCtrl.CampaignId.ToString();
            else if (sBattleType == eBattleType.InfiniteChallenge) DataLookupsCache.Instance.SearchDataByID<string>("infiniteChallenge.info.currentlayer", out level);
            else if (sBattleType == eBattleType.ExpSpringBattle || sBattleType == eBattleType.TreasuryBattle) level = LTResourceInstanceHudController.mChooseLevel.id.ToString();
            //上传友盟，开始挑战信息
            if (level != null) FusionTelemetry.PostStartCombat(((sBattleType == eBattleType.MainCampaignBattle) ? "Main" : sBattleType.ToString()) + level);
        }

        private eAttrTabType ParseTabType(string str)
        {
            if (str.Contains("All"))
                return eAttrTabType.All;
            else if (str.Contains("Feng"))
                return eAttrTabType.Feng;
            else if (str.Contains("Huo"))
                return eAttrTabType.Huo;
            else if (str.Contains("Shui"))
                return eAttrTabType.Shui;
            else if (str.Contains("Mercenary"))
                return eAttrTabType.Mercenary;
           EB.Debug.LogError("ParseTabType error str={0}" , str);
            return eAttrTabType.All;
        }

        #region create model
        /// <summary>
        /// 拖拽创建角色
        /// </summary>
        /// <param name="teamMemData"></param>
        /// <returns></returns>
        private void CreateModel(TeamMemberData teamMemData, System.Action<GameObject, TeamMemberData> fn)
        {
            if (controller.transform == null)
            {
                EB.Debug.LogError("CreateModel===>controller.transform == null");
                return;
            }
            if(sBattleType == eBattleType.HeroBattle)//英雄交锋
            {
                teamMemData.ModelName = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(teamMemData.InfoID, 0).model_name;
            }
            if (!ShowMercenary() && teamMemData.Uid>0)
            {
                mIsLoadingModel = false;
                return;
            }
            string prefab_path = $"Bundles/Player/Variants/{teamMemData.ModelName}-I";
            PoolModel.GetModelAsync(prefab_path, controller.transform.position, Quaternion.identity, delegate (UnityEngine.Object obj, object param)
            {
                GameObject variantObj = obj as GameObject;
                if (variantObj == null)
                {
                    EB.Debug.LogError("Failed to create hero game object");
                    return;
                }
                TeamMemberData callData = param as TeamMemberData;
                InitModel(variantObj, callData);
                modelList.Add(variantObj);
                fn(variantObj, callData);
            }, teamMemData);
        }

        private void InitModel(GameObject variantObj, TeamMemberData teamMemData)
        {
            if (variantObj == null)
            {
                EB.Debug.LogError("variantObj is null");
                return;
            }

            variantObj.transform.SetParent(controller.transform);
            variantObj.transform.localScale = Vector3.one;
            variantObj.transform.localRotation = Quaternion.Euler(-15, 0, 0);
            CharacterVariant variant = variantObj.GetComponent<CharacterVariant>();
            variant.InstantiateCharacter();
            GameObject character = variant.CharacterInstance;
            character.transform.SetParent(variant.transform);
            character.transform.localScale = modelScale;

            character.transform.localRotation = Quaternion.Euler(modelRotation);
            character.transform.localPosition = Vector3.zero;

            SetObjLayer(character, GameEngine.Instance.ui3dLayer);

            SmallPartnerModelData modelData = variantObj.AddMonoILRComponent<SmallPartnerModelData>("Hotfix_LT.UI.SmallPartnerModelData");
            modelData.modelData = teamMemData;


            MoveController mc = character.GetComponent<MoveController>();
            if (mc != null)
            {
                mc.InitAnimator();
            }
            MoveEditor.FXHelper fxHelper = character.GetComponent<MoveEditor.FXHelper>();
            fxHelper.PlayParticleAction = SetParticleOrder;
            //StartCoroutine (SetParticleOrderAndScale(character));
        }

        public void SetParticleOrder(ParticleSystem ps)
        {
            if (controller != null)
            {
                UIPanel up = controller.GetComponent<UIPanel>();
                if (ps != null && ps.GetComponent<Renderer>() != null)
                {
                    Renderer[] rens = ps.GetComponentsInChildren<Renderer>(true);
                    for (int j = 0; j < rens.Length; j++)//此处是为了防止粒子特效嵌套mesh by hzh
                    {
                        rens[j].sortingOrder = up.sortingOrder + 1;
                    }
                    //ps.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    var main = ps.main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    ps.transform.SetChildLayer(5);
                }
            }
        }

        public void SetObjLayer(GameObject _obj, int _nLayer)
        {
            _obj.layer = _nLayer;
            Renderer[] renderers = _obj.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; ++i)
            {
                renderers[i].gameObject.layer = _nLayer;
            }
        }

        IEnumerator SetParticleOrderAndScale(GameObject character)
        {
            FXLib fxLib = character.GetComponent<FXLib>();
            ParticleSystem[] sys = character.GetComponentsInChildren<ParticleSystem>();
            UIPanel up = controller.GetComponent<UIPanel>();
            for (int i = 0; i < sys.Length; i++)
            {
                if (sys[i] != null && sys[i].GetComponent<Renderer>() != null)
                {
                    Renderer[] rens = sys[i].GetComponentsInChildren<Renderer>(true);
                    for (int j = 0; j < rens.Length; j++)//此处是为了防止粒子特效嵌套mesh by hzh
                    {
                        rens[j].sortingOrder = up.sortingOrder + 1;
                    }

                    //sys[i].scalingMode = ParticleSystemScalingMode.Hierarchy;
                    var main = sys[i].main;
                    main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                    sys[i].transform.SetChildLayer(5);
                }
            }
            yield break;
        }

        private void ClearModelList(bool recycleFx = false)
        {
            if (controller != null && modelList != null)
            {
                for (int i = modelList.Count - 1; i >= 0; i--)
                {
                    if (modelList[i] == null)
                    {
                        continue;
                    }

                    if (recycleFx)
                    {
                        var psDestroy = modelList[i].GetComponentsInChildren<PSDestroyListener>();

                        if (psDestroy != null)
                        {
                            for (int j = 0; j < psDestroy.Length; j++)
                            {
                                ParticleSystem ps = psDestroy[j].gameObject.GetComponent<ParticleSystem>();
                                PSPoolManager.Instance.Recycle(ps);
                            }
                        }
                    }

                    SmallPartnerModelData md = modelList[i].GetMonoILRComponent<SmallPartnerModelData>();

                    if (md != null)
                    {
                        MoveEditor.FXHelper fxHelper = md.mDMono.transform.GetComponentInChildren<MoveEditor.FXHelper>(true);

                        if (fxHelper != null)
                        {
                            fxHelper.PlayParticleAction = null;
                        }

                        Object.Destroy(md.mDMono);
                    }
                    
                    PoolModel.DestroyModel(modelList[i]);
                }
                modelList.Clear();
            }
        }
        #endregion

        #region drag
        private void SetIconDragAction()
        {
            DynamicScroll.SetItemDragStartAction(OnModelDragStartByIcon);
            DynamicScroll.SetItemDragAction(OnModelDrag);
            DynamicScroll.SetItemDragEndAction(DragEndFunc);
        }

        public void DragEndFunc()
        {
            if (curDragType == eDragType.DragModel)
            {
                OnModelDragEnd();
            }
            else if (curDragType == eDragType.DragIcon)
            {
                OnModelDragEndByIcon();
            }
        }

		public void OnModelClicked(GameObject go)
		{
			if (LadderManager.Instance.IPrepareOK || curDragType != eDragType.None)
			{
				return;
			}

			Vector2 curClickPos = new Vector2(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y);
			Vector2 tempPos;
			GameObject accuracy = null;
			for (int i = 0; i < modelList.Count; i++)
			{
				tempPos = modelList[i].transform.position + modelDragOffset;
                //if (Vector2.Distance(tempPos, curClickPos) <= MIN_DRAG_DIST)
                if (JudgePosInclude(tempPos, curClickPos))
                {
                    float z = 100;
					if (modelList[i].transform.position.z < z)
						accuracy = modelList[i];
					break;
				}
			}

			if(accuracy)
			{
				TeamMemberData curModelData = accuracy.GetMonoILRComponent<SmallPartnerModelData>().modelData;
                if (curModelData.Uid > 0)
                {
                    LTFormationDataManager.Instance.GetMercenaryPlayerData(curModelData.Uid.ToString(), curModelData.HeroID,
                        (ha) =>
                        {
                            if (ha != null && ha.Count >= 1)
                            {
                                GlobalMenuManager.Instance.Open("LTPartnerInfoView", ha[0]);
                            }
                        });
                    return;
                }
				LTPartnerData parData = sBattleType == eBattleType.HeroBattle ? LTPartnerDataManager.Instance.GetPartnerByInfoId(curModelData.InfoID)
					: LTPartnerDataManager.Instance.GetPartnerByHeroId(curModelData.HeroID);

				if (curModelData.IsHire)parData = LTInstanceHireUtil.GetHirePartnerDataByHeroId(curModelData.HeroID);

				if (parData != null)
				{
					if (sBattleType == eBattleType.HeroBattle)
					{
						parData = LTFormationDataManager.Instance.GetHeroBattleDataByStatId(curModelData.InfoID + 1);
						parData = GetHeroBattlePatnerData(parData);

						OpenSimpleHeroTip(parData);
					}
					else
					{
						OtherPlayerPartnerData other = LTPartnerDataManager.Instance.Translated(parData);
						GlobalMenuManager.Instance.Open("LTPartnerInfoView", other);
					}
				}
			}
		}

		private void OpenSimpleHeroTip(LTPartnerData data)
		{
			Vector2 screenPos = UICamera.lastEventPosition;
			var ht = Johny.HashtablePool.Claim();
			ht.Add("id", data.InfoId);
			ht.Add("screenPos", screenPos);
			GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
		}

		/// <summary>
		/// 拖拽模型或者上阵头像开始（隐藏模型）
		/// </summary>
		public void OnModelDragStart(CombatPartnerCellController data = null)
        {
            if (LadderManager.Instance.IPrepareOK || curDragType != eDragType.None)
            {
                return;
            }

            curDragType = eDragType.DragModel;
            curDragModel = null;
            Vector2 curClickPos = new Vector2(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y);
            Vector2 tempPos;
            for (int i = 0; i < modelList.Count; i++)
            {
                tempPos = modelList[i].transform.position + modelDragOffset;
                //if (Vector2.Distance(tempPos, curClickPos) <= MIN_DRAG_DIST)
                if (JudgePosInclude(tempPos, curClickPos))
                {
                    float z = 100;
                    if (curDragModel != null)
                        z = curDragModel.transform.position.z;
                    if (modelList[i].transform.position.z < z)
                        curDragModel = modelList[i];
                }
            }

            if (data != null && curDragModel == null)
            {
                for (int i = 0; i < modelList.Count; i++)
                {
                    SmallPartnerModelData md = modelList[i].GetMonoILRComponent<SmallPartnerModelData>();
                    if (md.modelData.HeroID == data.ItemData.HeroId)
                    {
                        curDragModel = modelList[i];
                    }
                }
            }

            if (curDragModel != null)
            {
                if (sBattleType == eBattleType.SleepTower)
                {
                    TeamMemberData temp = curDragModel.GetMonoILRComponent<SmallPartnerModelData>().modelData;
                    if (!LTClimingTowerHudController.Instance.CanUpTeam(temp.HeroID))
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CLIMINGTOWER_DONT_UP"));
                        return;
                    }
                }

                SetVector(curDragModel.transform.position.x, curDragModel.transform.position.y, DRAG_Z, curDragModel.transform);
                TeamMemberData curModelData = curDragModel.GetMonoILRComponent<SmallPartnerModelData>().modelData;
                LTPartnerData parData = LTPartnerDataManager.Instance.GetPartnerByHeroId(curModelData.HeroID);
                if (curModelData.IsHire)
                {
                    parData = LTInstanceHireUtil.GetHirePartnerDataByHeroId(curModelData.HeroID);
                }
                if (sBattleType == eBattleType.HeroBattle)
                {
                    parData = LTFormationDataManager.Instance.GetHeroBattleDataByStatId(curModelData.InfoID + 1);
                    parData = GetHeroBattlePatnerData(parData);
                }

                if (parData==null)
                {
                    parData = AlliancesManager.Instance.GetAlliancePartnerByHeroId(curModelData.HeroID);
                }
                
                if (parData != null)
                {
                    DragPartnerCell.mDMono.gameObject.CustomSetActive(true);
                    curDragModel.CustomSetActive(false);
                    DragPartnerCell.mDMono.transform.position = curDragModel.transform.position;
                    DragPartnerCell.Fill(parData);
                }
            }
        }

        /// <summary>
        /// 拖拽模型或者上阵头像结束
        /// </summary>
        public void OnModelDragEnd()
        {
            if (curDragModel != null)//拖拽下阵
            {
                TeamMemberData curModelData = curDragModel.GetMonoILRComponent<SmallPartnerModelData>().modelData;
                Vector3 pos = Vector3.zero;

                if (curModelData != null && modelPosList != null &&modelPosList.Count>curModelData.Pos && modelPosList[curModelData.Pos] != null)
                {
                    pos = modelPosList[curModelData.Pos].transform.position;
                }

                if (IsModelDragOut() && (GetCurrentTeamMemList().Count > 1|| curModelData.HeroID<=0))
                {
                    if (curModelData != null && !curModelData.IsHire &&curModelData.HeroID>0)
                    {
                        if (!LTFormationDataManager.Instance.IsRequestDragoutVaild(curModelData.HeroID, FormationUtil.GetCurrentTeamName()) || sBattleType == eBattleType.LegionMercenary)
                        {
                            curDragModel.transform.position = new Vector3(pos.x + modelPos.x, pos.y + modelPos.y, pos.z + modelPos.z);
                            curDragModel.CustomSetActive(true);
                            PlayEntryAction(curDragModel);
                            curDragModel = null;
                            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                            curDragType = eDragType.None;
                            if(sBattleType != eBattleType.LegionMercenary)MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_NOT_HIRE_ALONE"));
                            return;
                        }
                    }

                    curDragModel.CustomSetActive(true);
                    OnModelDragOut(curDragModel);
                    curDragModel = null;
                    DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                    return;
                }

                bool isFindPos = false;//下面为拖拽模型换阵逻辑

                if (modelPosList != null)
                {
                    for (int i = modelPosList.Count - 1; i >= 0; i--)
                    {
                        //Vector2 tempPos = Vector2.zero;
                        //if (judgePosList != null && judgePosList[i] != null)
                        //{
                        //    tempPos = new Vector2(judgePosList[i].transform.position.x, judgePosList[i].transform.position.y);
                        //}
                        //if (Vector2.Distance(tempPos, curDragModel.transform.position) <= MIN_DRAG_IN_DIST)
                        if (judgePosList == null || judgePosList[i] == null) continue;
                        if (JudgePosInclude(judgePosList[i].transform.position, curDragModel.transform.position))
                        {
                            isFindPos = true;
                            GameObject targetModel = GetTargetModel(i);

                            if (curModelData != null && !GetCanAddHeroToFormation(curModelData, i, true))
                            {
                                //此处应该恢复模型原来位置防止拖到未解锁位置模型消失                      
                                DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                                curDragModel.transform.position = new Vector3(pos.x + modelPos.x, pos.y + modelPos.y, pos.z + modelPos.z);
                                curDragType = eDragType.None;
                                curDragModel.CustomSetActive(true);
                                PlayEntryAction(curDragModel);
                                curDragModel = null;
                                return;
                            }
                            
                            bool samePosition = targetModel != null && curModelData.Pos == i;
                            OnDrag2ModelPos(curModelData.HeroID,curModelData.InfoID+1, i, !samePosition, curModelData.IsHire);
                            if (targetModel != null)//模型 上阵头像互换
                            {
                                targetModel.transform.position = pos + modelPos;
                                var spmd = targetModel.GetMonoILRComponent<SmallPartnerModelData>();

                                if (spmd != null)
                                {
                                    spmd.modelData.Pos = curModelData.Pos;
                                }
                            }
                            SetVector(modelPosList[i].transform.position.x + modelPos.x, modelPosList[i].transform.position.y + modelPos.y, modelPosList[i].transform.position.z + modelPos.z, curDragModel.transform);
                            curModelData.Pos = i;
                            break;
                        }
                    }
                }

                if (!isFindPos)
                {
                    curDragModel.transform.position = new Vector3(pos.x + modelPos.x, pos.y + modelPos.y, pos.z + modelPos.z);
                }

                curDragModel.CustomSetActive(true);
                PlayEntryAction(curDragModel);
            }

            curDragModel = null;
            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
            curDragType = eDragType.None;
        }

        private GameObject GetTargetModel(int index)
        {
            for (int i = 0; i < modelList.Count; i++)
            {
                SmallPartnerModelData md = modelList[i].GetMonoILRComponent<SmallPartnerModelData>();
                if (md.modelData != null)
                {
                    if (md.modelData.Pos == index)
                        return modelList[i];
                }
            }
            return null;
        }

        private bool IsModelDragOut()
        {
            return !controller.BoxColliders["ModelPlaceArea"].bounds.Contains(UICamera.lastWorldPosition);
        }

        /// <summary>
        /// 请求下阵
        /// </summary>
        /// <param name="modelObj"></param>
        /// <param name="isNeedRequest"></param>
        public void OnModelDragOut(GameObject modelObj, bool isNeedRequest = true)
        {
            if (modelObj == null)
            {
                return;
            }
            var smallPartner = modelObj.GetMonoILRComponent<SmallPartnerModelData>();
            TeamMemberData curModelData = smallPartner.modelData;
            modelList.Remove(modelObj);
			Object.Destroy(smallPartner.mDMono);
            PoolModel.DestroyModel(modelObj);
            if (isNeedRequest)
            {
                LTCombatHudController.ResetAutoSkill();
                if (!curModelData.IsHire)
                {
                    LTFormationDataManager.Instance.RequestRemoveHeroFormation(curModelData.HeroID,curModelData.InfoID+1, FormationUtil.GetCurrentTeamName(), delegate ()
                    {
                        RefreshPartnerList(mCurPartnerTabType);
                    });
                }
                else if (curModelData.Uid>0)
                {
                    LTFormationDataManager.Instance.UnUseAllianceMercenary(curModelData.HeroID,curModelData.Pos, () =>
                    {
                        RefreshPartnerList(mCurPartnerTabType);
                    });
                }else
                {
                    LTFormationDataManager.Instance.RequestRemoveHeroFormation(-curModelData.HeroID, FormationUtil.GetCurrentTeamName(), delegate ()
                    {
                        RefreshPartnerList(mCurPartnerTabType);
                    });
                }
            }
        }

        private void SetVector(float x, float y, float z, Transform go)
        {
            Vector3 curr = go.position;
            curr.x = x;
            curr.y = y;
            curr.z = z;
            go.position = curr;
        }
        private void SetVector(float x, float y, out Vector2 vec)
        {
            vec.x = x;
            vec.y = y;
        }

        public void OnModelDrag()
        {
            if (curDragModel == null)
            {
                return;
            }
            SetVector(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y - DRAG_OFFSET_DIST, curDragModel.transform.position.z, curDragModel.transform);
            SetVector(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y, curDragModel.transform.position.z, DragPartnerCell.mDMono.transform);
        }

        /// <summary>
        /// 请求上阵
        /// </summary>
        /// <param name="heroID"></param>
        /// <param name="index"></param>
        /// <param name="isNeedReq"></param>
        /// <param name="isHire"></param>
        private void OnDrag2ModelPos(int heroID,int statId, int index, bool isNeedReq, bool isHire)
        {
            if (isNeedReq)
            {
                LTCombatHudController.ResetAutoSkill();
                if (sBattleType == eBattleType.LegionMercenary)
                {
                    LTPartnerData data= LTPartnerDataManager.Instance.GetPartnerByHeroId(heroID);
                    AlliancesManager.Instance.SetAllianceMarcenary(heroID,data.powerData.curPower,(bo)=>{RefreshPartnerList(mCurPartnerTabType);});
                    return;
                }
                heroID = isHire ? -heroID : heroID;
                
                //1.老的雇佣兵 存在于主线副本
                if (isHire && sBattleType == eBattleType.MainCampaignBattle)
                {
                    LTFormationDataManager.Instance.RequestDragHeroToFormationPos(heroID, index, FormationUtil.GetCurrentTeamName(), delegate () { RefreshPartnerList(mCurPartnerTabType); });
                    return;
                }
                //2.在能上军团雇佣兵的team走这里
                if ( LTFormationDataManager.Instance.ContainMercenaryType())
                {
                    LTFormationDataManager.Instance.RequestDragHeroToFormationPosWithMer(heroID,index,FormationUtil.GetCurrentTeamName(),delegate () { RefreshPartnerList(mCurPartnerTabType); });
                    return;
                }
                //3.其他情况
                LTFormationDataManager.Instance.RequestDragHeroToFormationPos(heroID,statId, index, FormationUtil.GetCurrentTeamName(), delegate () { RefreshPartnerList(mCurPartnerTabType); });
            }
        }

        void PlayEntryAction(GameObject variantOBJ)
        {
            if (variantOBJ.activeInHierarchy)
            {
                CharacterVariant cv = variantOBJ.GetComponent<CharacterVariant>();
                MoveController mc = cv.CharacterInstance.GetComponent<MoveController>();
                System.Action fn = ()=>{
                    mc.TransitionTo(MoveController.CombatantMoveState.kEntry);
                    mc.CrossFade(MoveController.m_entry_hash, 0.2f, 0, 0f);
                    StartCoroutine(TransitionToIdleState(mc));
                };
                if(!mc.IsInitialized){
                    mc.RegisterInitSuccCallBack(fn);
                }
                else
                {
                   fn();
                }
            }
        }

        IEnumerator TransitionToIdleState(MoveController mc)
        {
            yield return new WaitForSeconds(1);
            if ((int)mc.CurrentState == (int)MoveController.CombatantMoveState.kEntry)
            {
                mc.TransitionTo(MoveController.CombatantMoveState.kIdle);
                mc.CrossFade(MoveController.m_idle_hash, 0.2f, 0, 0f);
            }
        }

        private Vector3 tempVec;
        private CombatPartnerCellController tempCell;

        /// <summary>
        /// 拖拽Icon上阵开始
        /// </summary>
        /// <param name="partnerCell"></param>
        public void OnModelDragStartByIcon(CombatPartnerCellController partnerCell)
        {
            if (curDragType != eDragType.None)
            {
                return;
            }
            curDragType = eDragType.DragIcon;
            curDragModel = null;

            var partnerData = partnerCell.ItemData;
            if ((sBattleType == eBattleType.ChallengeCampaign || sBattleType == eBattleType.AlienMazeBattle) && !FormationUtil.IsAlive(partnerData.HeroId, partnerData.IsHire) && partnerCell.ItemData.uid<=0)
            {
                MessageTemplateManager.ShowMessage(902112);
                return;
            }

            if (ShowMercenary() && LTFormationDataManager.Instance.TeamMemListHasSame(partnerData.InfoId))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_BATTLEREADY_TIP"));
                return;
            }
            
            if (partnerCell.ShowUseFlag())
            {
                return;
            }
            if (sBattleType == eBattleType.HeroBattle)
            {
                int i;
                if (LTNewHeroBattleManager.GetInstance().GetCostByRoleGrade(partnerData.HeroInfo.role_grade) > Surplus && 
                    isNeedShowConstProgressBar(out i))//花费过高
                {
                    MessageTemplateManager.ShowMessage(902312);
                    return;
                }
                if (LTNewHeroBattleManager.GetInstance().HasChallengeHeroInfoID.Contains(partnerData.StatId)&& 
                    isNeedShowConstProgressBar(out i))//已上阵过了
                {
                    MessageTemplateManager.ShowMessage(902313);
                    return;
                }
            }
            if (sBattleType == eBattleType.SleepTower)
            {
                if (!LTClimingTowerHudController.Instance.CanUpTeam(partnerData.HeroId))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_CLIMINGTOWER_DONT_UP"));
                    return;
                }
            }
            if (IsInTeam(partnerData.HeroId) || !FormationUtil.IsHave(partnerData))
            {
                return;
            }

            tempCell = partnerCell;
            tempVec = partnerCell.mDMono.transform.localPosition;
            tempCell.mDMono.transform.localPosition = new Vector3(tempVec.x, tempVec.y + 1000, tempVec.z);
            TeamMemberData teamMemData = FormationUtil.NewTeamMemberData(partnerData, -1);
            CreateModel(teamMemData, delegate (GameObject go, TeamMemberData callData) {
                curDragModel = go;
                curDragModel.CustomSetActive(false);
                DragPartnerCell.mDMono.gameObject.CustomSetActive(true);
                DragPartnerCell.Fill(partnerData);
                if (curDragModel != null)
                {
                    DragPartnerCell.mDMono.transform.position = new Vector3(curDragModel.transform.position.x, curDragModel.transform.position.y, DRAG_Z);
                    curDragModel.transform.position = DragPartnerCell.mDMono.transform.position;
                }
                OnModelDrag();
            });
        }

        /// <summary>
        /// 拖拽Icon上阵结束
        /// </summary>
        public void OnModelDragEndByIcon()
        {
            if (curDragModel != null)
            {
                bool isNeedResetPos = true;
                curDragModel.CustomSetActive(true);
                StartCoroutine(SetParticleOrderAndScale(curDragModel));
                TeamMemberData curModelData = curDragModel.GetMonoILRComponent<SmallPartnerModelData>().modelData;
                if (IsModelDragOut())
                {
                    OnModelDragOut(curDragModel, false);
                    curDragModel = null;
                    DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                    tempCell.mDMono.transform.localPosition = tempVec;
                    return;
                }
                bool isFindPos = false;
                Vector2 tempPos = Vector2.zero;
                for (int i = modelPosList.Count - 1; i >= 0; i--)
                {
                    if (!judgePosList[i].activeInHierarchy) continue;
                    //SetVector(judgePosList[i].transform.position.x, judgePosList[i].transform.position.y, out tempPos);
                    //if (Vector2.Distance(tempPos, curDragModel.transform.position) <= MIN_DRAG_IN_DIST)
                    if(JudgePosInclude(judgePosList[i].transform.position, curDragModel.transform.position))
                    {
                        isNeedResetPos = false;
                        GameObject targetModel = GetTargetModel(i);

                        if (!GetOnlyOneFire(curModelData, i))  //雇佣兵只能上阵一个
                        {
                            OnModelDragOut(curDragModel, false);
                            curDragModel = null;
                            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                            tempCell.mDMono.transform.localPosition = tempVec;
                            return;
                        }
                        
                        if (targetModel != null)  //如果阵形中已有该家伙
                        {
                            if (curModelData.IsHire)
                            {
                                if (!LTFormationDataManager.Instance.IsRequestDragInVaild(i, FormationUtil.GetCurrentTeamName()))
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_NOT_HIRE_ALONE"));
                                    OnModelDragOut(curDragModel, false);
                                    curDragModel = null;
                                    DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                                    tempCell.mDMono.transform.localPosition = tempVec;
                                    break;
                                }
                            }
                            OnModelDragOut(targetModel, false);
                        }
                        else if (!GetCanAddHeroToFormation(curModelData, i))
                        {
                            OnModelDragOut(curDragModel, false);
                            curDragModel = null;
                            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
                            tempCell.mDMono.transform.localPosition = tempVec;
                            return;
                        }
                        isFindPos = true;
                        curModelData.Pos = i;
                        SetVector(modelPosList[i].transform.position.x + modelPos.x, modelPosList[i].transform.position.y + modelPos.y, modelPosList[i].transform.position.z + modelPos.z, curDragModel.transform);
                        OnDrag2ModelPos(curModelData.HeroID,curModelData.InfoID+1, i, true, curModelData.IsHire);
                        PlayEntryAction(curDragModel);
                        break;
                    }
                }
                if (!isFindPos)
                {
                    OnModelDragOut(curDragModel, false);
                }

                if (tempCell != null && isNeedResetPos)
                {
                    tempCell.mDMono.transform.localPosition = tempVec;
                }
            }
            curDragModel = null;
            DragPartnerCell.mDMono.gameObject.CustomSetActive(false);
            curDragType = eDragType.None;
        }

        WaitForSeconds wait01 = new WaitForSeconds(0.1f);
        private IEnumerator RepositionCell()
        {
            yield return wait01;
        }
        #endregion

        private int clickCount = 0;
        public override void OnCancelButtonClick()
        {
            //创建模型中不能返回
            if (mIsLoadingModel)
            {
                return;
            }
            //新手引导特殊处理
            if (GuideNodeManager.IsGuide && GuideToolController.Instance.controller.gameObject.activeSelf && !LTInstanceUtil.IsFirstChapterCompleted())
            {
                if (clickCount >= 3)
                {
                    clickCount = 0;
                    MessageTemplateManager.ShowMessage(901099, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            GuideNodeManager.currentGuideId = 0;
                            GuideNodeManager.GetInstance().JumpGuide();//跳过主线
                        }
                        return;
                    });
                }
                clickCount++;
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString(GuideNodeManager.GUIDE_CANNOT_RETURN));
                return;
            }

            ClearModelList(true);  //关闭界面时就清理模型特效，回收，不然在ondestrory取不到特效节点

            if (curDragType != eDragType.None) DragEndFunc();

            //关闭布阵时，重新设置伙伴上阵数据
            LTPartnerDataManager.Instance.InitPartnerBattleTeam();           
            base.OnCancelButtonClick();
        }

        /// <summary>
        /// 摧毁对象
        /// </summary>
        public override void OnDestroy()
        {
            ClearModelList(true);  //切战斗场景时清空
            base.OnDestroy();
        }

        public void CheckEnemyInfoBtnClick()
        {
            //添加查看敌人信息
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.5f);
            Hashtable ht = Param as Hashtable;


            if (ht != null && ht["enemyLayout"] != null)
            {
                string enemyLayout = ht["enemyLayout"] as string;
                long uid = 0;
                if (long.TryParse(enemyLayout, out uid))
                {
                    Hashtable viewData = Johny.HashtablePool.Claim();
                    viewData["infoType"] = eOtherPlayerInfoType.secret;
                    viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0] = uid;
                    viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE2;
                    viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE0;
                    GlobalMenuManager.Instance.Open("LTCheckPlayerFormationInfoUI", viewData);
                }
                else
                {
                    GlobalMenuManager.Instance.Open("LTCheakEnemyHud", enemyLayout);
                }
            }
        }
        
        public static void SetBattleType(eBattleType battleType)
        {
            sBattleType = battleType;
        }

        public static void Open(eBattleType battleType, System.Action startCombatCallback, string enemyLayout = null, Hotfix_LT.Data.eRoleAttr roleAttr = Hotfix_LT.Data.eRoleAttr.None, BossChallengeData challengeData = default)
        {
            sBattleType = battleType;
            if (roleAttr != Hotfix_LT.Data.eRoleAttr.None) sRoleAttr = roleAttr;

			var ht = Johny.HashtablePool.Claim();
            ht.Add("startCombatCallback", startCombatCallback);
            ht.Add("enemyLayout", enemyLayout);

			ChallengeData = challengeData;

            GlobalMenuManager.Instance.Open("LTCombatReadyUI", ht);
        }

        private const float MIN_DRAG_IN_DIST_X = 0.3f;
        private const float MIN_DRAG_IN_DIST_Y = 0.5f;
        public bool JudgePosInclude(Vector3 pos1,Vector3 pos2)
        {
            return Mathf.Abs(pos1.x- pos2.x)< MIN_DRAG_IN_DIST_X && Mathf.Abs(pos1.y - pos2.y) < MIN_DRAG_IN_DIST_Y;
        }
    }

	public struct BossChallengeData
	{
		public int UnitValue;
		public string BuffName;
		public string DescribeFormat;
	}
}