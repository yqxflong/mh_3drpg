///////////////////////////////////////////////////////////////////////
//
//  ConditionComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class ConditionComponent : BaseComponent
{
	public enum eCondition
	{
		Stun, 		// No abilities; no movement; no auto-attack
		Root, 		// No movement
		Fear, 		// No abilities; special movement; no auto-attack
		Silence, 	// No abilities
		Immune,		// Cannot be hit by any effects
		Interrupt,	// Temporarily interrupted,
		Frozen, 	// No abilities; no movement; no auto-attack; stun w/alternate visuals
	}

	//private EffectReceiver      _effectReceiver;

	private bool 				_canMove;
	private bool 				_canUseCombat;
	private bool 				_canUseAbilities;

	private bool 				_isStunned;
	private bool 				_isRooted;
	private bool 				_isAfraid;
	private bool 				_isSilenced;
	private bool				_isImmune;
	private bool				_isInterrupted;
	private bool				_isFrozen;

	//private Effect 				_conditionVFX;
	private eCondition 			_currentVFXCondition;

	private Animator 			_animator;
	private int 				_rootLayerIndex;
	private static string 		sRootLayerName = "Root";

	public bool CanUseCombat
	{
		get
		{
			return _canUseCombat;
		}
	}

	public bool CanMove
	{
		get
		{
			return _canMove;
		}
	}

	public bool IsStunned
	{
		get
		{
			return _isStunned;
		}
	}

	public bool IsRooted
	{
		get
		{
			return _isRooted;
		}
	}

	public bool IsAfraid
	{
		get
		{
			return _isAfraid;
		}
	}

	public bool IsSilenced
	{
		get
		{
			return _isSilenced;
		}
	}

	public bool IsImmune
	{
		get
		{
			return _isImmune;
		}
	}

	public bool IsFrozen
	{
		get
		{
			return _isFrozen;
		}
	}

	void Awake()
	{
		//_effectReceiver = new EffectReceiver(gameObject);
	}

	public void SetAnimator(Animator a)
	{
		_animator = a;
		_rootLayerIndex = -1;
		if (null != _animator)
		{
			for (int i = 0; i < _animator.layerCount; ++i)
			{
				if (sRootLayerName.Equals(_animator.GetLayerName(i)))
				{
					_rootLayerIndex = i;
					break;
				}
			}
		}
	}

	void Update()
    {
        if (!GameEngine.Instance.IsTimeToRootScene)
        {
            return;
        }
        // Reset modifiers and update effects which can affect the component.
  //      if (_effectReceiver != null)
		//{
		//	_effectReceiver.Update();
		//}
	}

	void LateUpdate()
    {
        if (!GameEngine.Instance.IsTimeToRootScene)
        {
            return;
        }
        UpdateFlags();
		UpdateVFX();
	}

	public void UpdateFlags()
	{
		_canMove = true;
		_canUseAbilities = true;
		_canUseCombat = true;
		_isStunned = false;
		_isRooted = false;
		_isAfraid = false;
		_isSilenced = false;
		_isImmune = false;
		_isInterrupted = false;
		_isFrozen = false;

		//List<Effect> effects = _effectReceiver.Effects;
		//for (int i = 0; i < effects.Count; ++i)
		//{
		//	Effect effect = effects[i];
		//	if (effect is EffectCondition && !effect.IsFinished && !effect.HasBeenStopped)
		//	{
		//		EffectCondition effectCondition = effect as EffectCondition;
		//		switch (effectCondition.Condition)
		//		{
		//			case eCondition.Stun:
		//				_canMove = false;
		//				_canUseCombat = false;
		//				_canUseAbilities = false;
		//				_isStunned = true;
		//				break;
		//			case eCondition.Root:
		//				_canMove = false;
		//				_isRooted = true;
		//				break;
		//			case eCondition.Fear:
		//				_canUseCombat = false;
		//				_canUseAbilities = false;
		//				_isAfraid = true;
		//				break;
		//			case eCondition.Silence:
		//				_canUseAbilities = false;
		//				_isSilenced = true;
		//				break;
		//			case eCondition.Immune:
		//				_isImmune = true;
		//				break;
		//			case eCondition.Interrupt:
		//				_isInterrupted = true;
		//				_canUseCombat = false;
		//				_canUseAbilities = false;
		//				break;
		//			case eCondition.Frozen:
		//				_isFrozen = true;
		//				_canMove = false;
		//				_canUseCombat = false;
		//				_canUseAbilities = false;
		//				break;
		//		}
		//	}
		//}
	}

	private void UpdateVFX()
	{
		if (IsAfraid)
		{
			if (_currentVFXCondition != eCondition.Fear)
			{
				SwitchVFX(eCondition.Fear);
			}
		}
		else if (IsStunned)
		{
			if (_currentVFXCondition != eCondition.Stun)
			{
				SwitchVFX(eCondition.Stun);
			}
		}
		else if (IsRooted)
		{
			if (_currentVFXCondition != eCondition.Root)
			{
				SwitchVFX(eCondition.Root);
			}
		}
		else if (IsSilenced)
		{
			if (_currentVFXCondition != eCondition.Silence)
			{
				SwitchVFX(eCondition.Silence);
			}
		}
		else if (IsFrozen)
		{
			if (_currentVFXCondition != eCondition.Frozen)
			{
				SwitchVFX(eCondition.Frozen);
			}
		}
		else
		{
			RemoveVFX();
		}

		if (null != _animator && -1 != _rootLayerIndex)
		{
			if (_canMove && !IsStunned)
			{
				//_animator.SetLayerWeight(_rootLayerIndex, 0f);
			}
			else
			{
				//_animator.SetLayerWeight(_rootLayerIndex, 1f);
			}
		}
	}

	private void SwitchVFX(eCondition condition)
	{
		_currentVFXCondition = condition;

		RemoveVFX();

		//EffectModel vfxEffectModel = GlobalBuffData.Instance.GetVFXModelForCondition(condition);
		//if (vfxEffectModel != null)
		//{
		//	EffectContext newContext = EffectContext.Create();
		//	newContext.Initialize(gameObject, gameObject);
		//	_conditionVFX = vfxEffectModel.CreateAndCast(gameObject, gameObject, newContext);
		//}
	}

	private void RemoveVFX()
	{
		//if (_conditionVFX != null)
		//{
		//	_conditionVFX.Remove();
		//	_conditionVFX = null;
		//}
	}

	//public EffectReceiver GetEffectReceiver()
	//{
	//	return _effectReceiver;
	//}

	public void Clear()
	{
		//if (_effectReceiver != null)
		//{
		//	_effectReceiver.Clear();
		//	UpdateFlags();
		//}
	}
}
