///////////////////////////////////////////////////////////////////////
//
//  CharacterComponent.cs
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
using Pathfinding.RVO;

public class CharacterComponent : BaseComponent
{
	public ILocomotionComponent LocomotionComponent
	{
		get
		{
			return _locomotionComponent;
		}
	}

	private eCampaignCharacterState _state;
	private Animator _animator;
	private Controller _controller;
	private CampaignMoveController _moveController;
	private ILocomotionComponent _locomotionComponent;
	private CombatController _combatController;
	private ReplicationView _viewRPC;
	private CharacterModel _characterModel;
	//private EffectReceiver _effectReceiver;
	private ConditionComponent _conditionComponent;
	private CharacterTargetingComponent _targetingComponent;

	private CharacterStateHandler _currentHandler;
	private static Dictionary<eCampaignCharacterState, System.Type> _stateHandlers;

	public CharacterModel Model
	{
		get
		{
			return _characterModel;
		}
	}

	// do we want to set the speed each frame
	public bool UpdateSpeedEachFrame { get; set; } = true;

	public eCampaignCharacterState State
	{
		get
		{
			return _state;
		}
		set
		{
			ChangeState(value);
		}
	}

	public CampaignMoveController MoveController
	{
		get
		{
			return _moveController;
		}
	}

	public CharacterStateHandler CurrentHandler
	{
		get
		{
			return _currentHandler;
		}
	}

	public CombatController CombatController
	{
		get
		{
			return _combatController;
		}
	}

	public CharacterTargetingComponent TargetingComponent
	{
		get
		{
			return _targetingComponent;
		}
	}

	public bool UseAdvancedLocomotionAnimation
	{
		get; set;
	}

	public float LastMoveCommandTimestamp
	{
		get; set;
	}

	void Awake()
	{
		if (_stateHandlers == null) 
		{
			_stateHandlers = new Dictionary<eCampaignCharacterState, System.Type>();

			//ToDo:���÷��䵼��������ù�ϵ���ײ鿴
			//Assembly assembly = Assembly.GetAssembly(typeof(CharacterStateHandler));
			//foreach (System.Type t in assembly.GetTypes())
			//{
			//	if (!t.IsAbstract && t.IsSubclassOf(typeof(CharacterStateHandler)))
			//	{
			//		CharacterStateHandlerAttribute attr = (CharacterStateHandlerAttribute)t.GetCustomAttributes(typeof(CharacterStateHandlerAttribute), false)[0];
			//		_stateHandlers.Add(attr.handledState, t);
			//	}
			//}

			var type = typeof(AttackTargetStateHandler);
			_stateHandlers.Add(((CharacterStateHandlerAttribute)type.GetCustomAttributes(typeof(CharacterStateHandlerAttribute), false)[0]).handledState, type);
			type = typeof(ChaseTargetStateHandler);
			_stateHandlers.Add(((CharacterStateHandlerAttribute)type.GetCustomAttributes(typeof(CharacterStateHandlerAttribute), false)[0]).handledState, type);
			type = typeof(IdleStateHandler);
			_stateHandlers.Add(((CharacterStateHandlerAttribute)type.GetCustomAttributes(typeof(CharacterStateHandlerAttribute), false)[0]).handledState, type);
			type = typeof(InteractStateHandler);
			_stateHandlers.Add(((CharacterStateHandlerAttribute)type.GetCustomAttributes(typeof(CharacterStateHandlerAttribute), false)[0]).handledState, type);
			type = typeof(MoveStateHandler);
			_stateHandlers.Add(((CharacterStateHandlerAttribute)type.GetCustomAttributes(typeof(CharacterStateHandlerAttribute), false)[0]).handledState, type);
		}

		_animator = GetComponentInChildren<Animator>();

		_controller = GetComponent<Controller>();
		if (_controller is EnemyController)
		{
			((EnemyController)_controller).onDestroy += OnEnemyKilled;
		}

		_combatController = GetComponent<CombatController>();
		_viewRPC = FindReplicationViewForComponent<CharacterComponent>();

		//_effectReceiver = new EffectReceiver(gameObject);

		_conditionComponent = GetComponent<ConditionComponent>();
		_targetingComponent = GetComponent<CharacterTargetingComponent>();
		//_animationController = GetComponent<AnimationController>();
	
		_moveController = GetComponent<CampaignMoveController>();

		_locomotionComponent = (ILocomotionComponent)GetComponent(typeof(ILocomotionComponent));
		if (_locomotionComponent != null)
		{
			_locomotionComponent.Initialize();
		}
		//_network = GetComponent<NetworkOwnershipComponent>();
	}

	public void OnSpawn(CharacterModel model, GameObject skinnedRigPrefabInstance, int spawnAnimationIndex, bool playSpawnEffect)
	{
		// For EnemyControllers, when Awake is called the child prefab containing the animator hasn't been set
		// So we need this special method here to initialize the reference to the animator once the full
		// initialization has been done
		_animator = skinnedRigPrefabInstance.GetComponent<Animator>();
		if (_animator == null)
		{
#if DEBUG
			DebugSystem.Log("Failed to find animator on rigged prefab: searching for animator in children", "CharacterComponent", LogType.Warning);
#endif
			_animator = skinnedRigPrefabInstance.GetComponentInChildren<Animator>();
		}

		//_animationController.Initialize(_animator, typeof(eCharacterAnimationState), typeof(eCharacterAnimationParameter));
		//_animationController.onAnimationEventBegin += OnAnimationEventBegin;

		_moveController.Initialize(_animator);

		_conditionComponent.SetAnimator(_animator);

		_characterModel = model;

		if (_combatController != null)
		{
			_combatController.SetAutoAttack(_characterModel.autoAttack);
		}

		if (_characterModel.team == eTeamId.Interactable)
		{
			if (GetComponent<InteractableComponent>() == null)
			{
				gameObject.AddComponent<InteractableComponent>();
			}
		}
		else if(_characterModel.team == eTeamId.Interactable)
		{
			gameObject.layer = LayerMask.NameToLayer("Enemy");
		}

		//GetComponent<Collider>().enabled = true;
		
		if (_locomotionComponent != null)
		{
			_locomotionComponent.MaxSpeed = model.speed;
			_locomotionComponent.Enabled = true;
		}

		RVOController rvoController = GetComponent<RVOController>();
		if (rvoController != null)
		{
			rvoController.radius = model.rvoRadius;
		}

		//EventManager.instance.Raise(new OnSpawnEvent(gameObject));

		if (playSpawnEffect)// && _characterModel.characterClass.onSpawnEffect != null)
		{
			//EffectContext newContext = EffectContext.Create();
			//newContext.Initialize(gameObject, gameObject);
			//_characterModel.characterClass.onSpawnEffect.CreateAndCast(gameObject, gameObject, newContext);
		}

        //如果没有寻路目标，才需初始化转换动作状态ChangeState
        if (State== eCampaignCharacterState.Idle) ChangeState(eCampaignCharacterState.Idle);


		if (GetComponent<Collider>() != null && GetComponent<Collider>() is BoxCollider)
		{
			BoxCollider box = GetComponent<Collider>() as BoxCollider;
			//box.size = new Vector3(1.75f, 1.75f, 1.75f) * _characterModel.CharacterRadiusNormalized + new Vector3(0, _characterModel.heightOffset, 0);
			//这里暂时把box的大小设置为定值， 因为_characterModel.CharacterRadiusNormalized 目前�?  为了人物之间没有碰撞  后期可以改动为另一个变量来设置合体大小
			//lzt-2015-7-21
			if(GetComponent<PlayerController>()!=null)
				box.size = new Vector3(1.5f, 1.5f, 1.5f) + new Vector3(0, _characterModel.heightOffset, 0);
			else if(GetComponent<EnemyController>()!=null)
				box.size = new Vector3(2f, 2f, 2f) + new Vector3(0, _characterModel.heightOffset, 0);
			box.center = new Vector3(0, box.size.y / 2.0f, 0);
		}
	}

	private void ChangeState(eCampaignCharacterState newState, bool ignoreDeath = false)
	{
		
		eCampaignCharacterState previousState = _state;

		_state = newState;

		if (_currentHandler != null && !_currentHandler.IsFinished)
		{
			_currentHandler.End(newState);

			CodeObjectManager.GetCharacterStateManager(_currentHandler.GetType()).Destroy(_currentHandler);
		}

		System.Type handlerType = _stateHandlers[State];

		_currentHandler = (CharacterStateHandler)CodeObjectManager.GetCharacterStateManager(handlerType).GetNext();

		_currentHandler.Controller = _controller;
		_currentHandler.MoveController = _moveController;
		//_currentHandler.Stats = _statsComponent;
		_currentHandler.Locomotion = LocomotionComponent;
		_currentHandler.Combat = _combatController;
		_currentHandler.ViewRPC = _viewRPC;
		//_currentHandler.EffectReceiver = _effectReceiver;
		_currentHandler.Conditions = _conditionComponent;
		_currentHandler.Targeting = _targetingComponent;
		_currentHandler.Character = this;

		_currentHandler.Begin(previousState);

		_moveController.SetPlaybackSpeed(1.0f);
	}

	public T GetStateHandler<T>() where T : CharacterStateHandler
	{
		if (_currentHandler is T) 
		{
			return (T)_currentHandler;
		}
		return null;
	}

	void OnEnable()
	{
		CharacterComponentManager.sCharacterComponents.Add(this);

		//if (_effectReceiver != null)
		//{
		//	_effectReceiver.Clear();
		//}

		if(_animator != null)
		{
			ChangeState(eCampaignCharacterState.Idle);
		}
	}

	void OnDisable()
	{
		CharacterComponentManager.sCharacterComponents.Remove(this);

		if (_currentHandler != null)
		{
			_currentHandler.End(eCampaignCharacterState.Idle);
			CodeObjectManager.GetCharacterStateManager(_currentHandler.GetType()).Destroy(_currentHandler);
			_currentHandler = null;
		}
	}

	void Update()
	{
		if (_currentHandler != null && !_currentHandler.IsFinished)
		{
			_currentHandler.EarlyUpdate();
			_currentHandler.Update();
		}
	}

	public void IssueMoveCommand(Vector3 position)
	{
		if ((_currentHandler != null) && _currentHandler.TryMove(position))
		{
			_currentHandler.IssueMoveCommand(position);
		}
	}

	public void IssueAttackCommand(GameObject target)
	{
		if (_currentHandler != null)
		{
			_currentHandler.IssueAttackCommand(target);	
		}
	}

	public void Stop()
	{
		if (State != eCampaignCharacterState.Idle)
		{
			State = eCampaignCharacterState.Idle;
			LocomotionComponent.Stop();
		}
	}

	public AutoAttackModel StartChargeAttack(GameObject target, ChargeAttackInputHandler inputHandler)
	{
		ReplicationView view = target.GetComponent<ReplicationView>();
		StartChargeAttackRPC(view.viewId);
		_viewRPC.RPC("StartChargeAttackRPC", EB.RPCMode.Others, view.viewId);

		return _combatController.GetCurrentAttack();
	}

	public void FireChargeAttack(float heldAmount) 
	{
		FireChargeAttackRPC(heldAmount);
		_viewRPC.RPC("FireChargeAttackRPC", EB.RPCMode.Others, heldAmount);
	}

	public void FaceToward(Vector3 pos, bool instant = false)
	{
		_currentHandler.FaceToward(pos, instant);
	}

	//IEffectReceiverComponent
	//public EffectReceiver GetEffectReceiver()
	//{
	//	return _effectReceiver;
	//}

	void OnEnemyKilled(EnemyController destroyed)
	{
		//if (_effectReceiver != null)
		//{
		//	_effectReceiver.Clear();
		//}
	}

	//[RPC]
	private void StartChargeAttackRPC(EB.Replication.ViewId viewId)
	{
	}

	//[RPC]
	private void FireChargeAttackRPC(float timeHeld) 
	{
	}
}
