using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialSelectCtrl : UIControllerHotfix
    {
        public override bool IsFullscreen()
        {
            return false;
        }

        public override bool ShowUIBlocker
        {
            get
            {
                return true;
            }
        }
        
        private UILabel mTimesLabel;

        private LTUltimateTrialSelectLevelScroll LevelScroll;
        private LTUltimateTrialSelectRightScroll RightScroll;

        public override void Awake()
        {
            base.Awake();
            UIButton backBtn = controller.transform.Find("Bg/Top/CancelBtn").GetComponent<UIButton>();
            controller.backButton = backBtn;
            LTUltimateTrialDataManager.Instance.curSelectLevel = Hotfix_LT.Data.EventTemplateManager.Instance.GetInfiniteChallengeLevelByLayer(LTUltimateTrialDataManager.Instance.GetCurLayer());

            LevelScroll = controller.transform.GetMonoILRComponent<LTUltimateTrialSelectLevelScroll>("Content/Left/Scroll/PlaceHolder/Grid");
            RightScroll = controller.transform.GetMonoILRComponent<LTUltimateTrialSelectRightScroll>("Content/Right/Scroll/PlaceHolder/Grid");

            mTimesLabel = controller.transform.Find("Bg/Bottom/Title").GetComponent<UILabel>();
        }

        public override void OnDestroy()
        {

            base.OnDestroy();
        }

        private int intParam=0;
        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            if(param!=null)intParam = (int)param;
        }

        public override IEnumerator OnAddToStack()
        {
            yield return base.OnAddToStack();
            InitUI();
            LTUltimateTrialDataManager.Instance.OnLevelSelect += OnSelectFunc;
            LTUltimateTrialDataManager.Instance.OnGotoBtnClick += OnGotoFunc;
            LTUltimateTrialDataManager.Instance.OnResetTimesLabel += OnResetTimesLabelFunc;
            if (intParam != 0)
            {
                if (LTUltimateTrialDataManager.Instance.OnLevelSelect != null)
                {
                    LTUltimateTrialDataManager.Instance.OnLevelSelect(LTUltimateTrialDataManager.Instance.curSelectLevel);
                }
            }
        }

        public override IEnumerator OnRemoveFromStack()
        {
            LTUltimateTrialDataManager.Instance.OnLevelSelect -= OnSelectFunc;
            LTUltimateTrialDataManager.Instance.OnGotoBtnClick -= OnGotoFunc;
            LTUltimateTrialDataManager.Instance.OnResetTimesLabel -= OnResetTimesLabelFunc;
            DestroySelf();
            return base.OnRemoveFromStack();
        }

        private void InitUI()
        {
            InitLeftList();
            InitRightList();
            int times = LTUltimateTrialDataManager.Instance.GetChallengeTimes();
            mTimesLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTUltimateTrialHudCtrl_11054"), times > 0 ? "[42fe79]" : "[ff6699]", times, LTUltimateTrialDataManager .Instance.GetChallengeMaxTimes);//(int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("infiniteChallengeTime"));
            if (LTUltimateTrialDataManager.Instance.OnLevelSelect != null)
            {
                LTUltimateTrialDataManager.Instance.OnLevelSelect(LTUltimateTrialDataManager.Instance.curSelectLevel);
            }
        }
        
        private void InitLeftList()
        {
            List<int> levelList = Hotfix_LT.Data.EventTemplateManager.Instance.GetInfiniteChallengeAllLevel();
            LevelScroll.SetItemDatas(levelList.ToArray());
            int moveto = Mathf.Clamp(LTUltimateTrialDataManager.Instance.curSelectLevel - 3, 0, levelList.Count - 6);//滚动区域内显示的最大个数6
            LevelScroll.MoveTo(moveto);
        }
        
        private void InitRightList()
        {
            List<Hotfix_LT.Data.InfiniteChallengeTemplate> layerList = Hotfix_LT.Data.EventTemplateManager.Instance.GetInfiniteChallengeTplListByLevel(LTUltimateTrialDataManager.Instance.curSelectLevel);
            RightScroll.SetItemDatas(layerList.ToArray());
            int mHeightestLayer = LTUltimateTrialDataManager.Instance.GetCurLayer();
            int index = 0;
            for (int i = layerList.Count - 1; i >= 0; --i)
            {
                if(layerList[i].layer<= mHeightestLayer)
                {
                    index = i;
                    break;
                }
            }
            int moveto = Mathf.Clamp(index - 2, 0, layerList.Count - 4);//滚动区域内显示的最大个数4
            RightScroll.MoveTo(moveto);
        }

        private void OnSelectFunc(int selectLevel)
        {
            if (LTUltimateTrialDataManager.Instance.curSelectLevel != selectLevel)
            {
                LTUltimateTrialDataManager.Instance.curSelectLevel = selectLevel;
                InitRightList();
            }
        }

        private void OnGotoFunc()
        {
            controller.Close();
        }

        private void OnResetTimesLabelFunc()
        {
            int times = LTUltimateTrialDataManager.Instance.GetChallengeTimes();
            mTimesLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTUltimateTrialHudCtrl_11054"), times > 0 ? "[42fe79]" : "[ff6699]", times, LTUltimateTrialDataManager.Instance.GetChallengeMaxTimes);//(int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("infiniteChallengeTime"));
        }
    }
}
