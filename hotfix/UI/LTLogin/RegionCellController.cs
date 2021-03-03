using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
    
namespace Hotfix_LT.UI
{
    public class ServerData
    {
    	public int WID;
    	public string WName;
    	public GameWorld.eState State;
    	public int CreateTime;
    	public string Name;
    	public int Level;
    }
    
    public class RegionCollection
    {
    	public string Name;
    	public List<ServerData> List;
    	public bool Selected;
    }
    
    public class RegionCellController : DynamicCellController<RegionCollection>
    {
        public System.Action<RegionCellController> callBack;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_WName = t.GetComponent<UILabel>("Name");
            m_BG = t.GetComponent<UISprite>("BG");

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate ( OnBtnClick));
        }
        
    	public RegionCollection RegionCollectionData { private set; get; }
    	public UILabel m_WName;
    	public UISprite m_BG;

        public void SetAction(System.Action<RegionCellController> action)
        {
            callBack = action;
        }

        public override void Clean()
    	{
    				
    	}
    
        public void OnBtnClick()
        {
            if (callBack != null) callBack(this);
        }

    	public override void Fill(RegionCollection itemData)
    	{
    		RegionCollectionData = itemData;
    		LTUIUtil.SetText(m_WName,RegionCollectionData.Name);		
    		HighLight(RegionCollectionData.Selected);
    	}
    
    	public void HighLight(bool state)
    	{
    		RegionCollectionData.Selected = state;
    		if(state)
    		{
    			m_BG.spriteName = "Ty_Mail_Di3";
    		}
    		else if (DataIndex % 2 == 0)
    		{
    			m_BG.spriteName = "Ty_Mail_Di1";
    		}
    		else
    		{
    			m_BG.spriteName = "Ty_Mail_Di2";
    		}
    	}
    }
}
