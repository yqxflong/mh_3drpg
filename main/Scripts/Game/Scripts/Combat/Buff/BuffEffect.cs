using UnityEngine;
using System.Collections.Generic;
using System;
namespace Hotfix_LT.Combat
{
    public class BuffEffect
    {
        public BuffEffect()
        {

        }

        public BuffEffect(MoveEditor.BuffEventProperties props)
        {
            m_EventProps = props;
        }

        ~BuffEffect()
        {

        }

        public virtual void Update(Combatant cb)
        {

        }

        protected virtual void LerpIn(Combatant cb)
        {

        }

        protected virtual void LerpOut(Combatant cb)
        {

        }

        protected virtual void StartLerpIn(Combatant cb)
        {
            m_EffectLerpInStartTime = Time.time;
        }

        protected virtual void StartLerpOut(Combatant cb)
        {
            m_EffectLerpOutStartTime = Time.time;
        }

        public virtual void StartWork(Combatant cb, MoveEditor.BuffEventProperties bfp)
        {
            m_EventProps = bfp;
            m_EffectLerpInTotalTime = m_EventProps._lastFrame;
            m_EffectLerpOutTotalTime = m_EventProps._lastFrame;
            StartLerpIn(cb);
        }

        public virtual void StopWork(Combatant cb)
        {
            StartLerpOut(cb);
            m_EventProps = null;
        }

        public virtual bool IsLerpingInEffect()
        {
            return false;
        }

        public virtual bool IsLerpingOutEffect()
        {
            return false;
        }

        public bool IsBuffWork()
        {
            if (m_EventProps != null)
            {
                return true;
            }
            return false;
        }

        public virtual void MergeSelf(Combatant cb, MoveEditor.BuffEventProperties bfp)
        {
            //don't need to do anything when a same buff is applied twice.
        }

        public BuffEffectManager.BuffType Type
        {
            set { m_Type = value; }
            get { return m_Type; }
        }

        public MoveEditor.BuffEventProperties EventProps
        {
            get { return m_EventProps; }
        }

        public float EffectLerpInStartTime
        {
            get { return m_EffectLerpInStartTime; }
        }

        public float EffectLerpOutStartTime
        {
            get { return m_EffectLerpOutStartTime; }
        }

        public float EffectLerpOutTotalTime
        {
            set { m_EffectLerpOutTotalTime = value; }
            get { return m_EffectLerpOutTotalTime; }
        }

        protected BuffEffectManager.BuffType m_Type;

        protected MoveEditor.BuffEventProperties m_EventProps;

        protected float m_EffectLerpInStartTime = -1.0f;
        protected float m_EffectLerpInTotalTime = -1.0f;
        protected float m_EffectLerpOutStartTime = -1.0f;
        protected float m_EffectLerpOutTotalTime = 1.0f;

        protected bool m_IsBuffWork = false;
    }


    public static class BuffCreator
    {
        public static BuffEffect CreateBuffEffect(BuffEffectManager.BuffType type, MoveEditor.BuffEventProperties props)
        {
            switch (type)
            {
                case BuffEffectManager.BuffType.Stone:
                    return new StoneBuffEffect(props);
                default:
                    return null;
            }
        }
    }

    public class BuffEffectManager
    {
        public enum BuffType
        {
            InValid = -1,
            Stone,
            //add here
            TotalCount,
        }

        public class BuffTypeComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            public int GetHashCode(int obj)
            {
                return obj;
            }
        }

        private static BuffEffectManager s_instance = null;
        public static BuffEffectManager Instance
        {
            get { return s_instance = s_instance ?? new BuffEffectManager(); }
        }

        private BuffEffectManager()
        {

        }

        ~BuffEffectManager()
        {

        }

        public string GetJsonName(BuffType buff)
        {
            switch (buff)
            {
                case BuffType.InValid:
                    return "Invalid buff!!";
                case BuffType.Stone:
                    return "stoneGazeList";
                default:
                    break;
            }
            return "Invalid buff!!";
        }

        public BuffType GetType(string buff_name)
        {
            buff_name = buff_name.ToLower();

            switch (buff_name)
            {
                case "stone":
                    return BuffType.Stone;
                default:
                    return BuffType.InValid;
            }
        }
    }
}