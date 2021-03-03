using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipmentPresetItem : DynamicMonoHotfix
    {
        private DynamicUISprite _icon;
        private UILabel _name;
        private UIButton _button;
        private UISprite _highlight;
        private LTPartnerEquipmentPresetController _equipmentPresetController;
        private LTPartnerEquipmentInfoController _equipmentInfoController;
        private GameObject _selectObj;
        private GameObject _defaultIcon;

        public KeyValuePair<string, EquipmentPresetModel> KVP
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否为当前装备Item
        /// </summary>
        public bool IsCurrentItem
        {
            get { return mDMono.gameObject.name.Equals("Current"); }
        }

        public override void Awake()
        {
            base.Awake();

            if (mDMono.ObjectParamList != null)
            {
                if (mDMono.ObjectParamList.Count > 0)
                {
                    _equipmentPresetController = (mDMono.ObjectParamList[0] as GameObject).GetMonoILRComponent<LTPartnerEquipmentPresetController>();
                }

                if (mDMono.ObjectParamList.Count > 1)
                {
                    _equipmentInfoController = (mDMono.ObjectParamList[1] as GameObject).GetMonoILRComponent<LTPartnerEquipmentInfoController>();
                }
            }

            _button = mDMono.transform.GetComponent<UIButton>();

            if (mDMono.gameObject.name.Equals("Add"))
            {
                _button.onClick.Add(new EventDelegate(OnClickAddButton));
            }
            else
            {
                _button.onClick.Add(new EventDelegate(OnItemClicked));
            }
        }

        private void OnClickAddButton()
        {
            if (LTPartnerEquipMainController.instance != null)
            {
                var presetName = _equipmentPresetController.GetDefaultPresetName();
                _equipmentInfoController.OnEnterEdit(presetName, null);
                LTPartnerEquipMainController.instance.OpenEquipmentPresetEditView();
            }
        }

        private void OnItemClicked()
        {
            _equipmentPresetController.SetHighlightFromItem(this);
        }

        public int[] SetCurrentItemKVP()
        {
            var infos = LTPartnerEquipMainController.CurrentPartnerData.EquipmentsInfo;
            var eids = new int[infos.Length];

            for (int i = 0; i < infos.Length; i++)
            {
                eids[i] = infos[i].Eid;
            }

            KVP = new KeyValuePair<string, EquipmentPresetModel>(EB.Localizer.GetString("ID_CURRENT_EQUIPMENT"), new EquipmentPresetModel(eids));
            return eids;
        }

        private void SetIcon(string spriteName)
        {
            if (_icon == null)
            {
                _icon = mDMono.transform.GetComponent<DynamicUISprite>("IconBg/Icon");
                _defaultIcon = mDMono.transform.FindEx("IconBg/DefaultIcon").gameObject;
            }

            if (_icon != null)
            {
                _icon.spriteName = spriteName;
                _defaultIcon.SetActive(string.IsNullOrEmpty(spriteName));
            }
        }

        public void SetName(string text)
        {
            if (_name == null)
            {
                _name = mDMono.transform.GetComponent<UILabel>("Name");
            }

            if (_name != null)
            {
                _name.text = text;
            }
        }

        public void SetEquipped(bool isEquipped)
        {
            if (_selectObj == null)
            {
                _selectObj = mDMono.transform.FindEx("SelectBg/Select").gameObject;
            }

            if (_selectObj != null)
            {
                _selectObj.SetActive(isEquipped);
            }

            if (isEquipped) {
                _equipmentPresetController.EquippedPresetItem = this;
            }
        }

        public void SetData(bool isEquipped, KeyValuePair<string, EquipmentPresetModel> kvp)
        {
            KVP = kvp;
            SetName(kvp.Key);
            SetEquipped(isEquipped); 
            SetHighlight(isEquipped);

            var attr = new HeroEquipmentTotleAttr();

            for (int i = 0; i < kvp.Value.eids.Length; i++)
            {
                int eid = kvp.Value.eids[i];

                if (eid != 0)
                {
                    DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);

                    if (info == null)
                    {
                        continue;
                    }

                    attr.AddSuitInfo(info);
                }
            }

            SuitAttrsSuitTypeAndCount data = null;
            int twoPieceSuitCount = 0;

            for (int i = 0; i < attr.SuitList.Count; i++)
            {
                if (data == null)
                {
                    data = attr.SuitList[i];
                }
                else if (attr.SuitList[i].count > data.count)
                {
                    data = attr.SuitList[i];
                }

                if (data.count >= 2) {
                    twoPieceSuitCount += 1;
                }
            }

            var suitTypeInfo = data != null ? Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(data.SuitType) : null;

            if (data != null && data.count >= 4 && suitTypeInfo != null && suitTypeInfo.SuitAttr4 != 0) {
                SetIcon(suitTypeInfo.SuitIcon);
            } else if (twoPieceSuitCount == 1 && data != null && data.count >= 2 && suitTypeInfo != null && suitTypeInfo.SuitAttr2 != 0) {
                SetIcon(suitTypeInfo.SuitIcon);  
            } else {
                SetIcon("Equipment_Icon_Taozhuang");
            }
        }

        public void SetHighlight(bool isShow)
        {
            if (_highlight == null)
            {
                _highlight = mDMono.transform.GetComponent<UISprite>("Highlight");
            }

            if (_highlight != null)
            {
                _highlight.alpha = isShow ? 1 : 0;
            }

            if (isShow)
            {
                int[] eids;

                // 当前装备
                if (IsCurrentItem)
                {
                    eids = SetCurrentItemKVP();
                }
                else
                {
                    eids = KVP.Value.eids;
                }

                _equipmentPresetController.CurrentEquipmentPresetItem = this;
                _equipmentInfoController.SetTitle(KVP.Key);
                _equipmentInfoController.Show(eids);
            }
        }
    }
}
