using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UITooltipPressPos : DynamicMonoHotfix
    {
        public UIWidget m_Container;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Container = t.GetComponent<UIWidget>("Container");
        }

        //获取这个物体的边界 根据pos 和anchor 设置
        public void SetPosAndAnchor(Vector3 pos, ePressTipAnchorType anchor)
        {
            if (anchor == ePressTipAnchorType.RightUp)
            {
                m_Container.pivot = UIWidget.Pivot.TopRight;
            }
            else if (anchor == ePressTipAnchorType.RightDown)
            {
                m_Container.pivot = UIWidget.Pivot.BottomRight;
            }
            else if (anchor == ePressTipAnchorType.LeftUp)
            {
                m_Container.pivot = UIWidget.Pivot.TopLeft;
            }
            else
            {
                m_Container.pivot = UIWidget.Pivot.BottomLeft;
            }

            m_Container.gameObject.transform.position = pos;
        }
    }
}
