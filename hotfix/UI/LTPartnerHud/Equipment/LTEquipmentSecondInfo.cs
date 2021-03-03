using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LTEquipmentSecondInfo : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            DataItem = t.GetMonoILRComponent<LTPartnerEquipCellController>("AA");
            NameLabel = t.GetComponent<UILabel>("TitleName");
            MainAttr = t.GetComponent<Transform>("Infor/MainAttr");

            ExAttr = new Transform[4];
            ExAttr[0] = t.GetComponent<Transform>("Infor/ExAttr (1)");
            ExAttr[1] = t.GetComponent<Transform>("Infor/ExAttr (2)");
            ExAttr[2] = t.GetComponent<Transform>("Infor/ExAttr (3)");
            ExAttr[3] = t.GetComponent<Transform>("Infor/ExAttr (4)");

            Effect_2Label = t.GetComponent<UILabel>("Infor (1)/Effect_2");
            Effect_4Label = t.GetComponent<UILabel>("Infor (1)/Effect_4");
            LockIcon = t.GetComponent<UISprite>("LockBtn/LockBtnBg");


            t.GetComponent<ConsecutiveClickCoolTrigger>("LockBtn").clickEvent.Add(new EventDelegate(OnLockBtnClick));
        }


        
        public LTPartnerEquipCellController DataItem;
    
        public UILabel NameLabel;
        public Transform MainAttr;
        public Transform []ExAttr;
        public UILabel Effect_2Label, Effect_4Label;
        
        public UISprite LockIcon;
        private DetailedEquipmentInfo data;
        private LTPartnerData Pdata;

        public void Show()
        {
            Pdata = LTPartnerEquipMainController.CurrentPartnerData;
            int eid = Pdata.GetEquipmentsInfo((int)LTPartnerEquipDataManager.Instance.CurType - 1).Eid;

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView)
            {
                eid = LTPartnerEquipmentInfoController.instance.Eids[(int)LTPartnerEquipDataManager.Instance.CurType - 1];
            }

            data = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
           
            if (data == null)
            {
                EB.Debug.LogError("LTEquipmentSecondInfo.Show data is null");
                return;
            }

            DataItem.Fill(data);
            NameLabel.applyGradient = true;
            NameLabel.gradientTop = LT.Hotfix.Utility.ColorUtility.QualityToGradientTopColor(data.QualityLevel);
            NameLabel.gradientBottom = LT.Hotfix.Utility.ColorUtility.QualityToGradientBottomColor(data.QualityLevel);
            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = data.Name;
            string MainStr = AttrTypeTrans(data.MainAttributes.Name);
            MainAttr.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + MainStr;
            MainAttr.GetChild(1).GetComponent<UILabel>().text = AttrTypeValue(data.MainAttributes);
            int ExIndex = data.ExAttributes.Count - 1;
            for (int i = 0; i < 4; i++)
            {
                if (i > ExIndex)
                {
                    ExAttr[i].gameObject.CustomSetActive(false);
                }
                else
                {
                    string ExNameStr = AttrTypeTrans(data.ExAttributes[i].Name);
                    ExAttr[i].GetChild(0).GetComponent<UILabel>().text = ExNameStr;
                    ExAttr[i].GetChild(1).GetComponent<UILabel>().text = AttrTypeValue(data.ExAttributes[i]);// string.Format("+{0}", (data.ExAttributes[i].Value > 1) ? data.ExAttributes[i].Value.ToString("f0") : ((data.ExAttributes[i].Value * 100.0f).ToString("f0") + "%"));
                    ExAttr[i].gameObject.CustomSetActive(true);
                }
            }
            LockIcon.spriteName = data.isLock ? "Equipment_Icon_Suoding" : "Equipment_Icon_Jiesuo";
    
            List<SuitAttrsSuitTypeAndCount> SuitList = Pdata.EquipmentTotleAttr.SuitList;
            int Count = 0;
            for(int i = 0; i < SuitList.Count; i++)
            {
                if(data.SuitType ==SuitList[i].SuitType)
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
    
            Effect_2Label.gameObject.CustomSetActive(false);
            mDMono.transform.GetComponent<UIWidget>().height = 445 + 88 * data.ExAttributes.Count + ((Effect_2Label.gameObject.activeSelf) ? Effect_2Label.height : 0) + ((Effect_4Label.gameObject.activeSelf) ? Effect_4Label.height : 0);
        }
    
        public void UpdateCollider()
        {
            mDMono.transform.GetComponent<UIWidget>().ResizeCollider();
        }
    
        public string AttrTypeTrans(string str)
        {
            switch (str)
            {
    			case "ATK": return EB.Localizer.GetString("ID_ATTR_ATK") + "："; 
    		    case "MaxHP": return EB.Localizer.GetString("ID_ATTR_MaxHP") + "："; 
    		    case "DEF": return EB.Localizer.GetString("ID_ATTR_DEF") + "："; 
    		    case "CritP": return EB.Localizer.GetString("ID_ATTR_CritP") + "："; 
    		    case "CritV": return EB.Localizer.GetString("ID_ATTR_CritV") + "："; 
    		    case "ChainAtk": return EB.Localizer.GetString("ID_ATTR_ChainAtk") + "："; 
    		    case "SpExtra": return EB.Localizer.GetString("ID_ATTR_SpExtra") + "："; 
    		    case "SpRes": return EB.Localizer.GetString("ID_ATTR_SpRes") + "："; 
    		    case "MaxHPrate": return EB.Localizer.GetString("ID_ATTR_MaxHPrate") + "："; 
    		    case "ATKrate": return EB.Localizer.GetString("ID_ATTR_ATKrate") + "："; 
    		    case "DEFrate": return EB.Localizer.GetString("ID_ATTR_DEFrate") + "："; 
    		    case "Speed": return EB.Localizer.GetString("ID_ATTR_Speed") + "："; 
    		    case "speedrate": return EB.Localizer.GetString("ID_ATTR_speedrate") + "："; 
    			default: return EB.Localizer.GetString("ID_ATTR_Unknown") + "："; 
    	}
    }
    
        public string AttrTypeValue(EquipmentAttr data)
        {
            switch (data.Name)
            {
                case "ATK": return ("+" + Mathf.FloorToInt(data.Value).ToString());
                case "MaxHP": return ("+" + Mathf.FloorToInt(data.Value).ToString());
                case "DEF": return ("+" + Mathf.FloorToInt(data.Value).ToString());
                case "CritP": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "CritV": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "ChainAtk": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "SpExtra": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "SpRes": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "MaxHPrate": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "ATKrate": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "DEFrate": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "Speed": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                case "speedrate": return ("+" + Mathf.FloorToInt(data.Value * 100).ToString() + "%");
                default: return EB.Localizer.GetString("ID_ATTR_Unknown") + "：";
            }
        }
    
        public void OnLockBtnClick()
        {
            LTPartnerEquipDataManager.Instance.RequireLock(data.Eid,!data.isLock, delegate {
                data = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(data.Eid);
                if (data == null)
                {
                    EB.Debug.LogError("LTEquipmentSecondInfo.OnLockBtnClick data is null");
                    return;
                }
                LockIcon.spriteName = data.isLock ? "Equipment_Icon_Suoding" : "Equipment_Icon_Jiesuo";
                Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
            });
        }
    }
}
