using Hotfix_LT.Data;
using LT.Hotfix.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI {
    public class LTPromotionTaskViewController : DynamicMonoHotfix {
        private UILabel _labCost;
        private GameObject _goPromotionBtn;
        private GameObject _goDoneTips;
        private GameObject _goItem;
        private GameObject _goRedPoint;
        private Transform _itemParent;
        private UIGrid _uiGrid;
        private bool _canPromoted;
        private List<LTPromotionTaskItemController> _itemControllerPool = new List<LTPromotionTaskItemController>();
        private List<TaskTemplate> _tasks;
        private LTPromotionUIController _promotionUIController;
        public override void Awake() {
            base.Awake();

            var t = mDMono.transform;
            _labCost = t.GetComponent<UILabel>("Lab_Cost");
            _goPromotionBtn = t.FindEx("Btn_Promotion").gameObject;
            _goDoneTips = t.FindEx("Lab_Done").gameObject;
            _goItem = t.FindEx("Scroll View/Grid/Item").gameObject;
            _goRedPoint = t.FindEx("Btn_Promotion/RedPoint").gameObject;
            _uiGrid = t.GetComponent<UIGrid>("Scroll View/Grid");
            _itemParent = _uiGrid.transform;
            _promotionUIController = t.parent.GetUIControllerILRComponent<LTPromotionUIController>();

            t.GetComponent<UIButton>("Btn_Promotion").onClick.Add(new EventDelegate(OnPromotionButtonClicked));
            t.GetComponent<UIButton>("Lab_Cost/Icon").onClick.Add(new EventDelegate(delegate{
                FusionAudio.PostEvent("UI/General/ButtonClick");
                UITooltipManager.Instance.DisplayTooltipSrc("2013", "Generic", "default");//来源;
            }));
        }

        public void Set() {
            ReachUpperLimit(false);
            var info = LTPromotionManager.Instance.GetPromotion();

            if (info == null) {
                EB.Debug.LogError("LTPromotionTaskViewController.Set -> info is null");
                return;
            }

            string[] taskIds = null;
            if (!string.IsNullOrEmpty(info.taskIds))taskIds =  info.taskIds.Split(',');         
            if (taskIds == null) {
                RefreshTaskList(null);
                ReachUpperLimit(true);
                return;
            }

            var list = new List<TaskTemplate>();
            var finishedList = new List<TaskTemplate>();
            
            for (var i = 0; i < taskIds.Length; i++) {
                var task = TaskTemplateManager.Instance.GetTask(taskIds[i]); 

                if (TaskUtility.IsTaskFinished(int.Parse(taskIds[i]))) {
                    finishedList.Add(task);
                } else {
                    list.Add(task);
                }
            }
            list.AddRange(finishedList);
            RefreshTaskList(list);
            var nextId = Data.CharacterTemplateManager.Instance.GetNextPromotionId(info.id);
            if (nextId == 0)
            {
                ReachUpperLimit(true);
                return;
            }
            RefreshPromotionCost();
            ShowRedPoint(taskIds);
        }

        private void ShowRedPoint(string[] taskIds) {
            _goRedPoint.SetActive(taskIds != null && TaskUtility.IsTasksFinished(_tasks) && _canPromoted);
        }

        private void RefreshTaskList(List<TaskTemplate> tasks) {
            _tasks = tasks;
            DeactivePool();

            if (tasks == null) {
                return;
            }

            for (var i = 0; i < tasks.Count; i++) {
                if (i < _itemControllerPool.Count) {
                    _itemControllerPool[i].Set(tasks[i]);
                    _itemControllerPool[i].mDMono.gameObject.SetActive(true);
                } else {
                    var go = UnityEngine.Object.Instantiate(_goItem, _itemParent);
                    go.SetActive(true);
                    var itemController = go.GetMonoILRComponent<LTPromotionTaskItemController>();
                    itemController.Set(tasks[i]);
                    _itemControllerPool.Add(itemController);
                }
            }

            _uiGrid.Reposition();
        }

        private void ReachUpperLimit(bool isReach) {
            _goDoneTips.SetActive(isReach);
            _goPromotionBtn.SetActive(!isReach);
            _labCost.gameObject.SetActive(!isReach);
        }

        private void DeactivePool() {
            for (var i = 0; i < _itemControllerPool.Count; i++) {
                _itemControllerPool[i].mDMono.gameObject.SetActive(false);
            }
        }

        private void RefreshPromotionCost() {
            var info = LTPromotionManager.Instance.GetPromotion();

            if (info == null) {
                EB.Debug.LogError("LTPromotionTaskViewController.RefreshPromotionCost -> info is null");
                return;
            }

            var count = GameItemUtil.GetInventoryItemNum(info.itemId);
            _labCost.text = LT.Hotfix.Utility.ColorUtility.FormatLeftSideColor(count, info.cost);
            _canPromoted = count >= info.cost;
        }

        private void OnPromotionButtonClicked() {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            if (LTPromotionManager.Instance.HasTrainingResult())
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_PROMOTION_UNSAVE"));
                return;
            }
            if (!TaskUtility.IsTasksFinished(_tasks)) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_TASK_UNFINISHED"));
                return;
            }

            if (!_canPromoted) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_ERROR_INSUFFICIENT_ITEMS"));
                return;
            }
            int prelevel = LTPromotionManager.Instance.GetPromotionLevel();
            PromotionTemplate info = LTPromotionManager.Instance.GetPromotion();
            int nextId = Data.CharacterTemplateManager.Instance.GetNextPromotionId(info.id);

            LTPromotionManager.Instance.Api.RequestPromotion(nextId, ht => {
                DataLookupsCache.Instance.CacheData("promotion", null);
                DataLookupsCache.Instance.CacheData(ht);
                DataLookupsCache.Instance.CacheData(string.Format("mainlands.pl.{0}.promoid", LoginManager.Instance.LocalUserId), nextId);
                _promotionUIController.Refresh(true);
                _promotionUIController.PlayUplevelFx(LTPromotionManager.Instance.GetPromotionLevel()>prelevel?1:0, info.additiveAttributeLevel);
                Hotfix_LT.Messenger.Raise<int, bool>(EventName.OnRefreshAllPowerChange, -1, true);
            });
        }
    }
}
