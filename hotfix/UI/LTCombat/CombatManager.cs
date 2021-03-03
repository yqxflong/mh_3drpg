using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using EB.Sparx;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class CombatManager : ManagerUnit
    {
        private GameObject m_listener;  //注册
        public EB.Sparx.SubSystemState State;

        // private static CombatManager s_cm;
        private static EB.Sparx.EndPoint s_endpoint;  //SparxHub
        private static EB.Sparx.EndPoint s_simEndpoint;  //EndPointSimulator
        private static CombatManager sInstance = null;

        public static CombatManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<CombatManager>(); }
        }

        public object startObj;

        public static EB.Sparx.EndPoint endPoint
        {
            get { return s_simEndpoint != null ? s_simEndpoint : s_endpoint; }
        }

        public static bool IsSim()
        {
            return s_simEndpoint != null;
        }

        public override void Initialize(EB.Sparx.Config config)
        {
            s_endpoint = SparxHub.Instance.ApiEndPoint;
            Hotfix_LT.Messenger.AddListener<int, System.Action<EB.Sparx.Response>>(Hotfix_LT.EventName.CombatCleanUp, RequestExitCombat);
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.DirectExitCombat, delegate () { BattleResultScreenController.DirectExitCombat(); });
            Hotfix_LT.Messenger.AddListener<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, DoDebugAction);

            LT.MainMessenger.AddListener<int, Action<Response>>("CombatEditorRequestExitCombat", RequestExitCombat);
        }

        public override void Dispose()
        {
            Hotfix_LT.Messenger.RemoveListener<int, System.Action<EB.Sparx.Response>>(Hotfix_LT.EventName.CombatCleanUp, RequestExitCombat);
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.DirectExitCombat, delegate () { BattleResultScreenController.DirectExitCombat(); });
            Hotfix_LT.Messenger.RemoveListener<string, Hashtable>(Hotfix_LT.EventName.DoDebugAction, DoDebugAction);
        }

        public void ClientCombatReadyFromCombatStruct(int combat_id)
        {
            ClientCombatReady(combat_id, delegate() {
                if (LTCombatEventReceiver.Instance != null)
                { 
                    LTCombatEventReceiver.Instance.Ready = true; 
                }
            });
        }

        public void ClientCombatReadyFromLTCombatEventReceiver(int combat_id)
        {
            ClientCombatReady(combat_id);
        }

        public void ClientCombatReady(int combat_id, System.Action callback = null)
        {
            Combat.CombatSyncData.Instance.CleanUp();

            if (Combat.CombatSyncData.Instance.EventQueue != null)
            {
                Combat.CombatSyncData.Instance.EventQueue.Clear();
            }

            var request = endPoint.Post("/combat/clientCombatReady");
            request.AddData("combat_id", combat_id);
            request.numRetries = 1;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;

            endPoint.Service(request, delegate (EB.Sparx.Response result)
            {
                if (result != null && !result.sucessful)
                {
                    if (LTCombatHudController.Instance != null)
                    {
                        LTCombatHudController.Instance.ShowCombatBaseUI();
                    }

                    if (result.error != null)
                    {
                        EB.Debug.LogError("CombatEventManager.clientCombatReady - /combat/clientCombatReady failed: {0}", result.error);
                    }
                        
                    OnNetworkError(result);
                }
                else
                {
                    callback?.Invoke();
                }
            });
        }

        public void ClientWatchReady(int combat_id)
        {
            var request = endPoint.Post("/combat/onWatcherReady");
            request.AddData("combat_id", combat_id);
            request.numRetries = 1;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;

            endPoint.Service(request, delegate (EB.Sparx.Response result)
            {
                if (!result.sucessful)
                {
                    if (result.error != null)
                    {
                        if (LTCombatHudController.Instance != null) LTCombatHudController.Instance.ExitWatchAsk();
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ALLIANCE_BATTLE_OVER"));
                    }
                }
            });
        }


        #region About 释放技能
        private EB.Sparx.Request _setskill_request = null;
        public bool GetSkillRequestState()
        {
            return _setskill_request != null;
        }
        public void SetSkill(int character_id, int skill_id, int combat_id, int team, int actor, int skillIndex, int target)
        {
            if (!Hotfix_LT.Combat.CombatSyncData.Instance.NeedSetSkill)
                return;

            if (LTCombatHudController.Instance.IsCombatOut)
            {
                return;
            }

            if(GetSkillRequestState())
            {
                return;
            }

            ClearPingTime();
            Hotfix_LT.Combat.CombatSyncData.Instance.NeedSetSkill = false;

            EB.Sparx.Request request = endPoint.Post("/combat/setSkill");
            request.AddData("combatId", combat_id);
            request.AddData("team", 0);
            request.AddData("actor", actor);
            request.AddData("action", skillIndex);
            request.AddData("target", target);
            request.numRetries = 1;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;
            endPoint.Service(request, OnSetSkillResponse);
            _setskill_request = request;
        }

        private void OnSetSkillResponse(EB.Sparx.Response result)
        {
            if (!result.sucessful)
            {
                if(result.error.Contains("ID_SPARX_NETWORK_ERROR"))
                {
                    EB.Debug.LogWarning("CombatManager.SetSkill===>Failed, retry! error: ID_SPARX_NETWORK_ERROR");
                    endPoint.Service(_setskill_request, OnSetSkillResponse);
                    return;
                }
                else
                {
                    EB.Debug.LogError("CombatManager.SetSkill - /combat/setSkill failed: {0}" , result.error);
                    OnNetworkError(result);
                }
            }
            _setskill_request = null;
        }
        #endregion

        #region About 释放卷轴技能
        private EB.Sparx.Request _setscrollskill_request = null;
        public void SetScrollSkill(int character_id, int skill_id, int combat_id, int team, int actor, int skillId, int target)
        {
            if (!Hotfix_LT.Combat.CombatSyncData.Instance.NeedSetSkill)
                return;

            if (LTCombatHudController.Instance.IsCombatOut)
            {
                return;
            }

            if(_setscrollskill_request != null)
            {
                return;
            }

            ClearPingTime();
            Hotfix_LT.Combat.CombatSyncData.Instance.NeedSetSkill = false;

            EB.Sparx.Request request = endPoint.Post("/combat/setScrollSkill");
            request.AddData("combatId", combat_id);
            request.AddData("team", 0);
            request.AddData("actor", actor);
            request.AddData("action", skillId);
            request.AddData("target", target);
            request.numRetries = 1;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;
            endPoint.Service(request, OnSetScrollSkillResponse);
            _setscrollskill_request = request;
        }

        private void OnSetScrollSkillResponse(EB.Sparx.Response result)
        {
            if (!result.sucessful)
            {
                if(result.error.Contains("ID_SPARX_NETWORK_ERROR"))
                {
                    EB.Debug.LogWarning("CombatManager.SetScrollSkill===>Failed, retry! error: ID_SPARX_NETWORK_ERROR");
                    endPoint.Service(_setscrollskill_request, OnSetScrollSkillResponse);
                    return;
                }
                else
                {
                    EB.Debug.LogError("CombatManager.SetScrollSkill - /combat/setSkill failed: {0}" , result.error);
                    OnNetworkError(result);
                }
            }
            _setscrollskill_request = null;
        }
        #endregion

        public void PingImmediately()
        {
            m_pingprocessing = true;
            EB.Sparx.Request request = endPoint.Post("/combat/ping");
            request.AddData("combat_id", Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);
            request.numRetries = 0;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;

            LoadingSpinner.Show();
            //endpoint need an error handler
            endPoint.Service(request, delegate (EB.Sparx.Response result)
            {
                if (!result.sucessful)
                {
                    if (result.error.ToString() == "ID_ERROR_NOT_IN_BATTLE")
                    {
                        LoadingSpinner.Hide();
                        if (LTCombatEventReceiver.Instance != null && !LTCombatEventReceiver.Instance.IsBattleOver)
                        {
                            EB.Sparx.Request request2 = endPoint.Get("/playstate/getPlayState");
                            endPoint.Service(request2, (result2) => {
                                LoadingSpinner.Hide();
                                if (result != null)
                                {
                                    DataLookupsCache.Instance.CacheData(result2.hashtable);
                                    LTCombatHudController.Instance.ShowBattleResultScreen(SceneLogic.BattleType, MainLandLogic.GetInstance().m_CombatOutCome, false);
                                }
                            });
                        }
                        return;
                    }
                    else
                    {
                        m_failedpingcount++;
                    }
                }
                else
                {
                    LoadingSpinner.Hide();
                    int subEid = EB.Dot.Integer("subEid", result.result, 0);
                    if (subEid <= 0)
                    {
                        m_failedpingcount++;
                    }
                    else
                    {
                        if (subEid + 1 != Hotfix_LT.Combat.CombatSyncData.Instance.getCurrentSubEid())
                        {
                            if (CombatLogic.Instance.LocalPlayerIsObserver) CombatManager.Instance.ClientWatchReady(Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);
                            else ClientCombatReady(Hotfix_LT.Combat.CombatSyncData.Instance.CombatId);
                        }
                    }
                }

                ClearPingTime();
                m_pingprocessing = false;
            });
        }
        public void Ping()
        {
            if (Hotfix_LT.Combat.CombatSyncData.Instance.PendingEvents.Count > 0 || Hotfix_LT.Combat.CombatSyncData.Instance.NeedSetSkill || !LTCombatEventReceiver.Instance.IsAnimStateIdle() || CombatLogic.Instance.LocalPlayerIsObserver) return;

            if (LTCombatEventReceiver.Instance == null || LTCombatEventReceiver.Instance != null && !LTCombatEventReceiver.Instance.Ready)
            {
                ClearPingTime();
                return;
            }

            if (!m_pingprocessing && (System.DateTime.Now - m_lastping).Seconds >= 30)
            {
                PingImmediately();
            }
        }
        public void ClearPingTime() { m_lastping = System.DateTime.Now; }
        private System.DateTime m_lastping = System.DateTime.Now;
        private int m_failedpingcount = 0;
        private bool m_pingprocessing = false;

        public void RegisterListener(GameObject go, bool isFTE, int combat_id)
        {
            m_listener = go;
        }

        public void UnRegisterListener()
        {
            m_listener = null;
        }

        private bool hasRequest = false;
        public void RequestExitCombat(int combat_id, System.Action<EB.Sparx.Response> callback)
        {
            if (hasRequest) return;
            hasRequest = true;
            var request = s_endpoint.Post("/combat/exit");
            request.AddData("combat_id", combat_id);
            request.numRetries = 0;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;

            s_endpoint.Service(request, delegate (EB.Sparx.Response result)
            {
                hasRequest = false;
                if (result.sucessful == false)
                {
                    EB.Debug.LogError("CombatManager.RequestExitCombat - /combat/exit failed: {0}" , result.error.ToString());
                    OnNetworkError(result);
                }
                //else if (callback != null)
                if (callback != null)
                {
                    callback(result);
                }
            });
        }

        public void LastEventPlayed(int combat_id, int logid)
        {
            var request = s_endpoint.Post("/combat/lastEventPlayed");
            request.AddData("combat_id", combat_id);
            request.AddData("logid", logid);

            if (SceneLogic.BattleType == eBattleType.InfiniteChallenge || SceneLogic.BattleType == eBattleType.ChallengeCampaign)
            {
                int allPower = 0;
                List<TeamMemberData> temp = LTFormationDataManager.Instance.GetTeamMemList(FormationUtil.GetCurrentTeamName(SceneLogic.BattleType));
                for (int j = 0; j < temp.Count; j++)
                {
                    LTPartnerData data = LTPartnerDataManager.Instance.GetPartnerByHeroId(temp[j].HeroID);
                    if (data != null)
                    {
                        allPower += data.powerData.curPower;
                    }
                }

                request.AddData("br", allPower);
                EB.Debug.Log("LastEventPlayed set br = {0}", allPower);
            }
            request.numRetries = 1;
            request.suspendMethod = EB.Sparx.Request.eSuspendMethod.Finish;

            s_endpoint.Service(request, delegate (EB.Sparx.Response result)
            {
                if (!result.sucessful && result.fatal)
                {
                    EB.Debug.LogError("CombatManager.lastEventPlayed - /combat/lastEventPlayed failed: {0}" , result.error.ToString());
                    OnNetworkError(result);
                }
                else
                {
                    //if (callback != null)
                    //{
                    //    callback(result);
                    //}
                }
            });
        }

        public void DoDebugAction(string action, System.Collections.Hashtable myHash)
        {
            var request = s_endpoint.Post("/combat/doDebugAction");
            request.AddData("myDebugAction", action);
            if (myHash != null)
            {
                foreach (System.Collections.DictionaryEntry entry in myHash)
                {
                    request.AddData(entry.Key.ToString(), entry.Value);
                }
            }

            s_endpoint.Service(request, delegate (EB.Sparx.Response result)
            {
                if (result.sucessful == false)
                {
                    EB.Debug.LogError("CombatManager.DoDebugAction - /combat/DoDebugAction failed: {0}" , result.error.ToString());
                    OnNetworkError(result);
                }
            });
        }

        public void RegisterSimulatorEndpointListener(GameObject go)
        {
            s_simEndpoint = new EB.Sparx.EndPointSimulator(s_endpoint.Url, new EB.Sparx.EndPointOptions { KeepAlive = false });
            ((EB.Sparx.EndPointSimulator)s_simEndpoint).RegisterListener(go);
        }

        public void UnRegisterSimulatorEndpoint()
        {
            if (s_simEndpoint != null)
            {
                ((EB.Sparx.EndPointSimulator)s_simEndpoint).RegisterListener(null);
                s_simEndpoint = null;
            }
            else
            {
                EB.Debug.LogError("UnRegisterSimulatorEndpoint: s_simEndpoint is null");
            }
        }



        public override void OnLoggedIn()
        {
            base.OnLoggedIn();
            ClearPingTime();
            hasRequest = false;
        }

        public override void Connect()
        {
            State = EB.Sparx.SubSystemState.Connected;
        }

        public override void Disconnect(bool isLogout)
        {
            State = EB.Sparx.SubSystemState.Disconnected;
            startObj = null;
            hasRequest = false;

            if (s_simEndpoint != null)
            {
                UnRegisterSimulatorEndpoint();
                s_simEndpoint = null;
            }

            if (m_listener != null)
            {
                UnRegisterListener();
            }
        }


        public override void OnEnteredBackground()  
        {
            if (!IsSim() && m_listener != null)
            {
                m_listener.BroadcastMessage("PauseCombat");
            }
            State = EB.Sparx.SubSystemState.Disconnected;
        }

        public override void OnEnteredForeground()
        {
            if (!IsSim() && m_listener != null)
            {

            }
            State = EB.Sparx.SubSystemState.Connected;
        }

        public override void Async(string message, object payload)
        {
            if (State != EB.Sparx.SubSystemState.Connected)
            {
                EB.Debug.LogWarning("SparxCombatEventManager.Async: not connected, message {0}, {1}", message, State);
                return;
            }

            if (IsSim())
            {
                if (message.StartsWith("sim."))
                {
                    message = message.Split('.')[1];
                    EB.Debug.Log("SparxCombatEventManager.Async: received simulate message {0}", message);
                }
                else
                {
                    EB.Debug.LogWarning("SparxCombatEventManager.Async: received message {0}", message);
                    return;
                }
            }

            switch (message.ToLower())
            {
                case "set":
                    OnCombatEventArrayReceived(payload);
                    break;
                case "rewarddata"://only for cache //从rewardData 改成 rewarddata
                    break;
                default:
                    EB.Debug.LogError("CombatEventManager: No response defined for async message {0}" , message);
                    break;
            }
        }
        
        public void OnCombatEventArrayReceived(object payload)
        {
            if (LTCombatEventReceiver.Instance != null)
            {
                LTCombatEventReceiver.Instance.OnCombatEventListReceived(payload);
            }

            UIStack.Instance.HideLoadingScreen();
            Hashtable data = payload as Hashtable;

            if (data == null)
            {
                EB.Debug.LogError("Combat message error");
                return;
            }

            ClearPingTime();
            LoadingSpinner.Hide();

            int eid = EB.Dot.Integer("combat.eid", data, 0);
            bool reset = EB.Dot.Bool("combat.reset", data, false);

            if (reset)
            {
                int subEid = EB.Dot.Integer("combat.subEid", data, 0);
                Combat.CombatSyncData.Instance.AddQueue(new Combat.CombatResumeEvent(eid, subEid));
                Combat.CombatSyncData.Instance.Parse(data, true);
                return;
            }

            if (eid == 0)
            {
                Combat.CombatSyncData.Instance.Parse(data, true);
                return;//直接处理
            }

            if (Combat.CombatSyncData.Instance.PendingEvents.ContainsKey(eid))
            {
                Combat.CombatSyncData.Instance.PendingEvents[eid] = data;
                EB.Debug.LogWarning(string.Format(" eid {0} repetition!!! ", eid));
            }
            else
            {
                Combat.CombatSyncData.Instance.PendingEvents.Add(eid, data);
            }

            ILRTimerManager.instance.RemoveTimerSafely(ref _WaitForCombatReady_Sequence);
            _WaitForCombatReady_Sequence = ILRTimerManager.instance.AddTimer(1, 0, Update_WaitForCombatReady);
        }
        
        #region Coroutine -> Timer
        private int _WaitForCombatReady_Sequence = 0;
        private void Update_WaitForCombatReady(int sequence)
        {
            if (CombatLogic.Instance.Ready)
            {
                if (Hotfix_LT.Combat.CombatSyncData.Instance.ParseAll())
                {
                    ILRTimerManager.instance.RemoveTimerSafely(ref _WaitForCombatReady_Sequence);
                } 
            }
        }
        #endregion

        public void OnNetworkError(EB.Sparx.Response result)
        {
            if (result.fatal)
            {
                DealError(result);
                return;
            }
        }

        private void DealError(EB.Sparx.Response result)
        {
            if (result.error.ToString() == "ID_ERROR_NOT_IN_BATTLE")
            {
                LoadingSpinner.Hide();
                if (!LTCombatEventReceiver.Instance.IsBattleOver)
                {
                    EB.Sparx.Request request2 = endPoint.Get("/playstate/getPlayState");
                    endPoint.Service(request2, (result2) => {
                        if (result != null)
                        {
                            DataLookupsCache.Instance.CacheData(result2.hashtable);
                            LTCombatHudController.Instance.ShowBattleResultScreen(SceneLogic.BattleType, MainLandLogic.GetInstance().m_CombatOutCome, false);
                        }
                    });
                }
            }
            else if (result.error.ToString().Contains("user is in another battle, combat_id = "))
            {
                BattleResultScreenController.DirectExitCombat();
            }
            else if (result.error.ToString().Contains("EndWrite failure") || result.error.ToString().Contains("ID_SPARX_ERROR_NOT_CONNECTED"))
            {
                SparxHub.Instance.Disconnect(false);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SPARX_ERROR_NOT_CONNECTED"));
            }
            else if(result.error.ToString().Contains("socket hang up")|| result.error.ToString().Contains("connection timed out"))
            {
                EB.Debug.LogError(result.error);
                SparxHub.Instance.Disconnect(false);
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SPARX_ERROR_NOT_CONNECTED"));
            }
            else
            {
                EB.Debug.LogError(result.error);
            }
        }

        public void WatchWarRequire(int cambatID)
        {
            EB.Sparx.Request request = endPoint.Post("/combat/enterWatch");
            request.AddData("combat_id", cambatID);
            s_endpoint.Service(request, delegate (EB.Sparx.Response result)
            {
                if (!result.sucessful && result.fatal)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ALLIANCE_BATTLE_OVER"));
                }
            });
        }
        public void LevelWarRequire(int cambatID)
        {
            EB.Sparx.Request request = endPoint.Post("/combat/leaveWatch");
            request.AddData("combat_id", cambatID);
            s_endpoint.Service(request, null);
        }
    }
}
