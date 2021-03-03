using GM.DataCache;
using Hotfix_LT.Data;
using LT.Hotfix.Utility;
using System;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionTaskItemController : DynamicMonoHotfix {
        private UILabel _labName;
        private UILabel _labDesc;
        private UILabel _labProgress;
        private UIProgressBar _progressBar;
        private GameObject _goGoBtn;
        private GameObject _goDoneTips;
        private TaskTemplate _taskInfo;
        private UISprite _progrerssBarForeground;

        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _labName = t.GetComponent<UILabel>("Name");
            _labDesc = t.GetComponent<UILabel>("Desc");
            _labProgress = t.GetComponent<UILabel>("ProgressBar/Lab_Progress");
            _progressBar = t.GetComponent<UIProgressBar>("ProgressBar");
            _progrerssBarForeground = t.GetComponent<UISprite>("ProgressBar/Foreground");
            _goGoBtn = t.FindEx("Btn_Go").gameObject;
            _goDoneTips = t.FindEx("Done").gameObject;

            t.GetComponent<UIButton>("Btn_Go").onClick.Add(new EventDelegate(OnGoButtonClicked));
        }

        public void Set(TaskTemplate taskInfo) {
            _taskInfo = taskInfo;

            if (taskInfo == null) {
                EB.Debug.LogError("LTPromotionTaskItemController.Set -> taskInfo is null");
                return;
            }

            _labName.text = taskInfo.task_name;
            _labDesc.text = taskInfo.target_tips;

            var currentNum = TaskUtility.GetEventCurrentNum(taskInfo.task_id);
            var targetNum = TaskUtility.GetEventTargetNum(taskInfo.task_id);
            _labProgress.text = LT.Hotfix.Utility.ColorUtility.FormatColorFullLevel(currentNum, targetNum);
            _progressBar.value = currentNum / (float)targetNum;

            var isFinished = TaskUtility.IsEventFinished(taskInfo.task_id);
            _goGoBtn.SetActive(!isFinished);
            _goDoneTips.SetActive(isFinished);
            SetProgressBarForeground(isFinished);
        }

        private void OnGoButtonClicked() {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            TaskSystem.ProcessTaskRunning(_taskInfo.task_id.ToString());
        }

        private void SetProgressBarForeground(bool isFull) {
            _progrerssBarForeground.spriteName = isFull ? "Ty_Strip_Yellow" : "Ty_Strip_Blue";
        }
    }
}
