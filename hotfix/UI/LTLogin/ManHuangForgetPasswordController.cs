using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class ManHuangForgetPasswordController : ManHuangUIControllerBase
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            PhoneNumberInput = t.GetComponent<UIInput>("Container/Content/Grid/PhoneInput/Input");
            PasswordInput = t.GetComponent<UIInput>("Container/Content/Grid/PasswordInput/Input");
            RePasswordInput = t.GetComponent<UIInput>("Container/Content/Grid/RePasswordInput/Input");
            ErrTipsLabel = t.GetComponent<UILabel>("Container/Content/ErrorTips");
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/UIFrame/Top/CloseBtn").onClick.Add(new EventDelegate(OnCloseBtnClick));
            t.GetComponent<UIButton>("Container/Content/SureBtn").onClick.Add(new EventDelegate(OnSureBtnClick));

        }


    
    	public UIInput PhoneNumberInput;
    	public UIInput PasswordInput;
    	public UIInput RePasswordInput;
    	public UILabel ErrTipsLabel;
    
    	public void SetData(string phoneNumber)
    	{
    		PhoneNumberInput.value = phoneNumber;
    		ErrTipsLabel.text = "";
    		PasswordInput.value = "";
    		RePasswordInput.value = "";
    	}
    
    	public void OnPasswordChange()
    	{
    		InputCheck(PasswordInput.value);
    	}
    
    	public void OnRePasswordChange()
    	{
    		InputCheck(RePasswordInput.value);
    	}
    
    	public void OnSureBtnClick()
    	{
    		if (string.IsNullOrEmpty(PasswordInput.value))
    		{
    			ErrTipsLabel.text = EB.Localizer.GetString("ID_codefont_in_ManHuangFindPasswordController_5475");
    			return;
    		}
    
    		if (string.IsNullOrEmpty(RePasswordInput.value))
    		{
    			ErrTipsLabel.text = EB.Localizer.GetString("ID_codefont_in_ManHuangFindPasswordController_5578");
    			return;
    		}
    
    		if (PasswordInput.value != RePasswordInput.value)
    		{
    			ErrTipsLabel.text = EB.Localizer.GetString("ID_codefont_in_ManHuangFindPasswordController_5682");
    			return;
    		}
    
    		if (!InputCheck(PasswordInput.value))
    		{
    			return;
    		}
    
    		if (!InputCheck(RePasswordInput.value))
    		{
    			return;
    		}
    
    		UIAccountController.Modifypassword(PhoneNumberInput.value, "", PasswordInput.value, delegate () {
    			Show(false);
                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.Show(true);
                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.FillData(PhoneNumberInput.value, PasswordInput.value);
    		});
    	}
    
    	public override void OnCloseBtnClick()
    	{
    		base.OnCloseBtnClick();
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.Show(true);
    	}
    
    	bool InputCheck(string inputValue)
    	{
    		if (GameUtils.GetStringWidth(inputValue) < 6)
    		{
    			ErrTipsLabel.text = EB.Localizer.GetString("ID_codefont_in_ManHuangFindPasswordController_6782");
    			return false;
    		}
    		ErrTipsLabel.text = "";
    		return true;
    	}
    }
}
