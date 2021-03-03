namespace Hotfix_LT.UI
{
    public class LTDrawCardConfig
    {
        public const string LOTTERY_GOLD_ID = "1061";
        public const string LOTTERY_HC_ID = "1062";

        //数字
        public static int Once_GoldCost
        {
            get
            {
                return (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("goldLotteryOneTime");
            }
        }

        public static int More_GoldCost
        {
            get
            {
                return (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("goldLotteryTenTimes");
            }
        }

        public static int FreeTimes = 0;
        public static int FreeTenTimes = 1;
        public static int Once_HCCost
        {
            get
            {
                return (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("hcLotteryOneTime");
            }
        }

        public static int More_HCCost
        {
            get
            {
                return (int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("hcLotteryTenTimes");
            }
        }

        //字符串
        public static string RedStrColor = "[ff6699]";
        public static string GreedStrColor = "[42fe79]";
        public static string NextResetStr = "ID_codefont_in_LTDrawCardConfig_1019";
        public static string NextFreeTimeStr = "ID_codefont_in_LTDrawCardConfig_1139";

        public static string GOLD_FreeTimeStr { get { return EB.Localizer.GetString("ID_codefont_in_LTDrawCardConfig_1262"); } }
        public static string GOLD_FreeLabelStr { get { return EB.Localizer.GetString("ID_codefont_in_LTDrawCardConfig_1328"); } }
        public static string HC_FreeTimeStr { get { return EB.Localizer.GetString("ID_codefont_in_LTDrawCardConfig_1390"); } }
        public static string HC_FreeLabelStr { get { return EB.Localizer.GetString("ID_codefont_in_LTDrawCardConfig_1454"); } }

        public static string GOLDICON = "Ty_Icon_Gold";
        public static string HCICON = "Ty_Icon_Jewel";
        public static string GOLDDRAWICON = "Ty_Icon_Yinchoujiang";
        public static string HCDRAWICON = "Ty_Icon_Jinchoujiang";
    }
}