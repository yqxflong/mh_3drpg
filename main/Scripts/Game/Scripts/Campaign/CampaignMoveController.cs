using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class CampaignMoveController : MonoBehaviour {
	private Animator _animator;
	private static Dictionary<int, AnimationStringValueAttribute> _stateValues;

	public void Initialize(Animator animator)
	{
		if(_stateValues == null)
		{
			_stateValues = ReadEnumStrings(typeof(eCampaignCharacterAnimationState));
		}
		_animator = animator;
	}
	

	public void SetDesiredState(MoveController.CombatantMoveState stateCommand)
	{
		if(_animator != null)
		{
			MoveController moveController = _animator.gameObject.GetComponent<MoveController>();
			if(moveController != null)
			{
				moveController.TransitionTo(stateCommand);
			}

			MountComponent mountComponent = _animator.gameObject.GetComponent<MountComponent>();
			if (mountComponent != null)
			{
				mountComponent.SetDesiredState(stateCommand);
			}
		}
	}

	public bool IsDesiredState(MoveController.CombatantMoveState stateCommand)
	{
		if(_animator == null)
		{
			return false;
		}
		return _animator.GetInteger("State") == (int)stateCommand;
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
	
	public void CrossFade(eCampaignCharacterAnimationState state, float transitionDuration, int layer = 0)
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

	public bool IsInCharacterState(params eCampaignCharacterAnimationState[] states)
	{
		return IsInCharacterState(0, states);
	}
	
	public bool IsInCharacterState(int layer, eCampaignCharacterAnimationState[] states)
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
