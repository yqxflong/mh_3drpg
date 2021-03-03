using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTWorldBossRankRewardCell : DynamicCellController<Hotfix_LT.Data.BossRewardTemplate>
    {
        public UILabel RankLabel;
        public UIGrid grid;
        public List<LTShowItem> Items;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            RankLabel = t.GetComponent<UILabel>("Label");
            grid = t.GetComponent<UIGrid>("Grid");
            Items = new List<LTShowItem>();

            var childCount = grid.transform.childCount;

            if (childCount > 0)
            {
                for (var i = 0; i < childCount; i++)
                {
                    Items.Add(grid.transform.GetChild(i).GetMonoILRComponent<LTShowItem>());
                }
            }
        }
    
        public override void Clean()
        {
            
        }
    
        public override void Fill(Hotfix_LT.Data.BossRewardTemplate itemData)
        {
            string[] rankStrs = itemData.ranks.Split(';');
            if (rankStrs.Length < 2)
            {
                EB.Debug.LogError("LTWorldBossRankRewardCell: RankStrs Length Less Than 2. Error!!!");
                return;
            }
            int top = int.Parse(rankStrs[0]);
            int down = int.Parse(rankStrs[1]);
    
            if (down == -1)
            {
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_386"), top);
            }
            else if (top == down)
            {
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_RANK_NUMBER"), top);
            }
            else if (down >= 1000)
            {
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_611"), top, down);
            }
            else
            {
                RankLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_ArenaRankAwardCell_716"), top, down);
            }
    
            string[] rewardStrs = itemData.reward.Split(';');
            for (int i = 0; i < Items.Count; i++)
            {
                if (i < rewardStrs.Length)
                {
                    Items[i].mDMono.gameObject.SetActive(true);
                    string[] rewardSingleStrs = rewardStrs[i].Split(',');
                    if (rewardSingleStrs.Length < 3)
                    {
                        EB.Debug.LogError("LTWorldBossRankRewardCell: RewardSingleStrs Length Less Than 3. Error!!!");
                        return;
                    }
    
                    Items[i].LTItemData = new LTShowItemData(rewardSingleStrs[0], int.Parse(rewardSingleStrs[1]), rewardSingleStrs[2], true);
                }
                else
                {
                    Items[i].mDMono.gameObject.SetActive(false);
                }
            }
    
            grid.Reposition();
        }
    }
}
