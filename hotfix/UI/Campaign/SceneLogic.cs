using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILRuntime.Runtime;
using Pathfinding;
using Pathfinding.RVO;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace Hotfix_LT.UI
{

    //屏蔽测试
    public enum eCombatOutcome
    {
        Win,
        Draw,
        Lose
    }

    public enum eBattleType
    {
        None = 0,
        CampaignBattle = 1,//主线副本(蛮荒)
        CityRobBattle = 2,//切磋
        PVPBattle = 3,
        Loot = 4,
        ArenaBattle = 5,//角斗场
        ExpeditionBattle = 6,//蓬莱寻宝(蛮荒)
        AllianceQualifyBattle = 7,//帮战(蛮荒)
        TreasuryBattle = 8,//金币副本
        ExpSpringBattle = 9,//经验副本
        GhostBattle = 10,//夺宝奇兵
        BossBattle = 11,//世界BOSS
        InfiniteChallenge = 12,//极限试炼
        LadderBattle = 13,//天梯
        TransferOrRob = 14,//军团护送&&军团拦截
        RedName = 15,//红名
        FastCampaignBattle = 16,
        ComboTest = 17,//连击(蛮荒)
        AllianceCampaignBattle = 18,//军团副本
        MainCampaignBattle = 19,//主线副本
        ChallengeCampaign = 20,//挑战副本 
        FirstBattle = 21,//开场大战
        AllieancePreBattle = 22,//军团战预赛
        HeroBattle = 23,//英雄交锋
        NationBattle = 24,//国战
        AllieanceFinalBattle = 25,//军团战半决与决赛
        HantBattle = 26,//幸运悬赏
        AwakeningBattle = 27,//觉醒副本
        AlienMazeBattle = 28,//异界迷宫
        SleepTower = 29,//睡梦之塔
        InfiniteCompete =30,//极限试炼进竞速模式
		HonorArena = 31,//荣耀角斗场
		BossChallenge = 32,//首领挑战
		LegionMercenary=33,//军团佣兵
	}

	/// <summary>
	/// 场景逻辑管理器
	/// </summary>
	public class SceneLogic : DynamicMonoHotfix
	{
		private static HashSet<eBattleType> stBattleTypeQuery = new HashSet<eBattleType>{
			eBattleType.CampaignBattle,
			eBattleType.TreasuryBattle,
			eBattleType.ExpSpringBattle,
			eBattleType.InfiniteChallenge,
			eBattleType.FastCampaignBattle,
			eBattleType.ComboTest,
			eBattleType.AllianceCampaignBattle,
			eBattleType.MainCampaignBattle,
			eBattleType.ChallengeCampaign,
			eBattleType.FirstBattle,
			eBattleType.GhostBattle,
			eBattleType.HantBattle,
			eBattleType.BossBattle,
			eBattleType.AlienMazeBattle,
			eBattleType.SleepTower
		};
		public enum eSceneState
		{
			NoneScene,
			RequestingScene,
			SceneLoop,
			RequestingCombatTransition,
			CombatTransition,
			CombatLoop,
			SceneTransition,
			SceneOutCome,
			DelayCombatTransition, //延迟进入
		}

		public struct PlayerEntry
		{
			public Vector3 position;
			public Quaternion rotation;
		}

		public static eBattleType BattleType;
		public static eSceneState SceneState;
		public eCombatOutcome m_CombatOutCome;

		//位置与怪物映射
		protected Dictionary<string, Hotfix_LT.UI.DungeonPlacement> m_DungeonPlacements;

		//位置与敌人映射
		protected Dictionary<string, EnemyController> m_EnemyControllers;
		public Dictionary<string, EnemyController> EnemyControllers
		{
			get
			{
				return m_EnemyControllers;
			}
		}

		protected PlayerEntry m_PlayerEntry;
		protected PlayerEntry m_PlayerSpawnEntry;

		protected SceneManager m_SparxSceneManager;

		protected ThemeLoadManager m_ThemeLoadManager;
		public ThemeLoadManager ThemeLoadManager
		{
			get { return m_ThemeLoadManager; }
		}

		protected int m_SceneId;
		public int SceneId
		{
			get
			{
				return m_SceneId;
			}
		}

		protected string m_CurrentSceneName;
		public string CurrentSceneName
		{
			get
			{
				return m_CurrentSceneName;
			}
			set
			{
				m_CurrentSceneName = value;
			}
		}

		protected string m_CurrentEnvironmentName;
		public string CurrentEnvironmentName
		{
			get
			{
				return m_CurrentEnvironmentName;
			}
		}

		// protected string m_CurrentEncounterGroup;
		// public string CurrentEncounterGroup
		// {
		// 	get
		// 	{
		// 		return m_CurrentEncounterGroup;
		// 	}
		// }

		public override void OnDestroy()
		{
			StopAllCoroutines();
		}

		public void Initialize()
		{
			m_DungeonPlacements = new Dictionary<string, Hotfix_LT.UI.DungeonPlacement>();
			m_EnemyControllers = new Dictionary<string, EnemyController>();
			m_PlayerEntry = new PlayerEntry();
			m_PlayerSpawnEntry = new PlayerEntry();
			SceneState = eSceneState.NoneScene;
			m_CurrentSceneName = string.Empty;
			m_SparxSceneManager = LTHotfixManager.GetManager<SceneManager>();
			mTaskDelegates = new Dictionary<string, TaskDelegate>();
		}

		public static int GetBattleType()
        {
			return (int)SceneLogic.BattleType;
		}

		public static void StartCombatFromMain()
        {
			if (SceneLogic.isPVP())
			{
				LTCombatHudController.Instance.controller.GetComponent<UIPanel>().alpha = 0.1f;
			}
		}
		public static bool NeedScale()
        {
			return stBattleTypeQuery.Contains(SceneLogic.BattleType);
		}

		protected void StartScene()
		{
			StartCoroutine(SafeStartScene());
		}

		private IEnumerator SafeStartScene()
		{
			FuncNpcLoadCompleted = false;
			yield return new WaitForEndOfFrame();
			while (GetHeroStartLocator() == null)
			{
				EB.Debug.LogWarning("Waiting for hero start locator ready!!!");
				yield return null;
			}
			GameObject locator = GetHeroStartLocator();
			if (locator != null)
			{
				m_PlayerSpawnEntry.position = locator.transform.position;
				m_PlayerSpawnEntry.rotation = locator.transform.rotation;

				if (m_PlayerEntry.position.Equals(Vector3.zero))
				{
					m_PlayerEntry.position = m_PlayerSpawnEntry.position;
					m_PlayerEntry.rotation = m_PlayerSpawnEntry.rotation;
				}
			}
			SceneState = eSceneState.SceneLoop;
			InitSceneObjectRegistry();
			m_SparxSceneManager.StartCombatFromQueue();
			yield break;
		}

		public GameObject GetHeroStartLocator()
		{
			GameObject locator = null;
			string sceneType = SceneLogicManager.getMultyPlayerSceneType();
			if (string.IsNullOrEmpty(sceneType)) return LocatorManager.Instance.GetHeroStartLocator();
			if (sceneType.Equals(SceneLogicManager.SCENE_MAINLAND_TYPE))
			{
				string mainlandName = MainLandLogic.GetInstance().CurrentSceneName;
				var mainlandTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetMainLand(mainlandName);
				if (mainlandTpl != null)
				{
					locator = LocatorManager.Instance.GetLocator(mainlandTpl.born_locator);
				}
			}
			if (locator == null) locator = LocatorManager.Instance.GetHeroStartLocator();
			return locator;
		}

		public EnemyController GetEnemyController(string locator)
		{
			if (m_EnemyControllers != null && m_EnemyControllers.ContainsKey(locator)) return m_EnemyControllers[locator];
			return null;
		}

		public virtual void RegisterDataID()
		{

		}

		public virtual void UnRegisterDataID()
		{

		}

		protected void DoSceneTransitionFinished()
		{
			if (IsSceneViewUnloadedOnTransition())
			{
				InitSceneObjectRegistry();
			}

			m_CombatOutCome = eCombatOutcome.Draw;
			if (SceneState == eSceneState.SceneTransition)
			{
				SceneState = eSceneState.SceneLoop;
			}
			m_SparxSceneManager.StartCombatFromQueue();
		}

		public void OnSceneViewLoaded()
		{
			SceneRootEntry entry = SceneLoadManager.GetSceneRoot(SceneLoadManager.CurrentSceneName);
			if (entry == null)
			{
				
				EB.Debug.LogError("OnSceneViewLoaded entry is null");
				return;
			}
			BattleType = eBattleType.None;
			m_ThemeLoadManager = entry.GetComponentInChildren<ThemeLoadManager>();
			SceneTriggerManager.Instance.InitTrigger(m_CurrentSceneName);
			if (SceneState <= SceneLogic.eSceneState.RequestingScene)
			{
				StartScene();
			}
			else
			{
				DoSceneTransitionFinished();
			}
		}

		protected virtual void GoToSceneViewInFSM()
		{

		}

		public void OnSceneEnter(Hashtable campaign)
		{
			if (null == campaign) return;
			PlayerManagerForFilter.Instance.StopShowPlayer();
			ClearSceneState();
			m_SceneId = EB.Dot.Integer("id", campaign, 0);
			m_CurrentEnvironmentName = EB.Dot.String("map_name", campaign, string.Empty);

			if (campaign.ContainsKey("name"))
			{
				m_CurrentSceneName = EB.Dot.String("name", campaign, string.Empty);
			}
			else
			{
				EB.Debug.LogError("Wrong  session data format: no  environment name!");
			}
			RegisterPlacementData(campaign);
			GoToSceneViewInFSM();
		}

		void RegisterPlacementData(Hashtable campaign_data)
		{
			RegisterNpcPlacementData(campaign_data);
			RegisterOtherPlayersPlacementData(campaign_data);
		}

		void PlacementDataClear()
		{
			m_DungeonPlacements.Clear();
		}

		void RegisterOtherPlayersPlacementData(Hashtable scene_data)
		{
			if (scene_data == null)
			{
				EB.Debug.LogError("Campaign data is null in campaign session!");
				return;
			}

			if (!scene_data.Contains("pl"))
			{
				EB.Debug.LogError(" session data format error: no data.placements field!");
				return;
			}

			Hashtable placements = scene_data["pl"] as Hashtable;
			if (placements == null) return;
			IDictionaryEnumerator enumerator = placements.GetEnumerator();

			while (enumerator.MoveNext())
			{
				object value = enumerator.Value;
				long userid = 0;
				if (!long.TryParse(enumerator.Key.ToString(), out userid)) continue;
				if (userid == LoginManager.Instance.LocalUserId.Value)
				{
					if (value is Hashtable)
					{
						Hashtable data = value as Hashtable;
						string pos_str = EB.Dot.String("pos", data, "");
						if (pos_str.Equals("")) continue;
						m_PlayerEntry.position = GM.LitJsonExtension.ImportVector3(pos_str);
					}
					else
					{
						EB.Debug.LogWarning("Wrong json format for dungeon placement data.");
					}
					continue;
				}
			}
		}

		void SetupHantMonster()
		{
			Hashtable Tasks = null;
			DataLookupsCache.Instance.SearchDataByID<Hashtable>("tasks", out Tasks);
            if (Tasks != null)
            {
                foreach (DictionaryEntry de in Tasks)
                {
                    IDictionary task = de.Value as IDictionary;
                    if (task == null)
                    {
                        EB.Debug.LogError("Scenelogic.SetupHantMonster task == null");
                        return;
                    }
                    else if (!task.Contains("task_type") || task["task_type"] == null)
                    {
                        EB.Debug.LogError("Scenelogic.SetupHantMonster task_type == null");
                        return;
                    }
                    eTaskType task_type = (eTaskType)System.Enum.Parse(typeof(eTaskType), task["task_type"].ToString());
                    string state = EB.Dot.String("state", task, TaskSystem.RUNNING);
                    LTBountyTaskHudController.DeleteMonster();
                    if (task_type == eTaskType.Hant && state == TaskSystem.RUNNING)
                    {
                        LTBountyTaskHudController.SetupHantMonster();
                    }
                }
            }
		}

		void RegisterPlayersTaskMonster()
		{
			Hashtable Tasks = null;
			DataLookupsCache.Instance.SearchDataByID<Hashtable>("tasks", out Tasks);
			if (Tasks == null) return;
			foreach (DictionaryEntry de in Tasks)
			{
				IDictionary task = de.Value as IDictionary;
				// eTaskType task_type = Hotfix_LT.EBCore.Dot.Enum<eTaskType>("task_type", task, eTaskType.Main);
				eTaskType task_type =(eTaskType) System.Enum.Parse(typeof(eTaskType), task["task_type"].ToString());
				string state = EB.Dot.String("state", task, TaskSystem.RUNNING);
				string task_id = de.Key.ToString();
				Hotfix_LT.Data.TaskTemplate ttp = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(int.Parse(task_id));
				if (ttp == null) continue;
				eTaskTargetType taget_type = (eTaskTargetType)ttp.target_type;
				if (state == TaskSystem.RUNNING)
				{
					if (task_type == eTaskType.Hant) RegisterOneTaskMonster(task_id);
					else if (task_type == eTaskType.Main && taget_type == eTaskTargetType.BeatMonster) RegisterOneTaskMonster(task_id);
				}
			}
		}


		void RegisterOneTaskMonster(string taskid)
		{
			Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskid);
			if (taskTpl == null)
			{
				EB.Debug.LogWarning("No such task in table!!!");
				return;
			}
			string task_npc = taskTpl.target_parameter_2;
			string task_scene = taskTpl.target_parameter_1;
			if (!task_scene.Equals(CurrentSceneName))
			{
				return;
			}
			var npcdata = Hotfix_LT.Data.SceneTemplateManager.GetMainLandsNPCData(task_scene, task_npc);
			if (npcdata == null) return;
            Hotfix_LT.UI.DungeonPlacement placement = new Hotfix_LT.UI.DungeonPlacement(npcdata); 

            if (placement == null)
            {
                return;
            }
            m_DungeonPlacements.Add(placement.Locator, placement);
        }

		/// <summary>
		/// 用于在接到任务是放置这个怪物  如果当前不是这个怪物需要出现的场景 则跳迿
		/// </summary>
		public void PlaceOneTaskMonster(string taskid)
		{
			Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskid);
			if (taskTpl == null)
			{
				EB.Debug.LogWarning("No such task in table!!!");
				return;
			}

			string task_npc = taskTpl.target_parameter_2;
			string task_scene = taskTpl.target_parameter_1;
			int task_type = taskTpl.task_type;
			int taget_type = taskTpl.target_type;
			if ((eTaskType)task_type != eTaskType.Hant &&
				!((eTaskType)task_type == eTaskType.Main && (eTaskTargetType)taget_type == eTaskTargetType.BeatMonster)) return;
			if (!task_scene.Equals(CurrentSceneName)) return;

			//根据这个npc找到怪物属怿 将这个怪物建立起来
			var npcdata = Hotfix_LT.Data.SceneTemplateManager.GetMainLandsNPCData(task_scene, task_npc);
			if (npcdata == null) return;

            Hotfix_LT.UI.DungeonPlacement placement = new Hotfix_LT.UI.DungeonPlacement(npcdata); 

            if (placement == null) return;
            m_DungeonPlacements.Add(placement.Locator, placement);
            StartCoroutine(SafePlaceOneTaskHantMonster(placement));
        }

		/// <summary>
		/// 用于在接到任务是放置这个怪物  如果当前不是这个怪物需要出现的场景 则跳迿
		/// </summary>
		public void PlaceOneTaskMonster(string task_scene, string task_npc, string model_name, System.Action loadComplete)
		{
			if (!task_scene.Equals(CurrentSceneName)) return;

			//根据这个npc找到怪物属怿 将这个怪物建立起来
			var npcdata = Hotfix_LT.Data.SceneTemplateManager.GetMainLandsNPCData(task_scene, task_npc);
			if (npcdata == null)
				return;

			GlobalMenuManager.Instance.ShowMainLandHandler(true);

            Hotfix_LT.UI.DungeonPlacement placement = new Hotfix_LT.UI.DungeonPlacement(npcdata); 

            placement.Encounter = model_name.Replace("-Variant", "") + "_NPC_Template";
            m_DungeonPlacements.Add(placement.Locator, placement);
            StartCoroutine(SafePlaceOneTaskHantMonster(placement, loadComplete));
        }

		public bool IsBusy(string npc_id)
		{
			if (string.IsNullOrEmpty(npc_id)) return false;
			bool busy = false;
			DataLookupsCache.Instance.SearchDataByID<bool>("mainlands.npc_list." + npc_id + ".busy", out busy);
			return busy;
		}

		public bool IsGhostMonster(string npc_id)
		{
			if (string.IsNullOrEmpty(npc_id)) return false;
			bool is_ghost = false;
			DataLookupsCache.Instance.SearchDataByID<bool>("mainlands.npc_list." + npc_id + ".is_ghost", out is_ghost);
			return is_ghost;
		}

		public bool IsHantMonster(string npc_id)
		{
			if (string.IsNullOrEmpty(npc_id)) return false;

			IDictionary tasks = null;
			DataLookupsCache.Instance.SearchDataByID<IDictionary>("tasks", out tasks);
			if (tasks == null)
			{
				return false;
			}

			foreach (DictionaryEntry de in tasks)
			{
				IDictionary task = de.Value as IDictionary;
				// eTaskType type = Hotfix_LT.EBCore.Dot.Enum<eTaskType>("task_type", task.Value, eTaskType.None);
				eTaskType type =(eTaskType) System.Enum.Parse(typeof(eTaskType), task["task_type"].ToString());
				if (eTaskType.Hant != type) continue;

				string task_npc = EB.Dot.String("event_count.monster_id", task, "");
				string task_scene = EB.Dot.String("event_count.scene_name", task, "");
				if (npc_id.Equals(task_npc) && task_scene.Equals(CurrentSceneName)) return true;
			}
			return false;
		}

		private IEnumerator SafePlaceOneTaskHantMonster(Hotfix_LT.UI.DungeonPlacement placement, System.Action loadComplete = null)
		{

			yield return SpawnPlacementAsync(placement);

			loadComplete?.Invoke();
		}

		void RegisterNpcPlacementData(Hashtable campaign_data)
		{
			if (campaign_data == null)
			{
				EB.Debug.LogError("Campaign data is null in campaign session!");
				return;
			}

			if (!campaign_data.Contains("npc_list"))
			{
				EB.Debug.LogError("Campaign session data format error: no data.placements field!");
				return;
			}

			if (!(campaign_data["npc_list"] is Hashtable))
			{
				EB.Debug.LogError("Campaign session data format error: data.placements field is not hashtable!");
				return;
			}

			Hashtable placements = campaign_data["npc_list"] as Hashtable;
			if (placements == null)
			{
				return;
			}
			try
			{
				IDictionaryEnumerator enumerator = placements.GetEnumerator();
				while (enumerator.MoveNext())
				{
					object value = enumerator.Value;
					if (value is Hashtable)
					{
						Hotfix_LT.UI.DungeonPlacement placement = new Hotfix_LT.UI.DungeonPlacement(this.CurrentSceneName, enumerator.Key.ToString(), value as Hashtable);
						if (placement == null)
						{
							continue;
						}
						if (placement.Role == NPC_ROLE.WORLD_BOSS && string.IsNullOrEmpty(placement.Encounter))//当BOSS死亡而活动未结束时
						{
							continue;
						}
						if (string.IsNullOrEmpty(placement.Encounter))//mainland不知道有什么用处，后面排序会报错暂时屏蔽
						{
							continue;
						}
						m_DungeonPlacements.Add(placement.Locator, placement);
					}
					else
					{
						EB.Debug.LogWarning("Wrong json format for dungeon placement data.");
					}
				}
			}
			catch(NullReferenceException e)
            {
				EB.Debug.LogError(e.ToString());
			}

			RegisterPlayersTaskMonster();
		}

		public void OnSceneTransitionResponse(Hashtable result)
		{
			if (null == result)
				return;
				
			Hashtable data = result as Hashtable;
			BattleType = (eBattleType)EB.Dot.Integer("combatType", data, (int)BattleType);
			if (data.Contains("isCombatWon"))
			{
				bool is_win = false;
				if (!bool.TryParse(data["isCombatWon"].ToString(), out is_win))
				{
					EB.Debug.LogError("error format data for campaign transition data: isWon");
				}
				m_CombatOutCome = is_win ? eCombatOutcome.Win : eCombatOutcome.Lose;
			}
			else
			{
				m_CombatOutCome = eCombatOutcome.Lose;
			}

			if (SceneState == eSceneState.CombatLoop)
			{
				SceneState = eSceneState.SceneTransition;
			}

			bool is_finished = false;
			if (data.Contains("isCampaignFinished"))
			{
				if (!bool.TryParse(data["isCampaignFinished"].ToString(), out is_finished))
				{
					EB.Debug.LogError("error format data for campaign transition data: isCampaignFinished");
				}
				int buffId = 0;
				DataLookupsCache.Instance.SearchDataByID("combat.buff.spinId", out buffId);
				if (is_finished || buffId > 0)
				{
					DoLogicBeforeRating(data, buffId);
					return;
				}
			}
			OnSceneTransitionOver(is_finished);
		}

		/// <summary>
		/// 结算界面之前的逻辑
		/// </summary>
		/// <param name="data"></param>
		private void DoLogicBeforeRating<T>(T data, int buffId)
		{
			if (buffId > 0)
				PlayBuff(buffId);
			else
				PlayCampaignEndDialogue(data);
		}

		private void PlayBuff(int buffId)
		{
			if (buffId > 0)
			{
				DataLookupsCache.Instance.CacheData("combat.buff.spinId", 0);
				var buffs = AlliancesManager.Instance.Config.GetAllBuff();
				int[] buffIds = buffs.Keys.ToArray<int>();
				List<int> tempDatas = new List<int>();
				for (int i = 0; i < 7; ++i)
				{
					int index = Random.Range(0, buffs.Count);
					tempDatas.Add(buffIds[index]);
				}
				int gotIndex = Random.Range(0, 8);
				tempDatas.Insert(gotIndex, buffId);
				System.Action action = delegate ()
				{
					OnEndDialogueOver();
				};

				var ht = Johny.HashtablePool.Claim();
				ht.Add("itemDatas", tempDatas );
				ht.Add("gotIndex", gotIndex);
				ht.Add("isBuff",true);
				ht.Add("endAction",action);
				GlobalMenuManager.Instance.Open("AllianceLotteryUI", ht);
			}
		}

		private void PlayCampaignEndDialogue<T>(T data)
		{
			string name = EB.Dot.String("campaigns.name", data, "");
			if (string.IsNullOrEmpty(name))
			{
				OnEndDialogueOver();
				return;
			}

			var campTpl = Hotfix_LT.Data.SceneTemplateManager.Instance.GetCampaign(name);
			if (campTpl != null)
			{
				int dialogue_id = campTpl.end_dialogue;
				if (dialogue_id != 0)
				{
					DialoguePlayUtil.Instance.Play(dialogue_id, OnEndDialogueOver);
					return;
				}
			}
			OnEndDialogueOver();
		}

		private void OnEndDialogueOver()
		{
			OnSceneTransitionOver(true);
		}

		private void OnSceneTransitionOver(bool isfinished)
		{
			Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.ShowBattleResultScreen, BattleType, m_CombatOutCome, isfinished);


			if (Camera.main != null)
			{
				CombatCamera combat_camera = Camera.main.GetComponent<CombatCamera>();

				if (combat_camera != null)
				{
					combat_camera.EnableBlurEffect();
				}
			}
		}

		public void RequestCombatTransition(eBattleType battletype, Vector3 pos, Quaternion rot, List<EnemyController> enemies)
		{
			if (SceneState != eSceneState.CombatTransition || SceneState != eSceneState.RequestingCombatTransition)
			{
				m_PlayerEntry.position = pos;
				m_PlayerEntry.rotation = rot;

				List<string> combat_list = new List<string>();
				if (enemies != null)
				{
					for (int i = 0; i < enemies.Count; i++)
					{
						if (m_EnemyControllers.ContainsKey(enemies[i].name))
						{
							combat_list.Add(enemies[i].name);
						}
					}
				}
				SceneState = eSceneState.RequestingCombatTransition;
				Vector3 euler_angles = rot.eulerAngles;
				BattleType = battletype;
				m_SparxSceneManager.RequestCombatTransition(battletype, m_SceneId, pos, euler_angles.y, combat_list, OnRequestCombatTransition);
			}
		}

		public void RequestFastCombatTransition(eBattleType battletype, string target)
		{
			if (SceneState != eSceneState.CombatTransition || SceneState != eSceneState.RequestingCombatTransition)
			{
				SceneState = eSceneState.RequestingCombatTransition;
				m_SparxSceneManager.RequestFastCombatTransition(battletype, target, OnRequestCombatTransition);
			}
		}

		public void RequestBountyTaskCombatTransition()
		{
			if (SceneState != eSceneState.CombatTransition || SceneState != eSceneState.RequestingCombatTransition)
			{
				SceneState = eSceneState.RequestingCombatTransition;
				m_SparxSceneManager.RequestBountyTaskCombat(OnRequestCombatTransition);
			}
		}

		/// <summary>
		/// 断线重连恢复战斗
		/// </summary>
		public void RequestResumeCombat(int combat_session_id)
		{
			Combat.CombatInfoData.GetInstance().LogResumeCombat(combat_session_id);

			if (combat_session_id < 0)
			{
				return;
			}

			SceneState = eSceneState.RequestingCombatTransition;
			//重连战斗
			m_SparxSceneManager.ResumeCombat(combat_session_id, result => { });
		}

		public void OnRequestCombatTransition(EB.Sparx.Response result)
		{
			if (result.sucessful)
			{
				PlayerManagerForFilter.Instance.StopShowPlayer();
				EB.Debug.Log("going to combat momentarily");
				DataLookupsCache.Instance.CacheData(result.hashtable);
				eSceneState tempState = SceneState;
				ClearSceneState();
				SceneState = tempState;
			}
			else if (result.fatal)
			{
				if (result.error.Equals("boss dead"))
				{
					MessageTemplateManager.ShowMessage(902090);
					return;
				}

				SparxHub.Instance.FatalError(result.localizedError);
				BattleType = eBattleType.None;
			}
			else
			{
				SceneState = eSceneState.SceneLoop;
				MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"), result.localizedError, EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, delegate (int ret)
				{
					UIStack.Instance.HideLoadingScreen();
					if (BattleType == eBattleType.GhostBattle ||
						BattleType == eBattleType.HantBattle ||
						BattleType == eBattleType.BossBattle ||
						BattleType == eBattleType.RedName ||
						BattleType == eBattleType.TransferOrRob)
					{
						Debug.LogError("此处Queue已屏蔽，出现该错误时再排查...");
					}
					BattleType = eBattleType.None;
				}, NGUIText.Alignment.Center);
			}
		}

		/// <summary>
		/// 服务器拉进战斗
		/// </summary>
		/// <param name="transition_data"></param>
		public void OnCombatTransition(object transition_data)
		{
			SceneState = eSceneState.CombatTransition;
			if (transition_data == null)
			{
				EB.Debug.LogError("Combat transition data is null!!!");
				return;
			}
			if (IsSceneViewUnloadedOnTransition())
			{
				//need to clear enemy controller registry
				ResetEnemyRegistry();
			}
			if(transition_data is IDictionary){
				CombatLogic.Instance.SetCombatDataFromCampaign(transition_data as IDictionary);
			}
			SendGameFlowEvent("GoToCombatView");
		}

		// public static void GoToQuickCombatView()
		// {
		// 	if (GameFlowControlManager.Instance != null)
		// 	{
		// 		SceneState = eSceneState.RequestingCombatTransition;
  //               GameFlowControlManager.Instance.SendEvent("GoToQuickCombatView");
		// 	}
		// }

		public void OnCombatViewLoaded()
		{
			DoCombatTransition();
		}

		private bool IsSceneViewUnloadedOnTransition()
		{
			return true;
		}

		protected void DoCombatTransition()
		{
			SceneState = eSceneState.CombatLoop;
			//停掉，选英雄界面 延迟进战斗场景的协程判断
			m_SparxSceneManager.StopDoCombatQueue();
		}

		public void SendGameFlowEvent(string event_name)
		{
			if (GameFlowControlManager.Instance != null)
			{
                GameFlowControlManager.Instance.SendEvent(event_name);
			}
		}

		void InitSceneObjectRegistry()
		{
			SceneTriggerManager.Instance.ActiveTrigger();
			InitializeLocalPlayer(m_PlayerEntry.position, m_PlayerEntry.rotation);
			PlayerManagerForFilter.Instance.BeginShowPlayer();
			DoPlacementRegistry();
		}

		public Coroutine PreloadAsync(long userid, System.Action<bool> callback)
		{
			string characterClass = BuddyAttributesManager.GetModelClass(userid.ToString());
			if (string.IsNullOrEmpty(characterClass))
			{
				EB.Debug.LogError("PreloadAsync ModelClassIsNullOrEmpty for userid= {0}", userid);
				callback(false);
				return null;
			}
			return PreloadAsync(characterClass, callback);
		}

		public Coroutine PreloadAsync(string characterClass, System.Action<bool> callback)
		{
			if (string.IsNullOrEmpty(characterClass))
			{
				EB.Debug.LogError("PreloadAsync ModelClassIsNullOrEmpty for characterClass= {0}", characterClass);
				callback(false);
				return null;
			}

			var model = CharacterCatalog.Instance.GetModel(characterClass);
			if (model == null)
			{
				EB.Debug.LogError("PreloadAsync model not found for characterClass = {0}", characterClass);
				callback(false);
				return null;
			}

			model.Preload();
			string prefabName = model.PrefabNameFromGenderMain(eGender.Male);
			return PoolModel.PreloadAsync(prefabName, delegate (UnityEngine.Object o)
			{
				callback(o != null);
			});
		}

		public void CreateOtherPlayer(Vector3 pos, Quaternion rot, long userid)
			=> OtherPlayerUtils.CreateOtherPlayer(m_PlayerSpawnEntry, m_ThemeLoadManager, pos, rot, userid);

		public void InitializeLocalPlayer(Vector3 pos, Quaternion rot)
		{
			SceneRootEntry sceneRoot = m_ThemeLoadManager.GetSceneRoot();

			#region InitCameraComponent
			PlayerCameraComponent cameraComponent = sceneRoot.GetComponentInChildren<PlayerCameraComponent>();
			if (cameraComponent != null && cameraComponent.transform.GetMonoILRComponent<OpeningAndClosingSequence>(false) == null)
			{
				SceneLoadConfig config = SceneLoadManager.GetSceneLoadConfig(SceneLoadManager.CurrentStateName);
				uint mask = config.GetHideLayerMask();
				Camera cam = cameraComponent.GetComponent<Camera>();
				cam.cullingMask = cam.cullingMask & (int)mask;
				cameraComponent.gameObject.AddMonoILRComponent<OpeningAndClosingSequence>("Hotfix_LT.UI.OpeningAndClosingSequence"); // THIS MUST BE SET-UP BEFORE THE PLAYER
			}
			#endregion	

			#region InitPlayerList
			Transform PlayerList = sceneRoot.m_SceneRoot.transform.Find("PlayerList");
			if (PlayerList == null)
			{
				GameObject partner = new GameObject("PlayerList");
				partner.transform.SetParent(sceneRoot.m_SceneRoot.transform);
				PlayerList = partner.transform;
			}
			#endregion

			#region Load Async My Player
			EB.Assets.LoadAsync("Bundles/Prefab/Player", typeof(GameObject), o =>
			{
				if(!o)
				{
					return;
				}

				PlayerList.gameObject.CustomSetActive(true);
				
				GameObject localObj = Replication.Instantiate(o, pos, rot) as GameObject;
				localObj.transform.SetParent(PlayerList);

				PlayerController player_controller = localObj.GetComponent<PlayerController>();
				player_controller.playerUid = LoginManager.Instance.LocalUserId.Value;
				Player.PlayerHotfixController hotfix_controller = player_controller.transform.GetMonoILRComponent<Player.PlayerHotfixController>();
                hotfix_controller.SetPlayerSpawnLocation(pos);
				hotfix_controller.InitDataLookupSet();

				cameraComponent.TargetNormal = localObj.transform;
				cameraComponent.TargetLeft = localObj.transform.Find("AnchorLeft");
				cameraComponent.TargetRight = localObj.transform.Find("AnchorRight");
				cameraComponent.SetInitialCameraPosition();
				cameraComponent.SetZoomDistance(1.5f);

				// Add a SelectionLogic component to allow input
				localObj.AddComponent<SelectionLogic>();

				//forbidden player controll self after transfer
				if (GuideManager.Instance.IsNeedToDisableNavigation())
				{
					PlayerController.LocalPlayerDisableNavigation();
				}
				//ControllerInput.Create();
				EB.Debug.Log("Player transform : " + cameraComponent.transform.localPosition);
				FusionAudio.InitLocalPlayer();
			});
            #endregion
        }

		private IEnumerator cor;
        private void DoPlacementRegistry()
		{
			SceneRootEntry sceneRoot = m_ThemeLoadManager.GetSceneRoot();
			if (sceneRoot != null)
			{
				InteractableObjectManager.Instance.SetParent(sceneRoot.m_SceneRoot.transform);
			}

			if (cor!=null)
			{
				StopCoroutine(cor);
			}
			cor=SafeSpawnPlacements();
			StartCoroutine(cor);
		}

		private void ResetEnemyRegistry()
		{
			//将场景里边的动态对象都回收
			DestroyDynamicGameObjects();
			//
			if (cor!=null)StopCoroutine(cor);
			// StopCoroutine(SafeSpawnPlacements());
			m_EnemyControllers.Clear();
		}

		public bool IsValidEnemy(EnemyController ec)
		{

			if (ec == null)
			{
				return false;
			}

			if (!m_EnemyControllers.ContainsKey(ec.name))
			{
				return false;
			}

			if (m_DungeonPlacements.ContainsKey(ec.name))
			{
				return true;
			}

			return false;
		}

		public static bool FuncNpcLoadCompleted
		{
			get; private set;
		}

		private IEnumerator SafeSpawnPlacements()
		{
			System.DateTime sceneStart = System.DateTime.Now;
			FuncNpcLoadCompleted = false;
			string scenetype = SceneLogicManager.getSceneType();

			List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>> list = new List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>>(m_DungeonPlacements);
			list.Sort(delegate (KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement> x, KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement> y)
			{
				return x.Value.Encounter.CompareTo(y.Value.Encounter);
			});

			List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>> FuncNpcList = new List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>>();
			List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>> OthercNpcList = new List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>>();
			for (int i = 0; i < list.Count; i++)
			{
				if (SceneState != SceneLogic.eSceneState.SceneLoop && SceneState != SceneLogic.eSceneState.SceneTransition) yield break;
				string locator = list[i].Value.Locator;
				if (!NpcManager.Instance.IsNpcManaged(locator))
				{
					if (list[i].Value.Role.CompareTo("guard") == 0 || list[i].Value.Role.CompareTo("ghost") == 0)
					{
						OthercNpcList.Add(list[i]);
					}
					else
					{
						FuncNpcList.Add(list[i]);
					}
				}
			}
			mTaskDelegates = new Dictionary<string, TaskDelegate>();
			if (this is MainLandLogic)
			{
				SpawnTasksDelegate();
			}
			yield return SpawnFuncNpcPlacementAsync(FuncNpcList, () =>
			{
				WorldMapPathManager.Instance.Process();

			});
			OnNpcSpawnFinish();
			FuncNpcLoadCompleted = true;

			for (int i = 0; i < OthercNpcList.Count; i++)
			{
				//yield return null;
				if (SceneState != SceneLogic.eSceneState.SceneLoop)
				{
					yield break;
				}
				NpcDelegate npc = new NpcDelegate(OthercNpcList[i].Value, scenetype + m_SceneId.ToString());
				NpcManager.Instance.NpcCommIn(npc);
			}

			SetupHantMonster();

			System.DateTime sceneEnd = System.DateTime.Now;
			EB.Assets.LoadCostLog("SafeSpawnPlacements", (sceneEnd - sceneStart).TotalMilliseconds);

			yield break;
		}

		private void removeOtherComponent(Player.EnemyHotfixController ec)
		{
			if (ec.Role == NPC_ROLE.WORLD_BOSS ||ec.Role == NPC_ROLE.FUNC ||ec.Role == NPC_ROLE.GUARD ||ec.Role == NPC_ROLE.ARENA_MODLE )
			{
				var rvoController = ec.mDMono.GetComponent<RVOController>();
				if (rvoController != null)
				{
                    Object.Destroy(rvoController);
					rvoController = null;
				}
				var selectable = ec.mDMono.GetComponent<Selectable>();
				if (selectable != null)
				{
					Object.Destroy(selectable);
					selectable = null;
				}
				var combatController = ec.mDMono.GetComponent<CombatController>();
				if (combatController != null)
				{
					Object.Destroy(combatController);
					combatController = null;
				}
				var locomotionComponentAPP = ec.mDMono.GetComponent<LocomotionComponentAPP>();
				if (locomotionComponentAPP != null)
				{
					Object.Destroy(locomotionComponentAPP);
					locomotionComponentAPP = null;
				}
				var funnelModifier = ec.mDMono.GetComponent<FunnelModifier>();
				if (funnelModifier != null)
				{
					Object.Destroy(funnelModifier);
					funnelModifier = null;
				}
				var characterTargetingComponent = ec.mDMono.GetComponent<CharacterTargetingComponent>();
				if (characterTargetingComponent != null)
				{
					Object.Destroy(characterTargetingComponent);
					characterTargetingComponent = null;
				}
				var networkLocomotionComponent = ec.mDMono.GetComponent<NetworkLocomotionComponent>();
				if (networkLocomotionComponent != null)
				{
					Object.Destroy(networkLocomotionComponent);
					networkLocomotionComponent = null;
				}
				var networkOwnershipComponent = ec.mDMono.GetComponent<NetworkOwnershipComponent>();
				if (networkOwnershipComponent != null)
				{
					Object.Destroy(networkOwnershipComponent);
					networkOwnershipComponent = null;
				}
				var playMakerFSM = ec.mDMono.GetComponent<PlayMakerFSM>();
				if (playMakerFSM != null)
				{
					Object.Destroy(playMakerFSM);
					playMakerFSM = null;
				}
				var campaignMoveController = ec.mDMono.GetComponent<CampaignMoveController>();
				if (campaignMoveController != null)
				{
					Object.Destroy(campaignMoveController);
					campaignMoveController = null;
				}
				var replications = ec.mDMono.GetComponents<ReplicationView>();
				if (replications != null)
				{
					for (int i = 0; i < replications.Length; i++)
					{
						Object.Destroy(replications[i]);
						replications[i] = null;
					}
					replications = null;
				}
			}
		}

		#region SpawnNpcAsync
		/// <summary>
		/// 异步加载主城景里的NPC
		/// </summary>
		/// <param name="FuncNpcList">NPC列表</param>
		/// <param name="callback">所有NPC加载完成</param>
		/// <returns></returns>
		public IEnumerator SpawnFuncNpcPlacementAsync(List<KeyValuePair<string, Hotfix_LT.UI.DungeonPlacement>> FuncNpcList, System.Action callback)
		{
			System.DateTime start = System.DateTime.Now;
			string scenetype = SceneLogicManager.getSceneType();
			int index = FuncNpcList.Count - 1;
			EB.Debug.Log("SpawnFuncNpcPlacementAsync start:time = {0}", Time.time);

			int loadCount = 0;
			int finishLoadCount = 0;
			while (index >= 0)
			{
				if (SceneState != SceneLogic.eSceneState.SceneLoop && SceneState != SceneLogic.eSceneState.SceneTransition)
				{
					callback();
					yield break;
				}

				Hotfix_LT.UI.DungeonPlacement npcdata = FuncNpcList[index].Value;
				GameObject locator_obj = LocatorManager.Instance.GetLocator(npcdata.Locator);
				if (locator_obj == null)
				{
					EB.Debug.LogWarning(string.Format("No available locator data for npc  at locator [{0}]", npcdata.Locator));
					index--;
					continue;
				}
				Vector3 v3 = locator_obj.transform.position;
				Quaternion rot = locator_obj.transform.rotation;

				if (!NpcManager.Instance.IsNpcManaged(npcdata.Locator))
				{
					EB.Debug.Log("start load npc start:time = {0}, name = {1}", Time.time, npcdata.Encounter);
					yield return PreloadAsync(npcdata.Encounter, (success) =>
					{
						EB.Debug.Log("end load npc start:time = {0}, name = {1}", Time.time, npcdata.Encounter);
						if (SceneState != SceneLogic.eSceneState.SceneLoop && SceneState != SceneLogic.eSceneState.SceneTransition)
						{
							return;
						}
						#region 异步加载一个NPC
						loadCount++;
						SceneLogic scene = MainLandLogic.GetInstance();
						scene.SpawnNpcAsync(npcdata.Locator, npcdata.Encounter, npcdata.Role, v3, rot, ec=>
						{
							finishLoadCount++;
							if (ec != null)
							{
                                Player.EnemyHotfixController ehc = ec.transform.GetMonoILRComponent<Player.EnemyHotfixController>();
                                NpcDelegate npc = new NpcDelegate(npcdata, scenetype + m_SceneId.ToString());
								NpcManager.Instance.RegisterNpc(npcdata.Locator, npc);
                                ehc.Role = npcdata.Role;
								removeOtherComponent(ehc);
                                ehc.Attr = npc.attr;
                                ehc.SetNpcName(scene.CurrentSceneName);
								if (!IsAppearingNpc(npcdata))
								{
                                    ehc.DisAppearBody();
								}
								if (mTaskDelegates.ContainsKey(CurrentSceneName + npcdata.Locator))
								{
									SpawnOneTask(mTaskDelegates[CurrentSceneName + npcdata.Locator].taskid, mTaskDelegates[CurrentSceneName + npcdata.Locator].sceneid, npcdata.Locator);
								}
								if (ehc.Role == NPC_ROLE.ARENA_MODLE)
								{
									ArenaModelTidDataLookup OPTD = ec.transform.GetDataLookupILRComponent<ArenaModelTidDataLookup>(false);

									if (OPTD == null)
									{
										OPTD = ec.gameObject.AddDataLookupILRComponent<ArenaModelTidDataLookup>("Hotfix_LT.UI.ArenaModelTidDataLookup");
										string tid;
										DataLookupsCache.Instance.SearchDataByID<string>(ArenaManager.ArenaModelDataId + ".templateId", out tid);//
										tid = (tid == null || tid.CompareTo("") == 0) ? "15011" : tid;

										string path = ArenaManager.ArenaModelDataId + ".ts";
										OPTD.Tid = tid;
										DataLookupsCache.Instance.SearchDataByID<string>(path, out OPTD.ResetTime);
										OPTD.mDL.DefaultDataID = path;
									}
								}
							}
						});
                        #endregion
                    });
				}
				index--;
			}

			yield return new WaitUntil(() =>
			{
				return loadCount == finishLoadCount;
			});

			EB.Debug.Log("SpawnFuncNpcPlacementAsync end:time = {0}", Time.time);
			callback();
			System.DateTime end = System.DateTime.Now;
			EB.Assets.LoadCostLog("SceneLogic.SpawnFuncNpcPlacementAsync", (end - start).TotalMilliseconds);
		}
        #endregion

        protected virtual void OnNpcSpawnFinish()
		{
			SetModelWithGoldBody();
			RegisterDataID();
		}

		public void StartNpcPathFinding()
		{
			object data;
			DataLookupsCache.Instance.SearchDataByID("npc_path", out data);

			if (data != null)
			{
				if (!(data is Hashtable))
					return;
				Hashtable task_npc = data as Hashtable;
				string scene_id = EB.Dot.String("scene_id", task_npc, "");
				string npc_id = EB.Dot.String("npc_id", task_npc, "");
				if (m_CurrentSceneName.Equals(scene_id))
				{
					PlayerManager.LocalPlayerController().CharacterComponent.LocomotionComponent.TouchTap();
					if (m_EnemyControllers.ContainsKey(npc_id) && m_EnemyControllers[npc_id] != null)
					{
						PlayerManager.LocalPlayerController().GetComponent<CharacterTargetingComponent>().AttackTarget = m_EnemyControllers[npc_id].gameObject;
					}
					else if (AreaTriggersManager.Instance.AreaTriggerDict.ContainsKey(npc_id))
					{
						PlayerManager.LocalPlayerController().GetComponent<CharacterTargetingComponent>().AttackTarget = AreaTriggersManager.Instance.AreaTriggerDict[npc_id].gameObject;
					}
					else
						EB.Debug.LogError("No Such Npc={0}" , npc_id);
					DataLookupsCache.Instance.CacheData("npc_path", null);
				}
			}
		}

		public void NpcPathFinding(string scene_id, string npc_id)
		{
			if (m_CurrentSceneName.Equals(scene_id))
			{
				PlayerManager.LocalPlayerController().CharacterComponent.LocomotionComponent.TouchTap();
				if (m_EnemyControllers.ContainsKey(npc_id) && m_EnemyControllers[npc_id] != null)
				{
					PlayerManager.LocalPlayerController().GetComponent<CharacterTargetingComponent>().AttackTarget = m_EnemyControllers[npc_id].gameObject;
				}
				else if (AreaTriggersManager.Instance.AreaTriggerDict.ContainsKey(npc_id))
				{
					PlayerManager.LocalPlayerController().GetComponent<CharacterTargetingComponent>().AttackTarget = AreaTriggersManager.Instance.AreaTriggerDict[npc_id].gameObject;
				}
				else
					EB.Debug.LogError("No Such Npc={0}" , npc_id);
				DataLookupsCache.Instance.CacheData("npc_path", null);
			}
		}

		public void LocatorPathFinding(string scene_id, string locator)
		{
			if (m_CurrentSceneName.Equals(scene_id))
			{
				GameObject locator_ob = LocatorManager.Instance.GetLocator(locator);
				if (locator_ob == null)
				{
					EB.Debug.LogWarning("No available locator data for locator [{0}]", locator);
					return;
				}
				PlayerManager.LocalPlayerController().TargetingComponent.SetMovementTargetNoRPC(locator_ob.transform.position);
			}
		}

		public class TaskDelegate
		{
			public string taskid;
			public string sceneid;
			public string npcid;

			public TaskDelegate(string taskid, string sceneid, string npcid)
			{
				this.taskid = taskid;
				this.sceneid = sceneid;
				this.npcid = npcid;
			}
		}

		protected Dictionary<string, TaskDelegate> mTaskDelegates = new Dictionary<string, TaskDelegate>();

		public void SpawnTasksDelegate()
		{
			//遍历task  
			object value;
			DataLookupsCache.Instance.SearchDataByID("tasks", out value);
			if (value != null)
			{
				if (!(value is Hashtable)) return;
				Hashtable tasks = value as Hashtable;
				foreach (DictionaryEntry de in tasks)
				{
					string task_id = de.Key.ToString();
					Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
					if (taskTpl != null)
					{
						string task_state = (de.Value as Hashtable)["state"] as string;
						if (task_state == null) continue;
						string task_npc;
						string task_scene;
						if (task_state.Equals(TaskSystem.ACCEPTABLE))
						{
							task_npc = taskTpl.npc_id;
							task_scene = taskTpl.scene_id;
						}
						else if (task_state.Equals(TaskSystem.UNACCEPTABLE))
						{
							task_npc = taskTpl.npc_id;
							task_scene = taskTpl.scene_id;
						}
						else if (task_state.Equals(TaskSystem.RUNNING))
						{
							task_npc = taskTpl.commit_npc_id;
							task_scene = taskTpl.commit_scene_id;
						}
						else if (task_state.Equals(TaskSystem.FINISHED))
						{
							task_npc = taskTpl.commit_npc_id;
							task_scene = taskTpl.commit_scene_id;
						}
						else
						{
							continue;
						}

						if (!mTaskDelegates.ContainsKey(task_scene + task_npc))
						{
							mTaskDelegates.Add(task_scene + task_npc, new TaskDelegate(task_id, task_scene, task_npc));
						}
					}
					else
					{
						EB.Debug.LogWarning("taskTpl is null for{0}" , task_id);
					}
				}
			}
		}

		//挂载任务
		public void SpawnTasks()
		{
			//遍历task  
			object value;
			DataLookupsCache.Instance.SearchDataByID("tasks", out value);
			if (value != null)
			{
				if (!(value is Hashtable)) return;
				Hashtable tasks = value as Hashtable;
				foreach (DictionaryEntry de in tasks)
				{
					string task_id = de.Key.ToString();
					Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
					if (taskTpl != null)
					{
						string task_state = (de.Value as Hashtable)["state"] as string;
						if (task_state == null) continue;
						string task_npc;
						string task_scene;
						if (task_state.Equals(TaskSystem.ACCEPTABLE))
						{
							task_npc = taskTpl.npc_id;
							task_scene = taskTpl.scene_id;
						}
						else if (task_state.Equals(TaskSystem.UNACCEPTABLE))
						{
							task_npc = taskTpl.npc_id;
							task_scene = taskTpl.scene_id;
						}
						else if (task_state.Equals(TaskSystem.RUNNING))
						{
							task_npc = taskTpl.commit_npc_id;
							task_scene = taskTpl.commit_scene_id;
						}
						else if (task_state.Equals(TaskSystem.FINISHED))
						{
							task_npc = taskTpl.commit_npc_id;
							task_scene = taskTpl.commit_scene_id;
						}
						else
						{
							continue;
						}
						SpawnOneTask(task_id, task_scene, task_npc);
					}
				}
			}
		}

		void SpawnOneTask(string taskid, string sceneid, string npcid)
		{
			if (npcid != null && m_EnemyControllers.ContainsKey(npcid) && sceneid.Equals(m_CurrentSceneName))
			{
				EnemyController ec = m_EnemyControllers[npcid];
				if (ec == null) return;
				//找到模型物体＿增加NpcTaskDataLookup  HeadBars2D
				MoveEditor.FXHelper fx_helper = ec.gameObject.GetComponentInChildren<MoveEditor.FXHelper>();
				if (fx_helper != null)
				{
					NpcTaskDataLookup taskDataLookup = fx_helper.gameObject.GetDataLookupILRComponent<NpcTaskDataLookup>(false);
					HeadBars2D headBars2D = fx_helper.gameObject.GetMonoILRComponent<HeadBars2D>(false);
					if (headBars2D == null)
					{
						fx_helper.gameObject.AddMonoILRComponent<HeadBars2D>("Hotfix_LT.UI.HeadBars2D");
					}
					if (taskDataLookup == null)
					{
						//迁移脚本挂载
						taskDataLookup = fx_helper.gameObject.AddDataLookupILRComponent<NpcTaskDataLookup>("Hotfix_LT.UI.NpcTaskDataLookup");
						taskDataLookup.m_npcName = ec.gameObject.name;
						taskDataLookup.mDL.DefaultDataID = "tasks." + taskid;

					}
					else
					{
						taskDataLookup.m_npcName = ec.gameObject.name;
						taskDataLookup.mDL.DefaultDataID = "tasks." + taskid;

					}
				}
			}
		}

		private bool IsAppearingNpc(Hotfix_LT.UI.DungeonPlacement placement)
		{
			if (placement.IsAppearing == 0)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 异步加载一个放置物
		/// </summary>
		/// <param name="placement"></param>
		/// <returns></returns>
		private IEnumerator SpawnPlacementAsync(Hotfix_LT.UI.DungeonPlacement placement)
		{
			if (placement == null)
			{
				yield break;
			}
			if (string.Compare(placement.Script, "ChestBox") == 0)
			{
				GameObject locator = LocatorManager.Instance.GetLocator(placement.Locator);
				if (locator == null)
				{
					EB.Debug.LogWarning(string.Format("No available locator data for dungeon placement at locator [{0}]", placement.Locator));
				}
				else
				{
					InteractableObjectManager.Instance.LoadInteractableObject(placement.Encounter, locator.transform.position, locator.transform.rotation);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(placement.Pos))
				{
					bool finishLoaded = false;
					Vector3 pos = GM.LitJsonExtension.ImportVector3(placement.Pos);
					Quaternion rot = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
					SpawnNpcAsync(placement.Locator, placement.Encounter, placement.Role, pos, rot, ec =>
					{
						finishLoaded = true;
						if (ec != null)
						{
                            Player.EnemyHotfixController ehc = ec.transform.GetMonoILRComponent<Player.EnemyHotfixController>();
                            ehc.SetBarHudState(eHeadBarHud.FightStateHud, null, placement.IsFighting);
                            ehc.Role = placement.Role;
							removeOtherComponent(ehc);
                            ehc.SetNpcName(this.CurrentSceneName);
							//ec.Locator = placement.Locator;
						}
					});

					yield return new WaitUntil(() =>
					{
						return finishLoaded;
					});
				}
				else
				{
					GameObject locator = LocatorManager.Instance.GetLocator(placement.Locator);
					if (locator == null)
					{
						EB.Debug.LogWarning(string.Format("No available locator data for dungeon placement at locator [{0}]", placement.Locator));
						yield break;
					}

					bool finishLoaded = false;
					SpawnNpcAsync(placement.Locator, placement.Encounter, placement.Role, locator.transform.position, locator.transform.rotation, ec =>
					{
						finishLoaded = true;
						if (ec != null)
						{
                            Player.EnemyHotfixController ehc = ec.transform.GetMonoILRComponent<Player.EnemyHotfixController>();
                            ehc.Role = placement.Role;
							removeOtherComponent(ehc);
                            ehc.SetNpcName(this.CurrentSceneName);
							//ec.Locator = placement.Locator;
							//判定当前这个NPC是否需要显礿
							if (!IsAppearingNpc(placement))
							{
                                ehc.DisAppearBody();
							}
						}
					});

					yield return new WaitUntil(() =>
					{
						return finishLoaded;
					});
				}
			}
		}

		/// <summary>
		/// 创建NPC 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="model_name"></param>
		/// <param name="pos"></param>
		/// <param name="qt"></param>
		/// <returns></returns>
		public void SpawnNpcAsync(string name, string model_name, string role, Vector3 pos, Quaternion qt, System.Action<EnemyController> fn)
		{

			if (m_EnemyControllers.ContainsKey(name))
			{
				fn(null);
			}
			else
			{
				EnemyUtils.CreateEnemy(name, model_name, pos, qt, enemyCon =>
				{
					if (enemyCon == null)
					{
						fn(null);
					}
					else
					{
						m_EnemyControllers[name] = enemyCon;
						fn(enemyCon);
					}
				});
			}
		}

		public void SetModelWithGoldBody()
		{
			EnemyController enemyController = GetEnemyController("NPCSpawns_F");
			if (enemyController)
			{
				Player.EnemyHotfixController ehc = enemyController.transform.GetMonoILRComponent<Player.EnemyHotfixController>();
                ehc.SetModelWithGoldBody();
			}
		}

		public void DeleteNpc(string locator)
		{
			EnemyController ec = GetEnemyController(locator);
			if (ec != null)
			{
                Player.EnemyHotfixController ehc = ec.HotfixController._ilrObject as Player.EnemyHotfixController;
                if (m_DungeonPlacements.ContainsKey(locator))
				{
					m_DungeonPlacements.Remove(locator);
				}
				m_EnemyControllers.Remove(locator);

                ec.gameObject.name = ehc.ObjectName;
				PoolModel.DestroyModel(ec.gameObject);
                ehc.Destroy();
			}
		}

		public bool IsInScene()
		{
			return SceneState > eSceneState.NoneScene;
		}

		public void OnQuitScene(EB.Sparx.Response result)
		{
			m_CurrentSceneName = string.Empty;
			m_CurrentEnvironmentName = string.Empty;
			// m_CurrentEncounterGroup = string.Empty;
			PlacementDataClear();
			ClearSceneState();
		}

		public void ClearSceneState()
		{
			SceneState = eSceneState.NoneScene;
			Dictionary<string, EnemyController>.Enumerator enemy_enumerator = m_EnemyControllers.GetEnumerator();
			while (enemy_enumerator.MoveNext())
			{
				try
				{
					if (enemy_enumerator.Current.Value != null)
					{
						var fxHelpers = enemy_enumerator.Current.Value.GetComponentsInChildren<MoveEditor.FXHelper>();

						if (fxHelpers != null && fxHelpers.Length > 0)
						{
							for (var i = 0; i < fxHelpers.Length; i++)
							{
								fxHelpers[i].StopAll();
							}
						}

						enemy_enumerator.Current.Value.gameObject.name = (enemy_enumerator.Current.Value.HotfixController._ilrObject as Player .EnemyHotfixController).ObjectName;
						PoolModel.DestroyModel(enemy_enumerator.Current.Value.gameObject);
					}
				}
				catch (System.Exception ex)
				{
					EB.Debug.LogError("The object of type 'EnemyController' has been destroyed but you are still trying to access it.{0}" , ex.ToString());
				}
			}
			m_EnemyControllers.Clear();
			UnRegisterDataID();
			PlayerManager.UnregsiterLocalPlayer();
			InteractableObjectManager.Instance.ClearAllIneractableOjbects();
			PlacementDataClear();
			m_PlayerEntry.position = new Vector3();
			m_PlayerEntry.rotation = new Quaternion();
			SceneTriggerManager.Instance.ClearTrigger();
		}

		void RegisterSceneSession(Hashtable data)
		{
			if (data == null)
			{
				return;
			}
			if (data.ContainsKey("id"))
			{
				m_SceneId = EB.Dot.Integer("id", data, 0);
			}

			if (data.ContainsKey("map_name"))
			{
				m_CurrentEnvironmentName = EB.Dot.String("map_name", data, string.Empty);
			}
			else
			{
				EB.Debug.LogError("Wrong campaign session data format: no campaign environment name!");
			}

			if (data.ContainsKey("name"))
			{
				m_CurrentSceneName = EB.Dot.String("name", data, string.Empty);
			}

			RegisterPlacementData(data);
		}

		public bool GetNearestEnemy(out EnemyController Enemy)
		{
			Enemy = null;
			if (PlayerManager.LocalPlayerController() == null) return false;
			Vector3 current_position = PlayerManager.LocalPlayerController().transform.localPosition;
			float mindist = float.MaxValue;
			if (m_EnemyControllers.Count <= 0) return false;
			Seeker seeker = PlayerManager.LocalPlayerController().GetComponent<Seeker>();
			if (seeker == null) return false;

			var iter = m_EnemyControllers.GetEnumerator();
			while (iter.MoveNext())
			{
				var de = iter.Current;
				if (de.Value != null)
				{
					//this is temp   fix to use config
					Vector3 targetposition = de.Value.transform.localPosition;
					Path p = seeker.StartPath(current_position, targetposition);
					AstarPath.WaitForPath(p);
					float dist = LocomotionComponentAPP.CalculateDistanceOnPath(p, 0, targetposition);
					if (dist <= mindist)
					{
						mindist = dist;
						Enemy = de.Value;
					}
				}
			}
			iter.Dispose();
			if (Enemy == null) return false;
			return true;
		}

		public bool GetNearestEnemyDirection(out Vector3 EnemyDirection, out float distance)
		{
			EnemyDirection = Vector3.one;
			distance = 0f;
			if (PlayerManager.LocalPlayerController() == null) return false;
			Vector3 current_position = PlayerManager.LocalPlayerController().transform.localPosition;
			float mindist = float.MaxValue;
			if (m_EnemyControllers.Count <= 0) return false;
			var iter = m_EnemyControllers.GetEnumerator();
			while (iter.MoveNext())
			{
				KeyValuePair<string, EnemyController> de = iter.Current;
				if (de.Value != null)
				{
					Vector3 dir = de.Value.transform.localPosition - current_position;
					float dist = dir.sqrMagnitude;
					if (dist <= mindist)
					{
						mindist = dist;
						EnemyDirection = dir.normalized;
						distance = dist;
					}
				}
			}
			iter.Dispose();

			return true;
		}

		public static void Transfer(string from_id, UnityEngine.Vector3 from_pos, float from_dir, string to_id, UnityEngine.Vector3 to_pos, float to_dir, System.Action<bool> callback = null)
		{
			InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 3600f);
			PlayerController.LocalPlayerDisableNavigation();
			MainLandLogic.GetInstance().RequestTransfer(from_id, from_pos, 90.0f, to_id, to_pos, 90.0f, delegate (bool result)
			{
				InputBlockerManager.Instance.UnBlock(InputBlockReason.CONVERT_FLY_ANIM);
				PlayerController.LocalPlayerEnableNavigation();
				if (result)
				{
					EB.Debug.Log("切换场景加载开始=====>");
					UIStack.Instance.ShowLoadingScreen(() =>
					{
					}, false, true);
				}
				if (callback != null)
				{
					callback(result);
				}
			});
		}

		public static void Transfer_RobDart(string from_id, UnityEngine.Vector3 from_pos, float from_dir, string to_id, UnityEngine.Vector3 to_pos, float to_dir, long uid, string dart_id)
		{
			MainLandLogic.GetInstance().RequestTransfer_Rob(from_id, from_pos, 90.0f, to_id, to_pos, 90.0f, uid, dart_id);
		}

		public void OnExitSceneView()
		{
			StopAllCoroutines();
			//Refresh player postion
			PlayerController local_player = PlayerManager.LocalPlayerController();
			if (local_player != null)
			{
				m_PlayerEntry.position = local_player.transform.position;
				m_PlayerEntry.rotation = local_player.transform.rotation;
			}
			UnRegisterDataID();
			SceneTriggerManager.Instance.ClearTrigger();
		}

		/// <summary>
		/// destroy all player and npc
		/// </summary>
		public void DestroyDynamicGameObjects()
		{
			if (m_EnemyControllers != null)
			{
				foreach (KeyValuePair<string, EnemyController> kv in m_EnemyControllers)
				{
					if (kv.Value != null)
					{
						//					kv.Value.Destroy();
						kv.Value.gameObject.name = (kv.Value.HotfixController._ilrObject as Player.EnemyHotfixController).ObjectName;
						PoolModel.DestroyModel(kv.Value.gameObject);
					}
				}
				m_EnemyControllers.Clear();
			}
			PlayerManager.DestroyPlayerControllers();
		}

		public void RequestCombatResumeToScene()
		{
			UIStack.Instance.ShowLoadingScreen(() =>
			{
				m_SparxSceneManager.RequestCombatResumeToScene(OnCombatResumeToScene);
				m_PlayerEntry.position = new Vector3();
				m_PlayerEntry.rotation = new Quaternion();
			}
				, false, true);
		}

		public void OnCombatResumeToScene(EB.Sparx.Response result)
		{
			if (result.sucessful)
			{
				DialoguePlayUtil.Instance.FinishDialogue();
				OnQuitScene(result);
				MainLandLogic.CleanMainLandData();
				DataLookupsCache.Instance.CacheData(result.hashtable);
				//SparxHub.Instance.GetManager<EB.Sparx.SceneManager>().EnterSceneByPlayState();
				m_SparxSceneManager.EnterSceneByPlayState();
			}
			else if (result.fatal)
			{
				OnQuitScene(result);
				EB.Debug.LogWarning("OnCombatResumeToScene: Fail to Resume!");
				SparxHub.Instance.FatalError(result.localizedError);
			}
			else
			{
				MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"), result.localizedError, EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, delegate (int ret)
				{
					UIStack.Instance.HideLoadingScreen();
				}, NGUIText.Alignment.Center);
			}
		}

		public static bool MainHeroLoadComplete = true;
		public static void PreLoadMainHero()
		{
			MainHeroLoadComplete = false;
			EB.Coroutines.Run(PreLoadAsyncMainHero());
		}
		static IEnumerator PreLoadAsyncMainHero()
		{
			yield return null;
			System.DateTime start = System.DateTime.Now;
			if (SceneLogicManager.getSceneType() == SceneLogicManager.SCENE_COMBAT)
			{
				if (GameFlowControlManager.Instance != null)
				{
					yield return PSPoolManager.Instance.LoadStandardCombatFX();
				}
				MainHeroLoadComplete = true;
				yield break;
			}
			else
			{
				string userid = LoginManager.Instance.LocalUserId.Value.ToString();
				string classname = BuddyAttributesManager.GetModelClass(userid);
				if (string.IsNullOrEmpty(classname))
				{
					EB.Debug.Log("PreLoadMainHero: classname not found for uid = {0}", userid);
					MainHeroLoadComplete = true;
					yield break;

				}
				CharacterModel characterModel = CharacterCatalog.Instance.GetModel(classname);
				string prefabName = characterModel.PrefabNameFromGenderMain(eGender.Male);
				yield return PoolModel.PreloadAsync(prefabName, delegate (UnityEngine.Object o)
				{
					MainHeroLoadComplete = true;
				});
			}

			System.DateTime end = System.DateTime.Now;
			EB.Assets.LoadCostLog("SceneLogic.PreLoadMainHero", (end - start).TotalMilliseconds);
		}

		/// <summary>
		/// 延迟加载场景
		/// </summary>
		/// <param name="waitTime"> 等待时间</param>
		/// <param name="overTime"> 超时时间</param>
		/// <param name="result"></param>
		public void DoDelayCombatTransition(int waitTime, int overTime, System.Action<bool> result)
		{
			if (overTime < waitTime)
			{
				overTime = waitTime;
			}
			m_SparxSceneManager.StartDoCombatQueue(waitTime, overTime, result);
		}

		/// <summary>
		/// 是否PVP玩法，
		/// </summary>
		/// <returns></returns>
		public static bool isPVP()
		{
			return BattleType == eBattleType.PVPBattle ||
				BattleType == eBattleType.LadderBattle ||
				BattleType == eBattleType.TransferOrRob ||
				BattleType == eBattleType.RedName ||
				BattleType == eBattleType.NationBattle ||
				BattleType == eBattleType.CityRobBattle;
		}
		//方便主工程访问
		public static bool isSceneLoop()
        {
			return SceneState == eSceneState.SceneLoop;
		}
	}
}