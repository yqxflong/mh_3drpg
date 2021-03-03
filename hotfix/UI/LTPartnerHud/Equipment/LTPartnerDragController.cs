using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerDragController : DynamicMonoHotfix
    {
        private const float MIN_DRAG_VALUE = 50f;
        private UIEventListener uiEventListener;
        private float m_DeltaX;
        [EB.Serializable]
        public EventDelegate action;

        public override void Awake()
        {
            base.Awake();

            var changeController = GetComponnetInParent(mDMono.transform);
            action = new EventDelegate(changeController.DragEvent);
            Init(action);
        }

        private LTPartnerChangeController GetComponnetInParent(Transform t)
        {
            if (t == null)
            {
                EB.Debug.LogError("LTPartnerChangeController -> changeController is null");
                return null;
            }

            var ilr = t.GetComponent<DynamicMonoILR>();

            if (ilr != null && ilr.hotfixClassPath == "Hotfix_LT.UI.LTPartnerChangeController")
            {
                return ilr._ilrObject as LTPartnerChangeController;
            }
            else
            {
                return GetComponnetInParent(t.parent);
            }
        }

        public void Init(EventDelegate action)
        {
            if (uiEventListener == null)
            {
                uiEventListener = mDMono.gameObject.AddComponent<UIEventListener>();
            }

            uiEventListener.onDragStart = MyOnDragStart;
            uiEventListener.onDrag = MyOnDrag;
            uiEventListener.onDragEnd = MyOnDragEnd;
            this.action = action;
        }

        private void MyOnDrag(GameObject go, Vector2 delta)
        {
            m_DeltaX += delta.x;
            // EB.Debug.LogError("DrAG");
        }

        private void MyOnDragEnd(GameObject go)
        {
            if (Mathf.Abs(m_DeltaX) > MIN_DRAG_VALUE)
            {
                DragEquOrSkill();
            }
        }

        private void DragEquOrSkill()
        {
            // if (ChangeController!=null)
            // {
            //     if (ChangeController.hud.IsSelectSkill)
            //     {
            //         ChangeController.OnClickEquipTitle();
            //     }
            //     else
            //     {
            //         ChangeController.OnClickSkillTile();
            //     }
            // }

            if (action != null)
            {
                // action();
                action.Execute();
            }
        }

        private void MyOnDragStart(GameObject go)
        {
            m_DeltaX = 0;
        }

    }
}
