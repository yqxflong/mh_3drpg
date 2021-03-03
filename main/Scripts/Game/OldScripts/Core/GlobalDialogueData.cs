///////////////////////////////////////////////////////////////////////
//
//  GlobalDialogueData.cs
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

[System.Serializable]
public class GlobalDialogueData : ScriptableObject
{
	public CameraLerp defaultDialogueLerp;
	public List<DialogueModel> dialogues = new List<DialogueModel>();


	public static GlobalDialogueData _instance;
	public static GlobalDialogueData Instance
	{
		get 
		{
			return _instance;
		}
	}

	public static void Init(System.Action<bool> fn)
    {
		EB.Assets.LoadAsync(GetDataPath(), typeof(GlobalDialogueData), o =>
		{
			if(o){
				_instance = o as GlobalDialogueData;
				fn(true);
			}
		});
	}

	public static string GetDataPath()
	{
		return "Bundles/DataModels/GlobalDialogueData";
	}
}
