using UnityEngine;
using System.Collections;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerEquipSuitCellController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MainIcon = t.GetComponent<DynamicUISprite>("Icon/Icon (1)");
            NumLabel = t.GetComponent<UILabel>("Label");

        }


    
        public DynamicUISprite MainIcon;
        public UILabel NumLabel;
    
        public Hotfix_LT.Data.SuitTypeInfo SData;
        private SuitAttrsSuitTypeAndCount data;

        public void Show(SuitAttrsSuitTypeAndCount itemData)
        {
            if (itemData == null) 
            { 
                SData = new Data.SuitTypeInfo(); 
                data = itemData; mDMono.gameObject.SetActive(false);
                MainIcon.spriteName = null;
                NumLabel.text = NumLabel.transform.GetChild(0).GetComponent<UILabel>().text = null;
                return; 
            }

            data = itemData;
            SData = Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(data.SuitType);
            MainIcon.spriteName = Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(itemData.SuitType).SuitIcon;//装备角标LTPartnerEquipConfig.SuitIconDic[SData.SuitType];
            NumLabel.text = NumLabel.transform.GetChild(0).GetComponent<UILabel>().text = "[42fe79]" + itemData.count.ToString();
            mDMono. gameObject.SetActive(true);
        }
    }
}
