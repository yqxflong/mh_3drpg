using Hotfix_LT.Data;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTDrawSSRWishItem:DynamicMonoHotfix
    {
        public GameObject EmptyObj;
        public GameObject ShowObj;
        public UIButton btn;

        public UISprite icon;
        public UISprite roleIcon;
        public UISprite gradeIcon;

        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;

        public override void Awake()
        {
            base.Awake();
            var t = mDMono.transform;
            EmptyObj = t.FindEx("Add").gameObject;
            ShowObj = t.FindEx("Show").gameObject;
            
            btn = t.GetComponent<UIButton>();
            btn.onClick.Add(new EventDelegate(OnWishClick));
            icon = t.Find("Show/Icon").GetComponent<UISprite>();
            roleIcon = t.Find("Show/Role").GetComponent<UISprite>();
            gradeIcon = t.Find("Show/Grade").GetComponent<UISprite>();  
            FillData();
            Messenger.AddListener<int>(EventName.OnSSRWishRefresh,OnSSRWishRefresh);
        }
        
        private void OnSSRWishRefresh(int arg)
        {
            FillData();
        }

        
        public override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.RemoveListener<int>(EventName.OnSSRWishRefresh,OnSSRWishRefresh); 
        }

        public void FillData()
        {
            int statId;
            if (DataLookupsCache.Instance.SearchIntByID(
                string.Format("tl_acs.{0}.current", SSRWishItem.ssrWishActivityId), out statId))
            {
                EmptyObj.CustomSetActive(false);
                ShowObj.CustomSetActive(true);
                HeroInfoTemplate data = CharacterTemplateManager.Instance.GetHeroInfo(statId-1);
                icon.spriteName = data.icon;
                roleIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[data.char_type];
                gradeIcon.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade) data.role_grade];

                HotfixCreateFX.ShowCharTypeFX(charFx, efClip, roleIcon.transform, (PartnerGrade) data.role_grade,
                    data.char_type);
                btn.gameObject.CustomSetActive(true);
            }
            else
            {
                EmptyObj.CustomSetActive(true);
                ShowObj.CustomSetActive(false);
            }
        }
        
        private void OnWishClick()
        {
            if (LTDrawCardTypeController.Instance != null)
            {
                if (LTDrawCardTypeController.Instance.IsSSRActivityOpen() == false)
                {
                    MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_codefont_in_LTHeroBattleMatchHudController_10973"));
                    LTDrawCardTypeController.Instance.controller.GObjects["SSRWish"].CustomSetActive(false);
                    return;
                }
            }
            GlobalMenuManager.Instance.Open("LTSSRWishPartnerHud");
        }
    }
}