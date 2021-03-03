using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTStoreBuyController : UIControllerHotfix
    {
        public UIStoreBuyShowItem Item;
        public UILabel HaveLabel;
        public UILabel m_Discount_Label;
        public UILabel BuyNum;
        public UILabel m_Cost_Label;
        public UISprite m_Cost_Sprite;
        public UILabel Info;

        private StoreItemData Data;

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            Item = t.GetMonoILRComponent<UIStoreBuyShowItem>("DataPanel/2_Icon");
            HaveLabel = t.GetComponent<UILabel>("DataPanel/2_Icon/Num");
            m_Discount_Label = t.GetComponent<UILabel>("DataPanel/2_Icon/DiscountLabel");
            BuyNum = t.GetComponent<UILabel>("DataPanel/Buy/OkBuy/Cost/num");
            m_Cost_Label = t.GetComponent<UILabel>("DataPanel/Buy/OkBuy/Cost/Price");
            m_Cost_Sprite = t.GetComponent<UISprite>("DataPanel/Buy/OkBuy/Cost/Currency");
            Info = t.GetComponent<UILabel>("DataPanel/Info/BG/Label");
            controller.backButton = t.GetComponent<UIButton>("DataPanel/CloseBtn");

            t.GetComponent<UIButton>("DataPanel/Buy/OkBuy").onClick.Add(new EventDelegate(()=> { OnBuyClick(); }));
        }



    
        public override void SetMenuData(object param) {
            base.SetMenuData(param);
            Data = param as StoreItemData;
            Item.LTItemData = new LTShowItemData(Data.id, Data.num, Data.type, false);
            HaveLabel.text= EB.Localizer.GetString("ID_codefont_in_LTStoreBuyController_544")+ Data.have;
            m_Discount_Label.text = LTChargeManager.GetDiscountText(Data.discount);
            m_Discount_Label.gameObject.CustomSetActive(Data.discount > 0 && Data.discount < 1);
    
            m_Cost_Label.text= m_Cost_Label.transform.GetChild(0).GetComponent<UILabel>().text = Data.cost.ToString();
            int resBalance = BalanceResourceUtil.GetResValue(Data.cost_id);
            if (resBalance < Data.cost)
                m_Cost_Label.color = LT.Hotfix.Utility.ColorUtility.RedColor;
            else
                m_Cost_Label.color = LT.Hotfix.Utility.ColorUtility.WhiteColor;
            m_Cost_Sprite.spriteName = BalanceResourceUtil.GetResSpriteName(Data.cost_id);
    
            string desc = string.Empty;
            if (Data.store_type == "challenge")
            {
                Hotfix_LT.Data.SkillTemplate skillTpl = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(int.Parse(Data.id));
                desc = skillTpl.Description;
            }
            else
            {
                var item = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(Data.id);
                desc = item.Desc;
            }
            Info.text = desc;
        }
    
        public void OnBuyClick() {
            Hotfix_LT.Messenger.Raise(EventName .StoreBuyEvent,Data);
        }
    
        public override IEnumerator OnRemoveFromStack()
        {
            base.OnRemoveFromStack();
            DestroySelf();
            yield break;
        }
    
        public override bool ShowUIBlocker {
            get {
                return true;
            }
        }

        public override void StartBootFlash() {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j) {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    }
}
