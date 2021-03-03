using UnityEngine;
using EB.Sparx;


namespace Hotfix_LT.UI
{
    /// <summary>
    /// 运营活动相应的按钮
    /// </summary>
    public class LTActivityTitleItem : DynamicMonoHotfix
    {
        public UITexture BG;
        public UILabel Title;
        public object ActivityData;
        public GameObject Pressed;
        public GameObject RedPoint;
        public GameObject OpenNotice;
        public UIEventTrigger Trigger;
        public LTActivityUIController parent;
        /// <summary>
        /// 当前活动id
        /// </summary>
        public int v_ActivityId;
        public string v_NavString;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            BG =t.GetComponent<UITexture>("Btn/BG");
            Title = t.GetComponent<UILabel>("Btn/BG/Label");
            Pressed = t.Find("Btn/Pressed").gameObject;
            RedPoint = t.Find("Btn/RedPoint").gameObject;
            OpenNotice = t.Find("Btn/OpenNotice").gameObject;
            Trigger = t.GetComponent<UIEventTrigger>("Btn");
            Trigger.onClick.Add(new EventDelegate(OnClick));
        }

        void OnClick()
        {
            parent.SelectTitle(this);
        }

        public void UpdateRedPoint()
        {
            if (LTMainHudManager.Instance != null)
            {
                if (LTMainHudManager.Instance.CheckEventRedPoint(ActivityData))
                {
                    RedPoint.CustomSetActive(true);
                }
                else
                {
                    RedPoint.CustomSetActive(false);
                }
            }
        }
    }
}