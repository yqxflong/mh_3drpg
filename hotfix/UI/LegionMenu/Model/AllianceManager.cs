using System;
using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using Sirenix.Utilities;


namespace Hotfix_LT.UI
{
    public class AlliancesManager : ManagerUnit, IManagerUnitUpdatable
    {
        private Dictionary<string, AllianceDataMeta> mDataMetas = new Dictionary<string, AllianceDataMeta>();

        private static AlliancesManager sInstance = null;

        public static AlliancesManager Instance
        {
            get { return sInstance = sInstance ?? LTHotfixManager.GetManager<AlliancesManager>(); }
        }

        public AllianceAPI Api { get; private set; }

        // public static int AccountAllianceIdFromILR()
        // {
        //     if (Instance != null)
        //     {
        //         return (int)Instance.Account.AllianceId;
        //     }

        //     return 0;
        // }

        // public static int AccountStateFromILR()
        // {
        //     if (Instance != null)
        //     {
        //         return (int)Instance.Account.State;
        //     }

        //     return 0;
        // }

        public AllianceAccount Account { get; private set; }

        public AllianceAccountApplies AccountApplies { get; private set; }

        public AllianceDetail Detail { get; private set; }

        public AllianceDetailApplies DetailApplies { get; private set; }

        public AllianceDetailMembers DetailMembers { get; private set; }

        public AllianceItems Alliances { get; private set; }

        public AllianceConfig Config { get; private set; }

        public AllianceSkillList Skills { get; private set; }

        //public AllianceBattleConfig BattleConfig
        //{
        //    get; private set;
        //}

        public AllianceBattleMembers BattleMembers { get; private set; }

        public AllianceBattleInfo BattleInfo { get; private set; }

        public AllianceBattleAwards BattleAwards { get; private set; }

        public AllianceForwardSelectionMembers ForwardSelectionMembers { get; private set; }

        public AllianceFinalsMembers FinalsMembers { get; private set; }

        public AllianceDartConfig DartConfig { get; private set; }

        // public static int DartDataStateFromILR()
        // {
        //     if (Instance != null)
        //     {
        //         return (int)Instance.DartData.State;
        //     }

        //     return 0;
        // }

        public AllianceDartData DartData { get; private set; }

        // public static string DartNameFromILR()
        // {
        //     if (Instance != null)
        //     {
        //         return Instance.TransferDartInfo.GetCurrentDart().DartName;
        //     }

        //     return string.Empty;
        // }

        // public static string TargetNpcFromILR()
        // {
        //     if (Instance != null)
        //     {
        //         return Instance.TransferDartInfo.GetCurrentDart().TargetNpc;
        //     }

        //     return string.Empty;
        // }

        // public static int NextTransferPointFromILR()
        // {
        //     if (Instance != null)
        //     {
        //         return Instance.TransferDartInfo.NextTransferPoint;
        //     }

        //     return 0;
        // }

        public AllianceTransferDartInfo TransferDartInfo { get; private set; }

        public AllianceRobDartInfo RobDartInfo { get; private set; }

        public AllianceApplyHelpInfo ApplyHelpInfo { get; private set; }

        public AllianceHelpApplyInfo HelpApplyInfo { get; private set; }

        public AllianceCopyBaseInfo CopyInfo { get; private set; }

        public AllianceInventoryInfo InventoryInfo { get; private set; }

        public AllianceCopyRankMembers CopyRankMembers { get; private set; }

        public AllianceTotalMessage TotalMessage { get; private set; }

        public PlayerFormation PlayerFormationInfo { get; private set; }

        public LegionDonateInfo CurDonateInfo { get; private set; }

        //public AllianceTechnology AllianceTechnologyInfo
        //{
        //    get; private set;
        //}

        // public bool UpdateOffline
        // {
        //     get { return false; }
        // }

        public static string accountDataId = "alliance.account";

        public static string accountAppliesDataId = "alliance.accountApplies";
        public static string detailDataId = "alliance.detail";
        public static string detailAppliesDataId = "alliance.detailApplies";
        public static string detailMembersDataId = "alliance.detailMembers";
        public static string listDataId = "alliance.list";
        public static string configDataId = "alliance.config";
        public static string skillDataId = "alliance.skill";
        public static string inviteCatalog = "alliance_join";

        public static string battleAwardInfoId = "alliance.battleAwardInfo";

        // public static string battleConfigId = "alliancesBattle.configs";
        public static string battleMembersDataId = "alliancesBattle.battleFields";
        public static string battleInfoDataId = "alliancesBattle.info";
        public static string battleForwardSelectionId = "alliancesBattle.forwardSelectionList";
        public static string battleFinalsId = "alliancesBattle.finalsInfo";
        public static string copyInfoDataId = "alliance.campaignInfo";
        public static string copyRankDataId = "alliance.campaignRankInfo";
        public static string inventoryDataId = "alliance.inventoryInfo";
        public static string messageDataID = "alliance.messages";
        public static string playerFormationId = "alliance.PlayerTeam";

        public static string curDonateDataId = "alliance.todayDonateTimes";
        //public static string allianceTechnologyId = "alliance.technology";

        //运镖、劫镖
        public static string dartConfigDataId = "escortAndRob.config";
        public static string dartDataId = "escortAndRob.info";
        public static string transferDartDataId = "escortAndRob.transferDart";
        public static string robDartDataId = "escortAndRob.robDart";
        public static string applyHelpDataId = "escortAndRob.applyHelp";
        public static string helpApplyDataId = "escortAndRob.helpApply";

        #region 错误码

        // public static int CodeHcUnenough = 901030; // 您的钻石不足，是否充值？
        // public static int CodeGoldUnenough = 901031; // 您的金币不足，是否购买？

        #endregion

        public override void Initialize(EB.Sparx.Config config)
        {
            Instance.Api = new AllianceAPI();
            Instance.Api.ErrorHandler += ErrorHandler;

            Account = GameDataSparxManager.Instance.Register<AllianceAccount>(accountDataId);
            AccountApplies = GameDataSparxManager.Instance.Register<AllianceAccountApplies>(accountAppliesDataId);
            Detail = GameDataSparxManager.Instance.Register<AllianceDetail>(detailDataId);
            DetailApplies = GameDataSparxManager.Instance.Register<AllianceDetailApplies>(detailAppliesDataId);
            DetailMembers = GameDataSparxManager.Instance.Register<AllianceDetailMembers>(detailMembersDataId);
            Alliances = GameDataSparxManager.Instance.Register<AllianceItems>(listDataId);
            Config = GameDataSparxManager.Instance.Register<AllianceConfig>(configDataId);
            Skills = GameDataSparxManager.Instance.Register<AllianceSkillList>(skillDataId);

            //BattleConfig = GameDataSparxManager.Instance.Register<AllianceBattleConfig>(battleConfigId);

            BattleMembers = GameDataSparxManager.Instance.Register<AllianceBattleMembers>(battleMembersDataId);
            BattleInfo = GameDataSparxManager.Instance.Register<AllianceBattleInfo>(battleInfoDataId);
            BattleAwards = GameDataSparxManager.Instance.Register<AllianceBattleAwards>(battleAwardInfoId);
            ForwardSelectionMembers =
                GameDataSparxManager.Instance.Register<AllianceForwardSelectionMembers>(battleForwardSelectionId);
            FinalsMembers = GameDataSparxManager.Instance.Register<AllianceFinalsMembers>(battleFinalsId);
            CopyInfo = GameDataSparxManager.Instance.Register<AllianceCopyBaseInfo>(copyInfoDataId);
            InventoryInfo = GameDataSparxManager.Instance.Register<AllianceInventoryInfo>(inventoryDataId);
            CopyRankMembers = GameDataSparxManager.Instance.Register<AllianceCopyRankMembers>(copyRankDataId);
            TotalMessage = GameDataSparxManager.Instance.Register<AllianceTotalMessage>(messageDataID);
            PlayerFormationInfo = GameDataSparxManager.Instance.Register<PlayerFormation>(playerFormationId);
            CurDonateInfo = GameDataSparxManager.Instance.Register<LegionDonateInfo>(curDonateDataId);
            DartConfig = GameDataSparxManager.Instance.Register<AllianceDartConfig>(dartConfigDataId);
            DartData = GameDataSparxManager.Instance.Register<AllianceDartData>(dartDataId);
            TransferDartInfo = GameDataSparxManager.Instance.Register<AllianceTransferDartInfo>(transferDartDataId);
            RobDartInfo = GameDataSparxManager.Instance.Register<AllianceRobDartInfo>(robDartDataId);
            ApplyHelpInfo = GameDataSparxManager.Instance.Register<AllianceApplyHelpInfo>(applyHelpDataId);
            HelpApplyInfo = GameDataSparxManager.Instance.Register<AllianceHelpApplyInfo>(helpApplyDataId);
            //AllianceTechnologyInfo = GameDataSparxManager.Instance.Register<AllianceTechnology>(allianceTechnologyId);

            mDataMetas[accountDataId] = new AllianceDataMeta(accountDataId);
            mDataMetas[accountAppliesDataId] = new AllianceDataMeta(accountAppliesDataId);
            mDataMetas[detailDataId] = new AllianceDataMeta(detailDataId, RequestAllianceDetail, 300);
            mDataMetas[detailAppliesDataId] = new AllianceDataMeta(detailAppliesDataId, RequestAllianceApplyList);
            mDataMetas[detailMembersDataId] = new AllianceDataMeta(detailMembersDataId, RequestAllianceMemberList, 300);
            mDataMetas[listDataId] = new AllianceDataMeta(listDataId, RequestAllianceList);
        }

        public override void OnLoggedIn()
        {
            //需要等LoginManager迁移

            isRequestDonate = false;
            if (autorefreshTimer != 0) ILRTimerManager.instance.RemoveTimerSafely(ref autorefreshTimer);
            Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;

            object alliance = loginData["alliance"];
            if (alliance == null)
            {
                EB.Debug.LogWarning("AlliancesManager.OnLoggedIn: alliance not found in LoginData");
                return;
            }

            loginData.Remove("alliance");
            CopyInfo.InitData();
            MarkAllDirty();
        }

        public override void OnEnteredBackground()
        {

        }

        public override void OnEnteredForeground()
        {
        }

        public override void Connect()
        {
            //State = EB.Sparx.SubSystemState.Connected;
            // Connect is before OnLoggedIn & after Initialize
            var im = LTHotfixManager.GetManager<InvitesManager>();
            im.OnAcceptListener += OnAccept;
            im.OnRejectListener += OnReject;
            im.OnRemoveListener += OnRemove;
            im.OnRequestListener += OnRequest;
            im.OnInviteListener += OnInvite;

            im.OnRemoveTargetListener += OnRemoveTarget;
        }

        public override void Disconnect(bool isLogout)
        {
            var im = LTHotfixManager.GetManager<InvitesManager>();
            im.OnAcceptListener -= OnAccept;
            im.OnRejectListener -= OnReject;
            im.OnRemoveListener -= OnRemove;
            im.OnRequestListener -= OnRequest;
            im.OnInviteListener -= OnInvite;

            im.OnRemoveTargetListener -= OnRemoveTarget;
            hireDatas = null;
            //State = EB.Sparx.SubSystemState.Disconnected;
        }

        public override void Async(string message, object payload)
        {
            LegionLogic.GetInstance().Async(message, payload);
            // payload already processed by PushManager/GameDataManager
            switch (message)
            {
                case "join": // (除新人之外所有人)新成员加入
                {
                    LegionLogic.GetInstance().IsHaveNewEvent = true;
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.haveevent, 1);
                    string playerName = EB.Dot.String("member.name", payload, string.Empty);
                    if (!string.IsNullOrEmpty(playerName))
                    {
                        //901051	{0}加入帮派。
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("0", playerName);
                        MessageTemplateManager.ShowMessage(901051, ht, null);
                    }
                    else
                    {
                        EB.Debug.LogError("alliance join name=null");
                    }
                }
                    break;
                case "levelup":
                {
                    string level = EB.Dot.String("member.aLevel", payload, string.Empty);
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("0", level);
                    MessageTemplateManager.ShowMessage(902269, ht, null);
                }
                    break;
                case "leave": //（除操作者之外所有人）成员离开
                case "kick": //（除操作者之外所有人）成员被踢
                case "promote": //（除操作者之外所有人）升职（提升为精英或帮主）
                case "demote": //（除操作者之外所有人）降职（降为精英或普通成员）
                    EB.Debug.Log(string.Format("<color=green>alliance push message={0}</color>", message));
                    LegionLogic.GetInstance().IsHaveNewEvent = true;
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.haveevent, 1);
                    switch (message)
                    {
                        case "leave":
                            //901052  { 0}退出了帮派。
                            string targetName = EB.Dot.String("alliance.op.targetName", payload, string.Empty);
                            if (!string.IsNullOrEmpty(targetName))
                            {
                                targetName = string.Format("[{0}]{1}[-]", LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal, targetName);
                                var ht = Johny.HashtablePool.Claim();
                                ht.Add("0", targetName);
                                MessageTemplateManager.ShowMessage(901052, ht, null);
                            }

                            break;
                        case "kick":
                            //901053	{0}被请离了帮派。
                            string tn = EB.Dot.String("alliance.op.targetName", payload, string.Empty);
                            if (!string.IsNullOrEmpty(tn))
                            {
                                tn = string.Format("[{0}]{1}[-]", LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal, tn);
                                var ht = Johny.HashtablePool.Claim();
                                ht.Add("0", tn);
                                MessageTemplateManager.ShowMessage(901053, ht, null);
                            }

                            break;
                        case "promote":
                        case "demote":
                            Hashtable detailMembers = EB.Dot.Object(detailMembersDataId, payload, null);
                            //long targetUid = EB.Dot.Long("alliance.op.targetUid", payload, 0);
                            //string operName = EB.Dot.String("alliance.op.operName", payload, string.Empty);
                            if (detailMembers != null && detailMembers.Count == 1)
                            {
                                long uid;
                                foreach (string k in detailMembers.Keys)
                                {
                                    if (long.TryParse(k, out uid))
                                    {
                                        AllianceMember m = DetailMembers.Find(uid);
                                        if (m != null)
                                        {
                                            eAllianceMemberRole role =
                                                (eAllianceMemberRole) EB.Dot.Integer(k + ".role", detailMembers, 1);
                                            if (role == eAllianceMemberRole.Owner)
                                            {
                                                var ht = Johny.HashtablePool.Claim();
                                                ht.Add("0", m.Name);
                                                ht.Add("1", EB.Localizer.GetString("ID_ALLIANCE_ROLE_OWNER_NAME"));
                                                MessageTemplateManager.ShowMessage(901049, ht, null);
                                                Johny.HashtablePool.Release(ht);
                                            }
                                            else if (role == eAllianceMemberRole.ExtraOwner)
                                            {
                                                var ht = Johny.HashtablePool.Claim();
                                                ht.Add("0", m.Name);
                                                ht.Add("1",
                                                    EB.Localizer.GetString("ID_ALLIANCE_ROLE_EXTRA_OWNER_NAME"));
                                                MessageTemplateManager.ShowMessage(901049, ht, null);
                                                Johny.HashtablePool.Release(ht);
                                            }
                                            else if (role == eAllianceMemberRole.Admin)
                                            {
                                                var ht = Johny.HashtablePool.Claim();
                                                ht.Add("0", m.Name);
                                                ht.Add("1", EB.Localizer.GetString("ID_ALLIANCE_ROLE_ADMIN_NAME"));
                                                MessageTemplateManager.ShowMessage(901049, ht, null);
                                                Johny.HashtablePool.Release(ht);
                                            }
                                            else if (role == eAllianceMemberRole.Member)
                                            {
                                                var ht = Johny.HashtablePool.Claim();
                                                ht.Add("0", m.Name);
                                                ht.Add("1",
                                                    EB.Localizer.GetString("ID_ALLIANCE_ROLE_COMMON_MEMBER_NAME"));
                                                MessageTemplateManager.ShowMessage(901049, ht, null);
                                                Johny.HashtablePool.Release(ht);
                                            }
                                            else
                                            {
                                                EB.Debug.LogError("alliance op role error");
                                            }
                                        }
                                        else
                                        {
                                            EB.Debug.LogError("alliance op DetailMembers.Find(uid) =null");
                                        }
                                    }
                                }
                            }

                            break;
                    }

                    break;
                case "update": //（除操作者之外所有人）帮派信息更新（改名等）
                    string notice = EB.Dot.String(detailDataId + ".notice", payload, string.Empty);
                    if (!string.IsNullOrEmpty(notice))
                    {
                        MessageTemplateManager.ShowMessage(901057);
                    }

                    break;
                case "memberupdated": //（所有人）成员信息更新（成员改名）
                    break;
                case "battleFieldUpdate":
                    break;
                case "battleFieldChangePosition":
                    break;
            }
        }

        public void Update()
        {
            DataUpdateCheck();

            UpdateDartState();
        }

        public bool isRequestDonate { get; private set; }
        public int autorefreshTimer;
        public void FristRequestDonate(System.Action callback = null)
        {
            if (isRequestDonate)
            {
                return;
            }

            if (!LegionModel.GetInstance().isJoinedLegion)
            {
                isRequestDonate = true;
                return;
            }

            LegionLogic.GetInstance().OnSendGetCurDonateInfo(delegate(Hashtable alliance)
            {
                if (alliance != null)
                {
                    GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
                    callback?.Invoke();
                    isRequestDonate = true;
                    int nextrefreshtime = 86400 - (EB.Time.Now + 3600 * ZoneTimeDiff.GetTimeZone()) % 86400 + 10;//整点刷新，延迟10秒请求数据
                    if (nextrefreshtime > 0 && autorefreshTimer == 0)
                    {
                        autorefreshTimer = ILRTimerManager.instance.AddTimer(nextrefreshtime * 1000, 1, AutoRefresh);
                    }
                }
            });
        }

        private void AutoRefresh(int seq)
        {
            autorefreshTimer = 0;
            isRequestDonate = false;
            FristRequestDonate(delegate 
            {
                if (CurDonateInfo != null) CurDonateInfo.ClearDonateChestState();
            });          
        }

        private void DataUpdateCheck()
        {
            var iter = mDataMetas.GetEnumerator();
            while (iter.MoveNext())
            {
                var pair = iter.Current;

                if (pair.Value.LifeTime > 0 && UnityEngine.Time.realtimeSinceStartup > pair.Value.LifeTime)
                {
                    MarkDirty(pair.Key);
                }

                if (IsDirty(pair.Key) && GameDataSparxManager.Instance.HasListener(pair.Key))
                {
                    ClearDirty(pair.Key);

                    if (pair.Value.Updater != null)
                    {
                        pair.Value.Updater();
                    }
                }
            }

            iter.Dispose();
        }

        private void UpdateDartState()
        {
            if (DartData.State == eAllianceDartCurrentState.Transfer ||
                DartData.State == eAllianceDartCurrentState.Transfering)
            {
                System.TimeSpan transferCountdownTs =
                    System.TimeSpan.FromSeconds(TransferDartInfo.TransferEndTs - EB.Time.Now);
                if (transferCountdownTs.TotalSeconds < 0 && GlobalMenuManager.Instance != null)
                {
                    TransferDartMember dart = TransferDartInfo.Find(TransferDartInfo.GetCurrentDartId());
                    if (dart != null)
                    {
                        dart.State = eAllianceTransferState.Fail;
                        GameDataSparxManager.Instance.SetDirty(transferDartDataId);
                    }
                    else
                    {
                        EB.Debug.LogError("GetCurrentDartId fail");
                    }

                    DartData.State = eAllianceDartCurrentState.None;

                    if (LTMainMenuHudController.Instance != null)
                    {
                        AlliancesManager.Instance.Finish(DartData.CurrentDartId, null);
                    }
                    else
                    {
                        AllianceEscortUtil.SetEscortResultHudCache(eDartResultType.TransferFailByTimeout);
                    }

                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.TransferDartEndEvent);

                    if (PlayerManager.LocalPlayerController() != null)
                    {
                        PlayerManager.LocalPlayerController().transform
                            .GetMonoILRComponent<Player.PlayerHotfixController>().StopTransfer();
                    }

                }
            }
        }

        // private IEnumerator DelayRefreshDartState(string dartId)
        // {
        //     yield return new UnityEngine.WaitForSeconds(5);

        //     Api.RefreshTransferTaskList(dartId, FetchDataHandler);
        // }

        private void OnInvite(InviteData invite)
        {
            if (invite.Catalog != inviteCatalog)
            {
                return;
            }
        }

        private void OnRequest(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.ESCORT_HELP)
            {
                var localUid = AllianceUtil.GetLocalUid();
                for (var i = 0; i < invite.ReceiverUids.Length; i++)
                {
                    var receiveId = invite.ReceiverUids[i];
                    if (localUid == receiveId)
                        GetHelpApplyInfo();
                }
            }

            if (invite.Catalog != inviteCatalog)
            {
                return;
            }

            if (AllianceUtil.IsOneOfAdmin(AllianceUtil.GetLocalUid()))
            {
                MarkDirty(detailDataId);
                MarkDirty(detailAppliesDataId);

                Detail.ApplyCount++;
                LegionLogic.GetInstance().IsHaveApplyMember();
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnAllianceApplyEvent, 1);
            }
        }

        private void OnAccept(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.ESCORT_HELP)
            {
                ///通知申请人已援助,并且更新列表

                long agreeUid = invite.OpUid;
                string helper = EB.Dot.String(agreeUid + ".name", invite.Infos, string.Empty);
                if (string.IsNullOrEmpty(helper))
                {

                    EB.Debug.LogError("agree Name=null");
                    return;
                }

                //902088	{0}同意了你的支援申请。
                long localUid = AllianceUtil.GetLocalUid();
                if (!agreeUid.Equals(localUid))
                    MessageTemplateManager.ShowMessage(902088, helper, null);
                return;
            }

            if (invite.Catalog != inviteCatalog)
            {
                return;
            }

            if (AllianceUtil.IsLocalPlayer(invite.SenderUid))
            {
                // update state
                int aid = int.Parse(invite.OrgnizationId.ToString());
                Account.State = eAllianceState.Joined;
                Account.AllianceId = aid;
                Account.ApplyTime = invite.SendTime;
                Account.JoinTime = AllianceUtil.Now();

                var history = Account.History;
                System.Array.Resize(ref history, history.Length + 1);
                history[Account.History.Length] = aid;
                Account.History = history;

                GameDataSparxManager.Instance.SetDirty(accountDataId);
            }
            else
            {
                // update members
                MarkDirty(detailDataId);

                // update requests
                if (AllianceUtil.IsOneOfAdmin(AllianceUtil.GetLocalUid()))
                {
                    if (DetailApplies.Find(invite.SenderUid) != null)
                    {
                        Detail.ApplyCount--;
                        LegionLogic.GetInstance().IsHaveApplyMember();
                        DetailApplies.Remove(invite.SenderUid);

                        GameDataSparxManager.Instance.SetDirty(detailDataId);
                        GameDataSparxManager.Instance.SetDirty(detailAppliesDataId);
                    }

                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnAllianceApplyEvent, 0);
                }
            }
        }

        private void OnReject(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.ESCORT_HELP)
            {
                //GetApplyHelpInfo();  

                long rejectUid = invite.OpUid;
                string rejecter = EB.Dot.String(rejectUid + ".name", invite.Infos, string.Empty);
                if (string.IsNullOrEmpty(rejecter))
                {
                    EB.Debug.LogError("reject Name=null");
                    return;
                }

                //902092	{0}拒绝了你的支援请求。
                long localUid = AllianceUtil.GetLocalUid();
                if (!rejectUid.Equals(localUid))
                    MessageTemplateManager.ShowMessage(902092, rejecter, null);
                return;
            }

            if (invite.Catalog != inviteCatalog)
            {
                return;
            }

            if (AllianceUtil.IsLocalPlayer(invite.SenderUid))
            {
                // update requests
                int aid = int.Parse(invite.OrgnizationId.ToString());
                if (AccountApplies.Find(aid) != null)
                {
                    AccountApplies.Remove(aid);
                    GameDataSparxManager.Instance.SetDirty(accountAppliesDataId);

                    //EventManager.instance.Raise(new LegionSearchMessageCallBack(1));
                }

                return;
            }

            if (AllianceUtil.IsOneOfAdmin(AllianceUtil.GetLocalUid()))
            {
                // update admin requests
                if (DetailApplies.Find(invite.SenderUid) != null)
                {
                    Detail.ApplyCount--;
                    LegionLogic.GetInstance().IsHaveApplyMember();
                    DetailApplies.Remove(invite.SenderUid);

                    GameDataSparxManager.Instance.SetDirty(detailDataId);
                    GameDataSparxManager.Instance.SetDirty(detailAppliesDataId);

                }

                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnAllianceApplyEvent, 0);
            }
        }

        private void OnRemove(InviteData invite)
        {
            if (invite.Catalog == InvitesMessage.ESCORT_HELP)
            {
                GetHelpApplyInfo(); //移除帮助申请人
            }

            if (invite.Catalog != inviteCatalog)
            {
                return;
            }

            if (AllianceUtil.IsLocalPlayer(invite.SenderUid))
            {
                // update requests
                int aid = int.Parse(invite.OrgnizationId.ToString());
                if (AccountApplies.Find(aid) != null)
                {
                    AccountApplies.Remove(aid);
                    GameDataSparxManager.Instance.SetDirty(accountAppliesDataId);
                }

                return;
            }

            if (AllianceUtil.IsOneOfAdmin(AllianceUtil.GetLocalUid()))
            {
                // update admin requests
                if (DetailApplies.Find(invite.SenderUid) != null)
                {
                    Detail.ApplyCount--;
                    LegionLogic.GetInstance().IsHaveApplyMember();
                    DetailApplies.Remove(invite.SenderUid);

                    GameDataSparxManager.Instance.SetDirty(detailDataId);
                    GameDataSparxManager.Instance.SetDirty(detailAppliesDataId);
                }

                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnAllianceApplyEvent, 0);
            }
        }

        private void OnRemoveTarget(RemoveData remove)
        {
            if (remove.Catalog == InvitesMessage.ESCORT_HELP)
            {
                for (var i = 0; i < HelpApplyInfo.HelpApplyList.Count; i++)
                {
                    var help = HelpApplyInfo.HelpApplyList[i];

                    if (help.Uid.Equals(remove.TargetUid))
                    {
                        HelpApplyInfo.Remove(remove.Id);
                        GameDataSparxManager.Instance.SetDirty(helpApplyDataId);
                        break;
                    }
                }
            }
        }

        private bool ErrorHandler(EB.Sparx.Response response, EB.Sparx.eResponseCode errCode)
        {
            return false;
        }

        // public static void OnConfigUpdated(string name, object payload)
        // {
        //     Instance.Config.OnUpdate(payload);
        // }
        // public void OnUpdatedConfig(AllianceConfig config)
        // {
        //     Config = config;
        // }
        private void FetchDataHandler(Hashtable alliance)
        {
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
            }
        }

        // private void MergeDataHandler(Hashtable alliance)
        // {
        //     if (alliance != null)
        //     {
        //         GameDataSparxManager.Instance.ProcessIncomingData(alliance, true);
        //     }
        // }

        public void MarkDirty(string flag)
        {
            mDataMetas[flag].Dirty = true;
            mDataMetas[flag].LifeTime = 0f;
        }

        public void ClearDirty(string flag)
        {
            mDataMetas[flag].Dirty = false;
            if (mDataMetas[flag].LifeDuration > 0f)
            {
                mDataMetas[flag].LifeTime = UnityEngine.Time.realtimeSinceStartup + mDataMetas[flag].LifeDuration;
            }
        }

        public bool IsDirty(string flag)
        {
            return mDataMetas[flag].Dirty;
        }

        public void MarkAllDirty()
        {
            foreach (var pair in mDataMetas)
            {
                pair.Value.Dirty = true;
                pair.Value.LifeTime = 0f;
            }
        }

        // public void ClearAllDirty()
        // {
        //     foreach (var pair in mDataMetas)
        //     {
        //         pair.Value.Dirty = false;
        //         pair.Value.LifeTime = 0f;
        //     }
        // }

        public void RequestAllianceList()
        {
            Api.GetAllianceList(FetchDataHandler);
        }

        public void RequestAllianceApplyList()
        {
            Api.GetAllianceApplyList(Account.AllianceId, FetchDataHandler);
        }

        public void RequestAllianceDetail()
        {
            if (AllianceUtil.IsJoinedAlliance)
                Api.GetAllianceDetail(Account.AllianceId, FetchDataHandler);
            else
                EB.Debug.LogWarning("not have alliance so cannot req alliance detailData");
        }

        public void RequestAllianceMemberList()
        {
            if (AllianceUtil.IsJoinedAlliance)
                Api.GetAllianceMemberList(Account.AllianceId, FetchDataHandler);
            else
                EB.Debug.LogWarning("not have alliance so cannot req AllianceMemberList");
        }

        // public void SearchAllianceList(string input)
        // {
        //     Api.SearchAllianceList(input, FetchDataHandler);
        // }

        // public void CreateAlliance(string name)
        // {
        //     Api.CreateAlliance(name, 0, MergeDataHandler);

        // }

        // public void ApplyRequest(int aid)
        // {
        //     Api.ApplyRequest(aid, MergeDataHandler);
        // }

        // public void CancelRequest(int aid)
        // {
        //     Api.CancelRequest(aid, MergeDataHandler);
        // }

        // public void AcceptAllianceApplyRequest(long uid)
        // {
        //     Api.AcceptAllianceApplyRequest(Account.AllianceId, uid, MergeDataHandler);
        // }

        // public void RejectAllianceApplyRequest(long uid)
        // {
        //     Api.RejectAllianceApplyRequest(Account.AllianceId, uid, false, MergeDataHandler);
        // }

        // public void LeaveAlliance()
        // {
        //     Api.LeaveAlliance(Account.AllianceId, MergeDataHandler);
        // }

        // public void AllianceDonate(int gold, int hc)
        // {
        //     Api.AllianceDonate(Account.AllianceId, gold, hc, MergeDataHandler);
        // }

        // public void AdminRemoveMember(long uid)
        // {
        //     Api.AllianceAdminRemoveMember(Account.AllianceId, uid, MergeDataHandler);
        // }

        // public void OwnerAppoint(long uid, eAllianceMemberRole role)
        // {
        //     Api.AllianceOwnerPromote(Account.AllianceId, uid, ToStrRole(role), MergeDataHandler);
        // }

        // public void OwnerDegrade(long uid, eAllianceMemberRole role)
        // {
        //     Api.AllianceOwnerDemote(Account.AllianceId, uid, ToStrRole(role), MergeDataHandler);
        // }

        // private string ToStrRole(eAllianceMemberRole role)
        // {
        //     string strRole = "member";
        //     switch (role)
        //     {
        //         case eAllianceMemberRole.Member:
        //             strRole = "member";
        //             break;
        //         case eAllianceMemberRole.Admin:
        //             strRole = "admin";
        //             break;
        //         case eAllianceMemberRole.ExtraOwner:
        //             strRole = "vice_owner";
        //             break;
        //     }
        //     return strRole;
        // }

        // public void OwnerTransfer(long uid)
        // {
        //     Api.AllianceOwnerTransfer(Account.AllianceId, uid, MergeDataHandler);
        // }

        // public void AdminLevelup(int from, int to)
        // {
        //     Api.AdminLevelup(Account.AllianceId, from, to, MergeDataHandler);
        // }

        // public void AdminRename(string name, System.Action callback)
        // {
        //     Api.AdminRename(Account.AllianceId, name, delegate (Hashtable result)
        //     {
        //         MergeDataHandler(result);
        //         callback();
        //     });
        // }

        // public void AdminNotice(string notice)
        // {
        //     Api.AdminNotice(Account.AllianceId, notice, MergeDataHandler);
        // }

        // public void AdminTechLevelUp(int from, int to)
        // {
        //     Api.AdminTechLevelUp(Account.AllianceId, from, to, FetchDataHandler);
        // }

        // public void GetSkillList()
        // {
        //     long uid =LoginManager.Instance.LocalUserId.Value;
        //     Api.GetSkillList(uid, Account.AllianceId, FetchDataHandler);
        // }

        // public void LearnSkill(int fromLevel, string skillName, System.Action<bool> callBack)
        // {
        //     long uid = LoginManager.Instance.LocalUserId.Value;
        //     Api.LearnSkill(uid, Account.AllianceId, fromLevel, skillName, delegate (Hashtable result)
        //     {
        //         if (result == null)
        //         {
        //             callBack(true);
        //         }
        //         else
        //         {
        //             FetchDataHandler(result);
        //             callBack(true);
        //         }
        //     });
        // }

        // //五族大战-
        // public void BattleApply(System.Action<bool> callBack)
        // {
        //     Api.BattleApply(Account.AllianceId, delegate (Hashtable result)
        //     {
        //         MergeDataHandler(result);
        //         callBack(result != null);
        //     });
        // }

        // public void GetBattleBaseInfo(System.Action callBack)
        // {
        //     Api.GetBattleBaseInfo(Account.AllianceId, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         if (result == null)
        //             EB.Debug.LogError("GetBattleBaseInfo result=null");
        //         if (callBack != null)
        //             callBack();
        //     });
        // }

        // public void EnterBattleField(System.Action<bool> callBack)
        // {
        //     Api.EnterBattleField(Account.AllianceId, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callBack(result != null);
        //     });
        // }

        // public void LeaveBattleField()
        // {
        //     Api.LeaveBattleField(Account.AllianceId, FetchDataHandler);
        // }

        // public void ChangePosition(Hashtable from, Hashtable to)
        // {
        //     Api.ChangePosition(Account.AllianceId, from, to, FetchDataHandler);
        // }

        // public void GetForwardSelectionInfo()
        // {
        //     Api.GetForwardSelectionInfo(Account.AllianceId, FetchDataHandler);
        // }

        // public void GetFinalsInfo()
        // {
        //     Api.GetFinalsInfo(Account.AllianceId, FetchDataHandler);
        // }

        // public void GetAwardInfo(System.Action callBack)
        // {
        //     Api.GetAwardInfo(Account.AllianceId, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callBack();
        //     });
        // }

        // public void AssignBattleAward(long uid, string boxName, System.Action<bool> callBack)
        // {
        //     Api.AssignBattleAward(Account.AllianceId, uid, boxName, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callBack(result != null);
        //     });
        // }

        // public void GiveMonthCard(long uid, System.Action<bool> callBack)
        // {
        //     Api.GiveMonthCard(Account.AllianceId, uid, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callBack(result != null);
        //         if (result != null)
        //             DataLookupsCache.Instance.CacheData(result);
        //     });
        // }

        // public void GetAllianceFightRank(System.Action<Hashtable> callback)
        // {
        //     Api.GetAllianceFightRank(Account.AllianceId, delegate (Hashtable result)
        //     {
        //         callback(result);
        //     });
        // }

        #region allianceDart

        public void GetTransferInfo(System.Action<bool> callBack = null)
        {
            long uid = AllianceUtil.GetLocalUid();
            if (Account.AllianceId <= 0)
            {
                EB.Debug.Log("GetTransferInfo GetTransferInfo is Error Account.AllianceId :{0}", Account.AllianceId);
                return;
            }

            Api.GetTransferInfo(Account.AllianceId, uid, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                if (callBack != null)
                {
                    callBack(result != null);
                }
            });
        }

        public void Refresh()
        {
            int CurHC = BalanceResourceUtil.GetUserDiamond();
            Api.Refresh(delegate(Hashtable result)
            {
                if (result != null && result["inventory"] != null)
                {
                    //result.Remove("escortAndRob");
                    Hashtable ht = Johny.HashtablePool.Claim();
                    ht["inventory"] = result["inventory"];
                    DataLookupsCache.Instance.CacheData(ht);
                    Johny.HashtablePool.Release(ht);
                }

                FetchDataHandler(result);
                if (CurHC - BalanceResourceUtil.GetUserDiamond() > 0)
                    FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc,
                        BalanceResourceUtil.GetUserDiamond() - CurHC, "军团护送刷新");
            });
        }

        // public void RefreshTransferList()
        // {
        //     long uid = AllianceUtil.GetLocalUid();
        //     Api.RefreshTransferList(Account.AllianceId, uid, FetchDataHandler);
        // }

        public void GetRobInfo()
        {
            long uid = AllianceUtil.GetLocalUid();
            Api.GetRobInfo(Account.AllianceId, uid, FetchDataHandler);
        }

        // public void RefreshRobList()
        // {
        //     long uid = AllianceUtil.GetLocalUid();
        //     Api.RefreshRobList(Account.AllianceId, uid, FetchDataHandler);
        // }

        public void Accept(string id, System.Action<bool> callback)
        {
            Api.Accept(Account.AllianceId, id, delegate(Hashtable hash)
            {
                callback(hash != null);
                FetchDataHandler(hash);
            });
        }

        //start transfer without help
        public void Start(string id, System.Action<bool> callBack)
        {
            Api.Start(Account.AllianceId, id, delegate(Hashtable result)
            {
                if (result == null)
                {
                    callBack(false);
                }
                else
                {
                    callBack(true);
                    FetchDataHandler(result);
                }
            });
        }

        //start transfer with help
        // public void Start(long uid, long id, System.Action<bool> callBack)
        // {
        //     Api.Start(Account.AllianceId, uid, id, delegate (Hashtable result)
        //     {
        //         if (result == null)
        //         {
        //             callBack(false);
        //         }
        //         else
        //         {
        //             FetchDataHandler(result);
        //             callBack(true);
        //         }
        //     });
        // }

        public static void RecordTransferPointFromILRWithCallback(int index)
        {
            if (Instance != null)
            {
                Instance.RecordTransferPoint(index, null);
            }
        }


        public void RecordTransferPoint(int nextPoint, System.Action<bool> callBack = null)
        {
            TransferDartInfo.NextTransferPoint = nextPoint;
            Api.RecordTransferPoint(nextPoint, delegate(Hashtable result) { });
        }

        public void Finish(string id, System.Action<bool> callback)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("ID_ESCORT_DART_TIMEOUT"))
                {
                    DartResultController.sResultType = eDartResultType.TransferFailByTimeout;
                    GlobalMenuManager.Instance.Open("LTShowDartResultHud");
                    return true;
                }

                return false;
            };
            Api.Finish(Account.AllianceId, id, delegate(Hashtable result)
            {
                FetchDataHandler(result); //改为自行处理结果
                if (callback != null) callback(result != null);
            });
        }

        public void Complete(string id, System.Action<Hashtable> callback)
        {
            Api.Complete(Account.AllianceId, id, delegate(Hashtable result)
            {
                callback(result);
                FetchDataHandler(result);
                DataLookupsCache.Instance.CacheData(result);
            });
        }

        public void Rob(long uid, string id, System.Action<Hashtable> callback)
        {
            Api.Rob(Account.AllianceId, uid, id, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                callback(result);

                if (result != null && result["playstate"] != null)
                {
                    result.Remove("escortAndRob");
                    DataLookupsCache.Instance.CacheData(result);
                }
            });
        }

        //public static void FightFromILR(long uid, string id)
        //{
        //    if (Instance != null)
        //    {
        //        Instance.Fight(uid, id);
        //    }
        //}

        public void Fight(long uid, string id)
        {
            Api.Fight(uid, id, delegate(Hashtable result)
            {
                if (result != null)
                {
                    FetchDataHandler(result);
                    if (result != null && result["playstate"] != null)
                    {
                        result.Remove("escortAndRob");
                        DataLookupsCache.Instance.CacheData(result);
                    }
                }
            });
        }

        // public static void TakeFightFromILR(long uid, string id)
        // {
        //     if (Instance != null)
        //     {
        //         Instance.TakeFight(uid, id);
        //     }
        // }

        public void TakeFight(long uid, string id)
        {
            Api.TakeFight(uid, id, delegate(Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
            });
        }

        public void RobFight(long uid, string id)
        {
            Api.RobFight(uid, id, delegate(Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
            });
        }

        // public static void DenyFightFromILR(long uid)
        // {
        //     if (Instance != null)
        //     {
        //         Instance.DenyFight(uid);
        //     }
        // }

        public void DenyFight(long uid)
        {
            Api.DenyFight(uid, delegate(Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
            });
        }

        public void AttackRedName(long targetUid)
        {
            Api.AttackRedName(Account.AllianceId, targetUid, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                if (result != null && result["playstate"] != null)
                {
                    result.Remove("escortAndRob");
                    DataLookupsCache.Instance.CacheData(result);
                }
            });
        }

        public void GetApplyHelpInfo()
        {
            Api.GetApplyHelpInfo(Account.AllianceId, FetchDataHandler);
        }

        public void ApplyHelp(long uid, System.Action<bool> callBack)
        {
            Api.ApplyHelp(Account.AllianceId, uid, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        // public void CancelHelp(long uid, string inviteId, System.Action callBack)
        // {
        //     Api.CancelHelp(Account.AllianceId, uid, inviteId, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callBack();
        //     });
        // }

        public void GetHelpApplyInfo()
        {
            Api.GetHelpApplyInfo(Account.AllianceId, FetchDataHandler);
        }

        public void GetHelpApplyInfo(System.Action<Hashtable> callBack)
        {
            Api.GetHelpApplyInfo(Account.AllianceId, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                callBack(result);
            });
        }

        public void Agree(long uid, string inviteId, System.Action<bool> callBack)
        {
            Api.Agree(Account.AllianceId, inviteId, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        public void Reject(long uid, string inviteId, System.Action callBack)
        {
            Api.Reject(Account.AllianceId, inviteId, delegate(Hashtable result)
            {
                FetchDataHandler(result);
                callBack();
            });
        }

        public void RejectAll(System.Action<bool> callBack)
        {
            Api.RejectAll(delegate(Hashtable result)
            {
                FetchDataHandler(result);
                callBack(result != null);
            });
        }

        #endregion

        #region allianceCopy

        // public void GetCopyInfo(System.Action<bool> callback)
        // {
        //     Api.GetCopyInfo(Account.AllianceId, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         if (callback != null)
        //             callback(result != null);
        //     });
        // }

        // public void Unlock(string name, System.Action<bool> callback)
        // {
        //     Api.Unlock(Account.AllianceId, name, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         if (callback != null)
        //             callback(result != null);
        //     });
        // }

        // public void Reset(int id)
        // {
        //     Api.Reset(id, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //     });
        // }

        // public void GetRankInfo(int id, System.Action<bool> callback)
        // {
        //     Api.GetRankInfo(id, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callback(result != null);
        //     });
        // }

        // public void GetInventoryInfo(System.Action<bool> callback)
        // {
        //     Api.GetInventoryInfo(delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         callback(result != null);
        //     });
        // }

        // public void Lottery(string type, string data, System.Action<Hashtable> callback)
        // {
        //     Api.Lottery(type, data, delegate (Hashtable result)
        //     {
        //         FetchDataHandler(result);
        //         if (result["inventory"] != null)
        //         {
        //             Hashtable ht = Johny.HashtablePool.Claim();
        //             ht["inventory"] = result["inventory"];
        //             DataLookupsCache.Instance.CacheData(ht);
        //             Johny.HashtablePool.Release(ht);
        //         }
        //         if (result["buddyinventory"] != null)
        //         {
        //             Hashtable ht = Johny.HashtablePool.Claim();
        //             ht["buddyinventory"] = result["buddyinventory"];
        //             DataLookupsCache.Instance.CacheData(ht);
        //             Johny.HashtablePool.Release(ht);
        //         }
        //         callback(result);
        //     });
        // }

        #endregion

//#if DEBUG

//        public void DebugSetBalance(int balance)
//        {
//            Api.DebugSetBalance(Account.AllianceId, balance, FetchDataHandler);
//        }

//        public void DebugSetLiveness(int liveness)
//        {
//            Api.DebugSetLiveness(Account.AllianceId, liveness, FetchDataHandler);
//        }

//#endif

        static public string FormatOfflineDuration(double utcTimestamp)
        {
            if (utcTimestamp == 0)
            {
                return EB.Localizer.GetString("ID_SPARX_STATUS_ONLINE");
            }

            return EB.Localizer.FormatPassedDuration(utcTimestamp);
        }

        public int GetMercenaryHeroId()
        {
            int heroID;
            DataLookupsCache.Instance.SearchDataByID("mercenary.info.alli.hero_id", out heroID);
            return heroID;
        }

        public int GetMercenaryTime()
        {
            int times;
            DataLookupsCache.Instance.SearchDataByID("alliance.todayDonateTimes.today_hire_times", out times);
            return times;
            // return CurDonateInfo.today_hire_times;
        }
        
        public bool GetIsReward()
        {
            bool reward=false;
            DataLookupsCache.Instance.SearchDataByID("alliance.todayDonateTimes.get_hire_reward", out reward);
            return reward;
            // return CurDonateInfo.get_hire_reward;
        }

        public int GetMercenaryMaxTime()
        {
            int time=(int)Hotfix_LT.Data.NewGameConfigTemplateManager.Instance.GetGameConfigValue("AllianceHireReward");
            return time;
        }

        List<LTShowItemData> showItemsList = new List<LTShowItemData>();

        public List<LTShowItemData> GetMercenaryReward()
        {
            if (showItemsList.Count > 0)
            {
                return showItemsList;
            }
            else
            {
                ArrayList aList =
                    EB.JSON.Parse(NewGameConfigTemplateManager.Instance.GetGameConfigStrValue("AllianceHireReward")) as
                        ArrayList;
                if (aList == null)
                {
                    return showItemsList;
                }

                for (int i = 0; i < aList.Count; i++)
                {
                    string id = EB.Dot.String("data", aList[i], string.Empty);
                    int count = EB.Dot.Integer("quantity", aList[i], 0);
                    string type = EB.Dot.String("type", aList[i], string.Empty);
                    if (!string.IsNullOrEmpty(id))
                    {
                        LTShowItemData showItemData = new LTShowItemData(id, count, type);
                        showItemsList.Add(showItemData);
                    }
                }

                return showItemsList;
            }
        }

        public void ReqMercenaryReward(Action<bool> callback)
        {
            Api.errorProcessFun = (EB.Sparx.Response response) =>
            {
                if (response.error != null)
                {
                    string strObjects = (string)response.error;
                    string[] strObject = strObjects.Split(",".ToCharArray(), 2);
                    switch (strObject[0])
                    {
                        case "not reach":
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_MERCENARY_TIME_NOTENOUGH"));
                            LegionLogic.GetInstance().OnSendGetCurDonateInfo((ha) =>
                            {
                                DataLookupsCache.Instance.CacheData(ha);
                                callback(false);
                            });
                            return true;
                        }
                    }
                }
                return false;
            };
            Api.ReqMercenaryReward(delegate(Hashtable result)
            {
                DataLookupsCache.Instance.CacheData(result);
                callback(result != null);
            });
        }

        public void SetAllianceMarcenary(int heroId,int br, Action<bool> callback)
        {
            Api.SetAllianceMarcenary(heroId,br, delegate(Hashtable result)
            {
                DataLookupsCache.Instance.CacheData(result);
                callback(result != null);
                Messenger.Raise(EventName.LegionMercenaryUpdateUI);
            });
        }

        public List<LTPartnerData> hireDatas;

        public void GetCacheAllianceMercenaries(Action<List<LTPartnerData>> callback)
        {
            if (hireDatas == null)
            {
                GetAllianceMercenaries((ha) => { callback(ha); });
            }
            else
            {
                callback(hireDatas);
            }
        }
        
        public void GetAllianceMercenaries(Action<List<LTPartnerData>> callback)
        {
            Api.GetAllianceMercenaries(delegate(Hashtable result)
            {
                if (EB.Dot.Object("mercenary.info.used_uids", result, null) != null)
                {
                    //这部分数据需要替换，不megra
                    DataLookupsCache.Instance.CacheData("mercenary.info.used_uids", null);
                }
                DataLookupsCache.Instance.CacheData(result);
                
                string dataPath = string.Format("mercenary.mercenaries");
                ArrayList InfoData = EB.Dot.Array(dataPath, result, null);
                hireDatas = new List<LTPartnerData>();
                if (InfoData != null)
                {
                    foreach (IDictionary temp in InfoData)
                    {
                        LTPartnerData data = ParseMercenaryData(temp);
                        if (data != null) hireDatas.Add(data);
                    }
                }
                hireDatas.Sort((x, y) => y.br.CompareTo(x.br));
                callback(hireDatas);
            });
        }

        private LTPartnerData ParseMercenaryData(IDictionary temp)
        {
            string staId = EB.Dot.String("tpl_id", temp, string.Empty);
            LTPartnerData data = LTFormationDataManager.Instance.GetHeroBattleDataByStatId(int.Parse(staId));
            data.IsHire = true; //是否为雇佣
            data.uid = EB.Dot.Long("uid", temp, 0);
            if (data.uid == LoginManager.Instance.LocalUserId.Value) return null;
            data.HireHeroId = EB.Dot.Integer("hero_id", temp, 0);
            data.HireLevel = EB.Dot.Integer("level", temp, 0);
            data.HireUpGradeId = EB.Dot.Integer("upgrade", temp, 0);
            data.HireStar = EB.Dot.Integer("star", temp, 0);
            data.HireAwakeLevel = EB.Dot.Integer("awaken", temp, 0);
            data.br = EB.Dot.Integer("br", temp, 0);
            return data;
        }
        

        public LTPartnerData GetAlliancePartnerByHeroId(int HeroId)
        {
            if (hireDatas != null)
            {
                for (int i = 0; i < hireDatas.Count; i++)
                {
                    if (hireDatas[i].HeroId==HeroId)
                    {
                        return hireDatas[i];
                    }
                } 
            }

            IDictionary dict;
            DataLookupsCache.Instance.SearchDataByID("mercenary.info.use_info", out dict);
            LTPartnerData data= ParseMercenaryData(dict);
            if (data!=null && data.HeroId ==HeroId)
            {
                return data;
            }
            return null;
        }

       
    }

}