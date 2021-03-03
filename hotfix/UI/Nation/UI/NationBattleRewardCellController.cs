using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class NationBattleRewardCellController : DynamicCellController<Hotfix_LT.Data.NationRatingRewardTemplate>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            RankLabel = t.GetComponent<UILabel>("Name");
            Items = new List<LTShowItem>();
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem"));
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (1)"));
            Items.Add(t.GetMonoILRComponent<LTShowItem>("Awards/LTShowItem (2)"));
        }


    	public UILabel RankLabel;
    	public List<LTShowItem> Items;
    
    	public override void Clean()
    	{
    	}
    
    	public override void Fill(Hotfix_LT.Data.NationRatingRewardTemplate itemData)
    	{
    		RankLabel.text = string.Format("{0}%:",itemData.rating);
    		for (int fillIndex = 0; fillIndex < Items.Count; ++fillIndex)
    		{
    			if (fillIndex < itemData.rewardDatas.Count)
    			{
    				Items[fillIndex].LTItemData = itemData.rewardDatas[fillIndex];
    				Items[fillIndex].mDMono.gameObject.SetActive(true);
    			}
    			else
    			{
    				Items[fillIndex].mDMono.gameObject.SetActive(false);
    			}
    		}
    	}
    }
}
