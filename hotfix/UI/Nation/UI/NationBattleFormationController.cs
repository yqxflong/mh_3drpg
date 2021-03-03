using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using _HotfixScripts.Utils;

namespace Hotfix_LT.UI
{
    public class NationBattleFormationController : UIControllerHotfix, IHotfixUpdate
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            DynamicScroll = t.GetMonoILRComponent<CombatPartnerDynamicScroll>("Content/Bottom/BuddyList/Placeholder/PartnerGrid");
            SelectLight = t.FindEx("BG/Light").gameObject;
            DragZ = -2;
            CurDragItem = t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/DragDropContainer/Template", "Hotfix_LT.UI.NationPartnerItem");
            controller.backButton = t.GetComponent<UIButton>("Content/UINormalFrameBG/CancelBtn");
            controller.hudRoot = t.GetComponent<Transform>("Content/Bottom/BuddyList");
            TeamLockNodes = new TeamLockNode[3];
            TeamLockNodes[0] = new TeamLockNode();
            TeamLockNodes[0].Name = "nation1";
            TeamLockNodes[0].LockRootGO = t.FindEx("Content/Formation/TeamList/nation1/LockRoot").gameObject;
            TeamLockNodes[1] = new TeamLockNode();
            TeamLockNodes[1].Name = "nation2";
            TeamLockNodes[1].LockRootGO = t.FindEx("Content/Formation/TeamList/nation2/LockRoot").gameObject;
            TeamLockNodes[2] = new TeamLockNode();
            TeamLockNodes[2].Name = "nation3";
            TeamLockNodes[2].LockRootGO = t.FindEx("Content/Formation/TeamList/nation3/LockRoot").gameObject;
            FormationPartnerItems = new List<NationPartnerItem>();
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation1/PartnerUIGrid/0_Pos", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation1/PartnerUIGrid/0_Pos (1)", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation1/PartnerUIGrid/0_Pos (2)", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation1/PartnerUIGrid/0_Pos (3)", "Hotfix_LT.UI.NationPartnerItem"));

            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation2/PartnerUIGrid/0_Pos", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation2/PartnerUIGrid/0_Pos (1)", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation2/PartnerUIGrid/0_Pos (2)", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation2/PartnerUIGrid/0_Pos (3)", "Hotfix_LT.UI.NationPartnerItem"));

            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation3/PartnerUIGrid/0_Pos", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation3/PartnerUIGrid/0_Pos (1)", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation3/PartnerUIGrid/0_Pos (2)", "Hotfix_LT.UI.NationPartnerItem"));
            FormationPartnerItems.Add(t.GetMonoILRComponentByClassPath<NationPartnerItem>("Content/Formation/TeamList/nation3/PartnerUIGrid/0_Pos (3)", "Hotfix_LT.UI.NationPartnerItem"));

            t.GetComponent<UIButton>("Content/Bottom/Title/BtnList/AllBtn").onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Content/Bottom/Title/BtnList/AllBtn").gameObject)));          
            t.GetComponent<UIButton>("Content/Bottom/Title/BtnList/FengBtn").onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Content/Bottom/Title/BtnList/FengBtn").gameObject)));           
            t.GetComponent<UIButton>("Content/Bottom/Title/BtnList/HuoBtn").onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Content/Bottom/Title/BtnList/HuoBtn").gameObject)));           
            t.GetComponent<UIButton>("Content/Bottom/Title/BtnList/ShuiBtn").onClick.Add(new EventDelegate(() => OnRaceTabClick(t.FindEx("Content/Bottom/Title/BtnList/ShuiBtn").gameObject)));
            t.GetComponent<UIButton>("Content/Formation/TeamList/nation1/LockRoot").onClick.Add(new EventDelegate(() => OnClickTeam(t.FindEx("Content/Formation/TeamList/nation1").gameObject)));
            t.GetComponent<UIButton>("Content/Formation/TeamList/nation2/LockRoot").onClick.Add(new EventDelegate(() => OnClickTeam(t.FindEx("Content/Formation/TeamList/nation2").gameObject)));
            t.GetComponent<UIButton>("Content/Formation/TeamList/nation3/LockRoot").onClick.Add(new EventDelegate(() => OnClickTeam(t.FindEx("Content/Formation/TeamList/nation3").gameObject)));
          
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		[System.Serializable]
    	public class TeamLockNode
    	{
    		public string Name;
    		public GameObject LockRootGO;
    		public GameObject InTheWarGO;
    		public GameObject DeathGO;
    	}
    
    	public override bool IsFullscreen() { return true; }
    
    	public TeamLockNode[] TeamLockNodes;
    	public List<NationPartnerItem> FormationPartnerItems;
    	public CombatPartnerDynamicScroll DynamicScroll;
    	public GameObject SelectLight;
    	private eAttrTabType mCurPartnerTabType = eAttrTabType.All;
    	NationPartnerItem PreSelect;
    	Dictionary<string,Coroutine> ReviveCoroutineDic=new Dictionary<string, Coroutine>();
    
    	public override IEnumerator OnAddToStack()
    	{
            DynamicScroll.mDMono.gameObject.CustomSetActive(true);
    
            InitTeamFormation();
    
    		NationManager.Instance.GetTeamInfo(null);
    		GameDataSparxManager.Instance.RegisterListener(NationManager.AccountDataId, OnNationAccountListener);
            RefreshPartnerList();
            return base.OnAddToStack();
    
        }
    
    	public override IEnumerator OnRemoveFromStack()
        {
            this.DynamicScroll.mDMono.gameObject.CustomSetActive(false);
            GameDataSparxManager.Instance.UnRegisterListener(NationManager.AccountDataId, OnNationAccountListener);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Nation);
    		DestroySelf();
    		yield break;
    	}
    
    	void OnNationAccountListener(string path, INodeData data)
    	{
    		NationAccount account = data as NationAccount;
    		if (account.TeamDataUpdated)
    		{
    			account.TeamDataUpdated = false;
    			
    			SetTeamState();
    		}
    	}
    
    	public void OnRuleBtnClick()
    	{
    		string text = EB.Localizer.GetString("ID_NATION_BATTLE_RULE");
    		GlobalMenuManager.Instance.Open("LTRuleUIView", text);
    	}
    
    	public void OnRaceTabClick(GameObject ui_toggle)
    	{
    		/*if (!ui_toggle.value)
    			return;*/
    
    		mCurPartnerTabType = ParseTabType(ui_toggle.name);
    		RefreshPartnerList();
    	}
    
        public void OnClickTeam(GameObject teamNode)
        {
            NationBattleTeam teamData = NationManager.Instance.Account.FindTeam(teamNode.name);
            switch (teamData.RealState)
            {
                case eTeamState.InTheWar:
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,EB.Localizer.GetString("ID_codefont_in_NationBattleFormationController_2158"));
                    break;
                case eTeamState.Death:
                case eTeamState.Arrive:
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_NationBattleFormationController_2347"));
                    break;
                default:
                    EB.Debug.LogError("Click Error for this team");
                    break;
            }
        }
    
    	private void RefreshPartnerList()
    	{
    		List<LTPartnerData> partnerList = LTPartnerDataManager.Instance.GetOwnPartnerList();
    		List<LTPartnerData> filterList = new List<LTPartnerData>();
    		for (var i = 0; i < partnerList.Count; i++)
    		{
                var partner = partnerList[i];

                if (IsInTeam(partner.HeroId))
    				continue;
    			if (mCurPartnerTabType == eAttrTabType.All)
    				filterList.Add(partner);
    			else if (mCurPartnerTabType == eAttrTabType.Feng && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Feng)
    				filterList.Add(partner);
    			else if (mCurPartnerTabType == eAttrTabType.Huo && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Huo)
    				filterList.Add(partner);
    			else if (mCurPartnerTabType == eAttrTabType.Shui && partner.HeroInfo.char_type == Hotfix_LT.Data.eRoleAttr.Shui)
    				filterList.Add(partner);
    		}
    		DynamicScroll.SetItemDatas(filterList.ToArray());
    		SetIconDragAction();
    	}
    
    	void InitTeamFormation()
    	{
    		for (int teamIndex = 0; teamIndex < NationUtil.TeamNames.Length; ++teamIndex)
    		{
    			List<TeamMemberData> teamMemDataList = LTFormationDataManager.Instance.GetTeamMemList(NationUtil.TeamNames[teamIndex]);
    
    			int startIndexInAllPartner = teamIndex * 4;
    
    			List<OtherPlayerPartnerData> fillPartnerDataList = new List<OtherPlayerPartnerData>();
    
    			List<int> fillPosList = new List<int>();
    			for (int teamMemIndex = 0; teamMemIndex < teamMemDataList.Count; ++teamMemIndex)
    			{
    				var memberData = teamMemDataList[teamMemIndex];
    				if (memberData.Pos >= 4)
    				{
    					EB.Debug.LogError("memberData.Pos >= 4 for team:{0},Pos:{1}", NationUtil.TeamNames[teamIndex], memberData.Pos);
    					continue;
    				}
    				fillPosList.Add(memberData.Pos);
    			    int HeroUpGradeId = 0;
    				OtherPlayerPartnerData partnerData = new OtherPlayerPartnerData();
    				partnerData.InfoId = memberData.InfoID;
    				partnerData.HeroID = memberData.HeroID;
    			    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.awake.level", partnerData.HeroID), out HeroUpGradeId);
                    var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(memberData.InfoID);
    				partnerData.Name = charTpl.name;
    				partnerData.Attr = charTpl.char_type;
    				partnerData.Icon = charTpl.icon;
    				partnerData.QualityLevel = charTpl.role_grade;
    			    partnerData.UpGradeId = HeroUpGradeId;
                    Hashtable heroStatData = null;
    				if (!DataLookupsCache.Instance.SearchDataByID<Hashtable>("heroStats." + memberData.HeroID, out heroStatData))
    				{
    					EB.Debug.LogError("not hero data for heroid={0}", memberData.HeroID);
    					continue;
    				}
    				partnerData.Level = EB.Dot.Integer("stat.level", heroStatData, 0);
    				partnerData.Star = EB.Dot.Integer("star", heroStatData, 0);
    				partnerData.awakenLevel = EB.Dot.Integer("stat.awaken",heroStatData,0);
    				fillPartnerDataList.Add(partnerData);
    				
    				int fillIndex = startIndexInAllPartner + memberData.Pos;
    				FormationPartnerItems[fillIndex].Fill(partnerData);
    			}
    
    			for (int idx = 0; idx < 4; ++idx)
    			{
    				if (!fillPosList.Contains(idx))
    				{
    					FormationPartnerItems[startIndexInAllPartner+idx].Fill(null);
    				}
    			}
    		}		
    	}
    
    	public void Update()
    	{
    		// base.Update();
    
    		DynamicSetSelectFX();
    	}
    
    	void DynamicSetSelectFX()
    	{
    		var dragitem = GetDragOverItem();
    
    		if (dragitem != null && CurDragItem.mDMono.gameObject.activeSelf)
    		{
    			if (!SelectLight.activeSelf)
    				SelectLight.SetActive(true);
    			SelectLight.transform.SetParent(dragitem.mDMono.transform, false);
    		}
    		else
    		{
    			if (SelectLight.activeSelf)
    				SelectLight.SetActive(false);
    		}
    	}
    
    	void SetTeamState()
    	{
    	    int DeathTeamCount = 0;
    	    int AllDeath = 3;
    	    bool hasTeamStartAction = false;
            NationBattleTeam[] teamDatas = new NationBattleTeam[TeamLockNodes.Length];
    	    for (int i = 0; i < TeamLockNodes.Length; i++)
    	    {
    	        TeamLockNode teamNode = TeamLockNodes[i];
               EB.Debug.LogError(teamNode.Name);
    			NationBattleTeam teamData = NationManager.Instance.Account.FindTeam(teamNode.Name);
    		    teamDatas[i] = teamData;
                if (teamData.RealState == eTeamState.InTheWar|| teamData.RealState == eTeamState.Death || teamData.RealState == eTeamState.Arrive)
    			{
    			    DeathTeamCount++;
                    DisableDrag(teamData.Name, false);
    			    hasTeamStartAction = true;
    			    //当已知有队伍出征的前提下，不能进行上下阵操作
    			    //teamNode.LockRootGO.SetActive(true);
    
    			    //teamNode.DeathGO.SetActive(false);
    			    //teamNode.InTheWarGO.SetActive(true);
    
    			    //不再进行单独计时
    			    //if (!ReviveCoroutineDic.ContainsKey(teamData.Name))
    			    //	ReviveCoroutineDic.Add(teamData.Name, StartCoroutine(ReviveTimer(teamData, teamNode)));
    			    //else if (ReviveCoroutineDic[teamData.Name] != null)
    			    //{
    			    //	StopCoroutine(ReviveCoroutineDic[teamData.Name]);
    			    //	ReviveCoroutineDic[teamData.Name] = StartCoroutine(ReviveTimer(teamData, teamNode));
    			    //}
    			    //else
    			    //	ReviveCoroutineDic[teamData.Name] = StartCoroutine(ReviveTimer(teamData, teamNode));
    			}
    			else
    			{
    				DisableDrag(teamData.Name, true);
    				teamNode.LockRootGO.SetActive(false);
    				//teamNode.DeathGO.SetActive(false);
    				//teamNode.InTheWarGO.SetActive(false);
    			}
    		}
    
    	    if (hasTeamStartAction)
    	    {
    	        for (int i = 0; i < TeamLockNodes.Length; i++)
    	        {
    	            TeamLockNode teamNode = TeamLockNodes[i];
    	            NationBattleTeam teamData = NationManager.Instance.Account.FindTeam(teamNode.Name);
    	            DisableDrag(teamData.Name, false);
                    TeamLockNodes[i].LockRootGO.SetActive(true);
    
    	        }
            }
            if (DeathTeamCount >= AllDeath)
    	    {
    	        for (int i = 0; i < teamDatas.Length; i++)
    	        {
    	            StartCoroutine(ReviveTimer(teamDatas[i], TeamLockNodes[i]));
    	        }
            }
    	}
    
    	IEnumerator ReviveTimer(NationBattleTeam teamData,TeamLockNode lockNode)
    	{
    		while (teamData.RealState != eTeamState.Available)
    		{
    			yield return null;
    		}
    		DisableDrag(teamData.Name, true);
    		lockNode.LockRootGO.SetActive(false);
    		lockNode.DeathGO.SetActive(false);
    		lockNode.InTheWarGO.SetActive(false);
    	}
    
    	void DisableDrag(string teamName,bool isEnable)
    	{
    		int teamIndex = System.Array.IndexOf(NationUtil.TeamNames, teamName);
    		int startIndexInAllPartner = teamIndex * 4;
    		for (int index = startIndexInAllPartner; index < startIndexInAllPartner + 4; ++index)
    		{
    			FormationPartnerItems[index].Collider.enabled = isEnable;
    		}
    
    	    //TeamLockNodes[teamIndex].LockRootGO.GetComponent<Collider>().enabled = !isEnable;
    	}
    
    	private eAttrTabType ParseTabType(string str)
    	{
    		if (str.Contains("All"))
    			return eAttrTabType.All;
    		else if (str.Contains("Feng"))
    			return eAttrTabType.Feng;
    		else if (str.Contains("Huo"))
    			return eAttrTabType.Huo;
    		else if (str.Contains("Shui"))
    			return eAttrTabType.Shui;
    		EB.Debug.LogError("ParseTabType error {0}", str);
    		return eAttrTabType.All;
    	}
    
    	#region drag
    	public int DragZ = -2;
    	public NationPartnerItem CurDragItem;
    	private NationPartnerItem ReferencePartnerItem;
    	private void SetIconDragAction()
    	{
    		DynamicScroll.SetItemDragStartAction(OnModelDragStartByIcon);
    		DynamicScroll.SetItemDragAction(OnModelDrag);
    		DynamicScroll.SetItemDragEndAction(OnModelDragEndByIcon);
    	}
    
    	public void OnModelDragStart(NationPartnerItem dragItem)
    	{
    		if (dragItem != null && !dragItem.IsEmpty)
    		{
    			//isOnlyOne = GetTeamMemList(dragItem.TeamName).Count <= 1;
    
    			ReferencePartnerItem = dragItem;
    			CurDragItem.TeamName = dragItem.TeamName;
    			CurDragItem.IndexInTeam = dragItem.IndexInTeam;
    			CurDragItem.Fill(dragItem.PartnerItem.PartnerData);
    			CurDragItem.mDMono.gameObject.SetActive(true);
    			dragItem.Fill(null);
    			CurDragItem.mDMono.transform.position = new Vector3(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y, DragZ);
    		}
    	}
    
    	public void OnModelDragEnd()
    	{
    		if (!CurDragItem.mDMono.gameObject.activeSelf)
    			return;		
    
    		NationPartnerItem targetItem = GetDragOverItem();
    	    if (targetItem == null)
    	    {
    	        OnModelDragOut(CurDragItem);
    	        CheckTeamState(CurDragItem.TeamName,-1);
            }
    	    else 
    		{
    			if (targetItem.TeamName == CurDragItem.TeamName)
    			{
                    //同一个队伍里面移动伙伴位置
    				bool samePosition = targetItem == ReferencePartnerItem;
    				OnDrag2ModelPos(targetItem.TeamName, CurDragItem.PartnerData.HeroID, targetItem.IndexInTeam, !samePosition, false);
                }
    			else if (!targetItem.IsEmpty)
    			{
                    //伙伴位置对换
    				OnDrag2ModelPos(CurDragItem.TeamName, CurDragItem.PartnerData.HeroID, CurDragItem.IndexInTeam,
    				targetItem.TeamName, targetItem.PartnerData.HeroID, targetItem.IndexInTeam);
                }
    			else
    			{
                    //移动伙伴到其他队伍
    				OnDrag2ModelPos(CurDragItem.TeamName, CurDragItem.PartnerData.HeroID, CurDragItem.IndexInTeam,
    				targetItem.TeamName, 0, targetItem.IndexInTeam);
    			    CheckTeamState(CurDragItem.TeamName,-1);
    			    CheckTeamState(targetItem.TeamName,1);
                }
                if (!targetItem.IsEmpty)
    			{
                    //EB.Log.Error("TargetItem Is not empty");
    				ReferencePartnerItem.Fill(targetItem.PartnerData);
    			}
    			targetItem.Fill(CurDragItem.PartnerData);
                CurDragItem.mDMono.gameObject.SetActive(false);
            }
        }
    
        public void CheckTeamState(string teamName, int teamChange)
        {
            int teamCount = GetTeamMemList(teamName).Count + teamChange;
            EB.Debug.Log("{0}:{1}",teamName, teamCount);
            switch (teamCount)
            {
                case 0:
                    NationManager.Instance.ChangeNationTeamStage(false, teamName, null);
                    break;
                case 1:
                    NationManager.Instance.ChangeNationTeamStage(true, teamName, null);
                    break;
            }
        }
    
    	public void OnModelDrag()
    	{
    		if (!CurDragItem.mDMono.gameObject.activeSelf)
    			return;
    
    		CurDragItem.mDMono.transform.position = new Vector3(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y, CurDragItem.mDMono.transform.position.z);
    	}
    
    	public void OnModelDragOut(NationPartnerItem dragItem, bool isNeedRequest = true)
    	{
    		dragItem.mDMono.gameObject.SetActive(false);
    		if (isNeedRequest)
    		{
    			//LTCombatHudController.ResetAutoSkill();
    			LTFormationDataManager.Instance.RequestRemoveHeroFormation(dragItem.PartnerItem.PartnerData.HeroID, dragItem.TeamName, delegate () {
    				RefreshPartnerList();
    			});
    		}
    	}
    
    	private void OnDrag2ModelPos(string teamName,int heroID, int index, bool isNeedReq,bool isAdd)
    	{
    		if (isNeedReq)
    		{
    			//LTCombatHudController.ResetAutoSkill();
    			LTFormationDataManager.Instance.RequestDragHeroToFormationPos(heroID, index, teamName, delegate () {
    				if(isAdd)
    					RefreshPartnerList();
    			});
    		}
    
    		//LTFormationDataManager.Instance.AddTeamMemData(teamMemData, GetCurrentTeamName());
    		//SmallPartnerController.instance.SortList();
    	}
    
    	private void OnDrag2ModelPos(string fromTeamName, int fromHeroID, int fromIndex, string toTeamName, int toHeroID, int toIndex)
    	{
    		LTFormationDataManager.Instance.RequestDragHeroToOtherTeam(fromHeroID, fromIndex, fromTeamName, toHeroID, toIndex, toTeamName, delegate () {
    		});
    	}
    
    	NationPartnerItem GetDragOverItem()
    	{
    		Vector2 curClickPos = new Vector2(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y);
    		for (var i = 0; i < FormationPartnerItems.Count; i++)
    		{
                var p = FormationPartnerItems[i];

                if (p.Collider.bounds.Contains(curClickPos))
    			{
    				return p;
    			}
    		}
    		return null;
    	}
    
    	//从伙伴列表中托人
    	public void OnModelDragStartByIcon(CombatPartnerCellController partnerCell)
    	{
    		var partnerData = partnerCell.ItemData;
    		if (IsInTeam(partnerData.HeroId) || !FormationUtil.IsHave(partnerData))
    		{
    			return;
    		}
    		partnerCell.OnSelect(true);
    
    		OtherPlayerPartnerData data = new OtherPlayerPartnerData();
    		data.HeroID = partnerData.HeroId;
    		data.Name= partnerData.HeroInfo.name;
    		data.Attr = partnerData.HeroInfo.char_type;
    		data.Icon = partnerData.HeroInfo.icon;
    		data.QualityLevel = partnerData.HeroInfo.role_grade;
    		data.Level = partnerData.Level;
    		data.Star = partnerData.Star;
    	    data.UpGradeId = partnerData.UpGradeId;
    		data.awakenLevel = partnerData.IsAwaken;
    		CurDragItem.Fill(data);
    		CurDragItem.mDMono.gameObject.SetActive(true);
    		CurDragItem.mDMono.transform.position = new Vector3(UICamera.lastWorldPosition.x, UICamera.lastWorldPosition.y, DragZ);
    	}
    
    	public void OnModelDragEndByIcon()
    	{
    		if (!CurDragItem.mDMono.gameObject.activeSelf)
    			return;
    
    		var dragOverItem = GetDragOverItem();
    		if (dragOverItem != null)
    		{
    			dragOverItem.Fill(CurDragItem.PartnerItem.PartnerData);
    			CurDragItem.mDMono.gameObject.SetActive(false);
    			OnDrag2ModelPos(dragOverItem.TeamName, CurDragItem.PartnerItem.PartnerData.HeroID, dragOverItem.IndexInTeam, true,true);
                CheckTeamState(dragOverItem.TeamName,1);
    		}
    		else
    		{
    			OnModelDragOut(CurDragItem, false);
    		}
        }
    
    	#endregion
    
    	bool IsInTeam(int heroID)
    	{
    		for (int teamIndex = 0; teamIndex < NationUtil.TeamNames.Length; ++teamIndex)
    		{
    			List<TeamMemberData> memList = LTFormationDataManager.Instance.GetTeamMemList(NationUtil.TeamNames[teamIndex]);
    
    			for (int i = 0; i < memList.Count; i++)
    			{
    				if (memList[i].HeroID == heroID)
    					return true;
    			}
    		}
    		return false;
    	}
    
    	List<TeamMemberData> GetTeamMemList(string teamName)
    	{
    		return LTFormationDataManager.Instance.GetTeamMemList(teamName);
    	}
    
    	bool GetCanAddHeroToFormation(string teamName,TeamMemberData memberData)
    	{
    		if (memberData.HeroID <= 0)
    		{
    			Debug.LogError("cannot AddHeroToFormation partnerData.HeroId <= 0");
    			return false;
    		}
    		//if (memberData.Star <= 0)
    		//{
    		//	Debug.LogError("cannot AddHeroToFormation partnerData.Star <= 0");
    		//	return false;
    		//}
    
    		//level limit num judge
    		if (GetTeamMemList(teamName).Count >= SmallPartnerPacketRule.TEAM_MAX_NUM)
    		{
    			int messageId;
    			if (!Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10046).IsConditionOK())
    			{
    				messageId = SmallPartnerPacketRule.ADD_FORMATION_CODE4;
    			}
    			else
    			{
    				messageId = SmallPartnerPacketRule.ADD_FORMATION_CODE1;  //上阵角色数量已达上限
    			}
    			MessageTemplateManager.ShowMessage(messageId);
    			return false;
    		}
    
    		if (IsInTeam(memberData.HeroID))
    		{
    			EB.Debug.LogError("is in team for heroid={0}", memberData.HeroID);
    			return false;
    		}
    		return true;
    	}
    
    	//[ContextMenu("AddFormationPartnerItems")]
    	//public void AddFormationPartnerItems()
    	//{
    	//	FormationPartnerItems = new List<NationPartnerItem>();
    	//	FormationPartnerItems.AddRange(controller.transform.GetComponentsInChildren<NationPartnerItem>());
    	//}
    }
}
