using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class LTLegionWarTime
    {
        public int day, hour, minute, second;
        public LTLegionWarTime()
        {
            day = -1; hour = 0; minute = 0; second = 0;
        }
        public LTLegionWarTime(int Day, int Hour, int Minute, int Second)
        {
            day = Day; hour = Hour; minute = Minute; second = Second;
        }
    }

    public class ScoreBoardData
    {
        public string Name;
        public int Rank;
        public int Score;
        public long id;
        public int buffID;
        public int buffLv;
        public int protectTime;
    }

    public class LegionRankData
    {//晋级队伍数据
        public int id;
        public string Name;
        public string Icon;
        public int Rank;
        public int Score;
        public int Stage;
        public bool enter;
    }

    public class FlowEnemyData
    {//预赛敌人
        public string TUid;
        public int Score;
        public string Uid;
        public string IconName;
        public string FrameName;
        public string Name;

        public bool IsFightOut;
    }

    public class FinalPlayerData
    {//团队个人
        public long uid;
        public string IconName;
        public string FrameName;
        public int LadderRank;
        public string Name;
        public bool Dead;
        public int aid;

        public void Parse(object value)
        {
            if (value != null)
            {
                this.uid = EB.Dot.Long("uid", value, this.uid);
                this.LadderRank = EB.Dot.Integer("rank", value, -1) + 1;
                this.Name = EB.Localizer.GetString(EB.Dot.String("name", value, this.Name));
                string IconID = EB.Dot.String("portrait", value, "10010");
                IconID = (IconID == "") ? "10010" : IconID;
                int skin = EB.Dot.Integer("skin", value, 0);
                Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(IconID, skin);
                this.IconName = (heroInfo != null) ? heroInfo.icon : "Partner_Head_Sugeladi";
                string frameStr = EB.Dot.String("headFrame", value, null);
                this.FrameName = EconemyTemplateManager.Instance.GetHeadFrame(frameStr).iconId;
            }

        }
    }

    public class FinalStatusData//各战场状态情况//
    {
        public int FieldNumber;
        public LegionWarField Type;
        public int Status;//0,1,2 开始（需观战），结束，准备中（需隐藏）
        public int[] Aids;
        public int WinAid;
        public int CombatId;
        public List<SimpleLogData> SimpleLogDataList;

        public void Parse(object value)
        {
            if (value != null)
            {
                this.FieldNumber = EB.Dot.Integer("fieldNumber", value, this.FieldNumber);
                int temp = EB.Dot.Integer("type", value, 3);
                this.Type = (LegionWarField)temp;
                this.Status = EB.Dot.Integer("status", value, -1);
                this.Aids = Hotfix_LT.EBCore.Dot.Array<int>("aids", value, this.Aids, delegate (object val) { return int.Parse(val.ToString()); }); ;
                this.WinAid = EB.Dot.Integer("winAid", value, -1);
                this.CombatId = EB.Dot.Integer("battleId", value, 0);
                ArrayList list = Hotfix_LT.EBCore.Dot.Array("simpleLog", value, null);
                if (list != null)
                {
                    SimpleLogDataList = new List<SimpleLogData>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var data = ParseSimpleLogData(list[i]);
                        SimpleLogDataList.Add(data);
                    }
                }
            }
        }

        private SimpleLogData ParseSimpleLogData(object value)
        {
            SimpleLogData data = new SimpleLogData();
            data.type = EB.Dot.String("type", value, string.Empty);
            data.round = EB.Dot.Integer("round", value, -1);
            //data.WinName = EB.Dot.String("winUserName", value, string .Empty);
            //data.LoseName = EB.Dot.String("lostUserName", value, string.Empty);
            data.winUid = EB.Dot.Long("winUid", value, 0);
            data.loseUid = EB.Dot.Long("lostUid", value, 0);
            return data;
        }
    }

    public class SimpleLogData
    {
        public string type;
        public int round;
        //public string WinName;
        //public string LoseName;
        public long winUid;
        public long loseUid;
    }

    public class LegionWarOpenTime
    {
        public LTLegionWarTime[] QualifyOpenTime = new LTLegionWarTime[2];
        public LTLegionWarTime[] SemiOpenTime = new LTLegionWarTime[3];
        public LTLegionWarTime[] FinalOpenTime = new LTLegionWarTime[3];

        public LegionWarOpenTime()
        {
            SetTimeDatas();
        }

        private void SetTimeDatas()
        {
            setTimeData("alliancewarprestart", out QualifyOpenTime[0]);
            setTimeData("alliancewarprestop", out QualifyOpenTime[1]);

            setTimeData("alliancewarsemistart", out SemiOpenTime[0]);
            setTimeData("alliancewarsemistop", out SemiOpenTime[1]);
            setTimeData("alliancewarsemiscombat", out SemiOpenTime[2]);

            setTimeData("alliancewarfinalstart", out FinalOpenTime[0]);
            setTimeData("alliancewarfinalstop", out FinalOpenTime[1]);
            setTimeData("alliancewarfinalcombat", out FinalOpenTime[2]);
        }

        private void setTimeData(string name, out LTLegionWarTime data)
        {
            var temp = new Hotfix_LT.Data.LTActivityTime(Hotfix_LT.Data.EventTemplateManager.Instance.GetCronJobsByName(name).interval);
            data = new LTLegionWarTime(temp.week[0], temp.hour[0], temp.minute, temp.second);
        }
    }

    public class LegionWarTimeData
    {
        // public static LegionWarField TakeTheField = LegionWarField.None;
        public static List<LTLegionWarTime> LegionWarTimes = new List<LTLegionWarTime>
        {
            new LTLegionWarTime { day=0,hour=20, minute=0, second=0 },//预赛0
            new LTLegionWarTime { day=0,hour=21, minute=0, second=0 },//预赛结束1

            new LTLegionWarTime { day=2,hour=20, minute=0, second=0 },//半决赛2-4
            new LTLegionWarTime { day=2,hour=20, minute=30, second=0 },
            new LTLegionWarTime { day=2,hour=21, minute=0, second=0 },//半决赛结束4

            new LTLegionWarTime { day=4,hour=20, minute=0, second=0 },//决赛5-7
            new LTLegionWarTime { day=4,hour=20, minute=30, second=0 },
            new LTLegionWarTime { day=4,hour=21, minute=0, second=0 },//决赛结束7
        };
    }

    public class WatchLogList
    {
        public List<FinalPlayerData> HomeTeam;
        public List<FinalPlayerData> AwayTeam;
        public List<FinalStatusData> AllFinalStatusDataList;

        public WatchLogList()
        {
            HomeTeam = new List<FinalPlayerData>();
            AwayTeam = new List<FinalPlayerData>();
            AllFinalStatusDataList = new List<FinalStatusData>();
        }

        public void OnUpdateFinalStatusData()
        {
            AllFinalStatusDataList.Clear();
            ArrayList battleFieldList;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("allianceWar.semiOrFinalStatus", out battleFieldList);
            if (battleFieldList != null)
            {
                for (int i = 0; i < battleFieldList.Count; i++)
                {
                    FinalStatusData data = new FinalStatusData();
                    data.Parse(battleFieldList[i]);
                    AllFinalStatusDataList.Add(data);
                }
            }
        }

        public void OnUpdateTeamData(int battleField, int type)
        {
            if (HomeTeam != null) HomeTeam.Clear();
            if (AwayTeam != null) AwayTeam.Clear();
            ArrayList battleFieldList;
            DataLookupsCache.Instance.SearchDataByID<ArrayList>("allianceWar.semiField", out battleFieldList);
            if (battleFieldList != null)
            {
                for (int i = 0; i < battleFieldList.Count; i++)
                {
                    if (battleFieldList[i] != null && battleField == i)
                    {
                        ArrayList list = Hotfix_LT.EBCore.Dot.Array("battleField", battleFieldList[i], null);
                        if (list != null)
                        {
                            for (int j = 0; j < list.Count; j++)
                            {
                                if (list[j] != null && type == j)
                                {
                                    var temp = list[j] as Hashtable;
                                    object h_temp = temp["hometeam"];
                                    Hashtable hn_temp = h_temp as Hashtable;
                                    HomeTeam = Hotfix_LT.EBCore.Dot.List<FinalPlayerData, int>(null, hn_temp != null ? hn_temp["playerTeam"] : null, HomeTeam, Parse);
                                    object a_temp = temp["awayteam"];
                                    Hashtable an_temp = a_temp as Hashtable;
                                    AwayTeam = Hotfix_LT.EBCore.Dot.List<FinalPlayerData, int>(null, an_temp != null ? an_temp["playerTeam"] : null, AwayTeam, Parse);
                                    for (int k = HomeTeam.Count; k < 5; k++)
                                    {
                                        HomeTeam.Add(new FinalPlayerData());
                                    }
                                    for (int k = AwayTeam.Count; k < 5; k++)
                                    {
                                        AwayTeam.Add(new FinalPlayerData());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private FinalPlayerData Parse(object value, int id)
        {
            if (value == null)
                return new FinalPlayerData();
            FinalPlayerData item = new FinalPlayerData();
            item.Parse(value);
            return item;
        }

        public FinalStatusData FindFinalStatusData(int Field, LegionWarField type)
        {
            FinalStatusData temp = null;
            for (int i = 0; i < AllFinalStatusDataList.Count; i++)
            {
                if (AllFinalStatusDataList[i].FieldNumber == Field && AllFinalStatusDataList[i].Type == type)
                {
                    temp = AllFinalStatusDataList[i];
                }
            }
            return temp;
        }

        public bool CanShowFieldWatchLog(int teamAid1, int teamAid2)
        {
            if (AllFinalStatusDataList != null && teamAid1 > 0 && teamAid2 > 0)
            {
                for (int i = 0; i < AllFinalStatusDataList.Count; i++)
                {
                    if (AllFinalStatusDataList[i].Aids != null &&
                        AllFinalStatusDataList[i].Aids.Length >= 2 &&
                        AllFinalStatusDataList[i].Aids.Contains(teamAid1) &&
                        AllFinalStatusDataList[i].Aids.Contains(teamAid2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CleanUp()
        {
            if (HomeTeam != null) HomeTeam.Clear();
            if (AwayTeam != null) AwayTeam.Clear();
            AllFinalStatusDataList.Clear();
        }
    }

    public class WarLegionRankList : INodeData
    {
        public WarLegionRankList()
        {
            LegionRankList = new List<LegionRankData>();
        }

        public object Clone()
        {
            return new WarLegionRankList();
        }
        public List<LegionRankData> LegionRankList
        {
            get;
            set;
        }
        public ScoreBoardData MyLegionRank;

        public void CleanUp()
        {
            LegionRankList.Clear();
            MyLegionRank = new ScoreBoardData();
        }

        public void OnUpdate(object obj)
        {
            CleanUp();
            List<LegionRankData> mList = new List<LegionRankData>();
            mList = Hotfix_LT.EBCore.Dot.List<LegionRankData, int>(null, obj, mList, Parse);
            for (int i = 0; i < mList.Count - 1; i++)
            {
                if (mList[i].id == 1) continue;
                mList[i].Rank = i + 1;
                LegionRankList.Add(mList[i]);
            }
            if (mList.Count > 0) MyLegionRank = changeData(mList[mList.Count - 1]);
        }
        private ScoreBoardData changeData(LegionRankData temp)
        {
            ScoreBoardData data = new ScoreBoardData();
            data.Name = temp.Name;
            data.Rank = temp.Rank;
            data.Score = temp.Score;
            data.id = temp.id;
            return data;
        }

        public void OnMerge(object obj)
        {
            CleanUp();
            List<LegionRankData> mList = new List<LegionRankData>();
            mList = Hotfix_LT.EBCore.Dot.List<LegionRankData, int>(null, obj, LegionRankList, Parse, (item, id) => item.id == id);
            for (int i = 0; i < mList.Count - 1; i++)
            {
                if (mList[i].id == 1) continue;
                mList[i].Rank = i + 1;
                LegionRankList.Add(mList[i]);
            }
            if (mList.Count > 0) MyLegionRank = changeData(mList[mList.Count - 1]);
        }

        private LegionRankData Parse(object value, int id)
        {
            if (value == null)
                return null;

            LegionRankData item = new LegionRankData();
            item.id = EB.Dot.Integer("id", value, item.id);
            item.Score = EB.Dot.Integer("score", value, item.Score);
            string name = EB.Dot.String("name", value, item.Name);
            item.Name = string.IsNullOrEmpty(name) ? null : EB.Localizer.GetString(name);
            item.Rank = EB.Dot.Integer("rank", value, item.Rank);
            item.Icon = EB.Dot.String("aIcon", value, null);
            return item;
        }

        public LegionRankData Find(int legionID)
        {
            LegionRankData item = LegionRankList.Where(m => m.id == legionID).FirstOrDefault();
            return item;
        }

        public void Remove(int legionID)
        {
            LegionRankList.RemoveAll(m => m.id == legionID);
        }
    }

    public class WarPersonRankList : INodeData
    {
        public WarPersonRankList()
        {
            PersonRankList = new List<ScoreBoardData>();
            MyPersonRank = new ScoreBoardData();
        }

        public object Clone()
        {
            return new WarPersonRankList();
        }
        public List<ScoreBoardData> PersonRankList
        {
            get; set;
        }
        public ScoreBoardData MyPersonRank;
        public void CleanUp()
        {
            MyPersonRank = new ScoreBoardData();
            PersonRankList.Clear();
        }

        public void OnUpdate(object obj)
        {
            CleanUp();
            List<ScoreBoardData> temp = new List<ScoreBoardData>();
            temp = Hotfix_LT.EBCore.Dot.List<ScoreBoardData, string>(null, obj, temp, Parse);
            for (int i = 0; i < temp.Count - 1; i++)
            {
                temp[i].Rank = i + 1;
                PersonRankList.Add(temp[i]);
            }
            if (temp.Count > 0) MyPersonRank = temp[temp.Count - 1];
        }

        public void OnMerge(object obj)
        {
            CleanUp();
            List<ScoreBoardData> temp = new List<ScoreBoardData>();
            temp = Hotfix_LT.EBCore.Dot.List<ScoreBoardData, string>(null, obj, PersonRankList, Parse, (item, Name) => item.Name == Name);
            for (int i = 0; i < temp.Count - 1; i++)
            {
                temp[i].Rank = i + 1;
                PersonRankList.Add(temp[i]);
            }
            if (temp.Count > 0) MyPersonRank = temp[temp.Count - 1];
        }

        private ScoreBoardData Parse(object value, string Name)
        {
            if (value == null)
                return null;

            ScoreBoardData item = Find(Name) ?? new ScoreBoardData();

            item.Score = EB.Dot.Integer("score", value, item.Score);
            item.Name = EB.Localizer.GetString(EB.Dot.String("name", value, item.Name));
            item.id = EB.Dot.Long("uid", value, 0);
            item.Rank = EB.Dot.Integer("rank", value, 0);
            return item;
        }

        public ScoreBoardData Find(string Name)
        {
            ScoreBoardData item = PersonRankList.Where(m => m.Name == Name).FirstOrDefault();
            return item;
        }

        public void Remove(string Name)
        {
            PersonRankList.RemoveAll(m => m.Name == Name);
        }
    }

    public class FlowEnemyList : INodeData
    {
        public FlowEnemyList()
        {
            FlowEnemyDataList = new List<FlowEnemyData>();
            MyScore = 0;
            awarScoreReward = new List<string>();
        }

        public object Clone()
        {
            return new FlowEnemyList();
        }
        private List<FlowEnemyData> FlowEnemyDataList;
        public int MyScore
        {
            get;
            private set;
        }
        public List<string> awarScoreReward
        {
            get;
            private set;
        }

        public void CleanUp()
        {
            FlowEnemyDataList.Clear();
            MyScore = 0;
            awarScoreReward.Clear();
        }

        public void OnUpdate(object obj)
        {

            Hashtable ha = obj as Hashtable;
            if (ha != null)
            {
                int prescore = MyScore;
                FlowEnemyDataList = Hotfix_LT.EBCore.Dot.List<FlowEnemyData, string>(null, ha["opponents"], FlowEnemyDataList, Parse);
                MyScore = EB.Dot.Integer("score", ha, MyScore);
                if(MyScore != prescore)
                {
                    LTDailyDataManager.Instance.SetDailyDataRefreshState();
                }
                Hashtable a = ha["awarScoreReward"] as Hashtable;
                awarScoreReward.Clear();
                if (a != null)
                {
                    foreach (string v in a.Keys)
                    {
                        awarScoreReward.Add(v);
                    }
                }
            }
        }

        public void OnMerge(object obj)
        {
            Hashtable hash = obj as Hashtable;
            if (hash != null)
            {
                FlowEnemyDataList = Hotfix_LT.EBCore.Dot.List<FlowEnemyData, string>(null, hash["opponents"], FlowEnemyDataList, Parse);
                MyScore = EB.Dot.Integer("score", hash, MyScore);
                Hashtable temp = hash["awarScoreReward"] as Hashtable;
                awarScoreReward.Clear();
                if (temp != null)
                {
                    foreach (string key in temp.Keys)
                    {
                        awarScoreReward.Add(key);
                    }
                }
            }
        }

        public List<FlowEnemyData> GetFlowEnemyDataList()
        {
            FlowEnemyDataList.Sort((a, b) =>
            {
                if (a.Score > b.Score)
                {
                    return 1;
                }
                else if (a.Score < b.Score)
                {
                    return -1;
                }
                else
                {
                    if (long.Parse(a.Uid) > long.Parse(b.Uid))
                        return -1;
                    else
                        return 1;
                }
            });
            return FlowEnemyDataList;
        }

        public FlowEnemyData Find(string uid)
        {
            FlowEnemyData item = FlowEnemyDataList.Where(m => m.Uid == uid).FirstOrDefault();
            return item;
        }

        public void Remove(string uid)
        {
            FlowEnemyDataList.RemoveAll(m => m.Uid == uid);
        }

        private FlowEnemyData Parse(object value, string uid)
        {
            if (value == null)
                return null;

            long Uid = EB.Dot.Long("guid", value, 0);
            FlowEnemyData item = Find(uid) ?? new FlowEnemyData();//预赛敌人读取数据

            item.Uid = EB.Dot.String("guid", value, item.Uid);
            item.TUid = EB.Dot.String("uid", value, item.TUid);
            item.Score = EB.Dot.Integer("point", value, item.Score);
            string IconID = EB.Dot.String("charId", value, "10010");
            IconID = (IconID == "") ? "10010" : IconID;
            int skin = EB.Dot.Integer("skin", value, 0);
            Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(IconID, skin);
            item.IconName = (heroInfo != null) ? heroInfo.icon : "Partner_Head_Sugeladi";
            item.Name = EB.Localizer.GetString(EB.Dot.String("name", value, item.Name));
            item.IsFightOut = EB.Dot.Bool("dead", value, false);
            string frameStr = EB.Dot.String("headFrame", value, null);
            item.FrameName = EconemyTemplateManager.Instance.GetHeadFrame(frameStr).iconId;
            return item;
        }
    }

    public class SemiOrFinalJoinList
    {
        public SemiOrFinalJoinList()
        {
            QualifyLegionRankList = new List<LegionRankData>();
            SemiLegionRankList = new List<LegionRankData>();
            FinalLegionRankList = new List<LegionRankData>();
        }

        public List<LegionRankData> QualifyLegionRankList
        {
            get; set;
        }
        public List<LegionRankData> SemiLegionRankList
        {
            get; set;
        }
        public List<LegionRankData> FinalLegionRankList
        {
            get; set;
        }


        public void CleanUp()
        {
            QualifyLegionRankList.Clear();
            SemiLegionRankList.Clear();
            FinalLegionRankList.Clear();
        }

        public void OnUpdate(object obj, object newObj)
        {
            QualifyLegionRankList.Clear();
            SemiLegionRankList.Clear();
            FinalLegionRankList.Clear();
            QualifyLegionRankList = Hotfix_LT.EBCore.Dot.List<LegionRankData, int>(null, (newObj != null) ? newObj : obj, QualifyLegionRankList, Parse);
            SemiLegionRankList = Hotfix_LT.EBCore.Dot.List<LegionRankData, int>(null, (newObj != null) ? newObj : obj, SemiLegionRankList, Parse1);
            FinalLegionRankList = Hotfix_LT.EBCore.Dot.List<LegionRankData, int>(null, obj, FinalLegionRankList, Parse2);
            FinalLegionRankList.RemoveAll((item) => item == null);
        }

        private LegionRankData Parse(object value, int id)
        {
            if (value == null)
                return null;

            LegionRankData item = new LegionRankData();
            item.id = EB.Dot.Integer("id", value, item.id);
            if (item.id == 1) return null;
            item.Score = EB.Dot.Integer("score", value, item.Score);
            item.Name = EB.Localizer.GetString(EB.Dot.String("name", value, item.Name));
            item.Icon = EB.Dot.String("aIcon", value, null);
            item.Stage = EB.Dot.Integer("stage", value, -1);
            return item;
        }
        private LegionRankData Parse1(object value, int id)
        {
            if (value == null)
                return null;

            LegionRankData item = new LegionRankData();
            item.id = EB.Dot.Integer("id", value, item.id);
            if (item.id == 1) return null;
            item.Score = EB.Dot.Integer("score", value, item.Score);
            item.Name = EB.Localizer.GetString(EB.Dot.String("name", value, item.Name));
            item.Icon = EB.Dot.String("aIcon", value, null);
            item.enter = EB.Dot.Bool("isInFinal", value, false);
            return item;
        }
        private LegionRankData Parse2(object value, int id)
        {
            if (value == null)
                return null;
            if (!EB.Dot.Bool("isInFinal", value, false)) return null;
            LegionRankData item = new LegionRankData();
            item.id = EB.Dot.Integer("id", value, item.id);
            if (item.id == 1) return null;
            item.Score = EB.Dot.Integer("score", value, item.Score);
            item.Name = EB.Localizer.GetString(EB.Dot.String("name", value, item.Name));
            item.Icon = EB.Dot.String("aIcon", value, null);
            item.enter = EB.Dot.Bool("isChampion", value, false);
            return item;
        }

        public LegionRankData FindBase(int legionID)
        {
            if (legionID == 1) return null;
            LegionRankData item = null;
            for (int i = 0; i < QualifyLegionRankList.Count; i++)
            {
                if (QualifyLegionRankList[i] != null && QualifyLegionRankList[i].id == legionID) item = QualifyLegionRankList[i];
            }
            return item;
        }

        public LegionRankData Find(int legionID)
        {
            if (legionID == 1) return null;
            LegionRankData item = null;
            for (int i = 0; i < FinalLegionRankList.Count; i++)
            {
                if (FinalLegionRankList[i] != null && FinalLegionRankList[i].id == legionID) item = FinalLegionRankList[i];
            }
            return item;
        }

        public void Remove(int legionID)
        {
            QualifyLegionRankList.RemoveAll(m => m.id == legionID);
            SemiLegionRankList.RemoveAll(m => m.id == legionID);
            FinalLegionRankList.RemoveAll(m => m.id == legionID);
        }
    }

    public class FinalPlayerList : INodeData
    {

        private int typeCount = 3;
        private LegionWarField parseState;
        public List<FinalPlayerData>[] AllHomeTeam;
        public List<FinalPlayerData>[] AllAwayTeam;
        public LegionWarField MyWarField;
        public FinalPlayerList()
        {
            AllHomeTeam = new List<FinalPlayerData>[typeCount];
            AllAwayTeam = new List<FinalPlayerData>[typeCount];
            MyWarField = LegionWarField.None;
        }

        public object Clone()
        {
            return new SemiOrFinalJoinList();
        }

        public void CleanUp()
        {
            for (int i = 0; i < typeCount; i++)
            {
                if (AllHomeTeam[i] != null) AllHomeTeam[i].Clear();
                if (AllAwayTeam[i] != null) AllAwayTeam[i].Clear();
            }
            MyWarField = LegionWarField.None;
        }

        public void OnUpdate(object obj)
        {
            ArrayList Datas = Hotfix_LT.EBCore.Dot.Array("", obj, null);
            for (int i = 0; i < typeCount; i++)
            {
                if (AllHomeTeam[i] != null) AllHomeTeam[i].Clear();
                if (AllAwayTeam[i] != null) AllAwayTeam[i].Clear();
            }
            MyWarField = LegionWarField.None;
            if (Datas != null)
            {
                for (int i = 0; i < Datas.Count; i++)
                {
                    Hashtable temp = Datas[i] as Hashtable;
                    LTLegionWarManager.Instance.HomeTeamAid = EB.Dot.Integer("hometeam.aid", temp, LTLegionWarManager.Instance.HomeTeamAid);
                    parseState = (LegionWarField)i;
                    object h_temp = temp["hometeam"];
                    Hashtable hn_temp = h_temp as Hashtable;
                    AllHomeTeam[i] = Hotfix_LT.EBCore.Dot.List<FinalPlayerData, int>(null, hn_temp != null ? hn_temp["playerTeam"] : null, AllHomeTeam[i], Parse);//Data["hometeam"]);
                    object a_temp = temp["awayteam"];
                    Hashtable an_temp = a_temp as Hashtable;
                    AllAwayTeam[i] = Hotfix_LT.EBCore.Dot.List<FinalPlayerData, int>(null, an_temp != null ? an_temp["playerTeam"] : null, AllAwayTeam[i], Parse);//Data["awayteam"]
                    for (int j = AllHomeTeam[i].Count; j < 5; j++)
                    {
                        AllHomeTeam[i].Add(new FinalPlayerData());
                    }
                    for (int j = AllAwayTeam[i].Count; j < 5; j++)
                    {
                        AllAwayTeam[i].Add(new FinalPlayerData());
                    }
                }
            }

        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        private FinalPlayerData Parse(object value, int id)
        {
            if (value == null)
                return new FinalPlayerData();
            FinalPlayerData item = new FinalPlayerData();
            item.Parse(value);
            /*item.uid = EB.Dot.Long("uid", value, item.uid);
            item.LadderRank = EB.Dot.Integer("rank", value, -1)+1;
            item.Name = EB.Localizer.GetString(EB.Dot.String("name", value, item.Name));
            string IconID = EB.Dot.String("portrait", value, "10010");
            IconID = (IconID == "") ? "10010" : IconID;
            Hotfix_LT.Data.HeroInfoTemplate heroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(IconID);
            item.IconName = (heroInfo != null) ? heroInfo.icon : "Partner_Head_Sugeladi";*/
            if (item.uid != 0)
            {
                switch (parseState)
                {
                    case LegionWarField.Wind:
                        {
                            if (item.uid ==LoginManager.Instance.LocalUserId.Value) MyWarField = LegionWarField.Wind;
                        }; break;
                    case LegionWarField.Water:
                        {
                            if (item.uid ==LoginManager.Instance.LocalUserId.Value) MyWarField = LegionWarField.Water;
                        }; break;
                    case LegionWarField.Fire:
                        {
                            if (item.uid == LoginManager.Instance.LocalUserId.Value) MyWarField = LegionWarField.Fire;
                        }; break;
                }
            }
            return item;
        }
    }

    public class FinalStatusList : INodeData
    {
        public FinalStatusList()
        {
            AllFinalStatusDataList = new List<FinalStatusData>();
        }

        public object Clone()
        {
            return new FinalStatusList();
        }

        public List<FinalStatusData> AllFinalStatusDataList
        {
            get; set;
        }

        public void CleanUp()
        {
            AllFinalStatusDataList.Clear();
        }

        public FinalStatusData ThisFinalStatusData
        {
            get; set;
        }

        public void OnUpdate(object obj)
        {
            AllFinalStatusDataList = new List<FinalStatusData>();
            AllFinalStatusDataList = Hotfix_LT.EBCore.Dot.List<FinalStatusData, int>(null, obj, AllFinalStatusDataList, Parse);
            Messenger.Raise(EventName.LegionWar_UpdataFinal);
        }

        public void OnMerge(object obj)
        {
            AllFinalStatusDataList = new List<FinalStatusData>();
            AllFinalStatusDataList = Hotfix_LT.EBCore.Dot.List<FinalStatusData, int>(null, obj, AllFinalStatusDataList, Parse);
            Messenger.Raise(EventName.LegionWar_UpdataFinal);
        }
        private FinalStatusData Parse(object value, int id)
        {
            if (value == null)
                return new FinalStatusData();
            FinalStatusData item = new FinalStatusData();
            item.Parse(value);
            /*item.FieldNumber = EB.Dot.Integer("fieldNumber", value, item.FieldNumber);
            int temp= EB.Dot.Integer("type", value, 3);
            item .Type=(LegionWarField)temp;
            item.Status = EB.Dot.Integer("status", value, -1); 
            item.Aids = Hotfix_LT.EBCore.Dot.Array<int>("aids", value, item.Aids, delegate (object val) { return int.Parse(val.ToString()); }); ;
            item.WinAid = EB.Dot.Integer("winAid", value, -1);
            item.CombatId = EB.Dot.Integer("battleId", value, 0);
            item.redPaperEventOpen = EB.Dot.Bool("redPaperEventOpen", value, false);
            if (LTRedPacketHudController.Instance != null&& LTLegionWarManager .Instance .SemiFinalField==item.FieldNumber && LTLegionWarManager.Instance.FieldType == temp)
            {
                LTRedPacketHudController.Instance.redPaperEventOpen = item.redPaperEventOpen;
            }*/
            return item;
        }

        public void Remove(int id)
        {
        }
    }
}
