using System;
using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTActivityWishSSRItem : DynamicMonoHotfix
    {
        public GameObject RedPoint;

        public GameObject EmptyObj;

        public GameObject FilledObj;
        public CampaignTextureCmp TextureCamp;
        public UISprite AttrSprite;
        public UISprite GradeSprite;
        public UIGrid StarGrid;
        public GameObject SSRParticle;
        private UIEventTrigger trigger;
        public Action wishBtnClick;
        
       

        public override void Awake()
        {
            base.Awake();

            RedPoint = mDMono.transform.Find("RedPoint").gameObject;
            EmptyObj = mDMono.transform.Find("Empty").gameObject;
            FilledObj = mDMono.transform.Find("Filled").gameObject;
            TextureCamp = mDMono.transform.Find("Filled/Portrait").GetComponent<CampaignTextureCmp>();
            AttrSprite = mDMono.transform.Find("BG/Attr").GetComponent<UISprite>();
            GradeSprite = mDMono.transform.Find("Filled/Grade").GetComponent<UISprite>();
            StarGrid = mDMono.transform.Find("Filled/Star").GetComponent<UIGrid>();
            SSRParticle = mDMono.transform.Find("Filled/Fx/SSR").gameObject;
            trigger= mDMono.transform.Find("BG").GetComponent<UIEventTrigger>();
            trigger.onClick.Add(new EventDelegate(OnItemClick));
            EmptyObj.CustomSetActive(true);
            FilledObj.CustomSetActive(false);
        }

        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;

        private HeroInfoTemplate Data;
        private string CurBuddyId;


        public void SetData(HeroInfoTemplate data)
        {
            bool isEmpty = data == null;
            Data = data;
            EmptyObj.CustomSetActive(isEmpty);
            FilledObj.CustomSetActive(!isEmpty);
            if (!isEmpty)
            {
                SetFilledState();
            }
        }

        private void SetFilledState()
        {
            Data.eRoleAttr type = Data.char_type;
            PartnerGrade grade = (PartnerGrade)Data.role_grade;
            int star = 3;
             TextureCamp.spriteName = Data.skin;
            AttrSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[type]; 
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, AttrSprite.transform, grade, type);
            GradeSprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[grade];

            for (int i = 0; i < StarGrid.transform.childCount; i++)
            {
                StarGrid.transform.GetChild(i).gameObject.CustomSetActive(i < star);
            }
            StarGrid.repositionNow = true;
        }

        public void OnItemClick()
        {
            if (wishBtnClick!=null)
            {
                wishBtnClick();
            }
        }

    }

}