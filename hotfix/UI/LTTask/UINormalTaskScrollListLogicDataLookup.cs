using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class UINormalTaskScrollListLogicDataLookup : UIScrollListLogicDataLookup
{
    public UINormalTaskScroll m_GridScroll;
    public GameObject m_DefaultTip;

    public override void Awake()
    {
	    PageSettings = new List<PageSettingsValue>();
	    PageSettingsValue pageSettingsValue= new PageSettingsValue();
	    pageSettingsValue.DataID = "tasks";
	    InspectablePredicate inspectablePredicate = new InspectablePredicate();
	    inspectablePredicate.Query = new List<InspectablePredicate.Part>();
	    InspectablePredicate.Part part = new InspectablePredicate.Part();
	    part.PreviousPartLogicOperator = InspectablePredicate.LogicalOperatorValue.AND;
	    part.PropertyName = "task_type";
	    part.ComparaisonOperator = InspectablePredicate.ComparaisonOperatorValue.EQUAL_TO;
	    part.Value = "3";
	    inspectablePredicate.Query.Add(part);
	    pageSettingsValue.InspectableFilter = inspectablePredicate;
	    PageSettings.Add(pageSettingsValue);
	    m_GridScroll = mDL.transform.GetMonoILRComponent<UINormalTaskScroll>("Placeholder/Grid");
	    base.Awake();
    }

    public static class SortingComparisonHandler
    {
		public static int task_id = 0;
		public static int Comparison(IDictionary x, IDictionary y)
        {
            int result = 0;
			int x_task_id = EB.Dot.Integer("task_id", x, 1);
			int y_task_id = EB.Dot.Integer("task_id", y, 1);
			if(x_task_id!=y_task_id)
			{
				if (x_task_id == task_id) return -1;
				if (y_task_id == task_id) return 1;

				string x_state = EB.Dot.String("state", x, TaskSystem.RUNNING);
				string y_state = EB.Dot.String("state", y, TaskSystem.RUNNING);
				result = GetTaskStateOrderIndex(x_state) - GetTaskStateOrderIndex(y_state);
				if (result == 0)
				{
					result = x_task_id - y_task_id;
				}
				return result;
			}
			return result;
		}
    }

	public override void Start()
    {
		//place guide task in the first location
		int guideid = 0;
		if (GuideManager.Instance.GuideState )
		{
			guideid = GuideManager.Instance.StepCurrentId;
        }
		else if(OpenGuideManager.Instance.IsGuideReady())
		{
			guideid = OpenGuideManager.Instance.StepCurrentId;
		}
		if(guideid!=0)
		{
			GuideStepData step = null;
            if(GuideManager.Instance.GetStep(guideid, out  step))
			{
				if ("NewTask_Stretch".Contains(step.view))
				{
					SortingComparisonHandler.task_id = step.GetTask();
				}
            }
        }
		//SortingComparisonHandler.task_id = 2006; //test
		SortingComparison = SortingComparisonHandler.Comparison;
        base.Start();
    }

    protected override void RealDrawList(List<DictionaryEntry> entries)
    {
        List<UINormalTaskScrollItemData> Datas = new List<UINormalTaskScrollItemData>();
        if (entries != null)
        {
            if(entries.Count>0)
            {
                if (m_DefaultTip) m_DefaultTip.SetActive(false);
            }
            else
            {
                if (m_DefaultTip) m_DefaultTip.SetActive(true);
            }
            for (int i = 0; i < entries.Count; i++)
            {
                //string task_id = entries[i].Key.ToString();
                Datas.Add(new UINormalTaskScrollItemData(GetItemDataIDFromKey(entries[i].Key.ToString()), i));
            }

            List<int> mUnlockFuncIdLists = FuncTemplateManager.Instance.GetUnlockFuncIdLists();
            for (int i = 0; i < Datas.Count; i++)
            {
	            // EB.Debug.LogError(Datas[i].data_id);
	            string task_id = Datas[i].data_id.Remove(0, TaskSystem.TASKPREFIX_LENGTH);
	            TaskTemplate taskTpl = TaskTemplateManager.Instance.GetTask(task_id);
                    if (taskTpl != null && taskTpl.function_limit != 0)
                    {
                        int function = taskTpl.function_limit;
                        Datas[i].hide = mUnlockFuncIdLists.Contains(function);
                    }
                }

            Datas.RemoveAll(x => x.hide == true);
            m_GridScroll.SetItemDatas(Datas.ToArray());
        }
        else
        {
            if (m_DefaultTip) m_DefaultTip.SetActive(true);
        }
	}

    protected string GetItemDataIDFromKey(string key)
    {
        return mDL.DefaultDataID + "." + key;
    }

    // public override void OnLookupUpdate(string dataID, object value)
    // {
	   //  base.OnLookupUpdate(dataID, value);
    // }

    private static int GetTaskStateOrderIndex(string state)
    {
        if (!TaskStateOrder.Contains(state))
            TaskStateOrder.Add(state);

        return TaskStateOrder.IndexOf(state);
    }

    private static List<string> TaskStateOrder = new List<string> {
            TaskSystem.FINISHED,
            TaskSystem.RUNNING
        };
}
}