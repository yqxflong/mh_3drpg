using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    public class TaskUITabController : UITabControllerHotFix
    {
        public override void Awake()
        {
            base.Awake();
            TabLibPrefabs = new List<TabLibEntry>();
            TabLibEntry entry = new TabLibEntry();

            GameObject TabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_Main/EnchantTab1").gameObject;
            GameObject PressedTabObj1 = mDMono.transform.Find("UpButtons/ButtonGrid/0_Main/EnchantTab2").gameObject;
            GameObject GameViewObj1 = mDMono.transform.Find("TaskViews/Main").gameObject;
            entry.TabObj = TabObj1;
            entry.PressedTabObj = PressedTabObj1;
            entry.GameViewObj = GameViewObj1;
            TabLibPrefabs.Add(entry);

            TabObj1.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                OnTabPressed(TabObj1);
            }));

            GameObject TabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Normal/ConvertTaba1").gameObject;
            GameObject PressedTabObj2 = mDMono.transform.Find("UpButtons/ButtonGrid/1_Normal/ConvertTaba2").gameObject;
            GameObject GameViewObj2 = mDMono.transform.Find("TaskViews/Normal").gameObject;
            entry.TabObj = TabObj2;
            entry.PressedTabObj = PressedTabObj2;
            entry.GameViewObj = GameViewObj2;
            TabLibPrefabs.Add(entry);

            TabObj2.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                OnTabPressed(TabObj2);
            }));
         

            GameObject TabObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/2_Week/ConvertTaba1").gameObject;
            GameObject PressedTabObj3 = mDMono.transform.Find("UpButtons/ButtonGrid/2_Week/ConvertTaba2").gameObject;
            GameObject GameViewObj3 = mDMono.transform.Find("TaskViews/Week").gameObject;
            entry.TabObj = TabObj3;
            entry.PressedTabObj = PressedTabObj3;
            entry.GameViewObj = GameViewObj3;
            TabLibPrefabs.Add(entry);

            TabObj3.gameObject.GetComponent<UIEventTrigger>().onPress.Add(new EventDelegate(() =>
            {
                OnTabPressed(TabObj3);
            }));

    }
}
}