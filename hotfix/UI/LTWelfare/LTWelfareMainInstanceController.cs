using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTWelfareMainInstanceController : DynamicMonoHotfix
    {
        public UIButton BuyBtn;
        public UILabel MoneyPriceLab;
        public LTWelfareMainInstanceScroll Scrool;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            BuyBtn =t.GetComponent<UIButton>("BG/ReceiveButton");
            MoneyPriceLab = t.GetComponent<UILabel>("BG/ReceiveButton/Label");
            BuyBtn.onClick.Add(new EventDelegate(OnBuyBtnClick));
            Scrool = t.GetMonoILRComponent<LTWelfareMainInstanceScroll>("ScrollView/Placeholder/Grid");
            EB.IAP.Item GiftItem = null;
            EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(6003, out GiftItem);
            if(GiftItem!=null) MoneyPriceLab.text =string.Format ("{0} {1}", GiftItem.localizedCost,EB.Localizer .GetString("ID_BUY"));

            ChangeUpdataUI();
        }

        public override void Start()
        {
            base.Start();
            LTWelfareEvent.WelfareOnfocus += ChangeUpdataUI;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            LTWelfareEvent.WelfareOnfocus -= ChangeUpdataUI;
        }

        public void ChangeUpdataUI()
        {
            var dates = EventTemplateManager.Instance.GetAllMainCampaignRewardTp1();
            ResetBuyBtn();
            Scrool.SetItemDatas(dates.ToArray());
        }

        private void ResetBuyBtn()
        {
            if (LTWelfareModel.Instance.HasMain)
            {
                BuyBtn.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                BuyBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                BuyBtn.isEnabled = false;
            }
            else
            {
                BuyBtn.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                BuyBtn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                BuyBtn.isEnabled = true;
            }
        }

        private void OnBuyBtnClick()
        {
            if (!LTWelfareModel.Instance.HasMain)
            {
                EB.IAP.Item GiftItem = null;
                EB.Sparx.Hub.Instance.WalletManager.GetGiftItem(6003, out GiftItem);
                GlobalMenuManager.Instance.Open("LTChargeStoreGiftUI", GiftItem);
            }
        }
    }
}
