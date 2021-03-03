using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hotfix_LT.UI
{
    public class HandbookCardItem : DynamicMonoHotfix
    {
        public IHandBookAddAttrType Type;

        public GameObject RedPoint;

        public UISprite AttrAddSprite;
        public UILabel AttrAddLabel;

        public GameObject LockObj;
        public UILabel LockTipLabel;
        public UISprite RoleAttrIcon;

        public GameObject EmptyObj;

        public GameObject FilledObj;
        public DialogueTextureCmp TextureCamp;
        public UISprite AttrSprite;
        public UISprite GradeSprite;
        public UIGrid StarGrid;
        private UISprite[] starSpArray;
        public GameObject URBorder,SSRBorder, SRBorder, RBorder;

        public GameObject SRParticle;
        public GameObject SSRParticle;

        private UIEventTrigger trigger;

        public override void Awake()
        {
            base.Awake();
            Transform t = mDMono.transform;
            RedPoint = t.Find("RedPoint").gameObject;
            AttrAddSprite = t.Find("AddAttr").GetComponent<UISprite>();
            AttrAddLabel = t.Find("AddAttr/AttrAddNum").GetComponent<UILabel>();
            LockObj = t.Find("Lock").gameObject;
            LockTipLabel = t.Find("Lock/Label").GetComponent<UILabel>();
            RoleAttrIcon = t.Find("Lock/Icon").GetComponent<UISprite>();
            EmptyObj = t.Find("Empty").gameObject;
            FilledObj = t.Find("Filled").gameObject;
            TextureCamp = t.Find("Filled/Portrait").GetComponent<DialogueTextureCmp>();
            AttrSprite = t.Find("Filled/Attr").GetComponent<UISprite>();
            GradeSprite = t.Find("Filled/Grade").GetComponent<UISprite>();
            StarGrid = t.Find("Filled/Star").GetComponent<UIGrid>();
            starSpArray = new UISprite[6];
            for (int i = 0; i < 6; i++)
            {
                starSpArray[i] = t.GetComponent<UISprite>($"Filled/Star/{i}1");
            }
            URBorder = t.Find("Filled/URBorder").gameObject;
            SSRBorder = t.Find("Filled/SSRBorder").gameObject;
            SRBorder = t.Find("Filled/SRBorder").gameObject;
            RBorder = t.Find("Filled/RBorder").gameObject;
            SRParticle = t.Find("Filled/Fx/SR").gameObject;
            SSRParticle = t.Find("Filled/Fx/SSR").gameObject;
            trigger= t.Find("BG").GetComponent<UIEventTrigger>();
            trigger.onClick.Add(new EventDelegate(OnItemClick));
        }

        private ParticleSystemUIComponent charFx;
        private EffectClip efClip;

        private HandbookCardData Data;
        private string CurBuddyId;

        public void SetType(IHandBookAddAttrType e)
        {
            Type = e;
        }

        public void SetData(HandbookCardData data)
        {
            bool isChange = Data != null;
            Data = data;
            Data.SetHandBookCard();
            SetAttAddNum(Data.PartnerData);
            LockObj.CustomSetActive(Data.State == HandbookCardState.Lock);
            EmptyObj.CustomSetActive(Data.State == HandbookCardState.Empty);
            FilledObj.CustomSetActive(Data.State == HandbookCardState.Filled);
            switch (Data.State)
            {
                case HandbookCardState.Lock:
                    {
                        RoleAttrIcon.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[Data.handbookId]; 
                        LockTipLabel.text = string.Format(EB.Localizer.GetString("ID_codefont_in_BattleReadyHudController_12002"), Data.UnlockLevel);
                    }
                    ; break;
                case HandbookCardState.Empty:; break;
                case HandbookCardState.Filled:
                    SetFilledState();
                    break;
                default:; break;
            }
            InitCardParticle(isChange);

            if (Data.State==HandbookCardState.Empty &&  LTPartnerHandbookManager.Instance.TheHandbookList.Find(Data.handbookId).HasAvailableCard )
            {
               RedPoint.SetActive(true);
            }
            else
            {
                RedPoint.SetActive(false);
            }
        }

        private void InitCardParticle(bool isChange)
        {
            if (Data.PartnerData != null)
            {
                if (isChange && !Data.PartnerData.HeroId.ToString().Equals(CurBuddyId))
                {
                    SRParticle.CustomSetActive(false);
                    SSRParticle.CustomSetActive(false);
                    SRParticle.CustomSetActive(Data.PartnerData.HeroInfo.role_grade == (int)PartnerGrade.SR);
                    SSRParticle.CustomSetActive(Data.PartnerData.HeroInfo.role_grade >= (int)PartnerGrade.SSR);
                }
                CurBuddyId = Data.PartnerData.HeroId.ToString();
            }
        }

        public void ResetCardParticle()
        {
            Data = null;
            CurBuddyId = string.Empty;
            SSRParticle.CustomSetActive(false);
            SRParticle.CustomSetActive(false);
            LockObj.CustomSetActive(false);
            EmptyObj.CustomSetActive(false);
            FilledObj.CustomSetActive(false);
        }

        private void SetFilledState()
        {
            Hotfix_LT.Data.eRoleAttr type = Data.PartnerData.HeroInfo.char_type;
            PartnerGrade grade = (PartnerGrade)Data.PartnerData.HeroInfo.role_grade;
            int star = Data.PartnerData.Star;
            int awakelevel = Data.PartnerData.IsAwaken;
            TextureCamp.spriteName = Data.PartnerData.HeroInfo.skin;
            AttrSprite.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[type]; 
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, AttrSprite.transform, grade, type);
            GradeSprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[grade];

            for (int i = 0; i < StarGrid.transform.childCount; i++)
            {
                StarGrid.transform.GetChild(i).gameObject.CustomSetActive(i < star);
                starSpArray[i].spriteName = LTPartnerConfig.PARTNER_AWAKN_STAR_DIC[awakelevel];
            }
            StarGrid.repositionNow = true;
            URBorder.CustomSetActive(grade == PartnerGrade.UR);
            SSRBorder.CustomSetActive(grade == PartnerGrade.SSR);
            SRBorder.CustomSetActive(grade == PartnerGrade.SR);
            RBorder.CustomSetActive(grade == PartnerGrade.R);
        }

        public void OnItemClick()
        {
            if(Data == null)
            {
                EB.Debug.LogError("HandbookCardItem.OnItemClick Data == null");
                return;
            }
            //打开上阵界面或返回
            if (Data.State == HandbookCardState.Lock)
            {
                MessageTemplateManager.ShowMessage(eMessageUIType.FloatingText, EB.Localizer.GetString("ID_HANDBOOK_BTN_TIP1"));
                return;
            }
            GlobalMenuManager.Instance.Open("LTPartnerHandbookTakeTheFieldView", Data);
        }

        private void SetAttAddNum(LTPartnerData partnerData)
        {
            string Addtxt = string.Format("{0}{1}{2}", EB.Localizer.GetString(LTPartnerHandbookManager.Instance.GetType(Data.handbookId)), EB.Localizer.GetString("ID_PARTNER"), LTPartnerHandbookManager.Instance.GetAttAddNum(partnerData, Type));
            AttrAddSprite.color = (partnerData == null) ? Color.magenta : Color.white;
            AttrAddLabel.text = Addtxt;
        }

    }

}