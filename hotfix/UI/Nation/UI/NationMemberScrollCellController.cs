using UnityEngine;
using System.Collections;
using System;
using EB.Sparx;
    
namespace Hotfix_LT.UI
{
    public class NationMemberScrollCellController : DynamicCellController<NationMember>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            BG = t.GetComponent<UISprite>("BG");
            NameLabel = t.GetComponent<UILabel>("NotEmpty/LTPlayerPortrait/Name");
            LevelLabel = t.GetComponent<UILabel>("NotEmpty/LTPlayerPortrait/TargetLevelBG/Level");
            PortraitSprite = t.GetComponent<DynamicUISprite>("NotEmpty/LTPlayerPortrait/Icon");
            RankLabel = t.GetComponent<UILabel>("NotEmpty/Rank");
            TotalDonateLabel = t.GetComponent<UILabel>("NotEmpty/TotalDonate");
            WeekDonateLabel = t.GetComponent<UILabel>("NotEmpty/WeekDonate");
            StateLabel = t.GetComponent<UILabel>("NotEmpty/State");
            NotEmptyNode = t.FindEx("NotEmpty").gameObject;
            EmptyNode = t.FindEx("EmptyNode").gameObject;
            EmptyNodeRankLabel = t.GetComponent<UILabel>("EmptyNode/Rank");

            t.GetComponent<UIEventTrigger>("NotEmpty/LTPlayerPortrait").onClick.Add(new EventDelegate(OnIconClick));
        }


    
    	public UISprite BG;
    	public UILabel NameLabel;
    	public UILabel LevelLabel;
    	public DynamicUISprite PortraitSprite;
    	public UILabel RankLabel;
    	public UILabel TotalDonateLabel;
    	public UILabel WeekDonateLabel;
    	public UILabel StateLabel;
    	public GameObject NotEmptyNode;
    	public GameObject EmptyNode;
    	public UILabel EmptyNodeRankLabel;
    	NationMember mItemData;
    
    	public override void Clean()
    	{
    
    	}
    
    	public override void Fill(NationMember itemData)
    	{
    		mItemData = itemData;
    		if (DataIndex % 2 == 0)
    		{
    			BG.spriteName = "Ty_Mail_Di1";
    		}
    		else
    		{
    			BG.spriteName = "Ty_Mail_Di2";
    		}
    
    
            if (itemData.Name == LoginManager.Instance.LocalUser.Name)
            {
                BG.spriteName = "Ty_Mail_Di3";
            }
            if (itemData.Name == EB.Localizer.GetString("ID_codefont_in_NationStruct_17151"))
    		{
    			NotEmptyNode.SetActive(false);
    			EmptyNode.SetActive(true);
    			
    			LTUIUtil.SetText(EmptyNodeRankLabel, NationUtil.LocalizeRankName(itemData.Rank));
    		}
    		else
    		{
    			NotEmptyNode.SetActive(true);
    			EmptyNode.SetActive(false);
    			LTUIUtil.SetText(NameLabel, itemData.Name);
    			LTUIUtil.SetText(LevelLabel, itemData.Level.ToString());
    			PortraitSprite.spriteName = itemData.Portrait;
    			LTUIUtil.SetText(RankLabel, NationUtil.LocalizeRankName(itemData.Rank));
    			LTUIUtil.SetText(TotalDonateLabel, itemData.TotalDonate.ToString());
    			LTUIUtil.SetText(WeekDonateLabel, EB.Localizer.GetString("ID_codefont_in_NationMemberScrollCellController_1385")+itemData.WeekDonate.ToString());
    			StateLabel.color = itemData.OfflineTime > 0 ? LT.Hotfix.Utility.ColorUtility.RedColor : LT.Hotfix.Utility.ColorUtility.WhiteColor;
    			LTUIUtil.SetText(StateLabel, AlliancesManager.FormatOfflineDuration(itemData.OfflineTime));
    		}		
    	}
    
    	public void OnIconClick()
    	{
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (mItemData.Name == EB.Localizer.GetString("ID_codefont_in_NationStruct_17151"))
    		{
    			return;
    		}
    		if (mItemData.Uid== LoginManager.Instance.LocalUserId.Value)
    		{
    			return;
    		}
    
    		Hashtable mainData = Johny.HashtablePool.Claim();
    		mainData.Add("name", mItemData.Name);
    		mainData.Add("icon", mItemData.Portrait);
    		mainData.Add("level", mItemData.Level);
    		Hashtable data = Johny.HashtablePool.Claim();
    		data.Add(SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3_TYPE0, 11111);
    		Hashtable viewData = Johny.HashtablePool.Claim();
    		viewData["mainData"] = mainData;
    		viewData["infoType"] = eOtherPlayerInfoType.canInteraction;
    		viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0] = mItemData.Uid;
    		viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE1;
    		viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE1;
    		viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM3] = data;
    		GlobalMenuManager.Instance.Open("LTCheckPlayerFormationInfoUI", viewData);
    	}
    }
}
