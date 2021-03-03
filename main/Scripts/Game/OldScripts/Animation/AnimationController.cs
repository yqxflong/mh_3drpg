///////////////////////////////////////////////////////////////////////
//
//  AnimationController.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public enum eAnimationEvent
{
	[AnimationStringValue("hit_e")]
	Hit,

	[AnimationStringValue("leaveground_e")]
	LeaveGround,

	[AnimationStringValue("hitground_e")]
	HitGround,

	[AnimationStringValue("interruptcombo_e")]
	InterruptCombo,

	[AnimationStringValue("resetcombo_e")]
	ResetCombo,

	[AnimationStringValue("interruptmove_e")]
	InterruptMove,

	[AnimationStringValue("vfx_e")]
	VFX,

	[AnimationStringValue("sound_e")]
	Sound,

	[AnimationStringValue("footstep_e")]
	Footstep
}

public class AnimationController : MonoBehaviour
{
	public delegate void OnAnimationEvent(eAnimationEvent eventType);

	public event OnAnimationEvent onAnimationEventBegin;
	public event OnAnimationEvent onAnimationEventSustain;
	public event OnAnimationEvent onAnimationEventEnd;

	private static Dictionary<int, AnimationStringValueAttribute> _eventValues;
	private Animator _animator;
	private System.Type _animStateEnum;
	private System.Type _animParameterEnum;
	private Dictionary<int, AnimationStringValueAttribute> _stateValues;
	private Dictionary<int, AnimationStringValueAttribute> _parameterValues;
	private bool[] _lastAnimationEventActive;
	private eAnimationEvent[] _monitoredAnimationEvents;

	public void Initialize(Animator animator, System.Type animStateEnum, System.Type animParameterEnum)
	{
		_animator = animator;
		_animStateEnum = animStateEnum;
		_animParameterEnum = animParameterEnum;

		_stateValues = ReadEnumStrings(_animStateEnum);
		_parameterValues = ReadEnumStrings(_animParameterEnum);

		if (AnimationController._eventValues == null)
		{
			AnimationController._eventValues = ReadEnumStrings(typeof(eAnimationEvent));
		}

		//These are the events that we want to send out delegat events when they change.
		//It's somewhat expensive to monitor for these
		_monitoredAnimationEvents = new eAnimationEvent[] {
			//eAnimationEvent.Hit,
			//eAnimationEvent.InterruptCombo,
			//eAnimationEvent.ResetCombo,
			//eAnimationEvent.InterruptMove,
			//eAnimationEvent.Footstep
		};

		int[] allAnimationEventValues = (int[])System.Enum.GetValues(typeof(eAnimationEvent));

		List<bool> animEvents = new List<bool>();
		foreach (int animEvent in allAnimationEventValues)
		{
			animEvents.Insert(animEvent, false);
		}
		_lastAnimationEventActive = animEvents.ToArray();
	}

	void Update()
	{
		for (int i = 0; i < _monitoredAnimationEvents.Length; i++)
		{
			MonitorAnimEvent(_monitoredAnimationEvents[i]);
		}
	}

	private void MonitorAnimEvent(eAnimationEvent evt)
	{
		int eventValue = (int)evt;
		eAnimationEvent animEvent = (eAnimationEvent)eventValue;
		bool wasActiveLastFrame = _lastAnimationEventActive[eventValue];
		if (IsEventActive(animEvent))
		{
			if (!wasActiveLastFrame)
			{
				if (onAnimationEventBegin != null)
				{
					onAnimationEventBegin(animEvent);
				}
			}
			else
			{
				if (onAnimationEventSustain != null)
				{
					onAnimationEventSustain(animEvent);
				}
			}
			_lastAnimationEventActive[eventValue] = true;
		}
		else
		{
			if (wasActiveLastFrame)
			{
				if (onAnimationEventEnd != null)
				{
					onAnimationEventEnd(animEvent);
				}
			}
			_lastAnimationEventActive[eventValue] = false;
		}
	}

	public bool IsInCharacterState(params eCharacterAnimationState[] states)
	{
		return IsInCharacterState(0, states);
	}

	public bool IsInCharacterState(int layer, eCharacterAnimationState[] states)
	{
		AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(layer);

		foreach (eCharacterAnimationState state in states)
		{
			if (animStateInfo.fullPathHash == _stateValues[(int)state].HashValue)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsInState(params int[] stateHashes)
	{
		return IsInState(0, stateHashes);
	}

	public bool IsInState(int layer, int[] stateHashes)
	{
		AnimatorStateInfo animStateInfo = _animator.GetCurrentAnimatorStateInfo(layer);

		for (int i = 0; i < stateHashes.Length; i++)
		{
			if (animStateInfo.fullPathHash == stateHashes[i])
			{
				return true;
			}
		}
		return false;
	}
	
	public bool IsCharacterStateNext(params eCharacterAnimationState[] states)
	{
		return IsCharacterStateNext(0, states);
	}

	public bool IsCharacterStateNext(int layer, params eCharacterAnimationState[] states)
	{
		AnimatorStateInfo animStateInfo = _animator.GetNextAnimatorStateInfo(layer);

		foreach (eCharacterAnimationState state in states)
		{
			if (animStateInfo.fullPathHash == _stateValues[(int)state].HashValue)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsStateNext(params int[] stateHashes)
	{
		return IsStateNext(0, stateHashes);
	}

	public bool IsStateNext(int layer, params int[] stateHashes)
	{
		AnimatorStateInfo animStateInfo = _animator.GetNextAnimatorStateInfo(layer);

		for (int i = 0; i < stateHashes.Length; i++)
		{
			if (animStateInfo.fullPathHash == stateHashes[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool IsEventActive(eAnimationEvent eventType)
	{
		return _animator.GetFloat(AnimationController._eventValues[(int)eventType].HashValue) > 0.9f;
	}

	public void SetDesiredState(eCharacterStateCommand stateCommand)
	{
		// Useful for figuring out who is setting animation state
		//if (_animator.GetInteger("State") != Convert.ToInt32(stateCommand))
		//{
		//	EB.Debug.LogWarning(gameObject.name + " setting anim to " + stateCommand + " at time " + Time.time);
		//}
		if(_animator != null)
		{
			_animator.SetInteger("State", (int)stateCommand);
		}
	}

	public void SetDesiredState(eInventoryControllerStateCommand stateCommand)
	{
		SetDesiredState((eCharacterStateCommand)stateCommand);
	}

	public bool IsDesiredState(eCharacterStateCommand stateCommand)
	{
		return _animator.GetInteger("State") == (int)stateCommand;
	}

	public void SetFloat(eCharacterAnimationParameter parameter, float value)
	{
		_animator.SetFloat(_parameterValues[(int)parameter].HashValue, value);
	}

	public void SetFloat(eInventoryControllerParam parameter, float value)
	{
		SetFloat((eCharacterAnimationParameter)parameter, value);
	}

	public void SetFloat(eCharacterAnimationParameter parameter, float value, float dampTime)
	{
		StartCoroutine(SetFloatCoroutine(parameter, value, dampTime));
	}

	private System.Collections.IEnumerator SetFloatCoroutine(eCharacterAnimationParameter parameter, float value, float dampTime)
	{
		while (_animator != null && Mathf.Abs(_animator.GetFloat(_parameterValues[(int)parameter].HashValue) - value) > float.Epsilon)
		{
			_animator.SetFloat(_parameterValues[(int)parameter].HashValue, value, dampTime, Time.deltaTime);
			yield return null;
		}

		yield break;
	}

	public void SetInteger(eCharacterAnimationParameter parameter, int value)
	{
		_animator.SetInteger(_parameterValues[(int)parameter].HashValue, value);
	}

	public void SetInteger(eInventoryControllerParam parameter, int value)
	{
		SetInteger((eCharacterAnimationParameter)parameter, value);
	}

	public void SetBool(eCharacterAnimationParameter parameter, bool value)
	{
		_animator.SetBool(_parameterValues[(int)parameter].HashValue, value);
	}

	public void SetBool(eInventoryControllerParam parameter, bool value)
	{
		SetBool((eCharacterAnimationParameter)parameter, value);
	}

	public float GetFloat(eCharacterAnimationParameter parameter)
	{
		return _animator.GetFloat(_parameterValues[(int)parameter].HashValue);
	}

	public float GetFloat(eInventoryControllerParam parameter)
	{
		return GetFloat((eCharacterAnimationParameter)parameter);
	}

	public int GetInteger(eCharacterAnimationParameter parameter)
	{
		return _animator.GetInteger(_parameterValues[(int)parameter].HashValue);
	}

	public int GetInteger(eInventoryControllerParam parameter)
	{
		return GetInteger((eCharacterAnimationParameter)parameter);
	}

	public bool GetBool(eCharacterAnimationParameter parameter)
	{
		return _animator.GetBool(_parameterValues[(int)parameter].HashValue);
	}

	public bool GetBool(eInventoryControllerParam parameter)
	{
		return GetBool((eCharacterAnimationParameter)parameter);
	}

	public void SetPlaybackSpeed(float speed)
	{
		if(_animator != null)
		{
			_animator.speed = speed;
		}
	}

	public float GetCurrentStateLength(int layer = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(layer).length;
	}

	public void RestartCurrentAnimation(int layer = 0)
	{
		// tsl: 4.3 compatability
		// _animator.ForceStateNormalizedTime(0f);
		AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
		_animator.Play(stateInfo.fullPathHash, layer, 0f);
	}

	public void CrossFade(eCharacterAnimationState state, float transitionDuration, int layer = 0)
	{
		_animator.CrossFade(_stateValues[(int)state].HashValue, transitionDuration, layer, 0f);
	}

	public bool HasAnimationFinished(int layer = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1.0f;
	}

	public float GetNormalizedTime(int layer = 0)
	{
		return _animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
	}

	public bool IsInTransition()
	{
		return IsInTransition(0);
	}

	public bool IsInTransition(int layer)
	{
		return _animator.IsInTransition(layer); 
	}

	public AnimationClip GetPrimaryAnimationClip(int layer = 0)
	{
		AnimationClip primaryClip = null;
		float maxWeight = 0f;
		foreach (AnimatorClipInfo animInfo in _animator.GetCurrentAnimatorClipInfo(layer))
		{
			if (primaryClip == null || animInfo.weight > maxWeight)
			{
				primaryClip = animInfo.clip;
				maxWeight = animInfo.weight;
			}
		}
		return primaryClip;
	}

	public void SetLayerWeight(int layer, float weight)
	{
		_animator.SetLayerWeight(layer, weight);
	}

	public void SetLayerWeight(string layer, float weight)
	{
		SetLayerWeight(GetLayerIndex(layer), weight);
	}

	public float GetLayerWeight(int layer)
	{
		return _animator.GetLayerWeight(layer);
	}

	public int GetLayerIndex(string name)
	{
		for (int i = 0; i < _animator.layerCount; ++i)
		{
			if (name.Equals(_animator.GetLayerName(i)))
			{
				return i;
			}
		}
		return 0;
	}

	private static Dictionary<int, AnimationStringValueAttribute> ReadEnumStrings(System.Type enumType)
	{
		Dictionary<int, AnimationStringValueAttribute> values = new Dictionary<int, AnimationStringValueAttribute>();
		foreach (FieldInfo field in enumType.GetFields())
		{
			System.Attribute[] attrs = (System.Attribute[])field.GetCustomAttributes(typeof(AnimationStringValueAttribute), false);
			if (attrs.Length > 0)
			{
				AnimationStringValueAttribute stringValueAttr = (AnimationStringValueAttribute)attrs[0];
				values.Add((int)field.GetValue(null), stringValueAttr);
			}
		}

		return values;
	}
}
