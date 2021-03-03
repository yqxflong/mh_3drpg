using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class NationDonateItemCell : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            PlayerNameLabel = t.GetComponent<UILabel>("NoEmpty/LTPlayerPortrait/Name");
            PlayerLevelLabel = t.GetComponent<UILabel>("NoEmpty/LTPlayerPortrait/LevelBG/Level");
            PortraitUISprite = t.GetComponent<DynamicUISprite>("NoEmpty/LTPlayerPortrait/Icon");
            DonateLabel = t.GetComponent<UILabel>("NoEmpty/DonateLabel");
            EmptyNode = t.FindEx("Empty").gameObject;
            NoEmptyNode = t.FindEx("NoEmpty").gameObject;
            BG = t.GetComponent<UISprite>("BG");
            Clean();
        }


    
    	public UILabel PlayerNameLabel;
    	public UILabel PlayerLevelLabel;
    	public DynamicUISprite PortraitUISprite;
    	public UILabel DonateLabel;
    	public GameObject EmptyNode;
    	public GameObject NoEmptyNode;
        public UISprite BG;
      
    	public void Clean()
    	{
    		LTUIUtil.SetText(PlayerNameLabel, "");
    		LTUIUtil.SetText(PlayerLevelLabel, "");
    		PortraitUISprite.spriteName = "";
    		LTUIUtil.SetText(DonateLabel, "");
            BG.color = new Color(213f / 255f, 223f / 255f, 232f / 255f);
        }
    
    	public void Fill(NationDonateData memberData)
    	{
    		if (memberData == null || memberData.Rank < 0)
    		{
    			EmptyNode.gameObject.SetActive(true);
    			NoEmptyNode.gameObject.SetActive(false);
                BG.color = new Color(213f / 255f, 223f / 255f, 232f / 255f);
            }
    		else
    		{			
    			LTUIUtil.SetText(PlayerNameLabel, memberData.Name);
    			LTUIUtil.SetText(PlayerLevelLabel, memberData.Level.ToString());
    			PortraitUISprite.spriteName = memberData.Portrait;
    			LTUIUtil.SetText(DonateLabel, memberData.WeekDonate.ToString());
    			EmptyNode.gameObject.CustomSetActive(false);
    			NoEmptyNode.gameObject.CustomSetActive(true);
                BG.color = (memberData.Uid ==LoginManager.Instance.LocalUserId.Value)?new Color (188f / 255f,254f / 255f, 216f / 255f) : new Color(213f / 255f, 223f / 255f, 232f / 255f);
            }		
    	}
    }
}
