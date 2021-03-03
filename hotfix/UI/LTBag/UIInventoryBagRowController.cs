
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class UIInventoryBagRowController : DynamicRowController<UIInventoryBagCellData, UIInventoryBagCellController>
    {
        protected UIInventoryGridScroll m_GridScroll;
        protected UIWidget m_Widget;
        protected float m_Alpha;
        public float m_FadeAlpha;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;

            if (cellCtrls == null)
            {
                cellCtrls = new UIInventoryBagCellController[t.childCount];

                for (var i = 0; i < t.childCount; i++)
                {
                    cellCtrls[i] = t.GetChild(i).GetMonoILRComponentByClassPath<UIInventoryBagCellController>("Hotfix_LT.UI.UIInventoryBagCellController");
                }
            }

            m_FadeAlpha = 0.8f;
            m_Alpha = 1f;
            m_GridScroll = t.parent.GetMonoILRComponentByClassPath<UIInventoryGridScroll>("Hotfix_LT.UI.UIInventoryGridScroll");
            m_Widget = t.GetComponent<UIWidget>();
        }
    }
}