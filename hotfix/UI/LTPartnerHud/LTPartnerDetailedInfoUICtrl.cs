using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class LTPartnerDetailedInfoUICtrl : UIControllerHotfix
    {
        public override bool IsFullscreen() { return false; }
        public override bool ShowUIBlocker { get { return true; } }

        private UILabel[] AttrNumLabels;
        LTPartnerData partnerData;

        public override void Awake()
        {
            base.Awake();
            Transform t = controller.transform;
            controller.backButton = t.GetComponent<UIButton>("BG/Top/CloseBtn");

            AttrNumLabels = new UILabel[11];
            AttrNumLabels[0] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr/NumLabel");//攻
            AttrNumLabels[1] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (1)/NumLabel");//防
            AttrNumLabels[2] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (2)/NumLabel");//血
            AttrNumLabels[3] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (3)/NumLabel");//暴
            AttrNumLabels[4] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (4)/NumLabel");//爆
            AttrNumLabels[5] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (5)/NumLabel");//速
            AttrNumLabels[6] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (6)/NumLabel");//命
            AttrNumLabels[7] = t.GetComponent<UILabel>("Content/Basic/Grid/Attr (7)/NumLabel");//抗

            AttrNumLabels[8] = t.GetComponent<UILabel>("Content/Advanced/Grid/Attr/NumLabel");//增
            AttrNumLabels[9] = t.GetComponent<UILabel>("Content/Advanced/Grid/Attr (1)/NumLabel");//减
            AttrNumLabels[10] = t.GetComponent<UILabel>("Content/Advanced/Grid/Attr (2)/NumLabel");//抗
        }

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            partnerData = param as LTPartnerData;
        }

        public override IEnumerator OnAddToStack()
        {
            ShowAttr();
            yield return base.OnAddToStack();
        }

        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }

        private void ShowAttr()
        {
            LTAttributesData curAttrData = AttributesManager.GetPartnerAttributesByParnterData(partnerData);
            LTAttributesData baseAttrData = AttributesUtil.GetBaseAttributes(partnerData);
            LTAttributesData equipData = AttributesManager.GetPartnerEquipmentAttributes(baseAttrData, partnerData.EquipmentTotleAttr);
            curAttrData.Add(equipData);
            
            AttrNumLabels[0].text = Mathf.FloorToInt(curAttrData.m_ATK).ToString();

            AttrNumLabels[1].text = Mathf.FloorToInt(curAttrData.m_DEF).ToString();

            AttrNumLabels[2].text = Mathf.FloorToInt(curAttrData.m_MaxHP).ToString();

            float num = curAttrData.m_CritP * 100;
            AttrNumLabels[3].text = string.Format("{0}%", num.ToString("f1"));

            num = curAttrData.m_CritV * 100;
            AttrNumLabels[4].text = string.Format("{0}%", num.ToString("f1"));

            AttrNumLabels[5].text = Mathf.FloorToInt(curAttrData.m_Speed).ToString();

            num =curAttrData.m_SpExtra * 100;
            AttrNumLabels[6].text = string.Format("{0}%", num.ToString("f1"));

            num = curAttrData.m_SpRes * 100;
            AttrNumLabels[7].text = string.Format("{0}%", num.ToString("f1"));

            num = curAttrData.m_DamageAdd * 100;
            AttrNumLabels[8].text = string.Format("{0}%", num.ToString("f1"));

            num =curAttrData.m_DamageReduce * 100;
            AttrNumLabels[9].text = string.Format("{0}%", num.ToString("f1"));

            num =curAttrData.m_CritDef * 100;
            AttrNumLabels[10].text = string.Format("{0}%", num.ToString("f1"));
        }
    }
}