namespace Hotfix_LT.UI
{
    using System;
    using UnityEngine;
    using System.Collections;

    public class UINormalTaskScrollItemData
    {
        public string data_id;
        public int index;
        public bool hide;
        public UINormalTaskScrollItemData(string taskDataId, int index)
        {
            this.data_id = taskDataId;
            this.index = index;
        }
    }

    public class UINormalTaskScrollItem : DynamicCellController<UINormalTaskScrollItemData>
    {
        public UINormalTaskScrollItemData Data
        {
            get; set;
        }

        public override void Awake()
        {
        
        }

        public override void Fill(UINormalTaskScrollItemData itemData)
        {
            Data = itemData;
            UpdateUI();
        }

        public override void Clean()
        {
            Data = null;
        }

        void UpdateUI()
        {
            mDMono.gameObject.name = Data.index.ToString() ;
            //    m_Item.DefaultDataID = Data.data_id;
            mDMono.GetComponent<DataLookupILR>().DefaultDataID = Data.data_id;
        }
    }
}