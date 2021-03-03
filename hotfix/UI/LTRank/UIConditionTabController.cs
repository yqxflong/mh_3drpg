using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UIConditionTabController : UITabControllerHotFix
    {
        public bool IsUNConditionShow = true;//条件不满足是显示 还是不显示
        public bool IsTabConditionGray;

        protected override void InitGameViewObjs()
        {
	        for (int i = 0; i < TabLibPrefabs.Count; ++i)
            {
                //TabLibEntry entry = new TabLibEntry();

                //entry.TabObj = TabLibPrefabs[i].TabObj;
                //entry.PressedTabObj = TabLibPrefabs[i].PressedTabObj;
                //entry.TabTitle = TabLibPrefabs[i].TabTitle;
                //entry.GameObj3D = TabLibPrefabs[i].GameObj3D;
                //entry.GameViewObj = TabLibPrefabs[i].GameViewObj;
                //entry.GameViewObj.name = TabLibPrefabs[i].GameViewObj.name;
                if (!IsUNConditionShow)
                {
                    if (TabLibPrefabs[i].GameViewObj != null)
                    {
                        UIConditionTabAdapt condition = TabLibPrefabs[i].GameViewObj.GetMonoILRComponent<UIConditionTabAdapt>();
                        if (condition != null && !condition.IsConditionOk())
                        {
                            TabLibPrefabs[i].GameViewObj.SetActive(false);
                            if (TabLibPrefabs[i].TabObj != null)
                            {
                                TabLibPrefabs[i].TabObj.transform.parent.gameObject.SetActive(false);
                            }
                            if (TabLibPrefabs[i].GameObj3D != null)
                                TabLibPrefabs[i].GameObj3D.SetActive(false);
                            continue;
                        }
                    }
                }
                else
                {
                    if (IsTabConditionGray)
                    {
                        if (TabLibPrefabs[i].GameViewObj != null)
                        {
                            UIConditionTabAdapt condition = TabLibPrefabs[i].GameViewObj.GetMonoILRComponent<UIConditionTabAdapt>();
                            if (!condition.IsConditionOk())
                                TabLibPrefabs[i].TabObj.GetComponent<UIWidget>().color = new Color32(255, 0, 255, 255);
                            else
                                TabLibPrefabs[i].TabObj.GetComponent<UIWidget>().color = new Color32(255, 255, 255, 255);
                        }
                    }
                }
                //m_tabLibObjs.Add(entry);
            }
            HideAllGameViewAndPressedTabObj();
        }

        public override void OnTabPressNoSound(GameObject tabObj)
        {
            if (IsUNConditionShow)
            {
                TabLibEntry clickedTabEntry = TabLibPrefabs.Find(x => x.TabObj == tabObj);

				if (currentTabEntry.TabTitle != null && string.Compare(currentTabEntry.TabTitle.text, clickedTabEntry.TabTitle.text) == 0)
                {
                    return;
                }
                if (clickedTabEntry.GameViewObj != null)
                {
                    // UIFuncConditionTabAdapt condition = clickedTabEntry.GameViewObj.GetMonoILRComponent<UIFuncConditionTabAdapt>();
                    UIConditionTabAdapt condition =
                        clickedTabEntry.GameViewObj.GetMonoILRComponentByClassPath<UIConditionTabAdapt>(
                            "Hotfix_LT.UI.UIFuncConditionTabAdapt", false);
                    if (condition != null && !condition.ShowConditionMessage())
                    {
                        return;
                    }
                }
            }
            base.OnTabPressNoSound(tabObj);
        }

        public override void OnTabPressed(GameObject tabObj)
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (IsUNConditionShow)
            {
                TabLibEntry clickedTabEntry = TabLibPrefabs.Find(x => x.TabObj == tabObj);

                if (currentTabEntry.TabTitle != null && string.Compare(currentTabEntry.TabTitle.text, clickedTabEntry.TabTitle.text) == 0)
                {
                    return;
                }
                if (clickedTabEntry.GameViewObj != null)
                {
                    // UIFuncConditionTabAdapt condition = clickedTabEntry.GameViewObj.GetMonoILRComponent<UIFuncConditionTabAdapt>();
                    UIConditionTabAdapt condition =
                        clickedTabEntry.GameViewObj.GetMonoILRComponentByClassPath<UIConditionTabAdapt>(
                            "Hotfix_LT.UI.UIFuncConditionTabAdapt", false);
                    if (condition != null && !condition.ShowConditionMessage())
                    {
                        return;
                    }
                }
            }
            base.OnTabPressed(tabObj);
        }

        public void UpdateTabState()
        {
            if (IsUNConditionShow && IsTabConditionGray)
            {
                for (int i = 0; i < TabLibPrefabs.Count; ++i)
                {
                    if (TabLibPrefabs[i].GameViewObj != null)
                    {
                        UIConditionTabAdapt condition = TabLibPrefabs[i].GameViewObj.GetMonoILRComponent<UIConditionTabAdapt>();
                        if (!condition.IsConditionOk())
                            TabLibPrefabs[i].TabObj.GetComponent<UIWidget>().color = new Color32(255, 0, 255, 255);
                        else
                            TabLibPrefabs[i].TabObj.GetComponent<UIWidget>().color = new Color32(255, 255, 255, 255);
                    }
                }
            }
        }

        public override void Awake()
        {
            base.Awake();
            IsUNConditionShow = true;
        }
    }
}