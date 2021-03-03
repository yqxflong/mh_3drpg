using System.Collections;
using EB.Sparx;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class ArenaModelData
    {
        public long Uid;
        public string UName;
        public string AName;
        public string LeaderId;
    }

    public class ArenaManager : ManagerUnit
    {
        public SubSystemState State { get; set; }
        public const string ArenaConfigDataId = "arena.configs";
        public const string ArenaChallengeListDataId = "arena.challengeList[]";
        public const string ArenaInfoDataId = "arena.info";
        public const string ArenaBattleLogsDataId = "arena.battleLog";
        public const string ArenaModelDataId = "arena.model";

        private static ArenaManager sInstance = null;
        public static ArenaManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<ArenaManager>(); }
        }

        public ArenaAPI Api
        {
            get; private set;
        }

        public ArenaConfig Config
        {
            get; private set;
        }

        public ArenaInfo Info
        {
            get; private set;
        }

        public ArenaBattleLogs Logs
        {
            get; private set;
        }

        public ArenaHightestInfo HightestInfo
        {
            get; private set;
        }

        public ArenaChallenger Challenger
        {
            get; private set;
        }

        public const double LifeTime = 5 * 60;

        public double Timeout
        {
            get; set;
        }

        public bool ArenaRP()
        {
             FuncTemplate m_FuncTpl = FuncTemplateManager.Instance.GetFunc(10018);
            if (m_FuncTpl.IsConditionOK())
            {
                ArenaInfo info = ArenaManager.Instance.Info;
                int totalTimes = VIPTemplateManager.Instance.GetTotalNum(VIPPrivilegeKey.ArenaTimes) + info.buyTimes;
                return totalTimes - info.usedTimes > 0;
            }
            else
            {
                return false;
            }
        }

        public ArenaModelData GetArenaModelData()
        {
            ArenaModelData data = new ArenaModelData();
            Hashtable table;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>(ArenaModelDataId, out table);
            data.Uid = EB.Dot.Long("uid", table, 0);
            data.UName = EB.Dot.String("name", table, null);
            data.AName = EB.Dot.String("aname", table, null);
            data.LeaderId = EB.Dot.String("templateId", table, null);
            return data;
        }

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new ArenaAPI();
            Instance.Api.ErrorHandler += ErrorHandler;

            Config = GameDataSparxManager.Instance.Register<ArenaConfig>(ArenaConfigDataId);
            Info = GameDataSparxManager.Instance.Register<ArenaInfo>(ArenaInfoDataId);
            Logs = GameDataSparxManager.Instance.Register<ArenaBattleLogs>(ArenaBattleLogsDataId);
            Challenger = GameDataSparxManager.Instance.Register<ArenaChallenger>(ArenaChallengeListDataId);
        }

        public override void OnLoggedIn()
        {
            Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;

            object arena = loginData["arena"];
            if (arena == null)
            {
                EB.Debug.LogWarning("ArenaManager.OnLoggedIn: arena not found in LoginData");
                return;
            }
            loginData.Remove("arena");
        }

        public bool IsHightestRecordChanged = false;
        public bool IsRankChanged;
        public override void Async(string message, object payload)
        {
            switch (message)
            {
                case "highestChange":
                    break;
                case "rank_change":
                    {
                        if (Info.rank > Info.preRank && Info.preRank >= 0)
                        {
                            IsRankChanged = true;
                            Messenger.Raise(Hotfix_LT.EventName.ArenaRankChangeEvent);
                        }
                    }
                    break;
                case "newBattleLog":
                    {
                        if (Info.rank > Info.preRank && Info.preRank >= 0)
                        {
                            IsRankChanged = true;
                            Messenger.Raise(Hotfix_LT.EventName.ArenaRankChangeEvent);
                        }
                    }
                    break;
            }
        }

        public bool UpdateArenaTeam()
        {
            int num = 0;
            Hashtable userTeamData;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("userTeam", out userTeamData);
            ArrayList teamHash = Hotfix_LT.EBCore.Dot.Array("arena.formation_info", userTeamData, null);
            for (var i = 0; i < teamHash.Count; i++)
            {
                var teamMemData = teamHash[i];
                IDictionary teamMemDataDic = teamMemData as IDictionary;
                if (teamMemDataDic == null || !teamMemDataDic.Contains(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID) || teamMemDataDic[SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID] == null)
                    continue;
                int nHeroID = EB.Dot.Integer(SmallPartnerPacketRule.USER_TEAM_FORMATION_HERO_ID, teamMemDataDic, 0); ;
                if (nHeroID <= 0) continue;
                num++;
            }
            int ownNum = LTPartnerDataManager.Instance.GetOwnPartnerNum();
            return num < 4 && ownNum > num;
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

            if (Info.rank > Info.preRank)
            {
                IsRankChanged = true;
                Messenger.Raise(Hotfix_LT.EventName.ArenaRankChangeEvent);
            }
        }

        private void MergeDataHandler(Hashtable payload)
        {
            if (payload != null)
            {
                if (payload["playstate"] != null)
                {
                    payload.Remove("arena");
                    DataLookupsCache.Instance.CacheData(payload);
                }
                GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
            }
        }

        public void GetInfo()
        {
            Api.GetInfo(FetchDataHandler);
        }

        public void StartChallenge(long uid, int rank)
        {
            Api.errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "ID_ERROR_ARENA_DEFENDER_INPROGRESS":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ArenaManager_5645"));
                                return true;
                            }
                        case "not in challenge list":
                        case "ID_ERROR_CHALLENGE_NOT_IN_RANK":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_NOT_IN_RANK"));
                                RefreshChallengers();
                                return true;
                            }
                        case "time limit":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_TIME_LIMIT"));
                                return true;
                            }
                        case "ID_ERROR_NOT_IN_LEADERBOARD":
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_NOT_IN_RANK"));
                                return true;
                            }
                    }
                }
                return false;
            };
            Api.StartChallenge(uid, rank, MergeDataHandler);
        }
        
        public void fastChallenge(long uid, int rank,bool fast=false)
        {
            Api.errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "ID_ERROR_ARENA_DEFENDER_INPROGRESS":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_ArenaManager_5645"));
                            return true;
                        }
                        case "not in challenge list":
                        case "ID_ERROR_CHALLENGE_NOT_IN_RANK":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_NOT_IN_RANK"));
                            RefreshChallengers();
                            return true;
                        }
                        case "time limit":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_TIME_LIMIT"));
                            return true;
                        }
                        case "ID_ERROR_NOT_IN_LEADERBOARD":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_CHALLENGE_NOT_IN_RANK"));
                            return true;
                        }
                    }
                }
                return false;
            };
            Api.StartChallenge(uid, rank, (payload) =>
            {
                if (fast) DataLookupsCache.Instance.CacheData("combat.rewards", null);
                DataLookupsCache.Instance.CacheData(payload);
                GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
            },true);
        }

        public void RefreshChallengers()
        {
            Api.RefreshChallenge(MergeDataHandler);
        }

        public void SaveTeam(long battleRating)
        {
            Api.SaveTeam(battleRating, MergeDataHandler);
        }

        public void RefreshCooldown(System.Action<bool> callback)
        {
            Api.RefreshCooldown(delegate (Hashtable payload)
            {
                if (payload == null)
                {
                    callback(false);
                }
                else
                {
                    MergeDataHandler(payload);
                    callback(true);
                }
            });
        }

        public void BuyChallengeTimes(System.Action<bool> callback)
        {
            Api.BuyChallengeTimes(delegate (Hashtable payload)
            {
                if (payload == null)
                {
                    callback(false);
                }
                else
                {
                    MergeDataHandler(payload);
                    callback(true);
                }
            });
        }

        public override void Connect()
        {
            State = EB.Sparx.SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {

        }

        public override void Dispose()
        {
            Info.SaveRank();
        }
    }

}