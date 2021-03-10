///////////////////////////////////////////////////////////////////////
//
//  EnvironmentHelper.cs
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

/// <summary>
/// 环境光描述信息
/// </summary>
[ExecuteInEditMode]
public class EnvironmentHelper : GridHelper
{	
    /// <summary>
    /// 环境光分类（已经不再使用）
    /// </summary>
	public enum eEnvironmentType
	{
		Unknown,
		GenericIndoors = 10,
		GenericOutdoors,
		Badlands,
		Cave,
		Forest,
		Grasslands,
		Swamp,
		Seashore,
		Mountain,
		Crypt,
		ForestTown,
		Docks,
	}

	[HideInInspector][SerializeField]
	public eEnvironmentType EnvironmentType = eEnvironmentType.Unknown;

	public string DefaultMusicLoopEvent;
	public string DefaultAmbientLoopEvent;

	public string MusicLoopEvent
	{
		get
		{
			if (!string.IsNullOrEmpty(DefaultMusicLoopEvent))
			{
				return DefaultMusicLoopEvent;
			}

			if (GameStateManager.Instance == null)
			{
				return string.Empty;
			}
            
			if (GameFlowControlManager.Instance == null || GameFlowControlManager.Instance.m_StateMachine == null)
			{
				return string.Empty;
			}

			string evt = GameFlowControlManager .Instance.m_StateMachine.FsmVariables.GetFsmString("MusicLoopEvent").Value;
			if (!string.IsNullOrEmpty(evt))
			{
				return evt;
			}

			return string.Empty;
		}
	}

	public string AmbientLoopEvent
	{
		get
		{
			if (!string.IsNullOrEmpty(DefaultAmbientLoopEvent))
			{
				return DefaultAmbientLoopEvent;
			}

			if (GameStateManager.Instance == null)
			{
				return string.Empty;
			}
            
			if (GameFlowControlManager.Instance== null || GameFlowControlManager.Instance.m_StateMachine)
			{
				return string.Empty;
			}

			string evt = GameFlowControlManager.Instance.m_StateMachine.FsmVariables.GetFsmString("AmbientLoopEvent").Value;
			if (!string.IsNullOrEmpty(evt))
			{
				return evt;
			}

			return string.Empty;
		}
	}

#if UNITY_EDITOR
	private bool _navMeshGenerationContinuousTechnique = false;
	public bool NavMeshGenerationContinuousTechnique
	{
		get { return _navMeshGenerationContinuousTechnique; }
		set { _navMeshGenerationContinuousTechnique = value; }
	}

	private bool _navMeshGenerationClampToBounds = true;
	public bool NavMeshGenerationClampToBounds
	{
		get { return _navMeshGenerationClampToBounds; }
		set { _navMeshGenerationClampToBounds = value; }
	}

	private static Transform _activeZone;
	public static Transform ActiveZone
	{
		get { return _activeZone; }
		set { _activeZone = value; }
	}
#endif

	[SerializeField][HideInInspector]
	private string _environmentName;
	public string OutputPath
	{
		get
		{
			string correctedPath = _environmentName.Replace("\\", "/");
			return correctedPath;
		}
		set
		{
			string correctedPath = value.Replace("\\", "/");
			_environmentName = correctedPath;
		}
	}

	[UnityEngine.Serialization.FormerlySerializedAs("_folderName")]
	public string FolderName = string.Empty;
	
	// we want to render or not render all the AStarPathfindingWalkableAreas
	static public void RenderAllAStarPathfindingWalkableAreas(bool turnOn)
	{
		AStarPathfindingWalkableArea[] walkAreas = GameObject.FindObjectsOfType(typeof(AStarPathfindingWalkableArea)) as AStarPathfindingWalkableArea[];
		foreach (AStarPathfindingWalkableArea walk in walkAreas)
		{
			walk.EnableRendererComponent(turnOn);
		}
	}

	void OnEnable()
	{
		if (Application.isPlaying)
		{
			string music = MusicLoopEvent;
			// start music
			if (!string.IsNullOrEmpty(music))
			{
				FusionAudio.PostGlobalMusicEvent(music, true);
			}
			string ambient = AmbientLoopEvent;
			// start ambient
			if (!string.IsNullOrEmpty(ambient))
			{
				FusionAudio.PostAmbientSoundEvent(ambient, true);
			}
		}
	}

	void OnDisable()
	{
		if (Application.isPlaying)
		{
			string music = MusicLoopEvent;
			// stop music
			if (!string.IsNullOrEmpty(music))
			{
				FusionAudio.PostGlobalMusicEvent(music, false);
			}
			string ambient = AmbientLoopEvent;
			// stop ambient
			if (!string.IsNullOrEmpty(ambient))
			{
				FusionAudio.PostAmbientSoundEvent(ambient, false);
			}
		}
	}
}
