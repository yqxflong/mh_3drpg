using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 几日登陆活动面板
    /// </summary>
    public class LTActivityBodyItem_SevenDay : LTActivityBodyItem
    {
        private UIGrid GiftGrid;
        private List<LTWelfareSevenDayAwardItem> Items;

        public override void Awake()
        {
            base.Awake();

            GiftGrid = mDMono.transform.Find("GiftGrid").GetComponent<UIGrid>();
            GiftGrid.enabled = false;
        }

        public override void SetData(object netData)
        {
            base.SetData(netData);
            //
            Items = new List<LTWelfareSevenDayAwardItem>();
            DynamicMonoILR[] items = mDMono.transform.Find("GiftGrid").GetComponentsInChildren<DynamicMonoILR>();
            for (int i = 0; i < items.Length; i++)
            {
                LTWelfareSevenDayAwardItem temp = items[i]._ilrObject as LTWelfareSevenDayAwardItem;
                if (temp != null)
                {
                    Items.Add(temp);
                    temp.mDMono.gameObject.SetActive(false);
                }
            }
            //
            int ActivityId = EB.Dot.Integer("activity_id", netData, 0);
            //
            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + ActivityId, out activityData);
            int LoginDayCount = EB.Dot.Integer("total", activityData, 0);
            //获取相应连续几日活动的配置数据
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(ActivityId);
            for (int i = 0; i < Items.Count; ++i)
            {
                if (stages.Count > i)
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
                        if (i + 1 <= LoginDayCount)
                        {
                            if (GetReceiveState(stages[i].id, activityData))
                                state = eReceiveState.have;
                            else
                                state = eReceiveState.can;
                        }
                        else
                            state = eReceiveState.cannot;

                    }
                    Items[i].SetUI(new RewardStageData(stages[i].id, stages[i].activity_id, stages[i].stage, itemDatas, state), LTWelfareSevenDayAwardItem.ItemType.Activity);
                    Items[i].mDMono.gameObject.SetActive(true);
                }
            }
            //GiftGrid.Reposition();
            SetItemsPos(stages.Count);

        }

        private void SetItemsPos(int stagesCount)
        {
            int RowCount = GiftGrid.maxPerLine;
            int Yu = stagesCount % RowCount;
            for (int i = 0; i < (stagesCount / RowCount) + (Yu > 0 ? 1 : 0); i++)
            {
                for (int j = 0; j < RowCount; j++)
                {
                    if (stagesCount > i * RowCount + j)
                    {
                        Items[i * RowCount + j].mDMono.transform.localPosition = new Vector3(GiftGrid.cellWidth * j, -GiftGrid.cellHeight * i, 0);
                    }
                }
            }

            int TPos = (int)((RowCount - Yu) * (GiftGrid.cellWidth / 2));

            for (int i = (stagesCount / RowCount) * RowCount; i < stagesCount; i++)
            {
                Items[i].mDMono.gameObject.transform.localPosition += new Vector3(TPos, 0, 0);
            }
        }

        private Hotfix_LT.Data.TimeLimitActivityStageTemplate FindStageTpl(List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> datas, int stage)
        {
            Hotfix_LT.Data.TimeLimitActivityStageTemplate data = new Hotfix_LT.Data.TimeLimitActivityStageTemplate();
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i].stage == stage) { data = datas[i]; break; }
            }
            return data;
        }

        private bool GetReceiveState(int id, Hashtable activityData)
        {
            int rewardgot = EB.Dot.Integer(string.Format("stages.{0}", id), activityData, 0);
            return rewardgot > 0;
        }

    }
}