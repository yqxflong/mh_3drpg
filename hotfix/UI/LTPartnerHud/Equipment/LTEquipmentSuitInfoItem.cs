using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTEquipmentSuitInfoItem : DynamicCellController<Hotfix_LT.Data.SuitTypeInfo>
    {
        public DynamicUISprite Icon;
        public UISprite Bg;
        public UILabel NameLabel, SuitAttrLabel, CountLabel;
        public Hotfix_LT.Data.SuitTypeInfo data;
        public GameObject ChooseObj, AllSuitObj;
        public LTPartnerEquipMainController MainController;
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Icon = t.GetComponent<DynamicUISprite>("Icon/IMG");
            Bg = t.GetComponent<UISprite>("UnSuit");
            NameLabel = t.GetComponent<UILabel>("Label");
            SuitAttrLabel = t.GetComponent<UILabel>("Effect");
            CountLabel = t.GetComponent<UILabel>("NumLabel");
            ChooseObj = t.FindEx("SelectBg/Sprite").gameObject;
            AllSuitObj = t.FindEx("Icon/All").gameObject;
            MainController = t.transform.parent.parent.parent.parent.parent.parent.parent.GetUIControllerILRComponent<LTPartnerEquipMainController>();
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => MainController.SetSuitType(data)));
        }

        public override void Clean()
        {
            ChooseObj.SetActive(false);
            CountLabel.gameObject.SetActive(false);
            Bg.gameObject.SetActive(false);
            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = SuitAttrLabel.text = "";
            mDMono.gameObject.SetActive(false);
        }

        public override void Fill(Hotfix_LT.Data.SuitTypeInfo itemData)
        {
            data = itemData;
            mDMono.gameObject.SetActive(data != null);
            //, Bg未实现
            ChooseObj.SetActive(data.SuitType == LTPartnerEquipDataManager.Instance.CurSuitType);

            if (itemData.SuitType != -1)
            {
                Icon.spriteName = itemData.SuitIcon;//装备角标 LTPartnerEquipConfig.SuitIconDic[data.SuitType];
                Icon.gameObject.SetActive(true);
                AllSuitObj.SetActive(false);

                if (itemData.SuitAttr2 != 0)
                {
                    Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(itemData.SuitAttr2);//套装2
                    SuitAttrLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_1348"), suitAttr.Description);
                }
                else if (itemData.SuitAttr4 != 0)
                {
                    Hotfix_LT.Data.SkillTemplate suitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(itemData.SuitAttr4);//套装4
                    SuitAttrLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_1621"), suitAttr.Description);
                }

                //Hotfix_LT.Data.SuitAttribute suitAttr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(data.SuitAttr2);//套装2
                //string str = string.Format("{0} +{1}%", suitAttr.desc, (int)(suitAttr.value * 100f));
                //Hotfix_LT.Data.SkillTemplate suitAttr2 = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(data.SuitAttr4);//套装4
                //string str2 = suitAttr2.Description;
                //SuitAttrLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_2088"), str, str2);
            }
            else
            {
                SuitAttrLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_2204"));
                Icon.gameObject.SetActive(false);
                AllSuitObj.SetActive(true);
            }

            int count = 0;

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.SynthesisView)
            {
                count = (LTPartnerEquipDataManager.Instance.EquipSynSuitTypeAndCountDic.ContainsKey(data.SuitType)) ? LTPartnerEquipDataManager.Instance.EquipSynSuitTypeAndCountDic[data.SuitType] : 0;
            }
            else if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView) 
            {
                count = (LTPartnerEquipDataManager.Instance.SuitTypeAndCountDic.ContainsKey(data.SuitType)) ? LTPartnerEquipDataManager.Instance.SuitTypeAndCountDic[data.SuitType] : 0;
            } 
            else
            {
                count = (LTPartnerEquipDataManager.Instance.SuitTypeAndCountDicWithoutEquiped.ContainsKey(data.SuitType)) ? LTPartnerEquipDataManager.Instance.SuitTypeAndCountDicWithoutEquiped[data.SuitType] : 0;//未实现
            }

            CountLabel.text = string.Format("{0}", count);

            if (count == 0)
            {
                Bg.gameObject.SetActive(true);
                CountLabel.gameObject.SetActive(false);
                mDMono.transform.GetComponent<BoxCollider>().enabled = false;
            }
            else
            {
                Bg.gameObject.SetActive(false);
                CountLabel.gameObject.SetActive(true);
                mDMono.transform.GetComponent<BoxCollider>().enabled = true;
            }

            NameLabel.text = NameLabel.transform.GetChild(0).GetComponent<UILabel>().text = string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentSuitInfoItem_3047"), data.TypeName);
        }

    }
}
