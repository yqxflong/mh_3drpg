using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 用于判定阵型的最大人数
    /// </summary>
    public class FormationConfig
    {
        public static int NormalMaxIndex = 3;

        public static List<int> OpenTeamFuncID = new List<int>
        {
            10046,
            10080,
            10081
        };

        /// <summary>
        /// 根据战斗类型获得可上阵的最大索引，从0开始
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetTeamMaxIndex(eBattleType type)
        {
            if (type == eBattleType.LegionMercenary)
            {
                return 0;
            }
            
            if (type == eBattleType.HeroBattle)//英雄交锋只能4个
            {
                return 3;
            }

            int maxIndex = 2;
            int level = BalanceResourceUtil.GetUserLevel();
            for (int i = 0; i < OpenTeamFuncID.Count; ++i)
            {
                var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(OpenTeamFuncID[i]);
                if (func != null && func.IsConditionOK())
                {
                    maxIndex++;
                }
            }
            return maxIndex;
        }

        public static bool IsIndexVaild(eBattleType type, int index, out int maxIndex, out string message)
        {
            maxIndex = 0;
            message = "";
            maxIndex = GetTeamMaxIndex(type);

            if (index > maxIndex)
            {
                if (type == eBattleType.HeroBattle ||type == eBattleType.LegionMercenary)
                {

                }
                else if (index == 4)
                {
                    message =  Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10080).GetConditionStrShort();
                }
                else if (index == 5)
                {
                    message = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10081).GetConditionStrShort();
                }
                else
                {
                    var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10046);
                    if (!func.IsConditionOK())
                    {
                        message = func.GetConditionStrShort();
                    }
                }
            }

            return index > maxIndex;
        }
    }
}