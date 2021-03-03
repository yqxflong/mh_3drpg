using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EB;
using GM.DataCache;
using Hotfix_LT.Data;
using Hotfix_LT.Player;
using SkillTemplate = Hotfix_LT.Data.SkillTemplate;

namespace Hotfix_LT.UI
{
    public class AttributesManager
    {
        /// <summary>[伙伴基础白字属性]</summary>
        public static LTAttributesData GetPartnerBaseAttributesByParnterData(int heroStatId, int heroUpgradeId, int star, int starHole, int heroLevel, int AllRoundLevel, int ControlLevel, int StrongLevel, int RageLevel, int AbsorbedLevel, int AwakenLevel, int infoId, IDictionary fetter, int commonSkillLevel, int activeSkillLevel, int passiveSkillLevel)
        {
            //********最终白字计算*********
            //伙伴基础白字属性 = [角色成长数值 + 觉醒固定值(只有速度需要计算) + 进阶数值 + 星槽数值 + 潜能固定值(只有速度需要计算)] * [星级加成% + 技能加成% + 巅峰加成% + 觉醒加成% + 羁绊加成%]
            //*****************************
            LTAttributesData baseAttrData = AttributesManager.GetPartnerBaseAttr(heroStatId, heroUpgradeId, star, starHole, heroLevel,
                AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel,
                AwakenLevel, infoId);
            
            LTAttributesData totalAttrData = new LTAttributesData(baseAttrData);
            LTAttributesData tempAttrData = new LTAttributesData();

            //星级加成
            tempAttrData = GetPartnerStarNewAttributes(baseAttrData, heroStatId, star);
            totalAttrData.Add(tempAttrData);

            //技能加成
            tempAttrData = GetPartnerSkillAttributes(baseAttrData, heroStatId, commonSkillLevel, activeSkillLevel, passiveSkillLevel);
            totalAttrData.Add(tempAttrData);

            //巅峰加成
            tempAttrData = GetProficiencyMultiAttributes(AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel, baseAttrData);
            totalAttrData.Add(tempAttrData);

            //觉醒加成
            tempAttrData = GetPartnerShowAwakenAttributes(baseAttrData, infoId, AwakenLevel > 0);
            totalAttrData.Add(tempAttrData);

            //羁绊加成
            if (fetter != null)
            {
                tempAttrData = GetOtherPartnerGuardAttributes(baseAttrData, fetter);
                totalAttrData.Add(tempAttrData);
            }
            return totalAttrData;
        }

        /// <summary>[伙伴最终白字属性] - 仅计算自己伙伴，其他玩家伙伴通过LTFormationDataMnager.HandleOtherPlayerAttribute</summary>
        public static LTAttributesData GetPartnerAttributesByParnterData(LTPartnerData partnerData, int heroLv = -1)
        {
            //********伙伴最终白字计算*********
            //伙伴最终白字属性 = 图鉴固定值(只有攻防血需要计算)) + 伙伴基础白字属性  * [军团加成% + 图鉴加成% + 晋升%]
            //*****************************
            LTAttributesData baseAttrData = GetPartnerBaseAttributesByParnterData(partnerData.HeroStat.id, partnerData.UpGradeId, partnerData.Star, partnerData.StarHole, (heroLv > 0 ? heroLv : partnerData.Level), partnerData.AllRoundLevel, partnerData.ControlLevel, partnerData.StrongLevel, partnerData.RageLevel, partnerData.AbsorbedLevel, partnerData.IsAwaken, partnerData.InfoId, LTPartnerDataManager.Instance.GetGuardHash(partnerData.HeroId), partnerData.CommonSkillLevel, partnerData.ActiveSkillLevel, partnerData.PassiveSkillLevel);

            LTAttributesData totalAttrData = new LTAttributesData(baseAttrData);
            LTAttributesData tempAttrData = new LTAttributesData();

            //图鉴固定值
            LTAttributesData handbookAttr = GetPartnerAHandbookAttributes(LTPartnerHandbookManager.Instance.GetHandBookLevel());
            totalAttrData.Add(handbookAttr);

            //军团加成
            tempAttrData = GetPartnerLegionAttributes(baseAttrData, partnerData);
            totalAttrData.Add(tempAttrData);

            //图鉴加成
            tempAttrData = GetPartnerHandBookAttributes(baseAttrData, partnerData);
            totalAttrData.Add(tempAttrData);

            //晋升固定值及百分比
            tempAttrData = GetPartnerPromotionAttributes(partnerData);
            totalAttrData.Add(tempAttrData);
            totalAttrData.SpecialAdd(baseAttrData, tempAttrData);

            return totalAttrData;
        }
        
        /// <summary>[伙伴最终绿字属性]</summary>
        public static LTAttributesData GetPartnerEquipmentAttributes(LTAttributesData baseAttr, HeroEquipmentTotleAttr attr, SuitAttrAdditions AllBuffSuitAttr = null)
        {
            //********伙伴最终绿字计算*********
            //伙伴最终绿字属性 = 装备加成固定数值 + 伙伴基础白字属性 * [装备加成% + 全军装备加成%(不显示在面板)]
            //*****************************
            LTAttributesData attrData = new LTAttributesData();

            attrData.m_MaxHP = attr.MaxHP + baseAttr.m_MaxHP * (attr.MaxHPrate + attr.SuitAttr.MaxHPrate + (AllBuffSuitAttr != null ? AllBuffSuitAttr.MaxHPrate : 0f));
            attrData.m_ATK = attr.ATK + baseAttr.m_ATK * (attr.ATKrate + attr.SuitAttr.ATKrate + (AllBuffSuitAttr != null ? AllBuffSuitAttr.ATKrate : 0f));
            attrData.m_DEF = attr.DEF + baseAttr.m_DEF * (attr.DEFrate + attr.SuitAttr.DEFrate + (AllBuffSuitAttr != null ? AllBuffSuitAttr.DEFrate : 0f));
            attrData.m_Speed = attr.Speed + baseAttr.m_Speed * (attr.Speedrate + attr.SuitAttr.Speedrate + (AllBuffSuitAttr != null ? AllBuffSuitAttr.Speedrate : 0f));
            attrData.m_CritP = attr.CritP + attr.SuitAttr.CritP + (AllBuffSuitAttr != null ? AllBuffSuitAttr.CritP : 0f);
            attrData.m_CritV = attr.CritV + attr.SuitAttr.CritV + (AllBuffSuitAttr != null ? AllBuffSuitAttr.CritV : 0f);
            attrData.m_SpExtra = attr.SpExtra + attr.SuitAttr.SpExtra + (AllBuffSuitAttr != null ? AllBuffSuitAttr.SpExtra : 0f);
            attrData.m_SpRes = attr.SpRes + attr.SuitAttr.SpRes + (AllBuffSuitAttr != null ? AllBuffSuitAttr.SpRes : 0f);

            return attrData;
        }
        
        /// <summary>获取觉醒加成</summary>
        public static LTAttributesData GetPartnerAwakenAttributes(LTPartnerData partnerData)
        {
            LTAttributesData attrData = new LTAttributesData();
            if (partnerData.IsAwaken <= 0) return attrData;
            LTAttributesData baseAttr = AttributesManager.GetPartnerBaseAttr(partnerData.HeroStat.id, partnerData.UpGradeId, partnerData.Star, partnerData.StarHole, partnerData.Level, partnerData.AllRoundLevel, partnerData.ControlLevel, partnerData.StrongLevel, partnerData.RageLevel, partnerData.AbsorbedLevel, partnerData.IsAwaken, partnerData.InfoId);
            Hotfix_LT.Data.HeroAwakeInfoTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(partnerData.InfoId);
            attrData.m_MaxHP = baseAttr.m_MaxHP * temp.inc_MaxHP;
            attrData.m_ATK = baseAttr.m_ATK * temp.inc_ATK;
            attrData.m_DEF = baseAttr.m_DEF * temp.inc_DEF;
            attrData.m_Speed = baseAttr.m_Speed * temp.inc_speed;
            return attrData;
        }


        #region 基础白字加算部分
        /// <summary>获取基础白字的固定值</summary>
        private static LTAttributesData GetPartnerBaseAttr(int heroStatId, int heroUpgradeId, int star, int starHole, int heroLevel, int AllRoundLevel, int ControlLevel, int StrongLevel, int RageLevel, int AbsorbedLevel, int awakenLevel, int infoId)
        {
            //********基础白字计算*********
            //[角色成长数值 + 觉醒固定值(只有速度需要计算) + 进阶数值 + 星槽数值 + 潜能固定值(只有速度需要计算)]
            //*****************************
            //加算在此
            LTAttributesData baseAttrData = new LTAttributesData();
            Hotfix_LT.Data.LevelUpInfoTemplate baseAttrInfo = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetLevelUpInfoByIDAndLevel(heroStatId, heroLevel);

            //角色成长数值
            baseAttrData.m_MaxHP = baseAttrInfo.maxHP;
            baseAttrData.m_ATK = baseAttrInfo.ATK;
            baseAttrData.m_DEF = baseAttrInfo.DEF;
            baseAttrData.m_CritP = baseAttrInfo.CritP;
            baseAttrData.m_CritV = baseAttrInfo.CritV;
            baseAttrData.m_SpExtra = baseAttrInfo.SpExtra;
            baseAttrData.m_SpRes = baseAttrInfo.SpRes;
            baseAttrData.m_Speed = baseAttrInfo.speed;

            //觉醒固定值
            LTAttributesData awakeAttr = GetPartnerAwakenCentainAttributes(awakenLevel, infoId);
            baseAttrData.Add(awakeAttr);

            //进阶数值
            LTAttributesData gradeAttr = GetPartnerGradeAttributes(heroStatId, heroUpgradeId);
            baseAttrData.Add(gradeAttr);

            //星槽数值
            LTAttributesData holeAttr = GetPartnerHoleAttributes(heroStatId, star, starHole);
            baseAttrData.Add(holeAttr);

            //潜能固定值
            LTAttributesData proficiencyAttr = GetProficiencyAddAttributes(AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel);
            baseAttrData.Add(proficiencyAttr);

            return baseAttrData;
        }

        /// <summary>觉醒固定值</summary>
        private static LTAttributesData GetPartnerAwakenCentainAttributes(int awakenlevel, int infoId)
        {
            LTAttributesData awakeAttr = new LTAttributesData();
            if (awakenlevel <= 0) return awakeAttr;
            Hotfix_LT.Data.HeroAwakeInfoTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(infoId);
            if (temp.awakeType == 2)
            {
                awakeAttr.m_Speed = temp.speedAdd;
                awakeAttr.m_SpExtra = temp.SpExtra;
                awakeAttr.m_SpRes = temp.SpRes;
                awakeAttr.m_CritV = temp.CritV;
                awakeAttr.m_CritP = temp.CritP;

            }
            return awakeAttr;
        }

        /// <summary>进阶数值</summary>
        private static LTAttributesData GetPartnerGradeAttributes(int heroStatId, int gradeId)
        {
            LTAttributesData attrData = new LTAttributesData();
            Hotfix_LT.Data.HeroStatTemplate heroTem = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(heroStatId);           
            if (heroTem != null)
            {
                Hotfix_LT.Data.HeroInfoTemplate heroInfoTpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroInfo(heroTem.character_id);
                if (heroInfoTpl == null)
                {
                    EB.Debug.LogError("伙伴信息为空，Characterid = {0}",heroTem.character_id);
                    return attrData;
                }
                Hotfix_LT.Data.UpGradeInfoTemplate gradeInfoByGrade = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetUpGradeInfo(gradeId, heroInfoTpl.char_type);
                if (gradeInfoByGrade == null)
                {
                    EB.Debug.LogError("gradeInfoByGrade升阶信息为空,gradeid = {0}" , gradeId);
                    return attrData;
                }
                Hotfix_LT.Data.LevelUpInfoTemplate BaseAttr = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetLevelUpInfoByIDAndLevel(heroStatId, gradeInfoByGrade.level);
                if (gradeInfoByGrade != null)
                {
                    attrData.m_MaxHP = BaseAttr.maxHP * gradeInfoByGrade.inc_maxhp;
                    attrData.m_ATK = BaseAttr.ATK * gradeInfoByGrade.inc_atk;
                    attrData.m_DEF = BaseAttr.DEF * gradeInfoByGrade.inc_def;
                    attrData.m_Speed = BaseAttr.speed * gradeInfoByGrade.inc_speed;
                }
            }

            return attrData;
        }

        /// <summary>星槽数值</summary>
        private static LTAttributesData GetPartnerHoleAttributes(int heroStatId, int star, int hole)
        {
            LTAttributesData attrData;
            // 三星零孔和二星十孔属性是同一个，一星零孔没有属性加成
            int starId = star * 100 + hole;
            if (hole <= 0)
            {
                if (star > 1)
                {
                    starId = (star - 1) * 100 + 10;
                }
                else
                {
                    starId = 0;
                }
            }
            attrData = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarHoleAttrByStarId(heroStatId, starId);
            return attrData;
        }

        /// <summary>潜能固定值</summary>
        private static LTAttributesData GetProficiencyAddAttributes(int AllRoundLevel, int ControlLevel, int StrongLevel, int RageLevel, int AbsorbedLevel)
        {
            HeroProficiencyTotleAttr ProficiencyAttr = new HeroProficiencyTotleAttr(AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel);
            LTAttributesData attrData = ProficiencyAttr.GetAddAttrData();
            return attrData;
        }
        #endregion


        #region 基础白字乘算部分
        /// <summary>星级加成</summary>
        private static LTAttributesData GetPartnerStarNewAttributes(LTAttributesData baseAttr, int heroStatId, int star)
        {
            LTAttributesData attrData = new LTAttributesData(); ;
            Hotfix_LT.Data.HeroStatTemplate heroTem = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(heroStatId);
            if (heroTem != null)
            {
                Hotfix_LT.Data.StarUpInfoTemplate tpl = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetStarUpInfo(star);
                if (tpl != null)
                {
                    attrData.m_MaxHP = baseAttr.m_MaxHP * (tpl.IncMaxHp - 1);
                    attrData.m_ATK = baseAttr.m_ATK * (tpl.IncATK - 1);
                    attrData.m_DEF = baseAttr.m_DEF * (tpl.IncDEF - 1);
                }
            }

            return attrData;
        }
        
        /// <summary>技能加成</summary>
        private static LTAttributesData GetPartnerSkillAttributes(LTAttributesData baseAttr, int heroStatId, int commonSkillLevel, int activeSkillLevel, int passiveSkillLevel)
        {
            LTAttributesData attrData = new LTAttributesData();
            Hotfix_LT.Data.HeroStatTemplate heroTem = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroStat(heroStatId);
            if (heroTem != null)
            {
                LTAttributesData attr1 = GetSkillAttr(baseAttr, heroTem.common_skill, commonSkillLevel);
                attrData.Add(attr1);
                LTAttributesData attr2 = GetSkillAttr(baseAttr, heroTem.active_skill, activeSkillLevel);
                attrData.Add(attr2);
                LTAttributesData attr3 = GetSkillAttr(baseAttr, heroTem.passive_skill, passiveSkillLevel);
                attrData.Add(attr3);
            }
            return attrData;
        }
        /// <summary>计算单技能加成属性</summary>
        private static LTAttributesData GetSkillAttr(LTAttributesData baseAttr, int skillDskillId, int skillLevel)
        {
            LTAttributesData attrData = new LTAttributesData();
            Hotfix_LT.Data.SkillLevelUpTemplate skillData = Hotfix_LT.Data.SkillTemplateManager.Instance.GetSkillLevelUpTemplate(skillDskillId);
            if (skillData == null)
            {
                return baseAttr;
            }
            if (skillData.MaxHpIncPercent > 0)
            {
                attrData.m_MaxHP = baseAttr.m_MaxHP * skillData.MaxHpIncPercent * (skillLevel - 1);
            }
            if (skillData.AtkIncPercent > 0)
            {
                attrData.m_ATK = baseAttr.m_ATK * skillData.AtkIncPercent * (skillLevel - 1);
            }
            if (skillData.DefIncPercnet > 0)
            {
                attrData.m_DEF = baseAttr.m_DEF * skillData.DefIncPercnet * (skillLevel - 1);
            }
            return attrData;
        }

        /// <summary>巅峰加成</summary>
        private static LTAttributesData GetProficiencyMultiAttributes(int AllRoundLevel, int ControlLevel, int StrongLevel, int RageLevel, int AbsorbedLevel, LTAttributesData baseAttr)
        {
            HeroProficiencyTotleAttr ProficiencyAttr = new HeroProficiencyTotleAttr(AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel);
            LTAttributesData attrData = ProficiencyAttr.GetMulAttrData(baseAttr);
            return attrData;
        }

        /// <summary>觉醒加成</summary>
        private static LTAttributesData GetPartnerShowAwakenAttributes(LTAttributesData baseAttr, int infoId, bool isAwaken)
        {
            LTAttributesData attrData = new LTAttributesData();
            if (!isAwaken) return attrData;
            Hotfix_LT.Data.HeroAwakeInfoTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(infoId);
            attrData.m_MaxHP = baseAttr.m_MaxHP * temp.inc_MaxHP;
            attrData.m_ATK = baseAttr.m_ATK * temp.inc_ATK;
            attrData.m_DEF = baseAttr.m_DEF * temp.inc_DEF;
            attrData.m_Speed = baseAttr.m_Speed * temp.inc_speed;
            return attrData;
        }

        /// <summary>羁绊加成</summary>
        private static LTAttributesData GetOtherPartnerGuardAttributes(LTAttributesData baseAttr,IDictionary dictionary)
        {
            LTAttributesData attrData = new LTAttributesData();
            float inf_ATK = 0F;
            float inc_MaxHP = 0F;
            float inc_DEF = 0F;
            for (int index = 1; index <= 3; index++)
            {
                for (int con = 1; con <= 3; con++)
                {
                    int level = EB.Dot.Integer(string.Format("{0}.{1}", index, con), dictionary, 0);
                    HeroGuardTemplate heroGuard = CharacterTemplateManager.Instance.GetGuard(index, con, level);
                    inf_ATK += heroGuard.IncATK;
                    inc_MaxHP += heroGuard.IncMaxHP;
                    inc_DEF += heroGuard.IncDEF;
                }
            }
            attrData.m_MaxHP = baseAttr.m_MaxHP * inc_MaxHP;
            attrData.m_ATK = baseAttr.m_ATK * inf_ATK;
            attrData.m_DEF = baseAttr.m_DEF * inc_DEF;
            return attrData;
        }
        #endregion


        #region  最终白字部分
        /// <summary>图鉴固定值</summary>
        public static LTAttributesData GetPartnerAHandbookAttributes(int handbookLevel)
        {
            LTAttributesData awakeAttr = new LTAttributesData();
            if (handbookLevel <= 0) return awakeAttr;
            Hotfix_LT.Data.MannualScoreTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetMannualScoreTemplateById(handbookLevel);
            if (temp != null)
            {
                awakeAttr.m_ATK = temp.ATK;
                awakeAttr.m_DEF = temp.DEF;
                awakeAttr.m_MaxHP = temp.maxHP;
            }
            return awakeAttr;
        }
        
        /// <summary>获取公会科技加成</summary>
        private static LTAttributesData GetPartnerLegionAttributes(LTAttributesData baseAttr,LTPartnerData partnerData)
        {
            LTAttributesData attrData = new LTAttributesData();
            if (!LegionModel.GetInstance().isJoinedLegion)
            {
                return attrData;
            }
            var levedic = AlliancesManager.Instance.Account.legionTechInfo.TechlevelDic;
            if (levedic != null)
            {
                List<AllianceTechSkill> data = AllianceTemplateManager.Instance.mTechSkillList;
                float Atkadd = 0, Defadd = 0,MaxHpadd = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    var skilllist = data[i];
                    if (partnerData.HeroInfo.char_type != (eRoleAttr)skilllist.charType)
                    {
                        continue;
                    }     
                    if (levedic.TryGetValue(skilllist.skillid, out int level))
                    {
                        switch (data[i].addtionType)
                        {
                            case (int)Data.TechSkillAddtionType.ATK:
                                Atkadd += skilllist.levelinfo[level].addition;
                                break;
                            case (int)Data.TechSkillAddtionType.DEF:
                                Defadd += skilllist.levelinfo[level].addition;
                                break;
                            case (int)Data.TechSkillAddtionType.MaxHp:
                                MaxHpadd += skilllist.levelinfo[level].addition;
                                break;
                            default:
                                break;
                        }
                    }
                }
                attrData.m_MaxHP = baseAttr.m_MaxHP * MaxHpadd;
                attrData.m_ATK = baseAttr.m_ATK * Atkadd;
                attrData.m_DEF = baseAttr.m_DEF * Defadd;
            }
            return attrData;
        }

        /// <summary>
        /// 获取图鉴加成 
        /// </summary>
        private static LTAttributesData GetPartnerHandBookAttributes(LTAttributesData baseAttr, LTPartnerData partnerData)
        {
            LTAttributesData attrData = new LTAttributesData();
            if (LTPartnerHandbookManager.Instance.TheHandbookList != null)
            {
                HandbookData handbookDatas = LTPartnerHandbookManager.Instance.TheHandbookList.Find(partnerData.HeroInfo.char_type);
                if (handbookDatas != null)
                {
                    Hotfix_LT.Data.MannualBreakTemplate temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetBreakTemplateByLevel(handbookDatas.HandbookId, handbookDatas.BreakLevel);
                    if (temp != null)
                    {
                        attrData.m_MaxHP = baseAttr.m_MaxHP * temp.IncATK;
                        attrData.m_ATK = baseAttr.m_ATK * temp.IncDEF;
                        attrData.m_DEF = baseAttr.m_DEF * temp.IncMaxHp;
                    }
                }
            }
            attrData.m_Speed = baseAttr.m_Speed * LTPartnerHandbookManager.Instance.GetHandbookAddtionFromCard(partnerData, IHandBookAddAttrType.ATTR_Speed) / 100;
            attrData.m_CritP = LTPartnerHandbookManager.Instance.GetHandbookAddtionFromCard(partnerData, IHandBookAddAttrType.ATTR_CritP) / 100;
            attrData.m_CritDef = LTPartnerHandbookManager.Instance.GetHandbookAddtionFromCard(partnerData, IHandBookAddAttrType.ATTR_CRIresist) / 100;//暴击抵抗
            attrData.m_DamageAdd = LTPartnerHandbookManager.Instance.GetHandbookAddtionFromCard(partnerData, IHandBookAddAttrType.ID_ATTR_DMGincrease) / 100;//伤害加成
            attrData.m_DamageReduce = LTPartnerHandbookManager.Instance.GetHandbookAddtionFromCard(partnerData, IHandBookAddAttrType.ID_ATTR_DMGreduction) / 100;//伤害减免

            return attrData;
        }

        /// <summary>
        /// 获取晋升加成 
        /// </summary>
        private static LTAttributesData GetPartnerPromotionAttributes( LTPartnerData partnerData) {
            LTAttributesData attrData = new LTAttributesData();
            var attrList = LTPromotionManager.Instance.GetAttrList();
            for (int i = 0; i < attrList.Count; ++i)
            {
                float value = LTPromotionManager.Instance.GetCurrentAttrLevel(attrList[i].id) * attrList[i].attrValue;
                attrData.Add(attrList[i].name, value);
            }
            return attrData;

        }
        #endregion


        #region 战力计算相关
        private static Dictionary<int, int> suitNum;

        private static List<int> ECid = new List<int>();

        /// <summary>计算伙伴属性战力</summary>
        public static float GetSelfAttributeCombatPower(LTPartnerData partnerData)
        {
            LTAttributesData attr = GetPartnerAllAttributesByParnterData(partnerData);//所有基础属性         
            float power = attr.Power() * (1 + attr.PowerPer());
            return power;
        }

        /// <summary>计算伙伴技能加成战力</summary>
        public static float GetPartnerSkillPowerPer(LTPartnerData partnerData)
        {
            float skillPer = 0;
            SkillTemplate commonSkillTem = SkillTemplateManager.Instance.GetTemplateWithAwake(partnerData, partnerData.HeroStat.common_skill);
            SkillTemplate passiveSkillTem = SkillTemplateManager.Instance.GetTemplateWithAwake(partnerData, partnerData.HeroStat.passive_skill);
            SkillTemplate activeSkillTem = SkillTemplateManager.Instance.GetTemplateWithAwake(partnerData, partnerData.HeroStat.active_skill);
            skillPer = partnerData.CommonSkillLevel * commonSkillTem.BattleRating +
                             partnerData.PassiveSkillLevel * passiveSkillTem.BattleRating +
                             partnerData.ActiveSkillLevel * activeSkillTem.BattleRating;
            SkillTemplate starskill;
            switch (partnerData.Star)
            {
                case 5:
                    starskill = SkillTemplateManager.Instance.GetTemplate(partnerData.HeroStat.starskill5);
                    skillPer += starskill.BattleRating;
                    break;
                case 6:
                    starskill = SkillTemplateManager.Instance.GetTemplate(partnerData.HeroStat.starskill5);
                    skillPer += starskill.BattleRating;
                    starskill = SkillTemplateManager.Instance.GetTemplate(partnerData.HeroStat.starskill6);
                    skillPer += starskill.BattleRating;
                    break;
                default:
                    break;
            }
            return skillPer;
        }

        /// <summary>计算装备套装加成战力</summary>
        public static float GetEquipSuitPer(LTPartnerData partnerData)
        {
            ECid.Clear();
            for (int i = 0; i < partnerData.EquipmentsInfo.Length; i++)
            {
                int ecid = partnerData.EquipmentsInfo[i].ECid;
                if (ecid != 0)
                {
                    ECid.Add(ecid);
                }
            }
            return GetEquipSuitPer(ECid);
        }

        /// <summary>计算技能及装备套装加成的战力</summary>
        public static float GetOtherCombatPower(LTAttributesData attr, int[] _skill, int[] skill_level, List<int> ECid, int infoId, int star, int awakenlevel)
        {
            SkillTemplate commonSkillTem = SkillTemplateManager.Instance.GetTemplateWithAwake(infoId, _skill[0], awakenlevel);
            SkillTemplate passiveSkillTem = SkillTemplateManager.Instance.GetTemplateWithAwake(infoId, _skill[1], awakenlevel);
            SkillTemplate activeSkillTem = SkillTemplateManager.Instance.GetTemplateWithAwake(infoId, _skill[2], awakenlevel);
            float skillPer = skill_level[0] * commonSkillTem.BattleRating +
                             skill_level[1] * passiveSkillTem.BattleRating +
                             skill_level[2] * activeSkillTem.BattleRating;
            int skillid;
            SkillTemplate skilltemp;
            switch (star)
            {
                case 5:
                    skillid = CharacterTemplateManager.Instance.GetHeroStatByInfoId(infoId).starskill5;
                    skilltemp = SkillTemplateManager.Instance.GetTemplate(skillid);
                    skillPer += skilltemp.BattleRating;
                    break;
                case 6:
                    skillid = CharacterTemplateManager.Instance.GetHeroStatByInfoId(infoId).starskill5;
                    skilltemp = SkillTemplateManager.Instance.GetTemplate(skillid);
                    skillPer += skilltemp.BattleRating;
                    skillid = CharacterTemplateManager.Instance.GetHeroStatByInfoId(infoId).starskill6;
                    skilltemp = SkillTemplateManager.Instance.GetTemplate(skillid);
                    skillPer += skilltemp.BattleRating;
                    break;
                default:
                    break;
            }

            skillPer += GetEquipSuitPer(ECid);
            float power = attr.Power() * (1 + attr.PowerPer()) * (1 + skillPer);
            return power;
        }

        /// <summary>获取最终属性</summary>
        private static LTAttributesData GetPartnerAllAttributesByParnterData(LTPartnerData partnerData)
        {
            //最终白字属性
            LTAttributesData attrData = GetPartnerAttributesByParnterData(partnerData);
            //基础白字属性
            LTAttributesData baseData = AttributesUtil.GetBaseAttributes(partnerData);
            //绿字属性
            LTAttributesData equipData = GetPartnerEquipmentAttributes(baseData, partnerData.EquipmentTotleAttr);

            attrData.Add(equipData);

            return attrData;
        }
        
        /// <summary>获取装备套装战力</summary>
        private static float GetEquipSuitPer(List<int> ECid)
        {
            if (suitNum == null) suitNum = new Dictionary<int, int>();
            float suitPer = 0f;
            SkillTemplate suitAttr;
            for (int i = 0; i < ECid.Count; i++)
            {
                EquipmentItemTemplate tpl = EconemyTemplateManager.Instance.GetEquipment(ECid[i]);
                int skill_Id = tpl.SuitAttrId_1 == 0 ? tpl.SuitAttrId_2 : tpl.SuitAttrId_1;
                if (tpl.SuitAttrId_1 != 0)
                {
                    skill_Id = tpl.SuitAttrId_1;
                    if (suitNum.ContainsKey(skill_Id))
                    {
                        suitNum[skill_Id]--;
                        if (suitNum[skill_Id] == 0)
                        {
                            suitAttr = SkillTemplateManager.Instance.GetTemplate(skill_Id); //套装2
                            suitPer += suitAttr.BattleRating;
                            suitNum.Remove(skill_Id);
                        }
                    }
                    else
                        suitNum[skill_Id] = 1;
                }
                else
                {
                    if (suitNum.ContainsKey(skill_Id))
                    {
                        suitNum[skill_Id]--;
                        if (suitNum[skill_Id] == 0)
                        {
                            suitAttr = SkillTemplateManager.Instance.GetTemplate(skill_Id); //套装2
                            suitPer += suitAttr.BattleRating;
                            suitNum.Remove(skill_Id);
                        }
                    }
                    else suitNum[skill_Id] = 3;
                }
            }

            suitNum.Clear();
            return suitPer;
        }
        #endregion
    }

    public class AttributesUtil
    {
        public static LTAttributesData GetBaseAttributes(LTPartnerData partnerData)
        {
            LTAttributesData baseAttrData = AttributesManager.GetPartnerBaseAttributesByParnterData(partnerData.HeroStat.id, partnerData.UpGradeId, partnerData.Star, partnerData.StarHole, partnerData.Level, partnerData.AllRoundLevel, partnerData.ControlLevel, partnerData.StrongLevel, partnerData.RageLevel, partnerData.AbsorbedLevel, partnerData.IsAwaken, partnerData.InfoId, LTPartnerDataManager.Instance.GetGuardHash(partnerData.HeroId), partnerData.CommonSkillLevel, partnerData.ActiveSkillLevel, partnerData.PassiveSkillLevel);
            return baseAttrData;
        }
    }

}