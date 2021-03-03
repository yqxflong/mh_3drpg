using EB.Sparx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hotfix_LT.UI
{
    /// <summary>
    /// 军团总逻辑类
    /// </summary>
    public class LegionLogic : ManagerUnit
    {
        private AllianceAPI Api;
        private const float AutoSearchListWaitTime = 10;

        private static LegionLogic _instance;
        public static LegionLogic GetInstance()
        {
            if (_instance == null)
            {
                _instance = LTHotfixManager.GetManager<LegionLogic>();
            }
            return _instance;
        }

        public AllianceAPI GetAPI()
        {
            return Api;
        }
        public override void Async(string message, object payload)
        {
            switch (message)
            {
                //加入帮派
                case "join":
                case "leave": //（除操作者之外所有人）成员离开
                case "kick": //（除操作者之外所有人）成员被踢
                    long uid = EB.Dot.Long("member.uid", payload, 0);
                    //重新获取一下帮派成员相信信息。
                    if (LegionModel.GetInstance().isJoinedLegion)
                    {
                        if (LoginManager.Instance.LocalUserId.Value == uid) //如果是自己
                        {
                            if (message.Equals("leave") || message.Equals("kick")) //被踢与离开 直接设置被踢出
                            {
                                OnRefreshPower();
                            }
                        }
                        else //非自己就拉取成员详细数据
                        {
                            Api.GetAllianceMemberList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler);
                        }
                    }
                    else
                    {
                        if (LoginManager.Instance.LocalUserId.Value == uid) //如果是自己 
                        {
                            if (message.Equals("join")) //如果是自己加入 但是自己又不在军团 那就要获取一次军团数据
                            {
                                if (Api.ExceptionFun == null)
                                {
                                    Api.GetAlliance(FetchDataHandler);
                                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.LegionSearchMessageCallBack, 2);
                                    OnRefreshPower();
                                }
                                else
                                {
                                    // 如果ExceptionFun不为空，则说明有其他地方在向服务器请求消息（极有可能是申请加入军团的消息），如果此消息以错误码的形式返回，
                                    //   则这里同步发的消息会被消息队列清空，让消息没有发出去，导致数据异常
                                    //   所以这里会延迟向服务器请求消息
                                    EB.Coroutines.Run(WaitToGetLegionInfo());
                                }
                            }
                        }
                    }
                    break;
                case "update"://军团
                              //legionlevel = legionlevel == 0 ? LegionModel.GetInstance().legionData.legionLevel: legionlevel;
                              ////if (LegionModel.GetInstance().legionData.legionLevel)
                              //TimerManager.instance.AddTimer(3, 1, OnRefreshPower);
                    break;
                case "promote":  //（除操作者之外所有人）升职（提升为精英或帮主）
                case "demote": //（除操作者之外所有人）降职（降为精英或普通成员）


                    break;
            }
        }

        public override void Initialize(Config config)
        {
            LegionModel.GetInstance();
            Api = new AllianceAPI();
            LegionEvent.OpenManagerMenu += OnOpenManagerMenu;
            LegionEvent.LegionShowUI += ShowUI;
            LegionEvent.SearchQuickJoinLegion += OnSearchQuickJoinLegion;
            LegionEvent.SearchLegion += OnSearchLegion;
            LegionEvent.SendCreateLegionMsg += OnSendCreateLegionMsg;
            LegionEvent.SendLegionMail += OnSendLegionMail;
            LegionEvent.SendSaveLimit += OnSendSaveLimit;
            LegionEvent.SendConsentRequestJoin += OnSendConsentRequestJoin;
            LegionEvent.SendRejectRequestJoin += OnSendRejectRequestJoin;
            LegionEvent.SendRejectTotalRequestJoin += OnSendRejectTotalRequestJoin;
            LegionEvent.SendApplyJoinLegion += OnSendApplyJoinLegion;
            LegionEvent.SendCancelApplyJoinLegion += OnSendCancelApplyJoinLegion;
            LegionEvent.SendSaveLegionNotice += OnSendSaveLegionNotice;
            LegionEvent.SendLeaveLegion += OnSendLeaveLegion;
            LegionEvent.SendMemberPromote += OnSendMemberPromote;
            LegionEvent.SendMemberDemote += OnSendMemberDemote;
            LegionEvent.SendMemberGiveOwner += OnSendMemberGiveOwner;
            LegionEvent.SendMemberKickOut += OnSendMemberKickOut;
            LegionEvent.SendGoldDonate += OnSendGoldDonate;
            LegionEvent.SendDiamandDonate += OnSendDiamandDonate;
            LegionEvent.SendLuxuryDonate += OnSendLuxuryDonate;
            LegionEvent.SendGetLegionMessages += OnSendGetLegionMessages;
            LegionEvent.SendMemberAddFriend += OnSendMemberAddFriend;
            LegionEvent.SendGiveMonthCard += OnSendGiveMonthCard;
            LegionEvent.OpenMemberTalk += OnOpenMemberTalk;
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, GetInstance().DataRefresh);
        }

        private void DataRefresh()
        {
            if (LegionLogic.GetInstance().IsHaveLegion() &&
            (LegionLogic.GetInstance().IsHaveApplyMember() ||
            LegionLogic.GetInstance().HaveDonateRP() ||
            LegionLogic.GetInstance().IsHaveTechRP() ||
            LegionLogic.GetInstance().IsOpenActivity()))
            {
            }
        }

        private void OnRefreshPower(int sep = 0)
        {
            LTPartnerDataManager.Instance.OnDestineTypePowerChanged(Data.eRoleAttr.None, (prm) =>
            {
                LTFormationDataManager.OnRefreshMainTeamPower(prm);
            }, false);
        }

        public override void Dispose()
        {
            LegionEvent.OpenManagerMenu -= OnOpenManagerMenu;
            LegionEvent.LegionShowUI -= ShowUI;
            LegionEvent.SearchQuickJoinLegion -= OnSearchQuickJoinLegion;
            LegionEvent.SearchLegion -= OnSearchLegion;
            LegionEvent.SendCreateLegionMsg -= OnSendCreateLegionMsg;
            LegionEvent.SendLegionMail -= OnSendLegionMail;
            LegionEvent.SendSaveLimit -= OnSendSaveLimit;
            LegionEvent.SendConsentRequestJoin -= OnSendConsentRequestJoin;
            LegionEvent.SendRejectRequestJoin -= OnSendRejectRequestJoin;
            LegionEvent.SendRejectTotalRequestJoin -= OnSendRejectTotalRequestJoin;
            LegionEvent.SendApplyJoinLegion -= OnSendApplyJoinLegion;
            LegionEvent.SendCancelApplyJoinLegion -= OnSendCancelApplyJoinLegion;
            LegionEvent.SendSaveLegionNotice -= OnSendSaveLegionNotice;
            LegionEvent.SendLeaveLegion -= OnSendLeaveLegion;
            LegionEvent.SendMemberPromote -= OnSendMemberPromote;
            LegionEvent.SendMemberDemote -= OnSendMemberDemote;
            LegionEvent.SendMemberGiveOwner -= OnSendMemberGiveOwner;
            LegionEvent.SendMemberKickOut -= OnSendMemberKickOut;
            LegionEvent.SendGoldDonate -= OnSendGoldDonate;
            LegionEvent.SendDiamandDonate -= OnSendDiamandDonate;
            LegionEvent.SendLuxuryDonate -= OnSendLuxuryDonate;
            LegionEvent.SendGetLegionMessages -= OnSendGetLegionMessages;
            LegionEvent.SendMemberAddFriend -= OnSendMemberAddFriend;
            LegionEvent.SendGiveMonthCard -= OnSendGiveMonthCard;
            LegionEvent.OpenMemberTalk -= OnOpenMemberTalk;
        }

        public override void OnLoggedIn()
        {
            base.OnLoggedIn();

            Hashtable loginData = EB.Sparx.Hub.Instance.DataStore.LoginDataStore.LoginData;

            if (LegionModel.GetInstance().isJoinedLegion)
            {
                Api.GetAllianceMemberList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler, true);
                OnSendGetCurDonateInfo(delegate (Hashtable res) { DataLookupsCache.Instance.CacheData(res); });
            }

            object alliance = loginData["alliance"];
            if (alliance == null)
            {
                return;
            }
            loginData.Remove("alliance");
        }

        public override void Disconnect(bool isLogout)
        {
            //State = EB.Sparx.SubSystemState.Disconnected;
        }

        public override void Connect()
        {
            //State = EB.Sparx.SubSystemState.Connected;
        }

        public void ShowUI()
        {
            //判断是否有军团，有军团进入军团页面，无军团进入军团搜索页面
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10019);
            if (ft != null && !ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }
            if (LegionModel.GetInstance().isJoinedLegion)
            {
                GlobalMenuManager.Instance.Open("LTLegionMainMenu", LegionModel.GetInstance().legionData);
                Api.GetAllianceDetail(LegionModel.GetInstance().legionData.legionID, GetHandlerRequestMemberList);
            }
            else
            {
                GlobalMenuManager.Instance.Open("LTSearchJTMenu");
                Api.GetAllianceList(FetchDataHandler);
            }
        }

        public void ShowUI(ChoiceState state)
        {
            //判断是否有军团，有军团进入军团页面，无军团进入军团搜索页面
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10019);
            if (ft != null && !ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }
            if (LegionModel.GetInstance().isJoinedLegion)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("legionData", LegionModel.GetInstance().legionData);
                ht.Add("choiceState", state);
                GlobalMenuManager.Instance.Open("LTLegionMainMenu", ht);
                Api.GetAllianceDetail(LegionModel.GetInstance().legionData.legionID, GetHandlerRequestMemberList);
            }
            else
            {
                GlobalMenuManager.Instance.Open("LTSearchJTMenu");
                Api.GetAllianceList(FetchDataHandler);
            }
        }

        public void OpenLegionBossUI()
        {
            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10019);
            if (ft != null && !ft.IsConditionOK())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                return;
            }
            if (LegionModel.GetInstance().isJoinedLegion)
            {
                if (AllianceUtil.GetIsInTransferDart("")) return;
                InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                GlobalMenuManager.Instance.Open("LTLegionFBUI");
                FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.alliance_camp_topic,
                FusionTelemetry.GamePlayData.alliance_camp_event_id, FusionTelemetry.GamePlayData.alliance_camp_umengId, "open");
            }
            else
            {
                GlobalMenuManager.Instance.Open("LTSearchJTMenu");
                Api.GetAllianceList(FetchDataHandler);
            }

        }

        public void HideUI()
        {
            if (LegionEvent.CloseLegionHudUI != null)
            {
                LegionEvent.CloseLegionHudUI();
            }
        }

        void OnSearchQuickJoinLegion()
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("no alliance"))
                {
                    MessageTemplateManager.ShowMessage(902280);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_8599"));
                    return true;
                }

                if (error.Equals("aid is not defined"))
                {
                    MessageTemplateManager.ShowMessage(902274);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_8821"));
                    return true;
                }

                if (error.Equals("Already in Alliance") || error.Equals("alreadyInAlliance"))
                {
                    MessageTemplateManager.ShowMessage(902276);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_9084"));
                    return true;
                }
                return false;
            };

            //发送快速加入军团的消息 ，若成功 军团界面将关闭搜索弹出军团。
            Api.AutoJoinAlliance(FetchDataHandler);
            EB.Debug.Log("OnSearchQuickJoinLegion");
        }

        void OnSearchLegion(string str)
        {
            SearchLegionList(str);
        }

        //void OnSearchCreateLegion()
        //{
        //    GlobalMenuManager.Instance.Open("ExpeditionUI");
        //}

        void OnSendCreateLegionMsg(string createName, int iconID)
        {
            if (BalanceResourceUtil.GetUserDiamond() < AlliancesManager.Instance.Config.CreateCost)
            {
                //MessageTemplateManager.ShowMessage(901030, null, delegate (int r)
                //{
                //    if (r == 0)
                //    {
                //        InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                //        GlobalMenuManager.Instance.Open("LTResourceShopUI");
                //    }
                //});
                BalanceResourceUtil.HcLessMessage();
                return;
            }

            if (createName.Equals(""))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_10399"));
                return;
            }

            if (!EB.ProfanityFilter.Test(createName))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_10580"));
                return;
            }

            if (createName.Contains(" "))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_10756"));
                return;
            }
            //发送创建军团的消息，若成功创建将关闭搜索界面弹出军团界面。
            EB.Debug.Log("OnSendCreateLegionMsg{0}", createName);

            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("name_min_len"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_11096"));
                    return true;
                }
                if (error.Equals("ID_ERROR_NAME_EXIST"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_NAME_EXIST"));
                    return true;
                }
                return false;
            };
            Api.CreateAlliance(createName, iconID, FetchDataHandler_Creat);
        }

        void OnSendLegionMail(string title, string content)
        {
            //发送军团邮件 发送成功后要主动刷新剩余邮件
            EB.Debug.Log("OnSendLegionMail{0} {1}", title, content);
            EB.Debug.LogError("OnSendLegionMail");
            Api.AllianceMail(LegionModel.GetInstance().legionData.legionID, title, content, MergeDataHandler);
        }

        public void SendGetAlliance()
        {
            Api.GetAlliance(FetchDataHandler_ChangeName);
        }

        //上传友盟回调
        private void FetchDataHandler_Creat(Hashtable alliance)
        {
            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, -AlliancesManager.Instance.Config.CreateCost, "创建军团");
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
            }
        }
        private void FetchDataHandler_ChangeName(Hashtable alliance)
        {
            FusionTelemetry.CurrencyChangeData.PostEvent(FusionTelemetry.CurrencyChangeData.hc, -AlliancesManager.Instance.Config.RenameCost, "军团改名");
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
            }
        }

        private void FetchDataHandler(Hashtable alliance)
        {
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
            }
        }
        private void MergeDataHandler(Hashtable alliance)
        {
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, true);
            }
        }

        /// <summary>
        /// 获取到数据后发送获取军团成员列表
        /// </summary>
        /// <param name="alliance"></param>
        private void GetHandlerRequestMemberList(Hashtable alliance)
        {
            if (alliance != null)
            {
                GameDataSparxManager.Instance.ProcessIncomingData(alliance, false);
                Api.GetAllianceMemberList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler);
            }
        }

        public void SearchLegionList(string searchName)
        {
            Api.SearchAllianceList(searchName, FetchDataHandler);
        }

        void OnOpenManagerMenu()
        {
            GlobalMenuManager.Instance.Open("LTLegionManagerMenu", LegionModel.GetInstance().legionData);
            Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler);
        }

        void OnSendSaveLimit(int limitLevel, bool isApprove)
        {
            Api.SetLegionLimitRequest(LegionModel.GetInstance().legionData.legionID, limitLevel, isApprove, (Hashtable h) =>
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14032"));
                FetchDataHandler(h);
            });
        }

        void OnSendConsentRequestJoin(long uid)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("alliance join invite mot found") || error.Equals("alreadyInAlliance"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14405"));
                    //Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler); //重新拉取军团申请列表
                    return true;
                }

                if (error.Equals("full"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14742"));
                    return true;
                }

                if (error.Equals("NOT FOUND"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14949"));
                    //Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler); //重新拉取军团申请列表
                    return true;
                }

                return false;
            };
            Api.AcceptAllianceApplyRequest(LegionModel.GetInstance().legionData.legionID, uid, FetchDataHandler);
        }

        void OnSendRejectRequestJoin(long uid)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("alliance join invite mot found") || error.Equals("alreadyInAlliance"))
                {
                    //MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_15590"));
                    //Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler); //重新拉取军团申请列表
                    return true;
                }

                if (error.Equals("full"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14742"));
                    return true;
                }

                if (error.Equals("NOT FOUND"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14949"));
                    //Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler); //重新拉取军团申请列表
                    return true;
                }

                return false;
            };

            Api.RejectAllianceApplyRequest(LegionModel.GetInstance().legionData.legionID, uid, false, FetchDataHandler);
        }

        void OnSendRejectTotalRequestJoin()
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("alliance join invite mot found") || error.Equals("alreadyInAlliance"))
                {
                    //MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_16771"));
                    //Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler); //重新拉取军团申请列表
                    return true;
                }

                if (error.Equals("full"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14742"));
                    return true;
                }

                if (error.Equals("NOT FOUND"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_14949"));
                    //Api.GetAllianceApplyList(LegionModel.GetInstance().legionData.legionID, FetchDataHandler); //重新拉取军团申请列表
                    return true;
                }

                return false;
            };

            Api.RejectAllianceApplyRequest(LegionModel.GetInstance().legionData.legionID, 0, true, FetchDataHandler);
        }

        /// <summary>
        /// 在苹果上出现了2个按钮在一个位置 depth不同却被同时触发的情况
        /// </summary>
        private float _waitApplyOrCancelJoinCD;
        void OnSendApplyJoinLegion(int legionID)
        {
            if (Time.unscaledTime - _waitApplyOrCancelJoinCD < 0.5f)
            {
                return;
            }
            _waitApplyOrCancelJoinCD = Time.unscaledTime;
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("inJoinInterval"))
                {
                    MessageTemplateManager.ShowMessage(902274);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_18200"));
                    return true;
                }

                if (error.Equals("full"))
                {
                    MessageTemplateManager.ShowMessage(902275);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_18412"));
                    return true;
                }

                if (error.Equals("Already in Alliance") || error.Equals("alreadyInAlliance"))
                {
                    MessageTemplateManager.ShowMessage(902276);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_9084"));
                    return true;
                }

                if (error.Equals("need levelup"))
                {
                    MessageTemplateManager.ShowMessage(902277);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_18893"));
                    SearchLegionList(""); //重新刷新下军团
                    return true;
                }

                if (error.Equals("send Invite too much"))
                {
                    MessageTemplateManager.ShowMessage(902278);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_19167"));
                    return true;
                }

                if (error.Equals("NOT FOUND"))
                {
                    MessageTemplateManager.ShowMessage(902279);//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_19385"));
                    return true;
                }
                return false;
            };
            Api.ApplyRequest(legionID, FetchDataHandler);
        }

        /// <summary>
        /// 取消对某个军团的申请
        /// </summary>
        /// <param name="legionID"></param>
        void OnSendCancelApplyJoinLegion(int legionID)
        {
            if (Time.unscaledTime - _waitApplyOrCancelJoinCD < 0.5f)
            {
                return;
            }
            _waitApplyOrCancelJoinCD = Time.unscaledTime;

            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("not such invite"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_20055"));
                    return true;
                }
                return false;
            };

            Api.CancelRequest(legionID, FetchDataHandler);
        }

        void OnSendSaveLegionNotice(string notice)
        {
            if (string.IsNullOrEmpty(notice))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_20402"));
                return;
            }

            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("noChanges"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_20648"));
                    return true;
                }
                return false;
            };
            Api.AdminNotice(LegionModel.GetInstance().legionData.legionID, notice, (Hashtable h) =>
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_20928"));
                FetchDataHandler(h);
            });
        }
        /// <summary>
        /// 退出军团
        /// </summary>
        void OnSendLeaveLegion()
        {

            //Debug.LogError("AllianceInitListCount: " + DataClass.GM.DataCache.ConditionAlliance.AllianceWarRewardLength);
            // EB.Debug.LogError("AllianceInitListCount: {0}", Hotfix_LT.Data.AllianceTemplateManager.Instance.mWarConfigList.Count);
            if (LegionModel.GetInstance().legionData.userMemberData.dutyType == eAllianceMemberRole.Owner && LegionModel.GetInstance().legionData.currentPeopleNum > 1)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_21333"));
                return;

            }

            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_codefont_in_LegionLogic_21459"), (int result) =>
            {
                switch (result)
                {
                    case 0:
                        Api.ExceptionFun = (string error) =>
                        {
                            if (error.Equals("allianceWarService Exit"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_21834"));
                                return true;
                            }
                            return false;
                        };
                        Api.LeaveAlliance(LegionModel.GetInstance().legionData.legionID, (Hashtable h) =>
                        {
                            if (AlliancesManager.Instance.TotalMessage != null)
                            {
                                // 离开军团后清空军团事件
                                AlliancesManager.Instance.TotalMessage.CleanUp();
                                LegionLogic.GetInstance().OnSendGetCurDonateInfo((ha) =>
                                {
                                    DataLookupsCache.Instance.CacheData(ha);
                                });
                            }
                            FetchDataHandler(h);
                        });
                        break;
                    case 1:
                    case 2:
                        break;
                }
            });



        }


        void OnSendMemberPromote(long uid)
        {
            if (LegionModel.GetInstance().legionData != null && LegionModel.GetInstance().legionData.listMember != null)
            {
                List<LegionMemberData> listMember = LegionModel.GetInstance().legionData.listMember;
                for (int i = 0; i < listMember.Count; i++)
                {
                    if (listMember[i].uid == uid)
                    {
                        Api.ExceptionFun = (string error) =>
                        {
                            if (error.Equals("rankFull"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_23220"));
                                return true;
                            }

                            if (error.Equals("appoint failed"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_23495"));
                                return true;
                            }

                            if (error.Equals("allianceWarService Exit"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_23779"));
                                return true;
                            }
                            return false;
                        };
                        Api.AllianceOwnerPromote(LegionModel.GetInstance().legionData.legionID, uid, GetPromote(listMember[i].dutyType), MemberPostChangeCallBack);
                        break;
                    }
                }
            }
        }
        void OnSendMemberDemote(long uid)
        {
            if (LegionModel.GetInstance().legionData != null && LegionModel.GetInstance().legionData.listMember != null)
            {
                List<LegionMemberData> listMember = LegionModel.GetInstance().legionData.listMember;
                for (int i = 0; i < listMember.Count; i++)
                {
                    if (listMember[i].uid == uid)
                    {
                        Api.ExceptionFun = (string error) =>
                        {
                            if (error.Equals("degrade failed"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_24828"));
                                return true;
                            }

                            if (error.Equals("allianceWarService Exit"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_25112"));
                                return true;
                            }
                            return false;
                        };
                        Api.AllianceOwnerDemote(LegionModel.GetInstance().legionData.legionID, uid, GetDemote(listMember[i].dutyType), MemberPostChangeCallBack);
                        break;
                    }
                }
            }
        }

        /// <summary> 获取玩家阵容信息 </summary>
        public void OnSendGetPlayerFormationInfo(long uid, System.Action<Hashtable> dataHandler)
        {
            Api.GetPlayerFormationInfo(uid, dataHandler);
        }

        // 军团成员职位变动回调
        private void MemberPostChangeCallBack(Hashtable alliance)
        {
            MergeDataHandler(alliance);
            if (LegionEvent.MemberPostChangeCallBack != null)
            {
                LegionEvent.MemberPostChangeCallBack();
            }
            if (LegionEvent.MessageCallBack != null)
            {
                LegionEvent.MessageCallBack();
            }
        }

        // 军团信息提示回调
        private void LegionMessageCallBack(Hashtable alliance)
        {
            if (EB.Dot.Object("alliance.account.medal", alliance, null) != null)
            {
                //这部分数据需要替换，不megra
                DataLookupsCache.Instance.CacheData("alliance.account.medal.send", null);
            }

            MergeDataHandler(alliance);
            if (LegionEvent.MessageCallBack != null)
            {
                LegionEvent.MessageCallBack();
            }
        }
        /// <summary>
        /// 请求升级军团技能
        /// </summary>
        /// <param name="skillId"></param>
        public void LegionUplevelTechLevel(int aid, int fromLevel, int skillId)
        {
            Api.AdminTechLevelUp(aid, fromLevel, skillId, MergeDataHandler);
        }
        /// <summary>
        /// 请求领取科技宝箱
        /// </summary>
        /// <param name="skillId"></param>

        public void OnRequestTechChest(int aid, System.Action callback)
        {
            Api.RecieveTechChest(aid, delegate (Hashtable data)
            {
                MergeDataHandler(data);
                callback?.Invoke();
            });
        }
        /// <summary> 获取当前捐献的信息 </summary>
        public void OnSendGetCurDonateInfo(System.Action<Hashtable> dataHandler)
        {
            Api.GetCurDonateInfo(dataHandler, true);
        }

        /// <summary>
        /// 任命团长
        /// </summary>
        /// <param name="uid"></param>
        void OnSendMemberGiveOwner(long uid)
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LegionLogic_26671"), (int result) =>
            {
                switch (result)
                {
                    case 0:
                        Api.ExceptionFun = (string error) =>
                        {
                            if (error.Equals("allianceWarService Exit"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_27050"));
                                return true;
                            }
                            return false;
                        };
                        Api.AllianceOwnerTransfer(LegionModel.GetInstance().legionData.legionID, uid, LegionMessageCallBack);
                        break;
                    case 1:
                    case 2:
                        break;
                }
            });

        }



        /// <summary>
        /// 购买军团勋章
        /// </summary>
        /// <param name="dataHandler"></param>
        public void BuyLegionMedal(System.Action<bool> callback)
        {
            Api.BuyLegionMedal(delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        /// <summary>
        /// 赠送勋章
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void SendMedal(long uid, System.Action<bool> callback)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("hasSend"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_LEGION_ERR_HASSEND"));//eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_8599"));
                    return true;
                }
                return false;
            };

            Api.SendMedal(uid, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        /// <summary>
        /// 解除勋章
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="dataHandler"></param>
        public void UnlinkMedalPair(long uid, System.Action<bool> callback)
        {
            Api.UnlinkMedalPair(uid, delegate (Hashtable result)
            {
                if (result != null)
                {
                    DataLookupsCache.Instance.CacheData(result);
                }
                if (callback != null)
                {
                    callback(result != null);
                }
            });
        }

        private string GetPromote(eAllianceMemberRole role)
        {
            string strRole = "vice_owner";
            switch (role)
            {
                case eAllianceMemberRole.Member:
                    strRole = "admin";
                    break;
                case eAllianceMemberRole.Admin:
                    strRole = "vice_owner";
                    break;
            }
            return strRole;
        }

        private string GetDemote(eAllianceMemberRole role)
        {
            string strRole = "member";
            switch (role)
            {
                case eAllianceMemberRole.ExtraOwner:
                    strRole = "admin";
                    break;
                case eAllianceMemberRole.Admin:
                    strRole = "member";
                    break;
            }
            return strRole;
        }


        /// <summary>
        /// 踢出军团
        /// </summary>
        /// <param name="uid"></param>
        void OnSendMemberKickOut(long uid)
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_LegionLogic_28425"), (int result) =>
            {
                switch (result)
                {
                    case 0:
                        Api.ExceptionFun = (string error) =>
                        {
                            if (error.Equals("allianceWarService Exit"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_28800"));
                                return true;
                            }
                            return false;
                        };
                        Api.AllianceAdminRemoveMember(LegionModel.GetInstance().legionData.legionID, uid, LegionMessageCallBack);
                        break;
                    case 1:
                    case 2:
                        break;
                }
            });

        }

        void OnSendGoldDonate()
        {
            if (!IsDonate(0, AlliancesManager.Instance.Config.DonateGoldSpend)) return;
            Api.ExceptionFun = (string error) =>
            {
                return DonateError(error);
            };
            Api.AllianceDonate(LegionModel.GetInstance().legionData.legionID, "gold", DonateResultDo);
        }
        void OnSendDiamandDonate()
        {
            if (!IsDonate(1, AlliancesManager.Instance.Config.DonateHcSpend)) return;
            Api.ExceptionFun = (string error) =>
            {
                return DonateError(error);
            };
            Api.AllianceDonate(LegionModel.GetInstance().legionData.legionID, "hc", DonateResultDo);
        }

        void OnSendLuxuryDonate()
        {
            if (!IsDonate(2, AlliancesManager.Instance.Config.DonateLuxurySpend)) return;
            Api.ExceptionFun = (string error) =>
            {
                return DonateError(error);
            };
            Api.AllianceDonate(LegionModel.GetInstance().legionData.legionID, "luxury", DonateResultDo);
        }

        void DonateResultDo(Hashtable data)
        {
            FusionAudio.PostEvent("UI/New/JuanXian", true);
            MergeDataHandler(data);
            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnLegionDonateSucc);
        }

        private bool DonateError(string error)
        {
            switch (error)
            {
                case "costNum is not defined":
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LegionLogic_30632"));
                    return true;
            }
            return false;
        }

        private bool IsDonate(int type, int many)
        {
            //if (LegionModel.GetInstance().legionData.userMemberData.todayDonateTimes >= AlliancesManager.Instance.Config.DonateMaxTimes)
            //{
            //    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_30987"));
            //    return false;
            //}
            //else
            {
                switch (type)
                {
                    case 0:
                        if (AlliancesManager.Instance.CurDonateInfo.goldDonateTimes >= AlliancesManager.Instance.Config.GoldDonateMaxTimes)
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_30987"));
                            return false;
                        }
                        if (many > BalanceResourceUtil.GetUserGold())
                        {
                            GlobalMenuManager.Instance.Open("LTGetGold");
                            //MessageTemplateManager.ShowMessage(AlliancesManager.CodeGoldUnenough);
                            return false;
                        }
                        break;
                    case 1:
                        if (AlliancesManager.Instance.CurDonateInfo.hcDonateTimes >= AlliancesManager.Instance.Config.DiamondDonateMaxTimes)
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_30987"));
                            return false;
                        }
                        if (many > BalanceResourceUtil.GetUserDiamond())
                        {
                            MessageTemplateManager.ShowMessage(LegionConfig.CodeHcUnenough, null, delegate (int r)
                            {
                                if (r == 0)
                                {
                                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                    GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                }
                            });
                            return false;
                        }
                        break;
                    case 2:
                        if (AlliancesManager.Instance.CurDonateInfo.luxuryDonateTimes >= AlliancesManager.Instance.Config.LuxuryDonateMaxTimes)
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_30987"));
                            return false;
                        }
                        if (many > BalanceResourceUtil.GetUserDiamond())
                        {
                            MessageTemplateManager.ShowMessage(LegionConfig.CodeHcUnenough, null, delegate (int r)
                            {
                                if (r == 0)
                                {
                                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                                    GlobalMenuManager.Instance.Open("LTChargeStoreHud", null);
                                }
                            });
                            return false;
                        }
                        break;
                }
            }
            return true;
        }

        void OnSendGetLegionMessages()
        {
            if (LegionModel.GetInstance().legionData != null && Time.unscaledTime - LegionModel.GetInstance().legionData.messageLastRuntimeTime > 10)
            {
                LegionModel.GetInstance().legionData.messageLastRuntimeTime = Time.unscaledTime;
                Api.AllianceGetMessages(LegionModel.GetInstance().legionData.legionID, LegionModel.GetInstance().legionData.messageLastTime, MergeDataHandler);
            }
        }

        void OnSendMemberAddFriend(long uid)
        {
            if (FriendManager.Instance.Info.MyFriendNum >= FriendManager.Instance.Config.MaxFriendNum)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeFriendNumMax);
                return;
            }

            if (FriendManager.Instance.MyFriends.Find(uid) != null)
            {
                MessageTemplateManager.ShowMessage(FriendManager.CodeHasFriend);
                return;
            }

            FriendManager.Instance.Search("#" + uid, delegate (Hashtable result)
            {
                if (result == null)
                {
                    EB.Debug.LogError("Search result=null");
                    return;
                }
                bool isFriend = EB.Dot.Bool("friendsInfo.search." + uid + ".isFriend", result, false);
                if (isFriend)
                {
                    MessageTemplateManager.ShowMessage(FriendManager.CodeHasFriend);
                    return;
                }

                var ht = Johny.HashtablePool.Claim();
                ht.Add("uid", uid);
                ht.Add("addWay", eFriendAddWay.Normal);
                GlobalMenuManager.Instance.Open("FriendApplyUI", ht);
            });
        }

        /// <summary>
        ///发送领取宝箱请求
        /// </summary>
        /// <param name="chestindex"></param>
        /// <param name="onResult"></param>
        public void SendRecieveDonateChest(int aid, int chestindex, Action onResult)
        {
            Api.GetAllianceDonateChest(aid, chestindex, (Hashtable result) =>
            {
                FetchDataHandler(result);
                DataLookupsCache.Instance.CacheData(result);
                onResult();

            });
        }

        void OnSendGiveMonthCard(long uid, Action<long> onResult)
        {
            Api.ExceptionFun = (string error) =>
            {
                if (error.Equals("ID_ERROR_INSUFFICIENT_ITEMS"))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_1, EB.Localizer.GetString("ID_codefont_in_LegionLogic_33945"));
                    return true;
                }
                return false;
            };
            Api.AllianceGiveMonthCard(LegionModel.GetInstance().legionData.legionID, uid, (Hashtable has) =>
            {
                if (has != null)
                {
                    DataLookupsCache.Instance.CacheData(has);
                }

                FetchDataHandler(has);
                onResult(uid);
            }
             );
        }

        void OnOpenMemberTalk(long uid)
        {
            var ht = Johny.HashtablePool.Claim();
            ht.Add("type", eFriendType.Recently);
            ht.Add("uid", uid);
            GlobalMenuManager.Instance.Open("FriendHud", ht);
        }

        private WaitForSeconds wait1 = new WaitForSeconds(1f);
        private IEnumerator WaitToGetLegionInfo()
        {
            yield return wait1;
            Api.GetAlliance(FetchDataHandler);
            MessageDialog.HideCurrent();
            OnRefreshPower();
        }

        #region 军团红点逻辑

        //是否有新事件
        public bool IsHaveNewEvent
        {
            set
            {
                string flag = value ? "TURE" : "FLASE";
                PlayerPrefs.SetString(LoginManager.Instance.LocalUserId.Value.ToString() + "LegionEvent", flag);
            }
            get
            {
                string flagStr = PlayerPrefs.GetString(LoginManager.Instance.LocalUserId.Value + "LegionEvent");

                return !string.IsNullOrEmpty(flagStr) && flagStr == "TURE";
            }
        }

        //是否有军团
        public bool IsHaveLegion()
        {
            bool hasJoin = LegionModel.GetInstance().isJoinedLegion;
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.havelegion, hasJoin ? 0 : 1);
            return hasJoin;
        }

        //是否有人申请军团
        public bool IsHaveApplyMember()
        {
            if (LegionModel.GetInstance().isJoinedLegion)
            {
                long uid = AllianceUtil.GetLocalUid();
                if ((AllianceUtil.IsOwner(uid) || (AllianceUtil.IsExtraOwner(uid))))
                {
                    bool hasApply = AlliancesManager.Instance.Detail.ApplyCount > 0;
                    LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.haveapply, hasApply ? 1 : 0);
                    return hasApply;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.haveapply, 0);
            return false;
        }

        public bool HaveDonateRP()
        {
            if (!AlliancesManager.Instance.isRequestDonate)
            {
                AlliancesManager.Instance.FristRequestDonate();
            }
            int haveRp = IsHaveDonateCount() + IshaveCouldReciveDonateChest();
            //LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.donate, haveRp);
            return haveRp > 0;
        }

        //是否有金币捐献次数
        public int IsHaveDonateCount()
        {
            if (!LegionModel.GetInstance().isJoinedLegion || LegionModel.GetInstance().legionData == null)
            {
                return 0;
            }
            int resTime = 0;
            resTime = AlliancesManager.Instance.Config.GoldDonateMaxTimes - AlliancesManager.Instance.CurDonateInfo.goldDonateTimes;
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.havedonatecount, resTime);
            return resTime;
        }

        public int IshaveCouldReciveDonateChest()
        {
            if (!LegionModel.GetInstance().isJoinedLegion || LegionModel.GetInstance().legionData == null)
            {
                return 0;
            }
            int havechest = 0;
            int totalDonate = AlliancesManager.Instance.Detail.TodayTotalExp;
            var chestlist = AlliancesManager.Instance.CurDonateInfo.chsetinfolist;
            if (chestlist == null)
            {
                return 0;
            }
            for (int i = 0; i < chestlist.Count; i++)
            {
                var tempchest = chestlist[i];
                if (tempchest.hasRecieve == 0 && totalDonate >= tempchest.score)
                {
                    havechest = 1;
                    break;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.havedonatechest, havechest);
            return havechest;
        }

        public int IsHaveCouldLevelupTechSkill()
        {
            if (!LegionModel.GetInstance().isJoinedLegion || LegionModel.GetInstance().legionData == null)
            {
                return 0;
            }
            int hasRed = 0;
            var techskilllist = AlliancesManager.Instance.Account.legionTechInfo.TechInfoList;
            if (techskilllist != null)
            {
                int totalMedal = BalanceResourceUtil.GetUserAllianceDonate();
                int skilllevel;
                int cost;
                int LegionLevel = LegionModel.GetInstance().legionData.legionLevel;
                for (int i = 0; i < techskilllist.Count; i++)
                {
                    var skillitem = techskilllist[i];
                    int skillhasred = 0;
                    skilllevel = skillitem.level;
                    cost = skillitem.cost;
                    if (LegionLevel > skilllevel && skilllevel < skillitem.MaxLevel && cost >= 0 && totalMedal >= cost)
                    {
                        hasRed += 1;
                        skillhasred = 1;
                    }
                    else
                    {
                        skillhasred = 0;
                    }
                    LTRedPointSystem.Instance.SetRedPointNodeNum(string.Format("Main.Legion.Technology.techskill{0}", i + 1), skillhasred);
                }
            }
            return hasRed;
        }

        public int isHaveTechResCanRecieve()
        {
            if (!LegionModel.GetInstance().isJoinedLegion || LegionModel.GetInstance().legionData == null)
            {
                return 0;
            }
            int iscouldrecieve = 0;
            var TechChestList = Data.AllianceTemplateManager.Instance.mTechChestList;
            if (TechChestList == null)
            {
                return 0;
            }
            var chestInfo = AlliancesManager.Instance.Account.legionTechInfo;
            var TechChest = AlliancesManager.Instance.Account.legionTechInfo.CurtechChest;
            if (chestInfo.curExp > 0 && chestInfo.curExp >= (TechChest.expmax * 0.2))
            {
                iscouldrecieve = 1;
            }
            else
            {
                iscouldrecieve = 0;
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.techchest, iscouldrecieve);
            return iscouldrecieve;
        }


        public bool IsHaveTechRP()
        {
            int hasRed = isHaveTechResCanRecieve() + IsHaveCouldLevelupTechSkill();
            //int rednum = hasRed ? 1 : 0;
            //LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.legiontechnology, rednum);
            return hasRed > 0;
        }

        //是否有活动开启
        public bool IsOpenActivity()
        {
            return IsOpenConvoy() || IsOpenLegionFB() || IsOpenLegionBattle();
        }

        //军团护送是否还有次数和活动开启
        public bool IsOpenConvoy()
        {
            bool isOpen = AllianceEscortUtil.GetResidueTransferDartNum() > 0 && Hotfix_LT.Data.EventTemplateManager.Instance.IsTimeOK("escort_start", "escort_stop");
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.convoy, isOpen ? 1 : 0);
            return isOpen;
        }

        private int RequestTime = 0;
        /// <summary>
        /// 军团副本是否开启
        /// </summary>
        /// <param name="setRedpoint">设置红点回调</param>
        /// <returns></returns>
        public bool IsOpenLegionFB()
        {
            Hashtable hashtable;
            DataLookupsCache.Instance.SearchDataByID<Hashtable>("alliance.todayDonateTimes.boss", out hashtable);
            int currentExp = LegionModel.GetInstance().legionData.todayExp;
            Hotfix_LT.Data.AllianceFBBoss currentBoss = null;
            int redpointnum = 0;
            for (int i = 0; i < Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList.Count; i++)
            {
                currentBoss = Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList[i];
                //有进度，又有次数
                if (currentExp >= currentBoss.donate && IsHaveChallengeNum(hashtable, currentBoss,out int num))
                {
                    redpointnum += num;
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.legionfb, redpointnum);
            return redpointnum>0;
        }

        //public void GetLegionFBData(Action<bool> callBack)
        //{
        //    LegionLogic.GetInstance().OnSendGetCurDonateInfo((Hashtable obj) =>
        //    {
        //        DataLookupsCache.Instance.CacheData(obj);
        //        if (callBack != null)
        //        {
        //            Hashtable hashtable = EB.Dot.Object("alliance.todayDonateTimes.boss", obj, null);
        //            //当前的进度
        //            int currentExp = LegionModel.GetInstance().legionData.todayExp;
        //            Hotfix_LT.Data.AllianceFBBoss currentBoss = null;
        //            for (int i = 0; i < Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList.Count; i++)
        //            {
        //                currentBoss = Hotfix_LT.Data.AllianceTemplateManager.Instance.mFBBossList[i];
        //                //有进度，又有次数
        //                if (currentExp >= currentBoss.donate && IsHaveChallengeNum(hashtable, currentBoss))
        //                {
        //                    callBack(true);
        //                    return;
        //                }
        //            }
        //            callBack(false);
        //        }
        //    });
        //}

        /// <summary>
        /// 是否有挑战次数
        /// </summary>
        /// <param name="hashtable">服务器数据</param>
        /// <param name="currentBoss">当前BOSS数据</param>
        /// <returns></returns>
        private bool IsHaveChallengeNum(Hashtable hashtable, Hotfix_LT.Data.AllianceFBBoss currentBoss,out int rpnum)
        {
            bool isHaveChallengeNum = true;
            rpnum = currentBoss.challenge;
            if (hashtable == null)
            {

                return isHaveChallengeNum;
            }
            //判断当前BOSS是否有次数
            foreach (DictionaryEntry entry in hashtable)
            {
                if (int.Parse(entry.Key.ToString()) == currentBoss.monsterId)
                {
                    int challenge = EB.Dot.Integer(entry.Key.ToString(), hashtable, 0);
                    rpnum = currentBoss.challenge - challenge;
                    isHaveChallengeNum = rpnum > 0 ;
                    break;
                }
            }
            return isHaveChallengeNum;
        }

        //军团战是否开启
        public bool IsOpenLegionBattle()
        {
            if (LTLegionWarManager.Instance.IsOpenWarTime())
            {
                LegionWarTimeLine status = LTLegionWarManager.GetLegionWarStatus();
                if (status == LegionWarTimeLine.QualifyGame || status == LegionWarTimeLine.SemiFinal || status == LegionWarTimeLine.Final)
                {
                    string savestring = PlayerPrefs.GetString(LTLegionWarManager.Instance.LocalSavekey);
                    if (string.IsNullOrEmpty(savestring) || !int.TryParse(savestring, out int time) || (EB.Time.Now - time) > 86400)
                    {
                        LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.legionwar, 1);
                        return true;
                    }
                }
            }
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.legionwar, 0);
            return false;
        }

        #endregion

        public bool IsHaveMercenary()
        {
          int heroId = AlliancesManager.Instance.GetMercenaryHeroId();
          if (heroId<=0)
          {
              LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.mercenary, 1);
              return true;
          }

          int cur = AlliancesManager.Instance.GetMercenaryTime();
          int max = AlliancesManager.Instance.GetMercenaryMaxTime();
          bool cannotreward = cur < max || AlliancesManager.Instance.GetIsReward();
          LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.mercenary,
              cannotreward? 0 : 1);
          return !cannotreward;    
        }
    }
}


