using UnityEngine;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class ApplyHelpDynamicCellController : DynamicCellController<ApplyHelpNode>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            PlayerNameLabel = t.GetComponent<UILabel>("LTPlayerPortrait/Name");
            PlayerLevelLabel = t.GetComponent<UILabel>("LTPlayerPortrait/LevelBG/Level");
            PlayerIcon = t.GetComponent<UISprite>("LTPlayerPortrait/Icon");
            PlayerFrame = t.GetComponent<UISprite>("LTPlayerPortrait/Icon/Frame");
            ApplyBtn = t.FindEx("ApplyBtn").gameObject;
            HasApplyBtn = t.FindEx("HasApplyBtn").gameObject;


            t.GetComponent<ContinueClickCDTrigger>("ApplyBtn").m_CallBackPress.Add(new EventDelegate(OnApplyHelpClick));
        }
    
    	public UILabel PlayerNameLabel;
    	public UILabel PlayerLevelLabel;
        public UISprite PlayerIcon;
        public UISprite PlayerFrame;
        public GameObject ApplyBtn,HasApplyBtn;
    
    	private ApplyHelpNode mItem;
    
    	public override void Clean()
    	{
    		mItem = null;
    
    		PlayerNameLabel.text = string.Empty;
    		PlayerLevelLabel.text = string.Empty;
    		PlayerIcon.spriteName = string.Empty;
            PlayerFrame.spriteName = string.Empty;
        }
    
    	public override void Fill(ApplyHelpNode item)
    	{
    		mItem = item;
    
    		LTUIUtil.SetText(PlayerNameLabel , mItem.PlayerName);
    		PlayerLevelLabel.text = mItem.PlayerLevel.ToString();
    		PlayerIcon.spriteName = mItem.Portrait;
            PlayerFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(mItem.HeadFrame).iconId;
            RefreshBtnState();
    	}
    
    	private void RefreshBtnState()
    	{
    		switch (mItem.State)
    		{
    			case eAllianceApplyHelpState.None:
    				ApplyBtn.gameObject.CustomSetActive(true);
    				HasApplyBtn.gameObject.CustomSetActive(false);
    				break;
    			case eAllianceApplyHelpState.Applyed:
    			case eAllianceApplyHelpState.Agreed:
    				ApplyBtn.gameObject.CustomSetActive(false);
    				HasApplyBtn.gameObject.CustomSetActive(true);
    				break;
    		}
    	}
    
    	public void OnApplyHelpClick()
    	{
    		int totalNum = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.EscortHelpTimes);
    		int residueNum = totalNum - AlliancesManager.Instance.ApplyHelpInfo.HaveApplyNum;
    		if (residueNum <= 0)
    		{
    			MessageTemplateManager.ShowMessage(902093, EB.Localizer.GetString("ID_APPLY_HELP_COUNT"), null);
    			EB.Debug.LogError("apply Help Residue Num <0");
    		}
    		else
    		{
    			AlliancesManager.Instance.ApplyHelp(mItem.Uid,delegate(bool successful) {
    				if (successful)
    				{
    					mItem.State = eAllianceApplyHelpState.Applyed;
    					RefreshBtnState();
    				}
    				else
    				{
    					Debug.LogError("ApplyHelp Req Fail");
    					AlliancesManager.Instance.ApplyHelpInfo.Remove(mItem.Uid);
    					GameDataSparxManager.Instance.SetDirty(AlliancesManager.applyHelpDataId);
    				}				
    			});
    		}
    	}
    }
}
