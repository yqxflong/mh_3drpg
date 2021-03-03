using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
    
namespace Hotfix_LT.UI
{
    public class EquipSelectPartnerEven
    {
        public static Action<int> SelectPartnerID;
    }
    
    
    
    public class LTEquipPartnerCellController : DynamicCellController<LTPartnerData >
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            IconSprite = t.GetComponent<UISprite>("PartnerItem/Icon");
            FrameSprite = t.GetComponent<UISprite>("PartnerItem/Frame");
            FrameBGSprite = t.GetComponent<UISprite>("PartnerItem/FrameBG");
            LevelSprite = t.GetComponent<UISprite>("PartnerItem/Property");
            LevelLabel = t.GetComponent<UILabel>("PartnerItem/LabelLevel");
            breakLebel = t.GetComponent<UILabel>("PartnerItem/Break");
            StarController = t.GetMonoILRComponent<LTPartnerStarController>("PartnerItem/StarList");
            SelectSprite = t.GetComponent<UISprite>("PartnerItem/Select");

            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnSelectBtnClick));
        }


        public override void OnEnable()
        {
            EquipSelectPartnerEven.SelectPartnerID += OnSelectFun;
        }
        public override void OnDisable()
        {
            EquipSelectPartnerEven.SelectPartnerID -= OnSelectFun;
        }
        
        public UISprite IconSprite;
        public UISprite FrameSprite;
        public UISprite FrameBGSprite;
        public UISprite LevelSprite;
        public UILabel LevelLabel;
        public UILabel breakLebel;
        public LTPartnerStarController StarController;
        /*public UILabel CallLabel;
        public UISlider ShardSlider;
        public UISprite ShardSprite;
        public UILabel ShardLabel;*/
        public UISprite SelectSprite;
        private int curSelectId;
        private float clickTime;
    
        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;
    
        public enum SELECT_STATE
        {
            SELECT,
            UN_SELECT,
        }
    
        private SELECT_STATE mSelectState = SELECT_STATE.UN_SELECT;
    
        private LTPartnerData partnerData;
    
      
        public override void Fill(LTPartnerData itemData)
        {
            SetItemData(itemData);
        }
    
        public override void Clean()
        {
            SetItemData(null);
        }
    
        private void SetItemData(LTPartnerData itemData)
        {
            partnerData = itemData;
            UpdateItem();
        }
    
        private void UpdateItem()
        {
            if (partnerData == null|| partnerData.HeroId ==0)
            {
                mDMono.transform.GetChild(0).gameObject.SetActive(false);
                return;
            }
            mDMono.transform.GetChild(0).gameObject.SetActive(true);
            SelectSprite.gameObject.SetActive(partnerData.HeroId ==LTPartnerEquipDataManager.Instance.CurrentPartnerData .HeroId);
            IconSprite.spriteName = partnerData.HeroInfo.icon;
    
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(partnerData.UpGradeId, out quality, out addLevel);
    
            FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
    
            if (partnerData.HeroId <= 0)
            {
                SetGrey(IconSprite);
                SetGrey(FrameSprite);
                SetDark(FrameBGSprite);
            }
            else
            {
                SetNormal(IconSprite);
                SetNormal(FrameSprite);
            }
    
            if (addLevel > 0)
            {
                breakLebel.gameObject.SetActive(true);
                breakLebel.text = "+" + addLevel.ToString();
            }
            else
            {
                breakLebel.gameObject.SetActive(false);
            }
            //BottomSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];

            LevelSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[partnerData.HeroInfo.char_type];
            HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, FrameSprite.transform, quality, upgradeefclip);
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, LevelSprite.transform, (PartnerGrade)partnerData.HeroInfo.role_grade, partnerData.HeroInfo.char_type);
            LevelLabel.text = partnerData.Level.ToString();
    
            /*ShardSlider.value = (float)partnerData.ShardNum / (float)partnerData.HeroInfo.summon_shard;
            ShardSprite.spriteName = partnerData.ShardNum >= partnerData.HeroInfo.summon_shard ? "Ty_Strip_Green" : "Ty_Strip_Blue";
            ShardLabel.text = string.Format("{0}/{1}", partnerData.ShardNum, partnerData.HeroInfo.summon_shard);*/
    
            SetItemState();
            StarController.SetSrarList(partnerData.Star,partnerData.IsAwaken);
        }
    
        private void SetItemState()
        {
            if (partnerData == null)
            {
                return;
            }
            bool isOwn = partnerData.HeroId > 0;
            bool isCanCall = partnerData.ShardNum >= partnerData.HeroInfo.summon_shard;
            LevelSprite.gameObject.SetActive(isOwn);
            LevelLabel.gameObject.SetActive(isOwn);
            StarController.mDMono.gameObject.SetActive(isOwn);
            /*CallLabel.gameObject.SetActive(!isOwn && isCanCall);
            ShardSlider.gameObject.SetActive(!isOwn);*/
        }
    
        public void SetItemDataOther(LTPartnerData itemData, int quality = -1, int addLevel = -1)
        {
            SetItemData(itemData);
            SelectSprite.gameObject.SetActive(false);
    
            if (quality > -1 && itemData.HeroId > 0)
            {
                FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
                GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
                breakLebel.gameObject.SetActive(addLevel > 0);
                if (addLevel > 0)
                {
                    breakLebel.text = "+" + addLevel.ToString();
                }
            }
        }
    
        public void SetItemDataByStarUp(LTPartnerData itemData, int star)
        {
            SetItemData(itemData);
            SelectSprite.gameObject.SetActive(false);
            StarController.SetSrarList(star,itemData.IsAwaken);
        }

        private void SetGrey(UIWidget item)
        {
            item.color = new Color(1, 0, 1, 1);
        }
    
        private void SetNormal(UIWidget item)
        {
            item.color = new Color(1, 1, 1, 1);
        }
    
        private void SetDark(UIWidget item)
        {
            item.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }

        public void OnSelectBtnClick()
        {
            if (partnerData == null||partnerData.HeroId==0|| LTPartnerEquipDataManager.Instance.CurrentPartnerData.HeroId == partnerData.HeroId) return;
            else EquipSelectPartnerEven.SelectPartnerID(partnerData.HeroId);
        }
    
        private void OnSelectFun(int e)
        {
            if(partnerData!=null)SelectSprite.gameObject.SetActive(e == partnerData.HeroId);
        }
    }
}
