///////////////////////////////////////////////////////////////////////
//
//  GameState.cs
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

public enum eGameState
{
	DebugStartScreen,       // Internal.
	Download,               // Internal.
	Start,					// Internal.
	Login,					// Menu.
	LoadGame,               // Menu + Scene
}

public class GameStateAttribute : System.Attribute
{
	public eGameState State
	{
		get;
		set;
	}

	public GameStateAttribute(eGameState state)
	{
		State = state;
	}
}

public abstract class GameState
{
	public static GameStateManager Mgr
	{
		get
		{
			return GameStateManager.Instance;
		}
	}

	public virtual IEnumerator Start(GameState oldState)
	{
		yield break;
	}

	public virtual void Update()
	{
	}

	public virtual void End(GameState newState)
	{
	}

    public virtual void OnPause(bool pauseStatus)
    {
    }

    public virtual void OnFocus(bool focusStatus)
    {
    }

    public static BuildPlatform GetCurrentBuildPlatform()
	{
#if UNITY_IPHONE
		return BuildPlatform.IOS;
#elif UNITY_ANDROID
		return BuildPlatform.Android;
#else
		return BuildPlatform.Standalones;
#endif
	}

	public static BuildPlatform GetRuntimeBuildPlatform()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return BuildPlatform.IOS;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			return BuildPlatform.Android;
		}
		else
		{
			return BuildPlatform.Standalones;
		}
	}

}
