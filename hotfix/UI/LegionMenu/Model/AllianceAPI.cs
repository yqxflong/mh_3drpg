using System;
using System.Collections;


namespace Hotfix_LT.UI
{
    //20200320  liuhouchao 测试修改
    public class AllianceAPI : EB.Sparx.SparxAPI
    {
        public AllianceAPI()
        {
            endPoint = EB.Sparx.Hub.Instance.ApiEndPoint;
        }

        private void DefaultDataHandler(Hashtable alliance)
        {
            EB.Debug.Log("AllianceAPI.DefaultDataHandler: call default data handler");
        }

        public System.Func<EB.Sparx.Response, bool> errorProcessFun = null;
        private void ProcessAllianceResult(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);

            if (ExceptionFun != null && response.error != null) //处理Lostemp的异常
            {
                string error = response.error.ToString();
                if (ExceptionFun(error))
                {
                    ExceptionFun = null;
                    return;
                }
            }

            ExceptionFun = null;
            
            if (response.error != null)
            {
                EB.Debug.LogError(response.error);
            }

            if (errorProcessFun != null)
            {
                if (!errorProcessFun(response))
                {
                    if (ProcessResponse(response))
                    {
                        dataHandler(response.hashtable);
                    }
                    else
                    {
                        dataHandler(null);
                    }
                }
                else
                {
                    if (response.sucessful)
                    {
                        dataHandler(response.hashtable);
                    }
                }
                errorProcessFun = null;
            }
            else
            {
                if (ProcessResponse(response))
                {
                    dataHandler(response.hashtable);
                }
                else
                {
                    dataHandler(null);
                }
            }
        }

        private void ProcessAllianceResultEx(EB.Sparx.Response response, System.Action<Hashtable> dataHandler)
        {
            dataHandler = dataHandler ?? new System.Action<Hashtable>(DefaultDataHandler);
            if (ProcessResponse(response))
            {
                dataHandler(response.hashtable);
            }
            else
            {
                dataHandler(null);
            }
        }

        public override bool ProcessResponse(EB.Sparx.Response response)
        {
            if (!response.sucessful && response.fatal)
            {
                EB.Debug.LogError("SparxAPI.ProcessResponse: error {0} occur when request {1}", response.error,
                    response.request.uri);
                ProcessError(response, CheckError(response.error.ToString()));
                return false;
            }

            if (!response.sucessful)
            {
                string errStr = response.error.ToString();
                if (errStr != null && errStr.Equals("ID_FIELD_HAVE_FULL"))
                {
                    GlobalMenuManager.Instance.RemoveCache("AllianceHudUI");
                    GlobalMenuManager.Instance.RemoveCache("AllianceBattleResultUI");
                    GlobalMenuManager.Instance.CloseMenu("AllianceBattleResultUI");
                    AlliancesManager.Instance.BattleInfo.IsFieldFull = true;
                }
                else if (errStr != null && errStr.Equals("ID_FIELD_CONDITION_NOT_MEET"))
                {
                    MessageTemplateManager.ShowMessage(902187, null, delegate(int result)
                    {
                        if (result == 0)
                        {
                            GlobalMenuManager.Instance.RemoveCache("AllianceHudUI");
                            GlobalMenuManager.Instance.RemoveCache("AllianceBattleResultUI");
                            GlobalMenuManager.Instance.CloseMenu("AllianceBattleResultUI");
                        }
                    });
                    return false;
                }
                else if (errStr != null &&
                         (errStr.Equals("ID_ALLIANCE_NOT_IN_TRANSFER") || errStr.Equals("ID_ALLIANCE_ROBED")))
                {
                    AlliancesManager.Instance.GetRobInfo();
                }

                EB.Sparx.eResponseCode errCode = CheckError(response.error.ToString());
                if (errCode != EB.Sparx.eResponseCode.Success && !ProcessError(response, errCode))
                {
                    EB.Debug.LogError("SparxAPI.ProcessResponse: request {0} failed, {1}", response.request.uri,
                        response.error);
                    return false;
                }
            }

            return ProcessResult(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="dataHandler"></param>
        /// <param name="exceptionFun">异常处理 返回ture能截断错误不让往后处理</param>
        /// <returns></returns>
        private int BlockService(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            LoadingSpinner.Show();

            return endPoint.Service(request, delegate(EB.Sparx.Response response)
            {
                LoadingSpinner.Hide();

                ProcessAllianceResult(response, dataHandler);
            });
        }

        public System.Func<string, bool> ExceptionFun;

        private int Service(EB.Sparx.Request request, System.Action<Hashtable> dataHandler)
        {
            return endPoint.Service(request,
                delegate(EB.Sparx.Response response) { ProcessAllianceResult(response, dataHandler); });
        }

        /// <summary>
        /// 获取公会列表
        /// </summary>
        /// <param name="dataHandler"></param>
        public void GetAllianceList(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/list");
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 搜索公会列表
        /// </summary>
        /// <param name="input"></param>
        /// <param name="dataHandler"></param>
        public void SearchAllianceList(string input, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/list");
            request.AddData("search", input);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 创建军团
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iconID"></param>
        /// <param name="dataHandler"></param>
        public void CreateAlliance(string name, int iconID, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/create");
            request.AddData("name", name);
            request.AddData("iconID", iconID);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 军团重命名及修改团徽
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="name"></param>
        /// <param name="iconID"></param>
        /// <param name="dataHandler"></param>
        public void LegionRename(int aid, string name, int iconID, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/rename");
            request.AddData("aid", aid);
            request.AddData("name", name);
            request.AddData("iconID", iconID);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 申请入会
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="dataHandler"></param>
        public void ApplyRequest(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/apply");
            request.AddData("aid", aid);
            Service(request, dataHandler);
        }

        /// <summary>
        /// 取消申请入会
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="dataHandler"></param>
        public void CancelRequest(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/apply/cancel");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 获取入会申请列表
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="dataHandler"></param>
        public void GetAllianceApplyList(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/apply/list");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 会长同意入会请求
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void AcceptAllianceApplyRequest(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/apply/accept");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            Service(request, dataHandler);
        }

        /// <summary>
        /// 会长拒绝入会请求
        /// </summary>
        /// <param name="aid">军团id</param>
        /// <param name="uid">用户id</param>
        /// <param name="isTotal">是否移除全部</param>
        /// <param name="dataHandler"></param>
        public void RejectAllianceApplyRequest(int aid, long uid, bool isTotal, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/apply/reject");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("isTotal", isTotal);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 设置军团入会申请限制
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="level"></param>
        /// <param name="review"></param>
        /// <param name="dataHandler"></param>
        public void SetLegionLimitRequest(int aid, int level, bool review, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/setJoinCondition");
            request.AddData("aid", aid);
            request.AddData("level", level);
            request.AddData("review", review);
            BlockService(request, dataHandler);
        }

        public void LeaveAlliance(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/leave");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void GetAlliance(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/account");
            BlockService(request, dataHandler);
        }

        public void GetAllianceDetail(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/detail");
            request.AddData("aid", aid);
            Service(request, dataHandler);
        }

        public void GetAllianceMemberList(int aid, System.Action<Hashtable> dataHandler, bool isLogin = false)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/detail/member/list");
            request.AddData("aid", aid);
            if (isLogin) Service(request, dataHandler);
            else BlockService(request, dataHandler);
        }

        /// <summary>
        /// 快速加入军团
        /// </summary>
        public void AutoJoinAlliance(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/autoJoin");
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 军团获取事件信息
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="lastTime"></param>
        /// <param name="dataHandler"></param>
        public void AllianceGetMessages(int aid, double lastTime, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getAllianceLogs");
            request.AddData("aid", aid);
            request.AddData("lastTime", lastTime);
            BlockService(request, dataHandler);
        }

        public void AllianceMail(int aid, string title, string body, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/sendMail");
            request.AddData("aid", aid);
            request.AddData("title", title);
            request.AddData("body", body);
            BlockService(request, dataHandler);
        }

        public void AllianceDonate(int aid, int gold, int hc, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/detail/donate");
            request.AddData("aid", aid);
            request.AddData("gold", gold);
            request.AddData("hc", hc);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// LSTP 军团使用的新捐献方法
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="type"></param>
        /// <param name="dataHandler"></param>
        public void AllianceDonate(int aid, string type, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/detail/donate");
            request.AddData("aid", aid);
            request.AddData("type", type);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 军团长送给月卡
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void AllianceGiveMonthCard(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/giveMonthCard");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 军团长踢出成员
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void AllianceAdminRemoveMember(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/member/remove");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 军团长提升成员职级
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="targetRole"></param>
        /// <param name="dataHandler"></param>
        public void AllianceOwnerPromote(int aid, long uid, string targetRole, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/member/appoint");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("rank", targetRole);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 军团长降低成员职级
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="targetRole"></param>
        /// <param name="dataHandler"></param>
        public void AllianceOwnerDemote(int aid, long uid, string targetRole, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/member/degrade");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("rank", targetRole);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 任职新的军团长
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void AllianceOwnerTransfer(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/member/transfer");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void AdminLevelup(int aid, int from, int to, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/levelup");
            request.AddData("aid", aid);
            request.AddData("from", from);
            request.AddData("to", to);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 管理层更改军团名
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="name"></param>
        /// <param name="dataHandler"></param>
        public void AdminRename(int aid, string name, System.Action<Hashtable> dataHandler)
        {
            //EB.Sparx.Request request = endPoint.Post("/alliances/rename");
            //request.AddData("aid", aid);
            //request.AddData("name", name);
            //BlockService(request, dataHandler);
        }

        public void AdminNotice(int aid, string notice, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/notice");
            request.AddData("aid", aid);
            request.AddData("notice", notice);
            BlockService(request, dataHandler);
        }

        public void AdminTechLevelUp(int aid, int fromLevel, int skillId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/learnAllianceSkill");
            request.AddData("aid", aid);
            request.AddData("fromLevel", fromLevel);
            request.AddData("skillId", skillId);
            BlockService(request, dataHandler);
        }

        public void RecieveTechChest(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getTechnologyLevelChest");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void GetSkillList(long uid, int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getSkillList");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            Service(request, dataHandler);
        }

        public void LearnSkill(long uid, int aid, int fromLevel, string skillName, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/learnSkill");
            request.AddData("uid", uid);
            request.AddData("aid", aid);
            request.AddData("fromLevel", fromLevel);
            request.AddData("skillName", skillName);
            BlockService(request, dataHandler);
        }

        //五族大战
        public void BattleApply(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/apply");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void GetBattleBaseInfo(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/getBaseInfo");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void EnterBattleField(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/enterBattleField");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void LeaveBattleField(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/leaveBattleField");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void ChangePosition(int aid, Hashtable from, Hashtable to, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/changePosition");
            Hashtable data = Johny.HashtablePool.Claim();
            data.Add("aid", aid);
            data.Add("from", from);
            data.Add("to", to);

            request.AddData(data);
            BlockService(request, dataHandler);
        }

        public void GetForwardSelectionInfo(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/getForwardSelectionInfo");
            Hashtable data = Johny.HashtablePool.Claim();
            data.Add("aid", aid);
            request.AddData(data);
            BlockService(request, dataHandler);
        }

        public void GetFinalsInfo(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliancesBattle/getFinalsInfo");
            Hashtable data = Johny.HashtablePool.Claim();
            data.Add("aid", aid);
            request.AddData(data);
            BlockService(request, dataHandler);
        }

        public void GetAwardInfo(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/battle/getAwardInfo");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void AssignBattleAward(int aid, long uid, string boxName, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/battle/assignBattleAward");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("boxName", boxName);
            BlockService(request, dataHandler);
        }

        public void GiveMonthCard(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/giveMonthCard");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void ReceiveMonthCard(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/receiveMonthCard");
            request.AddData("aid", aid);
            //request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        // 获取玩家阵容信息
        public void GetPlayerFormationInfo(long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getPlayerTeam");
            request.AddData("userId", uid);
            BlockService(request, dataHandler);
        }

        // 获取当前捐献信息
        public void GetCurDonateInfo(System.Action<Hashtable> dataHandler, bool isLogin)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getDonateTypeTimes");
            if (isLogin) Service(request, dataHandler);
            else BlockService(request, dataHandler);
        }

        /// <summary>
        /// 购买军团勋章
        /// </summary>
        /// <param name="dataHandler"></param>
        public void BuyLegionMedal(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/buyUserMedal");
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 赠送勋章
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void SendMedal(long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/sendMedal");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        /// <summary>
        /// 解除勋章
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void UnlinkMedalPair(long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/unlinkMedalPair");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }


        public void GetTransferInfo(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/getTransferInfo");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            Service(request, dataHandler);
        }

        public void Refresh(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/refresh");
            BlockService(request, dataHandler);
        }

        public void RefreshTransferList(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/refreshTransferList");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void RefreshTransferTaskList(string dartId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/refreshTransferTaskList");
            request.AddData("id", dartId);
            BlockService(request, dataHandler);
        }

        public void GetRobInfo(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/getRobInfo");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void RefreshRobList(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/refreshRobList");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void Accept(int aid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/accept");
            request.AddData("aid", aid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        //start transfer without help
        public void Start(int aid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/start");
            request.AddData("aid", aid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        //start transfer with help
        public void Start(int aid, long uid, long id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/start");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void RecordTransferPoint(int nextPoint, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/recordTransferPoint");
            request.AddData("nextPoint", nextPoint);
            Service(request, dataHandler);
        }

        public void Finish(int aid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/finish");
            request.AddData("aid", aid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void Complete(int aid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/complete");
            request.AddData("aid", aid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void Rob(int aid, long uid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/rob");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void Fight(long uid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/fight");
            request.AddData("uid", uid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void TakeFight(long uid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/takefight");
            request.AddData("uid", uid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void DenyFight(long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/denyfight");
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void RobFight(long uid, string id, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/robfight");
            request.AddData("uid", uid);
            request.AddData("id", id);
            BlockService(request, dataHandler);
        }

        public void AttackRedName(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/attackRedName");
            request.AddData("uid", uid);
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void GetApplyHelpInfo(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/getApplyHelpInfo");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void ApplyHelp(int aid, long uid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/applyHelp");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            BlockService(request, dataHandler);
        }

        public void CancelHelp(int aid, long uid, string inviteId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/cancelHelp");
            request.AddData("aid", aid);
            request.AddData("uid", uid);
            request.AddData("inviteId", inviteId);
            BlockService(request, dataHandler);
        }

        public void GetHelpApplyInfo(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/getHelpApplyInfo");
            request.AddData("aid", aid);
            Service(request, dataHandler);
        }

        public void Agree(int aid, string inviteId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/agree");
            request.AddData("aid", aid);
            request.AddData("requesterId", AllianceUtil.GetLocalUid());
            request.AddData("inviteId", inviteId);
            BlockService(request, dataHandler);
        }

        public void Reject(int aid, string inviteId, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/reject");
            request.AddData("aid", aid);
            request.AddData("inviteId", inviteId);
            BlockService(request, dataHandler);
        }

        public void RejectAll(System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/escortAndRob/rejectAll");
            BlockService(request, dataHandler);
        }

        public void GetAllianceFightRank(int aid, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getAllianceFightRank");
            request.AddData("aid", aid);
            BlockService(request, dataHandler);
        }

        public void GetAllianceDonateChest(int aid, int index, System.Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getDonateChest");
            request.AddData("aid", aid);
            request.AddData("chestId", index);
            BlockService(request, dataHandler);
        }

        public void ReqMercenaryReward(Action<Hashtable> dataHandler)
        {
            EB.Sparx.Request request = endPoint.Post("/alliances/getHireReward");
            request.AddData("aid", LegionModel.GetInstance().legionData.legionID);
            BlockService(request, dataHandler);
        }


        public void SetAllianceMarcenary(int heroId, int br, Action<Hashtable> callback)
        {
            EB.Sparx.Request request = endPoint.Post("/mercenary/setAllianceMercenary");
            request.AddData("heroId", heroId);
            request.AddData("br", br);
            BlockService(request, callback);
        }

        public void GetAllianceMercenaries(Action<Hashtable> callback)
        {
            EB.Sparx.Request request = endPoint.Post("/mercenary/getAllianceMercenaries");
            BlockService(request, callback);
        }
    }
}