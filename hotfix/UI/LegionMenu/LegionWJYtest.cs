using UnityEngine;

namespace Hotfix_LT.UI {
    public class LegionWJYtest : DynamicMonoHotfix
    {
        public UIButton IconButton;
        public GameObject IconEditView;
        public GameObject NameEditView;

        public override void Awake()
        {
            IconEditView = mDMono.transform.parent.parent.Find("LTLegionIconEditView").gameObject;
            NameEditView = mDMono.transform.parent.parent.Find("LTLegionNameEditView").gameObject;
            if (NameEditView != null)
                NameEditView.SetActive(false);
            if (IconEditView != null)
                IconEditView.SetActive(false);
            if (IconButton != null)
                EventDelegate.Add(this.IconButton.onClick, this.BtnClicked);
        }

        private void BtnClicked()
        {
            NameEditView.SetActive(true);
        }
    }
}