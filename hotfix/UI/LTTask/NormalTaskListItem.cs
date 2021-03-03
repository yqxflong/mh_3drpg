using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.UI
{
    public class NormalTaskListItem : DataLookupHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDL.transform;
            m_UISprite = t.GetComponent<UISprite>("Left/Score");
            m_NameLabel = t.GetComponent<UILabel>("Left/TaskName");
            m_TipsLabel = t.GetComponent<UILabel>("Left/TaskTips");
            m_NeedTimesLabel = t.GetComponent<UILabel>("Left/ProgressBar/NeedTimes");
            m_NeedTimesLabel2 = t.GetComponent<UILabel>("Left/NeedTimes");
            m_GetPointLabel = t.GetComponent<UILabel>("Left/Score/Num");
            m_TaskState = t.GetDataLookupILRComponent<NormalTaskListItemState>("Left");
            m_ProgressBar = t.GetComponent<UIProgressBar>("Left/ProgressBar");
            m_RewardItemList = new List<LTShowItem>();
	        m_RewardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Left/Rewards/LTShowItem"));
	        m_RewardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Left/Rewards/LTShowItem (1)"));
	        m_RewardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Left/Rewards/LTShowItem (2)"));
	        
            t.GetComponent<UIButton>("Btns/Go").onClick.Add(new EventDelegate(OnGoBtnClick));
            t.GetComponent<UIButton>("Btns/Receive").onClick.Add(new EventDelegate(OnCompleteBtnClick));
        }


    	private static Dictionary<int,string> dict =new Dictionary<int, string>(){{1,"Task_Icon_Jinpai"},{3,"Task_Icon_Yinpai"},{8,"Task_Icon_Zipai"}};
    	public UISprite m_UISprite;
    	public UILabel m_NameLabel;
    	public UILabel m_TipsLabel;
    	public UILabel m_NeedTimesLabel;
    	public UILabel m_NeedTimesLabel2;//主线任务
    	public UILabel m_GetPointLabel;
    	public NormalTaskListItemState m_TaskState;
    	public UIProgressBar m_ProgressBar;
    
    	//public UIGameItem m_Gold;
    	//public UIGameItem m_Exp;
    	//public UIGameItem m_Hc;
    	public List<LTShowItem> m_RewardItemList;
    
    	private int m_taskid;
    	public override void OnLookupUpdate(string dataID, object value)
    	{
    		base.OnLookupUpdate(dataID, value);
    		if (dataID != null)
    		{
    			m_taskid = 0;
    			//dataID: tasks.2010
    			int.TryParse(dataID.Remove(0, TaskSystem.TASKPREFIX_LENGTH), out m_taskid);
    			string taskState = EB.Dot.String("state", value, "");
    			Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(m_taskid);
    			if (taskTpl == null)return;
    			if(dict.ContainsKey(taskTpl.task_type))m_UISprite.spriteName = dict[taskTpl.task_type];
    			LTUIUtil.SetText(m_NameLabel,taskTpl.task_name);
    			LTUIUtil.SetText(m_TipsLabel, taskTpl.target_tips);
    			LTUIUtil.SetText(m_GetPointLabel,"x"+(taskTpl.task_type == (int)eTaskType.Main?taskTpl.achievement_point.ToString():taskTpl.activity_point.ToString()));
    			{
    				int needtimes = EB.Dot.Integer("event_count.target_num", value, 1);
    				int currenttimes = EB.Dot.Integer("event_count.current_num", value, 1);
    				if (m_NeedTimesLabel != null)
    				{
    					//if (taskTpl.task_type == (int)eTaskType.Normal)
    					{ 
    						if (currenttimes >= needtimes)
    						{
    							currenttimes = needtimes;
    						}
    					}
    
    					if (taskTpl.task_type  == (int)eTaskType.Normal  || taskTpl.task_type  == (int)eTaskType.Week)
    					{
    						m_NeedTimesLabel2.gameObject.CustomSetActive(false);
    						m_ProgressBar.gameObject.CustomSetActive(true);
    						LTUIUtil.SetText(m_NeedTimesLabel,LT.Hotfix.Utility.ColorUtility.TaskFormatEnoughStr(currenttimes, needtimes));
    						m_ProgressBar.value = currenttimes*1.0f / needtimes;
    					}
    					else if (taskTpl.task_type  == (int)eTaskType.Main  )
    					{
    						m_NeedTimesLabel2.gameObject.CustomSetActive(true);
    						m_ProgressBar.gameObject.CustomSetActive(false);
    						LTUIUtil.SetText(m_NeedTimesLabel2,LT.Hotfix.Utility.ColorUtility.TaskFormatEnoughStr(currenttimes, needtimes));
    					}
    				}
    			}			
                
                m_TaskState.mDL.DefaultDataID = dataID + ".state";
    			int taskid = 0;
    			int.TryParse(dataID.Replace(TaskStaticData.TaskPrefix, ""),out taskid);
    			if (m_taskid != taskid)
    				Debug.LogError("m_taskid == taskid");
    
                List<LTShowItemData> rewardDatas = TaskStaticData.GetItemRewardList(taskid);
                List<LTShowItemData> temp = new List<LTShowItemData>();
              
    			for (int viewIndex = 0; viewIndex < m_RewardItemList.Count; ++viewIndex)
    			{
    				if (viewIndex < rewardDatas.Count)
    				{
    					m_RewardItemList[viewIndex].LTItemData
                        =new LTShowItemData(rewardDatas[viewIndex].id,rewardDatas[viewIndex].count,rewardDatas[viewIndex].type,false);
    					//日常任务没有了奖励
    				if(taskTpl.task_type==1)m_RewardItemList[viewIndex].mDMono.gameObject.SetActive(true);
    				}
    				else
    				{
    					m_RewardItemList[viewIndex].mDMono.gameObject.SetActive(false);
    				}
    			}
    		}
    	}
    
    	public void OnGoBtnClick()
    	{
    		FusionAudio.PostEvent("UI/General/ButtonClick");
            TaskSystem.ProcessTaskRunning(m_taskid.ToString());
    	}
    
    	public void OnCompleteBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
            CompleteTask();
    	}
    
    	
    	private void CompleteTask()
    	{
    		InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 10f);
            LTHotfixManager.GetManager<TaskManager>().RequestComplete(m_taskid.ToString(), OnCompleteTask);
    	}
    
    	public void OnCompleteTask(EB.Sparx.Response result)
    	{
    		InputBlockerManager.Instance.UnBlock(InputBlockReason.FUSION_BLOCK_UI_INTERACTION);
    		if (result.sucessful)
    		{
    			if (result.hashtable != null){
    				DataLookupsCache.Instance.CacheData(result.hashtable);
				}

				{
					var ht = Johny.HashtablePool.Claim();
					ht.Add("0", m_NameLabel.text);
					MessageTemplateManager.ShowMessage(901036, ht, null);
					Johny.HashtablePool.Release(ht);
				}
    			
                //上传友盟获得钻石，任务
                List<LTShowItemData> mlist = TaskStaticData.GetItemRewardList(m_taskid);
                FusionTelemetry.ItemsUmengCurrency(mlist,"任务获得");
    
                Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(m_taskid);
                FusionTelemetry.TaskData. PostEvent(m_taskid, taskTpl.task_name, taskTpl.task_type.ToString());
    
                if (TaskStaticData.GetItemRewardList(m_taskid).Count>0)
                {
					var ht = Johny.HashtablePool.Claim();
					ht.Add("reward", TaskStaticData.GetItemRewardList(m_taskid));
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
