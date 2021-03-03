using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class OnPartnerSkillBreakGoodsRemoveSucc : GameEvent
    {
        public string goodsId;
        public int economyId;
    
        public OnPartnerSkillBreakGoodsRemoveSucc(string goodsId, int economyId)
        {
            this.goodsId = goodsId;
            this.economyId = economyId;
        }
    }
    
    public class LTPartnerSkillBreakCellController : DynamicCellController<UIInventoryBagCellData>, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconDySprite = t.GetComponent<DynamicUISprite>("ItemObj/Icon");
            QualitySprite = t.GetComponent<UISprite>("ItemObj/Quality");
            FrameBGSprite = t.GetComponent<UISprite>("ItemObj/FrameBG");
            PartnerChipSprite = t.GetComponent<UISprite>("ItemObj/Chip");
            NumLabel = t.GetComponent<UILabel>("ItemObj/Num");
            DelBtnObj = t.FindEx("ItemObj/RemoveBtn").gameObject;
            ItemObj = t.FindEx("ItemObj").gameObject;
            RecommendObj = t.FindEx("ItemObj/Recommend").gameObject;
           

            t.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(OnItemBtnPress));
            t.GetComponent<UIEventTrigger>().onRelease.Add(new EventDelegate(OnItemBtnRelease));
            t.GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(OnItemBtnClick));
            t.GetComponent<UIEventTrigger>().onDragStart.Add(new EventDelegate(OnItemBtnRelease));
            t.GetComponent<UIEventTrigger>("ItemObj/RemoveBtn").onPress.Add(new EventDelegate(OnRemoveBtnPress));
            t.GetComponent<UIEventTrigger>("ItemObj/RemoveBtn").onRelease.Add(new EventDelegate(OnRemoveBtnRelease));
            t.GetComponent<UIEventTrigger>("ItemObj/RemoveBtn").onClick.Add(new EventDelegate(OnRemoveBtnClick));
            
            Messenger.AddListener<string,int>(Hotfix_LT.EventName.OnPartnerSkillBreakItemClick,OnPartnerSkillBreakItemClick);
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}
        public override void OnDisable()
        {
            base.OnDisable();
            ErasureMonoUpdater();
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener<string,int>(Hotfix_LT.EventName.OnPartnerSkillBreakItemClick,OnPartnerSkillBreakItemClick);
        }


        public const string m_EconomyIDPropertyName = "economy_id";
        public const string m_NumPropertyName = "num";
        public const string m_QualityLevelPropertyName = "qualityLevel";
    
        public DynamicUISprite IconDySprite;
        public UISprite QualitySprite;
        public UISprite FrameBGSprite;
        public UISprite PartnerChipSprite;
        public UILabel NumLabel;
        public GameObject DelBtnObj;
        public GameObject ItemObj;
        public GameObject RecommendObj;
    
        private string goodsId; // 服务器的物品唯一ID
        private int economyId;  // 表格的物品ID
        private int goodsCount;
        private int quality;
        private int delGoodsCount;
    
        private LTPartnerSkillBreakController _con;

        public LTPartnerSkillBreakController con
        {
            get
            {
                if (_con==null)
                {
                    _con = mDMono.transform.parent.parent.parent.parent.parent.parent.GetUIControllerILRComponent<LTPartnerSkillBreakController>();
                }

                return _con;
            }
        }
        private void OnPartnerSkillBreakItemClick(string _goodsId, int _times)
        {
            if (_goodsId==goodsId)
            {
                OnItemBtnClickByTime(_times);
            }
        }
    
    
        public override void Clean()
        {
            SetItemData(null);
        }
    
        public override void Fill(UIInventoryBagCellData itemData)
        {
            SetItemData(itemData);
        }
    
        private void SetItemData(UIInventoryBagCellData itemData)
        {
            if (itemData == null)
            {
                ItemObj.SetActive(false);
                return;
            }
            goodsId = itemData.m_DataID;
            ItemObj.SetActive(true);
            ShowItem();
        }
    
        public void ShowItem()
        {
            IDictionary itemData;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>(string.Format("inventory.{0}", goodsId), out itemData);
            if (itemData == null)
            {
                ItemObj.SetActive(false);
                return;
            }
            delGoodsCount = LTPartnerDataManager.Instance.GetSkillBreakDelGoodsByGoodsId(goodsId);
    
            economyId = EB.Dot.Integer(m_EconomyIDPropertyName, itemData, 0);
            goodsCount = EB.Dot.Integer(m_NumPropertyName, itemData, 0);
            quality = EB.Dot.Integer(m_QualityLevelPropertyName, itemData, 0);
    
            QualitySprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality - 1];
    
            Hotfix_LT.Data.EconemyItemTemplate tempItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(economyId);
            IconDySprite.spriteName = tempItemData.IconId;
    
            Hotfix_LT.Data.GeneralItemTemplate tempGenItemData = tempItemData as Hotfix_LT.Data.GeneralItemTemplate;
            if (tempGenItemData != null)
            {
                PartnerChipSprite.gameObject.CustomSetActive(tempGenItemData.System.CompareTo("HeroShard") == 0);
            }
    
            ShowGoodsCount();
            ShowGoodsColor();
        }
    
        private void ShowGoodsCount()
        {
            NumLabel.text = delGoodsCount <= 0 ? goodsCount.ToString() : string.Format("{0}/{1}", goodsCount - delGoodsCount, goodsCount);
            DelBtnObj.SetActive(delGoodsCount > 0);
            RecommendObj.CustomSetActive(!DelBtnObj.activeSelf && LTPartnerDataManager.Instance.IsSkillBreakRecommend(economyId));
        }
    
        private void ShowGoodsColor()
        {
            if (goodsCount > delGoodsCount)
            {
                SetNormal(QualitySprite);
                SetNormal(IconDySprite);
                SetNormal(PartnerChipSprite);
                GameItemUtil.SetColorfulPartnerCellFrame(quality - 1, FrameBGSprite);
            }
            else
            {
                SetGrey(QualitySprite);
                SetDark(FrameBGSprite);
                SetGrey(IconDySprite);
                SetGrey(PartnerChipSprite);
            }
        }
    
        private void SetGrey(UIWidget item)
        {
            item.color = new Color(1, 0, 1, 1);
        }
    
        private void SetNormal(UIWidget item)
        {
            item.color = new Color(1, 1, 1, 1);
        }
    
        private void SetDark(UIWidget item)
        {
            item.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    
        public void OnItemBtnClick()
        {
            OnItemBtnClickByTime(1);
        }
    
        public void OnItemBtnClickByTime(int times)
        {
            if (goodsCount - delGoodsCount <= 0 || con.IsMaxSkillLevel() || con.IsPlayingAni || !ItemObj.activeInHierarchy)
            {
                if (con.IsMaxSkillLevel() && goodsCount - delGoodsCount > 0)
                {
                    LTPartnerDataManager.Instance.ShowPartnerMessage(EB.Localizer.GetString("ID_codefont_in_LTPartnerDataManager_18538"));
                }
                mIsPressItemBtn = false;
                mIsPressRemoveBtn = false;
                return;
            }
            delGoodsCount+=times;
            ShowGoodsCount();
            ShowGoodsColor();
           Messenger.Raise(Hotfix_LT.EventName.OnPartnerSkillBreakGoodsAddSucc,goodsId,economyId,times);
        }
    
        public void OnRemoveBtnClick()
        {
            if (delGoodsCount <= 0)
            {
                mIsPressItemBtn = false;
                mIsPressRemoveBtn = false;
                return;
            }
            delGoodsCount--;
            ShowGoodsCount();
            ShowGoodsColor();
            Messenger.Raise(Hotfix_LT.EventName.OnPartnerSkillBreakGoodsRemoveSucc,goodsId,economyId);
        }
    
        private bool mIsPressItemBtn;
        private bool mIsPressRemoveBtn;
        private float mPressTime1 = 0.5f;
        private float mPressTime2 = 0.1f;
        //private float mPressTime3 = 0.1f;
        private float mPressTime;
        private int mIndex;
    
        public void OnItemBtnPress()
        {
            mIsPressItemBtn = true;
            mPressTime = 0;
            mIndex = 0;
    
            if (con.isOpenLimit)
            {
                mPressTime2 = con.limitValue;
            }
        }
    
        public void OnItemBtnRelease()
        {
            mIsPressItemBtn = false;
        }
    
        public void OnRemoveBtnPress()
        {
            mIsPressRemoveBtn = true;
            mPressTime = 0;
            mIndex = 0;
    
            if (con.isOpenLimit)
            {
                mPressTime2 = con.limitValue;
            }
        }
    
        public void OnRemoveBtnRelease()
        {
            mIsPressRemoveBtn = false;
        }
    
        public void Update()
        {
            if (mIsPressItemBtn || mIsPressRemoveBtn)
            {
                if (mPressTime > mPressTime1)
                {
                    if (mPressTime - mPressTime1 > mPressTime2 * mIndex)
                    {
                        mIndex++;
                        if (mIsPressItemBtn)
                        {
                            OnItemBtnClick();
                        }
                        else if (mIsPressRemoveBtn)
                        {
                            OnRemoveBtnClick();
                        }
                    }
                }
                mPressTime += Time.deltaTime;
            }
        }
    }
}
