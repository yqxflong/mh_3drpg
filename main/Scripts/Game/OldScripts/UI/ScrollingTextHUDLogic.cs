///////////////////////////////////////////////////////////////////////
//
//  ScrollingTextHUDLogic.cs
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

public class ScrollingTextHUDLogic : MonoBehaviour
{
	public delegate void ScrollEndDelegate(float completion);
	public const float YScrollTime = 20f;

	public GameObject textGameObject;

	private UILabel _textLabel;
	private OpaqueScreenHUDLogic.SpriteFader<UILabel> _textLabelFader;
	private float _yScreenPosition = 0f;
	private float _speedPerSecond = 1f / YScrollTime;

	private Camera _UICam;
	private UIRoot _root;
	private ScrollEndDelegate _onScrollEnd = null; // a callback for when the scroll ends

	static private ScrollingTextHUDLogic _instance = null;

	// get the instance of ScrollingTextHUDLogic, will be null before the HUD is created
	public static ScrollingTextHUDLogic Instance()
	{
		return _instance;
	}

	// start the text label scrolling with the specified text
	public bool StartScroll(string stringTableKey, float newTime, ScrollEndDelegate callback)
	{		
		if (null == _root)
		{
			_root = NGUITools.FindInParents<UIRoot>(_textLabel.gameObject);
		}

		string text;
		if (null != _root && null != _textLabel && !string.IsNullOrEmpty(stringTableKey) && GlobalStringData.Instance.GeneralStringTable.TryGetValue(stringTableKey, out text))
		{
			if (null != _onScrollEnd)
			{
				_onScrollEnd(CalculateCompletion());
				_onScrollEnd = null;
			}
			_onScrollEnd = callback;

			_textLabel.text = text;
			
			const float Cut = 0f;
			_textLabelFader.FadeToOpaque(Cut);

			_yScreenPosition = 0f;
			_speedPerSecond = 1f/newTime;
			SetTextPosition(_textLabel, _yScreenPosition);
			return true;
		}
		return false;
	}

	// end the text scroll over the course of the fade out time
	public void EndScroll(float fadeOutTime)
	{
		if (null != _textLabelFader)
		{
			_textLabelFader.FadeToTransparent(fadeOutTime);
		}
	}

	public bool IsActive()
	{
		return null != _textLabelFader && _textLabelFader.IsActive();
	}

	public float CalculateCompletionPerCent()
	{
		const float ConvertToPerCent = 100.0f;
		return CalculateCompletion() * ConvertToPerCent;
	}

	private float CalculateCompletionDistance()
	{
		float rootHeightToScreenHeight = (float)((float)Screen.height / (float)_root.minimumHeight);
		float textLabelScreenHeight = (float)_textLabel.height * rootHeightToScreenHeight;
		return (textLabelScreenHeight + (float)Screen.height);
	}

	private float CalculateCompletion()
	{
		if (IsActive())
		{
			return _yScreenPosition / CalculateCompletionDistance();
		}
		return 0f;
	}

	private void Scroll()
	{
		_yScreenPosition += (_speedPerSecond * CalculateCompletionDistance()) * Time.deltaTime;
	}

	private void Start()
	{
		_instance = this;
		_UICam = UICamera.mainCamera;
	}

	private void OnEnable()
	{
		if (null != textGameObject)
		{
			_textLabel = textGameObject.GetComponent<UILabel>();
			_textLabelFader = new OpaqueScreenHUDLogic.SpriteFader<UILabel>(_textLabel);
		}		
	}

	private void Update()
	{
		if (IsActive())
		{
			Scroll();
			SetTextPosition(_textLabel, _yScreenPosition);

			float completion = CalculateCompletion();
			if (completion >= 1f) // if the UILabel has scrolled off the top of the screen
			{
				const float Cut = 0f;
				_textLabelFader.FadeToTransparent(Cut);
			}
			_textLabelFader.Update(); // this may be fading out the label			

			if (!IsActive() && null != _onScrollEnd) // if we have faded out or got to the top of the screen
			{
				_onScrollEnd(completion);
				_onScrollEnd = null;
			}
		}
	}

	// puts the label at the screen position (pivot point of the widget will still effect exactly where the label gets placed)
	private void SetTextPosition(UILabel label, float yScreenPosition)
	{
		if (null == _UICam)
		{
			_UICam = UICamera.mainCamera;

			if (null == _UICam)
			{
				return;
			}
		}

		float screenWidthCenter = Screen.width * 0.5f;
		Vector3 worldPoint = _UICam.ScreenToWorldPoint(new Vector3(screenWidthCenter, yScreenPosition, 0.0f));
		worldPoint.z = 0f;

		transform.position = worldPoint;
	}
}
