using Hotfix_LT.Data;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class TaskRewardItem
    {
        public string m_ItemID;
        public int m_ItemNum;

        public TaskRewardItem(string id, int num)
        {
            m_ItemID = id;
            m_ItemNum = num;
        }
    }

    public class TaskStaticData
    {
        public static string TaskPrefix = "tasks.";

        public static int GetGoldRewardNum(int taskid)
        {
            TaskTemplate taskTpl = TaskTemplateManager.Instance.GetTask(taskid);

            if (taskTpl == null)
            {
                return 0;
            }

            if (taskTpl.gold.Length <= 0)
            {
                return 0;
            }

            int A = taskTpl.gold[0];

            if (taskTpl.gold.Length >= 2)
            {
                int B = taskTpl.gold[1];
                int lvl = BalanceResourceUtil.GetUserLevel();
                return A + B * lvl;
            }
            else
            {
                return A;
            }
        }

        public static int GetExpRewardNum(int taskid)
        {
            TaskTemplate taskTpl = TaskTemplateManager.Instance.GetTask(taskid);

            if (taskTpl == null)
            {
                return 0;
            }

            if (taskTpl.xp.Length <= 0)
            {
                return 0;
            }

            int A = taskTpl.xp[0];

            if (taskTpl.xp.Length >= 2)
            {
                int B = taskTpl.xp[1];
                int lvl = BalanceResourceUtil.GetUserLevel();
                return A + B * lvl;
            }
            else
            {
                return A;
            }
        }

        public static int GetHcRewardNum(int taskid)
        {
            TaskTemplate taskTpl = TaskTemplateManager.Instance.GetTask(taskid);

            if (taskTpl == null)
            {
                return 0;
            }

            return taskTpl.hc;
        }

        public static List<LTShowItemData> GetItemRewardList(int taskid)
        {
            TaskTemplate taskTpl = TaskTemplateManager.Instance.GetTask(taskid);

            if (taskTpl == null)
            {
                return null;
            }

            List<LTShowItemData> dataList = new List<LTShowItemData>();
            int expnum = GetExpRewardNum(taskid);
            int goldnum = GetGoldRewardNum(taskid);
            int hcnum = GetHcRewardNum(taskid);

            if (expnum > 0)
            {
                if (BalanceResourceUtil.GetUserLevel() >= CharacterTemplateManager.Instance.GetMaxPlayerLevel())
                {
                    dataList.Add(new LTShowItemData("poten-gold", expnum, LTShowItemType.TYPE_RES));
                }
                else
                {
                    dataList.Add(new LTShowItemData("exp", expnum, LTShowItemType.TYPE_RES));
                }
            }

            if (goldnum > 0)
            {
                dataList.Add(new LTShowItemData("gold", goldnum, LTShowItemType.TYPE_RES));
            }

            if (hcnum > 0)
            {
                dataList.Add(new LTShowItemData("hc", hcnum, LTShowItemType.TYPE_RES));
            }

            if (!string.IsNullOrEmpty(taskTpl.res_type) && taskTpl.res_count > 0)
            {
                dataList.Add(new LTShowItemData(taskTpl.res_type, taskTpl.res_count, LTShowItemType.TYPE_RES)); 
            }

            foreach (var reward in taskTpl.rewards.Items)
            {
                dataList.Add(new LTShowItemData(reward.Key.ToString(), reward.Value, LTShowItemType.TYPE_GAMINVENTORY));
            }

            int heroshardid = taskTpl.hero_shard;
            int count = taskTpl.shard_count * 1;

            if (heroshardid > 0 && count > 0)
            {
                dataList.Add(new LTShowItemData(heroshardid.ToString(), count, LTShowItemType.TYPE_HEROSHARD));
            }

            return dataList;
        }
    }
}