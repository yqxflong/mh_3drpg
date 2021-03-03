using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using Hotfix_LT.Player;

namespace Hotfix_LT.UI
{
    public class GameItemUtil
    {
        /// <summary>
        /// 获取背包中物品列表(可知总数)
        /// </summary>
        /// <param name="dic"></param>
        public static void QueryAllBagItemsSum(Dictionary<string ,int> dic)
        {
            Hashtable inventoryData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);

            if (inventoryData != null)
            {
                //准备筛选过的所有背包物品
                var it = inventoryData.GetEnumerator();

                while(it.MoveNext())
                {
                    var ht = it.Value as IDictionary;

                    if (ht == null)
                    {
                        continue;
                    }

                    string loc = ht["location"] as string;

                    if (loc != null && loc.CompareTo("bag_items") == 0)
                    {
                        string economyId = ht["economy_id"].ToString();
                        int sum;
                        dic.TryGetValue(economyId, out sum);
                        dic[economyId] = sum + Convert.ToInt32(ht["num"]); 
                    }
                }
            }
        }

        /// <summary>
        /// 统计背包中某个物品的个数
        /// </summary>
        /// <returns></returns>
        public static int GetInventoryItemNum(string itemId)
        {
            //获取背包数据
            Hashtable inventoryData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);
            if (inventoryData == null)
            {
                return 0;
            }

            int sum = 0;
            var it = inventoryData.GetEnumerator();
            while(it.MoveNext()){
                var ht = it.Value as IDictionary;
                if(ht["economy_id"].ToString() == itemId
                   && ht["location"].ToString().CompareTo("bag_items") == 0)
                {
                    sum += int.Parse(ht["num"].ToString());
                }
            }
            return sum;
        }

        public static int GetInventoryItemNum(string itemId,out string inventoryid)
        {
            //获取背包数据
            Hashtable inventoryData;
            inventoryid = string.Empty;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("inventory", out inventoryData);
            if (inventoryData == null)
            {               
                return 0;
            }

            int sum = 0;
            var it = inventoryData.GetEnumerator();
            while (it.MoveNext())
            {
                var ht = it.Value as IDictionary;
                if (ht["economy_id"].ToString() == itemId
                   && ht["location"].ToString().CompareTo("bag_items") == 0)
                {
                    sum += int.Parse(ht["num"].ToString());
                    inventoryid = it.Key.ToString();
                }
            }
            return sum;
        }

        public static int GetInventoryItemNum(int itemId)
        {
            return GetInventoryItemNum(itemId.ToString());
        }

        public static int GetItemAlreadyHave(string id, string type)
        {
            if (type.Equals(LTShowItemType.TYPE_GAMINVENTORY))
            {
                return GetInventoryItemNum(id);
            }
            else if (type.Equals(LTShowItemType.TYPE_HERO))
            {
                return GetHeroAlreadyHave(id);
            }
            else if (type.Equals(LTShowItemType.TYPE_HEROSHARD))
            {
                return GetHeroShardAlreadyHave(id);
            }
            else if (type.Equals(LTShowItemType.TYPE_RES) || type.Equals(LTShowItemType.TYPE_ACTICKET))
            {
                return BalanceResourceUtil.GetResValue(id);
            }
            else
            {
                return 0;
            }
        }

        public static bool GetItemIsCanBuy(string id, string type, out int messageId)
        {
            messageId = 0;
            return true;
        }
        
        public static string GetProfessionHeroInfoId()
        {
            string character_id = null;
            DataLookupsCache.Instance.SearchDataByID<string>("character_id", out character_id);
            return character_id;
        }
        
        public static int GetHeroAlreadyHave(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            IDictionary buddys;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("heroStats", out buddys)) return 0;
            else
            {
                foreach (DictionaryEntry de in buddys)
                {
                    string character_id = EB.Dot.String("character_id", de.Value, "");
                    int star = EB.Dot.Integer("star", de.Value, 0);
                    if (star > 0 && id.Equals(character_id)) return 1;
                }
                return 0;
            }
        }

        public static int GetHeroShardAlreadyHave(string id)
        {
            if (string.IsNullOrEmpty(id)) return 0;
            IDictionary buddys;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("heroStats", out buddys)) return 0;
            else
            {
                foreach (DictionaryEntry de in buddys)
                {
                    string character_id = EB.Dot.String("character_id", de.Value, "");
                    if (id.Equals(character_id))
                    {
                        int num = EB.Dot.Integer("shard", de.Value, 0);
                        return num;
                    }
                }
                return 0;
            }
        }
        //设置伙伴炫彩头像框
        public static void SetColorfulPartnerCellFrame(int quality, UISprite FrameBGSprite)
        {
            if (quality < 6)
                FrameBGSprite.spriteName = LTPartnerConfig.NormalQualityCellFrame;
            else
                FrameBGSprite.spriteName = LTPartnerConfig.ColorfulCellFrame;
            FrameBGSprite.color = LTPartnerConfig.QUANTITY_BG_COLOR_DIC[quality];
        }
    }
}