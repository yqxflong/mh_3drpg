using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipmentInfoController : DynamicMonoHotfix
    {
        public static bool isSuitTextShow = false;
        public static int isSuitTypeShow = -1;
        public static bool isChangeColor = false;
        public static LTPartnerEquipmentInfoController instance;

        public readonly int maxByteCount = 8;
        public readonly int maxEquipmentCount = 6;

        private LTPartnerEquipSuitCellController[] _leftEquipSuitCells;
        private LTPartnerEquipSuitCellController[] _rightEquipSuitCells;
        private LTPartnerEquipInfoCell[] _equipInfoCells;
        private UILabel[] _currentAttrValues;
        private UILabel[] _originAttrValues;
        private double[] _oldAttrValues;
        private double[] _equppedAttrValues;
        private UITweener[] _rightSuitFxTweeners;
        private UILabel _labTitle;
        private UIInput _input;
        private LTPartnerEquipmentPresetController _equipmentPresetController;
        private GameObject _arrowObj;
        private GameObject _editObj;
        private Transform _equipSuitLeft;
        private TweenPosition _tpEquipSuitRight;
        private bool _showCompareInfo;
        private bool _isEditing;

        public HeroEquipmentTotleAttr CurrentEquipmentTotalAttr
        {
            get; private set;
        }

        public int[] Eids
        {
            get; private set;
        }

        public override void Awake()
        {
            base.Awake();
            instance = this;

            var t = mDMono.transform;
            _input = t.GetComponent<UIInput>("Lab_Title/Edit/Btn_Edit");
            _input.onChange.Add(new EventDelegate(OnInputChange));
            _equipmentPresetController = t.parent.FindEx("EquipmentPreset").GetMonoILRComponent<LTPartnerEquipmentPresetController>();
            _arrowObj = t.FindEx("Arrow").gameObject;
            _editObj = t.FindEx("Lab_Title/Edit").gameObject;
            _editObj.SetActive(false);
            _equipSuitLeft = t.FindEx("EquipSuit_Left");
            _tpEquipSuitRight = t.GetComponent<TweenPosition>("EquipSuit_Right");

            _leftEquipSuitCells = new LTPartnerEquipSuitCellController[6];
            _leftEquipSuitCells[0] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Left/Container/EquipmentIcon");
            _leftEquipSuitCells[1] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Left/Container/EquipmentIcon (1)");
            _leftEquipSuitCells[2] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Left/Container/EquipmentIcon (2)");
            _leftEquipSuitCells[3] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Left/Container/EquipmentIcon (3)");
            _leftEquipSuitCells[4] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Left/Container/EquipmentIcon (4)");
            _leftEquipSuitCells[5] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Left/Container/EquipmentIcon (5)");

            _rightEquipSuitCells = new LTPartnerEquipSuitCellController[6];
            _rightEquipSuitCells[0] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Right/Container/EquipmentIcon");
            _rightEquipSuitCells[1] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Right/Container/EquipmentIcon (1)");
            _rightEquipSuitCells[2] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Right/Container/EquipmentIcon (2)");
            _rightEquipSuitCells[3] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Right/Container/EquipmentIcon (3)");
            _rightEquipSuitCells[4] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Right/Container/EquipmentIcon (4)");
            _rightEquipSuitCells[5] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("EquipSuit_Right/Container/EquipmentIcon (5)");

            _equipInfoCells = new LTPartnerEquipInfoCell[6];
            _equipInfoCells[0] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("Equip/item");
            _equipInfoCells[1] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("Equip/item (1)");
            _equipInfoCells[2] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("Equip/item (2)");
            _equipInfoCells[3] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("Equip/item (3)");
            _equipInfoCells[4] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("Equip/item (4)");
            _equipInfoCells[5] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("Equip/item (5)");

            _currentAttrValues = new UILabel[11];
            _currentAttrValues[0] = t.GetComponent<UILabel>("Attr/Bg/Current");
            _currentAttrValues[1] = t.GetComponent<UILabel>("Attr/Bg (1)/Current");
            _currentAttrValues[2] = t.GetComponent<UILabel>("Attr/Bg (2)/Current");
            _currentAttrValues[3] = t.GetComponent<UILabel>("Attr/Bg (3)/Current");
            _currentAttrValues[4] = t.GetComponent<UILabel>("Attr/Bg (4)/Current");
            _currentAttrValues[5] = t.GetComponent<UILabel>("Attr/Bg (5)/Current");
            _currentAttrValues[6] = t.GetComponent<UILabel>("Attr/Bg (6)/Current");
            _currentAttrValues[7] = t.GetComponent<UILabel>("Attr/Bg (7)/Current");
            _currentAttrValues[8] = t.GetComponent<UILabel>("Attr/Bg (8)/Current");
            _currentAttrValues[9] = t.GetComponent<UILabel>("Attr/Bg (9)/Current");
            _currentAttrValues[10] = t.GetComponent<UILabel>("Attr/Bg (10)/Current");

            _originAttrValues = new UILabel[11];
            _originAttrValues[0] = t.GetComponent<UILabel>("Attr/Bg/Origin");
            _originAttrValues[1] = t.GetComponent<UILabel>("Attr/Bg (1)/Origin");
            _originAttrValues[2] = t.GetComponent<UILabel>("Attr/Bg (2)/Origin");
            _originAttrValues[3] = t.GetComponent<UILabel>("Attr/Bg (3)/Origin");
            _originAttrValues[4] = t.GetComponent<UILabel>("Attr/Bg (4)/Origin");
            _originAttrValues[5] = t.GetComponent<UILabel>("Attr/Bg (5)/Origin");
            _originAttrValues[6] = t.GetComponent<UILabel>("Attr/Bg (6)/Origin");
            _originAttrValues[7] = t.GetComponent<UILabel>("Attr/Bg (7)/Origin");
            _originAttrValues[8] = t.GetComponent<UILabel>("Attr/Bg (8)/Origin");
            _originAttrValues[9] = t.GetComponent<UILabel>("Attr/Bg (9)/Origin");
            _originAttrValues[10] = t.GetComponent<UILabel>("Attr/Bg (10)/Origin");

            _oldAttrValues = new double[11];
            _equppedAttrValues = new double[11];
            _rightSuitFxTweeners = t.GetComponent<Transform>("EquipSuit_Right/Label").GetComponents<UITweener>();

            isSuitTextShow = false;
            isSuitTypeShow = -1;
            isChangeColor = false;
        }

        public override void OnEnable()
        {
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnEquipChangeFunc);
        }

        public override void OnDisable()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnEquipChangeFunc);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        private void OnEquipChangeFunc()
        {
            for (int i = 0; i < maxEquipmentCount; i++)
            {
                _equipInfoCells[i].tweenScaleAni.ResetToBeginning();
                _equipInfoCells[i].tweenScaleAni.PlayForward();
            }
        }

        private void OnInputChange()
        {
            LTPartnerEquipMainController.instance.EnableSaveSchemeBtn(CanSaveInEditMode);
            var chineseLetterCount = GetChineseLetterCount(_input.value);
            var otherLetterCount = _input.value.Length - chineseLetterCount;
            var byteCount = (chineseLetterCount * 2) + otherLetterCount;

            if (byteCount > maxByteCount)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.StringUtil.Format(EB.Localizer.GetString("ID_NO_MORE_THAN_CHARACTERS"), maxByteCount));
                _input.value = _input.value.Substring(0, _input.value.Length - 1);
            }
        }

        private int GetChineseLetterCount(string str)
        {
            var count = 0;
            System.CharEnumerator charEnumerator = str.GetEnumerator();
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[\u4E00-\u9FA5]{0,}$");

            while (charEnumerator.MoveNext()) 
            {
                if (regex.IsMatch(charEnumerator.Current.ToString(), 0)) 
                {
                    count += 1;
                }
            }

            return count;
        }

        private string _originPresetName;
        private bool _isFromAddScheme;  // true-来自新增方案；false-来自现有预设

        public void OnEnterEdit(string name, int[] eids)
        {
            _isFromAddScheme = eids == null;
            _originPresetName = _labTitle.text;
            _isEditing = true;
            _input.defaultText = name;
            _input.value = null;
            _editObj.SetActive(true);
            SetTitle(name);
            Show(eids);
            LTPartnerEquipMainController.instance.EnableSaveSchemeBtn(CanSaveInEditMode);
        }

        public bool CanSaveInEditMode
        {
            get
            {
                if (_isFromAddScheme)
                {
                    return true;
                }

                bool hasChange = false;

                for (var i = 0; i < maxEquipmentCount; i++)
                {
                    if (Eids[i] != _equipmentPresetController.CurrentEquipmentPresetItem.KVP.Value.eids[i])
                    {
                        hasChange = true;
                        break;
                    }
                }

                return hasChange || !string.IsNullOrEmpty(_input.value);
            }
        }

        public void OnQuitEdit()
        {
            if (!CanSaveInEditMode)
            {
                QuitAndRestore();
                return;
            }

            MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_UNSAVED_TIPS"), delegate (int r)
            {
                if (r == 0)
                {
                    QuitAndRestore();
                }
            });
        }

        private void QuitAndRestore()
        {
            SetTitle(_originPresetName);
            _isEditing = false; 
            _input.value = null;
            _editObj.SetActive(false);
            _originPresetName = null;
            _isFromAddScheme = false;

            if (LTPartnerEquipMainController.instance != null)
            {
                LTPartnerEquipMainController.instance.CloseEquipmentPresetEditView();
            }

            Show(_equipmentPresetController.CurrentEquipmentPresetItem.KVP.Value.eids);
        }

        public void OnSaveEdit()
        {           
            var presetName = string.IsNullOrEmpty(_input.value) ? _labTitle.text : _input.value;

            if (presetName.IndexOf(" ") >= 0) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_CONTAINS_SPACE"));
                return;
            }

            if (presetName.IndexOf("\n") >= 0) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_INPUT_CONTAINS_NEWLINE"));
                return;
            }

            if (!EB.ProfanityFilter.Test(presetName) || !ChangeNicknameController.IsNormalName(presetName)) {
                MessageDialog.Show(EB.Localizer.GetString("ID_MESSAGE_TITLE_STR"), EB.Localizer.GetString("ID_NAME_ILLEGEL"), EB.Localizer.GetString("ID_MESSAGE_BUTTON_STR"), null, false, true, true, null, NGUIText.Alignment.Center);
                return;
            }

            //重名检查
            if (!presetName.Equals(_originPresetName) && _equipmentPresetController.PresetNameSet.Contains(presetName))
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.MessageDialogue_4, EB.Localizer.GetString("ID_EQUIPMENT_PRESET_DUPLICATE_NAME_TIPS"), delegate (int r) {
                    if (r == 0) 
                    {
                        Save(presetName, _originPresetName);
                    }
                }); 
            }
            else
            {
                Save(presetName, _originPresetName); 
            }
        }

        private void Save(string presetName, string originPresetName) {
            _isEditing = false;
            _editObj.SetActive(false);

            if (LTPartnerEquipMainController.instance != null) {
                LTPartnerEquipMainController.instance.CloseEquipmentPresetEditView();
            }

            var ts = EB.Time.Now;

            if (!_isFromAddScheme) 
            {
                for (var i = 0; i < _equipmentPresetController.PresetList.Count; i++) 
                {
                    var kvp = _equipmentPresetController.PresetList[i];

                    if (kvp.Key.Equals(originPresetName)) 
                    {
                        ts = kvp.Value.ts;
                        break;
                    }
                }
            }  

            _equipmentPresetController.SaveScheme(presetName, ts, Eids, _isFromAddScheme ? null : originPresetName);
        }

        public void RemoveFromEquipmentInfoList(int pos)
        {
            Eids[pos - 1] = 0;
            Show(Eids);
            LTPartnerEquipMainController.instance.EnableSaveSchemeBtn(CanSaveInEditMode);
            MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTEquipmentInforUIController_4512"));
        }

        public void AddToEquipmentInfoList(int pos, int eid)
        {
            if (Eids[pos - 1] != 0) {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_REPLACE_SUCCESSFULLY"));
            } else {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_SUCCESSFULLY"));
            }

            Eids[pos - 1] = eid;
            Show(Eids);
            LTPartnerEquipMainController.instance.EnableSaveSchemeBtn(CanSaveInEditMode);
        }

        public void SetTitle(string text)
        {
            if (_labTitle == null)
            {
                _labTitle = mDMono.transform.GetComponent<UILabel>("Lab_Title");
            }

            if (_labTitle != null)
            {
                _labTitle.text = text;
            }
        }

        public void Show(HeroEquipmentInfo[] infos)
        {
            if (infos == null)
            {
                return;
            }

            Eids = new int[infos.Length];

            for (int i = 0; i < infos.Length; i++)
            {
                Eids[i] = infos[i].Eid;
            }

            Refresh();
        }

        public void Show(int[] eids)
        {
            Eids = new int[maxEquipmentCount];

            if (eids != null)
            {
                eids.CopyTo(Eids, 0);
            }

            Refresh();
        }

        public void Refresh()
        {
            _showCompareInfo = !_isEditing && _equipmentPresetController.EquippedPresetItem != null && _equipmentPresetController.EquippedPresetItem != _equipmentPresetController.CurrentEquipmentPresetItem;           
            CurrentEquipmentTotalAttr = new HeroEquipmentTotleAttr();

            if (_showCompareInfo) {
                ShowEquippedInfo(_equipmentPresetController.EquippedPresetItem.KVP.Value.eids);
            }

            //为了显示套装特效，这里需提前把数据归拢
            for (int i = 0; i < Eids.Length; i++)
            {
                int eid = Eids[i];

                if (eid != 0)
                {
                    DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);

                    if (info != null)
                    {
                        CurrentEquipmentTotalAttr.AddSuitInfo(info);
                    }
                }
            }

            for (int i = 0; i < Eids.Length; i++)
            {
                int eid = Eids[i];

                if (eid == 0)
                {
                    _equipInfoCells[i].Fill(null);
                }
                else
                {
                    DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);

                    if (info == null)
                    {
                        EB.Debug.LogError("LTPartnerEquipmentInfoController.Show info is null, Eid = {0}", eid);
                        continue;
                    }

                    _equipInfoCells[i].Fill(info); 
                }
            }

            TypeSelect();

            if (mDMono.gameObject.activeSelf)
            {
                StartCoroutine(ChangeCurrentAttrColor(isChangeColor));
            }

            for (int i = 0; i < CurrentEquipmentTotalAttr.SuitList.Count; i++)
            {
                Data.SuitTypeInfo info = Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(CurrentEquipmentTotalAttr.SuitList[i].SuitType);

                if (CurrentEquipmentTotalAttr.SuitList[i].SuitType == isSuitTypeShow)
                {
                    if (CurrentEquipmentTotalAttr.SuitList[i].count == 4 && isSuitTextShow && info.SuitAttr4 != 0 || CurrentEquipmentTotalAttr.SuitList[i].count == 2 && isSuitTextShow && info.SuitAttr2 != 0)
                    {
                        for (int j = 0; j < _rightSuitFxTweeners.Length; ++j)
                        {
                            _rightSuitFxTweeners[j].tweenFactor = 0;
                            _rightSuitFxTweeners[j].PlayForward();
                        }

                        StartCoroutine(PlayShowSuitItem());
                    }
                }
            }

            ShowCurrentSuitView(CurrentEquipmentTotalAttr.SuitList);
            isSuitTextShow = false;
            isChangeColor = false;
            ShowCompareInfo(_showCompareInfo);
        }

        public void TypeSelect()
        {
            for (int i = 0; i < maxEquipmentCount; i++)
            {
                if (((int)LTPartnerEquipDataManager.Instance.CurType - 1) == i)
                {
                    _equipInfoCells[i].SelectBG.gameObject.SetActive(true);
                }
                else
                {
                    _equipInfoCells[i].SelectBG.gameObject.SetActive(false);
                }
            }
        }

        IEnumerator PlayShowSuitItem()
        {
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < _equipInfoCells.Length; i++)
            {
                if (_equipInfoCells[i].Data != null && _equipInfoCells[i].Data.SuitType == isSuitTypeShow)
                {
                    yield return new WaitForSeconds(0.1f);
                    _equipInfoCells[i].mDMono.GetComponent<TweenScale>().ResetToBeginning();
                    _equipInfoCells[i].mDMono.GetComponent<TweenScale>().PlayForward();
                }
            }
        }

        IEnumerator ChangeCurrentAttrColor(bool isChange = false)
        {
            if (isChange)
            {
                float timers = _currentAttrValues[0].GetComponent<TweenScale>().duration;
                ShowCurrentColorAttr();
                yield return new WaitForSeconds(timers);
                yield return StartCoroutine(ChangeCurrentAttrColor());
            }
            else
            {
                ShowCurrentPresetAttr();
            }
        }

        public void ShowCurrentColorAttr()
        {
            string plusStr = "+";
            string redStr = "[ff6699]+";
            string greenStr = "[42fe79]+";

            //0攻击
            double temp = CurrentEquipmentTotalAttr.ATK;
            double value = temp - _oldAttrValues[0];
            string colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[0].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[0].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[0] = temp;

            //1暴击
            temp = (CurrentEquipmentTotalAttr.CritP + CurrentEquipmentTotalAttr.SuitAttr.CritP) * 100;
            value = temp - _oldAttrValues[1];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[1].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[1].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[1] = temp;

            //2攻击加成
            temp = (CurrentEquipmentTotalAttr.ATKrate + CurrentEquipmentTotalAttr.SuitAttr.ATKrate) * 100;
            value = temp - _oldAttrValues[2];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[2].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[2].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[2] = temp;

            //3暴击伤害
            temp = (CurrentEquipmentTotalAttr.CritV + CurrentEquipmentTotalAttr.SuitAttr.CritV) * 100;
            value = temp - _oldAttrValues[3];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[3].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[3].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[3] = temp;

            //4生命
            temp = CurrentEquipmentTotalAttr.MaxHP;
            value = temp - _oldAttrValues[4];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[4].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[4].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[4] = temp;

            //5效果命中
            temp = (CurrentEquipmentTotalAttr.SpExtra + CurrentEquipmentTotalAttr.SuitAttr.SpExtra) * 100;
            value = temp - _oldAttrValues[5];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[5].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[5].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[5] = temp;

            //6生命加成
            temp = (CurrentEquipmentTotalAttr.MaxHPrate + CurrentEquipmentTotalAttr.SuitAttr.MaxHPrate) * 100;
            value = temp - _oldAttrValues[6];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[6].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[6].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[6] = temp;

            //7效果抵抗
            temp = (CurrentEquipmentTotalAttr.SpRes + CurrentEquipmentTotalAttr.SuitAttr.SpRes) * 100;
            value = temp - _oldAttrValues[7];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[7].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[7].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[7] = temp;

            //8防御
            temp = CurrentEquipmentTotalAttr.DEF;
            value = temp - _oldAttrValues[8];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[8].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[8].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[8] = temp;

            //9速度
            temp = (CurrentEquipmentTotalAttr.Speedrate + CurrentEquipmentTotalAttr.SuitAttr.Speedrate) * 100;
            value = temp - _oldAttrValues[9];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[9].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[9].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[9] = temp;

            //防御加成
            temp = (CurrentEquipmentTotalAttr.DEFrate + CurrentEquipmentTotalAttr.SuitAttr.DEFrate) * 100;
            value = temp - _oldAttrValues[10];
            colorStr = value == 0 ? plusStr : (value < 0 ? redStr : greenStr);

            if (value != 0)
            {
                var ts = _currentAttrValues[10].GetComponent<TweenScale>();
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            _currentAttrValues[10].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)temp));
            _oldAttrValues[10] = temp;
        }

        public void ShowCurrentPresetAttr()
        {
            string plusStr = "+";

            //0攻击
            double temp = CurrentEquipmentTotalAttr.ATK;
            _currentAttrValues[0].text = EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[0].color = Color.white;
            _oldAttrValues[0] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[0].color = temp > _equppedAttrValues[0] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[0] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //1暴击
            temp = (CurrentEquipmentTotalAttr.CritP + CurrentEquipmentTotalAttr.SuitAttr.CritP) * 100;
            _currentAttrValues[1].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[1].color = Color.white;
            _oldAttrValues[1] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[1].color = temp > _equppedAttrValues[1] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[1] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //2攻击加成
            temp = (CurrentEquipmentTotalAttr.ATKrate + CurrentEquipmentTotalAttr.SuitAttr.ATKrate) * 100;
            _currentAttrValues[2].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[2].color = Color.white;
            _oldAttrValues[2] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[2].color = temp > _equppedAttrValues[2] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[2] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //3暴击伤害
            temp = (CurrentEquipmentTotalAttr.CritV + CurrentEquipmentTotalAttr.SuitAttr.CritV) * 100;
            _currentAttrValues[3].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[3].color = Color.white;
            _oldAttrValues[3] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[3].color = temp > _equppedAttrValues[3] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[3] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //4生命
            temp = CurrentEquipmentTotalAttr.MaxHP;
            _currentAttrValues[4].text = EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[4].color = Color.white;
            _oldAttrValues[4] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[4].color = temp > _equppedAttrValues[4] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[4] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //5效果命中
            temp = (CurrentEquipmentTotalAttr.SpExtra + CurrentEquipmentTotalAttr.SuitAttr.SpExtra) * 100;
            _currentAttrValues[5].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[5].color = Color.white;
            _oldAttrValues[5] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[5].color = temp > _equppedAttrValues[5] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[5] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //6生命加成
            temp = (CurrentEquipmentTotalAttr.MaxHPrate + CurrentEquipmentTotalAttr.SuitAttr.MaxHPrate) * 100;
            _currentAttrValues[6].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[6].color = Color.white;
            _oldAttrValues[6] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[6].color = temp > _equppedAttrValues[6] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[6] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //7效果抵抗
            temp = (CurrentEquipmentTotalAttr.SpRes + CurrentEquipmentTotalAttr.SuitAttr.SpRes) * 100;
            _currentAttrValues[7].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[7].color = Color.white;
            _oldAttrValues[7] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[7].color = temp > _equppedAttrValues[7] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[7] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //8防御
            temp = CurrentEquipmentTotalAttr.DEF;
            _currentAttrValues[8].text = EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[8].color = Color.white;
            _oldAttrValues[8] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[8].color = temp > _equppedAttrValues[8] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[8] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //9速度
            temp = (CurrentEquipmentTotalAttr.Speedrate + CurrentEquipmentTotalAttr.SuitAttr.Speedrate) * 100;
            _currentAttrValues[9].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[9].color = Color.white;
            _oldAttrValues[9] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[9].color = temp > _equppedAttrValues[9] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[9] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }

            //10防御加成
            temp = (CurrentEquipmentTotalAttr.DEFrate + CurrentEquipmentTotalAttr.SuitAttr.DEFrate) * 100;
            _currentAttrValues[10].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _currentAttrValues[10].color = Color.white;
            _oldAttrValues[10] = temp;

            if (_showCompareInfo)
            {
                _currentAttrValues[10].color = temp > _equppedAttrValues[10] ? LT.Hotfix.Utility.ColorUtility.GreenColor : (temp < _equppedAttrValues[10] ? LT.Hotfix.Utility.ColorUtility.RedColor : Color.white);
            }
        }

        private void ShowCurrentSuitView(List<SuitAttrsSuitTypeAndCount> suitList)
        {
            List<SuitAttrsSuitTypeAndCount> mList = suitList;

            mList.Sort((a, b) =>
            {
                if (a.count > b.count)
                    return -1;
                else if (a.count < b.count)
                    return 1;
                else if (a.SuitType > b.SuitType)
                    return -1;
                else
                    return 1;
            });

            for (int i = 0; i < _rightEquipSuitCells.Length; i++)//套装
            {
                if (i < mList.Count)
                {
                    _rightEquipSuitCells[i].Show(mList[i]);
                }
                else
                {
                    _rightEquipSuitCells[i].Show(null);
                }
            }

            if (suitList.Count < 1) {
                _rightEquipSuitCells[0].mDMono.gameObject.SetActive(true);
            }

            _tpEquipSuitRight.GetComponent<UIScrollView>().ResetPosition();
        }


        // ================== 已装备属性 ==================

        public void ShowEquippedInfo(int[] eids)
        {
            if (eids == null)
            {
                return;
            }

            var attr = new HeroEquipmentTotleAttr();

            for (int i = 0; i < eids.Length; i++)
            {
                int eid = eids[i];

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

            ShowEquippedSuitView(attr.SuitList);
            ShowEquippedAttr(attr);
        }

        private void ShowEquippedSuitView(List<SuitAttrsSuitTypeAndCount> suitList)
        {
            List<SuitAttrsSuitTypeAndCount> mList = suitList;

            mList.Sort((a, b) =>
            {
                if (a.count > b.count)
                    return -1;
                else if (a.count < b.count)
                    return 1;
                else if (a.SuitType > b.SuitType)
                    return -1;
                else
                    return 1;
            });

            for (int i = 0; i < _leftEquipSuitCells.Length; i++)//套装
            {
                if (i < mList.Count)
                {
                    _leftEquipSuitCells[i].Show(mList[i]);
                }
                else
                {
                    _leftEquipSuitCells[i].Show(null);
                }
            }

            var count = suitList.Count;

            if (suitList.Count < 1) {
                _leftEquipSuitCells[0].mDMono.gameObject.SetActive(true);
                count = 1;
            }

            _equipSuitLeft.GetComponent<UIScrollView>().ResetPosition();
            var num = count > 3 ? 3 : count;
            _equipSuitLeft.localPosition = new Vector3(-40 - (num * 190), _equipSuitLeft.localPosition.y);
        }

        private void ShowEquippedAttr(HeroEquipmentTotleAttr attr)
        {
            string plusStr = "+";

            //0攻击
            double temp = attr.ATK;
            _originAttrValues[0].text = EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[0] = temp;

            //1暴击
            temp = (attr.CritP + attr.SuitAttr.CritP) * 100;
            _originAttrValues[1].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[1] = temp;

            //2攻击加成
            temp = (attr.ATKrate + attr.SuitAttr.ATKrate) * 100;
            _originAttrValues[2].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[2] = temp;

            //3暴击伤害
            temp = (attr.CritV + attr.SuitAttr.CritV) * 100;
            _originAttrValues[3].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[3] = temp;

            //4生命
            temp = attr.MaxHP;
            _originAttrValues[4].text = EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[4] = temp;

            //5效果命中
            temp = (attr.SpExtra + attr.SuitAttr.SpExtra) * 100;
            _originAttrValues[5].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[5] = temp;

            //6生命加成
            temp = (attr.MaxHPrate + attr.SuitAttr.MaxHPrate) * 100;
            _originAttrValues[6].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[6] = temp;

            //7效果抵抗
            temp = (attr.SpRes + attr.SuitAttr.SpRes) * 100;
            _originAttrValues[7].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[7] = temp;

            //8防御
            temp = attr.DEF;
            _originAttrValues[8].text = EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[8] = temp;

            //9速度
            temp = (attr.Speedrate + attr.SuitAttr.Speedrate) * 100;
            _originAttrValues[9].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[9] = temp;

            //10防御加成
            temp = (attr.DEFrate + attr.SuitAttr.DEFrate) * 100;
            _originAttrValues[10].text = EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)temp));
            _equppedAttrValues[10] = temp;
        }

        public void ShowCompareInfo(bool isActive)
        {
            _showCompareInfo = isActive;

            for (var i = 0; i < _originAttrValues.Length; i++)
            {
                _originAttrValues[i].gameObject.SetActive(isActive);
            }

            _arrowObj.SetActive(isActive);
            _equipSuitLeft.gameObject.SetActive(isActive);

            if (isActive)
            {
                _tpEquipSuitRight.transform.localPosition = _tpEquipSuitRight.to;
            }
            else
            {
                _tpEquipSuitRight.transform.localPosition = _tpEquipSuitRight.from;
            }
        }
    }
}
