using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
	public class LadderConfig : INodeData
	{
		private Dictionary<string, int> StageScoreDic = new Dictionary<string, int>();
		public int TimeoutTime;   //超时时间，多少秒
		public int ReceiveAwardNeedMatchNum;
		public int TimingSkipTotalTime;
		public int DeployTotalTime;
		public int EnterBattleTotalTime;
		//public System.TimeSpan EveryDayRefreshTime;

		public void CleanUp()
		{
			StageScoreDic.Clear();
			ReceiveAwardNeedMatchNum = 3;
			TimingSkipTotalTime = 3;
			DeployTotalTime = 20;
			EnterBattleTotalTime = 3;
		}

		public object Clone()
		{
			return new LadderConfig();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
			TimeoutTime = EB.Dot.Integer("timeoutTime", obj, TimeoutTime);  //秒
			ReceiveAwardNeedMatchNum = EB.Dot.Integer("receiveBoxNeedMatchNum", obj, ReceiveAwardNeedMatchNum);
			TimingSkipTotalTime = EB.Dot.Integer("timingSkipTotalTime", obj, TimingSkipTotalTime);
			DeployTotalTime = EB.Dot.Integer("deployTotalTime", obj, DeployTotalTime);
			EnterBattleTotalTime = EB.Dot.Integer("enterBattleTotalTime", obj, EnterBattleTotalTime);
			ArrayList states = Hotfix_LT.EBCore.Dot.Array("stages", obj, null);
			if (states != null)
			{
				for (var i = 0; i < states.Count; i++)
				{
					var stage = states[i];
					Hashtable hash = stage as Hashtable;
					string name = EB.Dot.String("name", hash, string.Empty);
					int score = EB.Dot.Integer("point", hash, 0);

					bool isError = true;
					for (var j = 0; j < GameStringValue.Ladder_Stage_Names.Length; j++)
					{
						string n = GameStringValue.Ladder_Stage_Names[j];
						if (n.Equals(name))
						{
							isError = false;
							break;
						}
					}
					if (isError)
					{
						EB.Debug.LogError("ladder stage name error name={0}" , name);
					}
					else
					{
						if (StageScoreDic.ContainsKey(name))
						{
							StageScoreDic[name] = score;
						}
						else
						{
							StageScoreDic.Add(name, score);
						}
					}
				}
			}
			else
			{
				EB.Debug.LogError("ladder stages obj ==null");
			}
		}

		public LadderConfig()
		{
			CleanUp();
		}

		public int GetStageNeedScore(string name)
		{
			if (StageScoreDic.ContainsKey(name))
			{
				return StageScoreDic[name];
			}
			else
			{
				EB.Debug.LogError("GetStageScore fail name={0}" , name);
				return 0;
			}
		}
	}

	public class LadderInfo : INodeData
	{
		public string Stage { get; set; }
		public string NextStage { get; set; }
		public int Point { get; set; }
		public int NextStageNeedPoint { get; set; }
		public int Rank { get; set; }
		public List<LTShowItemData> EverydayAward;
		public int WinNum { get; set; }
		public int FailNum { get; set; }
		public int TotalNum { get { return WinNum + FailNum; } }
		public int FightNum { get; set; }
		public int LeftRewardNum { get; set; }
		public int EndTs { get; set; }
		public bool HasReceiveEverydayAward { get; set; }

		public void CleanUp()
		{
			EverydayAward = new List<LTShowItemData>();
			Point = 0;
			NextStageNeedPoint = 0;
			Stage = string.Empty;
			NextStage = string.Empty;
			Rank = 0;
			HasReceiveEverydayAward = false;
		}

		public object Clone()
		{
			return new LadderInfo();
		}

		public void OnMerge(object obj)
		{
			OnUpdate(obj);
		}

		public void OnUpdate(object obj)
		{
			Stage = EB.Dot.String("currentStage", obj, Stage);
			NextStage = EB.Dot.String("nextStage", obj, NextStage);
			Point = EB.Dot.Integer("point", obj, Point);
			NextStageNeedPoint = EB.Dot.Integer("nextStageNeedPoint", obj, NextStageNeedPoint);
			Rank = EB.Dot.Integer("rank", obj, Rank);
			EverydayAward = GameUtils.ParseAwardArr(Hotfix_LT.EBCore.Dot.Array("everydayAward", obj, null));
			WinNum = EB.Dot.Integer("winNum", obj, WinNum);
			FailNum = EB.Dot.Integer("failNum", obj, FailNum);
			FightNum = EB.Dot.Integer("fightNum", obj, FightNum);
			LeftRewardNum = EB.Dot.Integer("leftRewardNum", obj, LeftRewardNum);
			EndTs = EB.Dot.Integer("endTs", obj, EndTs);
			HasReceiveEverydayAward = EB.Dot.Bool("hasReceiveEverydayAward", obj, HasReceiveEverydayAward);
		}

		public LadderInfo()
		{
			CleanUp();
		}

		//private void ParseEndTime(string str)
		//{
		//	string[] ss = str.Split('.');
		//	int month, day, hour, minute;
		//	if (!int.TryParse(ss[0], out month))
		//	{
		//		EB.Debug.LogError("ladder strEndTime split error str=" + str);
		//	}
		//	if (!int.TryParse(ss[1], out day))
		//	{
		//		EB.Debug.LogError("ladder strEndTime split error str=" + str);
		//	}
		//	if (!int.TryParse(ss[2], out hour))
		//	{
		//		EB.Debug.LogError("ladder strEndTime split error str=" + str);
		//	}
		//	if (!int.TryParse(ss[3], out minute))
		//	{
		//		EB.Debug.LogError("ladder strEndTime split error str=" + str);
		//	}

		//	StrEndTime=string.Format(EB.Localizer.GetString("ID_TIME_FORMAT"),month,day,string.Format("{0:00}:{1:00}",hour,minute));
		//}
	}
}