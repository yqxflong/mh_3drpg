///////////////////////////////////////////////////////////////////////
//
//  ZoneDescriptor.cs
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

/// <summary>
/// «¯”Ú√Ë ˆ–≈œ¢
/// </summary>
public class ZoneDescriptor : MonoBehaviour 
{
	[System.Flags]
	public enum eZoneExit
	{
		North =	0x01,
		South =	0x02,
		East = 0x04,
		West = 0x08
	};

	[HideInInspector][SerializeField] // inspector rendering handled in ZoneDescriptorEditor
	public eZoneExit zoneExits;
	
	// allow each zone to override its parent (environment) environmentType
	[HideInInspector][SerializeField]
	public bool useParentEnvironmentType = true;
	[HideInInspector][SerializeField]
	public EnvironmentHelper.eEnvironmentType environmentType;

	private Dictionary<int, GameObject> _indexedComponents;

	//Used for debug
	public int RuntimeSocketIndex
	{
		get; set;
	}

	public T FindIndexedComponent<T>(int index) where T : Component
	{
		if (_indexedComponents == null)
		{
			_indexedComponents = IndexedZoneComponent.FindIndexedComponents(transform);
		}
		
		if (_indexedComponents.ContainsKey(index))
		{
			return _indexedComponents[index].GetComponent<T>();
		}
		else
		{
			return null;
		}
	}

	// given a transform representing a zone, gives back the min and max extents of the zone
	public static void CalculateZoneMinAndMax(ref Vector3 refMin, ref Vector3 refMax, Transform zone)
	{
		float gridSizeX = EditorVars.GridSize;
		float gridSizeY = EditorVars.GridSize;
		float gridSizeZ = EditorVars.GridSize;

		GlobalNavHelper nav_helper = zone.GetComponent<GlobalNavHelper>();
		if(nav_helper != null)
		{
			gridSizeX = nav_helper.m_Range.x;
			gridSizeY = nav_helper.m_Range.y;
			gridSizeZ = nav_helper.m_Range.z;
		}

		refMin.x = zone.position.x;
		refMin.y = zone.position.y - (gridSizeY / 2);
		refMin.z = zone.position.z;

		refMax.x = zone.position.x + gridSizeX;
		refMax.y = zone.position.y + (gridSizeY / 2);
		refMax.z = zone.position.z + gridSizeZ;
	}
}

