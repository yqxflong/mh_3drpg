using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
    public class InventoryItemNumDataLookup : DataLookupHotfix
    {
        private string ItemId;
        private int MaxCount;
        private List<string> strIds = new List<string>();
        private UILabel mLabel;
    
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
    
            if (mLabel == null)
            {
                mLabel = mDL.transform.GetComponent<UILabel>();
            }
    
            if (dataID == null) return;
    
            int num = 0;
            string curDataId = string.Empty;

            if (value != null)
            {
                num = EB.Dot.Integer("num", value, 0);
                string curId = EB.Dot.String("economy_id", value, "");
                curDataId = EB.Dot.Integer("inventory_id", value, 0).ToString();
            }

            if (num == 0 || num == MaxCount)//刷新下
            {
                strIds = GetServerGoodsId();
            }
    
            for (int i = 0; i < strIds.Count; i++)
            {
                if (strIds[i].CompareTo(curDataId) == 0)
                {
                    continue;
                }

                int Num = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("inventory.{0}.num", strIds[i]), out Num);
                num += Num;
            }
    
            LTUIUtil.SetText(mLabel, num.ToString());
        }
    
        public void SetData(string itemId,int maxCount=999)
        {
            ItemId = itemId;
            MaxCount = maxCount;
            if (mLabel == null)
            {
                mLabel = mDL.transform.GetComponent<UILabel>();
            }
            LTUIUtil.SetText(mLabel, 0.ToString());
            Add();
        }
    
        public void UpdateData()
        {
            if (string.IsNullOrEmpty(ItemId)) return;
            if (strIds.Count == 0) Add();
        }
    
        public void RemoveData()
        {
            if (string.IsNullOrEmpty(ItemId)) return;
            strIds.Clear();
            mDL.DataIDList.Clear();
            ItemId = string.Empty;
        }
    
        private void Add()
        {
            if (string.IsNullOrEmpty(ItemId)) return;
    
            strIds = GetServerGoodsId();
            for (var i = 0; i < strIds.Count; i++)
            {
                var strId = strIds[i];
                if (!mDL.DataIDList.Contains(string.Format("inventory.{0}", strId)) && !string.IsNullOrEmpty(strId))
                {
                    mDL.RegisterDataID(string.Format("inventory.{0}", strId));
                }
            }
        }
        
        private List<string> GetServerGoodsId()
        {
            Hashtable inventoryData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);
            if (inventoryData == null)
            {
                return new List<string>();
            }
    
            List<string> keys = new List<string>();
            foreach (var item in inventoryData.Cast<DictionaryEntry>())
            {
                var va = item.Value as IDictionary;
                if (ItemId.CompareTo(va["economy_id"].ToString()) == 0)
                {
                    keys.Add(item.Key.ToString());
                }
            }
            return keys;
        }
    }
}
