using System.Collections;
using _HotfixScripts.Utils;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTHeroTipUIController : UIControllerHotfix, IHotfixUpdate
    {
        public LTShowItem ShowItem;
        public UILabel TypeLabel;
        public UISprite QualitySprite;
        public UISprite TypeSprite;
        public UILabel DescTextLabel;
        public BoxCollider Bg;
        public GameObject FullBg;

        #region 技能UI
        public DynamicUISprite CommonSkillBreakSprite;
        public UILabel CommonSkillBreakNameLabel;

        public DynamicUISprite PassiveSkillBreakSprite;
        public UILabel PassiveSkillBreakNameLabel;

        public DynamicUISprite ActiveSkillBreakSprite;
        public UILabel ActiveSkillBreakNameLabel;
        #endregion

        public static bool m_Open;

        private Vector4 Margin = Vector4.zero;
        private bool CheckMouseClick;
        private Hotfix_LT.Data.HeroStatTemplate heroStat;

        public override void SetMenuData(object param)
        {
            base.SetMenuData(param);
            Hashtable hashData = param as Hashtable;
            string id = hashData["id"].ToString();
            Vector2 screenPos = (Vector2)hashData["screenPos"];
            ShowInfo(id);
            SetSkillUI();
            SetAnchor(screenPos);

            bool showFullBg =false;
            if (hashData.Contains("showFullBg"))
            {
                showFullBg = (bool)hashData["showFullBg"];
                if (showFullBg)
                {
                    var t = controller.transform.GetChild(0);
                    t.position = Vector3.zero;
                    t.localPosition = Vector3.zero;
                }
            }
            FullBg.CustomSetActive(showFullBg); 
        }

        public override IEnumerator OnAddToStack()
        {
            m_Open = true;
            yield return base.OnAddToStack();
            CheckMouseClick = true;
        }

        public override IEnumerator OnRemoveFromStack()
        {
            m_Open = false;
            CheckMouseClick = false;
            DestroySelf();
            yield break;
        }

        public override void StartBootFlash()
        {
			SetCurrentPanelAlpha(1);
			UITweener[] tweeners = controller.transform.GetComponents<UITweener>();
            for (int j = 0; j < tweeners.Length; ++j)
            {
                tweeners[j].tweenFactor = 0;
                tweeners[j].PlayForward();
            }
        }

        public override void Awake()
        {
            base.Awake();
            CheckMouseClick = false;

            var t = controller.transform;
            ShowItem = t.GetMonoILRComponent<LTShowItem>("Content/LTShowItem");
            TypeLabel = t.GetComponent<UILabel>("Content/LTShowItem/TypeBg/Label");
            QualitySprite = t.GetComponent<UISprite>("Content/LTShowItem/Quality");
            TypeSprite = t.GetComponent<UISprite>("Content/LTShowItem/TypeBg/Sprite");
            DescTextLabel = t.GetComponent<UILabel>("Content/DescLabel");
            Bg = t.GetComponent<BoxCollider>("Content/BG");
            FullBg = t.FindEx("Content/FullBG").gameObject;
            CommonSkillBreakSprite = t.GetComponent<DynamicUISprite>("Content/Grid/CommonSkill/SkillItem/Icon");
            CommonSkillBreakNameLabel = t.GetComponent<UILabel>("Content/Grid/CommonSkill/NameLabel");
            PassiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("Content/Grid/PassiveSkill/SkillItem/Icon");
            PassiveSkillBreakNameLabel = t.GetComponent<UILabel>("Content/Grid/PassiveSkill/NameLabel");
            ActiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("Content/Grid/ActiveSkill/SkillItem/Icon");
            ActiveSkillBreakNameLabel = t.GetComponent<UILabel>("Content/Grid/ActiveSkill/NameLabel");

            t.GetComponent<UIButton>("Content/Grid/CommonSkill/SkillItem").onClick.Add(new EventDelegate(OnCommonSkillItemClick));
            t.GetComponent<UIButton>("Content/Grid/ActiveSkill/SkillItem").onClick.Add(new EventDelegate(OnActiveSkillClick));
            t.GetComponent<UIButton>("Content/Grid/PassiveSkill/SkillItem").onClick.Add(new EventDelegate(OnPassiveSkillClick));
        }

		public override void OnEnable()
		{
			RegisterMonoUpdater();
		}

		public void Update()
        {
            if (CheckMouseClick && (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)))
            {
                if (UIStack.Instance.IsTop(controller) && !Bg.GetComponent<BoxCollider>().bounds.Contains(UICamera.lastWorldPosition))
                {
                    CheckMouseClick = false;
                    controller.Close();
                }
            }
        }

        void ShowInfo(string id)
        {
            bool isCharacterid = Hotfix_LT.Data.CharacterTemplateManager.Instance.HasHeroInfo(int.Parse(id));
            if (isCharacterid)
            {
                var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(id);
                heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(int.Parse(id) + 1);
                if (charTpl != null)
                {
                    ShowItem.LTItemData = new LTShowItemData(id, 1, LTShowItemType.TYPE_HERO, true);
                    LTUIUtil.SetText(TypeLabel, string.Format("{0}", charTpl.role_profile == null ? EB.Localizer.GetString("ID_NATION_BATTLE_BUFF_FULL_CALL") : charTpl.role_profile));
                    TypeSprite.spriteName = charTpl.role_profile_icon;
                    QualitySprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)charTpl.role_grade]; ;
                    LTUIUtil.SetText(DescTextLabel, charTpl.role_profile_text == null ? EB.Localizer.GetString("ID_NATION_BATTLE_BUFF_FULL_CALL") : charTpl.role_profile_text);
                }
            }
            else //这里做兼容，之前小伙伴没有品级，所有的id 都是character_id 现在id有可能有template_id
            {
                heroStat = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(id);
                if (heroStat != null)
                {
                    var charTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroStat.character_id);
                    if (charTpl != null)
                    {
                        ShowItem.LTItemData = new LTShowItemData(id, 1, LTShowItemType.TYPE_HERO, true);
                        TypeSprite.spriteName = charTpl.role_profile_icon;
                        QualitySprite.spriteName = LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)charTpl.role_grade]; ;
                        LTUIUtil.SetText(TypeLabel, string.Format("{0}", charTpl.role_profile == null ? EB.Localizer.GetString("ID_NATION_BATTLE_BUFF_FULL_CALL") : charTpl.role_profile));
                        LTUIUtil.SetText(DescTextLabel, charTpl.role_profile_text == null ? EB.Localizer.GetString("ID_NATION_BATTLE_BUFF_FULL_CALL") : charTpl.role_profile_text);
                    }
                }
            }
        }

        void SetAnchor(Vector2 screenPos)
        {
            var t = controller.transform.GetChild(0);
            Vector3 worldPos = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(Mathf.Clamp(screenPos.x, Bg.GetComponent<UIWidget>().width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2, (Screen.width - Bg.GetComponent<UIWidget>().width * ((float)Screen.width / (float)UIRoot.list[0].manualWidth) / 2)), screenPos.y));
            Bounds abs = NGUIMath.CalculateAbsoluteWidgetBounds(t);
            float aspect = (float)Screen.width / (float)Screen.height;
            Vector4 worldMargin = Margin * 2.0f / (float)UIRoot.list[0].manualHeight;
            worldPos.x = Mathf.Clamp(worldPos.x, -aspect + worldMargin.x, aspect - worldMargin.y);
            Vector3 currentPos = t.position;
            currentPos.x = worldPos.x;
            /*if (worldPos.y >= 1f - worldMargin.w - abs.size.y - 0.2f)
            {
                currentPos.y = worldPos.y - abs.size.y / 2 - 0.1f;
            }
            else
            {
                currentPos.y = worldPos.y + abs.size.y / 2 + 0.1f;
            }*/

            t.position = currentPos;
            t.localPosition = new Vector2(t.localPosition.x, 0);
        }

        private void SetSkillUI()
        {
            Hotfix_LT.Data.SkillTemplate commonSkillTem = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(heroStat.common_skill);
            if (commonSkillTem != null)
            {
                CommonSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                CommonSkillBreakSprite.spriteName = commonSkillTem.Icon;
                CommonSkillBreakNameLabel.text = commonSkillTem.Name;
            }
            else
            {
                CommonSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }

            Hotfix_LT.Data.SkillTemplate passiveSkillTem = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(heroStat.passive_skill);
            if (passiveSkillTem != null)
            {
                PassiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                PassiveSkillBreakSprite.spriteName = passiveSkillTem.Icon;
                PassiveSkillBreakNameLabel.text = passiveSkillTem.Name;
            }
            else
            {

                PassiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }

            Hotfix_LT.Data.SkillTemplate activeSkillTem = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(heroStat.active_skill);
            if (activeSkillTem != null)
            {
                ActiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                ActiveSkillBreakSprite.spriteName = activeSkillTem.Icon;
                ActiveSkillBreakNameLabel.text = activeSkillTem.Name;
            }
            else
            {
                ActiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="lab"></param>
        /// <param name="sp"></param>
        private void SetSkillType(Hotfix_LT.Data.eSkillType skillType, UILabel lab, UISprite sp)
        {
            if (skillType == Hotfix_LT.Data.eSkillType.ACTIVE)
            {
                lab.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4070");
                sp.color = LT.Hotfix.Utility.ColorUtility.RedColor;
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.NORMAL)
            {
                lab.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4440");
                sp.color = LT.Hotfix.Utility.ColorUtility.PurpleColor;
            }
            else if (skillType == Hotfix_LT.Data.eSkillType.PASSIVE)
            {
                lab.text = EB.Localizer.GetString("ID_codefont_in_UISkillDescContorller_4751");
                sp.color = LT.Hotfix.Utility.ColorUtility.BlueColor;
            }
        }

        /// <summary>
        /// 普攻
        /// </summary>
        public void OnActiveSkillClick()
        {
            string dataId = heroStat.active_skill.ToString() + "," + "1";
            UITooltipManager.Instance.DisplaySkillTip(dataId, new Vector3(465, 0, 0), ePressTipAnchorType.RightUp);
        }

        /// <summary>
        /// 奥义
        /// </summary>
        public void OnCommonSkillItemClick()
        {
            string dataId = heroStat.common_skill.ToString() + "," + "1";
            UITooltipManager.Instance.DisplaySkillTip(dataId, new Vector3(465, 0, 0), ePressTipAnchorType.RightUp);
        }

        /// <summary>
        /// 被动或另一个奥义
        /// </summary>
        public void OnPassiveSkillClick()
        {
            string dataId = heroStat.passive_skill.ToString() + "," + "1";
            UITooltipManager.Instance.DisplaySkillTip(dataId, new Vector3(465, 0, 0), ePressTipAnchorType.RightUp);
        }
    }
}
