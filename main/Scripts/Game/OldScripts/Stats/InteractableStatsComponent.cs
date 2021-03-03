///////////////////////////////////////////////////////////////////////
//
//  InteractableStatsComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

public class InteractableStatsComponent : BaseStatsComponent, IInteractable
{
	private ReplicationView _viewRPC;
	private InteractableComponent _interactable;
	private bool _shouldOutline = true;
	private float _lastInteractTimestamp;



	public override bool IsAlive 
	{
		get 
		{
			if (_interactable == null)
			{
				EB.Debug.LogError("Object should not have an interactable stat component without an interactable component.");
				return false;
			}
			return _interactable.IsAlive();
		}
	}

	public override eTeamId Team 
	{
		get 
		{
			return eTeamId.Interactable;
		}
		set
		{

		}
	}

	public override bool IsCountedAsTarget
	{
		get 
		{
			return false;
		}
	}

	public override bool CanReceivePrefabEffects
	{
		get 
		{
			return false;
		}
	}

	public override ReplicationView ReplicationView
	{
		get 
		{
			return _viewRPC;
		}
	}

	public float InteractionRangeSq
	{
		get
		{
			return Interactable.interactionRange * Interactable.interactionRange;
		}
	}

	public InteractableComponent Interactable
	{
		get 
		{
			return _interactable;
		}
	}

	public bool ShouldOutline
	{
		get 
		{
			return _shouldOutline;
		}
		set 
		{
			_shouldOutline = value;
		}
	}

	void Awake()
	{
		_interactable = GetComponent<InteractableComponent>();
	}

	void Start()
	{
		if (_interactable == null)
		{
			_interactable = GetComponent<InteractableComponent>();
		}
		
		if (_interactable == null)
		{
			EB.Debug.LogError("InteractableStatsComponent: _interactable is null");
		}

		LocalOnly = _interactable.localOnly;
		if (!LocalOnly)
		{
			_viewRPC = FindReplicationViewForComponent<InteractableStatsComponent>();
			if (_viewRPC == null)// && GameStateManager.HasSparxGame)
			{
				_viewRPC = gameObject.AddComponent<ReplicationView>();
				_viewRPC.observed = this;
			}
		}
	}


	public override void Heal(float amount, GameObject source)
	{

	}

	public override void ForceKill()
	{

	}



	public bool IsInRange(Transform character)
	{
		if (character == null || Interactable == null)
		{
			EB.Debug.LogError("IsInRange: character or interactable is null");
			return false;
		}
		return GameUtils.GetDistSqXZ(Interactable.GetClosestInteractionPoint(character.position), character.position) < InteractionRangeSq;
	}
    
	public void Interact(GameObject player)
	{
		if (Time.time - _lastInteractTimestamp < 0.5f)
		{
			return;
		}

		_lastInteractTimestamp = Time.time;
		InteractFromRPC(player);
		if (!LocalOnly && _viewRPC != null)
		{
			ReplicationView playerView = player.GetComponent<ReplicationView>();
			_viewRPC.RPC("InteractRPC", EB.RPCMode.Others, playerView.viewId);
		}
	}

	//[RPC]
	private void InteractRPC(EB.Replication.ViewId playerViewId)
	{
		GameObject player = Replication.GetObjectFromViewId(playerViewId);
		InteractFromRPC(player);
	}

	private void InteractFromRPC(GameObject player)
	{
		Interactable.Interact(player);
	}
}
