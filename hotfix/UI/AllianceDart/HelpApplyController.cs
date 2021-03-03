using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class HelpApplyController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ResidueHelpNumLabel = t.GetComponent<UILabel>("Content/ResidueNum/Label");
            Scroll = t.GetMonoILRComponent<HelpApplyDynamicScroll>("Content/ScrollViewPanel/Placeholder/Nodes");
            controller.backButton = t.GetComponent<UIButton>("LTPopFrame/CancelBtn");

            t.GetComponent<UIButton>("Content/RejectAllBtn").onClick.Add(new EventDelegate(OnRejectAllBtnClick));

        }
 
    	public override bool ShowUIBlocker { get { return true; } }
    
    	public UILabel ResidueHelpNumLabel;
    	public HelpApplyDynamicScroll Scroll;
    
    	public override IEnumerator OnAddToStack()
    	{
    		AlliancesManager.Instance.GetHelpApplyInfo();
    
    		yield return base.OnAddToStack();
    	}
    
    	public override void OnFocus()
    	{
    		base.OnFocus();
    		GameDataSparxManager.Instance.RegisterListener(AlliancesManager.helpApplyDataId, OnDataListener);
    	}
    
    	public override void OnBlur()
    	{
    		base.OnBlur();
    		GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.helpApplyDataId, OnDataListener);
    	}
    
    	private void OnDataListener(string path, INodeData data)
    	{
    		AllianceHelpApplyInfo info = data as AllianceHelpApplyInfo;
    
    		LTUIUtil.SetText(ResidueHelpNumLabel,LT.Hotfix.Utility.ColorUtility.FormatResidueStr(GetResidueHelpApplyCount(), VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortReinforceTimes)));
    
    		Scroll.SetItemDatas(info.HelpApplyList.ToArray());
    	}
    
    	public void OnRejectAllBtnClick()
    	{
    		AlliancesManager.Instance.RejectAll(delegate(bool successful) {
    			if (successful)
    			{
    				AlliancesManager.Instance.HelpApplyInfo.HelpApplyList.Clear();
    				GameDataSparxManager.Instance.SetDirty(AlliancesManager.helpApplyDataId);
    			}
    		});
    	}
    
    	static public int GetResidueHelpApplyCount()
    	{
    		int totalNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortReinforceTimes);
    		int haveHelpNum = AlliancesManager.Instance.HelpApplyInfo.HaveHelpNum;
    		int residueNum = totalNum - haveHelpNum;
    		if (residueNum < 0)
    		{
    			EB.Debug.LogError("help apply residueNum < 0");
    			residueNum = 0;
    		}
    
    		return residueNum;
    	}
    }
}
