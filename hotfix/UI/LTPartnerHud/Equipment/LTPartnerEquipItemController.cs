
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
    
namespace Hotfix_LT.UI
{
    public class LTPartnerEquipItemController : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            Level = t.GetComponent<UILabel>("LevelSp/Level");
            LevelBG = t.GetComponent<UISprite>("LevelSp");
            SuitType = t.GetComponent<DynamicUISprite>("SuitType");
            Frame = t.GetComponent<UISprite>("Frame");
            Icon = t.GetComponent<DynamicUISprite>("Icon");
            AddIconObj = t.FindEx("AddIcon").gameObject;
            IconBG = t.FindEx("BG2").gameObject;
            LockObj = t.FindEx("LockSp").gameObject;
            SuitEffect = t.FindEx("Fx").gameObject;
            tweenScaleAni = t.GetComponent<TweenScale>("AddIcon");
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnItemClick));
            oneKeySprite = t.parent.parent.Find("UnEquipAll/Icon").GetComponent<UISprite>();
        }

        public UILabel Level;
        public UISprite LevelBG;
        public DynamicUISprite SuitType;
        public UISprite Frame;
        public DynamicUISprite Icon;
        public GameObject AddIconObj,IconBG;
        public GameObject LockObj;
        public GameObject SuitEffect;
        public TweenScale tweenScaleAni;
        private UISprite oneKeySprite;
        private int eid;
        private EquipPartType equipType = EquipPartType.none;
        private bool isHaveEquip;
        private LTPartnerData partnerData;
        private LTPartnerDragController dragController;
    
        public void SetData(int eid, EquipPartType equipType, LTPartnerData partnerData)
        {
            this.eid = eid;
            this.equipType = equipType;
            if (partnerData != null)
            {
                this.partnerData = partnerData;
            }
            Init();

            //一键装备or一键卸装
            bool hasAny = LTPartnerEquipDataManager.Instance.HasAnySuitEquip(partnerData);
            oneKeySprite.spriteName = hasAny ? "Equipment_Icon_Shangzhuang" : "Equipment_Icon_Xiezhuang";
        }
    
        private void Init()
        {
            isHaveEquip = false;
            SuitType.gameObject.CustomSetActive(eid != 0);
            if (eid == 0)
            {
                SetEquipActive(false);
                SuitEffect.CustomSetActive(false);
                LockObj.CustomSetActive(false);
                LevelBG.gameObject.CustomSetActive(false);
                IconBG.CustomSetActive(true);
                if (partnerData.HeroId <= 0)
                {
                    SetAddIconObj(false);
                }
                else
                {
                   
                    isHaveEquip = IsHaveEquip();
                    SetAddIconObj(isHaveEquip);
                }
            }
            else
            {
                SetEquipActive(true);
                SetAddIconObj(false);
                IconBG.CustomSetActive(false);
                DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
                if (info == null)
                {
                    EB.Debug.LogError("LTPartnerEquipItemController.Init info is null");
                    return;
                }
                SetIcon(info.IconName);
                SetFrame(info.QualityLevel);
                SetSuit(info);
            }

        }
    
        public void AddFxCycleCallBack()
        {
            if (UIControllerHotfix.Current != null)
            {
                Action callback = new Action(() =>
                {
                    if (mDMono != null) mDMono.transform.Find("Fx").gameObject.CustomSetActive(false);
                });
                UIControllerHotfix.Current.CloseCallbacks.Add(callback);
            }
        }
        private void SetFrame(int quality)
        {
           
            if (quality >= 1 && quality <= 6)
            {
                Color frameColor = LT.Hotfix.Utility.ColorUtility.QualityToColor(quality);
                Frame.spriteName = "Ty_Equipment_Di1";
                Frame.color = frameColor;
            }
            else if (quality == 7)
            {
                Frame.spriteName = "Ty_Equipment_Di2";
                Frame.color = new Color(1, 1, 1, 1);
            }
            
        }
    
        private void SetIcon(string name)
        {
            Icon.spriteName = name;
        }
    
        private void SetSuit(DetailedEquipmentInfo info)
        {
            LockObj.CustomSetActive(info.isLock);
            LevelBG.gameObject.CustomSetActive(info.EquipLevel > 0);
            if (info.EquipLevel > 0)
            {
                Level.text = string.Format("+{0}", info.EquipLevel);
            }
            LevelBG.spriteName = UIItemLvlDataLookup.GetEquipLevelBGStr(info.QualityLevel);
            SuitType.spriteName = info.SuitIcon;
    
            SuitEffect.CustomSetActive(false);
            int count = (Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(info.SuitType).SuitAttr2 != 0) ? 2 : 4;//套装件数
            for (int i = 0; i < partnerData.EquipmentTotleAttr.SuitList.Count; i++)
            {
                if (partnerData.EquipmentTotleAttr.SuitList[i].SuitType == info.SuitType && partnerData.EquipmentTotleAttr.SuitList[i].count >= count)
                {
	                SuitEffect.CustomSetActive(true);
                    break;
                }
            }
        }
    
        private void SetEquipActive(bool isShow)
        {
            Frame.gameObject.CustomSetActive(isShow);
            Icon.gameObject.CustomSetActive(isShow);
        }
    
        private void SetAddIconObj(bool isShow)
        {
            AddIconObj.CustomSetActive(isShow);
            if (isShow)
            {
                tweenScaleAni.ResetToBeginning();
                tweenScaleAni.PlayForward();
               
            }
            else
            {
                oneKeySprite.spriteName = "Equipment_Icon_Xiezhuang";
            }
        }
    
        public void OnItemClick()
        {
            if (eid != 0 || !isHaveEquip || partnerData.HeroId <= 0)
            {
                if (eid != 0)
                {
                    LTPartnerEquipMainController.CurrentPartnerData = partnerData;
                    var ht = Johny.HashtablePool.Claim();
                    ht.Add("fromType", 1);
                    ht.Add("eid", eid);
                    ht.Add("pos", 0);
                    ht.Add("equipType", equipType);
                    GlobalMenuManager.Instance.Open("LTPartnerEquipmentInfoUI", ht);
                }
                return;
            }
    
            // 打开装备界面
            Hashtable table = Johny.HashtablePool.Claim();
            table["partnerData"] = partnerData;
            table["equipType"] = equipType;
            InputBlockerManager.Instance.Block(InputBlockReason.FUSION_BLOCK_UI_INTERACTION, 0.5f);
            GlobalMenuManager.Instance.Open("LTPartnerEquipmentUI", table);
        }
    
        private bool IsHaveEquip()
        {
            IDictionary itemDataCollection;
            DataLookupsCache.Instance.SearchDataByID("inventory", out itemDataCollection);
            if(itemDataCollection == null)
            {
                EB.Debug.LogError("LTPartnerEquipItemController.IsHaveEquip itemDataCollection == null");
                return false;
            }
            var iter = itemDataCollection.GetEnumerator();
            while (iter.MoveNext())
            {
                if (iter.Value!=null && EB.Dot.String("system", iter.Value, "").CompareTo("Equipment") == 0 && 
                    EB.Dot.String("location", iter.Value, "").CompareTo("bag_items") == 0 && 
                    EB.Dot.String("equipment_type", iter.Value, "").CompareTo(((int)equipType).ToString()) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
