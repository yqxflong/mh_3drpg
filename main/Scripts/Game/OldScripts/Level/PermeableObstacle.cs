///////////////////////////////////////////////////////////////////////
//
//  PermeableObstacle.cs
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

public class PermeableObstacle : MonoBehaviour
{	
	private GraphUpdateScene _graphUpdate;
	public GameObject renderOnlyDuringNavGeneration; // this object will only be turned on during nav generation

	public static List<PermeableObstacle> sAllObstacles = new List<PermeableObstacle>();

	public static void SetAllPermeable(bool isPermeable)
	{
		foreach (PermeableObstacle obstacle in sAllObstacles)
		{
			obstacle.SetPermeable(isPermeable);
		}
	}

	public void OnLevelStart(LevelStartEvent evt)
	{
		if (null != renderOnlyDuringNavGeneration)
		{
			renderOnlyDuringNavGeneration.SetActive(false);
		}

		SetPermeable(false);
	}

	public void OnEnable()
	{
		_graphUpdate = gameObject.GetComponent<GraphUpdateScene>() ?? gameObject.AddComponent<GraphUpdateScene>();
		_graphUpdate.modifyWalkability = true;
		sAllObstacles.Add(this);
		EventManager.instance.AddListener<LevelStartEvent>(OnLevelStart);
	}

	public void OnDisable()
	{
		EventManager.instance.RemoveListener<LevelStartEvent>(OnLevelStart);
		sAllObstacles.Remove(this);
		_graphUpdate = null;
	}

	public void SetPermeable(bool isPermeable)
	{
		if (null != _graphUpdate)
		{
			_graphUpdate.setWalkability = isPermeable;
			_graphUpdate.Apply();
		}
	}
}
