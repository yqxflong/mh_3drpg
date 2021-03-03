///////////////////////////////////////////////////////////////////////
//
//  MoveStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[CharacterStateHandler(eCampaignCharacterState.Move)]
public class MoveStateHandler : CharacterStateHandler 
{
	protected override void Reset() //Clear State
	{
		base.Reset();
	}

	public override void Begin(eCampaignCharacterState previousState) 
	{
		base.Begin(previousState);

		SetDesiredAnimationState(global::MoveController.CombatantMoveState.kLocomotion);

		string audioName = Character.Model.audioName;
		if (!string.IsNullOrEmpty(audioName))
		{
			FusionAudio.PostCharacterAnimation(audioName, "Run", true);
		}
	}

	public override void Update() 
	{
		base.Update();

		//If your movement target is basically inside your enemy target, switch to chase
		if (IsPlayer && Combat.IsMelee && Targeting.AttackTarget != null && Targeting.MovementTarget != null && ((Vector3)Targeting.MovementTarget - Targeting.AttackTarget.transform.position).sqrMagnitude < 4f)
		{
			SetState(eCampaignCharacterState.ChaseTarget);
			return;
		}

		_canInterruptMove = true;
		float remainingDistance = Locomotion.RemainingDistance;
		if (!Locomotion.PathPending && remainingDistance <= Locomotion.ArrivalThreshold)
		{
			//Targeting.SetMovementTarget(null);
			Targeting.StopMoveInDestination();
			if(!Targeting.MoveToNextPos())
				SetState(eCampaignCharacterState.Idle);					
		}

		if (Targeting.AttackTarget != null) 
		{
			if (Combat.CanMoveAndAttack)
			{
				//FOR GAM: No attack in campaign view. change to Idle state
				//SetState(eCampaignCharacterState.AttackTarget);
				SetState(eCampaignCharacterState.Idle);
			}
		}
	}

	public override void End(eCampaignCharacterState newState)
	{
		base.End(newState);

		string audioName = Character.Model.audioName;
		if (!string.IsNullOrEmpty(audioName))
		{
			FusionAudio.PostCharacterAnimation(audioName, "Run", false);
		}
	}
}
