namespace Hotfix_LT.UI
{
using UnityEngine;
using System.Collections;

public enum eFriendAddWay {
	Normal,Commend,FromBlack
}

public class FriendApplyController : UIControllerHotfix {

	public override bool ShowUIBlocker { get { return true; } }

	public UILabel VerifyLabel;
	private eFriendAddWay mAddWay;
	private long mUid;
	private System.Action mCallback;
	UIButton backButton;
	UIButton HotfixBtn1;
	UIButton HotfixBtn2;
	public override void Awake()
	{
		base.Awake();

		VerifyLabel =  controller.transform.Find("Frame/Content/Input/InputText").GetComponent<UILabel>();
		backButton =  controller.transform.Find("Frame/CloseBtn").GetComponent<UIButton>();
		backButton.onClick.Add(new EventDelegate(base.OnCancelButtonClick));
		HotfixBtn1 =  controller.transform.Find("Frame/Content/SureBtn").GetComponent<UIButton>();
		HotfixBtn1.onClick.Add(new EventDelegate(OnSureBtnClick));
		HotfixBtn2 =  controller.transform.Find("Frame/Content/CancelBtn").GetComponent<UIButton>();
		HotfixBtn2.onClick.Add(new EventDelegate(OnCancelButtonClick));
	}

    public override IEnumerator OnRemoveFromStack()
    {
        DestroySelf();
        return base.OnRemoveFromStack();
    }

    public override void SetMenuData(object param)
	{
		base.SetMenuData(param);

		Hashtable ht = param as Hashtable;
		mAddWay = (eFriendAddWay)ht["addWay"];
		mUid = (long)ht["uid"];
		mCallback = (System.Action)ht["callback"];
		VerifyLabel.text = EB.Localizer.GetString("ID_codefont_in_FriendApplyController_724") + LTGameSettingController.GetPlayerName();
	}

	public void OnSureBtnClick()
	{
	    FusionAudio.PostEvent("UI/General/ButtonClick");
		if (!EB.ProfanityFilter.Test(VerifyLabel.text))
		{
			MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"),
					EB.Localizer.GetString("ID_NAME_ILLEGEL"),
					EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, null, NGUIText.Alignment.Center);
			return;
		}

		var rejecter = FriendManager.Instance.Info.RejectTargets.Find(m => m.Uid == mUid);
		if (rejecter != null)
		{
			float ts = EB.Time.Now - rejecter.Ts;
			if (ts <= FriendManager.Instance.Config.RejectTimeInterval * 60)
			{
				EB.Debug.Log("have reject timespan={0}" , ts);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_FRIEND_APPLY_REJECT_TIP"));
                    controller.Close();
				return;
			}
		}

		switch (mAddWay)
		{
			case eFriendAddWay.Normal:
				FriendManager.Instance.Add(mUid, VerifyLabel.text, delegate (bool successful) {
					MessageTemplateManager.ShowMessage(FriendManager.CodeSendFriendInvite);
				});
				break;
			case eFriendAddWay.Commend:
				FriendManager.Instance.Recommends.Remove(mUid);
				GameDataSparxManager.Instance.SetDirty(FriendManager.RecommendListId);

				FriendManager.Instance.Add(mUid, VerifyLabel.text, delegate (bool successful) {
					MessageTemplateManager.ShowMessage(FriendManager.CodeSendFriendInvite);
				});
				break;
			case eFriendAddWay.FromBlack:				
				FriendManager.Instance.AddFromBlacklist(mUid, VerifyLabel.text, delegate (bool successful) {
					//FriendManager.Instance.Info.BlacklistNum--;
					FriendManager.Instance.BlackLists.Remove(mUid);
					MessageTemplateManager.ShowMessage(FriendManager.CodeSendFriendInvite);
					MessageTemplateManager.ShowMessage(902254);
					if (mCallback != null)
						mCallback();
				});
				break;
		}
		controller.Close();
	}
}

}