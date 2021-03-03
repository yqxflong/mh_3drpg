using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class ManHuangAgreementController : ManHuangUIControllerBase
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            TextLabel = t.GetComponent<UILabel>("Container/Content/Scroll/Container/Text");
            m_Container = t.FindEx("Container").gameObject;

            t.GetComponent<UIButton>("Container/UIFrame/Top/CloseBtn").onClick.Add(new EventDelegate(OnCloseBtnClick));

        }


    
    	public UILabel TextLabel;
    	string license;
    
    	public override void Show(bool isShowing)
    	{
    		base.Show(isShowing);
    		if (isShowing && string.IsNullOrEmpty(license))
    		{
    			UIAccountController.License(delegate (string licenseStr)
    			{
    				LTUIUtil.SetText(TextLabel,license = licenseStr);
    			});
    		}
    	}
    
    	public override void OnCloseBtnClick()
    	{
    		base.OnCloseBtnClick();

            mDMono.transform.parent.parent.GetUIControllerILRComponent<ManHuangAccountUIController>().AskController.Show(true);
    	}
    }
}
