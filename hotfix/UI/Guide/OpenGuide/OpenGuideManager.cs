using UnityEngine;
using System.Collections.Generic;

namespace Hotfix_LT.UI
{
    public class OpenGuideManager
    {
        private static OpenGuideManager m_Instance;
        public static OpenGuideManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new OpenGuideManager();
                }
                return m_Instance;
            }
        }

        private int m_StepProgress;
        private int m_StepCurrentId;
        public int StepCurrentId
        {
            get
            {
                return m_StepCurrentId;
            }
        }
        private bool m_GuideState = false;
        public bool GuideState
        {
            get
            {
                return m_GuideState;
            }
        }
        private eStepExcuteState m_StepExcuteState;
        private EB.Collections.Stack<object> m_StepStack = new EB.Collections.Stack<object>();
        private List<GameObject> m_StepFxs = new List<GameObject>();
        private List<OpenGuideStep> m_Steps = new List<OpenGuideStep>();
        private Transform m_CurrentFrame;
        private int m_RemoteProgressId;
        
        public void InitGuideState()
        {
            if (GuideManager.Instance.GuideState)
            {
                m_GuideState = false;
            }
            m_StepExcuteState = eStepExcuteState.Excuting;
            m_StepStack.Clear();
            int progressid;
            int local_progressid = PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value + "_progress_id", -1);
            if (local_progressid != -1) progressid = local_progressid;
            else
            {
                if (!DataLookupsCache.Instance.SearchIntByID("guide_progress", out progressid)) progressid = 0;
            }
            m_RemoteProgressId = progressid;
            if (0 == progressid)
            {
                m_StepCurrentId = 0;
                m_StepProgress = GuideManager.Instance.m_StepEndId;
                m_GuideState = true;
            }
            else
            {
                GuideStepData step;
                if (!GuideManager.Instance.GetStep(progressid, out step)) m_GuideState = false;
                else
                {
                    GuideStepData next_step;
                    if (!GuideManager.Instance.GetStep(step.next_id, out next_step))
                    {
                        m_GuideState = false;
                        return;
                    }
                    if (step.type == 1)
                    {
                        if (next_step.type == 0)// 接下来是开放式引导了
                        {
                            m_GuideState = true;
                            m_StepCurrentId = 0;
                            m_StepProgress = progressid;
                        }
                    }
                    else
                    {
                        m_GuideState = true;
                        m_StepCurrentId = 0;
                        m_StepProgress = progressid;
                    }
                }
            }
        }

        public bool HasGuidNow()
        {
            if (StepCurrentId > 0) return true;
            FindCouldStep();
            if (StepCurrentId > 0) return true;
            return false;
        }

        public bool ExcuteExReal(Transform Frame, System.Action onFinish)
        {
            bool started = false;
            if (HasGuidNow())
            {
                GuideStepData now;
                if (!GuideManager.Instance.GetStep(m_StepCurrentId, out now))
                {
                    return false;
                }
                if (!Frame.name.Contains(now.view)) return false;
                if (string.IsNullOrEmpty(now.target_path))
                {
                    //if (now.guide_id == now.rollback_id) m_StepStack.Push(now);
                    if (m_StepStack.Count > 0) m_StepStack.Pop();
                    m_StepStack.Push(now);
                    ExcuteCurrentStep(null, null, onFinish);
                }
                else
                {
                    Transform target = null;
                    target = Frame.Find(now.target_path);
                    if (target == null)
                    {
                        EB.Debug.LogWarning("Cant find obj for{0} for {1}" , now.target_path , m_StepCurrentId);
                        return false;
                    }
                    Transform target_panel = Frame.parent.Find(now.parameter);
                    //if(now.guide_id==now.rollback_id)m_StepStack.Push(now);
                    if (m_StepStack.Count > 0) m_StepStack.Pop();
                    m_StepStack.Push(now);
                    m_CurrentFrame = Frame;
                    ExcuteCurrentStep(target.gameObject, target_panel != null ? target_panel.gameObject : null, onFinish);
                }
                started = true;
            }
            return started;
        }

        public bool ExcuteEx(Transform Frame, System.Action onFinish)
        {
            if (m_GuideState == false) return false;
            InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.1f);//防止连续的点击
            PopFrameGuideStackEx();
            return ExcuteExReal(Frame, onFinish);
        }

        public OpenGuideStep ExcuteCurrentStep(GameObject stepObj, GameObject panel, System.Action onFinish)
        {
            GuideStepData now;
            if (!GuideManager.Instance.GetStep(m_StepCurrentId, out now)) return null;
            if (m_StepExcuteState != eStepExcuteState.Excuting) return null;
            m_StepExcuteState = eStepExcuteState.Excuted;
            string excute_type = now.excute_type.ToString();
            if (excute_type.Equals("default"))
            {
                OpenGuideStep step = new OpenGuideStep();
                step.GuideId = m_StepCurrentId;
                step.m_Panel = panel;
                step.m_StepObj = stepObj;
                if (onFinish != null) step.onFinish += onFinish;
                step.ExcuteReal();
                return step;
            }
            else
            {
                System.Type type = System.Type.GetType(excute_type);
                object obj = System.Activator.CreateInstance(type);
                OpenGuideStep step = (OpenGuideStep)obj;
                step.GuideId = m_StepCurrentId;
                step.m_StepObj = stepObj;
                step.m_Panel = panel;
                if (onFinish != null) step.onFinish += onFinish;
                step.ExcuteReal();
                return step;
            }
        }


        //完成当前步骤
        public void FinishStep()
        {
            EB.Debug.Log("=============FinishStep{0}" , m_StepCurrentId);
            m_StepCurrentId = 0;
            if (!m_GuideState) return;
            GuideStepData now;
            if (!GuideManager.Instance.GetStep(m_StepProgress, out now))
            {
                m_StepCurrentId = 0;
                return;
            }
            if (now.next_id == 0)
                m_GuideState = false;

            m_StepExcuteState = eStepExcuteState.Finished;
            //判定条件 设置m_StepCurrentId m_StepProgress
            FindCouldStep();
            //获取到了下一个步骤
            if (m_StepCurrentId > 0)
            {
                GuideStepData next;
                if (GuideManager.Instance.GetStep(m_StepCurrentId, out next))
                {
                    //判定执行类型  立即执行，还是触发执行
                    string trigger_type = next.trigger_type;
                    if (trigger_type.Equals("now")) ExcuteEx(m_CurrentFrame.transform, null);
                    else if (trigger_type.Equals("late_1")) return;
                    else if (trigger_type.Equals("late_2"))
                    {
                        MengBanController.Instance.FobiddenAll();
                        return;
                    }
                    else EB.Debug.LogError("trigger_type undefine for {0}" , trigger_type);
                }
            }
        }

        /// <summary>
        /// 判定是否有满足条件的步骤   在一个步骤完成 或者 条件满足时激发
        /// </summary>
        public void FindCouldStep()
        {
            if (m_StepCurrentId > 0) return;
            if (m_StepProgress == 0)
            {
            }
            else
            {
                //判定条件 设置m_StepCurrentId
                GuideStepData next;
                if (GuideManager.Instance.GetNextStep(m_StepProgress, out next))
                {
                    if (m_RemoteProgressId != m_StepProgress && next.guide_id == next.rollback_id)
                    {
                        //将stack弹出 
                        m_StepStack.Clear();
                        RemoveFxs();
                        m_RemoteProgressId = m_StepProgress;
                        m_StepExcuteState = eStepExcuteState.Excuting;
                    }
                    if (next.EnoughCondition())
                    {
                        m_StepCurrentId = next.guide_id;
                        m_StepProgress = m_StepCurrentId;
                        m_StepExcuteState = eStepExcuteState.Excuting;
                    }
                }
                else
                {
                    //将stack弹出 
                    m_StepStack.Clear();
                    m_RemoteProgressId = m_StepProgress;
                    m_GuideState = false;
                }
            }
        }

        public void AddFx(GameObject fx, OpenGuideStep step)
        {
            m_StepFxs.Add(fx);
            m_Steps.Add(step);
        }

        public void RemoveFxs()
        {
            for (int i = 0; i < m_StepFxs.Count; i++)
            {
                if (m_StepFxs[i] != null)
                {
                    GameObject.Destroy(m_StepFxs[i]);
                }
            }
            m_StepFxs.Clear();
            for (int i = 0; i < m_Steps.Count; i++)
            {
                if (m_Steps[i] != null)
                {
                    m_Steps[i].RemoveDelegate();
                }
            }
            m_Steps.Clear();
        }

        public void PopFrameGuideStackEx()
        {
            if (m_StepStack.Count == 0) return;
            if (m_StepExcuteState == eStepExcuteState.Excuted)
            {
                GuideStepData step = (GuideStepData)m_StepStack.Peek();
                m_StepCurrentId = step.rollback_id;
                m_StepProgress = m_StepCurrentId;
                m_StepExcuteState = eStepExcuteState.Excuting;
                while (m_StepStack.Count > 0)
                {
                    m_StepStack.Pop();
                }
                RemoveFxs();
            }
        }

        public void PopFrameGuideStackExForce()
        {
            if (m_StepStack.Count == 0) return;
            if (GuideManager.Instance.GuideState) return;
            if (m_StepExcuteState == eStepExcuteState.Excuted || m_StepExcuteState == eStepExcuteState.Excuting)
            {
                GuideStepData step = (GuideStepData)m_StepStack.Peek();
                m_StepCurrentId = step.rollback_id;
                m_StepProgress = m_StepCurrentId;
                m_StepExcuteState = eStepExcuteState.Excuting;
                while (m_StepStack.Count > 0)
                {
                    m_StepStack.Pop();
                }
                RemoveFxs();
            }
        }

        public bool IsGuideReady()
        {
            return m_StepCurrentId != 0;
        }
    }
}
