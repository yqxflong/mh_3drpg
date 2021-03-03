using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWelfareRechargeController : DynamicMonoHotfix
    {
        private UIButton Btn;
        private UILabel BtnText;
        private List<LTShowItem>TemplateItemList;
        private List<GameObject> HasGetObjList;
        private List<GameObject> FxObjList;

        public override void Awake()
        {
            Transform BGTrans = mDMono.transform.Find("BG");
            string lang = string.Empty;
            EB.Localizer.GetTexOrAtlasName("Welfare_Di_Shouchonglibao1_CN",out lang);
            BGTrans.GetComponent<CampaignTextureCmp>().spriteName = lang;
            base.Awake();
            Btn = mDMono.transform.Find("BuyButton").GetComponent<UIButton>();
            Btn.onClick.Add(new EventDelegate(OnBtnClick));

            BtnText = Btn.transform.GetChild(0).GetComponent<UILabel>();
            Transform Root = mDMono.transform.Find("GiftGrid");
            TemplateItemList = new List<LTShowItem>();

            for (int i = 0; i < Root.childCount; i++)
            {
                TemplateItemList.Add(Root.GetChild(i).GetMonoILRComponent<LTShowItem>());
            }

            Root= mDMono.transform.Find("GetRoot");
            HasGetObjList = new List<GameObject>();

            for (int i = 0; i < Root.childCount; i++)
            {
                HasGetObjList.Add(Root.GetChild(i).gameObject);
            }

            FxObjList = new List<GameObject>();

            for (int i = 0; i < BGTrans.childCount; i++)
            {
                FxObjList.Add(BGTrans.GetChild(i).gameObject);
            }

            LTWelfareEvent.WelfareOnfocus += UpdateInfo;
        }

        public override void OnDestroy()
        {
            LTWelfareEvent.WelfareOnfocus -= UpdateInfo;
            base.OnDestroy();
        }

        public override void Start()
        {
            base.Start();
            CreateAwardItem();
            UpdateInfo();
        }
        
        private void CreateAwardItem()
        {
            List<LTShowItemData> items = new List<LTShowItemData>();
            items.AddRange(WelfareTemplateManager.Instance.FirstChargeAward1Data.AwardItemList);
            items.AddRange(WelfareTemplateManager.Instance.FirstChargeAward2Data.AwardItemList);
            items.AddRange(WelfareTemplateManager.Instance.FirstChargeAward3Data.AwardItemList);

            for (int i=0;i< items.Count; i++)
            {
                if (TemplateItemList.Count > i)
                {
                    var item = items[i];
                    TemplateItemList[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                }
            }
        }

        public void UpdateInfo()
        {
            if (!mDMono.gameObject.activeInHierarchy)
            {
                return;
            }
            
            if (BalanceResourceUtil.ChargeHc > 0)
            {
                bool canReceive = false;
                for (int i = 0; i < FxObjList.Count; ++i)
                {
                    FxObjList[i].CustomSetActive(false);
                }
                if (LTWelfareDataManager.Instance.EverydayAwardData.FirstChargeLoginDay > 0 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift1)
                {
                    canReceive = true;
                    FxObjList[0].CustomSetActive(true);
                }
                else if (LTWelfareDataManager.Instance.EverydayAwardData.FirstChargeLoginDay > 1 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift2)
                {
                    canReceive = true;
                    FxObjList[1].CustomSetActive(true);
                }
                else if (LTWelfareDataManager.Instance.EverydayAwardData.FirstChargeLoginDay > 2 && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift3)
                {
                    canReceive = true;
                    FxObjList[2].CustomSetActive(true);
                }

                if (canReceive)
                {
                    Btn.transform.GetComponent<UISprite>().color = Color.white;
                    Btn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                    Btn.isEnabled = true;
                    BtnText.text = EB.Localizer.GetString("ID_RECEIVE_AWARD");
                }
                else
                {
                    Btn.transform.GetComponent<UISprite>().color = Color .magenta;
                    Btn.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    Btn.isEnabled = false;
                    BtnText.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                }
                HasGetObjList[0].CustomSetActive(LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift1);
                HasGetObjList[1].CustomSetActive(LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift2);
                HasGetObjList[2].CustomSetActive(LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift3);
            }
            else
            {
                Btn.isEnabled = true;
                BtnText.text = EB.Localizer.GetString("ID_WELFARE_SHOUCHONG_BTN"); 
            }
            Btn.gameObject.CustomSetActive(true);
        }

        public void OnBtnClick()
        {
            if (LTWelfareModel.Instance.Welfare_FirstCharge() && !LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift)
            {
                int day =0;
                if (!LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift1)
                {
                    LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift1 = true;
                    day=1;
                }
                else if(!LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift2)
                {
                    LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift2 = true;
                    day = 2;
                }
                else if (!LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift3)
                {
                    LTWelfareDataManager.Instance.EverydayAwardData.IsHaveReceiveFirstChargeGift3 = true;
                    day = 3;
                }

                LTWelfareModel.Instance.ReceiveFirstChargeGift(day,delegate (bool successful) {
                    if (successful)
                    {
                        if(LTWelfareEvent.WelfareHadFirstCharge!=null) LTWelfareEvent.WelfareHadFirstCharge();
                        List<LTShowItemData> ItemDatas;
                        if (day == 1)
                        {
                            ItemDatas = WelfareTemplateManager.Instance.FirstChargeAward1Data.AwardItemList;
                        }
                        else if (day == 2)
                        {
                            ItemDatas = WelfareTemplateManager.Instance.FirstChargeAward2Data.AwardItemList;
                        }
                        else if (day == 3)
                        {
                            ItemDatas = WelfareTemplateManager.Instance.FirstChargeAward3Data.AwardItemList;
                        }
                        else
                        {
                            ItemDatas = WelfareTemplateManager.Instance.FirstChargeAwardData.AwardItemList;
                        }

                        for (int i = 0; i < ItemDatas.Count; i++)
                        {
                            if (ItemDatas[i].id == "hc")
                                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, ItemDatas[i].count, "福利首充获得");
                            if (ItemDatas[i].id == "gold")
                                FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, ItemDatas[i].count, "福利首充获得");
                        }
                        GlobalMenuManager.Instance.Open("LTShowRewardView", ItemDatas);//GameUtils.ShowAwardMsg(WelfareTemplateManager.Instance.FirstChargeAwardData.AwardItemList);
                        UpdateInfo();
                    }
                });
            }
            else
            {
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTChargeStoreHud");
            }
        }
    }
}
