using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTUltimateTrialCompeteRankListHudCtrl : UIControllerHotfix
    {
        public override bool IsFullscreen() { return false; }
        public override bool ShowUIBlocker { get { return true; } }

        private CommonRankGridScroll Scroll;
        private UIConditionTabController categoryTabs;

        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseBtn");
            categoryTabs = t.GetMonoILRComponent<UIConditionTabController>("BG/Top");

            categoryTabs.TabLibPrefabs = new List<UITabControllerHotFix.TabLibEntry>();
            UITabControllerHotFix.TabLibEntry entry = new UITabControllerHotFix.TabLibEntry();
            Transform mDMono = categoryTabs.mDMono.transform;
            GameObject TabObj1 = mDMono.transform.Find("ButtonGrid/0_Cur/Tab1").gameObject;
            GameObject PressedTabObj1 = mDMono.transform.Find("ButtonGrid/0_Cur/Tab2").gameObject;
            GameObject GameViewObj1 = mDMono.transform.Find("ButtonGrid/0_Cur/Cur").gameObject;
            entry.TabObj = TabObj1;
            entry.PressedTabObj = PressedTabObj1;
            entry.GameViewObj = GameViewObj1;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj1);
            }));
            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj1.GetMonoILRComponent<UltimateTrialCompeteCurRankController>().ClickTitleRefreshGrid();
            }));

            GameObject TabObj2 = mDMono.transform.Find("ButtonGrid/1_History/Tab1").gameObject;
            GameObject PressedTabObj2 = mDMono.transform.Find("ButtonGrid/1_History/Tab2").gameObject;
            GameObject GameViewObj2 = mDMono.transform.Find("ButtonGrid/1_History/History").gameObject;
            entry.TabObj = TabObj2;
            entry.PressedTabObj = PressedTabObj2;
            entry.GameViewObj = GameViewObj2;
            categoryTabs.TabLibPrefabs.Add(entry);

            TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                categoryTabs.OnTabPressed(TabObj2);
            }));
            TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                GameViewObj2.GetMonoILRComponent<UltimateTrialCompeteHistoryRankController>().ClickTitleRefreshGrid();
            }));
        }
        
        public override IEnumerator OnAddToStack()
        {
            return base.OnAddToStack();

        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    }
}