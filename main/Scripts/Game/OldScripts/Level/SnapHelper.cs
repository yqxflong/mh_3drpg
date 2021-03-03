///////////////////////////////////////////////////////////////////////
//
//  SnapHelper.cs
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

public class SnapHelper : MonoBehaviour 
{
	public bool useGridSnapping = true;
	public bool useRotationSnapping = true;
	public float snapUnitsX = 1.0f;
	public float snapUnitsY = 1.0f;
	public float snapUnitsZ = 1.0f;
	public float snapYRotationDegrees = 90.0f;
	public bool clampZoneArea = false;
	
	void Awake()
	{
		enabled = false;	// this in only to hold the data, SnapHelperEditor will do the snapping in editor
	}
}
