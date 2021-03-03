using UnityEngine;
using System.Collections.Generic;
using Hotfix_LT.UI;

namespace Hotfix_LT.UI { 

public class UITabControllerHotFix : DynamicMonoHotfix
{
    [System.Serializable]
    public struct TabLibEntry
    {
        public GameObject TabObj;
        public GameObject PressedTabObj;
        public GameObject GameViewObj;
        public GameObject GameObj3D;
        public UILabel TabTitle;
    }

    public List<TabLibEntry> TabLibPrefabs;

    public Color NormalTextColor;
    public Color SelectedTextColor;

    public bool ShowBlackTransition = false;
    public GameObject m_tabObj;
    public string CurTabName { get { return m_tabObj.gameObject.name; } }

    protected TabLibEntry currentTabEntry = new TabLibEntry();
    public List<EventDelegate> PressEventList = new List<EventDelegate>();

    public override void Start()
    {
        if (m_tabObj == null && TabLibPrefabs != null)
        {
            m_tabObj = TabLibPrefabs[0].TabObj;
            ShowTabObj();
        }
    }

    protected virtual void InitGameViewObjs()
    {
        HideAllGameViewAndPressedTabObj();
    }

    public void SelectTab(int index)
    {
        var entry = TabLibPrefabs[index];
        OnTabPressNoSound(entry.TabObj);
    }



    /// <summary>
    /// 点击按钮操作
    /// </summary>
    /// <param name="tabObj"></param>
    public virtual void OnTabPressNoSound(GameObject tabObj)
    {
        m_tabObj = tabObj;
        if (PressEventList.Count > 0)
        {
            EventDelegate.Execute(PressEventList);
        }

        TabLibEntry clickedTabEntry = TabLibPrefabs.Find(x => x.TabObj == m_tabObj);

        if (currentTabEntry.TabTitle != null && string.Compare(currentTabEntry.TabTitle.text, clickedTabEntry.TabTitle.text) == 0)
        {
            return;
        }

        if (ShowBlackTransition)
        {
            ScreenTransitionMask.Instance.ShowMask(ShowTabObj);
        }
        else
        {
            ShowTabObj();
        }
    }

    public virtual void OnTabPressed(GameObject tabObj)
    {
        m_tabObj = tabObj;
        if (PressEventList.Count > 0)
        {
            EventDelegate.Execute(PressEventList);
        }

        TabLibEntry clickedTabEntry = TabLibPrefabs.Find(x => x.TabObj == m_tabObj);

        //if(currentTabEntry.TabTitle != null && string.Compare(currentTabEntry.TabTitle.text, clickedTabEntry.TabTitle.text) == 0)
        if (currentTabEntry.TabObj != null && string.Compare(currentTabEntry.TabObj.transform.parent.name, clickedTabEntry.TabObj.transform.parent.name) == 0)
        {
            return;
        }

        if (ShowBlackTransition)
        {
            ScreenTransitionMask.Instance.ShowMask(ShowTabObj);
        }
        else
        {
            ShowTabObj();
        }
    }

    void ShowTabObj()
    {
        TabLibEntry clickedTabEntry = TabLibPrefabs.Find(x => x.TabObj == m_tabObj);

        //if (currentTabEntry.TabTitle != null && string.Compare(currentTabEntry.TabTitle.text, clickedTabEntry.TabTitle.text) == 0)
        if (currentTabEntry.TabObj != null && string.Compare(currentTabEntry.TabObj.transform.parent.name, clickedTabEntry.TabObj.transform.parent.name) == 0)
        {
            return;
        }
        else
        {
            currentTabEntry = clickedTabEntry;
            HideAllGameViewAndPressedTabObj();
        }

        if (null != clickedTabEntry.GameViewObj)
        {
            clickedTabEntry.GameViewObj.CustomSetActive(true);
        }
        if (null != clickedTabEntry.GameObj3D)
        {
            clickedTabEntry.GameObj3D.CustomSetActive(true);
        }
        if (null != clickedTabEntry.PressedTabObj)
        {
            clickedTabEntry.PressedTabObj.CustomSetActive(true);
            if (clickedTabEntry.PressedTabObj.GetComponent<TweenScale>() != null)
            {
                TweenScale ts = clickedTabEntry.PressedTabObj.GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }
        }
        if (null != clickedTabEntry.TabTitle)
        {
            clickedTabEntry.TabTitle.color = SelectedTextColor;
        }
    }

    protected void HideAllGameViewAndPressedTabObj()
    {
        for (int i = 0; i < TabLibPrefabs.Count; i++)
        {
            if (null != TabLibPrefabs[i].GameViewObj)
            {
                TabLibPrefabs[i].GameViewObj.CustomSetActive(false);
            }
            if (null != TabLibPrefabs[i].GameObj3D)
            {
                TabLibPrefabs[i].GameObj3D.CustomSetActive(false);
            }
            if (null != TabLibPrefabs[i].PressedTabObj)
            {
                TabLibPrefabs[i].PressedTabObj.CustomSetActive(false);
            }
            if (null != TabLibPrefabs[i].TabTitle)
            {
                TabLibPrefabs[i].TabTitle.color = NormalTextColor;
            }
        }
    }

    public void InitTab()
    {
        m_tabObj = TabLibPrefabs[0].TabObj;
        TabLibEntry clickedTabEntry = TabLibPrefabs.Find(x => x.TabObj == m_tabObj);
        currentTabEntry = clickedTabEntry;
        HideAllGameViewAndPressedTabObj();

        if (null != clickedTabEntry.GameViewObj)
        {
            clickedTabEntry.PressedTabObj.CustomSetActive(true);
            if (clickedTabEntry.PressedTabObj.GetComponent<TweenScale>() != null)
            {
                TweenScale ts = clickedTabEntry.PressedTabObj.GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }
            clickedTabEntry.GameViewObj.CustomSetActive(true);
            if (clickedTabEntry.GameObj3D != null)
            {
                clickedTabEntry.GameObj3D.CustomSetActive(true);
            }

            if (clickedTabEntry.TabTitle != null)
                clickedTabEntry.TabTitle.color = SelectedTextColor;
        }
    }

    public void ClearData()
    {
        currentTabEntry = new TabLibEntry();
    }
}

}
