///////////////////////////////////////////////////////////////////////
//
//  TouchController.cs
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

public class TouchController : MonoBehaviour 
{
	//Public Variables
	public static TouchController Instance { get; private set; }
	public List<TouchWrapper> ActiveTouches { 
		get 
		{
			return _activeTouches;
		}
	}

	//Private variables
	private static TouchController _instance;
	private List<TouchWrapper> _activeTouches = new List<TouchWrapper>();

#if UNITY_EDITOR || UNITY_STANDALONE
	private TouchWrapper _mouseTouch = new TouchWrapper();
#endif
	
	private const float _stationaryThreshold = 0.2f;

	void Awake()
	{
		if (Instance != null)
		{
			// VCUtils.DestroyWithError(gameObject, "Only one TouchController can be in a scene!  Destroying the gameObject with this component.");
			EB.Debug.LogError("Only one TouchController can be in a scene!  Destroying the gameObject with this component.");
			Destroy(gameObject);
			return;
		}
		Instance = this;
		
		DontDestroyOnLoad(this);
	}

	void OnDestroy()
	{
		Instance = null;
	}
	
	void Update () 
	{
		_activeTouches.Clear();

#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetMouseButtonDown(0)) 
		{
			_mouseTouch = new TouchWrapper(1, TouchPhase.Began, Input.mousePosition);
		}
		else if (Input.GetMouseButton(0) && _mouseTouch.isActive)
		{
			Vector2 delta = (Vector2)Input.mousePosition - _mouseTouch.position;
			if (Vector2.SqrMagnitude(delta) > _stationaryThreshold * _stationaryThreshold) 
			{
				_mouseTouch = new TouchWrapper(1, TouchPhase.Moved, Input.mousePosition, delta);
			} 
			else 
			{
				_mouseTouch = new TouchWrapper(1, TouchPhase.Stationary, Input.mousePosition, delta);	
			}
		}
		else if (Input.GetMouseButtonUp(0) && _mouseTouch.isActive)
		{
			_mouseTouch = new TouchWrapper(1, TouchPhase.Ended, Input.mousePosition, (Vector2)Input.mousePosition - _mouseTouch.position);
		} 
		else
		{
			_mouseTouch = new TouchWrapper();
		}
		
		if (_mouseTouch.isActive)
		{
			_activeTouches.Add(_mouseTouch);
		}
#else
		for (int i = 0; i < Input.touchCount; ++i)
		{
			_activeTouches.Add(new TouchWrapper(Input.GetTouch(i)));
		}	
#endif
	}
}

public struct TouchWrapper
{
	public int fingerId;
	public Vector2 position;
	public Vector2 deltaPosition;
	public float deltaTime;
	public int tapCount;
	public TouchPhase phase;
	public bool isActive;
	
	public TouchWrapper(Touch touch) 
	{
		fingerId = touch.fingerId;
		position = touch.position;
		deltaPosition = touch.deltaPosition;
		deltaTime = touch.deltaTime;
		tapCount = touch.tapCount;
		phase = touch.phase;
		isActive = true;
	}
	
	public TouchWrapper(int fingerId, TouchPhase phase, Vector2 position) 
	{
		this.fingerId = fingerId;
		this.phase = phase;
		this.position = position;
		this.deltaPosition = new Vector2(0, 0);
		this.deltaTime = Time.deltaTime;
		this.tapCount = 1;
		this.isActive = true;
	}
				
	public TouchWrapper(int fingerId, TouchPhase phase, Vector2 position, Vector2 deltaPosition) 
	{
		this.fingerId = fingerId;
		this.phase = phase;
		this.position = position;
		this.deltaPosition = deltaPosition;
		this.deltaTime = Time.deltaTime;
		this.tapCount = 1;
		this.isActive = true;
	}
}