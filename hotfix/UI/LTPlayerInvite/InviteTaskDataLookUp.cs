using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.UI
{
    public class InviteTaskDataLookUp : DataLookupHotfix
    {
        private UILabel taskfinishTimes, taskfinishTimesShadow/*, buttonTip, buttonTipShadow*/;
        private ConsecutiveClickCoolTrigger GetBtn;
        private int taskId;
        private GameObject tiplabelobj, getbuttonobj;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDL.GetComponent<Transform>();
            taskfinishTimes = t.GetComponent<UILabel>("TipLabel");
            tiplabelobj = t.GetComponent<Transform>("TipLabel").gameObject;
            taskfinishTimesShadow = t.GetComponent<UILabel>("TipLabel/Label(Clone)");
            //buttonTip = t.GetComponent<UILabel>("GetButton/Label");
            getbuttonobj = t.GetComponent<Transform>("GetButton").gameObject;
            //buttonTipShadow = t.GetComponent<UILabel>("GetButton/Label/Label (1)");
            GetBtn = t.GetComponent<ConsecutiveClickCoolTrigger>("GetButton");
            GetBtn.clickEvent.Add(new EventDelegate(CompleteTask));
        }

        public void SetTaskData(int taskid,string defaultDataId)
        {
            mDL.DefaultDataID = defaultDataId;
            taskId = taskid;
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            int targetnum;
            int curnum;
            targetnum = EB.Dot.Integer("event_count.target_num", value, 0);
            curnum = EB.Dot.Integer("event_count.current_num", value, 0);
            bool hasfinish = curnum >= targetnum;

            if (!hasfinish)
            {
                taskfinishTimes.text = taskfinishTimesShadow.text = string.Format("{0}/{1}", curnum, targetnum);

            }
            getbuttonobj.CustomSetActive(hasfinish);
            tiplabelobj.CustomSetActive(!hasfinish);
        }

        //private void SetTaskBtnState(string state)
        //{
        //    if (state != null)
        //    {
        //        buttonTip.text = buttonTipShadow.text = state.Equals(TaskSystem.RUNNING)||state.Equals(TaskSystem.FINISHED) ?
        //            EB.Localizer.GetString("ID_BUTTON_LABEL_PULL"):EB.Localizer.GetString("ID_BUTTON_LABEL_HAD_PULL");
        //        LTUIUtil.SetGreyButtonEnable(GetBtn, state.Equals(TaskSystem.FINISHED));
        //    }
        //}

        private void CompleteTask()
        {
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 10f);
            LTHotfixManager.GetManager<TaskManager>().RequestComplete(taskId.ToString(), OnCompleteTask);
        }

        public void OnCompleteTask(EB.Sparx.Response result)
        {
            InputBlockerManager.Instance.UnBlock(InputBlockReason.FUSION_BLOCK_UI_INTERACTION);
            if (result.sucessful)
            {
                if (result.hashtable != null)
                {
                    DataLookupsCache.Instance.CacheData(result.hashtable);
                }
                List<LTShowItemData> mlist = TaskStaticData.GetItemRewardList(taskId);
                FusionTelemetry.ItemsUmengCurrency(mlist, "任务获得");
                Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskId);
                FusionTelemetry.TaskData.PostEvent(taskId, taskTpl.task_name, taskTpl.task_type.ToString());
                if (mlist.Count > 0)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("reward", mlist);
                    GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                }
            }
            else
            {
                result.CheckAndShowModal();
            }
        }
    }
}
