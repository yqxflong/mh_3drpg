///////////////////////////////////////////////////////////////////////
//
//  GlobalCameraData.cs
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
using System;

[System.Serializable]
public class GlobalCameraData : ScriptableObject
{
	public enum GameCameraParameters
	{
		close,
		medium,
		far,
		gameCamera,
	}
	[Parameter("Start Zoom Distance", "Game Cam Default")] //[Range(0f, 1f)]
	public float gameCameraStartZoomDistance = 1f;

	[Parameter("Zoom Speed", "Game Cam Default")] //[Range(0f, 1f)]
	public float gameCameraZoomSpeed = 0.5f;
	[Parameter("Close Pitch", "Game Cam Default")] //[Range(-89f, 89f)]	
	public float gameCameraClosePitch = 40.4f;
	[Parameter("Close Vertical", "Game Cam Default")] //[Range(0f, 200f)]
	public float gameCameraCloseVertical = 6f;
	[Parameter("Close Dist", "Game Cam Default")] //[Range(1f, 200f)]
	public float gameCameraCloseDist = 6f;

	[Parameter("Far Pitch", "Game Cam Default")] //[Range(-89f, 89f)]
	public float gameCameraFarPitch = 42.9f;
	[Parameter("Far Vertical", "Game Cam Default")] //[Range(0f, 200f)]
	public float gameCameraFarVertical = 17.6f;
	[Parameter("Far Dist", "Game Cam Default")] //[Range(1f, 200f)]
	public float gameCameraFarDist = 19.3f;

	[Parameter("Angle Degrees", "Game Cam Default")] //[Range(0f, 359f)]
	public float gameCameraAngleDegrees = 140f;

	[Parameter("Fields of View", "Game Cam Default")]
	public float gameCameraFieldsOfView = 45.0f;

	public List<CameraLerp> cameraLerps = new List<CameraLerp>();
	public List<GameCameraParams> gameCameraParams = new List<GameCameraParams>();
	public List<CinematicCameraParams> cinematicCameraParams = new List<CinematicCameraParams>();

    protected static Dictionary<string, GlobalCameraData> mInitializeCameras = new Dictionary<string, GlobalCameraData>();

    public const string CAMPAIGN_VIEW_CAMERA = "Bundles/DataModels/Camera/CampaignViewCameraData";
	public const string CITY_VIEW_CAMERA = "Bundles/DataModels/Camera/CityViewCameraData";
	public const string MAP_VIEW_CAMERA = "Bundles/DataModels/Camera/MapViewCameraData";
	public const string COMBAT_VIEW_CAMERA = "Bundles/DataModels/Camera/CombatViewCameraData";
	public static string _current_camera = string.Empty;
	public static GlobalCameraData _instance;
	public static GlobalCameraData Instance
	{
		get
		{
			return _instance;
		}
	}

	public static void Init(System.Action<bool> fn)
    {
		LoadCameraData(CITY_VIEW_CAMERA, fn);
	}

	public GameCameraParams FindGameCameraParamsByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		return gameCameraParams.Find(cams => (null != cams && cams.name == name));
	}

	public CameraLerp FindGameCameraLerpByName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		return cameraLerps.Find(cams => (null != cams && cams.name == name));
	}

	public static string GetDataPath()
	{
		return _current_camera;
	}

    public static void LoadCameraData(string path, System.Action<bool> fn = null)
    {
        if (!_current_camera.Equals(path))
        {
			mInitializeCameras.TryGetValue(path, out _instance);
			EB.Assets.LoadAsync(path, typeof(GlobalCameraData), o =>
			{
				if(o){
					_instance = o as GlobalCameraData;
					mInitializeCameras[path] = _instance;
					_current_camera = path;
					if(fn != null)
					{
						fn(true);
					}
				}
			});
        }
        else
        {
			if (fn != null)
			{
				fn(true);
			}
		}
	}
}
