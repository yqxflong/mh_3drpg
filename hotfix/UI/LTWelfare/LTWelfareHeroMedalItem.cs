using UnityEngine;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTWelfareHeroMedalItem : DynamicCellController<Hotfix_LT.Data.HeroMedalTemplate>
    {
        UILabel StageLabel;
        UISprite StageSprite;
        GameObject HasGetSprite;
        GameObject HasGetExSprite;
        private List<LTShowItem> m_ShowItems;
        private List<LTShowItem> m_ExShowItems;

        UISprite BtnSprite;
        UILabel BtnLabel;

        Hotfix_LT.Data.HeroMedalTemplate data;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            StageLabel = t.GetComponent<UILabel>("StageLabel");
            StageSprite = t.GetComponent<UISprite>("StageSprite");
            HasGetSprite = StageSprite.transform.FindEx("HasGet").gameObject;
            HasGetExSprite = StageSprite.transform.FindEx("HasGetEx").gameObject;
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
                if (!LTWelfareModel.Instance.HeroMedalRewardType(data.stage))//没领取初始
                {
                    BtnSprite.color = new Color(1, 1, 1);
                    BtnSprite.spriteName = "Ty_Button_3";
                    BtnSprite.GetComponent<UIButton>().isEnabled = true;
                    BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                }
                else//已领取初始
                {
                    HasGetSprite.CustomSetActive(true);
                    bool hasMedal = LTWelfareModel.Instance.HasHeroMedal;
                    bool hasGet = LTWelfareModel.Instance.HeroMedalExRewardType(data.stage);
                    if (hasMedal && hasGet)//购买了密令且已领取
                    {
                        HasGetExSprite.CustomSetActive(true);
                        BtnSprite.color = new Color(1, 0, 1);
                        BtnSprite.spriteName = "Ty_Button_2";
                        BtnSprite.GetComponent<UIButton>().isEnabled = false;
                        BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                    }
                    else if (!hasMedal)//没购买了密令
                    {
                        BtnSprite.color = new Color(1, 1, 1);
                        BtnSprite.spriteName = "Ty_Button_2";
                        BtnSprite.GetComponent<UIButton>().isEnabled = true;
                        BtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL_AGAIN");

                    }
                    else//没买没领取
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
            if (LTWelfareModel.Instance.IsHeroMedalActivityOver())
            {
                return;
            }
            bool isEx = LTWelfareModel.Instance.HeroMedalRewardType(data.stage);
            if (isEx && !LTWelfareModel.Instance.HasHeroMedal)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_WELFARE_YINXIONG_SHOULD_BUY"));
                return;
            }
            LTWelfareModel.Instance.ReceiveHeroMedalAward(data.stage, isEx, delegate (bool sucessful)
            {
                if (sucessful)
                {
                    GlobalMenuManager.Instance.Open("LTShowRewardView", isEx ? data.ExItemList : data.ItemList);
                    ShowBtn(LTWelfareModel.Instance.HeroMedalStage);
                }
            });
        }

        public override void Fill(HeroMedalTemplate itemData)
        {
            if (itemData == null)
            {
                Clean();
                return;
            }
            if (data == null || data.stage != itemData.stage)
            {
                data = itemData;
                StageLabel.text = data.stage.ToString();
                ShowReward(m_ShowItems, data.ItemList);
                ShowReward(m_ExShowItems, data.ExItemList);
            }
            int curStage = LTWelfareModel.Instance.HeroMedalStage;
            StageSprite.color = (curStage >= data.stage) ? Color.white : Color.magenta;
            ShowBtn(curStage);
        }

        public override void Clean()
        {
            data = null;
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}
