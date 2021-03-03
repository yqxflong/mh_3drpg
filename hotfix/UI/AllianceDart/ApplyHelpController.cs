using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class ApplyHelpController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            ResidueApplyHelpNumLabel = t.GetComponent<UILabel>("Content/ResidueNum/Label");
            Scroll = t.GetMonoILRComponent<ApplyHelpDynamicScroll>("Content/ScrollViewPanel/Placeholder/Nodes");
            controller.backButton = t.GetComponent<UIButton>("LTPopFrame/CloseBtn");

            t.GetComponent<UIButton>("Content/RefreshBtn").onClick.Add(new EventDelegate(OnRefreshBtnClick));

        }
    
    	public override bool ShowUIBlocker { get { return true; } }
    
    	public UILabel ResidueApplyHelpNumLabel;
    	public ApplyHelpDynamicScroll Scroll;
    	[System.NonSerialized]
    	static public string CurrentDartId;
    
    	public override void SetMenuData(object _menuData)
    	{
    		CurrentDartId = _menuData as string;
    	}
    
    	public override IEnumerator OnAddToStack()
    	{
    		AlliancesManager.Instance.GetApplyHelpInfo();
    
    		yield return base.OnAddToStack();
    	}
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    
    	public override void OnFocus()
    	{
    		base.OnFocus();
    		GameDataSparxManager.Instance.RegisterListener(AlliancesManager.applyHelpDataId, OnDataListenter);
    	}
    
    	public override void OnBlur()
    	{
    		base.OnBlur();
    		GameDataSparxManager.Instance.UnRegisterListener(AlliancesManager.applyHelpDataId, OnDataListenter);
    	}
    
    	private void OnDataListenter(string path, INodeData data)
    	{
            AllianceApplyHelpInfo info = data as AllianceApplyHelpInfo;
    		//System.TimeSpan transferCountdownTs = System.TimeSpan.FromSeconds(AlliancesManager.Instance.TransferDartInfo.TransferEndTs - EB.Time.Now);
    		
    		LTUIUtil.SetText(ResidueApplyHelpNumLabel,LT.Hotfix.Utility.ColorUtility.FormatResidueStr(GetResidueCount(info.HaveApplyNum), VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortHelpTimes)));
    
    		Scroll.SetItemDatas(info.ApplyHelpList.ToArray());
    	}
    
    	public void OnRefreshBtnClick()
    	{
    		AlliancesManager.Instance.GetApplyHelpInfo();
    	}
    
    	static public int GetResidueCount(int haveApplyNum)
    	{
    		int totalNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortHelpTimes);
    		int residueNum = totalNum - haveApplyNum;
    		if (residueNum < 0)
    		{
    			EB.Debug.LogError("apply help residueNum < 0");
    			residueNum = 0;
    		}
    		return residueNum;
    	}
    }
}
