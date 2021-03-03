using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWelfareSevenDayAwardItem : DynamicMonoHotfix
    {
        private LTShowItem Item;
        private ConsecutiveClickCoolTrigger Btn;
        private UILabel DayLabel;
        private GameObject FxObj;
        private GameObject hasReceiveObj;

        private RewardStageData StageDta;

        public enum ItemType
        {
            Welfare=0,
            Activity,
            ComeBack,
        }
        
        private ItemType m_Type;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Item = t.GetMonoILRComponent<LTShowItem>("LTShowItem");
            Btn = t.GetComponent<ConsecutiveClickCoolTrigger>("ReceiveBtn");
            Btn.clickEvent.Add(new EventDelegate(OnReceiveBtnClick));
            DayLabel = t.GetComponent<UILabel>("DayLabel");
            hasReceiveObj = t.FindEx("SelectUI").gameObject;
            FxObj = t.FindEx("FXObj").gameObject;
        }

        public void SetUI(RewardStageData sd, LTWelfareSevenDayAwardItem.ItemType Type)
        {
            m_Type = Type;
            this.StageDta = sd;

            Item.LTItemData = sd.Awards[0];
            DayLabel.text = string.Format(EB.Localizer.GetString("ID_DAY"), sd.Stage);
            UpdateReceiveState(sd.ReceiveState);
            mDMono.gameObject.CustomSetActive(true);
        }

        private void UpdateReceiveState(eReceiveState state)
        {
            switch (state)
            {
                case eReceiveState.cannot:
                    FxObj.CustomSetActive(false);
                    hasReceiveObj.CustomSetActive(false);
                    Btn.gameObject.CustomSetActive(false);
                    break;
                case eReceiveState.can:
                    FxObj.CustomSetActive(true);
                    hasReceiveObj.CustomSetActive(false);
                    Btn.gameObject.CustomSetActive(true);
                    break;
                case eReceiveState.have:
                    FxObj.CustomSetActive(false);
                    hasReceiveObj.CustomSetActive(true);
                    Btn.gameObject.CustomSetActive(false);
                    break;
            }
        }

        public void OnReceiveBtnClick()
        {
            switch (m_Type)
            {
                case ItemType.Welfare:
                    {
                        LTWelfareModel.Instance.ReceiveSevendayAward(StageDta.Id, delegate (bool successful)
                        {
                            if (successful)
                            {
                                UpdateReceiveState(eReceiveState.have);
                                string cachePath = string.Format("user_prize_data.sevenday_reward.{0}", StageDta.Id);
                                DataLookupsCache.Instance.CacheData(cachePath, true);
                                for (int i = 0; i < StageDta.Awards.Count; i++)
                                {
                                    if (StageDta.Awards[i].id == "hc") 
                                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, StageDta.Awards[i].count, "福利七日登录");
                                    if (StageDta.Awards[i].id == "gold")
                                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, StageDta.Awards[i].count, "福利七日登录");
                                }
                                GlobalMenuManager.Instance.Open("LTShowRewardView", StageDta.Awards);
                            }
                        });
                    }; break;
                case ItemType.Activity:
                    {
                        EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/gotReward");
                        request.AddData("activityId", StageDta.ActivityId);
                        request.AddData("stageId", StageDta.Id);
                        LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
                        {
                            if (data != null)
                            {
                                UpdateReceiveState(eReceiveState.have);
                                DataLookupsCache.Instance.CacheData(data);
                                for (int i = 0; i < StageDta.Awards.Count; i++)
                                {
                                    if (StageDta.Awards[i].id == "hc")
                                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, StageDta.Awards[i].count, "福利七日登录");
                                    if (StageDta.Awards[i].id == "gold")
                                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, StageDta.Awards[i].count, "福利七日登录");
                                }
                                List<LTShowItemData> awardDatas = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("reward", data, null));
                                GlobalMenuManager.Instance.Open("LTShowRewardView", awardDatas);
                            }
                        });
                    }; break;
                case ItemType.ComeBack:
                    {
                        EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/sign_in/drawAfterDaysLoginReward");
                        request.AddData("achievementId", StageDta.Id);
                        LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
                        {
                            if (data != null)
                            {
                                UpdateReceiveState(eReceiveState.have);
                                string cachePath = string.Format("user_prize_data.afterdays_login_reward.{0}", StageDta.Id);
                                DataLookupsCache.Instance.CacheData(cachePath, true);
                                DataLookupsCache.Instance.CacheData(data);
                                for (int i = 0; i < StageDta.Awards.Count; i++)
                                {
                                    if (StageDta.Awards[i].id == "hc")
                                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, StageDta.Awards[i].count, "福利七日登录");
                                    if (StageDta.Awards[i].id == "gold")
                                        FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.gold, StageDta.Awards[i].count, "福利七日登录");
                                }
                                GlobalMenuManager.Instance.Open("LTShowRewardView", StageDta.Awards);
                            }
                        });
                    };break;
            }
        }
    }
}
