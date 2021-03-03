using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 活动兑换面板
    /// </summary>
    public class LTActivityBodyItem_ScoreRepeat : LTActivityBodyItem_ScoreBase
    {
        public override void SetData(object data)
        {
            base.SetData(data);
            activity_id = EB.Dot.Integer("activity_id", data, 0);
            Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activity_id);
            if (activity == null) return;
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activity.id);
            for (var i = 0; i < stages.Count; ++i)
            {
                CreateSubItem(stages[i]);
            }
            ItemGrid.Reposition();
        }

        protected override void CreateSubItem(Hotfix_LT.Data.TimeLimitActivityStageTemplate stage)
        {
            int stageid = stage.id;
            var rewards = stage.reward_items;
            var item = UIControllerHotfix.InstantiateEx(RewardTemplate, RewardTemplate.transform.parent, stageid.ToString());
            UIGrid GiftGrid = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/GiftGrid", stageid)).GetComponent<UIGrid>();

            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activity_id, out activityData);

            for (var i = 0; i < rewards.Count; i++)
            {
                var rewardItem = UIControllerHotfix.InstantiateEx<LTShowItem>(ItemTemplate, GiftGrid.transform, i.ToString());
                rewardItem.LTItemData = new LTShowItemData(rewards[i].id, rewards[i].quantity, rewards[i].type, false);
            }
            GiftGrid.Reposition();

            int score = EB.Dot.Integer("current", activityData, 0);

            UIButton btn = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton", stageid)).GetComponent<UIButton>();
            LTShowItem Source = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/Source", stageid)).GetMonoILRComponent<LTShowItem>();
            UILabel progress = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/Source/Count", stageid)).GetComponent<UILabel>();
            Source.LTItemData = new LTShowItemData(activity_id.ToString(), 1, "act", false);

            int rewardgot = EB.Dot.Integer(string.Format("stages.{0}", stageid), activityData, 0);
            UILabel gotLabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton/Count", stageid)).GetComponent<UILabel>();
            gotLabel.text = string.Format("({0}/{1})", rewardgot, stage.num);

            if (score >= stage.stage)
            {
                btn.isEnabled = true;
                progress.text = string.Format("{0}/{1}", score, stage.stage);


                if (rewardgot >= stage.num)
                {
                    SetButtonToReceived(stageid, EB.Localizer.GetString("ID_EXCHANGE"));
                }
                else
                {
                    UILabel btnlabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton/Label", stageid)).GetComponent<UILabel>();

                    btnlabel.text = EB.Localizer.GetString("ID_EXCHANGE");
                    btn.onClick.Add(new EventDelegate(GetReward(stageid, delegate ()
                    {
                        DataLookupsCache.Instance.SearchDataByID("tl_acs." + activity_id, out activityData);
                        score = EB.Dot.Integer("current", activityData, 0);
                        rewardgot = EB.Dot.Integer(string.Format("stages.{0}", stageid), activityData, 0);
                        UpdateAllSubItem(score);
                        gotLabel.text = string.Format("({0}/{1})", rewardgot, stage.num);
                        if (rewardgot >= stage.num)
                        {
                            SetButtonToReceived(stageid, EB.Localizer.GetString("ID_EXCHANGE"));
                        }
                        progress.text = string.Format("{0}/{1}", score, stage.stage);
                    })));
                }
            }
            else
            {
                SetButtonToNotReach(stageid, EB.Localizer.GetString("ID_EXCHANGE"));
                progress.text = string.Format("{0}/{1}", score, stage.stage);
            }
        }

        protected override void UpdateSubItem(TimeLimitActivityStageTemplate stage, int data)
        {
            if (state == null)
            {
                return;
            }

            var tLab = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/Source/Count", stage.id));

            if (tLab != null)
            {
                var progress = tLab.GetComponent<UILabel>();

                if (progress != null)
                {
                    progress.text = string.Format("{0}/{1}", data, stage.stage);
                }
            }

            var tBtn = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton", stage.id));

            if (tBtn != null)
            { 
                var btn = tBtn.GetComponent<UIButton>();

                if (btn != null && btn.isEnabled && data < stage.stage)
                {
                    SetButtonToNotReach(stage.id, EB.Localizer.GetString("ID_EXCHANGE"));
                }
            }
        }
    }
}