namespace Hotfix_LT.UI
{
    public class LTHotfixGeneralFunc
    {
        public static void ShowChargeMess(int mess = 901030)
        {
            MessageTemplateManager.ShowMessage(mess, null, delegate (int r)
            {
                if (r == 0)
                {
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                }
            });
        }
    }
}