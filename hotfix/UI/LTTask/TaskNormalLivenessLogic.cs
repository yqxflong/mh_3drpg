namespace Hotfix_LT.UI
{
   using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskNormalLivenessLogic : DataLookupHotfix
{
	public eTaskType mTaskType;
	public GameObject m_ChestRoot;
    //每周宝箱需完成的任务数量
	//public UILabel m_LeftWeekRewardCountLabel;
    //当前活跃度
	public UILabel m_CurrLivenessLabel;
	public UIProgressBar m_LivenessProgressBar;
	public UIServerRequest m_ReceiveChestRequest;
	public UIServerRequest m_ReceiveWeekChestRequest;
	public GameChest[] m_Chests;
	//public GameChest m_WeekRewardChest;
	public Transform m_ProgressBarLeftPos, m_ProgressBarRightPos;
	private GameChest m_CurrReceiveChest;

	UIButton HotfixBtn0;
	UIButton HotfixBtn1;
	UIButton HotfixBtn2;
	UIButton HotfixBtn3;
	UIButton HotfixBtn4;
	
	public override void Awake()
	{
		base.Awake();
		mTaskType = mDL.DefaultDataID == "user_prize_data.taskweekliveness" ? eTaskType.Week : eTaskType.Normal;
		m_ChestRoot =  mDL.transform.Find("LivenessReward").gameObject;
		m_CurrLivenessLabel =  mDL.transform.Find("LivenessReward/Flag/Liveness").GetComponent<UILabel>();
		m_LivenessProgressBar =  mDL.transform.Find("LivenessReward/ProgressBar").GetComponent<UIProgressBar>();
		m_ReceiveChestRequest =  mDL.transform.Find("LivenessReward/ReceiveChestReq").GetComponent<UIServerRequest>();
		m_ReceiveChestRequest.onResponse.Add(new EventDelegate(mDL,"OnFetchData"));
		m_ReceiveWeekChestRequest =  mDL.transform.Find("LivenessReward/ReceiveWeekChestReq").GetComponent<UIServerRequest>();
		m_ReceiveWeekChestRequest.onResponse.Add(new EventDelegate(mDL,"OnFetchData"));
		m_ProgressBarLeftPos = mDL.transform.Find("LivenessReward/ProgressBar/LeftPosition");
		m_ProgressBarRightPos = mDL.transform.Find("LivenessReward/ProgressBar/RightPosition");
		ActivityId = mTaskType==eTaskType.Week?7201:7101;
		m_Chests = new GameChest[5];
		m_Chests[0] = new GameChest();
		m_Chests[0].Open =mDL.transform.Find("LivenessReward/0/Open").gameObject;
		m_Chests[0].UnOpen = mDL.transform.Find("LivenessReward/0/Close").gameObject;
		m_Chests[0].Light = mDL.transform.Find("LivenessReward/0/Light").gameObject;
		m_Chests[0].Value = mDL.transform.Find("LivenessReward/0/ActivityNum").GetComponent<UILabel>();
		
		m_Chests[1] = new GameChest();
		m_Chests[1].Open =mDL.transform.Find("LivenessReward/1/Open").gameObject;
		m_Chests[1].UnOpen = mDL.transform.Find("LivenessReward/1/Close").gameObject;
		m_Chests[1].Light = mDL.transform.Find("LivenessReward/1/Light").gameObject;
		m_Chests[1].Value = mDL.transform.Find("LivenessReward/1/ActivityNum").GetComponent<UILabel>();
		
		m_Chests[2] = new GameChest();
		m_Chests[2].Open =mDL.transform.Find("LivenessReward/2/Open").gameObject;
		m_Chests[2].UnOpen = mDL.transform.Find("LivenessReward/2/Close").gameObject;
		m_Chests[2].Light = mDL.transform.Find("LivenessReward/2/Light").gameObject;
		m_Chests[2].Value = mDL.transform.Find("LivenessReward/2/ActivityNum").GetComponent<UILabel>();
		
		m_Chests[3] = new GameChest();
		m_Chests[3].Open =mDL.transform.Find("LivenessReward/3/Open").gameObject;
		m_Chests[3].UnOpen = mDL.transform.Find("LivenessReward/3/Close").gameObject;
		m_Chests[3].Light = mDL.transform.Find("LivenessReward/3/Light").gameObject;
		m_Chests[3].Value = mDL.transform.Find("LivenessReward/3/ActivityNum").GetComponent<UILabel>();
		
		m_Chests[4] = new GameChest();
		m_Chests[4].Open =mDL.transform.Find("LivenessReward/4/Open").gameObject;
		m_Chests[4].UnOpen = mDL.transform.Find("LivenessReward/4/Close").gameObject;
		m_Chests[4].Light = mDL.transform.Find("LivenessReward/4/Light").gameObject;
		m_Chests[4].Value = mDL.transform.Find("LivenessReward/4/ActivityNum").GetComponent<UILabel>();
		 
		HotfixBtn0 =  mDL.transform.Find("LivenessReward/0").GetComponent<UIButton>();
		HotfixBtn0.onClick.Add(new EventDelegate(() => { OnChestClick(HotfixBtn0.gameObject); }));
		HotfixBtn1 =  mDL.transform.Find("LivenessReward/1").GetComponent<UIButton>();
		HotfixBtn1.onClick.Add(new EventDelegate(()=>{OnChestClick(HotfixBtn1.gameObject); }));
		HotfixBtn2 =  mDL.transform.Find("LivenessReward/2").GetComponent<UIButton>();
		HotfixBtn2.onClick.Add(new EventDelegate(()=>{OnChestClick(HotfixBtn2.gameObject); }));
		HotfixBtn3 =  mDL.transform.Find("LivenessReward/3").GetComponent<UIButton>();
		HotfixBtn3.onClick.Add(new EventDelegate(()=>{OnChestClick(HotfixBtn3.gameObject); }));
		HotfixBtn4 =  mDL.transform.Find("LivenessReward/4").GetComponent<UIButton>();
		HotfixBtn4.onClick.Add(new EventDelegate(()=>{OnChestClick(HotfixBtn4.gameObject); }));
		
		m_LivenessProgressBar.value = 0f;
		m_ChestRoot.CustomSetActive(false);
		for (int chestIndex = 0; chestIndex < m_Chests.Length; ++chestIndex)
		{
			m_Chests[chestIndex].OriginPos = m_Chests[chestIndex].UnOpen.transform.localPosition;
		}
	}

	public override void OnLookupUpdate(string dataID, object value)
	{
		base.OnLookupUpdate(dataID, value);

		UpdateChest();
	}

	public void UpdateChest()
	{
		int currLiveness = 0;
		string key = mTaskType==eTaskType.Normal?"user_prize_data.taskliveness.curr":"user_prize_data.taskweekliveness.curr";
		DataLookupsCache.Instance.SearchIntByID(key, out currLiveness);

		List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(ActivityId);
		int maxLiveness = stages[stages.Count - 1].stage;
		for (int i = 0; i < stages.Count; ++i)
		{
			var stage = stages[i];
			List<LTShowItemData> itemDatas = new List<LTShowItemData>();
			for (int j = 0; j < stage.reward_items.Count; ++j)
			{
				var reward = stage.reward_items[j];
				string id = reward.id.ToString();
				if (id.Equals("xp") && BalanceResourceUtil.GetUserLevel() >= Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMaxPlayerLevel())
				{
					id = "poten-gold";
				}
				int count = reward.quantity;
				string type = reward.type;
				LTShowItemData itemData = new LTShowItemData(id, count, type, false);
				itemDatas.Add(itemData);
			}

			bool received;
			key = mTaskType==eTaskType.Normal?"user_prize_data.taskliveness_reward.":"user_prize_data.taskliveness_week_reward.";
			DataLookupsCache.Instance.SearchDataByID<bool>(key + stage.id, out received);

			eReceiveState rs = eReceiveState.cannot;
			if (currLiveness >= stage.stage)
			{
				if (received)
					rs = eReceiveState.have;
				else
					rs = eReceiveState.can;
			}
			if(m_Chests[i]!=null)
				m_Chests[i].SetUI(new RewardStageData(stage.id, stage.stage, itemDatas, rs));
		}
		float chestY = m_Chests[0].Open.transform.parent.transform.position.y;
		for (int chestIndex = 0; chestIndex <stages.Count; ++chestIndex)
		{
			float x = m_ProgressBarLeftPos.position.x + (m_ProgressBarRightPos.position.x - m_ProgressBarLeftPos.position.x) * (float)m_Chests[chestIndex].StageData.Stage / m_Chests[stages.Count-1].StageData.Stage;
			m_Chests[chestIndex].Open.transform.parent.transform.position = new Vector3(x, chestY, 0);
		}

		LTUIUtil.SetText(m_CurrLivenessLabel, currLiveness.ToString());
		m_LivenessProgressBar.value = currLiveness / (float)maxLiveness;

		//weekReward
		UpdateWeekReward();

		m_ChestRoot.CustomSetActive(true);
	}

	public void OnChestClick(GameObject go)
	{
	    FusionAudio.PostEvent("UI/General/ButtonClick");
        int index;
        //去掉判断是否是每周宝箱
	    int.TryParse(go.name, out index);
        GameChest chest = m_Chests[index];

		if (chest.StageData.ReceiveState == eReceiveState.can)
		{
			m_CurrReceiveChest = chest;
			chest.UpdateReceiveState(eReceiveState.have);
			if(mTaskType==eTaskType.Week)
				SendReceiveWeekRewardReq(chest.StageData);
			else
				SendReceiveRewardReq(chest.StageData);
            return;
		}

		
		
		string tip="";
		if (chest.StageData.ReceiveState == eReceiveState.cannot)
		{
		    tip = string.Format(EB.Localizer.GetString("ID_codefont_in_TaskNormalLivenessLogic_3701"), chest.StageData.Stage);
        }
		else if(chest.StageData.ReceiveState==eReceiveState.have)
		{
			tip = EB.Localizer.GetString("ID_codefont_in_LadderController_11750");
		}
		var ht = Johny.HashtablePool.Claim();
		ht.Add("data", chest.StageData.Awards);
		ht.Add("tip",tip);
        GlobalMenuManager.Instance.Open("LTRewardShowUI", ht);
	}

	private void SendReceiveRewardReq(RewardStageData stageData)
	{
		m_ReceiveChestRequest.parameters[0].parameter = stageData.Id.ToString();		
		m_ReceiveChestRequest.SendRequest();
		LoadingSpinner.Show();
	}


    /// <summary>
    /// 发送请求每周宝箱
    /// </summary>
    /// <param name="stageData"></param>
	private void SendReceiveWeekRewardReq(RewardStageData stageData)
	{
		m_ReceiveWeekChestRequest.parameters[0].parameter = stageData.Id.ToString();
		m_ReceiveWeekChestRequest.SendRequest();
		LoadingSpinner.Show();
	}

    public override void OnFetchData(EB.Sparx.Response res, int reqInstanceID)
	{
		LoadingSpinner.Hide();
		if (res.sucessful)
		{
			string key = mTaskType==eTaskType.Normal?"user_prize_data.taskliveness_reward.":"user_prize_data.taskliveness_week_reward.";
			string cachePath= key + m_CurrReceiveChest.StageData.Id;
			DataLookupsCache.Instance.CacheData(cachePath, true);
            //上传友盟获得钻石，任务
            int hcCount = 0;
            List<LTShowItemData> mlist = m_CurrReceiveChest.StageData.Awards;
            for (int i = 0; i < mlist.Count; i++)
            {
                if (mlist[i].id == "hc") hcCount += mlist[i].count;
            }
            FusionTelemetry.PostBonus(hcCount, Umeng.GA.BonusSource.Source2);
            GlobalMenuManager.Instance.Open("LTShowRewardView", m_CurrReceiveChest.StageData.Awards);
		}
		else
		{
			res.CheckAndShowModal();
		}
	}

	public int ActivityId=7101;

	private void UpdateWeekReward()
	{
		// List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> weekStages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(WeekActivityId);
		// Hotfix_LT.Data.TimeLimitActivityStageTemplate weekStage = weekStages[0];
		// List<ShowItemData> weekItemDatas = new List<ShowItemData>();
		// for (int j = 0; j < weekStage.reward_items.Count; ++j)
		// {
		// 	var reward = weekStage.reward_items[j];
		// 	string id = reward.id.ToString();
		// 	int count = reward.quantity;
		// 	string type = reward.type;
		// 	ShowItemData itemData = new ShowItemData(id, count, type);
		// 	weekItemDatas.Add(itemData);
		// }
		//
		// bool received;
		// DataLookupsCache.Instance.SearchDataByID<bool>("user_prize_data.taskliveness_week_reward." + weekStage.id, out received);
		//
		// eReceiveState state = eReceiveState.cannot;
		// if (GetLeftWeekRewardTaskNum()<=0)
		// {
		// 	if (received)
		// 		state = eReceiveState.have;
		// 	else
		// 		state = eReceiveState.can;
		// }
//		m_WeekRewardChest.SetUI(new RewardStageData(weekStage.id, weekStage.stage, weekItemDatas, state));
//		LTUIUtil.SetText(m_LeftWeekRewardCountLabel , GetLeftWeekRewardTaskNum().ToString());
	}

	// private int GetLeftWeekRewardTaskNum()
	// {
	// 	List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(WeekActivityId);
	// 	int needNum = stages[0].stage;
	//
	// 	int weekFinishNum;
	// 	DataLookupsCache.Instance.SearchIntByID("user_prize_data.taskliveness.weekFinishNum", out weekFinishNum);
	//
	// 	return weekFinishNum > needNum ? 0 : needNum - weekFinishNum;
	// }
}

}