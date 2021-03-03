using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTWelfareDiamondDayItem : DynamicMonoHotfix {
        private UIButton   _uiBtn;
        private UISprite   _bgSprite;
        private UILabel    _dayLabel;
        private GameObject _lockObj;
        private GameObject _redPointObj;

        private int  _index = -1;
        private bool _isLock = false;

        public override void Awake() {
            base.Awake();

            Transform t = mDMono.transform;
            _uiBtn = t.GetComponent<UIButton>();
            _uiBtn.onClick.Add(new EventDelegate(OnClick));
            _bgSprite = t.Find("BG").GetComponent<UISprite>();
            _dayLabel = t.Find("DayLabel").GetComponent<UILabel>();
            _lockObj = t.Find("LockObj").gameObject;
            _redPointObj = t.Find("RedPoint").gameObject;

            LTWelfareEvent.WelfareDiamondReset += InitData;
        }

        public override void OnDestroy() {
            LTWelfareEvent.WelfareDiamondReset -= InitData;
            base.OnDestroy();
        }

        public void InitData(int i) {
            _index = i;
            InitData();
        }

        private void InitData() {
            _isLock = _index > LTWelfareDiamondController.nextDayIndex;
            _dayLabel.text = string.Format(EB.Localizer.GetString("ID_DAY"), _index + 1);
            _lockObj.CustomSetActive(_isLock);
        }

        public void SetSelectColor(bool isSelect = false) {
            _bgSprite.color = isSelect ? new Color(1, 1, 1) : new Color(1, 0, 1);
        }

        private void OnClick() {
            if (_isLock) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_TARGET_LOCKED"));
                return;
            }

            if (LTWelfareEvent.WelfareDiamondDayItemClicked != null) {
                LTWelfareEvent.WelfareDiamondDayItemClicked(_index);
            }
        }
    }
}
