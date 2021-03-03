using System;
using System.Collections;
using System.Collections.Generic;
using EB;

namespace Hotfix_LT.UI
{
    public class HonorArenaManager: ManagerUnit
    {
        public const string HonorArenaChallengeListDataId = "honorarena.challengeList[]";
        public const string HonorArenaInfoDataId = "honorarena.info";
        public const string ArenaBattleLogsDataId = "honorarena.battleLog";
        
        private static HonorArenaManager sInstance = null;
        public static HonorArenaManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<HonorArenaManager>(); }
        }
        
        public HonorArenaChallenger Challenger
        {
            get; private set;
        }
        
        public HonorArenaChallenger CurrentChallenger
        {
            get;set;
        }
        
        public HonorArenaInfo Info
        {
            get; private set;
        }
        
        public ArenaBattleLogs Logs
        {
            get; private set;
        }

        public HonorArenaAPI Api
        {
            get; private set;
        }
        
        private int allCombatPower;
        public int AllCombatPower
        {
            get
            {
                if (allCombatPower<=0)
                {
                    return setBR();
                }
                else
                {
                    return allCombatPower;
                }
            }
        }
        
        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new HonorArenaAPI();
            Instance.Api.ErrorHandler += ErrorHandler;
            
            Challenger = GameDataSparxManager.Instance.Register<HonorArenaChallenger>(HonorArenaChallengeListDataId);
            Info = GameDataSparxManager.Instance.Register<HonorArenaInfo>(HonorArenaInfoDataId);
            Logs = GameDataSparxManager.Instance.Register<ArenaBattleLogs>(ArenaBattleLogsDataId);

        }

        public override void Disconnect(bool isLogout)
        {
            base.Disconnect(isLogout);
            Info.CleanUp();
            allCombatPower = 0;
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }
        
        
        private void MergeDataHandler(Hashtable payload)
        {
            if (payload != null)
            {
                // if (payload["playstate"] != null)
                // {
                //     payload.Remove("arena");
                //     DataLookupsCache.Instance.CacheData(payload);
                // }
                GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
            }
        }

        private bool _isStartChallengeRequesting = false;

        public void StartChallenge(int index,bool fast)
        {
            if (_isStartChallengeRequesting)
            {
                return;
            }

            _isStartChallengeRequesting = true;

            Api.errorProcessFun = (EB.Sparx.Response response) =>
            {
                _isStartChallengeRequesting = false;

                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "maxtimes":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTResourceInstanceHudController_6819"));
                            return true;
                        }
                    }
                }
                return false;
            };

            Api.StartChallenge(index, (payload) =>
            {
                DataLookupsCache.Instance.CacheData(payload);
                GameDataSparxManager.Instance.ProcessIncomingData(payload, true);
                _isStartChallengeRequesting = false;              
            },fast);
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
        
        public void GetInfo()
        {
            Api.GetInfo(MergeDataHandler);
        }
        
        public void RefreshChallengers(Action callback=null)
        {
            Api.RefreshChallenge((data) =>
            {
                MergeDataHandler(data);
                callback?.Invoke();
            });
        }
        
        public int setBR(Action<Hashtable> callback=null)
        {
            int allPower = 0;
            for (int i = 0; i < HonorConfig.teamName2.Length; i++)
            {
                List<TeamMemberData> temp= LTFormationDataManager.Instance.GetTeamMemList(HonorConfig.teamName2[i]);
                for (int j = 0; j < temp.Count; j++)
                {
                    LTPartnerData par = LTPartnerDataManager.Instance.GetPartnerByHeroId(temp[j].HeroID);
                    if (par != null)
                    {
                        allPower += par.powerData.curPower;
                    }
                }
            }
            Api.setBR(allPower,callback);
            if (allCombatPower!=allPower)
            {
                Messenger.Raise(EventName.HonorCombatTeamPowerUpdate,allPower);
                allCombatPower = allPower;
            }
            return allPower;
        }

        public int GetHonorArenaFreeTimes()
        {
            int times = (int)Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("HonorArenaFreeTimes");
            return times;
        }

        public void GetOneHourReward(Action callback)
        {
            Api.GetOneHourReward((data) =>
            {
                MergeDataHandler(data);
                callback?.Invoke();
            });
        }
        
    }
}