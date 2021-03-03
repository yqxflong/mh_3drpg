using System.Collections;

namespace Hotfix_LT.UI
{
    public class AttributesData
    {
        public double m_Inc_MaxHP;
        public double m_Inc_PDEF;
        public double m_Inc_MDEF;
        public double m_Inc_PATK;
        public double m_Inc_MATK;
        public double m_MaxHP;
        public double m_PDEF;
        public double m_PATK;
        public double m_MDEF;
        public double m_MATK;
        public double m_Speed;
        public double m_Stun;
        public double m_Critical;
        public double m_CriticalHit;
        public double m_Penetration;
        public double m_Damage_Reduction;
        public double m_Heal_Recover;
        public double m_Spell_Penetration;
        public double m_Dodge_Rating;
        public double m_Threaten;
        public double m_Anti_Mu_Rating;
        public double m_Anti_Jin_Rating;
        public double m_Anti_Shui_Rating;
        public double m_Anti_Huo_Rating;
        public double m_Anti_Tu_Rating;

        public double m_Training_MaxHP;
        public double m_Training_PATK;
        public double m_Training_PDEF;
        public double m_Training_MATK;
        public double m_Training_MDEF;

        public AttributesData()
        {
            m_Inc_MaxHP = 0;
            m_Inc_PDEF = 0;
            m_Inc_MDEF = 0;
            m_Inc_PATK = 0;
            m_Inc_MATK = 0;
            m_MaxHP = 0;
            m_PDEF = 0;
            m_PATK = 0;
            m_MDEF = 0;
            m_MATK = 0;
            m_Speed = 0;
            m_Stun = 0;
            m_Critical = 0;
            m_CriticalHit = 0;
            m_Penetration = 0;
            m_Damage_Reduction = 0;
            m_Heal_Recover = 0;
            m_Spell_Penetration = 0;
            m_Dodge_Rating = 0;
            m_Threaten = 0;
            m_Anti_Mu_Rating = 0;
            m_Anti_Jin_Rating = 0;
            m_Anti_Shui_Rating = 0;
            m_Anti_Huo_Rating = 0;
            m_Anti_Tu_Rating = 0;
            m_Training_MaxHP = 0;
            m_Training_PATK = 0;
            m_Training_PDEF = 0;
            m_Training_MATK = 0;
            m_Training_MDEF = 0;
        }

        public AttributesData(AttributesData data)
        {
            m_Inc_MaxHP = data.m_Inc_MaxHP;
            m_Inc_PDEF = data.m_Inc_PDEF;
            m_Inc_MDEF = data.m_Inc_MDEF;
            m_Inc_PATK = data.m_Inc_PATK;
            m_Inc_MATK = data.m_Inc_MATK;
            m_MaxHP = data.m_MaxHP;
            m_PDEF = data.m_PDEF;
            m_PATK = data.m_PATK;
            m_MDEF = data.m_MDEF;
            m_MATK = data.m_MATK;
            m_Speed = data.m_Speed;
            m_Stun = data.m_Stun;
            m_Critical = data.m_Critical;
            m_CriticalHit = data.m_CriticalHit;
            m_Penetration = data.m_Penetration;
            m_Damage_Reduction = data.m_Damage_Reduction;
            m_Heal_Recover = data.m_Heal_Recover;
            m_Spell_Penetration = data.m_Spell_Penetration;
            m_Dodge_Rating = data.m_Dodge_Rating;
            m_Threaten = data.m_Threaten;
            m_Anti_Mu_Rating = data.m_Anti_Mu_Rating;
            m_Anti_Jin_Rating = data.m_Anti_Jin_Rating;
            m_Anti_Shui_Rating = data.m_Anti_Shui_Rating;
            m_Anti_Huo_Rating = data.m_Anti_Huo_Rating;
            m_Anti_Tu_Rating = data.m_Anti_Tu_Rating;
            m_Training_MaxHP = data.m_Training_MaxHP;
            m_Training_PATK = data.m_Training_PATK;
            m_Training_PDEF = data.m_Training_PDEF;
            m_Training_MATK = data.m_Training_MATK;
            m_Training_MDEF = data.m_Training_MDEF;
        }

        public void Add(AttributesData data)
        {
            m_Inc_MaxHP += data.m_Inc_MaxHP;
            m_Inc_PDEF += data.m_Inc_PDEF;
            m_Inc_MDEF += data.m_Inc_MDEF;
            m_Inc_PATK += data.m_Inc_PATK;
            m_Inc_MATK += data.m_Inc_MATK;
            m_MaxHP += data.m_MaxHP;
            m_PDEF += data.m_PDEF;
            m_PATK += data.m_PATK;
            m_MDEF += data.m_MDEF;
            m_MATK += data.m_MATK;
            m_Speed += data.m_Speed;
            m_Stun += data.m_Stun;
            m_Critical += data.m_Critical;
            m_CriticalHit += data.m_CriticalHit;
            m_Penetration += data.m_Penetration;
            m_Damage_Reduction += data.m_Damage_Reduction;
            m_Heal_Recover += data.m_Heal_Recover;
            m_Spell_Penetration += data.m_Spell_Penetration;
            m_Dodge_Rating += data.m_Dodge_Rating;
            m_Threaten += data.m_Threaten;
            m_Anti_Mu_Rating += data.m_Anti_Mu_Rating;
            m_Anti_Jin_Rating += data.m_Anti_Jin_Rating;
            m_Anti_Shui_Rating += data.m_Anti_Shui_Rating;
            m_Anti_Huo_Rating += data.m_Anti_Huo_Rating;
            m_Anti_Tu_Rating += data.m_Anti_Tu_Rating;
            m_Training_MaxHP += data.m_Training_MaxHP;
            m_Training_PATK += data.m_Training_PATK;
            m_Training_PDEF += data.m_Training_PDEF;
            m_Training_MATK += data.m_Training_MATK;
            m_Training_MDEF += data.m_Training_MDEF;
        }

        public void Mul(double mul)
        {
            m_Inc_MaxHP *= mul;
            m_Inc_PDEF *= mul;
            m_Inc_MDEF *= mul;
            m_Inc_PATK *= mul;
            m_Inc_MATK *= mul;
            m_MaxHP *= mul;
            m_PDEF *= mul;
            m_PATK *= mul;
            m_MDEF *= mul;
            m_MATK *= mul;
            m_Speed *= mul;
            m_Stun *= mul;
            m_Critical *= mul;
            m_CriticalHit *= mul;
            m_Penetration *= mul;
            m_Damage_Reduction *= mul;
            m_Heal_Recover *= mul;
            m_Spell_Penetration *= mul;
            m_Dodge_Rating *= mul;
            m_Threaten *= mul;
            m_Anti_Mu_Rating *= mul;
            m_Anti_Jin_Rating *= mul;
            m_Anti_Shui_Rating *= mul;
            m_Anti_Huo_Rating *= mul;
            m_Anti_Tu_Rating *= mul;
            m_Training_MaxHP *= mul;
            m_Training_PATK *= mul;
            m_Training_PDEF *= mul;
            m_Training_MATK *= mul;
            m_Training_MDEF *= mul;
        }

        public void SetDataByBuddy(Hashtable hashtable)
        {
            m_Inc_MaxHP = EB.Dot.Double("inc_MaxHP", hashtable, 0.0);
            m_Inc_PDEF = EB.Dot.Double("inc_PDEF", hashtable, 0.0);
            m_Inc_MDEF = EB.Dot.Double("inc_MDEF", hashtable, 0.0);
            m_Inc_PATK = EB.Dot.Double("inc_PATK", hashtable, 0.0);
            m_Inc_MATK = EB.Dot.Double("inc_MATK", hashtable, 0.0);
            m_MaxHP = EB.Dot.Double("base_MaxHP", hashtable, 0.0);
            m_PDEF = EB.Dot.Double("base_PDEF", hashtable, 0.0);
            m_PATK = EB.Dot.Double("base_PATK", hashtable, 0.0);
            m_MDEF = EB.Dot.Double("base_MDEF", hashtable, 0.0);
            m_MATK = EB.Dot.Double("base_MATK", hashtable, 0.0);
            m_Speed = EB.Dot.Double("speed", hashtable, 0.0);
            m_Stun = EB.Dot.Double("stun", hashtable, 0.0);
            m_Critical = EB.Dot.Double("critical", hashtable, 0.0);
            m_CriticalHit = EB.Dot.Double("critical_hit", hashtable, 0.0);
            m_Penetration = EB.Dot.Double("penetration", hashtable, 0.0);
            m_Damage_Reduction = EB.Dot.Double("damage_reduction", hashtable, 0.0);
            m_Heal_Recover = EB.Dot.Double("heal_recover", hashtable, 0.0);
            m_Spell_Penetration = EB.Dot.Double("spell_penetration", hashtable, 0.0);
            m_Dodge_Rating = EB.Dot.Double("dodge_rating", hashtable, 0.0);
            m_Threaten = EB.Dot.Double("threaten", hashtable, 0.0);

            m_Anti_Mu_Rating = EB.Dot.Double("anti_mu_rating", hashtable, 0.0);
            m_Anti_Jin_Rating = EB.Dot.Double("anti_jin_rating", hashtable, 0.0);
            m_Anti_Shui_Rating = EB.Dot.Double("anti_shui_rating", hashtable, 0.0);
            m_Anti_Huo_Rating = EB.Dot.Double("anti_huo_rating", hashtable, 0.0);
            m_Anti_Tu_Rating = EB.Dot.Double("anti_tu_rating", hashtable, 0.0);

            m_Training_MaxHP += EB.Dot.Double("training_MaxHP", hashtable, 0.0);
            m_Training_PATK += EB.Dot.Double("training_PATK", hashtable, 0.0);
            m_Training_PDEF += EB.Dot.Double("training_PDEF", hashtable, 0.0);
            m_Training_MATK += EB.Dot.Double("training_MATK", hashtable, 0.0);
            m_Training_MDEF += EB.Dot.Double("training_MDEF", hashtable, 0.0);
        }

        public void SetDataByBuddy(Hotfix_LT.Data.HeroStatTemplate hero)
        {
            /*m_Inc_MaxHP = hero.inc_MaxHP ;
            m_Inc_PDEF = hero.inc_PDEF;
            m_Inc_MDEF = hero.inc_MDEF; 
            m_Inc_PATK = hero.inc_PATK;
            m_Inc_MATK = hero.inc_MATK;
            m_MaxHP = hero.base_MaxHP;
            m_PDEF = hero.base_PDEF; 
            m_PATK = hero.base_PATK;
            m_MDEF = hero.base_MDEF; 
            m_MATK = hero.base_MATK; 
            m_Speed = hero.speed; 
            m_Stun = hero.stun; 
            m_Critical = hero.critical;
            m_CriticalHit = hero.critical_hit;
            m_Penetration = hero.penetration; 
            m_Damage_Reduction = hero.damage_reduction; 
            m_Heal_Recover = hero.heal_recover; 
            m_Spell_Penetration = hero.spell_penetration;
            m_Dodge_Rating = hero.dodge_rating;
            m_Threaten = hero.threaten; 

            m_Anti_Mu_Rating = hero.anti_mu_rating;
            m_Anti_Jin_Rating = hero.anti_jin_rating;
            m_Anti_Shui_Rating = hero.anti_shui_rating; 
            m_Anti_Huo_Rating = hero.anti_huo_rating;
            m_Anti_Tu_Rating = hero.anti_tu_rating;

            m_Training_MaxHP += hero.training_MaxHP;
            m_Training_PATK += hero.training_PATK;
            m_Training_PDEF += hero.training_PDEF; 
            m_Training_MATK += hero.training_MATK; 
            m_Training_MDEF += hero.training_MDEF;*/
        }

        public Hashtable ToHashTable()
        {
            Hashtable hashtable = Johny.HashtablePool.Claim();
            if (m_Inc_MaxHP > 0) hashtable.Add("inc_MaxHP", m_Inc_MaxHP);
            if (m_Inc_PDEF > 0.0001) hashtable.Add("inc_PDEF", m_Inc_PDEF);
            if (m_Inc_MDEF > 0.0001) hashtable.Add("inc_MDEF", m_Inc_MDEF);
            if (m_Inc_PATK > 0.0001) hashtable.Add("inc_PATK", m_Inc_PATK);
            if (m_Inc_MATK > 0.0001) hashtable.Add("inc_MATK", m_Inc_MATK);
            if (m_MaxHP > 0.0001) hashtable.Add("base_MaxHP", m_MaxHP);

            if (m_PDEF > 0.0001) hashtable.Add("base_PDEF", m_PDEF);
            if (m_PATK > 0.0001) hashtable.Add("base_PATK", m_PATK);
            if (m_MDEF > 0.0001) hashtable.Add("base_MDEF", m_MDEF);
            if (m_MATK > 0.0001) hashtable.Add("base_MATK", m_MATK);
            if (m_Speed > 0.0001) hashtable.Add("speed", m_Speed);
            if (m_Stun > 0.0001) hashtable.Add("stun", m_Stun);
            if (m_Critical > 0.0001) hashtable.Add("critical", m_Critical);
            if (m_CriticalHit > 0.0001) hashtable.Add("critical_hit", m_CriticalHit);
            if (m_Penetration > 0.0001) hashtable.Add("penetration", m_Penetration);
            if (m_Damage_Reduction > 0.0001) hashtable.Add("damage_reduction", m_Damage_Reduction);
            if (m_Heal_Recover > 0.0001) hashtable.Add("heal_recover", m_Heal_Recover);
            if (m_Spell_Penetration > 0.0001) hashtable.Add("spell_penetration", m_Spell_Penetration);
            if (m_Dodge_Rating > 0.0001) hashtable.Add("dodge_rating", m_Dodge_Rating);
            if (m_Threaten > 0.0001) hashtable.Add("threaten", m_Threaten);

            if (m_Anti_Mu_Rating > 0.0001) hashtable.Add("anti_mu_rating", m_Anti_Mu_Rating);
            if (m_Anti_Jin_Rating > 0.0001) hashtable.Add("anti_jin_rating", m_Anti_Jin_Rating);
            if (m_Anti_Shui_Rating > 0.0001) hashtable.Add("anti_shui_rating", m_Anti_Shui_Rating);
            if (m_Anti_Huo_Rating > 0.0001) hashtable.Add("anti_huo_rating", m_Anti_Huo_Rating);
            if (m_Anti_Tu_Rating > 0.0001) hashtable.Add("anti_tu_rating", m_Anti_Tu_Rating);

            if (m_Training_MaxHP > 0.0001) hashtable.Add("training_MaxHP", m_Training_MaxHP);
            if (m_Training_PATK > 0.0001) hashtable.Add("training_PATK", m_Training_PATK);
            if (m_Training_PDEF > 0.0001) hashtable.Add("training_PDEF", m_Training_PDEF);
            if (m_Training_MATK > 0.0001) hashtable.Add("training_MATK", m_Training_MATK);
            if (m_Training_MDEF > 0.0001) hashtable.Add("training_MDEF", m_Training_MDEF);
            return hashtable;
        }
    }

    public class LTAttributesData
    {
        public float m_MaxHP;          //生命
        public float m_ATK;            //攻击力
        public float m_DEF;            //防御力
        public float m_ChainATK;       //连击率
        public float m_CritP;          //暴击
        public float m_CritV;          //暴击伤害
        public float m_CritDef;        //暴击抵抗
        public float m_SpExtra;        //效果命中
        public float m_SpRes;          //效果抵抗
        public float m_Speed;          //速度
        public float m_DamageReduce;   //伤害增加
        public float m_DamageAdd;      //伤害减免

        //特殊百分比
        public float m_MaxHPrate;          //生命
        public float m_ATKrate;            //攻击力
        public float m_DEFrate;            //防御力
        //public float m_speedrate;          //速度

        public LTAttributesData()
        {
            m_MaxHP = 0;
            m_ATK = 0;
            m_DEF = 0;
            m_ChainATK = 0;
            m_CritP = 0;
            m_CritV = 0;
            m_CritDef = 0;
            m_SpExtra = 0;
            m_SpRes = 0;
            m_Speed = 0;
            m_DamageReduce = 0;
            m_DamageAdd = 0;
        }

        public LTAttributesData(LTAttributesData data)
        {
            m_MaxHP = data.m_MaxHP;
            m_ATK = data.m_ATK;
            m_DEF = data.m_DEF;
            m_ChainATK = data.m_ChainATK;
            m_CritP = data.m_CritP;
            m_CritV = data.m_CritV;      
            m_SpExtra = data.m_SpExtra;
            m_SpRes = data.m_SpRes;
            m_Speed = data.m_Speed;
            m_CritDef = data.m_CritDef;
            m_DamageReduce = data.m_DamageReduce;
            m_DamageAdd = data.m_DamageAdd;
        }

        /// <summary>
        /// 属性相加
        /// </summary>
        /// <param name="data"></param>
        public void Add(LTAttributesData data)
        {
            if (data == null) return;
            m_MaxHP += data.m_MaxHP;
            m_ATK += data.m_ATK;
            m_DEF += data.m_DEF;
            m_ChainATK += data.m_ChainATK;
            m_CritP += data.m_CritP;
            m_CritV += data.m_CritV;
            m_SpExtra += data.m_SpExtra;
            m_SpRes += data.m_SpRes;
            m_Speed += data.m_Speed;
            m_CritDef += data.m_CritDef;
            m_DamageReduce += data.m_DamageReduce;
            m_DamageAdd += data.m_DamageAdd;
        }

        /// <summary>
        /// 属性相乘百分比
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="data"></param>
        public void SpecialAdd(LTAttributesData baseData, LTAttributesData data)
        {
            if (data == null|| baseData==null) return;
            m_MaxHP += baseData.m_MaxHP * data.m_MaxHPrate;
            m_ATK += baseData.m_ATK * data.m_ATKrate;
            m_DEF += baseData.m_DEF * data.m_DEFrate;
        }

        public void Add(string name, float value)
        {
            if (string.IsNullOrEmpty(name)|| value==0) return;
            switch (name)
            {
                case "ATK": m_ATK += value; break;
                case "MaxHP": m_MaxHP += value; break;
                case "DEF": m_DEF += value; break;

                case "MaxHPrate": m_MaxHPrate += value; break;
                case "ATKrate": m_ATKrate += value; break;
                case "DEFrate": m_DEFrate += value; break;

                case "CritP": m_CritP += value; break;
                case "CritV": m_CritV += value; break;

                case "ChainAtk": m_ChainATK += value; break;

                case "SpExtra": m_SpExtra += value; break;
                case "SpRes": m_SpRes += value; break;
           
                case "speed": m_Speed += value; break;
                //case "speedrate": m_speedrate += value; break;

                case "DmgMulti": m_DamageAdd += value; break;
                case "DmgRes": m_DamageReduce += value; break;

                default: EB.Debug.LogError(string.Format("{0}:{1}", EB.Localizer.GetString("ID_ATTR_Unknown"),name)); break;
            }
            
        }

        /// <summary>
        /// 属性相减
        /// </summary>
        /// <param name="data"></param>
        public void Sub(LTAttributesData data)
        {
            if (data == null) return;
            m_MaxHP -= data.m_MaxHP;
            m_ATK -= data.m_ATK;
            m_DEF -= data.m_DEF;
            m_ChainATK -= data.m_ChainATK;
            m_CritP -= data.m_CritP;
            m_CritV -= data.m_CritV;
            m_SpExtra -= data.m_SpExtra;
            m_SpRes -= data.m_SpRes;
            m_Speed -= data.m_Speed;
            m_CritDef -= data.m_CritDef;
            m_DamageReduce -= data.m_DamageReduce;
            m_DamageAdd -= data.m_DamageAdd;
        }

        /// <summary>
        /// 属性百分比加成
        /// </summary>
        /// <param name="mul"></param>
        public void Mul(float mul)
        {
            m_MaxHP *= mul;
            m_ATK *= mul;
            m_DEF *= mul;
            m_ChainATK *= mul;
            m_CritP *= mul;
            m_CritV *= mul;
            m_SpExtra *= mul;
            m_SpRes *= mul;
            m_Speed *= mul;
            m_CritDef *= mul;
            m_DamageReduce *= mul;
            m_DamageAdd *= mul;
        }

        /// <summary>
        /// 各属性的百分比加成
        /// </summary>
        /// <param name="dataPercentage">百分比数值，例10%，则存的是0.1（特殊用法）</param>
        public void Mul(LTAttributesData dataPercentage)
        {
            if (dataPercentage == null) return;
            m_MaxHP *= dataPercentage.m_MaxHP;
            m_ATK *= dataPercentage.m_ATK;
            m_DEF *= dataPercentage.m_DEF;
            m_ChainATK *= dataPercentage.m_ChainATK;
            m_CritP *= dataPercentage.m_CritP;
            m_CritV *= dataPercentage.m_CritV;
            m_SpExtra *= dataPercentage.m_SpExtra;
            m_SpRes *= dataPercentage.m_SpRes;
            m_Speed *= dataPercentage.m_Speed;
            m_CritDef *= dataPercentage.m_CritDef;
            m_DamageReduce *= dataPercentage.m_DamageReduce;
            m_DamageAdd *= dataPercentage.m_DamageAdd;
        }

        public float Power()
        {
            float power = m_MaxHP* LTPartnerConfig.MaxHpper + LTPartnerConfig.Atkper * m_ATK + LTPartnerConfig.Defper * m_DEF;
            return power;
        }

        public float PowerPer()
        {
            //伤害增加  伤害减免 效果命中 抵抗 暴击 暴伤 爆抗 速度 连击
            float powerper = (float) (m_CritP*LTPartnerConfig.CritPper  + m_CritV * LTPartnerConfig.CritVper + m_CritDef*LTPartnerConfig.CritDper + m_SpExtra* LTPartnerConfig.SpExtraper +
                m_SpRes* LTPartnerConfig.SpResper + m_Speed * LTPartnerConfig.Speedper + m_DamageAdd* LTPartnerConfig.DmgAddper + m_DamageReduce* LTPartnerConfig.DmgReper +m_ChainATK*LTPartnerConfig.ChainATKper);
            return powerper;
        }
      
        
    }

    public class PowerData
    {
        public enum RefreshType
        {
            All = 0,
            Attribute = 1,
            Skill = 2,
            EquipsuitSkill = 3,           
        }
        public int curPower;
        private int perPower;
        private int add;
        private string str;
        private float attripower;
        private float skillper;
        private float suitskillper;
        public bool isNeedRefresh = false;


        public PowerData(){}
        public PowerData(LTPartnerData data)
        {
            if(data.Level == 0)
            {
                curPower = 0;
            }
            else{
                calculate(data, RefreshType.All);
            }
        }

        public void OnValueChanged(LTPartnerData data,bool isShow, RefreshType type)
        {
            perPower = curPower;
            calculate(data, type);
            add = curPower - perPower;
            if (isShow && add != 0)
            {
                str = add >= 0 ? "+" : "";
                MessageTemplateManager.ShowMessage(eMessageUIType.CombatPowerText, string.Format("{0},{1}{2}", curPower, str, add));
            }

        }

        private void calculate(LTPartnerData data, RefreshType type)
        {
            switch (type)
            {
                case RefreshType.Attribute:
                    attripower = AttributesManager.GetSelfAttributeCombatPower(data);
                    break;
                case RefreshType.Skill:
                    skillper = AttributesManager.GetPartnerSkillPowerPer(data);
                    attripower = AttributesManager.GetSelfAttributeCombatPower(data);
                    break;
                case RefreshType.EquipsuitSkill:
                    suitskillper = AttributesManager.GetEquipSuitPer(data);
                    attripower = AttributesManager.GetSelfAttributeCombatPower(data);
                    break;
                case RefreshType.All:
                    attripower = AttributesManager.GetSelfAttributeCombatPower(data);
                    skillper = AttributesManager.GetPartnerSkillPowerPer(data);
                    suitskillper = AttributesManager.GetEquipSuitPer(data);
                    break;
                default:
                    break;
            }
            curPower = (int)(attripower * (1 + skillper + suitskillper));
        }

    }
}