using System;
using UnityEngine;
using System.Collections;
using System.Reflection.Emit;
using Hotfix_LT.Data;
using Debug = EB.Debug;

/// <summary>
    /// 挂载在：LTClimbingTowerRewardHud
    /// </summary>
namespace Hotfix_LT.UI
{
    public class LTClimingTowerRewardControl : UIControllerHotfix
    {
        public UILabel progressBarLabel;
        public UIProgressBar progressBar;
        
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            progressBar = t.GetComponent<UIProgressBar>("Frame/ProgressWidget/ProgressBar");
            progressBarLabel = t.GetComponent<UILabel>("Frame/ProgressWidget/ProgressBar/Label");
            DynamicScroll = t.GetMonoILRComponent<LTClimingTowerDynamicScroll>("Content/ScrollView/Placehodler/Grid");
            UIButton backButton = t.GetComponent<UIButton>("Frame/BG/Top/CloseBtn");
            backButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
        }

        public override bool ShowUIBlocker { get { return true; } }
    
        public LTClimingTowerDynamicScroll DynamicScroll;
    
        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            int cur = LTClimingTowerManager.Instance.GetCurrentRecord();
            int max = LTClimingTowerManager.Instance.GetMaxRecord();
            progressBar.value = cur * 1.0f / max;
            progressBarLabel.text = $"{cur}/{max}";
            F_SetData();
        }
    
        public void F_SetData()
        {
            ClimingTowerRewardTemplate[] temp= EventTemplateManager.Instance.GetSleepRewardList().ToArray();
            DynamicScroll.SetItemDatas(temp);
            int i = LTClimingTowerManager.Instance.GetCurrentPosition();
            var VS=DynamicScroll.scrollView.GetComponent<UIPanel>().GetViewSize();
            //最大可见的物品数量
            int ActinCount =(int)( VS.y / DynamicScroll.mDMono.transform.GetComponent<UIGrid>().cellHeight);
            i = Mathf.Min(i-1, temp.Length - 1 - ActinCount);
            DynamicScroll.MoveTo(i < 0 ? 0 : i);
        }

        public override void OnCancelButtonClick()
        {
            SpringPanel sp = DynamicScroll.scrollView.GetComponent<SpringPanel>();
            if (sp != null && sp.enabled)
            {
                return;
            }
            base.OnCancelButtonClick();
            
        }
    }
}
