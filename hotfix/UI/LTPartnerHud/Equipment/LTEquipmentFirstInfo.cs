using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LT.Hotfix.Utility;

namespace Hotfix_LT.UI
{
    public class LTEquipmentFirstInfo : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            LevelupBtn = t.FindEx("UpLevelBtn").gameObject;
            ReplaceBtn = t.FindEx("ReplaceBtn").gameObject;
            GetOffBtn = t.FindEx("GetOffBtn").gameObject;
            EquipSynBtn = t.FindEx("EquipSynInOutBtn").gameObject;
            hasEquipObj = t.FindEx("Sprite").gameObject;
            DataItem = t.GetMonoILRComponent<LTPartnerEquipCellController>("AA");
            NameLabel = t.GetComponent<UILabel>("TitleName");
            MainAttr = t.GetComponent<Transform>("Infor/MainAttr");

            ExAttr = new Transform[4];
            ExAttr[0] = t.GetComponent<Transform>("Infor/ExAttr (1)");
            ExAttr[1] = t.GetComponent<Transform>("Infor/ExAttr (2)");
            ExAttr[2] = t.GetComponent<Transform>("Infor/ExAttr (3)");
            ExAttr[3] = t.GetComponent<Transform>("Infor/ExAttr (4)");

            Effect_2Label = t.GetComponent<UILabel>("EffectInfoPos/EffectPos/Infor (1)/Effect_2");
            Effect_4Label = t.GetComponent<UILabel>("EffectInfoPos/EffectPos/Infor (1)/Effect_4");
            EquipSynConditionLabel = t.GetComponent<UILabel>("EquipSynInOutBtn/Label");
            LockIcon = t.GetComponent<UISprite>("LockBtn/LockBtnBg");
            SecondInfoUI = t.GetMonoILRComponent<LTEquipmentSecondInfo>("EquipmentInfo");

            EffectPos = new Transform[2];
            EffectPos[0] = t.GetComponent<Transform>("EffectInfoPos/EffectPos (1)");
            EffectPos[1] = t.GetComponent<Transform>("EffectInfoPos/EffectPos");

            EffectTrans = t.GetComponent<Transform>("EffectInfoPos/EffectPos/Infor (1)");


            t.GetComponent<ConsecutiveClickCoolTrigger>("LockBtn").clickEvent.Add(new EventDelegate(OnLockBtnClick));
        }


    
        public GameObject LevelupBtn, ReplaceBtn, GetOffBtn, EquipSynBtn;
        public GameObject hasEquipObj;
        public LTPartnerEquipCellController DataItem;
        public LTEquipmentInforUIController _InforUI;

        public LTEquipmentInforUIController InforUI
        {
            get
            {
                if (_InforUI==null)
                {
                    _InforUI= mDMono.transform.parent.GetUIControllerILRComponent<LTEquipmentInforUIController>();
                }

                return _InforUI;
            }
        }
        public UILabel NameLabel;
        public Transform MainAttr;
        public Transform[] ExAttr;
        public UILabel Effect_2Label, Effect_4Label, EquipSynConditionLabel;
    
        public UISprite LockIcon;
        public DetailedEquipmentInfo data;
        public LTEquipmentSecondInfo SecondInfoUI;
        public LTPartnerData Pdata;
    
        public Transform[] EffectPos;
        public Transform EffectTrans;
    
        private int m_type = 0;
    
        public void Show(int type, int eid)//0为装备背包栏,1为已装备栏,2为升级背包栏
        {
            m_type = type;
            Pdata = LTPartnerEquipMainController.CurrentPartnerData;
            if(Pdata == null)
            {
                EB.Debug.LogError("LTEquipmentFirstInfo.Show Pdata == null");
            }
            data = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
            if(data == null)
            {
                EB.Debug.LogError("LTEquipmentFirstInfo.Show data == null");
                return;
            }
            if (type == 0)
            {
                LevelupBtn.transform.localPosition = new Vector3(-160, -985, 0);
                LevelupBtn.transform.localScale = Vector3.one;
                LevelupBtn.CustomSetActive(true);
                GetOffBtn.CustomSetActive(false);
                hasEquipObj.CustomSetActive(false);
                EquipSynBtn.CustomSetActive(false);

                if (Pdata == null)
                {
                    return;
                }

                var equippedId = Pdata.GetEquipmentsInfo((int)data.Type - 1).Eid;

                if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView)
                {
                    equippedId = LTPartnerEquipmentInfoController.instance.Eids[(int)data.Type - 1];
                }

                if (equippedId == 0)//没装备
                {
                    ReplaceBtn.transform.GetChild(0).GetComponent<UILabel>().text = ReplaceBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_EQUIP_TIPS_LOAD");
                    SecondInfoUI.mDMono.gameObject.CustomSetActive(false);
                }
                else//有装备
                {
                    ReplaceBtn.transform.GetChild(0).GetComponent<UILabel>().text = ReplaceBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_REPLACE");
                    SecondInfoUI.Show();
                    SecondInfoUI.mDMono.gameObject.CustomSetActive(true);
                }
    
                ReplaceBtn.CustomSetActive(true);
    
                Effect_4Label.effectStyle = UILabel.Effect.None;
                Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
                if (data.FirstSuitAttr != null)
                {
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[false], data.FirstSuitAttr);
                }
                else if (data.SecondSuitAttr != null)
                {
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[false], data.SecondSuitAttr);
                }
            }
            else if (type == 1)
            {
                LevelupBtn.transform.localPosition = new Vector3(-160, -985, 0);
                LevelupBtn.transform.localScale = Vector3.one;
                LevelupBtn.CustomSetActive(true);
                GetOffBtn.CustomSetActive(true);
                hasEquipObj.CustomSetActive(true);
                ReplaceBtn.CustomSetActive(false);
                EquipSynBtn.CustomSetActive(false);
                SecondInfoUI.mDMono.gameObject.CustomSetActive(false);
                if (LTPartnerEquipMainController.m_Open && LTPartnerEquipMainController.instance.CurrentEquipmentViewState != LTPartnerEquipMainController.EquipmentViewState.PresetView)
                {
                    GetOffBtn.transform.GetChild(0).GetComponent<UILabel>().text = GetOffBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_EQUIP_TIPS_UNLOAD");
                }
                else
                {
                    GetOffBtn.transform.GetChild(0).GetComponent<UILabel>().text = GetOffBtn.transform.GetChild(0).GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_REPLACE");
                }
                if (Pdata == null) return;
                List<SuitAttrsSuitTypeAndCount> SuitList = Pdata.EquipmentTotleAttr.SuitList;
                int Count = 0;
                for (int i = 0; i < SuitList.Count; i++)
                {
                    if (data.SuitType == SuitList[i].SuitType)
                    {
                        Count = SuitList[i].count;
                    }
                }
    
                int suitNeed = 6;
                if (data.FirstSuitAttr != null)
                {
                    suitNeed = 2;
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[Count >= suitNeed], data.FirstSuitAttr);
                }
                else if (data.SecondSuitAttr != null)
                {
                    suitNeed = 4;
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[Count >= suitNeed], data.SecondSuitAttr);
                }
    
                if (Count >= suitNeed)
                {
                    Effect_4Label.effectStyle = UILabel.Effect.Outline8;
                    Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.74f, 1f, 0.85f);
                }
                else
                {
                    Effect_4Label.effectStyle = UILabel.Effect.None;
                    Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
                }
            }
            else if (type == 2)
            {
                LevelupBtn.CustomSetActive(false);
                GetOffBtn.CustomSetActive(false);
                hasEquipObj.CustomSetActive(false);
                ReplaceBtn.CustomSetActive(false);
                EquipSynBtn.CustomSetActive(false);
                SecondInfoUI.mDMono.gameObject.CustomSetActive(false);
    
                Effect_4Label.effectStyle = UILabel.Effect.None;
                Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
                if (data.FirstSuitAttr != null)
                {
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[false], data.FirstSuitAttr);
                }
                else if (data.SecondSuitAttr != null)
                {
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[false], data.SecondSuitAttr);
                }
            }
            else if (type == 3)
            {
                LevelupBtn.transform.localPosition = new Vector3(160, -985, 0);
                LevelupBtn.transform.localScale = Vector3.one;
                LevelupBtn.CustomSetActive(true);
                GetOffBtn.CustomSetActive(false);
                hasEquipObj.CustomSetActive(false);
                ReplaceBtn.CustomSetActive(false);
                //判断显示取回还是放入
                BaseEquipmentInfo[] synArray = LTPartnerEquipMainController.EquipSynArray;
                bool tempisInList = false;
                
                for (int i = 0; i < synArray.Length; i++)
                {
                    if (synArray[i] != null)
                    {
                        if (synArray[i].Eid == eid)
                        {
                            //取出
                            EquipSynConditionLabel.text = EquipSynConditionLabel.transform.GetChild(0).GetComponent<UILabel>().text = EB.Localizer.GetString("ID_EQUIP_SYN_OUTOFLIST");
                            EquipSynBtn.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent[0] = new EventDelegate(delegate { SelectEquipEven.SelectSynEquipment(eid, false, delegate { InforUI.controller.Close(); }); });
                            tempisInList = true;
                        }
                    }
                }
                if (!tempisInList)
                {
                    //放入               
                    EquipSynConditionLabel.text = EquipSynConditionLabel.transform.GetChild(0).GetComponent<UILabel>().text= EB.Localizer.GetString("ID_EQUIP_SYN_INTOLIST");
                    EquipSynBtn.GetComponent<ConsecutiveClickCoolTrigger>().clickEvent[0] = new EventDelegate(delegate { SelectEquipEven.SelectSynEquipment(eid, true, delegate { InforUI.controller.Close(); }); });
                }
                EquipSynBtn.CustomSetActive(true);
                SecondInfoUI.mDMono.gameObject.CustomSetActive(false);
                Effect_4Label.effectStyle = UILabel.Effect.None;
                Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
                if (data.FirstSuitAttr != null)
                {
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[false], data.FirstSuitAttr);
                }
                else if (data.SecondSuitAttr != null)
                {
                    Effect_4Label.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[false], data.SecondSuitAttr);
                }
    
            }
    
            DataItem.Fill(data);
            NameLabel.applyGradient = true;
            NameLabel.gradientTop = LT.Hotfix.Utility.ColorUtility.QualityToGradientTopColor(data.QualityLevel);
            NameLabel.gradientBottom = LT.Hotfix.Utility.ColorUtility.QualityToGradientBottomColor(data.QualityLevel);
            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text =  data.Name;
    
            string MainStr = EquipmentUtility.AttrTypeTrans(data.MainAttributes.Name);
            MainAttr.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + MainStr;
            MainAttr.GetChild(1).GetComponent<UILabel>().text = EquipmentUtility.AttrTypeValue(data.MainAttributes);
            int ExIndex = data.ExAttributes.Count - 1;
            for (int i = 0; i < 4; i++)
            {
                if (i > ExIndex)
                {
                    ExAttr[i].gameObject.CustomSetActive(false);
                }
                else
                {
                    string ExNameStr = EquipmentUtility.AttrTypeTrans(data.ExAttributes[i].Name);
                    ExAttr[i].GetChild(0).GetComponent<UILabel>().text = ExNameStr;
                    ExAttr[i].GetChild(1).GetComponent<UILabel>().text = EquipmentUtility.AttrTypeValue(data.ExAttributes[i]);
                    ExAttr[i].gameObject.CustomSetActive(true);
                }
            }
            Effect_2Label.gameObject.CustomSetActive(false);
    
            int trans = 0;
            if (type == 2)
            {
                trans = 166;
                EffectTrans.SetParent(EffectPos[0]);
                EffectTrans.localPosition = Vector3.zero;
            }
            else
            {
                EffectTrans.SetParent(EffectPos[1]);
                EffectTrans.localPosition = Vector3.zero;
            }
            mDMono.transform.GetComponent<UIWidget>().height = 619 + 88 * data.ExAttributes.Count + ((Effect_2Label.gameObject.activeSelf) ? Effect_2Label.height : 0) + ((Effect_4Label.gameObject.activeSelf) ? Effect_4Label.height : 0) - trans;
            LockIcon.spriteName = data.isLock ? "Equipment_Icon_Suoding" : "Equipment_Icon_Jiesuo";
    
        }
    
        public void UpdateCollider()
        {
            mDMono.transform.GetComponent<UIWidget>().ResizeCollider();
            SecondInfoUI.UpdateCollider();
        }
    
        public void OnLockBtnClick()
        {
            LTPartnerEquipDataManager.Instance.RequireLock(data.Eid, !data.isLock, delegate
            {
                data = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(data.Eid);
                if(data == null)
                {
                    EB.Debug.LogError("LTEquipmentFirstinfo.OnLockBtnClick data is null");
                    return;
                }
                LockIcon.spriteName = data.isLock ? "Equipment_Icon_Suoding" : "Equipment_Icon_Jiesuo";
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                if (m_type == 2)
                {
                    if (LTPartnerEquipDataManager.Instance.UpLevelSelectList.Contains(data.Eid)) LTPartnerEquipDataManager.Instance.UpLevelSelectList.Remove(data.Eid);
                    SelectEquipEven.LockBtnClick(data.Eid);
                    SelectEquipEven.ChooseEquipment();
                    mDMono.transform.parent.GetComponent<UIController>().Close();
                }
            });
        }
    }
}
