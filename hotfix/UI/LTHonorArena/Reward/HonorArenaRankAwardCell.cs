namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class HonorArenaRankAwardCell: DynamicCellController<Hotfix_LT.Data.HonorArenaRewardTemplate> {

        public UILabel RankLabel;
        public List<LTShowItem> Items;
       
        public override void Clean()
        {
        }
        
        public override void Awake()
        {
            base.Awake();

            RankLabel =  mDMono.transform.Find("Name").GetComponent<UILabel>();
            Items = new List<LTShowItem>();
            //ArenaRankAwardUI/Content/ScrollView/Placehodler/Grid/Template/Awards/LTShowItem
            Items.Add(mDMono.transform.Find("Awards/LTShowItem").GetMonoILRComponent<LTShowItem>()); 
            Items.Add(mDMono.transform.Find("Awards/LTShowItem (1)").GetMonoILRComponent<LTShowItem>()); 
            Items.Add(mDMono.transform.Find("Awards/LTShowItem (2)").GetMonoILRComponent<LTShowItem>()); 
        }

        public override void Fill(Hotfix_LT.Data.HonorArenaRewardTemplate itemData)
        {
           
        }
    }

}