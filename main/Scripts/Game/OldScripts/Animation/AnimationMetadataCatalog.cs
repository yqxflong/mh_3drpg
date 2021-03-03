///////////////////////////////////////////////////////////////////////
//
//  AnimationMetadataCatalog.cs
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
public class AnimationMetadata
{
	public string name;
	public float length;

	public float hitTime = -1;
	public float leaveGroundTime = -1;
	public float hitGroundTime = -1;
	public float interruptComboTime = -1;
	public float resetComboTime = -1;
	public float interruptMoveTime = -1;
	public float vfxTime = -1;
	public float soundTime = -1;
	public float footstepTime = -1;

	public float GetEventTime(eAnimationEvent eventType)
	{
		switch (eventType)
		{
			case eAnimationEvent.Hit:
				return hitTime;
			case eAnimationEvent.LeaveGround:
				return leaveGroundTime;
			case eAnimationEvent.HitGround:
				return hitGroundTime;
			case eAnimationEvent.InterruptCombo:
				return interruptComboTime;
			case eAnimationEvent.ResetCombo:
				return resetComboTime;
			case eAnimationEvent.InterruptMove:
				return interruptMoveTime;
			case eAnimationEvent.VFX:
				return vfxTime;
			case eAnimationEvent.Sound:
				return soundTime;
			case eAnimationEvent.Footstep:
				return footstepTime;
			default: 
				return -1;
		}
	}
}

[System.Serializable]
public class AnimationMetadataCatalog : ScriptableObject
{
	[SerializeField]
	private List<AnimationMetadata> _allMetadata = new List<AnimationMetadata>();

	private Dictionary<string, AnimationMetadata> _metadataDictionary;

	private static AnimationMetadataCatalog _instance;
	public static AnimationMetadataCatalog Instance
	{
		get
		{
			return _instance;
		}
	}

	public static void Init(System.Action<bool> fn)
    {
		EB.Assets.LoadAsync("Bundles/DataModels/AnimationMetadata", typeof(AnimationMetadataCatalog), o =>
		{
			if(o){
				_instance = o as AnimationMetadataCatalog;
			}
			fn(true);
		});
	}



	public AnimationMetadata GetAnimationMetadata(string name)
	{
		if (_metadataDictionary == null)
		{
			RebuildDictionary();
		}

		if (_metadataDictionary.ContainsKey(name))
		{
			return _metadataDictionary[name];
		}
		else
		{
			return null;
		}
	}

	public void AddMetadata(AnimationMetadata metadata)
	{
		bool found = false;
		for (int i = 0; i < _allMetadata.Count; i++)
		{
			if (_allMetadata[i].name == metadata.name)
			{
				_allMetadata[i] = metadata;
				found = true;
			}
		}

		if (!found)
		{
			_allMetadata.Add(metadata);
		}
	}

	public void ClearMetadata()
	{
		_allMetadata.Clear();
		_metadataDictionary = null;
	}

	private void RebuildDictionary()
	{
		_metadataDictionary = new Dictionary<string, AnimationMetadata>();
		foreach (AnimationMetadata metadata in _allMetadata)
		{
			if (!_metadataDictionary.ContainsKey(metadata.name)) {
				_metadataDictionary.Add(metadata.name, metadata);
			}
		}
	}
}