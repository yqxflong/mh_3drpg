using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class LTLegionWarManager : ManagerUnit
    {
        private static LTLegionWarManager sInstance = null;

        public static LTLegionWarManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<LTLegionWarManager>(); }
        }

        static public string LegionRankListDataId = "allianceWar.allianceRankList";
        public static string QualifyEnemyListDataId = "allianceWar.preList";
        public static string PersonRankListDataId = "allianceWar.personRankList";
        public static string FinalPlayerDataListId = "allianceWar.SemibattleField.battleField";
        public static string FinalStatusListId = "allianceWar.SemibattleField.semiOrFinalStatus";

        public static string SemiOrFinalLegionListId = "allianceWar.semiOrFinalAllianceList";
        public static string SemiOrFinalLegionListNewId = "allianceWar.semiOrFinalAllianceNewList";
        public string LocalSavekey
        {
            get
            {
                return string.Format("legionwarPageTime.{0}", LoginManager.Instance.LocalUserId.Value);
            }
        }
        public WarLegionRankList LegionRankList;//预赛
        public WarPersonRankList PersonRankList;
        public FlowEnemyList QualifyEnemyList;
        private List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> rewardstages;
        private int maxScore;

        public SemiOrFinalJoinList SemiOrFinalLegionList;//半决决赛
        public FinalPlayerList FinalPlayerDataList;
        public FinalStatusList FinalStatusList;

        public WatchLogList WatchLogList;//战报数据
        public bool isInitQualityData;

        /// <summary> 军团战开放时间</summary>
        private LegionWarOpenTime m_warOpenTime;
        public int MaxScore
        {
            get
            {
                if (maxScore == 0)
                {
                    GetQualityRewardList();
                }
                return maxScore;
            }
        }
        public LegionWarOpenTime WarOpenTime
        {
            get
            {
                if (m_warOpenTime == null)
                {
                    m_warOpenTime = new LegionWarOpenTime();
                }
                return m_warOpenTime;
            }
        }
        public DateTime OpenWarTime()
        {
            var world = System.Array.Find(LoginManager.Instance.GameWorlds, w => w.Default);
            if (world.OpenTime > 0)
            {
                DateTime datetime = TaskSystem.TimeSpanToDateTime(world.OpenTime);
                int weeknow = (int)(datetime.DayOfWeek);
                int daydiff = 7 - weeknow;
                DateTime FirstDay = datetime.AddDays(daydiff);
                return FirstDay;
            }
            return Data.ZoneTimeDiff.GetServerTime();
        }
        public bool IsOpenWarTime()
        {
            var world = System.Array.Find(LoginManager.Instance.GameWorlds, w => w.Default);
            if (world.OpenTime > 0)
            {
                DateTime datetime = TaskSystem.TimeSpanToDateTime(world.OpenTime);
                int weeknow = (int)(datetime.DayOfWeek);
                int daydiff = 7 - weeknow;
                DateTime FirstDay = datetime.AddDays(daydiff);
                return EB.Time.Now > EB.Time.ToPosixTime(FirstDay);
            }
            return true;
        }

        public List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> GetQualityRewardList()
        {
            if (rewardstages == null)
            {
                rewardstages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(10001);
                int score = 0;
                for (int i = 0; i < rewardstages.Count; i++)
                {
                    if (rewardstages[i].stage > score)
                    {
                        score = rewardstages[i].stage;
                    }
                }
                maxScore = score;
            }
            return rewardstages;
        }

        public int serveCurState = 0;

        public int HomeTeamAid = 0;

        public int SemiFinalField = 0;//第几战场
        public int FieldType = 0;//战场什么类型

        public override void Initialize(Config config)
        {
            Instance.Api = new LTLegionWarApi();
            Instance.Api.ErrorHandler += ErrorHandler;

            LegionRankList = GameDataSparxManager.Instance.Register<WarLegionRankList>(LegionRankListDataId);
            QualifyEnemyList = GameDataSparxManager.Instance.Register<FlowEnemyList>(QualifyEnemyListDataId);
            PersonRankList = GameDataSparxManager.Instance.Register<WarPersonRankList>(PersonRankListDataId);
            FinalPlayerDataList = GameDataSparxManager.Instance.Register<FinalPlayerList>(FinalPlayerDataListId);
            FinalStatusList = GameDataSparxManager.Instance.Register<FinalStatusList>(FinalStatusListId);

            WatchLogList = new WatchLogList();
            SemiOrFinalLegionList = new SemiOrFinalJoinList();
        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            serveCurState = 0;
            m_warOpenTime = null;
            isInitQualityData = false;
            WatchLogList.CleanUp();
            SemiOrFinalLegionList.CleanUp();
            IsOpenWarTime();
        }

        public override void Connect()
        {
            //State = SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {
            //State = SubSystemState.Disconnected;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public override void Async(string message, object payload)
        {
            switch (message)
            {
                case "alliancePreBattlePlayers"://预赛的推送消息
                    alliancePreBattlePlayers(payload);
                    break;
                case "allianceBattleSemiAndFinalPlayers":
                    allianceBattleSemiAndFinalPlayers(payload);
                    break;
                case "allianceBattleSemiAndFinalResult":
                    ;
                    break;
            }
        }

        private void alliancePreBattlePlayers(object payload)
        {
            Hashtable data = payload as Hashtable;
            Hashtable data1 = data["allianceWarSettlement"] as Hashtable;
            Hashtable winner = data1["winner"] as Hashtable;
            Hashtable loser = data1["loser"] as Hashtable;
            Hashtable winAlliance = data1["winAlliance"] as Hashtable;

            ScoreBoardData winnerData = new ScoreBoardData();
            winnerData.Name = EB.Localizer.GetString(EB.Dot.String("name", winner, string.Empty));
            winnerData.Score = EB.Dot.Integer("score", winner, 0);
            winnerData.id = EB.Dot.Long("uid", winner, 0);
            winnerData.buffID = EB.Dot.Integer("buff.id", winner, 0);
            winnerData.buffLv = EB.Dot.Integer("buff.lv", winner, 0);
            winnerData.protectTime = EB.Dot.Integer("protectTime", loser, 0);

            ScoreBoardData loserData = new ScoreBoardData();
            string nameStr = EB.Dot.String("name", loser, string.Empty);
            loserData.Name = string.IsNullOrEmpty(nameStr) ? nameStr : EB.Localizer.GetString(nameStr);
            loserData.Score = EB.Dot.Integer("score", loser, 0);
            loserData.id = EB.Dot.Long("uid", loser, 0);
            loserData.buffID = EB.Dot.Integer("buff.id", loser, 0);
            loserData.buffLv = EB.Dot.Integer("buff.lv", loser, 0);
            loserData.protectTime = EB.Dot.Integer("protectTime", loser, 0);

            LegionRankData winAllianceData = new LegionRankData();
            winAllianceData.id = EB.Dot.Integer("id", winAlliance, -1);
            winAllianceData.Score = EB.Dot.Integer("score", winAlliance, 0);
            winAllianceData.Name = EB.Localizer.GetString(EB.Dot.String("name", winAlliance, string.Empty));
        }

        private void allianceBattleSemiAndFinalPlayers(object payload)
        {
            Hashtable data = payload as Hashtable;

            Hashtable data1 = data["battleField"] as Hashtable;
            ArrayList data3 = data["semiOrFinalStatus"] as ArrayList;
            LTLegionWarManager.Instance.FinalStatusList.OnUpdate(data3);

            if (Hotfix_LT.EBCore.Dot.Array("battleField", data1, null) != null)
            {
                ArrayList data2 = data1["battleField"] as ArrayList;
                FinalPlayerList pushList = new FinalPlayerList();
                if (LTLegionWarFinalController.Instance != null)
                {
                    pushList.OnUpdate(data2);
                    LTLegionWarFinalController.Instance.SetJoinPeopleCount(pushList);

                    int field = (int)(LTLegionWarFinalController.Instance.WarFiled);
                    if (field >= 0 && field < 3)
                    {

                        for (int i = pushList.AllHomeTeam[field].Count; i < 5; i++)
                        {
                            pushList.AllHomeTeam[field].Add(new FinalPlayerData());
                        }
                        for (int i = pushList.AllAwayTeam[field].Count; i < 5; i++)
                        {
                            pushList.AllAwayTeam[field].Add(new FinalPlayerData());
                        }
                        for (int i = 0; i < 5; i++)
                        {
                            if (pushList.AllHomeTeam[field][i].Name != FinalPlayerDataList.AllHomeTeam[field][i].Name)
                            {
                                FinalPlayerDataList.AllHomeTeam[field][i] = pushList.AllHomeTeam[field][i];
                                if (LTLegionWarFinalController.Legions[0].id == HomeTeamAid)
                                {
                                    LTLegionWarFinalController.Instance.UsFinalItems[i].SetData(pushList.AllHomeTeam[field][i]);
                                    LTLegionWarFinalController.Instance.UsFinalItems[i].StartBootFlash();
                                }
                                else
                                {
                                    LTLegionWarFinalController.Instance.EnemyFinalItems[i].SetData(pushList.AllHomeTeam[field][i]);
                                    LTLegionWarFinalController.Instance.EnemyFinalItems[i].StartBootFlash();
                                }
                            }
                            if (pushList.AllAwayTeam[field][i].Name != FinalPlayerDataList.AllAwayTeam[field][i].Name)
                            {
                                FinalPlayerDataList.AllAwayTeam[field][i] = pushList.AllAwayTeam[field][i];
                                if (LTLegionWarFinalController.Legions[0].id != HomeTeamAid)
                                {
                                    LTLegionWarFinalController.Instance.UsFinalItems[i].SetData(pushList.AllAwayTeam[field][i]);
                                    LTLegionWarFinalController.Instance.UsFinalItems[i].StartBootFlash();
                                }
                                else
                                {
                                    LTLegionWarFinalController.Instance.EnemyFinalItems[i].SetData(pushList.AllAwayTeam[field][i]);
                                    LTLegionWarFinalController.Instance.EnemyFinalItems[i].StartBootFlash();
                                }
                            }
                        }
                    }

                    FinalPlayerDataList.OnUpdate(data2);
                }
            }
        }

        public LTLegionWarApi Api
        {
            get; private set;
        }

        public void GetQualifyWarEnemyList()
        {
            Api.GetQualifyWarEnemyList(FetchDataHandler);
        }

        public void StartChallenge(string tuid, System.Action<bool> callBack = null)
        {
            Api.StartChallenge(tuid, delegate (Hashtable result)
            {
                if (callBack != null) callBack(result != null);
            });
        }

        public void EnterQualifyWar(System.Action<bool> callback = null)
        {
            Api.EnterQualifyWar(delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null) callback(false);
                    return;
                }
                if (callback != null) callback(true);
            });
        }

        public void GetPersonRank(System.Action callback = null)
        {//得到个人榜单列表
            if (callback != null) Api.GetPersonRank(delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null) callback();
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                UpdateQualifyPerData(result);
                if (callback != null) callback();
            });
            else Api.GetPersonRank(FetchDataHandler);
        }

        public void GetLegionRank(System.Action callback = null)
        {//得到团队榜单
            if (callback != null) Api.GetLegionRank(delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null) callback();
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                UpdateQualifyData(result);
                if (callback != null) callback();
            });
            else Api.GetLegionRank(FetchDataHandler);
        }

        public void InitSemiFinal(System.Action callback = null) //initSemi 进入半决赛/决赛初始化军团信息，无需参数
        {
            Api.InitSemifinal(delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null) callback();
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                UpdateLegionData(result);
                WatchLogList.OnUpdateFinalStatusData();
                if (callback != null) callback();
            });
        }

        public void GetQualifyData(System.Action callback)
        {
            if (IsOpenWarTime() && AllianceUtil.IsJoinedAlliance && GetLegionWarStatus() == LegionWarTimeLine.QualifyGame)
            {
                Instance.Api.ErrorRedultFunc = (EB.Sparx.Response response) =>
                {
                    if (response.error != null)
                    {
                        EB.Debug.LogError(response.error);
                        if (response.error.Equals("Error: service not found"))
                        {
                            return true;

                        }
                        else if (response.error.Equals("ID_ALLIANCE_NOT_FOUND"))
                        {
                            return true;
                        }
                    }
                    return false;
                };
                Api.GetQualifyWarEnemyList(delegate (Hashtable data)
                {
                    FetchDataHandler(data);
                    callback();
                    isInitQualityData = true;
                });
            }
            else
            {
                callback();
                isInitQualityData = true;
            }

        }

        private void UpdateLegionData(Hashtable result)
        {
            object Data = null;
            DataLookupsCache.Instance.SearchDataByID<object>(SemiOrFinalLegionListId, out Data);
            object NewData = null;
            DataLookupsCache.Instance.SearchDataByID<object>(SemiOrFinalLegionListNewId, out NewData);

            SemiOrFinalLegionList.OnUpdate(Data, NewData);
        }

        private void UpdateQualifyData(Hashtable result)
        {
            object Data = null;
            DataLookupsCache.Instance.SearchDataByID<object>(LegionRankListDataId, out Data);
            LegionRankList.OnUpdate(Data);
        }

        private void UpdateQualifyPerData(Hashtable result)
        {
            object Data = null;
            DataLookupsCache.Instance.SearchDataByID<object>(PersonRankListDataId, out Data);
            PersonRankList.OnUpdate(Data);
        }

        public void EnterSemiFinalFiled(int FieldNumber, int type)
        {// 点击查看vs军团上阵信息的 参数fieldNumber（战场号int）,type（战场类型int）0 水，1风，2火
            Api.EnterSemiFinalFiled(FieldNumber, type, NewFetchDataHandler);
        }

        public void goIntoBattle(int FieldNumber, int type, int position)
        {
            Api.changeBattleFieldPosition(FieldNumber, type, position, FetchDataHandler);
        }

        public void leaderChangeMemberPosition(int FieldNumber, int type, int position1, int position2)
        {
            Api.leaderChangeMemberPosition(FieldNumber, type, position1, position2, FetchDataHandler);
        }

        public void startSemiOrFinalBattle()
        {
            Api.startSemiOrFinalBattle();
        }

        public void FetchAward(Hashtable data)
        {
            FetchDataHandler(data);
        }

        private void FetchDataHandler(Hashtable data)
        {
            if (data != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(data, false);
            }
            else
                EB.Debug.LogWarning("LegionWar Data is null");
        }

        private void NewFetchDataHandler(Hashtable data)
        {
            if (data != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(data, false);
            }
            else
            {
                EB.Debug.LogWarning("LegionWar Data is null");
                if (LTLegionWarFinalController.Instance != null)
                {
                    LTLegionWarFinalController.Instance.isWarOver = true;
                }
            }
        }

        public void OnOpenAllianceWar(int stage, System.Action callBack = null)
        {
            Api.OnOpenWar(stage, delegate (Hashtable result)
            {
                if (callBack != null) callBack();
            });
        }

        public void OnCloseAllianceWar(int stage, System.Action callBack = null)
        {
            Api.OnCloseWar(stage, delegate
            {
                if (callBack != null) callBack();
            });
        }

        public void LevelAllianceWar()
        {
            Api.LeaveSemiOrFinalFiled();
        }

        public void LevelAlliancePreWar()
        {
            Api.LeavePreFiled();
        }

        #region GM
        public void GetBot()//GM添加机器人
        {
            Api.RequireGMBot(FetchDataHandler);
        }

        public void GetResetWar(System.Action callBack = null)//重置赛程
        {
            Api.ResetWar(delegate
            {
                if (callBack != null) callBack();
            });
        }

        public void debugAutoCombat()
        {
            Api.debugAutoCombat();
        }
        public void GetAddBot(int FieldNumber, int type, int position, string camp, System.Action<Hashtable> dataHandler = null)//GM半决决添加机器人
        {
            Api.AddBotInWar(FieldNumber, type, position, camp, FetchDataHandler);
        }

        public void GetRemoveBot(int FieldNumber, int type, int position, string camp, System.Action<Hashtable> dataHandler = null)//GM半决决减少机器人
        {
            Api.RemoveBotInWar(FieldNumber, type, position, camp, FetchDataHandler);
        }

        public void CheckState(System.Action<bool> callback = null)
        {
            Api.OnCheckWarStage(delegate (Hashtable result)
            {
                if (result == null)
                {
                    serveCurState = 0;
                    if (callback != null)
                    {
                        callback(false);
                    }
                    return;
                }
                DataLookupsCache.Instance.CacheData(result);
                if (callback != null)
                {
                    serveCurState = EB.Dot.Integer("allianceWar.currentStage", result, 0);
                    callback(result != null);
                }
            });
        }
        #endregion

        public void WatchWar(int cambatID)
        {
            CombatManager.Instance.WatchWarRequire(cambatID);
        }

        public void ReceiveBox(string id, System.Action<Hashtable> callback = null)//领取初赛奖励
        {
            Api.OnReceviceBox(id, callback);
        }

        public void ChangeTeams(int index, System.Action callBack = null)
        {
            Api.changeTeam(index, delegate
            {
                if (callBack != null) callBack();
            });
        }

        public void ReceiveRedPacket(int FieldNumber, int type, System.Action<List<LTShowItemData>> callback = null)
        {
            Api.ReceiveRedPacket(FieldNumber, type, delegate (Hashtable result)
            {
                if (result == null)
                {
                    if (callback != null)
                    {
                        callback(null);
                    }
                    return;
                }
                if (callback != null)
                {
                    List<LTShowItemData> list = GetReward(result);
                    callback(list);
                }
            });
        }

        private List<LTShowItemData> GetReward(Hashtable data)
        {
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("allianceWar.receiveRedPaper", data, null);
            List<LTShowItemData> gamList = new List<LTShowItemData>();
            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    string type = EB.Dot.String("type", item, string.Empty);
                    string id = EB.Dot.String("data", item, string.Empty);
                    int num = EB.Dot.Integer("quantity", item, 0);
                    LTShowItemData itemData = new LTShowItemData(id, num, type);
                    gamList.Add(itemData);
                }
            }
            return gamList;
        }

        /// <summary>
        /// 军团战前端状态判断方法
        /// </summary>
        /// <returns></returns>
        public static LegionWarTimeLine GetLegionWarStatus()
        {
            LegionWarTimeLine status = LegionWarTimeLine.none;
            var qualifyOpenTime = LTLegionWarManager.Instance.WarOpenTime.QualifyOpenTime;
            var semiOpenTime = LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime;
            var finalOpenTime = LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime;
            var now = Data.ZoneTimeDiff.GetServerTime();
            var day = (int)(now.DayOfWeek);
            var hour = now.Hour;
            var minute = now.Minute;
            var second = now.Second;
            int start_time1 = qualifyOpenTime[0].hour * 10000 + qualifyOpenTime[0].minute * 100 + qualifyOpenTime[0].second;
            int end_time1 = qualifyOpenTime[1].hour * 10000 + qualifyOpenTime[1].minute * 100 + qualifyOpenTime[1].second;
            int start_time2 = semiOpenTime[0].hour * 10000 + semiOpenTime[0].minute * 100 + semiOpenTime[0].second;
            int end_time2 = semiOpenTime[2].hour * 10000 + semiOpenTime[2].minute * 100 + semiOpenTime[2].second;
            int start_time3 = finalOpenTime[0].hour * 10000 + finalOpenTime[0].minute * 100 + finalOpenTime[0].second;
            int end_time3 = finalOpenTime[2].hour * 10000 + finalOpenTime[2].minute * 100 + finalOpenTime[2].second;
            int now_time = hour * 10000 + minute * 100 + second;
            if ((day == qualifyOpenTime[0].day || qualifyOpenTime[0].day == -1) && now_time >= start_time1 && now_time < end_time1)
            {
                status = LegionWarTimeLine.QualifyGame;
            }
            else if ((day == semiOpenTime[0].day || semiOpenTime[0].day == -1) && now_time >= start_time2 && now_time < end_time2)
            {
                status = LegionWarTimeLine.SemiFinal;
            }
            else if ((day == finalOpenTime[0].day || finalOpenTime[0].day == -1) && now_time >= start_time3 && now_time < end_time3)
            {
                status = LegionWarTimeLine.Final;
            }
            return status;
        }

        /// <summary>
        /// 获取显示待战还是已开战状态的方法，false为待战，true为开战
        /// </summary>
        /// <returns></returns>
        public static bool GetSemiOrFinalWarStatus()
        {
            var semiOpenTime = LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime;
            var finalOpenTime = LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime;
            var now = Data.ZoneTimeDiff.GetServerTime();
            var day = (int)(now.DayOfWeek);
            var hour = now.Hour;
            var minute = now.Minute;
            var second = now.Second;

            //准备阶段的配置时间
            int ConfigTime = (int)Hotfix_LT.Data.AllianceTemplateManager.Instance.GetWarConfigValue("preStartCombatTime");

            int start_time2 = semiOpenTime[0].hour * 10000 + (semiOpenTime[0].minute + ConfigTime) * 100 + semiOpenTime[0].second;
            int end_time2 = semiOpenTime[1].hour * 10000 + semiOpenTime[1].minute * 100 + semiOpenTime[1].second;
            int start_time3 = finalOpenTime[0].hour * 10000 + (finalOpenTime[0].minute + ConfigTime) * 100 + finalOpenTime[0].second;
            int end_time3 = finalOpenTime[1].hour * 10000 + finalOpenTime[1].minute * 100 + finalOpenTime[1].second;
            int now_time = hour * 10000 + minute * 100 + second;
            if (((day == semiOpenTime[0].day || semiOpenTime[0].day == -1) && now_time >= start_time2 && now_time < end_time2)
                || ((day == finalOpenTime[0].day || finalOpenTime[0].day == -1) && now_time >= start_time3 && now_time < end_time3))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

