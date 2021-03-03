using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class ChatRule
    {
        public enum CHAT_CHANNEL
        {
            CHAT_CHANNEL_NONE = 0,
            CHAT_CHANNEL_WORLD = 1,
            CHAT_CHANNEL_TEAM = 2,
            CHAT_CHANNEL_ALLIANCE = 3,
            CHAT_CHANNEL_PRIVATE = 4,
            CHAT_CHANNEL_SYSTEM = 5,
            CHAT_CHANNEL_NATION = 6,

            CHAT_CHANNEL_ALLIANCEWAR = 7,
        }

        public static readonly Dictionary<CHAT_CHANNEL, string> CHANNEL2STR = new Dictionary<CHAT_CHANNEL, string>
        {
            {CHAT_CHANNEL.CHAT_CHANNEL_WORLD, "world"},
            {CHAT_CHANNEL.CHAT_CHANNEL_TEAM, "team"},
            {CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE, "alliance"},
            {CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE, "private"},
            {CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, "system"},
            {CHAT_CHANNEL.CHAT_CHANNEL_NATION, "nation"},

            {CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR , "allianceWar"},
        };

        public static readonly Dictionary<string, CHAT_CHANNEL> STR2CHANNEL = new Dictionary<string, CHAT_CHANNEL>
        {
            {"world", CHAT_CHANNEL.CHAT_CHANNEL_WORLD},
            {"team", CHAT_CHANNEL.CHAT_CHANNEL_TEAM},
            {"alliance", CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE},
            {"private", CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE},
            {"system", CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM},
            {"nation", CHAT_CHANNEL.CHAT_CHANNEL_NATION},

            {"allianceWar",CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR },
        };

        public static readonly Dictionary<CHAT_CHANNEL, bool> IS_NEED_REQUEST_HISTORY = new Dictionary<CHAT_CHANNEL, bool>
        {
            {CHAT_CHANNEL.CHAT_CHANNEL_WORLD, true},
            {CHAT_CHANNEL.CHAT_CHANNEL_TEAM, false},
            {CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE, false},
            {CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE, true},
            {CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, false},
            {CHAT_CHANNEL.CHAT_CHANNEL_NATION, false},
            {CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCEWAR, false},
        };

        #region ui rules
        public static readonly Dictionary<CHAT_CHANNEL, string> CHANNEL2ICON = new Dictionary<CHAT_CHANNEL, string>
        {
            {CHAT_CHANNEL.CHAT_CHANNEL_WORLD, "Maininterface_Icons_Chat_Shijie"},
            {CHAT_CHANNEL.CHAT_CHANNEL_TEAM, "Maininterface_Icons_Chat_Duiwu"},
            {CHAT_CHANNEL.CHAT_CHANNEL_ALLIANCE, "Maininterface_Icons_Chat_Bangpai"},
            {CHAT_CHANNEL.CHAT_CHANNEL_PRIVATE, "Maininterface_Icons_Chat_Siliao"},
            {CHAT_CHANNEL.CHAT_CHANNEL_SYSTEM, "Maininterface_Icons_Chat_Xitong"},
            {CHAT_CHANNEL.CHAT_CHANNEL_NATION, "Maininterface_Icons_Chat_Duiwu"},
        };
        
        // public static readonly Dictionary<bool, string> CHANNEL_SPRITE = new Dictionary<bool, string>
        // {
        //     {true, "Ty_Navigationbar_Button_Pressed"},
        //     {false, "Ty_Navigationbar_Button_nor"},
        // };
        #endregion
    }
}