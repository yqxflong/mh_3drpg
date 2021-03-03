using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void OnUIDialogueButtonClick(eUIDialogueButtons button, UIDialogeOption option);

public class UIDialogeOption
{
	public eUIDialogueButtons buttons;
	public string title;
	public string body;
	public string input;
	public OnUIDialogueButtonClick onClose;
	public OnUIDialogueButtonClick onClick;
	public object param;

	public static string TitleConfirm { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_CONFIRM"); } }
	public static string TitleTips { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_TIPS"); } }
	public static string TitleWarning { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_WARNING"); } }
	public static string TitleError { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_ERROR"); } }

	protected static void DefaultOnClose(eUIDialogueButtons button, UIDialogeOption option) { }

	public static UIDialogeOption StyleConfirm
	{
		get
		{
			UIDialogeOption option = new UIDialogeOption();
			option.buttons = eUIDialogueButtons.Accept | eUIDialogueButtons.Cancel;
			option.title = TitleConfirm;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UIDialogeOption StyleAlert
	{
		get
		{
			UIDialogeOption option = new UIDialogeOption();
			option.buttons = eUIDialogueButtons.Accept;
			option.title = TitleWarning;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UIDialogeOption StyleError
	{
		get
		{
			UIDialogeOption option = new UIDialogeOption();
			option.buttons = eUIDialogueButtons.Accept;
			option.title = TitleError;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UIDialogeOption StylePrompt
	{
		get
		{
			UIDialogeOption option = new UIDialogeOption();
			option.buttons = eUIDialogueButtons.Accept | eUIDialogueButtons.Cancel;
			option.input = "";
			option.title = TitleConfirm;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UIDialogeOption StyleResponse
	{
		get
		{
			UIDialogeOption option = new UIDialogeOption();
			option.buttons = eUIDialogueButtons.Accept | eUIDialogueButtons.Decline;
			option.title = TitleTips;
			option.onClose = DefaultOnClose;
			return option;
		}
	}

	public static UIDialogeOption StyleTips
	{
		get
		{
			UIDialogeOption option = new UIDialogeOption();
			option.buttons = eUIDialogueButtons.None;
			option.title = TitleTips;
			return option;
		}
	}
}

public class UIDialogue : UIController
{
	public UITable layoutTable;
	public UIGrid buttonGrid;
	public UIButton acceptButton;
	public GameObject acceptObject;
	public UIButton declineButton;
	public GameObject declineObject;
	public UIButton cancelButton;
	public GameObject cancelObject;
	public GameObject header;
	public GameObject body;
	public GameObject input;

	public UIDialogeOption Option { get; private set; }

	private EB.Collections.Queue<UIDialogeOption> mQueue = new EB.Collections.Queue<UIDialogeOption>();
	private bool mClearQueue = true;

	public override bool ShowUIBlocker { get { return true; } }
	public override bool CanAutoBackstack() { return false; }

	protected void DefaultOnClose(eUIDialogueButtons button, UIDialogeOption option) { }

	public virtual void Confirm(string body, OnUIDialogueButtonClick onclose)
	{
		UIDialogeOption option = UIDialogeOption.StyleConfirm;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}

	public virtual void Alert(string body, OnUIDialogueButtonClick onclose)
	{
		UIDialogeOption option = UIDialogeOption.StyleAlert;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}

	public virtual void Error(string body, OnUIDialogueButtonClick onclose)
	{
		UIDialogeOption option = UIDialogeOption.StyleError;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}

	public virtual void Prompt(string body, OnUIDialogueButtonClick onclose)
	{
		UIDialogeOption option = UIDialogeOption.StylePrompt;
		option.body = body;
		option.onClose = onclose;
		Show(option);
	}

	public virtual void Response(string body, OnUIDialogueButtonClick onclose)
	{
		UIDialogeOption option = UIDialogeOption.StyleResponse;
		option.body = body;
		option.onClose = onclose ?? option.onClose;
		Show(option);
	}

	public virtual void Tips(string body)
	{
		UIDialogeOption option = UIDialogeOption.StyleTips;
		option.body = body;
		Show(option);
	}

	public virtual void Show(UIDialogeOption option)
	{
		if (IsOpen())
		{
			mQueue.Enqueue(option);
			return;
		}

		if (mQueue.Count > 0)
		{
			mQueue.Enqueue(option);
			return;
		}

		mClearQueue = true;
		Option = option;
		Open();

		if (header != null)
		{
			header.SetActive(option.title != null);
			if (header.activeSelf)
			{
				UILabel titleLabel = header.GetComponentInChildren<UILabel>();
				titleLabel.text = option.title;
			}
		}

		if (body != null)
		{
			body.SetActive(option.body != null);
			if (body.activeSelf)
			{
				UILabel bodyLabel = body.GetComponentInChildren<UILabel>();
                string [] strs= option.body.Split('\n');
                if (strs.Length > 5) bodyLabel.text = strs[0];
                else bodyLabel.text = option.body;
			}
		}

		if (input != null)
		{
			input.SetActive(option.input != null);
			if (input.activeSelf)
			{
				UIInput inputComponent = input.GetComponentInChildren<UIInput>();
				inputComponent.value = option.input;
			}
		}

		if (acceptButton != null)
		{
			acceptObject.SetActive((option.buttons & eUIDialogueButtons.Accept) == eUIDialogueButtons.Accept);
			acceptButton.onClick.Clear();
			if (acceptObject.activeSelf)
			{
				acceptButton.onClick.Add(new EventDelegate(OnAcceptButtonClick));
			}
		}

		if (declineButton != null)
		{
			declineObject.SetActive((option.buttons & eUIDialogueButtons.Decline) == eUIDialogueButtons.Decline);
			declineButton.onClick.Clear();
			if (declineObject.activeSelf)
			{
				declineButton.onClick.Add(new EventDelegate(OnDeclineButtonClick));
			}
		}

		if (cancelButton != null)
		{
			cancelObject.SetActive((option.buttons & eUIDialogueButtons.Cancel) == eUIDialogueButtons.Cancel);
			cancelButton.onClick.Clear();
			if (cancelObject.activeSelf)
			{
				cancelButton.onClick.Add(new EventDelegate(OnCancelButtonClick));
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();

		//buttonGrid.onReposition += delegate ()
		//{
		//	layoutTable.Reposition();
		//};
	}

	public override IEnumerator OnAddToStack()
	{
		if (ShowUIBlocker)
		{
			mBlocker = UIStack.Instance.GetPanelBlocker(this);
			mCollider = mBlocker.GetComponentInChildren<BoxCollider>();
			mTrigger = mCollider.GetComponent<ConsecutiveClickCoolTrigger>() ?? mCollider.gameObject.AddComponent<ConsecutiveClickCoolTrigger>();
			mTrigger.clickEvent.Clear();
		}

		buttonGrid.repositionNow = true;
        //layoutTable.repositionNow = true;
        yield return null;
        Show(true);
        yield break;
	}

	public override IEnumerator OnRemoveFromStack()
	{
		if (ShowUIBlocker)
		{
			mTrigger = null;
			mCollider = null;
			mBlocker = null;
		}

		Show(false);

		Option = null;
		if (mClearQueue)
		{
			mQueue.Clear();
		}

		yield return new WaitForEndOfFrame();
		
		if (mQueue.Count > 0)
		{
			Show(mQueue.Dequeue());
		}
		else if (!IsOpen())
		{
			DestroySelf();
		}
	}

	protected virtual void OnAcceptButtonClick()
	{
	    FusionAudio.PostEvent("UI/General/ButtonClick");

		if (acceptButton == null)
		{
			return;
		}

		if (!acceptButton.isEnabled)
		{
			return;
		}

		if (input != null)
		{
			Option.input = input.GetComponentInChildren<UIInput>().value;
		}

		if (Option.onClick != null)
		{
			Option.onClick(eUIDialogueButtons.Accept, Option);
		}

		if (Option.onClose != null)
		{
			EB.Coroutines.Run(Close(this, eUIDialogueButtons.Accept, Option));
		}
	}

	protected virtual void OnDeclineButtonClick()
	{
	    FusionAudio.PostEvent("UI/General/ButtonClick");

		if (declineButton == null)
		{
			return;
		}

		if (!declineButton.isEnabled)
		{
			return;
		}

		if (Option.onClick != null)
		{
			Option.onClick(eUIDialogueButtons.Decline, Option);
		}

		if (Option.onClose != null)
		{
			EB.Coroutines.Run(Close(this, eUIDialogueButtons.Decline, Option));
		}
	}

	public override void OnCancelButtonClick()
	{
	    FusionAudio.PostEvent("UI/General/ButtonClick");

		if (cancelButton == null)
		{
			return;
		}

		if (!cancelButton.isEnabled)
		{
			return;
		}

		if (Option.onClick != null)
		{
			Option.onClick(eUIDialogueButtons.Cancel, Option);
		}

		if (Option.onClose != null)
		{
			EB.Coroutines.Run(Close(this, eUIDialogueButtons.Cancel, Option));
		}
	}

	protected static IEnumerator Close(UIDialogue dialogue, eUIDialogueButtons button, UIDialogeOption option)
	{
		dialogue.mClearQueue = false;
		dialogue.Close();

		yield return null;

		option.onClose(button, option);
	}
}
