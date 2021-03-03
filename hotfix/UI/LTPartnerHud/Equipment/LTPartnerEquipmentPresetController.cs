using DocumentFormat.OpenXml.Drawing;
using LT.Hotfix.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public struct EquipmentPresetModel
    {
        public int ts;
        public int[] eids;

        public EquipmentPresetModel(Hashtable ht)
        {
            ts = Convert.ToInt32(ht["ts"]);

            eids = new int[6];
            eids[0] = Convert.ToInt32(ht["1"]);
            eids[1] = Convert.ToInt32(ht["2"]);
            eids[2] = Convert.ToInt32(ht["3"]);
            eids[3] = Convert.ToInt32(ht["4"]);
            eids[4] = Convert.ToInt32(ht["5"]);
            eids[5] = Convert.ToInt32(ht["6"]);
        }

        public EquipmentPresetModel(int[] eids)
        {
            ts = EB.Time.Now;
            this.eids = eids;
        }
    }

    public class LTPartnerEquipmentPresetController : DynamicMonoHotfix
    {
        public readonly int maxPresetCount = 20;

        private UILabel _labCount;
        private UILabel _labEquip;
        private UIButton _btnSave;
        private UIButton _btnDelete;
        private UIButton _btnEdit;
        private UIButton _btnEquip;
        private GameObject _currentItemObj;
        private GameObject _addItemObj;
        private UIGrid _uiGrid;
        private List<LTPartnerEquipmentPresetItem> _totalItems = new List<LTPartnerEquipmentPresetItem>();
        private List<LTPartnerEquipmentPresetItem> _activeItems = new List<LTPartnerEquipmentPresetItem>();
        private LTPartnerEquipmentInfoController _equipmentInfoController;
        private LTPartnerEquipmentPresetItem _currentObjItem;
        private LTPartnerEquipmentPresetItem _currentEquipmentPresetItem;
        private LTPartnerEquipmentPresetItem _equippedPresetItem;
        private UIScrollView _scrollView;

        public List<KeyValuePair<string, EquipmentPresetModel>> PresetList { get; } = new List<KeyValuePair<string, EquipmentPresetModel>>();
        public HashSet<string> PresetNameSet { get; } = new HashSet<string>(); 

        /// <summary>
        /// 当前选择的预设
        /// </summary>
        public LTPartnerEquipmentPresetItem CurrentEquipmentPresetItem 
        {
            get
            {
                return _currentEquipmentPresetItem;
            }

            set 
            {
                _currentEquipmentPresetItem = value;

                bool currentItemSelected = value.mDMono.gameObject.Equals(_currentItemObj);
                _btnSave.gameObject.SetActive(currentItemSelected);
                _btnDelete.gameObject.SetActive(!currentItemSelected);
                _btnEdit.gameObject.SetActive(!currentItemSelected);
                _btnEquip.gameObject.SetActive(!currentItemSelected); 

                if (EquippedPresetItem != null && value != EquippedPresetItem)
                {
                    _btnEquip.isEnabled = true;
                    _labEquip.text = EB.Localizer.GetString("ID_EQUIP_TIPS_LOAD");
                }
                else
                {
                    _btnEquip.isEnabled = false;
                    _labEquip.text = EB.Localizer.GetString("ID_EQUIPPED");
                }
            }
        }

        /// <summary>
        /// 已装备的预设
        /// </summary>
        public LTPartnerEquipmentPresetItem EquippedPresetItem
        {
            get { return _equippedPresetItem; }
            set 
            {
                _equippedPresetItem = value;

                if (_equippedPresetItem == CurrentEquipmentPresetItem)
                {
                    _btnEquip.isEnabled = false;
                    _labEquip.text = EB.Localizer.GetString("ID_EQUIPPED"); 
                }
            }
        }

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            _btnSave = t.GetComponent<UIButton>("Bg/Middle/BtnGroup/Btn_Save");
            _btnSave.onClick.Add(new EventDelegate(OnClickSaveButton));
            _btnDelete = t.GetComponent<UIButton>("Bg/Middle/BtnGroup/Btn_Delete");
            _btnDelete.onClick.Add(new EventDelegate(OnClickDeleteButton));
            _btnEdit = t.GetComponent<UIButton>("Bg/Middle/BtnGroup/Btn_Edit");
            _btnEdit.onClick.Add(new EventDelegate(OnClickEditButton));
            _btnEquip = t.GetComponent<UIButton>("Bg/Middle/BtnGroup/Btn_Equip");
            _btnEquip.onClick.Add(new EventDelegate(OnClickEquipButton));
            _labEquip = t.GetComponent<UILabel>("Bg/Middle/BtnGroup/Btn_Equip/Label");
            _labEquip.text = EB.Localizer.GetString("ID_EQUIP_TIPS_LOAD");
            _currentItemObj = t.FindEx("Bg/Middle/ScrollView/Grid/Current").gameObject;
            _addItemObj = t.FindEx("Bg/Middle/ScrollView/Grid/Add").gameObject;
            _uiGrid = t.GetComponent<UIGrid>("Bg/Middle/ScrollView/Grid");
            _equipmentInfoController = t.parent.FindEx("EquipmentInfos").GetMonoILRComponent<LTPartnerEquipmentInfoController>();
            _currentObjItem = _currentItemObj.GetMonoILRComponent<LTPartnerEquipmentPresetItem>();
            _scrollView = t.GetComponent<UIScrollView>("Bg/Middle/ScrollView");

            SetCount(PresetList.Count);
        }

        /// <summary>
        /// 预设列表中是否存在与当前装备数据一致的预设
        /// </summary>
        private bool HasSamePresetWithCurrentEquipment(List<KeyValuePair<string, EquipmentPresetModel>> list, out HashSet<string> presetNames)
        {
            var infos = LTPartnerEquipMainController.CurrentPartnerData.EquipmentsInfo;
            var names = new HashSet<string>();

            for (var i = 0; i < list.Count; i++)
            {
                var kvp = list[i];
                var hasSame = true;

                for (var j = 0; j < kvp.Value.eids.Length; j++)
                {
                    if (kvp.Value.eids[j] != infos[j].Eid)
                    {
                        hasSame = false;
                        break;
                    }
                }

                if (hasSame)
                {
                    names.Add(kvp.Key);
                }
            }

            presetNames = names;
            return names.Count > 0;
        }

        private void SetCount(int count)
        {
            if (_labCount == null)
            {
                _labCount = mDMono.transform.GetComponent<UILabel>("Bg/Middle/BtnGroup/Lab_Count");
            }

            if (_labCount != null)
            {
                _labCount.text = string.Format("[42fe79]{0}[-]/{1}", count, maxPresetCount);
            }
        }

        public string GetDefaultPresetName() 
        {
            var count = 1;
            var presetName = string.Format("{0}{1}", EB.Localizer.GetString("ID_SCHEME"), count);

            while (PresetNameSet.Contains(presetName)) 
            {
                count += 1;
                presetName = string.Format("{0}{1}", EB.Localizer.GetString("ID_SCHEME"), count);
            }

            return presetName;
        }

        private void OnClickSaveButton()
        {
            var presetName = GetDefaultPresetName();
            var ts = EB.Time.Now;
            SaveScheme(presetName, ts, CurrentEquipmentPresetItem.KVP.Value.eids);
        }

        public void SaveScheme(string presetName, int ts, int[] eids, string originPresetName = null)
        {
            if (string.IsNullOrEmpty(originPresetName) && PresetList.Count >= maxPresetCount)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIPMENT_PRESET_FULL"));
                return;
            }

            //编辑方案改名后，先将原方案删除再新增方案
            if (!string.IsNullOrEmpty(originPresetName) && !originPresetName.Equals(presetName) && PresetNameSet.Contains(originPresetName)) 
            {
                LTPartnerEquipDataManager.Instance.RequestDeleteEquipmentPreset(LoginManager.Instance.LocalUserId.Value, originPresetName, ht => {
                    OnAddScheme(presetName, ts, eids);
                });
            } 
            else 
            {
                OnAddScheme(presetName, ts, eids);
            }
        }

        private void OnAddScheme(string presetName, int ts, int[] eids) {
            var presetData = Johny.HashtablePool.Claim();
            presetData.Add("ts", ts);
            presetData.Add("1", eids[0]);
            presetData.Add("2", eids[1]);
            presetData.Add("3", eids[2]);
            presetData.Add("4", eids[3]);
            presetData.Add("5", eids[4]);
            presetData.Add("6", eids[5]);
            LTPartnerEquipDataManager.Instance.RequestAddEquipmentPreset(LoginManager.Instance.LocalUserId.Value, presetName, presetData, ht => {
                LTPartnerEquipDataManager.Instance.RequestGetEquipmentPresetList(LoginManager.Instance.LocalUserId.Value, result => {
                    Hashtable data = EB.Dot.Object("equipment_preset", result, null);
                    DataLookupsCache.Instance.CacheData("equipment_preset", null);
                    DataLookupsCache.Instance.CacheData("equipment_preset", data);
                    RefreshData(data);
                    SetHighlightFromName(presetName);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_SAVE_SUCCESSFULLY"));
                });
            });
        }

        private void OnClickDeleteButton()
        {
            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_DELETE_OR_NOT"), delegate (int r)
            {
                if (r == 0)
                {
                    LTPartnerEquipDataManager.Instance.RequestDeleteEquipmentPreset(LoginManager.Instance.LocalUserId.Value, CurrentEquipmentPresetItem.KVP.Key, ht => {
                        LTPartnerEquipDataManager.Instance.RequestGetEquipmentPresetList(LoginManager.Instance.LocalUserId.Value, result => {
                            Hashtable data = EB.Dot.Object("equipment_preset", result, null);
                            DataLookupsCache.Instance.CacheData("equipment_preset", null);
                            DataLookupsCache.Instance.CacheData("equipment_preset", data);
                            RefreshData(data);
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_DELETE_SUCCESS"));
                        });
                    });
                }
            });
        }

        private void OnClickEditButton()
        {
            if (LTPartnerEquipMainController.instance != null)
            {
                LTPartnerEquipMainController.instance.OpenEquipmentPresetEditView();
                _equipmentInfoController.OnEnterEdit(CurrentEquipmentPresetItem.KVP.Key, CurrentEquipmentPresetItem.KVP.Value.eids);
            }
        }

        private void OnClickEquipButton()
        {
            _canEquip = true;
            _index = -1;
            _unloadEquipmentCount = 0;
            OnEquipVerify();
        }

        bool _canEquip = true;
        int _index = -1;
        int _unloadEquipmentCount = 0;

        private void OnEquipVerify()
        {
            _index += 1;

            if (_index < CurrentEquipmentPresetItem.KVP.Value.eids.Length)
            {
                var eid = CurrentEquipmentPresetItem.KVP.Value.eids[_index];

                if (eid != 0 && _canEquip)
                {
                    if (InventoryUtility.IsEquipped(eid) && InventoryUtility.GetHeroId(eid) != LTPartnerEquipMainController.CurrentPartnerData.HeroId)
                    {
                        var content = EB.StringUtil.Format(EB.Localizer.GetString("ID_EQUIPMENT_PRESET_REPLACE_TIPS"), InventoryUtility.GetEquipmentName(eid), InventoryUtility.GetPartnerName(eid));
                        MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, content, delegate (int r)
                        {
                            if (r != 0)
                            {
                                _canEquip = false;
                            }

                            _unloadEquipmentCount += 1;
                            OnEquipVerify(); 
                        });
                    }
                    else
                    {
                        OnEquipVerify();
                    }
                }
                else
                {
                    OnEquipVerify();
                }
            }
            else
            {
                if (_canEquip)
                {
                    OnEquip();                   
                }
            }
        }

        private bool _isEquiping = false;

        private void OnEquip()
        {
            _unloadEquipmentCount += LTPartnerEquipMainController.CurrentPartnerData.EquipmentCount;

            if (LTPartnerEquipDataManager.Instance != null && LTPartnerEquipDataManager.Instance.isMaxEquipNumOneKey(_unloadEquipmentCount)) 
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_MailApi_1124"));
                return;
            }

            if (_isEquiping) 
            {
                return;
            }

            _isEquiping = true;

            var length = CurrentEquipmentPresetItem.KVP.Value.eids.Length;
            var kvps = new List<KeyValuePair<int, EquipPartType>>();

            // 把预设装备从其他伙伴身上卸下
            for (var i = 0; i < length; i++)
            {
                var eid = CurrentEquipmentPresetItem.KVP.Value.eids[i];

                if (eid != 0)
                {
                    kvps.Add(new KeyValuePair<int, EquipPartType>(eid, (EquipPartType)(i + 1)));
                    var heroId = InventoryUtility.GetHeroId(eid);

                    if (heroId != 0 && heroId != LTPartnerEquipMainController.CurrentPartnerData.HeroId)
                    {
                        LTPartnerEquipDataManager.Instance.RequireUnEquip(eid, heroId, null, false);
                    }
                }
            }
             
            // 卸下当前伙伴身上所有装备再重新上预设套装
            LTPartnerEquipDataManager.Instance.RequireUnEquipAll(LTPartnerEquipMainController.CurrentPartnerData.HeroId, delegate (bool success) {
                var count = kvps.Count;

                if (count < 1) {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_DRESS_SUCCESS"));
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 3, true);
                    EquippedPresetItem.SetEquipped(false);
                    CurrentEquipmentPresetItem.SetEquipped(true);
                    RefreshData();
                    _isEquiping = false;
                    return;
                }

                for (var i = 0; i < count; i++)
                {
                    var isUpdateEquipmentInfo = (i == count - 1);

                    LTPartnerEquipDataManager.Instance.RequireEquip(kvps[i].Key, LTPartnerEquipMainController.CurrentPartnerData.HeroId, kvps[i].Value, suc =>
                    {
                        if (isUpdateEquipmentInfo)
                        {
                            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_DRESS_SUCCESS"));  
                            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                            Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate, 3, true);
                            EquippedPresetItem.SetEquipped(false);
                            CurrentEquipmentPresetItem.SetEquipped(true);
                            RefreshData(); 
                            _isEquiping = false;
                        }
                    }, isUpdateEquipmentInfo);
                }
            });
        }

        private void EnableCurrentItem()
        {
            EquippedPresetItem = _currentObjItem;
            CurrentEquipmentPresetItem = _currentObjItem;
            _currentItemObj.SetActive(true);
            _currentObjItem.SetEquipped(true);
            _currentObjItem.SetHighlight(true);
            _currentObjItem.SetCurrentItemKVP();
            _currentObjItem.SetName(EB.Localizer.GetString("ID_CURRENT_EQUIPMENT"));
        }

        public void RefreshData(IDictionary dict = null)
        {
            if (dict == null)
            {
                DataLookupsCache.Instance.SearchDataByID("equipment_preset", out dict);
            }

            if (dict == null)
            {
                EnableCurrentItem();
                return;
            }
            
            PresetList.Clear();
            PresetNameSet.Clear();
            var enumerator = dict.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var presetName = enumerator.Key as string;
                PresetList.Add(new KeyValuePair<string, EquipmentPresetModel>(presetName, new EquipmentPresetModel(enumerator.Value as Hashtable)));
                PresetNameSet.Add(presetName);
            }

            PresetList.Sort((x, y) => { 
                if (x.Value.ts > y.Value.ts)
                {
                    return 1;
                }
                else if (x.Value.ts < y.Value.ts)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });

            RefreshList(PresetList);
        }

        private void RefreshList(List<KeyValuePair<string, EquipmentPresetModel>> list)
        {
            if (list == null)
            {
                return;
            }

            HashSet<string> samePresets;
            bool hasSame = HasSamePresetWithCurrentEquipment(list, out samePresets);
            var currentEquippedPresetName = string.Empty;
            var currentSelectedPresetName = string.Empty;
            var parent = _uiGrid.transform;
            _activeItems.Clear();

            if (CurrentEquipmentPresetItem != null) {
                currentSelectedPresetName = CurrentEquipmentPresetItem.KVP.Key;
            }

            if (hasSame) {
                if (EquippedPresetItem != null && samePresets.Contains(EquippedPresetItem.KVP.Key)) {
                    currentEquippedPresetName = EquippedPresetItem.KVP.Key;
                } else {
                    var enumerator = samePresets.GetEnumerator();

                    if (enumerator.MoveNext()) {
                        currentEquippedPresetName = enumerator.Current;
                    }
                }
            }

            for (var i = 0; i < _totalItems.Count; i++)
            {
                _totalItems[i].mDMono.gameObject.SetActive(false);
            }

            for (var i = 0; i < list.Count; i++)
            {
                LTPartnerEquipmentPresetItem item;

                if (_totalItems.Count > i)
                {
                    item = _totalItems[i];
                }
                else
                {
                    var go = GameObject.Instantiate(_currentItemObj, parent);
                    go.transform.SetSiblingIndex(parent.childCount - 2);
                    go.name = list[i].Value.ts.ToString();

                    item = go.GetMonoILRComponent<LTPartnerEquipmentPresetItem>();
                    _totalItems.Add(item);
                }

                item.SetData(list[i].Key.Equals(currentEquippedPresetName), list[i]);
                item.mDMono.gameObject.SetActive(true);
                _activeItems.Add(item);
            }

            if (hasSame)
            {
                _currentItemObj.SetActive(false);
            }
            else
            {
                EnableCurrentItem();
            }

            _addItemObj.SetActive(list.Count < maxPresetCount);
            _uiGrid.Reposition();  
            SetCount(list.Count);

            if (PresetNameSet.Contains(currentSelectedPresetName)) {
                SetHighlightFromName(currentSelectedPresetName); 
            }

            LocateToItem(CurrentEquipmentPresetItem);
        }

        public void LocateToItem(LTPartnerEquipmentPresetItem item) {
            LocateToItem(item.mDMono.transform);
        }

        public void LocateToItem(Transform item) {
            if (_scrollView != null && _scrollView.shouldMoveVertically) {
                var childs = new List<Transform>();

                for (var i = 0; i < _uiGrid.transform.childCount - 1; i++) {
                    var t = _uiGrid.transform.GetChild(i);

                    if (t.gameObject.activeSelf) {
                        childs.Add(t);
                    }
                }

                var rowCount = Mathf.CeilToInt(childs.Count / 2f);
                var index = childs.IndexOf(item);
                var y = ((float)(index / 2)) / (rowCount - 1);
                _scrollView.SetDragAmount(0, y, false);
            }
        }

        public void SetHighlightFromName(string presetName)
        {
            _currentObjItem.SetHighlight(_currentItemObj.name.Equals(presetName));

            for (var i = 0; i < _activeItems.Count; i++)
            {
                var item = _activeItems[i];
                item.SetHighlight(item.KVP.Key.Equals(presetName));
            }
        }

        public void SetHighlightFromItem(LTPartnerEquipmentPresetItem item)
        {
            for (var i = 0; i < _activeItems.Count; i++)
            {
                _activeItems[i].SetHighlight(_activeItems[i] == item);
            }

            _currentObjItem.SetHighlight(_currentObjItem == item);
        }
    }
}
