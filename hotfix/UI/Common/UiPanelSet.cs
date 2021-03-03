namespace Hotfix_LT.UI
{
    public class UiPanelSet : DynamicMonoHotfix
    {
        public int m_nRenderQ;

        public override void Awake()
        {
            base.Awake();

            if (mDMono.IntParamList != null)
            {
                var count = mDMono.IntParamList.Count;

                if (count > 0)
                {
                    m_nRenderQ = mDMono.IntParamList[0];
                }
            }

            var uiPanel = mDMono.gameObject.GetComponent<UIPanel>();

            if (uiPanel != null)
            {
                uiPanel.startingRenderQueue = m_nRenderQ;
            }
        }
    }
}
