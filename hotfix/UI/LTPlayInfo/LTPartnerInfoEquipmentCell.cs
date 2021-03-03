namespace Hotfix_LT.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LTPartnerInfoEquipmentCell : DynamicMonoHotfix
    {

        public UILabel Level;
        public UISprite LevelBG;
        public DynamicUISprite SuitType;
        public UISprite Frame;
        public DynamicUISprite Icon;
        public GameObject item;
        public UISprite FrameBG;

        private int equipmentType;
        private int eid = -1;
        private int equipmentLevel;
        private LTPartnerInfoController ctrl;
        private Hotfix_LT.Data.EquipmentItemTemplate tpl;

        private ParticleSystemUIComponent mQualityFX;
        private EffectClip mEffectClip;


        public void SetCellData(LTPartnerInfoController ctrl, int Eid, int equipmentType, int equipmentLevel)
        {
            //Debug.LogError("Eid cell is : " + Eid);
            if (Eid == -1)
            {
                this.eid = Eid;
                item.SetActive(false);
            }
            else
            {
                item.SetActive(true);
                this.ctrl = ctrl;
                this.eid = Eid;
                this.equipmentType = equipmentType;
                item.SetActive(Eid != 0);
                tpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(Eid);
                this.equipmentLevel = equipmentLevel;
                SetFrame(tpl.QualityLevel);
                SetIcon(tpl.IconId);
                SetSuit(tpl.QualityLevel, tpl.SuitIcon, equipmentLevel);
                HotfixCreateFX.ShowItemQualityFX(mQualityFX, mEffectClip, mDMono.transform, tpl.QualityLevel);
            }
        }

        private void SetFrame(int quality)
        {
            Frame.spriteName = UIItemLvlDataLookup.LvlToStr(quality.ToString());
            if (quality == 7) FrameBG.spriteName = "Ty_Quality_Xuancai_Di";
            else FrameBG.spriteName = "Ty_Di_2";
            FrameBG.color = LT.Hotfix.Utility.ColorUtility.QualityToFrameColor(quality);
        }

        private void SetIcon(string name)
        {
            Icon.spriteName = name;
        }

        private void SetSuit(int QualityLevel, string suitIcon, int EquipLevel)
        {
            if (EquipLevel > 0)
            {
                Level.text = string.Format("+{0}", EquipLevel);
            }
            Level.gameObject.SetActive(EquipLevel > 0);
            LevelBG.spriteName = UIItemLvlDataLookup.GetEquipLevelBGStr(QualityLevel);
            SuitType.spriteName = suitIcon;
        }

        /// <summary>
        /// 点击装备
        /// </summary>
        public void OnEquipmentClick()
        {
            if (eid < 0)
            {
                return;
            }
            //Debug.LogError(eid);
            ctrl.OnEquipmentClick(eid, equipmentLevel, equipmentType);
        }

        UIButton HotfixBtn0;
        public override void Awake()
        {
            base.Awake();

            Level = mDMono.transform.Find("EquipmentItem/LevelLabel").GetComponent<UILabel>();
            LevelBG = mDMono.transform.Find("EquipmentItem/LevelLabel/Sprite").GetComponent<UISprite>();
            SuitType = mDMono.transform.Find("EquipmentItem/listCellBG").GetComponent<DynamicUISprite>();
            Frame = mDMono.transform.Find("EquipmentItem/IMG/LvlBorder").GetComponent<UISprite>();
            Icon = mDMono.transform.Find("EquipmentItem/IMG").GetComponent<DynamicUISprite>();
            item = mDMono.transform.Find("EquipmentItem").gameObject;
            FrameBG = mDMono.transform.Find("EquipmentItem/IMG/LvlBorder/Bg").GetComponent<UISprite>();
            HotfixBtn0 = mDMono.GetComponent<UIButton>();
           if(HotfixBtn0!=null) HotfixBtn0.onClick.Add(new EventDelegate(OnEquipmentClick));
        }

    }

}
