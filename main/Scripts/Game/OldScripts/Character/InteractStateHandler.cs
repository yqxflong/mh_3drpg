///////////////////////////////////////////////////////////////////////
//
//  InteractStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[CharacterStateHandler(eCampaignCharacterState.Interact)]
public class InteractStateHandler : CharacterStateHandler 
{
	protected override void Reset() //Clear State
	{
		base.Reset();
	}

	public override void Begin(eCampaignCharacterState previousState) 
	{
		SetDesiredAnimationState(global::MoveController.CombatantMoveState.kIdle);
	}

	public override void Update() 
	{
		base.Update();

		if (MoveController.IsInCharacterState(eCampaignCharacterAnimationState.Interact))// && AnimController.IsEventActive(eAnimationEvent.Hit))
		{
			SetState(eCampaignCharacterState.Idle);
		}
	}

	public override void End(eCampaignCharacterState newState)
	{
	}

	private void OnAttackedBy(GameObject attacker)
	{
		SetState(eCampaignCharacterState.Idle);
	}
}
