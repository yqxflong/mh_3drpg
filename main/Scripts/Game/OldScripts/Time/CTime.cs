
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CTimer
{
    public enum TimerType
    {
        timer,
        framer,
    }

    //delegate
    public delegate void OnTimeUpHandler(int timerSequence);
    private OnTimeUpHandler m_timeUpHandler;

    public delegate void OnTimeUpArgHandler(int timerSequence, object arg);
    private OnTimeUpArgHandler m_timeUpArgHandler;

    //循环次数( < 0 表示无限循环)
    private int m_loop = 1;

    //计时(ms)
    private int m_totalTime;
    private int m_currentTime;

    //是否完成
    private bool m_isFinished;

    //是否处于运行状态
    private bool m_isRunning;

    //序列号
    private int m_sequence;

    private TimerType mTimerType;

    public object Arg;

    public int CurrentTime
    {
        get { return m_currentTime; }
    }

    void ResetTimer()
    {
        m_timeUpHandler = null;
        m_timeUpArgHandler = null;
        m_loop = 1;
        m_totalTime = 0;
        m_currentTime = 0;
        m_isFinished = false;
        m_isRunning = false;
        m_sequence = 0;
    }

    public void OnRelease()
    {
        ResetTimer();
    }

    public void OnUse()
    {
        ResetTimer();
    }

    //--------------------------------------
    /// 构造函数
    //--------------------------------------
    public void ResetWith(int time, int loop, OnTimeUpHandler timeUpHandler, int sequence)
    {
        if (loop == 0)
        {
            loop = -1;
        }

        m_currentTime = 0;
        m_totalTime = time;
        m_loop = loop;
        m_timeUpHandler = timeUpHandler;
        m_sequence = sequence;

        m_currentTime = 0;
        m_isRunning = true;
        m_isFinished = false;
    }
    public void ResetWith(int time, int loop, OnTimeUpArgHandler timeUpArgHandler, int sequence)
    {
        if (loop == 0)
        {
            loop = -1;
        }

        m_currentTime = 0;
        m_totalTime = time;
        m_loop = loop;
        m_timeUpArgHandler = timeUpArgHandler;
        m_sequence = sequence;

        m_currentTime = 0;
        m_isRunning = true;
        m_isFinished = false;
    }

    public CTimer(TimerType timerType)
    {
        mTimerType = timerType;
        ResetTimer();
    }

    //--------------------------------------
    /// Update
    /// @deltaTime
    //--------------------------------------
    public void Update(int deltaTime)
    {
        if (m_isFinished || !m_isRunning)
        {
            return;
        }

        if (m_loop == 0)
        {
            m_isFinished = true;
        }
        else
        {
            if (mTimerType == TimerType.timer)
            {
                m_currentTime += deltaTime;
            }
            else if (mTimerType == TimerType.framer)
            {
                m_currentTime++;
            }

            if (m_currentTime >= m_totalTime)
            {
                if (m_timeUpHandler != null)
                {
                    m_timeUpHandler(m_sequence);
                }
                else if (m_timeUpArgHandler != null)
                {
                    m_timeUpArgHandler(m_sequence, Arg);
                }

                m_currentTime = 0;
                m_loop--;
            }
        }
    }

    //--------------------------------------
    /// 还差多少时间
    //--------------------------------------
    public int GetLeftTime()
    {
        return m_totalTime - m_currentTime;
    }

    //--------------------------------------
    /// 结束Timer
    //--------------------------------------
    public void Finish()
    {
        m_isFinished = true;
    }

    //--------------------------------------
    /// 是否完成
    //--------------------------------------
    public bool IsFinished()
    {
        return m_isFinished;
    }

    /// <summary>
    /// Loops是否结束
    /// </summary>
    /// <returns></returns>
    public bool LoopOver()
    {
        return m_loop == 1;
    }

    //--------------------------------------
    /// 暂停
    //--------------------------------------
    public void Pause()
    {
        m_isRunning = false;
    }

    //--------------------------------------
    /// 恢复
    //--------------------------------------
    public void Resume()
    {
        m_isRunning = true;
    }

    //--------------------------------------
    /// 重置
    //--------------------------------------
    public void Reset()
    {
        m_currentTime = 0;
    }

    //--------------------------------------
    /// 重设倒计时
    //--------------------------------------
    public void ResetTotalTime(int totalTime)
    {
        if (m_totalTime == totalTime) return;
        m_currentTime = 0;
        m_totalTime = totalTime;
    }

    //--------------------------------------
    /// 检查sequence是否匹配
    //--------------------------------------
    public bool IsSequenceMatched(int sequence)
    {
        return (m_sequence == sequence);
    }

    //--------------------------------------
    /// 检查delegate是否匹配
    //--------------------------------------
    public bool IsDelegateMatched(OnTimeUpHandler timeUpHandler)
    {
        return (m_timeUpHandler == timeUpHandler);
    }
    public bool IsDelegateMatched(OnTimeUpArgHandler timeUpHandler)
    {
        return (m_timeUpArgHandler == timeUpHandler);
    }
    
};