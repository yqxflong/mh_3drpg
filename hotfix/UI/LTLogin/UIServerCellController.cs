using UnityEngine;
using System.Collections;
using System;
using EB.Sparx;
    
namespace Hotfix_LT.UI
{
    public class UIServerCellController : DynamicCellController<ServerData>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            m_WName_N = t.GetComponent<UILabel>("WName_N");
            m_WID_N = t.GetComponent<UILabel>("WID_N");
            m_State_N = t.GetComponent<UILabel>("StateLabel");
            m_Level = t.GetComponent<UILabel>("Level");
            m_StateBG = t.GetComponent<UISprite>("StateLabel/Sprite");
            m_BG = t.GetComponent<UISprite>("BG");

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClick));
        }


    
    	//private ServerData mData;
    	public UILabel m_WName_N;
    	public UILabel m_WID_N;
    	public UILabel m_State_N;
    	public UILabel m_Level;
    	public UISprite m_StateBG;
    	public UISprite m_BG;
    
    	public override void Clean()
    	{
    		mDMono .gameObject.SetActive(false);
    	}
    
    	public override void Fill(ServerData itemData)
    	{		
    		LTUIUtil.SetText(m_WName_N, itemData.WName);
    		LTUIUtil.SetText(m_WID_N,itemData.WID.ToString());
    		LTUIUtil.SetText(m_State_N,ServerListController.MapWorldState(itemData.State));
    		if (itemData.State == GameWorld.eState.Smooth || itemData.State == GameWorld.eState.Busy || itemData.State ==GameWorld.eState.Hot)
    		{
    			m_StateBG.spriteName = "Login_Label_Red";
    			m_StateBG.color = Color.white;
    		}
    		else if (itemData.State ==GameWorld.eState.New)
    		{
    			m_StateBG.spriteName = "Login_Label_Green";
    			m_StateBG.color = Color.white;
    		}
    		else
    		{
    			m_StateBG.color= Color.magenta;
    		}
    		//m_State_N.transform.parent.transform.GetComponent<UISprite>().spriteName = ServerListController.MapWorldFlag(itemData.State);
    		if (itemData.Level > 0)
    		{
    			LTUIUtil.SetText(m_Level, itemData.Name+" LV." + itemData.Level);
    			m_Level.gameObject.SetActive(true);
    		}
    		else
    		{
    			m_Level.gameObject.SetActive(false);
    		}

    		if (DataIndex % 4 == 0|| DataIndex % 4 == 1)
    		{
    			m_BG.spriteName = "Ty_Mail_Di1";
    		}
    		else if(DataIndex % 4 == 2 || DataIndex % 4 == 3)
    		{
    			m_BG.spriteName = "Ty_Mail_Di2";
    		}
    
    		mDMono . gameObject.SetActive(true);
    	}
    
    	public void OnClick()
    	{
    		//判定服务器状态 如果为维护则点击没有反应
    		//Hashtable data = m_List.m_ItemsData[dataIndex] as Hashtable;
    		int WID = int.Parse(m_WID_N.text);
    		for (var i = 0; i < LoginManager.Instance.GameWorlds.Length; i++)
    		{
                var world = LoginManager.Instance.GameWorlds[i];
                world.Default = world.Id == WID;
    		}
            //关闭自己        
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.CloseMenu, "ServerSelect");
    	}
    }
}
