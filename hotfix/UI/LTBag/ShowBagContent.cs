using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class ShowBagContent : DynamicMonoHotfix
    {
        public IDictionary BagItemData;
        public DynamicUISprite Icon;
        public UILabel NameLabel;
        public UILabel NumLabel;
        public UILabel ItemContent;
        public GameObject UseItem;
        public GameObject CompoundItem;
        public GameObject SourceItem;
        public UIGrid BtnGrid;
        public UISprite Border;
        public UISprite FrameBG;
    
        #region 装备item使用，不是装备item需要隐藏
        public DynamicUISprite mEquipSuitIcon;//装备类型图标（不是装备隐藏，附加套装箱子的物品也需要显示(by:2019/4/2)）
        public UISprite mEquipLevelBG;
        public UILabel mEquipLevel;
        public UILabel mEuqipLevelShadow;
        #endregion
    
        public string templateid;
        public int CurType=0;
        public GameObject TextBG;
    
        private ParticleSystemUIComponent mQualityFX;
        private EffectClip mEffectClip;
    
        #region 伙伴碎片item使用，不是伙伴碎片item需要隐藏
        public GameObject mClipFlag;
        #endregion
    
        #region 材料箱阶级数，不是材料箱或阶级数为0需要要隐藏
        public UILabel mboxGradeNumLab;
        #endregion
    
        private static ShowBagContent m_Instance;
        public static ShowBagContent Instance {
            get {
                if (m_Instance == null) {
                    //EB.Debug.LogError("ShowBagContent didnt Init");
                }
                return m_Instance;
            }
        }

        public override void Awake()
        {
            base.Awake();
            m_Instance = this;

            var t = mDMono.transform;
            Icon = t.GetComponent<DynamicUISprite>("TopLayout/TargetIcon/Icon");
            NameLabel = t.GetComponent<UILabel>("TopLayout/TargetIcon/Name");
            NumLabel = t.GetComponent<UILabel>("TopLayout/TargetIcon/NumLabel");
            ItemContent = t.GetComponent<UILabel>("BagWindow/Text");
            UseItem = t.FindEx("Buttons/Use").gameObject;
            CompoundItem = t.FindEx("Buttons/compound").gameObject;
            SourceItem = t.FindEx("Buttons/Source").gameObject;
            BtnGrid = t.GetComponent<UIGrid>("Buttons");
            Border = t.GetComponent<UISprite>("TopLayout/TargetIcon/Border");
            FrameBG = t.GetComponent<UISprite>("TopLayout/TargetIcon/FrameBG");
            mEquipSuitIcon = t.GetComponent<DynamicUISprite>("TopLayout/TargetIcon/EquipSuitIcon");
            mEquipLevelBG = t.GetComponent<UISprite>("TopLayout/TargetIcon/EquipLevel/Sprite");
            mEquipLevel = t.GetComponent<UILabel>("TopLayout/TargetIcon/EquipLevel");
            mEuqipLevelShadow = t.GetComponent<UILabel>("TopLayout/TargetIcon/EquipLevel/LabelShadow");
            CurType = 0;
            TextBG = t.FindEx("Sprite").gameObject;
            mClipFlag = t.FindEx("TopLayout/TargetIcon/Flag").gameObject;
            mboxGradeNumLab = t.GetComponent<UILabel>("TopLayout/TargetIcon/BoxGradeNum");
            mUseRequest = t.GetComponent<UIServerRequest>("Buttons/Use");
            mCompoundRequest = t.GetComponent<UIServerRequest>("Buttons/compound");
            dataLookup = t.GetDataLookupILRComponent<ItemUseDataLookup>();

            t.GetComponent<UIButton>("Buttons/Use").onClick.Add(new EventDelegate(OnUseButtonClick));
            t.GetComponent<UIButton>("Buttons/compound").onClick.Add(new EventDelegate(OnCompoundButtonClick));
            t.GetComponent<UIButton>("Buttons/Source").onClick.Add(new EventDelegate(OnSourceButtonClick));

            t.GetComponent<UIServerRequest>("Buttons/Use").onResponse.Add(new EventDelegate(mDMono, "OnFetchData"));
            t.GetComponent<UIServerRequest>("Buttons/compound").onResponse.Add(new EventDelegate(mDMono, "OnFetchData"));

            var parent = t.parent.parent;
            parent.GetComponent<UIButton>("UpButtons/Title/BtnList/AllBtn").onClick.Add(new EventDelegate(() => SelectBagItems(parent.FindEx("UpButtons/Title/BtnList/AllBtn").gameObject)));
            parent.GetComponent<UIButton>("UpButtons/Title/BtnList/ConsumablesBtn").onClick.Add(new EventDelegate(() => SelectBagItems(parent.FindEx("UpButtons/Title/BtnList/ConsumablesBtn").gameObject)));
            parent.GetComponent<UIButton>("UpButtons/Title/BtnList/EquipmentBtn").onClick.Add(new EventDelegate(() => SelectBagItems(parent.FindEx("UpButtons/Title/BtnList/EquipmentBtn").gameObject)));
            parent.GetComponent<UIButton>("UpButtons/Title/BtnList/PartnersBtn").onClick.Add(new EventDelegate(() => SelectBagItems(parent.FindEx("UpButtons/Title/BtnList/PartnersBtn").gameObject)));
            parent.GetComponent<UIButton>("UpButtons/Title/BtnList/MaterialsBtn").onClick.Add(new EventDelegate(() => SelectBagItems(parent.FindEx("UpButtons/Title/BtnList/MaterialsBtn").gameObject)));
        }

        public override void OnDestroy()
        {
            m_Instance = null;
        }
    
        public void  SetItemUseButton()
        {
            mCompoundRequest.gameObject.GetComponent<UIButton>().tweenTarget = null;
            bool IsCanUse = IsItemCanUse();
            Color btnColor = IsCanUse ? Color.white : Color.magenta;
            UISprite useBtn = mCompoundRequest.gameObject.GetComponent<UISprite>();
            BoxCollider colider = mCompoundRequest.gameObject.GetComponent<BoxCollider>();
            colider.enabled = IsCanUse;
            useBtn.color = btnColor;
        }
    
        public void SetBagContent(UIInventoryBagCellController cell) {
            if (cell != null&&cell.Data.m_DataID!=null) {
                this.BagItemData = cell.m_cell.ItemData;
                SetBagContentFunc();
            }
            else {
                ResetBagContentFunc();
            }
        }
    
        public void SetBagContentData(IDictionary data)
        {
            if (data != null)
            {
                this.BagItemData = data;
                SetBagContentFunc();
            }
            else
            {
                ResetBagContentFunc();
            }
        }
        
        private void SetBagContentFunc()
        {
            TextBG.CustomSetActive(true);
            NumLabel.text = EB.Localizer.GetString("ID_LABEL_NAME_HADE") + EB.Dot.Integer("num", BagItemData, 0).ToString();
            templateid = EB.Dot.String("economy_id", BagItemData, null);
            Icon.spriteName = Hotfix_LT.Data.EconemyTemplateManager.GetItemIcon(templateid);
            var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(templateid);
            LTUIUtil.SetText(NameLabel, item.Name);
            ItemContent.text = item.Desc;
            int quality_level = EB.Dot.Integer("qualityLevel", BagItemData, 0);
            ShowItemFX(quality_level);
            Border.spriteName = UIItemLvlDataLookup.LvlToStr(quality_level + "");
            if (quality_level == 7) FrameBG.spriteName = "Ty_Quality_Xuancai_Di";
            else FrameBG.spriteName = "Ty_Di_2";
            FrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor(quality_level + "");
            FrameBG.gameObject.CustomSetActive(true);
            string itemType = EB.Dot.String("system", BagItemData, null);
            mClipFlag.CustomSetActive(itemType == "HeroShard");
            mCompoundRequest.gameObject.GetComponent<UIButton>().tweenTarget = null;
            if (itemType == "Generic")
            {
                bool IsCanUse = IsItemCanUse();
                Color btnColor = IsCanUse ? Color.white : Color.magenta;
                UISprite useBtn = mCompoundRequest.gameObject.GetComponent<UISprite>();
                BoxCollider colider = mCompoundRequest.gameObject.GetComponent<BoxCollider>();
                colider.enabled = IsCanUse;
                useBtn.color = btnColor;
            }
    
            mEquipSuitIcon.gameObject.CustomSetActive(itemType == "Equipment");
            mEquipLevel.gameObject.CustomSetActive(itemType == "Equipment");
            if (itemType == "Equipment")
            {
                int equipLevel = EB.Dot.Integer("currentLevel", BagItemData, 1);           
                if (equipLevel <= 0)
                {
                    mEquipLevel.gameObject.CustomSetActive(false);
                }
                else
                {
                    mEquipLevel.gameObject.CustomSetActive(true);
                    mEquipLevel.text = mEuqipLevelShadow.text = "+" + equipLevel;
                    mEquipLevelBG.spriteName = UIItemLvlDataLookup.GetEquipLevelBGStr(quality_level);
                }
                mEquipSuitIcon.spriteName = Hotfix_LT.Data.EconemyTemplateManager.GetEquipSuitIcon(templateid);
            }
    
            string suitIcon = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipSuit(templateid);
            if (!string.IsNullOrEmpty(suitIcon))
            {
                mEquipSuitIcon.gameObject.CustomSetActive(true);
                mEquipSuitIcon.spriteName = suitIcon;
            }
    
            int grade = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGoodsGradeNum(templateid);
            mboxGradeNumLab.gameObject.CustomSetActive(grade != 0);
            if (grade != 0)
            {
                mboxGradeNumLab.text = string.Format("+{0}", grade);
            }
    
            if (!(item is Hotfix_LT.Data.GeneralItemTemplate))
            {
                UseItem.CustomSetActive(false);
                CompoundItem.CustomSetActive(false);
            }
            else
            {
                var GeneralItem = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGeneral(templateid);
                if (GeneralItem.CanUse)
                {
                    if (GeneralItem.CompoundItem != null)
                    {
                        UseItem.CustomSetActive(false);
                        CompoundItem.CustomSetActive(true);
                    }
                    else
                    {
                        UseItem.CustomSetActive(true);
                        CompoundItem.CustomSetActive(false);
                    }
                }
                else
                {
                    UseItem.CustomSetActive(false);
                    CompoundItem.CustomSetActive(false);
                }
            }
            SourceItem.GetComponent<BoxCollider>().enabled = true;
            SourceItem.GetComponent<UISprite>().color = Color.white;
            SourceItem.CustomSetActive(true);
            BtnGrid.Reposition();
        }
    
        private void ResetBagContentFunc()
        {
            this.BagItemData = null;
            LTUIUtil.SetText(NameLabel, "");
            NumLabel.text = "";
            ItemContent.text = "";
            templateid = "";
            Icon.spriteName = "";
            Border.spriteName = "";
            FrameBG.gameObject.CustomSetActive(false);
            Icon.gameObject.CustomSetActive(false);
            Icon.gameObject.CustomSetActive(true);
            if (SourceItem.activeSelf)
            {
                SourceItem.GetComponent<BoxCollider>().enabled = false;
                SourceItem.GetComponent<UISprite>().color = Color.magenta;
            }
            UseItem.CustomSetActive(false);
            SourceItem.CustomSetActive(false);
            CompoundItem.CustomSetActive(false);
            TextBG.CustomSetActive(false);
        }
    
        public void ShowItemFX(int qualityLevel)
        {
            HotfixCreateFX.ShowItemQualityFX(mQualityFX, mEffectClip, Icon.transform.parent, qualityLevel);
        }
    
        public void SelectBagItems(GameObject type) 
        {
            UIInventoryBagLogic.Instance.FirstItem = null;

            if (type.name == "AllBtn") 
            {
                if (CurType != 0)
                {
                    CurType = 0;
                    UIInventoryBagLogic.Instance.RefeshBag(CurType);
                }
            }
            else if(type.name == "ConsumablesBtn") 
            {
                if (CurType != 1)
                {
                    CurType = 1;
                    UIInventoryBagLogic.Instance.RefeshBag(CurType);
                }
            }
            else if (type.name == "EquipmentBtn") 
            {
                if (CurType != 2)
                {
                    CurType = 2;
                    UIInventoryBagLogic.Instance.RefeshBag(CurType);
                }
            }
            else if (type.name == "PartnersBtn") 
            {
                if (CurType != 3)
                {
                    CurType = 3;
                    UIInventoryBagLogic.Instance.RefeshBag(CurType);
                }
            }
            else if (type.name == "MaterialsBtn") 
            {
                if (CurType != 4)
                {
                    CurType = 4;
                    UIInventoryBagLogic.Instance.RefeshBag(CurType);
                }
            }
        }
    
        public void OnSourceButtonClick() {
            UITooltipManager.Instance.DisplayTooltipSrc(templateid, "Generic", "default");
        }
    
        public UIServerRequest mUseRequest;
        public UIServerRequest mCompoundRequest;
        public ItemUseDataLookup dataLookup;
    
        /// <summary>
        /// 根据物品数量判断物品是否可以使用
        /// </summary>
        public bool IsItemCanUse()
        {
            GenericItemInfo ItemInfo = new GenericItemInfo();
            ItemInfo.EconomyId = templateid;
            ItemInfo.InventoryId = EB.Dot.String("inventory_id", BagItemData, null);
            ItemInfo.Num = EB.Dot.Integer("num", BagItemData, 0);
            ItemInfo.Price = 0;
            Hotfix_LT.Data.GeneralItemTemplate item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(templateid) as Hotfix_LT.Data.GeneralItemTemplate;
            return ((item == null) ? false : ItemInfo.Num >= item.NeedNum);
        }
    
        public void OnCompoundButtonClick()
        {
            
            if (IsItemCanUse())
                this.OnCompound();
            else
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ShowBagContent_7616"));
        }
    
        public void OnCompound()
        {
            //        mCompoundRequest.parameters[0].parameter = EB.Dot.String("economyId", templateid, null);
            //        LoadingSpinner.Show();
            //        mCompoundRequest.SendRequest();
            //        dataLookup.ClearShowCache();
    
            GlobalMenuManager.Instance.Open("LTAwakeningGenericTrans", templateid);
            dataLookup.ClearShowCache();
        }
    
        public void OnUseButtonClick()
        {
            string itemType = EB.Dot.String("system", BagItemData, null);
            if (itemType.Equals("SelectBox"))
            {
                int num = EB.Dot.Integer("num", BagItemData, 0);
                string inventoryId = EB.Dot.String("inventory_id", BagItemData, string.Empty);
                List<Hotfix_LT.Data.SelectBox> selectBoxList = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSelectBoxListById(templateid);
                if (selectBoxList == null)
                {
                    EB.Debug.LogError("ShowBagContent OnUseButtonClick is Error, selectBoxList is Null");
                    return;
                }
    
                Hashtable table = Johny.HashtablePool.Claim();
                table["boxManNum"] = num;
                table["inventoryId"] = inventoryId;
                table["selectBoxList"] = selectBoxList;
    
                GlobalMenuManager.Instance.Open("LTSelectBoxUI", table);
            }
            else
            {
                GenericItemInfo ItemInfo = new GenericItemInfo();
                ItemInfo.EconomyId = templateid;
                ItemInfo.InventoryId = EB.Dot.String("inventory_id", BagItemData, null);
                ItemInfo.Num = EB.Dot.Integer("num", BagItemData, 0);
                ItemInfo.Price = 0;
                if (ItemInfo.Num > 1)
                {
                    GlobalMenuManager.Instance.Open("GenericUseView", ItemInfo);
                }
                else if (ItemInfo.Num == 1)
                {
                    OnUse();
                }
                else
                {
                    EB.Debug.Log("the number of items is smaller than 1");
                }
            }
        }
        
        public void OnUse() {
            mUseRequest.parameters[0].parameter = EB.Dot.String("inventory_id", BagItemData, null);
            mUseRequest.parameters[1].parameter = EB.Dot.Integer("num", BagItemData, 0).ToString();
            LoadingSpinner.Show();
            mUseRequest.SendRequest();
            dataLookup.ClearShowCache();
        }
    
        public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID) {
            LoadingSpinner.Hide();

            if (res.sucessful) {
                UIInventoryBagLogic.Instance.FirstItem = null;
                ShowAward(res.hashtable);
                DataLookupsCache.Instance.CacheData(res.hashtable);
                UIInventoryBagLogic.Instance.RefeshBag(CurType);           
            }
            else if (res.fatal) {
                SparxHub.Instance.FatalError(res.localizedError);
            }
        }

        private void ShowAward(Hashtable data) {
            if (data == null) {
                return;
            }
            if (data.ContainsKey("inventory")) {
                Hashtable allInventoryData = data["inventory"] as Hashtable;
                foreach (ICollection curInventory in allInventoryData.Values) {
                    string inventoryId = EB.Dot.String("inventory_id", curInventory, string.Empty);
    
                    int inventoryCurNumValue = EB.Dot.Integer("num", curInventory, 0);
                    int inventoryFormerNum = 0;
                    DataLookupsCache.Instance.SearchIntByID("inventory." + inventoryId + ".num", out inventoryFormerNum);
                    if (inventoryCurNumValue > inventoryFormerNum) {
                        LTShowItemData itemData = new LTShowItemData(EB.Dot.String("economy_id", curInventory, string.Empty), (inventoryCurNumValue - inventoryFormerNum), LTShowItemType.TYPE_GAMINVENTORY);
                        GameUtils.ShowAwardMsg(itemData);
                    }
                }
            }
            if (data.ContainsKey("heroStats")) {
                Hashtable heroStatsData = data["heroStats"] as Hashtable;
                foreach (ICollection curHeroStat in heroStatsData.Values) {
                    string shardId = EB.Dot.String("id", curHeroStat, string.Empty);
    
                    int curNumValue = EB.Dot.Integer("shard", curHeroStat, 0);
                    int formerNum = 0;
                    bool convertShard = EB.Dot.Bool("convertShard", curHeroStat, false);
                    string templateId = EB.Dot.String("template_id", curHeroStat, string.Empty);
                    if (!convertShard)
                    {
                        LTShowItemData itemData = new LTShowItemData(templateId, 1, LTShowItemType.TYPE_HERO);
                        GlobalMenuManager.Instance.Open("LTShowGetPartnerUI", itemData);
                    }
                    DataLookupsCache.Instance.SearchIntByID("heroStats." + shardId + ".shard", out formerNum);
                    if (curNumValue > formerNum) {
                        LTShowItemData itemData = new LTShowItemData(EB.Dot.String("character_id", curHeroStat, string.Empty), (curNumValue - formerNum), LTShowItemType.TYPE_HEROSHARD);
                        GameUtils.ShowAwardMsg(itemData);
                    }
                }
            }
            dataLookup.ShowAward();
        }
    }
}
