///////////////////////////////////////////////////////////////////////
//
//  LevelHelper.cs
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

/// <summary>
/// ����������
/// </summary>
public class LevelHelper : GridHelper
{
	public float navBoundsOffset = 0.0f;

	// all zones which should be excluded from nav mesh generation
	public List<string> zonesExcludedFromNavMesh = new List<string>();

	// are we generating an input mesh for the overworld
	public bool isOverworldNavMeshRequired = false;

	// activate/inactivate objects pre nav mash generation
	static public void ActivateObjectsPreNavMeshGeneration(out List<GameObject> allObjectsActivated, out List<GameObject> allCreated,
															out List<GameObject> allObjectsInactivated, out List<GameObject> zonesDeactivatedDuringNavMeshGeneration)
	{
		allObjectsActivated = FindAllObjectsToRenderDuringNavMeshGeneration();	// all permeable objects must be turned on before the scan				
		foreach (GameObject it in allObjectsActivated)
		{
			it.SetActive(true); // turn them all on
		}

		allCreated = CreateGameObjectsForNavMeshGeneration();

		zonesDeactivatedDuringNavMeshGeneration = new List<GameObject>(); 
		LevelHelper levelHelper = UnityEngine.GameObject.FindObjectOfType(typeof(LevelHelper)) as LevelHelper;
		if (null != levelHelper)
		{
			Transform zonesRoot = levelHelper.transform.Find(ZoneHelper.ZonesRootName);
			foreach (Transform singleZone in zonesRoot)
			{
				if (singleZone.gameObject.activeSelf && levelHelper.IsZoneExcludedFromNavMesh(singleZone.gameObject))
				{
					zonesDeactivatedDuringNavMeshGeneration.Add(singleZone.gameObject);
					singleZone.gameObject.SetActive(false);
				}
			}
		}

#if UNITY_EDITOR
		if (zonesDeactivatedDuringNavMeshGeneration.Count > 0)
		{
			string deactivatedZones = "";
			foreach (GameObject singleZone in zonesDeactivatedDuringNavMeshGeneration)
			{
				deactivatedZones += "\n-" + singleZone.gameObject.name;
			}
			UnityEditor.EditorUtility.DisplayDialog("Zones Deactivated During Nav Mesh Generation", "The following zones will not have nav mesh generated on them: \n" + deactivatedZones, "Ok");
		}
#endif

		allObjectsInactivated = FindAllObjectsToNOTRenderDuringNavMeshGeneration();
		foreach (GameObject it in allObjectsInactivated)
		{
			it.SetActive(false);
		}
	}

	// activate/inactivate objects pre nav mash geenration
	static public void ActivateObjectsPostNavMeshGeneration(List<GameObject> allObjectsActivated, List<GameObject> allCreated, List<GameObject> allObjectsInactivated,
															List<GameObject> zonesDeactivatedDuringNavMeshGeneration)
	{
		foreach (GameObject it in allObjectsInactivated)
		{
			it.SetActive(true); // restore set dressing back to active
		}

		foreach (GameObject it in zonesDeactivatedDuringNavMeshGeneration)
		{
			it.SetActive(true);
		}

		DestroyGameObjectsForNavMeshGeneration(allCreated);

		foreach (GameObject it in allObjectsActivated)
		{
			it.SetActive(false); // turn permeable obstacle off
		}
	}

	// do the navigation scan after turning off all the set dressing, and turning on all the permeable objecsts
	static public void FusionMenuScan(bool ActivateOrDeactivateAppropriateGeo = true)
	{
		if (null == AstarPath.active)
		{
			return;
		}

		List<GameObject> allPermeableObjects = null;	// all permeable objects must be turned on before the scan				
		List<GameObject> allCreated = null;
		List<GameObject> allSetDressing = null;
		List<GameObject> allZonesDeactivatedDuringNavMeshGeneration = null;
		if (ActivateOrDeactivateAppropriateGeo)
		{
			ActivateObjectsPreNavMeshGeneration(out allPermeableObjects, out allCreated, out allSetDressing, out allZonesDeactivatedDuringNavMeshGeneration);
		}
				
#if UNITY_EDITOR
		MenuScan();
#endif

		if (ActivateOrDeactivateAppropriateGeo)
		{
			ActivateObjectsPostNavMeshGeneration(allPermeableObjects, allCreated, allSetDressing, allZonesDeactivatedDuringNavMeshGeneration);
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// copy from AstarPathEditor, because can't access Editor code here
	/// </summary>
	public static void MenuScan()
	{
		if (AstarPath.active == null)
		{
			AstarPath.active = FindObjectOfType(typeof(AstarPath)) as AstarPath;
			if (AstarPath.active == null)
			{
				return;
			}
		}

		if (!Application.isPlaying && (AstarPath.active.astarData.graphs == null || AstarPath.active.astarData.graphTypes == null))
		{
			UnityEditor.EditorUtility.DisplayProgressBar("Scanning", "Deserializing", 0);
			AstarPath.active.astarData.DeserializeGraphs();
		}

		UnityEditor.EditorUtility.DisplayProgressBar("Scanning", "Scanning...", 0);

		try
		{
			OnScanStatus info = progress => UnityEditor.EditorUtility.DisplayProgressBar ("Scanning",progress.description,progress.progress);
			AstarPath.active.ScanLoop(info);
		}
		catch (System.Exception e)
		{
			EB.Debug.LogError("There was an error generating the graphs:\n" + e + "\n\nIf you think this is a bug, please contact me on arongranberg.com (post a comment)\n");
			UnityEditor.EditorUtility.DisplayDialog("Error Generating Graphs", "There was an error when generating graphs, check the console for more info", "Ok");
			throw e;
		}
		finally
		{
			UnityEditor.EditorUtility.ClearProgressBar();
		}
	}
#endif

	static public GameObject FindSetDressing(Transform zone)
	{
		Transform setDressing = zone.Find("SetDressing");
		if (setDressing != null)
		{
			return setDressing.gameObject;
		}
		return null;
	}

	public Transform GetZonesRoot()
	{
		return transform.Find(ZoneHelper.ZonesRootName);
	}

	public void CalculateAllZonesMinMax(out Vector3 outMin, out Vector3 outMax)
	{
		Transform zonesRoot = GetZonesRoot();

		// set min to highest possible x,z initially and max to lowest possible x,z initially (y isn't set)
		outMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		outMax = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);

		Vector3 tempMin = new Vector3();
		Vector3 tempMax = new Vector3();

		// walk zones to generate the bounding box encompassing the entire level (all zones)
		foreach (Transform zone in zonesRoot.transform)
		{
			ZoneDescriptor.CalculateZoneMinAndMax(ref tempMin, ref tempMax, zone);

			outMin.x = tempMin.x < outMin.x ? tempMin.x : outMin.x;
			outMax.x = tempMax.x > outMax.x ? tempMax.x : outMax.x;

			outMin.y = tempMin.y < outMin.y ? tempMin.y : outMin.y;
			outMax.y = tempMax.y > outMax.y ? tempMax.y : outMax.y;

			outMin.z = tempMin.z < outMin.z ? tempMin.z : outMin.z;
			outMax.z = tempMax.z > outMax.z ? tempMax.z : outMax.z;
		}
	}

	private void Awake()
	{
		var mainCamera = GameUtils.GetMainCamera();
		if (mainCamera != null)
		{
			AudioListener listener = mainCamera.GetComponent<AudioListener>();
			if (listener != null)
			{
				Destroy(listener);
			}
		}
	}

	// is the zone passed in excluded from nav mesh builds
	private bool IsZoneExcludedFromNavMesh(GameObject zone)
	{
		foreach (string excludedZone in zonesExcludedFromNavMesh)
		{
			if (zone.name == excludedZone)
			{
				return true;
			}
		}
		return false;
	}

	// get all the objects which must only be rendered during nav mesh generation
	static private List<GameObject> FindAllObjectsToRenderDuringNavMeshGeneration()
	{
		List<GameObject> allPermeableRenderObjects = new List<GameObject>();
		PermeableObstacle[] allPermeableObstacles = GameObject.FindObjectsOfType(typeof(PermeableObstacle)) as PermeableObstacle[];
		foreach (PermeableObstacle obstacle in allPermeableObstacles)
		{
			if (obstacle.renderOnlyDuringNavGeneration != null)
			{
				allPermeableRenderObjects.Add(obstacle.renderOnlyDuringNavGeneration);
			}
		}
		return allPermeableRenderObjects;
	}

	// create game objects which live for the length of the nav mesh generation
	static private List<GameObject> CreateGameObjectsForNavMeshGeneration()
	{		
		List<GameObject> allCreated = new List<GameObject>();

		if (!Application.isPlaying) // this would be too expensive to do at run-time
		{
			LayerMask obstacleLayer = LayerMask.NameToLayer("Obstacle");
			AStarPathfindingRecastCut[] allRecastCuts = GameObject.FindObjectsOfType(typeof(AStarPathfindingRecastCut)) as AStarPathfindingRecastCut[];
			foreach (AStarPathfindingRecastCut cut in allRecastCuts)
			{
				allCreated.Add(cut.CreateTempGameObject(obstacleLayer));
			}

			LayerMask groundLayer = LayerMask.NameToLayer("Ground");
			AStarPathfindingGenerationTimeGeo[] allGenerationTimeGeo = GameObject.FindObjectsOfType(typeof(AStarPathfindingGenerationTimeGeo)) as AStarPathfindingGenerationTimeGeo[];
			foreach (AStarPathfindingGenerationTimeGeo geo in allGenerationTimeGeo)
			{
				GameObject newObject = geo.CreateTempGameObject(groundLayer);
				if (null != newObject)
				{
					allCreated.Add(newObject);
				}
			}
		}
		return allCreated;
	}

	// destroy the game objects which live for the length of the nav mesh generation
	static private void DestroyGameObjectsForNavMeshGeneration(List<GameObject> navMeshGenerationGameObjects)
	{
		foreach (GameObject it in navMeshGenerationGameObjects)
		{
			Object.DestroyImmediate(it);
		}
	}

	// get all the objects which must NOT be rendered during nav mesh generation
	static private List<GameObject> FindAllObjectsToNOTRenderDuringNavMeshGeneration()
	{
		List<GameObject> allSetDressing = new List<GameObject>();
		// 'AStar' object should be a child of 'Level Root', which contains both 'AStar' and 'Zones' as sibling game objects
		Transform zonesRootTrans = null != AstarPath.active.transform.parent ? AstarPath.active.transform.parent.Find(ZoneHelper.ZonesRootName) : null;
		if (null != zonesRootTrans)
		{
			foreach (Transform zone in zonesRootTrans)
			{
				if (!zone.gameObject.activeSelf) // if the zone is not active
				{
					continue; // no need to make sub parts of the zone inactive, as the whole zone is not active
				}

				// disable all set dressing because we don't want to generate pathing
				GameObject setDressing = FindSetDressing(zone);
				if (setDressing != null)
				{
					allSetDressing.Add(setDressing);
				}
			}
		}
		return allSetDressing;
	}
}
