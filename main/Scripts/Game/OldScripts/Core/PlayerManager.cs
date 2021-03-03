///////////////////////////////////////////////////////////////////////
//
//  PlayerManager.cs
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

public class PlayerManager 
{
	private static List<PlayerController> _sPlayerControllers = new List<PlayerController>();

	public static List<PlayerController> sPlayerControllers
	{
		get
		{
			return _sPlayerControllers;
		}
	}

	private static GameObject _localPlayerGameObject = null;
	private static PlayerController _localPlayerController = null;

	public static void RegisterPlayerController(PlayerController player)
	{
		_sPlayerControllers.Add(player);
	}

	public static void UnregisterPlayerController(PlayerController player)
	{
		int index = _sPlayerControllers.IndexOf(player);
		if (index >= 0)
		{
			_sPlayerControllers.RemoveAt(index);
		}
	}

	public static void UnregsiterLocalPlayer()
	{
		if(_localPlayerGameObject != null)
		{
			GameObject.Destroy(_localPlayerGameObject);
			_localPlayerController = null;
			_localPlayerGameObject = null;
		}
	}
	
	public static GameObject LocalPlayerGameObject()
	{
		if ( _localPlayerGameObject != null )
		{
			return _localPlayerGameObject;
		}

		PlayerController pc = LocalPlayerController();
		if (null != pc)
		{
			return pc.gameObject;
		}
		return null;	
	}

	public static PlayerController LocalPlayerController()
	{
		if (_localPlayerController == null)
		{
			if (_localPlayerGameObject != null)
			{
				_localPlayerController = _localPlayerGameObject.GetComponent<PlayerController>();
			}

			for (int i = 0; i < _sPlayerControllers.Count; i++)
			{
				PlayerController pc = _sPlayerControllers[i];
				ReplicationView view = pc.gameObject.GetComponent<ReplicationView>();
				if ((view != null && view.instantiatorPlayer == Replication.LocalPlayer) /*|| !GameStateManager.HasSparxGame*/ || Replication.IsLocalGame)
				{
					_localPlayerController = pc;
					_localPlayerGameObject = _localPlayerController.gameObject;
					break;
				}
			}
		}
		
		return _localPlayerController;
	}

	public static bool IsLocalPlayer( GameObject player )
	{
		return ( LocalPlayerGameObject() == player );
	}

	// is this component on a player?
	public static bool IsPlayer(GameObject obj)
	{
		return obj.layer == LayerMask.NameToLayer("Player");
	}

	public static PlayerController GetPlayerController( EB.Sparx.Player player )
	{
		if (player == null)
		{
			EB.Debug.LogWarning("Player is null in PlayerManager.GetPlayerController");
			return null;
		}
		
		foreach ( PlayerController pc in _sPlayerControllers )
		{
			if ( pc.ReplicationPlayer.PlayerId == player.PlayerId )
			{
				return pc;
			}
		}

		return null;
	}

	public static PlayerController GetPlayerController(long userid)
	{
		foreach (PlayerController pc in _sPlayerControllers)
		{
				if (pc.playerUid==userid)
				{
					return pc;
				}
		}
		return null;
	}

	public static void DestroyPlayerControllers()
	{
		List<PlayerController> players = new List<PlayerController>(_sPlayerControllers.Count);
		foreach (PlayerController pc in _sPlayerControllers)
		{
			players.Add(pc);
		}
		for(int i=0;i<players.Count;i++)
		{
			players[i].Destroy();
		}
	}
}
