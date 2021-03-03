using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix_LT.UI
{
    public class LTPartnerData
    {
        //军团雇佣兵拥有
        public long uid;
        public int br;
        //
        public bool IsHire = false;//是否为雇佣
        public int HireHeroId;
        public int HireLevel;
        public int HireAllRoundLevel;
        public int HireUpGradeId;
        public int HireStar;
        public int HireAwakeLevel;
        public int HireArtifactLevel;

        public bool IsHeroBattle = false;//是否为英雄交锋
        public int HeroBattleCost;
        public int HeroBattleUpGradeId;
        public int HeroBattleStar;
        //觉醒相关用于构造英雄数据
        public int HeroBattleAwakenLevel;

        public int StatId;//templateId(main) eg:15011

        public int InfoId;//characterId  eg:15010

        public int mHeroId;//唯一id     eg:1000021
        public int HeroId
        {
            get
            {
                if (IsHire)
                {
                    return HireHeroId;
                }
                else
                {
                    return mHeroId;
                }
            }
        }

        public int Level
        {
            get
            {
                if (IsHire)
                {
                    return HireLevel;
                }
                else if (IsHeroBattle)
                {
                    return HeroBattleCost;
                }
                else
                {
                    int curLevel = 0;
                    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.level", this.HeroId), out curLevel);
                    return curLevel;
                }
            }
        }

        public int Exp
        {
            get
            {
                int curExp = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.xp", this.HeroId), out curExp);
                return curExp;
            }
        }

        public int UpGradeId
        {
            get
            {
                if (IsHire)
                {
                    return HireUpGradeId;
                }
                else if (IsHeroBattle)
                {
                    return HeroBattleUpGradeId;
                }
                else
                {
                    int upGradeId = 0;
                    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.awake.level", this.HeroId), out upGradeId);
                    return upGradeId;
                }
            }
        }

        public int Star
        {
            get
            {
                if (IsHire)
                {
                    return HireStar;
                }
                else if (IsHeroBattle)
                {
                    return HeroBattleStar;
                }
                else
                {
                    int star = 0;
                    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.star", this.HeroId), out star);
                    return star;
                }
            }
        }

        public int StarHole
        {
            get
            {
                int starHole = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.starhole", this.HeroId), out starHole);
                return starHole;
            }
        }

        #region cpu 69次，49ms
        public int ShardNum
        {
            get
            {
                return GameItemUtil.GetInventoryItemNum(this.StatId.ToString());
            }
        }
        #endregion

        public int CommonSkillLevel
        {
            get
            {
                int skillLevel = 1;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.skill_info.{1}.level", this.HeroId, this.HeroStat.common_skill), out skillLevel);
                skillLevel = skillLevel == 0 ? 1 : skillLevel;
                return skillLevel;
            }
        }

        public int CommonSkillExp
        {
            get
            {
                int exp = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.skill_info.{1}.xp", this.HeroId, this.HeroStat.common_skill), out exp);
                return exp;
            }
        }

        public int PassiveSkillLevel
        {
            get
            {
                int skillLevel = 1;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.skill_info.{1}.level", this.HeroId, this.HeroStat.passive_skill), out skillLevel);
                skillLevel = skillLevel == 0 ? 1 : skillLevel;
                return skillLevel;
            }
        }

        public int PassiveSkillExp
        {
            get
            {
                int exp = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.skill_info.{1}.xp", this.HeroId, this.HeroStat.passive_skill), out exp);
                return exp;
            }
        }

        public int ActiveSkillLevel
        {
            get
            {
                int skillLevel = 1;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.skill_info.{1}.level", this.HeroId, this.HeroStat.active_skill), out skillLevel);
                skillLevel = skillLevel == 0 ? 1 : skillLevel;
                return skillLevel;
            }
        }

        public int ActiveSkillExp
        {
            get
            {
                int exp = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.skill_info.{1}.xp", this.HeroId, this.HeroStat.active_skill), out exp);
                return exp;
            }
        }
        /// <summary>
        /// 是否觉醒
        /// </summary>
        public int IsAwaken
        {
            get
            {
                int condition = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.stat.awaken.level", this.HeroId), out condition);
                return condition;
            }
        }

        /// <summary>
        /// 当前使用皮肤0为默认，1为觉醒，后面新增顺序读表
        /// </summary>
        public int CurSkin//当前使用皮肤
        {
            get
            {
                int curskin = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.skin", this.HeroId), out curskin);
                return curskin;
            }
        }

        public bool HasReceiveReward
        {
            get
            {
                bool hasReceive = false;
                DataLookupsCache.Instance.SearchDataByID<bool>(string.Format("heroStats.{0}.is_get", this.HeroId), out hasReceive);
                return hasReceive;
            }
        }
        public bool IsGoIntoBattle
        {
            get
            {
                return LTPartnerDataManager.Instance.IsGoIntoBattle(HeroId);
            }
        }

        public Hotfix_LT.Data.HeroStatTemplate HeroStat;

        public Hotfix_LT.Data.HeroInfoTemplate HeroInfo;
        private PowerData m_powerData;
        public bool isPowerNeedRefresh = false;
       

        public PowerData powerData
        {
            get
            {
                if (m_powerData == null)
                {
                    m_powerData = new PowerData(this);
                    return m_powerData;
                }                    
                if (isPowerNeedRefresh)
                {
                    m_powerData.OnValueChanged(this, false, PowerData.RefreshType.Attribute);//监测数据是否需要改变
                    isPowerNeedRefresh = false;
                }
                return m_powerData;
            }
        }
        /// <summary>
        /// 获取伙伴的装备列表（这里已经做了6次循环了，拿出去的话不要放在循环里面重复循环）
        /// </summary>
        public HeroEquipmentInfo[] EquipmentsInfo
        {
            get
            {
                HeroEquipmentInfo[] items = new HeroEquipmentInfo[6];
                for (int i = 0; i < 6; i++)
                {
                    int temp = 0;
                    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.equip.{1}", this.HeroId, (i + 1)), out temp);
                    items[i].type =  (temp == 0) ? EquipPartType.none : (EquipPartType)(i + 1);
                    items[i].Eid = temp;
                    if (temp != 0)
                    {
                        int tempcid = 0;
                        DataLookupsCache.Instance.SearchIntByID(string.Format("inventory.{0}.economy_id", temp), out tempcid);
                        items[i].ECid = tempcid;
                    }

                }
                return items;//获得6件装备的物品id信息，仅id
            }
        }

        /// <summary>
        /// 拥有的装备数量
        /// </summary>
        public int EquipmentCount
        {
            get
            {
                HeroEquipmentInfo[] e = EquipmentsInfo;
                int count = 0;

                for (int i = 0; i < e.Length; i++)
                {
                    if (e[i].Eid != 0)
                    {
                        count += 1;
                    }
                }

                return count;
            }
        }

        #region cpu 845ms
        /// <summary>
        /// 获取某个部件的装备信息（0~5）
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HeroEquipmentInfo GetEquipmentsInfo(int index)
        {
            HeroEquipmentInfo item = new HeroEquipmentInfo();
            int temp = 0;
            DataLookupsCache.Instance.SearchDataByID(string.Format("heroStats.{0}.equip.{1}", this.HeroId, (index + 1)), out temp);
            item.type = (temp == 0) ? EquipPartType.none : (EquipPartType)(index + 1);
            item.Eid = temp;

            return item;
        }
        #endregion

        public bool IsCheckEquipUpLv()
        {
            if (HeroId <= 0)
            {
                return false;
            }

            if (!IsGoIntoBattle)
            {
                return false;
            }

            bool isOK = false;
            List<BaseEquipmentInfo> equipList = LTPartnerEquipDataManager.Instance.GetFreeEquipInfoList();
            List<int> EquipType = new List<int>();
            for (int i = 0; i < equipList.Count; i++)
            {
                if (!EquipType.Contains((int)equipList[i].Type))
                    EquipType.Add((int)equipList[i].Type);
                if (EquipType.Count == 6) break;
            }
            if (equipList.Count == 0) return false;

            HeroEquipmentInfo[] e = EquipmentsInfo;
            for (int i = 0; i < e.Length; i++)
            {
                if (e[i].Eid != 0)//有装备看是否到15级
                {
                    DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(e[i].Eid);
                    if(info == null)
                    {
                        EB.Debug.LogError("IsCheckEquipUpLv info is null,Eid = {0}", e[i].Eid);
                        continue;
                    }
                    if (info.EquipLevel != 15) { isOK = true; break; }
                }
                else//没装备看能否穿装备
                {
                    if (EquipType.Contains(i + 1)) { isOK = true; break; }
                }
            }
            return isOK;
        }

        
        //TODO 备注：发现这里循环在外面调会EquipmentTotleAttr.SuitList执行很多遍 TODO优化
        public HeroEquipmentTotleAttr EquipmentTotleAttr
        {
            get
            {
                HeroEquipmentTotleAttr temp = new HeroEquipmentTotleAttr();
                for (int i = 0; i < 6; i++)
                {
                    int eid = 0;
                    DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.equip.{1}", this.HeroId, (i + 1)), out eid);
                    if (eid != 0)
                    {
                        DetailedEquipmentInfo info = LTPartnerEquipDataManager.Instance.GetEquipmentInfoByEID(eid);
                        if(info == null)
                        {
                            EB.Debug.LogError("EquipmentTotleAttr info is null");
                            continue;
                        }
                        temp.AddSuitInfo(info);
                    }
                }
                return temp;
            }
        }

        public int GetProficiencyLevelByType(Hotfix_LT.Data.ProficiencyType type)
        {
            switch (type)
            {
                case Hotfix_LT.Data.ProficiencyType.AllRound:
                    return AllRoundLevel;
                case Hotfix_LT.Data.ProficiencyType.Control:
                    return ControlLevel;
                case Hotfix_LT.Data.ProficiencyType.Strong:
                    return StrongLevel;
                case Hotfix_LT.Data.ProficiencyType.Rage:
                    return RageLevel;
                case Hotfix_LT.Data.ProficiencyType.Absorbed:
                    return AbsorbedLevel;
                default:
                    return 0;
            }
        }

        public int ArtifactLevel
        {
            get
            {
                if (IsHire)
                {
                    return HireArtifactLevel;
                }

                //没有激活level为-1 激活为0
                int level = 0;
                if (DataLookupsCache.Instance.SearchIntByID(
                    string.Format("heroStats.{0}.artifact_equip.enhancement_level", this.HeroId), out level))
                {
                    return level;
                }
                return -1;
            }
        }
     

        public int AllRoundLevel
        {
            get
            {
                if (IsHire)
                {
                    return HireAllRoundLevel;
                }
                
                int level = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.proficiency.1.1", this.HeroId), out level);
                return level;
            }
        }
        public int ControlLevel
        {
            get
            {
                int level = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.proficiency.2.2", this.HeroId), out level);
                return level;
            }
        }
        public int StrongLevel
        {
            get
            {
                int level = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.proficiency.2.3", this.HeroId), out level);
                return level;
            }
        }
        public int RageLevel
        {
            get
            {
                int level = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.proficiency.2.4", this.HeroId), out level);
                return level;
            }
        }
        public int AbsorbedLevel
        {
            get
            {
                int level = 0;
                DataLookupsCache.Instance.SearchIntByID(string.Format("heroStats.{0}.proficiency.2.5", this.HeroId), out level);
                return level;
            }
        }
    }

    public class HeroProficiencyTotleAttr
    {
        public float maxHPrate;
        public float ATKrate;
        public float DEFrate;

        public float speed;
        public float CritP;
        public float CritV;
        public float SpExtra;
        public float SpRes;

        public float AntiCritP;
        public float DmgMulti;
        public float DmgRes;

        public HeroProficiencyTotleAttr(int AllRoundLevel, int ControlLevel, int StrongLevel, int RageLevel, int AbsorbedLevel)
        {
            int[] levels = new int[5] { AllRoundLevel, ControlLevel, StrongLevel, RageLevel, AbsorbedLevel };
            for (int i = 0; i < 5; i++)
            {
                var temp = Hotfix_LT.Data.CharacterTemplateManager.Instance.GetProficiencyUpByTypeAndLevel((Hotfix_LT.Data.ProficiencyType)(i + 1), levels[i]);
                if (temp != null)
                {
                    this.maxHPrate += temp.maxHP;
                    this.ATKrate += temp.ATK;
                    this.DEFrate += temp.DEF;

                    this.speed += temp.speed;
                    this.CritP += temp.CritP;
                    this.CritV += temp.CritV;
                    this.SpExtra += temp.SpExtra;
                    this.SpRes += temp.SpRes;
                    this.AntiCritP += temp.AntiCritP;
                    this.DmgMulti += temp.DmgMulti;
                    this.DmgRes += temp.DmgRes;
                }
            }
        }

        public LTAttributesData GetAddAttrData()
        {
            LTAttributesData attrData = new LTAttributesData();
            attrData.m_MaxHP = 0;
            attrData.m_ATK = 0;
            attrData.m_DEF = 0;
            attrData.m_CritP = CritP;
            attrData.m_CritV = CritV;
            attrData.m_SpExtra = SpExtra;
            attrData.m_SpRes = SpRes;
            attrData.m_Speed = speed;
            attrData.m_CritDef = AntiCritP;
            attrData.m_DamageAdd = DmgMulti;
            attrData.m_DamageReduce = DmgRes;
            return attrData;
        }

        public LTAttributesData GetMulAttrData(LTAttributesData Data)
        {
            LTAttributesData attrData = new LTAttributesData();
            attrData.m_MaxHP = Data.m_MaxHP * maxHPrate;
            attrData.m_ATK = Data.m_ATK * ATKrate;
            attrData.m_DEF = Data.m_DEF * DEFrate;
            return attrData;
        }
    }

    public class HeroEquipmentTotleAttr
    {
        public float MaxHP;
        public float ATK;
        public float DEF;
        public float ChainAtk;
        public float CritP;
        public float CritV;
        public float SpExtra;
        public float SpRes;
        public float MaxHPrate;
        public float ATKrate;
        public float DEFrate;
        public float Speed;
        public float Speedrate;

        public List<SuitAttrsSuitTypeAndCount> SuitList = new List<SuitAttrsSuitTypeAndCount>();
        public SuitAttrAdditions SuitAttr
        {
            get
            {
                SuitAttrAdditions temp = new SuitAttrAdditions();
                for (int i = 0; i < SuitList.Count; i++)
                {
                    Hotfix_LT.Data.SuitTypeInfo suitTypeInfo = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(SuitList[i].SuitType);
                    if (SuitList[i].count >= 2 && suitTypeInfo.SuitAttr2 != 0)
                    {
                        float multiplyingPower = SuitList[i].count / 2;
                        Hotfix_LT.Data.SuitAttribute suitAttr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(suitTypeInfo.SuitAttr2);
                        if (suitAttr != null && suitAttr.all == 0)
                        {
                            float value = suitAttr.value * multiplyingPower;
                            switch (suitAttr.attr)
                            {
                                case "MaxHPrate": { temp.MaxHPrate += value; }; break;
                                case "ATKrate": { temp.ATKrate += value; }; break;
                                case "DEFrate": { temp.DEFrate += value; }; break;
                                case "CritP": { temp.CritP += value; }; break;
                                case "CritV": { temp.CritV += value; }; break;
                                case "speedrate": { temp.Speedrate += value; }; break;
                                case "SpExtra": { temp.SpExtra += value; }; break;
                                case "SpRes": { temp.SpRes += value; }; break;
                            }
                        }
                    }
                    if (SuitList[i].count >= 4 && suitTypeInfo.SuitAttr4 != 0)
                    {
                        float multiplyingPower = SuitList[i].count / 4;
                        Hotfix_LT.Data.SuitAttribute suitAttr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(suitTypeInfo.SuitAttr4);
                        if (suitAttr != null && suitAttr.all == 0)
                        {
                            float value = suitAttr.value * multiplyingPower;
                            switch (suitAttr.attr)
                            {
                                case "MaxHPrate": { temp.MaxHPrate += value; }; break;
                                case "ATKrate": { temp.ATKrate += value; }; break;
                                case "DEFrate": { temp.DEFrate += value; }; break;
                                case "CritP": { temp.CritP += value; }; break;
                                case "CritV": { temp.CritV += value; }; break;
                                case "speedrate": { temp.Speedrate += value; }; break;
                                case "SpExtra": { temp.SpExtra += value; }; break;
                                case "SpRes": { temp.SpRes += value; }; break;
                            }
                        }
                    }
                }
                return temp;
            }
        }
        public SuitAttrAdditions BuffSuitAttr
        {
            get
            {
                SuitAttrAdditions temp = new SuitAttrAdditions();
                for (int i = 0; i < SuitList.Count; i++)
                {
                    Hotfix_LT.Data.SuitTypeInfo suitTypeInfo = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitTypeInfoByEcidSuitType(SuitList[i].SuitType);
                    if (SuitList[i].count >= 2 && suitTypeInfo.SuitAttr2 != 0)
                    {
                        float multiplyingPower = SuitList[i].count / 2;
                        Hotfix_LT.Data.SuitAttribute suitAttr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(suitTypeInfo.SuitAttr2);
                        if (suitAttr != null && suitAttr.all == 1)
                        {
                            float value = suitAttr.value * multiplyingPower;
                            switch (suitAttr.attr)
                            {
                                case "MaxHPrate": { temp.MaxHPrate += value; }; break;
                                case "ATKrate": { temp.ATKrate += value; }; break;
                                case "DEFrate": { temp.DEFrate += value; }; break;
                                case "CritP": { temp.CritP += value; }; break;
                                case "CritV": { temp.CritV += value; }; break;
                                case "speedrate": { temp.Speedrate += value; }; break;
                                case "SpExtra": { temp.SpExtra += value; }; break;
                                case "SpRes": { temp.SpRes += value; }; break;
                            }
                        }
                    }
                    if (SuitList[i].count >= 4 && suitTypeInfo.SuitAttr4 != 1)
                    {
                        float multiplyingPower = SuitList[i].count / 4;
                        Hotfix_LT.Data.SuitAttribute suitAttr = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetSuitAttrByID(suitTypeInfo.SuitAttr4);
                        if (suitAttr != null && suitAttr.all == 0)
                        {
                            float value = suitAttr.value * multiplyingPower;
                            switch (suitAttr.attr)
                            {
                                case "MaxHPrate": { temp.MaxHPrate += value; }; break;
                                case "ATKrate": { temp.ATKrate += value; }; break;
                                case "DEFrate": { temp.DEFrate += value; }; break;
                                case "CritP": { temp.CritP += value; }; break;
                                case "CritV": { temp.CritV += value; }; break;
                                case "speedrate": { temp.Speedrate += value; }; break;
                                case "SpExtra": { temp.SpExtra += value; }; break;
                                case "SpRes": { temp.SpRes += value; }; break;
                            }
                        }
                    }
                }
                return temp;
            }
        }

        public void AddSuitInfo(DetailedEquipmentInfo info)
        {

            bool unFind = true;
            for (int j = 0; j < this.SuitList.Count; j++)
            {
                if (info.SuitType == this.SuitList[j].SuitType)
                {
                    this.SuitList[j].count += 1;
                    unFind = false;
                    break;
                }
            }
            if (unFind)
            {
                SuitAttrsSuitTypeAndCount mTemp = new SuitAttrsSuitTypeAndCount(info.SuitType);
                this.SuitList.Add(mTemp);
            }
            switch (info.MainAttributes.Name)
            {
                case "ATK": this.ATK += info.MainAttributes.Value; break;
                case "MaxHP": this.MaxHP += info.MainAttributes.Value; break;
                case "DEF": this.DEF += info.MainAttributes.Value; break;
                case "CritP": this.CritP += info.MainAttributes.Value; break;
                case "CritV": this.CritV += info.MainAttributes.Value; break;
                case "ChainAtk": this.ChainAtk += info.MainAttributes.Value; break;
                case "SpExtra": this.SpExtra += info.MainAttributes.Value; break;
                case "SpRes": this.SpRes += info.MainAttributes.Value; break;
                case "MaxHPrate": this.MaxHPrate += info.MainAttributes.Value; break;
                case "ATKrate": this.ATKrate += info.MainAttributes.Value; break;
                case "DEFrate": this.DEFrate += info.MainAttributes.Value; break;
                case "Speed": this.Speed += info.MainAttributes.Value; break;
                case "speedrate": this.Speedrate += info.MainAttributes.Value; break;
                default: EB.Debug.LogWarning("Equipment MainAttribute Miss{0}", info.MainAttributes.Name); break;
            }
            for (int j = 0; j < info.ExAttributes.Count; j++)
            {
                switch (info.ExAttributes[j].Name)
                {
                    case "ATK": this.ATK += info.ExAttributes[j].Value; break;
                    case "MaxHP": this.MaxHP += info.ExAttributes[j].Value; break;
                    case "DEF": this.DEF += info.ExAttributes[j].Value; break;
                    case "CritP": this.CritP += info.ExAttributes[j].Value; break;
                    case "CritV": this.CritV += info.ExAttributes[j].Value; break;
                    case "ChainAtk": this.ChainAtk += info.ExAttributes[j].Value; break;
                    case "SpExtra": this.SpExtra += info.ExAttributes[j].Value; break;
                    case "SpRes": this.SpRes += info.ExAttributes[j].Value; break;
                    case "MaxHPrate": this.MaxHPrate += info.ExAttributes[j].Value; break;
                    case "ATKrate": this.ATKrate += info.ExAttributes[j].Value; break;
                    case "DEFrate": this.DEFrate += info.ExAttributes[j].Value; break;
                    case "Speed": this.Speed += info.ExAttributes[j].Value; break;
                    case "speedrate": this.Speedrate += info.ExAttributes[j].Value; break;
                    default: EB.Debug.LogWarning("Equipment ExAttribute Miss{0}", info.ExAttributes[j].Name); break;
                }
            }
        }
    }

    public class SuitAttrsSuitTypeAndCount
    {
        public int SuitType;
        public int count;
        public SuitAttrsSuitTypeAndCount(int STid)
        {
            SuitType = STid;
            count = 1;
        }
    }

    public class SuitAttrAdditions
    {
        public float MaxHPrate;
        public float ATKrate;
        public float DEFrate;
        public float CritP;
        public float CritV;
        //public float ChainAtk;
        public float SpExtra;
        public float SpRes;
        public float Speedrate;

        public void Add(SuitAttrAdditions other)
        {
            this.MaxHPrate += other.MaxHPrate;
            this.ATKrate += other.ATKrate;
            this.DEFrate += other.DEFrate;
            this.CritP += other.CritP;
            this.CritV += other.CritV;
            this.SpExtra += other.SpExtra;
            this.SpRes += other.SpRes;
            this.Speedrate += other.Speedrate;
        }
    }
    public struct HeroEquipmentInfo
    {
        public EquipPartType type;//装备类型none为不存在,part1为1号位，part2为2号位...
        public int Eid;//装备物品id
        public int ECid;//装备读表id;
    }


    /// <summary>
    /// 伙伴技能突破物品排序
    /// </summary>
    public class LTSkillBreakGoodsComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            IDictionary xData;
            IDictionary yData;
            DataLookupsCache.Instance.SearchDataByID<IDictionary>(string.Format("inventory.{0}", x), out xData);
            DataLookupsCache.Instance.SearchDataByID<IDictionary>(string.Format("inventory.{0}", y), out yData);

            if (xData == null || yData == null) return 1;

            int xEconomyId = EB.Dot.Integer("economy_id", xData, 0);
            int yEconomyId = EB.Dot.Integer("economy_id", yData, 0);
            Hotfix_LT.Data.EconemyItemTemplate xItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(xEconomyId);
            Hotfix_LT.Data.EconemyItemTemplate yItemData = Hotfix_LT.Data.EconemyTemplateManager.Instance.GetItem(yEconomyId);
            int xExp = (xItemData as Hotfix_LT.Data.GeneralItemTemplate) == null ? (xItemData as Hotfix_LT.Data.EquipmentItemTemplate).BaseExp : (xItemData as Hotfix_LT.Data.GeneralItemTemplate).Exp;
            int yExp = (yItemData as Hotfix_LT.Data.GeneralItemTemplate) == null ? (yItemData as Hotfix_LT.Data.EquipmentItemTemplate).BaseExp : (yItemData as Hotfix_LT.Data.GeneralItemTemplate).Exp;

            if (xExp != yExp)
            {
                return yExp - xExp;
            }
            else
            {
                return xEconomyId - yEconomyId;
            }
        }
    }
}