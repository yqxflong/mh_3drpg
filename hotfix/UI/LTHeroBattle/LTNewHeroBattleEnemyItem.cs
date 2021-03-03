using System.Collections;
using System.Collections.Generic;
using Hotfix_LT.Data;
using UnityEngine;
    
namespace Hotfix_LT.UI
{
    public class LTNewHeroBattleEnemyItem : DynamicMonoHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            CostLabel = t.GetComponent<UILabel>("HideObj/CostSprite/CostLabel");
            breakLebel = t.GetComponent<UILabel>("HideObj/BreakObj/Break");
            HeroIcon = t.GetComponent<UISprite>("Icon");
            CharSprite = t.GetComponent<UISprite>("HideObj/Property");
            FrameSprite = t.GetComponent<UISprite>("Frame");
            FrameBGSprite = t.GetComponent<UISprite>("FrameBG");
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(OnItemClick));

        }
        private Coroutine m_Coroutine;

    
        public UILabel CostLabel, breakLebel;
        public UISprite HeroIcon;
        public UISprite CharSprite, FrameSprite, FrameBGSprite;
    
        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;
        private int StatId;
        public void Fill(string statId)
        {
            if (string.IsNullOrEmpty(statId))
            {
                Clear();
                return;
            } 
            StatId = int.Parse(statId);
            var data= Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfoByStatId(StatId);
            CostLabel.text = LTNewHeroBattleManager.GetInstance().GetCostByRoleGrade(data.role_grade).ToString();
            HeroIcon.spriteName = data.icon;
            CharSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.char_type]; 
            // HotfixCreateFX.ShowCharTypeFX(charFx, efClip, CharSprite.transform, (PartnerGrade)data.role_grade, data.char_type);
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(LTPartnerConfig.MAX_GRADE_LEVEL, out quality, out addLevel);

            FrameSprite.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, FrameBGSprite);
            // HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, FrameBGSprite.transform, quality, upgradeefclip);
            if (addLevel > 0)
            {
                breakLebel.transform.parent.gameObject.CustomSetActive(true);
                breakLebel.text = "+" + addLevel.ToString();
            }
            else
            {
                breakLebel.transform.parent.gameObject.CustomSetActive(false);
            }
            mDMono.gameObject.CustomSetActive(true);

            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            m_Coroutine=StartCoroutine(LoadFx(data,quality));
        }

        private IEnumerator LoadFx(HeroInfoTemplate data, int quality )
        {
            yield return new  WaitForSeconds(0.2f);
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, CharSprite.transform, (PartnerGrade)data.role_grade, data.char_type);
            HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, FrameBGSprite.transform, quality, upgradeefclip);
        }

        public void Clear()
        {
            mDMono.gameObject.CustomSetActive(false);
        }
    
        public void OnItemClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            Vector2 screenPos = UICamera.lastEventPosition;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("id", StatId);
            ht.Add("screenPos", screenPos);
            GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
        }
    }
}
