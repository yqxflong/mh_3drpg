using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class ManHuangLoginController : ManHuangUIControllerBase
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            UserNameInput = t.GetComponent<UIInput>("Container/Content/UserNameInput/Input");
            PasswordInput = t.GetComponent<UIInput>("Container/Content/PasswordInput/Input");
            ErrTips = t.GetComponent<UILabel>("Container/Content/ErrTips");
            EnterGameBtn = t.GetComponent<UIButton>("Container/Content/EnterGameBtn");
            AccountRecordGrid = t.GetComponent<UIGrid>("Container/Content/AccountGrid");
            RecordBG = t.GetComponent<UISprite>("Container/Content/RecordBG");
            AccountEntryTemplate = t.FindEx("Container/Content/AccountTemplate").gameObject;
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/Content/UserNameInput/PullDown").onClick.Add(new EventDelegate(OnPopUserRecordBtnClick));
            t.GetComponent<UIButton>("Container/Content/EnterGameBtn").onClick.Add(new EventDelegate(OnEnterGameBtnClick));
            t.GetComponent<UIButton>("Container/Content/AccountTemplate").onClick.Add(new EventDelegate(() =>OnSelectAccountEntryClick(t.GetComponent<UILabel>("Container/Content/AccountTemplate/InputLabel"))));

            t.GetComponent<UIEventTrigger>("Container/Content/PasswordInput/ForgetTips").onClick.Add(new EventDelegate(OnForgetPasswordBtnClick));
            t.GetComponent<UIEventTrigger>("Container/Content/RegisterTips").onClick.Add(new EventDelegate(OnRegisterAccountBtnClick));

        }
        
    	public UIInput UserNameInput;
    	public UIInput PasswordInput;
    	public UILabel ErrTips;
    	public UIButton EnterGameBtn;
    	public UIGrid AccountRecordGrid;
    	public UISprite RecordBG;
    	public GameObject AccountEntryTemplate;
    	bool isCreateAccountRecord;
    	bool popAccountRecord;
    
    	public System.Action<string, string, string, string> LoginCallback;
    
    	public void FillData(string userID, string password)
    	{
    		UserNameInput.value = userID;
    		PasswordInput.value = password;
    	}
    
    	public void OnUserNameChange()
    	{
    	}
    
    	public void OnPasswordChange()
    	{
    	}
    
    	public void OnPopUserRecordBtnClick()
    	{
    		popAccountRecord = !popAccountRecord;
    		if (!isCreateAccountRecord)
    		{
    			isCreateAccountRecord = true;
    			if (EB.Sparx.MHAuthenticator.UserInfoList != null && EB.Sparx.MHAuthenticator.UserInfoList.Count > 0)
    			{
    				int counter = 0;
    				for (int index = EB.Sparx.MHAuthenticator.UserInfoList.Count - 1; index >= 0; --index)
    				{
    					GameObject ins = GameUtils.InstantiateEx(AccountEntryTemplate.transform, AccountRecordGrid.transform, EB.Sparx.MHAuthenticator.UserInfoList[index].phone);
                        UILabel t = ins.GetComponent<UILabel>("InputLabel");
                        ins.GetComponent <UIButton>().onClick.Add(new EventDelegate(() => OnSelectAccountEntryClick(t)));
                        t.text=t.transform .GetChild (0).GetComponent <UILabel>().text = EB.Sparx.MHAuthenticator.UserInfoList[index].phone;
                        counter++;
    					if (counter >= 10)
    						break;
    				}				
    				AccountRecordGrid.Reposition();
    				RecordBG.height = 48 + (int)(AccountRecordGrid.cellHeight * counter);
    			}
    		}
    		AccountRecordGrid.gameObject.CustomSetActive(popAccountRecord);
    		RecordBG.gameObject.CustomSetActive(popAccountRecord && EB.Sparx.MHAuthenticator.UserInfoList != null && EB.Sparx.MHAuthenticator.UserInfoList.Count > 0);
    	}
    
    	public void OnSelectAccountEntryClick(UILabel uilabel)
    	{
    		UserNameInput.value = uilabel.text;
    		PasswordInput.value = EB.Sparx.MHAuthenticator.UserInfoList.Find(m => m.phone == uilabel.text).password;
    
    		popAccountRecord = !popAccountRecord;
    		Transform selectSprite= uilabel.transform.parent.transform.Find("SelectSprite");
    		selectSprite.gameObject.CustomSetActive(true);
    		StartCoroutine(DelayDisactiveSelectFrame(selectSprite));
    	}
    
    	IEnumerator DelayDisactiveSelectFrame(Transform selectSprite)
    	{
    		yield return new WaitForSeconds(0.07f);
    		selectSprite.gameObject.CustomSetActive(false);
    		AccountRecordGrid.gameObject.CustomSetActive(false);
    		RecordBG.gameObject.CustomSetActive(false);
    	}
    
    	public void OnForgetPasswordBtnClick()
    	{
    		Show(false);
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().AskController.Show(true);
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().AskController.SetData(eRegisterOrForgetAccount.Forget, "");
    	}
    
    	public void OnRegisterAccountBtnClick()
    	{
    		Show(false);
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().AskController.Show(true);
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().AskController.SetData(eRegisterOrForgetAccount.Register, "");
    	}
    
    	public void OnEnterGameBtnClick()
    	{
    		if (string.IsNullOrEmpty(UserNameInput.value))
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_1039"));
    			return;
    		}
    
    		string inputStr = UserNameInput.value;
    		long phoneNum = 0;
    		if (!long.TryParse(inputStr, out phoneNum) || phoneNum.ToString().Length != 11 || phoneNum.ToString().Substring(0, 1) != "1")
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_1329"));
    			return;
    		}
    		// password 
    		if (string.IsNullOrEmpty(PasswordInput.value))
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangFindPasswordController_5475"));
    			return;
    		}
    
    		if (GameUtils.GetStringWidth(PasswordInput.value) < 6)
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangFindPasswordController_6782"));
    			return;
    		}
    
            UIAccountController.LoginByPhone(UserNameInput.value, PasswordInput.value, delegate (string phone, string openId, string accessToken, string password)
            {
                if (LoginCallback != null)
                {
                    LoginCallback(phone, openId, accessToken, password);
                }
                ManHuangAccountUIController ctrl = mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>();
                if (ctrl != null)
                {
                    ctrl.controller.Close();
                }
            });
    	}
    }
    
}
