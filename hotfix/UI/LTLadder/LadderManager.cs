using EB.Sparx;

namespace Hotfix_LT.UI
{
    using System.Collections;

    public class LadderManager : ManagerUnit
    {
        public const string LadderConfigDataId = "ladder.configs";
        public const string LadderInfoDataId = "ladder.info";
        public const string LadderBattleLogsDataId = "ladder.battleLog";
        private static LadderManager sInstance = null;
        public static LadderManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<LadderManager>(); }
        }

        public LadderAPI Api
        {
            get; private set;
        }

        public LadderConfig Config
        {
            get; private set;
        }

        public LadderInfo Info
        {
            get; private set;
        }
        
        public ArenaBattleLogs Logs
        {
            get; private set;
        }

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new LadderAPI();
            Instance.Api.ErrorHandler += ErrorHandler;

            Config = GameDataSparxManager.Instance.Register<LadderConfig>(LadderConfigDataId);
            Info = GameDataSparxManager.Instance.Register<LadderInfo>(LadderInfoDataId);
            Logs = GameDataSparxManager.Instance.Register<ArenaBattleLogs>(LadderBattleLogsDataId);
        }

        public override void OnLoggedIn()
        {
            IsTrusteeship = false;
            Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;

            object ladder = loginData["ladder"];
            if (ladder == null)
            {
                EB.Debug.LogWarning("LadderManager.OnLoggedIn: ladder not found in LoginData");
                return;
            }
            loginData.Remove("ladder");
        }

        public override void Async(string message, object payload)
        {
            base.Async(message, payload);

            switch (message)
            {
                case "match_suc":
                    EB.Coroutines.Run(CountdownSkipTime());
                    GlobalMenuManager.Instance.Open("LTMatchSuccessUI", payload);
                    break;
                case "prepare_suc":
                    OtherPrepareOK = true;
                    break;
                case "MatchTimeOut":
                    if (LadderController.Instance != null) LadderController.Instance.StopMatch();
                    break;
            }
        }
        
        /// <summary>托管布尔</summary>
        public bool IsTrusteeship {
            get;
            private set;
        }
        public bool IsTrusteeshiping()
        {
            if (IsTrusteeship)MessageTemplateManager.ShowMessage(eMessageUIType .FloatingText ,EB .Localizer .GetString("ID_LADDER_TRUSTEESHIP_TIP"));
            return IsTrusteeship;
        }
        public void OpenOrCloseTrusteeship(bool isOpen = false)
        {
            string tip;
            IsTrusteeship = isOpen;
            if (IsTrusteeship)
            {
                tip = EB.Localizer.GetString("ID_LADDER_TRUSTEESHIP_OPEN_TIP");
            }
            else
            {

                tip = EB.Localizer.GetString("ID_LADDER_TRUSTEESHIP_CLOSE_TIP");
            }
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, tip);
        }



        public bool ReachSkipTime;
        IEnumerator CountdownSkipTime()
        {
            ReachSkipTime = false;
            yield return new UnityEngine.WaitForSeconds(LadderManager.Instance.Config.TimingSkipTotalTime);
            ReachSkipTime = true;

            StartBattleTimer();
            yield break;
        }

        public int DeployTimer;
        public int EnterBattleTimer;
        public int WaitForActionTimer;
        public bool IPrepareOK, OtherPrepareOK;
        public void ResetBattleTimerData()
        {
            DeployTimer = Config.DeployTotalTime;
            EnterBattleTimer = Config.EnterBattleTotalTime;
            IPrepareOK = false;
            OtherPrepareOK = false;

            EB.Coroutines.Stop(Countdown());
        }

        public void StartBattleTimer()
        {
            EB.Coroutines.Run(Countdown());
        }

        IEnumerator Countdown()
        {
            while (true)
            {
                yield return new UnityEngine.WaitForSeconds(1);
                if ((!IPrepareOK || !OtherPrepareOK) && DeployTimer > 0)
                    DeployTimer--;
                else
                {
                    IPrepareOK = OtherPrepareOK = true;
                    if (EnterBattleTimer > 0)
                        EnterBattleTimer--;
                    else
                        yield break;
                }
            }
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        public void GetInfo(System.Action<bool> callback=null)
        {
            Api.GetInfo(delegate (Hashtable result)
            {
                if (result != null)
                    FetchDataHandler(result);
                if(callback!=null) callback(result != null);
            });
        }

        public void StartMatch(System.Action<bool> callback)
        {
            var partnerList = LTPartnerDataManager.Instance.GetOwnPartnerListSortByPowerData();
            ArrayList heroes = Johny.ArrayListPool.Claim();
            for (int i = 0; i < 6; ++i)
            {
                if (partnerList[i].HeroId > 0)
                {
                    Hashtable hero = Johny.HashtablePool.Claim();
                    hero.Add("tid", partnerList[i].HeroStat.id.ToString());
                    hero.Add("br", partnerList[i].powerData.curPower);
                    heroes.Add(hero);
                }
            }

            Api.StartMatch(delegate (Hashtable result)
            {
                if (result != null)
                    FetchDataHandler(result);
                callback(result != null);
            }, heroes);
        }

        public void CancelMatch()
        {
            Api.CancelMatch(FetchDataHandler);
        }

        public void ReceiveEveryAward(System.Action<bool> callback)
        {
            Api.ReceiveEveryAward(delegate (Hashtable result)
            {
                if (result != null)
                    DataLookupsCache.Instance.CacheData(result);
                callback(result != null);
            });
        }

        public void GiveUp(System.Action<bool> callback)
        {
            Api.GiveUp(delegate (Hashtable result)
            {
                if (result != null)
                    DataLookupsCache.Instance.CacheData(result);
                callback(result != null);
            });
        }

        public void Prepare()
        {
            Api.Prepare(FetchDataHandler);
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
                GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
            }
        }
    }

}