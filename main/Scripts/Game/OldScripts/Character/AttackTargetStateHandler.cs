///////////////////////////////////////////////////////////////////////
//
//  AttackTargetStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[CharacterStateHandler(eCampaignCharacterState.AttackTarget)]
public class AttackTargetStateHandler : CharacterStateHandler 
{
	private bool _shouldFaceTarget = false;

	private const float BLEND_SPEED_STRAFE = 5f;
	private const float BLEND_SPEED_IDLE = 5f;

	private int _attackLayer = -1;
	private bool _hasStartedFirstAttack = false;

	private bool DEBUG_MOVE_IN_PLACE = false;

	private bool IsPlayingAttack
	{
		get 
		{
			return MoveController.IsInCharacterState(eCampaignCharacterAnimationState.Attack);//, eCharacterAnimationState.StrafeAttack);
		}
	}

	protected override void Reset() //Clear State
	{
		base.Reset();
		_shouldFaceTarget = false;
		_attackLayer = -1;
		_hasStartedFirstAttack = false;
		DEBUG_MOVE_IN_PLACE = false;
	}

	public override void Begin(eCampaignCharacterState previousState) 
	{
		base.Begin(previousState);

		GameObject target = Targeting.AttackTarget;
		if (target != null)
		{
			CharacterComponent targetCharacter = target.GetComponent<CharacterComponent>();
			if(targetCharacter != null && targetCharacter.Model != null && targetCharacter.Model.team == eTeamId.Interactable)
			{
				return;
			}
		}

		if (!IsPlayingAttack || previousState != eCampaignCharacterState.AttackTarget)
		{
			Combat.ResetAttackSequence();
			RefreshComboAttack();
			_hasStartedFirstAttack = false;

			if (!Combat.CanMoveAndAttack)
			{
				_canInterruptMove = false;
			}
		}
		Locomotion.ShouldIgnoreRotation = true;

		SetDesiredAnimationState(global::MoveController.CombatantMoveState.kAttackTarget);
	}

	public override void Update() 
	{
		base.Update();

		GameObject target = Targeting.AttackTarget;

		if (target == null || Conditions.CanUseCombat == false)
		{
			SetState(eCampaignCharacterState.Idle);
			return;
		}

		//eTeamId team = target.GetComponent<BaseStatsComponent>().Team;

		bool is_interactable = false;
		CharacterComponent target_character = target.GetComponent<CharacterComponent>();
		is_interactable = target_character != null && target_character.Model.team == eTeamId.Interactable;

		if (is_interactable)
		{	
			SetState(eCampaignCharacterState.ChaseTarget);
			return;
		}

		bool inRange = IsInRangeOfTarget();

		if (Combat.CanMoveAndAttack)
		{
			UpdateMovingAttack(inRange);
		}
		else
		{
			UpdateStandingAttack(inRange);
		}

		ApplyAttackSpeed();
	}

	public override void End(eCampaignCharacterState newState)
	{
		base.End(newState);	

		if (newState != eCampaignCharacterState.AttackTarget) 
		{
			Locomotion.ShouldIgnoreRotation = false;
			Locomotion.MaxSpeed = Character.Model.speed;


			Combat.CancelAttack();
			if (Combat.CanMoveAndAttack)
			{
				//AnimController.SetFloat(eCharacterAnimationParameter.Attacking, 0.0f);
			}

			MoveController.SetPlaybackSpeed(1.0f);
		}
	}


	public override void IssueMoveCommand(Vector3 position)
	{
		if (Combat.CanMoveAndAttack)
		{
			Locomotion.Destination = position;
		}
		else
		{
			base.IssueMoveCommand(position);
		}
	}

	private void RefreshComboAttack()
	{
		_shouldFaceTarget = true;
	}

	private void ChooseChaseState()
	{
		if (Conditions.CanMove && Targeting.AttackTarget != null)
		{
			if (Controller is EnemyController)
			{
				if (MoveController.IsInCharacterState(eCampaignCharacterAnimationState.Attack) && !MoveController.HasAnimationFinished())
				{
					return;
				}

				SetState(eCampaignCharacterState.ChaseTarget);
			}
			else
			{
				SetState(eCampaignCharacterState.Idle);
			}
		}
	}

	private void UpdateStandingAttack(bool inRange)
	{
		if (!inRange)
		{
			ChooseChaseState();
			return;
		}

		GameObject target = Targeting.AttackTarget;

		if (!_hasStartedFirstAttack && IsInRangeOfTarget())
		{
			if (!MoveController.IsInTransition())
			{
				Combat.StartAttack(Targeting.AttackTarget);
				_hasStartedFirstAttack = true;
			}
		}

		if (Combat.CanInterruptCombo)
		{
			if (!Combat.HasAttackHit)
			{
				Combat.TryAttack(target);
			}
			Combat.IncrementAttackSequence();

			Combat.StartAttack(target);
			RefreshComboAttack();
			//AnimController.RestartCurrentAnimation();

			_canInterruptMove = true;
			if (CheckMovementInterrupts())
			{
				return;
			}
			_canInterruptMove = false;
		}
		else if ((MoveController.IsInCharacterState(eCampaignCharacterAnimationState.Attack) && MoveController.HasAnimationFinished() && !MoveController.IsInTransition()))
		{
			Combat.IncrementAttackSequence();

			Combat.StartAttack(target);
			RefreshComboAttack();
			MoveController.RestartCurrentAnimation();
		}
		//else if (MoveController.IsEventActive(eAnimationEvent.Hit))
		//{
			//Combat.TryAttack(target);
			//_shouldFaceTarget = false;
		//}

		if (_shouldFaceTarget && target != null)
		{
			FaceToward(target.transform.position);
		}
	}

	private void UpdateMovingAttack(bool inRange)
	{
		if (_attackLayer == -1)
		{
			_attackLayer = MoveController.GetLayerIndex("Attack Layer");
		}

		GameObject target = Targeting.AttackTarget;
		SetDesiredAnimationState(global::MoveController.CombatantMoveState.kAttackTarget);
		Locomotion.MaxSpeed = Character.Model.speed * Combat.AttackMovementSlowFactor;

		Vector3 direction = (target.transform.position - Character.transform.position).normalized;
		Vector3 normalizedVelocity = Locomotion.Velocity.normalized;
		//float speed = (Locomotion.CurrentSpeed > 0) ? 1.0f : 0.0f;

		if (DEBUG_MOVE_IN_PLACE)
		{
			normalizedVelocity = (Locomotion.Destination - Character.transform.position).normalized;
			//speed = 1.0f;
			Locomotion.MaxSpeed = 0.0f;	
		}

		float forward = Vector3.Dot(direction, normalizedVelocity);
		float lateral = -Vector3.Dot(Vector3.Cross(direction, Vector3.up), normalizedVelocity);

		if (Mathf.Abs(forward) > Mathf.Abs(lateral))
		{
			lateral = 0;
		}
		else
		{
			forward = 0;
		}

		float currentForward = 1.0f;//AnimController.GetFloat(eCharacterAnimationParameter.ForwardVelocity);
		float currentLateral = 1.0f;//AnimController.GetFloat(eCharacterAnimationParameter.LateralVelocity);

		forward = DoTweenUtils.FloatUpdate(currentForward, forward, BLEND_SPEED_STRAFE);
		lateral = DoTweenUtils.FloatUpdate(currentLateral, lateral, BLEND_SPEED_STRAFE);

		if (Mathf.Abs(forward) > .9f) {
			forward = Mathf.Sign(forward);
		}
		if (Mathf.Abs(lateral) > .9f) {
			lateral = Mathf.Sign(lateral);
		}

		//AnimController.SetFloat(eCharacterAnimationParameter.Speed, speed);
		//AnimController.SetFloat(eCharacterAnimationParameter.ForwardVelocity, forward);
		//AnimController.SetFloat(eCharacterAnimationParameter.LateralVelocity, lateral);

		if (target != null)
		{
			FaceToward(target.transform.position, DEBUG_MOVE_IN_PLACE);
		}

		if (inRange)
		{
			MoveAttack(target);
			MoveController.SetLayerWeight(_attackLayer, 1.0f);
		}
		else
		{
			//AnimController.SetFloat(eCharacterAnimationParameter.Attacking, 0.0f);
			MoveController.SetLayerWeight(_attackLayer, 0.0f);
		}

		Combat.AnimationLayer = _attackLayer;
	}

	private void MoveAttack(GameObject target)
	{
		if (target == null)
			return;

		float normalizedTime = MoveController.GetNormalizedTime(_attackLayer);

		bool isHitActive = (normalizedTime % 1) > Combat.GetCurrentAttackEvent(eAnimationEvent.Hit, _attackLayer);
		if (Combat.HasAttackHit && !isHitActive)
		{
			Combat.IncrementAttackSequence();
			RefreshComboAttack();
			Combat.StartAttack(target);
		}

		//AnimController.SetFloat(eCharacterAnimationParameter.Attacking, 1.0f);

		if (isHitActive)
		{
			Combat.TryAttack(target);
		}
	}

	private void ApplyAttackSpeed()
	{
		if (Combat.CanMoveAndAttack)
		{
			//AnimController.SetFloat(eCharacterAnimationParameter.AttackAnimSpeed, 1.0f);//Stats.GetAttackRate());
		}
		else
		{
			MoveController.SetPlaybackSpeed(1.0f);//Stats.GetAttackRate());
		}
	}
}
