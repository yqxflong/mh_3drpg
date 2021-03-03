#if !UNITY_ANDROID && !UNITY_IPHONE
using UnityEngine;
using System.Collections;

public class PlayVideo: MonoBehaviour
{
	public enum VideoState
	{
		STATE_PLAY,
		STATE_PAUSE,
		STATE_STOP,
	};

	public MovieTexture movTexture = null;

	UITexture _uiTex;
	UITexture uiTex
	{
		get
		{
			if(_uiTex == null)
			{
				_uiTex = GetComponent<UITexture> ();
			}

			return _uiTex;
		}
	}

	VideoState _state = VideoState.STATE_PLAY;

	public VideoState state 
	{
		get { return _state;}

		set 
		{
			_state = value;
			switch (_state) 
			{
			case VideoState.STATE_PLAY:
				if (!movTexture.isPlaying) 
				{
					movTexture.Play ();
				}   
				break;
			case VideoState.STATE_PAUSE:
				movTexture.Pause ();
				break;
			case VideoState.STATE_STOP:
				movTexture.Stop ();
				break;

			}
		}
	}
 
	/// <summary>
	/// Updates the video. Only need to call this when video has been changed.
	/// </summary>
	public void UpdateVideo ()
	{
		if (movTexture != null) 
		{
			if (uiTex != null) 
			{
				uiTex.mainTexture = movTexture;  
			} 

			movTexture.loop = true;
			movTexture.Play ();
		}
	}

}
#endif