using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{

    public class CurrencyBarCtrl : DynamicMonoHotfix
    {
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        UIButton HotfixBtn3;
        public override void Awake()
        {
            base.Awake();

            HotfixBtn0 = mDMono.transform.Find("Table/0_ ProficiencyExp/Bg").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate());
            HotfixBtn1 = mDMono.transform.Find("Table/1_Exp/Bg").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(ToBuddyExpScreen));
            HotfixBtn2 = mDMono.transform.Find("Table/2_Gold/Bg").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(ToGoldScreen));
            HotfixBtn3 = mDMono.transform.Find("Table/3_Diamond/Bg").GetComponent<UIButton>();
            HotfixBtn3.onClick.Add(new EventDelegate(ToDiamondScreen));
        }

        public void ToVigorScreen()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        }

        public void ToGoldScreen()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        }

        public void ToDiamondScreen()
        {
            GlobalMenuManager.Instance.Open("LTChargeStoreHud");
            FusionAudio.PostEvent("UI/General/ButtonClick");
        }

        static public void PopGoldScreen()
        {
            GlobalMenuManager.Instance.Open("GoldNormalBuy");
        }

        public void PopHcScreen()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
        }

        public void PopVigorScreen()
        {
        }

        static public void ShowGoldBuyPanel()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            PopGoldScreen();
        }

        public void ToBuddyExpScreen()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        }

        public void ToActionPowerScreen()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            GlobalMenuManager.Instance.Open("LTResourceShopUI");
        }

        public void ToVigorRestoreTimeScreen()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            Vector2 screenPos = UICamera.lastEventPosition;
            GlobalMenuManager.Instance.Open("LTVigorToolTipUI", screenPos);
        }
    }

}


