using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class NationManager : ManagerUnit
	{

		public const string ConfigDataId = "nation.configs";
		public const string BattleTimeConfigDataId = "nation.configs.battleTime";
		public const string ListDataId = "nation.list";
		public const string AccountDataId = "nation.account";
		public const string DetailDataId = "nation.detail";
		public const string MembersDataId = "nation.members";
		public const string TerritoryDataId = "nation.territory";

		public const string BattleDataId = "nation.battle";
		public const string ScoreRankDataId = "nation.scoreRank";

		private static NationManager sInstance = null;
		public static NationManager Instance
		{
			get { return sInstance = sInstance ?? LTHotfixManager.GetManager<NationManager>(); }
		}

		public NationAPI Api
		{
			get; private set;
		}

		public NationConfig Config
		{
			get; private set;
		}

		public NationBattleTimeConfig BattleTimeConfig
		{
			get; private set;
		}

		public Nations List
		{
			get; private set;
		}

		public NationDetail Detail
		{
			get; private set;
		}

		public NationAccount Account
		{
			get; private set;
		}

		public NationMembers Members
		{
			get; private set;
		}

		public NationTerritoryList TerritoryList
		{
			get; private set;
		}

		public NationBattleSyncData BattleSyncData
		{
			get; private set;
		}

		public NationScoreRankList ScoreRankList
		{
			get; private set;
		}

		public bool IsGetMemberDataByGetInfo;

		public bool IsPushMsg;
		public bool Actioned;
		public int BattleEventID;

		public bool IsPopRatingDialog;
		public float ResidueHp;
		public int ChangeCityDefend;
		public int DonateReward;
		public Hashtable BattleWinRatingData;
		public string CurrentGoOnTeamName = "nation1";
		public string WinNation;
		public string LoseNation;

		public override void Initialize(EB.Sparx.Config config)
		{
			Instance.Api = new NationAPI();
			Instance.Api.ErrorHandler += ErrorHandler;

			Config = GameDataSparxManager.Instance.Register<NationConfig>(ConfigDataId);
			BattleTimeConfig = GameDataSparxManager.Instance.Register<NationBattleTimeConfig>(BattleTimeConfigDataId);
			List = GameDataSparxManager.Instance.Register<Nations>(ListDataId);
			Detail = GameDataSparxManager.Instance.Register<NationDetail>(DetailDataId);
			Account = GameDataSparxManager.Instance.Register<NationAccount>(AccountDataId);
			Members = GameDataSparxManager.Instance.Register<NationMembers>(MembersDataId);
			TerritoryList = GameDataSparxManager.Instance.Register<NationTerritoryList>(TerritoryDataId);
			BattleSyncData = GameDataSparxManager.Instance.Register<NationBattleSyncData>(BattleDataId);
			ScoreRankList = GameDataSparxManager.Instance.Register<NationScoreRankList>(ScoreRankDataId);
		}

		public override void OnLoggedIn()
		{
			Actioned = false;

			if (NationBattleOpenCoroutine != null)
				EB.Coroutines.Stop(NationBattleOpenCoroutine);
			NationBattleOpenCoroutine = EB.Coroutines.Run(NationBattleOpenTimer());
			Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;

            object nation = loginData["nation"];
			if (nation == null)
			{
				//EB.Debug.LogWarning("NationManager.OnLoggedIn: nation not found in LoginData");
				return;
			}
			loginData.Remove("nation");
		}

		public override void Async(string message, object payload)
		{
			IsPushMsg = true;
			switch (message)
			{
				case "nationTeamInfo":
					LTFormationDataManager.Instance.SetFormationData();
					break;
				case "startAction":
					break;
				case "syncHp":
					//HP = EB.Dot.Integer("nation.battle.t_info.h", obj, HP);
					break;
				case "battle":
					break;
				case "battleOver":  //战胜
					break;
				case "actionOver":  //行动结束（战败或者直接到终点）
					break;
				case "myOver":      //我的队伍出征结束
					Actioned = false;
					BattleEventID = EB.Dot.Integer("nation.battleEvtId", payload, 0);

					string useTeam = EB.Dot.String("nation.useTeam", payload, "");
					float residueHp = EB.Dot.Single("nation.residueHp", payload, 0);
					int changeCityDefend = EB.Dot.Integer("nation.changeCityDefend", payload, 0);
					DonateReward = EB.Dot.Integer("nation.degree", payload, 0);
					
					BattleWinRatingData = Johny.HashtablePool.Claim();
					BattleWinRatingData.Add("useTeam", useTeam);
					BattleWinRatingData.Add("residueHp", residueHp);
					BattleWinRatingData.Add("changeCityDefend", changeCityDefend);
					BattleWinRatingData.Add("degree", DonateReward);

					PopAttackCityResultPanel();
					if (NationBattleHudController.Instance!=null)
						NationBattleHudController.Instance.DisableOperation(false);
					break;
				case "call":
					int territoryIndex = EB.Dot.Integer("nation.call.index", payload, -1);
					if (territoryIndex >= 0)
					{
						if (NationBattleHudController.Instance != null)
						{
							Debug.LogError("Call:Has Enter NationBattle");
							return;
						}

						MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_NationManager_4938"), delegate (int result)
						{
							if (result == 0)
							{
								if (NationBattleEntryController.Instance != null)
								{
									NationBattleEntryController.Instance.SkipToBattleHudUI(territoryIndex);
								}
								else
								{
									GlobalMenuManager.Instance.Open("LTNationBattleEntryUI", territoryIndex);
								}
							}
						});
					}
					else
					{
						Debug.LogError("call:territoryIndex Error");
					}
					break;
				case "captureCity":
					WinNation = EB.Dot.String("nation.winNation", payload, null);
					LoseNation = EB.Dot.String("nation.loseNation", payload, null);
					if (NationBattleHudController.Instance != null)
						NationBattleHudController.Instance.SetBattleOver();
					if (NationBattleHudController.Instance != null)
						GlobalMenuManager.Instance.Open("LTNationBattleCityVictory");
					else
						GlobalMenuManager.Instance.PushCache("LTNationBattleCityVictory");
					break;
			}
		}

		Coroutine NationBattleOpenCoroutine;
		WaitForSeconds wait1;
		IEnumerator NationBattleOpenTimer()
		{
			wait1 = new WaitForSeconds(1);
			while (!BattleTimeConfig.GetIsBattleStart())
			{
				yield return wait1;
			}
			if (!Hotfix_LT.Data.EventTemplateManager.Instance.GetRealmIsOpen("nationswarstartstage1"))
			{
				yield break;
			}

			//MessageTemplateManager.ShowMessage(902265);
			Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.RedPoint_Nation);
			yield break;
		}

		public void PopAttackCityResultPanel()
		{
			if (NationBattleHudController.Instance != null)
			{
				if (BattleEventID > 0)
				{

						System.Action cb = delegate () { PopRatingMenu(); };
						var ht = Johny.HashtablePool.Claim();
						ht.Add("eventId", BattleEventID);
						ht.Add("cb", cb);
						GlobalMenuManager.Instance.Open("LTNationBattleSpecialEventUI", ht);
				}
				else
				{
					PopRatingMenu();
				}
			}
		}

		void PopRatingMenu()
		{
			if (DonateReward > 0 && NationBattleHudController.Instance != null)
			{
				GlobalMenuManager.Instance.Open("LTNationBattleRatingUI", BattleWinRatingData);
				DonateReward = 0;
			}
		}

		private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
		{
			return false;
		}

		private void FetchDataHandler(Hashtable payload)
		{
			if (payload != null)
			{
				GameDataSparxManager.Instance.ProcessIncomingData(payload, false);
			}
		}

		private void MergeDataHandler(Hashtable payload)
		{
			if (payload != null)
			{
				//DataLookupsCache.Instance.CacheData(payload);
				GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
			}
		}

		//public int GetReduceTeamCDTime()
		//{
		//	return BattleSyncData.GeneralNum* Config.ReduceTeamCDByGeneral;
		//}

		public void GetInfo()
		{
			IsGetMemberDataByGetInfo = false;
			Api.GetInfo(FetchDataHandler);
		}

		public void Select(string nationName, System.Action<bool> callback)
		{
			Api.Select(nationName, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				callback(payload != null);
			});
		}

		public void GetMemberList(string rank, System.Action<bool> callback)
		{
			Members.ReqRankName = rank;
			Api.GetMemberList(rank, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void ModifyNotice(string notice, System.Action<bool> callback)
		{
			Api.ModifyNotice(notice, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				callback(payload != null);
			});
		}

		public void ReceiveRankReward(System.Action<bool> callback)
		{
			Api.ReceiveRankReward(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				callback(payload != null);
			});
		}

		public void GetTerritoryInfo(System.Action<bool> callback)
		{
			Api.GetTerritoryInfo(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void EnterField(int index, System.Action<bool> callback)
		{
			IsPushMsg = false;
			BattleSyncData.CleanUp();
			ScoreRankList.CleanUp();
			//Actioned = false;
			Api.EnterField(index, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void StartAction(string path, string team, string[] emptyTeam, System.Action<bool> callback)
		{
			Api.StartAction(path, team, emptyTeam, delegate (Hashtable payload)
			{
				if (payload != null)
				{
					Actioned = true;
					FetchDataHandler(payload);
				}
				if (callback != null)
					callback(payload != null);
			});
		}

		public void ChangeNationTeamStage(bool haveHero, string team, System.Action<bool> callback)
		{
			Api.ChangeNationTeamStage(haveHero, team, delegate (Hashtable payload)
			{
				if (payload != null)
				{
					Actioned = true;
					FetchDataHandler(payload);
				}
				if (callback != null)
					callback(payload != null);
			});
		}

		public void ContinueWalk(System.Action<bool> callback)
		{
			Api.ContinueWalk(delegate (Hashtable payload)
			{
				if (payload != null)
				{
					FetchDataHandler(payload);
				}
				if (callback != null)
					callback(payload != null);
			});
		}

		public void ExitField(int index, System.Action<bool> callback)
		{
			//BattleSyncData.CleanUp();
			//ScoreRankList.CleanUp();
			Api.ExitField(index, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void GetTeamInfo(System.Action<bool> callback)
		{
			Api.GetTeamInfo(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void Revive(System.Action<bool> callback)
		{
			Api.Revive(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void DonateRank(System.Action<Hashtable> callback)
		{
			Api.DonateRank(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload);
			});
		}

		public void ScoreRank(int territoryIndex, System.Action<Hashtable> callback)
		{
			Api.ScoreRank(territoryIndex, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload);
			});
		}

		public void RefreshCityScore(int status, System.Action<Hashtable> callback)
		{
			Api.RefreshCityScore(status, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload);
			});
		}

		public void SureEvent(System.Action<Hashtable> callback)
		{
			Api.SureEvent(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload);
			});
		}

		public void Call(System.Action<bool> callback)
		{
			Api.Call(delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void SetSkill(int index, System.Action<bool> callback)
		{
			Api.SetSkill(index, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		#region debug Func
		public void StartEvent(int realmId, int stage, System.Action<bool> callback)
		{
			Api.StartEvent(realmId, stage, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void StopEvent(int realmId, int stage, System.Action<bool> callback)
		{
			Api.StopEvent(realmId, stage, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void ResetNationRank(int realmId, System.Action<bool> callback)
		{
			Api.ResetNationRank(realmId, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void AddRobot(string nation, string side, System.Action<bool> callback)
		{
			Api.AddRobot(nation, side, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void RobotWork(string side, string path, System.Action<bool> callback)
		{
			Api.RobotWork(side, path, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}

		public void TestSetSkill(int skillId, System.Action<bool> callback)
		{
			Api.TestSetSkill(skillId, delegate (Hashtable payload)
			{
				if (payload != null)
					FetchDataHandler(payload);
				if (callback != null)
					callback(payload != null);
			});
		}
		#endregion

		public override void Connect()
		{
			//State = EB.Sparx.SubSystemState.Connected;
		}

		public override void Disconnect(bool isLogout)
		{

		}

		public override void Dispose()
		{
			//Info.SaveRank();
		}
	}
}
