using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTWelfareMainInstanceItem : DynamicCellController<MainCampaignRewardTemplate>
    {
        UILabel StageLabel;
        UISprite StageSprite;
        GameObject StageObj;
        GameObject HasGetSprite;
        GameObject HasGetExSprite;
        GameObject LockObj;
        private List<LTShowItem> m_ShowItems;
        private List<LTShowItem> m_ExShowItems;

        UISprite BtnSprite;
        UILabel BtnLabel;

        Hotfix_LT.Data.MainCampaignRewardTemplate data;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            StageLabel = t.GetComponent<UILabel>("StageLabel");
            StageSprite = t.GetComponent<UISprite>("StageLabel/StageSprite");
            StageObj = t.Find("StageLabel/StageSprite/BG").gameObject;
            HasGetSprite = t.FindEx("BG/HasGet").gameObject;
            HasGetExSprite = t.FindEx("EXBG/HasGetEx").gameObject;
            LockObj = t.FindEx("EXBG/Lock").gameObject;
            m_ShowItems = new List<LTShowItem>();
            LTShowItem[] items = t.FindEx("GiftGrid").GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem");

            for (int i = 0; i < items.Length; i++)
            {
                m_ShowItems.Add(items[i]);
            }

            m_ExShowItems = new List<LTShowItem>();
            LTShowItem[] exitems = t.FindEx("ExGiftGrid").GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem");

            for (int i = 0; i < exitems.Length; i++)
            {
                m_ExShowItems.Add(exitems[i]);
            }

            BtnSprite = t.GetComponent<UISprite>("ReceiveButton");
            BtnSprite.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnBtnClick));
            BtnLabel = BtnSprite.transform.GetComponent<UILabel>("Label");
        }

        public override void Clean()
        {
            data = null;
            mDMono.gameObject.CustomSetActive(false);
        }

        public override void Fill(MainCampaignRewardTemplate tempData)
        {
            if (tempData == null)
            {
                Clean();
                return;
            }
            if (data == null || data.stage != tempData.stage)
            {
                data = tempData;
                StageLabel.text =string.Format ( EB.Localizer .GetString("ID_uifont_in_LTGetHeroView_Label_1"), data.stage%100);
                ShowReward(m_ShowItems, data.ItemList);
                ShowReward(m_ExShowItems, data.ExItemList);
            }
            
            int curStage =LTWelfareModel.Instance .GetMaxCampaignLevel();
            if (curStage >= data.stage)
            {
                StageSprite.spriteName = "Welfare_Zhuxianjijing_Di_2";
                StageObj.CustomSetActive(true);
            }
            else
            {
                StageSprite.spriteName = "Welfare_Zhuxianjijing_Di_3";
                StageObj.CustomSetActive(false);
            }
            LockObj.CustomSetActive(!LTWelfareModel.Instance.HasMain);
            ShowBtn(curStage);
            mDMono.gameObject.CustomSetActive(true);
        }

        void ShowReward(List<LTShowItem> items, List<LTShowItemData> datas)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (datas.Count > i)
                {
                    var item = datas[i];
                    items[i].LTItemData = new LTShowItemData(item.id, item.count, item.type, false);
                    items[i].mDMono.gameObject.CustomSetActive(true);
                }
                else
                {
                    items[i].mDMono.gameObject.CustomSetActive(false);
                }
            }
        }

        //未完成，可领取，购买勋章，可领取，已领取
        void ShowBtn(int curStage)
        {
            HasGetSprite.CustomSetActive(false);
            HasGetExSprite.CustomSetActive(false);
            if (curStage < data.stage)//没达到条件
            {
                BtnSprite.color = new Color(1, 0, 1);
                BtnSprite.spriteName = "Ty_Button_2";
                BtnSprite.GetComponent<UIButton>().isEnabled = false;
                BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
            }
            else//达到条件
            {
                if (!LTWelfareModel.Instance.MainRewardType(data.stage))//没领取初始
                {
                    BtnSprite.color = new Color(1, 1, 1);
                    BtnSprite.spriteName = "Ty_Button_3";
                    BtnSprite.GetComponent<UIButton>().isEnabled = true;
                    BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                }
                else//已领取初始
                {
                    HasGetSprite.CustomSetActive(true);
                    bool hasMain = LTWelfareModel.Instance.HasMain;
                    bool hasGet = LTWelfareModel.Instance.MainExRewardType(data.stage);
                    if (hasMain && hasGet)//购买了密令且已领取
                    {
                        HasGetExSprite.CustomSetActive(true);
                        BtnSprite.color = new Color(1, 0, 1);
                        BtnSprite.spriteName = "Ty_Button_2";
                        BtnSprite.GetComponent<UIButton>().isEnabled = false;
                        BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                    }
                    else if (!hasMain)//没购买了密令
                    {
                        BtnSprite.color = new Color(1, 1, 1);
                        BtnSprite.spriteName = "Ty_Button_2";
                        BtnSprite.GetComponent<UIButton>().isEnabled = true;
                        BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL_AGAIN");

                    }
                    else//没领取
                    {
                        BtnSprite.color = new Color(1, 1, 1);
                        BtnSprite.spriteName = "Ty_Button_3";
                        BtnSprite.GetComponent<UIButton>().isEnabled = true;
                        BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL_AGAIN");
                    }
                }
            }
        }

        //领取奖励点击
        private void OnBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            bool isEx = LTWelfareModel.Instance.MainRewardType(data.stage);
            if (isEx && !LTWelfareModel.Instance.HasMain)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_WELFARE_MAIN_CAMPAIGN_BUY"));
                return;
            }
            LTWelfareModel.Instance.ReceiveMainAward(data.stage, isEx, delegate (bool sucessful)
            {
                if (sucessful)
                {
                    GlobalMenuManager.Instance.Open("LTShowRewardView", isEx ? data.ExItemList : data.ItemList);
                    int maxLevel = LTInstanceMapModel.Instance.GetMaxCampaignLevel();
                    int curStage = maxLevel == 13010 ? 30 : (maxLevel / 100) % 100;
                    ShowBtn(curStage);
                }
            });
        }
    }
}
