using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hotfix_LT.UI
{
	public enum eNationState
	{
		None = 1,
		Joined = 2,
		Leaved = 3,
	}

	public enum eNationName
	{
		persian,
		roman,
		egypt
	}

	public enum eRanks
	{
		plebs,   //平民
		warrior, //勇士
		knight,  //骑士
		general, //将军
		marshal, //元帅
		king,    //国王
	}

	public enum eTerritoryType
	{
		Main,
		Small,
		Neutral
	}

	public enum eTerritoryAttackOrDefendType
	{
		None,
		Attack,
		Defend
	}

	public class eTerritoryState
	{
		// static public string Close = "close";
		static public string Open = "open";
		// static public string Invincible = "protect";  //无敌
		static public string Capture = "capture";    //被占领
	}

	public enum eTeamState
	{
		Available = 0,
		InTheWar,
		Death,
		Arrive,
		Empty
	}

	public enum eNationBattleBuffType
	{
		fullAoyi,    //is mainCity
		cityDefend,  //is protect
		call,        //is have king
		addAttr,     //is have marshal
		reduceTeamCD //general num
	}

	public class TerritoryConfig
	{
		public eTerritoryType Type;
		public int MaxHp;

		public TerritoryConfig()
		{
		}

		public TerritoryConfig(eTerritoryType type, int maxhp)
		{
			Type = type;
			MaxHp = maxhp;
		}
	}

	public class NationConfig : INodeData
	{
		public int RandomRewardHC;
		public string ResetCronTime;
		public int TeamReviveCost;
		public int ReduceCityDefendDescendValue;
		public int ReduceTeamCDByGeneral;
		public int AddAttrSkillIDPrefix;
		public float AddSpeedMultiple;
		public TerritoryConfig[] TerritoryConfigs;

		public void CleanUp()
		{

		}

		public NationConfig()
		{
			RandomRewardHC = 100;
			ResetCronTime = "0 0 * * 1";
			TeamReviveCost = 10;
			ReduceCityDefendDescendValue = 90;
			ReduceTeamCDByGeneral = 5;
			AddAttrSkillIDPrefix = 10000;
			AddSpeedMultiple = 1.5f;
			eTerritoryType main = eTerritoryType.Main;
			eTerritoryType small = eTerritoryType.Small;
			eTerritoryType neutral = eTerritoryType.Neutral;
			TerritoryConfigs = new TerritoryConfig[10] { new TerritoryConfig(main,5000),new TerritoryConfig(small,5000),new TerritoryConfig(small,5000),
			new TerritoryConfig(small,5000),new TerritoryConfig(neutral,5000),new TerritoryConfig(small,5000),
			new TerritoryConfig(main,5000),new TerritoryConfig(small,5000),new TerritoryConfig(small,5000),
			new TerritoryConfig(main,5000)
		};
		}

		public void SetTeamReviveCost()
		{
			object obj = EB.JSON.Parse(Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("nationResetCost"));
			TeamReviveCost = EB.Dot.Integer("quantity", (obj as ArrayList)[0], 0);
		}


		public object Clone()
		{
			return new NationConfig();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
			RandomRewardHC = EB.Dot.Integer("randomRewardHC", obj, RandomRewardHC);
			ResetCronTime = EB.Dot.String("resetTime", obj, ResetCronTime);
			SetTeamReviveCost(); ;
			ReduceCityDefendDescendValue = EB.Dot.Integer("reduceCityDefendDescendValue", obj, ReduceCityDefendDescendValue);
			ReduceTeamCDByGeneral = EB.Dot.Integer("reduceTeamCDByGeneral", obj, ReduceTeamCDByGeneral);
			AddSpeedMultiple = EB.Dot.Single("addSpeedMultiple", obj, AddSpeedMultiple);
			Hashtable hashData = obj as Hashtable;
			if (hashData["territorys"] != null)
			{
				TerritoryConfigs = Hotfix_LT.EBCore.Dot.Array<TerritoryConfig>("territorys", obj, TerritoryConfigs, delegate (object value)
				{
					TerritoryConfig tc = new TerritoryConfig();
					tc.Type = Hotfix_LT.EBCore.Dot.Enum<eTerritoryType>("type", value, tc.Type);
					tc.MaxHp = EB.Dot.Integer("maxHp", value, 0);
					return tc;
				});
			}
		}

		public System.DateTime GetResetTime()
		{
			try
			{
				/*TimerScheduler timerScheduler = new TimerScheduler();
				timerScheduler.m_TimerRegular = TimerScheduler.AmendCronFormat(ResetCronTime);
				timerScheduler.ParseTimerRegular();
				if (!timerScheduler.isLegalTimer)
				{
					EBCore.Debug.LogError("parse nation resetTime: cronFormat is illegal");
				}
				System.DateTime date;
				timerScheduler.GetNext(EBCore.Time.LocalNow, out date);
				return date;*/
				return new System.DateTime();
			}
			catch
			{
				EB.Debug.LogError("parse nation resetTime exception");
				return new System.DateTime();
			}
		}
	}

	public class NationBattleTeam
	{
		public string Name;
		public eTeamState State;
		public int ReviveTs;

		public eTeamState RealState
		{
			get
			{
				//if (NationManager.Instance.BattleSyncData.BuffTypeList.Contains(eNationBattleBuffType.cityDefend) && NationBattleHudController.TerritoryData != null && NationBattleHudController.TerritoryData.ADType == eTerritoryAttackOrDefendType.Defend)
				//{
				//	return eTeamState.Available;
				//}
				if (LTFormationDataManager.Instance.GetTeamMemList(Name).Count <= 0 || ReviveTs == -2)
					return eTeamState.Empty;
				if (ReviveTs < 0)
				{
					return eTeamState.Death;
				}
				if (ReviveTs - EB.Time.Now <= 0)
				{
					return eTeamState.Available;
				}
				return State;
			}
		}
	}

	public class NationAccount : INodeData
	{
		public string NationName;
		public string Rank;
		public bool HaveReceiveRankReward;
		public bool ShowRankChange;
		public bool Updated;
		public bool TeamDataUpdated;

		public List<NationBattleTeam> TeamList
		{
			get; private set;
		}

		public void CleanUp()
		{
			NationName = string.Empty;
			Rank = string.Empty;
			HaveReceiveRankReward = false;
			Updated = false;
			TeamDataUpdated = false;

			TeamList = new List<NationBattleTeam>();
		}

		public NationAccount()
		{
			CleanUp();
		}

		public object Clone()
		{
			return new NationAccount();
		}

		public void OnMerge(object obj)
		{
			Updated = true;
			NationName = EB.Dot.String("nation", obj, NationName);
			Rank = EB.Dot.String("rank", obj, Rank);
			HaveReceiveRankReward = EB.Dot.Bool("haveReceiveRanksReward", obj, HaveReceiveRankReward);
			ShowRankChange = EB.Dot.Bool("showRankChange", obj, ShowRankChange);

			object teamObj = EB.Dot.Object("team", obj, null);
			if (teamObj != null)
			{
				TeamDataUpdated = true;
				TeamList = Hotfix_LT.EBCore.Dot.List<NationBattleTeam, string>(string.Empty, teamObj, TeamList, ParseMember, (member, name) => member.Name == name);
				if (TeamList.Count != 3)
					EB.Debug.LogError("nation TeamList count error");
			}
		}

		public void OnUpdate(object obj)
		{
			Updated = true;
			NationName = EB.Dot.String("nation", obj, NationName);
			Rank = EB.Dot.String("rank", obj, Rank);
			HaveReceiveRankReward = EB.Dot.Bool("haveReceiveRanksReward", obj, HaveReceiveRankReward);
			ShowRankChange = EB.Dot.Bool("showRankChange", obj, ShowRankChange);

			object teamObj = EB.Dot.Object("team", obj, null);
			if (teamObj != null)
			{
				TeamDataUpdated = true;
				TeamList = Hotfix_LT.EBCore.Dot.List<NationBattleTeam, string>(string.Empty, teamObj, TeamList, ParseMember);
				if (TeamList.Count != 3)
					EB.Debug.LogError("nation TeamList count error");
			}
		}

		private NationBattleTeam ParseMember(object value, string name)
		{
			if (value == null)
			{
				return null;
			}
			NationBattleTeam member = FindTeam(name) ?? new NationBattleTeam();
			member.Name = EB.Dot.String("n", value, member.Name);
			member.State = Hotfix_LT.EBCore.Dot.Enum<eTeamState>("s", value, member.State);
			member.ReviveTs = EB.Dot.Integer("rTs", value, -2);// -2代表为空
			return member;
		}

		public NationBattleTeam FindTeam(string teamName)
		{
			return TeamList.Find(m => m.Name == teamName);
		}
	}

	public class NationDetail : INodeData
	{
		public class KingData
		{
			public long Uid;
			public string Name;
			public int Level;
			public Hotfix_LT.Data.eRoleAttr Attr;
			public string Portrait;
			public string Model;
		}

		public string NationName;
		public string Notice;
		public KingData King;
		public bool Updated;

		public long KingUid
		{
			get
			{
				if (King == null)
					return 0;
				return King.Uid;
			}
		}

		public void CleanUp()
		{

		}

		public NationDetail()
		{
			CleanUp();
		}

		public object Clone()
		{
			return new NationDetail();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
			Updated = true;
			NationName = EB.Dot.String("nation", obj, NationName);
			Notice = EB.Dot.String("notice", obj, Notice);
			object kingData = EB.Dot.Object("king", obj, null);
			if (kingData != null)
			{
				King = new KingData();
				King.Uid = EB.Dot.Long("uid", kingData, 0);
				King.Name = EB.Dot.String("name", kingData, "");
				King.Level = EB.Dot.Integer("level", kingData, 0);
				King.Attr = Hotfix_LT.EBCore.Dot.Enum<Hotfix_LT.Data.eRoleAttr>("attr", kingData, 0);
				King.Portrait = EB.Dot.String("portrait", kingData, "");
				King.Model = EB.Dot.String("model", kingData, "");
			}
		}
	}

	public class NationData
	{
		public string Name;
		public int PeopleNum;
		public float Occupancy;
	}

	public class Nations : INodeData
	{
		public List<NationData> Members
		{
			get; set;
		}
		public bool Updated;

		public void CleanUp()
		{
			Members.Clear();
			Updated = false;
		}

		public object Clone()
		{
			return new Nations();
		}

		public void OnUpdate(object obj)
		{
			Updated = true;
			Members = Hotfix_LT.EBCore.Dot.List<NationData, string>(string.Empty, obj, Members, ParseMember);
			ConstructionData();
			if (Members.Count != 3)
				EB.Debug.LogError("nation list count error");
		}

		public void OnMerge(object obj)
		{
			Updated = true;
			Members = Hotfix_LT.EBCore.Dot.List<NationData, string>(string.Empty, obj, Members, ParseMember, (member, name) => member.Name == name);
			if (Members.Count != 3)
				EB.Debug.LogError("nation list count error");
		}

		private NationData ParseMember(object value, string name)
		{
			if (value == null)
			{
				return null;
			}

			NationData member = Find(name) ?? new NationData();
			member.Name = EB.Dot.String("name", value, member.Name);
			member.PeopleNum = EB.Dot.Integer("peopleNum", value, member.PeopleNum);
			member.Occupancy = EB.Dot.Single("occupancy", value, member.Occupancy);
			return member;
		}

		public Nations()
		{
			Members = new List<NationData>();
		}

		private void ConstructionData()
		{
			string[] nationNames = NationUtil.NameArr;
			for (var i = 0; i < nationNames.Length; i++)
			{
				string nationName = nationNames[i];
				if (Members.Find(m => m.Name == nationName) != null)
					continue;

				NationData data = new NationData();
				data.Name = nationName;
				data.PeopleNum = 0;
				data.Occupancy = 0;
				Members.Add(data);
			}
		}

		public string GetMinName()
		{
			if (Members.Count <= 0)
				return NationUtil.NameArr[0];

			NationData min = Members[0];
			for (var i = 0; i < Members.Count; i++)
			{
				var nation = Members[i];
				if (nation.PeopleNum < min.PeopleNum)
				{
					min = nation;
				}
			}
			return min.Name;
		}

		public NationData Find(string name)
		{
			NationData member = Members.Where(m => m.Name == name).FirstOrDefault();
			return member;
		}

		public void Remove(string name)
		{
			Members.RemoveAll(m => m.Name == name);
		}
	}

	public class NationMember
	{
		public long Uid;
		public string Name;
		public int Level;
		public string Portrait;
		public string Rank;
		public int TotalDonate;
		public int WeekDonate;
		public long JoinTime;
		public long OfflineTime;
		public long DegreeUpdateTime;
	}

	public class NationMemberComparer : IComparer<NationMember>
	{
		public int Compare(NationMember x, NationMember y)
		{
			if (x.TotalDonate != y.TotalDonate)
				return y.TotalDonate - x.TotalDonate;

			if (x.WeekDonate != y.WeekDonate)
				return y.WeekDonate - x.WeekDonate;

			if (x.DegreeUpdateTime != y.DegreeUpdateTime)
				return (int)(x.DegreeUpdateTime - y.DegreeUpdateTime);

			if (x.OfflineTime == 0 && y.OfflineTime != 0)
				return -1;
			else if (y.OfflineTime == 0 && x.OfflineTime != 0)
				return 1;

			if (x.OfflineTime != y.OfflineTime)
				return x.OfflineTime - y.OfflineTime < 0 ? 1 : -1;

			return (int)(x.JoinTime - y.JoinTime);
		}
	}

	public class NationMembers : INodeData
	{
		//public List<NationMember> Members
		//{
		//	get; set;
		//}

		public class MemberRequestRecord
		{
			public float Ts;
			public List<NationMember> DataList;
		}

		public string ReqRankName;
		public Dictionary<string, MemberRequestRecord> ReqMemberRecordDictionary;
		public bool Updated;
		public System.DateTime ResetTime;

		public void CleanUp()
		{
			ReqRankName = "";
			Updated = false;
			ReqMemberRecordDictionary.Clear();
			ResetTime = NationManager.Instance.Config.GetResetTime();
		}

		public object Clone()
		{
			return new NationMembers();
		}

		public void OnUpdate(object obj)
		{
			//Members = Hotfix_LT.EBCore.Dot.List<NationMember, long>(string.Empty, obj, Members, ParseMember);
			//if (ReqMemberRecordDictionary.ContainsKey(ReqRankName))
			//{
			//	EBCore.Debug.LogError("ReqMemberRecordDictionary has contain key for:" + ReqRankName);
			//	return;
			//}
			Updated = true;
			//tempList.Sort(new NationMemberComparer());
			if (!string.IsNullOrEmpty(ReqRankName))
			{
				if (ReqMemberRecordDictionary.ContainsKey(ReqRankName))
				{
					ReqMemberRecordDictionary[ReqRankName].DataList.Clear();
				}
			}
			else
			{
				if (ReqMemberRecordDictionary.ContainsKey(eRanks.king.ToString()))
				{
					ReqMemberRecordDictionary[eRanks.king.ToString()].DataList.Clear();
				}
				if (ReqMemberRecordDictionary.ContainsKey(eRanks.marshal.ToString()))
				{
					ReqMemberRecordDictionary[eRanks.marshal.ToString()].DataList.Clear();
				}
				if (ReqMemberRecordDictionary.ContainsKey(eRanks.general.ToString()))
				{
					ReqMemberRecordDictionary[eRanks.general.ToString()].DataList.Clear();
				}
			}

			List<NationMember> fullList = Hotfix_LT.EBCore.Dot.List<NationMember, long>(string.Empty, obj, new List<NationMember>(), ParseMember);
			for (int i = 0; i < fullList.Count; ++i)
			{
				string rankName = fullList[i].Rank;
				if (!ReqMemberRecordDictionary.ContainsKey(rankName))
				{
					MemberRequestRecord record = new MemberRequestRecord();
					record.Ts = EB.Time.Now;
					record.DataList = new List<NationMember>();
					record.DataList.Add(fullList[i]);
					ReqMemberRecordDictionary.Add(rankName, record);
				}
				else
				{
					ReqMemberRecordDictionary[rankName].Ts = EB.Time.Now;
					ReqMemberRecordDictionary[rankName].DataList.Add(fullList[i]);
				}
			}
			if (!string.IsNullOrEmpty(ReqRankName))
			{
				if (ReqMemberRecordDictionary.ContainsKey(ReqRankName))
				{
					ReqMemberRecordDictionary[ReqRankName].DataList.Sort(new NationMemberComparer());
				}
			}
			else
			{
				if (ReqMemberRecordDictionary.ContainsKey(eRanks.king.ToString()))
				{
					ReqMemberRecordDictionary[eRanks.king.ToString()].DataList.Sort(new NationMemberComparer());
				}
				if (ReqMemberRecordDictionary.ContainsKey(eRanks.marshal.ToString()))
				{
					ReqMemberRecordDictionary[eRanks.marshal.ToString()].DataList.Sort(new NationMemberComparer());
				}
				if (ReqMemberRecordDictionary.ContainsKey(eRanks.general.ToString()))
				{
					ReqMemberRecordDictionary[eRanks.general.ToString()].DataList.Sort(new NationMemberComparer());
				}
			}
			ReqRankName = null;
			NationManager.Instance.IsGetMemberDataByGetInfo = true;
		}

		public void OnMerge(object obj)
		{
			EB.Debug.LogError("Merge NationMembers Invalid");
			//Members = Hotfix_LT.EBCore.Dot.List<NationMember, long>(string.Empty, obj, Members, ParseMember, (member, uid) => member.Uid == uid);
		}

		private NationMember ParseMember(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}
			NationMember member = /*Find(uid) ??*/ new NationMember();
			member.Uid = EB.Dot.Long("uid", value, member.Uid);
			member.Name = EB.Dot.String("name", value, member.Name);
			member.JoinTime = EB.Dot.Long("joinTime", value, member.JoinTime);
			member.Level = EB.Dot.Integer("level", value, member.Level);
			int tid = EB.Dot.Integer("tId", value, 10011);/*template_id*/
			var tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(tid);
			if (tpl == null)
			{
				tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(10011);
			}
			var heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tpl.character_id);
			if (heroInfo != null)
			{
				member.Portrait = heroInfo.icon;
			}
			else
			{
				EB.Debug.LogError("cannot found heroInfo for c_id = {0}", tpl.character_id);
			}
			//member.Portrait = EB.Dot.String("portrait", value, "Partner_Head_Sidatuila");//Header_Sidatuilayitong_Main");头像更改
			member.Rank = EB.Dot.String("rank", value, member.Rank);
			member.TotalDonate = EB.Dot.Integer("totalDonate", value, member.TotalDonate);
			member.WeekDonate = EB.Dot.Integer("weekDonate", value, member.WeekDonate);
			member.OfflineTime = EB.Dot.Long("offlineTime", value, member.OfflineTime);
			member.DegreeUpdateTime = EB.Dot.Long("degreeUpdateTime", value, member.DegreeUpdateTime);
			return member;
		}

		public NationMembers()
		{
			ReqMemberRecordDictionary = new Dictionary<string, MemberRequestRecord>();
		}

		public List<NationMember> GetMemberByRank(string rank, bool realData = false)
		{
			List<NationMember> list = new List<NationMember>();
			if (ReqMemberRecordDictionary.ContainsKey(rank))
			{
				list = ReqMemberRecordDictionary[rank].DataList;
			}
			else
			{
				EB.Debug.LogWarning("MemberDictionary not ContainsKey for key:{0}", rank);
			}

			if (realData)
			{
				return list;
			}
			if (list.Count <= 0)
			{
				NationMember emptyData = new NationMember();
				emptyData.Name = EB.Localizer.GetString("ID_codefont_in_NationStruct_17151");
				emptyData.Rank = rank;
				emptyData.WeekDonate = -1;
				emptyData.OfflineTime = -1;
				list.Add(emptyData);
			}
			return list;
		}
	}

	public class TerritoryData
	{
		public int Index;
		public string CityName;
		public string Owner;
		public Dictionary<int, int> LeaderPositionDic;
		public int HP;
		public int MaxHP;
		public string State;
		public string[] CanAttackArr = new string[0];
		public string[] CanDefendArr = new string[0];
		public eTerritoryAttackOrDefendType ADType;
	}

	public class NationTerritoryList : INodeData
	{
		public TerritoryData[] TerritoryList
		{
			get; set;
		}

		public bool Updated;

		public void CleanUp()
		{
			TerritoryList = new TerritoryData[0];
			Updated = false;
		}

		public object Clone()
		{
			return new NationTerritoryList();
		}

		public void OnUpdate(object obj)
		{
			Updated = true;
			TerritoryList = Hotfix_LT.EBCore.Dot.Array<TerritoryData, string>(string.Empty, obj, TerritoryList, ParseMember);
			if (TerritoryList.Length != 10)
				EB.Debug.LogError("nation territory count error");
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		private TerritoryData ParseMember(object value, string name)
		{
			if (value == null)
			{
				return null;
			}
			TerritoryData member = new TerritoryData();
			member.CityName = EB.Dot.String("cn", value, member.CityName);
			member.Owner = EB.Dot.String("o", value, member.Owner);
			member.State = EB.Dot.String("s", value, member.State);
			member.CanAttackArr = Hotfix_LT.EBCore.Dot.Array("ca", value, member.CanAttackArr, delegate (object val) { return val.ToString(); });
			member.CanDefendArr = Hotfix_LT.EBCore.Dot.Array("cd", value, member.CanDefendArr, delegate (object val) { return val.ToString(); });
			int[] leaderDatas = Hotfix_LT.EBCore.Dot.Array("ps", value, null, delegate (object val) { return int.Parse(val.ToString()); });
			// 0 1 2  
			member.LeaderPositionDic = new Dictionary<int, int>();
			int index = 0;
			for (var i = 0; i < leaderDatas.Length; i++)
			{
				var leaderData = leaderDatas[i];

				if (leaderData > 0)
				{
					member.LeaderPositionDic.Add(index, leaderData);
				}
				index++;
			}
			member.HP = EB.Dot.Integer("h", value, member.HP);
			return member;
		}

		public NationTerritoryList()
		{
			TerritoryList = new TerritoryData[0];
		}
	}

	public enum eBattleCellState
	{
		Walk = 0,
		Battling,    //战斗中
		BattleOver   //战斗胜利继续行进
	}

	public enum eBattleDirection
	{
		UpAttack = 0,
		UpDefend,
		MiddleAttack,
		MiddleDefend,
		DownAttack,
		DownDefend
	}

	public class NationBattleTimeConfig : INodeData
	{
		private BattleTime[] TimeConfigArray;
		private System.DateTime EndDateTime;
		private bool HasInitCountDown;
		public string TimeStr;

		public bool InitCountDownTime()
		{
			BattleTime bt = GetCurBattleTime();
			if (bt == null)
			{
				EB.Debug.LogWarning("InitCountDownTime: GetCurBattleTime fail");
				return false;
			}
			EndDateTime = bt.GetEndDateTime();
			HasInitCountDown = true;
			return true;
		}

		public string GetCountDownStr(out bool isEnd)
		{
			isEnd = false;
			if (!HasInitCountDown)
			{
				InitCountDownTime();
				return "";
			}

			if (EndDateTime > EB.Time.LocalNow)
			{
				System.TimeSpan ts = EndDateTime - EB.Time.LocalNow;
				string countDownStr = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
				return countDownStr;
			}
			else
			{
				isEnd = true;
				return "00:00:00";
			}
		}

		public void CleanUp()
		{
			TimeConfigArray = new BattleTime[0];
			HasInitCountDown = false;
		}

		public object Clone()
		{
			return null;
			//return new AllianceBattleConfig();
		}

		public NationBattleTimeConfig()
		{
			TimeConfigArray = new BattleTime[0];
		}

		public void OnUpdate(object obj)
		{
			SetTime(obj);
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void SetTime(object obj)
		{
			TimeConfigArray = Hotfix_LT.EBCore.Dot.Array<BattleTime>("", obj, new BattleTime[0], ParseTime);

			string weekStr = "";
			int startHour = -1, endHour = -1;
			int startMinute = -1, endMinute = -1;
			for (int index = 0; index < TimeConfigArray.Length; ++index)
			{
				BattleTime time_config = TimeConfigArray[index];
				if (startHour == -1)
					startHour = time_config.Start.Hour;
				else if (startHour != time_config.Start.Hour)
				{
					EB.Debug.LogError("nation battle start time inconformity");
				}
				if (endHour == -1)
					endHour = time_config.End.Hour;
				else if (endHour != time_config.End.Hour)
				{
					EB.Debug.LogError("nation battle end time inconformity");
				}

				if (startMinute == -1)
					startMinute = time_config.Start.Minute;
				else if (startMinute != time_config.Start.Minute)
				{
					EB.Debug.LogError("nation battle start time inconformity");
				}
				if (endMinute == -1)
					endMinute = time_config.End.Minute;
				else if (endMinute != time_config.End.Minute)
				{
					EB.Debug.LogError("nation battle end time inconformity");
				}
				/*
				if (time_config.Week == 0)
					weekStr+=EBCore.Localizer.GetString(GameStringValue.CharacterDic[7]);
				else if(time_config.Week>0)
					weekStr+=EBCore.Localizer.GetString(GameStringValue.CharacterDic[time_config.Week]);
				*/

				weekStr += (EB.Localizer.GetString(Hotfix_LT.Data.EventTemplateManager.NumText[time_config.Week]));

				if (index < TimeConfigArray.Length - 1)
				{
					weekStr += ",";
				}
			}
			string start_hour_minute_str = string.Format("{0:00}:{1:00}", startHour, startMinute);
			string end_hour_minute_str = string.Format("{0:00}:{1:00}", endHour, endMinute);

			TimeStr = string.Format(EB.Localizer.GetString("ID_codefont_in_NationStruct_22996"), " " + weekStr, start_hour_minute_str, end_hour_minute_str);
		}

		BattleTime ParseTime(object obj)
		{
			string start_cron_time = EB.Dot.String("start", obj, "");
			string end_cron_time = EB.Dot.String("end", obj, "");
			BattleTime.BattleTimeDefine startDefine = CronToDateTimeDefine(start_cron_time);
			BattleTime.BattleTimeDefine endDefine = CronToDateTimeDefine(end_cron_time);


			if (startDefine.Week != endDefine.Week)
				EB.Debug.LogError("startDefine.Week != endDefine.Week");

			return new BattleTime(startDefine, endDefine);
		}

		private BattleTime GetCurBattleTime()
		{
			for (int i = 0; i < TimeConfigArray.Length; ++i)
			{
				if (TimeConfigArray[i].GetIsMeetTime())   //GetIsAtCurDay
					return TimeConfigArray[i];
			}
			return null;
		}

		public bool GetIsAtBattleTime()
		{
			for (int i = 0; i < TimeConfigArray.Length; ++i)
			{
				if (TimeConfigArray[i].GetIsMeetTime())
					return true;
			}
			return false;
		}

		public bool GetIsBattleStart()
		{
			for (int i = 0; i < TimeConfigArray.Length; ++i)
			{
				if (TimeConfigArray[i].Start.IsAtCurDay && EB.Time.LocalInRange(TimeConfigArray[i].Start.DayTs, TimeConfigArray[i].Start.DayTs + new System.TimeSpan(0, 0, 10)))
					return true;
			}
			return false;
		}

		public bool GetIsBattleStay()
		{
			for (int i = 0; i < TimeConfigArray.Length; ++i)
			{
				if (TimeConfigArray[i].Start.IsAtCurDay && EB.Time.LocalInRange(TimeConfigArray[i].Start.DayTs, TimeConfigArray[i].End.DayTs))
					return true;
			}
			return false;
		}

		public bool GetIsBattleOver()
		{
			BattleTime cur = GetCurBattleTime();
			if (cur == null)
			{
				//EBCore.Debug.LogError("GetCurBattleTime =null");
				return true;
			}
			if (EB.Time.LocalTimeOfDay > System.TimeSpan.FromMinutes(cur.End.Hour * 60 + cur.End.Minute))
				return true;
			return false;
		}

		//0 20 15 * * *前6个字段分别表示：
		//		秒钟：0-59
		//      分钟：0-59
		//      小时：1-23
		//      日期：1-31
		//      月份：1-12
		//      星期：0-6（0表示周日）
		public BattleTime.BattleTimeDefine CronToDateTimeDefine(string str)
		{
			BattleTime.BattleTimeDefine timeDefine = new BattleTime.BattleTimeDefine();
			string[] strs = str.Split(' ');
			if (strs.Length != 6)
				EB.Debug.LogError("cronTable Str Length!=6");

			string secondStr = strs[0], minuteStr = strs[1], hourStr = strs[2], dayStr = strs[3], monthStr = strs[4], weekStr = strs[5];


			int second, minute, hour, day, month, week;
			if (int.TryParse(secondStr, out second))
				timeDefine.Second = second;
			else
			{
				EB.Debug.LogError("timeDefine.Second={0}", secondStr);
				timeDefine.Second = 0;
			}
			if (int.TryParse(minuteStr, out minute))
				timeDefine.Minute = minute;
			else
			{
				EB.Debug.LogError("timeDefine.Minute={0}", minuteStr);
				timeDefine.Minute = 0;
			}
			if (int.TryParse(hourStr, out hour))
				timeDefine.Hour = hour;
			else
			{
				EB.Debug.LogError("timeDefine.Hour={0}", hourStr);
				timeDefine.Hour = 0;
			}
			if (int.TryParse(dayStr, out day))
				timeDefine.Day = day;
			else
				timeDefine.Day = -1;
			if (int.TryParse(monthStr, out month))
				timeDefine.Month = month;
			else
				timeDefine.Month = -1;
			if (int.TryParse(weekStr, out week))
				timeDefine.Week = week;
			else
				timeDefine.Week = -1;

			if (timeDefine.Month >= 0 && timeDefine.Day >= 0 && timeDefine.Week >= 0)
				EB.Debug.LogError("timeDefine.Month > 0 && timeDefine.Day > 0 && timeDefine.Week > 0 s={0}", str);

			timeDefine.DayTs = new System.TimeSpan(hour, minute, second);
			return timeDefine;
		}
	}

	public class BattleCellData
	{
		public long uid;
		public string model;
		public long startTime;
		public float speedMulti;
		public long startBattleTime;
		public long battleTime;
		public eBattleCellState state;
		public long battleUID;
		public eBattleDirection direction;
		public Transform modelTrans;
		public Transform shadowTrans;
		public Transform battleFlagTrans;

		/// <summary>
		/// 用于储存objectmanegr回收时的名字
		/// </summary>
		public string ObjectNmae;

		public override string ToString()
		{
			return string.Format("cellData(uid:{0},state:{1},startMoveTime:{2},dir:{3})", uid, state, moveTime, direction);
		}

		public long moveTime
		{
			get
			{
				long time;
				if (state == eBattleCellState.Battling)
					time = startBattleTime - startTime;
				else if (state == eBattleCellState.BattleOver)
					time = EB.Time.Now - startTime - battleTime;
				else
					time = EB.Time.Now - startTime;
				time = time < 0 ? 0 : time;
				return time;
			}
		}
	}

	public class BattlePathData
	{
		public List<BattleCellData> CacheAttackBattleCellList;
		public List<BattleCellData> CacheDefendBattleCellList;
		public bool AttackDataUpdated;
		public bool DefendDataUpdated;
		public List<BattleCellData> OnceAttackBattleCellList;
		public List<BattleCellData> OnceDefendBattleCellList;

		public BattlePathData()
		{
			CacheAttackBattleCellList = new List<BattleCellData>();
			CacheDefendBattleCellList = new List<BattleCellData>();
			AttackDataUpdated = DefendDataUpdated = false;
			OnceAttackBattleCellList = new List<BattleCellData>();
			OnceDefendBattleCellList = new List<BattleCellData>();
		}

		public void CleanUp()
		{
			CacheAttackBattleCellList.Clear();
			CacheDefendBattleCellList.Clear();
			AttackDataUpdated = DefendDataUpdated = false;
			OnceAttackBattleCellList.Clear();
			OnceDefendBattleCellList.Clear();
		}

		public void CleanOnceUpdateData()
		{
			OnceAttackBattleCellList.Clear();
			OnceDefendBattleCellList.Clear();
		}
	}

	public class StoneThrowerData
	{
		public bool isInit;
		public eBattleDirection dir;
		public string owner;
		public int hp;
		public long endTime;
		public bool isFire;
	}

	public class NationBattleSyncData : INodeData
	{
		private int PreHP;
		public int HP;
		public int Damage;
		public int MaxHP;

		public BattlePathData UpPathData;
		public BattlePathData MiddlePathData;
		public BattlePathData DownPathData;
		eBattleDirection battleDirection;
		public long UpPathActionOverUID;
		public bool UpPathActionOverMeetCity;
		public long MiddlePathActionOverUID;
		public bool MiddlePathActionOverMeetCity;
		public long DownPathActionOverUID;
		public bool DownPathActionOverMeetCity;
		//public bool BuffDataUpdated;
		//public bool IsMainCity;
		//public long ProtectEndTs; //city defend
		//public int KingNum;     //call
		//public int MarshalNum;  //add attr
		//public int GeneralNum;  //reduce cd
		//public List<eNationBattleBuffType> BuffTypeList;
		public bool IsSpeedDataUpdated;
		public long AttackSpeedUPBuffEndTs;
		public long DefendSpeedUPBuffEndTs;
		//public bool SkillDataUpdated;
		public bool UseSkillDataUpdated;
		public bool[] UseSkillDatas;
		public List<StoneThrowerData> StoneThrowerDataList;

		public void CleanUp()
		{
			HP = 0;
			MaxHP = 0;
			PreHP = 0;
			Damage = 0;
			UpPathData.CleanUp();
			MiddlePathData.CleanUp();
			DownPathData.CleanUp();
			UpPathActionOverUID = 0;
			MiddlePathActionOverUID = 0;
			DownPathActionOverUID = 0;
			UpPathActionOverMeetCity = false;
			MiddlePathActionOverMeetCity = false;
			DownPathActionOverMeetCity = false;
			StoneThrowerDataList.Clear();
			//IsMainCity = false;
			//ProtectEndTs = 0;
			//KingNum = 0;
			//MarshalNum = 0;
			//GeneralNum = 0;
			//BuffTypeList.Clear();
			IsSpeedDataUpdated = false;
			AttackSpeedUPBuffEndTs = 0;
			DefendSpeedUPBuffEndTs = 0;
		}

		public NationBattleSyncData()
		{
			UpPathData = new BattlePathData();
			MiddlePathData = new BattlePathData();
			DownPathData = new BattlePathData();
			StoneThrowerDataList = new List<StoneThrowerData>();
			//BuffTypeList = new List<eNationBattleBuffType>();
		}

		public object Clone()
		{
			return new NationBattleSyncData();
		}

		public void OnUpdate(object obj)
		{
			PreHP = HP;
			HP = EB.Dot.Integer("t_info.h", obj, HP);
			if (PreHP > 0)
				Damage = PreHP - HP;
			MaxHP = EB.Dot.Integer("t_info.mh", obj, MaxHP);

			object upathObj_attack = EB.Dot.Object("path.attack.u", obj, null);
			object mpathObj_attack = EB.Dot.Object("path.attack.m", obj, null);
			object dpathObj_attack = EB.Dot.Object("path.attack.d", obj, null);
			object upathObj_defend = EB.Dot.Object("path.defend.u", obj, null);
			object mpathObj_defend = EB.Dot.Object("path.defend.m", obj, null);
			object dpathObj_defend = EB.Dot.Object("path.defend.d", obj, null);
			if (upathObj_attack != null)
			{
				//EBCore.Debug.LogError("receive upathObj_attack msg");
				battleDirection = eBattleDirection.UpAttack;
				ParsePathData(upathObj_attack, UpPathData);
				if (UpPathData.CacheAttackBattleCellList.Count > 0)
					EB.Debug.Log("cachePathData.count:{0} direction:{1}", UpPathData.CacheAttackBattleCellList.Count, battleDirection);
			}
			if (mpathObj_attack != null)
			{
				//EBCore.Debug.LogError("receive mpathObj_attack msg");
				battleDirection = eBattleDirection.MiddleAttack;
				ParsePathData(mpathObj_attack, MiddlePathData);
				if (MiddlePathData.CacheAttackBattleCellList.Count > 0)
					EB.Debug.Log("cachePathData.count:{0} direction:{1}", MiddlePathData.CacheAttackBattleCellList.Count, battleDirection);
			}
			if (dpathObj_attack != null)
			{
				//EBCore.Debug.LogError("receive dpathObj_attack msg");
				battleDirection = eBattleDirection.DownAttack;
				ParsePathData(dpathObj_attack, DownPathData);
				if (DownPathData.CacheAttackBattleCellList.Count > 0)
					EB.Debug.Log("cachePathData.count:{0} direction:{1}", DownPathData.CacheAttackBattleCellList.Count, battleDirection);
			}
			if (upathObj_defend != null)
			{
				//EBCore.Debug.LogError("receive upathObj_defend msg");
				battleDirection = eBattleDirection.UpDefend;
				ParsePathDataDefend(upathObj_defend, UpPathData);
				if (UpPathData.CacheDefendBattleCellList.Count > 0)
					EB.Debug.Log("cachePathData.count:{0} direction:{1}", UpPathData.CacheDefendBattleCellList.Count, battleDirection);
			}
			if (mpathObj_defend != null)
			{
				//EBCore.Debug.LogError("receive mpathObj_defend msg");
				battleDirection = eBattleDirection.MiddleDefend;
				ParsePathDataDefend(mpathObj_defend, MiddlePathData);
				if (MiddlePathData.CacheDefendBattleCellList.Count > 0)
					EB.Debug.Log("cachePathData.count:{0} direction:{1}", MiddlePathData.CacheDefendBattleCellList.Count, battleDirection);
			}
			if (dpathObj_defend != null)
			{
				//EBCore.Debug.LogError("receive dpathObj_defend msg");
				battleDirection = eBattleDirection.DownDefend;
				ParsePathDataDefend(dpathObj_defend, DownPathData);
				if (DownPathData.CacheDefendBattleCellList.Count > 0)
					EB.Debug.Log("cachePathData.count:{0} direction:{1}", DownPathData.CacheDefendBattleCellList.Count, battleDirection);
			}

			//所有的队伍出征结束（战败 或 到终点）
			UpPathActionOverUID = EB.Dot.Long("over.u.uid", obj, 0);
			MiddlePathActionOverUID = EB.Dot.Long("over.m.uid", obj, 0);
			DownPathActionOverUID = EB.Dot.Long("over.d.uid", obj, 0);
			UpPathActionOverMeetCity = EB.Dot.Bool("over.u.isMeetCity", obj, false);
			MiddlePathActionOverMeetCity = EB.Dot.Bool("over.m.isMeetCity", obj, false);
			DownPathActionOverMeetCity = EB.Dot.Bool("over.d.isMeetCity", obj, false);

			ArrayList useSkillDataObj = Hotfix_LT.EBCore.Dot.Array("haveUseSkill", obj, null);
			if (useSkillDataObj != null)
			{
				UseSkillDataUpdated = true;
				UseSkillDatas = Hotfix_LT.EBCore.Dot.Array<bool>("", useSkillDataObj, UseSkillDatas, delegate (object val) { return bool.Parse(val.ToString()); });
			}

			Hashtable stoneThrowerObj = EB.Dot.Object("thrower", obj, null);
			if (stoneThrowerObj != null)
			{
				object ua = EB.Dot.Object("up.attack", stoneThrowerObj, null);
				ParseSTData(ua, eBattleDirection.UpAttack);

				object ud = EB.Dot.Object("up.defend", stoneThrowerObj, null);
				ParseSTData(ud, eBattleDirection.UpDefend);

				object ma = EB.Dot.Object("median.attack", stoneThrowerObj, null);
				ParseSTData(ma, eBattleDirection.MiddleAttack);

				object md = EB.Dot.Object("median.defend", stoneThrowerObj, null);
				ParseSTData(md, eBattleDirection.MiddleDefend);

				object da = EB.Dot.Object("down.attack", stoneThrowerObj, null);
				ParseSTData(da, eBattleDirection.DownAttack);

				object dd = EB.Dot.Object("down.defend", stoneThrowerObj, null);
				ParseSTData(dd, eBattleDirection.DownDefend);
			}

			Hashtable skillData = EB.Dot.Object("skillList", obj, null);
			if (skillData != null)
			{
				IsSpeedDataUpdated = true;
				//AttackSpeedUPBuffEndTs = EB.Dot.Long("atkBuff[2].endTime", skillData, AttackSpeedUPBuffEndTs);
				ArrayList arr1 = Hotfix_LT.EBCore.Dot.Array("atkBuff", skillData, null
					);
				AttackSpeedUPBuffEndTs = EB.Dot.Long("endTime", arr1[2], AttackSpeedUPBuffEndTs);
				ArrayList arr2 = Hotfix_LT.EBCore.Dot.Array("defBuff", skillData, null
					);
				DefendSpeedUPBuffEndTs = EB.Dot.Long("endTime", arr2[2], DefendSpeedUPBuffEndTs);
			}

			//Hashtable buffData = EB.Dot.Object("buffData", obj, null);
			//if (buffData != null)
			//{
			//	BuffDataUpdated = true;
			//	IsMainCity = EB.Dot.Bool("isMainCity", buffData, IsMainCity);
			//	if (IsMainCity)
			//	{
			//		if(!BuffTypeList.Contains(eNationBattleBuffType.fullAoyi))
			//			BuffTypeList.Add(eNationBattleBuffType.fullAoyi);
			//	}
			//	else
			//	{
			//		BuffTypeList.Remove(eNationBattleBuffType.fullAoyi);
			//	}
			//	ProtectEndTs = EB.Dot.Long("protectEndTs", buffData, ProtectEndTs);
			//	if (EBCore.Time.Now<ProtectEndTs)
			//	{
			//		if (!BuffTypeList.Contains(eNationBattleBuffType.cityDefend))
			//			BuffTypeList.Add(eNationBattleBuffType.cityDefend);
			//	}
			//	else
			//	{
			//		BuffTypeList.Remove(eNationBattleBuffType.cityDefend);
			//	}

			//	//KingNum = EB.Dot.Integer("kingNum", buffData, KingNum);
			//	//if (KingNum>0)
			//	//{
			//	//	BuffTypeList.Add(eNationBattleBuffType.call);
			//	//}
			//	//else
			//	//{
			//	//	BuffTypeList.Remove(eNationBattleBuffType.cityDefend);
			//	//}
			//	MarshalNum = EB.Dot.Integer("marshalNum", buffData, MarshalNum);
			//	if (MarshalNum>0)
			//	{
			//		if (!BuffTypeList.Contains(eNationBattleBuffType.addAttr))
			//			BuffTypeList.Add(eNationBattleBuffType.addAttr);
			//	}
			//	else
			//	{
			//		BuffTypeList.Remove(eNationBattleBuffType.addAttr);
			//	}
			//	GeneralNum = EB.Dot.Integer("generalNum", buffData, GeneralNum);
			//	if (GeneralNum > 0)
			//	{
			//		if (!BuffTypeList.Contains(eNationBattleBuffType.reduceTeamCD))
			//			BuffTypeList.Add(eNationBattleBuffType.reduceTeamCD);
			//	}
			//	else
			//	{
			//		BuffTypeList.Remove(eNationBattleBuffType.reduceTeamCD);
			//	}
			//}
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public class BattleCellDataComparer : IComparer<BattleCellData>
		{
			public int Compare(BattleCellData x, BattleCellData y)
			{

				return (int)(x.startTime - y.startTime);  //positive sequence
			}

			public static BattleCellDataComparer Default = new BattleCellDataComparer();
		}

		void ParsePathData(object obj, BattlePathData pathData)
		{
			pathData.AttackDataUpdated = true;
			pathData.OnceAttackBattleCellList = Hotfix_LT.EBCore.Dot.List<BattleCellData, long>(string.Empty, obj, null, ParseCellData);
			pathData.CacheAttackBattleCellList = Hotfix_LT.EBCore.Dot.List<BattleCellData, long>(string.Empty, obj, pathData.CacheAttackBattleCellList, ParseCellData, (member, uid) => member.uid == uid);
			pathData.CacheAttackBattleCellList.Sort(BattleCellDataComparer.Default);
		}

		void ParsePathDataDefend(object obj, BattlePathData pathData)
		{
			pathData.DefendDataUpdated = true;
			pathData.OnceDefendBattleCellList = Hotfix_LT.EBCore.Dot.List<BattleCellData, long>(string.Empty, obj, null, ParseCellData);
			pathData.CacheDefendBattleCellList = Hotfix_LT.EBCore.Dot.List<BattleCellData, long>(string.Empty, obj, pathData.CacheDefendBattleCellList, ParseCellData, (member, uid) => member.uid == uid);
			pathData.CacheDefendBattleCellList.Sort(BattleCellDataComparer.Default);
		}

		private BattleCellData ParseCellData(object value, long uid)
		{
			if (value == null)
			{
				return null;
			}
			BattleCellData cellData = Find(uid) ?? new BattleCellData();
			cellData.uid = EB.Dot.Long("uid", value, cellData.uid);
			cellData.model = EB.Dot.String("m", value, cellData.model);
			if (string.IsNullOrEmpty(cellData.model))
				cellData.model = "P004-Variant";
			cellData.startTime = EB.Dot.Long("st", value, cellData.startTime);
			cellData.speedMulti = EB.Dot.Single("sm", value, cellData.speedMulti);
			cellData.startBattleTime = EB.Dot.Long("sbt", value, cellData.startBattleTime);
			if (cellData.state == eBattleCellState.Battling && cellData.startBattleTime == 0)
				EB.Debug.LogError("cellData.state == eBattleCellState.Battling && cellData.startBattleTime == 0 for cell:{0}", cellData.ToString());
			cellData.battleTime = EB.Dot.Long("ft", value, cellData.battleTime);
			cellData.battleUID = EB.Dot.Long("xuid", value, cellData.battleUID);
			cellData.state = Hotfix_LT.EBCore.Dot.Enum<eBattleCellState>("s", value, cellData.state);
			cellData.direction = battleDirection;

			return cellData;
		}

		//public float GetSpeedMultiple(BattleCellData cellData)
		//{
		//	if (cellData.direction == eBattleDirection.UpAttack || cellData.direction == eBattleDirection.MiddleAttack || cellData.direction == eBattleDirection.DownAttack)
		//	{
		//		return cellData.startTime < AttackSpeedUPBuffEndTs?1.5f:1;
		//	}
		//	else
		//		return cellData.startTime < DefendSpeedUPBuffEndTs?1.5f:1;
		//}

		public void Remove(BattleCellData cellData)
		{
			switch (cellData.direction)
			{
				case eBattleDirection.UpAttack:
					if (!UpPathData.CacheAttackBattleCellList.Remove(cellData))
					{
						EB.Debug.LogError("remove data error for cell:{0}" ,cellData);
					}
					break;
				case eBattleDirection.UpDefend:
					if (!UpPathData.CacheDefendBattleCellList.Remove(cellData))
					{
						EB.Debug.LogError("remove data error for cell:{0}" ,cellData);
					}
					break;
				case eBattleDirection.MiddleAttack:
					if (!MiddlePathData.CacheAttackBattleCellList.Remove(cellData))
					{
						EB.Debug.LogError("remove data error for cell:{0}" ,cellData);
					}
					break;
				case eBattleDirection.MiddleDefend:
					if (!MiddlePathData.CacheDefendBattleCellList.Remove(cellData))
					{
						EB.Debug.LogError("remove data error for cell:{0}" ,cellData);
					}
					break;
				case eBattleDirection.DownAttack:
					if (!DownPathData.CacheAttackBattleCellList.Remove(cellData))
					{
						EB.Debug.LogError("remove data error for cell:{0}" ,cellData);
					}
					break;
				case eBattleDirection.DownDefend:
					if (!DownPathData.CacheDefendBattleCellList.Remove(cellData))
					{
						EB.Debug.LogError("remove data error for cell:{0}" ,cellData);
					}
					break;
			}
		}

		public List<BattleCellData> GetCellDataList(BattleCellData cellData)
		{
			switch (cellData.direction)
			{
				case eBattleDirection.UpAttack: return UpPathData.CacheAttackBattleCellList;
				case eBattleDirection.MiddleAttack: return MiddlePathData.CacheAttackBattleCellList;
				case eBattleDirection.DownAttack: return DownPathData.CacheAttackBattleCellList;
				case eBattleDirection.UpDefend: return UpPathData.CacheDefendBattleCellList;
				case eBattleDirection.MiddleDefend: return MiddlePathData.CacheDefendBattleCellList;
				case eBattleDirection.DownDefend: return DownPathData.CacheDefendBattleCellList;
				default: EB.Debug.LogError("cellData direction err for cell{0}" ,cellData); return new List<BattleCellData>();
			}
		}

		public BattleCellData Find(long uid)
		{
			List<BattleCellData> members;
			switch (battleDirection)
			{
				case eBattleDirection.UpAttack: members = UpPathData.CacheAttackBattleCellList; break;
				case eBattleDirection.MiddleAttack: members = MiddlePathData.CacheAttackBattleCellList; break;
				case eBattleDirection.DownAttack: members = DownPathData.CacheAttackBattleCellList; break;
				case eBattleDirection.UpDefend: members = UpPathData.CacheDefendBattleCellList; break;
				case eBattleDirection.MiddleDefend: members = MiddlePathData.CacheDefendBattleCellList; break;
				case eBattleDirection.DownDefend: members = DownPathData.CacheDefendBattleCellList; break;
				default: members = new List<BattleCellData>(); break;
			}
			BattleCellData member = members.Where(m => m.uid == uid).FirstOrDefault();
			return member;
		}

		void ParseSTData(object obj, eBattleDirection dir)
		{
			if (obj == null)
				return;

			var cachedata = StoneThrowerDataList.Find(m => m.dir == dir);
			if (cachedata != null)
			{
				cachedata.hp = EB.Dot.Integer("hp", obj, cachedata.hp);
				cachedata.owner = EB.Dot.String("tOwner", obj, cachedata.owner);
				cachedata.endTime = EB.Dot.Long("endTime", obj, cachedata.endTime);
				cachedata.isFire = EB.Dot.Bool("isFire", obj, cachedata.isFire);
			}
			else
			{
				int hp = EB.Dot.Integer("hp", obj, 0);
				string owner = EB.Dot.String("tOwner", obj, null);
				long endTime = EB.Dot.Long("endTime", obj, 0);
				bool isFire = EB.Dot.Bool("isFire", obj, false);
				if (hp > 0 && endTime > 0)
				{
					StoneThrowerData data = new StoneThrowerData();
					data.dir = dir;
					data.hp = hp;
					data.owner = owner;
					data.endTime = endTime;
					data.isFire = isFire;
					StoneThrowerDataList.Add(data);
				}
			}
		}
	}
	public class BattleTime
	{
		public class BattleTimeDefine
		{
			public int Hour;
			public int Minute;
			public int Second;
			//public int Year;
			public int Month;
			public int Day;
			public int Week;
			public System.TimeSpan DayTs;

			public bool IsAtCurDay
			{
				get
				{
					if (Month < 0 && Day < 0 && Week < 0)
						return true;
					return (EB.Time.LocalNow.Month == this.Month && EB.Time.LocalNow.Day == this.Day) || EB.Time.LocalWeek == this.Week;
				}
			}
		}

		public BattleTimeDefine Start;
		public BattleTimeDefine End;
		public int Week { get; private set; }

		public BattleTime(BattleTimeDefine start, BattleTimeDefine end)
		{
			this.Start = start;
			this.End = end;
			if (start.Month != end.Month)
				EB.Debug.LogError("start.Month != end.Month");
			Week = this.Start.Week;
		}

		public System.TimeSpan GetStartBattleTimeSpan()
		{
			return new System.TimeSpan(Start.Hour, Start.Minute, Start.Second);
		}

		public System.DateTime GetEndDateTime()
		{
			try
			{
				/*TimerScheduler timerScheduler = new TimerScheduler();
				timerScheduler.m_TimerRegular = string.Format("{0} {1} {2} {3} {4}", End.Minute, End.Hour, End.Day >= 0 ? End.Day.ToString() : "*", End.Month >= 0 ? End.Month.ToString() : "*", End.Week >= 0 ? End.Week.ToString() : "*");
				timerScheduler.ParseTimerRegular();
				if (!timerScheduler.isLegalTimer)
				{
					EBCore.Debug.LogError("GetEndDateTime: cronFormat is illegal");
				}
				System.DateTime date;
				timerScheduler.GetNext(EBCore.Time.LocalNow, out date);
				return date;*/
				return new System.DateTime();
			}
			catch
			{
				EB.Debug.LogError("GetEndDateTime exception");
				return new System.DateTime();
			}
		}

		public bool GetIsAtCurDay()
		{
			return this.Start.IsAtCurDay || this.End.IsAtCurDay;
		}

		private bool GetIsSameDay()
		{
			if (this.Start.Day >= 0)
				return this.Start.Day == this.End.Day;
			else if (this.Start.Week >= 0)
				return this.Start.Week == this.End.Week;
			else
				return true;
		}

		public bool GetIsMeetTime()
		{
			if (GetIsSameDay())
			{
				return GetIsAtCurDay() && EB.Time.LocalInRange(Start.DayTs, End.DayTs);
			}
			else
				return (this.Start.IsAtCurDay && EB.Time.LocalInRange(Start.DayTs, new System.TimeSpan(24, 0, 0))) || (this.End.IsAtCurDay && EB.Time.LocalInRange(new System.TimeSpan(0, 0, 0), End.DayTs)) || GetIsAtWeekRegoin() || GetIsAtDayRegoin();
		}

		public bool GetIsAtEnterTime(System.TimeSpan PrepareTimeTs)
		{
			return GetIsAtCurDay() && EB.Time.LocalInRange(Start.DayTs + new System.TimeSpan(0, 0, 4), Start.DayTs + PrepareTimeTs);
		}

		public bool GetIsAtAdjustFormationTime(System.TimeSpan PrepareTimeTs, System.TimeSpan BattleTimeTs)
		{
			System.TimeSpan endTs = Start.DayTs + BattleTimeTs;
			return GetIsAtCurDay() && EB.Time.LocalInRange(Start.DayTs + PrepareTimeTs, endTs);
		}

		private bool GetIsAtWeekRegoin()
		{
			int cur = EB.Time.LocalWeek;
			return Start.Week < cur && cur < End.Week;
		}

		private bool GetIsAtDayRegoin()
		{
			int cur = EB.Time.LocalNow.Day;
			return Start.Month == End.Month && Start.Day < cur && cur < End.Day;
		}
	}

	public class NationScore
	{
		public string Name;
		public long UpdateTime;
		public int Score;
	}

	public class NationScoreRankList : INodeData
	{
		string DefendNation;
		public List<NationScore> DataList;

		public void CleanUp()
		{
			DataList.Clear();
		}

		public object Clone()
		{
			return new NationScoreRankList();
		}

		public void OnUpdate(object obj)
		{
			ParseData(EB.Dot.Object("persian", obj, null), "persian");
			ParseData(EB.Dot.Object("roman", obj, null), "roman");
			ParseData(EB.Dot.Object("egypt", obj, null), "egypt");
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public NationScoreRankList()
		{
			DataList = new List<NationScore>();
		}

		void ParseData(object obj, string nationName)
		{
			if (nationName == DefendNation)
				return;

			int score = EB.Dot.Integer("score", obj, 0);
			if (score <= 0)
				return;
			NationScore data = DataList.Find(m => m.Name == nationName);
			if (data == null)
			{
				data = new NationScore();
				DataList.Add(data);
			}
			data.Name = nationName;
			data.Score = score;
			data.UpdateTime = EB.Dot.Long("updateTime", obj, data.UpdateTime);
		}

		public void SetDefendNation(string defendNation)
		{
			DefendNation = defendNation;
		}

		public List<NationScore> GetSortedDataList()
		{
			DataList.Sort(delegate (NationScore left, NationScore right)
			{
				if (left.Score != right.Score)
					return right.Score - left.Score;
				return (int)(left.UpdateTime - right.UpdateTime);
			});
			return DataList;
		}
	}
}