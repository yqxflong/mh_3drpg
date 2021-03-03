using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = EB.Debug;

namespace Hotfix_LT.UI
{
    public class LTPartnerInfoController : UIControllerHotfix
    {
        public override void Awake()
        {
            base.Awake();

            var t = controller.transform;
            OtherPlayerName = t.GetComponent<UILabel>("BG/Title/PlayerName");
             equipments = new LTPartnerInfoEquipmentCell[6];
             LTPartnerInfoEquipmentCell  info1= controller.transform.Find("Equip/item").GetMonoILRComponent<LTPartnerInfoEquipmentCell>();
             LTPartnerInfoEquipmentCell  info2= controller.transform.Find("Equip/item (1)").GetMonoILRComponent<LTPartnerInfoEquipmentCell>();
             LTPartnerInfoEquipmentCell  info3= controller.transform.Find("Equip/item (2)").GetMonoILRComponent<LTPartnerInfoEquipmentCell>();
             LTPartnerInfoEquipmentCell  info4= controller.transform.Find("Equip/item (3)").GetMonoILRComponent<LTPartnerInfoEquipmentCell>();
             LTPartnerInfoEquipmentCell  info5= controller.transform.Find("Equip/item (4)").GetMonoILRComponent<LTPartnerInfoEquipmentCell>();
             LTPartnerInfoEquipmentCell info6 = controller.transform.Find("Equip/item (5)").GetMonoILRComponent<LTPartnerInfoEquipmentCell>();
             equipments[0] = info1;
             equipments[1] = info2;
             equipments[2] = info3;
             equipments[3] = info4;
             equipments[4] = info5;
             equipments[5] = info6;
            CommonFrame = t.GetComponent<UISprite>("HeroIcon/Skill/CommonSkill/SkillItem");
            CommonSkillBreakSprite = t.GetComponent<DynamicUISprite>("HeroIcon/Skill/CommonSkill/SkillItem/Icon");
            CommonSkillBreakNameLabel = t.GetComponent<UILabel>("HeroIcon/Skill/CommonSkill/NameLabel");
            CommonSkillLevel = t.GetComponent<UILabel>("HeroIcon/Skill/CommonSkill/SkillItem/Sprite/Level");
            PassiveFrame = t.GetComponent<UISprite>("HeroIcon/Skill/PassiveSkill/SkillItem");
            PassiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("HeroIcon/Skill/PassiveSkill/SkillItem/Icon");
            PassiveSkillBreakNameLabel = t.GetComponent<UILabel>("HeroIcon/Skill/PassiveSkill/NameLabel");
            PassiveSkillLevel = t.GetComponent<UILabel>("HeroIcon/Skill/PassiveSkill/SkillItem/Sprite/Level");
            ActiveFrame = t.GetComponent<UISprite>("HeroIcon/Skill/ActiveSkill/SkillItem");
            ActiveSkillBreakSprite = t.GetComponent<DynamicUISprite>("HeroIcon/Skill/ActiveSkill/SkillItem/Icon");
            ActiveSkillBreakNameLabel = t.GetComponent<UILabel>("HeroIcon/Skill/ActiveSkill/NameLabel");
            ActiveSkillLevel = t.GetComponent<UILabel>("HeroIcon/Skill/ActiveSkill/SkillItem/Sprite/Level");
            AttackLabel = t.GetComponent<UILabel>("Info/AttrList/0/NumLabel");
            DefLabel = t.GetComponent<UILabel>("Info/AttrList/1/NumLabel");
            LifeLabel = t.GetComponent<UILabel>("Info/AttrList/2/NumLabel");
            CritLabel = t.GetComponent<UILabel>("Info/AttrList/3/NumLabel");
            CritVLabel = t.GetComponent<UILabel>("Info/AttrList/4/NumLabel");
            ComboLabel = t.GetComponent<UILabel>("Info/AttrList/5/NumLabel");
            SpExtraLabel = t.GetComponent<UILabel>("Info/AttrList/6/NumLabel");
            SpResLabel = t.GetComponent<UILabel>("Info/AttrList/7/NumLabel");
            HeroIcon = t.GetComponent<UISprite>("HeroIcon/Item/Icon");
            HeroType = t.GetComponent<UISprite>("HeroIcon/Item/Property");
            HeroIconBorder = t.GetComponent<UISprite>("HeroIcon/Item/Frame");
            HeriIconBorderBG = t.GetComponent<UISprite>("HeroIcon/Item/FrameBG");
            GradeIcon = t.GetComponent<UISprite>("HeroIcon/Item/HeroGrade");
            StarList = t.GetMonoILRComponent<LTPartnerStarController>("HeroIcon/Item/StarList");
            BreakLabel = t.GetComponent<UILabel>("HeroIcon/Item/BreakObj/Break");
            HeroLevel = t.GetComponent<UILabel>("HeroIcon/Item/LevelSprite/LabelLevel");
            HeroRoleProfile = t.GetComponent<UILabel>("HeroIcon/Sprite/Label");
            HeroRoleProfileSprite = t.GetComponent<UISprite>("HeroIcon/Sprite");
            HeroName = t.GetComponent<UILabel>("HeroIcon/heroName");
            CombatPower = t.GetComponent<UILabel>("HeroIcon/Item/CombatPower/Container/Base");
            EquipAttackLabel = t.GetComponent<UILabel>("Info/AttrList/0/EquipNumLabel");
            EquipDefLabel = t.GetComponent<UILabel>("Info/AttrList/1/EquipNumLabel");
            EquipLifeLabel = t.GetComponent<UILabel>("Info/AttrList/2/EquipNumLabel");
            EquipCritLabel = t.GetComponent<UILabel>("Info/AttrList/3/EquipNumLabel");
            EquipCritVLabel = t.GetComponent<UILabel>("Info/AttrList/4/EquipNumLabel");
            EquipComboLabel = t.GetComponent<UILabel>("Info/AttrList/5/EquipNumLabel");
            EquipSpExtraLabel = t.GetComponent<UILabel>("Info/AttrList/6/EquipNumLabel");
            EquipSpResLabel = t.GetComponent<UILabel>("Info/AttrList/7/EquipNumLabel");
            ClickItemCell = t.GetMonoILRComponent<LTPartnerInfoEquipmentCell>("Equip/EquipmentInfo/AA");
            ClickItemTitle = t.GetComponent<UILabel>("Equip/EquipmentInfo/TitleName");
            EffectPos = t.GetComponent<UIWidget>("Equip/EquipmentInfo/EffectInfoPos/EffectPos");
            Effect2BgPos = t.GetComponent<UIWidget>("Equip/EquipmentInfo/EffectInfoPos/EffectPos/Infor (1)/Effect_2/Sprite");
            Effect4BgPos = t.GetComponent<UIWidget>("Equip/EquipmentInfo/EffectInfoPos/EffectPos/Infor (1)/Effect_4/Sprite");
            Effect_2Label = t.GetComponent<UILabel>("Equip/EquipmentInfo/EffectInfoPos/EffectPos/Infor (1)/Effect_2");
            Effect_4Label = t.GetComponent<UILabel>("Equip/EquipmentInfo/EffectInfoPos/EffectPos/Infor (1)/Effect_4");
            MainAttr = t.GetComponent<Transform>("Equip/EquipmentInfo/Infor/MainAttr");

            ExAttr = new Transform[4];
            ExAttr[0] = t.GetComponent<Transform>("Equip/EquipmentInfo/Infor/ExAttr (1)");
            ExAttr[1] = t.GetComponent<Transform>("Equip/EquipmentInfo/Infor/ExAttr (2)");
            ExAttr[2] = t.GetComponent<Transform>("Equip/EquipmentInfo/Infor/ExAttr (3)");
            ExAttr[3] = t.GetComponent<Transform>("Equip/EquipmentInfo/Infor/ExAttr (4)");

            infoPanel = t.FindEx("Equip/EquipmentInfo").gameObject;

            t.GetComponent<UIButton>("BG/Title/CloseBtn").onClick.Add(new EventDelegate(OnCancelButtonClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("HeroIcon/Skill/CommonSkill/SkillItem").clickEvent.Add(new EventDelegate(OnCommonSkillItemClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("HeroIcon/Skill/ActiveSkill/SkillItem").clickEvent.Add(new EventDelegate(OnActiveSkillClick));
            t.GetComponent<ConsecutiveClickCoolTrigger>("HeroIcon/Skill/PassiveSkill/SkillItem").clickEvent.Add(new EventDelegate(OnPassiveSkillClick));
            t.GetComponent<UIButton>("Equip/EquipmentInfo/Sprite (1)").onClick.Add(new EventDelegate(OnCloseEquipPanel));
        }

        public UILabel OtherPlayerName;
        public LTPartnerInfoEquipmentCell[] equipments;
        public override bool ShowUIBlocker { get { return true; } }
        #region 技能UI
        public UISprite CommonFrame;
        public DynamicUISprite CommonSkillBreakSprite;
        public UILabel CommonSkillBreakNameLabel;
        public UILabel CommonSkillLevel;
    
        public UISprite PassiveFrame;
        public DynamicUISprite PassiveSkillBreakSprite;
        public UILabel PassiveSkillBreakNameLabel;
        public UILabel PassiveSkillLevel;
    
        public UISprite ActiveFrame;
        public DynamicUISprite ActiveSkillBreakSprite;
        public UILabel ActiveSkillBreakNameLabel;
        public UILabel ActiveSkillLevel;
        #endregion
    
        //属性
        public UILabel AttackLabel;
        public UILabel DefLabel;
        public UILabel LifeLabel;
        public UILabel CritLabel;
        public UILabel CritVLabel;
        public UILabel ComboLabel;
        public UILabel SpExtraLabel;
        public UILabel SpResLabel;
        private UIWidget EffectPos, Effect2BgPos, Effect4BgPos;

        //头像
        public UISprite HeroIcon;
        public UISprite HeroType;
        public UISprite HeroIconBorder;
        public UISprite HeriIconBorderBG;
        public UISprite GradeIcon;
        public LTPartnerStarController StarList;
        public UILabel BreakLabel;
        public UILabel HeroLevel;
        public UILabel HeroRoleProfile;
        public UISprite HeroRoleProfileSprite;
        public UILabel HeroName;
        public UILabel CombatPower;
    
        private LTAttributesData currentAttr;
    
        private OtherPlayerPartnerData data;
    
    
        private int commonSkillId, activateSkillId, passiveSkillId;
    
        public override IEnumerator OnAddToStack()
        {
            
    
            return base.OnAddToStack();
        }
    
        public override void SetMenuData(object param)
        {
            InitUI();
            data = param as OtherPlayerPartnerData;
            if (data == null)
            {
                EB.Debug.LogError("LTPartnerInfoController.SetMenuData data == null");
                return;
            }
            LTUIUtil.SetText(OtherPlayerName, data.otherPlayerName);
            LTUIUtil.SetText(CombatPower, ((int)data.GetOtherPower()).ToString());//需要再装备显示之前，因为这里边有装备属性数据的初始化
            SetHeroIcon(data.Name, data.UpGradeId, data.Icon, data.Attr, data.QualityLevel, data.Star, data.Level+data.AllRoundLevel, data.RoleProflie,data.RoleProflieSprite,data.awakenLevel);
            SetSkillInfo(data.commonSkill, data.commonSkillLevel, data.activeSkill, data.activeSkillLevel, data.passiveSkill, data.passiveSkilLevel,data.InfoId,data.awakenLevel);
            SetHeroEquipment(data.equipmentList);
            SetHeroAttr(data.FinalAttributes);
            SetEquipmentAttr(data.equipmentAdds);
        }
    
        void InitUI()
        {
            AttackLabel.text = "0";
            DefLabel.text = "0";
            LifeLabel.text = "0";
            CritLabel.text = 0 + "%";
            CritVLabel.text = "0" + "%";
            ComboLabel.text = "0";//由连击改为速度
            SpExtraLabel.text = "0" + "%";
            SpResLabel.text = "0" + "%";
        }
    
        /// <summary>
        /// 技能图标
        /// </summary>
        /// <param name="commonSkill"></param>
        /// <param name="commonSkillLevel"></param>
        /// <param name="activeSkill"></param>
        /// <param name="activeSkillLevel"></param>
        /// <param name="passiveSkill"></param>
        /// <param name="passiveSkillLevel"></param>
        /// <param name="infoId">用于读取觉醒技能替换信息</param>
        /// <param name="awakenLevel">是否觉醒</param>>
        private void SetSkillInfo(int commonSkill, int commonSkillLevel, int activeSkill, int activeSkillLevel, int passiveSkill, int passiveSkillLevel,int infoId,int awakenLevel)
        {
            SkillSetTool.SkillFrameStateSet(CommonFrame, false);
            SkillSetTool.SkillFrameStateSet(PassiveFrame, false);
            SkillSetTool.SkillFrameStateSet(ActiveFrame, false);
            if (awakenLevel > 0)
            {
    
                Hotfix_LT.Data.HeroAwakeInfoTemplate template = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(infoId);
                if (template.beforeSkill == commonSkill)
                {
                    commonSkill = template.laterSkill;
                    SkillSetTool.SkillFrameStateSet(CommonFrame, true);
                }
                if (template.beforeSkill == activeSkill)
                {
                    activeSkill = template.laterSkill;
                    SkillSetTool.SkillFrameStateSet(ActiveFrame, true);
    
                }
                if (template.beforeSkill == passiveSkill)
                {
                    passiveSkill = template.laterSkill;
                    SkillSetTool.SkillFrameStateSet(PassiveFrame, true);
                }
            }
            commonSkillId = commonSkill;
            activateSkillId = activeSkill;
            passiveSkillId = passiveSkill;
            Hotfix_LT.Data.SkillTemplate commonSkillTem = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(commonSkill);
            if (commonSkillTem != null)
            {
                CommonSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                CommonSkillBreakSprite.spriteName = commonSkillTem.Icon;
                CommonSkillBreakNameLabel.text = commonSkillTem.Name;
                CommonSkillLevel.text = commonSkillLevel.ToString();
            }
            else
            {
                CommonSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }
    
            Hotfix_LT.Data.SkillTemplate passiveSkillTem = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(passiveSkill);
            if (passiveSkillTem != null)
            {
                PassiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                PassiveSkillBreakSprite.spriteName = passiveSkillTem.Icon;
                PassiveSkillBreakNameLabel.text = passiveSkillTem.Name;
                PassiveSkillLevel.text = passiveSkillLevel.ToString();
            }
            else
            {
    
                PassiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }
    
            Hotfix_LT.Data.SkillTemplate activeSkillTem = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(activeSkill);
            if (activeSkillTem != null)
            {
                ActiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(true);
                ActiveSkillBreakSprite.spriteName = activeSkillTem.Icon;
                ActiveSkillBreakNameLabel.text = activeSkillTem.Name;
                ActiveSkillLevel.text = activeSkillLevel.ToString();
            }
            else
            {
                ActiveSkillBreakNameLabel.transform.parent.gameObject.CustomSetActive(false);
            }
        }
    
    
        private Dictionary<int, DetailedEquipmentInfo> equipmentAttrs = new Dictionary<int, DetailedEquipmentInfo>();
    
        /// <summary>
        /// 装备栏
        /// </summary>
        /// <param name="equipmentInfo"></param>
        private void SetHeroEquipment(Hashtable equipmentInfo)
        {
            for (int i = 0; i < equipments.Length; i++)
            {
                equipments[i].SetCellData(this, -1, -1, -1);
            }
    
            if (equipmentInfo == null)
            {
                EB.Debug.Log("null equipment");
                return;;
            }
            
            foreach (Hashtable info in equipmentInfo.Values)
            {
                if (info==null||info["equipment_type"] == null)
                {
                    continue;
                }
                int equipmentType = int.Parse(info["equipment_type"].ToString());
                int eid = int.Parse(info["economy_id"].ToString());
                int equipmentLevel = EB.Dot.Integer("level", info, 0);
    
                DetailedEquipmentInfo einfo = info.ContainsKey("currentLevel") ? LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(info) :
					LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(int.Parse(info["inventory_id"].ToString()));
                
                if (!equipmentAttrs.ContainsKey(eid))
                {
                    equipmentAttrs.Add(eid, einfo);
                }
                else
                {
                    equipmentAttrs[eid] = einfo;
                }
    
                equipments[equipmentType-1].SetCellData(this, eid, equipmentType, equipmentLevel);
            }
        }
    
        /// <summary>
        /// 伙伴基础属性
        /// </summary>
        /// <param name="attributes"></param>
        private void SetHeroAttr(LTAttributesData attributes)
        {
            currentAttr = attributes;
            AttackLabel.text = ((int)attributes.m_ATK).ToString("f0");
            DefLabel.text = ((int)attributes.m_DEF).ToString("f0");
            LifeLabel.text = ((int)attributes.m_MaxHP).ToString("f0");
            double value = attributes.m_CritP * 100;
            CritLabel.text = value.ToString("f0") + "%";
            value = attributes.m_CritV * 100;
            CritVLabel.text = value.ToString("f0") + "%";
            ComboLabel.text = ((int)(attributes.m_Speed)).ToString("f0");//由连击改为速度
            value = attributes.m_SpExtra * 100;
            SpExtraLabel.text = value.ToString("f0") + "%";
            value = attributes.m_SpRes * 100;
            SpResLabel.text = value.ToString("f0") + "%";
        }
    
    
        public override IEnumerator OnRemoveFromStack()
        {
            DestroySelf();
            yield break;
        }
    
        public UILabel EquipAttackLabel;
        public UILabel EquipDefLabel;
        public UILabel EquipLifeLabel;
        public UILabel EquipCritLabel;
        public UILabel EquipCritVLabel;
        public UILabel EquipComboLabel;
        public UILabel EquipSpExtraLabel;
        public UILabel EquipSpResLabel;
    
    
        /// <summary>
        /// 伙伴装备属性
        /// </summary>
        private void SetEquipmentAttr(Dictionary<string, float> equipmentAdds)
        {
            LTUIUtil.SetText(EquipAttackLabel, "");
            LTUIUtil.SetText(EquipDefLabel, "");
            LTUIUtil.SetText(EquipLifeLabel, "");
            LTUIUtil.SetText(EquipComboLabel, "");
            LTUIUtil.SetText(EquipCritLabel, "");
            LTUIUtil.SetText(EquipCritVLabel, "");
            LTUIUtil.SetText(EquipSpExtraLabel, "");
            LTUIUtil.SetText(EquipSpResLabel, "");
            if (equipmentAdds.Count == 0)
            {
                EB.Debug.Log("No equipment in this Hero");
                return;
            }
    
            foreach (string attrAdd in equipmentAdds.Keys)
            {
                float add = float.Parse(equipmentAdds[attrAdd].ToString());
                switch (attrAdd)
                {
                    case "ATK": LTUIUtil.SetText(EquipAttackLabel, add <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(add))); break;
                    case "MaxHP": LTUIUtil.SetText(EquipLifeLabel, add <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(add))); break;
                    case "DEF": LTUIUtil.SetText(EquipDefLabel, add <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(add))); break;
                    case "CritP": LTUIUtil.SetText(EquipCritLabel, add <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt(add * 100))); break;
                    case "CritV": LTUIUtil.SetText(EquipCritVLabel, add <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt(add * 100))); break;
                    case "SpExtra": LTUIUtil.SetText(EquipSpExtraLabel, add <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt(add * 100))); break;
                    case "SpRes": LTUIUtil.SetText(EquipSpResLabel, add <= 0 ? "" : string.Format("+{0}%", Mathf.FloorToInt(add * 100))); break;
                    case "speed": LTUIUtil.SetText(EquipComboLabel, add <= 0 ? "" : string.Format("+{0}", Mathf.FloorToInt(add))); break;
                    default: EB.Debug.LogWarning("Equipment MainAttribute Miss{0}",attrAdd); break;
                }
            }
        }
    
        private ParticleSystemUIComponent charFx, upgradeFx;
        private EffectClip efClip, upgradeefclip;
        /// <summary>
        /// 设置英雄头像
        /// </summary>
        /// <param name="UpGradeId"></param>
        /// <param name="icon"></param>
        /// <param name="char_type"></param>
        /// <param name="role_grade"></param>
        /// <param name="star"></param>
        /// <param name="heroLevel"></param>
        private void SetHeroIcon(string heroName, int UpGradeId, string icon, Hotfix_LT.Data.eRoleAttr char_type , int role_grade , int star , int heroLevel, string heroProfile,string profileSprite,int isawaken)
        {
            int quality = 0;
            int addLevel = 0;
            LTPartnerDataManager.GetPartnerQuality(UpGradeId, out quality, out addLevel);
            if (addLevel == 0)
            {
                LTUIUtil.SetText(BreakLabel, "");
            }
            else
            {
                LTUIUtil.SetText(BreakLabel, "+" + addLevel.ToString());
            }
            BreakLabel.gameObject.SetActive(addLevel != 0);
            LTUIUtil.SetLevelText(HeroLevel.transform.parent.GetComponent<UISprite>(),HeroLevel, heroLevel);
            LTUIUtil.SetText(HeroName, heroName);
            LTUIUtil.SetText(HeroRoleProfile, string.Format( "{0}", heroProfile == null ? EB.Localizer.GetString("ID_NATION_BATTLE_BUFF_FULL_CALL") : heroProfile));
            HeroRoleProfileSprite.spriteName = profileSprite;
            HeroIcon.spriteName = icon;
            HeroType.spriteName = LTPartnerConfig.LEVEL_SPRITE_NAME_DIC[char_type]; 
            HeroIconBorder.spriteName = LTPartnerConfig.OUT_LINE_SPRITE_NAME_DIC[quality];
            GameItemUtil.SetColorfulPartnerCellFrame(quality, HeriIconBorderBG);
            HotfixCreateFX.ShowUpgradeQualityFX(upgradeFx, HeroIconBorder.transform, quality, upgradeefclip);
            GradeIcon.spriteName =LTPartnerConfig.PARTNER_GRADE_SPRITE_NAME_DIC[(PartnerGrade)role_grade];
            HotfixCreateFX.ShowCharTypeFX(charFx, efClip, HeroType.transform, (PartnerGrade)role_grade, char_type);
            StarList.SetSrarList(star, isawaken);
        }
    
    
        /// <summary>
        /// 普攻
        /// </summary>
        public void OnActiveSkillClick()
        {
            string dataId = activateSkillId.ToString() + "," + data.activeSkillLevel;
            UITooltipManager.Instance.DisplaySkillTip(dataId, Vector3.zero, ePressTipAnchorType.RightUp);
        }
    
        /// <summary>
        /// 奥义
        /// </summary>
        public void OnCommonSkillItemClick()
        {
            string dataId = commonSkillId.ToString() + "," + data.commonSkillLevel;
            UITooltipManager.Instance.DisplaySkillTip(dataId, Vector3.zero, ePressTipAnchorType.RightUp);
        }
    
        /// <summary>
        /// 被动或另一个奥义
        /// </summary>
        public void OnPassiveSkillClick()
        {
            string dataId = passiveSkillId.ToString() + "," + data.passiveSkilLevel;
            UITooltipManager.Instance.DisplaySkillTip(dataId, Vector3.zero, ePressTipAnchorType.RightUp);
        }
    
        public LTPartnerInfoEquipmentCell ClickItemCell;
        public UILabel ClickItemTitle;
        public UILabel Effect_2Label, Effect_4Label;
        public Transform MainAttr;
        public Transform[] ExAttr;
        public GameObject infoPanel;
    
        public void OnCloseEquipPanel()
        {
            infoPanel.gameObject.SetActive(false);
        }
    
        public void OnEquipmentClick(int Eid , int equipmentLevel, int equipmentType)
        {
            infoPanel.gameObject.SetActive(true);
            infoPanel.GetComponent<TweenScale>().ResetToBeginning();
            infoPanel.GetComponent<TweenScale>().PlayForward();
            ClickItemCell.SetCellData(this, Eid, equipmentType, equipmentLevel);
            Hotfix_LT.Data.EquipmentItemTemplate tpl = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetEquipment(Eid);
            DetailedEquipmentInfo attr = equipmentAttrs[Eid];
            Effect_4Label.effectStyle = UILabel.Effect.None;
            Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
            ClickItemTitle.applyGradient = true;
            ClickItemTitle.gradientTop = LT.Hotfix.Utility.ColorUtility.QualityToGradientTopColor(tpl.QualityLevel);
            ClickItemTitle.gradientBottom = LT.Hotfix.Utility.ColorUtility.QualityToGradientBottomColor(tpl.QualityLevel);
            ClickItemTitle.text = ClickItemTitle.transform.GetChild(0).GetComponent<UILabel>().text = tpl.Name;
            Hotfix_LT.Data.SkillTemplate suitAttr1 = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(tpl.SuitAttrId_1);
            Hotfix_LT.Data.SkillTemplate suitAttr2 = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(tpl.SuitAttrId_2);
            int need = 0;
            int SuitAttrId = -1;
            if (suitAttr1 != null)
            {
                need = 2;
                string FirstSuitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(tpl.SuitAttrId_1).Description;
                SuitAttrId = tpl.SuitAttrId_1;
                if(data.equipmentSuits.Count > 0) LTUIUtil.SetText(Effect_4Label ,string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_1924"), LTPartnerEquipConfig.HasEffectStrDic[data.equipmentSuits[SuitAttrId] >= need], FirstSuitAttr));
            }   
            else if (suitAttr2 != null)
            {
                need = 4;
                SuitAttrId = tpl.SuitAttrId_2;
                string SecondSuitAttr = Hotfix_LT.Data.SkillTemplateManager.Instance.GetTemplate(tpl.SuitAttrId_2).Description;
                LTUIUtil.SetText(Effect_4Label, string.Format(EB.Localizer.GetString("ID_codefont_in_LTEquipmentFirstInfo_2136"), LTPartnerEquipConfig.HasEffectStrDic[data.equipmentSuits[SuitAttrId] >= need], SecondSuitAttr));
            }
    
            if (data.equipmentSuits.Count > 0 && data.equipmentSuits[SuitAttrId] >= need)
            {
                Effect_4Label.effectStyle = UILabel.Effect.Outline8;
                Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.74f, 1f, 0.85f);
            }
            else
            {
                Effect_4Label.effectStyle = UILabel.Effect.None;
                Effect_4Label.transform.GetChild(1).GetComponent<UISprite>().color = new Color(0.75f, 0.75f, 0.75f);
            }
    
            EquipmentAttr MainAttributes = attr.MainAttributes;
            string MainStr = AttrTypeTrans(MainAttributes.Name);
            MainAttr.GetChild(0).GetComponent<UILabel>().text = "[fff348]" + MainStr;
            MainAttr.GetChild(1).GetComponent<UILabel>().text = AttrTypeValue(MainAttributes.Name, MainAttributes.Value);
            
            List<EquipmentAttr> exadd = attr.ExAttributes;
            int exCount = exadd.Count;
            for (int i = 0; i < 4; i++)
            {
                if (i > exCount - 1)
                {
                    ExAttr[i].gameObject.CustomSetActive(false);
                }
                else
                {
                    EquipmentAttr add = exadd[i];
                    string exAddName = add.Name;
                    float exAddValue = add.Value;
                    string ExNameStr = AttrTypeTrans(exAddName);
                    ExAttr[i].GetChild(0).GetComponent<UILabel>().text = ExNameStr;
                    ExAttr[i].GetChild(1).GetComponent<UILabel>().text = AttrTypeValue(exAddName, exAddValue);
                    ExAttr[i].gameObject.CustomSetActive(true);
                }
            }
            infoPanel.GetComponent<UIWidget>().height = 413 + 88 * exCount + ((Effect_4Label.gameObject.activeSelf) ? Effect_4Label.height : 0);
            EffectPos.UpdateAnchors();
            Effect2BgPos.UpdateAnchors();
            Effect4BgPos.UpdateAnchors();
        }
    
        static public string AttrTypeTrans(string str)
        {
            switch (str)
            {
                case "ATK": return EB.Localizer.GetString("ID_ATTR_ATK") + "：";
                case "MaxHP": return EB.Localizer.GetString("ID_ATTR_MaxHP") + "：";
                case "DEF": return EB.Localizer.GetString("ID_ATTR_DEF") + "：";
                case "CritP": return EB.Localizer.GetString("ID_ATTR_CritP") + "：";
                case "CritV": return EB.Localizer.GetString("ID_ATTR_CritV") + "：";
                case "ChainAtk": return EB.Localizer.GetString("ID_ATTR_ChainAtk") + "：";
                case "SpExtra": return EB.Localizer.GetString("ID_ATTR_SpExtra") + "：";
                case "SpRes": return EB.Localizer.GetString("ID_ATTR_SpRes") + "：";
                case "MaxHPrate": return EB.Localizer.GetString("ID_ATTR_MaxHPrate") + "：";
                case "ATKrate": return EB.Localizer.GetString("ID_ATTR_ATKrate") + "：";
                case "DEFrate": return EB.Localizer.GetString("ID_ATTR_DEFrate") + "：";
                case "Speed": return EB.Localizer.GetString("ID_ATTR_Speed") + "：";
                case "speedrate": return EB.Localizer.GetString("ID_ATTR_speedrate") + "：";
                default: return EB.Localizer.GetString("ID_ATTR_Unknown") + "：";
            }
        }
    
        public string AttrTypeValue(string name, float value)
        {
            switch (name)
            {
                case "ATK": return ("+" + Mathf.FloorToInt(value).ToString());
                case "MaxHP": return ("+" + Mathf.FloorToInt(value).ToString()); 
                case "DEF": return ("+" + Mathf.FloorToInt(value).ToString()); 
                case "CritP": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "CritV": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "ChainAtk": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "SpExtra": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "SpRes": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "MaxHPrate": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "ATKrate": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "DEFrate": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "Speed": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                case "speedrate": return ("+" + Mathf.FloorToInt(value * 100).ToString() + "%"); 
                default: return EB.Localizer.GetString("ID_ATTR_Unknown") + "：";
            }
        }
    }
}
