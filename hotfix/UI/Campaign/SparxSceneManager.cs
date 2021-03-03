using System;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    /// <summary>
    /// 场景数据管理器
    /// </summary>
    public class SceneManager : ManagerUnit
    {
        private GameObject m_listener;
        private EndPoint m_endPoint;
        private List<CombatCacheEntry> m_combatQueue;
        private long m_currentCombatId;

        public const string MainLandName = "s001a";
        public static Vector3 HeroStart = new Vector3(95, 0, 69);

        class CombatCacheEntry
        {
            public object payload;
            public long combatid;
            public CombatCacheEntry(object payload, long combatid)
            {
                this.payload = payload;
                this.combatid = combatid;
            }
        }

        public override void Initialize(Config config)
        {
            m_endPoint = Hub.Instance.ApiEndPoint;
            m_combatQueue = new List<CombatCacheEntry>();
        }
        public override void Dispose()
        {
            m_endPoint = null;
        }

        public override void OnLoggedIn()
        {
            m_combatQueue.Clear();
        }

        public override void Async(string message, object payload)
        {
            switch (message)
            {
                case "startCampaign":
                    //OnCampaignStart(payload.ToString());
                    break;
                case "combatTransition"://进战斗场景
                    OnCombatTransition(payload);
                    break;
                case "campaignTransition"://出战斗场景
                    OnSceneTransition(payload);
                    break;
                case "newPlayer":
                    OnNewPlayer(payload);
                    break;
                case "leavePlayer":
                    OnLeavePlayer(payload);
                    break;
                case "newNpc":
                    OnNewNpc(payload);
                    break;
                case "leaveNpc":
                    OnLeaveNpc(payload);
                    break;
                case "updateNpc":
                    OnUpdateNpc(payload);
                    break;
                case "bossBloodUpdate":
                    bossBloodUpdate(payload);
                    break;
                case "Roll":
                    //GetRollMessage(payload);
                    break;
                case "rollDiceResult":
                    BossDiceNumUpdate(payload);
                    break;
                case "rollDiceRewards":
                    BossDiceRewardUpdate(payload);
                    break;
                case "playStateUpdate":
                    break;
                case "combatOver":
                    CombatCleanUp();
                    break;
                case "MainCampProgress":
                    UpdateMainInstanceProgress(payload);
                    break;
                case "FCComfirm":
                    ShowChallengeQuickBattleResult(payload);
                    break;
                case "prepareRobCombat":
                    //TakeRobFight(payload);
                    break;
                case "denyRobCombat":
                    MessageTemplateManager.ShowMessage(902069);
                    break;
            }
        }

        private void TakeRobFight(object payload)
        {
            long targetUID = EB.Dot.Long("uid", payload, 0);
            string DartID = EB.Dot.String("targetId", payload, "");
            if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop)
            {
                AlliancesManager.Instance.TakeFight(targetUID, DartID);
            }
            else
            {
                AlliancesManager.Instance.DenyFight(targetUID);
            }
        }

        private void CombatCleanUp()
        {
            int combat_session_id = -1;
            DataLookupsCache.Instance.SearchIntByID("playstate.Combat.combat_id", out combat_session_id);

            Hotfix_LT.Messenger.Raise<int, System.Action<EB.Sparx.Response>>(Hotfix_LT.EventName.CombatCleanUp, combat_session_id, delegate (EB.Sparx.Response result)
            {
                if (result != null && result.hashtable["retry"] != null)
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.DirectExitCombat);
                    return;
                }

                DataLookupsCache.Instance.CacheData(result.hashtable);

                EventManager.instance.Raise(eSimpleEventID.CombatCleanUpEvent);
                LTHotfixManager.GetManager<SceneManager>().EnterSceneByPlayState();
            });
        }


        /// <summary>
        /// 更新主线进度
        /// </summary>
        /// <param name="payload"></param>
        private void UpdateMainInstanceProgress(object payload)
        {
            Hashtable chapterData = EB.Dot.Object("normalChapters", payload, null);
            if (chapterData != null)
            {
                foreach (string chapterId in chapterData.Keys)
                {
                    DataLookupsCache.Instance.CacheData(string.Format("userCampaignStatus.normalChapters.{0}.progress", chapterId), null);
                }
                DataLookupsCache.Instance.CacheData("userCampaignStatus", payload as IDictionary);
            }
        }

        private Dictionary<string, int> TempHpDic;

        private void ShowChallengeQuickBattleResult(object payload)
        {
            bool isFastCombat = EB.Dot.Bool("isFastCombat", payload, false);
            if (isFastCombat)
            {
                SceneLogic.BattleType = eBattleType.ChallengeCampaign;
                Hashtable combatData = EB.Dot.Object("combat", payload, null);
                InitChallengeTempHp(combatData);
                bool isCombatWon = EB.Dot.Bool("isCombatWon", payload, false);
                long combatId = EB.Dot.Long("combatId", payload, 0);
                Hashtable table = Johny.HashtablePool.Claim();
                table["flag"] = isCombatWon;
                System.Action<int> action = new System.Action<int>(delegate (int confirm)
                {
                    Hotfix_LT.Messenger.Raise(EventName.ChallengeConfirmFastCombat, combatId, confirm);
                });
                table["action"] = action;
                table["isConfirm"] = true;
                GlobalMenuManager.Instance.Open("LTChallengeInstanceDefaultView", table);
            }
        }

        private void InitChallengeTempHp(object data)
        {
            TempHpDic = new Dictionary<string, int>();
            if (data == null)
            {
                return;
            }
            ArrayList hpList = Hotfix_LT.EBCore.Dot.Array("heroHp", data, null);
            if (hpList != null)
            {
                for (int i = 0; i < hpList.Count; i++)
                {
                    string heroId = EB.Dot.String("hero_id", hpList[i], string.Empty);
                    int hp = EB.Dot.Integer("hp", hpList[i], 0);
                    if (!TempHpDic.ContainsKey(heroId))
                    {
                        TempHpDic.Add(heroId, hp);
                    }
                }
            }
        }

        public int GetChallengeTempHp(string heroId)
        {
            if (TempHpDic.ContainsKey(heroId))
            {
                return TempHpDic[heroId];
            }
            return 0;
        }

        public bool GetChallengeHasLive()
        {
            foreach (int temp in TempHpDic.Values)
            {
                if (temp > 0) return true;
            }
            return false;
        }

        public void OnSceneTransition(object payload)
        {
            int type = EB.Dot.Integer("combatType", payload, 0);
            bool isFastCombat = EB.Dot.Bool("isFastCombat", payload, false);
            ArrayList battlerMetric = Hotfix_LT.EBCore.Dot.Array("battlerMetric", payload, Johny.ArrayListPool.Claim());
            DataLookupsCache.Instance.CacheData("battlerMetric", battlerMetric);

            DataLookupsCache.Instance.CacheData("honorBattleResult", EBCore.Dot.Array("honorBattleResult", payload, Johny.ArrayListPool.Claim()));

            if (isFastCombat)
            {
                if (type == (int)eBattleType.HonorArena)
                {
                    GlobalMenuManager.Instance.ComebackToMianMenu(); 
                    ShowQuickBattleResult(payload, type);
                }
                else if ((type == (int)eBattleType.LadderBattle))
                {
                    GlobalMenuManager.Instance.CloseMenu("LTHeroBattleMenu");
                    if (LadderController.Instance != null) LadderController.Instance.StopMatch(true);
                    ShowQuickBattleResult(payload, type);
                }
                else
                {
                    SceneLogic.BattleType = eBattleType.ChallengeCampaign;
                    bool isCombatWon = EB.Dot.Bool("isCombatWon", payload, false);

                    System.Action callback = delegate ()
                    {
                        Hotfix_LT.Messenger.Raise(EventName.ChallengeBattle, false);
                    };
                    if (isCombatWon)
                    {
                        //挑战副本在奖励界面关闭后再发送广播请求状态同步
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("reward", BattleResultScreenController.GetRewardItemDatas());
                        ht.Add("callback", callback);
                        GlobalMenuManager.Instance.Open("LTShowRewardView", ht);
                    }
                    else
                    {
                        callback();
                    }

                  
                }

                return;
            }

            long combatid = EB.Dot.Long("combatId", payload, 0);

            if (combatid == m_currentCombatId || combatid == 0)
            {
                PlayerManagerForFilter.Instance.StopShowPlayer();
                if (MainLandLogic.GetInstance() != null) 
                {
                    MainLandLogic.GetInstance().OnSceneTransitionResponse(payload as Hashtable);
                }
            }
            else if (type == (int)eBattleType.MainCampaignBattle || type == (int)eBattleType.ArenaBattle)
            {
                //快速战斗展示奖励
                ShowQuickBattleResult(payload, type);
            }
            else
            {
                for (int i = 0; i < m_combatQueue.Count; i++)
                {
                    if (m_combatQueue[i].combatid == combatid)
                    {
                        m_combatQueue.Remove(m_combatQueue[i]);
                        return;
                    }
                }
            }
        }

        private void ShowQuickBattleResult(object payload, int type)
        {
            var temp = payload as Hashtable;
            bool isWin;

            if (!bool.TryParse(temp["isCombatWon"].ToString(), out isWin))
            {
                EB.Debug.LogError("error format data for campaign transition data: isWon");
            }

            var combatOutCome = isWin ? eCombatOutcome.Win : eCombatOutcome.Lose;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("battleType", type);
            ht.Add("outCome", combatOutCome);
            ht.Add("campaignFinished", EB.Dot.Bool("isCampaignFinished", temp, true));
            ht.Add("isCombatOut", false);
            ht.Add("isFastCombat", true);
            GlobalMenuManager.Instance.Open("BattleResultScreen", ht);
        }

        public void OnCombatTransition(object payload)
        {
            long combatid = 0;
            combatid = EB.Dot.Long("combat.combatId", payload, combatid);

            FusionTelemetry.RecordCombat(combatid);//友盟，战斗开始统计。

            //CombatInfoData.GetInstance().LogJoinCombat(combatid);
            if (SceneLogic.SceneState != SceneLogic.eSceneState.SceneLoop && SceneLogic.SceneState != SceneLogic.eSceneState.RequestingCombatTransition)
            {
                m_combatQueue.Add(new CombatCacheEntry(payload, combatid));
            }
            else
            {
                DoCombatTransition(combatid, payload);
            }
        }

        void DoCombatTransition(long combatid, object payload)
        {
            PlayerManagerForFilter.Instance.StopShowPlayerForGoToCombat();
            SceneLogic.BattleType = (eBattleType)EB.Dot.Integer("combat.combatType", payload, (int)SceneLogic.BattleType);
            if (SceneLogic.BattleType == eBattleType.TransferOrRob || SceneLogic.BattleType == eBattleType.PVPBattle)
            {
                WorldMapPathManager.Instance.SaveBeforePullIntoCombat();
            }
            m_currentCombatId = combatid;
            if (SceneLogic.BattleType == eBattleType.NationBattle)
            {
                FusionAudio.PostEvent("UI/Battle/StartBattle", true);
            }
            if (!UIStack.Instance.IsLoadingScreenUp)
            {
                UIStack.Instance.ShowLoadingScreen(() =>
                {
                    ClearCombatCache();
                    MainLandLogic.GetInstance().OnCombatTransition(payload);
                }
                , false, true);
            }
            else
            {
                ClearCombatCache();
                MainLandLogic.GetInstance().OnCombatTransition(payload);
            }
        }
        //-------------------------------延迟执行------------------------
        private int _overTime;
        private int _waitTime;
        private System.Action<bool> _combatQueueResult;
        private float _buff;
        private float _time;
        private Coroutine _cDoCombatQueue;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitTime">等待时间</param>
        /// <param name="overTime">超时时间</param>
        /// <param name="result">结果处理回调</param>
        public void StartDoCombatQueue(int waitTime, int overTime, System.Action<bool> result)
        {
            this.StopDoCombatQueue();
            if (_combatQueueResult != null)
            {
                _combatQueueResult(false);
            }
            _overTime = overTime;
            _waitTime = waitTime;
            _combatQueueResult = result;
            _cDoCombatQueue = EB.Coroutines.Run(DoCombatQueue());
        }

        /// <summary>
        /// 停掉战斗协程
        /// </summary>
        public void StopDoCombatQueue()
        {
            if (_cDoCombatQueue != null)
            {
                EB.Coroutines.Stop(_cDoCombatQueue);
            }
        }

        IEnumerator DoCombatQueue()
        {
            _time = UnityEngine.Time.unscaledTime;
            bool isDo = false;
            while (_overTime > 0)
            {
                yield return new WaitForSeconds(1);
                _buff = UnityEngine.Time.unscaledTime - _time;
                if (_buff >= 1)
                {
                    _waitTime--;
                    _overTime--;
                    _time++;
                }

                if (_waitTime <= 0 && m_combatQueue.Count > 0) //等待时间完 
                {
                    isDo = true;
                    if (_combatQueueResult != null)
                    {
                        _combatQueueResult(true);
                    }
                    CombatCacheEntry combat = m_combatQueue[0];
                    DoCombatTransition(m_combatQueue[0].combatid, m_combatQueue[0].payload);
                    m_combatQueue.RemoveAt(0);
                    _overTime = 0;
                }
            }

            if (!isDo && _combatQueueResult != null)
            {
                _combatQueueResult(false);
            }
        }

        public void StartCombatFromQueue()
        {
            if (m_combatQueue.Count > 0)
            {
                CombatCacheEntry combat = m_combatQueue[0];
                m_combatQueue.Remove(combat);
                OnCombatTransition(combat.payload);
            }
        }

        public void OnNewPlayer(object payload)
        {
            PlayerManagerForFilter.Instance.ProcessPlayerCommInSync(payload);
        }

        public void OnLeavePlayer(object payload)
        {
            PlayerManagerForFilter.Instance.ProcessPlayerLeaveSync(payload);
        }

        public void OnNewNpc(object payload)
        {
            Hotfix_LT.Messenger.Raise("LTSpeedSnatchEvent.NewNpc");
            NpcManager.Instance.ProcessNpcCommInSync(payload);
        }

        public void OnLeaveNpc(object payload)
        {
            Hotfix_LT.Messenger.Raise("LTSpeedSnatchEvent.LeaveNpc");
            NpcManager.Instance.ProcessNpcLeaveSync(payload);
        }

        public void OnUpdateNpc(object payload)
        {
            NpcManager.Instance.ProcessNpcUpdateSync(payload);
        }

        public void bossBloodUpdate(object payload)
        {
            long left = 1;
            left = EB.Dot.Long("world_boss.blood.l", payload, left);
            if (left <= 0)
            {
                if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop && MainLandLogic.GetInstance().CurrentSceneName == MainLandName)
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.BossDieEvent);
                }
                AutoRefreshingManager.Instance.RemoveWorldBossRollingMsg();
            }
        }

        public void GetRollMessage(object payload)
        {
            /*float node = EB.Dot.Single("world_boss.roll", payload, 0);
            if (node > 0)
            {
                LTWorldBossDataManager.Instance.IsHaveWorldBossRoll = true;
                if (LTWorldBossRewardCtrl.Instance == null)
                {
                    GlobalMenuManager.Instance.Open("LTWorldBossRewardView", node);
                }
                else
                {
                    LTWorldBossRewardCtrl.Instance.CreateItem(node);
                }
            }*/
        }

        public void BossDiceNumUpdate(object payload)
        {
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnMaxRollUpdate);
        }

        public void BossDiceRewardUpdate(object payload)
        {
            ArrayList list = Hotfix_LT.EBCore.Dot.Array("world_boss.rollDiceRewards", payload, null);
            List<LTShowItemData> dataLis = new List<LTShowItemData>();
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string data = EB.Dot.String("data", list[i], string.Empty);
                    int num = EB.Dot.Integer("quantity", list[i], 0);
                    string type = EB.Dot.String("type", list[i], string.Empty);
                    dataLis.Add(new LTShowItemData(data, num, type));
                }
                GlobalMenuManager.Instance.Open("LTShowRewardView", dataLis);
            }
        }

        void ClearCombatCache()
        {
            Hashtable combatClearHash = Johny.HashtablePool.Claim();
            combatClearHash["combat"] = null;
            DataLookupsCache.Instance.CacheData(combatClearHash);
            Johny.HashtablePool.Release(combatClearHash);
        }

        public void RequestQuitCampaign(int campaignId, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/scenes/campaign/quitCampaign");
            request.AddData("campaignId", campaignId);
            m_endPoint.Service(request, callback);
        }

        public void RequestCombatResumeToScene(System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Get("/playstate/getPlayState");
            m_endPoint.Service(request, callback);
        }

        private List<DestinationStruct> mUpdatePlayerMovementBefores = new List<DestinationStruct>();
        private List<Request> mUpdatePlayerMovementRequests = new List<Request>();
        private List<System.Action<Response>> mUpdatePlayerMovementCallbacks = new List<System.Action<Response>>();
        public void UpdatePlayerMovement(int campaignId, Vector3 pos, float dir, System.Action<Response> callback)
        {
            if (IsSameData(campaignId, pos, mUpdatePlayerMovementBefores))
            {
                return;
            }
            EB.Sparx.Request request = m_endPoint.Post("/scenes/scene/updatePlayer");
            request.AddData("campaignId", campaignId);
            request.AddData("pos", pos.ToString());
            request.AddData("dir", dir);
            AddBeforesList(campaignId, pos, ref mUpdatePlayerMovementBefores);
            mUpdatePlayerMovementRequests.Add(request);
            mUpdatePlayerMovementCallbacks.Add(callback);
        }

        struct DestinationStruct
        {
            public int campaignId;
            public Vector3 pos;
        }
        private List<DestinationStruct> mUpdateMoveDestinationBefores = new List<DestinationStruct>();
        private List<Request> mUpdateMoveDestinationRequests = new List<Request>();
        private List<System.Action<Response>> mUpdateMoveDestinationCallbacks = new List<System.Action<Response>>();
        public void UpdateMoveDestination(int campaignId, Vector3 pos, System.Action<Response> callback)
        {
            if (IsSameData(campaignId, pos, mUpdateMoveDestinationBefores))
            {
                return;
            }
            EB.Sparx.Request request = m_endPoint.Post("/scenes/scene/updateMoveDestination");
            request.AddData("campaignId", campaignId);
            request.AddData("dest", pos.ToString());
            AddBeforesList(campaignId, pos, ref mUpdateMoveDestinationBefores);
            mUpdateMoveDestinationRequests.Add(request);
            mUpdateMoveDestinationCallbacks.Add(callback);
        }

        private bool IsSameData(int campaignId, Vector3 pos, List<DestinationStruct> destList)
        {
            return destList.Count > 0 && (int)destList[destList.Count - 1].campaignId == campaignId &&
                Vector3.Distance(destList[destList.Count - 1].pos, pos) <= 0.1f;
        }

        private void AddBeforesList(int campaignId, Vector3 pos, ref List<DestinationStruct> destList)
        {
            destList.Add(new DestinationStruct { campaignId = campaignId, pos = pos });
            if (destList.Count > 10)
            {
                destList.RemoveAt(0);
            }
        }

        private int mCurrentUpdateDestinationTime = 0;
        private const int TotalUpdateDestinationTime = 2;

        private int mCurrentUpdatePlayerTime = 0;
        private const int TotalUpdatePlayerTime = 3;
        //间隔时间发送同步消息
        public void OnLateUpdate()
        {
            //同步移动方向
            if (mUpdateMoveDestinationRequests.Count > 0)
            {
                mCurrentUpdateDestinationTime++;
                if (mCurrentUpdateDestinationTime >= TotalUpdateDestinationTime)
                {
                    mCurrentUpdateDestinationTime = 0;
                    m_endPoint.Service(mUpdateMoveDestinationRequests[mUpdateMoveDestinationRequests.Count - 1],
                        mUpdateMoveDestinationCallbacks[mUpdateMoveDestinationCallbacks.Count - 1]);
                    mUpdateMoveDestinationRequests.Clear();
                    mUpdateMoveDestinationCallbacks.Clear();
                }
            }

            //同步移动位置
            if (mUpdatePlayerMovementRequests.Count > 0)
            {
                mCurrentUpdatePlayerTime++;
                if (mCurrentUpdatePlayerTime >= TotalUpdatePlayerTime)
                {
                    mCurrentUpdatePlayerTime = 0;
                    m_endPoint.Service(mUpdatePlayerMovementRequests[mUpdatePlayerMovementRequests.Count - 1],
                        mUpdatePlayerMovementCallbacks[mUpdatePlayerMovementCallbacks.Count - 1]);
                    mUpdatePlayerMovementRequests.Clear();
                    mUpdatePlayerMovementCallbacks.Clear();
                }
            }
        }

        public void RequestCombatTransition(eBattleType battletype, int sceneid, Vector3 pos, float dir, List<string> enemies, System.Action<Response> callback)
        {
            EB.Sparx.Request request;
            switch (battletype)
            {
                case eBattleType.CampaignBattle:
                    request = m_endPoint.Post("/scenes/campaign/requestPvECombatTransition");
                    break;
                case eBattleType.HantBattle:
                    request = m_endPoint.Post("/scenes/mainland/requestPvECombatTransition");
                    break;
                case eBattleType.GhostBattle:
                    request = m_endPoint.Post("/specialactivity/startGhostChallenge");
                    break;
                case eBattleType.BossBattle:
                    request = m_endPoint.Post("/worldboss/startChallenge");
                    break;
                case eBattleType.ComboTest:
                    request = m_endPoint.Post("/combat/startComboPractice");
                    break;
                case eBattleType.AllianceCampaignBattle:
                    request = m_endPoint.Post("/allianceCampaign/requestPvECombatTransition");
                    break;
                default:
                    request = m_endPoint.Post("/scenes/mainland/requestPvECombatTransition");
                    break;
            }
            request.AddData("campaignId", sceneid);
            request.AddData("pos", pos.ToString());
            request.AddData("dir", dir);
            if (enemies != null)
            {
                string json = EB.JSON.Stringify(enemies);
                request.AddData("enemyGroup", json);    // fixme
            }
            else
            {
                request.AddData("enemyGroup", null);    // fixme
            }
            m_endPoint.Service(request, callback);
        }

        public void RequestCombatTransition(string campaign_name, string enemies, bool isFTE, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/combat/requestPvECombatTransition");
            request.AddData("fte", isFTE);
            request.AddData("levelName", campaign_name);
            request.AddData("enemyGroup", enemies);
            m_endPoint.Service(request, callback);
        }

        public void RequestFastCombatTransition(eBattleType battletype, string campaign_name, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/combat/startFirstCombat");   ///fastcampaign/requestPvECombatTransition
			request.AddData("campaignName", campaign_name);
            request.AddData("isFTE", GuideManager.Instance.GuideState);
            m_endPoint.Service(request, callback);
        }

        public void RequestBountyTaskCombat(System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/mhjtasks/startBountyTaskCombat");   ///fastcampaign/requestPvECombatTransition
			request.AddData("task_id", LTBountyTaskHudController.TaskID().ToString());
            request.AddData("mainlandId", MainLandLogic.GetInstance().CurrentSceneName);
            m_endPoint.Service(request, callback);
        }

        public void RequestCreateCampaign(string campaignName, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/scenes/campaign/create_campaign");
            request.AddData("campaignName", campaignName);
            m_endPoint.Service(request, callback);
        }

        public void RequestEnterCampaign(string campaignName, long campaignId, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/scenes/campaign/enterCampaign");
            request.AddData("campaignName", campaignName);
            request.AddData("campaignId", campaignId);
            m_endPoint.Service(request, callback);
        }

        public void RequestEnterAllianceCampaign(string campaignName, long campaignId, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/allianceCampaign/enterCampaign");
            request.AddData("campaignName", campaignName);
            request.AddData("campaignId", campaignId);
            m_endPoint.Service(request, callback);
        }

        public void RequestQuitAllianceCampaign(int campaignId, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/allianceCampaign/leaveCampaign");
            request.AddData("campaignId", campaignId);
            m_endPoint.Service(request, callback);
        }

        public void RequestTransfer(string from_id, Vector3 from_pos, float from_dir, string to_id, Vector3 to_pos, float to_dir, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/scenes/mainland/transfer");
            request.AddData("fromScene", from_id);
            request.AddData("fromPos", from_pos.ToString());
            request.AddData("toScene", to_id);
            request.AddData("toPos", to_pos.ToString());
            request.suspendMethod = Request.eSuspendMethod.Finish;//if retry When transfer logout may lead to callback bug 
            m_endPoint.Service(request, callback);
        }

        public void RequestRevive(System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/scenes/campaign/revive");
            m_endPoint.Service(request, callback);
        }


        public void RequestEnterCityView(System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/buildings/enterCity");
            m_endPoint.Service(request, callback);
        }

        public void RequestExitCityView(System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/buildings/leaveCity");
            m_endPoint.Service(request, callback);
        }

        public override void OnEnteredForeground()
        {
            EB.Debug.Log("OnEnteredForeground {0}", SceneLogic.SceneState);
            if (SceneLogic.SceneState == SceneLogic.eSceneState.SceneLoop
            || SceneLogic.SceneState == SceneLogic.eSceneState.SceneOutCome
            || SceneLogic.SceneState == SceneLogic.eSceneState.SceneTransition
            || SceneLogic.SceneState == SceneLogic.eSceneState.CombatLoop
            || SceneLogic.SceneState == SceneLogic.eSceneState.CombatTransition)
            {
                RequestPlayState();
            }
            else if (SceneLogic.SceneState != SceneLogic.eSceneState.CombatLoop)
            {
                string oldPlayState;
                DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out oldPlayState);
                if (oldPlayState == "Combat")
                {
                    //EnterSceneByPlayState();
                    int cid;
                    DataLookupsCache.Instance.SearchIntByID("playstate.Combat.combat_id", out cid);
                    ResumeCombat(cid, delegate (Response r) { });
                }
                else
                {
                    RequestPlayState();
                }
            }
        }

        public void RequestLoginData(System.Action<Response> callback)
        {
            Hashtable charData = Johny.HashtablePool.Claim();
            charData["worldId"] = LoginManager.Instance.LocalUser.WorldId;
            EB.Sparx.Request request = m_endPoint.Post("/auth/accountData");
            request.AddData(charData);
            m_endPoint.Service(request, callback);
        }

        public void RequestPlayState()
        {
            EB.Sparx.Request request = m_endPoint.Get("/playstate/getPlayState");
            m_endPoint.Service(request, OnRequestPlayStateResponse);
        }

        public void OnRequestPlayStateResponse(EB.Sparx.Response result)
        {
            if (result.sucessful)
            {
                string oldPlayState;
                DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out oldPlayState);
                Hashtable data = result.hashtable;
                string newPlayState = EB.Dot.String("playstate.state", data, "");
                EB.Debug.Log("OnRequestPlayStateResponse {0} {1}", oldPlayState, newPlayState);
                if (string.IsNullOrEmpty(newPlayState))
                {
                    EB.Debug.LogWarning("new PlayState is null");
                    return;
                }
                if (newPlayState == oldPlayState)
                {
                    IDictionary scenedata = null;
                    switch (newPlayState)
                    {
                        case "MainLand":
                            {
                                scenedata = EB.Dot.Object("mainlands", data, null);
                            }
                            break;
                        case "Campaign":
                            {
                                scenedata = EB.Dot.Object("campaigns", data, null);
                            }
                            break;
                        case "Combat":
                            {
                                if (SceneLogic.SceneState != SceneLogic.eSceneState.CombatLoop)
                                {
                                    int cid;
                                    DataLookupsCache.Instance.SearchIntByID("playstate.Combat.combat_id", out cid);
                                    ResumeCombat(cid, delegate (Response r) { });
                                }
                            }
                            break;
                        default:
                            {
                                scenedata = null;
                            }
                            break;
                    }
                    if (scenedata != null)
                    {
                        DataLookupsCache.Instance.CacheData(data);
                        string scenetype = EB.Dot.String("scenetype", data, "");
                        NpcManager.Instance.MergeNpc(scenetype, scenedata);
                        PlayerManagerForFilter.Instance.MergePlayer(scenetype, scenedata);
                    }
                }
                else
                {
                    DataLookupsCache.Instance.CacheData(data);
                    EnterSceneByPlayState();
                    EB.Debug.LogWarning("PlayState is change");
                    return;
                }
            }
            else if (result.fatal)
            {
                SparxHub.Instance.FatalError(result.localizedError);
            }
        }

        public void EnterSceneByPlayState()
        {
            //根据CampaignTransition 返回的数据决定是进入campaign还是mainland
            string state = "";
            DataLookupsCache.Instance.SearchDataByID<string>("playstate.state", out state);
            if (state == PlayState.PlayState_MainLand)
            {
                Hashtable mainlands;
                DataLookupsCache.Instance.SearchDataByID<Hashtable>("mainlands", out mainlands);
                if(mainlands == null){
                    mainlands = Johny.HashtablePool.Claim();
                }
                MainLandLogic.GetInstance().OnSceneEnter(mainlands);
            }
            else if (state == PlayState.PlayState_LCCampaign) //挑战副本
            {
                if (GameFlowControlManager.Instance != null)
                {
                    GameFlowControlManager.Instance.SendEvent("GoToInstanceView");
                }
            }
            else if (state == PlayState.PlayState_Combat)
            {
                if (GameFlowControlManager.Instance != null)
                {
                    GameFlowControlManager.Instance.SendEvent("GoToCombatView");
                }
            }
            else
            {
                EB.Debug.LogError("playstate.state is error!state = ", state);
            }
        }

        public void RequestOpenCampaignBox(string scene, string box)
        {
            EB.Sparx.Request request = m_endPoint.Post("/scenes/scene/openBox");
            request.AddData("sceneName", scene);
            request.AddData("boxId", box);
            m_endPoint.Service(request, delegate (Response result)
            {
                if (result.sucessful)
                {
                    DataLookupsCache.Instance.CacheData(result.hashtable);
                }
            });
        }

        public void RequestSetUserComboCount(int num, System.Action<Response> callback)
        {
            EB.Sparx.Request request = m_endPoint.Post("/userstats/setComboCount");
            request.AddData("comboCount", num);
            m_endPoint.Service(request, callback);
        }

        public void SetSceneViewPlayerNum(int num)
        {
            EB.Sparx.Request request = m_endPoint.Post("/playstate/setViewNum");
            request.AddData("viewNum", num);
            m_endPoint.Service(request, delegate (Response result)
            {
                if (result.sucessful)
                {
                    DataLookupsCache.Instance.CacheData(result.hashtable);
                }
                else if (result.fatal)
                {
                    Hub.Instance.FatalError(result.localizedError);
                    return;
                }
                else
                {
                    EB.Debug.LogError(result.localizedError);
                }
            });
        }

        /// <summary>
        /// 战斗掉线重连恢复
        /// </summary>
        /// <param name="combatId"></param>
        public void ResumeCombat(int combatId, System.Action<Response> callback)
        {
            //CombatSyncData.Instance.isResume = true; //设置断线重连  已迁移
            //CombatSyncData.Instance.isResumePvp = true;
            //CombatSyncData.Instance.PendingEvents.Clear();//清空战斗序列

            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.ResumeCombat);

            EB.Sparx.Request request = m_endPoint.Post("/combat/resume");
            request.AddData("combat_id", combatId);
            request.AddData("turn", 0);
            request.AddData("delay", 0);
            request.AddData("logid", 0);
            m_endPoint.Service(request, callback);
        }
    }
}
