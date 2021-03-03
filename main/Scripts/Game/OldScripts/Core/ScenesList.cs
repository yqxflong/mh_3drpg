///////////////////////////////////////////////////////////////////////
//
//  ScenesList.cs
//
//  Copyright (c) 2006-2013 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
/////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenesList : ScriptableObject
{
	// maintain a list of actual scenes included in a build
	[SerializeField]
	private List<Scene> _scenes = new List<Scene>();
	public List<Scene> Scenes
	{
		get
		{
			return _scenes;
		}
		set
		{
			_scenes = value;
		}
	}
}

[System.Serializable]
public class Scene
{
	public string Name;
	public string DisplayName;
	public string Path;

	public Scene (string name, string displayName, string path)
	{
		Name = name;
		DisplayName = displayName;
		Path = path;
	}
}
