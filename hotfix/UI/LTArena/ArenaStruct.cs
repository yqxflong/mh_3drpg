using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
	public class ArenaConfig : INodeData
	{
		public int minLevel { get; set; }
		public string teamName { get; set; }
		public ResourceContainer refreshCooldownCost { get; set; }
		//public ResourceContainer buyCostUtil { get; set; }

		public void CleanUp()
		{
			minLevel = 8;
			teamName = "arena";
			refreshCooldownCost.HC = 10;
			//buyCostUtil.HC = 20;
		}

		public object Clone()
		{
			return new ArenaConfig();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
			minLevel = EB.Dot.Integer("minLevel", obj, minLevel);
			teamName = EB.Dot.String("teamName", obj, teamName);
			refreshCooldownCost = ResourceContainer.Parse(Hotfix_LT.EBCore.Dot.Array("refreshCooldownCost", obj, null));
            //buyCostUtil= ResourceContainer.Parse( EB.Dot.Object("buyCostUtil", obj, null));
        }

        public ArenaConfig()
		{
			refreshCooldownCost = new ResourceContainer();
			//buyCostUtil = new ResourceContainer();
			CleanUp();
		}
	}

	public class ArenaInfo : INodeData
	{
		public int rank { get; set; }
		public int preRank { get; set; }
		public int usedTimes { get; set; }
		public int buyTimes { get; set; }
		public int buyCostHc { get; set; }
		public int buyCost { get; set; }
		//public int maxTimes { get; set; }
		public double nextChallengeTime { get; set; }

		public void CleanUp()
		{
			rank = GetRank();
			preRank = GetRank();
			usedTimes = 0;
			buyTimes = 0;
			//maxTimes = 0;
			nextChallengeTime = 0;
			buyCost = 0;
		}

		public object Clone()
		{
			return new ArenaInfo();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
			preRank = rank;
			rank = EB.Dot.Integer("rank", obj, rank);
			usedTimes = EB.Dot.Integer("usedTimes", obj, usedTimes);
			buyTimes = EB.Dot.Integer("buyTimes", obj, buyTimes);
			buyCost = EB.Dot.Integer("buyCost.quantity", obj, buyCost);
            //maxTimes =  EB.Dot.Integer("maxTimes", obj, maxTimes);
            nextChallengeTime = EB.Dot.Double("nextChallengeTime", obj, nextChallengeTime);
		}

		public ArenaInfo()
		{
			CleanUp();
		}

		public void SaveRank()
		{
			PlayerPrefs.SetInt(LoginManager.Instance.LocalUserId.Value + "ArenaRank", rank);
		}

		private int GetRank()
		{
			return PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value + "ArenaRank", -1);
		}
	}

	public class ArenaChallenger : INodeData
	{
		public long uid { get; set; }
		public string name { get; set; }
		public int rank { get; set; }
		public int level { get; set; }
		public string allianceName { get; set; }
		public int charId { get; set; }
		public int skin { get; set; }
        public string frame { get; set; }
        public void CleanUp()
		{
			uid = 0;
			name = frame = string.Empty;
			rank = 0;
			level = 0;
			allianceName = "";
			skin = 0;
		}

		public object Clone()
		{
			return new ArenaChallenger();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
            uid = EB.Dot.Long("uid", obj, uid);
            name = EB.Dot.String("name", obj, name);
            rank = EB.Dot.Integer("rank", obj, rank);
            level = EB.Dot.Integer("level", obj, level);
            allianceName = EB.Dot.String("allianceName", obj, "");
            charId = EB.Dot.Integer("charId", obj, charId);
            skin = EB.Dot.Integer("skin", obj, 0);
            frame = EB.Dot.String("headFrame", obj, "0_0");
        }

		public ArenaChallenger()
		{
			CleanUp();
		}
	}

	public class ArenaBattleLog
	{
		public enum eChallengeType
		{
			Invalid,
			Challenge,
			Defend
		}

		public string id { get; set; }
		public eChallengeType type { get; set; }
		public string opponentName { get; set; }
		public int opponentTpl { get; set; }
		public int opponentCharID { get; set; }
		public int opponentSkin { get; set; }
        public string opponentFrame { get; set; }
        public int opponentLevel { get; set; }
		public int rankChange { get; set; }
		public bool isWon { get; set; }
		public double occurTime { get; set; }
		public bool isChallenge { get; set; }
		public string icon { get; set; }

		public void CleanUp()
		{
			id = string.Empty;
			type = eChallengeType.Invalid;
			opponentName=icon = string.Empty;
			opponentTpl = 0;
			opponentCharID = 0;
			rankChange = 0;
			isWon = false;
			occurTime = 0;
			opponentSkin = 0;
            opponentFrame = null;
        }

		public ArenaBattleLog()
		{
			CleanUp();
		}
	}

	public class ArenaHightestInfo
	{
		public int CurrentRank;
		public int HighestRecord;
		public ResourceContainer RankAward;

		public ArenaHightestInfo()
		{
			CurrentRank = 0;
			HighestRecord = 0;
			RankAward = new ResourceContainer();
		}

		public void SetData(object obj)
		{
			CurrentRank = EB.Dot.Integer("arena.highestInfo.currentRank", obj, 0);
			HighestRecord = EB.Dot.Integer("arena.highestInfo.highestRecord", obj, 0);
			RankAward = ResourceContainer.Parse(Hotfix_LT.EBCore.Dot.Array("arena.highestInfo.rankAward", obj, null));
		}
	}

	public class ArenaBattleLogComparer : IComparer<ArenaBattleLog>
	{
		public int Compare(ArenaBattleLog x, ArenaBattleLog y)
		{
			double diff = x.occurTime - y.occurTime;
			if (diff > 0)
			{
				return -1;
			}
			else if (diff < 0)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}

	public class ArenaBattleLogs : INodeData
	{
		public List<ArenaBattleLog> logs { get; private set; }

		public void CleanUp()
		{
			logs.Clear();
		}

		public object Clone()
		{
			return new ArenaBattleLogs();
		}

		public void OnMerge(object obj)
		{
			logs = Hotfix_LT.EBCore.Dot.List<ArenaBattleLog, string>(string.Empty, obj, logs, ParseLog, (log, id) => log.id == id);
		}

		public void OnUpdate(object obj)
		{
			logs = Hotfix_LT.EBCore.Dot.List<ArenaBattleLog, string>(string.Empty, obj, logs, ParseLog);
		}

		public ArenaBattleLogs()
		{
			logs = new List<ArenaBattleLog>();
		}

		private ArenaBattleLog ParseLog(object value, string id)
		{
			if (value == null)
			{
				return null;
			}

			ArenaBattleLog log = new ArenaBattleLog();
			log.id = EB.Dot.String("id", value, log.id);
			log.opponentName = EB.Dot.String("opponentName", value, log.opponentName);
			log.opponentTpl = EB.Dot.Integer("opponentTpl", value, log.opponentTpl);
			log.opponentCharID = EB.Dot.Integer("opponentCharID", value, 15010);
			log.opponentSkin = EB.Dot.Integer("skin", value, 0);
            log.opponentFrame = EB.Dot.String("headFrame", value, null);
            log.opponentLevel = EB.Dot.Integer("opponentLevel", value, log.opponentLevel);
            if ((value as Hashtable).ContainsKey("icon"))
            {
	            log.icon = EB.Dot.String("icon", value, log.icon);
            }
            //角斗场是排名变化  荣耀角斗场是积分变化
            if ((value as Hashtable).ContainsKey("rankChange"))
            {
	            log.rankChange = EB.Dot.Integer("rankChange", value, log.rankChange);
            }
            else if ((value as Hashtable).ContainsKey("pointChange"))
            {
	            log.rankChange = EB.Dot.Integer("pointChange", value, log.rankChange); 
            }
			string t = EB.Dot.String("type", value, string.Empty);
			if (t.CompareTo("challenge") == 0) log.type = ArenaBattleLog.eChallengeType.Challenge;
			else if (t.CompareTo("defend") == 0) log.type = ArenaBattleLog.eChallengeType.Defend;
			else log.type = ArenaBattleLog.eChallengeType.Invalid;
			log.isWon = EB.Dot.Bool("isWon", value, log.isWon);
			log.occurTime = EB.Dot.Double("occurTime", value, log.occurTime);
			log.isChallenge = EB.Dot.String("type", value, string.Empty).CompareTo("challenge") == 0;

			return log;
		}
	}
}