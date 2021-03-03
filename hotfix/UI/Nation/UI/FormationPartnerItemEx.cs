using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class FormationPartnerItemEx : FormationPartnerItem
    {
        public override void Awake()
        {
            var t = mDMono.transform;
            Container = t.FindEx("HeroIcon").gameObject;
            EmptyGO = t.FindEx("EmptyNode").gameObject;
            LevelLabel = t.GetComponent<UILabel>("HeroIcon/LevelSprite/LabelLevel");
            LevelBgSprite=t.GetComponent<UISprite>("HeroIcon/LevelSprite");
            BreakLabel = t.GetComponent<UILabel>("HeroIcon/Break");
            AttrBGSprite = t.GetComponent<UISprite>("HeroIcon/Attr");
            QualityBorderSprite = t.GetComponent<UISprite>("HeroIcon/Lvlborder");
            QualityBorderSpriteBg = t.GetComponent<UISprite>("HeroIcon/Lvlborder/BG");
            IconSprite = t.GetComponent<DynamicUISprite>("HeroIcon/Icon");

            StarList = new UISprite[6];
            StarList[0] = t.GetComponent<UISprite>("HeroIcon/Stars/01");
            StarList[1] = t.GetComponent<UISprite>("HeroIcon/Stars/11");
            StarList[2] = t.GetComponent<UISprite>("HeroIcon/Stars/21");
            StarList[3] = t.GetComponent<UISprite>("HeroIcon/Stars/31");
            StarList[4] = t.GetComponent<UISprite>("HeroIcon/Stars/41");
            StarList[5] = t.GetComponent<UISprite>("HeroIcon/Stars/51");

            StarUIGrid = t.FindEx("HeroIcon/Stars").gameObject;
        }
   
    	public GameObject Container;
    	public GameObject EmptyGO;
    
    	public override void Fill(OtherPlayerPartnerData data,bool canOpen=false)
    	{
    	    Collider collider = mDMono.gameObject.GetComponent<Collider>();
            if (data == null)
    		{
    			EmptyGO.CustomSetActive(true);
    			Container.CustomSetActive(false);
    			return;
    		}
    		else
            {
                EmptyGO.CustomSetActive(false);
    		    Container.CustomSetActive(true);
            }
    		base.Fill(data);
    	}
    }
}
