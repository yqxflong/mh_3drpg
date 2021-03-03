using UnityEngine;
using System.Collections;
using System;
using Hotfix_LT.Player;
using LT.Hotfix.Utility;

namespace Hotfix_LT.UI
{
    public class SelectEquipEven
    {
        public static Action ChooseEquipment;
        public static Action<int> SelectEquipment;
        public static Action<int> LockBtnClick;
        public static Action<int, bool, Action> SelectSynEquipment;
    }


    public class LTPartnerEquipCellController : DynamicCellController<BaseEquipmentInfo>, _HotfixScripts.Utils.IHotfixUpdate
    {
        public DynamicUISprite EquipmentIcon, SuitIcon;
        public GameObject SelectBG, LockBG, ChooseObj, BreakItemObj, RemoveObj, ItemMask, DressBG;
        public UISprite BGIcon, BG2Icon;
        public UILabel CountLabel;
        public UILabel LevelLabel;
        public BaseEquipmentInfo Data;
        public bool IsSelect;

        private bool isEquipUpItem = false;
        private UISprite _tipsBackground;
        private UILabel _tipsLabel;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            EquipmentIcon = t.GetComponent<DynamicUISprite>("EquipmentItem/IMG");
            SuitIcon = t.GetComponent<DynamicUISprite>("EquipmentItem/listCellBG");
            BGIcon = t.GetComponent<UISprite>("EquipmentItem/IMG/LvlBorder");
            BG2Icon = t.GetComponent<UISprite>("EquipmentItem/IMG/LvlBorder/Bg");
            _tipsBackground = t.GetComponent<UISprite>("Tips", false);
            _tipsLabel = t.GetComponent<UILabel>("Tips/Label", false);

            if (t.Find("EquipmentItem/SelectUI") != null) { 
                SelectBG = t.Find("EquipmentItem/SelectUI").gameObject; 
            }

            if (t.Find("EquipmentItem/ChooseUI") != null) {
                ChooseObj = t.Find("EquipmentItem/ChooseUI").gameObject; 
            }

            LockBG = t.Find("EquipmentItem/LockSprite").gameObject;

            if (t.Find("EquipmentItem/ItemObj") != null) {
                BreakItemObj = t.Find("EquipmentItem/ItemObj").gameObject; 
            }

            if (t.Find("EquipmentItem/ItemObj/RemoveBtn") != null) {
                RemoveObj = t.Find("EquipmentItem/ItemObj/RemoveBtn").gameObject;
            }

            if (t.Find("EquipmentItem/MaskUI") != null) {
                ItemMask = t.Find("EquipmentItem/MaskUI").gameObject;
            }

            if (t.Find("EquipmentItem/DressSprite") != null) {
                DressBG = t.Find("EquipmentItem/DressSprite").gameObject;
            }

            if (t.Find("EquipmentItem/ItemObj/Num") != null) {
                CountLabel = t.GetComponent<UILabel>("EquipmentItem/ItemObj/Num");
            }

            IsSelect = false;
            LevelLabel = t.GetComponent<UILabel>("EquipmentItem/LevelLabel");
            var trigger1 = t.GetComponent<UIEventTrigger>();

            if (trigger1 != null)
            {
                trigger1.onPress.Add(new EventDelegate(OnItemBtnPress));
                trigger1.onRelease.Add(new EventDelegate(OnItemBtnRelease));
                trigger1.onClick.Add(new EventDelegate(OnBagItemClick));
            }

            var trigger2 = t.GetComponent<UIEventTrigger>("EquipmentItem/ItemObj/RemoveBtn", false);

            if (trigger2 != null)
            {
                trigger2.onPress.Add(new EventDelegate(OnRemoveBtnPress));
                trigger2.onRelease.Add(new EventDelegate(OnRemoveBtnRelease));
                trigger2.onClick.Add(new EventDelegate(OnRemoveBtnClick));
            }
        }
        
        public override void OnEnable()
        {
            SelectEquipEven.SelectEquipment += OnSelectEquip;
            SelectEquipEven.LockBtnClick += OnUnChooseEquip;
            //RegisterMonoUpdater();
        }

        public override void OnDisable()
        {
            ErasureMonoUpdater();
            SelectEquipEven.SelectEquipment -= OnSelectEquip;
            SelectEquipEven.LockBtnClick -= OnUnChooseEquip;
        }

        public override void Clean()
        {
            Data = null;
            if (Data == null || Data.Type != LTPartnerEquipDataManager.Instance.CurType)
            {
                IsSelect = false;
                isEquipUpItem = false;
                mDMono.transform.GetChild(0).gameObject.CustomSetActive(false);

                if (_tipsBackground != null) {
                    _tipsBackground.gameObject.SetActive(false);  
                }

                return;
            }
        }

        public override void Fill(BaseEquipmentInfo itemData)
        {
            if (itemData == null || string.IsNullOrEmpty(itemData.ECid))
            {
                Data = null;
                mDMono.transform.GetChild(0).gameObject.CustomSetActive(false);
                RefreshTips();
                return;
            }
            else
            {
                if (Data == itemData)
                {
                    if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.LevelView)
                    {
                        if (LTPartnerEquipDataManager.Instance.UpLevelSelectList.Contains(Data.Eid))
                        {
                            IsSelect = true;
                            SelectBG.CustomSetActive(true);
                        }
                        else
                        {
                            IsSelect = false;
                            SelectBG.CustomSetActive(false);
                        }
                        if (isEquipUpItem)
                        {
                            delGoodsCount = LTPartnerEquipDataManager.Instance.getEquipUpItemNum(Data.ECid);
                            ShowGoodsCount();
                            BreakItemObj.CustomSetActive(true);
                        }
                        else
                        {
                            ItemMask.CustomSetActive(false);
                            BreakItemObj.CustomSetActive(false);
                        }
                    }

                    RefreshTips();
                    return;
                }

                Data = itemData;
                isEquipUpItem = LTPartnerEquipDataManager.Instance.isEquipUpItem(Data.ECid);
                mDMono.transform.GetChild(0).gameObject.CustomSetActive(true);

                if (LTPartnerEquipMainController.instance!=null&&LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.LevelView &&
                    LTPartnerEquipDataManager.Instance.UpLevelSelectList.Contains(Data.Eid))
                {
                    IsSelect = true;
                    SelectBG.CustomSetActive(true);
                }
                else
                {
                    IsSelect = false;
                    SelectBG.CustomSetActive(false);
                }

                if (LockBG != null)
                {
                    LockBG.CustomSetActive(Data.isLock);
                }

                if (LTPartnerEquipMainController.instance != null && LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.SynthesisView)
                {
                    if (DressBG != null)
                    {
                        DressBG.CustomSetActive(Data.isDress);
                    }
                }
                else
                {
                    DressBG.CustomSetActive(false);
                }
            }
            EquipmentIcon.spriteName = Data.IconName;
            BGIcon.spriteName = UIItemLvlDataLookup.LvlToStr(Data.QualityLevel.ToString());
            if (Data.QualityLevel == 7)
            {
                BG2Icon.spriteName = "Ty_Quality_Xuancai_Di";
            }
            else
            {
                BG2Icon.spriteName = "Ty_Di_2";
            }
            BG2Icon.color = LT.Hotfix.Utility.ColorUtility.QualityToFrameColor(Data.QualityLevel);
            SuitIcon.spriteName = Data.SuitIcon;
            if (LevelLabel == null)
            {
                EB.Debug.LogError("LevelLabel Null");
            }
            LevelLabel.transform.GetChild(0).GetComponent<UISprite>().spriteName = UIItemLvlDataLookup.GetEquipLevelBGStr(Data.QualityLevel);
            if (Data.EquipLevel > 0)
            {
                LevelLabel.text = string.Format("+{0}", Data.EquipLevel);
                LevelLabel.gameObject.CustomSetActive(true);
            }
            else
            {
                LevelLabel.gameObject.CustomSetActive(false);
            }
            if (isEquipUpItem)
            {
                delGoodsCount = LTPartnerEquipDataManager.Instance.getEquipUpItemNum(Data.ECid);
                ShowGoodsCount();
                BreakItemObj.CustomSetActive(true);
            }
            else
            {
                ItemMask.CustomSetActive(false);
                BreakItemObj.CustomSetActive(false);
            }

            RefreshTips();
        }

        private void RefreshTips() {
            if (_tipsBackground == null) {
                return;            
            }

            if (Data != null && LTPartnerEquipMainController.instance != null
                && LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView
                && _tipsBackground != null) {
                bool isEquiped = InventoryUtility.IsEquipped(Data.Eid); 
                bool isInPreset = InventoryUtility.IsInEquipmentPreset(Data.Eid);
                _tipsBackground.gameObject.SetActive(isEquiped || isInPreset);
                _tipsBackground.spriteName = isEquiped ? "Equipment_Di_2" : "Equipment_Di_1";
                _tipsLabel.text = isEquiped ? EB.Localizer.GetString("ID_EQUIPMENT_PRESET_DRESSED") : EB.Localizer.GetString("ID_EQUIPMENT_PRESET_USED");
            } else {
                _tipsBackground.gameObject.SetActive(false);
            }
        }

        public void OnBagItemClick() {
            FusionAudio.PostEvent("UI/General/ButtonClick");

            if (Data == null || string.IsNullOrEmpty(Data.ECid)) {
                return;
            }

            if (Data.Eid == 0 && !string.IsNullOrEmpty(Data.ECid)) {
                UITooltipManager.Instance.DisplayTooltipSrc(Data.ECid, EconomyConstants.System.GENERIC, "default");
                return;
            }

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.LevelView) {
                if (LTPartnerEquipMainController.CurLevelupEid == Data.Eid) return;

                if (isEquipUpItem) {
                    OnItemBtnClick();
                    return;
                }

                if (JudgeExp()) {
                    return;
                }

                if (Data.isLock) {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("fromType", 2);
                    ht.Add("eid", Data.Eid);
                    ht.Add("pos", 1);
                    GlobalMenuManager.Instance.Open("LTPartnerEquipmentInfoUI", ht);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipCellController_3844"));
                    Johny.HashtablePool.Release(ht);
                    return;
                }

                IsSelect = !IsSelect;
                SelectBG.CustomSetActive(IsSelect);

                if (IsSelect) {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("fromType", 2);
                    ht.Add("eid", Data.Eid);
                    ht.Add("pos", 1);
                    GlobalMenuManager.Instance.Open("LTPartnerEquipmentInfoUI", ht);

                    if (LTPartnerEquipDataManager.Instance.UpLevelSelectList.Contains(Data.Eid)) {
                        return;
                    }

                    LTPartnerEquipDataManager.Instance.UpLevelSelectList.Add(Data.Eid);
                } else if (LTPartnerEquipDataManager.Instance.UpLevelSelectList.Contains(Data.Eid)) {
                    LTPartnerEquipDataManager.Instance.UpLevelSelectList.Remove(Data.Eid);
                }

                SelectEquipEven.ChooseEquipment();
                return;
            }

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.SynthesisView) {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("fromType", 3);
                ht.Add("eid", Data.Eid);
                ht.Add("pos", 1);
                GlobalMenuManager.Instance.Open("LTPartnerEquipmentInfoUI", ht);
                return;
            }

            bool showBelongInfo = false;

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView
                && LTEquipmentBelongInfoViewController.CanShow(Data.Eid)) {
                GlobalMenuManager.Instance.Open("LTEquipmentBelongInfoView", Data.Eid);
                showBelongInfo = true;
            }

            var hashTable = Johny.HashtablePool.Claim();
            hashTable.Add("fromType", 0);
            hashTable.Add("eid", Data.Eid);
            hashTable.Add("pos", showBelongInfo ? 3 : 1);
            GlobalMenuManager.Instance.Open("LTPartnerEquipmentInfoUI", hashTable);
        }

        public bool JudgeExp()
        {
            int CurEquipLevel = 0;
            DataLookupsCache.Instance.SearchIntByID(string.Format("inventory.{0}.currentLevel", LTPartnerEquipMainController.CurLevelupEid), out CurEquipLevel);
            if (CurEquipLevel >= LTPartnerEquipUplevelController.MaxLevel) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipCellController_3552")); return true; }
            if (LTPartnerEquipUplevelController.NextLevel >= LTPartnerEquipUplevelController.MaxLevel && !IsSelect) { MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTPartnerEquipCellController_3720")); return true; }
            return false;
        }

        private void OnSelectEquip(int id)
        {
            if (Data == null || Data.Eid == 0) return;
            if (id == Data.Eid) ChooseObj.CustomSetActive(true);
            else ChooseObj.CustomSetActive(false);
        }
        private void OnUnChooseEquip(int id)
        {
            if (Data == null || Data.Eid == 0) return;
            if (id == Data.Eid)
            {
                Data.isLock = !Data.isLock;
                LockBG.CustomSetActive(Data.isLock);
                if (Data.isLock)
                {
                    IsSelect = false;
                    SelectBG.CustomSetActive(false);
                }
            }
        }

        protected bool IsPressItemBtn
        {
            get
            {
                return m_IsPressItemBtn;
            }
            set
            {
                if (value != m_IsPressItemBtn) m_IsPressItemBtn = value;
                if (value)
                {
                    RegisterMonoUpdater();
                }
                else
                {
                    /*if(!IsPressRemoveBtn)*/
                    ErasureMonoUpdater();
                }
            }
        }
        private bool m_IsPressItemBtn;

        protected bool IsPressRemoveBtn
        {
            get { return m_IsPressRemoveBtn; }
            set
            {
                if (value != m_IsPressRemoveBtn) m_IsPressRemoveBtn = value;
                if (value)
                {
                    RegisterMonoUpdater();
                }
                else
                {
                    /*if (!IsPressItemBtn)*/
                    ErasureMonoUpdater();
                }
            }
        }
        private bool m_IsPressRemoveBtn;

        private float mPressTime1 = 0.5f;
        private float mPressTime2 = 0.1f;
        private float mPressTime;
        private int mIndex;
        private int delGoodsCount;

        private void ShowGoodsCount()
        {
            CountLabel.text = delGoodsCount <= 0 ? Data.Num.ToString() : string.Format("{0}/{1}", Data.Num - delGoodsCount, Data.Num);
            RemoveObj.CustomSetActive(delGoodsCount > 0);
            ItemMask.CustomSetActive(Data.Num - delGoodsCount <= 0);
        }

        public void OnItemBtnPress()
        {
            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState != LTPartnerEquipMainController.EquipmentViewState.LevelView)
            {
                return;
            }

            IsPressItemBtn = true;
            mPressTime = 0;
            mIndex = 0;
        }

        public void OnItemBtnRelease()
        {
            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState != LTPartnerEquipMainController.EquipmentViewState.LevelView)
            {
                return;
            }

            IsPressItemBtn = false;
        }

        public void OnRemoveBtnPress()
        {
            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState != LTPartnerEquipMainController.EquipmentViewState.LevelView)
            {
                return;
            }

            IsPressRemoveBtn = true;
            mPressTime = 0;
            mIndex = 0;
        }

        public void OnRemoveBtnRelease()
        {
            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState != LTPartnerEquipMainController.EquipmentViewState.LevelView || !isEquipUpItem)
            {
                return;
            }

            IsPressRemoveBtn = false;
        }

        public void Update()
        {
            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState != LTPartnerEquipMainController.EquipmentViewState.LevelView || !isEquipUpItem)
            {
                return;
            }

            if (IsPressItemBtn || IsPressRemoveBtn)
            {
                if (mPressTime > mPressTime1)
                {
                    if (mPressTime - mPressTime1 > mPressTime2 * mIndex)
                    {
                        mIndex++;
                        if (IsPressItemBtn)
                        {
                            OnItemBtnClick();
                        }
                        else if (IsPressRemoveBtn)
                        {
                            OnRemoveBtnClick();
                        }
                    }
                }
                mPressTime += Time.deltaTime;
            }
        }

        public void OnItemBtnClick()
        {
            if (Data == null || Data.Num - delGoodsCount <= 0 || JudgeExp())
            {
                IsPressItemBtn = false;
                IsPressRemoveBtn = false;
                return;
            }
            if (!LTResToolTipController.isOpen)
                LTResToolTipController.Show(LTShowItemType.TYPE_GAMINVENTORY, Data.ECid);
            delGoodsCount++;
            LTPartnerEquipDataManager.Instance.addEquipUpItemNum(Data.ECid);
            ShowGoodsCount();
            SelectEquipEven.ChooseEquipment();
        }

        public void OnRemoveBtnClick()
        {
            if (delGoodsCount <= 0)
            {
                IsPressItemBtn = false;
                IsPressRemoveBtn = false;
                return;
            }
            delGoodsCount--;
            LTPartnerEquipDataManager.Instance.removeEquipUpItemNum(Data.ECid);
            ShowGoodsCount();
            SelectEquipEven.ChooseEquipment();
        }
    }
}
