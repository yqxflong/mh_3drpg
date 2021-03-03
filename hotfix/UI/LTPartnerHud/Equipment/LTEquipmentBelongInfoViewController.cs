using UnityEngine;
using LT.Hotfix.Utility;
using System.Collections.Generic;
using _HotfixScripts.Utils;
using System.Collections;

namespace Hotfix_LT.UI {
    public class LTEquipmentBelongInfoViewController : UIControllerHotfix, IHotfixUpdate {
        private GameObject _goLeftPlaceholder;
        private GameObject _goRightPlaceholder;
        private GameObject _goScrollView;
        private GameObject _goPartnerItem;
        private GameObject _goItem;
        private UITable _uiTable;
        private LTPartnerListCellController _partnerCellController;
        private List<UILabel> _uiLabelPool = new List<UILabel>();
        private bool _isInit = false;

        public override void Awake() {
            base.Awake();
            Init();
        }

        public override bool ShowUIBlocker {
            get {
                return false;
            }
        }

        public override void SetMenuData(object param) {
            base.SetMenuData(param);
            Init();
            Show((int)param);
        }

        public override IEnumerator OnAddToStack() {
            yield return base.OnAddToStack();
            RegisterMonoUpdater();
        }
        public override IEnumerator OnRemoveFromStack() {
            ErasureMonoUpdater();
            DestroySelf();
            yield return base.OnRemoveFromStack();
        }

        public void Update() {
            if (!GuideNodeManager.IsGuide && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))) {
                if (!controller.GetComponentInChildren<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition)) {
                    controller.Close();
                }
            }
        }

        private void Init() {
            if (_isInit) {
                return;
            }

            var t = controller.transform;
            _goLeftPlaceholder = t.FindEx("Background/Left/Placeholder").gameObject;
            _goRightPlaceholder = t.FindEx("Background/Right/Placeholder").gameObject;
            _goScrollView = t.FindEx("Background/Left/Scroll View").gameObject;
            _goPartnerItem = t.FindEx("Background/Right/PartnerItem").gameObject;
            _uiTable = t.GetComponent<UITable>("Background/Left/Scroll View/Table");
            _goItem = t.FindEx("Background/Left/Scroll View/Table/Item").gameObject;
            _partnerCellController = t.GetMonoILRComponent<LTPartnerListCellController>("Background/Right/PartnerItem");
            _isInit = true;
            _uiLabelPool.Add(_goItem.GetComponent<UILabel>());
        }

        private void Show(int eid) {
            var partnerData = InventoryUtility.GetPartnerData(eid);
            var presetNames = InventoryUtility.GetAllEquipmentPresetNameContains(eid);
            var showPreset = presetNames != null && presetNames.Count > 0;
            var showPartner = partnerData != null;
            _goLeftPlaceholder.SetActive(!showPreset);
            _goScrollView.SetActive(showPreset);
            _goRightPlaceholder.SetActive(!showPartner);
            _goPartnerItem.SetActive(showPartner);
            _partnerCellController.Fill(partnerData);
            UpdatePresetNameList(presetNames);
        }

        private void UpdatePresetNameList(List<string> presetNames) {
            for (var i = 0; i < _uiLabelPool.Count; i++) {
                _uiLabelPool[i].gameObject.SetActive(false);
            }

            if (presetNames == null) {
                return;
            }

            for (var i = 0; i < presetNames.Count; i++) {
                UILabel lab;

                if (i < _uiLabelPool.Count) {
                    lab = _uiLabelPool[i];
                } else {
                    lab = Object.Instantiate(_goItem, _uiTable.transform).GetComponent<UILabel>();
                    _uiLabelPool.Add(lab);
                }

                lab.text = presetNames[i];
                lab.gameObject.SetActive(true);
            }

            _uiTable.Reposition();
        }

        public static bool CanShow(int eid) {
            var partnerData = InventoryUtility.GetPartnerData(eid);
            var presetNames = InventoryUtility.GetAllEquipmentPresetNameContains(eid);
            return partnerData != null || (presetNames != null && presetNames.Count > 0);
        }
    }
}
