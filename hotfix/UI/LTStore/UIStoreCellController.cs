using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class StoreItemData
    {
        public int sid;//shop id
        public string id;
        public string type;
        public int num;
        public int have;
        public string cost_id;//货币id hc gold grain ore 
        public int cost;
        public bool sell_out;
        public int weight;//权重
        public int buy_id;//购买id
        public string store_type;
        public string item_name;
        public float discount;//前端折扣
        public int remainBuyNum;

        public StoreItemData()
        {
            sid = 0;
            id = "";
            type = "";
            num = 0;
            have = 0;
            cost_id = "";
            cost = 0;
            sell_out = false;
            weight = 0;
            buy_id = 0;
            discount = 1;
            remainBuyNum = 0;
        }

        public StoreItemData(int sid, string id, string type, int num, int have, string constid, int cost, bool sellout, int weight, int buy_id, string store_type, float discount, int remainBuyNum = 0)
        {
            this.sid = sid;
            this.id = id;
            this.type = type;
            this.num = num;
            this.have = have;
            this.cost_id = constid;
            this.cost = cost;
            this.sell_out = sellout;
            this.weight = weight;
            this.buy_id = buy_id;
            this.store_type = store_type;
            this.discount = discount;
            this.remainBuyNum = remainBuyNum;
        }
    }

    public class UIStoreCellController : DynamicCellController<StoreItemData>
    {
        public StoreItemData Data
        {
            get; set;
        }
        public UIStoreShowItem m_Item;
        public UILabel m_Discount_Label;
        public UILabel m_Cost_Label;
        public UISprite m_Cost_Sprite;
        public UISprite m_SellOut_Sprite;
        public BoxCollider m_BuyBtn;
        public UIResourceComponent m_UIResourceComponent;
        public const string HaveFormatStr = "[9f9f9f]{0}[-][2f8ff2]{1}[-]";
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Item = t.GetMonoILRComponent<UIStoreShowItem>();
            m_Discount_Label = t.Find("DiscountLabel").GetComponent<UILabel>();
            m_Cost_Label = t.GetComponent<UILabel>("Cost/Cost");
            m_Cost_Sprite = t.GetComponent<UISprite>("Cost/Icon");
            m_SellOut_Sprite = t.GetComponent<UISprite>("SellOut");
            m_BuyBtn = t.GetComponent<BoxCollider>();
            m_UIResourceComponent = t.GetMonoILRComponent<UIResourceComponent>("Cost");

            t.GetComponent<UIEventTrigger>().onClick.Add(new EventDelegate(() => { OnCellClick(); }));
        }

        public void UpdateUI()
        {
            if (string.IsNullOrEmpty(Data.id))
            {
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            else
            {
                mDMono.gameObject.CustomSetActive(true);
            }

            //m_Have_Label.text = string.Format(HaveFormatStr,EB.Localizer.GetString("ID_LABEL_NAME_HADE"),Data.have);

            if (m_Discount_Label != null)
            {
                m_Discount_Label.text = LTChargeManager.GetDiscountText(Data.discount);
                m_Discount_Label.gameObject.CustomSetActive(Data.discount > 0 && Data.discount < 1);
            }
    
            int resBalance=BalanceResourceUtil.GetResValue(Data.cost_id);

            if (resBalance < Data.cost)
            {
                m_Cost_Label.color = LT.Hotfix.Utility.ColorUtility.RedColor;
            }
            else
            {
                m_Cost_Label.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            }

            LTUIUtil.AddBlackOutLineToLabel(m_Cost_Label, Data.cost.ToString());
            m_Cost_Sprite.spriteName = BalanceResourceUtil.GetResSpriteName(Data.cost_id);

            if (m_UIResourceComponent != null)
            {
                m_UIResourceComponent.m_ResID = Data.cost_id;
            }

            m_Item.LTItemData = new LTShowItemData(Data.id,Data.num, Data.type, false);
            LTIconNameQuality itemInfo = LTItemInfoTool.GetInfo(m_Item.LTItemData.id, m_Item.LTItemData.type, m_Item.LTItemData.coloring);
            Data.item_name = itemInfo.name;
            m_Item.Name.applyGradient = false;

            if (Data.sell_out)
            {
                m_Cost_Label.gameObject.CustomSetActive(false);
                m_Cost_Sprite.gameObject.CustomSetActive(false);
                m_SellOut_Sprite.gameObject.CustomSetActive(true);
                m_BuyBtn.enabled = false;
            }
            else
            {
                m_Cost_Label.gameObject.CustomSetActive(true);
                m_Cost_Sprite.gameObject.CustomSetActive(true);
                m_SellOut_Sprite.gameObject.CustomSetActive(false);
                m_BuyBtn.enabled = true;
            }      
        }
    
        public override void Fill(StoreItemData itemData)
        {
            Data = itemData;
            if (Data != null) mDMono.gameObject.CustomSetActive(true);
            UpdateUI();
            RefreshPurchaseLimit();
        }
    
        public override void Clean()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    
        public void OnCellClick()
        {
            //FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTStoreBuyUI",this.Data);
        }

        private UILabel _purchaseLimitLabel;
        
        private void RefreshPurchaseLimit()
        {
            if (_purchaseLimitLabel == null)
            {
                _purchaseLimitLabel = mDMono.transform.GetComponent<UILabel>("PurchaseLimit", false);
            }

            if (_purchaseLimitLabel != null && Data != null)
            {
                int maxBuyNum = 0;
                BossChallengeTemplate info = null;

                switch (Data.store_type)
                {
                    case "bosschallenge1":
                        info = ShopTemplateManager.Instance.GetBossChallenge1Template(Data.sid);
                        break;
                    case "bosschallenge2":
                        info = ShopTemplateManager.Instance.GetBossChallenge2Template(Data.sid);
                        break;
                    case "bosschallenge3":
                        info = ShopTemplateManager.Instance.GetBossChallenge3Template(Data.sid);
                        break;
                }

                if (info != null)
                {
                    maxBuyNum = info.buy_num;
                }

                _purchaseLimitLabel.text = EB.Localizer.GetString("ID_PURCHASE_LIMIT_ONLY") + string.Format("{0}/{1}", maxBuyNum - Data.remainBuyNum, maxBuyNum);
                _purchaseLimitLabel.gameObject.SetActive(Data.remainBuyNum > 0);
            }
            
        }
    }
}
