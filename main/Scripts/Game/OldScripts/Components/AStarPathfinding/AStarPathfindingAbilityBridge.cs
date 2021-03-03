///////////////////////////////////////////////////////////////////////
//
//  AStarPathfindingAbilityBridge.cs
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
using Pathfinding;

[ExecuteInEditMode]
public class AStarPathfindingAbilityBridge : BaseComponent
{
	private static Hashtable _bridgeLinks = null;
	private static List<AStarPathfindingAbilityBridge> _allAbilityBridges = null;
	private static bool _recalculationRequired = false;

#if UNITY_EDITOR && DEBUG
	private GraphNode _startNodePosition = null;
	private GraphNode _destNodePosition = null;
#endif

	// are the two nav meshes linked for ability purposes
	// NOTE: if start == dest, this function will return false
	public static bool IsLinked(uint startRegion, uint destRegion)
	{
		RecalculateAll(); // will only recalculate if it's required, due to nav mesh update etc
		if (null == _bridgeLinks)
		{
			return false;
		}

		HashSet<uint> startLinks = (HashSet<uint>)_bridgeLinks[startRegion];
		if (null != startLinks)
		{
			return startLinks.Contains(destRegion);
		}
		return false;
	}

	// recalculate all the navmesh links
	public static void NavMeshHasBeenUpdated()
	{
		_recalculationRequired = true;
	}

#if UNITY_EDITOR && DEBUG
	void OnDrawGizmos()
	{
		BoxCollider collider = GetComponent<BoxCollider>();

			Vector3 start = CalculateStartLocation(collider);
			Vector3 dest = CalculateDestLocation(collider);

			Gizmos.color = Color.white;
			Gizmos.DrawLine(start, dest);

			if (null != _startNodePosition)
			{
				Gizmos.color = AStarPathfindingDebugger.CalculateNodeDebugColor(_startNodePosition);
			}

			const float StartEndShapeSize = 1f;
			Gizmos.DrawCube(start, new Vector3(StartEndShapeSize, StartEndShapeSize, StartEndShapeSize)); // cube for the start

			if (null != _startNodePosition)
			{
				Gizmos.DrawLine(start, (Vector3)_startNodePosition.position);
			}

			if (null != _destNodePosition)
			{
				Gizmos.color = AStarPathfindingDebugger.CalculateNodeDebugColor(_destNodePosition);
			}
			Gizmos.DrawSphere(dest, StartEndShapeSize); // sphere for the dest

			if (null != _destNodePosition)
			{
				Gizmos.DrawLine(dest, (Vector3)_destNodePosition.position);
			}
	}
#endif

	// recalculate all the navmesh links
	private static void RecalculateAll()
	{
		if (_recalculationRequired)
		{
			_recalculationRequired = false;
			if (null != _allAbilityBridges)
			{
				_bridgeLinks = Johny.HashtablePool.Claim();
				for (int bridge = 0; bridge < _allAbilityBridges.Count; ++bridge)
				{
					_allAbilityBridges[bridge].Recalculate();
				}
			}
		}
	}

	// get the end location of the link
	private Vector3 CalculateDestLocation(BoxCollider collider)
	{
		if (null != collider)
		{
		return transform.position + collider.center + (collider.size.z * transform.localScale.z * 0.5f * transform.forward);
		}
		else
		{
			return transform.position + (transform.localScale.z * 0.5f * transform.forward);
		}
	}

	// get the start location of the link
	private Vector3 CalculateStartLocation(BoxCollider collider)
	{
		if (null != collider)
		{
		return transform.position + collider.center - (collider.size.z * transform.localScale.z * 0.5f * transform.forward);
		}
		else
		{
			return transform.position - (transform.localScale.z * 0.5f * transform.forward);
		}
	}

	private void OnLevelStart(LevelStartEvent evt)
	{
		if (null == _bridgeLinks)
		{
			_bridgeLinks = Johny.HashtablePool.Claim();
			_allAbilityBridges = new List<AStarPathfindingAbilityBridge>();
			_recalculationRequired = false;
		}
		_allAbilityBridges.Add(this);
		Recalculate();
	}

	// recalculate the link start and end nodes
	private void Recalculate()
	{
		BoxCollider collider = GetComponent<BoxCollider>();
			NNInfo startInfo = AstarPath.active.GetNearest(CalculateStartLocation(collider));
			NNInfo destInfo = AstarPath.active.GetNearest(CalculateDestLocation(collider));

			// if either is null, or they're the same, then nothing is being linked to anything else
			if (startInfo.node == null || destInfo.node == null || startInfo.node.Area == destInfo.node.Area)
			{
#if UNITY_EDITOR && DEBUG
				_startNodePosition = _destNodePosition = null;
#endif
				return;
			}

#if UNITY_EDITOR && DEBUG
			_startNodePosition = startInfo.node;
			_destNodePosition = destInfo.node;
#endif
			// add the start links
			HashSet<uint> startLinks = (HashSet<uint>)_bridgeLinks[startInfo.node.Area];
			if (null == startLinks)
			{
				startLinks = new HashSet<uint>();
				_bridgeLinks[startInfo.node.Area] = startLinks;
			}
			startLinks.Add(destInfo.node.Area);

			// add the destination links
			HashSet<uint> destLinks = (HashSet<uint>)_bridgeLinks[destInfo.node.Area];
			if (null == destLinks)
			{
				destLinks = new HashSet<uint>();
				_bridgeLinks[destInfo.node.Area] = destLinks;
			}
			destLinks.Add(startInfo.node.Area);
	
	}

	private void OnEnable()
	{
#if UNITY_EDITOR && DEBUG
		_startNodePosition = _destNodePosition = null;
#endif
		EventManager.instance.AddListener<LevelStartEvent>(OnLevelStart);
	}

	private void OnDisable()
	{
#if UNITY_EDITOR && DEBUG
		_startNodePosition = _destNodePosition = null;
#endif
		EventManager.instance.RemoveListener<LevelStartEvent>(OnLevelStart);
	}

	// assumed here that the level is being destroyed
	private void OnDestroy()
	{
		if (null != _bridgeLinks)
		{
			Johny.HashtablePool.ReleaseRecursion(_bridgeLinks);
			_bridgeLinks = null;
			_allAbilityBridges = null;
		}
#if UNITY_EDITOR && DEBUG
		_startNodePosition = _destNodePosition = null;
#endif
	}
}
