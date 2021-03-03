///////////////////////////////////////////////////////////////////////
//
//  IdleStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[CharacterStateHandler(eCampaignCharacterState.Idle)]
public class IdleStateHandler : CharacterStateHandler 
{
	protected override void Reset() //Clear State
	{
		base.Reset();
	}

	public override void Begin(eCampaignCharacterState previousState) 
	{
		SetDesiredAnimationState(global::MoveController.CombatantMoveState.kIdle);
		if(Character == null || Character.Model == null)
		{
			return;
		}

		string audioName = Character.Model.audioName;
		if (!string.IsNullOrEmpty(audioName))
		{
			FusionAudio.PostCharacterAnimation(audioName, "Idle", true);
		}
	}	

	public override void Update() 
	{
		base.Update();


		if (Targeting.AttackTarget != null)
		{
			if (Controller is EnemyController)
			{
				if(!IsInRangeOfTarget())
				{
					SetState(eCampaignCharacterState.ChaseTarget);
				}
			}
			//else if (IsInRangeOfTarget())
			//{
				//FOR GAM: No attack in campaign view. change to Idle state
				//SetState(eCampaignCharacterState.AttackTarget);
				//SetState(eCampaignCharacterState.Idle);
			//}
		}
	}

	public override void End(eCampaignCharacterState newState)
	{
		if(Character == null || Character.Model == null)
		{
			return;
		}

		string audioName = Character.Model.audioName;
		if (!string.IsNullOrEmpty(audioName))
		{
			FusionAudio.PostCharacterAnimation(audioName, "Idle", false);
		}
	}
}
