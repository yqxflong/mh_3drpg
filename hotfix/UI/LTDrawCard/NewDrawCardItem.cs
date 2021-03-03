using UnityEngine;

namespace Hotfix_LT.UI
{
    public class NewDrawCardItem : DynamicMonoHotfix
    {

        public DialogueTextureCmp MainIcon;
        public UIGrid StarGrid;
        public UISprite QualityIcon;
        public UISprite GradeSprite;
        public UISprite FrameBG;
        public UILabel NameLabel;

        public GameObject SSRFxObj;
        public GameObject SRFxObj;
        public GameObject WishFlag;

        private Data.HeroInfoTemplate m_data;

        public override void Awake()
        {
            base.Awake();

            var t = mDMono.transform;
            MainIcon = t.GetComponent<DialogueTextureCmp>("Portrait");
            StarGrid = t.GetComponent<UIGrid>("Panel/Star");
            QualityIcon = t.GetComponent<UISprite>("Panel/Attr");
            GradeSprite = t.GetComponent<UISprite>("Panel/Grade");
            FrameBG = t.GetComponent<UISprite>("BG");
            NameLabel = t.GetComponent<UILabel>("Panel/Name");
            SSRFxObj = t.FindEx("BG/SSR").gameObject;
            SRFxObj = t.FindEx("BG/SR").gameObject;
            WishFlag = t.FindEx("Panel/Wish").gameObject;
            t.GetComponent<UIButton>().onClick.Add(new EventDelegate(ShowHeroInfoTipClick));
        }

        public void Fill(Data.HeroInfoTemplate itemData)
        {
            if (itemData == null)
            {    
                mDMono.gameObject.CustomSetActive(false);
                return;
            }
            else
            {
                mDMono.gameObject.CustomSetActive(true);
                m_data = itemData;
            }
            int statId;
            WishFlag.CustomSetActive(false);
            if (DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current", SSRWishItem.ssrWishActivityId), out statId))
            {
                if (itemData.id == (statId-1))
                {
                    if (LTDrawCardTypeController.Instance!=null)
                    {
                        if (string.IsNullOrEmpty(LTDrawCardTypeController.Instance.currentEventTag))
                        {
                            WishFlag.CustomSetActive(true);
                        }
                    }
                }
            }

            MainIcon.spriteName = m_data.skin;
            if (m_data.char_type == Hotfix_LT.Data.eRoleAttr.None)
            {
                QualityIcon.gameObject.CustomSetActive(false);
            }
            else
            {
                NameLabel.text = m_data.name;
                QualityIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[m_data.char_type]; 
                FrameBG.color = GetFrameBGName((PartnerGrade)m_data.role_grade);
                GradeSprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)m_data.role_grade];
                //UIShowItem.ShowCharTypeFX(charFx, efClip, QualityIcon.transform, (PartnerGrade)m_data.role_grade, (Hotfix_LT.Data.eRoleAttr)m_data.char_type);
            }

            SSRFxObj.CustomSetActive((PartnerGrade)m_data.role_grade == PartnerGrade.SSR);
            SRFxObj.CustomSetActive((PartnerGrade)m_data.role_grade == PartnerGrade.SR);

            //星星设置和伙伴状态屏蔽
            StarGrid.gameObject.CustomSetActive(true);
            for (int i = 0; i < StarGrid.transform.childCount; i++)
            {
                StarGrid.transform.GetChild(i).gameObject.CustomSetActive(i < m_data.init_star);
            }
            StarGrid.Reposition();
        }

        public void ShowHeroInfoTipClick()
        {
            FusionAudio.PostEvent("UI/General/ButtonClick");
            Vector2 screenPos = UICamera.lastEventPosition;
            var ht = Johny.HashtablePool.Claim();
            ht.Add("id", m_data.id);
            ht.Add("screenPos", screenPos);
            GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
        }

        private Color GetFrameBGName(PartnerGrade grade)
        {
            switch (grade)
            {
                case PartnerGrade.N:
                case PartnerGrade.R:
                    return new Color(105 / 255.0f, 255 / 255.0f, 146 / 255.0f);
                case PartnerGrade.SR:
                    return new Color(244 / 255.0f, 116 / 255.0f, 255 / 255.0f);
                default:
                    return new Color(255 / 255.0f, 196 / 255.0f, 56 / 255.0f);
            }
        }
    }
}
