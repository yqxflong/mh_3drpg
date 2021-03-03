namespace Hotfix_LT.UI
{
    public class RankIconOrNumber : DynamicMonoHotfix
    {
    	public UILabel m_Number;
    	public UISprite m_Sprite;
    	//public UISprite m_BG;
    
    	private int m_Rank;
    	private const string RANK_ICON_1= "Ty_Ranking_Icon_Jin";
    	private const string RANK_ICON_2 = "Ty_Ranking_Icon_Yin";
    	private const string RANK_ICON_3 = "Ty_Ranking_Icon_Tong";
    	private const string RANK_ICON_45 = "RankingList_Icon_45";
    	public int Rank
    	{
    		set
    		{
    			m_Rank = value;
    			if(m_Number!=null)
    				LTUIUtil.SetText(m_Number,(m_Rank + 1)+".");
    			if (value < 3)
    				m_Sprite.gameObject.SetActive(true);
    			else
    				m_Sprite.gameObject.SetActive(false);
    
    			switch (m_Rank)
    			{
    				case 0:
    					m_Sprite.spriteName = RANK_ICON_1;
    					break;
    				case 1:
    					m_Sprite.spriteName = RANK_ICON_2;
    					break;
    				case 2:
    					m_Sprite.spriteName = RANK_ICON_3;
    					break;
    			}
    		}
    	}

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_Number = t.GetComponentEx<UILabel>();
            m_Sprite = t.GetComponent<UISprite>("BG");
        }
    }
}
