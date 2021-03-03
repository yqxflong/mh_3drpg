using System;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using LT.Hotfix.Utility;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipMainController : UIControllerHotfix
    {
        public enum EquipmentViewState
        {
            DefaultView,
            LevelView,
            SynthesisView,
            PresetView,
            SelectPartnerView,
            PresetEditView,
        }

        public EquipmentViewState CurrentEquipmentViewState { get; set; } = EquipmentViewState.DefaultView;

        public static LTPartnerEquipMainController instance;
        public static bool m_Open;

        public override bool ShowUIBlocker { get { return false; } }
        public GameObject EquipView_SuitBtn, EquipView_FastSelectList, EquipView_SortBtn;
        public UILabel EquipView_SortBtnLabel;

        public ToggleGroupState ToggleGroupState;
        public UILabel UplevelViewLabel;

        public LTPartnerEquipScroll _partnerEquipScroll;
        public UILabel EquipmentCountLabel;
        private List<BaseEquipmentInfo> ListData;
        private List<BaseEquipmentInfo> CurrentDatas;

        public LTPartnerEquipPartnerInfoController HeroInfoController;
        public LTPartnerEquipUplevelController EquipmentUplevelController;
        public GameObject UnEquipAllObj;

        private GameObject leftSynCellBg;
        private GameObject rightSynCellBg;

        public override void Awake()
        {
            base.Awake();
            instance = this;

            var t = controller.transform;
            m_Open = false;
            EquipView_SuitBtn = t.FindEx("Center/EquipmentItems/PartnerList/Contain/Bottom/AllSuitBtn").gameObject;
            EquipView_FastSelectList = t.FindEx("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList").gameObject;
            EquipView_SortBtn = t.FindEx("Center/EquipmentItems/PartnerList/Contain/Bottom/SortBtn").gameObject;
            EquipView_SortBtnLabel = t.GetComponent<UILabel>("Center/EquipmentItems/PartnerList/Contain/Bottom/SortBtn/Label");

            ToggleGroupState = t.GetComponent<ToggleGroupState>("Center/EquipmentItems/PartnerList/Top/RaceTab");
            UplevelViewLabel = t.GetComponent<UILabel>("Center/EquipmentItems/PartnerList/Top/Label");
            _partnerEquipScroll = t.GetMonoILRComponent<LTPartnerEquipScroll>("Center/EquipmentItems/PartnerList/Contain/SlotsContainer/Placeholder/Grid");
            EquipmentCountLabel = t.GetComponent<UILabel>("Center/EquipmentItems/PartnerList/Contain/Bottom/NumLabel");
            HeroInfoController = t.GetMonoILRComponent<LTPartnerEquipPartnerInfoController>("Center/PartnerDataInfo");
            EquipmentUplevelController = t.GetMonoILRComponent<LTPartnerEquipUplevelController>("Center/UplevelView");
            UnEquipAllObj = t.FindEx("Center/PartnerDataInfo/PartnerView/Anchor/UnEquipAll").gameObject;
            canFocus = true;
            SelectSuitView = t.FindEx("SelectSuit").gameObject;
            _selectSortView = t.FindEx("SelectSort").gameObject;
            SelectSuitType = -1;
            ScollView = t.GetMonoILRComponent<LTEquipmentSuitInfoGridController>("SelectSuit/Content/ScollView/Placeholder/Grid");
            _equipSortGridCtrl = t.GetMonoILRComponent<LTEquipmentSortGridController>("SelectSort/Content/ScollView/Placeholder/Grid");
            SelectSprite = new GameObject[6];
            SelectSprite[0] = t.Find("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList/Btn/Select").gameObject;
            SelectSprite[1] = t.Find("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList/Btn (1)/Select").gameObject;
            SelectSprite[2] = t.Find("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList/Btn (2)/Select").gameObject;
            SelectSprite[3] = t.Find("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList/Btn (3)/Select").gameObject;
            SelectSprite[4] = t.Find("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList/Btn (4)/Select").gameObject;
            SelectSprite[5] = t.Find("Center/EquipmentItems/PartnerList/Contain/Bottom/SelectList/Btn (5)/Select").gameObject;
            CurLevelupEid = 0;
            SuitView = t.GetMonoILRComponent<LTSuitSourceInfoController>("Source");
            SelectPartnerView = t.FindEx("Center/PartnerDataInfo/PartnerUI").gameObject;
            EquipmentsView = t.FindEx("Center/EquipmentItems").gameObject;
            TableScroll = t.GetMonoILRComponent<LTEquipPartnerScroll>("Center/PartnerDataInfo/PartnerUI/PartnerList/Container/Placeholder/Grid");

            TypeBtnList = new List<UIButton>();
            TypeBtnList.Add(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/All"));
            TypeBtnList.Add(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/N"));
            TypeBtnList.Add(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/R"));
            TypeBtnList.Add(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/SR"));
            TypeBtnList.Add(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/SSR"));
            TypeBtnList.Add(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/UR"));

            SelectGradeSp = t.GetComponent<UISprite>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/SelectBtn/Sprite");
            SelectGradeLab = t.FindEx("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/SelectBtn/AllLebel").gameObject;
            SelectGradeTP = t.GetComponent<TweenPosition>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType");
            LeftSynEquipCell = t.GetMonoILRComponent<LTPartnerEquipCellController>("Center/SynthesisView/PartnerView/EquipLeft/AA (1)");
            RightSynEquipCell = t.GetMonoILRComponent<LTPartnerEquipCellController>("Center/SynthesisView/PartnerView/EquipRight/AA (1)");
            PreviewEquipCell = t.GetMonoILRComponent<LTEquipmentSynPreViewController>("Center/SynthesisView/PartnerView/AAPreview");
            MainAttrLabel = t.GetComponent<UILabel>("Center/SynthesisView/PartnerView/mianAttrlabel");
            EquipSynView = t.FindEx("Center/SynthesisView").gameObject;
            StoneNum = t.GetComponent<UILabel>("Center/SynthesisView/PartnerView/EquipSynMaterial/Label");
            LeftEquipName = t.GetComponent<UILabel>("Center/SynthesisView/PartnerView/EquipLeft/Label");
            leftSynCellBg = t.Find("Center/SynthesisView/PartnerView/EquipLeft/LvlBorder").gameObject;
            rightSynCellBg = t.Find("Center/SynthesisView/PartnerView/EquipRight/LvlBorder").gameObject;
            RightEquipName = t.GetComponent<UILabel>("Center/SynthesisView/PartnerView/EquipRight/Label");
            EquipsynFx = t.GetComponent<ParticleSystem>("Center/SynthesisView/PartnerView/fx_hb_UI_Xuancai");
            t.GetComponent<UIButton>("BG/CancelBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));


            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Item").onClick.Add(new EventDelegate(OnSelectPartnerOpenBtnClick));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon/Icon").onClick.Add(new EventDelegate(() => SuitInfoBtnClick(t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon").SData)));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (1)/Icon").onClick.Add(new EventDelegate(() => SuitInfoBtnClick(t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (1)").SData)));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (2)/Icon").onClick.Add(new EventDelegate(() => SuitInfoBtnClick(t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (2)").SData)));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (3)/Icon").onClick.Add(new EventDelegate(() => SuitInfoBtnClick(t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (3)").SData)));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (4)/Icon").onClick.Add(new EventDelegate(() => SuitInfoBtnClick(t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (4)").SData)));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (5)/Icon").onClick.Add(new EventDelegate(() => SuitInfoBtnClick(t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("Center/PartnerDataInfo/PartnerView/EquipSuitView/Container/EquipmentIcon (5)").SData)));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Equip/item").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/1_/UnPress"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Equip/item (1)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/2_/UnPress"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Equip/item (2)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/3_/UnPress"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Equip/item (3)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/4_/UnPress"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Equip/item (4)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/5_/UnPress"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerView/Equip/item (5)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/6_/UnPress"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/N").onClick.Add(new EventDelegate(() => OnPartnerTypeBtnClick(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/N"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/R").onClick.Add(new EventDelegate(() => OnPartnerTypeBtnClick(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/R"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/SR").onClick.Add(new EventDelegate(() => OnPartnerTypeBtnClick(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/SR"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/SSR").onClick.Add(new EventDelegate(() => OnPartnerTypeBtnClick(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/SSR"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/UR").onClick.Add(new EventDelegate(() => OnPartnerTypeBtnClick(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/UR"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/All").onClick.Add(new EventDelegate(() => OnPartnerTypeBtnClick(t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/Btn/All"))));
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerTypeList/Anchor/PartnerType/SelectBtn").onClick.Add(new EventDelegate(OnSelectGradeBtnClick));
            //SureBtn 需要
            t.GetComponent<UIButton>("Center/PartnerDataInfo/PartnerUI/PartnerList/SureBtn").onClick.Add(new EventDelegate(OnSelectPartnerCloseBtnClick));
            t.GetComponent<UIButton>("Center/EquipmentItems/PartnerList/Contain/Bottom/AllSuitBtn").onClick.Add(new EventDelegate(OnSelectSuitOpenBtnClick));
            t.GetComponent<UIButton>("Center/EquipmentItems/PartnerList/Contain/Bottom/SortBtn").onClick.Add(new EventDelegate(OnSortBtnClick));
            t.GetComponent<UIButton>("Center/SynthesisView/PartnerView/RuleBtn").onClick.Add(new EventDelegate(OnEquipSynRuleBtnClick));
            t.GetComponent<UIButton>("SelectSuit/Other/CloseBtn").onClick.Add(new EventDelegate(OnSelectSuitCloseBtnClick));
            t.GetComponent<UIButton>("SelectSuit/Other/ChooseBtn").onClick.Add(new EventDelegate(ResetSuitType));
            //t.GetComponent<UIButton>("FastSelect/UpBtn/2Btn").onClick.Add(new EventDelegate(() => t.GetComponent<LTFastSelectController>("FastSelect").OnFastSelectBtnClick(t.GetComponent<Transform>("FastSelect/UpBtn/2Btn").gameObject)));
            //t.GetComponent<UIButton>("FastSelect/UpBtn/3Btn").onClick.Add(new EventDelegate(() => t.GetComponent<LTFastSelectController>("FastSelect").OnFastSelectBtnClick(t.GetComponent<Transform>("FastSelect/UpBtn/3Btn").gameObject)));
            //t.GetComponent<UIButton>("FastSelect/UpBtn/4Btn").onClick.Add(new EventDelegate(() => t.GetComponent<LTFastSelectController>("FastSelect").OnFastSelectBtnClick(t.GetComponent<Transform>("FastSelect/UpBtn/4Btn").gameObject)));
            //t.GetComponent<UIButton>("FastSelect/UpBtn/5Btn").onClick.Add(new EventDelegate(() => t.GetComponent<LTFastSelectController>("FastSelect").OnFastSelectBtnClick(t.GetComponent<Transform>("FastSelect/UpBtn/5Btn").gameObject)));
            //t.GetComponent<UIButton>("FastSelect/UpBtn/6Btn").onClick.Add(new EventDelegate(() => t.GetComponent<LTFastSelectController>("FastSelect").OnFastSelectBtnClick(t.GetComponent<Transform>("FastSelect/UpBtn/6Btn").gameObject)));
            t.GetComponent<UIButton>("Center/UplevelView/PartnerView/UpLevelBtnRoot/SelectBtn").onClick.Add(new EventDelegate(FastSelectViewSureBtnClick));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Center/UplevelView/PartnerView/UpLevelBtnRoot/LevelUpBtn").clickEvent.Add(new EventDelegate(OnEquipmentLevelUpBtnClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Center/UplevelView/PartnerView/SynthesisUI/EquipSynthesisBtn").clickEvent.Add(new EventDelegate(OnEquipSynthesisBtnCliclk));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Center/SynthesisView/PartnerView/EquipSynthesisBtn").clickEvent.Add(new EventDelegate(OnClickEquipSynBtn));
            t.GetComponent<ConsecutiveClickCoolTrigger>("Center/SynthesisView/PartnerView/EquipSynMaterial").clickEvent.Add(new EventDelegate(OnclickStone));


            t.GetComponent<TweenScale>("Center/EquipmentItems/PartnerList/Top/RaceTab/1_/PressSprite").onFinished.Add(new EventDelegate(() => OnEquipTypeFinishShow(t.FindEx("Center/EquipmentItems/PartnerList/Top/RaceTab/1_/PressSprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Center/EquipmentItems/PartnerList/Top/RaceTab/2_/PressSprite").onFinished.Add(new EventDelegate(() => OnEquipTypeFinishShow(t.FindEx("Center/EquipmentItems/PartnerList/Top/RaceTab/2_/PressSprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Center/EquipmentItems/PartnerList/Top/RaceTab/3_/PressSprite").onFinished.Add(new EventDelegate(() => OnEquipTypeFinishShow(t.FindEx("Center/EquipmentItems/PartnerList/Top/RaceTab/3_/PressSprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Center/EquipmentItems/PartnerList/Top/RaceTab/4_/PressSprite").onFinished.Add(new EventDelegate(() => OnEquipTypeFinishShow(t.FindEx("Center/EquipmentItems/PartnerList/Top/RaceTab/4_/PressSprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Center/EquipmentItems/PartnerList/Top/RaceTab/5_/PressSprite").onFinished.Add(new EventDelegate(() => OnEquipTypeFinishShow(t.FindEx("Center/EquipmentItems/PartnerList/Top/RaceTab/5_/PressSprite/Sprite (1)").gameObject)));
            t.GetComponent<TweenScale>("Center/EquipmentItems/PartnerList/Top/RaceTab/6_/PressSprite").onFinished.Add(new EventDelegate(() => OnEquipTypeFinishShow(t.FindEx("Center/EquipmentItems/PartnerList/Top/RaceTab/6_/PressSprite/Sprite (1)").gameObject)));

            ToggleAddChangeListener(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/1_/UnPress"));
            ToggleAddChangeListener(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/2_/UnPress"));
            ToggleAddChangeListener(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/3_/UnPress"));
            ToggleAddChangeListener(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/4_/UnPress"));
            ToggleAddChangeListener(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/5_/UnPress"));
            ToggleAddChangeListener(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/6_/UnPress"));

            #region 装备预设
            _openEquipmentPresetBtn = t.FindEx("Center/EquipmentItems/Btn_EquipmentPreset").gameObject;
            _saveEquipmentPresetBtn = t.FindEx("Center/EquipmentItems/PartnerList/Contain/Bottom/Btn_Save").gameObject;
            _equipmentInfoController = t.FindEx("Center/EquipmentInfos").GetMonoILRComponent<LTPartnerEquipmentInfoController>();
            _equipmentPresetController = t.FindEx("Center/EquipmentPreset").GetMonoILRComponent<LTPartnerEquipmentPresetController>();
            t.GetComponent<UIButton>("Center/EquipmentItems/Btn_EquipmentPreset").onClick.Add(new EventDelegate(OpenEquipmentPresetView));
            _btnSaveScheme = t.GetComponent<UIButton>("Center/EquipmentItems/PartnerList/Contain/Bottom/Btn_Save");
            _btnSaveScheme.onClick.Add(new EventDelegate(OnClickSaveButton));
            t.GetComponent<UIButton>("Center/EquipmentInfos/Equip/item").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/1_/UnPress"))));
            t.GetComponent<UIButton>("Center/EquipmentInfos/Equip/item (1)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/2_/UnPress"))));
            t.GetComponent<UIButton>("Center/EquipmentInfos/Equip/item (2)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/3_/UnPress"))));
            t.GetComponent<UIButton>("Center/EquipmentInfos/Equip/item (3)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/4_/UnPress"))));
            t.GetComponent<UIButton>("Center/EquipmentInfos/Equip/item (4)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/5_/UnPress"))));
            t.GetComponent<UIButton>("Center/EquipmentInfos/Equip/item (5)").onClick.Add(new EventDelegate(() => OnEquipItemClick(t.GetComponent<UIToggle>("Center/EquipmentItems/PartnerList/Top/RaceTab/6_/UnPress"))));
            #endregion
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        private void ToggleAddChangeListener(UIToggle toggle)
        {
            toggle.onChange.Add(new EventDelegate(() => OnEquipTypeBtnClick(toggle)));
            toggle.onChange.Add(new EventDelegate(() => ChangeEquipTypeBtnPos(toggle.transform.parent.Find("Sprite (2)").gameObject)));
        }

        public override bool IsFullscreen()
        {
            return true;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            mParam = param;
            Hashtable data = param as Hashtable;
            if (data == null)
            {
                if (param == null)
                {
                    CurrentPartnerData = LTPartnerEquipDataManager.Instance.CurrentPartnerData;
                    //LTPartnerEquipDataManager.Instance.CurrentPartnerData = null;
                }
                else
                    CurrentPartnerData = (LTPartnerData)param;
                LTPartnerEquipDataManager.Instance.CurType = EquipPartType.part1;
            }
            else
            {
                if (data["partnerData"] != null)
                {
                    CurrentPartnerData = (LTPartnerData)data["partnerData"];
                }
                else
                {
                    CurrentPartnerData = LTPartnerDataManager.Instance.GetPartnerByStatId(LTPartnerDataManager.Instance.DropSelectPartnerId);//(LTPartnerData)param;
                }
                if (data["equipType"] != null)
                {
                    EquipPartType type = (EquipPartType)data["equipType"];
                    LTPartnerEquipDataManager.Instance.CurType = type;
                }
            }
            InitFastSelectList();
            partnerDataList = new List<LTPartnerData>();
            // CurPartnerGrade = PartnerGrade.ALL;

            ListData = LTPartnerEquipDataManager.Instance.GetAllEquipsWithOrdered();
            //重置为全部套装
            LTPartnerEquipDataManager.Instance.CurSuitType = -1;
            EquipView_SuitBtn.transform.GetChild(0).GetComponent<UILabel>().text = EquipView_SuitBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text =
                string.Format("{0}{1}", EB.Localizer.GetString("ID_ALL"), EB.Localizer.GetString("ID_SUIT"));

       
            InitStoneState();
        }
        private object mParam;
        public override IEnumerator OnAddToStack()
        {
            GlobalMenuManager.Instance.PushCache("LTPartnerEquipmentHud");
            yield return base.OnAddToStack();
            m_Open = true;
            CurrentEquipmentViewState = EquipmentViewState.DefaultView;
            EquipSelectPartnerEven.SelectPartnerID += OnSelectFun;
            EquipmentInfoEven.SelectEquipmentID += OnEquipLevelUp;
            EquipmentInfoEven.SelectEquipIDBySyn += OnEquipSynCancleClick;
            SelectEquipEven.ChooseEquipment += OnSelectEquip;
            SelectEquipEven.SelectSynEquipment += OnEquipSynthesisSelect;
            EquipSynArray[0] = EquipSynArray[1] = null;
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnPartnerEquipChangeFunc);
            InitStoneState();
            yield return null;
            UpdatePartnerEquipmentInfoView();

            for (int i = 0; i < ToggleGroupState.m_Toggles.Count; i++)
            {
                if (ParseEquipmentType(ToggleGroupState.m_Toggles[i].transform.parent.name, ((int)LTPartnerEquipDataManager.Instance.CurType).ToString()))
                {

                    // Debug.LogError(LTPartnerEquipDataManager.Instance.CurType);
                    // Debug.LogError(i);
                    if (ToggleGroupState.m_Toggles[i].value)
                    {
                        // Debug.LogError("11111111");
                        OnUpdataEquipmentItemsView();
                    }
                    else
                    {
                        // Debug.LogError("11111111");
                        OnEquipItemClick(ToggleGroupState.m_Toggles[i]);
                    }
                }
                
                if (GuideNodeManager.IsGuide)
                {
                    ToggleGroupState.m_Toggles[i].GetComponent<BoxCollider>().enabled = false;
                }
            }
            
            Hashtable data = mParam as Hashtable;
            if (data != null && data["equipId"] != null)
            {
                int equipId = (int)data["equipId"];
                OnEquipLevelUp(equipId);
            }
        }
        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            CurrentEquipmentViewState = EquipmentViewState.DefaultView;
            EquipSelectPartnerEven.SelectPartnerID -= OnSelectFun;
            EquipmentInfoEven.SelectEquipmentID -= OnEquipLevelUp;
            EquipmentInfoEven.SelectEquipIDBySyn -= OnEquipSynCancleClick;
            SelectEquipEven.ChooseEquipment -= OnSelectEquip;
            SelectEquipEven.SelectSynEquipment -= OnEquipSynthesisSelect;
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnPartnerEquipChangeFunc);
            _partnerEquipScroll.Clear();
            DestroySelf();
            yield break;
        }
        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
            UITweener[] tweeners = controller.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        public void OnPartnerEquipChangeFunc()
        {
            if (CurrentEquipmentViewState == EquipmentViewState.SynthesisView)
            {
                ReflashLeftSynUI();
                return;
            }

            OnUpdataEquipmentItemsView();
            UpdatePartnerEquipmentInfoView();
        }

        //套装选择函数
        public void ResetSuitType()
        {
            LTPartnerEquipDataManager.Instance.CurSuitType = -1;
            EquipView_SuitBtn.transform.GetChild(0).GetComponent<UILabel>().text = EquipView_SuitBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipMainController_4793");
            OnSelectSuitCloseBtnClick();
            UIScrollView scrollView = _partnerEquipScroll.mDMono.transform.GetComponentInParent<UIScrollView>();
            scrollView.verticalScrollBar.value = 0;

            if (CurrentEquipmentViewState == EquipmentViewState.SynthesisView)
            {
                UpdateRightSynEquip();
                return;
            }

            UpdataEquipmentItemsView();
        }

        public void SetSuitType(Hotfix_LT.Data.SuitTypeInfo type)
        {
            if (LTPartnerEquipDataManager.Instance.CurSuitType == type.SuitType) { OnSelectSuitCloseBtnClick(); return; }
            LTPartnerEquipDataManager.Instance.CurSuitType = type.SuitType;
            EquipView_SuitBtn.transform.GetChild(0).GetComponent<UILabel>().text = EquipView_SuitBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", LTPartnerEquipDataManager.Instance.CurSuitType == -1 ? EB.Localizer.GetString("ID_ALL") : type.TypeName, EB.Localizer.GetString("ID_SUIT"));
            OnSelectSuitCloseBtnClick();

            if (CurrentEquipmentViewState == EquipmentViewState.SynthesisView)
            {
                UpdateRightSynEquip();
                return;
            }

            UpdataEquipmentItemsView();
        }

        public void OnUpdataEquipmentItemsView()
        {

            ListData = LTPartnerEquipDataManager.Instance.GetAllEquipsWithOrdered();
            UpdataEquipmentItemsView();
        }

        private void UpdataEquipmentItemsView()
        {
            CurrentDatas = new List<BaseEquipmentInfo>();
            int EquipCount = 0;
            HashSet<int> eids = null;

            if (CurrentEquipmentViewState == EquipmentViewState.LevelView)
            {
                eids = InventoryUtility.GetIdsInEquipmentPreset();
            }

            for (var i = 0; i < ListData.Count; i++)
            {
                BaseEquipmentInfo obj = ListData[i];

                if (CurrentEquipmentViewState == EquipmentViewState.LevelView)
                {
                    bool isUplevelId = CurLevelupEid == obj.Eid;

                    if (!obj.isDress && !isUplevelId && !eids.Contains(obj.Eid))
                    {
                        if (obj.SuitType != -1)
                        {
                            EquipCount++;
                        }

                        CurrentDatas.Add(obj);
                    }
                }
                else if (CurrentEquipmentViewState == EquipmentViewState.PresetEditView)
                {
                    if (obj.SuitType != -1)
                    {
                        EquipCount++;
                    }

                    bool isSelectSuitType = (LTPartnerEquipDataManager.Instance.CurSuitType == -1) ? true : obj.SuitType == LTPartnerEquipDataManager.Instance.CurSuitType;
                    bool isBelongEquipmentPreset = false;
                    int[] currentEids = _equipmentInfoController.Eids;

                    for (var j = 0; j < currentEids.Length; j++) {
                        if (currentEids[j] == obj.Eid) {
                            isBelongEquipmentPreset = true;
                            break;
                        }
                    }

                    if (obj.Type == LTPartnerEquipDataManager.Instance.CurType && isSelectSuitType && !isBelongEquipmentPreset)
                    {
                        CurrentDatas.Add(obj);
                    }
                }
                else
                {
                    bool isSelectSuitType = (LTPartnerEquipDataManager.Instance.CurSuitType == -1) ? true : obj.SuitType == LTPartnerEquipDataManager.Instance.CurSuitType;

                    if (!obj.isDress && obj.SuitType != -1)
                    {
                        EquipCount++;
                    }

                    if (obj.Type == LTPartnerEquipDataManager.Instance.CurType && !obj.isDress && isSelectSuitType)
                    {
                        CurrentDatas.Add(obj);
                    }
                }
            }

            ToggleGroupState.gameObject.CustomSetActive(CurrentEquipmentViewState != EquipmentViewState.LevelView);
            UplevelViewLabel.gameObject.CustomSetActive(CurrentEquipmentViewState == EquipmentViewState.LevelView);
            int extraAdd = LTPartnerConfig.Equip_BASE_MAX_VALUE + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InvEquipCount);
            string CountStr = EquipCount > extraAdd ? "[ff6699]"+extraAdd : extraAdd.ToString();
            EquipmentCountLabel.text = EquipmentCountLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[42fe79]{0}[-]/{1}", EquipCount, CountStr);//装备显示数量
            SortFunc();

            for (int i = CurrentDatas.Count; i < 25; i++)
            {
                CurrentDatas.Add(null);
            }

            _partnerEquipScroll.SetItemDatas(CurrentDatas);
        }

        public void UpdatePartnerEquipmentInfoView()
        {
            if (CurrentEquipmentViewState == EquipmentViewState.PresetView || CurrentEquipmentViewState == EquipmentViewState.PresetEditView) 
            {
                _equipmentInfoController.Refresh();
            } 

            HeroInfoController.Show(CurrentPartnerData);
        }

        private const float UnpressLenth = 167f;
        private const float TypeBtnLenth = 91.8f;
        private GameObject curObj = null;
        public void ChangeEquipTypeBtnPos(GameObject obj)
        {
            if (!(LTPartnerEquipDataManager.Instance.CurType == ParseEquipTypeTabType(obj.transform.parent.name))) return;
            float TempX = 0;
            float tempX = 0;
            if (curObj != null && curObj != obj)
            {
                curObj.CustomSetActive(true);
            }
            curObj = obj;
            obj.CustomSetActive(false);
            for (int i = 0; i < ToggleGroupState.m_Toggles.Count; i++)
            {
                bool value = LTPartnerEquipDataManager.Instance.CurType == ParseEquipTypeTabType(ToggleGroupState.m_Toggles[i].transform.parent.name);
                TempX += tempX + (value ? UnpressLenth : TypeBtnLenth);
                tempX = value ? UnpressLenth : TypeBtnLenth;
                int tempY = value ? 8 : 0;

                ToggleGroupState.m_Toggles[i].transform.parent.transform.localPosition = new Vector3(TempX, tempY, 0);
            }
        }
        public void OnEquipTypeFinishShow(GameObject obj)
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

        public static bool canFocus = true;
        public override void OnFocus()
        {
            base.OnFocus();
            if (CurrentEquipmentViewState == EquipmentViewState.LevelView)
            {
                int costCount = GetCostValue();
                int resGold = BalanceResourceUtil.GetUserGold();
                string colorStr2 = (costCount > resGold) ? "[ff6699]" : "[ffffff]";
                EquipmentUplevelController.CostLabel.text = EquipmentUplevelController.CostLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("{0}{1}", colorStr2, costCount);
            }

            if (!GuideNodeManager.IsGuide)
            {
                if (!ToggleGroupState.m_Toggles[0].GetComponent<BoxCollider>().enabled)
                {
                    for (int i = 0; i < ToggleGroupState.m_Toggles.Count; i++)
                    {
                        ToggleGroupState.m_Toggles[i].GetComponent<BoxCollider>().enabled = true;
                    }
                }
            }
            InvokeCloseAction = false;
		}

        public override void OnCancelButtonClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick", controller.gameObject, true);

            switch (CurrentEquipmentViewState)
            {
                case EquipmentViewState.LevelView:
                    LTPartnerEquipDataManager.Instance.cleanEquipUpItemNum();
                    OnEquipLevelUpClose();
                    break;
                case EquipmentViewState.SynthesisView:
                    LTPartnerEquipDataManager.Instance.CleanEquipSynItemNum();
                    OnEquipSynCancleClick(EquipSyneid);
                    break;
                case EquipmentViewState.SelectPartnerView:
                    OnSelectPartnerCloseBtnClick();
                    break;
                case EquipmentViewState.PresetView:
                    CloseEquipmentPresetView();
                    break;
                case EquipmentViewState.PresetEditView:
                    _equipmentInfoController.OnQuitEdit();
                    break;
                default:
                    GlobalMenuManager.Instance.RemoveCache("LTPartnerEquipmentHud");
                    LTPartnerEquipDataManager.Instance.CurrentPartnerData = null;
                    if (GlobalMenuManager.Instance.GetMenuByPrefabName("LTPartnerHud") != null) GlobalMenuManager.Instance.GetMenuByPrefabName("LTPartnerHud").HasPlayedTween = false;
                    base.OnCancelButtonClick();
                    break;
            }
        }

        #region （装备信息相关）
        //装备类型选择按键
        public void OnEquipItemClick(UIToggle ui_toggle)
        {
            ui_toggle.Set(true);
        }
        public void OnEquipTypeBtnClick(UIToggle ui_toggle)
        {
            if (!ui_toggle.value) return;
            LTPartnerEquipDataManager.Instance.CurType = ParseEquipTypeTabType(ui_toggle.transform.parent.name);
            UpdataEquipmentItemsView();
            HeroInfoController.TypeSelect();
            _equipmentInfoController.TypeSelect();
        }
        private bool ParseEquipmentType(string str1, string str2)
        {
            if (str1.Contains(str2)) { return true; }
            else return false;
        }
        private EquipPartType ParseEquipTypeTabType(string str)
        {
            if (str.Contains("1"))
                return EquipPartType.part1;
            else if (str.Contains("2"))
                return EquipPartType.part2;
            else if (str.Contains("3"))
                return EquipPartType.part3;
            else if (str.Contains("4"))
                return EquipPartType.part4;
            else if (str.Contains("5"))
                return EquipPartType.part5;
            else if (str.Contains("6"))
                return EquipPartType.part6;
            EB.Debug.LogError("ParseTabType error str={0}", str);
            return EquipPartType.none;
        }

        //套装选择按键
        public GameObject SelectSuitView;
        public static int SelectSuitType = -1;
        public LTEquipmentSuitInfoGridController ScollView;
        public void OnSelectSuitOpenBtnClick()
        {
            ScollView.SetItemDatas(Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfos());
            ScollView.scrollView.ResetPosition();
            SelectSuitView.CustomSetActive(true);
            SelectSuitView.GetComponent<TweenScale>().ResetToBeginning();
            SelectSuitView.GetComponent<TweenScale>().PlayForward();
        }
        public void OnSelectSuitCloseBtnClick()
        {
            SelectSuitView.CustomSetActive(false);
            StartCoroutine(ViewPlayCloseIE());
        }

        IEnumerator ViewPlayCloseIE()
        {
            // if(ScollView.mDMono.gameObject.activeSelf)ScollView.SetItemDatas(Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfos());
            yield return new WaitForSeconds(0.1f);
            SelectSuitView.GetComponent<TweenScale>().PlayReverse();
            yield return new WaitForSeconds(0.2f);
            SelectSuitView.CustomSetActive(false);
        }

        private LTEquipmentSortGridController _equipSortGridCtrl;
        private GameObject _selectSortView;

        //排序按键
        public void OnSortBtnClick()
        {
            _equipSortGridCtrl.SetItemDatas(EquipmentUtility.GetSelectSortList());
            _equipSortGridCtrl.scrollView.ResetPosition();
            _selectSortView.CustomSetActive(true);
            _selectSortView.GetComponent<TweenScale>().ResetToBeginning();
            _selectSortView.GetComponent<TweenScale>().PlayForward();
        }

        public void SetSortType(EquipmentSortType type)
        {
            if (LTPartnerEquipDataManager.Instance.CurSortType == type) 
            { 
                return; 
            }

            LTPartnerEquipDataManager.Instance.CurSortType = type;
            SortFunc();
            _partnerEquipScroll.SetItemDatas(CurrentDatas);
            _selectSortView.CustomSetActive(false);
        }

        private void SortFunc()
        {
            EquipView_SortBtnLabel.text = EquipmentUtility.AttrTypeTrans(LTPartnerEquipDataManager.Instance.CurSortType.ToString(), false);

            if (CurrentDatas == null || CurrentDatas.Count <= 0)
            {
                return;
            }

            if (CurrentEquipmentViewState == EquipmentViewState.LevelView)
            {
                if (LTPartnerEquipDataManager.Instance.CurSortType == EquipmentSortType.Quality)
                {
                    //品质升序
                    CurrentDatas.Sort((a, b) =>
                    {
                        if (a == null) return 1;
                        if (b == null) return -1;
                        if (a.SuitType == -1)
                        {
                            if (b.SuitType == -1)
                            {
                                if (a.QualityLevel > b.QualityLevel)
                                    return 1;
                                else if (a.QualityLevel < b.QualityLevel)
                                    return -1;
                            }
                            else return -1;
                        }
                        else if (b.SuitType == -1)
                        {
                            return 1;
                        }
                        if (a.QualityLevel < b.QualityLevel)
                            return -1;
                        else if (a.QualityLevel > b.QualityLevel)
                            return 1;
                        if (a.EquipLevel < b.EquipLevel)
                            return -1;
                        else if (a.EquipLevel > b.EquipLevel)
                            return 1;
                        else if (a.Eid > b.Eid)
                            return -1;
                        else
                            return 1;
                    });
                }
                else
                {
                    //等级升序
                    CurrentDatas.Sort((a, b) =>
                    {
                        if (a == null) return 1;
                        if (b == null) return -1;
                        if (a.SuitType == -1)
                        {
                            if (b.SuitType == -1)
                            {
                                if (a.QualityLevel > b.QualityLevel)
                                    return 1;
                                else if (a.QualityLevel < b.QualityLevel)
                                    return -1;
                            }
                            else return -1;
                        }
                        else if (b.SuitType == -1)
                        {
                            return 1;
                        }
                        if (a.EquipLevel < b.EquipLevel)
                            return -1;
                        else if (a.EquipLevel > b.EquipLevel)
                            return 1;
                        if (a.QualityLevel < b.QualityLevel)
                            return -1;
                        else if (a.QualityLevel > b.QualityLevel)
                            return 1;
                        else if (a.Eid > b.Eid)
                            return -1;
                        else
                            return 1;
                    });
                }

                return;
            }

            switch (LTPartnerEquipDataManager.Instance.CurSortType) 
            {
                case EquipmentSortType.Quality:
                    // 品质降序
                    CurrentDatas.Sort((a, b) =>
                    {
                        if (a == null) return 1;
                        if (b == null) return -1;
                        if (a.QualityLevel > b.QualityLevel)
                            return -1;
                        else if (a.QualityLevel < b.QualityLevel)
                            return 1;
                        if (a.EquipLevel > b.EquipLevel)
                            return -1;
                        else if (a.EquipLevel < b.EquipLevel)
                            return 1;
                        else if (a.Eid > b.Eid)
                            return -1;
                        else
                            return 1;
                    });
                    break;
                case EquipmentSortType.Level:
                    //等级降序
                    CurrentDatas.Sort((a, b) =>
                    {
                        if (a == null) 
                            return 1;
                        if (b == null) 
                            return -1;

                        if (a.EquipLevel > b.EquipLevel)
                            return -1;
                        else if (a.EquipLevel < b.EquipLevel)
                            return 1;
                        if (a.QualityLevel > b.QualityLevel)
                            return -1;
                        else if (a.QualityLevel < b.QualityLevel)
                            return 1;
                        else if (a.Eid > b.Eid)
                            return -1;
                        else
                            return 1;
                    });
                    break;
                case EquipmentSortType.GetOrder:
                    CurrentDatas.Sort((a, b) =>
                    {
                        if (a == null) 
                            return 1;
                        if (b == null) 
                            return -1;

                        if (a.Eid > b.Eid)
                            return -1;
                        else if (a.Eid < b.Eid)
                            return 1;
                        else if (a.QualityLevel > b.QualityLevel)
                            return -1;
                        else if (a.QualityLevel < b.QualityLevel)
                            return 1;
                        else if (a.EquipLevel > b.EquipLevel)
                            return -1;
                        else if (a.EquipLevel < b.EquipLevel)
                            return 1;
                        else
                            return 1;
                    });
                    break;
                default:
                    CurrentDatas.Sort((a, b) =>
                    {
                        if (a == null)
                            return 1;
                        if (b == null)
                            return -1;

                        var val = AttributeSort(a.Eid, b.Eid, LTPartnerEquipDataManager.Instance.CurSortType);

                        if (val == 0)
                        {
                            if (a.QualityLevel > b.QualityLevel)
                                return -1;
                            else if (a.QualityLevel < b.QualityLevel)
                                return 1;
                            else if (a.EquipLevel > b.EquipLevel)
                                return -1;
                            else if (a.EquipLevel < b.EquipLevel)
                                return 1;
                            else
                                return 1;
                        }

                        return val;
                    });
                    break;
            }
        }

        private int AttributeSort(int Eid1, int Eid2, EquipmentSortType type)
        {
            var detailEquipInfo1 = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(Eid1);
            var detailEquipInfo2 = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(Eid2);

            if (detailEquipInfo1 == null)
            { 
                return 0;
            }

            if (detailEquipInfo2 == null)
            {
                return 0;
            }

            string attrStr = (type == EquipmentSortType.Speed) ? "speedrate" : type.ToString();
            float val1 = 0f;
            float val2 = 0f;

            if (detailEquipInfo1.MainAttributes.Name.Equals(attrStr))
            {
                val1 += detailEquipInfo1.MainAttributes.Value;
            }

            if (detailEquipInfo1.ExAttributes != null)
            {
                for (var i = 0; i < detailEquipInfo1.ExAttributes.Count; i++)
                {
                    var attr = detailEquipInfo1.ExAttributes[i];

                    if (attr.Name.Equals(attrStr))
                    {
                        val1 += attr.Value;
                        break;
                    }
                }
            }

            if (detailEquipInfo2.MainAttributes.Name.Equals(attrStr))
            {
                val2 += detailEquipInfo2.MainAttributes.Value;
            }

            if (detailEquipInfo2.ExAttributes != null)
            {
                for (var i = 0; i < detailEquipInfo2.ExAttributes.Count; i++)
                {
                    var attr = detailEquipInfo2.ExAttributes[i];

                    if (attr.Name.Equals(attrStr))
                    {
                        val2 += attr.Value;
                        break;
                    }
                }
            }

            if (val1 > val2)
            {
                return -1;
            }
            else if (val1 < val2)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //快速选择
        public GameObject[] SelectSprite;
        private List<int> FastSelectList = new List<int>();
        private bool isInitSelectBtn = true;
        
        
        private void InitFastSelectList()
        {
            int maxLevel = 0;
            DataLookupsCache.Instance.SearchIntByID("userCampaignStatus.challengeChapters.bigFloor", out maxLevel);
            int qualityLevel = 1;
            if (maxLevel > 0)
            {
                var temp = SceneTemplateManager.Instance.GetCheckPointChapterByChapter(maxLevel);
                if (temp != null)
                {
                    LostChallengeRewardTemplate data = SceneTemplateManager.Instance.GetLostChallengeReward(System.DateTime.Now.DayOfWeek, maxLevel);
                    if (data != null  && data.DropList!=null)
                    {
                        string id = data.DropList[0];
                        var item = EconemyTemplateManager.Instance.GetItem(id);
                        qualityLevel = item.QualityLevel;
                    }
                }
                else
                {
                    qualityLevel = 6;
                }
            }
            SelectConfig = qualityLevel;
        }

        private int SelectConfig=0;
        
        private void ResetFastSelectList(int select = -1)
        {
            select = Mathf.Min(SelectConfig, 6);
            EquipView_FastSelectList.CustomSetActive(true);
            if (isInitSelectBtn)
            {
                isInitSelectBtn = false;
                for (int i = 0; i < SelectSprite.Length; i++)
                {
                    int index = i + 1;
                    SelectSprite[i].transform.parent.GetComponent<UIButton>().onClick.Add(new EventDelegate(delegate
                    {
                        RedultFunc(index);
                    }));
                }
            }

            if (FastSelectList != null)
            {
                if (select > 0)
                {
                    FastSelectList.Clear();
                    FastSelectList.Add(1);
                    for (int i = 1; i < select; ++i)
                    {
                        if (!FastSelectList.Contains(i)) FastSelectList.Add(i);
                    }
                }
                for (int i = 0; i < SelectSprite.Length; i++)
                {
                    SelectSprite[i].CustomSetActive(FastSelectList.Contains(i + 1));
                }
            }
        }
        List<int> Addarray = new List<int>(){3,6,9,12,15};
        public void FastSelectViewSureBtnClick()
        {
            LoadingSpinner.Show();
            if (EquipmentUplevelController.data.EquipLevel < EquipmentUplevelController.LevelUpList.Count)
            {
                int UpLevel = 0;
                int UpExp = 0;
                LTPartnerEquipDataManager.Instance.GetSelectUpLevelItemExp(EquipmentUplevelController.data, out UpLevel, out UpExp);
                int NextLevel = Mathf.Clamp(GetNextLevel(UpLevel), 1, EquipmentUplevelController.LevelUpList.Count);
                int overExp = EquipmentUplevelController.LevelUpList[NextLevel - 1].TotalNeedExp - (EquipmentUplevelController.data.Exp + UpExp);
                if (overExp > 0)
                {
                    //装备快速选择                                                                                                                                      
                    if (FastSelectList.Count > 0)
                    {
                        HashSet<int> eidsFromEquipmentPreset = InventoryUtility.GetIdsInEquipmentPreset(); 

                        for (var i = 0; i < ListData.Count; i++)
                        {
                            var data = ListData[i];
                            if (overExp <= 0) {
                                break;
                            }

                            if (data.SuitType != -1 && FastSelectList.Contains(data.QualityLevel) && 
                                !data.isDress && !data.isLock && data.Eid != CurLevelupEid && 
                                !LTPartnerEquipDataManager.Instance.UpLevelSelectList.Contains(data.Eid) &&
                                !eidsFromEquipmentPreset.Contains(data.Eid))
                            {
                                LTPartnerEquipDataManager.Instance.UpLevelSelectList.Add(data.Eid);
                                overExp -= LTPartnerEquipDataManager.Instance.GetTotleExpByEid(data.Eid);
                            }
                        }
                    }

                    //滞后选择道具
                    for (var k = 0; k < CurrentDatas.Count; k++)
                    {
                        var data = CurrentDatas[k];
                        if (overExp <= 0) break;
                        if (data != null && LTPartnerEquipDataManager.Instance.isEquipUpItem(data.ECid))
                        {
                            int exp = LTPartnerEquipDataManager.Instance.getEquipUpItemExp(data.ECid);
                            int hasUse = LTPartnerEquipDataManager.Instance.getEquipUpItemNum(data.ECid);
                            for (int i = 0; i < data.Num - hasUse; ++i)
                            {
                                LTPartnerEquipDataManager.Instance.addEquipUpItemNum(data.ECid);
                                overExp -= exp;
                                if (overExp <= 0) break;
                            }
                        }
                    }
                }
                UpdataEquipmentItemsView();
                EquipmentUplevelController.ShowLevelUp();
            }
            LoadingSpinner.Hide();
        }
        
        public int GetNextLevel(int cur)
        {
            for (int i = 0; i < Addarray.Count; i++)
            {
                if (cur<Addarray[i])
                {
                    return Addarray[i];
                }
            }

            return cur;
        }
        
        private void RedultFunc(int part)
        {
            if (FastSelectList.Contains(part))
            {
                FastSelectList.Remove(part);
                SelectSprite[part - 1].CustomSetActive(false);
                //去除选择了的装备
                bool refresh = false;
                for (int i = LTPartnerEquipDataManager.Instance.UpLevelSelectList.Count - 1; i >= 0; --i)
                {
                    int quality = 0;
                    if (DataLookupsCache.Instance.SearchIntByID(string.Format("inventory.{0}.qualityLevel", LTPartnerEquipDataManager.Instance.UpLevelSelectList[i]), out quality))
                    {
                        if (quality == part)
                        {
                            LTPartnerEquipDataManager.Instance.UpLevelSelectList.Remove(LTPartnerEquipDataManager.Instance.UpLevelSelectList[i]);
                            refresh = true;
                        }
                    }
                }
                if (refresh)
                {
                    UpdataEquipmentItemsView();
                    EquipmentUplevelController.ShowLevelUp();
                }
            }
            else
            {
                FastSelectList.Add(part);
                SelectSprite[part - 1].CustomSetActive(true);
            }
        }

        #endregion

        #region （伙伴信息与装备升级相关）
        public static int CurLevelupEid = 0;
        private EquipmentViewState _lastEquipmentViewState = EquipmentViewState.DefaultView;

        private void OnEquipLevelUp(int eid)
        {
            if (CurrentEquipmentViewState == EquipmentViewState.PresetView)
            {
                CloseEquipmentPresetView();
                _lastEquipmentViewState = EquipmentViewState.PresetView;
            }
            else if (CurrentEquipmentViewState == EquipmentViewState.PresetEditView)
            {
                CloseEquipmentPresetEditView();
                CloseEquipmentPresetView();
                _lastEquipmentViewState = EquipmentViewState.PresetEditView;
            }
            else
            {
                _lastEquipmentViewState = EquipmentViewState.DefaultView;
            }

            CurLevelupEid = eid;
            CurrentEquipmentViewState = EquipmentViewState.LevelView;
            ClearUpLevelSelect();
            EquipView_SuitBtn.CustomSetActive(false);
            EquipView_SortBtn.CustomSetActive(false);
            _openEquipmentPresetBtn.SetActive(false);


            var temp = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
            if (temp == null)
            {
                EB.Debug.LogError("LTPartnerEquipMainController.Init info is null");
                return;
            }
            ResetFastSelectList(temp.QualityLevel);
            EquipmentUplevelController.Show(eid);
            EquipmentUplevelController.ShowSynthesis(temp);
            ResetSuitType();

            StartCoroutine(OpenEquipLevelUpView());
        }

        private void OnSelectEquip()
        {
            EquipmentUplevelController.ShowLevelUp();
        }

        private void OnEquipLevelUpClose()
        {
            CurLevelupEid = 0;
            CurrentEquipmentViewState = _lastEquipmentViewState;
            ClearUpLevelSelect();
            StartCoroutine(CloseEquipLevelUpView());
            EquipView_SuitBtn.CustomSetActive(true);
            EquipView_SortBtn.CustomSetActive(true);
            EquipView_FastSelectList.CustomSetActive(false);
            OnUpdataEquipmentItemsView();
            UpdatePartnerEquipmentInfoView();
            HeroInfoController.ShowAttr();

            if (CurrentEquipmentViewState == EquipmentViewState.PresetView)
            {
                OpenEquipmentPresetView();
            }
            else if (CurrentEquipmentViewState == EquipmentViewState.PresetEditView)
            {
                OpenEquipmentPresetView(); 
                OpenEquipmentPresetEditView();
            }
            else 
            {
                _openEquipmentPresetBtn.SetActive(true);
            }
        }

        private void ClearUpLevelSelect()
        {
            LTPartnerEquipDataManager.Instance.UpLevelSelectList.Clear();
        }

        IEnumerator OpenEquipLevelUpView()
        {
            EquipmentUplevelController.UpLeveItem.mDMono.GetComponent<TweenScale>().ResetToBeginning();
            EquipmentUplevelController.UpLeveItem.mDMono.GetComponent<TweenAlpha>().ResetToBeginning();
            HeroInfoController.mDMono.GetComponent<TweenAlpha>().PlayForward();
            yield return new WaitForSeconds(HeroInfoController.mDMono.GetComponent<TweenAlpha>().duration);
            HeroInfoController.mDMono.gameObject.CustomSetActive(false);
            EquipmentUplevelController.mDMono.gameObject.CustomSetActive(true);
            EquipmentUplevelController.mDMono.GetComponent<TweenAlpha>().PlayForward();
            EquipmentUplevelController.mDMono.GetComponent<TweenPosition>().PlayForward();
            yield return new WaitForSeconds(EquipmentUplevelController.mDMono.GetComponent<TweenPosition>().duration);
            EquipmentUplevelController.UpLeveItem.mDMono.GetComponent<TweenScale>().PlayForward();
            EquipmentUplevelController.UpLeveItem.mDMono.GetComponent<TweenAlpha>().PlayForward();
            LTPartnerEquipDataManager.Instance.cleanEquipUpItemNum();
        }

        IEnumerator CloseEquipLevelUpView()
        {
            EquipmentUplevelController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
            EquipmentUplevelController.mDMono.GetComponent<TweenPosition>().PlayReverse();
            yield return new WaitForSeconds(EquipmentUplevelController.mDMono.GetComponent<TweenAlpha>().duration);
            EquipmentUplevelController.mDMono.gameObject.CustomSetActive(false);
            HeroInfoController.mDMono.gameObject.CustomSetActive(CurrentEquipmentViewState == EquipmentViewState.DefaultView);
            HeroInfoController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
        }

        public LTSuitSourceInfoController SuitView;

        public void SuitInfoBtnClick(Hotfix_LT.Data.SuitTypeInfo type)
        {
            Vector2 vector2 = UICamera.lastEventPosition;
            SuitView.Show(type, vector2);

            var ts = SuitView.mDMono.transform.GetComponent<TweenScale>();

            if (ts != null)
            {
                ts.ResetToBeginning();
                ts.PlayForward();
            }
        }

        public void SuitInfoViewClose()
        {
            SuitView.mDMono.gameObject.CustomSetActive(false);
        }

        private bool hasresult = false;

        private int GetCostValue()
        {
            if (EquipmentUplevelController.LevelUpList.Count == 0) return 0;
            int totalAddEXP = 0;
            Dictionary<string, int> Dics = LTPartnerEquipDataManager.Instance.getEquipUpItemNumDic();
            foreach (var item in Dics)
            {
                totalAddEXP += LTPartnerEquipDataManager.Instance.getEquipUpItemExp(item.Key) * item.Value;
            }
            for (int i = 0; i < LTPartnerEquipDataManager.Instance.UpLevelSelectList.Count; i++)
            {
                totalAddEXP += LTPartnerEquipDataManager.Instance.GetTotleExpByEid(LTPartnerEquipDataManager.Instance.UpLevelSelectList[i]);
            }
            Hotfix_LT.Data.EquipmentLevelUp LevelUpInfo = new Hotfix_LT.Data.EquipmentLevelUp();
            bool GetExpInfo = false;
            for (int i = EquipmentUplevelController.data.EquipLevel; i < EquipmentUplevelController.LevelUpList.Count; i++)
            {
                if (EquipmentUplevelController.data.Exp + totalAddEXP < EquipmentUplevelController.LevelUpList[i].TotalNeedExp)
                {
                    GetExpInfo = true;
                    LevelUpInfo = EquipmentUplevelController.LevelUpList[i]; break;
                }
            }
            int OverPlueExp = GetExpInfo ? (LevelUpInfo.TotalNeedExp - LevelUpInfo.needExp) : EquipmentUplevelController.LevelUpList[EquipmentUplevelController.LevelUpList.Count-1].TotalNeedExp;
            int costCount = totalAddEXP;
            if (totalAddEXP > 0 && !GetExpInfo)
            {
                costCount = OverPlueExp - EquipmentUplevelController.data.Exp;
            }
            return costCount;
        }

        public void OnEquipmentLevelUpBtnClick()//装备升级
        {
            FusionAudio.PostEvent("UI/Equipment/LvlUpBtn", true);
            if (hasresult == true) return;

            if (LTPartnerEquipDataManager.Instance.UpLevelSelectList.Count <= 0 && LTPartnerEquipDataManager.Instance.getEquipUpItemNumDic().Count <= 0) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipMainController_23384")); return; }

            int costCount = GetCostValue();
            int resGold = BalanceResourceUtil.GetUserGold();
            if (costCount > resGold)
            {
                MessageTemplateManager.ShowMessage(901031, null, delegate (int r)
                {
                    if (r == 0)
                    {
                        InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                        GlobalMenuManager.Instance.Open("LTResourceShopUI");
                    }
                });
                return;
            }

            bool hasValuable = false;
            for (var i = 0; i < LTPartnerEquipDataManager.Instance.UpLevelSelectList.Count; i++)
            {
                int eid = LTPartnerEquipDataManager.Instance.UpLevelSelectList[i];
                int qualityValue;
                DataLookupsCache.Instance.SearchIntByID(string.Format("inventory.{0}.qualityLevel", eid), out qualityValue);
                if (qualityValue >= 3) { hasValuable = true; break; }
            }
            if (hasValuable)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipMainController_24675"), delegate (int result)
              {
                  if (result == 0)
                  {
                      RequipUpEquipment();
                  }
              });
            }
            else
            {
                RequipUpEquipment();
            }


        }

        private void RequipUpEquipment()
        {
            hasresult = true;
            List<Hashtable> costsList = new List<Hashtable>();
            Dictionary<string, int> Dics = LTPartnerEquipDataManager.Instance.getEquipUpItemNumDic();
            var it = Dics.GetEnumerator();
            while(it.MoveNext())
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("id", LTPartnerEquipDataManager.Instance.findEidByEquipUpItemId(it.Current.Key) );
                ht.Add("count", it.Current.Value);
                costsList.Add(ht);
            }
            int goldCount = BalanceResourceUtil.GetUserGold();
            LTPartnerEquipDataManager.Instance.RequireUpLevel(CurLevelupEid, LTPartnerEquipDataManager.Instance.UpLevelSelectList, costsList, delegate (bool success)
            {
                hasresult = false;
                if (success)
                {
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, BalanceResourceUtil.GetUserGold() - goldCount, "装备强化");
                    LTPartnerEquipDataManager.Instance.cleanEquipUpItemNum();
                    UpLevelPlayFX();
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                    HeroEquipmentInfo[] infos = LTPartnerEquipDataManager.Instance.CurrentPartnerData.EquipmentsInfo;
                    for (int i = 0; i < infos.Length; i++)
                    {
                        if(infos[i].Eid == CurLevelupEid)
                        {
                            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                            break;
                        }
                    }

                }
            });
        }

        public void UpLevelPlayFX()
        {
            ClearUpLevelSelect();
            OnUpdataEquipmentItemsView();
            EquipmentUplevelController.PlayFxLevelUp();
        }

        public void OnRuleBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            string text1 = EB.Localizer.GetString("ID_RULE_EQUIPMENT_VIEW");
            GlobalMenuManager.Instance.Open("LTRuleUIView", text1);
        }


        #endregion

        #region （伙伴选择相关）
        public GameObject SelectPartnerView, EquipmentsView;

        public void OnSelectPartnerOpenBtnClick()
        {
            if (CurrentEquipmentViewState == EquipmentViewState.SelectPartnerView)
            {
                return;
            }

            StartCoroutine(OpenPartnerUI());
        }
        public void OnSelectPartnerCloseBtnClick()
        {
            StartCoroutine(ClosePartnerUI());
        }

        IEnumerator OpenPartnerUI()
        {
            CurrentEquipmentViewState = EquipmentViewState.SelectPartnerView;
            EquipmentsView.GetComponent<TweenAlpha>().PlayForward();
            yield return new WaitForSeconds(EquipmentsView.GetComponent<TweenAlpha>().duration);

            var tp = HeroInfoController.mDMono.GetComponent<TweenPosition>();
            tp.to = new Vector3(650f, 0f);
            tp.onFinished.Clear();
            tp.PlayForward();

            UnEquipAllObj.CustomSetActive(false);
            yield return new WaitForSeconds(HeroInfoController.mDMono.GetComponent<TweenPosition>().duration / 2);
            SelectPartnerView.CustomSetActive(true);
            SelectPartnerView.GetComponent<TweenAlpha>().SetOnFinished(FxClipFun);
            SelectPartnerView.GetComponent<TweenAlpha>().PlayForward();
            CurPartnerGrade = curPartnerGrade;
        }

        private void FxClipFun()
        {
            EffectClip[] clips = TableScroll.mDMono.transform.GetComponentsInChildren<EffectClip>();
            for (int i = 0; i < clips.Length; i++)
            {
                clips[i].Init();
            }
        }

        IEnumerator ClosePartnerUI()
        {
            SelectPartnerView.GetComponent<TweenAlpha>().SetOnFinished(() => { });
            SelectPartnerView.GetComponent<TweenAlpha>().PlayReverse();
            HeroInfoController.mDMono.GetComponent<TweenPosition>().PlayReverse();
            yield return new WaitForSeconds(SelectPartnerView.GetComponent<TweenAlpha>().duration);
            UnEquipAllObj.CustomSetActive(true);
            SelectPartnerView.CustomSetActive(false);
            EquipmentsView.CustomSetActive(true);
            EquipmentsView.GetComponent<TweenAlpha>().PlayReverse();
            CurrentEquipmentViewState = EquipmentViewState.DefaultView;
        }

        public void OnRaceTabClick(UIToggle ui_toggle)
        {
            if (!ui_toggle.value)
                return;

            eAttrTabType tabType = ParseTabType(ui_toggle.transform.parent.name);
            RefreshPartnerList(tabType);
        }

        private eAttrTabType mCurPartnerTabType = eAttrTabType.All;

        private static LTPartnerData currentPartnerData;
        public static LTPartnerData CurrentPartnerData
        {
            get
            {
                return currentPartnerData;
            }
            set
            {
                LTPartnerEquipDataManager.Instance.CurrentPartnerData = value;
                currentPartnerData = value;
            }
        }
        private void RefreshPartnerList(eAttrTabType tab_type)
        {
            mCurPartnerTabType = tab_type;
            List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetGeneralPartnerList();
            List<LTPartnerData> filterList = new List<LTPartnerData>();
            for (var i = 0; i < partnerList.Count; i++)
            {
                var partner = partnerList[i];
                if (tab_type == eAttrTabType.All && partner.Level > 0)
                    filterList.Add(partner);
                else if (tab_type == eAttrTabType.Feng && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Feng && partner.Level > 0)
                    filterList.Add(partner);
                else if (tab_type == eAttrTabType.Huo && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Huo && partner.Level > 0)
                    filterList.Add(partner);
                else if (tab_type == eAttrTabType.Shui && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Shui && partner.Level > 0)
                    filterList.Add(partner);
            }
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
            EB.Debug.LogError("ParseTabType error str={0}", str);
            return eAttrTabType.All;
        }

        private void OnSelectFun(int e)
        {
            CurrentPartnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(e);
            HeroInfoController.Show(CurrentPartnerData);
        }


        public LTEquipPartnerScroll TableScroll;
        public List<UIButton> TypeBtnList;
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
        public UISprite SelectGradeSp;
        public GameObject SelectGradeLab;
        public TweenPosition SelectGradeTP;
        private bool IsUnfold = false;
        public void OnSelectGradeBtnClick()
        {
            PlaySelectGradeTween();
        }
        private void PlaySelectGradeTween()
        {
            if (IsUnfold)
            {
                IsUnfold = false;
                SelectGradeTP.PlayReverse();
            }
            else
            {
                IsUnfold = true;
                SelectGradeTP.PlayForward();
            }
        }


        private void SetPartnerType()
        {
            InitPartnerList();
        }
        private void InitPartnerList()
        {
            List<LTPartnerData> PartnerList = LTPartnerDataManager.Instance.GetPartnerListByGrade((int)CurPartnerGrade);
            List<LTPartnerData> newPartnerList = new List<LTPartnerData>();
            if (PartnerList != null)
            {
                for (var i = 0; i < PartnerList.Count; i++)
                {
                    var data = PartnerList[i];

                    if (data.Level > 0) 
                    {
                        newPartnerList.Add(data); 
                    }
                }
            }
            partnerDataList = newPartnerList;
            for (int i = partnerDataList.Count; i < 16; i++)
            {
                partnerDataList.Add(new LTPartnerData());
            }
            TableScroll.SetItemDatas(partnerDataList);
        }
        private void SetSelectGradeState()
        {
            SelectGradeSp.gameObject.CustomSetActive(CurPartnerGrade != PartnerGrade.ALL);
            SelectGradeLab.CustomSetActive(CurPartnerGrade == PartnerGrade.ALL);
            if (CurPartnerGrade != PartnerGrade.ALL)
            {
                SelectGradeSp.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[CurPartnerGrade];
            }
        }
        public void OnPartnerTypeBtnClick(UIButton btn)
        {
            int index = TypeBtnList.IndexOf(btn);

            if (CurPartnerGrade == (PartnerGrade)index)
            {
                PlaySelectGradeTween();
                return;
            }
            StartCoroutine(ChangePartnerType(index));
        }
        private List<LTPartnerData> partnerDataList;
        IEnumerator ChangePartnerType(int index)
        {
            CurPartnerGrade = (PartnerGrade)index;
            PlaySelectGradeTween();
            yield return null;

            /*if (partnerDataList.Count > 0)
            {
                OnPartnerSelect evt = new OnPartnerSelect(partnerDataList[0].StatId);
                EventManager.instance.Raise(evt);
            }
            else
            {
                OnPartnerSelect evt = new OnPartnerSelect(0);
                EventManager.instance.Raise(evt);
            }*/
            yield break;
        }
        #endregion

        #region（装备合成相关）

        public LTPartnerEquipCellController LeftSynEquipCell;
        public LTPartnerEquipCellController RightSynEquipCell;
        public LTEquipmentSynPreViewController PreviewEquipCell;
        public UILabel MainAttrLabel;
        public GameObject EquipSynView;
        public UILabel StoneNum;
        public UILabel LeftEquipName;
        public UILabel RightEquipName;
        public static BaseEquipmentInfo[] EquipSynArray = new BaseEquipmentInfo[2];
        public static BaseEquipmentInfo currSynEquip;
        private int stonenum;
        public ParticleSystem EquipsynFx;
        /// <summary>
        /// 点击合成界面返回按钮
        /// </summary>
        private void OnEquipSynCancleClick(int syneid)
        {
            CurrentEquipmentViewState = EquipmentViewState.LevelView;
            EquipView_SuitBtn.CustomSetActive(false);
            EquipView_SortBtn.CustomSetActive(false);
            ResetFastSelectList();//EquipView_FastSelectList.CustomSetActive(true);
            EquipSynArray[0] = null;
            EquipSynArray[1] = null;
            StartCoroutine(OnCloseEquipSynView());
            UplevelViewLabel.text = UplevelViewLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTPartnerEquipmentHud_Label_9");
            ReflashLeftSynUI();
            OnEquipLevelUp(syneid);
            OnUpdataEquipmentItemsView();
        }
        int EquipSyneid;
        /// <summary>
        /// 刷新合成界面右侧材料显示
        /// </summary>
        private void UpdateRightSynEquip()
        {
            ListData = LTPartnerEquipDataManager.Instance.GetSynEquipList();
            CurrentDatas = new List<BaseEquipmentInfo>();
            List<BaseEquipmentInfo> SynMaterialList = new List<BaseEquipmentInfo>();
            int equipCount = 0;
            int eid1 = EquipSynArray[0] == null ? -1 : EquipSynArray[0].Eid;
            int eid2 = EquipSynArray[1] == null ? -1 : EquipSynArray[1].Eid;
            EquipPartType equipPartType = EquipPartType.none;
            if (EquipSynArray[0] == null && EquipSynArray[1] == null)
            {
                equipPartType = EquipPartType.none;
            }
            else if (EquipSynArray[0] == null && EquipSynArray[1] != null)
            {
                equipPartType = EquipSynArray[1].Type;
            }
            else if (EquipSynArray[0] != null && EquipSynArray[1] == null)
            {
                equipPartType = EquipSynArray[0].Type;
            }
            else
            {
                equipPartType = EquipSynArray[0].Type;
            }
            for (int i = 0; i < ListData.Count; i++)
            {
                bool isSelectSuitType = (LTPartnerEquipDataManager.Instance.CurSuitType == -1) ? true : ListData[i].SuitType == LTPartnerEquipDataManager.Instance.CurSuitType;


                if (ListData[i].Eid == eid1 || ListData[i].Eid == eid2)
                {
                    continue;
                }
                else
                {
                    if (isSelectSuitType && (ListData[i].Type == equipPartType || equipPartType == EquipPartType.none))
                    {
                        equipCount++;
                        CurrentDatas.Add(ListData[i]);
                    }
                }
            }
            if ((eid1 == -1 && eid2 != -1) || (eid1 != -1 && eid2 == -1) || (eid1 != -1 && eid2 != -1))
            {
                int suittype = eid1 == -1 ? EquipSynArray[1].SuitType : EquipSynArray[0].SuitType;
                CurrentDatas.Sort((a, b) =>
                {

                    if (a.EquipLevel > b.EquipLevel)
                        return -1;
                    else if (a.EquipLevel < b.EquipLevel)
                        return 1;
                    else if (Mathf.Abs(a.SuitType - suittype) > Mathf.Abs(b.SuitType - suittype))
                        return 1;
                    else if (Mathf.Abs(a.SuitType - suittype) < Mathf.Abs(b.SuitType - suittype))
                        return -1;
                    else if (a.Eid > b.Eid)
                        return -1;
                    else
                        return 1;
                });
            }
            else
            {
                CurrentDatas.Sort((a, b) =>
                {

                    if (a.EquipLevel > b.EquipLevel)
                        return -1;
                    else if (a.EquipLevel < b.EquipLevel)
                        return 1;
                    else if ((int)a.Type < (int)b.Type)
                        return -1;
                    else if ((int)a.Type > (int)b.Type)
                        return 1;
                    else if (a.SuitType > b.SuitType)
                        return 1;
                    else if (a.SuitType < b.SuitType)
                        return -1;
                    else if (a.Eid > b.Eid)
                        return -1;
                    else
                        return 1;
                });
            }
            for (int i = 0; i < ListData.Count; i++)
            {
                if (ListData[i].Type == equipPartType || equipPartType == EquipPartType.none)
                {
                    SynMaterialList.Add(ListData[i]);
                }
            }
            LTPartnerEquipDataManager.Instance.ReflashSynSuitInfo(SynMaterialList);
            int extraAdd = LTPartnerConfig.Equip_BASE_MAX_VALUE + VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.InvEquipCount);
            string CountStr = equipCount > extraAdd ? "[ff6699]"+ extraAdd : extraAdd.ToString();
            EquipmentCountLabel.text = EquipmentCountLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format("[42fe79]{0}[-]/{1}", equipCount, CountStr);//装备显示数量
            for (int i = CurrentDatas.Count; i < 25; i++) CurrentDatas.Add(null);
			_partnerEquipScroll.SetItemDatas(CurrentDatas);

        }
        /// <summary>
        /// 点击装备合成界面
        /// </summary>
        public void OnEquipSynthesisBtnCliclk()
        {
            CurrentEquipmentViewState = EquipmentViewState.SynthesisView;
            FusionAudio.PostEvent("UI/General/ButtonClick");
            UplevelViewLabel.text = UplevelViewLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_EQUIP_SYN_SYNSELECT");
            //打开装备合成界面
            EquipView_SuitBtn.CustomSetActive(true);
            EquipView_FastSelectList.CustomSetActive(false);
            EquipView_SortBtn.CustomSetActive(false);
            InitStoneState();
            StartCoroutine(OnOpenEquipSynView());
            currSynEquip = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(CurLevelupEid);
            EquipSyneid = CurLevelupEid;
            EquipSynArray[0] = currSynEquip;
            LeftSynEquipCell.Fill(EquipSynArray[0]);
            ClearUpLevelSelect();
            //刷新右边装备
            UpdateRightSynEquip();
            ReflashPreviewEquipment();
        }
        /// <summary>
        /// 刷新锻造石状态
        /// </summary>
        private void InitStoneState()
        {
            stonenum = GameItemUtil.GetInventoryItemNum(1425);
            string colorStr = stonenum >= LTPartnerEquipDataManager.Instance.EquipSynConsume ? "[42fe79]" : "[ff6699]";
            StoneNum.text = string.Format("{0}{1}/{2}{3}", colorStr, stonenum, "[FFFFFF]", LTPartnerEquipDataManager.Instance.EquipSynConsume);
        }
        public void OnclickStone()
        {
            UITooltipManager.Instance.DisplayTooltipSrc("1425", "Generic", "default");

        }

        /// <summary>
        /// 打开装备合成界面
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnOpenEquipSynView()
        {
            EquipmentUplevelController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
            EquipmentUplevelController.mDMono.GetComponent<TweenPosition>().PlayReverse();
            EquipSynView.GetComponent<TweenPosition>().PlayForward();
            EquipSynView.GetComponent<TweenAlpha>().PlayForward();
            TweenPosition tween = EquipView_SuitBtn.GetComponent<TweenPosition>();
            tween.from = new Vector3(130, 0, 0);
            tween.to = new Vector3(410, 0, 0);
            EquipView_SuitBtn.GetComponent<TweenPosition>().PlayForward();
            yield break;
        }

        /// <summary>
        /// 从装备合成界面返回装备强化界面
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnCloseEquipSynView()
        {
            EquipSynView.GetComponent<TweenPosition>().PlayReverse();
            EquipSynView.GetComponent<TweenAlpha>().PlayReverse();
            EquipView_SuitBtn.GetComponent<TweenPosition>().PlayReverse();
            yield break;
        }

        /// <summary>
        /// 装备合成时点击右侧材料装备和左侧装备时出现的状态
        /// </summary>
        /// <param name="eId"></param>
        private void OnEquipSynthesisSelect(int eId, bool isEnter, Action callback)
        {
            //填充右侧合成位
            DetailedEquipmentInfo equipInfo = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eId);
            if (equipInfo == null)
            {
                EB.Debug.LogError("LTPartnerEquipMainController.OnEquipSynthesisSelect equipInfo is null");
                return;
            }
            if (isEnter)
            {

                if (equipInfo.EquipLevel >= 15)
                {

                    if (EquipSynArray[0] != null)
                    {
                        if (equipInfo.Type == EquipSynArray[0].Type && equipInfo.SuitType == EquipSynArray[0].SuitType)
                        {
                            EquipSynArray[1] = equipInfo;
                            RightSynEquipCell.Fill(EquipSynArray[1]);

                        }
                        else
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_SYN_INTOERRORTIP"), null);
                        }
                    }
                    else if (EquipSynArray[0] == null && EquipSynArray[1] == null)
                    {
                        EquipSynArray[0] = equipInfo;
                        LeftSynEquipCell.Fill(EquipSynArray[0]);
                    }
                    else if (EquipSynArray[0] == null && EquipSynArray[1] != null)
                    {
                        if (equipInfo.Type == EquipSynArray[1].Type && equipInfo.SuitType == EquipSynArray[1].SuitType)
                        {
                            EquipSynArray[0] = equipInfo;
                            LeftSynEquipCell.Fill(EquipSynArray[0]);
                        }
                        else
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_SYN_INTOERRORTIP"), null);
                        }
                    }

                    UpdateRightSynEquip();
                    ReflashPreviewEquipment();

                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_SYN_INTOERRORTIP"), null);
                }

            }
            else
            {
                for (int i = 0; i < EquipSynArray.Length; i++)
                {
                    if (EquipSynArray[i] != null && EquipSynArray[i].Eid == eId)
                    {
                        EquipSynArray[i] = null;
                        if (i == 0)
                        {

                            LeftSynEquipCell.Clean();
                        }
                        if (i == 1)
                        {

                            RightSynEquipCell.Clean();
                        }
                        //有没有回复初始状态
                        UpdateRightSynEquip();

                    }
                }
                ReflashPreviewEquipment();
            }
            callback();
        }
        public void OnEquipSynRuleBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            string text1 = EB.Localizer.GetString("ID_EQUIP_SYN_RULE_TEXT");//装备合成规则
            GlobalMenuManager.Instance.Open("LTRuleUIView", text1);
        }
        private void ReflashLeftSynUI()
        {
            for (int i = 0; i < EquipSynArray.Length; i++)
            {
                if (EquipSynArray[i] == null)
                {
                    if (i == 0)
                    {

                        LeftSynEquipCell.Clean();
                    }
                    if (i == 1)
                    {

                        RightSynEquipCell.Clean();
                    }
                    //有没有回复初始状态

                }
                else
                {
                    if (i == 0)
                    {
                        LeftSynEquipCell.Data = null;
                        EquipSynArray[0] = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(EquipSynArray[0].Eid);
                        if (EquipSynArray[0] == null)
                        {
                            EB.Debug.LogError("LTPartnerEquipMainController.ReflashLeftSynUI EquipSynArray[0] is null");
                            continue;
                        }
                        LeftSynEquipCell.Fill(EquipSynArray[0]);
                    }
                    if (i == 1)
                    {
                        RightSynEquipCell.Data = null;
                        EquipSynArray[1] = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(EquipSynArray[1].Eid);
                        if (EquipSynArray[1] == null)
                        {
                            EB.Debug.LogError("LTPartnerEquipMainController.ReflashLeftSynUI EquipSynArray[1] is null");
                            continue;
                        }
                        RightSynEquipCell.Fill(EquipSynArray[1]);
                    }
                }
            }
            UpdateRightSynEquip();
        }
        /// <summary>
        /// 刷新预览装备槽
        /// </summary>
        private void ReflashPreviewEquipment()
        {

            if (EquipSynArray[0] == null && EquipSynArray[1] == null)
            {

                PreviewEquipCell.Clean();
                MainAttrLabel.text = "[FFFFFF]" + EB.Localizer.GetString("ID_EQUIP_SYN_SYNTIP");
                FillMaterialCellName();
                leftSynCellBg.CustomSetActive(true);
                rightSynCellBg.CustomSetActive(true);
            }
            else if (EquipSynArray[0] != null && EquipSynArray[1] != null)
            {

                PreviewCellFill(EquipSynArray[0]);
                FillMaterialCellName();
                string equip1MainAttrname = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(EquipSynArray[0].Eid)?.MainAttributes.Name;
                string equip2MainAttrname = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(EquipSynArray[1].Eid)?.MainAttributes.Name;
                //判断是否随机
                if (equip1MainAttrname.Equals(equip2MainAttrname))
                {

                    MainAttrLabel.text = "[42fe79]" + EB.Localizer.GetString("ID_uifont_in_LTChallengeInstanceBag_Desc_6") + "   " + AttrTypeTrans(equip1MainAttrname);
                }
                else
                {
                    MainAttrLabel.text = "[42fe79]" + EB.Localizer.GetString("ID_uifont_in_LTChallengeInstanceBag_Desc_6") + "   " + EB.Localizer.GetString("ID_RANDOM");
                }
                leftSynCellBg.CustomSetActive(false);
                rightSynCellBg.CustomSetActive(false);

            }
            else
            {
                BaseEquipmentInfo info = EquipSynArray[0] != null ? EquipSynArray[0] : EquipSynArray[1];
                PreviewCellFill(info);
                FillMaterialCellName();
                MainAttrLabel.text = "[42fe79]" + EB.Localizer.GetString("ID_uifont_in_LTChallengeInstanceBag_Desc_6") + "   " + EB.Localizer.GetString("ID_RANDOM");
                leftSynCellBg.CustomSetActive(EquipSynArray[0] == null);
                rightSynCellBg.CustomSetActive(EquipSynArray[1] == null);
            }
        }


        private string AttrTypeTrans(string str)
        {
            switch (str)
            {
                case "ATK": return EB.Localizer.GetString("ID_ATTR_ATK");
                case "MaxHP": return EB.Localizer.GetString("ID_ATTR_MaxHP");
                case "DEF": return EB.Localizer.GetString("ID_ATTR_DEF");
                case "CritP": return EB.Localizer.GetString("ID_ATTR_CritP");
                case "CritV": return EB.Localizer.GetString("ID_ATTR_CritV");
                case "ChainAtk": return EB.Localizer.GetString("ID_ATTR_ChainAtk");
                case "SpExtra": return EB.Localizer.GetString("ID_ATTR_SpExtra");
                case "SpRes": return EB.Localizer.GetString("ID_ATTR_SpRes");
                case "MaxHPrate": return EB.Localizer.GetString("ID_ATTR_MaxHPrate");
                case "ATKrate": return EB.Localizer.GetString("ID_ATTR_ATKrate");
                case "DEFrate": return EB.Localizer.GetString("ID_ATTR_DEFrate");
                case "Speed": return EB.Localizer.GetString("ID_ATTR_Speed");
                case "speedrate": return EB.Localizer.GetString("ID_ATTR_speedrate");
                default: return EB.Localizer.GetString("ID_ATTR_Unknown");
            }
        }
        /// <summary>
        /// 填充装备材料栏名称显示
        /// </summary>
        private void FillMaterialCellName()
        {

            if (EquipSynArray[0] != null)
            {

                LeftEquipName.text = "[42fe79]" + LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(EquipSynArray[0].Eid)?.Name;
            }
            else
            {
                LeftEquipName.text = "[FFFFFF]" + EB.Localizer.GetString("ID_EQUIP_SYN_INTOTIP");
            }
            if (EquipSynArray[1] != null)
            {

                RightEquipName.text = "[42fe79]" + LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(EquipSynArray[1].Eid)?.Name;
            }
            else
            {
                RightEquipName.text = "[FFFFFF]" + EB.Localizer.GetString("ID_EQUIP_SYN_INTOTIP");
            }
        }
        private void PreviewCellFill(BaseEquipmentInfo info)
        {

            int ecid = LTPartnerEquipDataManager.GetLegendEquipmentEcidByPartSuit(info.Type, info.SuitType);
            string iconName = Hotfix_LT.Data.EconemyTemplateManager.GetItemIcon(ecid.ToString());
            int QualityLevel = 7;
            string suitIcon = info.SuitIcon;
            PreviewEquipCell.Fill(iconName, suitIcon, QualityLevel);
        }

        /// <summary>
        /// 点击装备合成按钮
        /// </summary>
        /// 

        public void OnClickEquipSynBtn()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick", true);
            if (stonenum < LTPartnerEquipDataManager.Instance.EquipSynConsume)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIO_SYN_COSTNOTENOUGH"), null);
                return;
            }
            if (EquipSynArray[0] == null || EquipSynArray[1] == null)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_SYN_SYNSELECT"), null);
                return;
            }
            else
            {
                InitStoneState();
                if (EquipSynArray[0].isLock || EquipSynArray[1].isLock)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_EQUIP_SYN_LOCKTIP"), delegate (int r)
                    {
                        if (r == 0)
                        {
                            RequestSynSolve();
                        }
                    });
                    return;
                }
                RequestSynSolve();
            }

        }

        private void RequestSynSolve()
        {
            if (LTPartnerEquipDataManager.Instance.isMaxEquipNum)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                return;
            }
            if (!EquipSynArray[0].isDress && !EquipSynArray[1].isDress)
            {
                LTPartnerEquipDataManager.Instance.RequireEquipSyn(EquipSynArray[0].Eid, EquipSynArray[1].Eid, SynCallBack);
            }
            else
            {
                List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
                Dictionary<int, int> EquipHeroid = new Dictionary<int, int>();//需要卸载的装备
                Dictionary<int, int> DressEquipNum = new Dictionary<int, int>();
                LoadingSpinner.Show();
                for (int i = 0; i < EquipSynArray.Length; i++)
                {
                    if (EquipSynArray[i].isDress)
                    {
                        EquipHeroid[EquipSynArray[i].Eid] = 0;
                        DressEquipNum[EquipSynArray[i].Eid] = 0;
                    }
                }
                if (DressEquipNum.Count > 0)
                {


                    for (int i = 0; i < partnerList.Count; i++)
                    {
                        int tempeid = partnerList[i].GetEquipmentsInfo((int)EquipSynArray[0].Type - 1).Eid;
                        if (DressEquipNum.ContainsKey(tempeid))
                        {
                            DressEquipNum.Remove(tempeid);
                            EquipHeroid[tempeid] = partnerList[i].HeroId;                           
                        }
                        if (DressEquipNum.Count == 0)
                        {
                            break;
                        }

                    }
                    LTPartnerData partnertemp;
                    foreach (var item in EquipHeroid)
                    {
                        LTPartnerEquipDataManager.Instance.RequireUnEquip(item.Key, item.Value, delegate {
                            partnertemp = LTPartnerDataManager.Instance.GetPartnerByHeroId(item.Value);
                            partnertemp.powerData.OnValueChanged(partnertemp, false, PowerData.RefreshType.EquipsuitSkill);
                            LTFormationDataManager.OnRefreshMainTeamPower(false);                       
                        });
                    }

                }
                LTPartnerEquipDataManager.Instance.RequireEquipSyn(EquipSynArray[0].Eid, EquipSynArray[1].Eid, SynCallBack);
                LoadingSpinner.Hide();
            }

        }

        private void SynCallBack(bool isccess, Hashtable result)
        {
            if (isccess)
            {

                double Equipeid;
                int equipmenteid = -1;
                Equipeid = result["combineEquipId"] == null ? -1 : (double)result["combineEquipId"];
                equipmenteid = (int)Equipeid;
                if (equipmenteid == -1)
                {
                    return;
                }
                EquipSynArray[0] = null;
                EquipSynArray[1] = null;
                StartCoroutine(PlaySynFx(equipmenteid));
                FusionAudio.PostEvent("UI/New/XuanCai", true);
                EquipSyneid = equipmenteid;
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerSelect, LTPartnerDataManager.Instance.DropSelectPartnerId);
            }
        }

        private IEnumerator PlaySynFx(int equipmenteid)
        {
            EquipsynFx.gameObject.CustomSetActive(true);
            InputBlockerManager.Instance.Block(InputBlockReason.SCREEN_TRANSITION_MASK, 2.0f);
            yield return new WaitForSeconds(2.0f);
            ReflashLeftSynUI();
            UpdateRightSynEquip();
            InitStoneState();
            ReflashPreviewEquipment();
            DetailedEquipmentInfo equipInfo = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(equipmenteid);
            GlobalMenuManager.Instance.Open("LTShowEquipRewardView", equipInfo);
            GameUtils.ShowAwardMsgOnlySys(new LTShowItemData(equipInfo.ECid.ToString(), 1, "inventory"));
            EquipsynFx.gameObject.CustomSetActive(false);
            yield break;
        }
        #endregion

        #region 装备预设
        private GameObject _openEquipmentPresetBtn;
        private GameObject _saveEquipmentPresetBtn;
        private UIButton _btnSaveScheme;
        private LTPartnerEquipmentInfoController _equipmentInfoController;
        private LTPartnerEquipmentPresetController _equipmentPresetController;

        public void EnableSaveSchemeBtn(bool isEnabled)
        {
            _btnSaveScheme.isEnabled = isEnabled;
        }

        private void OnClickSaveButton()
        {
            _equipmentInfoController.OnSaveEdit();          
        }

        public void OnClickEquipmentInfoItem()
        {
            if (_equipmentPresetController.CurrentEquipmentPresetItem.IsCurrentItem)
            {
                CloseEquipmentPresetView();
            }
            else
            {
                OpenEquipmentPresetEditView();
                _equipmentInfoController.OnEnterEdit(_equipmentPresetController.CurrentEquipmentPresetItem.KVP.Key, _equipmentPresetController.CurrentEquipmentPresetItem.KVP.Value.eids);
            }
        }

        public void OpenEquipmentPresetView()
        {
            StartCoroutine(OpenEquipmentPresetViewAsync());
        }

        private IEnumerator OpenEquipmentPresetViewAsync()
        {
            UnEquipAllObj.SetActive(false);
            _openEquipmentPresetBtn.SetActive(false);
            _equipmentInfoController.mDMono.gameObject.SetActive(true);
            var tp = HeroInfoController.mDMono.GetComponent<TweenPosition>();
            tp.to = new Vector3(-2100f, 0f);
            tp.PlayForward();
            HeroInfoController.mDMono.GetComponent<TweenAlpha>().PlayForward();
            EquipmentsView.GetComponent<TweenPosition>().PlayForward();
            EquipmentsView.GetComponent<TweenAlpha>().PlayForward();
            _equipmentInfoController.mDMono.GetComponent<TweenPosition>().PlayForward();
            _equipmentInfoController.mDMono.GetComponent<TweenAlpha>().PlayForward();
            _equipmentInfoController.Show(CurrentPartnerData.EquipmentsInfo);
            _equipmentPresetController.mDMono.GetComponent<TweenPosition>().PlayForward();
            _equipmentPresetController.mDMono.GetComponent<TweenAlpha>().PlayForward();
            CurrentEquipmentViewState = EquipmentViewState.PresetView;
            _equipmentPresetController.RefreshData();

            yield return new WaitForSeconds(tp.duration);
            HeroInfoController.mDMono.gameObject.SetActive(false);
        }

        public void CloseEquipmentPresetView()
        {
            StartCoroutine(CloseEquipmentPresetViewAsync());
        }

        private IEnumerator CloseEquipmentPresetViewAsync()
        {
            HeroInfoController.mDMono.gameObject.SetActive(true);
            var tp = HeroInfoController.mDMono.GetComponent<TweenPosition>();
            tp.onFinished.Add(new EventDelegate(() => UnEquipAllObj.SetActive(true)));
            tp.PlayReverse();
            HeroInfoController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
            HeroInfoController.ShowAttr();
            EquipmentsView.GetComponent<TweenPosition>().PlayReverse();
            EquipmentsView.GetComponent<TweenAlpha>().PlayReverse();
            _equipmentInfoController.mDMono.GetComponent<TweenPosition>().PlayReverse();
            _equipmentInfoController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
            _equipmentPresetController.mDMono.GetComponent<TweenPosition>().PlayReverse();
            _equipmentPresetController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
            CurrentEquipmentViewState = EquipmentViewState.DefaultView;
            _openEquipmentPresetBtn.SetActive(true);  
            OnUpdataEquipmentItemsView();

            yield return new WaitForSeconds(tp.duration);
            _equipmentInfoController.mDMono.gameObject.SetActive(false);
        }

        public void OpenEquipmentPresetEditView()
        {
            _equipmentPresetController.mDMono.GetComponent<TweenPosition>().PlayReverse();
            _equipmentPresetController.mDMono.GetComponent<TweenAlpha>().PlayReverse();
            EquipmentsView.GetComponent<TweenPosition>().PlayReverse();
            EquipmentsView.GetComponent<TweenAlpha>().PlayReverse();
            _saveEquipmentPresetBtn.SetActive(true);
            CurrentEquipmentViewState = EquipmentViewState.PresetEditView;
            _equipmentInfoController.ShowCompareInfo(false);
            OnUpdataEquipmentItemsView(); 
        }

        public void CloseEquipmentPresetEditView()
        {
            _equipmentPresetController.mDMono.GetComponent<TweenPosition>().PlayForward();
            _equipmentPresetController.mDMono.GetComponent<TweenAlpha>().PlayForward();
            EquipmentsView.GetComponent<TweenPosition>().PlayForward();
            EquipmentsView.GetComponent<TweenAlpha>().PlayForward();
            _saveEquipmentPresetBtn.SetActive(false);
            CurrentEquipmentViewState = EquipmentViewState.PresetView;
            _equipmentInfoController.ShowCompareInfo(_equipmentPresetController.EquippedPresetItem != _equipmentPresetController.CurrentEquipmentPresetItem);
        }

        public void RemoveFromEquipmentInfoList(int pos)
        {
            _equipmentInfoController.RemoveFromEquipmentInfoList(pos);
            UpdataEquipmentItemsView();
        }

        public void AddToEquipmentInfoList(int pos, int eid)
        {
            _equipmentInfoController.AddToEquipmentInfoList(pos, eid);
            UpdataEquipmentItemsView();
        }
        #endregion
    }
}
