using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Boo.Lang;
using ILRuntime.Runtime;
using MoveEditor;

namespace Hotfix_LT.Combat
{
	public class SkillEventState : CombatEventState
	{
		public enum ComboType
		{
			None,
			Next,
			Wait,
			Other
		}

		private MoveEditor.PlayHitReactionProperties m_hitReactionProps = null;
		private MoveEditor.ParticleEventProperties m_particleProps = null;
		private MoveEditor.AudioEventProperties m_audioProps = null;
		private float m_hitReactionStartFrame = -1.0f;
		private float m_hitReactionStartTime = -1.0f;
		private List<ReactionEffectEvent> m_hitReactionTargets = new List<ReactionEffectEvent>();

		private List<ReactionEffectEvent> m_hitTargets = new List<ReactionEffectEvent>();

		private MoveController.CombatantMoveState m_lastMoveState = MoveController.CombatantMoveState.kIdle;

		private ComboType m_comboType = ComboType.None;
		private List<CombatEventTree> m_comboList = null;
		//private List<CombatEventTree> m_playedComboList = null;
		private float m_comboTriggerTime = 0.0f;
		private bool m_comboTriggered = false;
		//private bool m_triggerBeyondAction = false;

		private bool m_skillActionFlip = false;

		private static List<CombatantIndex> s_contextInvolved = new List<CombatantIndex>();
		private static bool s_fadeIrrelevance = false;
		protected static EnvironmentDisappearType CurrentFadeEnvironment = EnvironmentDisappearType.None;

		public CombatSkillEvent SkillEvent
		{
			get;
			private set;
		}

		//public SkillTemplate SkillData
		//{
		//	get;
		//	private set;
		//}

		public MoveEditor.Move SkillMove
		{
			get;
			private set;
		}

		#region SkillTemplate's fields
		private string moveName;
		private int doubleHitCount;
		private MoveEditor.Move.eSkillActionPositionType actionPosition;
		private MoveEditor.Move.eSkillTargetPositionType targetPosition;
		private int skillId;
		private EnvironmentDisappearType fadeEnvironment;
		private float targetDistance;
		private bool ignoreCollide = false;
		private bool hideOthers = false;

		private void CleanTemplate()
		{
			skillId = 0;
			moveName = String.Empty; ;
			doubleHitCount = 0;
			actionPosition = Move.eSkillActionPositionType.ORIGINAL;
			targetPosition = Move.eSkillTargetPositionType.NONE;
			fadeEnvironment = EnvironmentDisappearType.None;
			targetDistance = 0;
			ignoreCollide = false;
			hideOthers = false;
		}
		#endregion

		public SkillEventState(Combatant combatant, CombatSkillEvent skill_event)
			: base(combatant, skill_event)
		{
			SkillEvent = skill_event;

			GetTemplateInfo(ref moveName, ref doubleHitCount, ref actionPosition, ref targetPosition, ref skillId, ref fadeEnvironment, ref targetDistance, ref ignoreCollide, ref hideOthers);

			SkillMove = Combatant.MoveController.GetMove(moveName, false);
            if (SkillMove == null)
            {
                //EB.Debug.LogError("SkillEventState: skill move not found, move = " + SkillData.MoveName + ", combatant = " + combatant.myName);
                OnEnd();
                return;
            }

            SkillEvent.SetHitTimes(SkillMove._hitEvents.Count);
            SkillEvent.SetDoubleHitCount(doubleHitCount * skill_event.GetHitTargetCount());
        }

		public SkillEventState()
		{

		}

		private void GetTemplateInfo(ref string moveName, ref int doubleHitCount, 
			ref MoveEditor.Move.eSkillActionPositionType actionPosition, 
			ref MoveEditor.Move.eSkillTargetPositionType targetPosition, 
			ref int id, ref EnvironmentDisappearType fadeEnvironment, ref float targetDistance,
			ref bool ignoreCollide, ref bool hideOthers)
        {
			Hashtable data = (Hashtable)GlobalUtils.CallStaticHotfixEx("Hotfix_LT.Data.SkillTemplateManager", "Instance", "GetTemplateEx", SkillEvent.SkillId);
			moveName = data["moveName"].ToString();
			doubleHitCount = data["doubleHitCount"].ToInt32();
			actionPosition = (MoveEditor.Move.eSkillActionPositionType)data["actionPosition"];
			targetPosition = (MoveEditor.Move.eSkillTargetPositionType)data["targetPosition"];
			id = data["id"].ToInt32();
			fadeEnvironment = (EnvironmentDisappearType)((data["fadeEnvironment"]).ToInt32());
			targetDistance = data["targetDistance"].ToFloat();
			ignoreCollide = (bool)data["ignoreCollide"];
			hideOthers = (bool)data["hideOthers"];
		}

		public override void Init(Combatant combatant, CombatEvent combat_event)
		{
			//Debug.Assert(combat_event is CombatSkillEvent, "invalid event type");

			base.Init(combatant, combat_event);

			SkillEvent = combat_event as CombatSkillEvent;

			GetTemplateInfo(ref moveName, ref doubleHitCount, ref actionPosition, ref targetPosition, ref skillId, ref fadeEnvironment, ref targetDistance, ref ignoreCollide, ref hideOthers);

			string move = SkillEvent.IsCombo ? "SKILL_comboattack" : moveName;
            if (move == "")
            {
				//技能允许填空的相关改动：
				//先假装选取一个一定有的动作(普攻)，然后再这里骗过底层代码认为他已经进入到攻击状态
				//然后再SkillActionState.cs中，如果判断技能为空，则先播放普攻，然后再update中立刻stop
				move = "SKILL_normalattack";
                m_lastMoveState = MoveController.CombatantMoveState.kAttackTarget;
            }
            SkillMove = Combatant.MoveController.GetMove(move, false);

            if (SkillMove == null)
            {
                OnEnd();
                return;
            }

            SkillEvent.SetHitTimes(SkillMove._hitEvents.Count);
            SkillEvent.SetDoubleHitCount(doubleHitCount * SkillEvent.GetHitTargetCount());


            InitEnvironment();
		}

		public override void CleanUp()
		{
			Combatant.StopAllCoroutines();
			ClearEnvironment();

			SkillEvent = null;
			//SkillData = null;
			CleanTemplate();
			SkillMove = null;
			m_hitReactionProps = null;
			m_particleProps = null;
			m_audioProps = null;
			m_hitReactionStartFrame = -1.0f;
			m_hitReactionStartTime = -1.0f;
			m_hitReactionTargets = new List<ReactionEffectEvent>();
			m_hitTargets = new List<ReactionEffectEvent>();
			m_lastMoveState = MoveController.CombatantMoveState.kIdle;
			m_comboType = ComboType.None;
			m_comboList = null;
			//m_playedComboList = null;
			m_comboTriggerTime = 0.0f;
			m_comboTriggered = false;
			m_skillActionFlip = false;
			//m_triggerBeyondAction = false;

			base.CleanUp();
		}

		protected override void OnStart()
		{
			if (SkillMove == null)
			{
				return;
			}

			OnSkillStart();
			
			if (SkillEvent != null)
			{
				s_contextInvolved = SkillEvent.GetInvolved();
			}

			FadeEnvironment();
			FadeIrrelevance();

			try
			{
				MoveEditor.Move.eSkillActionPositionType actionPositionType = actionPosition == MoveEditor.Move.eSkillActionPositionType.INVALID ? SkillMove._actionPosition : actionPosition;
				if (actionPositionType == MoveEditor.Move.eSkillActionPositionType.MIDPOINT ||
					actionPositionType == MoveEditor.Move.eSkillActionPositionType.MIDLINE ||
					actionPositionType == MoveEditor.Move.eSkillActionPositionType.TRANSFORM ||
					actionPositionType == MoveEditor.Move.eSkillActionPositionType.TARGET)
				{
					if (SkillEvent != null && SkillEvent.Targets != null && SkillEvent.Targets.First() != null && SkillEvent.Targets.First().TeamIndex == Combatant.Index.TeamIndex)
					{
						m_skillActionFlip = true;
					}
				}
			}
			catch(System.NullReferenceException e)
			{
				EB.Debug.LogError(e.ToString());
			}
		}

		protected override void OnUpdate()
		{
			if (SkillMove == null)
				return;

			MoveController.CombatantMoveState current_move_state = Combatant.GetMoveState();
			MoveController.CombatantMoveState last_move_state = m_lastMoveState;
			m_lastMoveState = current_move_state;

			if (last_move_state == current_move_state)
			{
				if (current_move_state == MoveController.CombatantMoveState.kAttackTarget)
				{
					UpdateRotateHit();
				}
			}
			else
			{
				if (last_move_state == MoveController.CombatantMoveState.kForward)
				{
					OnForwardEnd();
				}
				else if (last_move_state == MoveController.CombatantMoveState.kBackward)
				{
					OnBackwardEnd();
				}
				else if (last_move_state == MoveController.CombatantMoveState.kAttackTarget)
				{
					OnSkillEnd();
				}
			}
		}

		protected override void OnEnd()
		{
			RecoverEnvironment();
			Hotfix_LT.UI.LTCombatEventReceiver.Instance.EndSkill(m_combatant.Data);
		}

		public override void Interrupt()
		{
			if (Disabled)
			{
				return;
			}
			Disabled = true;

			// stop action
			Combatant.ActionState.Stop();

			//// trigger events
			//CombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_FORWARD_START);
			//CombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_FORWARD_END);
			//CombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_SKILL_START);
			//CombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_SKILL_END);
			//CombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_BACKWARD_START);
			//CombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_BACKWARD_END);

			// reset timescale
			Combatant.TimeScale = null;
			if (!Hotfix_LT.UI.LTCombatEventReceiver.Instance.IsRadialBlurEffect())
			{
				Time.timeScale = Hotfix_LT.UI.LTCombatEventReceiver.Instance.TimeScale;
			}

			// trigger combo event if not calculated
			//if (m_comboType != ComboType.None && !m_comboTriggered && (m_comboTriggerTime <= 0.0f || !m_triggerBeyondAction))
			//{
			//	Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_COMBO);
			//}

			// clean state
			if (Combatant.EventState == this)
			{
				Combatant.EventState = null;
			}
			else
			{
				EB.Debug.LogWarning(string.Format("SkillEventState.Interrupt: EventState changed, {0} => {1}", Event.GetLogId(), Combatant.EventState.Event.GetLogId()));
			}

			// do something opposite to OnStart
			// 此前报错时已经调用过on end ，interrupt时如果再调用，就会触发两次同一技能的onend事件
			//OnEnd();

			// event done
			//Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnCombatEventEnd(Event);

			// extra clean
			Combatant.FXHelper.ReportAttackResult(MoveEditor.AttackResult.None);

			Combatant.StashEventState(this);
		}

		protected override void OnStop()
		{
			OnEnd();
		}

		public List<T> GetTargets<T>()
		{
			List<object> obj_list = GetTargets(typeof(T));

			List<T> targets = new List<T>();
			for (int i = 0, cnt = obj_list.Count; i < cnt; ++i)
			{
				object obj = obj_list[i];
				targets.Add((T)obj);
			}

			return targets;
		}

		private List<object> GetTargets(System.Type type)
		{
			List<object> targets = new List<object>();

			bool is_idx = type == typeof(CombatantIndex);
			bool is_combatant = type == typeof(Combatant);
			bool is_gameobject = type == typeof(GameObject);

			var it = SkillEvent.Targets.GetEnumerator();
			while(it.MoveNext())
			{
				CombatantIndex idx = it.Current;
				if (is_idx)
				{
					targets.Add(idx);
					continue;
				}

				Combatant combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(idx);
				if (combatant == null)
				{
					continue;
				}
				if (is_combatant)
				{
					targets.Add(combatant);
					continue;
				}

				if (is_gameobject)
				{
					targets.Add(combatant.gameObject);
					continue;
				}
			}

			return targets;
		}

		public List<T> GetHitTargets<T>()
		{
			List<object> obj_list = GetHitTargets(typeof(T));

			List<T> targets = new List<T>();
			for (int i = 0, cnt = obj_list.Count; i < cnt; ++i)
			{
				object obj = obj_list[i];
				targets.Add((T)obj);
			}

			return targets;
		}

		private List<object> GetHitTargets(System.Type type)
		{
			List<object> targets = new List<object>();

			bool is_idx = type == typeof(CombatantIndex);
			bool is_combatant = type == typeof(Combatant);
			bool is_gameobject = type == typeof(GameObject);

			if (m_hitTargets == null)
			{
				return targets;
			}

			for (int i = 0, cnt = m_hitTargets.Count; i < cnt; ++i)
			{
				ReactionEffectEvent evt = m_hitTargets[i];
				CombatantIndex idx = evt.Target;

				if (is_idx)
				{
					targets.Add(idx);
					continue;
				}

				Combatant combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(idx);
				if (is_combatant)
				{
					targets.Add(combatant);
					continue;
				}

				if (is_gameobject)
				{
					targets.Add(combatant.gameObject);
					continue;
				}
			}

			return targets;
		}

		//调试代码
		public float GetTargetRadius()
		{
			float radius = 0.0f;
			List<Combatant> targets = GetTargets<Combatant>();
			for (int i = 0, cnt = targets.Count; i < cnt; ++i)
			{
				Combatant target = targets[i];
				if (radius < target.Radius)
				{
					radius = target.Radius;
				}
			}
			if (radius <= 0.0f)
			{
				radius = 1.0f;
			}

			return radius;
		}

		public T GetAttackTarget<T>()
		{
			object obj = GetAttackTarget(typeof(T));
			return obj != null ? (T)obj : default(T);
		}

		private object GetAttackTarget(System.Type type)
		{
			// first alive one
			CombatantIndex idx = null;
			var it = SkillEvent.Targets.GetEnumerator();
			while(it.MoveNext())
			{
				idx = it.Current;
				if (Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(idx).IsAlive())
				{
					break;
				}
			}

			bool is_idx = type == typeof(CombatantIndex);
			bool is_combatant = type == typeof(Combatant);
			bool is_gameobject = type == typeof(GameObject);

			if (is_idx)
			{
				return idx;
			}

			Combatant combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(idx);
			if (is_combatant)
			{
				return combatant;
			}

			if (is_gameobject)
			{
				return combatant.gameObject;
			}

			return null;
		}

		public bool IsAttackRightSide()
		{
			Quaternion current_rotation = Combatant.transform.rotation;
			Quaternion start_rotation = Combatant.StartRotation;
			//switch(0 ~ 360) to (-180 & 180)
			float start_y = start_rotation.eulerAngles.y > 180 ? start_rotation.eulerAngles.y - 360 : start_rotation.eulerAngles.y;
			float current_y = current_rotation.eulerAngles.y > 180 ? current_rotation.eulerAngles.y - 360 : current_rotation.eulerAngles.y;

			if (current_y > start_y)
			{
				return true;
			}

			return false;
		}

		public bool IsCriticalAttack()
		{
			return false;
		}

		protected void OnForwardEnd()
		{
			StartSkillAction();
		}

		protected void OnBackwardEnd()
		{
			End();
		}

		protected void OnSkillStart()
		{
			if (NeedForward())
			{
				Forward();
			}
			else
			{
				StartSkillAction();
			}
		}

		protected void OnSkillEnd()
		{
			Combatant.FXHelper.Flip(CombatLogic.Instance.IsOpponentSide(Combatant.Index.TeamIndex));
			if (NeedBackward())
			{
				Backward();
			}
			else
			{
				End();
			}
			Combatant.TimeScale = null;
		}

		protected void UpdateRotateHit()
		{
			if (m_hitReactionProps == null)
			{
				return;
			}
			float phase_shift = 3.0f * Mathf.PI / 2.0f;
			float delta_time = Time.time - m_hitReactionStartTime;
			float duration_time = m_hitReactionProps.framesToPlay / Combatant.CurrentMove._animationClip.frameRate;
			float sin_curve = Mathf.Sin(delta_time * 2 * Mathf.PI * 1.5f / duration_time + phase_shift);
			AnimatorStateInfo state_info = Combatant.MoveController.GetCurrentStateInfo();
			float total_frames = Combatant.CurrentMove._animationClip.length * Combatant.CurrentMove._animationClip.frameRate;
			float current_frame = state_info.normalizedTime * total_frames;
			float delta_frame = current_frame - m_hitReactionStartFrame + 3 * sin_curve;
			float normalized_delta_frame = delta_frame / m_hitReactionProps.framesToPlay;

			if (normalized_delta_frame >= 1.0f)
			{
				for (int i = 0, cnt = m_hitReactionTargets.Count; i < cnt; ++i)
				{
					Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(m_hitReactionTargets[i].Target);
					target.FXHelper.PlayParticle(m_particleProps, false);
					target.FXHelper.PlayAudio(m_audioProps, false);
				}
				m_hitReactionTargets.Clear();
				m_hitReactionProps = null;
				return;
			}

			float normalized_remaining_frame = 1.0f - normalized_delta_frame;
			float lerp_angle = Mathf.Lerp(m_hitReactionProps.startAngle, m_hitReactionProps.endAngle, normalized_remaining_frame);
			bool rightToLeft = m_hitReactionProps.startAngle < m_hitReactionProps.endAngle;
			Vector3 sweepingRay = Quaternion.Euler(0.0f, lerp_angle, 0.0f) * Combatant.BaseForward;

			for (int i = 0, cnt = m_hitReactionTargets.Count; i < cnt;)
			{
				Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(m_hitReactionTargets[i].Target);
				float dotProduct = Vector3.Dot(sweepingRay, Vector3.Normalize(target.OriginPosition - Combatant.OriginPosition));
				bool playTheHitReaction = rightToLeft ? dotProduct >= 0.0f : dotProduct <= 0.0f;
				if (playTheHitReaction)
				{
					m_hitReactionTargets.RemoveAt(i);
				}
				else
				{
					++i;
				}
			}

			if (m_hitReactionTargets.Count == 0)
			{
				m_hitReactionProps = null;
				TimerManager.instance.RemoveTimerSafely(ref _DelayCalculateComboTime_Sequence);
                _DelayCalculateComboTime_Sequence = TimerManager.instance.AddTimer(200, 1, DelayCalculateComboTime);
            }
		}
		
		#region Coroutine -> timer
		private int _DelayCalculateComboTime_Sequence = 0;
		protected void DelayCalculateComboTime(int sequence)
		{
			CalculateComboTime();
			_DelayCalculateComboTime_Sequence = 0;
		}
		#endregion

		protected void FadeIrrelevance()
		{
			if (!s_fadeIrrelevance && hideOthers)
			{
				s_fadeIrrelevance = true;

				UI.LTCombatEventReceiver.Instance.ForEach(combatant =>
				{
					combatant.colorScale.SetIrrelevance(!s_contextInvolved.Contains(combatant.Index));
				});
			}
		}

		#region About Environment
		private int _currentFadeSkillID = -1;
		private List<int> skillList = new List<int>();
		private RenderSettings stRsRecoverInTime = null;

		private void InitEnvironment(){
			GameObject go = new GameObject("RecoverInTimeRenderSetting");
			go.transform.SetParent(RenderSettingsManager.Instance.transform);
			stRsRecoverInTime = go.AddComponent<RenderSettings>();
		}

		private void ClearEnvironment(){
			if(stRsRecoverInTime){
				GameObject.Destroy(stRsRecoverInTime);
				stRsRecoverInTime = null;
			}
		}

		protected void FadeEnvironment()
		{
			_currentFadeSkillID = skillId;
			skillList.Add(skillId);
			if (CurrentFadeEnvironment != fadeEnvironment && fadeEnvironment != EnvironmentDisappearType.None)
			{
				CurrentFadeEnvironment = fadeEnvironment;

				string settings = RenderSettingsManager.Instance.defaultSettings;
				switch (fadeEnvironment)
				{
					case EnvironmentDisappearType.Black:
						settings = "DarkRenderSettings";
						break;
					case EnvironmentDisappearType.Fade:
						settings = "FadeRenderSettings";
						break;
				}

				RenderSettings setting = RenderSettingsManager.Instance.GetCurrentRenderSettings();
				setting.ShowOrHideOtherShaderRenderers(false);
				setting.StartOrStopSceneFX(false);
				RenderSettingsManager.Instance.SetActiveRenderSettings(settings);
			}

			if (fadeEnvironment == EnvironmentDisappearType.None)
			{
				CurrentFadeEnvironment = EnvironmentDisappearType.None;
                RenderSettings rsRecoverInTime = null;//= new RenderSettings();
				if (!RenderSettingsManager.Instance.RenderSettingDic().ContainsKey("RecoverInTimeRenderSetting"))
				{
					GameObject rsRecoverInTimeObj = new GameObject("RecoverInTimeRenderSetting");
					rsRecoverInTimeObj.transform.SetParent(RenderSettingsManager.Instance.transform);
					rsRecoverInTime = rsRecoverInTimeObj.AddComponent<RenderSettings>();
					rsRecoverInTime.Clone(RenderSettingsManager.Instance.GetLastRenderSettings());
					rsRecoverInTime.BlendInTime = RenderSettingsManager.Instance.GetCurrentRenderSettings().BlendInTime;
				}
				else
				{
					RenderSettingsManager.Instance.RenderSettingDic().TryGetValue("RecoverInTimeRenderSetting", out rsRecoverInTime);
				}
				RenderSettings LastSetting = RenderSettingsManager.Instance.GetLastRenderSettings();
				LastSetting.ShowOrHideOtherShaderRenderers(true);
				LastSetting.StartOrStopSceneFX(true);
				RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.recoverSettings, rsRecoverInTime);
			}
		}

		protected void RecoverEnvironment()
		{
			skillList.Remove(skillId);
			if (m_comboList == null || m_comboList.Count == 0)
			{
				if (_currentFadeSkillID == skillId || skillList.Count == 0)
				{
					CurrentFadeEnvironment = EnvironmentDisappearType.None;
					RenderSettings rsRecoverInTime = null;//= new RenderSettings();
					if (!RenderSettingsManager.Instance.RenderSettingDic().ContainsKey("RecoverInTimeRenderSetting"))
					{
						GameObject rsRecoverInTimeObj = new GameObject("RecoverInTimeRenderSetting");
						rsRecoverInTimeObj.transform.SetParent(RenderSettingsManager.Instance.transform);
						rsRecoverInTime = rsRecoverInTimeObj.AddComponent<RenderSettings>();
						rsRecoverInTime.Clone(RenderSettingsManager.Instance.GetLastRenderSettings());
						rsRecoverInTime.BlendInTime = RenderSettingsManager.Instance.GetCurrentRenderSettings().BlendInTime;
					}
					else
					{
						RenderSettingsManager.Instance.RenderSettingDic().TryGetValue("RecoverInTimeRenderSetting", out rsRecoverInTime);
					}
					RenderSettingsManager.Instance.GetLastRenderSettings().ShowOrHideOtherShaderRenderers(true);
					RenderSettingsManager.Instance.GetLastRenderSettings().StartOrStopSceneFX(true);
					RenderSettingsManager.Instance.SetActiveRenderSettings(RenderSettingsManager.Instance.recoverSettings, rsRecoverInTime);
				}
			}
		}
		#endregion
		
		protected bool NeedForward()
		{
			MoveEditor.Move.eSkillActionPositionType actionPositionType = actionPosition == MoveEditor.Move.eSkillActionPositionType.INVALID ? SkillMove._actionPosition : actionPosition;
			if (actionPositionType == MoveEditor.Move.eSkillActionPositionType.MIDPOINT)
			{
				return true;
			}
			else if (actionPositionType == MoveEditor.Move.eSkillActionPositionType.MIDLINE)
			{
				return true;
			}
			else if (actionPositionType == MoveEditor.Move.eSkillActionPositionType.TARGET)
			{
				return true;
			}
			else if (actionPositionType == MoveEditor.Move.eSkillActionPositionType.ORIGINAL)
			{
				return false;
			}
			else if (actionPositionType == MoveEditor.Move.eSkillActionPositionType.TRANSFORM)
			{
				return true;
			}

			return false;
		}
		public Vector3 CalculateTargetPosition()
		{
            return _CalculateTargetPosition(SkillEvent, actionPosition, targetPosition, SkillMove);
        }

        private static Vector3 _CalculateTargetPosition(CombatSkillEvent skill_event, MoveEditor.Move.eSkillActionPositionType actionPosition, MoveEditor.Move.eSkillTargetPositionType targetPosition, MoveEditor.Move skill_move)
        {
            Vector3 target_position = Vector3.zero;
            MoveEditor.Move.eSkillTargetPositionType targetPositionType = actionPosition == MoveEditor.Move.eSkillActionPositionType.INVALID ? skill_move._targetPosition : targetPosition;

            if (targetPositionType == MoveEditor.Move.eSkillTargetPositionType.SELECT_TARGET_OR_FIRST_TARGET)
            {
                CombatantIndex target_index = skill_event.Target ?? (skill_event.Targets.Count > 0 ? skill_event.Targets.First() : null);
                Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(target_index);
                if (target != null) target_position = target.HitPoint;
                else target_position = Vector3.zero;
            }
            else if (targetPositionType == MoveEditor.Move.eSkillTargetPositionType.FIRST_TARGET)
            {
                CombatantIndex target_index = skill_event.Target ?? (skill_event.Targets.Count > 0 ? skill_event.Targets.First() : null);
                Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(target_index);
                if (target != null) target_position = target.HitPoint;
                else target_position = Vector3.zero;
            }
            else if (targetPositionType == MoveEditor.Move.eSkillTargetPositionType.TARGETS)
            {
                List<Vector3> list = new List<Vector3>();
                var it = skill_event.Targets.GetEnumerator();
                while (it.MoveNext())
                {
                    CombatantIndex target_index = it.Current ?? skill_event.Target;
                    Combatant target = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(target_index);
                    list.Add(target.HitPoint);
                }
                if (list.Count > 0)
                {
                    Bounds target_bounds = GameUtils.CalculateBounds(list);
                    target_position = target_bounds.center;
                }
                else
                {
                    target_position = Vector3.zero;
                }
            }
            else if (targetPositionType == MoveEditor.Move.eSkillTargetPositionType.TRANSFORM)
            {
                target_position = Vector3.zero;
            }
            return target_position;
        }
        protected Vector3 CalculateForwardPosition(CombatSkillEvent skill_event, Move.eSkillActionPositionType actionPosition, Move.eSkillTargetPositionType targetPosition, bool ignoreCollide)
        {
            Move.eSkillActionPositionType actionPositionType = actionPosition == Move.eSkillActionPositionType.INVALID ? SkillMove._actionPosition : actionPosition;
            float distance = actionPosition == Move.eSkillActionPositionType.INVALID ? SkillMove._attackRadius : targetDistance;

            Vector3 forward_position = Vector3.zero;

            Vector3 target_position = _CalculateTargetPosition(skill_event, actionPosition, targetPosition, SkillMove);

            Vector3 target_forward = Vector3.forward;
            {
                CombatantIndex target_index = skill_event.Targets.First() ?? skill_event.Target;  //maybe skill_event.Targets[0] is null
                Combatant target = UI.LTCombatEventReceiver.Instance.GetCombatant(target_index);
                target_forward = target.OriginForward;
            }

            if (actionPositionType == Move.eSkillActionPositionType.MIDPOINT)
            {
                forward_position = Combatant.transform.parent.parent.parent.parent.parent.position;
                forward_position -= distance * Combatant.transform.forward;
            }
            else if (actionPositionType == Move.eSkillActionPositionType.MIDLINE)
            {
                forward_position = Combatant.transform.parent.parent.parent.parent.parent.position;
                forward_position -= distance * Combatant.transform.forward;
                forward_position.x = target_position.x;
            }
            else if (actionPositionType == Move.eSkillActionPositionType.TARGET)
            {
                forward_position = target_position;
                forward_position += distance * target_forward;
            }
            else if (actionPositionType == Move.eSkillActionPositionType.TRANSFORM)
            {
                forward_position = SkillMove._attackLocation.position;
                forward_position += distance * target_forward;
            }

            // irrelevant combatants
            List<Combatant> irrelevants = new List<Combatant>();
            UI.LTCombatEventReceiver.Instance.ForEach(combatant =>
            {
                if (!s_contextInvolved.Contains(combatant.Index))
                {
                    irrelevants.Add(combatant);
                }
            });

			if (!ignoreCollide)
            {
                // sort, OrderBy ThenBy can't used in iOS platform (Mono Full AOT Compile)
                //Combatant[] sorted = irrelevants.OrderBy(combatant => (combatant.transform.position - forward_position).sqrMagnitude).ToArray();

                List<Combatant> sorted = new List<Combatant>(irrelevants);
                sorted.Sort(new CombatantDistanceComparer(forward_position));

                Vector3 MV = forward_position - Combatant.transform.position;
                float radius = Combatant.Collider.radius;

                if (Combatant.Collider.direction == 0)
                {
                    radius = Combatant.Collider.height / 2;
                }

				// check collision
				for (int i = 0, cnt = sorted.Count; i < cnt; ++i)
                {
                    Combatant combatant = sorted[i];
                    float radius1 = combatant.Collider.direction == 0 ? combatant.Collider.height / 2 : combatant.Collider.radius;

                    float R = radius + radius1;
                    Vector3 V = combatant.transform.position - forward_position;
                    float L = V.magnitude;

					if (L < R)
                    {
						forward_position.x = combatant.transform.position.x + R;
					}
                    else if (V.x * MV.x > 0)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return forward_position;
        }

        protected void Forward()
		{
			ForwardActionState fa = Combatant.GetActionState<ForwardActionState>();
            fa.DestnationLocation = CalculateForwardPosition(SkillEvent, actionPosition, targetPosition, ignoreCollide);
            Combatant.SetActionState(fa);

			m_lastMoveState = MoveController.CombatantMoveState.kForward;

			//Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_FORWARD_START);
		}

		protected bool NeedBackward()
		{
			if (!NeedForward())
			{
				return false;
			}

			if (m_comboType != ComboType.None && m_comboType != ComboType.Other)
			{
				if (m_comboType == ComboType.Next)
				{
					return false;
				}

				if (m_comboType == ComboType.Wait)
				{// check position
					return false;
				}

				return false;
			}
			// else if (m_comboType == ComboType.None)
			// {
			// 	// check next one is the same
			// 	//CombatEventTree node  = Hotfix_LT.UI.LTCombatEventReceiver.Instance.FindEventTree(SkillEvent);
			// 	//List<CombatEventTree> auto = node.GetChildren(eCombatEventTiming.AUTO);
			// 	//if (auto != null && auto.Count > 0)
			// 	//{
			// 	//	if (auto[0].Event.Type == eCombatEventType.SKILL)
			// 	//	{
			// 	//		CombatSkillEvent next_auto_skill = auto[0].Event as CombatSkillEvent;
			// 	//		if (CompareSkillEvent(SkillEvent, next_auto_skill))
			// 	//		{
			// 	//			return false;
			// 	//		}
			// 	//	}
			// 	//}
			// }

			return true;
		}

		protected Vector3 CalculateBackwardPosition(CombatSkillEvent skill_event)
		{
			return Combatant.OriginPosition;
		}

		protected void Backward()
		{
			BackwardActionState ba = Combatant.GetActionState<BackwardActionState>();
			ba.DestnationLocation = CalculateBackwardPosition(SkillEvent);
			Combatant.SetActionState(ba);

			m_lastMoveState = MoveController.CombatantMoveState.kBackward;

			//Hotfix_LT.UI.LTCombatEventReceiver.Instance.OnCombatEventTiming(SkillEvent, eCombatEventTiming.ON_BACKWARD_START);
		}

		protected void StartSkillAction()
		{
			SkillActionState sa = Combatant.GetActionState<SkillActionState>();
			if (SkillEvent.IsCombo)
			{
				sa.MoveName = "SKILL_comboattack";
			}
			else
			{
                sa.MoveName = moveName;
            }
			sa.StartRotate(Combatant.transform.localRotation, CalculateRotation(SkillEvent), CalculateHitTime(0));
			Combatant.SetActionState(sa);
			Combatant.FXHelper.Flip(CombatLogic.Instance.IsOpponentSide(Combatant.Index.TeamIndex) ^ m_skillActionFlip);
        }

		protected void StopSkillAction()
		{
			Combatant.ActionState.End();

			OnSkillEnd();
		}

		public Quaternion CalculateRotation(CombatSkillEvent skill_event)
		{
            Move.eSkillActionPositionType actionPositionType = (actionPosition == Move.eSkillActionPositionType.INVALID) ? SkillMove._actionPosition : actionPosition;
            Vector3 target_position = _CalculateTargetPosition(skill_event, actionPosition, targetPosition, SkillMove);
            Quaternion local_rotation = Quaternion.identity;
            if (actionPositionType == Move.eSkillActionPositionType.MIDPOINT ||
                actionPositionType == Move.eSkillActionPositionType.MIDLINE ||
                actionPositionType == Move.eSkillActionPositionType.TRANSFORM)
            {
                if (skill_event != null && skill_event.Targets != null && skill_event.Targets.Count > 0 && skill_event.Targets.First().TeamIndex == Combatant.Index.TeamIndex)
                {
                    return Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    bool isPlayerOrChallengerSide = Combatant.transform.parent.parent.parent.gameObject.name.IndexOf("PlayerOrChallengerSide") >= 0;
                    Quaternion direction = (isPlayerOrChallengerSide ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 180, 0f));
                    return Quaternion.Euler(Combatant.transform.parent.eulerAngles.x, -Combatant.transform.parent.eulerAngles.y, Combatant.transform.parent.eulerAngles.z) * direction;
                }
            }
            else if (actionPositionType == Move.eSkillActionPositionType.TARGET)
            {
                Vector3 to = (target_position - Combatant.transform.position).normalized;
                Quaternion direction = Quaternion.LookRotation(to, Vector3.up);
                return Quaternion.Euler(Combatant.transform.parent.eulerAngles.x, -Combatant.transform.parent.eulerAngles.y, Combatant.transform.parent.eulerAngles.z) * direction;
            }

            return local_rotation;
		}

		protected void CalculateComboTime()
		{
			if (m_comboType == ComboType.None)
			{// no combo
				return;
			}

			if (m_hitReactionProps != null)
			{// rotate hitting
				return;
			}

			if (SkillEvent.TotalHitTimes > 0 && SkillEvent.HitTimes > 0)
			{// attacking
				return;
			}

			Combatant target = GetAttackTarget<Combatant>();

			float combo_need_time = 0.0f;
			if (target.IsBeingAttacked())
			{
				combo_need_time = (target.ActionState as ReactionActionState).CalculateComboTime();
			}

			// combo skill info
			if (m_comboList.Count == 0)
			{
                return;
			}

			CombatSkillEvent combo_skill_event = m_comboList[0].Event as CombatSkillEvent;
			Combatant combo_combatant = Hotfix_LT.UI.LTCombatEventReceiver.Instance.GetCombatant(combo_skill_event.Sender);
			if (combo_combatant == null)
			{
				EB.Debug.LogError("SkillEventState.CalculateComboTime: combo sender not found, sender = {0}" , combo_skill_event.Sender);
				return;
			}

			string moveName = string.Empty;
			int doubleHitCount = 0;
			MoveEditor.Move.eSkillActionPositionType actionPosition = MoveEditor.Move.eSkillActionPositionType.INVALID;
			MoveEditor.Move.eSkillTargetPositionType targetPosition = MoveEditor.Move.eSkillTargetPositionType.NONE;
			int id = 0;
			EnvironmentDisappearType fadeEnvironment = EnvironmentDisappearType.None;
			float targetDistance = 0f;
			bool ignoreCollide = false;
		    bool hideOthers = false;
			GetTemplateInfo(ref moveName, ref doubleHitCount, ref actionPosition, ref targetPosition, ref id, ref fadeEnvironment, ref targetDistance, ref ignoreCollide, ref hideOthers);

            MoveEditor.Move combo_skill_move = combo_combatant.MoveController.GetMove(moveName, false);
            if (combo_skill_move == null)
            {
                EB.Debug.LogError("SkillEventState.CalculateComboTime: combo move not found, move = {0}, combo combatant = {1}", moveName, combo_combatant.myName);
                return;
            }

            if (combo_skill_move._hitEvents == null || combo_skill_move._hitEvents.Count == 0)
            {
                EB.Debug.LogError("SkillEventState.CalculateComboTime: hit event not found in combo move, combo move = {0}:{1}", combo_combatant.myName, moveName);
                return;
            }
            // first hit time
            float combo_skill_first_hit_time = combo_skill_move._hitEvents[0]._frame / combo_skill_move.NumFrames * combo_skill_move.AdjustedLength;
            // forward time
            float combo_forward_time = 0.0f;
			ForwardActionState fa = new ForwardActionState(combo_combatant);
            fa.DestnationLocation = CalculateForwardPosition(combo_skill_event, actionPosition, targetPosition, ignoreCollide);
            if (!fa.ArrivedAtDestination())
			{
				combo_forward_time = fa.CalculateLeftTime();
			}

			// total time
			float sender_need_time = combo_skill_first_hit_time + combo_forward_time/* + (combo_forward_time > 0.0f ? CalculatePrepareEffectTime() : 0.0f)*/;

			float delay_time = 0.0f;
			if (sender_need_time > combo_need_time)
			{// target animation will pause
				delay_time = 0.0f;
			}
			else
			{
				delay_time = combo_need_time - sender_need_time;
			}

			delay_time += combo_skill_event.ComboDelay;
			if (delay_time > 0.0f)
			{// delay trigger
				m_comboTriggerTime = StateTime + delay_time;
				float current_left_time = Combatant.ActionState.CalculateLeftTime();
				if (delay_time > current_left_time)
				{
                }
				else
				{
					//EB.Debug.Log("CalculateComboTime: combo when skill action end or by UpdateComboTime, m_comboTriggerTime = {0}, delay_time = {1} <= left_time = {2}", m_comboTriggerTime, delay_time, current_left_time);
				}
			}
			else
			{// trigger right now
				if (m_comboType == ComboType.Next)
				{
					StopSkillAction();
				}
            }
		}

		protected List<CombatEventTree> CalculateComboQueue(CombatEventTree tree)
		{
			List<CombatEventTree> combo_skill_list = new List<CombatEventTree>();

			if (tree.Event.Type == eCombatEventType.SKILL)
			{
				List<CombatEventTree> combo_list = tree.GetChildren(eCombatEventTiming.ON_COMBO);
				if (combo_list != null && combo_list.Count > 0)
				{
					for (int i = 0, cnt = combo_list.Count; i < cnt; ++i)
					{
						CombatEventTree child = combo_list[i];
						if (child.Event.Type == eCombatEventType.SKILL)
						{
							combo_skill_list.Add(child);
							combo_skill_list.AddRange(CalculateComboQueue(child));
						}
						else
						{
							combo_skill_list.AddRange(CalculateComboQueue(child));
						}
					}
				}
			}
			else if (tree.Event.Timing != eCombatEventTiming.AUTO)
			{
				var iter = tree.GetChildrenNonAlloc();
				while (iter.MoveNext())
				{
					CombatEventTree child = iter.Current;
					if (child.Event.Timing == eCombatEventTiming.AUTO)
					{
						continue;
					}

					if (child.Event.Type == eCombatEventType.SKILL)
					{
						combo_skill_list.Add(child);
						combo_skill_list.AddRange(CalculateComboQueue(child));
					}
					else
					{
						combo_skill_list.AddRange(CalculateComboQueue(child));
					}
				}
				iter.Dispose();
			}

			if (combo_skill_list.Count > 0)
			{
				return combo_skill_list;
			}

			List<CombatEventTree> auto_list = tree.GetChildren(eCombatEventTiming.AUTO);
			if (auto_list != null && auto_list.Count > 0)
			{
				for (int i = 0, cnt = auto_list.Count; i < cnt; ++i)
				{
					CombatEventTree child = auto_list[i];
					if (child.Event.Type == eCombatEventType.SKILL && (child.Event as CombatSkillEvent).IsCombo)
					{
						combo_skill_list.Add(child);
						combo_skill_list.AddRange(CalculateComboQueue(child));
					}
				}
			}

			return combo_skill_list;
		}

		protected List<CombatEventTree> CalculateParentComboQueue(CombatEventTree tree)
		{
			List<CombatEventTree> combo_skill_list = new List<CombatEventTree>();

			if (tree.Event.Type == eCombatEventType.SKILL)
			{
				CombatSkillEvent skill_event = tree.Event as CombatSkillEvent;
				if (skill_event.IsCombo)
				{// parent must be SkillEvent
					combo_skill_list.Add(tree.Parent);
					combo_skill_list.AddRange(CalculateParentComboQueue(tree.Parent));
				}
				else if (tree.Parent.Event.Timing != eCombatEventTiming.AUTO)
				{// parent may be everything
					if (tree.Parent.Event.Type == eCombatEventType.SKILL)
					{
						combo_skill_list.Add(tree.Parent);
					}
					combo_skill_list.AddRange(CalculateParentComboQueue(tree.Parent));
				}
			}
			else if (tree.Event.Timing != eCombatEventTiming.AUTO)
			{
				combo_skill_list.AddRange(CalculateParentComboQueue(tree.Parent));
			}

			return combo_skill_list;
		}

		public float CalculateHitTime(int index)
		{
			if (index < 0 || index >= GetTotalHitCount())
			{
				return -1.0f;
			}

			MoveEditor.HitEventInfo info = GetHitInfo(index);
			return info._frame / SkillMove.NumFrames * SkillMove._animationClip.length;
		}

		public int GetTotalHitCount()
		{
			return SkillMove._hitEvents.Count;
		}

		public MoveEditor.HitEventInfo GetHitInfo(int index)
		{
			return SkillMove._hitEvents[index];
		}
	}

	public class CombatantDistanceComparer : IComparer<Combatant>
	{
		private Vector3 mPosition;

		public CombatantDistanceComparer(Vector3 position)
		{
			mPosition = position;
		}

		public int Compare(Combatant x, Combatant y)
		{
            var xDis = (x.transform.position - mPosition).sqrMagnitude;
            var yDis = (y.transform.position - mPosition).sqrMagnitude;

            return xDis < yDis ? -1 : (xDis > yDis ? 1 : 0);
		}
	}
}