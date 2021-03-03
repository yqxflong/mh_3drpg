using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public enum eDartResultType
    {
        TransferSuccess,
        TransferFail,
        TransferFailByTimeout,
        RobSuccess,
        RobFail
    }

    public class DartResultController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();
            var t = controller.transform;
            TitleLabel = t.GetComponent<UILabel>("LTPopFrame/Title");
            ResiduePrefixLabel = t.GetComponent<UILabel>("Content/ResidueNumPrefixLabel");
            ResidueNumLabel = t.GetComponent<UILabel>("Content/ResidueNumLabel");
            SureBtnLabel = t.GetComponent<UILabel>("Content/SureBtn/Label");
            ContinueBtnLabel = t.GetComponent<UILabel>("Content/ContinueBtn/Label");
            RewardRoot = t.FindEx("Content/Award").gameObject;
            FailTipsLabel = t.GetComponent<UILabel>("Content/TimeoutTips");
            RewardItemList = new List<LTShowItem>();
            RewardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Award/LTShowItem"));
            RewardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Award/LTShowItem (1)"));
            RewardItemList.Add(t.GetMonoILRComponent<LTShowItem>("Content/Award/LTShowItem (2)"));
            sOpenFlag = false;
            controller.backButton = t.GetComponent<UIButton>("LTPopFrame/CloseBtn");
            t.GetComponent<UIButton>("Content/SureBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<UIButton>("Content/ContinueBtn").onClick.Add(new EventDelegate(OnContinueBtnClick));
            BG = t.Find("Content/BG").gameObject;
            BG2 = t.Find("Content/BG2").gameObject;
            m_Icon = t.GetComponent<UISprite>("Content/BG2/Head/Icon");
            m_Frame = t.GetComponent<UISprite>("Content/BG2/Head/Icon/Frame");
            t.GetComponent<UIButton>("Content/BG2/Head/Icon").onClick.Add(new EventDelegate(OnPlayerCheckClick));
        }

        public GameObject BG;
        public GameObject BG2;

        public class Interceptor
        {
            public long Uid;
            public string Name;
            public int Level;
            public string CharacterId;
            public int Skin;
            public string Frame;
            public string AllianceName;

            public Interceptor(object obj)
            {
                Uid = EB.Dot.Long("uid", obj, 0);
                Name = EB.Dot.String("name", obj, "");
                Level = EB.Dot.Integer("level", obj, 0);
                CharacterId = EB.Dot.String("portrait", obj, "");
                Skin = EB.Dot.Integer("skin", obj, 0);
                Frame = EB.Dot.String("headFrame", obj, "");
                AllianceName = EB.Dot.String("allianceName", obj, "");
            }
        }
        private Interceptor InterceptorData;

        public UISprite m_Icon;
        public UISprite m_Frame;

        public override bool ShowUIBlocker { get { return true;	 } }
    
    	public UILabel TitleLabel;
    	public UILabel ResiduePrefixLabel;
    	public UILabel ResidueNumLabel;
    	public List<LTShowItem> RewardItemList;
    	public UILabel SureBtnLabel;
    	public UILabel ContinueBtnLabel;
    	public GameObject RewardRoot;
    	public UILabel FailTipsLabel;
    	public static List<LTShowItemData> sRewardDataList;
    	public static eDartResultType sResultType;
    	public static bool sOpenFlag;
    
    	public override void SetMenuData(object param)
    	{
    		base.SetMenuData(param);
            if (param != null)
            {
                InterceptorData = new Interceptor(param);
            }
            else
            {
                InterceptorData = null;
            }
            if (PlayerManager.LocalPlayerController() != null)
            {
                PlayerManager.LocalPlayerController().transform
                    .GetMonoILRComponent<Player.PlayerHotfixController>().StopTransfer();
            }
            //PlayerManager.LocalPlayerController().transform .GetMonoILRComponent <Player .PlayerHotfixController>(). StopTransfer();
    	}
    
    	public override IEnumerator OnAddToStack()
    	{
    		InitUIMode();
    		yield return base.OnAddToStack();
            RewardRoot.GetComponent<UIGrid>().Reposition();
        }
    
    	public override IEnumerator OnRemoveFromStack()
    	{
    		DestroySelf();
    		yield break;
    	}
    
    	void InitUIMode()
    	{
    		List<LTShowItemData> rewardDatas = sRewardDataList;
            BG.CustomSetActive(true);
            BG2.CustomSetActive(false);
            RewardRoot.transform.localPosition = new Vector3(0, RewardRoot.transform.localPosition.y, 0);
            switch (sResultType)
    		{
    			case eDartResultType.TransferSuccess:
    				LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1251"));
                    RewardRoot.gameObject.CustomSetActive(true);
                    SetReward(rewardDatas);
    				FailTipsLabel.gameObject.CustomSetActive(false);
    				LTUIUtil.SetText(ResiduePrefixLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1425"));
    				AllianceEscortUtil.FormatResidueTransferDartNum(ResidueNumLabel);
    				LTUIUtil.SetText(ContinueBtnLabel,EB.Localizer.GetString("ID_codefont_in_DartResultController_1549"));
    				break;
    			case eDartResultType.TransferFail:
                    BG.CustomSetActive(false);
                    BG2.CustomSetActive(true);
                    RewardRoot.transform.localPosition = new Vector3(150, RewardRoot.transform.localPosition.y, 0);
                    if (InterceptorData != null)
                    {
                        var heroInfo = CharacterTemplateManager.Instance.GetHeroInfo(InterceptorData.CharacterId, InterceptorData.Skin);
                        if (heroInfo != null) m_Icon.spriteName = heroInfo.icon;
                        m_Frame.spriteName = EconemyTemplateManager.Instance.GetHeadFrame(InterceptorData.Frame).iconId;
                    }
                    else
                    {
                        m_Icon.gameObject.CustomSetActive(false);
                    }
                    LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1643"));
                    RewardRoot.gameObject.CustomSetActive(true);
                    SetReward(rewardDatas);
    				FailTipsLabel.gameObject.CustomSetActive(false);
    				LTUIUtil.SetText(ResiduePrefixLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1425"));
    				AllianceEscortUtil.FormatResidueTransferDartNum(ResidueNumLabel);
    				LTUIUtil.SetText(ContinueBtnLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1549"));
    				break;
    			case eDartResultType.TransferFailByTimeout:
    				LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1643"));
    				RewardRoot.gameObject.CustomSetActive(false);
    				LTUIUtil.SetText(FailTipsLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2136"));
    				FailTipsLabel.gameObject.CustomSetActive(true);
    				LTUIUtil.SetText(ResiduePrefixLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1425"));
    				AllianceEscortUtil.FormatResidueTransferDartNum(ResidueNumLabel);
    				LTUIUtil.SetText(ContinueBtnLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_1549"));
    				break;
    			case eDartResultType.RobSuccess:
    				LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2460"));
                    RewardRoot.gameObject.CustomSetActive(true);
                    SetReward(rewardDatas);
    				AlliancesManager.Instance.RobDartInfo.RobAwards = null;
    				FailTipsLabel.gameObject.CustomSetActive(false);
    				LTUIUtil.SetText(ResiduePrefixLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2694"));
    				AllianceEscortUtil.FormatResidueRobDartNum(ResidueNumLabel);
    				LTUIUtil.SetText(ContinueBtnLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2814"));
    				break;
    			case eDartResultType.RobFail:
    				LTUIUtil.SetText(TitleLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2903"));
    				RewardRoot.gameObject.CustomSetActive(false);
    				LTUIUtil.SetText(FailTipsLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2994"));
    				FailTipsLabel.gameObject.CustomSetActive(true);
    				LTUIUtil.SetText(ResiduePrefixLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2694"));
    				AllianceEscortUtil.FormatResidueRobDartNum(ResidueNumLabel);
    				LTUIUtil.SetText(ContinueBtnLabel, EB.Localizer.GetString("ID_codefont_in_DartResultController_2814"));
    				break;
    		}
    	}
    
    	void SetReward(List<LTShowItemData> rewardDatas)
    	{
    		for (int viewIndex = 0; viewIndex < RewardItemList.Count; ++viewIndex)
    		{
    			if (viewIndex < rewardDatas.Count)
    			{
    				RewardItemList[viewIndex].LTItemData = rewardDatas[viewIndex];
    				RewardItemList[viewIndex].mDMono.gameObject.CustomSetActive(true);
    			}
    			else
    			{
    				RewardItemList[viewIndex].mDMono.gameObject.CustomSetActive(false);
    			}
    		}
    	}
    
    	public void OnContinueBtnClick()
    	{
    	    FusionAudio.PostEvent("UI/General/ButtonClick");
    		switch (sResultType)
    		{
    			case eDartResultType.TransferSuccess:
    			case eDartResultType.TransferFail:
    			case eDartResultType.TransferFailByTimeout:
    				{
    					AllianceEscortUtil.GotoTranferDart();
    				}
    				break;
    			case eDartResultType.RobSuccess:
    			case eDartResultType.RobFail:
    				{
    					AllianceEscortUtil.GotoRobDart();
    				}
    				break;
    		}
    		controller.Close();
    	}
    
    	public override void OnCancelButtonClick()
    	{
    		base.OnCancelButtonClick();
    	}

        public void OnPlayerCheckClick()
        {
            if (InterceptorData == null) return;
            Hashtable mainData = Johny.HashtablePool.Claim();
            mainData.Add("name", InterceptorData.Name);
            mainData.Add("icon", m_Icon.spriteName);
            mainData.Add("headFrame", m_Frame.spriteName);
            mainData.Add("level", InterceptorData.Level);
            Hashtable viewData = Johny.HashtablePool.Claim();
            viewData["mainData"] = mainData;
            viewData["infoType"] = eOtherPlayerInfoType.canInteraction;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM0] = InterceptorData.Uid;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM1_TYPE1;
            viewData[SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2] = SmallPartnerPacketRule.REQUEST_OTHER_PLAYER_DATA_PARAM2_TYPE1;
            GlobalMenuManager.Instance.Open("LTCheckPlayerFormationInfoUI", viewData);
        }
    }
}
