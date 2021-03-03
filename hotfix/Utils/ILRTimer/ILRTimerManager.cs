//ILRTimerManager
//ILR层的Timer
//Johny

using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT
{
    public class ILRTimerManager
    {
        //Timer类型
        private enum enTimerType
        {
            Normal,
            LateUpdate,
        };

        //Timer List
        private List<ILRCTimer>[] m_timers;
        private int m_timerSequence;

        static ILRTimerManager instanceInternal = null;
        public static ILRTimerManager instance
        {
            get
            {
                if (instanceInternal == null)
                {
                    instanceInternal = new ILRTimerManager();
                }

                return instanceInternal;
            }
        }

        public ILRTimerManager()
        {
            Init();
        }
        
        //----------------------------------------------
        /// 初始化
        //----------------------------------------------
        public void Init()
        {
            m_timers = new List<ILRCTimer>[System.Enum.GetValues(typeof(enTimerType)).Length];

            for (int i = 0; i < m_timers.Length; i++)
            {
                m_timers[i] = new List<ILRCTimer>();
            }

            m_timerSequence = 0;
        }

        //----------------------------------------------
        /// Update
        /// @这里只更新Normal类型的Timer
        //----------------------------------------------
        public void Update()
        {
            UpdateTimer((int)(Time.deltaTime * 1000), enTimerType.Normal);
        }

        public void LateUpdate()
        {
            UpdateTimer((int)(Time.deltaTime * 1000), enTimerType.LateUpdate);
        }

        //----------------------------------------------
        /// UpdateTimer
        /// @delata
        /// @timerType
        //----------------------------------------------
        private void UpdateTimer(int delta, enTimerType timerType)
        {
            List<ILRCTimer> timers = m_timers[(int)timerType];

            for (int i = 0; i < timers.Count;)
            {
                if (timers[i].IsFinished())
                {
                    timers.RemoveAt(i);
                    continue;
                }

                timers[i].Update(delta);
                i++;
            }
        }

        //----------------------------------------------
        /// 添加Timer
        /// @time               : 计时时间(ms)
        /// @loop               : 循环次数(0: forever)
        /// @onTimeUpHandler    : 时间到时的回调函数
        /// @return sequence of timer
        //----------------------------------------------
        public int AddTimer(int time, int loop, ILRCTimer.OnTimeUpHandler onTimeUpHandler)
        {
            return AddTimer(time, loop, onTimeUpHandler, false, null);
        }

        public int AddLateUpdateTimer(int time, int loop, ILRCTimer.OnTimeUpHandler onTimeUpHandler)
        {
            return _AddTimer(time, loop, onTimeUpHandler, enTimerType.LateUpdate, null);
        }
        //----------------------------------------------
        /// 添加Timer
        /// @time               : 计时时间(ms)
        /// @loop               : 循环次数
        /// @onTimeUpHandler    : 时间到时的回调函数
        /// @useFrameSync       : 是否使用桢同步
        /// @return sequence of timer
        //----------------------------------------------
        private int AddTimer(int time, int loop, ILRCTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync, object arg)
        {
            return _AddTimer(time, loop, onTimeUpHandler, enTimerType.Normal, arg);
        }

        private int _AddTimer(int time, int loop, ILRCTimer.OnTimeUpHandler onTimeUpHandler, enTimerType type, object arg = null)
        {
            m_timerSequence++;

            if (Application.isPlaying)
            {
                var timer = new ILRCTimer(ILRCTimer.TimerType.timer);
                timer.Arg = arg;
                timer.ResetWith(time, loop, onTimeUpHandler, m_timerSequence);

                m_timers[(int)type].Add(timer);
            }

            return m_timerSequence;
        }

        //----------------------------------------------
        /// 移除Timer
        /// @sequence
        //----------------------------------------------
        public void RemoveTimer(int sequence)
        {
            for (int i = 0; i < m_timers.Length; i++)
            {
                List<ILRCTimer> timers = m_timers[i];

                for (int j = 0; j < timers.Count; j++)
                {
                    if (timers[j].IsSequenceMatched(sequence))
                    {
                        timers[j].Finish();
                        return;
                    }
                }
            }
        }

        //----------------------------------------------
        /// 移除Timer
        /// @sequence: ref，移除后清空
        //----------------------------------------------
        public void RemoveTimerSafely(ref int sequence)
        {
            if (sequence != 0)
            {
                RemoveTimer(sequence);
                sequence = 0;
            }
        }

        //----------------------------------------------
        /// 暂停Timer
        /// @sequence
        //----------------------------------------------
        public void PauseTimer(int sequence)
        {
            ILRCTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.Pause();
            }
        }

        //----------------------------------------------
        /// 恢复Timer
        /// @sequence
        //----------------------------------------------
        public void ResumeTimer(int sequence)
        {
            ILRCTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.Resume();
            }
        }

        //----------------------------------------------
        /// 重置Timer
        /// @sequence
        //----------------------------------------------
        public void ResetTimer(int sequence)
        {
            ILRCTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.Reset();
            }
        }

        //----------------------------------------------
        /// 重设Timer倒计时
        /// @sequence
        /// @totalTime
        //----------------------------------------------
        public void ResetTimerTotalTime(int sequence, int totalTime)
        {
            ILRCTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                timer.ResetTotalTime(totalTime);
            }
        }

        //----------------------------------------------
        /// 获取Timer的当前时间
        /// @sequence
        //----------------------------------------------
        public int GetTimerCurrent(int sequence)
        {
            ILRCTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                return timer.CurrentTime;
            }

            return -1;
        }

        //--------------------------------------
        /// 还差多少时间
        //--------------------------------------
        public int GetLeftTime(int sequence)
        {
            ILRCTimer timer = GetTimer(sequence);

            if (timer != null)
            {
                return timer.GetLeftTime() / 1000;  //转成多少秒
            }

            return -1;
        }

        //----------------------------------------------
        /// 返回指定sequence的Timer
        //----------------------------------------------
        public ILRCTimer GetTimer(int sequence)
        {
            for (int i = 0; i < m_timers.Length; i++)
            {
                List<ILRCTimer> timers = m_timers[i];

                for (int j = 0; j < timers.Count; j++)
                {
                    if (timers[j].IsSequenceMatched(sequence))
                    {
                        return timers[j];
                    }
                }
            }

            return null;
        }

        //----------------------------------------------
        /// 移除Timer
        /// @onTimeUpHandler
        //----------------------------------------------
        public void RemoveTimer(ILRCTimer.OnTimeUpHandler onTimeUpHandler)
        {
            List<ILRCTimer> timers = m_timers[(int)(enTimerType.Normal)];

            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].IsDelegateMatched(onTimeUpHandler))
                {
                    timers[i].Finish();
                    continue;
                }
            }
        }

        //----------------------------------------------
        /// 移除Timer
        /// @onTimeUpHandler
        //----------------------------------------------
        public void RemoveTimer(ILRCTimer.OnTimeUpArgHandler onTimeUpHandler)
        {
            List<ILRCTimer> timers = m_timers[(int)(enTimerType.Normal)];

            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].IsDelegateMatched(onTimeUpHandler))
                {
                    timers[i].Finish();
                    continue;
                }
            }
        }

        //----------------------------------------------
        /// 移除所有Timer
        //----------------------------------------------
        public void RemoveAllTimer()
        {
            for (int i = 0; i < m_timers.Length; i++)
            {
                var timers = m_timers[i];
                timers.Clear();
            }
        }
    }

}