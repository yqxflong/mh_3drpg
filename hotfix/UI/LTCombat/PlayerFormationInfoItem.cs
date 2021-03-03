
namespace Hotfix_LT.UI
{
    public class PlayerFormationInfoItem : DynamicMonoHotfix
    {
        public UILabel levelLab;
        public UISprite LevelLabSprite;
        public UILabel breskLevelLab;
        public UISprite frameSp;
        public UISprite frameBGSp;
        public UISprite parentTypeSp;
        public DynamicUISprite iconSp;
        public LTPartnerStarController starController;
    
        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;
        private PlayerFormationData mData;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            levelLab = t.GetComponent<UILabel>("LabelLevel");
            LevelLabSprite = t.GetComponent<UISprite>("LabelLevel/LevelSprite");
            breskLevelLab = t.GetComponent<UILabel>("Break");
            frameSp = t.GetComponent<UISprite>("Frame");
            frameBGSp = t.GetComponent<UISprite>("FrameBG");
            parentTypeSp = t.GetComponent<UISprite>("Property");
            iconSp = t.GetComponent<DynamicUISprite>("Icon");
            starController = t.GetMonoILRComponent<LTPartnerStarController>("StarList");
        }

        public void ShowUI(PlayerFormationData data)
        {
            mDMono.gameObject.SetActive(true);
            mData = data;
            Refresh();
        }
    
        public void HideUI()
        {
            mDMono.gameObject.SetActive(false);
        }
    
        private void Refresh()
        {
            LTUIUtil.SetLevelText(LevelLabSprite,levelLab,mData.level+mData.peak);
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(mData.awakeLevel, out quality, out addLevel);
    
            frameSp.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, frameBGSp);
    
            if (addLevel > 0)
            {
                breskLevelLab.gameObject.SetActive(true);
                breskLevelLab.text = "+" + addLevel.ToString();
            }
            else
            {
                breskLevelLab.gameObject.SetActive(false);
            }

            parentTypeSp.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[(Hotfix_LT.Data.eRoleAttr)mData.charType]; 

            starController.SetSrarList(mData.star,mData.isAwaken);
    
            Hotfix_LT.Data.HeroStatTemplate tempHeroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(mData.templateId);
    
            if (tempHeroStat != null)
            {
                Hotfix_LT.Data.HeroInfoTemplate tempHeroInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(tempHeroStat.character_id, mData.skin);
                if (tempHeroInfo != null)
                {
                    iconSp.spriteName = tempHeroInfo.icon;
                    HotfixCreateFX.ShowCharTypeFX(charFx, efClip, parentTypeSp.transform, (PartnerGrade)tempHeroInfo.role_grade, (Hotfix_LT.Data.eRoleAttr)mData.charType);
                    HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, frameSp.transform, quality, upgradeefclip);
                }
            }
        }
    }
}
