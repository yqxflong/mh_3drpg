using Hotfix_LT.Data;
using System;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionTrainingButtonController : DynamicMonoHotfix {
        private UILabel _labTips;
        private UILabel _labCost;
        private UILabel _labName;
        private GameObject _goFreeTips;
        private GameObject _goCost;
        private Transform _tRedPoint;
        private ConsecutiveClickCoolTrigger _btn;
        private UISprite _icon;
        private UISprite _btnSprite;

        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _labTips = t.GetComponent<UILabel>();
            _labCost = t.GetComponent<UILabel>("Btn_Training/Cost");
            _labName = t.GetComponent<UILabel>("Btn_Training/Name");
            _goFreeTips = t.FindEx("Btn_Training/Tips").gameObject;
            _goCost = t.FindEx("Btn_Training/Cost").gameObject;
            _tRedPoint = t.FindEx("Btn_Training/RedPoint", false);
            _btn = t.GetComponent<ConsecutiveClickCoolTrigger>("Btn_Training");
            _icon = t.GetComponent<UISprite>("Btn_Training/Cost/Icon");
            _btnSprite = t.GetComponent<UISprite>("Btn_Training");
        }

        public void Set(int trainingId, Action act) {
            var trainingInfo = CharacterTemplateManager.Instance.GetPromotionTrainingInfo(trainingId);

            if (trainingInfo == null) {
                EB.Debug.LogError("LTPromotionTrainingButtonController.Set -> info is null");
                return;
            }

            var strs1 = trainingInfo.cost.Split(';');
            var usedTimes = LTPromotionManager.Instance.GetUsedTrainingTimes(trainingId);
            var index = usedTimes >= strs1.Length ? strs1.Length - 1 : usedTimes;

            if (index < 0) {
                index = 0;
            }
            var strs2 = strs1[index].Split(',');
            var iconName = BalanceResourceUtil.GetResSpriteName(strs2[0]);
            var cost = strs2[1];
            int.TryParse(cost, out int costNum);
            var isEnough = trainingId == 1 ? BalanceResourceUtil.GetUserGold() >= costNum : BalanceResourceUtil.GetUserDiamond() >= costNum;
            var coststrcolor = isEnough ? LT.Hotfix.Utility.ColorUtility.WhiteColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
            var remainingTimes = trainingInfo.count - usedTimes;
            var isFree = LTPromotionManager.Instance.IsTrainingFree(trainingId);

            if (trainingInfo.count > 0) {
                var colorStr = remainingTimes > 0 ? LT.Hotfix.Utility.ColorUtility.GreenColorHexadecimal : LT.Hotfix.Utility.ColorUtility.RedColorHexadecimal;
                _labTips.text = EB.StringUtil.Format(EB.Localizer.GetString("ID_codefont_in_LTUltimateTrialHudCtrl_11054"), EB.StringUtil.Format("[{0}]", colorStr), remainingTimes, trainingInfo.count);
            } else {
                _labTips.text = string.Empty;
            }

            _icon.spriteName = iconName;
            _btnSprite.spriteName = trainingId == 1 ? "Ty_Button_1" : "Ty_Button_3";
            _goFreeTips.SetActive(isFree);
            _goCost.SetActive(!isFree);
            _labCost.text = string.Format("[{0}]{1}[-]",coststrcolor,cost);
            _labName.text = trainingInfo.name;
            _btn.clickEvent.Clear();
            _btn.clickEvent.Add(new EventDelegate(() => act?.Invoke()));

            ShowRedPoint(trainingId);
        }

        private void ShowRedPoint(int trainingId) {
            if (_tRedPoint == null) {
                return;   
            }

            _tRedPoint.gameObject.SetActive(LTPromotionManager.Instance.HasUnfinishedAttr() && LTPromotionManager.Instance.IsTrainingFree(trainingId));
        }
    }
}
