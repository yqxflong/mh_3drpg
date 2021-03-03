using UnityEngine;
using System.Collections;
using Hotfix_LT.Data;

namespace Hotfix_LT.UI
{
    public class NpcIntactUI : DynamicMonoHotfix
    {
        static public bool GetIsTargetInDartState(long uid)
        {
            eDartState dartState = eDartState.None;
            double fDartState;
            if (!DataLookupsCache.Instance.SearchDataByID<double>("mainlands.pl." + uid + ".state.TOR", out fDartState))
            {
                EB.Debug.LogError("when intact search data dartState state fail");
            }
            dartState = (eDartState)System.Convert.ToInt32(fDartState);
            if (dartState == eDartState.Transfer_Tian || dartState == eDartState.Transfer_Di || dartState == eDartState.Transfer_Xuan || dartState == eDartState.Transfer_Huang)
            {
                return true;
            }
            return false;
        }

        static public bool GetIsTargetInFightState(long uid)
        {
            bool isFighting = false;
            if (!DataLookupsCache.Instance.SearchDataByID<bool>("mainlands.pl." + uid + ".state.F", out isFighting))
            {
                EB.Debug.LogError("when intact search data dartState state fail");
            }
            return isFighting;
        }

        static public bool GetIsRedName(string uid, out string playerName)
        {
            bool isRedName = false;
            if (!DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("mainlands.pl.{0}.state.R", uid), out isRedName, null))
            {
                EB.Debug.LogError("searchdata redname fail uid={0}", uid);
            }
            if (!DataLookupsCache.Instance.SearchDataByID<string>(string.Format("mainlands.pl.{0}.un", uid), out playerName, null))
            {
                EB.Debug.LogError("searchdata redname playername fail");
            }
            return isRedName;
        }

        public override void Awake()
        {
            Hotfix_LT.Messenger.AddListener<GameObject, GameObject>("OnInteractEvent", OnIntactFunc);
            Hotfix_LT.Messenger.AddListener<GameObject, GameObject>("OnImmediatelyInteractEvent", OnImmediatelyIntactFunc);
            Hotfix_LT.Messenger.AddListener("InteractOverEvent", OnIntactOverFunc);
        }

        public override void OnDestroy()
        {
            Hotfix_LT.Messenger.RemoveListener<GameObject, GameObject>("OnInteractEvent", OnIntactFunc);
            Hotfix_LT.Messenger.RemoveListener<GameObject, GameObject>("OnImmediatelyInteractEvent", OnImmediatelyIntactFunc);
            Hotfix_LT.Messenger.RemoveListener("InteractOverEvent", OnIntactOverFunc);
        }
        
        
        //交互处理函数,到达寻路终点后发现目标进行交互
        private void OnIntactFunc(GameObject player, GameObject interactable)
        {
            //在这里区分 玩家之间交互， 玩家与NPC之间的交互
            if (interactable.GetComponent<PlayerController>() != null)
            {
                PlayerIntactProcess(player, interactable);
            }
            else
            {
                NpcIntactProcess(player, interactable);
            }
        }
        

        //点击立马交互处理函数
        private void OnImmediatelyIntactFunc(GameObject player, GameObject interactable)
        {
            //if (SceneLogicManager.isCampaign())return;
            //在这里区分 玩家之间交互， 玩家与NPC之间的交互
            if (interactable == null)//点击地面
            {
                FusionAudio.PostEvent("UI/MainView/ShowSelfSelection", true);
            }
            else if (interactable.GetComponent<PlayerController>() != null)
            {
                FusionAudio.PostEvent("UI/MainView/ShowSelfSelection", true);
                if (interactable != null)
                {
                    //目标可被点击
                    long uid = interactable.GetComponent<PlayerController>().playerUid;
                    string playerName;

                    if (GetIsTargetInFightState(uid) || GetIsTargetInDartState(uid))
                    {
                        //目标是否处于战斗等不可交互状态
                        MessageTemplateManager.ShowMessage(902069);
                        return;
                    }
                    else if (GetIsRedName(uid.ToString(), out playerName))
                    {
                        //目标是否是可被攻击的(红名玩家)
                        if (BalanceResourceUtil.GetUserLevel() < AlliancesManager.Instance.DartConfig.AttackRedNameLevelLimit)
                        {
                            //目标当前等级过高不可被攻击
                            Hashtable h = Johny.HashtablePool.Claim();
                            h.Add("0", AlliancesManager.Instance.DartConfig.AttackRedNameLevelLimit);
                            MessageTemplateManager.ShowMessage(902096, h, null);
                        }
                        else
                        {
                            //目标可被攻击
                            int aid = 0;
                            DataLookupsCache.Instance.SearchIntByID("mainlands.pl." + uid + ".aid", out aid);
                            if (aid > 0 && AllianceUtil.GetIsInAlliance(uid))
                            {
                                //不可攻击同军团玩家
                                MessageTemplateManager.ShowMessage(902095);
                            }
                            else
                            {
                                //是否要发起攻击
                                MessageTemplateManager.ShowMessage(902058, playerName, delegate (int result)
                                {
                                    if (result == 0)
                                    {
                                        if (!LegionModel.GetInstance().isJoinedLegion)
                                        {
                                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, string.Format(EB.Localizer.GetString("ID_NOT_JOIN_ALLIANCE")));
                                            return;
                                        }
                                        string pn;
                                        if (GetIsRedName(AllianceUtil.GetLocalUid().ToString(), out pn))
                                        {
                                            MessageTemplateManager.ShowMessage(902103);
                                        }
                                        else
                                        {
                                            AlliancesManager.Instance.AttackRedName(uid);
                                        }
                                    }
                                });
                            }
                        }
                        return;
                    }
                    DataLookupsCache.Instance.SetCache("intact.player", uid, true);

                    if (UIStack.Instance.GetBackStackCount() > 1)
                    {
                        EB.Debug.LogWarning("Don't Open OtherPlayerInfoView");
                        EB.Debug.Log(UIStack.Instance.GetBackStackItem());
                        return;
                    }
                    EB.Debug.Log(UIStack.Instance.GetBackStackItem());
                    GlobalMenuManager.Instance.Open("OtherPlayerInfoView");

                    //if (null != PlayerIntactUI.Instance)
                    //	PlayerIntactUI.Instance.Uid = uid;               
                }
            }
            else
            {
                if (IsGhost(interactable))
                {
                    FusionAudio.PostEvent("SFX_CombatView_BattleStart");
                }
            }
        }

        //交互结束处理函数
        private void OnIntactOverFunc()
        {
            if (!string.IsNullOrEmpty(mCurrentCharacterAudioName))
            {
                FusionAudio.PostCharacterAnimation(mCurrentCharacterAudioName, "Dialogue_01", false);
            }
        }

        private void PlayerIntactProcess(GameObject player, GameObject interactable)
        {
        }

        //交互处理函数
        private void NpcIntactProcess(GameObject player, GameObject interactable)
        {
            //play Audio
            PostAudioMessage(player, interactable);

            //有任务优先显示任务  -修改为有主线任务优先显示
            NpcTaskDataLookup task_datalookup = interactable.transform.GetDataLookupILRComponentInChildren<NpcTaskDataLookup>("Hotfix_LT.UI.NpcTaskDataLookup",true,false);
            //老江需求，正在进行的任务不弹框

            if (task_datalookup != null && task_datalookup.mDL.DefaultDataID != null && task_datalookup.IsMainTask() && !task_datalookup.IsRunning())
            {
                string task_state = TaskSystem.ACCEPTABLE;
                DataLookupsCache.Instance.SearchDataByID<string>(task_datalookup.mDL.DefaultDataID + ".state", out task_state);
                if (task_state.Equals(TaskSystem.UNACCEPTABLE))
                {
                    PopFuncitonIntactMenu(player, interactable);
                    return;
                }

                if (IsNpcFuncMenu(player, interactable))
                {
                    string npc_id = interactable.name;
                    string scene_name = MainLandLogic.GetInstance().CurrentSceneName;
                    Hotfix_LT.Data.MainLandEncounterTemplate met = Hotfix_LT.Data.SceneTemplateManager.GetMainLandsNPCData(scene_name, npc_id);
                    if (IsFunctionNPC(npc_id, scene_name) && met != null && met.func_id_1 > 0)
                    {
                        if (IsOnlyHaveDeliveryDart(met) && AlliancesManager.Instance.DartData.State != eAllianceDartCurrentState.Transfering)
                        {
                            PopTaskIntactMenu(player, interactable);
                            return;
                        }

                        int m_taskId;
                        if (int.TryParse(task_datalookup.mDL.DefaultDataID.Replace(TaskStaticData.TaskPrefix, ""), out m_taskId))
                        {
                            TaskTemplate taskTpl = TaskTemplateManager.Instance.GetTask(m_taskId);
                            string taskName = taskTpl.task_name;
                            System.Action callBack = delegate ()
                            {
                                PopTaskIntactMenu(player, interactable);
                            };
                            Hashtable taskData = new Hashtable() { { "taskName", taskName }, { "callback", callBack } };
                            Hashtable data = new Hashtable() { { "npc", npc_id }, { "scene", scene_name }, { "task", taskData } };
                            GlobalMenuManager.Instance.Open("NpcFuncMenu", data);
                            return;
                        }
                        else
                        {
                            EB.Debug.LogError("parse taskId fail taskId=" + task_datalookup.mDL.DefaultDataID);
                        }
                    }
                }

                PopTaskIntactMenu(player, interactable);
                return;
            }

            //普通功能
            PopFuncitonIntactMenu(player, interactable);
        }

        private bool IsOnlyHaveDeliveryDart(Hotfix_LT.Data.MainLandEncounterTemplate met)
        {
            int[] func_ids = new int[3] { met.func_id_1, met.func_id_2, met.func_id_3 };
            if (met.func_id_1 == 0 && met.func_id_2 == 0 && met.func_id_3 == 0)
                return false;

            bool isOnlyDeliveryDart = true;

            for (var i = 0; i < func_ids.Length; i++)
            {
                int id = func_ids[i];

                if (id > 0)
                {
                    FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(id);
                    if (func.ui_model != "DeliveryDart")
                    {
                        isOnlyDeliveryDart = false;
                    }
}
            }

            return isOnlyDeliveryDart;
        }

        private string mCurrentCharacterAudioName = string.Empty;
        void PostAudioMessage(GameObject player, GameObject interactable)
        {
            if (IsNpcFuncMenu(player, interactable))
            {
                CharacterComponent Character = interactable.GetComponent<CharacterComponent>();
                mCurrentCharacterAudioName = Character.Model.audioName;
                if (!string.IsNullOrEmpty(mCurrentCharacterAudioName))
                {
                    FusionAudio.PostCharacterAnimation(mCurrentCharacterAudioName, "Dialogue_01", true);
                }
            }
        }

        //接任务  交任务
        private void PopTaskIntactMenu(GameObject player, GameObject interactable)
        {
            NpcTaskDataLookup task_datalookup = interactable.transform.GetDataLookupILRComponentInChildren<NpcTaskDataLookup>("Hotfix_LT.UI.NpcTaskDataLookup",true,false);
            if (task_datalookup != null && task_datalookup.mDL.DefaultDataID != null)
            {
                Hashtable data = new Hashtable() { { "taskid", task_datalookup.mDL.DefaultDataID } };
                GlobalMenuManager.Instance.Open("TaskChase", data);
            }
        }

        private bool IsNpcFuncMenu(GameObject player, GameObject interactable)
        {
            if (!IsHanterMonster(interactable) && !IsBeatMonster(interactable) && !IsGhost(interactable) && !IsWorldBoss(interactable))
                return true;
            return false;
        }

        //对话
        private void PopChaseIntactMenu(GameObject player, GameObject interactable)
        {

        }

        //打开功能页面
        private void PopFuncitonIntactMenu(GameObject player, GameObject interactable)
        {
            if (IsHanterMonster(interactable))
            {
                if (AllianceUtil.GetIsInTransferDart("ID_HANT_MONSTER"))
                {
                    return;
                }

                if (LTBountyTaskConversationController.sTriggerSpecialEvent)  //trigger special event
                {
                    GlobalMenuManager.Instance.Open("LTBountyTaskConversationUI");
                }
                else
                {
                    System.Action dialogueFinish = delegate ()
                    {
                        EnemyController _enemyController = interactable.GetComponent<EnemyController>();
                        AttackNpc(_enemyController, eBattleType.HantBattle);
                    };
                    GlobalMenuManager.Instance.Open("LTBountyTaskConversationUI", dialogueFinish);
                }
            }
            else if (IsBeatMonster(interactable))
            {
                if (AllianceUtil.GetIsInTransferDart("ID_BEAT_MONSTER"))
                {
                    return;
                }

                MessageTemplateManager.ShowMessage(902076, null, delegate (int result)
                {
                    if (result == 0)
                    {
                        if (interactable != null)
                        {
                            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
                            AttackNpc(_enemyController, eBattleType.HantBattle);//与悬赏公用一个战斗类型
                        }
                    }
                });
            }
            else if (IsGhost(interactable))
            {
                if (AllianceUtil.GetIsInTransferDart(null))
                {
                    return;
                }

                var activitytmp = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10073);
                if (activitytmp != null)
                {
                    if (!activitytmp.IsConditionOK())
                    {
                        Hashtable data = new Hashtable() { { "0", activitytmp.condition } };
                        MessageTemplateManager.ShowMessage(902009, data, null);
                        return;
                    }
                }

                if (!IsBusy(interactable))
                {
                    if (interactable != null)
                    {
                        EnemyController _enemyController = interactable.GetComponent<EnemyController>();
                        AttackNpc(_enemyController, eBattleType.GhostBattle);
                    }
                }
                else
                {
                    MessageTemplateManager.ShowMessage(902059);
                }
            }
            else if (IsAllianceMonster(interactable))
            {
                if (AllianceUtil.GetIsInTransferDart("ID_ALLIANCE_COPY"))
                {
                    return;
                }


                if (interactable != null)
                {
                    MessageTemplateManager.ShowMessage(902223, null, delegate (int result)
                    {
                        if (result == 0)
                        {
                            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
                            AttackNpc(_enemyController, eBattleType.AllianceCampaignBattle);
                        }
                    });
                }
            }
            else if (IsWorldBoss(interactable))
            {
                if (AllianceUtil.GetIsInTransferDart("ID_WORLD_BOSS"))
                {
                    return;
                }

                var activitytmp = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(LTWorldBossDataManager.ActivityId);
                if (activitytmp != null)
                {
                    var func = Data.FuncTemplateManager.Instance.GetFunc(activitytmp.funcId);
                    if (func!=null&&!func.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText,func.GetConditionStrSpecial());
                        return;
                    }
                }
                //1 可以战斗 2活动还没开始 3等待复活
                bool start = LTWorldBossDataManager.Instance.IsWorldBossStart();
                if (start)
                {
                    bool islive = LTWorldBossDataManager.Instance.IsLive();
                    if (islive)
                    {
                        /*bool needrevive = IsWorldBossNeedRevive();
    					if(needrevive)
    					{
                            BossReviveEvent e = new BossReviveEvent();
                            EventManager.instance.Raise(e);
    						return;
    					}*/
                        if (interactable != null)
                        {
                            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
                            //AttackNpc(_enemyController, eBattleType.BossBattle);
                            GlobalMenuManager.Instance.Open("LTWorldBossHud");
                        }
                    }
                    else
                    {
                        MessageTemplateManager.ShowMessage(902185);

                    }
                }
                else
                {
                    MessageTemplateManager.ShowMessage(902090);
                }
            }
            else
            {
            }
        }

        private void SetCameraNpcDialogueCameraView(Transform target)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null && mainCam.GetComponent<PlayerCameraComponent>())
            {
                PlayerCameraComponent playerCam = mainCam.GetComponent<PlayerCameraComponent>();
                playerCam.EnterNpcCameraMotion(target.gameObject);
            }
        }

        void ResetPlayerCameraState()
        {
            Camera mainCam = Camera.main;
            if (mainCam != null && mainCam.GetComponent<PlayerCameraComponent>())
            {
                PlayerCameraComponent playerCam = mainCam.GetComponent<PlayerCameraComponent>();
                playerCam.ResetCameraState();
            }
        }

        void AttackNpc(EnemyController ec, eBattleType battletype)
        {
            if (ec != null)
            {
                GameStateLoadGame loadState = GameStateManager.Instance.GetGameState<GameStateLoadGame>();
                if (GameFlowControlManager.Instance != null)
                {
                    // FsmStateAction[] actions = GameFlowControlManager.Instance.m_StateMachine.Fsm.ActiveState.Actions;
                    // if (actions == null)
                    //     return;
                    if (battletype != eBattleType.AllianceCampaignBattle)
                    {
                        // for (int i = 0; i < actions.Length; i++)
                        // {
                        //     if (actions[i] is BaseFlowAction && actions[i].State.Name == "MainLandView")
                        //     {
                        //         Hotfix_LT.Messenger.Raise("AttackEnemyImediatly", ec, battletype);
                        //     }
                        // }
                        
                        if (GameFlowControlManager.Instance.ActiveStateName == "MainLandView")
                        {
                            Hotfix_LT.Messenger.Raise("AttackEnemyImediatly", ec, battletype);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 是否是悬赏怪
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        private bool IsHanterMonster(GameObject interactable)
        {
            if (null == interactable) return false;
            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
            if (null == _enemyController) return false;
            Player.EnemyHotfixController ehc = _enemyController.HotfixController._ilrObject as Player.EnemyHotfixController;
            if (NPC_ROLE.HANTED == ehc.Role)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是主线任务怪
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        private bool IsBeatMonster(GameObject interactable)
        {
            if (null == interactable) return false;
            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
            if (null == _enemyController) return false;
            Player.EnemyHotfixController ehc = _enemyController.HotfixController._ilrObject as Player.EnemyHotfixController;
            if (NPC_ROLE.BEAT_MONSTER == ehc.Role)
            {
                return true;
            }
            return false;
        }

        private bool IsGhost(GameObject interactable)
        {
            if (null == interactable) return false;
            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
            if (null == _enemyController) return false;
            Player.EnemyHotfixController ehc = _enemyController.HotfixController._ilrObject as Player.EnemyHotfixController;
            if (NPC_ROLE.GHOST == ehc.Role)
            {
                return true;
            }
            return false;
        }

        private bool IsAllianceMonster(GameObject interactable)
        {
            if (null == interactable)
                return false;
            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
            if (null == _enemyController)
                return false;
            Player.EnemyHotfixController ehc = _enemyController.HotfixController ._ilrObject as Player.EnemyHotfixController;
            if (NPC_ROLE.ALLIANCE_CAMPAIGN_ENEMY == ehc.Role || NPC_ROLE.ALLIANCE_CAMPAIGN_BOSS == ehc.Role)
            {
                return true;
            }
            return false;
        }

        private bool IsWorldBoss(GameObject interactable)
        {
            if (null == interactable) return false;
            EnemyController _enemyController = interactable.GetComponent<EnemyController>();
            if (null == _enemyController) return false;
            Player.EnemyHotfixController ehc = _enemyController.HotfixController._ilrObject as Player.EnemyHotfixController;
            if (NPC_ROLE.WORLD_BOSS == ehc.Role)
            {
                return true;
            }
            return false;
        }

        private bool IsWorldBossNeedRevive()
        {
            int reviveTime = 0;
            DataLookupsCache.Instance.SearchIntByID("world_boss.rt", out reviveTime);
            if (EB.Time.Now >= reviveTime)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsBusy(GameObject interactable)
        {
            if (null == interactable) return false;
            return MainLandLogic.GetInstance().IsBusy(interactable.name);
        }

        private bool IsLevelOk(int level)
        {
            int userlevel = BalanceResourceUtil.GetUserLevel();
            if (userlevel >= level) return true;
            else return false;
        }

        /// <summary>
        /// 是否是功能NPC
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        private bool IsFunctionNPC(string npc_id, string scene_name)
        {
            Hotfix_LT.Data.MainLandEncounterTemplate met = Hotfix_LT.Data.SceneTemplateManager.GetMainLandsNPCData(scene_name, npc_id);
            if (met != null)
            {
                if (met.func_id_1 > 0 || met.func_id_2 > 0 || met.func_id_3 > 0 || met.dialogue_id > 0) return true;
            }
            return false;
        }

    }
}
