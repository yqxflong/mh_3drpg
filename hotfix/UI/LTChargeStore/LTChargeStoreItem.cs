using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTChargeStoreItem : DynamicMonoHotfix
    {
        public UILabel TitleLab;
        public UILabel PriceValueLab;
        public UILabel TagLab;
        public UILabel DiscountLab;
        public UILabel TimesLabel;

        public UISprite IconSp;
        public UISprite TagBGSp;
        public GameObject Uimask;
        public BoxCollider boxcollider;
        public GameObject TagObj;
        private EB.IAP.Item curChargeData;

        private System.Action<EB.IAP.Item> onClickItem;

        ConsecutiveClickCoolTrigger HotfixCoolBtn0;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            TitleLab = t.Find("Title").GetComponent<UILabel>();
            PriceValueLab = t.Find("Price/PriceValue").GetComponent<UILabel>();
            TagLab = t.Find("Tag/Label").GetComponent<UILabel>();
            DiscountLab = t.Find("Discount/DiscountLab").GetComponent<UILabel>();
            TimesLabel = t.Find("TimesLabel").GetComponent<UILabel>();
            IconSp = t.Find("Icon").GetComponent<UISprite>();
            TagBGSp = t.Find("Tag/BG").GetComponent<UISprite>();
            Uimask = t.Find("Mask").gameObject;
            boxcollider = t.GetComponent<BoxCollider>();
            TagObj = t.Find("Tag").gameObject;
            HotfixCoolBtn0 = t.GetComponent<ConsecutiveClickCoolTrigger>();
            HotfixCoolBtn0.clickEvent.Add(new EventDelegate(OnClickItem));
        }

        public void Show(EB.IAP.Item data)
        {
            if (data == null)
            {
                EB.Debug.LogError("ChargeData is Null !!!!");
                return;
            }

            curChargeData = data;

            RefreshUI();
        }

        public void SetClickEvent(System.Action<EB.IAP.Item> onClickItem)
        {
            this.onClickItem = onClickItem;
        }

        private void RefreshUI()
        {
            IconSp.spriteName = curChargeData.icon;
            //免费礼包显示价格        
            PriceValueLab.text = curChargeData.localizedCost;

            string colorStr = curChargeData.categoryValue == 2 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.YellowColorHexadecimal;
            TitleLab.text = string.Format("[{0}]{1}[-]", colorStr, curChargeData.longName);

            DiscountLab.text = LTChargeManager.GetDiscountText(curChargeData.discount);
            DiscountLab.transform.parent.gameObject.CustomSetActive(curChargeData.discount > 0 && curChargeData.discount < 1);

            if (curChargeData.limitNum > 0)
            {
                TimesLabel.text = curChargeData.limitNum.ToString();
                TimesLabel.gameObject.CustomSetActive(true);
            }
            else
            {
                TimesLabel.gameObject.CustomSetActive(false);
            }


            ShowTag();
            ShowMask();
        }

        private void ShowMask()
        {
            if (curChargeData.cost == 0)
            {
                if (curChargeData.categoryValue == 3 && LTChargeManager.Instance.IsCouldGetDayFreeGift())
                {
                    Uimask.CustomSetActive(false);
                    boxcollider.enabled = true;

                    return;
                }
                if (curChargeData.categoryValue == 4 && LTChargeManager.Instance.IsCouldGetWeekFreeGift())
                {
                    Uimask.CustomSetActive(false);
                    boxcollider.enabled = true;
                    return;
                }
                if (curChargeData.categoryValue == 5 && LTChargeManager.Instance.IsCouldGetMonthFreeGift())
                {
                    Uimask.CustomSetActive(false);
                    boxcollider.enabled = true;
                    return;
                }
                Uimask.CustomSetActive(true);
                boxcollider.enabled = false;
            }
            else
            {
                Uimask.CustomSetActive(false);
                boxcollider.enabled = true;
            }
        }

        //显示标签
        private void ShowTag()
        {
            TagBGSp.gameObject.CustomSetActive(false);
            if (curChargeData.categoryValue == (int)LTChargeStoreController.EChargeType.eCharge)
            {
                TagObj.CustomSetActive(curChargeData.twoMultiple);
                if (curChargeData.twoMultiple)
                {
                    TagLab.text = EB.Localizer.GetString("ID_uifont_in_LTBountyTaskHudUI_Label_5");
                    TagBGSp.spriteName = "Mall_Label_2";
                }
            }
            else
            {
                if (curChargeData.buyLimit == 1)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYONLYONE");
                    TagBGSp.spriteName = "Mall_Label_2";
                }
                else if (curChargeData.buyLimit > 1)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = string.Format(EB.Localizer.GetString("ID_PURCHASE_LIMIT"), curChargeData.buyLimit);
                    TagBGSp.spriteName = "Mall_Label_2";
                }
                else if (curChargeData.dayBuyLimit > 0)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYEVERYDAY");
                    TagBGSp.spriteName = "Mall_Label_1";
                }
                else if (curChargeData.weeklyBuyLimit > 0)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYEVERYWEEK");
                    TagBGSp.spriteName = "Mall_Label_1";
                }
                else if (curChargeData.monthlyBuyLimit > 0)
                {
                    TagObj.CustomSetActive(true);
                    TagLab.text = EB.Localizer.GetString("ID_CHARGE_BUYEVERYMONTH");
                    TagBGSp.spriteName = "Mall_Label_1";
                }
                else
                {
                    TagObj.CustomSetActive(false);
                }
            }
            TagBGSp.gameObject.CustomSetActive(true);//解决锚点问题
        }

        public void OnClickItem()
        {
            if (onClickItem != null && curChargeData != null)
            {
                onClickItem(curChargeData);
            }
        }
    }

}