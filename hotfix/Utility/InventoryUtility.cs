using Hotfix_LT.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace LT.Hotfix.Utility {
    public static class InventoryUtility {
        /// <summary>
        /// 该背包物品是否已附加到伙伴身上
        /// </summary>
        /// <param name="eid">背包物品id</param>
        public static bool IsEquipped(int eid) {
            string location;
            DataLookupsCache.Instance.SearchDataByID(string.Format("inventory.{0}.location", eid), out location);
            return location.Equals("equipment");
        }

        /// <summary>
        /// 该背包物品是否属于装备预设内容
        /// </summary>
        /// <param name="eid">背包物品id</param>
        public static bool IsInEquipmentPreset(int eid) {
            IDictionary dict;
            DataLookupsCache.Instance.SearchDataByID("equipment_preset", out dict);

            if (dict == null) {
                return false;
            }

            var enumerator = dict.GetEnumerator();

            while (enumerator.MoveNext()) {
                var ht = enumerator.Value as Hashtable;

                for (var i = 1; i <= 6; i++) {
                    if (Convert.ToInt32(ht[i.ToString()]) == eid) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取包含该背包物品的所有装备预设名称集合
        /// </summary>
        /// <param name="eid">背包物品id</param>
        public static List<string> GetAllEquipmentPresetNameContains(int eid) {
            IDictionary dict;
            DataLookupsCache.Instance.SearchDataByID("equipment_preset", out dict);
            
            if (dict == null) {
                return null;
            }

            var enumerator = dict.GetEnumerator();
            var list = new List<string>();

            while (enumerator.MoveNext()) {
                var ht = enumerator.Value as Hashtable;

                for (var i = 1; i <= 6; i++) {
                    if (Convert.ToInt32(ht[i.ToString()]) == eid) {
                        list.Add(enumerator.Key.ToString());
                        break;
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取装备预设中的所有背包物品id
        /// </summary>
        public static HashSet<int> GetIdsInEquipmentPreset() {
            IDictionary dict;
            DataLookupsCache.Instance.SearchDataByID("equipment_preset", out dict);
            var hashSet = new HashSet<int>();

            if (dict == null) {
                return hashSet;
            }

            var enumerator = dict.GetEnumerator();

            while (enumerator.MoveNext()) {
                var ht = enumerator.Value as Hashtable;

                for (var i = 1; i <= 6; i++) {
                    var eid = Convert.ToInt32(ht[i.ToString()]);

                    if (!hashSet.Contains(eid)) {
                        hashSet.Add(eid);
                    }
                }
            }

            return hashSet;
        }

        /// <summary>
        /// 获取背包物品所附加到的伙伴-HeroId
        /// </summary> 
        /// <param name="eid">背包物品id</param>
        public static int GetHeroId(int eid) {
            int heroId;
            DataLookupsCache.Instance.SearchDataByID(string.Format("inventory.{0}.position", eid), out heroId);
            return heroId;
        }

        /// <summary>
        /// 获取背包物品所附加到的伙伴-名称
        /// </summary>
        /// <param name="eid">背包物品id</param>
        public static string GetPartnerName(int eid) {
            var heroId = GetHeroId(eid);
            var partnerData = LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId);
            return partnerData != null ? partnerData.HeroInfo.name : string.Empty;
        }

        /// <summary>
        /// 获取背包物品所附加到的伙伴数据
        /// </summary>
        /// <param name="eid">背包物品id</param>
        public static LTPartnerData GetPartnerData(int eid) {
            var heroId = GetHeroId(eid);
            return heroId > 0 ? LTPartnerDataManager.Instance.GetPartnerByHeroId(heroId) : null; 
        }

        /// <summary>
        /// 获取背包中装备物品名称
        /// </summary>
        /// <param name="eid">背包物品id</param>
        public static string GetEquipmentName(int eid) {
            var info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
            return info != null ? info.Name : string.Empty;
        }
    }
}