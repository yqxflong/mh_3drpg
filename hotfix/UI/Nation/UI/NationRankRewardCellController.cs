using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class NationRankRewardItemData
    {
    	public string RankName;
    	public List<LTShowItemData> RewardDatas;
    	public eReceiveState ReceiveState;
    }
    
    public class NationRankRewardCellController : DynamicCellController<NationRankRewardItemData>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            RankNameLabel = t.GetComponent<UILabel>("Name");
            UnableReceiveBtn = t.FindEx("CannotReceiveBtn").gameObject;
            ReceiveBtn = t.FindEx("ReceiveBTN").gameObject;
            HasReceiveBtn = t.FindEx("HasReceiveBtn").gameObject;
            t.GetComponent<ConsecutiveClickCoolTrigger>("ReceiveBTN").clickEvent.Add(new EventDelegate(OnReceiveClick));
            RewardShowItems = new List<LTShowItem>();
            RewardShowItems.Add(t.GetMonoILRComponent<LTShowItem>("RewardGrid/LTShowItem"));
            RewardShowItems.Add(t.GetMonoILRComponent<LTShowItem>("RewardGrid/LTShowItem (1)"));
            RewardShowItems.Add(t.GetMonoILRComponent<LTShowItem>("RewardGrid/LTShowItem (2)"));
        }


    
    	public UILabel RankNameLabel;
    	public List<LTShowItem> RewardShowItems;
    	public GameObject UnableReceiveBtn;
    	public GameObject ReceiveBtn;
    	public GameObject HasReceiveBtn;
    	NationRankRewardItemData mItemData;
    
    	public override void Clean()
    	{
    	
    	}
    
    
    	public override void Fill(NationRankRewardItemData itemData)
    	{
    		mItemData = itemData;
    		LTUIUtil.SetText(RankNameLabel, itemData.RankName);
    		for (int viewIndex = 0; viewIndex < RewardShowItems.Count; ++viewIndex)
    		{
    			if (viewIndex < itemData.RewardDatas.Count)
    			{
    				RewardShowItems[viewIndex].LTItemData = itemData.RewardDatas[viewIndex];
    				RewardShowItems[viewIndex].mDMono.gameObject.SetActive(true);
    			}
    			else
    			{
    				RewardShowItems[viewIndex].mDMono.gameObject.SetActive(false);
    			}
    		}
    		SetReceiveState();
    	}
    
    	void SetReceiveState()
    	{
    		UnableReceiveBtn.SetActive(mItemData.ReceiveState == eReceiveState.cannot);
    		ReceiveBtn.gameObject.SetActive(mItemData.ReceiveState == eReceiveState.can);
    		HasReceiveBtn.gameObject.SetActive(mItemData.ReceiveState == eReceiveState.have);
    	}
    
    	public void OnReceiveClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		if (!NationManager.Instance.Account.HaveReceiveRankReward)
    		{
    			NationManager.Instance.Account.HaveReceiveRankReward = true;
    			mItemData.ReceiveState = eReceiveState.have;
    			SetReceiveState();
    			NationManager.Instance.ReceiveRankReward(delegate (bool successful)
    			{
    				if (successful)
    				{
                        GlobalMenuManager.Instance.Open("LTShowRewardView", mItemData.RewardDatas);
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Nation);
    				}
    			});
    		}
    	}
    }
}
