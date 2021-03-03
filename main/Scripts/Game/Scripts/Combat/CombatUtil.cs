using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class CombatUtil
    {
        private static Dictionary<string, eCombatEventType> sEventTypeDic = new Dictionary<string, eCombatEventType>();
        private static Dictionary<int, System.Type> sEventClassDic = new Dictionary<int, System.Type>();
        private static Dictionary<string, eCombatEventTiming> sEventTimingDic = new Dictionary<string, eCombatEventTiming>();
        private static Dictionary<string, eCombatEffectType> sEffectTypeDic = new Dictionary<string, eCombatEffectType>();
        private static Dictionary<int, System.Type> sEffectClassDic = new Dictionary<int, System.Type>();
        private static Dictionary<string, CombatantAttributes.eAttribute> sAttrTypeDic = new Dictionary<string, CombatantAttributes.eAttribute>();

        static CombatUtil()
        {
            InitDictionaries();
        }

        /// <summary>
        /// 判断当前的特效是否是出场英雄
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public static bool IsHeroStartEffect(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }
            //居然把出场写成chushang，我也没办法(之前的人留下注解)
            return assetName.Contains("chuchang") || assetName.Contains("chushang");
        }

        private static void InitDictionaries()
        {
            if (sEventTypeDic.Count == 0)
            {
                sEventTypeDic["skill"] = eCombatEventType.SKILL;
                sEventTypeDic["impact"] = eCombatEventType.IMPACT;
                sEventTypeDic["sync"] = eCombatEventType.SYNC;
                sEventTypeDic["conversation"] = eCombatEventType.CONVERSATION;
                sEventTypeDic["quit"] = eCombatEventType.QUIT;
                sEventTypeDic["guide"] = eCombatEventType.GUIDE;
                sEventTypeDic["lobby"] = eCombatEventType.LOBBY;
            }

            if (sEventClassDic.Count == 0)
            {
                sEventClassDic[(int)eCombatEventType.SKILL] = typeof(CombatSkillEvent);
                sEventClassDic[(int)eCombatEventType.IMPACT] = typeof(CombatImpactEvent);
                sEventClassDic[(int)eCombatEventType.SYNC] = typeof(CombatSyncEvent);
                sEventClassDic[(int)eCombatEventType.CONVERSATION] = typeof(CombatConversationEvent);
                sEventClassDic[(int)eCombatEventType.QUIT] = typeof(CombatQuitEvent);
                sEventClassDic[(int)eCombatEventType.GUIDE] = typeof(CombatGuideEvent);
                sEventClassDic[(int)eCombatEventType.LOBBY] = typeof(CombatLobbyEvent);
            }

            if (sEventTimingDic.Count == 0)
            {
                sEventTimingDic["auto"] = eCombatEventTiming.AUTO;
                sEventTimingDic["on_start"] = eCombatEventTiming.ON_START;
                sEventTimingDic["on_end"] = eCombatEventTiming.ON_END;
                sEventTimingDic["on_forward_start"] = eCombatEventTiming.ON_FORWARD_START;
                sEventTimingDic["on_forward_end"] = eCombatEventTiming.ON_FORWARD_END;
                sEventTimingDic["on_skill_start"] = eCombatEventTiming.ON_SKILL_START;
                sEventTimingDic["on_skill_end"] = eCombatEventTiming.ON_SKILL_END;
                sEventTimingDic["on_hit_start"] = eCombatEventTiming.ON_HIT_START;
                sEventTimingDic["on_hit_end"] = eCombatEventTiming.ON_HIT_END;
                sEventTimingDic["on_backward_start"] = eCombatEventTiming.ON_BACKWARD_START;
                sEventTimingDic["on_backward_end"] = eCombatEventTiming.ON_BACKWARD_END;
                sEventTimingDic["hit_queue"] = eCombatEventTiming.HIT_QUEUE;
                sEventTimingDic["on_reaction_start"] = eCombatEventTiming.ON_REACTION_START;
                sEventTimingDic["on_reaction_end"] = eCombatEventTiming.ON_REACTION_END;
                sEventTimingDic["on_combo"] = eCombatEventTiming.ON_COMBO;
                sEventTimingDic["collide_queue"] = eCombatEventTiming.COLLIDE_QUEUE;
            }

            if (sEffectTypeDic.Count == 0)
            {
                sEffectTypeDic["reaction"] = eCombatEffectType.REACTION;
                sEffectTypeDic["damage"] = eCombatEffectType.DAMAGE;
                sEffectTypeDic["heal"] = eCombatEffectType.HEAL;
                sEffectTypeDic["impact_active"] = eCombatEffectType.IMPACT_ACTIVE;
                sEffectTypeDic["impact_fadeout"] = eCombatEffectType.IMPACT_FADEOUT;
                sEffectTypeDic["break"] = eCombatEffectType.BREAK_MANUAL_SKILL;
                sEffectTypeDic["exile"] = eCombatEffectType.EXILE;
                sEffectTypeDic["add_combatant"] = eCombatEffectType.Add_COMBATANT;
                sEffectTypeDic["remove_combatant"] = eCombatEffectType.REMOVE_COMBATANT;
                sEffectTypeDic["attributes"] = eCombatEffectType.ATTRIBUTES;
                sEffectTypeDic["revive"] = eCombatEffectType.REVIVE;
                sEffectTypeDic["shield"] = eCombatEffectType.SHIELD;
                sEffectTypeDic["tips"] = eCombatEffectType.TIPS;
            }

            if (sEffectClassDic.Count == 0)
            {
                sEffectClassDic[(int)eCombatEffectType.REACTION] = typeof(ReactionEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.DAMAGE] = typeof(DamageEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.HEAL] = typeof(HealEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.IMPACT_ACTIVE] = typeof(ImpactActiveEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.IMPACT_FADEOUT] = typeof(ImpactFadeoutEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.BREAK_MANUAL_SKILL] = typeof(BreakEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.EXILE] = typeof(ExileEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.Add_COMBATANT] = typeof(AddCombatantEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.REMOVE_COMBATANT] = typeof(RemoveCombatantEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.ATTRIBUTES] = typeof(AttributesEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.REVIVE] = typeof(ReviveEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.SHIELD] = typeof(ShieldEffectEvent);
                sEffectClassDic[(int)eCombatEffectType.TIPS] = typeof(TipsEffectEvent);
            }

            if (sAttrTypeDic.Count == 0)
            {
                sAttrTypeDic["maxHP"] = CombatantAttributes.eAttribute.MaxHp;
                sAttrTypeDic["maxMP"] = CombatantAttributes.eAttribute.MaxMp;
                sAttrTypeDic["speed"] = CombatantAttributes.eAttribute.Speed;
                sAttrTypeDic["dodgeRating"] = CombatantAttributes.eAttribute.Miss;
                sAttrTypeDic["critical"] = CombatantAttributes.eAttribute.Critical;
                sAttrTypeDic["criticalFactor"] = CombatantAttributes.eAttribute.CriticalFactor;
                sAttrTypeDic["criticalFactorAdd"] = CombatantAttributes.eAttribute.CriticalFactorAdd;
                sAttrTypeDic["PATK"] = CombatantAttributes.eAttribute.PhysicAttack;
                sAttrTypeDic["MATK"] = CombatantAttributes.eAttribute.MagicAttack;
                sAttrTypeDic["PDEF"] = CombatantAttributes.eAttribute.PhysicDefend;
                sAttrTypeDic["MDEF"] = CombatantAttributes.eAttribute.MagicDefend;
                sAttrTypeDic["penetration"] = CombatantAttributes.eAttribute.Penetration;
                sAttrTypeDic["spellPenetration"] = CombatantAttributes.eAttribute.SpellPenetration;
                sAttrTypeDic["damageReduction"] = CombatantAttributes.eAttribute.DamageReduction;
                sAttrTypeDic["stun"] = CombatantAttributes.eAttribute.Stun;
                sAttrTypeDic["hpRecover"] = CombatantAttributes.eAttribute.HpRecover;
                sAttrTypeDic["hp"] = CombatantAttributes.eAttribute.Hp;
                sAttrTypeDic["mp"] = CombatantAttributes.eAttribute.Mp;
                sAttrTypeDic["shield"] = CombatantAttributes.eAttribute.Shield;
                sAttrTypeDic["canRevive"] = CombatantAttributes.eAttribute.CanRevive;
                sAttrTypeDic["canUseManualSkill"] = CombatantAttributes.eAttribute.CanUseManualSkill;
                sAttrTypeDic["canNormalAttack"] = CombatantAttributes.eAttribute.CanNormalAttack;
                sAttrTypeDic["canUseComboSkill"] = CombatantAttributes.eAttribute.CanUseComboSkill;
                sAttrTypeDic["slow"] = CombatantAttributes.eAttribute.Slow;
                sAttrTypeDic["freezeHitReaction"] = CombatantAttributes.eAttribute.FreezeHitReaction;
            }
        }

        public static eCombatEventType GetType(string type_info)
        {
            Dictionary<string, eCombatEventType> dic = sEventTypeDic;

            if (string.IsNullOrEmpty(type_info) || !dic.ContainsKey(type_info))
            {
                return eCombatEventType.INVALID;
            }

            return dic[type_info];
        }

        public static CombatEvent CreateEventInstance(eCombatEventType type)
        {
            Dictionary<int, System.Type> class_type_dic = sEventClassDic;

            if (!class_type_dic.ContainsKey((int)type))
            {
                return null;
            }

            System.Type class_type = class_type_dic[(int)type];
            CombatEvent instance = System.Activator.CreateInstance(class_type) as CombatEvent;
            return instance;
        }

        public static eCombatEventTiming GetTiming(string timing_info)
        {
            Dictionary<string, eCombatEventTiming> timing_dic = sEventTimingDic;

            if (string.IsNullOrEmpty(timing_info) || !timing_dic.ContainsKey(timing_info))
            {
                return eCombatEventTiming.INVALID;
            }

            return timing_dic[timing_info];
        }

        public static eCombatEffectType GetEffectType(string type_info)
        {
            Dictionary<string, eCombatEffectType> type_dic = sEffectTypeDic;

            if (string.IsNullOrEmpty(type_info) || !type_dic.ContainsKey(type_info))
            {
                return eCombatEffectType.INVALID;
            }

            return type_dic[type_info];
        }

        public static CombatEffectEvent CreateEffectInstance(int effect_type)
        {
            Dictionary<int, System.Type> class_type_dic = sEffectClassDic;

            if (!class_type_dic.ContainsKey(effect_type))
            {
                return null;
            }

            System.Type class_type = class_type_dic[effect_type];
            CombatEffectEvent effect = System.Activator.CreateInstance(class_type) as CombatEffectEvent;
            return effect;
        }

        public static List<CombatEvent> ParseEffects(Hashtable info)
        {
            ArrayList effects_info = EB.Dot.Array("effects", info, null);
            if (effects_info == null)
            {
                EB.Debug.LogWarning("Parse effect failed, effects key not found");
                return new List<CombatEvent>();
            }

            List<CombatEvent> effects = new List<CombatEvent>();

            var enumerator = effects_info.GetEnumerator();
            while(enumerator.MoveNext())
            {
                Hashtable effect_info = enumerator.Current as Hashtable;
                CombatEffectEvent effect = ParseEffect(effect_info);
                if (effect == null)
                {
                    EB.Debug.LogError("Parse effect failed, {0}", EB.JSON.Stringify(effect_info));
                    return null;
                }

                effects.Add(effect);
            }

            return effects;
        }

        public static CombatEffectEvent ParseEffect(Hashtable info)
        {
            string type_info = EB.Dot.String("type", info, null);
            if (string.IsNullOrEmpty(type_info))
            {
                EB.Debug.LogError("ParseEffect: type is empty, {0}", EB.JSON.Stringify(info));
                return null;
            }
            eCombatEffectType effect_type = GetEffectType(type_info);
            if (effect_type == eCombatEffectType.INVALID)
            {
                EB.Debug.LogError("ParseEffect: invalid type, {0}", effect_type);
                return null;
            }

            CombatEffectEvent effect = CreateEffectInstance((int)effect_type);
            if (effect == null)
            {
                EB.Debug.LogError("ParseEffect: create instance failed, {0}", effect_type);
                return null;
            }

            if (!effect.Parse(info))
            {
                EB.Debug.LogError("ParseEffect: parse failed, {0}", EB.JSON.Stringify(info));
                return null;
            }

            return effect;
        }

        public static CombatantAttributes ParseCombatantAttributes(IDictionary info, CombatantAttributes fill)
        {
            if (fill == null)
            {
                fill = new CombatantAttributes();
            }

            var enumerator = sAttrTypeDic.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                if (!info.Contains(pair.Key))
                {
                    continue;
                }

                CombatantAttributes.eAttribute attr = pair.Value;
                switch (attr)
                {
                    case CombatantAttributes.eAttribute.CanNormalAttack:
                    case CombatantAttributes.eAttribute.CanRevive:
                    case CombatantAttributes.eAttribute.CanUseComboSkill:
                    case CombatantAttributes.eAttribute.CanUseManualSkill:
                    case CombatantAttributes.eAttribute.Slow:
                    case CombatantAttributes.eAttribute.FreezeHitReaction:
                        fill.SetBoolAttr(attr, bool.Parse(info[pair.Key].ToString()));
                        break;
                    default:
                        fill.SetIntAttr(attr, int.Parse(info[pair.Key].ToString()));
                        break;
                }
            }

            return fill;
        }

        public static CombatantData ParseCombatantData(IDictionary info, CombatantData fill)
        {
            if (fill == null)
            {
                fill = new CombatantData();
            }

            fill.IsPlayer = info.Contains("isUser") && info["isUser"] != null && bool.Parse(info["isUser"].ToString());
            fill.IsPlayerMirror = info.Contains("isUserMirror") && info["isUserMirror"] != null && bool.Parse(info["isUserMirror"].ToString());
            fill.PlayerId = info.Contains("uid") && info["uid"] != null ? long.Parse(info["uid"].ToString()) : 0;
            fill.IsEnemy = info.Contains("isEnemy") && info["isEnemy"] != null && bool.Parse(info["isEnemy"].ToString());
            fill.EnemyId = info.Contains("enemyId") && info["enemyId"] != null ? int.Parse(info["enemyId"].ToString()) : 0;
            fill.IsPlayerTroop = info.Contains("isPlayerTroop") && info["isPlayerTroop"] != null && bool.Parse(info["isPlayerTroop"].ToString());
            fill.IsPlayerTroopMirror = info.Contains("isPlayerTroopMirror") && info["isPlayerTroopMirror"] != null && bool.Parse(info["isPlayerTroopMirror"].ToString());
            fill.IsEnemyTroop = info.Contains("isEnemyTroop") && info["isEnemyTroop"] != null && bool.Parse(info["isEnemyTroop"].ToString());
            fill.TroopId = info.Contains("troopId") && info["troopId"] != null ? int.Parse(info["troopId"].ToString()) : 0;
            fill.Threaten = info.Contains("threaten") && bool.Parse(info["threaten"].ToString());
            fill.Level = info.Contains("level") && info["level"] != null ? int.Parse(info["level"].ToString()) : 0;
            fill.Name = info.Contains("name") ? info["name"].ToString() : string.Empty;
            fill.Model = info["model"].ToString();
            fill.Portrait = info["portrait"].ToString();
            fill.Position = info["pos"].ToString();
            fill.TplId = int.Parse(info["tplId"].ToString());
            fill.CharacterId = int.Parse(info["characterId"].ToString());
            fill.Index = CombatantIndex.Parse(info["index"] as IDictionary);

            ArrayList equipments = info.Contains("equipments") ? info["equipments"] as ArrayList : null;
            if (equipments != null)
            {
                fill.Equipments.Clear();

                var enumerator = equipments.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    IDictionary equip = enumerator.Current as IDictionary; 
                    string name = equip["name"].ToString();
                    string assetName = equip["assetName"].ToString();
                    fill.Equipments[name] = assetName;
                }
            }

            if (string.IsNullOrEmpty(fill.Name))
            {
                fill.Name = (string)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.CharacterTemplateManager", "Instance", "GetHeroName", fill.CharacterId);
            }

            return fill;
        }

        public static TeamData ParseTeamData(IDictionary info, TeamData fill)
        {
            if (fill == null)
            {
                fill = new TeamData();
            }

            fill.Index = TeamIndex.Parse(info["index"] as IDictionary);
            fill.Layout = info["layout"].ToString();
            fill.LeaderIndex = int.Parse(info["leader"].ToString());
            fill.TeamId = info.Contains("teamId") ? info["teamId"].ToString() : fill.TeamId;
            fill.Team = new List<CombatantData>();

            ArrayList team = info["team"] as ArrayList;

            var enumerator = team.GetEnumerator();
            while(enumerator.MoveNext())
            {
                IDictionary member = enumerator.Current as IDictionary;
                CombatantData combatant_data = ParseCombatantData(member, null);
                fill.Team.Add(combatant_data);
            }

            return fill;
        }

        public static SkillData ParseSkillData(IDictionary info, SkillData fill)
        {
            if (fill == null)
            {
                fill = new SkillData();
            }

            fill.CoolDown = int.Parse(info["cooldown"].ToString());
            fill.DeathStatus = info.Contains("deathStatus") && bool.Parse(info["deathStatus"].ToString());
            fill.EffectStatus = info.Contains("effectStatus") && bool.Parse(info["effectStatus"].ToString());
            fill.SkillId = int.Parse(info["id"].ToString());
            fill.Index = CombatantIndex.Parse(info["index"] as IDictionary);
            fill.Level = int.Parse(info["level"].ToString());

            ArrayList required = info.Contains("required") ? info["required"] as ArrayList : null;
            if (required != null)
            {
                var enumerator = required.GetEnumerator();
                while(enumerator.MoveNext())
                {
                    var idx = enumerator.Current;
                    fill.Required.Add(int.Parse(idx.ToString()));
                }
            }

            return fill;
        }

        public static ImpactData ParseImpactData(IDictionary info, ImpactData fill)
        {
            if (fill == null)
            {
                fill = new ImpactData();
            }

            fill.Fadeout = bool.Parse(info["fadeout"].ToString());
            fill.ImpactId = int.Parse(info["id"].ToString());
            fill.ImpactIndex = int.Parse(info["impact_index"].ToString());
            fill.Sender = CombatantIndex.Parse(info["sender"] as IDictionary);
            fill.SkillId = int.Parse(info["skill_id"].ToString());
            fill.TurnLeft = int.Parse(info["turn_left"].ToString());

            return fill;
        }

        public static eCombatHudMode ParseHudMode(string mode)
        {
            switch (mode)
            {
                case "autoInteract":
                    return eCombatHudMode.AutoInteract;
                case "onlyInteract":
                    return eCombatHudMode.OnlyInteract;
                default:
                    return eCombatHudMode.Normal;
            }
        }
    }
}