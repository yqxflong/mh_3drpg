using UnityEngine;

namespace Hotfix_LT.UI
{
    public class GuardHeroData
    {
        public int heroId;
        
        public string heroName;
        public int UpGradeId; 
        public string icon;
        public Data.eRoleAttr char_type; 
        public int role_grade ;
        public int star ; 
        public int heroLevel;
        public int isawaken;
    }
    
    
    
    public class LTPartnerGuardHeroItem:DynamicMonoHotfix
    {
        public UISprite BgSprite;
        //头像
        public UISprite HeroIcon;
        public UISprite HeroType;
        public UISprite HeroIconBorder;
        public UISprite HeriIconBorderBG;
        public UISprite GradeIcon;
        public LTPartnerStarController StarController;
        public UILabel BreakLabel;
        public UILabel HeroLevel;
        public UILabel HeroName;

        public GameObject redPoint;
        
        private int lastStarCount = 0;
        private int lastAwakenLevel = 0;
        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;

        private LTPartnerData PData;
        private GuardHeroData hData;
        private int index;
        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;

            BgSprite = t.GetComponent<UISprite>();
            HeroName = t.GetComponent<UILabel>("heroName");
            GradeIcon = t.GetComponent<UISprite>("HeroGrade");

            HeroIcon = t.GetComponent<UISprite>("Item/Icon");
            HeroType = t.GetComponent<UISprite>("Item/Property");
            HeroIconBorder = t.GetComponent<UISprite>("Item/Frame");
            HeriIconBorderBG = t.GetComponent<UISprite>("Item/FrameBG");
            StarController = t.GetMonoILRComponent<LTPartnerStarController>("Item/StarList");;
            BreakLabel = t.GetComponent<UILabel>("Item/BreakObj/Break");
            HeroLevel = t.GetComponent<UILabel>("Item/LevelSprite/LabelLevel");
            
            redPoint =  t.FindEx("Item/RedPoint").gameObject;
            // AddListener();
        }

        // private void AddListener()
        // {
        //     Hotfix_LT.Messenger.AddListener<bool>(Hotfix_LT.EventName.PartnerCultivateRP, SetRedPoint);
        // }
        //
        // public override void OnDestroy()
        // {
        //     Hotfix_LT.Messenger.RemoveListener<bool>(Hotfix_LT.EventName.PartnerCultivateRP, SetRedPoint);
        // }

        public void SetRedPoint()
        {
            bool isShow = LTPartnerDataManager.Instance.IsCanGuardLevelUp(PData.HeroId, hData, index);
            redPoint.SetActive(isShow);
        }
        
        
        public void SetHeroIcon(LTPartnerData PData,GuardHeroData hData,int index)
        {
            this.PData = PData;
            this.hData = hData;
            this.index = index;
            SetRedPoint();
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(hData.UpGradeId, out quality, out addLevel);
            if (addLevel > 0)
            {
                LTUIUtil.SetText(BreakLabel, "+" + addLevel.ToString());
                BreakLabel.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                BreakLabel.transform.parent.gameObject.SetActive(false);
            }
            BreakLabel.gameObject.SetActive(addLevel != 0);
            LTUIUtil.SetText(HeroLevel, hData.heroLevel.ToString());
            LTUIUtil.SetText(HeroName, hData.heroName);
            HeroIcon.spriteName = hData.icon;
            HeroType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[hData.char_type]; 
            HeroIconBorder.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, HeriIconBorderBG);
            GradeIcon.spriteName =LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)hData.role_grade];
            HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, HeroIconBorder.transform, quality, upgradeefclip);
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, HeroType.transform, (PartnerGrade)hData.role_grade, hData.char_type);
    
            int starCount = hData.star;
            if (starCount != lastStarCount||hData.isawaken!=lastAwakenLevel)
            {
                StarController.SetSrarList(starCount, hData.isawaken);
            }
            
            if (hData.heroLevel <= 0)
            {
                SetGrey(HeroIcon);
                SetGrey(HeroIconBorder);
                SetDark(HeriIconBorderBG);
            }
            else
            {
                SetNormal(HeroIcon);
                SetNormal(HeroIconBorder);
            }
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
    
    }
}