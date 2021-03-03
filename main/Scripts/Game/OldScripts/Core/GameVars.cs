///////////////////////////////////////////////////////////////////////
//
//  GameVars.cs
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
using EB.Replication;

public class GameVars
{
	public const float CameraFOV = 10.0f;
	
	// hack until we properly implement lives/dungeon attempts
	public static int livesRemaining = 2;
	
	public static Vector3 MainLightPosition = Vector3.zero;
	public static Quaternion MainLightRotation = Quaternion.Euler(25.0f, 340.0f, 180.0f);
	public static float MainLightIntensity = 0.25f;
	public static Color MainLightColor = new Color(1.0f, 1.0f, 1.0f);
	public static Color GlobalAmbient = new Color(0.75f, 0.75f, 0.75f); // 191,191,191

	public static Color FireColor = new Color(230/255f, 93/255f, 4/255f);
	public static Color IceColor = new Color(122/255f, 192/255f, 255/255f);
	public static Color NatureColor = new Color(112/255f, 193/255f, 44/255f);
	public static Color ArcaneColor = new Color(255/255f, 244/255f, 57/255f);
	public static Color VoidColor = new Color(169/255f, 113/255f, 249/255f);

	public const string EmptyFilename = "FolderPlaceholder.txt";
	
	private static bool _paused;
	public static bool paused
	{
		get
		{
			return Replication.IsLocalGame ? _paused : false;
		}
		
		set
		{
			_paused = value;
			
			if (Replication.IsLocalGame)
			{
				Time.timeScale = _paused ? 0.0f : 1.0f;
			}
		}
	}
	
	public const float SceneFadeTime = 1f;
	public const int GridSize = 64;
}
