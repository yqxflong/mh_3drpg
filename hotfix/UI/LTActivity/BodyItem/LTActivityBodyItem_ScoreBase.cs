using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 活动滚动item面板——基类
    /// </summary>
    public abstract class LTActivityBodyItem_ScoreBase : LTActivityBodyItem
    {
        protected GameObject RewardTemplate;
        protected UIGrid ItemGrid;
        protected LTShowItem ItemTemplate;
        protected int activity_id;

        public override void Awake()
        {
            base.Awake();
            desc = mDMono.transform.Find("ScrollView/Placeholder/TextContainer/DESC").GetComponent<UILabel>();
            Event_Time = mDMono.transform.Find("ScrollView/Placeholder/TextContainer/EventTime").GetComponent<UILabel>();
            RewardTemplate = mDMono.transform.Find("ScrollView/Placeholder/Grid/Item").gameObject;
            ItemGrid = mDMono.transform.Find("ScrollView/Placeholder/Grid").GetComponent<UIGrid>();
            ItemTemplate = mDMono.transform.Find("ScrollView/Placeholder/Grid/Item/GiftGrid/ShowItem").GetMonoILRComponent<LTShowItem>();
            RewardTemplate.CustomSetActive(false);
            ItemTemplate.mDMono.gameObject.CustomSetActive(false);
        }

        protected abstract void CreateSubItem(Hotfix_LT.Data.TimeLimitActivityStageTemplate stage);
        protected abstract void UpdateSubItem(Hotfix_LT.Data.TimeLimitActivityStageTemplate stage, int data);
        protected void UpdateAllSubItem(int data)
        {
            Hotfix_LT.Data.TimeLimitActivityTemplate activity = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activity_id);
            if (activity == null) return;
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activity.id);
            for (var i = 0; i < stages.Count; ++i)
            {
                UpdateSubItem(stages[i], data);
            }
        }

        protected void SetButtonToReceived(int stage, string btnStr = null)
        {
            UILabel btnlabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton/Label", stage)).GetComponent<UILabel>();

            if (btnlabel != null)
            { 
                btnlabel.text = (btnStr != null) ? btnStr : EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
            }

            UIButton btn = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton", stage)).GetComponent<UIButton>();

            if (btn != null)
            {
                btn.SetState(UIButtonColor.State.Disabled, true);
                btn.isEnabled = false;
                btn.onClick.Clear();
            }
        }

        protected void SetButtonToNotReach(int stage, string btnStr = null)
        {
            UILabel btnlabel = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton/Label", stage)).GetComponent<UILabel>();
            btnlabel.text = (btnStr != null) ? btnStr : EB.Localizer.GetString("ID_BUTTON_LABEL_PULL");
            UIButton btn = mDMono.transform.Find(string.Format("ScrollView/Placeholder/Grid/{0}/ReceiveButton", stage)).GetComponent<UIButton>();
            btn.SetState(UIButtonColor.State.Disabled, true);
            btn.isEnabled = false;
            btn.onClick.Clear();
        }

        protected EventDelegate.Callback GetReward(int stage, System.Action refresh, System.Action err = null)
        {
            return delegate ()
            {
                LTHotfixApi.GetInstance().ExceptionFunc = (EB.Sparx.Response response) =>
                {
                    if (response.error != null)
                    {
                        string strObjects = (string)response.error;
                        string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                        switch (strObject[0])
                        {
                            case "insufficient num":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ACTIVITY_REALM_CHALLENGE_ERROR"));
                                LTMainHudManager.Instance.UpdateActivityLoginData(err);
                                return true;
                            }
                        }
                    }
                    return false;
                };
                EB.Sparx.Request request = LTHotfixApi.GetInstance().Post("/specialactivity/gotReward");
                request.AddData("activityId", activity_id);
                request.AddData("stageId", stage);
                LTHotfixApi.GetInstance().BlockService(request, delegate (Hashtable data)
                {
                    DataLookupsCache.Instance.CacheData(data);
                    refresh();
                    title.UpdateRedPoint();
                    List<LTShowItemData> awardDatas = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("reward", data, null));
                    GlobalMenuManager.Instance.Open("LTShowRewardView", awardDatas);
                });
            };
        }
    }
}