///////////////////////////////////////////////////////////////////////
//
//  InteractableComponent.cs
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

public class InteractableComponent : BaseComponent
{
	protected IInteractable _stats;

	public float interactionRange = 2.0f;
	public List<Transform> interactionPoints = new List<Transform>();
	public bool deselectOnInteract = true;
	public bool localOnly = false;
	public bool disableInteraction = false;
	public bool interactOnTap = false;

	public event System.Action<GameObject> onInteract = delegate(GameObject player) {};
	
	private static int _interacableLayer = -1;
	private const string InteractableLayerName = "Interactable";
	public static int GetInteractableLayer()
	{
		if (_interacableLayer < 0)
		{
			_interacableLayer = LayerMask.NameToLayer(InteractableLayerName);
		}
		return _interacableLayer;
	}

	public static bool IsGameObjectInteractable(GameObject obj)
	{
		return (InteractableComponent.GetInteractableLayer() == obj.layer);
	}

	public virtual void Awake()
	{
		if (!gameObject.activeInHierarchy)
			return;
		_stats = (IInteractable)GetComponent(typeof(IInteractable));
		if (_stats == null)
		{
			_stats = gameObject.AddComponent<InteractableStatsComponent>();
		}

		if (GetComponent<Selectable>() == null)
		{
			gameObject.AddComponent<Selectable>();
		}

		if (GetComponent<Collider>() == null)
		{
			DebugSystem.Log("Interactable " + gameObject.name + " does not have a have a collider!", LogType.Error);
			gameObject.SetActive(false);
			return;
		}

		gameObject.layer = LayerMask.NameToLayer(InteractableLayerName);
	}
	public virtual void Interact(GameObject player)
	{
		GlobalUtils.CallStaticHotfixEx("Hotfix_LT.UI.WorldMapPathManager", "Instance","StopPath");//这个还需要存在，不然寻路到NPC处寻路数据未清理
		if (!UIStack.Instance.IsUIBlockingScreen)
		{
			GlobalUtils.CallStaticHotfix("Hotfix_LT.MessengerAdapter", "OnInteractEvent", player,gameObject);
			onInteract(player);
		}
	}
	public Vector3 GetClosestInteractionPoint(Vector3 origin)
	{
		return GetClosestInteractionPointTransform(origin).position;
	}

	public Transform GetClosestInteractionPointTransform(Vector3 origin)
	{
		float minDistance = float.MaxValue;
		Transform closest = transform;
		foreach (Transform t in interactionPoints)
		{
			float xzDistSq = GameUtils.GetDistSqXZ(origin, t.position);
			if (xzDistSq < minDistance)
			{
				minDistance = xzDistSq;
				closest = t;
			}
		}

		return closest;
	}

	public virtual bool IsAlive()
	{
		return !disableInteraction;
	}
}
