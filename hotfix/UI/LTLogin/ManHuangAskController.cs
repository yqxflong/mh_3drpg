using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public enum eRegisterOrForgetAccount
    {
    	Register,
    	Forget
    }
    
    public class ManHuangAskController : ManHuangUIControllerBase
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            TitleLabel = t.GetComponent<UILabel>("Container/UIFrame/Top/Label");
            UserPhoneInput = t.GetComponent<UIInput>("Container/Content/PhoneInput/Input");
            BtnLabel = t.GetComponent<UILabel>("Container/Content/Btn/Label");
            ErrTipsLabel = t.GetComponent<UILabel>("Container/Content/ErrTips");
            AgreementClickLabel = t.GetComponent<UILabel>("Container/Content/IAgree");
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/UIFrame/Top/CloseBtn").onClick.Add(new EventDelegate(OnCloseBtnClick));
            t.GetComponent<UIButton>("Container/Content/Btn").onClick.Add(new EventDelegate(OnBtnClick));

            t.GetComponent<UIEventTrigger>("Container/Content/GotoLoginBtn").onClick.Add(new EventDelegate(OnGotoLoginBtnClick));
            t.GetComponent<UIEventTrigger>("Container/Content/IAgree/Agreement").onClick.Add(new EventDelegate(OnUserAgreementBtnClick));

        }
        
    	public UILabel TitleLabel;
    	public UIInput UserPhoneInput;
    	public UILabel BtnLabel;
    	public UILabel ErrTipsLabel;
    	public UILabel AgreementClickLabel;
    	private eRegisterOrForgetAccount mRof;
    
    	public void SetData(eRegisterOrForgetAccount rof,string phoneNumber)
    	{
    		UserPhoneInput.value = "";
    		ErrTipsLabel.text = "";
    		mRof = rof;
    		if (rof == eRegisterOrForgetAccount.Register)
    		{
    			LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_592"));
    			LTUIUtil.SetText(BtnLabel, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_631"));
    			AgreementClickLabel.gameObject.SetActive(true);
    		}
    		else if (rof == eRegisterOrForgetAccount.Forget)
    		{
    			LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_780"));
    			LTUIUtil.SetText(BtnLabel, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_819"));
    			AgreementClickLabel.gameObject.SetActive(false);
    		}
    	}
    
    	public void OnBtnClick()
    	{
    		if (string.IsNullOrEmpty(UserPhoneInput.value))
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_1039"));
    			return;
    		}
    
    		string inputStr = UserPhoneInput.value;
    		long phoneNum = 0;
    		if (!long.TryParse(inputStr, out phoneNum) || phoneNum.ToString().Length != 11 || phoneNum.ToString().Substring(0, 1) != "1")
    		{
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangAskController_1329"));
    			UserPhoneInput.value = "";
    			return;
    		}
    
    		if (!mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.GetIsExpire(mRof, UserPhoneInput.value))
    		{
    			Show(false);
                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.Show(true);
                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.SetData(mRof, UserPhoneInput.value, 0);
    			return;
    		}
    
    		if (mRof == eRegisterOrForgetAccount.Register)
    		{
    			UIAccountController.RegByPhone(UserPhoneInput.value, delegate (string err) {
    				if (string.IsNullOrEmpty(err))
    				{
    					Show(false);
                        mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.Show(true);
                        mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.SetData(mRof, UserPhoneInput.value, EB.Time.Now + 60);
    				}
    			});
    		}
    		else if (mRof == eRegisterOrForgetAccount.Forget)
    		{
    			UIAccountController.Forgetpassword(UserPhoneInput.value, delegate (string msg) {
    				if (msg == "OK")
    				{
    					Show(false);
                        mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.Show(true);
                        mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().InputVerificationController.SetData(mRof, UserPhoneInput.value, EB.Time.Now + 60);
    				}
    			});
    		}
    	}
    
    	public void OnGotoLoginBtnClick()
    	{
    		Show(false);
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.Show(true);
    	}
    
    	public void OnUserAgreementBtnClick()
    	{
    		Show(false);
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().UserAgreementUIController.Show(true);
    	}
    
    	public override void OnCloseBtnClick()
    	{
    		base.OnCloseBtnClick();
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.Show(true);
    	}
    }
}
