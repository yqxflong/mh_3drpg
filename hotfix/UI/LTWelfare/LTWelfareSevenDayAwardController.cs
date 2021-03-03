using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWelfareSevenDayAwardController : DynamicMonoHotfix
    {
        private List<LTWelfareSevenDayAwardItem> Items;
        private UILabel DayLabel;

        private int ActivityId = 6001;

        public override void Awake()
        {
            mDMono.transform.Find("BG").GetComponent<CampaignTextureCmp>().spriteName = "Welfare_Di_Qiridenglu";
            base.Awake();
            Items = new List<LTWelfareSevenDayAwardItem>();
            DayLabel = mDMono.transform.Find("BG/TotleDayTitle").GetComponent<UILabel>();
        }

        public override void Start()
        {
            EB.Coroutines .Run( InitData());
        }
        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.LTWelfareHudOpen, InitItems);
            base.OnDestroy();
        }

        IEnumerator  InitData()
        {
            yield return null;
            DynamicMonoILR[] items = mDMono.transform.Find("GiftGrid").GetComponentsInChildren<DynamicMonoILR>();
            for (int i = 0; i < items.Length; i++)
            {
                LTWelfareSevenDayAwardItem temp = items[i]._ilrObject as LTWelfareSevenDayAwardItem;
                if (temp != null) Items.Add(temp);
            }
            InitItems();
            Hotfix_LT.Messenger.AddListener(EventName.LTWelfareHudOpen, InitItems);
        }

        int curIdc = 0;
        void InitItems()
        {
            if (curIdc == LoginDayCount) return;
            curIdc = LoginDayCount;
            DayLabel.text = string.Format(EB.Localizer.GetString("ID_WELFARE_QIRI_LEIJI"), curIdc);
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(ActivityId);
            for (int i = 0; i < Items.Count; ++i)
            {
                Hotfix_LT.Data.TimeLimitActivityStageTemplate data = FindStageTpl(stages, i + 1);
                List<LTShowItemData> itemDatas = new List<LTShowItemData>();
                eReceiveState state = eReceiveState.cannot;
                if (data != null && data.reward_items != null)
                {
                    for (int j = 0; j < data.reward_items.Count; j++)
                    {
                        string id = data.reward_items[j].id.ToString();
                        int count = data.reward_items[j].quantity;
                        string type = data.reward_items[j].type;
                        itemDatas.Add(new LTShowItemData(id, count, type, false));
                    }
                    if (i + 1 <= curIdc)
                    {
                        if (GetReceiveState(i + 1))
                            state = eReceiveState.have;
                        else
                            state = eReceiveState.can;
                    }
                    else
                        state = eReceiveState.cannot;

                }
                Items[i].SetUI(new RewardStageData(data.id, i + 1, itemDatas, state), LTWelfareSevenDayAwardItem.ItemType.Welfare);
            }
        }

        private Hotfix_LT.Data.TimeLimitActivityStageTemplate FindStageTpl(List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> datas,int stage)
        {
            Hotfix_LT.Data.TimeLimitActivityStageTemplate data = new Hotfix_LT.Data.TimeLimitActivityStageTemplate();
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].stage == stage) { data = datas[i]; break; }
            }
            return data;
        }

        private int LoginDayCount
        {
            get
            {
                int ldc=0;
                DataLookupsCache.Instance.SearchIntByID("user_prize_data.daily_login.login_day_count", out ldc);
                return ldc;
            }
        }

        private bool GetReceiveState(int day)
        {
            bool state = false;
            DataLookupsCache.Instance.SearchDataByID<bool>( string .Format("user_prize_data.sevenday_reward.{0}{1:00}" ,ActivityId ,day), out state);
            return state;
        }

        public bool GetReceiveable()
        {
            int ldc = LoginDayCount;
            for (int day = 1; day < 8; ++day)
            {
                if (day <= ldc)
                {
                    if (!GetReceiveState(day))
                        return true;
                }
                else
                    return false;
            }
            return false;
        }

    }
}
