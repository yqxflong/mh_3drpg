///////////////////////////////////////////////////////////////////////
//
//  OpaqueScreenHUDLogic.cs
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

public class OpaqueScreenHUDLogic : MonoBehaviour
{
	public class SpriteFader<T> where T : UIWidget
	{
		private enum eAlphaMode
		{
			fadingToTransparent,
			fadingToOpaque,
			opaque,
			transparent,
		}

		private T _sprite;
		private eAlphaMode _alphaMode = eAlphaMode.transparent;
		private float _startingFadeTime = 0f;
		private float _fadeLength = GameVars.SceneFadeTime;

		public SpriteFader(GameObject obj)
		{
			if (obj)
			{
				_sprite = obj.GetComponent<T>();
			}

			SetActive(false);
		}

		public SpriteFader(T obj)
		{
			_sprite = obj;			
			SetActive(false);
		}

		// Fade the sprite to fully opaque
		public void FadeToOpaque(float time)
		{
			if (time <= float.Epsilon) // imediate
			{
				SetActive(true);
				_alphaMode = eAlphaMode.opaque;						
				_sprite.alpha = 1f;
				return;
			}
			
			switch (_alphaMode)
			{
				case eAlphaMode.fadingToOpaque: // we are already fading to opaque
					_startingFadeTime = CalculateFakeStartTime(_fadeLength, time);
					break;
				case eAlphaMode.fadingToTransparent:
					_alphaMode = eAlphaMode.fadingToOpaque; // we're fading to opaque now
					// here, as we're mid-way through a fade, we're faking that the fade started in the past so we get no jump in alpha value
					_startingFadeTime = CalculateFakeStartTimeForDirectionChange(_fadeLength, time);
					break;
				case eAlphaMode.transparent:
					SetActive(true);
					if (null != _sprite)
					{
						_sprite.alpha = 0f;
					}
					_alphaMode = eAlphaMode.fadingToOpaque; // we're fading to opaque now
					_startingFadeTime = Time.time;
					break;
				default: break;
			}
			_fadeLength = time;
		}

		// Fade the sprite to fully transparent
		public void FadeToTransparent(float time)
		{
			if (time <= float.Epsilon) // imediate
			{
				SetActive(false);
				_alphaMode = eAlphaMode.transparent;
				return;
			}
			
			switch (_alphaMode)
			{
				case eAlphaMode.fadingToTransparent: // we are already fading to opaque
					_startingFadeTime = CalculateFakeStartTime(_fadeLength, time);
					break;
				case eAlphaMode.fadingToOpaque:
					_alphaMode = eAlphaMode.fadingToTransparent; // we're fading to transparent now
					// here, as we're mid-way through a fade, we're faking that the fade started in the past so we get no jump in alpha value					
					_startingFadeTime = CalculateFakeStartTimeForDirectionChange(_fadeLength, time);
					break;
				case eAlphaMode.opaque:
					_alphaMode = eAlphaMode.fadingToTransparent; // we're fading to transparent
					_startingFadeTime = Time.time;
					break;
				default: break;
			}
			_fadeLength = time;
		}

		// how long till we're fully blended in/out
		public float CalculateFadeTimeRemaining()
		{
			switch (_alphaMode)
			{
				case eAlphaMode.fadingToOpaque:
				case eAlphaMode.fadingToTransparent:
					return Mathf.Clamp(_fadeLength - (Time.time - _startingFadeTime), 0f, _fadeLength);
				default: break; // not fading right now
			}
			return 0f; // we are not fading right now
		}

		// is the sprite currently on screen
		public bool IsActive()
		{
			return (null != _sprite && _sprite.gameObject.activeSelf);
		}

		public void Update()
		{
			if (IsActive())
			{
				float newAlpha = ((Time.time - _startingFadeTime) / _fadeLength);
				switch (_alphaMode)
				{
					case eAlphaMode.fadingToTransparent:
						newAlpha = 1f - newAlpha;
						if (newAlpha <= 0f)
						{
							SetActive(false);
							_alphaMode = eAlphaMode.transparent;
						}
						else
						{
							_sprite.alpha = newAlpha;
						}
						break;
					case eAlphaMode.fadingToOpaque:
						if (newAlpha > 1f)
						{
							newAlpha = 1f;
							_alphaMode = eAlphaMode.opaque;
						}
						_sprite.alpha = newAlpha;
						break;
					default: break;
				}
			}
		}

		// fake the starting fade time to avoid alpha jumps if changing directly from a fade to alpha to fade to opaque
		private float CalculateFakeStartTimeForDirectionChange(float currentFadeTime, float newFadeTime)
		{
			float perCentIntoCurrentFade = Mathf.Min((Time.time - _startingFadeTime) / currentFadeTime, 1f);
			float perCentIntoNewFade = (1f - perCentIntoCurrentFade);			
			return Time.time - (perCentIntoNewFade * newFadeTime);
		}

		// fake the starting fade time to avoid alpha jumps if changing fade length
		private float CalculateFakeStartTime(float currentFadeTime, float newFadeTime)
		{
			float currentAlpha = ((Time.time - _startingFadeTime) / currentFadeTime);
			return Time.time - (currentAlpha * newFadeTime);
		}

		// turn the sprite on/off
		private void SetActive(bool active)
		{
			if (null != _sprite && _sprite.gameObject.activeSelf != active)
			{
				NGUITools.SetActive(_sprite.gameObject, active);
			}
		}
	}

	public GameObject screenBlocker;

	private SpriteFader<UISprite> _fader;
	static private OpaqueScreenHUDLogic _instance = null;

	// get the instance of OpaqueScreenHUDLogic, will be null before the HUD is created
	public static OpaqueScreenHUDLogic Instance()
	{
		return _instance;
	}

	// Fade the screen blocker to fully opaque
	public void FadeToOpaque(float time, bool turnOffHUD)
	{
		_fader.FadeToOpaque(time);
		if (turnOffHUD && UIHierarchyHelper.Instance)
		{
			UIHierarchyHelper.Instance.ShowRegularHUD(false, _fader.CalculateFadeTimeRemaining()); // we're going to black screen, so we don't want the HUD
		}
	}

	// Fade the screen blocker to fully transparent
	public void FadeToTransparent(float time, bool turnOnHUD)
	{
		_fader.FadeToTransparent(time);
		if (turnOnHUD && UIHierarchyHelper.Instance)
		{
			UIHierarchyHelper.Instance.ShowRegularHUD(true, _fader.CalculateFadeTimeRemaining()); // we're going back to normal, so we want the HUD
		}
	}

	private void Start()
	{
		_instance = this;
	}

	private void OnEnable()
	{
		_fader = new SpriteFader<UISprite>(screenBlocker);
	}

	private void Update()
	{
		_fader.Update();
	}
}
