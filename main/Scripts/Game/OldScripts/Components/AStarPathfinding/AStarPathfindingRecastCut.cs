///////////////////////////////////////////////////////////////////////
//
//  AStarPathfindingRecastCut.cs
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

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AStarPathfindingRecastCut : BaseComponent
{
	public enum Shape
	{
		cube, 
		cylinder
	}

	public Shape shapeType = Shape.cube;

#if UNITY_EDITOR
	[MenuItem("Fusion/Nav Mesh/Create Recast Cube Cut", false, priority: 5000)]
#endif
	public static void InitCube()
	{
		GameObject recastCutObject = new GameObject("Recast Cut Cube");

		AStarPathfindingRecastCut cut = recastCutObject.AddComponent<AStarPathfindingRecastCut>();
		cut.shapeType = AStarPathfindingRecastCut.Shape.cube;

		recastCutObject.AddComponent<BoxCollider>();

		Init(recastCutObject);
	}

#if UNITY_EDITOR
	[MenuItem("Fusion/Nav Mesh/Create Recast Cylinder Cut", false, priority: 5000)]
#endif
	public static void InitCylinder()
	{
		GameObject recastCutObject = new GameObject("Recast Cut Cylinder");

		AStarPathfindingRecastCut cut = recastCutObject.AddComponent<AStarPathfindingRecastCut>();
		cut.shapeType = AStarPathfindingRecastCut.Shape.cylinder;

		recastCutObject.AddComponent<CapsuleCollider>();

		Init(recastCutObject);
	}

	public static void DestroyRecastCutObjects()
	{
		AStarPathfindingRecastCut[] recastObjs = GameObject.FindObjectsOfType(typeof(AStarPathfindingRecastCut)) as AStarPathfindingRecastCut[];
		foreach (AStarPathfindingRecastCut obj in recastObjs)
		{
			Object.DestroyImmediate(obj);
		}
	}

	// create the game object for the nav mesh generation
	public GameObject CreateTempGameObject(LayerMask obstacleLayer)
	{
		EnforceConstraints();

		GameObject tempGameObject = null;
		if (AStarPathfindingRecastCut.Shape.cube == shapeType)
		{
			tempGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tempGameObject.transform.localScale = gameObject.transform.localScale;
		}
		else if (AStarPathfindingRecastCut.Shape.cylinder == shapeType)
		{
			tempGameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			tempGameObject.GetComponent<CapsuleCollider>().height = 1f;

			float maxGroundPlaneAxis = Mathf.Max(gameObject.transform.localScale.x, gameObject.transform.localScale.z);
			tempGameObject.transform.localScale = new Vector3(maxGroundPlaneAxis, gameObject.transform.localScale.y*0.5f, maxGroundPlaneAxis);
		}
		tempGameObject.layer = obstacleLayer;
		tempGameObject.transform.position = gameObject.transform.position;
		tempGameObject.transform.rotation = gameObject.transform.rotation;
		return tempGameObject;
	}

	private static void Init(GameObject recastCutObject)
	{
		if (null != Camera.current)
		{
			// do a ray cast out into the environment to place the cube
			Ray ray = new Ray(Camera.current.transform.position, Camera.current.transform.forward);
			RaycastHit hitInfo;
			bool hasHit = Physics.Raycast(ray, out hitInfo, 2000, 1 << LayerMask.NameToLayer("Ground"));
			if (hasHit)
			{
				recastCutObject.transform.position = hitInfo.point;
			}
		}
#if UNITY_EDITOR
		Selection.activeGameObject = recastCutObject; // select our newly created game object
#endif
	}

	private void OnDrawGizmos()
	{
		EnforceConstraints();
	}

	private void EnforceConstraints()
	{
		const float Height = 1f;
		gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, Height, gameObject.transform.localScale.z);

		CapsuleCollider capsule;
		BoxCollider box;
		switch (shapeType)
		{
			case AStarPathfindingRecastCut.Shape.cube:
				capsule = gameObject.GetComponent<CapsuleCollider>();
				if (null != capsule)
				{
					if (Application.isPlaying)
					{
						Object.Destroy(capsule);
					}
					else
					{
						Object.DestroyImmediate(capsule);
					}
				}
				box = gameObject.GetComponent<BoxCollider>();
				if (null == box)
				{
					box = gameObject.AddComponent<BoxCollider>();
				}
				box.center = Vector3.zero;
				box.size = Vector3.one;
				break;
			case AStarPathfindingRecastCut.Shape.cylinder:
				box = gameObject.GetComponent<BoxCollider>();
				if (null != box)
				{
					if (Application.isPlaying)
					{
						Object.Destroy(box);
					}
					else
					{
						Object.DestroyImmediate(box);
					}
				}
				capsule = gameObject.GetComponent<CapsuleCollider>();
				if (null == capsule)
				{
					capsule = gameObject.AddComponent<CapsuleCollider>();
				}
				capsule.center = Vector3.zero;
				capsule.radius = 0.5f;
				capsule.height = 1f;
				capsule.direction = 1; // y axis
				break;
			default: break;
		}
	}
}
