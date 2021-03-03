using System;
using System.Collections;
using Hotfix_LT.Data;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class LTActivityBodyItem_SSRWish : LTActivityBodyItem
    {
        private UILabel _titleLabel;
        private LTActivityWishSSRItem _ssrItem;
        private UIButton newSSRBtn;
        private DialogueTextureCmp newSsRtTexture;
        
        public override void Awake()
        {
            base.Awake();
            _titleLabel = mDMono.transform.GetComponent<UILabel>("TitleLabel");
            _ssrItem = mDMono.transform.GetMonoILRComponent<LTActivityWishSSRItem>("0");
            newSsRtTexture = mDMono.transform.GetComponent<DialogueTextureCmp>("HalfTexture");
            newSSRBtn = mDMono.transform.GetComponent<UIButton>("NewButtonBg");
            newSSRBtn.onClick.Add(new EventDelegate(NewSSRBtnClick));
          }

        public override void OnEnable()
        {
            base.OnEnable();
            Messenger.AddListener<int>(EventName.OnSSRWishRefresh,RefreshWishHero);
        }

        private void NewSSRBtnClick()
        {
            HeroInfoTemplate template =  CharacterTemplateManager.Instance.GetHeroInfoNewest();
            if (template != null)
            {
                var ht = Johny.HashtablePool.Claim();
                ht.Add("id", template.id);
                ht.Add("screenPos", UICamera.lastEventPosition);
                ht.Add("showFullBg", true);
                GlobalMenuManager.Instance.Open("LTHeroToolTipUI", ht);
            }
        }
        
        

        public override void OnDisable()
        {
            base.OnDisable();
            Messenger.RemoveListener<int>(EventName.OnSSRWishRefresh,RefreshWishHero);
      }

        public override void SetData(object data)
        {
            base.SetData(data);

            HeroInfoTemplate template = CharacterTemplateManager.Instance.GetHeroInfoNewest();
            if (template != null)
            {
                newSsRtTexture.spriteName = template.portrait;
            }
            _titleLabel.gameObject.CustomSetActive(false);
            NavButton.gameObject.CustomSetActive(false);
            _ssrItem.mDMono.gameObject.CustomSetActive(false);
            
            if (state.Equals("pending") || !state.Equals("running") || EB.Time.Now > fintime)
            {
                return;
            }
            _ssrItem.mDMono.gameObject.CustomSetActive(true);
            _ssrItem.wishBtnClick = OnWishButtonClicked;
            
            int activityId = EB.Dot.Integer("activity_id", data, 0);
            int statId;
             
            if (DataLookupsCache.Instance.SearchIntByID(string.Format("tl_acs.{0}.current", activityId), out statId))
            {
                 RefreshWishHero(statId-1);
            }
            
            ArrayList arrayList;
            int count = 0;
            if (DataLookupsCache.Instance.SearchDataByID(string.Format("tl_acs.{0}.wish_reward", activityId), out arrayList) && arrayList != null)
            {
                count = arrayList.Count;
            }
            
            SetTitleLabel(count);
            NavLabel.text = EB.Localizer.GetString("ID_DIALOG_BUTTON_GO");
            NavButton.gameObject.CustomSetActive(true);
            NavButton.onClick.Clear();
            NavButton.onClick.Add(new EventDelegate(() => GlobalMenuManager.Instance.Open("LTDrawCardTypeUI",DrawCardType.hc)));
        }

        public void RefreshWishHero(int infoId)
        {
            HeroInfoTemplate temp = CharacterTemplateManager.Instance.GetHeroInfo(infoId);
            _ssrItem.SetData(temp);
        }

        private void SetTitleLabel(int num)
        {
            if (_titleLabel != null)
            {
                _titleLabel.gameObject.CustomSetActive(true);
                _titleLabel.text = string.Format(EB.Localizer.GetString("ID_PARTNER_WISH_GET"), num);
            }
        }

      

        private void OnWishButtonClicked()
        {
            if (!state.Equals("running") || EB.Time.Now > fintime)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_uifont_in_LTLegionWarQualify_End_4"));
                return;
            }

            GlobalMenuManager.Instance.Open("LTSSRWishPartnerHud");
        }
    }
}