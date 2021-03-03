using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EB.Sparx;

namespace Hotfix_LT.UI
{
    public class CronEntryBase
    {
        public static int RolledOver = -1;
        public List<int> Values { get; private set; }
        public string Expression { get; private set; }
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }

        /// <summary>
        /// Gets the first value.
        /// </summary>
        /// <value>The first.</value>
        public int First
        {
            get { return Values[0]; }
        }

        /// <summary>
        /// Nexts the specified value.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public int Next(int start)
        {
            for (var i = 0; i < Values.Count; i++)
            {
                var value = Values[i];
                if (value >= start)
                {
                    return value;
                }
            }
            
            return RolledOver;
        }

        /// <summary>
        /// Initializes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        public bool Initialize(string expression, int minValue, int maxValue)
        {
            if (string.IsNullOrEmpty(expression))
            {
                EB.Debug.LogError("expression is illegal");
                return false;
            }
            Expression = expression;
            MinValue = minValue;
            MaxValue = maxValue;
            return ParseExpression();
        }

        private bool ParseExpression()
        {
            Values = new List<int>();
            
            int commaIndex = Expression.IndexOf(",");
            if (commaIndex == -1)
            {
                if (!ParseEntry(Expression)) return false;
            }
            else
            {
                string[] entrys = Expression.Split(new[] { ',' });
                for (var i = 0; i < entrys.Length; i++)
                {
                    string entry = entrys[i];
                    if (!ParseEntry(entry.Trim())) return false;
                }
            }
            return true;
        }

        private bool ParseEntry(string entry)
        {
            // Ensure the entry is not empty.
            //
            if (string.IsNullOrEmpty(entry))
            {
                EB.Debug.LogError("expression is illegal");
                return false;
            }

            // Initialize the indexing information to
            //	add all the values from min to max.
            //
            int minUsed = MinValue;
            int maxUsed = MaxValue;
            int interval = -1;

            // Is there an interval specified?
            //
            if (entry.IndexOf("/") != -1)
            {
                string[] vals = entry.Split('/');
                entry = vals[0];
                if (string.IsNullOrEmpty(entry))
                {
                    EB.Debug.LogError("Entry is empty.");
                }
                if (!int.TryParse(vals[1], out interval))
                {
                    EB.Debug.LogError("Found unexpected character. entry={0}" , entry);
                }
                if (interval < 1)
                {
                    EB.Debug.LogError("Interval out of bounds.{0}" , entry);
                }
            }

            // Is this a wild card
            //
            if (entry[0] == '*' && entry.Length == 1)
            {
                // Wild card only.
                //
                AddValues(minUsed, maxUsed, interval);
            }
            else
            {
                // No wild card.
                // Is this a range?
                //
                if (entry.IndexOf("-") != -1)
                {
                    // Found a range.
                    //
                    string[] vals = entry.Split('-');
                    if (!int.TryParse(vals[0], out minUsed))
                    {
                        EB.Debug.LogError("Found unexpected character. entry={0}" , entry);
                        return false;
                    }
                    if (minUsed < MinValue)
                    {
                        EB.Debug.LogError("Minimum value less than expected.");
                        return false;
                    }
                    if (!int.TryParse(vals[1], out maxUsed))
                    {
                        EB.Debug.LogError("Found unexpected character. entry={0}" , entry);
                        return false;
                    }
                    if (maxUsed > MaxValue)
                    {
                        EB.Debug.LogError("Maximum value greater than expected.");
                        return false;
                    }
                    if (minUsed > maxUsed)
                    {
                        EB.Debug.LogError("Maximum value less than minimum value.");
                        return false;
                    }
                    AddValues(minUsed, maxUsed, interval);
                }
                else
                {
                    // Must be a single number.
                    //
                    if (!int.TryParse(entry, out minUsed))
                    {
                        EB.Debug.LogError("Found unexpected character. entry={0}" , entry);
                        return false;
                    }
                    if (minUsed < MinValue)
                    {
                        EB.Debug.LogError("Value is less than minimum expected.");
                        return false;
                        //throw new CronEntryException("Value is less than minimum expected.");
                    }
                    if (interval == -1)
                    {
                        // No interval (eg. '5' or '9')
                        //
                        maxUsed = minUsed;
                        if (maxUsed > MaxValue)
                        {
                            EB.Debug.LogError("Value is greater than maximum expected.");
                            return false;
                        }
                        AddValues(minUsed, maxUsed, interval);
                    }
                    else
                    {
                        // Interval and a single number
                        // eg. '2/5'  --> 2,7,12,17,...<max
                        //
                        maxUsed = MaxValue;
                        AddValues(minUsed, maxUsed, interval);
                    }
                }

            }
            return true;
        }

        private void AddValues(int minUsed, int maxUsed, int interval)
        {
            if (interval == -1)
            {
                interval = 1;
            }
            for (int i = minUsed; i <= maxUsed; i += interval)
            {
                if (!Values.Contains(i))
                {
                    Values.Add(i);
                }
            }
        }
    }

    public class TimerScheduler
    {
        public string m_TimerRegular;
        public bool isLegalTimer = true;
        private CronEntryBase _minutes;
        private CronEntryBase _hours;
        private CronEntryBase _days;
        private CronEntryBase _months;
        private CronEntryBase _daysOfWeek;

        public void SetTimerRegular()
        {

        }

        public void ParseTimerRegular()
        {
            if (string.IsNullOrEmpty(m_TimerRegular))
            {
                isLegalTimer = false;
                return;
            }
            string[] splits = m_TimerRegular.Split(' ');
            if (splits.Length < 5)
            {
                isLegalTimer = false;
                return;
            }
            _minutes = new CronEntryBase();
            _hours = new CronEntryBase();
            _days = new CronEntryBase();
            _months = new CronEntryBase();
            _daysOfWeek = new CronEntryBase();
            isLegalTimer = isLegalTimer && _minutes.Initialize(splits[0], 0, 59);
            isLegalTimer = isLegalTimer && _hours.Initialize(splits[1], 0, 23);
            isLegalTimer = isLegalTimer && _days.Initialize(splits[2], 1, 31);
            isLegalTimer = isLegalTimer && _months.Initialize(splits[3], 1, 12);
            isLegalTimer = isLegalTimer && _daysOfWeek.Initialize(splits[4], 0, 6);
            return;
        }

        public List<System.DateTime> GetAll(System.DateTime start, System.DateTime end)
        {
            List<System.DateTime> result = new List<System.DateTime>();

            System.DateTime current = start;
            while (current <= end)
            {
                System.DateTime next;
                if (!GetNext(current, end, out next))
                {
                    // Did not find any new ones...return what we have
                    //
                    break;
                }
                result.Add(next);
                current = next;
            }
            return result;
        }

        public bool GetNext(System.DateTime start, out System.DateTime next)
        {
            return GetNext(start, System.DateTime.MaxValue, out next);
        }

        public bool GetNext(System.DateTime start, System.DateTime end, out System.DateTime next)
        {
            next = System.DateTime.MinValue;
            
            System.DateTime baseSearch = start.AddMinutes(1.0);
            int baseMinute = baseSearch.Minute;
            int baseHour = baseSearch.Hour;
            int baseDay = baseSearch.Day;
            int baseMonth = baseSearch.Month;
            int baseYear = baseSearch.Year;
            
            int minute = _minutes.Next(baseMinute);
            if (minute == CronEntryBase.RolledOver)
            {
                minute = _minutes.First;
                baseHour++;
            }
            
            int hour = _hours.Next(baseHour);
            if (hour == CronEntryBase.RolledOver)
            {
                minute = _minutes.First;
                hour = _hours.First;
                baseDay++;
            }
            else if (hour > baseHour)
            {
                minute = _minutes.First;
            }
            
            int day = _days.Next(baseDay);
            if (day == CronEntryBase.RolledOver)
            {
                minute = _minutes.First;
                hour = _hours.First;
                day = _days.First;
                baseMonth++;
                if (baseMonth > 12)
                {
                    baseMonth = 1;
                    baseYear++;
                }
            }
            else if (day > baseDay)
            {
                minute = _minutes.First;
                hour = _hours.First;
            }
            while (day > System.DateTime.DaysInMonth(baseYear, baseMonth))
            {
                minute = _minutes.First;
                hour = _hours.First;
                day = _days.First;
                baseMonth++;
            }
            
            int month = _months.Next(baseMonth);
            if (month == CronEntryBase.RolledOver)
            {
                minute = _minutes.First;
                hour = _hours.First;
                day = _days.First;
                month = _months.First;
                baseYear++;
            }
            else if (month > baseMonth)
            {
                minute = _minutes.First;
                hour = _hours.First;
                day = _days.First;
            }
            while (day > System.DateTime.DaysInMonth(baseYear, month))
            {
                minute = _minutes.First;
                hour = _hours.First;
                day = _days.First;
                month = _months.Next(month + 1);
                if (month == CronEntryBase.RolledOver)
                {
                    minute = _minutes.First;
                    hour = _hours.First;
                    day = _days.First;
                    month = _months.First;
                    baseYear++;
                }
            }
            
            System.DateTime suggested = new System.DateTime(baseYear, month, day, hour, minute, 0, 0);
            if (suggested >= end)
            {
                return false;
            }
            
            if (_daysOfWeek.Values.Contains((int)suggested.DayOfWeek))
            {
                next = suggested;
                return true;
            }
            
            return GetNext(new System.DateTime(baseYear, month, day, 23, 59, 0, 0), out next);
        }

        public static string AmendCronFormat(string origin_cron)
        {
            string[] splits = origin_cron.Split(' ');
            if (splits.Length >= 6)
            {
                string amend = origin_cron.Substring(splits[0].Length + 1, origin_cron.Length - (splits[0].Length + 1));
                string[] ss = amend.Split(' ');
                if (ss.Length >= 6)
                {
                    return AmendCronFormat(amend);
                }
                else
                    return amend;
            }
            else
                return origin_cron;
        }
    }

    public abstract class RefreshManager
    {
        protected float timer = 0;
        protected float fixedTimer = 15;

        public RefreshManager(int delta)
        {
            fixedTimer = delta;
        }

        public virtual void InitRefresher(IDictionary rules)
        {
            timer = 0;
        }

        public virtual void Process()
        {

        }

        public virtual void End()
        {

        }

        public void Update()
        {
            timer += Time.deltaTime;
            if (timer >= fixedTimer)
            {
                timer = timer - fixedTimer;
                Process();
            }
        }
    }

    public class DeltaActionManager : RefreshManager
    {
        private List<DeltaActionExcuter> m_schedulers = new List<DeltaActionExcuter>();

        public DeltaActionManager(int delta) : base(delta)
        {

        }

        public override void InitRefresher(IDictionary rules)
        {
            base.InitRefresher(rules);
        }

        public void AddAction(DeltaActionExcuter action)
        {
            m_schedulers.Add(action);
            EB.Debug.Log("DeltaAction {0} is Start s={1} e={2} d={3}", action.id, action.start, action.end, action.delta);
        }

        public void RemoveAction(string id)
        {
            for (int i = m_schedulers.Count - 1; i >= 0; i--)
            {
                if (m_schedulers[i].id == id)
                {
                    EB.Debug.Log("DeltaAction {0} is Start s={1} e={2} d={3}", m_schedulers[i].id, m_schedulers[i].start, m_schedulers[i].end, m_schedulers[i].delta);
                    m_schedulers.RemoveAt(i);
                    break;
                }
            }
        }

        public override void Process()
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                m_schedulers[i].Process();
            }

            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].need_dispose)
                {
                    m_schedulers.RemoveAt(i);
                }
            }
        }

        public override void End()
        {
            m_schedulers.Clear();
            base.End();
        }
    }

    public class DeltaActionExcuter
    {
        public class DeltaAction
        {
            public virtual void Excute(object data)
            {

            }
        }
        public string id;
        public int start;
        public int end;
        public int delta;
        private object data;
        private string action;

        private int next_action_time;
        public bool need_dispose = false;

        public DeltaActionExcuter(string id, int start, int end, int delta, string action, object data)
        {
            this.id = id;
            this.start = start;
            this.end = end;
            this.delta = delta;
            this.data = data;
            this.action = action;
            next_action_time = EB.Time.Now + delta;
        }

        public void Register()
        {
            LTHotfixManager.GetManager<AutoRefreshingManager>().AddDeltaActionExcute(this);
        }

        public void Process()
        {
            int now = EB.Time.Now;
            if (now > end)
            {
                need_dispose = true;
            }
            if (now < start)
            {
                return;
            }
            if (now >= next_action_time)
            {
                next_action_time = EB.Time.Now + delta;
                try
                {
                    Type type = Type.GetType(action);
                    object obj = Activator.CreateInstance(type);
                    DeltaAction Action = (DeltaAction)obj;
                    Action.Excute(data);
                }
                catch (Exception e)
                {
                    EB.Debug.Log(e.StackTrace);
                    need_dispose = true;
                }
            }
        }
    }

    public class CronRefreshManager : RefreshManager
    {
        private List<CronRefreshExcuter> m_schedulers = new List<CronRefreshExcuter>();

        public CronRefreshManager(int delta) : base(delta)
        {

        }

        public override void InitRefresher(IDictionary rules)
        {
            base.InitRefresher(rules);

            m_schedulers.Clear();
        }

        public override void Process()
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                m_schedulers[i].Process();
            }
        }

        public override void End()
        {
            m_schedulers.Clear();

            base.End();
        }

        public void AddCronRefreshExcuter(CronRefreshExcuter excuter)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].Name == excuter.Name)
                {
                    EB.Debug.LogError("CronReFreshExcuter Multiple for Name = {0}", excuter.Name);
                    return;
                }
            }

            m_schedulers.Add(excuter);
        }

        public void RemoveCronRefreshExcuter(string name)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].Name == name)
                {
                    m_schedulers.Remove(m_schedulers[i]);
                    return;
                }
            }
        }

        public CronRefreshExcuter GetCronRefreshExcuter(string name)
        {
            return m_schedulers.Find(excuter => excuter.Name == name);
        }
    }

    public class CronRefreshExcuter
    {
        public TimerScheduler m_TimerScheduler = null;
        public string m_RequestURL = string.Empty;
        private int m_NextRequestTime = 0;
        private string m_Name = null;
        private string m_TimePath = string.Empty;
        private int m_Level = 0;
        private Hashtable m_Parameters = null;
        private bool m_IsPost = true;
        private System.Action<Hashtable> m_Callback;

        public string Name
        {
            get { return m_Name; }
        }

        public int NextRequestTime
        {
            get { return m_NextRequestTime; }
        }

        public CronRefreshExcuter(string name, bool isPost = true)
        {
            m_Name = name;
            m_IsPost = isPost;
        }

        public void Init(IDictionary rule, System.Action<Hashtable> callback = null)
        {
            if (rule == null)
                return;

            if (callback != null)
                m_Callback = callback;
            m_RequestURL = EB.Dot.String("url", rule, m_RequestURL);
            m_TimePath = EB.Dot.String("time_path", rule, m_TimePath);
            m_Level = EB.Dot.Integer("level", rule, m_Level);
            m_Parameters = EB.Dot.Object("parameters", rule, m_Parameters);

            string regular = EB.Dot.String("regular", rule, "");

            m_TimerScheduler = new TimerScheduler();
            m_TimerScheduler.m_TimerRegular = regular;
            m_TimerScheduler.ParseTimerRegular();
            if (!m_TimerScheduler.isLegalTimer)
            {
               EB.Debug.LogError("CronRefreshExcuter:Init m_TimerScheduler.Not isLegalTimer regular=" + regular);
                return;
            }

            m_NextRequestTime = GetNextRequestTime(m_TimePath);
            EB.Debug.Log("CronRefreshExcuter.Init: next time = {0}", EB.Time.FromPosixTime(m_NextRequestTime));
        }

        private int GetNextRequestTime(string path)
        {
            System.DateTime next;
            m_TimerScheduler.GetNext(System.TimeZone.CurrentTimeZone.ToLocalTime(EB.Time.FromPosixTime(EB.Time.Now)), out next);
            return EB.Time.ToPosixTime(System.TimeZone.CurrentTimeZone.ToUniversalTime(next)) /*+ UnityEngine.Random.Range(1, 20)*/;
        }

        public void Process()
        {
            if (!m_TimerScheduler.isLegalTimer)
                return;

            if (m_NextRequestTime <= 0)
            {
                m_NextRequestTime = GetNextRequestTime(m_TimePath);
                EB.Debug.Log("CronRefreshExcuter.Process: next time = {0}", EB.Time.FromPosixTime(m_NextRequestTime));
                if (m_NextRequestTime <= 0) return;
            }

            int now = EB.Time.Now;
            if (now > m_NextRequestTime)
            {
                m_NextRequestTime = GetNextRequestTime(m_TimePath);
                EB.Debug.Log("CronRefreshExcuter.Process: next time = {0}", EB.Time.FromPosixTime(m_NextRequestTime));
                if (m_NextRequestTime <= 0)
                    return;

                DataLookupSparxManager lookup = SparxHub.Instance.GetManager<DataLookupSparxManager>();
                var request = (m_IsPost) ? lookup.EndPoint.Post(m_RequestURL) : lookup.EndPoint.Get(m_RequestURL);
                request.numRetries = 2;
                if (m_Parameters != null)
                {
                    request.AddData(m_Parameters);
                }


                lookup.Service(request, delegate (EB.Sparx.Response res)
                {
                    if (res.sucessful)
                    {
                        if (m_Callback != null)
                        {
                            m_Callback(res.hashtable);
                        }
                        EB.Debug.Log("CronRefreshExcuter.Process: sucessful");
                    }
                    else if (res.fatal)
                    {
                        SparxHub.Instance.FatalError(res.localizedError);
                    }
                    else
                    {
                        EB.Debug.LogWarning("CronRefreshExcuter.Process: error = {0}", res.localizedError);
                    }
                }, true);
            }
        }
    }

    public class DeltaTimeRefresherManager : RefreshManager
    {
        private List<DeltaTimeRefresher> m_schedulers = new List<DeltaTimeRefresher>();

        public DeltaTimeRefresherManager(int delta) : base(delta)
        {

        }

        public override void InitRefresher(IDictionary rules)
        {
            base.InitRefresher(rules);

            m_schedulers.Clear();
        }

        public override void Process()
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                m_schedulers[i].Process();
            }
        }

        public override void End()
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                m_schedulers[i].End();
            }
            m_schedulers.Clear();

            base.End();
        }

        public T AddRefresher<T>(IDictionary rule) where T : DeltaTimeRefresher, new()
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i] is T)
                {
                    m_schedulers[i].Init(rule);
                    return m_schedulers[i] as T;
                }
            }

            T refresher = new T();
            refresher.Init(rule);
            m_schedulers.Add(refresher);
            return refresher;
        }

        public void RemoveRefresher<T>() where T : DeltaTimeRefresher
        {
            for (int i = m_schedulers.Count - 1; i >= 0; i--)
            {
                if (m_schedulers[i] is T)
                {
                    m_schedulers.RemoveAt(i);
                }
            }
        }

        public T GetRefresher<T>() where T : DeltaTimeRefresher
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i] is T)
                {
                    return m_schedulers[i] as T;
                }
            }

            return null;
        }
    }

    public class DeltaTimeRefresher
    {
        protected int m_delta = 0;
        protected string m_dataPath = string.Empty;
        protected int m_min = 0;
        protected int m_max = 0;
        protected int m_offset = 0;
        protected int m_nextRequestTime = 0;
        protected string m_timePath = string.Empty;

        public DeltaTimeRefresher()
        {

        }

        public virtual void Init(IDictionary rule)
        {
            if (rule == null) return;

            m_dataPath = EB.Dot.String("data_path", rule, m_dataPath);
            m_min = EB.Dot.Integer("min", rule, m_min);
            m_max = EB.Dot.Integer("max", rule, m_max);
            m_delta = EB.Dot.Integer("delta", rule, m_delta);
            m_offset = EB.Dot.Integer("offset", rule, m_offset);
            m_timePath = EB.Dot.String("time_path", rule, m_timePath);

            m_nextRequestTime = GetNextRequestTime(m_timePath);
        }

        public virtual void Process()
        {
            if (m_offset == 0 || m_delta == 0) return;
            if (m_nextRequestTime == 0)
            {
                m_nextRequestTime = GetNextRequestTime(m_timePath);
                if (m_nextRequestTime == 0) return;
            }
            if (EB.Time.Now >= m_nextRequestTime)
            {
                m_nextRequestTime = m_nextRequestTime + m_delta;
                int current_value;
                if (DataLookupsCache.Instance.SearchIntByID(m_dataPath, out current_value))
                {
                    if (current_value >= m_max) return;
                    current_value += m_offset;
                    if (current_value > m_max)
                    {
                        current_value = m_max;
                    }
                    if (current_value < m_min)
                    {
                        current_value = m_min;
                    }
                    DataLookupsCache.Instance.CacheData(m_dataPath, current_value);
                }
            }
        }

        public virtual void End()
        {
        }

        private int GetNextRequestTime(string path)
        {
            int time = 0;
            if (!DataLookupsCache.Instance.SearchIntByID(path, out time)) return 0;
            if (time <= 0) time = EB.Time.Now + m_delta;
            return time;
        }
    }

    public class TimeRefresherManager
    {
        private List<TimeRefresher> m_schedulers = new List<TimeRefresher>();

        public virtual void InitRefresher()
        {
            m_schedulers.Clear();
        }

        public void AddRefreshExcuter(TimeRefresher excuter)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].Name == excuter.Name)
                {
                    EB.Debug.LogError("TimeRefresherExcuter Multiple for Name = {0}", excuter.Name);
                    return;
                }
            }

            m_schedulers.Add(excuter);
        }

        public bool GetRefreshed(string name)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].Name == name)
                {
                    return m_schedulers[i].Refreshed;
                }
            }
            EB.Debug.LogError("cannot find timeRrefresher excuter ={0}" , name);
            return false;
        }

        public System.DateTime GetRefreshNextTime(string name)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].Name == name)
                {
                    return m_schedulers[i].NextTime();
                }
            }
            EB.Debug.LogError("cannot find timeRrefresher excuter ={0}", name);
            return Data.ZoneTimeDiff.GetServerTime();
        }

        public virtual void End()
        {
            m_schedulers.Clear();
        }
    }

    public class TimeRefresher
    {
        private System.DateTime m_RefreshTime;
        private TimerScheduler m_TimerScheduler;
        public string Name;

        public TimeRefresher(string name, System.DateTime beginTime, int refreshHour, int refreshMinute)
        {
            this.Name = name;
            string cronFormat = string.Format("{0} {1} * * *", refreshMinute, refreshHour);
            m_TimerScheduler = new TimerScheduler();
            m_TimerScheduler.m_TimerRegular = cronFormat;
            m_TimerScheduler.ParseTimerRegular();
            if (!m_TimerScheduler.isLegalTimer)
            {
                EB.Debug.LogError("TimeRefresher cronFormat is illegal");
                return;
            }
            m_TimerScheduler.GetNext(beginTime, out m_RefreshTime);
        }

        public bool Refreshed
        {
            get
            {
                bool isRefreshed = Data .ZoneTimeDiff .GetServerTime() > m_RefreshTime;
                if (isRefreshed)
                {
                    m_TimerScheduler.GetNext(Data.ZoneTimeDiff.GetServerTime(), out m_RefreshTime);
                }
                return isRefreshed;
            }
        }


        public System .DateTime NextTime()
        {
            return m_RefreshTime;
        }
    }

    public class ActivityRollingMsgManager : RefreshManager
    {
        private List<ActivityRollingMsgRefresher> m_schedulers = new List<ActivityRollingMsgRefresher>();

        public ActivityRollingMsgManager(int delta) : base(delta)
        {
        }

        public override void InitRefresher(IDictionary rules)
        {
            base.InitRefresher(rules);

            m_schedulers.Clear();
        }

        public override void Process()
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                m_schedulers[i].Process();
            }
        }

        public override void End()
        {
            m_schedulers.Clear();
            base.End();
        }

        /// <summary>
        /// 添加前端活动跑马灯逻辑
        /// </summary>
        /// <param name="id">guide表中words中的id,用来触发文字提示的事件</param>
        /// <param name="name">event表中活动cronjobs中的name，用来拿时间</param>
        public void AddRollingMsgActivity(int msgId, string name)
        {
            Hotfix_LT.Data.CronJobs cron = Hotfix_LT.Data.EventTemplateManager.Instance.GetCronJobsByName(name);
            string[] strs = cron.interval.Split(' ');
            if (strs.Length != 6)
            {
                EB.Debug.LogError("cronTable Str Length!=6");
            }
            else
            {
                string minute = strs[1];
                string[] hour = strs[2].Split(',');
                string week = strs[5];
                for (int i = 0; i < hour.Length; i++)
                {
                    m_schedulers.Add(new ActivityRollingMsgRefresher(msgId, Data.ZoneTimeDiff.GetServerTime(), minute, hour[i], week));
                }
            }
        }

        /// <summary>
        /// 移除跑马灯逻辑
        /// </summary>
        /// <param name="msgId"></param>
        public void RemoveRollingMsgActivity(int msgId)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].MsgId == msgId)
                {
                    m_schedulers.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 跑马灯强行移至下次开放时间
        /// </summary>
        /// <param name="msgId"></param>
        public void MoveNextRollingMsgActivity(int msgId, DateTime dateTime)
        {
            for (int i = 0; i < m_schedulers.Count; i++)
            {
                if (m_schedulers[i].MsgId == msgId)
                {
                    m_schedulers[i].MoveNext(dateTime);
                    break;
                }
            }
        }
    }

    public class ActivityRollingMsgRefresher
    {
        public int MsgId;
        private System.DateTime m_RefreshTime;
        private TimerScheduler m_TimerScheduler;

        public ActivityRollingMsgRefresher(int msgId, System.DateTime beginTime, string refreshMinute, string refreshHour, string refreshWeek)
        {
            this.MsgId = msgId;
            string cronFormat = string.Format("{0} {1} * * {2}", refreshMinute, refreshHour, refreshWeek);
            m_TimerScheduler = new TimerScheduler();
            m_TimerScheduler.m_TimerRegular = cronFormat;
            m_TimerScheduler.ParseTimerRegular();
            if (!m_TimerScheduler.isLegalTimer)
            {
                EB.Debug.LogError("TimeRefresher cronFormat is illegal");
                return;
            }
            m_TimerScheduler.GetNext(beginTime, out m_RefreshTime);
        }

        public void Process()
        {
            if (Refreshed)
            {
                MessageTemplateManager.ShowMessage(MsgId);
            }
        }

        private bool Refreshed
        {
            get
            {
                bool isRefreshed = Data.ZoneTimeDiff.GetServerTime() > m_RefreshTime;
                if (isRefreshed)
                {
                    m_TimerScheduler.GetNext(Data.ZoneTimeDiff.GetServerTime(), out m_RefreshTime);
                }
                return isRefreshed;
            }
        }

        public void MoveNext(DateTime dateTime)
        {
            m_TimerScheduler.GetNext(dateTime, out m_RefreshTime);
        }
    }

    public class VigorDeltaTimeRefresher : DeltaTimeRefresher
    {
        public override void Init(IDictionary rule)
        {
            base.Init(rule);

            EB.Sparx.ResourcesManager resManager = SparxHub.Instance.GetManager<EB.Sparx.ResourcesManager>();
            resManager.OnResourcesUpdateListener += OnResourcesUpdate;
            EB.Sparx.Hub.Instance.LevelRewardsManager.OnLevelChange += OnLevelChange;
        }

        public override void End()
        {
            EB.Sparx.ResourcesManager resManager = SparxHub.Instance.GetManager<EB.Sparx.ResourcesManager>();
            resManager.OnResourcesUpdateListener -= OnResourcesUpdate;
            EB.Sparx.Hub.Instance.LevelRewardsManager.OnLevelChange -= OnLevelChange;
        }

        private void OnLevelChange(LevelRewardsStatus status)
        {
            m_max = BalanceResourceUtil.GetUserVigorMax();
        }

        private void OnResourcesUpdate(object payload)
        {
            if (payload is Hashtable)
            {
                Hashtable data = payload as Hashtable;
                if (data.Contains("resource") && data.Contains("balance"))
                {
                    string resourceName = data["resource"].ToString();

                    if (string.IsNullOrEmpty(resourceName)) return;
                    if (resourceName == "vigor")
                    {
                        long nextGrowthTime = 0L;
                        long.TryParse(data["nextGrowthTime"].ToString(), out nextGrowthTime);
                        if (nextGrowthTime > 0L)
                            m_nextRequestTime = (int)nextGrowthTime;
                    }
                }
            }
            else if (payload is ArrayList)
            {
                ArrayList array = payload as ArrayList;
                for (var i = 0; i < array.Count; i++)
                {
                    object obj = array[i];
                    Hashtable data = obj as Hashtable;
                    if (data.Contains("resource") && data.Contains("balance"))
                    {
                        string resourceName = data["resource"].ToString();
                        
                        if (string.IsNullOrEmpty(resourceName))
                            return;
                        if (resourceName == "vigor")
                        {
                            long nextGrowthTime = 0L;
                            long.TryParse(data["nextGrowthTime"].ToString(), out nextGrowthTime);
                            if (nextGrowthTime > 0L)
                                m_nextRequestTime = (int)nextGrowthTime;
                            break;
                        }
                    }
                }
            }
        }

        public string GetVigorRecoverOneCountDown()
        {
            int current_value;
            DataLookupsCache.Instance.SearchIntByID(m_dataPath, out current_value);
            if (current_value >= BalanceResourceUtil.GetUserVigorMax())
                return EB.Localizer.GetString("ID_HAVE_RECOVER_FULL");

            System.TimeSpan ts = EB.Time.FromPosixTime(m_nextRequestTime) - EB.Time.FromPosixTime(EB.Time.Now);  //减去7点半
            int hours = ts.Hours;
            int minutes = ts.Minutes;
            int seconds = ts.Seconds;
            string timeS = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            return timeS;
        }

        public string GetVigorRecoverAllCountDown()
        {
            int current_value;
            if (DataLookupsCache.Instance.SearchIntByID(m_dataPath, out current_value))
            {
                int residueVigor = BalanceResourceUtil.GetUserVigorMax() - current_value - 1;

                if (residueVigor <= 0)
                {
                    return string.Empty;
                }

                int totalSeconds = residueVigor * m_delta;

                System.TimeSpan ts = EB.Time.FromPosixTime(m_nextRequestTime) - EB.Time.FromPosixTime(EB.Time.Now);  //减去7点半
                ts += System.TimeSpan.FromSeconds(totalSeconds);
                int hours = ts.Hours + ts.Days * 24;
                int minutes = ts.Minutes;
                int seconds = ts.Seconds;

                string timeS = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                return timeS;
            }
            return "00:00:00";
        }

        public int GetVigorRecoverFullNeedTime()
        {
            int current_value;
            if (DataLookupsCache.Instance.SearchIntByID(m_dataPath, out current_value))
            {
                int residueVigor = m_max - current_value - 1;

                if (residueVigor <= 0)
                {
                    return 0;
                }

                int totalSeconds = residueVigor * m_delta;

                System.TimeSpan ts = EB.Time.FromPosixTime(m_nextRequestTime) - EB.Time.FromPosixTime(EB.Time.Now);
                ts += System.TimeSpan.FromSeconds(totalSeconds);
                return (int)ts.TotalSeconds;
            }
            return 0;
        }

        public static int GetVigorFreshDeltaTime()
        {
            int delta = 360;
            DataLookupsCache.Instance.SearchIntByID("res.vigor.growthInterval", out delta);
            return delta;
        }
    }
}
