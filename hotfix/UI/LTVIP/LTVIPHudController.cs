using UnityEngine;
using System.Collections;

namespace Hotfix_LT.UI
{
    public class LTVIPHudController : UIControllerHotfix
    {
        private UILabel IDLabel;
        private UIButton CopyBtn;

        public override bool ShowUIBlocker { get { return true; } }
        public override bool IsFullscreen() { return false; }

        private string ID;
        public override void Awake()
        {
            base.Awake();
            IDLabel = controller.transform.Find("IDLabel").GetComponent <UILabel >();
            CopyBtn = controller.transform.Find("CopyBtn").GetComponent<UIButton>();
            CopyBtn.onClick.Add(new EventDelegate(OnCopyBtnClick));
            ID = Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("VIPCustomerService");
            IDLabel.text = ID;
        }
        
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        private void OnCopyBtnClick()
        {
            GUIUtility.systemCopyBuffer = ID;
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText ,"复制成功！");
        }
    }
}