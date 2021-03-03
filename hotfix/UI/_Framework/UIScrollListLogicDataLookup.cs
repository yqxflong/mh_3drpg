using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;


namespace Hotfix_LT.UI
{
    public abstract class UIScrollListLogicDataLookup : DataLookupHotfix
    {
        public Comparison<IDictionary> SortingComparison;
        protected bool hasStarted;
        // Use this for initialization
        public override void Start()
        {
            base.Start();
            InitPageSettings();
            hasStarted = true;
        }


        #region page settings
        // TODO : rename to TabSettings
        public List<PageSettingsValue> PageSettings;

        [Serializable]
        public class PageSettingsValue
        { // use class in place of struct for inheritance support
            public string PageID;
            public string DataID;
            public Predicate<IDictionary> Filter; // Filter must never be NULL. If no filter needed, set one that always returns TRUE
            public InspectablePredicate InspectableFilter; // InspectableFilter is never NULL cause it shows up in the Inspector
            public string PageName;
            public GameObject TabButton;

            /// <summary>
            /// Sets number of columns (x) and rows (y). If rows equals 0, the list will auto-calculate it. Number of columns MUST be set to a number greater than 0
            /// </summary>
            public Vector2 Size;
        }

        private void InitPageSettings()
        {
            for (var i = 0; i < PageSettings.Count; i++)
            {
                var entry = PageSettings[i];
                entry.Filter = entry.InspectableFilter.ToNativePredicate();
            }
        }
        #endregion

        #region current page
        //private bool IsSwitchingCurrentPageIndex; // TEMP

        // TODO : rename to currentTabIndex
        protected int currentPageIndex;
        public virtual int CurrentPageIndex
        {
            get { return currentPageIndex; }

            set
            {
                currentPageIndex = value;
                mDL.DefaultDataID = CurrentPageSettings.DataID;
                DrawList();
            }
        }

        // TODO : rename to CurrentTabID
        public string CurrentPageID
        {
            get { return CurrentPageSettings.PageID; }
            set { CurrentPageIndex = PageSettings.FindIndex(arg => arg.PageID == value); }
        }

        // TODO : rename to CurrentTabSettings
        public PageSettingsValue CurrentPageSettings
        {
            get
            {
                return PageSettings[CurrentPageIndex];
            }
        }

        // TODO : rename to CurrentTabNumColumns
        public int CurrentPageNumColumns
        {
            get { return (int)CurrentPageSettings.Size.x; }
        }

        public override void OnEnable()
        {
            //base.OnEnable();
            //re-draw the list. cause datacache may be updated(sell, purchase, etc).
            if (hasStarted)
            {
                DrawList();
            }
        }

        #endregion
        public override void OnLookupUpdate(string dataID, object value)
        {
            if (!hasStarted) Start();
            base.OnLookupUpdate(dataID, value);
            DrawList();
        }

        protected void DrawList()
        {
            List<DictionaryEntry> entries = new List<DictionaryEntry>(ItemsCollectionList);
            if (SortingComparison != null)
                entries.Sort((x, y) => SortingComparison(x.Value as IDictionary, y.Value as IDictionary));
            RealDrawList(entries);
        }

        protected virtual void RealDrawList(List<DictionaryEntry>  entries)
        {
            //List<UINormalTaskScrollItemData> Datas = new List<UINormalTaskScrollItemData>();
            //if (entries != null)
            //{
            //    for (int i = 0; i < entries.Length; i++)
            //    {
            //        string task_id = entries[i].Key.ToString();
            //        Datas.Add(new UINormalTaskScrollItemData(task_id, task_type, i));
            //    }
            //    m_GridScroll.dataItems = Datas.ToArray();
            //}
        }

        public virtual DictionaryEntry[] ItemsCollectionList
        {
            get
            {
                IDictionary itemDataCollection = mDL.GetDefaultLookupData<IDictionary>();

                if (itemDataCollection == null)
                    return null;

                Predicate<IDictionary> filter = CurrentPageSettings.Filter;

                DictionaryEntry[] entries = itemDataCollection
                    .Cast<DictionaryEntry>()
                    .Where(arg => filter(arg.Value as IDictionary))
                        .ToArray();

                return entries;
            }
        }
    }
}