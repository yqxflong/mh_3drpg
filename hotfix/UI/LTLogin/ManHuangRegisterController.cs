using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class ManHuangRegisterController : ManHuangUIControllerBase
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            PhoneInput = t.GetComponent<UIInput>("Container/Content/Grid/Phone/Input");
            PasswordInput = t.GetComponent<UIInput>("Container/Content/Grid/PasswordInput/Input");
            RePasswordInput = t.GetComponent<UIInput>("Container/Content/Grid/RePasswordInput/Input");
            InviteInput = t.GetComponent<UIInput>("Container/Content/Grid/InviteCodeInput/Input");
            ErrTipsLabel = t.GetComponent<UILabel>("Container/Content/ErrorTips");
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/UIFrame/Top/CloseBtn").onClick.Add(new EventDelegate(OnCloseBtnClick));
            t.GetComponent<UIButton>("Container/Content/SureBtn").onClick.Add(new EventDelegate(OnSureBtnClick));

        }
        
    	public UIInput PhoneInput;
    	public UIInput PasswordInput;
    	public UIInput RePasswordInput;
    	public UIInput InviteInput;
    	public UILabel ErrTipsLabel;
    
    	public void SetData(string phoneNumber)
    	{
    		PhoneInput.value = phoneNumber;
    		PasswordInput.value = "";
    		RePasswordInput.value = "";
    		ErrTipsLabel.text = "";		
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
    
    		UIAccountController.CreateUser(PasswordInput.value, InviteInput.value, delegate () {
    			Show(false);
    			mDMono .transform .parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.Show(true);
                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().LoginUIController.FillData(PhoneInput.value, PasswordInput.value);
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
