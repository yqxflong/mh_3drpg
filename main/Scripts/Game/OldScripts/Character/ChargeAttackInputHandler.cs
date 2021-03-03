///////////////////////////////////////////////////////////////////////
//
//  ChargeAttackInputHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class ChargeAttackInputHandler : IPlayerInputHandler
{
	public bool ShouldBeRemoved 
	{
		get 
		{
			return _finished;
		}
	}

	public float TimeHeld
	{	
		get
		{
			return _timeHeld;
		}
	}

	private bool _finished;
	private PlayerController _playerController;
	private CharacterTargetingComponent _targeting;
	private float _timeHeld = 0f;
	private bool _chargeStarted = false;

	private AutoAttackModel _attack;

	public void Start(PlayerController controller)
	{
		_playerController = controller;
		_targeting = controller.GetComponent<CharacterTargetingComponent>();
		BeginCharging();
	}

	public bool HandleInputEvent(GameEvent e)
	{
		if (e is TouchEndEvent)
		{
			if (_chargeStarted)
			{
				EndCharging();
			}
			_finished = true;
		}
		return _chargeStarted;
	}

	public void Update()
	{
		_timeHeld += Time.deltaTime;

		if (_chargeStarted && _attack.fireAtMaxTime && _timeHeld > _attack.maxChargeTime)
		{
			EndCharging();
		}
	}

	public void End()
	{
		
	}

	public void OnAbilityEvent(eEffectEvent eventType)
	{
		
	}
	
	private void BeginCharging() 
	{
		if (_targeting.AttackTarget == null)
		{
			_timeHeld = 0f;
			return;
		}
		_chargeStarted = true;
		_attack = _playerController.CharacterComponent.StartChargeAttack(_targeting.AttackTarget, this);
	}

	private void EndCharging()
	{
		_playerController.CharacterComponent.FireChargeAttack(TimeHeld);
	}
}