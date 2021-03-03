using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Hotfix_LT.Combat
{
    public class ReactionEffectEvent : CombatEffectEvent, IHitQueueSplittable, IHitQueueHitable
    {
        protected int m_total_times = -1;
        protected int m_times = -1;

        protected int m_total_damage = -1;
        protected int m_damage = -1;

        protected int m_total_show = -1;
        protected int m_show = -1;

        protected int m_shield = -1;

        protected MoveEditor.PlayHitReactionProperties.eReactionType m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.Hit;
        protected bool m_immune_reaction_effect = false;

        protected bool m_miss = false;
        protected bool m_critical = false;
        protected bool m_absorb = false;

        protected bool m_will_be_combo = false;

        protected bool m_play_state = false;

        public int TotalTimes
        {
            get { return m_total_times; }
            set { m_total_times = value > 1 && IsMiss ? 1 : value; }
        }

        public int Times
        {
            get { return m_times; }
            set { m_times = value; }
        }

        public int TotalDamage
        {
            get { return m_total_damage; }
            set { m_total_damage = value; }
        }

        public int Damage
        {
            get { return m_damage; }
            set { m_damage = value; }
        }

        public int TotalShow
        {
            get { return m_total_show; }
            set { m_total_show = value; }
        }

        public int Shield
        {
            get { return m_shield; }
            set { m_shield = value; }
        }

        public int Show
        {
            get { return m_show; }
            set { m_show = value; }
        }

        public MoveEditor.PlayHitReactionProperties.eReactionType Reaction
        {
            get { return m_reaction_type; }
            set { m_reaction_type = value; }
        }

        public bool IsMiss
        {
            get { return m_miss; }
            set { m_miss = value; }
        }

        public bool IsAbsorb
        {
            get { return m_absorb; }
        }

        public bool IsCritical
        {
            get { return m_critical; }
            set { m_critical = value; }
        }

        public bool IsBlocked
        {
            get { return Damage <= 0 && !IsMiss && !IsAbsorb; }
        }

        public bool EffectPlayed
        {
            get { return m_play_state; }
            set { m_play_state = value; }
        }

        public bool PlayEffect
        {
            get { return m_play_state; }
            set { m_play_state = value; }
        }

        public bool WillBeCombo
        {
            get { return m_will_be_combo; }
            set { m_will_be_combo = value; }
        }

        public bool ImmuneReactionEffect
        {
            get { return m_immune_reaction_effect; }
            set { m_immune_reaction_effect = value; }
        }

        public ReactionEffectEvent()
        {
            m_effect_type = eCombatEffectType.REACTION;
        }

        public override bool Parse(Hashtable info)
        {
            if (!base.Parse(info))
            {
                return false;
            }

            m_total_damage = EB.Dot.Integer("damage", info, 0);
            m_damage = m_total_damage;

            m_total_show = EB.Dot.Integer("show", info, -1);
            if (m_total_show < 0)
            {
                m_total_show = m_total_damage;
            }
            m_show = m_total_show;

            m_shield = EB.Dot.Integer("shield", info, 0);

            string action = EB.Dot.String("action", info, "");

            if (m_damage == 0 && action.CompareTo("block") == 0)
            {
                m_miss = true;
            }

            if (m_damage == 0 && m_shield > 0)
            {
                m_absorb = true;
            }

            m_critical = EB.Dot.Bool("critical", info, false);

            if (action == "beat_back")
            {
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.Back;
            }
            else if (action == "beat_down")
            {
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.Down;
            }
            else if (action == "little_flow")
            {
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.LittleFlow;
            }
            else if (action == "large_flow")
            {
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.LargeFlow;
            }
            else if (action == "block")
            {
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.Block;
            }
            else if (action == "hit")
            {
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.Hit;
            }
            else
            {
                EB.Debug.LogWarning("ReactionEffectEvent.Parse: invalid action = {0}", action);
                m_reaction_type = MoveEditor.PlayHitReactionProperties.eReactionType.Hit;
            }

            m_immune_reaction_effect = EB.Dot.Bool("immuneReactionEffect", info, false);

            return true;
        }

        public CombatEffectEvent Split(float rating)
        {
            if (m_timing != eCombatEventTiming.HIT_QUEUE)
            {
                return null;
            }

            if (m_times >= m_total_times)
            {
                return null;
            }

            m_times++;

            ReactionEffectEvent reaction = new ReactionEffectEvent();

            reaction.m_type = m_type;
            reaction.m_timing = eCombatEventTiming.ON_HIT;

            reaction.m_effect_type = m_effect_type;
            reaction.m_sender = m_sender;
            reaction.m_target = m_target;
            reaction.m_parent = -1;

            reaction.m_children = new List<CombatEvent>();

            reaction.m_total_times = m_total_times;
            reaction.m_times = m_times;
            reaction.m_total_damage = m_total_damage;
            // ignore m_damage
            reaction.m_total_show = m_total_show;
            // ignore m_show
            reaction.m_shield = m_shield;

            reaction.m_critical = m_critical;
            reaction.m_miss = m_miss;

            reaction.m_reaction_type = m_reaction_type;
            reaction.m_immune_reaction_effect = m_immune_reaction_effect;

            reaction.m_will_be_combo = m_will_be_combo;
            reaction.m_play_state = m_play_state;

            if (reaction.IsFirst())
            {
                reaction.m_absorb = m_absorb;
            }

            // calculate damage
            if (reaction.IsLast())
            {
                reaction.Damage = Damage;
                reaction.Show = Show;
            }
            else
            {
                reaction.Damage = Mathf.CeilToInt(TotalDamage * rating);
                reaction.Show = Mathf.CeilToInt(TotalShow * rating);
            }
            Damage -= reaction.Damage;
            Show -= reaction.Show;

            return reaction;
        }

        public bool IsFirst()
        {
            return m_times == 1;
        }

        public bool IsLast()
        {
            return m_times == m_total_times;
        }

        public List<eCombatEventTiming> TransferChildren()
        {
            List<eCombatEventTiming> translate_list = new List<eCombatEventTiming>();
            if (IsFirst())
            {
                translate_list.Add(eCombatEventTiming.ON_START);
                translate_list.Add(eCombatEventTiming.ON_REACTION_START);
                translate_list.Add(eCombatEventTiming.ON_REACTION_END);
            }

            if (IsLast())
            {
                translate_list.Add(eCombatEventTiming.ON_END);
            }

            if (Reaction == MoveEditor.PlayHitReactionProperties.eReactionType.Back)
            {
                translate_list.Add(eCombatEventTiming.COLLIDE_QUEUE);
            }

            return translate_list;
        }

        public void Hit(bool use_effect_action, MoveEditor.PlayHitReactionProperties.eReactionType default_action)
        {
            // ensure effect action will play
            if (!use_effect_action && IsLast() && !EffectPlayed)
            {
                use_effect_action = true;
            }

            // translate combo state to current_reaction, only enable combo once
            PlayEffect = !EffectPlayed && use_effect_action;

            // calculate action            
            if (!use_effect_action)
            {
                Reaction = default_action;
            }
        }
    }
}