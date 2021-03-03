using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class LTPartnerEquipInfoCell : DynamicCellController<DetailedEquipmentInfo>
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            EquipmentIcon = t.GetComponent<DynamicUISprite>("Icon");
            SuitIcon = t.GetComponent<DynamicUISprite>("equipmentType");
            SelectBG = t.FindEx("SelectObj").gameObject;
            LockBG = t.FindEx("LockSprite").gameObject;
            Frame = t.GetComponent<UISprite>("Frame");
            AddIconObj = t.FindEx("AddIcon").gameObject;
            IconBG = t.FindEx("BG2").gameObject;
            tweenScaleAni = t.GetComponent<TweenScale>("AddIcon");
            EquipType = (EquipPartType)(t.GetSiblingIndex() + 1);
            IsSelect = false;
            LevelLabel = t.GetComponent<UILabel>("Label");
            FxObj = t.FindEx("Fx").gameObject;

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnEquipItemClick));

            var icon = mDMono.transform.parent.parent.Find("Anchor/UnEquipAll/Icon");

            if (icon != null)
            {
                oneKeySprite = icon.GetComponent<UISprite>();
            }
        }


        public DynamicUISprite EquipmentIcon;
        public DynamicUISprite SuitIcon;
        public GameObject SelectBG, LockBG;
        public UISprite Frame;
        public GameObject AddIconObj, IconBG;
        public TweenScale tweenScaleAni;
        public EquipPartType EquipType;

        [HideInInspector] public bool IsSelect;
        public BaseEquipmentInfo Data;
        public UILabel LevelLabel;
        public GameObject FxObj;
        private UISprite oneKeySprite;


        public override void Clean()
        {
            Data = null;
            if (Data == null)
            {
                SuitIcon.gameObject.SetActive(false);
                LockBG.SetActive(false);
                EquipmentIcon.spriteName = "";
                LevelLabel.gameObject.SetActive(false);
                SetAddIconObj(false);
                Frame.gameObject.SetActive(false);
                FxObj.SetActive(false);
                return;
            }
        }

        public override void Fill(DetailedEquipmentInfo itemData)
        {
            if (itemData == null || itemData.Eid == 0)
            {
                Data = null;
                SuitIcon.gameObject.SetActive(false);
                LockBG.SetActive(false);
                EquipmentIcon.spriteName = "";
                LevelLabel.gameObject.SetActive(false);
                IconBG.CustomSetActive(true);
                SetAddIconObj(IsHaveEquip);
                Frame.gameObject.SetActive(false);
                FxObj.SetActive(false);
                return;
            }
            else
            {
                SuitIcon.gameObject.SetActive(true);
                IconBG.CustomSetActive(false);
                Data = itemData;
            }

            EquipmentIcon.spriteName = Data.IconName;
            LockBG.SetActive(Data.isLock);
            SetFrame(Data.QualityLevel);
            Frame.gameObject.SetActive(true);
            SuitIcon.spriteName = Data.SuitIcon;//LTPartnerEquipConfig.SuitIconDic[Data.SuitType];

            if (Data.EquipLevel > 0)
            {
                LevelLabel.text = string.Format("+{0}", Data.EquipLevel);
                LevelLabel.transform.GetChild(0).GetComponent<UISprite>().spriteName = UIItemLvlDataLookup.GetEquipLevelBGStr(Data.QualityLevel);
                LevelLabel.gameObject.SetActive(true);
            }
            else
            {
                LevelLabel.gameObject.SetActive(false);
            }

            SetAddIconObj(false);
            FxObj.SetActive(false);
            int count = (Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(Data.SuitType).SuitAttr2 != 0) ? 2 : 4;//套装件数
            List<SuitAttrsSuitTypeAndCount> suitList;

            if (LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView ||
                LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetView)
            {
                suitList = LTPartnerEquipmentInfoController.instance.CurrentEquipmentTotalAttr.SuitList;
            }
            else
            {
                suitList = LTPartnerEquipMainController.CurrentPartnerData.EquipmentTotleAttr.SuitList;
            }

            for (int i = 0; i < suitList.Count; i++)
            {
                if (suitList[i].SuitType == Data.SuitType && suitList[i].count >= count)
                {
                    FxObj.SetActive(true);
                }
            }
        }

        private void SetFrame(int quality)
        {
            Color frameColor = LT.Hotfix.Utility.ColorUtility.QualityToColor(quality);
            if (quality >= 1 && quality <= 6)
            {
                Frame.spriteName = "Ty_Equipment_Di1";
            }
            else if (quality == 7)
            {
                frameColor = new Color(1, 1, 1, 1);
                Frame.spriteName = "Ty_Equipment_Di2";
            }

            Frame.color = frameColor;
        }

        private void SetAddIconObj(bool isShow)
        {
            AddIconObj.CustomSetActive(isShow);
            //一键装备or一键卸装
            bool hasAny = LTPartnerEquipDataManager.Instance.HasAnySuitEquip(LTPartnerEquipMainController.CurrentPartnerData);

            if (oneKeySprite != null)
            {
                oneKeySprite.spriteName = hasAny ? "Equipment_Icon_Shangzhuang" : "Equipment_Icon_Xiezhuang";
            }
        }

        private bool IsHaveEquip
        {
            get
            {
                IDictionary itemDataCollection;
                DataLookupsCache.Instance.SearchDataByID("inventory", out itemDataCollection);
                foreach (var info in itemDataCollection.Values)
                {
                    var inBag = EB.Dot.String("location", info, "") == "bag_items";

                    if (LTPartnerEquipMainController.instance != null &&
                        LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetEditView) {
                        inBag = true;
                    }

                    if (EB.Dot.String("system", info, "") == "Equipment" && inBag && EB.Dot.String("equipment_type", info, "") == ((int)EquipType).ToString())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void OnEquipItemClick()
        {
            if (Data != null)//&&!LTPartnerEquipMainController.isSelectPartnerOpen)//选择伙伴时，锁定防止弹框
            {
                if (Data.Eid != 0)
                {
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("fromType", 1);
                    ht.Add("eid", Data.Eid);
                    ht.Add("pos", 2);
                    GlobalMenuManager.Instance.Open("LTPartnerEquipmentInfoUI", ht);
                }
            }
            else
            {
                if (AddIconObj.activeSelf && LTPartnerEquipMainController.instance.CurrentEquipmentViewState == LTPartnerEquipMainController.EquipmentViewState.PresetView)
                {
                    LTPartnerEquipMainController.instance.OnClickEquipmentInfoItem();
                }
            }
        }

    }
}
