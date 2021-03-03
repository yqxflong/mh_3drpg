using EB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPLayerLevelUpTipController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            TaskName = t.GetComponent<UILabel>("LookupScrollView/Placeholder/Grid/Channel/Sprite (0)/Label");
            TaskDes = t.GetComponent<UILabel>("LookupScrollView/Placeholder/Grid/Channel/Label");
            UpgradeName = t.GetComponent<UILabel>("LookupScrollView/Placeholder/Grid/Channel (1)/Sprite (0)/Label");
            UpgradeDes = t.GetComponent<UILabel>("LookupScrollView/Placeholder/Grid/Channel (1)/Label");
            TipLabel = t.GetComponent<UILabel>("TextureBg/DesLabel");
            controller.backButton = t.GetComponent<UIButton>("TextureBg/CloseBtn");
            ShowTipDesLabel = t.GetComponent<UILabel>("VigourAndUpgradeTip/Label");
            LevelView = t.Find("LookupScrollView").gameObject;
            ShowTipView = t.Find("VigourAndUpgradeTip").gameObject;
            VigourIcon = t.Find("VigourAndUpgradeTip/VigorSpriteRoot").gameObject;
            UpgradeTip = t.Find("VigourAndUpgradeTip/PartnerUpgradeTip").gameObject;
            Item1Line = t.GetComponent<UISprite>("VigourAndUpgradeTip/PartnerUpgradeTip/Item (1)/Frame");
            Item1Frame = t.GetComponent<UISprite>("VigourAndUpgradeTip/PartnerUpgradeTip/Item (1)/FrameBG");
            Item2Line = t.GetComponent<UISprite>("VigourAndUpgradeTip/PartnerUpgradeTip/Item (2)/Frame");
            Item2Frame = t.GetComponent<UISprite>("VigourAndUpgradeTip/PartnerUpgradeTip/Item (2)/FrameBG");
            Item1level = t.Find("VigourAndUpgradeTip/PartnerUpgradeTip/Item (1)/BreakObj").gameObject;
            Item2level = t.Find("VigourAndUpgradeTip/PartnerUpgradeTip/Item (2)/BreakObj").gameObject;
            UpgradeLabel1 = t.GetComponent<UILabel>("VigourAndUpgradeTip/PartnerUpgradeTip/Item (1)/BreakObj/Break");
            UpgradeLabel2 = t.GetComponent<UILabel>("VigourAndUpgradeTip/PartnerUpgradeTip/Item (2)/BreakObj/Break");
            GotoButton = t.GetComponent<UIButton>("VigourAndUpgradeTip/GoButton");
            if (GotoButton.onClick != null)
            {
                GotoButton.onClick.Clear();
            }
            GotoButton.onClick.Add(new EventDelegate(OnClickPartnerUpgradeButton));
            t.GetComponent<UIButton>("LookupScrollView/Placeholder/Grid/Channel/Sprite (0)").onClick.Add(new EventDelegate(OnClickDayTaskButton));
            t.GetComponent<UIButton>("LookupScrollView/Placeholder/Grid/Channel/Sprite (1)").onClick.Add(new EventDelegate(OnClickDayTaskButton));
            t.GetComponent<UIButton>("LookupScrollView/Placeholder/Grid/Channel (1)/Sprite (0)").onClick.Add(new EventDelegate(OnClickPartnerUpgradeButton));
            t.GetComponent<UIButton>("LookupScrollView/Placeholder/Grid/Channel (1)/Sprite (1)").onClick.Add(new EventDelegate(OnClickPartnerUpgradeButton));

        }
       
    	public UILabel TaskName, TaskDes, UpgradeName, UpgradeDes, TipLabel,ShowTipDesLabel,UpgradeLabel1,UpgradeLabel2;
    	private GameObject LevelView, ShowTipView,VigourIcon,UpgradeTip,Item1level,Item2level;
    	private UISprite Item1Line,Item2Line,Item1Frame,Item2Frame;//设置icon显示信息，由于设置信息较少暂不特殊处理
    	private UIButton GotoButton;
    	private string param = "Develop_Upgrade";
    	public override bool ShowUIBlocker
    	{
    		get
    		{
    			return true ;
    		}
    	}
    
    	public override bool IsFullscreen()
    	{
    		return false;
    	}
    	public override void SetMenuData(object param)
    	{
    		string type = "Levelup";
    		if (param != null)//参数为string类型，Levelup为升级助手，Vigour为体力使用提示，Upgrade_3\Upgrade_6\Upgrade_9\为升阶扫荡引导
    		{
    			 type= param as string;
    		}
    
    		InitShowView(type);
    	}
    	private void InitShowView(string type)
    	{
    		if (type.Equals("Levelup"))
    		{
    			LevelView.CustomSetActive(true);
    			ShowTipView.CustomSetActive(false);
    			TaskName.text = TaskName.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_TASK");
    			TaskDes.text = TaskDes.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_LEVELUP_DAILYTASK");
    			UpgradeName.text = UpgradeName.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_uifont_in_LTPartnerHud_Label_6");
    			UpgradeDes.text = UpgradeDes.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_LEVELUP_UPGRADE");
    			TipLabel.text = EB.Localizer.GetString("ID_LEVELUP_TIP_1");
    			param = "Develop_Upgrade";
    		}
    		else if (type.Equals("Vigour"))
    		{
    			LevelView.CustomSetActive(false);
    			ShowTipView.CustomSetActive(true);
    			UpgradeTip.CustomSetActive(false);
    			VigourIcon.CustomSetActive(true);
    			int vigour = 0;
    			DataLookupsCache.Instance.SearchIntByID("res.vigor.v", out vigour);
                string tip = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_7");
                ShowTipDesLabel.text = ShowTipDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(tip, "       ", vigour);
                string[] splits= tip.Replace("{0}", "|").Split('|');
                if (splits.Length > 1)
                {
                    VigourIcon.GetComponent<UILabel>().text = splits[0];
                }
                TipLabel.text = EB.Localizer.GetString("ID_LEVELUP_TIP_2");
    			param = "Develop_Upgrade";
    
    		}
    		else if (type.Contains("Upgrade"))
    		{
    			param = "Develop_Upgrade";
    			LevelView.CustomSetActive(false);
    			ShowTipView.CustomSetActive(true);
    			VigourIcon.CustomSetActive(false);
    			UpgradeTip.CustomSetActive(true);
    			//GotoButton.transform.localPosition = new Vector3(576f, GotoButton.transform.localPosition.y, 0);
    			if (type.Equals("Upgrade_3"))
    			{
    				param = "Develop_Upgrade_3";
    				Item1Line.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[0];
    				Item1Frame.color = LT.Hotfix.Utility.ColorUtility.FrameWhiteColor;
    				Item2Line.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[1];
    				Item2Frame.color = LT.Hotfix.Utility.ColorUtility.FrameGreenColor;
    				Item1level.CustomSetActive(false);
    				Item2level.CustomSetActive(false);
    				ShowTipDesLabel.text = ShowTipDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_4");
    				TipLabel.text = string.Format(EB.Localizer.GetString("ID_LEVELUP_TIP_3"),EB.Localizer.GetString("ID_scenes_lost_main_chapter_103_name"));
    			}
    			else if (type.Equals("Upgrade_6"))
    			{
    				param = "Develop_Upgrade_6";
    				Item1Line.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[1];
    				Item1Frame.color = LT.Hotfix.Utility.ColorUtility.FrameGreenColor;
    				Item2Line.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[2];
    				Item2Frame.color = LT.Hotfix.Utility.ColorUtility.FrameBlueColor;
    				Item1level.CustomSetActive(false);
    				Item2level.CustomSetActive(false);
    				ShowTipDesLabel.text = ShowTipDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_5");
    				TipLabel.text = string.Format(EB.Localizer.GetString("ID_LEVELUP_TIP_3"), EB.Localizer.GetString("ID_scenes_lost_main_chapter_106_name"));
    			}
    			else if (type.Equals("Upgrade_9"))
    			{
    				param = "Develop_Upgrade_9";
    				Item1Line.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[2];
    				Item1Frame.color = LT.Hotfix.Utility.ColorUtility.FrameBlueColor;
    				Item2Line.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[2];
    				Item2Frame.color = LT.Hotfix.Utility.ColorUtility.FrameBlueColor;
    				Item1level.CustomSetActive(false);
    				Item2level.CustomSetActive(true);
    				UpgradeLabel2.text = UpgradeLabel2.transform.GetChild(0).GetComponent<UILabel>().text = "+1"; 
    				ShowTipDesLabel.text = ShowTipDesLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTNER_UPGRADE_TIP_6");
    				TipLabel.text = string.Format(EB.Localizer.GetString("ID_LEVELUP_TIP_3"), EB.Localizer.GetString("ID_scenes_lost_main_chapter_109_name"));
    			}
    		}
    	}
    	public override IEnumerator OnAddToStack()
    	{
    		return base.OnAddToStack();
    	}
    	public override IEnumerator OnRemoveFromStack()
    	{
    		controller.Close();
    		return base.OnRemoveFromStack();
    	}
    	public void OnClickDayTaskButton()
    	{
    		controller.Close();
    		InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
    		GlobalMenuManager.Instance.Open("NormalTaskView", null);
    		
    	}
    	public void OnClickPartnerUpgradeButton()
    	{
    		controller.Close();
    		InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
    		Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
            if (!ft.IsConditionOK())
            {
    			MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,ft.GetConditionStr());
    			return;
            }
    		GlobalMenuManager.Instance.Open("LTPartnerHud", param);
    
    	}
    
    	public override void OnCancelButtonClick()
    	{
    		base.OnCancelButtonClick();
    	}
    }
}
