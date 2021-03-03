using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 转盘活动面板
    /// </summary>
    public class LTActivityBodyItem_Turntable : LTActivityBodyItem_ScoreBase
    {
        public override void Awake()
        {
            base.Awake();
            NavButton = mDMono.transform.Find("ScrollView/Placeholder/TextContainer/NavButton").GetComponent<UIButton>();
            NavLabel = NavButton.transform.Find("Label").GetComponent<UILabel>();
            NavButton.onClick.Add(new EventDelegate(OnNavClick));
        }

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
            int stageid = stage.stage;
            var rewards = stage.reward_items;
            Transform itemRoot = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}", stageid));
            if (itemRoot == null)
            {
                var item = UIControllerHotfix.InstantiateEx(RewardTemplate, RewardTemplate.transform.parent, stageid.ToString());
                UILabel hint = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/HintLabel", stageid)).GetComponent<UILabel>();
                hint.text = EB.Localizer.GetString(string.Format("ID_ACTIVITY_TURNTABLE_{0}", stageid));
            }
            UIGrid GiftGrid = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/GiftGrid", stageid)).GetComponent<UIGrid>();
            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activity_id, out activityData);
            int hasGet = EB.Dot.Integer(string.Format("stages.{0}", stage.id), activityData, 0);
            for (var i = 0; i < rewards.Count; i++)
            {
                var rewardItem = UIControllerHotfix.InstantiateEx<LTShowItem>(ItemTemplate, GiftGrid.transform, stage.id.ToString());
                rewardItem.LTItemData = new LTShowItemData(rewards[i].id, rewards[i].quantity, rewards[i].type, false);
                rewardItem.mDMono.transform.Find("HasGet").gameObject.CustomSetActive(hasGet > 0);
            }
            GiftGrid.Reposition();
        }

        protected override void UpdateSubItem(Hotfix_LT.Data.TimeLimitActivityStageTemplate stage, int data) { }
    }
}