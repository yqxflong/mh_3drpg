///////////////////////////////////////////////////////////////////////
//
//  GameCameraParams.cs
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
public class GameCameraParams : CameraParams
{
	public bool isExpanded = false;
	public bool gameCamera = false;
	public bool useTargetLocalSpace = false;
	public float distance = 5f;
	public float pitch = 40f;
	public float lookAtY = 0.0f;
	public float yawOffset = 0f; // offset from default game camera yaw
	public float heightOffset = 0f;
	public bool yawFlipped = false;
    public float clipNear = 0.01f;

	private const string InitialName = "GameCameraParams";

	static public GameCameraParams Create()
	{
		GameCameraParams model = (GameCameraParams)ScriptableObject.CreateInstance<GameCameraParams>();
		model.name = InitialName;
		return model;
	}

	public override CameraParams Clone ()
	{	
		GameCameraParams param = Create ();
		param.name = string.Format("{0}(Clone)", name);
		param.gameCamera = gameCamera;
		param.distance = distance;
		param.pitch = pitch;
		param.yawOffset = yawOffset;
		param.heightOffset = heightOffset;
		param.useTargetLocalSpace = useTargetLocalSpace;
		param.yawFlipped = yawFlipped;
        param.clipNear = clipNear;
		return param;
	}
}
