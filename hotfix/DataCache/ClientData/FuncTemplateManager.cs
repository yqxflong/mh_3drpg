using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using System.Linq;
using System.Text;
using Hotfix_LT.UI;

namespace Hotfix_LT.Data
{

    public enum FuncOpenType
    {
        none,
        locked,
        time,
        level,
        maincampaign,
    }
    /// <summary>
    /// 功能表格  每个功能UI 功能条件 功能名字  功能参数
    /// </summary>
    public class FuncTemplate
    {
        public int id = 0;
        public string ui_model = "";
        public string display_name = "";
        public string condition = "";
        public string parameter = "";
        public string iconName = "";
        public string discript = null;
        public bool notice;
        public FuncOpenType openType = FuncOpenType.none;
        public int conditionParam = 0;
        public static FuncTemplate Parse(GM.DataCache.FunctionInfo obj)
        {
            FuncTemplate tpl = new FuncTemplate();
            if (obj != null)
            {
                tpl.id = obj.Id;
                tpl.ui_model = obj.UiModel;
                tpl.display_name = EB.Localizer.GetTableString(string.Format("ID_guide_functions_{0}_display_name", tpl.id), obj.DisplayName);// ;
                tpl.condition = obj.Condition;
                tpl.parameter = obj.Parameter;
                tpl.iconName = obj.Icon;
                tpl.discript = EB.Localizer.GetTableString(string.Format("ID_guide_functions_{0}_discript", tpl.id), obj.Discript);// ;
                tpl.notice = obj.Notice == 1;
            }
            string t_condition = tpl.condition;
            if (string.IsNullOrEmpty(t_condition))
            {
                tpl.openType = FuncOpenType.none;
                tpl.conditionParam = 0;
            }
            else if (int.TryParse(t_condition, out int level))
            {
                if (level >= 999)
                {
                    tpl.openType = FuncOpenType.locked;
                }
                else
                {
                    tpl.openType = FuncOpenType.level;
                }
                tpl.conditionParam = level;
            }
            else if (t_condition.Contains("d-"))
            {
                tpl.openType = FuncOpenType.time;
                int.TryParse(t_condition.Replace("d-", string.Empty), out int t_day);
                tpl.conditionParam = t_day;
            }
            else if (t_condition.Contains("m-"))
            {
                tpl.openType = FuncOpenType.maincampaign;
                int.TryParse(t_condition.Replace("m-", string.Empty), out int t_campaign);
                tpl.conditionParam = t_campaign;

            }
            return tpl;
        }

        public bool IsConditionOK()
        {
            switch (openType)
            {
                case FuncOpenType.none:
                    return true;
                case FuncOpenType.locked:
                    return false;
                case FuncOpenType.time:
                    long timeFinal = 0;
                    long timeJoin = 0;
                    DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
                    timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, conditionParam - 1);
                    long ts = timeFinal - EB.Time.Now;
                    if (ts < 0) return true;
                    break;
                case FuncOpenType.level:
                    int user_level = 0;
                    if (EB.Sparx.Hub.Instance == null || (EB.Sparx.Hub.Instance.LevelRewardsManager.Level == 0))
                    {
                        user_level = (LoginManager.Instance == null || LoginManager.Instance.LocalUser == null) ? 0 : LoginManager.Instance.LocalUser.Level;
                    }
                    else
                    {
                        user_level = EB.Sparx.Hub.Instance.LevelRewardsManager.Level; //BalanceResourceUtil.GetUserLevel();
                    }
                    return (user_level >= conditionParam);
                case FuncOpenType.maincampaign:
                    return LTInstanceUtil.IsCampaignsComplete(conditionParam.ToString());
                default:
                    break;
            }
            return false;
        }

        public string GetConditionStr()
        {
            switch (openType)
            {
                case FuncOpenType.none:
                    return string.Empty;
                case FuncOpenType.locked:
                    return EB.Localizer.GetString("ID_codefont_in_LadderController_8669");
                case FuncOpenType.time:
                    long timeFinal = 0;
                    long timeJoin = 0;
                    DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
                    timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, conditionParam - 1);
                    long ts = timeFinal - EB.Time.Now;
                    if (ts > 24 * 60 * 60)
                    {
                        return string.Format(EB.Localizer.GetString("ID_UNLOCK_CONDITION_DAY"), conditionParam);
                    }
                    else if (ts > 0)
                    {
                        return EB.Localizer.GetString("ID_UNLOCK_CONDITION_TOMORROW");
                    }
                    else
                    {
                        return string.Empty;
                    }
                case FuncOpenType.level:
                    return string.Format(EB.Localizer.GetString("ID_FUNC_OPEN_LEVEL"), conditionParam);
                case FuncOpenType.maincampaign:
                    if (conditionParam >= 10000)
                    {
                        var t_campaign = Data.SceneTemplateManager.Instance.GetLostMainCampaignTplById(conditionParam.ToString());
                        return string.Format(EB.Localizer.GetString("ID_FUNC_OPENTIP_1"), t_campaign.Name);
                    }
                    return string.Empty;
                default:
                    break;
            }
            return string.Empty;
        }

        public string GetConditionStrShort()//简单描述 x级解锁
        {
            if (openType == FuncOpenType.level)
            {
                return string.Format(EB.Localizer.GetString("ID_codefont_in_BattleReadyHudController_12002"), condition);
            }
            else
            {
                return GetConditionStr();
            }
        }

        public string GetConditionStrSpecial()
        {
            if (openType == FuncOpenType.level)
            {
                return string.Format(EB.Localizer.GetString("ID_guide_words_902014_context"), condition);//达到x级解锁
            }
            else
            {
                return GetConditionStr();
            }
        }
        public int NeedLevel
        {
            get
            {
                int level_condition = 0;
                int.TryParse(condition, out level_condition);
                return level_condition;
            }
        }
    }

    public class FuncTemplateManager
    {

        private static FuncTemplateManager sInstance = null;

        private Dictionary<int, FuncTemplate> mFuncTbl = null;

        public List<FuncTemplate> mNoticeFuncTbl = null;
        
        public List<int> mUnlockFuncIdLists = null;

        // public Dictionary<int, FuncTemplate> FuncTbl { get { return mFuncTbl; } }

        public static FuncTemplateManager Instance
        {
            get { return sInstance = sInstance ?? new FuncTemplateManager(); }
        }

        // public static bool IsNeedArenaRankLimit = true;

        public static void ClearUp()
        {
            if (sInstance != null)
            {
                sInstance.mFuncTbl.Clear();
                sInstance.mNoticeFuncTbl = null;
                sInstance.mUnlockFuncIdLists = null;
            }
        }

        public bool InitFromDataCache(GM.DataCache.ConditionGuide tbls)
        {
            if (tbls == null)
            {
                EB.Debug.LogError("InitFromDataCache: tbls is null");
                return false;
            }

            var conditionSet = tbls;
            mFuncTbl = new Dictionary<int, FuncTemplate>(conditionSet.FunctionsLength);
            for (int i = 0; i < conditionSet.FunctionsLength; ++i)
            {
                var tpl = FuncTemplate.Parse(conditionSet.GetFunctions(i));
                if (mFuncTbl.ContainsKey(tpl.id))
                {
                    EB.Debug.LogError("InitFuncTbl: {0} exists", tpl.id);
                    mFuncTbl.Remove(tpl.id);
                }
                mFuncTbl.Add(tpl.id, tpl);
            }
            return true;
        }

        public List<int> GetUnlockFuncIdLists()
        {
            // if (mUnlockFuncIdLists!=null)
            // {
            //     return mUnlockFuncIdLists;
            // }
            mUnlockFuncIdLists = new List<int>();
            
            var enumerator = mFuncTbl.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                FuncTemplate obj = enumerator.Current;
                if (!obj.IsConditionOK())
                {
                    mUnlockFuncIdLists.Add(obj.id);
                }
            }
            return mUnlockFuncIdLists;
        }

        public FuncTemplate GetFunc(int id)
        {
            FuncTemplate result = null;
            if (!mFuncTbl.TryGetValue(id, out result))
            {
                EB.Debug.LogWarning("GetFunc: func not found, id = {0}", id);
            }
            return result;
        }

        public List<FuncTemplate> GetLevelUpFunc(int curLevel)//返回升级显示功能列表，现在返回主线副本和等级条件
        {
            List<FuncTemplate> temp = new List<FuncTemplate>();

            var enumerator = mFuncTbl.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                FuncTemplate obj = enumerator.Current;
                if (obj.condition == null || string.IsNullOrEmpty(obj.discript)|| obj.discript.Equals("0")) continue;
                switch (obj.openType)
                {
                    case FuncOpenType.none:
                        break;
                    case FuncOpenType.locked:
                        break;
                    case FuncOpenType.time:
                        break;
                    case FuncOpenType.level:
                        if (obj.conditionParam > curLevel)
                        {
                            temp.Add(obj);
                        }
                        break;
                    case FuncOpenType.maincampaign:
                        if (!obj.IsConditionOK())
                        {
                            temp.Add(obj);
                        }
                        break;
                    default:
                        break;
                }

            }
            temp.Sort((a, b) =>
            {
                int flagx;
                int flagy;
                flagx = a.IsConditionOK() ? 0 : 1;
                flagy = b.IsConditionOK() ? 0 : 1;
                if (flagx != flagy)
                {
                    return flagx - flagy;
                }
                else
                {
                    flagx = a.openType == FuncOpenType.maincampaign ? 0 : 1;
                    flagy = b.openType == FuncOpenType.maincampaign ? 0 : 1;
                    if (flagx != flagy)
                    {
                        return flagx - flagy;
                    }
                    else
                    {
                        return a.conditionParam - b.conditionParam;
                    }
                }
            });
            return temp;
        }

        //初始化预告列表
        public void InitNotice()
        {
            if (mFuncTbl == null) return;
            if (mNoticeFuncTbl == null)
            {
                mNoticeFuncTbl = new List<FuncTemplate>();
            }
            else
            {
                mNoticeFuncTbl.Clear();
            }
            var enumerator = mFuncTbl.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                FuncTemplate funTemp = enumerator.Current;
                if (funTemp.notice && !funTemp.IsConditionOK())
                {
                    mNoticeFuncTbl.Add(funTemp);
                }
            }
            mNoticeFuncTbl.Sort((x, y) =>
            {
                int flagx = 0;
                int flagy = 0;
                flagx = x.condition.Contains("m-") ? 0 : 1;
                flagy = y.condition.Contains("m-") ? 0 : 1;
                if (flagx != flagy)
                {
                    return flagx - flagy;
                }
                int.TryParse(x.condition.Replace("m-", string.Empty), out flagx);
                int.TryParse(y.condition.Replace("m-", string.Empty), out flagy);
                return flagx - flagy;
            });
        }

        public List<FuncTemplate> RefreshNoticeList()
        {
            if (mNoticeFuncTbl == null) InitNotice();
            for (int i = 0; i < mNoticeFuncTbl.Count; i++)
            {
                var noticedata = mNoticeFuncTbl[i];
                if (noticedata.IsConditionOK())
                {
                    mNoticeFuncTbl.RemoveAt(i);
                    i--;
                }
                else
                {
                    return mNoticeFuncTbl;
                }
            }
            return mNoticeFuncTbl;
        }

        public static void OpenFunc(int func_id, object _param = null, bool isqueued = false)
        {
            if (sInstance == null) return;
            FuncTemplate func = sInstance.GetFunc(func_id);
            if (func != null)
            {
                if (func.IsConditionOK())
                {
                    if (func.ui_model.Equals("TaskChase"))
                    {
                        GlobalMenuManager.Instance.Open(func.ui_model, _param);
                    }
                    else if (func.ui_model.Equals("PartnerHandbookHudView"))
                    {
                        GlobalMenuManager.Instance.Open("PartnerHandbookHudView");
                    }
                    else if (func.ui_model.Equals("ArenaView"))
                    {
                        GlobalMenuManager.Instance.Open("ArenaHudUI");
                    }
                    else if (func.ui_model.Equals("HonorArenaView"))
                    {
                        GlobalMenuManager.Instance.Open("HonorArenaView");
                    }
                    else if (func.ui_model.Equals("AllianceView"))
                    {
                        //AllianceHudUI.OpenHud(null);
                    }
                    else if (func.ui_model.Equals("ExpeditionHud"))
                    {
                        GlobalMenuManager.Instance.Open("ExpeditionUI");
                    }
                    else if (func.ui_model.Equals("GoldBuyView"))
                    {
                        GlobalMenuManager.Instance.Open("GoldNormalBuy");
                    }
                    else if (func.ui_model.Equals("LadderView"))
                    {
                        GlobalMenuManager.Instance.Open("LadderUI");
                    }
                    else if (func.ui_model.Equals("LTRuleUIView"))
                    {
                        if (_param != null)
                        {
                            string id = _param as string;
                            string text = EB.Localizer.GetString(EB.Symbols.LocIdPrefix + id.ToUpper());
                            GlobalMenuManager.Instance.Open(func.ui_model, text);
                        }
                        else
                        {
                            GlobalMenuManager.Instance.Open(func.ui_model);
                        }
                    }
                    else if (func.ui_model.Equals("LTLegionWarJoinView"))
                    {
                        if (!AllianceUtil.IsJoinedAlliance)
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_2, EB.Localizer.GetString("ID_codefont_in_FuncTemplateManager_5098"), delegate (int r)
                            {
                                if (r == 0)
                                {

                                    GlobalMenuManager.Instance.Open("LTSearchJTMenu");
                                    if ((LegionModel.GetInstance().searchItemDatas == null || LegionModel.GetInstance().searchItemDatas.Length == 0) || Time.unscaledTime - LegionModel.GetInstance().searchListTime > 10)  //无军团列表数据或最近一次不是自动搜索或拉取数据超过CD10秒
                                    {
                                        LegionModel.GetInstance().searchListTime = Time.unscaledTime;
                                        AlliancesManager.Instance.RequestAllianceList();
                                    }
                                }
                            });
                        }
                        else
                        {
                            GlobalMenuManager.Instance.Open(func.ui_model);
                            FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.alliance_war_topic,
                            FusionTelemetry.GamePlayData.alliance_war_event_id, FusionTelemetry.GamePlayData.alliance_war_umengId, "open");
                        }
                    }
                    else if (func.ui_model.Equals("LTWorldBossHud"))
                    {
						//var activitytmp = EventTemplateManager.Instance.GetSpecialActivity(LTWorldBossDataManager.ActivityId);//满足条件才进入无需再次判断
						//if (activitytmp != null)
						//{
						//    if (BalanceResourceUtil.GetUserLevel() < activitytmp.level)
						//    {
						//        var data = Johny.HashtablePool.Claim();
						//        data.Add("0", activitytmp.level);
						//        MessageTemplateManager.ShowMessage(902009, data, null);
						//        Johny.HashtablePool.Release(data);
						//        return;
						//    }
						//}

						//注掉判断使在非活动时间内依然可以打开世界boos活动界面需要在后面加限制
						if (!LTWorldBossDataManager.Instance.IsLive())
						{
							//MessageTemplateManager.ShowMessage(902185);
							//return;
						}

						if (!LTWorldBossDataManager.Instance.IsWorldBossStart())
                        {
                            //MessageTemplateManager.ShowMessage(902090);
                            //return;
                        }

                        GlobalMenuManager.Instance.Open("LTWorldBossHud");
                        FusionTelemetry.GamePlayData.PostEvent(FusionTelemetry.GamePlayData.worldboss_topic,
                        FusionTelemetry.GamePlayData.worldboss_event_id, FusionTelemetry.GamePlayData.worldboss_umengId, "open");
                    }
                    else
                    {
                        if (func.ui_model.Equals("LTAllianceEscortHud"))
                        {
                            if (!EventTemplateManager.Instance.IsTimeOK("escort_start", "escort_stop"))
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_4329"));
                                return;
                            }

                            if (!AllianceUtil.IsJoinedAlliance)
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_AllianceEscortUtil_4483"));
                                return;
                            }

                            if (AlliancesManager.Instance.DartData.State == eAllianceDartCurrentState.Transfering)
                            {
                                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_FuncTemplateManager_7841"));
                                return;
                            }
                            FusionTelemetry.GamePlayData.PostEsortEvent("open", "esort");
                            //现在没有这种限制
                            //if (AlliancesManager.Instance.DartData.DartState == eAllianceDartState.Robed)
                            //{
                            //	MessageTemplateManager.ShowMessage(902055);  //已劫掠过，不能接受运镖任务。
                            //	return;
                            //}
                        }
                        else if (func.ui_model.Equals("LTNationBattleEntryUI"))
                        {
                            if (string.IsNullOrEmpty(NationManager.Instance.Account.NationName))
                            {
                                GlobalMenuManager.Instance.Open("LTNationHudUI");
                                return;
                            }
                        }

                        if (_param != null)
                        {
                            GlobalMenuManager.Instance.Open(func.ui_model, _param);
                        }
                        else
                        {
                            GlobalMenuManager.Instance.Open(func.ui_model);
                        }
                    }
                }
                else
                {
                    if (func.openType != FuncOpenType.level)
                    {
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, func.GetConditionStr());
                    }
                    else
                    {
                        var ht = Johny.HashtablePool.Claim();
                        ht.Add("0", func.condition);
                        MessageTemplateManager.ShowMessage(902014, ht, null);
                        Johny.HashtablePool.Release(ht);
                    }
                }
            }
        }
    }
}