using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerEquipPartnerInfoController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            HeroQualityIcon = t.GetComponent<UISprite>("PartnerView/Quality");
            HeroNameLabel = t.GetComponent<UILabel>("PartnerView/TitleName");
            PartnerInfoItem = t.GetMonoILRComponent<LTPartnerListCellController>("PartnerView/Item");

            EquipmentCell = new LTPartnerEquipInfoCell[6];
            EquipmentCell[0] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("PartnerView/Equip/item");
            EquipmentCell[1] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("PartnerView/Equip/item (1)");
            EquipmentCell[2] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("PartnerView/Equip/item (2)");
            EquipmentCell[3] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("PartnerView/Equip/item (3)");
            EquipmentCell[4] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("PartnerView/Equip/item (4)");
            EquipmentCell[5] = t.GetMonoILRComponent<LTPartnerEquipInfoCell>("PartnerView/Equip/item (5)");
            
            
            AttrsLabel = new UILabel[8];
            AttrsLabel[0] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg/ValueLabel");
            AttrsLabel[1] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (1)/ValueLabel");
            AttrsLabel[2] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (2)/ValueLabel");
            AttrsLabel[3] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (3)/ValueLabel");
            AttrsLabel[4] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (4)/ValueLabel");
            AttrsLabel[5] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (5)/ValueLabel");
            AttrsLabel[6] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (6)/ValueLabel");
            AttrsLabel[7] = t.GetComponent<UILabel>("Switch/Content/InfoView/Bg (7)/ValueLabel");

            SuitObj = new LTPartnerEquipSuitCellController[6];
            SuitObj[0] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("PartnerView/EquipSuitView/Container/EquipmentIcon");
            SuitObj[1] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("PartnerView/EquipSuitView/Container/EquipmentIcon (1)");
            SuitObj[2] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("PartnerView/EquipSuitView/Container/EquipmentIcon (2)");
            SuitObj[3] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("PartnerView/EquipSuitView/Container/EquipmentIcon (3)");
            SuitObj[4] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("PartnerView/EquipSuitView/Container/EquipmentIcon (4)");
            SuitObj[5] = t.GetMonoILRComponent<LTPartnerEquipSuitCellController>("PartnerView/EquipSuitView/Container/EquipmentIcon (5)");
            SuitFxObj = t.GetComponent<Transform>("PartnerView/EquipSuitView/Label");
            isSuitTextShow = false;
            isSuitTypeShow = -1;
            isChangeColor = false;

            t.GetComponent<ConsecutiveClickCoolTrigger>("PartnerView/Anchor/UnEquipAll").clickEvent.Add(new EventDelegate(() =>OnFullAllEquipClick(t.GetComponent<UISprite>("PartnerView/Anchor/UnEquipAll/Icon"))));
        }


        
        public UISprite HeroQualityIcon;//伙伴品质
        public UILabel HeroNameLabel;//伙伴名字
        public LTPartnerListCellController PartnerInfoItem;//伙伴item
    
        public LTPartnerEquipInfoCell[] EquipmentCell;//装备栏
        
        public UILabel[] AttrsLabel;//0攻击，1防御，2生命，3连击，4暴击，5暴伤，6命中，7抵抗
        public double[] OldE_AttrValue=new double[8];//0攻击，1防御，2生命，3连击，4暴击，5暴伤，6命中，7抵抗
    
        public LTPartnerEquipSuitCellController[]SuitObj;
        public Transform SuitFxObj;
    
        public static bool isSuitTextShow=false ;
        public static int isSuitTypeShow = -1;
        public static bool isChangeColor = false;
    
        private LTPartnerData m_Data;
    
        public override void OnEnable()
        {            
            Hotfix_LT.Messenger.AddListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnEquipChangeFunc);
        }
    
        public override void OnDisable()
        {
            Hotfix_LT.Messenger.RemoveListener(Hotfix_LT.EventName.OnPartnerEquipChange, OnEquipChangeFunc);
        }
    
        private void OnEquipChangeFunc()
        {
            for (int i = 0; i < 6; i++)
            {
                EquipmentCell[i].tweenScaleAni.ResetToBeginning();
                EquipmentCell[i].tweenScaleAni.PlayForward();
            }
        }
    
        public void Show(LTPartnerData Data)
        {
            if (Data == null) return;
            m_Data = Data;
            HeroQualityIcon.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)Data.HeroInfo.role_grade];
            HeroNameLabel.text = Data.HeroInfo.name;
            PartnerInfoItem.Clean();
            PartnerInfoItem.Fill(Data);
    
            for(int i = 0; i < 6; i++)
            {
                int Eid = Data.GetEquipmentsInfo(i).Eid;
                if(Eid == 0) { EquipmentCell[i].Fill(null); }
                else
                {
                    DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(Eid);
                    if(info == null)
                    {
                        EB.Debug.LogError("LTPartnerEquipPartnerInfoController.Show info is null,Eid = {0}",Eid);
                        continue;
                    }
                    EquipmentCell[i].Fill(info);
                }
            }
            TypeSelect();
    
            LTAttributesData attrData = AttributesUtil.GetBaseAttributes(Data);
            HeroEquipmentTotleAttr E_Attr = Data.EquipmentTotleAttr;
    
            if(mDMono.gameObject.activeSelf)StartCoroutine(ColorAttrShow(isChangeColor));
    
            for (int k=0;k< E_Attr.SuitList.Count; k++)
            {
                Hotfix_LT.Data.SuitTypeInfo info = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(E_Attr.SuitList[k].SuitType);
                if (E_Attr.SuitList[k].SuitType == isSuitTypeShow)
                {
                    if (E_Attr.SuitList[k].count == 4 && isSuitTextShow &&info.SuitAttr4 !=0 || E_Attr.SuitList[k].count == 2 && isSuitTextShow&& info.SuitAttr2 != 0)
                    {
                        UITweener[] tweeners = SuitFxObj.GetComponents<UITweener>();
                        for (int j = 0; j < tweeners.Length; ++j)
                        {
                            tweeners[j].tweenFactor = 0;
                            tweeners[j].PlayForward();
                        }
                        StartCoroutine ( PlayShowSuitItem());
                    }
                }
            }
            SuitViewShow(E_Attr.SuitList);
            isSuitTextShow = false;
            isChangeColor = false;
        }
        public void TypeSelect()
        {
            for (int i = 0; i < 6; i++)
            {
                if (((int)LTPartnerEquipDataManager.Instance.CurType - 1) == i)
                {
                    EquipmentCell[i].SelectBG.gameObject.SetActive(true);
                }
                else
                {
                    EquipmentCell[i].SelectBG.gameObject.SetActive(false);
                }
            }
        }
    
        IEnumerator  PlayShowSuitItem()
        {
            yield return new WaitForSeconds(0.2f);
            for (int i=0;i< EquipmentCell.Length;i++)
            {
                if (EquipmentCell[i].Data!=null&&EquipmentCell[i].Data.SuitType==isSuitTypeShow)
                {
                    yield return new WaitForSeconds(0.1f);
                    EquipmentCell[i].mDMono.GetComponent<TweenScale>().ResetToBeginning();
                    EquipmentCell[i].mDMono.GetComponent<TweenScale>().PlayForward();
                }
            }
            yield break;
        }
    
        IEnumerator  ColorAttrShow(bool isChange=false)
        {
            if (isChange)
            {
                float timers=AttrsLabel[0].GetComponent<TweenScale>().duration;
                ShowColorAttr();
                yield return new WaitForSeconds(timers);
                yield return StartCoroutine(ColorAttrShow());
            }
            else
            {
                ShowAttr();
            }
            yield break;
        }
    
        public void ShowColorAttr()
        {
            LTAttributesData attrData = AttributesUtil.GetBaseAttributes(m_Data);
            HeroEquipmentTotleAttr E_Attr = m_Data.EquipmentTotleAttr;
    
            string plusStr = (E_Attr.ATK + E_Attr.ATKrate + E_Attr.SuitAttr.ATKrate) == 0 ? "[ffffff00]+[-]" : "+";
            double Temp = (attrData.m_ATK * (E_Attr.ATKrate + E_Attr.SuitAttr.ATKrate) + E_Attr.ATK);
            double Value = Temp - OldE_AttrValue[0];
            string colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[0].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)Temp));//0攻击
            if (Value != 0) { AttrsLabel[0].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[0].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[0] = Temp;
    
    
            plusStr = (E_Attr.DEF + E_Attr.DEFrate) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = (attrData.m_DEF * (E_Attr.DEFrate + E_Attr.SuitAttr.DEFrate) + E_Attr.DEF);
            Value = Temp - OldE_AttrValue[1];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[1].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)Temp));//1防御
            if (Value != 0) { AttrsLabel[1].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[1].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[1] = Temp;
    
    
            plusStr = (E_Attr.MaxHP + E_Attr.MaxHPrate + E_Attr.SuitAttr.MaxHPrate) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = (attrData.m_MaxHP * (E_Attr.MaxHPrate + E_Attr.SuitAttr.MaxHPrate) + E_Attr.MaxHP);
            Value = Temp - OldE_AttrValue[2];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[2].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)Temp));//2生命
            if (Value != 0) { AttrsLabel[2].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[2].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[2] = Temp;
    
    
            plusStr = (attrData.m_Speed * (E_Attr.Speedrate + E_Attr.SuitAttr.Speedrate)) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = (attrData.m_Speed * (E_Attr.Speedrate + E_Attr.SuitAttr.Speedrate));
            Value = Temp - OldE_AttrValue[3];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[3].text = EB.StringUtil.Format("{0}{1}", colorStr, Mathf.FloorToInt((float)Temp));//3速度
            if (Value != 0) { AttrsLabel[3].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[3].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[3] = Temp;
    
    
            plusStr = (E_Attr.CritP + E_Attr.SuitAttr.CritP) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = ((E_Attr.CritP + E_Attr.SuitAttr.CritP) * 100);
            Value = Temp - OldE_AttrValue[4];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[4].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)Temp));//4暴击
            if (Value != 0) { AttrsLabel[4].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[4].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[4] = Temp;
    
    
            plusStr = (E_Attr.CritV + E_Attr.SuitAttr.CritV) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = ((E_Attr.CritV + E_Attr.SuitAttr.CritV) * 100);
            Value = Temp - OldE_AttrValue[5];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[5].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)Temp));//5暴伤
            if (Value != 0) { AttrsLabel[5].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[5].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[5] = Temp;
    
    
            plusStr = (E_Attr.SpExtra + E_Attr.SuitAttr.SpExtra) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = ((E_Attr.SpExtra + E_Attr.SuitAttr.SpExtra) * 100);
            Value = Temp - OldE_AttrValue[6];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[6].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)Temp));//6命中
            if (Value != 0) { AttrsLabel[6].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[6].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[6] = Temp;
    
    
            plusStr = (E_Attr.SpRes + E_Attr.SuitAttr.SpRes) == 0 ? "[ffffff00]+[-]" : "+";
            Temp = ((/*attrData.m_SpRes +*/ E_Attr.SpRes + E_Attr.SuitAttr.SpRes) * 100);
            Value = Temp - OldE_AttrValue[7];
            colorStr = Value == 0 ? plusStr : Value < 0 ? "[ff6699]+" : "[42fe79]+";
            AttrsLabel[7].text = EB.StringUtil.Format("{0}{1}%", colorStr, Mathf.FloorToInt((float)Temp));//7抵抗
            if (Value != 0) { AttrsLabel[7].GetComponent<TweenScale>().ResetToBeginning(); AttrsLabel[7].GetComponent<TweenScale>().PlayForward(); }
            OldE_AttrValue[7] = Temp;
        }
        public void ShowAttr()
        {
            LTAttributesData attrData = AttributesUtil.GetBaseAttributes(m_Data);
            HeroEquipmentTotleAttr E_Attr = m_Data.EquipmentTotleAttr;
    
            string plusStr = (E_Attr.ATK + E_Attr.ATKrate + E_Attr.SuitAttr.ATKrate) == 0 ? "" : "+";
            double Temp = (attrData.m_ATK * (E_Attr.ATKrate + E_Attr.SuitAttr.ATKrate) + E_Attr.ATK);
            double Value = Temp - OldE_AttrValue[0];

			AttrsLabel[0].text = string.IsNullOrEmpty(plusStr) ? 
			EB.StringUtil.Format("{0}", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)Temp));//0攻击
			OldE_AttrValue[0] = Temp;
        
            plusStr = (E_Attr.DEF + E_Attr.DEFrate) == 0 ? "" : "+";
            Temp = (attrData.m_DEF * (E_Attr.DEFrate + E_Attr.SuitAttr.DEFrate) + E_Attr.DEF);
            Value = Temp - OldE_AttrValue[1];

			AttrsLabel[1].text = string.IsNullOrEmpty(plusStr) ? 
			EB.StringUtil.Format("{0}", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)Temp));//1防御
			OldE_AttrValue[1] = Temp;
    
            plusStr = (E_Attr.MaxHP + E_Attr.MaxHPrate + E_Attr.SuitAttr.MaxHPrate) == 0 ? "" : "+";
            Temp = (attrData.m_MaxHP * (E_Attr.MaxHPrate + E_Attr.SuitAttr.MaxHPrate) + E_Attr.MaxHP);
            Value = Temp - OldE_AttrValue[2];

			AttrsLabel[2].text = string.IsNullOrEmpty(plusStr) ?
			EB.StringUtil.Format("{0}", Mathf.FloorToInt((float)Temp)) :
			EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)Temp));//2生命
			OldE_AttrValue[2] = Temp;    
    
            plusStr = (attrData.m_Speed * (E_Attr.Speedrate + E_Attr.SuitAttr.Speedrate)) == 0 ? "" : "+";
            Temp = (attrData.m_Speed * (E_Attr.Speedrate + E_Attr.SuitAttr.Speedrate));
            Value = Temp - OldE_AttrValue[3];

            AttrsLabel[3].text = string.IsNullOrEmpty(plusStr) ?
			EB.StringUtil.Format("{0}", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}", plusStr, Mathf.FloorToInt((float)Temp));//3速度
			OldE_AttrValue[3] = Temp;   
    
            plusStr = (E_Attr.CritP + E_Attr.SuitAttr.CritP) == 0 ? "" : "+";
            Temp = ((E_Attr.CritP + E_Attr.SuitAttr.CritP) * 100);
            Value = Temp - OldE_AttrValue[4];

            AttrsLabel[4].text = string.IsNullOrEmpty(plusStr) ?
			EB.StringUtil.Format("{0}%", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)Temp));//4暴击
            OldE_AttrValue[4] = Temp;
    
    
            plusStr = (E_Attr.CritV + E_Attr.SuitAttr.CritV) == 0 ? "" : "+";
            Temp = ((E_Attr.CritV + E_Attr.SuitAttr.CritV) * 100);
            Value = Temp - OldE_AttrValue[5];

			AttrsLabel[5].text = string.IsNullOrEmpty(plusStr) ?
			EB.StringUtil.Format("{0}%", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)Temp));//5暴伤
            OldE_AttrValue[5] = Temp;
    
    
            plusStr = (E_Attr.SpExtra + E_Attr.SuitAttr.SpExtra) == 0 ? "" : "+";
            Temp = ((E_Attr.SpExtra + E_Attr.SuitAttr.SpExtra) * 100);
            Value = Temp - OldE_AttrValue[6];

            AttrsLabel[6].text = string.IsNullOrEmpty(plusStr) ?
			EB.StringUtil.Format("{0}%", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)Temp));//6命中
            OldE_AttrValue[6] = Temp;
    
    
            plusStr = (E_Attr.SpRes + E_Attr.SuitAttr.SpRes) == 0 ? "" : "+";
            Temp = ((E_Attr.SpRes + E_Attr.SuitAttr.SpRes) * 100);
            Value = Temp - OldE_AttrValue[7];

            AttrsLabel[7].text = string.IsNullOrEmpty(plusStr) ?
			EB.StringUtil.Format("{0}%", Mathf.FloorToInt((float)Temp)) : 
			EB.StringUtil.Format("{0}{1}%", plusStr, Mathf.FloorToInt((float)Temp));//7抵抗
            OldE_AttrValue[7] = Temp;
        }
    
        private void SuitViewShow(List<SuitAttrsSuitTypeAndCount> suitList)
        {
            List<SuitAttrsSuitTypeAndCount> mList = suitList;
            mList.Sort((a, b) => {
                if (a.count  > b.count)
                    return -1;
                else if (a.count < b.count)
                    return 1;
                else if (a.SuitType > b.SuitType)
                    return -1;
                else
                    return 1;
            });
            for(int i = 0; i < SuitObj.Length; i++)//套装
            {
                if(i< mList.Count)//&& mList[i].count>1)
                {
                    SuitObj[i].Show(mList[i]);
                }
                else
                {
                    SuitObj[i].Show(null);
                }
            }
        }
    
        /// <summary>
        /// 一键装满装备
        /// </summary>
        public void OnFullAllEquipClick(UISprite uiSprite)
        {
            if (uiSprite.spriteName=="Equipment_Icon_Shangzhuang")
            {
                //一键装备
                LTPartnerEquipDataManager.Instance.RequireEquipAll(m_Data.HeroId, ( success) =>
                {
                    if (success)
                    {
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                        Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                        MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_EQUIP_DRESS_SUCCESS"));
                    }
                });
                return;
            }
            
            LTPartnerEquipDataManager.Instance.RequireUnEquipAll(m_Data.HeroId, delegate (bool success) {
                if (success)
                {
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.OnPartnerEquipChange);
                    Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.onPartnerCombatPowerUpdate,3,true);
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTEquipmentInforUIController_4512"));
                    isChangeColor = true;
                }
            });
        }
    }
}
