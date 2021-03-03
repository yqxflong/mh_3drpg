using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
    public class LTPartnerDataLookup : DataLookupHotfix
    {
        private string heroId;
        private int maxClip;
    
        private List<string> strIds=new List<string>();
        private UILabel mLabel;
    
        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);
    
            if (mLabel == null)
            {
                mLabel = mDL.transform.GetComponent<UILabel>();
            }
    
            if (dataID == null || string.IsNullOrEmpty(heroId)) return;

            int num = 0;
            string curDataId = string.Empty;
            if (strIds.Count == 0 && dataID == "inventory")
            {
                Add(false, value as Hashtable);
            }
            else
            {                           
                if (value != null)
                {
                    num = EB.Dot.Integer("num", value, 0);
                    string curId = EB.Dot.String("economy_id", value, "");
                    curDataId = EB.Dot.Integer("inventory_id", value, 0).ToString();
                    if (curId.CompareTo(heroId) != 0)
                    {
                        return;
                    }
                }
            }
    
            for (int i=0;i< strIds.Count; i++)
            {
                if (strIds[i].CompareTo(curDataId) == 0) continue;
                int Num = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("inventory.{0}.num", strIds[i]), out Num);
                num += Num;
            }
            if(num == 0)
            {
                Add(false, value as Hashtable);
            }
    
            if (maxClip != 0)
            {
                string colorStr = num >= maxClip ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                LTUIUtil.SetText(mLabel, string.Format("[{0}]{1}[-]/{2}", colorStr, num.ToString(), maxClip));
            }
            else
            {
                LTUIUtil.SetText(mLabel,  num.ToString());
            }
    
            Hotfix_LT.Messenger.Raise<bool>("OnPartnerHeroChipChange", num >= maxClip);
        }
    
        public void SetData(string heroId, int maxClip)
        {
            this.heroId = heroId;
            this.maxClip = maxClip;
            Add(true);
        }
    
        public void SetMaxClip(int maxClip)
        {
            this.maxClip = maxClip;
        }
    
        /// <summary>
        /// 添加物品检索位置
        /// </summary>
        /// <param name="isFrominventory">是否从完整背包数据中获取</param>
        /// <param name="data">检索数据</param>
        private void Add(bool isFrominventory, Hashtable data = null)
        {
            if (string.IsNullOrEmpty(heroId)) return;
    
            strIds = GetServerGoodsId(true);
            if (strIds.Count > 0)
            {
                mDL.DataIDList.Clear();
                for (int i = 0; i < strIds.Count; i++)
                {
                    if (!mDL.DataIDList.Contains(string.Format("inventory.{0}", strIds[i])) && !string.IsNullOrEmpty(strIds[i]))
                    {
                        mDL.RegisterDataID(string.Format("inventory.{0}", strIds[i]));
                    }
                }
            }
            else//添加无该物品时容错
            {
                if (!mDL.DataIDList.Contains("inventory"))
                {
                    mDL.RegisterDataID("inventory");
                }
            }
        }
    
        private void Remove()
        {
            if (string.IsNullOrEmpty(heroId)) return;
            strIds.Clear();
            mDL.DataIDList.Clear();
        }
    
        //获取服务器伙伴碎片id
        private List<string> GetServerGoodsId(bool isFrominventory,Hashtable data = null)
        {
            Hashtable inventoryData;
            if (isFrominventory)
            {
                DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);
            }
            else
            {
                inventoryData = data;
            }
            if (inventoryData == null)
            {
                return new List<string>();
            }
    
            List<string> keys = new List<string>();
            foreach (var item in inventoryData.Cast<DictionaryEntry>())
            {
                var va = item.Value as IDictionary;
                if (heroId.CompareTo(va["economy_id"].ToString()) == 0)
                {
                    keys.Add(item.Key.ToString());
                }
            }
            return keys;
        }
    
        public override void OnDisable()
        {
            Remove();
            heroId = "";
        }
    }
}
