namespace Hotfix_LT.UI
{
    public class SkillSetTool
    {
        /// <summary>
        /// 觉醒技能框替换
        /// </summary>
        /// <param name="frame"></param>
        /// <param name=""></param>
        public static void SkillFrameStateSet(UISprite frame, bool isAwaken)
        {
            if (isAwaken)
            {
                frame.spriteName = LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC[1];
            }
            else
            {
                frame.spriteName = LTPartnerConfig.PARTNER_AWAKN_SKILLFRAME_DIC[0];
            }
        }
    }
}
