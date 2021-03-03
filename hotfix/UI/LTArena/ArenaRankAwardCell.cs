namespace Hotfix_LT.UI
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class ArenaRankAwardCell: DynamicCellController<Hotfix_LT.Data.ArenaAwardTemplate> {

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

        public override void Fill(Hotfix_LT.Data.ArenaAwardTemplate itemData)
        {
            if (itemData.rank_down == -1)
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_386"), itemData.rank_top);
            else if(itemData.rank_top==itemData.rank_down)
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_RANK_NUMBER"), itemData.rank_top);
            else if(itemData.rank_down>=1000)
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_611"), itemData.rank_top, itemData.rank_down);
            else 
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_716"), itemData.rank_top, itemData.rank_down);

            int award_count = 0;
            if (itemData.hc != 0)
            {
                Items[award_count].LTItemData = new LTShowItemData("hc", itemData.hc, "res",false);
                award_count++;
            }
            if (itemData.gold != 0)
            {
                Items[award_count].LTItemData = new LTShowItemData("gold", itemData.gold, "res",false);
                award_count++;
            }
            if (itemData.arena_gold != 0 && award_count <= 2)
            {
                Items[award_count].LTItemData = new LTShowItemData("arena-gold", itemData.arena_gold, "res",false);
                award_count++;
            }
            for (int j = award_count; j < Items.Count; ++j)
            {
                Items[j].mDMono.gameObject.SetActive(false);
            }
        }
    }

}