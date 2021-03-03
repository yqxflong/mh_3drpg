using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Hotfix_LT.UI
{
	public enum eStepExcuteState
	{
		Excuting = 0,
		Excuted = 1,
		Finished = 2
	}

	public class GuideManager
	{

		#region Public Mem
		public int END_LEVEL = 1;
		public int m_StepEndId;
		public int StepCurrentId
		{
			get
			{
				return m_StepCurrentId;
			}
		}
		public bool GuideState
		{
			get
			{
				return m_GuideState;
			}
		}
		#endregion

		#region Private mem
		private Hashtable m_StepObjMap = Johny.HashtablePool.Claim();
		private Dictionary<int, GuideStepData> m_GuideTable;
		private int m_StepProgress;
		private int m_StepCurrentId;
		private eStepExcuteState m_StepExcuteState;
		private int m_StepStartId;
		private bool m_GuideState = false;
		private Transform m_CurrentFrame;
		#endregion

		#region Instance
		private static GuideManager m_Instance;
		public static GuideManager Instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = new GuideManager();
				}
				return m_Instance;
			}
		}
		#endregion

		private GuideManager()
		{
			m_GuideTable = new Dictionary<int, GuideStepData>();
		}

		public static void ClearUp()
		{
			if (m_Instance != null)
			{
				m_Instance.m_GuideTable.Clear();
			}
		}

		public void InitConfigData(GM.DataCache.ConditionGuide GuideList)
		{
			if (GuideList == null) return;

			var conditionSet = GuideList;
			m_GuideTable.Clear();
			for (int i = 0; i < conditionSet.GuideLength; i++)
			{
                GuideStepData step = new GuideStepData(conditionSet.GetGuide(i));
				m_GuideTable[step.guide_id] =  step;
				if (step.fore_id == 0) m_StepStartId = step.guide_id;
				if (step.type == 1 && step.level > END_LEVEL) END_LEVEL = step.level;
			}

			m_StepEndId = FindForceGuideEndId();
		}

		public void InitGuideState()
		{
			m_StepExcuteState = eStepExcuteState.Excuting;
			if (!GameEngine.Instance.IsFTE)
			{
				m_GuideState = false;
				return;
			}


			int progressid;
			int local_progressid = PlayerPrefs.GetInt(LoginManager.Instance.LocalUserId.Value + "_progress_id", -1);
			if (local_progressid != -1) progressid = local_progressid;
			else
			{
				if (!DataLookupsCache.Instance.SearchIntByID("guide_progress", out progressid)) progressid = 0;
			}

			if (0 == progressid)
			{
				m_StepCurrentId = m_StepStartId;
				m_StepProgress = m_StepStartId;
				m_GuideState = true;
			}
			else
			{
				GuideStepData step;
				if (!GetStep(progressid, out step)) m_GuideState = false;
				else
				{
					//if (0 == step.next_id )
					GuideStepData next_step;
					if (!GetStep(step.next_id, out next_step))
					{
						m_GuideState = false;
						return;
					}
					if (next_step.type == 0)// 接下来是开放式引导了
					{
						m_GuideState = false;
						//OpenGuideManager.Instance.InitGuideState();
					}
					else
					{
						//1 判定当前的回滚点是否条件满足 满足则设置回滚 否则找前一个回滚点
						GuideStepData rollback;
						if (!GetStep(step.rollback_id, out rollback)) m_GuideState = false;
						else
						{
							int rollbackid;
							if (rollback.EnoughCondition()) rollbackid = step.rollback_id;
							else
							{
								rollbackid = GetLastRollBackId(progressid);
								if (!GetStep(rollbackid, out rollback) || !rollback.EnoughCondition())
								{
									m_GuideState = false;
									EB.Debug.LogWarning("GuideManager init ===========rollbackid={0}Condition is not ok  stop guide",rollbackid);
									return;
								}
							}
							EB.Debug.Log("GuideManager init ===========progressid={0}==rollbackid={1}" , progressid , rollbackid);
							m_StepCurrentId = rollbackid;
							m_StepProgress = rollbackid;
							m_GuideState = true;
						}
					}
				}
			}

			if (m_GuideState)
			{
				IsBigThanMaxLevel();
			}
		}

		public bool ExcuteExReal(Transform Frame, System.Action onFinish)
		{
			bool started = false;
			if (HasGuidNow())
			{
				GuideStepData now;
				if (!GetStep(m_StepCurrentId, out now))
				{
					return false;
				}
				if (!Frame.name.Contains(now.view)) return false;
				if (string.IsNullOrEmpty(now.target_path))
				{
					GuideManager.Instance.ExcuteCurrentStep(null, onFinish);
				}
				else
				{
					Transform target = null;
					if (now.excute_type.Equals("CampaignGuideStep"))
					{
						target = MainLandLogic.GetInstance().ThemeLoadManager.GetSceneRootObject().transform.Find(now.target_path);
					}
					else target = Frame.Find(now.target_path);
					if (target == null)
					{
						EB.Debug.LogWarning("Cant find obj for{0} for {1}" , now.target_path , m_StepCurrentId);
						return false;
					}
					else if (!target.gameObject.activeInHierarchy)
					{
						return false;
					}
					InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.2f);//防止连续的点击
					m_CurrentFrame = Frame;
					GuideManager.Instance.ExcuteCurrentStep(target.gameObject, onFinish);
				}
				started = true;
			}
			return started;
		}

		public bool ExcuteEx(Transform Frame, System.Action onFinish)
		{
			if (m_GuideState == false)
			{
				OpenGuideManager.Instance.ExcuteEx(Frame, null);
				return false;
			}
			else
			{
				if (IsBigThanMaxLevel()) return false;
			}
			InputBlockerManager.Instance.Block(InputBlockReason.CONVERT_FLY_ANIM, 0.1f);//防止连续的点击
			return ExcuteExReal(Frame, onFinish);
		}

		bool IsBigThanMaxLevel()
		{
			int level = BalanceResourceUtil.GetUserLevel();
			if (level > END_LEVEL)
			{
				EB.Debug.Log("IsBigThanMaxLevel Guide End  ={0}" , level);
				JumToEnd();
				return true;
			}
			return false;
		}

		public bool HasGuidNow()
		{
			if (StepCurrentId > 0) return true;
			FindCouldStep();
			if (StepCurrentId > 0) return true;
			return false;
		}

		public eTipAnchorType GuidNowAnchor()
		{
			if (!HasGuidNow()) return eTipAnchorType.center;
			GuideStepData now;
			if (!GetStep(m_StepCurrentId, out now)) return eTipAnchorType.center;
			return (eTipAnchorType)now.tips_anchor;
		}

		public GuideStep ExcuteCurrentStep(GameObject stepObj, System.Action onFinish)
		{
			mCurrentStep = null;
			GuideStepData now;
			if (!GetStep(m_StepCurrentId, out now)) return null;
			if (m_StepExcuteState == eStepExcuteState.Excuted) return null;
			m_StepExcuteState = eStepExcuteState.Excuted;
			string excute_type = now.excute_type.ToString();
			if (excute_type.Equals("default"))
			{
				GuideStep step = new GuideStep();
				step.GuideId = m_StepCurrentId;
				step.m_StepObj = stepObj;
				if (onFinish != null) step.onFinish += onFinish;
				step.ExcuteReal();
				mCurrentStep = step;
				return step;
			}
			else
			{
				Type type = Type.GetType(excute_type);
				object obj = Activator.CreateInstance(type);
				GuideStep step = (GuideStep)obj;
				step.GuideId = m_StepCurrentId;
				step.m_StepObj = stepObj;
				if (onFinish != null) step.onFinish += onFinish;
				step.ExcuteReal();
				mCurrentStep = step;
				return step;
			}
		}

		//执行当前步骤
		public GuideStep ExcuteCurrentStep(GameObject stepObj)
		{
			return ExcuteCurrentStep(stepObj, null);
		}

		public bool IsNeedToDisableNavigation()
		{
			if (!m_GuideState) return false;
			if (m_StepCurrentId == 0) return false;
			GuideStepData now;
			if (!GetStep(m_StepCurrentId, out now)) return false;
			if (now != null && now.trigger_type.Equals("late_2")) return true;
			return false;
		}

		private GuideStep mCurrentStep = null;
		//完成当前步骤
		public void FinishStep()
		{
			EB.Debug.Log("=============FinishStep{0}" , m_StepCurrentId);
			m_StepCurrentId = 0;
			MengBanController.Instance.Hide();
			if (!m_GuideState) return;
			GuideStepData now;
			if (!GetStep(m_StepProgress, out now))
			{
				m_StepCurrentId = 0;
				return;
			}
			if (now.next_id == 0)
				m_GuideState = false;

			m_StepExcuteState = eStepExcuteState.Excuting;
			//判定条件 设置m_StepCurrentId m_StepProgress
			FindCouldStep();
			//获取到了下一个步骤
			if (m_StepCurrentId > 0)
			{
				GuideStepData next;
				if (GetStep(m_StepCurrentId, out next))
				{
					//判定执行类型  立即执行，还是触发执行
					string trigger_type = next.trigger_type;
					//if (trigger_type.Equals("now"))ExcuteCurrentStep(m_StepObjMap[StepCurrentId + ""] as GameObject);
					if (trigger_type.Equals("now"))
					{
						if (m_StepObjMap[StepCurrentId + ""] != null)
						{
							ExcuteCurrentStep(m_StepObjMap[StepCurrentId + ""] as GameObject);
						}
						else
						{
							ExcuteEx(m_CurrentFrame.transform, null);
						}
					}
					else if (trigger_type.Equals("late_1"))
					{
						mCurrentStep = null;
					}
					else if (trigger_type.Equals("late_2"))
					{
						mCurrentStep = null;
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
				if (GetNextStep(m_StepProgress, out next))
				{
					if (next.type == 0)// 接下来是开放式引导了
					{
						m_GuideState = false;
						OpenGuideManager.Instance.InitGuideState();
						return;
					}
					if (next.EnoughCondition())
					{
						m_StepCurrentId = next.guide_id;
						m_StepProgress = m_StepCurrentId;
					}
					else EB.Debug.LogWarning("Next guide condition is not ok id={0}" , next.guide_id);
				}
			}
		}

		//通过id获得一个引导步骤的数据
		public bool GetStep(int guideid, out GuideStepData step)
		{
			if (m_GuideTable.ContainsKey(guideid))
			{
				step = m_GuideTable[guideid];
				return true;
			}
			if (guideid != 0) EB.Debug.LogWarning("No step data for GuideID={0}" , guideid);
			step = null;
			return false;
		}

		//通过id获得下一个引导步骤的数据
		public bool GetNextStep(int guideid, out GuideStepData next)
		{
			GuideStepData now;
			if (!GetStep(guideid, out now))
			{
				next = null;
				return false;
			}
			if (now.next_id == 0)
			{
				next = null;
				m_GuideState = false;
				return false;
			}
			if (!GetStep(now.next_id, out next))
			{
				return false;
			}
			return true;
		}

		public int GetLastRollBackId(int GuideId)
		{
			GuideStepData now;
			int now_guide = GuideId;
			int next_rollback_id = 0;
			if (!GetStep(now_guide, out now)) return 0;
			else
			{
				next_rollback_id = now.rollback_id;
				now_guide = now.fore_id;
			}
			int i = 0;
			while (i < 20)
			{
				i++;
				if (!GetStep(now_guide, out now)) return next_rollback_id;
				else
				{
					if (now.rollback_id != next_rollback_id) return now.rollback_id;
					else now_guide = now.fore_id;
				}
			}
			return next_rollback_id;
		}

		public void JumToEnd()
		{
			m_GuideState = false;
			JumpTo(m_StepEndId, true);
			//if (SmallPartnerHudController.instance!=null) SmallPartnerHudController.instance.isStopUpdateTeamSetting = false;
			OpenGuideManager.Instance.InitGuideState();
		}

		public void JumpTo(int guidid, bool isJumpEnd)
		{
			m_StepCurrentId = guidid;
			m_StepProgress = guidid;
			if (mCurrentStep != null)
			{
				mCurrentStep.FinishReal();
			}
		}

		int FindForceGuideEndId()
		{
			int end = 0;
			GuideStepData step_now = null;
			GuideStepData step_next = null;
			if (!GetStep(m_StepStartId, out step_now)) return end;
			int limit = 200;
			while (limit > 0)
			{
				limit--;
				if (!GetStep(step_now.next_id, out step_next)) break;
				if (step_now.type == 1 && step_next.type != 1)
				{
					end = step_now.guide_id;
					break;
				}
				step_now = step_next;
			}
			return end;
		}
	}
}
