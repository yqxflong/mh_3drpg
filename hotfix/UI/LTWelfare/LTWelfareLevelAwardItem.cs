using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWelfareLevelAwardItem : DynamicCellController<TimeActivityExchangeTypeScrollItemData>
    {
        private UILabel m_StageLabel;
        private UIGrid ItemGrid;
        private List<LTShowItem> m_ShowItems;
        private UIButton m_ExchangeButton;
        private UILabel m_ExchangeButtonLabel;
        private TimeActivityExchangeTypeScrollItemData Data;
        private bool isOpen;
        public override void Awake()
        {
            isOpen = true;
            DataLookupsCache.Instance.SearchDataByID("isOpenEC", out isOpen);
            base.Awake();

            var t = mDMono.transform;
            ItemGrid = t.Find("GiftGrid").GetComponent<UIGrid>();
            m_ShowItems = new List<LTShowItem>();
            LTShowItem[] items = ItemGrid.transform.GetMonoILRComponentsInChildren<LTShowItem>("Hotfix_LT.UI.LTShowItem"); 

            for(int i=0;i< items.Length; i++)
            {
                m_ShowItems.Add(items[i]);
            }

            m_StageLabel = t.GetComponent<UILabel>("LevelSprite/Label"); 
            m_ExchangeButton = t.GetComponent<UIButton>("ReceiveButton");
            m_ExchangeButton.onClick.Add(new EventDelegate (ExchangeBtnClick));
            m_ExchangeButtonLabel = t.GetComponent<UILabel>("ReceiveButton/Label");
        }

        public void ExchangeBtnClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (Data.had == 0)
            {
                LTWelfareModel.Instance.ReceiveLevelAward(Data.id, delegate (bool sucessful)
                {
                    if (sucessful)
                    {
                        DataLookupsCache.Instance.CacheData("user_prize_data.level_reward." + Data.id, true);
                        Data.had ++;
                        ShowGotRewardItems();
                        Fill(Data);
                    }
                });
            }
            else if(Data.had == 1 && isOpen)
            {
                //判断是否买了特权
                if (LTChargeManager.Instance.IsSilverVIP())
                {
                    LTWelfareModel.Instance.ReceiveLevelAward(Data.id, delegate (bool sucessful)
                    {
                        if (sucessful)
                        {
                            DataLookupsCache.Instance.CacheData("user_prize_data.level_reward_extra." + Data.id, true);
                            Data.had++;
                            ShowGotRewardItems();
                            Fill(Data);
                        }
                    },1);
                }
                else
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_WELFARE_PRIVILEGE_TIP2"), delegate (int result)
                    {
                        if (result == 0)
                        {
                            GlobalMenuManager.Instance.Open("LTChargeStoreHud", LTChargeStoreController.EChargeType.ePrivilege);
                        }
                    });
                }
            }
        }
        void ShowGotRewardItems()
        {
            if (Data != null && Data.stage_template != null)
            {
                List<LTShowItemData> ItemDatas = new List<LTShowItemData>();
                List<ItemStruct> array = Data.stage_template.reward_items;
                for (int i = 0; i < array.Count; i++)
                {
                    string id = array[i].id.ToString();
                    int count = array[i].quantity;
                    string type = array[i].type;
                    ItemDatas.Add(new LTShowItemData(id, count, type));
                }
                for (int i = 0; i < ItemDatas.Count; i++)
                {
                    if (ItemDatas[i].id == "hc")
                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, ItemDatas[i].count, "福利等级礼包");
                    if (ItemDatas[i].id == "gold")
                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, ItemDatas[i].count, "福利等级礼包");
                }
                GlobalMenuManager.Instance.Open("LTShowRewardView", ItemDatas);
            }
        }

        public override void Fill(TimeActivityExchangeTypeScrollItemData itemData)
        {
            if (itemData == null)
            {
                Clean();
                return;
            }
            Data = itemData;
            Hotfix_LT.Data.TimeLimitActivityStageTemplate stagetpl = Data.stage_template;
            if (stagetpl == null) return;
            if (m_StageLabel != null) m_StageLabel.text = string.Format(EB.Localizer.GetString("ID_LEVEL_FORMAT"), stagetpl.stage.ToString());
            int rewardcount = stagetpl.reward_items.Count;
            for (int i = 0; i < m_ShowItems.Count; i++)
            {
                if (rewardcount > i)
                {
                    var item = stagetpl.reward_items[i];
                    m_ShowItems[i].LTItemData = new LTShowItemData(item.id, item.quantity, item.type, false);
                    m_ShowItems[i].mDMono.gameObject.SetActive(true);
                }
                else
                {
                    m_ShowItems[i].mDMono.gameObject.SetActive(false);
                }
            }

            if (stagetpl == null) return;
            if (Data.total < stagetpl.stage)
            {
                if (Data.current < stagetpl.stage)
                {
                    m_ExchangeButton.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                    m_ExchangeButton.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    m_ExchangeButton.isEnabled = false;
                    if (m_StageLabel != null) m_ExchangeButtonLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                }
                else
                {
                    m_ExchangeButton.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                    m_ExchangeButton.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                    m_ExchangeButton.isEnabled = true;
                    if (m_StageLabel != null) m_ExchangeButtonLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                }
                return;
            }
            else
            {
                if (Data.had == 0)
                {
                    m_ExchangeButton.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                    m_ExchangeButton.transform.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                    m_ExchangeButton.isEnabled = true;
                    if (m_StageLabel != null) m_ExchangeButtonLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                }
                else if (Data.had == 1 && isOpen)
                {
                    //额外机会
                    m_ExchangeButton.transform.GetComponent<UISprite>().color = new Color(1, 1, 1);
                    m_ExchangeButton.transform.GetComponent<UISprite>().spriteName = LTChargeManager.Instance.IsSilverVIP() ? "Ty_Button_3" : "Ty_Button_2";
                    m_ExchangeButton.isEnabled = true;
                    if (m_StageLabel != null) m_ExchangeButtonLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL_MORE");
                }
                else
                {
                    m_ExchangeButton.transform.GetComponent<UISprite>().color = new Color(1, 0, 1);
                    m_ExchangeButton.transform.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                    m_ExchangeButton.isEnabled = false;
                    if (m_StageLabel != null) m_ExchangeButtonLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
                }
                mDMono.gameObject.CustomSetActive(true);
            }
        }

        public override void Clean()
        {
            Data = null;
            mDMono.gameObject.CustomSetActive(false);
        }
    }
}