using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class HelpApplyDynamicCellController : DynamicCellController<HelpApplyNode>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            PlayerNameLabel = t.GetComponent<UILabel>("LTPlayerPortrait/Name");
            PlayerLevelLabel = t.GetComponent<UILabel>("LTPlayerPortrait/LevelBG/Level");
            PlayerIcon = t.GetComponent<UISprite>("LTPlayerPortrait/Icon");
            PlayerFrame = t.GetComponent<UISprite>("LTPlayerPortrait/Icon/Frame");
            t.GetComponent<ContinueClickCDTrigger>("AgreeBtn").m_CallBackPress.Add(new EventDelegate(OnAgreeBtnClick));
            t.GetComponent<ContinueClickCDTrigger>("RejectBtn").m_CallBackPress.Add(new EventDelegate(OnRejectBtnClick));
        }
    	public UILabel PlayerNameLabel, PlayerLevelLabel;
        public UISprite PlayerIcon;
        public UISprite PlayerFrame;

        private HelpApplyNode mItemData;
    
    	public override void Fill(HelpApplyNode itemData)
    	{
    		mItemData = itemData;
            PlayerNameLabel.text = PlayerNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = mItemData.PlayerName;
    		PlayerLevelLabel.text = mItemData.PlayerLevel.ToString();
    		PlayerIcon.spriteName = mItemData.Portrait;
            PlayerFrame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(mItemData.HeadFrame).iconId;
        }
    
    	public override void Clean()
    	{
    		mItemData = null;
            PlayerNameLabel.text = PlayerNameLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Empty;
    		PlayerLevelLabel.text = string.Empty;
    		PlayerIcon.spriteName = string.Empty;
            PlayerFrame.spriteName = string.Empty;
        }
    
    	public void OnAgreeBtnClick()
    	{
    		if (HelpApplyController.GetResidueHelpApplyCount()<=0)
    		{
    			MessageTemplateManager.ShowMessage(902093, EB.Localizer.GetString("ID_uifont_in_LTMainMenu_Label_11"), null);
    			return;
    		}
    		AlliancesManager.Instance.Agree(mItemData.Uid, mItemData.InviteId, delegate (bool successful)
    		{				
    			if (successful)
    			{
    				AlliancesManager.Instance.HelpApplyInfo.HaveHelpNum += 1;
    				if(mItemData!=null) MessageTemplateManager.ShowMessage(902089, mItemData.PlayerName, null);
    			}
                if (mItemData != null) AlliancesManager.Instance.HelpApplyInfo.Remove(mItemData.Uid);
    			GameDataSparxManager.Instance.SetDirty(AlliancesManager.helpApplyDataId);
    		});
    	}
    
    	public void OnRejectBtnClick()
    	{
    		if (HelpApplyController.GetResidueHelpApplyCount() <= 0)
    		{
    			MessageTemplateManager.ShowMessage(902093, EB.Localizer.GetString("ID_uifont_in_LTMainMenu_Label_11"), null);
    			return;
    		}
    
    		AlliancesManager.Instance.Reject(mItemData.Uid, mItemData.InviteId, delegate ()
    		{
                if (mItemData != null) AlliancesManager.Instance.HelpApplyInfo.Remove(mItemData.Uid);
    			GameDataSparxManager.Instance.SetDirty(AlliancesManager.helpApplyDataId);
    		});
    	}
    }
}
