using System;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public enum LegionWarTimeLine
    {
        BeforeQualifyGame = 0,
        QualifyGame,

        BeforeSemifinal = 2,
        SemiFinal,

        BeforeFinal = 4,
        Final,

        AfterFinal = 6,

        none
    }

    public class LTLegionWarTimeLine : DynamicMonoHotfix
    {
       

        public static LTLegionWarTimeLine Instance {
            get {
                InitTimeStamps();
                return _Instance;
            }
        }
    
        private static LTLegionWarTimeLine _Instance;
    
        public static LegionWarTimeLine TimeNow;
    
        public static int IntTimeNow {
            get {
                return (int)TimeNow;
            }
        }
    
        public long TimeNowBeginTime {
            get {
                return LegionWarTimeStamps[7];
            }
        }
    
        public long QualifyEndTime
        {
            get
            {
                return LegionWarTimeStamps[1];//预赛结束时间
            }
        }
        public long SemiFinalStopTime
        {
            get
            {
                return LegionWarTimeStamps[3];//半决报名结束时间
            }
        }
    
        public long SemiFinalEndTime
        {
            get
            {
                return LegionWarTimeStamps[4];//半决结束时间
            }
        }
        public long FinalStopTime
        {
            get
            {
                return LegionWarTimeStamps[6];//半决报名结束时间
            }
        }
    
        public long FinalEndTime
        {
            get
            {
                return LegionWarTimeStamps[7];//决赛结束时间
            }
        }
    
        private static DateTime Monday;
        private static List<long> LegionWarTimeStamps;
        
        public override  void Awake() {
            _Instance = this;
            Monday = GetWeekFirstDayMon();
            OnResetOpenTime();
        }
        
        public override void OnDestroy()
        {
            _Instance = null;
        }
    
        private int sequence=0;
        public void StartGetTimeNow()
        {
            sequence=ILRTimerManager.instance.AddTimer(1000,int.MaxValue , delegate { GetTimeNow(); });
        }
    
        public void StopGetTimeNow()
        {
            ILRTimerManager.instance.RemoveTimer(sequence);
            sequence = 0;
        }
    
        private static void InitTimeStamps() {
            LegionWarTimeStamps= new List<long>();
            for (var i = 0; i < LegionWarTimeData.LegionWarTimes.Count; i++)
            {
                var time = LegionWarTimeData.LegionWarTimes[i];
                DateTime date = Monday.AddDays((time.day<0)?(int)EB.Time.LocalNow.DayOfWeek:time.day);
                date = date.AddHours(time.hour);
                date = date.AddMinutes(time.minute);
                date = date.AddSeconds(time.second);
                LegionWarTimeStamps.Add(TaskSystem.GetTimeSpan(date));
            }
        }
    
        public void GetTimeNow() {
            InitTimeStamps();
            if(LTLegionWarManager.Instance.serveCurState >0)
            {
                switch (LTLegionWarManager.Instance.serveCurState)
                {
                    case 1: {
                            TimeNow= LegionWarTimeLine.QualifyGame;
                        }; break;
                    case 2: {
                            TimeNow = LegionWarTimeLine.SemiFinal;
                        }; break;
                    case 3: {
                            TimeNow = LegionWarTimeLine.Final;
                        }; break;
                    default: {
                            EB.Debug.LogError ( "LTLegionWarManager.Instance.serveCurState is error!");
                        } break;
                }
            }
            else TimeNow = CalculateTimeLinePoint();
        }
    
        private LegionWarTimeLine CalculateTimeLinePoint() {
            long now = EB.Time.Now;
            if (now < LegionWarTimeStamps[0])
                return LegionWarTimeLine.BeforeQualifyGame;
            else if (now >= LegionWarTimeStamps[0] && now < LegionWarTimeStamps[1])
                return LegionWarTimeLine.QualifyGame;
            else if (now >= LegionWarTimeStamps[1] && now < LegionWarTimeStamps[2])
                return LegionWarTimeLine.BeforeSemifinal;
            else if (now >= LegionWarTimeStamps[2] && now < LegionWarTimeStamps[4])
                return LegionWarTimeLine.SemiFinal;
            else if (now >= LegionWarTimeStamps[4] && now < LegionWarTimeStamps[5])
                return LegionWarTimeLine.BeforeFinal;
            else if (now >= LegionWarTimeStamps[5] && now < LegionWarTimeStamps[7])
                return LegionWarTimeLine.Final;
            else
                return LegionWarTimeLine.AfterFinal;
        }
    
        private DateTime GetWeekFirstDayMon()
        {
            DateTime datetime = TaskSystem.TimeSpanToDateTime(EB.Time.Now);
            int weeknow = (int)datetime.DayOfWeek;
            int daydiff = (-1) * weeknow;
            string FirstDay = datetime.AddDays(daydiff).ToString("yyyy-MM-dd");
            return Convert.ToDateTime(FirstDay+" 00:00:00");
        }
    
        public void OnResetOpenTime()
        {
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.QualifyOpenTime[0], 0);
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.QualifyOpenTime[1], 1);
    
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime[0], 2);
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime[2], 3);
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.SemiOpenTime[1], 4);
    
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime[0], 5);
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime[2], 6);
            OnOpenTimeSetting(LTLegionWarManager.Instance.WarOpenTime.FinalOpenTime[1], 7);
        }
    
        private void OnOpenTimeSetting(LTLegionWarTime data, int index)
        {
            LegionWarTimeData.LegionWarTimes[index] = data;
        }
    
    }
}
