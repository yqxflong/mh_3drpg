///////////////////////////////////////////////////////////////////////
//
//  ChaseTargetStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[CharacterStateHandler(eCampaignCharacterState.ChaseTarget)]
public class ChaseTargetStateHandler : CharacterStateHandler 
{
	private const float MinDistForRepathSq = 1;

	private Vector3 m_ChaseStartLocation = Vector3.zero;
	protected override void Reset() //Clear State
	{
		base.Reset();
	}

	public override void Begin(eCampaignCharacterState previousState) 
	{
		base.Begin(previousState);

		SetDesiredAnimationState(global::MoveController.CombatantMoveState.kLocomotion);

		if (Targeting == null)
		{
			return;
		}

		GameObject target = Targeting.AttackTarget;

		if (target == null)
		{
			SetState(eCampaignCharacterState.Idle);
			return;
		}
		
		//BaseStatsComponent targetStats = target.GetComponent<BaseStatsComponent>();
		bool is_interactable = false;
		CharacterComponent targetCharacter = target.GetComponent<CharacterComponent>();
		is_interactable = targetCharacter != null && targetCharacter.Model.team == eTeamId.Interactable;
		bool is_area = target.GetComponent<LTCameraTrigger>() != null;

		if (is_interactable|| is_area)
		{
			Locomotion.Destination = target.transform.position;
			// Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, Locomotion.Destination);
			//GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "PlayerMoveSyncManagerMove", Locomotion.Destination);
		}
		else
		{
			IInteractable interactableStats = (IInteractable)target.GetComponent(typeof(IInteractable));
			if(interactableStats != null && interactableStats.Interactable != null)
			{
				Locomotion.Destination = interactableStats.Interactable.GetClosestInteractionPoint(Character.transform.position);
			}
		}

		if(Controller != null)
		{
			m_ChaseStartLocation = Controller.transform.position;
		}
	}

	public override void Update() 
	{
		base.Update();

		if (Targeting == null)
		{
			return;
		}

		GameObject target = Targeting.AttackTarget;

		if (target == null)
		{
			if (Locomotion != null)
			{
				Locomotion.Stop();
			}

			SetState(eCampaignCharacterState.Idle);
			return;
		}

		if(!IsPlayer && Controller != null)
		{
			float chase_dist = Vector3.Distance(Controller.transform.position, m_ChaseStartLocation);
			if(chase_dist > Controller.ChaseDistance)
			{
				//Targeting.SetAttackTarget(null);
				IssueMoveCommand(Controller.SpawnLocation);
				return;
			}
		}

		//BaseStatsComponent targetStats = target.GetComponent<BaseStatsComponent>();
		CharacterComponent targetCharacter = target.GetComponent<CharacterComponent>();
		bool is_interatable = (targetCharacter != null && targetCharacter.Model.team == eTeamId.Interactable); 
        bool is_otherplayer= targetCharacter != null && targetCharacter.Model.team == eTeamId.Player;
        bool is_enemy = targetCharacter != null && Character.Model.team.IsEnemy(targetCharacter.Model.team);
		bool is_area = target.GetComponent<LTCameraTrigger>() != null;

		if (!IsInRangeOfTarget() && Conditions != null && Conditions.CanMove)
		{
			// Check if the target has moved and we need to repath
			Vector3 targetLocation = Controller.gameObject.transform.position;
			if (!is_interatable|| is_area)
			{
				targetLocation = target.transform.position;	
			}
			else
			{
				IInteractable interactableStats = (IInteractable)target.GetComponent(typeof(IInteractable));
				if(interactableStats != null && interactableStats.Interactable != null)
				{
					targetLocation = interactableStats.Interactable.GetClosestInteractionPoint(Character.transform.position);
				}
			}

			float distSq = GameUtils.GetDistSqXZ(Locomotion.Destination, targetLocation);
			if (distSq > MinDistForRepathSq)
			{
				Locomotion.Destination = targetLocation;
                if (IsPlayer)
                {
					// Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, Controller.transform.position);		
					//GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "PlayerMoveSyncManagerMove", Controller.transform.position);
				} 
			}

			//float currentSpeed = Locomotion.Velocity.magnitude;
		}
		else
		{
			Locomotion.Stop();
            if (IsPlayer)
            {
                float distSq = GameUtils.GetDistSqXZ(Locomotion.Destination, (Controller as PlayerController).transform.position);
                if(distSq > MinDistForRepathSq)
                {
					// Hotfix_LT.Messenger.Raise(Hotfix_LT.EventName.PlayerMoveSyncManagerMove, Locomotion.Destination);		
					//GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "PlayerMoveSyncManagerMove", Locomotion.Destination);			
				}
            } 
			if (is_enemy)
			{
				//FOR GAM: No attack in campaign view. change to Idle state
				//SetState(eCampaignCharacterState.AttackTarget);
				SetState(eCampaignCharacterState.Idle);
			}
			else if (is_interatable|| is_area)
			{
				IInteractable interactableStats = (IInteractable)target.GetComponent(typeof(IInteractable));

				if (interactableStats != null && interactableStats.Interactable.deselectOnInteract)
				{
					Targeting.SetAttackTarget(null);
				}
				//SetState(eCampaignCharacterState.Idle);
				if (interactableStats != null)
				{
					interactableStats.Interact(Character.gameObject);
				}

				if (Character.State != eCampaignCharacterState.Interact)
				{
					SetState(eCampaignCharacterState.Idle);
				}
			}
            else if(is_otherplayer)
            {
                IInteractable interactableStats = (IInteractable)target.GetComponent(typeof(IInteractable));

                if (interactableStats != null && interactableStats.Interactable.deselectOnInteract)
                {
                    Targeting.SetAttackTarget(null);
                }
				//SetState(eCampaignCharacterState.Idle);
				if (interactableStats != null)
				{
					interactableStats.Interact(Character.gameObject);
				}

                if (Character.State != eCampaignCharacterState.Interact)
                {
                    SetState(eCampaignCharacterState.Idle);
                }
            }
		}


		_canInterruptMove = true;
	}

	public override void End(eCampaignCharacterState newState)
	{
		base.End(newState);
	}
}
