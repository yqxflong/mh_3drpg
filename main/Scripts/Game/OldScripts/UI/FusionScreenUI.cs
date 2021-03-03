///////////////////////////////////////////////////////////////////////
//
//  FusionScreenUI.cs
//
//  Copyright (c) 2006-2014 KABAM, INC. All rights reserved.
//  This material contains the confidential and proprietary
//  information of Kabam and may not be copied in whole
//  or in part without the express written permission of Kabam.
//  This copyright notice does not imply publication.
//
///////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eScreens
{
	Undefined,
	Inventory,
	ManageSpirits,
	EquipSpirits,
	Any,
	SpiritPowerupResults,
}

public class FusionScreenUI : MonoBehaviour, IStackableUI
{
	public virtual bool EnstackOnCreate	{ get { return true; } }
	public virtual bool ShowUIBlocker { get { return false; } }
	public virtual float BackgroundUIFadeTime { get { return 0.0f; } }
	public virtual bool Visibility { get { return gameObject.activeSelf; } }

	public eScreens screenType;
	public FusionAudio.eEvent audioEvent;
	public bool forceDuckSounds = false;

	private static FusionAudio.eEvent _currentlyPlayingAudio = FusionAudio.eEvent.None;

	public virtual void Show(bool isShowing)
	{
		UIPanel panel = GetComponent<UIPanel>();
		if (panel)
		{
			panel.alpha = isShowing ? 1.0f : 0.0f;
		}
		if (audioEvent != FusionAudio.eEvent.None && isShowing)
		{
			PostAudioEvent(isShowing);
		}
	}

	public virtual void OnFocus()
	{
	}

	public virtual void OnBlur()
	{

	}

	public virtual bool CanAutoBackstack()
	{
		return true;
	}

	public virtual bool IsFullscreen()
	{
		return true;
	}
    
	public virtual bool IsRenderingWorldWhileFullscreen()
	{
		return false;
	}

	public virtual IEnumerator OnAddToStack()
	{
		if (audioEvent != FusionAudio.eEvent.None)
		{
			PostAudioEvent();
		}
		if (audioEvent != FusionAudio.eEvent.None || forceDuckSounds)
		{
			AudioManager.Instance.Duck(AudioManager.eSoundFolders.Ambience | AudioManager.eSoundFolders.Music | AudioManager.eSoundFolders.SFX);
		}
		yield break;
	}

	public virtual IEnumerator OnRemoveFromStack()
	{
		if (audioEvent != FusionAudio.eEvent.None)
		{
			PostAudioEvent(false);
		}
		if (audioEvent != FusionAudio.eEvent.None || forceDuckSounds)
		{
			AudioManager.Instance.UnDuck(AudioManager.eSoundFolders.Ambience | AudioManager.eSoundFolders.Music | AudioManager.eSoundFolders.SFX);
		}
		GameObject.Destroy(gameObject);
		yield break;
	}

	protected void PostAudioEvent(bool play = true)
	{
		if (play)
		{
			if (_currentlyPlayingAudio != audioEvent)
			{
				if (_currentlyPlayingAudio != FusionAudio.eEvent.None)
				{
					FusionAudio.PostEvent(_currentlyPlayingAudio, false);
				}

				_currentlyPlayingAudio = audioEvent;

				if (audioEvent != FusionAudio.eEvent.None)
				{
					FusionAudio.PostEvent(audioEvent);
				}
			}
		}
		else
		{
			if (_currentlyPlayingAudio == audioEvent)
			{
				FusionAudio.PostEvent(audioEvent, false);
				_currentlyPlayingAudio = FusionAudio.eEvent.None;
			}
		}
	}

    public IEnumerator OnPrepareAddToStack()
    {
        yield break;
    }

    public void ClearData()
    {
    }
}
