using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class VerificationCodeInfo
    {
    	public string Code;
    	public int ExpireTime;
    }
    
    public class ManHuangInputVerificationCodeController : ManHuangUIControllerBase
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Input = t.GetComponent<UIInput>("Container/Content/Input");
            Input.onChange.Add(new EventDelegate(OnNumberInputChange));
            ErrorTips = t.GetComponent<UILabel>("Container/Content/ErrorTips");
            TargetPhoneLabel = t.GetComponent<UILabel>("Container/Content/TargetPhoneTips");
            CountdownLabel = t.GetComponent<UILabel>("Container/Content/LeftTimeLabel");
            OtherTipsLabel = t.GetComponent<UILabel>("Container/Content/OtherTips");

            NumLabels = new UILabel[6];
            NumLabels[0] = t.GetComponent<UILabel>("Container/Content/NumberBGGrid/Num/Label");
            NumLabels[1] = t.GetComponent<UILabel>("Container/Content/NumberBGGrid/Num (1)/Label");
            NumLabels[2] = t.GetComponent<UILabel>("Container/Content/NumberBGGrid/Num (2)/Label");
            NumLabels[3] = t.GetComponent<UILabel>("Container/Content/NumberBGGrid/Num (3)/Label");
            NumLabels[4] = t.GetComponent<UILabel>("Container/Content/NumberBGGrid/Num (4)/Label");
            NumLabels[5] = t.GetComponent<UILabel>("Container/Content/NumberBGGrid/Num (5)/Label");

            ReSendBtn = t.GetComponent<UIButton>("Container/Content/SendBtn");
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/UIFrame/Top/CloseBtn").onClick.Add(new EventDelegate(OnCloseBtnClick));
            t.GetComponent<UIButton>("Container/Content/SendBtn").onClick.Add(new EventDelegate(OnReSendBtnClick));

        }
        
    	public UIInput Input;
    	public UILabel ErrorTips;
    	public UILabel TargetPhoneLabel;
    	public UILabel CountdownLabel;
    	public UILabel OtherTipsLabel;
        public UILabel[] NumLabels;
    	public UIButton ReSendBtn;
    	private eRegisterOrForgetAccount mRof;
    	private string PhoneNumber;
    	private bool IsRequesting;
    	private int mExpireTime;
    	private  Dictionary<string, VerificationCodeInfo> RegisterVerificationInfoDic=new Dictionary<string, VerificationCodeInfo>();
    	private Dictionary<string, VerificationCodeInfo> ForgetVerificationInfoDic=new Dictionary<string, VerificationCodeInfo>();
    
    	public void SetData(eRegisterOrForgetAccount rof,string phoneNumber,int expireTime)
    	{
    		mRof = rof;
    		PhoneNumber = phoneNumber;
    		if (mRof == eRegisterOrForgetAccount.Register && expireTime>0)
    		{
    			if (!RegisterVerificationInfoDic.ContainsKey(phoneNumber))
    				RegisterVerificationInfoDic.Add(phoneNumber, new VerificationCodeInfo()
    				{ ExpireTime = expireTime });
    			else
    				RegisterVerificationInfoDic[phoneNumber].ExpireTime = expireTime;
    		}
    		else if(mRof == eRegisterOrForgetAccount.Forget && expireTime > 0)
    		{
    			if (!ForgetVerificationInfoDic.ContainsKey(phoneNumber))
    				ForgetVerificationInfoDic.Add(phoneNumber, new VerificationCodeInfo()
    				{ ExpireTime = expireTime });
    			else
    				ForgetVerificationInfoDic[phoneNumber].ExpireTime = expireTime;
    		}
    		mExpireTime = GetExpireTime();
    		Input.value = "";
    		Input.enabled = true;
    		ErrorTips.text = "";
    		LTUIUtil.SetText(TargetPhoneLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_ManHuangInputVerificationCodeController_1755"), PhoneNumber));
    
    		StartCoroutine(SendVerificationCodeCountdown());
    
            for (int i = 0; i < NumLabels.Length; i++)
            {
                NumLabels[i].text = string.Empty;
            }
    	}
    
        private WaitForSeconds wait1 = new WaitForSeconds(1f);
        int timeDiff;
    	IEnumerator SendVerificationCodeCountdown()
    	{
    		if (mExpireTime > EB.Time.Now)
    		{
    			TargetPhoneLabel.gameObject.SetActive(true);
    			CountdownLabel.gameObject.SetActive(true);
    			OtherTipsLabel.gameObject.SetActive(true);
    			ReSendBtn.gameObject.SetActive(false);
    		}		
    		while (true)
    		{
    			timeDiff = mExpireTime - EB.Time.Now;
    			LTUIUtil.SetText(CountdownLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_ChatController_3281"), timeDiff));
    			if (timeDiff <= 0)
    			{
    				TargetPhoneLabel.gameObject.SetActive(false);
    				CountdownLabel.gameObject.SetActive(false);
    				OtherTipsLabel.gameObject.SetActive(false);
    				ReSendBtn.gameObject.SetActive(true);
    				yield break;
    			}
    			yield return wait1;
    		}
    	}
    
        private void InitNumLabel()
        {
            for (int i = 0; i < NumLabels.Length; i++)
            {
                if (i < Input.value.Length)
                {
                    NumLabels[i].text = Input.value[i].ToString();
                }
                else
                {
                    NumLabels[i].text = string.Empty;
                }
            }
        }
    
    	public void OnNumberInputChange()
    	{
            InitNumLabel();
    		int numberLength = Input.value.Length;
    		if (numberLength >= 6)
    		{
    			if (mRof == eRegisterOrForgetAccount.Register)
    			{
    				if (!string.IsNullOrEmpty(RegisterVerificationInfoDic[PhoneNumber].Code) && !GetIsExpire(mRof, PhoneNumber))
    				{
    					if (RegisterVerificationInfoDic[PhoneNumber].Code != Input.value)
    					{
    						Input.value = "";
    						MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangInputVerificationCodeController_3585"));
    					}
    					else
    					{
    						Show(false);
                            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().RegisterPasswordController.Show(true);
                            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().RegisterPasswordController.SetData(PhoneNumber);
    					}
    				}
    				else
    				{
    					if (IsRequesting)
    						return;
    					IsRequesting = true;
    					UIAccountController.SMSVerify(PhoneNumber, Input.value, delegate (bool successful)
    					{
    						IsRequesting = false;
    						if (successful)
    						{
    							RegisterVerificationInfoDic[PhoneNumber].Code = Input.value;
    							Input.enabled = false;
    							Show(false);
                                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().RegisterPasswordController.Show(true);
                                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().RegisterPasswordController.SetData(PhoneNumber);
    						}
    						else
    						{
    							Input.value = "";
    						}
    					});
    				}
    			}
    			else if (mRof == eRegisterOrForgetAccount.Forget)
    			{
    				if (!string.IsNullOrEmpty(ForgetVerificationInfoDic[PhoneNumber].Code) && !GetIsExpire(mRof, PhoneNumber))
    				{
    					if (ForgetVerificationInfoDic[PhoneNumber].Code != Input.value)
    					{
    						Input.value = "";
    						MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ManHuangInputVerificationCodeController_3585"));
    					}
    					else
    					{
    						Show(false);
                            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().ForgetPasswordController.Show(true);
                            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().ForgetPasswordController.SetData(PhoneNumber);
    					}
    				}
    				else
    				{
    					if (IsRequesting)
    						return;
    					IsRequesting = true;
    					UIAccountController.ForgetpasswordVerify(PhoneNumber, Input.value, delegate (bool successful) {
    						IsRequesting = false;
    
    						if (successful)
    						{
    							ForgetVerificationInfoDic[PhoneNumber].Code = Input.value;
    							Input.enabled = false;
    							Show(false);
                                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().ForgetPasswordController.Show(true);
                                mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().ForgetPasswordController.SetData(PhoneNumber);
    						}
    						else
    						{
    							Input.value = "";
    						}
    					});
    				}
    			}		
    		}
    		else
    		{
    		}
    	}
    
    	public void OnReSendBtnClick()
    	{
    		if (mRof == eRegisterOrForgetAccount.Register)
    		{
    			UIAccountController.RegByPhone(PhoneNumber, delegate (string err)
    			{
    				if (string.IsNullOrEmpty(err))
    				{
    					RegisterVerificationInfoDic[PhoneNumber].ExpireTime = EB.Time.Now + 60;
    					mExpireTime = GetExpireTime();
    					StartCoroutine(SendVerificationCodeCountdown());
    				}
    			});
    		}
    		else if (mRof == eRegisterOrForgetAccount.Forget)
    		{
    			UIAccountController.Forgetpassword(PhoneNumber, delegate (string msg)
    			{
    				if (msg == "OK")
    				{
    					ForgetVerificationInfoDic[PhoneNumber].ExpireTime = EB.Time.Now + 60;
    					mExpireTime = GetExpireTime();
    					StartCoroutine(SendVerificationCodeCountdown());
    				}
    			});
    		}
    	}
    
    	public override void OnCloseBtnClick()
    	{
    		mExpireTime = 0;
    		base.OnCloseBtnClick();
            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().AskController.Show(true);
    	}
    
    	int GetExpireTime()
    	{
    		int tempExpireTime = 0;
    		if (mRof == eRegisterOrForgetAccount.Register)
    			tempExpireTime = RegisterVerificationInfoDic[PhoneNumber].ExpireTime;
    		else if (mRof == eRegisterOrForgetAccount.Forget)
    			tempExpireTime = ForgetVerificationInfoDic[PhoneNumber].ExpireTime;
    
    		return tempExpireTime;
    	}
    
    	public bool GetIsExpire(eRegisterOrForgetAccount rof,string phoneNumber)
    	{
    		if (rof == eRegisterOrForgetAccount.Register)
    		{
    			if (RegisterVerificationInfoDic.ContainsKey(phoneNumber))
    				return EB.Time.Now >= RegisterVerificationInfoDic[phoneNumber].ExpireTime;
    			else
    				return true;
    		}
    		else if (rof == eRegisterOrForgetAccount.Forget)
    		{
    			if (ForgetVerificationInfoDic.ContainsKey(phoneNumber))
    				return EB.Time.Now >= ForgetVerificationInfoDic[phoneNumber].ExpireTime;
    			else
    				return true;
    		}
    		else
    			return true;
    	}
    }
}
