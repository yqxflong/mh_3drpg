using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
    /// <summary>
    /// 服务器数组		服务器数据：WorldList:[{WID:10,WName:bbbb",State:"0/1/2/3",UName:"ddddd",Level:"100",Type:"10000100/10000200/10000300/10000500/10000600}]
    /// LastWorld:10
    /// </summary>
namespace Hotfix_LT.UI
{
    public class ServerListController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            m_UIRegionList = t.GetMonoILRComponent<RegionDymamicScroll>("Container/AllRegionList/List/Placeholder/Grid");
            m_UIRegionList.SetAction(OnSelectRegionClick);

            m_UIServerList = t.GetMonoILRComponent<UIServerDynamicScroll>("Container/ServerList/List/Placeholder/Grid");
            m_Container = t.FindEx("Container").gameObject;
            m_RecommendListView = t.FindEx("Container/ServerList/RecommendView").gameObject;
            m_NormalListView = t.FindEx("Container/ServerList/List").gameObject;
            m_UIRecommendServerList = t.GetMonoILRComponent<UIServerDynamicScroll>("Container/ServerList/RecommendView/Recommend/Placeholder/Grid");
            m_ServersPerFrame = 10;
            controller.hudRoot = t.GetComponent<Transform>();
            controller.backButton = t.GetComponent<UIButton>("Container/LTFrame/Content/Title/CloseBtn");
            LastlyLoginServerArray = new UIServerCellController[4];
            LastlyLoginServerArray[0] = t.GetMonoILRComponent<UIServerCellController>("Container/ServerList/RecommendView/LastlyLogin/Grid/Item/A");
            LastlyLoginServerArray[1] = t.GetMonoILRComponent<UIServerCellController>("Container/ServerList/RecommendView/LastlyLogin/Grid/Item/B");
            LastlyLoginServerArray[2] = t.GetMonoILRComponent<UIServerCellController>("Container/ServerList/RecommendView/LastlyLogin/Grid/Item (1)/A");
            LastlyLoginServerArray[3] = t.GetMonoILRComponent<UIServerCellController>("Container/ServerList/RecommendView/LastlyLogin/Grid/Item (1)/B");

        }
        
    	//大区列表
    	public RegionDymamicScroll m_UIRegionList;
    	public UIServerDynamicScroll m_UIServerList;
    	public GameObject m_Container;
    	public GameObject m_RecommendListView;
    	public GameObject m_NormalListView;
    	public UIServerCellController[] LastlyLoginServerArray;
    	public UIServerDynamicScroll m_UIRecommendServerList;
    
        //每一页显示的服务器数目
        public int m_ServersPerFrame=10;
    
    	public override void Show(bool isShowing)
        {
            m_Container.SetActive(isShowing);
        }
    
        public override bool Visibility
        {
            get { return m_Container.activeSelf; }
        }
    
        public override bool ShowUIBlocker
        {
            get { return true; }
        }
    
        public override IEnumerator OnAddToStack()
        {
            InitData();
    
            ShowData();
    
            yield return base.OnAddToStack();
        }
    
        public override IEnumerator OnRemoveFromStack()
    	{
    		return base.OnRemoveFromStack();
    	}
    
    	void InitData()
        {
    		Selected = null;
    		PrepareRegionData(LoginManager.Instance.GameWorlds, LoginManager.Instance.Account);
        }
    
        void ShowData()
        {
    		m_UIRegionList.SetItemDatas ( m_RegionsData.ToArray());
    		ShowRecommendList();
    		StartCoroutine(SelectRecommendRegionCoroutine());
    	}
    
    	IEnumerator SelectRecommendRegionCoroutine()
    	{
    		yield return new WaitForEndOfFrame();
    		yield return null;
    		RegionCellController cellCtrl = m_UIRegionList.mDMono .transform.GetChild(0).GetMonoILRComponent<RegionCellController>();
    		while (cellCtrl.RegionCollectionData==null)
    		{
    			yield return null;
    		}
    		OnSelectRegionClick(cellCtrl);
    	}
    
    	private void ShowRecommendList()
    	{
    		if (HasSettingRecommendData)
    			return;
    		HasSettingRecommendData = true;
    		List<ServerData> lastlyLoginList = GetLastlyLoginData(LoginManager.Instance.GameWorlds, LoginManager.Instance.Account);
    
    		for (int i = 0; i < lastlyLoginList.Count; ++i)
    		{
    			LastlyLoginServerArray[i].mDMono .gameObject.CustomSetActive(true);
    			LastlyLoginServerArray[i].Fill(lastlyLoginList[i]);
    		}
    		for (int j = lastlyLoginList.Count; j<4;++j)
    		{
    			LastlyLoginServerArray[j].mDMono .gameObject.CustomSetActive(false);
    		}
    
    		m_UIRecommendServerList.SetItemDatas(GetRecommendData(LoginManager.Instance.GameWorlds, LoginManager.Instance.Account).ToArray());
    	}
    
    	private RegionCellController Selected=null;
    	private bool HasSettingRecommendData;
    	public void OnSelectRegionClick(RegionCellController region)
    	{
    		if (region == Selected)
    			return;
    		if(Selected!=null)
    			Selected.HighLight(false);
    		Selected = region;
    		Selected.HighLight(true);
    
    		if (Selected.DataIndex == 0)
    		{
    			if (!m_RecommendListView.activeSelf)
    				m_RecommendListView.gameObject.CustomSetActive(true);
    			if (m_NormalListView.activeSelf)
    				m_NormalListView.gameObject.CustomSetActive(false);
    			ShowRecommendList();
    		}
    		else
    		{
    			if (m_RecommendListView.activeSelf)
    				m_RecommendListView.gameObject.CustomSetActive(false);
    			if (!m_NormalListView.activeSelf)
    				m_NormalListView.gameObject.CustomSetActive(true);
    			m_UIServerList.SetItemDatas(Selected.RegionCollectionData.List.ToArray());
    		}
    	}
    
        public static string MapWorldState(GameWorld.eState state)
        {
            switch (state)
            {
                case GameWorld.eState.Smooth:
    			case GameWorld.eState.Busy:
    			case GameWorld.eState.Hot:
    				return EB.Localizer.GetString("ID_SERVER_STATE_HUOBAO");
                case GameWorld.eState.Down:
    				return EB.Localizer.GetString("ID_SERVER_STATE_WEIHU");
    			case GameWorld.eState.New:
    				return EB.Localizer.GetString("ID_SERVER_STATE_XINQU");
    			default:
    				return EB.Localizer.GetString("ID_SERVER_STATE_HUOBAO");
            }
        }
        
    	static Color32 GreenColor = new Color32(25,193,99,255);
    	static Color32 RedColor = new Color32(255, 60, 62, 255);
    	public static Color MapWorldColor(GameWorld.eState state)
        {		
            switch (state)
            {
                case GameWorld.eState.Smooth:
                    return RedColor;
                case GameWorld.eState.Busy:
                    return RedColor;
                case GameWorld.eState.Hot:
                    return RedColor;
                case GameWorld.eState.Down:
                    return Color.white;
    			case GameWorld.eState.New:
    				return GreenColor;
    			default:
                    return RedColor;
            }
        }
    
    	public static string MapWorldFlag(GameWorld.eState state)
    	{
    		switch (state)
    		{
    			case GameWorld.eState.Smooth:
    				return "Ty_Flag_2";
    			case GameWorld.eState.Busy:
    				return "Ty_Flag_1";
    			case GameWorld.eState.Hot:
    				return "Ty_Flag_1";
    			case GameWorld.eState.Down:
    				return "Ty_Flag_3";
    			case GameWorld.eState.New:
    				return "Ty_Flag_2";
    			default:
    				return "Ty_Flag_2";
    		}
    	}
    
    	public List<RegionCollection> m_RegionsData = new List<RegionCollection>();
        //计算总共有多少个大区栏目	推荐大区+ 服务器数/每大区显示数目	 不能整除就+1
    
        //根据大区数显示栏目
        void PrepareRegionData(GameWorld[] srcData, Account account)
        {
            int allservers = srcData[srcData.Length-1].Id;
    
            int allregions = allservers % m_ServersPerFrame == 0 ? allservers / m_ServersPerFrame : allservers / m_ServersPerFrame + 1;
    
            m_RegionsData.Clear();
    		RegionCollection region_recommend = new RegionCollection();
            region_recommend.Name=EB.Localizer.GetString("ID_LOGIN_RECOMEND_NAME");
    		m_RegionsData.Add(region_recommend);
    		int endindex = srcData.Length - 1;
            for (int i = allregions - 1; i >= 0; i--)
            {
    			RegionCollection region = new RegionCollection();
                int startnumber = i * m_ServersPerFrame + 1;
                int endnumber = (i+1) * m_ServersPerFrame;
                string RName= startnumber + "--"+ endnumber + EB.Localizer.GetString("ID_LOGIN_SERVER_NAME");
    			region.Name = RName;
    			List<ServerData> regionServersData = PrepareRegionServersData(srcData, startnumber - 1, ref endindex, account);
    			regionServersData.Reverse();
    			region.List= regionServersData;
    			if (regionServersData==null || regionServersData.Count==0)
    			{
    				continue;
    			}
                m_RegionsData.Add(region);
            }
        }
    
        private List<ServerData> PrepareRegionServersData(GameWorld[] srcData, int startindex,ref int endindex, Account account)
        {
    		List<ServerData> serverData =new List<ServerData>();
            for (int i = endindex; i >=0; i--)
            {
    			endindex = i;
                if (srcData[i].Id <= startindex)
    			{
    				break;
    			}
    			ServerData tmp = new ServerData();
                tmp.State = srcData[i].State;
                tmp.WID = srcData[i].Id;
                tmp.WName = srcData[i].Name;
                var f = System.Array.Find(account.Users, u => u.WorldId == srcData[i].Id);
                if (f != null)
                {				
                    tmp.Level = f.Level;
                    tmp.Name = f.Name;
                }
    			serverData.Add(tmp);
    		}
            return serverData;
        }
        
        //找到所有有角色的服务器	 和最后一个服务器  如果没有角色 则推荐里面只显示最新的一个服务器
        private List<ServerData> GetLastlyLoginData(GameWorld[] srcData, Account account)
        {
    		List<ServerData> lastlyLoginData = new List<ServerData>();
            int count = srcData.Length;
    		for (int i = 0; i < count; i++)
            {
                var f = System.Array.Find(account.Users, u => u.WorldId == srcData[i].Id);
                if (f != null)
                {
    				ServerData tmp = new ServerData();
                    tmp.State = srcData[i].State;
    				tmp.WID = srcData[i].Id;
                    tmp.WName = srcData[i].Name;
    				tmp.Name = f.Name;
                    tmp.Level = f.Level;
    				tmp.CreateTime = f.CreateTime;
    				lastlyLoginData.Add(tmp);
    			}
            }
    
    		lastlyLoginData.Sort(Comparison);
    		List<ServerData> retLastlyLoginData = new List<ServerData>();
    		for (int i = 0; i < 4; ++i)
    		{
    			if (i < lastlyLoginData.Count)
    				retLastlyLoginData.Add(lastlyLoginData[i]);
    			else
    				break;
    		}
    		
    		return retLastlyLoginData;
        }
    
    	public int Comparison(ServerData x, ServerData y)
    	{
    		return y.CreateTime - x.CreateTime;
    	}
    
    	private List<ServerData> GetRecommendData(GameWorld[] srcData, Account account)
    	{
    		List<ServerData> RecommendData = new List<ServerData>();
    		int count = srcData.Length;
    		for (int i = 0; i < count; i++)
    		{
    			var gameWorld = srcData[i];
    			if (gameWorld.State == GameWorld.eState.New)
    			{
    				ServerData tmp = new ServerData();
    				tmp.State = gameWorld.State;
    				tmp.WID = gameWorld.Id;
    				tmp.WName = gameWorld.Name;
    				var f = System.Array.Find(account.Users, u => u.WorldId == srcData[i].Id);
    				if (f != null)
    				{
    					tmp.Level = f.Level;
    					tmp.Name = f.Name;
    				}
    				RecommendData.Add(tmp);
    			}
    		}
    		return RecommendData;
    	}
    }
}
