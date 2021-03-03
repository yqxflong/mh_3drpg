///////////////////////////////////////////////////////////////////////
//
//  LevelDescriptor.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景描述信息
/// </summary>
[ExecuteInEditMode]
public class LevelDescriptor : MonoBehaviour
{
    /// <summary>
    /// 当前场景的所有光照贴图
    /// </summary>
	public LightMapTextures[] levelLightMaps;

	private Transform _zonesTransform;
    private int mZonechildCount = 0;
    private List<Transform> mZoneChildTransform = new List<Transform>();
    private List<GlobalNavHelper> mZoneChildGlobalNavHelper = new List<GlobalNavHelper>();
    private List<ZoneDescriptor> mZoneChildZoneDescriptor = new List<ZoneDescriptor>();

    void Start()
	{
		_zonesTransform = transform.Find(ZoneHelper.ZonesRootName);

        mZonechildCount = _zonesTransform.childCount;
        for (int i = 0; i < mZonechildCount; i++)
        {
            Transform t = _zonesTransform.GetChild(i);
            mZoneChildTransform.Add(t);
            mZoneChildGlobalNavHelper.Add(t.GetComponent<GlobalNavHelper>());
            mZoneChildZoneDescriptor.Add(t.GetComponent<ZoneDescriptor>());
        }

        PopulateZones();

#if UNITY_EDITOR
		if(!Application.isPlaying)
		{
			RestoreLightmapData();
		}
#endif
	}
    
	public ZoneDescriptor GetZoneForPosition(Vector3 position) 
	{
		if (_zonesTransform == null) 
		{
			EB.Debug.LogWarning("Could not find zones under level descriptor. Tracking may not work correctly.");
			return null;
		}

		float x = Mathf.Floor(position.x / 64f) * 64f;
		float z = Mathf.Floor(position.z / 64f) * 64f;
		Vector3 zonePosition = new Vector3(x, 0, z);

		for (int i = 0; i < mZonechildCount; i++)
		{
			Transform t = mZoneChildTransform[i];
            GlobalNavHelper navHelper = mZoneChildGlobalNavHelper[i];

            if (navHelper != null)
			{
				continue;
			}
			if ((t.position - zonePosition).sqrMagnitude < 1f)
			{
				return mZoneChildZoneDescriptor[i];
			}
		}

		return null;
	}
	
	public void PopulateZones()
	{
		EB.Debug.Log ("LevelDescriptor: Populate Zones");

		EventManager.instance.Raise(new PopulateZonesPlayingEvent());
		SetupLightmaps();
	}

	public void SetupLightmaps()
	{
		if (levelLightMaps != null && levelLightMaps.Length > 0)
		{
			LightmapData[] lightmapData = new LightmapData[levelLightMaps.Length];
			for (int i = 0; i < levelLightMaps.Length; i++)
			{
				lightmapData[i] = new LightmapData();
				lightmapData[i].lightmapColor = levelLightMaps[i].lightmapLight;
				//lightmapData[i].lightmapNear = levelLightMaps[i].lightmapDir;
			}
			LightmapSettings.lightmaps = lightmapData;
            //LightmapSettings.lightmapsMode = LightmapsMode.CombinedDirectional;
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        }
	}

	public void RestoreLightmapData()
	{
		LightMapDataComponent[] allLightMapDatas = GetComponentsInChildren<LightMapDataComponent>();
		for(int i = 0; i < allLightMapDatas.Length; i++)
		{
			allLightMapDatas[i].RestoreLightMapData();
		}
	}

	//private void PopulateSpawners(ZoneDescriptor zoneDescriptor, EB.Sparx.SocketData socketData)
	//{
	//	if (socketData == null)
	//		return;

	//	try
	//	{
	//		InteractionSetGroupComponent interactionSetGroup = zoneDescriptor.GetComponentInChildren<InteractionSetGroupComponent>();
	//		InteractionSetComponent selectedInteractionSet = zoneDescriptor.FindIndexedComponent<InteractionSetComponent>(socketData.InteractionSetIndex);

	//		interactionSetGroup.SetInteractionSet(selectedInteractionSet);

	//		foreach (EB.Sparx.SocketData.SpawnerData spawnerData in socketData.Spawners)
	//		{
	//			SpawnerComponent spawner = zoneDescriptor.FindIndexedComponent<SpawnerComponent>(spawnerData.SpawnerIndex);

	//			ServerSpawning serverSpawnLogic = new ServerSpawning();
	//			serverSpawnLogic.LoadFromServer(spawnerData);
	//			spawner.SpawningLogic = serverSpawnLogic;
	//		}

	//		foreach (EB.Sparx.SocketData.LootableData lootableData in socketData.Lootables)
	//		{
	//			TreasureDropComponent treasureDrop = zoneDescriptor.FindIndexedComponent<TreasureDropComponent>(lootableData.LootableIndex);
	//			treasureDrop.PredeterminedDrops = lootableData.GetDropsForPlayer(LoginManager.Instance.LocalUserId.ToString());
	//		}
	//	}
	//	catch (System.Exception e)
	//	{
	//		EB.Debug.LogError("Exception in Populating Spawners: " + e);
	//		EB.Debug.LogError("Server data may be out of sync. Please push ZoneCatalog to Dev server.");
	//	}
	//}
}
