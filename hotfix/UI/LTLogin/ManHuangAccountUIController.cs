using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class ManHuangAccountUIController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LoginUIController = t.GetMonoILRComponent<ManHuangLoginController>("Panel/LoginAccountUI");
            AskController = t.GetMonoILRComponent<ManHuangAskController>("Panel/AskUI");
            InputVerificationController = t.GetMonoILRComponent<ManHuangInputVerificationCodeController>("Panel/InputVerificationCodeUI");
            RegisterPasswordController = t.GetMonoILRComponent<ManHuangRegisterController>("Panel/RegisterPasswordUI");
            ForgetPasswordController = t.GetMonoILRComponent<ManHuangForgetPasswordController>("Panel/ForgetPasswordUI");
            UserAgreementUIController = t.GetMonoILRComponent<ManHuangAgreementController>("Panel/UserAgreementUI");
            m_BlurBG = t.FindEx("Panel/BackgroundMask").gameObject;
            controller.backButton = t.GetComponent<UIButton>("Panel/LoginAccountUI/Container/UIFrame/Top/CloseBtn");

        }

    	public override bool IsFullscreen() { return true; }
        
    	public ManHuangLoginController LoginUIController;
    	public ManHuangAskController AskController;
    	public ManHuangInputVerificationCodeController InputVerificationController;
    	public ManHuangRegisterController RegisterPasswordController;
    	public ManHuangForgetPasswordController ForgetPasswordController;
    	public ManHuangAgreementController UserAgreementUIController;
        public GameObject m_BlurBG;
    
    	public override void SetMenuData(object param)
    	{		
    		base.SetMenuData(param);
    
    		Hashtable htData = param as Hashtable;
    		string userID = htData["userID"] as string;
    		string password = htData["password"] as string;
    		System.Action<string, string, string, string> loginCallback = htData["callback"] as System.Action<string, string, string, string>;
            LoginUIController.Show(true);
            LoginUIController.FillData(userID, password);
    		LoginUIController.LoginCallback = loginCallback;
            m_BlurBG.CustomSetActive(false);
        }
    
    	public override IEnumerator OnAddToStack()
    	{
    		yield return base.OnAddToStack();
            m_BlurBG.CustomSetActive(true);
        }
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		yield return base.OnRemoveFromStack();
    	}
    
    	void PlayTween(Transform trans)
    	{
    		UITweener[] tweeners = trans.GetComponentsInChildren<UITweener>();
    		for (int j = 0; j < tweeners.Length; ++j)
    		{
    			tweeners[j].tweenFactor = 0;
    			tweeners[j].PlayForward();
    		}
    	}
    
        public override void OnCancelButtonClick()
        {
            LoginUIController.OnCloseBtnClick();
            base.OnCancelButtonClick();
            EB.Debug.LogWarning("ManHuangAccountUIController.LoginCancel!");
            SparxHub.Instance.Disconnect(true);
        }
    }
}
