///////////////////////////////////////////////////////////////////////
//
//  SceneReference.cs
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

[System.Serializable]
public class SceneReference 
{
	private GameObject _cachedValue = null;
	public GameObject Value
	{
		get
		{
			if (_cachedValue == null && !string.IsNullOrEmpty(scenePath))
			{
				_cachedValue = GameObject.Find(scenePath);
			}
			return _cachedValue;
		}
		set
		{
			_cachedValue = value;
			scenePath = HierarchyUtils.GetGameObjectPath(value);
		}
	}

#if UNITY_EDITOR
	public string ScenePath
	{
		get
		{
			return scenePath;
		}
		set
		{
			if (scenePath != value)
			{
				scenePath = value;
				_cachedValue = GameObject.Find(scenePath);
			}
		}
	}
#endif

	[HideInInspector, SerializeField]
	private string scenePath = "";
}
