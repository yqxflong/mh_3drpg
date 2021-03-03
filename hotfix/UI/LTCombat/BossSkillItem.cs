using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class BossSkillItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            TypeTips = t.GetComponent<UILabel>("Type/Label");
            TypeTipsBG = t.GetComponent<UISprite>("Type");
            Icon = t.GetComponent<DynamicUISprite>("Icon/Icon");
            Name = t.GetComponent<UILabel>("Name");
            Desc = t.GetComponent<UILabel>("Desc");
            PartingLine = t.FindEx("BG").gameObject;
        }

    	public UILabel TypeTips;
    	public UISprite TypeTipsBG;
    	public DynamicUISprite Icon;
    	public UILabel Name;
    	public UILabel Desc;
    	public GameObject PartingLine;
    
    	public void Fill(int skillId, int skillLevel)
    	{
    		Hotfix_LT.Data.SkillTemplate tplData = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(skillId);
    		Fill(tplData, skillLevel);
    	}
    	public void Fill(Hotfix_LT.Data.SkillTemplate tplData,int skillLevel)
    	{
    		if (tplData.Type == Hotfix_LT.Data.eSkillType.ACTIVE)
    		{
    			LTUIUtil.SetText(TypeTips, EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4070"));
    			TypeTipsBG.color = LT.Hotfix.Utility.ColorUtility.RedColor;
    		}
    		else if (tplData.Type == Hotfix_LT.Data.eSkillType.NORMAL)
    		{
    			LTUIUtil.SetText(TypeTips, EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4440"));
    			TypeTipsBG.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
    		}
    		else if (tplData.Type == Hotfix_LT.Data.eSkillType.PASSIVE)
    		{
    			LTUIUtil.SetText(TypeTips, EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4751"));
    			TypeTipsBG.color = LT.Hotfix.Utility.ColorUtility.BlueColor;
    		}
    
    		Icon.spriteName = tplData.Icon;
    		LTUIUtil.SetText(Name, tplData.Name);
    		LTUIUtil.SetText(Desc, SkillDescTransverter.ChangeDescription(tplData.Description, skillLevel, tplData.ID));
    	}
    }
}
