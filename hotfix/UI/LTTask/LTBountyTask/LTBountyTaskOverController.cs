using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTBountyTaskOverController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            LeftTimesLabel = t.GetComponent<UILabel>("Control/Table/TimesLabel");
           t.GetComponent<UIButton>("Control/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));

            t.GetComponent<UIButtonText>("Control/Table/ButtonGrid/SureButton").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButtonText>("Control/Table/ButtonGrid/ContinueButton").onClick.Add(new EventDelegate(OnContinueBtnClick));

        }


    
    	public override bool ShowUIBlocker { get { return true; } }
    
    	public UILabel LeftTimesLabel;
    
    	public override IEnumerator OnAddToStack()
    	{
            string colorStr = (LTBountyTaskHudController.CurHantTimes != 0) ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
    
            LTUIUtil.SetText(LeftTimesLabel, string.Format(EB.Localizer.GetString("ID_codefont_in_LTBountyTaskOverController_477"), colorStr, LTBountyTaskHudController.CurHantTimes, LTBountyTaskHudController.TotalHantTimes));
            //重新刷新下主城
            if (LTMainMenuHudController.Instance != null)
            {
	            LTMainMenuHudController.Instance.OnFocus();
            }
    		return base.OnAddToStack();
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    
    	public void OnContinueBtnClick()
    	{
    		if(AllianceUtil.GetIsInTransferDart(null))
    		{
    			//WorldMapPathManager.Instance.StartPathFindToNpc(MainLandLogic.GetInstance().CurrentSceneName, Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(10068).mainland_name, Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(10068).locator);
    		}
    		else
    		{
    			WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(10068).mainland_name, Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLandEncounter(10068).locator);
    		}
    		controller.Close();
    	}
    }
}
