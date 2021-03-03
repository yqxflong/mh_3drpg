using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using EB;

namespace Hotfix_LT.UI
{
    public enum eTaskTargetType
    {
        None = 0,
        SearchNPC = 1,
        FinishCampaign = 2,
        PlayerLevel = 3,
        JoinFaction = 4,
        Build = 5,
        TowerLevel = 6,
        SpecialAction = 7,  //日常任务类型
        Hant = 8,
        Tian = 9,
        Di = 10,
        Xuan = 11,
        Huang = 12,
        InviteFriend = 13,
        ChallengeCampaign = 103,
        BeatMonster = 104,
        DoubleHit = 201,
        EnchantLevel = 202,
        PartnerCollect = 203,
        RefineLevel = 204,
    }

    public enum eTaskSpecialType
    {
        NONE = 0,
        FINISH_CAMPAIGN = 1,  //主线
        ALLIACE_DONATE = 2,
        GOLD_CAMPAIGN = 3,
        WORLD_CHAT = 4,
        LADDER = 5,
        ARENA = 6,

        BUY_GOLD = 8,

        GAMBLE = 10,
        YABIAO = 11,
        UlTIMATE = 12,
        HANT = 13,
        LADDER_CHALLENGE = 14,

        BUY_VIGOR = 16,
        BUY_ITEM = 17,
        PK = 18,
        EVERYDAY_LOGIN = 19,
        CHALLENGE_CAMPAIGN = 20,
        SEND_VIGOR = 21,
        COST_DIAMOND = 22,
        EQUIP_INTENSIFY = 23,
        CLASH_OF_HEROES = 24,
        EXP_CAMPAIGN = 25,
        CHALLENGE_INSTANCE = 26,//挑战副本消耗入场券
        LEGION_INSSTANCE = 27,  //军团副本
        WORLD_BOSS = 28,
        PARENER_UPLEVEL = 29,
        PARENER_ADVANCED = 30
    }

    public enum eTaskType
    {
        None = 0,
        Main = 1,       //主线
        Branch = 2,     //支线
        Normal = 3,     //日常
        Hant = 4,       //通缉
        Transfer = 5,   //运镖
        OtherSet = 6,   //成长目标
        Maze = 7,       //挑战副本首通
        Week = 8,       //周常
        ComeBack=9,     //回归
        Invite = 10,    //好友邀请
        Promotion = 11, //晋升
    }

    public class TaskSystem
    {
        public static string FINISHED = "finished";
        public static string RUNNING = "running";
        public static string COMPLETED = "completed";
        public static string ACCEPTABLE = "acceptable";
        public static string UNACCEPTABLE = "unacceptable";
        
        public static int TASKPREFIX_LENGTH = 6;

        public static void StartTaskAcceptNpcPathFinding(string taskid)
        {
            if (taskid == null)
            {
                return;
            }

            Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskid);
            if (taskTpl == null)
            {
                EB.Debug.LogWarning("No such task in table!!!");
                return;
            }

            string task_npc = taskTpl.npc_id;
            string task_scene = taskTpl.scene_id;
            if (task_npc != null)
            {
                if (AllianceUtil.IsInTransferDart)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FuncOpenConditionComponent_4248"));
                }
                else
                {
                    EB.Coroutines.Stop(FindNpcFly(task_scene, task_npc));
                    EB.Coroutines.Run(FindNpcFly(task_scene, task_npc));
                }
            }
        }
        
        private static IEnumerator FindNpcFly(string task_scene, string task_npc)
        {
            GlobalMenuManager.Instance.ComebackToMianMenu();
            yield return null;
            WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, task_scene, task_npc);
        }

        //如果处于进行状态  则导航到目的（NPC，场景，副本，某某功能）
        public static void ProcessTaskRunning(string taskid)
        {
            Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskid);
            switch (taskTpl.target_type)
            {
                case 103:
                    StartTaskCampaignFinding(taskid, false);
                    break;
                case 2:
                    StartTaskCampaignFinding(taskid, true);
                    break;
                case 301: //301 = 增加主线（累计计数型）
                    int target_parameter = int.Parse(taskTpl.target_parameter_1);
                    switch (target_parameter)
                    {
                        case 1:  //主角等级
                            GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
                            break;
                        case 2:  //征战关卡获得星数
                            if (AllianceUtil.IsInTransferDart)
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                                return;
                            }
                            GlobalMenuManager.Instance.Open("LTInstanceMapHud", null);
                            break;
                        case 3:  //夺宝奇兵累计杀怪数
                            GlobalMenuManager.Instance.ComebackToMianMenu();
                            break;
                        case 4:  //累计军团护送次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 5:  //累计军团援助次数
                            GlobalMenuManager.Instance.ComebackToMianMenu();
                            break;
                        case 6:  //累计军团拦截次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 7:  //累计幸运悬赏次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 8:  //累计英雄交锋次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 9:  //累计角斗场次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 10:  //累计伙伴上阵图鉴数量
                            Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10049);
                            if (!func.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                                return;
                            }
                            GlobalMenuManager.Instance.Open("PartnerHandbookHudView");
                            break;
                        case 11:  //玩家拥有好友数量
                            {
                                Hotfix_LT.Data.FuncTemplate ftemp = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
                                if (ftemp != null && !ftemp.IsConditionOK())
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ftemp.GetConditionStr());
                                    return;
                                }
                                GlobalMenuManager.Instance.Open("FriendHud", null);
                            }
                            break;
                        case 12:  //玩家拥有伙伴数量
                            GlobalMenuManager.Instance.Open("LTDrawCardTypeUI");
                            break;
                        case 13:  //伙伴总星级  
                            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                            if (ft != null && !ft.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                                return;
                            }
                            GlobalMenuManager.Instance.Open("LTPartnerView", "Develop_StarUp");
                            break;
                        case 14:  //累计军团预赛获得分数
                            LegionLogic.GetInstance().ShowUI();
                            break;
                        case 15:  //累计购买金币次数
                            GlobalMenuManager.Instance.Open("LTResourceShopUI");
                            break;
                        case 16:  //累计购买体力次数
                            GlobalMenuManager.Instance.Open("LTResourceShopUI");
                            break;
                        case 17:  //累计购买伙伴经验次数
                            GlobalMenuManager.Instance.Open("LTResourceShopUI");
                            break;
                        case 18:  //累计购买入场券次数
                            GlobalMenuManager.Instance.Open("LTResourceShopUI");
                            break;
                        case 19:  //累计购买金币抽卡次数
                            GlobalMenuManager.Instance.Open("LTDrawCardTypeUI", DrawCardType.gold);
                            break;
                        case 20:  //累计购买钻石抽卡次数
                            GlobalMenuManager.Instance.Open("LTDrawCardTypeUI", DrawCardType.hc);
                            break;
                        case 21:  //累计完成日常任务次数
                            GlobalMenuManager.Instance.Open("NormalTaskView", null);
                            break;
                        case 22:  //累计天梯胜利次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 23:  //累计军团捐献获得军团币数量
                            LegionLogic.GetInstance().ShowUI(ChoiceState.Donate);
                            break;
                        case 24:  //累计军团护送次数
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        default:
                            EB.Debug.LogError("Main Task target_parameter_1 Error ,target_parameter_1={0}", target_parameter);
                            break;
                    }
                    break;
                case 302:
                    int target_parameter_other = int.Parse(taskTpl.target_parameter_1);
                    switch (target_parameter_other)
                    {
                        case 1:  //玩家完美通关金币副本XX难度
                            {
                                if (AllianceUtil.IsInTransferDart)
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                                    return;
                                }
                                Hotfix_LT.Data.FuncTemplate ftemp = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10020);
                                if (ftemp != null && !ftemp.IsConditionOK())
                                {
                                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ftemp.GetConditionStr());
                                    return;
                                }
                                GlobalMenuManager.Instance.Open("LTResourceInstanceUI", "Gold");
                            }
                            break;
                        case 2:  //玩家完美通关经验副本XX难度
                            if (AllianceUtil.IsInTransferDart)
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                                return;
                            }
                            Hotfix_LT.Data.FuncTemplate ftemp1 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10021);
                            if (ftemp1 != null && !ftemp1.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ftemp1.GetConditionStr());
                                return;
                            }
                            GlobalMenuManager.Instance.Open("LTResourceInstanceUI", "Exp");
                            break;
                        case 3:  //玩家通关极限试练X-Y
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 4:  //玩家天梯达到XX段位
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 5:  //玩家角斗场排名达到X
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 6:  //玩家爵位晋升为XX
                            Hotfix_LT.Data.FuncTemplateManager.OpenFunc(10069);
                            break;
                        case 7:  //玩家攻击城池累计伤害达到X
                            GlobalMenuManager.Instance.Open("LTNationBattleEntryUI");
                            break;
                        case 8:  //玩家防守城池累计修理值达到X
                            GlobalMenuManager.Instance.Open("LTNationBattleEntryUI");
                            break;
                        case 9:  //玩家X个图鉴评分等级评分达到Y
                            Hotfix_LT.Data.FuncTemplate func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10049);
                            if (!func.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                                return;
                            }
                            GlobalMenuManager.Instance.Open("PartnerHandbookHudView");
                            break;
                        case 10:  //玩家把X个伙伴升到Y级
                            Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                            if (ft != null && !ft.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                                return;
                            }
                            GlobalMenuManager.Instance.Open("LTPartnerView", "Develop");
                            break;
                        case 11:  //玩家把X件装备强化到Y级
                            Hotfix_LT.Data.FuncTemplate ft1 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                            if (ft1 != null && !ft1.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft1.GetConditionStr());
                                return;
                            }
                            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                            GlobalMenuManager.Instance.Open("LTPartnerEquipmentUI", LTPartnerDataManager.Instance.GetOwnPartnerList()[0]);
                            break;
                        case 12:  //玩家把X个伙伴进阶到Y
                            Hotfix_LT.Data.FuncTemplate ft2 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                            if (ft2 != null && !ft2.IsConditionOK())
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft2.GetConditionStr());
                                return;
                            }
                            GlobalMenuManager.Instance.Open("LTPartnerView", "Develop_Upgrade");
                            break;
                        case 13:  //荣耀角斗场积分达到Y分
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 14:  //天梯达到Y分
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 15:  //战力达到Y                   
                            GlobalMenuManager.Instance.Open("LTPartnerView", "Develop_Upgrade");
                            break;
                        case 16://荣耀角斗场获胜次数达到X场
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 17://亚曼拉单次挑战伤害达到X
                            StartTaskAcceptNpcPathFinding(taskid);
                            break;
                        case 18://军团副本单次挑战获得宝箱数达到X个
                            LegionLogic.GetInstance().OpenLegionBossUI();
                            break;
                        default:
                            EB.Debug.LogError("Main Task target_parameter_1 Error ,target_parameter_1={0}", target_parameter_other);
                            break;
                    }
                    break;
                case 7:  //日常任务
                    ProcessSpecialActionTaskRunning(taskid);
                    break;
                default:
                    break;
            }
        }

        static void ProcessSpecialActionTaskRunning(string taskid)
        {
            Hotfix_LT.Data.TaskTemplate taskTpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskid);
            if (taskTpl == null)
            {
                return;
            }
            int taskspecialtype = 0; ;
            if (!int.TryParse(taskTpl.target_parameter_1, out taskspecialtype))
                return;

            switch ((eTaskSpecialType)taskspecialtype)
            {
                case eTaskSpecialType.FINISH_CAMPAIGN:
                    if (AllianceUtil.IsInTransferDart)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTInstanceMapHud");
                    break;
                case eTaskSpecialType.ALLIACE_DONATE:
                    LegionLogic.GetInstance().ShowUI(ChoiceState.Donate);
                    break;
                case eTaskSpecialType.GOLD_CAMPAIGN:
                    if (AllianceUtil.IsInTransferDart)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                        return;
                    }
                    Hotfix_LT.Data.FuncTemplate ftemp = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10020);
                    if (ftemp != null && !ftemp.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ftemp.GetConditionStr());
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTResourceInstanceUI", "Gold");
                    break;
                case eTaskSpecialType.WORLD_CHAT:
                    GlobalMenuManager.Instance.Open("ChatHudView", null);
                    break;
                case eTaskSpecialType.LADDER:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.ARENA:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.BUY_GOLD:
                    GlobalMenuManager.Instance.Open("LTResourceShopUI");
                    break;
                case eTaskSpecialType.GAMBLE:
                    GlobalMenuManager.Instance.Open("LTDrawCardTypeUI");
                    break;
                case eTaskSpecialType.YABIAO:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.UlTIMATE:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.HANT:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.LADDER_CHALLENGE:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.BUY_VIGOR:
                    GlobalMenuManager.Instance.Open("LTResourceShopUI");
                    break;
                case eTaskSpecialType.BUY_ITEM:
                    GlobalMenuManager.Instance.Open("LTStoreUI");
                    break;
                case eTaskSpecialType.PK:
                    GlobalMenuManager.Instance.ComebackToMianMenu();
                    break;
                case eTaskSpecialType.CHALLENGE_CAMPAIGN:
                    if (AllianceUtil.IsInTransferDart)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTChallengeInstanceSelectHud");
                    break;
                case eTaskSpecialType.SEND_VIGOR:
                    Hotfix_LT.Data.FuncTemplate ft3 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10057);
                    if (ft3 != null && !ft3.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft3.GetConditionStr());
                        return;
                    }
                    GlobalMenuManager.Instance.Open("FriendHud", null);
                    break;
                case eTaskSpecialType.COST_DIAMOND:
                    GlobalMenuManager.Instance.Open("LTDrawCardTypeUI", DrawCardType.hc);
                    break;
                case eTaskSpecialType.EQUIP_INTENSIFY:
                    Hotfix_LT.Data.FuncTemplate ft2 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                    if (ft2 != null && !ft2.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft2.GetConditionStr());
                        return;
                    }
                    InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
                    GlobalMenuManager.Instance.Open("LTPartnerEquipmentUI", LTPartnerDataManager.Instance.GetOwnPartnerList()[0]);
                    break;
                case eTaskSpecialType.CLASH_OF_HEROES:
                    StartTaskAcceptNpcPathFinding(taskid);
                    break;
                case eTaskSpecialType.EXP_CAMPAIGN:
                    if (AllianceUtil.IsInTransferDart)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                        return;
                    }
                    Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10021);
                    if (ft != null && !ft.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTResourceInstanceUI", "Exp");
                    break;
                case eTaskSpecialType.CHALLENGE_INSTANCE:
                    if (AllianceUtil.IsInTransferDart)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                        return;
                    }
                    Hotfix_LT.Data.FuncTemplate f_ch = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10065);
                    if (f_ch != null && !f_ch.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, f_ch.GetConditionStr());
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTChallengeInstanceSelectHud");
                    break;
                case eTaskSpecialType.LEGION_INSSTANCE:
                    if (!LegionModel.GetInstance().isJoinedLegion)
                    {
                        LegionLogic.GetInstance().ShowUI();
                    }
                    else
                    {
                        GlobalMenuManager.Instance.Open("LTLegionFBUI");
                    }
                    break;
                case eTaskSpecialType.WORLD_BOSS:
                    if (AllianceUtil.GetIsInTransferDart(null))
                    {
                        return;
                    }
                    //配置npc
                    GlobalMenuManager.Instance.CloseMenu("NormalTaskView");
                    string[] strs = new string[2] { "s001a", "AreaTrigger_E" };
                    WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, strs[0], strs[1]);
                    break;
                case eTaskSpecialType.PARENER_ADVANCED:
                    Hotfix_LT.Data.FuncTemplate ft4 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                    if (ft4 != null && !ft4.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft4.GetConditionStr());
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTPartnerView", "Develop_Upgrade");
                    break;

                case eTaskSpecialType.PARENER_UPLEVEL:
                    Hotfix_LT.Data.FuncTemplate ft5 = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10050);
                    if (ft5 != null && !ft5.IsConditionOK())
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft5.GetConditionStr());
                        return;
                    }
                    GlobalMenuManager.Instance.Open("LTPartnerView", "Develop");
                    break;
                default:
                    break;
            }
        }
        

        //主线任务打开副本界面
        public static void StartTaskCampaignFinding(string taskid, bool isMainCampaign)
        {
            if (AllianceUtil.IsInTransferDart)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_5855"));
                return;
            }
            if (isMainCampaign)
            {
                string campaign_name = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(taskid).target_parameter_1;
                Hotfix_LT.Data.LostMainCampaignsTemplate campaignData = Hotfix_LT.Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(campaign_name);

                if (!LTInstanceUtil.GetChapterIsOpen(int.Parse(campaignData.ChapterId)))
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_TaskSystem_17027"));
                    return;
                }
                Action act = new Action(delegate
                {
                    LTMainInstanceHudController.EnterInstance(campaignData.ChapterId);
                });
                UIStack.Instance.ShowLoadingScreen(act, false, true, true);
            }
            else
            {
                Hotfix_LT.Data.FuncTemplate ft = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(10065);
                if (ft != null && !ft.IsConditionOK())
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, ft.GetConditionStr());
                    return;
                }
                GlobalMenuManager.Instance.Open("LTChallengeInstanceSelectHud");
            }
        }
        
        public static Hashtable GetTaskHashTable()
        {
            Hashtable datas;
            if (!DataLookupsCache.Instance.SearchDataByID<Hashtable>("tasks", out datas, null))
            {
                EB.Debug.LogWarning("DataLookupsCache SearchData tasks fail");
            }
            return datas;
        }
        
        public static void RequestChatTaskFinish(ChatRule.CHAT_CHANNEL _eChannel)
        {
            if (_eChannel == ChatRule.CHAT_CHANNEL.CHAT_CHANNEL_WORLD)
            {
                IDictionary tasks = null;
                DataLookupsCache.Instance.SearchDataByID<IDictionary>("tasks", out tasks);
                if (tasks == null)
                {
                    return;
                }
                string task_state = "";
                int task_id = 0;
                var iter = tasks.GetEnumerator();
                while (iter.MoveNext())
                {
                    IDictionary task = iter.Value as IDictionary;
                    eTaskType type =(eTaskType) System.Enum.Parse(typeof(eTaskType), task["task_type"].ToString());
                    if (eTaskType.Normal != type && eTaskType.Week != type) continue;

                    task_state = EB.Dot.String("state", iter.Value, "");
                    if (task_state != "running") continue;

                    task_id = int.Parse(iter.Key.ToString());
                    var tpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
                    if (tpl == null || tpl.target_parameter_1 != "4") continue;

                    LTHotfixManager.GetManager<TaskManager>().RequestChatTaskFinish(task_id, null);
                    break;
                }
            }
        }

        public static void RequestBuddyLevelUpFinish()
        {
            bool shouldSend = false;
            
            IDictionary tasks = null;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("tasks", out tasks);
            if (tasks == null)
            {
                return;
            }
            string task_state = "";
            int task_id = 0;
            var iter = tasks.GetEnumerator();
            while (iter.MoveNext())
            {
                IDictionary task = iter.Value as IDictionary;
                eTaskType type =(eTaskType) System.Enum.Parse(typeof(eTaskType), task["task_type"].ToString());
                if (eTaskType.Normal != type && eTaskType.Week != type && eTaskType.ComeBack != type) continue;

                task_state = EB.Dot.String("state", iter.Value, "");
                if (task_state != "running") continue;

                task_id = int.Parse(iter.Key.ToString());
                var tpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
                if (tpl == null || tpl.target_parameter_1 != "29") continue;

                shouldSend = true;
                break;
            }

            if (shouldSend)
            {
                LTHotfixManager.GetManager<TaskManager>().RequestUplevelTaskFinish(task_id, null);
            }
        }
        
        public static void RequestCombatPowerFinish(int power)
        {
            IDictionary tasks = null;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>("tasks", out tasks);
            if (tasks == null)
            {
                return;
            }
            string task_state = "";
            int task_id = 0;
            var iter = tasks.GetEnumerator();
            bool shouldSend = false;
            while (iter.MoveNext())
            {
                IDictionary task = iter.Value as IDictionary;
                eTaskType type =(eTaskType) System.Enum.Parse(typeof(eTaskType), task["task_type"].ToString());

                task_state = EB.Dot.String("state", iter.Value, "");
                if (task_state != "running") continue;

                task_id = int.Parse(iter.Key.ToString());
                var tpl = Hotfix_LT.Data.TaskTemplateManager.Instance.GetTask(task_id);
                if (tpl == null || tpl.target_type != 302 || tpl.target_parameter_1 != "15") continue;
                shouldSend = true;
                break;
            }

            if (shouldSend)
            {
                LTHotfixManager.GetManager<TaskManager>().RequestUpdateCombatPower(power,task_id, null);
            }
        }
        
        public static string GetState(string task_id)
        {
            string dataID = string.Format("tasks.{0}.state", task_id);
            string state = "";
            if (!DataLookupsCache.Instance.SearchDataByID<string>(dataID, out state))
            {
                EB.Debug.LogWarning("not find task for taskid:{0}", task_id);
            }
            return state;
        }
        
        //DateTime转换为时间戳
        public static long GetTimeSpan(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(time - startTime).TotalSeconds;
        }

        //timeSpan转换为DateTime
        public static DateTime TimeSpanToDateTime(long span)
        {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            time = startTime.AddSeconds(span);
            return time;
        }
    }

}