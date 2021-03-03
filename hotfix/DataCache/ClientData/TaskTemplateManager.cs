using Hotfix_LT.UI;
using System.Collections.Generic;
using Unity.Standard.ScriptsWarp;

namespace Hotfix_LT.Data
{
    public class TaskTemplate
    {
        public int task_id;
        public string task_name;
        //public int pre_task;
        //public int role_level;
        //public int receiving_means;
        public string scene_id;
        public string npc_id;
        public string tips;
        //public bool is_auto_complete;
        public string commit_scene_id;
        public string commit_npc_id;
        public int task_type;
        public int target_type;
        public string target_parameter_1;
        public string target_parameter_2;
        public string target_parameter_3;
        public string target_tips;
        public int[] xp;
        public int[] gold;
        public int hc;
        public string res_type;
        public int res_count;
        public ResourceContainer rewards;
        public int hero_shard;
        public int shard_count;
        //public int accept_dialogue;
        //public int complete_dialogue;
        public int achievement_point;
        public int activity_point;
        public int function_limit;

        public static TaskTemplate Search = new TaskTemplate();
        public static TaskTemplateComparer Comparer = new TaskTemplateComparer();
    }

    public class TaskTemplateComparer : IComparer<TaskTemplate>
    {
        public int Compare(TaskTemplate x, TaskTemplate y)
        {
            return x.task_id - y.task_id;
        }
    }

    public class BattlePassTemplate
    {
        public int id;
        public List<LTShowItemData> awards;
    }

    public enum LTGTriggerType
    {
        None = 0,

        TimeStamp = 1,//时间戳触发类型
        Level = 2,//等级触发类型
        Main = 3,//主线
        Challenge = 4,//挑战
        UltimateTrial = 5,//极限试炼
        Draw = 6,//抽奖抽出新ssr伙伴

        DrawFail = 7,//抽奖非酋
        StarUp = 8,//升星

        DrawSame = 9,//抽奖抽到已有的ssr
    }
    public class LimitedTimeGiftTemplate
    {
        public string ID;
        public LTGTriggerType TriggerType;
        public string TargetParameter;
        public int TriggerOpenTime;
        public int Duration;
        public List<int> GiftList;
    }

    public class TaskTemplateManager
    {
        private static TaskTemplateManager sInstance = null;
        private TaskTemplate[] mTasks = null;
        private Dictionary<int, BattlePassTemplate> mBattlePassDic;
        private List<LimitedTimeGiftTemplate> mLimitedTimeGiftList;

        private int[] mMonsterPool;

        public static TaskTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new TaskTemplateManager(); }
        }

        private TaskTemplateManager()
        {

        }

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                sInstance.mTasks = null;
                sInstance.mBattlePassDic.Clear();
                sInstance.mLimitedTimeGiftList.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Task task)
        {
            if (task == null)
            {
                EB.Debug.LogError("InitFromDataCache: task is null");
                return false;
            }

            var conditionSet = task.GetArray(0);
            if (!InitTasks(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init tasks failed");
                return false;
            }

            if (!InitMonsterPool(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: InitMonsterPool failed");
                return false;
            }

            if (!InitBattlePass(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: InitBattlePass failed");
                return false;
            }

            if (!InitLimitedTimeGift(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: InitLimitedTimeGift failed");
                return false;
            }

            return true;
        }

        private bool InitTasks(GM.DataCache.ConditionTask tasks)
        {
            if (tasks == null)
            {
                EB.Debug.LogError("InitTasks: tasks is null");
                return false;
            }

            mTasks = new TaskTemplate[tasks.TasksLength];
            for (int i = 0; i < mTasks.Length; ++i)
            {
                mTasks[i] = ParseTask(tasks.GetTasks(i));
            }

            System.Array.Sort(mTasks, TaskTemplate.Comparer);
            return true;
        }

        private TaskTemplate ParseTask(GM.DataCache.TaskInfo obj)
        {
            TaskTemplate tpl = new TaskTemplate();
            tpl.task_id = int.Parse(obj.TaskId);
			using (ZString.Block())
			{
				ZString strID = ZString.Format("ID_task_tasks_{0}_task_name", tpl.task_id);
				tpl.task_name = EB.Localizer.GetTableString(strID, obj.TaskName); ;//tpl.TaskName;

				strID = ZString.Format("ID_task_tasks_{0}_target_tips", tpl.task_id);
				tpl.target_tips = EB.Localizer.GetTableString(strID, obj.TargetTips); ;//tpl.TargetTips;
			};
			tpl.scene_id = obj.SceneId;
            tpl.npc_id = obj.NpcId;
            tpl.tips = obj.Tips;
            tpl.commit_scene_id = obj.CommitSceneId;
            tpl.commit_npc_id = obj.CommitNpcId;
            tpl.task_type = obj.TaskType;
            tpl.target_type = obj.TargetType;
            tpl.target_parameter_1 = obj.TargetParameter1;
            tpl.target_parameter_2 = obj.TargetParameter2;
            tpl.target_parameter_3 = obj.TargetParameter3;            
            tpl.xp = new int[obj.XpLength];
            for (int i = 0; i < obj.XpLength; ++i)
            {
                tpl.xp[i] = obj.GetXp(i);
            }
            tpl.gold = new int[obj.GoldLength];
            for (int i = 0; i < obj.GoldLength; ++i)
            {
                tpl.gold[i] = obj.GetGold(i);
            }
            tpl.hc = obj.Hc;
            tpl.res_type = obj.ResType;
            tpl.res_count = obj.ResCount;

            tpl.rewards = new ResourceContainer();
            do
            {
                int id = 0, amount = 0;

                int.TryParse(obj.Reward1, out id);
                amount = obj.Count1;
                if (id <= 0 || amount <= 0) break;
                if (tpl.rewards.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseTask: reward {0} exists");
                    tpl.rewards.Items.Remove(id);
                }
                tpl.rewards.Items.Add(id, amount);

                int.TryParse(obj.Reward2, out id);
                amount = obj.Count2;
                if (id <= 0 || amount <= 0) break;
                if (tpl.rewards.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseTask: reward {0} exists");
                    tpl.rewards.Items.Remove(id);
                }
                tpl.rewards.Items.Add(id, amount);

                int.TryParse(obj.Reward3, out id);
                amount = obj.Count3;
                if (id <= 0 || amount <= 0) break;
                if (tpl.rewards.Items.ContainsKey(id))
                {
                    EB.Debug.LogError("ParseTask: reward {0} exists");
                    tpl.rewards.Items.Remove(id);
                }
                tpl.rewards.Items.Add(id, amount);
            } while (false);

            int.TryParse(obj.HeroShard, out tpl.hero_shard);
            tpl.shard_count = obj.ShardCount;
            //tpl.accept_dialogue = obj.AcceptDialogue;
            //tpl.complete_dialogue = obj.CompleteDialogue;
            tpl.achievement_point = obj.AchievementPoint;
            tpl.activity_point = obj.ActivityPoint;
            tpl.function_limit = obj.FunctionLimit;
            return tpl;
        }

        public TaskTemplate GetTask(int id)
        {
            TaskTemplate task = new TaskTemplate();
            task.task_id = id;
            int index = System.Array.BinarySearch<TaskTemplate>(mTasks, task, TaskTemplate.Comparer);
            if (index >= 0)
            {
                return mTasks[index];
            }
            else
            {
                EB.Debug.LogWarning("GetTask: task not found, id = {0}", id);
                return null;
            }
        }

        public TaskTemplate GetTask(string id)
        {
            return GetTask(int.Parse(id));
        }

        private bool InitMonsterPool(GM.DataCache.ConditionTask tasks)
        {
            if (tasks == null)
            {
                EB.Debug.LogError("InitMonsterPool: tasks is null");
                return false;
            }

            mMonsterPool = new int[tasks.MonsterPoolLength];
            for (int i = 0; i < mMonsterPool.Length; ++i)
            {
                mMonsterPool[i] = int.Parse(tasks.GetMonsterPool(i).MonsterId);
            }
            return true;
        }

        public int[] GetMonsterPool()
        {
            return mMonsterPool;
        }

        private bool InitBattlePass(GM.DataCache.ConditionTask tasks)
        {
            if (tasks == null)
            {
                EB.Debug.LogError("InitBattlePass: BattlePass is null");
                return false;
            }

            mBattlePassDic = new Dictionary<int, BattlePassTemplate>();
            for (int i = 0; i < tasks.BattlePassLength; ++i)
            {
                BattlePassTemplate data = ParseBattlePass(tasks.GetBattlePass(i));
                if (data != null && data.id > 0)
                {
                    mBattlePassDic.Add(data.id, data);
                }
            }

            return true;
        }
        private BattlePassTemplate ParseBattlePass(GM.DataCache.BattlePass obj)
        {
            BattlePassTemplate tpl = new BattlePassTemplate();
            if (obj == null || string.IsNullOrEmpty(obj.Id)) return null;
            tpl.id = int.Parse(obj.Id);
            tpl.awards = new List<LTShowItemData>();
            if (!string.IsNullOrEmpty(obj.Ri1)) tpl.awards.Add(new LTShowItemData(obj.Ri1, obj.Rn1, obj.Rt1));
            if (!string.IsNullOrEmpty(obj.Ri2)) tpl.awards.Add(new LTShowItemData(obj.Ri2, obj.Rn2, obj.Rt2));
            if (!string.IsNullOrEmpty(obj.Ri3)) tpl.awards.Add(new LTShowItemData(obj.Ri3, obj.Rn3, obj.Rt3));
            if (!string.IsNullOrEmpty(obj.Ri4)) tpl.awards.Add(new LTShowItemData(obj.Ri4, obj.Rn4, obj.Rt4));

            return tpl;
        }

        private bool InitLimitedTimeGift(GM.DataCache.ConditionTask tasks)
        {
            if (tasks == null)
            {
                EB.Debug.LogError("InitLimitedTimeGift: LimitedTimeGift is null");
                return false;
            }

            mLimitedTimeGiftList = new List<LimitedTimeGiftTemplate>();
            for (int i = 0; i < tasks.LimitedTimeGiftLength; ++i)
            {
                LimitedTimeGiftTemplate data = ParseLimitedTimeGift(tasks.GetLimitedTimeGift(i));
                if (data != null && !string.IsNullOrEmpty(data.ID))
                {
                    mLimitedTimeGiftList.Add(data);
                }
            }

            return true;
        }
        private LimitedTimeGiftTemplate ParseLimitedTimeGift(GM.DataCache.LimitedTimeGift obj)
        {
            LimitedTimeGiftTemplate tpl = new LimitedTimeGiftTemplate();
            tpl.ID = obj.Id;
            tpl.TriggerType = (LTGTriggerType)obj.TriggerType;
            tpl.TargetParameter = obj.TargetParameter;
            tpl.TriggerOpenTime = obj.TriggerOpenTime;
            tpl.Duration = obj.Duration;
            tpl.GiftList = ParseGiftList(obj.Gift);
            return tpl;
        }
        private List<int> ParseGiftList(string temp)
        {
            List<int> list = new List<int>();
            if (!string.IsNullOrEmpty(temp))
            {
                string[] split = temp.Split(',');
                for (int i = 0; i < split.Length; ++i)
                {
                    list.Add(int.Parse(split[i]));
                }
            }
            return list;
        }

        public LimitedTimeGiftTemplate GetLimitedTimeGiftById(string id)
        {
            for (int i = 0; i < mLimitedTimeGiftList.Count; ++i)
            {
                if (mLimitedTimeGiftList[i].ID.Equals(id))
                {
                    return mLimitedTimeGiftList[i];
                }
            }
            return null;
        }

        public LimitedTimeGiftTemplate GetLimitedTimeGiftByTypeAndParam(LTGTriggerType type, string param)
        {
            for (int i = 0; i < mLimitedTimeGiftList.Count; ++i)
            {
                if (mLimitedTimeGiftList[i].TriggerType == type)
                {
                    if (string.IsNullOrEmpty(param))
                    {
                        return mLimitedTimeGiftList[i];
                    }
                    else if (!string.IsNullOrEmpty(mLimitedTimeGiftList[i].TargetParameter))
                    {
                        if (mLimitedTimeGiftList[i].TargetParameter.StartsWith("<"))
                        {
                            var str = mLimitedTimeGiftList[i].TargetParameter.Replace("<", "").Split(',');
                            if (str != null && str.Length > 0)
                            {
                                if (int.Parse(param) < int.Parse(str[0]))
                                {
                                    return mLimitedTimeGiftList[i];
                                }
                            }
                        }
                        else if (mLimitedTimeGiftList[i].TargetParameter.StartsWith(">"))
                        {
                            var str = mLimitedTimeGiftList[i].TargetParameter.Replace(">", "").Split(',');
                            if (str != null && str.Length > 0)
                            {
                                if (int.Parse(param) > int.Parse(str[0]))
                                {
                                    return mLimitedTimeGiftList[i];
                                }
                            }
                        }
                        else
                        {
                            if (mLimitedTimeGiftList[i].TargetParameter.Equals(param))
                            {
                                return mLimitedTimeGiftList[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        public List<LimitedTimeGiftTemplate> GetLimitedTimeGiftListByType(LTGTriggerType type)
        {
            List<LimitedTimeGiftTemplate> temp = new List<LimitedTimeGiftTemplate>();
            for (int i = 0; i < mLimitedTimeGiftList.Count; ++i)
            {
                if (mLimitedTimeGiftList[i].TriggerType == type)
                {
                    temp.Add(mLimitedTimeGiftList[i]);
                }
            }
            return temp;
        }

        public bool CheckBattlePassByTaskId(int taskId)
        {
            return mBattlePassDic.ContainsKey(taskId);
        }

        public List<LTShowItemData> GetBattlePassByTaskId(int taskId)
        {
            if (mBattlePassDic.ContainsKey(taskId))
            {
                return mBattlePassDic[taskId].awards;
            }
            else
                return null;
        }
    }
}