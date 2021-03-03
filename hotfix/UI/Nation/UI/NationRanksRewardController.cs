using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class NationRanksRewardController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<NationRanksRewardDynamicScroll>("Content/MembersScrollView/Placeholder/Grid");
            controller. backButton = t.GetComponent<UIButton>("UINormalFrame/CloseBtn");
                      

        }
    	public override bool ShowUIBlocker
    	{
    		get
    		{
    			return true;
    		}
    	}
    
    	public NationRanksRewardDynamicScroll DynamicScroll;
    	bool isMyRankOutScrollview;
    
    	public override void SetMenuData(object param)
    	{
    		base.SetMenuData(param);
    
    		List<NationRankRewardItemData> rankRewardItemDatas = new List<NationRankRewardItemData>();
    		isMyRankOutScrollview=false;
    		for (int rankIndex = 0; rankIndex < NationUtil.RankArr.Length; ++rankIndex)
    		{
    			string rankName = NationUtil.RankArr[rankIndex];
    			string myRank = NationManager.Instance.Account.Rank;
    			NationRankRewardItemData rewardItemData = new NationRankRewardItemData();
    			rewardItemData.RankName = NationUtil.LocalizeRankName(NationUtil.RankArr[rankIndex]);
    			rewardItemData.RewardDatas = Hotfix_LT.Data.EventTemplateManager.Instance.GetNationRankRewardTpl(rankIndex + 1).rewardDatas;
    			if (rankName == myRank)
    			{
    				isMyRankOutScrollview = rankIndex <= 4;
    				rewardItemData.ReceiveState = NationManager.Instance.Account.HaveReceiveRankReward ? eReceiveState.have : eReceiveState.can;
    			}
    			else
    				rewardItemData.ReceiveState = eReceiveState.cannot;
    
    			rankRewardItemDatas.Add(rewardItemData);
    		}
    		rankRewardItemDatas.Reverse();
    
    		DynamicScroll.SetItemDatas(rankRewardItemDatas.ToArray());		
    	}
    
    	public override IEnumerator OnAddToStack()
    	{
    		yield return base.OnAddToStack();
    
    		if (isMyRankOutScrollview)
    			DynamicScroll.mDMono.GetComponentInParent<UIScrollView>().SetDragAmount(1, 1, false);
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    }
}
