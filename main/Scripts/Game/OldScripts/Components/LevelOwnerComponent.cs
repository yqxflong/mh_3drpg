///////////////////////////////////////////////////////////////////////
//
//  LevelOwnerComponent.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOwnerComponent : BaseComponent
{
#if DEBUG
	private NetworkDebugComponent _networkDebugger = null; 		
#endif

	private static List<NetworkOwnershipComponent> _networkOwnershipComponents = new List<NetworkOwnershipComponent>();

	public static void Register(NetworkOwnershipComponent component)
	{
		_networkOwnershipComponents.Add(component);
	}

	public static void Unregister(NetworkOwnershipComponent component)
	{
		_networkOwnershipComponents.Remove(component);
	}

	public static bool IsLevelOwner(EB.Sparx.Player player)
	{
		try{
			if (player == null)
			{
				return false;
			}

			return player.IsHost; // the host is also the level owner
		}
		catch(System.NullReferenceException e)
		{
			EB.Debug.LogError(e.ToString());
			return false;
		}
	}

	public static void AssignLevelOwner(GameObject player)
	{
		//EB.Debug.LogWarning("LevelOwnerComponent.AssignLevelOwner() : Start");
		if (EB.Replication.Manager.IsHost && PlayerManager.IsLocalPlayer(player)) // are we the host and is this the local player added?
		{			
			if (null == player.GetComponent<LevelOwnerComponent>())
			{
				//EB.Debug.LogWarning("LevelOwnerComponent.AssignLevelOwner() : player.AddComponent<LevelOwnerComponent>()");
				player.AddComponent<LevelOwnerComponent>(); // add the level owner component to the local player
			}
		}
		//EB.Debug.LogWarning("LevelOwnerComponent.AssignLevelOwner() : End");
	}

#if DEBUG
	public void OnEnable()
	{
		//EB.Debug.LogWarning("LevelOwnerComponent.OnEnable()");
		CacheNetworkDebugger();
	}
#endif

	public void OnDisable()
	{
		//EB.Debug.LogWarning("LevelOwnerComponent.OnDisable()");
	}

	public void OnDestroy()
	{
		//EB.Debug.LogWarning("LevelOwnerComponent.OnDestroy()");
	}

	public void Update()
	{
#if DEBUG
		if (null == _networkDebugger)
		{
			CacheNetworkDebugger();
		}

		if (null != _networkDebugger && !_networkDebugger.turnOnOwnershipTransfer)
		{
			return; // we don't want ownership transfer
		}
#endif

		if (PlayerManager.sPlayerControllers.Count < 2)
		{
			return; // this is only a single player game, no need for any ownership reassignment
		}
		TryTransferOwnership();		
	}

	// look each frame at who should be owner of what
	private static void TryTransferOwnership()
	{
		for (int i = 0; i < _networkOwnershipComponents.Count; ++i)
		{
			NetworkOwnershipComponent netOwner = _networkOwnershipComponents[i];
			if (!netOwner.IsOwnershipTransferAllowed)
			{
				continue; // we don't want to change who owns this object right now
			}

			EnemyController enemy = netOwner.gameObject.GetComponent<EnemyController>();
			if (null != enemy)
			{
				PlayerController bestOwner = CalculateBestOwner(enemy, netOwner);
				if (null != bestOwner && !netOwner.IsOwner(bestOwner.ReplicationPlayer)) // if the best owner is not the current owner
				{
					netOwner.ChangeOwner(bestOwner.ReplicationPlayer);
				}
			}
		}
	}

	// see which player is best suited to own this enemy
	private static PlayerController CalculateBestOwner(EnemyController enemy, NetworkOwnershipComponent netOwner)
	{
		CharacterTargetingComponent enemyTargeting = enemy.gameObject.GetComponent<CharacterTargetingComponent>();

		PlayerController closestPlayer = null;
		float distToClosestPlayerSqr = float.MaxValue;
		for (int controller = 0; controller < PlayerManager.sPlayerControllers.Count; ++controller)
		{
			PlayerController pc = PlayerManager.sPlayerControllers[controller];
			if (pc.gameObject.GetComponent<CharacterTargetingComponent>().AttackTarget == enemy.gameObject) // if this enemy is this player's attack target, then we should be owned by this player
			{
				return pc;
			}

			float distToPlayerSqr = GameUtils.GetDistSqXZ(pc.transform.position, enemy.transform.position);
			if (distToPlayerSqr < distToClosestPlayerSqr)
			{
				distToClosestPlayerSqr = distToPlayerSqr;
				closestPlayer = pc;
			}
		}

		if (null != enemyTargeting.AttackTarget && PlayerManager.IsPlayer(enemyTargeting.AttackTarget))
		{
			return enemyTargeting.AttackTarget.GetComponent<PlayerController>(); // if this player is this enemies attack target, then we should be owned by this player
		}

		if (!netOwner.IsOwner(closestPlayer.ReplicationPlayer)) // if the closest player is not our current owner, we may want to change owners
		{
			EB.Sparx.Player sparxPlayer = netOwner.GetOwner();
			if (sparxPlayer == null)
			{
				return closestPlayer;
			}
			
			PlayerController currentOwner = PlayerManager.GetPlayerController(sparxPlayer);
			float distFromCurrentOwnerSqr = GameUtils.GetDistSqXZ(currentOwner.transform.position, enemy.transform.position);

			// these distances are arbitrary, but are designed so that dithering between different owners should not occur
			const float RequiredDistFromCurrentOwnerSqr = 12f * 12f;
			const float RequiredDistToNewOwnerSqr = 5f * 5f;

			if (distFromCurrentOwnerSqr > RequiredDistFromCurrentOwnerSqr && // if we are far enough away from our current owner
				distToClosestPlayerSqr < RequiredDistToNewOwnerSqr) // we are close enough to our potential new owner
			{
				return closestPlayer;
			}
		}
		return null;
	}

#if DEBUG
	private void CacheNetworkDebugger()
	{
		if (null != Camera.main && null != Camera.main.gameObject)
		{
			_networkDebugger = Camera.main.gameObject.GetComponent<NetworkDebugComponent>();
			if (null == _networkDebugger) // if it doesn't already exist
			{
				_networkDebugger = Camera.main.gameObject.AddComponent<NetworkDebugComponent>(); // add the component
			}
		}
	}
#endif

	//private float lastRandomReassignmentTime = 0;
	//private void RandomReassignment()
	//{
	//	// every x seconds let change who is the owner of enemies
	//	if ((Time.realtimeSinceStartup - lastRandomReassignmentTime) < 6f)
	//	{
	//		return;
	//	}
	//	lastRandomReassignmentTime = Time.realtimeSinceStartup;
//
	//	// get all the EnemyController's
	//	foreach (EnemyController enemy in EnemyManager.sEnemyControllers)
	//	{
	//		NetworkOwnershipComponent network = enemy.GetComponent<NetworkOwnershipComponent>();
//
	//		for (int player = 0; player < PlayerManager.sPlayerControllers.Length; ++player)
		//	{
		//		if (network.IsOwner(PlayerManager.sPlayerControllers[player].ReplicationPlayer))// && PlayerManager.IsLocalPlayer(PlayerManager.sPlayerControllers[player].gameObject)) // this is the current owner of the object
		//		{
		//			// reassign to the next player in the list
		//			network.ChangeOwner(PlayerManager.sPlayerControllers[(player + 1) % PlayerManager.sPlayerControllers.Length].ReplicationPlayer);
		//			break;
		//		}
		//	}
	//	}
	//}
}
