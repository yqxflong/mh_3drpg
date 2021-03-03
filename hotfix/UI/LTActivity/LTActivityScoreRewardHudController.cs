using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTActivityScoreRewardHudController : UIControllerHotfix
    {
        private int activityid,score;
        private UILabel scorelabel,scoreshadow;
        private List<Data.TimeLimitActivityStageTemplate> stageslist;
        private List<ActivityScoreRewardItemData> itemlist;
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            scorelabel = t.GetComponent<UILabel>("Content/Des/score");
            scoreshadow = t.GetComponent<UILabel>("Content/Des/score/Label");
            DynamicScroll = t.GetMonoILRComponent<ActivityScoreRewardDynamicScroll>("Content/ScrollView/Placehodler/Grid");
            UIButton backButton = t.GetComponent<UIButton>("Frame/BG/Top/CloseBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
        }

        public override bool ShowUIBlocker { get { return true; } }

        public ActivityScoreRewardDynamicScroll DynamicScroll;

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            Messenger.AddListener(EventName.OnURScoreRewardRecieve, RefreshData);
            F_SetData();
        }
        public override IEnumerator OnRemoveFromStack()
        {
            Messenger.RemoveListener(EventName.OnURScoreRewardRecieve, RefreshData);
            yield return base.OnRemoveFromStack();
        }
        public override void SetMenuData(object param)
        {
            activityid = param != null ? (int)param : 0;
            InitData();
        }

        private void InitData()
        {
            stageslist = Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activityid);
            itemlist = new List<ActivityScoreRewardItemData>(stageslist.Count);
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activityid, out Hashtable activityData);
            score = EB.Dot.Integer("current", activityData, 0);
            scorelabel.text = scoreshadow.text = score.ToString();
            for (int i = 0; i < stageslist.Count; i++)
            {
                var tempstage = stageslist[i];
                int ewardgot = EB.Dot.Integer(string.Format("stages.{0}", tempstage.id), activityData, 0);
                if(ewardgot == 0 && score >= tempstage.stage)
                {
                    ewardgot = -1;
                }
                List<LTShowItemData> showitemlist = new List<LTShowItemData>();
                for (int j = 0; j < tempstage.reward_items.Count; j++)
                {
                    var rewardstruct = tempstage.reward_items[j];
                    showitemlist.Add(new LTShowItemData(rewardstruct.id, rewardstruct.quantity, rewardstruct.type));
                }
                itemlist.Add(new ActivityScoreRewardItemData(activityid,tempstage.id,tempstage.stage, ewardgot, showitemlist));
            }
            itemlist.Sort(RewardDataSort);
        }

        private int RewardDataSort(ActivityScoreRewardItemData x, ActivityScoreRewardItemData y)
        {
            int sortvalue = 0;
            sortvalue = x.state - y.state;
            if (sortvalue != 0)
            {
                return sortvalue;
            }
            sortvalue = x.stage - y.stage;
            return sortvalue;
        }

        private void RefreshData()
        {
            DataLookupsCache.Instance.SearchDataByID("tl_acs." + activityid, out Hashtable activityData);
            for (int i = 0; i < itemlist.Count; i++)
            {
                var tempdata = itemlist[i];
                int ewardgot = EB.Dot.Integer(string.Format("stages.{0}", tempdata.id), activityData, 0);
                if (ewardgot == 0 && score >= tempdata.stage)
                {
                    ewardgot = -1;
                }
                tempdata.RefreshState(ewardgot);
            }
            itemlist.Sort(RewardDataSort);
            F_SetData();
        }

        public void F_SetData()
        {
            DynamicScroll.SetItemDatas(itemlist.ToArray());
        }
    }
}

