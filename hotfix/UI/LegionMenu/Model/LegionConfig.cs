namespace Hotfix_LT.UI
{
    public class LegionConfig
    {
        #region 错误码
        public static int CodeHcUnenough = 901030; // 您的钻石不足，是否充值？
        public static int CodeGoldUnenough = 901031; // 您的金币不足，是否购买？
        #endregion

        #region 军团本地化
        public static string GetLegionText(string codeID)
        {
            return EB.Localizer.GetString(codeID);
        }
        #endregion
    }
}