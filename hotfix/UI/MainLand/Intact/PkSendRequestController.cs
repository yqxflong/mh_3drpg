using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class PkSendRequestController : UIControllerHotfix, IHotfixUpdate
    {

        public UILabel TargetLabel;
        public UIProgressBar ProgressBar;
        private bool IsStart = false;
        private const float WaitSecond = 10f;
        private float ResidueSecond = 0;
        string Name;
        long Uid;

        UIButton HotfixBtn0;
        UIButton HotfixBtn1;

        public override void Awake()
        {
            base.Awake();
            TargetLabel = controller.transform.Find("Frame/Content/TargetTips").GetComponent<UILabel>();
            ProgressBar = controller.transform.Find("Frame/Content/ProgressBar").GetComponent<UIProgressBar>();
            HotfixBtn0 = controller.transform.Find("Frame/CenterBG/Top/CloseBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(base.OnCancelButtonClick));
            HotfixBtn1 = controller.transform.Find("Frame/Content/CancelBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnMyCancelBtnClick));
            TargetLabel.text = string.Empty;
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable data = param as Hashtable;
            Name = data["name"].ToString();
            Uid = (long)data["uid"];
        }

        public override IEnumerator OnAddToStack()
        {
            IsStart = true;
            ResidueSecond = WaitSecond;

            Hotfix_LT.Messenger.AddListener(EventName.PkRejectEvent, PkRejectListener);
            LTUIUtil.SetText(TargetLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_PkSendRequestController_819"), LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, Name));
            return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.PkRejectEvent, PkRejectListener);

            DestroySelf();
            yield break;
        }

        public void OnMyCancelBtnClick()
        {
            SocialIntactManager.Instance.CancelPVP(Uid, delegate (EB.Sparx.Response res) {
                if (!res.sucessful)
                {
                    res.CheckAndShowModal();
                }
            });
           controller.Close();
        }

        private void PkRejectListener()
        {
            controller.Close();
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            // base.Update();

            if (!IsStart)
                return;

            ResidueSecond -= Time.deltaTime;

            ProgressBar.value = ResidueSecond / WaitSecond;
            //CountdownLabel.text = Mathf.CeilToInt(ResidueSecond) +EB.Localizer.GetString("ID_SECOND");
            if (ResidueSecond <= 0f)
            {
                MessageTemplateManager.ShowMessage(902069);
                controller.Close();
            }
        }

        public override bool CanAutoBackstack()
        {
            return !controller.IsHudUI;
        }
    }

}