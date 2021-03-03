using System;
using System.Collections;
using System.Collections.Generic;
using EB;
using Hotfix_LT.UI;
using Unity.Standard.ScriptsWarp;

namespace Hotfix_LT.Data
{
    public enum eSkillDamageType
    {
        INVALID = -1,
        Physic = 0,
        Magic = 1,
        PandM = 2,
        Max
    }
    public enum eSkillElemental
    {
        INVALID = -1,
        None = 0,
        Gold = 1,
        Wood = 2,
        Water = 3,
        Fire = 4,
        Earth = 5,
        Max
    }

    public enum eSkillType
    {
        INVALID = -1,
        NORMAL = 0,
        ACTIVE = 1,
        PASSIVE = 2,

        MANUAL = 3,
        COMBO = 4,
        SCROLL = 5,
        SYSTEM_PASSIVE = 6,
        INDICATOR = 7
        //MAX
    }

    public enum eSkillSelectType
    {
        INVALID = -1,
        AUTO = 0,
        MANUAL_ENEMY = 1,
        MANUAL_TEAMMATE = 2,
        MAX
    }

    /// <summary>
    /// 技能选择目标类型
    /// </summary>
    public enum eSkillSelectTargetType
    {
        None = 0,
        /// <summary>敌方单体</summary>
        ENEMY_TEMPLATE = 1,
        /// <summary>敌方全体</summary>
        ENEMY_ALL = 2,
        /// <summary>自身</summary>
        SELF = 3,
        /// <summary>己方单体</summary>
        FRIEND_TEMPLATE = 4,
        /// <summary>己方全体</summary>
        FRIEND_ALL = 5,
        /// <summary>非自身所有单位</summary>
        All_NOT_SELF = 6,
        /// <summary>随机敌方目标</summary>
        ENEMY_RANDOM = 7,
        /// <summary>随机己方目标</summary>
        FRIEND_RANDOM = 8
    }

    [System.Flags]
    public enum eSpecialFlags
    {
        None = 0,
        UseInDeath = (1 << 1),
        SelectDeath = (1 << 2)
    }

    public enum eEnvironmentHideType
    {
        INVALID = -1,
        NONE = 0,
        FADE = 1,
        BLACK = 2
    }

    public enum eSkillTriggerType
    {
        INVALID = -1,
        NONE = 0,
        ON_COMBAT_READY = 1,
        BEFORE_MANUAL = 2,
        AFTER_MANUAL = 3,
        AFTER_ATTACKED_BY_NORMAL = 4,
        AFTER_ATTACKED_BY_MANUAL = 5,
        ON_DAMAGE_TARGET = 6,
        ON_LITTLE_FLOW = 7,
        ON_LARGE_FLOW = 8,
        ON_BEAT_BACK = 9,
        ON_BEAT_DOWN = 10,
        ON_FROZEN = 11,
        ON_DOUBLE_HIT = 12,
        BEFORE_ATTACKED_BY_MANUAL = 13,
        ON_TEAMMATE_DEBUFF = 14,
        ON_TEAMMATE_DEATH = 15,
        ON_ENEMY_DEATH = 16,
        ON_DEATH = 17,
        ON_SELF_DEATH = 18,
        ON_SELF_RELIVE = 19,
        ON_SELF_CRITICAL = 20,
        ON_SELF_SELECTED = 21,
        MAX
    }

    public class SkillTemplate
    {
        public int ID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public eSkillType Type { get; set; }
        public eSkillSelectTargetType SelectTargetType { get; set; }
        public int SPCost { get; set; }
        public int MaxCooldown { get; set; }
        public string AnimatorStateFullPathName { get; set; }
        public string MoveName { get { return AnimatorStateFullPathName.Split('.')[1]; } }
        public MoveEditor.Move.eSkillTargetPositionType TargetPosition { get; set; }
        public MoveEditor.Move.eSkillActionPositionType ActionPosition { get; set; }
        public float TargetDistance { get; set; }
        public bool IgnoreCollide { get; set; }
        public bool IsDefault { get; set; }
        public bool IsNormalAttack { get { return Type == eSkillType.NORMAL && IsDefault; } }
        public bool HideOthers { get; set; }
        public eEnvironmentHideType FadeEnvironment { get; set; }
        public int DoubleHitCount { get; set; }
        public eSkillTriggerType TriggerType { get; set; }
//#if UNITY_EDITOR//未发现引用
//        public int DamageRating { get; set; }
//        public int DamageIncRating { get; set; }
//        public int PATKRating { get; set; }
//        public int MATKRating { get; set; }
//        public int PDEFRating { get; set; }
//        public int MDEFRating { get; set; }
//        public int BaseDamage { get; set; }
//        public int IncDamage { get; set; }
//#endif
        public eSpecialFlags SpecialFlags { get; set; }
        public bool CanUseInDeath { get { return (SpecialFlags & eSpecialFlags.UseInDeath) == eSpecialFlags.UseInDeath; } }
        public bool CanSelectDeath { get { return (SpecialFlags & eSpecialFlags.SelectDeath) == eSpecialFlags.SelectDeath; } }

        public List<int> BuffDescribleID { get; set; }
        public float BattleRating { get; set; }
    }

    public class SkillLevelUpTemplate
    {
        public int ID { get; set; }
        public float Rating { get; set; }
        public float DamageIncPercent { get; set; }
        public float AtkIncPercent { get; set; }
        public float DefIncPercnet { get; set; }
        public float MaxHpIncPercent { get; set; }
        public float CDRating { get; set; }
        public int CDCount { get; set; }
    }

    public class SkillTemplateManager
    {
        private static SkillTemplateManager s_instance;

        private Dictionary<int, SkillTemplate> m_skillDataDictionary = new Dictionary<int, SkillTemplate>();
        private Dictionary<int, SkillLevelUpTemplate> m_skillLevelUpDataDic = new Dictionary<int, SkillLevelUpTemplate>();

        public static SkillTemplateManager Instance
        {
            get { return s_instance = s_instance ?? new SkillTemplateManager(); }
        }

        private SkillTemplateManager()
        {

        }

        public SkillTemplate GetTemplate(int skill_id)
        {
            if (skill_id <= 0) return null;
            SkillTemplate skillTemplate;
            if (m_skillDataDictionary.TryGetValue(skill_id, out skillTemplate))
            {
                return skillTemplate;
                //skillTemplate= m_skillDataDictionary[skill_id];
            }

            EB.Debug.LogWarning("GetTemplate: skill not found, id = {0}", skill_id);
            return null;
        }
        
        public SkillTemplate GetTemplateWithAwake(LTPartnerData curPartnerData,int skill_id,Action action=null)
        {
            SkillTemplate skillTemplate = null;
            if (skill_id <= 0) return null;
            if (!m_skillDataDictionary.TryGetValue(skill_id,out skillTemplate))
            {
                return null;
                //skillTemplate= m_skillDataDictionary[skill_id];
            }
            HeroAwakeInfoTemplate curAwakenTemplate =CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(curPartnerData.InfoId);
            if (curPartnerData.IsAwaken>0 && skill_id==curAwakenTemplate.beforeSkill)
            {
                skillTemplate =Instance.GetTemplate(curAwakenTemplate.laterSkill);
                action?.Invoke();
            }
            // EB.Debug.LogWarning("GetTemplate: skill not found, id = {0}", skill_id);
            return skillTemplate;
        }

        //其他人技能显示觉醒替换
        public SkillTemplate GetTemplateWithAwake(int infoId, int skill_id, int awakelevel, Action action = null)
        {
            SkillTemplate skillTemplate = null;
            if (skill_id <= 0) return null;
            if (!m_skillDataDictionary.TryGetValue(skill_id, out skillTemplate))
            {
                return null;
                //skillTemplate= m_skillDataDictionary[skill_id];
            }
            HeroAwakeInfoTemplate curAwakenTemplate = CharacterTemplateManager.Instance.GetHeroAwakeInfoByInfoID(infoId);
            if (awakelevel > 0 && skill_id == curAwakenTemplate.beforeSkill)
            {
                skillTemplate = Instance.GetTemplate(curAwakenTemplate.laterSkill);
                action?.Invoke();
            }
            // EB.Debug.LogWarning("GetTemplate: skill not found, id = {0}", skill_id);
            return skillTemplate;
        }

        public Hashtable GetTemplateEx(int skill_id)
        {
            SkillTemplate data = GetTemplate(skill_id);
            return new Hashtable() { 
                { "moveName", data.MoveName }, 
                { "doubleHitCount", data.DoubleHitCount }, 
                { "actionPosition", data.ActionPosition } , 
                { "targetPosition", data.TargetPosition },
                { "id", data.ID } , 
                { "fadeEnvironment", data.FadeEnvironment },
                { "targetDistance", data.TargetDistance },
                { "ignoreCollide", data.IgnoreCollide },
                { "hideOthers", data.HideOthers }
            };
        }

        public string GetSkillName(int id)
        {
            var skill = GetTemplate(id);
            return skill != null ? skill.Name : "UnknownSkill";
        }
        public int GetSkillType(int skill_id)
        {
            int type = 0;
            SkillTemplate data = GetTemplate(skill_id);
            if (data!=null)
            {
                type = (int) data.Type;
            }
            else
            {
                Debug.LogError("没有找到对应的skill_id:{0}",skill_id);
            }
            return type;
        }
        public string GetSkillIcon(int skill_id)
        {
            SkillTemplate data = GetTemplate(skill_id);
            return data.Icon;
        }
        public int GetMaxCooldown(int skill_id)
        {
            SkillTemplate data = GetTemplate(skill_id);
            return data.MaxCooldown;
        }
        
        public Hashtable GetSkillInfo(int skill_id)
        {
            SkillTemplate data = GetTemplate(skill_id);
            return new Hashtable() { { "skillType", data.Type}, { "moveName", data.MoveName } };
        }
        public bool IsComBo(int skill_id)
        {
            SkillTemplate skill_data = GetTemplate(skill_id);
            return skill_data.Type == Hotfix_LT.Data.eSkillType.COMBO;
        }

        public SkillLevelUpTemplate GetSkillLevelUpTemplate(int skill_id)
        {
            if (m_skillLevelUpDataDic.ContainsKey(skill_id))
            {
                return m_skillLevelUpDataDic[skill_id];
            }

            EB.Debug.LogWarning("GetSkillLevelUpTemplate: skill not found, id = {0}", skill_id);
            return null;
        }

        public bool HasSkillLevelUpTemplate(int skill_id)
        {
            return m_skillLevelUpDataDic.ContainsKey(skill_id);
        }

        public static void ClearUp()
        {
            if (s_instance != null)
            {
                s_instance.m_skillDataDictionary.Clear();
                s_instance.m_skillLevelUpDataDic.Clear();
            }
        }

        public bool InitFromDataCache(GM.DataCache.Combat combat)
        {
            if (combat == null)
            {
                EB.Debug.LogError("can not find skills data");
                return false;
            }

            m_skillDataDictionary.Clear();
            var conditionSet = combat.GetArray(0);
            for (int i = 0; i < conditionSet.SkillsLength; ++i)
            {
                var skill = conditionSet.GetSkills(i);
                var tpl = ParseTemplate(skill);
                m_skillDataDictionary[tpl.ID] = tpl;
            }

            return true;
        }

        private SkillTemplate ParseTemplate(GM.DataCache.SkillTemplate skill)
        {
            SkillTemplate skill_data = new SkillTemplate();

            skill_data.ID = skill.Id;
			using (ZString.Block())
			{
				ZString strID = ZString.Format("ID_combat_skills_{0}_name", skill_data.ID);
				skill_data.Name = EB.Localizer.GetTableString(strID, skill.Name); ;//skill.Name;

				strID = ZString.Format("ID_combat_skills_{0}_description", skill_data.ID);
				skill_data.Description = EB.Localizer.GetTableString(strID, skill.Description); ;//skill.Description;
			}			
            
            skill_data.Icon = skill.Icon;
            skill_data.Type = (eSkillType)(skill.SkillType);
            skill_data.SelectTargetType = (eSkillSelectTargetType)skill.Target;
            skill_data.SPCost = skill.SPCost;
            skill_data.MaxCooldown = skill.CooldownTurnNum;
            string move = skill.Action ?? string.Empty;
			if(!move.Equals(string.Empty))
			{
				StringView view = new StringView(move);
				var list = view.Split2List('.');
				if (list.Count == 1)
				{
					move = "Moves." + move;
				}
				list = null;
            }
            else
            {
                move = "Moves.";
            }
			
            skill_data.AnimatorStateFullPathName = move;
            int action_position = skill.SkillPosition;

            switch (action_position)
            {
                case 0:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.ORIGINAL;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.SELECT_TARGET_OR_FIRST_TARGET;
                    break;
                case 1:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.MIDPOINT;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.SELECT_TARGET_OR_FIRST_TARGET;
                    break;
                case 2:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.TARGET;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.SELECT_TARGET_OR_FIRST_TARGET;
                    break;
                case 3:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.TARGET;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.TARGETS;
                    break;
                case 4:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.MIDLINE;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.SELECT_TARGET_OR_FIRST_TARGET;
                    break;
                case 5:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.MIDPOINT;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.TARGETS;
                    break;
                default:
                    skill_data.ActionPosition = MoveEditor.Move.eSkillActionPositionType.ORIGINAL;
                    skill_data.TargetPosition = MoveEditor.Move.eSkillTargetPositionType.SELECT_TARGET_OR_FIRST_TARGET;
                    break;
            }
            skill_data.TargetDistance = skill.TargetDistance / 100.0f;
            skill_data.IgnoreCollide = true;
            skill_data.HideOthers = skill.HideOthers;
            skill_data.FadeEnvironment = (eEnvironmentHideType)skill.FadeEnvironment;
            skill_data.BattleRating = skill.BattleRating;
            if (!string.IsNullOrEmpty(skill.BuffDescribleId))
            {
                skill_data.BuffDescribleID = new List<int>();
				StringView view = new StringView(skill.BuffDescribleId);
				var ids = view.Split2List(',');
                for (int i = 0; i < ids.Count; i++)
                {
                    int buffid = 0;
                    int.TryParse(ids[i].ToString(), out buffid);
                    if (buffid != 0) skill_data.BuffDescribleID.Add(buffid);
                }
            }
            return skill_data;
        }

        public bool InitSkillLevelUpFromDataCache(GM.DataCache.Combat combat)
        {
            if (combat == null)
            {
                EB.Debug.LogError("InitSkillLevelUpFromDataCache + can not find skills data");
                return false;
            }

            m_skillLevelUpDataDic.Clear();
            var conditionSet = combat.GetArray(0);
            for (int i = 0; i < conditionSet.SkillLevelUpLength; ++i)
            {
                var skill = conditionSet.GetSkillLevelUp(i);
                var tpl = ParseSkillLevelUpTemplate(skill);
                m_skillLevelUpDataDic[tpl.ID] = tpl;
            }

            return true;
        }

        private SkillLevelUpTemplate ParseSkillLevelUpTemplate(GM.DataCache.SkillLevelUp skillLevelUp)
        {
            SkillLevelUpTemplate skillLevelUpData = new SkillLevelUpTemplate();
            skillLevelUpData.ID = skillLevelUp.Id;
            skillLevelUpData.Rating = skillLevelUp.Rating;
            skillLevelUpData.DamageIncPercent = skillLevelUp.DamageIncPercent;
            skillLevelUpData.AtkIncPercent = skillLevelUp.ATKIncPercent;
            skillLevelUpData.DefIncPercnet = skillLevelUp.DEFIncPercent;
            skillLevelUpData.MaxHpIncPercent = skillLevelUp.MaxHPIncPercent;

            if (string.IsNullOrEmpty(skillLevelUp.ReduceCdRating))
            {
                return skillLevelUpData;
            }
            string[] strs = skillLevelUp.ReduceCdRating.Split('#');
            if (strs.Length < 2)
            {
                EB.Debug.LogError("SkillLevelUpTemplate Init Error, ReduceCDRating Length Less 2, SkillID : {0}" , skillLevelUp.Id);
            }
            else
            {
                float cdRating = 0;
                float.TryParse(strs[0], out cdRating);
                skillLevelUpData.CDRating = cdRating;

                int cdCount = 0;
                int.TryParse(strs[1], out cdCount);
                skillLevelUpData.CDCount = cdCount;
            }

            return skillLevelUpData;
        }

    }
}