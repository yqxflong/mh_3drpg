using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class NpcTaskDataLookup : DataLookupHotfix
    {

        public string m_npcName;
        private bool isMyTask(string dataID)
        {
            //这里的6 是有问题的 因为当id的路径变化时 长度就不是6了
            string task_id = dataID.Remove(0, TaskSystem.TASKPREFIX_LENGTH);
            Hashtable task = mDL.GetDefaultLookupData<Hashtable>();
            if (task == null)
                return false;
            Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
            if (taskTpl != null)
            {
                //normal task didnot have npc
                if (taskTpl.task_type == (int)eTaskType.Normal) return false;
                string npc_id = m_npcName;
                string task_npc_id;
                string task_npc_scene;
                if (task["state"].Equals(TaskSystem.UNACCEPTABLE))
                {
                    task_npc_id = /*task_template_h["npc_id"] as string; */taskTpl.npc_id;
                    task_npc_scene = taskTpl.scene_id;

                }
                else if (task["state"].Equals(TaskSystem.ACCEPTABLE))
                {
                    task_npc_id = /*task_template_h["npc_id"] as string; */taskTpl.npc_id;
                    task_npc_scene = taskTpl.scene_id;
                }
                else if (task["state"].Equals(TaskSystem.RUNNING))
                {
                    task_npc_id = /*task_template_h["commit_npc_id"] as string;*/taskTpl.commit_npc_id;
                    task_npc_scene = taskTpl.commit_scene_id;
                }
                else if (task["state"].Equals(TaskSystem.FINISHED))
                {
                    task_npc_id = /*task_template_h["commit_npc_id"] as string;*/taskTpl.commit_npc_id;
                    task_npc_scene = taskTpl.commit_scene_id;
                }
                else
                {
                    task_npc_id = /*task_template_h["npc_id"] as string;*/taskTpl.npc_id;
                    task_npc_scene = taskTpl.scene_id;
                }
                if (!npc_id.Equals(task_npc_id) || task_npc_scene != MainLandLogic.GetInstance().CurrentSceneName)
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);

            if (dataID != null && !isMyTask(dataID))
            {
                mDL.UnregisterDataID(dataID);
                return;
            }
            if (value != null)
            {
                Hashtable task = null;
                if(value is Hashtable){
                    task = value as Hashtable;
                }
            }
        }
        
        /// <summary>
        /// 是否主线任务 task_type==1
        /// </summary>
        /// <returns></returns>
        public bool IsMainTask()
        {
            string task_id = mDL.DefaultDataID.Remove(0, TaskSystem.TASKPREFIX_LENGTH);
            Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
            if (taskTpl != null)
            {
                if (taskTpl.task_type == 1) return true;
            }
            return false;
        }

        public bool IsRunning()
        {
            Hashtable task = mDL.GetDefaultLookupData<Hashtable>();
            if (task == null)
                return false;
            if (task["state"].Equals(TaskSystem.RUNNING))
            {
                return true;
            }
            return false;
        }
    }
}