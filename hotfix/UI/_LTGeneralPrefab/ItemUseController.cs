using UnityEngine;
using System.Collections;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class ItemUseController : UIControllerHotfix
    {
        #region Overriden Fields
    
        public override bool ShowUIBlocker
        {
            get { return true; }
        }
    
        #endregion
    
        #region Pubic Fields
    
        public UILabel mName, mNumLabel, mUseNumLabel;
        public DynamicUISprite mIcon;
        public UISprite mFrame, mFrameBG;
        public GameObject mReduceBtnMask, mAddBtnMask;
        public UIButton mReduceBtn, mAddBtn, mUseBtn, mMaxBtn;
        public UIServerRequest mUseRequest;
        public ItemUseDataLookup dataLookup;
        public DynamicUISprite mEquipSuitIcon;
        private ParticleSystemUIComponent mQualityFX;
        private EffectClip mEffectClip;
        #endregion
    
        #region 材料箱阶级数，不是材料箱或阶级数为0需要要隐藏
        public UILabel mboxGradeNumLab;
        #endregion
    
        #region Private Fields
    
        private int mCurNum, mTotalNum, qualityLevel;
        private GenericItemInfo curItemInfo = new GenericItemInfo();

        #endregion

        #region Overriden Methods
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            mName = t.GetComponent<UILabel>("DataPanel/Item/Name");
            mNumLabel = t.GetComponent<UILabel>("DataPanel/Item/Num");
            mUseNumLabel = t.GetComponent<UILabel>("DataPanel/Use/Adjust/UseNum");
            mIcon = t.GetComponent<DynamicUISprite>("DataPanel/Item/Icon");
            mFrame = t.GetComponent<UISprite>("DataPanel/Item/Icon/LvlBorder");
            mFrameBG = t.GetComponent<UISprite>("DataPanel/Item/Icon/FrameBG");
            mReduceBtnMask = t.FindEx("DataPanel/Use/Adjust/Reduce/ReduceMask").gameObject;
            mAddBtnMask = t.FindEx("DataPanel/Use/Adjust/Add/AddMask").gameObject;
            mReduceBtn = t.GetComponent<UIButton>("DataPanel/Use/Adjust/Reduce");
            mAddBtn = t.GetComponent<UIButton>("DataPanel/Use/Adjust/Add");
            mUseBtn = t.GetComponent<UIButton>("DataPanel/Use/OkUse");
            mMaxBtn = t.GetComponent<UIButton>("DataPanel/Use/Adjust/Max");
            mUseRequest = t.GetComponent<UIServerRequest>("DataPanel/Use");
            dataLookup = t.GetDataLookupILRComponent<ItemUseDataLookup>();
            mEquipSuitIcon = t.GetComponent<DynamicUISprite>("DataPanel/Item/Icon/EquipSuitIcon");
            mboxGradeNumLab = t.GetComponent<UILabel>("DataPanel/Item/Icon/BoxGradeNum");
            controller.backButton = t.GetComponent<UIButton>("DataPanel/CloseBtn");

            mMaxBtn.onClick.Add(new EventDelegate(OnMaxClick));
            mUseBtn.onClick.Add(new EventDelegate(OnUse));

            t.GetComponent<ContinuePressTrigger>("DataPanel/Use/Adjust/Reduce").m_CallBackPress.Add(new EventDelegate(OnReduceClick));
            t.GetComponent<ContinuePressTrigger>("DataPanel/Use/Adjust/Add").m_CallBackPress.Add(new EventDelegate(OnAddClick));

            t.GetComponent<UIServerRequest>("DataPanel/Use").onResponse.Add(new EventDelegate(controller, "OnFetchData"));
        }

        public override void SetMenuData(object param)
        {
            var itemInfo = param as GenericItemInfo;
            if (itemInfo != null)
            {
                curItemInfo = itemInfo;
                var inl = LTItemInfoTool.GetInfo(curItemInfo.EconomyId, LTShowItemType.TYPE_GAMINVENTORY);
                mIcon.spriteName = inl.icon;
                mFrame.spriteName = UIItemLvlDataLookup.LvlToStr(inl.quality);
                mFrameBG.spriteName = UIItemLvlDataLookup.GetItemFrameBGSprite(inl.quality);
                mFrameBG.color = UIItemLvlDataLookup.GetItemFrameBGColor(inl.quality);
                mName.text = inl.name;
                mTotalNum = curItemInfo.Num;
                mCurNum = mTotalNum;
                qualityLevel = int.Parse(inl.quality);
                Show();
    
                var suitIcon = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipSuit(curItemInfo.EconomyId);
                if (!string.IsNullOrEmpty(suitIcon))
                {
                    mEquipSuitIcon.gameObject.SetActive(true);
                    mEquipSuitIcon.spriteName = suitIcon;
                }
                else
                {
                    mEquipSuitIcon.gameObject.SetActive(false);
    
                }
    
                var grade = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetGoodsGradeNum(curItemInfo.EconomyId);
                mboxGradeNumLab.gameObject.CustomSetActive(grade != 0);
                if (grade != 0) mboxGradeNumLab.text = string.Format("+{0}", grade);
            }
        }
    
        public override void OnFocus()
        {
            base.OnFocus();
            HotfixCreateFX.ShowItemQualityFX(mQualityFX, mEffectClip, mIcon.transform, qualityLevel);
        }
    
        #endregion
    
        #region Public Methods
    
        public void OnReduceClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (mCurNum > 1)
            {
                --mCurNum;
                UpdateInfo();
            }
        }
    
        public void OnAddClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (mCurNum < mTotalNum)
            {
                ++mCurNum;
                UpdateInfo();
            }
        }
    
        public void OnMaxClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (mCurNum != mTotalNum)
            {
                mCurNum = mTotalNum;
                UpdateInfo();
            }
        }
    
        public void OnUse()
        {
            mUseRequest.parameters[0].parameter = curItemInfo.InventoryId.Replace("inventory.", "");
            mUseRequest.parameters[1].parameter = mCurNum.ToString();
            LoadingSpinner.Show();
            mUseRequest.SendRequest();
            dataLookup.ClearShowCache();
        }
    
        public override void OnFetchData(Response res, int reqInstanceID)
        {
            LoadingSpinner.Hide();
            if (res.sucessful)
            {
                ShowAward(res.hashtable);
                DataLookupsCache.Instance.CacheData(res.hashtable);
                if (res.hashtable.ContainsKey("heroStats"))
                {
                    LTPartnerDataManager.Instance.InitPartnerData();
                }
                mTotalNum = mTotalNum - mCurNum;
                mCurNum = 1;
                Show();
                if (mTotalNum <= 0)
                {
                    UIInventoryBagLogic.Instance.FirstItem = null;
                    UIInventoryBagLogic.Instance.RefeshBag(ShowBagContent.Instance.CurType);
                    controller.Close();
                }
                else
                {
                    UIInventoryBagLogic.Instance.RefeshBag(ShowBagContent.Instance.CurType);
                }
            }
            else if (res.fatal)
            {
                Hub.Instance.FatalError(res.localizedError);
            }
        }
    
        #endregion
    
        #region Private Methods
    
        private void Show()
        {
            mNumLabel.text = EB.Localizer.GetString("ID_LABEL_NAME_HADE") + mTotalNum;
            if (mTotalNum <= 0)
            {
                mUseNumLabel.color = LT.Hotfix.Utility.ColorUtility.RedColor;
                mUseBtn.isEnabled = false;
            }
            else
            {
                mUseNumLabel.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
                mUseBtn.isEnabled = true;
                mMaxBtn.isEnabled = true;
            }
            UpdateInfo();
        }
    
        private void UpdateInfo()
        {
            mUseNumLabel.text = mCurNum.ToString();
            
            mAddBtnMask.CustomSetActive(false);
            mReduceBtnMask.CustomSetActive(false);
            mReduceBtn.isEnabled = true;
            mAddBtn.isEnabled = true;
            mReduceBtn.defaultColor = Color.white;
            if (mCurNum <= 1)
            {
                mReduceBtnMask.CustomSetActive(true);
                mReduceBtn.isEnabled = false;
                
            }
            if (mCurNum >= mTotalNum)
            {
                mAddBtnMask.CustomSetActive(true);
                mAddBtn.isEnabled = false;
               
            }
        }
    
        private void ShowAward(Hashtable data)
        {
            if (data == null) return;
            if (data.ContainsKey("inventory"))
            {
                var allInventoryData = data["inventory"] as Hashtable;
                foreach (ICollection curInventory in allInventoryData.Values)
                {
                    var inventoryId = EB.Dot.String("inventory_id", curInventory, string.Empty);
    
                    var inventoryCurNumValue = EB.Dot.Integer("num", curInventory, 0);
    
                    var inventoryFormerNum = 0;
                    DataLookupsCache.Instance.SearchIntByID("inventory." + inventoryId + ".num", out inventoryFormerNum);
                    if (inventoryCurNumValue > inventoryFormerNum)
                    {
                        var itemData = new LTShowItemData(EB.Dot.String("economy_id", curInventory, string.Empty), inventoryCurNumValue - inventoryFormerNum, LTShowItemType.TYPE_GAMINVENTORY);
                        //上传友盟获得钻石，道具
                        if (itemData.id == "hc") FusionTelemetry.PostBonus(itemData.count, Umeng.GA.BonusSource.Source8);
                        GameUtils.ShowAwardMsg(itemData);
                    }
                }
            }
    		if (data.ContainsKey("heroStats"))
    		{
    			var heroStatsData = data["heroStats"] as Hashtable;               
                foreach (ICollection curHeroStat in heroStatsData.Values)
    			{
    				var shardId = EB.Dot.String("id", curHeroStat, string.Empty);
    
    				var curNumValue = EB.Dot.Integer("shard", curHeroStat, 0);
    
    				var formerNum = 0;
    			    var characterId = EB.Dot.String("character_id", curHeroStat, string.Empty);
    			    var templateId = EB.Dot.String("template_id", curHeroStat, string.Empty);
                    var convertShard = EB.Dot.Bool("convertShard", curHeroStat, false);
    			    if (!convertShard)
    			    {
    			        var itemData = new LTShowItemData(templateId, 1, "hero");
                        LTInventoryAllController.SetShowAwardsQueue(itemData);
                    }
                    DataLookupsCache.Instance.SearchIntByID("heroStats." + shardId + ".shard", out formerNum);
    				if (curNumValue > formerNum)
    				{
    					var itemData = new LTShowItemData(characterId, curNumValue - formerNum, LTShowItemType.TYPE_HEROSHARD);
                        GameUtils.ShowAwardMsg(itemData);
                    }
    			}
                Messenger.Raise(Hotfix_LT.EventName.InventoryEvent);            
            }
            dataLookup.ShowAward();
        }
    
        #endregion
    }
}
