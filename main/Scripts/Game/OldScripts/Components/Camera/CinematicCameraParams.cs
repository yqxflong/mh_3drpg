///////////////////////////////////////////////////////////////////////
//
//  CameraParams.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;

[System.Serializable]
public class CinematicCameraParams : CameraParams
{
	public bool isExpanded = false;
	public float somedummyval = 5f;

	private const string InitialName = "CinematicCameraParams";

	static public CinematicCameraParams Create()
	{
		CinematicCameraParams model = (CinematicCameraParams)ScriptableObject.CreateInstance(typeof(CinematicCameraParams));
		model.name = InitialName;
		return model;
	}
}
