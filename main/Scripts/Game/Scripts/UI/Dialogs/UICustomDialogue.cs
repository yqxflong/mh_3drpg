using UnityEngine;
using System.Collections;

public class UICustomeDialogueOption : UIDialogeOption
{
	public int minHeight;
	public int lineSpace;
	public NGUIText.Alignment alignment = NGUIText.Alignment.Center;
	public string accept;
	public string decline;
	public string cancel;
	public bool hideBodyBackground;

	public const int TwoLineHeight = 120;
	public const int ThreeLineHeight = 180;
	
	public static string ButtonOk { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_OK"); } }
	public static string ButtonGo { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_GO"); } }
	public static string ButtonCancel { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_CANCEL"); } }
	public static string ButtonAccept { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_ACCEPT"); } }
	public static string ButtonReject { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_DECLINE"); } }

	public static UICustomeDialogueOption CustomStyleConfirm
	{
		get
		{
			UICustomeDialogueOption option = new UICustomeDialogueOption();
			option.buttons = eUIDialogueButtons.Accept | eUIDialogueButtons.Cancel;
			option.title = TitleConfirm;
			option.accept = ButtonOk;
			option.cancel = ButtonCancel;
			option.minHeight = ThreeLineHeight;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UICustomeDialogueOption CustomStyleAlert
	{
		get
		{
			UICustomeDialogueOption option = new UICustomeDialogueOption();
			option.buttons = eUIDialogueButtons.Accept;
			option.title = TitleWarning;
			option.accept = ButtonOk;
			option.minHeight = ThreeLineHeight;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UICustomeDialogueOption CustomStyleError
	{
		get
		{
			UICustomeDialogueOption option = new UICustomeDialogueOption();
			option.buttons = eUIDialogueButtons.Accept;
			option.title = TitleError;
			option.accept = ButtonOk;
			option.minHeight = ThreeLineHeight;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UICustomeDialogueOption CustomStylePrompt
	{
		get
		{
			UICustomeDialogueOption option = new UICustomeDialogueOption();
			option.buttons = eUIDialogueButtons.Accept | eUIDialogueButtons.Cancel;
			option.input = "";
			option.title = TitleConfirm;
			option.accept = ButtonOk;
			option.cancel = ButtonCancel;
			option.hideBodyBackground = true;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UICustomeDialogueOption CustomStyleResponse
	{
		get
		{
			UICustomeDialogueOption option = new UICustomeDialogueOption();
			option.buttons = eUIDialogueButtons.Accept | eUIDialogueButtons.Decline;
			option.title = TitleTips;
			option.accept = ButtonAccept;
			option.decline = ButtonReject;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UICustomeDialogueOption CustomStyleTips
	{
		get
		{
			UICustomeDialogueOption option = new UICustomeDialogueOption();
			option.buttons = eUIDialogueButtons.None;
			option.title = TitleTips;
			return option;
		}
	}
}

public class UICustomDialogue : UIDialogue
{
	private UICustomeDialogueOption mCustomOption;
	private NGUIText.Alignment mOriginAllianment;
	private UILabel.Overflow mOriginOverflow;
	private int mOriginHeight;
	private int mOriginSpace;
	private string mOriginAccept;
	private string mOriginDecline;
	private string mOriginCancel;
	private bool mOriginBodyBackgroundEnabled;

	public override void Show(UIDialogeOption option)
	{
		base.Show(option);

		if (Option != option)
		{
			// queued
			return;
		}

		if (option is UICustomeDialogueOption == false)
		{
			return;
		}

		mCustomOption = option as UICustomeDialogueOption;

		// set style
		if (mCustomOption.accept != null && acceptButton != null && acceptObject != null && acceptObject.activeSelf)
		{
			UILabel acceptLabel = acceptButton.GetComponentInChildren<UILabel>();
			if (acceptLabel != null)
			{
				mOriginAccept = acceptLabel.text;
				acceptLabel.text = mCustomOption.accept;
			}
		}

		if (mCustomOption.decline != null && declineButton != null && declineObject != null && declineObject.activeSelf)
		{
			UILabel declineLabel = declineButton.GetComponentInChildren<UILabel>();
			if (declineLabel != null)
			{
				mOriginDecline = declineLabel.text;
				declineLabel.text = mCustomOption.decline;
			}
		}

		if (mCustomOption.cancel != null && cancelButton != null && cancelObject != null && cancelObject.activeSelf)
		{
			UILabel cancelLabel = cancelButton.GetComponentInChildren<UILabel>();
			if (cancelLabel != null)
			{
				mOriginCancel = cancelLabel.text;
				cancelLabel.text = mCustomOption.cancel;
			}
		}

		if (body != null && body.activeSelf)
		{
			UILabel bodyLabel = body.GetComponentInChildren<UILabel>();

			mOriginAllianment = bodyLabel.alignment;
			mOriginOverflow = bodyLabel.overflowMethod;
			mOriginHeight = bodyLabel.height;
			mOriginSpace = bodyLabel.spacingY;

			//bodyLabel.alignment = mCustomOption.alignment;
			if (mCustomOption.lineSpace != 0)
			{
				bodyLabel.spacingY = mCustomOption.lineSpace;
			}
			if (mCustomOption.minHeight != 0)
			{
				bodyLabel.ProcessText();
				if (bodyLabel.height < mCustomOption.minHeight)
				{
					bodyLabel.overflowMethod = UILabel.Overflow.ShrinkContent;
					bodyLabel.height = Mathf.Max(bodyLabel.height, mCustomOption.minHeight);
				}
			}

			UISprite bodyBackground = body.GetComponentInChildren<UISprite>(true);
			if (bodyBackground != null)
			{
				mOriginBodyBackgroundEnabled = bodyBackground.enabled;
				bodyBackground.enabled = !mCustomOption.hideBodyBackground;
			}
		}
	}

    public override IEnumerator OnAddToStack()
    {
        GetComponent<UIPanel>().sortingOrder = 20000;
        GetComponent<UIPanel>().depth = 20000; 
        return base.OnAddToStack();
    }

    public override IEnumerator OnRemoveFromStack()
	{
		if (mCustomOption != null)
		{
			if (mCustomOption.accept != null && acceptButton != null && acceptObject != null && acceptObject.activeSelf)
			{
				UILabel acceptLabel = acceptButton.GetComponentInChildren<UILabel>();
				if (acceptLabel != null)
				{
					acceptLabel.text = mOriginAccept;
				}
			}

			if (mCustomOption.decline != null && declineButton != null && declineObject != null && declineObject.activeSelf)
			{
				UILabel declineLabel = declineButton.GetComponentInChildren<UILabel>();
				if (declineLabel != null)
				{
					declineLabel.text = mOriginDecline;
				}
			}

			if (mCustomOption.cancel != null && cancelButton != null && cancelObject != null && cancelObject.activeSelf)
			{
				UILabel cancelLabel = cancelButton.GetComponentInChildren<UILabel>();
				if (cancelLabel != null)
				{
					cancelLabel.text = mOriginCancel;
				}
			}

			if (body != null && body.activeSelf)
			{
				UILabel bodyLabel = body.GetComponentInChildren<UILabel>();
				bodyLabel.alignment = mOriginAllianment;
				if (mCustomOption.lineSpace != 0)
				{
					bodyLabel.spacingY = mOriginSpace;
				}
				if (mCustomOption.minHeight != 0)
				{
					bodyLabel.overflowMethod = mOriginOverflow;
					bodyLabel.height = mOriginHeight;
                    bodyLabel.transform.localPosition = Vector3.zero;

                }

				UISprite bodyBackground = body.GetComponentInChildren<UISprite>(true);
				if (bodyBackground != null)
				{
					bodyBackground.enabled = mOriginBodyBackgroundEnabled;
				}
			}

			mCustomOption = null;
		}

		yield return base.OnRemoveFromStack();
	}

	public override void Confirm(string body, OnUIDialogueButtonClick onclose)
	{
		UICustomeDialogueOption option = UICustomeDialogueOption.CustomStyleConfirm;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}

	public override void Alert(string body, OnUIDialogueButtonClick onclose)
	{
		UICustomeDialogueOption option = UICustomeDialogueOption.CustomStyleAlert;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}

	public override void Error(string body, OnUIDialogueButtonClick onclose)
	{
		UICustomeDialogueOption option = UICustomeDialogueOption.CustomStyleError;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}
    
	public override void Tips(string body)
	{
		UICustomeDialogueOption option = UICustomeDialogueOption.CustomStyleTips;
		option.body = body;
		Show(option);
	}
}
