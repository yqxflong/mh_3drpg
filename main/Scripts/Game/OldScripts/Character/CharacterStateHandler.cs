///////////////////////////////////////////////////////////////////////
//
//  CharacterStateHandler.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////
using UnityEngine;

public class CharacterStateHandlerAttribute : System.Attribute
{
	public eCampaignCharacterState handledState;

	public CharacterStateHandlerAttribute(eCampaignCharacterState stateToHandle)
	{
		handledState = stateToHandle;
	}
}

public abstract class CharacterStateHandler : IPoolable
{
	public Controller Controller
	{
		get;
		set;
	}

	public CampaignMoveController MoveController
	{
		get;
		set;
	}

	public ILocomotionComponent Locomotion
	{
		get;
		set;
	}

	public CombatController Combat
	{
		get;
		set;
	}

	public ReplicationView ViewRPC
	{
		get;
		set;
	}

	//public EffectReceiver EffectReceiver
	//{
	//	get;
	//	set;
	//}

	public ConditionComponent Conditions
	{
		get;
		set;
	}

	public CharacterComponent Character
	{
		get;
		set;
	}

	public CharacterTargetingComponent Targeting
	{
		get;
		set;
	}

	public bool IsPlayer
	{
		get;
		private set;
	}

	public bool CachedIsInRange
	{ //Use this for checking range when it's not super important
		get
		{
			if (_isRangeCacheValid)
			{
				return _cachedIsInRange;
			}
			return IsInRangeOfTarget();
		}
	}

	public bool IsFinished
	{
		get;
		set;
	}

	protected bool _canInterruptMove = true;
	protected bool _shouldMoveInterrupt = false;
	protected Vector3 _moveInterruptPosition;
	private float _previousMaxSpeed;
	private bool _cachedIsInRange = false;
	private bool _isRangeCacheValid = false;

	public CharacterStateHandler()
	{
	
	}

	public void OnPoolActivate()
	{
		Reset();
	}

	public void OnPoolDeactivate()
	{

	}

	protected virtual void Reset() //Clear State
	{
		Controller = null;
		MoveController = null;

		Locomotion = null;
		Combat = null;
		ViewRPC = null;
		//EffectReceiver = null;
		Conditions = null;
		Character = null;
		Targeting = null;
		IsPlayer = false;
		IsFinished = false;
		_canInterruptMove = true;
		_shouldMoveInterrupt = false;
		_moveInterruptPosition = default(Vector3);
		_previousMaxSpeed = 0f;
		_cachedIsInRange = false;
		_isRangeCacheValid = false;
	}

	public virtual void Begin(eCampaignCharacterState previousState)
	{
		if (Character != null)
		{
			_previousMaxSpeed = Character.Model.speed;
		}

		IsPlayer = (Controller is PlayerController);
	}

	public virtual void End(eCampaignCharacterState newState)
	{
		IsFinished = true;
	}

	public virtual void EarlyUpdate()
	{
		// Reset modifiers and update effects which can affect the component.
		// This could cause a state change, so it is in a separate update method.
	}

	public virtual void Update()
	{
		_isRangeCacheValid = false;

		if (null != Locomotion)
		{
			CheckMovementInterrupts();

			if (Character.UpdateSpeedEachFrame)
			{
				// Update max speed in the navigation agent
				Locomotion.MaxSpeed = Character.Model.speed;
			}

			if (!Conditions.CanMove && Locomotion.MaxSpeed > 0.0f)
			{
				_previousMaxSpeed = Locomotion.MaxSpeed;
				Locomotion.MaxSpeed = 0.0f;
			}
			if (Conditions.CanMove && Locomotion.MaxSpeed == 0.0f)
			{
				Locomotion.MaxSpeed = _previousMaxSpeed;
			}

			if (Character.UseAdvancedLocomotionAnimation)
			{
				//UpdateRotationGoal(Locomotion.Destination);
			}
		}
	}

	public bool TryMove(Vector3 position)
	{
		if (!_canInterruptMove)
		{
			_moveInterruptPosition = position;
			_shouldMoveInterrupt = true;
			return false;
		}
		return true;
	}

	// puts us into the animation state we require, if character is able to make the transition right now
	public void SetDesiredAnimationState(global::MoveController.CombatantMoveState stateDesired)
	{
		if(MoveController != null)
		{
			MoveController.SetDesiredState(stateDesired);
		}
	}

	public void FaceToward(Vector3 pos, bool instant = false)
	{
		Transform transform = Character.transform;

		// Make the character face the target
		Vector3 facingDirection = (pos - transform.position);
		facingDirection.y = 0f;
		facingDirection = facingDirection.normalized;

		Quaternion lookAt = Quaternion.LookRotation(facingDirection);
		float speed = Locomotion.AngularSpeed;
		if (!Conditions.CanMove)
		{
			speed = Character.Model.speed;
		}

		transform.rotation = instant ? lookAt : Quaternion.Lerp(transform.rotation, lookAt, Time.deltaTime * speed);		
	}

	public void UpdateRotationGoal(Vector3 pos)
	{
		Transform transform = Character.transform;
		Vector3 facingDirection = (pos - transform.position).normalized;
		facingDirection.y = 0f;
		//Quaternion lookAt = Quaternion.LookRotation(facingDirection);
		//float distance = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, lookAt.eulerAngles.y);
		
		Locomotion.AngularSpeed = Character.Model.speed;
	}

	public virtual void IssueMoveCommand(Vector3 position)
	{
		if ((Character.transform.position - position).sqrMagnitude >= Locomotion.ArrivalThreshold)
		{
			if (Character.State != eCampaignCharacterState.Move)
			{
				SetState(eCampaignCharacterState.Move);
			}
            Locomotion.Destination = position;

            Character.LastMoveCommandTimestamp = Time.time;
		}
	}

	public virtual void IssueAttackCommand(GameObject target)
	{
		if (Character.State != eCampaignCharacterState.AttackTarget)
		{
			if (Character.State != eCampaignCharacterState.ChaseTarget)
			{
				SetState(eCampaignCharacterState.ChaseTarget);
			}
		}
	}

	public bool IsInRangeOfTarget()
	{
		GameObject target = Targeting.AttackTarget;
		//BaseStatsComponent targetStats = target.GetComponent<BaseStatsComponent>();
		bool inRange = false;
		
		//bool isEnemy = Stats.Team.IsEnemy(targetStats.Team);


		CharacterComponent target_character = target.GetComponent<CharacterComponent>();
		bool isEnemy = target_character != null && Character.Model.team.IsEnemy(target_character.Model.team);

		if(isEnemy)
		{
			inRange = Combat.IsInRange(target);
			if (inRange && Combat.IsRanged)
			{
				inRange = Combat.HasLineOfSight(target.transform.position, target, true);
			}
		}
		else
		{
			IInteractable interactableStats = (IInteractable)target.GetComponent(typeof(IInteractable));
			if (interactableStats != null)
			{
				inRange = interactableStats.IsInRange(Character.transform);
			}
			else if (target.GetComponent<LTCameraTrigger>())
			{
				inRange = GameUtils.GetDistXZ(Character.transform.position, target.transform.position) < target.GetComponent<LTCameraTrigger>().stopDist;				
			}
		}
		_cachedIsInRange = inRange;
		_isRangeCacheValid = true;
		return inRange;
	}

	protected bool CheckMovementInterrupts()
	{
		if (_shouldMoveInterrupt && _canInterruptMove)
		{
			Character.IssueMoveCommand(_moveInterruptPosition);
			_shouldMoveInterrupt = false;
			return true;
		}
		return false;
	}

	protected void SetState(eCampaignCharacterState state)
	{
		Character.State = state;
	}

	protected bool IsCharacterMoving()
	{
		const float MovingTolSqr = 0.01f * 0.01f;
		return Locomotion.Velocity.sqrMagnitude > MovingTolSqr;
	}
}
