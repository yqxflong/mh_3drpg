using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class SmallPartnerPacketRule
    {
        #region server data

        #region hero stats
        public static readonly string OWN_HERO_STATS = "heroStats";
        public static readonly string HERO_TEMPLATE_ID = "template_id";
        public static readonly string HERO_SKIN = "skin";
        #endregion

        #region buddy inventory
        public static readonly string BUDDY_INVENTORY = "buddyinventory";
        #endregion

        #region team data
        public static readonly string USER_TEAM = "userTeam";
        public static readonly string USER_TEAM_FORMATION = "formation_info";
        public static readonly string USER_TEAM_TEAM_INFO = "team_info";
        public static readonly string USER_TEAM_FORMATION_HERO_ID = "hero_id";
        public static readonly List<string> USER_TEAM_LIST = new List<string>
        {
            "team1",
            "team2",
            "team3",
            "arena",
            "lt_challenge_camp",
            "nation1",
            "nation2",
            "nation3",
            "awar",
            "lt_coh",
            "lt_aw_camp",
            "lt_st",
            "honor_attack_1",
            "honor_attack_2",
            "honor_attack_3",
            "honor_defend_1",
            "honor_defend_2",
            "honor_defend_3",
            "lt_mercenary"
        };
        public static readonly string USER_TEAM_CUR_TEAM = "current_team";
        #endregion

        #endregion


        #region error code
        public static readonly int ADD_FORMATION_CODE1 = 902047;
        public static readonly int ADD_FORMATION_CODE4 = 902183;
        #endregion

        #region other
        public static int TEAM_MAX_NUM
        {
            get
            {
                if (!Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10046).IsConditionOK())
                {
                    return 3;
                }
                else
                {
                    return 4;
                }
            }
        }
        #endregion

        #region other player data
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM0 = "uid";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM1 = "type";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE0 = "arena";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE1 = "normal";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE2 = "alli_war";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM2 = "data_type";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE0 = "mirror";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE1 = "current";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM3 = "data";
        public static readonly string REQUEST_OTHER_PLAYER_DATA_PARAM3_TYPE0 = "ranking";


        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA0 = "id";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA1 = "template_id";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA2 = "stat.level";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA3 = "star";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA4 = "type";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA6 = "charId";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA7 = "stat.upgrade";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA8 = "skin";
        public static readonly string OTHER_PLAYER_DATA_PARAM1_DATA9 = "stat.awaken";
        public static readonly string OTHER_PALYER_TYPE2 = "enemy";
        public static readonly string OTHER_PALYER_Fetter = "fetter";
        
        public static readonly string OTHER_PALYER_ALLIANCE = "alliance";

        #endregion
    }
}