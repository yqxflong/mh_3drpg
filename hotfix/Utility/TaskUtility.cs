using Hotfix_LT.Data;
using Hotfix_LT.UI;
using System.Collections.Generic;

namespace LT.Hotfix.Utility {
    public static class TaskUtility {
        /// <summary>
        /// 获取任务所需执行事件进度目标值。
        /// </summary>
        public static int GetEventTargetNum(int taskId) {
            int value;
            DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.event_count.target_num", taskId), out value);
            return value;
        }

        /// <summary>
        /// 获取任务所需执行事件进度当前值。
        /// </summary>
        public static int GetEventCurrentNum(int taskId) {
            int value;
            DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.event_count.current_num", taskId), out value);
            return value;
        }

        /// <summary>
        /// 获取任务所需执行事件是否完成。
        /// </summary>
        public static bool IsEventFinished(int taskId) {
            bool value;
            DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.event_count.finished", taskId), out value);
            return value;
        }

        /// <summary>
        /// 获取任务是否完成。
        /// </summary>
        public static bool IsTaskFinished(int taskId) {
            var state = GetTaskState(taskId);
            return state == TaskSystem.FINISHED || state == TaskSystem.COMPLETED;
        }

        /// <summary>
        /// 获取任务完成状态：acceptable（领取任务）；running（执行中）；finished（完成）；completed（领取奖励）
        /// </summary>
        public static string GetTaskState(int taskId) {
            string value;
            DataLookupsCache.Instance.SearchDataByID(string.Format("tasks.{0}.state", taskId), out value);
            return value;
        }

        public static bool IsTasksFinished(List<TaskTemplate> tasks) {
            if (tasks == null) {
                return false;
            }

            for (var i = 0; i < tasks.Count; i++) {
                if (!IsTaskFinished(tasks[i].task_id)) {
                    return false;
                }
            }

            return true;
        }
    }
}