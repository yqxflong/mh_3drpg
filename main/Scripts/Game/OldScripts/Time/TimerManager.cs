
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager
{
    //Timer类型
    private enum enTimerType
    {
        Normal,
        LateUpdate,
    };

    //Timer List
    private List<CTimer>[] m_timers;
    private int m_timerSequence;

    static TimerManager instanceInternal = null;
    public static TimerManager instance
    {
        get
        {
            if (instanceInternal == null)
            {
                instanceInternal = new TimerManager();
            }

            return instanceInternal;
        }
    }
    
    /// <summary>
    /// 是否到了结束时间
    /// </summary>
    /// <param name="currentTime"></param>
    /// <param name="intervalTime"></param>
    /// <returns></returns>
    public bool IsTimeUp(ref float currentTime, float intervalTime)
    {
        currentTime += Time.deltaTime;
        if (currentTime < intervalTime)
        {
            return false;
        }
        currentTime = 0;
        return true;
    }

    //----------------------------------------------
    /// 初始化
    //----------------------------------------------
    public void Init()
    {
        m_timers = new List<CTimer>[System.Enum.GetValues(typeof(enTimerType)).Length];

        for (int i = 0; i < m_timers.Length; i++)
        {
            m_timers[i] = new List<CTimer>();
        }

        m_timerSequence = 0;
    }

    //----------------------------------------------
    /// Update
    /// @这里只更新Normal类型的Timer
    //----------------------------------------------
    public void Update()
    {
        if (m_timers == null)
        {
            Init();
        }
        UpdateTimer((int)(Time.deltaTime * 1000), enTimerType.Normal);
    }

    public void LateUpdate()
    {
        if (m_timers == null)
        {
            Init();
        }
        UpdateTimer((int)(Time.deltaTime * 1000), enTimerType.LateUpdate);
    }

    //----------------------------------------------
    /// UpdateTimer
    /// @delata
    /// @timerType
    //----------------------------------------------
    private void UpdateTimer(int delta, enTimerType timerType)
    {
        List<CTimer> timers = m_timers[(int)timerType];

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
    /// @loop               : 循环次数
    /// @onTimeUpHandler    : 时间到时的回调函数
    /// @return sequence of timer
    //----------------------------------------------
    public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler)
    {
        return AddTimer(time, loop, onTimeUpHandler, false, null);
    }

    public int AddLateUpdateTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler)
    {
        return _AddTimer(time, loop, onTimeUpHandler, enTimerType.LateUpdate, null);
    }

    public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, object arg)
    {
        return AddTimer(time, loop, onTimeUpHandler, false, arg);
    }

    public int AddLateUpdateTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, object arg)
    {
        return _AddTimer(time, loop, onTimeUpHandler, enTimerType.LateUpdate, arg);
    }


    //Timer List
    public int AddFramer(int time, int loop, CTimer.OnTimeUpHandler onFrameUpHandler)
    {
        m_timerSequence++;

        if (Application.isPlaying)
        {
            if (m_timers.Length > 0 && m_timers[0] != null)
            {
                var timer = new CTimer(CTimer.TimerType.framer);
                timer.ResetWith(time, loop, onFrameUpHandler, m_timerSequence);

                m_timers[0].Add(timer);
            }
        }

        return m_timerSequence;
    }
    //Timer List
    public int AddFramer(int time, int loop, CTimer.OnTimeUpArgHandler onFrameUpHandler, object arg)
    {
        m_timerSequence++;
        if (Application.isPlaying)
        {
            if (m_timers == null)
            {
                Init();
            }
            if (m_timers.Length > 0 && m_timers[0] != null)
            {
                var timer = new CTimer(CTimer.TimerType.framer);
                timer.Arg = arg;
                timer.ResetWith(time, loop, onFrameUpHandler, m_timerSequence);
                m_timers[0].Add(timer);
            }
        }

        return m_timerSequence;
    }

    //----------------------------------------------
    /// 添加Timer
    /// @time               : 计时时间(ms)
    /// @loop               : 循环次数
    /// @onTimeUpHandler    : 时间到时的回调函数
    /// @useFrameSync       : 是否使用桢同步
    /// @return sequence of timer
    //----------------------------------------------
    public int AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, bool useFrameSync, object arg)
    {
        return _AddTimer(time, loop, onTimeUpHandler, enTimerType.Normal, arg);
    }

    int _AddTimer(int time, int loop, CTimer.OnTimeUpHandler onTimeUpHandler, enTimerType type, object arg = null)
    {
        m_timerSequence++;

        if (Application.isPlaying)
        {
            var timer = new CTimer(CTimer.TimerType.timer);
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
            List<CTimer> timers = m_timers[i];

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
        CTimer timer = GetTimer(sequence);

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
        CTimer timer = GetTimer(sequence);

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
        CTimer timer = GetTimer(sequence);

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
        CTimer timer = GetTimer(sequence);

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
        CTimer timer = GetTimer(sequence);

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
        CTimer timer = GetTimer(sequence);

        if (timer != null)
        {
            return timer.GetLeftTime() / 1000;  //转成多少秒
        }

        return -1;
    }

    //----------------------------------------------
    /// 返回指定sequence的Timer
    //----------------------------------------------
    public CTimer GetTimer(int sequence)
    {
        for (int i = 0; i < m_timers.Length; i++)
        {
            List<CTimer> timers = m_timers[i];

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
    public void RemoveTimer(CTimer.OnTimeUpHandler onTimeUpHandler)
    {
        List<CTimer> timers = m_timers[(int)(enTimerType.Normal)];

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
    public void RemoveTimer(CTimer.OnTimeUpArgHandler onTimeUpHandler)
    {
        List<CTimer> timers = m_timers[(int)(enTimerType.Normal)];

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
};