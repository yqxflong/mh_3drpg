///////////////////////////////////////////////////////////////////////
//
//  GlobalZoneData.cs
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
public class GlobalZoneData : ScriptableObject
{	
	public List<string> tags = new List<string>();
	
	public static GlobalZoneData _instance;
	public static GlobalZoneData Instance
	{
		get 
		{
			return _instance;
		}
	}

	public static void Init(System.Action<bool> fn)
    {
		EB.Assets.LoadAsync("Bundles/DataModels/GlobalZoneData", typeof(GlobalZoneData), o =>
		{
			if(o){
				_instance = o as GlobalZoneData;
			}
			fn(true);
		});
	}
}