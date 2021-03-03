using ILRuntime.Runtime.GeneratedAdapter;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class CombatSkillEvent : CombatEvent
    {
        protected CombatantIndex m_sender = null;
        protected CombatantIndex m_trigger = null;
        protected CombatantIndex m_target = null;
        protected int m_skillId = -1;
        protected HashSet<CombatantIndex> m_targets = null;
        protected int m_hitTimes = -1;
        protected int m_totalHitTimes = -1;
        protected int m_totalDoubleHitCount = -1;
        protected int m_leftDoubleHitCount = -1;
        protected bool m_skip = false;
        protected int m_comboIndex = 0;

        public bool Skip
        {
            get { return m_skip; }
        }

        public CombatantIndex Sender
        {
            get { return m_sender; }
        }

        public CombatantIndex Trigger
        {
            get { return m_trigger; }
        }

        // Target may be null
        public CombatantIndex Target
        {
            get { return m_target; }
            set { m_target = value; }
        }

        public int SkillId
        {
            get { return m_skillId; }
            set { m_skillId = value; }
        }

        public HashSet<CombatantIndex> Targets
        {
            get { return m_targets; }
            set { m_targets = value; }
        }

        public int ComboIndex
        {
            get { return m_comboIndex; }
        }

        public int HitTimes
        {
            get { return m_hitTimes; }
            set { m_hitTimes = value; }
        }

        public int TotalHitTimes
        {
            get { return m_totalHitTimes; }
        }

        public int TotalDoubleHitCount
        {
            get { return m_totalDoubleHitCount; }
        }

        public int LeftDoubleHitCount
        {
            get { return m_leftDoubleHitCount; }
            set { m_leftDoubleHitCount = value; }
        }

        public bool IsCombo
        {
            get;
            set;
        }

        public float ComboDelay
        {
            get;
            private set;
        }

        public CombatSkillEvent()
        {

        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            IsCombo = m_timing != eCombatEventTiming.AUTO;
            ComboDelay = EB.Dot.Single("delay", info, 0.0f);

            // skip flag
            m_skip = EB.Dot.Bool("skip", info, false);

            m_sender = ParseSender(info);
            if (m_sender == null)
            {
                EB.Debug.LogWarning("CombatSkillEvent.Parse: sender is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            // may be empty
            m_trigger = ParseTrigger(info);
            m_target = ParseTarget(info);

            m_skillId = ParseSkillId(info);
            if (m_skillId < 0)
            {
                EB.Debug.LogWarning("CombatSkillEvent.Parse: skill is empty {0}", EB.JSON.Stringify(info));
                return false;
            }

            m_targets = ParseTargets(info);
            if (m_targets == null)
            {
                if (m_skip && m_target == null)
                {
                    m_targets = new HashSet<CombatantIndex>();
                }
                else
                {
                    EB.Debug.LogWarning("CombatSkillEvent.Parse: targets is empty {0}", EB.JSON.Stringify(info));
                    return false;
                }
            }

            m_comboIndex = EB.Dot.Integer("combo_index", info, m_comboIndex);

            m_children = CombatUtil.ParseEffects(info);
            if (m_children == null)
            {
                if (m_skip && m_target == null)
                {
                    m_children = new List<CombatEvent>();
                }
                else
                {
                    EB.Debug.LogWarning("CombatSkillEvent.Parse: effects is empty {0}", EB.JSON.Stringify(info));
                    return false;
                }
            }

            int len = m_children.Count;
            for (int i = 0; i < len; i++)
            {
                CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
                if (effect.Timing == eCombatEventTiming.AUTO)
                {
                    effect.Timing = eCombatEventTiming.RUNTIME;
                }

                if (effect.Sender == null)
                {
                    effect.Sender = new CombatantIndex(m_sender.TeamIndex, m_sender.IndexOnTeam);
                }
            }

            return true;
        }

        private List<CombatantIndex> cachedInvolved = null;
        public override List<CombatantIndex> GetInvolved()
        {
            if (cachedInvolved != null)
            {
                return cachedInvolved;
            }

            List<CombatantIndex> list = new List<CombatantIndex>();
            
            #region TODO: Parse函数永远没走过，m_sender永远是空的
            // if(m_sender != null){
            //     list.Add(m_sender);
            // }
            #endregion

            list.AddRange(m_targets);
            list.Sort((l,r)=>{
                if (l == null)
                {
                    return -1;
                }
                return l.CompareTo(r);
            });

            //去重
            for (int i = list.Count - 1; i > 0; --i)
            {
                if (list[i].Equals(list[i - 1]))
                {
                    list.RemoveAt(i);
                }
            }

            cachedInvolved = list;
            return list;
        }

        protected static CombatantIndex ParseSender(Hashtable info)
        {
            CombatantIndex sender = new CombatantIndex();
            sender.TeamIndex = EB.Dot.Integer("sender.team", info, -1);
            if (sender.TeamIndex < 0)
            {
                return null;
            }

            sender.IndexOnTeam = EB.Dot.Integer("sender.combatant", info, -1);
            if (sender.IndexOnTeam < 0)
            {
                return null;
            }

            return sender;
        }

        protected static CombatantIndex ParseTrigger(Hashtable info)
        {
            CombatantIndex trigger = new CombatantIndex();
            trigger.TeamIndex = EB.Dot.Integer("trigger.team", info, -1);
            if (trigger.TeamIndex < 0)
            {
                return null;
            }

            trigger.IndexOnTeam = EB.Dot.Integer("trigger.combatant", info, -1);
            if (trigger.IndexOnTeam < 0)
            {
                return null;
            }

            return trigger;
        }

        protected static CombatantIndex ParseTarget(Hashtable info)
        {
            CombatantIndex target = new CombatantIndex();
            target.TeamIndex = EB.Dot.Integer("target.team", info, -1);
            if (target.TeamIndex < 0)
            {
                return null;
            }

            target.IndexOnTeam = EB.Dot.Integer("target.combatant", info, -1);
            if (target.IndexOnTeam < 0)
            {
                return null;
            }

            return target;
        }

        protected static int ParseSkillId(Hashtable event_info)
        {
            int skill_id = EB.Dot.Integer("skill_id", event_info, -1);
            return skill_id;
        }

        protected static HashSet<CombatantIndex> ParseTargets(Hashtable info)
        {
            ArrayList targets_info = EB.Dot.Array("targets", info, null);
            if (targets_info == null)
            {
                return null;
            }

            if (targets_info.Count == 0)
            {
                return null;
            }

            HashSet<CombatantIndex> targets = new HashSet<CombatantIndex>();

            var enumerator = targets_info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Hashtable target_info = enumerator.Current as Hashtable;

                CombatantIndex target = new CombatantIndex();
                target.TeamIndex = EB.Dot.Integer("team", target_info, -1);
                if (target.TeamIndex < 0)
                {
                    return null;
                }

                target.IndexOnTeam = EB.Dot.Integer("combatant", target_info, -1);
                if (target.IndexOnTeam < 0)
                {
                    return null;
                }

                targets.Add(target);
            }

            return targets;
        }

        public void SetHitTimes(int times)
        {
            m_totalHitTimes = times;
            m_hitTimes = times;

            int len = m_children.Count;
            for (int i = 0; i < len; i++)
            {
                CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
                if (effect.EffectType != eCombatEffectType.REACTION)
                {
                    continue;
                }

                ReactionEffectEvent reaction = effect as ReactionEffectEvent;
                reaction.TotalTimes = m_hitTimes;
                reaction.Times = 0;
            }
        }

        public void SetDoubleHitCount(int count)
        {
            m_totalDoubleHitCount = count;
            m_leftDoubleHitCount = m_totalDoubleHitCount;
        }

        public ReactionEffectEvent FindComboTriggerReaction(CombatSkillEvent combo_skill)
        {
            bool res = (bool)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "IsComBo", combo_skill.SkillId);
            if (!res)
            {
                return null;
            }
            
            CombatantIndex trigger = combo_skill.Trigger;
            if (trigger == null)
            {
                return null;
            }

            int len = m_children.Count;
            for (int i = 0; i < len; i++)
            {
                CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
                if (effect.EffectType != eCombatEffectType.REACTION)
                {
                    continue;
                }

                if (!effect.Target.Equals(trigger))
                {
                    continue;
                }

                return effect as ReactionEffectEvent;
            }

            return null;
        }

        public int GetHitTargetCount()
        {
            int cnt = 0;
            int len = m_children.Count;
            for (int i = 0; i < len; i++)
            {
                CombatEffectEvent effect = m_children[i] as CombatEffectEvent;
                if (effect.EffectType != eCombatEffectType.REACTION)
                {
                    continue;
                }

                ReactionEffectEvent reaction = effect as ReactionEffectEvent;
                cnt += reaction.IsMiss ? 0 : 1;
            }
            return cnt;
        }
    }
}