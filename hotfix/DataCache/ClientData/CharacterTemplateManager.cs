using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hotfix_LT.UI;
using ILRuntime.Runtime;
using UnityEngine;
using GM.DataCache;

namespace Hotfix_LT.Data
{
    public enum eRoleAttr
    {
        None = -1,
        Feng = 1,
        Shui,
        Huo
    }
    
    public class HeroGuardTemplate
    {
        //单独设置
        public bool IsMaxLevel{ get; set; }
        public int Index { get; set; }
        public int Condition  { get; set; }
        public string Param { get; set; }
        public int Level  { get; set; }
        public float IncMaxHP  { get; set; }
        public float IncATK  { get; set; }
        public float IncDEF  { get; set; }

        public HeroGuardTemplate()
        {
            Index = 0;
            Condition = 0;
            Param = "";
            Level = 0;
            IncMaxHP = 0;
            IncATK = 0;
            IncDEF = 0;
        }

        public HeroGuardTemplate(GM.DataCache.HeroFetter heroFetter)
        {
            Index = heroFetter.Index;
            Condition =heroFetter.Condition;
            Param =heroFetter.Param;
            Level = heroFetter.Level;
            IncMaxHP =heroFetter.IncMaxHP;
            IncATK = heroFetter.IncATK;
            IncDEF = heroFetter.IncDEF;
        }
    }


    public class HeroStatTemplate
    {
        public int id { get { return int.Parse(heroStats.Id); } }
        public int character_id { get { return int.Parse(heroStats.CharacterId); } }
        public string name { get { return heroStats.Name; } }

        public int active_skill { get { return heroStats.ActiveSkill; } }
        public int common_skill { get { return heroStats.CommonSkill; } }
        public int passive_skill { get { return heroStats.PassiveSkill; } }

        public int starskill5 { get { return heroStats.StarSkill5; } }
        public int starskill6 { get { return heroStats.StarSkill6; } }
        public int HeroFetter1 {get { return int.Parse(heroStats.HeroFetter1); }  }
        public int HeroFetter2 {get { return int.Parse(heroStats.HeroFetter2); }  }
        public int HeroFetter3 {get { return int.Parse(heroStats.HeroFetter3); }  }
        public GM.DataCache.HeroStat heroStats;
    }

    public class HeroInfoTemplate
    {
        public int id { get { return int.Parse(obj.Id); } }
        public string name { get { return EB.Localizer.GetTableString(string.Format("ID_herostats_hero_infos_{0}_name", id), obj.Name); } }
        public eRoleAttr char_type { get { return (eRoleAttr)obj.CharType; } }
        public int race { get { return obj.Race; } }
        public int summon_shard { get { return obj.SummonShard; } }
        public int init_star { get { return obj.InitStar; } }
        public string role_profile_text { get { return EB.Localizer.GetTableString(string.Format("ID_herostats_hero_infos_{0}_role_profile_text", id), obj.RoleProfileText); } }
        public string role_profile_icon { get { return obj.RoleProfileIcon; } }
        public int role_grade {
            get
            {
                int result;
                int.TryParse(obj.RoleGrade, out result);
                return result;
            }}
        public string role_profile { get { return EB.Localizer.GetTableString(string.Format("ID_herostats_hero_infos_{0}_role_profile", id), obj.RoleProfile); } }
        public LobbyCameraData lobby_camera { get { return SetCameraData(obj.LobbyCamera); } }
        public string portrait { get { return obj.Portrait; } }

        public int draw { get { return obj.Draw; } }
        public int cultivateGift { get { return obj.CultivateGift; } }
        public bool isShowInClash { get { return obj.ShowInClash == 1; } }
        public bool isShowInWiki { get { return obj.ShowInWiki == 1; } }
        public string icon { get; set; }
        public string model_name { get; set; }
        public string skin { get; set; }
        public int reward { get { return obj.Reward; } }

        public int suitAttributeId1 { get { return obj.SuitAttributeId1; } }
        public string suitAttributeId2 { get { return obj.SuitAttributeId2; } }

        public GM.DataCache.HeroInfo obj;

        private LobbyCameraData SetCameraData(string lobbyCamera)
        {
            string cameraParam;
            if (lobbyCamera != null)
            {
                cameraParam = lobbyCamera;
            }
            else
            {
                cameraParam = "0,2.3,0,9,16,-3,1.4,1";
                //return new LobbyCameraData { Orthographic = true, Position = Vector3.zero, Rotation = Vector3.zero, Size = 0 };
            }

            string[] sArray = cameraParam.Split(',');
            float[] fArray = LTUIUtil.ToFloat(sArray);
            Vector3 position = new Vector3(fArray[0], fArray[1], fArray[2]);
            Vector3 rotation = new Vector3(fArray[3], fArray[4], fArray[5]);
            float size = fArray[6];
            bool orthographic = fArray[7] != 0;

            Vector3 iconPosition = new Vector3(0, -6, 0);
            Vector3 iconRot = Vector3.zero;
            if (fArray.Length > 8)
            {
                iconPosition = new Vector3(fArray[8], fArray[9], fArray[10]);
                iconRot = new Vector3(fArray[11], fArray[12], fArray[13]);
            }

            LobbyCameraData lobbyCameraData = new LobbyCameraData
            {
                Orthographic = orthographic,
                Position = position,
                Rotation = rotation,
                Size = size,
                IconPosition = iconPosition,
                IconRotation = iconRot
            };
            return lobbyCameraData;
        }
    }


    public class MonsterInfoTemplate
    {
        public int id { get { return int.Parse(obj.Id); } }
        public string character_id { get { return obj.CharacterId; } }
        public string name { get { return EB.Localizer.GetTableString(string.Format("ID_herostats_hero_infos_{0}_name", character_id), obj.Name); } }

        public int level { get { return obj.Level; } }
        public int star { get { return obj.Star; } }
        public int upgrade { get { return obj.Upgrade; } }

        public float base_MaxHP { get; set; }
        public float base_ATK { get; set; }
        public float base_DEF { get; set; }
        public float speed { get; set; }
        public float CritP  { get; set; }
        public float CritV  { get; set; }
        public float SpExtra { get; set; }
        public float SpRes  { get; set; }

        public int active_skill_3 { get { return obj.ActiveSkill3; } }
        public int common_skill { get { return obj.CommonSkill; } }
        public int passive_skill_1 { get { return obj.PassiveSkill1; } }
        public int passive_skill_2 { get { return obj.PassiveSkill2; } }
        public int passive_skill_3 { get { return obj.PassiveSkill3; } }
        public int monster_type { get { return obj.MonsterType; } }
        public int hp_number { get; set; }
        public float scale_mul { get { return obj.ScaleMultiple; } }
        public string spawn_camera { get { return obj.SpawnCamera; } }
        private GM.DataCache.Monster _obj;
        public GM.DataCache.Monster obj 
        {
            get { return _obj; }
            set
            {
                _obj = value;

                base_ATK = _obj.BaseATK;
                base_DEF = _obj.BaseDEF;
                base_MaxHP = _obj.BaseMaxHP;
                speed = _obj.Speed;
                CritP = _obj.CritP;
                CritV = _obj.CritV;
                SpExtra = _obj.SpExtra;
                SpRes = _obj.SpRes;
                hp_number = _obj.HpNumber;
            }
        }
    }

    public class StarInfoTemplate
    {
        public string id;
        public int star_level;
        public int hole_position;
        public int cost_shard;
        public string attr;
        public float param;
        public int level;
    }

    public class StarUpInfoTemplate
    {
        public int StarId;
        public float IncMaxHp;
        public float IncATK;
        public float IncDEF;
    }

    public class UpGradeInfoTemplate
    {
        public string name;
        public int quality;
        public int upGradeId;
        public float inc_maxhp;
        public float inc_atk;
        public float inc_def;
        public float inc_speed;
        public int level;
        public int level_limit;
        public int needHcNum;
        public int needGoldNum;
        public eRoleAttr char_type;
        public Dictionary<string, int> materialDic;
    }

    public class HeroAwakeInfoTemplate
    {
        public int id;
        public string name;
        public int limitLevel;
        public int limitUpgrade;
        public int limitStar;
        public string awakeHeadIcon;
        public string awakeSkin;
        public int awakeType;
        public float DmgMulti = 0.0f;
        public float DmgRes = 0.0f;
        public int speedAdd = 0;
        public float SpExtra = 0.0f;
        public float SpRes = 0.0f;
        public float AntiCritP = 0.0f;
        public float CritP = 0.0f;
        public float CritV = 0.0f;
        public int beforeSkill;
        public int laterSkill;
        public Dictionary<string, int> awakeMaterDic;
        public float inc_MaxHP;
        public float inc_ATK;
        public float inc_DEF;
        public float inc_speed;

    }
    public class SkillLevelTemplate
    {
        public int level;
        public int starLimit;
        public int exp;
    }

    #region 图鉴相关
    public class MannualBreakTemplate
    {
        public eRoleAttr type;
        public int level;
        public string material_1;
        public int quantity_1;

        public float score_promotion;//废弃

        public float IncMaxHp;
        public float IncATK;
        public float IncDEF;
    }

    public class MannualScoreTemplate
    {
        public int id;
        public int score;
        public int totleScore;
        public string evaluate;//废弃，改为levelLimit
        public float attribute_addition;
        //新图鉴
        public int levelLimit;
        public float maxHP;
        public float ATK;
        public float DEF;
    }

    public class MannualRoleGradeTemplate
    {
        public int id;
        public float score_addition;//废弃，改为ScoreList
        public float star_addition;
        //新图鉴
        public List<int> ScoreList;
    }

    public class MannualUpgradeScoreTemplate
    {
        public string upgrade;
        public int score;
    }
    #endregion

    public class HeroLevelInfoTemplate
    {
        public int level;
        public int buddy_exp;
    }

    public class LevelUpInfoTemplate
    {
        public string id { get { return obj.Id; } }
        public int level { get { return obj.Level; } }
        public float maxHP {
            get { 
                if(obj == null)
                {
                    EB.Debug.LogError("LevelUpInfoTemplate.mapHp===>obj == null!!!");
                    return 0;
                }
                else
                    return obj.MaxHP; 
            } 
        }
        public float ATK {
            get{
                if(obj == null)
                {
                    EB.Debug.LogError("obj == null!!!");
                    return 0.0f;
                } 
                return obj.ATK; 
            } 
        }
        public float DEF { get { return obj.DEF; } }
        public float speed { get { return obj.Speed; } }
        public float CritP { get { return obj.CritP; } }
        public float CritV { get { return obj.CritV; } }
        public float SpExtra { get { return obj.SpExtra; } }
        public float SpRes{ get { return obj.SpRes; } }

        public GM.DataCache.LevelUp obj;
    }

    public class PlayerLevelTemplate
    {
        public int level;
        public int expRequirement;
    }

    public enum ProficiencyType
    {
        AllRound = 1,
        Control,
        Strong,
        Rage,
        Absorbed,
    }

    public class ProficiencyUpTemplate
    {
        public int id;
        public int form;
        public ProficiencyType type;
        public int level;

        public int[] chipCost;
        public int potenCost;
        public int[] goldCost;

        public float maxHP;
        public float ATK;
        public float DEF;
        public float speed;
        public float CritP;
        public float CritV;
        public float SpExtra;
        public float SpRes;
        public float AntiCritP;
        public float DmgMulti;
        public float DmgRes;

        public ProficiencyUpTemplate()
        {

        }
        public ProficiencyUpTemplate(ProficiencyUpTemplate cur, ProficiencyUpTemplate next)
        {
            this.maxHP = (cur != null) ? next.maxHP - cur.maxHP : next.maxHP;
            this.ATK = (cur != null) ? next.ATK - cur.ATK : next.ATK;
            this.DEF = (cur != null) ? next.DEF - cur.DEF : next.DEF;
            this.speed = (cur != null) ? next.speed - cur.speed : next.speed;
            this.CritP = (cur != null) ? next.CritP - cur.CritP : next.CritP;
            this.SpExtra = (cur != null) ? next.SpExtra - cur.SpExtra : next.SpExtra;
            this.CritV = (cur != null) ? next.CritV - cur.CritV : next.CritV;
            this.SpRes = (cur != null) ? next.SpRes - cur.SpRes : next.SpRes;
            this.AntiCritP = (cur != null) ? next.AntiCritP - cur.AntiCritP : next.AntiCritP;
            this.DmgMulti = (cur != null) ? next.DmgMulti - cur.DmgMulti : next.DmgMulti;
            this.DmgRes = (cur != null) ? next.DmgRes - cur.DmgRes : next.DmgRes;
        }
    }

    public class PartnerTransCostTemple
    {
        public int upgrade;
        public int cost;
    }
    public class ProficiencyDescribeTemplate
    {
        public ProficiencyType id;
        public string name;
        public string desc;
        public string icon;
		public int[] limit;
    }

    public class HeroStrategyInfo 
    {
        public int infoid;
        public int dmglevel;
        public int existlevel;
        public int controllevel;
        public int assistlevel;
        public int recommondedsuit;
        public string recommondedAttr;
        public string[] matchheroArray;
        public string matchDes;
    }

    public class PromotionTemplate 
    {
        public int id;
        public int qualityLevel;
        public int level;
        public int star;
        public int itemId;
        public int cost;
        public int additiveAttributeLevel;
        public int attributeLevelUpperLimit;
        public string name;
        public string bigIcon;
        public string smallIcon;
        public string taskIds;
    }

    public class PromotionTrainingTemplate 
    {
        public int id;
        public int upperLimit;
        public int count;
        public string name;
        public string negativeRegion;
        public string positiveRegion;
        public string regionPercent;
        public string negativeProbability;
        public string cost;
    }

    public class PromotionAttributeLevelTemplate 
    {
        public int id;
        public int unlockLevel;
        public float attrValue;
        public string name;
    }

    public class ArtifactEquipmentTemplate
    {
        public int id;
        public string name;
        public int heroId;
        public int enhancementLevel;
        public int qualityLevel;
        public string iconId;
        public string AttributeAdd;
        public string ItemCost;
        public int skillId;
        public string desc;
    }

    public class CharacterTemplateManager
    {

        private static CharacterTemplateManager s_instance;

        private ConditionCharacter conditionSet;
        private Dictionary<string, int> monsterIdToIndex = new Dictionary<string, int>();

        private Dictionary<int, HeroStatTemplate> mHeroStats;
        private Dictionary<int, HeroInfoTemplate> mHeroInfos;
        private List<HeroGuardTemplate> mGuardList = new List<HeroGuardTemplate>();
        private Dictionary<int, MonsterInfoTemplate> mMonsterInfos = new Dictionary<int, MonsterInfoTemplate>();

        private UpGradeInfoTemplate[] mUpGradeInfos = new UpGradeInfoTemplate[0];
        private SkillLevelTemplate[] mSkillLevels = new SkillLevelTemplate[0];
        private List<StarInfoTemplate> mStarInfoList = new List<StarInfoTemplate>();
        private List<HeroLevelInfoTemplate> mHeroLevelInfoList = new List<HeroLevelInfoTemplate>();
        private Dictionary<int, StarUpInfoTemplate> mStarUpInfoDic = new Dictionary<int, StarUpInfoTemplate>();

        private List<MannualBreakTemplate> mMannualBreakInfosList = new List<MannualBreakTemplate>();
        private List<MannualScoreTemplate> mMannualScoreInfosList = new List<MannualScoreTemplate>();
        private List<MannualRoleGradeTemplate> mMannualRoleGradeInfosList = new List<MannualRoleGradeTemplate>();
        private List<MannualUpgradeScoreTemplate> mMannualUpgradeScoreInfosList = new List<MannualUpgradeScoreTemplate>();
        private Dictionary<string, Dictionary<int, LevelUpInfoTemplate>> mLevelUpInfoDic = new Dictionary<string, Dictionary<int, LevelUpInfoTemplate>>();
        private List<PlayerLevelTemplate> mPlayerLevelList = new List<PlayerLevelTemplate>();
        private List<ProficiencyUpTemplate> mProficiencyUpList = new List<ProficiencyUpTemplate>();
        private List<ProficiencyDescribeTemplate> mProficiencyDescribeList = new List<ProficiencyDescribeTemplate>();
        private Dictionary<ProficiencyType, ProficiencyUpTemplate> mTheTopProficiencyUpDic = new Dictionary<ProficiencyType, ProficiencyUpTemplate>();
        private Dictionary<int, PartnerTransCostTemple> mSwitchCost = new Dictionary<int, PartnerTransCostTemple>();
        private Dictionary<int, HeroAwakeInfoTemplate> mAwakenInfos = new Dictionary<int, HeroAwakeInfoTemplate>();
        public List<int> mAwakenSkillList = new List<int>();
        public List<HeroStrategyInfo> mHeroStrategyInfos = new List<HeroStrategyInfo>();

        private Dictionary<int, PromotionTemplate> mPromotionDictById = new Dictionary<int, PromotionTemplate>();
        private Dictionary<string, PromotionTemplate> mPromotionDictByLevelStar = new Dictionary<string, PromotionTemplate>();
        private Dictionary<int, int> mPromotionStarCountDictByLevel = new Dictionary<int, int>();
        private Dictionary<int, PromotionTrainingTemplate> mPromotionTrainingDictById = new Dictionary<int, PromotionTrainingTemplate>();
        private Dictionary<int, PromotionAttributeLevelTemplate> mPromotionAttributeLevelDictById = new Dictionary<int, PromotionAttributeLevelTemplate>();
        private Dictionary<string, PromotionAttributeLevelTemplate> mPromotionAttributeLevelDictByName = new Dictionary<string, PromotionAttributeLevelTemplate>();
        private Dictionary<int, int> mPromotionLinkedDict = new Dictionary<int, int>();
        private List<ArtifactEquipmentTemplate> _artifactEquipmentTemplates = new List<ArtifactEquipmentTemplate>();
        public static CharacterTemplateManager Instance
        {
            get { return s_instance = s_instance ?? new CharacterTemplateManager(); }
        }

        private CharacterTemplateManager()
        {

        }

        public static void ClearUp()
        {
            if (s_instance != null)
            {
                s_instance.monsterIdToIndex.Clear();
                s_instance.mHeroStats.Clear();
                s_instance.mHeroInfos.Clear();
                s_instance.mMonsterInfos.Clear();
                s_instance.mUpGradeInfos = null;
                s_instance.mSkillLevels = null;
                s_instance.mStarInfoList.Clear();
                s_instance.mHeroLevelInfoList.Clear();
                s_instance.mStarUpInfoDic.Clear();
                s_instance.mMannualBreakInfosList.Clear();
                s_instance.mMannualScoreInfosList.Clear();
                s_instance.mMannualRoleGradeInfosList.Clear();
                s_instance.mMannualUpgradeScoreInfosList.Clear();
                s_instance.mLevelUpInfoDic.Clear();
                s_instance.mPlayerLevelList.Clear();
                s_instance.mProficiencyUpList.Clear();
                s_instance.mProficiencyDescribeList.Clear();
                s_instance.mSwitchCost.Clear();
                s_instance.mAwakenInfos.Clear();
                s_instance.mAwakenSkillList.Clear();
                s_instance.mGuardList.Clear();
                s_instance.mHeroStrategyInfos.Clear();
                s_instance.mPromotionDictById.Clear();
                s_instance.mPromotionTrainingDictById.Clear();
                s_instance.mPromotionAttributeLevelDictById.Clear();
                s_instance._artifactEquipmentTemplates.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Character hero)
        {
            if (hero == null)
            {
                EB.Debug.LogError("InitFromDataCache: hero is null");
                return false;
            }

            conditionSet = hero.GetArray(0);

            int len = conditionSet.MonsterLength;
            for (int i = 0; i < len; ++i)
            {
                Monster info = conditionSet.GetMonster(i);
                monsterIdToIndex.Add(info.Id, i);
            }

            if (!InitHeroStats(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init heroStats failed");
                return false;
            }

            if (!InitHeroInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init heroInfos failed");
                return false;
            }
            
            if (!InitGuards(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init heroInfos failed");
                return false;
            }

            //if (!InitMonsterInfos(conditionSet))
            //{
            //    EB.Debug.LogError("InitFromDataCache: init monsterInfos failed");
            //    return false;
            //}

            if (!InitStarInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init starInfos failed");
                return false;
            }

            if (!InitUpGradeInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init upGradeInfos failed");
                return false;
            }

            if (!InitSkillLevel(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init skillLevels failed");
                return false;
            }

            if (!InitHeroLevelInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init heroLevelInfos failed");
                return false;
            }

            if (!InitStarUpInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init starUpInfos failed");
                return false;
            }

            if (!InitMannualBreakInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init MannualBreakInfos failed");
                return false;
            }

            //if (!InitLevelUpInfos(conditionSet))
            //{
            //    EB.Debug.LogError("InitFromDataCache: init levelUpInfos failed");
            //    return false;
            //}

            if (!InitMannualScore(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitMannualScore failed");
                return false;
            }
            if (!InitMannualRoleGrade(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitMannualRoleGrade failed");
                return false;
            }
            if (!InitMannualUpgradeScore(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitMannualUpgradeScore failed");
                return false;
            }
            if (!InitPlayerLevelInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitPlayerLevelInfos failed");
                return false;
            }

            if (!InitProficiencyUpInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitProficiencyUpInfos failed");
                return false;
            }
            if (!InitProficiencyDescribeInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitProficiencyDescribeInfos failed");
                return false;
            }
            if (!InitSwitchCostInfos(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitSwitchCostInfos failed");
                return false;
            }
            if (!InitAwakeInfo(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitHeroAwakeInfos failed");
                return false;
            } 
            if (!InitPartnerstrategyInfo(conditionSet))
            {
                EB.Debug.LogError("InitFromDataCache: init InitPartnerstrategyInfo failed");
                return false;
            }

            if (!InitPromotion(conditionSet)) {
                EB.Debug.LogError("InitFromDataCache: init InitPromotion failed");
                return false;
            }
            if (!InitPromotionTraining(conditionSet)) {
                EB.Debug.LogError("InitFromDataCache: init InitPromotionTraining failed");
                return false;
            }
            if (!InitPromotionAttributeLevel(conditionSet)) {
                EB.Debug.LogError("InitFromDataCache: init InitPromotionAttributeLevel failed");
                return false;
            }

            if (!InitArtifactEquipments(conditionSet)) {
                EB.Debug.LogError("InitFromDataCache: init InitPromotionAttributeLevel failed");
                return false;
            }
            return true;
        }

        private bool InitHeroStats(GM.DataCache.ConditionCharacter heroStats)
        {
            if (heroStats == null)
            {
                EB.Debug.LogError("InitHeroStats: can not find heroStats data");
                return false;
            }

            int len = heroStats.HeroStatsLength;
            mHeroStats = new Dictionary<int, HeroStatTemplate>(len);

            for (int i = 0; i < len; ++i)
            {
                GM.DataCache.HeroStat heroStat = heroStats.GetHeroStats(i);
                mHeroStats.Add(int.Parse(heroStat.Id), ParseHeroStat(heroStat)); 
            }

            return true;
        }

        private HeroStatTemplate ParseHeroStat(GM.DataCache.HeroStat heroStats)
        {
            HeroStatTemplate tpl = new HeroStatTemplate
            {
                heroStats = heroStats
            };

            return tpl;
        }
        private bool InitHeroInfos(GM.DataCache.ConditionCharacter heroInfos)
        {
            if (heroInfos == null)
            {
                EB.Debug.LogError("InitHeroInfos: can not find heroInfos data");
                return false;
            }
            int len = heroInfos.HeroInfosLength;
            mHeroInfos = new Dictionary<int, HeroInfoTemplate>(len);

            for (int i = 0; i < len; ++i)
            {
                var info = heroInfos.GetHeroInfos(i);
                mHeroInfos.Add(int.Parse(info.Id), ParseHeroInfo(info));
            }

            return true;
        }
        
        private bool InitGuards(GM.DataCache.ConditionCharacter conditionCharacter)
        {
            if (conditionCharacter == null)
            {
                EB.Debug.LogError("InitHeroInfos: can not find conditionCharacter data");
                return false;
            }
            int len = conditionCharacter.HeroFetterLength;

            for (int i = 0; i < len; ++i)
            {
                var info = conditionCharacter.GetHeroFetter(i);
                mGuardList.Add(ParseGuard(info));
            }

            return true;
        }

        private HeroInfoTemplate ParseHeroInfo(GM.DataCache.HeroInfo obj)
        {
            HeroInfoTemplate tpl = new HeroInfoTemplate() {
                obj = obj,
                icon = obj.Icon,
                model_name = obj.ModelName,
                skin = obj.Skin,
            };
            return tpl;
        }
        
        private HeroGuardTemplate ParseGuard(HeroFetter obj)
        {
            // EB.Debug.LogError("index:{0},condition:{1},level:{2},HP:{3},ATK:{4},DEF:{5}",
            //     obj.Index,obj.Condition,obj.Level,obj.IncMaxHP,obj.IncATK,obj.IncDEF);
            
            HeroGuardTemplate tpl = new HeroGuardTemplate(obj);
            return tpl;
        }

        //private bool InitMonsterInfos(GM.DataCache.ConditionCharacter monsterInfos)
        //{
        //    if (monsterInfos == null)
        //    {
        //        EB.Debug.LogError("InitMonsterInfos: can not find monsterInfos data");
        //        return false;
        //    }

        //    int len = monsterInfos.MonsterLength;
        //    mMonsterInfos = new Dictionary<int, MonsterInfoTemplate>(len);

        //    for (int i = 0; i < len; ++i)
        //    {
        //        GM.DataCache.Monster info = monsterInfos.GetMonster(i);
        //        mMonsterInfos.Add(int.Parse(info.Id), ParseMonsterInfo(info));
        //    }

        //    return true;
        //}

        private MonsterInfoTemplate ParseMonsterInfo(GM.DataCache.Monster obj)
        {
            MonsterInfoTemplate tpl = new MonsterInfoTemplate()
            {
                obj = obj,
            };

            if (tpl.monster_type == 3 && tpl.hp_number == 0)
            {
                EB.Debug.LogError("boss hp_number is 0 tplId: {0}" , obj.Id);
                tpl.hp_number = 1;
            }
            else
            {
                tpl.hp_number = obj.HpNumber;
            }

            return tpl;
        }

        private bool InitStarInfos(GM.DataCache.ConditionCharacter starInfos)
        {
            if (starInfos == null)
            {
                EB.Debug.LogError("InitStarInfos: can not find starInfos data");
                return false;
            }

            mStarInfoList = new List<StarInfoTemplate>();
            for (int i = 0; i < starInfos.StarLength; i++)
            {
                StarInfoTemplate tpl = ParseStarInfo(starInfos.GetStar(i));
                mStarInfoList.Add(tpl);
            }

            return true;
        }

        private StarInfoTemplate ParseStarInfo(GM.DataCache.Star obj)
        {
            StarInfoTemplate tpl = new StarInfoTemplate();
            tpl.id = obj.Id;
            tpl.star_level = obj.StarLevel;
            tpl.hole_position = obj.HolePosition;
            tpl.cost_shard = obj.CostShard;
            tpl.attr = obj.Attr;
            tpl.param = obj.Param;
            tpl.level = obj.Level;
            return tpl;
        }

        private bool InitUpGradeInfos(GM.DataCache.ConditionCharacter upGradeInfos)
        {
            if (upGradeInfos == null)
            {
                EB.Debug.LogError("InitUpGradeInfos: can not find upGradeInfos data");
                return false;
            }

            mUpGradeInfos = new UpGradeInfoTemplate[upGradeInfos.UpgradeLength];
            for (int i = 0; i < upGradeInfos.UpgradeLength; ++i)
            {
                mUpGradeInfos[i] = ParseUpGradeInfo(upGradeInfos.GetUpgrade(i));

                LTPartnerConfig.MAX_GRADE_LEVEL = mUpGradeInfos[i].upGradeId > LTPartnerConfig.MAX_GRADE_LEVEL ? mUpGradeInfos[i].upGradeId : LTPartnerConfig.MAX_GRADE_LEVEL;

                if (LTPartnerConfig.UP_GRADE_ID_DIC.ContainsKey(mUpGradeInfos[i].quality))
                {
                    LTPartnerConfig.UP_GRADE_ID_DIC[mUpGradeInfos[i].quality] = LTPartnerConfig.UP_GRADE_ID_DIC[mUpGradeInfos[i].quality] <= mUpGradeInfos[i].upGradeId ? LTPartnerConfig.UP_GRADE_ID_DIC[mUpGradeInfos[i].quality] : mUpGradeInfos[i].upGradeId;
                }
                else
                {
                    LTPartnerConfig.UP_GRADE_ID_DIC.Add(mUpGradeInfos[i].quality, mUpGradeInfos[i].upGradeId);
                }
            }

            return true;
        }

        private UpGradeInfoTemplate ParseUpGradeInfo(GM.DataCache.Upgrade obj)
        {
            UpGradeInfoTemplate tpl = new UpGradeInfoTemplate();

            tpl.quality = obj.Quality;
            tpl.upGradeId = obj.UpgradeId;
            tpl.name = EB.Localizer.GetString(string.Format("ID_herostats_upgrade_{0}_name", obj.UpgradeId));
            tpl.inc_maxhp = obj.IncMaxHP;
            tpl.inc_atk = obj.IncATK;
            tpl.inc_def = obj.IncDEF;
            tpl.inc_speed = obj.IncSpeed;
            tpl.level = obj.Level;
            tpl.level_limit = obj.LevelLimit;
            tpl.needHcNum = obj.Hc;
            tpl.needGoldNum = obj.Gold;
            tpl.char_type = (eRoleAttr)obj.CharType;
            tpl.materialDic = new Dictionary<string, int>();
            if (obj.Material1 != "0" && !string.IsNullOrEmpty(obj.Material1))
            {
                tpl.materialDic.Add(obj.Material1, obj.Quantity1);
            }

            if (obj.Material2 != "0" && !string.IsNullOrEmpty(obj.Material2))
            {
                tpl.materialDic.Add(obj.Material2, obj.Quantity2);
            }

            if (obj.Material3 != "0" && !string.IsNullOrEmpty(obj.Material3))
            {
                tpl.materialDic.Add(obj.Material3, obj.Quantity3);
            }

            if (obj.Material4 != "0" && !string.IsNullOrEmpty(obj.Material4))
            {
                tpl.materialDic.Add(obj.Material4, obj.Quantity4);
            }
            return tpl;
        }
        private bool InitAwakeInfo(GM.DataCache.ConditionCharacter awakeInfo)
        {
            if (awakeInfo == null)
            {
                EB.Debug.LogError("InitAwakeInfo: can not find awakeInfo data");
                return false;
            }
            mAwakenInfos = new Dictionary<int, HeroAwakeInfoTemplate>();
            for (int i = 0; i < awakeInfo.HeroAwakenLength; i++)
            {
                HeroAwakeInfoTemplate temp = ParseAwakenInfo(awakeInfo.GetHeroAwaken(i));
                if (temp.laterSkill != 0)
                {
                    mAwakenSkillList.Add(temp.laterSkill);
                }
                mAwakenInfos.Add(temp.id, temp);
            }
            return true;
        }
        private HeroAwakeInfoTemplate ParseAwakenInfo(GM.DataCache.HeroAwaken obj)
        {
            HeroAwakeInfoTemplate heroawaken = new HeroAwakeInfoTemplate();
            heroawaken.id = int.Parse(obj.Id);
            heroawaken.limitLevel = obj.Level;
            heroawaken.limitUpgrade = obj.UpgradeId;
            heroawaken.limitStar = obj.Star;
            heroawaken.awakeHeadIcon = obj.AwakenIcon;
            heroawaken.awakeSkin = obj.AwakenSkin;
            heroawaken.awakeType = obj.AwakeType;
            heroawaken.beforeSkill = obj.AwakenBeforeSkill;
            heroawaken.laterSkill = obj.AwakenLaterSkill;
            heroawaken.awakeMaterDic = new Dictionary<string, int>();
            if (obj.Material1 != null && obj.Quantity1 != 0)
            {
                heroawaken.awakeMaterDic.Add(obj.Material1, obj.Quantity1);
            }
            if (obj.Material2 != null && obj.Quantity2 != 0)
            {
                heroawaken.awakeMaterDic.Add(obj.Material2, obj.Quantity2);
            }
            if (obj.Material3 != null && obj.Quantity3 != 0)
            {
                heroawaken.awakeMaterDic.Add(obj.Material3, obj.Quantity3);
            }
            if (obj.Material4 != null && obj.Quantity4 != 0)
            {
                heroawaken.awakeMaterDic.Add(obj.Material4, obj.Quantity4);
            }
            heroawaken.inc_MaxHP = obj.IncMaxHP;
            heroawaken.inc_ATK = obj.IncATK;
            heroawaken.inc_DEF = obj.IncDEF;
            heroawaken.inc_speed = obj.IncSpeed;
            if (string.IsNullOrEmpty(obj.AttributeAdd))
            {
                heroawaken.DmgMulti = 0;
                heroawaken.DmgRes = 0;
            }
            else
            {

                string[] attr = obj.AttributeAdd.Split(';');
                for (int i = 0; i < attr.Length; i++)
                {
                    if (attr[i].Contains("DmgMulti"))
                    {
                        heroawaken.DmgMulti = float.Parse(attr[i].Replace("DmgMulti,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("DmgRes"))
                    {
                        heroawaken.DmgRes = float.Parse(attr[i].Replace("DmgRes,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("speed"))
                    {
                        heroawaken.speedAdd = int.Parse(attr[i].Replace("speed,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("SpExtra"))
                    {
                        heroawaken.SpExtra = float.Parse(attr[i].Replace("SpExtra,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("SpRes"))
                    {
                        heroawaken.SpRes = float.Parse(attr[i].Replace("SpRes,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("AntiCritP"))
                    {
                        heroawaken.AntiCritP = float.Parse(attr[i].Replace("AntiCritP,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("CritP"))
                    {
                        heroawaken.CritP = float.Parse(attr[i].Replace("CritP,", string.Empty));
                        continue;
                    }
                    if (attr[i].Contains("CritV"))
                    {
                        heroawaken.CritV = float.Parse(attr[i].Replace("CritV,", string.Empty));
                        continue;
                    }

                }
            }



            return heroawaken;
        }

        private  bool InitPartnerstrategyInfo(GM.DataCache.ConditionCharacter obj)
        {
            if (obj == null)
            {
                EB.Debug.LogError("InitAwakeInfo: can not find herostrategy data");
                return false;
            }
            if (mHeroStrategyInfos == null)
            {
                mHeroStrategyInfos = new List<HeroStrategyInfo>();
            }
            else if(mHeroStrategyInfos.Count >0)
            {
                mHeroStrategyInfos.Clear();
            }
            for (int i = 0; i < obj.HeroStrategyLength; i++)
            {
                HeroStrategyInfo temp = ParseHeroStrategyInfo(obj.GetHeroStrategy(i));
                if (temp != null)
                {
                    mHeroStrategyInfos.Add(temp);
                }
            }
            return true;
        }

        private HeroStrategyInfo ParseHeroStrategyInfo(GM.DataCache.HeroStrategy heroStrategy)
        {
            if(heroStrategy == null)
            {
                EB.Debug.LogError("ParseHeroStrategyInfo heroStrategy is null");
                return null;
            }
            HeroStrategyInfo tempinfo = new HeroStrategyInfo();
            tempinfo.infoid = heroStrategy.InfoId;
            tempinfo.dmglevel = heroStrategy.Damage;
            tempinfo.existlevel = heroStrategy.Existence;
            tempinfo.controllevel = heroStrategy.Control;
            tempinfo.assistlevel = heroStrategy.Assist;
            tempinfo.recommondedsuit = heroStrategy.RecommendedSuit;
            tempinfo.recommondedAttr = heroStrategy.RecommendedAttr;
            if (!string.IsNullOrEmpty(heroStrategy.RecommendedMatch))
            {
                tempinfo.matchheroArray = heroStrategy.RecommendedMatch.Split(';');
            }
            tempinfo.matchDes = EB.Localizer.GetString(string.Format("ID_herostats_hero_strategy_{0}_match_desc",heroStrategy.InfoId));
            //tempinfo.matchDes = heroStrategy.MatchDesc;
            return tempinfo;
        }

        public HeroStrategyInfo GetHeroStrategyInfoByInfoId(int infoid)
        {
            if (mHeroStrategyInfos == null)
            {
                return null;
            }
            HeroStrategyInfo tempdata;
            for (int i = 0; i < mHeroStrategyInfos.Count; i++)
            {
                tempdata = mHeroStrategyInfos[i];
                if (tempdata.infoid == infoid)
                {
                    return tempdata;
                }
            }
            EB.Debug.LogError("Can not find this partner HeroStrageInfo,  id = {0}", infoid);
            return null;
        }

        private bool InitSkillLevel(GM.DataCache.ConditionCharacter skillLevels)
        {
            if (skillLevels == null)
            {
                EB.Debug.LogError("InitSkillLevel: can not find skillLevels data");
                return false;
            }

            mSkillLevels = new SkillLevelTemplate[skillLevels.SkillLevelLength];
            for (int i = 0; i < skillLevels.SkillLevelLength; ++i)
            {
                mSkillLevels[i] = ParseSkillLevel(skillLevels.GetSkillLevel(i));

                LTPartnerConfig.MAX_SKILL_LEVEL = LTPartnerConfig.MAX_SKILL_LEVEL > mSkillLevels[i].level ? LTPartnerConfig.MAX_SKILL_LEVEL : mSkillLevels[i].level;

                if (LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.ContainsKey(mSkillLevels[i].level))
                {
                    LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC[mSkillLevels[i].level] = mSkillLevels[i].exp;
                }
                else
                {
                    LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.Add(mSkillLevels[i].level, mSkillLevels[i].exp);
                }

                // if (LTPartnerConfig.SKILL_BREAK_LIMIT_LEVEL_DIC.ContainsKey(mSkillLevels[i].starLimit))
                // {
                //     LTPartnerConfig.SKILL_BREAK_LIMIT_LEVEL_DIC[mSkillLevels[i].starLimit] = LTPartnerConfig.SKILL_BREAK_LIMIT_LEVEL_DIC[mSkillLevels[i].starLimit] >= mSkillLevels[i].level ? LTPartnerConfig.SKILL_BREAK_LIMIT_LEVEL_DIC[mSkillLevels[i].starLimit] : mSkillLevels[i].level;
                // }
                // else
                // {
                //     LTPartnerConfig.SKILL_BREAK_LIMIT_LEVEL_DIC.Add(mSkillLevels[i].starLimit, mSkillLevels[i].level);
                // }
            }

            if (!LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.ContainsKey(skillLevels.SkillLevelLength + 1))
            {
                LTPartnerConfig.SKILL_BREAK_LEVEL_EXP_DIC.Add(skillLevels.SkillLevelLength + 1, 50000);
            }

            return true;
        }

        private SkillLevelTemplate ParseSkillLevel(GM.DataCache.SkillLevel obj)
        {
            SkillLevelTemplate tpl = new SkillLevelTemplate();
            tpl.level = obj.Level;
            tpl.starLimit = obj.StarLimit;
            tpl.exp = obj.Exp;

            return tpl;
        }

        private bool InitHeroLevelInfos(GM.DataCache.ConditionCharacter heroLevelInfos)
        {
            if (heroLevelInfos == null)
            {
                EB.Debug.LogError("InitHeroLevelInfos: can not find heroLevelInfos data");
                return false;
            }

            mHeroLevelInfoList = new List<HeroLevelInfoTemplate>();
            for (int i = 0; i < heroLevelInfos.LevelLength; i++)
            {
                HeroLevelInfoTemplate tpl = ParseHeroLevelInfo(heroLevelInfos.GetLevel(i));
                LTPartnerConfig.MAX_LEVEL = tpl.level > LTPartnerConfig.MAX_LEVEL ? tpl.level : LTPartnerConfig.MAX_LEVEL;
                mHeroLevelInfoList.Add(tpl);
            }

            return true;
        }

        private HeroLevelInfoTemplate ParseHeroLevelInfo(GM.DataCache.HeroLevel obj)
        {
            HeroLevelInfoTemplate tpl = new HeroLevelInfoTemplate();
            tpl.level = obj.Level;
            tpl.buddy_exp = obj.BuddyExp;

            return tpl;
        }

        private bool InitStarUpInfos(GM.DataCache.ConditionCharacter tbl)
        {
            if (tbl == null)
            {
                EB.Debug.LogError("InitStarUpInfos:tbl is null");
                return false;
            }

            mStarUpInfoDic = new Dictionary<int, StarUpInfoTemplate>();
            for (int i = 0; i < tbl.StarUpLength; i++)
            {
                StarUpInfoTemplate tpl = ParseStarUpInfo(tbl.GetStarUp(i));
                if (mStarUpInfoDic.ContainsKey(tpl.StarId))
                {
                    EB.Debug.LogError("InitStarUpInfos:{0} exits", tpl.StarId);
                    mStarUpInfoDic.Remove(tpl.StarId);
                }

                mStarUpInfoDic.Add(tpl.StarId, tpl);
            }

            return true;
        }

        private StarUpInfoTemplate ParseStarUpInfo(GM.DataCache.StarUp obj)
        {
            StarUpInfoTemplate tpl = new StarUpInfoTemplate();
            tpl.StarId = obj.StarId;
            tpl.IncMaxHp = obj.IncMaxHP;
            tpl.IncATK = obj.IncATK;
            tpl.IncDEF = obj.IncDEF;
            return tpl;
        }

        private bool InitMannualBreakInfos(GM.DataCache.ConditionCharacter mb)
        {
            if (mb == null)
            {
                EB.Debug.LogError("InitMannualBreakInfos:mb is null");
                return false;
            }

            mMannualBreakInfosList = new List<MannualBreakTemplate>();
            for (int i = 0; i < mb.MannualBreakLength; i++)
            {
                MannualBreakTemplate tp1 = ParseManualBreakInfo(mb.GetMannualBreak(i));
                mMannualBreakInfosList.Add(tp1);
            }

            return true;
        }

        private Dictionary<eRoleAttr, int> _handbookMaxLevels = new Dictionary<eRoleAttr, int>();
        public int GetBreakTemplateMaxLevel(eRoleAttr handBookId)
        {
            if (_handbookMaxLevels.ContainsKey(handBookId))
            {
                return _handbookMaxLevels[handBookId];
            }
            int maxLevel = 0;
            for (int i = 0; i < mMannualBreakInfosList.Count; i++)
            {
                if (mMannualBreakInfosList[i].type == handBookId)
                {
                    maxLevel++;
                }
            }
            _handbookMaxLevels[handBookId] = maxLevel - 1;
            return maxLevel - 1;
        }

        public MannualBreakTemplate GetBreakTemplateByLevel(eRoleAttr handBookId, int level)
        {
            for (int i = 0; i < mMannualBreakInfosList.Count; i++)
            {
                if (mMannualBreakInfosList[i].type == handBookId && mMannualBreakInfosList[i].level == level)
                {
                    return mMannualBreakInfosList[i];
                }
            }
            return null;
        }

        private MannualBreakTemplate ParseManualBreakInfo(GM.DataCache.MannualBreak obj)
        {
            MannualBreakTemplate tp1 = new MannualBreakTemplate();
            tp1.level = obj.Level;
            tp1.material_1 = obj.Material1;
            tp1.quantity_1 = obj.Quantity1;
            tp1.type = (eRoleAttr)obj.Type;
            tp1.score_promotion = obj.ScorePromotion;

            tp1.IncATK = obj.IncATK;
            tp1.IncDEF = obj.IncDEF;
            tp1.IncMaxHp = obj.IncMaxHP;
            return tp1;
        }

        private bool InitMannualScore(GM.DataCache.ConditionCharacter ms)
        {
            if (ms == null)
            {
                EB.Debug.LogError("InitMannualScore: can not find MannualScore data");
                return false;
            }
            mMannualScoreInfosList = new List<MannualScoreTemplate>();
            mMannualScoreInfosList = new List<MannualScoreTemplate>();
            int totle = 0;
            for (int i = 0; i < ms.MannualScoreLength; i++)
            {
                MannualScoreTemplate tp1 = ParseManualScoreInfo(ms.GetMannualScore(i));
                totle += tp1.score;
                tp1.totleScore = totle;
                mMannualScoreInfosList.Add(tp1);
            }

            return true;
        }

        private MannualScoreTemplate ParseManualScoreInfo(GM.DataCache.MannualScore obj)
        {
            MannualScoreTemplate tp1 = new MannualScoreTemplate();
            tp1.id = obj.Id;
            tp1.attribute_addition = obj.AttributeAddition;
            tp1.score = obj.Score;

            tp1.levelLimit = obj.Evaluate;
            tp1.ATK = obj.ATK;
            tp1.DEF = obj.DEF;
            tp1.maxHP = obj.MaxHP;
            return tp1;
        }

        private MannualRoleGradeTemplate ParseMannualRoleGrade(GM.DataCache.MannualRoleGrade obj)
        {
            MannualRoleGradeTemplate tp1 = new MannualRoleGradeTemplate();
            tp1.id = obj.Id;
            tp1.star_addition = obj.StarAddition;

            tp1.ScoreList = new List<int>();
            if (!string.IsNullOrEmpty(obj.ScoreAddition))
            {
                var strs = obj.ScoreAddition.Split(',');
                for (int i = 0; i < strs.Length; ++i)
                {
                    tp1.ScoreList.Add(int.Parse(strs[i]));
                }
            }
            return tp1;
        }

        public int GetMannualScoreMaxByScore(int score)
        {
            for (int i = 0; i < mMannualScoreInfosList.Count; i++)
            {
                if (score > mMannualScoreInfosList[i].score)
                {
                    continue;
                }

                return mMannualScoreInfosList[i].score;
            }
            EB.Debug.LogWarning("can not find MannualScoreTemplete by score : {0},MayBe You Have the max Score" , score);
            return -1;
        }

        public MannualScoreTemplate GetMannualScoreTemplateByScore(int Score)
        {
            for (int i = 0; i < mMannualScoreInfosList.Count; i++)
            {
                if (Score <= 0)
                {
                    return mMannualScoreInfosList[0];
                }
                if (Score > mMannualScoreInfosList[i].score)
                {
                    continue;
                }
                return mMannualScoreInfosList[i - 1];
            }

            if (Score > mMannualScoreInfosList[mMannualScoreInfosList.Count - 1].score)
            {
                return mMannualScoreInfosList[mMannualScoreInfosList.Count - 1];
            }
            EB.Debug.LogError("can not find MannualScoreTemplete by score : {0}" , Score);
            return null;
        }

        public int GetMannualUnlockLevel()
        {
            for (int i = 0; i < mMannualScoreInfosList.Count; ++i)
            {
                if (mMannualScoreInfosList[i].levelLimit > 0)
                {
                    return mMannualScoreInfosList[i].id;
                }
            }
            return 0;
        }

        public MannualScoreTemplate GetMannualScoreTemplateById(int id)
        {
            for (int i = 0; i < mMannualScoreInfosList.Count; ++i)
            {
                if (mMannualScoreInfosList[i].id == id)
                {
                    return mMannualScoreInfosList[i];
                }
            }
            return null;
        }

        private bool InitMannualRoleGrade(GM.DataCache.ConditionCharacter mr)
        {
            if (mr == null)
            {
                EB.Debug.LogError("InitMannualRoleGrade: can not find MannualRoleGrade data");
                return false;
            }
            mMannualRoleGradeInfosList = new List<MannualRoleGradeTemplate>();
            for (int i = 0; i < mr.MannualRoleGradeLength; i++)
            {
                MannualRoleGradeTemplate tp1 = ParseMannualRoleGrade(mr.GetMannualRoleGrade(i));
                mMannualRoleGradeInfosList.Add(tp1);
            }
            return true;
        }

        public MannualRoleGradeTemplate GetMannualRoleGradeTempleteByRoleGrade(int RoleGrade)
        {
            var count = mMannualRoleGradeInfosList.Count;

            for (int i = 0; i < count; i++)
            {
                if (mMannualRoleGradeInfosList[i].id == RoleGrade)
                {
                    return mMannualRoleGradeInfosList[i];
                }
            }
            EB.Debug.LogError("can not find this role grade : {0}", RoleGrade);
            return null;
        }

        private MannualUpgradeScoreTemplate ParseMannualUpgradeScore(GM.DataCache.MannualUpgradeScore obj)
        {
            MannualUpgradeScoreTemplate tp1 = new MannualUpgradeScoreTemplate();
            tp1.upgrade = obj.Upgrade;
            tp1.score = obj.Score;
            return tp1;
        }

        private bool InitMannualUpgradeScore(GM.DataCache.ConditionCharacter mu)
        {
            if (mu == null)
            {
                EB.Debug.LogError("InitMannualRoleGrade: can not find MannualRoleGrade data");
                return false;
            }
            mMannualUpgradeScoreInfosList = new List<MannualUpgradeScoreTemplate>();
            for (int i = 0; i < mu.MannualUpgradeScoreLength; i++)
            {
                MannualUpgradeScoreTemplate tp1 = ParseMannualUpgradeScore(mu.GetMannualUpgradeScore(i));
                mMannualUpgradeScoreInfosList.Add(tp1);
            }
            return true;
        }

        private bool InitPlayerLevelInfos(GM.DataCache.ConditionCharacter mu)
        {
            if (mu == null)
            {
                EB.Debug.LogError("InitPlayerLevelInfos: can not find PlayerLevelInfos data");
                return false;
            }
            mPlayerLevelList = new List<PlayerLevelTemplate>();
            for (int i = 0; i < mu.PlayerXpProgressionLength; i++)
            {
                PlayerLevelTemplate tp1 = ParsePlayerLevel(mu.GetPlayerXpProgression(i));
                mPlayerLevelList.Add(tp1);
            }
            return true;
        }

        private PlayerLevelTemplate ParsePlayerLevel(GM.DataCache.PlayerXPProgression obj)
        {
            PlayerLevelTemplate tp = new PlayerLevelTemplate();
            tp.level = obj.Level;
            tp.expRequirement = obj.ExpRequirement;
            return tp;
        }

        private bool InitProficiencyUpInfos(GM.DataCache.ConditionCharacter mu)
        {
            if (mu == null)
            {
                EB.Debug.LogError("InitProficiencyUpInfos: can not find ProficiencyUpInfos data");
                return false;
            }
            mProficiencyUpList = new List<ProficiencyUpTemplate>();
            for (int i = 0; i < mu.ProficiencyUpLength; i++)
            {
                ProficiencyUpTemplate tp1 = ParseProficiencyUp(mu.GetProficiencyUp(i));
                mProficiencyUpList.Add(tp1);
                if (mTheTopProficiencyUpDic.ContainsKey(tp1.type))
                {
                    if (tp1.level > mTheTopProficiencyUpDic[tp1.type].level)
                    {
                        mTheTopProficiencyUpDic[tp1.type] = tp1;
                    }
                }
                else
                {
                    mTheTopProficiencyUpDic.Add(tp1.type, tp1);
                }
            }
            return true;
        }

        private ProficiencyUpTemplate ParseProficiencyUp(GM.DataCache.ProficiencyUp obj)
        {
            ProficiencyUpTemplate tp = new ProficiencyUpTemplate();
            tp.id = obj.Id;
            tp.form = obj.Form;
            tp.type = (ProficiencyType)obj.Type;
            tp.level = obj.Level;

            tp.chipCost = new int[obj.ChipCostLength];
			for (int i = 0; i < obj.ChipCostLength; ++i)
			{
				tp.chipCost[i] = obj.GetChipCost(i);
			}
			tp.goldCost = new int[obj.GoldCostLength];
			for (int i = 0; i < obj.ChipCostLength; ++i)
			{
				tp.goldCost[i] = obj.GetGoldCost(i);
			}
			tp.potenCost = obj.PotenCost;

            tp.ATK = obj.ATK;
            tp.maxHP = obj.MaxHP;
            tp.DEF = obj.DEF;
            tp.speed = obj.Speed;
            tp.CritP = obj.CritP;
            tp.CritV = obj.CritV;
            tp.AntiCritP = obj.AntiCritP;
            tp.SpExtra = obj.SpExtra;
            tp.SpRes = obj.SpRes;
            tp.DmgMulti = obj.DmgMulti;
            tp.DmgRes = obj.DmgRes;
            return tp;
        }

        private bool InitSwitchCostInfos(GM.DataCache.ConditionCharacter sc)
        {
            if (sc == null)
            {
                return false;
            }
            mSwitchCost = new Dictionary<int, PartnerTransCostTemple>();
           EB.Debug.Log(sc.SwitchCostLength);
            for (int i = 0; i < sc.SwitchCostLength; i++)
            {
                PartnerTransCostTemple ptc = ParsePartnerTransCostTemple(sc.GetSwitchCost(i));
                mSwitchCost.Add(ptc.upgrade, ptc);
            }
            return true;
        }

        public int GetSwitchCostByUpGrade(int upgrade)
        {

            return mSwitchCost[upgrade].cost;
        }
        private PartnerTransCostTemple ParsePartnerTransCostTemple(GM.DataCache.SwitchCost obj)
        {
            PartnerTransCostTemple ptc = new PartnerTransCostTemple();
            ptc.upgrade = obj.Upgrade;
            ptc.cost = obj.Cost;
            return ptc;
        }
        private bool InitProficiencyDescribeInfos(GM.DataCache.ConditionCharacter mu)
        {
            if (mu == null)
            {
                EB.Debug.LogError("InitProficiencyDescribeInfos: can not find ProficiencyDescribeInfos data");
                return false;
            }
            mProficiencyDescribeList = new List<ProficiencyDescribeTemplate>();
            for (int i = 0; i < mu.ProficiencyDescribeLength; i++)
            {
                ProficiencyDescribeTemplate tp1 = ParseProficiencyDescribe(mu.GetProficiencyDescribe(i));
                mProficiencyDescribeList.Add(tp1);
            }
            return true;
        }

        private ProficiencyDescribeTemplate ParseProficiencyDescribe(GM.DataCache.ProficiencyDescribe obj)
        {
            ProficiencyDescribeTemplate tp = new ProficiencyDescribeTemplate();
            tp.id = (ProficiencyType)obj.Id;
            tp.name = EB.Localizer.GetTableString(string.Format("ID_herostats_proficiency_describe_{0}_name", obj.Id), obj.Name);// obj.Name;
            tp.desc = EB.Localizer.GetTableString(string.Format("ID_herostats_proficiency_describe_{0}_describe", obj.Id), obj.Describe); //obj.Describe;
            tp.icon = obj.Icon;
			tp.limit = new int[obj.LimitLength];
			for(int i=0;i<obj.LimitLength;i++)
				tp.limit[i] = obj.GetLimit(i);		
            return tp;
        }

        public ProficiencyUpTemplate GetTheTopProficiencyType(ProficiencyType type)
        {
            if (mTheTopProficiencyUpDic.ContainsKey(type))
            {
                return mTheTopProficiencyUpDic[type];
            }
            return null;
        }

        public ProficiencyUpTemplate GetProficiencyUpByTypeAndLevel(ProficiencyType type, int level)
        {
            ProficiencyUpTemplate temp = null;
            for (int i = 0; i < mProficiencyUpList.Count; i++)
            {
                if (mProficiencyUpList[i].type == type && mProficiencyUpList[i].level == level)
                    temp = mProficiencyUpList[i];
            }
            return temp;
        }

        public ProficiencyDescribeTemplate GetProficiencyDescByType(ProficiencyType type)
        {
            ProficiencyDescribeTemplate temp = null;
            for (int i = 0; i < mProficiencyDescribeList.Count; i++)
            {
                if (mProficiencyDescribeList[i].id == type)
                    temp = mProficiencyDescribeList[i];
            }
            return temp;
        }

        public List<ProficiencyDescribeTemplate> GetAllProficiencyDesc()
        {
            return mProficiencyDescribeList;
        }

        public string GetProficiencyUpName(string str)
        {
            switch (str)
            {
                case "ATK": return EB.Localizer.GetString("ID_ATTR_ATKrate");
                case "MaxHP": return EB.Localizer.GetString("ID_ATTR_MaxHPrate");
                case "DEF": return EB.Localizer.GetString("ID_ATTR_DEFrate");
                case "CritP": return EB.Localizer.GetString("ID_ATTR_CritP");
                case "CritV": return EB.Localizer.GetString("ID_ATTR_CritV");
                case "SpExtra": return EB.Localizer.GetString("ID_ATTR_SpExtra");
                case "SpRes": return EB.Localizer.GetString("ID_ATTR_SpRes");
                case "AntiCritP": return EB.Localizer.GetString("ID_ATTR_CRIresist");
                case "DmgMulti": return EB.Localizer.GetString("ID_ATTR_DMGincrease");
                case "DmgRes": return EB.Localizer.GetString("ID_ATTR_DMGreduction");
                case "speed": return EB.Localizer.GetString("ID_ATTR_Speed");
                default: return EB.Localizer.GetString("ID_ATTR_Unknown");
            }
        }

        public int GetCharacterUpgradeScore(int upgrade)
        {
            if (mMannualUpgradeScoreInfosList.Count > upgrade)
            {
                return mMannualUpgradeScoreInfosList[upgrade].score;
            }
            else
            {
                return 0;
            }
        }

        public int GetCharacterScoreByLevel(int level)
        {
            var count = mMannualUpgradeScoreInfosList.Count;

            for (int i = 0; i < count; i++)
            {
                var item = mMannualUpgradeScoreInfosList[i];

                if (level == int.Parse(item.upgrade))
                {
                    return item.score;
                }
            }
            EB.Debug.LogError("can not find MannuaCharacterScore Data by Level : {0}", level);
            return 0;
        }

        //private bool InitLevelUpInfos(GM.DataCache.ConditionCharacter tbl)
        //{
        //    if (tbl == null)
        //    {
        //        EB.Debug.LogError("InitLevelUpInfos:tbl is null");
        //        return false;
        //    }

        //    mLevelUpInfoDic.Clear();
        //    for (int i = 0; i < tbl.LevelUpLength; i++)
        //    {
        //        LevelUpInfoTemplate tpl = new LevelUpInfoTemplate()
        //        {
        //            obj = tbl.GetLevelUp(i)
        //        };

        //        if (tpl != null)
        //        {
        //            Dictionary<int, LevelUpInfoTemplate> info;
        //            if (mLevelUpInfoDic.TryGetValue(tpl.id, out info) == false)
        //            {
        //                info = new Dictionary<int, LevelUpInfoTemplate>
        //                {
        //                    { tpl.level, tpl }
        //                };

        //                mLevelUpInfoDic.Add(tpl.id, info);
        //            }
        //            else
        //            {
        //                LevelUpInfoTemplate levelInfo;
        //                if (info.TryGetValue(tpl.level, out levelInfo) == false)
        //                {
        //                    info.Add(tpl.level, tpl);
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}

        public HeroStatTemplate GetHeroStat(string id)
        {
            int iid;
            if (int.TryParse(id, out iid))
            {
                return GetHeroStat(iid);
            }
            else
            {
               EB.Debug.LogError("GetHeroStat Fail For id={0}" , id);
                return null;
            }
        }

        public HeroStatTemplate[] GetHeroStats()
        {
            return mHeroStats.Values.ToArray();
        }

        public HeroStatTemplate GetHeroStat(int tplId)
        {
            HeroStatTemplate result;
            if (mHeroStats.TryGetValue(tplId, out result) == false)
            {
                //EB.Debug.LogWarning("GetHeroStat: heroStat not found, id = {0}", tplId);
                return null;
            }

            return result;
        }

        public string GetCharacterName(int tplId)
        {
            HeroStatTemplate heroStat = GetHeroStat(tplId);
            MonsterInfoTemplate monsterInfo = GetMonsterInfo(tplId);

            return (heroStat != null ? heroStat.name : (monsterInfo != null ? monsterInfo.name : "UnknownCharName"));
        }

        /// <summary>
        /// 根据伙伴infoid获取伙伴stat数据
        /// </summary>
        /// <param name="infoId"></param>
        /// <returns></returns>
        public HeroStatTemplate GetHeroStatByInfoId(int infoId)
        {
            Dictionary<int, HeroStatTemplate>.Enumerator enumerator = mHeroStats.GetEnumerator();

            while(enumerator.MoveNext())
            {
                HeroStatTemplate result = enumerator.Current.Value;
                if (result.character_id == infoId)
                {
                    return result;
                }
            }
            return null;
        }

        public HeroInfoTemplate GetHeroInfoByStatId(int statId)
        {
            HeroStatTemplate temp = GetHeroStat(statId);
            if (temp != null)
            {
                return GetHeroInfo(temp.character_id);
            }

            return null;
        }

        public string GetHeroName(int characterId)
        {
            string id = string.Format("ID_herostats_hero_infos_{0}_name", characterId);
            return EB.Localizer.GetString(id);
        }

        public string TemplateidToCharacterid(string id)
        {
            HeroStatTemplate hero = GetHeroStat(id);
            if (hero != null) return hero.character_id.ToString();
            return null;
        }

        public string TemplateidToCharacteridEX(string id)
        {
            HeroStatTemplate hero = GetHeroStat(id);
            if (hero != null)
            {
                return hero.character_id.ToString();
            }
            else
            {
                MonsterInfoTemplate monsterInfo = GetMonsterInfo(int.Parse(id));
                if (monsterInfo != null)
                    return monsterInfo.character_id;
                else
                    return null;
            }
        }

        public bool HasHeroInfo(int id)
        {
            return mHeroInfos.ContainsKey(id);
        }
        public string GetIcon(string characterid)
        {
            HeroInfoTemplate data = GetHeroInfo(characterid);
            return data.icon;
        }
        public int GetRace(string characterid)
        {
            HeroInfoTemplate data = GetHeroInfo(characterid);
            return data.race;
        }
        public string GetModelName(string characterid, int skin)
        {
            HeroInfoTemplate data = GetHeroInfo(int.Parse(characterid), skin);
            return data.model_name;
        }
        public HeroInfoTemplate GetHeroInfo(string id)//不带皮肤string类型接口
        {
            if (string.IsNullOrEmpty(id)) return null;
            return GetHeroInfo(int.Parse(id), 0);
        }

        public HeroInfoTemplate GetHeroInfo(string id, int skinIndex)//带皮肤string类型接口
        {
            if (string.IsNullOrEmpty(id)) return null;
            return GetHeroInfo(int.Parse(id), skinIndex);
        }

        public Hashtable GetHeroInfoEx(int id, int skinIndex)//带皮肤string类型接口
        {
            HeroInfoTemplate data = GetHeroInfo(id, skinIndex);
            return new Hashtable() { { "model_name" , data.model_name }, { "char_type", (int)data.char_type }, { "icon", data.icon } };
        }

        public HeroInfoTemplate GetHeroInfo(int id)//不带带皮肤int类型接口
        {
            return GetHeroInfo(id, 0);
        }

        public HeroInfoTemplate GetHeroInfo(int id, int skinIndex)//带皮肤int类型接口
        {
            HeroInfoTemplate info;
            if (mHeroInfos.TryGetValue(id, out info))
            {
                HeroAwakeInfoTemplate temp = GetHeroAwakeInfoByInfoID(id);
                if (temp != null && !string.IsNullOrEmpty(temp.awakeSkin) && skinIndex > 0)
                {
                    HeroInfoTemplate tpl = new HeroInfoTemplate()
                    {
                        obj = info.obj,
                    };

                    //觉醒后需要改变的属性
                    //头像、半身像和皮肤不再绑定显示
                    if (string.IsNullOrEmpty(temp.awakeHeadIcon))
                    {
                        tpl.icon = info.icon;
                        tpl.skin = info.skin;
                    }
                    else
                    {
                        tpl.icon = string.Format("{0}_{1}", info.icon, skinIndex);
                        tpl.skin = string.Format("{0}_{1}", info.skin, skinIndex);
                    }
                    tpl.model_name = info.model_name.Replace("-", string.Format("_{0}-", skinIndex));
                    return tpl;
                }

                return info;
            }
            else
            {
                EB.Debug.LogError("GetHeroInfo: HeroInfo not found, id = {0}", id);
                return null;
            }
        }
        //通过templateid获取ModelName，方便热更调用
        public string GetHeroInfoByTemplateAndSkinId(string tid,int skin)
        {
            string charaterid = TemplateidToCharacterid(tid);
            HeroInfoTemplate heroInfo = GetHeroInfo(charaterid, skin);
            if (heroInfo != null)
            {
                return heroInfo.model_name;
            }      
                return null;
        }
        public HeroInfoTemplate GetHeroInfoByModel(string modelName)
        {
            Dictionary<int, HeroInfoTemplate>.Enumerator enumerator = mHeroInfos.GetEnumerator();

            while (enumerator.MoveNext())
            {
                HeroInfoTemplate result = enumerator.Current.Value;
                if (result.model_name == modelName)//主线获得英雄跟雇佣兵没处理觉醒相关
                {
                    return result;
                }
            }
            return null;
        }

        public List<ArtifactEquipmentTemplate> GetArtifactEquipmentHaveDesc(int infoId)
        {
            List<ArtifactEquipmentTemplate> havedescTemplate = new List<ArtifactEquipmentTemplate>();
            foreach (var template in _artifactEquipmentTemplates)
            {
                if (infoId == template.heroId && !string.IsNullOrEmpty(template.desc))
                {
                    havedescTemplate.Add(template);
                }
            }

            return havedescTemplate;
        }
        
        public int GetArtifactEquipmentMaxLevel(int infoId)
        {
            int level = 0;
            foreach (var template in _artifactEquipmentTemplates)
            {
                if (infoId == template.heroId && template.enhancementLevel >level)
                {
                    level = template.enhancementLevel;
                }
            }

            return level;
        }

        public ArtifactEquipmentTemplate GetArtifactEquipmentByLevel(int infoId,int level,bool logError=false)
        {
            //level限定大于0
            level = Mathf.Max(0, level);
            foreach (var template in _artifactEquipmentTemplates)
            {
                if (infoId == template.heroId && level == template.enhancementLevel)
                {
                    return template;
                }
            }
            if(logError) Debug.LogError($"Get ArtifactEquipment fail infoId:{infoId},level:{level}");
            return null;
        }

        public HeroInfoTemplate[] GetHeroInfos()
        {
            return mHeroInfos.Values.ToArray();
        }

        public HeroInfoTemplate GetHeroInfoNewest()
        {
            var AllInfo = GetHeroInfos();
            int maxId = 0;
            HeroInfoTemplate heroInfoTemplate = null;
            for (int i = 0; i < AllInfo.Length; i++)
            {
                if (AllInfo[i].obj.IsNew == 1)
                {
                    if (AllInfo[i].id > maxId)
                    {
                        maxId = AllInfo[i].id;
                        heroInfoTemplate = AllInfo[i];
                    }
                }
            }
            return heroInfoTemplate;
        }
        
        /// <summary>
        /// 传入比当前等级高一级的将返回当前等级的数据，如果不存在，将会设置isMaxLevel为true
        /// </summary>
        /// <param name="index">左边位置</param>
        /// <param name="condition">右边位置</param>
        /// <param name="level">等级</param>
        /// <returns></returns>
        public HeroGuardTemplate GetGuard(int index,int condition,int level)
        {
            bool isMaxLevel=false;
            if (level==0)
            {
                return new HeroGuardTemplate();
            }
            else
            {
                HeroGuardTemplate tpl=mGuardList.Find((guard) => { return
                    (guard.Index==index && guard.Condition==condition  && guard.Level == level);
                });
                
                if (tpl!=null)
                {
                    return tpl;
                }
                else
                {
                    isMaxLevel = true;
                }
                HeroGuardTemplate sub_tpl= mGuardList.Find((guard) => { return
                    (guard.Index==index && guard.Condition==condition  && guard.Level == (level-1));
                });

                if (sub_tpl!=null)
                {
                    sub_tpl.IsMaxLevel = isMaxLevel;
                    return sub_tpl;
                }
                else
                {
                    EB.Debug.LogError("not find index:{0},condition:{1},level:{2} row!",index,condition,level);
                    return null;
                }
            }
        }

        //伙伴的HeroId 不是护卫的
        public HeroGuardTemplate GetGuardByHeroId(int index,int condition,int HeroId)
        {
            int level = LTPartnerDataManager.Instance.GetGuardLevel(HeroId,index,condition);
            return GetGuard(index,condition,level);
        }
        public MonsterInfoTemplate GetMonsterInfo(int id)
        {
            if (!mMonsterInfos.TryGetValue(id, out MonsterInfoTemplate result))
            {
                if (monsterIdToIndex.TryGetValue(id.ToString(), out int index))
                {
                    Monster info = conditionSet.GetMonster(index);
                    result = ParseMonsterInfo(info);
                    mMonsterInfos.Add(id, result);
                }
            }

            return result;
        }

        public Hashtable GetMonsterInfoFromMain(int TplId)
        {
            MonsterInfoTemplate data = GetMonsterInfo(TplId);
            if (data != null)
            {
                string str = data.spawn_camera ?? null;
                return new Hashtable() { { "monster_type", data.monster_type }, { "spawn_camera", str }, { "scale_mul", data.scale_mul } };
            }
            return null;
        }

        public bool IsBoss(int TplId)
        {
            MonsterInfoTemplate monsterInfo = GetMonsterInfo(TplId);
            HeroStatTemplate heroStat = GetHeroStat(TplId);
            if (monsterInfo != null && heroStat == null)
                return monsterInfo.monster_type == 3;
            return false;
        }

        public int GerHeroPastExpSum(int level)
        {
            int sum = 0;
            for (int i = 0; i < mHeroLevelInfoList.Count; i++)
            {
                if (mHeroLevelInfoList[i].level < level)
                {
                    sum += mHeroLevelInfoList[i].buddy_exp;
                }
            }

            return sum;
        }

        public int GetHeroLevelByExp(int exp)
        {
            int level = 1;
            int expSum = 0;
            for (int i = 0; i < mHeroLevelInfoList.Count; i++)
            {
                if (expSum <= exp)
                {
                    expSum = expSum + mHeroLevelInfoList[i].buddy_exp;
                    level = mHeroLevelInfoList[i].level;
                }
                else
                {
                    break;
                }
            }
            return level;
        }

        public HeroLevelInfoTemplate GetHeroLevelInfo(int level)
        {
            for (int i = 0; i < mHeroLevelInfoList.Count; i++)
            {
                if (mHeroLevelInfoList[i].level == level)
                {
                    return mHeroLevelInfoList[i];
                }
            }
            return null;
        }

        public PlayerLevelTemplate GetPlayerLevelInfo(int level)
        {
            for (int i = 0; i < mPlayerLevelList.Count; i++)
            {
                if (mPlayerLevelList[i].level == level)
                {
                    return mPlayerLevelList[i];
                }
            }

            return mPlayerLevelList[mPlayerLevelList.Count - 1];
        }

        public int GetMaxPlayerLevel()
        {
            return mPlayerLevelList.Count;
        }

        public LTAttributesData GetStarHoleAttrByStarId(int heroStatId, int starId)
        {
            // mStarHoleAttr 的含义是 某个星级某个孔的所有加成属性
            // 例如：3星5孔， 那么对应的starId就是305；4星10孔 对应的starId就是410。
            // 因为开孔的属性全是累加的，所以例如starId：305，这个LTAttributesData就包含了3星5孔和之前孔的所有属性加成

            LTAttributesData ltarr;

            Dictionary<int, LTAttributesData> mStarHoleAttr = new Dictionary<int, LTAttributesData>();
            mStarInfoList.Sort((a, b) => { return a.id.CompareTo(b.id); });
            for (int i = 0; i < mStarInfoList.Count; i++)
            {
                LTAttributesData tempArr = new LTAttributesData();
                if (i > 0)
                {
                    //加上上一个星级孔的所有加成属性
                    LTAttributesData tempArrTemp = mStarHoleAttr[int.Parse(mStarInfoList[i - 1].id)];
                    tempArr.Add(tempArrTemp);
                }

                LevelUpInfoTemplate baseAttr = GetLevelUpInfoByIDAndLevel(heroStatId, mStarInfoList[i].level);
                if (mStarInfoList[i].attr == "MaxHP")
                {
                    tempArr.m_MaxHP += baseAttr.maxHP * mStarInfoList[i].param;
                }
                else if (mStarInfoList[i].attr == "ATK")
                {
                    tempArr.m_ATK += baseAttr.ATK * mStarInfoList[i].param;
                }
                else if (mStarInfoList[i].attr == "DEF")
                {
                    tempArr.m_DEF += baseAttr.DEF * mStarInfoList[i].param;
                }

                mStarHoleAttr.Add(int.Parse(mStarInfoList[i].id), tempArr);
                if (int.Parse(mStarInfoList[i].id) == starId) break;
            }

            mStarHoleAttr.TryGetValue(starId, out ltarr);

            return ltarr;
        }

        public List<StarInfoTemplate> GetStarInfoList()
        {
            return mStarInfoList;
        }

        public List<StarInfoTemplate> GetStarInfoListByStarLevel(int starLevel)
        {
            List<StarInfoTemplate> list = new List<StarInfoTemplate>();
            for (int i = 0; i < mStarInfoList.Count; i++)
            {
                if (mStarInfoList[i].star_level == starLevel)
                {
                    list.Add(mStarInfoList[i]);
                }
            }

            return list;
        }

        public StarInfoTemplate GetStarInfoByLevelHole(int starLevel, int starHole)
        {
            for (int i = 0; i < mStarInfoList.Count; i++)
            {
                if (mStarInfoList[i].star_level == starLevel && mStarInfoList[i].hole_position == starHole)
                {
                    return mStarInfoList[i];
                }
            }

            return null;
        }

        public UpGradeInfoTemplate GetUpGradeInfo(int level, eRoleAttr char_type)
        {
            for (int i = 0; i < mUpGradeInfos.Length; i++)
            {
                if (mUpGradeInfos[i].upGradeId == level && mUpGradeInfos[i].char_type == char_type)
                {
                    return mUpGradeInfos[i];
                }
            }

            return null;
        }

        public StarUpInfoTemplate GetStarUpInfo(int starId)
        {
            if (mStarUpInfoDic.ContainsKey(starId))
            {
                return mStarUpInfoDic[starId];
            }

            return null;
        }

        private LevelUp GetLevelUpInfo(string id, int level)
        {
            int len = conditionSet.LevelUpLength;

            for (int i = 0; i < len; i++)
            {
                LevelUp info = conditionSet.GetLevelUp(i);
                if (info.Id.Equals(id) && info.Level.Equals(level))
                {
                    return info;
                }
            }

            return null;
        }

        private LevelUpInfoTemplate GetLevelUpInfoByIDAndLevelEx(string id, int level)
        {
            Dictionary<int, LevelUpInfoTemplate> info;
            if (mLevelUpInfoDic.TryGetValue(id, out info) == false)
            {
                LevelUpInfoTemplate tpl = new LevelUpInfoTemplate()
                {
                    obj = GetLevelUpInfo(id, level)
                };

                info = new Dictionary<int, LevelUpInfoTemplate>
                {
                    { level, tpl }
                };

                mLevelUpInfoDic.Add(id, info);

                return tpl;
            }
            else
            {
                LevelUpInfoTemplate levelInfo;
                if (info.TryGetValue(level, out levelInfo) == false)
                {
                    levelInfo = new LevelUpInfoTemplate()
                    {
                        obj = GetLevelUpInfo(id, level)
                    };

                    info.Add(level, levelInfo);
                }

                return levelInfo;
            }
        }

        //public LevelUpInfoTemplate GetLevelUpInfoByIDAndLevel(string id, int level)
        //{
        //    if (mLevelUpInfoDic.ContainsKey(id) && mLevelUpInfoDic[id].ContainsKey(level))
        //    {
        //        return mLevelUpInfoDic[id][level];
        //    }

        //    return new LevelUpInfoTemplate();
        //}

        public LevelUpInfoTemplate GetLevelUpInfoByIDAndLevel(int id, int level)
        {
            //return GetLevelUpInfoByIDAndLevel(id.ToString(), level);
            return GetLevelUpInfoByIDAndLevelEx(id.ToString(), level);
        }

        public bool HasHeroAwakeInfo(int id)
        {
            return mAwakenInfos.ContainsKey(id);
        }

        public HeroAwakeInfoTemplate GetHeroAwakeInfoByInfoID(int id)
        {
            if (mAwakenInfos.ContainsKey(id))
            {
                return mAwakenInfos[id];
            }
            return null;
        }
        public HeroAwakeInfoTemplate GetHeroAwakeInfoByInfoID(string id)
        {
            return GetHeroAwakeInfoByInfoID(int.Parse(id));
        }

        public bool IsAwakenSkill(int skillId)
        {
            if (mAwakenSkillList.Contains(skillId))
            {
                return true;
            }
            return false;
        }

        public string GetAwakenExtraAttri(HeroAwakeInfoTemplate temp)
        {
            string str = null;
            if (temp.DmgMulti != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_DMGincrease") + "+{0}% ", temp.DmgMulti * 100);
            if (temp.DmgRes != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_DMGreduction") + "+{0}% ", temp.DmgRes * 100);
            if (temp.speedAdd != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_Speed") + "+{0} ", temp.speedAdd);
            if (temp.SpExtra != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_SpExtra") + "+{0}% ", temp.SpExtra * 100);
            if (temp.SpRes != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_SpRes") + "+{0}% ", temp.SpRes * 100);
            if (temp.AntiCritP != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_CRIresist") + "+{0}%", temp.AntiCritP * 100);
            if (temp.CritP != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_CritP") + "+{0}% ", temp.CritP * 100);
            if (temp.CritV != 0) str += string.Format(EB.Localizer.GetString("ID_ATTR_CritV") + "+{0%}", temp.CritV * 100);
            return str;

        }

        private Dictionary<int, HeroInfoTemplate> hbHeroInfoTemplates = new Dictionary<int, HeroInfoTemplate>();
        public Dictionary<int, HeroInfoTemplate> GetAllHBHeroInfoTemplate()
        {
            if (hbHeroInfoTemplates.Count == 0)
            {
                Dictionary<int, HeroInfoTemplate>.Enumerator enumerator = mHeroInfos.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    HeroInfoTemplate result = enumerator.Current.Value;
                    if (result.isShowInClash)
                    {
                        hbHeroInfoTemplates.Add(result.id, result);
                    }
                }
            }
            return hbHeroInfoTemplates;
        }

        #region 晋升相关
        private bool InitPromotion(GM.DataCache.ConditionCharacter conditionCharacter) {
            if (conditionCharacter == null) {
                EB.Debug.LogError("InitPromotion: can not find conditionCharacter data");
                return false;
            }

            int len = conditionCharacter.PromotionLength;
            mPromotionDictById = new Dictionary<int, PromotionTemplate>(len);
            mPromotionDictByLevelStar = new Dictionary<string, PromotionTemplate>(len);
            mPromotionStarCountDictByLevel = new Dictionary<int, int>();
            mPromotionLinkedDict = new Dictionary<int, int>();
            int previousId = 0;

            for (int i = 0; i < len; ++i) {
                var info = conditionCharacter.GetPromotion(i);
                var tpl = ParsePromotion(info);
                mPromotionDictById.Add(info.Id, tpl);
                mPromotionDictByLevelStar.Add(string.Format("{0}-{1}", info.Level, info.Star), tpl);

                if (mPromotionStarCountDictByLevel.ContainsKey(info.Level) && mPromotionStarCountDictByLevel[info.Level] < info.Star) {
                    mPromotionStarCountDictByLevel[info.Level] = info.Star;
                } else {
                    mPromotionStarCountDictByLevel.Add(info.Level, info.Star);
                }

                if (i != 0) {
                    mPromotionLinkedDict.Add(previousId, info.Id);
                }

                previousId = info.Id;
            }

            return true;
        }

        private PromotionTemplate ParsePromotion(GM.DataCache.Promotion obj) {
            var tpl = new PromotionTemplate();
            tpl.id = obj.Id;
            tpl.qualityLevel = obj.QualityLevel;
            tpl.level = obj.Level;
            tpl.star = obj.Star;
            tpl.itemId = obj.ItemId;
            tpl.cost = obj.Cost;
            tpl.additiveAttributeLevel = obj.AdditiveAttributeLevel;
            tpl.attributeLevelUpperLimit = obj.AttributeLevelUpperLimit;
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_herostats_promotion_{0}_name", obj.Id), obj.Name);
            tpl.bigIcon = obj.BigIcon;
            tpl.smallIcon = obj.SmallIcon;
            tpl.taskIds = obj.TaskIds;
            return tpl;
        }

        private bool InitPromotionTraining(GM.DataCache.ConditionCharacter conditionCharacter) {
            if (conditionCharacter == null) {
                EB.Debug.LogError("InitPromotionTraining: can not find conditionCharacter data");
                return false;
            }

            int len = conditionCharacter.PromotionTrainingLength;
            mPromotionTrainingDictById = new Dictionary<int, PromotionTrainingTemplate>(len);

            for (int i = 0; i < len; ++i) {
                var info = conditionCharacter.GetPromotionTraining(i);
                mPromotionTrainingDictById.Add(info.Id, ParsePromotionTraining(info));
            }

            return true;
        }

        private PromotionTrainingTemplate ParsePromotionTraining(GM.DataCache.PromotionTraining obj) {
            var tpl = new PromotionTrainingTemplate();
            tpl.id = obj.Id;
            tpl.upperLimit = obj.UpperLimit;
            tpl.count = obj.Count;
            tpl.name = EB.Localizer.GetTableString(string.Format("ID_herostats_promotion_training_{0}_name", obj.Id), obj.Name);
            tpl.cost = obj.Cost;
            tpl.negativeRegion = obj.NegativeRegion;
            tpl.positiveRegion = obj.PositiveRegion;
            tpl.regionPercent = obj.RegionPercent;
            tpl.negativeProbability = obj.NegativeProbability;
            return tpl;
        }

        private bool InitPromotionAttributeLevel(GM.DataCache.ConditionCharacter conditionCharacter) {
            if (conditionCharacter == null) {
                EB.Debug.LogError("InitPromotionAttributeLevel: can not find conditionCharacter data");
                return false;
            }

            int len = conditionCharacter.PromotionAttributeLevelLength;
            mPromotionAttributeLevelDictById = new Dictionary<int, PromotionAttributeLevelTemplate>(len);
            mPromotionAttributeLevelDictByName = new Dictionary<string, PromotionAttributeLevelTemplate>(len);

            for (int i = 0; i < len; ++i) {
                var info = conditionCharacter.GetPromotionAttributeLevel(i);
                var tpl = ParsePromotionAttributeLevel(info);
                mPromotionAttributeLevelDictById.Add(info.Id, tpl);
                mPromotionAttributeLevelDictByName.Add(info.Name, tpl);
            }

            return true;
        }
        
        private bool InitArtifactEquipments(ConditionCharacter conditionCharacter)
        {
            int len = conditionCharacter.ArtifactEquipmentsLength;
            for (int i = 0; i < len; i++)
            {
                var info = conditionCharacter.GetArtifactEquipments(i);
                _artifactEquipmentTemplates.Add(ParseArtifactEquipment(info));
            }
            return true;
        }

        private ArtifactEquipmentTemplate ParseArtifactEquipment(ArtifactEquipments item)
        {
            ArtifactEquipmentTemplate template = new ArtifactEquipmentTemplate();
            template.id = item.Id;
            template.name = item.Name;
            template.heroId = Convert.ToInt32(item.HeroId);
            template.enhancementLevel = item.EnhancementLevel;
            template.qualityLevel = item.QualityLevel;
            template.iconId = item.IconId;
            template.AttributeAdd = item.AttributeAdd;
            template.ItemCost = item.ItemCost;
            template.skillId = item.SkillId;
            template.desc = item.EnhancementSkillDescribe;
            return template;
        }

        private PromotionAttributeLevelTemplate ParsePromotionAttributeLevel(GM.DataCache.PromotionAttributeLevel obj) {
            var tpl = new PromotionAttributeLevelTemplate();
            tpl.id = obj.Id;
            tpl.unlockLevel = obj.UnlockLevel;
            tpl.attrValue = obj.AttrValue;
            tpl.name = obj.Name;
            return tpl;
        }

        public PromotionTemplate GetPromotionInfo(int id) {
            PromotionTemplate value;

            if (mPromotionDictById.TryGetValue(id, out value)) {
                return value;
            } else {
                return null;
            }
        }

        public PromotionTemplate GetPromotionInfo(int level, int star) {
            var key = string.Format("{0}-{1}", level, star);
            PromotionTemplate value;

            if (mPromotionDictByLevelStar.TryGetValue(key, out value)) {
                return value;
            }else {
                EB.Debug.Log("not find promotioninfo==>level{0} star==>{1}", level, star);
                return null;
            }
        }

        public int GetPromotionLevelStarCount(int level) {
            int value;

            if (mPromotionStarCountDictByLevel.TryGetValue(level, out value)) {
                return value;
            } else {
                return 0;
            }
        }

        public int GetNextPromotionId(int id) {
            int value;

            if (mPromotionLinkedDict.TryGetValue(id, out value)) {
                return value;
            } else {
                return 0;
            }
        }

        public PromotionTrainingTemplate GetPromotionTrainingInfo(int id) {
            PromotionTrainingTemplate value;

            if (mPromotionTrainingDictById.TryGetValue(id, out value)) {
                return value;
            } else {
                return null;
            }
        }

        public PromotionAttributeLevelTemplate GetPromotionAttributeLevelInfo(int id) {
            PromotionAttributeLevelTemplate value;

            if (mPromotionAttributeLevelDictById.TryGetValue(id, out value)) {
                return value;
            } else {
                return null;
            }
        }


        public PromotionAttributeLevelTemplate GetPromotionAttributeLevelInfo(string attrName) {
            PromotionAttributeLevelTemplate value;

            if (mPromotionAttributeLevelDictByName.TryGetValue(attrName, out value)) {
                return value;
            } else {
                return null;
            }
        }
         
        public List<PromotionAttributeLevelTemplate> GetPromotionAttributeLevelList(int level) {
            var list = new List<PromotionAttributeLevelTemplate>();

            foreach (var val in mPromotionAttributeLevelDictByName.Values) {
                if (level >= val.unlockLevel) {
                    list.Add(val);
                }
            }

            list.Sort((left, right) => { 
                if (left.id < right.id) {
                    return -1;
                } else if (left.id > right.id) {
                    return 1;
                } else {
                    return 0;
                }
            });

            return list;
        }
        #endregion
    }
}