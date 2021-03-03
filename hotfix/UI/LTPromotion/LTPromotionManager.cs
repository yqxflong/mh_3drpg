using EB.Sparx;
using Hotfix_LT.Data;
using LT.Hotfix.Utility;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI {
    public class LTPromotionManager : ManagerUnit {
        private static LTPromotionManager _instance;

        public static LTPromotionManager Instance {
            get {
                return _instance = _instance ?? LTHotfixManager.GetManager<LTPromotionManager>();
            }
        }

        public LTPromotionApi Api {
            get; private set;
        }

        public override void Initialize(Config config) {
            Instance.Api = new LTPromotionApi();
            Hotfix_LT.Messenger.AddListener(EventName.AllRedPointDataRefresh, Instance.SetRedPointStatus);
        }

        public void SetRedPointStatus() {
            bool hasFreeTraining = HasUnfinishedAttr() && IsTrainingFree(1);
            bool canPromoted = false;
            var info = GetPromotion();

            if (info != null&& !string.IsNullOrEmpty(info.taskIds)) {
                var taskIds = info.taskIds.Split(',');  
                var IsTasksFinished = true;

                if (taskIds != null) {
                    for (var i = 0; i < taskIds.Length; i++) {
                        if (!TaskUtility.IsTaskFinished(int.Parse(taskIds[i]))) {
                            IsTasksFinished = false;
                            break;
                        }
                    }

                    var count = GameItemUtil.GetInventoryItemNum(info.itemId);
                    canPromoted = IsTasksFinished && count >= info.cost;
                }
            }
            
            LTRedPointSystem.Instance.SetRedPointNodeNum(RedPointConst.promotion, (hasFreeTraining || canPromoted) ? 1 : 0);
        }

        public int GetPromotionLevel() {
            int value;
            DataLookupsCache.Instance.SearchDataByID("promotion.level", out value);
            return value;
        }

        public int GetPromotionStar() {
            int value;
            DataLookupsCache.Instance.SearchDataByID("promotion.star", out value);
            return value;
        }

        public PromotionTemplate GetPromotion() {
            var level = GetPromotionLevel();
            var star = GetPromotionStar();
            return CharacterTemplateManager.Instance.GetPromotionInfo(level, star);
        }
        
        public PromotionTemplate GetOtherPromotion(int id)
        {
            return CharacterTemplateManager.Instance.GetPromotionInfo(id);
        }
        
        public int GetUsedTrainingTimes(int trainingId) {
            int value;
            DataLookupsCache.Instance.SearchDataByID(EB.StringUtil.Format("promotion.times.{0}.current", trainingId), out value);
            return value;
        }

        public void ResetUsedTrainingTimes(int trainingId) {
            DataLookupsCache.Instance.CacheData(EB.StringUtil.Format("promotion.times.{0}.current", trainingId), 0);
        }

        public bool IsTrainingFree(int trainingId) {
            bool value;
            DataLookupsCache.Instance.SearchDataByID(EB.StringUtil.Format("promotion.times.{0}.is_free", trainingId), out value);
            return value;
        }

        public void SetTrainingFree(int trainingId) {
            DataLookupsCache.Instance.CacheData(EB.StringUtil.Format("promotion.times.{0}.is_free", trainingId), true);
        }

        public int GetCurrentAttrLevel(int id) {
            int value;
            DataLookupsCache.Instance.SearchDataByID(EB.StringUtil.Format("promotion.attrs.{0}.level", id), out value);
            return value;
        }

        public bool GetTrainingAfterAttrLevel(int id, out int value) {
            return DataLookupsCache.Instance.SearchDataByID(EB.StringUtil.Format("promotion.trainingResult.attrs.{0}", id), out value);
        }

        public int GetTrainingId() {
            int value;
            DataLookupsCache.Instance.SearchDataByID("promotion.trainingResult.trainingId", out value);
            return value;
        }

        public bool HasTrainingResult() {
            Hashtable value;
            DataLookupsCache.Instance.SearchDataByID("promotion.trainingResult", out value);
            return value != null && value.Count > 0;
        }

        public bool HasFallingAttr() {
            var list = GetAttrList();

            for (var i = 0; i < list.Count; i++) {
                int value;

                if (GetTrainingAfterAttrLevel(list[i].id, out value) && value < GetCurrentAttrLevel(list[i].id)) {
                    return true;
                }
            }

            return false;
        }

        public List<PromotionAttributeLevelTemplate> GetAttrList() {
            return CharacterTemplateManager.Instance.GetPromotionAttributeLevelList(GetPromotionLevel());
        }

        public bool HasUnfinishedAttr() {
            var list = GetAttrList();
            var promotionInfo = GetPromotion();

            if (list == null || promotionInfo == null) {
                return false;
            }

            for (var i = 0; i < list.Count; i++) {
                var currentLevel = GetCurrentAttrLevel(list[i].id);
                var targetLevel = promotionInfo.attributeLevelUpperLimit;

                if (currentLevel < targetLevel) {
                    return true;
                }
            }

            return false;
        }

        public int GetTrainingCost(int trainingId) {
            var info = CharacterTemplateManager.Instance.GetPromotionTrainingInfo(trainingId);

            if (info == null) {
                EB.Debug.LogError("LTPromotionManager.GetCost -> info is null");
                return 0;
            }

            var strs1 = info.cost.Split(';');
            var usedTimes = GetUsedTrainingTimes(trainingId);
            var index = usedTimes >= strs1.Length ? strs1.Length - 1 : usedTimes;

            if (index < 0) {
                index = 0;
            }

            var strs2 = strs1[index].Split(',');
            var cost = int.Parse(strs2[1]);
            return cost;
        }

        public int GetTrainingTotalTimes(int trainingId) {
            var info = CharacterTemplateManager.Instance.GetPromotionTrainingInfo(trainingId);
            return info != null ? info.count : 0;
        }

        public bool IsPromotionFunctionEnabled() {
            var func = FuncTemplateManager.Instance.GetFunc(10100);

            if (func != null) {
                return func.IsConditionOK();
            }

            return false;
        }

        public bool HasPromotionBadgeInMainland() {
            int value;
            DataLookupsCache.Instance.SearchDataByID(EB.StringUtil.Format("mainlands.pl.{0}.promoid", LoginManager.Instance.LocalUserId.Value), out value);
            return value > 0;
        }

        public void SetPlayerPromotionBadge() {
            var promotion = GetPromotion();

            if (promotion != null) {
                DataLookupsCache.Instance.CacheData(EB.StringUtil.Format("mainlands.pl.{0}.promoid", LoginManager.Instance.LocalUserId.Value), promotion.id);
            } else {
                EB.Debug.LogError("LTPromotionManager.SetPlayerPromotionBadge -> promotion is null");
            }
        }
    }
}