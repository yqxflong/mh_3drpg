using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipConfig
    {
        public static Dictionary<bool, string> HasEffectStrDic = new Dictionary<bool, string>(){
        {true,"[42fe79]"},
        {false,"[000000]"}
        };

        #region 用于构建装备图标需要拼接使用
        public static Dictionary<int, string> EquipIconTypeStr = new Dictionary<int, string>()
        {
            { 1,"Goods_Equip_Weapon_" },
            { 2,"Goods_Equip_Helmet_" },
            { 3,"Goods_Equip_Armour_"},
            { 4,"Goods_Equip_Gloves_"},
            { 5,"Goods_Equip_Legs_"},
            { 6,"Goods_Equip_Shoe_"}
        };

        public static Dictionary<int, string> EquipIconQualityStr = new Dictionary<int, string>()
        {
            { 1,"White" },
            { 2,"Green" },
            { 3,"Blue"},
            { 4,"Purple"},
            { 5,"Yellow"},
            { 6,"Red"},
            { 7,"Xuancai"}
        };
        #endregion
    }
}