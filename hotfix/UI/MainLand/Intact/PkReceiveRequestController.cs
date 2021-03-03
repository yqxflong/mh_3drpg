using UnityEngine;
using System.Collections;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class PkReceiveRequestController : UIControllerHotfix, IHotfixUpdate
    {
        //static private PkReceiveRequestController _Instance;
        //static public PkReceiveRequestController Instance { get { return _Instance; } }

        public UILabel TargetLabel;
        public UIProgressBar ProgressBar;
        //public GameObject Thumb;
        private bool IsStart = false;
        //private bool IsAutoClose = false;
        private long WaitSecond;
        private float ResidueSecond = 0;
        private System.Action<int> CallBack;
        private int AgreeCode = 0, RejectCode = 1;
        private string FromName;
        UIButton HotfixBtn0;
        UIButton HotfixBtn1;
        UIButton HotfixBtn2;
        public override void Awake()
        {
            base.Awake();
            TargetLabel = controller.transform.Find("Frame/Content/TargetTips").GetComponent<UILabel>();
            ProgressBar = controller.transform.Find("Frame/Content/ProgressBar").GetComponent<UIProgressBar>();
            HotfixBtn0 = controller.transform.Find("Frame/CenterBG/Top/CloseBtn").GetComponent<UIButton>();
            HotfixBtn0.onClick.Add(new EventDelegate(OnRejectClick));
            HotfixBtn1 = controller.transform.Find("Frame/Content/RejectBtn").GetComponent<UIButton>();
            HotfixBtn1.onClick.Add(new EventDelegate(OnRejectClick));
            HotfixBtn2 = controller.transform.Find("Frame/Content/AgreelBtn").GetComponent<UIButton>();
            HotfixBtn2.onClick.Add(new EventDelegate(OnAgreeClick));
            TargetLabel.text = string.Empty;
        }

        //void OnDisable()
        //{
        //	if (!IsAutoClose)
        //	{
        //		OnRejectClick();
        //	}
        //}

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            SetData(param);
            ShowUI();
        }

        public override IEnumerator OnAddToStack()
        {
            Hotfix_LT.Messenger.AddListener(EventName.PKCancelEvent, OnCancelPKListener);
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            Hotfix_LT.Messenger.RemoveListener(EventName.PKCancelEvent, OnCancelPKListener);

            DestroySelf();
            yield break;
        }

        private void SetData(object param)
        {
            Hashtable p = param as Hashtable;
            FromName = p["fromName"] as string;
            CallBack = p["callBack"] as System.Action<int>;
            WaitSecond = (long)p["expireTs"];

            IsStart = true;
            //IsAutoClose = false;
            ResidueSecond = Mathf.FloorToInt(WaitSecond);
        }

        private void ShowUI()
        {
            controller.gameObject.SetActive(true);

            LTUIUtil.SetText(TargetLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_PkReceiveRequestController_1626"), LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal, FromName));
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            if (!IsStart)
                return;

            ResidueSecond -= Time.deltaTime;

            ProgressBar.value = ResidueSecond / WaitSecond;
            //Thumb.SetActive(false);
            //Thumb.SetActive(true);
            //CountdownLabel.text = Mathf.CeilToInt(ResidueSecond) + EB.Localizer.GetString("ID_SECOND");

            if (ResidueSecond <= 0f)
            {
                //MessageTemplateManager.ShowMessage(902069);
                controller.Close();
            }
        }

        void OnCancelPKListener()
        {
            controller.Close();
        }

        public void OnRejectClick()
        {
            if (CallBack != null)
            {
                CallBack(RejectCode);
                CallBack = null;
            }
            controller.Close();
        }

        public void OnAgreeClick()
        {
            if (CallBack != null)
            {
                CallBack(AgreeCode);
                CallBack = null;
            }
            controller.Close();
        }

        //private void Close()
        //{
        //	IsStart = false;
        //	IsAutoClose = true;
        //	gameObject.SetActive(false);
        //}
    }

}