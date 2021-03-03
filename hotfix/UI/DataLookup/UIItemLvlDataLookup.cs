using Hotfix_LT.Player;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UIItemLvlDataLookup : DataLookupHotfix
    {
        //白绿蓝紫黄红彩
        //Fanqi,Liqi,Mingqi,Xianqi,Shenqi,chuanshuo
        public static string FanqiStr = "Ty_Quality_Baise";
        public static string LiqiStr = "Ty_Quality_Green";
        public static string MingqiStr = "Ty_Quality_Blue";
        public static string XianqiStr = "Ty_Quality_Violet";
        public static string ShenqiStr = "Ty_Quality_Yellow";
        public static string ShengqiStr = "Ty_Quality_Gules";
        public static string ChuanshuoStr = "Ty_Quality_Xuancai";
        //普通item框底板
        public static string ordinaryFramestr = "Ty_Di_2";
        public static string XuancaiFramestr = "Ty_Quality_Xuancai_Di";

        public static string LvlToStr(string lvl)
        {
            switch (lvl)
            {
                case "Poor":
                    return "";
                case EconomyConstants.Quality.POOR:
                    return FanqiStr;
                case EconomyConstants.Quality.COMMON:
                    return LiqiStr;
                case EconomyConstants.Quality.UNCOMMON:
                    return MingqiStr;
                case EconomyConstants.Quality.EPIC:
                    return XianqiStr;
                case EconomyConstants.Quality.LEGENDARY:
                    return ShenqiStr;
                case EconomyConstants.Quality.HALLOWS:
                    return ShengqiStr;
                case EconomyConstants.Quality.LEGEND:
                    return ChuanshuoStr;
                default:
                    return "";
            }
        }

        public static string GetItemFrameBGSprite(string lvl)
        {
            switch (lvl)
            {
                case "Poor":
                    return "";
                case EconomyConstants.Quality.POOR:
                    return ordinaryFramestr;
                case EconomyConstants.Quality.COMMON:
                    return ordinaryFramestr;
                case EconomyConstants.Quality.UNCOMMON:
                    return ordinaryFramestr;
                case EconomyConstants.Quality.EPIC:
                    return ordinaryFramestr;
                case EconomyConstants.Quality.LEGENDARY:
                    return ordinaryFramestr;
                case EconomyConstants.Quality.HALLOWS:
                    return ordinaryFramestr;
                case EconomyConstants.Quality.LEGEND:
                    return XuancaiFramestr;
                default:
                    return "";
            }
        }

        public static string GetItemFrameBGSprite(int Quality)
        {
            switch (Quality)
            {
                case 1:
                    return ordinaryFramestr;
                case 2:
                    return ordinaryFramestr;
                case 3:
                    return ordinaryFramestr;
                case 4:
                    return ordinaryFramestr;
                case 5:
                    return ordinaryFramestr;
                case 6:
                    return ordinaryFramestr;
                case 7:
                    return XuancaiFramestr;
                default:
                    return ordinaryFramestr;
            }
        }

        public static Color GetItemFrameBGColor(string lvl)
        {
            switch (lvl)
            {
                case "Poor":
                    return LT.Hotfix.Utility.ColorUtility.FrameWhiteColor;
                case EconomyConstants.Quality.POOR:
                    return LT.Hotfix.Utility.ColorUtility.FrameWhiteColor;
                case EconomyConstants.Quality.COMMON:
                    return LT.Hotfix.Utility.ColorUtility.FrameGreenColor;
                case EconomyConstants.Quality.UNCOMMON:
                    return LT.Hotfix.Utility.ColorUtility.FrameBlueColor;
                case EconomyConstants.Quality.EPIC:
                    return LT.Hotfix.Utility.ColorUtility.FramePurpleColor;
                case EconomyConstants.Quality.LEGENDARY:
                    return LT.Hotfix.Utility.ColorUtility.FrameYellowColor;
                case EconomyConstants.Quality.HALLOWS:
                    return LT.Hotfix.Utility.ColorUtility.FrameRedColor;
                case EconomyConstants.Quality.LEGEND:
                    return LT.Hotfix.Utility.ColorUtility.WhiteColor;
                default:
                    return LT.Hotfix.Utility.ColorUtility.FrameWhiteColor;
            }
        }

        public static string GetEquipLevelBGStr(int quality)
        {
            switch (quality)
            {
                case 1:
                    return "Ty_Quality_Blue_Di";
                case 2:
                    return "Ty_Quality_Green_Di";
                case 3:
                    return "Ty_Quality_Blue_Di";
                case 4:
                    return "Ty_Quality_Purple_Di";
                case 5:
                    return "Ty_Quality_Yellow_Di";
                case 6:
                    return "Ty_Quality_Red_Di";
                case 7:
                    return "Ty_Quality_Xuancai_Di1";
                default:
                    EB.Debug.LogError("item quality error quality={0}" , quality);
                    return "Ty_Quality_Red_Di";
            }
        }

        public override void OnLookupUpdate(string dataID, object value)
        {
            base.OnLookupUpdate(dataID, value);

            string spname = string.Empty;
            var upgrade_id = GetUpgradeId();

            if (upgrade_id != null)
            {
                //var item = EconemyTemplateManager.Instance.GetItem(upgrade_id);  TODOX
                //spname = UIItemLvlDataLookup.LvlToStr(item.QualityLevel.ToString());
            }
            else
            {
                string LvlName = mDL.GetDefaultLookupData<string>();
                spname = UIItemLvlDataLookup.LvlToStr(LvlName);
            }

            UISprite uiSprite = mDL.transform.GetComponent<UISprite>();

            if (uiSprite != null)
            {
                uiSprite.spriteName = spname;
            }
        }

        private string GetUpgradeId()
        {
            string item_id;

            if (!DataLookupsCache.Instance.SearchDataByID("next_upgrade_id", out item_id))
            {
                return null;
            }

            return item_id;
        }
    }
}