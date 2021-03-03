using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum eUIDialogueButtons
{
    None = 1 << 0,
    Accept = 1 << 1,
    Decline = 1 << 2,
    Cancel = 1 << 3,
}

public class LTDownloadDialogueController : MonoBehaviour
{
    public static string TitleConfirm { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_CONFIRM"); } }
    public static string TitleTips { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_TIPS"); } }
    public static string TitleWarning { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_WARNING"); } }
    public static string TitleError { get { return EB.Localizer.GetString("ID_DIALOG_TITLE_ERROR"); } }

    public static string ButtonOk { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_OK"); } }
    public static string ButtonGo { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_GO"); } }
    public static string ButtonCancel { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_CANCEL"); } }
    public static string ButtonAccept { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_ACCEPT"); } }
    public static string ButtonReject { get { return EB.Localizer.GetString("ID_DIALOG_BUTTON_DECLINE"); } }

    private UIGrid ButtonGrid;
    private UILabel TitleLabel;
    private UILabel BodyLabel;
    private GameObject AcceptButton;
    private UILabel AcceptLabel;
    private GameObject CancelButton;
    private UILabel CancelLabel;
    private GameObject DeclineButton;
    private UILabel DeclineLabel;
    private TweenScale TS;
    private System.Action<eUIDialogueButtons> CallBack;
    private bool isInit = false;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (!isInit)
        {
            isInit = true;
            ButtonGrid = transform.Find("LayoutTable/ButtonGrid").GetComponent<UIGrid>();
            TitleLabel = transform.Find("LayoutTable/TitleLabel").GetComponent<UILabel>();
            BodyLabel = transform.Find("LayoutTable/Body/Content").GetComponent<UILabel>();

            AcceptButton = ButtonGrid.transform.Find("AcceptButton").gameObject;
            UIButton btn = AcceptButton.transform.GetChild(0).GetComponent<UIButton>();
            btn.onClick.Add(new EventDelegate(OnAcceptButtonClick));
            AcceptLabel = btn.transform.GetChild(0).GetComponent<UILabel>();

            CancelButton = ButtonGrid.transform.Find("CancelButton").gameObject;
            btn = CancelButton.transform.GetChild(0).GetComponent<UIButton>();
            btn.onClick.Add(new EventDelegate(OnCancelButtonClick));
            CancelLabel = btn.transform.GetChild(0).GetComponent<UILabel>();

            DeclineButton = ButtonGrid.transform.Find("DeclineButton").gameObject;
            btn = DeclineButton.transform.GetChild(0).GetComponent<UIButton>();
            btn.onClick.Add(new EventDelegate(OnDeclineButtonClick));
            DeclineLabel = btn.transform.GetChild(0).GetComponent<UILabel>();

            TS = GetComponent<TweenScale>();
        }
    }

    public void Show_Confirm(string content, System .Action<eUIDialogueButtons> callback)
    {
        Init();
        TitleLabel.text = TitleTips;
        AcceptButton.gameObject.CustomSetActive(true);
        AcceptLabel.text = ButtonOk;
        CancelButton.gameObject.CustomSetActive(true);
        CancelLabel.text = ButtonCancel;
        DeclineButton.gameObject.CustomSetActive(false);
        ButtonGrid.repositionNow = true;
        Show(content,callback);
    }
    public void Show_Tip(string content, System.Action<eUIDialogueButtons> callback)
    {
        Init();
        TitleLabel.text = TitleTips;
        AcceptButton.gameObject.CustomSetActive(true);
        AcceptLabel.text = ButtonOk;
        CancelButton.gameObject.CustomSetActive(false);
        DeclineButton.gameObject.CustomSetActive(false);
        ButtonGrid.repositionNow = true;
        Show(content, callback);
    }

    private void Show(string content, System.Action<eUIDialogueButtons> callback)
    {
        BodyLabel.text = content;
        CallBack = callback;
        TS.ResetToBeginning();
        TS .PlayForward();
        gameObject.CustomSetActive(true);
    }
    private void Hide()
    {
        CallBack = null;
        gameObject.CustomSetActive(false);
    }
    private void OnCancelButtonClick()
    {
        if (CallBack != null)CallBack(eUIDialogueButtons.Cancel);
        Hide();
    }
    private void OnAcceptButtonClick()
    {
        if (CallBack != null) CallBack(eUIDialogueButtons.Accept );
        Hide();
    }
    private void OnDeclineButtonClick()
    {
        if (CallBack != null) CallBack(eUIDialogueButtons.Decline );
        Hide();
    }

}
