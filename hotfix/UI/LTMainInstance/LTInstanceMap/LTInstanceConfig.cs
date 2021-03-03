using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTInstanceConfig
    {
        public enum InstanceType
        {
            None = 0,
            MainInstance,
            ChallengeInstance,
            AlienMazeInstance,
            MonopolyInstance,
        }

        public const string InChallengeState = "InChallengeState";
        public const string OutChallengeState = "OutChallengeState";
        public const string AlienMazeTypeStr = "1";

        public const string MonopolyDice1 = "normal";
        public const string MonopolyDice2 = "control";

        #region Player Model

        public static float MODEL_MOVE_TIME = 0.3f;
        
        public static Vector3 LEFT_MODEL_ROTATION = new Vector3(20, -130, 20);

        public static Vector3 RIGHT_MODEL_ROTATION = new Vector3(-20,50, -20);

        public static Vector3 TOP_MODEL_ROTATION = new Vector3(-20, -50, 20);

        public static Vector3 BOTTOM_MODEL_ROTATION = new Vector3(20,130, -20);


        #endregion

        #region Instance Map

        public const int MAP_SCALE = 2;

        public const float MAP_X =0.9f;

        public const float MAP_Y = 0.4f;

        public const float MAP_XZ = 1;

        public const float MAP_YZ = 1;

        public const float MAP_MODLE_SCALE = 0.6f;

        public const float MAP_BOSS_SCALE = 0.6f;

        public const int SMALL_MAP_SCALE = 58;

        public const int SMALL_MAP_SCALE_X = 62;

        public const int SMALL_MAP_SCALE_Y = 32;
        
        #endregion
        
        #region Special Role Sprite

        public static List<string> EmptyRoleList = new List<string>()
        {
            "Start",//起点
            "Copy_Map_Boss",//通关的BOSS点
            "Copy_Map_Xiaoguai",//通关的小怪点
            "Door",//挑战副本的门
            "Bomb",//超级炸弹格子
        };

        public static List<string> CanNotStandList = new List<string>()
        {
            "Copy_Icon_Mimabaoxiang",//密码宝箱
        };

        /// <summary>
        /// 图片所对应的特效名称
        /// </summary>
        public static Dictionary<string, string> RoleEffectNameDic = new Dictionary<string, string>()
        {
            {"Copy_Icon_Fengyinchuansongmen", "fx_fb_chuangshongmen" },
            {"#Copy_Icon_Fengyinchuansongmen", "fx_fb_chuangshongmen" },
            {"Copy_Icon_Fengyinchuansongmen2" ,"fx_ui_fb_chuangshongmen_02"},
            {"Copy_Icon_Yaoshui", "fx_fb_Stars" },
            {"Copy_Icon_Moli", "fx_fb_Stars" }
        };
        
        /// <summary>
        /// 图片所对应的预设名称(不包含RoleBase)
        /// </summary>
        public static Dictionary<string, string> RoleItemNameDic = new Dictionary<string, string>()
        {
            {"Copy_Icon_Fengyinchuansongmen", "RoleMen1" },
            {"#Copy_Icon_Fengyinchuansongmen", "RoleMen2" },
            {"Copy_Icon_Fengyinchuansongmen2","RoleMen1" },

            {"Copy_Icon_Yaoshui", "RoleYaoshui" },
            {"Copy_Icon_Jingyanyaoshui", "RoleYaoshui" },
            {"Copy_Icon_Moli", "RoleYaoshui" },

            {"Copy_Icon_Moliquanshui", "RoleMoliquanshui" },

            {"Copy_Icon_Jinengjuanzhou", "RoleSkill" },

            {"Copy_Icon_Xianjing", "RoleXianjing" },
            {"Copy_Icon_Molixianjing", "RoleMolixianjing" },
        };

        public static Vector3 GuideNoticePos = new Vector3(-162, 100, 0);

        public static Vector3 HeroNoticePos = new Vector3(162, 100, 0);

        #endregion
    }
}