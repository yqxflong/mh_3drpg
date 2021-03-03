using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Hotfix_LT.UI {
    public class LTWelfareDiamondController : DynamicMonoHotfix {
        private UILabel _dayLabel;
        private UILabel _timeLabel;
        private UILabel _titleLabel;

        private GameObject _dayItem;
        private UIGrid _dayGrid;

        private GameObject _taskItem;
        private UIGrid _taskGrid;

        private List<LTWelfareDiamondDayItem> _dayList;
        private List<LTWelfareDiamondTaskItem> _tasksList;

        private WaitForSeconds _timer = new WaitForSeconds(1f);
        private long _timeFinal;

        private static int _currentDayIndex = -1;

        public static bool activityOver;
        public static int nextDayIndex = -1;

        public override void Awake() {
            base.Awake();

            Transform t = mDMono.transform;
            _dayLabel = t.Find("TaskView/TimeLabel/DayLabel").GetComponent<UILabel>();
            _timeLabel = t.Find("TaskView/TimeLabel/TimeLabel").GetComponent<UILabel>();
            _titleLabel = t.Find("TaskView/Lab_Title").GetComponent<UILabel>();
            _titleLabel.text = EB.Localizer.GetString("ID_DIAMOND_GIFT_TIPS");

            _dayItem = t.Find("DayView/Item").gameObject;
            _dayGrid = t.Find("DayView/ScrollView/Placeholder/Grid").GetComponent<UIGrid>();

            _taskItem = t.Find("TaskView/Item").gameObject;
            _taskGrid = t.Find("TaskView/ScrollView/Placeholder/Grid").GetComponent<UIGrid>();

            _dayList = new List<LTWelfareDiamondDayItem>();
            _tasksList = new List<LTWelfareDiamondTaskItem>();

            _timeFinal = 0;
            activityOver = false;
        }

        public override void OnDestroy() {
            _currentDayIndex = -1;
            LTWelfareEvent.WelfareDiamondDayItemClicked -= OnDayItemClicked;
            activityOver = true;
            EB.Coroutines.Stop(InitData());
            EB.Coroutines.Stop(ShowTimeLabel());
            base.OnDestroy();
        }

        public override void Start() {
            if (_timeFinal == 0) {
                long timeJoin = 0;
                DataLookupsCache.Instance.SearchDataByID<long>("user.time_join", out timeJoin);
                _timeFinal = Data.ZoneTimeDiff.GetFinishServerTime(timeJoin, 0, 7);
            }

            long ts = _timeFinal - EB.Time.Now;

            if (ts < 0) {
                ts = 0; 
            }

            int day = (int)(ts / (24 * 60 * 60));

            if (nextDayIndex != 7 - day) {
                nextDayIndex = 7 - day;
            }

            EB.Coroutines.Run(InitData());
            ResetRedPoint();
        }

        public override void OnEnable() {
            //base.OnEnable();
            ResetRedPoint();
        }

        IEnumerator InitData() {
            yield return null;
            InitDayList();
            EB.Coroutines.Run(ShowTimeLabel());
            LTWelfareEvent.WelfareDiamondDayItemClicked += OnDayItemClicked;
            yield break;
        }

        IEnumerator ShowTimeLabel() {
            while (!activityOver) {
                UpdataTimeFun();
                yield return _timer;
            }
        }

        public void UpdataTimeFun() {
            long ts = _timeFinal - EB.Time.Now;
            string colorStr = "";

            if (ts < 0) {
                activityOver = true;
                ts = 0;
                colorStr = "[ff6699]";
            }

            int day = (int)(ts / (24 * 60 * 60));

            if (nextDayIndex != 7 - day) {
                nextDayIndex = 7 - day;

                InitDayList();

                if (LTWelfareEvent.WelfareDiamondReset != null) {
                    LTWelfareEvent.WelfareDiamondReset();
                }
            }

            _dayLabel.text = string.Format("{0}{1}", colorStr, day);

            string timeStr = "";
            timeStr = (ts > 0) ? (string.Format("{0:D2}:{1:D2}:{2:D2}", (ts % (24 * 60 * 60)) / (60 * 60), (ts % (60 * 60)) / 60, ts % 60)) : "00:00:00";
            _timeLabel.text = string.Format(EB.Localizer.GetString("ID_DAY_FORMAT"), colorStr, timeStr);
        }

        private DateTime TimeSpanToDateTime(long span) {
            DateTime time = DateTime.MinValue;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            time = startTime.AddSeconds(span);
            return time;
        }

        private long GetTimeSpan(DateTime time) {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (long)(time - startTime).TotalSeconds;
        }

        private void InitDayList() {
            for (int i = 0; i < _dayList.Count; i++) {
                GameObject.Destroy(_dayList[i].mDMono.gameObject);
            }

            _dayList.Clear();
            int dayCount = Hotfix_LT.Data.EventTemplateManager.Instance.DiamondGiftDict.Keys.Count;

            for (int i = 0; i < dayCount; i++) {
                GameObject obj = GameObject.Instantiate(_dayItem);
                obj.CustomSetActive(true);
                obj.transform.SetParent(_dayGrid.transform);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;

                if (i < nextDayIndex) {
                    _currentDayIndex = i;
                }

                var item = obj.GetComponent<DynamicMonoILR>()._ilrObject as LTWelfareDiamondDayItem;
                item.InitData(i);
                _dayList.Add(item);
            }

            _dayGrid.enabled = true;
            _dayGrid.Reposition();
            _dayGrid.transform.parent.GetComponent<UIWidget>().height = (int)(dayCount * _dayGrid.cellHeight);

            if (_currentDayIndex == -1) { 
                _currentDayIndex = nextDayIndex - 1;
            }

            for (int i = 0; i < _dayList.Count; i++) {
                _dayList[i].SetSelectColor(i == _currentDayIndex);
            }

            int activeCount = (int)(_dayGrid.transform.parent.parent.GetComponent<UIPanel>().baseClipRegion.z / _dayGrid.cellHeight);
            float value = (float)(_currentDayIndex + 1 - activeCount) / (float)(dayCount - activeCount);
            float scrollValue = _currentDayIndex + 1 > activeCount ? value : 0f;

            Transform parent = _dayGrid.transform.parent.parent;
            parent.GetComponent<UIScrollView>().UpdatePosition();
            parent.GetComponent<UIScrollView>().UpdateScrollbars();
            parent.GetComponent<UIScrollView>().verticalScrollBar.value = scrollValue;

            RefreshTasksList(_currentDayIndex);
        }

        /// <param name="day">0~6</param>
        private void RefreshTasksList(int day) {
            var list = Hotfix_LT.Data.EventTemplateManager.Instance.DiamondGiftDict[day + 1];

            for (int i = 0; i < list.Count; i++) {
                if (i < _tasksList.Count) {
                    _tasksList[i].InitData(list[i]);
                    _tasksList[i].mDMono.gameObject.CustomSetActive(true);
                } else {
                    GameObject obj = GameObject.Instantiate(_taskItem);
                    obj.CustomSetActive(true);
                    obj.transform.SetParent(_taskGrid.transform);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;

                    var item = obj.GetComponent<DynamicMonoILR>()._ilrObject as LTWelfareDiamondTaskItem;
                    item.InitData(list[i]);
                    _tasksList.Add(item);
                }
            }

            for (int i = list.Count; i < _tasksList.Count; i++) {
                _tasksList[i].mDMono.gameObject.CustomSetActive(false);
            }

            _taskGrid.enabled = true;
            _taskGrid.Reposition();
            _taskGrid.transform.parent.GetComponent<UIWidget>().height = (int)(list.Count * _taskGrid.cellHeight);

            var scrollView = _taskGrid.transform.parent.parent.GetComponent<UIScrollView>();
            scrollView.UpdatePosition();
            scrollView.UpdateScrollbars();
        }

        private bool btnClick = false;

        private void OnDayItemClicked(int day) {
            if (_currentDayIndex == day || btnClick) {
                return;
            }

            btnClick = true;
            _currentDayIndex = day;

            for (int i = 0; i < _dayList.Count; i++) {
                _dayList[i].SetSelectColor(i == _currentDayIndex);
            }

            RefreshTasksList(day);
            btnClick = false;
        }

        private void ResetRedPoint() {
            long uid =LoginManager.Instance.LocalUserId.Value;
            string key = uid.ToString() + "HasOpenWelfareDiamondGiftToday";

            if (PlayerPrefs.GetInt(key) != nextDayIndex) {
                PlayerPrefs.SetInt(key, nextDayIndex);
                PlayerPrefs.Save();
            }

            LTWelfareModel.Instance.Welfare_DiamondGift();
        }
    }
}
