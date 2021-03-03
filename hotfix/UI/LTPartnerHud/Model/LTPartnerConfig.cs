using UnityEngine;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public enum CultivateType
    {
        Info = 0,
        UpGrade,
        StarUp,
        Skill,
        Awaken,
        Skin,
    }

    public class LTPartnerConfig
    {
        #region Url & Params
        public const int Equip_BASE_MAX_VALUE = 500;
        public const int MAX_STAR = 6;
        public static int MAX_SKILL_LEVEL;
        public static int MAX_GRADE_LEVEL;
        public static int MAX_LEVEL;
        public static int MAX_HANDBOOKPAGE = 1;//各属性图鉴最大页数-1
        #endregion

        #region
        public static float MaxHpper = 0f;
        public static float Atkper = 0f;
        public static float Defper = 0f;
        public static float ChainATKper = 0f;
        public static float CritPper = 0f;
        public static float CritVper = 0f;
        public static float CritDper = 0f;
        public static float SpExtraper = 0f;
        public static float SpResper = 0f;
        public static float Speedper = 0f;
        public static float DmgReper = 0f;
        public static float DmgAddper = 0f;
        #endregion         

        #region DataPath
        public const string DATA_PATH_ROOT = "heroStats";
        #endregion            


        #region SpriteName

        public const string NormalQualityCellFrame = "Ty_Di_2";
        public const string ColorfulCellFrame = "Ty_Quality_Xuancai_Di";
        public static Dictionary<Hotfix_LT.Data.eRoleAttr, string> LEVEL_SPRITE_NAME_DIC = new Dictionary<Hotfix_LT.Data.eRoleAttr, string>
        {
            {Hotfix_LT.Data.eRoleAttr.Feng, "Ty_Attribute_Feng"},
            {Hotfix_LT.Data.eRoleAttr.Shui, "Ty_Attribute_Shui"},
            {Hotfix_LT.Data.eRoleAttr.Huo, "Ty_Attribute_Huo"},
            {Hotfix_LT.Data.eRoleAttr.None ,""},
        };
        
        // public static Dictionary<Hotfix_LT.Data.eRoleAttr, string> LEVEL_SPRITE_NAME_DIC = new Dictionary<Hotfix_LT.Data.eRoleAttr, string>
        // {
        //     {Hotfix_LT.Data.eRoleAttr.Feng, "Shenqi_Icon_1_Feng"},
        //     {Hotfix_LT.Data.eRoleAttr.Shui, "Shenqi_Icon_1_Huo"},
        //     {Hotfix_LT.Data.eRoleAttr.Huo, "Shenqi_Icon_1_Shui"},
        //     {Hotfix_LT.Data.eRoleAttr.None ,""},
        // };

        public static Dictionary<int, string> OUT_LINE_SPRITE_NAME_DIC = new Dictionary<int, string>
        {
            {0, "Ty_Quality_Baise"},
            {1, "Ty_Quality_Green"},
            {2, "Ty_Quality_Blue"},
            {3, "Ty_Quality_Violet"},
            {4, "Ty_Quality_Yellow"},
            {5, "Ty_Quality_Gules"},
            { 6,"Ty_Quality_Xuancai"},
        };

        public static Dictionary<int, Color32> QUANTITY_BG_COLOR_DIC = new Dictionary<int, Color32>
        {
            {0, LT.Hotfix.Utility.ColorUtility.FrameWhiteColor},
            {1, LT.Hotfix.Utility.ColorUtility.FrameGreenColor},
            {2, LT.Hotfix.Utility.ColorUtility.FrameBlueColor},
            {3, LT.Hotfix.Utility.ColorUtility.FramePurpleColor},
            {4, LT.Hotfix.Utility.ColorUtility.FrameYellowColor},
            {5, LT.Hotfix.Utility.ColorUtility.FrameRedColor},
            {6, LT.Hotfix.Utility.ColorUtility.WhiteColor},
        };

        public static Dictionary<PartnerGrade, string> PARTNER_GRADE_SPRITE_NAME_DIC = new Dictionary<PartnerGrade, string>
        {
            { PartnerGrade.UR, "Ty_Strive_Icon_UR"},
            { PartnerGrade.SSR , "Ty_Strive_Icon_SSR"},
            { PartnerGrade.SR , "Ty_Strive_Icon_SR"},
            { PartnerGrade.R , "Ty_Strive_Icon_R"},
            { PartnerGrade.N , "Ty_Strive_Icon_N"},
            { PartnerGrade .ALL,""}
        };

        // public const string CULTIVATE_TAB_OPEN = "Ty_Title_Choice_1";
        // public const string CULTIVATE_TAB_CLOSE = "Ty_Title_Choice_2";

        // public static Dictionary<Hotfix_LT.Data.eRoleAttr, string> PARTNER_STAR_UP_BG_SPRITE_NAME_DIC = new Dictionary<Hotfix_LT.Data.eRoleAttr, string>
        // {
        //     { Hotfix_LT.Data.eRoleAttr.Huo, "Partner_Star_Di2"},
        //     { Hotfix_LT.Data.eRoleAttr.Feng, "Partner_Star_Di1"},
        //     { Hotfix_LT.Data.eRoleAttr.Shui, "Partner_Star_Di3"},
        // };

        public static Dictionary<Hotfix_LT.Data.eRoleAttr, string> PARTNER_STAR_ITEM_SPRITE_NAME_DIC = new Dictionary<Hotfix_LT.Data.eRoleAttr, string>
        {
            { Hotfix_LT.Data.eRoleAttr.Huo, "Partner_Star_Huo"},
            { Hotfix_LT.Data.eRoleAttr.Feng, "Partner_Star_Feng"},
            { Hotfix_LT.Data.eRoleAttr.Shui, "Partner_Star_Shui"},
        };

        public static Dictionary<string, string> PARTNER_STAR_UP_ATTR_NAME_DIC = new Dictionary<string, string>
        {
            { "MaxHP","ID_ATTR_HP"},
            { "ATK","ID_ATTR_ATK"},
            { "DEF","ID_ATTR_DEF"},
        };

        public static Dictionary<PartnerGrade, string> PARTNER_GRADE_STR_DIC = new Dictionary<PartnerGrade, string>
        {
            { PartnerGrade.UR, "UR"},
            { PartnerGrade.SSR , "SSR"},
            { PartnerGrade.SR , "SR"},
            { PartnerGrade.R , "R"},
            { PartnerGrade.N , "N"},
            { PartnerGrade .ALL,""}
        };
        public static Dictionary<int, string> PARTNER_AWAKN_STAR_DIC = new Dictionary<int, string>
        {
            { 0, "Ty_Icon_Xingxing"},
            { 1, "Ty_Icon_Xingxing2"}
        };

        public static Dictionary<int, string> PARTNER_AWAKN_SKILLFRAME_DIC = new Dictionary<int, string>
        {
            { 0, "Ty_Combat_Frame_Skill_Di1"},
            { 1, "Ty_Combat_Frame_Skill_Di2"}
        };

        public static Dictionary<int, string> PARTNER_STRATEGY_LEVEL_SPRITE = new Dictionary<int, string>
        {
            { 1,"Illustration_Word_D"},
            { 2,"Illustration_Word_C"},
            { 3,"Illustration_Word_B"},
            { 4,"Illustration_Word_A"},
            { 5,"Illustration_Word_S"},
        };
        public static Dictionary<int, Color> PARTNER_STRATEGY_LEVEL_COLOR = new Dictionary<int, Color>
        {
            { 1,new Color(0.28f,1f,0.33f)},
            { 2,new Color(0.27f,0.65f,1f)},
            { 3,new Color(0.16f,1f,0.93f)},
            { 4,new Color(1f,0.16f,0.94f)},
            { 5,new Color(1f,0.96f,0)},
        };

        #endregion

        #region DATA
        public static Dictionary<int, int> UP_GRADE_ID_DIC = new Dictionary<int, int>
        {
        };

        public static Dictionary<int, int> SKILL_BREAK_LEVEL_EXP_DIC = new Dictionary<int, int>
        {
            {0, 0 },//容错，服务器的等级可能从0开始，表里面从1开始（ps：其实是服务器没有改）
            {16,50000 }
        };

        
        //技能上限等级
        // public static Dictionary<int, int> SKILL_BREAK_LIMIT_LEVEL_DIC = new Dictionary<int, int>
        // {
        // };
        #endregion

        public static void SetLevelSprite(UISprite levelSprite,eRoleAttr char_type, bool b)
        {
            // b = true;
            levelSprite.keepAspectRatio = UIWidget.AspectRatioSource.Free;
            levelSprite.width = b ? (int)(1.2 * levelSprite.height) : levelSprite.height;
            string prefix = b ? "A_" : string.Empty;
            levelSprite.spriteName = prefix + LEVEL_SPRITE_NAME_DIC[char_type];
        }
    }
}