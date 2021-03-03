using UnityEngine;

namespace Hotfix_LT.UI
{
    public class MessageDialog : UIControllerHotfix
    {
        public GameObject m_Control;
        public GameObject m_Header;
        public UIButton m_CloseButton;
        public UIButton m_OKButton;
        public UIButton m_CancelButton;
        public UILabel m_Title;
        public UILabel m_Content;
        public UILabel m_OKLabel;
        public UILabel m_CancelLabel;
        public UIGrid m_Grid;
        public UITable m_Table;

        private static MessageDialog m_instance;

        public delegate void OnClose(int result); // 0 = OK, 1 = Cancel, 2 = Close.

        private OnClose onClose = null;

        public override bool ShowUIBlocker { get { return false; } }

        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_Control = t.FindEx("Control").gameObject;
            m_Header = t.FindEx("Control/Title/Label").gameObject;
            m_CloseButton = t.GetComponent<UIButton>("Control/CloseButton");
            m_OKButton = t.GetComponent<UIButton>("Control/Table/3_ButtonGrid/2_OKButton");
            m_CancelButton = t.GetComponent<UIButton>("Control/Table/3_ButtonGrid/2_CancelButton");
            m_Title = t.GetComponent<UILabel>("Control/Title/Label");
            m_Content = t.GetComponent<UILabel>("Control/Table/2_Content");
            m_OKLabel = t.GetComponent<UILabel>("Control/Table/3_ButtonGrid/2_OKButton/Label");
            m_CancelLabel = t.GetComponent<UILabel>("Control/Table/3_ButtonGrid/2_CancelButton/Label");
            m_Grid = t.GetComponent<UIGrid>("Control/Table/3_ButtonGrid");
            m_Table = t.GetComponent<UITable>("Control/Table");
            controller.backButton = t.GetComponent<UIButton>("Control/CloseButton");
        }

        public override void OnFocus()
        {
            string focusName = CommonConditionParse.FocusViewName; //为了不干扰焦点
            base.OnFocus();
            CommonConditionParse.FocusViewName = focusName;
            EB.Debug.Log("FocusViewName <color=#800000ff>{0}</color>",CommonConditionParse.FocusViewName);
        }

        public override bool CanAutoBackstack()
        {
            return true;
        }

        public override bool Visibility
        {
            get { return m_Control.activeSelf; }
        }

        public override void Show(bool isShowing)
        {
            m_Control.SetActive(isShowing);
        }

        public override void Start()
        {
            m_instance = this;

            EventDelegate.Add(m_CloseButton.onClick, OnCancelButtonClick);
            EventDelegate.Add(m_OKButton.onClick, OnOKButtonClick);
            EventDelegate.Add(m_CancelButton.onClick, OnDialogCancelButtonClick);

            m_Control.transform.localScale = new Vector3(0.1f, 0.1f, 1.0f);
            Show(false);
        }

        public override void OnCancelButtonClick()
        {
            Hide();
            if (onClose != null)
            {
                onClose(2);

                if (!controller.IsOpen())
                {
                    ClearOnClose();
                }
            }
        }

        void OnOKButtonClick()
        {
            Hide();
            if (onClose != null)
            {
                onClose(0);

                if (!controller.IsOpen())
                {
                    ClearOnClose();
                }
            }
        }

        void OnDialogCancelButtonClick()
        {
            Hide();
            if (onClose != null)
            {
                onClose(1);

                if (!controller.IsOpen())
                {
                    ClearOnClose();
                }
            }
        }

        void Show()
        {
            UIHierarchyHelper.Instance.SetBlockPanel(true);
            controller.Open();
            m_instance.m_Grid.Reposition();
            m_instance.m_Table.repositionNow = true;
            m_Control.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            //LeanTween.scale(m_Control, new Vector3(1.0f, 1.0f, 1.0f), 0.2f).tweenType = LeanTweenType.easeInSine;
            BootFlash();
        }

        void Hide()
        {
            UIHierarchyHelper.Instance.SetBlockPanel(false);
            controller.Close();
        }

        void ClearOnClose()
        {
            onClose = null;
        }

        public static void HideCurrent()
        {
            if (m_instance != null && m_instance.controller.IsOpen())
            {
                m_instance.Hide();
                m_instance.ClearOnClose();
            }
        }

        public static void Show(string title, string content, string ok, string cancel,
                                bool hideHeader = false, bool hideCancelButton = false, bool hideCloseButton = false,
                                OnClose closeCallback = null, NGUIText.Alignment alignment = NGUIText.Alignment.Center,
                                bool UseWiderMode = false, bool Higher = false)
        {
            if (m_instance == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(title))
            {
                m_instance.m_Title.text = m_instance.m_Title.transform.GetChild(0).GetComponent<UILabel>().text = title;
                m_instance.m_Title.gameObject.CustomSetActive(true);
            }
            else m_instance.m_Title.gameObject.CustomSetActive(false);

            m_instance.m_Content.width = UseWiderMode ? 900 : 900;
            m_instance.m_Content.text = m_instance.m_Content.transform.GetChild(0).GetComponent<UILabel>().text = content;
            m_instance.m_Content.alignment = m_instance.m_Content.transform.GetChild(0).GetComponent<UILabel>().alignment = alignment;
            m_instance.m_OKLabel.text = m_instance.m_OKLabel.transform.GetChild(0).GetComponent<UILabel>().text = ok;
            m_instance.m_CancelLabel.text = m_instance.m_CancelLabel.transform.GetChild(0).GetComponent<UILabel>().text = cancel;
            m_instance.onClose = closeCallback;
            //m_instance.m_Header.gameObject.CustomSetActive(!hideHeader);
            m_instance.m_CancelButton.gameObject.CustomSetActive(!hideCancelButton);
            m_instance.m_CloseButton.gameObject.CustomSetActive(!hideCloseButton);
            m_instance.Show();
        }

        private void BootFlash()
        {
            UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }
    }
}
