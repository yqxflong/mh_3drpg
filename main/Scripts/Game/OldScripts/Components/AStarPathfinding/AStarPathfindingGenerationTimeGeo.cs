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
public class AStarPathfindingGenerationTimeGeo : BaseComponent
{
	public Mesh mesh;

#if UNITY_EDITOR
	[MenuItem("Fusion/Nav Mesh/Create Generation Time Geo", false, priority: 5000)]
#endif
	public static void InitGenerationtimeGeo()
	{
		GameObject generationTimeGeo = new GameObject("Generation Time Geo");
		generationTimeGeo.AddComponent<AStarPathfindingGenerationTimeGeo>();
	}

	void OnDrawGizmos()
	{
		const float StartEndShapeSize = 1f;
		Vector3 start = transform.position - Vector3.up * ((StartEndShapeSize * 2f) + 5f);
		Vector3 dest = transform.position + Vector3.up * ((StartEndShapeSize*2f) + 5f);

		Gizmos.color = Color.white;
		Gizmos.DrawLine(start, dest);
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(start, StartEndShapeSize); // sphere for the start
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(dest, StartEndShapeSize); // sphere for the dest		
	}

	public static void DestroyGenerationTimeGeoObjects()
	{
		AStarPathfindingGenerationTimeGeo[] generationObjs = GameObject.FindObjectsOfType(typeof(AStarPathfindingGenerationTimeGeo)) as AStarPathfindingGenerationTimeGeo[];
		foreach (AStarPathfindingGenerationTimeGeo obj in generationObjs)
		{
			Object.DestroyImmediate(obj);
		}
	}

	// create the game object for the nav mesh generation
	public GameObject CreateTempGameObject(LayerMask obstacleLayer)
	{
		if (null != mesh)
		{
			GameObject tempGameObject = new GameObject("Generation Time Geo DELETE NO CHECK IN");

			MeshFilter filter = tempGameObject.AddComponent<MeshFilter>();
			tempGameObject.AddComponent<MeshRenderer>();

			filter.sharedMesh = mesh;
			
			tempGameObject.layer = obstacleLayer;
			tempGameObject.transform.position = gameObject.transform.position;
			tempGameObject.transform.rotation = gameObject.transform.rotation;
			tempGameObject.transform.localScale = gameObject.transform.localScale;
			return tempGameObject;
		}
		return null;
	}
}
