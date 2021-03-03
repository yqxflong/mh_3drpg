using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class OnPartnerTurnToUpgrade : GameEvent
    {
        public CultivateType type;
        public OnPartnerTurnToUpgrade(CultivateType type)
        {
            this.type = type;
        }
    }

    public class LTPartnerCultivateController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            LevelUpMaxObj = t.FindEx("Title/TopLevelAttr").gameObject;
            GuardObj = t.FindEx("Title/GuardAttr").gameObject;
            LevelUpMaxLevelLabel = t.GetComponent<UILabel>("Title/TopLevelAttr/Label");
            GuardLevelLabel = t.GetComponent<UILabel>("Title/GuardAttr/Label");
            isOpenCulView = false;
            AttributesRedPoint = t.FindEx("Title/BtnList/InfoBtn/RedPoint").gameObject;
            GradeUpRedPoint = t.FindEx("Title/BtnList/UpGradeBtn/RedPoint").gameObject;
            StarUpRedPoint = t.FindEx("Title/BtnList/StarUpBtn/RedPoint").gameObject;
            SkillBreakRedPoint = t.FindEx("Title/BtnList/SkillBtn/RedPoint").gameObject;
            TopLevelRedPoint = t.FindEx("Title/TopLevelAttr/RedPoint").gameObject;
            GuardRedPoint = t.FindEx("Title/GuardAttr/RedPoint").gameObject;
            AwakeRedPoint = t.FindEx("Title/BtnList/AwakenBtn/RedPoint").gameObject;
            SkinBtnObj = t.FindEx("Title/SkinBtn").gameObject;
            LevelLabel = t.GetComponent<UILabel>("Info/Label/LevelLabel");
            ExpLabel = t.GetComponent<UILabel>("Info/ExpLabel/ExpBar/Label");
            ExpSlider = t.GetComponent<UISlider>("Info/ExpLabel/ExpBar/Sprite");
            AttackLabel = t.GetComponent<UILabel>("Info/AttrList/0/NumLabel");
            DefLabel = t.GetComponent<UILabel>("Info/AttrList/1/NumLabel");
            LifeLabel = t.GetComponent<UILabel>("Info/AttrList/2/NumLabel");
            CritLabel = t.GetComponent<UILabel>("Info/AttrList/3/NumLabel");
            CritVLabel = t.GetComponent<UILabel>("Info/AttrList/4/NumLabel");
            ComboLabel = t.GetComponent<UILabel>("Info/AttrList/5/NumLabel");
            SpExtraLabel = t.GetComponent<UILabel>("Info/AttrList/6/NumLabel");
            SpResLabel = t.GetComponent<UILabel>("Info/AttrList/7/NumLabel");
            EquipAttackLabel = t.GetComponent<UILabel>("Info/AttrList/0/EquipNumLabel");
            EquipDefLabel = t.GetComponent<UILabel>("Info/AttrList/1/EquipNumLabel");
            EquipLifeLabel = t.GetComponent<UILabel>("Info/AttrList/2/EquipNumLabel");
            EquipCritLabel = t.GetComponent<UILabel>("Info/AttrList/3/EquipNumLabel");
            EquipCritVLabel = t.GetComponent<UILabel>("Info/AttrList/4/EquipNumLabel");
            EquipComboLabel = t.GetComponent<UILabel>("Info/AttrList/5/EquipNumLabel");
            EquipSpExtraLabel = t.GetComponent<UILabel>("Info/AttrList/6/EquipNumLabel");
            EquipSpResLabel = t.GetComponent<UILabel>("Info/AttrList/7/EquipNumLabel");

            AttrAni = new TweenScale[3];
            AttrAni[0] = t.GetComponent<TweenScale>("Info/AttrList/0/NumLabel");
            AttrAni[1] = t.GetComponent<TweenScale>("Info/AttrList/1/NumLabel");
            AttrAni[2] = t.GetComponent<TweenScale>("Info/AttrList/2/NumLabel");

            LvUpLabelTweenPos = t.GetComponent<TweenPosition>("Info/LvUpTextAni/StarUpTitle");
            LvUpLabelTweenAlp = t.GetComponent<TweenAlpha>("Info/LvUpTextAni/StarUpTitle");
            ExpSliderAni = t.GetMonoILRComponent<SliderAni>("Info/ExpLabel/ExpBar/Sprite");
            ExpNumAni = t.GetMonoILRComponent<NumAni>("Info/ExpLabel/ExpBar/Label");
            AttrDesc = t.GetMonoILRComponent<LTPartnerAttrDesc>("AttrDesc");
            StarUpDesc = t.GetMonoILRComponent<LTPartnerStarUpDesc>("StarUpDesc");
            ExItem = t.GetMonoILRComponent<LTPartnerListCellController>("UpGrade/ExItem");
            CurItem = t.GetMonoILRComponent<LTPartnerListCellController>("UpGrade/CurItem");
            upgradeDatalookup = t.GetDataLookupILRComponent<PartnerAwakenCusumeDataLookUp>("UpGrade/Cost");
            UpGradeCostItemList = new List<LTShowItem>();
            UpGradeCostItemList.Add(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item"));
            UpGradeCostItemList.Add(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item (1)"));
            UpGradeCostItemList.Add(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item (2)"));
            UpGradeCostItemList.Add(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item (3)"));
            ItemGrid = t.GetComponent<UIGrid>("UpGrade/Cost");
            GradeLimitLabel = t.GetComponent<UILabel>("UpGrade/Condition/NumLabel");
            GradeUpIncomeLabel = t.GetComponent<UILabel>("UpGrade/Income/NumLabel");
            ShardOwnNumLabel = t.GetComponent<UILabel>("StarUp/OwnShard/NumLabel");
            StarUpTip = t.GetComponent<UILabel>("StarUp/Label");

            StarUpBG = new GameObject[3];
            StarUpBG[0] = t.FindEx("StarUp/BG/BGFeng").gameObject;
            StarUpBG[1] = t.FindEx("StarUp/BG/BGShui").gameObject;
            StarUpBG[2] = t.FindEx("StarUp/BG/BGHuo").gameObject;


            StarUpAttrTranList = new List<Transform>();
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (1)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (2)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (3)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (4)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (5)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (6)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (7)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (8)"));
            StarUpAttrTranList.Add(t.GetComponent<Transform>("StarUp/AttrList/AttrItem (9)"));

            TipBtnSprite = t.GetComponent<UISprite>("StarUp/TipBtn/TipBtnSprite");
            TipBtnSpriteFrame = t.GetComponent<UISprite>("StarUp/TipBtn/TipBtnSprite/Sprite");
            StarUpBtn = t.GetComponent<UIButton>("StarUp/StarUpBtn");
            StarHoleUpBtn = t.GetComponent<UIButton>("StarUp/StarHoleUpBtn");
            ProficiencyUpView = t.GetMonoILRComponent<LTPartnerProficiencyUpController>("StarUp/ProficiencyView");
            HeroClipChangeDataLookup = t.GetDataLookupILRComponent<LTPartnerDataLookup>("StarUp/OwnShard/NumLabel");
            StarHoleNextFx = t.GetComponent<ParticleSystemUIComponent>("StarUp/FxNext");
            StarHoleCurPlayFx = t.GetComponent<ParticleSystemUIComponent>("StarUp/FxCur");
            ChipTransController = t.GetMonoILRComponentByClassPath<LTPartnerChipTransController>("StarUp/GenericTrans", "Hotfix_LT.UI.LTPartnerChipTransController");
            chipsshowobj = t.Find("StarUp/OwnChips").gameObject;
            ChipTypeSprite = t.GetComponent<DynamicUISprite>("StarUp/OwnChips/Sprite");
            ChipNum = t.GetComponent<UILabel>("StarUp/OwnChips/NumLabel");
            commonsprite = t.GetComponent<UISprite>("Skill/CommonSkill/SkillItem");
            CommonSkillBreakSprite = t.GetComponent<DynamicUISprite>("Skill/CommonSkill/SkillItem/Icon");
            CommonSkillBreakNameLabel = t.GetComponent<UILabel>("Skill/CommonSkill/NameLabel");
            CommonSkillBreakLevelLabel = t.GetComponent<UILabel>("Skill/CommonSkill/LevelLabel");
            CommonSkillLevel = t.GetComponent<UILabel>("Skill/CommonSkill/SkillItem/Sprite/Level");
            CommonSkillType = t.GetComponent<UILabel>("Skill/CommonSkill/SkillIconBG/Label");
            CommonSkillTypeBG = t.GetComponent<UISprite>("Skill/CommonSkill/SkillIconBG");
            passivesprite = t.GetComponent<UISprite>("Skill/PassiveSkill/SkillItem");
            PassiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("Skill/PassiveSkill/SkillItem/Icon");
            PassiveSkillBreakNameLabel = t.GetComponent<UILabel>("Skill/PassiveSkill/NameLabel");
            PassiveSkillBreakLevelLabel = t.GetComponent<UILabel>("Skill/PassiveSkill/LevelLabel");
            PassiveSkillLevel = t.GetComponent<UILabel>("Skill/PassiveSkill/SkillItem/Sprite/Level");
            PassiveSkillType = t.GetComponent<UILabel>("Skill/PassiveSkill/SkillIconBG/Label");
            PassiveSkillTypeBG = t.GetComponent<UISprite>("Skill/PassiveSkill/SkillIconBG");
            activesprite = t.GetComponent<UISprite>("Skill/ActiveSkill/SkillItem");
            ActiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("Skill/ActiveSkill/SkillItem/Icon");
            ActiveSkillBreakNameLabel = t.GetComponent<UILabel>("Skill/ActiveSkill/NameLabel");
            ActiveSkillBreakLevelLabel = t.GetComponent<UILabel>("Skill/ActiveSkill/LevelLabel");
            ActiveSkillLevel = t.GetComponent<UILabel>("Skill/ActiveSkill/SkillItem/Sprite/Level");
            ActiveSkillType = t.GetComponent<UILabel>("Skill/ActiveSkill/SkillIconBG/Label");
            ActiveSkillTypeBG = t.GetComponent<UISprite>("Skill/ActiveSkill/SkillIconBG");

            CultivateBodyObj = new GameObject[5];
            CultivateBodyObj[0] = t.FindEx("Info").gameObject;
            CultivateBodyObj[1] = t.FindEx("UpGrade").gameObject;
            CultivateBodyObj[2] = t.FindEx("StarUp").gameObject;
            CultivateBodyObj[3] = t.FindEx("Skill").gameObject;
            CultivateBodyObj[4] = t.FindEx("AwakenView").gameObject;

            TitleController = t.GetMonoILRComponent<TitleListController>("Title");

            TabBtnList = new List<UIButton>();
            TabBtnList.Add(t.GetComponent<UIButton>("Title/BtnList/InfoBtn"));
            TabBtnList.Add(t.GetComponent<UIButton>("Title/BtnList/UpGradeBtn"));
            TabBtnList.Add(t.GetComponent<UIButton>("Title/BtnList/StarUpBtn"));
            TabBtnList.Add(t.GetComponent<UIButton>("Title/BtnList/SkillBtn"));
            TabBtnList.Add(t.GetComponent<UIButton>("Title/BtnList/AwakenBtn"));

            CulTitle = t.FindEx("Title").gameObject;
            CulBG = t.FindEx("BG").gameObject;
            AwakenSkillTipObj = t.FindEx("AwakenView/showLabel/Reward/ExtralGet/Sprite").gameObject;
            AwakenConsumeObj = t.FindEx("AwakenView/Consume").gameObject;
            AwakenSkinTip = t.FindEx("AwakenView/HeadIcon/ShowItemAwaken/Sprite").gameObject;
            headHideObj = t.FindEx("AwakenView/HeadIcon/HideItem").gameObject;
            HeadShowObj = t.FindEx("AwakenView/HeadIcon/ShowItemAwaken").gameObject;
            SkinSelectObj = t.FindEx("AwakenView/SkillSwitchBtn").gameObject;
            OriginSkinHandle = t.FindEx("AwakenView/SkillSwitchBtn/AwakeBtn (1)/Handle").gameObject;
            AwakenSkinHandle = t.FindEx("AwakenView/SkillSwitchBtn/AwakeBtn (2)/Handle").gameObject;
            ConditionNameLabel = t.FindEx("AwakenView/showLabel/Condit/Condition").gameObject;
            ConsumeObj = t.FindEx("AwakenView/Consume").gameObject;
            QuicklyGetBtn = t.FindEx("UpGrade/QuicklyCollecteBtn").gameObject;
            UpGradeBtn = t.FindEx("UpGrade/UpGradeBtn").gameObject;
            OldCell = t.GetMonoILRComponent<LTPartnerListCellController>("AwakenView/HeadIcon/HideItem/PreItem");
            PreviewCell = t.GetMonoILRComponent<LTPartnerListCellController>("AwakenView/HeadIcon/ShowItemAwaken/AwakenItem");
            AwakenCondition = t.GetComponent<UILabel>("AwakenView/showLabel/Condit/conditioncont");
            AwakenRewardLabel = t.GetComponent<UILabel>("AwakenView/showLabel/Reward/ExtralGet/Label");
            AwakenBtnLabel = t.GetComponent<UILabel>("AwakenView/AwakeBtn/Label");
            HasAwkenLabel = t.GetComponent<UILabel>("AwakenView/showLabel/Condit/HasAwaken");           

            RewardLabel = new UILabel[4];
            RewardLabel[0] = t.GetComponent<UILabel>("AwakenView/showLabel/Reward/Grid/Attack/BG/Label");
            RewardLabel[1] = t.GetComponent<UILabel>("AwakenView/showLabel/Reward/Grid/Def/BG/Label");
            RewardLabel[2] = t.GetComponent<UILabel>("AwakenView/showLabel/Reward/Grid/Life/BG/Label");
            RewardLabel[3] = t.GetComponent<UILabel>("AwakenView/showLabel/Reward/Grid/Speed/BG/Label");

            SkillCompare = t.GetMonoILRComponent<LTPartnerAwakenSkillCompare>("AwakenView/AwakenSkillPanel");

            ConsumeArray = new AwakenConsumeController[3];
            ConsumeArray[0] = t.GetMonoILRComponent<AwakenConsumeController>("AwakenView/Consume/Cost/item");
            ConsumeArray[1] = t.GetMonoILRComponent<AwakenConsumeController>("AwakenView/Consume/Cost/item (1)");
            ConsumeArray[2] = t.GetMonoILRComponent<AwakenConsumeController>("AwakenView/Consume/Cost/item (2)");

            AwakenConsumeGrid = t.GetComponent<UIGrid>("AwakenView/Consume/Cost");
            AwakenBtn = t.GetComponent<Transform>("AwakenView/AwakeBtn");
            MaterialdatalookUp = t.GetDataLookupILRComponent<PartnerAwakenCusumeDataLookUp>("AwakenView/Consume/Cost");
            TitleGrid = t.GetComponent<UIGrid>("Title/BtnList");

            TitleBtnObj = new List<GameObject>();
            TitleBtnObj.Add(t.FindEx("Title/BtnList/InfoBtn").gameObject);
            TitleBtnObj.Add(t.FindEx("Title/BtnList/UpGradeBtn").gameObject);
            TitleBtnObj.Add(t.FindEx("Title/BtnList/StarUpBtn").gameObject);
            TitleBtnObj.Add(t.FindEx("Title/BtnList/SkillBtn").gameObject);
            TitleBtnObj.Add(t.FindEx("Title/BtnList/AwakenBtn").gameObject);

            SkinView = t.GetMonoILRComponent<LTPartnerSkinController>("Skin");
            IsSkinViewOpen = false;

            t.GetComponent<UIButton>("Title/BtnList/InfoBtn").onClick.Add(new EventDelegate(() => OnTabBtnClick(t.GetComponent<UIButton>("Title/BtnList/InfoBtn"))));
            t.GetComponent<UIButton>("Title/BtnList/UpGradeBtn").onClick.Add(new EventDelegate(() => OnTabBtnClick(t.GetComponent<UIButton>("Title/BtnList/UpGradeBtn"))));
            t.GetComponent<UIButton>("Title/BtnList/StarUpBtn").onClick.Add(new EventDelegate(() => OnTabBtnClick(t.GetComponent<UIButton>("Title/BtnList/StarUpBtn"))));
            t.GetComponent<UIButton>("Title/BtnList/SkillBtn").onClick.Add(new EventDelegate(() => OnTabBtnClick(t.GetComponent<UIButton>("Title/BtnList/SkillBtn"))));
            t.GetComponent<UIButton>("Title/BtnList/AwakenBtn").onClick.Add(new EventDelegate(() => OnTabBtnClick(t.GetComponent<UIButton>("Title/BtnList/AwakenBtn"))));
            t.GetComponent<UIButton>("Title/TopLevelAttr").onClick.Add(new EventDelegate(OnParterMaxLevelFuncBtnClick));
            t.GetComponent<UIButton>("Title/GuardAttr").onClick.Add(new EventDelegate(OnGuardFuncBtnClick));
            t.GetComponent<UIButton>("Info/AttrList/0").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/0").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/1").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/1").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/2").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/2").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/3").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/3").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/4").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/4").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/5").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/5").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/6").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/6").gameObject)));
            t.GetComponent<UIButton>("Info/AttrList/7").onClick.Add(new EventDelegate(() => OnAttrBtnClick(t.FindEx("Info/AttrList/7").gameObject)));
            t.GetComponent<UIButton>("StarUp/OwnShard/AddBtn").onClick.Add(new EventDelegate(OnShardBtnClick));
            t.GetComponent<UIButton>("StarUp/TipBtn").onClick.Add(new EventDelegate(OnStarUpTipBtnClick));

            t.GetComponent<UIButton>("Skill/CommonSkill/Btn").onClick.Add(new EventDelegate(OnCommonSkillBreakBtnClick));
            t.GetComponent<UIButton>("Skill/ActiveSkill/Btn").onClick.Add(new EventDelegate(OnActiveSkillBreakBtnClick));
            t.GetComponent<UIButton>("Skill/PassiveSkill/Btn").onClick.Add(new EventDelegate(OnPassiveSkillBreakBtnClick));
            t.GetComponent<UIButton>("AwakenView/showLabel/Reward/ExtralGet/Sprite").onClick.Add(new EventDelegate(OnCliCkAwakenSkillTip));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Title/SkinBtn").clickEvent.Add(new EventDelegate(() => OnSkinViewOpenAndClose()));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Info/LevelBtn").clickEvent.Add(new EventDelegate(OnLevelUpBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Info/MulitLevelBtn").clickEvent.Add(new EventDelegate(OnMulitLevelUpBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Info/DetailedInfoBtn").clickEvent.Add(new EventDelegate(OnDetailedInfoBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpGrade/Cost/Item").clickEvent.Add(new EventDelegate(() => OnClickGradeUpShowItem(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item"))));
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpGrade/Cost/Item (1)").clickEvent.Add(new EventDelegate(() => OnClickGradeUpShowItem(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item (1)"))));
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpGrade/Cost/Item (2)").clickEvent.Add(new EventDelegate(() => OnClickGradeUpShowItem(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item (2)"))));
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpGrade/Cost/Item (3)").clickEvent.Add(new EventDelegate(() => OnClickGradeUpShowItem(t.GetMonoILRComponent<LTShowItem>("UpGrade/Cost/Item (3)"))));
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpGrade/UpGradeBtn").clickEvent.Add(new EventDelegate(OnUpGradeBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("UpGrade/QuicklyCollecteBtn").clickEvent.Add(new EventDelegate(OnQuicklyCollectBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("StarUp/StarHoleUpBtn").clickEvent.Add(new EventDelegate(OnStarHoleUpBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("StarUp/StarUpBtn").clickEvent.Add(new EventDelegate(OnStarUpBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Skill/CommonSkill/SkillItem").clickEvent.Add(new EventDelegate(OnCommonSkillClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Skill/ActiveSkill/SkillItem").clickEvent.Add(new EventDelegate(OnActiveSkillClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Skill/PassiveSkill/SkillItem").clickEvent.Add(new EventDelegate(OnPassiveSkillClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("AwakenView/AwakeBtn").clickEvent.Add(new EventDelegate(OnClickPartnerAwaken));
            t.GetComponent<ConsecutiveClickCoolTrigger>("AwakenView/SkillSwitchBtn/AwakeBtn (1)").clickEvent.Add(new EventDelegate(UseOriginSkin));
            t.GetComponent<ConsecutiveClickCoolTrigger>("AwakenView/SkillSwitchBtn/AwakeBtn (2)").clickEvent.Add(new EventDelegate(UseAwakeSkin));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Skin/Scroll/PlaceHolder/Grid/Item").clickEvent.Add(new EventDelegate(t.GetComponent<UICenterOnClick>("Skin/Scroll/PlaceHolder/Grid/Item").OnClick));

            t.GetComponent<TweenScale>("Title/BtnList/InfoBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("Title/BtnList/InfoBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("Title/BtnList/UpGradeBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("Title/BtnList/UpGradeBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("Title/BtnList/StarUpBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("Title/BtnList/StarUpBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("Title/BtnList/SkillBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("Title/BtnList/SkillBtn/Sprite/Sprite").gameObject)));
            t.GetComponent<TweenScale>("Title/BtnList/AwakenBtn/Sprite").onFinished.Add(new EventDelegate(() => OnFinishShow(t.FindEx("Title/BtnList/AwakenBtn/Sprite/Sprite").gameObject)));

            AddListener();
        }

        private CultivateType curType = CultivateType.Info;

        public CultivateType CurType
        {
            get
            {
                return curType;
            }
            set
            {
                curType = value;
                SetViewType();
            }
        }
        private CultivateType typeBeforeSkin = CultivateType.Info;
        [HideInInspector]
        public GameObject PotenObj;
        public GameObject LevelUpMaxObj;
        public GameObject GuardObj;
        public UILabel LevelUpMaxLevelLabel;
        public UILabel GuardLevelLabel;
        [HideInInspector]
        public bool isOpenCulView = false;

        private LTPartnerData partnerData;

        //private void Start()
        //{
        //    CurType = CultivateType.Info;
        //}

        public void InitCultivateInfo(LTPartnerData itemData)
        {
            if (itemData == null || itemData.HeroId <= 0)
            {
                //UpdatePoten(true);
                return;
            }
            partnerData = itemData;
            if (CurType == CultivateType.Awaken && !LTPartnerDataManager.Instance.IsOpenAwakenFun(itemData.InfoId))
            {
                CurType = CultivateType.Info;
                OnTabBtnClick(TabBtnList[(int)CurType]);
            }

            switch (CurType)
            {
                case CultivateType.Info:
                    InitInfo();
                    break;
                case CultivateType.UpGrade:
                    InitUpGrade();
                    break;
                case CultivateType.StarUp:
                    SetHeroClipChangeData();
                    InitStarUp();
                    break;
                case CultivateType.Skill:
                    InitSkill();
                    break;
                case CultivateType.Awaken:
                    InitAwakeData();
                    break;
                case CultivateType.Skin:
                    SkinView.Show(partnerData, isShowAwakenSkin);
                    break;
            }
            //InitTitleShown();
            UpdatePoten();
            ShowSkinBtnState();
        }

        private void AddListener()
        {
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnParnerSkillChange, OnPartnerSkillChangeFunc);
            Hotfix_LT.Messenger.AddListener<bool>(Hotfix_LT.EventName.PartnerCultivateRP, PartnerCultivateRPEvent);
            Hotfix_LT.Messenger.AddListener<bool>(Hotfix_LT.EventName.OnPartnerHeroChipChange, OnPartnerHeroChipChange);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerHoleUpError, OnPartnerHoleUpErrorEvent);
            Hotfix_LT.Messenger.AddListener<CultivateType>(Hotfix_LT.EventName.OnPartnerUIRefresh, OnPartnerUIRefresh);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerAwakenSuccess, OnRefreshAwakenView);
            Hotfix_LT.Messenger.AddListener<CultivateType>(Hotfix_LT.EventName.OnPartnerTurnToUpgrade, OnTurnToUpgradeViewInPartnerCul);
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnParnerSkillChange, OnPartnerSkillChangeFunc);
            Hotfix_LT.Messenger.RemoveListener<bool>(Hotfix_LT.EventName.PartnerCultivateRP, PartnerCultivateRPEvent);
            Hotfix_LT.Messenger.RemoveListener<bool>(Hotfix_LT.EventName.OnPartnerHeroChipChange, OnPartnerHeroChipChange);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerHoleUpError, OnPartnerHoleUpErrorEvent);
            Hotfix_LT.Messenger.RemoveListener<CultivateType>(Hotfix_LT.EventName.OnPartnerUIRefresh, OnPartnerUIRefresh);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerAwakenSuccess, OnRefreshAwakenView);
            Hotfix_LT.Messenger.RemoveListener<CultivateType>(Hotfix_LT.EventName.OnPartnerTurnToUpgrade, OnTurnToUpgradeViewInPartnerCul);
        }

        public GameObject AttributesRedPoint;
        public GameObject GradeUpRedPoint;
        public GameObject StarUpRedPoint;
        public GameObject SkillBreakRedPoint;
        public GameObject TopLevelRedPoint;
        public GameObject GuardRedPoint;
        public GameObject AwakeRedPoint;
        public GameObject SkinBtnObj;
        private GameObject QuicklyGetBtn,UpGradeBtn;

        public UILabel LevelLabel;
        public UILabel ExpLabel;
        public UISlider ExpSlider;

        public UILabel AttackLabel;
        public UILabel DefLabel;
        public UILabel LifeLabel;
        public UILabel CritLabel;
        public UILabel CritVLabel;
        public UILabel ComboLabel;
        public UILabel SpExtraLabel;
        public UILabel SpResLabel;

        public UILabel EquipAttackLabel;
        public UILabel EquipDefLabel;
        public UILabel EquipLifeLabel;
        public UILabel EquipCritLabel;
        public UILabel EquipCritVLabel;
        public UILabel EquipComboLabel;
        public UILabel EquipSpExtraLabel;
        public UILabel EquipSpResLabel;
        public UILabel StarUpTip;

        public TweenScale[] AttrAni;
        public TweenPosition LvUpLabelTweenPos;
        public TweenAlpha LvUpLabelTweenAlp;
        public SliderAni ExpSliderAni;
        public NumAni ExpNumAni;

        [HideInInspector]
        public ParticleSystemUIComponent LevelUpFx;
        public LTPartnerAttrDesc AttrDesc;
        public LTPartnerStarUpDesc StarUpDesc;
        private LTAttributesData curFinalAttrData;
        private LTAttributesData curBaseAttrData;
        private System.Action mPlayLobbyMove;

        private bool isPlayingAni;
        public bool IsPlayingAni { set { isPlayingAni = value; } get { return isPlayingAni; } }

        private void InitInfo()
        {
            Hotfix_LT.Data.UpGradeInfoTemplate curEvTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
            if (curEvTpl != null)
            {
                LevelLabel.text = string.Format("{0}/{1}", partnerData.Level.ToString(), BalanceResourceUtil.GetUserLevel());
            }

            Hotfix_LT.Data.HeroLevelInfoTemplate levelTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroLevelInfo(partnerData.Level);
            if (levelTpl != null)
            {
                int curExp = LTPartnerDataManager.Instance.GetPartnerCurLevelExp(partnerData.StatId);
                LTUIUtil.SetText(ExpLabel, string.Format("{0}/{1}", curExp, levelTpl.buddy_exp));
                if (levelTpl.buddy_exp != 0 && curExp > 0)
                {
                    ExpSlider.gameObject.CustomSetActive(true);
                    ExpSlider.value = curExp / (float)levelTpl.buddy_exp;
                }
                else
                {
                    ExpSlider.value = 0;
                }
            }

            UpdateAttrData();

            AttackLabel.text = ((int)curFinalAttrData.m_ATK).ToString();
            DefLabel.text = ((int)curFinalAttrData.m_DEF).ToString();
            LifeLabel.text = ((int)curFinalAttrData.m_MaxHP).ToString();
            int num = Mathf.FloorToInt(curFinalAttrData.m_CritP * 100);
            CritLabel.text = string.Format("{0}%", num);
            num = Mathf.FloorToInt(curFinalAttrData.m_CritV * 100);
            CritVLabel.text = string.Format("{0}%", num);
            ComboLabel.text = ((int)(curFinalAttrData.m_Speed)).ToString();
            num = Mathf.FloorToInt(curFinalAttrData.m_SpExtra * 100);
            SpExtraLabel.text = string.Format("{0}%", num);
            num = Mathf.FloorToInt(curFinalAttrData.m_SpRes * 100);
            SpResLabel.text = string.Format("{0}%", num);

            InitEquipAttr();
        }

        private void UpdateAttrData()
        {
            curFinalAttrData = AttributesManager.GetPartnerAttributesByParnterData(partnerData);
            curBaseAttrData = AttributesUtil.GetBaseAttributes(partnerData);
        }

        public void InitEquipAttr()
        {
            LTAttributesData equipData = AttributesManager.GetPartnerEquipmentAttributes(curBaseAttrData, partnerData.EquipmentTotleAttr);

            LTUIUtil.SetText(EquipAttackLabel, equipData.m_ATK <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(equipData.m_ATK).ToString()));
            LTUIUtil.SetText(EquipDefLabel, equipData.m_DEF <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(equipData.m_DEF).ToString()));
            LTUIUtil.SetText(EquipLifeLabel, equipData.m_MaxHP <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(equipData.m_MaxHP).ToString()));
            LTUIUtil.SetText(EquipComboLabel, equipData.m_Speed <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(equipData.m_Speed).ToString()));
            LTUIUtil.SetText(EquipCritLabel, equipData.m_CritP <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt((equipData.m_CritP * 100)).ToString()));
            LTUIUtil.SetText(EquipCritVLabel, equipData.m_CritV <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt((equipData.m_CritV * 100)).ToString()));
            LTUIUtil.SetText(EquipSpExtraLabel, equipData.m_SpExtra <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt((equipData.m_SpExtra * 100)).ToString()));
            LTUIUtil.SetText(EquipSpResLabel, equipData.m_SpRes <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt((equipData.m_SpRes * 100)).ToString()));
        }

        /// <summary>
        ///  播放伙伴属性变动动画
        /// </summary>
        /// <param name="oldAttr"></param>
        /// <param name="newAttr"></param>
        /// <param name="curLevel"></param>
        public void PlayAttrAni(LTAttributesData oldAttr, LTAttributesData newAttr, int curLevel)
        {
            if (newAttr == null)
            {
                return;
            }
            FusionAudio.PostEvent("UI/Partner/LevelUp");
            LvUpLabelTweenPos.ResetToBeginning();
            LvUpLabelTweenAlp.ResetToBeginning();
            LvUpLabelTweenPos.PlayForward();
            LvUpLabelTweenAlp.PlayForward();

            double oldAtk = oldAttr.m_ATK;
            double oldDef = oldAttr.m_DEF;
            double oldMaxHp = oldAttr.m_MaxHP;
            double newAtk = newAttr.m_ATK;
            double newDef = newAttr.m_DEF;
            double newMaxHp = newAttr.m_MaxHP;

            if (newAtk - oldAtk > 0)
            {
                AttackLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
                PlayAttrAni(AttrAni[0]);
            }

            if (newDef - oldDef > 0)
            {
                DefLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
                PlayAttrAni(AttrAni[1]);
            }

            if (newMaxHp - oldMaxHp > 0)
            {
                LifeLabel.color = LT.Hotfix.Utility.ColorUtility.GreenColor;
                PlayAttrAni(AttrAni[2]);
            }

            AttackLabel.text = newAtk.ToString("f0");
            DefLabel.text = newDef.ToString("f0");
            LifeLabel.text = newMaxHp.ToString("f0");

            LevelUpFx.gameObject.CustomSetActive(true);
            LevelUpFx.Play();

            PlayLobbyMove();

            InitEquipAttr();


            Hotfix_LT.Data.UpGradeInfoTemplate curEvTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
            if (curEvTpl != null)
            {
                LevelLabel.text = string.Format("{0}/{1}", curLevel.ToString(), BalanceResourceUtil.GetUserLevel());
            }
        }

        /// <summary>
        /// 伙伴属性动画
        /// </summary>
        /// <param name="ani"></param>
        private void PlayAttrAni(TweenScale ani)
        {
            ani.ResetToBeginning();
            ani.PlayForward();
            ani.SetOnFinished(PlayAttrAniOnFinished);
        }

        /// <summary>
        /// 伙伴属性动画播放完毕
        /// </summary>
        private void PlayAttrAniOnFinished()
        {
            AttackLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            DefLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            LifeLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;

            InitEquipAttr();
        }

        //是否播放lobby动画
        private bool isPlayLobbyMove = false;
        /// <summary>
        /// 播放Lobby动画
        /// </summary>
        private void PlayLobbyMove()
        {
            if (isPlayLobbyMove)
            {
                if (mPlayLobbyMove != null)
                {
                    mPlayLobbyMove();
                }
                isPlayLobbyMove = false;
            }
        }

        /// <summary>
        /// 设置lobby动画
        /// </summary>
        /// <param name="moveAction"></param>
        public void SetLobbyMoveAction(System.Action moveAction)
        {
            mPlayLobbyMove = moveAction;
        }

        /// <summary>
        ///停止播放伙伴属性变动动画
        /// </summary>
        /// <param name="isNeedRefreshInfo">是否需要更新属性信息</param>
        public void StopAni(bool isNeedRefreshInfo = false)
        {
            if (!isPlayingAni && !LvUpLabelTweenPos.enabled)
            {
                return;
            }

            LTPartnerLvUpAniManager.Instance.StopAni();

            AttackLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            DefLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            LifeLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;

            for (int i = 0; i < AttrAni.Length; i++)
            {
                AttrAni[i].transform.localScale = Vector3.one;
                AttrAni[i].enabled = false;
            }
            LvUpLabelTweenAlp.GetComponent<UILabel>().alpha = 0;
            LvUpLabelTweenPos.enabled = false;
            LvUpLabelTweenAlp.enabled = false;

            if (isNeedRefreshInfo)
            {
                InitInfo();
            }
        }

        public void HideAwakeSkillPanel()
        {
            if (Exist(CultivateBodyObj) && CultivateBodyObj.Length > 4)
                CultivateBodyObj[4].FindEx("AwakenSkillPanel").SetActive(false);
        }

        /// <summary>
        /// 点击升一级按钮
        /// </summary>
        public void OnLevelUpBtnClick()
        {
            if (isPlayingAni)
            {
                return;
            }

            int messageId = 0;
            if (LTPartnerDataManager.Instance.IsCanLevelUp(partnerData.StatId, 1, out messageId))
            {
                LTPartnerLvUpAniData aniData = new LTPartnerLvUpAniData(this, ExpSliderAni, ExpNumAni);
                aniData.oldLv = partnerData.Level;
                aniData.oldExp = LTPartnerDataManager.Instance.GetPartnerCurLevelExp(partnerData.StatId);
                aniData.oldAttr = new LTAttributesData(curFinalAttrData);
                LTPartnerDataManager.Instance.LevelUp(partnerData.HeroId, 1, delegate (Hashtable result)
                {
                    if (result != null)
                    {
                        int curBuddyExp = EB.Dot.Integer("currentBuddyExp", result, 0);
                        if (curBuddyExp != 0)
                        {
                            DataLookupsCache.Instance.CacheData("res.buddy-exp.v", curBuddyExp);
                        }
                    }
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerLevelUpSucc);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 1,true);
                    isPlayLobbyMove = true;
                    aniData.curLv = partnerData.Level;
                    aniData.curExp = LTPartnerDataManager.Instance.GetPartnerCurLevelExp(partnerData.StatId);
                    aniData.parData = partnerData;
                    LTPartnerLvUpAniManager.Instance.SetAniData(aniData);
                    UpdateAttrData();

                    UpdatePoten();
                    if (partnerData.Level >= Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMaxPlayerLevel()) GlobalMenuManager.Instance.Open("ShowGradeStarUpView", new ShowGradeStarUp(LTParGradeStarUp.MaxLevel, partnerData, curFinalAttrData));
                });
            }
            else
            {
                if (messageId == 902262)
                {
                    //GetParExp.Init();
                    GlobalMenuManager.Instance.Open("LTPartnerGetParExp", "GetExp");//获取经验
                    return;
                }
                else if (messageId == 902261)
                {
                    GlobalMenuManager.Instance.Open("LTPartnerGetParExp", "PlayerLevelUp");//提升玩家等级
                    return;
                }
                else if (messageId == -1)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_Label_35"));
                    return;
                }
                MessageTemplateManager.ShowMessage(messageId);
            }
        }

        /// <summary>
        /// 点击升五级按钮
        /// </summary>
        public void OnMulitLevelUpBtnClick()
        {
            if (isPlayingAni)
            {
                return;
            }

            int messageId = 0;
            if (LTPartnerDataManager.Instance.IsCanLevelUp(partnerData.StatId, 5, out messageId))
            {
                LTPartnerLvUpAniData aniData = new LTPartnerLvUpAniData(this, ExpSliderAni, ExpNumAni);
                aniData.oldLv = partnerData.Level;
                aniData.oldExp = LTPartnerDataManager.Instance.GetPartnerCurLevelExp(partnerData.StatId);
                aniData.oldAttr = new LTAttributesData(curFinalAttrData);
                LTPartnerDataManager.Instance.LevelUp(partnerData.HeroId, 5, delegate
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerLevelUpSucc);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 1,true);
                    isPlayLobbyMove = true;
                    aniData.curLv = partnerData.Level;
                    aniData.curExp = LTPartnerDataManager.Instance.GetPartnerCurLevelExp(partnerData.StatId);
                    aniData.parData = partnerData;
                    LTPartnerLvUpAniManager.Instance.SetAniData(aniData);
                    UpdateAttrData();

                    UpdatePoten();
                    if (partnerData.Level >= Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMaxPlayerLevel()) GlobalMenuManager.Instance.Open("ShowGradeStarUpView", new ShowGradeStarUp(LTParGradeStarUp.MaxLevel, partnerData, curFinalAttrData));
                });
            }
            else
            {
                if (messageId == 902262)
                {
                    GlobalMenuManager.Instance.Open("LTPartnerGetParExp", "GetExp");
                    return;
                }
                else if (messageId == 902261)
                {
                    GlobalMenuManager.Instance.Open("LTPartnerGetParExp", "PlayerLevelUp");//提升玩家等级
                    return;
                }
                else if (messageId == -1)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_Label_35"));
                    return;
                }

                MessageTemplateManager.ShowMessage(messageId);
            }
        }

        public void OnDetailedInfoBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTPartnerDetailedInfoUI", partnerData);
        }

        /// <summary>
        ///点击属性按钮
        /// </summary>
        /// <param name="obj"></param>
        public void OnAttrBtnClick(GameObject obj)
        {
            int index = int.Parse(obj.name);
            AttrDesc.ShowUI(index);
        }

        /// <summary>
        ///点击升星Tip按钮
        /// </summary>
        /// <param name="obj"></param>
        public void OnStarUpTipBtnClick()
        {
            StarUpDesc.ShowUI(partnerData.Star, partnerData.HeroStat.starskill5, partnerData.HeroStat.starskill6);
        }
        Hotfix_LT.Data.HeroAwakeInfoTemplate skillLevelUptpl = new Hotfix_LT.Data.HeroAwakeInfoTemplate();
        /// <summary>
        /// 点击第一个技能图标
        /// </summary>
        public void OnActiveSkillClick()
        {
            if (partnerData.HeroStat.active_skill <= 0)
            {
                return;
            }
            int ShowskillId = partnerData.HeroStat.active_skill;
            if (partnerData.IsAwaken > 0)
            {
                skillLevelUptpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
                if (ShowskillId == skillLevelUptpl.beforeSkill) ShowskillId = skillLevelUptpl.laterSkill;
            }
            UITooltipManager.Instance.DisplayTooltipForPress(ShowskillId.ToString() + "," + partnerData.ActiveSkillLevel.ToString(), "Skill", "default", Vector3.zero, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });//-374
        }


        public void OnCommonSkillClick()
        {
            if (partnerData.HeroStat.common_skill <= 0)
            {
                return;
            }
            int ShowskillId = partnerData.HeroStat.common_skill;
            if (partnerData.IsAwaken > 0)
            {
                skillLevelUptpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
                if (ShowskillId == skillLevelUptpl.beforeSkill) ShowskillId = skillLevelUptpl.laterSkill;
            }

            UITooltipManager.Instance.DisplayTooltipForPress(ShowskillId.ToString() + "," + partnerData.CommonSkillLevel.ToString(), "Skill", "default", Vector3.zero, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });//-24
        }


        public void OnPassiveSkillClick()
        {
            if (partnerData.HeroStat.passive_skill <= 0)
            {
                return;
            }
            int ShowskillId = partnerData.HeroStat.passive_skill;
            if (partnerData.IsAwaken > 0)
            {
                skillLevelUptpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
                if (ShowskillId == skillLevelUptpl.beforeSkill) ShowskillId = skillLevelUptpl.laterSkill;
            }
            //18:9的分辨率技能描述ui会超出屏幕外，在这里做处理
            int width = Screen.width;
            int height = Screen.height;
            Vector3 pos = new Vector3(0, -72400, 0);
            //1.9为容错
            if ((float)width / height >= 1.9)
            {
                pos = new Vector3(0, -65000, 0);
            }
            UITooltipManager.Instance.DisplayTooltipForPress(ShowskillId.ToString() + "," + partnerData.PassiveSkillLevel.ToString(), "Skill", "default", Vector3.zero, ePressTipAnchorType.LeftDown, false, false, false, delegate () { });
        }

        //public LTPartnerGradeStarUp GradeStarUp;

        public LTPartnerListCellController ExItem;
        public LTPartnerListCellController CurItem;
        private PartnerAwakenCusumeDataLookUp upgradeDatalookup;
        public List<LTShowItem> UpGradeCostItemList;
        public UIGrid ItemGrid;
        public UILabel GradeLimitLabel;
        public UILabel GradeUpIncomeLabel;
        private List<string> gradeItemIDList;
        private List<int> gradeItemCountList;

        /// <summary>
        ///初始化升阶信息
        /// </summary>
        private void InitUpGrade()
        {
            ExItem.SetItemDataOther(partnerData);

            int curQuality = 0;
            int curAddLevel = 0;
            if (partnerData.UpGradeId < LTPartnerConfig.MAX_GRADE_LEVEL)
            {
                LTPartnerDataManager.GetPartnerQuality(partnerData.UpGradeId + 1, out curQuality, out curAddLevel);
                curAddLevel = curAddLevel > 2 ? 2 : curAddLevel;
                CurItem.SetItemDataOther(partnerData, curQuality, curAddLevel);
            }
            else
            {
                CurItem.SetItemDataOther(partnerData);
            }
            Hotfix_LT.Data.UpGradeInfoTemplate curEvoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId, partnerData.HeroInfo.char_type);
            string colorStrT = partnerData.Level >= curEvoTpl.level_limit ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            GradeLimitLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_16666"), colorStrT, curEvoTpl.level_limit);
            Hotfix_LT.Data.UpGradeInfoTemplate evoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(partnerData.UpGradeId + 1, partnerData.HeroInfo.char_type);

            if (evoTpl != null)
            {
                var curData = GetPartnerGradeAttributes(partnerData.StatId, partnerData.UpGradeId);
                var data = GetPartnerGradeAttributes(partnerData.StatId, partnerData.UpGradeId + 1);
                GradeUpIncomeLabel.text = string.Format("[42fe79]{0}+{1}  {2}+{3}  {4}+{5}  {6}+{7}[-]",
                    EB.Localizer.GetString("ID_COMBAT_TIPS_ATTR_PHYSIC_ATTACK"), (int)(data.m_ATK - curData.m_ATK),
                    EB.Localizer.GetString("ID_COMBAT_TIPS_ATTR_PHYSIC_DEFEND"), (int)(data.m_DEF - curData.m_DEF),
                    EB.Localizer.GetString("ID_MaxHP"), (int)(data.m_MaxHP - curData.m_MaxHP),
                    EB.Localizer.GetString("ID_COMBAT_TIPS_ATTR_SPEED"), (int)(data.m_Speed - curData.m_Speed));

                gradeItemIDList = new List<string>(evoTpl.materialDic.Keys);
                gradeItemCountList = new List<int>(evoTpl.materialDic.Values);
                if (evoTpl.needGoldNum > 0)
                {
                    gradeItemIDList.Add(LTResID.GoldName);
                    gradeItemCountList.Add(evoTpl.needGoldNum);
                }
                if (evoTpl.needHcNum > 0)
                {
                    gradeItemIDList.Add(LTResID.HcName);
                    gradeItemCountList.Add(evoTpl.needHcNum);
                }
            }
            else
            {
                GradeUpIncomeLabel.text = string.Empty;
                gradeItemIDList = null;
                gradeItemCountList = null;
            }
            //设置升阶按钮显示
            SetUpGradeBtnCondition(partnerData.UpGradeId);
            //设置材料显示
            int index = 0;
            if (evoTpl != null)
            {
                for (; index < gradeItemIDList.Count; index++)
                {
                    SetUpgradeMaterialItem(index, gradeItemIDList[index]);
                }
                upgradeDatalookup.InitDataList(gradeItemIDList, gradeItemCountList, evoTpl.needGoldNum, evoTpl.needHcNum);

            }
            else
            {
                upgradeDatalookup.ClearData();
            }
            for (; index < UpGradeCostItemList.Count; index++)
            {
                UpGradeCostItemList[index].mDMono.gameObject.CustomSetActive(false);
            }
            ItemGrid.Reposition();
        }

        private void SetUpgradeMaterialItem(int index, string id)
        {
            string type;
            if (int.TryParse(id,  out int mid))
            {
                type = LTShowItemType.TYPE_GAMINVENTORY;
            }
            else
            {
                type = LTShowItemType.TYPE_RES;
            }
            if (index >= UpGradeCostItemList.Count) return;
            UpGradeCostItemList[index].mDMono.gameObject.CustomSetActive(true);
            UpGradeCostItemList[index].LTItemData = new LTShowItemData(id, 0, type, false);
        }


        private void SetUpGradeBtnCondition(int UpgradeId)
        {
            Vector3 pos;
            if (UpgradeId >= 13)
            {
                QuicklyGetBtn.CustomSetActive(false);
                pos = UpGradeBtn.transform.localPosition;
                UpGradeBtn.transform.localPosition = new Vector3(165, pos.y, 0);
            }
            else
            {
                pos = UpGradeBtn.transform.localPosition;
                UpGradeBtn.transform.localPosition = new Vector3(415, pos.y, 0);
                QuicklyGetBtn.CustomSetActive(true);              
            }
        }

        /// <summary>
        //点击伙伴升阶材料图标
        /// </summary>
        /// <param name="item"></param>
        public void OnClickGradeUpShowItem(LTShowItem item)
        {
            int index = UpGradeCostItemList.IndexOf(item);
            if (index >= gradeItemIDList.Count) return;
            LTPartnerDataManager.Instance.itemID = gradeItemIDList[index];
            LTPartnerDataManager.Instance.itemNeedCount = gradeItemCountList[index];
        }

        /// <summary>
        //点击伙伴升阶按钮
        /// </summary>
        public void OnUpGradeBtnClick()
        {
            int messageId = 0;
            int levelLimit = 0;
            UpdateAttrData();
            if (LTPartnerDataManager.Instance.IsCanUpGrade(partnerData.StatId, out messageId, out levelLimit))
            {
                LTPartnerDataManager.Instance.UpGrade(partnerData.HeroId, delegate
                 {
                     InitUpGrade();
                     GlobalMenuManager.Instance.Open("ShowGradeStarUpView", new ShowGradeStarUp(LTParGradeStarUp.GradeUp, partnerData, curFinalAttrData));
                     Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerUpGradeSucc);
                     Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 1,true);
                 });
            }
            else
            {
                if (messageId == -10000)
                {
                    LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_19467"));
                }
                else if (levelLimit > 0)
                {
                    Hashtable data = Johny.HashtablePool.Claim();
                    data.Add("0", levelLimit);
                    MessageTemplateManager.ShowMessage(messageId, data, null);
                }
                else
                {
                    MessageTemplateManager.ShowMessage(messageId);
                }
            }
        }

        public void OnQuicklyCollectBtnClick()
        {
            //Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10077);
            //if (ft != null && !ft.IsConditionOK())
            //{
            //    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
            //    return;
            //}
            if (partnerData.UpGradeId >= LTPartnerConfig.MAX_GRADE_LEVEL)
            {
                LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_19467"));
                return;
            }
            GlobalMenuManager.Instance.Open("QuicklyGetUpgradeMaterial", partnerData);
        }



        public UILabel ShardOwnNumLabel;
        public GameObject[] StarUpBG;
        public List<Transform> StarUpAttrTranList;
        public UISprite TipBtnSprite, TipBtnSpriteFrame;
        //public List<Transform> StarUpAttrTranShuiList;
        //public List<Transform> StarUpAttrTranHuoList;
        public UIButton StarUpBtn;
        //public UISprite StarUpSprite;
        public UIButton StarHoleUpBtn;
        //public UISprite StarHoleUpSprite;
        public LTPartnerProficiencyUpController ProficiencyUpView;

        public LTPartnerDataLookup HeroClipChangeDataLookup;
        public ParticleSystemUIComponent StarHoleNextFx;
        public ParticleSystemUIComponent StarHoleCurPlayFx;
        public LTPartnerChipTransController ChipTransController;
        public DynamicUISprite ChipTypeSprite;
        private GameObject chipsshowobj;
        public UILabel ChipNum;

        //设置升星显示专用
        private UISprite attrSprite;
        private UILabel attrLabel;
        private GameObject fx, fx_Huo, fx_Shui, fx_Feng;
        /// <summary>
        ///初始化伙伴升星信息
        /// </summary>
        public void InitStarUp()
        {
            bool isHaveNext = false;
            Hotfix_LT.Data.StarInfoTemplate nextLevelStarInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarInfoByLevelHole(partnerData.Star, partnerData.StarHole + 1);
            if (nextLevelStarInfo != null)
            {
                isHaveNext = partnerData.ShardNum >= nextLevelStarInfo.cost_shard;
                string colorStr = partnerData.ShardNum >= nextLevelStarInfo.cost_shard ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                LTUIUtil.SetText(ShardOwnNumLabel, string.Format("[{0}]{1}[-]/{2}", colorStr, partnerData.ShardNum.ToString(), nextLevelStarInfo.cost_shard.ToString()));
            }
            else
            {
                LTUIUtil.SetText(ShardOwnNumLabel, string.Format("{0}", partnerData.ShardNum.ToString()));
            }
            StarUpBG[(int)Hotfix_LT.Data.eRoleAttr.Feng - 1].CustomSetActive(partnerData.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Feng);
            StarUpBG[(int)Hotfix_LT.Data.eRoleAttr.Shui - 1].CustomSetActive(partnerData.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Shui);
            StarUpBG[(int)Hotfix_LT.Data.eRoleAttr.Huo - 1].CustomSetActive(partnerData.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Huo);
            TipBtnSprite.spriteName = GetStarSkillIconName(partnerData.HeroInfo.role_profile_icon); //被動技能圖標
            if (partnerData.Star == 1)
            {
                TipBtnSpriteFrame.gameObject.CustomSetActive(false);
            }
            else
            {
                TipBtnSpriteFrame.color = LT.Hotfix.Utility.ColorUtility.QualityToColor(partnerData.Star);
                TipBtnSpriteFrame.gameObject.CustomSetActive(true);
            }

            if (partnerData.Star < LTPartnerConfig.MAX_STAR)
            {
                ProficiencyUpView.Hide();
                List<Hotfix_LT.Data.StarInfoTemplate> curLevelStarInfolist = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarInfoListByStarLevel(partnerData.Star);
                StarUpAttrTranList[0].parent.gameObject.CustomSetActive(true);
                StarUpTip.text = EB.Localizer.GetString("ID_PARTNER_STARUP_TIP7");
                StarUpTip.gameObject.CustomSetActive(true);
                for (int i = 0; i < StarUpAttrTranList.Count; i++)
                {
                    if (i < curLevelStarInfolist.Count)
                    {
                        StarUpAttrTranList[i].gameObject.CustomSetActive(true);
                        attrSprite = StarUpAttrTranList[i].Find("Sprite").GetComponent<UISprite>();
                        attrLabel = StarUpAttrTranList[i].Find("Label").GetComponent<UILabel>();
                        fx = StarUpAttrTranList[i].Find("Fx").gameObject;
                        fx_Feng = StarUpAttrTranList[i].Find("Fx/Fx_Feng").gameObject;
                        fx_Shui = StarUpAttrTranList[i].Find("Fx/Fx_Shui").gameObject;
                        fx_Huo = StarUpAttrTranList[i].Find("Fx/Fx_Huo").gameObject;
                        if (i < partnerData.StarHole)
                        {
                            attrSprite.spriteName = LTPartnerConfig.PARTNER_STAR_ITEM_SPRITE_NAME_DIC[partnerData.HeroInfo.char_type];
                            attrSprite.gameObject.CustomSetActive(true);
                            fx.CustomSetActive(true);
                            fx_Feng.CustomSetActive(partnerData.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Feng);
                            fx_Shui.CustomSetActive(partnerData.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Shui);
                            fx_Huo.CustomSetActive(partnerData.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Huo);
                            attrLabel.gameObject.CustomSetActive(true);
                            string strColor = string.Format("[{0}]", LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal);
                            float attrValue = GetStarUpAttrValue(curLevelStarInfolist[i].level, curLevelStarInfolist[i].attr);
                            string str = string.Format("{0}{1} +{2}", EB.Localizer.GetString(LTPartnerConfig.PARTNER_STAR_UP_ATTR_NAME_DIC[curLevelStarInfolist[i].attr]), strColor, (curLevelStarInfolist[i].param * attrValue).ToString("f0"));
                            LTUIUtil.SetText(attrLabel, str);

                            if (i == StarUpAttrTranList.Count - 1)
                            {
                                StarHoleNextFx.gameObject.CustomSetActive(false);
                            }
                        }
                        else if (i == partnerData.StarHole)
                        {
                            attrSprite.gameObject.CustomSetActive(false);
                            fx.CustomSetActive(false);
                            attrLabel.gameObject.CustomSetActive(true);
                            string strColor = string.Format("[{0}]", LT.Hotfix.Utility.ColorUtility.GrayColorHexadecimal);
                            float attrValue = GetStarUpAttrValue(curLevelStarInfolist[i].level, curLevelStarInfolist[i].attr);
                            string str = string.Format("{0}{1} +{2}", strColor, EB.Localizer.GetString(LTPartnerConfig.PARTNER_STAR_UP_ATTR_NAME_DIC[curLevelStarInfolist[i].attr]), (curLevelStarInfolist[i].param * attrValue).ToString("f0"));
                            LTUIUtil.SetText(attrLabel, str);

                            if (isHaveNext)
                            {
                                StarHoleNextFx.gameObject.CustomSetActive(true);
                                StarHoleNextFx.transform.SetParent(StarUpAttrTranList[i].transform);
                                StarHoleNextFx.transform.localPosition = Vector3.zero;
                                StarHoleNextFx.transform.localEulerAngles = Vector3.zero;
                                StarHoleNextFx.transform.localScale = Vector3.one;
                                StarHoleNextFx.Play();
                            }
                            else
                            {
                                StarHoleNextFx.gameObject.CustomSetActive(false);
                            }
                        }
                        else
                        {
                            attrSprite.gameObject.CustomSetActive(false);
                            fx.CustomSetActive(false);
                            attrLabel.gameObject.CustomSetActive(false);
                        }
                    }
                    else
                    {
                        StarUpAttrTranList[i].gameObject.CustomSetActive(false);
                    }
                }

                StarHoleUpBtn.gameObject.CustomSetActive(partnerData.StarHole < 10);
                //StarHoleUpBtn.isEnabled = partnerData.Star < LTPartnerConfig.MAX_STAR;
                //StarHoleUpSprite.color = partnerData.Star < LTPartnerConfig.MAX_STAR ? new Color(1, 1, 1) : new Color(1, 0, 1);
                StarUpBtn.gameObject.CustomSetActive(partnerData.StarHole >= 10);
            }
            else
            {
                StarUpAttrTranList[0].parent.gameObject.CustomSetActive(false);
                StarHoleUpBtn.gameObject.CustomSetActive(false);
                StarUpBtn.gameObject.CustomSetActive(false);
                ProficiencyUpView.Show(partnerData);
                StarUpTip.gameObject.CustomSetActive(false);
            }

            //chipsshowobj.CustomSetActive(true);
            ChipTransController.InitWithPartnerData(partnerData);
            ChipTransController.mDMono.gameObject.CustomSetActive(false);
            UpdataChipCount();
        }

        private string GetStarSkillIconName(string location)
        {
            if (location.Equals("Huoban_Icon_Qungong"))
            {
                return "Partner_Skill_Quntigongji";
            }
            else if (location.Equals("Huoban_Icon_Kongzhi"))
            {
                return "Partner_Skill_Kongzhi";
            }
            else if (location.Equals("Huoban_Icon_Dangong"))
            {
                return "Partner_Skill_Dantigongji";
            }
            else if (location.Equals("Huoban_Icon_Fuzhu"))
            {
                return "Partner_Skill_Fuzhu";
            }
            else
            {
                return "Partner_Skill_Zhiliao";
            }


        }
        public void UpdataChipCount()
        {
            //设置转换石显示
            if (partnerData == null || partnerData.HeroInfo == null) return;
            int chipNum;
            int chipId = 1422;
            if (partnerData.HeroInfo.role_grade == (int)PartnerGrade.UR)//UR伙伴不需要刷新
            {
                chipId = 1427;
                ChipTypeSprite.spriteName = "Goods_Prop_1427";
            }
            else
            {
                switch (partnerData.HeroInfo.char_type)
                {
                    case Hotfix_LT.Data.eRoleAttr.Feng:
                        chipId = 1422;
                        ChipTypeSprite.spriteName = "Goods_Prop_1422";
                        break;
                    case Hotfix_LT.Data.eRoleAttr.Huo:
                        chipId = 1421;
                        ChipTypeSprite.spriteName = "Goods_Prop_1421";
                        break;
                    case Hotfix_LT.Data.eRoleAttr.Shui:
                        chipId = 1423;
                        ChipTypeSprite.spriteName = "Goods_Prop_1423";
                        break;
                }
            }
            chipNum = GameItemUtil.GetInventoryItemNum(chipId);
            LTUIUtil.SetText(ChipNum, chipNum.ToString());
        }

        /// <summary>
        ///  伙伴碎片变动监控事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnPartnerHeroChipChange(bool evt)
        {
            if (evt)
            {
                if (!StarHoleNextFx.gameObject.activeInHierarchy && partnerData.StarHole < 10)
                {
                    StarHoleNextFx.gameObject.CustomSetActive(true);
                    StarHoleNextFx.transform.SetParent(StarUpAttrTranList[partnerData.StarHole].transform);
                    StarHoleNextFx.transform.localPosition = Vector3.zero;
                    StarHoleNextFx.transform.localEulerAngles = Vector3.zero;
                    StarHoleNextFx.transform.localScale = Vector3.one;
                    StarHoleNextFx.Play();
                }
            }
            else
            {
                StarHoleNextFx.gameObject.CustomSetActive(false);
            }
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnRefreshPartnerCellRP, true, true);
            RefreshStarRP();
        }

        private float GetStarUpAttrValue(int level, string attrName)
        {
            float value = 0;
            Hotfix_LT.Data.LevelUpInfoTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetLevelUpInfoByIDAndLevel(partnerData.StatId, level);
            switch (attrName)
            {
                case "MaxHP": value = temp.maxHP; break;
                case "ATK": value = temp.ATK; break;
                case "DEF": value = temp.DEF; break;
            }
            return value;
        }

        private void SetHeroClipChangeData()
        {
            Hotfix_LT.Data.StarInfoTemplate nextLevelStarInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarInfoByLevelHole(partnerData.Star, partnerData.StarHole + 1);
            int maxClip = 0;
            if (nextLevelStarInfo != null)
            {
                maxClip = nextLevelStarInfo.cost_shard;
            }
            HeroClipChangeDataLookup.SetData(partnerData.StatId.ToString(), maxClip);
        }

        private void SetHeroMaxClipData()
        {
            Hotfix_LT.Data.StarInfoTemplate nextLevelStarInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarInfoByLevelHole(partnerData.Star, partnerData.StarHole + 1);
            int maxClip = 0;
            if (nextLevelStarInfo != null)
            {
                maxClip = nextLevelStarInfo.cost_shard;
            }
            HeroClipChangeDataLookup.SetMaxClip(maxClip);
        }

        private void PlayStarHoleAni()
        {
            int index = partnerData.StarHole - 1;
            if(index < 0 || index >= StarUpAttrTranList.Count)
            {
                return;
            }
            Transform tf = StarUpAttrTranList[partnerData.StarHole - 1];
            if (StarHoleCurPlayFx != null)
            {
                StarHoleCurPlayFx.gameObject.CustomSetActive(true);
                StarHoleCurPlayFx.transform.SetParent(tf);
                StarHoleCurPlayFx.transform.localPosition = Vector3.zero;
                StarHoleCurPlayFx.transform.localEulerAngles = Vector3.zero;
                StarHoleCurPlayFx.transform.localScale = Vector3.one;
                StarHoleCurPlayFx.Play();
            }

            FusionAudio.PostEvent("UI/Partner/Practice");
            UITweener tween = tf.GetChild(0).GetComponent<TweenScale>();
            if (tween != null)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }

        public void OnStarUpBtnClick()
        {
            UpdateAttrData();
            LTPartnerDataManager.Instance.StarUp(partnerData.InfoId, delegate
            {
                SetHeroMaxClipData();

                InitStarUp();
                GlobalMenuManager.Instance.Open("ShowGradeStarUpView", new ShowGradeStarUp(LTParGradeStarUp.StarUp, partnerData, curFinalAttrData));
                RefreshStarRP();
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 2,true);
            });
        }

        bool IsStarHoleUpRequesting;
        public void OnStarHoleUpBtnClick()
        {
            if (IsStarHoleUpRequesting)
            {
                return;
            }
            int messageId = 0;
            if (LTPartnerDataManager.Instance.IsCanStarHoleUp(partnerData.StatId, out messageId))
            {
                IsStarHoleUpRequesting = true;
                LTPartnerDataManager.Instance.StarUp(partnerData.InfoId, delegate
                {
                    IsStarHoleUpRequesting = false;
                    if (partnerData.StarHole >= 10)
                    {
                        HeroClipChangeDataLookup.SetMaxClip(0);
                    }
                    InitStarUp();
                    PlayStarHoleAni();
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 1,true);
                });
            }
            else
            {
                ChipTransController.mDMono.gameObject.CustomSetActive(true);
            }
        }

        public void OnPartnerHoleUpErrorEvent()
        {
            IsStarHoleUpRequesting = false;
            ChipTransController.RefreshUIInfo();
            ChipTransController.mDMono.gameObject.CustomSetActive(true);
        }

        public void OnShardBtnClick()
        {
            UITooltipManager.Instance.DisplayTooltipSrc(partnerData.StatId.ToString(), "Generic", "default");
        }

        //1.技能图片
        public UISprite commonsprite;
        public DynamicUISprite CommonSkillBreakSprite;
        public UILabel CommonSkillBreakNameLabel;
        public UILabel CommonSkillBreakLevelLabel;
        //2。技能等级
        public UILabel CommonSkillLevel;
        public UILabel CommonSkillType;
        public UISprite CommonSkillTypeBG;

        public UISprite passivesprite;
        public DynamicUISprite PassiveSkillBreakSprite;
        public UILabel PassiveSkillBreakNameLabel;
        public UILabel PassiveSkillBreakLevelLabel;
        public UILabel PassiveSkillLevel;
        public UILabel PassiveSkillType;
        public UISprite PassiveSkillTypeBG;

        public UISprite activesprite;
        public DynamicUISprite ActiveSkillBreakSprite;
        public UILabel ActiveSkillBreakNameLabel;
        public UILabel ActiveSkillBreakLevelLabel;
        public UILabel ActiveSkillLevel;
        public UILabel ActiveSkillType;
        public UISprite ActiveSkillTypeBG;


        private void InitSkill()
        {
            if (partnerData.HeroId <= 0 && partnerData.HeroStat == null) return;
            SkillTemplate commonSkillTem = SkillTemplateManager.Instance.GetTemplate(partnerData.HeroStat.common_skill);
            SkillTemplate passiveSkillTem = SkillTemplateManager.Instance.GetTemplate(partnerData.HeroStat.passive_skill);
            SkillTemplate activeSkillTem = SkillTemplateManager.Instance.GetTemplate(partnerData.HeroStat.active_skill);
            SkillSetTool.SkillFrameStateSet(commonsprite, false);
            SkillSetTool.SkillFrameStateSet(passivesprite, false);
            SkillSetTool.SkillFrameStateSet(activesprite, false);
            if (partnerData.IsAwaken > 0)
            {
                HeroAwakeInfoTemplate tpl = CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
                if (tpl != null)
                {
                    if (tpl.beforeSkill == partnerData.HeroStat.common_skill)
                    {
                        commonSkillTem = SkillTemplateManager.Instance.GetTemplate(tpl.laterSkill);

                        if (LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC.ContainsKey(1))
                        {
                            commonsprite.spriteName = LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC[1];
                        }

                        SkillSetTool.SkillFrameStateSet(commonsprite, true);

                    }
                    if (tpl.beforeSkill == partnerData.HeroStat.passive_skill)
                    {
                        passiveSkillTem = SkillTemplateManager.Instance.GetTemplate(tpl.laterSkill);
                        SkillSetTool.SkillFrameStateSet(passivesprite, true);

                        if (LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC.ContainsKey(1))
                        {
                            passivesprite.spriteName = LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC[1];
                        }
                    }
                    if (tpl.beforeSkill == partnerData.HeroStat.active_skill)
                    {
                        activeSkillTem = SkillTemplateManager.Instance.GetTemplate(tpl.laterSkill);
                        SkillSetTool.SkillFrameStateSet(activesprite, true);

                        if (LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC.ContainsKey(1))
                        {
                            activesprite.spriteName = LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC[1];
                        }
                    }
                }

            }
            int skillLevelLimit = LTPartnerConfig.MAX_SKILL_LEVEL;

            if (commonSkillTem != null)
            {
                CommonSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                CommonSkillBreakSprite.spriteName = commonSkillTem.Icon;
                CommonSkillBreakNameLabel.text = commonSkillTem.Name;
                SetSkillType(commonSkillTem.Type, CommonSkillType, CommonSkillTypeBG);
                CommonSkillBreakLevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_34459"), partnerData.CommonSkillLevel, skillLevelLimit);
                CommonSkillLevel.text = partnerData.CommonSkillLevel.ToString();
            }
            else
            {
                CommonSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }


            if (passiveSkillTem != null)
            {
                PassiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                PassiveSkillBreakSprite.spriteName = passiveSkillTem.Icon;
                PassiveSkillBreakNameLabel.text = passiveSkillTem.Name;
                SetSkillType(passiveSkillTem.Type, PassiveSkillType, PassiveSkillTypeBG);
                PassiveSkillBreakLevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_34459"), partnerData.PassiveSkillLevel, skillLevelLimit);
                PassiveSkillLevel.text = partnerData.PassiveSkillLevel.ToString();
            }
            else
            {

                PassiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }


            if (activeSkillTem != null)
            {
                ActiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                ActiveSkillBreakSprite.spriteName = activeSkillTem.Icon;
                ActiveSkillBreakNameLabel.text = activeSkillTem.Name;
                SetSkillType(activeSkillTem.Type, ActiveSkillType, ActiveSkillTypeBG);
                ActiveSkillBreakLevelLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_34459"), partnerData.ActiveSkillLevel, skillLevelLimit);
                ActiveSkillLevel.text = partnerData.ActiveSkillLevel.ToString();
            }
            else
            {
                ActiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }
        }

        private void SetSkillType(Hotfix_LT.Data.eSkillType skillType, UILabel lab, UISprite sp)
        {
            if (skillType == Hotfix_LT.Data.eSkillType.ACTIVE)
            {
                lab.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4070");
                sp.color = LT.Hotfix.Utility.ColorUtility.RedColor;
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.NORMAL)
            {
                lab.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4440");
                sp.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.PASSIVE)
            {
                lab.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4751");
                sp.color = LT.Hotfix.Utility.ColorUtility.BlueColor;
            }
        }

        public void OnCommonSkillBreakBtnClick()
        {
            if (partnerData != null)
            {
                if (partnerData.HeroId <= 0)
                {
                    MessageTemplateManager.ShowMessage(902221);
                    return;
                }
                if (partnerData.HeroStat.common_skill <= 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_37369"));
                    return;
                }
                LTPartnerSkillBreskData data = new LTPartnerSkillBreskData();
                data.skillType = SkillType.Common;
                data.partnerData = partnerData;
                GlobalMenuManager.Instance.Open("LTPartnerSkillBreakView", data);
            }
        }

        public void OnPassiveSkillBreakBtnClick()
        {
            if (partnerData != null)
            {
                if (partnerData.HeroId <= 0)
                {
                    MessageTemplateManager.ShowMessage(902221);
                    return;
                }
                if (partnerData.HeroStat.passive_skill <= 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_37369"));
                    return;
                }
                LTPartnerSkillBreskData data = new LTPartnerSkillBreskData();
                data.skillType = SkillType.Passive;
                data.partnerData = partnerData;
                GlobalMenuManager.Instance.Open("LTPartnerSkillBreakView", data);
            }
        }

        public void OnActiveSkillBreakBtnClick()
        {
            if (partnerData != null)
            {
                if (partnerData.HeroId <= 0)
                {
                    MessageTemplateManager.ShowMessage(902221);
                    return;
                }
                if (partnerData.HeroStat.active_skill <= 0)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerCultivateController_37369"));
                    return;
                }

                LTPartnerSkillBreskData data = new LTPartnerSkillBreskData();
                data.skillType = SkillType.Active;
                data.partnerData = partnerData;
                GlobalMenuManager.Instance.Open("LTPartnerSkillBreakView", data);
            }
        }

        public GameObject[] CultivateBodyObj;
        public TitleListController TitleController;

        private void SetViewType()
        {
            CultivateBodyObj[(int)CultivateType.Info].CustomSetActive(CurType == CultivateType.Info);

            CultivateBodyObj[(int)CultivateType.UpGrade].CustomSetActive(CurType == CultivateType.UpGrade);

            CultivateBodyObj[(int)CultivateType.StarUp].CustomSetActive(CurType == CultivateType.StarUp);

            CultivateBodyObj[(int)CultivateType.Skill].CustomSetActive(CurType == CultivateType.Skill);

            CultivateBodyObj[(int)CultivateType.Awaken].CustomSetActive(CurType == CultivateType.Awaken);

            TitleGrid.transform.parent.gameObject.CustomSetActive(CurType != CultivateType.Skin);
            if(curType == CultivateType.UpGrade)
            {
                GlobalMenuManager.Instance.RemoveCache("LTPartnerHud");
                GlobalMenuManager.Instance.PushCache("LTPartnerHud", "Develop_Upgrade_1");
            }
            else
            {
                GlobalMenuManager.Instance.RemoveCache("LTPartnerHud");
                GlobalMenuManager.Instance.PushCache("LTPartnerHud");
            }
            if (CurType != CultivateType.Skin)
            {
                SkinView.Hide();
            }

            if (CurType != CultivateType.StarUp) ProficiencyUpView.Hide();

            InitCultivateInfo(partnerData);
        }

        public List<UIButton> TabBtnList;
        private FuncTemplate awakenFuncTemplate;
        private FuncTemplate _partnerStarFT;
        private FuncTemplate _partnerAdvanceFT;

        public void OnTabBtnClick(UIButton btn)
        {
            int index = TabBtnList.IndexOf(btn);

            if ((CultivateType)index != CultivateType.Info)
            {
                StopAni();
            }

            //伙伴进阶
            if (index == 1)
            {
                if (_partnerAdvanceFT == null)
                {
                    _partnerAdvanceFT = FuncTemplateManager.Instance.GetFunc(10096);
                }

                if (_partnerAdvanceFT != null && !_partnerAdvanceFT.IsConditionOK())
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, _partnerAdvanceFT.GetConditionStr());
                    TitleController.OnTitleBtnClick(TitleController.SelectObjList[(int)CurType]);
                    return;
                }
            }

            //伙伴升星
            if (index == 2)
            {
                if (_partnerStarFT == null)
                {
                    _partnerStarFT = FuncTemplateManager.Instance.GetFunc(10095);
                }

                if (_partnerStarFT != null && !_partnerStarFT.IsConditionOK())
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, _partnerStarFT.GetConditionStr());
                    TitleController.OnTitleBtnClick(TitleController.SelectObjList[(int)CurType]);
                    return;
                }
            }

            //觉醒需要做未开放英雄屏蔽
            if (index == 4)
            {
                if (awakenFuncTemplate == null)
                {
                    awakenFuncTemplate = FuncTemplateManager.Instance.GetFunc(10087);
                }

                if (awakenFuncTemplate != null && !awakenFuncTemplate.IsConditionOK())
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, awakenFuncTemplate.GetConditionStr());
                    TitleController.OnTitleBtnClick(TitleController.SelectObjList[(int)CurType]);
                    return;
                }

                if (!LTPartnerDataManager.Instance.IsOpenAwakenFun(partnerData.InfoId))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_3"));
                    TitleController.OnTitleBtnClick(TitleController.SelectObjList[(int)CurType]);
                    return;
                }
            }

            CurType = (CultivateType)index;
            //CurType = (CultivateType)TabBtnList.IndexOf(btn);
            TitleController.OnTitleBtnClick(TitleController.SelectObjList[index]);
        }

        //界面内跳转进阶
        private void OnTurnToUpgradeViewInPartnerCul(CultivateType evt)
        {
            if (evt == CultivateType.UpGrade && CurType != CultivateType.UpGrade)
            {
                CurType = CultivateType.UpGrade;
                OnTabBtnClick(TabBtnList[(int)CurType]);
            }
        }
        private void OnPartnerSkillChangeFunc()
        {
            InitSkill();
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 2,true);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnRefreshPartnerCellRP, true, false);
            RefreshSkillRP();
        }

        private void PartnerCultivateRPEvent(bool showRedpoint)
        {
            if (partnerData == null)
            {
                EB.Debug.LogError("[{0}] PartnerCultivateRPEvent 伙伴数据为空！！！！", Time.frameCount);
                return;
            }
            //
            if (showRedpoint)
            {
                RefreshRedPoint();
            }
            else
            {
                AttributesRedPoint.CustomSetActive(false);
                GradeUpRedPoint.CustomSetActive(false);
                StarUpRedPoint.CustomSetActive(false);
                SkillBreakRedPoint.CustomSetActive(false);
                TopLevelRedPoint.CustomSetActive(false);
                GuardRedPoint.CustomSetActive(false);
            }
        }

        public void OnPartnerUIRefresh(CultivateType evt)
        {
            if (evt == CultivateType.UpGrade)
            {
                if (CultivateBodyObj[(int)CultivateType.UpGrade].activeInHierarchy)
                {
                    InitUpGrade();
                }
            }
            if (evt == CultivateType.Info)
            {
                RefreshTopLevelRP();
                UpdatePoten();
                if (CultivateBodyObj[(int)CultivateType.Info].activeInHierarchy)
                {
                    InitInfo();
                }
            }
        }

        public void RefreshByActive()
        {
            if (curType == CultivateType.UpGrade)
            {
                ItemGrid.Reposition();
            }
            if (curType == CultivateType.StarUp)
            {
                SetHeroClipChangeData();
            }
        }

        public void OnFinishShow(GameObject obj)
        {
            UITweener[] ts = obj.GetComponents<UITweener>();

            if (ts == null)
            {
                return;
            }

            for (var i = 0; i < ts.Length; i++)
            {
                var t = ts[i];
                t.ResetToBeginning();
                t.PlayForward();
            }
        }

        private void UpdatePoten(bool hide = false)
        {
            //用户大于60级显示巅峰币
            if (BalanceResourceUtil.GetUserLevel() >= LTPartnerDataManager.Instance.GetPeakOpenLevel())//!hide && partnerData.Level >= Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMaxPlayerLevel())
            {
                PotenObj.CustomSetActive(true);
                PotenObj.transform.parent.GetComponent<UIGrid>().Reposition();
            }
            else
            {
                PotenObj.CustomSetActive(false);
            }

            //伙伴大于60级显示巅峰
            if (partnerData != null && partnerData.Level >= LTPartnerDataManager.Instance.GetPeakOpenLevel())
            {
                LevelUpMaxLevelLabel.text = string.Format("Lv.{0}", partnerData.GetProficiencyLevelByType(Hotfix_LT.Data.ProficiencyType.AllRound));
            }
            else
            {
                LevelUpMaxLevelLabel.text = string.Format(EB.Localizer.GetString("ID_PARTNER_GUARD_TIP_03"), LTPartnerDataManager.Instance.GetPeakOpenLevel());
            }

            //伙伴大于40级显示护卫
            if (partnerData != null && partnerData.Level >= LTPartnerDataManager.Instance.GetGuardOpenLevel())
            {
                GuardLevelLabel.color = Color.clear;
            }
            else
            {
                GuardLevelLabel.color = Color.white;
                GuardLevelLabel.text = string.Format(EB.Localizer.GetString("ID_PARTNER_GUARD_TIP_03"), LTPartnerDataManager.Instance.GetGuardOpenLevel());
            }
        }

        public void OnParterMaxLevelFuncBtnClick()
        {
            if (partnerData.Level >= LTPartnerDataManager.Instance.GetPeakOpenLevel())
            {
                GlobalMenuManager.Instance.Open("LTPartnerTopLevelUI", partnerData);
            }
            else
            {
                //未解锁
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,
                    string.Format(EB.Localizer.GetString("ID_COMMON_LOCK_TIP_01"), LTPartnerDataManager.Instance.GetPeakOpenLevel()), null);
            }


        }

        public void OnGuardFuncBtnClick()
        {
            if (partnerData.Level >= LTPartnerDataManager.Instance.GetGuardOpenLevel())
            {
                GlobalMenuManager.Instance.Open("LTPartnerGuardUI", partnerData);
            }
            else
            {
                //未解锁
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,
                    string.Format(EB.Localizer.GetString("ID_COMMON_LOCK_TIP_01"), LTPartnerDataManager.Instance.GetGuardOpenLevel()), null);
            }

        }


        #region(伙伴觉醒)
        public GameObject CulTitle, CulBG, AwakenSkillTipObj, AwakenConsumeObj, AwakenSkinTip, headHideObj, HeadShowObj, SkinSelectObj, OriginSkinHandle, AwakenSkinHandle, ConditionNameLabel, ConsumeObj;
        public LTPartnerListCellController OldCell, PreviewCell;
        public UILabel AwakenCondition, AwakenRewardLabel, AwakenBtnLabel, HasAwkenLabel;
        public UILabel[] RewardLabel;
        public LTPartnerAwakenSkillCompare SkillCompare;
        public AwakenConsumeController[] ConsumeArray;
        public UIGrid AwakenConsumeGrid;
        public Transform AwakenBtn;
        public PartnerAwakenCusumeDataLookUp MaterialdatalookUp;

        //private bool isConsumeEnough = true;
        private Vector3 awakenShowObjVec = new Vector3(-230, 0, 0);
        private Vector3 normalShowObjVec = Vector3.zero;
        private Hotfix_LT.Data.HeroAwakeInfoTemplate curAwakeTemplate = new Hotfix_LT.Data.HeroAwakeInfoTemplate();

        public UIGrid TitleGrid;
        public List<GameObject> TitleBtnObj;
        private string consumeIcon = "Ty_Icon_Aoyidian";

        public void InitAwakeData()
        {
            if (!LTPartnerDataManager.Instance.IsOpenAwakenFun(partnerData.InfoId))
            {
                //ClosePartnerAwakeView();
                return;
            }
            curAwakeTemplate = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
            SetPartnerAwakeHeadIcon(partnerData.IsAwaken);
            SetPartnerAwakeCondition(partnerData.IsAwaken <= 0);
            SetPartnerAwakeReward();
            InitPartnerAwakenConsume(partnerData.IsAwaken <= 0);
        }

        private void SetPartnerAwakeHeadIcon(int awakenLevel)
        {
            if (awakenLevel == 0)
            {
                headHideObj.CustomSetActive(true);
                HeadShowObj.transform.localPosition = normalShowObjVec;
                HeadShowObj.transform.localScale = Vector3.one;
                //OldCell.SetItemDataOther(partnerData);
                OldCell.SetItemAwakenData(partnerData,Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId, 0).icon, partnerData.Star, 0);
            }
            else
            {
                HeadShowObj.transform.localPosition = awakenShowObjVec;
                HeadShowObj.transform.localScale = Vector3.one;
                headHideObj.CustomSetActive(false);
            }
            //PreviewCell.Fill(partnerData);
            int awakenSkinIndex = 0;
            if (!string.IsNullOrEmpty(curAwakeTemplate.awakeSkin)) awakenSkinIndex = 1;
            PreviewCell.SetItemAwakenData(partnerData,Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(partnerData.InfoId, awakenSkinIndex).icon, partnerData.Star, 1);
            AwakenSkinTip.CustomSetActive(awakenSkinIndex == 1);
        }


        private void OnRefreshAwakenView()
        {
            RefreshAwakeRP();
            SetPartnerAwakeCondition(partnerData.IsAwaken <= 0);
            SetPartnerAwakeReward();
            InitPartnerAwakenConsume(partnerData.IsAwaken <= 0);
        }

        private void SetPartnerAwakeCondition(bool isShow)
        {
            if (!isShow)
            {
                ConditionNameLabel.CustomSetActive(false);
                HasAwkenLabel.text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_AREADY");
                AwakenCondition.text = "";
                return;
            }
            ConditionNameLabel.CustomSetActive(true);
            HasAwkenLabel.text = "";
            string conditionstr = string.Empty;
            string colorStr;
            if (curAwakeTemplate.limitLevel != 0)
            {
                colorStr = partnerData.Level >= curAwakeTemplate.limitLevel ? "[42fe79]" : "[ff6699]";
                conditionstr += string.Format(EB.Localizer.GetString("ID_PARTNER_AWAKEN_CONDITION_1"), "  ", colorStr, curAwakeTemplate.limitLevel);
            }
            if (curAwakeTemplate.limitUpgrade != 0)
            {
                colorStr = partnerData.UpGradeId >= curAwakeTemplate.limitUpgrade ? "[42fe79]" : "[ff6699]";
                conditionstr += string.Format(EB.Localizer.GetString("ID_PARTNER_AWAKEN_CONDITION_3"), "  ", colorStr, Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(curAwakeTemplate.limitUpgrade, partnerData.HeroInfo.char_type).name, "");
            }
            if (curAwakeTemplate.limitStar != 0)
            {
                colorStr = partnerData.Star >= curAwakeTemplate.limitStar ? "[42fe79]" : "[ff6699]";
                conditionstr += string.Format(EB.Localizer.GetString("ID_PARTNER_AWAKEN_CONDITION_2"), "  ", colorStr, curAwakeTemplate.limitStar, "  ");
            }

            AwakenCondition.text = AwakenCondition.transform.GetChild(0).GetComponent<UILabel>().text = conditionstr;
        }



        private void SetPartnerAwakeReward()
        {
            string awakenRewardStr = string.Empty;
            string strColor = "[42fe79]";
            RewardLabel[0].text = strColor + "+" + (curAwakeTemplate.inc_ATK * 100).ToString("0") + "%";
            RewardLabel[1].text = strColor + "+" + (curAwakeTemplate.inc_DEF * 100).ToString("0") + "%";
            RewardLabel[2].text = strColor + "+" + (curAwakeTemplate.inc_MaxHP * 100).ToString("0") + "%";
            RewardLabel[3].text = strColor + "+" + (curAwakeTemplate.inc_speed * 100).ToString("0") + "%";
            switch (curAwakeTemplate.awakeType)
            {
                case 1:
                    Hotfix_LT.Data.SkillTemplate awakenskill = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curAwakeTemplate.laterSkill);
                    Hotfix_LT.Data.SkillTemplate Normalskill = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(curAwakeTemplate.beforeSkill);
                    awakenRewardStr = string.Format(EB.Localizer.GetString("ID_PARTER_AWAKEN_SKILLADD"), strColor, awakenskill.Name);
                    //NormalSkillIcon.spriteName = awakenskill.Icon;
                    //NormalSkillName.text = awakenskill.Name;
                    AwakenSkillTipObj.CustomSetActive(true);
                    SkillCompare.Fill(awakenskill, Normalskill, partnerData);
                    break;
                case 2:
                    AwakenSkillTipObj.CustomSetActive(false);
                    awakenRewardStr = string.Format(EB.Localizer.GetString("ID_PARTER_AWAKEN_ATTRIADD"), strColor, Hotfix_LT.Data.CharacterTemplateManager.Instance.GetAwakenExtraAttri(curAwakeTemplate));
                    break;
                default:
                    break;
            }
            AwakenRewardLabel.text = AwakenRewardLabel.transform.GetChild(0).GetComponent<UILabel>().text = awakenRewardStr;
            if (AwakenSkillTipObj)
            {
                UISprite sprite = AwakenSkillTipObj.GetComponent<UISprite>();
                sprite.UpdateAnchors();
            }
        }

        public void OnCliCkAwakenSkillTip()
        {
            SkillCompare.mDMono.gameObject.CustomSetActive(true);
        }

        private void InitPartnerAwakenConsume(bool isShow)
        {

            if (!isShow)
            {
                ConsumeObj.CustomSetActive(false);
                if (string.IsNullOrEmpty(curAwakeTemplate.awakeSkin))
                {
                    SkinSelectObj.CustomSetActive(false);
                    AwakenBtn.gameObject.CustomSetActive(true);
                    AwakenBtn.transform.GetComponent<UISprite>().color = new Color(1, 0, 1, 1);
                    AwakenBtn.transform.GetComponent<BoxCollider>().enabled = false;
                    AwakenBtnLabel.text = AwakenBtnLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_AREADY");
                }
                else
                {
                    AwakenBtn.gameObject.CustomSetActive(false);
                    SkinSelectObj.CustomSetActive(true);
                    OriginSkinHandle.CustomSetActive(partnerData.CurSkin == 0);
                    AwakenSkinHandle.CustomSetActive(partnerData.CurSkin == 1);
                }

                return;
            }
            SkinSelectObj.CustomSetActive(false);
            AwakenBtn.gameObject.CustomSetActive(true);
            AwakenBtnLabel.text = AwakenBtnLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_AWAKEN_NAME");
            AwakenBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
            AwakenBtn.transform.GetComponent<BoxCollider>().enabled = true;
            int i = 0;
            foreach (var item in curAwakeTemplate.awakeMaterDic)
            {
                consumeIcon = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetCurrencyIconById(item.Key);
                ConsumeArray[i].Fill(int.Parse(item.Key), consumeIcon);
                ConsumeArray[i].mDMono.gameObject.CustomSetActive(true);
                i++;
            }
            for (; i < ConsumeArray.Length; i++)
            {
                ConsumeArray[i].mDMono.gameObject.CustomSetActive(false);
            }
            MaterialdatalookUp.InitDataList(curAwakeTemplate.awakeMaterDic);
            AwakenConsumeGrid.Reposition();
            //AwakenConsumeGrid.gameObject.CustomSetActive(true);
            ConsumeObj.CustomSetActive(true);
        }


        public bool IsCusumeEnough()
        {
            bool isConsumeEnough = true;
            foreach (var item in curAwakeTemplate.awakeMaterDic)
            {
                int num = GameItemUtil.GetInventoryItemNum(item.Key);
                if (num < item.Value)
                {
                    isConsumeEnough = false;
                    return isConsumeEnough;
                }
            }
            return isConsumeEnough;
        }
        public void OnClickConsume(GameObject item)
        {
            int index = item.transform.GetSiblingIndex();
        }

        public void OnClickPartnerAwaken()
        {
            if (partnerData.Level < curAwakeTemplate.limitLevel || partnerData.Star < curAwakeTemplate.limitStar || partnerData.UpGradeId < curAwakeTemplate.limitUpgrade)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_1"));
                return;
            }
            if (!IsCusumeEnough())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_2"));
                return;
            }
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, string.Format(EB.Localizer.GetString("ID_PARTNER_AWAKEN_TIP_4"), partnerData.HeroInfo.name), delegate (int r)
             {
                 if (r == 0)
                 {
                     RequestPartnerAwake();
                 }
             });

        }


        private void RequestPartnerAwake()
        {
            LTPartnerDataManager.Instance.PartnerAwake(partnerData.HeroId, delegate (bool isSuccess)
            {
                if (!isSuccess) return;
                GlobalMenuManager.Instance.Open("ShowAwakeView", partnerData);
            });
        }


        private void InitDressUp()
        {
            //DressCenter.CenterOn(partnerData)
        }


        public void UseAwakeSkin()
        {
            if (partnerData.CurSkin == 1)
            {

                return;
            }
            else
            {
                OnSkinBtnClick(1);
            }

        }
        public void UseOriginSkin()
        {
            if (partnerData.CurSkin == 0)
            {
                return;
            }
            else
            {
                OnSkinBtnClick(0);
            }

        }

        private void SetSkinStateShow()
        {
            OriginSkinHandle.CustomSetActive(partnerData.CurSkin == 0);
            AwakenSkinHandle.CustomSetActive(partnerData.CurSkin == 1);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSkinSelect, partnerData.CurSkin);
        }

        private void OnSkinBtnClick(int skinIndex)
        {
            int sceneId = MainLandLogic.GetInstance().SceneId;
            LTPartnerDataManager.Instance.PartnerUseAwakeSkin(partnerData.HeroId, skinIndex, sceneId, new System.Action<bool>(delegate
            {
                var partner = LTPartnerDataManager.Instance.RefreshSkinData(partnerData.HeroId);
                SetSkinStateShow();
                if (partner.StatId == LTMainHudManager.Instance.UserLeaderTID)
                {
                    if (!AllianceUtil.IsInTransferDart)
                    {
                        Hotfix_LT.Messenger.Raise("SetLeaderEvent");
                    }
                }
            }));
        }

        #endregion


        #region 伙伴皮肤
        Hotfix_LT.Data.FuncTemplate skinOpenTemplate = null;
        public LTPartnerSkinController SkinView;
        [HideInInspector]
        public bool IsSkinViewOpen = false;
        private bool isShowAwakenSkin = false;
        public void OnSkinViewOpenAndClose(bool isShowAwake = false)
        {
            if (IsSkinViewOpen)
            {
                IsSkinViewOpen = false;
                CurType = typeBeforeSkin;
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSkinSelect, partnerData.CurSkin);
                TitleController.SetTitleBtn((int)CurType);
                isShowAwakenSkin = false;
            }
            else
            {
                IsSkinViewOpen = true;
                isShowAwakenSkin = isShowAwake;
                typeBeforeSkin = CurType;
                CurType = CultivateType.Skin;
            }
        }

        public void OnPreviewAwakenSkin()
        {
            OnSkinViewOpenAndClose(true);
        }
        public void ShowSkinBtnState()
        {
            if (skinOpenTemplate == null) skinOpenTemplate = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10088);
            if (skinOpenTemplate != null && !skinOpenTemplate.IsConditionOK())
            {
                SkinBtnObj.CustomSetActive(false);
                return;
            }
            SkinBtnObj.CustomSetActive(LTPartnerDataManager.Instance.IsHaveSkin(partnerData.InfoId));
        }
        #endregion

        #region 点击觉醒皮肤调用
        private void RefreshRedPoint()
        {
            RefreshInfoRP();
            RefreshGradeRP();
            RefreshStarRP();
            RefreshSkillRP();
            RefreshTopLevelRP();
            RefreshAwakeRP();
            RefreshGuardRP();
        }

        private void RefreshInfoRP()
        {
            AttributesRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanLevelUp(partnerData));
        }

        private void RefreshGuardRP()
        {
            GuardRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanGuardLevelUp(partnerData));
        }

        private void RefreshGradeRP()
        {
            GradeUpRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanGradeUp(partnerData));
        }

        private void RefreshStarRP()
        {
            StarUpRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanStarUp(partnerData));
        }

        private void RefreshSkillRP()
        {
            SkillBreakRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanSkillBreak(partnerData));
        }

        private void RefreshTopLevelRP()
        {
            TopLevelRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanTopLevelUp(partnerData));
        }
        private void RefreshAwakeRP()
        {
            AwakeRedPoint.CustomSetActive(LTPartnerDataManager.Instance.IsCanAwaken(partnerData));
        }

        #endregion

        #region From: OtherPlayerAttributesManager
        public static LTAttributesData GetPartnerGradeAttributes(int heroStatId, int gradeId)
        {
            LTAttributesData attrData = new LTAttributesData();
            Hotfix_LT.Data.HeroStatTemplate heroTem = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(heroStatId);
            Hotfix_LT.Data.HeroInfoTemplate heroInfoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroTem.character_id);

            if (heroTem != null)
            {
                Hotfix_LT.Data.UpGradeInfoTemplate gradeInfoByGrade = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(gradeId, heroInfoTpl.char_type);
                Hotfix_LT.Data.LevelUpInfoTemplate BaseAttr = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetLevelUpInfoByIDAndLevel(heroStatId, gradeInfoByGrade.level);

                if (gradeInfoByGrade != null)
                {
                    attrData.m_MaxHP = BaseAttr.maxHP * gradeInfoByGrade.inc_maxhp;
                    attrData.m_ATK = BaseAttr.ATK * gradeInfoByGrade.inc_atk;
                    attrData.m_DEF = BaseAttr.DEF * gradeInfoByGrade.inc_def;
                    attrData.m_Speed = BaseAttr.speed * gradeInfoByGrade.inc_speed;
                }
            }

            return attrData;
        }
        #endregion
    }
}
