using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI
{
    public class LTWelfareLevelAwardController : DynamicMonoHotfix
    {
        public UIServerRequest m_ServerRequest;
        private int m_ActivityId = 3001;
        private TimeActivityExchangeTypeScrollItemData m_CurrentGotItemData;

        private LTWelfareLevelAwardScroll Scroll;

        private UIButton BuyButton;
        private UILabel BuyBtnLabel;

        private bool isOpen;
        public override void Awake()//ID_BUTTON_LABEL_BUY_PRIVILEGE
        {
            var t = mDMono.transform;
            isOpen = true;
            DataLookupsCache.Instance.SearchDataByID("isOpenEC", out isOpen);
            t.Find("BG/Texture").GetComponent<CampaignTextureCmp>().spriteName = "Welfare_People_1";
            base.Awake();
            Scroll=t.GetMonoILRComponent <LTWelfareLevelAwardScroll>("ScrollView/Placeholder/Grid");
            BuyButton = t.Find("BG/BuyButton").GetComponent<UIButton>();
            UILabel TipLabel = t.GetComponent<UILabel>("BG/TitleLabel");
            string tip = EB.Localizer.GetString("ID_WELFARE_PRIVILEGE_TIP1");
            TipLabel.text = tip;
            string[] splits = tip.Replace("            ", "|").Split('|');
            if (splits.Length > 1)
            {
                UILabel NumRootLabel = TipLabel.transform.GetComponent<UILabel>("NumRoot");
                NumRootLabel.text = splits[0];
            }
            BuyButton.onClick.Add(new EventDelegate(OnBuyBtnClick));
            BuyBtnLabel = BuyButton.transform.Find("Label").GetComponent<UILabel>();
            BuyButton.gameObject.CustomSetActive(isOpen);
            if (!isOpen)
            {
                t.Find("BG/TitleLabel").gameObject.CustomSetActive(false);
            }

        }
        bool isVip = false;
        public override void Start()
        {
            EB.Coroutines.Run(UpdateUI());
            isVip = LTChargeManager.Instance.IsSilverVIP();
            SetBuyBtnState();
            Hotfix_LT.Messenger.AddListener(EventName.LTWelfareHudOpen, ChangeUpdataUI);
            LTWelfareEvent.WelfareOnfocus += OnfocusFunc;
        }

        private void SetBuyBtnState()
        {
            if (isVip)
            {
                BuyButton.GetComponent<UISprite>().spriteName = "Ty_Button_2";
                BuyButton.GetComponent<BoxCollider>().enabled = false;
                BuyButton.GetComponent<UISprite>().color = Color.magenta;
                BuyBtnLabel.text = EB.Localizer.GetString("ID_PURCHASED");
            }
            else
            {
                BuyButton.GetComponent<UISprite>().spriteName = "Ty_Button_3";
                BuyButton.GetComponent<BoxCollider>().enabled = true;
                BuyButton.GetComponent<UISprite>().color = Color.white;
                BuyBtnLabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_BUY_PRIVILEGE");
            }
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.LTWelfareHudOpen, ChangeUpdataUI);
            LTWelfareEvent.WelfareOnfocus -= OnfocusFunc;
            base.OnDestroy();
        }

        bool updataUI = false;
        void ChangeUpdataUI()
        {
            updataUI = true;
        }

        void OnfocusFunc()
        {
            if (isVip != LTChargeManager.Instance.IsSilverVIP())
            {
                isVip = LTChargeManager.Instance.IsSilverVIP();
                SetBuyBtnState();
                EB.Coroutines.Run(UpdateUI());
            }
        }

        public override void OnEnable()
        {
            //base.OnEnable();
            if (updataUI)
            {
                updataUI = false;
                if (level != BalanceResourceUtil.GetUserLevel())
                {
                    EB.Coroutines.Run(UpdateUI());
                }
            }
        }

        int level = 0;
        IEnumerator UpdateUI()
        {
            yield return null;
            IDictionary cachedata;
            IDictionary extraCachedata;
            level = BalanceResourceUtil.GetUserLevel();
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.level_reward", out cachedata))
            {
                yield break;
            }
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("user_prize_data.level_reward_extra", out extraCachedata);
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(m_ActivityId);
            if (stages != null)
            {
                List<TimeActivityExchangeTypeScrollItemData> datas = new List<TimeActivityExchangeTypeScrollItemData>(stages.Count);
                for (int i = 0; i < stages.Count; i++)
                {
                    TimeActivityExchangeTypeScrollItemData data = new TimeActivityExchangeTypeScrollItemData();
                    data.id = stages[i].id;
                    data.sort = stages[i].sort;
                    data.stage_template = stages[i];
                    bool had = false;
                    bool extraHad = false;
                    had = EB.Dot.Bool(data.id.ToString(), cachedata, had);
                    extraHad = EB.Dot.Bool(data.id.ToString(), extraCachedata, extraHad);

                    if (had) data.had++;
                    if (extraHad) data.had++;

                    data.current = level;
                    data.total = level;
                    datas.Add(data);
                }
                InitLevelList(datas);
            }
        }
        
        private void InitLevelList(List<TimeActivityExchangeTypeScrollItemData> datas)
        {
            List<TimeActivityExchangeTypeScrollItemData> levelList = datas;
            Scroll.SetItemDatas(datas.ToArray());
        }
        
        public void OnGetReward(EB.Sparx.Response result)
        {
            LoadingSpinner.Hide();
            if (result.sucessful)
            {
                DataLookupsCache.Instance.CacheData("user_prize_data.level_reward." + m_CurrentGotItemData.id, true);
                ShowGotRewardItems();
            }
            else if (result.fatal)
            {
                SparxHub.Instance.FatalError(result.localizedError);
            }
            else
            {
                MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"), result.localizedError, EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, delegate (int ret)
                {
                }, NGUIText.Alignment.Center);
            }
        }

        void ShowGotRewardItems()
        {
            if (m_CurrentGotItemData != null && m_CurrentGotItemData.stage_template != null)
            {
                List<LTShowItemData> ItemDatas = new List<LTShowItemData>();
                List<ItemStruct> array = m_CurrentGotItemData.stage_template.reward_items;
                for (int i = 0; i < array.Count; i++)
                {
                    string id = array[i].id.ToString();
                    int count = array[i].quantity;
                    string type = array[i].type;
                    ItemDatas.Add(new LTShowItemData(id, count, type));
                }
                GlobalMenuManager.Instance.Open("LTShowRewardView", ItemDatas);//GameUtils.ShowAwardMsg(ItemDatas);
            }
        }

        void OnBuyBtnClick()
        {
            GlobalMenuManager.Instance.Open("LTChargeStoreHud", LTChargeStoreController.EChargeType.ePrivilege);
        }
    }
}
