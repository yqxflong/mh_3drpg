using Hotfix_LT.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 物品类型
    /// </summary>
    public class LTShowItemType
    {
        //可识别类型
        public static string TYPE_RES = "res";
        public static string TYPE_GAMINVENTORY = "gaminventory";
        public static string TYPE_HERO = "hero";
        public static string TYPE_HEROSHARD = "heroshard";
        public static string TYPE_SCROLL = "scroll";
        // public static string TYPE_DONATE = "donate";
        public static string TYPE_ACTIVITY = "act";
        public static string TYPE_HEROMEDAL = "heromedal";
        public static string TYPE_ACTICKET = "acticket";
        public static string TYPE_HEADFRAME = "headframe";
        public static string TYPE_VIPPOINT = "vippoint";
        //不可识别类型
        public static string TYPE_MCARD = "mcard";
        public static string TYPE_BPT = "bpt";
        public static string TYPE_PAD = "pab";
        public static string TYPE_PDB = "pdb";

        // public static List<string> ChargeIgnoreType = new List<string> { "mcard", "bpt", "pab", "pdb", "heromedal", "act" };
    }

    /// <summary>
    /// 物品类型-res资源的对照表
    /// </summary>
    public class LTResID
    {
        public const string HcName = "hc";
        public const string GoldName = "gold";
        // public const string VigorName = "vigor";
        // public const string XpName = "xp"; public const string ExpName = "exp";
        public const string BuddyExpName = "buddy-exp";
        // public const string AllianceGoldName = "alliance-gold";
        // public const string NationGoldName = "nation-gold";
        // public const string LadderGoldName = "ladder-gold";
        // public const string ArenaGoldName = "arena-gold";
        // public const string HeroGoldName = "hero-gold";
        // public const string ChallCampPointName = "chall-camp-point";
        // public const string PotenGoldName = "poten-gold";
        // public const string ShilianName = "shilian";
        // public const string ScoreName = "score";
        // public const string HandbookPointName = "handbook-point";
        // public const string UnknowName = "unknow";

        // public static string GetResStrID(int id)
        // {
        //     switch (id)
        //     {
        //         case 1: return HcName;
        //         case 2: return GoldName;
        //         case 3: return VigorName;
        //         case 4: return XpName;
        //         case 5: return BuddyExpName;
        //         case 6: return AllianceGoldName;
        //         case 7: return NationGoldName;
        //         case 8: return LadderGoldName;
        //         case 9: return ArenaGoldName;
        //         case 10: return HeroGoldName;
        //         case 11: return ChallCampPointName;
        //         case 12: return PotenGoldName;
        //         case 13: return ShilianName;
        //         case 14: return ScoreName;
        //         case 15: return HandbookPointName;
        //         default: return UnknowName;
        //     }
        // }

        // public static int GetResID(string res_id)
        // {
        //     switch (res_id)
        //     {
        //         case HcName: return 1;
        //         case GoldName: return 2;
        //         case VigorName: return 3;
        //         case XpName: case ExpName: return 4;
        //         case BuddyExpName: return 5;
        //         case AllianceGoldName: return 6;
        //         case NationGoldName: return 7;
        //         case LadderGoldName: return 8;
        //         case ArenaGoldName: return 9;
        //         case HeroGoldName: return 10;
        //         case ChallCampPointName: return 11;
        //         case PotenGoldName: return 12;
        //         case ShilianName: return 13;
        //         case ScoreName: return 14;
        //         case HandbookPointName: return 15;
        //         default: return -1;
        //     }
        // }
    }

    /// <summary>
    /// 获取物品信息
    /// </summary>
    public class LTItemInfoTool
    {
        public static LTIconNameQuality GetInfo(string id, string type, bool nameWithColor = false)
        {
            string icon = "";
            string quality = "1";
            string name = "";

            if (type.Equals(LTShowItemType.TYPE_GAMINVENTORY))
            {
                var item = EconemyTemplateManager.Instance.GetItem(id);

                if (item != null)
                {
                    icon = item.IconId;
                    quality = item.QualityLevel.ToString();
                    name = item.Name;
                }
            }
            else if (type.Equals(LTShowItemType.TYPE_RES) || type.Equals(LTShowItemType.TYPE_HEROMEDAL)|| type.Equals(LTShowItemType.TYPE_VIPPOINT)|| type.Equals(LTShowItemType.TYPE_ACTICKET))
            {
                int resId = BalanceResourceUtil.GetResID(id);
                var item = EconemyTemplateManager.Instance.GetItem(resId);
                quality = "1";

                if (item != null)
                {
                    icon = item.IconId;
                    quality = item.QualityLevel.ToString();
                    name = item.Name;
                }
            }
            else if (type.Equals(LTShowItemType.TYPE_HERO))
            {
                bool isCharacterid = CharacterTemplateManager.Instance.HasHeroInfo(int.Parse(id));

                if (isCharacterid)
                {
                    var item = CharacterTemplateManager.Instance.GetHeroInfo(id);

                    if (item != null)
                    {
                        quality = (item.role_grade + 1).ToString();
                        name = item.name;
                        icon = item.icon;
                    }
                }
                else
                {
                    var temp = CharacterTemplateManager.Instance.GetHeroStat(id);
                    quality = "2";

                    if (temp != null)
                    {
                        var item = CharacterTemplateManager.Instance.GetHeroInfo(temp.character_id);

                        if (item != null)
                        {
                            quality = (item.role_grade + 1).ToString();
                            name = item.name;
                            icon = item.icon;
                        }
                    }
                }
            }
            else if (type.Equals(LTShowItemType.TYPE_HEROSHARD))
            {
                bool isCharacterid = CharacterTemplateManager.Instance.HasHeroInfo(int.Parse(id));

                if (isCharacterid)
                {
                    var item = CharacterTemplateManager.Instance.GetHeroInfo(id);

                    if (item != null)
                    {
                        quality = (item.role_grade + 1).ToString();
                        name = item.name;
                        icon = item.icon;
                    }
                }
                else
                {
                    var temp = CharacterTemplateManager.Instance.GetHeroStat(id);
                    quality = "2";

                    if (temp != null)
                    {
                        var item = CharacterTemplateManager.Instance.GetHeroInfo(temp.character_id);

                        if (item != null)
                        {
                            quality = (item.role_grade + 1).ToString();
                            name = item.name + EB.Localizer.GetString("ID_HERO_SHARD_FIX");
                            icon = item.icon;
                        }
                    }
                }
            }
            else if (type.Equals(LTShowItemType.TYPE_SCROLL))
            {
                SkillTemplate item = SkillTemplateManager.Instance.GetTemplate(int.Parse(id));
                quality = "2";

                if (item != null)
                {
                    name = item.Name;
                    icon = item.Icon;
                }
            }
            else if (type.Equals(LTShowItemType.TYPE_HEADFRAME))
            {
                var item = EconemyTemplateManager.Instance.GetHeadFrame(id, 1);
                quality = "5";

                if (item != null)
                {
                    name = item.name;
                    icon = item.iconId;
                }
            }
            else if (type.Equals(LTShowItemType.TYPE_ACTIVITY))
            {
                var item = EconemyTemplateManager.Instance.GetItem(id);

                if (id.Equals("2005"))//脚印
                {
                    item = EconemyTemplateManager.Instance.GetItem("2012");
                }

                if (item != null)
                {
                    quality = item.QualityLevel.ToString();
                    name = item.Name;
                    icon = item.IconId;
                }
            }

            string colorname = string.Format(LTGameColor.QualityToColorStr(quality, nameWithColor), name);
            return new LTIconNameQuality(type, icon, colorname, quality);
        }
    }

    /// <summary>
    /// 物品颜色相关
    /// </summary>
    public class LTGameColor
    {
        public struct Quality
        {
            public const string LEGEND_7 = "7";
            public const string HALLOWS_6 = "6";
            public const string LEGENDARY_5 = "5";
            public const string EPIC_4 = "4";
            public const string UNCOMMON_3 = "3";
            public const string COMMON_2 = "2";
            public const string POOR_1 = "1";
        }

        public struct QualityColorStr
        {
            public const string LEGEND = "[FF1C54]{0}[-]";
            public const string HALLOWS = "[FF6699]{0}[-]";
            public const string LEGENDARY = "[FFF348]{0}[-]";
            public const string EPIC = "[CC66FF]{0}[-]";
            public const string UNCOMMON = "[33B2FF]{0}[-]";
            public const string COMMON = "[42FE79]{0}[-]";
            public const string POOR = "[FFFFFF]{0}[-]";
        }

        public static string QualityToColorStr(string quality, bool nameWithColor = true)
        {
            if (!nameWithColor) return "{0}";

            switch (quality)
            {
                case Quality.POOR_1:
                    return QualityColorStr.POOR;
                case Quality.COMMON_2:
                    return QualityColorStr.COMMON;
                case Quality.UNCOMMON_3:
                    return QualityColorStr.UNCOMMON;
                case Quality.EPIC_4:
                    return QualityColorStr.EPIC;
                case Quality.LEGENDARY_5:
                    return QualityColorStr.LEGENDARY;
                case Quality.HALLOWS_6:
                    return QualityColorStr.HALLOWS;
                case Quality.LEGEND_7://炫彩品质需要特殊处理
                    return QualityColorStr.LEGEND;                  
                default:
                    return QualityColorStr.POOR;
            }
        }
    }

}