using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationTerritoryController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            CombatCountdownTimeLabel = t.GetComponent<UILabel>("Content/LTPopFrame/CombatTime/Countdown/TimeLabel");
            CombatOpenTimeLabel = t.GetComponent<UILabel>("Content/LTPopFrame/CombatTime/OpenStartTime/TimeLabel");
            RedPoint = t.FindEx("Content/LTPopFrame/Goto/RedPoint").gameObject;
            controller.backButton = t.GetComponent<UIButton>("UIFrameBG/TopLeftAnchor/CancelBtn");

            CombatRewardShowItems = new LTShowItem[3];
            CombatRewardShowItems[0] = t.GetMonoILRComponent<LTShowItem>("Content/LTPopFrame/Reward/RewardGrid/LTShowItem");
            CombatRewardShowItems[0] = t.GetMonoILRComponent<LTShowItem>("Content/LTPopFrame/Reward/RewardGrid/LTShowItem (1)");
            CombatRewardShowItems[0] = t.GetMonoILRComponent<LTShowItem>("Content/LTPopFrame/Reward/RewardGrid/LTShowItem (2)");

            OccupancyEntrys = new NationOccupancyEntry[3];
            OccupancyEntrys[0].natinName = "persian";
            OccupancyEntrys[0].percentLabel = t.GetComponent<UILabel>("Content/LTPopFrame/Occupancy/Grid/Item/ProgressBar/Label");
            OccupancyEntrys[0].progressBar = t.GetComponent<UIProgressBar>("Content/LTPopFrame/Occupancy/Grid/Item/ProgressBar");

            OccupancyEntrys[1].natinName = "roman";
            OccupancyEntrys[1].percentLabel = t.GetComponent<UILabel>("Content/LTPopFrame/Occupancy/Grid/Item (1)/ProgressBar/Label");
            OccupancyEntrys[1].progressBar = t.GetComponent<UIProgressBar>("Content/LTPopFrame/Occupancy/Grid/Item (1)/ProgressBar");

            OccupancyEntrys[2].natinName = "egypt";
            OccupancyEntrys[2].percentLabel = t.GetComponent<UILabel>("Content/LTPopFrame/Occupancy/Grid/Item (2)/ProgressBar/Label");
            OccupancyEntrys[2].progressBar = t.GetComponent<UIProgressBar>("Content/LTPopFrame/Occupancy/Grid/Item (2)/ProgressBar");

            t.GetComponent<UIButton>("Content/LTPopFrame/RuleBtn").onClick.Add(new EventDelegate(OnRuleBtnClick));
            t.GetComponent<UIButton>("Content/LTPopFrame/Goto").onClick.Add(new EventDelegate(OnGotoCombatClick));

        }


    
    	[System.Serializable]
    	public class NationOccupancyEntry
    	{
    		public string natinName;
    		public UILabel percentLabel;
    		public UIProgressBar progressBar;
    	    public UIProgressBar progressBar1;
    	}
    
    	public NationOccupancyEntry[] OccupancyEntrys;
    	public UILabel CombatCountdownTimeLabel;
    	public UILabel CombatOpenTimeLabel;
    	public LTShowItem[] CombatRewardShowItems;
    
        public GameObject RedPoint;
    
    	public override bool IsFullscreen()
    	{
    		return true;
    	}
    
    	public override IEnumerator OnAddToStack()
    	{
    	    CampaignTextureCmp BG = controller.gameObject.GetComponent<CampaignTextureCmp>();
    	    BG.spriteName = "Game_Background_2";
            NationManager.Instance.GetTerritoryInfo(null);
    		GameDataSparxManager.Instance.RegisterListener(NationManager.TerritoryDataId, OnTerritoryListListener);
    
    		if (NationManager.Instance.BattleTimeConfig.GetIsAtBattleTime())
    		{
    			NationManager.Instance.BattleTimeConfig.InitCountDownTime();
    			StartCoroutine(CountdownBattleEndTime());
    			CombatOpenTimeLabel.transform.parent.gameObject.SetActive(false);
    			CombatCountdownTimeLabel.transform.parent.gameObject.SetActive(true);
    		}
    		else
    		{
    			LTUIUtil.SetText(CombatOpenTimeLabel, NationManager.Instance.BattleTimeConfig.TimeStr);
    			CombatOpenTimeLabel.transform.parent.gameObject.SetActive(true);
    			CombatCountdownTimeLabel.transform.parent.gameObject.SetActive(false);
    		}
    
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.RedPoint_Nation, SetRP);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Nation);
            yield return base.OnAddToStack();
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
            CampaignTextureCmp BG = controller.gameObject.GetComponent<CampaignTextureCmp>();
            BG.spriteName = string.Empty;
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.RedPoint_Nation, SetRP);
            GameDataSparxManager.Instance.UnRegisterListener(NationManager.TerritoryDataId, OnTerritoryListListener);
    		DestroySelf();
    		yield break;
    	}
    
        private void SetRP()
        {
            RedPoint.CustomSetActive(NationManager.Instance.BattleTimeConfig.GetIsBattleStay());
        }
    
    	void OnTerritoryListListener(string path, INodeData data)
    	{
    		NationTerritoryList territoryData = data as NationTerritoryList;
    		if (!territoryData.Updated)
    			return;
    
    		for (var i = 0; i < NationManager.Instance.List.Members.Count; i++)
    		{
                var nation = NationManager.Instance.List.Members[i];
                int territory_count = System.Array.FindAll(NationManager.Instance.TerritoryList.TerritoryList, m => m.Owner == nation.Name).Length;
    			nation.Occupancy = (float)territory_count / NationManager.Instance.TerritoryList.TerritoryList.Length;
    		}
    
    		for (var i = 0; i < OccupancyEntrys.Length; i++)
    		{
                var entry = OccupancyEntrys[i];
                NationData nationData = NationManager.Instance.List.Find(entry.natinName);
    			LTUIUtil.SetText(entry.percentLabel, (nationData.Occupancy * 100) + "%");
    			entry.progressBar.value = nationData.Occupancy;
    		}
    
    		float myNationOccupancy = NationManager.Instance.List.Find(NationManager.Instance.Account.NationName).Occupancy;
    		Hotfix_LT.Data.NationRatingRewardTemplate ratingReward = Hotfix_LT.Data.EventTemplateManager.Instance.GetNationRatingRewardTpl((int)(myNationOccupancy*100));
    		if (ratingReward != null)
    		{
    			for (int fillIndex = 0; fillIndex < CombatRewardShowItems.Length; ++fillIndex)
    			{
    				if (fillIndex < ratingReward.rewardDatas.Count)
    					CombatRewardShowItems[fillIndex].LTItemData = ratingReward.rewardDatas[fillIndex];
    				else
    					CombatRewardShowItems[fillIndex].mDMono.gameObject.SetActive(false);
    			}
    		}
    		else
    		{
    			//Debug.Log("Hotfix_LT.Data.NationRatingRewardTemplate is null for Occupancy=" + myNationOccupancy);
    			for (int fillIndex = 0; fillIndex < CombatRewardShowItems.Length; ++fillIndex)
    			{
    				CombatRewardShowItems[fillIndex].mDMono.gameObject.SetActive(false);
    			}
    		}
    	}
    
    	IEnumerator CountdownBattleEndTime()
    	{
    		while (true)
    		{
    			bool isEnd;
    			LTUIUtil.SetText(CombatCountdownTimeLabel, NationManager.Instance.BattleTimeConfig.GetCountDownStr(out isEnd));
    			if (isEnd)
    			{
    				GlobalMenuManager.Instance.Open("LTNationBattleResultUI");
    				yield break;
    			}
    			yield return new WaitForSeconds(1f);
    		}
    	}
    
    	public void OnRuleBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		string text = EB.Localizer.GetString("ID_NATION_TERRITORY_RULE");
    		if (text.Contains("ID_"))
    			text = EB.Localizer.GetString("ID_codefont_in_NationTerritoryController_4277");
    		GlobalMenuManager.Instance.Open("LTRuleUIView", text);
    	}
    
    	public void OnGotoCombatClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		// should goto npc pos
    		GlobalMenuManager.Instance.Open("LTNationBattleEntryUI");
    	}
    }
}
