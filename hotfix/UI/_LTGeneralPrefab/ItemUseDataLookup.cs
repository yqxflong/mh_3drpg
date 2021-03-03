using System.Collections;

namespace Hotfix_LT.UI
{
    public class ItemUseDataLookup : DataLookupHotfix
    {
        private Hashtable resCache = Johny.HashtablePool.Claim();
        private Hashtable showCahe = Johny.HashtablePool.Claim();
    
        public override void OnLookupUpdate(string dataID, object value)
        {
            if (value == null) return;
    
            if (resCache.ContainsKey(dataID))
            {
                int curValue = 0;
                int.TryParse(value.ToString(), out curValue);
                int lastValue = 0;
                int.TryParse(resCache[dataID].ToString(), out lastValue);
                if (curValue > lastValue)
                {
                    string resId = string.Empty;
                    string[] split = dataID.Split('.');
                    if (split.Length > 2)
                    {
                        resId = split[1];
                    }
                    if (!string.IsNullOrEmpty(resId))
                    {
                        int addResnum = curValue - lastValue;
                        if (showCahe.ContainsKey(resId))
                        {
                            int showResNum = 0;
                            int.TryParse(showCahe[resId].ToString(), out showResNum);
                            showResNum = showResNum + addResnum;
                            showCahe[resId] = showResNum;
                        }
                        else
                        {
                            showCahe[resId] = addResnum;
                        }
                        resCache[dataID] = value;
                    }
                }
                else
                {
                    resCache[dataID] = value;
                }
            }
            else
            {
                resCache[dataID] = value;
            }
        }
    
        public void ClearShowCache()
        {
            showCahe = Johny.HashtablePool.Claim();
        }
    
        public void ShowAward()
        {
            foreach (object curItemKey in showCahe.Keys)
            {
                string resId = curItemKey.ToString();
                string curItemValue = showCahe[curItemKey].ToString();
                int num = 0;
                int.TryParse(curItemValue, out num);
                LTShowItemData itemData = new LTShowItemData(resId, num, "res");
                GameUtils.ShowAwardMsg(itemData);
            }
            showCahe.Clear();
        }
    }
}
