using UnityEngine;

namespace Hotfix_LT.UI
{
    public class UIResourceComponent : DynamicMonoHotfix
    {
        public UIWidget m_ClickTarget;
        public Transform m_Anchor;
        public string m_ResID;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_ClickTarget = t.GetComponent<UIWidget>("Bg");
            m_Anchor = t.GetComponent<Transform>("Anchor");

            if (m_ClickTarget.GetComponent<Collider>() == null)
            {
                NGUITools.AddWidgetCollider(m_ClickTarget.gameObject);
            }

            UIEventTrigger trigger = m_ClickTarget.GetComponent<UIEventTrigger>();

            if (trigger == null)
            {
                trigger = m_ClickTarget.gameObject.AddComponent<UIEventTrigger>();
            }

            trigger.onClick.Add(new EventDelegate(OnResourceClick));
        }

        public void OnResourceClick()
        {
            if (m_ResID != "alliance-gold" &&
               m_ResID != "arena-gold" &&
               m_ResID != "expedition-gold" &&
               m_ResID != "train" &&
               m_ResID != "ladder-gold")
            {
                return;
            }

            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            Vector3 pos = m_Anchor.position;
            ePressTipAnchorType anchor = ePressTipAnchorType.LeftDown;
            UITooltipManager.Instance.DisplayResSrcTooltip(m_ResID, "Res", "default", pos, anchor);
        }
    }
}
