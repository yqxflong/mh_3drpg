namespace Hotfix_LT.UI
{
    public class LTDailyData
    {
        public Hotfix_LT.Data.SpecialActivityTemplate ActivityData;
        public EDailyType DailyType;
        public bool IsSelected;
        public string OpenTimeStr;
        public string OpenTimeWeek;

        public int OpenTimeValue;
        public int StopTimeValue;
    }

    public class LTBounTaskData : INodeData
    {
        private int TotalHantTimes, HantTimes;
        public LTBounTaskData()
        {
            TotalHantTimes = 0;
            HantTimes = 0;
        }
        public void CleanUp()
        {
            TotalHantTimes = 0;
            HantTimes = 0;
        }

        public object Clone()
        {
            return new LTBounTaskData();
        }

        public void OnMerge(object obj)
        {
            OnUpdate(obj);
        }

        public void OnUpdate(object obj)
        {
            TotalHantTimes = EB.Dot.Integer("day_bounty_times", obj,0);
            int prehants = HantTimes;
            HantTimes = EB.Dot.Integer("bounty_times", obj, 0);
            if(prehants!=HantTimes && TotalHantTimes - HantTimes == 0)
            {
                LTDailyDataManager.Instance.SetDailyDataRefreshState();
            }

        }
    }
}