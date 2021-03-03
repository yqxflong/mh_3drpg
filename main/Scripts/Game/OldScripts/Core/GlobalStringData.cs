///////////////////////////////////////////////////////////////////////
//
//  GlobalStringData.cs
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
public class GlobalStringData : ScriptableObject
{	
	[Parameter("Loading Screen Tips")]
	public List<string> loadingTips;

	public List<string> actNames;

	[Parameter("Inventory Full", "Game Messages")]
	public string inventoryFullText;

	public List<string> generalStringPairs = new List<string>();

	private Dictionary<string, string> _generalStringTable;

	public Dictionary<string, string> GeneralStringTable
	{
		get
		{
			return _generalStringTable;
		}
	}

	public string this[string key]
	{
		get
		{
			if (_generalStringTable.ContainsKey(key))
			{
				return _generalStringTable[key];
			}
			return "";
		}
	}

	public static GlobalStringData _instance;
	public static GlobalStringData Instance
	{
		get 
		{
			return _instance;
		}
	}

	public static void Init(System.Action<bool> fn)
    {
		EB.Assets.LoadAsync("Bundles/DataModels/GlobalStringData", typeof(GlobalStringData), o =>
		{
			if(o){
				_instance = o as GlobalStringData;
				_instance._generalStringTable = new Dictionary<string, string>();
				foreach (string pair in _instance.generalStringPairs)
				{
					string[] split = pair.Split(new string[] { "||" }, System.StringSplitOptions.None);
					string key = (split.Length > 0) ? split[0] : "";
					string value = (split.Length > 1) ? split[1] : "";

					if (string.IsNullOrEmpty(key) || _instance._generalStringTable.ContainsKey(key))
					{
						continue;
					}

					_instance._generalStringTable[key] = value;
				}
				fn(true);
			}
		});
	}
}