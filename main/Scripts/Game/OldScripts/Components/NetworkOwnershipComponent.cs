///////////////////////////////////////////////////////////////////////
//
//  NetworkOwnershipComponent.cs
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

public class NetworkOwnershipComponent : BaseComponent
{
	private ReplicationView _networkOwnershipComponentViewRPC = null; // networking remote procedure calls

	private bool _isOwnershipTransferAllowed = true;  // can we transfer control right now?

	private const float OwnershipTranferCooldown = 5f; // the min time between control transfers
	private float _lastOwnershipTransferActivity = 0f; // this stops control being transferred to frequently
	private ReplicationView[] allGameObjectViews = null;


	public bool IsLocallyOwned
	{
		get
		{
			return GetNetworkOwnershipComponentViewRPC() == null ? false : _networkOwnershipComponentViewRPC.isMine;
		}
	}

	public bool IsOwnershipTransferAllowed // can we transfer control right now?
	{
		get
		{
			return _isOwnershipTransferAllowed && ((Time.realtimeSinceStartup - _lastOwnershipTransferActivity) > OwnershipTranferCooldown);
		}
	}
    
    // is the passed in game object locally controlled
    public static bool IsGameObjectLocallyOwned(GameObject obj, NetworkOwnershipComponent ownershipCompPassedIn = null)
	{
		if (PlayerManager.IsPlayer(obj))
		{
			return PlayerManager.IsLocalPlayer(obj); // players are local on there local machine
		}
		// everything barring players are local to the host
		return Replication.IsHost;
	}

	public void SetIsOwnershipTransferAllowed(bool allowed)
	{
		_isOwnershipTransferAllowed = allowed;

		if (GetNetworkOwnershipComponentViewRPC() != null)
		{
			_networkOwnershipComponentViewRPC.RPC("SetIsOwnershipTransferAllowedRPC", EB.RPCMode.Others, _isOwnershipTransferAllowed);
		}

		if (!LevelOwnerComponent.IsLevelOwner(PlayerManager.LocalPlayerController().ReplicationPlayer))
		{
			EB.Debug.LogWarning("NetworkOwnershipComponent: SetIsOwnershipTransferAllowed called on a machine other than the level owner has potential for bugs");
		}
	}

	// is the specified sparx player the owner of this object?
	public bool IsOwner(EB.Sparx.Player player)
	{
		if (GetNetworkOwnershipComponentViewRPC() != null && player != null)
		{
			return _networkOwnershipComponentViewRPC.ownerId == player.PlayerId;
		}

		return false;
	}

	public EB.Sparx.Player GetOwner()
	{
		if (GetNetworkOwnershipComponentViewRPC() != null)
		{
			return _networkOwnershipComponentViewRPC.ownerPlayer;
		}

		return null;
	}

	public void ChangeOwner(EB.Sparx.Player newOwner, bool force = false)
	{
		try
		{
			if (!LevelOwnerComponent.IsLevelOwner(PlayerManager.LocalPlayerController().ReplicationPlayer))
			{
				EB.Debug.LogWarning("NetworkOwnershipComponent: ChangeOwner called on a machine other than the level owner has potential for bugs");
			}

			if (IsOwnershipTransferAllowed || force)
			{
				if (IsLocallyOwned)
				{
					RequestNewOwner(newOwner.PlayerId);
				}
				else // transfering ownership can only be done by the machine this object is local to where all data is authorative
				{
					_lastOwnershipTransferActivity = Time.realtimeSinceStartup;

					if (GetNetworkOwnershipComponentViewRPC() != null)
					{
						_networkOwnershipComponentViewRPC.RPC("RequestNewOwnerRPC", EB.RPCMode.Others, newOwner.PlayerId);
					}
				}	
			}
		}
		catch(System.NullReferenceException e)
		{
			EB.Debug.LogError(e.ToString());
		}
	
	}

	public void OnEnable()
	{
		allGameObjectViews = GetComponents<ReplicationView>();
		LevelOwnerComponent.Register(this);
	}

	public void OnDisable()
	{
		LevelOwnerComponent.Unregister(this);
	}

	public void Awake()
	{
		_networkOwnershipComponentViewRPC = FindReplicationViewForComponent<NetworkOwnershipComponent>();
	}

	private ReplicationView GetNetworkOwnershipComponentViewRPC()
	{
		if (_networkOwnershipComponentViewRPC == null)
		{
			Awake();
		}

		return _networkOwnershipComponentViewRPC;
	}

	private void RequestNewOwner(uint newOwnerID)
	{
		if (IsLocallyOwned && !IsOwner(newOwnerID)) // we're local, so we can do what we want
		{
			SetNewOwner(newOwnerID);

			if (GetNetworkOwnershipComponentViewRPC() != null)
			{
				_networkOwnershipComponentViewRPC.RPC("SetNewOwnerRPC", EB.RPCMode.Others, newOwnerID);
			}
		}
	}

	// is the specified sparx player the owner of this object?
	private bool IsOwner(uint playerID)
	{
		if (GetNetworkOwnershipComponentViewRPC() != null)
		{
			return _networkOwnershipComponentViewRPC.ownerId == playerID;
		}

		return false;
	}

	private void SetNewOwner(uint newOwnerID)
	{		
		if (newOwnerID == PlayerManager.LocalPlayerController().ReplicationPlayer.PlayerId) // if the local player is the new owner
		{			
			if (null == allGameObjectViews)
			{
				allGameObjectViews = GetComponents<ReplicationView>();
			}
			foreach (ReplicationView view in allGameObjectViews)
			{
				// if it's marked as host migration, it means it's important that this replication view's ownership be transferred when the owner of the object is transferred
				if (EB.Replication.NetworkMigrationSynchronization.Host == view.migrationSynchronization) 
				{
					EB.Replication.Manager.TransferOwnership(view.viewId);
				}
			}
		}
		_lastOwnershipTransferActivity = Time.realtimeSinceStartup;
	}

	//[RPC]
	private void SetNewOwnerRPC(uint newOwnerID)
	{
		SetNewOwner(newOwnerID);
	}

	//[RPC]
	private void RequestNewOwnerRPC(uint newOwnerID)
	{
		RequestNewOwner(newOwnerID);
	}

	//[RPC]
	private void SetIsOwnershipTransferAllowedRPC(bool doLock)
	{
		_isOwnershipTransferAllowed = doLock;
	}
}
