using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class ActivityUtil
    {
        public static void FunctionOpen(string parameter, int activityid)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                EB.Debug.LogError("Activity NavParameter xxxxid (int)  is illegal {0}", activityid);
                return;
            }

            int functionid = 0;
            if (!int.TryParse(parameter, out functionid))
            {
                EB.Debug.LogError("Activity NavParameter xxxxid (int)  is illegal {0}", activityid);
                return;
            }

            var func = Hotfix_LT.Data.FuncTemplateManager.Instance.GetFunc(functionid);
            if (func == null)
            {
                EB.Debug.LogError("Activity NavParameter xxxxid (int)  is illegal func {0}", activityid);
                return;
            }
            if (string.IsNullOrEmpty(func.parameter)) Hotfix_LT.Data.FuncTemplateManager.OpenFunc(func.id, null);
            else Hotfix_LT.Data.FuncTemplateManager.OpenFunc(func.id, func.parameter);
        }

        public static void NpcFind(string parameter, int activityid)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                EB.Debug.LogError("Activity NavParameter (xxxscene;xxxlocator)  is illegal {0}", activityid);
                return;
            }

            string[] splits = parameter.Split(';');
            if (splits != null && splits.Length == 2)
            {
                if (AllianceUtil.GetIsInTransferDart(null))
                //if (PlayerManager.LocalPlayerController().IsPlayerInDart())
                {
                    //WorldMapPathManager.Instance.StartPathFindToNpc(MainLandLogic.GetInstance().CurrentSceneName, splits[0], splits[1]);
                }
                else
                {
                    WorldMapPathManager.Instance.StartPathFindToNpcFly(MainLandLogic.GetInstance().CurrentSceneName, splits[0], splits[1]);
                }
            }
            else
            {
                EB.Debug.LogError("Activity NavParameter (xxxscene;xxxlocator) is illegal {0}", activityid);
            }
        }

        public static string TransferTime(string time)
        {
            string[] splits = time.Split(',');
            string result = EB.Localizer.GetString("ID_WEEK_PREFIX");

            if (splits != null && splits.Length > 0)
            {
                if (splits.Length == 7) return EB.Localizer.GetString("ID_EVERYDAY");
                if (splits.Length == 1) return result + IntToWeekDay(splits[0]); ;
                int day = -1;
                bool needtransfer = true;
                int start = 0;
                int end = 0;
                for (int i = 0; i < splits.Length; i++)
                {
                    if (needtransfer)
                    {
                        int tmp = 0;
                        int.TryParse(splits[i], out tmp);
                        if (i != 0)
                        {
                            if (tmp != day) needtransfer = false;
                        }
                        else start = tmp;
                        day = tmp + 1;
                        end = tmp;
                        if (day > 6) day = 0;
                    }
                    if (i == 0) result = result + IntToWeekDay(splits[i]);
                    else result = result + "，" + IntToWeekDay(splits[i]);
                }
                if (needtransfer) result = string.Format(EB.Localizer.GetString("ID_DATE_TO_DATE"), IntToWeekDay(start.ToString()), IntToWeekDay(end.ToString()));
            }
            return result;
        }

        public static string IntToWeekDay(string weekday)
        {
            switch (weekday)
            {
                case "0":
                    return EB.Localizer.GetString("ID_SUNDAY");
                case "1":
                    return EB.Localizer.GetString("ID_1");
                case "2":
                    return EB.Localizer.GetString("ID_2");
                case "3":
                    return EB.Localizer.GetString("ID_3");
                case "4":
                    return EB.Localizer.GetString("ID_4");
                case "5":
                    return EB.Localizer.GetString("ID_5");
                case "6":
                    return EB.Localizer.GetString("ID_6");
                default:
                    EB.Debug.LogError("No weekday for {0}", weekday);
                    return "";
            }
        }

        public static bool IsTimeOk(int activity_id)
        {
            Hotfix_LT.Data.SpecialActivityTemplate template = Hotfix_LT.Data.EventTemplateManager.Instance.GetSpecialActivity(activity_id);
            if (template == null)
            {
                EB.Debug.LogError("Hotfix_LT.Data.SpecialActivityTemplate =null  activity_id={0}", activity_id);
                return false;
            }
            List<Hotfix_LT.Data.NormalActivityInstanceTemplate> instances = Hotfix_LT.Data.EventTemplateManager.Instance.GetNormalActivityInstanceTemplates(activity_id);

            int curweek = EB.Time.LocalWeek;
            int curhours = EB.Time.LocalTimeOfDay.Hours;
            int curminutes = EB.Time.LocalTimeOfDay.Minutes;
            int curtime = curhours * 100 + curminutes;
            string weeks = template.open_time;
            for (var i = 0; i < instances.Count; i++)
            {
                var ins = instances[i];
                int s_i = int.Parse(ins.s.Replace(":", ""));
                int e_i = int.Parse(ins.e.Replace(":", ""));
                bool is_timeok = IsTimeOk(curweek, curtime, ins.t, s_i, e_i, weeks);
                if (is_timeok)
                    return true;
            }
            return false;
        }

        public static bool IsTimeOk(int curweek, int curtime, int t, int s, int e, string week)
        {
            if (t == 0)
            {
                if (curtime >= s && curtime <= e)//时间段确定了 要么ok 要么就不ok
                {
                    if (week.Contains(curweek.ToString())) return true;
                    else return false;
                }
            }
            else
            {
                if (curtime >= s && curtime <= 2359)
                {
                    if (week.Contains(curweek.ToString())) return true;
                    //else return false;
                }

                if (curtime >= 0 && curtime <= e)
                {
                    if (curweek == 0)
                    {
                        if (week.Contains(6.ToString())) return true;
                        else return false;
                    }
                    else if (week.Contains((curweek - 1).ToString())) return true;
                    else return false;
                }
            }
            return false;
        }

        public static bool HasActivityCanGetReward()
        {
            IDictionary activitys = null;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("tl_acs", out activitys)) return false;
            bool state = false;
            if (activitys == null) return state;
            foreach (DictionaryEntry de in activitys)
            {
                int activityid = int.Parse(de.Key.ToString());
                state = state || ActivityCanGetReward(activityid, (IDictionary)de.Value);
                if (state) return state;
            }
            return state;
        }

        public static bool ActivityCanGetReward(int activityid, IDictionary activitydata)
        {
            bool state = false;
            if (activitydata == null) return state;
            int current = EB.Dot.Integer("current", activitydata, 0);
            if (activityid / 1000 == 1) return false;
            List<Hotfix_LT.Data.TimeLimitActivityStageTemplate> stages = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivityStages(activityid);
            for (int i = 0; i < stages.Count; i++)
            {
                Hotfix_LT.Data.TimeLimitActivityStageTemplate stage = stages[i];
                int had = EB.Dot.Integer("stages." + stage.id, activitydata, 0);
                if (current >= stage.stage && stage.num > had)
                {
                    state = true;
                    break;
                }
            }
            return state;
        }

        public static bool IsTimeLimitActivityOpen(int activityid)
        {
            IDictionary activitydata;
            if (!DataLookupsCache.Instance.SearchDataByID<IDictionary>("tl_acs." + activityid, out activitydata))
            {
                return false;
            }

            int s = 0;
            s = EB.Dot.Integer("s", activitydata, s);

            int e = 0;
            e = EB.Dot.Integer("e", activitydata, e);
            int now = EB.Time.Now;
            if (now >= s && now < e) return true;
            return false;
        }

        public static float GetTimeLimitActivityMul(int activityid)
        {
            float mul = 1;
            if (ActivityUtil.IsTimeLimitActivityOpen(activityid))
            {
                Hotfix_LT.Data.TimeLimitActivityTemplate tpl = Hotfix_LT.Data.EventTemplateManager.Instance.GetTimeLimitActivity(activityid);
                if (tpl != null)
                {
                    int tpl_mul = 1;
                    if (int.TryParse(tpl.parameter1, out tpl_mul))
                    {
                        mul = ((float)tpl_mul) / 100;
                    }
                }
            }
            return mul;
        }

        private static ArrayList arrayList;
        public static bool isTimeLimitActivityOpen(int activityId)
        {
            if (DataLookupsCache.Instance.SearchDataByID("events.events", out arrayList))
            {
                for (int i = 0; i < arrayList.Count; i++)
                {
                    int aid = EB.Dot.Integer("activity_id", arrayList[i], 0);
                    if (aid == activityId)
                    {
                        string state = EB.Dot.String("state", arrayList[i], "");
                        int disappear = EB.Dot.Integer("displayuntil", arrayList[i], 0);
                        if (disappear > EB.Time.Now && state.Equals("running"))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    continue;
                }
            }
            return false;
        }

        public static float GetTimeLimitActivityMulWithoutReLog(int activityid)
        {
            float mul = 1;
            Data.TimeLimitActivityTemplate tpl = Data.EventTemplateManager.Instance.GetTimeLimitActivity(activityid);
            if (tpl != null)
            {
                int tpl_mul = 1;
                if (int.TryParse(tpl.parameter1, out tpl_mul))
                {
                    mul = ((float)tpl_mul) / 100;
                }
            }
            return mul;
        }

        #region (UR伙伴排行榜刷新)
        public enum ActivityRankType
        {
            URPartnerRank = 1,
        }
        private static Dictionary<int, int> ActivityRankDic = new Dictionary<int, int> //ios下不能使用未绑定类型报错
        {        
            { 1, 0 },
        };

        public static void RequestRankData(int type,System.Action callback)
        {           
            int lastTime;
            if(ActivityRankDic.TryGetValue(type,out lastTime))
            {
                int nowTime = EB.Time.Now;
                if (nowTime - lastTime > 300)
                {
                    LTMainHudManager.Instance.UpdateActivityLoginData(callback);
                    ActivityRankDic[type] = nowTime;
                }
                else
                {
                    if (callback != null) callback.Invoke();
                }
            }
        }
        public static void ResetRankRefreshRecord(int type)
        {
            int lastTime;
            if (ActivityRankDic.TryGetValue(type, out lastTime))
            {
                if (lastTime == 0) return;
                ActivityRankDic[type] = 0;
            }
        }
        #endregion
    }
}
