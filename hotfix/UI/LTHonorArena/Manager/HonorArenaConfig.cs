using System;
using System.Collections.Generic;
using EB;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class HonorArenaConfig
    {
        private static HonorArenaConfig sInstance = null;
        public static HonorArenaConfig Instance
        {
            get { return sInstance = sInstance ?? new HonorArenaConfig(); }
        }
        
        private List<HonorArenaReward> honorArenaRewards;
        public List<HonorArenaReward> InitRewardView()
        {
            if (honorArenaRewards == null)
            {
                honorArenaRewards = new List<HonorArenaReward>();
                List<HonorArenaRewardTemplate> tpls = Hotfix_LT.Data.EventTemplateManager.Instance.GetHonorArenaRewardList();
                for (int i = 0; i < tpls.Count; i++)
                {
                    HonorArenaReward reward = new HonorArenaReward();
                    reward.id = tpls[i].id;
                    try
                    {
                        reward.oneHourGold = int.Parse(tpls[i].oneHourReward.Split(',')[1]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                    string[] rankStrs = tpls[i].ranks.Split(';');
                    if (rankStrs.Length < 2)
                    {
                        EB.Debug.LogError("HonorArenaReward: Rank Length Less Than 2. Error!!!");
                        return null;
                    }
                    reward.top = int.Parse(rankStrs[0]);
                    reward.down = int.Parse(rankStrs[1]);
                    
                    string[] rewardStrs = tpls[i].reward.Split(';');
                    for (int j = 0; j < rewardStrs.Length; j++)
                    {
                        string[] rewardSingleStrs = rewardStrs[j].Split(',');
                        if (rewardSingleStrs.Length < 3)
                        {
                            EB.Debug.LogError("HonorArenaReward: RewardSingleStrs Length Less Than 3. Error!!!");
                            return null;
                        }
                        LTShowItemData data1 = new LTShowItemData(rewardSingleStrs[0], int.Parse(rewardSingleStrs[1]), rewardSingleStrs[2], true);
                        reward.listShowItemData.Add(data1);
                    }
                    honorArenaRewards.Add(reward);
                }
            }

            return honorArenaRewards;

        }
        
        
        public int GetHonorArenaUpperLimit()
        {
            int UpperLimit = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("HonorArenaIdelReward_UpperLimit");
            int extraAdd = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.HonorArenaIdleRewardLimit);
            return UpperLimit + extraAdd;
        }
        
        public int GetOneHourByReward(int rank)
        {
            List<HonorArenaReward> temp= HonorArenaConfig.Instance.InitRewardView();
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].top<=rank && temp[i].down>=rank)
                {
                    return temp[i].oneHourGold;
                }
            }

            return 0;
        }
    }
}