using Hotfix_LT.UI;
using System.Collections.Generic;
using UnityEngine;

namespace LT.Hotfix.Utility {
    public enum EquipmentSortType {
        Level,
        Quality,
        GetOrder,
        Speed,
        ATK,
        ATKrate,
        DEF,
        DEFrate,
        MaxHP,
        MaxHPrate,
        CritP,
        CritV,
        SpExtra,
        SpRes
    }

    public static class EquipmentUtility {
        public static List<EquipmentSortType> GetSelectSortList() {
            return new List<EquipmentSortType>() {
                EquipmentSortType.Level,
                EquipmentSortType.Quality,
                EquipmentSortType.GetOrder,
                EquipmentSortType.Speed,
                EquipmentSortType.ATK,
                EquipmentSortType.ATKrate,
                EquipmentSortType.DEF,
                EquipmentSortType.DEFrate,
                EquipmentSortType.MaxHP,
                EquipmentSortType.MaxHPrate,
                EquipmentSortType.CritP,
                EquipmentSortType.CritV,
                EquipmentSortType.SpExtra,
                EquipmentSortType.SpRes
            };
        }

        /// <summary>
        /// 装备属性多语言转换
        /// </summary>
        /// <param name="str">装备属性名称</param>
        /// <param name="withSeparator">是否使用分隔符</param>
        /// <returns></returns>
        public static string AttrTypeTrans(string str, bool withSeparator = true) {
            string separator = withSeparator ? "：" : "";

            switch (str) {
                case "ATK":
                    return EB.Localizer.GetString("ID_ATTR_ATK") + separator;
                case "MaxHP":
                    return EB.Localizer.GetString("ID_ATTR_MaxHP") + separator;
                case "DEF":
                    return EB.Localizer.GetString("ID_ATTR_DEF") + separator;
                case "CritP":
                    return EB.Localizer.GetString("ID_ATTR_CritP") + separator;
                case "CritV":
                    return EB.Localizer.GetString("ID_ATTR_CritV") + separator;
                case "ChainAtk":
                    return EB.Localizer.GetString("ID_ATTR_ChainAtk") + separator;
                case "SpExtra":
                    return EB.Localizer.GetString("ID_ATTR_SpExtra") + separator;
                case "SpRes":
                    return EB.Localizer.GetString("ID_ATTR_SpRes") + separator;
                case "MaxHPrate":
                    return EB.Localizer.GetString("ID_ATTR_MaxHPrate") + separator;
                case "ATKrate":
                    return EB.Localizer.GetString("ID_ATTR_ATKrate") + separator;
                case "DEFrate":
                    return EB.Localizer.GetString("ID_ATTR_DEFrate") + separator;
                case "Speed":
                case "speed":
                    return EB.Localizer.GetString("ID_ATTR_Speed") + separator;
                case "speedrate":
                    return EB.Localizer.GetString("ID_ATTR_speedrate") + separator;
                case "Level":
                    return EB.Localizer.GetString("ID_LEVEL") + separator;
                case "Quality":
                    return EB.Localizer.GetString("ID_QUALITY") + separator;
                case "GetOrder":
                    return EB.Localizer.GetString("ID_GET_ORDER") + separator;
                case "DmgMulti":
                    return EB.Localizer.GetString("ID_ATTR_DMGincrease") + separator;
                case "DmgRes":
                    return EB.Localizer.GetString("ID_ATTR_DMGreduction") + separator;
                default:
                    return EB.Localizer.GetString("ID_ATTR_Unknown") + separator;
            }
        }

        /// <summary>
        /// 装备属性显示样式转换
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string AttrTypeValue(EquipmentAttr data) {
            switch (data.Name) {
                case "ATK":
                    return ("+" + Mathf.FloorToInt(data.Value).ToString());
                case "MaxHP":
                    return ("+" + Mathf.FloorToInt(data.Value).ToString());
                case "DEF":
                    return ("+" + Mathf.FloorToInt(data.Value).ToString());
                case "CritP":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "CritV":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "ChainAtk":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "SpExtra":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "SpRes":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "MaxHPrate":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "ATKrate":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "DEFrate":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "Speed":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "speedrate":
                    return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                default:
                    return EB.Localizer.GetString("ID_ATTR_Unknown");
            }
        }
    }
}