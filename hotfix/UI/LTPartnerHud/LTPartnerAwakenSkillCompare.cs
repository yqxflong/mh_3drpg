    
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerAwakenSkillCompare : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            PreSkillDes = t.GetComponent<UILabel>("CurSkill/NorSkillDes/PreSkillDes");
            AwakenSkillDes = t.GetComponent<UILabel>("CurSkill/AwaSkillDes/AwakenSkillDes");
            SkillLevel = t.GetComponent<UILabel>("CurSkill/SkillItem/Sprite/Level");
            SkillName = t.GetComponent<UILabel>("CurSkill/NameLabel");
            SkillRange = t.GetComponent<UILabel>("CurSkill/Sprite/LevelLabel");
            SkillType = t.GetComponent<UILabel>("CurSkill/SkillIconBG/Label");
            preSkilllabel = t.GetComponent<UILabel>("CurSkill/NorSkillDes/preSkilllabel");
            awakenSkilllabel = t.GetComponent<UILabel>("CurSkill/AwaSkillDes/awakenSkilllabel");
            SkillIcon = t.GetComponent<DynamicUISprite>("CurSkill/SkillItem/Icon");
            BgSprite = t.GetComponent<UIWidget>("skillcompare");
            SkillRangeBg = t.GetComponent<UISprite>("CurSkill/Sprite");
            SkillTypeBg = t.GetComponent<UISprite>("CurSkill/SkillIconBG");
            SkillCooldownLabel = t.GetComponent<UILabel>("CurSkill/Sprite (1)/LevelLabel");

            t.GetComponent<UIButton>("CurSkill/SkillItem").onClick.Add(new EventDelegate(OnClickAwakenIcon));

            t.GetComponent<ConsecutiveClickCoolTrigger>("Mask").clickEvent.Add(new EventDelegate(OnClickMask));
        }


        
    	public UILabel PreSkillDes, AwakenSkillDes,SkillLevel,SkillName,SkillRange,SkillType,preSkilllabel,awakenSkilllabel;
    	public DynamicUISprite SkillIcon;
    	public UIWidget BgSprite;
    	public UISprite SkillRangeBg,SkillTypeBg;
        public UILabel SkillCooldownLabel;
    	private Hotfix_LT.Data.SkillTemplate awakenSkill;
    	private Hotfix_LT.Data.SkillTemplate preSkill;
    	private int skilllevel = 1;
    	private Color FriendColor = new Color(67 / 255f, 253 / 255f, 122 / 255f, 1);
    	private Color EnemyColor = new Color(254 / 255f, 104 / 255f, 154 / 255f, 1);
    
    	public void Fill(Hotfix_LT.Data.SkillTemplate awaken, Hotfix_LT.Data.SkillTemplate pre, LTPartnerData data)
    	{
	        mDMono.transform.GetComponent<UIPanel>().sortingOrder = mDMono.transform.parent.parent.GetComponent<UIPanel>().sortingOrder + 1;
	        mDMono.transform.GetComponent<UIPanel>().depth = mDMono.transform.parent.parent.GetComponent<UIPanel>().depth + 150;
    		awakenSkill = awaken;
    		preSkill = pre;
    		if (data.HeroStat.active_skill == preSkill.ID)
    		{
    			skilllevel = data.ActiveSkillLevel;
    		}
    		else if (data.HeroStat.passive_skill == preSkill.ID)
    		{
    			skilllevel = data.PassiveSkillLevel;
    		}
    		else if (data.HeroStat.common_skill == preSkill.ID)
    		{
    			skilllevel = data.CommonSkillLevel;
    		}
    		SetSkillType(awaken.Type);
    		SkillIcon.spriteName = awaken.Icon;
    		SkillLevel.text = skilllevel.ToString();
    		SkillName.text = awaken.Name;
    		SetSkillTargetLabel(awaken.SelectTargetType);
    		preSkilllabel.text = preSkilllabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTER_AWAKEN_BEFORE");
    		awakenSkilllabel.text = awakenSkilllabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_PARTER_AWAKEN_LATER");
            
            string cooldownStr = (pre.MaxCooldown > 0) ? (pre.MaxCooldown + EB.Localizer.GetString("ID_uifont_in_CombatHudV4_TurnFont_4")) : EB.Localizer.GetString("ID_SKILL_COOLDOWN_NOT");
            SkillCooldownLabel.text = string.Format("{0}{1}", EB.Localizer.GetString("ID_SKILL_COOLDOWN"), cooldownStr);
    
            PreSkillDes.text = SkillDescTransverter.ChangeDescription(pre.Description, skilllevel, pre.ID);
    		AwakenSkillDes.text = SkillDescTransverter.ChangeDescription(awaken.Description, skilllevel, awaken.ID);
            mDMono.gameObject.transform.localPosition = new Vector3(-1112, (BgSprite.height - 726) / 2, 0);
    	}
    
    	public void OnClickAwakenIcon()
    	{
    		UITooltipManager.Instance.DisplayTooltipForPress(awakenSkill.ID.ToString() + "," + skilllevel, "Skill", "default",new Vector3(1000,0,0), ePressTipAnchorType.RightDown, false, false, false, delegate () { });
    	}
    
    	public void OnClickMask()
    	{
	        mDMono.gameObject.CustomSetActive(false);
    	}
    
    	public void SetSkillTargetLabel(Hotfix_LT.Data.eSkillSelectTargetType targetType)
    	{
    		switch (targetType)
    		{
    			case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_ALL:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_2525");
    				SkillRangeBg.color = EnemyColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_TEMPLATE:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_2711");
    				SkillRangeBg.color = EnemyColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.ENEMY_RANDOM:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_2895");
    				SkillRangeBg.color = EnemyColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.SELF:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3071");
    				SkillRangeBg.color = FriendColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_ALL:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3252");
    				SkillRangeBg.color = FriendColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_RANDOM:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3438");
    				SkillRangeBg.color = FriendColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.FRIEND_TEMPLATE:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3626");
    				SkillRangeBg.color = FriendColor;
    				break;
    			case Hotfix_LT.Data.eSkillSelectTargetType.All_NOT_SELF:
    				SkillRange.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_3811");
    				SkillRangeBg.color = EnemyColor;
    				break;
    		}
    	}
    
    	private void SetSkillType(Hotfix_LT.Data.eSkillType skillType)
    	{
    		if (skillType == Hotfix_LT.Data.eSkillType.ACTIVE)
    		{
    			SkillType.text = SkillType.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4070");
    			SkillTypeBg.color = LT.Hotfix.Utility.ColorUtility.RedColor;
    		}
    		else if (skillType == Hotfix_LT.Data.eSkillType.NORMAL)
    		{
    			SkillType.text = SkillType.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4440");
    			SkillTypeBg.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
    		}
    		else if (skillType == Hotfix_LT.Data.eSkillType.PASSIVE)
    		{
    			SkillType.text = SkillType.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4751");
    			SkillTypeBg.color = LT.Hotfix.Utility.ColorUtility.BlueColor;
    		}
    	}
    }
}
