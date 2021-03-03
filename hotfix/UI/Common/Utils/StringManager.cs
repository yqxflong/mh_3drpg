using System.Text.RegularExpressions;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class SkillDescTransverter
    {
        //将字符串中<>及其内的内容转换为<>内的内容所对应的数值
        private static int SkillLevel;
        private static int SkillID;
        public static string ChangeDescription(string description, int skillLevel, int skillID)
        {
            SkillLevel = skillLevel;
            SkillID = skillID;
            string str = description;
            Regex DP_Regex = new Regex(@"\$([0-9]+)%\$");
            str = DP_Regex.Replace(str, ReturnDPString);
            Regex R_Regex = new Regex(@"#([0-9]+)%#");
            str = R_Regex.Replace(str, ReturnRString);
            return string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, "000023", str);
        }

        private static string ReturnDPString(Match match)
        {
            float temp = Hotfix_LT.Data.SkillTemplateManager.Instance.GetSkillLevelUpTemplate(SkillID).DamageIncPercent;
            int param = int.Parse(match.Groups[1].Value);
            int i = 0;
            if (SkillLevel > 1)
            {
                i = Mathf.FloorToInt(param * (1 + temp * (SkillLevel - 1)));
            }
            else
            {
                i = param;
            }
            return string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, "008527", i + "%");
        }

        private static string ReturnRString(Match match)
        {
            float temp = Hotfix_LT.Data.SkillTemplateManager.Instance.GetSkillLevelUpTemplate(SkillID).Rating;
            int param = int.Parse(match.Groups[1].Value);
            int i = 0;
            if (SkillLevel > 1)
            {
                i = Mathf.FloorToInt(param + (temp * (SkillLevel - 1) * 100f));
            }
            else
            {
                i = param;
            }
            return string.Format(LT.Hotfix.Utility.ColorUtility.ColorStringFormat, "008527", i + "%");
        }
    }
}