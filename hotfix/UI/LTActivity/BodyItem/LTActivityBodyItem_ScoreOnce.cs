using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 活动达成面板
    /// </summary>
    public class LTActivityBodyItem_ScoreOnce : LTActivityBodyItem_ScoreBase
    {
        public override void SetData(object data)
        {
            base.SetData(data);
            activity_id = EB.Dot.Integer("activity_id", data, 0);
            Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activity_id);
            if (activity == null) return;
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activity.id);

            List<string> parameter1sList = new List<string>();
            if (!string.IsNullOrEmpty(activity.parameter1))
            {
                string[] parameter1s = activity.parameter1.Split(',');
                for (int i = 0; i < parameter1s.Length; i++)
                {
                    parameter1sList.Add(parameter1s[i]);
                }
            }
            for (var i = 0; i < stages.Count; ++i)
            {
                CreateSubItem(stages[i]);
                if (!string.IsNullOrEmpty(activity.parameter1) && parameter1sList.Contains(stages[i].id.ToString()))
                {
                    int score = 0;
                    DataLookupsCache.Instance.SearchDataByID(string.Format("tl_acs.{0}.total", activity_id), out score);
                    if (score < stages[i].stage)
                    {
                        break;
                    }
                }
            }
            ItemGrid.Reposition();
        }

        protected override void UpdateSubItem(Hotfix_LT.Data.TimeLimitActivityStageTemplate stage, int data)
        {
            if (stage == null)
            {
                return;
            }

            int stageid = stage.id;
            Hashtable activityData;
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activity_id, out activityData);
            UILabel realmNumLabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/TotleTimesLabel", stageid)).GetComponent<UILabel>();
            int rewardgot = 0;
            int selfgot = EB.Dot.Integer(string.Format("stages.{0}", stageid), activityData, 0);
            int TotleNum = 1;
            if (stage.realm_num > 0)//服务器公共可领取次数
            {
                TotleNum = stage.realm_num;
                rewardgot = EB.Dot.Integer(string.Format("realm_num.{0}", stageid), activityData, 0);
                realmNumLabel.gameObject.CustomSetActive(true);
                realmNumLabel.text = string.Format(EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_NUM"), TotleNum - rewardgot);
            }
            int score = EB.Dot.Integer("total", activityData, 0);
            if (score >= stage.stage && (rewardgot >= TotleNum || selfgot > 0))
            {
                SetButtonToReceived(stageid, (selfgot > 0) ? null : EB.Localizer.GetString("ID_FINISHED"));
            }
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

            int score = EB.Dot.Integer("total", activityData, 0);

            UIButton btn = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton", stageid)).GetComponent<UIButton>();
            UILabel hint = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/HintLabel", stageid)).GetComponent<UILabel>();
            UILabel progress = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ProgressLabel", stageid)).GetComponent<UILabel>();
            UILabel realmNumLabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/TotleTimesLabel", stageid)).GetComponent<UILabel>();
            if (score >= stage.stage)
            {
                btn.gameObject.CustomSetActive(true);
                hint.gameObject.CustomSetActive(false);
                progress.gameObject.CustomSetActive(false);

                int rewardgot = 0;
                int selfgot = EB.Dot.Integer(string.Format("stages.{0}", stageid), activityData, 0);
                int TotleNum = 1;
                if (stage.realm_num > 0)//服务器公共可领取次数
                {
                    TotleNum = stage.realm_num;
                    rewardgot = EB.Dot.Integer(string.Format("realm_num.{0}", stageid), activityData, 0);
                    realmNumLabel.gameObject.CustomSetActive(true);
                    realmNumLabel.text = string.Format(EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_NUM"), TotleNum - rewardgot);
                }
                else
                {
                    TotleNum = stage.num;
                    rewardgot = selfgot;
                    realmNumLabel.gameObject.CustomSetActive(false);
                }

                if (rewardgot >= TotleNum || selfgot > 0)
                {
                    SetButtonToReceived(stageid, (selfgot > 0) ? null : EB.Localizer.GetString("ID_FINISHED"));
                }
                else
                {
                    UILabel btnlabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton/Label", stageid)).GetComponent<UILabel>();
                    btnlabel.text = EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
                    int curRealmNum = stage.realm_num;
                    btn.onClick.Add(
                        new EventDelegate(GetReward(stageid,
                            delegate () { SetButtonToReceived(stageid); },
                            delegate ()
                            {
                                UpdateAllSubItem(0);
                            })));

                }
            }
            else
            {
                btn.gameObject.CustomSetActive(false);
                hint.gameObject.CustomSetActive(true);
                progress.gameObject.CustomSetActive(true);
                Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activity_id);

                if (stage.activity_id == 2001 || stage.activity_id >= 6501 && stage.activity_id <= 6505)//累计充值
                {
                    if (activity.title != null) hint.text = string.Format(activity.title, (float)stage.stage / 100);
                    progress.text = string.Format("({0}/{1})", (float)score / 100, (float)stage.stage / 100);
                }
                //else if (stage.activity_id == 6506)//副本活动//改用大层后不再需要处理
                //{
                //    var data = SceneTemplateManager.Instance.GetLostChallengeChapterById(stage.stage);
                //    var data2 = SceneTemplateManager.Instance.GetLostChallengeChapterById(score);
                //    if (activity.title != null) hint.text = string.Format(activity.title, data.CurChapter);
                //    progress.text = string.Format("({0}/{1})", (data2 == null ? 0 : (data2.CurChapter - (data2.IsBoss ? 0 : 1))), data.CurChapter);
                //}
                else
                {
                    if (activity.title != null) hint.text = string.Format(activity.title, stage.stage);
                    progress.text = string.Format("({0}/{1})", score, stage.stage);
                }
                if (stage.realm_num > 0)//服务器公共可领取次数
                {
                    int rewardgot = 0;
                    int TotleNum = 1;
                    TotleNum = stage.realm_num;
                    rewardgot = EB.Dot.Integer(string.Format("realm_num.{0}", stageid), activityData, 0);
                    realmNumLabel.gameObject.CustomSetActive(true);
                    realmNumLabel.text = string.Format(EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_NUM"), TotleNum - rewardgot);
                }
            }
        }
    }
}