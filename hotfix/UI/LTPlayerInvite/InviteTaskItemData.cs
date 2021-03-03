using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{


    public class InviteTaskItemData
    {
        public int taskid { get; private set; }
        public string taskdes { get; private set; }
        public List<LTShowItemData> rewarddata { get; private set; }
        public int tasktype { get; private set; }

        public InviteTaskItemData(int taskid, int tasktype,Data.TaskTemplate tasktpl)
        {
            this.taskid = taskid;
            this.tasktype = tasktype; 
            this.taskdes = tasktpl.target_tips;
            rewarddata = TaskStaticData.GetItemRewardList(taskid);
        }
    }
}
