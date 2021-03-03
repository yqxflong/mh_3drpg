///////////////////////////////////////////////////////////////////////
//
//  AudioManager.cs
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
using System.Collections.Generic;

public class AudioManager
{
	[System.Flags]
	public enum eSoundFolders
	{
		Ambience = 1 << 0,
		Music = 1 << 1,
		SFX = 1 << 2,
		UI = 1 << 3,
		UIMusic = 1 << 4,
        BGM = 1 << 5,
	}

	private static AudioManager _instance = null;
	public static AudioManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new AudioManager();
			}
			return _instance;
		}
	}

	private class SoundFolderData
	{
		public Fabric.Component audioComponent;
		public float DefaultVolume {get; private set;}

		private float _userSetVolume;
		public float UserSetVolume
		{
			get
			{
				return _userSetVolume;
			}
			set
			{
				if (value != _userSetVolume)
				{
					_userSetVolume = value;
					UpdateVolume();
				}
			}
		}

		private float _deltaVolume = 0.0f;
		public float DeltaVolume
		{
			get
			{
				return _deltaVolume;
			}
			set
			{
				if (value != _deltaVolume)
				{
					_deltaVolume = value;
					UpdateVolume();
				}
			}
		}

		public SoundFolderData(Fabric.Component audioComponent)
		{
			this.audioComponent = audioComponent;
			DefaultVolume = audioComponent.Volume;
			_userSetVolume =Mathf.Sqrt(DefaultVolume);
			_deltaVolume = 0.0f;
			UpdateVolume();
		}

		public void ResetUserSettings()
		{
			_userSetVolume = Mathf.Sqrt(DefaultVolume);
            UpdateVolume();
		}

		private void UpdateVolume()
		{
			if (audioComponent != null)
			{
                //audioComponent.Volume = _userSetVolume + _deltaVolume;
                audioComponent.Volume = Mathf.Clamp(_userSetVolume * _userSetVolume, 0,1);
            }
		}
	}

	public eSoundFolders DuckedFolders
	{
		get;
		private set;
	}

	private Dictionary<eSoundFolders, SoundFolderData> _soundFolders = new Dictionary<eSoundFolders, SoundFolderData>();

	private class TweenedFolder
	{
		public SoundFolderData folder;
		public float variationPerSecond;
		public float targetDeltaVolume;
	}

	private List<TweenedFolder> _tweenedFolders = new List<TweenedFolder>();
	private bool _isTweeningRunning = false;

	public static void Initialize()
	{
		if (_instance == null)
		{
			_instance = new AudioManager();
		}
	}

	public void SetUserVolume(eSoundFolders folders, float volume)
	{
		foreach (eSoundFolders folder in _soundFolders.Keys)
		{
			if ((folders & folder) != 0)
			{
				_soundFolders[folder].UserSetVolume = volume;
			}
		}
	}

	public void SetUserVolumeMute(eSoundFolders folders)
	{
		SetUserVolume(folders, 0.0f);
	}

	public void RevertUserVolumeToDefault(eSoundFolders folders)
	{
		foreach (eSoundFolders folder in _soundFolders.Keys)
		{
			if ((folders & folder) != 0)
			{
				_soundFolders[folder].ResetUserSettings();
			}
		}
	}

	public void RevertVolumesToUserSetting()
	{
		foreach (eSoundFolders folder in _soundFolders.Keys)
		{
			_soundFolders[folder].DeltaVolume = 0.0f;
		}
	}

	public void Duck(eSoundFolders folders)
	{
		// It will be -0.8 until someone has a better idea.
		SetDeltaVolume(folders, -1.0f, 1.0f);
		DuckedFolders |= folders;
	}

	public void UnDuck(eSoundFolders folders)
	{
		SetDeltaVolume(folders, 0.0f, 1.0f);
		DuckedFolders = DuckedFolders & (~folders);
	}

	private AudioManager()
	{
		// Find all the sound folders
		InitializeSoundFolders();

		// Initialize the volumes based on user data
		/*if (UserData.IsAmbienceMute)
		{
			SetUserVolumeMute(eSoundFolders.Ambience);
		}*/
		if (UserData.IsMusicMute)
		{
            SetUserVolumeMute(eSoundFolders.Music | eSoundFolders.Ambience| eSoundFolders.BGM);//eSoundFolders.UIMusic);
		}
        else
        {
            SetUserVolume(eSoundFolders.Music | eSoundFolders.Ambience | eSoundFolders.BGM, UserData.MusicVolume);
        }
		if (UserData.IsSfxMute)
		{
			SetUserVolumeMute(eSoundFolders.SFX | eSoundFolders.UI);
		}
        else
        {
            SetUserVolume(eSoundFolders.SFX | eSoundFolders.UI, UserData.SFXVolume);
        }

        AudioListener.volume = UserData.AudioEnabled ? 1.0f : 0.0f;
	}

	private void InitializeSoundFolders()
	{
		eSoundFolders[] enumValues = (eSoundFolders[])System.Enum.GetValues(typeof(eSoundFolders));

		for (int i = 0; i < enumValues.Length; i++)
		{
			MakeSoundFolderData(enumValues[i]);
		}
    }

    private void SetAudioVolume()
    {
        SetDeltaVolume(eSoundFolders.Ambience, UserData .MusicVolume);
        SetDeltaVolume(eSoundFolders.Music, UserData.MusicVolume);
        SetDeltaVolume(eSoundFolders.BGM, UserData.MusicVolume);
        SetDeltaVolume(eSoundFolders.UI, UserData.SFXVolume );
        SetDeltaVolume(eSoundFolders.SFX, UserData.SFXVolume);
    }

	public void SetDeltaVolume(eSoundFolders folders, float Volume)
	{
		foreach (eSoundFolders folder in _soundFolders.Keys)
		{
			if ((folders & folder) != 0)
			{
				_soundFolders[folder].UserSetVolume = Volume;
			}
		}
	}

	public void SetDeltaVolume(eSoundFolders folders, float deltaVolume, float time)
	{
		foreach (eSoundFolders folder in _soundFolders.Keys)
		{
			if ((folders & folder) != 0)
			{
				TweenedFolder tf = _tweenedFolders.Find((TweenedFolder test) => test.folder == _soundFolders[folder]);
				if (tf == null)
				{
					tf = new TweenedFolder();
					tf.folder = _soundFolders[folder];
					_tweenedFolders.Add(tf);
				}
				tf.targetDeltaVolume = deltaVolume;
				tf.variationPerSecond = (deltaVolume - tf.folder.DeltaVolume) / time;
			}
		}

		if (!_isTweeningRunning)
		{
			EB.Coroutines.Run(TweenVolumes());
		}
	}

	private IEnumerator TweenVolumes()
	{
		_isTweeningRunning = true;
		int index = 0;

		while (_tweenedFolders.Count > 0)
		{
			index = 0;

			while (index < _tweenedFolders.Count)
			{
				TweenedFolder current = _tweenedFolders[index];
				current.folder.DeltaVolume += current.variationPerSecond * Time.deltaTime;
				if ((current.variationPerSecond > 0 && current.folder.DeltaVolume > current.targetDeltaVolume) || 
				    (current.variationPerSecond < 0 && current.folder.DeltaVolume < current.targetDeltaVolume))
				{
					current.folder.DeltaVolume = current.targetDeltaVolume;
					_tweenedFolders.Remove(current);
				}
				else
				{
					index++;
				}
			}

			yield return null;
		}

		_isTweeningRunning = false;
		yield break;
	}

	private void MakeSoundFolderData(eSoundFolders folder)
	{
		GameObject folderGo = GameObject.Find(SoundFolderToScenePath(folder));

		if (folderGo == null)
		{
			return;		// EARLY RETURN
		}

		_soundFolders[folder] = new SoundFolderData(folderGo.GetComponent<Fabric.Component>());
	}

	private string SoundFolderToScenePath(eSoundFolders folder)
	{
		switch (folder)
		{
			case eSoundFolders.Ambience:
                return "AudioListener(Clone)/AudioManager/AMB";//"Audio/Ambience";
            case eSoundFolders.Music:
                return "AudioListener(Clone)/AudioManager/MUS";//"Audio/Music";
            case eSoundFolders.SFX:
                return "AudioListener(Clone)/AudioManager/SFX";//"Audio/SFX";
            case eSoundFolders.UI:
                return "AudioListener(Clone)/AudioManager/UI";//"Audio/UI";
			case eSoundFolders.UIMusic:
				return "AudioListener(Clone)/Audio/UIMusic";
            case eSoundFolders.BGM:
                return "AudioListener(Clone)/AudioManager/BGM";
            default:
				return "";
		}
	}
}
