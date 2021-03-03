using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTEquipmentSynPreViewController : DynamicMonoHotfix
    {
        public GameObject EquipmentCell;
        public DynamicUISprite EquipIcon;
        public DynamicUISprite EquipsuitIcon;
        public UISprite QualityLevel;
        public UISprite QualityFrameBg;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            EquipmentCell = t.FindEx("EquipmentItem").gameObject;
            EquipIcon = t.GetComponent<DynamicUISprite>("EquipmentItem/IMG");
            EquipsuitIcon = t.GetComponent<DynamicUISprite>("EquipmentItem/listCellBG");
            QualityLevel = t.GetComponent<UISprite>("EquipmentItem/IMG/LvlBorder");
            QualityFrameBg = t.GetComponent<UISprite>("EquipmentItem/IMG/LvlBorder/Bg");
        }

        public void Fill(string iconname,string suiticon,int qualityLevel)
        {
            EquipIcon.spriteName = iconname;
            EquipsuitIcon.spriteName = suiticon;
            QualityLevel.spriteName = UIItemLvlDataLookup.LvlToStr(qualityLevel.ToString());
            QualityFrameBg.spriteName = UIItemLvlDataLookup.GetItemFrameBGSprite(qualityLevel);
            QualityFrameBg.color = UIItemLvlDataLookup.GetItemFrameBGColor(qualityLevel.ToString());
            EquipmentCell.CustomSetActive(true);
        }

        public void Clean()
        {
            EquipmentCell.CustomSetActive(false);
        }
    }
}
