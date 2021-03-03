using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.UI
{
    public class InviteScrollDataLookUp : DataLookupHotfix
    {
        private const string taskdataid = "tasks";
        private eTaskType tasktype;
        private eInvitePageType pageType;
        private eTaskTabType tasktab;
        private InviteTaskGridScroll taskgrid;
        private List<int> curTaskIdList;
        private List<InviteTaskItemData> taskitemList;
        private bool hasInit = true;
        public override void Awake()
        {
            base.Awake();
            Transform t = mDL.transform;
            taskgrid = t.GetMonoILRComponent<InviteTaskGridScroll>();
        }

        public void SetTaskTypeAndParam(eTaskType tasktype, eInvitePageType pageType, eTaskTabType tasktab)
        {
            hasInit = false;
            this.tasktype = tasktype;
            this.pageType = pageType;
            this.tasktab = tasktab;
            mDL.DefaultDataID = taskdataid;
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            if (dataID == null) return;
            Hashtable taskdata = value as Hashtable;
            if (!hasInit)
            {              
                if (taskdata != null)
                {
                    if (taskitemList == null)
                    {
                        taskitemList = new List<InviteTaskItemData>();
                        curTaskIdList = new List<int>();
                    }
                    else
                    {
                        taskitemList.Clear();
                        curTaskIdList.Clear();
                    }
                    var enumerator = taskdata.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        DictionaryEntry v = enumerator.Entry;
                        IDictionary dic = v.Value as IDictionary;
                        int.TryParse(v.Key as string, out int taskid);
                        int taskType = EB.Dot.Integer("task_type", dic, 0);
                        SetData(taskid, taskType, dic);
                    }
                    //设置scrollview显示
                    curTaskIdList.Sort((x, y) => { return x - y; });
                    taskitemList.Sort((x, y) => { return x.taskid - y.taskid; });
                    taskgrid.SetItemDatas(taskitemList.ToArray());
                }
                hasInit = true;
            }
            else
            {
                if (taskdata == null) return;
                bool isNeedrefreshred = false;
                var enumerator = taskdata.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DictionaryEntry v = enumerator.Entry;
                    IDictionary dic = v.Value as IDictionary;
                    int.TryParse(v.Key as string, out int taskid);
                    if (taskid != 0)
                    {
                        int index = curTaskIdList.FindIndex(m => m == taskid);
                        if (index >= 0)
                        {
                            string state = EB.Dot.String("state", dic, null);
                            if (state != null && state.Equals(TaskSystem.COMPLETED))
                            {
                                taskitemList.RemoveAt(index);
                                curTaskIdList.RemoveAt(index);
                                isNeedrefreshred = true;                                
                            }
                            else if(state!=null && state.Equals(TaskSystem.FINISHED))
                            {
                                isNeedrefreshred = true;
                            }
                        }
                        else
                        {
                            int taskType = EB.Dot.Integer("task_type", dic, 0);
                            SetData(taskid, taskType, dic);
                        }
                    }
                }
                if(isNeedrefreshred) PlayerInviteManager.Instance.ReflashRedPoint();//通知红点刷新
                if (taskitemList.Count == 0)
                {
                    Messenger.Raise(EventName.OnInvitePlayerTaskStateChanged);//通知刷新页签
                }
                curTaskIdList.Sort((x, y) => { return x - y; });
                taskitemList.Sort((x, y) => { return x.taskid - y.taskid; });
                taskgrid.SetItemDatas(taskitemList.ToArray());
            }

        }

        private void SetData(int taskid,int taskType,IDictionary data)
        {
            if (taskType == (int)tasktype && PlayerInviteManager.GetTaskPage(taskid) == pageType)
            {
                string state = EB.Dot.String("state", data, null);
                if (state != null && state.Equals(TaskSystem.COMPLETED))
                {
                    return;
                }
                var tasktemplate = Data.TaskTemplateManager.Instance.GetTask(taskid);
                if (tasktemplate == null)
                {
                    EB.Debug.LogError("TaskTemolate == null is = {0}", taskid);
                    return;
                }
                if (PlayerInviteManager.GetInviteTaskTab(tasktemplate.target_parameter_1) == tasktab)
                {
                    InviteTaskItemData taskItemdata = new InviteTaskItemData(taskid, (int)tasktype, tasktemplate);
                    taskitemList.Add(taskItemdata);
                    curTaskIdList.Add(taskid);
                }
            }
        }

    }
}
